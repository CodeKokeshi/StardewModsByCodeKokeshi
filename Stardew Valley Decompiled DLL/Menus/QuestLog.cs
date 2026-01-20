// Decompiled with JetBrains decompiler
// Type: StardewValley.Menus.QuestLog
// Assembly: Stardew Valley, Version=1.6.15.24356, Culture=neutral, PublicKeyToken=null
// MVID: AA1F513A-94F0-4EF6-A35F-2D5B4A3721BF
// Assembly location: D:\SteamLibrary\steamapps\common\Stardew Valley\Stardew Valley.dll
// XML documentation location: D:\SteamLibrary\steamapps\common\Stardew Valley\Stardew Valley.xml

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using StardewValley.BellsAndWhistles;
using StardewValley.Quests;
using StardewValley.SpecialOrders;
using StardewValley.SpecialOrders.Objectives;
using System;
using System.Collections.Generic;

#nullable disable
namespace StardewValley.Menus;

public class QuestLog : IClickableMenu
{
  public const int questsPerPage = 6;
  public const int region_forwardButton = 101;
  public const int region_backButton = 102;
  public const int region_rewardBox = 103;
  public const int region_cancelQuestButton = 104;
  protected List<List<IQuest>> pages;
  public List<ClickableComponent> questLogButtons;
  protected int currentPage;
  protected int questPage = -1;
  public ClickableTextureComponent forwardButton;
  public ClickableTextureComponent backButton;
  public ClickableTextureComponent rewardBox;
  public ClickableTextureComponent cancelQuestButton;
  protected IQuest _shownQuest;
  protected List<string> _objectiveText;
  protected float _contentHeight;
  protected float _scissorRectHeight;
  public float scrollAmount;
  public ClickableTextureComponent upArrow;
  public ClickableTextureComponent downArrow;
  public ClickableTextureComponent scrollBar;
  protected bool scrolling;
  public Rectangle scrollBarBounds;
  private string hoverText = "";

  public QuestLog()
    : base(0, 0, 0, 0, true)
  {
    Game1.dayTimeMoneyBox.DismissQuestPing();
    Game1.playSound("bigSelect");
    this.paginateQuests();
    this.width = 832;
    this.height = 576;
    if (LocalizedContentManager.CurrentLanguageCode == LocalizedContentManager.LanguageCode.ko || LocalizedContentManager.CurrentLanguageCode == LocalizedContentManager.LanguageCode.fr)
      this.height += 64 /*0x40*/;
    Vector2 centeringOnScreen = Utility.getTopLeftPositionForCenteringOnScreen(this.width, this.height);
    this.xPositionOnScreen = (int) centeringOnScreen.X;
    this.yPositionOnScreen = (int) centeringOnScreen.Y + 32 /*0x20*/;
    this.questLogButtons = new List<ClickableComponent>();
    for (int index = 0; index < 6; ++index)
      this.questLogButtons.Add(new ClickableComponent(new Rectangle(this.xPositionOnScreen + 16 /*0x10*/, this.yPositionOnScreen + 16 /*0x10*/ + index * ((this.height - 32 /*0x20*/) / 6), this.width - 32 /*0x20*/, (this.height - 32 /*0x20*/) / 6 + 4), index.ToString() ?? "")
      {
        myID = index,
        downNeighborID = -7777,
        upNeighborID = index > 0 ? index - 1 : -1,
        rightNeighborID = -7777,
        leftNeighborID = -7777,
        fullyImmutable = true
      });
    this.upperRightCloseButton = new ClickableTextureComponent(new Rectangle(this.xPositionOnScreen + this.width - 20, this.yPositionOnScreen - 8, 48 /*0x30*/, 48 /*0x30*/), Game1.mouseCursors, new Rectangle(337, 494, 12, 12), 4f);
    ClickableTextureComponent textureComponent1 = new ClickableTextureComponent(new Rectangle(this.xPositionOnScreen - 64 /*0x40*/, this.yPositionOnScreen + 8, 48 /*0x30*/, 44), Game1.mouseCursors, new Rectangle(352, 495, 12, 11), 4f);
    textureComponent1.myID = 102;
    textureComponent1.rightNeighborID = -7777;
    this.backButton = textureComponent1;
    ClickableTextureComponent textureComponent2 = new ClickableTextureComponent(new Rectangle(this.xPositionOnScreen + this.width + 64 /*0x40*/ - 48 /*0x30*/, this.yPositionOnScreen + this.height - 48 /*0x30*/, 48 /*0x30*/, 44), Game1.mouseCursors, new Rectangle(365, 495, 12, 11), 4f);
    textureComponent2.myID = 101;
    this.forwardButton = textureComponent2;
    ClickableTextureComponent textureComponent3 = new ClickableTextureComponent(new Rectangle(this.xPositionOnScreen + this.width / 2 - 80 /*0x50*/, this.yPositionOnScreen + this.height - 32 /*0x20*/ - 96 /*0x60*/, 96 /*0x60*/, 96 /*0x60*/), Game1.mouseCursors, new Rectangle(293, 360, 24, 24), 4f, true);
    textureComponent3.myID = 103;
    this.rewardBox = textureComponent3;
    ClickableTextureComponent textureComponent4 = new ClickableTextureComponent(new Rectangle(this.xPositionOnScreen + 4, this.yPositionOnScreen + this.height + 4, 48 /*0x30*/, 48 /*0x30*/), Game1.mouseCursors, new Rectangle(322, 498, 12, 12), 4f, true);
    textureComponent4.myID = 104;
    this.cancelQuestButton = textureComponent4;
    int x = this.xPositionOnScreen + this.width + 16 /*0x10*/;
    this.upArrow = new ClickableTextureComponent(new Rectangle(x, this.yPositionOnScreen + 96 /*0x60*/, 44, 48 /*0x30*/), Game1.mouseCursors, new Rectangle(421, 459, 11, 12), 4f);
    this.downArrow = new ClickableTextureComponent(new Rectangle(x, this.yPositionOnScreen + this.height - 64 /*0x40*/, 44, 48 /*0x30*/), Game1.mouseCursors, new Rectangle(421, 472, 11, 12), 4f);
    this.scrollBarBounds = new Rectangle();
    this.scrollBarBounds.X = this.upArrow.bounds.X + 12;
    this.scrollBarBounds.Width = 24;
    this.scrollBarBounds.Y = this.upArrow.bounds.Y + this.upArrow.bounds.Height + 4;
    this.scrollBarBounds.Height = this.downArrow.bounds.Y - 4 - this.scrollBarBounds.Y;
    this.scrollBar = new ClickableTextureComponent(new Rectangle(this.scrollBarBounds.X, this.scrollBarBounds.Y, 24, 40), Game1.mouseCursors, new Rectangle(435, 463, 6, 10), 4f);
    if (!Game1.options.SnappyMenus)
      return;
    this.populateClickableComponentList();
    this.snapToDefaultClickableComponent();
  }

  protected override void customSnapBehavior(int direction, int oldRegion, int oldID)
  {
    if (oldID >= 0 && oldID < 6 && this.questPage == -1)
    {
      switch (direction)
      {
        case 1:
          if (this.currentPage < this.pages.Count - 1)
          {
            this.currentlySnappedComponent = this.getComponentWithID(101);
            this.currentlySnappedComponent.leftNeighborID = oldID;
            break;
          }
          break;
        case 2:
          if (oldID < 5 && this.pages[this.currentPage].Count - 1 > oldID)
          {
            this.currentlySnappedComponent = this.getComponentWithID(oldID + 1);
            break;
          }
          break;
        case 3:
          if (this.currentPage > 0)
          {
            this.currentlySnappedComponent = this.getComponentWithID(102);
            this.currentlySnappedComponent.rightNeighborID = oldID;
            break;
          }
          break;
      }
    }
    else if (oldID == 102)
    {
      if (this.questPage != -1)
        return;
      this.currentlySnappedComponent = this.getComponentWithID(0);
    }
    this.snapCursorToCurrentSnappedComponent();
  }

  public override void snapToDefaultClickableComponent()
  {
    this.currentlySnappedComponent = this.getComponentWithID(0);
    this.snapCursorToCurrentSnappedComponent();
  }

  /// <inheritdoc />
  public override void receiveGamePadButton(Buttons button)
  {
    switch (button)
    {
      case Buttons.RightTrigger:
        if (this.questPage != -1 || this.currentPage >= this.pages.Count - 1)
          break;
        this.nonQuestPageForwardButton();
        break;
      case Buttons.LeftTrigger:
        if (this.questPage != -1 || this.currentPage <= 0)
          break;
        this.nonQuestPageBackButton();
        break;
    }
  }

  /// <summary>Get the paginated list of quests which should be shown in the quest log.</summary>
  protected virtual void paginateQuests()
  {
    this.pages = new List<List<IQuest>>();
    IList<IQuest> allQuests = this.GetAllQuests();
    int index1 = 0;
    while (index1 < allQuests.Count)
    {
      List<IQuest> questList = new List<IQuest>();
      for (int index2 = 0; index2 < 6 && index1 < allQuests.Count; ++index2)
      {
        questList.Add(allQuests[index1]);
        ++index1;
      }
      this.pages.Add(questList);
    }
    if (this.pages.Count == 0)
      this.pages.Add(new List<IQuest>());
    this.currentPage = Utility.Clamp(this.currentPage, 0, this.pages.Count - 1);
    this.questPage = -1;
  }

  /// <summary>Get the quests which should be shown in the quest log.</summary>
  protected virtual IList<IQuest> GetAllQuests()
  {
    List<IQuest> allQuests = new List<IQuest>();
    for (int index = Game1.player.team.specialOrders.Count - 1; index >= 0; --index)
    {
      SpecialOrder specialOrder = Game1.player.team.specialOrders[index];
      if (!specialOrder.IsHidden())
        allQuests.Add((IQuest) specialOrder);
    }
    for (int index = Game1.player.questLog.Count - 1; index >= 0; --index)
    {
      Quest quest = Game1.player.questLog[index];
      if (quest == null || quest.destroy.Value)
        Game1.player.questLog.RemoveAt(index);
      else if (!quest.IsHidden())
        allQuests.Add((IQuest) quest);
    }
    return (IList<IQuest>) allQuests;
  }

  public bool NeedsScroll()
  {
    return (this._shownQuest == null || !this._shownQuest.ShouldDisplayAsComplete()) && this.questPage != -1 && (double) this._contentHeight > (double) this._scissorRectHeight;
  }

  /// <inheritdoc />
  public override void receiveScrollWheelAction(int direction)
  {
    if (this.NeedsScroll())
    {
      float num = this.scrollAmount - (float) (Math.Sign(direction) * 64 /*0x40*/ / 2);
      if ((double) num < 0.0)
        num = 0.0f;
      if ((double) num > (double) this._contentHeight - (double) this._scissorRectHeight)
        num = this._contentHeight - this._scissorRectHeight;
      if ((double) this.scrollAmount != (double) num)
      {
        this.scrollAmount = num;
        Game1.playSound("shiny4");
        this.SetScrollBarFromAmount();
      }
    }
    base.receiveScrollWheelAction(direction);
  }

  /// <inheritdoc />
  public override void performHoverAction(int x, int y)
  {
    this.hoverText = "";
    base.performHoverAction(x, y);
    if (this.questPage == -1)
    {
      for (int index = 0; index < this.questLogButtons.Count; ++index)
      {
        if (this.pages.Count > 0 && this.pages[0].Count > index && this.questLogButtons[index].containsPoint(x, y) && !this.questLogButtons[index].containsPoint(Game1.getOldMouseX(), Game1.getOldMouseY()))
          Game1.playSound("Cowboy_gunshot");
      }
    }
    else if (this._shownQuest.CanBeCancelled() && this.cancelQuestButton.containsPoint(x, y))
      this.hoverText = Game1.content.LoadString("Strings\\StringsFromCSFiles:QuestLog.cs.11364");
    this.forwardButton.tryHover(x, y, 0.2f);
    this.backButton.tryHover(x, y, 0.2f);
    this.cancelQuestButton.tryHover(x, y, 0.2f);
    if (!this.NeedsScroll())
      return;
    this.upArrow.tryHover(x, y);
    this.downArrow.tryHover(x, y);
    this.scrollBar.tryHover(x, y);
  }

  /// <inheritdoc />
  public override void receiveKeyPress(Keys key)
  {
    if (Game1.isAnyGamePadButtonBeingPressed() && this.questPage != -1 && Game1.options.doesInputListContain(Game1.options.menuButton, key))
      this.exitQuestPage();
    else
      base.receiveKeyPress(key);
    if (!Game1.options.doesInputListContain(Game1.options.journalButton, key) || !this.readyToClose())
      return;
    Game1.exitActiveMenu();
    Game1.playSound("bigDeSelect");
  }

  private void nonQuestPageForwardButton()
  {
    ++this.currentPage;
    Game1.playSound("shwip");
    if (!Game1.options.SnappyMenus || this.currentPage != this.pages.Count - 1)
      return;
    this.currentlySnappedComponent = this.getComponentWithID(0);
    this.snapCursorToCurrentSnappedComponent();
  }

  private void nonQuestPageBackButton()
  {
    --this.currentPage;
    Game1.playSound("shwip");
    if (!Game1.options.SnappyMenus || this.currentPage != 0)
      return;
    this.currentlySnappedComponent = this.getComponentWithID(0);
    this.snapCursorToCurrentSnappedComponent();
  }

  /// <inheritdoc />
  public override void leftClickHeld(int x, int y)
  {
    if (GameMenu.forcePreventClose)
      return;
    base.leftClickHeld(x, y);
    if (!this.scrolling)
      return;
    this.SetScrollFromY(y);
  }

  /// <inheritdoc />
  public override void releaseLeftClick(int x, int y)
  {
    if (GameMenu.forcePreventClose)
      return;
    base.releaseLeftClick(x, y);
    this.scrolling = false;
  }

  public virtual void SetScrollFromY(int y)
  {
    int y1 = this.scrollBar.bounds.Y;
    this.scrollAmount = Utility.Clamp((float) (y - this.scrollBarBounds.Y) / (float) (this.scrollBarBounds.Height - this.scrollBar.bounds.Height), 0.0f, 1f) * (this._contentHeight - this._scissorRectHeight);
    this.SetScrollBarFromAmount();
    int y2 = this.scrollBar.bounds.Y;
    if (y1 == y2)
      return;
    Game1.playSound("shiny4");
  }

  public void UpArrowPressed()
  {
    this.upArrow.scale = this.upArrow.baseScale;
    this.scrollAmount -= 64f;
    if ((double) this.scrollAmount < 0.0)
      this.scrollAmount = 0.0f;
    this.SetScrollBarFromAmount();
  }

  public void DownArrowPressed()
  {
    this.downArrow.scale = this.downArrow.baseScale;
    this.scrollAmount += 64f;
    if ((double) this.scrollAmount > (double) this._contentHeight - (double) this._scissorRectHeight)
      this.scrollAmount = this._contentHeight - this._scissorRectHeight;
    this.SetScrollBarFromAmount();
  }

  private void SetScrollBarFromAmount()
  {
    if (!this.NeedsScroll())
    {
      this.scrollAmount = 0.0f;
    }
    else
    {
      if ((double) this.scrollAmount < 8.0)
        this.scrollAmount = 0.0f;
      if ((double) this.scrollAmount > (double) this._contentHeight - (double) this._scissorRectHeight - 8.0)
        this.scrollAmount = this._contentHeight - this._scissorRectHeight;
      this.scrollBar.bounds.Y = (int) ((double) this.scrollBarBounds.Y + (double) (this.scrollBarBounds.Height - this.scrollBar.bounds.Height) / (double) Math.Max(1f, this._contentHeight - this._scissorRectHeight) * (double) this.scrollAmount);
    }
  }

  public override void applyMovementKey(int direction)
  {
    base.applyMovementKey(direction);
    if (!this.NeedsScroll())
      return;
    if (direction != 0)
    {
      if (direction != 2)
        return;
      this.DownArrowPressed();
    }
    else
      this.UpArrowPressed();
  }

  /// <inheritdoc />
  public override void receiveLeftClick(int x, int y, bool playSound = true)
  {
    base.receiveLeftClick(x, y, playSound);
    if (Game1.activeClickableMenu == null)
      return;
    if (this.questPage == -1)
    {
      for (int index = 0; index < this.questLogButtons.Count; ++index)
      {
        if (this.pages.Count > 0 && this.pages[this.currentPage].Count > index && this.questLogButtons[index].containsPoint(x, y))
        {
          Game1.playSound("smallSelect");
          this.questPage = index;
          this._shownQuest = this.pages[this.currentPage][index];
          this._objectiveText = this._shownQuest.GetObjectiveDescriptions();
          this._shownQuest.MarkAsViewed();
          this.scrollAmount = 0.0f;
          this.SetScrollBarFromAmount();
          if (!Game1.options.SnappyMenus)
            return;
          this.currentlySnappedComponent = this.getComponentWithID(102);
          this.currentlySnappedComponent.rightNeighborID = -7777;
          this.currentlySnappedComponent.downNeighborID = this.HasMoneyReward() ? 103 : (this._shownQuest.CanBeCancelled() ? 104 : -1);
          this.snapCursorToCurrentSnappedComponent();
          return;
        }
      }
      if (this.currentPage < this.pages.Count - 1 && this.forwardButton.containsPoint(x, y))
        this.nonQuestPageForwardButton();
      else if (this.currentPage > 0 && this.backButton.containsPoint(x, y))
      {
        this.nonQuestPageBackButton();
      }
      else
      {
        Game1.playSound("bigDeSelect");
        this.exitThisMenu();
      }
    }
    else
    {
      Quest shownQuest = this._shownQuest as Quest;
      int num = !this._shownQuest.IsTimedQuest() || this._shownQuest.GetDaysLeft() <= 0 || SpriteText.getWidthOfString(this._shownQuest.GetName()) <= this.width / 2 ? 0 : -48;
      if (this.questPage != -1 && this._shownQuest.ShouldDisplayAsComplete() && this._shownQuest.HasMoneyReward() && this.rewardBox.containsPoint(x, y + num))
      {
        Game1.player.Money += this._shownQuest.GetMoneyReward();
        Game1.playSound("purchaseRepeat");
        this._shownQuest.OnMoneyRewardClaimed();
      }
      else if (this.questPage != -1 && shownQuest != null && !shownQuest.completed.Value && shownQuest.canBeCancelled.Value && this.cancelQuestButton.containsPoint(x, y))
      {
        shownQuest.accepted.Value = false;
        if (shownQuest.dailyQuest.Value && shownQuest.dayQuestAccepted.Value == Game1.Date.TotalDays)
          Game1.player.acceptedDailyQuest.Set(false);
        Game1.player.questLog.Remove(shownQuest);
        this.pages[this.currentPage].RemoveAt(this.questPage);
        this.questPage = -1;
        Game1.playSound("trashcan");
        if (Game1.options.SnappyMenus && this.currentPage == 0)
        {
          this.currentlySnappedComponent = this.getComponentWithID(0);
          this.snapCursorToCurrentSnappedComponent();
        }
      }
      else if (!this.NeedsScroll() || this.backButton.containsPoint(x, y))
        this.exitQuestPage();
      if (!this.NeedsScroll())
        return;
      if (this.downArrow.containsPoint(x, y) && (double) this.scrollAmount < (double) this._contentHeight - (double) this._scissorRectHeight)
      {
        this.DownArrowPressed();
        Game1.playSound("shwip");
      }
      else if (this.upArrow.containsPoint(x, y) && (double) this.scrollAmount > 0.0)
      {
        this.UpArrowPressed();
        Game1.playSound("shwip");
      }
      else if (this.scrollBar.containsPoint(x, y))
        this.scrolling = true;
      else if (this.scrollBarBounds.Contains(x, y))
      {
        this.scrolling = true;
      }
      else
      {
        if (this.downArrow.containsPoint(x, y) || x <= this.xPositionOnScreen + this.width || x >= this.xPositionOnScreen + this.width + 128 /*0x80*/ || y <= this.yPositionOnScreen || y >= this.yPositionOnScreen + this.height)
          return;
        this.scrolling = true;
        this.leftClickHeld(x, y);
        this.releaseLeftClick(x, y);
      }
    }
  }

  public bool HasReward() => this._shownQuest.HasReward();

  public bool HasMoneyReward() => this._shownQuest.HasMoneyReward();

  public void exitQuestPage()
  {
    if (this._shownQuest.OnLeaveQuestPage())
      this.pages[this.currentPage].RemoveAt(this.questPage);
    this.questPage = -1;
    this.paginateQuests();
    Game1.playSound("shwip");
    if (!Game1.options.SnappyMenus)
      return;
    this.snapToDefaultClickableComponent();
  }

  /// <inheritdoc />
  public override void update(GameTime time)
  {
    base.update(time);
    if (this.questPage == -1 || !this.HasReward())
      return;
    this.rewardBox.scale = this.rewardBox.baseScale + Game1.dialogueButtonScale / 20f;
  }

  /// <inheritdoc />
  public override void draw(SpriteBatch b)
  {
    if (!Game1.options.showClearBackgrounds)
      b.Draw(Game1.fadeToBlackRect, Game1.graphics.GraphicsDevice.Viewport.Bounds, Color.Black * 0.75f);
    SpriteText.drawStringWithScrollCenteredAt(b, Game1.content.LoadString("Strings\\StringsFromCSFiles:QuestLog.cs.11373"), this.xPositionOnScreen + this.width / 2, this.yPositionOnScreen - 64 /*0x40*/);
    if (this.questPage == -1)
    {
      IClickableMenu.drawTextureBox(b, Game1.mouseCursors, new Rectangle(384, 373, 18, 18), this.xPositionOnScreen, this.yPositionOnScreen, this.width, this.height, Color.White, 4f);
      for (int index = 0; index < this.questLogButtons.Count; ++index)
      {
        if (this.pages.Count > 0 && this.pages[this.currentPage].Count > index)
        {
          IClickableMenu.drawTextureBox(b, Game1.mouseCursors, new Rectangle(384, 396, 15, 15), this.questLogButtons[index].bounds.X, this.questLogButtons[index].bounds.Y, this.questLogButtons[index].bounds.Width, this.questLogButtons[index].bounds.Height, this.questLogButtons[index].containsPoint(Game1.getOldMouseX(), Game1.getOldMouseY()) ? Color.Wheat : Color.White, 4f, false);
          if (this.pages[this.currentPage][index].ShouldDisplayAsNew() || this.pages[this.currentPage][index].ShouldDisplayAsComplete())
            Utility.drawWithShadow(b, Game1.mouseCursors, new Vector2((float) (this.questLogButtons[index].bounds.X + 64 /*0x40*/ + 4), (float) (this.questLogButtons[index].bounds.Y + 44)), new Rectangle(this.pages[this.currentPage][index].ShouldDisplayAsComplete() ? 341 : 317, 410, 23, 9), Color.White, 0.0f, new Vector2(11f, 4f), (float) (4.0 + (double) Game1.dialogueButtonScale * 10.0 / 250.0), layerDepth: 0.99f);
          else
            Utility.drawWithShadow(b, Game1.mouseCursors, new Vector2((float) (this.questLogButtons[index].bounds.X + 32 /*0x20*/), (float) (this.questLogButtons[index].bounds.Y + 28)), this.pages[this.currentPage][index].IsTimedQuest() ? new Rectangle(410, 501, 9, 9) : new Rectangle(395 + (this.pages[this.currentPage][index].IsTimedQuest() ? 3 : 0), 497, 3, 8), Color.White, 0.0f, Vector2.Zero, 4f, layerDepth: 0.99f);
          this.pages[this.currentPage][index].IsTimedQuest();
          SpriteText.drawString(b, this.pages[this.currentPage][index].GetName(), this.questLogButtons[index].bounds.X + 128 /*0x80*/ + 4, this.questLogButtons[index].bounds.Y + 20);
        }
      }
    }
    else
    {
      int widthOfString = SpriteText.getWidthOfString(this._shownQuest.GetName());
      if (widthOfString > this.width / 2)
      {
        IClickableMenu.drawTextureBox(b, Game1.mouseCursors, new Rectangle(384, 373, 18, 18), this.xPositionOnScreen, this.yPositionOnScreen, this.width, this.height + (this._shownQuest.ShouldDisplayAsComplete() ? 48 /*0x30*/ : 0), Color.White, 4f);
        SpriteText.drawStringHorizontallyCenteredAt(b, this._shownQuest.GetName(), this.xPositionOnScreen + this.width / 2, this.yPositionOnScreen + 32 /*0x20*/);
      }
      else
      {
        IClickableMenu.drawTextureBox(b, Game1.mouseCursors, new Rectangle(384, 373, 18, 18), this.xPositionOnScreen, this.yPositionOnScreen, this.width, this.height, Color.White, 4f);
        SpriteText.drawStringHorizontallyCenteredAt(b, this._shownQuest.GetName(), this.xPositionOnScreen + this.width / 2 + (!this._shownQuest.IsTimedQuest() || this._shownQuest.GetDaysLeft() <= 0 ? 0 : Math.Max(32 /*0x20*/, SpriteText.getWidthOfString(this._shownQuest.GetName()) / 3) - 32 /*0x20*/), this.yPositionOnScreen + 32 /*0x20*/);
      }
      float yOffset = 0.0f;
      if (this._shownQuest.IsTimedQuest() && this._shownQuest.GetDaysLeft() > 0)
      {
        int num = 0;
        if (widthOfString > this.width / 2)
        {
          num = 28;
          yOffset = 48f;
        }
        Utility.drawWithShadow(b, Game1.mouseCursors, new Vector2((float) (this.xPositionOnScreen + num + 32 /*0x20*/), (float) (this.yPositionOnScreen + 48 /*0x30*/ - 8) + yOffset), new Rectangle(410, 501, 9, 9), Color.White, 0.0f, Vector2.Zero, 4f, layerDepth: 0.99f);
        Utility.drawTextWithShadow(b, Game1.parseText(this.pages[this.currentPage][this.questPage].GetDaysLeft() > 1 ? Game1.content.LoadString("Strings\\StringsFromCSFiles:QuestLog.cs.11374", (object) this.pages[this.currentPage][this.questPage].GetDaysLeft()) : Game1.content.LoadString("Strings\\StringsFromCSFiles:Quest_FinalDay"), Game1.dialogueFont, this.width - 128 /*0x80*/), Game1.dialogueFont, new Vector2((float) (this.xPositionOnScreen + num + 80 /*0x50*/), (float) (this.yPositionOnScreen + 48 /*0x30*/ - 8) + yOffset), Game1.textColor);
      }
      string text1 = Game1.parseText(this._shownQuest.GetDescription(), Game1.dialogueFont, this.width - 128 /*0x80*/);
      Rectangle scissorRectangle = b.GraphicsDevice.ScissorRectangle;
      Vector2 vector2 = Game1.dialogueFont.MeasureString(text1);
      Rectangle scissor_rect = new Rectangle()
      {
        X = this.xPositionOnScreen + 32 /*0x20*/,
        Y = this.yPositionOnScreen + 96 /*0x60*/ + (int) yOffset
      };
      scissor_rect.Height = this.yPositionOnScreen + this.height - 32 /*0x20*/ - scissor_rect.Y;
      scissor_rect.Width = this.width - 64 /*0x40*/;
      this._scissorRectHeight = (float) scissor_rect.Height;
      Rectangle screen = Utility.ConstrainScissorRectToScreen(scissor_rect);
      b.End();
      SpriteBatch spriteBatch = b;
      BlendState alphaBlend = BlendState.AlphaBlend;
      SamplerState pointClamp = SamplerState.PointClamp;
      RasterizerState rasterizerState = new RasterizerState();
      rasterizerState.ScissorTestEnable = true;
      Matrix? transformMatrix = new Matrix?();
      spriteBatch.Begin(blendState: alphaBlend, samplerState: pointClamp, rasterizerState: rasterizerState, transformMatrix: transformMatrix);
      Game1.graphics.GraphicsDevice.ScissorRectangle = screen;
      Utility.drawTextWithShadow(b, text1, Game1.dialogueFont, new Vector2((float) (this.xPositionOnScreen + 64 /*0x40*/), (float) ((double) this.yPositionOnScreen - (double) this.scrollAmount + 96.0) + yOffset), Game1.textColor);
      float y = (float) ((double) (this.yPositionOnScreen + 96 /*0x60*/) + (double) vector2.Y + 32.0) - this.scrollAmount + yOffset;
      if (this._shownQuest.ShouldDisplayAsComplete())
      {
        b.End();
        b.GraphicsDevice.ScissorRectangle = scissorRectangle;
        b.Begin(blendState: BlendState.AlphaBlend, samplerState: SamplerState.PointClamp);
        SpriteText.drawString(b, Game1.content.LoadString("Strings\\StringsFromCSFiles:QuestLog.cs.11376"), this.xPositionOnScreen + 32 /*0x20*/ + 4, this.rewardBox.bounds.Y + 21 + 4 + (int) yOffset);
        this.rewardBox.draw(b, Color.White, 0.9f, yOffset: (int) yOffset);
        if (this.HasMoneyReward())
        {
          b.Draw(Game1.mouseCursors, new Vector2((float) (this.rewardBox.bounds.X + 16 /*0x10*/), (float) (this.rewardBox.bounds.Y + 16 /*0x10*/) - Game1.dialogueButtonScale / 2f + yOffset), new Rectangle?(new Rectangle(280, 410, 16 /*0x10*/, 16 /*0x10*/)), Color.White, 0.0f, Vector2.Zero, 4f, SpriteEffects.None, 1f);
          SpriteText.drawString(b, Game1.content.LoadString("Strings\\StringsFromCSFiles:LoadGameMenu.cs.11020", (object) this._shownQuest.GetMoneyReward()), this.xPositionOnScreen + 448, this.rewardBox.bounds.Y + 21 + 4 + (int) yOffset);
        }
      }
      else
      {
        for (int index1 = 0; index1 < this._objectiveText.Count; ++index1)
        {
          string text2 = this._objectiveText[index1];
          int num1 = this.width - 192 /*0xC0*/;
          SpriteFont dialogueFont = Game1.dialogueFont;
          int width1 = num1;
          string text3 = Game1.parseText(text2, dialogueFont, width1);
          int num2 = !(this._shownQuest is SpecialOrder shownQuest1) ? 0 : (shownQuest1.objectives[index1].IsComplete() ? 1 : 0);
          Color color1 = Game1.unselectedOptionColor;
          if (num2 == 0)
          {
            color1 = Color.DarkBlue;
            Utility.drawWithShadow(b, Game1.mouseCursors, new Vector2((float) (this.xPositionOnScreen + 96 /*0x60*/) + (float) (8.0 * (double) Game1.dialogueButtonScale / 10.0), y), new Rectangle(412, 495, 5, 4), Color.White, 1.57079637f, Vector2.Zero);
          }
          Utility.drawTextWithShadow(b, text3, Game1.dialogueFont, new Vector2((float) (this.xPositionOnScreen + 128 /*0x80*/), y - 8f), color1);
          y += Game1.dialogueFont.MeasureString(text3).Y;
          if (this._shownQuest is SpecialOrder shownQuest2)
          {
            OrderObjective objective = shownQuest2.objectives[index1];
            if (objective.GetMaxCount() > 1 && objective.ShouldShowProgress())
            {
              Color color2 = Color.DarkRed;
              Color color3 = Color.Red;
              if (objective.GetCount() >= objective.GetMaxCount())
              {
                color3 = Color.LimeGreen;
                color2 = Color.Green;
              }
              int num3 = 64 /*0x40*/;
              int num4 = 160 /*0xA0*/;
              int num5 = 4;
              Rectangle rectangle1 = new Rectangle(0, 224 /*0xE0*/, 47, 12);
              Rectangle rectangle2 = new Rectangle(47, 224 /*0xE0*/, 1, 12);
              int num6 = 3;
              int num7 = 3;
              int width2 = 5;
              int num8 = objective.GetCount();
              string str1 = num8.ToString();
              num8 = objective.GetMaxCount();
              string str2 = num8.ToString();
              string text4 = $"{str1}/{str2}";
              int x1 = (int) Game1.dialogueFont.MeasureString($"{objective.GetMaxCount().ToString()}/{objective.GetMaxCount().ToString()}").X;
              int x2 = (int) Game1.dialogueFont.MeasureString(text4).X;
              int x3 = this.xPositionOnScreen + this.width - num3 - x2;
              int num9 = this.xPositionOnScreen + this.width - num3 - x1;
              Utility.drawTextWithShadow(b, text4, Game1.dialogueFont, new Vector2((float) x3, y), Color.DarkBlue);
              Rectangle rectangle3 = new Rectangle(this.xPositionOnScreen + num3, (int) y, this.width - num3 * 2 - num4, rectangle1.Height * 4);
              if (rectangle3.Right > num9 - 16 /*0x10*/)
              {
                int num10 = rectangle3.Right - (num9 - 16 /*0x10*/);
                rectangle3.Width -= num10;
              }
              b.Draw(Game1.mouseCursors2, new Rectangle(rectangle3.X, rectangle3.Y, width2 * 4, rectangle3.Height), new Rectangle?(new Rectangle(rectangle1.X, rectangle1.Y, width2, rectangle1.Height)), Color.White, 0.0f, Vector2.Zero, SpriteEffects.None, 0.5f);
              b.Draw(Game1.mouseCursors2, new Rectangle(rectangle3.X + width2 * 4, rectangle3.Y, rectangle3.Width - 2 * width2 * 4, rectangle3.Height), new Rectangle?(new Rectangle(rectangle1.X + width2, rectangle1.Y, rectangle1.Width - 2 * width2, rectangle1.Height)), Color.White, 0.0f, Vector2.Zero, SpriteEffects.None, 0.5f);
              b.Draw(Game1.mouseCursors2, new Rectangle(rectangle3.Right - width2 * 4, rectangle3.Y, width2 * 4, rectangle3.Height), new Rectangle?(new Rectangle(rectangle1.Right - width2, rectangle1.Y, width2, rectangle1.Height)), Color.White, 0.0f, Vector2.Zero, SpriteEffects.None, 0.5f);
              float num11 = (float) objective.GetCount() / (float) objective.GetMaxCount();
              if (objective.GetMaxCount() < num5)
                num5 = objective.GetMaxCount();
              rectangle3.X += 4 * num6;
              rectangle3.Width -= 4 * num6 * 2;
              for (int index2 = 1; index2 < num5; ++index2)
                b.Draw(Game1.mouseCursors2, new Vector2((float) rectangle3.X + (float) rectangle3.Width * ((float) index2 / (float) num5), (float) rectangle3.Y), new Rectangle?(rectangle2), Color.White, 0.0f, Vector2.Zero, 4f, SpriteEffects.None, 0.5f);
              rectangle3.Y += 4 * num7;
              rectangle3.Height -= 4 * num7 * 2;
              Rectangle destinationRectangle = new Rectangle(rectangle3.X, rectangle3.Y, (int) ((double) rectangle3.Width * (double) num11) - 4, rectangle3.Height);
              b.Draw(Game1.staminaRect, destinationRectangle, new Rectangle?(), color3, 0.0f, Vector2.Zero, SpriteEffects.None, (float) destinationRectangle.Y / 10000f);
              destinationRectangle.X = destinationRectangle.Right;
              destinationRectangle.Width = 4;
              b.Draw(Game1.staminaRect, destinationRectangle, new Rectangle?(), color2, 0.0f, Vector2.Zero, SpriteEffects.None, (float) destinationRectangle.Y / 10000f);
              y += (float) ((rectangle1.Height + 4) * 4);
            }
          }
          this._contentHeight = y + this.scrollAmount - (float) screen.Y;
        }
        b.End();
        b.GraphicsDevice.ScissorRectangle = scissorRectangle;
        b.Begin(blendState: BlendState.AlphaBlend, samplerState: SamplerState.PointClamp);
        if (this._shownQuest.CanBeCancelled())
          this.cancelQuestButton.draw(b);
        if (this.NeedsScroll())
        {
          if ((double) this.scrollAmount > 0.0)
            b.Draw(Game1.staminaRect, new Rectangle(screen.X, screen.Top, screen.Width, 4), Color.Black * 0.15f);
          if ((double) this.scrollAmount < (double) this._contentHeight - (double) this._scissorRectHeight)
            b.Draw(Game1.staminaRect, new Rectangle(screen.X, screen.Bottom - 4, screen.Width, 4), Color.Black * 0.15f);
        }
      }
    }
    if (this.NeedsScroll())
    {
      this.upArrow.draw(b);
      this.downArrow.draw(b);
      this.scrollBar.draw(b);
    }
    if (this.currentPage < this.pages.Count - 1 && this.questPage == -1)
      this.forwardButton.draw(b);
    if (this.currentPage > 0 || this.questPage != -1)
      this.backButton.draw(b);
    base.draw(b);
    Game1.mouseCursorTransparency = 1f;
    this.drawMouse(b);
    if (this.hoverText.Length <= 0)
      return;
    IClickableMenu.drawHoverText(b, this.hoverText, Game1.dialogueFont);
  }
}
