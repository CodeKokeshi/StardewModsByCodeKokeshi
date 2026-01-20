// Decompiled with JetBrains decompiler
// Type: StardewValley.FarmAnimal
// Assembly: Stardew Valley, Version=1.6.15.24356, Culture=neutral, PublicKeyToken=null
// MVID: AA1F513A-94F0-4EF6-A35F-2D5B4A3721BF
// Assembly location: D:\SteamLibrary\steamapps\common\Stardew Valley\Stardew Valley.dll
// XML documentation location: D:\SteamLibrary\steamapps\common\Stardew Valley\Stardew Valley.xml

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Netcode;
using StardewValley.Buildings;
using StardewValley.Extensions;
using StardewValley.GameData;
using StardewValley.GameData.Buildings;
using StardewValley.GameData.FarmAnimals;
using StardewValley.Menus;
using StardewValley.Monsters;
using StardewValley.Network;
using StardewValley.Objects;
using StardewValley.Pathfinding;
using StardewValley.TerrainFeatures;
using StardewValley.TokenizableStrings;
using StardewValley.Tools;
using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Xml.Serialization;
using xTile.Dimensions;

#nullable disable
namespace StardewValley;

public class FarmAnimal : Character
{
  public const byte eatGrassBehavior = 0;
  public const short newHome = 0;
  public const short happy = 1;
  public const short neutral = 2;
  public const short unhappy = 3;
  public const short hungry = 4;
  public const short disturbedByDog = 5;
  public const short leftOutAtNight = 6;
  public const double chancePerUpdateToChangeDirection = 0.007;
  public const byte fullnessValueOfGrass = 60;
  public const int noWarpTimerTime = 3000;
  public new const double chanceForSound = 0.002;
  public const double chanceToGoOutside = 0.002;
  public const int uniqueDownFrame = 16 /*0x10*/;
  public const int uniqueRightFrame = 18;
  public const int uniqueUpFrame = 20;
  public const int uniqueLeftFrame = 22;
  public const int pushAccumulatorTimeTillPush = 60;
  public const int timePerUniqueFrame = 500;
  /// <summary>The texture name to load if the animal's actual sprite can't be loaded.</summary>
  public const string ErrorTextureName = "Animals\\Error";
  /// <summary>The pixel size of sprites in the <see cref="F:StardewValley.FarmAnimal.ErrorTextureName" />.</summary>
  public const int ErrorSpriteSize = 16 /*0x10*/;
  public NetBool isSwimming = new NetBool();
  [XmlIgnore]
  public Vector2 hopOffset = new Vector2(0.0f, 0.0f);
  [XmlElement("currentProduce")]
  public readonly NetString currentProduce = new NetString();
  [XmlElement("friendshipTowardFarmer")]
  public readonly NetInt friendshipTowardFarmer = new NetInt();
  [XmlElement("skinID")]
  public readonly NetString skinID = new NetString();
  [XmlIgnore]
  public int pushAccumulator;
  [XmlIgnore]
  public int uniqueFrameAccumulator = -1;
  [XmlElement("age")]
  public readonly NetInt age = new NetInt();
  [XmlElement("daysOwned")]
  public readonly NetInt daysOwned = new NetInt(-1);
  [XmlElement("health")]
  public readonly NetInt health = new NetInt();
  [XmlElement("produceQuality")]
  public readonly NetInt produceQuality = new NetInt();
  [XmlElement("daysSinceLastLay")]
  public readonly NetInt daysSinceLastLay = new NetInt();
  [XmlElement("happiness")]
  public readonly NetInt happiness = new NetInt();
  [XmlElement("fullness")]
  public readonly NetInt fullness = new NetInt();
  [XmlElement("wasAutoPet")]
  public readonly NetBool wasAutoPet = new NetBool();
  [XmlElement("wasPet")]
  public readonly NetBool wasPet = new NetBool();
  [XmlElement("allowReproduction")]
  public readonly NetBool allowReproduction = new NetBool(true);
  [XmlElement("type")]
  public readonly NetString type = new NetString();
  [XmlElement("buildingTypeILiveIn")]
  public readonly NetString buildingTypeILiveIn = new NetString();
  [XmlElement("myID")]
  public readonly NetLong myID = new NetLong();
  [XmlElement("ownerID")]
  public readonly NetLong ownerID = new NetLong();
  [XmlElement("parentId")]
  public readonly NetLong parentId = new NetLong(-1L);
  [XmlIgnore]
  private readonly NetLocationRef netHomeInterior = new NetLocationRef();
  [XmlElement("hasEatenAnimalCracker")]
  public readonly NetBool hasEatenAnimalCracker = new NetBool();
  [XmlIgnore]
  public int noWarpTimer;
  [XmlIgnore]
  public int hitGlowTimer;
  [XmlIgnore]
  public int pauseTimer;
  [XmlElement("moodMessage")]
  public readonly NetInt moodMessage = new NetInt();
  [XmlElement("isEating")]
  public readonly NetBool isEating = new NetBool();
  [XmlIgnore]
  private readonly NetEvent1Field<int, NetInt> doFarmerPushEvent = new NetEvent1Field<int, NetInt>();
  [XmlIgnore]
  private readonly NetEvent0 doBuildingPokeEvent = new NetEvent0();
  [XmlIgnore]
  private readonly NetEvent0 doDiveEvent = new NetEvent0();
  private string _displayHouse;
  private string _displayType;
  public static int NumPathfindingThisTick = 0;
  public static int MaxPathfindingPerTick = 1;
  [XmlIgnore]
  public int nextRipple;
  [XmlIgnore]
  public int nextFollowDirectionChange;
  protected FarmAnimal _followTarget;
  protected Point? _followTargetPosition;
  protected float _nextFollowTargetScan = 1f;
  [XmlIgnore]
  public int bobOffset;
  [XmlIgnore]
  protected Vector2 _swimmingVelocity = Vector2.Zero;
  [XmlIgnore]
  public static HashSet<Grass> reservedGrass = new HashSet<Grass>();
  [XmlIgnore]
  public Grass foundGrass;

  /// <summary>The building within which the animal is normally housed, if any.</summary>
  [XmlIgnore]
  public Building home
  {
    get => this.netHomeInterior.Value?.ParentBuilding;
    set => this.netHomeInterior.Value = value?.GetIndoors();
  }

  [XmlIgnore]
  public GameLocation homeInterior
  {
    get => this.netHomeInterior.Value;
    set => this.netHomeInterior.Value = value;
  }

  [XmlIgnore]
  public string displayHouse
  {
    get
    {
      if (this._displayHouse == null)
      {
        FarmAnimalData animalData = this.GetAnimalData();
        BuildingData buildingData;
        this._displayHouse = animalData == null ? this.buildingTypeILiveIn.Value : (Game1.buildingData.TryGetValue(animalData.House, out buildingData) ? TokenParser.ParseText(buildingData.Name) : animalData.House);
      }
      return this._displayHouse;
    }
    set => this._displayHouse = value;
  }

  [XmlIgnore]
  public string displayType
  {
    get
    {
      if (this._displayType == null)
        this._displayType = TokenParser.ParseText(this.GetAnimalData()?.DisplayName);
      return this._displayType;
    }
    set => this._displayType = value;
  }

  public override string displayName
  {
    get => this.Name;
    set
    {
    }
  }

  /// <summary>Get whether the farm animal is currently inside their home building.</summary>
  [MemberNotNullWhen(true, "home")]
  public bool IsHome
  {
    [MemberNotNullWhen(true, "home")] get
    {
      GameLocation homeInterior = this.homeInterior;
      return homeInterior != null && homeInterior.animals.ContainsKey(this.myID.Value);
    }
  }

  public FarmAnimal()
  {
  }

  protected override void initNetFields()
  {
    this.bobOffset = Game1.random.Next(0, 1000);
    base.initNetFields();
    this.NetFields.AddField((INetSerializable) this.currentProduce, "currentProduce").AddField((INetSerializable) this.friendshipTowardFarmer, "friendshipTowardFarmer").AddField((INetSerializable) this.age, "age").AddField((INetSerializable) this.health, "health").AddField((INetSerializable) this.produceQuality, "produceQuality").AddField((INetSerializable) this.daysSinceLastLay, "daysSinceLastLay").AddField((INetSerializable) this.happiness, "happiness").AddField((INetSerializable) this.fullness, "fullness").AddField((INetSerializable) this.wasPet, "wasPet").AddField((INetSerializable) this.wasAutoPet, "wasAutoPet").AddField((INetSerializable) this.allowReproduction, "allowReproduction").AddField((INetSerializable) this.type, "type").AddField((INetSerializable) this.buildingTypeILiveIn, "buildingTypeILiveIn").AddField((INetSerializable) this.myID, "myID").AddField((INetSerializable) this.ownerID, "ownerID").AddField((INetSerializable) this.parentId, "parentId").AddField((INetSerializable) this.netHomeInterior.NetFields, "netHomeInterior.NetFields").AddField((INetSerializable) this.moodMessage, "moodMessage").AddField((INetSerializable) this.isEating, "isEating").AddField((INetSerializable) this.doFarmerPushEvent, "doFarmerPushEvent").AddField((INetSerializable) this.doBuildingPokeEvent, "doBuildingPokeEvent").AddField((INetSerializable) this.isSwimming, "isSwimming").AddField(this.doDiveEvent.NetFields, "doDiveEvent.NetFields").AddField((INetSerializable) this.daysOwned, "daysOwned").AddField((INetSerializable) this.skinID, "skinID").AddField((INetSerializable) this.hasEatenAnimalCracker, "hasEatenAnimalCracker");
    this.position.Field.AxisAlignedMovement = true;
    this.doFarmerPushEvent.onEvent += new AbstractNetEvent1<int>.Event(this.doFarmerPush);
    this.doBuildingPokeEvent.onEvent += new NetEvent0.Event(this.doBuildingPoke);
    this.doDiveEvent.onEvent += new NetEvent0.Event(this.doDive);
    this.skinID.fieldChangeVisibleEvent += (FieldChange<NetString, string>) ((a, b, c) =>
    {
      if (Game1.gameMode == (byte) 6)
        return;
      this.ReloadTextureIfNeeded();
    });
    this.isSwimming.fieldChangeVisibleEvent += (FieldChange<NetBool, bool>) ((a, b, c) =>
    {
      if (this.isSwimming.Value)
        this.position.Field.AxisAlignedMovement = false;
      else
        this.position.Field.AxisAlignedMovement = true;
    });
    this.name.FilterStringEvent += new NetString.FilterString(Utility.FilterDirtyWords);
  }

  public FarmAnimal(string type, long id, long ownerID)
    : base((AnimatedSprite) null, new Vector2((float) (64 /*0x40*/ * Game1.random.Next(2, 9)), (float) (64 /*0x40*/ * Game1.random.Next(4, 8))), 2, type)
  {
    this.ownerID.Value = ownerID;
    this.health.Value = 3;
    this.myID.Value = id;
    if (type == "Dairy Cow")
      type = "Brown Cow";
    this.type.Value = type;
    this.Name = Dialogue.randomName();
    this.displayName = this.name.Value;
    this.happiness.Value = (int) byte.MaxValue;
    this.fullness.Value = (int) byte.MaxValue;
    this._nextFollowTargetScan = Utility.RandomFloat(1f, 3f);
    this.ReloadTextureIfNeeded(true);
    FarmAnimalData animalData = this.GetAnimalData();
    if (animalData == null)
      Game1.log.Warn($"Constructed farm animal type '{type}' which has no entry in Data/FarmAnimals.");
    this.buildingTypeILiveIn.Value = animalData?.House;
    if (animalData?.Skins == null)
      return;
    Random random = Utility.CreateRandom((double) id);
    float max = 1f;
    foreach (FarmAnimalSkin skin in animalData.Skins)
      max += skin.Weight;
    float num = Utility.RandomFloat(0.0f, max, random);
    foreach (FarmAnimalSkin skin in animalData.Skins)
    {
      num -= skin.Weight;
      if ((double) num <= 0.0)
      {
        this.skinID.Value = skin.Id;
        break;
      }
    }
  }

  /// <summary>Get the animal's <see cref="P:StardewValley.Character.Sprite" /> value, loading it if needed.</summary>
  public AnimatedSprite GetOrLoadTexture()
  {
    AnimatedSprite sprite = this.Sprite;
    if (sprite == null)
    {
      this.ReloadTextureIfNeeded();
      sprite = this.Sprite;
    }
    return sprite;
  }

  /// <summary>Reload the texture if the asset name should change based on the current animal state and data.</summary>
  /// <param name="forceReload">Whether to reload the texture even if the texture path hasn't changed.</param>
  public void ReloadTextureIfNeeded(bool forceReload = false)
  {
    if (this.Sprite == null | forceReload)
    {
      FarmAnimalData animalData = this.GetAnimalData();
      string str;
      int spriteWidth;
      int spriteHeight;
      if (animalData != null)
      {
        str = this.GetTexturePath(animalData);
        spriteWidth = animalData.SpriteWidth;
        spriteHeight = animalData.SpriteHeight;
      }
      else
      {
        str = "Animals\\Error";
        spriteWidth = 16 /*0x10*/;
        spriteHeight = 16 /*0x10*/;
      }
      if (!Game1.content.DoesAssetExist<Texture2D>(str))
      {
        Game1.log.Warn($"Farm animal '{this.type.Value}' failed to load texture path '{str}': asset doesn't exist. Defaulting to error texture.");
        str = "Animals\\Error";
        spriteWidth = 16 /*0x10*/;
        spriteHeight = 16 /*0x10*/;
      }
      this.Sprite = new AnimatedSprite(str, 0, spriteWidth, spriteHeight)
      {
        textureUsesFlippedRightForLeft = animalData != null && animalData.UseFlippedRightForLeft
      };
      this.ValidateSpritesheetSize();
    }
    else
    {
      string texturePath = this.GetTexturePath();
      if (!(this.Sprite.textureName.Value != texturePath))
        return;
      this.Sprite.LoadTexture(texturePath);
    }
  }

  public string GetTexturePath() => this.GetTexturePath(this.GetAnimalData());

  public virtual string GetTexturePath(FarmAnimalData data)
  {
    string texturePath = "Animals\\" + this.type.Value;
    if (data != null)
    {
      FarmAnimalSkin farmAnimalSkin = (FarmAnimalSkin) null;
      if (this.skinID.Value != null && data.Skins != null)
      {
        foreach (FarmAnimalSkin skin in data.Skins)
        {
          if (this.skinID.Value == skin.Id)
          {
            farmAnimalSkin = skin;
            break;
          }
        }
      }
      if (farmAnimalSkin != null && farmAnimalSkin.Texture != null)
        texturePath = farmAnimalSkin.Texture;
      else if (data.Texture != null)
        texturePath = data.Texture;
      if (this.currentProduce.Value == null)
      {
        if (farmAnimalSkin != null && farmAnimalSkin.HarvestedTexture != null)
          texturePath = farmAnimalSkin.HarvestedTexture;
        else if (data.HarvestedTexture != null)
          texturePath = data.HarvestedTexture;
      }
      if (this.isBaby())
      {
        if (farmAnimalSkin != null && farmAnimalSkin.BabyTexture != null)
          texturePath = farmAnimalSkin.BabyTexture;
        else if (data.BabyTexture != null)
          texturePath = data.BabyTexture;
      }
    }
    return texturePath;
  }

  public static FarmAnimalData GetAnimalDataFromEgg(Item eggItem, GameLocation location)
  {
    FarmAnimalData data;
    return !FarmAnimal.TryGetAnimalDataFromEgg(eggItem, location, out string _, out data) ? (FarmAnimalData) null : data;
  }

  public static bool TryGetAnimalDataFromEgg(
    Item eggItem,
    GameLocation location,
    out string id,
    out FarmAnimalData data)
  {
    if (!eggItem.HasTypeObject())
    {
      id = (string) null;
      data = (FarmAnimalData) null;
      return false;
    }
    List<string> validOccupantTypes = location?.ParentBuilding?.GetData()?.ValidOccupantTypes;
    foreach (KeyValuePair<string, FarmAnimalData> keyValuePair in (IEnumerable<KeyValuePair<string, FarmAnimalData>>) Game1.farmAnimalData)
    {
      FarmAnimalData farmAnimalData = keyValuePair.Value;
      if (farmAnimalData.EggItemIds != null && farmAnimalData.EggItemIds.Count != 0 && (validOccupantTypes == null || validOccupantTypes.Contains(farmAnimalData.House)) && farmAnimalData.EggItemIds.Contains(eggItem.ItemId))
      {
        id = keyValuePair.Key;
        data = farmAnimalData;
        return true;
      }
    }
    id = (string) null;
    data = (FarmAnimalData) null;
    return false;
  }

  public virtual FarmAnimalData GetAnimalData()
  {
    FarmAnimalData farmAnimalData;
    return !Game1.farmAnimalData.TryGetValue(this.type.Value, out farmAnimalData) ? (FarmAnimalData) null : farmAnimalData;
  }

  /// <summary>Get the translated display name for a farm animal from its data, if any.</summary>
  /// <param name="id">The animal type ID in <c>Data/FarmAnimals</c>.</param>
  /// <param name="forShop">Whether to get the shop name, if applicable.</param>
  public static string GetDisplayName(string id, bool forShop = false)
  {
    FarmAnimalData farmAnimalData;
    return !Game1.farmAnimalData.TryGetValue(id, out farmAnimalData) ? (string) null : TokenParser.ParseText(forShop ? farmAnimalData.ShopDisplayName ?? farmAnimalData.DisplayName : farmAnimalData.DisplayName);
  }

  /// <summary>Get the translated shop description for a farm animal from its data, if any.</summary>
  /// <param name="id">The animal type ID in <c>Data/FarmAnimals</c>.</param>
  public static string GetShopDescription(string id)
  {
    FarmAnimalData farmAnimalData;
    return !Game1.farmAnimalData.TryGetValue(id, out farmAnimalData) ? (string) null : TokenParser.ParseText(farmAnimalData.ShopDescription);
  }

  public string shortDisplayType()
  {
    switch (LocalizedContentManager.CurrentLanguageCode)
    {
      case LocalizedContentManager.LanguageCode.en:
        return ((IEnumerable<string>) ArgUtility.SplitBySpace(this.displayType)).Last<string>();
      case LocalizedContentManager.LanguageCode.ja:
        if (this.displayType.Contains("トリ"))
          return "トリ";
        if (this.displayType.Contains("ウシ"))
          return "ウシ";
        return !this.displayType.Contains("ブタ") ? this.displayType : "ブタ";
      case LocalizedContentManager.LanguageCode.ru:
        if (this.displayType.ContainsIgnoreCase("курица"))
          return "Курица";
        return !this.displayType.ContainsIgnoreCase("корова") ? this.displayType : "Корова";
      case LocalizedContentManager.LanguageCode.zh:
        if (this.displayType.Contains('鸡'))
          return "鸡";
        if (this.displayType.Contains('牛'))
          return "牛";
        return !this.displayType.Contains('猪') ? this.displayType : "猪";
      case LocalizedContentManager.LanguageCode.pt:
      case LocalizedContentManager.LanguageCode.es:
        return ArgUtility.SplitBySpaceAndGet(this.displayType, 0);
      case LocalizedContentManager.LanguageCode.de:
        return ((IEnumerable<string>) ((IEnumerable<string>) ArgUtility.SplitBySpace(this.displayType)).Last<string>().Split('-')).Last<string>();
      default:
        return this.displayType;
    }
  }

  public Microsoft.Xna.Framework.Rectangle GetHarvestBoundingBox()
  {
    Vector2 position = this.Position;
    return new Microsoft.Xna.Framework.Rectangle((int) ((double) position.X + (double) (this.Sprite.getWidth() * 4 / 2) - 32.0 + 4.0), (int) ((double) position.Y + (double) (this.Sprite.getHeight() * 4) - 64.0 - 24.0), 56, 72);
  }

  public Microsoft.Xna.Framework.Rectangle GetCursorPetBoundingBox()
  {
    Vector2 position = this.Position;
    FarmAnimalData animalData = this.GetAnimalData();
    if (animalData == null)
      return new Microsoft.Xna.Framework.Rectangle((int) ((double) position.X + (double) (this.Sprite.getWidth() * 4 / 2) - 32.0 + 4.0), (int) ((double) position.Y + (double) (this.Sprite.getHeight() * 4) - 64.0 - 24.0), 56, 72);
    int width;
    int height;
    if (this.isBaby())
    {
      if (this.FacingDirection == 0 || this.FacingDirection == 2 || this.Sprite.currentFrame >= 12)
      {
        width = (int) ((double) animalData.BabyUpDownPetHitboxTileSize.X * 64.0);
        height = (int) ((double) animalData.BabyUpDownPetHitboxTileSize.Y * 64.0);
      }
      else
      {
        width = (int) ((double) animalData.BabyLeftRightPetHitboxTileSize.X * 64.0);
        height = (int) ((double) animalData.BabyLeftRightPetHitboxTileSize.Y * 64.0);
      }
    }
    else if (this.FacingDirection == 0 || this.FacingDirection == 2 || this.Sprite.currentFrame >= 12)
    {
      width = (int) ((double) animalData.UpDownPetHitboxTileSize.X * 64.0);
      height = (int) ((double) animalData.UpDownPetHitboxTileSize.Y * 64.0);
    }
    else
    {
      width = (int) ((double) animalData.LeftRightPetHitboxTileSize.X * 64.0);
      height = (int) ((double) animalData.LeftRightPetHitboxTileSize.Y * 64.0);
    }
    return new Microsoft.Xna.Framework.Rectangle((int) ((double) this.Position.X + (double) (this.Sprite.getWidth() * 4 / 2) - (double) (width / 2)), (int) ((double) this.Position.Y - 24.0 + (double) (this.Sprite.getHeight() * 4) - (double) height), width, height);
  }

  public override Microsoft.Xna.Framework.Rectangle GetBoundingBox()
  {
    Vector2 position = this.Position;
    return new Microsoft.Xna.Framework.Rectangle((int) ((double) position.X + (double) (this.Sprite.getWidth() * 4 / 2) - 32.0 + 8.0), (int) ((double) position.Y + (double) (this.Sprite.getHeight() * 4) - 64.0 + 8.0), 48 /*0x30*/, 48 /*0x30*/);
  }

  public void reload(GameLocation homeInterior)
  {
    this.homeInterior = homeInterior;
    this.ReloadTextureIfNeeded();
  }

  public void reload(Building home) => this.reload(home?.GetIndoors());

  public int GetDaysOwned()
  {
    if (this.daysOwned.Value < 0)
      this.daysOwned.Value = this.age.Value;
    return this.daysOwned.Value;
  }

  public void pet(Farmer who, bool is_auto_pet = false)
  {
    if (!is_auto_pet)
    {
      if (who.FarmerSprite.PauseForSingleAnimation)
        return;
      who.Halt();
      who.faceGeneralDirection(this.Position, 0, false, false);
      if (Game1.timeOfDay >= 1900 && !this.isMoving())
      {
        Game1.drawObjectDialogue(Game1.content.LoadString("Strings\\FarmAnimals:TryingToSleep", (object) this.displayName));
        return;
      }
      this.Halt();
      this.Sprite.StopAnimation();
      this.uniqueFrameAccumulator = -1;
      switch (Game1.player.FacingDirection)
      {
        case 0:
          this.Sprite.currentFrame = 0;
          break;
        case 1:
          this.Sprite.currentFrame = 12;
          break;
        case 2:
          this.Sprite.currentFrame = 8;
          break;
        case 3:
          this.Sprite.currentFrame = 4;
          break;
      }
      if (!this.hasEatenAnimalCracker.Value && who.ActiveObject?.QualifiedItemId == "(O)GoldenAnimalCracker")
      {
        bool? eatGoldenCrackers = this.GetAnimalData()?.CanEatGoldenCrackers;
        if (eatGoldenCrackers.HasValue && !eatGoldenCrackers.GetValueOrDefault())
        {
          Game1.playSound("cancel");
          this.doEmote(8);
          return;
        }
        this.hasEatenAnimalCracker.Value = true;
        Game1.playSound("give_gift");
        this.doEmote(56);
        Game1.player.reduceActiveItemByOne();
        return;
      }
    }
    else if (this.wasAutoPet.Value)
      return;
    if (!this.wasPet.Value)
    {
      if (!is_auto_pet)
        this.wasPet.Value = true;
      int num1 = 7;
      if (this.wasAutoPet.Value)
        this.friendshipTowardFarmer.Value = Math.Min(1000, this.friendshipTowardFarmer.Value + num1);
      else if (is_auto_pet)
        this.friendshipTowardFarmer.Value = Math.Min(1000, this.friendshipTowardFarmer.Value + (15 - num1));
      else
        this.friendshipTowardFarmer.Value = Math.Min(1000, this.friendshipTowardFarmer.Value + 15);
      if (is_auto_pet)
        this.wasAutoPet.Value = true;
      FarmAnimalData animalData = this.GetAnimalData();
      int happinessDrain = animalData != null ? animalData.HappinessDrain : 0;
      if (!is_auto_pet)
      {
        if (animalData != null && animalData.ProfessionForHappinessBoost >= 0 && who.professions.Contains(animalData.ProfessionForHappinessBoost))
        {
          this.friendshipTowardFarmer.Value = Math.Min(1000, this.friendshipTowardFarmer.Value + 15);
          this.happiness.Value = (int) (byte) Math.Min((int) byte.MaxValue, this.happiness.Value + Math.Max(5, 30 + happinessDrain));
        }
        int num2 = 20;
        if (this.wasAutoPet.Value)
          num2 = 32 /*0x20*/;
        this.doEmote(this.moodMessage.Value == 4 ? 12 : num2);
      }
      this.happiness.Value = (int) (byte) Math.Min((int) byte.MaxValue, this.happiness.Value + Math.Max(5, 30 + happinessDrain));
      if (is_auto_pet)
        return;
      this.makeSound();
      who.gainExperience(0, 5);
    }
    else
    {
      if (is_auto_pet || !(who.ActiveObject?.QualifiedItemId != "(O)178"))
        return;
      Game1.activeClickableMenu = (IClickableMenu) new AnimalQueryMenu(this);
    }
  }

  public void farmerPushing()
  {
    ++this.pushAccumulator;
    if (this.pushAccumulator <= 60)
      return;
    this.doFarmerPushEvent.Fire(Game1.player.FacingDirection);
    Game1.player.TemporaryPassableTiles.Add(Utility.ExpandRectangle(this.GetBoundingBox(), Utility.GetOppositeFacingDirection(Game1.player.FacingDirection), 6));
    this.pushAccumulator = 0;
  }

  public virtual void doDive()
  {
    this.yJumpVelocity = 8f;
    this.yJumpOffset = 1;
  }

  public void doFarmerPush(int direction)
  {
    if (!Game1.IsMasterGame)
      return;
    switch (direction)
    {
      case 0:
        this.Halt();
        break;
      case 1:
        this.Halt();
        break;
      case 2:
        this.Halt();
        break;
      case 3:
        this.Halt();
        break;
    }
  }

  public void Poke() => this.doBuildingPokeEvent.Fire();

  public void doBuildingPoke()
  {
    if (!Game1.IsMasterGame)
      return;
    this.FacingDirection = Game1.random.Next(4);
    this.setMovingInFacingDirection();
  }

  public void setRandomPosition(GameLocation location)
  {
    this.StopAllActions();
    Microsoft.Xna.Framework.Rectangle parsed;
    if (!location.TryGetMapPropertyAs("ProduceArea", out parsed, true))
      return;
    this.Position = new Vector2((float) (Game1.random.Next(parsed.X, parsed.Right) * 64 /*0x40*/), (float) (Game1.random.Next(parsed.Y, parsed.Bottom) * 64 /*0x40*/));
    int num = 0;
    while (this.Position.Equals(Vector2.Zero) || location.Objects.ContainsKey(this.Position) || location.isCollidingPosition(this.GetBoundingBox(), Game1.viewport, false, 0, false, (Character) this))
    {
      this.Position = new Vector2((float) Game1.random.Next(parsed.X, parsed.Right), (float) Game1.random.Next(parsed.Y, parsed.Bottom)) * 64f;
      ++num;
      if (num > 64 /*0x40*/)
        break;
    }
    this.SleepIfNecessary();
  }

  public virtual void StopAllActions()
  {
    this.foundGrass = (Grass) null;
    this.controller = (PathFindController) null;
    this.isSwimming.Value = false;
    this.hopOffset = Vector2.Zero;
    this._followTarget = (FarmAnimal) null;
    this._followTargetPosition = new Point?();
    this.Halt();
    this.Sprite.StopAnimation();
    this.Sprite.UpdateSourceRect();
  }

  public virtual void HandleStatsOnProduceCollected(Item item, uint amount = 1)
  {
    this.HandleStats(this.GetAnimalData()?.StatToIncrementOnProduce, item, amount);
  }

  public virtual void HandleStats(List<StatIncrement> stats, Item item, uint amount = 1)
  {
    if (stats == null)
      return;
    foreach (StatIncrement stat in stats)
    {
      if (stat.RequiredItemId == null || ItemRegistry.HasItemId(item, stat.RequiredItemId))
      {
        List<string> requiredTags = stat.RequiredTags;
        // ISSUE: explicit non-virtual call
        if ((requiredTags != null ? (__nonvirtual (requiredTags.Count) > 0 ? 1 : 0) : 0) == 0 || ItemContextTagManager.DoAllTagsMatch((IList<string>) stat.RequiredTags, item.GetContextTags()))
        {
          int num = (int) Game1.stats.Increment(stat.StatName, amount);
        }
      }
    }
  }

  public string GetProduceID(Random r, bool deluxe = false)
  {
    FarmAnimalData animalData = this.GetAnimalData();
    if (animalData == null)
      return (string) null;
    List<FarmAnimalProduce> options = new List<FarmAnimalProduce>();
    if (deluxe)
    {
      if (animalData.DeluxeProduceItemIds != null)
        options.AddRange((IEnumerable<FarmAnimalProduce>) animalData.DeluxeProduceItemIds);
    }
    else if (animalData.ProduceItemIds != null)
      options.AddRange((IEnumerable<FarmAnimalProduce>) animalData.ProduceItemIds);
    options.RemoveAll((Predicate<FarmAnimalProduce>) (produce =>
    {
      if (produce.MinimumFriendship > 0 && this.friendshipTowardFarmer.Value < produce.MinimumFriendship)
        return true;
      return produce.Condition != null && !GameStateQuery.CheckConditions(produce.Condition, this.currentLocation, random: r);
    }));
    return r.ChooseFrom<FarmAnimalProduce>((IList<FarmAnimalProduce>) options)?.ItemId;
  }

  /// <summary>Update the animal state when setting up the new day, before the game saves overnight.</summary>
  /// <param name="environment">The location containing the animal.</param>
  /// <remarks>See also <see cref="M:StardewValley.FarmAnimal.OnDayStarted" />, which happens after saving when the day has started.</remarks>
  public void dayUpdate(GameLocation environment)
  {
    if (this.daysOwned.Value < 0)
      this.daysOwned.Value = this.age.Value;
    FarmAnimalData animalData1 = this.GetAnimalData();
    FarmAnimalData animalData2 = this.GetAnimalData();
    int happinessDrain = animalData2 != null ? animalData2.HappinessDrain : 0;
    int num1 = animalData1 == null || animalData1.FriendshipForFasterProduce < 0 || this.friendshipTowardFarmer.Value < animalData1.FriendshipForFasterProduce ? 0 : 1;
    this.StopAllActions();
    this.health.Value = 3;
    bool flag1 = false;
    GameLocation homeInterior = this.homeInterior;
    if (homeInterior != null && !this.IsHome)
    {
      if (!this.home.animalDoorOpen.Value)
      {
        this.moodMessage.Value = 6;
        flag1 = true;
        this.happiness.Value /= 2;
      }
      else
      {
        environment.animals.Remove(this.myID.Value);
        homeInterior.animals.TryAdd(this.myID.Value, this);
        if (Game1.timeOfDay > 1800 && this.controller == null)
          this.happiness.Value /= 2;
        this.setRandomPosition(homeInterior);
        return;
      }
    }
    else if (homeInterior != null && this.IsHome && !this.home.animalDoorOpen.Value)
      this.happiness.Value = (int) (byte) Math.Min((int) byte.MaxValue, this.happiness.Value + happinessDrain * 2);
    ++this.daysSinceLastLay.Value;
    if (!this.wasPet.Value && !this.wasAutoPet.Value)
    {
      this.friendshipTowardFarmer.Value = Math.Max(0, this.friendshipTowardFarmer.Value - (10 - this.friendshipTowardFarmer.Value / 200));
      this.happiness.Value = (int) (byte) Math.Max(0, this.happiness.Value - 50);
    }
    this.wasPet.Value = false;
    this.wasAutoPet.Value = false;
    ++this.daysOwned.Value;
    if (this.fullness.Value < 200 && environment is AnimalHouse)
    {
      foreach (KeyValuePair<Vector2, Object> keyValuePair in environment.objects.Pairs.ToArray<KeyValuePair<Vector2, Object>>())
      {
        if (keyValuePair.Value.QualifiedItemId == "(O)178")
        {
          environment.objects.Remove(keyValuePair.Key);
          this.fullness.Value = (int) byte.MaxValue;
          break;
        }
      }
    }
    Random random = Utility.CreateRandom((double) this.myID.Value / 2.0, (double) Game1.stats.DaysPlayed);
    int? nullable;
    if (this.fullness.Value > 200 || random.NextDouble() < (double) (this.fullness.Value - 30) / 170.0)
    {
      int num2 = this.age.Value;
      nullable = animalData1 != null ? new int?(animalData1.DaysToMature - 1) : new int?();
      int valueOrDefault = nullable.GetValueOrDefault();
      if (num2 == valueOrDefault & nullable.HasValue)
        this.growFully(random);
      else
        ++this.age.Value;
      this.happiness.Value = (int) (byte) Math.Min((int) byte.MaxValue, this.happiness.Value + happinessDrain * 2);
    }
    if (this.fullness.Value < 200)
    {
      this.happiness.Value = (int) (byte) Math.Max(0, this.happiness.Value - 100);
      this.friendshipTowardFarmer.Value = Math.Max(0, this.friendshipTowardFarmer.Value - 20);
    }
    Farmer farmer = Game1.GetPlayer(this.ownerID.Value) ?? Game1.MasterPlayer;
    if (animalData1 != null && animalData1.ProfessionForFasterProduce >= 0 && farmer.professions.Contains(animalData1.ProfessionForFasterProduce))
      ++num1;
    int num3 = this.daysSinceLastLay.Value;
    nullable = animalData1 != null ? new int?(animalData1.DaysToProduce - num1) : new int?();
    int valueOrDefault1 = nullable.GetValueOrDefault();
    bool flag2 = num3 >= valueOrDefault1 & nullable.HasValue && random.NextDouble() < (double) this.fullness.Value / 200.0 && random.NextDouble() < (double) this.happiness.Value / 70.0;
    string str;
    if (!flag2 || this.isBaby())
    {
      str = (string) null;
    }
    else
    {
      str = this.GetProduceID(random);
      if (random.NextDouble() < (double) this.happiness.Value / 150.0)
      {
        float num4 = this.happiness.Value > 200 ? (float) this.happiness.Value * 1.5f : (this.happiness.Value <= 100 ? (float) (this.happiness.Value - 100) : 0.0f);
        string produceId = this.GetProduceID(random, true);
        if (animalData1 != null && (double) animalData1.DeluxeProduceCareDivisor >= 0.0 && produceId != null && this.friendshipTowardFarmer.Value >= animalData1.DeluxeProduceMinimumFriendship && random.NextDouble() < ((double) this.friendshipTowardFarmer.Value + (double) num4) / (double) animalData1.DeluxeProduceCareDivisor + Game1.player.team.AverageDailyLuck() * (double) animalData1.DeluxeProduceLuckMultiplier)
          str = produceId;
        this.daysSinceLastLay.Value = 0;
        double num5 = (double) this.friendshipTowardFarmer.Value / 1000.0 - (1.0 - (double) this.happiness.Value / 225.0);
        if (animalData1 != null && animalData1.ProfessionForQualityBoost >= 0 && farmer.professions.Contains(animalData1.ProfessionForQualityBoost))
          num5 += 0.33;
        if (num5 >= 0.95 && random.NextDouble() < num5 / 2.0)
          this.produceQuality.Value = 4;
        else if (random.NextDouble() < num5 / 2.0)
          this.produceQuality.Value = 2;
        else if (random.NextDouble() < num5)
          this.produceQuality.Value = 1;
        else
          this.produceQuality.Value = 0;
      }
    }
    if (((animalData1 != null ? (animalData1.HarvestType != 0 ? 1 : 0) : 1) & (flag2 ? 1 : 0)) != 0)
    {
      this.currentProduce.Value = str;
      str = (string) null;
    }
    if (str != null && this.home != null)
    {
      bool flag3 = true;
      Object o = ItemRegistry.Create<Object>("(O)" + str);
      o.CanBeSetDown = false;
      o.Quality = this.produceQuality.Value;
      if (this.hasEatenAnimalCracker.Value)
        o.Stack = 2;
      this.HandleStats(animalData1?.StatToIncrementOnProduce, (Item) o, (uint) o.Stack);
      foreach (Object @object in homeInterior.objects.Values)
      {
        if (@object.QualifiedItemId == "(BC)165" && @object.heldObject.Value is Chest chest && chest.addItem((Item) o) == null)
        {
          @object.showNextIndex.Value = true;
          flag3 = false;
          break;
        }
      }
      if (flag3)
      {
        o.Stack = 1;
        Utility.spawnObjectAround(this.Tile, o, environment);
        if (this.hasEatenAnimalCracker.Value)
          Utility.spawnObjectAround(this.Tile, (Object) o.getOne(), environment);
      }
    }
    if (!flag1)
    {
      if (this.fullness.Value < 30)
        this.moodMessage.Value = 4;
      else if (this.happiness.Value < 30)
        this.moodMessage.Value = 3;
      else if (this.happiness.Value < 200)
        this.moodMessage.Value = 2;
      else
        this.moodMessage.Value = 1;
    }
    this.fullness.Value = 0;
    if (Utility.isFestivalDay())
      this.fullness.Value = 250;
    this.reload(this.homeInterior);
  }

  /// <summary>Handle the new day starting after the player saves, loads, or connects.</summary>
  /// <remarks>See also <see cref="M:StardewValley.FarmAnimal.dayUpdate(StardewValley.GameLocation)" />, which happens while setting up the day before saving.</remarks>
  public void OnDayStarted()
  {
    FarmAnimalData animalData = this.GetAnimalData();
    if ((animalData != null ? (animalData.GrassEatAmount < 1 ? 1 : 0) : 0) == 0)
      return;
    this.fullness.Value = (int) byte.MaxValue;
  }

  public int getSellPrice()
  {
    FarmAnimalData animalData = this.GetAnimalData();
    return (int) ((animalData != null ? (double) animalData.SellPrice : 0.0) * ((double) this.friendshipTowardFarmer.Value / 1000.0 + 0.3));
  }

  public bool isMale()
  {
    FarmAnimalGender? gender = this.GetAnimalData()?.Gender;
    if (gender.HasValue)
    {
      switch (gender.GetValueOrDefault())
      {
        case FarmAnimalGender.Female:
          return false;
        case FarmAnimalGender.Male:
          return true;
      }
    }
    return this.myID.Value % 2L == 0L;
  }

  public string getMoodMessage()
  {
    string str = this.isMale() ? "Male" : "Female";
    switch (this.moodMessage.Value)
    {
      case 0:
        return this.parentId.Value != -1L ? Game1.content.LoadString("Strings\\FarmAnimals:MoodMessage_NewHome_Baby_" + str, (object) this.displayName) : Game1.content.LoadString($"Strings\\FarmAnimals:MoodMessage_NewHome_Adult_{str}_{(Game1.dayOfMonth % 2 + 1).ToString()}", (object) this.displayName);
      case 4:
        return Game1.content.LoadString("Strings\\FarmAnimals:MoodMessage_" + (((long) Game1.dayOfMonth + this.myID.Value) % 2L == 0L ? "Hungry1" : "Hungry2"), (object) this.displayName);
      case 5:
        return Game1.content.LoadString("Strings\\FarmAnimals:MoodMessage_DisturbedByDog_" + str, (object) this.displayName);
      case 6:
        return Game1.content.LoadString("Strings\\FarmAnimals:MoodMessage_LeftOutsideAtNight_" + str, (object) this.displayName);
      default:
        if (this.happiness.Value < 30)
          this.moodMessage.Value = 3;
        else if (this.happiness.Value < 200)
          this.moodMessage.Value = 2;
        else
          this.moodMessage.Value = 1;
        switch (this.moodMessage.Value)
        {
          case 1:
            return Game1.content.LoadString("Strings\\FarmAnimals:MoodMessage_Happy", (object) this.displayName);
          case 2:
            return Game1.content.LoadString("Strings\\FarmAnimals:MoodMessage_Fine", (object) this.displayName);
          case 3:
            return Game1.content.LoadString("Strings\\FarmAnimals:MoodMessage_Sad", (object) this.displayName);
          default:
            return "";
        }
    }
  }

  /// <summary>Get whether this farm animal is fully grown.</summary>
  /// <remarks>See also <see cref="M:StardewValley.FarmAnimal.isBaby" />.</remarks>
  public bool isAdult()
  {
    int? daysToMature = this.GetAnimalData()?.DaysToMature;
    if (!daysToMature.HasValue)
      return true;
    int num = this.age.Value;
    int? nullable = daysToMature;
    int valueOrDefault = nullable.GetValueOrDefault();
    return num >= valueOrDefault & nullable.HasValue;
  }

  /// <summary>Get whether this farm animal is a baby.</summary>
  /// <remarks>See also <see cref="M:StardewValley.FarmAnimal.isAdult" />.</remarks>
  public bool isBaby()
  {
    int num = this.age.Value;
    int? daysToMature = this.GetAnimalData()?.DaysToMature;
    int valueOrDefault = daysToMature.GetValueOrDefault();
    return num < valueOrDefault & daysToMature.HasValue;
  }

  /// <summary>Get whether this farm animal's produce can be collected using a given tool.</summary>
  /// <param name="tool">The tool to check.</param>
  public bool CanGetProduceWithTool(Tool tool)
  {
    return tool != null && tool.Name != null && this.GetAnimalData().HarvestTool == tool.Name;
  }

  /// <summary>Get the way in which the animal's produce is output.</summary>
  public FarmAnimalHarvestType? GetHarvestType() => this.GetAnimalData()?.HarvestType;

  /// <summary>Get whether this farm animal can live in a building.</summary>
  /// <param name="building">The building to check.</param>
  /// <remarks>This doesn't check whether there's room for it in the building; see <see cref="M:StardewValley.AnimalHouse.isFull" /> on <see cref="M:StardewValley.Buildings.Building.GetIndoors" /> for that.</remarks>
  public bool CanLiveIn(Building building)
  {
    BuildingData data = building?.GetData();
    return data?.ValidOccupantTypes != null && data.ValidOccupantTypes.Contains(this.buildingTypeILiveIn.Value) && !building.isUnderConstruction() && building.GetIndoors() is AnimalHouse;
  }

  public void warpHome()
  {
    GameLocation homeInterior = this.homeInterior;
    if (homeInterior == null || homeInterior == this.currentLocation)
      return;
    if (homeInterior.animals.TryAdd(this.myID.Value, this))
    {
      this.setRandomPosition(homeInterior);
      ++this.home.currentOccupants.Value;
    }
    this.currentLocation?.animals.Remove(this.myID.Value);
    this.controller = (PathFindController) null;
    this.isSwimming.Value = false;
    this.hopOffset = Vector2.Zero;
    this._followTarget = (FarmAnimal) null;
    this._followTargetPosition = new Point?();
  }

  /// <summary>If the animal is a baby, instantly age it to adult.</summary>
  /// <param name="random">The RNG with which to select its produce, if applicable.</param>
  public void growFully(Random random = null)
  {
    FarmAnimalData animalData = this.GetAnimalData();
    int num = this.age.Value;
    int? daysToMature = animalData?.DaysToMature;
    int valueOrDefault = daysToMature.GetValueOrDefault();
    if (!(num <= valueOrDefault & daysToMature.HasValue))
      return;
    this.age.Value = animalData.DaysToMature;
    if (animalData.ProduceOnMature)
      this.currentProduce.Value = this.GetProduceID(random ?? Game1.random);
    this.daysSinceLastLay.Value = 99;
    this.ReloadTextureIfNeeded();
  }

  public override void draw(SpriteBatch b)
  {
    Vector2 vector2 = new Vector2(0.0f, (float) this.yJumpOffset);
    Microsoft.Xna.Framework.Rectangle boundingBox = this.GetBoundingBox();
    FarmAnimalData animalData = this.GetAnimalData();
    bool isSwimming = this.IsActuallySwimming();
    bool isBaby = this.isBaby();
    FarmAnimalShadowData shadow = animalData?.GetShadow(isBaby, isSwimming);
    if ((shadow != null ? (shadow.Visible ? 1 : 0) : 1) != 0)
    {
      int? nullable1;
      if (shadow == null)
      {
        nullable1 = new int?();
      }
      else
      {
        ref Point? local = ref shadow.Offset;
        nullable1 = local.HasValue ? new int?(local.GetValueOrDefault().X) : new int?();
      }
      int valueOrDefault1 = nullable1.GetValueOrDefault();
      int? nullable2;
      if (shadow == null)
      {
        nullable2 = new int?();
      }
      else
      {
        ref Point? local = ref shadow.Offset;
        nullable2 = local.HasValue ? new int?(local.GetValueOrDefault().Y) : new int?();
      }
      int valueOrDefault2 = nullable2.GetValueOrDefault();
      if (isSwimming)
      {
        float scale = (float) ((double) shadow?.Scale ?? (isBaby ? 2.5 : 3.5));
        Vector2 globalPosition = new Vector2(this.Position.X + (float) valueOrDefault1, this.Position.Y - 24f + (float) valueOrDefault2);
        this.Sprite.drawShadow(b, Game1.GlobalToLocal(Game1.viewport, globalPosition), scale, 0.5f);
        int num = (int) ((Math.Sin(Game1.currentGameTime.TotalGameTime.TotalSeconds * 4.0 + (double) this.bobOffset) + 0.5) * 3.0);
        vector2.Y += (float) num;
      }
      else
      {
        float scale = (float) ((double) shadow?.Scale ?? (isBaby ? 3.0 : 4.0));
        Vector2 globalPosition = new Vector2(this.Position.X + (float) valueOrDefault1, this.Position.Y - 24f + (float) valueOrDefault2);
        this.Sprite.drawShadow(b, Game1.GlobalToLocal(Game1.viewport, globalPosition), scale);
      }
    }
    vector2.Y += (float) this.yJumpOffset;
    float layerDepth = (float) (((double) (boundingBox.Center.Y + 4) + (double) this.Position.X / 20000.0) / 10000.0);
    this.Sprite.draw(b, Utility.snapDrawPosition(Game1.GlobalToLocal(Game1.viewport, this.Position - new Vector2(0.0f, 24f) + vector2)), layerDepth, 0, 0, this.hitGlowTimer > 0 ? Color.Red : Color.White, this.FacingDirection == 3, 4f);
    if (!this.isEmoting)
      return;
    int num1 = this.Sprite.SpriteWidth / 2 * 4 - 32 /*0x20*/ + (animalData != null ? animalData.EmoteOffset.X : 0);
    int num2 = (animalData != null ? animalData.EmoteOffset.Y : 0) - 64 /*0x40*/;
    Vector2 local1 = Game1.GlobalToLocal(Game1.viewport, new Vector2(this.Position.X + vector2.X + (float) num1, this.Position.Y + vector2.Y + (float) num2));
    b.Draw(Game1.emoteSpriteSheet, local1, new Microsoft.Xna.Framework.Rectangle?(new Microsoft.Xna.Framework.Rectangle(this.CurrentEmoteIndex * 16 /*0x10*/ % Game1.emoteSpriteSheet.Width, this.CurrentEmoteIndex * 16 /*0x10*/ / Game1.emoteSpriteSheet.Width * 16 /*0x10*/, 16 /*0x10*/, 16 /*0x10*/)), Color.White, 0.0f, Vector2.Zero, 4f, SpriteEffects.None, (float) boundingBox.Bottom / 10000f);
  }

  public virtual void updateWhenNotCurrentLocation(
    Building currentBuilding,
    GameTime time,
    GameLocation environment)
  {
    this.doFarmerPushEvent.Poll();
    this.doBuildingPokeEvent.Poll();
    this.doDiveEvent.Poll();
    AnimatedSprite orLoadTexture = this.GetOrLoadTexture();
    if (!Game1.shouldTimePass())
      return;
    this.update(time, environment, this.myID.Value, false);
    if (!Game1.IsMasterGame)
      return;
    if (this.hopOffset != Vector2.Zero)
    {
      this.HandleHop();
    }
    else
    {
      if (currentBuilding != null && Game1.random.NextBool(0.002) && currentBuilding.animalDoorOpen.Value && Game1.timeOfDay < 1630 && !environment.IsRainingHere() && !environment.IsWinterHere() && !environment.farmers.Any())
      {
        GameLocation parentLocation = currentBuilding.GetParentLocation();
        Microsoft.Xna.Framework.Rectangle rectForAnimalDoor = currentBuilding.getRectForAnimalDoor();
        rectForAnimalDoor.Inflate(-2, -2);
        if (parentLocation.isCollidingPosition(rectForAnimalDoor, Game1.viewport, false, 0, false, (Character) this, false) || parentLocation.isCollidingPosition(new Microsoft.Xna.Framework.Rectangle(rectForAnimalDoor.X, rectForAnimalDoor.Y + 64 /*0x40*/, rectForAnimalDoor.Width, rectForAnimalDoor.Height), Game1.viewport, false, 0, false, (Character) this, false))
          return;
        parentLocation.animals.Remove(this.myID.Value);
        currentBuilding.GetIndoors().animals.Remove(this.myID.Value);
        parentLocation.animals.TryAdd(this.myID.Value, this);
        this.faceDirection(2);
        this.SetMovingDown(true);
        this.Position = new Vector2((float) rectForAnimalDoor.X, (float) (rectForAnimalDoor.Y - (orLoadTexture.getHeight() * 4 - this.GetBoundingBox().Height) + 32 /*0x20*/));
        if (FarmAnimal.NumPathfindingThisTick < FarmAnimal.MaxPathfindingPerTick)
        {
          ++FarmAnimal.NumPathfindingThisTick;
          this.controller = new PathFindController((Character) this, parentLocation, new PathFindController.isAtEnd(FarmAnimal.grassEndPointFunction), Game1.random.Next(4), new PathFindController.endBehavior(FarmAnimal.behaviorAfterFindingGrassPatch), 200, Point.Zero);
        }
        if (this.controller?.pathToEndPoint == null || this.controller.pathToEndPoint.Count < 3)
        {
          this.SetMovingDown(true);
          this.controller = (PathFindController) null;
        }
        else
        {
          this.faceDirection(2);
          this.Position = new Vector2((float) (this.controller.pathToEndPoint.Peek().X * 64 /*0x40*/), (float) (this.controller.pathToEndPoint.Peek().Y * 64 /*0x40*/ - (orLoadTexture.getHeight() * 4 - this.GetBoundingBox().Height) + 16 /*0x10*/));
          if (orLoadTexture.SpriteWidth * 4 > 64 /*0x40*/)
            this.position.X -= 32f;
        }
        this.noWarpTimer = 3000;
        --currentBuilding.currentOccupants.Value;
        if (Utility.isOnScreen(this.TilePoint, 192 /*0xC0*/, parentLocation))
          parentLocation.localSound("sandyStep");
        environment.isTileOccupiedByFarmer(this.Tile)?.TemporaryPassableTiles.Add(this.GetBoundingBox());
      }
      this.UpdateRandomMovements();
      this.behaviors(time, environment);
    }
  }

  public static void behaviorAfterFindingGrassPatch(Character c, GameLocation environment)
  {
    TerrainFeature terrainFeature;
    if (environment.terrainFeatures.TryGetValue(c.Tile, out terrainFeature) && terrainFeature is Grass grass)
      FarmAnimal.reservedGrass.Remove(grass);
    if (((FarmAnimal) c).fullness.Value >= (int) byte.MaxValue)
      return;
    ((FarmAnimal) c).eatGrass(environment);
  }

  public static bool grassEndPointFunction(
    PathNode currentPoint,
    Point endPoint,
    GameLocation location,
    Character c)
  {
    Vector2 key = new Vector2((float) currentPoint.x, (float) currentPoint.y);
    TerrainFeature terrainFeature;
    if (!location.terrainFeatures.TryGetValue(key, out terrainFeature) || !(terrainFeature is Grass grass) || ((IEnumerable<TerrainFeature>) FarmAnimal.reservedGrass).Contains<TerrainFeature>(terrainFeature))
      return false;
    FarmAnimal.reservedGrass.Add(grass);
    if (c is FarmAnimal farmAnimal)
      farmAnimal.foundGrass = grass;
    return true;
  }

  public virtual void updatePerTenMinutes(int timeOfDay, GameLocation environment)
  {
    if (timeOfDay >= 1800)
    {
      FarmAnimalData animalData = this.GetAnimalData();
      int happinessDrain = animalData != null ? animalData.HappinessDrain : 0;
      int num = 0;
      if (environment.IsOutdoors)
        num = timeOfDay > 1900 || environment.IsRainingHere() || environment.IsWinterHere() ? -happinessDrain : happinessDrain;
      else if (this.happiness.Value > 150 && environment.IsWinterHere())
        num = environment.numberOfObjectsWithName("Heater") > 0 ? happinessDrain : -happinessDrain;
      if (num != 0)
        this.happiness.Value = (int) (byte) MathHelper.Clamp(this.happiness.Value + num, 0, (int) byte.MaxValue);
    }
    environment.isTileOccupiedByFarmer(this.Tile)?.TemporaryPassableTiles.Add(this.GetBoundingBox());
  }

  public void eatGrass(GameLocation environment)
  {
    TerrainFeature terrainFeature;
    if (!environment.terrainFeatures.TryGetValue(this.Tile, out terrainFeature) || !(terrainFeature is Grass grass))
      return;
    FarmAnimal.reservedGrass.Remove(grass);
    if (this.foundGrass != null)
      FarmAnimal.reservedGrass.Remove(this.foundGrass);
    this.foundGrass = (Grass) null;
    this.Eat(environment);
  }

  public virtual void Eat(GameLocation location)
  {
    Vector2 tile = this.Tile;
    this.isEating.Value = true;
    int num = 1;
    TerrainFeature terrainFeature;
    if (location.terrainFeatures.TryGetValue(tile, out terrainFeature) && terrainFeature is Grass grass)
    {
      num = (int) grass.grassType.Value;
      FarmAnimalData animalData = this.GetAnimalData();
      int number = animalData != null ? animalData.GrassEatAmount : 2;
      if (grass.reduceBy(number, location.Equals(Game1.currentLocation)))
        location.terrainFeatures.Remove(tile);
    }
    this.Sprite.loop = false;
    this.fullness.Value = (int) byte.MaxValue;
    if (this.moodMessage.Value == 5 || this.moodMessage.Value == 6 || location.IsRainingHere())
      return;
    this.happiness.Value = (int) byte.MaxValue;
    this.friendshipTowardFarmer.Value = Math.Min(1000, this.friendshipTowardFarmer.Value + (num == 7 ? 16 /*0x10*/ : 8));
  }

  public virtual bool behaviors(GameTime time, GameLocation location)
  {
    if (!Game1.IsMasterGame)
      return false;
    Building home = this.home;
    if (home == null)
      return false;
    if (this.isBaby() && this.CanFollowAdult())
    {
      this._nextFollowTargetScan -= (float) time.ElapsedGameTime.TotalSeconds;
      if ((double) this._nextFollowTargetScan < 0.0)
      {
        this._nextFollowTargetScan = Utility.RandomFloat(1f, 3f);
        if (this.controller != null || !location.IsOutdoors)
        {
          this._followTarget = (FarmAnimal) null;
          this._followTargetPosition = new Point?();
        }
        else if (this._followTarget == null)
        {
          if (location.IsOutdoors)
          {
            foreach (FarmAnimal animal in location.animals.Values)
            {
              if (!animal.isBaby() && animal.type.Value == this.type.Value && FarmAnimal.GetFollowRange(animal, 4).Contains(this.StandingPixel))
              {
                this._followTarget = animal;
                this.GetNewFollowPosition();
                return false;
              }
            }
          }
        }
        else
        {
          if (!FarmAnimal.GetFollowRange(this._followTarget).Contains(this._followTargetPosition.Value))
            this.GetNewFollowPosition();
          return false;
        }
      }
    }
    if (this.isEating.Value)
    {
      if (home != null && home.getRectForAnimalDoor().Intersects(this.GetBoundingBox()))
      {
        FarmAnimal.behaviorAfterFindingGrassPatch((Character) this, location);
        this.isEating.Value = false;
        this.Halt();
        return false;
      }
      FarmAnimalData animalData = this.GetAnimalData();
      int startFrame = 16 /*0x10*/;
      if (!this.Sprite.textureUsesFlippedRightForLeft)
        startFrame += 4;
      bool? uniqueAnimationFrames = animalData?.UseDoubleUniqueAnimationFrames;
      if (uniqueAnimationFrames.HasValue && uniqueAnimationFrames.GetValueOrDefault())
        startFrame += 4;
      if (this.Sprite.Animate(time, startFrame, 4, 100f))
      {
        this.isEating.Value = false;
        this.Sprite.loop = true;
        this.Sprite.currentFrame = 0;
        this.faceDirection(2);
      }
      return true;
    }
    if (this.controller != null)
      return true;
    if (!this.isSwimming.Value && location.IsOutdoors && this.fullness.Value < 195 && Game1.random.NextDouble() < 0.002 && FarmAnimal.NumPathfindingThisTick < FarmAnimal.MaxPathfindingPerTick)
    {
      ++FarmAnimal.NumPathfindingThisTick;
      this.controller = new PathFindController((Character) this, location, new PathFindController.isAtEnd(FarmAnimal.grassEndPointFunction), -1, new PathFindController.endBehavior(FarmAnimal.behaviorAfterFindingGrassPatch), 200, Point.Zero);
      this._followTarget = (FarmAnimal) null;
      this._followTargetPosition = new Point?();
    }
    if (Game1.timeOfDay >= 1700 && location.IsOutdoors && this.controller == null && Game1.random.NextDouble() < 0.002 && home.animalDoorOpen.Value)
    {
      if (!location.farmers.Any())
      {
        GameLocation indoors = home.GetIndoors();
        location.animals.Remove(this.myID.Value);
        indoors.animals.TryAdd(this.myID.Value, this);
        this.setRandomPosition(indoors);
        this.faceDirection(Game1.random.Next(4));
        this.controller = (PathFindController) null;
        return true;
      }
      if (FarmAnimal.NumPathfindingThisTick < FarmAnimal.MaxPathfindingPerTick)
      {
        ++FarmAnimal.NumPathfindingThisTick;
        this.controller = new PathFindController((Character) this, location, new PathFindController.isAtEnd(PathFindController.isAtEndPoint), 0, (PathFindController.endBehavior) null, 200, new Point(home.tileX.Value + home.animalDoor.X, home.tileY.Value + home.animalDoor.Y));
        this._followTarget = (FarmAnimal) null;
        this._followTargetPosition = new Point?();
      }
    }
    if (location.IsOutdoors && !location.IsRainingHere() && !location.IsWinterHere() && this.currentProduce.Value != null && this.isAdult() && this.GetHarvestType().GetValueOrDefault() == FarmAnimalHarvestType.DigUp && Game1.random.NextDouble() < 0.0002)
    {
      Object produce = ItemRegistry.Create<Object>(this.currentProduce.Value);
      Microsoft.Xna.Framework.Rectangle boundingBox = this.GetBoundingBox();
      for (int corner = 0; corner < 4; ++corner)
      {
        Vector2 cornersOfThisRectangle = Utility.getCornersOfThisRectangle(ref boundingBox, corner);
        Vector2 key = new Vector2((float) (int) ((double) cornersOfThisRectangle.X / 64.0), (float) (int) ((double) cornersOfThisRectangle.Y / 64.0));
        if (location.terrainFeatures.ContainsKey(key) || location.objects.ContainsKey(key))
          return false;
      }
      if (Game1.player.currentLocation.Equals(location))
      {
        DelayedAction.playSoundAfterDelay("dirtyHit", 450);
        DelayedAction.playSoundAfterDelay("dirtyHit", 900);
        DelayedAction.playSoundAfterDelay("dirtyHit", 1350);
      }
      if (location.Equals(Game1.currentLocation))
      {
        switch (this.FacingDirection)
        {
          case 0:
            this.Sprite.setCurrentAnimation(new List<FarmerSprite.AnimationFrame>()
            {
              new FarmerSprite.AnimationFrame(9, 250),
              new FarmerSprite.AnimationFrame(11, 250),
              new FarmerSprite.AnimationFrame(9, 250),
              new FarmerSprite.AnimationFrame(11, 250),
              new FarmerSprite.AnimationFrame(9, 250),
              new FarmerSprite.AnimationFrame(11, 250, false, false, (AnimatedSprite.endOfAnimationBehavior) (_ => this.DigUpProduce(location, produce)))
            });
            break;
          case 1:
            this.Sprite.setCurrentAnimation(new List<FarmerSprite.AnimationFrame>()
            {
              new FarmerSprite.AnimationFrame(5, 250),
              new FarmerSprite.AnimationFrame(7, 250),
              new FarmerSprite.AnimationFrame(5, 250),
              new FarmerSprite.AnimationFrame(7, 250),
              new FarmerSprite.AnimationFrame(5, 250),
              new FarmerSprite.AnimationFrame(7, 250, false, false, (AnimatedSprite.endOfAnimationBehavior) (_ => this.DigUpProduce(location, produce)))
            });
            break;
          case 2:
            this.Sprite.setCurrentAnimation(new List<FarmerSprite.AnimationFrame>()
            {
              new FarmerSprite.AnimationFrame(1, 250),
              new FarmerSprite.AnimationFrame(3, 250),
              new FarmerSprite.AnimationFrame(1, 250),
              new FarmerSprite.AnimationFrame(3, 250),
              new FarmerSprite.AnimationFrame(1, 250),
              new FarmerSprite.AnimationFrame(3, 250, false, false, (AnimatedSprite.endOfAnimationBehavior) (_ => this.DigUpProduce(location, produce)))
            });
            break;
          case 3:
            this.Sprite.setCurrentAnimation(new List<FarmerSprite.AnimationFrame>()
            {
              new FarmerSprite.AnimationFrame(5, 250, false, true),
              new FarmerSprite.AnimationFrame(7, 250, false, true),
              new FarmerSprite.AnimationFrame(5, 250, false, true),
              new FarmerSprite.AnimationFrame(7, 250, false, true),
              new FarmerSprite.AnimationFrame(5, 250, false, true),
              new FarmerSprite.AnimationFrame(7, 250, false, true, (AnimatedSprite.endOfAnimationBehavior) (_ => this.DigUpProduce(location, produce)))
            });
            break;
        }
        this.Sprite.loop = false;
      }
      else
        this.DigUpProduce(location, produce);
    }
    return false;
  }

  public virtual void DigUpProduce(GameLocation location, Object produce)
  {
    Random random = Utility.CreateRandom((double) this.myID.Value / 2.0, (double) Game1.stats.DaysPlayed, (double) Game1.timeOfDay);
    bool flag = false;
    if (produce.QualifiedItemId == "(O)430" && random.NextDouble() < 0.002)
    {
      RockCrab rockCrab = new RockCrab(this.Tile, "Truffle Crab");
      Vector2 tileForCharacter = Utility.recursiveFindOpenTileForCharacter((Character) rockCrab, location, this.Tile, 50, false);
      if (tileForCharacter != Vector2.Zero)
      {
        rockCrab.setTileLocation(tileForCharacter);
        location.addCharacter((NPC) rockCrab);
        flag = true;
      }
    }
    if (!flag && Utility.spawnObjectAround(Utility.getTranslatedVector2(this.Tile, this.FacingDirection, 1f), produce, this.currentLocation) && produce.QualifiedItemId == "(O)430")
      ++Game1.stats.TrufflesFound;
    if (random.NextBool((double) this.friendshipTowardFarmer.Value / 1500.0))
      return;
    this.currentProduce.Value = (string) null;
  }

  public static Microsoft.Xna.Framework.Rectangle GetFollowRange(FarmAnimal animal, int distance = 2)
  {
    Point standingPixel = animal.StandingPixel;
    return new Microsoft.Xna.Framework.Rectangle(standingPixel.X - distance * 64 /*0x40*/, standingPixel.Y - distance * 64 /*0x40*/, distance * 64 /*0x40*/ * 2, 64 /*0x40*/ * distance * 2);
  }

  public virtual void GetNewFollowPosition()
  {
    if (this._followTarget == null)
      this._followTargetPosition = new Point?();
    else if (this._followTarget.isMoving() && this._followTarget.IsActuallySwimming())
      this._followTargetPosition = new Point?(Utility.Vector2ToPoint(Utility.getRandomPositionInThisRectangle(FarmAnimal.GetFollowRange(this._followTarget, 1), Game1.random)));
    else
      this._followTargetPosition = new Point?(Utility.Vector2ToPoint(Utility.getRandomPositionInThisRectangle(FarmAnimal.GetFollowRange(this._followTarget), Game1.random)));
  }

  public void hitWithWeapon(MeleeWeapon t)
  {
  }

  public void makeSound()
  {
    if (this.currentLocation != Game1.currentLocation || Game1.options.muteAnimalSounds)
      return;
    string soundId = this.GetSoundId();
    if (soundId == null)
      return;
    Game1.playSound(soundId, new int?(1200 + Game1.random.Next(-200, 201)));
  }

  /// <summary>Get the sound ID produced by the animal (e.g. when pet).</summary>
  public string GetSoundId()
  {
    FarmAnimalData animalData = this.GetAnimalData();
    if (this.isBaby() && animalData != null && animalData.BabySound != null)
      return animalData.BabySound;
    return animalData?.Sound;
  }

  public virtual bool CanHavePregnancy()
  {
    FarmAnimalData animalData = this.GetAnimalData();
    return animalData != null && animalData.CanGetPregnant;
  }

  public virtual bool SleepIfNecessary()
  {
    if (Game1.timeOfDay < 2000)
      return false;
    this.isSwimming.Value = false;
    this.hopOffset = Vector2.Zero;
    this._followTarget = (FarmAnimal) null;
    this._followTargetPosition = new Point?();
    if (this.isMoving())
      this.Halt();
    FarmAnimalData animalData = this.GetAnimalData();
    this.Sprite.currentFrame = animalData != null ? animalData.SleepFrame : 12;
    this.FacingDirection = 2;
    this.Sprite.UpdateSourceRect();
    return true;
  }

  public override bool isMoving()
  {
    if (this._swimmingVelocity != Vector2.Zero)
      return true;
    return (this.IsActuallySwimming() || this.uniqueFrameAccumulator == -1) && base.isMoving();
  }

  public virtual bool updateWhenCurrentLocation(GameTime time, GameLocation location)
  {
    if (!Game1.shouldTimePass())
      return false;
    if (this.health.Value <= 0)
      return true;
    AnimatedSprite orLoadTexture = this.GetOrLoadTexture();
    this.doBuildingPokeEvent.Poll();
    this.doDiveEvent.Poll();
    if (this.IsActuallySwimming())
    {
      int num1 = 1;
      if (this.isMoving())
        num1 = 4;
      this.nextRipple -= (int) time.ElapsedGameTime.TotalMilliseconds * num1;
      if (this.nextRipple <= 0)
      {
        this.nextRipple = 2000;
        float scale = 1f;
        if (this.isBaby())
          scale = 0.65f;
        Point standingPixel = this.StandingPixel;
        float num2 = this.Position.X - (float) standingPixel.X;
        TemporaryAnimatedSprite temporaryAnimatedSprite = new TemporaryAnimatedSprite("TileSheets\\animations", new Microsoft.Xna.Framework.Rectangle(0, 0, 64 /*0x40*/, 64 /*0x40*/), this.isMoving() ? 75f : 150f, 8, 0, new Vector2((float) standingPixel.X + num2 * scale, (float) standingPixel.Y - 32f * scale), false, Game1.random.NextBool(), 0.01f, 0.01f, Color.White * 0.75f, scale, 0.0f, 0.0f, 0.0f);
        Vector2 vector2 = Utility.PointToVector2(Utility.getTranslatedPoint(new Point(), this.FacingDirection, -1));
        temporaryAnimatedSprite.motion = vector2 * 0.25f;
        location.TemporarySprites.Add(temporaryAnimatedSprite);
      }
    }
    if (this.hitGlowTimer > 0)
      this.hitGlowTimer -= time.ElapsedGameTime.Milliseconds;
    if (orLoadTexture.CurrentAnimation != null)
    {
      if (orLoadTexture.animateOnce(time))
        orLoadTexture.CurrentAnimation = (List<FarmerSprite.AnimationFrame>) null;
      return false;
    }
    this.update(time, location, this.myID.Value, false);
    if (this.hopOffset != Vector2.Zero)
    {
      orLoadTexture.UpdateSourceRect();
      this.HandleHop();
      return false;
    }
    if (Game1.IsMasterGame && this.behaviors(time, location) || orLoadTexture.CurrentAnimation != null)
      return false;
    PathFindController controller = this.controller;
    if ((controller != null ? (controller.timerSinceLastCheckPoint > 10000 ? 1 : 0) : 0) != 0)
    {
      this.controller = (PathFindController) null;
      this.Halt();
    }
    if (Game1.IsMasterGame)
    {
      if (!this.IsHome && this.noWarpTimer <= 0)
      {
        GameLocation homeInterior = this.homeInterior;
        if (homeInterior != null)
        {
          Microsoft.Xna.Framework.Rectangle boundingBox = this.GetBoundingBox();
          if (this.home.getRectForAnimalDoor().Contains(boundingBox.Center.X, boundingBox.Top))
          {
            if (Utility.isOnScreen(this.TilePoint, 192 /*0xC0*/, location))
              location.localSound("dwoop");
            location.animals.Remove(this.myID.Value);
            homeInterior.animals[this.myID.Value] = this;
            this.setRandomPosition(homeInterior);
            this.faceDirection(Game1.random.Next(4));
            this.controller = (PathFindController) null;
            return true;
          }
        }
      }
      this.noWarpTimer = Math.Max(0, this.noWarpTimer - time.ElapsedGameTime.Milliseconds);
    }
    if (this.pauseTimer > 0)
      this.pauseTimer -= time.ElapsedGameTime.Milliseconds;
    if (this.SleepIfNecessary())
    {
      if (!this.isEmoting && Game1.random.NextDouble() < 0.002)
        this.doEmote(24);
    }
    else if (this.pauseTimer <= 0 && Game1.random.NextDouble() < 0.001 && this.isAdult() && Game1.gameMode == (byte) 3 && Utility.isOnScreen(this.Position, 192 /*0xC0*/))
      this.makeSound();
    if (Game1.IsMasterGame)
    {
      this.UpdateRandomMovements();
      if (this.uniqueFrameAccumulator != -1 && this._followTarget != null && !FarmAnimal.GetFollowRange(this._followTarget, 1).Contains(this.StandingPixel))
        this.uniqueFrameAccumulator = -1;
      if (this.uniqueFrameAccumulator != -1)
      {
        this.uniqueFrameAccumulator += time.ElapsedGameTime.Milliseconds;
        if (this.uniqueFrameAccumulator > 500)
        {
          bool? uniqueAnimationFrames = this.GetAnimalData()?.UseDoubleUniqueAnimationFrames;
          if (uniqueAnimationFrames.HasValue && uniqueAnimationFrames.GetValueOrDefault())
            orLoadTexture.currentFrame = orLoadTexture.currentFrame + 1 - orLoadTexture.currentFrame % 2 * 2;
          else if (orLoadTexture.currentFrame > 12)
          {
            orLoadTexture.currentFrame = (orLoadTexture.currentFrame - 13) * 4;
          }
          else
          {
            switch (this.FacingDirection)
            {
              case 0:
                orLoadTexture.currentFrame = 15;
                break;
              case 1:
                orLoadTexture.currentFrame = 14;
                break;
              case 2:
                orLoadTexture.currentFrame = 13;
                break;
              case 3:
                orLoadTexture.currentFrame = 14;
                break;
            }
          }
          this.uniqueFrameAccumulator = 0;
          if (Game1.random.NextDouble() < 0.4)
            this.uniqueFrameAccumulator = -1;
        }
        if (this.IsActuallySwimming())
          this.MovePosition(time, Game1.viewport, location);
      }
      else
        this.MovePosition(time, Game1.viewport, location);
    }
    if (this.IsActuallySwimming())
    {
      FarmAnimalData animalData = this.GetAnimalData();
      orLoadTexture.UpdateSourceRect();
      Microsoft.Xna.Framework.Rectangle sourceRect = orLoadTexture.SourceRect;
      sourceRect.Offset(animalData != null ? animalData.SwimOffset : new Point(0, 112 /*0x70*/));
      orLoadTexture.SourceRect = sourceRect;
    }
    return false;
  }

  public virtual void UpdateRandomMovements()
  {
    if (!Game1.IsMasterGame || Game1.timeOfDay >= 2000 || this.pauseTimer > 0)
      return;
    if (this.fullness.Value < (int) byte.MaxValue && this.IsActuallySwimming() && Game1.random.NextDouble() < 0.002 && !this.isEating.Value)
      this.Eat(this.currentLocation);
    if (Game1.random.NextDouble() < 0.007 && this.uniqueFrameAccumulator == -1)
    {
      int direction = Game1.random.Next(5);
      if (direction != (this.FacingDirection + 2) % 4 || this.IsActuallySwimming())
      {
        if (direction < 4)
        {
          int facingDirection = this.FacingDirection;
          this.faceDirection(direction);
          if (!this.currentLocation.isOutdoors.Value && this.currentLocation.isCollidingPosition(this.nextPosition(direction), Game1.viewport, (Character) this))
          {
            this.faceDirection(facingDirection);
            return;
          }
        }
        switch (direction)
        {
          case 0:
            this.SetMovingUp(true);
            break;
          case 1:
            this.SetMovingRight(true);
            break;
          case 2:
            this.SetMovingDown(true);
            break;
          case 3:
            this.SetMovingLeft(true);
            break;
          default:
            this.Halt();
            this.Sprite.StopAnimation();
            break;
        }
      }
      else if (this.noWarpTimer <= 0)
      {
        this.Halt();
        this.Sprite.StopAnimation();
      }
    }
    if (!this.isMoving() || Game1.random.NextDouble() >= 0.014 || this.uniqueFrameAccumulator != -1)
      return;
    this.Halt();
    this.Sprite.StopAnimation();
    if (Game1.random.NextDouble() < 0.75)
    {
      FarmAnimalData animalData = this.GetAnimalData();
      this.uniqueFrameAccumulator = 0;
      bool? nullable = animalData?.UseDoubleUniqueAnimationFrames;
      if (nullable.HasValue && nullable.GetValueOrDefault())
      {
        switch (this.FacingDirection)
        {
          case 0:
            this.Sprite.currentFrame = 20;
            break;
          case 1:
            this.Sprite.currentFrame = 18;
            break;
          case 2:
            this.Sprite.currentFrame = 16 /*0x10*/;
            break;
          case 3:
            this.Sprite.currentFrame = 22;
            break;
        }
      }
      else
      {
        switch (this.FacingDirection)
        {
          case 0:
            this.Sprite.currentFrame = 15;
            break;
          case 1:
            this.Sprite.currentFrame = 14;
            break;
          case 2:
            this.Sprite.currentFrame = 13;
            break;
          case 3:
            AnimatedSprite sprite = this.Sprite;
            nullable = animalData?.UseFlippedRightForLeft;
            int num = !nullable.HasValue || !nullable.GetValueOrDefault() ? 12 : 14;
            sprite.currentFrame = num;
            break;
        }
      }
      this.uniqueFrameAccumulator = 0;
    }
    this.Sprite.UpdateSourceRect();
  }

  public virtual bool CanSwim()
  {
    FarmAnimalData animalData = this.GetAnimalData();
    return animalData != null && animalData.CanSwim;
  }

  public virtual bool CanFollowAdult()
  {
    if (!this.isBaby())
      return false;
    FarmAnimalData animalData = this.GetAnimalData();
    return animalData != null && animalData.BabiesFollowAdults;
  }

  public override bool shouldCollideWithBuildingLayer(GameLocation location) => true;

  public virtual void HandleHop()
  {
    int val1 = 4;
    if (!(this.hopOffset != Vector2.Zero))
      return;
    if ((double) this.hopOffset.X != 0.0)
    {
      int delta = (int) Math.Min((float) val1, Math.Abs(this.hopOffset.X));
      this.Position = this.Position + new Vector2((float) (delta * Math.Sign(this.hopOffset.X)), 0.0f);
      this.hopOffset.X = Utility.MoveTowards(this.hopOffset.X, 0.0f, (float) delta);
    }
    if ((double) this.hopOffset.Y != 0.0)
    {
      int delta = (int) Math.Min((float) val1, Math.Abs(this.hopOffset.Y));
      this.Position = this.Position + new Vector2(0.0f, (float) (delta * Math.Sign(this.hopOffset.Y)));
      this.hopOffset.Y = Utility.MoveTowards(this.hopOffset.Y, 0.0f, (float) delta);
    }
    if (!(this.hopOffset == Vector2.Zero) || !this.isSwimming.Value)
      return;
    this.Splash();
    this._swimmingVelocity = Utility.getTranslatedVector2(Vector2.Zero, this.FacingDirection, (float) this.speed);
    this.Position = new Vector2((float) (int) Math.Round((double) this.Position.X), (float) (int) Math.Round((double) this.Position.Y));
  }

  public override void MovePosition(
    GameTime time,
    xTile.Dimensions.Rectangle viewport,
    GameLocation currentLocation)
  {
    if (this.pauseTimer > 0 || Game1.IsClient)
      return;
    Location location = this.nextPositionTile();
    if (!currentLocation.isTileOnMap(new Vector2((float) location.X, (float) location.Y)))
    {
      this.FacingDirection = Utility.GetOppositeFacingDirection(this.FacingDirection);
      this.moveUp = this.facingDirection.Value == 0;
      this.moveLeft = this.facingDirection.Value == 3;
      this.moveDown = this.facingDirection.Value == 2;
      this.moveRight = this.facingDirection.Value == 1;
      this._followTarget = (FarmAnimal) null;
      this._followTargetPosition = new Point?();
      this._swimmingVelocity = Vector2.Zero;
    }
    else
    {
      if (this._followTarget != null && (this._followTarget.currentLocation != currentLocation || this._followTarget.health.Value <= 0))
      {
        this._followTarget = (FarmAnimal) null;
        this._followTargetPosition = new Point?();
      }
      if (this._followTargetPosition.HasValue)
      {
        Point standingPixel = this.StandingPixel;
        Point point1 = this._followTargetPosition.Value;
        Point point2 = new Point(standingPixel.X - point1.X, standingPixel.Y - point1.Y);
        if (Math.Abs(point2.X) <= 64 /*0x40*/ || Math.Abs(point2.Y) <= 64 /*0x40*/)
        {
          this.moveDown = false;
          this.moveUp = false;
          this.moveLeft = false;
          this.moveRight = false;
          this.GetNewFollowPosition();
        }
        else if (this.nextFollowDirectionChange >= 0)
        {
          this.nextFollowDirectionChange -= (int) time.ElapsedGameTime.TotalMilliseconds;
        }
        else
        {
          this.nextFollowDirectionChange = !this.IsActuallySwimming() ? 500 : 100;
          this.moveDown = false;
          this.moveUp = false;
          this.moveLeft = false;
          this.moveRight = false;
          if (Math.Abs(standingPixel.X - this._followTargetPosition.Value.X) < Math.Abs(standingPixel.Y - this._followTargetPosition.Value.Y))
          {
            if (standingPixel.Y > this._followTargetPosition.Value.Y)
              this.moveUp = true;
            else if (standingPixel.Y < this._followTargetPosition.Value.Y)
              this.moveDown = true;
          }
          else if (standingPixel.X < this._followTargetPosition.Value.X)
            this.moveRight = true;
          else if (standingPixel.X > this._followTargetPosition.Value.X)
            this.moveLeft = true;
        }
      }
      if (this.IsActuallySwimming())
      {
        Vector2 vector2 = new Vector2();
        if (!this.isEating.Value)
        {
          if (this.moveUp)
            vector2.Y = (float) -this.speed;
          else if (this.moveDown)
            vector2.Y = (float) this.speed;
          if (this.moveLeft)
            vector2.X = (float) -this.speed;
          else if (this.moveRight)
            vector2.X = (float) this.speed;
        }
        this._swimmingVelocity = new Vector2(Utility.MoveTowards(this._swimmingVelocity.X, vector2.X, 0.025f), Utility.MoveTowards(this._swimmingVelocity.Y, vector2.Y, 0.025f));
        Vector2 position = this.Position;
        this.Position = this.Position + this._swimmingVelocity;
        Microsoft.Xna.Framework.Rectangle boundingBox = this.GetBoundingBox();
        this.Position = position;
        int num = -1;
        if (!currentLocation.isCollidingPosition(boundingBox, Game1.viewport, false, 0, false, (Character) this, false))
        {
          this.Position = this.Position + this._swimmingVelocity;
          if ((double) Math.Abs(this._swimmingVelocity.X) > (double) Math.Abs(this._swimmingVelocity.Y))
          {
            if ((double) this._swimmingVelocity.X < 0.0)
              num = 3;
            else if ((double) this._swimmingVelocity.X > 0.0)
              num = 1;
          }
          else if ((double) this._swimmingVelocity.Y < 0.0)
            num = 0;
          else if ((double) this._swimmingVelocity.Y > 0.0)
            num = 2;
          switch (num)
          {
            case 0:
              this.Sprite.AnimateUp(time);
              this.faceDirection(0);
              break;
            case 1:
              this.Sprite.AnimateRight(time);
              this.faceDirection(1);
              break;
            case 2:
              this.Sprite.AnimateDown(time);
              this.faceDirection(2);
              break;
            case 3:
              this.Sprite.AnimateRight(time);
              this.FacingDirection = 3;
              break;
          }
        }
        else
        {
          if (this.HandleCollision(boundingBox))
            return;
          this.Halt();
          this.Sprite.StopAnimation();
          this._swimmingVelocity *= -1f;
        }
      }
      else if (this.moveUp)
      {
        if (!currentLocation.isCollidingPosition(this.nextPosition(0), Game1.viewport, false, 0, false, (Character) this, false))
        {
          this.position.Y -= (float) this.speed;
          this.Sprite.AnimateUp(time);
        }
        else if (!this.HandleCollision(this.nextPosition(0)))
        {
          this.Halt();
          this.Sprite.StopAnimation();
          if (Game1.random.NextDouble() < 0.6 || this.IsActuallySwimming())
            this.SetMovingDown(true);
        }
        this.faceDirection(0);
      }
      else if (this.moveRight)
      {
        if (!currentLocation.isCollidingPosition(this.nextPosition(1), Game1.viewport, false, 0, false, (Character) this))
        {
          this.position.X += (float) this.speed;
          this.Sprite.AnimateRight(time);
        }
        else if (!this.HandleCollision(this.nextPosition(1)))
        {
          this.Halt();
          this.Sprite.StopAnimation();
          if (Game1.random.NextDouble() < 0.6 || this.IsActuallySwimming())
            this.SetMovingLeft(true);
        }
        this.faceDirection(1);
      }
      else if (this.moveDown)
      {
        if (!currentLocation.isCollidingPosition(this.nextPosition(2), Game1.viewport, false, 0, false, (Character) this))
        {
          this.position.Y += (float) this.speed;
          this.Sprite.AnimateDown(time);
        }
        else if (!this.HandleCollision(this.nextPosition(2)))
        {
          this.Halt();
          this.Sprite.StopAnimation();
          if (Game1.random.NextDouble() < 0.6 || this.IsActuallySwimming())
            this.SetMovingUp(true);
        }
        this.faceDirection(2);
      }
      else
      {
        if (!this.moveLeft)
          return;
        if (!currentLocation.isCollidingPosition(this.nextPosition(3), Game1.viewport, false, 0, false, (Character) this))
        {
          this.position.X -= (float) this.speed;
          this.Sprite.AnimateRight(time);
        }
        else if (!this.HandleCollision(this.nextPosition(3)))
        {
          this.Halt();
          this.Sprite.StopAnimation();
          if (Game1.random.NextDouble() < 0.6 || this.IsActuallySwimming())
            this.SetMovingRight(true);
        }
        this.FacingDirection = 3;
      }
    }
  }

  public virtual bool HandleCollision(Microsoft.Xna.Framework.Rectangle next_position)
  {
    if (this._followTarget != null)
    {
      this._followTarget = (FarmAnimal) null;
      this._followTargetPosition = new Point?();
    }
    if (this.currentLocation.IsOutdoors && this.CanSwim() && (this.isSwimming.Value || this.controller == null) && this.wasPet.Value && this.hopOffset == Vector2.Zero)
    {
      this.Position = new Vector2((float) (int) Math.Round((double) this.Position.X), (float) (int) Math.Round((double) this.Position.Y));
      Microsoft.Xna.Framework.Rectangle boundingBox = this.GetBoundingBox();
      Vector2 translatedVector2 = Utility.getTranslatedVector2(Vector2.Zero, this.FacingDirection, 1f);
      if (translatedVector2 != Vector2.Zero)
      {
        Point tilePoint = this.TilePoint;
        tilePoint.X += (int) translatedVector2.X;
        tilePoint.Y += (int) translatedVector2.Y;
        Vector2 v = translatedVector2 * 128f;
        Microsoft.Xna.Framework.Rectangle position = boundingBox;
        position.Offset(Utility.Vector2ToPoint(v));
        Point point = new Point(position.X / 64 /*0x40*/, position.Y / 64 /*0x40*/);
        if (this.currentLocation.isWaterTile(tilePoint.X, tilePoint.Y) && this.currentLocation.doesTileHaveProperty(tilePoint.X, tilePoint.Y, "Passable", "Buildings") == null && !this.currentLocation.isCollidingPosition(position, Game1.viewport, false, 0, false, (Character) this) && this.currentLocation.isOpenWater(point.X, point.Y) != this.isSwimming.Value)
        {
          this.isSwimming.Value = !this.isSwimming.Value;
          if (!this.isSwimming.Value)
            this.Splash();
          this.hopOffset = v;
          this.pauseTimer = 0;
          this.doDiveEvent.Fire();
        }
        return true;
      }
    }
    return false;
  }

  public virtual bool IsActuallySwimming()
  {
    return this.isSwimming.Value && this.hopOffset == Vector2.Zero;
  }

  public virtual void Splash()
  {
    if (Utility.isOnScreen(this.TilePoint, 192 /*0xC0*/, this.currentLocation))
      this.currentLocation.playSound("dropItemInWater");
    Game1.multiplayer.broadcastSprites(this.currentLocation, new TemporaryAnimatedSprite(28, 100f, 2, 1, this.getStandingPosition() + new Vector2(-0.5f, -0.5f) * 64f, false, false)
    {
      delayBeforeAnimationStart = 0,
      layerDepth = (float) this.StandingPixel.Y / 10000f
    });
  }

  public override void animateInFacingDirection(GameTime time)
  {
    if (this.FacingDirection == 3)
      this.Sprite.AnimateRight(time);
    else
      base.animateInFacingDirection(time);
  }

  /// <summary>Log warnings if the farm animal's sprite is incorrectly sized, which would otherwise lead to hard-to-diagnose issues like animals freezing.</summary>
  private void ValidateSpritesheetSize()
  {
    int num1 = 5 + (!this.Sprite.textureUsesFlippedRightForLeft ? 1 : 0);
    bool? uniqueAnimationFrames = this.GetAnimalData()?.UseDoubleUniqueAnimationFrames;
    int num2 = uniqueAnimationFrames.HasValue && uniqueAnimationFrames.GetValueOrDefault() ? 1 : 0;
    int num3 = num1 + num2;
    if (this.Sprite.Texture.Height < num3 * this.Sprite.SpriteHeight)
      Game1.log.Warn($"Farm animal '{this.type.Value}' has sprite height {this.Sprite.Texture.Height}px, but expected at least {num3 * this.Sprite.SpriteHeight}px based on its data. This may cause issues like frozen animations.");
    if (this.Sprite.Texture.Width == 4 * this.Sprite.SpriteWidth)
      return;
    Game1.log.Warn($"Farm animal '{this.type.Value}' has sprite width {this.Sprite.Texture.Width}px, but it should be exactly {4 * this.Sprite.SpriteWidth}px.");
  }
}
