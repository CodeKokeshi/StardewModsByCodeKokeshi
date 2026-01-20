// Decompiled with JetBrains decompiler
// Type: StardewValley.Menus.ConfirmationDialog
// Assembly: Stardew Valley, Version=1.6.15.24356, Culture=neutral, PublicKeyToken=null
// MVID: AA1F513A-94F0-4EF6-A35F-2D5B4A3721BF
// Assembly location: D:\SteamLibrary\steamapps\common\Stardew Valley\Stardew Valley.dll
// XML documentation location: D:\SteamLibrary\steamapps\common\Stardew Valley\Stardew Valley.xml

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using StardewValley.Extensions;
using System;

#nullable disable
namespace StardewValley.Menus;

public class ConfirmationDialog : IClickableMenu
{
  public const int region_okButton = 101;
  public const int region_cancelButton = 102;
  protected string message;
  public ClickableTextureComponent okButton;
  public ClickableTextureComponent cancelButton;
  protected ConfirmationDialog.behavior onConfirm;
  protected ConfirmationDialog.behavior onCancel;
  private bool active = true;
  private int delayBeforeCancellable;

  public ConfirmationDialog(
    string message,
    ConfirmationDialog.behavior onConfirm,
    ConfirmationDialog.behavior onCancel = null)
    : base(Game1.uiViewport.Width / 2 - (int) Game1.dialogueFont.MeasureString(message).X / 2 - IClickableMenu.borderWidth, Game1.uiViewport.Height / 2 - (int) Game1.dialogueFont.MeasureString(message).Y / 2, (int) Game1.dialogueFont.MeasureString(message).X + IClickableMenu.borderWidth * 2, (int) Game1.dialogueFont.MeasureString(message).Y + IClickableMenu.borderWidth * 2 + 160 /*0xA0*/)
  {
    if (onCancel == null)
      onCancel = new ConfirmationDialog.behavior(this.closeDialog);
    else
      this.onCancel = onCancel;
    this.onConfirm = onConfirm;
    Rectangle titleSafeArea = Game1.graphics.GraphicsDevice.Viewport.GetTitleSafeArea();
    message = Game1.parseText(message, Game1.dialogueFont, Math.Min(titleSafeArea.Width - 64 /*0x40*/, this.width));
    this.message = message;
    ClickableTextureComponent textureComponent1 = new ClickableTextureComponent("OK", new Rectangle(this.xPositionOnScreen + this.width - IClickableMenu.borderWidth - IClickableMenu.spaceToClearSideBorder - 128 /*0x80*/ - 4, this.yPositionOnScreen + this.height - IClickableMenu.borderWidth - IClickableMenu.spaceToClearTopBorder + 21, 64 /*0x40*/, 64 /*0x40*/), (string) null, (string) null, Game1.mouseCursors, Game1.getSourceRectForStandardTileSheet(Game1.mouseCursors, 46), 1f);
    textureComponent1.myID = 101;
    textureComponent1.rightNeighborID = 102;
    this.okButton = textureComponent1;
    ClickableTextureComponent textureComponent2 = new ClickableTextureComponent("OK", new Rectangle(this.xPositionOnScreen + this.width - IClickableMenu.borderWidth - IClickableMenu.spaceToClearSideBorder - 64 /*0x40*/, this.yPositionOnScreen + this.height - IClickableMenu.borderWidth - IClickableMenu.spaceToClearTopBorder + 21, 64 /*0x40*/, 64 /*0x40*/), (string) null, (string) null, Game1.mouseCursors, Game1.getSourceRectForStandardTileSheet(Game1.mouseCursors, 47), 1f);
    textureComponent2.myID = 102;
    textureComponent2.leftNeighborID = 101;
    this.cancelButton = textureComponent2;
    if (!Game1.options.SnappyMenus)
      return;
    this.populateClickableComponentList();
    this.snapToDefaultClickableComponent();
    this.delayBeforeCancellable = 300;
  }

  /// <inheritdoc />
  public override void gameWindowSizeChanged(Rectangle oldBounds, Rectangle newBounds)
  {
    base.gameWindowSizeChanged(oldBounds, newBounds);
    this.okButton.setPosition(this.xPositionOnScreen + this.width - IClickableMenu.borderWidth - IClickableMenu.spaceToClearSideBorder - 128 /*0x80*/ - 4, this.yPositionOnScreen + this.height - IClickableMenu.borderWidth - IClickableMenu.spaceToClearTopBorder + 21);
    this.cancelButton.setPosition(this.xPositionOnScreen + this.width - IClickableMenu.borderWidth - IClickableMenu.spaceToClearSideBorder - 64 /*0x40*/, this.yPositionOnScreen + this.height - IClickableMenu.borderWidth - IClickableMenu.spaceToClearTopBorder + 21);
  }

  public virtual void closeDialog(Farmer who)
  {
    if (Game1.activeClickableMenu is TitleMenu activeClickableMenu)
      activeClickableMenu.backButtonPressed();
    else
      Game1.exitActiveMenu();
  }

  public override void snapToDefaultClickableComponent()
  {
    this.currentlySnappedComponent = this.getComponentWithID(102);
    this.snapCursorToCurrentSnappedComponent();
  }

  public void confirm()
  {
    if (!this.active)
      return;
    this.active = false;
    ConfirmationDialog.behavior onConfirm = this.onConfirm;
    if (onConfirm != null)
      onConfirm(Game1.player);
    Game1.playSound("smallSelect");
  }

  public void cancel()
  {
    if (this.onCancel != null)
      this.onCancel(Game1.player);
    else
      this.closeDialog(Game1.player);
    Game1.playSound("bigDeSelect");
  }

  /// <inheritdoc />
  public override void receiveLeftClick(int x, int y, bool playSound = true)
  {
    if (!this.active)
      return;
    if (this.okButton.containsPoint(x, y))
      this.confirm();
    if (!this.cancelButton.containsPoint(x, y) || this.delayBeforeCancellable > 0)
      return;
    this.cancel();
  }

  /// <inheritdoc />
  public override void receiveKeyPress(Keys key)
  {
    base.receiveKeyPress(key);
    if (this.active && Game1.activeClickableMenu == null && this.onCancel != null)
      this.onCancel(Game1.player);
    if (!this.active)
      return;
    if (key != Keys.N)
    {
      if (key != Keys.Y)
        return;
      this.confirm();
    }
    else
      this.cancel();
  }

  public override void update(GameTime time)
  {
    base.update(time);
    if (this.delayBeforeCancellable <= 0)
      return;
    this.delayBeforeCancellable -= (int) time.ElapsedGameTime.TotalMilliseconds;
  }

  /// <inheritdoc />
  public override void performHoverAction(int x, int y)
  {
    if (this.okButton.containsPoint(x, y))
      this.okButton.scale = Math.Min(this.okButton.scale + 0.02f, this.okButton.baseScale + 0.2f);
    else
      this.okButton.scale = Math.Max(this.okButton.scale - 0.02f, this.okButton.baseScale);
    if (this.cancelButton.containsPoint(x, y))
      this.cancelButton.scale = (double) this.cancelButton.baseScale == 1.0 ? Math.Min(this.cancelButton.scale + 0.02f, this.cancelButton.baseScale + 0.2f) : Math.Min(this.cancelButton.scale + 0.1f, this.cancelButton.baseScale + 0.75f);
    else
      this.cancelButton.scale = (double) this.cancelButton.baseScale == 1.0 ? Math.Max(this.cancelButton.scale - 0.02f, this.cancelButton.baseScale) : Math.Max(this.cancelButton.scale - 0.1f, this.cancelButton.baseScale);
  }

  /// <inheritdoc />
  public override void draw(SpriteBatch b)
  {
    if (!this.active)
      return;
    b.Draw(Game1.fadeToBlackRect, new Rectangle(0, 0, Game1.uiViewport.Width, Game1.uiViewport.Height), Color.Black * 0.5f);
    Game1.drawDialogueBox(this.xPositionOnScreen, this.yPositionOnScreen, this.width, this.height, false, true);
    b.DrawString(Game1.dialogueFont, this.message, new Vector2((float) (this.xPositionOnScreen + IClickableMenu.borderWidth), (float) (this.yPositionOnScreen + IClickableMenu.spaceToClearTopBorder + IClickableMenu.borderWidth / 2)), Game1.textColor);
    this.okButton.draw(b);
    this.cancelButton.draw(b);
    this.drawMouse(b);
  }

  public delegate void behavior(Farmer who);
}
