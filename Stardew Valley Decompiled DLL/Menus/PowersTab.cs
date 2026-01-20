// Decompiled with JetBrains decompiler
// Type: StardewValley.Menus.PowersTab
// Assembly: Stardew Valley, Version=1.6.15.24356, Culture=neutral, PublicKeyToken=null
// MVID: AA1F513A-94F0-4EF6-A35F-2D5B4A3721BF
// Assembly location: D:\SteamLibrary\steamapps\common\Stardew Valley\Stardew Valley.dll
// XML documentation location: D:\SteamLibrary\steamapps\common\Stardew Valley\Stardew Valley.xml

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using StardewValley.GameData.Powers;
using StardewValley.TokenizableStrings;
using System;
using System.Collections.Generic;
using System.Linq;

#nullable disable
namespace StardewValley.Menus;

public class PowersTab : IClickableMenu
{
  public const int region_forwardButton = 707;
  public const int region_backButton = 706;
  public const int distanceFromMenuBottomBeforeNewPage = 128 /*0x80*/;
  public int currentPage;
  public string descriptionText = "";
  public string hoverText = "";
  public ClickableTextureComponent backButton;
  public ClickableTextureComponent forwardButton;
  public List<List<ClickableTextureComponent>> powers;

  public PowersTab(int x, int y, int width, int height)
    : base(x, y, width, height)
  {
    ClickableTextureComponent textureComponent1 = new ClickableTextureComponent(new Rectangle(this.xPositionOnScreen + 48 /*0x30*/, this.yPositionOnScreen + height - 80 /*0x50*/, 48 /*0x30*/, 44), Game1.mouseCursors, new Rectangle(352, 495, 12, 11), 4f);
    textureComponent1.myID = 706;
    textureComponent1.rightNeighborID = -7777;
    this.backButton = textureComponent1;
    ClickableTextureComponent textureComponent2 = new ClickableTextureComponent(new Rectangle(this.xPositionOnScreen + width - 32 /*0x20*/ - 60, this.yPositionOnScreen + height - 80 /*0x50*/, 48 /*0x30*/, 44), Game1.mouseCursors, new Rectangle(365, 495, 12, 11), 4f);
    textureComponent2.myID = 707;
    textureComponent2.leftNeighborID = -7777;
    this.forwardButton = textureComponent2;
  }

  public override void snapToDefaultClickableComponent()
  {
    base.snapToDefaultClickableComponent();
    this.currentlySnappedComponent = this.getComponentWithID(0);
    this.snapCursorToCurrentSnappedComponent();
  }

  public override void populateClickableComponentList()
  {
    if (this.powers == null)
    {
      this.powers = new List<List<ClickableTextureComponent>>();
      Dictionary<string, PowersData> dictionary = (Dictionary<string, PowersData>) null;
      try
      {
        dictionary = DataLoader.Powers(Game1.content);
      }
      catch (Exception ex)
      {
      }
      if (dictionary != null)
      {
        int num1 = 9;
        int num2 = 0;
        int num3 = this.xPositionOnScreen + IClickableMenu.borderWidth + IClickableMenu.spaceToClearSideBorder;
        int num4 = this.yPositionOnScreen + IClickableMenu.borderWidth + IClickableMenu.spaceToClearTopBorder - 16 /*0x10*/;
        foreach (KeyValuePair<string, PowersData> keyValuePair in dictionary)
        {
          int x = num3 + num2 % num1 * 76;
          int y = num4 + num2 / num1 * 76;
          bool drawShadow = GameStateQuery.CheckConditions(keyValuePair.Value.UnlockedCondition);
          string label = TokenParser.ParseText(keyValuePair.Value.DisplayName) ?? keyValuePair.Key;
          string hoverText = TokenParser.ParseText(keyValuePair.Value.Description) ?? "";
          Texture2D texture = Game1.content.Load<Texture2D>(keyValuePair.Value.TexturePath);
          if (this.powers.Count == 0 || y > this.yPositionOnScreen + this.height - 128 /*0x80*/)
          {
            this.powers.Add(new List<ClickableTextureComponent>());
            num2 = 0;
            x = num3;
            y = num4;
          }
          List<ClickableTextureComponent> textureComponentList1 = this.powers.Last<List<ClickableTextureComponent>>();
          List<ClickableTextureComponent> textureComponentList2 = textureComponentList1;
          ClickableTextureComponent textureComponent = new ClickableTextureComponent(keyValuePair.Key, new Rectangle(x, y, 64 /*0x40*/, 64 /*0x40*/), label, hoverText, texture, new Rectangle(keyValuePair.Value.TexturePosition.X, keyValuePair.Value.TexturePosition.Y, 16 /*0x10*/, 16 /*0x10*/), 4f, drawShadow);
          textureComponent.myID = textureComponentList1.Count;
          textureComponent.rightNeighborID = (textureComponentList1.Count + 1) % num1 == 0 ? -1 : textureComponentList1.Count + 1;
          textureComponent.leftNeighborID = textureComponentList1.Count % num1 == 0 ? -1 : textureComponentList1.Count - 1;
          textureComponent.downNeighborID = y + 76 > this.yPositionOnScreen + this.height - 128 /*0x80*/ ? -7777 : textureComponentList1.Count + num1;
          textureComponent.upNeighborID = textureComponentList1.Count < num1 ? 12346 : textureComponentList1.Count - num1;
          textureComponent.fullyImmutable = true;
          textureComponent.drawLabel = false;
          textureComponentList2.Add(textureComponent);
          ++num2;
        }
      }
    }
    base.populateClickableComponentList();
  }

  /// <inheritdoc />
  public override void performHoverAction(int x, int y)
  {
    this.hoverText = "";
    this.descriptionText = "";
    base.performHoverAction(x, y);
    foreach (ClickableTextureComponent textureComponent in this.powers[this.currentPage])
    {
      if (textureComponent.containsPoint(x, y))
      {
        textureComponent.scale = Math.Min(textureComponent.scale + 0.02f, textureComponent.baseScale + 0.1f);
        this.hoverText = textureComponent.drawShadow ? textureComponent.label : "???";
        this.descriptionText = Game1.parseText(textureComponent.hoverText, Game1.smallFont, Math.Max((int) Game1.dialogueFont.MeasureString(this.hoverText).X, 320));
      }
      else
        textureComponent.scale = Math.Max(textureComponent.scale - 0.02f, textureComponent.baseScale);
    }
    this.forwardButton.tryHover(x, y, 0.5f);
    this.backButton.tryHover(x, y, 0.5f);
  }

  /// <inheritdoc />
  public override void receiveLeftClick(int x, int y, bool playSound = true)
  {
    if (this.backButton.containsPoint(x, y) && this.currentPage > 0)
    {
      if (playSound)
        Game1.playSound("shwip");
      --this.currentPage;
    }
    else if (this.forwardButton.containsPoint(x, y) && this.currentPage < this.powers.Count - 1)
    {
      if (playSound)
        Game1.playSound("shwip");
      ++this.currentPage;
    }
    else
      base.receiveLeftClick(x, y, playSound);
  }

  /// <inheritdoc />
  public override void draw(SpriteBatch b)
  {
    if (this.currentPage > 0)
      this.backButton.draw(b);
    if (this.currentPage < this.powers.Count - 1)
      this.forwardButton.draw(b);
    b.End();
    b.Begin(SpriteSortMode.FrontToBack, BlendState.AlphaBlend, SamplerState.PointClamp);
    foreach (ClickableTextureComponent textureComponent in this.powers[this.currentPage])
    {
      bool drawShadow = textureComponent.drawShadow;
      textureComponent.draw(b, drawShadow ? Color.White : Color.Black * 0.2f, 0.86f);
    }
    b.End();
    b.Begin(blendState: BlendState.AlphaBlend, samplerState: SamplerState.PointClamp);
    if (!this.descriptionText.Equals("") && this.hoverText != "???")
    {
      IClickableMenu.drawHoverText(b, this.descriptionText, Game1.smallFont, boldTitleText: this.hoverText);
    }
    else
    {
      if (this.hoverText.Equals(""))
        return;
      IClickableMenu.drawHoverText(b, this.hoverText, Game1.smallFont);
    }
  }
}
