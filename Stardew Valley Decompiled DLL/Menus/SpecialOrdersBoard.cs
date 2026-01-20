// Decompiled with JetBrains decompiler
// Type: StardewValley.Menus.SpecialOrdersBoard
// Assembly: Stardew Valley, Version=1.6.15.24356, Culture=neutral, PublicKeyToken=null
// MVID: AA1F513A-94F0-4EF6-A35F-2D5B4A3721BF
// Assembly location: D:\SteamLibrary\steamapps\common\Stardew Valley\Stardew Valley.dll
// XML documentation location: D:\SteamLibrary\steamapps\common\Stardew Valley\Stardew Valley.xml

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using StardewValley.BellsAndWhistles;
using StardewValley.SpecialOrders;
using StardewValley.SpecialOrders.Rewards;
using StardewValley.TokenizableStrings;
using System;
using System.Collections.Generic;

#nullable disable
namespace StardewValley.Menus;

public class SpecialOrdersBoard : IClickableMenu
{
  private Texture2D billboardTexture;
  public const int basewidth = 338;
  public const int baseheight = 198;
  public ClickableComponent acceptLeftQuestButton;
  public ClickableComponent acceptRightQuestButton;
  public string boardType = "";
  public SpecialOrder leftOrder;
  public SpecialOrder rightOrder;
  public string[] emojiIndices = new string[38]
  {
    "Abigail",
    "Penny",
    "Maru",
    "Leah",
    "Haley",
    "Emily",
    "Alex",
    "Shane",
    "Sebastian",
    "Sam",
    "Harvey",
    "Elliott",
    "Sandy",
    "Evelyn",
    "Marnie",
    "Caroline",
    "Robin",
    "Pierre",
    "Pam",
    "Jodi",
    "Lewis",
    "Linus",
    "Marlon",
    "Willy",
    "Wizard",
    "Morris",
    "Jas",
    "Vincent",
    "Krobus",
    "Dwarf",
    "Gus",
    "Gunther",
    "George",
    "Demetrius",
    "Clint",
    "Baby",
    "Baby",
    "Bear"
  };

  public SpecialOrdersBoard(string board_type = "")
    : base(0, 0, 0, 0, true)
  {
    SpecialOrder.UpdateAvailableSpecialOrders(board_type, false);
    this.boardType = board_type;
    this.billboardTexture = !(this.boardType == "Qi") ? Game1.temporaryContent.Load<Texture2D>("LooseSprites\\SpecialOrdersBoard") : Game1.temporaryContent.Load<Texture2D>("LooseSprites\\SpecialOrdersBoard");
    this.width = 1352;
    this.height = 792;
    Vector2 centeringOnScreen = Utility.getTopLeftPositionForCenteringOnScreen(this.width, this.height);
    this.xPositionOnScreen = (int) centeringOnScreen.X;
    this.yPositionOnScreen = (int) centeringOnScreen.Y;
    this.acceptLeftQuestButton = new ClickableComponent(new Rectangle(this.xPositionOnScreen + this.width / 4 - 128 /*0x80*/, this.yPositionOnScreen + this.height - 128 /*0x80*/, (int) Game1.dialogueFont.MeasureString(Game1.content.LoadString("Strings\\UI:AcceptQuest")).X + 24, (int) Game1.dialogueFont.MeasureString(Game1.content.LoadString("Strings\\UI:AcceptQuest")).Y + 24), "")
    {
      myID = 0,
      leftNeighborID = -99998,
      rightNeighborID = -99998,
      upNeighborID = -99998,
      downNeighborID = -99998
    };
    this.acceptRightQuestButton = new ClickableComponent(new Rectangle(this.xPositionOnScreen + this.width * 3 / 4 - 128 /*0x80*/, this.yPositionOnScreen + this.height - 128 /*0x80*/, (int) Game1.dialogueFont.MeasureString(Game1.content.LoadString("Strings\\UI:AcceptQuest")).X + 24, (int) Game1.dialogueFont.MeasureString(Game1.content.LoadString("Strings\\UI:AcceptQuest")).Y + 24), "")
    {
      myID = 1,
      leftNeighborID = -99998,
      rightNeighborID = -99998,
      upNeighborID = -99998,
      downNeighborID = -99998
    };
    this.leftOrder = Game1.player.team.GetAvailableSpecialOrder(type: this.GetOrderType());
    this.rightOrder = Game1.player.team.GetAvailableSpecialOrder(1, this.GetOrderType());
    this.upperRightCloseButton = new ClickableTextureComponent(new Rectangle(this.xPositionOnScreen + this.width - 20, this.yPositionOnScreen, 48 /*0x30*/, 48 /*0x30*/), Game1.mouseCursors, new Rectangle(337, 494, 12, 12), 4f);
    Game1.playSound("bigSelect");
    this.UpdateButtons();
    if (!Game1.options.SnappyMenus)
      return;
    this.populateClickableComponentList();
    this.snapToDefaultClickableComponent();
  }

  public virtual void UpdateButtons()
  {
    if (this.leftOrder == null)
      this.acceptLeftQuestButton.visible = false;
    if (this.rightOrder == null)
      this.acceptRightQuestButton.visible = false;
    if (!Game1.player.team.acceptedSpecialOrderTypes.Contains(this.GetOrderType()))
      return;
    this.acceptLeftQuestButton.visible = false;
    this.acceptRightQuestButton.visible = false;
  }

  public override void snapToDefaultClickableComponent()
  {
    this.currentlySnappedComponent = this.getComponentWithID(0);
    this.snapCursorToCurrentSnappedComponent();
  }

  /// <inheritdoc />
  public override void gameWindowSizeChanged(Rectangle oldBounds, Rectangle newBounds)
  {
    base.gameWindowSizeChanged(oldBounds, newBounds);
    Game1.activeClickableMenu = (IClickableMenu) new SpecialOrdersBoard(this.boardType);
  }

  /// <inheritdoc />
  public override void receiveRightClick(int x, int y, bool playSound = true)
  {
    Game1.playSound("bigDeSelect");
    this.exitThisMenu();
  }

  /// <inheritdoc />
  public override void receiveLeftClick(int x, int y, bool playSound = true)
  {
    base.receiveLeftClick(x, y, playSound);
    if (this.acceptLeftQuestButton.visible && this.acceptLeftQuestButton.containsPoint(x, y))
    {
      Game1.playSound("newArtifact");
      if (this.leftOrder == null)
        return;
      Game1.player.team.acceptedSpecialOrderTypes.Add(this.GetOrderType());
      SpecialOrder leftOrder = this.leftOrder;
      Game1.player.team.AddSpecialOrder(leftOrder.questKey.Value, new int?(leftOrder.generationSeed.Value));
      Game1.multiplayer.globalChatInfoMessage("AcceptedSpecialOrder", Game1.player.Name, TokenStringBuilder.SpecialOrderName(leftOrder.questKey.Value));
      this.UpdateButtons();
    }
    else
    {
      if (!this.acceptRightQuestButton.visible || !this.acceptRightQuestButton.containsPoint(x, y))
        return;
      Game1.playSound("newArtifact");
      if (this.rightOrder == null)
        return;
      Game1.player.team.acceptedSpecialOrderTypes.Add(this.GetOrderType());
      SpecialOrder rightOrder = this.rightOrder;
      Game1.player.team.AddSpecialOrder(rightOrder.questKey.Value, new int?(rightOrder.generationSeed.Value));
      Game1.multiplayer.globalChatInfoMessage("AcceptedSpecialOrder", Game1.player.Name, TokenStringBuilder.SpecialOrderName(rightOrder.questKey.Value));
      this.UpdateButtons();
    }
  }

  public string GetOrderType() => this.boardType;

  /// <inheritdoc />
  public override void performHoverAction(int x, int y)
  {
    base.performHoverAction(x, y);
    if (Game1.questOfTheDay == null || Game1.questOfTheDay.accepted.Value)
      return;
    float scale1 = this.acceptLeftQuestButton.scale;
    this.acceptLeftQuestButton.scale = this.acceptLeftQuestButton.bounds.Contains(x, y) ? 1.5f : 1f;
    if ((double) this.acceptLeftQuestButton.scale > (double) scale1)
      Game1.playSound("Cowboy_gunshot");
    float scale2 = this.acceptRightQuestButton.scale;
    this.acceptRightQuestButton.scale = this.acceptRightQuestButton.bounds.Contains(x, y) ? 1.5f : 1f;
    if ((double) this.acceptRightQuestButton.scale <= (double) scale2)
      return;
    Game1.playSound("Cowboy_gunshot");
  }

  /// <inheritdoc />
  public override void draw(SpriteBatch b)
  {
    if (!Game1.options.showClearBackgrounds)
      b.Draw(Game1.fadeToBlackRect, Game1.graphics.GraphicsDevice.Viewport.Bounds, Color.Black * 0.75f);
    b.Draw(this.billboardTexture, new Vector2((float) this.xPositionOnScreen, (float) this.yPositionOnScreen), new Rectangle?(new Rectangle(0, this.boardType == "Qi" ? 198 : 0, 338, 198)), Color.White, 0.0f, Vector2.Zero, 4f, SpriteEffects.None, 1f);
    if (this.leftOrder != null && this.leftOrder.IsIslandOrder())
      b.Draw(this.billboardTexture, new Vector2((float) this.xPositionOnScreen, (float) this.yPositionOnScreen), new Rectangle?(new Rectangle(338, 0, 169, 198)), Color.White, 0.0f, Vector2.Zero, 4f, SpriteEffects.None, 1f);
    if (this.rightOrder != null && this.rightOrder.IsIslandOrder())
      b.Draw(this.billboardTexture, new Vector2((float) (this.xPositionOnScreen + 676), (float) this.yPositionOnScreen), new Rectangle?(new Rectangle(507, 0, 169, 198)), Color.White, 0.0f, Vector2.Zero, 4f, SpriteEffects.None, 1f);
    if (!Game1.player.team.acceptedSpecialOrderTypes.Contains(this.GetOrderType()))
      SpriteText.drawStringWithScrollCenteredAt(b, Game1.content.LoadString("Strings\\UI:ChooseOne"), this.xPositionOnScreen + this.width / 2, Math.Max(10, this.yPositionOnScreen - 70), SpriteText.getWidthOfString(Game1.content.LoadString("Strings\\UI:ChooseOne") + "W"));
    if (this.leftOrder != null)
    {
      SpecialOrder leftOrder = this.leftOrder;
      this.DrawQuestDetails(b, leftOrder, this.xPositionOnScreen + 64 /*0x40*/ + 32 /*0x20*/);
    }
    if (this.rightOrder != null)
    {
      SpecialOrder rightOrder = this.rightOrder;
      this.DrawQuestDetails(b, rightOrder, this.xPositionOnScreen + 704 + 32 /*0x20*/);
    }
    if (this.acceptLeftQuestButton.visible)
    {
      IClickableMenu.drawTextureBox(b, Game1.mouseCursors, new Rectangle(403, 373, 9, 9), this.acceptLeftQuestButton.bounds.X, this.acceptLeftQuestButton.bounds.Y, this.acceptLeftQuestButton.bounds.Width, this.acceptLeftQuestButton.bounds.Height, (double) this.acceptLeftQuestButton.scale > 1.0 ? Color.LightPink : Color.White, 4f * this.acceptLeftQuestButton.scale);
      Utility.drawTextWithShadow(b, Game1.content.LoadString("Strings\\UI:AcceptQuest"), Game1.dialogueFont, new Vector2((float) (this.acceptLeftQuestButton.bounds.X + 12), (float) (this.acceptLeftQuestButton.bounds.Y + (LocalizedContentManager.CurrentLanguageLatin ? 16 /*0x10*/ : 12))), Game1.textColor);
    }
    if (this.acceptRightQuestButton.visible)
    {
      IClickableMenu.drawTextureBox(b, Game1.mouseCursors, new Rectangle(403, 373, 9, 9), this.acceptRightQuestButton.bounds.X, this.acceptRightQuestButton.bounds.Y, this.acceptRightQuestButton.bounds.Width, this.acceptRightQuestButton.bounds.Height, (double) this.acceptRightQuestButton.scale > 1.0 ? Color.LightPink : Color.White, 4f * this.acceptRightQuestButton.scale);
      Utility.drawTextWithShadow(b, Game1.content.LoadString("Strings\\UI:AcceptQuest"), Game1.dialogueFont, new Vector2((float) (this.acceptRightQuestButton.bounds.X + 12), (float) (this.acceptRightQuestButton.bounds.Y + (LocalizedContentManager.CurrentLanguageLatin ? 16 /*0x10*/ : 12))), Game1.textColor);
    }
    base.draw(b);
    Game1.mouseCursorTransparency = 1f;
    if (Game1.options.SnappyMenus && !this.acceptLeftQuestButton.visible && !this.acceptRightQuestButton.visible)
      return;
    this.drawMouse(b);
  }

  public KeyValuePair<Texture2D, Rectangle>? GetPortraitForRequester(string requester_name)
  {
    if (requester_name == null)
      return new KeyValuePair<Texture2D, Rectangle>?();
    for (int index = 0; index < this.emojiIndices.Length; ++index)
    {
      if (this.emojiIndices[index] == requester_name)
        return new KeyValuePair<Texture2D, Rectangle>?(new KeyValuePair<Texture2D, Rectangle>(ChatBox.emojiTexture, new Rectangle(index % 14 * 9, 99 + index / 14 * 9, 9, 9)));
    }
    return new KeyValuePair<Texture2D, Rectangle>?();
  }

  public void DrawQuestDetails(SpriteBatch b, SpecialOrder order, int x)
  {
    bool flag1 = false;
    bool flag2 = false;
    foreach (SpecialOrder specialOrder in Game1.player.team.specialOrders)
    {
      if (specialOrder.questState.Value == SpecialOrderStatus.InProgress)
      {
        foreach (SpecialOrder availableSpecialOrder in Game1.player.team.availableSpecialOrders)
        {
          if (!(availableSpecialOrder.orderType.Value != this.GetOrderType()) && specialOrder.questKey.Value == availableSpecialOrder.questKey.Value)
          {
            if (order.questKey.Value != specialOrder.questKey.Value)
              flag1 = true;
            flag2 = true;
            break;
          }
        }
        if (flag2)
          break;
      }
    }
    if (!flag2 && Game1.player.team.acceptedSpecialOrderTypes.Contains(this.GetOrderType()))
      flag1 = true;
    SpriteFont spriteFont = LocalizedContentManager.CurrentLanguageCode == LocalizedContentManager.LanguageCode.ko ? Game1.smallFont : Game1.dialogueFont;
    Color color = Game1.textColor;
    float shadowIntensity = 0.5f;
    float num1 = 1f;
    if (flag1)
    {
      color = Game1.textColor * 0.25f;
      shadowIntensity = 0.0f;
      num1 = 0.25f;
    }
    if (this.boardType == "Qi")
    {
      color = Color.White;
      shadowIntensity = 0.0f;
      if (flag1)
      {
        color = Color.White * 0.25f;
        num1 = 0.25f;
      }
    }
    int y1 = this.yPositionOnScreen + 128 /*0x80*/;
    string name = order.GetName();
    KeyValuePair<Texture2D, Rectangle>? portraitForRequester = this.GetPortraitForRequester(order.requester.Value);
    if (portraitForRequester.HasValue)
      Utility.drawWithShadow(b, portraitForRequester.Value.Key, new Vector2((float) x, (float) y1), portraitForRequester.Value.Value, Color.White * num1, 0.0f, Vector2.Zero, 4f, shadowIntensity: shadowIntensity * 0.6f);
    Utility.drawTextWithShadow(b, name, spriteFont, new Vector2((float) (x + 256 /*0x0100*/) - spriteFont.MeasureString(name).X / 2f, (float) y1), color, shadowIntensity: shadowIntensity);
    if (this.boardType == "" && Game1.player.team.completedSpecialOrders.Contains(order.questKey.Value))
      b.Draw(Game1.mouseCursors_1_6, new Vector2((float) x, (float) ((double) this.yPositionOnScreen + 576.0 + 32.0 + 8.0)), new Rectangle?(new Rectangle(404, 213, 11, 11)), Color.White, 0.0f, Vector2.Zero, 4f, SpriteEffects.None, 0.9f);
    string description = order.GetDescription();
    string text = Game1.parseText(description, spriteFont, 512 /*0x0200*/);
    float y2 = spriteFont.MeasureString(text).Y;
    float scale = 1f;
    for (float num2 = 400f; (double) y2 > (double) num2 && (double) scale > 0.25; y2 = spriteFont.MeasureString(text).Y)
    {
      scale -= 0.05f;
      text = Game1.parseText(description, spriteFont, (int) (512.0 / (double) scale));
    }
    Utility.drawTextWithShadow(b, text, spriteFont, new Vector2((float) x, (float) (this.yPositionOnScreen + 192 /*0xC0*/)), color, scale, shadowIntensity: shadowIntensity);
    if (flag1)
      return;
    int daysLeft = order.GetDaysLeft();
    int x1 = x;
    int y3 = this.yPositionOnScreen + 576;
    Utility.drawWithShadow(b, Game1.mouseCursors, new Vector2((float) x1, (float) y3), new Rectangle(410, 501, 9, 9), Color.White, 0.0f, Vector2.Zero, 4f, layerDepth: 0.99f, shadowIntensity: shadowIntensity * 0.6f);
    Utility.drawTextWithShadow(b, Game1.parseText(daysLeft > 1 ? Game1.content.LoadString("Strings\\StringsFromCSFiles:QuestLog.cs.11374", (object) daysLeft) : Game1.content.LoadString("Strings\\StringsFromCSFiles:QuestLog.cs.11375", (object) daysLeft), Game1.dialogueFont, this.width - 128 /*0x80*/), Game1.dialogueFont, new Vector2((float) (x1 + 48 /*0x30*/), (float) y3), color, shadowIntensity: shadowIntensity);
    if (this.boardType == "Qi")
    {
      int num3 = -1;
      GemsReward gemsReward1 = (GemsReward) null;
      foreach (OrderReward reward in order.rewards)
      {
        if (reward is GemsReward gemsReward2)
        {
          gemsReward1 = gemsReward2;
          break;
        }
      }
      if (gemsReward1 != null)
        num3 = gemsReward1.amount.Value;
      if (num3 == -1)
        return;
      Utility.drawWithShadow(b, Game1.objectSpriteSheet, new Vector2((float) ((double) x1 + 512.0 / (double) scale - (double) Game1.dialogueFont.MeasureString(num3.ToString() ?? "").X - 12.0 - 60.0), (float) (y3 - 8)), new Rectangle(288, 561, 15, 15), Color.White, 0.0f, Vector2.Zero, 4f, layerDepth: 0.99f, shadowIntensity: shadowIntensity * 0.6f);
      Utility.drawTextWithShadow(b, Game1.parseText(num3.ToString() ?? "", Game1.dialogueFont, this.width - 128 /*0x80*/), Game1.dialogueFont, new Vector2((float) ((double) x1 + 512.0 / (double) scale - (double) Game1.dialogueFont.MeasureString(num3.ToString() ?? "").X - 4.0), (float) y3), color, shadowIntensity: shadowIntensity);
      Utility.drawTextWithShadow(b, Game1.parseText(Utility.loadStringShort("StringsFromCSFiles", "QuestLog.cs.11376"), Game1.dialogueFont, this.width - 128 /*0x80*/), Game1.dialogueFont, new Vector2((float) ((double) x1 + 512.0 / (double) scale - (double) Game1.dialogueFont.MeasureString(Utility.loadStringShort("StringsFromCSFiles", "QuestLog.cs.11376")).X + 8.0), (float) (y3 - 60)), color * 0.6f, shadowIntensity: shadowIntensity);
    }
    else
    {
      StardewValley.Object @object = (StardewValley.Object) null;
      foreach (OrderReward reward in order.rewards)
      {
        if (reward is ObjectReward objectReward)
        {
          @object = objectReward.objectInstance;
          break;
        }
      }
      if (@object == null)
        return;
      Utility.drawWithShadow(b, ItemRegistry.GetDataOrErrorItem(@object.QualifiedItemId).GetTexture(), new Vector2((float) ((double) x1 + 512.0 / (double) scale - (double) Game1.dialogueFont.MeasureString(@object.Stack.ToString() ?? "").X - 12.0 - 60.0), (float) (y3 - 8)), ItemRegistry.GetDataOrErrorItem(@object.QualifiedItemId).GetSourceRect(), Color.White, 0.0f, Vector2.Zero, 4f, layerDepth: 0.99f, shadowIntensity: shadowIntensity * 0.6f);
      Utility.drawTextWithShadow(b, Game1.parseText(@object.Stack.ToString() ?? "", Game1.dialogueFont, this.width - 128 /*0x80*/), Game1.dialogueFont, new Vector2((float) ((double) x1 + 512.0 / (double) scale - (double) Game1.dialogueFont.MeasureString(@object.Stack.ToString() ?? "").X - 4.0), (float) y3), color, shadowIntensity: shadowIntensity);
      Utility.drawTextWithShadow(b, Game1.parseText(ItemRegistry.GetDataOrErrorItem(@object.QualifiedItemId).DisplayName, Game1.dialogueFont, this.width - 128 /*0x80*/), Game1.dialogueFont, new Vector2((float) ((double) x1 + 512.0 / (double) scale - (double) Game1.dialogueFont.MeasureString(ItemRegistry.GetDataOrErrorItem(@object.QualifiedItemId).DisplayName).X + 8.0), (float) (y3 - 60)), color * 0.6f, shadowIntensity: shadowIntensity);
    }
  }
}
