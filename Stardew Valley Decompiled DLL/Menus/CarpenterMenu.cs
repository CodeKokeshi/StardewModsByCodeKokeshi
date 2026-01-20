// Decompiled with JetBrains decompiler
// Type: StardewValley.Menus.CarpenterMenu
// Assembly: Stardew Valley, Version=1.6.15.24356, Culture=neutral, PublicKeyToken=null
// MVID: AA1F513A-94F0-4EF6-A35F-2D5B4A3721BF
// Assembly location: D:\SteamLibrary\steamapps\common\Stardew Valley\Stardew Valley.dll
// XML documentation location: D:\SteamLibrary\steamapps\common\Stardew Valley\Stardew Valley.xml

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Netcode;
using StardewValley.BellsAndWhistles;
using StardewValley.Buildings;
using StardewValley.Extensions;
using StardewValley.GameData.Buildings;
using StardewValley.Locations;
using StardewValley.Objects;
using StardewValley.TerrainFeatures;
using StardewValley.TokenizableStrings;
using System;
using System.Collections.Generic;
using System.Linq;
using xTile.Dimensions;
using xTile.Layers;

#nullable disable
namespace StardewValley.Menus;

public class CarpenterMenu : IClickableMenu
{
  public const int region_backButton = 101;
  public const int region_forwardButton = 102;
  public const int region_upgradeIcon = 103;
  public const int region_demolishButton = 104;
  public const int region_moveBuitton = 105;
  public const int region_okButton = 106;
  public const int region_cancelButton = 107;
  public const int region_paintButton = 108;
  public const int region_appearanceButton = 109;
  /// <summary>The backing field for <see cref="P:StardewValley.Menus.CarpenterMenu.readOnly" />.</summary>
  private bool _readOnly;
  public int maxWidthOfBuildingViewer = 448;
  public int maxHeightOfBuildingViewer = 512 /*0x0200*/;
  public int maxWidthOfDescription = 416;
  /// <summary>The name of the NPC whose building menu is being shown (the vanilla values are <see cref="F:StardewValley.Game1.builder_robin" /> and <see cref="F:StardewValley.Game1.builder_wizard" />). This affects which buildings are available to build based on the <see cref="F:StardewValley.GameData.Buildings.BuildingData.Builder" /> value.</summary>
  public readonly string Builder;
  /// <summary>The name of the location to return to after exiting the farm view.</summary>
  public readonly string BuilderLocationName;
  /// <summary>The viewport position to return to after exiting the farm view.</summary>
  public readonly Location BuilderViewport;
  /// <summary>The location in which to construct or manage buildings.</summary>
  public GameLocation TargetLocation;
  /// <summary>The tile to center on when switching to the <see cref="F:StardewValley.Menus.CarpenterMenu.TargetLocation" />, or <c>null</c> to apply the default behavior.</summary>
  public Vector2? TargetViewportCenterOnTile;
  /// <summary>The blueprints available in the menu.</summary>
  public readonly List<CarpenterMenu.BlueprintEntry> Blueprints = new List<CarpenterMenu.BlueprintEntry>();
  /// <summary>The current blueprint shown in the menu.</summary>
  public CarpenterMenu.BlueprintEntry Blueprint;
  public ClickableTextureComponent okButton;
  public ClickableTextureComponent cancelButton;
  public ClickableTextureComponent backButton;
  public ClickableTextureComponent forwardButton;
  public ClickableTextureComponent upgradeIcon;
  public ClickableTextureComponent demolishButton;
  public ClickableTextureComponent moveButton;
  public ClickableTextureComponent paintButton;
  public ClickableTextureComponent appearanceButton;
  public Building currentBuilding;
  public Building buildingToMove;
  /// <summary>The materials needed to build the <see cref="F:StardewValley.Menus.CarpenterMenu.currentBuilding" />, if any. The stack size for each item is the number required.</summary>
  public readonly List<Item> ingredients = new List<Item>();
  /// <summary>Whether the menu is currently showing the target location (regardless of whether it's the farm), so the player can choose a building or position.</summary>
  public bool onFarm;
  /// <summary>The action which the player selected to perform.</summary>
  public CarpenterMenu.CarpentryAction Action;
  public bool drawBG = true;
  public bool freeze;
  private string hoverText = "";

  public bool readOnly
  {
    get => this._readOnly;
    set
    {
      if (value == this._readOnly)
        return;
      this._readOnly = value;
      this.resetBounds();
    }
  }

  /// <summary>Construct an instance.</summary>
  /// <param name="builder">The name of the NPC whose building menu is being shown (the vanilla values are <see cref="F:StardewValley.Game1.builder_robin" /> and <see cref="F:StardewValley.Game1.builder_wizard" />). This affects which buildings are available to build based on the <see cref="F:StardewValley.GameData.Buildings.BuildingData.Builder" /> value.</param>
  /// <param name="targetLocation">The location in which to construct the building, or <c>null</c> for the farm.</param>
  public CarpenterMenu(string builder, GameLocation targetLocation = null)
  {
    this.Builder = builder;
    this.BuilderLocationName = Game1.currentLocation.NameOrUniqueName;
    this.BuilderViewport = Game1.viewport.Location;
    this.TargetLocation = targetLocation ?? (GameLocation) Game1.getFarm();
    Game1.player.forceCanMove();
    this.resetBounds();
    int num = 0;
    foreach (KeyValuePair<string, BuildingData> keyValuePair in (IEnumerable<KeyValuePair<string, BuildingData>>) Game1.buildingData)
    {
      if (!(keyValuePair.Value.Builder != builder) && GameStateQuery.CheckConditions(keyValuePair.Value.BuildCondition, targetLocation) && (keyValuePair.Value.BuildingToUpgrade == null || this.TargetLocation.getNumberBuildingsConstructed(keyValuePair.Value.BuildingToUpgrade) != 0) && this.IsValidBuildingForLocation(keyValuePair.Key, keyValuePair.Value, this.TargetLocation))
      {
        this.Blueprints.Add(new CarpenterMenu.BlueprintEntry(num++, keyValuePair.Key, keyValuePair.Value, (string) null));
        if (keyValuePair.Value.Skins != null)
        {
          foreach (BuildingSkin skin in keyValuePair.Value.Skins)
          {
            if (skin.ShowAsSeparateConstructionEntry && GameStateQuery.CheckConditions(skin.Condition, this.TargetLocation))
              this.Blueprints.Add(new CarpenterMenu.BlueprintEntry(num++, keyValuePair.Key, keyValuePair.Value, skin.Id));
          }
        }
      }
    }
    this.SetNewActiveBlueprint(0);
    if (!Game1.options.SnappyMenus)
      return;
    this.populateClickableComponentList();
    this.snapToDefaultClickableComponent();
  }

  public override bool shouldClampGamePadCursor() => this.onFarm;

  public override void snapToDefaultClickableComponent()
  {
    this.currentlySnappedComponent = this.getComponentWithID(107);
    this.snapCursorToCurrentSnappedComponent();
  }

  private void resetBounds()
  {
    bool flag1 = false;
    bool flag2 = false;
    foreach (Building building in this.TargetLocation.buildings)
    {
      if (building.hasCarpenterPermissions())
        flag1 = true;
      if ((building.CanBePainted() || building.CanBeReskinned(true)) && this.HasPermissionsToPaint(building))
        flag2 = true;
    }
    this.xPositionOnScreen = Game1.uiViewport.Width / 2 - this.maxWidthOfBuildingViewer - IClickableMenu.spaceToClearSideBorder;
    this.yPositionOnScreen = Game1.uiViewport.Height / 2 - this.maxHeightOfBuildingViewer / 2 - IClickableMenu.spaceToClearTopBorder + 32 /*0x20*/;
    this.width = this.maxWidthOfBuildingViewer + this.maxWidthOfDescription + IClickableMenu.spaceToClearSideBorder * 2 + 64 /*0x40*/;
    this.height = this.maxHeightOfBuildingViewer + IClickableMenu.spaceToClearTopBorder;
    bool flag3 = this.readOnly;
    this.initialize(this.xPositionOnScreen, this.yPositionOnScreen, this.width, this.height, true);
    ClickableTextureComponent textureComponent1 = new ClickableTextureComponent("OK", new Microsoft.Xna.Framework.Rectangle(this.xPositionOnScreen + this.width - IClickableMenu.borderWidth - IClickableMenu.spaceToClearSideBorder - 192 /*0xC0*/ - 12, this.yPositionOnScreen + this.maxHeightOfBuildingViewer + 64 /*0x40*/, 64 /*0x40*/, 64 /*0x40*/), (string) null, (string) null, Game1.mouseCursors, new Microsoft.Xna.Framework.Rectangle(366, 373, 16 /*0x10*/, 16 /*0x10*/), 4f);
    textureComponent1.myID = 106;
    textureComponent1.rightNeighborID = 104;
    textureComponent1.leftNeighborID = 105;
    textureComponent1.upNeighborID = 109;
    textureComponent1.visible = !flag3;
    this.okButton = textureComponent1;
    ClickableTextureComponent textureComponent2 = new ClickableTextureComponent("OK", new Microsoft.Xna.Framework.Rectangle(this.xPositionOnScreen + this.width - IClickableMenu.borderWidth - IClickableMenu.spaceToClearSideBorder - 64 /*0x40*/, this.yPositionOnScreen + this.maxHeightOfBuildingViewer + 64 /*0x40*/, 64 /*0x40*/, 64 /*0x40*/), (string) null, (string) null, Game1.mouseCursors, Game1.getSourceRectForStandardTileSheet(Game1.mouseCursors, 47), 1f);
    textureComponent2.myID = 107;
    textureComponent2.leftNeighborID = flag3 ? 102 : 104;
    textureComponent2.upNeighborID = 109;
    this.cancelButton = textureComponent2;
    ClickableTextureComponent textureComponent3 = new ClickableTextureComponent(new Microsoft.Xna.Framework.Rectangle(this.xPositionOnScreen + 64 /*0x40*/, this.yPositionOnScreen + this.maxHeightOfBuildingViewer + 64 /*0x40*/, 48 /*0x30*/, 44), Game1.mouseCursors, new Microsoft.Xna.Framework.Rectangle(352, 495, 12, 11), 4f);
    textureComponent3.myID = 101;
    textureComponent3.rightNeighborID = 102;
    textureComponent3.upNeighborID = 109;
    this.backButton = textureComponent3;
    ClickableTextureComponent textureComponent4 = new ClickableTextureComponent(new Microsoft.Xna.Framework.Rectangle(this.xPositionOnScreen + this.maxWidthOfBuildingViewer - 256 /*0x0100*/ + 16 /*0x10*/, this.yPositionOnScreen + this.maxHeightOfBuildingViewer + 64 /*0x40*/, 48 /*0x30*/, 44), Game1.mouseCursors, new Microsoft.Xna.Framework.Rectangle(365, 495, 12, 11), 4f);
    textureComponent4.myID = 102;
    textureComponent4.leftNeighborID = 101;
    textureComponent4.rightNeighborID = -99998;
    textureComponent4.upNeighborID = 109;
    this.forwardButton = textureComponent4;
    ClickableTextureComponent textureComponent5 = new ClickableTextureComponent(Game1.content.LoadString("Strings\\UI:Carpenter_Demolish"), new Microsoft.Xna.Framework.Rectangle(this.xPositionOnScreen + this.width - IClickableMenu.borderWidth - IClickableMenu.spaceToClearSideBorder - 128 /*0x80*/ - 8, this.yPositionOnScreen + this.maxHeightOfBuildingViewer + 64 /*0x40*/ - 4, 64 /*0x40*/, 64 /*0x40*/), (string) null, (string) null, Game1.mouseCursors, new Microsoft.Xna.Framework.Rectangle(348, 372, 17, 17), 4f);
    textureComponent5.myID = 104;
    textureComponent5.rightNeighborID = 107;
    textureComponent5.leftNeighborID = 106;
    textureComponent5.upNeighborID = 109;
    textureComponent5.visible = !flag3 && Game1.IsMasterGame;
    this.demolishButton = textureComponent5;
    ClickableTextureComponent textureComponent6 = new ClickableTextureComponent(new Microsoft.Xna.Framework.Rectangle(this.xPositionOnScreen + this.maxWidthOfBuildingViewer - 128 /*0x80*/ + 32 /*0x20*/, this.yPositionOnScreen + 8, 36, 52), Game1.mouseCursors, new Microsoft.Xna.Framework.Rectangle(402, 328, 9, 13), 4f);
    textureComponent6.myID = 103;
    textureComponent6.rightNeighborID = 104;
    textureComponent6.leftNeighborID = 105;
    textureComponent6.upNeighborID = 109;
    textureComponent6.visible = !flag3;
    this.upgradeIcon = textureComponent6;
    ClickableTextureComponent textureComponent7 = new ClickableTextureComponent(Game1.content.LoadString("Strings\\UI:Carpenter_MoveBuildings"), new Microsoft.Xna.Framework.Rectangle(this.xPositionOnScreen + this.width - IClickableMenu.borderWidth - IClickableMenu.spaceToClearSideBorder - 256 /*0x0100*/ - 20, this.yPositionOnScreen + this.maxHeightOfBuildingViewer + 64 /*0x40*/, 64 /*0x40*/, 64 /*0x40*/), (string) null, (string) null, Game1.mouseCursors, new Microsoft.Xna.Framework.Rectangle(257, 284, 16 /*0x10*/, 16 /*0x10*/), 4f);
    textureComponent7.myID = 105;
    textureComponent7.rightNeighborID = 106;
    textureComponent7.leftNeighborID = -99998;
    textureComponent7.upNeighborID = 109;
    textureComponent7.visible = !flag3 && (Game1.IsMasterGame || Game1.player.team.farmhandsCanMoveBuildings.Value == FarmerTeam.RemoteBuildingPermissions.On || Game1.player.team.farmhandsCanMoveBuildings.Value == FarmerTeam.RemoteBuildingPermissions.OwnedBuildings & flag1);
    this.moveButton = textureComponent7;
    ClickableTextureComponent textureComponent8 = new ClickableTextureComponent(Game1.content.LoadString("Strings\\UI:Carpenter_PaintBuildings"), new Microsoft.Xna.Framework.Rectangle(this.xPositionOnScreen + this.width - IClickableMenu.borderWidth - IClickableMenu.spaceToClearSideBorder - 320 - 20, this.yPositionOnScreen + this.maxHeightOfBuildingViewer + 64 /*0x40*/, 64 /*0x40*/, 64 /*0x40*/), (string) null, (string) null, Game1.mouseCursors2, new Microsoft.Xna.Framework.Rectangle(80 /*0x50*/, 208 /*0xD0*/, 16 /*0x10*/, 16 /*0x10*/), 4f);
    textureComponent8.myID = 105;
    textureComponent8.rightNeighborID = -99998;
    textureComponent8.leftNeighborID = -99998;
    textureComponent8.upNeighborID = 109;
    textureComponent8.visible = !flag3 & flag2;
    this.paintButton = textureComponent8;
    ClickableTextureComponent textureComponent9 = new ClickableTextureComponent(Game1.content.LoadString("Strings\\UI:Carpenter_ChangeAppearance"), new Microsoft.Xna.Framework.Rectangle(this.xPositionOnScreen + this.maxWidthOfBuildingViewer - 128 /*0x80*/ + 16 /*0x10*/, this.yPositionOnScreen + this.maxHeightOfBuildingViewer - 64 /*0x40*/ + 32 /*0x20*/, 64 /*0x40*/, 64 /*0x40*/), (string) null, (string) null, Game1.mouseCursors2, new Microsoft.Xna.Framework.Rectangle(96 /*0x60*/, 208 /*0xD0*/, 16 /*0x10*/, 16 /*0x10*/), 4f);
    textureComponent9.myID = 109;
    textureComponent9.downNeighborID = -99998;
    this.appearanceButton = textureComponent9;
    if (!this.demolishButton.visible)
    {
      this.upgradeIcon.rightNeighborID = this.demolishButton.rightNeighborID;
      this.okButton.rightNeighborID = this.demolishButton.rightNeighborID;
      this.cancelButton.leftNeighborID = this.demolishButton.leftNeighborID;
    }
    if (!this.moveButton.visible)
    {
      this.upgradeIcon.leftNeighborID = this.moveButton.leftNeighborID;
      this.forwardButton.rightNeighborID = -99998;
      this.okButton.leftNeighborID = this.moveButton.leftNeighborID;
    }
    this.UpdateAppearanceButtonVisibility();
  }

  public void SetNewActiveBlueprint(int index)
  {
    index %= this.Blueprints.Count;
    if (index < 0)
      index = this.Blueprints.Count + index;
    this.SetNewActiveBlueprint(this.Blueprints[index]);
  }

  public void SetNewActiveBlueprint(CarpenterMenu.BlueprintEntry blueprint)
  {
    this.Blueprint = blueprint;
    this.currentBuilding = Building.CreateInstanceFromId(blueprint.Id, Vector2.Zero);
    this.currentBuilding.skinId.Value = blueprint.Skin?.Id;
    this.ingredients.Clear();
    if (blueprint.BuildMaterials != null)
    {
      foreach (BuildingMaterial buildMaterial in blueprint.BuildMaterials)
        this.ingredients.Add(ItemRegistry.Create(buildMaterial.ItemId, buildMaterial.Amount));
    }
    this.UpdateAppearanceButtonVisibility();
    if (!Game1.options.SnappyMenus || this.currentlySnappedComponent == null || this.currentlySnappedComponent != this.appearanceButton || this.appearanceButton.visible)
      return;
    this.setCurrentlySnappedComponentTo(102);
    this.snapToDefaultClickableComponent();
  }

  public virtual void UpdateAppearanceButtonVisibility()
  {
    if (this.appearanceButton == null || this.currentBuilding == null)
      return;
    this.appearanceButton.visible = this.currentBuilding.CanBeReskinned(true);
  }

  /// <inheritdoc />
  public override void performHoverAction(int x, int y)
  {
    this.cancelButton.tryHover(x, y);
    base.performHoverAction(x, y);
    if (!this.onFarm)
    {
      this.backButton.tryHover(x, y, 1f);
      this.forwardButton.tryHover(x, y, 1f);
      this.okButton.tryHover(x, y);
      this.demolishButton.tryHover(x, y);
      this.moveButton.tryHover(x, y);
      this.paintButton.tryHover(x, y);
      this.appearanceButton.tryHover(x, y);
      if (this.Blueprint.IsUpgrade && this.upgradeIcon.containsPoint(x, y))
        this.hoverText = Game1.content.LoadString("Strings\\UI:Carpenter_Upgrade", (object) this.Blueprint.GetDisplayNameForBuildingToUpgrade());
      else if (this.demolishButton.containsPoint(x, y) && this.CanDemolishThis())
        this.hoverText = Game1.content.LoadString("Strings\\UI:Carpenter_Demolish");
      else if (this.moveButton.containsPoint(x, y))
        this.hoverText = Game1.content.LoadString("Strings\\UI:Carpenter_MoveBuildings");
      else if (this.okButton.containsPoint(x, y) && this.CanBuildCurrentBlueprint())
        this.hoverText = Game1.content.LoadString("Strings\\UI:Carpenter_Build");
      else if (this.paintButton.containsPoint(x, y))
        this.hoverText = this.paintButton.name;
      else if (this.appearanceButton.containsPoint(x, y))
        this.hoverText = this.appearanceButton.name;
      else
        this.hoverText = "";
    }
    else
    {
      if (this.Action == CarpenterMenu.CarpentryAction.None || this.freeze)
        return;
      foreach (Building building in this.TargetLocation.buildings)
        building.color = Color.White;
      Vector2 tile = new Vector2((float) ((Game1.viewport.X + Game1.getOldMouseX(false)) / 64 /*0x40*/), (float) ((Game1.viewport.Y + Game1.getOldMouseY(false)) / 64 /*0x40*/));
      Building building1 = this.TargetLocation.getBuildingAt(tile) ?? this.TargetLocation.getBuildingAt(new Vector2(tile.X, tile.Y + 1f)) ?? this.TargetLocation.getBuildingAt(new Vector2(tile.X, tile.Y + 2f)) ?? this.TargetLocation.getBuildingAt(new Vector2(tile.X, tile.Y + 3f));
      BuildingData data = building1?.GetData();
      if (data != null)
      {
        int num = (data.SourceRect.IsEmpty ? building1.texture.Value.Height : building1.GetData().SourceRect.Height) * 4 / 64 /*0x40*/ - building1.tilesHigh.Value;
        if ((double) (building1.tileY.Value - num) > (double) tile.Y)
          building1 = (Building) null;
      }
      switch (this.Action)
      {
        case CarpenterMenu.CarpentryAction.Demolish:
          if (building1 == null || !this.hasPermissionsToDemolish(building1) || !this.CanDemolishThis(building1))
            break;
          building1.color = Color.Red * 0.8f;
          break;
        case CarpenterMenu.CarpentryAction.Move:
          if (building1 == null || !this.hasPermissionsToMove(building1))
            break;
          building1.color = Color.Lime * 0.8f;
          break;
        case CarpenterMenu.CarpentryAction.Paint:
          if (building1 == null || !building1.CanBePainted() && !building1.CanBeReskinned(true) || !this.HasPermissionsToPaint(building1))
            break;
          building1.color = Color.Lime * 0.8f;
          break;
        case CarpenterMenu.CarpentryAction.Upgrade:
          if (building1 == null)
            break;
          building1.color = building1.buildingType.Value == this.Blueprint.UpgradeFrom ? Color.Lime * 0.8f : Color.Red * 0.8f;
          break;
      }
    }
  }

  public bool hasPermissionsToDemolish(Building b) => Game1.IsMasterGame && this.CanDemolishThis(b);

  public bool HasPermissionsToPaint(Building b)
  {
    return !b.isCabin && !b.HasIndoorsName("Farmhouse") || !(b.GetIndoors() is FarmHouse indoors) || indoors.IsOwnedByCurrentPlayer || indoors.OwnerId.ToString() == Game1.player.spouse;
  }

  public bool hasPermissionsToMove(Building b)
  {
    if (!Game1.getFarm().greenhouseUnlocked.Value && b is GreenhouseBuilding)
      return false;
    if (Game1.IsMasterGame)
      return true;
    switch (Game1.player.team.farmhandsCanMoveBuildings.Value)
    {
      case FarmerTeam.RemoteBuildingPermissions.OwnedBuildings:
        if (b.hasCarpenterPermissions())
          return true;
        break;
      case FarmerTeam.RemoteBuildingPermissions.On:
        return true;
    }
    return false;
  }

  /// <inheritdoc />
  public override void receiveGamePadButton(Buttons button)
  {
    base.receiveGamePadButton(button);
    if (this.onFarm)
      return;
    if (button != Buttons.RightTrigger)
    {
      if (button != Buttons.LeftTrigger)
        return;
      this.SetNewActiveBlueprint(this.Blueprint.Index - 1);
      Game1.playSound("shwip");
    }
    else
    {
      this.SetNewActiveBlueprint(this.Blueprint.Index + 1);
      Game1.playSound("shwip");
    }
  }

  public override void gamePadButtonHeld(Buttons b)
  {
    base.gamePadButtonHeld(b);
    if (!this.onFarm || b != Buttons.DPadDown && b != Buttons.DPadRight && b != Buttons.DPadLeft && b != Buttons.DPadUp)
      return;
    GamePadState gamePadState = Game1.input.GetGamePadState();
    MouseState mouseState = Game1.input.GetMouseState();
    int num1 = 12 + (gamePadState.IsButtonDown(Buttons.RightTrigger) || gamePadState.IsButtonDown(Buttons.RightShoulder) ? 8 : 0);
    int num2;
    switch (b)
    {
      case Buttons.DPadLeft:
        num2 = -num1;
        break;
      case Buttons.DPadRight:
        num2 = num1;
        break;
      default:
        num2 = 0;
        break;
    }
    int num3 = num2;
    int num4;
    switch (b)
    {
      case Buttons.DPadUp:
        num4 = -num1;
        break;
      case Buttons.DPadDown:
        num4 = num1;
        break;
      default:
        num4 = 0;
        break;
    }
    int num5 = num4;
    Game1.setMousePositionRaw(mouseState.X + num3, mouseState.Y + num5);
  }

  /// <inheritdoc />
  public override void receiveKeyPress(Keys key)
  {
    if (this.freeze)
      return;
    if (!this.onFarm)
      base.receiveKeyPress(key);
    if (Game1.IsFading() || !this.onFarm)
      return;
    if (Game1.options.doesInputListContain(Game1.options.menuButton, key) && this.readyToClose() && Game1.locationRequest == null)
    {
      this.returnToCarpentryMenu();
    }
    else
    {
      if (Game1.options.SnappyMenus)
        return;
      if (Game1.options.doesInputListContain(Game1.options.moveDownButton, key))
        Game1.panScreen(0, 4);
      else if (Game1.options.doesInputListContain(Game1.options.moveRightButton, key))
        Game1.panScreen(4, 0);
      else if (Game1.options.doesInputListContain(Game1.options.moveUpButton, key))
      {
        Game1.panScreen(0, -4);
      }
      else
      {
        if (!Game1.options.doesInputListContain(Game1.options.moveLeftButton, key))
          return;
        Game1.panScreen(-4, 0);
      }
    }
  }

  /// <inheritdoc />
  public override void update(GameTime time)
  {
    base.update(time);
    if (!this.onFarm || Game1.IsFading())
      return;
    int num1 = Game1.getOldMouseX(false) + Game1.viewport.X;
    int num2 = Game1.getOldMouseY(false) + Game1.viewport.Y;
    if (num1 - Game1.viewport.X < 64 /*0x40*/)
      Game1.panScreen(-8, 0);
    else if (num1 - (Game1.viewport.X + Game1.viewport.Width) >= (int) sbyte.MinValue)
      Game1.panScreen(8, 0);
    if (num2 - Game1.viewport.Y < 64 /*0x40*/)
      Game1.panScreen(0, -8);
    else if (num2 - (Game1.viewport.Y + Game1.viewport.Height) >= -64)
      Game1.panScreen(0, 8);
    foreach (Keys pressedKey in Game1.oldKBState.GetPressedKeys())
      this.receiveKeyPress(pressedKey);
    if (Game1.IsMultiplayer)
      return;
    GameLocation targetLocation = this.TargetLocation;
    foreach (Character character in targetLocation.animals.Values)
      character.MovePosition(Game1.currentGameTime, Game1.viewport, targetLocation);
  }

  protected bool VerifyTileAccessibility(int tileX, int tileY, Vector2 buildingPosition)
  {
    if (!this.TargetLocation.isTilePassable(new Location(tileX, tileY), Game1.viewport) || !this.buildingToMove.isTilePassable(new Vector2((float) (this.buildingToMove.tileX.Value + (tileX - (int) buildingPosition.X)), (float) (this.buildingToMove.tileY.Value + (tileY - (int) buildingPosition.Y)))))
      return false;
    Building buildingAt = this.TargetLocation.getBuildingAt(new Vector2((float) tileX, (float) tileY));
    if (buildingAt != null && !buildingAt.isMoving && !buildingAt.isTilePassable(new Vector2((float) tileX, (float) tileY)))
      return false;
    Microsoft.Xna.Framework.Rectangle rectangle = new Microsoft.Xna.Framework.Rectangle(tileX * 64 /*0x40*/, tileY * 64 /*0x40*/, 64 /*0x40*/, 64 /*0x40*/);
    rectangle.Inflate(-1, -1);
    foreach (TerrainFeature resourceClump in this.TargetLocation.resourceClumps)
    {
      if (resourceClump.getBoundingBox().Intersects(rectangle))
        return false;
    }
    foreach (TerrainFeature largeTerrainFeature in this.TargetLocation.largeTerrainFeatures)
    {
      if (largeTerrainFeature.getBoundingBox().Intersects(rectangle))
        return false;
    }
    return true;
  }

  public virtual bool ConfirmBuildingAccessibility(Vector2 buildingPosition)
  {
    if (this.buildingToMove == null)
      return false;
    if (this.buildingToMove.buildingType.Value != "Farmhouse")
      return true;
    Point point1 = this.buildingToMove.humanDoor.Value;
    point1.X += (int) buildingPosition.X;
    point1.Y += (int) buildingPosition.Y;
    ++point1.Y;
    HashSet<Point> pointSet1 = new HashSet<Point>();
    Stack<Point> pointStack = new Stack<Point>();
    pointStack.Push(point1);
    pointSet1.Add(point1);
    HashSet<Point> pointSet2 = new HashSet<Point>();
    foreach (Warp warp in (NetList<Warp, NetRef<Warp>>) this.TargetLocation.warps)
    {
      if (!(warp.TargetName == "FarmCave"))
        pointSet2.Add(new Point(warp.X, warp.Y));
    }
    bool flag = false;
    while (pointStack.Count > 0)
    {
      Point point2 = pointStack.Pop();
      if (pointSet2.Contains(point2))
      {
        flag = true;
        break;
      }
      if (this.TargetLocation.isTileOnMap(point2.X, point2.Y) && this.VerifyTileAccessibility(point2.X, point2.Y, buildingPosition))
      {
        Point point3 = point2;
        ++point3.X;
        if (pointSet1.Add(point3))
          pointStack.Push(point3);
        point3 = point2;
        --point3.X;
        if (pointSet1.Add(point3))
          pointStack.Push(point3);
        point3 = point2;
        --point3.Y;
        if (pointSet1.Add(point3))
          pointStack.Push(point3);
        point3 = point2;
        ++point3.Y;
        if (pointSet1.Add(point3))
          pointStack.Push(point3);
      }
    }
    return flag;
  }

  /// <inheritdoc />
  public override void receiveLeftClick(int x, int y, bool playSound = true)
  {
    if (this.freeze)
      return;
    if (!this.onFarm)
      base.receiveLeftClick(x, y, playSound);
    if (this.cancelButton.containsPoint(x, y))
    {
      if (!this.onFarm)
      {
        this.exitThisMenu();
        Game1.player.forceCanMove();
        Game1.playSound("bigDeSelect");
      }
      else
      {
        if (this.Action == CarpenterMenu.CarpentryAction.Move && this.buildingToMove != null)
        {
          Game1.playSound("cancel");
          return;
        }
        this.returnToCarpentryMenu();
        Game1.playSound("smallSelect");
        return;
      }
    }
    if (!this.onFarm && this.backButton.containsPoint(x, y))
    {
      this.SetNewActiveBlueprint(this.Blueprint.Index - 1);
      Game1.playSound("shwip");
      this.backButton.scale = this.backButton.baseScale;
    }
    if (!this.onFarm && this.forwardButton.containsPoint(x, y))
    {
      this.SetNewActiveBlueprint(this.Blueprint.Index + 1);
      this.forwardButton.scale = this.forwardButton.baseScale;
      Game1.playSound("shwip");
    }
    if (!this.onFarm)
    {
      if (this.demolishButton.containsPoint(x, y) && this.demolishButton.visible && this.CanDemolishThis())
      {
        Game1.globalFadeToBlack(new Game1.afterFadeFunction(this.setUpForBuildingPlacement));
        Game1.playSound("smallSelect");
        this.onFarm = true;
        this.Action = CarpenterMenu.CarpentryAction.Demolish;
      }
      else if (this.moveButton.containsPoint(x, y) && this.moveButton.visible)
      {
        Game1.globalFadeToBlack(new Game1.afterFadeFunction(this.setUpForBuildingPlacement));
        Game1.playSound("smallSelect");
        this.onFarm = true;
        this.Action = CarpenterMenu.CarpentryAction.Move;
      }
      else if (this.paintButton.containsPoint(x, y) && this.paintButton.visible)
      {
        Game1.globalFadeToBlack(new Game1.afterFadeFunction(this.setUpForBuildingPlacement));
        Game1.playSound("smallSelect");
        this.onFarm = true;
        this.Action = CarpenterMenu.CarpentryAction.Paint;
      }
      else if (this.appearanceButton.containsPoint(x, y) && this.appearanceButton.visible)
      {
        if (this.currentBuilding.CanBeReskinned(true))
        {
          BuildingSkinMenu skinMenu = new BuildingSkinMenu(this.currentBuilding, true);
          Game1.playSound("smallSelect");
          BuildingSkinMenu buildingSkinMenu = skinMenu;
          buildingSkinMenu.behaviorBeforeCleanup = buildingSkinMenu.behaviorBeforeCleanup + (System.Action<IClickableMenu>) (menu =>
          {
            if (Game1.options.SnappyMenus)
            {
              this.setCurrentlySnappedComponentTo(109);
              this.snapCursorToCurrentSnappedComponent();
            }
            this.Blueprint.SetSkin(skinMenu.Skin?.Id);
          });
          this.SetChildMenu((IClickableMenu) skinMenu);
        }
      }
      else if (this.okButton.containsPoint(x, y) && !this.onFarm && this.CanBuildCurrentBlueprint())
      {
        Game1.globalFadeToBlack(new Game1.afterFadeFunction(this.setUpForBuildingPlacement));
        Game1.playSound("smallSelect");
        this.onFarm = true;
      }
    }
    if (!this.onFarm || this.freeze || Game1.IsFading())
      return;
    switch (this.Action)
    {
      case CarpenterMenu.CarpentryAction.Demolish:
        GameLocation farm = this.TargetLocation;
        Building destroyed = farm.getBuildingAt(new Vector2((float) ((Game1.viewport.X + Game1.getOldMouseX(false)) / 64 /*0x40*/), (float) ((Game1.viewport.Y + Game1.getOldMouseY(false)) / 64 /*0x40*/)));
        if (destroyed == null)
          break;
        GameLocation interior = destroyed.GetIndoors();
        Cabin cabin = interior as Cabin;
        if (destroyed != null)
        {
          if (cabin != null && !Game1.IsMasterGame)
          {
            Game1.addHUDMessage(new HUDMessage(Game1.content.LoadString("Strings\\UI:Carpenter_CantDemolish_LockFailed"), 3));
            destroyed = (Building) null;
            break;
          }
          if (!this.CanDemolishThis(destroyed))
          {
            destroyed = (Building) null;
            break;
          }
          if (!Game1.IsMasterGame && !this.hasPermissionsToDemolish(destroyed))
          {
            destroyed = (Building) null;
            break;
          }
        }
        Cabin cabin1 = cabin;
        if ((cabin1 != null ? (cabin1.HasOwner ? 1 : 0) : 0) != 0 && cabin.owner.isCustomized.Value)
        {
          Game1.currentLocation.createQuestionDialogue(Game1.content.LoadString("Strings\\UI:Carpenter_DemolishCabinConfirm", (object) cabin.owner.Name), Game1.currentLocation.createYesNoResponses(), (GameLocation.afterQuestionBehavior) ((f, answer) =>
          {
            if (answer == "Yes")
            {
              Game1.activeClickableMenu = (IClickableMenu) this;
              // ISSUE: method pointer
              Game1.player.team.demolishLock.RequestLock(new System.Action(ContinueDemolish), new System.Action((object) this, __methodptr(\u003CreceiveLeftClick\u003Eg__BuildingLockFailed\u007C58_2)));
            }
            else
              DelayedAction.functionAfterDelay(new System.Action(this.returnToCarpentryMenu), 500);
          }));
          break;
        }
        if (destroyed == null)
          break;
        // ISSUE: method pointer
        Game1.player.team.demolishLock.RequestLock(new System.Action(ContinueDemolish), new System.Action((object) this, __methodptr(\u003CreceiveLeftClick\u003Eg__BuildingLockFailed\u007C58_2)));
        break;

        void ContinueDemolish()
        {
          if (this.Action != CarpenterMenu.CarpentryAction.Demolish || destroyed == null || !farm.buildings.Contains(destroyed))
            return;
          if (destroyed.daysOfConstructionLeft.Value > 0 || destroyed.daysUntilUpgrade.Value > 0)
            Game1.addHUDMessage(new HUDMessage(Game1.content.LoadString("Strings\\UI:Carpenter_CantDemolish_DuringConstruction"), 3));
          else if (interior is AnimalHouse animalHouse && animalHouse.animalsThatLiveHere.Count > 0)
            Game1.addHUDMessage(new HUDMessage(Game1.content.LoadString("Strings\\UI:Carpenter_CantDemolish_AnimalsHere"), 3));
          else if (interior != null && interior.farmers.Any())
          {
            Game1.addHUDMessage(new HUDMessage(Game1.content.LoadString("Strings\\UI:Carpenter_CantDemolish_PlayerHere"), 3));
          }
          else
          {
            if (cabin != null)
            {
              foreach (Farmer allFarmer in Game1.getAllFarmers())
              {
                if (allFarmer.currentLocation != null && allFarmer.currentLocation.Name == cabin.GetCellarName())
                {
                  Game1.addHUDMessage(new HUDMessage(Game1.content.LoadString("Strings\\UI:Carpenter_CantDemolish_PlayerHere"), 3));
                  return;
                }
              }
              if (cabin.IsOwnerActivated)
              {
                Game1.addHUDMessage(new HUDMessage(Game1.content.LoadString("Strings\\UI:Carpenter_CantDemolish_FarmhandOnline"), 3));
                return;
              }
            }
            destroyed.BeforeDemolish();
            Chest chest = (Chest) null;
            if (cabin != null)
            {
              List<Item> list = cabin.demolish();
              if (list.Count > 0)
              {
                chest = new Chest(true);
                chest.fixLidFrame();
                chest.Items.OverwriteWith((IList<Item>) list);
              }
            }
            if (!farm.destroyStructure(destroyed))
              return;
            Game1.flashAlpha = 1f;
            destroyed.showDestroyedAnimation(this.TargetLocation);
            Game1.playSound("explosion");
            Utility.spreadAnimalsAround(destroyed, farm);
            DelayedAction.functionAfterDelay(new System.Action(this.returnToCarpentryMenu), 1500);
            this.freeze = true;
            if (chest == null)
              return;
            farm.objects[new Vector2((float) (destroyed.tileX.Value + destroyed.tilesWide.Value / 2), (float) (destroyed.tileY.Value + destroyed.tilesHigh.Value / 2))] = (StardewValley.Object) chest;
          }
        }
      case CarpenterMenu.CarpentryAction.Move:
        if (this.buildingToMove == null)
        {
          this.buildingToMove = this.TargetLocation.getBuildingAt(new Vector2((float) ((Game1.viewport.X + Game1.getMouseX(false)) / 64 /*0x40*/), (float) ((Game1.viewport.Y + Game1.getMouseY(false)) / 64 /*0x40*/)));
          if (this.buildingToMove == null)
            break;
          if (this.buildingToMove.daysOfConstructionLeft.Value > 0)
          {
            this.buildingToMove = (Building) null;
            break;
          }
          if (!this.hasPermissionsToMove(this.buildingToMove))
          {
            this.buildingToMove = (Building) null;
            break;
          }
          this.buildingToMove.isMoving = true;
          Game1.playSound("axchop");
          break;
        }
        Vector2 vector2 = new Vector2((float) ((Game1.viewport.X + Game1.getMouseX(false)) / 64 /*0x40*/), (float) ((Game1.viewport.Y + Game1.getMouseY(false)) / 64 /*0x40*/));
        if (this.ConfirmBuildingAccessibility(vector2))
        {
          if (this.TargetLocation.buildStructure(this.buildingToMove, vector2, Game1.player))
          {
            this.buildingToMove.isMoving = false;
            this.buildingToMove = (Building) null;
            Game1.playSound("axchop");
            DelayedAction.playSoundAfterDelay("dirtyHit", 50);
            DelayedAction.playSoundAfterDelay("dirtyHit", 150);
            break;
          }
          Game1.playSound("cancel");
          break;
        }
        Game1.addHUDMessage(new HUDMessage(Game1.content.LoadString("Strings\\UI:Carpenter_CantBuild"), 3));
        Game1.playSound("cancel");
        break;
      case CarpenterMenu.CarpentryAction.Paint:
        Building buildingAt1 = this.TargetLocation.getBuildingAt(new Vector2((float) ((Game1.viewport.X + Game1.getMouseX(false)) / 64 /*0x40*/), (float) ((Game1.viewport.Y + Game1.getMouseY(false)) / 64 /*0x40*/)));
        if (buildingAt1 == null)
          break;
        if (!buildingAt1.CanBePainted() && !buildingAt1.CanBeReskinned(true))
        {
          Game1.addHUDMessage(new HUDMessage(Game1.content.LoadString("Strings\\UI:Carpenter_CannotPaint"), 3));
          break;
        }
        if (!this.HasPermissionsToPaint(buildingAt1))
        {
          Game1.addHUDMessage(new HUDMessage(Game1.content.LoadString("Strings\\UI:Carpenter_CannotPaint_Permission"), 3));
          break;
        }
        buildingAt1.color = Color.White;
        this.SetChildMenu(buildingAt1.CanBePainted() ? (IClickableMenu) new BuildingPaintMenu(buildingAt1) : (IClickableMenu) new BuildingSkinMenu(buildingAt1, true));
        break;
      case CarpenterMenu.CarpentryAction.Upgrade:
        Building buildingAt2 = this.TargetLocation.getBuildingAt(new Vector2((float) ((Game1.viewport.X + Game1.getOldMouseX(false)) / 64 /*0x40*/), (float) ((Game1.viewport.Y + Game1.getOldMouseY(false)) / 64 /*0x40*/)));
        if (buildingAt2 != null && buildingAt2.buildingType.Value == this.Blueprint.UpgradeFrom)
        {
          this.ConsumeResources();
          buildingAt2.upgradeName.Value = this.Blueprint.Id;
          buildingAt2.daysUntilUpgrade.Value = Math.Max(this.Blueprint.BuildDays, 1);
          buildingAt2.showUpgradeAnimation(this.TargetLocation);
          Game1.playSound("axe");
          DelayedAction.functionAfterDelay(new System.Action(this.returnToCarpentryMenuAfterSuccessfulBuild), 1500);
          this.freeze = true;
          Game1.multiplayer.globalChatInfoMessage("BuildingBuild", Game1.player.Name, "aOrAn:" + this.Blueprint.TokenizedDisplayName, this.Blueprint.TokenizedDisplayName, Game1.player.farmName.Value);
          if (this.Blueprint.BuildDays < 1)
          {
            buildingAt2.FinishConstruction();
            break;
          }
          Game1.netWorldState.Value.MarkUnderConstruction(this.Builder, buildingAt2);
          break;
        }
        if (buildingAt2 == null)
          break;
        Game1.addHUDMessage(new HUDMessage(Game1.content.LoadString("Strings\\UI:Carpenter_CantUpgrade_BuildingType"), 3));
        break;
      default:
        Game1.player.team.buildLock.RequestLock((System.Action) (() =>
        {
          if (this.onFarm && Game1.locationRequest == null)
          {
            if (this.tryToBuild())
            {
              this.ConsumeResources();
              DelayedAction.functionAfterDelay(new System.Action(this.returnToCarpentryMenuAfterSuccessfulBuild), 2000);
              this.freeze = true;
            }
            else
              Game1.addHUDMessage(new HUDMessage(Game1.content.LoadString("Strings\\UI:Carpenter_CantBuild"), 3));
          }
          Game1.player.team.buildLock.ReleaseLock();
        }));
        break;
    }
  }

  public bool tryToBuild()
  {
    NetString skinId = this.currentBuilding.skinId;
    Building constructed;
    if (!this.TargetLocation.buildStructure(this.currentBuilding.buildingType.Value, new Vector2((float) ((Game1.viewport.X + Game1.getOldMouseX(false)) / 64 /*0x40*/), (float) ((Game1.viewport.Y + Game1.getOldMouseY(false)) / 64 /*0x40*/)), Game1.player, out constructed, this.Blueprint.MagicalConstruction))
      return false;
    constructed.skinId.Value = skinId.Value;
    if (constructed.isUnderConstruction())
      Game1.netWorldState.Value.MarkUnderConstruction(this.Builder, constructed);
    return true;
  }

  public virtual void returnToCarpentryMenu()
  {
    LocationRequest locationRequest = Game1.getLocationRequest(this.BuilderLocationName);
    locationRequest.OnWarp += (LocationRequest.Callback) (() =>
    {
      this.onFarm = false;
      Game1.player.viewingLocation.Value = (string) null;
      this.resetBounds();
      this.Action = CarpenterMenu.CarpentryAction.None;
      this.buildingToMove = (Building) null;
      this.freeze = false;
      Game1.displayHUD = true;
      Game1.viewportFreeze = false;
      Game1.viewport.Location = this.BuilderViewport;
      this.drawBG = true;
      Game1.displayFarmer = true;
      if (!Game1.options.SnappyMenus)
        return;
      this.populateClickableComponentList();
      this.snapToDefaultClickableComponent();
    });
    Game1.warpFarmer(locationRequest, Game1.player.TilePoint.X, Game1.player.TilePoint.Y, Game1.player.FacingDirection);
  }

  public void returnToCarpentryMenuAfterSuccessfulBuild()
  {
    LocationRequest locationRequest = Game1.getLocationRequest(this.BuilderLocationName);
    locationRequest.OnWarp += (LocationRequest.Callback) (() =>
    {
      Game1.displayHUD = true;
      Game1.player.viewingLocation.Value = (string) null;
      Game1.viewportFreeze = false;
      Game1.viewport.Location = this.BuilderViewport;
      this.freeze = true;
      Game1.displayFarmer = true;
      this.robinConstructionMessage();
    });
    Game1.warpFarmer(locationRequest, Game1.player.TilePoint.X, Game1.player.TilePoint.Y, Game1.player.FacingDirection);
  }

  public void robinConstructionMessage()
  {
    this.exitThisMenu();
    Game1.player.forceCanMove();
    if (this.Blueprint.MagicalConstruction)
      return;
    string translationKey = $"Data\\ExtraDialogue:Robin_{(this.Action == CarpenterMenu.CarpentryAction.Upgrade ? "Upgrade" : "New")}Construction";
    if (Utility.isFestivalDay(Game1.dayOfMonth + 1, Game1.season))
      translationKey += "_Festival";
    string displayName = this.Blueprint.DisplayName;
    string nameForGeneralType = this.Blueprint.DisplayNameForGeneralType;
    if (this.Blueprint.BuildDays <= 0)
      Game1.DrawDialogue(Game1.getCharacterFromName("Robin"), "Data\\ExtraDialogue:Robin_Instant", (object) displayName.ToLower(), (object) displayName);
    else
      Game1.DrawDialogue(Game1.getCharacterFromName("Robin"), translationKey, (object) displayName.ToLower(), (object) nameForGeneralType.ToLower(), (object) displayName, (object) nameForGeneralType);
  }

  /// <inheritdoc />
  public override bool overrideSnappyMenuCursorMovementBan() => this.onFarm;

  public void setUpForBuildingPlacement()
  {
    Game1.currentLocation.cleanupBeforePlayerExit();
    this.hoverText = "";
    Game1.currentLocation = this.TargetLocation;
    Game1.player.viewingLocation.Value = this.TargetLocation.NameOrUniqueName;
    Game1.currentLocation.resetForPlayerEntry();
    Game1.globalFadeToClear();
    this.onFarm = true;
    this.cancelButton.bounds.X = Game1.uiViewport.Width - 128 /*0x80*/;
    this.cancelButton.bounds.Y = Game1.uiViewport.Height - 128 /*0x80*/;
    Game1.displayHUD = false;
    Game1.viewportFreeze = true;
    Game1.viewport.Location = this.GetInitialBuildingPlacementViewport(this.TargetLocation);
    Game1.clampViewportToGameMap();
    Game1.panScreen(0, 0);
    this.drawBG = false;
    this.freeze = false;
    Game1.displayFarmer = false;
    if (!this.Blueprint.IsUpgrade || this.Action != CarpenterMenu.CarpentryAction.None)
      return;
    this.Action = CarpenterMenu.CarpentryAction.Upgrade;
  }

  /// <summary>Get the viewport to set when we start building placement.</summary>
  /// <param name="location">The location for which to get a viewport.</param>
  public Location GetInitialBuildingPlacementViewport(GameLocation location)
  {
    if (this.TargetViewportCenterOnTile.HasValue)
    {
      Vector2 vector2 = this.TargetViewportCenterOnTile.Value;
      return CenterOnTile((int) vector2.X, (int) vector2.Y);
    }
    Building building = location.getBuildingByName("FarmHouse") ?? location.buildings.FirstOrDefault<Building>();
    if (building != null)
      return CenterOnTile(building.tileX.Value + building.tilesWide.Value / 2, building.tileY.Value + building.tilesHigh.Value / 2);
    Layer layer = location.Map.Layers[0];
    return CenterOnTile(layer.LayerWidth / 2, layer.LayerHeight / 2);

    static Location CenterOnTile(int x, int y)
    {
      x = (int) ((double) (x * 64 /*0x40*/) - (double) Game1.viewport.Width / 2.0);
      y = (int) ((double) (y * 64 /*0x40*/) - (double) Game1.viewport.Height / 2.0);
      return new Location(x, y);
    }
  }

  /// <inheritdoc />
  public override void gameWindowSizeChanged(Microsoft.Xna.Framework.Rectangle oldBounds, Microsoft.Xna.Framework.Rectangle newBounds)
  {
    this.resetBounds();
  }

  /// <summary>Get whether a building can ever be built in the target location.</summary>
  /// <param name="typeId">The building type ID in <c>Data/Buildings</c>.</param>
  /// <param name="data">The building data from <c>Data/Buildings</c>.</param>
  /// <param name="targetLocation">The location it would be built in.</param>
  public virtual bool IsValidBuildingForLocation(
    string typeId,
    BuildingData data,
    GameLocation targetLocation)
  {
    return !(typeId == "Cabin") || !(this.TargetLocation.Name != "Farm");
  }

  /// <summary>Get whether the player can build the current blueprint now.</summary>
  public virtual bool CanBuildCurrentBlueprint()
  {
    CarpenterMenu.BlueprintEntry blueprint = this.Blueprint;
    return this.IsValidBuildingForLocation(blueprint.Id, blueprint.Data, this.TargetLocation) && this.DoesFarmerHaveEnoughResourcesToBuild() && (blueprint.BuildCost <= 0 || Game1.player.Money >= blueprint.BuildCost);
  }

  /// <summary>Get whether it's safe to demolish the current building.</summary>
  public bool CanDemolishThis() => this.CanDemolishThis(this.currentBuilding);

  /// <summary>Get whether it's safe to demolish a given building.</summary>
  /// <param name="building">The building to check.</param>
  public virtual bool CanDemolishThis(Building building)
  {
    string buildingType = building?.buildingType.Value;
    switch (buildingType)
    {
      case "Farmhouse":
        if (building.HasIndoorsName("FarmHouse"))
          return false;
        break;
      case "Greenhouse":
        if (building.HasIndoorsName("Greenhouse"))
          return false;
        break;
      case "Pet Bowl":
      case "Shipping Bin":
        if (this.TargetLocation == Game1.getFarm() && !this.TargetLocation.HasMinBuildings(buildingType, 2))
          return false;
        break;
    }
    return building != null;
  }

  /// <inheritdoc />
  public override void draw(SpriteBatch b)
  {
    CarpenterMenu.BlueprintEntry blueprint = this.Blueprint;
    if (this.drawBG && !Game1.options.showClearBackgrounds)
      b.Draw(Game1.fadeToBlackRect, Game1.graphics.GraphicsDevice.Viewport.Bounds, Color.Black * 0.6f);
    if (Game1.IsFading() || this.freeze)
      return;
    if (!this.onFarm)
    {
      base.draw(b);
      Microsoft.Xna.Framework.Rectangle rectangle1 = new Microsoft.Xna.Framework.Rectangle(this.xPositionOnScreen - 96 /*0x60*/, this.yPositionOnScreen - 16 /*0x10*/, this.maxWidthOfBuildingViewer + 64 /*0x40*/, this.maxHeightOfBuildingViewer + 64 /*0x40*/);
      IClickableMenu.drawTextureBox(b, rectangle1.X, rectangle1.Y, rectangle1.Width, rectangle1.Height, blueprint.MagicalConstruction ? Color.RoyalBlue : Color.White);
      rectangle1.Inflate(-12, -12);
      b.End();
      b.Begin(SpriteSortMode.FrontToBack, BlendState.AlphaBlend, SamplerState.PointClamp, rasterizerState: Utility.ScissorEnabled);
      b.GraphicsDevice.ScissorRectangle = rectangle1;
      Microsoft.Xna.Framework.Rectangle rectangle2 = this.currentBuilding.getSourceRectForMenu() ?? this.currentBuilding.getSourceRect();
      Point buildMenuDrawOffset = blueprint.Data.BuildMenuDrawOffset;
      this.currentBuilding.drawInMenu(b, this.xPositionOnScreen + this.maxWidthOfBuildingViewer / 2 - this.currentBuilding.tilesWide.Value * 64 /*0x40*/ / 2 - 64 /*0x40*/ + buildMenuDrawOffset.X, this.yPositionOnScreen + this.maxHeightOfBuildingViewer / 2 - rectangle2.Height * 4 / 2 + buildMenuDrawOffset.Y);
      b.End();
      b.Begin(blendState: BlendState.AlphaBlend, samplerState: SamplerState.PointClamp);
      if (blueprint.IsUpgrade)
        this.upgradeIcon.draw(b);
      string s = " Deluxe  Barn   ";
      if (SpriteText.getWidthOfString(blueprint.DisplayName) >= SpriteText.getWidthOfString(s))
        s = blueprint.DisplayName + " ";
      SpriteText.drawStringWithScrollCenteredAt(b, blueprint.DisplayName, this.xPositionOnScreen + this.maxWidthOfBuildingViewer - IClickableMenu.spaceToClearSideBorder - 16 /*0x10*/ + 64 /*0x40*/ + (this.width - (this.maxWidthOfBuildingViewer + 128 /*0x80*/)) / 2, this.yPositionOnScreen, SpriteText.getWidthOfString(s));
      int width;
      switch (LocalizedContentManager.CurrentLanguageCode)
      {
        case LocalizedContentManager.LanguageCode.es:
          width = this.maxWidthOfDescription + 64 /*0x40*/ + (blueprint.Id == "Deluxe Barn" ? 96 /*0x60*/ : 0);
          break;
        case LocalizedContentManager.LanguageCode.fr:
          width = this.maxWidthOfDescription + 96 /*0x60*/ + (blueprint.Id == "Slime Hutch" || blueprint.Id == "Deluxe Coop" || blueprint.Id == "Deluxe Barn" ? 72 : 0);
          break;
        case LocalizedContentManager.LanguageCode.ko:
          width = this.maxWidthOfDescription + 96 /*0x60*/ + (blueprint.Id == "Slime Hutch" ? 64 /*0x40*/ : (blueprint.Id == "Deluxe Coop" ? 96 /*0x60*/ : (blueprint.Id == "Deluxe Barn" ? 112 /*0x70*/ : (blueprint.Id == "Big Barn" ? 64 /*0x40*/ : 0))));
          break;
        case LocalizedContentManager.LanguageCode.it:
          width = this.maxWidthOfDescription + 96 /*0x60*/;
          break;
        default:
          width = this.maxWidthOfDescription + 64 /*0x40*/;
          break;
      }
      IClickableMenu.drawTextureBox(b, this.xPositionOnScreen + this.maxWidthOfBuildingViewer - 16 /*0x10*/, this.yPositionOnScreen + 80 /*0x50*/, width, this.maxHeightOfBuildingViewer - 32 /*0x20*/, blueprint.MagicalConstruction ? Color.RoyalBlue : Color.White);
      if (blueprint.MagicalConstruction)
      {
        Utility.drawTextWithShadow(b, Game1.parseText(blueprint.Description, Game1.dialogueFont, width - 32 /*0x20*/), Game1.dialogueFont, new Vector2((float) (this.xPositionOnScreen + this.maxWidthOfBuildingViewer - 4), (float) (this.yPositionOnScreen + 80 /*0x50*/ + 16 /*0x10*/ + 4)), Game1.textColor * 0.25f, shadowIntensity: 0.0f);
        Utility.drawTextWithShadow(b, Game1.parseText(blueprint.Description, Game1.dialogueFont, width - 32 /*0x20*/), Game1.dialogueFont, new Vector2((float) (this.xPositionOnScreen + this.maxWidthOfBuildingViewer - 1), (float) (this.yPositionOnScreen + 80 /*0x50*/ + 16 /*0x10*/ + 4)), Game1.textColor * 0.25f, shadowIntensity: 0.0f);
      }
      Utility.drawTextWithShadow(b, Game1.parseText(blueprint.Description, Game1.dialogueFont, width - 32 /*0x20*/), Game1.dialogueFont, new Vector2((float) (this.xPositionOnScreen + this.maxWidthOfBuildingViewer), (float) (this.yPositionOnScreen + 80 /*0x50*/ + 16 /*0x10*/)), blueprint.MagicalConstruction ? Color.PaleGoldenrod : Game1.textColor, shadowIntensity: blueprint.MagicalConstruction ? 0.0f : 0.75f);
      Vector2 location = new Vector2((float) (this.xPositionOnScreen + this.maxWidthOfBuildingViewer + 16 /*0x10*/), (float) (this.yPositionOnScreen + 256 /*0x0100*/ + 32 /*0x20*/));
      if (this.ingredients.Count < 3 && (LocalizedContentManager.CurrentLanguageCode == LocalizedContentManager.LanguageCode.fr || LocalizedContentManager.CurrentLanguageCode == LocalizedContentManager.LanguageCode.ko || LocalizedContentManager.CurrentLanguageCode == LocalizedContentManager.LanguageCode.pt))
        location.Y += 64f;
      if (blueprint.BuildCost >= 0)
      {
        b.Draw(Game1.mouseCursors_1_6, location + new Vector2(-8f, -4f), new Microsoft.Xna.Framework.Rectangle?(new Microsoft.Xna.Framework.Rectangle(241, 303, 14, 13)), Color.White, 0.0f, Vector2.Zero, 4f, SpriteEffects.None, 0.89f);
        string numberWithCommas = Utility.getNumberWithCommas(blueprint.BuildCost);
        if (blueprint.MagicalConstruction)
        {
          Utility.drawTextWithShadow(b, Game1.content.LoadString("Strings\\StringsFromCSFiles:LoadGameMenu.cs.11020", (object) numberWithCommas), Game1.dialogueFont, new Vector2(location.X + 64f, location.Y + 8f), Game1.textColor * 0.5f, shadowIntensity: 0.0f);
          Utility.drawTextWithShadow(b, Game1.content.LoadString("Strings\\StringsFromCSFiles:LoadGameMenu.cs.11020", (object) numberWithCommas), Game1.dialogueFont, new Vector2((float) ((double) location.X + 64.0 + 4.0 - 1.0), location.Y + 8f), Game1.textColor * 0.25f, shadowIntensity: 0.0f);
        }
        Utility.drawTextWithShadow(b, Game1.content.LoadString("Strings\\StringsFromCSFiles:LoadGameMenu.cs.11020", (object) numberWithCommas), Game1.dialogueFont, new Vector2((float) ((double) location.X + 64.0 + 4.0), location.Y + 4f), Game1.player.Money >= blueprint.BuildCost ? (blueprint.MagicalConstruction ? Color.PaleGoldenrod : Game1.textColor) : Color.Red, shadowIntensity: blueprint.MagicalConstruction ? 0.0f : 0.25f);
      }
      if (!blueprint.MagicalConstruction)
      {
        int buildDays = blueprint.BuildDays;
        string text = buildDays > 1 ? Game1.content.LoadString("Strings\\StringsFromCSFiles:QuestLog.cs.11374", (object) buildDays) : (buildDays == 1 ? Game1.content.LoadString("Strings\\StringsFromCSFiles:QuestLog.cs.11375", (object) buildDays) : Game1.content.LoadString("Strings\\1_6_Strings:Instant"));
        rectangle1 = new Microsoft.Xna.Framework.Rectangle(this.xPositionOnScreen - 96 /*0x60*/ + this.width + 64 /*0x40*/, this.yPositionOnScreen + 80 /*0x50*/, 72 + (int) Game1.smallFont.MeasureString(text).X, 68);
        IClickableMenu.drawTextureBox(b, rectangle1.X - 8, rectangle1.Y, rectangle1.Width + 16 /*0x10*/, rectangle1.Height, Color.White);
        b.Draw(Game1.mouseCursors, new Vector2((float) (rectangle1.X + 8), (float) (rectangle1.Y + 16 /*0x10*/)), new Microsoft.Xna.Framework.Rectangle?(new Microsoft.Xna.Framework.Rectangle(410, 501, 9, 9)), Color.White, 0.0f, Vector2.Zero, 4f, SpriteEffects.None, 0.89f);
        Utility.drawTextWithShadow(b, text, Game1.smallFont, new Vector2((float) (rectangle1.X + 8 + 44), (float) (rectangle1.Y + 20)), Game1.textColor);
      }
      location.X -= 16f;
      location.Y -= 21f;
      foreach (Item ingredient in this.ingredients)
      {
        location.Y += 68f;
        ingredient.drawInMenu(b, location, 1f);
        bool flag = Game1.player.Items.ContainsId(ingredient.QualifiedItemId, ingredient.Stack);
        if (blueprint.MagicalConstruction)
        {
          Utility.drawTextWithShadow(b, ingredient.DisplayName, Game1.dialogueFont, new Vector2((float) ((double) location.X + 64.0 + 12.0), location.Y + 24f), Game1.textColor * 0.25f, shadowIntensity: 0.0f);
          Utility.drawTextWithShadow(b, ingredient.DisplayName, Game1.dialogueFont, new Vector2((float) ((double) location.X + 64.0 + 16.0 - 1.0), location.Y + 24f), Game1.textColor * 0.25f, shadowIntensity: 0.0f);
        }
        Utility.drawTextWithShadow(b, ingredient.DisplayName, Game1.dialogueFont, new Vector2((float) ((double) location.X + 64.0 + 16.0), location.Y + 20f), flag ? (blueprint.MagicalConstruction ? Color.PaleGoldenrod : Game1.textColor) : Color.Red, shadowIntensity: blueprint.MagicalConstruction ? 0.0f : 0.25f);
      }
      this.backButton.draw(b);
      this.forwardButton.draw(b);
      this.okButton.draw(b, this.CanBuildCurrentBlueprint() ? Color.White : Color.Gray * 0.8f, 0.88f);
      this.demolishButton.draw(b, this.CanDemolishThis() ? Color.White : Color.Gray * 0.8f, 0.88f);
      this.moveButton.draw(b);
      this.paintButton.draw(b);
      this.appearanceButton.draw(b);
    }
    else
    {
      string s;
      switch (this.Action)
      {
        case CarpenterMenu.CarpentryAction.Demolish:
          s = Game1.content.LoadString("Strings\\UI:Carpenter_SelectBuilding_Demolish");
          break;
        case CarpenterMenu.CarpentryAction.Move:
          s = Game1.content.LoadString("Strings\\UI:Carpenter_SelectBuilding_Move");
          break;
        case CarpenterMenu.CarpentryAction.Paint:
          s = Game1.content.LoadString("Strings\\UI:Carpenter_SelectBuilding_Paint");
          break;
        case CarpenterMenu.CarpentryAction.Upgrade:
          s = Game1.content.LoadString("Strings\\UI:Carpenter_SelectBuilding_Upgrade", (object) blueprint.GetDisplayNameForBuildingToUpgrade());
          break;
        default:
          s = Game1.content.LoadString("Strings\\UI:Carpenter_ChooseLocation");
          break;
      }
      SpriteText.drawStringWithScrollBackground(b, s, Game1.uiViewport.Width / 2 - SpriteText.getWidthOfString(s) / 2, 16 /*0x10*/);
      Game1.StartWorldDrawInUI(b);
      switch (this.Action)
      {
        case CarpenterMenu.CarpentryAction.None:
          Vector2 vector2_1 = new Vector2((float) ((Game1.viewport.X + Game1.getOldMouseX(false)) / 64 /*0x40*/), (float) ((Game1.viewport.Y + Game1.getOldMouseY(false)) / 64 /*0x40*/));
          for (int y = 0; y < this.currentBuilding.tilesHigh.Value; ++y)
          {
            for (int x = 0; x < this.currentBuilding.tilesWide.Value; ++x)
            {
              int structurePlacementTile = this.currentBuilding.getTileSheetIndexForStructurePlacementTile(x, y);
              Vector2 tileLocation = new Vector2(vector2_1.X + (float) x, vector2_1.Y + (float) y);
              if (!Game1.currentLocation.isBuildable(tileLocation))
                ++structurePlacementTile;
              b.Draw(Game1.mouseCursors, Game1.GlobalToLocal(Game1.viewport, tileLocation * 64f), new Microsoft.Xna.Framework.Rectangle?(new Microsoft.Xna.Framework.Rectangle(194 + structurePlacementTile * 16 /*0x10*/, 388, 16 /*0x10*/, 16 /*0x10*/)), Color.White, 0.0f, Vector2.Zero, 4f, SpriteEffects.None, 0.999f);
            }
          }
          using (IEnumerator<BuildingPlacementTile> enumerator = this.currentBuilding.GetAdditionalPlacementTiles().GetEnumerator())
          {
            while (enumerator.MoveNext())
            {
              BuildingPlacementTile current = enumerator.Current;
              bool needsToBePassable = current.OnlyNeedsToBePassable;
              foreach (Point point in current.TileArea.GetPoints())
              {
                int x = point.X;
                int y = point.Y;
                int structurePlacementTile = this.currentBuilding.getTileSheetIndexForStructurePlacementTile(x, y);
                Vector2 tileLocation = new Vector2(vector2_1.X + (float) x, vector2_1.Y + (float) y);
                if (!Game1.currentLocation.isBuildable(tileLocation, needsToBePassable))
                  ++structurePlacementTile;
                b.Draw(Game1.mouseCursors, Game1.GlobalToLocal(Game1.viewport, tileLocation * 64f), new Microsoft.Xna.Framework.Rectangle?(new Microsoft.Xna.Framework.Rectangle(194 + structurePlacementTile * 16 /*0x10*/, 388, 16 /*0x10*/, 16 /*0x10*/)), Color.White, 0.0f, Vector2.Zero, 4f, SpriteEffects.None, 0.999f);
              }
            }
            break;
          }
        case CarpenterMenu.CarpentryAction.Move:
          if (this.buildingToMove != null)
          {
            Vector2 vector2_2 = new Vector2((float) ((Game1.viewport.X + Game1.getOldMouseX(false)) / 64 /*0x40*/), (float) ((Game1.viewport.Y + Game1.getOldMouseY(false)) / 64 /*0x40*/));
            for (int y = 0; y < this.buildingToMove.tilesHigh.Value; ++y)
            {
              for (int x = 0; x < this.buildingToMove.tilesWide.Value; ++x)
              {
                int structurePlacementTile = this.buildingToMove.getTileSheetIndexForStructurePlacementTile(x, y);
                Vector2 tileLocation = new Vector2(vector2_2.X + (float) x, vector2_2.Y + (float) y);
                if (!Game1.currentLocation.isBuildable(tileLocation))
                  ++structurePlacementTile;
                b.Draw(Game1.mouseCursors, Game1.GlobalToLocal(Game1.viewport, tileLocation * 64f), new Microsoft.Xna.Framework.Rectangle?(new Microsoft.Xna.Framework.Rectangle(194 + structurePlacementTile * 16 /*0x10*/, 388, 16 /*0x10*/, 16 /*0x10*/)), Color.White, 0.0f, Vector2.Zero, 4f, SpriteEffects.None, 0.999f);
              }
            }
            using (IEnumerator<BuildingPlacementTile> enumerator = this.buildingToMove.GetAdditionalPlacementTiles().GetEnumerator())
            {
              while (enumerator.MoveNext())
              {
                BuildingPlacementTile current = enumerator.Current;
                bool needsToBePassable = current.OnlyNeedsToBePassable;
                foreach (Point point in current.TileArea.GetPoints())
                {
                  int x = point.X;
                  int y = point.Y;
                  int structurePlacementTile = this.buildingToMove.getTileSheetIndexForStructurePlacementTile(x, y);
                  Vector2 tileLocation = new Vector2(vector2_2.X + (float) x, vector2_2.Y + (float) y);
                  if (!Game1.currentLocation.isBuildable(tileLocation, needsToBePassable))
                    ++structurePlacementTile;
                  b.Draw(Game1.mouseCursors, Game1.GlobalToLocal(Game1.viewport, tileLocation * 64f), new Microsoft.Xna.Framework.Rectangle?(new Microsoft.Xna.Framework.Rectangle(194 + structurePlacementTile * 16 /*0x10*/, 388, 16 /*0x10*/, 16 /*0x10*/)), Color.White, 0.0f, Vector2.Zero, 4f, SpriteEffects.None, 0.999f);
                }
              }
              break;
            }
          }
          break;
      }
      Game1.EndWorldDrawInUI(b);
    }
    this.cancelButton.draw(b);
    if (this.GetChildMenu() != null)
      return;
    this.drawMouse(b);
    if (this.hoverText.Length <= 0)
      return;
    IClickableMenu.drawHoverText(b, this.hoverText, Game1.dialogueFont);
  }

  /// <summary>Deduct the money and materials from the player's inventory to construct the current blueprint.</summary>
  public void ConsumeResources()
  {
    CarpenterMenu.BlueprintEntry blueprint = this.Blueprint;
    foreach (Item ingredient in this.ingredients)
      Game1.player.Items.ReduceId(ingredient.QualifiedItemId, ingredient.Stack);
    Game1.player.Money -= blueprint.BuildCost;
  }

  /// <summary>Get whether the player has the money and materials needed to construct the current blueprint.</summary>
  public bool DoesFarmerHaveEnoughResourcesToBuild()
  {
    CarpenterMenu.BlueprintEntry blueprint = this.Blueprint;
    if (blueprint.BuildCost < 0)
      return false;
    foreach (Item ingredient in this.ingredients)
    {
      if (!Game1.player.Items.ContainsId(ingredient.QualifiedItemId, ingredient.Stack))
        return false;
    }
    return Game1.player.Money >= blueprint.BuildCost;
  }

  /// <summary>A building action that can be performed through the carpenter menu.</summary>
  public enum CarpentryAction
  {
    /// <summary>The player hasn't selected an action to perform yet.</summary>
    None,
    /// <summary>The player is demolishing buildings.</summary>
    Demolish,
    /// <summary>The player is moving buildings.</summary>
    Move,
    /// <summary>The player is painting buildings.</summary>
    Paint,
    /// <summary>The player is upgrading buildings.</summary>
    Upgrade,
  }

  /// <summary>Metadata for a building shown in the construction menu.</summary>
  public class BlueprintEntry
  {
    /// <summary>The index of the blueprint in the construction menu's list.</summary>
    public int Index { get; }

    /// <summary>The building type ID in <c>Data/Buildings</c>.</summary>
    public string Id { get; }

    /// <summary>The building data from <c>Data/Buildings</c>.</summary>
    public BuildingData Data { get; }

    /// <summary>The building skin to apply from <c>Data/Buildings</c>, if applicable.</summary>
    public BuildingSkin Skin { get; private set; }

    /// <summary>The translated display name.</summary>
    public string DisplayName { get; private set; }

    /// <summary>The translated display name for the general building type, like 'Coop' for a Deluxe Coop.</summary>
    public string DisplayNameForGeneralType { get; private set; }

    /// <summary>The <see cref="P:StardewValley.Menus.CarpenterMenu.BlueprintEntry.DisplayName" /> as a <see cref="T:StardewValley.TokenizableStrings.TokenParser">tokenizable string</see>.</summary>
    public string TokenizedDisplayName { get; private set; }

    /// <summary>The <see cref="P:StardewValley.Menus.CarpenterMenu.BlueprintEntry.DisplayNameForGeneralType" /> as a <see cref="T:StardewValley.TokenizableStrings.TokenParser">tokenizable string</see>.</summary>
    public string TokenizedDisplayNameForGeneralType { get; private set; }

    /// <summary>The translated description.</summary>
    public string Description { get; private set; }

    /// <summary>The number of tiles horizontally for the constructed building's collision box.</summary>
    public int TilesWide { get; }

    /// <summary>The number of tiles vertically for the constructed building's collision box.</summary>
    public int TilesHigh { get; }

    /// <inheritdoc cref="F:StardewValley.GameData.Buildings.BuildingData.BuildingToUpgrade" />
    public bool IsUpgrade
    {
      get
      {
        string buildingToUpgrade = this.Data.BuildingToUpgrade;
        return buildingToUpgrade != null && buildingToUpgrade.Length > 0;
      }
    }

    /// <inheritdoc cref="F:StardewValley.GameData.Buildings.BuildingData.BuildDays" />
    public int BuildDays => (int?) this.Skin?.BuildDays ?? this.Data.BuildDays;

    /// <inheritdoc cref="F:StardewValley.GameData.Buildings.BuildingData.BuildCost" />
    public int BuildCost => (int?) this.Skin?.BuildCost ?? this.Data.BuildCost;

    /// <inheritdoc cref="F:StardewValley.GameData.Buildings.BuildingData.BuildMaterials" />
    public List<BuildingMaterial> BuildMaterials
    {
      get => this.Skin?.BuildMaterials ?? this.Data.BuildMaterials;
    }

    /// <inheritdoc cref="F:StardewValley.GameData.Buildings.BuildingData.BuildingToUpgrade" />
    public string UpgradeFrom => this.Data.BuildingToUpgrade;

    /// <inheritdoc cref="F:StardewValley.GameData.Buildings.BuildingData.MagicalConstruction" />
    public bool MagicalConstruction => this.Data.MagicalConstruction;

    /// <summary>Construct an instance.</summary>
    /// <param name="index">The index of the blueprint in the construction menu's list.</param>
    /// <param name="id">The building type ID in <c>Data/Buildings</c>.</param>
    /// <param name="data">The building data from <c>Data/Buildings</c>.</param>
    /// <param name="skinId">The building skin ID, if applicable.</param>
    public BlueprintEntry(int index, string id, BuildingData data, string skinId)
    {
      this.Index = index;
      this.Id = id;
      this.Data = data;
      this.TilesWide = data.Size.X;
      this.TilesHigh = data.Size.Y;
      this.SetSkin(skinId);
    }

    /// <summary>Set the selected building skin.</summary>
    /// <param name="id">The skin ID.</param>
    public void SetSkin(string id)
    {
      if (this.Data.Skins != null)
      {
        foreach (BuildingSkin skin in this.Data.Skins)
        {
          if (skin.Id == id)
          {
            this.Skin = skin;
            this.TokenizedDisplayName = skin.Name ?? this.Data.Name;
            this.TokenizedDisplayNameForGeneralType = skin.NameForGeneralType;
            this.DisplayName = TokenParser.ParseText(this.TokenizedDisplayName);
            this.DisplayNameForGeneralType = TokenParser.ParseText(this.TokenizedDisplayNameForGeneralType) ?? this.DisplayName;
            this.Description = TokenParser.ParseText(skin.Description) ?? TokenParser.ParseText(this.Data.Description);
            return;
          }
        }
      }
      this.Skin = (BuildingSkin) null;
      this.TokenizedDisplayName = this.Data.Name;
      this.TokenizedDisplayNameForGeneralType = this.Data.NameForGeneralType;
      this.DisplayName = TokenParser.ParseText(this.TokenizedDisplayName);
      this.DisplayNameForGeneralType = TokenParser.ParseText(this.TokenizedDisplayNameForGeneralType) ?? this.DisplayName;
      this.Description = TokenParser.ParseText(this.Data.Description);
    }

    /// <summary>Get the display name for the building this upgrades from, if applicable.</summary>
    public string GetDisplayNameForBuildingToUpgrade()
    {
      BuildingData buildingData;
      return !this.IsUpgrade || !Game1.buildingData.TryGetValue(this.Data.BuildingToUpgrade, out buildingData) ? (string) null : TokenParser.ParseText(buildingData.Name);
    }
  }
}
