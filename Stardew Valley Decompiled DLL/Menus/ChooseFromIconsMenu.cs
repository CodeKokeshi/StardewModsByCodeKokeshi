// Decompiled with JetBrains decompiler
// Type: StardewValley.Menus.ChooseFromIconsMenu
// Assembly: Stardew Valley, Version=1.6.15.24356, Culture=neutral, PublicKeyToken=null
// MVID: AA1F513A-94F0-4EF6-A35F-2D5B4A3721BF
// Assembly location: D:\SteamLibrary\steamapps\common\Stardew Valley\Stardew Valley.dll
// XML documentation location: D:\SteamLibrary\steamapps\common\Stardew Valley\Stardew Valley.xml

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using StardewValley.BellsAndWhistles;
using StardewValley.Tools;
using System;
using System.Collections.Generic;

#nullable disable
namespace StardewValley.Menus;

public class ChooseFromIconsMenu : IClickableMenu
{
  private Rectangle iconBackRectangle;
  private Texture2D texture;
  private Point iconBackHighlightPosition;
  private Point iconFrontHighlightPositionOffset;
  private string which;
  public List<ClickableTextureComponent> icons = new List<ClickableTextureComponent>();
  public List<ClickableTextureComponent> iconFronts = new List<ClickableTextureComponent>();
  private int iconXOffset;
  private int maxTooltipHeight;
  private int maxTooltipWidth;
  private float destroyTimer = -1f;
  private List<TemporaryAnimatedSprite> temporarySprites = new List<TemporaryAnimatedSprite>();
  public StardewValley.Object sourceObject;
  private bool hasTooltips = true;
  private string title;
  private string hoverSound;
  private int titleStyle = 3;
  private int selected = -1;

  public ChooseFromIconsMenu(string which) => this.setUpIcons(which);

  /// <inheritdoc />
  public override void gameWindowSizeChanged(Rectangle oldBounds, Rectangle newBounds)
  {
    base.gameWindowSizeChanged(oldBounds, newBounds);
    this.setUpIcons(this.which);
  }

  public void setUpIcons(string which)
  {
    int num1 = 32 /*0x20*/;
    int num2 = 12;
    int num3 = 4;
    this.which = which;
    this.title = Game1.content.LoadString("Strings\\1_6_Strings:ChooseOne");
    this.hoverSound = "boulderCrack";
    this.icons.Clear();
    this.iconFronts.Clear();
    switch (which)
    {
      case "dwarfStatue":
        Game1.playSound("stone_button");
        this.iconBackRectangle = new Rectangle((int) sbyte.MaxValue, 123, 21, 21);
        this.iconBackHighlightPosition = new Point((int) sbyte.MaxValue, 144 /*0x90*/);
        this.iconFrontHighlightPositionOffset = new Point(0, 17);
        this.texture = Game1.mouseCursors_1_6;
        Random random = Utility.CreateRandom((double) (Game1.stats.DaysPlayed * 77U), (double) Game1.uniqueIDForThisGame);
        int num4 = random.Next(5);
        int num5;
        do
        {
          num5 = random.Next(5);
        }
        while (num5 == num4);
        List<ClickableTextureComponent> icons1 = this.icons;
        ClickableTextureComponent textureComponent1 = new ClickableTextureComponent(new Rectangle(0, 0, 84, 84), this.texture, this.iconBackRectangle, 4f, true);
        textureComponent1.name = num4.ToString() ?? "";
        textureComponent1.hoverText = Game1.content.LoadString("Strings\\1_6_Strings:DwarfStatue_" + num4.ToString());
        icons1.Add(textureComponent1);
        List<ClickableTextureComponent> icons2 = this.icons;
        ClickableTextureComponent textureComponent2 = new ClickableTextureComponent(new Rectangle(0, 0, 84, 84), this.texture, this.iconBackRectangle, 4f, true);
        textureComponent2.name = num5.ToString() ?? "";
        textureComponent2.hoverText = Game1.content.LoadString("Strings\\1_6_Strings:DwarfStatue_" + num5.ToString());
        icons2.Add(textureComponent2);
        this.iconFronts.Add(new ClickableTextureComponent(new Rectangle(0, 0, 17, 17), this.texture, new Rectangle(148 + num4 * 17, 123, 17, 17), 4f));
        this.iconFronts.Add(new ClickableTextureComponent(new Rectangle(0, 0, 17, 17), this.texture, new Rectangle(148 + num5 * 17, 123, 17, 17), 4f));
        break;
      case "bobbers":
        if (Game1.player.usingRandomizedBobber)
          Game1.player.bobberStyle.Value = -2;
        int num6 = Game1.player.fishCaught.Count() / 2;
        num1 = 4;
        this.iconBackRectangle = new Rectangle(222, 317, 16 /*0x10*/, 16 /*0x10*/);
        this.iconBackHighlightPosition = new Point(256 /*0x0100*/, 317);
        this.texture = Game1.mouseCursors_1_6;
        for (int tilePosition = 0; tilePosition < FishingRod.NUM_BOBBER_STYLES; ++tilePosition)
        {
          int num7 = tilePosition > num6 ? 1 : 0;
          Rectangle standardTileSheet = Game1.getSourceRectForStandardTileSheet(Game1.bobbersTexture, tilePosition, 16 /*0x10*/, 32 /*0x20*/) with
          {
            Height = 16 /*0x10*/
          };
          List<ClickableTextureComponent> icons3 = this.icons;
          ClickableTextureComponent textureComponent3 = new ClickableTextureComponent(new Rectangle(0, 0, 64 /*0x40*/, 64 /*0x40*/), this.texture, this.iconBackRectangle, 4f, true);
          textureComponent3.name = tilePosition.ToString() ?? "";
          icons3.Add(textureComponent3);
          if (num7 != 0)
          {
            List<ClickableTextureComponent> iconFronts = this.iconFronts;
            ClickableTextureComponent textureComponent4 = new ClickableTextureComponent(new Rectangle(0, 0, 16 /*0x10*/, 16 /*0x10*/), Game1.mouseCursors_1_6, new Rectangle(272, 317, 16 /*0x10*/, 16 /*0x10*/), 4f);
            textureComponent4.name = "ghosted";
            iconFronts.Add(textureComponent4);
          }
          else
            this.iconFronts.Add(new ClickableTextureComponent(new Rectangle(0, 0, 16 /*0x10*/, 16 /*0x10*/), Game1.bobbersTexture, standardTileSheet, 4f, true));
        }
        List<ClickableTextureComponent> icons4 = this.icons;
        ClickableTextureComponent textureComponent5 = new ClickableTextureComponent(new Rectangle(0, 0, 64 /*0x40*/, 64 /*0x40*/), (Texture2D) null, new Rectangle(0, 0, 0, 0), 4f, true);
        textureComponent5.name = "-2";
        icons4.Add(textureComponent5);
        this.iconFronts.Add(new ClickableTextureComponent(new Rectangle(0, 0, 10, 10), Game1.mouseCursors_1_6, new Rectangle(496, 28, 16 /*0x10*/, 16 /*0x10*/), 4f, true));
        this.selected = Game1.player.bobberStyle.Value;
        num2 = 0;
        num3 = 0;
        this.hasTooltips = false;
        this.title = Game1.content.LoadString("Strings\\1_6_Strings:ChooseBobber");
        this.titleStyle = 0;
        this.hoverSound = (string) null;
        break;
    }
    int val2_1 = this.hasTooltips ? 240 /*0xF0*/ : 0;
    int num8 = Math.Max(this.iconBackRectangle.Width * 4, val2_1) + num1;
    this.iconXOffset = num8 / 2 - this.iconBackRectangle.Width * 4 / 2 - 4;
    this.width = Math.Max(800, Game1.uiViewport.Width / 3);
    this.xPositionOnScreen = Game1.uiViewport.Width / 2 - this.width / 2;
    this.height = 100;
    this.maxTooltipHeight = 0;
    this.maxTooltipWidth = 0;
    if (this.hasTooltips)
    {
      foreach (ClickableTextureComponent icon in this.icons)
      {
        icon.hoverText = Game1.parseText(icon.hoverText, Game1.smallFont, val2_1 - 32 /*0x20*/);
        this.maxTooltipHeight = Math.Max(this.maxTooltipHeight, (int) Game1.smallFont.MeasureString(icon.hoverText).Y);
        this.maxTooltipWidth = Math.Max(this.maxTooltipWidth, (int) Game1.smallFont.MeasureString(icon.hoverText).X);
      }
      this.maxTooltipHeight += 48 /*0x30*/;
      this.maxTooltipWidth += 48 /*0x30*/;
    }
    this.height += (this.icons.Count * num8 / this.width + 1) * (this.maxTooltipHeight + this.icons[0].bounds.Height + num1);
    int val2_2 = this.width / num8;
    this.yPositionOnScreen = Game1.uiViewport.Height / 2 - this.height / 2;
    int num9 = this.yPositionOnScreen + 100;
    for (int index1 = 0; index1 < this.icons.Count; index1 += val2_2)
    {
      int num10 = Math.Min(this.icons.Count - index1, val2_2);
      int num11 = this.xPositionOnScreen + this.width / 2 - num10 * num8 / 2;
      for (int index2 = 0; index2 < num10; ++index2)
      {
        int index3 = index2 + index1;
        this.icons[index3].bounds.X = num11 + index2 * num8;
        this.icons[index3].bounds.Y = num9;
        this.icons[index3].bounds.Width = num8;
        this.icons[index3].bounds.Height += this.maxTooltipHeight;
        this.iconFronts[index3].bounds.X = this.icons[index3].bounds.X + num2;
        this.iconFronts[index3].bounds.Y = this.icons[index3].bounds.Y + num3;
        this.icons[index3].myID = index3;
        this.icons[index3].leftNeighborID = index3 - 1;
        this.icons[index3].rightNeighborID = index3 + 1;
        this.icons[index3].downNeighborID = index3 + num10;
        this.icons[index3].upNeighborID = index3 - num10;
      }
      num9 += this.maxTooltipHeight + this.icons[0].bounds.Height + num1;
    }
    this.initialize(this.xPositionOnScreen, this.yPositionOnScreen, this.width, this.height, true);
    if (!Game1.options.SnappyMenus)
      return;
    this.populateClickableComponentList();
    this.currentlySnappedComponent = this.getComponentWithID(0);
    this.snapCursorToCurrentSnappedComponent();
  }

  /// <inheritdoc />
  public override void update(GameTime time)
  {
    base.update(time);
    if ((double) this.destroyTimer > 0.0)
    {
      this.destroyTimer -= (float) time.ElapsedGameTime.TotalMilliseconds;
      if ((double) this.destroyTimer <= 0.0)
      {
        this.flairOnDestroy();
        Game1.activeClickableMenu = (IClickableMenu) null;
      }
    }
    this.temporarySprites.RemoveAll((Predicate<TemporaryAnimatedSprite>) (sprite => sprite.update(time)));
  }

  /// <inheritdoc />
  public override void performHoverAction(int x, int y)
  {
    base.performHoverAction(x, y);
    for (int index = 0; index < this.icons.Count; ++index)
    {
      ClickableTextureComponent icon = this.icons[index];
      this.iconFronts[index].sourceRect = this.iconFronts[index].startingSourceRect;
      if (icon.containsPoint(x, y) && (double) this.destroyTimer == -1.0)
      {
        if (icon.sourceRect == icon.startingSourceRect && this.hoverSound != null)
          Game1.playSound(this.hoverSound);
        icon.sourceRect.Location = this.iconBackHighlightPosition;
        this.iconFronts[index].sourceRect.Location = new Point(this.iconFronts[index].sourceRect.Location.X + this.iconFrontHighlightPositionOffset.X, this.iconFronts[index].sourceRect.Location.Y + this.iconFrontHighlightPositionOffset.Y);
      }
      else
        icon.sourceRect = this.iconBackRectangle;
    }
  }

  /// <inheritdoc />
  public override void receiveLeftClick(int x, int y, bool playSound = true)
  {
    if ((double) this.destroyTimer >= 0.0)
      return;
    base.receiveLeftClick(x, y, playSound);
    for (int index1 = 0; index1 < this.icons.Count; ++index1)
    {
      ClickableTextureComponent icon = this.icons[index1];
      if (icon.containsPoint(x, y))
      {
        bool flag = this.iconFronts[index1].name.Contains("ghosted");
        switch (this.which)
        {
          case "dwarfStatue":
            Game1.playSound("button_tap");
            DelayedAction.playSoundAfterDelay("button_tap", 70);
            DelayedAction.playSoundAfterDelay("discoverMineral", 750);
            for (int index2 = 0; index2 < 16 /*0x10*/; ++index2)
              this.temporarySprites.Add(new TemporaryAnimatedSprite("LooseSprites\\Cursors_1_6", new Rectangle(98 + Game1.random.Next(3) * 4, 161, 4, 4), Utility.getRandomPositionInThisRectangle(icon.bounds, Game1.random), false, 0.0f, Color.White)
              {
                local = true,
                scale = 4f,
                interval = 9999f,
                motion = new Vector2((float) Game1.random.Next(-15, 16 /*0x10*/) / 10f, (float) ((double) Game1.random.Next(-10, 11) / 10.0 - 7.0)),
                acceleration = new Vector2(0.0f, 0.5f)
              });
            this.destroyTimer = 800f;
            break;
          case "bobbers":
            if (flag)
            {
              Game1.playSound("smallSelect");
              return;
            }
            if (Game1.player.bobberStyle.Value != Convert.ToInt32(icon.name))
            {
              Game1.playSound("button1");
              this.hoverSound = (string) null;
              Game1.player.bobberStyle.Value = Convert.ToInt32(icon.name);
              this.selected = Game1.player.bobberStyle.Value;
              Game1.player.usingRandomizedBobber = this.selected == -2;
              break;
            }
            break;
        }
        this.doIconAction(icon.name);
      }
    }
  }

  private void doIconAction(string iconName)
  {
    if (!(this.which == "dwarfStatue") || Game1.player.hasBuffWithNameContainingString("dwarfStatue"))
      return;
    Game1.player.applyBuff($"{this.which}_{iconName}");
  }

  private void flairOnDestroy()
  {
    if (!(this.which == "dwarfStatue"))
      return;
    this.sourceObject.shakeTimer = 500;
    if (this.sourceObject.Location == null)
      return;
    Utility.addSprinklesToLocation(this.sourceObject.Location, (int) this.sourceObject.TileLocation.X, (int) this.sourceObject.TileLocation.Y, 3, 4, 800, 40, Color.White);
  }

  /// <inheritdoc />
  public override void draw(SpriteBatch b)
  {
    b.Draw(Game1.fadeToBlackRect, new Rectangle(0, 0, Game1.uiViewport.Width, Game1.uiViewport.Height), Color.Black * 0.7f);
    base.draw(b);
    SpriteText.drawStringWithScrollCenteredAt(b, this.title, this.xPositionOnScreen + this.width / 2, this.yPositionOnScreen + 20, color: new Color?(this.titleStyle == 3 ? Color.LightGray : Game1.textColor), scrollType: this.titleStyle);
    for (int index = 0; index < this.icons.Count; ++index)
    {
      if (this.selected == index || this.selected == -2 && index == this.icons.Count - 1)
      {
        if (this.selected == index)
        {
          Rectangle bounds = this.icons[index].bounds;
          bounds.Inflate(2, 4);
          bounds.X += this.iconXOffset - 2;
          b.Draw(Game1.staminaRect, bounds, Color.Red);
          if (this.icons[index].sourceRect.Width > 0)
          {
            this.icons[index].sourceRect.X = this.iconBackHighlightPosition.X;
            this.icons[index].sourceRect.Y = this.iconBackHighlightPosition.Y;
          }
        }
        else
          b.Draw(Game1.mouseCursors_1_6, this.icons[index].getVector2(), new Rectangle?(new Rectangle(480, 28, 16 /*0x10*/, 16 /*0x10*/)), Color.Red, 0.0f, Vector2.Zero, 4f, SpriteEffects.None, 1f);
      }
      this.icons[index].draw(b, Color.White, 0.0f, xOffset: this.iconXOffset);
      this.iconFronts[index].draw(b, this.iconFronts[index].name.Equals("ghosted_fade") ? Color.Black * 0.4f : Color.White, 0.87f, xOffset: this.iconXOffset);
      IClickableMenu.drawHoverText(b, this.icons[index].hoverText, Game1.smallFont, overrideX: this.icons[index].bounds.X + 4, overrideY: this.icons[index].bounds.Y + this.icons[index].bounds.Height - this.maxTooltipHeight + 4, boxTexture: Game1.mouseCursors_1_6, boxSourceRect: new Rectangle?(this.icons[index].sourceRect != this.icons[index].startingSourceRect ? new Rectangle(111, 145, 15, 15) : new Rectangle(96 /*0x60*/, 145, 15, 15)), textColor: new Color?(Color.White), textShadowColor: new Color?(new Color(26, 26, 43)), boxScale: 4f, boxWidthOverride: this.maxTooltipWidth, boxHeightOverride: this.maxTooltipHeight);
    }
    foreach (TemporaryAnimatedSprite temporarySprite in this.temporarySprites)
      temporarySprite.draw(b);
    this.drawMouse(b);
  }
}
