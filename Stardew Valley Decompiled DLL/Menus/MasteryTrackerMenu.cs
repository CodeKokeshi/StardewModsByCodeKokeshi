// Decompiled with JetBrains decompiler
// Type: StardewValley.Menus.MasteryTrackerMenu
// Assembly: Stardew Valley, Version=1.6.15.24356, Culture=neutral, PublicKeyToken=null
// MVID: AA1F513A-94F0-4EF6-A35F-2D5B4A3721BF
// Assembly location: D:\SteamLibrary\steamapps\common\Stardew Valley\Stardew Valley.dll
// XML documentation location: D:\SteamLibrary\steamapps\common\Stardew Valley\Stardew Valley.xml

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using StardewValley.BellsAndWhistles;
using StardewValley.Constants;
using System;
using System.Collections.Generic;

#nullable disable
namespace StardewValley.Menus;

public class MasteryTrackerMenu : IClickableMenu
{
  public const int MASTERY_EXP_PER_LEVEL = 10000;
  public const int WIDTH = 200;
  public const int HEIGHT = 80 /*0x50*/;
  public ClickableTextureComponent mainButton;
  private float pressedButtonTimer;
  private float destroyTimer;
  private List<ClickableTextureComponent> rewards = new List<ClickableTextureComponent>();
  private int which;
  private bool canClaim;

  public MasteryTrackerMenu(int whichSkill = -1)
    : base((int) Utility.getTopLeftPositionForCenteringOnScreen(800, 320).X, (int) Utility.getTopLeftPositionForCenteringOnScreen(800, 320).Y, 800, 320, true)
  {
    this.which = whichSkill;
    this.closeSound = "stone_button";
    Texture2D texture = Game1.content.Load<Texture2D>("TileSheets\\Objects_2");
    switch (whichSkill)
    {
      case 0:
        List<ClickableTextureComponent> rewards1 = this.rewards;
        ClickableTextureComponent textureComponent1 = new ClickableTextureComponent(Rectangle.Empty, Game1.content.Load<Texture2D>("TileSheets\\weapons"), new Rectangle(32 /*0x20*/, 128 /*0x80*/, 16 /*0x10*/, 16 /*0x10*/), 4f, true);
        textureComponent1.name = Game1.content.LoadString("Strings\\Weapons:IridiumScythe_Name");
        textureComponent1.label = Game1.content.LoadString("Strings\\Weapons:IridiumScythe_Description");
        textureComponent1.hoverText = "(W)66";
        rewards1.Add(textureComponent1);
        List<ClickableTextureComponent> rewards2 = this.rewards;
        ClickableTextureComponent textureComponent2 = new ClickableTextureComponent(Rectangle.Empty, Game1.bigCraftableSpriteSheet, new Rectangle(32 /*0x20*/, 1152, 16 /*0x10*/, 32 /*0x20*/), 4f, true);
        textureComponent2.name = ItemRegistry.GetDataOrErrorItem("(BC)StatueOfBlessings").DisplayName;
        textureComponent2.label = ItemRegistry.GetDataOrErrorItem("(BC)StatueOfBlessings").Description;
        textureComponent2.myAlternateID = 1;
        textureComponent2.hoverText = "Statue Of Blessings";
        rewards2.Add(textureComponent2);
        List<ClickableTextureComponent> rewards3 = this.rewards;
        ClickableTextureComponent textureComponent3 = new ClickableTextureComponent(Rectangle.Empty, Game1.mouseCursors_1_6, new Rectangle(103, 90, 17, 16 /*0x10*/), 4f, true);
        textureComponent3.name = "";
        textureComponent3.label = Game1.content.LoadString("Strings\\1_6_Strings:Farming_Mastery");
        textureComponent3.myAlternateID = 0;
        rewards3.Add(textureComponent3);
        Game1.playSound("weed_cut");
        break;
      case 1:
        List<ClickableTextureComponent> rewards4 = this.rewards;
        ClickableTextureComponent textureComponent4 = new ClickableTextureComponent(Rectangle.Empty, Game1.toolSpriteSheet, new Rectangle(272, 0, 16 /*0x10*/, 16 /*0x10*/), 4f, true);
        textureComponent4.name = Game1.content.LoadString("Strings\\Tools:FishingRod_AdvancedIridium_Name");
        textureComponent4.label = Game1.content.LoadString("Strings\\Tools:FishingRod_AdvancedIridium_Description");
        textureComponent4.hoverText = "(T)AdvancedIridiumRod";
        rewards4.Add(textureComponent4);
        List<ClickableTextureComponent> rewards5 = this.rewards;
        ClickableTextureComponent textureComponent5 = new ClickableTextureComponent(Rectangle.Empty, texture, new Rectangle(0, 144 /*0x90*/, 16 /*0x10*/, 16 /*0x10*/), 4f, true);
        textureComponent5.name = ItemRegistry.GetDataOrErrorItem("(O)ChallengeBait").DisplayName;
        textureComponent5.label = ItemRegistry.GetDataOrErrorItem("(O)ChallengeBait").Description;
        textureComponent5.myAlternateID = 1;
        textureComponent5.hoverText = "Challenge Bait";
        rewards5.Add(textureComponent5);
        Game1.playSound("waterSlosh");
        List<ClickableTextureComponent> rewards6 = this.rewards;
        ClickableTextureComponent textureComponent6 = new ClickableTextureComponent(Rectangle.Empty, Game1.mouseCursors_1_6, new Rectangle(103, 90, 17, 16 /*0x10*/), 4f, true);
        textureComponent6.name = "";
        textureComponent6.label = Game1.content.LoadString("Strings\\1_6_Strings:Fishing_Mastery");
        textureComponent6.myAlternateID = 0;
        rewards6.Add(textureComponent6);
        break;
      case 2:
        List<ClickableTextureComponent> rewards7 = this.rewards;
        ClickableTextureComponent textureComponent7 = new ClickableTextureComponent(Rectangle.Empty, texture, new Rectangle(80 /*0x50*/, 112 /*0x70*/, 16 /*0x10*/, 16 /*0x10*/), 4f, true);
        textureComponent7.name = ItemRegistry.GetDataOrErrorItem("(O)MysticTreeSeed").DisplayName;
        textureComponent7.label = ItemRegistry.GetDataOrErrorItem("(O)MysticTreeSeed").Description;
        textureComponent7.myAlternateID = 1;
        textureComponent7.hoverText = "Mystic Tree Seed";
        rewards7.Add(textureComponent7);
        List<ClickableTextureComponent> rewards8 = this.rewards;
        ClickableTextureComponent textureComponent8 = new ClickableTextureComponent(Rectangle.Empty, texture, new Rectangle(112 /*0x70*/, 128 /*0x80*/, 16 /*0x10*/, 16 /*0x10*/), 4f, true);
        textureComponent8.name = ItemRegistry.GetDataOrErrorItem("(O)TreasureTotem").DisplayName;
        textureComponent8.label = ItemRegistry.GetDataOrErrorItem("(O)TreasureTotem").Description;
        textureComponent8.myAlternateID = 1;
        textureComponent8.hoverText = "Treasure Totem";
        rewards8.Add(textureComponent8);
        Game1.playSound("axchop");
        List<ClickableTextureComponent> rewards9 = this.rewards;
        ClickableTextureComponent textureComponent9 = new ClickableTextureComponent(Rectangle.Empty, Game1.mouseCursors_1_6, new Rectangle(103, 90, 17, 16 /*0x10*/), 4f, true);
        textureComponent9.name = "";
        textureComponent9.label = Game1.content.LoadString("Strings\\1_6_Strings:Foraging_Mastery");
        textureComponent9.myAlternateID = 0;
        rewards9.Add(textureComponent9);
        break;
      case 3:
        List<ClickableTextureComponent> rewards10 = this.rewards;
        ClickableTextureComponent textureComponent10 = new ClickableTextureComponent(Rectangle.Empty, Game1.bigCraftableSpriteSheet, new Rectangle(64 /*0x40*/, 1152, 16 /*0x10*/, 32 /*0x20*/), 4f, true);
        textureComponent10.name = ItemRegistry.GetDataOrErrorItem("(BC)StatueOfTheDwarfKing").DisplayName;
        textureComponent10.label = ItemRegistry.GetDataOrErrorItem("StatueOfTheDwarfKing").Description;
        textureComponent10.myAlternateID = 1;
        textureComponent10.hoverText = "Statue Of The Dwarf King";
        rewards10.Add(textureComponent10);
        List<ClickableTextureComponent> rewards11 = this.rewards;
        ClickableTextureComponent textureComponent11 = new ClickableTextureComponent(Rectangle.Empty, Game1.bigCraftableSpriteSheet, new Rectangle(0, 1152, 16 /*0x10*/, 32 /*0x20*/), 4f, true);
        textureComponent11.name = ItemRegistry.GetDataOrErrorItem("(BC)HeavyFurnace").DisplayName;
        textureComponent11.label = ItemRegistry.GetDataOrErrorItem("(BC)HeavyFurnace").Description;
        textureComponent11.myAlternateID = 1;
        textureComponent11.hoverText = "Heavy Furnace";
        rewards11.Add(textureComponent11);
        Game1.playSound("stoneCrack");
        List<ClickableTextureComponent> rewards12 = this.rewards;
        ClickableTextureComponent textureComponent12 = new ClickableTextureComponent(Rectangle.Empty, Game1.mouseCursors_1_6, new Rectangle(103, 90, 17, 16 /*0x10*/), 4f, true);
        textureComponent12.name = "";
        textureComponent12.label = Game1.content.LoadString("Strings\\1_6_Strings:Mining_Mastery");
        textureComponent12.myAlternateID = 0;
        rewards12.Add(textureComponent12);
        break;
      case 4:
        Game1.playSound("cavedrip");
        List<ClickableTextureComponent> rewards13 = this.rewards;
        ClickableTextureComponent textureComponent13 = new ClickableTextureComponent(Rectangle.Empty, Game1.bigCraftableSpriteSheet, new Rectangle(80 /*0x50*/, 1152, 16 /*0x10*/, 32 /*0x20*/), 4f, true);
        textureComponent13.name = ItemRegistry.GetDataOrErrorItem("(BC)Anvil").DisplayName;
        textureComponent13.label = ItemRegistry.GetDataOrErrorItem("(BC)Anvil").Description;
        textureComponent13.myAlternateID = 1;
        textureComponent13.hoverText = "Anvil";
        rewards13.Add(textureComponent13);
        List<ClickableTextureComponent> rewards14 = this.rewards;
        ClickableTextureComponent textureComponent14 = new ClickableTextureComponent(Rectangle.Empty, Game1.bigCraftableSpriteSheet, new Rectangle(96 /*0x60*/, 1152, 16 /*0x10*/, 32 /*0x20*/), 4f, true);
        textureComponent14.name = ItemRegistry.GetDataOrErrorItem("(BC)MiniForge").DisplayName;
        textureComponent14.label = ItemRegistry.GetDataOrErrorItem("(BC)MiniForge").Description;
        textureComponent14.myAlternateID = 1;
        textureComponent14.hoverText = "Mini-Forge";
        rewards14.Add(textureComponent14);
        List<ClickableTextureComponent> rewards15 = this.rewards;
        ClickableTextureComponent textureComponent15 = new ClickableTextureComponent(Rectangle.Empty, Game1.mouseCursors_1_6, new Rectangle(103, 90, 17, 16 /*0x10*/), 4f, true);
        textureComponent15.name = "";
        textureComponent15.label = Game1.content.LoadString("Strings\\1_6_Strings:Trinkets_Description");
        textureComponent15.myAlternateID = 0;
        rewards15.Add(textureComponent15);
        break;
    }
    float num1 = 80f;
    for (int index = 0; index < this.rewards.Count; ++index)
    {
      this.rewards[index].bounds = new Rectangle(this.xPositionOnScreen + 40, this.yPositionOnScreen + 64 /*0x40*/ + (int) num1, 64 /*0x40*/, 64 /*0x40*/);
      this.rewards[index].label = Game1.parseText(this.rewards[index].label, Game1.smallFont, this.width - 200);
      num1 += Game1.smallFont.MeasureString(this.rewards[index].label).Y;
      if (index < this.rewards.Count - 1)
        num1 += this.rewards[index].sourceRect.Height > 16 /*0x10*/ ? 132f : 80f;
    }
    this.height += (int) num1;
    this.height -= 48 /*0x30*/;
    if (whichSkill != -1)
      this.height -= 64 /*0x40*/;
    int positionOnScreen1 = this.yPositionOnScreen;
    this.yPositionOnScreen = (int) Utility.getTopLeftPositionForCenteringOnScreen(800, this.height).Y;
    int positionOnScreen2 = this.yPositionOnScreen;
    int num2 = positionOnScreen1 - positionOnScreen2;
    foreach (ClickableComponent reward in this.rewards)
      reward.bounds.Y -= num2;
    this.upperRightCloseButton.bounds.Y -= num2;
    this.canClaim = MasteryTrackerMenu.getCurrentMasteryLevel() - (int) Game1.stats.Get("masteryLevelsSpent") > 0;
    if (Game1.player.stats.Get(StatKeys.Mastery(whichSkill)) <= 0U)
    {
      ClickableTextureComponent textureComponent16 = new ClickableTextureComponent(new Rectangle(this.xPositionOnScreen + this.width / 2 - 84, this.yPositionOnScreen + this.height - 112 /*0x70*/, 168, 80 /*0x50*/), Game1.mouseCursors_1_6, new Rectangle(0, 123, 42, 21), 4f);
      textureComponent16.visible = whichSkill != -1;
      textureComponent16.myID = 0;
      this.mainButton = textureComponent16;
    }
    if (whichSkill == -1)
      Game1.playSound("boulderCrack");
    if (!Game1.options.SnappyMenus)
      return;
    this.populateClickableComponentList();
    if (this.mainButton == null)
      this.currentlySnappedComponent = this.getComponentWithID(this.upperRightCloseButton.myID);
    else
      this.currentlySnappedComponent = this.getComponentWithID(0);
    this.snapCursorToCurrentSnappedComponent();
  }

  /// <inheritdoc />
  public override void performHoverAction(int x, int y)
  {
    if ((double) this.destroyTimer > 0.0)
      return;
    if (this.mainButton != null && this.mainButton.containsPoint(x, y) && (double) this.pressedButtonTimer <= 0.0 && this.canClaim)
    {
      if (this.mainButton.sourceRect.X == 0)
        Game1.playSound("Cowboy_gunshot");
      this.mainButton.sourceRect.X = 42;
    }
    else if (this.mainButton != null)
      this.mainButton.sourceRect.X = 0;
    base.performHoverAction(x, y);
  }

  /// <inheritdoc />
  public override void receiveLeftClick(int x, int y, bool playSound = true)
  {
    if ((double) this.destroyTimer > 0.0)
      return;
    if (this.mainButton != null && this.mainButton.containsPoint(x, y) && (double) this.pressedButtonTimer <= 0.0 && this.canClaim)
    {
      Game1.playSound("cowboy_monsterhit");
      DelayedAction.playSoundAfterDelay("cowboy_monsterhit", 200);
      this.pressedButtonTimer = 200f;
      this.claimReward();
    }
    base.receiveLeftClick(x, y, playSound);
  }

  /// <inheritdoc />
  public override void receiveRightClick(int x, int y, bool playSound = true)
  {
    base.receiveRightClick(x, y, playSound);
    this.exitThisMenu();
  }

  private void claimReward()
  {
    List<Item> objList = new List<Item>();
    foreach (ClickableTextureComponent reward in this.rewards)
    {
      if (reward.myAlternateID == 1)
      {
        Game1.player.craftingRecipes.TryAdd(reward.hoverText, 0);
      }
      else
      {
        string hoverText = reward.hoverText;
        if ((hoverText != null ? (hoverText.Length > 0 ? 1 : 0) : 0) != 0)
        {
          Item obj = ItemRegistry.Create(reward.hoverText);
          if (!Game1.player.addItemToInventoryBool(obj))
            objList.Add(obj);
        }
      }
    }
    foreach (Item obj in objList)
      Game1.createItemDebris(obj, Game1.player.getStandingPosition(), 2);
    int num1 = (int) Game1.player.stats.Increment(StatKeys.Mastery(this.which), 1);
    if (this.which == 4)
      Game1.player.stats.Set("trinketSlots", 1);
    int num2 = (int) Game1.stats.Increment("masteryLevelsSpent");
    Game1.currentLocation.removeTemporarySpritesWithID(8765 + this.which);
    MasteryTrackerMenu.addSkillFlairPlaque(this.which);
    int num3 = (int) Game1.stats.Get("MasteryExp");
    if (MasteryTrackerMenu.getCurrentMasteryLevel() - (int) Game1.stats.Get("masteryLevelsSpent") <= 0)
    {
      Game1.currentLocation.removeTemporarySpritesWithID(8765);
      Game1.currentLocation.removeTemporarySpritesWithID(8766);
      Game1.currentLocation.removeTemporarySpritesWithID(8767);
      Game1.currentLocation.removeTemporarySpritesWithID(8768);
      Game1.currentLocation.removeTemporarySpritesWithID(8769);
    }
    if (!MasteryTrackerMenu.hasCompletedAllMasteryPlaques())
      return;
    DelayedAction.functionAfterDelay((Action) (() => MasteryTrackerMenu.addSpiritCandles()), 500);
    Game1.player.freezePause = 2000;
    DelayedAction.functionAfterDelay((Action) (() => Game1.changeMusicTrack("grandpas_theme")), 2000);
    DelayedAction.functionAfterDelay((Action) (() =>
    {
      Game1.showGlobalMessage(Game1.content.LoadString("Strings\\1_6_Strings:MasteryCompleteToast"));
      Game1.playSound("newArtifact");
    }), 4000);
  }

  public static void addSpiritCandles(bool instant = false)
  {
    MasteryTrackerMenu.addCandle(58, 67, instant ? 0 : 500);
    MasteryTrackerMenu.addCandle(88, 51, instant ? 0 : 700);
    MasteryTrackerMenu.addCandle(120, 51, instant ? 0 : 900);
    MasteryTrackerMenu.addCandle(152, 51, instant ? 0 : 1100);
    MasteryTrackerMenu.addCandle(183, 67, instant ? 0 : 1300);
    Game1.currentLocation.temporarySprites.Add(new TemporaryAnimatedSprite("LooseSprites\\Cursors_1_6", new Rectangle(483, 0, 29, 27), new Vector2(61f, 82f) * 4f, false, 0.0f, Color.White)
    {
      interval = 99999f,
      totalNumberOfLoops = 99999,
      animationLength = 1,
      lightId = "MasteryTrackerMenu_GrandpaHat",
      id = 6666,
      lightRadius = 1f,
      scale = 4f,
      layerDepth = 0.0449f,
      delayBeforeAnimationStart = instant ? 0 : 250
    });
    Game1.currentLocation.removeTile(10, 9, "Buildings");
    if (instant)
      return;
    Utility.addSprinklesToLocation(Game1.currentLocation, 10, 9, 1, 1, 300, 100, Color.White);
    Utility.addSprinklesToLocation(Game1.currentLocation, 4, 6, 1, 2, 300, 50, Color.White);
  }

  private static void addCandle(int x, int y, int delay)
  {
    TemporaryAnimatedSpriteList temporarySprites = Game1.currentLocation.temporarySprites;
    TemporaryAnimatedSprite temporaryAnimatedSprite = new TemporaryAnimatedSprite("LooseSprites\\Cursors", new Rectangle(536, 1945, 8, 8), new Vector2((float) x, (float) y) * 4f + new Vector2(-3f, -6f) * 4f, false, 0.0f, Color.White);
    temporaryAnimatedSprite.interval = 50f + (float) Game1.random.Next(15);
    temporaryAnimatedSprite.totalNumberOfLoops = 99999;
    temporaryAnimatedSprite.animationLength = 7;
    temporaryAnimatedSprite.lightId = $"{nameof (MasteryTrackerMenu)}_SpiritCandle_{x}_{y}";
    temporaryAnimatedSprite.id = 6666;
    temporaryAnimatedSprite.lightRadius = 1f;
    temporaryAnimatedSprite.scale = 3f;
    temporaryAnimatedSprite.layerDepth = 0.0385000035f;
    temporaryAnimatedSprite.delayBeforeAnimationStart = delay;
    temporaryAnimatedSprite.startSound = delay > 0 ? "fireball" : (string) null;
    temporaryAnimatedSprite.drawAboveAlwaysFront = true;
    temporarySprites.Add(temporaryAnimatedSprite);
  }

  public static void addSkillFlairPlaque(int which)
  {
    switch (which)
    {
      case 0:
        Game1.currentLocation.temporarySprites.Add(new TemporaryAnimatedSprite("LooseSprites\\Cursors_1_6", new Rectangle(21, 59, 15, 21), new Vector2(113f, 61f) * 4f, false, 0.0f, Color.White)
        {
          animationLength = 1,
          interval = 9999f,
          totalNumberOfLoops = 999999,
          scale = 4f
        });
        break;
      case 1:
        Game1.currentLocation.temporarySprites.Add(new TemporaryAnimatedSprite("LooseSprites\\Cursors_1_6", new Rectangle(37, 59, 16 /*0x10*/, 21), new Vector2(143f, 63f) * 4f, false, 0.0f, Color.White)
        {
          animationLength = 1,
          interval = 9999f,
          totalNumberOfLoops = 999999,
          scale = 4f
        });
        break;
      case 2:
        Game1.currentLocation.temporarySprites.Add(new TemporaryAnimatedSprite("LooseSprites\\Cursors_1_6", new Rectangle(10, 59, 10, 21), new Vector2(82f, 61f) * 4f, false, 0.0f, Color.White)
        {
          animationLength = 1,
          interval = 9999f,
          totalNumberOfLoops = 999999,
          scale = 4f
        });
        break;
      case 3:
        Game1.currentLocation.temporarySprites.Add(new TemporaryAnimatedSprite("LooseSprites\\Cursors_1_6", new Rectangle(54, 59, 16 /*0x10*/, 21), new Vector2(175f, 75f) * 4f, false, 0.0f, Color.White)
        {
          animationLength = 1,
          interval = 9999f,
          totalNumberOfLoops = 999999,
          scale = 4f
        });
        break;
      case 4:
        Game1.currentLocation.temporarySprites.Add(new TemporaryAnimatedSprite("LooseSprites\\Cursors_1_6", new Rectangle(0, 59, 9, 21), new Vector2(53f, 75f) * 4f, false, 0.0f, Color.White)
        {
          animationLength = 1,
          interval = 9999f,
          totalNumberOfLoops = 999999,
          scale = 4f
        });
        break;
    }
  }

  public static bool hasCompletedAllMasteryPlaques()
  {
    return Game1.player.stats.Get(StatKeys.Mastery(0)) > 0U && Game1.player.stats.Get(StatKeys.Mastery(1)) > 0U && Game1.player.stats.Get(StatKeys.Mastery(2)) > 0U && Game1.player.stats.Get(StatKeys.Mastery(3)) > 0U && Game1.player.stats.Get(StatKeys.Mastery(4)) > 0U;
  }

  /// <inheritdoc />
  public override void update(GameTime time)
  {
    if ((double) this.destroyTimer > 0.0)
    {
      this.destroyTimer -= (float) (int) time.ElapsedGameTime.TotalMilliseconds;
      if ((double) this.destroyTimer <= 0.0)
      {
        Game1.activeClickableMenu = (IClickableMenu) null;
        Game1.playSound("discoverMineral");
      }
    }
    if ((double) this.pressedButtonTimer > 0.0)
    {
      this.pressedButtonTimer -= (float) (int) time.ElapsedGameTime.TotalMilliseconds;
      this.mainButton.sourceRect.X = 84;
      if ((double) this.pressedButtonTimer <= 0.0)
        this.destroyTimer = 100f;
    }
    base.update(time);
  }

  public static int getMasteryExpNeededForLevel(int level)
  {
    switch (level)
    {
      case 0:
        return 0;
      case 1:
        return 10000;
      case 2:
        return 25000;
      case 3:
        return 45000;
      case 4:
        return 70000;
      case 5:
        return 100000;
      default:
        return int.MaxValue;
    }
  }

  public static int getCurrentMasteryLevel()
  {
    int num = (int) Game1.stats.Get("MasteryExp");
    int currentMasteryLevel = 0;
    for (int level = 1; level <= 5; ++level)
    {
      if (num >= MasteryTrackerMenu.getMasteryExpNeededForLevel(level))
        ++currentMasteryLevel;
    }
    return currentMasteryLevel;
  }

  public static void drawBar(SpriteBatch b, Vector2 topLeftSpot, float widthScale = 1f)
  {
    int num1 = (int) Game1.stats.Get("MasteryExp");
    int currentMasteryLevel = MasteryTrackerMenu.getCurrentMasteryLevel();
    int width = (int) (576.0 * (double) (num1 - MasteryTrackerMenu.getMasteryExpNeededForLevel(currentMasteryLevel)) / (double) (MasteryTrackerMenu.getMasteryExpNeededForLevel(currentMasteryLevel + 1) - MasteryTrackerMenu.getMasteryExpNeededForLevel(currentMasteryLevel)) * (double) widthScale);
    if (currentMasteryLevel >= 5)
      width = (int) (576.0 * (double) widthScale);
    if (currentMasteryLevel >= 5 || width > 0)
    {
      Color color1 = new Color(60, 180, 80 /*0x50*/);
      Color color2 = new Color(0, 113, 62);
      Color color3 = new Color(0, 80 /*0x50*/, 50);
      Color color4 = new Color(0, 60, 30);
      if (currentMasteryLevel >= 5 && (double) widthScale == 1.0)
      {
        color1 = new Color(220, 220, 220);
        color2 = new Color(140, 140, 140);
        color3 = new Color(80 /*0x50*/, 80 /*0x50*/, 80 /*0x50*/);
        color4 = color2;
      }
      if ((double) widthScale != 1.0)
        color4 = color3;
      b.Draw(Game1.staminaRect, new Rectangle((int) topLeftSpot.X + 112 /*0x70*/, (int) topLeftSpot.Y + 144 /*0x90*/, width, 32 /*0x20*/), color2);
      b.Draw(Game1.staminaRect, new Rectangle((int) topLeftSpot.X + 112 /*0x70*/, (int) topLeftSpot.Y + 148, 4, 28), color3);
      if (width > 8)
      {
        b.Draw(Game1.staminaRect, new Rectangle((int) topLeftSpot.X + 112 /*0x70*/, (int) topLeftSpot.Y + 172, width - 8, 4), color3);
        b.Draw(Game1.staminaRect, new Rectangle((int) topLeftSpot.X + 116, (int) topLeftSpot.Y + 144 /*0x90*/, width - 4, 4), color1);
        b.Draw(Game1.staminaRect, new Rectangle((int) topLeftSpot.X + 104 + width, (int) topLeftSpot.Y + 144 /*0x90*/, 4, 28), color1);
        b.Draw(Game1.staminaRect, new Rectangle((int) topLeftSpot.X + 108 + width, (int) topLeftSpot.Y + 144 /*0x90*/, 4, 32 /*0x20*/), color4);
      }
    }
    if (currentMasteryLevel >= 5)
      return;
    int num2 = num1 - MasteryTrackerMenu.getMasteryExpNeededForLevel(currentMasteryLevel);
    string str1 = num2.ToString();
    num2 = MasteryTrackerMenu.getMasteryExpNeededForLevel(currentMasteryLevel + 1) - MasteryTrackerMenu.getMasteryExpNeededForLevel(currentMasteryLevel);
    string str2 = num2.ToString();
    string text = $"{str1}/{str2}";
    b.DrawString(Game1.smallFont, text, new Vector2((float) ((double) ((int) topLeftSpot.X + 112 /*0x70*/) + 288.0 * (double) widthScale - (double) Game1.smallFont.MeasureString(text).X / 2.0), (float) (int) topLeftSpot.Y + 146f), Color.White * 0.75f);
  }

  /// <inheritdoc />
  public override void draw(SpriteBatch b)
  {
    if (!Game1.options.showClearBackgrounds)
      b.Draw(Game1.fadeToBlackRect, Game1.graphics.GraphicsDevice.Viewport.Bounds, Color.Black * 0.6f);
    IClickableMenu.drawTextureBox(b, Game1.mouseCursors_1_6, new Rectangle(1, 85, 21, 21), this.xPositionOnScreen, this.yPositionOnScreen, this.width, this.height, Color.White, 4f);
    b.Draw(Game1.mouseCursors_1_6, this.Position + new Vector2(6f, 7f) * 4f, new Rectangle?(new Rectangle(0, 144 /*0x90*/, 23, 23)), Color.White, 0.0f, Vector2.Zero, 4f, SpriteEffects.None, 0.88f);
    b.Draw(Game1.mouseCursors_1_6, this.Position + new Vector2(24f, (float) (this.height - 24)), new Rectangle?(new Rectangle(0, 144 /*0x90*/, 23, 23)), Color.White, -1.57079637f, Vector2.Zero, 4f, SpriteEffects.None, 0.88f);
    b.Draw(Game1.mouseCursors_1_6, this.Position + new Vector2((float) (this.width - 24), 28f), new Rectangle?(new Rectangle(0, 144 /*0x90*/, 23, 23)), Color.White, -4.712389f, Vector2.Zero, 4f, SpriteEffects.None, 0.88f);
    b.Draw(Game1.mouseCursors_1_6, this.Position + new Vector2((float) (this.width - 24), (float) (this.height - 24)), new Rectangle?(new Rectangle(0, 144 /*0x90*/, 23, 23)), Color.White, 3.14159274f, Vector2.Zero, 4f, SpriteEffects.None, 0.88f);
    int num1 = (int) Game1.stats.Get("MasteryExp");
    int currentMasteryLevel = MasteryTrackerMenu.getCurrentMasteryLevel();
    int num2 = currentMasteryLevel - (int) Game1.stats.Get("masteryLevelsSpent");
    if (this.which == -1)
    {
      SpriteText.drawStringHorizontallyCenteredAt(b, Game1.content.LoadString("Strings\\1_6_Strings:FinalPath"), this.xPositionOnScreen + this.width / 2, this.yPositionOnScreen + 48 /*0x30*/, 9999, height: 9999, color: new Color?(Color.Black));
      IClickableMenu.drawTextureBox(b, Game1.mouseCursors_1_6, new Rectangle(0, 107, 15, 15), this.xPositionOnScreen + 100, this.yPositionOnScreen + 128 /*0x80*/, 600, 64 /*0x40*/, Color.White, 4f);
      MasteryTrackerMenu.drawBar(b, new Vector2((float) this.xPositionOnScreen, (float) this.yPositionOnScreen));
      for (int index = 0; index < 5; ++index)
        b.Draw(Game1.mouseCursors_1_6, new Vector2((float) (this.xPositionOnScreen + this.width / 2) - 110f + (float) (index * 11 * 4), (float) (this.yPositionOnScreen + 220)), new Rectangle?(new Rectangle(index < currentMasteryLevel - num2 || index >= currentMasteryLevel ? (currentMasteryLevel > index ? 33 : 23) : 43 + (int) Game1.currentGameTime.TotalGameTime.TotalMilliseconds % 600 / 100 * 10, 89, 10, 11)), Color.White, 0.0f, Vector2.Zero, 4f, SpriteEffects.None, 0.88f);
    }
    else
    {
      SpriteText.drawStringHorizontallyCenteredAt(b, Game1.content.LoadString($"Strings\\1_6_Strings:{this.which.ToString()}_Mastery"), this.xPositionOnScreen + this.width / 2, this.yPositionOnScreen + 48 /*0x30*/, 9999, height: 9999, color: new Color?(Color.Black));
      float y = Game1.smallFont.MeasureString("I").Y;
      foreach (ClickableTextureComponent reward in this.rewards)
      {
        if ((double) Game1.smallFont.MeasureString(reward.label).Y < (double) y * 2.0)
          Utility.drawWithShadow(b, reward.texture, reward.getVector2() + new Vector2(0.0f, -16f), reward.sourceRect, Color.White, 0.0f, Vector2.Zero, 4f);
        else
          Utility.drawWithShadow(b, reward.texture, reward.getVector2(), reward.sourceRect, Color.White, 0.0f, Vector2.Zero, 4f);
        if (reward.name != "")
          Utility.drawTextWithColoredShadow(b, reward.name, Game1.dialogueFont, reward.getVector2() + new Vector2(104f, 0.0f), Color.Black, Color.Black * 0.2f);
        Utility.drawTextWithColoredShadow(b, reward.label, Game1.smallFont, reward.getVector2() + new Vector2(104f, reward.name == "" ? 0.0f : 48f), Color.Black, Color.Black * 0.2f);
        if (reward.myAlternateID == 1)
          b.Draw(Game1.objectSpriteSheet, reward.getVector2() + new Vector2(32f, (float) (32 /*0x20*/ + (reward.sourceRect.Height > 16 /*0x10*/ ? 64 /*0x40*/ : 0))), new Rectangle?(Game1.getSourceRectForStandardTileSheet(Game1.objectSpriteSheet, 451, 16 /*0x10*/, 16 /*0x10*/)), Color.White, 0.0f, Vector2.Zero, 3f, SpriteEffects.None, 0.9f);
      }
      if (this.mainButton != null)
      {
        this.mainButton?.draw(b, num2 > 0 ? Color.White : Color.White * 0.5f, 0.88f);
        string text = Game1.content.LoadString("Strings\\1_6_Strings:Claim");
        Utility.drawTextWithColoredShadow(b, text, Game1.dialogueFont, this.mainButton.getVector2() + new Vector2((float) (this.mainButton.bounds.Width / 2) - Game1.dialogueFont.MeasureString(text).X / 2f, (float) (6.0 + (this.mainButton.sourceRect.X == 84 ? 8.0 : 0.0))), Color.Black * (num2 > 0 ? 1f : 0.5f), Color.Black * 0.2f, layerDepth: 0.9f);
      }
    }
    base.draw(b);
    this.drawMouse(b);
  }
}
