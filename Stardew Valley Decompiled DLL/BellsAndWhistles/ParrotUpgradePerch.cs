// Decompiled with JetBrains decompiler
// Type: StardewValley.BellsAndWhistles.ParrotUpgradePerch
// Assembly: Stardew Valley, Version=1.6.15.24356, Culture=neutral, PublicKeyToken=null
// MVID: AA1F513A-94F0-4EF6-A35F-2D5B4A3721BF
// Assembly location: D:\SteamLibrary\steamapps\common\Stardew Valley\Stardew Valley.dll
// XML documentation location: D:\SteamLibrary\steamapps\common\Stardew Valley\Stardew Valley.xml

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Netcode;
using StardewValley.Extensions;
using StardewValley.Locations;
using StardewValley.Network;
using StardewValley.TerrainFeatures;
using System;
using System.Collections.Generic;
using xTile.Dimensions;

#nullable disable
namespace StardewValley.BellsAndWhistles;

public class ParrotUpgradePerch : INetObject<NetFields>
{
  public const string GoldenParrotMailKey = "activateGoldenParrotsTonight";
  public NetEvent0 animationEvent = new NetEvent0();
  public NetMutex upgradeMutex = new NetMutex();
  public NetPoint tilePosition = new NetPoint();
  public Texture2D texture;
  public NetRectangle upgradeRect = new NetRectangle();
  public List<ParrotUpgradePerch.Parrot> parrots = new List<ParrotUpgradePerch.Parrot>();
  public NetEvent0 upgradeCompleteEvent = new NetEvent0();
  public NetEnum<ParrotUpgradePerch.UpgradeState> currentState = new NetEnum<ParrotUpgradePerch.UpgradeState>(ParrotUpgradePerch.UpgradeState.Idle);
  public float stateTimer;
  public NetInt requiredNuts = new NetInt(0);
  public float squawkTime;
  public float timeUntilChomp;
  public float timeUntilSqwawk;
  public float shakeTime;
  public float costShakeTime;
  public const int PARROT_COUNT = 24;
  public bool parrotPresent = true;
  public bool isPlayerNearby;
  public NetString upgradeName = new NetString("");
  public NetString requiredMail = new NetString("");
  public float nextParrotSpawn;
  public NetLocationRef locationRef = new NetLocationRef();
  public Action onApplyUpgrade;
  public Func<bool> onUpdateCompletionStatus;
  protected bool _cachedAvailablity;

  public NetFields NetFields { get; } = new NetFields(nameof (ParrotUpgradePerch));

  public ParrotUpgradePerch()
  {
    this.InitNetFields();
    this.texture = Game1.content.Load<Texture2D>("LooseSprites\\parrots");
  }

  public virtual void UpdateCompletionStatus()
  {
    if (this.onUpdateCompletionStatus == null || !this.onUpdateCompletionStatus())
      return;
    this.currentState.Value = ParrotUpgradePerch.UpgradeState.Complete;
  }

  public virtual void InitNetFields()
  {
    this.NetFields.SetOwner((INetObject<NetFields>) this).AddField((INetSerializable) this.tilePosition, "tilePosition").AddField((INetSerializable) this.upgradeRect, "upgradeRect").AddField((INetSerializable) this.currentState, "currentState").AddField((INetSerializable) this.upgradeMutex.NetFields, "upgradeMutex.NetFields").AddField(this.animationEvent.NetFields, "animationEvent.NetFields").AddField(this.upgradeCompleteEvent.NetFields, "upgradeCompleteEvent.NetFields").AddField((INetSerializable) this.locationRef.NetFields, "locationRef.NetFields").AddField((INetSerializable) this.requiredNuts, "requiredNuts").AddField((INetSerializable) this.upgradeName, "upgradeName").AddField((INetSerializable) this.requiredMail, "requiredMail");
    this.animationEvent.onEvent += new NetEvent0.Event(this.PerformAnimation);
    this.upgradeCompleteEvent.onEvent += new NetEvent0.Event(this.PerformCompleteAnimation);
  }

  public virtual void PerformCompleteAnimation()
  {
    if (this.upgradeName.Value.Contains("Volcano"))
    {
      for (int index = 0; index < 16 /*0x10*/; ++index)
      {
        this.locationRef.Value.temporarySprites.Add(new TemporaryAnimatedSprite(5, Utility.getRandomPositionInThisRectangle(this.upgradeRect.Value, Game1.random) * 64f, Color.White)
        {
          motion = new Vector2((float) Game1.random.Next(-1, 2), -1f),
          scale = 1f,
          layerDepth = 1f,
          drawAboveAlwaysFront = true,
          delayBeforeAnimationStart = index * 15
        });
        TemporaryAnimatedSprite temporaryAnimatedSprite1 = new TemporaryAnimatedSprite("LooseSprites\\Cursors2", new Microsoft.Xna.Framework.Rectangle(65, 229, 16 /*0x10*/, 6), Utility.getRandomPositionInThisRectangle(this.upgradeRect.Value, Game1.random) * 64f, Game1.random.NextBool(), 0.0f, Color.White)
        {
          motion = new Vector2((float) Game1.random.Next(-2, 3), -16f),
          acceleration = new Vector2(0.0f, 0.5f),
          rotationChange = (float) Game1.random.Next(-4, 5) * 0.05f,
          scale = 4f,
          animationLength = 1,
          totalNumberOfLoops = 1,
          interval = (float) (1000 + Game1.random.Next(500)),
          layerDepth = 1f,
          drawAboveAlwaysFront = true,
          yStopCoordinate = (this.upgradeRect.Bottom + 1) * 64 /*0x40*/
        };
        temporaryAnimatedSprite1.reachedStopCoordinate = new TemporaryAnimatedSprite.endBehavior(temporaryAnimatedSprite1.bounce);
        this.locationRef.Value.TemporarySprites.Add(temporaryAnimatedSprite1);
        TemporaryAnimatedSprite temporaryAnimatedSprite2 = new TemporaryAnimatedSprite("LooseSprites\\Cursors2", new Microsoft.Xna.Framework.Rectangle(65, 229, 16 /*0x10*/, 6), Utility.getRandomPositionInThisRectangle(this.upgradeRect.Value, Game1.random) * 64f, Game1.random.NextBool(), 0.0f, Color.White)
        {
          motion = new Vector2((float) Game1.random.Next(-2, 3), -16f),
          acceleration = new Vector2(0.0f, 0.5f),
          rotationChange = (float) Game1.random.Next(-4, 5) * 0.05f,
          scale = 4f,
          animationLength = 1,
          totalNumberOfLoops = 1,
          interval = (float) (1000 + Game1.random.Next(500)),
          layerDepth = 1f,
          drawAboveAlwaysFront = true,
          yStopCoordinate = (this.upgradeRect.Bottom + 1) * 64 /*0x40*/
        };
        temporaryAnimatedSprite2.reachedStopCoordinate = new TemporaryAnimatedSprite.endBehavior(temporaryAnimatedSprite2.bounce);
        this.locationRef.Value.TemporarySprites.Add(temporaryAnimatedSprite2);
      }
      if (this.locationRef.Value == Game1.currentLocation)
      {
        Game1.flashAlpha = 1f;
        Game1.playSound("boulderBreak");
      }
    }
    else
    {
      switch (this.upgradeName.Value)
      {
        case "House":
          for (int index = 0; index < 16 /*0x10*/; ++index)
          {
            this.locationRef.Value.temporarySprites.Add(new TemporaryAnimatedSprite(5, Utility.getRandomPositionInThisRectangle(this.upgradeRect.Value, Game1.random) * 64f, Color.White)
            {
              motion = new Vector2((float) Game1.random.Next(-1, 2), -1f),
              scale = 1f,
              layerDepth = 1f,
              drawAboveAlwaysFront = true,
              delayBeforeAnimationStart = index * 15
            });
            TemporaryAnimatedSprite temporaryAnimatedSprite3 = new TemporaryAnimatedSprite("LooseSprites\\Cursors2", new Microsoft.Xna.Framework.Rectangle(49 + 16 /*0x10*/ * Game1.random.Next(3), 229, 16 /*0x10*/, 6), Utility.getRandomPositionInThisRectangle(this.upgradeRect.Value, Game1.random) * 64f, Game1.random.NextBool(), 0.0f, Color.White)
            {
              motion = new Vector2((float) Game1.random.Next(-2, 3), -16f),
              acceleration = new Vector2(0.0f, 0.5f),
              rotationChange = (float) Game1.random.Next(-4, 5) * 0.05f,
              scale = 4f,
              animationLength = 1,
              totalNumberOfLoops = 1,
              interval = (float) (1000 + Game1.random.Next(500)),
              layerDepth = 1f,
              drawAboveAlwaysFront = true,
              yStopCoordinate = (this.upgradeRect.Bottom + 1) * 64 /*0x40*/
            };
            temporaryAnimatedSprite3.reachedStopCoordinate = new TemporaryAnimatedSprite.endBehavior(temporaryAnimatedSprite3.bounce);
            this.locationRef.Value.TemporarySprites.Add(temporaryAnimatedSprite3);
            TemporaryAnimatedSprite temporaryAnimatedSprite4 = new TemporaryAnimatedSprite("LooseSprites\\Cursors2", new Microsoft.Xna.Framework.Rectangle(49 + 16 /*0x10*/ * Game1.random.Next(3), 229, 16 /*0x10*/, 6), Utility.getRandomPositionInThisRectangle(this.upgradeRect.Value, Game1.random) * 64f, Game1.random.NextBool(), 0.0f, Color.White)
            {
              motion = new Vector2((float) Game1.random.Next(-2, 3), -16f),
              acceleration = new Vector2(0.0f, 0.5f),
              rotationChange = (float) Game1.random.Next(-4, 5) * 0.05f,
              scale = 4f,
              animationLength = 1,
              totalNumberOfLoops = 1,
              interval = (float) (1000 + Game1.random.Next(500)),
              layerDepth = 1f,
              drawAboveAlwaysFront = true,
              yStopCoordinate = (this.upgradeRect.Bottom + 1) * 64 /*0x40*/
            };
            temporaryAnimatedSprite4.reachedStopCoordinate = new TemporaryAnimatedSprite.endBehavior(temporaryAnimatedSprite4.bounce);
            this.locationRef.Value.TemporarySprites.Add(temporaryAnimatedSprite4);
          }
          if (this.locationRef.Value == Game1.currentLocation)
          {
            Game1.flashAlpha = 1f;
            Game1.playSound("boulderBreak");
            break;
          }
          break;
        case "Resort":
        case "Trader":
        case "Obelisk":
        case "House_Mailbox":
          if (this.locationRef.Value == Game1.currentLocation)
          {
            Game1.flashAlpha = 1f;
            break;
          }
          break;
      }
    }
    if (this.locationRef.Value == Game1.currentLocation && this.upgradeName.Value != "Hut")
      DelayedAction.playSoundAfterDelay("secret1", 800);
    if (this.locationRef.Value == null || !(this.locationRef.Value is IslandLocation islandLocation))
      return;
    foreach (ParrotUpgradePerch parrotUpgradePerch in islandLocation.parrotUpgradePerches)
      parrotUpgradePerch.resetCache();
  }

  public ParrotUpgradePerch(
    GameLocation location,
    Point tile_position,
    Microsoft.Xna.Framework.Rectangle upgrade_rectangle,
    int required_nuts,
    Action apply_upgrade,
    Func<bool> update_completion_status,
    string upgrade_name = "",
    string required_mail = "")
    : this()
  {
    this.locationRef.Value = location;
    this.tilePosition.Value = tile_position;
    this.upgradeRect.Value = upgrade_rectangle;
    this.onApplyUpgrade = apply_upgrade;
    this.onUpdateCompletionStatus = update_completion_status;
    this.requiredNuts.Value = required_nuts;
    this.parrots = new List<ParrotUpgradePerch.Parrot>();
    this.UpdateCompletionStatus();
    this.upgradeName.Value = upgrade_name;
    if (!(required_mail != ""))
      return;
    this.requiredMail.Value = required_mail;
  }

  public bool IsAtTile(int x, int y)
  {
    return this.tilePosition.X == x && this.tilePosition.Y == y && this.currentState.Value == ParrotUpgradePerch.UpgradeState.Idle;
  }

  public virtual void PerformAnimation()
  {
    this.currentState.Value = ParrotUpgradePerch.UpgradeState.StartBuilding;
    this.stateTimer = 3f;
    if (Game1.currentLocation != this.locationRef.Value)
      return;
    Game1.playSound("parrot_squawk");
    this.parrots.Clear();
    this.parrotPresent = true;
    Game1.currentLocation.temporarySprites.Add(new TemporaryAnimatedSprite("Maps\\springobjects", Game1.getSourceRectForStandardTileSheet(Game1.objectSpriteSheet, 73, 16 /*0x10*/, 16 /*0x10*/), 2000f, 1, 0, (Utility.PointToVector2(this.tilePosition.Value) + new Vector2(0.25f, -2.5f)) * 64f, false, false, (float) (this.tilePosition.Y * 64 /*0x40*/ + 1) / 10000f, 0.0f, Color.White, 3f, -0.015f, 0.0f, 0.0f)
    {
      motion = new Vector2(-0.1f, -7f),
      acceleration = new Vector2(0.0f, 0.25f),
      id = 98765,
      drawAboveAlwaysFront = true
    });
    Game1.playSound("dwop");
    if (this.upgradeMutex.IsLockHeld())
      Game1.player.freezePause = this.upgradeName.Value != "Hut" ? 10000 : 3000;
    this.timeUntilChomp = 1f;
    this.squawkTime = 1f;
  }

  public virtual bool IsAvailable(bool use_cached_value = false)
  {
    if (use_cached_value && Game1.currentLocation == this.locationRef.Value)
      return this._cachedAvailablity;
    if (this.requiredMail.Value == "")
      return true;
    foreach (string str in this.requiredMail.Value.Split(','))
    {
      if (!Game1.MasterPlayer.hasOrWillReceiveMail(str.Trim()))
        return false;
    }
    return true;
  }

  public virtual void StartAnimation() => this.animationEvent.Fire();

  public bool CheckAction(Location tile_location, Farmer farmer)
  {
    if (!this.IsAtTile(tile_location.X, tile_location.Y) || !this.IsAvailable())
      return false;
    string format;
    if (this.upgradeName.Value == "GoldenParrot")
    {
      if (Game1.netWorldState.Value.GoldenWalnutsFound >= 130)
      {
        this.squawkTime = 0.5f;
        this.shakeTime = 0.5f;
        Game1.playSound("parrot_squawk");
        return true;
      }
      format = Game1.content.LoadStringReturnNullIfNotFound("Strings\\1_6_Strings:GoldenParrot");
    }
    else
      format = Game1.content.LoadStringReturnNullIfNotFound("Strings\\UI:UpgradePerch_" + this.upgradeName.Value);
    GameLocation gameLocation = this.locationRef.Value;
    if (format != null && gameLocation != null)
    {
      string str = string.Format(format, (object) this.requiredNuts.Value);
      this.costShakeTime = 0.5f;
      this.squawkTime = 0.5f;
      this.shakeTime = 0.5f;
      if (this.locationRef.Value == Game1.currentLocation)
        Game1.playSound("parrot_squawk");
      if (this.upgradeName.Value == "GoldenParrot")
      {
        if (Game1.MasterPlayer.hasOrWillReceiveMail("activateGoldenParrotsTonight"))
        {
          Game1.playSound("parrot_squawk");
          Game1.drawObjectDialogue(Game1.content.LoadString("Strings\\1_6_Strings:GoldenParrot_Tonight"));
          return true;
        }
        int number = (130 - Game1.netWorldState.Value.GoldenWalnutsFound) * 10000;
        gameLocation.createQuestionDialogue(str, new Response[2]
        {
          new Response("Yes", string.Format(Game1.content.LoadString("Strings\\1_6_Strings:GoldenParrot_Yes"), (object) Utility.getNumberWithCommas(number))).SetHotKey(Keys.Y),
          new Response("No", Game1.content.LoadString("Strings\\Lexicon:QuestionDialogue_No")).SetHotKey(Keys.Escape)
        }, "GoldenParrot");
      }
      else if (Game1.netWorldState.Value.GoldenWalnuts >= this.requiredNuts.Value)
        gameLocation.createQuestionDialogue(str, gameLocation.createYesNoResponses(), "UpgradePerch_" + this.upgradeName.Value);
      else
        Game1.drawDialogueNoTyping(str);
    }
    else if (Game1.netWorldState.Value.GoldenWalnuts >= this.requiredNuts.Value)
      this.AttemptConstruction();
    else
      this.ShowInsufficientNuts();
    return true;
  }

  public virtual bool AnswerQuestion(Response answer)
  {
    if (Game1.currentLocation.lastQuestionKey != null)
    {
      string str = $"{ArgUtility.SplitBySpace(Game1.currentLocation.lastQuestionKey)[0]}_{answer.responseKey}";
      if (str == $"UpgradePerch_{this.upgradeName.Value}_Yes")
      {
        this.AttemptConstruction();
        return true;
      }
      switch (str)
      {
        case "GoldenParrot_Yes":
          int number = (130 - Game1.netWorldState.Value.GoldenWalnutsFound) * 10000;
          if (Game1.player.Money < number)
          {
            Game1.drawObjectDialogue(Game1.content.LoadString("Strings\\UI:NotEnoughMoney3"));
            break;
          }
          this.locationRef.Value.createQuestionDialogue(Game1.content.LoadString("Strings\\1_6_Strings:GoldenParrot_Sure"), new Response[2]
          {
            new Response("Yes", string.Format(Game1.content.LoadString("Strings\\1_6_Strings:GoldenParrot_Yes"), (object) Utility.getNumberWithCommas(number))).SetHotKey(Keys.Y),
            new Response("No", Game1.content.LoadString("Strings\\Lexicon:QuestionDialogue_No")).SetHotKey(Keys.Escape)
          }, "GoldenParrotConfirm");
          return true;
        case "GoldenParrotConfirm_Yes":
          int num = (130 - Game1.netWorldState.Value.GoldenWalnutsFound) * 10000;
          if (Game1.player.Money < num)
          {
            Game1.drawObjectDialogue(Game1.content.LoadString("Strings\\UI:NotEnoughMoney3"));
          }
          else
          {
            Game1.player.Money -= num;
            Game1.multiplayer.broadcastPartyWideMail("activateGoldenParrotsTonight", no_letter: true);
            Game1.drawObjectDialogue(Game1.content.LoadString("Strings\\1_6_Strings:GoldenParrot_Tonight"));
            Game1.playSound("parrot_squawk");
          }
          return true;
      }
    }
    return false;
  }

  public static void ActivateGoldenParrot()
  {
    string[] strArray = new string[67]
    {
      "Bush_IslandNorth_13_33",
      "Bush_IslandNorth_5_30",
      "Buried_IslandNorth_19_39",
      "Bush_IslandNorth_4_42",
      "Bush_IslandNorth_45_38",
      "Bush_IslandNorth_47_40",
      "IslandLeftPlantRestored",
      "IslandRightPlantRestored",
      "IslandBatRestored",
      "IslandFrogRestored",
      "IslandCenterSkeletonRestored",
      "IslandSnakeRestored",
      "Buried_IslandNorth_19_13",
      "Buried_IslandNorth_57_79",
      "Buried_IslandNorth_54_21",
      "Buried_IslandNorth_42_77",
      "Buried_IslandNorth_62_54",
      "Buried_IslandNorth_26_81",
      "Bush_IslandNorth_20_26",
      "Bush_IslandNorth_9_84",
      "Bush_IslandNorth_56_27",
      "Bush_IslandSouth_31_5",
      "TreeNut",
      "IslandWestCavePuzzle",
      "SandDuggy",
      "TigerSlimeNut",
      "Buried_IslandWest_21_81",
      "Buried_IslandWest_62_76",
      "Buried_IslandWest_39_24",
      "Buried_IslandWest_88_14",
      "Buried_IslandWest_43_74",
      "Buried_IslandWest_30_75",
      "MusselStone",
      "IslandFarming",
      "Bush_IslandWest_104_3",
      "Bush_IslandWest_31_24",
      "Bush_IslandWest_38_56",
      "Bush_IslandWest_75_29",
      "Bush_IslandWest_64_30",
      "Bush_IslandWest_54_18",
      "Bush_IslandWest_25_30",
      "Bush_IslandWest_15_3",
      "IslandFishing",
      "VolcanoNormalChest",
      "VolcanoRareChest",
      "VolcanoBarrel",
      "VolcanoMining",
      "VolcanoMonsterDrop",
      "Island_N_BuriedTreasureNut",
      "Island_W_BuriedTreasureNut",
      "Island_W_BuriedTreasureNut2",
      "Mermaid",
      "TreeNutShot",
      "Buried_IslandSouthEastCave_36_26",
      "Buried_IslandSouthEast_25_17",
      "StardropPool",
      "Bush_Caldera_28_36",
      "Bush_Caldera_9_34",
      "Bush_CaptainRoom_2_4",
      "BananaShrine",
      "Bush_IslandEast_17_37",
      "Darts",
      "IslandGourmand1",
      "IslandGourmand2",
      "IslandGourmand3",
      "IslandShrinePuzzle",
      "Bush_IslandShrine_23_34"
    };
    foreach (string key in strArray)
    {
      Game1.player.team.limitedNutDrops[key] = 9999;
      Game1.player.team.collectedNutTracker.Add(key);
      Game1.netWorldState.Value.FoundBuriedNuts.Add(key.Replace("Buried_", ""));
    }
    int num = 130 - Game1.netWorldState.Value.GoldenWalnutsFound;
    Game1.netWorldState.Value.GoldenWalnutsFound = 130;
    Game1.netWorldState.Value.GoldenWalnuts += num;
    Game1.netWorldState.Value.GoldenCoconutCracked = true;
    Game1.netWorldState.Value.ActivatedGoldenParrot = true;
    foreach (GameLocation location in (IEnumerable<GameLocation>) Game1.locations)
    {
      if (location is IslandLocation)
      {
        foreach (LargeTerrainFeature largeTerrainFeature in location.largeTerrainFeatures)
        {
          if (largeTerrainFeature is Bush bush)
          {
            bush.tileSheetOffset.Value = 0;
            bush.setUpSourceRect();
          }
        }
      }
      if (!(location is IslandNorth islandNorth))
      {
        if (!(location is IslandWestCave1 islandWestCave1))
        {
          if (!(location is IslandEast islandEast))
          {
            if (!(location is IslandHut islandHut))
            {
              if (!(location is IslandSouthEast islandSouthEast))
              {
                if (!(location is IslandShrine islandShrine))
                {
                  if (!(location is IslandWest islandWest))
                  {
                    if (location is IslandFarmCave islandFarmCave)
                      islandFarmCave.gourmandRequestsFulfilled.Value = 3;
                  }
                  else
                    islandWest.sandDuggy.Value.whacked.Value = true;
                }
                else
                  islandShrine.puzzleFinished.Value = true;
              }
              else
              {
                islandSouthEast.mermaidPuzzleFinished.Value = true;
                islandSouthEast.fishedWalnut.Value = true;
              }
            }
            else
              islandHut.treeNutObtained.Value = true;
          }
          else
            islandEast.bananaShrineComplete.Value = true;
        }
        else
          islandWestCave1.completed.Value = true;
      }
      else
      {
        islandNorth.treeNutShot.Value = true;
        foreach (ParrotUpgradePerch parrotUpgradePerch in islandNorth.parrotUpgradePerches)
        {
          if (parrotUpgradePerch.upgradeName.Value == "GoldenParrot")
            parrotUpgradePerch.currentState.Value = ParrotUpgradePerch.UpgradeState.Complete;
        }
      }
    }
  }

  public virtual void AttemptConstruction()
  {
    Game1.player.Halt();
    Game1.player.canMove = false;
    this.upgradeMutex.RequestLock((Action) (() =>
    {
      Game1.player.canMove = true;
      if (Game1.netWorldState.Value.GoldenWalnuts >= this.requiredNuts.Value)
      {
        if (Game1.content.LoadStringReturnNullIfNotFound("Strings\\UI:Chat_UpgradePerch_" + this.upgradeName.Value) != null)
          Game1.multiplayer.globalChatInfoMessage("UpgradePerch_" + this.upgradeName.Value, Game1.player.Name);
        Game1.netWorldState.Value.GoldenWalnuts -= this.requiredNuts.Value;
        this.StartAnimation();
      }
      else
        this.ShowInsufficientNuts();
    }), (Action) (() => Game1.player.canMove = true));
  }

  public virtual void ShowInsufficientNuts()
  {
    if (!this.IsAvailable(true))
      return;
    this.costShakeTime = 0.5f;
    this.squawkTime = 0.5f;
    this.shakeTime = 0.5f;
    if (this.locationRef.Value != Game1.currentLocation)
      return;
    Game1.playSound("parrot_squawk");
  }

  public virtual void ApplyUpgrade()
  {
    Action onApplyUpgrade = this.onApplyUpgrade;
    if (onApplyUpgrade == null)
      return;
    onApplyUpgrade();
  }

  public virtual void Cleanup()
  {
    if (this.upgradeMutex.IsLockHeld())
      this.upgradeMutex.ReleaseLock();
    if (!this.isPlayerNearby)
      return;
    this.isPlayerNearby = false;
  }

  public virtual void ResetForPlayerEntry() => this.resetCache();

  public void resetCache()
  {
    this._cachedAvailablity = this.IsAvailable();
    this.parrotPresent = this.currentState.Value == ParrotUpgradePerch.UpgradeState.Idle;
  }

  public virtual void UpdateEvenIfFarmerIsntHere(GameTime time)
  {
    this.animationEvent.Poll();
    this.upgradeCompleteEvent.Poll();
    this.upgradeMutex.Update(this.locationRef.Value);
    if (!Game1.IsMasterGame)
      return;
    if ((double) this.stateTimer > 0.0)
      this.stateTimer -= (float) time.ElapsedGameTime.TotalSeconds;
    if (this.currentState.Value == ParrotUpgradePerch.UpgradeState.StartBuilding && (double) this.stateTimer <= 0.0)
    {
      this.currentState.Value = ParrotUpgradePerch.UpgradeState.Building;
      this.stateTimer = 5f;
      if (this.upgradeName.Value == "Hut")
        this.stateTimer = 0.1f;
    }
    if (this.currentState.Value != ParrotUpgradePerch.UpgradeState.Building || (double) this.stateTimer > 0.0)
      return;
    this.ApplyUpgrade();
    this.currentState.Value = ParrotUpgradePerch.UpgradeState.Complete;
    this.upgradeMutex.ReleaseLock();
    this.upgradeCompleteEvent.Fire();
  }

  public virtual void Update(GameTime time)
  {
    TimeSpan elapsedGameTime;
    if ((double) this.squawkTime > 0.0)
    {
      double squawkTime = (double) this.squawkTime;
      elapsedGameTime = time.ElapsedGameTime;
      double totalSeconds = elapsedGameTime.TotalSeconds;
      this.squawkTime = (float) (squawkTime - totalSeconds);
    }
    if ((double) this.shakeTime > 0.0)
    {
      double shakeTime = (double) this.shakeTime;
      elapsedGameTime = time.ElapsedGameTime;
      double totalSeconds = elapsedGameTime.TotalSeconds;
      this.shakeTime = (float) (shakeTime - totalSeconds);
    }
    if ((double) this.costShakeTime > 0.0)
    {
      double costShakeTime = (double) this.costShakeTime;
      elapsedGameTime = time.ElapsedGameTime;
      double totalSeconds = elapsedGameTime.TotalSeconds;
      this.costShakeTime = (float) (costShakeTime - totalSeconds);
    }
    if ((double) this.timeUntilChomp > 0.0)
    {
      double timeUntilChomp = (double) this.timeUntilChomp;
      elapsedGameTime = time.ElapsedGameTime;
      double totalSeconds = elapsedGameTime.TotalSeconds;
      this.timeUntilChomp = (float) (timeUntilChomp - totalSeconds);
      if ((double) this.timeUntilChomp <= 0.0)
      {
        if (this.locationRef.Value == Game1.currentLocation)
          Game1.playSound("eat");
        this.timeUntilChomp = 0.0f;
        this.shakeTime = 0.25f;
        if (this.locationRef.Value.getTemporarySpriteByID(98765) != null)
        {
          for (int index = 0; index < 6; ++index)
            this.locationRef.Value.temporarySprites.Add(new TemporaryAnimatedSprite("LooseSprites\\Cursors2", new Microsoft.Xna.Framework.Rectangle(9, 252, 3, 3), this.locationRef.Value.getTemporarySpriteByID(98765).position + new Vector2(8f, 8f) * 4f, Game1.random.NextBool(), 0.0f, Color.White)
            {
              motion = new Vector2((float) Game1.random.Next(-1, 2), -6f),
              acceleration = new Vector2(0.0f, 0.25f),
              rotationChange = (float) Game1.random.Next(-4, 5) * 0.05f,
              scale = 4f,
              animationLength = 1,
              totalNumberOfLoops = 1,
              interval = (float) (500 + Game1.random.Next(500)),
              layerDepth = 1f,
              drawAboveAlwaysFront = true
            });
        }
        this.locationRef.Value.removeTemporarySpritesWithID(98765);
        this.timeUntilSqwawk = 1f;
      }
    }
    if ((double) this.timeUntilSqwawk > 0.0)
    {
      double timeUntilSqwawk = (double) this.timeUntilSqwawk;
      elapsedGameTime = time.ElapsedGameTime;
      double totalSeconds = elapsedGameTime.TotalSeconds;
      this.timeUntilSqwawk = (float) (timeUntilSqwawk - totalSeconds);
      if ((double) this.timeUntilSqwawk <= 0.0)
      {
        this.timeUntilSqwawk = 0.0f;
        if (this.locationRef.Value == Game1.currentLocation)
          Game1.playSound("parrot_squawk");
        this.squawkTime = 0.5f;
        this.shakeTime = 0.5f;
      }
    }
    if (this.parrotPresent && this.currentState.Value > ParrotUpgradePerch.UpgradeState.StartBuilding)
    {
      if (this.currentState.Value == ParrotUpgradePerch.UpgradeState.Building)
        this.parrots.Add(new ParrotUpgradePerch.Parrot(this, Utility.PointToVector2(this.tilePosition.Value))
        {
          isPerchedParrot = true
        });
      this.parrotPresent = false;
    }
    if (this.IsAvailable(true))
    {
      bool flag = this.currentState.Value == ParrotUpgradePerch.UpgradeState.Idle && !this.upgradeMutex.IsLocked() && Math.Abs(Game1.player.TilePoint.X - this.tilePosition.X) <= 1 && Math.Abs(Game1.player.TilePoint.Y - this.tilePosition.Y) <= 1;
      if (flag != this.isPlayerNearby)
      {
        this.isPlayerNearby = flag;
        if (this.isPlayerNearby)
        {
          if (this.locationRef.Value == Game1.currentLocation)
            Game1.playSound("parrot_squawk");
          this.squawkTime = 0.5f;
          this.shakeTime = 0.5f;
          this.costShakeTime = 0.5f;
          Game1.specialCurrencyDisplay.ShowCurrency("walnuts", (Func<bool>) (() => this.isPlayerNearby), 0.0f);
        }
      }
    }
    if (this.currentState.Value == ParrotUpgradePerch.UpgradeState.Building && this.parrots.Count < 24)
    {
      if ((double) this.nextParrotSpawn > 0.0)
      {
        double nextParrotSpawn = (double) this.nextParrotSpawn;
        elapsedGameTime = time.ElapsedGameTime;
        double totalSeconds = elapsedGameTime.TotalSeconds;
        this.nextParrotSpawn = (float) (nextParrotSpawn - totalSeconds);
      }
      if ((double) this.nextParrotSpawn <= 0.0)
      {
        this.nextParrotSpawn = 0.05f;
        Microsoft.Xna.Framework.Rectangle r = this.upgradeRect.Value;
        r.Inflate(5, 0);
        this.parrots.Add(new ParrotUpgradePerch.Parrot(this, Utility.getRandomPositionInThisRectangle(r, Game1.random), this.parrots.Count % 10 == 0));
      }
    }
    this.parrots.RemoveAll((Predicate<ParrotUpgradePerch.Parrot>) (parrot => parrot.Update(time)));
  }

  public virtual void Draw(SpriteBatch b)
  {
    if (!this.IsAvailable(true) || !this.parrotPresent && !(this.upgradeName.Value == "Hut"))
      return;
    int num = 0;
    Vector2 zero = Vector2.Zero;
    if ((double) this.squawkTime > 0.0)
      num = 1;
    if ((double) this.shakeTime > 0.0)
    {
      zero.X = Utility.RandomFloat(-0.5f, 0.5f) * 4f;
      zero.Y = Utility.RandomFloat(-0.5f, 0.5f) * 4f;
    }
    b.Draw(this.texture, Game1.GlobalToLocal(Game1.viewport, (Utility.PointToVector2(this.tilePosition.Value) + new Vector2(0.5f, -1f)) * 64f) + zero, new Microsoft.Xna.Framework.Rectangle?(new Microsoft.Xna.Framework.Rectangle(num * 24, this.upgradeName.Value == "GoldenParrot" ? 96 /*0x60*/ : 0, 24, 24)), Color.White, 0.0f, new Vector2(12f, 16f), 4f, SpriteEffects.None, (float) ((((double) this.tilePosition.Y + 1.0) * 64.0 - 1.0) / 10000.0));
  }

  public virtual void DrawAboveAlwaysFrontLayer(SpriteBatch b)
  {
    if (this.parrotPresent && this.IsAvailable(true) && this.isPlayerNearby)
    {
      if (this.upgradeName.Value == "GoldenParrot" && Game1.MasterPlayer.hasOrWillReceiveMail("activateGoldenParrotsTonight"))
        return;
      Vector2 zero = Vector2.Zero;
      if ((double) this.costShakeTime > 0.0)
      {
        zero.X = Utility.RandomFloat(-0.5f, 0.5f) * 4f;
        zero.Y = Utility.RandomFloat(-0.5f, 0.5f) * 4f;
      }
      float num1 = (float) (2.0 * Math.Round(Math.Sin(Game1.currentGameTime.ElapsedGameTime.TotalMilliseconds / 250.0), 2));
      Vector2 vector2 = Utility.PointToVector2(this.tilePosition.Value);
      float num2 = (float) ((double) vector2.Y * 64.0 / 10000.0);
      float num3 = num1 - 72f;
      b.Draw(Game1.mouseCursors, Game1.GlobalToLocal(Game1.viewport, new Vector2(vector2.X * 64f, (float) ((double) vector2.Y * 64.0 - 96.0 - 48.0) + num3)) + zero, new Microsoft.Xna.Framework.Rectangle?(new Microsoft.Xna.Framework.Rectangle(141, 465, 20, 24)), Color.White * 0.75f, 0.0f, Vector2.Zero, 4f, SpriteEffects.None, num2 + 1E-06f);
      Vector2 position = Game1.GlobalToLocal(Game1.viewport, new Vector2((float) ((double) vector2.X * 64.0 + 32.0 + 8.0), (float) ((double) vector2.Y * 64.0 - 64.0 - 32.0 - 8.0) + num3)) + zero;
      if (this.upgradeName.Value == "GoldenParrot")
      {
        b.Draw(Game1.mouseCursors_1_6, position, new Microsoft.Xna.Framework.Rectangle?(new Microsoft.Xna.Framework.Rectangle(131, 0, 16 /*0x10*/, 16 /*0x10*/)), Color.White, 0.0f, new Vector2(8f, 8f), 4f, SpriteEffects.None, num2 + 1E-05f);
      }
      else
      {
        b.Draw(Game1.objectSpriteSheet, position, new Microsoft.Xna.Framework.Rectangle?(Game1.getSourceRectForStandardTileSheet(Game1.objectSpriteSheet, 73, 16 /*0x10*/, 16 /*0x10*/)), Color.White, 0.0f, new Vector2(8f, 8f), 4f, SpriteEffects.None, num2 + 1E-05f);
        Utility.drawTinyDigits(this.requiredNuts.Value, b, position + new Vector2((float) ((double) (64 /*0x40*/ - Utility.getWidthOfTinyDigitString(this.requiredNuts.Value, 3f)) + 3.0 - 32.0), 16f), 3f, 1f, Color.White);
      }
    }
    foreach (ParrotUpgradePerch.Parrot parrot in this.parrots)
      parrot.Draw(b);
  }

  public enum UpgradeState
  {
    Idle,
    StartBuilding,
    Building,
    Complete,
  }

  public class Parrot
  {
    public Vector2 position;
    public float height;
    protected ParrotUpgradePerch _perch;
    protected Vector2 targetPosition = Vector2.Zero;
    protected Vector2 startPosition = Vector2.Zero;
    public Texture2D texture;
    public bool bounced;
    public bool flipped;
    public bool isPerchedParrot;
    private int baseFrame;
    private int birdType;
    private int flapFrame;
    private float nextFlapTime;
    public float alpha;
    public float moveTime;
    public float moveDuration = 1f;
    public bool firstBounce;
    public bool flyAway;
    private bool soundBird;

    public Parrot(
      ParrotUpgradePerch perch,
      Vector2 start_position,
      bool soundBird = false,
      bool goldenParrot = false)
    {
      this.soundBird = soundBird;
      this._perch = perch;
      this.texture = perch.texture;
      this.position = (start_position + new Vector2(0.5f, 0.5f)) * 64f;
      this.startPosition = start_position;
      this.height = 64f;
      this.birdType = Game1.random.Next(0, 4);
      if (goldenParrot)
        this.birdType = 4;
      this.FindNewTarget();
      this.firstBounce = true;
    }

    public virtual void FindNewTarget()
    {
      this.isPerchedParrot = false;
      this.firstBounce = false;
      this.startPosition = this.position;
      this.moveTime = 0.0f;
      this.moveDuration = 1f;
      Microsoft.Xna.Framework.Rectangle r = this._perch.upgradeRect.Value;
      if (this._perch.currentState.Value == ParrotUpgradePerch.UpgradeState.Complete)
      {
        this.flyAway = true;
        this.moveDuration = 5f;
        r.Inflate(5, 0);
      }
      this.targetPosition = (Utility.getRandomPositionInThisRectangle(r, Game1.random) + new Vector2(0.5f, 0.5f)) * 64f;
      Vector2 zero = Vector2.Zero with
      {
        X = this.targetPosition.X - this.position.X,
        Y = this.targetPosition.Y - this.position.Y
      };
      if ((double) zero.X < 0.0)
        this.flipped = false;
      else if ((double) zero.X > 0.0)
        this.flipped = true;
      if ((double) Math.Abs(zero.X) > (double) Math.Abs(zero.Y))
        this.baseFrame = 2;
      else if ((double) zero.Y > 0.0)
        this.baseFrame = 5;
      else
        this.baseFrame = 8;
    }

    public virtual bool Update(GameTime time)
    {
      this.moveTime += (float) time.ElapsedGameTime.TotalSeconds;
      if ((double) this.moveTime > (double) this.moveDuration)
        this.moveTime = this.moveDuration;
      float t = this.moveTime / this.moveDuration;
      this.position.X = Utility.Lerp(this.startPosition.X, this.targetPosition.X, t);
      this.position.Y = Utility.Lerp(this.startPosition.Y, this.targetPosition.Y, t);
      if (this.isPerchedParrot)
      {
        this.height = this.EaseInOutQuad(t, 24f, -24f, 1f);
        this.firstBounce = false;
        this.birdType = 0;
      }
      else if (this.flyAway)
        this.height = this.EaseInQuad(t, 0.0f, 1536f, 1f);
      else if (this.firstBounce)
      {
        this.height = this.EaseInOutQuad(t, 64f, -64f, 1f);
      }
      else
      {
        float num = 24f;
        this.height = (double) t > 0.5 ? this.EaseInQuad(t - 0.5f, num, -num, 0.5f) : this.EaseInOutQuad(t, 0.0f, num, 0.5f);
      }
      if ((double) t >= 1.0)
      {
        if (this.flyAway)
          return true;
        this.FindNewTarget();
        if (!this.firstBounce && this._perch.upgradeName.Value != "Turtle")
          this._perch.locationRef.Value.temporarySprites.Add(new TemporaryAnimatedSprite("LooseSprites\\Cursors2", new Microsoft.Xna.Framework.Rectangle(49 + 16 /*0x10*/ * (this._perch.upgradeName.Value.Contains("Volcano") ? 1 : Game1.random.Next(3)), 229, 16 /*0x10*/, 6), this.position, Game1.random.NextBool(), 0.0f, Color.White)
          {
            motion = new Vector2((float) Game1.random.Next(-2, 3), -12f),
            acceleration = new Vector2(0.0f, 0.25f),
            rotationChange = (float) Game1.random.Next(-4, 5) * 0.05f,
            scale = 4f,
            animationLength = 1,
            totalNumberOfLoops = 1,
            interval = (float) (1000 + Game1.random.Next(500)),
            layerDepth = 1f,
            drawAboveAlwaysFront = true
          });
      }
      if (this.firstBounce)
        this.alpha = Utility.Clamp(t / 0.25f, 0.0f, 1f);
      else if (this.flyAway)
      {
        float num = 0.1f;
        this.alpha = 1f - Utility.Clamp((t - (1f - num)) / num, 0.0f, 1f);
      }
      else
        this.alpha = 1f;
      if ((double) this.nextFlapTime > 0.0)
        this.nextFlapTime -= (float) time.ElapsedGameTime.TotalSeconds;
      if ((double) this.nextFlapTime <= 0.0)
      {
        this.flapFrame = (this.flapFrame + 1) % 3;
        if (this.flyAway || this.firstBounce || (double) this.height < 12.0 || (double) t < 0.5)
        {
          if (this.flapFrame == 0)
          {
            if (this._perch.upgradeName.Value != "Hut")
            {
              if ((!this.flyAway || (double) t < 0.5) && this.soundBird)
                this._perch.locationRef.Value.localSound("parrot_flap");
              if (!this.flyAway && !this.firstBounce)
              {
                if (this._perch.upgradeName.Value.Contains("Volcano"))
                {
                  if (Game1.random.NextDouble() < 0.15000000596046448)
                    this._perch.locationRef.Value.localSound("hammer");
                }
                else if (this._perch.upgradeName.Value == "Turtle")
                {
                  if (Game1.random.NextDouble() < 0.15000000596046448)
                    this._perch.locationRef.Value.localSound("hitEnemy");
                }
                else
                {
                  if (Game1.random.NextDouble() < 0.05000000074505806)
                    this._perch.locationRef.Value.localSound("axe");
                  if (Game1.random.NextDouble() < 0.05000000074505806)
                    this._perch.locationRef.Value.localSound("dirtyHit");
                  if (Game1.random.NextDouble() < 0.05000000074505806)
                    this._perch.locationRef.Value.localSound("crafting");
                }
              }
            }
            this.nextFlapTime = 0.1f;
          }
          else
            this.nextFlapTime = 0.05f;
        }
        else if (this.flapFrame == 0)
        {
          if (this.soundBird)
            this._perch.locationRef.Value.localSound("parrot_flap");
          this.nextFlapTime = 0.3f;
        }
        else
          this.nextFlapTime = 0.2f;
      }
      return false;
    }

    private float EaseInOutQuad(float t, float b, float c, float d)
    {
      return (double) (t /= d / 2f) < 1.0 ? c / 2f * t * t + b : (float) (-(double) c / 2.0 * ((double) --t * ((double) t - 2.0) - 1.0)) + b;
    }

    private float EaseInQuad(float t, float b, float c, float d) => c * (t /= d) * t + b;

    public virtual void Draw(SpriteBatch b)
    {
      int num = this.baseFrame + this.flapFrame;
      b.Draw(this.texture, Game1.GlobalToLocal(Game1.viewport, Utility.snapDrawPosition(this.position - new Vector2(0.0f, this.height * 4f))), new Microsoft.Xna.Framework.Rectangle?(new Microsoft.Xna.Framework.Rectangle(num * 24, this.birdType * 24, 24, 24)), Color.White * this.alpha, 0.0f, new Vector2(12f, 18f), 4f, this.flipped ? SpriteEffects.FlipHorizontally : SpriteEffects.None, this.position.Y / 10000f);
    }
  }
}
