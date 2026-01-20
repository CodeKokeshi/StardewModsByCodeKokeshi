// Decompiled with JetBrains decompiler
// Type: StardewValley.Menus.TitleMenu
// Assembly: Stardew Valley, Version=1.6.15.24356, Culture=neutral, PublicKeyToken=null
// MVID: AA1F513A-94F0-4EF6-A35F-2D5B4A3721BF
// Assembly location: D:\SteamLibrary\steamapps\common\Stardew Valley\Stardew Valley.dll
// XML documentation location: D:\SteamLibrary\steamapps\common\Stardew Valley\Stardew Valley.xml

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using StardewValley.BellsAndWhistles;
using StardewValley.Extensions;
using StardewValley.Minigames;
using System;
using System.Collections.Generic;

#nullable disable
namespace StardewValley.Menus;

public class TitleMenu : IClickableMenu, IDisposable
{
  public static bool SkipSplashScreens = false;
  public const int region_muteMusic = 81111;
  public const int region_windowedButton = 81112;
  public const int region_aboutButton = 81113;
  public const int region_backButton = 81114;
  public const int region_newButton = 81115;
  public const int region_loadButton = 81116;
  public const int region_coopButton = 81119;
  public const int region_exitButton = 81117;
  public const int region_languagesButton = 81118;
  public const int fadeFromWhiteDuration = 2000;
  public const int viewportFinalPosition = -1000;
  public const int logoSwipeDuration = 1000;
  public const int numberOfButtons = 4;
  public const int spaceBetweenButtons = 8;
  public const float bigCloudDX = 0.1f;
  public const float mediumCloudDX = 0.2f;
  public const float smallCloudDX = 0.3f;
  public const float bgmountainsParallaxSpeed = 0.66f;
  public const float mountainsParallaxSpeed = 1f;
  public const float foregroundJungleParallaxSpeed = 2f;
  public const float cloudsParallaxSpeed = 0.5f;
  public static int pixelZoom = 3;
  public const string titleButtonsTextureName = "Minigames\\TitleButtons";
  public LocalizedContentManager menuContent = Game1.content.CreateTemporary();
  public Texture2D cloudsTexture;
  public Texture2D titleButtonsTexture;
  public bool specialSurprised;
  public float specialSurprisedTimeStamp;
  private Texture2D amuzioTexture;
  private List<float> bigClouds = new List<float>();
  private List<float> smallClouds = new List<float>();
  private TemporaryAnimatedSpriteList tempSprites = new TemporaryAnimatedSpriteList();
  private TemporaryAnimatedSpriteList behindSignTempSprites = new TemporaryAnimatedSpriteList();
  public List<ClickableTextureComponent> buttons = new List<ClickableTextureComponent>();
  public ClickableTextureComponent backButton;
  public ClickableTextureComponent muteMusicButton;
  public ClickableTextureComponent aboutButton;
  public ClickableTextureComponent languageButton;
  public ClickableTextureComponent windowedButton;
  public ClickableComponent skipButton;
  protected bool _movedCursor;
  public TemporaryAnimatedSpriteList birds = new TemporaryAnimatedSpriteList();
  private Rectangle eRect;
  private Rectangle screwRect;
  private Rectangle cornerRect;
  private Rectangle r_hole_rect;
  private Rectangle r_hole_rect2;
  private List<Rectangle> leafRects;
  [InstancedStatic]
  internal static IClickableMenu _subMenu;
  public readonly StartupPreferences startupPreferences;
  public int globalXOffset;
  public float viewportY;
  public float viewportDY;
  public float logoSwipeTimer;
  public float globalCloudAlpha = 1f;
  public float cornerClickEndTimer;
  public float cornerClickParrotTimer;
  public float cornerClickSoundEffectTimer;
  private bool? hasRoomAnotherFarm = new bool?(false);
  public int fadeFromWhiteTimer;
  public int pauseBeforeViewportRiseTimer;
  public int buttonsToShow;
  public int showButtonsTimer;
  public int logoFadeTimer;
  public int logoSurprisedTimer;
  public int clicksOnE;
  public int clicksOnLeaf;
  public int clicksOnScrew;
  public int cornerClicks;
  public int buttonsDX;
  public bool titleInPosition;
  public bool isTransitioningButtons;
  public bool shades;
  public bool cornerPhaseHolding;
  public bool showCornerClickEasterEgg;
  public bool transitioningCharacterCreationMenu;
  private int amuzioTimer;
  internal static int windowNumber = 3;
  public string startupMessage = "";
  public Color startupMessageColor = Color.DeepSkyBlue;
  public string debugSaveFileToTry;
  private int bCount;
  private string whichSubMenu = "";
  private int quitTimer;
  private bool transitioningFromLoadScreen;
  [NonInstancedStatic]
  public static int ticksUntilLanguageLoad = 1;
  private bool disposedValue;

  /// <summary>An event raised when the player clicks the button to start after creating their new main character.</summary>
  public static event Action OnCreatedNewCharacter;

  /// <summary>The sub-menu to show instead of the main title screen.</summary>
  /// <remarks>When returning to the main title screen from a submenu, call <see cref="M:StardewValley.Menus.TitleMenu.ReturnToMainTitleScreen" /> instead of setting it to null to allow cleanup.</remarks>
  public static IClickableMenu subMenu
  {
    get => TitleMenu._subMenu;
    set
    {
      if (TitleMenu._subMenu != null)
      {
        TitleMenu._subMenu.exitFunction = (IClickableMenu.onExit) null;
        if (TitleMenu._subMenu is IDisposable subMenu && !TitleMenu.subMenu.HasDependencies())
          subMenu.Dispose();
      }
      TitleMenu._subMenu = value;
      if (TitleMenu._subMenu == null)
        return;
      if (Game1.activeClickableMenu is TitleMenu activeClickableMenu)
        TitleMenu._subMenu.exitFunction += new IClickableMenu.onExit(activeClickableMenu.CloseSubMenu);
      if (!Game1.options.snappyMenus || !Game1.options.gamepadControls)
        return;
      TitleMenu._subMenu.snapToDefaultClickableComponent();
    }
  }

  /// <summary>Exit the current sub-menu and return to the main title screen.</summary>
  public static void ReturnToMainTitleScreen()
  {
    TitleMenu.subMenu = (IClickableMenu) null;
    Game1.game1.ResetGameStateOnTitleScreen();
  }

  public void ForceSubmenu(IClickableMenu menu)
  {
    this.skipToTitleButtons();
    TitleMenu.subMenu = menu;
    this.moveFeatures(1920, 0);
    this.globalXOffset = 1920;
    this.buttonsToShow = 4;
    this.showButtonsTimer = 0;
    this.viewportDY = 0.0f;
    this.logoSwipeTimer = 0.0f;
    this.titleInPosition = true;
  }

  public bool HasActiveUser => true;

  public TitleMenu()
    : base(0, 0, Game1.uiViewport.Width, Game1.uiViewport.Height)
  {
    LocalizedContentManager.OnLanguageChange += new LocalizedContentManager.LanguageChangedHandler(this.OnLanguageChange);
    this.cloudsTexture = this.menuContent.Load<Texture2D>("Minigames\\Clouds");
    this.titleButtonsTexture = this.menuContent.Load<Texture2D>("Minigames\\TitleButtons");
    if (Program.sdk.IsJapaneseRegionRelease)
      this.amuzioTexture = this.menuContent.Load<Texture2D>("Minigames\\Amuzio");
    this.viewportY = 0.0f;
    this.fadeFromWhiteTimer = 4000;
    this.logoFadeTimer = 5000;
    if (Program.sdk.IsJapaneseRegionRelease)
      this.amuzioTimer = 4000;
    this.bigClouds.Add((float) (this.width * 3 / 4));
    this.shades = Game1.random.NextBool();
    this.smallClouds.Add((float) (this.width - 1));
    this.smallClouds.Add((float) (this.width - 1 + 230 * TitleMenu.pixelZoom));
    this.smallClouds.Add((float) (this.width * 2 / 3));
    this.smallClouds.Add((float) (this.width / 8));
    this.smallClouds.Add((float) (this.width - 1 + 430 * TitleMenu.pixelZoom));
    this.smallClouds.Add((float) (this.width * 3 / 4));
    this.smallClouds.Add(1f);
    this.smallClouds.Add((float) (this.width / 2 + 150 * TitleMenu.pixelZoom));
    this.smallClouds.Add((float) (this.width - 1 + 630 * TitleMenu.pixelZoom));
    this.smallClouds.Add((float) (this.width - 1 + 130 * TitleMenu.pixelZoom));
    this.smallClouds.Add((float) (this.width / 3 + 190 * TitleMenu.pixelZoom));
    this.smallClouds.Add((float) (1 + 100 * TitleMenu.pixelZoom));
    this.smallClouds.Add((float) (this.width / 2 + 830 * TitleMenu.pixelZoom));
    this.smallClouds.Add((float) (this.width * 2 / 3 + 120 * TitleMenu.pixelZoom));
    this.smallClouds.Add((float) (this.width * 3 / 4 + 170 * TitleMenu.pixelZoom));
    this.smallClouds.Add((float) (this.width / 4 + 220 * TitleMenu.pixelZoom));
    for (int index = 0; index < this.smallClouds.Count; ++index)
      this.smallClouds[index] += (float) Game1.random.Next(400);
    this.birds.Add(new TemporaryAnimatedSprite("Minigames\\TitleButtons", new Rectangle(296, 227, 26, 21), new Vector2((float) (this.width - 70 * TitleMenu.pixelZoom), (float) (this.height - 130 * TitleMenu.pixelZoom)), false, 0.0f, Color.White)
    {
      scale = (float) TitleMenu.pixelZoom,
      pingPong = true,
      animationLength = 4,
      interval = 100f,
      totalNumberOfLoops = 9999,
      local = true,
      motion = new Vector2(-1f, 0.0f),
      layerDepth = 0.25f
    });
    this.birds.Add(new TemporaryAnimatedSprite("Minigames\\TitleButtons", new Rectangle(296, 227, 26, 21), new Vector2((float) (this.width - 40 * TitleMenu.pixelZoom), (float) (this.height - 120 * TitleMenu.pixelZoom)), false, 0.0f, Color.White)
    {
      scale = (float) TitleMenu.pixelZoom,
      pingPong = true,
      animationLength = 4,
      interval = 100f,
      totalNumberOfLoops = 9999,
      local = true,
      delayBeforeAnimationStart = 100,
      motion = new Vector2(-1f, 0.0f),
      layerDepth = 0.25f
    });
    this.setUpIcons();
    ClickableTextureComponent textureComponent1 = new ClickableTextureComponent(new Rectangle(16 /*0x10*/, 16 /*0x10*/, 36, 36), Game1.mouseCursors, new Rectangle(128 /*0x80*/, 384, 9, 9), 4f);
    textureComponent1.myID = 81111;
    textureComponent1.downNeighborID = 81115;
    textureComponent1.rightNeighborID = 81112;
    this.muteMusicButton = textureComponent1;
    ClickableTextureComponent textureComponent2 = new ClickableTextureComponent(new Rectangle(Game1.uiViewport.Width - 36 - 16 /*0x10*/, 16 /*0x10*/, 36, 36), Game1.mouseCursors, new Rectangle(Game1.options == null || Game1.options.isCurrentlyWindowed() ? 146 : 155, 384, 9, 9), 4f);
    textureComponent2.myID = 81112;
    textureComponent2.leftNeighborID = 81111;
    textureComponent2.downNeighborID = 81113;
    this.windowedButton = textureComponent2;
    this.startupPreferences = new StartupPreferences();
    this.startupPreferences.loadPreferences(false, false);
    this.applyPreferences();
    switch (this.startupPreferences.timesPlayed)
    {
      case 2:
        this.startupMessage = this.menuContent.LoadString("Strings\\StringsFromCSFiles:TitleMenu.cs.11717");
        break;
      case 3:
        this.startupMessage = this.menuContent.LoadString("Strings\\StringsFromCSFiles:TitleMenu.cs.11718");
        break;
      case 4:
        this.startupMessage = this.menuContent.LoadString("Strings\\StringsFromCSFiles:TitleMenu.cs.11719");
        break;
      case 5:
        this.startupMessage = this.menuContent.LoadString("Strings\\StringsFromCSFiles:TitleMenu.cs.11720");
        break;
      case 6:
        this.startupMessage = this.menuContent.LoadString("Strings\\StringsFromCSFiles:TitleMenu.cs.11721");
        break;
      case 7:
        this.startupMessage = this.menuContent.LoadString("Strings\\StringsFromCSFiles:TitleMenu.cs.11722");
        break;
      case 8:
        this.startupMessage = this.menuContent.LoadString("Strings\\StringsFromCSFiles:TitleMenu.cs.11723");
        break;
      case 9:
        this.startupMessage = this.menuContent.LoadString("Strings\\StringsFromCSFiles:TitleMenu.cs.11724");
        break;
      case 10:
        this.startupMessage = this.menuContent.LoadString("Strings\\StringsFromCSFiles:TitleMenu.cs.11725");
        break;
      case 15:
        if (LocalizedContentManager.CurrentLanguageCode == LocalizedContentManager.LanguageCode.en)
        {
          string randomNoun1 = Dialogue.getRandomNoun();
          string randomNoun2 = Dialogue.getRandomNoun();
          this.startupMessage = $"{this.menuContent.LoadString("Strings\\StringsFromCSFiles:TitleMenu.cs.11726")}{Environment.NewLine}The {Dialogue.getRandomAdjective()} {randomNoun1} {Dialogue.getRandomVerb()} {Dialogue.getRandomPositional()} the {(randomNoun1.Equals(randomNoun2) ? "other " + randomNoun2 : randomNoun2)}";
          break;
        }
        int num = Game1.random.Next(1, 15);
        this.startupMessage = this.menuContent.LoadString("Strings\\StringsFromCSFiles:TitleMenu.cs.11726") + this.menuContent.LoadString("Strings\\StringsFromCSFiles:RandomSentence." + num.ToString());
        break;
      case 20:
        this.startupMessage = "<";
        break;
      case 30:
        this.startupMessage = this.menuContent.LoadString("Strings\\StringsFromCSFiles:TitleMenu.cs.11731");
        break;
      case 100:
        this.startupMessage = this.menuContent.LoadString("Strings\\StringsFromCSFiles:TitleMenu.cs.11732");
        break;
      case 1000:
        this.startupMessage = this.menuContent.LoadString("Strings\\StringsFromCSFiles:TitleMenu.cs.11733");
        break;
      case 10000:
        this.startupMessage = this.menuContent.LoadString("Strings\\StringsFromCSFiles:TitleMenu.cs.11734");
        break;
    }
    this.startupPreferences.savePreferences(false);
    Game1.setRichPresence("menus");
    if (Game1.options.snappyMenus && Game1.options.gamepadControls)
    {
      this.populateClickableComponentList();
      this.snapToDefaultClickableComponent();
    }
    if (TitleMenu.SkipSplashScreens)
      this.skipToTitleButtons();
    else
      TitleMenu.SkipSplashScreens = true;
  }

  private bool alternativeTitleGraphic()
  {
    return LocalizedContentManager.CurrentLanguageCode == LocalizedContentManager.LanguageCode.zh;
  }

  public void applyPreferences()
  {
    if (this.startupPreferences.playerLimit > 0)
      Game1.multiplayer.playerLimit = this.startupPreferences.playerLimit;
    if (this.startupPreferences.startMuted)
      this.muteMusicButton.sourceRect.X = !Utility.toggleMuteMusic() ? 128 /*0x80*/ : 137;
    if (this.startupPreferences.skipWindowPreparation && TitleMenu.windowNumber == 3)
      TitleMenu.windowNumber = -1;
    if (this.startupPreferences.windowMode == 2 && this.startupPreferences.fullscreenResolutionX != 0 && this.startupPreferences.fullscreenResolutionY != 0)
    {
      Game1.options.preferredResolutionX = this.startupPreferences.fullscreenResolutionX;
      Game1.options.preferredResolutionY = this.startupPreferences.fullscreenResolutionY;
    }
    Game1.options.gamepadMode = this.startupPreferences.gamepadMode;
    Game1.game1.CheckGamepadMode();
    if (!Game1.options.gamepadControls || !Game1.options.snappyMenus)
      return;
    this.populateClickableComponentList();
    this.snapToDefaultClickableComponent();
  }

  private void OnLanguageChange(LocalizedContentManager.LanguageCode code)
  {
    this.titleButtonsTexture = this.menuContent.Load<Texture2D>("Minigames\\TitleButtons");
    this.setUpIcons();
    this.tempSprites.Clear();
    this.startupPreferences.OnLanguageChange(code);
  }

  public void skipToTitleButtons()
  {
    this.logoFadeTimer = 0;
    this.logoSwipeTimer = 0.0f;
    this.titleInPosition = false;
    this.pauseBeforeViewportRiseTimer = 0;
    this.fadeFromWhiteTimer = 0;
    this.viewportY = -999f;
    this.viewportDY = -0.01f;
    this.birds.Clear();
    this.logoSwipeTimer = 1f;
    this.amuzioTimer = 0;
    Game1.changeMusicTrack("MainTheme");
    if (!Game1.options.SnappyMenus || !Game1.options.gamepadControls)
      return;
    this.snapToDefaultClickableComponent();
  }

  public void setUpIcons()
  {
    this.buttons.Clear();
    int num1 = 74;
    int x1 = this.width / 2 - (num1 * 4 * TitleMenu.pixelZoom + 24 * TitleMenu.pixelZoom) / 2;
    List<ClickableTextureComponent> buttons1 = this.buttons;
    ClickableTextureComponent textureComponent1 = new ClickableTextureComponent("New", new Rectangle(x1, this.height - 58 * TitleMenu.pixelZoom - 8 * TitleMenu.pixelZoom, num1 * TitleMenu.pixelZoom, 58 * TitleMenu.pixelZoom), (string) null, "", this.titleButtonsTexture, new Rectangle(0, 187, 74, 58), (float) TitleMenu.pixelZoom);
    textureComponent1.myID = 81115;
    textureComponent1.rightNeighborID = 81116;
    textureComponent1.upNeighborID = 81111;
    buttons1.Add(textureComponent1);
    int x2 = x1 + (num1 + 8) * TitleMenu.pixelZoom;
    List<ClickableTextureComponent> buttons2 = this.buttons;
    ClickableTextureComponent textureComponent2 = new ClickableTextureComponent("Load", new Rectangle(x2, this.height - 58 * TitleMenu.pixelZoom - 8 * TitleMenu.pixelZoom, 74 * TitleMenu.pixelZoom, 58 * TitleMenu.pixelZoom), (string) null, "", this.titleButtonsTexture, new Rectangle(74, 187, 74, 58), (float) TitleMenu.pixelZoom);
    textureComponent2.myID = 81116;
    textureComponent2.leftNeighborID = 81115;
    textureComponent2.rightNeighborID = -7777;
    textureComponent2.upNeighborID = 81111;
    buttons2.Add(textureComponent2);
    int x3 = x2 + (num1 + 8) * TitleMenu.pixelZoom;
    List<ClickableTextureComponent> buttons3 = this.buttons;
    ClickableTextureComponent textureComponent3 = new ClickableTextureComponent("Co-op", new Rectangle(x3, this.height - 58 * TitleMenu.pixelZoom - 8 * TitleMenu.pixelZoom, 74 * TitleMenu.pixelZoom, 58 * TitleMenu.pixelZoom), (string) null, "", this.titleButtonsTexture, new Rectangle(148, 187, 74, 58), (float) TitleMenu.pixelZoom);
    textureComponent3.myID = 81119;
    textureComponent3.leftNeighborID = 81116;
    textureComponent3.rightNeighborID = 81117;
    buttons3.Add(textureComponent3);
    int x4 = x3 + (num1 + 8) * TitleMenu.pixelZoom;
    List<ClickableTextureComponent> buttons4 = this.buttons;
    ClickableTextureComponent textureComponent4 = new ClickableTextureComponent("Exit", new Rectangle(x4, this.height - 58 * TitleMenu.pixelZoom - 8 * TitleMenu.pixelZoom, 74 * TitleMenu.pixelZoom, 58 * TitleMenu.pixelZoom), (string) null, "", this.titleButtonsTexture, new Rectangle(222, 187, 74, 58), (float) TitleMenu.pixelZoom);
    textureComponent4.myID = 81117;
    textureComponent4.leftNeighborID = 81119;
    textureComponent4.rightNeighborID = 81118;
    textureComponent4.upNeighborID = 81111;
    buttons4.Add(textureComponent4);
    int num2 = this.ShouldShrinkLogo() ? 2 : TitleMenu.pixelZoom;
    this.eRect = new Rectangle(this.width / 2 - 200 * num2 + 251 * num2, -300 * num2 - (int) ((double) this.viewportY / 3.0) * num2 + 26 * num2, 42 * num2, 68 * num2);
    this.screwRect = new Rectangle(this.width / 2 + 150 * num2, -300 * num2 - (int) ((double) this.viewportY / 3.0) * num2 + 80 /*0x50*/ * num2, 5 * num2, 5 * num2);
    this.cornerRect = new Rectangle(this.width / 2 - 200 * num2, -300 * num2 - (int) ((double) this.viewportY / 3.0) * num2 + 165 * num2, 20 * num2, 20 * num2);
    this.r_hole_rect = new Rectangle(this.width / 2 - 21 * num2, -300 * num2 - (int) ((double) this.viewportY / 3.0) * num2 + 39 * num2, 10 * num2, 11 * num2);
    this.r_hole_rect2 = new Rectangle(this.width / 2 - 35 * num2, -300 * num2 - (int) ((double) this.viewportY / 3.0) * num2 + 24 * num2, 7 * num2, 7 * num2);
    this.populateLeafRects();
    ClickableTextureComponent textureComponent5 = new ClickableTextureComponent(this.menuContent.LoadString("Strings\\StringsFromCSFiles:TitleMenu.cs.11739"), new Rectangle(this.width + -66 * TitleMenu.pixelZoom - 8 * TitleMenu.pixelZoom * 2, this.height - 27 * TitleMenu.pixelZoom - 8 * TitleMenu.pixelZoom, 66 * TitleMenu.pixelZoom, 27 * TitleMenu.pixelZoom), (string) null, "", this.titleButtonsTexture, new Rectangle(296, 252, 66, 27), (float) TitleMenu.pixelZoom);
    textureComponent5.myID = 81114;
    this.backButton = textureComponent5;
    ClickableTextureComponent textureComponent6 = new ClickableTextureComponent(this.menuContent.LoadString("Strings\\StringsFromCSFiles:TitleMenu.cs.11740"), new Rectangle(this.width + -22 * TitleMenu.pixelZoom - 8 * TitleMenu.pixelZoom * 2, this.height - 25 * TitleMenu.pixelZoom - 8 * TitleMenu.pixelZoom, 22 * TitleMenu.pixelZoom, 25 * TitleMenu.pixelZoom), (string) null, "", this.titleButtonsTexture, new Rectangle(8, 458, 22, 25), (float) TitleMenu.pixelZoom);
    textureComponent6.myID = 81113;
    textureComponent6.upNeighborID = 81118;
    textureComponent6.leftNeighborID = -7777;
    this.aboutButton = textureComponent6;
    ClickableTextureComponent textureComponent7 = new ClickableTextureComponent(this.menuContent.LoadString("Strings\\StringsFromCSFiles:TitleMenu.cs.11740"), new Rectangle(this.width + -22 * TitleMenu.pixelZoom - 8 * TitleMenu.pixelZoom * 2, this.height - 25 * TitleMenu.pixelZoom * 2 - 16 /*0x10*/ * TitleMenu.pixelZoom, 27 * TitleMenu.pixelZoom, 25 * TitleMenu.pixelZoom), (string) null, "", this.titleButtonsTexture, new Rectangle(52, 458, 27, 25), (float) TitleMenu.pixelZoom);
    textureComponent7.myID = 81118;
    textureComponent7.downNeighborID = 81113;
    textureComponent7.leftNeighborID = -7777;
    textureComponent7.upNeighborID = 81112;
    this.languageButton = textureComponent7;
    this.skipButton = new ClickableComponent(new Rectangle(this.width / 2 - 87 * TitleMenu.pixelZoom, this.height / 2 - 34 * TitleMenu.pixelZoom, 83 * TitleMenu.pixelZoom, 67 * TitleMenu.pixelZoom), this.menuContent.LoadString("Strings\\StringsFromCSFiles:TitleMenu.cs.11741"));
    if (this.globalXOffset > this.width)
      this.globalXOffset = this.width;
    foreach (ClickableComponent button in this.buttons)
      button.bounds.X += this.globalXOffset;
    if (!Game1.options.gamepadControls || !Game1.options.snappyMenus)
      return;
    this.populateClickableComponentList();
    this.snapToDefaultClickableComponent();
  }

  public override void snapToDefaultClickableComponent()
  {
    if (TitleMenu.subMenu != null)
    {
      TitleMenu.subMenu.snapToDefaultClickableComponent();
    }
    else
    {
      StartupPreferences startupPreferences = this.startupPreferences;
      this.currentlySnappedComponent = this.getComponentWithID((startupPreferences != null ? (startupPreferences.timesPlayed > 0 ? 1 : 0) : 0) != 0 ? 81116 : 81115);
      this.snapCursorToCurrentSnappedComponent();
    }
  }

  protected override void customSnapBehavior(int direction, int oldRegion, int oldID)
  {
    if (oldID == 81116 && direction == 1)
    {
      if (this.getComponentWithID(81119) != null)
      {
        this.setCurrentlySnappedComponentTo(81119);
        this.snapCursorToCurrentSnappedComponent();
      }
      else if (this.getComponentWithID(81117) != null)
      {
        this.setCurrentlySnappedComponentTo(81117);
        this.snapCursorToCurrentSnappedComponent();
      }
      else
      {
        this.setCurrentlySnappedComponentTo(81118);
        this.snapCursorToCurrentSnappedComponent();
      }
    }
    else
    {
      if (oldID != 81118 && oldID != 81113 || direction != 3)
        return;
      if (this.getComponentWithID(81117) != null)
      {
        this.setCurrentlySnappedComponentTo(81117);
        this.snapCursorToCurrentSnappedComponent();
      }
      else
      {
        this.setCurrentlySnappedComponentTo(81116);
        this.snapCursorToCurrentSnappedComponent();
      }
    }
  }

  public void populateLeafRects()
  {
    int num = this.ShouldShrinkLogo() ? 2 : TitleMenu.pixelZoom;
    this.leafRects = new List<Rectangle>()
    {
      new Rectangle(this.width / 2 - 200 * num + 251 * num - 196 * num, -300 * num - (int) ((double) this.viewportY / 3.0) * num + 26 * num + 109 * num, 17 * num, 30 * num),
      new Rectangle(this.width / 2 - 200 * num + 251 * num + 91 * num, -300 * num - (int) ((double) this.viewportY / 3.0) * num + 26 * num - 26 * num, 17 * num, 31 /*0x1F*/ * num),
      new Rectangle(this.width / 2 - 200 * num + 251 * num + 79 * num, -300 * num - (int) ((double) this.viewportY / 3.0) * num + 26 * num + 83 * num, 25 * num, 17 * num),
      new Rectangle(this.width / 2 - 200 * num + 251 * num - 213 * num, -300 * num - (int) ((double) this.viewportY / 3.0) * num + 26 * num - 24 * num, 14 * num, 23 * num),
      new Rectangle(this.width / 2 - 200 * num + 251 * num - 234 * num, -300 * num - (int) ((double) this.viewportY / 3.0) * num + 26 * num - 11 * num, 18 * num, 12 * num)
    };
  }

  /// <inheritdoc />
  public override void receiveRightClick(int x, int y, bool playSound = true)
  {
    if (!this.ShouldAllowInteraction() || this.transitioningCharacterCreationMenu)
      return;
    TitleMenu.subMenu?.receiveRightClick(x, y);
  }

  public override bool readyToClose() => false;

  /// <inheritdoc />
  public override bool overrideSnappyMenuCursorMovementBan() => !this.titleInPosition;

  /// <inheritdoc />
  public override void leftClickHeld(int x, int y)
  {
    if (this.transitioningCharacterCreationMenu)
      return;
    base.leftClickHeld(x, y);
    if (TitleMenu.subMenu == null)
      return;
    TitleMenu.subMenu.leftClickHeld(x, y);
  }

  /// <inheritdoc />
  public override void releaseLeftClick(int x, int y)
  {
    if (this.transitioningCharacterCreationMenu || this.transitioningCharacterCreationMenu)
      return;
    base.releaseLeftClick(x, y);
    TitleMenu.subMenu?.releaseLeftClick(x, y);
  }

  /// <inheritdoc />
  public override void receiveKeyPress(Keys key)
  {
    if (this.transitioningCharacterCreationMenu)
      return;
    switch (key)
    {
      case Keys.Escape:
      case Keys.B:
        if (this.logoFadeTimer > 0)
        {
          ++this.bCount;
          if (key == Keys.Escape)
            this.bCount += 3;
          if (this.bCount >= 3)
          {
            Game1.playSound("bigDeSelect");
            this.logoFadeTimer = 0;
            this.fadeFromWhiteTimer = 0;
            Game1.delayedActions.Clear();
            Game1.morningSongPlayAction = (DelayedAction) null;
            this.pauseBeforeViewportRiseTimer = 0;
            this.fadeFromWhiteTimer = 0;
            this.viewportY = -999f;
            this.viewportDY = -0.01f;
            this.birds.Clear();
            this.logoSwipeTimer = 1f;
            this.amuzioTimer = 0;
            Game1.changeMusicTrack("MainTheme");
            break;
          }
          break;
        }
        break;
      case Keys.N:
        if (!Program.releaseBuild && Game1.oldKBState.IsKeyDown(Keys.RightShift) && Game1.oldKBState.IsKeyDown(Keys.LeftControl))
        {
          Season season = Season.Spring;
          if (Game1.oldKBState.IsKeyDown(Keys.D1))
            Game1.whichFarm = 1;
          else if (Game1.oldKBState.IsKeyDown(Keys.D2))
            Game1.whichFarm = 2;
          else if (Game1.oldKBState.IsKeyDown(Keys.D3))
            Game1.whichFarm = 3;
          else if (Game1.oldKBState.IsKeyDown(Keys.D4))
            Game1.whichFarm = 4;
          else if (Game1.oldKBState.IsKeyDown(Keys.D5))
            Game1.whichFarm = 5;
          else if (Game1.oldKBState.IsKeyDown(Keys.D6))
            Game1.whichFarm = 6;
          if (Game1.oldKBState.IsKeyDown(Keys.C))
          {
            Game1.whichFarm = Game1.random.Next(6);
            Game1.season = (Season) Game1.random.Next(4);
          }
          Game1.game1.loadForNewGame();
          Game1.saveOnNewDay = false;
          Game1.player.eventsSeen.Add("60367");
          Game1.player.currentLocation = (GameLocation) Utility.getHomeOfFarmer(Game1.player);
          Game1.player.Position = new Vector2(9f, 9f) * 64f;
          Game1.player.isInBed.Value = true;
          Game1.player.farmName.Value = "Test";
          if (Game1.oldKBState.IsKeyDown(Keys.C))
          {
            Game1.season = season;
            Game1.setGraphicsForSeason(true);
          }
          Game1.player.mailReceived.Add("button_tut_1");
          Game1.player.mailReceived.Add("button_tut_2");
          Game1.NewDay(0.0f);
          Game1.exitActiveMenu();
          Game1.setGameMode((byte) 3);
          return;
        }
        break;
    }
    if (Game1.options.doesInputListContain(Game1.options.menuButton, key) || !this.ShouldAllowInteraction())
      return;
    TitleMenu.subMenu?.receiveKeyPress(key);
    if (!Game1.options.snappyMenus || !Game1.options.gamepadControls || TitleMenu.subMenu != null)
      return;
    base.receiveKeyPress(key);
  }

  /// <inheritdoc />
  public override void receiveGamePadButton(Buttons button)
  {
    base.receiveGamePadButton(button);
    TitleMenu.subMenu?.receiveGamePadButton(button);
    if (button != Buttons.B || !this.titleInPosition || this.logoFadeTimer > 0 || this.fadeFromWhiteTimer > 0)
      return;
    switch (TitleMenu.subMenu)
    {
      case LoadGameMenu loadGameMenu:
        if (loadGameMenu.deleteConfirmationScreen)
          break;
        this.backButtonPressed();
        break;
      case CharacterCustomization characterCustomization:
        if (characterCustomization.showingCoopHelp)
          break;
        this.backButtonPressed();
        break;
      default:
        this.backButtonPressed();
        break;
    }
  }

  /// <inheritdoc />
  public override void gamePadButtonHeld(Buttons b)
  {
    if (!Game1.lastCursorMotionWasMouse)
      this._movedCursor = true;
    TitleMenu.subMenu?.gamePadButtonHeld(b);
  }

  public void backButtonPressed()
  {
    if (TitleMenu.subMenu == null || !TitleMenu.subMenu.readyToClose())
      return;
    Game1.playSound("bigDeSelect");
    this.buttonsDX = -1;
    switch (TitleMenu.subMenu)
    {
      case AboutMenu _:
        TitleMenu.ReturnToMainTitleScreen();
        this.buttonsDX = 0;
        if (!Game1.options.SnappyMenus)
          break;
        this.setCurrentlySnappedComponentTo(81113);
        this.snapCursorToCurrentSnappedComponent();
        break;
      case TitleTextInputMenu titleTextInputMenu when titleTextInputMenu.context == "join_menu":
      case FarmhandMenu _:
        this.buttonsDX = 0;
        ((CoopMenu) (TitleMenu.subMenu = (IClickableMenu) new CoopMenu(false))).SetTab(CoopMenu.Tab.JOIN_TAB, false);
        if (!Game1.options.SnappyMenus)
          break;
        TitleMenu.subMenu.snapToDefaultClickableComponent();
        break;
      case CharacterCustomization characterCustomization when characterCustomization.source == CharacterCustomization.Source.HostNewFarm:
        this.buttonsDX = 0;
        ((CoopMenu) (TitleMenu.subMenu = (IClickableMenu) new CoopMenu(false))).SetTab(CoopMenu.Tab.HOST_TAB, false);
        Game1.changeMusicTrack("title_night");
        if (!Game1.options.SnappyMenus)
          break;
        TitleMenu.subMenu.snapToDefaultClickableComponent();
        break;
      default:
        this.isTransitioningButtons = true;
        if (TitleMenu.subMenu is LoadGameMenu)
          this.transitioningFromLoadScreen = true;
        TitleMenu.ReturnToMainTitleScreen();
        Game1.changeMusicTrack("spring_day_ambient");
        break;
    }
  }

  private void UpdateHasRoomAnotherFarm()
  {
    lock (this)
      this.hasRoomAnotherFarm = new bool?();
    Game1.GetHasRoomAnotherFarmAsync((ReportHasRoomAnotherFarmDelegate) (yes =>
    {
      lock (this)
        this.hasRoomAnotherFarm = new bool?(yes);
    }));
  }

  protected void CloseSubMenu()
  {
    if (!TitleMenu.subMenu.readyToClose())
      return;
    this.buttonsDX = -1;
    switch (TitleMenu.subMenu)
    {
      case AboutMenu _:
      case LanguageSelectionMenu _:
        TitleMenu.subMenu = (IClickableMenu) null;
        this.buttonsDX = 0;
        break;
      default:
        this.isTransitioningButtons = true;
        if (TitleMenu.subMenu is LoadGameMenu)
          this.transitioningFromLoadScreen = true;
        TitleMenu.subMenu = (IClickableMenu) null;
        Game1.changeMusicTrack("spring_day_ambient");
        break;
    }
  }

  /// <inheritdoc />
  public override void receiveLeftClick(int x, int y, bool playSound = true)
  {
    if (this.HasActiveUser && this.muteMusicButton.containsPoint(x, y))
    {
      this.startupPreferences.startMuted = Utility.toggleMuteMusic();
      this.muteMusicButton.sourceRect.X = this.muteMusicButton.sourceRect.X != 128 /*0x80*/ ? 128 /*0x80*/ : 137;
      Game1.playSound("drumkit6");
      this.startupPreferences.savePreferences(false);
    }
    else if (this.HasActiveUser && this.windowedButton.containsPoint(x, y))
    {
      if (!Game1.options.isCurrentlyWindowed())
      {
        Game1.options.setWindowedOption("Windowed");
        this.windowedButton.sourceRect.X = 146;
        this.startupPreferences.windowMode = 1;
      }
      else
      {
        Game1.options.setWindowedOption("Windowed Borderless");
        this.windowedButton.sourceRect.X = 155;
        this.startupPreferences.windowMode = 0;
      }
      this.startupPreferences.savePreferences(false);
      Game1.playSound("drumkit6");
    }
    else
    {
      if (this.logoFadeTimer > 0 && this.skipButton != null && this.skipButton.containsPoint(x, y))
      {
        if (this.logoSurprisedTimer <= 0)
        {
          int num = 1200;
          this.logoSurprisedTimer = 1500;
          string cueName = "fishSlap";
          Game1.changeMusicTrack("none");
          switch (Game1.random.Next(2))
          {
            case 0:
              cueName = "Duck";
              num = 0;
              break;
            case 1:
              cueName = "fishSlap";
              break;
          }
          if (Game1.random.NextDouble() < 0.02)
          {
            this.specialSurprised = true;
            Game1.playSound("moss_cut");
            this.fadeFromWhiteTimer = 3000;
          }
          else
            Game1.playSound(cueName, new int?(num));
        }
        else if (this.logoSurprisedTimer > 1)
          this.logoSurprisedTimer = Math.Max(1, this.logoSurprisedTimer - 500);
      }
      if (this.amuzioTimer > 500)
        this.amuzioTimer = 500;
      if (this.logoFadeTimer > 0 || this.fadeFromWhiteTimer > 0 || this.transitioningCharacterCreationMenu)
        return;
      if (TitleMenu.subMenu != null)
      {
        bool flag1 = false;
        if (Game1.options.SnappyMenus && TitleMenu.subMenu.currentlySnappedComponent != null && TitleMenu.subMenu.currentlySnappedComponent.myID != 81114)
          flag1 = true;
        bool flag2 = false;
        if (TitleMenu.subMenu.readyToClose() && this.backButton != null && this.backButton.containsPoint(x, y) && !flag1)
        {
          this.backButtonPressed();
          flag2 = true;
        }
        else if (!this.isTransitioningButtons)
          TitleMenu.subMenu.receiveLeftClick(x, y);
        if (flag2 || TitleMenu.subMenu == null || !TitleMenu.subMenu.readyToClose() || !(TitleMenu.subMenu is TooManyFarmsMenu) && (this.backButton == null || !this.backButton.containsPoint(x, y)) || flag1)
          return;
        Game1.playSound("bigDeSelect");
        this.buttonsDX = -1;
        switch (TitleMenu.subMenu)
        {
          case AboutMenu _:
          case LanguageSelectionMenu _:
            TitleMenu.ReturnToMainTitleScreen();
            this.buttonsDX = 0;
            break;
          default:
            this.isTransitioningButtons = true;
            if (TitleMenu.subMenu is LoadGameMenu)
              this.transitioningFromLoadScreen = true;
            TitleMenu.ReturnToMainTitleScreen();
            Game1.changeMusicTrack("spring_day_ambient");
            break;
        }
      }
      else if (this.logoFadeTimer <= 0 && !this.titleInPosition && (double) this.logoSwipeTimer == 0.0)
      {
        this.pauseBeforeViewportRiseTimer = 0;
        this.fadeFromWhiteTimer = 0;
        this.viewportY = -999f;
        this.viewportDY = -0.01f;
        this.birds.Clear();
        this.logoSwipeTimer = 1f;
      }
      else
      {
        if (!this.alternativeTitleGraphic())
        {
          if (this.clicksOnLeaf >= 10 && Game1.random.NextDouble() < 0.001)
            Game1.playSound("junimoMeep1");
          if (this.titleInPosition && this.eRect.Contains(x, y) && this.clicksOnE < 10)
          {
            ++this.clicksOnE;
            Game1.playSound("woodyStep");
            if (this.clicksOnE == 10)
            {
              int num = this.ShouldShrinkLogo() ? 2 : TitleMenu.pixelZoom;
              Game1.playSound("openChest");
              this.tempSprites.Add(new TemporaryAnimatedSprite("Minigames\\TitleButtons", new Rectangle(0, 491, 42, 68), new Vector2((float) (this.width / 2 - 200 * num + 251 * num), (float) (-300 * num - (int) ((double) this.viewportY / 3.0) * num + 26 * num)), false, 0.0f, Color.White)
              {
                scale = (float) num,
                animationLength = 9,
                interval = 200f,
                local = true,
                holdLastFrame = true
              });
            }
          }
          else if (this.titleInPosition)
          {
            bool flag = false;
            foreach (Rectangle leafRect in this.leafRects)
            {
              if (leafRect.Contains(x, y))
              {
                flag = true;
                break;
              }
            }
            if (this.screwRect.Contains(x, y) && this.clicksOnScrew < 10)
            {
              Game1.playSound("cowboy_monsterhit");
              ++this.clicksOnScrew;
              if (this.clicksOnScrew == 10)
                this.showButterflies();
            }
            if (Game1.content.GetCurrentLanguage() != LocalizedContentManager.LanguageCode.zh)
            {
              if (this.cornerPhaseHolding && (this.r_hole_rect.Contains(x, y) || this.r_hole_rect2.Contains(x, y)) && this.cornerClicks < 999)
              {
                Game1.playSound("coin");
                this.cornerClickEndTimer = 1000f;
                this.cornerClickSoundEffectTimer = 400f;
                this.cornerClicks = 9999;
                this.showCornerClickEasterEgg = true;
              }
              else if (this.cornerRect.Contains(x, y) && !this.cornerPhaseHolding)
              {
                int num = this.ShouldShrinkLogo() ? 2 : TitleMenu.pixelZoom;
                ++this.cornerClicks;
                if (this.cornerClicks > 5)
                {
                  if (!this.cornerPhaseHolding)
                  {
                    Game1.playSound("coin");
                    this.cornerClicks = 0;
                    this.cornerPhaseHolding = true;
                  }
                }
                else
                {
                  Game1.playSound("hammer");
                  for (int index = 0; index < 3; ++index)
                    this.tempSprites.Add(new TemporaryAnimatedSprite("LooseSprites\\Cursors", new Rectangle(280 + Game1.random.Choose<int>(8, 0), 1954, 8, 8), 1000f, 1, 99, new Vector2((float) (this.width / 2 - 190 * num), (float) (-300 * num - (int) ((double) this.viewportY / 3.0) * num + 175 * num)), false, false, 1f, 0.0f, Color.White, (float) TitleMenu.pixelZoom, 0.0f, 0.0f, (float) Game1.random.Next(-10, 11) / 100f)
                    {
                      motion = new Vector2((float) Game1.random.Next(-4, 5), (float) ((double) Game1.random.Next(-10, 1) / 100.0 - 8.0)),
                      acceleration = new Vector2(0.0f, 0.3f),
                      local = true,
                      delayBeforeAnimationStart = index * 15
                    });
                }
              }
            }
            if (flag)
            {
              ++this.clicksOnLeaf;
              if (this.clicksOnLeaf == 10)
              {
                int num = this.ShouldShrinkLogo() ? 2 : TitleMenu.pixelZoom;
                Game1.playSound("discoverMineral");
                this.tempSprites.Add(new TemporaryAnimatedSprite("Minigames\\TitleButtons", new Rectangle(264, 464, 16 /*0x10*/, 16 /*0x10*/), new Vector2((float) (this.width / 2 - 200 * num + 80 /*0x50*/ * num), (float) (-300 * num - (int) ((double) this.viewportY / 3.0) * num + 10 * num + 2)), false, 0.0f, Color.White)
                {
                  scale = (float) num,
                  animationLength = 8,
                  interval = 80f,
                  totalNumberOfLoops = 999999,
                  local = true,
                  holdLastFrame = false,
                  delayBeforeAnimationStart = 200
                });
                this.tempSprites.Add(new TemporaryAnimatedSprite("Minigames\\TitleButtons", new Rectangle(136, 448, 16 /*0x10*/, 16 /*0x10*/), new Vector2((float) (this.width / 2 - 200 * num + 80 /*0x50*/ * num), (float) (-300 * num - (int) ((double) this.viewportY / 3.0) * num + 10 * num)), false, 0.0f, Color.White)
                {
                  scale = (float) num,
                  animationLength = 8,
                  interval = 50f,
                  local = true,
                  holdLastFrame = false
                });
                this.tempSprites.Add(new TemporaryAnimatedSprite("Minigames\\TitleButtons", new Rectangle(200, 464, 16 /*0x10*/, 16 /*0x10*/), new Vector2((float) (this.width / 2 - 200 * num + 178 * num), (float) (-300 * num - (int) ((double) this.viewportY / 3.0) * num + 141 * num + 2)), false, 0.0f, Color.White)
                {
                  scale = (float) num,
                  animationLength = 4,
                  interval = 150f,
                  totalNumberOfLoops = 999999,
                  local = true,
                  holdLastFrame = false,
                  delayBeforeAnimationStart = 400
                });
                this.tempSprites.Add(new TemporaryAnimatedSprite("Minigames\\TitleButtons", new Rectangle(136, 448, 16 /*0x10*/, 16 /*0x10*/), new Vector2((float) (this.width / 2 - 200 * num + 178 * num), (float) (-300 * num - (int) ((double) this.viewportY / 3.0) * num + 141 * num)), false, 0.0f, Color.White)
                {
                  scale = (float) num,
                  animationLength = 8,
                  interval = 50f,
                  local = true,
                  holdLastFrame = false,
                  delayBeforeAnimationStart = 200
                });
                this.tempSprites.Add(new TemporaryAnimatedSprite("Minigames\\TitleButtons", new Rectangle(136, 464, 16 /*0x10*/, 16 /*0x10*/), new Vector2((float) (this.width / 2 - 200 * num + 294 * num), (float) (-300 * num - (int) ((double) this.viewportY / 3.0) * num + 89 * num + 2)), false, 0.0f, Color.White)
                {
                  scale = (float) num,
                  animationLength = 4,
                  interval = 150f,
                  totalNumberOfLoops = 999999,
                  local = true,
                  holdLastFrame = false,
                  delayBeforeAnimationStart = 600
                });
                this.tempSprites.Add(new TemporaryAnimatedSprite("Minigames\\TitleButtons", new Rectangle(136, 448, 16 /*0x10*/, 16 /*0x10*/), new Vector2((float) (this.width / 2 - 200 * num + 294 * num), (float) (-300 * num - (int) ((double) this.viewportY / 3.0) * num + 89 * num)), false, 0.0f, Color.White)
                {
                  scale = (float) num,
                  animationLength = 8,
                  interval = 50f,
                  local = true,
                  holdLastFrame = false,
                  delayBeforeAnimationStart = 400
                });
              }
              else
              {
                Game1.playSound("leafrustle");
                int num = this.ShouldShrinkLogo() ? 2 : TitleMenu.pixelZoom;
                for (int index = 0; index < 2; ++index)
                  this.tempSprites.Add(new TemporaryAnimatedSprite("LooseSprites\\Cursors", new Rectangle(355, 1199 + Game1.random.Next(-1, 2) * 16 /*0x10*/, 16 /*0x10*/, 16 /*0x10*/), new Vector2((float) (x + Game1.random.Next(-8, 9)), (float) (y + Game1.random.Next(-8, 9))), Game1.random.NextBool(), 0.0f, Color.White)
                  {
                    scale = (float) num,
                    animationLength = 11,
                    interval = (float) (50 + Game1.random.Next(50)),
                    totalNumberOfLoops = 999,
                    motion = new Vector2((float) Game1.random.Next(-100, 101) / 100f, (float) (1.0 + (double) Game1.random.Next(-100, 100) / 500.0)),
                    xPeriodic = Game1.random.NextBool(),
                    xPeriodicLoopTime = (float) Game1.random.Next(6000, 16000),
                    xPeriodicRange = (float) Game1.random.Next(64 /*0x40*/, 192 /*0xC0*/),
                    alphaFade = 1f / 1000f,
                    local = true,
                    holdLastFrame = false,
                    delayBeforeAnimationStart = index * 20
                  });
              }
            }
          }
        }
        if (!this.ShouldAllowInteraction() || !this.HasActiveUser || TitleMenu.subMenu != null && !TitleMenu.subMenu.readyToClose() || this.isTransitioningButtons)
          return;
        for (int index = 0; index < this.buttons.Count; ++index)
        {
          ClickableTextureComponent button = this.buttons[index];
          if (button.containsPoint(x, y))
            this.performButtonAction(button.name);
        }
        if (this.aboutButton.containsPoint(x, y))
        {
          TitleMenu.subMenu = (IClickableMenu) new AboutMenu();
          Game1.playSound("newArtifact");
        }
        if (!this.languageButton.visible || !this.languageButton.containsPoint(x, y))
          return;
        TitleMenu.subMenu = (IClickableMenu) new LanguageSelectionMenu();
        Game1.playSound("newArtifact");
      }
    }
  }

  public void performButtonAction(string which)
  {
    this.whichSubMenu = which;
    switch (which)
    {
      case "New":
        this.buttonsDX = 1;
        this.isTransitioningButtons = true;
        Game1.playSound("select");
        foreach (TemporaryAnimatedSprite tempSprite in this.tempSprites)
          tempSprite.pingPong = false;
        this.UpdateHasRoomAnotherFarm();
        break;
      case "Co-op":
        this.buttonsDX = 1;
        this.isTransitioningButtons = true;
        Game1.playSound("select");
        this.UpdateHasRoomAnotherFarm();
        break;
      case "Load":
      case "Invite":
        this.buttonsDX = 1;
        this.isTransitioningButtons = true;
        Game1.playSound("select");
        break;
      case "Exit":
        Game1.playSound("bigDeSelect");
        Game1.changeMusicTrack("none");
        this.quitTimer = 500;
        break;
    }
  }

  private void addRightLeafGust()
  {
    if (this.isTransitioningButtons || this.tempSprites.Count > 0 || this.alternativeTitleGraphic())
      return;
    int num = this.ShouldShrinkLogo() ? 2 : TitleMenu.pixelZoom;
    this.tempSprites.Add(new TemporaryAnimatedSprite("Minigames\\TitleButtons", new Rectangle(296, 187, 27, 21), new Vector2((float) (this.width / 2 - 200 * num + 327 * num), (float) (-300 * num) - this.viewportY / 3f * (float) num + (float) (107 * num)), false, 0.0f, Color.White)
    {
      scale = (float) num,
      pingPong = true,
      animationLength = 3,
      interval = 100f,
      totalNumberOfLoops = 3,
      local = true
    });
  }

  public bool ShouldShrinkLogo() => this.height <= 850;

  private void addLeftLeafGust()
  {
    if (this.isTransitioningButtons || this.tempSprites.Count > 0 || this.alternativeTitleGraphic())
      return;
    int num = this.ShouldShrinkLogo() ? 2 : TitleMenu.pixelZoom;
    this.tempSprites.Add(new TemporaryAnimatedSprite("Minigames\\TitleButtons", new Rectangle(296, 208 /*0xD0*/, 22, 18), new Vector2((float) (this.width / 2 - 200 * num + 16 /*0x10*/ * num), (float) (-300 * num) - this.viewportY / 3f * (float) num + (float) (16 /*0x10*/ * num)), false, 0.0f, Color.White)
    {
      scale = (float) num,
      pingPong = true,
      animationLength = 3,
      interval = 100f,
      totalNumberOfLoops = 3,
      local = true
    });
  }

  public void createdNewCharacter(bool skipIntro)
  {
    Action createdNewCharacter = TitleMenu.OnCreatedNewCharacter;
    if (createdNewCharacter != null)
      createdNewCharacter();
    Game1.playSound("smallSelect");
    TitleMenu.subMenu = (IClickableMenu) null;
    this.transitioningCharacterCreationMenu = true;
    if (!skipIntro)
      return;
    Game1.game1.loadForNewGame();
    Game1.saveOnNewDay = true;
    Game1.player.eventsSeen.Add("60367");
    Game1.player.currentLocation = (GameLocation) Utility.getHomeOfFarmer(Game1.player);
    Game1.player.Position = new Vector2(9f, 9f) * 64f;
    Game1.player.isInBed.Value = true;
    Game1.NewDay(0.0f);
    Game1.exitActiveMenu();
    Game1.setGameMode((byte) 3);
  }

  /// <inheritdoc />
  public override void update(GameTime time)
  {
    if (Game1.game1.IsMainInstance)
    {
      if (TitleMenu.ticksUntilLanguageLoad > 0)
        --TitleMenu.ticksUntilLanguageLoad;
      else if (TitleMenu.ticksUntilLanguageLoad == 0)
      {
        --TitleMenu.ticksUntilLanguageLoad;
        this.startupPreferences.loadPreferences(false, true);
      }
    }
    if (TitleMenu.windowNumber > 0)
    {
      if (this.startupPreferences.displayIndex >= 0 && !GameRunner.instance.Window.CenterOnDisplay(this.startupPreferences.displayIndex))
      {
        Game1.log.Error($"Error: Couldn't find display with index {this.startupPreferences.displayIndex.ToString()}. Reverting to windowed mode on display 0.");
        this.startupPreferences.windowMode = 1;
      }
      Game1.options.setWindowedOption(this.startupPreferences.windowMode);
      TitleMenu.windowNumber = 0;
    }
    if (!Game1.options.isCurrentlyWindowed())
    {
      Vector2 position = new Vector2((float) (Game1.viewport.Width - 36 - 16 /*0x10*/), 16f);
      Rectangle displayBounds = GameRunner.instance.Window.GetDisplayBounds(GameRunner.instance.Window.GetDisplayIndex());
      position.X = (float) (Math.Min(displayBounds.Right - GameRunner.instance.Window.ClientBounds.Left, Game1.viewport.Width) - 36 - 16 /*0x10*/);
      this.windowedButton.setPosition(position);
    }
    base.update(time);
    TitleMenu.subMenu?.update(time);
    TimeSpan elapsedGameTime;
    if (this.transitioningCharacterCreationMenu)
    {
      double globalCloudAlpha = (double) this.globalCloudAlpha;
      elapsedGameTime = time.ElapsedGameTime;
      double num = (double) elapsedGameTime.Milliseconds * (1.0 / 1000.0);
      this.globalCloudAlpha = (float) (globalCloudAlpha - num);
      if ((double) this.globalCloudAlpha <= 0.0)
      {
        this.transitioningCharacterCreationMenu = false;
        this.globalCloudAlpha = 0.0f;
        TitleMenu.subMenu = (IClickableMenu) null;
        Game1.currentMinigame = (IMinigame) new GrandpaStory();
        Game1.exitActiveMenu();
        Game1.setGameMode((byte) 3);
      }
    }
    if (this.quitTimer > 0)
    {
      int quitTimer = this.quitTimer;
      elapsedGameTime = time.ElapsedGameTime;
      int milliseconds = elapsedGameTime.Milliseconds;
      this.quitTimer = quitTimer - milliseconds;
      if (this.quitTimer <= 0)
      {
        Game1.quit = true;
        Game1.exitActiveMenu();
      }
    }
    if (this.amuzioTimer > 0)
    {
      int amuzioTimer = this.amuzioTimer;
      elapsedGameTime = time.ElapsedGameTime;
      int milliseconds = elapsedGameTime.Milliseconds;
      this.amuzioTimer = amuzioTimer - milliseconds;
    }
    else if (this.logoFadeTimer > 0)
    {
      if (this.logoSurprisedTimer > 0)
      {
        int logoSurprisedTimer = this.logoSurprisedTimer;
        elapsedGameTime = time.ElapsedGameTime;
        int milliseconds = elapsedGameTime.Milliseconds;
        this.logoSurprisedTimer = logoSurprisedTimer - milliseconds;
        if (this.logoSurprisedTimer <= 0)
          this.logoFadeTimer = 1;
      }
      else
      {
        int logoFadeTimer1 = this.logoFadeTimer;
        int logoFadeTimer2 = this.logoFadeTimer;
        elapsedGameTime = time.ElapsedGameTime;
        int milliseconds = elapsedGameTime.Milliseconds;
        this.logoFadeTimer = logoFadeTimer2 - milliseconds;
        if (this.logoFadeTimer < 4000 & logoFadeTimer1 >= 4000)
          Game1.playSound("mouseClick");
        if (this.logoFadeTimer < 2500 & logoFadeTimer1 >= 2500)
          Game1.playSound("mouseClick");
        if (this.logoFadeTimer < 2000 & logoFadeTimer1 >= 2000)
          Game1.playSound("mouseClick");
        if (this.logoFadeTimer <= 0)
          Game1.changeMusicTrack("MainTheme");
      }
    }
    else if (this.fadeFromWhiteTimer > 0)
    {
      int fadeFromWhiteTimer = this.fadeFromWhiteTimer;
      elapsedGameTime = time.ElapsedGameTime;
      int milliseconds = elapsedGameTime.Milliseconds;
      this.fadeFromWhiteTimer = fadeFromWhiteTimer - milliseconds;
      if (this.fadeFromWhiteTimer <= 0)
        this.pauseBeforeViewportRiseTimer = 3500;
    }
    else if (this.pauseBeforeViewportRiseTimer > 0)
    {
      int viewportRiseTimer = this.pauseBeforeViewportRiseTimer;
      elapsedGameTime = time.ElapsedGameTime;
      int milliseconds = elapsedGameTime.Milliseconds;
      this.pauseBeforeViewportRiseTimer = viewportRiseTimer - milliseconds;
      if (this.pauseBeforeViewportRiseTimer <= 0)
        this.viewportDY = -0.05f;
    }
    this.viewportY += this.viewportDY;
    if ((double) this.viewportDY < 0.0)
      this.viewportDY -= 3f / 500f;
    if ((double) this.viewportY <= -1000.0)
    {
      if ((double) this.viewportDY != 0.0)
      {
        this.logoSwipeTimer = 1000f;
        this.showButtonsTimer = 200;
      }
      this.viewportDY = 0.0f;
    }
    if ((double) this.logoSwipeTimer > 0.0)
    {
      double logoSwipeTimer = (double) this.logoSwipeTimer;
      elapsedGameTime = time.ElapsedGameTime;
      double milliseconds = (double) elapsedGameTime.Milliseconds;
      this.logoSwipeTimer = (float) (logoSwipeTimer - milliseconds);
      if ((double) this.logoSwipeTimer <= 0.0)
      {
        this.addLeftLeafGust();
        this.addRightLeafGust();
        this.titleInPosition = true;
        int num = this.ShouldShrinkLogo() ? 2 : TitleMenu.pixelZoom;
        this.eRect = new Rectangle(this.width / 2 - 200 * num + 251 * num, -300 * num - (int) ((double) this.viewportY / 3.0) * num + 26 * num, 42 * num, 68 * num);
        this.screwRect = new Rectangle(this.width / 2 + 150 * num, -300 * num - (int) ((double) this.viewportY / 3.0) * num + 80 /*0x50*/ * num, 5 * num, 5 * num);
        this.cornerRect = new Rectangle(this.width / 2 - 200 * num, -300 * num - (int) ((double) this.viewportY / 3.0) * num + 165 * num, 20 * num, 20 * num);
        this.r_hole_rect = new Rectangle(this.width / 2 - 21 * num, -300 * num - (int) ((double) this.viewportY / 3.0) * num + 39 * num, 10 * num, 11 * num);
        this.r_hole_rect2 = new Rectangle(this.width / 2 - 35 * num, -300 * num - (int) ((double) this.viewportY / 3.0) * num + 24 * num, 7 * num, 7 * num);
        this.populateLeafRects();
      }
    }
    if (this.showButtonsTimer > 0 && this.HasActiveUser && TitleMenu.subMenu == null)
    {
      int showButtonsTimer = this.showButtonsTimer;
      elapsedGameTime = time.ElapsedGameTime;
      int milliseconds = elapsedGameTime.Milliseconds;
      this.showButtonsTimer = showButtonsTimer - milliseconds;
      if (this.showButtonsTimer <= 0)
      {
        if (this.buttonsToShow < 4)
        {
          ++this.buttonsToShow;
          Game1.playSound("Cowboy_gunshot");
          this.showButtonsTimer = 200;
        }
        else if (Game1.options.gamepadControls && Game1.options.snappyMenus)
        {
          this.populateClickableComponentList();
          this.snapToDefaultClickableComponent();
        }
      }
    }
    if (this.titleInPosition && !this.isTransitioningButtons && this.globalXOffset == 0 && Game1.random.NextDouble() < 0.005)
    {
      if (Game1.random.NextBool())
        this.addLeftLeafGust();
      else
        this.addRightLeafGust();
    }
    if (this.titleInPosition)
    {
      if (this.isTransitioningButtons)
      {
        int buttonsDx = this.buttonsDX;
        elapsedGameTime = time.ElapsedGameTime;
        int totalMilliseconds = (int) elapsedGameTime.TotalMilliseconds;
        int dx = buttonsDx * totalMilliseconds;
        int num1 = this.globalXOffset + dx;
        int num2 = num1 - this.width;
        if (num2 > 0)
        {
          num1 -= num2;
          dx -= num2;
        }
        this.globalXOffset = num1;
        this.moveFeatures(dx, 0);
        if (this.buttonsDX > 0 && this.globalXOffset >= this.width)
        {
          if (TitleMenu.subMenu != null)
          {
            if (TitleMenu.subMenu.readyToClose())
            {
              this.isTransitioningButtons = false;
              this.buttonsDX = 0;
            }
          }
          else
          {
            switch (this.whichSubMenu)
            {
              case "Load":
                TitleMenu.subMenu = (IClickableMenu) new LoadGameMenu();
                Game1.changeMusicTrack("title_night");
                this.buttonsDX = 0;
                this.isTransitioningButtons = false;
                break;
              case "Co-op":
                if (this.hasRoomAnotherFarm.HasValue)
                {
                  this.buttonsDX = 0;
                  this.isTransitioningButtons = false;
                  if (true)
                  {
                    TitleMenu.subMenu = (IClickableMenu) new CoopMenu(!this.hasRoomAnotherFarm.Value);
                    Game1.changeMusicTrack("title_night");
                    break;
                  }
                  Game1.playSound("bigDeSelect");
                  if (Game1.options.SnappyMenus)
                  {
                    this.setCurrentlySnappedComponentTo(81119);
                    this.snapCursorToCurrentSnappedComponent();
                    break;
                  }
                  break;
                }
                break;
              case "Invite":
                TitleMenu.subMenu = (IClickableMenu) new FarmhandMenu();
                Game1.changeMusicTrack("title_night");
                this.buttonsDX = 0;
                this.isTransitioningButtons = false;
                break;
              case "New":
                if (this.hasRoomAnotherFarm.HasValue)
                {
                  if (!this.hasRoomAnotherFarm.Value)
                  {
                    TitleMenu.subMenu = (IClickableMenu) new TooManyFarmsMenu();
                    Game1.playSound("newArtifact");
                    this.buttonsDX = 0;
                    this.isTransitioningButtons = false;
                    break;
                  }
                  Game1.resetPlayer();
                  TitleMenu.subMenu = (IClickableMenu) new CharacterCustomization(CharacterCustomization.Source.NewGame);
                  if (this.startupPreferences.timesPlayed > 1 && !this.startupPreferences.sawAdvancedCharacterCreationIndicator)
                  {
                    if (TitleMenu.subMenu is CharacterCustomization subMenu)
                      subMenu.showAdvancedCharacterCreationHighlight();
                    this.startupPreferences.sawAdvancedCharacterCreationIndicator = true;
                    this.startupPreferences.savePreferences(false);
                  }
                  Game1.playSound("select");
                  Game1.changeMusicTrack("CloudCountry");
                  Game1.player.favoriteThing.Value = "";
                  this.buttonsDX = 0;
                  this.isTransitioningButtons = false;
                  break;
                }
                break;
            }
          }
          if (!this.isTransitioningButtons)
            this.whichSubMenu = "";
        }
        else if (this.buttonsDX < 0 && this.globalXOffset <= 0)
        {
          this.globalXOffset = 0;
          this.isTransitioningButtons = false;
          this.buttonsDX = 0;
          this.setUpIcons();
          this.whichSubMenu = "";
          this.transitioningFromLoadScreen = false;
        }
      }
      if ((double) this.cornerClickEndTimer > 0.0)
      {
        double cornerClickEndTimer = (double) this.cornerClickEndTimer;
        elapsedGameTime = Game1.currentGameTime.ElapsedGameTime;
        double totalMilliseconds = elapsedGameTime.TotalMilliseconds;
        this.cornerClickEndTimer = (float) (cornerClickEndTimer - totalMilliseconds);
        if ((double) this.cornerClickEndTimer <= 0.0)
          this.cornerClickParrotTimer = 400f;
      }
      if ((double) this.cornerClickSoundEffectTimer > 0.0)
      {
        double soundEffectTimer = (double) this.cornerClickSoundEffectTimer;
        elapsedGameTime = Game1.currentGameTime.ElapsedGameTime;
        double totalMilliseconds = elapsedGameTime.TotalMilliseconds;
        this.cornerClickSoundEffectTimer = (float) (soundEffectTimer - totalMilliseconds);
        if ((double) this.cornerClickSoundEffectTimer <= 0.0)
          Game1.playSound("goldenWalnut");
      }
      if ((double) this.cornerClickParrotTimer > 0.0)
      {
        double clickParrotTimer = (double) this.cornerClickParrotTimer;
        elapsedGameTime = Game1.currentGameTime.ElapsedGameTime;
        double totalMilliseconds = elapsedGameTime.TotalMilliseconds;
        this.cornerClickParrotTimer = (float) (clickParrotTimer - totalMilliseconds);
        if ((double) this.cornerClickParrotTimer <= 0.0)
        {
          int scale = this.ShouldShrinkLogo() ? 2 : TitleMenu.pixelZoom;
          this.behindSignTempSprites.Add(new TemporaryAnimatedSprite("LooseSprites\\parrots", new Rectangle(120, 0, 24, 24), 100f, 3, 999, new Vector2((float) (this.globalXOffset + this.width / 2 - 200 * scale), (float) (-300 * scale) - this.viewportY / 3f * (float) scale + (float) (100 * scale)), false, false, 0.2f, 0.0f, Color.White, (float) scale, 0.01f, 0.0f, 0.0f, true)
          {
            pingPong = true,
            motion = new Vector2(-6f, -1f),
            acceleration = new Vector2(0.02f, 0.02f)
          });
          this.behindSignTempSprites.Add(new TemporaryAnimatedSprite("LooseSprites\\parrots", new Rectangle(120, 48 /*0x30*/, 24, 24), 95f, 3, 999, new Vector2((float) (this.globalXOffset + this.width / 2 - 200 * scale), (float) (-300 * scale) - this.viewportY / 3f * (float) scale + (float) (120 * scale)), false, false, 0.2f, 0.0f, Color.White, (float) scale, 0.01f, 0.0f, 0.0f, true)
          {
            pingPong = true,
            motion = new Vector2(-6f, -1f),
            acceleration = new Vector2(0.02f, 0.02f),
            delayBeforeAnimationStart = 300,
            startSound = "leafrustle"
          });
          this.behindSignTempSprites.Add(new TemporaryAnimatedSprite("LooseSprites\\parrots", new Rectangle(120, 24, 24, 24), 100f, 3, 999, new Vector2((float) (this.globalXOffset + this.width / 2 - 200 * scale), (float) (-300 * scale) - this.viewportY / 3f * (float) scale + (float) (100 * scale)), false, false, 0.2f, 0.0f, Color.White, (float) scale, 0.01f, 0.0f, 0.0f, true)
          {
            pingPong = true,
            motion = new Vector2(-6f, -1f),
            acceleration = new Vector2(0.02f, 0.02f),
            delayBeforeAnimationStart = 600,
            startSound = "parrot_squawk"
          });
          this.behindSignTempSprites.Add(new TemporaryAnimatedSprite("LooseSprites\\parrots", new Rectangle(120, 72, 24, 24), 95f, 3, 999, new Vector2((float) (this.globalXOffset + this.width / 2 - 200 * scale), (float) (-300 * scale) - this.viewportY / 3f * (float) scale + (float) (120 * scale)), false, false, 0.2f, 0.0f, Color.White, (float) scale, 0.01f, 0.0f, 0.0f, true)
          {
            pingPong = true,
            motion = new Vector2(-6f, -1f),
            acceleration = new Vector2(0.02f, 0.02f),
            delayBeforeAnimationStart = 1300,
            startSound = "leafrustle"
          });
          this.behindSignTempSprites.Add(new TemporaryAnimatedSprite("LooseSprites\\parrots", new Rectangle(120, 0, 24, 24), 100f, 3, 999, new Vector2((float) (this.globalXOffset + this.width / 2 + 200 * scale - 24 * scale), (float) (-300 * scale) - this.viewportY / 3f * (float) scale + (float) (100 * scale)), false, true, 0.2f, 0.0f, Color.White, (float) scale, 0.01f, 0.0f, 0.0f, true)
          {
            pingPong = true,
            motion = new Vector2(6f, -1f),
            acceleration = new Vector2(-0.02f, -0.02f),
            delayBeforeAnimationStart = 600
          });
          this.behindSignTempSprites.Add(new TemporaryAnimatedSprite("LooseSprites\\parrots", new Rectangle(120, 48 /*0x30*/, 24, 24), 95f, 3, 999, new Vector2((float) (this.globalXOffset + this.width / 2 + 200 * scale - 24 * scale), (float) (-300 * scale) - this.viewportY / 3f * (float) scale + (float) (120 * scale)), false, true, 0.2f, 0.0f, Color.White, (float) scale, 0.01f, 0.0f, 0.0f, true)
          {
            pingPong = true,
            motion = new Vector2(6f, -1f),
            acceleration = new Vector2(-0.02f, -0.02f),
            delayBeforeAnimationStart = 900,
            startSound = "leafrustle"
          });
          this.behindSignTempSprites.Add(new TemporaryAnimatedSprite("LooseSprites\\parrots", new Rectangle(120, 24, 24, 24), 100f, 3, 999, new Vector2((float) (this.globalXOffset + this.width / 2 + 200 * scale - 24 * scale), (float) (-300 * scale) - this.viewportY / 3f * (float) scale + (float) (100 * scale)), false, true, 0.2f, 0.0f, Color.White, (float) scale, 0.01f, 0.0f, 0.0f, true)
          {
            pingPong = true,
            motion = new Vector2(6f, -1f),
            acceleration = new Vector2(-0.02f, -0.02f),
            delayBeforeAnimationStart = 1200
          });
          this.behindSignTempSprites.Add(new TemporaryAnimatedSprite("LooseSprites\\parrots", new Rectangle(120, 72, 24, 24), 95f, 3, 999, new Vector2((float) (this.globalXOffset + this.width / 2 + 200 * scale - 24 * scale), (float) (-300 * scale) - this.viewportY / 3f * (float) scale + (float) (120 * scale)), false, true, 0.2f, 0.0f, Color.White, (float) scale, 0.01f, 0.0f, 0.0f, true)
          {
            pingPong = true,
            motion = new Vector2(6f, -1f),
            acceleration = new Vector2(-0.02f, -0.02f),
            delayBeforeAnimationStart = 1500
          });
          for (int index = 0; index < 14; ++index)
            this.tempSprites.Add(new TemporaryAnimatedSprite("LooseSprites\\Cursors", new Rectangle(355, 1199, 16 /*0x10*/, 16 /*0x10*/), new Vector2((float) (this.globalXOffset + this.width / 2 - 220 * scale), (float) (-300 * scale) - this.viewportY / 3f * (float) scale + (float) (60 * scale) + (float) (Game1.random.Next(100) * scale)), Game1.random.NextBool(), 0.0f, new Color(180, 180, 240 /*0xF0*/))
            {
              scale = (float) scale,
              animationLength = 11,
              interval = (float) (50 + Game1.random.Next(50)),
              totalNumberOfLoops = 999,
              motion = new Vector2((float) Game1.random.Next(-100, 101) / 100f, (float) (1.0 + (double) Game1.random.Next(-100, 100) / 500.0)),
              xPeriodic = Game1.random.NextBool(),
              xPeriodicLoopTime = (float) Game1.random.Next(6000, 16000),
              xPeriodicRange = (float) Game1.random.Next(64 /*0x40*/, 192 /*0xC0*/),
              alphaFade = 1f / 1000f,
              local = true,
              holdLastFrame = false,
              delayBeforeAnimationStart = 100 + index * 20
            });
          for (int index = 0; index < 14; ++index)
            this.tempSprites.Add(new TemporaryAnimatedSprite("LooseSprites\\Cursors", new Rectangle(355, 1199, 16 /*0x10*/, 16 /*0x10*/), new Vector2((float) (this.globalXOffset + this.width / 2 + 220 * scale), (float) (-300 * scale) - this.viewportY / 3f * (float) scale + (float) (60 * scale) + (float) (Game1.random.Next(100) * scale)), Game1.random.NextBool(), 0.0f, new Color(180, 180, 240 /*0xF0*/))
            {
              scale = (float) scale,
              animationLength = 11,
              interval = (float) (50 + Game1.random.Next(50)),
              totalNumberOfLoops = 999,
              motion = new Vector2((float) Game1.random.Next(-100, 101) / 100f, (float) (1.0 + (double) Game1.random.Next(-100, 100) / 500.0)),
              xPeriodic = Game1.random.NextBool(),
              xPeriodicLoopTime = (float) Game1.random.Next(6000, 16000),
              xPeriodicRange = (float) Game1.random.Next(64 /*0x40*/, 192 /*0xC0*/),
              alphaFade = 1f / 1000f,
              local = true,
              holdLastFrame = false,
              delayBeforeAnimationStart = 900 + index * 20
            });
        }
      }
    }
    for (int index1 = this.bigClouds.Count - 1; index1 >= 0; --index1)
    {
      this.bigClouds[index1] -= 0.1f;
      List<float> bigClouds = this.bigClouds;
      int index2 = index1;
      List<float> floatList = bigClouds;
      int index3 = index2;
      double num3 = (double) bigClouds[index2];
      int buttonsDx = this.buttonsDX;
      elapsedGameTime = time.ElapsedGameTime;
      int milliseconds = elapsedGameTime.Milliseconds;
      double num4 = (double) (buttonsDx * milliseconds / 2);
      double num5 = num3 + num4;
      floatList[index3] = (float) num5;
      if ((double) this.bigClouds[index1] < (double) (-512 * TitleMenu.pixelZoom))
        this.bigClouds[index1] = (float) this.width;
    }
    for (int index4 = this.smallClouds.Count - 1; index4 >= 0; --index4)
    {
      this.smallClouds[index4] -= 0.3f;
      List<float> smallClouds = this.smallClouds;
      int index5 = index4;
      List<float> floatList = smallClouds;
      int index6 = index5;
      double num6 = (double) smallClouds[index5];
      int buttonsDx = this.buttonsDX;
      elapsedGameTime = time.ElapsedGameTime;
      int milliseconds = elapsedGameTime.Milliseconds;
      double num7 = (double) (buttonsDx * milliseconds / 2);
      double num8 = num6 + num7;
      floatList[index6] = (float) num8;
      if ((double) this.smallClouds[index4] < (double) (-149 * TitleMenu.pixelZoom))
        this.smallClouds[index4] = (float) this.width;
    }
    this.tempSprites.RemoveWhere<TemporaryAnimatedSprite>((Predicate<TemporaryAnimatedSprite>) (sprite => sprite.update(time)));
    this.behindSignTempSprites.RemoveWhere<TemporaryAnimatedSprite>((Predicate<TemporaryAnimatedSprite>) (sprite => sprite.update(time)));
    this.birds.RemoveWhere<TemporaryAnimatedSprite>((Predicate<TemporaryAnimatedSprite>) (bird =>
    {
      bird.position.Y -= this.viewportDY * 2f;
      return bird.update(time);
    }));
  }

  private void moveFeatures(int dx, int dy)
  {
    foreach (TemporaryAnimatedSprite tempSprite in this.tempSprites)
    {
      tempSprite.position.X += (float) dx;
      tempSprite.position.Y += (float) dy;
    }
    foreach (TemporaryAnimatedSprite behindSignTempSprite in this.behindSignTempSprites)
    {
      behindSignTempSprite.position.X += (float) dx;
      behindSignTempSprite.position.Y += (float) dy;
    }
    foreach (ClickableTextureComponent button in this.buttons)
    {
      button.bounds.X += dx;
      button.bounds.Y += dy;
    }
  }

  /// <inheritdoc />
  public override void receiveScrollWheelAction(int direction)
  {
    if (!this.ShouldAllowInteraction())
      return;
    base.receiveScrollWheelAction(direction);
    TitleMenu.subMenu?.receiveScrollWheelAction(direction);
  }

  /// <inheritdoc />
  public override void performHoverAction(int x, int y)
  {
    if (!this.ShouldAllowInteraction())
    {
      x = int.MinValue;
      y = int.MinValue;
    }
    base.performHoverAction(x, y);
    this.muteMusicButton.tryHover(x, y);
    if (TitleMenu.subMenu != null)
    {
      TitleMenu.subMenu.performHoverAction(x, y);
      if (this.backButton == null || !TitleMenu.subMenu.readyToClose())
        return;
      if (this.backButton.containsPoint(x, y))
      {
        if (this.backButton.sourceRect.Y == 252)
          Game1.playSound("Cowboy_Footstep");
        this.backButton.sourceRect.Y = 279;
      }
      else
        this.backButton.sourceRect.Y = 252;
      this.backButton.tryHover(x, y, 0.25f);
    }
    else
    {
      if (!this.titleInPosition || !this.HasActiveUser)
        return;
      foreach (ClickableTextureComponent button in this.buttons)
      {
        if (button.containsPoint(x, y))
        {
          if (button.sourceRect.Y == 187)
            Game1.playSound("Cowboy_Footstep");
          button.sourceRect.Y = 245;
        }
        else
          button.sourceRect.Y = 187;
        button.tryHover(x, y, 0.25f);
      }
      this.aboutButton.tryHover(x, y, 0.25f);
      if (this.aboutButton.containsPoint(x, y))
      {
        if (this.aboutButton.sourceRect.X == 8)
          Game1.playSound("Cowboy_Footstep");
        this.aboutButton.sourceRect.X = 30;
      }
      else
        this.aboutButton.sourceRect.X = 8;
      if (!this.languageButton.visible)
        return;
      this.languageButton.tryHover(x, y, 0.25f);
      if (this.languageButton.containsPoint(x, y))
      {
        if (this.languageButton.sourceRect.X == 52)
          Game1.playSound("Cowboy_Footstep");
        this.languageButton.sourceRect.X = 79;
      }
      else
        this.languageButton.sourceRect.X = 52;
    }
  }

  /// <inheritdoc />
  public override void draw(SpriteBatch b)
  {
    int num1;
    switch (TitleMenu.subMenu)
    {
      case null:
      case AboutMenu _:
        num1 = 1;
        break;
      default:
        num1 = TitleMenu.subMenu is LanguageSelectionMenu ? 1 : 0;
        break;
    }
    bool flag1 = num1 != 0;
    b.Draw(Game1.staminaRect, new Rectangle(0, 0, this.width, this.height), new Color(64 /*0x40*/, 136, 248));
    b.Draw(Game1.mouseCursors, new Rectangle(0, (int) ((double) (-300 * TitleMenu.pixelZoom) - (double) this.viewportY * 0.6600000262260437), this.width, 300 * TitleMenu.pixelZoom + this.height - 120 * TitleMenu.pixelZoom), new Rectangle?(new Rectangle(703, 1912, 1, 264)), Color.White);
    if (!this.whichSubMenu.Equals("Load"))
    {
      for (int index = -10; index < this.width; index += 638)
        b.Draw(Game1.mouseCursors, new Vector2((float) (index * TitleMenu.pixelZoom), (float) (-360 * TitleMenu.pixelZoom) - this.viewportY * 0.66f), new Rectangle?(new Rectangle(0, 1453, 638, 195)), Color.White * (float) (1.0 - (double) this.globalXOffset / 1200.0), 0.0f, Vector2.Zero, 4f, SpriteEffects.None, 0.8f);
    }
    foreach (float bigCloud in this.bigClouds)
      b.Draw(this.cloudsTexture, new Vector2(bigCloud, (float) (this.height - 250 * TitleMenu.pixelZoom) - this.viewportY * 0.5f), new Rectangle?(new Rectangle(0, 0, 512 /*0x0200*/, 337)), Color.White * this.globalCloudAlpha, 0.0f, Vector2.Zero, (float) TitleMenu.pixelZoom, SpriteEffects.None, 0.01f);
    b.Draw(Game1.mouseCursors, new Vector2((float) (-30 * TitleMenu.pixelZoom), (float) (this.height - 158 * TitleMenu.pixelZoom) - this.viewportY * 0.66f), new Rectangle?(new Rectangle(0, 886, 639, 148)), Color.White, 0.0f, Vector2.Zero, (float) TitleMenu.pixelZoom, SpriteEffects.None, 0.08f);
    b.Draw(Game1.mouseCursors, new Vector2((float) (-30 * TitleMenu.pixelZoom + 639 * TitleMenu.pixelZoom), (float) (this.height - 158 * TitleMenu.pixelZoom) - this.viewportY * 0.66f), new Rectangle?(new Rectangle(0, 886, 640, 148)), Color.White, 0.0f, Vector2.Zero, (float) TitleMenu.pixelZoom, SpriteEffects.None, 0.08f);
    for (int index = 0; index < this.smallClouds.Count; ++index)
      b.Draw(this.cloudsTexture, new Vector2(this.smallClouds[index], (float) (this.height - 300 * TitleMenu.pixelZoom - index * 12 * TitleMenu.pixelZoom) - this.viewportY * 0.5f), new Rectangle?(index % 3 == 0 ? new Rectangle(152, 447, 123, 55) : (index % 3 == 1 ? new Rectangle(0, 471, 149, 66) : new Rectangle(410, 467, 63 /*0x3F*/, 37))), Color.White * this.globalCloudAlpha, 0.0f, Vector2.Zero, (float) TitleMenu.pixelZoom, SpriteEffects.None, 0.01f);
    b.Draw(Game1.mouseCursors, new Vector2(0.0f, (float) (this.height - 148 * TitleMenu.pixelZoom) - this.viewportY * 1f), new Rectangle?(new Rectangle(0, 737, 639, 148)), Color.White, 0.0f, Vector2.Zero, (float) TitleMenu.pixelZoom, SpriteEffects.None, 0.1f);
    b.Draw(Game1.mouseCursors, new Vector2((float) (639 * TitleMenu.pixelZoom), (float) (this.height - 148 * TitleMenu.pixelZoom) - this.viewportY * 1f), new Rectangle?(new Rectangle(0, 737, 640, 148)), Color.White, 0.0f, Vector2.Zero, (float) TitleMenu.pixelZoom, SpriteEffects.None, 0.1f);
    foreach (TemporaryAnimatedSprite bird in this.birds)
      bird.draw(b);
    b.Draw(this.cloudsTexture, new Vector2(0.0f, (float) (this.height - 142 * TitleMenu.pixelZoom) - this.viewportY * 2f), new Rectangle?(new Rectangle(0, 554, 165, 142)), Color.White, 0.0f, Vector2.Zero, (float) TitleMenu.pixelZoom, SpriteEffects.None, 0.2f);
    b.Draw(this.cloudsTexture, new Vector2((float) (this.width - 122 * TitleMenu.pixelZoom), (float) (this.height - 153 * TitleMenu.pixelZoom) - this.viewportY * 2f), new Rectangle?(new Rectangle(390, 543, 122, 153)), Color.White, 0.0f, Vector2.Zero, (float) TitleMenu.pixelZoom, SpriteEffects.None, 0.2f);
    int scale = this.ShouldShrinkLogo() ? 2 : TitleMenu.pixelZoom;
    if (!this.whichSubMenu.Equals("Load") && !this.whichSubMenu.Equals("Co-op"))
    {
      int num2;
      switch (TitleMenu.subMenu)
      {
        case LoadGameMenu _:
          goto label_27;
        case CharacterCustomization characterCustomization:
          num2 = characterCustomization.source == CharacterCustomization.Source.HostNewFarm ? 1 : 0;
          break;
        default:
          num2 = 0;
          break;
      }
      if (num2 == 0 && !this.transitioningFromLoadScreen)
        goto label_33;
    }
label_27:
    Texture2D mouseCursors = Game1.mouseCursors;
    Rectangle destinationRectangle = new Rectangle(0, 0, this.width, this.height);
    Rectangle rectangle = new Rectangle(702, 1912, 1, 264);
    b.Draw(mouseCursors, destinationRectangle, new Rectangle?(rectangle), Color.White * ((float) this.globalXOffset / 1200f));
    SpriteEffects effects = SpriteEffects.None;
    for (int y = 0; y < this.height; y += 195)
    {
      for (int x = 0; x < this.width; x += 638)
        b.Draw(Game1.mouseCursors, new Vector2((float) x, (float) y) * 4f, new Rectangle?(new Rectangle(0, 1453, 638, 195)), Color.White * ((float) this.globalXOffset / 1200f), 0.0f, Vector2.Zero, 4f, effects, 0.8f);
      effects = effects == SpriteEffects.None ? SpriteEffects.FlipHorizontally : SpriteEffects.None;
    }
label_33:
    if (flag1)
    {
      foreach (TemporaryAnimatedSprite behindSignTempSprite in this.behindSignTempSprites)
        behindSignTempSprite.draw(b);
      if (this.showCornerClickEasterEgg && Game1.content.GetCurrentLanguage() != LocalizedContentManager.LanguageCode.zh)
      {
        float num3 = 1f - Math.Min(1f, (float) (1.0 - (double) this.cornerClickEndTimer / 700.0));
        float num4 = (float) (40 * scale) * num3;
        Vector2 vector2 = new Vector2((float) (this.globalXOffset + this.width / 2 - 200 * scale), (float) (-300 * scale) - this.viewportY / 3f * (float) scale);
        b.Draw(Game1.mouseCursors2, vector2 + new Vector2((float) (80 /*0x50*/ * scale), (float) (-10 * scale) + num4), new Rectangle?(new Rectangle(224 /*0xE0*/, 148, 32 /*0x20*/, 21)), Color.White, 0.0f, Vector2.Zero, (float) scale, SpriteEffects.None, 0.01f);
        b.Draw(Game1.mouseCursors2, vector2 + new Vector2((float) (120 * scale), (float) (-15 * scale) + num4), new Rectangle?(new Rectangle(224 /*0xE0*/, 148, 32 /*0x20*/, 21)), Color.White, 0.0f, Vector2.Zero, (float) scale, SpriteEffects.None, 0.01f);
        b.Draw(Game1.mouseCursors, vector2 + new Vector2((float) (160 /*0xA0*/ * scale), (float) (-25 * scale) + num4), new Rectangle?(new Rectangle(646, 895, 55, 48 /*0x30*/)), Color.White, 0.0f, Vector2.Zero, (float) scale, SpriteEffects.None, 0.01f);
        b.Draw(Game1.mouseCursors2, vector2 + new Vector2((float) (220 * scale), (float) (-15 * scale) + num4), new Rectangle?(new Rectangle(224 /*0xE0*/, 148, 32 /*0x20*/, 21)), Color.White, 0.0f, Vector2.Zero, (float) scale, SpriteEffects.None, 0.01f);
        b.Draw(Game1.mouseCursors2, vector2 + new Vector2((float) (260 * scale), (float) (-5 * scale) + num4), new Rectangle?(new Rectangle(224 /*0xE0*/, 148, 32 /*0x20*/, 21)), Color.White, 0.0f, Vector2.Zero, (float) scale, SpriteEffects.None, 0.01f);
        float num5 = (float) (40 * scale) * num3;
        b.Draw(Game1.mouseCursors2, vector2 + new Vector2((float) (-10 * scale) + num5, (float) (70 * scale)), new Rectangle?(new Rectangle(224 /*0xE0*/, 148, 32 /*0x20*/, 21)), Color.White, -1.57079637f, Vector2.Zero, (float) scale, SpriteEffects.None, 0.01f);
        b.Draw(Game1.mouseCursors2, vector2 + new Vector2((float) (-5 * scale) + num5, (float) (100 * scale)), new Rectangle?(new Rectangle(224 /*0xE0*/, 148, 32 /*0x20*/, 21)), Color.White, -1.57079637f, Vector2.Zero, (float) scale, SpriteEffects.None, 0.01f);
        b.Draw(Game1.mouseCursors2, vector2 + new Vector2((float) (-12 * scale) + num5, (float) (130 * scale)), new Rectangle?(new Rectangle(224 /*0xE0*/, 148, 32 /*0x20*/, 21)), Color.White, -1.57079637f, Vector2.Zero, (float) scale, SpriteEffects.None, 0.01f);
        b.Draw(Game1.mouseCursors2, vector2 + new Vector2((float) (-10 * scale) + num5, (float) (160 /*0xA0*/ * scale)), new Rectangle?(new Rectangle(224 /*0xE0*/, 148, 32 /*0x20*/, 21)), Color.White, -1.57079637f, Vector2.Zero, (float) scale, SpriteEffects.None, 0.01f);
        float num6 = (float) (-40 * scale) * num3;
        b.Draw(Game1.mouseCursors2, vector2 + new Vector2((float) (410 * scale) + num6, (float) (40 * scale)), new Rectangle?(new Rectangle(224 /*0xE0*/, 148, 32 /*0x20*/, 21)), Color.White, 1.57079637f, Vector2.Zero, (float) scale, SpriteEffects.None, 0.01f);
        b.Draw(Game1.mouseCursors2, vector2 + new Vector2((float) (415 * scale) + num6, (float) (70 * scale)), new Rectangle?(new Rectangle(224 /*0xE0*/, 148, 32 /*0x20*/, 21)), Color.White, 1.57079637f, Vector2.Zero, (float) scale, SpriteEffects.None, 0.01f);
        b.Draw(Game1.mouseCursors2, vector2 + new Vector2((float) (405 * scale) + num6, (float) (100 * scale)), new Rectangle?(new Rectangle(224 /*0xE0*/, 148, 32 /*0x20*/, 21)), Color.White, 1.57079637f, Vector2.Zero, (float) scale, SpriteEffects.None, 0.01f);
        b.Draw(Game1.mouseCursors2, vector2 + new Vector2((float) (410 * scale) + num6, (float) (130 * scale)), new Rectangle?(new Rectangle(224 /*0xE0*/, 148, 32 /*0x20*/, 21)), Color.White, 1.57079637f, Vector2.Zero, (float) scale, SpriteEffects.None, 0.01f);
      }
      b.Draw(this.titleButtonsTexture, new Vector2((float) (this.globalXOffset + this.width / 2 - 200 * scale), (float) (-300 * scale) - this.viewportY / 3f * (float) scale), new Rectangle?(new Rectangle(0, 0, 400, 187)), Color.White, 0.0f, Vector2.Zero, (float) scale, SpriteEffects.None, 0.2f);
      if ((double) this.logoSwipeTimer > 0.0)
        b.Draw(this.titleButtonsTexture, new Vector2((float) (this.globalXOffset + this.width / 2), (float) (-300 * scale) - this.viewportY / 3f * (float) scale + (float) (93 * scale)), new Rectangle?(new Rectangle(0, 0, 400, 187)), Color.White, 0.0f, new Vector2(200f, 93f), (float) scale + (float) ((0.5 - (double) Math.Abs((float) ((double) this.logoSwipeTimer / 1000.0 - 0.5))) * 0.10000000149011612), SpriteEffects.None, 0.2f);
      if (this.cornerPhaseHolding && this.cornerClicks > 999 && Game1.content.GetCurrentLanguage() != LocalizedContentManager.LanguageCode.zh)
        b.Draw(Game1.mouseCursors2, new Vector2((float) (this.globalXOffset + this.r_hole_rect.X + scale), (float) (this.r_hole_rect.Y - 2)), new Rectangle?(new Rectangle(131, 196, 9, 10)), Color.White, 0.0f, Vector2.Zero, (float) scale, SpriteEffects.None, 0.24f);
    }
    if (flag1)
    {
      bool flag2 = TitleMenu.subMenu is AboutMenu || TitleMenu.subMenu is LanguageSelectionMenu;
      for (int index = 0; index < this.buttonsToShow; ++index)
      {
        if (this.buttons.Count > index)
          this.buttons[index].draw(b, TitleMenu.subMenu == null || !flag2 ? Color.White : Color.LightGray * 0.8f, 1f);
      }
      if (TitleMenu.subMenu == null)
      {
        foreach (TemporaryAnimatedSprite tempSprite in this.tempSprites)
          tempSprite.draw(b);
      }
    }
    if (TitleMenu.subMenu != null && !this.isTransitioningButtons)
    {
      if (this.backButton != null && TitleMenu.subMenu.readyToClose())
        this.backButton.draw(b);
      TitleMenu.subMenu.draw(b);
      if (this.backButton != null && !(TitleMenu.subMenu is CharacterCustomization) && TitleMenu.subMenu.readyToClose())
        this.backButton.draw(b);
    }
    else if (TitleMenu.subMenu == null && this.isTransitioningButtons && (this.whichSubMenu.Equals("Load") || this.whichSubMenu.Equals("New")))
    {
      int x = 84;
      int y = Game1.uiViewport.Height - 64 /*0x40*/;
      int width = 0;
      int height = 64 /*0x40*/;
      Utility.makeSafe(ref x, ref y, width, height);
      SpriteText.drawStringWithScrollBackground(b, Game1.content.LoadString("Strings\\StringsFromCSFiles:Game1.cs.3689"), x, y);
    }
    else if (((TitleMenu.subMenu != null || this.isTransitioningButtons || !this.titleInPosition || this.transitioningCharacterCreationMenu ? 0 : (this.HasActiveUser ? 1 : 0)) & (flag1 ? 1 : 0)) != 0)
    {
      this.aboutButton.draw(b);
      this.languageButton.draw(b);
    }
    if (this.amuzioTimer > 0)
    {
      b.Draw(Game1.staminaRect, new Rectangle(0, 0, this.width, this.height), Color.White);
      Vector2 position = new Vector2((float) (this.width / 2 - this.amuzioTexture.Width / 2 * 4), (float) (this.height / 2 - this.amuzioTexture.Height / 2 * 4));
      position.X = MathHelper.Lerp(position.X, (float) (-this.amuzioTexture.Width * 4), (float) Math.Max(0, this.amuzioTimer - 3750) / 250f);
      b.Draw(this.amuzioTexture, position, new Rectangle?(), Color.White * Math.Min(1f, (float) this.amuzioTimer / 500f), 0.0f, Vector2.Zero, 4f, SpriteEffects.None, 0.2f);
    }
    else if (this.logoFadeTimer > 0 || this.fadeFromWhiteTimer > 0)
    {
      b.Draw(Game1.staminaRect, new Rectangle(0, 0, this.width, this.height), Color.White * ((float) this.fadeFromWhiteTimer / 2000f));
      if (!this.specialSurprised)
        b.Draw(this.titleButtonsTexture, new Vector2((float) (this.width / 2), (float) (this.height / 2 - 30 * TitleMenu.pixelZoom)), new Rectangle?(new Rectangle(171 + (this.logoFadeTimer / 100 % 2 != 0 || this.logoSurprisedTimer > 0 ? 0 : 111), 311, 111, 60)), Color.White * (this.logoFadeTimer < 500 ? (float) this.logoFadeTimer / 500f : (this.logoFadeTimer > 4500 ? (float) (1.0 - (double) (this.logoFadeTimer - 4500) / 500.0) : 1f)), 0.0f, Vector2.Zero, (float) TitleMenu.pixelZoom, SpriteEffects.None, 0.2f);
      if (this.logoSurprisedTimer <= 0)
        b.Draw(this.titleButtonsTexture, new Vector2((float) (this.width / 2 - 87 * TitleMenu.pixelZoom), (float) (this.height / 2 - 34 * TitleMenu.pixelZoom)), new Rectangle?(new Rectangle(this.logoFadeTimer / 100 % 2 == 0 ? 85 : 0, 306 + (this.shades ? 69 : 0), 85, 69)), Color.White * (this.logoFadeTimer < 500 ? (float) this.logoFadeTimer / 500f : (this.logoFadeTimer > 4500 ? (float) (1.0 - (double) (this.logoFadeTimer - 4500) / 500.0) : 1f)), 0.0f, Vector2.Zero, (float) TitleMenu.pixelZoom, SpriteEffects.None, 0.2f);
      if (this.specialSurprised)
      {
        if (this.logoFadeTimer > 0)
          b.Draw(Game1.staminaRect, new Rectangle(0, 0, this.width, this.height), new Color(221, (int) byte.MaxValue, 198));
        b.Draw(Game1.staminaRect, new Rectangle(0, 0, this.width, this.height), new Color(221, (int) byte.MaxValue, 198) * ((float) this.fadeFromWhiteTimer / 2000f));
        int totalMilliseconds = (int) Game1.currentGameTime.TotalGameTime.TotalMilliseconds;
        for (int x = 64 /*0x40*/; x < this.width + 1000; x += 192 /*0xC0*/)
        {
          for (int y = -1000; y < this.height; y += 192 /*0xC0*/)
            b.Draw(Game1.mouseCursors, new Vector2((float) x, (float) y) + new Vector2((float) -totalMilliseconds / 20f, (float) totalMilliseconds / 20f), new Rectangle?(new Rectangle(355 + (totalMilliseconds + x * 77 + y * 77) / 12 % 110 / 11 * 16 /*0x10*/, 1200, 16 /*0x10*/, 16 /*0x10*/)), Color.White * 0.66f * ((float) (this.fadeFromWhiteTimer - (2000 - this.fadeFromWhiteTimer)) / 2000f), 0.0f, Vector2.Zero, (float) TitleMenu.pixelZoom, SpriteEffects.None, 0.18f);
        }
        b.Draw(this.titleButtonsTexture, new Vector2((float) (this.width / 2), (float) (this.height / 2 - 30 * TitleMenu.pixelZoom)), new Rectangle?(new Rectangle(171 + (totalMilliseconds / 200 % 2 == 0 ? 111 : 0), 563, 111, 60)), Color.White * ((float) (this.fadeFromWhiteTimer - (2000 - this.fadeFromWhiteTimer)) / 2000f), 0.0f, Vector2.Zero, (float) TitleMenu.pixelZoom, SpriteEffects.None, 0.2f);
        this.specialSurprisedTimeStamp += (float) Game1.currentGameTime.ElapsedGameTime.TotalSeconds;
        Utility.drawWithShadow(b, this.titleButtonsTexture, new Vector2((float) (this.width / 2 - 87 * TitleMenu.pixelZoom), (float) (this.height / 2 - 34 * TitleMenu.pixelZoom)), new Rectangle(totalMilliseconds / 200 % 2 == 0 ? 85 : 0, 559, 85, 69), Color.White * ((float) (this.fadeFromWhiteTimer - (2000 - this.fadeFromWhiteTimer)) / 2000f), 0.0f, Vector2.Zero, (float) TitleMenu.pixelZoom, layerDepth: 0.2f, horizontalShadowOffset: -4, verticalShadowOffset: -4, shadowIntensity: 0.0f);
      }
      else if (this.logoSurprisedTimer > 0)
        b.Draw(this.titleButtonsTexture, new Vector2((float) (this.width / 2 - 87 * TitleMenu.pixelZoom), (float) (this.height / 2 - 34 * TitleMenu.pixelZoom)), new Rectangle?(new Rectangle(this.logoSurprisedTimer > 800 || this.logoSurprisedTimer < 400 ? 176 /*0xB0*/ : 260, 375, 85, 69)), Color.White * (this.logoSurprisedTimer < 200 ? (float) this.logoSurprisedTimer / 200f : 1f), 0.0f, Vector2.Zero, (float) TitleMenu.pixelZoom, SpriteEffects.None, 0.22f);
      if (this.startupMessage.Length > 0 && this.logoFadeTimer > 0)
        b.DrawString(Game1.smallFont, Game1.parseText(this.startupMessage, Game1.smallFont, 640), new Vector2(8f, (float) ((double) Game1.uiViewport.Height - (double) Game1.smallFont.MeasureString(Game1.parseText(this.startupMessage, Game1.smallFont, 640)).Y - 4.0)), this.startupMessageColor * (this.logoFadeTimer < 500 ? (float) this.logoFadeTimer / 500f : (this.logoFadeTimer > 4500 ? (float) (1.0 - (double) (this.logoFadeTimer - 4500) / 500.0) : 1f)));
    }
    if (this.quitTimer > 0)
      b.Draw(Game1.staminaRect, new Rectangle(0, 0, this.width, this.height), Color.Black * (float) (1.0 - (double) this.quitTimer / 500.0));
    if (this.HasActiveUser)
    {
      this.muteMusicButton.draw(b);
      this.windowedButton.draw(b);
    }
    if (!this.ShouldDrawCursor())
      return;
    int cursor = -1;
    if (TitleMenu.subMenu is LoadGameMenu)
      cursor = (TitleMenu.subMenu as LoadGameMenu).IsDoingTask() ? 1 : -1;
    this.drawMouse(b, cursor: cursor);
    if (!this.cornerPhaseHolding || this.cornerClicks >= 100)
      return;
    b.Draw(Game1.mouseCursors2, new Vector2((float) (Game1.getMouseX() + 32 /*0x20*/ + 4), (float) (Game1.getMouseY() + 32 /*0x20*/ + 4)), new Rectangle?(new Rectangle(131, 196, 9, 10)), Color.White, 0.0f, Vector2.Zero, (float) scale, SpriteEffects.None, 0.9999f);
  }

  protected bool ShouldAllowInteraction()
  {
    if (this.quitTimer > 0 || this.isTransitioningButtons || this.showButtonsTimer > 0 && this.HasActiveUser && TitleMenu.subMenu == null)
      return false;
    if (TitleMenu.subMenu != null)
    {
      if (TitleMenu.subMenu is LoadGameMenu subMenu && subMenu.IsDoingTask())
        return false;
    }
    else if (!this.titleInPosition)
      return false;
    return true;
  }

  protected bool ShouldDrawCursor()
  {
    if (!Game1.options.gamepadControls || !Game1.options.snappyMenus)
      return true;
    if (this.pauseBeforeViewportRiseTimer > 0 || (double) this.logoSwipeTimer > 0.0)
      return false;
    return this.logoFadeTimer > 0 ? this._movedCursor : this.fadeFromWhiteTimer <= 0 && this.titleInPosition && (double) this.viewportDY == 0.0 && !(TitleMenu._subMenu is TooManyFarmsMenu) && this.ShouldAllowInteraction();
  }

  /// <inheritdoc />
  public override void gameWindowSizeChanged(Rectangle oldBounds, Rectangle newBounds)
  {
    if (this.globalXOffset >= this.width)
      this.globalXOffset = Game1.uiViewport.Width;
    this.width = Game1.uiViewport.Width;
    this.height = Game1.uiViewport.Height;
    this.setUpIcons();
    TitleMenu.subMenu?.gameWindowSizeChanged(oldBounds, newBounds);
    ClickableTextureComponent textureComponent1 = new ClickableTextureComponent(this.menuContent.LoadString("Strings\\StringsFromCSFiles:TitleMenu.cs.11739"), new Rectangle(this.width + -66 * TitleMenu.pixelZoom - 8 * TitleMenu.pixelZoom * 2, this.height - 27 * TitleMenu.pixelZoom - 8 * TitleMenu.pixelZoom, 66 * TitleMenu.pixelZoom, 27 * TitleMenu.pixelZoom), (string) null, "", this.titleButtonsTexture, new Rectangle(296, 252, 66, 27), (float) TitleMenu.pixelZoom);
    textureComponent1.myID = 81114;
    this.backButton = textureComponent1;
    this.tempSprites.Clear();
    if (this.birds.Count > 0 && !this.titleInPosition)
    {
      for (int index = 0; index < this.birds.Count; ++index)
        this.birds[index].position = index % 2 == 0 ? new Vector2((float) (this.width - 70 * TitleMenu.pixelZoom), (float) (this.height - 120 * TitleMenu.pixelZoom)) : new Vector2((float) (this.width - 40 * TitleMenu.pixelZoom), (float) (this.height - 110 * TitleMenu.pixelZoom));
    }
    ClickableTextureComponent textureComponent2 = new ClickableTextureComponent(new Rectangle(Game1.viewport.Width - 36 - 16 /*0x10*/, 16 /*0x10*/, 36, 36), Game1.mouseCursors, new Rectangle(Game1.options == null || Game1.options.isCurrentlyWindowed() ? 146 : 155, 384, 9, 9), 4f);
    textureComponent2.myID = 81112;
    textureComponent2.leftNeighborID = 81111;
    textureComponent2.downNeighborID = 81113;
    this.windowedButton = textureComponent2;
    if (!Game1.options.SnappyMenus)
      return;
    int id = this.currentlySnappedComponent != null ? this.currentlySnappedComponent.myID : 81115;
    this.populateClickableComponentList();
    this.currentlySnappedComponent = this.getComponentWithID(id);
    if (TitleMenu._subMenu != null)
      TitleMenu._subMenu.snapCursorToCurrentSnappedComponent();
    else
      this.snapCursorToCurrentSnappedComponent();
  }

  private void showButterflies()
  {
    Game1.playSound("yoba");
    int num = this.ShouldShrinkLogo() ? 2 : TitleMenu.pixelZoom;
    this.tempSprites.Add(new TemporaryAnimatedSprite("TileSheets\\critters", new Rectangle(128 /*0x80*/, 96 /*0x60*/, 16 /*0x10*/, 16 /*0x10*/), new Vector2((float) (this.width / 2 - 240 /*0xF0*/ * num), (float) (-300 * num - (int) ((double) this.viewportY / 3.0) * num + 86 * num)), false, 0.0f, Color.White)
    {
      scale = (float) num,
      animationLength = 4,
      totalNumberOfLoops = 999999,
      pingPong = true,
      interval = 75f,
      local = true,
      yPeriodic = true,
      yPeriodicLoopTime = 3200f,
      yPeriodicRange = 16f,
      xPeriodic = true,
      xPeriodicLoopTime = 5000f,
      xPeriodicRange = 21f,
      alpha = 1f / 1000f,
      alphaFade = -0.03f
    });
    TemporaryAnimatedSpriteList values1 = Utility.sparkleWithinArea(new Rectangle(this.width / 2 - 240 /*0xF0*/ * num - 8 * num, -300 * num - (int) ((double) this.viewportY / 3.0) * num + 86 * num - 8 * num, 80 /*0x50*/, 64 /*0x40*/), 2, Color.White * 0.75f);
    foreach (TemporaryAnimatedSprite temporaryAnimatedSprite in values1)
    {
      temporaryAnimatedSprite.local = true;
      temporaryAnimatedSprite.scale = (float) num / 4f;
    }
    this.tempSprites.AddRange((IEnumerable<TemporaryAnimatedSprite>) values1);
    this.tempSprites.Add(new TemporaryAnimatedSprite("TileSheets\\critters", new Rectangle(192 /*0xC0*/, 96 /*0x60*/, 16 /*0x10*/, 16 /*0x10*/), new Vector2((float) (this.width / 2 + 220 * num), (float) (-300 * num - (int) ((double) this.viewportY / 3.0) * num + 15 * num)), false, 0.0f, Color.White)
    {
      scale = (float) num,
      animationLength = 4,
      totalNumberOfLoops = 999999,
      pingPong = true,
      delayBeforeAnimationStart = 10,
      interval = 70f,
      local = true,
      yPeriodic = true,
      yPeriodicLoopTime = 2800f,
      yPeriodicRange = 12f,
      xPeriodic = true,
      xPeriodicLoopTime = 4000f,
      xPeriodicRange = 16f,
      alpha = 1f / 1000f,
      alphaFade = -0.03f
    });
    TemporaryAnimatedSpriteList values2 = Utility.sparkleWithinArea(new Rectangle(this.width / 2 + 220 * num - 8 * num, -300 * num - (int) ((double) this.viewportY / 3.0) * num + 15 * num - 8 * num, 80 /*0x50*/, 64 /*0x40*/), 2, Color.White * 0.75f);
    foreach (TemporaryAnimatedSprite temporaryAnimatedSprite in values2)
    {
      temporaryAnimatedSprite.local = true;
      temporaryAnimatedSprite.scale = (float) num / 4f;
    }
    this.tempSprites.AddRange((IEnumerable<TemporaryAnimatedSprite>) values2);
    this.tempSprites.Add(new TemporaryAnimatedSprite("TileSheets\\critters", new Rectangle(256 /*0x0100*/, 96 /*0x60*/, 16 /*0x10*/, 16 /*0x10*/), new Vector2((float) (this.width / 2 - 250 * num), (float) (-300 * num - (int) ((double) this.viewportY / 3.0) * num + 35 * num)), false, 0.0f, Color.White)
    {
      scale = (float) num,
      animationLength = 4,
      totalNumberOfLoops = 999999,
      pingPong = true,
      delayBeforeAnimationStart = 20,
      interval = 65f,
      local = true,
      yPeriodic = true,
      yPeriodicLoopTime = 3500f,
      yPeriodicRange = 16f,
      xPeriodic = true,
      xPeriodicLoopTime = 3000f,
      xPeriodicRange = 10f,
      alpha = 1f / 1000f,
      alphaFade = -0.03f
    });
    TemporaryAnimatedSpriteList values3 = Utility.sparkleWithinArea(new Rectangle(this.width / 2 - 250 * num - 8 * num, -300 * num - (int) ((double) this.viewportY / 3.0) * num + 35 * num - 8 * num, 80 /*0x50*/, 64 /*0x40*/), 2, Color.White * 0.75f);
    foreach (TemporaryAnimatedSprite temporaryAnimatedSprite in values3)
    {
      temporaryAnimatedSprite.local = true;
      temporaryAnimatedSprite.scale = (float) num / 4f;
    }
    this.tempSprites.AddRange((IEnumerable<TemporaryAnimatedSprite>) values3);
    this.tempSprites.Add(new TemporaryAnimatedSprite("TileSheets\\critters", new Rectangle(256 /*0x0100*/, 112 /*0x70*/, 16 /*0x10*/, 16 /*0x10*/), new Vector2((float) (this.width / 2 + 250 * num), (float) (-300 * num - (int) ((double) this.viewportY / 3.0) * num + 60 * num)), false, 0.0f, Color.White)
    {
      scale = (float) num,
      animationLength = 4,
      totalNumberOfLoops = 999999,
      yPeriodic = true,
      yPeriodicLoopTime = 3000f,
      yPeriodicRange = 16f,
      pingPong = true,
      delayBeforeAnimationStart = 30,
      interval = 85f,
      local = true,
      xPeriodic = true,
      xPeriodicLoopTime = 5000f,
      xPeriodicRange = 16f,
      alpha = 1f / 1000f,
      alphaFade = -0.03f
    });
    TemporaryAnimatedSpriteList values4 = Utility.sparkleWithinArea(new Rectangle(this.width / 2 + 250 * num - 8 * num, -300 * num - (int) ((double) this.viewportY / 3.0) * num + 60 * num - 8 * num, 80 /*0x50*/, 64 /*0x40*/), 2, Color.White * 0.75f);
    foreach (TemporaryAnimatedSprite temporaryAnimatedSprite in values4)
    {
      temporaryAnimatedSprite.local = true;
      temporaryAnimatedSprite.scale = (float) num / 4f;
    }
    this.tempSprites.AddRange((IEnumerable<TemporaryAnimatedSprite>) values4);
  }

  protected virtual void Dispose(bool disposing)
  {
    if (this.disposedValue)
      return;
    if (disposing)
    {
      this.tempSprites?.Clear();
      if (this.menuContent != null)
      {
        this.menuContent.Dispose();
        this.menuContent = (LocalizedContentManager) null;
      }
      LocalizedContentManager.OnLanguageChange -= new LocalizedContentManager.LanguageChangedHandler(this.OnLanguageChange);
      TitleMenu.subMenu = (IClickableMenu) null;
    }
    this.disposedValue = true;
  }

  ~TitleMenu() => this.Dispose(false);

  public void Dispose()
  {
    this.Dispose(true);
    GC.SuppressFinalize((object) this);
  }
}
