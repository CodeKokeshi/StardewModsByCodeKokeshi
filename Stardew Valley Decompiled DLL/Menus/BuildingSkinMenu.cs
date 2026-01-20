// Decompiled with JetBrains decompiler
// Type: StardewValley.Menus.BuildingSkinMenu
// Assembly: Stardew Valley, Version=1.6.15.24356, Culture=neutral, PublicKeyToken=null
// MVID: AA1F513A-94F0-4EF6-A35F-2D5B4A3721BF
// Assembly location: D:\SteamLibrary\steamapps\common\Stardew Valley\Stardew Valley.dll
// XML documentation location: D:\SteamLibrary\steamapps\common\Stardew Valley\Stardew Valley.xml

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using StardewValley.BellsAndWhistles;
using StardewValley.Buildings;
using StardewValley.GameData.Buildings;
using StardewValley.TokenizableStrings;
using System;
using System.Collections.Generic;

#nullable disable
namespace StardewValley.Menus;

public class BuildingSkinMenu : IClickableMenu
{
  public const int region_okButton = 101;
  public const int region_nextSkin = 102;
  public const int region_prevSkin = 103;
  public static int WindowWidth = 576;
  public static int WindowHeight = 576;
  public Rectangle PreviewPane;
  public ClickableTextureComponent OkButton;
  /// <summary>The building whose skin to change.</summary>
  public Building Building;
  public ClickableTextureComponent NextSkinButton;
  public ClickableTextureComponent PreviousSkinButton;
  public string BuildingDisplayName;
  public string BuildingDescription;
  /// <summary>The building skins available in the menu.</summary>
  public List<BuildingSkinMenu.SkinEntry> Skins = new List<BuildingSkinMenu.SkinEntry>();
  /// <summary>The current building skin shown in the menu.</summary>
  public BuildingSkinMenu.SkinEntry Skin;

  /// <summary>Construct an instance.</summary>
  /// <param name="targetBuilding">The building whose skin to change.</param>
  /// <param name="ignoreSeparateConstructionEntries">Whether to ignore skins with <see cref="F:StardewValley.GameData.Buildings.BuildingSkin.ShowAsSeparateConstructionEntry" /> set to true.</param>
  public BuildingSkinMenu(Building targetBuilding, bool ignoreSeparateConstructionEntries = false)
    : base(Game1.uiViewport.Width / 2 - BuildingSkinMenu.WindowWidth / 2, Game1.uiViewport.Height / 2 - BuildingSkinMenu.WindowHeight / 2, BuildingSkinMenu.WindowWidth, BuildingSkinMenu.WindowHeight)
  {
    Game1.player.Halt();
    this.Building = targetBuilding;
    BuildingData data = targetBuilding.GetData();
    this.BuildingDisplayName = TokenParser.ParseText(data.Name);
    this.BuildingDescription = TokenParser.ParseText(data.Description);
    int num1 = 0;
    List<BuildingSkinMenu.SkinEntry> skins = this.Skins;
    int index = num1;
    int num2 = index + 1;
    BuildingSkinMenu.SkinEntry skinEntry = new BuildingSkinMenu.SkinEntry(index, (BuildingSkin) null, this.BuildingDisplayName, this.BuildingDescription);
    skins.Add(skinEntry);
    if (data.Skins != null)
    {
      foreach (BuildingSkin skin in data.Skins)
      {
        if (!(skin.Id != this.Building.skinId.Value) || (!ignoreSeparateConstructionEntries || !skin.ShowAsSeparateConstructionEntry) && GameStateQuery.CheckConditions(skin.Condition, this.Building.GetParentLocation()))
          this.Skins.Add(new BuildingSkinMenu.SkinEntry(num2++, skin));
      }
    }
    this.RepositionElements();
    this.SetSkin(Math.Max(this.Skins.FindIndex((Predicate<BuildingSkinMenu.SkinEntry>) (skin => skin.Id == this.Building.skinId.Value)), 0));
    this.populateClickableComponentList();
    if (!Game1.options.SnappyMenus)
      return;
    this.snapToDefaultClickableComponent();
  }

  public override void snapToDefaultClickableComponent()
  {
    this.currentlySnappedComponent = this.getComponentWithID(101);
    this.snapCursorToCurrentSnappedComponent();
  }

  /// <inheritdoc />
  public override void receiveGamePadButton(Buttons button)
  {
    switch (button)
    {
      case Buttons.RightTrigger:
        Game1.playSound("shwip");
        this.SetSkin(this.Skin.Index + 1);
        break;
      case Buttons.LeftTrigger:
        Game1.playSound("shwip");
        this.SetSkin(this.Skin.Index - 1);
        break;
    }
    base.receiveGamePadButton(button);
  }

  /// <inheritdoc />
  public override void receiveLeftClick(int x, int y, bool playSound = true)
  {
    if (this.OkButton.containsPoint(x, y))
      this.exitThisMenu(playSound);
    else if (this.PreviousSkinButton.containsPoint(x, y))
    {
      Game1.playSound("shwip");
      this.SetSkin(this.Skin.Index - 1);
    }
    else if (this.NextSkinButton.containsPoint(x, y))
    {
      this.SetSkin(this.Skin.Index + 1);
      Game1.playSound("shwip");
    }
    else
      base.receiveLeftClick(x, y, playSound);
  }

  public void SetSkin(int index)
  {
    if (this.Skins.Count == 0)
    {
      this.SetSkin((BuildingSkinMenu.SkinEntry) null);
    }
    else
    {
      index %= this.Skins.Count;
      if (index < 0)
        index = this.Skins.Count + index;
      this.SetSkin(this.Skins[index]);
    }
  }

  public virtual void SetSkin(BuildingSkinMenu.SkinEntry skin)
  {
    this.Skin = skin;
    if (!(this.Building.skinId.Value != skin.Id))
      return;
    this.Building.skinId.Value = skin.Id;
    this.Building.netBuildingPaintColor.Value.Color1Default.Value = true;
    this.Building.netBuildingPaintColor.Value.Color2Default.Value = true;
    this.Building.netBuildingPaintColor.Value.Color3Default.Value = true;
    BuildingData data = this.Building.GetData();
    if (data == null || this.Building.daysOfConstructionLeft.Value != data.BuildDays)
      return;
    this.Building.daysOfConstructionLeft.Value = (int?) skin.Data?.BuildDays ?? data.BuildDays;
  }

  /// <inheritdoc />
  public override void performHoverAction(int x, int y)
  {
    this.OkButton.tryHover(x, y);
    this.PreviousSkinButton.tryHover(x, y);
    this.NextSkinButton.tryHover(x, y);
  }

  public virtual void RepositionElements()
  {
    this.PreviewPane.Y = this.yPositionOnScreen + 48 /*0x30*/;
    this.PreviewPane.Width = 576;
    this.PreviewPane.Height = 576;
    this.PreviewPane.X = this.xPositionOnScreen + this.width / 2 - this.PreviewPane.Width / 2;
    Rectangle previewPane = this.PreviewPane;
    previewPane.Inflate(-16, -16);
    ClickableTextureComponent textureComponent1 = new ClickableTextureComponent(new Rectangle(previewPane.Left, previewPane.Center.Y - 32 /*0x20*/, 64 /*0x40*/, 64 /*0x40*/), Game1.mouseCursors, Game1.getSourceRectForStandardTileSheet(Game1.mouseCursors, 44), 1f);
    textureComponent1.myID = 103;
    textureComponent1.leftNeighborID = -99998;
    textureComponent1.rightNeighborID = -99998;
    textureComponent1.downNeighborID = 101;
    textureComponent1.upNeighborID = -99998;
    textureComponent1.fullyImmutable = true;
    this.PreviousSkinButton = textureComponent1;
    ClickableTextureComponent textureComponent2 = new ClickableTextureComponent(new Rectangle(previewPane.Right - 64 /*0x40*/, previewPane.Center.Y - 32 /*0x20*/, 64 /*0x40*/, 64 /*0x40*/), Game1.mouseCursors, Game1.getSourceRectForStandardTileSheet(Game1.mouseCursors, 33), 1f);
    textureComponent2.myID = 102;
    textureComponent2.leftNeighborID = -99998;
    textureComponent2.rightNeighborID = -99998;
    textureComponent2.downNeighborID = 101;
    textureComponent2.upNeighborID = -99998;
    textureComponent2.fullyImmutable = true;
    this.NextSkinButton = textureComponent2;
    previewPane.Y += 64 /*0x40*/;
    previewPane.Height = 0;
    previewPane.Y += 80 /*0x50*/;
    previewPane.Y += 64 /*0x40*/;
    ClickableTextureComponent textureComponent3 = new ClickableTextureComponent(new Rectangle(this.PreviewPane.Right - 64 /*0x40*/ - 16 /*0x10*/, this.PreviewPane.Bottom - 64 /*0x40*/ - 16 /*0x10*/, 64 /*0x40*/, 64 /*0x40*/), Game1.mouseCursors, Game1.getSourceRectForStandardTileSheet(Game1.mouseCursors, 46), 1f);
    textureComponent3.myID = 101;
    textureComponent3.upNeighborID = 102;
    this.OkButton = textureComponent3;
    if (this.Skins.Count == 0)
    {
      this.NextSkinButton.visible = false;
      this.PreviousSkinButton.visible = false;
    }
    this.populateClickableComponentList();
  }

  public virtual bool SaveColor() => true;

  public virtual void SetRegion(int newRegion) => this.RepositionElements();

  /// <inheritdoc />
  public override void draw(SpriteBatch b)
  {
    if (!Game1.options.showClearBackgrounds)
      b.Draw(Game1.fadeToBlackRect, Game1.graphics.GraphicsDevice.Viewport.Bounds, Color.Black * 0.75f);
    Game1.DrawBox(this.PreviewPane.X, this.PreviewPane.Y, this.PreviewPane.Width, this.PreviewPane.Height);
    Rectangle previewPane = this.PreviewPane;
    previewPane.Inflate(0, 0);
    b.End();
    b.Begin(SpriteSortMode.FrontToBack, BlendState.AlphaBlend, SamplerState.PointClamp, rasterizerState: Utility.ScissorEnabled);
    b.GraphicsDevice.ScissorRectangle = previewPane;
    Vector2 vector2 = new Vector2((float) (this.PreviewPane.X + this.PreviewPane.Width / 2), (float) (this.PreviewPane.Y + this.PreviewPane.Height / 2 - 16 /*0x10*/));
    Rectangle rectangle = this.Building.getSourceRectForMenu() ?? this.Building.getSourceRect();
    this.Building?.drawInMenu(b, (int) vector2.X - (int) ((double) this.Building.tilesWide.Value / 2.0 * 64.0), (int) vector2.Y - rectangle.Height * 4 / 2);
    b.End();
    b.Begin(blendState: BlendState.AlphaBlend, samplerState: SamplerState.PointClamp);
    SpriteText.drawStringWithScrollCenteredAt(b, Game1.content.LoadString("Strings\\Buildings:BuildingSkinMenu_ChooseAppearance", (object) this.BuildingDisplayName), this.xPositionOnScreen + this.width / 2, this.PreviewPane.Top - 96 /*0x60*/);
    this.OkButton.draw(b);
    this.NextSkinButton.draw(b);
    this.PreviousSkinButton.draw(b);
    this.drawMouse(b);
  }

  /// <summary>Metadata for a skin shown in the menu.</summary>
  public class SkinEntry
  {
    /// <summary>The index of the skin in the menu's list.</summary>
    public int Index;
    /// <summary>The skin ID in <c>Data/Buildings</c>.</summary>
    public readonly string Id;
    /// <summary>The translated display name.</summary>
    public readonly string DisplayName;
    /// <summary>The translated description.</summary>
    public readonly string Description;
    /// <summary>The skin data from <c>Data/Buildings</c>.</summary>
    public readonly BuildingSkin Data;

    /// <summary>Construct an instance.</summary>
    /// <param name="index">The index of the skin in the menu's list.</param>
    /// <param name="skin">The skin ID in <c>Data/Buildings</c>.</param>
    public SkinEntry(int index, BuildingSkin skin)
      : this(index, skin, TokenParser.ParseText(skin.Name), TokenParser.ParseText(skin.Description))
    {
    }

    /// <summary>Construct an instance.</summary>
    /// <param name="index">The index of the skin in the menu's list.</param>
    /// <param name="skin">The skin data from <c>Data/Buildings</c>.</param>
    /// <param name="displayName">The translated display name.</param>
    /// <param name="description">The translated description.</param>
    public SkinEntry(int index, BuildingSkin skin, string displayName, string description)
    {
      this.Index = index;
      this.Id = skin?.Id;
      this.Data = skin;
      this.DisplayName = displayName;
      this.Description = description;
    }
  }
}
