// Decompiled with JetBrains decompiler
// Type: StardewValley.Menus.TailorRecipeListTool
// Assembly: Stardew Valley, Version=1.6.15.24356, Culture=neutral, PublicKeyToken=null
// MVID: AA1F513A-94F0-4EF6-A35F-2D5B4A3721BF
// Assembly location: D:\SteamLibrary\steamapps\common\Stardew Valley\Stardew Valley.dll
// XML documentation location: D:\SteamLibrary\steamapps\common\Stardew Valley\Stardew Valley.xml

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using StardewValley.GameData.Crafting;
using StardewValley.Objects;
using System;
using System.Collections.Generic;

#nullable disable
namespace StardewValley.Menus;

public class TailorRecipeListTool : IClickableMenu
{
  public Rectangle scrollView;
  public List<ClickableTextureComponent> recipeComponents = new List<ClickableTextureComponent>();
  public ClickableTextureComponent okButton;
  public float scrollY;
  public Dictionary<string, KeyValuePair<Item, Item>> _recipeLookup = new Dictionary<string, KeyValuePair<Item, Item>>();
  public Item hoveredItem;
  public string hoverText = "";
  public Dictionary<string, string> _recipeHoverTexts = new Dictionary<string, string>();
  public Dictionary<string, string> _recipeOutputIds = new Dictionary<string, string>();
  public Dictionary<string, Color> _recipeColors = new Dictionary<string, Color>();

  public TailorRecipeListTool()
    : base(Game1.uiViewport.Width / 2 - (632 + IClickableMenu.borderWidth * 2) / 2, Game1.uiViewport.Height / 2 - (600 + IClickableMenu.borderWidth * 2) / 2 - 64 /*0x40*/, 632 + IClickableMenu.borderWidth * 2, 600 + IClickableMenu.borderWidth * 2 + 64 /*0x40*/)
  {
    TailoringMenu tailoringMenu = new TailoringMenu();
    Game1.player.faceDirection(2);
    Game1.player.FarmerSprite.StopAnimation();
    Item obj1 = (Item) ItemRegistry.Create<StardewValley.Object>("(O)428");
    foreach (string allId in ItemRegistry.GetObjectTypeDefinition().GetAllIds())
    {
      StardewValley.Object @object = new StardewValley.Object(allId, 1);
      if (!@object.Name.Contains("Seeds") && !@object.Name.Contains("Floor") && !@object.Name.Equals("Lumber") && !@object.Name.Contains("Fence") && !@object.Name.Equals("Gate") && !@object.Name.Contains("Starter") && !@object.Name.Equals("Secret Note") && !@object.Name.Contains("Guide") && !@object.Name.Contains("Path") && !@object.Name.Contains("Ring") && @object.category.Value != -22 && @object.Category != -999 && !@object.isSapling())
      {
        Item obj2 = tailoringMenu.CraftItem(obj1, (Item) @object);
        TailorItemRecipe recipeForItems = tailoringMenu.GetRecipeForItems(obj1, (Item) @object);
        KeyValuePair<Item, Item> keyValuePair = new KeyValuePair<Item, Item>((Item) @object, obj2);
        this._recipeLookup[Utility.getStandardDescriptionFromItem((Item) @object, 1)] = keyValuePair;
        string str = "";
        Color? dyeColor = TailoringMenu.GetDyeColor((Item) @object);
        if (dyeColor.HasValue)
          this._recipeColors[Utility.getStandardDescriptionFromItem((Item) @object, 1)] = dyeColor.Value;
        if (recipeForItems != null)
        {
          str = $"clothes id: {recipeForItems.CraftedItemId} from ";
          foreach (string secondItemTag in recipeForItems.SecondItemTags)
            str = $"{str}{secondItemTag} ";
          str.Trim();
        }
        this._recipeOutputIds[Utility.getStandardDescriptionFromItem((Item) @object, 1)] = TailoringMenu.ConvertLegacyItemId(recipeForItems?.CraftedItemId) ?? obj2.QualifiedItemId;
        this._recipeHoverTexts[Utility.getStandardDescriptionFromItem((Item) @object, 1)] = str;
        ClickableTextureComponent textureComponent = new ClickableTextureComponent(new Rectangle(0, 0, 64 /*0x40*/, 64 /*0x40*/), (Texture2D) null, new Rectangle(), 1f);
        textureComponent.myID = 0;
        textureComponent.name = Utility.getStandardDescriptionFromItem((Item) @object, 1);
        textureComponent.label = @object.DisplayName;
        this.recipeComponents.Add(textureComponent);
      }
    }
    ClickableTextureComponent textureComponent1 = new ClickableTextureComponent("OK", new Rectangle(this.xPositionOnScreen + this.width - IClickableMenu.borderWidth - IClickableMenu.spaceToClearSideBorder - 64 /*0x40*/, this.yPositionOnScreen + this.height - IClickableMenu.borderWidth - IClickableMenu.spaceToClearTopBorder + 16 /*0x10*/, 64 /*0x40*/, 64 /*0x40*/), (string) null, (string) null, Game1.mouseCursors, Game1.getSourceRectForStandardTileSheet(Game1.mouseCursors, 46), 1f);
    textureComponent1.upNeighborID = -99998;
    textureComponent1.leftNeighborID = -99998;
    textureComponent1.rightNeighborID = -99998;
    textureComponent1.downNeighborID = -99998;
    this.okButton = textureComponent1;
    this.RepositionElements();
  }

  /// <inheritdoc />
  public override void gameWindowSizeChanged(Rectangle oldBounds, Rectangle newBounds)
  {
    base.gameWindowSizeChanged(oldBounds, newBounds);
    this.xPositionOnScreen = Game1.uiViewport.Width / 2 - (632 + IClickableMenu.borderWidth * 2) / 2;
    this.yPositionOnScreen = Game1.uiViewport.Height / 2 - (600 + IClickableMenu.borderWidth * 2) / 2 - 64 /*0x40*/;
    this.RepositionElements();
  }

  private void RepositionElements()
  {
    this.scrollView = new Rectangle(this.xPositionOnScreen + IClickableMenu.borderWidth, this.yPositionOnScreen + IClickableMenu.borderWidth + IClickableMenu.spaceToClearTopBorder, this.width - IClickableMenu.borderWidth, 500);
    if (this.scrollView.Left < Game1.graphics.GraphicsDevice.ScissorRectangle.Left)
    {
      int num = Game1.graphics.GraphicsDevice.ScissorRectangle.Left - this.scrollView.Left;
      this.scrollView.X += num;
      this.scrollView.Width -= num;
    }
    if (this.scrollView.Right > Game1.graphics.GraphicsDevice.ScissorRectangle.Right)
    {
      int num = this.scrollView.Right - Game1.graphics.GraphicsDevice.ScissorRectangle.Right;
      this.scrollView.X -= num;
      this.scrollView.Width -= num;
    }
    if (this.scrollView.Top < Game1.graphics.GraphicsDevice.ScissorRectangle.Top)
    {
      int num = Game1.graphics.GraphicsDevice.ScissorRectangle.Top - this.scrollView.Top;
      this.scrollView.Y += num;
      this.scrollView.Width -= num;
    }
    if (this.scrollView.Bottom > Game1.graphics.GraphicsDevice.ScissorRectangle.Bottom)
    {
      int num = this.scrollView.Bottom - Game1.graphics.GraphicsDevice.ScissorRectangle.Bottom;
      this.scrollView.Y -= num;
      this.scrollView.Width -= num;
    }
    this.RepositionScrollElements();
  }

  public void RepositionScrollElements()
  {
    int scrollY = (int) this.scrollY;
    if ((double) this.scrollY > 0.0)
      this.scrollY = 0.0f;
    foreach (ClickableTextureComponent recipeComponent in this.recipeComponents)
    {
      recipeComponent.bounds.X = this.scrollView.X;
      recipeComponent.bounds.Y = this.scrollView.Y + scrollY;
      scrollY += recipeComponent.bounds.Height;
      if (this.scrollView.Intersects(recipeComponent.bounds))
        recipeComponent.visible = true;
      else
        recipeComponent.visible = false;
    }
  }

  public override void snapToDefaultClickableComponent()
  {
    this.snapCursorToCurrentSnappedComponent();
  }

  /// <inheritdoc />
  public override void receiveLeftClick(int x, int y, bool playSound = true)
  {
    foreach (ClickableTextureComponent recipeComponent in this.recipeComponents)
    {
      if (recipeComponent.bounds.Contains(x, y))
      {
        if (this.scrollView.Contains(x, y))
        {
          try
          {
            Item obj = ItemRegistry.Create(this._recipeOutputIds[recipeComponent.name]);
            Color color;
            if (obj is Clothing clothing && this._recipeColors.TryGetValue(recipeComponent.name, out color))
              clothing.Dye(color, 1f);
            Game1.player.addItemToInventoryBool(obj);
          }
          catch (Exception ex)
          {
          }
        }
      }
    }
    if (!this.okButton.containsPoint(x, y))
      return;
    this.exitThisMenu();
  }

  /// <inheritdoc />
  public override void receiveKeyPress(Keys key)
  {
  }

  /// <inheritdoc />
  public override void receiveScrollWheelAction(int direction)
  {
    this.scrollY += (float) direction;
    this.RepositionScrollElements();
    base.receiveScrollWheelAction(direction);
  }

  /// <inheritdoc />
  public override void performHoverAction(int x, int y)
  {
    this.hoveredItem = (Item) null;
    this.hoverText = "";
    foreach (ClickableTextureComponent recipeComponent in this.recipeComponents)
    {
      if (recipeComponent.containsPoint(x, y))
      {
        this.hoveredItem = this._recipeLookup[recipeComponent.name].Value;
        this.hoverText = this._recipeHoverTexts[recipeComponent.name];
      }
    }
  }

  public bool canLeaveMenu() => true;

  /// <inheritdoc />
  public override void draw(SpriteBatch b)
  {
    Game1.drawDialogueBox(this.xPositionOnScreen, this.yPositionOnScreen, this.width, this.height, false, true);
    b.End();
    Rectangle scissorRectangle = b.GraphicsDevice.ScissorRectangle;
    b.Begin(blendState: BlendState.AlphaBlend, samplerState: SamplerState.PointClamp, rasterizerState: Utility.ScissorEnabled);
    b.GraphicsDevice.ScissorRectangle = this.scrollView;
    foreach (ClickableTextureComponent recipeComponent in this.recipeComponents)
    {
      if (recipeComponent.visible)
      {
        this.drawHorizontalPartition(b, recipeComponent.bounds.Bottom - 32 /*0x20*/, true);
        KeyValuePair<Item, Item> keyValuePair = this._recipeLookup[recipeComponent.name];
        recipeComponent.draw(b);
        keyValuePair.Key.drawInMenu(b, new Vector2((float) recipeComponent.bounds.X, (float) recipeComponent.bounds.Y), 1f);
        Color color;
        if (this._recipeColors.TryGetValue(recipeComponent.name, out color))
        {
          int num = 24;
          b.Draw(Game1.staminaRect, new Rectangle(this.scrollView.Left + this.scrollView.Width / 2 - num / 2, recipeComponent.bounds.Center.Y - num / 2, num, num), color);
        }
        keyValuePair.Value?.drawInMenu(b, new Vector2((float) (this.scrollView.Left + this.scrollView.Width - 128 /*0x80*/), (float) recipeComponent.bounds.Y), 1f);
      }
    }
    b.End();
    b.GraphicsDevice.ScissorRectangle = scissorRectangle;
    b.Begin(blendState: BlendState.AlphaBlend, samplerState: SamplerState.PointClamp);
    this.okButton.draw(b);
    this.drawMouse(b);
    if (this.hoveredItem == null)
      return;
    Utility.drawTextWithShadow(b, this.hoverText, Game1.smallFont, new Vector2((float) (this.xPositionOnScreen + IClickableMenu.borderWidth), (float) (this.yPositionOnScreen + this.height - 64 /*0x40*/)), Color.Black);
    if (Game1.oldKBState.IsKeyDown(Keys.LeftShift))
      return;
    IClickableMenu.drawToolTip(b, this.hoveredItem.getDescription(), this.hoveredItem.DisplayName, this.hoveredItem);
  }
}
