// Decompiled with JetBrains decompiler
// Type: StardewValley.Menus.DayTimeMoneyBox
// Assembly: Stardew Valley, Version=1.6.15.24356, Culture=neutral, PublicKeyToken=null
// MVID: AA1F513A-94F0-4EF6-A35F-2D5B4A3721BF
// Assembly location: D:\SteamLibrary\steamapps\common\Stardew Valley\Stardew Valley.dll
// XML documentation location: D:\SteamLibrary\steamapps\common\Stardew Valley\Stardew Valley.xml

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Netcode;
using StardewValley.BellsAndWhistles;
using StardewValley.ItemTypeDefinitions;
using StardewValley.Quests;
using System;
using System.Text;

#nullable disable
namespace StardewValley.Menus;

public class DayTimeMoneyBox : IClickableMenu
{
  public new const int width = 300;
  public new const int height = 284;
  public Vector2 position;
  private Rectangle sourceRect;
  public MoneyDial moneyDial = new MoneyDial(8);
  public int timeShakeTimer;
  public int moneyShakeTimer;
  public int questPulseTimer;
  public int whenToPulseTimer;
  public ClickableTextureComponent questButton;
  public ClickableTextureComponent zoomOutButton;
  public ClickableTextureComponent zoomInButton;
  private StringBuilder _hoverText = new StringBuilder();
  private StringBuilder _timeText = new StringBuilder();
  private StringBuilder _dateText = new StringBuilder();
  private StringBuilder _hours = new StringBuilder();
  private StringBuilder _padZeros = new StringBuilder();
  private StringBuilder _temp = new StringBuilder();
  private int _lastDayOfMonth = -1;
  private string _lastDayOfMonthString;
  private string _amString;
  private string _pmString;
  private int questNotificationTimer;
  private Texture2D questPingTexture;
  private Rectangle questPingSourceRect;
  private string questPingString;
  private int goldCoinCounter;
  private int goldCoinTimer;
  private string goldCoinString;
  private LocalizedContentManager.LanguageCode _languageCode = ~LocalizedContentManager.LanguageCode.en;
  public bool questsDirty;
  public int questPingTimer;

  public static int Width => 300;

  public DayTimeMoneyBox()
    : base(Game1.uiViewport.Width - 300 + 32 /*0x20*/, 8, 300, 284)
  {
    this.position = new Vector2((float) this.xPositionOnScreen, (float) this.yPositionOnScreen);
    this.sourceRect = new Rectangle(333, 431, 71, 43);
    this.questButton = new ClickableTextureComponent(new Rectangle(this.xPositionOnScreen + 220, this.yPositionOnScreen + 240 /*0xF0*/, 44, 46), Game1.mouseCursors, new Rectangle(383, 493, 11, 14), 4f);
    this.zoomOutButton = new ClickableTextureComponent(new Rectangle(this.xPositionOnScreen + 92, this.yPositionOnScreen + 244, 28, 32 /*0x20*/), Game1.mouseCursors, new Rectangle(177, 345, 7, 8), 4f);
    this.zoomInButton = new ClickableTextureComponent(new Rectangle(this.xPositionOnScreen + 124, this.yPositionOnScreen + 244, 28, 32 /*0x20*/), Game1.mouseCursors, new Rectangle(184, 345, 7, 8), 4f);
    this.questButton = new ClickableTextureComponent(new Rectangle(this.xPositionOnScreen + 220, this.yPositionOnScreen + 240 /*0xF0*/, 44, 46), Game1.mouseCursors, new Rectangle(383, 493, 11, 14), 4f);
    this.zoomOutButton = new ClickableTextureComponent(new Rectangle(this.xPositionOnScreen + 92, this.yPositionOnScreen + 244, 28, 32 /*0x20*/), Game1.mouseCursors, new Rectangle(177, 345, 7, 8), 4f);
    this.zoomInButton = new ClickableTextureComponent(new Rectangle(this.xPositionOnScreen + 124, this.yPositionOnScreen + 244, 28, 32 /*0x20*/), Game1.mouseCursors, new Rectangle(184, 345, 7, 8), 4f);
  }

  public override bool isWithinBounds(int x, int y)
  {
    return Game1.options.zoomButtons && (this.zoomInButton.containsPoint(x, y) || this.zoomOutButton.containsPoint(x, y)) || Game1.player.hasVisibleQuests && this.questButton.containsPoint(x, y);
  }

  /// <inheritdoc />
  public override void receiveLeftClick(int x, int y, bool playSound = true)
  {
    if (Game1.player.hasVisibleQuests && this.questButton.containsPoint(x, y) && Game1.player.CanMove && !Game1.dialogueUp && !Game1.eventUp && Game1.farmEvent == null)
      Game1.activeClickableMenu = (IClickableMenu) new QuestLog();
    if (!Game1.options.zoomButtons)
      return;
    if (this.zoomInButton.containsPoint(x, y) && (double) Game1.options.desiredBaseZoomLevel < 2.0)
    {
      int num = (int) Math.Round((double) Game1.options.desiredBaseZoomLevel * 100.0);
      Game1.options.desiredBaseZoomLevel = Math.Min(2f, (float) (num - num % 5 + 5) / 100f);
      Game1.forceSnapOnNextViewportUpdate = true;
      Game1.playSound("drumkit6");
    }
    else
    {
      if (!this.zoomOutButton.containsPoint(x, y) || (double) Game1.options.desiredBaseZoomLevel <= 0.75)
        return;
      int num = (int) Math.Round((double) Game1.options.desiredBaseZoomLevel * 100.0);
      Game1.options.desiredBaseZoomLevel = Math.Max(0.75f, (float) (num - num % 5 - 5) / 100f);
      Game1.forceSnapOnNextViewportUpdate = true;
      Program.gamePtr.refreshWindowSettings();
      Game1.playSound("drumkit6");
    }
  }

  public void gotGoldCoin(int amount)
  {
    this.goldCoinCounter += amount;
    this.goldCoinTimer = 4000;
    this.goldCoinString = "+" + Game1.content.LoadString("Strings\\StringsFromCSFiles:LoadGameMenu.cs.11020", (object) this.goldCoinCounter);
  }

  public void pingQuest(Quest quest)
  {
    if (!quest.dailyQuest.Value)
      return;
    this.questNotificationTimer = 3000;
    this.questPingString = (string) null;
    switch (quest)
    {
      case SlayMonsterQuest slayMonsterQuest:
        if (slayMonsterQuest.monster.Value?.Sprite?.Texture != null && (NetFieldBase<string, NetString>) slayMonsterQuest.monsterName != (NetString) null)
        {
          this.questPingTexture = slayMonsterQuest.monster.Value.Sprite.Texture;
          this.questPingSourceRect = new Rectangle(0, 5, 16 /*0x10*/, 16 /*0x10*/);
          if (slayMonsterQuest.monsterName.Equals((object) "Green Slime"))
            this.questPingSourceRect = new Rectangle(0, 264, 16 /*0x10*/, 16 /*0x10*/);
          else if (slayMonsterQuest.monsterName.Value.Contains("Frost"))
            this.questPingSourceRect = new Rectangle(16 /*0x10*/, 264, 16 /*0x10*/, 16 /*0x10*/);
          else if (slayMonsterQuest.monsterName.Value.Contains("Sludge"))
            this.questPingSourceRect = new Rectangle(32 /*0x20*/, 264, 16 /*0x10*/, 16 /*0x10*/);
          else if (slayMonsterQuest.monsterName.Value.Equals("Dust Spirit"))
            this.questPingSourceRect.Y = 9;
          else if (slayMonsterQuest.monsterName.Value.Contains("Crab"))
            this.questPingSourceRect = new Rectangle(48 /*0x30*/, 106, 16 /*0x10*/, 16 /*0x10*/);
          else if (slayMonsterQuest.monsterName.Value.Contains("Duggy"))
            this.questPingSourceRect = new Rectangle(0, 32 /*0x20*/, 16 /*0x10*/, 16 /*0x10*/);
          else if (slayMonsterQuest.monsterName.Equals((object) "Squid Kid"))
            this.questPingSourceRect = new Rectangle(0, 0, 16 /*0x10*/, 16 /*0x10*/);
          if (slayMonsterQuest.numberToKill.Value == slayMonsterQuest.numberKilled.Value)
            break;
          this.questPingString = $"{slayMonsterQuest.numberKilled.Value.ToString()}/{slayMonsterQuest.numberToKill.Value.ToString()}";
          break;
        }
        this.questNotificationTimer = 0;
        break;
      case ResourceCollectionQuest resourceCollectionQuest:
        ParsedItemData data1 = ItemRegistry.GetData(resourceCollectionQuest.ItemId.Value);
        this.questPingTexture = data1.GetTexture();
        this.questPingSourceRect = data1.GetSourceRect();
        if (resourceCollectionQuest.numberCollected.Value == resourceCollectionQuest.number.Value)
          break;
        this.questPingString = $"{resourceCollectionQuest.numberCollected.Value.ToString()}/{resourceCollectionQuest.number.Value.ToString()}";
        break;
      case FishingQuest fishingQuest:
        ParsedItemData data2 = ItemRegistry.GetData(fishingQuest.ItemId.Value);
        this.questPingTexture = data2.GetTexture();
        this.questPingSourceRect = data2.GetSourceRect();
        if (fishingQuest.numberFished.Value == fishingQuest.numberToFish.Value)
          break;
        this.questPingString = $"{fishingQuest.numberFished.Value.ToString()}/{fishingQuest.numberToFish.Value.ToString()}";
        break;
      case SocializeQuest socializeQuest:
        this.questPingTexture = Game1.mouseCursors_1_6;
        this.questPingSourceRect = new Rectangle(298, 237, 12, 12);
        if (socializeQuest.whoToGreet.Count == 0)
          break;
        this.questPingString = $"{(socializeQuest.total.Value - socializeQuest.whoToGreet.Count).ToString()}/{socializeQuest.total.Value.ToString()}";
        break;
      default:
        this.questNotificationTimer = 0;
        break;
    }
  }

  /// <inheritdoc />
  public override void receiveRightClick(int x, int y, bool playSound = true)
  {
    this.updatePosition();
  }

  /// <inheritdoc />
  public override void performHoverAction(int x, int y)
  {
    this.updatePosition();
    if (Game1.player.hasVisibleQuests && this.questButton.containsPoint(x, y))
    {
      this._hoverText.Clear();
      if (Game1.options.gamepadControls)
        this._hoverText.Append(Game1.content.LoadString("Strings\\UI:QuestButton_Hover_Console"));
      else
        this._hoverText.Append(Game1.content.LoadString("Strings\\UI:QuestButton_Hover", (object) Game1.options.journalButton[0].ToString()));
    }
    if (!Game1.options.zoomButtons)
      return;
    if (this.zoomInButton.containsPoint(x, y))
    {
      this._hoverText.Clear();
      this._hoverText.Append(Game1.content.LoadString("Strings\\UI:ZoomInButton_Hover"));
    }
    else
    {
      if (!this.zoomOutButton.containsPoint(x, y))
        return;
      this._hoverText.Clear();
      this._hoverText.Append(Game1.content.LoadString("Strings\\UI:ZoomOutButton_Hover"));
    }
  }

  public void drawMoneyBox(SpriteBatch b, int overrideX = -1, int overrideY = -1)
  {
    this.updatePosition();
    b.Draw(Game1.mouseCursors, (overrideY != -1 ? new Vector2(overrideX == -1 ? this.position.X : (float) overrideX, (float) (overrideY - 172)) : this.position) + new Vector2((float) (28 + (this.moneyShakeTimer > 0 ? Game1.random.Next(-3, 4) : 0)), (float) (172 + (this.moneyShakeTimer > 0 ? Game1.random.Next(-3, 4) : 0))), new Rectangle?(new Rectangle(340, 472, 65, 17)), Color.White, 0.0f, Vector2.Zero, 4f, SpriteEffects.None, 0.9f);
    this.moneyDial.draw(b, (overrideY != -1 ? new Vector2(overrideX == -1 ? this.position.X : (float) overrideX, (float) (overrideY - 172)) : this.position) + new Vector2((float) (68 + (this.moneyShakeTimer > 0 ? Game1.random.Next(-3, 4) : 0)), (float) (196 + (this.moneyShakeTimer > 0 ? Game1.random.Next(-3, 4) : 0))), Game1.player.Money);
    if (this.moneyShakeTimer <= 0)
      return;
    this.moneyShakeTimer -= Game1.currentGameTime.ElapsedGameTime.Milliseconds;
  }

  /// <inheritdoc />
  public override void update(GameTime time)
  {
    base.update(time);
    if (this._languageCode != LocalizedContentManager.CurrentLanguageCode)
    {
      this._languageCode = LocalizedContentManager.CurrentLanguageCode;
      this._amString = Game1.content.LoadString("Strings\\StringsFromCSFiles:DayTimeMoneyBox.cs.10370");
      this._pmString = Game1.content.LoadString("Strings\\StringsFromCSFiles:DayTimeMoneyBox.cs.10371");
    }
    TimeSpan elapsedGameTime;
    if (this.questPingTimer > 0)
    {
      int questPingTimer = this.questPingTimer;
      elapsedGameTime = time.ElapsedGameTime;
      int totalMilliseconds = (int) elapsedGameTime.TotalMilliseconds;
      this.questPingTimer = questPingTimer - totalMilliseconds;
    }
    if (this.questPingTimer < 0)
      this.questPingTimer = 0;
    if (this.questNotificationTimer > 0)
    {
      int notificationTimer = this.questNotificationTimer;
      elapsedGameTime = time.ElapsedGameTime;
      int totalMilliseconds = (int) elapsedGameTime.TotalMilliseconds;
      this.questNotificationTimer = notificationTimer - totalMilliseconds;
    }
    if (this.goldCoinTimer > 0)
    {
      int goldCoinTimer = this.goldCoinTimer;
      elapsedGameTime = time.ElapsedGameTime;
      int totalMilliseconds = (int) elapsedGameTime.TotalMilliseconds;
      this.goldCoinTimer = goldCoinTimer - totalMilliseconds;
      if (this.goldCoinTimer <= 0)
        this.goldCoinCounter = 0;
    }
    if (!this.questsDirty)
      return;
    if (Game1.player.hasPendingCompletedQuests)
      this.PingQuestLog();
    this.questsDirty = false;
  }

  public virtual void PingQuestLog() => this.questPingTimer = 6000;

  public virtual void DismissQuestPing() => this.questPingTimer = 0;

  /// <inheritdoc />
  public override void draw(SpriteBatch b)
  {
    SpriteFont font = LocalizedContentManager.CurrentLanguageCode == LocalizedContentManager.LanguageCode.ko ? Game1.smallFont : Game1.dialogueFont;
    this.updatePosition();
    TimeSpan timeSpan;
    if (this.timeShakeTimer > 0)
    {
      int timeShakeTimer = this.timeShakeTimer;
      timeSpan = Game1.currentGameTime.ElapsedGameTime;
      int milliseconds = timeSpan.Milliseconds;
      this.timeShakeTimer = timeShakeTimer - milliseconds;
    }
    if (this.questPulseTimer > 0)
    {
      int questPulseTimer = this.questPulseTimer;
      timeSpan = Game1.currentGameTime.ElapsedGameTime;
      int milliseconds = timeSpan.Milliseconds;
      this.questPulseTimer = questPulseTimer - milliseconds;
    }
    if (this.whenToPulseTimer >= 0)
    {
      int whenToPulseTimer = this.whenToPulseTimer;
      timeSpan = Game1.currentGameTime.ElapsedGameTime;
      int milliseconds = timeSpan.Milliseconds;
      this.whenToPulseTimer = whenToPulseTimer - milliseconds;
      if (this.whenToPulseTimer <= 0)
      {
        this.whenToPulseTimer = 3000;
        if (Game1.player.hasNewQuestActivity())
          this.questPulseTimer = 1000;
      }
    }
    b.Draw(Game1.mouseCursors, this.position, new Rectangle?(this.sourceRect), Color.White, 0.0f, Vector2.Zero, 4f, SpriteEffects.None, 0.9f);
    if (Game1.dayOfMonth != this._lastDayOfMonth)
    {
      this._lastDayOfMonth = Game1.dayOfMonth;
      this._lastDayOfMonthString = Game1.shortDayDisplayNameFromDayOfSeason(this._lastDayOfMonth);
    }
    this._dateText.Clear();
    switch (LocalizedContentManager.CurrentLanguageCode)
    {
      case LocalizedContentManager.LanguageCode.ja:
        this._dateText.AppendEx(Game1.dayOfMonth);
        this._dateText.Append("日 (");
        this._dateText.Append(this._lastDayOfMonthString);
        this._dateText.Append(")");
        break;
      case LocalizedContentManager.LanguageCode.zh:
        this._dateText.AppendEx(Game1.dayOfMonth);
        this._dateText.Append("日 ");
        this._dateText.Append(this._lastDayOfMonthString);
        break;
      case LocalizedContentManager.LanguageCode.mod:
        this._dateText.Append(LocalizedContentManager.CurrentModLanguage.ClockDateFormat.Replace("[DAY_OF_WEEK]", this._lastDayOfMonthString).Replace("[DAY_OF_MONTH]", Game1.dayOfMonth.ToString()));
        break;
      default:
        this._dateText.Append(this._lastDayOfMonthString);
        this._dateText.Append(". ");
        this._dateText.AppendEx(Game1.dayOfMonth);
        break;
    }
    Vector2 vector2_1 = font.MeasureString(this._dateText);
    Vector2 vector2_2 = new Vector2((float) ((double) this.sourceRect.X * (9.0 / 16.0) - (double) vector2_1.X / 2.0), (float) ((double) this.sourceRect.Y * (LocalizedContentManager.CurrentLanguageLatin ? 0.10000000149011612 : 0.10000000149011612) - (double) vector2_1.Y / 2.0));
    Utility.drawTextWithShadow(b, this._dateText, font, this.position + vector2_2, Game1.textColor);
    b.Draw(Game1.mouseCursors, this.position + new Vector2(212f, 68f), new Rectangle?(new Rectangle(406, 441 + Game1.seasonIndex * 8, 12, 8)), Color.White, 0.0f, Vector2.Zero, 4f, SpriteEffects.None, 0.9f);
    if (Game1.weatherIcon == 999)
      b.Draw(Game1.mouseCursors_1_6, this.position + new Vector2(116f, 68f), new Rectangle?(new Rectangle(243, 293, 12, 8)), Color.White, 0.0f, Vector2.Zero, 4f, SpriteEffects.None, 0.9f);
    else
      b.Draw(Game1.mouseCursors, this.position + new Vector2(116f, 68f), new Rectangle?(new Rectangle(317 + 12 * Game1.weatherIcon, 421, 12, 8)), Color.White, 0.0f, Vector2.Zero, 4f, SpriteEffects.None, 0.9f);
    this._padZeros.Clear();
    if (Game1.timeOfDay % 100 == 0)
      this._padZeros.Append("0");
    this._hours.Clear();
    switch (LocalizedContentManager.CurrentLanguageCode)
    {
      case LocalizedContentManager.LanguageCode.ru:
      case LocalizedContentManager.LanguageCode.zh:
      case LocalizedContentManager.LanguageCode.pt:
      case LocalizedContentManager.LanguageCode.es:
      case LocalizedContentManager.LanguageCode.de:
      case LocalizedContentManager.LanguageCode.th:
      case LocalizedContentManager.LanguageCode.fr:
      case LocalizedContentManager.LanguageCode.tr:
      case LocalizedContentManager.LanguageCode.hu:
        this._temp.Clear();
        this._temp.AppendEx(Game1.timeOfDay / 100 % 24);
        if (Game1.timeOfDay / 100 % 24 <= 9)
          this._hours.Append("0");
        this._hours.AppendEx(this._temp);
        break;
      default:
        if (Game1.timeOfDay / 100 % 12 == 0)
        {
          if (LocalizedContentManager.CurrentLanguageCode == LocalizedContentManager.LanguageCode.ja)
          {
            this._hours.Append("0");
            break;
          }
          this._hours.Append("12");
          break;
        }
        this._hours.AppendEx(Game1.timeOfDay / 100 % 12);
        break;
    }
    this._timeText.Clear();
    this._timeText.AppendEx(this._hours);
    this._timeText.Append(":");
    this._timeText.AppendEx(Game1.timeOfDay % 100);
    this._timeText.AppendEx(this._padZeros);
    switch (LocalizedContentManager.CurrentLanguageCode)
    {
      case LocalizedContentManager.LanguageCode.en:
      case LocalizedContentManager.LanguageCode.it:
        this._timeText.Append(" ");
        if (Game1.timeOfDay < 1200 || Game1.timeOfDay >= 2400)
        {
          this._timeText.Append(this._amString);
          break;
        }
        this._timeText.Append(this._pmString);
        break;
      case LocalizedContentManager.LanguageCode.ja:
        this._temp.Clear();
        this._temp.AppendEx(this._timeText);
        this._timeText.Clear();
        if (Game1.timeOfDay < 1200 || Game1.timeOfDay >= 2400)
        {
          this._timeText.Append(this._amString);
          this._timeText.Append(" ");
          this._timeText.AppendEx(this._temp);
          break;
        }
        this._timeText.Append(this._pmString);
        this._timeText.Append(" ");
        this._timeText.AppendEx(this._temp);
        break;
      case LocalizedContentManager.LanguageCode.ko:
        if (Game1.timeOfDay < 1200 || Game1.timeOfDay >= 2400)
        {
          this._timeText.Append(this._amString);
          break;
        }
        this._timeText.Append(this._pmString);
        break;
      case LocalizedContentManager.LanguageCode.mod:
        this._timeText.Clear();
        this._timeText.Append(LocalizedContentManager.FormatTimeString(Game1.timeOfDay, LocalizedContentManager.CurrentModLanguage.ClockTimeFormat));
        break;
    }
    Vector2 vector2_3 = font.MeasureString(this._timeText);
    Vector2 vector2_4 = new Vector2((float) ((double) this.sourceRect.X * 0.550000011920929 - (double) vector2_3.X / 2.0 + (this.timeShakeTimer > 0 ? (double) Game1.random.Next(-2, 3) : 0.0)), (float) ((double) this.sourceRect.Y * (LocalizedContentManager.CurrentLanguageLatin ? 0.31000000238418579 : 0.31000000238418579) - (double) vector2_3.Y / 2.0 + (this.timeShakeTimer > 0 ? (double) Game1.random.Next(-2, 3) : 0.0)));
    int num1;
    if (!Game1.shouldTimePass() && !Game1.fadeToBlack)
    {
      timeSpan = Game1.currentGameTime.TotalGameTime;
      num1 = timeSpan.TotalMilliseconds % 2000.0 > 1000.0 ? 1 : 0;
    }
    else
      num1 = 1;
    bool flag = num1 != 0;
    Utility.drawTextWithShadow(b, this._timeText, font, this.position + vector2_4, Game1.timeOfDay >= 2400 ? Color.Red : Game1.textColor * (flag ? 1f : 0.5f));
    int num2 = (int) ((double) (Game1.timeOfDay - Game1.timeOfDay % 100) + (double) (Game1.timeOfDay % 100 / 10) * 16.659999847412109);
    if (Game1.player.hasVisibleQuests)
    {
      this.questButton.draw(b);
      if (this.questPulseTimer > 0)
      {
        float num3 = (float) (1.0 / ((double) Math.Max(300f, (float) Math.Abs(this.questPulseTimer % 1000 - 500)) / 500.0));
        b.Draw(Game1.mouseCursors, new Vector2((float) (this.questButton.bounds.X + 24), (float) (this.questButton.bounds.Y + 32 /*0x20*/)) + ((double) num3 > 1.0 ? new Vector2((float) Game1.random.Next(-1, 2), (float) Game1.random.Next(-1, 2)) : Vector2.Zero), new Rectangle?(new Rectangle(395, 497, 3, 8)), Color.White, 0.0f, new Vector2(2f, 4f), 4f * num3, SpriteEffects.None, 0.99f);
      }
      if (this.questPingTimer > 0)
        b.Draw(Game1.mouseCursors, new Vector2((float) (Game1.dayTimeMoneyBox.questButton.bounds.Left - 16 /*0x10*/), (float) (Game1.dayTimeMoneyBox.questButton.bounds.Bottom + 8)), new Rectangle?(new Rectangle(128 /*0x80*/ + (this.questPingTimer / 200 % 2 == 0 ? 0 : 16 /*0x10*/), 208 /*0xD0*/, 16 /*0x10*/, 16 /*0x10*/)), Color.White, 0.0f, Vector2.Zero, 4f, SpriteEffects.None, 0.9f);
    }
    if (Game1.options.zoomButtons)
    {
      this.zoomInButton.draw(b, Color.White * ((double) Game1.options.desiredBaseZoomLevel >= 2.0 ? 0.5f : 1f), 1f);
      this.zoomOutButton.draw(b, Color.White * ((double) Game1.options.desiredBaseZoomLevel <= 0.75 ? 0.5f : 1f), 1f);
    }
    this.drawMoneyBox(b);
    if (this._hoverText.Length > 0 && this.isWithinBounds(Game1.getOldMouseX(), Game1.getOldMouseY()))
      IClickableMenu.drawHoverText(b, this._hoverText, Game1.dialogueFont);
    b.Draw(Game1.mouseCursors, this.position + new Vector2(88f, 88f), new Rectangle?(new Rectangle(324, 477, 7, 19)), Color.White, (float) (Math.PI + Math.Min(Math.PI, ((double) num2 + (double) Game1.gameTimeInterval / (double) Game1.realMilliSecondsPerGameTenMinutes * 16.600000381469727 - 600.0) / 2000.0 * Math.PI)), new Vector2(3f, 17f), 4f, SpriteEffects.None, 0.9f);
    if (this.questNotificationTimer > 0)
    {
      Vector2 position = this.position + new Vector2(27f, 76f) * 4f;
      b.Draw(Game1.mouseCursors_1_6, position, new Rectangle?(new Rectangle(257, 228, 39, 18)), Color.White, 0.0f, Vector2.Zero, 4f, SpriteEffects.None, 0.9f);
      b.Draw(this.questPingTexture, position + new Vector2(1f, 1f) * 4f, new Rectangle?(this.questPingSourceRect), Color.White, 0.0f, Vector2.Zero, 4f, SpriteEffects.None, 0.91f);
      if (this.questPingString != null)
        Utility.drawTextWithShadow(b, this.questPingString, Game1.smallFont, position + new Vector2(27f, 9.5f) * 4f - Game1.smallFont.MeasureString(this.questPingString) * 0.5f, Game1.textColor);
      else
        b.Draw(Game1.mouseCursors_1_6, position + new Vector2(22f, 5f) * 4f, new Rectangle?(new Rectangle(297, 229, 9, 8)), Color.White, 0.0f, Vector2.Zero, 4f, SpriteEffects.None, 0.91f);
    }
    if (this.goldCoinTimer <= 0)
      return;
    SpriteText.drawSmallTextBubble(b, this.goldCoinString, this.position + new Vector2(5f, 73f) * 4f, layerDepth: 0.99f, drawPointerOnTop: true);
  }

  private void updatePosition()
  {
    this.position = new Vector2((float) (Game1.uiViewport.Width - 300), 8f);
    if (Game1.isOutdoorMapSmallerThanViewport())
      this.position = new Vector2(Math.Min(this.position.X, (float) (-Game1.uiViewport.X + Game1.currentLocation.map.Layers[0].LayerWidth * 64 /*0x40*/ - 300)), 8f);
    Utility.makeSafe(ref this.position, 300, 284);
    this.xPositionOnScreen = (int) this.position.X;
    this.yPositionOnScreen = (int) this.position.Y;
    this.questButton.bounds = new Rectangle(this.xPositionOnScreen + 212, this.yPositionOnScreen + 240 /*0xF0*/, 44, 46);
    this.zoomOutButton.bounds = new Rectangle(this.xPositionOnScreen + 92, this.yPositionOnScreen + 244, 28, 32 /*0x20*/);
    this.zoomInButton.bounds = new Rectangle(this.xPositionOnScreen + 124, this.yPositionOnScreen + 244, 28, 32 /*0x20*/);
  }
}
