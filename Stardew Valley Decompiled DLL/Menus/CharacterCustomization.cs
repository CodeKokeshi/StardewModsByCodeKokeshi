// Decompiled with JetBrains decompiler
// Type: StardewValley.Menus.CharacterCustomization
// Assembly: Stardew Valley, Version=1.6.15.24356, Culture=neutral, PublicKeyToken=null
// MVID: AA1F513A-94F0-4EF6-A35F-2D5B4A3721BF
// Assembly location: D:\SteamLibrary\steamapps\common\Stardew Valley\Stardew Valley.dll
// XML documentation location: D:\SteamLibrary\steamapps\common\Stardew Valley\Stardew Valley.xml

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using StardewValley.BellsAndWhistles;
using StardewValley.Characters;
using StardewValley.Extensions;
using StardewValley.GameData;
using StardewValley.GameData.Pants;
using StardewValley.GameData.Pets;
using StardewValley.GameData.Shirts;
using StardewValley.Minigames;
using StardewValley.Objects;
using System;
using System.Collections.Generic;

#nullable disable
namespace StardewValley.Menus;

public class CharacterCustomization : IClickableMenu
{
  public const int region_okbutton = 505;
  public const int region_skipIntroButton = 506;
  public const int region_randomButton = 507;
  public const int region_male = 508;
  public const int region_female = 509;
  public const int region_dog = 510;
  public const int region_cat = 511 /*0x01FF*/;
  public const int region_shirtLeft = 512 /*0x0200*/;
  public const int region_shirtRight = 513;
  public const int region_hairLeft = 514;
  public const int region_hairRight = 515;
  public const int region_accLeft = 516;
  public const int region_accRight = 517;
  public const int region_skinLeft = 518;
  public const int region_skinRight = 519;
  public const int region_directionLeft = 520;
  public const int region_directionRight = 521;
  public const int region_cabinsLeft = 621;
  public const int region_cabinsRight = 622;
  public const int region_cabinsClose = 623;
  public const int region_cabinsSeparate = 624;
  public const int region_coopHelp = 625;
  public const int region_coopHelpOK = 626;
  public const int region_difficultyLeft = 627;
  public const int region_difficultyRight = 628;
  public const int region_petLeft = 627;
  public const int region_petRight = 628;
  public const int region_pantsLeft = 629;
  public const int region_pantsRight = 630;
  public const int region_walletsLeft = 631;
  public const int region_walletsRight = 632;
  public const int region_coopHelpRight = 633;
  public const int region_coopHelpLeft = 634;
  public const int region_coopHelpButtons = 635;
  public const int region_advancedOptions = 636;
  public const int region_colorPicker1 = 522;
  public const int region_colorPicker2 = 523;
  public const int region_colorPicker3 = 524;
  public const int region_colorPicker4 = 525;
  public const int region_colorPicker5 = 526;
  public const int region_colorPicker6 = 527;
  public const int region_colorPicker7 = 528;
  public const int region_colorPicker8 = 529;
  public const int region_colorPicker9 = 530;
  public const int region_farmSelection1 = 531;
  public const int region_farmSelection2 = 532;
  public const int region_farmSelection3 = 533;
  public const int region_farmSelection4 = 534;
  public const int region_farmSelection5 = 535;
  public const int region_farmSelection6 = 545;
  public const int region_farmSelection7 = 546;
  public const int region_farmSelection8 = 547;
  public const int region_farmSelection9 = 548;
  public const int region_farmSelection10 = 549;
  public const int region_farmSelection11 = 550;
  public const int region_farmSelection12 = 551;
  public const int region_farmSelectionLeft = 647;
  public const int region_farmSelectionRight = 648;
  public const int region_nameBox = 536;
  public const int region_farmNameBox = 537;
  public const int region_favThingBox = 538;
  public const int colorPickerTimerDelay = 100;
  public const int widthOfMultiplayerArea = 256 /*0x0100*/;
  private int colorPickerTimer;
  public ColorPicker pantsColorPicker;
  public ColorPicker hairColorPicker;
  public ColorPicker eyeColorPicker;
  public List<ClickableComponent> labels = new List<ClickableComponent>();
  public List<ClickableComponent> leftSelectionButtons = new List<ClickableComponent>();
  public List<ClickableComponent> rightSelectionButtons = new List<ClickableComponent>();
  public List<ClickableComponent> genderButtons = new List<ClickableComponent>();
  public List<ClickableTextureComponent> farmTypeButtons = new List<ClickableTextureComponent>();
  public ClickableTextureComponent farmTypeNextPageButton;
  public ClickableTextureComponent farmTypePreviousPageButton;
  private List<string> farmTypeButtonNames = new List<string>();
  private List<string> farmTypeHoverText = new List<string>();
  private List<KeyValuePair<Texture2D, Rectangle>> farmTypeIcons = new List<KeyValuePair<Texture2D, Rectangle>>();
  protected int _currentFarmPage;
  protected int _farmPages;
  public List<ClickableComponent> colorPickerCCs = new List<ClickableComponent>();
  public List<ClickableTextureComponent> cabinLayoutButtons = new List<ClickableTextureComponent>();
  public ClickableTextureComponent okButton;
  public ClickableTextureComponent skipIntroButton;
  public ClickableTextureComponent randomButton;
  public ClickableTextureComponent coopHelpButton;
  public ClickableTextureComponent coopHelpOkButton;
  public ClickableTextureComponent coopHelpRightButton;
  public ClickableTextureComponent coopHelpLeftButton;
  public ClickableTextureComponent advancedOptionsButton;
  private TextBox nameBox;
  private TextBox farmnameBox;
  private TextBox favThingBox;
  private bool skipIntro;
  public bool isModifyingExistingPet;
  public bool showingCoopHelp;
  public int coopHelpScreen;
  public CharacterCustomization.Source source;
  private Vector2 helpStringSize;
  private string hoverText;
  private string hoverTitle;
  private string coopHelpString;
  private string noneString;
  private string normalDiffString;
  private string toughDiffString;
  private string hardDiffString;
  private string superDiffString;
  private string sharedWalletString;
  private string separateWalletString;
  public ClickableComponent nameBoxCC;
  public ClickableComponent farmnameBoxCC;
  public ClickableComponent favThingBoxCC;
  public ClickableComponent backButton;
  private ClickableComponent nameLabel;
  private ClickableComponent farmLabel;
  private ClickableComponent favoriteLabel;
  private ClickableComponent shirtLabel;
  private ClickableComponent skinLabel;
  private ClickableComponent hairLabel;
  private ClickableComponent accLabel;
  private ClickableComponent pantsStyleLabel;
  private ClickableComponent startingCabinsLabel;
  private ClickableComponent cabinLayoutLabel;
  private ClickableComponent separateWalletLabel;
  private ClickableComponent difficultyModifierLabel;
  private ColorPicker _sliderOpTarget;
  private Action _sliderAction;
  private readonly Action _recolorEyesAction;
  private readonly Action _recolorPantsAction;
  private readonly Action _recolorHairAction;
  protected Clothing _itemToDye;
  protected bool _shouldShowBackButton = true;
  protected bool _isDyeMenu;
  protected Farmer _displayFarmer;
  public Rectangle portraitBox;
  public Rectangle? petPortraitBox;
  public string oldName = "";
  private bool _multiplayerServer;
  private float advancedCCHighlightTimer;
  protected List<KeyValuePair<string, string>> _petTypesAndBreeds;
  private ColorPicker lastHeldColorPicker;
  private int timesRandom;

  public CharacterCustomization(Clothing item)
    : this(CharacterCustomization.Source.ClothesDye)
  {
    this._itemToDye = item;
    this.ResetComponents();
    if (this.source == CharacterCustomization.Source.NewGame || this.source == CharacterCustomization.Source.HostNewFarm)
      Game1.spawnMonstersAtNight = false;
    this._recolorPantsAction = (Action) (() => this.DyeItem(this.pantsColorPicker.getSelectedColor()));
    switch (this._itemToDye.clothesType.Value)
    {
      case Clothing.ClothesType.SHIRT:
        this._displayFarmer.Equip<Clothing>(this._itemToDye, this._displayFarmer.shirtItem);
        break;
      case Clothing.ClothesType.PANTS:
        this._displayFarmer.Equip<Clothing>(this._itemToDye, this._displayFarmer.pantsItem);
        break;
    }
    this._displayFarmer.UpdateClothing();
  }

  public void DyeItem(Color color)
  {
    if (this._itemToDye == null)
      return;
    this._itemToDye.Dye(color, 1f);
    this._displayFarmer.FarmerRenderer.MarkSpriteDirty();
  }

  public CharacterCustomization(CharacterCustomization.Source source, bool multiplayerServer = false)
    : base(Game1.uiViewport.Width / 2 - (632 + IClickableMenu.borderWidth * 2) / 2, Game1.uiViewport.Height / 2 - (648 + IClickableMenu.borderWidth * 2) / 2 - 64 /*0x40*/, 632 + IClickableMenu.borderWidth * 2, 648 + IClickableMenu.borderWidth * 2 + 64 /*0x40*/)
  {
    if (source == CharacterCustomization.Source.NewGame || source == CharacterCustomization.Source.HostNewFarm)
    {
      Game1.player.difficultyModifier = 1f;
      Game1.player.team.useSeparateWallets.Value = false;
      Game1.startingCabins = source == CharacterCustomization.Source.HostNewFarm ? 1 : 0;
    }
    this.LoadFarmTypeData();
    this.oldName = Game1.player.Name;
    this._multiplayerServer = multiplayerServer;
    int num = 0;
    if (source == CharacterCustomization.Source.ClothesDye || source == CharacterCustomization.Source.DyePots)
    {
      this._isDyeMenu = true;
      switch (source)
      {
        case CharacterCustomization.Source.ClothesDye:
          num = 1;
          break;
        case CharacterCustomization.Source.DyePots:
          if (Game1.player.CanDyePants())
            ++num;
          if (Game1.player.CanDyeShirt())
          {
            ++num;
            break;
          }
          break;
      }
      this.height = 308 + IClickableMenu.borderWidth * 2 + 64 /*0x40*/ + 72 * num - 4;
      this.xPositionOnScreen = Game1.uiViewport.Width / 2 - this.width / 2;
      this.yPositionOnScreen = Game1.uiViewport.Height / 2 - this.height / 2 - 64 /*0x40*/;
    }
    this.source = source;
    this.ResetComponents();
    this._recolorEyesAction = (Action) (() => Game1.player.changeEyeColor(this.eyeColorPicker.getSelectedColor()));
    this._recolorPantsAction = (Action) (() => Game1.player.changePantsColor(this.pantsColorPicker.getSelectedColor()));
    this._recolorHairAction = (Action) (() => Game1.player.changeHairColor(this.hairColorPicker.getSelectedColor()));
    if (source == CharacterCustomization.Source.DyePots)
    {
      this._recolorHairAction = (Action) (() =>
      {
        if (!Game1.player.CanDyeShirt())
          return;
        Game1.player.shirtItem.Value.clothesColor.Value = this.hairColorPicker.getSelectedColor();
        Game1.player.FarmerRenderer.MarkSpriteDirty();
        this._displayFarmer.FarmerRenderer.MarkSpriteDirty();
      });
      this._recolorPantsAction = (Action) (() =>
      {
        if (!Game1.player.CanDyePants())
          return;
        Game1.player.pantsItem.Value.clothesColor.Value = this.pantsColorPicker.getSelectedColor();
        Game1.player.FarmerRenderer.MarkSpriteDirty();
        this._displayFarmer.FarmerRenderer.MarkSpriteDirty();
      });
      this.favThingBoxCC.visible = false;
      this.nameBoxCC.visible = false;
      this.farmnameBoxCC.visible = false;
      this.favoriteLabel.visible = false;
      this.nameLabel.visible = false;
      this.farmLabel.visible = false;
    }
    this._displayFarmer = this.GetOrCreateDisplayFarmer();
  }

  public Farmer GetOrCreateDisplayFarmer()
  {
    if (this._displayFarmer == null)
    {
      this._displayFarmer = this.source == CharacterCustomization.Source.ClothesDye || this.source == CharacterCustomization.Source.DyePots ? Game1.player.CreateFakeEventFarmer() : Game1.player;
      if (this.source == CharacterCustomization.Source.NewFarmhand)
      {
        if (this._displayFarmer.pants.Value == null)
          this._displayFarmer.pants.Value = this._displayFarmer.GetPantsId();
        if (this._displayFarmer.shirt.Value == null)
          this._displayFarmer.shirt.Value = this._displayFarmer.GetShirtId();
      }
      this._displayFarmer.faceDirection(2);
      this._displayFarmer.FarmerSprite.StopAnimation();
    }
    return this._displayFarmer;
  }

  /// <inheritdoc />
  public override void gameWindowSizeChanged(Rectangle oldBounds, Rectangle newBounds)
  {
    base.gameWindowSizeChanged(oldBounds, newBounds);
    this.ResetComponents();
  }

  public void showAdvancedCharacterCreationHighlight() => this.advancedCCHighlightTimer = 4000f;

  private void ResetComponents()
  {
    if (this._isDyeMenu)
    {
      this.xPositionOnScreen = Game1.uiViewport.Width / 2 - this.width / 2;
      this.yPositionOnScreen = Game1.uiViewport.Height / 2 - this.height / 2 - 64 /*0x40*/;
    }
    else
    {
      this.xPositionOnScreen = Game1.uiViewport.Width / 2 - (632 + IClickableMenu.borderWidth * 2) / 2;
      this.yPositionOnScreen = Game1.uiViewport.Height / 2 - (600 + IClickableMenu.borderWidth * 2) / 2 - 64 /*0x40*/;
    }
    this.colorPickerCCs.Clear();
    if (this.source == CharacterCustomization.Source.ClothesDye && this._itemToDye == null)
      return;
    bool flag1 = this.source == CharacterCustomization.Source.NewGame || this.source == CharacterCustomization.Source.HostNewFarm;
    bool flag2 = this.source != CharacterCustomization.Source.Wizard && this.source != CharacterCustomization.Source.ClothesDye && this.source != CharacterCustomization.Source.DyePots;
    bool flag3 = this.source != CharacterCustomization.Source.ClothesDye && this.source != CharacterCustomization.Source.DyePots;
    this.labels.Clear();
    this.genderButtons.Clear();
    this.cabinLayoutButtons.Clear();
    this.leftSelectionButtons.Clear();
    this.rightSelectionButtons.Clear();
    this.farmTypeButtons.Clear();
    if (flag1)
    {
      ClickableTextureComponent textureComponent = new ClickableTextureComponent("Advanced", new Rectangle(this.xPositionOnScreen - 80 /*0x50*/, this.yPositionOnScreen + this.height - 80 /*0x50*/ - 16 /*0x10*/, 80 /*0x50*/, 80 /*0x50*/), (string) null, (string) null, Game1.mouseCursors2, new Rectangle(154, 154, 20, 20), 4f);
      textureComponent.myID = 636;
      textureComponent.upNeighborID = -99998;
      textureComponent.leftNeighborID = -99998;
      textureComponent.rightNeighborID = -99998;
      textureComponent.downNeighborID = -99998;
      this.advancedOptionsButton = textureComponent;
    }
    else
      this.advancedOptionsButton = (ClickableTextureComponent) null;
    ClickableTextureComponent textureComponent1 = new ClickableTextureComponent("OK", new Rectangle(this.xPositionOnScreen + this.width - IClickableMenu.borderWidth - IClickableMenu.spaceToClearSideBorder - 64 /*0x40*/, this.yPositionOnScreen + this.height - IClickableMenu.borderWidth - IClickableMenu.spaceToClearTopBorder + 16 /*0x10*/, 64 /*0x40*/, 64 /*0x40*/), (string) null, (string) null, Game1.mouseCursors, Game1.getSourceRectForStandardTileSheet(Game1.mouseCursors, 46), 1f);
    textureComponent1.myID = 505;
    textureComponent1.upNeighborID = -99998;
    textureComponent1.leftNeighborID = -99998;
    textureComponent1.rightNeighborID = -99998;
    textureComponent1.downNeighborID = -99998;
    this.okButton = textureComponent1;
    this.backButton = new ClickableComponent(new Rectangle(Game1.uiViewport.Width + -66 * TitleMenu.pixelZoom - 8 * TitleMenu.pixelZoom * 2, Game1.uiViewport.Height - 27 * TitleMenu.pixelZoom - 8 * TitleMenu.pixelZoom, 66 * TitleMenu.pixelZoom, 27 * TitleMenu.pixelZoom), "")
    {
      myID = 81114,
      upNeighborID = -99998,
      leftNeighborID = -99998,
      rightNeighborID = -99998,
      downNeighborID = -99998
    };
    this.nameBox = new TextBox(Game1.content.Load<Texture2D>("LooseSprites\\textBox"), (Texture2D) null, Game1.smallFont, Game1.textColor)
    {
      X = this.xPositionOnScreen + 64 /*0x40*/ + IClickableMenu.spaceToClearSideBorder + IClickableMenu.borderWidth + 256 /*0x0100*/,
      Y = this.yPositionOnScreen + IClickableMenu.borderWidth + IClickableMenu.spaceToClearTopBorder - 16 /*0x10*/,
      Text = Game1.player.Name
    };
    this.nameBoxCC = new ClickableComponent(new Rectangle(this.xPositionOnScreen + 64 /*0x40*/ + IClickableMenu.spaceToClearSideBorder + IClickableMenu.borderWidth + 256 /*0x0100*/, this.yPositionOnScreen + IClickableMenu.borderWidth + IClickableMenu.spaceToClearTopBorder - 16 /*0x10*/, 192 /*0xC0*/, 48 /*0x30*/), "")
    {
      myID = 536,
      upNeighborID = -99998,
      leftNeighborID = -99998,
      rightNeighborID = -99998,
      downNeighborID = -99998
    };
    int num1 = LocalizedContentManager.CurrentLanguageCode == LocalizedContentManager.LanguageCode.ru || LocalizedContentManager.CurrentLanguageCode == LocalizedContentManager.LanguageCode.es || LocalizedContentManager.CurrentLanguageCode == LocalizedContentManager.LanguageCode.pt ? -4 : 0;
    this.labels.Add(this.nameLabel = new ClickableComponent(new Rectangle(this.xPositionOnScreen + num1 + 16 /*0x10*/ + IClickableMenu.spaceToClearSideBorder + IClickableMenu.borderWidth + 192 /*0xC0*/ + 4, this.yPositionOnScreen + IClickableMenu.borderWidth + IClickableMenu.spaceToClearTopBorder - 8, 1, 1), Game1.content.LoadString("Strings\\UI:Character_Name")));
    this.farmnameBox = new TextBox(Game1.content.Load<Texture2D>("LooseSprites\\textBox"), (Texture2D) null, Game1.smallFont, Game1.textColor)
    {
      X = this.xPositionOnScreen + 64 /*0x40*/ + IClickableMenu.spaceToClearSideBorder + IClickableMenu.borderWidth + 256 /*0x0100*/,
      Y = this.yPositionOnScreen + IClickableMenu.borderWidth + IClickableMenu.spaceToClearTopBorder - 16 /*0x10*/ + 64 /*0x40*/,
      Text = Game1.MasterPlayer.farmName.Value
    };
    this.farmnameBoxCC = new ClickableComponent(new Rectangle(this.xPositionOnScreen + 64 /*0x40*/ + IClickableMenu.spaceToClearSideBorder + IClickableMenu.borderWidth + 256 /*0x0100*/, this.yPositionOnScreen + IClickableMenu.borderWidth + IClickableMenu.spaceToClearTopBorder - 16 /*0x10*/ + 64 /*0x40*/, 192 /*0xC0*/, 48 /*0x30*/), "")
    {
      myID = 537,
      upNeighborID = -99998,
      leftNeighborID = -99998,
      rightNeighborID = -99998,
      downNeighborID = -99998
    };
    int num2 = LocalizedContentManager.CurrentLanguageCode == LocalizedContentManager.LanguageCode.ko ? -16 : 0;
    this.labels.Add(this.farmLabel = new ClickableComponent(new Rectangle(this.xPositionOnScreen + num1 * 3 + 16 /*0x10*/ + IClickableMenu.spaceToClearSideBorder + IClickableMenu.borderWidth + 192 /*0xC0*/ + 4 + num2, this.yPositionOnScreen + IClickableMenu.borderWidth + IClickableMenu.spaceToClearTopBorder - 16 /*0x10*/ + 64 /*0x40*/, 1, 1), Game1.content.LoadString("Strings\\UI:Character_Farm")));
    int num3 = LocalizedContentManager.CurrentLanguageCode == LocalizedContentManager.LanguageCode.ko ? 48 /*0x30*/ : 0;
    this.favThingBox = new TextBox(Game1.content.Load<Texture2D>("LooseSprites\\textBox"), (Texture2D) null, Game1.smallFont, Game1.textColor)
    {
      X = this.xPositionOnScreen + 64 /*0x40*/ + IClickableMenu.spaceToClearSideBorder + IClickableMenu.borderWidth + 256 /*0x0100*/ + num3,
      Y = this.yPositionOnScreen + IClickableMenu.borderWidth + IClickableMenu.spaceToClearTopBorder - 16 /*0x10*/ + 128 /*0x80*/,
      Text = Game1.player.favoriteThing.Value
    };
    this.favThingBoxCC = new ClickableComponent(new Rectangle(this.xPositionOnScreen + 64 /*0x40*/ + IClickableMenu.spaceToClearSideBorder + IClickableMenu.borderWidth + 256 /*0x0100*/, this.yPositionOnScreen + IClickableMenu.borderWidth + IClickableMenu.spaceToClearTopBorder - 16 /*0x10*/ + 128 /*0x80*/, 192 /*0xC0*/, 48 /*0x30*/), "")
    {
      myID = 538,
      upNeighborID = -99998,
      leftNeighborID = -99998,
      rightNeighborID = -99998,
      downNeighborID = -99998
    };
    this.labels.Add(this.favoriteLabel = new ClickableComponent(new Rectangle(this.xPositionOnScreen + num1 + 16 /*0x10*/ + IClickableMenu.spaceToClearSideBorder + IClickableMenu.borderWidth + 192 /*0xC0*/ + 4, this.yPositionOnScreen + IClickableMenu.borderWidth + IClickableMenu.spaceToClearTopBorder - 16 /*0x10*/ + 128 /*0x80*/, 1, 1), Game1.content.LoadString("Strings\\UI:Character_FavoriteThing")));
    ClickableTextureComponent textureComponent2 = new ClickableTextureComponent(new Rectangle(this.xPositionOnScreen + 48 /*0x30*/, this.yPositionOnScreen + 64 /*0x40*/ + 56, 40, 40), Game1.mouseCursors, new Rectangle(381, 361, 10, 10), 4f);
    textureComponent2.myID = 507;
    textureComponent2.upNeighborID = -99998;
    textureComponent2.leftNeighborImmutable = true;
    textureComponent2.leftNeighborID = -99998;
    textureComponent2.rightNeighborID = -99998;
    textureComponent2.downNeighborID = -99998;
    this.randomButton = textureComponent2;
    if (this.source == CharacterCustomization.Source.DyePots || this.source == CharacterCustomization.Source.ClothesDye)
      this.randomButton.visible = false;
    this.portraitBox = new Rectangle(this.xPositionOnScreen + 64 /*0x40*/ + 42 - 2, this.yPositionOnScreen + IClickableMenu.borderWidth + IClickableMenu.spaceToClearTopBorder - 16 /*0x10*/, 128 /*0x80*/, 192 /*0xC0*/);
    if (this._isDyeMenu)
    {
      this.portraitBox.X = this.xPositionOnScreen + (this.width - this.portraitBox.Width) / 2;
      this.randomButton.bounds.X = this.portraitBox.X - 56;
    }
    int num4 = 128 /*0x80*/;
    List<ClickableComponent> selectionButtons1 = this.leftSelectionButtons;
    ClickableTextureComponent textureComponent3 = new ClickableTextureComponent("Direction", new Rectangle(this.portraitBox.X - 32 /*0x20*/, this.portraitBox.Y + 144 /*0x90*/, 64 /*0x40*/, 64 /*0x40*/), (string) null, "", Game1.mouseCursors, Game1.getSourceRectForStandardTileSheet(Game1.mouseCursors, 44), 1f);
    textureComponent3.myID = 520;
    textureComponent3.upNeighborID = -99998;
    textureComponent3.leftNeighborID = -99998;
    textureComponent3.leftNeighborImmutable = true;
    textureComponent3.rightNeighborID = -99998;
    textureComponent3.downNeighborID = -99998;
    selectionButtons1.Add((ClickableComponent) textureComponent3);
    List<ClickableComponent> selectionButtons2 = this.rightSelectionButtons;
    ClickableTextureComponent textureComponent4 = new ClickableTextureComponent("Direction", new Rectangle(this.portraitBox.Right - 32 /*0x20*/, this.yPositionOnScreen + IClickableMenu.borderWidth + IClickableMenu.spaceToClearTopBorder + num4, 64 /*0x40*/, 64 /*0x40*/), (string) null, "", Game1.mouseCursors, Game1.getSourceRectForStandardTileSheet(Game1.mouseCursors, 33), 1f);
    textureComponent4.myID = 521;
    textureComponent4.upNeighborID = -99998;
    textureComponent4.leftNeighborID = -99998;
    textureComponent4.rightNeighborID = -99998;
    textureComponent4.downNeighborID = -99998;
    selectionButtons2.Add((ClickableComponent) textureComponent4);
    int num5 = LocalizedContentManager.CurrentLanguageCode == LocalizedContentManager.LanguageCode.ru || LocalizedContentManager.CurrentLanguageCode == LocalizedContentManager.LanguageCode.es || LocalizedContentManager.CurrentLanguageCode == LocalizedContentManager.LanguageCode.pt ? -20 : 0;
    this.isModifyingExistingPet = false;
    if (flag1)
    {
      this.petPortraitBox = new Rectangle?(new Rectangle(this.xPositionOnScreen + IClickableMenu.spaceToClearSideBorder + IClickableMenu.borderWidth + 448 - 16 /*0x10*/ + (LocalizedContentManager.CurrentLanguageCode == LocalizedContentManager.LanguageCode.ru ? 60 : 0), this.yPositionOnScreen + IClickableMenu.borderWidth + IClickableMenu.spaceToClearTopBorder + 192 /*0xC0*/ - 16 /*0x10*/, 64 /*0x40*/, 64 /*0x40*/));
      this.labels.Add(new ClickableComponent(new Rectangle(this.xPositionOnScreen + 16 /*0x10*/ + IClickableMenu.spaceToClearSideBorder + IClickableMenu.borderWidth + 192 /*0xC0*/ + 8 + num1, this.yPositionOnScreen + IClickableMenu.borderWidth + IClickableMenu.spaceToClearTopBorder - 8 + 192 /*0xC0*/, 1, 1), Game1.content.LoadString("Strings\\UI:Character_Animal")));
    }
    if (flag1 || this.source == CharacterCustomization.Source.NewFarmhand || this.source == CharacterCustomization.Source.Wizard)
    {
      List<ClickableComponent> genderButtons1 = this.genderButtons;
      ClickableTextureComponent textureComponent5 = new ClickableTextureComponent("Male", new Rectangle(this.xPositionOnScreen + IClickableMenu.spaceToClearSideBorder + IClickableMenu.borderWidth + 32 /*0x20*/ + 8, this.yPositionOnScreen + IClickableMenu.borderWidth + IClickableMenu.spaceToClearTopBorder + 192 /*0xC0*/, 64 /*0x40*/, 64 /*0x40*/), (string) null, "Male", Game1.mouseCursors, new Rectangle(128 /*0x80*/, 192 /*0xC0*/, 16 /*0x10*/, 16 /*0x10*/), 4f);
      textureComponent5.myID = 508;
      textureComponent5.upNeighborID = -99998;
      textureComponent5.leftNeighborID = -99998;
      textureComponent5.rightNeighborID = -99998;
      textureComponent5.downNeighborID = -99998;
      genderButtons1.Add((ClickableComponent) textureComponent5);
      List<ClickableComponent> genderButtons2 = this.genderButtons;
      ClickableTextureComponent textureComponent6 = new ClickableTextureComponent("Female", new Rectangle(this.xPositionOnScreen + IClickableMenu.spaceToClearSideBorder + IClickableMenu.borderWidth + 32 /*0x20*/ + 64 /*0x40*/ + 24, this.yPositionOnScreen + IClickableMenu.borderWidth + IClickableMenu.spaceToClearTopBorder + 192 /*0xC0*/, 64 /*0x40*/, 64 /*0x40*/), (string) null, "Female", Game1.mouseCursors, new Rectangle(144 /*0x90*/, 192 /*0xC0*/, 16 /*0x10*/, 16 /*0x10*/), 4f);
      textureComponent6.myID = 509;
      textureComponent6.upNeighborID = -99998;
      textureComponent6.leftNeighborID = -99998;
      textureComponent6.rightNeighborID = -99998;
      textureComponent6.downNeighborID = -99998;
      genderButtons2.Add((ClickableComponent) textureComponent6);
      if (this.source == CharacterCustomization.Source.Wizard)
      {
        List<ClickableComponent> genderButtons3 = this.genderButtons;
        // ISSUE: explicit non-virtual call
        if ((genderButtons3 != null ? (__nonvirtual (genderButtons3.Count) > 0 ? 1 : 0) : 0) != 0)
        {
          int num6 = this.xPositionOnScreen + IClickableMenu.spaceToClearSideBorder + IClickableMenu.borderWidth + 320 + 16 /*0x10*/;
          int num7 = this.yPositionOnScreen + IClickableMenu.borderWidth + IClickableMenu.spaceToClearTopBorder + 64 /*0x40*/ + 48 /*0x30*/;
          for (int index = 0; index < this.genderButtons.Count; ++index)
          {
            this.genderButtons[index].bounds.X = num6 + 80 /*0x50*/ * index;
            this.genderButtons[index].bounds.Y = num7;
          }
        }
      }
      num4 = 256 /*0x0100*/;
      if (this.source == CharacterCustomization.Source.Wizard)
        num4 = 192 /*0xC0*/;
      num5 = LocalizedContentManager.CurrentLanguageCode == LocalizedContentManager.LanguageCode.ru || LocalizedContentManager.CurrentLanguageCode == LocalizedContentManager.LanguageCode.es || LocalizedContentManager.CurrentLanguageCode == LocalizedContentManager.LanguageCode.pt || LocalizedContentManager.CurrentLanguageCode == LocalizedContentManager.LanguageCode.fr ? -20 : 0;
      List<ClickableComponent> selectionButtons3 = this.leftSelectionButtons;
      ClickableTextureComponent textureComponent7 = new ClickableTextureComponent("Skin", new Rectangle(this.xPositionOnScreen + IClickableMenu.spaceToClearSideBorder + IClickableMenu.borderWidth + 16 /*0x10*/ + num5, this.yPositionOnScreen + IClickableMenu.borderWidth + IClickableMenu.spaceToClearTopBorder + num4, 64 /*0x40*/, 64 /*0x40*/), (string) null, "", Game1.mouseCursors, Game1.getSourceRectForStandardTileSheet(Game1.mouseCursors, 44), 1f);
      textureComponent7.myID = 518;
      textureComponent7.upNeighborID = -99998;
      textureComponent7.leftNeighborID = -99998;
      textureComponent7.rightNeighborID = -99998;
      textureComponent7.downNeighborID = -99998;
      selectionButtons3.Add((ClickableComponent) textureComponent7);
      this.labels.Add(this.skinLabel = new ClickableComponent(new Rectangle(this.xPositionOnScreen + IClickableMenu.spaceToClearSideBorder + IClickableMenu.borderWidth + 16 /*0x10*/ + 64 /*0x40*/ + 8 + num5 / 2, this.yPositionOnScreen + IClickableMenu.borderWidth + IClickableMenu.spaceToClearTopBorder + num4 + 16 /*0x10*/, 1, 1), Game1.content.LoadString("Strings\\UI:Character_Skin")));
      List<ClickableComponent> selectionButtons4 = this.rightSelectionButtons;
      ClickableTextureComponent textureComponent8 = new ClickableTextureComponent("Skin", new Rectangle(this.xPositionOnScreen + 16 /*0x10*/ + IClickableMenu.spaceToClearSideBorder + IClickableMenu.borderWidth + 128 /*0x80*/, this.yPositionOnScreen + IClickableMenu.borderWidth + IClickableMenu.spaceToClearTopBorder + num4, 64 /*0x40*/, 64 /*0x40*/), (string) null, "", Game1.mouseCursors, Game1.getSourceRectForStandardTileSheet(Game1.mouseCursors, 33), 1f);
      textureComponent8.myID = 519;
      textureComponent8.upNeighborID = -99998;
      textureComponent8.leftNeighborID = -99998;
      textureComponent8.rightNeighborID = -99998;
      textureComponent8.downNeighborID = -99998;
      selectionButtons4.Add((ClickableComponent) textureComponent8);
    }
    if (flag1)
      this.RefreshFarmTypeButtons();
    if (this.source == CharacterCustomization.Source.HostNewFarm)
    {
      this.labels.Add(this.startingCabinsLabel = new ClickableComponent(new Rectangle(this.xPositionOnScreen - 21 - 128 /*0x80*/, this.yPositionOnScreen + IClickableMenu.borderWidth * 2 + 84, 1, 1), Game1.content.LoadString("Strings\\UI:Character_StartingCabins")));
      List<ClickableComponent> selectionButtons5 = this.leftSelectionButtons;
      ClickableTextureComponent textureComponent9 = new ClickableTextureComponent("Cabins", new Rectangle(this.xPositionOnScreen - 256 /*0x0100*/ + IClickableMenu.borderWidth / 2 + 8, this.yPositionOnScreen + IClickableMenu.borderWidth * 2 + 108, 64 /*0x40*/, 64 /*0x40*/), (string) null, "", Game1.mouseCursors, Game1.getSourceRectForStandardTileSheet(Game1.mouseCursors, 44), 1f);
      textureComponent9.myID = 621;
      textureComponent9.upNeighborID = -99998;
      textureComponent9.leftNeighborID = -99998;
      textureComponent9.rightNeighborID = -99998;
      textureComponent9.downNeighborID = -99998;
      selectionButtons5.Add((ClickableComponent) textureComponent9);
      List<ClickableComponent> selectionButtons6 = this.rightSelectionButtons;
      ClickableTextureComponent textureComponent10 = new ClickableTextureComponent("Cabins", new Rectangle(this.xPositionOnScreen - 256 /*0x0100*/ + IClickableMenu.borderWidth + 128 /*0x80*/ + 8, this.yPositionOnScreen + IClickableMenu.borderWidth * 2 + 108, 64 /*0x40*/, 64 /*0x40*/), (string) null, "", Game1.mouseCursors, Game1.getSourceRectForStandardTileSheet(Game1.mouseCursors, 33), 1f);
      textureComponent10.myID = 622;
      textureComponent10.upNeighborID = -99998;
      textureComponent10.leftNeighborID = -99998;
      textureComponent10.rightNeighborID = -99998;
      textureComponent10.downNeighborID = -99998;
      selectionButtons6.Add((ClickableComponent) textureComponent10);
      this.labels.Add(this.cabinLayoutLabel = new ClickableComponent(new Rectangle(this.xPositionOnScreen - 128 /*0x80*/ - (int) ((double) Game1.smallFont.MeasureString(Game1.content.LoadString("Strings\\UI:Character_CabinLayout")).X / 2.0), this.yPositionOnScreen + IClickableMenu.borderWidth * 2 + 120 + 64 /*0x40*/, 1, 1), Game1.content.LoadString("Strings\\UI:Character_CabinLayout")));
      List<ClickableTextureComponent> cabinLayoutButtons1 = this.cabinLayoutButtons;
      ClickableTextureComponent textureComponent11 = new ClickableTextureComponent("Close", new Rectangle(this.xPositionOnScreen - 256 /*0x0100*/ + IClickableMenu.borderWidth, this.yPositionOnScreen + IClickableMenu.borderWidth * 2 + 160 /*0xA0*/ + 64 /*0x40*/, 64 /*0x40*/, 64 /*0x40*/), (string) null, Game1.content.LoadString("Strings\\UI:Character_Close"), Game1.mouseCursors, new Rectangle(208 /*0xD0*/, 192 /*0xC0*/, 16 /*0x10*/, 16 /*0x10*/), 4f);
      textureComponent11.myID = 623;
      textureComponent11.upNeighborID = -99998;
      textureComponent11.leftNeighborID = -99998;
      textureComponent11.rightNeighborID = -99998;
      textureComponent11.downNeighborID = -99998;
      cabinLayoutButtons1.Add(textureComponent11);
      List<ClickableTextureComponent> cabinLayoutButtons2 = this.cabinLayoutButtons;
      ClickableTextureComponent textureComponent12 = new ClickableTextureComponent("Separate", new Rectangle(this.xPositionOnScreen - 256 /*0x0100*/ + IClickableMenu.borderWidth + 128 /*0x80*/ - 8, this.yPositionOnScreen + IClickableMenu.borderWidth * 2 + 160 /*0xA0*/ + 64 /*0x40*/, 64 /*0x40*/, 64 /*0x40*/), (string) null, Game1.content.LoadString("Strings\\UI:Character_Separate"), Game1.mouseCursors, new Rectangle(224 /*0xE0*/, 192 /*0xC0*/, 16 /*0x10*/, 16 /*0x10*/), 4f);
      textureComponent12.myID = 624;
      textureComponent12.upNeighborID = -99998;
      textureComponent12.leftNeighborID = -99998;
      textureComponent12.rightNeighborID = -99998;
      textureComponent12.downNeighborID = -99998;
      cabinLayoutButtons2.Add(textureComponent12);
      this.labels.Add(this.difficultyModifierLabel = new ClickableComponent(new Rectangle(this.xPositionOnScreen - 21 - 128 /*0x80*/, this.yPositionOnScreen + IClickableMenu.borderWidth * 2 + 256 /*0x0100*/ + 56, 1, 1), Game1.content.LoadString("Strings\\UI:Character_Difficulty")));
      List<ClickableComponent> selectionButtons7 = this.leftSelectionButtons;
      ClickableTextureComponent textureComponent13 = new ClickableTextureComponent("Difficulty", new Rectangle(this.xPositionOnScreen - 256 /*0x0100*/ + IClickableMenu.borderWidth / 2 - 4, this.yPositionOnScreen + IClickableMenu.borderWidth * 2 + 256 /*0x0100*/ + 80 /*0x50*/, 64 /*0x40*/, 64 /*0x40*/), (string) null, "", Game1.mouseCursors, Game1.getSourceRectForStandardTileSheet(Game1.mouseCursors, 44), 1f);
      textureComponent13.myID = 627;
      textureComponent13.upNeighborID = -99998;
      textureComponent13.leftNeighborID = -99998;
      textureComponent13.rightNeighborID = -99998;
      textureComponent13.downNeighborID = -99998;
      selectionButtons7.Add((ClickableComponent) textureComponent13);
      List<ClickableComponent> selectionButtons8 = this.rightSelectionButtons;
      ClickableTextureComponent textureComponent14 = new ClickableTextureComponent("Difficulty", new Rectangle(this.xPositionOnScreen - 256 /*0x0100*/ + IClickableMenu.borderWidth + 128 /*0x80*/ + 12, this.yPositionOnScreen + IClickableMenu.borderWidth * 2 + 256 /*0x0100*/ + 80 /*0x50*/, 64 /*0x40*/, 64 /*0x40*/), (string) null, "", Game1.mouseCursors, Game1.getSourceRectForStandardTileSheet(Game1.mouseCursors, 33), 1f);
      textureComponent14.myID = 628;
      textureComponent14.upNeighborID = -99998;
      textureComponent14.leftNeighborID = -99998;
      textureComponent14.rightNeighborID = -99998;
      textureComponent14.downNeighborID = -99998;
      selectionButtons8.Add((ClickableComponent) textureComponent14);
      int y = this.yPositionOnScreen + IClickableMenu.borderWidth * 2 + 320 + 100;
      this.labels.Add(this.separateWalletLabel = new ClickableComponent(new Rectangle(this.xPositionOnScreen - 21 - 128 /*0x80*/, y - 24, 1, 1), Game1.content.LoadString("Strings\\UI:Character_Wallets")));
      List<ClickableComponent> selectionButtons9 = this.leftSelectionButtons;
      ClickableTextureComponent textureComponent15 = new ClickableTextureComponent("Wallets", new Rectangle(this.xPositionOnScreen - 256 /*0x0100*/ + IClickableMenu.borderWidth / 2 - 4, y, 64 /*0x40*/, 64 /*0x40*/), (string) null, "", Game1.mouseCursors, Game1.getSourceRectForStandardTileSheet(Game1.mouseCursors, 44), 1f);
      textureComponent15.myID = 631;
      textureComponent15.upNeighborID = -99998;
      textureComponent15.leftNeighborID = -99998;
      textureComponent15.rightNeighborID = -99998;
      textureComponent15.downNeighborID = -99998;
      selectionButtons9.Add((ClickableComponent) textureComponent15);
      List<ClickableComponent> selectionButtons10 = this.rightSelectionButtons;
      ClickableTextureComponent textureComponent16 = new ClickableTextureComponent("Wallets", new Rectangle(this.xPositionOnScreen - 256 /*0x0100*/ + IClickableMenu.borderWidth + 128 /*0x80*/ + 12, y, 64 /*0x40*/, 64 /*0x40*/), (string) null, "", Game1.mouseCursors, Game1.getSourceRectForStandardTileSheet(Game1.mouseCursors, 33), 1f);
      textureComponent16.myID = 632;
      textureComponent16.upNeighborID = -99998;
      textureComponent16.leftNeighborID = -99998;
      textureComponent16.rightNeighborID = -99998;
      textureComponent16.downNeighborID = -99998;
      selectionButtons10.Add((ClickableComponent) textureComponent16);
      ClickableTextureComponent textureComponent17 = new ClickableTextureComponent("CoopHelp", new Rectangle(this.xPositionOnScreen - 256 /*0x0100*/ + IClickableMenu.borderWidth + 128 /*0x80*/ - 8, this.yPositionOnScreen + IClickableMenu.borderWidth * 2 + 448 + 40, 64 /*0x40*/, 64 /*0x40*/), (string) null, Game1.content.LoadString("Strings\\UI:Character_CoopHelp"), Game1.mouseCursors, new Rectangle(240 /*0xF0*/, 192 /*0xC0*/, 16 /*0x10*/, 16 /*0x10*/), 4f);
      textureComponent17.myID = 625;
      textureComponent17.upNeighborID = -99998;
      textureComponent17.leftNeighborID = -99998;
      textureComponent17.rightNeighborID = -99998;
      textureComponent17.downNeighborID = -99998;
      this.coopHelpButton = textureComponent17;
      ClickableTextureComponent textureComponent18 = new ClickableTextureComponent("CoopHelpOK", new Rectangle(this.xPositionOnScreen - 256 /*0x0100*/ - 12, this.yPositionOnScreen + this.height - 64 /*0x40*/, 64 /*0x40*/, 64 /*0x40*/), (string) null, (string) null, Game1.mouseCursors, Game1.getSourceRectForStandardTileSheet(Game1.mouseCursors, 46), 1f);
      textureComponent18.myID = 626;
      textureComponent18.region = 635;
      textureComponent18.upNeighborID = -99998;
      textureComponent18.leftNeighborID = -99998;
      textureComponent18.rightNeighborID = -99998;
      textureComponent18.downNeighborID = -99998;
      this.coopHelpOkButton = textureComponent18;
      this.noneString = Game1.content.LoadString("Strings\\UI:Character_none");
      this.normalDiffString = Game1.content.LoadString("Strings\\UI:Character_Normal");
      this.toughDiffString = Game1.content.LoadString("Strings\\UI:Character_Tough");
      this.hardDiffString = Game1.content.LoadString("Strings\\UI:Character_Hard");
      this.superDiffString = Game1.content.LoadString("Strings\\UI:Character_Super");
      this.separateWalletString = Game1.content.LoadString("Strings\\UI:Character_SeparateWallet");
      this.sharedWalletString = Game1.content.LoadString("Strings\\UI:Character_SharedWallet");
      ClickableTextureComponent textureComponent19 = new ClickableTextureComponent("CoopHelpRight", new Rectangle(this.xPositionOnScreen + this.width, this.yPositionOnScreen + this.height, 64 /*0x40*/, 64 /*0x40*/), (string) null, "", Game1.mouseCursors, Game1.getSourceRectForStandardTileSheet(Game1.mouseCursors, 33), 1f);
      textureComponent19.myID = 633;
      textureComponent19.region = 635;
      textureComponent19.upNeighborID = -99998;
      textureComponent19.leftNeighborID = -99998;
      textureComponent19.rightNeighborID = -99998;
      textureComponent19.downNeighborID = -99998;
      this.coopHelpRightButton = textureComponent19;
      ClickableTextureComponent textureComponent20 = new ClickableTextureComponent("CoopHelpLeft", new Rectangle(this.xPositionOnScreen, this.yPositionOnScreen + this.height, 64 /*0x40*/, 64 /*0x40*/), (string) null, "", Game1.mouseCursors, Game1.getSourceRectForStandardTileSheet(Game1.mouseCursors, 44), 1f);
      textureComponent20.myID = 634;
      textureComponent20.region = 635;
      textureComponent20.upNeighborID = -99998;
      textureComponent20.leftNeighborID = -99998;
      textureComponent20.rightNeighborID = -99998;
      textureComponent20.downNeighborID = -99998;
      this.coopHelpLeftButton = textureComponent20;
    }
    Point point = new Point(this.xPositionOnScreen + IClickableMenu.spaceToClearSideBorder + 320 + 48 /*0x30*/ + IClickableMenu.borderWidth, this.yPositionOnScreen + IClickableMenu.borderWidth + IClickableMenu.spaceToClearTopBorder + num4);
    int x = this.xPositionOnScreen + 16 /*0x10*/ + IClickableMenu.spaceToClearSideBorder + IClickableMenu.borderWidth + 192 /*0xC0*/ + 8;
    if (this._isDyeMenu)
      x = this.xPositionOnScreen + IClickableMenu.spaceToClearSideBorder + IClickableMenu.borderWidth;
    if (flag1 || this.source == CharacterCustomization.Source.NewFarmhand || this.source == CharacterCustomization.Source.Wizard)
    {
      this.labels.Add(new ClickableComponent(new Rectangle(this.xPositionOnScreen + 16 /*0x10*/ + IClickableMenu.spaceToClearSideBorder + IClickableMenu.borderWidth + 192 /*0xC0*/ + 8, this.yPositionOnScreen + IClickableMenu.borderWidth + IClickableMenu.spaceToClearTopBorder + num4 + 16 /*0x10*/, 1, 1), Game1.content.LoadString("Strings\\UI:Character_EyeColor")));
      this.eyeColorPicker = new ColorPicker("Eyes", point.X, point.Y);
      this.eyeColorPicker.setColor(Game1.player.newEyeColor.Value);
      this.colorPickerCCs.Add(new ClickableComponent(new Rectangle(point.X, point.Y, 128 /*0x80*/, 20), "")
      {
        myID = 522,
        downNeighborID = -99998,
        upNeighborID = -99998,
        leftNeighborImmutable = true,
        rightNeighborImmutable = true
      });
      this.colorPickerCCs.Add(new ClickableComponent(new Rectangle(point.X, point.Y + 20, 128 /*0x80*/, 20), "")
      {
        myID = 523,
        upNeighborID = -99998,
        downNeighborID = -99998,
        leftNeighborImmutable = true,
        rightNeighborImmutable = true
      });
      this.colorPickerCCs.Add(new ClickableComponent(new Rectangle(point.X, point.Y + 40, 128 /*0x80*/, 20), "")
      {
        myID = 524,
        upNeighborID = -99998,
        downNeighborID = -99998,
        leftNeighborImmutable = true,
        rightNeighborImmutable = true
      });
      num4 += 68;
      List<ClickableComponent> selectionButtons11 = this.leftSelectionButtons;
      ClickableTextureComponent textureComponent21 = new ClickableTextureComponent("Hair", new Rectangle(this.xPositionOnScreen + 16 /*0x10*/ + IClickableMenu.borderWidth + IClickableMenu.spaceToClearSideBorder + num5, this.yPositionOnScreen + IClickableMenu.borderWidth + IClickableMenu.spaceToClearTopBorder + num4, 64 /*0x40*/, 64 /*0x40*/), (string) null, "", Game1.mouseCursors, Game1.getSourceRectForStandardTileSheet(Game1.mouseCursors, 44), 1f);
      textureComponent21.myID = 514;
      textureComponent21.upNeighborID = -99998;
      textureComponent21.leftNeighborID = -99998;
      textureComponent21.rightNeighborID = -99998;
      textureComponent21.downNeighborID = -99998;
      selectionButtons11.Add((ClickableComponent) textureComponent21);
      this.labels.Add(this.hairLabel = new ClickableComponent(new Rectangle(this.xPositionOnScreen + 16 /*0x10*/ + IClickableMenu.spaceToClearSideBorder + IClickableMenu.borderWidth + 64 /*0x40*/ + 8 + num5 / 2, this.yPositionOnScreen + IClickableMenu.borderWidth + IClickableMenu.spaceToClearTopBorder + num4 + 16 /*0x10*/, 1, 1), Game1.content.LoadString("Strings\\UI:Character_Hair")));
      List<ClickableComponent> selectionButtons12 = this.rightSelectionButtons;
      ClickableTextureComponent textureComponent22 = new ClickableTextureComponent("Hair", new Rectangle(this.xPositionOnScreen + 16 /*0x10*/ + IClickableMenu.spaceToClearSideBorder + 128 /*0x80*/ + IClickableMenu.borderWidth, this.yPositionOnScreen + IClickableMenu.borderWidth + IClickableMenu.spaceToClearTopBorder + num4, 64 /*0x40*/, 64 /*0x40*/), (string) null, "", Game1.mouseCursors, Game1.getSourceRectForStandardTileSheet(Game1.mouseCursors, 33), 1f);
      textureComponent22.myID = 515;
      textureComponent22.upNeighborID = -99998;
      textureComponent22.leftNeighborID = -99998;
      textureComponent22.rightNeighborID = -99998;
      textureComponent22.downNeighborID = -99998;
      selectionButtons12.Add((ClickableComponent) textureComponent22);
    }
    point = new Point(this.xPositionOnScreen + IClickableMenu.spaceToClearSideBorder + 320 + 48 /*0x30*/ + IClickableMenu.borderWidth, this.yPositionOnScreen + IClickableMenu.borderWidth + IClickableMenu.spaceToClearTopBorder + num4);
    if (flag1 || this.source == CharacterCustomization.Source.NewFarmhand || this.source == CharacterCustomization.Source.Wizard)
    {
      this.labels.Add(new ClickableComponent(new Rectangle(x, this.yPositionOnScreen + IClickableMenu.borderWidth + IClickableMenu.spaceToClearTopBorder + num4 + 16 /*0x10*/, 1, 1), Game1.content.LoadString("Strings\\UI:Character_HairColor")));
      this.hairColorPicker = new ColorPicker("Hair", point.X, point.Y);
      this.hairColorPicker.setColor(Game1.player.hairstyleColor.Value);
      this.colorPickerCCs.Add(new ClickableComponent(new Rectangle(point.X, point.Y, 128 /*0x80*/, 20), "")
      {
        myID = 525,
        downNeighborID = -99998,
        upNeighborID = -99998,
        leftNeighborImmutable = true,
        rightNeighborImmutable = true
      });
      this.colorPickerCCs.Add(new ClickableComponent(new Rectangle(point.X, point.Y + 20, 128 /*0x80*/, 20), "")
      {
        myID = 526,
        upNeighborID = -99998,
        downNeighborID = -99998,
        leftNeighborImmutable = true,
        rightNeighborImmutable = true
      });
      this.colorPickerCCs.Add(new ClickableComponent(new Rectangle(point.X, point.Y + 40, 128 /*0x80*/, 20), "")
      {
        myID = 527,
        upNeighborID = -99998,
        downNeighborID = -99998,
        leftNeighborImmutable = true,
        rightNeighborImmutable = true
      });
    }
    if (this.source == CharacterCustomization.Source.DyePots)
    {
      num4 += 68;
      if (Game1.player.CanDyeShirt())
      {
        point = new Point(this.xPositionOnScreen + IClickableMenu.spaceToClearSideBorder + 320 + 48 /*0x30*/ + IClickableMenu.borderWidth, this.yPositionOnScreen + IClickableMenu.borderWidth + IClickableMenu.spaceToClearTopBorder + num4);
        point.X = this.xPositionOnScreen + this.width - IClickableMenu.spaceToClearSideBorder - IClickableMenu.borderWidth - 160 /*0xA0*/;
        this.labels.Add(new ClickableComponent(new Rectangle(x, this.yPositionOnScreen + IClickableMenu.borderWidth + IClickableMenu.spaceToClearTopBorder + num4 + 16 /*0x10*/, 1, 1), Game1.content.LoadString("Strings\\UI:Character_ShirtColor")));
        this.hairColorPicker = new ColorPicker("Hair", point.X, point.Y);
        this.hairColorPicker.setColor(Game1.player.GetShirtColor());
        this.colorPickerCCs.Add(new ClickableComponent(new Rectangle(point.X, point.Y, 128 /*0x80*/, 20), "")
        {
          myID = 525,
          downNeighborID = -99998,
          upNeighborID = -99998,
          leftNeighborImmutable = true,
          rightNeighborImmutable = true
        });
        this.colorPickerCCs.Add(new ClickableComponent(new Rectangle(point.X, point.Y + 20, 128 /*0x80*/, 20), "")
        {
          myID = 526,
          upNeighborID = -99998,
          downNeighborID = -99998,
          leftNeighborImmutable = true,
          rightNeighborImmutable = true
        });
        this.colorPickerCCs.Add(new ClickableComponent(new Rectangle(point.X, point.Y + 40, 128 /*0x80*/, 20), "")
        {
          myID = 527,
          upNeighborID = -99998,
          downNeighborID = -99998,
          leftNeighborImmutable = true,
          rightNeighborImmutable = true
        });
        num4 += 64 /*0x40*/;
      }
      if (Game1.player.CanDyePants())
      {
        point = new Point(this.xPositionOnScreen + IClickableMenu.spaceToClearSideBorder + 320 + 48 /*0x30*/ + IClickableMenu.borderWidth, this.yPositionOnScreen + IClickableMenu.borderWidth + IClickableMenu.spaceToClearTopBorder + num4);
        point.X = this.xPositionOnScreen + this.width - IClickableMenu.spaceToClearSideBorder - IClickableMenu.borderWidth - 160 /*0xA0*/;
        int num8 = LocalizedContentManager.CurrentLanguageCode == LocalizedContentManager.LanguageCode.tr ? -16 : 0;
        this.labels.Add(new ClickableComponent(new Rectangle(x, this.yPositionOnScreen + IClickableMenu.borderWidth + IClickableMenu.spaceToClearTopBorder + num4 + 16 /*0x10*/ + num8, 1, 1), Game1.content.LoadString("Strings\\UI:Character_PantsColor")));
        this.pantsColorPicker = new ColorPicker("Pants", point.X, point.Y);
        this.pantsColorPicker.setColor(Game1.player.GetPantsColor());
        this.colorPickerCCs.Add(new ClickableComponent(new Rectangle(point.X, point.Y, 128 /*0x80*/, 20), "")
        {
          myID = 528,
          downNeighborID = -99998,
          upNeighborID = -99998,
          rightNeighborImmutable = true,
          leftNeighborImmutable = true
        });
        this.colorPickerCCs.Add(new ClickableComponent(new Rectangle(point.X, point.Y + 20, 128 /*0x80*/, 20), "")
        {
          myID = 529,
          downNeighborID = -99998,
          upNeighborID = -99998,
          rightNeighborImmutable = true,
          leftNeighborImmutable = true
        });
        this.colorPickerCCs.Add(new ClickableComponent(new Rectangle(point.X, point.Y + 40, 128 /*0x80*/, 20), "")
        {
          myID = 530,
          downNeighborID = -99998,
          upNeighborID = -99998,
          rightNeighborImmutable = true,
          leftNeighborImmutable = true
        });
      }
    }
    else if (flag2)
    {
      num4 += 68;
      int num9 = LocalizedContentManager.CurrentLanguageCode == LocalizedContentManager.LanguageCode.tr ? 8 : 0;
      List<ClickableComponent> selectionButtons13 = this.leftSelectionButtons;
      ClickableTextureComponent textureComponent23 = new ClickableTextureComponent("Shirt", new Rectangle(this.xPositionOnScreen + 16 /*0x10*/ + IClickableMenu.spaceToClearSideBorder + IClickableMenu.borderWidth + num5 - num9, this.yPositionOnScreen + IClickableMenu.borderWidth + IClickableMenu.spaceToClearTopBorder + num4, 64 /*0x40*/, 64 /*0x40*/), (string) null, "", Game1.mouseCursors, Game1.getSourceRectForStandardTileSheet(Game1.mouseCursors, 44), 1f);
      textureComponent23.myID = 512 /*0x0200*/;
      textureComponent23.upNeighborID = -99998;
      textureComponent23.leftNeighborID = -99998;
      textureComponent23.rightNeighborID = -99998;
      textureComponent23.downNeighborID = -99998;
      selectionButtons13.Add((ClickableComponent) textureComponent23);
      this.labels.Add(this.shirtLabel = new ClickableComponent(new Rectangle(this.xPositionOnScreen + 16 /*0x10*/ + IClickableMenu.spaceToClearSideBorder + IClickableMenu.borderWidth + 64 /*0x40*/ + 8 + num5 / 2, this.yPositionOnScreen + IClickableMenu.borderWidth + IClickableMenu.spaceToClearTopBorder + num4 + 16 /*0x10*/, 1, 1), Game1.content.LoadString("Strings\\UI:Character_Shirt")));
      List<ClickableComponent> selectionButtons14 = this.rightSelectionButtons;
      ClickableTextureComponent textureComponent24 = new ClickableTextureComponent("Shirt", new Rectangle(this.xPositionOnScreen + 16 /*0x10*/ + IClickableMenu.spaceToClearSideBorder + 128 /*0x80*/ + IClickableMenu.borderWidth + num9, this.yPositionOnScreen + IClickableMenu.borderWidth + IClickableMenu.spaceToClearTopBorder + num4, 64 /*0x40*/, 64 /*0x40*/), (string) null, "", Game1.mouseCursors, Game1.getSourceRectForStandardTileSheet(Game1.mouseCursors, 33), 1f);
      textureComponent24.myID = 513;
      textureComponent24.upNeighborID = -99998;
      textureComponent24.leftNeighborID = -99998;
      textureComponent24.rightNeighborID = -99998;
      textureComponent24.downNeighborID = -99998;
      selectionButtons14.Add((ClickableComponent) textureComponent24);
      int num10 = LocalizedContentManager.CurrentLanguageCode == LocalizedContentManager.LanguageCode.tr ? -16 : 0;
      this.labels.Add(new ClickableComponent(new Rectangle(x, this.yPositionOnScreen + IClickableMenu.borderWidth + IClickableMenu.spaceToClearTopBorder + num4 + 16 /*0x10*/ + num10, 1, 1), Game1.content.LoadString("Strings\\UI:Character_PantsColor")));
      point = new Point(this.xPositionOnScreen + IClickableMenu.spaceToClearSideBorder + 320 + 48 /*0x30*/ + IClickableMenu.borderWidth, this.yPositionOnScreen + IClickableMenu.borderWidth + IClickableMenu.spaceToClearTopBorder + num4);
      this.pantsColorPicker = new ColorPicker("Pants", point.X, point.Y);
      this.pantsColorPicker.setColor(Game1.player.GetPantsColor());
      this.colorPickerCCs.Add(new ClickableComponent(new Rectangle(point.X, point.Y, 128 /*0x80*/, 20), "")
      {
        myID = 528,
        downNeighborID = -99998,
        upNeighborID = -99998,
        rightNeighborImmutable = true,
        leftNeighborImmutable = true
      });
      this.colorPickerCCs.Add(new ClickableComponent(new Rectangle(point.X, point.Y + 20, 128 /*0x80*/, 20), "")
      {
        myID = 529,
        downNeighborID = -99998,
        upNeighborID = -99998,
        rightNeighborImmutable = true,
        leftNeighborImmutable = true
      });
      this.colorPickerCCs.Add(new ClickableComponent(new Rectangle(point.X, point.Y + 40, 128 /*0x80*/, 20), "")
      {
        myID = 530,
        downNeighborID = -99998,
        upNeighborID = -99998,
        rightNeighborImmutable = true,
        leftNeighborImmutable = true
      });
    }
    else if (this.source == CharacterCustomization.Source.ClothesDye)
    {
      num4 += 60;
      point = new Point(this.xPositionOnScreen + IClickableMenu.spaceToClearSideBorder + 320 + 48 /*0x30*/ + IClickableMenu.borderWidth, this.yPositionOnScreen + IClickableMenu.borderWidth + IClickableMenu.spaceToClearTopBorder + num4);
      point.X = this.xPositionOnScreen + this.width - IClickableMenu.spaceToClearSideBorder - IClickableMenu.borderWidth - 160 /*0xA0*/;
      this.labels.Add(new ClickableComponent(new Rectangle(x, this.yPositionOnScreen + IClickableMenu.borderWidth + IClickableMenu.spaceToClearTopBorder + num4 + 16 /*0x10*/, 1, 1), Game1.content.LoadString("Strings\\UI:Character_DyeColor")));
      this.pantsColorPicker = new ColorPicker("Pants", point.X, point.Y);
      this.pantsColorPicker.setColor(this._itemToDye.clothesColor.Value);
      this.colorPickerCCs.Add(new ClickableComponent(new Rectangle(point.X, point.Y, 128 /*0x80*/, 20), "")
      {
        myID = 528,
        downNeighborID = -99998,
        upNeighborID = -99998,
        rightNeighborImmutable = true,
        leftNeighborImmutable = true
      });
      this.colorPickerCCs.Add(new ClickableComponent(new Rectangle(point.X, point.Y + 20, 128 /*0x80*/, 20), "")
      {
        myID = 529,
        downNeighborID = -99998,
        upNeighborID = -99998,
        rightNeighborImmutable = true,
        leftNeighborImmutable = true
      });
      this.colorPickerCCs.Add(new ClickableComponent(new Rectangle(point.X, point.Y + 40, 128 /*0x80*/, 20), "")
      {
        myID = 530,
        downNeighborID = -99998,
        upNeighborID = -99998,
        rightNeighborImmutable = true,
        leftNeighborImmutable = true
      });
    }
    ClickableTextureComponent textureComponent25 = new ClickableTextureComponent("Skip Intro", new Rectangle(this.xPositionOnScreen + IClickableMenu.spaceToClearSideBorder + 320 - 48 /*0x30*/ + IClickableMenu.borderWidth, this.yPositionOnScreen + IClickableMenu.borderWidth + IClickableMenu.spaceToClearTopBorder + num4 + 80 /*0x50*/, 36, 36), (string) null, Game1.content.LoadString("Strings\\UI:Character_SkipIntro"), Game1.mouseCursors, new Rectangle(227, 425, 9, 9), 4f);
    textureComponent25.myID = 506;
    textureComponent25.upNeighborID = 530;
    textureComponent25.leftNeighborID = 517;
    textureComponent25.rightNeighborID = 505;
    this.skipIntroButton = textureComponent25;
    this.skipIntroButton.sourceRect.X = this.skipIntro ? 236 : 227;
    if (flag2)
    {
      num4 += 68;
      List<ClickableComponent> selectionButtons15 = this.leftSelectionButtons;
      ClickableTextureComponent textureComponent26 = new ClickableTextureComponent("Pants Style", new Rectangle(this.xPositionOnScreen + 16 /*0x10*/ + IClickableMenu.spaceToClearSideBorder + IClickableMenu.borderWidth + num5, this.yPositionOnScreen + IClickableMenu.borderWidth + IClickableMenu.spaceToClearTopBorder + num4, 64 /*0x40*/, 64 /*0x40*/), (string) null, "", Game1.mouseCursors, Game1.getSourceRectForStandardTileSheet(Game1.mouseCursors, 44), 1f);
      textureComponent26.myID = 629;
      textureComponent26.upNeighborID = -99998;
      textureComponent26.leftNeighborID = -99998;
      textureComponent26.rightNeighborID = -99998;
      textureComponent26.downNeighborID = -99998;
      selectionButtons15.Add((ClickableComponent) textureComponent26);
      this.labels.Add(this.pantsStyleLabel = new ClickableComponent(new Rectangle(this.xPositionOnScreen + 16 /*0x10*/ + IClickableMenu.spaceToClearSideBorder + IClickableMenu.borderWidth + 64 /*0x40*/ + 8 + num5 / 2, this.yPositionOnScreen + IClickableMenu.borderWidth + IClickableMenu.spaceToClearTopBorder + num4 + 16 /*0x10*/, 1, 1), Game1.content.LoadString("Strings\\UI:Character_Pants")));
      List<ClickableComponent> selectionButtons16 = this.rightSelectionButtons;
      ClickableTextureComponent textureComponent27 = new ClickableTextureComponent("Pants Style", new Rectangle(this.xPositionOnScreen + 16 /*0x10*/ + IClickableMenu.spaceToClearSideBorder + 128 /*0x80*/ + IClickableMenu.borderWidth, this.yPositionOnScreen + IClickableMenu.borderWidth + IClickableMenu.spaceToClearTopBorder + num4, 64 /*0x40*/, 64 /*0x40*/), (string) null, "", Game1.mouseCursors, Game1.getSourceRectForStandardTileSheet(Game1.mouseCursors, 33), 1f);
      textureComponent27.myID = 517;
      textureComponent27.upNeighborID = -99998;
      textureComponent27.leftNeighborID = -99998;
      textureComponent27.rightNeighborID = -99998;
      textureComponent27.downNeighborID = -99998;
      selectionButtons16.Add((ClickableComponent) textureComponent27);
    }
    int num11 = num4 + 68;
    if (flag3)
    {
      int num12 = LocalizedContentManager.CurrentLanguageCode == LocalizedContentManager.LanguageCode.ko || LocalizedContentManager.CurrentLanguageCode == LocalizedContentManager.LanguageCode.tr ? 32 /*0x20*/ : 0;
      List<ClickableComponent> selectionButtons17 = this.leftSelectionButtons;
      ClickableTextureComponent textureComponent28 = new ClickableTextureComponent("Acc", new Rectangle(this.xPositionOnScreen + 16 /*0x10*/ + IClickableMenu.spaceToClearSideBorder + IClickableMenu.borderWidth + num5 - num12, this.yPositionOnScreen + IClickableMenu.borderWidth + IClickableMenu.spaceToClearTopBorder + num11, 64 /*0x40*/, 64 /*0x40*/), (string) null, "", Game1.mouseCursors, Game1.getSourceRectForStandardTileSheet(Game1.mouseCursors, 44), 1f);
      textureComponent28.myID = 516;
      textureComponent28.upNeighborID = -99998;
      textureComponent28.leftNeighborID = -99998;
      textureComponent28.rightNeighborID = -99998;
      textureComponent28.downNeighborID = -99998;
      selectionButtons17.Add((ClickableComponent) textureComponent28);
      this.labels.Add(this.accLabel = new ClickableComponent(new Rectangle(this.xPositionOnScreen + 16 /*0x10*/ + IClickableMenu.spaceToClearSideBorder + IClickableMenu.borderWidth + 64 /*0x40*/ + 8 + num5 / 2, this.yPositionOnScreen + IClickableMenu.borderWidth + IClickableMenu.spaceToClearTopBorder + num11 + 16 /*0x10*/, 1, 1), Game1.content.LoadString("Strings\\UI:Character_Accessory")));
      List<ClickableComponent> selectionButtons18 = this.rightSelectionButtons;
      ClickableTextureComponent textureComponent29 = new ClickableTextureComponent("Acc", new Rectangle(this.xPositionOnScreen + 16 /*0x10*/ + IClickableMenu.spaceToClearSideBorder + 128 /*0x80*/ + IClickableMenu.borderWidth + num12, this.yPositionOnScreen + IClickableMenu.borderWidth + IClickableMenu.spaceToClearTopBorder + num11, 64 /*0x40*/, 64 /*0x40*/), (string) null, "", Game1.mouseCursors, Game1.getSourceRectForStandardTileSheet(Game1.mouseCursors, 33), 1f);
      textureComponent29.myID = 517;
      textureComponent29.upNeighborID = -99998;
      textureComponent29.leftNeighborID = -99998;
      textureComponent29.rightNeighborID = -99998;
      textureComponent29.downNeighborID = -99998;
      selectionButtons18.Add((ClickableComponent) textureComponent29);
    }
    if (Game1.gameMode == (byte) 3)
    {
      IList<GameLocation> locations = Game1.locations;
    }
    if (this.petPortraitBox.HasValue)
    {
      List<ClickableComponent> selectionButtons19 = this.leftSelectionButtons;
      ClickableTextureComponent textureComponent30 = new ClickableTextureComponent("Pet", new Rectangle(this.petPortraitBox.Value.Left - 64 /*0x40*/, this.petPortraitBox.Value.Top, 64 /*0x40*/, 64 /*0x40*/), (string) null, "", Game1.mouseCursors, Game1.getSourceRectForStandardTileSheet(Game1.mouseCursors, 44), 1f);
      textureComponent30.myID = 511 /*0x01FF*/;
      textureComponent30.upNeighborID = -99998;
      textureComponent30.leftNeighborID = -99998;
      textureComponent30.rightNeighborID = -99998;
      textureComponent30.downNeighborID = -99998;
      selectionButtons19.Add((ClickableComponent) textureComponent30);
      List<ClickableComponent> selectionButtons20 = this.rightSelectionButtons;
      ClickableTextureComponent textureComponent31 = new ClickableTextureComponent("Pet", new Rectangle(this.petPortraitBox.Value.Left + 64 /*0x40*/, this.petPortraitBox.Value.Top, 64 /*0x40*/, 64 /*0x40*/), (string) null, "", Game1.mouseCursors, Game1.getSourceRectForStandardTileSheet(Game1.mouseCursors, 33), 1f);
      textureComponent31.myID = 510;
      textureComponent31.upNeighborID = -99998;
      textureComponent31.leftNeighborID = -99998;
      textureComponent31.rightNeighborID = -99998;
      textureComponent31.downNeighborID = -99998;
      selectionButtons20.Add((ClickableComponent) textureComponent31);
      List<ClickableComponent> colorPickerCcs = this.colorPickerCCs;
      // ISSUE: explicit non-virtual call
      if ((colorPickerCcs != null ? (__nonvirtual (colorPickerCcs.Count) > 0 ? 1 : 0) : 0) != 0)
      {
        this.colorPickerCCs[0].upNeighborID = 511 /*0x01FF*/;
        this.colorPickerCCs[0].upNeighborImmutable = true;
      }
    }
    this._shouldShowBackButton = true;
    if (this.source == CharacterCustomization.Source.Dresser || this.source == CharacterCustomization.Source.Wizard || this.source == CharacterCustomization.Source.ClothesDye)
      this._shouldShowBackButton = false;
    if (this.source == CharacterCustomization.Source.Dresser || this.source == CharacterCustomization.Source.Wizard || this._isDyeMenu)
    {
      this.nameBoxCC.visible = false;
      this.farmnameBoxCC.visible = false;
      this.favThingBoxCC.visible = false;
      this.farmLabel.visible = false;
      this.nameLabel.visible = false;
      this.favoriteLabel.visible = false;
    }
    if (this.source == CharacterCustomization.Source.Wizard)
    {
      this.nameLabel.visible = true;
      this.nameBoxCC.visible = true;
      this.favThingBoxCC.visible = true;
      this.favoriteLabel.visible = true;
      this.favThingBoxCC.bounds.Y = this.farmnameBoxCC.bounds.Y;
      this.favoriteLabel.bounds.Y = this.farmLabel.bounds.Y;
      this.favThingBox.Y = this.farmnameBox.Y;
    }
    this.skipIntroButton.visible = flag1;
    if (!Game1.options.snappyMenus || !Game1.options.gamepadControls)
      return;
    this.populateClickableComponentList();
    this.snapToDefaultClickableComponent();
  }

  public virtual void LoadFarmTypeData()
  {
    List<ModFarmType> modFarmTypeList = DataLoader.AdditionalFarms(Game1.content);
    this.farmTypeButtonNames.Add("Standard");
    this.farmTypeButtonNames.Add("Riverland");
    this.farmTypeButtonNames.Add("Forest");
    this.farmTypeButtonNames.Add("Hills");
    this.farmTypeButtonNames.Add("Wilderness");
    this.farmTypeButtonNames.Add("Four Corners");
    this.farmTypeButtonNames.Add("Beach");
    this.farmTypeHoverText.Add(this.GetFarmTypeTooltip("Strings\\UI:Character_FarmStandard"));
    this.farmTypeHoverText.Add(this.GetFarmTypeTooltip("Strings\\UI:Character_FarmFishing"));
    this.farmTypeHoverText.Add(this.GetFarmTypeTooltip("Strings\\UI:Character_FarmForaging"));
    this.farmTypeHoverText.Add(this.GetFarmTypeTooltip("Strings\\UI:Character_FarmMining"));
    this.farmTypeHoverText.Add(this.GetFarmTypeTooltip("Strings\\UI:Character_FarmCombat"));
    this.farmTypeHoverText.Add(this.GetFarmTypeTooltip("Strings\\UI:Character_FarmFourCorners"));
    this.farmTypeHoverText.Add(this.GetFarmTypeTooltip("Strings\\UI:Character_FarmBeach"));
    this.farmTypeIcons.Add(new KeyValuePair<Texture2D, Rectangle>(Game1.mouseCursors, new Rectangle(0, 324, 22, 20)));
    this.farmTypeIcons.Add(new KeyValuePair<Texture2D, Rectangle>(Game1.mouseCursors, new Rectangle(22, 324, 22, 20)));
    this.farmTypeIcons.Add(new KeyValuePair<Texture2D, Rectangle>(Game1.mouseCursors, new Rectangle(44, 324, 22, 20)));
    this.farmTypeIcons.Add(new KeyValuePair<Texture2D, Rectangle>(Game1.mouseCursors, new Rectangle(66, 324, 22, 20)));
    this.farmTypeIcons.Add(new KeyValuePair<Texture2D, Rectangle>(Game1.mouseCursors, new Rectangle(88, 324, 22, 20)));
    this.farmTypeIcons.Add(new KeyValuePair<Texture2D, Rectangle>(Game1.mouseCursors, new Rectangle(0, 345, 22, 20)));
    this.farmTypeIcons.Add(new KeyValuePair<Texture2D, Rectangle>(Game1.mouseCursors, new Rectangle(22, 345, 22, 20)));
    if (modFarmTypeList != null)
    {
      foreach (ModFarmType modFarmType in modFarmTypeList)
      {
        this.farmTypeButtonNames.Add("ModFarm_" + modFarmType.Id);
        this.farmTypeHoverText.Add(this.GetFarmTypeTooltip(modFarmType.TooltipStringPath));
        if (modFarmType.IconTexture != null)
          this.farmTypeIcons.Add(new KeyValuePair<Texture2D, Rectangle>(Game1.content.Load<Texture2D>(modFarmType.IconTexture), new Rectangle(0, 0, 22, 20)));
        else
          this.farmTypeIcons.Add(new KeyValuePair<Texture2D, Rectangle>(Game1.mouseCursors, new Rectangle(1, 324, 22, 20)));
      }
    }
    this._farmPages = 1;
    if (modFarmTypeList == null)
      return;
    this._farmPages = (int) Math.Floor((double) (this.farmTypeButtonNames.Count - 1) / 12.0) + 1;
  }

  public virtual void RefreshFarmTypeButtons()
  {
    this.farmTypeButtons.Clear();
    Point point = new Point(this.xPositionOnScreen + this.width + 4 + 8, this.yPositionOnScreen + IClickableMenu.borderWidth);
    int index = this._currentFarmPage * 12;
    if (index < this.farmTypeButtonNames.Count)
    {
      List<ClickableTextureComponent> farmTypeButtons = this.farmTypeButtons;
      ClickableTextureComponent textureComponent = new ClickableTextureComponent(this.farmTypeButtonNames[index], new Rectangle(point.X, point.Y + 88, 88, 80 /*0x50*/), (string) null, this.farmTypeHoverText[index], this.farmTypeIcons[index].Key, this.farmTypeIcons[index].Value, 4f);
      textureComponent.myID = 531;
      textureComponent.downNeighborID = -99998;
      textureComponent.rightNeighborID = -99998;
      textureComponent.leftNeighborID = 537;
      farmTypeButtons.Add(textureComponent);
      ++index;
    }
    if (index < this.farmTypeButtonNames.Count)
    {
      List<ClickableTextureComponent> farmTypeButtons = this.farmTypeButtons;
      ClickableTextureComponent textureComponent = new ClickableTextureComponent(this.farmTypeButtonNames[index], new Rectangle(point.X, point.Y + 176 /*0xB0*/, 88, 80 /*0x50*/), (string) null, this.farmTypeHoverText[index], this.farmTypeIcons[index].Key, this.farmTypeIcons[index].Value, 4f);
      textureComponent.myID = 532;
      textureComponent.upNeighborID = -99998;
      textureComponent.leftNeighborID = -99998;
      textureComponent.rightNeighborID = -99998;
      textureComponent.downNeighborID = -99998;
      farmTypeButtons.Add(textureComponent);
      ++index;
    }
    if (index < this.farmTypeButtonNames.Count)
    {
      List<ClickableTextureComponent> farmTypeButtons = this.farmTypeButtons;
      ClickableTextureComponent textureComponent = new ClickableTextureComponent(this.farmTypeButtonNames[index], new Rectangle(point.X, point.Y + 264, 88, 80 /*0x50*/), (string) null, this.farmTypeHoverText[index], this.farmTypeIcons[index].Key, this.farmTypeIcons[index].Value, 4f);
      textureComponent.myID = 533;
      textureComponent.upNeighborID = -99998;
      textureComponent.leftNeighborID = -99998;
      textureComponent.rightNeighborID = -99998;
      textureComponent.downNeighborID = -99998;
      farmTypeButtons.Add(textureComponent);
      ++index;
    }
    if (index < this.farmTypeButtonNames.Count)
    {
      List<ClickableTextureComponent> farmTypeButtons = this.farmTypeButtons;
      ClickableTextureComponent textureComponent = new ClickableTextureComponent(this.farmTypeButtonNames[index], new Rectangle(point.X, point.Y + 352, 88, 80 /*0x50*/), (string) null, this.farmTypeHoverText[index], this.farmTypeIcons[index].Key, this.farmTypeIcons[index].Value, 4f);
      textureComponent.myID = 534;
      textureComponent.upNeighborID = -99998;
      textureComponent.leftNeighborID = -99998;
      textureComponent.rightNeighborID = -99998;
      textureComponent.downNeighborID = -99998;
      farmTypeButtons.Add(textureComponent);
      ++index;
    }
    if (index < this.farmTypeButtonNames.Count)
    {
      List<ClickableTextureComponent> farmTypeButtons = this.farmTypeButtons;
      ClickableTextureComponent textureComponent = new ClickableTextureComponent(this.farmTypeButtonNames[index], new Rectangle(point.X, point.Y + 440, 88, 80 /*0x50*/), (string) null, this.farmTypeHoverText[index], this.farmTypeIcons[index].Key, this.farmTypeIcons[index].Value, 4f);
      textureComponent.myID = 535;
      textureComponent.upNeighborID = -99998;
      textureComponent.leftNeighborID = -99998;
      textureComponent.rightNeighborID = -99998;
      textureComponent.downNeighborID = -99998;
      farmTypeButtons.Add(textureComponent);
      ++index;
    }
    if (index < this.farmTypeButtonNames.Count)
    {
      List<ClickableTextureComponent> farmTypeButtons = this.farmTypeButtons;
      ClickableTextureComponent textureComponent = new ClickableTextureComponent(this.farmTypeButtonNames[index], new Rectangle(point.X, point.Y + 528, 88, 80 /*0x50*/), (string) null, this.farmTypeHoverText[index], this.farmTypeIcons[index].Key, this.farmTypeIcons[index].Value, 4f);
      textureComponent.myID = 545;
      textureComponent.upNeighborID = -99998;
      textureComponent.leftNeighborID = -99998;
      textureComponent.rightNeighborID = -99998;
      textureComponent.downNeighborID = -99998;
      farmTypeButtons.Add(textureComponent);
      ++index;
    }
    KeyValuePair<Texture2D, Rectangle> farmTypeIcon;
    if (index < this.farmTypeButtonNames.Count)
    {
      List<ClickableTextureComponent> farmTypeButtons = this.farmTypeButtons;
      string farmTypeButtonName = this.farmTypeButtonNames[index];
      Rectangle bounds = new Rectangle(point.X + 96 /*0x60*/, point.Y + 88, 88, 80 /*0x50*/);
      string hoverText = this.farmTypeHoverText[index];
      farmTypeIcon = this.farmTypeIcons[index];
      Texture2D key = farmTypeIcon.Key;
      farmTypeIcon = this.farmTypeIcons[index];
      Rectangle sourceRect = farmTypeIcon.Value;
      ClickableTextureComponent textureComponent = new ClickableTextureComponent(farmTypeButtonName, bounds, (string) null, hoverText, key, sourceRect, 4f);
      textureComponent.myID = 546;
      textureComponent.upNeighborID = -99998;
      textureComponent.leftNeighborID = -99998;
      textureComponent.rightNeighborID = -99998;
      textureComponent.downNeighborID = -99998;
      farmTypeButtons.Add(textureComponent);
      ++index;
    }
    if (index < this.farmTypeButtonNames.Count)
    {
      List<ClickableTextureComponent> farmTypeButtons = this.farmTypeButtons;
      string farmTypeButtonName = this.farmTypeButtonNames[index];
      Rectangle bounds = new Rectangle(point.X + 96 /*0x60*/, point.Y + 176 /*0xB0*/, 88, 80 /*0x50*/);
      string hoverText = this.farmTypeHoverText[index];
      farmTypeIcon = this.farmTypeIcons[index];
      Texture2D key = farmTypeIcon.Key;
      farmTypeIcon = this.farmTypeIcons[index];
      Rectangle sourceRect = farmTypeIcon.Value;
      ClickableTextureComponent textureComponent = new ClickableTextureComponent(farmTypeButtonName, bounds, (string) null, hoverText, key, sourceRect, 4f);
      textureComponent.myID = 547;
      textureComponent.upNeighborID = -99998;
      textureComponent.leftNeighborID = -99998;
      textureComponent.rightNeighborID = -99998;
      textureComponent.downNeighborID = -99998;
      farmTypeButtons.Add(textureComponent);
      ++index;
    }
    if (index < this.farmTypeButtonNames.Count)
    {
      List<ClickableTextureComponent> farmTypeButtons = this.farmTypeButtons;
      string farmTypeButtonName = this.farmTypeButtonNames[index];
      Rectangle bounds = new Rectangle(point.X + 96 /*0x60*/, point.Y + 264, 88, 80 /*0x50*/);
      string hoverText = this.farmTypeHoverText[index];
      farmTypeIcon = this.farmTypeIcons[index];
      Texture2D key = farmTypeIcon.Key;
      farmTypeIcon = this.farmTypeIcons[index];
      Rectangle sourceRect = farmTypeIcon.Value;
      ClickableTextureComponent textureComponent = new ClickableTextureComponent(farmTypeButtonName, bounds, (string) null, hoverText, key, sourceRect, 4f);
      textureComponent.myID = 548;
      textureComponent.upNeighborID = -99998;
      textureComponent.leftNeighborID = -99998;
      textureComponent.rightNeighborID = -99998;
      textureComponent.downNeighborID = -99998;
      farmTypeButtons.Add(textureComponent);
      ++index;
    }
    if (index < this.farmTypeButtonNames.Count)
    {
      List<ClickableTextureComponent> farmTypeButtons = this.farmTypeButtons;
      string farmTypeButtonName = this.farmTypeButtonNames[index];
      Rectangle bounds = new Rectangle(point.X + 96 /*0x60*/, point.Y + 352, 88, 80 /*0x50*/);
      string hoverText = this.farmTypeHoverText[index];
      farmTypeIcon = this.farmTypeIcons[index];
      Texture2D key = farmTypeIcon.Key;
      farmTypeIcon = this.farmTypeIcons[index];
      Rectangle sourceRect = farmTypeIcon.Value;
      ClickableTextureComponent textureComponent = new ClickableTextureComponent(farmTypeButtonName, bounds, (string) null, hoverText, key, sourceRect, 4f);
      textureComponent.myID = 549;
      textureComponent.upNeighborID = -99998;
      textureComponent.leftNeighborID = -99998;
      textureComponent.rightNeighborID = -99998;
      textureComponent.downNeighborID = -99998;
      farmTypeButtons.Add(textureComponent);
      ++index;
    }
    if (index < this.farmTypeButtonNames.Count)
    {
      List<ClickableTextureComponent> farmTypeButtons = this.farmTypeButtons;
      string farmTypeButtonName = this.farmTypeButtonNames[index];
      Rectangle bounds = new Rectangle(point.X + 96 /*0x60*/, point.Y + 440, 88, 80 /*0x50*/);
      string hoverText = this.farmTypeHoverText[index];
      farmTypeIcon = this.farmTypeIcons[index];
      Texture2D key = farmTypeIcon.Key;
      farmTypeIcon = this.farmTypeIcons[index];
      Rectangle sourceRect = farmTypeIcon.Value;
      ClickableTextureComponent textureComponent = new ClickableTextureComponent(farmTypeButtonName, bounds, (string) null, hoverText, key, sourceRect, 4f);
      textureComponent.myID = 550;
      textureComponent.upNeighborID = -99998;
      textureComponent.leftNeighborID = -99998;
      textureComponent.rightNeighborID = -99998;
      textureComponent.downNeighborID = -99998;
      farmTypeButtons.Add(textureComponent);
      ++index;
    }
    if (index < this.farmTypeButtonNames.Count)
    {
      List<ClickableTextureComponent> farmTypeButtons = this.farmTypeButtons;
      string farmTypeButtonName = this.farmTypeButtonNames[index];
      Rectangle bounds = new Rectangle(point.X + 96 /*0x60*/, point.Y + 528, 88, 80 /*0x50*/);
      string hoverText = this.farmTypeHoverText[index];
      farmTypeIcon = this.farmTypeIcons[index];
      Texture2D key = farmTypeIcon.Key;
      farmTypeIcon = this.farmTypeIcons[index];
      Rectangle sourceRect = farmTypeIcon.Value;
      ClickableTextureComponent textureComponent = new ClickableTextureComponent(farmTypeButtonName, bounds, (string) null, hoverText, key, sourceRect, 4f);
      textureComponent.myID = 551;
      textureComponent.upNeighborID = -99998;
      textureComponent.leftNeighborID = -99998;
      textureComponent.rightNeighborID = -99998;
      textureComponent.downNeighborID = -99998;
      farmTypeButtons.Add(textureComponent);
      int num = index + 1;
    }
    this.farmTypePreviousPageButton = (ClickableTextureComponent) null;
    this.farmTypeNextPageButton = (ClickableTextureComponent) null;
    if (this._currentFarmPage > 0)
    {
      ClickableTextureComponent textureComponent = new ClickableTextureComponent("", new Rectangle(point.X - 64 /*0x40*/ + 16 /*0x10*/, point.Y + 352 + 12, 64 /*0x40*/, 64 /*0x40*/), (string) null, "", Game1.mouseCursors, Game1.getSourceRectForStandardTileSheet(Game1.mouseCursors, 44), 1f);
      textureComponent.myID = 647;
      textureComponent.upNeighborID = -99998;
      textureComponent.leftNeighborID = -99998;
      textureComponent.rightNeighborID = -99998;
      textureComponent.downNeighborID = -99998;
      this.farmTypePreviousPageButton = textureComponent;
    }
    if (this._currentFarmPage >= this._farmPages - 1)
      return;
    ClickableTextureComponent textureComponent1 = new ClickableTextureComponent("", new Rectangle(point.X + 172, point.Y + 352 + 12, 64 /*0x40*/, 64 /*0x40*/), (string) null, "", Game1.mouseCursors, Game1.getSourceRectForStandardTileSheet(Game1.mouseCursors, 33), 1f);
    textureComponent1.myID = 647;
    textureComponent1.upNeighborID = -99998;
    textureComponent1.leftNeighborID = -99998;
    textureComponent1.rightNeighborID = -99998;
    textureComponent1.downNeighborID = -99998;
    this.farmTypeNextPageButton = textureComponent1;
  }

  public override void snapToDefaultClickableComponent()
  {
    if (this.showingCoopHelp)
      this.currentlySnappedComponent = this.getComponentWithID(626);
    else
      this.currentlySnappedComponent = this.getComponentWithID(521);
    this.snapCursorToCurrentSnappedComponent();
  }

  /// <inheritdoc />
  public override void gamePadButtonHeld(Buttons b)
  {
    base.gamePadButtonHeld(b);
    if (this.currentlySnappedComponent == null)
      return;
    switch (b)
    {
      case Buttons.DPadLeft:
      case Buttons.LeftThumbstickLeft:
        switch (this.currentlySnappedComponent.myID)
        {
          case 522:
            this.eyeColorPicker.LastColor = this.eyeColorPicker.getSelectedColor();
            this.eyeColorPicker.changeHue(-1);
            this.eyeColorPicker.Dirty = true;
            this._sliderOpTarget = this.eyeColorPicker;
            this._sliderAction = this._recolorEyesAction;
            return;
          case 523:
            this.eyeColorPicker.LastColor = this.eyeColorPicker.getSelectedColor();
            this.eyeColorPicker.changeSaturation(-1);
            this.eyeColorPicker.Dirty = true;
            this._sliderOpTarget = this.eyeColorPicker;
            this._sliderAction = this._recolorEyesAction;
            return;
          case 524:
            this.eyeColorPicker.LastColor = this.eyeColorPicker.getSelectedColor();
            this.eyeColorPicker.changeValue(-1);
            this.eyeColorPicker.Dirty = true;
            this._sliderOpTarget = this.eyeColorPicker;
            this._sliderAction = this._recolorEyesAction;
            return;
          case 525:
            this.hairColorPicker.LastColor = this.hairColorPicker.getSelectedColor();
            this.hairColorPicker.changeHue(-1);
            this.hairColorPicker.Dirty = true;
            this._sliderOpTarget = this.hairColorPicker;
            this._sliderAction = this._recolorHairAction;
            return;
          case 526:
            this.hairColorPicker.LastColor = this.hairColorPicker.getSelectedColor();
            this.hairColorPicker.changeSaturation(-1);
            this.hairColorPicker.Dirty = true;
            this._sliderOpTarget = this.hairColorPicker;
            this._sliderAction = this._recolorHairAction;
            return;
          case 527:
            this.hairColorPicker.LastColor = this.hairColorPicker.getSelectedColor();
            this.hairColorPicker.changeValue(-1);
            this.hairColorPicker.Dirty = true;
            this._sliderOpTarget = this.hairColorPicker;
            this._sliderAction = this._recolorHairAction;
            return;
          case 528:
            this.pantsColorPicker.LastColor = this.pantsColorPicker.getSelectedColor();
            this.pantsColorPicker.changeHue(-1);
            this.pantsColorPicker.Dirty = true;
            this._sliderOpTarget = this.pantsColorPicker;
            this._sliderAction = this._recolorPantsAction;
            return;
          case 529:
            this.pantsColorPicker.LastColor = this.pantsColorPicker.getSelectedColor();
            this.pantsColorPicker.changeSaturation(-1);
            this.pantsColorPicker.Dirty = true;
            this._sliderOpTarget = this.pantsColorPicker;
            this._sliderAction = this._recolorPantsAction;
            return;
          case 530:
            this.pantsColorPicker.LastColor = this.pantsColorPicker.getSelectedColor();
            this.pantsColorPicker.changeValue(-1);
            this.pantsColorPicker.Dirty = true;
            this._sliderOpTarget = this.pantsColorPicker;
            this._sliderAction = this._recolorPantsAction;
            return;
          default:
            return;
        }
      case Buttons.DPadRight:
      case Buttons.LeftThumbstickRight:
        switch (this.currentlySnappedComponent.myID)
        {
          case 522:
            this.eyeColorPicker.LastColor = this.eyeColorPicker.getSelectedColor();
            this.eyeColorPicker.changeHue(1);
            this.eyeColorPicker.Dirty = true;
            this._sliderOpTarget = this.eyeColorPicker;
            this._sliderAction = this._recolorEyesAction;
            return;
          case 523:
            this.eyeColorPicker.LastColor = this.eyeColorPicker.getSelectedColor();
            this.eyeColorPicker.changeSaturation(1);
            this.eyeColorPicker.Dirty = true;
            this._sliderOpTarget = this.eyeColorPicker;
            this._sliderAction = this._recolorEyesAction;
            return;
          case 524:
            this.eyeColorPicker.LastColor = this.eyeColorPicker.getSelectedColor();
            this.eyeColorPicker.changeValue(1);
            this.eyeColorPicker.Dirty = true;
            this._sliderOpTarget = this.eyeColorPicker;
            this._sliderAction = this._recolorEyesAction;
            return;
          case 525:
            this.hairColorPicker.LastColor = this.hairColorPicker.getSelectedColor();
            this.hairColorPicker.changeHue(1);
            this.hairColorPicker.Dirty = true;
            this._sliderOpTarget = this.hairColorPicker;
            this._sliderAction = this._recolorHairAction;
            return;
          case 526:
            this.hairColorPicker.LastColor = this.hairColorPicker.getSelectedColor();
            this.hairColorPicker.changeSaturation(1);
            this.hairColorPicker.Dirty = true;
            this._sliderOpTarget = this.hairColorPicker;
            this._sliderAction = this._recolorHairAction;
            return;
          case 527:
            this.hairColorPicker.LastColor = this.hairColorPicker.getSelectedColor();
            this.hairColorPicker.changeValue(1);
            this.hairColorPicker.Dirty = true;
            this._sliderOpTarget = this.hairColorPicker;
            this._sliderAction = this._recolorHairAction;
            return;
          case 528:
            this.pantsColorPicker.LastColor = this.pantsColorPicker.getSelectedColor();
            this.pantsColorPicker.changeHue(1);
            this.pantsColorPicker.Dirty = true;
            this._sliderOpTarget = this.pantsColorPicker;
            this._sliderAction = this._recolorPantsAction;
            return;
          case 529:
            this.pantsColorPicker.LastColor = this.pantsColorPicker.getSelectedColor();
            this.pantsColorPicker.changeSaturation(1);
            this.pantsColorPicker.Dirty = true;
            this._sliderOpTarget = this.pantsColorPicker;
            this._sliderAction = this._recolorPantsAction;
            return;
          case 530:
            this.pantsColorPicker.LastColor = this.pantsColorPicker.getSelectedColor();
            this.pantsColorPicker.changeValue(1);
            this.pantsColorPicker.Dirty = true;
            this._sliderOpTarget = this.pantsColorPicker;
            this._sliderAction = this._recolorPantsAction;
            return;
          default:
            return;
        }
    }
  }

  /// <inheritdoc />
  public override void receiveGamePadButton(Buttons button)
  {
    base.receiveGamePadButton(button);
    if (this.currentlySnappedComponent == null)
      return;
    switch (button)
    {
      case Buttons.B:
        if (!this.showingCoopHelp)
          break;
        this.receiveLeftClick(this.coopHelpOkButton.bounds.Center.X, this.coopHelpOkButton.bounds.Center.Y, true);
        break;
      case Buttons.RightTrigger:
        switch (this.currentlySnappedComponent.myID)
        {
          case 512 /*0x0200*/:
          case 513:
          case 514:
          case 515:
          case 516:
          case 517:
          case 518:
          case 519:
          case 520:
          case 521:
            this.selectionClick(this.currentlySnappedComponent.name, 1);
            return;
          default:
            return;
        }
      case Buttons.LeftTrigger:
        switch (this.currentlySnappedComponent.myID)
        {
          case 512 /*0x0200*/:
          case 513:
          case 514:
          case 515:
          case 516:
          case 517:
          case 518:
          case 519:
          case 520:
          case 521:
            this.selectionClick(this.currentlySnappedComponent.name, -1);
            return;
          default:
            return;
        }
    }
  }

  private void optionButtonClick(string name)
  {
    if (name.StartsWith("ModFarm_"))
    {
      if (this.source == CharacterCustomization.Source.NewGame || this.source == CharacterCustomization.Source.HostNewFarm)
      {
        List<ModFarmType> modFarmTypeList = DataLoader.AdditionalFarms(Game1.content);
        string str = name.Substring("ModFarm_".Length);
        foreach (ModFarmType modFarmType in modFarmTypeList)
        {
          if (modFarmType.Id == str)
          {
            Game1.whichFarm = 7;
            Game1.whichModFarm = modFarmType;
            Game1.spawnMonstersAtNight = modFarmType.SpawnMonstersByDefault;
            break;
          }
        }
      }
    }
    else if (name != null)
    {
      switch (name.Length)
      {
        case 2:
          if (name == "OK")
          {
            if (!this.canLeaveMenu())
              return;
            if (this._itemToDye != null)
            {
              if (!Game1.player.IsEquippedItem((Item) this._itemToDye))
                Utility.CollectOrDrop((Item) this._itemToDye);
              this._itemToDye = (Clothing) null;
            }
            if (this.source == CharacterCustomization.Source.ClothesDye)
            {
              Game1.exitActiveMenu();
              break;
            }
            Game1.player.Name = Utility.FilterDirtyWordsIfStrictPlatform(this.nameBox.Text.Trim());
            Game1.player.displayName = Game1.player.Name;
            Game1.player.favoriteThing.Value = Utility.FilterDirtyWordsIfStrictPlatform(this.favThingBox.Text.Trim());
            Game1.player.isCustomized.Value = true;
            Game1.player.ConvertClothingOverrideToClothesItems();
            if (this.source == CharacterCustomization.Source.HostNewFarm)
              Game1.multiplayerMode = (byte) 2;
            try
            {
              if (Game1.player.Name != this.oldName)
              {
                int num1 = Game1.player.Name.IndexOf("[");
                int num2 = Game1.player.Name.IndexOf("]");
                if (num1 >= 0)
                {
                  if (num2 > num1)
                  {
                    string displayName = ItemRegistry.GetData(Game1.player.Name.Substring(num1 + 1, num2 - num1 - 1))?.DisplayName;
                    if (displayName != null)
                    {
                      switch (Game1.random.Next(5))
                      {
                        case 0:
                          Game1.chatBox.addMessage(Game1.content.LoadString("Strings\\UI:NameChange_EasterEgg1"), new Color(104, 214, (int) byte.MaxValue));
                          break;
                        case 1:
                          Game1.chatBox.addMessage(Game1.content.LoadString("Strings\\UI:NameChange_EasterEgg2", (object) Lexicon.makePlural(displayName)), new Color(100, 50, (int) byte.MaxValue));
                          break;
                        case 2:
                          Game1.chatBox.addMessage(Game1.content.LoadString("Strings\\UI:NameChange_EasterEgg3", (object) Lexicon.makePlural(displayName)), new Color(0, 220, 40));
                          break;
                        case 3:
                          Game1.chatBox.addMessage(Game1.content.LoadString("Strings\\UI:NameChange_EasterEgg4"), new Color(0, 220, 40));
                          DelayedAction.functionAfterDelay((Action) (() => Game1.chatBox.addMessage(Game1.content.LoadString("Strings\\UI:NameChange_EasterEgg5"), new Color(104, 214, (int) byte.MaxValue))), 12000);
                          break;
                        case 4:
                          Game1.chatBox.addMessage(Game1.content.LoadString("Strings\\UI:NameChange_EasterEgg6", (object) Lexicon.getProperArticleForWord(displayName), (object) displayName), new Color(100, 120, (int) byte.MaxValue));
                          break;
                      }
                    }
                  }
                }
              }
            }
            catch
            {
            }
            string str = (string) null;
            if (this.petPortraitBox.HasValue && Game1.IsMasterGame && Game1.gameMode == (byte) 3 && Game1.locations != null)
            {
              Pet characterFromName = Game1.getCharacterFromName<Pet>(Game1.player.getPetName(), false);
              if (characterFromName != null && this.petHasChanges(characterFromName))
              {
                characterFromName.petType.Value = Game1.player.whichPetType;
                characterFromName.whichBreed.Value = Game1.player.whichPetBreed;
                str = characterFromName.getName();
              }
            }
            if (Game1.activeClickableMenu is TitleMenu activeClickableMenu)
            {
              activeClickableMenu.createdNewCharacter(this.skipIntro);
              break;
            }
            Game1.exitActiveMenu();
            if (Game1.currentMinigame is Intro currentMinigame)
            {
              currentMinigame.doneCreatingCharacter();
              break;
            }
            switch (this.source)
            {
              case CharacterCustomization.Source.Wizard:
                if (str != null)
                  Game1.multiplayer.globalChatInfoMessage("Makeover_Pet", Game1.player.Name, str);
                else
                  Game1.multiplayer.globalChatInfoMessage("Makeover", Game1.player.Name);
                Game1.flashAlpha = 1f;
                Game1.playSound("yoba");
                break;
              case CharacterCustomization.Source.ClothesDye:
                Game1.playSound("yoba");
                break;
            }
          }
          else
            break;
          break;
        case 3:
          switch (name[0])
          {
            case 'C':
              if (name == "Cat" && (this.source == CharacterCustomization.Source.NewGame || this.source == CharacterCustomization.Source.HostNewFarm))
              {
                Game1.player.whichPetType = "Cat";
                break;
              }
              break;
            case 'D':
              if (name == "Dog" && (this.source == CharacterCustomization.Source.NewGame || this.source == CharacterCustomization.Source.HostNewFarm))
              {
                Game1.player.whichPetType = "Dog";
                break;
              }
              break;
          }
          break;
        case 4:
          if (name == "Male")
          {
            Game1.player.changeGender(true);
            if (this.source != CharacterCustomization.Source.Wizard)
            {
              Game1.player.changeHairStyle(0);
              break;
            }
            break;
          }
          break;
        case 5:
          switch (name[0])
          {
            case 'B':
              if (name == "Beach" && (this.source == CharacterCustomization.Source.NewGame || this.source == CharacterCustomization.Source.HostNewFarm))
              {
                Game1.whichFarm = 6;
                Game1.whichModFarm = (ModFarmType) null;
                Game1.spawnMonstersAtNight = false;
                break;
              }
              break;
            case 'C':
              if (name == "Close")
              {
                Game1.cabinsSeparate = false;
                break;
              }
              break;
            case 'H':
              if (name == "Hills" && (this.source == CharacterCustomization.Source.NewGame || this.source == CharacterCustomization.Source.HostNewFarm))
              {
                Game1.whichFarm = 3;
                Game1.whichModFarm = (ModFarmType) null;
                Game1.spawnMonstersAtNight = false;
                break;
              }
              break;
          }
          break;
        case 6:
          switch (name[1])
          {
            case 'e':
              if (name == "Female")
              {
                Game1.player.changeGender(false);
                if (this.source != CharacterCustomization.Source.Wizard)
                {
                  Game1.player.changeHairStyle(16 /*0x10*/);
                  break;
                }
                break;
              }
              break;
            case 'o':
              if (name == "Forest" && (this.source == CharacterCustomization.Source.NewGame || this.source == CharacterCustomization.Source.HostNewFarm))
              {
                Game1.whichFarm = 2;
                Game1.whichModFarm = (ModFarmType) null;
                Game1.spawnMonstersAtNight = false;
                break;
              }
              break;
          }
          break;
        case 8:
          switch (name[1])
          {
            case 'e':
              if (name == "Separate")
              {
                Game1.cabinsSeparate = true;
                break;
              }
              break;
            case 't':
              if (name == "Standard" && (this.source == CharacterCustomization.Source.NewGame || this.source == CharacterCustomization.Source.HostNewFarm))
              {
                Game1.whichFarm = 0;
                Game1.whichModFarm = (ModFarmType) null;
                Game1.spawnMonstersAtNight = false;
                break;
              }
              break;
          }
          break;
        case 9:
          if (name == "Riverland" && (this.source == CharacterCustomization.Source.NewGame || this.source == CharacterCustomization.Source.HostNewFarm))
          {
            Game1.whichFarm = 1;
            Game1.whichModFarm = (ModFarmType) null;
            Game1.spawnMonstersAtNight = false;
            break;
          }
          break;
        case 10:
          if (name == "Wilderness" && (this.source == CharacterCustomization.Source.NewGame || this.source == CharacterCustomization.Source.HostNewFarm))
          {
            Game1.whichFarm = 4;
            Game1.whichModFarm = (ModFarmType) null;
            Game1.spawnMonstersAtNight = true;
            break;
          }
          break;
        case 12:
          if (name == "Four Corners" && (this.source == CharacterCustomization.Source.NewGame || this.source == CharacterCustomization.Source.HostNewFarm))
          {
            Game1.whichFarm = 5;
            Game1.whichModFarm = (ModFarmType) null;
            Game1.spawnMonstersAtNight = false;
            break;
          }
          break;
      }
    }
    Game1.playSound("coin");
  }

  public bool petHasChanges(Pet pet)
  {
    return Game1.player.whichPetType != pet.petType.Value || Game1.player.whichPetBreed != pet.whichBreed.Value;
  }

  /// <summary>Load the tooltip translation for a farm type in the expected format.</summary>
  /// <param name="translationKey">The translation key to load.</param>
  /// <remarks>This returns a tooltip string in the form <c>name_description</c>.</remarks>
  protected virtual string GetFarmTypeTooltip(string translationKey)
  {
    string farmTypeTooltip = Game1.content.LoadString(translationKey);
    string[] strArray = farmTypeTooltip.Split('_', 2);
    if (strArray.Length == 1 || strArray[1].Length == 0)
      farmTypeTooltip = strArray[0] + "_ ";
    return farmTypeTooltip;
  }

  protected List<KeyValuePair<string, string>> GetPetTypesAndBreeds()
  {
    if (this._petTypesAndBreeds == null)
    {
      this._petTypesAndBreeds = new List<KeyValuePair<string, string>>();
      foreach (KeyValuePair<string, PetData> keyValuePair in (IEnumerable<KeyValuePair<string, PetData>>) Game1.petData)
      {
        if (!this.isModifyingExistingPet || !(Game1.player.whichPetType != keyValuePair.Key))
        {
          foreach (PetBreed breed in keyValuePair.Value.Breeds)
          {
            if (breed.CanBeChosenAtStart)
              this._petTypesAndBreeds.Add(new KeyValuePair<string, string>(keyValuePair.Key, breed.Id));
          }
        }
      }
    }
    return this._petTypesAndBreeds;
  }

  private void selectionClick(string name, int change)
  {
    if (name == null)
      return;
    switch (name.Length)
    {
      case 3:
        switch (name[0])
        {
          case 'A':
            if (!(name == "Acc"))
              return;
            Game1.player.changeAccessory(Game1.player.accessory.Value + change);
            Game1.playSound("purchase");
            return;
          case 'P':
            if (!(name == "Pet"))
              return;
            List<KeyValuePair<string, string>> petTypesAndBreeds = this.GetPetTypesAndBreeds();
            int num = petTypesAndBreeds.IndexOf(new KeyValuePair<string, string>(Game1.player.whichPetType, Game1.player.whichPetBreed));
            int index1 = num != -1 ? num + change : 0;
            if (index1 < 0)
              index1 = petTypesAndBreeds.Count - 1;
            else if (index1 >= petTypesAndBreeds.Count)
              index1 = 0;
            KeyValuePair<string, string> keyValuePair = petTypesAndBreeds[index1];
            Game1.player.whichPetType = keyValuePair.Key;
            Game1.player.whichPetBreed = keyValuePair.Value;
            Game1.playSound("coin");
            return;
          default:
            return;
        }
      case 4:
        switch (name[0])
        {
          case 'H':
            if (!(name == "Hair"))
              return;
            List<int> hairstyleIndices = Farmer.GetAllHairstyleIndices();
            int index2 = hairstyleIndices.IndexOf(Game1.player.hair.Value) + change;
            if (index2 >= hairstyleIndices.Count)
              index2 = 0;
            else if (index2 < 0)
              index2 = hairstyleIndices.Count - 1;
            Game1.player.changeHairStyle(hairstyleIndices[index2]);
            Game1.playSound("grassyStep");
            return;
          case 'S':
            if (!(name == "Skin"))
              return;
            Game1.player.changeSkinColor(Game1.player.skin.Value + change);
            Game1.playSound("skeletonStep");
            return;
          default:
            return;
        }
      case 5:
        if (!(name == "Shirt"))
          break;
        Game1.player.rotateShirt(change, this.GetValidShirtIds());
        Game1.playSound("coin");
        break;
      case 6:
        if (!(name == "Cabins"))
          break;
        if ((Game1.startingCabins != 0 || change >= 0) && (Game1.startingCabins != Game1.multiplayer.playerLimit - 1 || change <= 0))
          Game1.playSound("axchop");
        Game1.startingCabins += change;
        Game1.startingCabins = Math.Max(0, Math.Min(Game1.multiplayer.playerLimit - 1, Game1.startingCabins));
        break;
      case 7:
        if (!(name == "Wallets"))
          break;
        if (Game1.player.team.useSeparateWallets.Value)
        {
          Game1.playSound("coin");
          Game1.player.team.useSeparateWallets.Value = false;
          break;
        }
        Game1.playSound("coin");
        Game1.player.team.useSeparateWallets.Value = true;
        break;
      case 9:
        if (!(name == "Direction"))
          break;
        this._displayFarmer.faceDirection((this._displayFarmer.FacingDirection - change + 4) % 4);
        this._displayFarmer.FarmerSprite.StopAnimation();
        this._displayFarmer.completelyStopAnimatingOrDoingAction();
        Game1.playSound("pickUpItem");
        break;
      case 10:
        if (!(name == "Difficulty"))
          break;
        if ((double) Game1.player.difficultyModifier < 1.0 && change < 0)
        {
          Game1.playSound("breathout");
          Game1.player.difficultyModifier += 0.25f;
          break;
        }
        if ((double) Game1.player.difficultyModifier <= 0.25 || change <= 0)
          break;
        Game1.playSound("batFlap");
        Game1.player.difficultyModifier -= 0.25f;
        break;
      case 11:
        if (!(name == "Pants Style"))
          break;
        Game1.player.rotatePantStyle(change, this.GetValidPantsIds());
        Game1.playSound("coin");
        break;
    }
  }

  public void ShowAdvancedOptions()
  {
    this.AddDependency();
    (TitleMenu.subMenu = (IClickableMenu) new AdvancedGameOptions()).exitFunction = (IClickableMenu.onExit) (() =>
    {
      TitleMenu.subMenu = (IClickableMenu) this;
      this.RemoveDependency();
      this.ResetComponents();
      this.populateClickableComponentList();
      if (!Game1.options.SnappyMenus)
        return;
      this.setCurrentlySnappedComponentTo(636);
      this.snapCursorToCurrentSnappedComponent();
    });
  }

  public override bool readyToClose()
  {
    if (this.showingCoopHelp)
      return false;
    if (Game1.lastCursorMotionWasMouse)
    {
      foreach (ClickableComponent farmTypeButton in this.farmTypeButtons)
      {
        if (farmTypeButton.containsPoint(Game1.getMouseX(true), Game1.getMouseY(true)))
          return false;
      }
    }
    return base.readyToClose();
  }

  /// <inheritdoc />
  public override void receiveLeftClick(int x, int y, bool playSound = true)
  {
    if (this.showingCoopHelp)
    {
      if (this.coopHelpOkButton != null && this.coopHelpOkButton.containsPoint(x, y))
      {
        this.showingCoopHelp = false;
        Game1.playSound("bigDeSelect");
        if (Game1.options.SnappyMenus)
        {
          this.currentlySnappedComponent = (ClickableComponent) this.coopHelpButton;
          this.snapCursorToCurrentSnappedComponent();
        }
      }
      if (this.coopHelpScreen == 0 && this.coopHelpRightButton != null && this.coopHelpRightButton.containsPoint(x, y))
      {
        ++this.coopHelpScreen;
        this.coopHelpString = Game1.parseText(Game1.content.LoadString("Strings\\UI:Character_CoopHelpString2").Replace("^", Environment.NewLine), Game1.dialogueFont, this.width + 384 - IClickableMenu.borderWidth * 2);
        Game1.playSound("shwip");
      }
      if (this.coopHelpScreen != 1 || this.coopHelpLeftButton == null || !this.coopHelpLeftButton.containsPoint(x, y))
        return;
      --this.coopHelpScreen;
      this.coopHelpString = Game1.parseText(string.Format(Game1.content.LoadString("Strings\\UI:Character_CoopHelpString").Replace("^", Environment.NewLine), (object) (Game1.multiplayer.playerLimit - 1)), Game1.dialogueFont, this.width + 384 - IClickableMenu.borderWidth * 2);
      Game1.playSound("shwip");
    }
    else
    {
      if (this.genderButtons.Count > 0)
      {
        foreach (ClickableComponent genderButton in this.genderButtons)
        {
          if (genderButton.containsPoint(x, y))
          {
            this.optionButtonClick(genderButton.name);
            genderButton.scale -= 0.5f;
            genderButton.scale = Math.Max(3.5f, genderButton.scale);
          }
        }
      }
      if (this.farmTypeNextPageButton != null && this.farmTypeNextPageButton.containsPoint(x, y))
      {
        Game1.playSound("shwip");
        ++this._currentFarmPage;
        this.RefreshFarmTypeButtons();
      }
      else if (this.farmTypePreviousPageButton != null && this.farmTypePreviousPageButton.containsPoint(x, y))
      {
        Game1.playSound("shwip");
        --this._currentFarmPage;
        this.RefreshFarmTypeButtons();
      }
      else if (this.farmTypeButtons.Count > 0)
      {
        foreach (ClickableComponent farmTypeButton in this.farmTypeButtons)
        {
          if (farmTypeButton.containsPoint(x, y) && !farmTypeButton.name.Contains("Gray"))
          {
            this.optionButtonClick(farmTypeButton.name);
            farmTypeButton.scale -= 0.5f;
            farmTypeButton.scale = Math.Max(3.5f, farmTypeButton.scale);
          }
        }
      }
      if (this.cabinLayoutButtons.Count > 0)
      {
        foreach (ClickableComponent cabinLayoutButton in this.cabinLayoutButtons)
        {
          if (Game1.startingCabins > 0 && cabinLayoutButton.containsPoint(x, y))
          {
            this.optionButtonClick(cabinLayoutButton.name);
            cabinLayoutButton.scale -= 0.5f;
            cabinLayoutButton.scale = Math.Max(3.5f, cabinLayoutButton.scale);
          }
        }
      }
      if (this.leftSelectionButtons.Count > 0)
      {
        foreach (ClickableComponent leftSelectionButton in this.leftSelectionButtons)
        {
          if (leftSelectionButton.containsPoint(x, y))
          {
            this.selectionClick(leftSelectionButton.name, -1);
            if ((double) leftSelectionButton.scale != 0.0)
            {
              leftSelectionButton.scale -= 0.25f;
              leftSelectionButton.scale = Math.Max(0.75f, leftSelectionButton.scale);
            }
          }
        }
      }
      if (this.rightSelectionButtons.Count > 0)
      {
        foreach (ClickableComponent rightSelectionButton in this.rightSelectionButtons)
        {
          if (rightSelectionButton.containsPoint(x, y))
          {
            this.selectionClick(rightSelectionButton.name, 1);
            if ((double) rightSelectionButton.scale != 0.0)
            {
              rightSelectionButton.scale -= 0.25f;
              rightSelectionButton.scale = Math.Max(0.75f, rightSelectionButton.scale);
            }
          }
        }
      }
      if (this.okButton.containsPoint(x, y) && this.canLeaveMenu())
      {
        this.optionButtonClick(this.okButton.name);
        this.okButton.scale -= 0.25f;
        this.okButton.scale = Math.Max(0.75f, this.okButton.scale);
      }
      if (this.hairColorPicker != null && this.hairColorPicker.containsPoint(x, y))
      {
        Color c = this.hairColorPicker.click(x, y);
        if (this.source == CharacterCustomization.Source.DyePots)
        {
          if (Game1.player.CanDyeShirt())
          {
            Game1.player.shirtItem.Value.clothesColor.Value = c;
            Game1.player.FarmerRenderer.MarkSpriteDirty();
            this._displayFarmer.FarmerRenderer.MarkSpriteDirty();
          }
        }
        else
          Game1.player.changeHairColor(c);
        this.lastHeldColorPicker = this.hairColorPicker;
      }
      else if (this.pantsColorPicker != null && this.pantsColorPicker.containsPoint(x, y))
      {
        Color color = this.pantsColorPicker.click(x, y);
        switch (this.source)
        {
          case CharacterCustomization.Source.ClothesDye:
            this.DyeItem(color);
            break;
          case CharacterCustomization.Source.DyePots:
            if (Game1.player.CanDyePants())
            {
              Game1.player.pantsItem.Value.clothesColor.Value = color;
              Game1.player.FarmerRenderer.MarkSpriteDirty();
              this._displayFarmer.FarmerRenderer.MarkSpriteDirty();
              break;
            }
            break;
          default:
            Game1.player.changePantsColor(color);
            break;
        }
        this.lastHeldColorPicker = this.pantsColorPicker;
      }
      else if (this.eyeColorPicker != null && this.eyeColorPicker.containsPoint(x, y))
      {
        Game1.player.changeEyeColor(this.eyeColorPicker.click(x, y));
        this.lastHeldColorPicker = this.eyeColorPicker;
      }
      if (this.source != CharacterCustomization.Source.Dresser && this.source != CharacterCustomization.Source.ClothesDye && this.source != CharacterCustomization.Source.DyePots)
      {
        this.nameBox.Update();
        if (this.source == CharacterCustomization.Source.NewGame || this.source == CharacterCustomization.Source.HostNewFarm)
          this.farmnameBox.Update();
        else
          this.farmnameBox.Text = Game1.MasterPlayer.farmName.Value;
        this.favThingBox.Update();
        if ((this.source == CharacterCustomization.Source.NewGame || this.source == CharacterCustomization.Source.HostNewFarm) && this.skipIntroButton.containsPoint(x, y))
        {
          Game1.playSound("drumkit6");
          this.skipIntroButton.sourceRect.X = this.skipIntroButton.sourceRect.X == 227 ? 236 : 227;
          this.skipIntro = !this.skipIntro;
        }
      }
      if (this.coopHelpButton != null && this.coopHelpButton.containsPoint(x, y))
      {
        if (Game1.options.SnappyMenus)
        {
          this.currentlySnappedComponent = (ClickableComponent) this.coopHelpOkButton;
          this.snapCursorToCurrentSnappedComponent();
        }
        Game1.playSound("bigSelect");
        this.showingCoopHelp = true;
        this.coopHelpScreen = 0;
        this.coopHelpString = Game1.parseText(string.Format(Game1.content.LoadString("Strings\\UI:Character_CoopHelpString").Replace("^", Environment.NewLine), (object) (Game1.multiplayer.playerLimit - 1)), Game1.dialogueFont, this.width + 384 - IClickableMenu.borderWidth * 2);
        this.helpStringSize = Game1.dialogueFont.MeasureString(this.coopHelpString);
        this.coopHelpRightButton.bounds.Y = this.yPositionOnScreen + (int) this.helpStringSize.Y + IClickableMenu.borderWidth * 2 - 4;
        this.coopHelpRightButton.bounds.X = this.xPositionOnScreen + (int) this.helpStringSize.X - IClickableMenu.borderWidth * 5;
        this.coopHelpLeftButton.bounds.Y = this.yPositionOnScreen + (int) this.helpStringSize.Y + IClickableMenu.borderWidth * 2 - 4;
        this.coopHelpLeftButton.bounds.X = this.xPositionOnScreen - IClickableMenu.borderWidth * 4;
      }
      if (this.advancedOptionsButton != null && this.advancedOptionsButton.containsPoint(x, y))
      {
        Game1.playSound("drumkit6");
        this.ShowAdvancedOptions();
      }
      if (!this.randomButton.containsPoint(x, y))
        return;
      string cueName = "drumkit6";
      if (this.timesRandom > 0)
      {
        switch (Game1.random.Next(15))
        {
          case 0:
            cueName = "drumkit1";
            break;
          case 1:
            cueName = "dirtyHit";
            break;
          case 2:
            cueName = "axchop";
            break;
          case 3:
            cueName = "hoeHit";
            break;
          case 4:
            cueName = "fishSlap";
            break;
          case 5:
            cueName = "drumkit6";
            break;
          case 6:
            cueName = "drumkit5";
            break;
          case 7:
            cueName = "drumkit6";
            break;
          case 8:
            cueName = "junimoMeep1";
            break;
          case 9:
            cueName = "coin";
            break;
          case 10:
            cueName = "axe";
            break;
          case 11:
            cueName = "hammer";
            break;
          case 12:
            cueName = "drumkit2";
            break;
          case 13:
            cueName = "drumkit4";
            break;
          case 14:
            cueName = "drumkit3";
            break;
        }
      }
      Game1.playSound(cueName);
      ++this.timesRandom;
      if (this.accLabel != null && this.accLabel.visible)
      {
        if (Game1.random.NextDouble() < 0.33)
        {
          if (Game1.player.IsMale)
          {
            if (Game1.random.NextDouble() < 0.33)
            {
              if (Game1.random.NextDouble() < 0.8)
                Game1.player.changeAccessory(Game1.random.Next(7));
              else
                Game1.player.changeAccessory(Game1.random.Next(19, 21));
            }
            else if (Game1.random.NextDouble() < 0.33)
              Game1.player.changeAccessory(Game1.random.Choose<int>(25, 14, 17, 10, 9));
            else if (Game1.random.NextDouble() < 0.1)
              Game1.player.changeAccessory(Game1.random.Next(19));
          }
          else if (Game1.random.NextDouble() < 0.33)
            Game1.player.changeAccessory(Game1.random.Next(6, 19));
          else if (Game1.random.NextDouble() < 0.5)
            Game1.player.changeAccessory(Game1.random.Choose<int>(23, 27, 28));
          else
            Game1.player.changeAccessory(Game1.random.Choose<int>(25, 14, 17, 10, 9));
        }
        else
          Game1.player.changeAccessory(-1);
      }
      if (this.skinLabel != null && this.skinLabel.visible)
      {
        Game1.player.changeSkinColor(Game1.random.Next(6));
        if (Game1.random.NextDouble() < 0.15)
          Game1.player.changeSkinColor(Game1.random.Next(24));
      }
      if (this.hairLabel != null && this.hairLabel.visible)
      {
        if (Game1.player.IsMale)
          Game1.player.changeHairStyle(Game1.random.NextBool() ? Game1.random.Next(16 /*0x10*/) : Game1.random.Next(108, 118));
        else
          Game1.player.changeHairStyle(Game1.random.Next(16 /*0x10*/, 41));
        Color color = new Color(Game1.random.Next(25, 254), Game1.random.Next(25, 254), Game1.random.Next(25, 254));
        if (Game1.random.NextBool())
        {
          color.R /= (byte) 2;
          color.G /= (byte) 2;
          color.B /= (byte) 2;
        }
        if (Game1.random.NextBool())
          color.R = (byte) Game1.random.Next(15, 50);
        if (Game1.random.NextBool())
          color.G = (byte) Game1.random.Next(15, 50);
        if (Game1.random.NextBool())
          color.B = (byte) Game1.random.Next(15, 50);
        if (Game1.random.NextBool())
        {
          if ((int) color.B > (int) color.R)
            color.B = (byte) Math.Max(0, (int) color.B - 50);
          if ((int) color.B > (int) color.G)
            color.B = (byte) Math.Max(0, (int) color.B - 50);
          if ((int) color.G > (int) color.R)
            color.G = (byte) Math.Max(0, (int) color.R - 50);
          color.R = (byte) Math.Min((int) byte.MaxValue, (int) color.R + 50);
          color.G = (byte) Math.Min((int) byte.MaxValue, (int) color.G + 50);
        }
        else if (Game1.random.NextDouble() < 0.33)
          color = new Color(Game1.random.Next(80 /*0x50*/, 130), Game1.random.Next(35, 70), 0);
        if (color.R < (byte) 100 && color.G < (byte) 100 && color.B < (byte) 100 && Game1.random.NextDouble() < 0.8)
          color = Utility.getBlendedColor(color, Color.Tan);
        if (Game1.player.hasDarkSkin() && Game1.random.NextDouble() < 0.5)
          color = new Color(Game1.random.Next(50, 100), Game1.random.Next(25, 40), 0);
        Game1.player.changeHairColor(color);
        this.hairColorPicker.setColor(color);
      }
      if (this.shirtLabel != null && this.shirtLabel.visible)
      {
        string itemId = "";
        List<string> validShirtIds = this.GetValidShirtIds();
        HashSet<string> except;
        if (!Game1.player.IsMale)
        {
          except = new HashSet<string>();
        }
        else
        {
          except = new HashSet<string>();
          except.Add("1056");
          except.Add("1057");
          except.Add("1070");
          except.Add("1046");
          except.Add("1040");
          except.Add("1060");
          except.Add("1090");
          except.Add("1051");
          except.Add("1082");
          except.Add("1107");
          except.Add("1080");
          except.Add("1083");
          except.Add("1092");
          except.Add("1072");
          except.Add("1076");
          except.Add("1041");
        }
        Random random = Game1.random;
        ref string local = ref itemId;
        Utility.TryGetRandomExcept<string>((IList<string>) validShirtIds, (ISet<string>) except, random, out local);
        Game1.player.changeShirt(itemId);
      }
      if (this.pantsStyleLabel != null && this.pantsStyleLabel.visible)
      {
        Color color = new Color(Game1.random.Next(25, 254), Game1.random.Next(25, 254), Game1.random.Next(25, 254));
        if (Game1.random.NextBool())
        {
          color.R /= (byte) 2;
          color.G /= (byte) 2;
          color.B /= (byte) 2;
        }
        if (Game1.random.NextBool())
          color.R = (byte) Game1.random.Next(15, 50);
        if (Game1.random.NextBool())
          color.G = (byte) Game1.random.Next(15, 50);
        if (Game1.random.NextBool())
          color.B = (byte) Game1.random.Next(15, 50);
        switch (Game1.player.GetShirtIndex())
        {
          case 0:
          case 7:
          case 71:
            color = new Color(34, 29, 173);
            break;
          case 50:
            color = new Color(226, 133, 160 /*0xA0*/);
            break;
          case 67:
          case 72:
            color = new Color(108, 134, 224 /*0xE0*/);
            break;
          case 68:
          case 88:
            color = new Color(119, 215, 130);
            break;
          case 79:
          case 99:
          case 103:
            color = new Color(55, 55, 60);
            break;
        }
        Game1.player.changePantsColor(color);
        this.pantsColorPicker.setColor(Game1.player.GetPantsColor());
      }
      if (this.eyeColorPicker != null)
      {
        Color c = new Color(Game1.random.Next(25, 254), Game1.random.Next(25, 254), Game1.random.Next(25, 254));
        c.R /= (byte) 2;
        c.G /= (byte) 2;
        c.B /= (byte) 2;
        if (Game1.random.NextBool())
          c.R = (byte) Game1.random.Next(15, 50);
        if (Game1.random.NextBool())
          c.G = (byte) Game1.random.Next(15, 50);
        if (Game1.random.NextBool())
          c.B = (byte) Game1.random.Next(15, 50);
        if (Game1.random.NextBool())
        {
          if ((int) c.B > (int) c.R)
            c.B = (byte) Math.Max(0, (int) c.B - 50);
          if ((int) c.B > (int) c.G)
            c.B = (byte) Math.Max(0, (int) c.B - 50);
          if ((int) c.G > (int) c.R)
            c.G = (byte) Math.Max(0, (int) c.R - 50);
        }
        Game1.player.changeEyeColor(c);
        this.eyeColorPicker.setColor(Game1.player.newEyeColor.Value);
      }
      this.randomButton.scale = 3.5f;
    }
  }

  /// <summary>Get the shirts or pants which can be selected on the character customization screen.</summary>
  /// <typeparam name="TData">The clothing data.</typeparam>
  /// <param name="equippedId">The unqualified item ID for the item equipped by the player.</param>
  /// <param name="data">The data to search.</param>
  /// <param name="canChooseDuringCharacterCustomization">Get whether a clothing item should be visible on the character customization screen.</param>
  public List<string> GetValidClothingIds<TData>(
    string equippedId,
    IDictionary<string, TData> data,
    Func<TData, bool> canChooseDuringCharacterCustomization)
  {
    List<string> validClothingIds = new List<string>();
    foreach (KeyValuePair<string, TData> keyValuePair in (IEnumerable<KeyValuePair<string, TData>>) data)
    {
      if (keyValuePair.Key == equippedId || canChooseDuringCharacterCustomization(keyValuePair.Value))
        validClothingIds.Add(keyValuePair.Key);
    }
    return validClothingIds;
  }

  /// <summary>Get the pants which can be selected on the character customization screen.</summary>
  public List<string> GetValidPantsIds()
  {
    return this.GetValidClothingIds<PantsData>(Game1.player.pants.Value, Game1.pantsData, (Func<PantsData, bool>) (data => data.CanChooseDuringCharacterCustomization));
  }

  /// <summary>Get the shirts which can be selected on the character customization screen.</summary>
  public List<string> GetValidShirtIds()
  {
    return this.GetValidClothingIds<ShirtData>(Game1.player.shirt.Value, Game1.shirtData, (Func<ShirtData, bool>) (data => data.CanChooseDuringCharacterCustomization));
  }

  /// <inheritdoc />
  public override void leftClickHeld(int x, int y)
  {
    this.colorPickerTimer -= Game1.currentGameTime.ElapsedGameTime.Milliseconds;
    if (this.colorPickerTimer > 0)
      return;
    if (this.lastHeldColorPicker != null && !Game1.options.SnappyMenus)
    {
      if (this.lastHeldColorPicker.Equals((object) this.hairColorPicker))
      {
        Color c = this.hairColorPicker.clickHeld(x, y);
        if (this.source == CharacterCustomization.Source.DyePots)
        {
          if (Game1.player.CanDyeShirt())
          {
            Game1.player.shirtItem.Value.clothesColor.Value = c;
            Game1.player.FarmerRenderer.MarkSpriteDirty();
            this._displayFarmer.FarmerRenderer.MarkSpriteDirty();
          }
        }
        else
          Game1.player.changeHairColor(c);
      }
      if (this.lastHeldColorPicker.Equals((object) this.pantsColorPicker))
      {
        Color color = this.pantsColorPicker.clickHeld(x, y);
        switch (this.source)
        {
          case CharacterCustomization.Source.ClothesDye:
            this.DyeItem(color);
            break;
          case CharacterCustomization.Source.DyePots:
            if (Game1.player.CanDyePants())
            {
              Game1.player.pantsItem.Value.clothesColor.Value = color;
              Game1.player.FarmerRenderer.MarkSpriteDirty();
              this._displayFarmer.FarmerRenderer.MarkSpriteDirty();
              break;
            }
            break;
          default:
            Game1.player.changePantsColor(color);
            break;
        }
      }
      if (this.lastHeldColorPicker.Equals((object) this.eyeColorPicker))
        Game1.player.changeEyeColor(this.eyeColorPicker.clickHeld(x, y));
    }
    this.colorPickerTimer = 100;
  }

  /// <inheritdoc />
  public override void releaseLeftClick(int x, int y)
  {
    this.hairColorPicker?.releaseClick();
    this.pantsColorPicker?.releaseClick();
    this.eyeColorPicker?.releaseClick();
    this.lastHeldColorPicker = (ColorPicker) null;
  }

  /// <inheritdoc />
  public override void receiveKeyPress(Keys key)
  {
    if (key == Keys.Tab)
    {
      switch (this.source)
      {
        case CharacterCustomization.Source.NewGame:
        case CharacterCustomization.Source.HostNewFarm:
          if (this.nameBox.Selected)
          {
            this.farmnameBox.SelectMe();
            this.nameBox.Selected = false;
            break;
          }
          if (this.farmnameBox.Selected)
          {
            this.farmnameBox.Selected = false;
            this.favThingBox.SelectMe();
            break;
          }
          this.favThingBox.Selected = false;
          this.nameBox.SelectMe();
          break;
        case CharacterCustomization.Source.NewFarmhand:
          if (this.nameBox.Selected)
          {
            this.favThingBox.SelectMe();
            this.nameBox.Selected = false;
            break;
          }
          this.favThingBox.Selected = false;
          this.nameBox.SelectMe();
          break;
      }
    }
    if (!Game1.options.SnappyMenus || Game1.options.doesInputListContain(Game1.options.menuButton, key) || Game1.GetKeyboardState().GetPressedKeys().Length != 0)
      return;
    base.receiveKeyPress(key);
  }

  /// <inheritdoc />
  public override void performHoverAction(int x, int y)
  {
    this.hoverText = "";
    this.hoverTitle = "";
    foreach (ClickableTextureComponent leftSelectionButton in this.leftSelectionButtons)
    {
      if (leftSelectionButton.containsPoint(x, y))
        leftSelectionButton.scale = Math.Min(leftSelectionButton.scale + 0.02f, leftSelectionButton.baseScale + 0.1f);
      else
        leftSelectionButton.scale = Math.Max(leftSelectionButton.scale - 0.02f, leftSelectionButton.baseScale);
      if (leftSelectionButton.name.Equals("Cabins") && Game1.startingCabins == 0)
        leftSelectionButton.scale = 0.0f;
    }
    foreach (ClickableTextureComponent rightSelectionButton in this.rightSelectionButtons)
    {
      if (rightSelectionButton.containsPoint(x, y))
        rightSelectionButton.scale = Math.Min(rightSelectionButton.scale + 0.02f, rightSelectionButton.baseScale + 0.1f);
      else
        rightSelectionButton.scale = Math.Max(rightSelectionButton.scale - 0.02f, rightSelectionButton.baseScale);
      if (rightSelectionButton.name.Equals("Cabins") && Game1.startingCabins == Game1.multiplayer.playerLimit - 1)
        rightSelectionButton.scale = 0.0f;
    }
    if (this.source == CharacterCustomization.Source.NewGame || this.source == CharacterCustomization.Source.HostNewFarm)
    {
      foreach (ClickableTextureComponent farmTypeButton in this.farmTypeButtons)
      {
        if (farmTypeButton.containsPoint(x, y) && !farmTypeButton.name.Contains("Gray"))
        {
          farmTypeButton.scale = Math.Min(farmTypeButton.scale + 0.02f, farmTypeButton.baseScale + 0.1f);
          this.hoverTitle = farmTypeButton.hoverText.Split('_')[0];
          this.hoverText = farmTypeButton.hoverText.Split('_')[1];
        }
        else
        {
          farmTypeButton.scale = Math.Max(farmTypeButton.scale - 0.02f, farmTypeButton.baseScale);
          if (farmTypeButton.name.Contains("Gray") && farmTypeButton.containsPoint(x, y))
            this.hoverText = $"Reach level 10 {Game1.content.LoadString("Strings\\UI:Character_" + farmTypeButton.name.Split('_')[1])} to unlock.";
        }
      }
    }
    foreach (ClickableTextureComponent genderButton in this.genderButtons)
    {
      if (genderButton.containsPoint(x, y))
        genderButton.scale = Math.Min(genderButton.scale + 0.05f, genderButton.baseScale + 0.5f);
      else
        genderButton.scale = Math.Max(genderButton.scale - 0.05f, genderButton.baseScale);
    }
    if (this.source == CharacterCustomization.Source.NewGame || this.source == CharacterCustomization.Source.HostNewFarm)
    {
      foreach (ClickableTextureComponent cabinLayoutButton in this.cabinLayoutButtons)
      {
        if (Game1.startingCabins > 0 && cabinLayoutButton.containsPoint(x, y))
        {
          cabinLayoutButton.scale = Math.Min(cabinLayoutButton.scale + 0.05f, cabinLayoutButton.baseScale + 0.5f);
          this.hoverText = cabinLayoutButton.hoverText;
        }
        else
          cabinLayoutButton.scale = Math.Max(cabinLayoutButton.scale - 0.05f, cabinLayoutButton.baseScale);
      }
    }
    if (this.okButton.containsPoint(x, y) && this.canLeaveMenu())
      this.okButton.scale = Math.Min(this.okButton.scale + 0.02f, this.okButton.baseScale + 0.1f);
    else
      this.okButton.scale = Math.Max(this.okButton.scale - 0.02f, this.okButton.baseScale);
    if (this.coopHelpButton != null)
    {
      if (this.coopHelpButton.containsPoint(x, y))
      {
        this.coopHelpButton.scale = Math.Min(this.coopHelpButton.scale + 0.05f, this.coopHelpButton.baseScale + 0.5f);
        this.hoverText = this.coopHelpButton.hoverText;
      }
      else
        this.coopHelpButton.scale = Math.Max(this.coopHelpButton.scale - 0.05f, this.coopHelpButton.baseScale);
    }
    if (this.coopHelpOkButton != null)
    {
      if (this.coopHelpOkButton.containsPoint(x, y))
        this.coopHelpOkButton.scale = Math.Min(this.coopHelpOkButton.scale + 0.025f, this.coopHelpOkButton.baseScale + 0.2f);
      else
        this.coopHelpOkButton.scale = Math.Max(this.coopHelpOkButton.scale - 0.025f, this.coopHelpOkButton.baseScale);
    }
    if (this.coopHelpRightButton != null)
    {
      if (this.coopHelpRightButton.containsPoint(x, y))
        this.coopHelpRightButton.scale = Math.Min(this.coopHelpRightButton.scale + 0.025f, this.coopHelpRightButton.baseScale + 0.2f);
      else
        this.coopHelpRightButton.scale = Math.Max(this.coopHelpRightButton.scale - 0.025f, this.coopHelpRightButton.baseScale);
    }
    if (this.coopHelpLeftButton != null)
    {
      if (this.coopHelpLeftButton.containsPoint(x, y))
        this.coopHelpLeftButton.scale = Math.Min(this.coopHelpLeftButton.scale + 0.025f, this.coopHelpLeftButton.baseScale + 0.2f);
      else
        this.coopHelpLeftButton.scale = Math.Max(this.coopHelpLeftButton.scale - 0.025f, this.coopHelpLeftButton.baseScale);
    }
    this.advancedOptionsButton?.tryHover(x, y);
    this.farmTypeNextPageButton?.tryHover(x, y);
    this.farmTypePreviousPageButton?.tryHover(x, y);
    this.randomButton.tryHover(x, y, 0.25f);
    this.randomButton.tryHover(x, y, 0.25f);
    if (this.hairColorPicker != null && this.hairColorPicker.containsPoint(x, y) || this.pantsColorPicker != null && this.pantsColorPicker.containsPoint(x, y) || this.eyeColorPicker != null && this.eyeColorPicker.containsPoint(x, y))
      Game1.SetFreeCursorDrag();
    this.nameBox.Hover(x, y);
    this.farmnameBox.Hover(x, y);
    this.favThingBox.Hover(x, y);
    this.skipIntroButton.tryHover(x, y);
  }

  public bool canLeaveMenu()
  {
    if (this.source == CharacterCustomization.Source.ClothesDye || this.source == CharacterCustomization.Source.DyePots)
      return true;
    return Game1.player.Name.Length > 0 && Game1.player.farmName.Length > 0 && Game1.player.favoriteThing.Length > 0;
  }

  private string getNameOfDifficulty()
  {
    if ((double) Game1.player.difficultyModifier < 0.5)
      return this.superDiffString;
    if ((double) Game1.player.difficultyModifier < 0.75)
      return this.hardDiffString;
    return (double) Game1.player.difficultyModifier < 1.0 ? this.toughDiffString : this.normalDiffString;
  }

  /// <inheritdoc />
  public override void draw(SpriteBatch b)
  {
    if (this.showingCoopHelp)
    {
      IClickableMenu.drawTextureBox(b, this.xPositionOnScreen - 192 /*0xC0*/, this.yPositionOnScreen + 64 /*0x40*/, (int) this.helpStringSize.X + IClickableMenu.borderWidth * 2, (int) this.helpStringSize.Y + IClickableMenu.borderWidth * 2, Color.White);
      Utility.drawTextWithShadow(b, this.coopHelpString, Game1.dialogueFont, new Vector2((float) (this.xPositionOnScreen + IClickableMenu.borderWidth - 192 /*0xC0*/), (float) (this.yPositionOnScreen + IClickableMenu.borderWidth + 64 /*0x40*/)), Game1.textColor);
      this.coopHelpOkButton?.draw(b, Color.White, 0.95f);
      this.coopHelpRightButton?.draw(b, Color.White, 0.95f);
      this.coopHelpLeftButton?.draw(b, Color.White, 0.95f);
      this.drawMouse(b);
    }
    else
    {
      Game1.drawDialogueBox(this.xPositionOnScreen, this.yPositionOnScreen, this.width, this.height, false, true);
      if (this.source == CharacterCustomization.Source.HostNewFarm)
      {
        IClickableMenu.drawTextureBox(b, this.xPositionOnScreen - 256 /*0x0100*/ + 4 - (LocalizedContentManager.CurrentLanguageCode == LocalizedContentManager.LanguageCode.ko ? 25 : 0), this.yPositionOnScreen + IClickableMenu.borderWidth * 2 + 68, LocalizedContentManager.CurrentLanguageCode == LocalizedContentManager.LanguageCode.ko ? 320 : 256 /*0x0100*/, 512 /*0x0200*/, Color.White);
        foreach (ClickableTextureComponent cabinLayoutButton in this.cabinLayoutButtons)
        {
          cabinLayoutButton.draw(b, Color.White * (Game1.startingCabins > 0 ? 1f : 0.5f), 0.9f);
          if (Game1.startingCabins > 0 && (cabinLayoutButton.name.Equals("Close") && !Game1.cabinsSeparate || cabinLayoutButton.name.Equals("Separate") && Game1.cabinsSeparate))
            b.Draw(Game1.mouseCursors, cabinLayoutButton.bounds, new Rectangle?(Game1.getSourceRectForStandardTileSheet(Game1.mouseCursors, 34)), Color.White);
        }
      }
      b.Draw(Game1.daybg, new Vector2((float) this.portraitBox.X, (float) this.portraitBox.Y), Color.White);
      foreach (ClickableTextureComponent genderButton in this.genderButtons)
      {
        if (genderButton.visible)
        {
          genderButton.draw(b);
          if (genderButton.name.Equals("Male") && Game1.player.IsMale || genderButton.name.Equals("Female") && !Game1.player.IsMale)
            b.Draw(Game1.mouseCursors, genderButton.bounds, new Rectangle?(Game1.getSourceRectForStandardTileSheet(Game1.mouseCursors, 34)), Color.White);
        }
      }
      if (this.nameBoxCC.visible)
        Game1.player.Name = this.nameBox.Text;
      if (this.favThingBoxCC.visible)
        Game1.player.favoriteThing.Value = this.favThingBox.Text;
      if (this.farmnameBoxCC.visible)
        Game1.player.farmName.Value = this.farmnameBox.Text;
      if (this.source == CharacterCustomization.Source.NewFarmhand)
        Game1.player.farmName.Value = Game1.MasterPlayer.farmName.Value;
      foreach (ClickableTextureComponent leftSelectionButton in this.leftSelectionButtons)
        leftSelectionButton.draw(b);
      foreach (ClickableComponent label in this.labels)
      {
        if (label.visible)
        {
          string str = "";
          float num1 = 0.0f;
          float num2 = 0.0f;
          Color color = Game1.textColor;
          int num3;
          if (label == this.nameLabel)
          {
            string name = Game1.player.Name;
            color = (name != null ? (name.Length < 1 ? 1 : 0) : 0) != 0 ? Color.Red : Game1.textColor;
          }
          else if (label == this.farmLabel)
            color = Game1.player.farmName.Value == null || Game1.player.farmName.Length >= 1 ? Game1.textColor : Color.Red;
          else if (label == this.favoriteLabel)
            color = Game1.player.favoriteThing.Value == null || Game1.player.favoriteThing.Length >= 1 ? Game1.textColor : Color.Red;
          else if (label == this.shirtLabel)
          {
            num1 = (float) (21.0 - (double) Game1.smallFont.MeasureString(label.name).X / 2.0);
            num3 = Game1.player.GetShirtIndex();
            str = num3.ToString();
            int result;
            if (int.TryParse(str, out result))
            {
              num3 = result + 1;
              str = num3.ToString();
            }
          }
          else if (label == this.skinLabel)
          {
            num1 = (float) (21.0 - (double) Game1.smallFont.MeasureString(label.name).X / 2.0);
            num3 = Game1.player.skin.Value + 1;
            str = num3.ToString() ?? "";
          }
          else if (label == this.hairLabel)
          {
            num1 = (float) (21.0 - (double) Game1.smallFont.MeasureString(label.name).X / 2.0);
            if (!label.name.Contains("Color"))
            {
              num3 = Farmer.GetAllHairstyleIndices().IndexOf(Game1.player.hair.Value) + 1;
              str = num3.ToString() ?? "";
            }
          }
          else if (label == this.accLabel)
          {
            num1 = (float) (21.0 - (double) Game1.smallFont.MeasureString(label.name).X / 2.0);
            num3 = Game1.player.accessory.Value + 2;
            str = num3.ToString() ?? "";
          }
          else if (label == this.pantsStyleLabel)
          {
            num1 = (float) (21.0 - (double) Game1.smallFont.MeasureString(label.name).X / 2.0);
            num3 = Game1.player.GetPantsIndex();
            str = num3.ToString();
            int result;
            if (int.TryParse(str, out result))
            {
              num3 = result + 1;
              str = num3.ToString();
            }
          }
          else if (label == this.startingCabinsLabel)
          {
            num1 = (float) (21.0 - (double) Game1.smallFont.MeasureString(label.name).X / 2.0);
            str = Game1.startingCabins != 0 || this.noneString == null ? Game1.startingCabins.ToString() ?? "" : this.noneString;
            num2 = 4f;
          }
          else if (label == this.difficultyModifierLabel)
          {
            num1 = (float) (21.0 - (double) Game1.smallFont.MeasureString(label.name).X / 2.0);
            num2 = 4f;
            str = this.getNameOfDifficulty();
          }
          else if (label == this.separateWalletLabel)
          {
            num1 = (float) (21.0 - (double) Game1.smallFont.MeasureString(label.name).X / 2.0);
            num2 = 4f;
            str = Game1.player.team.useSeparateWallets.Value ? this.separateWalletString : this.sharedWalletString;
          }
          else
            color = Game1.textColor;
          Utility.drawTextWithShadow(b, label.name, Game1.smallFont, new Vector2((float) label.bounds.X + num1, (float) label.bounds.Y), color);
          if (str.Length > 0)
            Utility.drawTextWithShadow(b, str, Game1.smallFont, new Vector2((float) (label.bounds.X + 21) - Game1.smallFont.MeasureString(str).X / 2f, (float) (label.bounds.Y + 32 /*0x20*/) + num2), color);
        }
      }
      foreach (ClickableTextureComponent rightSelectionButton in this.rightSelectionButtons)
        rightSelectionButton.draw(b);
      if (this.farmTypeButtons.Count > 0)
      {
        IClickableMenu.drawTextureBox(b, this.farmTypeButtons[0].bounds.X - 16 /*0x10*/, this.farmTypeButtons[0].bounds.Y - 20, 220, 564, Color.White);
        for (int index1 = 0; index1 < this.farmTypeButtons.Count; ++index1)
        {
          this.farmTypeButtons[index1].draw(b, this.farmTypeButtons[index1].name.Contains("Gray") ? Color.Black * 0.5f : Color.White, 0.88f);
          if (this.farmTypeButtons[index1].name.Contains("Gray"))
            b.Draw(Game1.mouseCursors, new Vector2((float) (this.farmTypeButtons[index1].bounds.Center.X - 12), (float) (this.farmTypeButtons[index1].bounds.Center.Y - 8)), new Rectangle?(new Rectangle(107, 442, 7, 8)), Color.White, 0.0f, Vector2.Zero, 4f, SpriteEffects.None, 0.89f);
          bool flag = false;
          int index2 = index1 + this._currentFarmPage * 6;
          if (Game1.whichFarm == 7)
          {
            if ("ModFarm_" + Game1.whichModFarm.Id == this.farmTypeButtonNames[index2])
              flag = true;
          }
          else if (Game1.whichFarm == index2)
            flag = true;
          if (flag)
            IClickableMenu.drawTextureBox(b, Game1.mouseCursors, new Rectangle(375, 357, 3, 3), this.farmTypeButtons[index1].bounds.X, this.farmTypeButtons[index1].bounds.Y - 4, this.farmTypeButtons[index1].bounds.Width, this.farmTypeButtons[index1].bounds.Height + 8, Color.White, 4f, false);
        }
        this.farmTypeNextPageButton?.draw(b);
        this.farmTypePreviousPageButton?.draw(b);
      }
      PetData data;
      if (this.petPortraitBox.HasValue && Pet.TryGetData(Game1.MasterPlayer.whichPetType, out data))
      {
        Texture2D texture = (Texture2D) null;
        Rectangle rectangle = Rectangle.Empty;
        foreach (PetBreed breed in data.Breeds)
        {
          if (breed.Id == Game1.MasterPlayer.whichPetBreed)
          {
            texture = Game1.content.Load<Texture2D>(breed.IconTexture);
            rectangle = breed.IconSourceRect;
            break;
          }
        }
        if (texture != null)
          b.Draw(texture, this.petPortraitBox.Value, new Rectangle?(rectangle), Color.White, 0.0f, Vector2.Zero, SpriteEffects.None, 0.89f);
      }
      this.advancedOptionsButton?.draw(b);
      if (this.canLeaveMenu())
      {
        this.okButton.draw(b, Color.White, 0.75f);
      }
      else
      {
        this.okButton.draw(b, Color.White, 0.75f);
        this.okButton.draw(b, Color.Black * 0.5f, 0.751f);
      }
      this.coopHelpButton?.draw(b, Color.White, 0.75f);
      this.hairColorPicker?.draw(b);
      this.pantsColorPicker?.draw(b);
      this.eyeColorPicker?.draw(b);
      if (this.source != CharacterCustomization.Source.Dresser && this.source != CharacterCustomization.Source.DyePots && this.source != CharacterCustomization.Source.ClothesDye)
      {
        this.nameBox.Draw(b);
        this.favThingBox.Draw(b);
      }
      if (this.farmnameBoxCC.visible)
      {
        this.farmnameBox.Draw(b);
        Utility.drawTextWithShadow(b, Game1.content.LoadString("Strings\\UI:Character_FarmNameSuffix"), Game1.smallFont, new Vector2((float) (this.farmnameBox.X + this.farmnameBox.Width + 8), (float) (this.farmnameBox.Y + 12)), Game1.textColor);
      }
      if (this.skipIntroButton != null && this.skipIntroButton.visible)
      {
        this.skipIntroButton.draw(b);
        Utility.drawTextWithShadow(b, Game1.content.LoadString("Strings\\UI:Character_SkipIntro"), Game1.smallFont, new Vector2((float) (this.skipIntroButton.bounds.X + this.skipIntroButton.bounds.Width + 8), (float) (this.skipIntroButton.bounds.Y + 8)), Game1.textColor);
      }
      if ((double) this.advancedCCHighlightTimer > 0.0)
        b.Draw(Game1.mouseCursors, this.advancedOptionsButton.getVector2() + new Vector2(4f, 84f), new Rectangle?(new Rectangle(128 /*0x80*/ + ((double) this.advancedCCHighlightTimer % 500.0 < 250.0 ? 16 /*0x10*/ : 0), 208 /*0xD0*/, 16 /*0x10*/, 16 /*0x10*/)), Color.White * Math.Min(1f, this.advancedCCHighlightTimer / 500f), 0.0f, Vector2.Zero, 4f, SpriteEffects.None, 0.5f);
      this.randomButton.draw(b);
      b.End();
      b.Begin(SpriteSortMode.FrontToBack, BlendState.AlphaBlend, SamplerState.PointClamp);
      this._displayFarmer.FarmerRenderer.draw(b, this._displayFarmer.FarmerSprite.CurrentAnimationFrame, this._displayFarmer.FarmerSprite.CurrentFrame, this._displayFarmer.FarmerSprite.SourceRect, new Vector2((float) (this.portraitBox.Center.X - 32 /*0x20*/), (float) (this.portraitBox.Bottom - 160 /*0xA0*/)), Vector2.Zero, 0.8f, Color.White, 0.0f, 1f, this._displayFarmer);
      b.End();
      b.Begin(blendState: BlendState.AlphaBlend, samplerState: SamplerState.PointClamp);
      string hoverTitle = this.hoverTitle;
      if ((hoverTitle != null ? (hoverTitle.Length > 0 ? 1 : 0) : 0) != 0)
      {
        int width = Math.Max((int) Game1.dialogueFont.MeasureString(this.hoverTitle).X, 256 /*0x0100*/);
        IClickableMenu.drawHoverText(b, Game1.parseText(this.hoverText, Game1.smallFont, width), Game1.smallFont, boldTitleText: this.hoverTitle);
      }
      this.drawMouse(b);
    }
  }

  public override void emergencyShutDown()
  {
    if (this._itemToDye != null)
    {
      if (!Game1.player.IsEquippedItem((Item) this._itemToDye))
        Utility.CollectOrDrop((Item) this._itemToDye);
      this._itemToDye = (Clothing) null;
    }
    base.emergencyShutDown();
  }

  public override bool IsAutomaticSnapValid(
    int direction,
    ClickableComponent a,
    ClickableComponent b)
  {
    if (a.region != b.region || this.advancedOptionsButton != null && this.backButton != null && a == this.advancedOptionsButton && b == this.backButton || this.source == CharacterCustomization.Source.Wizard && (a == this.favThingBoxCC && b.myID >= 522 && b.myID <= 530 || b == this.favThingBoxCC && a.myID >= 522 && a.myID <= 530) || this.source == CharacterCustomization.Source.Wizard && (a.name == "Direction" && b.name == "Pet" || b.name == "Direction" && a.name == "Pet"))
      return false;
    if (this.randomButton != null)
    {
      switch (direction)
      {
        case 0:
          if (a.myID == 622 && direction == 1 && (b == this.nameBoxCC || b == this.favThingBoxCC || b == this.farmnameBoxCC))
            return false;
          break;
        case 3:
          if (b == this.randomButton && a.name == "Direction")
            return false;
          goto case 0;
        default:
          if (a == this.randomButton && b.name != "Direction" || b == this.randomButton && a.name != "Direction")
            return false;
          goto case 0;
      }
    }
    return base.IsAutomaticSnapValid(direction, a, b);
  }

  /// <inheritdoc />
  public override void update(GameTime time)
  {
    base.update(time);
    if (this.showingCoopHelp)
    {
      this.backButton.visible = false;
      switch (this.coopHelpScreen)
      {
        case 0:
          this.coopHelpRightButton.visible = true;
          this.coopHelpLeftButton.visible = false;
          break;
        case 1:
          this.coopHelpRightButton.visible = false;
          this.coopHelpLeftButton.visible = true;
          break;
      }
    }
    else
      this.backButton.visible = this._shouldShowBackButton;
    if (this._sliderOpTarget != null)
    {
      Color selectedColor = this._sliderOpTarget.getSelectedColor();
      if (this._sliderOpTarget.Dirty && this._sliderOpTarget.LastColor == selectedColor)
      {
        this._sliderAction();
        this._sliderOpTarget.LastColor = this._sliderOpTarget.getSelectedColor();
        this._sliderOpTarget.Dirty = false;
        this._sliderOpTarget = (ColorPicker) null;
      }
      else
        this._sliderOpTarget.LastColor = selectedColor;
    }
    if ((double) this.advancedCCHighlightTimer <= 0.0)
      return;
    this.advancedCCHighlightTimer -= (float) time.ElapsedGameTime.TotalMilliseconds;
  }

  public enum Source
  {
    NewGame,
    NewFarmhand,
    Wizard,
    HostNewFarm,
    Dresser,
    ClothesDye,
    DyePots,
  }
}
