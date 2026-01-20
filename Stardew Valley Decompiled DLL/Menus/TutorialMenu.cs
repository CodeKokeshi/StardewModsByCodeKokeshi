// Decompiled with JetBrains decompiler
// Type: StardewValley.Menus.TutorialMenu
// Assembly: Stardew Valley, Version=1.6.15.24356, Culture=neutral, PublicKeyToken=null
// MVID: AA1F513A-94F0-4EF6-A35F-2D5B4A3721BF
// Assembly location: D:\SteamLibrary\steamapps\common\Stardew Valley\Stardew Valley.dll
// XML documentation location: D:\SteamLibrary\steamapps\common\Stardew Valley\Stardew Valley.xml

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;

#nullable disable
namespace StardewValley.Menus;

public class TutorialMenu : IClickableMenu
{
  public const int constructionTab = 4;
  public const int friendshipTab = 5;
  public const int townTab = 6;
  public const int animalsTab = 7;
  private int currentTab = -1;
  private List<ClickableTextureComponent> topics = new List<ClickableTextureComponent>();
  private ClickableTextureComponent backButton;
  private ClickableTextureComponent okButton;
  private List<ClickableTextureComponent> icons = new List<ClickableTextureComponent>();

  public TutorialMenu()
    : base(Game1.uiViewport.Width / 2 - (600 + IClickableMenu.borderWidth * 2) / 2, Game1.uiViewport.Height / 2 - (600 + IClickableMenu.borderWidth * 2) / 2 - 192 /*0xC0*/, 600 + IClickableMenu.borderWidth * 2, 600 + IClickableMenu.borderWidth * 2 + 192 /*0xC0*/)
  {
    int x = this.xPositionOnScreen + 64 /*0x40*/ + 42 - 2;
    int y1 = this.yPositionOnScreen + IClickableMenu.borderWidth + IClickableMenu.spaceToClearTopBorder - 16 /*0x10*/;
    this.topics.Add(new ClickableTextureComponent("", new Rectangle(x, y1, this.width, 64 /*0x40*/), Game1.content.LoadString("Strings\\StringsFromCSFiles:TutorialMenu.cs.11805"), "", Game1.content.Load<Texture2D>("LooseSprites\\TutorialImages\\FarmTut"), Rectangle.Empty, 1f));
    this.icons.Add(new ClickableTextureComponent(new Rectangle(x, y1, 64 /*0x40*/, 64 /*0x40*/), Game1.objectSpriteSheet, Game1.getSourceRectForStandardTileSheet(Game1.objectSpriteSheet, 276), 1f));
    int y2 = y1 + 68;
    this.topics.Add(new ClickableTextureComponent("", new Rectangle(x, y2, this.width, 64 /*0x40*/), Game1.content.LoadString("Strings\\StringsFromCSFiles:TutorialMenu.cs.11807"), "", Game1.content.Load<Texture2D>("LooseSprites\\TutorialImages\\FarmTut"), Rectangle.Empty, 1f));
    this.icons.Add(new ClickableTextureComponent(new Rectangle(x, y2, 64 /*0x40*/, 64 /*0x40*/), Game1.objectSpriteSheet, Game1.getSourceRectForStandardTileSheet(Game1.objectSpriteSheet, 142), 1f));
    int y3 = y2 + 68;
    this.topics.Add(new ClickableTextureComponent("", new Rectangle(x, y3, this.width, 64 /*0x40*/), Game1.content.LoadString("Strings\\StringsFromCSFiles:TutorialMenu.cs.11809"), "", Game1.content.Load<Texture2D>("LooseSprites\\TutorialImages\\FarmTut"), Rectangle.Empty, 1f));
    this.icons.Add(new ClickableTextureComponent(new Rectangle(x, y3, 64 /*0x40*/, 64 /*0x40*/), Game1.objectSpriteSheet, Game1.getSourceRectForStandardTileSheet(Game1.objectSpriteSheet, 334), 1f));
    int y4 = y3 + 68;
    this.topics.Add(new ClickableTextureComponent("", new Rectangle(x, y4, this.width, 64 /*0x40*/), Game1.content.LoadString("Strings\\StringsFromCSFiles:TutorialMenu.cs.11811"), "", Game1.content.Load<Texture2D>("LooseSprites\\TutorialImages\\FarmTut"), Rectangle.Empty, 1f));
    this.icons.Add(new ClickableTextureComponent(new Rectangle(x, y4, 64 /*0x40*/, 64 /*0x40*/), Game1.objectSpriteSheet, Game1.getSourceRectForStandardTileSheet(Game1.objectSpriteSheet, 308), 1f));
    int y5 = y4 + 68;
    this.topics.Add(new ClickableTextureComponent("", new Rectangle(x, y5, this.width, 64 /*0x40*/), Game1.content.LoadString("Strings\\StringsFromCSFiles:TutorialMenu.cs.11813"), "", Game1.content.Load<Texture2D>("LooseSprites\\TutorialImages\\FarmTut"), Rectangle.Empty, 1f));
    this.icons.Add(new ClickableTextureComponent(new Rectangle(x, y5, 64 /*0x40*/, 64 /*0x40*/), Game1.objectSpriteSheet, Game1.getSourceRectForStandardTileSheet(Game1.objectSpriteSheet, 395), 1f));
    int y6 = y5 + 68;
    this.topics.Add(new ClickableTextureComponent("", new Rectangle(x, y6, this.width, 64 /*0x40*/), Game1.content.LoadString("Strings\\StringsFromCSFiles:TutorialMenu.cs.11815"), "", Game1.content.Load<Texture2D>("LooseSprites\\TutorialImages\\FarmTut"), Rectangle.Empty, 1f));
    this.icons.Add(new ClickableTextureComponent(new Rectangle(x, y6, 64 /*0x40*/, 64 /*0x40*/), Game1.objectSpriteSheet, Game1.getSourceRectForStandardTileSheet(Game1.objectSpriteSheet, 458), 1f));
    int y7 = y6 + 68;
    this.topics.Add(new ClickableTextureComponent("", new Rectangle(x, y7, this.width, 64 /*0x40*/), Game1.content.LoadString("Strings\\StringsFromCSFiles:TutorialMenu.cs.11817"), "", Game1.content.Load<Texture2D>("LooseSprites\\TutorialImages\\FarmTut"), Rectangle.Empty, 1f));
    this.icons.Add(new ClickableTextureComponent(new Rectangle(x, y7, 64 /*0x40*/, 64 /*0x40*/), Game1.objectSpriteSheet, Game1.getSourceRectForStandardTileSheet(Game1.objectSpriteSheet, 102), 1f));
    int y8 = y7 + 68;
    this.topics.Add(new ClickableTextureComponent("", new Rectangle(x, y8, this.width, 64 /*0x40*/), Game1.content.LoadString("Strings\\StringsFromCSFiles:TutorialMenu.cs.11819"), "", Game1.content.Load<Texture2D>("LooseSprites\\TutorialImages\\FarmTut"), Rectangle.Empty, 1f));
    this.icons.Add(new ClickableTextureComponent(new Rectangle(x, y8, 64 /*0x40*/, 64 /*0x40*/), Game1.objectSpriteSheet, Game1.getSourceRectForStandardTileSheet(Game1.objectSpriteSheet, 403), 1f));
    int num = y8 + 68;
    this.okButton = new ClickableTextureComponent("OK", new Rectangle(this.xPositionOnScreen + this.width - IClickableMenu.borderWidth - IClickableMenu.spaceToClearSideBorder - 64 /*0x40*/, this.yPositionOnScreen + this.height - IClickableMenu.borderWidth - IClickableMenu.spaceToClearTopBorder + 16 /*0x10*/, 64 /*0x40*/, 64 /*0x40*/), (string) null, (string) null, Game1.mouseCursors, Game1.getSourceRectForStandardTileSheet(Game1.mouseCursors, 46), 1f);
    this.backButton = new ClickableTextureComponent("Back", new Rectangle(this.xPositionOnScreen + this.width - IClickableMenu.borderWidth - IClickableMenu.spaceToClearSideBorder - 48 /*0x30*/, this.yPositionOnScreen + this.height - IClickableMenu.borderWidth - IClickableMenu.spaceToClearTopBorder + 16 /*0x10*/, 64 /*0x40*/, 64 /*0x40*/), (string) null, (string) null, Game1.mouseCursors, Game1.getSourceRectForStandardTileSheet(Game1.mouseCursors, 44), 1f);
  }

  /// <inheritdoc />
  public override void receiveLeftClick(int x, int y, bool playSound = true)
  {
    if (this.currentTab == -1)
    {
      for (int index = 0; index < this.topics.Count; ++index)
      {
        if (this.topics[index].containsPoint(x, y))
        {
          this.currentTab = index;
          Game1.playSound("smallSelect");
          break;
        }
      }
    }
    if (this.currentTab != -1 && this.backButton.containsPoint(x, y))
    {
      this.currentTab = -1;
      Game1.playSound("bigDeSelect");
    }
    else
    {
      if (this.currentTab != -1 || !this.okButton.containsPoint(x, y))
        return;
      Game1.playSound("bigDeSelect");
      Game1.exitActiveMenu();
      if (Game1.currentLocation.currentEvent == null)
        return;
      ++Game1.currentLocation.currentEvent.CurrentCommand;
    }
  }

  /// <inheritdoc />
  public override void performHoverAction(int x, int y)
  {
    foreach (ClickableComponent topic in this.topics)
      topic.scale = !topic.containsPoint(x, y) ? 1f : 2f;
    if (this.okButton.containsPoint(x, y))
      this.okButton.scale = Math.Min(this.okButton.scale + 0.02f, this.okButton.baseScale + 0.1f);
    else
      this.okButton.scale = Math.Max(this.okButton.scale - 0.02f, this.okButton.baseScale);
    if (this.backButton.containsPoint(x, y))
      this.backButton.scale = Math.Min(this.backButton.scale + 0.02f, this.backButton.baseScale + 0.1f);
    else
      this.backButton.scale = Math.Max(this.backButton.scale - 0.02f, this.backButton.baseScale);
  }

  /// <inheritdoc />
  public override void draw(SpriteBatch b)
  {
    b.Draw(Game1.fadeToBlackRect, Game1.graphics.GraphicsDevice.Viewport.Bounds, Color.Black * 0.4f);
    Game1.drawDialogueBox(this.xPositionOnScreen, this.yPositionOnScreen, this.width, this.height, false, true);
    if (this.currentTab != -1)
    {
      this.backButton.draw(b);
      b.Draw(this.topics[this.currentTab].texture, new Vector2((float) (this.xPositionOnScreen + IClickableMenu.borderWidth + IClickableMenu.spaceToClearSideBorder), (float) (this.yPositionOnScreen + IClickableMenu.borderWidth + IClickableMenu.spaceToClearTopBorder - 16 /*0x10*/)), new Rectangle?(this.topics[this.currentTab].texture.Bounds), Color.White, 0.0f, Vector2.Zero, 2f, SpriteEffects.None, 0.89f);
    }
    else
    {
      foreach (ClickableTextureComponent topic in this.topics)
      {
        Color color = (double) topic.scale > 1.0 ? Color.Blue : Game1.textColor;
        b.DrawString(Game1.smallFont, topic.label, new Vector2((float) (topic.bounds.X + 64 /*0x40*/ + 16 /*0x10*/), (float) (topic.bounds.Y + 21)), color);
      }
      foreach (ClickableTextureComponent icon in this.icons)
        icon.draw(b);
      this.okButton.draw(b);
    }
    this.drawMouse(b);
  }
}
