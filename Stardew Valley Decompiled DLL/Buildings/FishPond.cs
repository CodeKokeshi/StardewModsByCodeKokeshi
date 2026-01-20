// Decompiled with JetBrains decompiler
// Type: StardewValley.Buildings.FishPond
// Assembly: Stardew Valley, Version=1.6.15.24356, Culture=neutral, PublicKeyToken=null
// MVID: AA1F513A-94F0-4EF6-A35F-2D5B4A3721BF
// Assembly location: D:\SteamLibrary\steamapps\common\Stardew Valley\Stardew Valley.dll
// XML documentation location: D:\SteamLibrary\steamapps\common\Stardew Valley\Stardew Valley.xml

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Netcode;
using StardewValley.Extensions;
using StardewValley.GameData;
using StardewValley.GameData.Buildings;
using StardewValley.GameData.FishPonds;
using StardewValley.Internal;
using StardewValley.ItemTypeDefinitions;
using StardewValley.Menus;
using StardewValley.Network;
using StardewValley.Objects;
using StardewValley.Tools;
using System;
using System.Collections.Generic;
using System.Xml.Serialization;

#nullable disable
namespace StardewValley.Buildings;

public class FishPond : Building
{
  public const int MAXIMUM_OCCUPANCY = 10;
  public static readonly float FISHING_MILLISECONDS = 1000f;
  public static readonly int HARVEST_BASE_EXP = 10;
  public static readonly float HARVEST_OUTPUT_EXP_MULTIPLIER = 0.04f;
  public static readonly int QUEST_BASE_EXP = 20;
  public static readonly float QUEST_SPAWNRATE_EXP_MULTIPIER = 5f;
  public const int NUMBER_OF_NETTING_STYLE_TYPES = 4;
  [XmlArrayItem("int")]
  public readonly NetString fishType = new NetString();
  public readonly NetInt lastUnlockedPopulationGate = new NetInt(0);
  public readonly NetBool hasCompletedRequest = new NetBool(false);
  /// <summary>Whether a player has added a golden cracker to the fish pond.</summary>
  public readonly NetBool goldenAnimalCracker = new NetBool(false);
  /// <summary>Whether a player has added a golden cracker to the fish pond, but it hasn't landed yet.</summary>
  [XmlIgnore]
  public readonly NetBool isPlayingGoldenCrackerAnimation = new NetBool(false);
  public readonly NetRef<StardewValley.Object> sign = new NetRef<StardewValley.Object>();
  public readonly NetColor overrideWaterColor = new NetColor(Color.White);
  public readonly NetRef<Item> output = new NetRef<Item>();
  public readonly NetRef<Item> neededItem = new NetRef<Item>();
  public readonly NetIntDelta neededItemCount = new NetIntDelta(0);
  public readonly NetInt daysSinceSpawn = new NetInt(0);
  public readonly NetInt nettingStyle = new NetInt(0);
  public readonly NetInt seedOffset = new NetInt(0);
  public readonly NetBool hasSpawnedFish = new NetBool(false);
  [XmlIgnore]
  public readonly NetMutex needsMutex = new NetMutex();
  [XmlIgnore]
  protected bool _hasAnimatedSpawnedFish;
  [XmlIgnore]
  protected float _delayUntilFishSilhouetteAdded;
  [XmlIgnore]
  protected int _numberOfFishToJump;
  [XmlIgnore]
  protected float _timeUntilFishHop;
  [XmlIgnore]
  protected StardewValley.Object _fishObject;
  [XmlIgnore]
  public List<PondFishSilhouette> _fishSilhouettes = new List<PondFishSilhouette>();
  [XmlIgnore]
  public List<JumpingFish> _jumpingFish = new List<JumpingFish>();
  [XmlIgnore]
  private readonly NetEvent0 animateHappyFishEvent = new NetEvent0();
  [XmlIgnore]
  public TemporaryAnimatedSpriteList animations = new TemporaryAnimatedSpriteList();
  [XmlIgnore]
  protected FishPondData _fishPondData;

  public FishPond(Vector2 tileLocation)
    : base("Fish Pond", tileLocation)
  {
    this.UpdateMaximumOccupancy();
    this.fadeWhenPlayerIsBehind.Value = false;
    this.Reseed();
  }

  public FishPond()
    : this(Vector2.Zero)
  {
  }

  protected override void initNetFields()
  {
    base.initNetFields();
    this.NetFields.AddField((INetSerializable) this.fishType, "fishType").AddField((INetSerializable) this.output, "output").AddField((INetSerializable) this.daysSinceSpawn, "daysSinceSpawn").AddField((INetSerializable) this.lastUnlockedPopulationGate, "lastUnlockedPopulationGate").AddField((INetSerializable) this.animateHappyFishEvent, "animateHappyFishEvent").AddField((INetSerializable) this.hasCompletedRequest, "hasCompletedRequest").AddField((INetSerializable) this.goldenAnimalCracker, "goldenAnimalCracker").AddField((INetSerializable) this.isPlayingGoldenCrackerAnimation, "isPlayingGoldenCrackerAnimation").AddField((INetSerializable) this.neededItem, "neededItem").AddField((INetSerializable) this.seedOffset, "seedOffset").AddField((INetSerializable) this.hasSpawnedFish, "hasSpawnedFish").AddField((INetSerializable) this.needsMutex.NetFields, "needsMutex.NetFields").AddField((INetSerializable) this.neededItemCount, "neededItemCount").AddField((INetSerializable) this.overrideWaterColor, "overrideWaterColor").AddField((INetSerializable) this.sign, "sign").AddField((INetSerializable) this.nettingStyle, "nettingStyle");
    this.animateHappyFishEvent.onEvent += new NetEvent0.Event(this.AnimateHappyFish);
    this.fishType.fieldChangeVisibleEvent += new FieldChange<NetString, string>(this.OnFishTypeChanged);
  }

  public virtual void OnFishTypeChanged(NetString field, string old_value, string new_value)
  {
    this._fishSilhouettes.Clear();
    this._jumpingFish.Clear();
    this._fishObject = (StardewValley.Object) null;
  }

  public virtual void Reseed() => this.seedOffset.Value = DateTime.UtcNow.Millisecond;

  public List<PondFishSilhouette> GetFishSilhouettes() => this._fishSilhouettes;

  public void UpdateMaximumOccupancy()
  {
    this.GetFishPondData();
    if (this._fishPondData == null)
      return;
    if (this._fishPondData.MaxPopulation > 0)
    {
      this.maxOccupants.Value = this._fishPondData.MaxPopulation;
    }
    else
    {
      for (int index = 1; index <= 10; ++index)
      {
        if (index <= this.lastUnlockedPopulationGate.Value)
        {
          this.maxOccupants.Set(index);
        }
        else
        {
          bool? nullable = this._fishPondData.PopulationGates?.ContainsKey(index);
          if (nullable.HasValue && nullable.GetValueOrDefault())
            break;
          this.maxOccupants.Set(index);
        }
      }
    }
  }

  public FishPondData GetFishPondData()
  {
    FishPondData rawData = FishPond.GetRawData(this.fishType.Value);
    if (rawData == null)
      return (FishPondData) null;
    this._fishPondData = rawData;
    if (this._fishPondData.SpawnTime == -1)
    {
      int price = this.GetFishObject().Price;
      this._fishPondData.SpawnTime = price > 30 ? (price > 80 /*0x50*/ ? (price > 120 ? (price > 250 ? 5 : 4) : 3) : 2) : 1;
    }
    return this._fishPondData;
  }

  /// <summary>Get the data entry matching a fish item ID.</summary>
  /// <param name="itemId">The unqualified fish item ID.</param>
  public static FishPondData GetRawData(string itemId)
  {
    if (itemId == null)
      return (FishPondData) null;
    HashSet<string> baseContextTags = ItemContextTagManager.GetBaseContextTags(itemId);
    if (baseContextTags.Contains("fish_pond_ignore"))
      return (FishPondData) null;
    FishPondData rawData = (FishPondData) null;
    foreach (FishPondData fishPondData in DataLoader.FishPondData(Game1.content))
    {
      int? precedence1 = rawData?.Precedence;
      int precedence2 = fishPondData.Precedence;
      if (!(precedence1.GetValueOrDefault() <= precedence2 & precedence1.HasValue) && ItemContextTagManager.DoAllTagsMatch((IList<string>) fishPondData.RequiredTags, baseContextTags))
        rawData = fishPondData;
    }
    return rawData;
  }

  public Item GetFishProduce(Random random = null)
  {
    if (random == null)
      random = Game1.random;
    FishPondData fishPondData = this.GetFishPondData();
    if (fishPondData == null)
      return (Item) null;
    GameLocation parentLocation = this.GetParentLocation();
    StardewValley.Object fish = this.GetFishObject();
    FishPondReward selectedOutput = (FishPondReward) null;
    foreach (FishPondReward producedItem in fishPondData.ProducedItems)
    {
      int? precedence1 = selectedOutput?.Precedence;
      int precedence2 = producedItem.Precedence;
      if (!(precedence1.GetValueOrDefault() <= precedence2 & precedence1.HasValue) && this.currentOccupants.Value >= producedItem.RequiredPopulation && random.NextBool(producedItem.Chance) && GameStateQuery.CheckConditions(producedItem.Condition, parentLocation, inputItem: (Item) fish))
        selectedOutput = producedItem;
    }
    Item fishProduce = (Item) null;
    if (selectedOutput != null)
      fishProduce = ItemQueryResolver.TryResolveRandomItem((ISpawnItemData) selectedOutput, new ItemQueryContext(parentLocation, (Farmer) null, (Random) null, $"fish pond data '{this.fishType.Value}' > reward '{selectedOutput.Id}'"), formatItemId: (Func<string, string>) (id => !(ItemRegistry.QualifyItemId(selectedOutput.ItemId) == "(O)812") ? id : "FLAVORED_ITEM Roe " + fish.QualifiedItemId), inputItem: (Item) fish);
    if (fishProduce != null)
    {
      if (fishProduce.Name.Contains("Roe"))
      {
        while (random.NextDouble() < 0.2)
          ++fishProduce.Stack;
      }
      if (this.goldenAnimalCracker.Value)
        fishProduce.Stack *= 2;
    }
    return fishProduce;
  }

  public int FishCount => this.currentOccupants.Value;

  private Item CreateFishInstance() => (Item) new StardewValley.Object(this.fishType.Value, 1);

  public override bool doAction(Vector2 tileLocation, Farmer who)
  {
    if (this.daysOfConstructionLeft.Value <= 0 && this.occupiesTile(tileLocation))
    {
      if (who.isMoving())
        Game1.haltAfterCheck = false;
      if (who.ActiveObject != null && this.performActiveObjectDropInAction(who, false))
        return true;
      if (this.output.Value != null)
      {
        Item obj = this.output.Value;
        this.output.Value = (Item) null;
        if (who.addItemToInventoryBool(obj))
        {
          Game1.playSound("coin");
          int num = 0;
          if (obj is StardewValley.Object @object)
            num = (int) ((double) @object.sellToStorePrice(-1L) * (double) FishPond.HARVEST_OUTPUT_EXP_MULTIPLIER);
          who.gainExperience(1, num + FishPond.HARVEST_BASE_EXP);
        }
        else
        {
          this.output.Value = obj;
          Game1.showRedMessage(Game1.content.LoadString("Strings\\StringsFromCSFiles:Crop.cs.588"));
        }
        return true;
      }
      if (who.ActiveObject != null && this.HasUnresolvedNeeds() && who.ActiveObject.QualifiedItemId == this.neededItem.Value.QualifiedItemId)
      {
        if (this.neededItemCount.Value == 1)
          this.showObjectThrownIntoPondAnimation(who, who.ActiveObject, (Action) (() =>
          {
            if (this.neededItemCount.Value > 0)
              return;
            Game1.playSound("jingle1");
          }));
        else
          this.showObjectThrownIntoPondAnimation(who, who.ActiveObject);
        who.reduceActiveItemByOne();
        if (who == Game1.player)
        {
          --this.neededItemCount.Value;
          if (this.neededItemCount.Value <= 0)
          {
            this.needsMutex.RequestLock((Action) (() =>
            {
              this.needsMutex.ReleaseLock();
              this.ResolveNeeds(who);
            }));
            this.neededItemCount.Value = -1;
          }
        }
        if (this.neededItemCount.Value <= 0)
          this.animateHappyFishEvent.Fire();
        return true;
      }
      if (who.ActiveObject != null && (who.ActiveObject.Category == -4 || who.ActiveObject.QualifiedItemId == "(O)393" || who.ActiveObject.QualifiedItemId == "(O)397"))
      {
        if (this.fishType.Value != null)
        {
          if (!this.isLegalFishForPonds(this.fishType.Value))
          {
            string displayName = who.ActiveObject.DisplayName;
            Game1.drawObjectDialogue(Game1.content.LoadString("Strings\\Buildings:CantPutInPonds", (object) displayName.ToLower()));
            return true;
          }
          if (who.ActiveObject.ItemId != this.fishType.Value)
          {
            string displayName1 = who.ActiveObject.DisplayName;
            if (who.ActiveObject.QualifiedItemId == "(O)393" || who.ActiveObject.QualifiedItemId == "(O)397")
            {
              Game1.drawObjectDialogue(Game1.content.LoadString("Strings\\Buildings:WrongFishTypeCoral", (object) displayName1));
            }
            else
            {
              string displayName2 = ItemRegistry.GetDataOrErrorItem(this.fishType.Value).DisplayName;
              if (Game1.content.GetCurrentLanguage() == LocalizedContentManager.LanguageCode.de)
                Game1.drawObjectDialogue(Game1.content.LoadString("Strings\\Buildings:WrongFishType", (object) displayName1, (object) displayName2));
              else
                Game1.drawObjectDialogue(Game1.content.LoadString("Strings\\Buildings:WrongFishType", (object) displayName1.ToLower(), (object) displayName2.ToLower()));
            }
            return true;
          }
          if (this.currentOccupants.Value < this.maxOccupants.Value)
            return this.addFishToPond(who, who.ActiveObject);
          Game1.drawObjectDialogue(Game1.content.LoadString("Strings\\Buildings:PondFull"));
          return true;
        }
        if (this.isLegalFishForPonds(who.ActiveObject.ItemId))
          return this.addFishToPond(who, who.ActiveObject);
        string displayName3 = who.ActiveObject.DisplayName;
        Game1.drawObjectDialogue(Game1.content.LoadString("Strings\\Buildings:CantPutInPonds", (object) displayName3));
        return true;
      }
      if (this.fishType.Value != null)
      {
        if (Game1.didPlayerJustRightClick(true))
        {
          Game1.playSound("bigSelect");
          Game1.activeClickableMenu = (IClickableMenu) new PondQueryMenu(this);
          return true;
        }
      }
      else if (Game1.didPlayerJustRightClick(true))
      {
        Game1.drawObjectDialogue(Game1.content.LoadString("Strings\\Buildings:NoFish"));
        return true;
      }
    }
    return base.doAction(tileLocation, who);
  }

  public void AnimateHappyFish()
  {
    this._numberOfFishToJump = this.currentOccupants.Value;
    this._timeUntilFishHop = 1f;
  }

  public Vector2 GetItemBucketTile()
  {
    return new Vector2((float) (this.tileX.Value + 4), (float) (this.tileY.Value + 4));
  }

  public Vector2 GetRequestTile()
  {
    return new Vector2((float) (this.tileX.Value + 2), (float) (this.tileY.Value + 2));
  }

  public Vector2 GetCenterTile()
  {
    return new Vector2((float) (this.tileX.Value + 2), (float) (this.tileY.Value + 2));
  }

  public void ResolveNeeds(Farmer who)
  {
    this.Reseed();
    this.hasCompletedRequest.Value = true;
    this.lastUnlockedPopulationGate.Value = this.maxOccupants.Value + 1;
    this.UpdateMaximumOccupancy();
    this.daysSinceSpawn.Value = 0;
    int num = 0;
    FishPondData fishPondData = this.GetFishPondData();
    if (fishPondData != null)
      num = (int) ((double) fishPondData.SpawnTime * (double) FishPond.QUEST_SPAWNRATE_EXP_MULTIPIER);
    who.gainExperience(1, num + FishPond.QUEST_BASE_EXP);
    Random daySaveRandom = Utility.CreateDaySaveRandom((double) this.seedOffset.Value);
    Game1.showGlobalMessage(PondQueryMenu.getCompletedRequestString(this, this.GetFishObject(), daySaveRandom));
  }

  public override void resetLocalState()
  {
    base.resetLocalState();
    this._jumpingFish.Clear();
    while (this._fishSilhouettes.Count < this.currentOccupants.Value)
    {
      PondFishSilhouette pondFishSilhouette = new PondFishSilhouette(this);
      this._fishSilhouettes.Add(pondFishSilhouette);
      pondFishSilhouette.position = (this.GetCenterTile() + new Vector2(Utility.Lerp(-0.5f, 0.5f, (float) Game1.random.NextDouble()) * (float) (this.tilesWide.Value - 2), Utility.Lerp(-0.5f, 0.5f, (float) Game1.random.NextDouble()) * (float) (this.tilesHigh.Value - 2))) * 64f;
    }
  }

  private bool isLegalFishForPonds(string itemId) => FishPond.GetRawData(itemId) != null;

  private void showObjectThrownIntoPondAnimation(Farmer who, StardewValley.Object whichObject, Action callback = null)
  {
    who.faceGeneralDirection(this.GetCenterTile() * 64f + new Vector2(32f, 32f));
    if (who.FacingDirection == 1 || who.FacingDirection == 3)
    {
      float num1 = Vector2.Distance(who.Position, this.GetCenterTile() * 64f);
      float num2 = (float) ((double) this.GetCenterTile().Y * 64.0 + 32.0) - who.position.Y;
      float num3 = num1 - 8f;
      float y = 1f / 400f;
      float num4 = num3 * (float) Math.Sqrt((double) y / (2.0 * ((double) num3 + 96.0)));
      float delay = (float) (2.0 * ((double) num4 / (double) y)) + ((float) Math.Sqrt((double) num4 * (double) num4 + 2.0 * (double) y * 96.0) - num4) / y + num2;
      float num5 = 0.0f;
      if ((double) num2 > 0.0)
      {
        num5 = num2 / 832f;
        delay += num5 * 200f;
      }
      Game1.playSound("throwDownITem");
      TemporaryAnimatedSpriteList sprites = new TemporaryAnimatedSpriteList();
      ParsedItemData dataOrErrorItem = ItemRegistry.GetDataOrErrorItem(whichObject.QualifiedItemId);
      sprites.Add(new TemporaryAnimatedSprite(dataOrErrorItem.GetTextureName(), dataOrErrorItem.GetSourceRect(), who.Position + new Vector2(0.0f, -64f), false, 0.0f, Color.White)
      {
        scale = 4f,
        layerDepth = 1f,
        totalNumberOfLoops = 1,
        interval = delay,
        motion = new Vector2((who.FacingDirection == 3 ? -1f : 1f) * (num4 - num5), (float) (-(double) num4 * 3.0 / 2.0)),
        acceleration = new Vector2(0.0f, y),
        timeBasedMotion = true
      });
      sprites.Add(new TemporaryAnimatedSprite(28, 100f, 2, 1, this.GetCenterTile() * 64f, false, false)
      {
        delayBeforeAnimationStart = (int) delay,
        layerDepth = (float) ((((double) this.tileY.Value + 0.5) * 64.0 + 2.0) / 10000.0)
      });
      sprites.Add(new TemporaryAnimatedSprite("TileSheets\\animations", new Microsoft.Xna.Framework.Rectangle(0, 0, 64 /*0x40*/, 64 /*0x40*/), 55f, 8, 0, this.GetCenterTile() * 64f, false, Game1.random.NextBool(), (float) ((((double) this.tileY.Value + 0.5) * 64.0 + 1.0) / 10000.0), 0.01f, Color.White, 0.75f, 3f / 1000f, 0.0f, 0.0f)
      {
        delayBeforeAnimationStart = (int) delay
      });
      sprites.Add(new TemporaryAnimatedSprite("TileSheets\\animations", new Microsoft.Xna.Framework.Rectangle(0, 0, 64 /*0x40*/, 64 /*0x40*/), 65f, 8, 0, this.GetCenterTile() * 64f + new Vector2((float) Game1.random.Next(-32, 32 /*0x20*/), (float) Game1.random.Next(-16, 32 /*0x20*/)), false, Game1.random.NextBool(), (float) ((((double) this.tileY.Value + 0.5) * 64.0 + 1.0) / 10000.0), 0.01f, Color.White, 0.75f, 3f / 1000f, 0.0f, 0.0f)
      {
        delayBeforeAnimationStart = (int) delay
      });
      sprites.Add(new TemporaryAnimatedSprite("TileSheets\\animations", new Microsoft.Xna.Framework.Rectangle(0, 0, 64 /*0x40*/, 64 /*0x40*/), 75f, 8, 0, this.GetCenterTile() * 64f + new Vector2((float) Game1.random.Next(-32, 32 /*0x20*/), (float) Game1.random.Next(-16, 32 /*0x20*/)), false, Game1.random.NextBool(), (float) ((((double) this.tileY.Value + 0.5) * 64.0 + 1.0) / 10000.0), 0.01f, Color.White, 0.75f, 3f / 1000f, 0.0f, 0.0f)
      {
        delayBeforeAnimationStart = (int) delay
      });
      if (who.IsLocalPlayer)
      {
        DelayedAction.playSoundAfterDelay("waterSlosh", (int) delay, who.currentLocation);
        if (callback != null)
          DelayedAction.functionAfterDelay(callback, (int) delay);
      }
      if (this.fishType.Value != null && whichObject.ItemId == this.fishType.Value)
        this._delayUntilFishSilhouetteAdded = delay / 1000f;
      Game1.multiplayer.broadcastSprites(who.currentLocation, sprites);
    }
    else
    {
      float num6 = Vector2.Distance(who.Position, this.GetCenterTile() * 64f);
      float num7 = Math.Abs(num6);
      if (who.FacingDirection == 0)
      {
        num6 = -num6;
        num7 += 64f;
      }
      float num8 = this.GetCenterTile().X * 64f - who.position.X;
      float y = 1f / 400f;
      float num9 = (float) Math.Sqrt(2.0 * (double) y * (double) num7);
      float num10 = (float) (Math.Sqrt(2.0 * ((double) num7 - (double) num6) / (double) y) + (double) num9 / (double) y) * 1.05f;
      float delay = (float) ((who.FacingDirection != 0 ? (double) (num10 * 2.5f) : (double) (num10 * 0.7f)) - (double) Math.Abs(num8) / (who.FacingDirection == 0 ? 100.0 : 2.0));
      Game1.playSound("throwDownITem");
      TemporaryAnimatedSpriteList sprites = new TemporaryAnimatedSpriteList();
      ParsedItemData dataOrErrorItem = ItemRegistry.GetDataOrErrorItem(whichObject.QualifiedItemId);
      sprites.Add(new TemporaryAnimatedSprite(dataOrErrorItem.GetTextureName(), dataOrErrorItem.GetSourceRect(), who.Position + new Vector2(0.0f, -64f), false, 0.0f, Color.White)
      {
        scale = 4f,
        layerDepth = 1f,
        totalNumberOfLoops = 1,
        interval = delay,
        motion = new Vector2(num8 / (who.FacingDirection == 0 ? 900f : 1000f), -num9),
        acceleration = new Vector2(0.0f, y),
        timeBasedMotion = true
      });
      sprites.Add(new TemporaryAnimatedSprite(28, 100f, 2, 1, this.GetCenterTile() * 64f, false, false)
      {
        delayBeforeAnimationStart = (int) delay,
        layerDepth = (float) ((((double) this.tileY.Value + 0.5) * 64.0 + 2.0) / 10000.0)
      });
      sprites.Add(new TemporaryAnimatedSprite("TileSheets\\animations", new Microsoft.Xna.Framework.Rectangle(0, 0, 64 /*0x40*/, 64 /*0x40*/), 55f, 8, 0, this.GetCenterTile() * 64f, false, Game1.random.NextBool(), (float) ((((double) this.tileY.Value + 0.5) * 64.0 + 1.0) / 10000.0), 0.01f, Color.White, 0.75f, 3f / 1000f, 0.0f, 0.0f)
      {
        delayBeforeAnimationStart = (int) delay
      });
      sprites.Add(new TemporaryAnimatedSprite("TileSheets\\animations", new Microsoft.Xna.Framework.Rectangle(0, 0, 64 /*0x40*/, 64 /*0x40*/), 65f, 8, 0, this.GetCenterTile() * 64f + new Vector2((float) Game1.random.Next(-32, 32 /*0x20*/), (float) Game1.random.Next(-16, 32 /*0x20*/)), false, Game1.random.NextBool(), (float) ((((double) this.tileY.Value + 0.5) * 64.0 + 1.0) / 10000.0), 0.01f, Color.White, 0.75f, 3f / 1000f, 0.0f, 0.0f)
      {
        delayBeforeAnimationStart = (int) delay
      });
      sprites.Add(new TemporaryAnimatedSprite("TileSheets\\animations", new Microsoft.Xna.Framework.Rectangle(0, 0, 64 /*0x40*/, 64 /*0x40*/), 75f, 8, 0, this.GetCenterTile() * 64f + new Vector2((float) Game1.random.Next(-32, 32 /*0x20*/), (float) Game1.random.Next(-16, 32 /*0x20*/)), false, Game1.random.NextBool(), (float) ((((double) this.tileY.Value + 0.5) * 64.0 + 1.0) / 10000.0), 0.01f, Color.White, 0.75f, 3f / 1000f, 0.0f, 0.0f)
      {
        delayBeforeAnimationStart = (int) delay
      });
      if (who.IsLocalPlayer)
      {
        DelayedAction.playSoundAfterDelay("waterSlosh", (int) delay, who.currentLocation);
        if (callback != null)
          DelayedAction.functionAfterDelay(callback, (int) delay);
      }
      if (this.fishType.Value != null && whichObject.ItemId == this.fishType.Value)
        this._delayUntilFishSilhouetteAdded = delay / 1000f;
      Game1.multiplayer.broadcastSprites(who.currentLocation, sprites);
    }
  }

  private bool addFishToPond(Farmer who, StardewValley.Object fish)
  {
    who.reduceActiveItemByOne();
    ++this.currentOccupants.Value;
    if (this.currentOccupants.Value == 1)
    {
      this.fishType.Value = fish.ItemId;
      this._fishPondData = (FishPondData) null;
      this.UpdateMaximumOccupancy();
    }
    this.showObjectThrownIntoPondAnimation(who, fish);
    return true;
  }

  public override void dayUpdate(int dayOfMonth)
  {
    this.hasSpawnedFish.Value = false;
    this._hasAnimatedSpawnedFish = false;
    if (this.hasCompletedRequest.Value)
    {
      this.neededItem.Value = (Item) null;
      this.neededItemCount.Set(-1);
      this.hasCompletedRequest.Value = false;
    }
    FishPondData fishPondData = this.GetFishPondData();
    if (this.currentOccupants.Value > 0 && fishPondData != null)
    {
      Random daySaveRandom = Utility.CreateDaySaveRandom((double) (this.tileX.Value * 1000), (double) (this.tileY.Value * 2000));
      if (((double) fishPondData.BaseMinProduceChance >= (double) fishPondData.BaseMaxProduceChance ? (daySaveRandom.NextBool(fishPondData.BaseMinProduceChance) ? 1 : 0) : (daySaveRandom.NextDouble() < (double) Utility.Lerp(fishPondData.BaseMinProduceChance, fishPondData.BaseMaxProduceChance, (float) this.currentOccupants.Value / 10f) ? 1 : 0)) != 0)
        this.output.Value = this.GetFishProduce(daySaveRandom);
      ++this.daysSinceSpawn.Value;
      if (this.daysSinceSpawn.Value > fishPondData.SpawnTime)
        this.daysSinceSpawn.Value = fishPondData.SpawnTime;
      if (this.daysSinceSpawn.Value >= fishPondData.SpawnTime)
      {
        string itemId;
        int count;
        if (this.TryGetNeededItemData(out itemId, out count))
        {
          if (this.currentOccupants.Value >= this.maxOccupants.Value && this.neededItem.Value == null)
          {
            this.neededItem.Value = ItemRegistry.Create(itemId);
            this.neededItemCount.Value = count;
          }
        }
        else
          this.SpawnFish();
      }
      if (this.currentOccupants.Value == 10 && this.fishType.Value == "717")
      {
        foreach (Farmer allFarmer in Game1.getAllFarmers())
        {
          if (allFarmer.mailReceived.Add("FullCrabPond"))
            allFarmer.activeDialogueEvents["FullCrabPond"] = 14;
        }
      }
      this.doFishSpecificWaterColoring();
    }
    base.dayUpdate(dayOfMonth);
  }

  private void doFishSpecificWaterColoring()
  {
    FishPondData fishPondData = this.GetFishPondData();
    Color? nullable = new Color?();
    if (fishPondData != null)
    {
      int? count = fishPondData.WaterColor?.Count;
      int num = 0;
      if (count.GetValueOrDefault() > num & count.HasValue)
      {
        foreach (FishPondWaterColor fishPondWaterColor in fishPondData.WaterColor)
        {
          if (this.currentOccupants.Value >= fishPondWaterColor.MinPopulation && this.lastUnlockedPopulationGate.Value >= fishPondWaterColor.MinUnlockedPopulationGate && (fishPondWaterColor.Condition == null || GameStateQuery.CheckConditions(fishPondWaterColor.Condition, this.GetParentLocation(), inputItem: (Item) this.GetFishObject())))
          {
            if (fishPondWaterColor.Color.EqualsIgnoreCase("CopyFromInput"))
            {
              StardewValley.Object fishObject = this.GetFishObject();
              nullable = fishObject is ColoredObject coloredObject ? new Color?(coloredObject.color.Value) : ItemContextTagManager.GetColorFromTags((Item) fishObject);
              break;
            }
            nullable = Utility.StringToColor(fishPondWaterColor.Color);
            break;
          }
        }
      }
    }
    this.overrideWaterColor.Value = nullable ?? Color.White;
  }

  /// <inheritdoc />
  public override Color? GetWaterColor(Vector2 tile)
  {
    return !(this.overrideWaterColor.Value != Color.White) ? new Color?() : new Color?(this.overrideWaterColor.Value);
  }

  public bool JumpFish()
  {
    if (this._fishSilhouettes.Count == 0)
      return false;
    PondFishSilhouette pondFishSilhouette = Game1.random.ChooseFrom<PondFishSilhouette>((IList<PondFishSilhouette>) this._fishSilhouettes);
    this._fishSilhouettes.Remove(pondFishSilhouette);
    this._jumpingFish.Add(new JumpingFish(this, pondFishSilhouette.position, (this.GetCenterTile() + new Vector2(0.5f, 0.5f)) * 64f));
    return true;
  }

  public void SpawnFish()
  {
    if (this.currentOccupants.Value >= this.maxOccupants.Value || this.currentOccupants.Value <= 0)
      return;
    this.hasSpawnedFish.Value = true;
    this.daysSinceSpawn.Value = 0;
    ++this.currentOccupants.Value;
    if (this.currentOccupants.Value <= this.maxOccupants.Value)
      return;
    this.currentOccupants.Value = this.maxOccupants.Value;
  }

  public override bool performActiveObjectDropInAction(Farmer who, bool probe)
  {
    StardewValley.Object activeObject = who.ActiveObject;
    if (this.IsValidSignItem((Item) activeObject) && (this.sign.Value == null || activeObject.QualifiedItemId != this.sign.Value.QualifiedItemId))
    {
      if (probe)
        return true;
      StardewValley.Object @object = this.sign.Value;
      this.sign.Value = (StardewValley.Object) activeObject.getOne();
      who.reduceActiveItemByOne();
      if (@object != null)
        Game1.createItemDebris((Item) @object, new Vector2((float) this.tileX.Value + 0.5f, (float) (this.tileY.Value + this.tilesHigh.Value)) * 64f, 3, who.currentLocation);
      who.currentLocation.playSound("axe");
      return true;
    }
    if (!(activeObject?.QualifiedItemId == "(O)GoldenAnimalCracker") || this.goldenAnimalCracker.Value || this.currentOccupants.Value <= 0)
      return base.performActiveObjectDropInAction(who, probe);
    if (probe)
      return true;
    who.reduceActiveItemByOne();
    this.goldenAnimalCracker.Value = true;
    this.isPlayingGoldenCrackerAnimation.Value = true;
    this.showObjectThrownIntoPondAnimation(who, activeObject, (Action) (() => this.isPlayingGoldenCrackerAnimation.Value = false));
    return true;
  }

  public override void performToolAction(Tool t, int tileX, int tileY)
  {
    switch (t)
    {
      case Axe _:
      case Pickaxe _:
        if (this.sign.Value != null)
        {
          if (t.getLastFarmerToUse() != null)
            Game1.createItemDebris((Item) this.sign.Value, new Vector2((float) this.tileX.Value + 0.5f, (float) (this.tileY.Value + this.tilesHigh.Value)) * 64f, 3, t.getLastFarmerToUse().currentLocation);
          this.sign.Value = (StardewValley.Object) null;
          t.getLastFarmerToUse().currentLocation.playSound("hammer", new Vector2?(new Vector2((float) tileX, (float) tileY)));
          break;
        }
        break;
    }
    base.performToolAction(t, tileX, tileY);
  }

  /// <inheritdoc />
  public override void performActionOnConstruction(GameLocation location, Farmer who)
  {
    base.performActionOnConstruction(location, who);
    this.nettingStyle.Value = (this.tileX.Value / 3 + this.tileY.Value / 3) % 3;
  }

  /// <inheritdoc />
  public override void performActionOnBuildingPlacement()
  {
    base.performActionOnBuildingPlacement();
    this.nettingStyle.Value = (this.tileX.Value / 3 + this.tileY.Value / 3) % 3;
  }

  public bool HasUnresolvedNeeds()
  {
    return this.neededItem.Value != null && this.TryGetNeededItemData(out string _, out int _) && !this.hasCompletedRequest.Value;
  }

  private bool TryGetNeededItemData(out string itemId, out int count)
  {
    itemId = (string) null;
    count = 1;
    if (this.currentOccupants.Value < this.maxOccupants.Value)
      return false;
    this.GetFishPondData();
    List<string> options;
    if (this._fishPondData?.PopulationGates == null || this.maxOccupants.Value + 1 <= this.lastUnlockedPopulationGate.Value || !this._fishPondData.PopulationGates.TryGetValue(this.maxOccupants.Value + 1, out options))
      return false;
    Random daySaveRandom = Utility.CreateDaySaveRandom((double) Utility.CreateRandomSeed((double) (this.tileX.Value * 1000), (double) (this.tileY.Value * 2000)));
    string[] strArray = ArgUtility.SplitBySpace(daySaveRandom.ChooseFrom<string>((IList<string>) options));
    if (strArray.Length >= 1)
      itemId = strArray[0];
    if (strArray.Length >= 3)
      count = daySaveRandom.Next(Convert.ToInt32(strArray[1]), Convert.ToInt32(strArray[2]) + 1);
    else if (strArray.Length >= 2)
      count = Convert.ToInt32(strArray[1]);
    return true;
  }

  public void ClearPond()
  {
    Microsoft.Xna.Framework.Rectangle boundingBox = this.GetBoundingBox();
    for (int index = 0; index < this.currentOccupants.Value; ++index)
    {
      Vector2 pixelOrigin = Utility.PointToVector2(boundingBox.Center);
      int direction = Game1.random.Next(4);
      switch (direction)
      {
        case 0:
          pixelOrigin = new Vector2((float) Game1.random.Next(boundingBox.Left, boundingBox.Right), (float) boundingBox.Top);
          break;
        case 1:
          pixelOrigin = new Vector2((float) boundingBox.Right, (float) Game1.random.Next(boundingBox.Top, boundingBox.Bottom));
          break;
        case 2:
          pixelOrigin = new Vector2((float) Game1.random.Next(boundingBox.Left, boundingBox.Right), (float) boundingBox.Bottom);
          break;
        case 3:
          pixelOrigin = new Vector2((float) boundingBox.Left, (float) Game1.random.Next(boundingBox.Top, boundingBox.Bottom));
          break;
      }
      Game1.createItemDebris(this.CreateFishInstance(), pixelOrigin, direction, Game1.currentLocation, flopFish: true);
    }
    this._hasAnimatedSpawnedFish = false;
    this.hasSpawnedFish.Value = false;
    this._fishSilhouettes.Clear();
    this._jumpingFish.Clear();
    this.goldenAnimalCracker.Value = false;
    this.isPlayingGoldenCrackerAnimation.Value = false;
    this._fishObject = (StardewValley.Object) null;
    this.currentOccupants.Value = 0;
    this.daysSinceSpawn.Value = 0;
    this.neededItem.Value = (Item) null;
    this.neededItemCount.Value = -1;
    this.lastUnlockedPopulationGate.Value = 0;
    this.fishType.Value = (string) null;
    this.Reseed();
    this.overrideWaterColor.Value = Color.White;
  }

  public StardewValley.Object CatchFish()
  {
    if (this.currentOccupants.Value == 0)
      return (StardewValley.Object) null;
    --this.currentOccupants.Value;
    return (StardewValley.Object) this.CreateFishInstance();
  }

  public StardewValley.Object GetFishObject()
  {
    if (this._fishObject == null)
      this._fishObject = new StardewValley.Object(this.fishType.Value, 1);
    return this._fishObject;
  }

  public override void Update(GameTime time)
  {
    this.needsMutex.Update(this.GetParentLocation());
    this.animateHappyFishEvent.Poll();
    if (!this._hasAnimatedSpawnedFish && this.hasSpawnedFish.Value && this._numberOfFishToJump <= 0 && Utility.isOnScreen((this.GetCenterTile() + new Vector2(0.5f, 0.5f)) * 64f, 64 /*0x40*/))
    {
      this._hasAnimatedSpawnedFish = true;
      if (this.fishType.Value != "393" && this.fishType.Value != "397")
      {
        this._numberOfFishToJump = 1;
        this._timeUntilFishHop = Utility.RandomFloat(2f, 5f);
      }
    }
    if ((double) this._delayUntilFishSilhouetteAdded > 0.0)
    {
      this._delayUntilFishSilhouetteAdded -= (float) time.ElapsedGameTime.TotalSeconds;
      if ((double) this._delayUntilFishSilhouetteAdded < 0.0)
        this._delayUntilFishSilhouetteAdded = 0.0f;
    }
    if (this._numberOfFishToJump > 0 && (double) this._timeUntilFishHop > 0.0)
    {
      this._timeUntilFishHop -= (float) time.ElapsedGameTime.TotalSeconds;
      if ((double) this._timeUntilFishHop <= 0.0 && this.JumpFish())
      {
        --this._numberOfFishToJump;
        this._timeUntilFishHop = Utility.RandomFloat(0.15f, 0.25f);
      }
    }
    while (this._fishSilhouettes.Count > this.currentOccupants.Value - this._jumpingFish.Count)
      this._fishSilhouettes.RemoveAt(0);
    if ((double) this._delayUntilFishSilhouetteAdded <= 0.0)
    {
      while (this._fishSilhouettes.Count < this.currentOccupants.Value - this._jumpingFish.Count)
        this._fishSilhouettes.Add(new PondFishSilhouette(this));
    }
    TimeSpan elapsedGameTime;
    for (int index = 0; index < this._fishSilhouettes.Count; ++index)
    {
      PondFishSilhouette fishSilhouette = this._fishSilhouettes[index];
      elapsedGameTime = time.ElapsedGameTime;
      double totalSeconds = elapsedGameTime.TotalSeconds;
      fishSilhouette.Update((float) totalSeconds);
    }
    for (int index = 0; index < this._jumpingFish.Count; ++index)
    {
      JumpingFish jumpingFish = this._jumpingFish[index];
      elapsedGameTime = time.ElapsedGameTime;
      double totalSeconds = elapsedGameTime.TotalSeconds;
      if (jumpingFish.Update((float) totalSeconds))
      {
        this._fishSilhouettes.Add(new PondFishSilhouette(this)
        {
          position = this._jumpingFish[index].position
        });
        this._jumpingFish.RemoveAt(index);
        --index;
      }
    }
    base.Update(time);
  }

  public override bool isTileFishable(Vector2 tile)
  {
    return this.daysOfConstructionLeft.Value <= 0 && (double) tile.X > (double) this.tileX.Value && (double) tile.X < (double) (this.tileX.Value + this.tilesWide.Value - 1) && (double) tile.Y > (double) this.tileY.Value && (double) tile.Y < (double) (this.tileY.Value + this.tilesHigh.Value - 1);
  }

  public override bool CanRefillWateringCan() => this.daysOfConstructionLeft.Value <= 0;

  public override Microsoft.Xna.Framework.Rectangle? getSourceRectForMenu()
  {
    return new Microsoft.Xna.Framework.Rectangle?(new Microsoft.Xna.Framework.Rectangle(0, 0, 80 /*0x50*/, 80 /*0x50*/));
  }

  public override void drawInMenu(SpriteBatch b, int x, int y)
  {
    BuildingData data = this.GetData();
    y += 32 /*0x20*/;
    if (this.ShouldDrawShadow(data))
      this.drawShadow(b, x, y);
    b.Draw(this.texture.Value, new Vector2((float) x, (float) y), new Microsoft.Xna.Framework.Rectangle?(new Microsoft.Xna.Framework.Rectangle(0, 80 /*0x50*/, 80 /*0x50*/, 80 /*0x50*/)), new Color(60, 126, 150) * this.alpha, 0.0f, new Vector2(0.0f, 0.0f), 4f, SpriteEffects.None, 0.75f);
    for (int index1 = this.tileY.Value; index1 < this.tileY.Value + 5; ++index1)
    {
      for (int index2 = this.tileX.Value; index2 < this.tileX.Value + 4; ++index2)
      {
        int num = index1 == this.tileY.Value + 4 ? 1 : 0;
        bool flag = index1 == this.tileY.Value;
        if (num != 0)
          b.Draw(Game1.mouseCursors, new Vector2((float) (x + index2 * 64 /*0x40*/ + 32 /*0x20*/), (float) (y + (index1 + 1) * 64 /*0x40*/ - (int) Game1.currentLocation.waterPosition - 32 /*0x20*/)), new Microsoft.Xna.Framework.Rectangle?(new Microsoft.Xna.Framework.Rectangle(Game1.currentLocation.waterAnimationIndex * 64 /*0x40*/, 2064 + ((index2 + index1) % 2 == 0 ? (Game1.currentLocation.waterTileFlip ? 128 /*0x80*/ : 0) : (Game1.currentLocation.waterTileFlip ? 0 : 128 /*0x80*/)), 64 /*0x40*/, 32 /*0x20*/ + (int) Game1.currentLocation.waterPosition - 5)), Game1.currentLocation.waterColor.Value, 0.0f, Vector2.Zero, 1f, SpriteEffects.None, 0.8f);
        else
          b.Draw(Game1.mouseCursors, new Vector2((float) (x + index2 * 64 /*0x40*/ + 32 /*0x20*/), (float) (y + index1 * 64 /*0x40*/ + 32 /*0x20*/ - (!flag ? (int) Game1.currentLocation.waterPosition : 0))), new Microsoft.Xna.Framework.Rectangle?(new Microsoft.Xna.Framework.Rectangle(Game1.currentLocation.waterAnimationIndex * 64 /*0x40*/, 2064 + ((index2 + index1) % 2 == 0 ? (Game1.currentLocation.waterTileFlip ? 128 /*0x80*/ : 0) : (Game1.currentLocation.waterTileFlip ? 0 : 128 /*0x80*/)) + (flag ? (int) Game1.currentLocation.waterPosition : 0), 64 /*0x40*/, 64 /*0x40*/ + (flag ? (int) -(double) Game1.currentLocation.waterPosition : 0))), Game1.currentLocation.waterColor.Value, 0.0f, Vector2.Zero, 1f, SpriteEffects.None, 0.8f);
      }
    }
    b.Draw(this.texture.Value, new Vector2((float) x, (float) y), new Microsoft.Xna.Framework.Rectangle?(new Microsoft.Xna.Framework.Rectangle(0, 0, 80 /*0x50*/, 80 /*0x50*/)), this.color * this.alpha, 0.0f, new Vector2(0.0f, 0.0f), 4f, SpriteEffects.None, 0.9f);
    b.Draw(this.texture.Value, new Vector2((float) (x + 64 /*0x40*/), (float) (y + 44 + (Game1.currentGameTime.TotalGameTime.TotalMilliseconds % 2500.0 < 1250.0 ? 4 : 0))), new Microsoft.Xna.Framework.Rectangle?(new Microsoft.Xna.Framework.Rectangle(16 /*0x10*/, 160 /*0xA0*/, 48 /*0x30*/, 7)), this.color * this.alpha, 0.0f, Vector2.Zero, 4f, SpriteEffects.None, 0.95f);
    b.Draw(this.texture.Value, new Vector2((float) x, (float) (y - 128 /*0x80*/)), new Microsoft.Xna.Framework.Rectangle?(new Microsoft.Xna.Framework.Rectangle(80 /*0x50*/, 0, 80 /*0x50*/, 48 /*0x30*/)), this.color * this.alpha, 0.0f, new Vector2(0.0f, 0.0f), 4f, SpriteEffects.None, 1f);
  }

  public override void OnEndMove()
  {
    foreach (PondFishSilhouette fishSilhouette in this._fishSilhouettes)
      fishSilhouette.position = (this.GetCenterTile() + new Vector2(Utility.Lerp(-0.5f, 0.5f, (float) Game1.random.NextDouble()) * (float) (this.tilesWide.Value - 2), Utility.Lerp(-0.5f, 0.5f, (float) Game1.random.NextDouble()) * (float) (this.tilesHigh.Value - 2))) * 64f;
  }

  public override void draw(SpriteBatch b)
  {
    if (this.isMoving)
      return;
    if (this.daysOfConstructionLeft.Value > 0)
    {
      this.drawInConstruction(b);
    }
    else
    {
      BuildingData data1 = this.GetData();
      for (int index = this.animations.Count - 1; index >= 0; --index)
        this.animations[index].draw(b);
      if (this.ShouldDrawShadow(data1))
        this.drawShadow(b);
      b.Draw(this.texture.Value, Game1.GlobalToLocal(Game1.viewport, new Vector2((float) (this.tileX.Value * 64 /*0x40*/), (float) (this.tileY.Value * 64 /*0x40*/ + this.tilesHigh.Value * 64 /*0x40*/))), new Microsoft.Xna.Framework.Rectangle?(new Microsoft.Xna.Framework.Rectangle(0, 80 /*0x50*/, 80 /*0x50*/, 80 /*0x50*/)), (this.overrideWaterColor.Value == Color.White ? new Color(60, 126, 150) : this.overrideWaterColor.Value) * this.alpha, 0.0f, new Vector2(0.0f, 80f), 4f, SpriteEffects.None, (float) ((((double) this.tileY.Value + 0.5) * 64.0 - 3.0) / 10000.0));
      for (int index1 = this.tileY.Value; index1 < this.tileY.Value + 5; ++index1)
      {
        for (int index2 = this.tileX.Value; index2 < this.tileX.Value + 4; ++index2)
        {
          int num = index1 == this.tileY.Value + 4 ? 1 : 0;
          bool flag = index1 == this.tileY.Value;
          if (num != 0)
            b.Draw(Game1.mouseCursors, Game1.GlobalToLocal(Game1.viewport, new Vector2((float) (index2 * 64 /*0x40*/ + 32 /*0x20*/), (float) ((index1 + 1) * 64 /*0x40*/ - (int) Game1.currentLocation.waterPosition - 32 /*0x20*/))), new Microsoft.Xna.Framework.Rectangle?(new Microsoft.Xna.Framework.Rectangle(Game1.currentLocation.waterAnimationIndex * 64 /*0x40*/, 2064 + ((index2 + index1) % 2 == 0 ? (Game1.currentLocation.waterTileFlip ? 128 /*0x80*/ : 0) : (Game1.currentLocation.waterTileFlip ? 0 : 128 /*0x80*/)), 64 /*0x40*/, 32 /*0x20*/ + (int) Game1.currentLocation.waterPosition - 5)), this.overrideWaterColor.Equals(Color.White) ? Game1.currentLocation.waterColor.Value : this.overrideWaterColor.Value * 0.5f, 0.0f, Vector2.Zero, 1f, SpriteEffects.None, (float) ((((double) this.tileY.Value + 0.5) * 64.0 - 2.0) / 10000.0));
          else
            b.Draw(Game1.mouseCursors, Game1.GlobalToLocal(Game1.viewport, new Vector2((float) (index2 * 64 /*0x40*/ + 32 /*0x20*/), (float) (index1 * 64 /*0x40*/ + 32 /*0x20*/ - (!flag ? (int) Game1.currentLocation.waterPosition : 0)))), new Microsoft.Xna.Framework.Rectangle?(new Microsoft.Xna.Framework.Rectangle(Game1.currentLocation.waterAnimationIndex * 64 /*0x40*/, 2064 + ((index2 + index1) % 2 == 0 ? (Game1.currentLocation.waterTileFlip ? 128 /*0x80*/ : 0) : (Game1.currentLocation.waterTileFlip ? 0 : 128 /*0x80*/)) + (flag ? (int) Game1.currentLocation.waterPosition : 0), 64 /*0x40*/, 64 /*0x40*/ + (flag ? (int) -(double) Game1.currentLocation.waterPosition : 0))), this.overrideWaterColor.Value == Color.White ? Game1.currentLocation.waterColor.Value : this.overrideWaterColor.Value * 0.5f, 0.0f, Vector2.Zero, 1f, SpriteEffects.None, (float) ((((double) this.tileY.Value + 0.5) * 64.0 - 2.0) / 10000.0));
        }
      }
      TimeSpan totalGameTime;
      if (this.overrideWaterColor.Value.Equals(Color.White))
      {
        SpriteBatch spriteBatch = b;
        Texture2D texture = this.texture.Value;
        xTile.Dimensions.Rectangle viewport = Game1.viewport;
        double x = (double) (this.tileX.Value * 64 /*0x40*/ + 64 /*0x40*/);
        int num1 = this.tileY.Value * 64 /*0x40*/ + 44;
        totalGameTime = Game1.currentGameTime.TotalGameTime;
        int num2 = totalGameTime.TotalMilliseconds % 2500.0 < 1250.0 ? 4 : 0;
        double y = (double) (num1 + num2);
        Vector2 globalPosition = new Vector2((float) x, (float) y);
        Vector2 local = Game1.GlobalToLocal(viewport, globalPosition);
        Microsoft.Xna.Framework.Rectangle? sourceRectangle = new Microsoft.Xna.Framework.Rectangle?(new Microsoft.Xna.Framework.Rectangle(16 /*0x10*/, 160 /*0xA0*/, 48 /*0x30*/, 7));
        Color color = this.color * this.alpha;
        Vector2 zero = Vector2.Zero;
        double layerDepth = (((double) this.tileY.Value + 0.5) * 64.0 + 1.0) / 10000.0;
        spriteBatch.Draw(texture, local, sourceRectangle, color, 0.0f, zero, 4f, SpriteEffects.None, (float) layerDepth);
      }
      b.Draw(this.texture.Value, Game1.GlobalToLocal(Game1.viewport, new Vector2((float) (this.tileX.Value * 64 /*0x40*/), (float) (this.tileY.Value * 64 /*0x40*/ + this.tilesHigh.Value * 64 /*0x40*/))), new Microsoft.Xna.Framework.Rectangle?(new Microsoft.Xna.Framework.Rectangle(0, 0, 80 /*0x50*/, 80 /*0x50*/)), this.color * this.alpha, 0.0f, new Vector2(0.0f, 80f), 4f, SpriteEffects.None, (float) (((double) this.tileY.Value + 0.5) * 64.0 / 10000.0));
      if (this.nettingStyle.Value < 3)
        b.Draw(this.texture.Value, Game1.GlobalToLocal(Game1.viewport, new Vector2((float) (this.tileX.Value * 64 /*0x40*/), (float) (this.tileY.Value * 64 /*0x40*/ + this.tilesHigh.Value * 64 /*0x40*/ - 128 /*0x80*/))), new Microsoft.Xna.Framework.Rectangle?(new Microsoft.Xna.Framework.Rectangle(80 /*0x50*/, this.nettingStyle.Value * 48 /*0x30*/, 80 /*0x50*/, 48 /*0x30*/)), this.color * this.alpha, 0.0f, new Vector2(0.0f, 80f), 4f, SpriteEffects.None, (float) ((((double) this.tileY.Value + 0.5) * 64.0 + 2.0) / 10000.0));
      if (this.sign.Value != null)
      {
        ParsedItemData dataOrErrorItem = ItemRegistry.GetDataOrErrorItem(this.sign.Value.QualifiedItemId);
        b.Draw(dataOrErrorItem.GetTexture(), Game1.GlobalToLocal(Game1.viewport, new Vector2((float) (this.tileX.Value * 64 /*0x40*/ + 8), (float) (this.tileY.Value * 64 /*0x40*/ + this.tilesHigh.Value * 64 /*0x40*/ - 128 /*0x80*/ - 32 /*0x20*/))), new Microsoft.Xna.Framework.Rectangle?(dataOrErrorItem.GetSourceRect()), this.color * this.alpha, 0.0f, Vector2.Zero, 4f, SpriteEffects.None, (float) ((((double) this.tileY.Value + 0.5) * 64.0 + 2.0) / 10000.0));
        if (this.fishType.Value != null)
        {
          ParsedItemData data2 = ItemRegistry.GetData(this.fishType.Value);
          if (data2 != null)
          {
            Texture2D texture = data2.GetTexture();
            Microsoft.Xna.Framework.Rectangle sourceRect = data2.GetSourceRect();
            float num = this.maxOccupants.Value == 1 ? 6f : 0.0f;
            b.Draw(texture, Game1.GlobalToLocal(Game1.viewport, new Vector2((float) (this.tileX.Value * 64 /*0x40*/ + 8 + 8 - 4), (float) (this.tileY.Value * 64 /*0x40*/ + this.tilesHigh.Value * 64 /*0x40*/ - 128 /*0x80*/ - 8 + 4) + num)), new Microsoft.Xna.Framework.Rectangle?(sourceRect), Color.Black * 0.4f * this.alpha, 0.0f, Vector2.Zero, 3f, SpriteEffects.None, (float) ((((double) this.tileY.Value + 0.5) * 64.0 + 3.0) / 10000.0));
            b.Draw(texture, Game1.GlobalToLocal(Game1.viewport, new Vector2((float) (this.tileX.Value * 64 /*0x40*/ + 8 + 8 - 1), (float) (this.tileY.Value * 64 /*0x40*/ + this.tilesHigh.Value * 64 /*0x40*/ - 128 /*0x80*/ - 8 + 1) + num)), new Microsoft.Xna.Framework.Rectangle?(sourceRect), this.color * this.alpha, 0.0f, Vector2.Zero, 3f, SpriteEffects.None, (float) ((((double) this.tileY.Value + 0.5) * 64.0 + 4.0) / 10000.0));
            if (this.maxOccupants.Value > 1)
              Utility.drawTinyDigits(this.currentOccupants.Value, b, Game1.GlobalToLocal(Game1.viewport, new Vector2((float) (this.tileX.Value * 64 /*0x40*/ + 32 /*0x20*/ + 8 + (this.currentOccupants.Value < 10 ? 8 : 0)), (float) (this.tileY.Value * 64 /*0x40*/ + this.tilesHigh.Value * 64 /*0x40*/ - 96 /*0x60*/))), 3f, (float) ((((double) this.tileY.Value + 0.5) * 64.0 + 5.0) / 10000.0), Color.LightYellow * this.alpha);
          }
        }
      }
      if (this._fishObject != null && (this._fishObject.QualifiedItemId == "(O)393" || this._fishObject.QualifiedItemId == "(O)397"))
      {
        for (int index = 0; index < this.currentOccupants.Value; ++index)
        {
          Vector2 vector2 = Vector2.Zero;
          int num = (index + this.seedOffset.Value) % 10;
          switch (num)
          {
            case 0:
              vector2 = new Vector2(0.0f, 0.0f);
              break;
            case 1:
              vector2 = new Vector2(48f, 32f);
              break;
            case 2:
              vector2 = new Vector2(80f, 72f);
              break;
            case 3:
              vector2 = new Vector2(140f, 28f);
              break;
            case 4:
              vector2 = new Vector2(96f, 0.0f);
              break;
            case 5:
              vector2 = new Vector2(0.0f, 96f);
              break;
            case 6:
              vector2 = new Vector2(140f, 80f);
              break;
            case 7:
              vector2 = new Vector2(64f, 120f);
              break;
            case 8:
              vector2 = new Vector2(140f, 140f);
              break;
            case 9:
              vector2 = new Vector2(0.0f, 150f);
              break;
          }
          b.Draw(Game1.shadowTexture, Game1.GlobalToLocal(Game1.viewport, new Vector2((float) (this.tileX.Value * 64 /*0x40*/ + 64 /*0x40*/ + 7), (float) (this.tileY.Value * 64 /*0x40*/ + 64 /*0x40*/ + 32 /*0x20*/)) + vector2), new Microsoft.Xna.Framework.Rectangle?(Game1.shadowTexture.Bounds), this.color * this.alpha, 0.0f, Vector2.Zero, 3f, SpriteEffects.None, (float) ((((double) this.tileY.Value + 0.5) * 64.0 - 2.0) / 10000.0 - 1.1000000085914508E-05));
          ParsedItemData dataOrErrorItem = ItemRegistry.GetDataOrErrorItem("(O)" + this.fishType.Value);
          Texture2D texture = dataOrErrorItem.GetTexture();
          Microsoft.Xna.Framework.Rectangle sourceRect = dataOrErrorItem.GetSourceRect();
          b.Draw(texture, Game1.GlobalToLocal(Game1.viewport, new Vector2((float) (this.tileX.Value * 64 /*0x40*/ + 64 /*0x40*/), (float) (this.tileY.Value * 64 /*0x40*/ + 64 /*0x40*/)) + vector2), new Microsoft.Xna.Framework.Rectangle?(sourceRect), this.color * this.alpha * 0.75f, 0.0f, Vector2.Zero, 3f, num % 3 == 0 ? SpriteEffects.FlipHorizontally : SpriteEffects.None, (float) ((((double) this.tileY.Value + 0.5) * 64.0 - 2.0) / 10000.0 - 9.9999997473787516E-06));
        }
      }
      else
      {
        for (int index = 0; index < this._fishSilhouettes.Count; ++index)
          this._fishSilhouettes[index].Draw(b);
      }
      for (int index = 0; index < this._jumpingFish.Count; ++index)
        this._jumpingFish[index].Draw(b);
      if (this.HasUnresolvedNeeds())
      {
        Vector2 globalPosition = this.GetRequestTile() * 64f + 64f * new Vector2(0.5f, 0.5f);
        totalGameTime = Game1.currentGameTime.TotalGameTime;
        float num = (float) (3.0 * Math.Round(Math.Sin(totalGameTime.TotalMilliseconds / 250.0), 2));
        float layerDepth = (float) (((double) globalPosition.Y + 160.0) / 10000.0 + 9.9999999747524271E-07);
        globalPosition.Y += num - 32f;
        b.Draw(Game1.mouseCursors, Game1.GlobalToLocal(Game1.viewport, globalPosition), new Microsoft.Xna.Framework.Rectangle?(new Microsoft.Xna.Framework.Rectangle(403, 496, 5, 14)), Color.White * 0.75f, 0.0f, new Vector2(2f, 14f), 4f, SpriteEffects.None, layerDepth);
      }
      bool flag1 = this.goldenAnimalCracker.Value && !this.isPlayingGoldenCrackerAnimation.Value;
      if (flag1)
        b.Draw(this.texture.Value, Game1.GlobalToLocal(Game1.viewport, new Vector2((float) (this.tileX.Value * 64 /*0x40*/), (float) (this.tileY.Value * 64 /*0x40*/)) + new Vector2(65f, 59f) * 4f), new Microsoft.Xna.Framework.Rectangle?(new Microsoft.Xna.Framework.Rectangle(130, 160 /*0xA0*/, 15, 16 /*0x10*/)), this.color * this.alpha, 0.0f, Vector2.Zero, 4f, SpriteEffects.None, (float) ((((double) this.tileY.Value + 0.5) * 64.0 + 2.0) / 10000.0));
      if (this.output.Value == null)
        return;
      b.Draw(this.texture.Value, Game1.GlobalToLocal(Game1.viewport, new Vector2((float) (this.tileX.Value * 64 /*0x40*/), (float) (this.tileY.Value * 64 /*0x40*/)) + new Vector2(65f, 59f) * 4f), new Microsoft.Xna.Framework.Rectangle?(new Microsoft.Xna.Framework.Rectangle(0, 160 /*0xA0*/, 15, 16 /*0x10*/)), this.color * this.alpha, 0.0f, Vector2.Zero, 4f, SpriteEffects.None, (float) ((((double) this.tileY.Value + 0.5) * 64.0 + 1.0) / 10000.0));
      if (flag1)
        b.Draw(this.texture.Value, Game1.GlobalToLocal(Game1.viewport, new Vector2((float) (this.tileX.Value * 64 /*0x40*/), (float) (this.tileY.Value * 64 /*0x40*/)) + new Vector2(65f, 59f) * 4f), new Microsoft.Xna.Framework.Rectangle?(new Microsoft.Xna.Framework.Rectangle(145, 160 /*0xA0*/, 15, 16 /*0x10*/)), this.color * this.alpha, 0.0f, Vector2.Zero, 4f, SpriteEffects.None, (float) ((((double) this.tileY.Value + 0.5) * 64.0 + 3.0) / 10000.0));
      Vector2 vector2_1 = this.GetItemBucketTile() * 64f;
      totalGameTime = Game1.currentGameTime.TotalGameTime;
      Vector2 globalPosition1 = vector2_1 + new Vector2(0.0f, -2f) * 64f + new Vector2(0.0f, (float) (4.0 * Math.Round(Math.Sin(totalGameTime.TotalMilliseconds / 250.0), 2)));
      Vector2 vector2_2 = new Vector2(40f, 36f);
      float layerDepth1 = (float) (((double) vector2_1.Y + 64.0) / 10000.0 + 9.9999999747524271E-07);
      float layerDepth2 = (float) (((double) vector2_1.Y + 64.0) / 10000.0 + 9.9999997473787516E-06);
      b.Draw(Game1.mouseCursors, Game1.GlobalToLocal(Game1.viewport, globalPosition1), new Microsoft.Xna.Framework.Rectangle?(new Microsoft.Xna.Framework.Rectangle(141, 465, 20, 24)), Color.White * 0.75f, 0.0f, Vector2.Zero, 4f, SpriteEffects.None, layerDepth1);
      ParsedItemData dataOrErrorItem1 = ItemRegistry.GetDataOrErrorItem(this.output.Value.QualifiedItemId);
      Texture2D texture1 = dataOrErrorItem1.GetTexture();
      b.Draw(texture1, Game1.GlobalToLocal(Game1.viewport, globalPosition1 + vector2_2), new Microsoft.Xna.Framework.Rectangle?(dataOrErrorItem1.GetSourceRect()), Color.White * 0.75f, 0.0f, new Vector2(8f, 8f), 4f, SpriteEffects.None, layerDepth2);
      if (this.output.Value is ColoredObject coloredObject)
      {
        Microsoft.Xna.Framework.Rectangle sourceRect = ItemRegistry.GetDataOrErrorItem(this.output.Value.QualifiedItemId).GetSourceRect(1);
        b.Draw(texture1, Game1.GlobalToLocal(Game1.viewport, globalPosition1 + vector2_2), new Microsoft.Xna.Framework.Rectangle?(sourceRect), coloredObject.color.Value * 0.75f, 0.0f, new Vector2(8f, 8f), 4f, SpriteEffects.None, layerDepth2 + 1E-05f);
      }
      if (this.output.Value.Stack <= 1)
        return;
      Utility.drawTinyDigits(this.output.Value.Stack, b, Game1.GlobalToLocal(Game1.viewport, globalPosition1 + vector2_2 + new Vector2(16f, 12f)), 3f, layerDepth2 + 2E-05f, Color.LightYellow * this.alpha);
    }
  }

  /// <summary>Get whether an item can be placed on the fish pond as a sign.</summary>
  /// <param name="item">The item to check.</param>
  public bool IsValidSignItem(Item item)
  {
    if (item == null)
      return false;
    return item.HasContextTag("sign_item") || item.QualifiedItemId == "(BC)34";
  }
}
