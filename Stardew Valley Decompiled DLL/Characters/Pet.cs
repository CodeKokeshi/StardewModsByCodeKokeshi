// Decompiled with JetBrains decompiler
// Type: StardewValley.Characters.Pet
// Assembly: Stardew Valley, Version=1.6.15.24356, Culture=neutral, PublicKeyToken=null
// MVID: AA1F513A-94F0-4EF6-A35F-2D5B4A3721BF
// Assembly location: D:\SteamLibrary\steamapps\common\Stardew Valley\Stardew Valley.dll
// XML documentation location: D:\SteamLibrary\steamapps\common\Stardew Valley\Stardew Valley.xml

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Netcode;
using StardewValley.BellsAndWhistles;
using StardewValley.Buildings;
using StardewValley.Extensions;
using StardewValley.GameData;
using StardewValley.GameData.Pets;
using StardewValley.Internal;
using StardewValley.Locations;
using StardewValley.Network;
using StardewValley.Objects;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml.Serialization;

#nullable disable
namespace StardewValley.Characters;

public class Pet : NPC
{
  /// <summary>The cat's pet type ID in <c>Data/Pets</c>.</summary>
  public const string type_cat = "Cat";
  /// <summary>The dog's pet type ID in <c>Data/Pets</c>.</summary>
  public const string type_dog = "Dog";
  /// <summary>A unique ID for this pet.</summary>
  /// <remarks>This matches the <see cref="F:StardewValley.Buildings.PetBowl.petId" /> of the pet's bowl, if any. See also <see cref="M:StardewValley.Characters.Pet.GetPetBowl" />.</remarks>
  [XmlElement("guid")]
  public NetGuid petId = new NetGuid(Guid.NewGuid());
  public const int bedTime = 2000;
  public const int maxFriendship = 1000;
  public const string behavior_Walk = "Walk";
  public const string behavior_Sleep = "Sleep";
  public const string behavior_SitDown = "SitDown";
  public const string behavior_Sprint = "Sprint";
  protected int behaviorTimer = -1;
  protected int animationLoopsLeft;
  [XmlElement("petType")]
  public readonly NetString petType = new NetString("Dog");
  [XmlElement("whichBreed")]
  public readonly NetString whichBreed = new NetString("0");
  private readonly NetString netCurrentBehavior = new NetString();
  /// <summary>The unique name of the location containing the pet's bowl, if any.</summary>
  [XmlElement("homeLocationName")]
  public readonly NetString homeLocationName = new NetString();
  [XmlIgnore]
  public readonly NetEvent1Field<long, NetLong> petPushEvent = new NetEvent1Field<long, NetLong>();
  [XmlIgnore]
  protected string _currentBehavior;
  [XmlElement("lastPetDay")]
  public NetLongDictionary<int, NetInt> lastPetDay = new NetLongDictionary<int, NetInt>();
  [XmlElement("grantedFriendshipForPet")]
  public NetBool grantedFriendshipForPet = new NetBool(false);
  [XmlElement("friendshipTowardFarmer")]
  public NetInt friendshipTowardFarmer = new NetInt(0);
  [XmlElement("timesPet")]
  public NetInt timesPet = new NetInt(0);
  [XmlElement("hat")]
  public readonly NetRef<Hat> hat = new NetRef<Hat>();
  protected int _walkFromPushTimer;
  public NetBool isSleepingOnFarmerBed = new NetBool(false);
  [XmlIgnore]
  public readonly NetMutex mutex = new NetMutex();
  private int pushingTimer;

  /// <inheritdoc />
  [XmlIgnore]
  public override bool IsVillager => false;

  public override void reloadData()
  {
  }

  protected override string translateName() => this.name.Value.Trim();

  public Pet(int xTile, int yTile, string petBreed, string petType)
  {
    this.Name = petType;
    this.displayName = this.name.Value;
    this.petType.Value = petType;
    this.whichBreed.Value = petBreed;
    this.Sprite = new AnimatedSprite(this.getPetTextureName(), 0, 32 /*0x20*/, 32 /*0x20*/);
    this.Position = new Vector2((float) xTile, (float) yTile) * 64f;
    this.Breather = false;
    this.willDestroyObjectsUnderfoot = false;
    this.currentLocation = Game1.currentLocation;
    this.HideShadow = true;
  }

  public Pet()
    : this(0, 0, "0", "Dog")
  {
  }

  public string CurrentBehavior
  {
    get => this.netCurrentBehavior.Value;
    set
    {
      if (!(this.netCurrentBehavior.Value != value))
        return;
      this.netCurrentBehavior.Value = value;
    }
  }

  protected override void initNetFields()
  {
    base.initNetFields();
    this.NetFields.AddField((INetSerializable) this.petId, "petId").AddField((INetSerializable) this.petType, "petType").AddField((INetSerializable) this.whichBreed, "whichBreed").AddField((INetSerializable) this.netCurrentBehavior, "netCurrentBehavior").AddField((INetSerializable) this.homeLocationName, "homeLocationName").AddField((INetSerializable) this.petPushEvent, "petPushEvent").AddField((INetSerializable) this.lastPetDay, "lastPetDay").AddField((INetSerializable) this.grantedFriendshipForPet, "grantedFriendshipForPet").AddField((INetSerializable) this.friendshipTowardFarmer, "friendshipTowardFarmer").AddField((INetSerializable) this.isSleepingOnFarmerBed, "isSleepingOnFarmerBed").AddField((INetSerializable) this.mutex.NetFields, "mutex.NetFields").AddField((INetSerializable) this.hat, "hat").AddField((INetSerializable) this.timesPet, "timesPet");
    this.name.FilterStringEvent += new NetString.FilterString(Utility.FilterDirtyWords);
    this.name.fieldChangeVisibleEvent += (FieldChange<NetString, string>) ((_param1, _param2, _param3) => this.resetCachedDisplayName());
    this.petPushEvent.onEvent += new AbstractNetEvent1<long>.Event(this.OnPetPush);
    this.friendshipTowardFarmer.fieldChangeVisibleEvent += (FieldChange<NetInt, int>) ((field, old_value, new_value) => this.GrantLoveMailIfNecessary());
    this.isSleepingOnFarmerBed.fieldChangeVisibleEvent += (FieldChange<NetBool, bool>) ((a, b, c) => this.UpdateSleepingOnBed());
    this.petType.fieldChangeVisibleEvent += (FieldChange<NetString, string>) ((a, b, c) => this.reloadBreedSprite());
    this.whichBreed.fieldChangeVisibleEvent += (FieldChange<NetString, string>) ((a, b, c) => this.reloadBreedSprite());
    this.netCurrentBehavior.fieldChangeVisibleEvent += (FieldChange<NetString, string>) ((a, b, c) =>
    {
      if (!(this._currentBehavior != this.CurrentBehavior))
        return;
      this._OnNewBehavior();
    });
  }

  public virtual void OnPetPush(long farmerId)
  {
    this.pushingTimer = 0;
    if (!Game1.IsMasterGame)
      return;
    Farmer who = Game1.GetPlayer(farmerId) ?? Game1.player;
    Vector2 playerTrajectory = Utility.getAwayFromPlayerTrajectory(this.GetBoundingBox(), who);
    this.setTrajectory((int) playerTrajectory.X / 2, (int) playerTrajectory.Y / 2);
    this._walkFromPushTimer = 250;
    this.CurrentBehavior = "Walk";
    this.OnNewBehavior();
    this.Halt();
    this.faceDirection(who.FacingDirection);
    this.setMovingInFacingDirection();
  }

  public override int getTimeFarmerMustPushBeforeStartShaking() => 300;

  public override int getTimeFarmerMustPushBeforePassingThrough() => 750;

  public override void behaviorOnFarmerLocationEntry(GameLocation location, Farmer who)
  {
    base.behaviorOnFarmerLocationEntry(location, who);
    if (location is Farm && Game1.timeOfDay >= 2000 && !location.farmers.Any())
    {
      if (this.CurrentBehavior != "Sleep" || this.currentLocation is Farm)
        Game1.player.team.requestPetWarpHomeEvent.Fire(Game1.player.UniqueMultiplayerID);
    }
    else if (Game1.timeOfDay < 2000 && Game1.random.NextBool() && this._currentBehavior != "Sleep")
    {
      this.CurrentBehavior = "Sleep";
      this._OnNewBehavior();
      this.Sprite.UpdateSourceRect();
    }
    this.UpdateSleepingOnBed();
  }

  public override void behaviorOnLocalFarmerLocationEntry(GameLocation location)
  {
    base.behaviorOnLocalFarmerLocationEntry(location);
    this.netCurrentBehavior.CancelInterpolation();
    if (this.netCurrentBehavior.Value == "Sleep")
    {
      this.position.NetFields.CancelInterpolation();
      if (this._currentBehavior != "Sleep")
      {
        this._OnNewBehavior();
        this.Sprite.UpdateSourceRect();
      }
    }
    this.UpdateSleepingOnBed();
  }

  public override bool canTalk() => false;

  /// <summary>Get the data from <c>Data/Pets</c> for the pet type, if it's valid.</summary>
  public PetData GetPetData()
  {
    PetData data;
    return !Pet.TryGetData(this.petType.Value, out data) ? (PetData) null : data;
  }

  /// <summary>Get the underlying content data for a pet type, if any.</summary>
  /// <param name="petType">The pet type's ID in <c>Data/Pets</c>.</param>
  /// <param name="data">The pet data, if found.</param>
  /// <returns>Returns whether the pet data was found.</returns>
  public static bool TryGetData(string petType, out PetData data)
  {
    if (petType != null && Game1.petData.TryGetValue(petType, out data))
      return true;
    data = (PetData) null;
    return false;
  }

  /// <summary>Get the icon to show in the game menu for this pet.</summary>
  /// <param name="assetName">The asset name for the texture.</param>
  /// <param name="sourceRect">The 16x16 pixel area within the texture for the icon.</param>
  public void GetPetIcon(out string assetName, out Rectangle sourceRect)
  {
    PetData petData = this.GetPetData();
    PetBreed petBreed1 = petData?.GetBreedById(this.whichBreed.Value);
    if (petBreed1 == null)
    {
      PetBreed petBreed2;
      if (petData == null)
      {
        petBreed2 = (PetBreed) null;
      }
      else
      {
        List<PetBreed> breeds = petData.Breeds;
        petBreed2 = breeds != null ? breeds.FirstOrDefault<PetBreed>() : (PetBreed) null;
      }
      if (petBreed2 == null)
      {
        PetData data;
        if (!Pet.TryGetData("Dog", out data))
        {
          petBreed1 = (PetBreed) null;
        }
        else
        {
          List<PetBreed> breeds = data.Breeds;
          petBreed1 = breeds != null ? breeds.FirstOrDefault<PetBreed>() : (PetBreed) null;
        }
      }
      else
        petBreed1 = petBreed2;
    }
    PetBreed petBreed3 = petBreed1;
    if (petBreed3 != null)
    {
      assetName = petBreed3.IconTexture;
      sourceRect = petBreed3.IconSourceRect;
    }
    else
    {
      assetName = "Animals\\dog";
      sourceRect = new Rectangle(208 /*0xD0*/, 208 /*0xD0*/, 16 /*0x10*/, 16 /*0x10*/);
    }
  }

  public virtual string getPetTextureName()
  {
    try
    {
      PetData petData = this.GetPetData();
      if (petData != null)
        return petData.GetBreedById(this.whichBreed.Value).Texture;
    }
    catch (Exception ex)
    {
    }
    return "Animals\\dog";
  }

  public void reloadBreedSprite() => this.Sprite?.LoadTexture(this.getPetTextureName());

  /// <inheritdoc />
  public override void reloadSprite(bool onlyAppearance = false)
  {
    this.reloadBreedSprite();
    this.HideShadow = true;
    this.Breather = false;
    if (onlyAppearance)
      return;
    this.DefaultPosition = new Vector2(54f, 8f) * 64f;
    this.setAtFarmPosition();
    if (this.GetPetBowl() == null)
      this.warpToFarmHouse(Game1.MasterPlayer);
    this.GrantLoveMailIfNecessary();
  }

  /// <inheritdoc />
  public override void ChooseAppearance(LocalizedContentManager content = null)
  {
    if (this.Sprite?.Texture != null)
      return;
    this.reloadSprite(true);
  }

  public void warpToFarmHouse(Farmer who)
  {
    PetData petData = this.GetPetData();
    this.isSleepingOnFarmerBed.Value = false;
    FarmHouse homeOfFarmer = Utility.getHomeOfFarmer(who);
    int num1 = 0;
    Vector2 vector2 = new Vector2((float) Game1.random.Next(2, homeOfFarmer.map.Layers[0].LayerWidth - 3), (float) Game1.random.Next(3, homeOfFarmer.map.Layers[0].LayerHeight - 5));
    List<Furniture> options = new List<Furniture>();
    foreach (Furniture furniture in homeOfFarmer.furniture)
    {
      if (furniture.furniture_type.Value == 12)
        options.Add(furniture);
    }
    BedFurniture playerBed = homeOfFarmer.GetPlayerBed();
    float num2 = 0.0f;
    float num3 = 0.3f;
    float num4 = 0.5f;
    if (petData != null)
    {
      num2 = petData.SleepOnBedChance;
      num3 = petData.SleepNearBedChance;
      num4 = petData.SleepOnRugChance;
    }
    if (playerBed != null && !Game1.newDay && Game1.timeOfDay >= 2000 && Game1.random.NextDouble() <= (double) num2)
    {
      vector2 = Utility.PointToVector2(playerBed.GetBedSpot()) + new Vector2(-1f, 0.0f);
      if (homeOfFarmer.isCharacterAtTile(vector2) == null)
      {
        Game1.warpCharacter((NPC) this, (GameLocation) homeOfFarmer, vector2);
        this.NetFields.CancelInterpolation();
        this.CurrentBehavior = "Sleep";
        this.isSleepingOnFarmerBed.Value = true;
        Rectangle boundingBox = this.GetBoundingBox();
        foreach (Furniture furniture in homeOfFarmer.furniture)
        {
          if (furniture is BedFurniture bedFurniture && bedFurniture.GetBoundingBox().Intersects(boundingBox))
          {
            bedFurniture.ReserveForNPC();
            break;
          }
        }
        this.UpdateSleepingOnBed();
        this._OnNewBehavior();
        this.Sprite.UpdateSourceRect();
        return;
      }
    }
    else if (Game1.random.NextDouble() <= (double) num3)
      vector2 = Utility.PointToVector2(homeOfFarmer.getBedSpot()) + new Vector2(0.0f, 2f);
    else if (Game1.random.NextDouble() <= (double) num4)
    {
      Furniture furniture = Game1.random.ChooseFrom<Furniture>((IList<Furniture>) options);
      if (furniture != null)
        vector2 = Utility.getRandomPositionInThisRectangle(furniture.boundingBox.Value, Game1.random) / 64f;
    }
    for (; num1 < 50 && (!homeOfFarmer.canPetWarpHere(vector2) || !homeOfFarmer.CanItemBePlacedHere(vector2, collisionMask: CollisionMask.Buildings | CollisionMask.Characters | CollisionMask.Flooring | CollisionMask.Furniture | CollisionMask.Objects | CollisionMask.TerrainFeatures | CollisionMask.LocationSpecific) || !homeOfFarmer.CanItemBePlacedHere(vector2 + new Vector2(1f, 0.0f), collisionMask: CollisionMask.Buildings | CollisionMask.Characters | CollisionMask.Flooring | CollisionMask.Furniture | CollisionMask.Objects | CollisionMask.TerrainFeatures | CollisionMask.LocationSpecific) || homeOfFarmer.isTileOnWall((int) vector2.X, (int) vector2.Y)); ++num1)
      vector2 = new Vector2((float) Game1.random.Next(2, homeOfFarmer.map.Layers[0].LayerWidth - 3), (float) Game1.random.Next(3, homeOfFarmer.map.Layers[0].LayerHeight - 4));
    if (num1 < 50)
    {
      Game1.warpCharacter((NPC) this, (GameLocation) homeOfFarmer, vector2);
      this.CurrentBehavior = "Sleep";
    }
    else
      this.WarpToPetBowl();
    this.UpdateSleepingOnBed();
    this._OnNewBehavior();
    this.Sprite.UpdateSourceRect();
  }

  public virtual void UpdateSleepingOnBed()
  {
    this.drawOnTop = false;
    this.collidesWithOtherCharacters.Value = !this.isSleepingOnFarmerBed.Value;
    this.farmerPassesThrough = this.isSleepingOnFarmerBed.Value;
  }

  public override void dayUpdate(int dayOfMonth)
  {
    this.isSleepingOnFarmerBed.Value = false;
    this.UpdateSleepingOnBed();
    this.DefaultPosition = new Vector2(54f, 8f) * 64f;
    this.Sprite.loop = false;
    this.Breather = false;
    if (Game1.IsMasterGame && this.GetPetBowl() == null)
    {
      foreach (Building building in Game1.getFarm().buildings)
      {
        if (building is PetBowl petBowl && !petBowl.HasPet())
        {
          petBowl.AssignPet(this);
          break;
        }
      }
    }
    PetBowl petBowl1 = this.GetPetBowl();
    if (Game1.isRaining)
    {
      this.CurrentBehavior = "SitDown";
      this.warpToFarmHouse(Game1.player);
    }
    else if (petBowl1 != null && this.currentLocation is FarmHouse)
      this.setAtFarmPosition();
    else if (petBowl1 == null)
      this.warpToFarmHouse(Game1.player);
    if (Game1.IsMasterGame)
    {
      if (petBowl1 != null && petBowl1.watered.Value)
      {
        this.friendshipTowardFarmer.Set(Math.Min(1000, this.friendshipTowardFarmer.Value + 6));
        petBowl1.watered.Set(false);
      }
      if (petBowl1 == null)
        this.friendshipTowardFarmer.Value -= 10;
    }
    if (petBowl1 == null)
      Game1.addMorningFluffFunction((Action) (() => this.doEmote(28)));
    this.Halt();
    this.CurrentBehavior = "Sleep";
    this.grantedFriendshipForPet.Set(false);
    this._OnNewBehavior();
    this.Sprite.UpdateSourceRect();
  }

  public void GrantLoveMailIfNecessary()
  {
    if (this.friendshipTowardFarmer.Value < 1000)
      return;
    foreach (Farmer allFarmer in Game1.getAllFarmers())
    {
      if (allFarmer != null && allFarmer.mailReceived.Add("petLoveMessage") && allFarmer == Game1.player)
      {
        if (Game1.newDay)
          Game1.addMorningFluffFunction((Action) (() => Game1.showGlobalMessage(Game1.content.LoadString("Strings\\Characters:PetLovesYou", (object) this.displayName))));
        else
          Game1.showGlobalMessage(Game1.content.LoadString("Strings\\Characters:PetLovesYou", (object) this.displayName));
      }
      if (!allFarmer.hasOrWillReceiveMail("MarniePetAdoption"))
        Game1.addMailForTomorrow("MarniePetAdoption");
    }
  }

  /// <summary>Get the pet bowl assigned to this pet, if any.</summary>
  public PetBowl GetPetBowl()
  {
    foreach (Building building in (Game1.getLocationFromName(this.homeLocationName.Value) ?? (GameLocation) Game1.getFarm()).buildings)
    {
      if (building is PetBowl petBowl && petBowl.petId.Value == this.petId.Value)
        return petBowl;
    }
    return (PetBowl) null;
  }

  /// <summary>Warp the pet to its assigned pet bowl, if any.</summary>
  public virtual void WarpToPetBowl()
  {
    PetBowl petBowl = this.GetPetBowl();
    if (petBowl == null)
      return;
    this.faceDirection(2);
    Game1.warpCharacter((NPC) this, petBowl.parentLocationName.Value, petBowl.GetPetSpot());
  }

  public void setAtFarmPosition()
  {
    if (!Game1.IsMasterGame)
      return;
    if (!Game1.isRaining)
      this.WarpToPetBowl();
    else
      this.warpToFarmHouse(Game1.MasterPlayer);
  }

  public override bool shouldCollideWithBuildingLayer(GameLocation location) => true;

  public override bool canPassThroughActionTiles() => false;

  public void unassignPetBowl()
  {
    foreach (Building building in (Game1.getLocationFromName(this.homeLocationName.Value) ?? (GameLocation) Game1.getFarm()).buildings)
    {
      if (building is PetBowl petBowl && petBowl.petId.Value == this.petId.Value)
        petBowl.petId.Value = Guid.Empty;
    }
  }

  public void applyButterflyPowder(Farmer who, string responseKey)
  {
    if (!responseKey.Contains("Yes"))
      return;
    GameLocation currentLocation = this.currentLocation;
    this.unassignPetBowl();
    currentLocation.characters.Remove((NPC) this);
    this.playContentSound();
    Game1.playSound("fireball");
    Rectangle boundingBox = this.GetBoundingBox();
    boundingBox.Inflate(32 /*0x20*/, 32 /*0x20*/);
    boundingBox.X -= 32 /*0x20*/;
    boundingBox.Y -= 32 /*0x20*/;
    currentLocation.temporarySprites.AddRange((IEnumerable<TemporaryAnimatedSprite>) Utility.sparkleWithinArea(boundingBox, 6, Color.White, 50));
    currentLocation.temporarySprites.Add(new TemporaryAnimatedSprite(5, Utility.PointToVector2(this.GetBoundingBox().Center) - new Vector2(32f), Color.White, animationInterval: 50f));
    for (int index = 0; index < 8; ++index)
      currentLocation.temporarySprites.Add(new TemporaryAnimatedSprite("LooseSprites\\Cursors", new Rectangle(372, 1956, 10, 10), this.Position + new Vector2(32f) + new Vector2((float) Game1.random.Next(-16, 16 /*0x10*/), (float) Game1.random.Next(-32, 16 /*0x10*/)), false, 1f / 500f, Color.White)
      {
        alphaFade = 0.00433333358f,
        alpha = 0.75f,
        motion = new Vector2((float) Game1.random.Next(-10, 11) / 20f, -1f),
        acceleration = new Vector2(0.0f, 0.0f),
        interval = 99999f,
        layerDepth = 1f,
        scale = 3f,
        scaleChange = 0.01f,
        rotationChange = (float) ((double) Game1.random.Next(-5, 6) * 3.1415927410125732 / 256.0)
      });
    currentLocation.instantiateCrittersList();
    currentLocation.addCritter((Critter) new Butterfly(currentLocation, this.Tile + new Vector2(0.0f, 1f)));
    who.reduceActiveItemByOne();
    if (this.hat.Value != null)
      Game1.createItemDebris((Item) this.hat.Value, this.Position, -1, currentLocation);
    Game1.showGlobalMessage(Game1.content.LoadString("Strings\\1_6_Strings:ButterflyPowder_Goodbye", (object) this.Name));
  }

  public override bool checkAction(Farmer who, GameLocation l)
  {
    if (who.Items.Count > who.CurrentToolIndex && who.Items[who.CurrentToolIndex] != null && who.Items[who.CurrentToolIndex] is Hat && (this.petType.Value == "Cat" || this.petType.Value == "Dog"))
    {
      if (this.hat.Value != null)
      {
        Game1.createItemDebris((Item) this.hat.Value, this.Position, this.FacingDirection);
        this.hat.Value = (Hat) null;
      }
      else
      {
        Hat hat = who.Items[who.CurrentToolIndex] as Hat;
        who.Items[who.CurrentToolIndex] = (Item) null;
        this.hat.Value = hat;
        Game1.playSound("dirtyHit");
      }
      this.mutex.ReleaseLock();
    }
    if (who.CurrentItem != null && who.CurrentItem.QualifiedItemId.Equals("(O)ButterflyPowder"))
      l.createQuestionDialogue(Game1.content.LoadString("Strings\\1_6_Strings:ButterflyPowder_Question", (object) this.Name), l.createYesNoResponses(), new GameLocation.afterQuestionBehavior(this.applyButterflyPowder));
    int num;
    if (this.lastPetDay.TryGetValue(who.UniqueMultiplayerID, out num) && num == Game1.Date.TotalDays)
      return false;
    this.lastPetDay[who.UniqueMultiplayerID] = Game1.Date.TotalDays;
    this.mutex.RequestLock((Action) (() =>
    {
      if (!this.grantedFriendshipForPet.Value)
      {
        this.grantedFriendshipForPet.Set(true);
        this.friendshipTowardFarmer.Set(Math.Min(1000, this.friendshipTowardFarmer.Value + 12));
        if (Utility.CreateDaySaveRandom((double) this.timesPet.Value, 71928.0, (double) this.petId.Value.GetHashCode()).NextDouble() < (double) this.GetPetData().GiftChance)
        {
          Item giftItem = this.TryGetGiftItem(this.GetPetData().Gifts);
          if (giftItem != null)
            Game1.createMultipleItemDebris(giftItem, this.Position, -1, l, flopFish: true);
        }
        ++this.timesPet.Value;
      }
      this.mutex.ReleaseLock();
    }));
    this.doEmote(20);
    this.playContentSound();
    return true;
  }

  public virtual void playContentSound()
  {
    if (!Utility.isOnScreen(this.TilePoint, 128 /*0x80*/, this.currentLocation) || Game1.options.muteAnimalSounds)
      return;
    PetData petData = this.GetPetData();
    if (petData == null || petData.ContentSound == null)
      return;
    string contentSound = petData.ContentSound;
    this.PlaySound(contentSound, true, -1, -1);
    if (petData.RepeatContentSoundAfter < 0)
      return;
    DelayedAction.functionAfterDelay((Action) (() => this.PlaySound(contentSound, true, -1, -1)), petData.RepeatContentSoundAfter);
  }

  public void hold(Farmer who)
  {
    FarmerSprite.AnimationFrame animationFrame = this.Sprite.CurrentAnimation.Last<FarmerSprite.AnimationFrame>();
    this.flip = animationFrame.flip;
    this.Sprite.CurrentFrame = animationFrame.frame;
    this.Sprite.CurrentAnimation = (List<FarmerSprite.AnimationFrame>) null;
    this.Sprite.loop = false;
  }

  public override void behaviorOnFarmerPushing()
  {
    if (this.CurrentBehavior == "Sprint")
      return;
    this.pushingTimer += 2;
    if (this.pushingTimer <= 100)
      return;
    this.petPushEvent.Fire(Game1.player.UniqueMultiplayerID);
  }

  public override void update(GameTime time, GameLocation location, long id, bool move)
  {
    base.update(time, location, id, move);
    this.pushingTimer = Math.Max(0, this.pushingTimer - 1);
  }

  public override void update(GameTime time, GameLocation location)
  {
    base.update(time, location);
    this.petPushEvent.Poll();
    if (this.isSleepingOnFarmerBed.Value && this.CurrentBehavior != "Sleep" && Game1.IsMasterGame)
    {
      this.isSleepingOnFarmerBed.Value = false;
      this.UpdateSleepingOnBed();
    }
    if (this.currentLocation == null)
      this.currentLocation = location;
    this.mutex.Update(location);
    if (Game1.eventUp)
      return;
    if (this._currentBehavior != this.CurrentBehavior)
      this._OnNewBehavior();
    this.RunState(time);
    if (Game1.IsMasterGame)
    {
      PetBehavior currentPetBehavior = this.GetCurrentPetBehavior();
      if (currentPetBehavior != null && currentPetBehavior.WalkInDirection)
      {
        if (currentPetBehavior.Animation == null)
          this.MovePosition(time, Game1.viewport, location);
        else
          this.tryToMoveInDirection(this.FacingDirection, false, -1, false);
      }
    }
    this.flip = false;
    if (this.FacingDirection != 3 || this.Sprite.CurrentFrame < 16 /*0x10*/)
      return;
    this.flip = true;
  }

  public Item TryGetGiftItem(List<PetGift> gifts)
  {
    float totalWeight = 0.0f;
    gifts = new List<PetGift>((IEnumerable<PetGift>) gifts);
    gifts.RemoveAll((Predicate<PetGift>) (gift =>
    {
      if (this.friendshipTowardFarmer.Value < gift.MinimumFriendshipThreshold || !GameStateQuery.CheckConditions(gift.Condition))
        return true;
      totalWeight += gift.Weight;
      return false;
    }));
    if (gifts.Count > 0)
    {
      totalWeight = Utility.RandomFloat(0.0f, totalWeight);
      foreach (PetGift gift in gifts)
      {
        totalWeight -= gift.Weight;
        if ((double) totalWeight <= 0.0)
        {
          Item giftItem = ItemQueryResolver.TryResolveRandomItem((ISpawnItemData) gift, (ItemQueryContext) null);
          if (giftItem != null && !giftItem.Name.Contains("Error Item"))
            return giftItem;
        }
      }
    }
    return (Item) null;
  }

  public bool TryBehaviorChange(List<PetBehaviorChanges> changes)
  {
    float max = 0.0f;
    foreach (PetBehaviorChanges change in changes)
    {
      if (!change.OutsideOnly || this.currentLocation.IsOutdoors)
        max += change.Weight;
    }
    float num = Utility.RandomFloat(0.0f, max);
    foreach (PetBehaviorChanges change in changes)
    {
      if (!change.OutsideOnly || this.currentLocation.IsOutdoors)
      {
        num -= change.Weight;
        if ((double) num <= 0.0)
        {
          string str = (string) null;
          switch (this.FacingDirection)
          {
            case 0:
              str = change.UpBehavior;
              break;
            case 1:
              str = change.RightBehavior;
              break;
            case 2:
              str = change.DownBehavior;
              break;
            case 3:
              str = change.LeftBehavior;
              break;
          }
          if (str == null)
            str = change.Behavior;
          if (str != null)
            this.CurrentBehavior = str;
          return true;
        }
      }
    }
    return false;
  }

  public PetBehavior GetCurrentPetBehavior()
  {
    PetData petData = this.GetPetData();
    if (petData?.Behaviors != null)
    {
      foreach (PetBehavior behavior in petData.Behaviors)
      {
        if (behavior.Id == this.CurrentBehavior)
          return behavior;
      }
    }
    return (PetBehavior) null;
  }

  public virtual void RunState(GameTime time)
  {
    if (this._currentBehavior == "Walk" && Game1.IsMasterGame && this._walkFromPushTimer <= 0 && this.currentLocation.isCollidingPosition(this.nextPosition(this.FacingDirection), Game1.viewport, (Character) this))
    {
      int direction = Game1.random.Next(0, 4);
      if (!this.currentLocation.isCollidingPosition(this.nextPosition(this.FacingDirection), Game1.viewport, (Character) this))
        this.faceDirection(direction);
    }
    if (Game1.IsMasterGame && Game1.timeOfDay >= 2000 && this.Sprite.CurrentAnimation == null && (double) this.xVelocity == 0.0 && (double) this.yVelocity == 0.0)
      this.CurrentBehavior = "Sleep";
    if (this.CurrentBehavior == "Sleep")
    {
      if (Game1.IsMasterGame && Game1.timeOfDay < 2000 && Game1.random.NextDouble() < 0.001)
        this.CurrentBehavior = "Walk";
      if (Game1.random.NextDouble() < 0.002)
        this.doEmote(24);
    }
    TimeSpan elapsedGameTime;
    if (this._walkFromPushTimer > 0)
    {
      int walkFromPushTimer = this._walkFromPushTimer;
      elapsedGameTime = time.ElapsedGameTime;
      int totalMilliseconds = (int) elapsedGameTime.TotalMilliseconds;
      this._walkFromPushTimer = walkFromPushTimer - totalMilliseconds;
      if (this._walkFromPushTimer <= 0)
        this._walkFromPushTimer = 0;
    }
    PetBehavior currentPetBehavior = this.GetCurrentPetBehavior();
    if (currentPetBehavior == null || !Game1.IsMasterGame)
      return;
    if (this.behaviorTimer >= 0)
    {
      int behaviorTimer = this.behaviorTimer;
      elapsedGameTime = time.ElapsedGameTime;
      int totalMilliseconds = (int) elapsedGameTime.TotalMilliseconds;
      this.behaviorTimer = behaviorTimer - totalMilliseconds;
      if (this.behaviorTimer <= 0)
      {
        this.behaviorTimer = -1;
        this.TryBehaviorChange(currentPetBehavior.TimeoutBehaviorChanges);
        return;
      }
    }
    if (this._walkFromPushTimer <= 0)
    {
      if (currentPetBehavior.RandomBehaviorChanges != null && (double) currentPetBehavior.RandomBehaviorChangeChance > 0.0 && Game1.random.NextDouble() < (double) currentPetBehavior.RandomBehaviorChangeChance)
      {
        this.TryBehaviorChange(currentPetBehavior.RandomBehaviorChanges);
        return;
      }
      if (currentPetBehavior.PlayerNearbyBehaviorChanges != null && this.withinPlayerThreshold(2))
      {
        this.TryBehaviorChange(currentPetBehavior.PlayerNearbyBehaviorChanges);
        return;
      }
    }
    if (currentPetBehavior.JumpLandBehaviorChanges == null || this.yJumpOffset != 0 || (double) this.yJumpVelocity != 0.0)
      return;
    this.TryBehaviorChange(currentPetBehavior.JumpLandBehaviorChanges);
  }

  protected override void updateSlaveAnimation(GameTime time)
  {
    if (this.Sprite.CurrentAnimation != null)
    {
      this.Sprite.animateOnce(time);
    }
    else
    {
      if (!(this.CurrentBehavior == "Walk"))
        return;
      this.Sprite.faceDirection(this.FacingDirection);
      if (this.isMoving())
      {
        this.animateInFacingDirection(time);
        int num = -1;
        switch (this.FacingDirection)
        {
          case 0:
            num = 12;
            break;
          case 1:
            num = 8;
            break;
          case 2:
            num = 4;
            break;
          case 3:
            num = 16 /*0x10*/;
            break;
        }
        if (this.Sprite.CurrentFrame != num)
          return;
        this.Sprite.CurrentFrame -= 4;
      }
      else
        this.Sprite.StopAnimation();
    }
  }

  protected void _OnNewBehavior()
  {
    this._currentBehavior = this.CurrentBehavior;
    this.Halt();
    this.Sprite.CurrentAnimation = (List<FarmerSprite.AnimationFrame>) null;
    this.OnNewBehavior();
  }

  public virtual void OnNewBehavior()
  {
    this.Sprite.loop = false;
    this.Sprite.CurrentAnimation = (List<FarmerSprite.AnimationFrame>) null;
    this.behaviorTimer = -1;
    this.animationLoopsLeft = -1;
    if (this.CurrentBehavior == "Sleep")
    {
      this.Sprite.loop = true;
      bool flip = Game1.random.NextBool();
      this.Sprite.setCurrentAnimation(new List<FarmerSprite.AnimationFrame>()
      {
        new FarmerSprite.AnimationFrame(28, 1000, false, flip),
        new FarmerSprite.AnimationFrame(29, 1000, false, flip)
      });
    }
    PetBehavior currentPetBehavior = this.GetCurrentPetBehavior();
    if (currentPetBehavior == null)
      return;
    if (Game1.IsMasterGame)
    {
      if (this._walkFromPushTimer <= 0)
      {
        int parsed;
        if (Utility.TryParseDirection(currentPetBehavior.Direction, out parsed))
          this.FacingDirection = parsed;
        if (currentPetBehavior.RandomizeDirection)
          this.FacingDirection = currentPetBehavior.IsSideBehavior ? Game1.random.Choose<int>(3, 1) : Game1.random.Next(4);
      }
      if ((this.FacingDirection == 0 || this.FacingDirection == 2) && currentPetBehavior.IsSideBehavior)
        this.FacingDirection = Game1.random.NextBool() ? 3 : 1;
      if (currentPetBehavior.WalkInDirection)
      {
        if (currentPetBehavior.MoveSpeed >= 0)
          this.speed = currentPetBehavior.MoveSpeed;
        this.setMovingInFacingDirection();
      }
      if (currentPetBehavior.Duration >= 0)
        this.behaviorTimer = currentPetBehavior.Duration;
      else if (currentPetBehavior.MinimumDuration >= 0 && currentPetBehavior.MaximumDuration >= 0)
        this.behaviorTimer = Game1.random.Next(currentPetBehavior.MinimumDuration, currentPetBehavior.MaximumDuration + 1);
    }
    if (currentPetBehavior.SoundOnStart != null)
      this.PlaySound(currentPetBehavior.SoundOnStart, currentPetBehavior.SoundIsVoice, currentPetBehavior.SoundRangeFromBorder, currentPetBehavior.SoundRange);
    if (currentPetBehavior.Shake > 0)
      this.shake(currentPetBehavior.Shake);
    if (currentPetBehavior.Animation == null)
      return;
    this.Sprite.ClearAnimation();
    for (int index = 0; index < currentPetBehavior.Animation.Count; ++index)
    {
      FarmerSprite.AnimationFrame frame = new FarmerSprite.AnimationFrame(currentPetBehavior.Animation[index].Frame, currentPetBehavior.Animation[index].Duration, false, false);
      if (currentPetBehavior.Animation[index].HitGround)
        frame.AddFrameAction(new AnimatedSprite.endOfAnimationBehavior(this.hitGround));
      if (currentPetBehavior.Animation[index].Jump)
        this.jump();
      if (currentPetBehavior.AnimationMinimumLoops >= 0 && currentPetBehavior.AnimationMaximumLoops >= 0)
        this.animationLoopsLeft = Game1.random.Next(currentPetBehavior.AnimationMinimumLoops, currentPetBehavior.AnimationMaximumLoops + 1);
      if (currentPetBehavior.Animation[index].Sound != null)
        frame.AddFrameAction(new AnimatedSprite.endOfAnimationBehavior(this._PerformAnimationSound));
      if (index == currentPetBehavior.Animation.Count - 1)
      {
        if (this.animationLoopsLeft > 0 || currentPetBehavior.AnimationEndBehaviorChanges != null)
          frame.AddFrameEndAction(new AnimatedSprite.endOfAnimationBehavior(this._TryAnimationEndBehaviorChange));
        if (currentPetBehavior.LoopMode == PetAnimationLoopMode.Hold)
        {
          if (currentPetBehavior.AnimationEndBehaviorChanges != null)
            frame.AddFrameEndAction(new AnimatedSprite.endOfAnimationBehavior(this.hold));
          else
            frame.AddFrameAction(new AnimatedSprite.endOfAnimationBehavior(this.hold));
        }
      }
      this.Sprite.AddFrame(frame);
      if (currentPetBehavior.Animation.Count == 1 && currentPetBehavior.LoopMode == PetAnimationLoopMode.Hold)
        this.Sprite.AddFrame(frame);
      this.Sprite.UpdateSourceRect();
    }
    this.Sprite.loop = currentPetBehavior.LoopMode == PetAnimationLoopMode.Loop || this.animationLoopsLeft > 0;
  }

  public void _PerformAnimationSound(Farmer who)
  {
    PetBehavior currentPetBehavior = this.GetCurrentPetBehavior();
    if (currentPetBehavior?.Animation == null || this.Sprite.currentAnimationIndex < 0 || this.Sprite.currentAnimationIndex >= currentPetBehavior.Animation.Count)
      return;
    PetAnimationFrame petAnimationFrame = currentPetBehavior.Animation[this.Sprite.currentAnimationIndex];
    if (petAnimationFrame.Sound == null)
      return;
    this.PlaySound(petAnimationFrame.Sound, petAnimationFrame.SoundIsVoice, petAnimationFrame.SoundRangeFromBorder, petAnimationFrame.SoundRange);
  }

  public void PlaySound(string sound, bool is_voice, int range_from_border, int range)
  {
    if (Game1.options.muteAnimalSounds & is_voice || !this.IsSoundInRange(range_from_border, range))
      return;
    float num = 1f;
    PetBreed breedById = this.GetPetData().GetBreedById(this.whichBreed.Value);
    if (sound == "BARK")
    {
      sound = this.GetPetData().BarkSound;
      if (breedById.BarkOverride != null)
        sound = breedById.BarkOverride;
    }
    if (is_voice)
      num = breedById.VoicePitch;
    if ((double) num != 1.0)
      this.playNearbySoundAll(sound, new int?((int) (1200.0 * (double) num)));
    else
      Game1.playSound(sound);
  }

  public bool IsSoundInRange(int range_from_border, int sound_range)
  {
    if (sound_range > 0)
      return this.withinLocalPlayerThreshold(sound_range);
    return range_from_border <= 0 || Utility.isOnScreen(this.TilePoint, range_from_border * 64 /*0x40*/, this.currentLocation);
  }

  public virtual void _TryAnimationEndBehaviorChange(Farmer who)
  {
    if (this.animationLoopsLeft <= 0)
    {
      if (this.animationLoopsLeft == 0)
      {
        this.animationLoopsLeft = -1;
        this.hold(who);
      }
      PetBehavior currentPetBehavior = this.GetCurrentPetBehavior();
      if (currentPetBehavior == null || !Game1.IsMasterGame)
        return;
      this.TryBehaviorChange(currentPetBehavior.AnimationEndBehaviorChanges);
    }
    else
      --this.animationLoopsLeft;
  }

  public override Rectangle GetBoundingBox()
  {
    Vector2 position = this.Position;
    return new Rectangle((int) position.X + 16 /*0x10*/, (int) position.Y + 16 /*0x10*/, this.Sprite.SpriteWidth * 4 * 3 / 4, 32 /*0x20*/);
  }

  public virtual void drawHat(SpriteBatch b, Vector2 shake)
  {
    if (this.hat.Value == null)
      return;
    Vector2 vector2 = Vector2.Zero * 4f;
    if ((double) vector2.X <= -100.0)
      return;
    float num = Math.Max(0.0f, this.isSleepingOnFarmerBed.Value ? (float) (((double) this.StandingPixel.Y + 112.0) / 10000.0) : (float) this.StandingPixel.Y / 10000f);
    vector2.X = -2f;
    vector2.Y = -24f;
    float layerDepth = num + 1E-07f;
    int direction = 2;
    bool flag = this.flip || this.sprite.Value.CurrentAnimation != null && this.sprite.Value.CurrentAnimation[this.sprite.Value.currentAnimationIndex].flip;
    float scaleSize = 1.33333337f;
    switch (this.petType.Value)
    {
      case "Cat":
        switch (this.Sprite.CurrentFrame)
        {
          case 0:
          case 2:
            vector2.Y += 28f;
            direction = 2;
            break;
          case 1:
          case 3:
            vector2.Y += 32f;
            direction = 2;
            break;
          case 4:
          case 6:
            direction = 1;
            vector2.X += 23f;
            vector2.Y += 20f;
            break;
          case 5:
          case 7:
            vector2.Y += 4f;
            direction = 1;
            vector2.X += 23f;
            vector2.Y += 20f;
            break;
          case 8:
          case 10:
            direction = 0;
            vector2.Y -= 4f;
            break;
          case 9:
          case 11:
            direction = 0;
            break;
          case 12:
          case 14:
            direction = 3;
            vector2.X -= 22f;
            vector2.Y += 20f;
            break;
          case 13:
          case 15:
            vector2.Y += 20f;
            vector2.Y += 4f;
            direction = 3;
            vector2.X -= 22f;
            break;
          case 16 /*0x10*/:
            vector2.Y += 20f;
            direction = 2;
            break;
          case 17:
          case 20:
          case 22:
            vector2.Y += 12f;
            break;
          case 18:
          case 19:
            vector2.Y += 8f;
            break;
          case 21:
          case 23:
            vector2.Y += 16f;
            break;
          case 24:
            direction = flag ? 3 : 1;
            vector2.X += (float) ((flag ? -1 : 1) * 29);
            vector2.Y += 28f;
            break;
          case 25:
            direction = flag ? 3 : 1;
            vector2.X += (float) ((flag ? -1 : 1) * 29);
            vector2.Y += 36f;
            break;
          case 26:
            direction = flag ? 3 : 1;
            vector2.X += (float) ((flag ? -1 : 1) * 29);
            vector2.Y += 40f;
            break;
          case 27:
            direction = flag ? 3 : 1;
            vector2.X += (float) ((flag ? -1 : 1) * 29);
            vector2.Y += 44f;
            break;
          case 28:
          case 29:
            scaleSize = 1.2f;
            vector2.Y += 46f;
            vector2.X -= (float) ((flag ? 0 : -1) * 4);
            vector2.X += (float) ((flag ? -1 : 1) * 2);
            direction = flag ? 1 : 3;
            break;
          case 30:
          case 31 /*0x1F*/:
            direction = flag ? 3 : 1;
            vector2.X += (float) ((flag ? -1 : 1) * 25);
            vector2.Y += 32f;
            break;
        }
        if ((this.whichBreed.Value == "3" || this.whichBreed.Value == "4") && direction == 3)
        {
          vector2.X -= 4f;
          break;
        }
        break;
      case "Dog":
        vector2.Y -= 20f;
        switch (this.Sprite.CurrentFrame)
        {
          case 0:
          case 2:
            vector2.Y += 28f;
            direction = 2;
            break;
          case 1:
          case 3:
            vector2.Y += 32f;
            direction = 2;
            break;
          case 4:
          case 6:
            direction = 1;
            vector2.X += 26f;
            vector2.Y += 24f;
            break;
          case 5:
          case 7:
            direction = 1;
            vector2.X += 26f;
            vector2.Y += 28f;
            break;
          case 8:
          case 10:
            direction = 0;
            vector2.Y += 4f;
            break;
          case 9:
          case 11:
            direction = 0;
            vector2.Y += 8f;
            break;
          case 12:
          case 14:
            direction = 3;
            vector2.X -= 26f;
            vector2.Y += 24f;
            break;
          case 13:
          case 15:
            vector2.Y += 24f;
            vector2.Y += 4f;
            direction = 3;
            vector2.X -= 26f;
            break;
          case 16 /*0x10*/:
            vector2.Y += 20f;
            direction = 2;
            break;
          case 17:
            vector2.Y += 12f;
            break;
          case 18:
          case 19:
            vector2.Y += 8f;
            break;
          case 20:
            direction = flag ? 3 : 1;
            vector2.X += 26f;
            vector2.Y += this.whichBreed.Value == "2" ? 16f : (this.whichBreed.Value == "1" ? 24f : 20f);
            break;
          case 21:
            direction = flag ? 3 : 1;
            vector2.X += 22f;
            vector2.Y += this.whichBreed.Value == "2" ? 12f : (this.whichBreed.Value == "1" ? 20f : 16f);
            break;
          case 22:
            direction = flag ? 3 : 1;
            vector2.X += 18f;
            vector2.Y += this.whichBreed.Value == "2" ? 8f : (this.whichBreed.Value == "1" ? 8f : 12f);
            break;
          case 23:
            direction = flag ? 3 : 1;
            vector2.X += 18f;
            vector2.Y += 8f;
            break;
          case 24:
          case 25:
            direction = flag ? 3 : 1;
            vector2.X += (float) (21 - (flag ? 4 : 4) + 1);
            vector2.Y += 8f;
            break;
          case 26:
            direction = flag ? 3 : 1;
            vector2.X += 18f;
            vector2.Y -= 8f;
            break;
          case 27:
            direction = 2;
            vector2.Y += (float) (12 + (this.whichBreed.Value == "2" ? -4 : 0));
            break;
          case 28:
          case 29:
            scaleSize = 1.33333337f;
            vector2.Y += 48f;
            vector2.X += (float) ((flag ? 6 : 5) * 4);
            vector2.X += 2f;
            direction = 2;
            break;
          case 30:
          case 31 /*0x1F*/:
            direction = flag ? 3 : 1;
            vector2.X += 18f;
            vector2.Y += 8f;
            break;
          case 32 /*0x20*/:
            direction = flag ? 3 : 1;
            vector2.X += 26f;
            vector2.Y += this.whichBreed.Value == "2" ? 12f : 16f;
            break;
          case 33:
            direction = flag ? 3 : 1;
            vector2.X += 26f;
            vector2.Y += this.whichBreed.Value == "2" ? 16f : 20f;
            break;
          case 34:
            direction = flag ? 3 : 1;
            vector2.X += 26f;
            vector2.Y += this.whichBreed.Value == "2" ? 20f : 24f;
            break;
        }
        switch (this.whichBreed.Value)
        {
          case "2":
            if (direction == 1)
              vector2.X -= 4f;
            vector2.Y += 8f;
            break;
          case "3":
            if (direction == 3 && this.Sprite.CurrentFrame > 16 /*0x10*/)
            {
              vector2.X += 4f;
              break;
            }
            break;
        }
        if (flag)
        {
          vector2.X *= -1f;
          break;
        }
        break;
    }
    vector2 += shake;
    if (flag)
      vector2.X -= 4f;
    this.hat.Value.draw(b, this.getLocalPosition(Game1.viewport) + vector2 + new Vector2(30f, -42f), scaleSize, 1f, layerDepth, direction, true);
  }

  public override void draw(SpriteBatch b)
  {
    int y = this.StandingPixel.Y;
    Vector2 shake = this.shakeTimer <= 0 || this.isSleepingOnFarmerBed.Value ? Vector2.Zero : new Vector2((float) Game1.random.Next(-1, 2), (float) Game1.random.Next(-1, 2));
    b.Draw(this.Sprite.Texture, this.getLocalPosition(Game1.viewport) + new Vector2((float) (this.Sprite.SpriteWidth * 4 / 2), (float) (this.GetBoundingBox().Height / 2)) + shake, new Rectangle?(this.Sprite.SourceRect), Color.White, this.rotation, new Vector2((float) (this.Sprite.SpriteWidth / 2), (float) ((double) this.Sprite.SpriteHeight * 3.0 / 4.0)), Math.Max(0.2f, this.scale.Value) * 4f, this.flip || this.Sprite.CurrentAnimation != null && this.Sprite.CurrentAnimation[this.Sprite.currentAnimationIndex].flip ? SpriteEffects.FlipHorizontally : SpriteEffects.None, Math.Max(0.0f, this.isSleepingOnFarmerBed.Value ? (float) (((double) y + 112.0) / 10000.0) : (float) y / 10000f));
    this.drawHat(b, shake);
    if (!this.IsEmoting)
      return;
    Vector2 localPosition = this.getLocalPosition(Game1.viewport);
    PetData petData = this.GetPetData();
    Point point = petData != null ? petData.EmoteOffset : Point.Zero;
    Vector2 position = new Vector2(localPosition.X + 32f + (float) point.X, localPosition.Y - 96f + (float) point.Y);
    b.Draw(Game1.emoteSpriteSheet, position, new Rectangle?(new Rectangle(this.CurrentEmoteIndex * 16 /*0x10*/ % Game1.emoteSpriteSheet.Width, this.CurrentEmoteIndex * 16 /*0x10*/ / Game1.emoteSpriteSheet.Width * 16 /*0x10*/, 16 /*0x10*/, 16 /*0x10*/)), Color.White, 0.0f, Vector2.Zero, 4f, SpriteEffects.None, (float) ((double) y / 10000.0 + 9.9999997473787516E-05));
  }

  public virtual bool withinLocalPlayerThreshold(int threshold)
  {
    if (this.currentLocation != Game1.currentLocation)
      return false;
    Vector2 tile1 = this.Tile;
    Vector2 tile2 = Game1.player.Tile;
    return (double) Math.Abs(tile1.X - tile2.X) <= (double) threshold && (double) Math.Abs(tile1.Y - tile2.Y) <= (double) threshold;
  }

  public override bool withinPlayerThreshold(int threshold)
  {
    if (this.currentLocation != null && !this.currentLocation.farmers.Any())
      return false;
    Vector2 tile1 = this.Tile;
    foreach (Character farmer in this.currentLocation.farmers)
    {
      Vector2 tile2 = farmer.Tile;
      if ((double) Math.Abs(tile1.X - tile2.X) <= (double) threshold && (double) Math.Abs(tile1.Y - tile2.Y) <= (double) threshold)
        return true;
    }
    return false;
  }

  public void hitGround(Farmer who)
  {
    if (!Utility.isOnScreen(this.TilePoint, 128 /*0x80*/, this.currentLocation))
      return;
    this.currentLocation.playTerrainSound(this.Tile, (Character) this, false);
  }
}
