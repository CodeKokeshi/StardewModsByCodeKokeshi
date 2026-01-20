// Decompiled with JetBrains decompiler
// Type: StardewValley.Menus.SkillsPage
// Assembly: Stardew Valley, Version=1.6.15.24356, Culture=neutral, PublicKeyToken=null
// MVID: AA1F513A-94F0-4EF6-A35F-2D5B4A3721BF
// Assembly location: D:\SteamLibrary\steamapps\common\Stardew Valley\Stardew Valley.dll
// XML documentation location: D:\SteamLibrary\steamapps\common\Stardew Valley\Stardew Valley.xml

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using StardewValley.Locations;
using System;
using System.Collections.Generic;
using System.Linq;

#nullable disable
namespace StardewValley.Menus;

public class SkillsPage : IClickableMenu
{
  public const int region_special1 = 10201;
  public const int region_special2 = 10202;
  public const int region_special3 = 10203;
  public const int region_special4 = 10204;
  public const int region_special5 = 10205;
  public const int region_special6 = 10206;
  public const int region_special7 = 10207;
  public const int region_special8 = 10208;
  public const int region_special9 = 10209;
  public const int region_special_skullkey = 10210;
  public const int region_special_townkey = 10211;
  public const int region_ccTracker = 30211;
  public const int region_skillArea1 = 0;
  public const int region_skillArea2 = 1;
  public const int region_skillArea3 = 2;
  public const int region_skillArea4 = 3;
  public const int region_skillArea5 = 4;
  public List<ClickableTextureComponent> skillBars = new List<ClickableTextureComponent>();
  public List<ClickableTextureComponent> skillAreas = new List<ClickableTextureComponent>();
  public List<ClickableTextureComponent> specialItems = new List<ClickableTextureComponent>();
  public List<ClickableComponent> ccTrackerButtons = new List<ClickableComponent>();
  private string hoverText = "";
  private string hoverTitle = "";
  private int professionImage = -1;
  private int playerPanelIndex;
  private int playerPanelTimer;
  private Rectangle playerPanel;
  private int[] playerPanelFrames = new int[4]{ 0, 1, 0, 2 };
  private int timesClickedJunimo;

  public SkillsPage(int x, int y, int width, int height)
    : base(x, y, width, height)
  {
    int positionOnScreen1 = this.xPositionOnScreen;
    int toClearSideBorder = IClickableMenu.spaceToClearSideBorder;
    int positionOnScreen2 = this.yPositionOnScreen;
    int toClearTopBorder = IClickableMenu.spaceToClearTopBorder;
    double num1 = (double) height / 2.0;
    this.playerPanel = new Rectangle(this.xPositionOnScreen + 64 /*0x40*/, this.yPositionOnScreen + IClickableMenu.borderWidth + IClickableMenu.spaceToClearTopBorder, 128 /*0x80*/, 192 /*0xC0*/);
    ClickableComponent.SetUpNeighbors<ClickableTextureComponent>(this.specialItems, 4);
    ClickableComponent.ChainNeighborsLeftRight<ClickableTextureComponent>(this.specialItems);
    if (!Game1.MasterPlayer.hasCompletedCommunityCenter() && !Game1.MasterPlayer.hasOrWillReceiveMail("JojaMember") && (Game1.MasterPlayer.hasOrWillReceiveMail("canReadJunimoText") || Game1.player.hasOrWillReceiveMail("canReadJunimoText")))
    {
      int num2 = this.yPositionOnScreen + IClickableMenu.spaceToClearTopBorder + (int) ((double) height / 2.0) + 21;
      int x1 = this.xPositionOnScreen + IClickableMenu.spaceToClearSideBorder * 2 + 80 /*0x50*/;
      int y1 = num2 + 16 /*0x10*/;
      CommunityCenter communityCenter = Game1.RequireLocation<CommunityCenter>("CommunityCenter");
      if (!Game1.MasterPlayer.hasOrWillReceiveMail("ccBulletin"))
        this.ccTrackerButtons.Add(new ClickableComponent(new Rectangle(x1, y1, 44, 44), 5.ToString() ?? "", communityCenter.shouldNoteAppearInArea(5) ? Game1.content.LoadString("Strings\\Locations:CommunityCenter_AreaName_BulletinBoard") : "???")
        {
          myID = 30211,
          downNeighborID = -99998,
          rightNeighborID = -99998,
          leftNeighborID = -99998,
          upNeighborID = 4
        });
      if (!Game1.MasterPlayer.hasOrWillReceiveMail("ccBoilerRoom"))
        this.ccTrackerButtons.Add(new ClickableComponent(new Rectangle(x1 + 60, y1 + 28, 44, 44), 3.ToString() ?? "", communityCenter.shouldNoteAppearInArea(3) ? Game1.content.LoadString("Strings\\Locations:CommunityCenter_AreaName_BoilerRoom") : "???")
        {
          myID = 30212,
          upNeighborID = 30211,
          leftNeighborID = 30211,
          downNeighborID = 30213,
          rightNeighborID = 4
        });
      if (!Game1.MasterPlayer.hasOrWillReceiveMail("ccVault"))
        this.ccTrackerButtons.Add(new ClickableComponent(new Rectangle(x1 + 60, y1 + 88, 44, 44), 4.ToString() ?? "", communityCenter.shouldNoteAppearInArea(4) ? Game1.content.LoadString("Strings\\Locations:CommunityCenter_AreaName_Vault") : "???")
        {
          myID = 30213,
          upNeighborID = 30212,
          downNeighborID = 30216,
          leftNeighborID = 30215
        });
      if (!Game1.MasterPlayer.hasOrWillReceiveMail("ccCraftsRoom"))
        this.ccTrackerButtons.Add(new ClickableComponent(new Rectangle(x1 - 60, y1 + 28, 44, 44), 1.ToString() ?? "", communityCenter.shouldNoteAppearInArea(1) ? Game1.content.LoadString("Strings\\Locations:CommunityCenter_AreaName_CraftsRoom") : "???")
        {
          myID = 30214,
          upNeighborID = 30211,
          downNeighborID = 30215,
          rightNeighborID = 30212
        });
      if (!Game1.MasterPlayer.hasOrWillReceiveMail("ccFishTank"))
        this.ccTrackerButtons.Add(new ClickableComponent(new Rectangle(x1 - 60, y1 + 88, 44, 44), 2.ToString() ?? "", communityCenter.shouldNoteAppearInArea(2) ? Game1.content.LoadString("Strings\\Locations:CommunityCenter_AreaName_FishTank") : "???")
        {
          myID = 30215,
          upNeighborID = 30214,
          downNeighborID = 30216,
          rightNeighborID = 30213
        });
      if (!Game1.MasterPlayer.hasOrWillReceiveMail("ccPantry"))
        this.ccTrackerButtons.Add(new ClickableComponent(new Rectangle(x1, y1 + 120, 44, 44), 0.ToString() ?? "", communityCenter.shouldNoteAppearInArea(0) ? Game1.content.LoadString("Strings\\Locations:CommunityCenter_AreaName_Pantry") : "???")
        {
          myID = 30216,
          upNeighborID = 30211,
          rightNeighborID = 30213,
          leftNeighborID = 30215
        });
    }
    int num3 = 0;
    int num4 = LocalizedContentManager.CurrentLanguageCode == LocalizedContentManager.LanguageCode.ru || LocalizedContentManager.CurrentLanguageCode == LocalizedContentManager.LanguageCode.it ? this.xPositionOnScreen + width - 448 - 48 /*0x30*/ + 4 : this.xPositionOnScreen + IClickableMenu.borderWidth + IClickableMenu.spaceToClearTopBorder + 256 /*0x0100*/ - 4;
    int num5 = this.yPositionOnScreen + IClickableMenu.spaceToClearTopBorder + IClickableMenu.borderWidth - 12;
    for (int index1 = 4; index1 < 10; index1 += 5)
    {
      for (int index2 = 0; index2 < 5; ++index2)
      {
        string professionBlurb = "";
        string professionTitle = "";
        bool flag = false;
        int whichProfession = -1;
        switch (index2)
        {
          case 0:
            flag = Game1.player.FarmingLevel > index1;
            whichProfession = Game1.player.getProfessionForSkill(0, index1 + 1);
            this.parseProfessionDescription(ref professionBlurb, ref professionTitle, LevelUpMenu.getProfessionDescription(whichProfession));
            break;
          case 1:
            flag = Game1.player.MiningLevel > index1;
            whichProfession = Game1.player.getProfessionForSkill(3, index1 + 1);
            this.parseProfessionDescription(ref professionBlurb, ref professionTitle, LevelUpMenu.getProfessionDescription(whichProfession));
            break;
          case 2:
            flag = Game1.player.ForagingLevel > index1;
            whichProfession = Game1.player.getProfessionForSkill(2, index1 + 1);
            this.parseProfessionDescription(ref professionBlurb, ref professionTitle, LevelUpMenu.getProfessionDescription(whichProfession));
            break;
          case 3:
            flag = Game1.player.FishingLevel > index1;
            whichProfession = Game1.player.getProfessionForSkill(1, index1 + 1);
            this.parseProfessionDescription(ref professionBlurb, ref professionTitle, LevelUpMenu.getProfessionDescription(whichProfession));
            break;
          case 4:
            flag = Game1.player.CombatLevel > index1;
            whichProfession = Game1.player.getProfessionForSkill(4, index1 + 1);
            this.parseProfessionDescription(ref professionBlurb, ref professionTitle, LevelUpMenu.getProfessionDescription(whichProfession));
            break;
          case 5:
            flag = Game1.player.LuckLevel > index1;
            whichProfession = Game1.player.getProfessionForSkill(5, index1 + 1);
            this.parseProfessionDescription(ref professionBlurb, ref professionTitle, LevelUpMenu.getProfessionDescription(whichProfession));
            break;
        }
        if (flag && (index1 + 1) % 5 == 0)
        {
          List<ClickableTextureComponent> skillBars = this.skillBars;
          ClickableTextureComponent textureComponent = new ClickableTextureComponent(whichProfession.ToString() ?? "", new Rectangle(num3 + num4 - 4 + index1 * 36, num5 + index2 * 68, 56, 36), (string) null, professionBlurb, Game1.mouseCursors, new Rectangle(159, 338, 14, 9), 4f, true);
          textureComponent.myID = index1 + 1 == 5 ? 100 + index2 : 200 + index2;
          textureComponent.leftNeighborID = index1 + 1 == 5 ? index2 : 100 + index2;
          textureComponent.rightNeighborID = index1 + 1 == 5 ? 200 + index2 : -1;
          textureComponent.downNeighborID = -99998;
          skillBars.Add(textureComponent);
        }
      }
      num3 += 24;
    }
    for (int index = 0; index < this.skillBars.Count; ++index)
    {
      if (index < this.skillBars.Count - 1 && Math.Abs(this.skillBars[index + 1].myID - this.skillBars[index].myID) < 50)
      {
        this.skillBars[index].downNeighborID = this.skillBars[index + 1].myID;
        this.skillBars[index + 1].upNeighborID = this.skillBars[index].myID;
      }
    }
    if (this.skillBars.Count > 1 && this.skillBars.Last<ClickableTextureComponent>().myID >= 200 && this.skillBars[this.skillBars.Count - 2].myID >= 200)
      this.skillBars.Last<ClickableTextureComponent>().upNeighborID = this.skillBars[this.skillBars.Count - 2].myID;
    for (int index = 0; index < 5; ++index)
    {
      int num6 = index == 1 ? 3 : (index == 3 ? 1 : index);
      string hoverText = "";
      switch (num6)
      {
        case 0:
          if (Game1.player.FarmingLevel > 0)
          {
            hoverText = Game1.content.LoadString("Strings\\StringsFromCSFiles:SkillsPage.cs.11592", (object) Game1.player.FarmingLevel) + Environment.NewLine + Game1.content.LoadString("Strings\\StringsFromCSFiles:SkillsPage.cs.11594", (object) Game1.player.FarmingLevel);
            break;
          }
          break;
        case 1:
          if (Game1.player.FishingLevel > 0)
          {
            hoverText = Game1.content.LoadString("Strings\\StringsFromCSFiles:SkillsPage.cs.11598", (object) Game1.player.FishingLevel);
            break;
          }
          break;
        case 2:
          if (Game1.player.ForagingLevel > 0)
          {
            hoverText = Game1.content.LoadString("Strings\\StringsFromCSFiles:SkillsPage.cs.11596", (object) Game1.player.ForagingLevel);
            break;
          }
          break;
        case 3:
          if (Game1.player.MiningLevel > 0)
          {
            hoverText = Game1.content.LoadString("Strings\\StringsFromCSFiles:SkillsPage.cs.11600", (object) Game1.player.MiningLevel);
            break;
          }
          break;
        case 4:
          if (Game1.player.CombatLevel > 0)
          {
            hoverText = Game1.content.LoadString("Strings\\StringsFromCSFiles:SkillsPage.cs.11602", (object) (Game1.player.CombatLevel * 5));
            break;
          }
          break;
      }
      List<ClickableTextureComponent> skillAreas = this.skillAreas;
      ClickableTextureComponent textureComponent = new ClickableTextureComponent(num6.ToString() ?? "", new Rectangle(num4 - 128 /*0x80*/ - 48 /*0x30*/, num5 + index * 68, 148, 36), num6.ToString() ?? "", hoverText, (Texture2D) null, Rectangle.Empty, 1f);
      textureComponent.myID = index;
      textureComponent.downNeighborID = index < 4 ? index + 1 : -99998;
      textureComponent.upNeighborID = index > 0 ? index - 1 : 12341;
      textureComponent.rightNeighborID = 100 + index;
      skillAreas.Add(textureComponent);
    }
  }

  private void parseProfessionDescription(
    ref string professionBlurb,
    ref string professionTitle,
    List<string> professionDescription)
  {
    if (professionDescription.Count <= 0)
      return;
    professionTitle = professionDescription[0];
    for (int index = 1; index < professionDescription.Count; ++index)
    {
      professionBlurb += professionDescription[index];
      if (index < professionDescription.Count - 1)
        professionBlurb += Environment.NewLine;
    }
  }

  public override void snapToDefaultClickableComponent()
  {
    this.currentlySnappedComponent = this.skillAreas.Count > 0 ? this.getComponentWithID(0) : (ClickableComponent) null;
    this.snapCursorToCurrentSnappedComponent();
  }

  /// <inheritdoc />
  public override void receiveLeftClick(int x, int y, bool playSound = true)
  {
    if (x > this.xPositionOnScreen + IClickableMenu.spaceToClearSideBorder * 2 && x < this.xPositionOnScreen + IClickableMenu.spaceToClearSideBorder * 2 + 200 && y > this.yPositionOnScreen + IClickableMenu.spaceToClearTopBorder + (int) ((double) this.height / 2.0) + 21 && y < this.yPositionOnScreen + this.height && Game1.MasterPlayer.hasCompletedCommunityCenter() && !Game1.MasterPlayer.hasOrWillReceiveMail("JojaMember") && !Game1.player.mailReceived.Contains("activatedJungleJunimo"))
    {
      ++this.timesClickedJunimo;
      if (this.timesClickedJunimo > 6)
      {
        Game1.playSound("discoverMineral");
        Game1.playSound("leafrustle");
        Game1.player.mailReceived.Add("activatedJungleJunimo");
      }
      else
        Game1.playSound("hammer");
    }
    foreach (ClickableComponent ccTrackerButton in this.ccTrackerButtons)
    {
      if (ccTrackerButton != null && ccTrackerButton.containsPoint(x, y) && !ccTrackerButton.label.Equals("???"))
      {
        Game1.activeClickableMenu = (IClickableMenu) new JunimoNoteMenu(true, Convert.ToInt32(ccTrackerButton.name), true)
        {
          gameMenuTabToReturnTo = GameMenu.skillsTab
        };
        break;
      }
    }
  }

  /// <inheritdoc />
  public override void performHoverAction(int x, int y)
  {
    this.hoverText = "";
    this.hoverTitle = "";
    this.professionImage = -1;
    foreach (ClickableComponent ccTrackerButton in this.ccTrackerButtons)
    {
      if (ccTrackerButton != null && ccTrackerButton.containsPoint(x, y))
      {
        this.hoverText = ccTrackerButton.label;
        break;
      }
    }
    foreach (ClickableTextureComponent skillBar in this.skillBars)
    {
      skillBar.scale = 4f;
      if (skillBar.containsPoint(x, y) && skillBar.hoverText.Length > 0 && !skillBar.name.Equals("-1"))
      {
        this.hoverText = skillBar.hoverText;
        this.hoverTitle = LevelUpMenu.getProfessionTitleFromNumber(Convert.ToInt32(skillBar.name));
        this.professionImage = Convert.ToInt32(skillBar.name);
        skillBar.scale = 0.0f;
      }
    }
    foreach (ClickableTextureComponent skillArea in this.skillAreas)
    {
      if (skillArea.containsPoint(x, y) && skillArea.hoverText.Length > 0)
      {
        this.hoverText = skillArea.hoverText;
        this.hoverTitle = Farmer.getSkillDisplayNameFromIndex(Convert.ToInt32(skillArea.name));
        break;
      }
    }
    if (this.playerPanel.Contains(x, y))
    {
      this.playerPanelTimer -= Game1.currentGameTime.ElapsedGameTime.Milliseconds;
      if (this.playerPanelTimer > 0)
        return;
      this.playerPanelIndex = (this.playerPanelIndex + 1) % 4;
      this.playerPanelTimer = 150;
    }
    else
      this.playerPanelIndex = 0;
  }

  /// <inheritdoc />
  public override void draw(SpriteBatch b)
  {
    int x1 = this.xPositionOnScreen + 64 /*0x40*/ - 8;
    int num1 = this.yPositionOnScreen + IClickableMenu.borderWidth + IClickableMenu.spaceToClearTopBorder;
    b.Draw(Game1.timeOfDay >= 1900 ? Game1.nightbg : Game1.daybg, new Vector2((float) x1, (float) (num1 - 16 /*0x10*/ - 4)), Color.White);
    FarmerRenderer.isDrawingForUI = true;
    Game1.player.FarmerRenderer.draw(b, new FarmerSprite.AnimationFrame(Game1.player.bathingClothes.Value ? 108 : this.playerPanelFrames[this.playerPanelIndex], 0, false, false), Game1.player.bathingClothes.Value ? 108 : this.playerPanelFrames[this.playerPanelIndex], new Rectangle(this.playerPanelFrames[this.playerPanelIndex] * 16 /*0x10*/, Game1.player.bathingClothes.Value ? 576 : 0, 16 /*0x10*/, 32 /*0x20*/), new Vector2((float) (x1 + 32 /*0x20*/), (float) (num1 + 16 /*0x10*/ - 4)), Vector2.Zero, 0.8f, 2, Color.White, 0.0f, 1f, Game1.player);
    if (Game1.timeOfDay >= 1900)
      Game1.player.FarmerRenderer.draw(b, new FarmerSprite.AnimationFrame(this.playerPanelFrames[this.playerPanelIndex], 0, false, false), this.playerPanelFrames[this.playerPanelIndex], new Rectangle(this.playerPanelFrames[this.playerPanelIndex] * 16 /*0x10*/, 0, 16 /*0x10*/, 32 /*0x20*/), new Vector2((float) (x1 + 32 /*0x20*/), (float) (num1 + 16 /*0x10*/ - 4)), Vector2.Zero, 0.8f, 2, Color.DarkBlue * 0.3f, 0.0f, 1f, Game1.player);
    FarmerRenderer.isDrawingForUI = false;
    b.Draw(Game1.staminaRect, new Rectangle(this.xPositionOnScreen + IClickableMenu.spaceToClearSideBorder * 2, this.yPositionOnScreen + IClickableMenu.spaceToClearTopBorder + (int) ((double) this.height / 2.0) + 21, this.width - IClickableMenu.spaceToClearSideBorder * 4 - 8, 4), new Color(214, 143, 84));
    b.DrawString(Game1.smallFont, Game1.player.Name, new Vector2((float) (x1 + 64 /*0x40*/) - Game1.smallFont.MeasureString(Game1.player.Name).X / 2f, (float) (num1 + 192 /*0xC0*/ - 17)), Game1.textColor);
    b.DrawString(Game1.smallFont, Game1.player.getTitle(), new Vector2((float) (x1 + 64 /*0x40*/) - Game1.smallFont.MeasureString(Game1.player.getTitle()).X / 2f, (float) (num1 + 256 /*0x0100*/ - 32 /*0x20*/ - 19)), Game1.textColor);
    int num2 = LocalizedContentManager.CurrentLanguageCode == LocalizedContentManager.LanguageCode.ru || LocalizedContentManager.CurrentLanguageCode == LocalizedContentManager.LanguageCode.it ? this.xPositionOnScreen + this.width - 448 - 48 /*0x30*/ : this.xPositionOnScreen + IClickableMenu.borderWidth + IClickableMenu.spaceToClearTopBorder + 256 /*0x0100*/ - 8;
    int num3 = this.yPositionOnScreen + IClickableMenu.spaceToClearTopBorder + IClickableMenu.borderWidth - 8;
    int num4 = 0;
    int num5 = 68;
    for (int index1 = 0; index1 < 10; ++index1)
    {
      for (int index2 = 0; index2 < 5; ++index2)
      {
        bool flag1 = false;
        bool flag2 = false;
        string text = "";
        int number = 0;
        Rectangle rectangle = Rectangle.Empty;
        switch (index2)
        {
          case 0:
            flag1 = Game1.player.FarmingLevel > index1;
            if (index1 == 0)
              text = Game1.content.LoadString("Strings\\StringsFromCSFiles:SkillsPage.cs.11604");
            number = Game1.player.FarmingLevel;
            flag2 = Game1.player.buffs.FarmingLevel > 0;
            rectangle = new Rectangle(10, 428, 10, 10);
            break;
          case 1:
            flag1 = Game1.player.MiningLevel > index1;
            if (index1 == 0)
              text = Game1.content.LoadString("Strings\\StringsFromCSFiles:SkillsPage.cs.11605");
            number = Game1.player.MiningLevel;
            flag2 = Game1.player.buffs.MiningLevel > 0;
            rectangle = new Rectangle(30, 428, 10, 10);
            break;
          case 2:
            flag1 = Game1.player.ForagingLevel > index1;
            if (index1 == 0)
              text = Game1.content.LoadString("Strings\\StringsFromCSFiles:SkillsPage.cs.11606");
            number = Game1.player.ForagingLevel;
            flag2 = Game1.player.buffs.ForagingLevel > 0;
            rectangle = new Rectangle(60, 428, 10, 10);
            break;
          case 3:
            flag1 = Game1.player.FishingLevel > index1;
            if (index1 == 0)
              text = Game1.content.LoadString("Strings\\StringsFromCSFiles:SkillsPage.cs.11607");
            number = Game1.player.FishingLevel;
            flag2 = Game1.player.buffs.FishingLevel > 0;
            rectangle = new Rectangle(20, 428, 10, 10);
            break;
          case 4:
            flag1 = Game1.player.CombatLevel > index1;
            if (index1 == 0)
              text = Game1.content.LoadString("Strings\\StringsFromCSFiles:SkillsPage.cs.11608");
            number = Game1.player.CombatLevel;
            flag2 = Game1.player.buffs.CombatLevel > 0;
            rectangle = new Rectangle(120, 428, 10, 10);
            break;
          case 5:
            flag1 = Game1.player.LuckLevel > index1;
            if (index1 == 0)
              text = Game1.content.LoadString("Strings\\StringsFromCSFiles:SkillsPage.cs.11609");
            number = Game1.player.LuckLevel;
            flag2 = Game1.player.buffs.LuckLevel > 0;
            rectangle = new Rectangle(50, 428, 10, 10);
            break;
        }
        if (!text.Equals(""))
        {
          b.DrawString(Game1.smallFont, text, new Vector2((float) ((double) num2 - (double) Game1.smallFont.MeasureString(text).X + 4.0 - 64.0), (float) (num3 + 4 + index2 * num5)), Game1.textColor);
          b.Draw(Game1.mouseCursors, new Vector2((float) (num2 - 56), (float) (num3 + index2 * num5)), new Rectangle?(rectangle), Color.Black * 0.3f, 0.0f, Vector2.Zero, 4f, SpriteEffects.None, 0.85f);
          b.Draw(Game1.mouseCursors, new Vector2((float) (num2 - 52), (float) (num3 - 4 + index2 * num5)), new Rectangle?(rectangle), Color.White, 0.0f, Vector2.Zero, 4f, SpriteEffects.None, 0.87f);
        }
        if (!flag1 && (index1 + 1) % 5 == 0)
        {
          b.Draw(Game1.mouseCursors, new Vector2((float) (num4 + num2 - 4 + index1 * 36), (float) (num3 + index2 * num5)), new Rectangle?(new Rectangle(145, 338, 14, 9)), Color.Black * 0.35f, 0.0f, Vector2.Zero, 4f, SpriteEffects.None, 0.87f);
          b.Draw(Game1.mouseCursors, new Vector2((float) (num4 + num2 + index1 * 36), (float) (num3 - 4 + index2 * num5)), new Rectangle?(new Rectangle(145 + (flag1 ? 14 : 0), 338, 14, 9)), Color.White * (flag1 ? 1f : 0.65f), 0.0f, Vector2.Zero, 4f, SpriteEffects.None, 0.87f);
        }
        else if ((index1 + 1) % 5 != 0)
        {
          b.Draw(Game1.mouseCursors, new Vector2((float) (num4 + num2 - 4 + index1 * 36), (float) (num3 + index2 * num5)), new Rectangle?(new Rectangle(129, 338, 8, 9)), Color.Black * 0.35f, 0.0f, Vector2.Zero, 4f, SpriteEffects.None, 0.85f);
          b.Draw(Game1.mouseCursors, new Vector2((float) (num4 + num2 + index1 * 36), (float) (num3 - 4 + index2 * num5)), new Rectangle?(new Rectangle(129 + (flag1 ? 8 : 0), 338, 8, 9)), Color.White * (flag1 ? 1f : 0.65f), 0.0f, Vector2.Zero, 4f, SpriteEffects.None, 0.87f);
        }
        if (index1 == 9)
        {
          NumberSprite.draw(number, b, new Vector2((float) (num4 + num2 + (index1 + 2) * 36 + 12 + (number >= 10 ? 12 : 0)), (float) (num3 + 16 /*0x10*/ + index2 * num5)), Color.Black * 0.35f, 1f, 0.85f, 1f, 0);
          NumberSprite.draw(number, b, new Vector2((float) (num4 + num2 + (index1 + 2) * 36 + 16 /*0x10*/ + (number >= 10 ? 12 : 0)), (float) (num3 + 12 + index2 * num5)), (flag2 ? Color.LightGreen : Color.SandyBrown) * (number == 0 ? 0.75f : 1f), 1f, 0.87f, 1f, 0);
        }
      }
      if ((index1 + 1) % 5 == 0)
        num4 += 24;
    }
    foreach (ClickableTextureComponent skillBar in this.skillBars)
      skillBar.draw(b);
    foreach (ClickableTextureComponent skillBar in this.skillBars)
    {
      if ((double) skillBar.scale == 0.0)
      {
        IClickableMenu.drawTextureBox(b, skillBar.bounds.X - 16 /*0x10*/ - 8, skillBar.bounds.Y - 16 /*0x10*/ - 16 /*0x10*/, 96 /*0x60*/, 96 /*0x60*/, Color.White);
        b.Draw(Game1.mouseCursors, new Vector2((float) (skillBar.bounds.X - 8), (float) (skillBar.bounds.Y - 32 /*0x20*/ + 16 /*0x10*/)), new Rectangle?(new Rectangle(this.professionImage % 6 * 16 /*0x10*/, 624 + this.professionImage / 6 * 16 /*0x10*/, 16 /*0x10*/, 16 /*0x10*/)), Color.White, 0.0f, Vector2.Zero, 4f, SpriteEffects.None, 1f);
      }
    }
    int num6 = this.xPositionOnScreen + IClickableMenu.spaceToClearSideBorder + 32 /*0x20*/ - 8;
    int y1 = this.yPositionOnScreen + IClickableMenu.spaceToClearTopBorder + 320 - 36;
    int num7;
    if (Game1.netWorldState.Value.GoldenWalnuts > 0)
    {
      b.Draw(Game1.objectSpriteSheet, new Vector2((float) (num6 + (Game1.player.QiGems <= 0 ? 24 : 0)), (float) y1), new Rectangle?(Game1.getSourceRectForStandardTileSheet(Game1.objectSpriteSheet, 73, 16 /*0x10*/, 16 /*0x10*/)), Color.White, 0.0f, Vector2.Zero, 2f, SpriteEffects.None, 0.0f);
      int x2 = num6 + (Game1.player.QiGems <= 0 ? 60 : 36);
      SpriteBatch spriteBatch = b;
      SpriteFont smallFont = Game1.smallFont;
      num7 = Game1.netWorldState.Value.GoldenWalnuts;
      string text = num7.ToString() ?? "";
      Vector2 position = new Vector2((float) x2, (float) y1);
      Color textColor = Game1.textColor;
      spriteBatch.DrawString(smallFont, text, position, textColor);
      num6 = x2 + 56;
    }
    if (Game1.player.QiGems > 0)
    {
      b.Draw(Game1.objectSpriteSheet, new Vector2((float) (num6 + (Game1.netWorldState.Value.GoldenWalnuts <= 0 ? 24 : 0)), (float) y1), new Rectangle?(Game1.getSourceRectForStandardTileSheet(Game1.objectSpriteSheet, 858, 16 /*0x10*/, 16 /*0x10*/)), Color.White, 0.0f, Vector2.Zero, 2f, SpriteEffects.None, 0.0f);
      int x3 = num6 + (Game1.netWorldState.Value.GoldenWalnuts <= 0 ? 60 : 36);
      SpriteBatch spriteBatch = b;
      SpriteFont smallFont = Game1.smallFont;
      num7 = Game1.player.QiGems;
      string text = num7.ToString() ?? "";
      Vector2 position = new Vector2((float) x3, (float) y1);
      Color textColor = Game1.textColor;
      spriteBatch.DrawString(smallFont, text, position, textColor);
      int num8 = x3 + 64 /*0x40*/;
    }
    int num9 = this.yPositionOnScreen + IClickableMenu.spaceToClearTopBorder + (int) ((double) this.height / 2.0) + 21;
    int num10 = this.xPositionOnScreen + IClickableMenu.spaceToClearSideBorder * 2;
    bool flag3 = Game1.MasterPlayer.mailReceived.Contains("JojaMember");
    int x4 = num10 + 80 /*0x50*/;
    int y2 = num9 + 16 /*0x10*/;
    if (flag3 || Game1.MasterPlayer.hasOrWillReceiveMail("canReadJunimoText") || Game1.player.hasOrWillReceiveMail("canReadJunimoText"))
    {
      if (!flag3)
        b.Draw(Game1.mouseCursors_1_6, new Vector2((float) x4, (float) y2), new Rectangle?(new Rectangle(Game1.MasterPlayer.hasOrWillReceiveMail("ccBulletin") ? 374 : 363, 298 + (flag3 ? 11 : 0), 11, 11)), Color.White, 0.0f, Vector2.Zero, 4f, SpriteEffects.None, 0.7f);
      else
        b.Draw(Game1.mouseCursors_1_6, new Vector2((float) (x4 - 80 /*0x50*/), (float) (y2 - 16 /*0x10*/)), new Rectangle?(new Rectangle(363, 250, 51, 48 /*0x30*/)), Color.White, 0.0f, Vector2.Zero, 4f, SpriteEffects.None, 0.7f);
      b.Draw(Game1.mouseCursors_1_6, new Vector2((float) (x4 + 60), (float) (y2 + 28)), new Rectangle?(new Rectangle(Game1.MasterPlayer.hasOrWillReceiveMail("ccBoilerRoom") ? 374 : 363, 298 + (flag3 ? 11 : 0), 11, 11)), Color.White, 0.0f, Vector2.Zero, 4f, SpriteEffects.None, 0.7f);
      b.Draw(Game1.mouseCursors_1_6, new Vector2((float) (x4 + 60), (float) (y2 + 88)), new Rectangle?(new Rectangle(Game1.MasterPlayer.hasOrWillReceiveMail("ccVault") ? 374 : 363, 298 + (flag3 ? 11 : 0), 11, 11)), Color.White, 0.0f, Vector2.Zero, 4f, SpriteEffects.None, 0.7f);
      b.Draw(Game1.mouseCursors_1_6, new Vector2((float) (x4 - 60), (float) (y2 + 28)), new Rectangle?(new Rectangle(Game1.MasterPlayer.hasOrWillReceiveMail("ccCraftsRoom") ? 374 : 363, 298 + (flag3 ? 11 : 0), 11, 11)), Color.White, 0.0f, Vector2.Zero, 4f, SpriteEffects.None, 0.7f);
      b.Draw(Game1.mouseCursors_1_6, new Vector2((float) (x4 - 60), (float) (y2 + 88)), new Rectangle?(new Rectangle(Game1.MasterPlayer.hasOrWillReceiveMail("ccFishTank") ? 374 : 363, 298 + (flag3 ? 11 : 0), 11, 11)), Color.White, 0.0f, Vector2.Zero, 4f, SpriteEffects.None, 0.7f);
      b.Draw(Game1.mouseCursors_1_6, new Vector2((float) x4, (float) (y2 + 120)), new Rectangle?(new Rectangle(Game1.MasterPlayer.hasOrWillReceiveMail("ccPantry") ? 374 : 363, 298 + (flag3 ? 11 : 0), 11, 11)), Color.White, 0.0f, Vector2.Zero, 4f, SpriteEffects.None, 0.7f);
      if (!Utility.hasFinishedJojaRoute() && Game1.MasterPlayer.hasCompletedCommunityCenter())
      {
        b.Draw(Game1.mouseCursors_1_6, new Vector2((float) (x4 - 4) + 30f, (float) (y2 + 52) + 30f), new Rectangle?(new Rectangle(386, 299, 13, 15)), Color.White, 0.0f, new Vector2(7.5f), (float) (4.0 + (double) this.timesClickedJunimo * 0.20000000298023224), SpriteEffects.None, 0.7f);
        if (Game1.player.mailReceived.Contains("activatedJungleJunimo"))
          b.Draw(Game1.mouseCursors_1_6, new Vector2((float) (x4 - 80 /*0x50*/), (float) (y2 - 16 /*0x10*/)), new Rectangle?(new Rectangle(311, 251, 51, 48 /*0x30*/)), Color.White, 0.0f, Vector2.Zero, 4f, SpriteEffects.None, 0.7f);
      }
    }
    else
      b.Draw(Game1.mouseCursors_1_6, new Vector2((float) (x4 - 80 /*0x50*/), (float) (y2 - 16 /*0x10*/)), new Rectangle?(new Rectangle(414, 250, 52, 47)), Color.White, 0.0f, Vector2.Zero, 4f, SpriteEffects.None, 0.7f);
    int x5 = x4 + 124;
    b.Draw(Game1.staminaRect, new Rectangle(x5, y2 - 16 /*0x10*/, 4, (int) ((double) this.height / 3.0) - 32 /*0x20*/ - 4), new Color(214, 143, 84));
    int num11 = 0;
    if ((double) Game1.smallFont.MeasureString(Game1.content.LoadString("Strings\\UI:Inventory_PortraitHover_Level", (object) (Game1.player.houseUpgradeLevel.Value + 1))).X > 120.0)
      num11 -= 20;
    int y3 = y2 + 108;
    int num12 = x5 + 28;
    b.Draw(Game1.mouseCursors, new Vector2((float) (num12 + num11 + 20), (float) (y3 - 4)), new Rectangle?(new Rectangle(653, 880, 10, 10)), Color.White, 0.0f, Vector2.Zero, 4f, SpriteEffects.None, 0.7f);
    Utility.drawTextWithShadow(b, Game1.content.LoadString("Strings\\UI:Inventory_PortraitHover_Level", (object) (Game1.player.houseUpgradeLevel.Value + 1)), Game1.smallFont, new Vector2((float) (num12 + num11 + 72), (float) y3), Game1.textColor);
    if (Game1.player.houseUpgradeLevel.Value >= 3)
    {
      int num13 = 709;
      SpriteBatch spriteBatch1 = b;
      Texture2D mouseCursors1 = Game1.mouseCursors;
      Vector2 position1 = new Vector2((float) (num12 + num11) + 50f, (float) y3 - 4f) + new Vector2(0.0f, (float) (-Game1.currentGameTime.TotalGameTime.TotalMilliseconds % 2000.0 * 0.0099999997764825821));
      Rectangle? sourceRectangle1 = new Rectangle?(new Rectangle(372, 1956, 10, 10));
      Color color1 = new Color(80 /*0x50*/, 80 /*0x50*/, 80 /*0x50*/) * 1f * 0.53f;
      TimeSpan totalGameTime1 = Game1.currentGameTime.TotalGameTime;
      double num14 = 1.0 - totalGameTime1.TotalMilliseconds % 2000.0 / 2000.0;
      Color color2 = color1 * (float) num14;
      totalGameTime1 = Game1.currentGameTime.TotalGameTime;
      double rotation1 = -totalGameTime1.TotalMilliseconds % 2000.0 * (1.0 / 1000.0);
      Vector2 origin1 = new Vector2(3f, 3f);
      totalGameTime1 = Game1.currentGameTime.TotalGameTime;
      double scale1 = 0.5 + totalGameTime1.TotalMilliseconds % 2000.0 / 1000.0;
      spriteBatch1.Draw(mouseCursors1, position1, sourceRectangle1, color2, (float) rotation1, origin1, (float) scale1, SpriteEffects.None, 0.7f);
      SpriteBatch spriteBatch2 = b;
      Texture2D mouseCursors2 = Game1.mouseCursors;
      Vector2 position2 = new Vector2((float) (num12 + num11) + 50f, (float) y3 - 4f) + new Vector2(0.0f, (float) (-(Game1.currentGameTime.TotalGameTime.TotalMilliseconds + (double) num13) % 2000.0 * 0.0099999997764825821));
      Rectangle? sourceRectangle2 = new Rectangle?(new Rectangle(372, 1956, 10, 10));
      Color color3 = new Color(80 /*0x50*/, 80 /*0x50*/, 80 /*0x50*/) * 1f * 0.53f;
      TimeSpan totalGameTime2 = Game1.currentGameTime.TotalGameTime;
      double num15 = 1.0 - (totalGameTime2.TotalMilliseconds + (double) num13) % 2000.0 / 2000.0;
      Color color4 = color3 * (float) num15;
      totalGameTime2 = Game1.currentGameTime.TotalGameTime;
      double rotation2 = -(totalGameTime2.TotalMilliseconds + (double) num13) % 2000.0 * (1.0 / 1000.0);
      Vector2 origin2 = new Vector2(5f, 5f);
      double scale2 = 0.5 + (Game1.currentGameTime.TotalGameTime.TotalMilliseconds + (double) num13) % 2000.0 / 1000.0;
      spriteBatch2.Draw(mouseCursors2, position2, sourceRectangle2, color4, (float) rotation2, origin2, (float) scale2, SpriteEffects.None, 0.7f);
      SpriteBatch spriteBatch3 = b;
      Texture2D mouseCursors3 = Game1.mouseCursors;
      Vector2 position3 = new Vector2((float) (num12 + num11) + 50f, (float) y3 - 4f) + new Vector2(0.0f, (float) (-(Game1.currentGameTime.TotalGameTime.TotalMilliseconds + (double) (num13 * 2)) % 2000.0 * 0.0099999997764825821));
      Rectangle? sourceRectangle3 = new Rectangle?(new Rectangle(372, 1956, 10, 10));
      Color color5 = new Color(80 /*0x50*/, 80 /*0x50*/, 80 /*0x50*/) * 1f * 0.53f * (float) (1.0 - (Game1.currentGameTime.TotalGameTime.TotalMilliseconds + (double) (num13 * 2)) % 2000.0 / 2000.0);
      TimeSpan totalGameTime3 = Game1.currentGameTime.TotalGameTime;
      double rotation3 = -(totalGameTime3.TotalMilliseconds + (double) (num13 * 2)) % 2000.0 * (1.0 / 1000.0);
      Vector2 origin3 = new Vector2(4f, 4f);
      totalGameTime3 = Game1.currentGameTime.TotalGameTime;
      double scale3 = 0.5 + (totalGameTime3.TotalMilliseconds + (double) (num13 * 2)) % 2000.0 / 1000.0;
      spriteBatch3.Draw(mouseCursors3, position3, sourceRectangle3, color5, (float) rotation3, origin3, (float) scale3, SpriteEffects.None, 0.7f);
    }
    int num16 = num12 + 180;
    int y4 = y3 - 8;
    bool flag4 = false;
    int lowestLevelReached = MineShaft.lowestLevelReached;
    if (lowestLevelReached > 120)
    {
      lowestLevelReached -= 120;
      flag4 = true;
    }
    b.Draw(Game1.mouseCursors_1_6, new Vector2((float) (num16 + 8), (float) y4), new Rectangle?(new Rectangle(lowestLevelReached == 0 ? 434 : 385, 315, 13, 13)), Color.White, 0.0f, Vector2.Zero, 4f, SpriteEffects.None, 0.7f);
    if (lowestLevelReached != 0)
      Utility.drawTextWithShadow(b, lowestLevelReached.ToString() ?? "", Game1.smallFont, new Vector2((float) (num16 + 72 + (flag4 ? 8 : 0)), (float) (y4 + 8)), Game1.textColor);
    if (flag4)
      b.Draw(Game1.mouseCursors_1_6, new Vector2((float) (num16 + 40), (float) (y4 + 24)), new Rectangle?(new Rectangle(412, 319, 8, 9)), Color.White, 0.0f, Vector2.Zero, 4f, SpriteEffects.None, 0.7f);
    int num17 = num16 + 120;
    int num18 = Utility.numStardropsFound();
    if (num18 > 0)
    {
      b.Draw(Game1.mouseCursors_1_6, new Vector2((float) (num17 + 32 /*0x20*/), (float) (y4 - 4)), new Rectangle?(new Rectangle(399, 314, 12, 14)), Color.White, 0.0f, Vector2.Zero, 4f, SpriteEffects.None, 0.7f);
      Utility.drawTextWithShadow(b, "x " + num18.ToString(), Game1.smallFont, new Vector2((float) (num17 + 88), (float) (y4 + 8)), num18 >= 7 ? new Color(160 /*0xA0*/, 30, 235) : Game1.textColor);
    }
    else
      b.Draw(Game1.mouseCursors_1_6, new Vector2((float) (num17 + 32 /*0x20*/), (float) (y4 - 4)), new Rectangle?(new Rectangle(421, 314, 12, 14)), Color.White, 0.0f, Vector2.Zero, 4f, SpriteEffects.None, 0.7f);
    if (Game1.stats.Get("MasteryExp") > 0U)
    {
      int currentMasteryLevel = MasteryTrackerMenu.getCurrentMasteryLevel();
      string text = Game1.content.LoadString("Strings\\1_6_Strings:Mastery").TrimEnd(':');
      float x6 = Game1.smallFont.MeasureString(text).X;
      int num19 = (int) x6 - 64 /*0x40*/;
      int num20 = 84;
      b.DrawString(Game1.smallFont, text, new Vector2((float) (this.xPositionOnScreen + 256 /*0x0100*/), (float) (num20 + this.yPositionOnScreen + 408)), Game1.textColor);
      Utility.drawWithShadow(b, Game1.mouseCursors_1_6, new Vector2((float) (num19 + this.xPositionOnScreen + 332), (float) (num20 + this.yPositionOnScreen + 400)), new Rectangle(457, 298, 11, 11), Color.White, 0.0f, Vector2.Zero);
      float widthScale = 0.64f - (float) (((double) x6 - 100.0) / 800.0);
      if (Game1.content.GetCurrentLanguage() == LocalizedContentManager.LanguageCode.ru)
        widthScale += 0.1f;
      b.Draw(Game1.staminaRect, new Rectangle(num19 + this.xPositionOnScreen + 380 - 1, num20 + this.yPositionOnScreen + 408, (int) (584.0 * (double) widthScale) + 4, 40), Color.Black * 0.35f);
      b.Draw(Game1.staminaRect, new Rectangle(num19 + this.xPositionOnScreen + 384, num20 + this.yPositionOnScreen + 404, (int) ((double) ((currentMasteryLevel >= 5 ? 144 /*0x90*/ : 146) * 4) * (double) widthScale) + 4, 40), new Color(60, 60, 25));
      b.Draw(Game1.staminaRect, new Rectangle(num19 + this.xPositionOnScreen + 388, num20 + this.yPositionOnScreen + 408, (int) (576.0 * (double) widthScale), 32 /*0x20*/), new Color(173, 129, 79));
      MasteryTrackerMenu.drawBar(b, new Vector2((float) (num19 + this.xPositionOnScreen + 276), (float) (num20 + this.yPositionOnScreen + 264)), widthScale);
      NumberSprite.draw(currentMasteryLevel, b, new Vector2((float) (num19 + this.xPositionOnScreen + 408 + (int) (584.0 * (double) widthScale)), (float) (num20 + this.yPositionOnScreen + 428)), Color.Black * 0.35f, 1f, 0.85f, 1f, 0);
      NumberSprite.draw(currentMasteryLevel, b, new Vector2((float) (num19 + this.xPositionOnScreen + 412 + (int) (584.0 * (double) widthScale)), (float) (num20 + this.yPositionOnScreen + 424)), Color.SandyBrown * (currentMasteryLevel == 0 ? 0.75f : 1f), 1f, 0.87f, 1f, 0);
    }
    else
      b.Draw(Game1.mouseCursors_1_6, new Vector2((float) (num17 - 304), (float) (y4 - 88)), new Rectangle?(new Rectangle(366, 236, 142, 12)), Color.White, 0.0f, Vector2.Zero, 4f, SpriteEffects.None, 0.7f);
    Rectangle rectangle1 = new Rectangle(394, 120 + Game1.seasonIndex * 23, 33, 23);
    if (Game1.isGreenRain)
      rectangle1 = new Rectangle(427, 143, 33, 23);
    else if (Game1.player.activeDialogueEvents.ContainsKey("married"))
      rectangle1 = new Rectangle(427, 97, 33, 23);
    else if (Game1.IsSpring && Game1.dayOfMonth == 13)
      rectangle1.X += 33;
    else if (Game1.IsSummer && Game1.dayOfMonth == 11)
      rectangle1.X += 66;
    else if (Game1.IsFall && Game1.dayOfMonth == 27)
      rectangle1.X += 33;
    else if (Game1.IsWinter && Game1.dayOfMonth == 25)
      rectangle1.X += 33;
    b.Draw(Game1.mouseCursors_1_6, new Vector2((float) (num17 + 144 /*0x90*/), (float) (y4 - 20)), new Rectangle?(rectangle1), Color.White, 0.0f, Vector2.Zero, 4f, SpriteEffects.None, 0.7f);
    if (Game1.IsWinter && Game1.player.mailReceived.Contains("sawSecretSanta" + Game1.year.ToString()) && (Game1.dayOfMonth >= 18 && Game1.dayOfMonth < 25 || Game1.dayOfMonth == 25 && Game1.timeOfDay < 1500))
    {
      NPC winterStarParticipant = Utility.GetRandomWinterStarParticipant();
      Texture2D texture;
      try
      {
        texture = Game1.content.Load<Texture2D>($"Characters\\{winterStarParticipant.Name}_Winter");
      }
      catch
      {
        texture = winterStarParticipant.Sprite.Texture;
      }
      Rectangle mugShotSourceRect = winterStarParticipant.getMugShotSourceRect();
      mugShotSourceRect.Height -= 5;
      b.Draw(texture, new Vector2((float) (num17 + 180), (float) y4), new Rectangle?(mugShotSourceRect), Color.White, 0.0f, Vector2.Zero, 4f, SpriteEffects.None, 0.7f);
      b.Draw(Game1.mouseCursors, new Vector2((float) (num17 + 244), (float) (y4 + 40)), new Rectangle?(new Rectangle(147, 412, 10, 11)), Color.White, 0.0f, Vector2.Zero, 4f, SpriteEffects.None, 0.7f);
    }
    if (this.hoverText.Length <= 0)
      return;
    IClickableMenu.drawHoverText(b, this.hoverText, Game1.smallFont, boldTitleText: this.hoverTitle.Length > 0 ? this.hoverTitle : (string) null);
  }
}
