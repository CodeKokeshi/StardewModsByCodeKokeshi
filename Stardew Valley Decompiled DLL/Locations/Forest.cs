// Decompiled with JetBrains decompiler
// Type: StardewValley.Locations.Forest
// Assembly: Stardew Valley, Version=1.6.15.24356, Culture=neutral, PublicKeyToken=null
// MVID: AA1F513A-94F0-4EF6-A35F-2D5B4A3721BF
// Assembly location: D:\SteamLibrary\steamapps\common\Stardew Valley\Stardew Valley.dll
// XML documentation location: D:\SteamLibrary\steamapps\common\Stardew Valley\Stardew Valley.xml

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Netcode;
using StardewValley.Characters;
using StardewValley.Extensions;
using StardewValley.Network;
using StardewValley.Network.NetEvents;
using StardewValley.TerrainFeatures;
using System;
using System.Xml.Serialization;
using xTile;
using xTile.Dimensions;

#nullable disable
namespace StardewValley.Locations;

public class Forest : GameLocation
{
  public const string raccoonStumpCheckFlag = "checkedRaccoonStump";
  public const string raccoontreeFlag = "raccoonTreeFallen";
  /// <summary>The tile area in the forest map in which to spawn cows and respawn grass.</summary>
  public Microsoft.Xna.Framework.Rectangle MarnieLivestockArea = new Microsoft.Xna.Framework.Rectangle(94, 17, 10, 5);
  [XmlIgnore]
  public readonly NetObjectList<FarmAnimal> marniesLivestock = new NetObjectList<FarmAnimal>();
  [XmlIgnore]
  public readonly NetList<Microsoft.Xna.Framework.Rectangle, NetRectangle> travelingMerchantBounds = new NetList<Microsoft.Xna.Framework.Rectangle, NetRectangle>();
  [XmlIgnore]
  public readonly NetBool netTravelingMerchantDay = new NetBool(false);
  /// <summary>Obsolete. This is only kept to preserve data from old save files. The log blocking access to the Secret Woods is now in <see cref="F:StardewValley.GameLocation.resourceClumps" />.</summary>
  [XmlElement("log")]
  public ResourceClump obsolete_log;
  [XmlElement("stumpFixed")]
  public readonly NetBool stumpFixed = new NetBool();
  [XmlIgnore]
  public NetMutex derbyMutex = new NetMutex();
  private int numRaccoonBabies = -1;
  private int chimneyTimer = 500;
  private bool hasShownCCUpgrade;
  private Microsoft.Xna.Framework.Rectangle hatterSource = new Microsoft.Xna.Framework.Rectangle(600, 1957, 64 /*0x40*/, 32 /*0x20*/);
  private Vector2 hatterPos = new Vector2(2056f, 6016f);

  [XmlIgnore]
  public bool travelingMerchantDay
  {
    get => this.netTravelingMerchantDay.Value;
    set => this.netTravelingMerchantDay.Value = value;
  }

  public Forest()
  {
  }

  public Forest(string map, string name)
    : base(map, name)
  {
    this.marniesLivestock.Add(new FarmAnimal("Dairy Cow", Game1.multiplayer.getNewID(), -1L));
    this.marniesLivestock.Add(new FarmAnimal("Dairy Cow", Game1.multiplayer.getNewID(), -1L));
    this.marniesLivestock[0].Position = new Vector2((float) ((this.MarnieLivestockArea.X + 4) * 64 /*0x40*/), (float) ((this.MarnieLivestockArea.Y + 3) * 64 /*0x40*/));
    this.marniesLivestock[1].Position = new Vector2((float) ((this.MarnieLivestockArea.X + 7) * 64 /*0x40*/), (float) ((this.MarnieLivestockArea.Y + 3) * 64 /*0x40*/));
    this.resourceClumps.Add(new ResourceClump(602, 2, 2, new Vector2(1f, 6f)));
  }

  protected override void initNetFields()
  {
    base.initNetFields();
    this.NetFields.AddField((INetSerializable) this.marniesLivestock, "marniesLivestock").AddField((INetSerializable) this.travelingMerchantBounds, "travelingMerchantBounds").AddField((INetSerializable) this.netTravelingMerchantDay, "netTravelingMerchantDay").AddField((INetSerializable) this.stumpFixed, "stumpFixed").AddField((INetSerializable) this.derbyMutex.NetFields, "derbyMutex.NetFields");
    this.stumpFixed.fieldChangeEvent += (FieldChange<NetBool, bool>) ((f, oldValue, newValue) =>
    {
      if (!newValue || this.mapPath.Value == null)
        return;
      Forest.fixStump((GameLocation) this);
    });
    this.characters.OnValueAdded += (NetCollection<NPC>.ContentsChangeEvent) (newCharacter => this.adjustDerbyFisherman(newCharacter));
  }

  /// <inheritdoc />
  public override void seasonUpdate(bool onLoad = false)
  {
    base.seasonUpdate(onLoad);
    if (onLoad || Game1.season != Season.Spring)
      return;
    Microsoft.Xna.Framework.Rectangle marnieLivestockArea = this.MarnieLivestockArea;
    this.loadPathsLayerObjectsInArea(marnieLivestockArea.X, marnieLivestockArea.Y, marnieLivestockArea.Width, marnieLivestockArea.Height);
  }

  private void adjustDerbyFisherman(NPC npc)
  {
    if (npc.name.Equals((object) "derby_contestent0"))
    {
      npc.drawOffset = new Vector2(0.0f, 96f);
      npc.shouldShadowBeOffset = true;
      if (npc.Sprite?.Texture == null)
        npc.Sprite = new AnimatedSprite("Characters\\Assorted_Fishermen", 0, 16 /*0x10*/, 64 /*0x40*/);
      npc.SimpleNonVillagerNPC = true;
      npc.HideShadow = true;
      npc.Breather = false;
    }
    if (npc.name.Equals((object) "derby_contestent1"))
    {
      npc.drawOffset = new Vector2(0.0f, 96f);
      npc.shouldShadowBeOffset = true;
      if (npc.Sprite?.Texture == null)
        npc.Sprite = new AnimatedSprite("Characters\\Assorted_Fishermen", 2, 16 /*0x10*/, 64 /*0x40*/);
      npc.Sprite.CurrentFrame = 2;
      npc.SimpleNonVillagerNPC = true;
      npc.HideShadow = true;
      npc.Breather = false;
    }
    if (npc.name.Equals((object) "derby_contestent2"))
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
    if (npc.name.Equals((object) "derby_contestent3"))
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
    if (npc.name.Equals((object) "derby_contestent4"))
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
    if (npc.name.Equals((object) "derby_contestent5"))
    {
      if (npc.Sprite?.Texture == null)
        npc.Sprite = new AnimatedSprite("Characters\\Assorted_Fishermen", 8, 32 /*0x20*/, 32 /*0x20*/);
      npc.Sprite.CurrentFrame = 8;
      npc.shouldShadowBeOffset = true;
      npc.SimpleNonVillagerNPC = true;
      npc.HideShadow = true;
      npc.Breather = false;
    }
    if (npc.name.Equals((object) "derby_contestent6"))
    {
      if (npc.Sprite?.Texture == null)
        npc.Sprite = new AnimatedSprite("Characters\\Assorted_Fishermen", 9, 32 /*0x20*/, 32 /*0x20*/);
      npc.Sprite.CurrentFrame = 9;
      npc.shouldShadowBeOffset = true;
      npc.SimpleNonVillagerNPC = true;
      npc.HideShadow = true;
      npc.Breather = false;
    }
    if (npc.name.Equals((object) "derby_contestent7"))
    {
      if (npc.Sprite?.Texture == null)
        npc.Sprite = new AnimatedSprite("Characters\\Assorted_Fishermen", 10, 32 /*0x20*/, 32 /*0x20*/);
      npc.Sprite.CurrentFrame = 10;
      npc.shouldShadowBeOffset = true;
      npc.SimpleNonVillagerNPC = true;
      npc.HideShadow = true;
      npc.Breather = false;
    }
    if (npc.name.Equals((object) "derby_contestent8"))
    {
      if (npc.Sprite?.Texture == null)
        npc.Sprite = new AnimatedSprite("Characters\\Assorted_Fishermen", 11, 32 /*0x20*/, 32 /*0x20*/);
      npc.Sprite.CurrentFrame = 11;
      npc.shouldShadowBeOffset = true;
      npc.SimpleNonVillagerNPC = true;
      npc.HideShadow = true;
      npc.Breather = false;
    }
    if (!npc.name.Equals((object) "derby_contestent9"))
      return;
    if (npc.Sprite?.Texture == null)
      npc.Sprite = new AnimatedSprite("Characters\\Assorted_Fishermen", 12, 32 /*0x20*/, 32 /*0x20*/);
    npc.Sprite.CurrentFrame = 12;
    npc.shouldShadowBeOffset = true;
    npc.SimpleNonVillagerNPC = true;
    npc.HideShadow = true;
    npc.Breather = false;
  }

  public static void fixStump(GameLocation location)
  {
    if (!NetWorldState.checkAnywhereForWorldStateID("forestStumpFixed"))
      NetWorldState.addWorldStateIDEverywhere("forestStumpFixed");
    location.updateMap();
    for (int x = 52; x < 60; ++x)
    {
      for (int y = 0; y < 2; ++y)
        location.removeTile(x, y, "AlwaysFront");
    }
    location.ApplyMapOverride("Forest_RaccoonHouse", destination_rect: new Microsoft.Xna.Framework.Rectangle?(new Microsoft.Xna.Framework.Rectangle(53, 2, 7, 6)));
    location.largeTerrainFeatures.Remove(location.getLargeTerrainFeatureAt(55, 10));
    location.largeTerrainFeatures.Remove(location.getLargeTerrainFeatureAt(56, 13));
    location.largeTerrainFeatures.Remove(location.getLargeTerrainFeatureAt(61, 10));
    Game1.currentLightSources.Add(new LightSource("Forest_RaccoonHouse", 4, new Vector2(3540f, 357f), 0.75f, Color.Black * 0.6f, onlyLocation: location.NameOrUniqueName));
  }

  public void removeSewerTrash()
  {
    this.ApplyMapOverride("Forest-SewerClean", destination_rect: new Microsoft.Xna.Framework.Rectangle?(new Microsoft.Xna.Framework.Rectangle(83, 97, 24, 12)));
    this.removeMapTile(43, 106, "Buildings");
    this.removeMapTile(17, 106, "Buildings");
    this.removeMapTile(13, 105, "Buildings");
    this.removeMapTile(4, 85, "Buildings");
    this.removeMapTile(2, 85, "Buildings");
  }

  protected override void resetLocalState()
  {
    base.resetLocalState();
    this.addFrog();
    if (Game1.year > 2 && this.getCharacterFromName("TrashBear") != null && NetWorldState.checkAnywhereForWorldStateID("trashBearDone"))
      this.characters.Remove(this.getCharacterFromName("TrashBear"));
    if (this.numRaccoonBabies == -1)
    {
      this.numRaccoonBabies = Game1.netWorldState.Value.TimesFedRaccoons - 1;
      if (Game1.netWorldState.Value.Date.TotalDays - Game1.netWorldState.Value.DaysPlayedWhenLastRaccoonBundleWasFinished < 7)
        --this.numRaccoonBabies;
      if (this.numRaccoonBabies < 0)
        this.numRaccoonBabies = 0;
      if (this.numRaccoonBabies >= 8)
        Game1.getAchievement(39);
    }
    if (!Game1.eventUp && !Game1.player.mailReceived.Contains("seenRaccoonFinishEvent") && this.numRaccoonBabies >= 8 && !Game1.isRaining && !Game1.isSnowing && Game1.timeOfDay < Game1.getStartingToGetDarkTime((GameLocation) this))
    {
      Game1.player.mailReceived.Add("seenRaccoonFinishEvent");
      this.startEvent(new StardewValley.Event($"none/-10000 -1000/farmer 56 15 0/skippable/specificTemporarySprite raccoonCircle/viewport 56 6 true/pause 3000/specificTemporarySprite raccoonSong/playSound raccoonSong/precisePause 9505/specificTemporarySprite raccoonCircle2/precisePause 9405/specificTemporarySprite raccoonbutterflies/precisePause 9505/specificTemporarySprite raccoondance1/precisePause 9505/specificTemporarySprite raccoondance2/pause 6000/globalfade .003 false/viewport -10000 -1000/spriteText 6 \"{Game1.content.LoadString("Strings\\1_6_Strings:RaccoonFinal")}\"/pause 500/end"));
    }
    if (Game1.stats.DaysPlayed > 3U)
    {
      Random daySaveRandom = Utility.CreateDaySaveRandom();
      int endTime = Utility.ModifyTime(1920, daySaveRandom.Next(390));
      int num = Utility.CalculateMinutesBetweenTimes(Game1.timeOfDay, endTime) * Game1.realMilliSecondsPerGameMinute;
      if (num > 0)
      {
        if (daySaveRandom.NextDouble() < 0.5)
          this.temporarySprites.Add(new TemporaryAnimatedSprite("Characters\\asldkfjsquaskutanfsldk", new Microsoft.Xna.Framework.Rectangle(0, 0, 32 /*0x20*/, 48 /*0x30*/), new Vector2(146f, 3851f), true, 0.0f, Color.White)
          {
            animationLength = 8,
            totalNumberOfLoops = 99,
            interval = 100f,
            motion = new Vector2(-4f, 0.0f),
            scale = 5.5f,
            delayBeforeAnimationStart = num
          });
        else
          this.temporarySprites.Add(new TemporaryAnimatedSprite("Characters\\asldkfjsquaskutanfsldk", new Microsoft.Xna.Framework.Rectangle(0, 0, 32 /*0x20*/, 48 /*0x30*/), new Vector2(318f, 1129f), true, 0.0f, Color.White)
          {
            animationLength = 8,
            totalNumberOfLoops = 99,
            interval = 100f,
            motion = new Vector2(-4f, 0.0f),
            scale = 5.5f,
            delayBeforeAnimationStart = num
          });
      }
    }
    if (!Utility.doesAnyFarmerHaveMail("asdlkjfg1") || Utility.CreateDaySaveRandom(105.0).NextDouble() >= 0.03)
      return;
    this.temporarySprites.Add(new TemporaryAnimatedSprite("LooseSprites\\Cursors_1_6", new Microsoft.Xna.Framework.Rectangle(495, 412, 16 /*0x10*/, 16 /*0x10*/), new Vector2(4f, 90f) * 64f, false, 0.0f, Color.White * 0.66f)
    {
      scale = 4f,
      layerDepth = 0.0f
    });
    this.temporarySprites.Add(new TemporaryAnimatedSprite("LooseSprites\\Cursors_1_6", new Microsoft.Xna.Framework.Rectangle(495, 412, 16 /*0x10*/, 16 /*0x10*/), new Vector2(3.2f, 89f) * 64f, true, 0.0f, Color.White)
    {
      scale = 4f,
      layerDepth = 0.0f
    });
    this.temporarySprites.Add(new TemporaryAnimatedSprite("LooseSprites\\Cursors_1_6", new Microsoft.Xna.Framework.Rectangle(495, 412, 16 /*0x10*/, 16 /*0x10*/), new Vector2(4f, 88f) * 64f, false, 0.0f, Color.White)
    {
      scale = 4f,
      layerDepth = 0.0f
    });
    this.temporarySprites.Add(new TemporaryAnimatedSprite("LooseSprites\\Cursors_1_6", new Microsoft.Xna.Framework.Rectangle(495, 412, 16 /*0x10*/, 16 /*0x10*/), new Vector2(3f, 87f) * 64f, true, 0.0f, Color.White * 0.66f)
    {
      scale = 4f,
      layerDepth = 0.0f
    });
  }

  public override void cleanupBeforePlayerExit()
  {
    base.cleanupBeforePlayerExit();
    this.derbyMutex.ReleaseLock();
  }

  public override void MakeMapModifications(bool force = false)
  {
    base.MakeMapModifications(force);
    if (force)
      this.hasShownCCUpgrade = false;
    if (this.stumpFixed.Value)
      Forest.fixStump((GameLocation) this);
    else if (Game1.MasterPlayer.mailReceived.Contains("raccoonTreeFallen"))
    {
      for (int x = 52; x < 60; ++x)
      {
        for (int y = 0; y < 2; ++y)
          this.removeTile(x, y, "AlwaysFront");
      }
      this.ApplyMapOverride("Forest_RaccoonStump", destination_rect: new Microsoft.Xna.Framework.Rectangle?(new Microsoft.Xna.Framework.Rectangle(53, 2, 7, 6)));
    }
    if (NetWorldState.checkAnywhereForWorldStateID("trashBearDone"))
      this.removeSewerTrash();
    if (Game1.MasterPlayer.mailReceived.Contains("communityUpgradeShortcuts"))
      this.showCommunityUpgradeShortcuts();
    if (Game1.IsSummer && Game1.dayOfMonth >= 17 && Game1.dayOfMonth <= 19)
      this.ApplyMapOverride(Game1.game1.xTileContent.Load<Map>("Maps\\Forest_FishingDerbySign"), "Forest_FishingDerbySign", dest_rect: new Microsoft.Xna.Framework.Rectangle?(new Microsoft.Xna.Framework.Rectangle(69, 44, 2, 3)), perTileCustomAction: new Action<Point>(((GameLocation) this).cleanUpTileForMapOverride));
    else if (this._appliedMapOverrides.Contains("Forest_FishingDerbySign"))
    {
      this.ApplyMapOverride("Forest_FishingDerbySign_Revert", destination_rect: new Microsoft.Xna.Framework.Rectangle?(new Microsoft.Xna.Framework.Rectangle(69, 44, 2, 3)));
      this._appliedMapOverrides.Remove("Forest_FishingDerbySign");
      this._appliedMapOverrides.Remove("Forest_FishingDerbySign_Revert");
    }
    if (Game1.IsSummer && Game1.dayOfMonth >= 20 && Game1.dayOfMonth <= 21)
    {
      if (this.getCharacterFromName("derby_contestent0") == null && (Game1.IsMasterGame || !Game1.player.sleptInTemporaryBed.Value))
        this.derbyMutex.RequestLock((Action) (() =>
        {
          if (this.getCharacterFromName("derby_contestent0") == null)
          {
            if (this.checkForTerrainFeaturesAndObjectsButDestroyNonPlayerItems(66, 50))
            {
              NetCollection<NPC> characters = this.characters;
              characters.Add(new NPC(new AnimatedSprite("Characters\\Assorted_Fishermen", 0, 16 /*0x10*/, 64 /*0x40*/), new Vector2(66f, 50f) * 64f, -1, "derby_contestent0")
              {
                Breather = false,
                HideShadow = true,
                drawOffset = new Vector2(0.0f, 96f),
                shouldShadowBeOffset = true,
                SimpleNonVillagerNPC = true
              });
            }
            if (this.checkForTerrainFeaturesAndObjectsButDestroyNonPlayerItems(69, 50))
            {
              NetCollection<NPC> characters = this.characters;
              characters.Add(new NPC(new AnimatedSprite("Characters\\Assorted_Fishermen", 2, 16 /*0x10*/, 64 /*0x40*/), new Vector2(69f, 50f) * 64f, -1, "derby_contestent1")
              {
                Breather = false,
                HideShadow = true,
                drawOffset = new Vector2(0.0f, 96f),
                shouldShadowBeOffset = true,
                SimpleNonVillagerNPC = true
              });
            }
            if (this.checkForTerrainFeaturesAndObjectsButDestroyNonPlayerItems(74, 50))
            {
              NetCollection<NPC> characters = this.characters;
              characters.Add(new NPC(new AnimatedSprite("Characters\\Assorted_Fishermen", 3, 16 /*0x10*/, 64 /*0x40*/), new Vector2(74f, 50f) * 64f, -1, "derby_contestent2")
              {
                Breather = false,
                HideShadow = true,
                drawOffset = new Vector2(0.0f, 96f),
                shouldShadowBeOffset = true,
                SimpleNonVillagerNPC = true
              });
            }
            if (this.checkForTerrainFeaturesAndObjectsButDestroyNonPlayerItems(43, 59))
            {
              NetCollection<NPC> characters = this.characters;
              characters.Add(new NPC(new AnimatedSprite("Characters\\Assorted_Fishermen", 1, 16 /*0x10*/, 64 /*0x40*/), new Vector2(43f, 59f) * 64f, -1, "derby_contestent3")
              {
                Breather = false,
                HideShadow = true,
                drawOffset = new Vector2(0.0f, 96f),
                shouldShadowBeOffset = true,
                SimpleNonVillagerNPC = true
              });
            }
            if (this.checkForTerrainFeaturesAndObjectsButDestroyNonPlayerItems(84, 40) && this.checkForTerrainFeaturesAndObjectsButDestroyNonPlayerItems(85, 40))
            {
              NetCollection<NPC> characters = this.characters;
              characters.Add(new NPC(new AnimatedSprite("Characters\\Assorted_Fishermen", 2, 32 /*0x20*/, 64 /*0x40*/), new Vector2(84f, 40f) * 64f, -1, "derby_contestent4")
              {
                Breather = false,
                HideShadow = true,
                drawOffset = new Vector2(0.0f, 96f),
                shouldShadowBeOffset = true,
                SimpleNonVillagerNPC = true
              });
            }
            if (this.checkForTerrainFeaturesAndObjectsButDestroyNonPlayerItems(88, 49))
            {
              NetCollection<NPC> characters = this.characters;
              characters.Add(new NPC(new AnimatedSprite("Characters\\Assorted_Fishermen", 8, 32 /*0x20*/, 32 /*0x20*/), new Vector2(88f, 49f) * 64f, -1, "derby_contestent5")
              {
                Breather = false,
                HideShadow = true,
                drawOffset = new Vector2(0.0f, 0.0f),
                shouldShadowBeOffset = true,
                SimpleNonVillagerNPC = true
              });
            }
            if (this.checkForTerrainFeaturesAndObjectsButDestroyNonPlayerItems(92, 54))
            {
              NetCollection<NPC> characters = this.characters;
              characters.Add(new NPC(new AnimatedSprite("Characters\\Assorted_Fishermen", 9, 32 /*0x20*/, 32 /*0x20*/), new Vector2(91f, 54f) * 64f, -1, "derby_contestent6")
              {
                Breather = false,
                HideShadow = true,
                drawOffset = new Vector2(0.0f, 0.0f),
                shouldShadowBeOffset = true,
                SimpleNonVillagerNPC = true
              });
            }
            if (this.checkForTerrainFeaturesAndObjectsButDestroyNonPlayerItems(20, 73))
            {
              NetCollection<NPC> characters = this.characters;
              characters.Add(new NPC(new AnimatedSprite("Characters\\Assorted_Fishermen", 10, 32 /*0x20*/, 32 /*0x20*/), new Vector2(20f, 73f) * 64f, -1, "derby_contestent7")
              {
                Breather = false,
                HideShadow = true,
                drawOffset = new Vector2(0.0f, 0.0f),
                shouldShadowBeOffset = true,
                SimpleNonVillagerNPC = true
              });
            }
            if (this.checkForTerrainFeaturesAndObjectsButDestroyNonPlayerItems(77, 48 /*0x30*/))
            {
              NetCollection<NPC> characters = this.characters;
              characters.Add(new NPC(new AnimatedSprite("Characters\\Assorted_Fishermen", 11, 32 /*0x20*/, 32 /*0x20*/), new Vector2(76f, 48f) * 64f, -1, "derby_contestent8")
              {
                Breather = false,
                HideShadow = true,
                drawOffset = new Vector2(0.0f, 0.0f),
                shouldShadowBeOffset = true,
                SimpleNonVillagerNPC = true
              });
            }
            if (this.checkForTerrainFeaturesAndObjectsButDestroyNonPlayerItems(83, 51))
            {
              NetCollection<NPC> characters = this.characters;
              characters.Add(new NPC(new AnimatedSprite("Characters\\Assorted_Fishermen", 12, 32 /*0x20*/, 32 /*0x20*/), new Vector2(82f, 51f) * 64f, -1, "derby_contestent9")
              {
                Breather = false,
                HideShadow = true,
                drawOffset = new Vector2(0.0f, 0.0f),
                shouldShadowBeOffset = true,
                SimpleNonVillagerNPC = true
              });
            }
          }
          this.derbyMutex.ReleaseLock();
        }));
      this.ApplyMapOverride(Game1.game1.xTileContent.Load<Map>("Maps\\Forest_FishingDerby"), "Forest_FishingDerby", dest_rect: new Microsoft.Xna.Framework.Rectangle?(new Microsoft.Xna.Framework.Rectangle(63 /*0x3F*/, 43, 11, 5)), perTileCustomAction: new Action<Point>(((GameLocation) this).cleanUpTileForMapOverride));
      Game1.currentLightSources.Add(new LightSource("FishingDerby_1", 1, new Vector2(4596f, 2968f), 3f, onlyLocation: this.NameOrUniqueName));
      Game1.currentLightSources.Add(new LightSource("FishingDerby_2", 1, new Vector2(4324f, 3044f), 3f, onlyLocation: this.NameOrUniqueName));
    }
    else
    {
      if (!this._appliedMapOverrides.Contains("Forest_FishingDerby") && !this.hasTileAt(63 /*0x3F*/, 47, "Buildings"))
        return;
      this.ApplyMapOverride("Forest_FishingDerby_Revert", destination_rect: new Microsoft.Xna.Framework.Rectangle?(new Microsoft.Xna.Framework.Rectangle(63 /*0x3F*/, 43, 11, 5)));
      this._appliedMapOverrides.Remove("Forest_FishingDerby");
      this._appliedMapOverrides.Remove("Forest_FishingDerby_Revert");
      this.characters.RemoveWhere((Func<NPC, bool>) (npc => npc.Name.StartsWith("derby_contestent")));
    }
  }

  private void showCommunityUpgradeShortcuts()
  {
    if (this.hasShownCCUpgrade)
      return;
    this.removeTile(119, 36, "Buildings");
    LargeTerrainFeature largeTerrainFeature1 = (LargeTerrainFeature) null;
    foreach (LargeTerrainFeature largeTerrainFeature2 in this.largeTerrainFeatures)
    {
      if (largeTerrainFeature2.Tile == new Vector2(119f, 35f))
      {
        largeTerrainFeature1 = largeTerrainFeature2;
        break;
      }
    }
    if (largeTerrainFeature1 != null)
      this.largeTerrainFeatures.Remove(largeTerrainFeature1);
    this.hasShownCCUpgrade = true;
    this.warps.Add(new Warp(120, 35, "Beach", 0, 6, false));
    this.warps.Add(new Warp(120, 36, "Beach", 0, 6, false));
  }

  protected override void resetSharedState()
  {
    base.resetSharedState();
    if (this.ShouldTravelingMerchantVisitToday())
    {
      if (!this.travelingMerchantDay)
      {
        this.travelingMerchantDay = true;
        Point merchantCartTile = this.GetTravelingMerchantCartTile();
        this.travelingMerchantBounds.Clear();
        this.travelingMerchantBounds.Add(new Microsoft.Xna.Framework.Rectangle(merchantCartTile.X * 64 /*0x40*/, merchantCartTile.Y * 64 /*0x40*/, 492, 116));
        this.travelingMerchantBounds.Add(new Microsoft.Xna.Framework.Rectangle(merchantCartTile.X * 64 /*0x40*/ + 180, merchantCartTile.Y * 64 /*0x40*/ + 104, 76, 48 /*0x30*/));
        this.travelingMerchantBounds.Add(new Microsoft.Xna.Framework.Rectangle(merchantCartTile.X * 64 /*0x40*/ + 340, merchantCartTile.Y * 64 /*0x40*/ + 104, 104, 48 /*0x30*/));
        foreach (Microsoft.Xna.Framework.Rectangle travelingMerchantBound in this.travelingMerchantBounds)
          Utility.clearObjectsInArea(travelingMerchantBound, (GameLocation) this);
      }
    }
    else
    {
      this.travelingMerchantDay = false;
      this.travelingMerchantBounds.Clear();
    }
    if (Game1.year > 2 && !this.IsRainingHere() && !Utility.isFestivalDay() && this.getCharacterFromName("TrashBear") == null && !NetWorldState.checkAnywhereForWorldStateID("trashBearDone"))
      this.characters.Add((NPC) new TrashBear());
    if (!Game1.MasterPlayer.mailReceived.Contains("raccoonMovedIn"))
      return;
    if (this.getCharacterFromName("Raccoon") == null)
      this.characters.Add((NPC) new Raccoon(false));
    if (this.getCharacterFromName("MrsRaccoon") != null || Game1.netWorldState.Value.TimesFedRaccoons <= 1 && (Game1.netWorldState.Value.DaysPlayedWhenLastRaccoonBundleWasFinished == 0 || Game1.netWorldState.Value.Date.TotalDays - Game1.netWorldState.Value.DaysPlayedWhenLastRaccoonBundleWasFinished < 7))
      return;
    this.characters.Add((NPC) new Raccoon(true));
  }

  public static bool isWizardHouseUnlocked()
  {
    if (Game1.player.mailReceived.Contains("wizardJunimoNote") || Game1.MasterPlayer.mailReceived.Contains("JojaMember"))
      return true;
    int num1 = Game1.MasterPlayer.mailReceived.Contains("ccFishTank") ? 1 : 0;
    bool flag1 = Game1.MasterPlayer.mailReceived.Contains("ccBulletin");
    bool flag2 = Game1.MasterPlayer.mailReceived.Contains("ccPantry");
    bool flag3 = Game1.MasterPlayer.mailReceived.Contains("ccVault");
    bool flag4 = Game1.MasterPlayer.mailReceived.Contains("ccBoilerRoom");
    bool flag5 = Game1.MasterPlayer.mailReceived.Contains("ccCraftsRoom");
    int num2 = flag1 ? 1 : 0;
    return (num1 & num2 & (flag2 ? 1 : 0) & (flag3 ? 1 : 0) & (flag4 ? 1 : 0) & (flag5 ? 1 : 0)) != 0;
  }

  /// <summary>Get whether the traveling cart should visit the forest today.</summary>
  public bool ShouldTravelingMerchantVisitToday() => Game1.dayOfMonth % 7 % 5 == 0;

  /// <summary>Get the tile coordinates for the top-left corner of the traveling cart's bounding area.</summary>
  public Point GetTravelingMerchantCartTile()
  {
    Point parsed;
    return !this.TryGetMapPropertyAs("TravelingCartPosition", out parsed) ? new Point(23, 10) : parsed;
  }

  public override bool checkAction(Location tileLocation, xTile.Dimensions.Rectangle viewport, Farmer who)
  {
    int tileIndexAt = this.getTileIndexAt(tileLocation, "Buildings", "outdoors");
    if (tileIndexAt == 901 && !Forest.isWizardHouseUnlocked())
    {
      Game1.drawObjectDialogue(Game1.content.LoadString("Strings\\Locations:Forest_WizardTower_Locked"));
      return false;
    }
    if (base.checkAction(tileLocation, viewport, who))
      return true;
    switch (tileIndexAt)
    {
      case 1394:
        if (who.mailReceived.Contains("OpenedSewer"))
        {
          Game1.warpFarmer("Sewer", 3, 48 /*0x30*/, 0);
          this.playSound("openChest");
          break;
        }
        if (who.hasRustyKey)
        {
          this.playSound("openBox");
          Game1.drawObjectDialogue(Game1.parseText(Game1.content.LoadString("Strings\\Locations:Forest_OpenedSewer")));
          who.mailReceived.Add("OpenedSewer");
          break;
        }
        Game1.drawObjectDialogue(Game1.content.LoadString("Strings\\Locations:LockedDoor"));
        break;
      case 1972:
        if (who.achievements.Count > 0)
        {
          Utility.TryOpenShopMenu("HatMouse", "HatMouse");
          break;
        }
        Game1.drawObjectDialogue(Game1.content.LoadString("Strings\\Locations:Forest_HatMouseStore_Abandoned"));
        break;
    }
    if (this.travelingMerchantDay && Game1.timeOfDay < 2000)
    {
      Point merchantCartTile = this.GetTravelingMerchantCartTile();
      if (tileLocation.X == merchantCartTile.X + 4 && tileLocation.Y == merchantCartTile.Y + 1)
      {
        Utility.TryOpenShopMenu("Traveler", (string) null);
        return true;
      }
      if (tileLocation.X == merchantCartTile.X && tileLocation.Y == merchantCartTile.Y + 1)
      {
        this.playSound("pig");
        return true;
      }
    }
    return false;
  }

  public override bool isCollidingPosition(
    Microsoft.Xna.Framework.Rectangle position,
    xTile.Dimensions.Rectangle viewport,
    bool isFarmer,
    int damagesFarmer,
    bool glider,
    Character character,
    bool pathfinding,
    bool projectile = false,
    bool ignoreCharacterRequirement = false,
    bool skipCollisionEffects = false)
  {
    if (this.travelingMerchantBounds != null)
    {
      foreach (Microsoft.Xna.Framework.Rectangle travelingMerchantBound in this.travelingMerchantBounds)
      {
        if (position.Intersects(travelingMerchantBound))
          return true;
      }
    }
    return base.isCollidingPosition(position, viewport, isFarmer, damagesFarmer, glider, character, pathfinding, projectile, ignoreCharacterRequirement);
  }

  public override bool isTilePlaceable(Vector2 v, bool itemIsPassable = false)
  {
    if (this.travelingMerchantBounds != null)
    {
      Microsoft.Xna.Framework.Rectangle rectangle = new Microsoft.Xna.Framework.Rectangle((int) v.X * 64 /*0x40*/, (int) v.Y * 64 /*0x40*/, 64 /*0x40*/, 64 /*0x40*/);
      foreach (Microsoft.Xna.Framework.Rectangle travelingMerchantBound in this.travelingMerchantBounds)
      {
        if (rectangle.Intersects(travelingMerchantBound))
          return false;
      }
    }
    return base.isTilePlaceable(v, itemIsPassable);
  }

  public override void DayUpdate(int dayOfMonth)
  {
    base.DayUpdate(dayOfMonth);
    this.numRaccoonBabies = -1;
    if (Game1.IsMasterGame && this.ShouldTravelingMerchantVisitToday() && Game1.netWorldState.Value.VisitsUntilY1Guarantee >= 0)
      --Game1.netWorldState.Value.VisitsUntilY1Guarantee;
    if (this.IsSpringHere())
    {
      for (int index = 0; index < 7; ++index)
      {
        Vector2 tileLocation = new Vector2((float) Game1.random.Next(70, this.map.Layers[0].LayerWidth - 10), (float) Game1.random.Next(68, this.map.Layers[0].LayerHeight - 15));
        if ((double) tileLocation.Y > 30.0)
        {
          foreach (Vector2 openTile in Utility.recursiveFindOpenTiles((GameLocation) this, tileLocation, 16 /*0x10*/))
          {
            string str = this.doesTileHaveProperty((int) openTile.X, (int) openTile.Y, "Diggable", "Back");
            if (!this.terrainFeatures.ContainsKey(openTile) && str != null && Game1.random.NextDouble() < 1.0 - (double) Vector2.Distance(tileLocation, openTile) * 0.15000000596046448)
              this.terrainFeatures.Add(openTile, (TerrainFeature) new HoeDirt(0, new Crop(true, "1", (int) openTile.X, (int) openTile.Y, (GameLocation) this)));
          }
        }
      }
    }
    if (Game1.year > 2 && this.getCharacterFromName("TrashBear") != null)
      this.characters.Remove(this.getCharacterFromName("TrashBear"));
    if (Game1.IsSummer)
      this.characters.RemoveWhere((Func<NPC, bool>) (npc => npc.Name.StartsWith("derby_contestent")));
    if (!Game1.IsSpring)
      return;
    switch (Game1.dayOfMonth)
    {
      case 17:
        this.objects.TryAdd(new Vector2(52f, 98f), ItemRegistry.Create<StardewValley.Object>("(O)PotOfGold"));
        break;
      case 18:
        if (!(this.objects.GetValueOrDefault(new Vector2(52f, 98f))?.QualifiedItemId == "(O)PotOfGold"))
          break;
        this.objects.Remove(new Vector2(52f, 98f));
        break;
    }
  }

  public override void updateEvenIfFarmerIsntHere(GameTime time, bool ignoreWasUpdatedFlush = false)
  {
    base.updateEvenIfFarmerIsntHere(time, ignoreWasUpdatedFlush);
    this.derbyMutex.Update((GameLocation) this);
  }

  public override void UpdateWhenCurrentLocation(GameTime time)
  {
    base.UpdateWhenCurrentLocation(time);
    foreach (FarmAnimal farmAnimal in (NetList<FarmAnimal, NetRef<FarmAnimal>>) this.marniesLivestock)
      farmAnimal.updateWhenCurrentLocation(time, (GameLocation) this);
    if (Game1.timeOfDay >= 2000)
      return;
    Point merchantCartTile = this.GetTravelingMerchantCartTile();
    if (this.travelingMerchantDay)
    {
      if (Game1.random.NextDouble() < 0.001)
        this.temporarySprites.Add(new TemporaryAnimatedSprite("LooseSprites\\Cursors", new Microsoft.Xna.Framework.Rectangle(99, 1423, 13, 19), new Vector2((float) (merchantCartTile.X * 64 /*0x40*/), (float) (merchantCartTile.Y * 64 /*0x40*/ + 32 /*0x20*/ - 4)), false, 0.0f, Color.White)
        {
          interval = (float) Game1.random.Next(500, 1500),
          layerDepth = 0.07682f,
          scale = 4f
        });
      if (Game1.random.NextDouble() < 0.001)
        this.temporarySprites.Add(new TemporaryAnimatedSprite("LooseSprites\\Cursors", new Microsoft.Xna.Framework.Rectangle(51, 1444, 5, 5), new Vector2((float) (merchantCartTile.X * 64 /*0x40*/ + 32 /*0x20*/ - 4), (float) ((merchantCartTile.Y + 1) * 64 /*0x40*/ + 32 /*0x20*/ + 8)), false, 0.0f, Color.White)
        {
          interval = 500f,
          animationLength = 1,
          layerDepth = 0.07682f,
          scale = 4f
        });
      if (Game1.random.NextDouble() < 0.003)
        this.temporarySprites.Add(new TemporaryAnimatedSprite("LooseSprites\\Cursors", new Microsoft.Xna.Framework.Rectangle(89, 1445, 6, 3), new Vector2((float) ((merchantCartTile.X + 4) * 64 /*0x40*/ + 32 /*0x20*/ + 4), (float) (merchantCartTile.Y * 64 /*0x40*/ + 24)), false, 0.0f, Color.White)
        {
          interval = 50f,
          animationLength = 3,
          pingPong = true,
          totalNumberOfLoops = 1,
          layerDepth = 0.07682f,
          scale = 4f
        });
    }
    this.chimneyTimer -= time.ElapsedGameTime.Milliseconds;
    if (this.chimneyTimer > 0)
      return;
    this.chimneyTimer = this.travelingMerchantDay ? 500 : Game1.random.Next(200, 2000);
    this.temporarySprites.Add(new TemporaryAnimatedSprite("LooseSprites\\Cursors", new Microsoft.Xna.Framework.Rectangle(372, 1956, 10, 10), this.travelingMerchantDay ? new Vector2((float) ((merchantCartTile.X + 6) * 64 /*0x40*/ + 12), (float) ((merchantCartTile.Y - 2) * 64 /*0x40*/ + 12)) : new Vector2(5592f, 608f), false, 1f / 500f, Color.Gray)
    {
      alpha = 0.75f,
      motion = new Vector2(0.0f, -0.5f),
      acceleration = new Vector2(1f / 500f, 0.0f),
      interval = 99999f,
      layerDepth = 1f,
      scale = 3f,
      scaleChange = 0.01f,
      rotationChange = (float) ((double) Game1.random.Next(-5, 6) * 3.1415927410125732 / 256.0)
    });
    if (this.stumpFixed.Value && Game1.MasterPlayer.mailReceived.Contains("raccoonMovedIn"))
      this.temporarySprites.Add(new TemporaryAnimatedSprite("LooseSprites\\Cursors", new Microsoft.Xna.Framework.Rectangle(372, 1956, 10, 10), new Vector2(57.33f, 1.75f) * 64f, false, 1f / 500f, Color.Gray)
      {
        alpha = 0.75f,
        motion = new Vector2(0.0f, -0.5f),
        acceleration = new Vector2(1f / 500f, 0.0f),
        interval = 99999f,
        drawAboveAlwaysFront = true,
        layerDepth = 1f,
        scale = 3f,
        scaleChange = 0.01f,
        rotationChange = (float) ((double) Game1.random.Next(-5, 6) * 3.1415927410125732 / 256.0)
      });
    if (!this.travelingMerchantDay)
      return;
    this.temporarySprites.Add(new TemporaryAnimatedSprite("LooseSprites\\Cursors", new Microsoft.Xna.Framework.Rectangle(225, 1388, 7, 5), new Vector2((float) ((merchantCartTile.X + 6) * 64 /*0x40*/ + 12), (float) ((merchantCartTile.Y - 2) * 64 /*0x40*/ + 24)), false, 0.0f, Color.White)
    {
      interval = (float) (this.chimneyTimer - this.chimneyTimer / 5),
      animationLength = 1,
      layerDepth = 0.99f,
      scale = 4.3f,
      scaleChange = -0.015f
    });
  }

  public override bool performAction(string[] action, Farmer who, Location tileLocation)
  {
    if (Game1.MasterPlayer.mailReceived.Contains("raccoonTreeFallen") && action.Length != 0 && action[0] == "FixRaccoonStump")
    {
      if (who.Items.ContainsId("(O)709", 100))
      {
        this.createQuestionDialogue(Game1.content.LoadString("Strings\\1_6_Strings:FixRaccoonStump_Question"), this.createYesNoResponses(), "ForestStump");
      }
      else
      {
        Game1.drawObjectDialogue(Game1.content.LoadString("Strings\\1_6_Strings:FixRaccoonStump_Hint"));
        if (!who.mailReceived.Contains("checkedRaccoonStump"))
        {
          who.addQuest("134");
          who.mailReceived.Add("checkedRaccoonStump");
        }
      }
    }
    return base.performAction(action, who, tileLocation);
  }

  public override bool answerDialogueAction(string questionAndAnswer, string[] questionParams)
  {
    if (!(questionAndAnswer == "ForestStump_Yes"))
      return base.answerDialogueAction(questionAndAnswer, questionParams);
    Game1.globalFadeToBlack(new Game1.afterFadeFunction(this.fadedForStumpFix));
    Game1.player.Items.ReduceId("(O)709", 100);
    Game1.player.team.RequestSetSimpleFlag(SimpleFlagType.HasQuest, PlayerActionTarget.All, "134", false);
    return true;
  }

  public void fadedForStumpFix()
  {
    Game1.freezeControls = true;
    DelayedAction.playSoundAfterDelay("crafting", 1000);
    DelayedAction.playSoundAfterDelay("crafting", 1500);
    DelayedAction.playSoundAfterDelay("crafting", 2000);
    DelayedAction.playSoundAfterDelay("crafting", 2500);
    DelayedAction.playSoundAfterDelay("axchop", 3000);
    DelayedAction.playSoundAfterDelay("discoverMineral", 3200);
    Game1.viewportFreeze = true;
    Game1.viewport.X = -10000;
    this.stumpFixed.Value = true;
    Game1.pauseThenDoFunction(4000, new Game1.afterFadeFunction(this.doneWithStumpFix));
    Forest.fixStump((GameLocation) this);
    Game1.addMailForTomorrow("raccoonMovedIn", true, true);
  }

  public void doneWithStumpFix()
  {
    Game1.globalFadeToClear((Game1.afterFadeFunction) (() =>
    {
      if (Game1.fadeToBlack)
        return;
      Game1.drawObjectDialogue(Game1.content.LoadString("Strings\\1_6_Strings:FixRaccoonStump_Done"));
    }));
    Game1.viewportFreeze = false;
    Game1.freezeControls = false;
  }

  public override void performTenMinuteUpdate(int timeOfDay)
  {
    base.performTenMinuteUpdate(timeOfDay);
    if (this.travelingMerchantDay && Game1.random.NextDouble() < 0.4)
    {
      this.temporarySprites.Add(new TemporaryAnimatedSprite("LooseSprites\\Cursors", new Microsoft.Xna.Framework.Rectangle(57, 1430, 4, 12), new Vector2(1792f, 656f), false, 0.0f, Color.White)
      {
        interval = 50f,
        animationLength = 10,
        pingPong = true,
        totalNumberOfLoops = 1,
        layerDepth = 0.07682f,
        scale = 4f
      });
      if (Game1.random.NextDouble() < 0.66)
        this.temporarySprites.Add(new TemporaryAnimatedSprite("LooseSprites\\Cursors", new Microsoft.Xna.Framework.Rectangle(89, 1445, 6, 3), new Vector2(1764f, 664f), false, 0.0f, Color.White)
        {
          interval = 50f,
          animationLength = 3,
          pingPong = true,
          totalNumberOfLoops = 1,
          layerDepth = 0.07683001f,
          scale = 4f
        });
    }
    if (!Game1.IsSummer || Game1.dayOfMonth < 20 || Game1.dayOfMonth > 21)
      return;
    Random daySaveRandom = Utility.CreateDaySaveRandom((double) (Game1.timeOfDay * 20));
    NPC characterFromName = this.getCharacterFromName("derby_contestent" + daySaveRandom.Next(10).ToString());
    if (characterFromName == null)
      return;
    characterFromName.shake(600);
    if (!daySaveRandom.NextBool(0.25))
      return;
    int num = daySaveRandom.Next(7);
    characterFromName.showTextAboveHead(Game1.content.LoadString("Strings\\1_6_Strings:FishingDerby_Exclamation" + num.ToString()));
    if (num == 0 || num == 6)
      this.temporarySprites.Add(new TemporaryAnimatedSprite(138, 1500f, 1, 1, characterFromName.Position, false, false, false, 0.0f)
      {
        motion = new Vector2((float) Game1.random.Next(-10, 10) / 10f, -7f),
        acceleration = new Vector2(0.0f, 0.1f),
        alphaFade = 1f / 1000f,
        drawAboveAlwaysFront = true
      });
    characterFromName.jump(4f);
  }

  public override void draw(SpriteBatch spriteBatch)
  {
    base.draw(spriteBatch);
    foreach (Character character in (NetList<FarmAnimal, NetRef<FarmAnimal>>) this.marniesLivestock)
      character.draw(spriteBatch);
    if (this.travelingMerchantDay)
    {
      Point merchantCartTile = this.GetTravelingMerchantCartTile();
      spriteBatch.Draw(Game1.mouseCursors, Game1.GlobalToLocal(new Vector2((float) ((merchantCartTile.X + 1) * 64 /*0x40*/), (float) ((merchantCartTile.Y - 2) * 64 /*0x40*/))), new Microsoft.Xna.Framework.Rectangle?(new Microsoft.Xna.Framework.Rectangle(142, 1382, 109, 70)), Color.White, 0.0f, Vector2.Zero, 4f, SpriteEffects.None, 0.0768f);
      spriteBatch.Draw(Game1.mouseCursors, Game1.GlobalToLocal(new Vector2((float) (merchantCartTile.X * 64 /*0x40*/), (float) (merchantCartTile.Y * 64 /*0x40*/ + 32 /*0x20*/))), new Microsoft.Xna.Framework.Rectangle?(new Microsoft.Xna.Framework.Rectangle(112 /*0x70*/, 1424, 30, 24)), Color.White, 0.0f, Vector2.Zero, 4f, SpriteEffects.None, 0.07681f);
      spriteBatch.Draw(Game1.mouseCursors, Game1.GlobalToLocal(new Vector2((float) ((merchantCartTile.X + 1) * 64 /*0x40*/), (float) ((merchantCartTile.Y + 1) * 64 /*0x40*/ + 32 /*0x20*/ - 8))), new Microsoft.Xna.Framework.Rectangle?(new Microsoft.Xna.Framework.Rectangle(142, 1424, 16 /*0x10*/, 3)), Color.White, 0.0f, Vector2.Zero, 4f, SpriteEffects.None, 0.07682f);
      spriteBatch.Draw(Game1.mouseCursors, Game1.GlobalToLocal(new Vector2((float) ((merchantCartTile.X + 1) * 64 /*0x40*/ + 8), (float) (merchantCartTile.Y * 64 /*0x40*/ - 32 /*0x20*/ - 8))), new Microsoft.Xna.Framework.Rectangle?(new Microsoft.Xna.Framework.Rectangle(71, 1966, 18, 18)), Color.White, 0.0f, Vector2.Zero, 4f, SpriteEffects.None, 0.07678001f);
      spriteBatch.Draw(Game1.mouseCursors, Game1.GlobalToLocal(new Vector2((float) (merchantCartTile.X * 64 /*0x40*/), (float) (merchantCartTile.Y * 64 /*0x40*/ - 32 /*0x20*/))), new Microsoft.Xna.Framework.Rectangle?(new Microsoft.Xna.Framework.Rectangle(167, 1966, 18, 18)), Color.White, 0.0f, Vector2.Zero, 4f, SpriteEffects.None, 0.07678001f);
      if (Game1.timeOfDay >= 2000)
        spriteBatch.Draw(Game1.staminaRect, Game1.GlobalToLocal(Game1.viewport, new Microsoft.Xna.Framework.Rectangle((merchantCartTile.X + 4) * 64 /*0x40*/ + 16 /*0x10*/, merchantCartTile.Y * 64 /*0x40*/, 64 /*0x40*/, 64 /*0x40*/)), new Microsoft.Xna.Framework.Rectangle?(Game1.staminaRect.Bounds), Color.Black, 0.0f, Vector2.Zero, SpriteEffects.None, 0.0768400058f);
    }
    if (Game1.player.achievements.Count > 0)
      spriteBatch.Draw(Game1.mouseCursors, Game1.GlobalToLocal(this.hatterPos), new Microsoft.Xna.Framework.Rectangle?(this.hatterSource), Color.White, 0.0f, Vector2.Zero, 4f, SpriteEffects.None, 0.6016f);
    if (!this.stumpFixed.Value && Game1.MasterPlayer.mailReceived.Contains("raccoonTreeFallen") && !Game1.player.mailReceived.Contains("checkedRaccoonStump"))
    {
      float num = (float) (4.0 * Math.Round(Math.Sin(Game1.currentGameTime.TotalGameTime.TotalMilliseconds / 250.0), 2) - 8.0);
      spriteBatch.Draw(Game1.mouseCursors, Game1.GlobalToLocal(Game1.viewport, new Vector2(3576f, 272f + num)), new Microsoft.Xna.Framework.Rectangle?(new Microsoft.Xna.Framework.Rectangle(141, 465, 20, 24)), Color.White * 0.75f, 0.0f, Vector2.Zero, 4f, SpriteEffects.None, 0.0504009947f);
      spriteBatch.Draw(Game1.mouseCursors, Game1.GlobalToLocal(Game1.viewport, new Vector2(3616f, 312f + num)), new Microsoft.Xna.Framework.Rectangle?(new Microsoft.Xna.Framework.Rectangle(175, 425, 12, 12)), Color.White * 0.75f, 0.0f, new Vector2(6f, 6f), 4f, SpriteEffects.None, 0.050409995f);
    }
    else if (this.numRaccoonBabies > 0)
    {
      for (int index = 0; index < Math.Min(this.numRaccoonBabies, 8); ++index)
      {
        switch (index)
        {
          case 0:
            spriteBatch.Draw(Game1.mouseCursors_1_6, Game1.GlobalToLocal(new Vector2(3706f, 340f)), new Microsoft.Xna.Framework.Rectangle?(new Microsoft.Xna.Framework.Rectangle(213 + (Game1.currentGameTime.TotalGameTime.TotalMilliseconds % 5000.0 < 200.0 ? 10 : 0), 472, 10, 9)), Color.White, 0.0f, new Vector2(5.5f, 9f), 4f, SpriteEffects.None, 0.0448f);
            break;
          case 1:
            spriteBatch.Draw(Game1.mouseCursors_1_6, Game1.GlobalToLocal(new Vector2(54f, 4f) * 64f + new Vector2(8f, -12f)), new Microsoft.Xna.Framework.Rectangle?(new Microsoft.Xna.Framework.Rectangle(235 + (Game1.currentGameTime.TotalGameTime.TotalMilliseconds % 4500.0 < 200.0 ? 9 : 0), 472, 9, 12)), Color.White, 0.0f, Vector2.Zero, 4f, SpriteEffects.FlipHorizontally, 0.0448f);
            break;
          case 2:
            spriteBatch.Draw(Game1.mouseCursors_1_6, Game1.GlobalToLocal(new Vector2(3462f, 433f)), new Microsoft.Xna.Framework.Rectangle?(new Microsoft.Xna.Framework.Rectangle(213 + (Game1.currentGameTime.TotalGameTime.TotalMilliseconds % 6000.0 < 200.0 ? 10 : 0), 472, 10, 9)), Color.White, 0.0f, new Vector2(5.5f, 9f), 4f, SpriteEffects.None, 0.0448f);
            break;
          case 3:
            spriteBatch.Draw(Game1.mouseCursors_1_6, Game1.GlobalToLocal(new Vector2(58f, 4f) * 64f + new Vector2(4f, -20f)), new Microsoft.Xna.Framework.Rectangle?(new Microsoft.Xna.Framework.Rectangle(235 + (Game1.currentGameTime.TotalGameTime.TotalMilliseconds % 4800.0 < 200.0 ? 9 : 0), 472, 9, 12)), Color.White, 0.0f, Vector2.Zero, 4f, SpriteEffects.None, 0.0448f);
            break;
          case 4:
            spriteBatch.Draw(Game1.mouseCursors_1_6, Game1.GlobalToLocal(new Vector2(3770f, 408f)), new Microsoft.Xna.Framework.Rectangle?(new Microsoft.Xna.Framework.Rectangle(213 + (Game1.currentGameTime.TotalGameTime.TotalMilliseconds % 5000.0 < 200.0 ? 10 : 0), 472, 10, 9)), Color.White, 0.0f, new Vector2(5.5f, 9f), 4f, SpriteEffects.None, 0.0448f);
            break;
          case 5:
            spriteBatch.Draw(Game1.mouseCursors_1_6, Game1.GlobalToLocal(new Vector2(55f, 3f) * 64f + new Vector2(12f, 4f)), new Microsoft.Xna.Framework.Rectangle?(new Microsoft.Xna.Framework.Rectangle(213 + (Game1.currentGameTime.TotalGameTime.TotalMilliseconds % 5000.0 < 200.0 ? 10 : 0), 472, 10, 9)), Color.White, 0.0f, new Vector2(5.5f, 9f), 4f, SpriteEffects.None, 0.0064f);
            break;
          case 6:
            spriteBatch.Draw(Game1.mouseCursors_1_6, Game1.GlobalToLocal(new Vector2(56f, 3f) * 64f + new Vector2(40f, -8f)), new Microsoft.Xna.Framework.Rectangle?(new Microsoft.Xna.Framework.Rectangle(213 + (Game1.currentGameTime.TotalGameTime.TotalMilliseconds % 5200.0 < 200.0 ? 10 : 0), 472, 10, 9)), Color.White, 0.0f, new Vector2(5.5f, 9f), 4f, SpriteEffects.None, 0.0064f);
            break;
          case 7:
            spriteBatch.Draw(Game1.mouseCursors_1_6, Game1.GlobalToLocal(new Vector2(58f, 3f) * 64f + new Vector2(-20f, -48f)), new Microsoft.Xna.Framework.Rectangle?(new Microsoft.Xna.Framework.Rectangle(235 + (Game1.currentGameTime.TotalGameTime.TotalMilliseconds % 4600.0 < 200.0 ? 9 : 0), 472, 9, 12)), Color.White, 0.0f, Vector2.Zero, 4f, SpriteEffects.None, 0.0448f);
            break;
        }
      }
    }
    if (!Game1.IsSpring || Game1.dayOfMonth != 17)
      return;
    spriteBatch.Draw(Game1.mouseCursors_1_6, Game1.GlobalToLocal(new Vector2(52f, 97f) * 64f), new Microsoft.Xna.Framework.Rectangle?(new Microsoft.Xna.Framework.Rectangle(257, 108, 136, 116)), Color.White, 0.0f, Vector2.Zero, 4f, SpriteEffects.None, 1f);
  }
}
