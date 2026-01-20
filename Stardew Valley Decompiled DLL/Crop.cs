// Decompiled with JetBrains decompiler
// Type: StardewValley.Crop
// Assembly: Stardew Valley, Version=1.6.15.24356, Culture=neutral, PublicKeyToken=null
// MVID: AA1F513A-94F0-4EF6-A35F-2D5B4A3721BF
// Assembly location: D:\SteamLibrary\steamapps\common\Stardew Valley\Stardew Valley.dll
// XML documentation location: D:\SteamLibrary\steamapps\common\Stardew Valley\Stardew Valley.xml

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Netcode;
using StardewValley.Characters;
using StardewValley.Extensions;
using StardewValley.GameData.Crops;
using StardewValley.GameData.GiantCrops;
using StardewValley.ItemTypeDefinitions;
using StardewValley.Locations;
using StardewValley.Mods;
using StardewValley.Objects;
using StardewValley.TerrainFeatures;
using System;
using System.Collections.Generic;
using System.Xml.Serialization;

#nullable disable
namespace StardewValley;

public class Crop : INetObject<NetFields>, IHaveModData
{
  public const string mixedSeedsId = "770";
  public const string mixedSeedsQId = "(O)770";
  public const int seedPhase = 0;
  public const int rowOfWildSeeds = 23;
  public const int finalPhaseLength = 99999;
  public const int forageCrop_springOnion = 1;
  public const string forageCrop_springOnionID = "1";
  public const int forageCrop_ginger = 2;
  public const string forageCrop_gingerID = "2";
  /// <summary>The <see cref="F:StardewValley.Item.specialVariable" /> value which indicates the object was spawned by a farmed forage crop.</summary>
  public const int specialVariable_farmedForageCrop = 724519;
  /// <summary>The backing field for <see cref="P:StardewValley.Crop.currentLocation" />.</summary>
  private GameLocation currentLocationImpl;
  /// <summary>The number of days in each visual step of growth before the crop is harvestable. The last entry in this list is <see cref="F:StardewValley.Crop.finalPhaseLength" />.</summary>
  public readonly NetIntList phaseDays = new NetIntList();
  /// <summary>The index of this crop in the spritesheet texture (one crop per row).</summary>
  [XmlElement("rowInSpriteSheet")]
  public readonly NetInt rowInSpriteSheet = new NetInt();
  [XmlElement("phaseToShow")]
  public readonly NetInt phaseToShow = new NetInt(-1);
  [XmlElement("currentPhase")]
  public readonly NetInt currentPhase = new NetInt();
  /// <summary>The unqualified item ID produced when this crop is harvested.</summary>
  [XmlElement("indexOfHarvest")]
  public readonly NetString indexOfHarvest = new NetString();
  [XmlElement("dayOfCurrentPhase")]
  public readonly NetInt dayOfCurrentPhase = new NetInt();
  /// <summary>The seed ID, if this is a forage or wild seed crop.</summary>
  [XmlElement("whichForageCrop")]
  public readonly NetString whichForageCrop = new NetString();
  /// <summary>If set, the qualified object ID to spawn on the crop's tile when it's full-grown. The crop will be removed when the object is spawned.</summary>
  [XmlElement("overrideHarvestItemId")]
  public readonly NetString replaceWithObjectOnFullGrown = new NetString();
  /// <summary>The tint colors that can be applied to the crop sprite, if any.</summary>
  [XmlElement("tintColor")]
  public readonly NetColor tintColor = new NetColor();
  [XmlElement("flip")]
  public readonly NetBool flip = new NetBool();
  [XmlElement("fullGrown")]
  public readonly NetBool fullyGrown = new NetBool();
  /// <summary>Whether this is a raised crop on a trellis that can't be walked through.</summary>
  [XmlElement("raisedSeeds")]
  public readonly NetBool raisedSeeds = new NetBool();
  /// <summary>Whether to apply the <see cref="F:StardewValley.Crop.tintColor" />.</summary>
  [XmlElement("programColored")]
  public readonly NetBool programColored = new NetBool();
  [XmlElement("dead")]
  public readonly NetBool dead = new NetBool();
  [XmlElement("forageCrop")]
  public readonly NetBool forageCrop = new NetBool();
  /// <summary>The unqualified seed ID, if this is a regular crop.</summary>
  [XmlElement("seedIndex")]
  public readonly NetString netSeedIndex = new NetString();
  /// <summary>The asset name for the crop texture under the game's <c>Content</c> folder, or null to use <see cref="F:StardewValley.Game1.cropSpriteSheetName" />.</summary>
  [XmlElement("overrideTexturePath")]
  public readonly NetString overrideTexturePath = new NetString();
  protected Texture2D _drawnTexture;
  protected bool? _isErrorCrop;
  [XmlIgnore]
  public Vector2 drawPosition;
  [XmlIgnore]
  public Vector2 tilePosition;
  [XmlIgnore]
  public float layerDepth;
  [XmlIgnore]
  public float coloredLayerDepth;
  [XmlIgnore]
  public Rectangle sourceRect;
  [XmlIgnore]
  public Rectangle coloredSourceRect;
  private static Vector2 origin = new Vector2(8f, 24f);
  private static Vector2 smallestTileSizeOrigin = new Vector2(8f, 8f);

  /// <summary>The location containing the crop.</summary>
  [XmlIgnore]
  public GameLocation currentLocation
  {
    get => this.currentLocationImpl;
    set
    {
      if (value == this.currentLocationImpl)
        return;
      this.currentLocationImpl = value;
      this.updateDrawMath(this.tilePosition);
    }
  }

  /// <summary>The dirt which contains this crop.</summary>
  [XmlIgnore]
  public HoeDirt Dirt { get; set; }

  [XmlIgnore]
  public Texture2D DrawnCropTexture
  {
    get
    {
      if (this.dead.Value)
        return Game1.cropSpriteSheet;
      if (this._drawnTexture == null)
      {
        if (this.overrideTexturePath.Value == null)
          this.overrideTexturePath.Value = this.GetData()?.GetCustomTextureName("TileSheets\\crops");
        this._drawnTexture = (Texture2D) null;
        if (this.overrideTexturePath.Value != null)
        {
          try
          {
            this._drawnTexture = Game1.content.Load<Texture2D>(this.overrideTexturePath.Value);
          }
          catch (Exception ex)
          {
            this._drawnTexture = (Texture2D) null;
          }
        }
        if (this._drawnTexture == null)
          this._drawnTexture = Game1.cropSpriteSheet;
      }
      return this._drawnTexture;
    }
  }

  /// <inheritdoc />
  [XmlIgnore]
  public ModDataDictionary modData { get; } = new ModDataDictionary();

  /// <inheritdoc />
  [XmlElement("modData")]
  public ModDataDictionary modDataForSerialization
  {
    get => this.modData.GetForSerialization();
    set => this.modData.SetFromSerialization(value);
  }

  public NetFields NetFields { get; } = new NetFields(nameof (Crop));

  public Crop()
  {
    this.NetFields.SetOwner((INetObject<NetFields>) this).AddField((INetSerializable) this.phaseDays, nameof (phaseDays)).AddField((INetSerializable) this.rowInSpriteSheet, nameof (rowInSpriteSheet)).AddField((INetSerializable) this.phaseToShow, nameof (phaseToShow)).AddField((INetSerializable) this.currentPhase, nameof (currentPhase)).AddField((INetSerializable) this.indexOfHarvest, nameof (indexOfHarvest)).AddField((INetSerializable) this.dayOfCurrentPhase, nameof (dayOfCurrentPhase)).AddField((INetSerializable) this.whichForageCrop, nameof (whichForageCrop)).AddField((INetSerializable) this.replaceWithObjectOnFullGrown, nameof (replaceWithObjectOnFullGrown)).AddField((INetSerializable) this.tintColor, nameof (tintColor)).AddField((INetSerializable) this.flip, nameof (flip)).AddField((INetSerializable) this.fullyGrown, nameof (fullyGrown)).AddField((INetSerializable) this.raisedSeeds, nameof (raisedSeeds)).AddField((INetSerializable) this.programColored, nameof (programColored)).AddField((INetSerializable) this.dead, nameof (dead)).AddField((INetSerializable) this.forageCrop, nameof (forageCrop)).AddField((INetSerializable) this.netSeedIndex, nameof (netSeedIndex)).AddField((INetSerializable) this.overrideTexturePath, nameof (overrideTexturePath)).AddField((INetSerializable) this.modData, nameof (modData));
    this.dayOfCurrentPhase.fieldChangeVisibleEvent += (FieldChange<NetInt, int>) ((_param1, _param2, _param3) => this.updateDrawMath(this.tilePosition));
    this.fullyGrown.fieldChangeVisibleEvent += (FieldChange<NetBool, bool>) ((_param1, _param2, _param3) => this.updateDrawMath(this.tilePosition));
    this.currentLocation = Game1.currentLocation;
  }

  public Crop(bool forageCrop, string which, int tileX, int tileY, GameLocation location)
    : this()
  {
    this.currentLocation = location;
    this.forageCrop.Value = forageCrop;
    this.whichForageCrop.Value = which;
    this.fullyGrown.Value = true;
    this.currentPhase.Value = 5;
    this.updateDrawMath(new Vector2((float) tileX, (float) tileY));
  }

  public Crop(string seedId, int tileX, int tileY, GameLocation location)
    : this()
  {
    this.currentLocation = location;
    seedId = Crop.ResolveSeedId(seedId, location);
    CropData data;
    if (Crop.TryGetData(seedId, out data))
    {
      ParsedItemData dataOrErrorItem = ItemRegistry.GetDataOrErrorItem(data.HarvestItemId);
      if (!dataOrErrorItem.HasTypeObject())
        Game1.log.Warn($"Crop seed {seedId} produces non-object item {dataOrErrorItem.QualifiedItemId}, which isn't valid.");
      this.phaseDays.AddRange((IEnumerable<int>) data.DaysInPhase);
      this.phaseDays.Add(99999);
      this.rowInSpriteSheet.Value = data.SpriteIndex;
      this.indexOfHarvest.Value = dataOrErrorItem.ItemId;
      this.overrideTexturePath.Value = data.GetCustomTextureName("TileSheets\\crops");
      if (this.isWildSeedCrop())
      {
        this.whichForageCrop.Value = seedId;
        this.replaceWithObjectOnFullGrown.Value = this.getRandomWildCropForSeason(true);
      }
      else
        this.netSeedIndex.Value = seedId;
      this.raisedSeeds.Value = data.IsRaised;
      List<string> tintColors = data.TintColors;
      // ISSUE: explicit non-virtual call
      if ((tintColors != null ? (__nonvirtual (tintColors.Count) > 0 ? 1 : 0) : 0) != 0)
      {
        Color? color = Utility.StringToColor(Utility.CreateRandom((double) tileX * 1000.0, (double) tileY, (double) Game1.dayOfMonth).ChooseFrom<string>((IList<string>) data.TintColors));
        if (color.HasValue)
        {
          this.tintColor.Value = color.Value;
          this.programColored.Value = true;
        }
      }
    }
    else
    {
      this.netSeedIndex.Value = seedId ?? "0";
      this.indexOfHarvest.Value = seedId ?? "0";
    }
    this.flip.Value = Game1.random.NextBool();
    this.updateDrawMath(new Vector2((float) tileX, (float) tileY));
  }

  /// <summary>Choose a random seed from a bag of mixed seeds, if applicable.</summary>
  /// <param name="itemId">The unqualified item ID for the seed item.</param>
  /// <param name="location">The location for which to resolve the crop.</param>
  /// <returns>Returns the unqualified seed ID to use.</returns>
  public static string ResolveSeedId(string itemId, GameLocation location)
  {
    switch (itemId)
    {
      case "MixedFlowerSeeds":
        return Crop.getRandomFlowerSeedForThisSeason(location.GetSeason());
      case "770":
        string str = Crop.getRandomLowGradeCropForThisSeason(location.GetSeason());
        if (str == "473")
          str = "472";
        if (location is IslandLocation)
        {
          switch (Game1.random.Next(4))
          {
            case 0:
              str = "479";
              break;
            case 1:
              str = "833";
              break;
            case 2:
              str = "481";
              break;
            default:
              str = "478";
              break;
          }
        }
        return str;
      default:
        return itemId;
    }
  }

  /// <summary>Get the crop's data from <see cref="F:StardewValley.Game1.cropData" />, if found.</summary>
  public CropData GetData()
  {
    CropData data;
    return !Crop.TryGetData(this.isWildSeedCrop() ? this.whichForageCrop.Value : this.netSeedIndex.Value, out data) ? (CropData) null : data;
  }

  /// <summary>Try to get a crop's data from <see cref="F:StardewValley.Game1.cropData" />.</summary>
  /// <param name="seedId">The unqualified item ID for the crop's seed (i.e. the key in <see cref="F:StardewValley.Game1.cropData" />).</param>
  /// <param name="data">The crop data, if found.</param>
  /// <returns>Returns whether the crop data was found.</returns>
  public static bool TryGetData(string seedId, out CropData data)
  {
    if (seedId != null)
      return Game1.cropData.TryGetValue(seedId, out data);
    data = (CropData) null;
    return false;
  }

  /// <summary>Get whether this crop is in season for the given location.</summary>
  /// <param name="location">The location to check.</param>
  public bool IsInSeason(GameLocation location)
  {
    return location.SeedsIgnoreSeasonsHere() || (this.GetData()?.Seasons?.Contains(location.GetSeason()) ?? false);
  }

  /// <summary>Get whether a crop is in season for the given location.</summary>
  /// <param name="location">The location to check.</param>
  /// <param name="seedId">The unqualified item ID for the crop's seed.</param>
  public static bool IsInSeason(GameLocation location, string seedId)
  {
    if (location.SeedsIgnoreSeasonsHere())
      return true;
    CropData data;
    return Crop.TryGetData(seedId, out data) && (data.Seasons?.Contains(location.GetSeason()) ?? false);
  }

  /// <summary>Get the method by which the crop can be harvested.</summary>
  public HarvestMethod GetHarvestMethod()
  {
    CropData data = this.GetData();
    return data == null ? HarvestMethod.Grab : data.HarvestMethod;
  }

  /// <summary>Get whether this crop regrows after it's harvested.</summary>
  public bool RegrowsAfterHarvest()
  {
    CropData data = this.GetData();
    return data != null && data.RegrowDays > 0;
  }

  public virtual bool IsErrorCrop()
  {
    if (this.forageCrop.Value)
      return false;
    if (!this._isErrorCrop.HasValue)
      this._isErrorCrop = new bool?(this.GetData() == null);
    return this._isErrorCrop.Value;
  }

  public virtual void ResetPhaseDays()
  {
    CropData data = this.GetData();
    if (data == null)
      return;
    this.phaseDays.Clear();
    this.phaseDays.AddRange((IEnumerable<int>) data.DaysInPhase);
    this.phaseDays.Add(99999);
  }

  public static string getRandomLowGradeCropForThisSeason(Season season)
  {
    if (season == Season.Winter)
      season = Game1.random.Choose<Season>(Season.Spring, Season.Summer, Season.Fall);
    switch (season)
    {
      case Season.Spring:
        return Game1.random.Next(472, 476).ToString();
      case Season.Summer:
        switch (Game1.random.Next(4))
        {
          case 0:
            return "487";
          case 1:
            return "483";
          case 2:
            return "482";
          default:
            return "484";
        }
      case Season.Fall:
        return Game1.random.Next(487, 491).ToString();
      default:
        return (string) null;
    }
  }

  public static string getRandomFlowerSeedForThisSeason(Season season)
  {
    if (season == Season.Winter)
      season = Game1.random.Choose<Season>(Season.Spring, Season.Summer, Season.Fall);
    switch (season)
    {
      case Season.Spring:
        return Game1.random.Choose<string>("427", "429");
      case Season.Summer:
        return Game1.random.Choose<string>("455", "453", "431");
      case Season.Fall:
        return Game1.random.Choose<string>("431", "425");
      default:
        return "-1";
    }
  }

  public virtual void growCompletely()
  {
    this.currentPhase.Value = this.phaseDays.Count - 1;
    this.dayOfCurrentPhase.Value = 0;
    if (this.RegrowsAfterHarvest())
      this.fullyGrown.Value = true;
    this.updateDrawMath(this.tilePosition);
  }

  public virtual bool hitWithHoe(int xTile, int yTile, GameLocation location, HoeDirt dirt)
  {
    if (!this.forageCrop.Value || !(this.whichForageCrop.Value == "2"))
      return false;
    dirt.state.Value = location.IsRainingHere() ? 1 : 0;
    Object @object = ItemRegistry.Create<Object>("(O)829");
    Game1.multiplayer.broadcastSprites(location, new TemporaryAnimatedSprite(12, new Vector2((float) (xTile * 64 /*0x40*/), (float) (yTile * 64 /*0x40*/)), Color.White, flipped: Game1.random.NextBool(), animationInterval: 50f));
    location.playSound("dirtyHit");
    Game1.createItemDebris(@object.getOne(), new Vector2((float) (xTile * 64 /*0x40*/ + 32 /*0x20*/), (float) (yTile * 64 /*0x40*/ + 32 /*0x20*/)), -1);
    return true;
  }

  public virtual bool harvest(
    int xTile,
    int yTile,
    HoeDirt soil,
    JunimoHarvester junimoHarvester = null,
    bool isForcedScytheHarvest = false)
  {
    if (this.dead.Value)
      return junimoHarvester != null;
    bool flag = false;
    if (this.forageCrop.Value)
    {
      Object i = (Object) null;
      int howMuch = 3;
      Random daySaveRandom = Utility.CreateDaySaveRandom((double) (xTile * 1000), (double) (yTile * 2000));
      switch (this.whichForageCrop.Value)
      {
        case "1":
          i = ItemRegistry.Create<Object>("(O)399");
          break;
        case "2":
          soil.shake((float) Math.PI / 48f, (float) Math.PI / 40f, (double) (xTile * 64 /*0x40*/) < (double) Game1.player.Position.X);
          return false;
      }
      if (Game1.player.professions.Contains(16 /*0x10*/))
        i.Quality = 4;
      else if (daySaveRandom.NextDouble() < (double) Game1.player.ForagingLevel / 30.0)
        i.Quality = 2;
      else if (daySaveRandom.NextDouble() < (double) Game1.player.ForagingLevel / 15.0)
        i.Quality = 1;
      Game1.stats.ItemsForaged += (uint) i.Stack;
      if (junimoHarvester != null)
      {
        junimoHarvester.tryToAddItemToHut((Item) i);
        return true;
      }
      if (isForcedScytheHarvest)
      {
        Vector2 vector2 = new Vector2((float) xTile, (float) yTile);
        Game1.createItemDebris((Item) i, new Vector2((float) ((double) vector2.X * 64.0 + 32.0), (float) ((double) vector2.Y * 64.0 + 32.0)), -1);
        Game1.player.gainExperience(2, howMuch);
        Game1.player.currentLocation.playSound("moss_cut");
        return true;
      }
      if (Game1.player.addItemToInventoryBool((Item) i))
      {
        Vector2 vector2 = new Vector2((float) xTile, (float) yTile);
        Game1.player.animateOnce(279 + Game1.player.FacingDirection);
        Game1.player.canMove = false;
        Game1.player.currentLocation.playSound(nameof (harvest));
        DelayedAction.playSoundAfterDelay("coin", 260);
        if (!this.RegrowsAfterHarvest())
        {
          Game1.multiplayer.broadcastSprites(Game1.currentLocation, new TemporaryAnimatedSprite(17, new Vector2(vector2.X * 64f, vector2.Y * 64f), Color.White, 7, daySaveRandom.NextBool(), 125f));
          Game1.multiplayer.broadcastSprites(Game1.currentLocation, new TemporaryAnimatedSprite(14, new Vector2(vector2.X * 64f, vector2.Y * 64f), Color.White, 7, daySaveRandom.NextBool(), 50f));
        }
        Game1.player.gainExperience(2, howMuch);
        return true;
      }
      Game1.showRedMessage(Game1.content.LoadString("Strings\\StringsFromCSFiles:Crop.cs.588"));
    }
    else if (this.currentPhase.Value >= this.phaseDays.Count - 1 && (!this.fullyGrown.Value || this.dayOfCurrentPhase.Value <= 0))
    {
      if (string.IsNullOrWhiteSpace(this.indexOfHarvest.Value))
        return true;
      CropData data = this.GetData();
      Random random = Utility.CreateRandom((double) xTile * 7.0, (double) yTile * 11.0, (double) Game1.stats.DaysPlayed, (double) Game1.uniqueIDForThisGame);
      int qualityBoostLevel = soil.GetFertilizerQualityBoostLevel();
      double num1 = 0.2 * ((double) Game1.player.FarmingLevel / 10.0) + 0.2 * (double) qualityBoostLevel * (((double) Game1.player.FarmingLevel + 2.0) / 12.0) + 0.01;
      double num2 = Math.Min(0.75, num1 * 2.0);
      int num3 = 0;
      if (qualityBoostLevel >= 3 && random.NextDouble() < num1 / 2.0)
        num3 = 4;
      else if (random.NextDouble() < num1)
        num3 = 2;
      else if (random.NextDouble() < num2 || qualityBoostLevel >= 3)
        num3 = 1;
      int quality = MathHelper.Clamp(num3, data != null ? data.HarvestMinQuality : 0, (int?) data?.HarvestMaxQuality ?? num3);
      int num4 = 1;
      if (data != null)
      {
        int harvestMinStack = data.HarvestMinStack;
        int num5 = Math.Max(harvestMinStack, data.HarvestMaxStack);
        if ((double) data.HarvestMaxIncreasePerFarmingLevel > 0.0)
          num5 += (int) ((double) Game1.player.FarmingLevel * (double) data.HarvestMaxIncreasePerFarmingLevel);
        if (harvestMinStack > 1 || num5 > 1)
          num4 = random.Next(harvestMinStack, num5 + 1);
      }
      if (data != null && data.ExtraHarvestChance > 0.0)
      {
        while (random.NextDouble() < Math.Min(0.9, data.ExtraHarvestChance))
          ++num4;
      }
      Item obj1;
      if (!this.programColored.Value)
      {
        obj1 = ItemRegistry.Create(this.indexOfHarvest.Value, quality: quality);
      }
      else
      {
        obj1 = (Item) new ColoredObject(this.indexOfHarvest.Value, 1, this.tintColor.Value);
        obj1.Quality = quality;
      }
      Item obj2 = obj1;
      HarvestMethod harvestMethod = data != null ? data.HarvestMethod : HarvestMethod.Grab;
      if (harvestMethod == HarvestMethod.Scythe | isForcedScytheHarvest)
      {
        if (junimoHarvester != null)
        {
          DelayedAction.playSoundAfterDelay("daggerswipe", 150, junimoHarvester.currentLocation);
          if (Utility.isOnScreen(junimoHarvester.TilePoint, 64 /*0x40*/, junimoHarvester.currentLocation))
          {
            junimoHarvester.currentLocation.playSound(nameof (harvest));
            DelayedAction.playSoundAfterDelay("coin", 260, junimoHarvester.currentLocation);
          }
          junimoHarvester.tryToAddItemToHut(obj2.getOne());
        }
        else
          Game1.createItemDebris(obj2.getOne(), new Vector2((float) (xTile * 64 /*0x40*/ + 32 /*0x20*/), (float) (yTile * 64 /*0x40*/ + 32 /*0x20*/)), -1);
        flag = true;
      }
      else if (junimoHarvester != null || obj2 != null && Game1.player.addItemToInventoryBool(obj2.getOne()))
      {
        Vector2 vector2 = new Vector2((float) xTile, (float) yTile);
        if (junimoHarvester == null)
        {
          Game1.player.animateOnce(279 + Game1.player.FacingDirection);
          Game1.player.canMove = false;
        }
        else
          junimoHarvester.tryToAddItemToHut(obj2.getOne());
        if (random.NextDouble() < Game1.player.team.AverageLuckLevel() / 1500.0 + Game1.player.team.AverageDailyLuck() / 1200.0 + 9.9999997473787516E-05)
        {
          num4 *= 2;
          if (junimoHarvester == null)
            Game1.player.currentLocation.playSound("dwoop");
          else if (Utility.isOnScreen(junimoHarvester.TilePoint, 64 /*0x40*/, junimoHarvester.currentLocation))
            junimoHarvester.currentLocation.playSound("dwoop");
        }
        else if (harvestMethod == HarvestMethod.Grab)
        {
          if (junimoHarvester == null)
            Game1.player.currentLocation.playSound(nameof (harvest));
          else if (Utility.isOnScreen(junimoHarvester.TilePoint, 64 /*0x40*/, junimoHarvester.currentLocation))
            junimoHarvester.currentLocation.playSound(nameof (harvest));
          if (junimoHarvester == null)
            DelayedAction.playSoundAfterDelay("coin", 260, Game1.player.currentLocation);
          else if (Utility.isOnScreen(junimoHarvester.TilePoint, 64 /*0x40*/, junimoHarvester.currentLocation))
            DelayedAction.playSoundAfterDelay("coin", 260, junimoHarvester.currentLocation);
          if (!this.RegrowsAfterHarvest() && (junimoHarvester == null || junimoHarvester.currentLocation.Equals(Game1.currentLocation)))
          {
            Game1.multiplayer.broadcastSprites(Game1.currentLocation, new TemporaryAnimatedSprite(17, new Vector2(vector2.X * 64f, vector2.Y * 64f), Color.White, 7, Game1.random.NextBool(), 125f));
            Game1.multiplayer.broadcastSprites(Game1.currentLocation, new TemporaryAnimatedSprite(14, new Vector2(vector2.X * 64f, vector2.Y * 64f), Color.White, 7, Game1.random.NextBool(), 50f));
          }
        }
        flag = true;
      }
      else
        Game1.showRedMessage(Game1.content.LoadString("Strings\\StringsFromCSFiles:Crop.cs.588"));
      if (flag)
      {
        if (this.indexOfHarvest.Value == "421")
        {
          this.indexOfHarvest.Value = "431";
          num4 = random.Next(1, 4);
        }
        Item obj3 = this.programColored.Value ? (Item) new ColoredObject(this.indexOfHarvest.Value, 1, this.tintColor.Value) : ItemRegistry.Create(this.indexOfHarvest.Value);
        int num6 = 0;
        if (obj3 is Object @object)
          num6 = @object.Price;
        float a = (float) (16.0 * Math.Log(0.018 * (double) num6 + 1.0, Math.E));
        if (junimoHarvester == null)
          Game1.player.gainExperience(0, (int) Math.Round((double) a));
        for (int index = 0; index < num4 - 1; ++index)
        {
          if (junimoHarvester == null)
            Game1.createItemDebris(obj3.getOne(), new Vector2((float) (xTile * 64 /*0x40*/ + 32 /*0x20*/), (float) (yTile * 64 /*0x40*/ + 32 /*0x20*/)), -1);
          else
            junimoHarvester.tryToAddItemToHut(obj3.getOne());
        }
        switch (this.indexOfHarvest.Value)
        {
          case "262":
            if (random.NextDouble() < 0.4)
            {
              Item obj4 = ItemRegistry.Create("(O)178");
              if (junimoHarvester == null)
              {
                Game1.createItemDebris(obj4.getOne(), new Vector2((float) (xTile * 64 /*0x40*/ + 32 /*0x20*/), (float) (yTile * 64 /*0x40*/ + 32 /*0x20*/)), -1);
                break;
              }
              junimoHarvester.tryToAddItemToHut(obj4.getOne());
              break;
            }
            break;
          case "771":
            soil?.Location?.playSound("cut");
            if (random.NextDouble() < 0.1)
            {
              Item obj5 = ItemRegistry.Create("(O)770");
              if (junimoHarvester == null)
              {
                Game1.createItemDebris(obj5.getOne(), new Vector2((float) (xTile * 64 /*0x40*/ + 32 /*0x20*/), (float) (yTile * 64 /*0x40*/ + 32 /*0x20*/)), -1);
                break;
              }
              junimoHarvester.tryToAddItemToHut(obj5.getOne());
              break;
            }
            break;
        }
        int num7 = data != null ? data.RegrowDays : -1;
        if (num7 <= 0)
          return true;
        this.fullyGrown.Value = true;
        if (this.dayOfCurrentPhase.Value == num7)
          this.updateDrawMath(this.tilePosition);
        this.dayOfCurrentPhase.Value = num7;
      }
    }
    return false;
  }

  /// <summary>Get a random qualified object ID to harvest from wild seeds.</summary>
  /// <param name="onlyDeterministic">Only return a value if it can be accurately predicted ahead of time (i.e. the harvest doesn't depend on the date that it's harvested).</param>
  /// <remarks>This uses the season associated with the crop (e.g. spring for Spring Seeds) or the current location's season.</remarks>
  public string getRandomWildCropForSeason(bool onlyDeterministic = false)
  {
    switch (this.whichForageCrop.Value)
    {
      case "495":
        return this.getRandomWildCropForSeason(Season.Spring);
      case "496":
        return this.getRandomWildCropForSeason(Season.Summer);
      case "497":
        return this.getRandomWildCropForSeason(Season.Fall);
      case "498":
        return this.getRandomWildCropForSeason(Season.Winter);
      default:
        return onlyDeterministic && !this.currentLocation.SeedsIgnoreSeasonsHere() ? (string) null : this.getRandomWildCropForSeason(this.currentLocation.GetSeason());
    }
  }

  /// <summary>Get a random qualified object ID to harvest from wild seeds.</summary>
  /// <param name="season">The season for which to choose a produce.</param>
  public string getRandomWildCropForSeason(Season season)
  {
    switch (season)
    {
      case Season.Spring:
        return Game1.random.Choose<string>("(O)16", "(O)18", "(O)20", "(O)22");
      case Season.Summer:
        return Game1.random.Choose<string>("(O)396", "(O)398", "(O)402");
      case Season.Fall:
        return Game1.random.Choose<string>("(O)404", "(O)406", "(O)408", "(O)410");
      case Season.Winter:
        return Game1.random.Choose<string>("(O)412", "(O)414", "(O)416", "(O)418");
      default:
        return "(O)22";
    }
  }

  public virtual Rectangle getSourceRect(int number)
  {
    if (this.dead.Value)
      return new Rectangle(192 /*0xC0*/ + number % 4 * 16 /*0x10*/, 384, 16 /*0x10*/, 32 /*0x20*/);
    int num = this.rowInSpriteSheet.Value;
    Season seasonForLocation = Game1.GetSeasonForLocation(this.currentLocation);
    if (this.indexOfHarvest.Value == "771")
    {
      switch (seasonForLocation)
      {
        case Season.Fall:
          num = this.rowInSpriteSheet.Value + 1;
          break;
        case Season.Winter:
          num = this.rowInSpriteSheet.Value + 2;
          break;
      }
    }
    return new Rectangle(Math.Min(240 /*0xF0*/, (this.fullyGrown.Value ? (this.dayOfCurrentPhase.Value <= 0 ? 6 : 7) : (this.phaseToShow.Value != -1 ? this.phaseToShow.Value : this.currentPhase.Value) + ((this.phaseToShow.Value != -1 ? this.phaseToShow.Value : this.currentPhase.Value) != 0 || number % 2 != 0 ? 0 : -1) + 1) * 16 /*0x10*/ + (num % 2 != 0 ? 128 /*0x80*/ : 0)), num / 2 * 16 /*0x10*/ * 2, 16 /*0x10*/, 32 /*0x20*/);
  }

  /// <summary>Get the giant crops which can grow from this crop, if any.</summary>
  /// <param name="giantCrops">The giant crops which can grow from this crop.</param>
  /// <returns>Returns whether <paramref name="giantCrops" /> is non-empty.</returns>
  public bool TryGetGiantCrops(
    out IReadOnlyList<KeyValuePair<string, GiantCropData>> giantCrops)
  {
    giantCrops = GiantCrop.GetGiantCropsFor("(O)" + this.indexOfHarvest.Value);
    return giantCrops.Count > 0;
  }

  public void Kill()
  {
    this.dead.Value = true;
    this.raisedSeeds.Value = false;
  }

  public virtual void newDay(int state)
  {
    GameLocation currentLocation = this.currentLocation;
    Vector2 tilePosition = this.tilePosition;
    Utility.Vector2ToPoint(tilePosition);
    if (currentLocation.isOutdoors.Value && (this.dead.Value || !this.IsInSeason(currentLocation)))
    {
      this.Kill();
    }
    else
    {
      if (state != 1)
      {
        CropData data = this.GetData();
        if ((data != null ? (!data.NeedsWatering ? 1 : 0) : 0) == 0)
          goto label_14;
      }
      if (!this.fullyGrown.Value)
        this.dayOfCurrentPhase.Value = Math.Min(this.dayOfCurrentPhase.Value + 1, this.phaseDays.Count > 0 ? this.phaseDays[Math.Min(this.phaseDays.Count - 1, this.currentPhase.Value)] : 0);
      else
        --this.dayOfCurrentPhase.Value;
      if (this.dayOfCurrentPhase.Value >= (this.phaseDays.Count > 0 ? this.phaseDays[Math.Min(this.phaseDays.Count - 1, this.currentPhase.Value)] : 0) && this.currentPhase.Value < this.phaseDays.Count - 1)
      {
        ++this.currentPhase.Value;
        this.dayOfCurrentPhase.Value = 0;
      }
      NetInt currentPhase;
      for (; this.currentPhase.Value < this.phaseDays.Count - 1 && this.phaseDays.Count > 0 && this.phaseDays[this.currentPhase.Value] <= 0; ++currentPhase.Value)
        currentPhase = this.currentPhase;
      if (this.isWildSeedCrop() && this.phaseToShow.Value == -1 && this.currentPhase.Value > 0)
        this.phaseToShow.Value = Game1.random.Next(1, 7);
      this.TryGrowGiantCrop();
label_14:
      if ((!this.fullyGrown.Value || this.dayOfCurrentPhase.Value <= 0) && this.currentPhase.Value >= this.phaseDays.Count - 1)
      {
        if (this.replaceWithObjectOnFullGrown.Value != null || this.isWildSeedCrop())
        {
          Object object1;
          if (currentLocation.objects.TryGetValue(tilePosition, out object1))
          {
            if (object1 is IndoorPot indoorPot)
            {
              indoorPot.heldObject.Value = ItemRegistry.Create<Object>(this.replaceWithObjectOnFullGrown.Value ?? this.getRandomWildCropForSeason());
              indoorPot.hoeDirt.Value.crop = (Crop) null;
            }
            else
              currentLocation.objects.Remove(tilePosition);
          }
          if (!currentLocation.objects.ContainsKey(tilePosition))
          {
            Object object2 = ItemRegistry.Create<Object>(this.replaceWithObjectOnFullGrown.Value ?? this.getRandomWildCropForSeason());
            object2.IsSpawnedObject = true;
            object2.CanBeGrabbed = true;
            object2.SpecialVariable = 724519;
            currentLocation.objects.Add(tilePosition, object2);
          }
          TerrainFeature terrainFeature;
          if (currentLocation.terrainFeatures.TryGetValue(tilePosition, out terrainFeature) && terrainFeature is HoeDirt hoeDirt)
            hoeDirt.crop = (Crop) null;
        }
        if (this.indexOfHarvest.Value != null && this.indexOfHarvest.Value != null && this.indexOfHarvest.Value.Length > 0 && currentLocation.IsFarm)
        {
          foreach (Farmer allFarmer in Game1.getAllFarmers())
            allFarmer.autoGenerateActiveDialogueEvent("cropMatured_" + this.indexOfHarvest.Value);
        }
      }
      if (this.fullyGrown.Value && this.indexOfHarvest.Value != null && this.indexOfHarvest.Value != null && this.indexOfHarvest.Value == "595")
        Game1.getFarm().hasMatureFairyRoseTonight = true;
      this.updateDrawMath(tilePosition);
    }
  }

  /// <summary>Try to replace the grid of crops with this one at its top-left corner with a giant crop, if valid and the probability check passes.</summary>
  /// <param name="checkPreconditions">Whether to check that the location allows giant crops and the crop is fully grown. Setting this to false won't affect other conditions like having a grid of crops or the per-giant-crop conditions and chance.</param>
  /// <param name="random">The RNG to use for random checks, or <c>null</c> for the default seed logic.</param>
  public virtual bool TryGrowGiantCrop(bool checkPreconditions = true, Random random = null)
  {
    GameLocation currentLocation = this.currentLocation;
    Vector2 tilePosition = this.tilePosition;
    IReadOnlyList<KeyValuePair<string, GiantCropData>> giantCrops;
    if (checkPreconditions && (!(currentLocation is Farm) && !currentLocation.HasMapPropertyWithValue("AllowGiantCrops") || this.currentPhase.Value != this.phaseDays.Count - 1) || !this.TryGetGiantCrops(out giantCrops))
      return false;
    foreach (KeyValuePair<string, GiantCropData> keyValuePair in (IEnumerable<KeyValuePair<string, GiantCropData>>) giantCrops)
    {
      string key1 = keyValuePair.Key;
      GiantCropData giantCropData = keyValuePair.Value;
      if (((double) giantCropData.Chance >= 1.0 || (random ?? Utility.CreateDaySaveRandom((double) tilePosition.X, (double) tilePosition.Y, (double) Game1.hash.GetDeterministicHashCode(key1))).NextBool(giantCropData.Chance)) && GameStateQuery.CheckConditions(giantCropData.Condition, currentLocation))
      {
        bool flag = true;
        for (int y = (int) tilePosition.Y; (double) y < (double) tilePosition.Y + (double) giantCropData.TileSize.Y; ++y)
        {
          for (int x = (int) tilePosition.X; (double) x < (double) tilePosition.X + (double) giantCropData.TileSize.X; ++x)
          {
            if (!(currentLocation.terrainFeatures.GetValueOrDefault(new Vector2((float) x, (float) y)) is HoeDirt valueOrDefault) || !(valueOrDefault.crop?.indexOfHarvest.Value == this.indexOfHarvest.Value))
            {
              flag = false;
              break;
            }
          }
          if (!flag)
            break;
        }
        if (flag)
        {
          for (int y = (int) tilePosition.Y; (double) y < (double) tilePosition.Y + (double) giantCropData.TileSize.Y; ++y)
          {
            for (int x = (int) tilePosition.X; (double) x < (double) tilePosition.X + (double) giantCropData.TileSize.X; ++x)
            {
              Vector2 key2 = new Vector2((float) x, (float) y);
              ((HoeDirt) currentLocation.terrainFeatures[key2]).crop = (Crop) null;
            }
          }
          currentLocation.resourceClumps.Add((ResourceClump) new GiantCrop(key1, tilePosition));
          return true;
        }
      }
    }
    return false;
  }

  public virtual bool isPaddyCrop() => this.GetData()?.IsPaddyCrop ?? false;

  public virtual bool shouldDrawDarkWhenWatered() => !this.isPaddyCrop() && !this.raisedSeeds.Value;

  /// <summary>Get whether this is a vanilla wild seed crop.</summary>
  public virtual bool isWildSeedCrop()
  {
    return (this.overrideTexturePath.Value == null || this.overrideTexturePath.Value == Game1.cropSpriteSheet.Name) && this.rowInSpriteSheet.Value == 23;
  }

  public virtual void updateDrawMath(Vector2 tileLocation)
  {
    if (tileLocation.Equals(Vector2.Zero))
      return;
    if (this.forageCrop.Value)
    {
      int result;
      if (!int.TryParse(this.whichForageCrop.Value, out result))
        result = 1;
      this.drawPosition = new Vector2((float) ((double) tileLocation.X * 64.0 + (((double) tileLocation.X * 11.0 + (double) tileLocation.Y * 7.0) % 10.0 - 5.0) + 32.0), (float) ((double) tileLocation.Y * 64.0 + (((double) tileLocation.Y * 11.0 + (double) tileLocation.X * 7.0) % 10.0 - 5.0) + 32.0));
      this.layerDepth = (float) (((double) tileLocation.Y * 64.0 + 32.0 + (((double) tileLocation.Y * 11.0 + (double) tileLocation.X * 7.0) % 10.0 - 5.0)) / 10000.0);
      this.sourceRect = new Rectangle((int) ((double) tileLocation.X * 51.0 + (double) tileLocation.Y * 77.0) % 3 * 16 /*0x10*/, 128 /*0x80*/ + result * 16 /*0x10*/, 16 /*0x10*/, 16 /*0x10*/);
    }
    else
    {
      this.drawPosition = new Vector2((float) ((double) tileLocation.X * 64.0 + (!this.shouldDrawDarkWhenWatered() || this.currentPhase.Value >= this.phaseDays.Count - 1 ? 0.0 : ((double) tileLocation.X * 11.0 + (double) tileLocation.Y * 7.0) % 10.0 - 5.0) + 32.0), (float) ((double) tileLocation.Y * 64.0 + (this.raisedSeeds.Value || this.currentPhase.Value >= this.phaseDays.Count - 1 ? 0.0 : ((double) tileLocation.Y * 11.0 + (double) tileLocation.X * 7.0) % 10.0 - 5.0) + 32.0));
      this.layerDepth = (float) (((double) tileLocation.Y * 64.0 + 32.0 + (!this.shouldDrawDarkWhenWatered() || this.currentPhase.Value >= this.phaseDays.Count - 1 ? 0.0 : ((double) tileLocation.Y * 11.0 + (double) tileLocation.X * 7.0) % 10.0 - 5.0)) / 10000.0 / (this.currentPhase.Value != 0 || !this.shouldDrawDarkWhenWatered() ? 1.0 : 2.0));
      this.sourceRect = this.getSourceRect((int) tileLocation.X * 7 + (int) tileLocation.Y * 11);
      this.coloredSourceRect = new Rectangle((this.fullyGrown.Value ? (this.dayOfCurrentPhase.Value <= 0 ? 6 : 7) : this.currentPhase.Value + 1 + 1) * 16 /*0x10*/ + (this.rowInSpriteSheet.Value % 2 != 0 ? 128 /*0x80*/ : 0), this.rowInSpriteSheet.Value / 2 * 16 /*0x10*/ * 2, 16 /*0x10*/, 32 /*0x20*/);
      this.coloredLayerDepth = (float) (((double) tileLocation.Y * 64.0 + 32.0 + (((double) tileLocation.Y * 11.0 + (double) tileLocation.X * 7.0) % 10.0 - 5.0)) / 10000.0 / (this.currentPhase.Value != 0 || !this.shouldDrawDarkWhenWatered() ? 1.0 : 2.0));
    }
    this.tilePosition = tileLocation;
  }

  public virtual void draw(SpriteBatch b, Vector2 tileLocation, Color toTint, float rotation)
  {
    Vector2 local = Game1.GlobalToLocal(Game1.viewport, this.drawPosition);
    if (this.forageCrop.Value)
    {
      if (this.whichForageCrop.Value == "2")
        b.Draw(Game1.mouseCursors, Game1.GlobalToLocal(Game1.viewport, new Vector2((float) ((double) tileLocation.X * 64.0 + (((double) tileLocation.X * 11.0 + (double) tileLocation.Y * 7.0) % 10.0 - 5.0) + 32.0), (float) ((double) tileLocation.Y * 64.0 + (((double) tileLocation.Y * 11.0 + (double) tileLocation.X * 7.0) % 10.0 - 5.0) + 64.0))), new Rectangle?(new Rectangle(128 /*0x80*/ + (int) ((Game1.currentGameTime.TotalGameTime.TotalMilliseconds + ((double) tileLocation.X * 111.0 + (double) tileLocation.Y * 77.0)) % 800.0 / 200.0) * 16 /*0x10*/, 128 /*0x80*/, 16 /*0x10*/, 16 /*0x10*/)), Color.White, rotation, new Vector2(8f, 16f), 4f, SpriteEffects.None, (float) (((double) tileLocation.Y * 64.0 + 32.0 + (((double) tileLocation.Y * 11.0 + (double) tileLocation.X * 7.0) % 10.0 - 5.0)) / 10000.0));
      else
        b.Draw(Game1.mouseCursors, local, new Rectangle?(this.sourceRect), Color.White, 0.0f, Crop.smallestTileSizeOrigin, 4f, SpriteEffects.None, this.layerDepth);
    }
    else if (this.IsErrorCrop())
    {
      ParsedItemData dataOrErrorItem = ItemRegistry.GetDataOrErrorItem("(O)" + this.indexOfHarvest.Value);
      b.Draw(dataOrErrorItem.GetTexture(), local, new Rectangle?(dataOrErrorItem.GetSourceRect()), toTint, rotation, new Vector2(8f, 8f), 4f, SpriteEffects.None, this.layerDepth);
    }
    else
    {
      SpriteEffects effects = this.flip.Value ? SpriteEffects.FlipHorizontally : SpriteEffects.None;
      b.Draw(this.DrawnCropTexture, local, new Rectangle?(this.sourceRect), toTint, rotation, Crop.origin, 4f, effects, this.layerDepth);
      Color color = this.tintColor.Value;
      if (color.Equals(Color.White) || this.currentPhase.Value != this.phaseDays.Count - 1 || this.dead.Value)
        return;
      b.Draw(this.DrawnCropTexture, local, new Rectangle?(this.coloredSourceRect), color, rotation, Crop.origin, 4f, effects, this.coloredLayerDepth);
    }
  }

  public virtual void drawInMenu(
    SpriteBatch b,
    Vector2 screenPosition,
    Color toTint,
    float rotation,
    float scale,
    float layerDepth)
  {
    if (this.IsErrorCrop())
    {
      ParsedItemData dataOrErrorItem = ItemRegistry.GetDataOrErrorItem("(O)" + this.indexOfHarvest.Value);
      b.Draw(dataOrErrorItem.GetTexture(), screenPosition, new Rectangle?(dataOrErrorItem.GetSourceRect()), toTint, rotation, new Vector2(32f, 32f), scale, this.flip.Value ? SpriteEffects.FlipHorizontally : SpriteEffects.None, layerDepth);
    }
    else
      b.Draw(this.DrawnCropTexture, screenPosition, new Rectangle?(this.getSourceRect(0)), toTint, rotation, new Vector2(32f, 96f), scale, this.flip.Value ? SpriteEffects.FlipHorizontally : SpriteEffects.None, layerDepth);
  }

  public virtual void drawWithOffset(
    SpriteBatch b,
    Vector2 tileLocation,
    Color toTint,
    float rotation,
    Vector2 offset)
  {
    if (this.IsErrorCrop())
    {
      ParsedItemData dataOrErrorItem = ItemRegistry.GetDataOrErrorItem("(O)" + this.indexOfHarvest.Value);
      b.Draw(dataOrErrorItem.GetTexture(), Game1.GlobalToLocal(Game1.viewport, offset + new Vector2(tileLocation.X * 64f, tileLocation.Y * 64f)), new Rectangle?(dataOrErrorItem.GetSourceRect()), toTint, rotation, new Vector2(8f, 8f), 4f, this.flip.Value ? SpriteEffects.FlipHorizontally : SpriteEffects.None, (float) (((double) tileLocation.Y + 0.6600000262260437) * 64.0 / 10000.0 + (double) tileLocation.X * 9.9999997473787516E-06));
    }
    else if (this.forageCrop.Value)
    {
      b.Draw(Game1.mouseCursors, Game1.GlobalToLocal(Game1.viewport, offset + new Vector2(tileLocation.X * 64f, tileLocation.Y * 64f)), new Rectangle?(this.sourceRect), Color.White, 0.0f, new Vector2(8f, 8f), 4f, SpriteEffects.None, (float) (((double) tileLocation.Y + 0.6600000262260437) * 64.0 / 10000.0 + (double) tileLocation.X * 9.9999997473787516E-06));
    }
    else
    {
      b.Draw(this.DrawnCropTexture, Game1.GlobalToLocal(Game1.viewport, offset + new Vector2(tileLocation.X * 64f, tileLocation.Y * 64f)), new Rectangle?(this.sourceRect), toTint, rotation, new Vector2(8f, 24f), 4f, this.flip.Value ? SpriteEffects.FlipHorizontally : SpriteEffects.None, (float) (((double) tileLocation.Y + 0.6600000262260437) * 64.0 / 10000.0 + (double) tileLocation.X * 9.9999997473787516E-06));
      if (this.tintColor.Equals(Color.White) || this.currentPhase.Value != this.phaseDays.Count - 1 || this.dead.Value)
        return;
      b.Draw(this.DrawnCropTexture, Game1.GlobalToLocal(Game1.viewport, offset + new Vector2(tileLocation.X * 64f, tileLocation.Y * 64f)), new Rectangle?(this.coloredSourceRect), this.tintColor.Value, rotation, new Vector2(8f, 24f), 4f, this.flip.Value ? SpriteEffects.FlipHorizontally : SpriteEffects.None, (float) (((double) tileLocation.Y + 0.67000001668930054) * 64.0 / 10000.0 + (double) tileLocation.X * 9.9999997473787516E-06));
    }
  }
}
