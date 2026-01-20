// Decompiled with JetBrains decompiler
// Type: StardewValley.Menus.Billboard
// Assembly: Stardew Valley, Version=1.6.15.24356, Culture=neutral, PublicKeyToken=null
// MVID: AA1F513A-94F0-4EF6-A35F-2D5B4A3721BF
// Assembly location: D:\SteamLibrary\steamapps\common\Stardew Valley\Stardew Valley.dll
// XML documentation location: D:\SteamLibrary\steamapps\common\Stardew Valley\Stardew Valley.xml

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using StardewValley.BellsAndWhistles;
using StardewValley.GameData;
using StardewValley.GameData.Characters;
using StardewValley.Locations;
using StardewValley.Network;
using StardewValley.TokenizableStrings;
using System;
using System.Collections.Generic;
using System.Linq;

#nullable disable
namespace StardewValley.Menus;

public class Billboard : IClickableMenu
{
  private Texture2D billboardTexture;
  public const int basewidth = 338;
  public const int baseWidth_calendar = 301;
  public const int baseheight = 198;
  private bool dailyQuestBoard;
  public ClickableComponent acceptQuestButton;
  public List<ClickableTextureComponent> calendarDays;
  private string hoverText = "";
  private List<int> booksellerdays;
  /// <summary>The events to show on the calendar for each day.</summary>
  /// <remarks>This only has entries for days that have events.</remarks>
  public readonly Dictionary<int, Billboard.BillboardDay> calendarDayData = new Dictionary<int, Billboard.BillboardDay>();

  public Billboard(bool dailyQuest = false)
    : base(0, 0, 0, 0, true)
  {
    if (!Game1.player.hasOrWillReceiveMail("checkedBulletinOnce"))
    {
      Game1.player.mailReceived.Add("checkedBulletinOnce");
      Game1.RequireLocation<Town>("Town").checkedBoard();
    }
    this.dailyQuestBoard = dailyQuest;
    this.billboardTexture = Game1.temporaryContent.Load<Texture2D>("LooseSprites\\Billboard");
    this.width = (dailyQuest ? 338 : 301) * 4;
    this.height = 792;
    Vector2 centeringOnScreen = Utility.getTopLeftPositionForCenteringOnScreen(this.width, this.height);
    this.xPositionOnScreen = (int) centeringOnScreen.X;
    this.yPositionOnScreen = (int) centeringOnScreen.Y;
    if (!dailyQuest)
      this.booksellerdays = Utility.getDaysOfBooksellerThisSeason();
    this.acceptQuestButton = new ClickableComponent(new Rectangle(this.xPositionOnScreen + this.width / 2 - 128 /*0x80*/, this.yPositionOnScreen + this.height - 128 /*0x80*/, (int) Game1.dialogueFont.MeasureString(Game1.content.LoadString("Strings\\UI:AcceptQuest")).X + 24, (int) Game1.dialogueFont.MeasureString(Game1.content.LoadString("Strings\\UI:AcceptQuest")).Y + 24), "")
    {
      myID = 0
    };
    this.UpdateDailyQuestButton();
    this.upperRightCloseButton = new ClickableTextureComponent(new Rectangle(this.xPositionOnScreen + this.width - 20, this.yPositionOnScreen, 48 /*0x30*/, 48 /*0x30*/), Game1.mouseCursors, new Rectangle(337, 494, 12, 12), 4f);
    Game1.playSound("bigSelect");
    if (!dailyQuest)
    {
      this.calendarDays = new List<ClickableTextureComponent>();
      Dictionary<int, List<NPC>> birthdays = this.GetBirthdays();
      for (int index = 1; index <= 28; ++index)
      {
        List<Billboard.BillboardEvent> eventsForDay = this.GetEventsForDay(index, birthdays);
        if (eventsForDay.Count > 0)
          this.calendarDayData[index] = new Billboard.BillboardDay(eventsForDay.ToArray());
        int num = index - 1;
        List<ClickableTextureComponent> calendarDays = this.calendarDays;
        ClickableTextureComponent textureComponent = new ClickableTextureComponent(index.ToString(), new Rectangle(this.xPositionOnScreen + 152 + num % 7 * 32 /*0x20*/ * 4, this.yPositionOnScreen + 200 + num / 7 * 32 /*0x20*/ * 4, 124, 124), string.Empty, string.Empty, (Texture2D) null, Rectangle.Empty, 1f);
        textureComponent.myID = index;
        textureComponent.rightNeighborID = index % 7 != 0 ? index + 1 : -1;
        textureComponent.leftNeighborID = index % 7 != 1 ? index - 1 : -1;
        textureComponent.downNeighborID = index + 7;
        textureComponent.upNeighborID = index > 7 ? index - 7 : -1;
        calendarDays.Add(textureComponent);
      }
    }
    if (!Game1.options.SnappyMenus)
      return;
    this.populateClickableComponentList();
    this.snapToDefaultClickableComponent();
  }

  /// <summary>Get all NPC birthdays that should be shown on the calendar this month, indexed by day.</summary>
  public virtual Dictionary<int, List<NPC>> GetBirthdays()
  {
    HashSet<string> addedBirthdays = new HashSet<string>();
    Dictionary<int, List<NPC>> birthdays = new Dictionary<int, List<NPC>>();
    Utility.ForEachVillager((Func<NPC, bool>) (npc =>
    {
      if (npc.Birthday_Season != Game1.currentSeason)
        return true;
      CalendarBehavior? calendar = npc.GetData()?.Calendar;
      if (calendar.GetValueOrDefault() == CalendarBehavior.HiddenAlways || calendar.GetValueOrDefault() == CalendarBehavior.HiddenUntilMet && !Game1.player.friendshipData.ContainsKey(npc.Name) || addedBirthdays.Contains(npc.Name))
        return true;
      List<NPC> npcList;
      if (!birthdays.TryGetValue(npc.Birthday_Day, out npcList))
        birthdays[npc.Birthday_Day] = npcList = new List<NPC>();
      npcList.Add(npc);
      addedBirthdays.Add(npc.Name);
      return true;
    }));
    return birthdays;
  }

  /// <summary>Get the events to show on a given calendar day.</summary>
  /// <param name="day">The day of month.</param>
  /// <param name="birthdays">A cached lookup of birthdays by day.</param>
  public virtual List<Billboard.BillboardEvent> GetEventsForDay(
    int day,
    Dictionary<int, List<NPC>> birthdays)
  {
    List<Billboard.BillboardEvent> eventsForDay = new List<Billboard.BillboardEvent>();
    if (Utility.isFestivalDay(day, Game1.season))
    {
      string str = Game1.currentSeason + day.ToString();
      string displayName = Game1.temporaryContent.Load<Dictionary<string, string>>("Data\\Festivals\\" + str)["name"];
      eventsForDay.Add(new Billboard.BillboardEvent(Billboard.BillboardEventType.Festival, new string[1]
      {
        str
      }, displayName));
    }
    string id;
    PassiveFestivalData data;
    if (Utility.TryGetPassiveFestivalDataForDay(day, Game1.season, (string) null, out id, out data, true))
    {
      bool? showOnCalendar = data?.ShowOnCalendar;
      if (showOnCalendar.HasValue && showOnCalendar.GetValueOrDefault())
      {
        string text = TokenParser.ParseText(data.DisplayName);
        if (!GameStateQuery.CheckConditions(data.Condition))
          eventsForDay.Add(new Billboard.BillboardEvent(Billboard.BillboardEventType.PassiveFestival, new string[1]
          {
            id
          }, "???")
          {
            locked = true
          });
        else
          eventsForDay.Add(new Billboard.BillboardEvent(Billboard.BillboardEventType.PassiveFestival, new string[1]
          {
            id
          }, text));
      }
    }
    if (Game1.IsSummer && (day == 20 || day == 21))
    {
      string displayName = Game1.content.LoadString("Strings\\1_6_Strings:TroutDerby");
      eventsForDay.Add(new Billboard.BillboardEvent(Billboard.BillboardEventType.FishingDerby, LegacyShims.EmptyArray<string>(), displayName));
    }
    else if (Game1.IsWinter && (day == 12 || day == 13))
    {
      string displayName = Game1.content.LoadString("Strings\\1_6_Strings:SquidFest");
      eventsForDay.Add(new Billboard.BillboardEvent(Billboard.BillboardEventType.FishingDerby, LegacyShims.EmptyArray<string>(), displayName));
    }
    if (this.booksellerdays.Contains(day))
    {
      string displayName = Game1.content.LoadString("Strings\\1_6_Strings:Bookseller");
      eventsForDay.Add(new Billboard.BillboardEvent(Billboard.BillboardEventType.Bookseller, LegacyShims.EmptyArray<string>(), displayName));
    }
    List<NPC> npcList;
    if (birthdays.TryGetValue(day, out npcList))
    {
      foreach (NPC npc in npcList)
      {
        char ch = npc.displayName.Last<char>();
        string displayName = ch == 's' || LocalizedContentManager.CurrentLanguageCode == LocalizedContentManager.LanguageCode.de && (ch == 'x' || ch == 'ß' || ch == 'z') ? Game1.content.LoadString("Strings\\UI:Billboard_SBirthday", (object) npc.displayName) : Game1.content.LoadString("Strings\\UI:Billboard_Birthday", (object) npc.displayName);
        Texture2D texture;
        try
        {
          texture = Game1.content.Load<Texture2D>("Characters\\" + npc.getTextureName());
        }
        catch
        {
          texture = npc.Sprite.Texture;
        }
        eventsForDay.Add(new Billboard.BillboardEvent(Billboard.BillboardEventType.Birthday, new string[1]
        {
          npc.Name
        }, displayName, texture, npc.getMugShotSourceRect()));
      }
    }
    HashSet<Farmer> farmerSet = new HashSet<Farmer>();
    FarmerCollection onlineFarmers = Game1.getOnlineFarmers();
    foreach (Farmer farmer in onlineFarmers)
    {
      if (!farmerSet.Contains(farmer) && farmer.isEngaged() && !farmer.hasCurrentOrPendingRoommate())
      {
        string sub2 = (string) null;
        WorldDate worldDate = (WorldDate) null;
        NPC characterFromName = Game1.getCharacterFromName(farmer.spouse);
        if (characterFromName != null)
        {
          worldDate = farmer.friendshipData[farmer.spouse].WeddingDate;
          sub2 = characterFromName.displayName;
        }
        else
        {
          long? spouse = farmer.team.GetSpouse(farmer.UniqueMultiplayerID);
          if (spouse.HasValue)
          {
            Farmer player = Game1.GetPlayer(spouse.Value);
            if (player != null && onlineFarmers.Contains(player))
            {
              worldDate = farmer.team.GetFriendship(farmer.UniqueMultiplayerID, spouse.Value).WeddingDate;
              farmerSet.Add(player);
              sub2 = player.Name;
            }
          }
        }
        if (!(worldDate == (WorldDate) null))
        {
          if (worldDate.TotalDays < Game1.Date.TotalDays)
          {
            worldDate = new WorldDate(Game1.Date);
            ++worldDate.TotalDays;
          }
          int? totalDays1 = worldDate?.TotalDays;
          int totalDays2 = Game1.Date.TotalDays;
          if (totalDays1.GetValueOrDefault() >= totalDays2 & totalDays1.HasValue && Game1.season == worldDate.Season && day == worldDate.DayOfMonth)
          {
            eventsForDay.Add(new Billboard.BillboardEvent(Billboard.BillboardEventType.Wedding, new string[2]
            {
              farmer.Name,
              sub2
            }, Game1.content.LoadString("Strings\\UI:Calendar_Wedding", (object) farmer.Name, (object) sub2)));
            farmerSet.Add(farmer);
          }
        }
      }
    }
    return eventsForDay;
  }

  public override void snapToDefaultClickableComponent()
  {
    this.currentlySnappedComponent = this.getComponentWithID(!this.dailyQuestBoard ? 1 : 0);
    this.snapCursorToCurrentSnappedComponent();
  }

  /// <inheritdoc />
  public override void gameWindowSizeChanged(Rectangle oldBounds, Rectangle newBounds)
  {
    base.gameWindowSizeChanged(oldBounds, newBounds);
    Game1.activeClickableMenu = (IClickableMenu) new Billboard(this.dailyQuestBoard);
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
    if (!this.acceptQuestButton.visible || !this.acceptQuestButton.containsPoint(x, y))
      return;
    Game1.playSound("newArtifact");
    Game1.questOfTheDay.dailyQuest.Value = true;
    Game1.questOfTheDay.dayQuestAccepted.Value = Game1.Date.TotalDays;
    Game1.questOfTheDay.accepted.Value = true;
    Game1.questOfTheDay.canBeCancelled.Value = true;
    Game1.questOfTheDay.daysLeft.Value = 2;
    Game1.player.questLog.Add(Game1.questOfTheDay);
    Game1.player.acceptedDailyQuest.Set(true);
    this.UpdateDailyQuestButton();
  }

  /// <inheritdoc />
  public override void performHoverAction(int x, int y)
  {
    base.performHoverAction(x, y);
    this.hoverText = "";
    if (this.dailyQuestBoard && Game1.questOfTheDay != null && !Game1.questOfTheDay.accepted.Value)
    {
      float scale = this.acceptQuestButton.scale;
      this.acceptQuestButton.scale = this.acceptQuestButton.bounds.Contains(x, y) ? 1.5f : 1f;
      if ((double) this.acceptQuestButton.scale > (double) scale)
        Game1.playSound("Cowboy_gunshot");
    }
    if (this.calendarDays == null)
      return;
    foreach (ClickableTextureComponent calendarDay in this.calendarDays)
    {
      if (calendarDay.bounds.Contains(x, y))
      {
        Billboard.BillboardDay billboardDay;
        this.hoverText = this.calendarDayData.TryGetValue(calendarDay.myID, out billboardDay) ? billboardDay.HoverText : string.Empty;
        break;
      }
    }
  }

  /// <inheritdoc />
  public override void draw(SpriteBatch b)
  {
    bool flag1 = false;
    if (!Game1.options.showClearBackgrounds)
      b.Draw(Game1.fadeToBlackRect, Game1.graphics.GraphicsDevice.Viewport.Bounds, Color.Black * 0.75f);
    b.Draw(this.billboardTexture, new Vector2((float) this.xPositionOnScreen, (float) this.yPositionOnScreen), new Rectangle?(this.dailyQuestBoard ? new Rectangle(0, 0, 338, 198) : new Rectangle(0, 198, 301, 198)), Color.White, 0.0f, Vector2.Zero, 4f, SpriteEffects.None, 1f);
    if (!this.dailyQuestBoard)
    {
      b.DrawString(Game1.dialogueFont, Utility.getSeasonNameFromNumber(Game1.seasonIndex), new Vector2((float) (this.xPositionOnScreen + 160 /*0xA0*/), (float) (this.yPositionOnScreen + 80 /*0x50*/)), Game1.textColor);
      b.DrawString(Game1.dialogueFont, Game1.content.LoadString("Strings\\UI:Billboard_Year", (object) Game1.year), new Vector2((float) (this.xPositionOnScreen + 448), (float) (this.yPositionOnScreen + 80 /*0x50*/)), Game1.textColor);
      for (int index = 0; index < this.calendarDays.Count; ++index)
      {
        ClickableTextureComponent calendarDay = this.calendarDays[index];
        Billboard.BillboardDay billboardDay;
        if (this.calendarDayData.TryGetValue(calendarDay.myID, out billboardDay))
        {
          if (billboardDay.Texture != null)
            b.Draw(billboardDay.Texture, new Vector2((float) (calendarDay.bounds.X + 48 /*0x30*/), (float) (calendarDay.bounds.Y + 28)), new Rectangle?(billboardDay.TextureSourceRect), Color.White, 0.0f, Vector2.Zero, 4f, SpriteEffects.None, 1f);
          if (billboardDay.Type.HasFlag((Enum) Billboard.BillboardEventType.PassiveFestival))
            Utility.drawWithShadow(b, Game1.mouseCursors, new Vector2((float) (calendarDay.bounds.X + 12), (float) (calendarDay.bounds.Y + 60) - Game1.dialogueButtonScale / 2f), new Rectangle(346, 392, 8, 8), billboardDay.GetEventOfType(Billboard.BillboardEventType.PassiveFestival).locked ? Color.Black * 0.3f : Color.White, 0.0f, Vector2.Zero, 4f, layerDepth: 1f);
          if (billboardDay.Type.HasFlag((Enum) Billboard.BillboardEventType.Festival))
            Utility.drawWithShadow(b, this.billboardTexture, new Vector2((float) (calendarDay.bounds.X + 40), (float) (calendarDay.bounds.Y + 56) - Game1.dialogueButtonScale / 2f), new Rectangle(1 + (int) (Game1.currentGameTime.TotalGameTime.TotalMilliseconds % 600.0 / 100.0) * 14, 398, 14, 12), Color.White, 0.0f, Vector2.Zero, 4f, layerDepth: 1f);
          if (billboardDay.Type.HasFlag((Enum) Billboard.BillboardEventType.FishingDerby))
            Utility.drawWithShadow(b, Game1.mouseCursors_1_6, new Vector2((float) (this.calendarDays[index].bounds.X + 8), (float) (this.calendarDays[index].bounds.Y + 60) - Game1.dialogueButtonScale / 2f), new Rectangle(103, 2, 10, 11), Color.White, 0.0f, Vector2.Zero, 4f, layerDepth: 1f);
          if (billboardDay.Type.HasFlag((Enum) Billboard.BillboardEventType.Wedding))
            b.Draw(Game1.mouseCursors2, new Vector2((float) (calendarDay.bounds.Right - 56), (float) (calendarDay.bounds.Top - 12)), new Rectangle?(new Rectangle(112 /*0x70*/, 32 /*0x20*/, 16 /*0x10*/, 14)), Color.White, 0.0f, Vector2.Zero, 4f, SpriteEffects.None, 1f);
          if (billboardDay.Type.HasFlag((Enum) Billboard.BillboardEventType.Bookseller))
            b.Draw(Game1.mouseCursors_1_6, new Vector2((float) (calendarDay.bounds.Right - 72) - (float) (2.0 * Math.Sin((Game1.currentGameTime.TotalGameTime.TotalSeconds + (double) index * 0.3) * 3.0)), (float) (calendarDay.bounds.Top + 52) - (float) (2.0 * Math.Cos((Game1.currentGameTime.TotalGameTime.TotalSeconds + (double) index * 0.3) * 2.0))), new Rectangle?(new Rectangle(71, 63 /*0x3F*/, 8, 15)), Color.White, 0.0f, Vector2.Zero, 4f, SpriteEffects.None, 1f);
        }
        if (Game1.dayOfMonth > index + 1)
          b.Draw(Game1.staminaRect, calendarDay.bounds, Color.Gray * 0.25f);
        else if (Game1.dayOfMonth == index + 1)
        {
          int num = (int) (4.0 * (double) Game1.dialogueButtonScale / 8.0);
          IClickableMenu.drawTextureBox(b, Game1.mouseCursors, new Rectangle(379, 357, 3, 3), calendarDay.bounds.X - num, calendarDay.bounds.Y - num, calendarDay.bounds.Width + num * 2, calendarDay.bounds.Height + num * 2, Color.Blue, 4f, false);
        }
      }
    }
    else
    {
      if (Game1.options.SnappyMenus)
        flag1 = true;
      if (string.IsNullOrEmpty(Game1.questOfTheDay?.currentObjective))
      {
        b.DrawString(Game1.dialogueFont, Game1.content.LoadString("Strings\\UI:Billboard_NothingPosted"), new Vector2((float) (this.xPositionOnScreen + 384), (float) (this.yPositionOnScreen + 320)), Game1.textColor);
      }
      else
      {
        SpriteFont spriteFont = LocalizedContentManager.CurrentLanguageCode == LocalizedContentManager.LanguageCode.ko ? Game1.smallFont : Game1.dialogueFont;
        string text = Game1.parseText(Game1.questOfTheDay.questDescription, spriteFont, 640);
        Utility.drawTextWithShadow(b, text, spriteFont, new Vector2((float) (this.xPositionOnScreen + 320 + 32 /*0x20*/), (float) (this.yPositionOnScreen + 256 /*0x0100*/)), Game1.textColor, shadowIntensity: 0.5f);
        if (this.acceptQuestButton.visible)
        {
          flag1 = false;
          IClickableMenu.drawTextureBox(b, Game1.mouseCursors, new Rectangle(403, 373, 9, 9), this.acceptQuestButton.bounds.X, this.acceptQuestButton.bounds.Y, this.acceptQuestButton.bounds.Width, this.acceptQuestButton.bounds.Height, (double) this.acceptQuestButton.scale > 1.0 ? Color.LightPink : Color.White, 4f * this.acceptQuestButton.scale);
          Utility.drawTextWithShadow(b, Game1.content.LoadString("Strings\\UI:AcceptQuest"), Game1.dialogueFont, new Vector2((float) (this.acceptQuestButton.bounds.X + 12), (float) (this.acceptQuestButton.bounds.Y + (LocalizedContentManager.CurrentLanguageLatin ? 16 /*0x10*/ : 12))), Game1.textColor);
        }
        if (Game1.stats.Get("BillboardQuestsDone") % 3U == 2U && (this.acceptQuestButton.visible || !Game1.questOfTheDay.completed.Value))
        {
          Utility.drawWithShadow(b, Game1.content.Load<Texture2D>("TileSheets\\Objects_2"), this.Position + new Vector2(215f, 144f) * 4f, new Rectangle(80 /*0x50*/, 128 /*0x80*/, 16 /*0x10*/, 16 /*0x10*/), Color.White, 0.0f, Vector2.Zero, 4f);
          SpriteText.drawString(b, "x1", (int) this.Position.X + 936, (int) this.Position.Y + 596);
        }
      }
      bool flag2 = Game1.stats.Get("BillboardQuestsDone") % 3U == 0U && Game1.questOfTheDay != null && Game1.questOfTheDay.completed.Value;
      for (int index = 0; (long) index < (flag2 ? 3L : (long) (Game1.stats.Get("BillboardQuestsDone") % 3U)); ++index)
        b.Draw(this.billboardTexture, this.Position + new Vector2((float) (18 + 12 * index), 36f) * 4f, new Rectangle?(new Rectangle(140, 397, 10, 11)), Color.White, 0.0f, Vector2.Zero, 4f, SpriteEffects.None, 0.6f);
      if (Game1.player.hasCompletedCommunityCenter())
        b.Draw(this.billboardTexture, this.Position + new Vector2(290f, 59f) * 4f, new Rectangle?(new Rectangle(0, 427, 39, 54)), Color.White, 0.0f, Vector2.Zero, 4f, SpriteEffects.None, 0.6f);
    }
    base.draw(b);
    if (flag1)
      return;
    Game1.mouseCursorTransparency = 1f;
    this.drawMouse(b);
    if (this.hoverText.Length <= 0)
      return;
    IClickableMenu.drawHoverText(b, this.hoverText, Game1.dialogueFont);
  }

  public void UpdateDailyQuestButton()
  {
    if (this.acceptQuestButton == null)
      return;
    if (!this.dailyQuestBoard)
      this.acceptQuestButton.visible = false;
    else
      this.acceptQuestButton.visible = Game1.CanAcceptDailyQuest();
  }

  /// <summary>An event type that can be shown in the calendar.</summary>
  [Flags]
  public enum BillboardEventType
  {
    /// <summary>No event.</summary>
    None = 0,
    /// <summary>An NPC's birthday.</summary>
    Birthday = 1,
    /// <summary>A non-passive festival.</summary>
    Festival = 2,
    /// <summary>A fishing derby like Trophy Derby or Squidfest.</summary>
    FishingDerby = 4,
    /// <summary>A passive festival.</summary>
    PassiveFestival = 8,
    /// <summary>A wedding between a player and a player/NPC.</summary>
    Wedding = 16, // 0x00000010
    /// <summary>A day that Marcello's Books will be in town</summary>
    Bookseller = 32, // 0x00000020
  }

  /// <summary>The cached data for a calendar day.</summary>
  public class BillboardDay
  {
    /// <summary>The event types on this day.</summary>
    public Billboard.BillboardEventType Type { get; }

    /// <summary>The events on this day.</summary>
    public Billboard.BillboardEvent[] Events { get; }

    /// <summary>The combined hover text for the events on this day.</summary>
    public string HoverText { get; }

    /// <summary>The texture to show for the calendar slot, if any.</summary>
    public Texture2D Texture { get; }

    /// <summary>The pixel area to draw within the <see cref="P:StardewValley.Menus.Billboard.BillboardDay.Texture" />, if applicable.</summary>
    public Rectangle TextureSourceRect { get; }

    /// <summary>Construct an instance.</summary>
    /// <param name="events">The events on this day.</param>
    public BillboardDay(Billboard.BillboardEvent[] events)
    {
      this.Events = events;
      this.HoverText = string.Empty;
      foreach (Billboard.BillboardEvent billboardEvent in events)
      {
        this.Type = this.Type | billboardEvent.Type;
        if (this.Texture == null && billboardEvent.Texture != null)
        {
          this.Texture = billboardEvent.Texture;
          this.TextureSourceRect = billboardEvent.TextureSourceRect;
        }
        this.HoverText = this.HoverText + billboardEvent.DisplayName + Environment.NewLine;
      }
      this.HoverText = this.HoverText.Trim();
    }

    public Billboard.BillboardEvent GetEventOfType(Billboard.BillboardEventType type)
    {
      foreach (Billboard.BillboardEvent eventOfType in this.Events)
      {
        if (eventOfType.Type == type)
          return eventOfType;
      }
      return (Billboard.BillboardEvent) null;
    }
  }

  /// <summary>An event shown on the calendar.</summary>
  public class BillboardEvent
  {
    /// <summary>If this event is currently unavailable. (e.g. Desert festival before desert is open)</summary>
    public bool locked;

    /// <summary>The event type.</summary>
    public Billboard.BillboardEventType Type { get; }

    /// <summary>The values related to the event (like the names of the players or NPCs getting married).</summary>
    public string[] Arguments { get; }

    /// <summary>The name to show on the calendar.</summary>
    public string DisplayName { get; }

    /// <summary>The texture to show for the calendar slot, if any.</summary>
    public Texture2D Texture { get; }

    /// <summary>The pixel area to draw within the <see cref="P:StardewValley.Menus.Billboard.BillboardEvent.Texture" />, if applicable.</summary>
    public Rectangle TextureSourceRect { get; }

    /// <summary>Construct an instance.</summary>
    /// <param name="type">The event type.</param>
    /// <param name="arguments">The values related to the event (like the names of the players or NPCs getting married).</param>
    /// <param name="displayName">The name to show on the calendar.</param>
    /// <param name="texture">The texture to show for the calendar slot, if any.</param>
    /// <param name="sourceRect">The pixel area to draw within the <paramref name="texture" />, if applicable.</param>
    public BillboardEvent(
      Billboard.BillboardEventType type,
      string[] arguments,
      string displayName,
      Texture2D texture = null,
      Rectangle sourceRect = default (Rectangle))
    {
      this.Type = type;
      this.Arguments = arguments;
      this.DisplayName = displayName;
      this.Texture = texture;
      this.TextureSourceRect = sourceRect;
    }
  }
}
