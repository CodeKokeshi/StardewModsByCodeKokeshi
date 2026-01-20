// Decompiled with JetBrains decompiler
// Type: StardewValley.Menus.Toolbar
// Assembly: Stardew Valley, Version=1.6.15.24356, Culture=neutral, PublicKeyToken=null
// MVID: AA1F513A-94F0-4EF6-A35F-2D5B4A3721BF
// Assembly location: D:\SteamLibrary\steamapps\common\Stardew Valley\Stardew Valley.dll
// XML documentation location: D:\SteamLibrary\steamapps\common\Stardew Valley\Stardew Valley.xml

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System;
using System.Collections.Generic;
using System.Linq;

#nullable disable
namespace StardewValley.Menus;

public class Toolbar : IClickableMenu
{
  public List<ClickableComponent> buttons = new List<ClickableComponent>();
  public new int yPositionOnScreen;
  public Item hoverItem;
  public float transparency = 1f;
  private bool hoverDirty = true;
  public string[] slotText = new string[12]
  {
    "1",
    "2",
    "3",
    "4",
    "5",
    "6",
    "7",
    "8",
    "9",
    "0",
    "-",
    "="
  };
  public Rectangle toolbarTextSource = new Rectangle(0, 256 /*0x0100*/, 60, 60);

  public Toolbar()
    : base(Game1.uiViewport.Width / 2 - 384 - 64 /*0x40*/, Game1.uiViewport.Height, 896, 208 /*0xD0*/)
  {
    for (int index = 0; index < 12; ++index)
      this.buttons.Add(new ClickableComponent(new Rectangle(Game1.uiViewport.Width / 2 - 384 + index * 64 /*0x40*/, this.yPositionOnScreen - 96 /*0x60*/ + 8, 64 /*0x40*/, 64 /*0x40*/), index.ToString() ?? ""));
  }

  /// <inheritdoc />
  public override void receiveLeftClick(int x, int y, bool playSound = true)
  {
    if (Game1.player.UsingTool || Game1.IsChatting || Game1.farmEvent != null)
      return;
    foreach (ClickableComponent button in this.buttons)
    {
      if (button.containsPoint(x, y))
      {
        Game1.player.CurrentToolIndex = Convert.ToInt32(button.name);
        if (Game1.player.ActiveObject != null)
        {
          Game1.player.showCarrying();
          Game1.playSound("pickUpItem");
          break;
        }
        Game1.player.showNotCarrying();
        Game1.playSound("stoneStep");
        break;
      }
    }
  }

  /// <inheritdoc />
  public override void receiveRightClick(int x, int y, bool playSound = true)
  {
    if (!Game1.GetKeyboardState().IsKeyDown(Keys.LeftShift) && !Game1.GetKeyboardState().IsKeyDown(Keys.LeftControl))
      return;
    foreach (ClickableComponent button in this.buttons)
    {
      if (button.containsPoint(x, y))
      {
        int int32 = Convert.ToInt32(button.name);
        if (int32 < Game1.player.Items.Count && Game1.player.Items[int32] != null)
        {
          this.hoverItem = Game1.player.Items[int32];
          if (this.hoverItem.canBeDropped())
          {
            Game1.playSound("throwDownITem");
            Game1.player.Items[int32] = (Item) null;
            Game1.createItemDebris(this.hoverItem, Game1.player.getStandingPosition(), Game1.player.FacingDirection).DroppedByPlayerID.Value = Game1.player.UniqueMultiplayerID;
            break;
          }
        }
      }
    }
  }

  /// <inheritdoc />
  public override void performHoverAction(int x, int y)
  {
    if (this.hoverDirty)
    {
      this.gameWindowSizeChanged(new Rectangle(Game1.viewport.X, Game1.viewport.Y, Game1.viewport.Width, Game1.viewport.Height), new Rectangle(Game1.viewport.X, Game1.viewport.Y, Game1.viewport.Width, Game1.viewport.Height));
      this.hoverDirty = false;
    }
    this.hoverItem = (Item) null;
    foreach (ClickableComponent button in this.buttons)
    {
      if (button.containsPoint(x, y))
      {
        int int32 = Convert.ToInt32(button.name);
        if (int32 < Game1.player.Items.Count && Game1.player.Items[int32] != null)
        {
          button.scale = Math.Min(button.scale + 0.05f, 1.1f);
          this.hoverItem = Game1.player.Items[int32];
        }
      }
      else
        button.scale = Math.Max(button.scale - 0.025f, 1f);
    }
  }

  public void shifted(bool right)
  {
    if (right)
    {
      for (int index = 0; index < this.buttons.Count; ++index)
        this.buttons[index].scale = (float) (1.0 + (double) index * 0.029999999329447746);
    }
    else
    {
      for (int index = this.buttons.Count - 1; index >= 0; --index)
        this.buttons[index].scale = (float) (1.0 + (double) (11 - index) * 0.029999999329447746);
    }
  }

  /// <inheritdoc />
  public override void gameWindowSizeChanged(Rectangle oldBounds, Rectangle newBounds)
  {
    for (int index = 0; index < 12; ++index)
      this.buttons[index].bounds = new Rectangle(Game1.uiViewport.Width / 2 - 384 + index * 64 /*0x40*/, this.yPositionOnScreen - 96 /*0x60*/ + 8, 64 /*0x40*/, 64 /*0x40*/);
  }

  public override bool isWithinBounds(int x, int y)
  {
    ClickableComponent button = this.buttons[0];
    return new Rectangle(button.bounds.X, button.bounds.Y, this.buttons.Last<ClickableComponent>().bounds.X - button.bounds.X + 64 /*0x40*/, 64 /*0x40*/).Contains(x, y);
  }

  /// <inheritdoc />
  public override void draw(SpriteBatch b)
  {
    if (Game1.activeClickableMenu != null)
      return;
    Point standingPixel = Game1.player.StandingPixel;
    Vector2 globalPosition = new Vector2((float) standingPixel.X, (float) standingPixel.Y);
    Vector2 local = Game1.GlobalToLocal(Game1.viewport, globalPosition);
    bool flag;
    if (Game1.options.pinToolbarToggle)
    {
      flag = false;
      this.transparency = Math.Min(1f, this.transparency + 0.075f);
      if ((double) local.Y > (double) (Game1.viewport.Height - 192 /*0xC0*/))
        this.transparency = Math.Max(0.33f, this.transparency - 0.15f);
    }
    else
    {
      flag = (double) local.Y > (double) (Game1.viewport.Height / 2 + 64 /*0x40*/);
      this.transparency = 1f;
    }
    int num = Utility.makeSafeMarginY(8);
    int positionOnScreen1 = this.yPositionOnScreen;
    if (!flag)
    {
      this.yPositionOnScreen = Game1.uiViewport.Height;
      this.yPositionOnScreen += 8;
      this.yPositionOnScreen -= num;
    }
    else
    {
      this.yPositionOnScreen = 112 /*0x70*/;
      this.yPositionOnScreen -= 8;
      this.yPositionOnScreen += num;
    }
    int positionOnScreen2 = this.yPositionOnScreen;
    if (positionOnScreen1 != positionOnScreen2)
    {
      for (int index = 0; index < 12; ++index)
        this.buttons[index].bounds.Y = this.yPositionOnScreen - 96 /*0x60*/ + 8;
    }
    IClickableMenu.drawTextureBox(b, Game1.menuTexture, this.toolbarTextSource, Game1.uiViewport.Width / 2 - 384 - 16 /*0x10*/, this.yPositionOnScreen - 96 /*0x60*/ - 8, 800, 96 /*0x60*/, Color.White * this.transparency, drawShadow: false);
    for (int index = 0; index < 12; ++index)
    {
      Vector2 position = new Vector2((float) (Game1.uiViewport.Width / 2 - 384 + index * 64 /*0x40*/), (float) (this.yPositionOnScreen - 96 /*0x60*/ + 8));
      b.Draw(Game1.menuTexture, position, new Rectangle?(Game1.getSourceRectForStandardTileSheet(Game1.menuTexture, Game1.player.CurrentToolIndex == index ? 56 : 10)), Color.White * this.transparency);
      if (!Game1.options.gamepadControls)
        b.DrawString(Game1.tinyFont, this.slotText[index], position + new Vector2(4f, -8f), Color.DimGray * this.transparency);
    }
    for (int index = 0; index < 12; ++index)
    {
      this.buttons[index].scale = Math.Max(1f, this.buttons[index].scale - 0.025f);
      Vector2 location = new Vector2((float) (Game1.uiViewport.Width / 2 - 384 + index * 64 /*0x40*/), (float) (this.yPositionOnScreen - 96 /*0x60*/ + 8));
      if (Game1.player.Items.Count > index && Game1.player.Items[index] != null)
        Game1.player.Items[index].drawInMenu(b, location, Game1.player.CurrentToolIndex == index ? 0.9f : this.buttons[index].scale * 0.8f, this.transparency, 0.88f);
    }
    if (this.hoverItem == null)
      return;
    IClickableMenu.drawToolTip(b, this.hoverItem.getDescription(), this.hoverItem.DisplayName, this.hoverItem);
    this.hoverItem = (Item) null;
  }
}
