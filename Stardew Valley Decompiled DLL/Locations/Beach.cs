// Decompiled with JetBrains decompiler
// Type: StardewValley.Locations.Beach
// Assembly: Stardew Valley, Version=1.6.15.24356, Culture=neutral, PublicKeyToken=null
// MVID: AA1F513A-94F0-4EF6-A35F-2D5B4A3721BF
// Assembly location: D:\SteamLibrary\steamapps\common\Stardew Valley\Stardew Valley.dll
// XML documentation location: D:\SteamLibrary\steamapps\common\Stardew Valley\Stardew Valley.xml

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Netcode;
using StardewValley.BellsAndWhistles;
using StardewValley.Constants;
using StardewValley.Extensions;
using StardewValley.Menus;
using StardewValley.Network;
using System;
using System.Xml.Serialization;
using xTile;
using xTile.Dimensions;

#nullable disable
namespace StardewValley.Locations;

public class Beach : GameLocation
{
  private NPC oldMariner;
  [XmlElement("bridgeFixed")]
  public readonly NetBool bridgeFixed = new NetBool();
  [XmlIgnore]
  public NetMutex derbyMutex = new NetMutex();
  private bool hasShownCCUpgrade;

  public Beach()
  {
  }

  public Beach(string mapPath, string name)
    : base(mapPath, name)
  {
  }

  protected override void initNetFields()
  {
    base.initNetFields();
    this.NetFields.AddField((INetSerializable) this.bridgeFixed, "bridgeFixed").AddField((INetSerializable) this.derbyMutex.NetFields, "derbyMutex.NetFields");
    this.bridgeFixed.fieldChangeEvent += (FieldChange<NetBool, bool>) ((f, oldValue, newValue) =>
    {
      if (!newValue || this.mapPath.Value == null)
        return;
      Beach.fixBridge((GameLocation) this);
    });
    this.characters.OnValueAdded += (NetCollection<NPC>.ContentsChangeEvent) (newCharacter => this.adjustDerbyFisherman(newCharacter));
  }

  private void adjustDerbyFisherman(NPC npc)
  {
    if (npc.name.Equals((object) "winter_derby_contestent0"))
    {
      if (npc.Sprite?.Texture == null)
        npc.Sprite = new AnimatedSprite("Characters\\Assorted_Fishermen", 0, 16 /*0x10*/, 64 /*0x40*/);
      npc.drawOffset = new Vector2(0.0f, 96f);
      npc.shouldShadowBeOffset = true;
      npc.SimpleNonVillagerNPC = true;
      npc.HideShadow = true;
      npc.Breather = false;
    }
    if (npc.name.Equals((object) "winter_derby_contestent1"))
    {
      if (npc.Sprite?.Texture == null)
        npc.Sprite = new AnimatedSprite("Characters\\Assorted_Fishermen", 2, 16 /*0x10*/, 64 /*0x40*/);
      npc.Sprite.CurrentFrame = 2;
      npc.drawOffset = new Vector2(0.0f, 96f);
      npc.shouldShadowBeOffset = true;
      npc.SimpleNonVillagerNPC = true;
      npc.HideShadow = true;
      npc.Breather = false;
    }
    if (npc.name.Equals((object) "winter_derby_contestent2"))
    {
      if (npc.Sprite?.Texture == null)
        npc.Sprite = new AnimatedSprite("Characters\\Assorted_Fishermen", 3, 16 /*0x10*/, 64 /*0x40*/);
      npc.Sprite.CurrentFrame = 3;
      npc.drawOffset = new Vector2(0.0f, 96f);
      npc.shouldShadowBeOffset = true;
      npc.SimpleNonVillagerNPC = true;
      npc.HideShadow = true;
      npc.Breather = false;
    }
    if (npc.name.Equals((object) "winter_derby_contestent3"))
    {
      if (npc.Sprite?.Texture == null)
        npc.Sprite = new AnimatedSprite("Characters\\Assorted_Fishermen", 1, 16 /*0x10*/, 64 /*0x40*/);
      npc.Sprite.CurrentFrame = 1;
      npc.drawOffset = new Vector2(0.0f, 96f);
      npc.shouldShadowBeOffset = true;
      npc.SimpleNonVillagerNPC = true;
      npc.HideShadow = true;
      npc.Breather = false;
    }
    if (npc.name.Equals((object) "winter_derby_contestent4"))
    {
      if (npc.Sprite?.Texture == null)
        npc.Sprite = new AnimatedSprite("Characters\\Assorted_Fishermen", 2, 32 /*0x20*/, 64 /*0x40*/);
      npc.Sprite.CurrentFrame = 2;
      npc.drawOffset = new Vector2(0.0f, 96f);
      npc.shouldShadowBeOffset = true;
      npc.SimpleNonVillagerNPC = true;
      npc.HideShadow = true;
      npc.Breather = false;
    }
    if (npc.name.Equals((object) "winter_derby_contestent5"))
    {
      if (npc.Sprite?.Texture == null)
        npc.Sprite = new AnimatedSprite("Characters\\Assorted_Fishermen", 8, 32 /*0x20*/, 32 /*0x20*/);
      npc.Sprite.CurrentFrame = 8;
      npc.shouldShadowBeOffset = true;
      npc.SimpleNonVillagerNPC = true;
      npc.HideShadow = true;
      npc.Breather = false;
    }
    if (npc.name.Equals((object) "winter_derby_contestent6"))
    {
      if (npc.Sprite?.Texture == null)
        npc.Sprite = new AnimatedSprite("Characters\\Assorted_Fishermen", 9, 32 /*0x20*/, 32 /*0x20*/);
      npc.Sprite.CurrentFrame = 9;
      npc.shouldShadowBeOffset = true;
      npc.SimpleNonVillagerNPC = true;
      npc.HideShadow = true;
      npc.Breather = false;
    }
    if (npc.name.Equals((object) "winter_derby_contestent7"))
    {
      if (npc.Sprite?.Texture == null)
        npc.Sprite = new AnimatedSprite("Characters\\Assorted_Fishermen", 10, 32 /*0x20*/, 32 /*0x20*/);
      npc.Sprite.CurrentFrame = 10;
      npc.shouldShadowBeOffset = true;
      npc.SimpleNonVillagerNPC = true;
      npc.HideShadow = true;
      npc.Breather = false;
    }
    if (npc.name.Equals((object) "winter_derby_contestent8"))
    {
      if (npc.Sprite?.Texture == null)
        npc.Sprite = new AnimatedSprite("Characters\\Assorted_Fishermen", 11, 32 /*0x20*/, 32 /*0x20*/);
      npc.Sprite.CurrentFrame = 11;
      npc.shouldShadowBeOffset = true;
      npc.SimpleNonVillagerNPC = true;
      npc.HideShadow = true;
      npc.Breather = false;
    }
    if (npc.name.Equals((object) "winter_derby_contestent9"))
    {
      if (npc.Sprite?.Texture == null)
        npc.Sprite = new AnimatedSprite("Characters\\Assorted_Fishermen", 12, 32 /*0x20*/, 32 /*0x20*/);
      npc.Sprite.CurrentFrame = 12;
      npc.shouldShadowBeOffset = true;
      npc.SimpleNonVillagerNPC = true;
      npc.HideShadow = true;
      npc.Breather = false;
    }
    if (npc.name.Equals((object) "winter_derby_contestent10"))
    {
      if (npc.Sprite?.Texture == null)
        npc.Sprite = new AnimatedSprite("Characters\\Assorted_Fishermen", 6, 16 /*0x10*/, 64 /*0x40*/);
      npc.Sprite.CurrentFrame = 6;
      npc.drawOffset = new Vector2(0.0f, 96f);
      npc.shouldShadowBeOffset = true;
      npc.SimpleNonVillagerNPC = true;
      npc.HideShadow = true;
      npc.Breather = false;
    }
    if (!npc.name.Equals((object) "winter_derby_contestent11"))
      return;
    if (npc.Sprite?.Texture == null)
      npc.Sprite = new AnimatedSprite("Characters\\Assorted_Fishermen", 7, 16 /*0x10*/, 64 /*0x40*/);
    npc.Sprite.CurrentFrame = 7;
    npc.drawOffset = new Vector2(0.0f, 96f);
    npc.shouldShadowBeOffset = true;
    npc.SimpleNonVillagerNPC = true;
    npc.HideShadow = true;
    npc.Breather = false;
  }

  public override void updateEvenIfFarmerIsntHere(GameTime time, bool ignoreWasUpdatedFlush = false)
  {
    base.updateEvenIfFarmerIsntHere(time, ignoreWasUpdatedFlush);
    this.derbyMutex.Update((GameLocation) this);
  }

  public override void UpdateWhenCurrentLocation(GameTime time)
  {
    if (this.wasUpdated)
      return;
    base.UpdateWhenCurrentLocation(time);
    this.oldMariner?.update(time, (GameLocation) this);
    if (Game1.eventUp || Game1.random.NextDouble() >= 1E-06)
      return;
    Vector2 position = new Vector2((float) (Game1.random.Next(15, 47) * 64 /*0x40*/), (float) (Game1.random.Next(29, 42) * 64 /*0x40*/));
    bool flag = true;
    for (float yTile = position.Y / 64f; (double) yTile < (double) this.map.RequireLayer("Back").LayerHeight; ++yTile)
    {
      if (!this.isWaterTile((int) position.X / 64 /*0x40*/, (int) yTile) || !this.isWaterTile((int) position.X / 64 /*0x40*/ - 1, (int) yTile) || !this.isWaterTile((int) position.X / 64 /*0x40*/ + 1, (int) yTile))
      {
        flag = false;
        break;
      }
    }
    if (!flag)
      return;
    this.temporarySprites.Add((TemporaryAnimatedSprite) new SeaMonsterTemporarySprite(250f, 4, Game1.random.Next(7), position));
  }

  public override void cleanupBeforePlayerExit()
  {
    base.cleanupBeforePlayerExit();
    this.oldMariner = (NPC) null;
    this.derbyMutex.ReleaseLock();
  }

  public override void DayUpdate(int dayOfMonth)
  {
    base.DayUpdate(dayOfMonth);
    Microsoft.Xna.Framework.Rectangle rectangle1 = new Microsoft.Xna.Framework.Rectangle(65, 11, 25, 12);
    for (float num = 1f; Game1.random.NextDouble() < (double) num; num /= 2f)
    {
      string itemId = Game1.random.NextDouble() < 0.2 ? "(O)397" : "(O)393";
      Vector2 tile = new Vector2((float) Game1.random.Next(rectangle1.X, rectangle1.Right), (float) Game1.random.Next(rectangle1.Y, rectangle1.Bottom));
      if (this.CanItemBePlacedHere(tile))
        this.dropObject(ItemRegistry.Create<StardewValley.Object>(itemId), tile * 64f, Game1.viewport, true);
    }
    Microsoft.Xna.Framework.Rectangle rectangle2 = new Microsoft.Xna.Framework.Rectangle(66, 24, 19, 1);
    for (float num = 0.25f; Game1.random.NextDouble() < (double) num; num /= 2f)
    {
      if (Game1.random.NextDouble() < 0.1)
      {
        Vector2 tile = new Vector2((float) Game1.random.Next(rectangle2.X, rectangle2.Right), (float) Game1.random.Next(rectangle2.Y, rectangle2.Bottom));
        if (this.CanItemBePlacedHere(tile))
          this.dropObject(ItemRegistry.Create<StardewValley.Object>("(O)152"), tile * 64f, Game1.viewport, true);
      }
    }
    if (this.IsSummerHere() && Game1.dayOfMonth >= 12 && Game1.dayOfMonth <= 14)
    {
      for (int index = 0; index < 5; ++index)
        this.spawnObjects();
      for (float num = 1.5f; Game1.random.NextDouble() < (double) num; num /= 1.1f)
      {
        string itemId = Game1.random.NextDouble() < 0.2 ? "(O)397" : "(O)393";
        Vector2 randomTile = this.getRandomTile();
        randomTile.Y /= 2f;
        string str = this.doesTileHaveProperty((int) randomTile.X, (int) randomTile.Y, "Type", "Back");
        if (this.CanItemBePlacedHere(randomTile))
        {
          switch (str)
          {
            case "Wood":
              continue;
            default:
              this.dropObject(ItemRegistry.Create<StardewValley.Object>(itemId), randomTile * 64f, Game1.viewport, true);
              continue;
          }
        }
      }
    }
    if (!Game1.IsWinter)
      return;
    this.characters.RemoveWhere((Func<NPC, bool>) (npc => npc.Name.Contains("derby_contestent")));
  }

  public void doneWithBridgeFix()
  {
    Game1.globalFadeToClear();
    Game1.viewportFreeze = false;
    Game1.freezeControls = false;
  }

  public void fadedForBridgeFix()
  {
    Game1.freezeControls = true;
    DelayedAction.playSoundAfterDelay("crafting", 1000);
    DelayedAction.playSoundAfterDelay("crafting", 1500);
    DelayedAction.playSoundAfterDelay("crafting", 2000);
    DelayedAction.playSoundAfterDelay("crafting", 2500);
    DelayedAction.playSoundAfterDelay("axchop", 3000);
    DelayedAction.playSoundAfterDelay("Ship", 3200);
    Game1.viewportFreeze = true;
    Game1.viewport.X = -10000;
    this.bridgeFixed.Value = true;
    Game1.pauseThenDoFunction(4000, new Game1.afterFadeFunction(this.doneWithBridgeFix));
    Beach.fixBridge((GameLocation) this);
  }

  public override bool answerDialogueAction(string questionAndAnswer, string[] questionParams)
  {
    if (!(questionAndAnswer == "BeachBridge_Yes"))
      return base.answerDialogueAction(questionAndAnswer, questionParams);
    Game1.globalFadeToBlack(new Game1.afterFadeFunction(this.fadedForBridgeFix));
    Game1.player.Items.ReduceId("(O)388", 300);
    return true;
  }

  public override bool checkAction(Location tileLocation, xTile.Dimensions.Rectangle viewport, Farmer who)
  {
    switch (this.getTileIndexAt(tileLocation, "Buildings", "untitled tile sheet"))
    {
      case 284:
        if (who.Items.ContainsId("(O)388", 300))
        {
          this.createQuestionDialogue(Game1.content.LoadString("Strings\\Locations:Beach_FixBridge_Question"), this.createYesNoResponses(), "BeachBridge");
          break;
        }
        Game1.drawObjectDialogue(Game1.content.LoadString("Strings\\Locations:Beach_FixBridge_Hint"));
        break;
      case 496:
        if (Game1.Date.TotalDays < 1)
        {
          Game1.drawLetterMessage(Game1.content.LoadString("Strings\\Locations:Beach_GoneFishingMessage").Replace('\n', '^'));
          return false;
        }
        break;
    }
    if (this.oldMariner == null || this.oldMariner.TilePoint.X != tileLocation.X || this.oldMariner.TilePoint.Y != tileLocation.Y)
      return base.checkAction(tileLocation, viewport, who);
    string sub1 = Game1.content.LoadString("Strings\\Locations:Beach_Mariner_Player_" + (who.IsMale ? "Male" : "Female"));
    if (!who.isMarriedOrRoommates() && who.specialItems.Contains("460") && !Utility.doesItemExistAnywhere("(O)460"))
      who.specialItems.RemoveWhere((Func<string, bool>) (id => id == "460"));
    if (who.isMarriedOrRoommates())
      Game1.drawObjectDialogue(Game1.parseText(Game1.content.LoadString("Strings\\Locations:Beach_Mariner_PlayerMarried", (object) sub1)));
    else if (who.specialItems.Contains("460"))
      Game1.drawObjectDialogue(Game1.parseText(Game1.content.LoadString("Strings\\Locations:Beach_Mariner_PlayerHasItem", (object) sub1)));
    else if (who.hasAFriendWithHeartLevel(10, true) && who.houseUpgradeLevel.Value == 0)
      Game1.drawObjectDialogue(Game1.parseText(Game1.content.LoadString("Strings\\Locations:Beach_Mariner_PlayerNotUpgradedHouse", (object) sub1)));
    else if (who.hasAFriendWithHeartLevel(10, true))
    {
      Response[] answerChoices = new Response[2]
      {
        new Response("Buy", Game1.content.LoadString("Strings\\Locations:Beach_Mariner_PlayerBuyItem_AnswerYes")),
        new Response("Not", Game1.content.LoadString("Strings\\Locations:Beach_Mariner_PlayerBuyItem_AnswerNo"))
      };
      this.createQuestionDialogue(Game1.parseText(Game1.content.LoadString("Strings\\Locations:Beach_Mariner_PlayerBuyItem_Question", (object) sub1)), answerChoices, "mariner");
    }
    else
      Game1.drawObjectDialogue(Game1.parseText(Game1.content.LoadString("Strings\\Locations:Beach_Mariner_PlayerNoRelationship", (object) sub1)));
    return true;
  }

  public override bool isCollidingPosition(
    Microsoft.Xna.Framework.Rectangle position,
    xTile.Dimensions.Rectangle viewport,
    bool isFarmer,
    int damagesFarmer,
    bool glider,
    Character character)
  {
    return this.oldMariner != null && position.Intersects(this.oldMariner.GetBoundingBox()) || base.isCollidingPosition(position, viewport, isFarmer, damagesFarmer, glider, character);
  }

  /// <inheritdoc />
  public override void checkForMusic(GameTime time)
  {
    if (Game1.random.NextDouble() < 0.003 && Game1.timeOfDay < 1900)
      this.localSound("seagulls");
    base.checkForMusic(time);
  }

  protected override void resetSharedState()
  {
    base.resetSharedState();
    if (!this.IsSummerHere() || Game1.dayOfMonth < 12 || Game1.dayOfMonth > 14)
      return;
    this.waterColor.Value = new Color(0, (int) byte.MaxValue, 0) * 0.4f;
  }

  public override void performTenMinuteUpdate(int timeOfDay)
  {
    base.performTenMinuteUpdate(timeOfDay);
    if (!Game1.IsWinter || Game1.dayOfMonth < 12 || Game1.dayOfMonth > 13)
      return;
    Random daySaveRandom = Utility.CreateDaySaveRandom((double) (Game1.timeOfDay * 20));
    NPC characterFromName = this.getCharacterFromName("winter_derby_contestent" + daySaveRandom.Next(10).ToString());
    if (characterFromName == null)
      return;
    characterFromName.shake(600);
    if (!daySaveRandom.NextBool(0.25))
      return;
    int num = daySaveRandom.Next(7);
    characterFromName.showTextAboveHead(Game1.content.LoadString("Strings\\1_6_Strings:FishingDerby_Exclamation" + num.ToString()));
    if (num == 0 || num == 6)
      this.temporarySprites.Add(new TemporaryAnimatedSprite(151, 1500f, 1, 1, characterFromName.Position, false, false, false, 0.0f)
      {
        motion = new Vector2((float) Game1.random.Next(-10, 10) / 10f, -7f),
        acceleration = new Vector2(0.0f, 0.1f),
        alphaFade = 1f / 1000f,
        drawAboveAlwaysFront = true
      });
    characterFromName.jump(4f);
  }

  public override void MakeMapModifications(bool force = false)
  {
    base.MakeMapModifications(force);
    if (force)
      this.hasShownCCUpgrade = false;
    if (this.bridgeFixed.Value)
      Beach.fixBridge((GameLocation) this);
    if (Game1.MasterPlayer.mailReceived.Contains("communityUpgradeShortcuts"))
      Beach.showCommunityUpgradeShortcuts((GameLocation) this, ref this.hasShownCCUpgrade);
    if (Game1.IsWinter && Game1.dayOfMonth >= 9 && Game1.dayOfMonth <= 11)
      this.ApplyMapOverride(Game1.game1.xTileContent.Load<Map>("Maps\\Forest_FishingDerbySign"), "Forest_FishingDerbySign", dest_rect: new Microsoft.Xna.Framework.Rectangle?(new Microsoft.Xna.Framework.Rectangle(15, 5, 2, 3)), perTileCustomAction: new Action<Point>(((GameLocation) this).cleanUpTileForMapOverride));
    else if (this._appliedMapOverrides.Contains("Forest_FishingDerbySign"))
    {
      this.ApplyMapOverride("Beach_SquidFestSign_Revert", destination_rect: new Microsoft.Xna.Framework.Rectangle?(new Microsoft.Xna.Framework.Rectangle(15, 5, 2, 3)));
      this._appliedMapOverrides.Remove("Forest_FishingDerbySign");
      this._appliedMapOverrides.Remove("Beach_SquidFestSign_Revert");
    }
    if (Game1.IsWinter && Game1.dayOfMonth >= 12 && Game1.dayOfMonth <= 13)
    {
      if (this.getCharacterFromName("winter_derby_contestent0") == null && (Game1.IsMasterGame || !Game1.player.sleptInTemporaryBed.Value))
        this.derbyMutex.RequestLock((Action) (() =>
        {
          if (this.getCharacterFromName("winter_derby_contestent0") == null)
          {
            if (this.checkForTerrainFeaturesAndObjectsButDestroyNonPlayerItems(15, 17))
            {
              NetCollection<NPC> characters = this.characters;
              characters.Add(new NPC(new AnimatedSprite("Characters\\Assorted_Fishermen_Winter", 0, 16 /*0x10*/, 64 /*0x40*/), new Vector2(15f, 17f) * 64f, -1, "winter_derby_contestent0")
              {
                Breather = false,
                HideShadow = true,
                drawOffset = new Vector2(0.0f, 96f),
                shouldShadowBeOffset = true,
                SimpleNonVillagerNPC = true
              });
            }
            if (this.checkForTerrainFeaturesAndObjectsButDestroyNonPlayerItems(30, 21))
            {
              NetCollection<NPC> characters = this.characters;
              characters.Add(new NPC(new AnimatedSprite("Characters\\Assorted_Fishermen_Winter", 2, 16 /*0x10*/, 64 /*0x40*/), new Vector2(30f, 21f) * 64f, -1, "winter_derby_contestent1")
              {
                Breather = false,
                HideShadow = true,
                drawOffset = new Vector2(0.0f, 96f),
                shouldShadowBeOffset = true,
                SimpleNonVillagerNPC = true
              });
            }
            if (this.checkForTerrainFeaturesAndObjectsButDestroyNonPlayerItems(13, 39))
            {
              NetCollection<NPC> characters = this.characters;
              characters.Add(new NPC(new AnimatedSprite("Characters\\Assorted_Fishermen_Winter", 3, 16 /*0x10*/, 64 /*0x40*/), new Vector2(13f, 39f) * 64f, -1, "winter_derby_contestent2")
              {
                Breather = false,
                HideShadow = true,
                drawOffset = new Vector2(0.0f, 96f),
                shouldShadowBeOffset = true,
                SimpleNonVillagerNPC = true
              });
            }
            if (this.checkForTerrainFeaturesAndObjectsButDestroyNonPlayerItems(42, 25))
            {
              NetCollection<NPC> characters = this.characters;
              characters.Add(new NPC(new AnimatedSprite("Characters\\Assorted_Fishermen_Winter", 1, 16 /*0x10*/, 64 /*0x40*/), new Vector2(42f, 25f) * 64f, -1, "winter_derby_contestent3")
              {
                Breather = false,
                HideShadow = true,
                drawOffset = new Vector2(0.0f, 96f),
                shouldShadowBeOffset = true,
                SimpleNonVillagerNPC = true
              });
            }
            if (this.checkForTerrainFeaturesAndObjectsButDestroyNonPlayerItems(50, 25) && this.checkForTerrainFeaturesAndObjectsButDestroyNonPlayerItems(51, 25))
            {
              NetCollection<NPC> characters = this.characters;
              characters.Add(new NPC(new AnimatedSprite("Characters\\Assorted_Fishermen_Winter", 2, 32 /*0x20*/, 64 /*0x40*/), new Vector2(50f, 25f) * 64f, -1, "winter_derby_contestent4")
              {
                Breather = false,
                HideShadow = true,
                drawOffset = new Vector2(0.0f, 96f),
                shouldShadowBeOffset = true,
                SimpleNonVillagerNPC = true
              });
            }
            if (this.checkForTerrainFeaturesAndObjectsButDestroyNonPlayerItems(56, 19))
            {
              NetCollection<NPC> characters = this.characters;
              characters.Add(new NPC(new AnimatedSprite("Characters\\Assorted_Fishermen_Winter", 8, 32 /*0x20*/, 32 /*0x20*/), new Vector2(56f, 19f) * 64f, -1, "winter_derby_contestent5")
              {
                Breather = false,
                HideShadow = true,
                drawOffset = new Vector2(0.0f, 0.0f),
                shouldShadowBeOffset = true,
                SimpleNonVillagerNPC = true
              });
            }
            if (this.checkForTerrainFeaturesAndObjectsButDestroyNonPlayerItems(11, 28))
            {
              NetCollection<NPC> characters = this.characters;
              characters.Add(new NPC(new AnimatedSprite("Characters\\Assorted_Fishermen_Winter", 9, 32 /*0x20*/, 32 /*0x20*/), new Vector2(10f, 28f) * 64f, -1, "winter_derby_contestent6")
              {
                Breather = false,
                HideShadow = true,
                drawOffset = new Vector2(0.0f, 0.0f),
                shouldShadowBeOffset = true,
                SimpleNonVillagerNPC = true
              });
            }
            if (this.checkForTerrainFeaturesAndObjectsButDestroyNonPlayerItems(14, 39))
            {
              NetCollection<NPC> characters = this.characters;
              characters.Add(new NPC(new AnimatedSprite("Characters\\Assorted_Fishermen_Winter", 10, 32 /*0x20*/, 32 /*0x20*/), new Vector2(14f, 39f) * 64f, -1, "winter_derby_contestent7")
              {
                Breather = false,
                HideShadow = true,
                drawOffset = new Vector2(0.0f, 0.0f),
                shouldShadowBeOffset = true,
                SimpleNonVillagerNPC = true
              });
            }
            if (this.checkForTerrainFeaturesAndObjectsButDestroyNonPlayerItems(90, 40))
            {
              NetCollection<NPC> characters = this.characters;
              characters.Add(new NPC(new AnimatedSprite("Characters\\Assorted_Fishermen_Winter", 11, 32 /*0x20*/, 32 /*0x20*/), new Vector2(90f, 40f) * 64f, -1, "winter_derby_contestent8")
              {
                Breather = false,
                HideShadow = true,
                drawOffset = new Vector2(0.0f, 0.0f),
                shouldShadowBeOffset = true,
                SimpleNonVillagerNPC = true
              });
            }
            if (this.checkForTerrainFeaturesAndObjectsButDestroyNonPlayerItems(8, 12))
            {
              NetCollection<NPC> characters = this.characters;
              characters.Add(new NPC(new AnimatedSprite("Characters\\Assorted_Fishermen_Winter", 12, 32 /*0x20*/, 32 /*0x20*/), new Vector2(7f, 12f) * 64f, -1, "winter_derby_contestent9")
              {
                Breather = false,
                HideShadow = true,
                drawOffset = new Vector2(0.0f, 0.0f),
                shouldShadowBeOffset = true,
                SimpleNonVillagerNPC = true
              });
            }
            if (this.checkForTerrainFeaturesAndObjectsButDestroyNonPlayerItems(47, 21))
            {
              NetCollection<NPC> characters = this.characters;
              characters.Add(new NPC(new AnimatedSprite("Characters\\Assorted_Fishermen_Winter", 6, 16 /*0x10*/, 64 /*0x40*/), new Vector2(47f, 21f) * 64f, -1, "winter_derby_contestent10")
              {
                Breather = false,
                HideShadow = true,
                drawOffset = new Vector2(0.0f, 96f),
                shouldShadowBeOffset = true,
                SimpleNonVillagerNPC = true
              });
            }
            if (this.checkForTerrainFeaturesAndObjectsButDestroyNonPlayerItems(22, 8))
            {
              NetCollection<NPC> characters = this.characters;
              characters.Add(new NPC(new AnimatedSprite("Characters\\Assorted_Fishermen_Winter", 7, 16 /*0x10*/, 64 /*0x40*/), new Vector2(22f, 8f) * 64f, -1, "winter_derby_contestent11")
              {
                Breather = false,
                HideShadow = true,
                drawOffset = new Vector2(0.0f, 96f),
                shouldShadowBeOffset = true,
                SimpleNonVillagerNPC = true
              });
            }
          }
          this.derbyMutex.ReleaseLock();
        }));
      this.ApplyMapOverride(Game1.game1.xTileContent.Load<Map>("Maps\\Beach_SquidFest"), "Beach_SquidFest", dest_rect: new Microsoft.Xna.Framework.Rectangle?(new Microsoft.Xna.Framework.Rectangle(11, 3, 16 /*0x10*/, 5)), perTileCustomAction: new Action<Point>(((GameLocation) this).cleanUpTileForMapOverride));
      if (Game1.dayOfMonth == 13)
      {
        string overrideTilesheetId = GameLocation.GetAddedMapOverrideTilesheetId("Beach_SquidFest", "16");
        this.setMapTile(13, 6, 51, "Front", overrideTilesheetId);
        this.setMapTile(13, 5, 43, "AlwaysFront", overrideTilesheetId);
      }
      this.setFireplace(true, 48 /*0x30*/, 20, false, yOffset: 64 /*0x40*/);
      Game1.currentLightSources.Add(new LightSource("SquidFest_1", 1, new Vector2(732f, 480f), 4f, onlyLocation: this.NameOrUniqueName));
      Game1.currentLightSources.Add(new LightSource("SquidFest_2", 1, new Vector2(1064f, 368f), 4f, onlyLocation: this.NameOrUniqueName));
      Game1.currentLightSources.Add(new LightSource("SquidFest_3", 1, new Vector2(1692f, 476f), 4f, onlyLocation: this.NameOrUniqueName));
      Game1.currentLightSources.Add(new LightSource("SquidFest_4", 1, new Vector2(1372f, 476f), 4f, onlyLocation: this.NameOrUniqueName));
      Game1.currentLightSources.Add(new LightSource("SquidFest_5", 1, new Vector2(1532f, 380f), 4f, onlyLocation: this.NameOrUniqueName));
      Game1.currentLightSources.Add(new LightSource("SquidFest_6", 1, new Vector2(15.5f, 17.5f) * 64f, 4f, onlyLocation: this.NameOrUniqueName));
      Game1.currentLightSources.Add(new LightSource("SquidFest_7", 1, new Vector2(30.5f, 21f) * 64f, 4f, onlyLocation: this.NameOrUniqueName));
    }
    else
    {
      if (!this._appliedMapOverrides.Contains("Beach_SquidFest") && this.getTileIndexAt(11, 7, "Buildings", "16") != 45)
        return;
      this.ApplyMapOverride("Beach_SquidFest_Revert", destination_rect: new Microsoft.Xna.Framework.Rectangle?(new Microsoft.Xna.Framework.Rectangle(11, 3, 16 /*0x10*/, 5)));
      this._appliedMapOverrides.Remove("Beach_SquidFest");
      this._appliedMapOverrides.Remove("Beach_SquidFest_Revert");
      this.characters.RemoveWhere((Func<NPC, bool>) (npc => npc.Name.Contains("derby_contestent")));
    }
  }

  public override void drawOverlays(SpriteBatch b)
  {
    if (Game1.IsWinter && Game1.dayOfMonth >= 12 && Game1.dayOfMonth <= 13)
      SpecialCurrencyDisplay.Draw(b, new Vector2(16f, 0.0f), (int) Game1.stats.Get(StatKeys.SquidFestScore(Game1.dayOfMonth, Game1.year)), Game1.objectSpriteSheet, new Microsoft.Xna.Framework.Rectangle(112 /*0x70*/, 96 /*0x60*/, 16 /*0x10*/, 16 /*0x10*/));
    base.drawOverlays(b);
  }

  protected override void resetLocalState()
  {
    base.resetLocalState();
    int number = Game1.random.Next(6);
    foreach (Vector2 vector2 in Utility.getPositionsInClusterAroundThisTile(new Vector2((float) Game1.random.Next(this.map.DisplayWidth / 64 /*0x40*/), (float) Game1.random.Next(12, this.map.DisplayHeight / 64 /*0x40*/)), number))
    {
      if (this.isTileOnMap(vector2) && (this.CanItemBePlacedHere(vector2) || this.isWaterTile((int) vector2.X, (int) vector2.Y)) && ((double) vector2.X < 23.0 || (double) vector2.X > 46.0))
      {
        int startingState = 3;
        if (this.isWaterTile((int) vector2.X, (int) vector2.Y) && this.doesTileHaveProperty((int) vector2.X, (int) vector2.Y, "Passable", "Buildings") == null)
        {
          startingState = 2;
          if (Game1.random.NextBool())
            continue;
        }
        this.critters.Add((Critter) new Seagull(vector2 * 64f + new Vector2(32f, 32f), startingState));
      }
    }
    this.tryAddPrismaticButterfly();
    if (!this.IsRainingHere() || Game1.timeOfDay >= 1900)
      return;
    this.oldMariner = new NPC(new AnimatedSprite("Characters\\Mariner", 0, 16 /*0x10*/, 32 /*0x20*/), new Vector2(80f, 5f) * 64f, 2, "Old Mariner")
    {
      AllowDynamicAppearance = false
    };
  }

  public static void showCommunityUpgradeShortcuts(GameLocation location, ref bool flag)
  {
    if (flag)
      return;
    flag = true;
    location.warps.Add(new Warp(-1, 4, "Forest", 119, 35, false));
    location.warps.Add(new Warp(-1, 5, "Forest", 119, 35, false));
    location.warps.Add(new Warp(-1, 6, "Forest", 119, 36, false));
    location.warps.Add(new Warp(-1, 7, "Forest", 119, 36, false));
    for (int x = 0; x < 5; ++x)
    {
      for (int y = 4; y < 7; ++y)
        location.removeTile(x, y, "Buildings");
    }
    location.removeTile(7, 6, "Buildings");
    location.removeTile(5, 6, "Buildings");
    location.removeTile(6, 6, "Buildings");
    location.setMapTile(3, 7, 107, "Back", "untitled tile sheet");
    location.removeTile(67, 5, "Buildings");
    location.removeTile(67, 4, "Buildings");
    location.removeTile(67, 3, "Buildings");
    location.removeTile(67, 2, "Buildings");
    location.removeTile(67, 1, "Buildings");
    location.removeTile(67, 0, "Buildings");
    location.removeTile(66, 3, "Buildings");
    location.removeTile(68, 3, "Buildings");
  }

  public static void fixBridge(GameLocation location)
  {
    if (!NetWorldState.checkAnywhereForWorldStateID("beachBridgeFixed"))
      NetWorldState.addWorldStateIDEverywhere("beachBridgeFixed");
    location.updateMap();
    location.setMapTile(58, 13, 301, "Buildings", "untitled tile sheet");
    location.setMapTile(59, 13, 301, "Buildings", "untitled tile sheet");
    location.setMapTile(60, 13, 301, "Buildings", "untitled tile sheet");
    location.setMapTile(61, 13, 301, "Buildings", "untitled tile sheet");
    location.removeTileProperty(58, 13, "Buildings", "Action");
    location.setMapTile(58, 14, 336, "Back", "untitled tile sheet");
    location.setMapTile(59, 14, 336, "Back", "untitled tile sheet");
    location.setMapTile(60, 14, 336, "Back", "untitled tile sheet");
    location.setMapTile(61, 14, 336, "Back", "untitled tile sheet");
  }

  public override void draw(SpriteBatch b)
  {
    this.oldMariner?.draw(b);
    base.draw(b);
    if (this.bridgeFixed.Value)
      return;
    float num = (float) (4.0 * Math.Round(Math.Sin(Game1.currentGameTime.TotalGameTime.TotalMilliseconds / 250.0), 2));
    b.Draw(Game1.mouseCursors, Game1.GlobalToLocal(Game1.viewport, new Vector2(3704f, 720f + num)), new Microsoft.Xna.Framework.Rectangle?(new Microsoft.Xna.Framework.Rectangle(141, 465, 20, 24)), Color.White * 0.75f, 0.0f, Vector2.Zero, 4f, SpriteEffects.None, 0.095401f);
    b.Draw(Game1.mouseCursors, Game1.GlobalToLocal(Game1.viewport, new Vector2(3744f, 760f + num)), new Microsoft.Xna.Framework.Rectangle?(new Microsoft.Xna.Framework.Rectangle(175, 425, 12, 12)), Color.White * 0.75f, 0.0f, new Vector2(6f, 6f), 4f, SpriteEffects.None, 0.09541f);
  }
}
