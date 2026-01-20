// Decompiled with JetBrains decompiler
// Type: StardewValley.Locations.DesertFestival
// Assembly: Stardew Valley, Version=1.6.15.24356, Culture=neutral, PublicKeyToken=null
// MVID: AA1F513A-94F0-4EF6-A35F-2D5B4A3721BF
// Assembly location: D:\SteamLibrary\steamapps\common\Stardew Valley\Stardew Valley.dll
// XML documentation location: D:\SteamLibrary\steamapps\common\Stardew Valley\Stardew Valley.xml

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Netcode;
using StardewValley.BellsAndWhistles;
using StardewValley.Buffs;
using StardewValley.Extensions;
using StardewValley.GameData.MakeoverOutfits;
using StardewValley.GameData.Shops;
using StardewValley.ItemTypeDefinitions;
using StardewValley.Menus;
using StardewValley.Network;
using StardewValley.Objects;
using StardewValley.Pathfinding;
using StardewValley.Quests;
using StardewValley.SpecialOrders;
using StardewValley.TokenizableStrings;
using System;
using System.Collections.Generic;
using System.Text;
using System.Xml.Serialization;
using xTile.Dimensions;

#nullable disable
namespace StardewValley.Locations;

public class DesertFestival : Desert
{
  public const int CALICO_STATUE_GHOST_INVASION = 0;
  public const int CALICO_STATUE_SERPENT_INVASION = 1;
  public const int CALICO_STATUE_SKELETON_INVASION = 2;
  public const int CALICO_STATUE_BAT_INVASION = 3;
  public const int CALICO_STATUE_ASSASSIN_BUGS = 4;
  public const int CALICO_STATUE_THIN_SHELLS = 5;
  public const int CALICO_STATUE_MEAGER_MEALS = 6;
  public const int CALICO_STATUE_MONSTER_SURGE = 7;
  public const int CALICO_STATUE_SHARP_TEETH = 8;
  public const int CALICO_STATUE_MUMMY_CURSE = 9;
  public const int CALICO_STATUE_SPEED_BOOST = 10;
  public const int CALICO_STATUE_REFRESH = 11;
  public const int CALICO_STATUE_50_EGG_TREASURE = 12;
  public const int CALICO_STATUE_NO_EFFECT = 13;
  public const int CALICO_STATUE_TOOTH_FILE = 14;
  public const int CALICO_STATUE_25_EGG_TREASURE = 15;
  public const int CALICO_STATUE_10_EGG_TREASURE = 16 /*0x10*/;
  public const int CALICO_STATUE_100_EGG_TREASURE = 17;
  public static readonly int[] CalicoStatueInvasionIds = new int[4]
  {
    3,
    0,
    1,
    2
  };
  public const int NUM_SCHOLAR_QUESTIONS = 4;
  public const string FISHING_QUEST_ID = "98765";
  protected RandomizedPlantFurniture _cactusGuyRevealItem;
  protected float _cactusGuyRevealTimer = -1f;
  protected float _cactusShakeTimer = -1f;
  protected int _currentlyShownCactusID;
  protected NetEvent1Field<int, NetInt> _revealCactusEvent = new NetEvent1Field<int, NetInt>();
  protected NetEvent1Field<int, NetInt> _hideCactusEvent = new NetEvent1Field<int, NetInt>();
  protected MoneyDial eggMoneyDial;
  [XmlIgnore]
  public NetList<Racer, NetRef<Racer>> netRacers = new NetList<Racer, NetRef<Racer>>();
  [XmlIgnore]
  protected List<Racer> _localRacers = new List<Racer>();
  [XmlIgnore]
  protected float festivalChimneyTimer;
  [XmlIgnore]
  public List<int> finishedRacers = new List<int>();
  [XmlIgnore]
  public int racerCount = 3;
  [XmlIgnore]
  public int totalRacers = 5;
  [XmlIgnore]
  public NetEvent1Field<string, NetString> announceRaceEvent = new NetEvent1Field<string, NetString>();
  [XmlIgnore]
  public NetEnum<DesertFestival.RaceState> currentRaceState = new NetEnum<DesertFestival.RaceState>(DesertFestival.RaceState.PreRace);
  [XmlIgnore]
  public NetLongDictionary<int, NetInt> sabotages = new NetLongDictionary<int, NetInt>();
  [XmlIgnore]
  public NetLongDictionary<int, NetInt> raceGuesses = new NetLongDictionary<int, NetInt>();
  [XmlIgnore]
  public NetLongDictionary<int, NetInt> nextRaceGuesses = new NetLongDictionary<int, NetInt>();
  [XmlIgnore]
  public NetLongDictionary<bool, NetBool> specialRewardsCollected = new NetLongDictionary<bool, NetBool>();
  [XmlIgnore]
  public NetLongDictionary<int, NetInt> rewardsToCollect = new NetLongDictionary<int, NetInt>();
  [XmlIgnore]
  public NetInt lastRaceWinner = new NetInt();
  [XmlIgnore]
  protected float _raceStateTimer;
  protected string _raceText;
  protected float _raceTextTimer;
  protected bool _raceTextShake;
  protected int _localSabotageText = -1;
  protected int _currentScholarQuestion = -1;
  protected int _cookIngredient = -1;
  protected int _cookSauce = -1;
  public Vector3[][] raceTrack = new Vector3[16 /*0x10*/][]
  {
    new Vector3[2]
    {
      new Vector3(41f, 39f, 0.0f),
      new Vector3(42f, 39f, 0.0f)
    },
    new Vector3[2]
    {
      new Vector3(41f, 29f, 0.0f),
      new Vector3(42f, 28f, 0.0f)
    },
    new Vector3[2]
    {
      new Vector3(6f, 29f, 0.0f),
      new Vector3(5f, 28f, 0.0f)
    },
    new Vector3[2]
    {
      new Vector3(6f, 35f, 0.0f),
      new Vector3(5f, 36f, 0.0f)
    },
    new Vector3[2]
    {
      new Vector3(10f, 35f, 2f),
      new Vector3(10f, 36f, 2f)
    },
    new Vector3[2]
    {
      new Vector3(12.5f, 35f, 0.0f),
      new Vector3(12.5f, 36f, 0.0f)
    },
    new Vector3[2]
    {
      new Vector3(17.5f, 35f, 1f),
      new Vector3(17.5f, 36f, 1f)
    },
    new Vector3[2]
    {
      new Vector3(23.5f, 35f, 0.0f),
      new Vector3(23.5f, 36f, 0.0f)
    },
    new Vector3[2]
    {
      new Vector3(28.5f, 35f, 1f),
      new Vector3(28.5f, 36f, 1f)
    },
    new Vector3[2]
    {
      new Vector3(31f, 35f, 0.0f),
      new Vector3(31f, 36f, 0.0f)
    },
    new Vector3[2]
    {
      new Vector3(32f, 35f, 0.0f),
      new Vector3(31f, 36f, 0.0f)
    },
    new Vector3[2]
    {
      new Vector3(32f, 38f, 3f),
      new Vector3(31f, 38f, 3f)
    },
    new Vector3[2]
    {
      new Vector3(32f, 43f, 0.0f),
      new Vector3(31f, 43f, 0.0f)
    },
    new Vector3[2]
    {
      new Vector3(32f, 46f, 0.0f),
      new Vector3(31f, 47f, 0.0f)
    },
    new Vector3[2]
    {
      new Vector3(41f, 46f, 0.0f),
      new Vector3(42f, 47f, 0.0f)
    },
    new Vector3[2]
    {
      new Vector3(41f, 39f, 0.0f),
      new Vector3(42f, 39f, 0.0f)
    }
  };
  private bool checkedMineExplanation;

  public DesertFestival() => this.forceLoadPathLayerLights = true;

  public DesertFestival(string mapPath, string name)
    : base(mapPath, name)
  {
    this.forceLoadPathLayerLights = true;
  }

  protected override void initNetFields()
  {
    base.initNetFields();
    this.NetFields.AddField((INetSerializable) this._revealCactusEvent, "_revealCactusEvent").AddField((INetSerializable) this._hideCactusEvent, "_hideCactusEvent").AddField((INetSerializable) this.netRacers, "netRacers").AddField((INetSerializable) this.announceRaceEvent, "announceRaceEvent").AddField((INetSerializable) this.sabotages, "sabotages").AddField((INetSerializable) this.raceGuesses, "raceGuesses").AddField((INetSerializable) this.rewardsToCollect, "rewardsToCollect").AddField((INetSerializable) this.specialRewardsCollected, "specialRewardsCollected").AddField((INetSerializable) this.nextRaceGuesses, "nextRaceGuesses").AddField((INetSerializable) this.lastRaceWinner, "lastRaceWinner").AddField((INetSerializable) this.currentRaceState, "currentRaceState");
    this._revealCactusEvent.onEvent += new AbstractNetEvent1<int>.Event(this.CactusGuyRevealCactus);
    this._hideCactusEvent.onEvent += new AbstractNetEvent1<int>.Event(this.CactusGuyHideCactus);
    this.announceRaceEvent.onEvent += new AbstractNetEvent1<string>.Event(this.AnnounceRace);
  }

  public static void SetupMerchantSchedule(NPC character, int shop_index)
  {
    StringBuilder stringBuilder = new StringBuilder();
    if (shop_index == 0)
      stringBuilder.Append("/a1130 Desert 15 40 2");
    else
      stringBuilder.Append("/a1140 Desert 26 40 2");
    stringBuilder.Append("/2400 bed");
    stringBuilder.Remove(0, 1);
    GameLocation locationFromName = Game1.getLocationFromName(character.DefaultMap);
    if (locationFromName != null)
      Game1.warpCharacter(character, locationFromName, new Vector2((float) (int) ((double) character.DefaultPosition.X / 64.0), (float) (int) ((double) character.DefaultPosition.Y / 64.0)));
    character.islandScheduleName.Value = "festival_vendor";
    character.TryLoadSchedule("desertFestival", stringBuilder.ToString());
    character.performSpecialScheduleChanges();
  }

  public override void OnCamel()
  {
    Game1.playSound("camel");
    this.ShowCamelAnimation();
    Game1.player.faceDirection(0);
    Game1.haltAfterCheck = false;
  }

  public override void ShowCamelAnimation()
  {
    this.temporarySprites.Add(new TemporaryAnimatedSprite()
    {
      texture = Game1.temporaryContent.Load<Texture2D>("LooseSprites\\temporary_sprites_1"),
      sourceRect = new Microsoft.Xna.Framework.Rectangle(273, 524, 65, 49),
      sourceRectStartingPos = new Vector2(273f, 524f),
      animationLength = 1,
      totalNumberOfLoops = 1,
      interval = 300f,
      scale = 4f,
      position = new Vector2(536f, 340f) * 4f,
      layerDepth = 0.1332f,
      id = 999
    });
  }

  public override void checkForMusic(GameTime time)
  {
    Game1.changeMusicTrack(this.GetFestivalMusic(), true);
  }

  public virtual string GetFestivalMusic()
  {
    return Utility.IsPassiveFestivalOpen(nameof (DesertFestival)) ? "event2" : "summer_day_ambient";
  }

  public override string GetLocationSpecificMusic() => this.GetFestivalMusic();

  public override void digUpArtifactSpot(int xLocation, int yLocation, Farmer who)
  {
    Random daySaveRandom = Utility.CreateDaySaveRandom((double) (xLocation * 2000), (double) yLocation);
    Game1.createMultipleObjectDebris("CalicoEgg", xLocation, yLocation, daySaveRandom.Next(3, 7), who.UniqueMultiplayerID, (GameLocation) this);
    base.digUpArtifactSpot(xLocation, yLocation, who);
  }

  public virtual void CollectRacePrizes()
  {
    List<Item> inventory = new List<Item>();
    bool flag;
    if (this.specialRewardsCollected.TryGetValue(Game1.player.UniqueMultiplayerID, out flag) && !flag)
    {
      this.specialRewardsCollected[Game1.player.UniqueMultiplayerID] = true;
      inventory.Add(ItemRegistry.Create("CalicoEgg", 100));
    }
    for (int index = 0; index < this.rewardsToCollect[Game1.player.UniqueMultiplayerID]; ++index)
      inventory.Add(ItemRegistry.Create("CalicoEgg", 20));
    this.rewardsToCollect[Game1.player.UniqueMultiplayerID] = 0;
    Game1.activeClickableMenu = (IClickableMenu) new ItemGrabMenu((IList<Item>) inventory, false, true, (InventoryMenu.highlightThisItem) null, (ItemGrabMenu.behaviorOnItemSelect) null, "Rewards", canBeExitedWithKey: true, playRightClickSound: false, allowRightClick: false, context: (object) this);
  }

  public override void performTouchAction(
    string full_action_string,
    Vector2 player_standing_position)
  {
    if (Game1.eventUp)
      return;
    if (full_action_string.Split(' ')[0] == "DesertMakeover")
    {
      if (Game1.player.controller != null)
        return;
      bool flag = false;
      string failMessageKey = (string) null;
      NPC stylist = this.GetStylist();
      if (!flag && stylist == null)
      {
        stylist = (NPC) null;
        failMessageKey = "Strings\\1_6_Strings:MakeOver_NoStylist";
        flag = true;
      }
      if (!flag && Game1.player.activeDialogueEvents.ContainsKey("DesertMakeover"))
      {
        failMessageKey = $"Strings\\1_6_Strings:MakeOver_{stylist.Name}_AlreadyStyled";
        flag = true;
      }
      int num = 0;
      if (Game1.player.hat.Value != null)
        ++num;
      if (Game1.player.shirtItem.Value != null)
        ++num;
      if (Game1.player.pantsItem.Value != null)
        ++num;
      if (!flag && Game1.player.freeSpotsInInventory() < num)
      {
        failMessageKey = $"Strings\\1_6_Strings:MakeOver_{stylist.Name}_InventoryFull";
        flag = true;
      }
      if (flag)
      {
        Game1.freezeControls = true;
        Game1.displayHUD = false;
        int finalFacingDirection = 2;
        if (stylist != null)
          finalFacingDirection = 3;
        Game1.player.controller = new PathFindController((Character) Game1.player, (GameLocation) this, new Point(26, 52), finalFacingDirection, (PathFindController.endBehavior) ((character, location) =>
        {
          Game1.freezeControls = false;
          Game1.displayHUD = true;
          if (stylist != null)
          {
            stylist.faceTowardFarmerForPeriod(1000, 2, false, Game1.player);
            if (failMessageKey == null)
              return;
            Game1.DrawDialogue(stylist, failMessageKey);
          }
          else
          {
            if (failMessageKey == null)
              return;
            Game1.drawObjectDialogue(Game1.content.LoadString(failMessageKey));
          }
        }));
      }
      else
      {
        Game1.player.activeDialogueEvents["DesertMakeover"] = 0;
        Game1.freezeControls = true;
        Game1.displayHUD = false;
        Game1.player.controller = new PathFindController((Character) Game1.player, (GameLocation) this, new Point(27, 50), 0);
        Game1.globalFadeToBlack((Game1.afterFadeFunction) (() =>
        {
          Game1.freezeControls = false;
          Game1.forceSnapOnNextViewportUpdate = true;
          StardewValley.Event evt = new StardewValley.Event(this.GetMakeoverEvent());
          evt.onEventFinished += new Action(this.ReceiveMakeOver);
          this.startEvent(evt);
          Game1.globalFadeToClear();
        }));
      }
    }
    else
      base.performTouchAction(full_action_string, player_standing_position);
  }

  public virtual string GetMakeoverEvent()
  {
    NPC stylist = this.GetStylist();
    Random daySaveRandom = Utility.CreateDaySaveRandom((double) Game1.year);
    StringBuilder stringBuilder = new StringBuilder();
    stringBuilder.Append("continue/26 51/farmer 27 50 2 ");
    foreach (NPC character in this.characters)
    {
      if (!(character.Name == stylist.Name) && !(character.Name == "Sandy"))
        stringBuilder.Append($"{character.Name} {character.Tile.X.ToString()} {character.Tile.Y.ToString()} {character.FacingDirection.ToString()} ");
    }
    if (stylist.Name == "Emily")
    {
      stringBuilder.Append("Emily 25 52 2 Sandy 22 52 2/skippable/pause 1200/speak Emily \"");
      stringBuilder.Append(Game1.content.LoadString("Strings\\1_6_Strings:MakeOver_Emily_1"));
      stringBuilder.Append("\"/pause 100/");
      switch (daySaveRandom.Next(0, 3))
      {
        case 0:
          stringBuilder.Append("animate Emily false true 200 39 39/");
          break;
        case 1:
          stringBuilder.Append("animate Emily false true 300 16 17 18 19 20 21 22 23/");
          break;
        case 2:
          stringBuilder.Append("animate Emily false true 300 31 48 49/");
          break;
      }
      stringBuilder.Append("pause 1000/faceDirection Sandy 1 true/pause 2000/textAboveHead Emily \"");
      stringBuilder.Append(Game1.content.LoadString("Strings\\1_6_Strings:MakeOver_Emily_2"));
      stringBuilder.Append("\"/pause 3000/stopAnimation Emily 2/playSound dwop/shake Emily 100/jump Emily 4/pause 300/speak Emily \"");
      stringBuilder.Append(Game1.content.LoadString("Strings\\1_6_Strings:MakeOver_Emily_3"));
      stringBuilder.Append("\"/pause 100/advancedMove Emily false 1 0 0 -1 0 -1 0 -1 1 100/pause 100/");
      stringBuilder.Append("advancedMove Sandy false 1 0 1 0 1 0 1 0 2 100/pause 3000/playSound openChest/pause 1000/");
      List<string> list = new List<string>()
      {
        $"playSound dustMeep/pause 300/playSound dustMeep/pause 300/playSound dustMeep/textAboveHead Emily \"{Game1.content.LoadString("Strings\\1_6_Strings:MakeOver_Emily_Reaction1")}\"/",
        $"playSound rooster/playSound dwop/shake Sandy 400/jump Sandy 4/pause 500/textAboveHead Emily \"{Game1.content.LoadString("Strings\\1_6_Strings:MakeOver_Emily_Reaction2")}\"/",
        $"playSound slimeHit/pause 300/playSound slimeHit/pause 600/playSound slimedead/textAboveHead Emily \"{Game1.content.LoadString("Strings\\1_6_Strings:MakeOver_Emily_Reaction3")}\"/",
        $"textAboveHead Emily \"{Game1.content.LoadString("Strings\\1_6_Strings:MakeOver_Emily_Reaction4")}\"/playSound trashcanlid/pause 1000/playSound trashcan/",
        $"textAboveHead Emily \"{Game1.content.LoadString("Strings\\1_6_Strings:MakeOver_Emily_Reaction5")}\"/pause 1000/playSound cast/pause 500/playSound axe/pause 200/playSound ow/",
        $"textAboveHead Emily \"{Game1.content.LoadString("Strings\\1_6_Strings:MakeOver_Emily_Reaction6")}\"/pause 1000/playSound eat/",
        $"textAboveHead Emily \"{Game1.content.LoadString("Strings\\1_6_Strings:MakeOver_Emily_Reaction7")}\"/playSound scissors/pause 300/playSound scissors/pause 300/playSound scissors/",
        $"textAboveHead Emily \"{Game1.content.LoadString("Strings\\1_6_Strings:MakeOver_Emily_Reaction8")}\"/pause 500/playSound trashbear/pause 300/playSound trashbear/pause 300/playSound trashbear/",
        $"textAboveHead Emily \"{Game1.content.LoadString("Strings\\1_6_Strings:MakeOver_Emily_Reaction9")}\"/pause 1000/playSound fishingRodBend/pause 500/playSound fishingRodBend/pause 1000/playSound fishingRodBend/"
      };
      Utility.Shuffle<string>(daySaveRandom, list);
      for (int index = 0; index < 3; ++index)
      {
        stringBuilder.Append("pause 500/");
        stringBuilder.Append(list[index]);
        stringBuilder.Append("pause 1500/");
      }
      stringBuilder.Append("pause 500/playSound money/textAboveHead Emily \"");
      stringBuilder.Append(Game1.content.LoadString("Strings\\1_6_Strings:MakeOver_Emily_4"));
      stringBuilder.Append("\"/playSound dwop/shake Sandy 400/jump Sandy 4/pause 750/advancedMove Sandy false -1 0 -1 0 -1 0 -1 0 1 100/pause 2000/advancedMove Emily false 0 1 0 1 0 1 2 100/pause 2000/speak Emily \"");
      stringBuilder.Append(Game1.content.LoadString("Strings\\1_6_Strings:MakeOver_Emily_5"));
    }
    else
    {
      stringBuilder.Append("Sandy 22 52 2/skippable/pause 2000/textAboveHead Sandy \"");
      stringBuilder.Append(Game1.content.LoadString("Strings\\1_6_Strings:MakeOver_Sandy_1"));
      stringBuilder.Append("\"/");
      stringBuilder.Append("pause 1000/playSound dwop/shake Sandy 400/jump Sandy 4/textAboveHead Sandy \"");
      stringBuilder.Append(Game1.content.LoadString("Strings\\1_6_Strings:MakeOver_Sandy_2"));
      stringBuilder.Append("\"/");
      stringBuilder.Append("pause 200/advancedMove Sandy false 1 0 1 0 1 0 1 0 4 100/");
      stringBuilder.Append("pause 2500/speak Sandy \"");
      stringBuilder.Append(Game1.content.LoadString("Strings\\1_6_Strings:MakeOver_Sandy_3"));
      stringBuilder.Append("\"/");
      stringBuilder.Append("pause 500/advancedMove Sandy false 0 -1 0 -1 0 -1/pause 3000/playSound openChest/pause 1000/");
      stringBuilder.Append($"textAboveHead Sandy \"{Game1.content.LoadString("Strings\\1_6_Strings:MakeOver_Sandy_4")}\"/pause 1000/playSound fishingRodBend/pause 500/playSound fishingRodBend/pause 1000/playSound fishingRodBend/");
      stringBuilder.Append("pause 1500/");
      stringBuilder.Append("pause 500/playSound money/textAboveHead Sandy \"");
      stringBuilder.Append(Game1.content.LoadString("Strings\\1_6_Strings:MakeOver_Sandy_5"));
      stringBuilder.Append("\"/pause 200/advancedMove Sandy false 0 1 0 1 0 1 2 100/pause 2000/speak Sandy \"");
      stringBuilder.Append(Game1.content.LoadString("Strings\\1_6_Strings:MakeOver_Sandy_6"));
    }
    stringBuilder.Append("\"/pause 500/end");
    return stringBuilder.ToString();
  }

  private void ReceiveMakeOver() => this.ReceiveMakeOver(-1);

  public virtual void ReceiveMakeOver(int randomSeedOverride = -1)
  {
    Random random = randomSeedOverride == -1 ? Utility.CreateDaySaveRandom((double) Game1.year) : Utility.CreateRandom((double) randomSeedOverride);
    if (randomSeedOverride == -1 && random.NextDouble() < 0.75)
      random = Utility.CreateDaySaveRandom((double) Game1.year, (double) (int) Game1.player.uniqueMultiplayerID.Value);
    List<MakeoverOutfit> collection = DataLoader.MakeoverOutfits(Game1.content);
    if (collection == null)
      return;
    List<MakeoverOutfit> options = new List<MakeoverOutfit>((IEnumerable<MakeoverOutfit>) collection);
    for (int index = 0; index < options.Count; ++index)
    {
      MakeoverOutfit makeoverOutfit = options[index];
      if (makeoverOutfit.Gender.HasValue && makeoverOutfit.Gender.Value != Game1.player.Gender)
      {
        options.RemoveAt(index);
        --index;
      }
      else
      {
        bool flag = false;
        foreach (MakeoverItem outfitPart in makeoverOutfit.OutfitParts)
        {
          if (outfitPart.MatchesGender(Game1.player.Gender))
          {
            ParsedItemData dataOrErrorItem = ItemRegistry.GetDataOrErrorItem(outfitPart.ItemId);
            flag = Game1.player.hat.Value?.QualifiedItemId == dataOrErrorItem.QualifiedItemId || Game1.player.shirtItem.Value?.QualifiedItemId == dataOrErrorItem.QualifiedItemId;
            if (flag)
              break;
          }
        }
        if (flag)
        {
          options.RemoveAt(index);
          --index;
        }
      }
    }
    Farmer player = Game1.player;
    foreach (Item heldItem in new List<Item>()
    {
      (Item) player.Equip<Clothing>((Clothing) null, player.shirtItem),
      (Item) player.Equip<Clothing>((Clothing) null, player.pantsItem),
      (Item) player.Equip<Hat>((Hat) null, player.hat)
    })
    {
      Item obj = Utility.PerformSpecialItemGrabReplacement(heldItem);
      if (obj != null && player.addItemToInventory(obj) != null)
      {
        player.team.returnedDonations.Add(obj);
        player.team.newLostAndFoundItems.Value = true;
      }
    }
    MakeoverOutfit makeoverOutfit1 = random.ChooseFrom<MakeoverOutfit>((IList<MakeoverOutfit>) options);
    Random daySaveRandom = Utility.CreateDaySaveRandom();
    if (Utility.GetDayOfPassiveFestival(nameof (DesertFestival)) == 2 && daySaveRandom.NextDouble() < 0.03)
      makeoverOutfit1 = new MakeoverOutfit()
      {
        OutfitParts = new List<MakeoverItem>()
        {
          new MakeoverItem() { ItemId = "(H)LaurelWreathCrown" },
          new MakeoverItem()
          {
            ItemId = "(P)3",
            Color = "247 245 205"
          },
          new MakeoverItem() { ItemId = "(S)1199" }
        }
      };
    if (makeoverOutfit1?.OutfitParts == null)
      return;
    bool flag1 = false;
    bool flag2 = false;
    bool flag3 = false;
    foreach (MakeoverItem outfitPart in makeoverOutfit1.OutfitParts)
    {
      if (outfitPart.MatchesGender(Game1.player.Gender))
      {
        Item obj = ItemRegistry.Create(outfitPart.ItemId);
        if (!(obj is Hat newItem2))
        {
          if (obj is Clothing newItem1)
          {
            Color? color = Utility.StringToColor(outfitPart.Color);
            if (color.HasValue)
              newItem1.clothesColor.Value = color.Value;
            switch (newItem1.clothesType.Value)
            {
              case Clothing.ClothesType.SHIRT:
                if (!flag2)
                {
                  player.Equip<Clothing>(newItem1, player.shirtItem);
                  flag2 = true;
                  continue;
                }
                continue;
              case Clothing.ClothesType.PANTS:
                if (!flag3)
                {
                  player.Equip<Clothing>(newItem1, player.pantsItem);
                  flag3 = true;
                  continue;
                }
                continue;
              default:
                continue;
            }
          }
        }
        else if (!flag1)
        {
          player.Equip<Hat>(newItem2, player.hat);
          flag1 = true;
        }
      }
    }
  }

  public virtual void AfterMakeOver()
  {
    Game1.player.canOnlyWalk = false;
    Game1.freezeControls = false;
    Game1.displayHUD = true;
    NPC stylist = this.GetStylist();
    if (stylist == null)
      return;
    Game1.DrawDialogue(stylist, $"Strings\\1_6_Strings:MakeOver_{stylist.Name}_Done");
    stylist.faceTowardFarmerForPeriod(1000, 2, false, Game1.player);
  }

  public NPC GetStylist()
  {
    NPC characterFromName1 = this.getCharacterFromName("Emily");
    if (characterFromName1 != null && characterFromName1.TilePoint == new Point(25, 52))
      return characterFromName1;
    NPC characterFromName2 = this.getCharacterFromName("Sandy");
    if (characterFromName2 != null && characterFromName2.TilePoint == new Point(22, 52))
    {
      NPC characterFromName3 = this.getCharacterFromName("Emily");
      if (characterFromName3 != null && characterFromName3.islandScheduleName.Value == "festival_vendor")
        return characterFromName2;
    }
    return (NPC) null;
  }

  public static void addCalicoStatueSpeedBuff()
  {
    BuffEffects effects = new BuffEffects();
    effects.Speed.Value = 1f;
    Game1.player.applyBuff(new Buff("CalicoStatueSpeed", "Calico Statue", Game1.content.LoadString("Strings\\1_6_Strings:DF_Mine_CalicoStatue"), 300000, Game1.buffsIcons, 9, effects, new bool?(false), Game1.content.LoadString("Strings\\1_6_Strings:DF_Mine_CalicoStatue_Name_10")));
  }

  public override bool performAction(string action, Farmer who, Location tile_location)
  {
    string festivalId = nameof (DesertFestival);
    DataLoader.Shops(Game1.content);
    if (action != null)
    {
      switch (action.Length)
      {
        case 9:
          if (action == "DesertGil")
          {
            if (Game1.Date == who.lastGotPrizeFromGil.Value)
            {
              if (Utility.GetDayOfPassiveFestival(nameof (DesertFestival)) == 3)
                Game1.DrawDialogue(Game1.RequireLocation<AdventureGuild>("AdventureGuild").Gil, "Strings\\1_6_Strings:Gil_NextYear");
              else
                Game1.DrawDialogue(Game1.RequireLocation<AdventureGuild>("AdventureGuild").Gil, "Strings\\1_6_Strings:Gil_ComeBack");
            }
            else if (Game1.player.team.highestCalicoEggRatingToday.Value == 0)
              Game1.DrawDialogue(Game1.RequireLocation<AdventureGuild>("AdventureGuild").Gil, "Strings\\1_6_Strings:Gil_NoRating");
            else
              this.createQuestionDialogue(Game1.content.LoadString("Strings\\1_6_Strings:Gil_SubmitRating", (object) (Game1.player.team.highestCalicoEggRatingToday.Value + 1)), this.createYesNoResponses(), "Gil_EggRating");
            return true;
          }
          break;
        case 10:
          if (action == "DesertFood")
          {
            Game1.player.faceDirection(0);
            this.createQuestionDialogue(Game1.content.LoadString("Strings\\1_6_Strings:Cook_Intro"), this.createYesNoResponses(), "Cook_Intro");
            break;
          }
          break;
        case 12:
          switch (action[6])
          {
            case 'M':
              if (action == "DesertMarlon")
              {
                if (!Game1.player.mailReceived.Contains("Desert_Festival_Marlon"))
                {
                  Game1.player.mailReceived.Add("Desert_Festival_Marlon");
                  Game1.DrawDialogue(Game1.getCharacterFromName("Marlon"), "Strings\\1_6_Strings:Marlon_Intro");
                  break;
                }
                bool flag1 = false;
                bool flag2 = false;
                if (Game1.player.team.acceptedSpecialOrderTypes.Contains("DesertFestivalMarlon"))
                {
                  flag2 = true;
                  foreach (SpecialOrder specialOrder in Game1.player.team.specialOrders)
                  {
                    if (specialOrder.orderType.Value == "DesertFestivalMarlon")
                    {
                      flag1 = true;
                      if (specialOrder.questState.Value != SpecialOrderStatus.InProgress)
                      {
                        if (specialOrder.questState.Value != SpecialOrderStatus.Failed)
                          break;
                      }
                      flag2 = false;
                      break;
                    }
                  }
                }
                if (flag2)
                {
                  if (Utility.GetDayOfPassiveFestival(nameof (DesertFestival)) < 3)
                  {
                    Game1.DrawDialogue(Game1.getCharacterFromName("Marlon"), "Strings\\1_6_Strings:Marlon_Challenge_Finished");
                    return true;
                  }
                  Game1.DrawDialogue(Game1.getCharacterFromName("Marlon"), "Strings\\1_6_Strings:Marlon_Challenge_Finished_LastDay");
                  return true;
                }
                if (flag1)
                  Game1.DrawDialogue(Game1.getCharacterFromName("Marlon"), "Strings\\1_6_Strings:Marlon_Challenge_Chosen");
                else
                  Game1.DrawDialogue(Game1.getCharacterFromName("Marlon"), "Strings\\1_6_Strings:Marlon_" + Game1.random.Next(1, 5).ToString());
                Game1.afterDialogues += (Game1.afterFadeFunction) (() => Game1.activeClickableMenu = (IClickableMenu) new SpecialOrdersBoard("DesertFestivalMarlon"));
                return true;
              }
              break;
            case 'V':
              if (action == "DesertVendor")
              {
                Game1.player.faceDirection(0);
                if (!Utility.IsPassiveFestivalOpen(festivalId))
                  return false;
                Microsoft.Xna.Framework.Rectangle rectangle = new Microsoft.Xna.Framework.Rectangle(tile_location.X, tile_location.Y - 1, 1, 1);
                using (List<NPC>.Enumerator enumerator = this.characters.GetEnumerator())
                {
                  while (enumerator.MoveNext())
                  {
                    NPC current = enumerator.Current;
                    if (rectangle.Contains(current.TilePoint) && Utility.TryOpenShopMenu($"{festivalId}_{current.Name}", current.Name))
                      return true;
                  }
                  break;
                }
              }
              break;
          }
          break;
        case 13:
          switch (action[6])
          {
            case 'E':
              if (action == "DesertEggShop")
              {
                if (!Utility.IsPassiveFestivalOpen(festivalId))
                {
                  Game1.drawObjectDialogue(Game1.content.LoadString("Strings\\1_6_Strings:EggShop_Closed"));
                  break;
                }
                Utility.TryOpenShopMenu("DesertFestival_EggShop", "Vendor");
                break;
              }
              break;
            case 'S':
              if (action == "DesertScholar")
              {
                if (!Utility.IsPassiveFestivalOpen(festivalId))
                {
                  Game1.drawObjectDialogue(Game1.content.LoadString("Strings\\1_6_Strings:Scholar_Closed"));
                  return true;
                }
                if (Game1.player.mailReceived.Contains(this.GetScholarMail()))
                {
                  Game1.drawObjectDialogue(Game1.content.LoadString("Strings\\1_6_Strings:Scholar_DoneThisYear"));
                  return true;
                }
                if (this._currentScholarQuestion == -2)
                {
                  Game1.drawObjectDialogue(Game1.content.LoadString("Strings\\1_6_Strings:Scholar_Failed"));
                  return true;
                }
                this.createQuestionDialogue(Game1.content.LoadString("Strings\\1_6_Strings:Scholar_Intro"), this.createYesNoResponses(), "DesertScholar");
                break;
              }
              break;
          }
          break;
        case 14:
          switch (action[6])
          {
            case 'R':
              if (action == "DesertRacerMan")
              {
                Game1.player.faceGeneralDirection(new Vector2((float) tile_location.X + 0.5f, (float) tile_location.Y + 0.5f) * 64f);
                bool flag;
                if (this.specialRewardsCollected.TryGetValue(Game1.player.UniqueMultiplayerID, out flag) && !flag)
                {
                  Game1.drawObjectDialogue(Game1.content.LoadString("Strings\\1_6_Strings:Race_Collect_Prize_Special"));
                  Game1.afterDialogues += new Game1.afterFadeFunction(this.CollectRacePrizes);
                }
                else
                {
                  int num1;
                  if (this.rewardsToCollect.TryGetValue(Game1.player.UniqueMultiplayerID, out num1) && num1 > 0)
                  {
                    Game1.drawObjectDialogue(Game1.content.LoadString("Strings\\1_6_Strings:Race_Collect_Prize"));
                    Game1.afterDialogues += new Game1.afterFadeFunction(this.CollectRacePrizes);
                  }
                  else if (!Utility.IsPassiveFestivalOpen(festivalId) && Game1.timeOfDay < 1000)
                    Game1.drawObjectDialogue(Game1.content.LoadString("Strings\\1_6_Strings:Race_Closed"));
                  else if (this.currentRaceState.Value >= DesertFestival.RaceState.Go && this.currentRaceState.Value < DesertFestival.RaceState.AnnounceWinner4)
                  {
                    int num2;
                    if (this.raceGuesses.TryGetValue(Game1.player.UniqueMultiplayerID, out num2) && this.currentRaceState.Value == DesertFestival.RaceState.Go)
                      Game1.drawObjectDialogue(Game1.content.LoadString("Strings\\1_6_Strings:Race_Guess_Already_Made", (object) Game1.content.LoadString("Strings\\1_6_Strings:Racer_" + num2.ToString())));
                    else
                      Game1.drawObjectDialogue(Game1.content.LoadString("Strings\\1_6_Strings:Race_Ongoing"));
                  }
                  else if (!this.CanMakeAnotherRaceGuess())
                  {
                    Game1.drawObjectDialogue(Game1.content.LoadString("Strings\\1_6_Strings:Race_Ended"));
                  }
                  else
                  {
                    int num3;
                    if (this.nextRaceGuesses.TryGetValue(Game1.player.UniqueMultiplayerID, out num3))
                      Game1.drawObjectDialogue(Game1.content.LoadString("Strings\\1_6_Strings:Race_Guess_Already_Made", (object) Game1.content.LoadString("Strings\\1_6_Strings:Racer_" + num3.ToString())));
                    else
                      this.createQuestionDialogue(Game1.content.LoadString("Strings\\1_6_Strings:Race_Question"), this.createYesNoResponses(), "Race");
                  }
                }
                return true;
              }
              break;
            case 'S':
              if (action == "DesertShadyGuy")
              {
                Game1.player.faceDirection(0);
                if (!Utility.IsPassiveFestivalOpen(festivalId) && Game1.timeOfDay < 1000)
                  Game1.drawObjectDialogue(Game1.content.LoadString("Strings\\1_6_Strings:Shady_Guy_Closed"));
                if (this.currentRaceState.Value >= DesertFestival.RaceState.Go && this.currentRaceState.Value < DesertFestival.RaceState.AnnounceWinner4)
                  Game1.drawObjectDialogue(Game1.content.LoadString("Strings\\1_6_Strings:Shady_Guy_Ongoing"));
                else if (!this.CanMakeAnotherRaceGuess())
                  Game1.drawObjectDialogue(Game1.content.LoadString("Strings\\1_6_Strings:Shady_Guy_Ended"));
                else if (this.sabotages.ContainsKey(Game1.player.UniqueMultiplayerID))
                  this.ShowSabotagedRaceText();
                else if (!Game1.player.mailReceived.Contains("Desert_Festival_Shady_Guy"))
                {
                  Game1.player.mailReceived.Add("Desert_Festival_Shady_Guy");
                  Game1.multipleDialogues(new string[3]
                  {
                    Game1.content.LoadString("Strings\\1_6_Strings:Shady_Guy_Intro"),
                    Game1.content.LoadString("Strings\\1_6_Strings:Shady_Guy_Intro_2"),
                    Game1.content.LoadString("Strings\\1_6_Strings:Shady_Guy_Intro_3")
                  });
                  Game1.afterDialogues += (Game1.afterFadeFunction) (() => this.createQuestionDialogue(Game1.content.LoadString("Strings\\1_6_Strings:Shady_Guy"), this.createYesNoResponses(), "Shady_Guy"));
                }
                else
                  this.createQuestionDialogue(Game1.content.LoadString("Strings\\1_6_Strings:Shady_Guy_2nd"), this.createYesNoResponses(), "Shady_Guy");
                return true;
              }
              break;
          }
          break;
        case 15:
          if (action == "DesertCactusMan")
          {
            Game1.player.faceDirection(0);
            if (!Utility.IsPassiveFestivalOpen(festivalId))
            {
              Game1.drawObjectDialogue(Game1.content.LoadString("Strings\\1_6_Strings:CactusMan_Closed"));
              break;
            }
            if (Game1.player.isInventoryFull())
            {
              Game1.drawObjectDialogue(Game1.content.LoadString("Strings\\1_6_Strings:CactusMan_Yes_Full"));
              break;
            }
            if (!Game1.player.mailReceived.Contains(this.GetCactusMail()))
            {
              Game1.drawObjectDialogue(Game1.content.LoadString("Strings\\1_6_Strings:CactusMan_Intro_" + Game1.random.Next(1, 4).ToString()));
              Game1.afterDialogues += (Game1.afterFadeFunction) (() => this.createQuestionDialogue(Game1.content.LoadString("Strings\\1_6_Strings:CactusMan_Question"), this.createYesNoResponses(), "CactusMan"));
              break;
            }
            Game1.drawObjectDialogue(Game1.content.LoadString("Strings\\1_6_Strings:CactusMan_Collected"));
            break;
          }
          break;
        case 18:
          if (action == "DesertFishingBoard" && Game1.Date != who.lastDesertFestivalFishingQuest.Value)
          {
            List<Response> responseList = new List<Response>()
            {
              new Response("Yes", Game1.content.LoadString("Strings\\1_6_Strings:Accept")),
              new Response("No", Game1.content.LoadString("Strings\\1_6_Strings:Decline"))
            };
            this.createQuestionDialogue(Game1.content.LoadString("Strings\\1_6_Strings:Willy_DesertFishing" + Utility.GetDayOfPassiveFestival(nameof (DesertFestival)).ToString()), responseList.ToArray(), "Fishing_Quest");
            break;
          }
          break;
        case 29:
          if (action == "DesertFestivalMineExplanation")
          {
            Game1.player.mailReceived.Add("Checked_DF_Mine_Explanation");
            this.checkedMineExplanation = true;
            Game1.multipleDialogues(new string[3]
            {
              Game1.content.LoadString("Strings\\1_6_Strings:DF_Mine_Explanation"),
              Game1.content.LoadString("Strings\\1_6_Strings:DF_Mine_Explanation_2"),
              Game1.content.LoadString("Strings\\1_6_Strings:DF_Mine_Explanation_3")
            });
            break;
          }
          break;
      }
    }
    return base.performAction(action, who, tile_location);
  }

  public string GetCactusMail() => $"Y{Game1.year.ToString()}_Cactus";

  public string GetScholarMail() => $"Y{Game1.year.ToString()}_Scholar";

  public virtual Response[] GetRacerResponses()
  {
    List<Response> responseList = new List<Response>();
    foreach (Racer netRacer in this.netRacers)
      responseList.Add(new Response(netRacer.racerIndex.ToString(), Game1.content.LoadString("Strings\\1_6_Strings:Racer_" + netRacer.racerIndex.Value.ToString())));
    responseList.Add(new Response("cancel", Game1.content.LoadString("Strings\\Locations:MineCart_Destination_Cancel")));
    return responseList.ToArray();
  }

  public virtual void ShowSabotagedRaceText()
  {
    int num;
    if (!this.sabotages.TryGetValue(Game1.player.UniqueMultiplayerID, out num))
      return;
    if (this._localSabotageText == -1)
      this._localSabotageText = Game1.random.Next(1, 4);
    Game1.drawObjectDialogue(Game1.content.LoadString("Strings\\1_6_Strings:Shady_Guy_Selected_" + this._localSabotageText.ToString(), (object) Game1.content.LoadString("Strings\\1_6_Strings:Racer_" + num.ToString())));
  }

  private void generateNextScholarQuestion()
  {
    Random random = Utility.CreateRandom((double) Game1.uniqueIDForThisGame);
    int num = (random.Next(3) + Game1.year) % 3;
    string str1 = $"Scholar_Question_{this._currentScholarQuestion.ToString()}_{num.ToString()}";
    string str2 = $"Scholar_Question_{this._currentScholarQuestion.ToString()}_{num.ToString()}_Options";
    string str3 = $"Scholar_Question_{this._currentScholarQuestion.ToString()}_{num.ToString()}_Answers";
    string[] strArray1 = (string[]) null;
    int index1 = 0;
    try
    {
      strArray1 = Game1.content.LoadString("Strings\\1_6_Strings:" + str2).Split(',');
      index1 = random.Next(strArray1.Length);
    }
    catch (Exception ex)
    {
    }
    string[] strArray2 = Game1.content.LoadString("Strings\\1_6_Strings:" + str3).Split(',');
    string question = strArray1 != null ? Game1.content.LoadString("Strings\\1_6_Strings:" + str1, (object) strArray1[index1]) : Game1.content.LoadString("Strings\\1_6_Strings:" + str1);
    List<Response> list = new List<Response>();
    if (this._currentScholarQuestion == 2 && num == 1)
    {
      list.Add(new Response("Correct", Game1.stats.StepsTaken.ToString() ?? ""));
      list.Add(new Response("Wrong", (Game1.stats.StepsTaken * 2U).ToString() ?? ""));
      list.Add(new Response("Wrong", (Game1.stats.StepsTaken / 2U).ToString() ?? ""));
    }
    else
    {
      list.Add(new Response("Correct", strArray2[index1]));
      int index2 = index1;
      while (index2 == index1)
        index2 = random.Next(strArray2.Length);
      list.Add(new Response("Wrong", strArray2[index2]));
      int index3 = index1;
      while (index3 == index1 || index3 == index2)
        index3 = random.Next(strArray2.Length);
      list.Add(new Response("Wrong", strArray2[index3]));
    }
    Utility.Shuffle<Response>(random, list);
    this.createQuestionDialogue(question, list.ToArray(), "DesertScholar_Answer_");
    ++this._currentScholarQuestion;
  }

  public override void customQuestCompleteBehavior(string questId)
  {
    if (questId == "98765")
    {
      switch (Utility.GetDayOfPassiveFestival(nameof (DesertFestival)))
      {
        case 1:
          Game1.player.addItemByMenuIfNecessary(ItemRegistry.Create("CalicoEgg", 25));
          break;
        case 2:
          Game1.player.addItemByMenuIfNecessary(ItemRegistry.Create("CalicoEgg", 50));
          break;
        case 3:
          Game1.player.addItemByMenuIfNecessary(ItemRegistry.Create("CalicoEgg", 30));
          break;
      }
    }
    base.customQuestCompleteBehavior(questId);
  }

  public override bool answerDialogueAction(string question_and_answer, string[] question_params)
  {
    if (question_and_answer == null)
      return false;
    if (question_and_answer != null)
    {
      switch (question_and_answer.Length)
      {
        case 8:
          if (question_and_answer == "Race_Yes")
          {
            this.createQuestionDialogue(Game1.content.LoadString("Strings\\1_6_Strings:Race_Guess"), this.GetRacerResponses(), "Race_Guess_");
            return true;
          }
          break;
        case 12:
          if (question_and_answer == "CactusMan_No")
          {
            Game1.drawObjectDialogue(Game1.content.LoadString("Strings\\1_6_Strings:CactusMan_No"));
            return true;
          }
          break;
        case 13:
          switch (question_and_answer[0])
          {
            case 'C':
              if (question_and_answer == "CactusMan_Yes")
              {
                Game1.drawObjectDialogue(Game1.content.LoadString("Strings\\1_6_Strings:CactusMan_Yes_Intro"));
                Game1.afterDialogues += (Game1.afterFadeFunction) (() =>
                {
                  if (Game1.player.isInventoryFull())
                  {
                    Game1.drawObjectDialogue(Game1.content.LoadString("Strings\\1_6_Strings:CactusMan_Yes_Full"));
                  }
                  else
                  {
                    int seed = Utility.CreateRandomSeed((double) Game1.player.UniqueMultiplayerID, (double) Game1.year);
                    Game1.player.freezePause = 4000;
                    DelayedAction.functionAfterDelay((Action) (() => this._revealCactusEvent.Fire(seed)), 1000);
                    DelayedAction.functionAfterDelay((Action) (() =>
                    {
                      Random random = Utility.CreateRandom((double) seed);
                      random.Next();
                      random.Next();
                      random.Next();
                      Game1.drawObjectDialogue(Game1.content.LoadString("Strings\\1_6_Strings:CactusMan_Yes_" + random.Next(1, 6).ToString()));
                      Game1.afterDialogues += (Game1.afterFadeFunction) (() =>
                      {
                        if (Game1.player.addItemToInventoryBool((Item) new RandomizedPlantFurniture("FreeCactus", Vector2.Zero, seed)))
                        {
                          Game1.playSound("coin");
                          Game1.player.mailReceived.Add(this.GetCactusMail());
                        }
                        this._hideCactusEvent.Fire(seed);
                        Game1.player.freezePause = 100;
                      });
                    }), 3000);
                  }
                });
                return true;
              }
              break;
            case 'S':
              if (question_and_answer == "Shady_Guy_Yes")
              {
                if (Game1.player.Items.CountId("CalicoEgg") >= 1)
                {
                  Game1.player.Items.ReduceId("CalicoEgg", 1);
                  this.createQuestionDialogue(Game1.content.LoadString("Strings\\1_6_Strings:Shady_Guy_Question"), this.GetRacerResponses(), "Shady_Guy_Sabotage_");
                  break;
                }
                Game1.drawObjectDialogue(Game1.content.LoadString("Strings\\1_6_Strings:Shady_Guy_NoEgg"));
                break;
              }
              break;
          }
          break;
        case 17:
          switch (question_and_answer[0])
          {
            case 'F':
              if (question_and_answer == "Fishing_Quest_Yes")
              {
                Quest quest;
                if (Utility.GetDayOfPassiveFestival(nameof (DesertFestival)) == 3)
                {
                  quest = (Quest) new ItemDeliveryQuest("Willy", "GoldenBobber", Game1.content.LoadString("Strings\\1_6_Strings:Willy_Challenge"), Game1.content.LoadString("Strings\\1_6_Strings:Willy_Challenge_Description_" + Utility.GetDayOfPassiveFestival(nameof (DesertFestival)).ToString()), "Strings\\1_6_Strings:Willy_GoldenBobber", Game1.content.LoadString("Strings\\1_6_Strings:Willy_Challenge_Return_" + Utility.GetDayOfPassiveFestival(nameof (DesertFestival)).ToString()));
                }
                else
                {
                  string itemId = Utility.GetDayOfPassiveFestival(nameof (DesertFestival)) == 1 ? "164" : "165";
                  int numberToFish = Utility.GetDayOfPassiveFestival(nameof (DesertFestival)) == 1 ? 3 : 1;
                  string questTitle = Game1.content.LoadString("Strings\\1_6_Strings:Willy_Challenge");
                  LocalizedContentManager content1 = Game1.content;
                  int ofPassiveFestival = Utility.GetDayOfPassiveFestival(nameof (DesertFestival));
                  string path1 = "Strings\\1_6_Strings:Willy_Challenge_Description_" + ofPassiveFestival.ToString();
                  string questDescription = content1.LoadString(path1);
                  LocalizedContentManager content2 = Game1.content;
                  ofPassiveFestival = Utility.GetDayOfPassiveFestival(nameof (DesertFestival));
                  string path2 = "Strings\\1_6_Strings:Willy_Challenge_Return_" + ofPassiveFestival.ToString();
                  string returnDialogue = content2.LoadString(path2);
                  quest = (Quest) new FishingQuest(itemId, numberToFish, "Willy", questTitle, questDescription, returnDialogue);
                }
                quest.daysLeft.Value = 1;
                quest.id.Value = "98765";
                Game1.player.questLog.Add(quest);
                Game1.player.lastDesertFestivalFishingQuest.Value = Game1.Date;
                return true;
              }
              break;
            case 'G':
              if (question_and_answer == "Gil_EggRating_Yes")
              {
                Game1.player.lastGotPrizeFromGil.Value = Game1.Date;
                Game1.player.freezePause = 1400;
                DelayedAction.playSoundAfterDelay("coin", 500);
                DelayedAction.functionAfterDelay((Action) (() =>
                {
                  int num = Game1.player.team.highestCalicoEggRatingToday.Value + 1;
                  int eggPrize = 0;
                  Item extraPrize = (Item) null;
                  if (num >= 1000)
                    Game1.drawObjectDialogue(Game1.content.LoadString("Strings\\1_6_Strings:Gil_Rating_1000"));
                  else if (num >= 55)
                  {
                    Game1.DrawDialogue(Game1.RequireLocation<AdventureGuild>("AdventureGuild").Gil, "Strings\\1_6_Strings:Gil_Rating_50", (object) num);
                    eggPrize = 500;
                    extraPrize = (Item) new StardewValley.Object("279", 1);
                  }
                  else if (num >= 25)
                  {
                    Game1.DrawDialogue(Game1.RequireLocation<AdventureGuild>("AdventureGuild").Gil, "Strings\\1_6_Strings:Gil_Rating_25", (object) num);
                    eggPrize = 200;
                    if (!Game1.player.mailReceived.Contains("DF_Gil_Hat"))
                    {
                      extraPrize = (Item) new Hat("GilsHat");
                      Game1.player.mailReceived.Add("DF_Gil_Hat");
                    }
                    else
                      extraPrize = (Item) new StardewValley.Object("253", 5);
                  }
                  else if (num >= 20)
                  {
                    Game1.DrawDialogue(Game1.RequireLocation<AdventureGuild>("AdventureGuild").Gil, "Strings\\1_6_Strings:Gil_Rating_20to24", (object) num);
                    eggPrize = 100;
                    extraPrize = (Item) new StardewValley.Object("253", 5);
                  }
                  else if (num >= 15)
                  {
                    Game1.DrawDialogue(Game1.RequireLocation<AdventureGuild>("AdventureGuild").Gil, "Strings\\1_6_Strings:Gil_Rating_15to19", (object) num);
                    eggPrize = 50;
                    extraPrize = (Item) new StardewValley.Object("253", 3);
                  }
                  else if (num >= 10)
                  {
                    Game1.DrawDialogue(Game1.RequireLocation<AdventureGuild>("AdventureGuild").Gil, "Strings\\1_6_Strings:Gil_Rating_10to14", (object) num);
                    eggPrize = 25;
                    extraPrize = (Item) new StardewValley.Object("253", 1);
                  }
                  else if (num >= 5)
                  {
                    Game1.DrawDialogue(Game1.RequireLocation<AdventureGuild>("AdventureGuild").Gil, "Strings\\1_6_Strings:Gil_Rating_5to9", (object) num);
                    eggPrize = 10;
                    extraPrize = (Item) new StardewValley.Object("395", 1);
                  }
                  else
                  {
                    Game1.DrawDialogue(Game1.RequireLocation<AdventureGuild>("AdventureGuild").Gil, "Strings\\1_6_Strings:Gil_Rating_1to4", (object) num);
                    eggPrize = 1;
                    extraPrize = (Item) new StardewValley.Object("243", 1);
                  }
                  Game1.afterDialogues = (Game1.afterFadeFunction) (() =>
                  {
                    Game1.player.addItemByMenuIfNecessaryElseHoldUp((Item) new StardewValley.Object("CalicoEgg", eggPrize));
                    if (extraPrize == null)
                      return;
                    Game1.afterDialogues = (Game1.afterFadeFunction) (() => Game1.player.addItemByMenuIfNecessary(extraPrize));
                  });
                }), 1000);
                break;
              }
              break;
          }
          break;
        case 18:
          if (question_and_answer == "WarperQuestion_Yes")
          {
            if (Game1.player.Money < 250)
            {
              Game1.drawObjectDialogue(Game1.content.LoadString("Strings\\Locations:BusStop_NotEnoughMoneyForTicket"));
            }
            else
            {
              Game1.player.Money -= 250;
              Game1.player.CanMove = true;
              ItemRegistry.Create<StardewValley.Object>("(O)688").performUseAction((GameLocation) this);
              Game1.player.freezePause = 5000;
            }
            return true;
          }
          break;
      }
    }
    if (question_and_answer.StartsWith("Race_Guess_"))
    {
      string s = question_and_answer.Substring("Race_Guess_".Length + 1);
      int num = -1;
      ref int local = ref num;
      if (int.TryParse(s, out local))
      {
        if (this.currentRaceState.Value >= DesertFestival.RaceState.Go && this.currentRaceState.Value < DesertFestival.RaceState.AnnounceWinner4)
        {
          Game1.drawObjectDialogue(Game1.content.LoadString("Strings\\1_6_Strings:Race_Late_Guess"));
          return true;
        }
        string str = "Strings\\1_6_Strings:Racer_" + num.ToString();
        string sub1 = Game1.content.LoadString(str);
        Game1.drawObjectDialogue(Game1.content.LoadString("Strings\\1_6_Strings:Race_Guess_Made", (object) sub1));
        Game1.multiplayer.globalChatInfoMessage("GuessRacer_" + Game1.random.Next(1, 11).ToString(), Game1.player.Name, TokenStringBuilder.LocalizedText(str));
        this.nextRaceGuesses[Game1.player.UniqueMultiplayerID] = num;
      }
      return true;
    }
    if (question_and_answer.StartsWith("Shady_Guy_Sabotage_"))
    {
      string s = question_and_answer.Substring("Shady_Guy_Sabotage_".Length + 1);
      int num = -1;
      ref int local = ref num;
      if (int.TryParse(s, out local))
      {
        if (this.currentRaceState.Value >= DesertFestival.RaceState.Go && this.currentRaceState.Value < DesertFestival.RaceState.AnnounceWinner4)
        {
          Game1.drawObjectDialogue(Game1.content.LoadString("Strings\\1_6_Strings:Shady_Guy_Late"));
          return true;
        }
        if (!this.sabotages.Any() && Game1.random.NextDouble() < 0.25)
          Game1.multiplayer.globalChatInfoMessage("RaceSabotage_" + Game1.random.Next(1, 6).ToString());
        this.sabotages[Game1.player.UniqueMultiplayerID] = num;
        this._localSabotageText = -1;
        this.ShowSabotagedRaceText();
      }
      return true;
    }
    if (question_and_answer.StartsWith("DesertScholar"))
    {
      if (question_and_answer == "DesertScholar_Yes")
      {
        ++this._currentScholarQuestion;
        Game1.drawObjectDialogue(Game1.content.LoadString("Strings\\1_6_Strings:Scholar_Intro2"));
        Game1.afterDialogues += (Game1.afterFadeFunction) (() => this.generateNextScholarQuestion());
      }
      else if (question_and_answer.StartsWith("DesertScholar_Answer_"))
      {
        switch (question_and_answer)
        {
          case "DesertScholar_Answer__Wrong":
            Game1.playSound("cancel");
            Game1.drawObjectDialogue(Game1.content.LoadString("Strings\\1_6_Strings:Scholar_Wrong"));
            this._currentScholarQuestion = -2;
            break;
          case "DesertScholar_Answer__Correct":
            Game1.drawObjectDialogue(Game1.content.LoadString("Strings\\1_6_Strings:Scholar_Correct"));
            Game1.playSound("give_gift");
            if (this._currentScholarQuestion == 4)
            {
              Game1.player.mailReceived.Add(this.GetScholarMail());
              Game1.afterDialogues += (Game1.afterFadeFunction) (() =>
              {
                Game1.drawObjectDialogue(Game1.content.LoadString("Strings\\1_6_Strings:Scholar_Win"));
                Game1.afterDialogues += (Game1.afterFadeFunction) (() =>
                {
                  Game1.player.addItemByMenuIfNecessary(ItemRegistry.Create("CalicoEgg", 50));
                  Game1.playSound("coin");
                });
              });
              break;
            }
            Game1.afterDialogues += (Game1.afterFadeFunction) (() => this.generateNextScholarQuestion());
            break;
        }
      }
    }
    if (question_and_answer.StartsWith("Cook"))
    {
      if (question_and_answer.EndsWith("No"))
        Game1.drawObjectDialogue(Game1.content.LoadString("Strings\\1_6_Strings:Cook_Intro_No"));
      else if (question_and_answer.StartsWith("Cook_ChoseSauce"))
      {
        Game1.playSound("smallSelect");
        this._cookSauce = Convert.ToInt32(question_and_answer[question_and_answer.Length - 1].ToString() ?? "");
        Game1.drawObjectDialogue(Game1.content.LoadString("Strings\\1_6_Strings:Cook_ChoseSauce", (object) Game1.content.LoadString("Strings\\1_6_Strings:Cook_Sauce" + this._cookSauce.ToString())));
        Game1.afterDialogues += (Game1.afterFadeFunction) (() =>
        {
          this.temporarySprites.Add(new TemporaryAnimatedSprite("Maps\\desert_festival_tilesheet", new Microsoft.Xna.Framework.Rectangle(320, 280, 29, 24), new Vector2(480f, 1372f), false, 0.0f, Color.White)
          {
            id = 1001,
            animationLength = 2,
            interval = 200f,
            totalNumberOfLoops = 9999,
            scale = 4f,
            layerDepth = 0.1343f
          });
          this.temporarySprites.Add(new TemporaryAnimatedSprite("Maps\\desert_festival_tilesheet", new Microsoft.Xna.Framework.Rectangle(378, 280, 29, 24), new Vector2(480f, 1372f), false, 0.0f, Color.White)
          {
            id = 1002,
            animationLength = 4,
            interval = 100f,
            totalNumberOfLoops = 4,
            delayBeforeAnimationStart = 400,
            scale = 4f,
            layerDepth = 0.1344f
          });
          DelayedAction.playSoundAfterDelay("hammer", 800, (GameLocation) this);
          DelayedAction.playSoundAfterDelay("hammer", 1200, (GameLocation) this);
          DelayedAction.playSoundAfterDelay("hammer", 1600, (GameLocation) this);
          DelayedAction.playSoundAfterDelay("hammer", 2000, (GameLocation) this);
          DelayedAction.playSoundAfterDelay("furnace", 2500, (GameLocation) this);
          for (int index = 0; index < 12; ++index)
          {
            this.temporarySprites.Add(new TemporaryAnimatedSprite(30, new Vector2(460.8f + (float) Game1.random.Next(-10, 10), (float) (1388 + Game1.random.Next(-10, 10))), Color.White, 4, numberOfLoops: 2)
            {
              delayBeforeAnimationStart = 2700 + index * 80 /*0x50*/,
              motion = new Vector2((float) ((double) Game1.random.Next(-5, 5) / 10.0 - 1.0), (float) ((double) Game1.random.Next(-5, 5) / 10.0 - 1.0)),
              drawAboveAlwaysFront = true
            });
            this.temporarySprites.Add(new TemporaryAnimatedSprite(30, new Vector2(544f + (float) Game1.random.Next(-10, 10), (float) (1388 + Game1.random.Next(-10, 10))), Color.White, 4, numberOfLoops: 2)
            {
              delayBeforeAnimationStart = 2700 + index * 80 /*0x50*/,
              motion = new Vector2((float) (1.0 + (double) Game1.random.Next(-5, 5) / 10.0), (float) ((double) Game1.random.Next(-5, 5) / 10.0 - 1.0)),
              drawAboveAlwaysFront = true
            });
            if (index % 2 == 0)
              this.temporarySprites.Add(new TemporaryAnimatedSprite("Tilesheets\\Animations", new Microsoft.Xna.Framework.Rectangle(0, 2944, 64 /*0x40*/, 64 /*0x40*/), new Vector2(505.6f + (float) Game1.random.Next(-16, 16 /*0x10*/), 1344f), Game1.random.NextDouble() < 0.5, 0.0f, Color.Gray)
              {
                delayBeforeAnimationStart = 2700 + index * 80 /*0x50*/,
                motion = new Vector2(0.0f, -0.25f),
                animationLength = 8,
                interval = 70f,
                drawAboveAlwaysFront = true
              });
          }
          Game1.player.freezePause = 4805;
          DelayedAction.functionAfterDelay((Action) (() =>
          {
            this.removeTemporarySpritesWithID(1001);
            this.removeTemporarySpritesWithID(1002);
            Game1.drawObjectDialogue(Game1.content.LoadString("Strings\\1_6_Strings:Cook_Done", (object) Game1.content.LoadString($"Strings\\1_6_Strings:Cook_DishNames_{this._cookIngredient.ToString()}_{this._cookSauce.ToString()}")));
            Game1.afterDialogues += (Game1.afterFadeFunction) (() =>
            {
              StardewValley.Object food = new StardewValley.Object();
              food.edibility.Value = Game1.player.maxHealth;
              string path = $"Strings\\1_6_Strings:Cook_DishNames_{this._cookIngredient.ToString()}_{this._cookSauce.ToString()}";
              food.name = Game1.content.LoadString(path);
              food.displayNameFormat = $"[LocalizedText {path}]";
              BuffEffects effects = new BuffEffects();
              switch (this._cookIngredient)
              {
                case 0:
                  effects.Defense.Value = 3f;
                  break;
                case 1:
                  effects.MiningLevel.Value = 3f;
                  break;
                case 2:
                  effects.LuckLevel.Value = 3f;
                  break;
                case 3:
                  effects.Attack.Value = 3f;
                  break;
                case 4:
                  effects.FishingLevel.Value = 3f;
                  break;
              }
              switch (this._cookSauce)
              {
                case 0:
                  effects.Defense.Value = 1f;
                  break;
                case 1:
                  effects.MiningLevel.Value = 1f;
                  break;
                case 2:
                  effects.LuckLevel.Value = 1f;
                  break;
                case 3:
                  effects.Attack.Value = 1f;
                  break;
                case 4:
                  effects.Speed.Value = 1f;
                  break;
              }
              food.customBuff = (Func<Buff>) (() => new Buff(nameof (DesertFestival), food.Name, food.Name, 600 * Game1.realMilliSecondsPerGameMinute, effects: effects));
              int sourceIndex = this._cookIngredient * 4 + this._cookSauce + (this._cookSauce > this._cookIngredient ? -1 : 0);
              Game1.player.tempFoodItemTextureName.Value = "TileSheets\\Objects_2";
              Game1.player.tempFoodItemSourceRect.Value = Utility.getSourceRectWithinRectangularRegion(0, 32 /*0x20*/, 128 /*0x80*/, sourceIndex, 16 /*0x10*/, 16 /*0x10*/);
              Game1.player.faceDirection(2);
              Game1.player.eatObject(food);
            });
          }), 4800);
        });
      }
      else if (question_and_answer.StartsWith("Cook_PickedIngredient"))
      {
        Game1.playSound("smallSelect");
        this._cookIngredient = Convert.ToInt32(question_and_answer[question_and_answer.Length - 1].ToString() ?? "");
        List<Response> responseList = new List<Response>();
        for (int index = 0; index < 5; ++index)
        {
          if (index != this._cookIngredient || this._cookIngredient == 4)
            responseList.Add(new Response(index.ToString() ?? "", Game1.content.LoadString("Strings\\1_6_Strings:Cook_Sauce" + index.ToString())));
        }
        this.createQuestionDialogue(Game1.content.LoadString("Strings\\1_6_Strings:Cook_ChoseIngredient", (object) Game1.content.LoadString("Strings\\1_6_Strings:Cook_Ingredient" + this._cookIngredient.ToString())), responseList.ToArray(), "Cook_ChoseSauce");
      }
      else
      {
        switch (question_and_answer)
        {
          case "Cook_Intro_Yes":
            Game1.playSound("smallSelect");
            Game1.drawObjectDialogue(Game1.content.LoadString("Strings\\1_6_Strings:Cook_Intro_Yes"));
            Game1.afterDialogues += (Game1.afterFadeFunction) (() =>
            {
              Game1.playSound("smallSelect");
              this.createQuestionDialogue(Game1.content.LoadString("Strings\\1_6_Strings:Cook_Intro_Yes2"), this.createYesNoResponses(), "Cook_Intro2");
            });
            break;
          case "Cook_Intro2_Yes":
            Game1.playSound("smallSelect");
            Response[] answerChoices = new Response[5];
            for (int index = 0; index < 5; ++index)
              answerChoices[index] = new Response(index.ToString() ?? "", Game1.content.LoadString("Strings\\1_6_Strings:Cook_Ingredient" + index.ToString()));
            this.createQuestionDialogue(Game1.content.LoadString("Strings\\1_6_Strings:Cook_Intro_Yes3"), answerChoices, "Cook_PickedIngredient");
            break;
        }
      }
    }
    return base.answerDialogueAction(question_and_answer, question_params);
  }

  public void CactusGuyHideCactus(int seed)
  {
    if (this._currentlyShownCactusID != seed)
      return;
    this._cactusGuyRevealItem = (RandomizedPlantFurniture) null;
    this._cactusGuyRevealTimer = -1f;
    this._cactusShakeTimer = -1f;
    this._currentlyShownCactusID = -1;
  }

  public void CactusGuyRevealCactus(int seed)
  {
    RandomizedPlantFurniture randomizedPlantFurniture = new RandomizedPlantFurniture("FreeCactus", Vector2.Zero, seed);
    this._currentlyShownCactusID = seed;
    this._cactusGuyRevealItem = randomizedPlantFurniture.getOne() as RandomizedPlantFurniture;
    this._cactusGuyRevealTimer = 0.0f;
    this._cactusShakeTimer = -1f;
    Random random = Utility.CreateRandom((double) seed);
    random.Next();
    random.Next();
    List<string> options = new List<string>()
    {
      "pig",
      "Duck",
      "dog_bark",
      "cat",
      "camel"
    };
    Game1.playSound("throwDownITem");
    DelayedAction.playSoundAfterDelay("thudStep", 500);
    DelayedAction.playSoundAfterDelay("thudStep", 750);
    DelayedAction.playSoundAfterDelay(random.ChooseFrom<string>((IList<string>) options), 1000);
    DelayedAction.functionAfterDelay((Action) (() => this._cactusShakeTimer = 0.25f), 1000);
  }

  public bool CanMakeAnotherRaceGuess()
  {
    return Game1.timeOfDay < 2200 || this.currentRaceState.Value < DesertFestival.RaceState.Go;
  }

  public override void UpdateWhenCurrentLocation(GameTime time)
  {
    if ((double) this._cactusShakeTimer > 0.0)
    {
      this._cactusShakeTimer -= (float) time.ElapsedGameTime.TotalSeconds;
      if ((double) this._cactusShakeTimer <= 0.0)
        this._cactusShakeTimer = -1f;
    }
    if ((double) this._raceTextTimer > 0.0)
    {
      this._raceTextTimer -= (float) time.ElapsedGameTime.TotalSeconds;
      if ((double) this._raceTextTimer < 0.0)
        this._raceTextTimer = 0.0f;
    }
    if ((double) this._cactusGuyRevealTimer >= 0.0 && (double) this._cactusGuyRevealTimer < 1.0)
    {
      this._cactusGuyRevealTimer += (float) (time.ElapsedGameTime.TotalSeconds / 0.75);
      if ((double) this._cactusGuyRevealTimer >= 1.0)
        this._cactusGuyRevealTimer = 1f;
    }
    this._revealCactusEvent.Poll();
    this._hideCactusEvent.Poll();
    this.announceRaceEvent.Poll();
    if (Game1.shouldTimePass())
    {
      if (Game1.IsMasterGame)
      {
        if ((double) this._raceStateTimer >= 0.0)
        {
          this._raceStateTimer -= (float) time.ElapsedGameTime.TotalSeconds;
          if ((double) this._raceStateTimer <= 0.0)
          {
            this._raceStateTimer = 0.0f;
            switch (this.currentRaceState.Value)
            {
              case DesertFestival.RaceState.StartingLine:
                this.announceRaceEvent.Fire("Race_Ready");
                this._raceStateTimer = 3f;
                this.currentRaceState.Value = DesertFestival.RaceState.Ready;
                break;
              case DesertFestival.RaceState.Ready:
                this.currentRaceState.Value = DesertFestival.RaceState.Set;
                this.announceRaceEvent.Fire("Race_Set");
                this._raceStateTimer = 3f;
                break;
              case DesertFestival.RaceState.Set:
                this.currentRaceState.Value = DesertFestival.RaceState.Go;
                this.announceRaceEvent.Fire("Race_Go");
                this.raceGuesses.Clear();
                foreach (KeyValuePair<long, int> pair in this.nextRaceGuesses.Pairs)
                  this.raceGuesses[pair.Key] = pair.Value;
                this.nextRaceGuesses.Clear();
                foreach (Racer netRacer in this.netRacers)
                {
                  netRacer.sabotages.Value = 0;
                  foreach (int num in this.sabotages.Values)
                  {
                    if (num == netRacer.racerIndex.Value)
                      ++netRacer.sabotages.Value;
                  }
                  netRacer.ResetMoveSpeed();
                }
                this.sabotages.Clear();
                this._raceStateTimer = 3f;
                break;
              case DesertFestival.RaceState.AnnounceWinner:
              case DesertFestival.RaceState.AnnounceWinner2:
              case DesertFestival.RaceState.AnnounceWinner3:
              case DesertFestival.RaceState.AnnounceWinner4:
                this._raceStateTimer = 2f;
                switch (this.currentRaceState.Value)
                {
                  case DesertFestival.RaceState.AnnounceWinner:
                    this.announceRaceEvent.Fire("Race_Comment_" + Game1.random.Next(1, 5).ToString());
                    this._raceStateTimer = 4f;
                    break;
                  case DesertFestival.RaceState.AnnounceWinner2:
                    this.announceRaceEvent.Fire("Race_Winner");
                    this._raceStateTimer = 2f;
                    break;
                  case DesertFestival.RaceState.AnnounceWinner3:
                    this.announceRaceEvent.Fire("Racer_" + this.lastRaceWinner.Value.ToString());
                    this._raceStateTimer = 4f;
                    break;
                  case DesertFestival.RaceState.AnnounceWinner4:
                    this.announceRaceEvent.Fire("RESULT");
                    this._raceStateTimer = 2f;
                    this.finishedRacers.Clear();
                    break;
                }
                ++this.currentRaceState.Value;
                break;
              case DesertFestival.RaceState.RaceEnd:
                if (!this.CanMakeAnotherRaceGuess())
                {
                  if (Utility.GetDayOfPassiveFestival(nameof (DesertFestival)) < 3)
                    this.announceRaceEvent.Fire("Race_Close");
                  else
                    this.announceRaceEvent.Fire("Race_Close_LastDay");
                  this.currentRaceState.Value = DesertFestival.RaceState.RacesOver;
                  break;
                }
                this.currentRaceState.Value = DesertFestival.RaceState.PreRace;
                break;
            }
          }
        }
        if (this.currentRaceState.Value == DesertFestival.RaceState.Go)
        {
          if (this.finishedRacers.Count >= this.racerCount)
          {
            this.currentRaceState.Value = DesertFestival.RaceState.AnnounceWinner;
            this._raceStateTimer = 2f;
          }
          else
          {
            foreach (Racer netRacer in this.netRacers)
              netRacer.UpdateRaceProgress(this);
          }
        }
      }
      foreach (Racer netRacer in this.netRacers)
        netRacer.Update(this);
    }
    this.festivalChimneyTimer -= (float) time.ElapsedGameTime.Milliseconds;
    if ((double) this.festivalChimneyTimer <= 0.0)
    {
      this.AddSmokePuff(new Vector2(7.25f, 16.25f) * 64f);
      this.AddSmokePuff(new Vector2(28.25f, 6f) * 64f);
      this.festivalChimneyTimer = 500f;
    }
    if (Game1.isStartingToGetDarkOut((GameLocation) this) && Game1.outdoorLight.R > (byte) 160 /*0xA0*/)
    {
      Game1.outdoorLight.R = (byte) 160 /*0xA0*/;
      Game1.outdoorLight.G = (byte) 160 /*0xA0*/;
      Game1.outdoorLight.B = (byte) 0;
    }
    base.UpdateWhenCurrentLocation(time);
  }

  public void OnRaceWon(int winner)
  {
    this.lastRaceWinner.Value = winner;
    if (this.raceGuesses.FieldDict.Count <= 0)
      return;
    List<string> stringList = new List<string>();
    foreach (KeyValuePair<long, int> pair in this.raceGuesses.Pairs)
    {
      if (pair.Value == winner)
      {
        if (winner == 3 && !this.specialRewardsCollected.ContainsKey(pair.Key))
        {
          this.specialRewardsCollected[pair.Key] = false;
        }
        else
        {
          if (!this.rewardsToCollect.ContainsKey(pair.Key))
            this.rewardsToCollect[pair.Key] = 0;
          this.rewardsToCollect[pair.Key]++;
          Farmer player = Game1.GetPlayer(pair.Key);
          if (player != null)
            stringList.Add(player.Name);
        }
      }
    }
    string str = TokenStringBuilder.LocalizedText("Strings\\1_6_Strings:Racer_" + winner.ToString());
    switch (stringList.Count)
    {
      case 0:
        Game1.multiplayer.globalChatInfoMessage("RaceWinners_Zero", str);
        break;
      case 1:
        Game1.multiplayer.globalChatInfoMessage("RaceWinners_One", str, stringList[0]);
        break;
      case 2:
        Game1.multiplayer.globalChatInfoMessage("RaceWinners_Two", str, stringList[0], stringList[1]);
        break;
      default:
        Game1.multiplayer.globalChatInfoMessage("RaceWinners_Many", str);
        for (int index = 0; index < stringList.Count; ++index)
        {
          if (index < stringList.Count - 1)
            Game1.multiplayer.globalChatInfoMessage("RaceWinners_List", stringList[index]);
          else
            Game1.multiplayer.globalChatInfoMessage("RaceWinners_Final", stringList[index]);
        }
        break;
    }
  }

  public void AddSmokePuff(Vector2 v)
  {
    this.temporarySprites.Add(new TemporaryAnimatedSprite("LooseSprites\\Cursors", new Microsoft.Xna.Framework.Rectangle(372, 1956, 10, 10), v, false, 1f / 500f, Color.Gray)
    {
      alpha = 0.75f,
      motion = new Vector2(0.0f, -0.5f),
      acceleration = new Vector2(1f / 500f, 0.0f),
      interval = 99999f,
      layerDepth = 1f,
      scale = 2f,
      scaleChange = 0.02f,
      drawAboveAlwaysFront = true,
      rotationChange = (float) ((double) Game1.random.Next(-5, 6) * 3.1415927410125732 / 256.0)
    });
  }

  public static void CleanupFestival()
  {
    Game1.player.team.itemsToRemoveOvernight.Add("CalicoEgg");
    SpecialOrder.RemoveAllSpecialOrders("DesertFestivalMarlon");
  }

  public override void draw(SpriteBatch spriteBatch)
  {
    if ((double) this._cactusGuyRevealTimer > 0.0 && this._cactusGuyRevealItem != null)
    {
      Vector2 vector2_1 = new Vector2(29f, 66.5f) * 64f;
      Vector2 vector2_2 = new Vector2(27.5f, 66.5f) * 64f;
      float num1 = 0.6f;
      float num2 = (double) this._cactusGuyRevealTimer >= (double) num1 ? (float) (Math.Sin(((double) this._cactusGuyRevealTimer - (double) num1) / (1.0 - (double) num1) * Math.PI) * 8.0 * 4.0) : (float) (Math.Sin((double) this._cactusGuyRevealTimer / (double) num1 * Math.PI) * 16.0 * 4.0);
      Vector2 globalPosition = new Vector2(Utility.Lerp(vector2_1.X, vector2_2.X, this._cactusGuyRevealTimer), Utility.Lerp(vector2_1.Y, vector2_2.Y, this._cactusGuyRevealTimer));
      float y = globalPosition.Y;
      if ((double) this._cactusShakeTimer > 0.0)
      {
        globalPosition.X += (float) Game1.random.Next(-1, 2);
        globalPosition.Y += (float) Game1.random.Next(-1, 2);
      }
      this._cactusGuyRevealItem.DrawFurniture(spriteBatch, Game1.GlobalToLocal(Game1.viewport, globalPosition + new Vector2(0.0f, -num2)), 1f, new Vector2(8f, 16f), 4f, y / 10000f);
      spriteBatch.Draw(Game1.shadowTexture, Game1.GlobalToLocal(Game1.viewport, globalPosition), new Microsoft.Xna.Framework.Rectangle?(), Color.White * 0.75f, 0.0f, new Vector2((float) (Game1.shadowTexture.Width / 2), (float) (Game1.shadowTexture.Height / 2)), new Vector2(4f, 4f), SpriteEffects.None, (float) ((double) y / 10000.0 - 1.0000000116860974E-07));
    }
    foreach (Racer localRacer in this._localRacers)
    {
      if (!localRacer.drawAboveMap.Value)
        localRacer.Draw(spriteBatch);
    }
    if (Game1.Date != Game1.player.lastDesertFestivalFishingQuest.Value)
    {
      float num = (float) (4.0 * Math.Round(Math.Sin(Game1.currentGameTime.TotalGameTime.TotalMilliseconds / 250.0), 2));
      spriteBatch.Draw(Game1.mouseCursors, Game1.GlobalToLocal(Game1.viewport, new Vector2(984f, 842f + num)), new Microsoft.Xna.Framework.Rectangle?(new Microsoft.Xna.Framework.Rectangle(395, 497, 3, 8)), Color.White, 0.0f, new Vector2(1f, 4f), 4f + Math.Max(0.0f, (float) (0.25 - (double) num / 16.0)), SpriteEffects.None, 1f);
    }
    if (!this.checkedMineExplanation)
    {
      float num = (float) (4.0 * Math.Round(Math.Sin(Game1.currentGameTime.TotalGameTime.TotalMilliseconds / 250.0), 2));
      spriteBatch.Draw(Game1.mouseCursors, Game1.GlobalToLocal(Game1.viewport, new Vector2(609.6f, 320f + num)), new Microsoft.Xna.Framework.Rectangle?(new Microsoft.Xna.Framework.Rectangle(395, 497, 3, 8)), Color.White, 0.0f, new Vector2(1f, 4f), 4f + Math.Max(0.0f, (float) (0.25 - (double) num / 16.0)), SpriteEffects.None, 1f);
    }
    if (Game1.timeOfDay < 1000)
      spriteBatch.Draw(Game1.mouseCursors_1_6, Game1.GlobalToLocal(new Vector2(45f, 14f) * 64f + new Vector2(7f, 9f) * 4f), new Microsoft.Xna.Framework.Rectangle?(new Microsoft.Xna.Framework.Rectangle(239, 317, 16 /*0x10*/, 17)), Color.White, 0.0f, Vector2.Zero, 4f, SpriteEffects.None, 0.096f);
    base.draw(spriteBatch);
  }

  public override bool checkAction(Location tileLocation, xTile.Dimensions.Rectangle viewport, Farmer who)
  {
    switch (this.getTileIndexAt(tileLocation, "Buildings", "desert-festival"))
    {
      case 792:
      case 793:
        this.playSound("pig");
        return true;
      case 796:
      case 797:
        Utility.TryOpenShopMenu("Traveler", (GameLocation) this);
        return true;
      case 1073:
        this.createQuestionDialogue(Game1.content.LoadString("Strings\\Locations:BeachNightMarket_WarperQuestion"), this.createYesNoResponses(), "WarperQuestion");
        return true;
      default:
        return base.checkAction(tileLocation, viewport, who);
    }
  }

  public override void drawOverlays(SpriteBatch b)
  {
    SpecialCurrencyDisplay.Draw(b, new Vector2(16f, 0.0f), this.eggMoneyDial, Game1.player.Items.CountId("CalicoEgg"), Game1.mouseCursors_1_6, new Microsoft.Xna.Framework.Rectangle(0, 21, 0, 0));
    base.drawOverlays(b);
  }

  public override void drawAboveAlwaysFrontLayer(SpriteBatch sb)
  {
    base.drawAboveAlwaysFrontLayer(sb);
    this._localRacers.Sort((Comparison<Racer>) ((a, b) => a.position.Y.CompareTo(b.position.Y)));
    foreach (Racer localRacer in this._localRacers)
    {
      if (localRacer.drawAboveMap.Value)
        localRacer.Draw(sb);
    }
    if ((double) this._raceTextTimer <= 0.0 || this._raceText == null)
      return;
    Vector2 local = Game1.GlobalToLocal(new Vector2(44.5f, 39.5f) * 64f);
    if (this._raceTextShake)
      local += new Vector2((float) Game1.random.Next(-1, 2), (float) Game1.random.Next(-1, 2));
    float alpha = Utility.Clamp(this._raceTextTimer / 0.25f, 0.0f, 1f);
    SpriteText.drawStringWithScrollCenteredAt(sb, this._raceText, (int) local.X, (int) local.Y - 192 /*0xC0*/, alpha: alpha, scrollType: 1, layerDepth: (float) ((double) local.Y / 10000.0 + 1.0 / 1000.0));
  }

  public Vector3 GetTrackPosition(int track_index, float horizontal_position)
  {
    Vector2 vector2_1 = new Vector2(this.raceTrack[track_index][0].X + 0.5f, this.raceTrack[track_index][0].Y + 0.5f);
    Vector2 vector2_2 = new Vector2(this.raceTrack[track_index][1].X + 0.5f, this.raceTrack[track_index][1].Y + 0.5f);
    int num = vector2_1 == vector2_2 ? 1 : 0;
    Vector2 vector2_3 = vector2_2 - vector2_1;
    vector2_3.Normalize();
    Vector2 vector2_4 = vector2_1 * 64f;
    Vector2 vector2_5 = vector2_2 * 64f;
    Vector2 vector2_6 = vector2_4 - vector2_3 * 64f / 4f;
    Vector2 vector2_7 = vector2_5 + vector2_3 * 64f / 4f;
    return new Vector3(Utility.Lerp(vector2_6.X, vector2_7.X, horizontal_position), Utility.Lerp(vector2_6.Y, vector2_7.Y, horizontal_position), this.raceTrack[track_index][0].Z);
  }

  public override void performTenMinuteUpdate(int timeOfDay)
  {
    string festivalId = nameof (DesertFestival);
    base.performTenMinuteUpdate(timeOfDay);
    if (!Game1.IsMasterGame || !Utility.IsPassiveFestivalOpen(festivalId) || timeOfDay % 200 != 0 || timeOfDay >= 2400 || this.currentRaceState.Value != DesertFestival.RaceState.PreRace)
      return;
    this.announceRaceEvent.Fire("Race_Begin");
    this.currentRaceState.Value = DesertFestival.RaceState.StartingLine;
    if (this.nextRaceGuesses.FieldDict.Count > 0)
      Game1.multiplayer.globalChatInfoMessage("RaceStarting");
    this._raceStateTimer = 5f;
  }

  public virtual void AnnounceRace(string text)
  {
    this._raceTextShake = false;
    this._raceTextTimer = 2f;
    if (text == "Race_Go" || text == "Race_Finish" || text.StartsWith("Racer_"))
      this._raceTextShake = true;
    if (text.StartsWith("Race_Close"))
      this._raceTextTimer = 4f;
    if (text == "RESULT")
    {
      this._raceTextTimer = 4f;
      int num;
      if (!this.raceGuesses.TryGetValue(Game1.player.UniqueMultiplayerID, out num))
        return;
      if (this.lastRaceWinner.Value == num)
        this._raceText = Game1.content.LoadString("Strings\\1_6_Strings:Race_Win");
      else
        this._raceText = Game1.content.LoadString("Strings\\1_6_Strings:Race_Lose");
    }
    else
    {
      this._raceText = Game1.content.LoadString("Strings\\1_6_Strings:" + text);
      if (!text.StartsWith("Racer_"))
        return;
      this._raceText += "!";
    }
  }

  public override void DayUpdate(int dayOfMonth)
  {
    base.DayUpdate(dayOfMonth);
    Game1.player.team.calicoEggSkullCavernRating.Value = 0;
    Game1.player.team.highestCalicoEggRatingToday.Value = 0;
    Game1.player.team.calicoStatueEffects.Clear();
    MineShaft.totalCalicoStatuesActivatedToday = 0;
    this.finishedRacers.Clear();
    this.lastRaceWinner.Value = -1;
    this.rewardsToCollect.Clear();
    this.specialRewardsCollected.Clear();
    this.raceGuesses.Clear();
    this.nextRaceGuesses.Clear();
    this.sabotages.Clear();
    this.currentRaceState.Value = DesertFestival.RaceState.PreRace;
    this._raceStateTimer = 0.0f;
    this._currentScholarQuestion = -1;
  }

  public override void cleanupBeforePlayerExit()
  {
    this._localRacers.Clear();
    this._cactusGuyRevealTimer = -1f;
    this._cactusGuyRevealItem = (RandomizedPlantFurniture) null;
    base.cleanupBeforePlayerExit();
  }

  protected override void resetLocalState()
  {
    base.resetLocalState();
    if (Game1.player.mailReceived.Contains("Checked_DF_Mine_Explanation"))
      this.checkedMineExplanation = true;
    this._localRacers.Clear();
    this._localRacers.AddRange((IEnumerable<Racer>) this.netRacers);
    if (this.critters == null)
      this.critters = new List<Critter>();
    for (int index = 0; index < 8; ++index)
      this.critters.Add((Critter) new Butterfly((GameLocation) this, this.getRandomTile(), forceSummerButterfly: true));
    this.eggMoneyDial = new MoneyDial(4, false);
    this.eggMoneyDial.currentValue = Game1.player.Items.CountId("CalicoEgg");
  }

  public static void SetupFestivalDay()
  {
    string festival_id = nameof (DesertFestival);
    int day_number = Utility.GetDayOfPassiveFestival(festival_id);
    Dictionary<string, ShopData> store_data_sheet = DataLoader.Shops(Game1.content);
    List<NPC> allVillagers = Utility.getAllVillagers();
    allVillagers.RemoveAll((Predicate<NPC>) (character => !store_data_sheet.ContainsKey($"{festival_id}_{character.Name}") || character.Name == "Leo" && !Game1.MasterPlayer.mailReceived.Contains("leoMoved") || character.getMasterScheduleRawData().ContainsKey($"{festival_id}_{day_number.ToString()}")));
    Random daySaveRandom = Utility.CreateDaySaveRandom();
    for (int index1 = 0; index1 < day_number - 1; ++index1)
    {
      for (int index2 = 0; index2 < 2; ++index2)
      {
        NPC npc = daySaveRandom.ChooseFrom<NPC>((IList<NPC>) allVillagers);
        allVillagers.Remove(npc);
        if (allVillagers.Count == 0)
          break;
      }
    }
    if (allVillagers.Count > 0)
    {
      NPC character = daySaveRandom.ChooseFrom<NPC>((IList<NPC>) allVillagers);
      allVillagers.Remove(character);
      DesertFestival.SetupMerchantSchedule(character, 0);
    }
    if (allVillagers.Count > 0)
    {
      NPC character = daySaveRandom.ChooseFrom<NPC>((IList<NPC>) allVillagers);
      allVillagers.Remove(character);
      DesertFestival.SetupMerchantSchedule(character, 1);
    }
    if (Game1.getLocationFromName(nameof (DesertFestival)) is DesertFestival locationFromName)
    {
      locationFromName.netRacers.Clear();
      List<int> options = new List<int>();
      for (int index = 0; index < locationFromName.totalRacers; ++index)
        options.Add(index);
      for (int index3 = 0; index3 < locationFromName.racerCount; ++index3)
      {
        int index4 = daySaveRandom.ChooseFrom<int>((IList<int>) options);
        options.Remove(index4);
        Racer racer = new Racer(index4);
        racer.position.Value = new Vector2(44.5f, 37.5f - (float) index3) * 64f;
        racer.segmentStart = racer.position.Value;
        racer.segmentEnd = racer.position.Value;
        locationFromName.netRacers.Add(racer);
      }
    }
    SpecialOrder.UpdateAvailableSpecialOrders("DesertFestivalMarlon", true);
  }

  public enum RaceState
  {
    PreRace,
    StartingLine,
    Ready,
    Set,
    Go,
    AnnounceWinner,
    AnnounceWinner2,
    AnnounceWinner3,
    AnnounceWinner4,
    RaceEnd,
    RacesOver,
  }
}
