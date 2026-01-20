// Decompiled with JetBrains decompiler
// Type: StardewValley.Menus.BuildingPaintMenu
// Assembly: Stardew Valley, Version=1.6.15.24356, Culture=neutral, PublicKeyToken=null
// MVID: AA1F513A-94F0-4EF6-A35F-2D5B4A3721BF
// Assembly location: D:\SteamLibrary\steamapps\common\Stardew Valley\Stardew Valley.dll
// XML documentation location: D:\SteamLibrary\steamapps\common\Stardew Valley\Stardew Valley.xml

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using StardewValley.BellsAndWhistles;
using StardewValley.Buildings;
using System;
using System.Collections.Generic;

#nullable disable
namespace StardewValley.Menus;

public class BuildingPaintMenu : IClickableMenu
{
  public const int region_colorButtons = 1000;
  public const int region_okButton = 101;
  public const int region_nextRegion = 102;
  public const int region_prevRegion = 103;
  public const int region_copyColor = 104;
  public const int region_defaultColor = 105;
  public const int region_hueSlider = 106;
  public const int region_saturationSlider = 107;
  public const int region_lightnessSlider = 108;
  public const int region_appearanceButton = 109;
  public static int WINDOW_WIDTH = 1024 /*0x0400*/;
  public static int WINDOW_HEIGHT = 576;
  public Rectangle previewPane;
  public Rectangle colorPane;
  public BuildingPaintMenu.BuildingColorSlider activeSlider;
  public ClickableTextureComponent appearanceButton;
  public ClickableTextureComponent okButton;
  public static List<Vector3> savedColors = (List<Vector3>) null;
  public List<Color> buttonColors = new List<Color>();
  public BuildingPaintMenu.ColorSliderPanel colorSliderPanel;
  private string hoverText = "";
  public Building building;
  public string buildingType = "";
  public BuildingPaintColor colorTarget;
  protected Dictionary<string, string> _paintData;
  public int currentPaintRegion;
  /// <summary>The paint regions for the building.</summary>
  public List<BuildingPaintMenu.RegionData> regions;
  public ClickableTextureComponent nextRegionButton;
  public ClickableTextureComponent previousRegionButton;
  public ClickableTextureComponent copyColorButton;
  public ClickableTextureComponent defaultColorButton;
  public List<ClickableTextureComponent> savedColorButtons = new List<ClickableTextureComponent>();
  public List<ClickableComponent> sliderHandles = new List<ClickableComponent>();

  public BuildingPaintMenu(Building target_building)
    : base(Game1.uiViewport.Width / 2 - BuildingPaintMenu.WINDOW_WIDTH / 2, Game1.uiViewport.Height / 2 - BuildingPaintMenu.WINDOW_HEIGHT / 2, BuildingPaintMenu.WINDOW_WIDTH, BuildingPaintMenu.WINDOW_HEIGHT)
  {
    this.InitializeSavedColors();
    this._paintData = DataLoader.PaintData(Game1.content);
    Game1.player.Halt();
    this.building = target_building;
    this.colorTarget = target_building.netBuildingPaintColor.Value;
    this.buildingType = this.building.buildingType.Value;
    this.SetRegion(0);
    this.populateClickableComponentList();
    if (!Game1.options.SnappyMenus)
      return;
    this.snapToDefaultClickableComponent();
  }

  public virtual void InitializeSavedColors()
  {
    if (BuildingPaintMenu.savedColors != null)
      return;
    BuildingPaintMenu.savedColors = new List<Vector3>();
  }

  public override void snapToDefaultClickableComponent()
  {
    this.currentlySnappedComponent = this.getComponentWithID(101);
    this.snapCursorToCurrentSnappedComponent();
  }

  public override void applyMovementKey(int direction)
  {
    if (this.colorSliderPanel.ApplyMovementKey(direction))
      return;
    base.applyMovementKey(direction);
  }

  /// <inheritdoc />
  public override void receiveGamePadButton(Buttons button)
  {
    switch (button)
    {
      case Buttons.RightTrigger:
        Game1.playSound("shwip");
        this.SetRegion((this.currentPaintRegion + 1 + this.regions.Count) % this.regions.Count);
        break;
      case Buttons.LeftTrigger:
        Game1.playSound("shwip");
        this.SetRegion((this.currentPaintRegion - 1 + this.regions.Count) % this.regions.Count);
        break;
    }
    base.receiveGamePadButton(button);
  }

  /// <inheritdoc />
  public override void update(GameTime time)
  {
    this.activeSlider?.Update(Game1.getMouseX(), Game1.getMouseY());
    base.update(time);
  }

  /// <inheritdoc />
  public override void releaseLeftClick(int x, int y)
  {
    this.activeSlider = (BuildingPaintMenu.BuildingColorSlider) null;
    base.releaseLeftClick(x, y);
  }

  /// <inheritdoc />
  public override void receiveRightClick(int x, int y, bool playSound = true)
  {
    for (int index = 0; index < this.savedColorButtons.Count; ++index)
    {
      if (this.savedColorButtons[index].containsPoint(x, y))
      {
        BuildingPaintMenu.savedColors.RemoveAt(index);
        this.RepositionElements();
        Game1.playSound("coin");
        return;
      }
    }
    base.receiveRightClick(x, y, playSound);
  }

  /// <inheritdoc />
  public override void receiveLeftClick(int x, int y, bool playSound = true)
  {
    if (this.colorSliderPanel.ReceiveLeftClick(x, y, playSound))
      return;
    if (this.defaultColorButton.containsPoint(x, y))
    {
      switch (this.currentPaintRegion)
      {
        case 0:
          this.colorTarget.Color1Default.Value = true;
          break;
        case 1:
          this.colorTarget.Color2Default.Value = true;
          break;
        default:
          this.colorTarget.Color3Default.Value = true;
          break;
      }
      Game1.playSound("coin");
      this.RepositionElements();
    }
    else
    {
      for (int index = 0; index < this.savedColorButtons.Count; ++index)
      {
        if (this.savedColorButtons[index].containsPoint(x, y))
        {
          this.colorSliderPanel.hueSlider.SetValue((int) BuildingPaintMenu.savedColors[index].X);
          this.colorSliderPanel.saturationSlider.SetValue((int) BuildingPaintMenu.savedColors[index].Y);
          this.colorSliderPanel.lightnessSlider.SetValue((int) Utility.Lerp((float) this.colorSliderPanel.lightnessSlider.min, (float) this.colorSliderPanel.lightnessSlider.max, BuildingPaintMenu.savedColors[index].Z));
          Game1.playSound("coin");
          return;
        }
      }
      if (this.copyColorButton.containsPoint(x, y))
      {
        if (this.SaveColor())
        {
          Game1.playSound("coin");
          this.RepositionElements();
        }
        else
          Game1.playSound("cancel");
      }
      else if (this.okButton.containsPoint(x, y))
        this.exitThisMenu(playSound);
      else if (this.appearanceButton.containsPoint(x, y))
      {
        Game1.playSound("smallSelect");
        BuildingSkinMenu menu1 = new BuildingSkinMenu(this.building);
        BuildingSkinMenu buildingSkinMenu = menu1;
        buildingSkinMenu.behaviorBeforeCleanup = buildingSkinMenu.behaviorBeforeCleanup + (Action<IClickableMenu>) (menu =>
        {
          if (this.building.CanBePainted())
          {
            BuildingPaintMenu menu2 = new BuildingPaintMenu(this.building);
            IClickableMenu iclickableMenu1 = Game1.activeClickableMenu;
            IClickableMenu iclickableMenu2 = (IClickableMenu) null;
            while (iclickableMenu1.GetChildMenu() != null)
            {
              iclickableMenu2 = iclickableMenu1;
              iclickableMenu1 = iclickableMenu1.GetChildMenu();
              if (iclickableMenu1 is BuildingPaintMenu)
                break;
            }
            if (iclickableMenu2 == null)
              Game1.activeClickableMenu = (IClickableMenu) menu2;
            else
              iclickableMenu2.SetChildMenu((IClickableMenu) menu2);
            if (!Game1.options.SnappyMenus)
              return;
            menu2.setCurrentlySnappedComponentTo(109);
            menu2.snapCursorToCurrentSnappedComponent();
          }
          else
            this.exitThisMenuNoSound();
        });
        this.SetChildMenu((IClickableMenu) menu1);
      }
      else if (this.previousRegionButton.containsPoint(x, y))
      {
        Game1.playSound("shwip");
        this.SetRegion((this.currentPaintRegion - 1 + this.regions.Count) % this.regions.Count);
      }
      else if (this.nextRegionButton.containsPoint(x, y))
      {
        Game1.playSound("shwip");
        this.SetRegion((this.currentPaintRegion + 1) % this.regions.Count);
      }
      else
        base.receiveLeftClick(x, y, playSound);
    }
  }

  /// <inheritdoc />
  public override void performHoverAction(int x, int y)
  {
    this.hoverText = "";
    this.okButton.tryHover(x, y);
    this.previousRegionButton.tryHover(x, y);
    this.nextRegionButton.tryHover(x, y);
    this.copyColorButton.tryHover(x, y);
    this.defaultColorButton.tryHover(x, y);
    this.appearanceButton.tryHover(x, y);
    if (this.appearanceButton.containsPoint(x, y))
      this.hoverText = this.appearanceButton.name;
    foreach (ClickableTextureComponent savedColorButton in this.savedColorButtons)
      savedColorButton.tryHover(x, y);
    this.colorSliderPanel.PerformHoverAction(x, y);
  }

  public virtual void RepositionElements()
  {
    this.previewPane.X = this.xPositionOnScreen;
    this.previewPane.Y = this.yPositionOnScreen;
    this.previewPane.Width = 512 /*0x0200*/;
    this.previewPane.Height = 576;
    this.colorPane.Width = 448;
    this.colorPane.X = this.xPositionOnScreen + this.width - this.colorPane.Width;
    this.colorPane.Y = this.yPositionOnScreen;
    this.colorPane.Height = 576;
    Rectangle start_rect = this.colorPane;
    start_rect.Inflate(-32, -32);
    ClickableTextureComponent textureComponent1 = new ClickableTextureComponent(new Rectangle(start_rect.Left, start_rect.Top, 64 /*0x40*/, 64 /*0x40*/), Game1.mouseCursors, Game1.getSourceRectForStandardTileSheet(Game1.mouseCursors, 44), 1f);
    textureComponent1.myID = 103;
    textureComponent1.leftNeighborID = -99998;
    textureComponent1.rightNeighborID = -99998;
    textureComponent1.downNeighborID = 105;
    textureComponent1.upNeighborID = -99998;
    textureComponent1.fullyImmutable = true;
    this.previousRegionButton = textureComponent1;
    ClickableTextureComponent textureComponent2 = new ClickableTextureComponent(new Rectangle(start_rect.Right - 64 /*0x40*/, start_rect.Top, 64 /*0x40*/, 64 /*0x40*/), Game1.mouseCursors, Game1.getSourceRectForStandardTileSheet(Game1.mouseCursors, 33), 1f);
    textureComponent2.myID = 102;
    textureComponent2.leftNeighborID = -99998;
    textureComponent2.rightNeighborID = -99998;
    textureComponent2.downNeighborID = 105;
    textureComponent2.upNeighborID = -99998;
    textureComponent2.fullyImmutable = true;
    this.nextRegionButton = textureComponent2;
    start_rect.Y += 64 /*0x40*/;
    start_rect.Height = 0;
    int left = start_rect.Left;
    ClickableTextureComponent textureComponent3 = new ClickableTextureComponent(new Rectangle(left, start_rect.Bottom, 64 /*0x40*/, 64 /*0x40*/), Game1.mouseCursors2, new Rectangle(80 /*0x50*/, 144 /*0x90*/, 16 /*0x10*/, 16 /*0x10*/), 4f);
    textureComponent3.region = 1000;
    textureComponent3.myID = 105;
    textureComponent3.upNeighborID = -99998;
    textureComponent3.downNeighborID = -99998;
    textureComponent3.leftNeighborID = -99998;
    textureComponent3.rightNeighborID = -99998;
    textureComponent3.fullyImmutable = true;
    this.defaultColorButton = textureComponent3;
    int x = left + 80 /*0x50*/;
    this.savedColorButtons.Clear();
    this.buttonColors.Clear();
    for (int index = 0; index < BuildingPaintMenu.savedColors.Count; ++index)
    {
      if (x + 64 /*0x40*/ > start_rect.X + start_rect.Width)
      {
        x = start_rect.X;
        start_rect.Y += 72;
      }
      ClickableTextureComponent textureComponent4 = new ClickableTextureComponent(new Rectangle(x, start_rect.Bottom, 64 /*0x40*/, 64 /*0x40*/), Game1.mouseCursors2, new Rectangle(96 /*0x60*/, 144 /*0x90*/, 16 /*0x10*/, 16 /*0x10*/), 4f);
      textureComponent4.region = 1000;
      textureComponent4.myID = index;
      textureComponent4.upNeighborID = -99998;
      textureComponent4.downNeighborID = -99998;
      textureComponent4.leftNeighborID = -99998;
      textureComponent4.rightNeighborID = -99998;
      textureComponent4.fullyImmutable = true;
      ClickableTextureComponent textureComponent5 = textureComponent4;
      x += 80 /*0x50*/;
      this.savedColorButtons.Add(textureComponent5);
      Vector3 savedColor = BuildingPaintMenu.savedColors[index];
      int r;
      int g;
      int b;
      Utility.HSLtoRGB((double) savedColor.X, (double) savedColor.Y / 100.0, (double) Utility.Lerp(0.25f, 0.5f, savedColor.Z), out r, out g, out b);
      this.buttonColors.Add(new Color((int) (byte) r, (int) (byte) g, (int) (byte) b));
    }
    if (x + 64 /*0x40*/ > start_rect.X + start_rect.Width)
    {
      x = start_rect.X;
      start_rect.Y += 72;
    }
    ClickableTextureComponent textureComponent6 = new ClickableTextureComponent(new Rectangle(x, start_rect.Bottom, 64 /*0x40*/, 64 /*0x40*/), Game1.mouseCursors, new Rectangle(274, 284, 16 /*0x10*/, 16 /*0x10*/), 4f);
    textureComponent6.region = 1000;
    textureComponent6.myID = 104;
    textureComponent6.upNeighborID = -99998;
    textureComponent6.downNeighborID = -99998;
    textureComponent6.leftNeighborID = -99998;
    textureComponent6.rightNeighborID = -99998;
    textureComponent6.fullyImmutable = true;
    this.copyColorButton = textureComponent6;
    start_rect.Y += 80 /*0x50*/;
    start_rect = this.colorSliderPanel.Reposition(start_rect);
    start_rect.Y += 64 /*0x40*/;
    ClickableTextureComponent textureComponent7 = new ClickableTextureComponent(new Rectangle(this.colorPane.Right - 64 /*0x40*/ - 16 /*0x10*/, this.colorPane.Bottom - 64 /*0x40*/ - 16 /*0x10*/, 64 /*0x40*/, 64 /*0x40*/), Game1.mouseCursors, Game1.getSourceRectForStandardTileSheet(Game1.mouseCursors, 46), 1f);
    textureComponent7.myID = 101;
    textureComponent7.upNeighborID = 108;
    textureComponent7.leftNeighborID = 109;
    this.okButton = textureComponent7;
    ClickableTextureComponent textureComponent8 = new ClickableTextureComponent(Game1.content.LoadString("Strings\\UI:Carpenter_ChangeAppearance"), new Rectangle(this.previewPane.Right - 64 /*0x40*/ - 16 /*0x10*/, this.colorPane.Bottom - 64 /*0x40*/ - 16 /*0x10*/, 64 /*0x40*/, 64 /*0x40*/), (string) null, (string) null, Game1.mouseCursors2, new Rectangle(96 /*0x60*/, 208 /*0xD0*/, 16 /*0x10*/, 16 /*0x10*/), 4f);
    textureComponent8.myID = 109;
    textureComponent8.upNeighborID = 108;
    textureComponent8.rightNeighborID = 101;
    textureComponent8.visible = this.building.CanBeReskinned();
    this.appearanceButton = textureComponent8;
    this.populateClickableComponentList();
  }

  public override bool IsAutomaticSnapValid(
    int direction,
    ClickableComponent a,
    ClickableComponent b)
  {
    if (a.region == 1000 && b.region != 1000)
    {
      switch (direction)
      {
        case 1:
        case 3:
          return false;
        case 2:
          if (b.myID != 106)
            return false;
          break;
      }
    }
    return base.IsAutomaticSnapValid(direction, a, b);
  }

  public virtual bool SaveColor()
  {
    if (this.currentPaintRegion == 0 && this.colorTarget.Color1Default.Value || this.currentPaintRegion == 1 && this.colorTarget.Color2Default.Value || this.currentPaintRegion == 2 && this.colorTarget.Color3Default.Value)
      return false;
    Vector3 vector3 = new Vector3((float) this.colorSliderPanel.hueSlider.GetValue(), (float) this.colorSliderPanel.saturationSlider.GetValue(), (float) (this.colorSliderPanel.lightnessSlider.GetValue() - this.colorSliderPanel.lightnessSlider.min) / (float) (this.colorSliderPanel.lightnessSlider.max - this.colorSliderPanel.lightnessSlider.min));
    if (BuildingPaintMenu.savedColors.Count >= 8)
      BuildingPaintMenu.savedColors.RemoveAt(0);
    BuildingPaintMenu.savedColors.Add(vector3);
    return true;
  }

  public virtual void SetRegion(int new_region)
  {
    if (this.regions == null)
      this.LoadRegionData();
    if (new_region < this.regions.Count && new_region >= 0)
    {
      this.currentPaintRegion = new_region;
      BuildingPaintMenu.RegionData region = this.regions[new_region];
      this.colorSliderPanel = new BuildingPaintMenu.ColorSliderPanel(this, new_region, region.Id, region.MinBrightness, region.MaxBrightness);
    }
    this.RepositionElements();
  }

  public virtual void LoadRegionData()
  {
    if (this.regions != null)
      return;
    this.regions = new List<BuildingPaintMenu.RegionData>();
    string paintDataKey = this.building.GetPaintDataKey(this._paintData);
    string str1;
    string str2 = paintDataKey == null || !this._paintData.TryGetValue(paintDataKey, out str1) ? (string) null : str1.Replace("\n", "").Replace("\t", "");
    if (str2 == null)
      return;
    string[] strArray1 = str2.Split('/');
    for (int index = 0; index < strArray1.Length / 2; ++index)
    {
      if (!(strArray1[index].Trim() == ""))
      {
        string id = strArray1[index * 2];
        string[] strArray2 = ArgUtility.SplitBySpace(strArray1[index * 2 + 1]);
        int minBrightness = -100;
        int maxBrightness = 100;
        if (strArray2.Length >= 2)
        {
          try
          {
            minBrightness = int.Parse(strArray2[0]);
            maxBrightness = int.Parse(strArray2[1]);
          }
          catch (Exception ex)
          {
          }
        }
        string displayName = Game1.content.LoadStringReturnNullIfNotFound("Strings/Buildings:Paint_Region_" + id) ?? id;
        this.regions.Add(new BuildingPaintMenu.RegionData(id, displayName, minBrightness, maxBrightness));
      }
    }
  }

  /// <inheritdoc />
  public override void draw(SpriteBatch b)
  {
    if (!Game1.options.showClearBackgrounds)
      b.Draw(Game1.fadeToBlackRect, Game1.graphics.GraphicsDevice.Viewport.Bounds, Color.Black * 0.75f);
    Game1.DrawBox(this.previewPane.X, this.previewPane.Y, this.previewPane.Width, this.previewPane.Height);
    Rectangle previewPane = this.previewPane;
    previewPane.Inflate(0, 0);
    b.End();
    b.Begin(SpriteSortMode.FrontToBack, BlendState.AlphaBlend, SamplerState.PointClamp, rasterizerState: Utility.ScissorEnabled);
    b.GraphicsDevice.ScissorRectangle = previewPane;
    Vector2 vector2 = new Vector2((float) (this.previewPane.X + this.previewPane.Width / 2), (float) (this.previewPane.Y + this.previewPane.Height / 2 - 16 /*0x10*/));
    Rectangle? nullable = this.building.getSourceRectForMenu();
    Rectangle rectangle = nullable ?? this.building.getSourceRect();
    this.building.drawInMenu(b, (int) vector2.X - (int) ((double) this.building.tilesWide.Value / 2.0 * 64.0), (int) vector2.Y - rectangle.Height * 4 / 2);
    b.End();
    b.Begin(blendState: BlendState.AlphaBlend, samplerState: SamplerState.PointClamp);
    Game1.DrawBox(this.colorPane.X, this.colorPane.Y, this.colorPane.Width, this.colorPane.Height);
    BuildingPaintMenu.RegionData region = this.regions[this.currentPaintRegion];
    int heightOfString = SpriteText.getHeightOfString(region.DisplayName);
    SpriteText.drawStringHorizontallyCenteredAt(b, region.DisplayName, this.colorPane.X + this.colorPane.Width / 2, this.nextRegionButton.bounds.Center.Y - heightOfString / 2);
    this.okButton.draw(b);
    this.appearanceButton.draw(b);
    this.colorSliderPanel.Draw(b);
    this.nextRegionButton.draw(b);
    this.previousRegionButton.draw(b);
    this.copyColorButton.draw(b);
    this.defaultColorButton.draw(b);
    for (int index = 0; index < this.savedColorButtons.Count; ++index)
      this.savedColorButtons[index].draw(b, this.buttonColors[index], 1f);
    if (this.GetChildMenu() != null)
      return;
    this.drawMouse(b);
    string hoverText1 = this.hoverText;
    if ((hoverText1 != null ? (hoverText1.Length > 0 ? 1 : 0) : 0) == 0)
      return;
    SpriteBatch b1 = b;
    string hoverText2 = this.hoverText;
    SpriteFont dialogueFont = Game1.dialogueFont;
    nullable = new Rectangle?();
    Rectangle? boxSourceRect = nullable;
    Color? textColor = new Color?();
    Color? textShadowColor = new Color?();
    IClickableMenu.drawHoverText(b1, hoverText2, dialogueFont, boxSourceRect: boxSourceRect, textColor: textColor, textShadowColor: textShadowColor);
  }

  /// <summary>The data model for a paint region.</summary>
  public class RegionData
  {
    /// <summary>The unique region ID within the building's paint regions.</summary>
    public string Id { get; }

    /// <summary>The localized display name.</summary>
    public string DisplayName { get; }

    /// <summary>The minimum brightness allowed.</summary>
    public int MinBrightness { get; }

    /// <summary>The maximum brightness allowed.</summary>
    public int MaxBrightness { get; }

    /// <summary>Construct an instance.</summary>
    /// <param name="id">The unique region ID within the building's paint regions.</param>
    /// <param name="displayName">The localized display name.</param>
    /// <param name="minBrightness">The minimum brightness allowed.</param>
    /// <param name="maxBrightness">The maximum brightness allowed.</param>
    public RegionData(string id, string displayName, int minBrightness, int maxBrightness)
    {
      this.Id = id;
      this.DisplayName = displayName;
      this.MinBrightness = minBrightness;
      this.MaxBrightness = maxBrightness;
    }
  }

  public class ColorSliderPanel
  {
    public BuildingPaintMenu buildingPaintMenu;
    public int regionIndex;
    public string regionId = "Paint Region Name";
    public Rectangle rectangle;
    public Vector2 colorDrawPosition;
    public List<KeyValuePair<string, List<int>>> colors = new List<KeyValuePair<string, List<int>>>();
    public int selectedColor;
    public BuildingPaintMenu.BuildingColorSlider hueSlider;
    public BuildingPaintMenu.BuildingColorSlider saturationSlider;
    public BuildingPaintMenu.BuildingColorSlider lightnessSlider;
    public int minimumBrightness = -100;
    public int maximumBrightness = 100;

    public ColorSliderPanel(
      BuildingPaintMenu menu,
      int region_index,
      string regionId,
      int min_brightness = -100,
      int max_brightness = 100)
    {
      this.regionIndex = region_index;
      this.buildingPaintMenu = menu;
      this.regionId = regionId;
      this.minimumBrightness = min_brightness;
      this.maximumBrightness = max_brightness;
    }

    public virtual int GetHeight() => this.rectangle.Height;

    public virtual Rectangle Reposition(Rectangle start_rect)
    {
      this.buildingPaintMenu.sliderHandles.Clear();
      this.rectangle.X = start_rect.X;
      this.rectangle.Y = start_rect.Y;
      this.rectangle.Width = start_rect.Width;
      this.rectangle.Height = 0;
      this.lightnessSlider = (BuildingPaintMenu.BuildingColorSlider) null;
      this.hueSlider = (BuildingPaintMenu.BuildingColorSlider) null;
      this.saturationSlider = (BuildingPaintMenu.BuildingColorSlider) null;
      this.colorDrawPosition = new Vector2((float) (start_rect.X + start_rect.Width - 64 /*0x40*/), (float) start_rect.Y);
      this.hueSlider = new BuildingPaintMenu.BuildingColorSlider(this.buildingPaintMenu, 106, new Rectangle(this.rectangle.Left, this.rectangle.Bottom, this.rectangle.Width - 100, 12), 0, 360, (Action<int>) (v =>
      {
        switch (this.regionIndex)
        {
          case 0:
            this.buildingPaintMenu.colorTarget.Color1Default.Value = false;
            break;
          case 1:
            this.buildingPaintMenu.colorTarget.Color2Default.Value = false;
            break;
          default:
            this.buildingPaintMenu.colorTarget.Color3Default.Value = false;
            break;
        }
        this.ApplyColors();
      }));
      this.hueSlider.getDrawColor += (Func<float, Color>) (val => this.GetColorForValues(val, 100f));
      switch (this.regionIndex)
      {
        case 0:
          this.hueSlider.SetValue(this.buildingPaintMenu.colorTarget.Color1Hue.Value, true);
          break;
        case 1:
          this.hueSlider.SetValue(this.buildingPaintMenu.colorTarget.Color2Hue.Value, true);
          break;
        default:
          this.hueSlider.SetValue(this.buildingPaintMenu.colorTarget.Color3Hue.Value, true);
          break;
      }
      this.rectangle.Height += 24;
      this.saturationSlider = new BuildingPaintMenu.BuildingColorSlider(this.buildingPaintMenu, 107, new Rectangle(this.rectangle.Left, this.rectangle.Bottom, this.rectangle.Width - 100, 12), 0, 75, (Action<int>) (v =>
      {
        switch (this.regionIndex)
        {
          case 0:
            this.buildingPaintMenu.colorTarget.Color1Default.Value = false;
            break;
          case 1:
            this.buildingPaintMenu.colorTarget.Color2Default.Value = false;
            break;
          default:
            this.buildingPaintMenu.colorTarget.Color3Default.Value = false;
            break;
        }
        this.ApplyColors();
      }));
      this.saturationSlider.getDrawColor += (Func<float, Color>) (val => this.GetColorForValues((float) this.hueSlider.GetValue(), val));
      switch (this.regionIndex)
      {
        case 0:
          this.saturationSlider.SetValue(this.buildingPaintMenu.colorTarget.Color1Saturation.Value, true);
          break;
        case 1:
          this.saturationSlider.SetValue(this.buildingPaintMenu.colorTarget.Color2Saturation.Value, true);
          break;
        default:
          this.saturationSlider.SetValue(this.buildingPaintMenu.colorTarget.Color3Saturation.Value, true);
          break;
      }
      this.rectangle.Height += 24;
      this.lightnessSlider = new BuildingPaintMenu.BuildingColorSlider(this.buildingPaintMenu, 108, new Rectangle(this.rectangle.Left, this.rectangle.Bottom, this.rectangle.Width - 100, 12), this.minimumBrightness, this.maximumBrightness, (Action<int>) (v =>
      {
        switch (this.regionIndex)
        {
          case 0:
            this.buildingPaintMenu.colorTarget.Color1Default.Value = false;
            break;
          case 1:
            this.buildingPaintMenu.colorTarget.Color2Default.Value = false;
            break;
          default:
            this.buildingPaintMenu.colorTarget.Color3Default.Value = false;
            break;
        }
        this.ApplyColors();
      }));
      this.lightnessSlider.getDrawColor += (Func<float, Color>) (val => this.GetColorForValues((float) this.hueSlider.GetValue(), (float) this.saturationSlider.GetValue(), val));
      switch (this.regionIndex)
      {
        case 0:
          this.lightnessSlider.SetValue(this.buildingPaintMenu.colorTarget.Color1Lightness.Value, true);
          break;
        case 1:
          this.lightnessSlider.SetValue(this.buildingPaintMenu.colorTarget.Color2Lightness.Value, true);
          break;
        default:
          this.lightnessSlider.SetValue(this.buildingPaintMenu.colorTarget.Color3Lightness.Value, true);
          break;
      }
      this.rectangle.Height += 24;
      if (this.regionIndex == 0 && this.buildingPaintMenu.colorTarget.Color1Default.Value || this.regionIndex == 1 && this.buildingPaintMenu.colorTarget.Color2Default.Value || this.regionIndex == 2 && this.buildingPaintMenu.colorTarget.Color3Default.Value)
      {
        this.hueSlider.SetValue(this.hueSlider.min, true);
        this.saturationSlider.SetValue(this.saturationSlider.max, true);
        this.lightnessSlider.SetValue((this.lightnessSlider.min + this.lightnessSlider.max) / 2, true);
      }
      this.buildingPaintMenu.sliderHandles.Add((ClickableComponent) this.hueSlider.handle);
      this.buildingPaintMenu.sliderHandles.Add((ClickableComponent) this.saturationSlider.handle);
      this.buildingPaintMenu.sliderHandles.Add((ClickableComponent) this.lightnessSlider.handle);
      this.hueSlider.handle.upNeighborID = 104;
      this.hueSlider.handle.downNeighborID = 107;
      this.saturationSlider.handle.downNeighborID = 108;
      this.saturationSlider.handle.upNeighborID = 106;
      this.lightnessSlider.handle.upNeighborID = 107;
      this.rectangle.Height += 32 /*0x20*/;
      start_rect.Y += this.rectangle.Height;
      return start_rect;
    }

    public virtual void ApplyColors()
    {
      switch (this.regionIndex)
      {
        case 0:
          this.buildingPaintMenu.colorTarget.Color1Hue.Value = this.hueSlider.GetValue();
          this.buildingPaintMenu.colorTarget.Color1Saturation.Value = this.saturationSlider.GetValue();
          this.buildingPaintMenu.colorTarget.Color1Lightness.Value = this.lightnessSlider.GetValue();
          break;
        case 1:
          this.buildingPaintMenu.colorTarget.Color2Hue.Value = this.hueSlider.GetValue();
          this.buildingPaintMenu.colorTarget.Color2Saturation.Value = this.saturationSlider.GetValue();
          this.buildingPaintMenu.colorTarget.Color2Lightness.Value = this.lightnessSlider.GetValue();
          break;
        default:
          this.buildingPaintMenu.colorTarget.Color3Hue.Value = this.hueSlider.GetValue();
          this.buildingPaintMenu.colorTarget.Color3Saturation.Value = this.saturationSlider.GetValue();
          this.buildingPaintMenu.colorTarget.Color3Lightness.Value = this.lightnessSlider.GetValue();
          break;
      }
    }

    public virtual void Draw(SpriteBatch b)
    {
      if ((this.regionIndex != 0 || !this.buildingPaintMenu.colorTarget.Color1Default.Value) && (this.regionIndex != 1 || !this.buildingPaintMenu.colorTarget.Color2Default.Value) && (this.regionIndex != 2 || !this.buildingPaintMenu.colorTarget.Color3Default.Value))
      {
        Color colorForValues = this.GetColorForValues((float) this.hueSlider.GetValue(), (float) this.saturationSlider.GetValue(), (float) this.lightnessSlider.GetValue());
        b.Draw(Game1.staminaRect, new Rectangle((int) this.colorDrawPosition.X - 4, (int) this.colorDrawPosition.Y - 4, 72, 72), new Rectangle?(), Game1.textColor, 0.0f, Vector2.Zero, SpriteEffects.None, 1f);
        b.Draw(Game1.staminaRect, new Rectangle((int) this.colorDrawPosition.X, (int) this.colorDrawPosition.Y, 64 /*0x40*/, 64 /*0x40*/), new Rectangle?(), colorForValues, 0.0f, Vector2.Zero, SpriteEffects.None, 1f);
      }
      this.hueSlider?.Draw(b);
      this.saturationSlider?.Draw(b);
      this.lightnessSlider?.Draw(b);
    }

    public Color GetColorForValues(float hue_slider, float saturation_slider)
    {
      int r;
      int g;
      int b;
      Utility.HSLtoRGB((double) hue_slider, (double) saturation_slider / 100.0, 0.5, out r, out g, out b);
      return new Color((int) (byte) r, g, b);
    }

    public Color GetColorForValues(
      float hue_slider,
      float saturation_slider,
      float lightness_slider)
    {
      int r;
      int g;
      int b;
      Utility.HSLtoRGB((double) hue_slider, (double) saturation_slider / 100.0, (double) Utility.Lerp(0.25f, 0.5f, (lightness_slider - (float) this.lightnessSlider.min) / (float) (this.lightnessSlider.max - this.lightnessSlider.min)), out r, out g, out b);
      return new Color((int) (byte) r, g, b);
    }

    public virtual bool ApplyMovementKey(int direction)
    {
      if (direction == 3 || direction == 1)
      {
        if (this.saturationSlider.handle == this.buildingPaintMenu.currentlySnappedComponent)
        {
          this.saturationSlider.ApplyMovementKey(direction);
          return true;
        }
        if (this.hueSlider.handle == this.buildingPaintMenu.currentlySnappedComponent)
        {
          this.hueSlider.ApplyMovementKey(direction);
          return true;
        }
        if (this.lightnessSlider.handle == this.buildingPaintMenu.currentlySnappedComponent)
        {
          this.lightnessSlider.ApplyMovementKey(direction);
          return true;
        }
      }
      return false;
    }

    public virtual void PerformHoverAction(int x, int y)
    {
    }

    public virtual bool ReceiveLeftClick(int x, int y, bool play_sound = true)
    {
      this.hueSlider?.ReceiveLeftClick(x, y);
      this.saturationSlider?.ReceiveLeftClick(x, y);
      this.lightnessSlider?.ReceiveLeftClick(x, y);
      return false;
    }
  }

  public class BuildingColorSlider
  {
    public ClickableTextureComponent handle;
    public BuildingPaintMenu buildingPaintMenu;
    public Rectangle bounds;
    protected float _sliderPosition;
    public int min;
    public int max;
    public Action<int> onValueSet;
    public Func<float, Color> getDrawColor;
    protected int _displayedValue;

    public BuildingColorSlider(
      BuildingPaintMenu bpm,
      int handle_id,
      Rectangle bounds,
      int min,
      int max,
      Action<int> on_value_set = null)
    {
      this.handle = new ClickableTextureComponent(new Rectangle(0, 0, 4, 5), Game1.mouseCursors, new Rectangle(72, 256 /*0x0100*/, 16 /*0x10*/, 20), 1f);
      this.handle.myID = handle_id;
      this.handle.upNeighborID = -99998;
      this.handle.upNeighborImmutable = true;
      this.handle.downNeighborID = -99998;
      this.handle.downNeighborImmutable = true;
      this.handle.leftNeighborImmutable = true;
      this.handle.rightNeighborImmutable = true;
      this.buildingPaintMenu = bpm;
      this.bounds = bounds;
      this.min = min;
      this.max = max;
      this.onValueSet = on_value_set;
    }

    public virtual void ApplyMovementKey(int direction)
    {
      int num = Math.Max((this.max - this.min) / 50, 1);
      if (direction == 3)
        this.SetValue(this._displayedValue - num);
      else
        this.SetValue(this._displayedValue + num);
      if (this.buildingPaintMenu.currentlySnappedComponent != this.handle || !Game1.options.SnappyMenus)
        return;
      this.buildingPaintMenu.snapCursorToCurrentSnappedComponent();
    }

    public virtual void ReceiveLeftClick(int x, int y)
    {
      if (!this.bounds.Contains(x, y))
        return;
      this.buildingPaintMenu.activeSlider = this;
      this.SetValueFromPosition(x, y);
    }

    public virtual void SetValueFromPosition(int x, int y)
    {
      if (this.bounds.Width == 0 || this.min == this.max)
        return;
      float num1 = (float) (x - this.bounds.Left) / (float) this.bounds.Width;
      if ((double) num1 < 0.0)
        num1 = 0.0f;
      if ((double) num1 > 1.0)
        num1 = 1f;
      int num2 = this.max - this.min;
      float num3 = num1 / (float) num2 * (float) num2;
      if ((double) this._sliderPosition == (double) num3)
        return;
      this._sliderPosition = num3;
      this.SetValue(this.min + (int) ((double) this._sliderPosition * (double) num2));
    }

    public void SetValue(int value, bool skip_value_set = false)
    {
      if (value > this.max)
        value = this.max;
      if (value < this.min)
        value = this.min;
      this._sliderPosition = (float) (value - this.min) / (float) (this.max - this.min);
      this.handle.bounds.X = (int) Utility.Lerp((float) this.bounds.Left, (float) this.bounds.Right, this._sliderPosition) - this.handle.bounds.Width / 2 * 4;
      this.handle.bounds.Y = this.bounds.Top - 4;
      if (this._displayedValue == value)
        return;
      this._displayedValue = value;
      if (skip_value_set)
        return;
      Action<int> onValueSet = this.onValueSet;
      if (onValueSet == null)
        return;
      onValueSet(value);
    }

    public int GetValue() => this._displayedValue;

    public virtual void Draw(SpriteBatch b)
    {
      int num = 20;
      for (int index = 0; index < num; ++index)
      {
        Rectangle destinationRectangle = new Rectangle((int) ((double) this.bounds.X + (double) this.bounds.Width / (double) num * (double) index), this.bounds.Y, (int) Math.Ceiling((double) this.bounds.Width / (double) num), this.bounds.Height);
        Color color = Color.Black;
        if (this.getDrawColor != null)
          color = this.getDrawColor(Utility.Lerp((float) this.min, (float) this.max, (float) index / (float) num));
        b.Draw(Game1.staminaRect, destinationRectangle, color);
      }
      this.handle.draw(b);
    }

    public virtual void Update(int x, int y) => this.SetValueFromPosition(x, y);
  }
}
