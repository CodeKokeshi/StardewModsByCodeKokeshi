// Decompiled with JetBrains decompiler
// Type: StardewValley.Menus.ExitPage
// Assembly: Stardew Valley, Version=1.6.15.24356, Culture=neutral, PublicKeyToken=null
// MVID: AA1F513A-94F0-4EF6-A35F-2D5B4A3721BF
// Assembly location: D:\SteamLibrary\steamapps\common\Stardew Valley\Stardew Valley.dll
// XML documentation location: D:\SteamLibrary\steamapps\common\Stardew Valley\Stardew Valley.xml

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

#nullable disable
namespace StardewValley.Menus;

public class ExitPage : IClickableMenu
{
  public ClickableComponent exitToTitle;
  public ClickableComponent exitToDesktop;

  public ExitPage(int x, int y, int width, int height)
    : base(x, y, width, height)
  {
    string str = Game1.content.LoadString("Strings\\UI:ExitToTitle");
    if (!Game1.game1.IsMainInstance)
      str = Game1.content.LoadString("Strings\\UI:DropOutLocalMulti");
    Vector2 vector2 = new Vector2((float) (this.xPositionOnScreen + width / 2 - (int) (((double) Game1.dialogueFont.MeasureString(str).X + 64.0) / 2.0)), (float) (this.yPositionOnScreen + 256 /*0x0100*/ - 32 /*0x20*/));
    this.exitToTitle = new ClickableComponent(new Rectangle((int) vector2.X, (int) vector2.Y, (int) Game1.dialogueFont.MeasureString(str).X + 64 /*0x40*/, 96 /*0x60*/), "", str)
    {
      myID = 535,
      upNeighborID = 12349,
      downNeighborID = 536
    };
    vector2 = new Vector2((float) (this.xPositionOnScreen + width / 2 - (int) (((double) Game1.dialogueFont.MeasureString(Game1.content.LoadString("Strings\\UI:ExitToDesktop")).X + 64.0) / 2.0)), (float) (this.yPositionOnScreen + 384 + 8 - 32 /*0x20*/));
    this.exitToDesktop = new ClickableComponent(new Rectangle((int) vector2.X, (int) vector2.Y, (int) Game1.dialogueFont.MeasureString(Game1.content.LoadString("Strings\\UI:ExitToDesktop")).X + 64 /*0x40*/, 96 /*0x60*/), "", Game1.content.LoadString("Strings\\UI:ExitToDesktop"))
    {
      myID = 536,
      upNeighborID = 535
    };
    if (!Game1.game1.IsMainInstance)
      this.exitToDesktop.visible = false;
    if (!Game1.options.SnappyMenus)
      return;
    this.populateClickableComponentList();
    this.snapToDefaultClickableComponent();
  }

  public override void snapToDefaultClickableComponent()
  {
    this.currentlySnappedComponent = this.getComponentWithID(12349);
    this.snapCursorToCurrentSnappedComponent();
  }

  /// <inheritdoc />
  public override void receiveLeftClick(int x, int y, bool playSound = true)
  {
    if (Game1.conventionMode)
      return;
    if (this.exitToTitle.containsPoint(x, y) && this.exitToTitle.visible)
    {
      if (Game1.options.optionsDirty)
        Game1.options.SaveDefaultOptions();
      Game1.playSound("bigDeSelect");
      Game1.ExitToTitle();
    }
    if (!this.exitToDesktop.containsPoint(x, y) || !this.exitToDesktop.visible)
      return;
    if (Game1.options.optionsDirty)
      Game1.options.SaveDefaultOptions();
    Game1.playSound("bigDeSelect");
    Game1.quit = true;
  }

  /// <inheritdoc />
  public override void performHoverAction(int x, int y)
  {
    if (this.exitToTitle.containsPoint(x, y) && this.exitToTitle.visible)
    {
      if ((double) this.exitToTitle.scale == 0.0)
        Game1.playSound("Cowboy_gunshot");
      this.exitToTitle.scale = 1f;
    }
    else
      this.exitToTitle.scale = 0.0f;
    if (this.exitToDesktop.containsPoint(x, y) && this.exitToDesktop.visible)
    {
      if ((double) this.exitToDesktop.scale == 0.0)
        Game1.playSound("Cowboy_gunshot");
      this.exitToDesktop.scale = 1f;
    }
    else
      this.exitToDesktop.scale = 0.0f;
  }

  /// <inheritdoc />
  public override void draw(SpriteBatch b)
  {
    if (this.exitToTitle.visible)
    {
      IClickableMenu.drawTextureBox(b, Game1.mouseCursors, new Rectangle(432, 439, 9, 9), this.exitToTitle.bounds.X, this.exitToTitle.bounds.Y, this.exitToTitle.bounds.Width, this.exitToTitle.bounds.Height, (double) this.exitToTitle.scale > 0.0 ? Color.Wheat : Color.White, 4f);
      Utility.drawTextWithShadow(b, this.exitToTitle.label, Game1.dialogueFont, new Vector2((float) this.exitToTitle.bounds.Center.X, (float) (this.exitToTitle.bounds.Center.Y + 4)) - Game1.dialogueFont.MeasureString(this.exitToTitle.label) / 2f, Game1.textColor, shadowIntensity: 0.0f);
    }
    if (!this.exitToDesktop.visible)
      return;
    IClickableMenu.drawTextureBox(b, Game1.mouseCursors, new Rectangle(432, 439, 9, 9), this.exitToDesktop.bounds.X, this.exitToDesktop.bounds.Y, this.exitToDesktop.bounds.Width, this.exitToDesktop.bounds.Height, (double) this.exitToDesktop.scale > 0.0 ? Color.Wheat : Color.White, 4f);
    Utility.drawTextWithShadow(b, this.exitToDesktop.label, Game1.dialogueFont, new Vector2((float) this.exitToDesktop.bounds.Center.X, (float) (this.exitToDesktop.bounds.Center.Y + 4)) - Game1.dialogueFont.MeasureString(this.exitToDesktop.label) / 2f, Game1.textColor, shadowIntensity: 0.0f);
  }
}
