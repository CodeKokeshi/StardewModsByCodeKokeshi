// Decompiled with JetBrains decompiler
// Type: StardewValley.Farm
// Assembly: Stardew Valley, Version=1.6.15.24356, Culture=neutral, PublicKeyToken=null
// MVID: AA1F513A-94F0-4EF6-A35F-2D5B4A3721BF
// Assembly location: D:\SteamLibrary\steamapps\common\Stardew Valley\Stardew Valley.dll
// XML documentation location: D:\SteamLibrary\steamapps\common\Stardew Valley\Stardew Valley.xml

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Netcode;
using Netcode.Validation;
using StardewValley.BellsAndWhistles;
using StardewValley.Buildings;
using StardewValley.Characters;
using StardewValley.Extensions;
using StardewValley.GameData.Buildings;
using StardewValley.GameData.Characters;
using StardewValley.Inventories;
using StardewValley.ItemTypeDefinitions;
using StardewValley.Locations;
using StardewValley.Menus;
using StardewValley.Monsters;
using StardewValley.Network;
using StardewValley.Objects;
using StardewValley.TerrainFeatures;
using System;
using System.Collections.Generic;
using System.IO;
using System.Xml.Serialization;
using xTile;
using xTile.Dimensions;
using xTile.Layers;
using xTile.Tiles;

#nullable disable
namespace StardewValley;

public class Farm : GameLocation
{
  [XmlIgnore]
  [NonInstancedStatic]
  public static Texture2D houseTextures = Game1.content.Load<Texture2D>("Buildings\\houses");
  /// <summary>Obsolete. This is only kept to preserve data from old save files. Use <see cref="F:StardewValley.Buildings.Building.netBuildingPaintColor" /> instead.</summary>
  [NotNetField]
  public NetRef<BuildingPaintColor> housePaintColor = new NetRef<BuildingPaintColor>();
  public const int default_layout = 0;
  public const int riverlands_layout = 1;
  public const int forest_layout = 2;
  public const int mountains_layout = 3;
  public const int combat_layout = 4;
  public const int fourCorners_layout = 5;
  public const int beach_layout = 6;
  public const int mod_layout = 7;
  public const int layout_max = 7;
  [XmlElement("grandpaScore")]
  public readonly NetInt grandpaScore = new NetInt(0);
  [XmlElement("farmCaveReady")]
  public NetBool farmCaveReady = new NetBool(false);
  private TemporaryAnimatedSprite shippingBinLid;
  private Microsoft.Xna.Framework.Rectangle shippingBinLidOpenArea = new Microsoft.Xna.Framework.Rectangle(4480, 832, 256 /*0x0100*/, 192 /*0xC0*/);
  [XmlIgnore]
  private readonly NetRef<Inventory> sharedShippingBin = new NetRef<Inventory>(new Inventory());
  [XmlIgnore]
  public Item lastItemShipped;
  public bool hasSeenGrandpaNote;
  protected Dictionary<string, Dictionary<Point, Tile>> _baseSpouseAreaTiles = new Dictionary<string, Dictionary<Point, Tile>>();
  [XmlIgnore]
  public bool hasMatureFairyRoseTonight;
  [XmlElement("greenhouseUnlocked")]
  public readonly NetBool greenhouseUnlocked = new NetBool();
  [XmlElement("greenhouseMoved")]
  public readonly NetBool greenhouseMoved = new NetBool();
  private readonly NetEvent1Field<Vector2, NetVector2> spawnCrowEvent = new NetEvent1Field<Vector2, NetVector2>();
  public readonly NetEvent1<Farm.LightningStrikeEvent> lightningStrikeEvent = new NetEvent1<Farm.LightningStrikeEvent>();
  [XmlIgnore]
  public Point? mapGrandpaShrinePosition;
  [XmlIgnore]
  public Point? mapMainMailboxPosition;
  [XmlIgnore]
  public Point? mainFarmhouseEntry;
  [XmlIgnore]
  public Vector2? mapSpouseAreaCorner;
  [XmlIgnore]
  public Vector2? mapShippingBinPosition;
  protected Microsoft.Xna.Framework.Rectangle? _mountainForageRectangle;
  protected bool? _shouldSpawnForestFarmForage;
  protected bool? _shouldSpawnBeachFarmForage;
  protected bool? _oceanCrabPotOverride;
  protected string _fishLocationOverride;
  protected float _fishChanceOverride;
  public Point spousePatioSpot;
  public const int numCropsForCrow = 16 /*0x10*/;

  public Farm()
  {
  }

  public Farm(string mapPath, string name)
    : base(mapPath, name)
  {
    this.isAlwaysActive.Value = true;
  }

  public override bool IsBuildableLocation() => true;

  /// <inheritdoc />
  public override void AddDefaultBuildings(bool load = true)
  {
    this.AddDefaultBuilding("Farmhouse", this.GetStarterFarmhouseLocation(), load);
    this.AddDefaultBuilding("Greenhouse", this.GetGreenhouseStartLocation(), load);
    this.AddDefaultBuilding("Shipping Bin", this.GetStarterShippingBinLocation(), load);
    this.AddDefaultBuilding("Pet Bowl", this.GetStarterPetBowlLocation(), load);
    this.BuildStartingCabins();
  }

  public override string GetDisplayName()
  {
    return base.GetDisplayName() ?? Game1.content.LoadString("Strings\\StringsFromCSFiles:MapPage.cs.11064", (object) Game1.player.farmName.Value);
  }

  /// <summary>Get the tile position at which the shipping bin should be created when it's missing.</summary>
  public virtual Vector2 GetStarterShippingBinLocation()
  {
    if (!this.mapShippingBinPosition.HasValue)
    {
      Vector2 parsed;
      if (!this.TryGetMapPropertyAs("ShippingBinLocation", out parsed))
        parsed = new Vector2(71f, 14f);
      this.mapShippingBinPosition = new Vector2?(parsed);
    }
    return this.mapShippingBinPosition.Value;
  }

  /// <summary>Get the tile position at which the pet bowl should be created when it's missing.</summary>
  public virtual Vector2 GetStarterPetBowlLocation()
  {
    Vector2 parsed;
    return !this.TryGetMapPropertyAs("PetBowlLocation", out parsed) ? new Vector2(53f, 7f) : parsed;
  }

  /// <summary>Get the tile position at which the farmhouse should be created when it's missing.</summary>
  /// <remarks>See also <see cref="M:StardewValley.Farm.GetMainFarmHouseEntry" />.</remarks>
  public virtual Vector2 GetStarterFarmhouseLocation()
  {
    Point mainFarmHouseEntry = this.GetMainFarmHouseEntry();
    return new Vector2((float) (mainFarmHouseEntry.X - 5), (float) (mainFarmHouseEntry.Y - 3));
  }

  /// <summary>Get the tile position at which the greenhouse should be created when it's missing.</summary>
  public virtual Vector2 GetGreenhouseStartLocation()
  {
    Vector2 parsed;
    if (this.TryGetMapPropertyAs("GreenhouseLocation", out parsed))
      return parsed;
    switch (Game1.whichFarm)
    {
      case 5:
        return new Vector2(36f, 29f);
      case 6:
        return new Vector2(14f, 14f);
      default:
        return new Vector2(25f, 10f);
    }
  }

  protected override void initNetFields()
  {
    base.initNetFields();
    this.NetFields.AddField((INetSerializable) this.sharedShippingBin, "sharedShippingBin").AddField((INetSerializable) this.spawnCrowEvent, "spawnCrowEvent").AddField((INetSerializable) this.lightningStrikeEvent, "lightningStrikeEvent").AddField((INetSerializable) this.grandpaScore, "grandpaScore").AddField((INetSerializable) this.greenhouseUnlocked, "greenhouseUnlocked").AddField((INetSerializable) this.greenhouseMoved, "greenhouseMoved").AddField((INetSerializable) this.farmCaveReady, "farmCaveReady");
    this.spawnCrowEvent.onEvent += new AbstractNetEvent1<Vector2>.Event(this.doSpawnCrow);
    this.lightningStrikeEvent.onEvent += new AbstractNetEvent1<Farm.LightningStrikeEvent>.Event(this.doLightningStrike);
    this.greenhouseMoved.fieldChangeVisibleEvent += (FieldChange<NetBool, bool>) ((field, old_value, new_value) => this.ClearGreenhouseGrassTiles());
  }

  public virtual void ClearGreenhouseGrassTiles()
  {
    if (this.map == null || Game1.gameMode == (byte) 6 || !this.greenhouseMoved.Value)
      return;
    switch (Game1.whichFarm)
    {
      case 0:
      case 3:
      case 4:
        this.ApplyMapOverride("Farm_Greenhouse_Dirt", destination_rect: new Microsoft.Xna.Framework.Rectangle?(new Microsoft.Xna.Framework.Rectangle((int) this.GetGreenhouseStartLocation().X, (int) this.GetGreenhouseStartLocation().Y, 9, 6)));
        break;
      case 5:
        this.ApplyMapOverride("Farm_Greenhouse_Dirt_FourCorners", destination_rect: new Microsoft.Xna.Framework.Rectangle?(new Microsoft.Xna.Framework.Rectangle((int) this.GetGreenhouseStartLocation().X, (int) this.GetGreenhouseStartLocation().Y, 9, 6)));
        break;
    }
  }

  public static string getMapNameFromTypeInt(int type)
  {
    switch (type)
    {
      case 0:
        return nameof (Farm);
      case 1:
        return "Farm_Fishing";
      case 2:
        return "Farm_Foraging";
      case 3:
        return "Farm_Mining";
      case 4:
        return "Farm_Combat";
      case 5:
        return "Farm_FourCorners";
      case 6:
        return "Farm_Island";
      case 7:
        if (Game1.whichModFarm != null)
          return Game1.whichModFarm.MapName;
        break;
    }
    return nameof (Farm);
  }

  public void onNewGame()
  {
    if (Game1.whichFarm == 3 || this.ShouldSpawnMountainOres())
    {
      for (int index = 0; index < 28; ++index)
        this.doDailyMountainFarmUpdate();
    }
    else if (Game1.whichFarm == 5)
    {
      for (int index = 0; index < 10; ++index)
        this.doDailyMountainFarmUpdate();
    }
    else
    {
      if (!(Game1.GetFarmTypeID() == "MeadowlandsFarm"))
        return;
      for (int x = 47; x < 63 /*0x3F*/; ++x)
        this.objects.Add(new Vector2((float) x, 20f), (Object) new Fence(new Vector2((float) x, 20f), "322", false));
      for (int y = 16 /*0x10*/; y < 20; ++y)
        this.objects.Add(new Vector2(47f, (float) y), (Object) new Fence(new Vector2(47f, (float) y), "322", false));
      for (int y = 7; y < 20; ++y)
        this.objects.Add(new Vector2(62f, (float) y), (Object) new Fence(new Vector2(62f, (float) y), "322", y == 13));
      Building building = new Building("Coop", new Vector2(54f, 9f));
      building.FinishConstruction(true);
      building.LoadFromBuildingData(building.GetData(), forConstruction: true);
      building.load();
      FarmAnimal animal1 = new FarmAnimal("White Chicken", Game1.multiplayer.getNewID(), Game1.player.UniqueMultiplayerID);
      FarmAnimal animal2 = new FarmAnimal("Brown Chicken", Game1.multiplayer.getNewID(), Game1.player.UniqueMultiplayerID);
      string[] strArray = Game1.content.LoadString("Strings\\1_6_Strings:StarterChicken_Names").Split('|');
      string str = strArray[Game1.random.Next(strArray.Length)];
      animal1.Name = str.Split(',')[0].Trim();
      animal2.Name = str.Split(',')[1].Trim();
      (building.GetIndoors() as AnimalHouse).adoptAnimal(animal1);
      (building.GetIndoors() as AnimalHouse).adoptAnimal(animal2);
      this.buildings.Add(building);
    }
  }

  public override void DayUpdate(int dayOfMonth)
  {
    base.DayUpdate(dayOfMonth);
    this.UpdatePatio();
    for (int index = this.characters.Count - 1; index >= 0; --index)
    {
      if (this.characters[index] is Pet character && (this.hasTileAt(character.TilePoint, "Buildings") || this.hasTileAt(character.TilePoint.X + 1, character.TilePoint.Y, "Buildings") || !this.CanSpawnCharacterHere(character.Tile) || !this.CanSpawnCharacterHere(new Vector2((float) (character.TilePoint.X + 1), (float) character.TilePoint.Y))))
        character.WarpToPetBowl();
    }
    this.lastItemShipped = (Item) null;
    if (this.characters.Count > 5)
    {
      int num = this.characters.RemoveWhere((Func<NPC, bool>) (npc => npc is GreenSlime && Game1.random.NextDouble() < 0.035));
      if (num > 0)
        Game1.multiplayer.broadcastGlobalMessage(num == 1 ? "Strings\\Locations:Farm_1SlimeEscaped" : "Strings\\Locations:Farm_NSlimesEscaped", false, (GameLocation) null, num.ToString() ?? "");
    }
    Vector2 key;
    if (Game1.whichFarm == 5)
    {
      if (this.CanItemBePlacedHere(new Vector2(5f, 32f), ignorePassables: CollisionMask.None) && this.CanItemBePlacedHere(new Vector2(6f, 32f), ignorePassables: CollisionMask.None) && this.CanItemBePlacedHere(new Vector2(6f, 33f), ignorePassables: CollisionMask.None) && this.CanItemBePlacedHere(new Vector2(5f, 33f), ignorePassables: CollisionMask.None))
        this.resourceClumps.Add(new ResourceClump(600, 2, 2, new Vector2(5f, 32f)));
      if (this.objects.Length > 0)
      {
        for (int index = 0; index < 6; ++index)
        {
          Object @object;
          if (Utility.TryGetRandom(this.objects, out key, out @object) && @object.IsWeeds() && (double) @object.tileLocation.X < 36.0 && (double) @object.tileLocation.Y < 34.0)
            @object.SetIdAndSprite(792 + Game1.seasonIndex);
        }
      }
    }
    if (this.ShouldSpawnBeachFarmForage())
    {
      while (Game1.random.NextDouble() < 0.9)
      {
        Vector2 randomTile = this.getRandomTile();
        if (this.CanItemBePlacedHere(randomTile) && !this.hasTileAt((int) randomTile.X, (int) randomTile.Y, "AlwaysFront"))
        {
          string str = (string) null;
          if (this.doesTileHavePropertyNoNull((int) randomTile.X, (int) randomTile.Y, "BeachSpawn", "Back") != "")
          {
            str = "372";
            int num = (int) Game1.stats.Increment("beachFarmSpawns");
            switch (Game1.random.Next(6))
            {
              case 0:
                str = "393";
                break;
              case 1:
                str = "719";
                break;
              case 2:
                str = "718";
                break;
              case 3:
                str = "723";
                break;
              case 4:
              case 5:
                str = "152";
                break;
            }
            if (Game1.stats.DaysPlayed > 1U)
            {
              if (Game1.random.NextDouble() < 0.15 || Game1.stats.Get("beachFarmSpawns") % 4U == 0U)
              {
                string itemId = Game1.random.Next(922, 925).ToString();
                this.objects.Add(randomTile, new Object(itemId, 1)
                {
                  Fragility = 2,
                  MinutesUntilReady = 3
                });
                str = (string) null;
              }
              else if (Game1.random.NextDouble() < 0.1)
                str = "397";
              else if (Game1.random.NextDouble() < 0.05)
                str = "392";
              else if (Game1.random.NextDouble() < 0.02)
                str = "394";
            }
          }
          else if (Game1.season != Season.Winter && new Microsoft.Xna.Framework.Rectangle(20, 66, 33, 18).Contains((int) randomTile.X, (int) randomTile.Y) && this.doesTileHavePropertyNoNull((int) randomTile.X, (int) randomTile.Y, "Type", "Back") == "Grass")
            str = Utility.getRandomBasicSeasonalForageItem(Game1.season, (int) Game1.stats.DaysPlayed);
          if (str != null)
          {
            Object @object = ItemRegistry.Create<Object>("(O)" + str);
            @object.CanBeSetDown = false;
            @object.IsSpawnedObject = true;
            this.dropObject(@object, randomTile * 64f, Game1.viewport, true);
          }
        }
      }
    }
    if (Game1.whichFarm == 2)
    {
      for (int x = 0; x < 20; ++x)
      {
        for (int y = 0; y < this.map.Layers[0].LayerHeight; ++y)
        {
          if (this.getTileIndexAt(x, y, "Paths") == 21 && this.CanItemBePlacedHere(new Vector2((float) x, (float) y), ignorePassables: CollisionMask.None) && this.CanItemBePlacedHere(new Vector2((float) (x + 1), (float) y), ignorePassables: CollisionMask.None) && this.CanItemBePlacedHere(new Vector2((float) (x + 1), (float) (y + 1)), ignorePassables: CollisionMask.None) && this.CanItemBePlacedHere(new Vector2((float) x, (float) (y + 1)), ignorePassables: CollisionMask.None))
            this.resourceClumps.Add(new ResourceClump(600, 2, 2, new Vector2((float) x, (float) y)));
        }
      }
    }
    if (this.ShouldSpawnForestFarmForage() && !Game1.IsWinter)
    {
      while (Game1.random.NextDouble() < 0.75)
      {
        Vector2 tile = new Vector2((float) Game1.random.Next(18), (float) Game1.random.Next(this.map.Layers[0].LayerHeight));
        if (Game1.random.NextBool() || Game1.whichFarm != 2)
          tile = this.getRandomTile();
        if (this.CanItemBePlacedHere(tile, ignorePassables: CollisionMask.None) && !this.hasTileAt((int) tile.X, (int) tile.Y, "AlwaysFront") && (Game1.whichFarm == 2 && (double) tile.X < 18.0 || this.doesTileHavePropertyNoNull((int) tile.X, (int) tile.Y, "Type", "Back").Equals("Grass")))
        {
          string itemId;
          switch (Game1.season)
          {
            case Season.Spring:
              switch (Game1.random.Next(4))
              {
                case 0:
                  itemId = "(O)" + 16 /*0x10*/.ToString();
                  break;
                case 1:
                  itemId = "(O)" + 22.ToString();
                  break;
                case 2:
                  itemId = "(O)" + 20.ToString();
                  break;
                default:
                  itemId = "(O)257";
                  break;
              }
              break;
            case Season.Summer:
              switch (Game1.random.Next(4))
              {
                case 0:
                  itemId = "(O)402";
                  break;
                case 1:
                  itemId = "(O)396";
                  break;
                case 2:
                  itemId = "(O)398";
                  break;
                default:
                  itemId = "(O)404";
                  break;
              }
              break;
            case Season.Fall:
              switch (Game1.random.Next(4))
              {
                case 0:
                  itemId = "(O)281";
                  break;
                case 1:
                  itemId = "(O)420";
                  break;
                case 2:
                  itemId = "(O)422";
                  break;
                default:
                  itemId = "(O)404";
                  break;
              }
              break;
            default:
              itemId = "(O)792";
              break;
          }
          Object @object = ItemRegistry.Create<Object>(itemId);
          @object.CanBeSetDown = false;
          @object.IsSpawnedObject = true;
          this.dropObject(@object, tile * 64f, Game1.viewport, true);
        }
      }
      if (this.objects.Length > 0)
      {
        for (int index = 0; index < 6; ++index)
        {
          Object @object;
          if (Utility.TryGetRandom(this.objects, out key, out @object) && @object.IsWeeds())
            @object.SetIdAndSprite(792 + Game1.seasonIndex);
        }
      }
    }
    if (Game1.whichFarm == 3 || Game1.whichFarm == 5 || this.ShouldSpawnMountainOres())
      this.doDailyMountainFarmUpdate();
    if (this.terrainFeatures.Length > 0 && Game1.season == Season.Fall && Game1.dayOfMonth > 1 && Game1.random.NextDouble() < 0.05)
    {
      for (int index = 0; index < 10; ++index)
      {
        TerrainFeature terrainFeature;
        if (Utility.TryGetRandom<Vector2, TerrainFeature, NetRef<TerrainFeature>, SerializableDictionary<Vector2, TerrainFeature>, NetVector2Dictionary<TerrainFeature, NetRef<TerrainFeature>>>((NetDictionary<Vector2, TerrainFeature, NetRef<TerrainFeature>, SerializableDictionary<Vector2, TerrainFeature>, NetVector2Dictionary<TerrainFeature, NetRef<TerrainFeature>>>) this.terrainFeatures, out Vector2 _, out terrainFeature) && terrainFeature is Tree tree && tree.growthStage.Value >= 5 && !tree.tapped.Value && !tree.isTemporaryGreenRainTree.Value)
        {
          tree.treeType.Value = "7";
          tree.loadSprite();
          break;
        }
      }
    }
    this.addCrows();
    int numDebris;
    switch (Game1.season)
    {
      case Season.Summer:
        numDebris = 30;
        break;
      case Season.Winter:
label_90:
        this.spawnWeeds(false);
        this.HandleGrassGrowth(dayOfMonth);
        return;
      default:
        numDebris = 20;
        break;
    }
    this.spawnWeedsAndStones(numDebris);
    goto label_90;
  }

  public void doDailyMountainFarmUpdate()
  {
    double num1 = 1.0;
    while (Game1.random.NextDouble() < num1)
    {
      Vector2 vector2 = this.ShouldSpawnMountainOres() ? Utility.getRandomPositionInThisRectangle(this._mountainForageRectangle.Value, Game1.random) : (Game1.whichFarm == 5 ? Utility.getRandomPositionInThisRectangle(new Microsoft.Xna.Framework.Rectangle(51, 67, 11, 3), Game1.random) : Utility.getRandomPositionInThisRectangle(new Microsoft.Xna.Framework.Rectangle(5, 37, 22, 8), Game1.random));
      if (this.doesTileHavePropertyNoNull((int) vector2.X, (int) vector2.Y, "Type", "Back").Equals("Dirt") && this.CanItemBePlacedHere(vector2, ignorePassables: CollisionMask.None))
      {
        string itemId = "668";
        int num2 = 2;
        if (Game1.random.NextDouble() < 0.15)
        {
          this.objects.Add(vector2, ItemRegistry.Create<Object>("(O)590"));
          continue;
        }
        if (Game1.random.NextBool())
          itemId = "670";
        if (Game1.random.NextDouble() < 0.1)
        {
          if (Game1.player.MiningLevel >= 8 && Game1.random.NextDouble() < 0.33)
          {
            itemId = "77";
            num2 = 7;
          }
          else if (Game1.player.MiningLevel >= 5 && Game1.random.NextBool())
          {
            itemId = "76";
            num2 = 5;
          }
          else
          {
            itemId = "75";
            num2 = 3;
          }
        }
        if (Game1.random.NextDouble() < 0.21)
        {
          itemId = "751";
          num2 = 3;
        }
        if (Game1.player.MiningLevel >= 4 && Game1.random.NextDouble() < 0.15)
        {
          itemId = "290";
          num2 = 4;
        }
        if (Game1.player.MiningLevel >= 7 && Game1.random.NextDouble() < 0.1)
        {
          itemId = "764";
          num2 = 8;
        }
        if (Game1.player.MiningLevel >= 10 && Game1.random.NextDouble() < 0.01)
        {
          itemId = "765";
          num2 = 16 /*0x10*/;
        }
        this.objects.Add(vector2, new Object(itemId, 10)
        {
          MinutesUntilReady = num2
        });
      }
      num1 *= 0.75;
    }
  }

  /// <inheritdoc />
  public override bool catchOceanCrabPotFishFromThisSpot(int x, int y)
  {
    if (this.map != null)
    {
      if (!this._oceanCrabPotOverride.HasValue)
        this._oceanCrabPotOverride = new bool?(this.map.Properties.ContainsKey("FarmOceanCrabPotOverride"));
      if (this._oceanCrabPotOverride.Value)
        return true;
    }
    return base.catchOceanCrabPotFishFromThisSpot(x, y);
  }

  public void addCrows()
  {
    int num1 = 0;
    foreach (KeyValuePair<Vector2, TerrainFeature> pair in this.terrainFeatures.Pairs)
    {
      if (pair.Value is HoeDirt hoeDirt && hoeDirt.crop != null)
        ++num1;
    }
    List<Vector2> vector2List = new List<Vector2>();
    foreach (KeyValuePair<Vector2, Object> pair in this.objects.Pairs)
    {
      if (pair.Value.IsScarecrow())
        vector2List.Add(pair.Key);
    }
    int num2 = Math.Min(4, num1 / 16 /*0x10*/);
    for (int index1 = 0; index1 < num2; ++index1)
    {
      if (Game1.random.NextDouble() < 0.3)
      {
        for (int index2 = 0; index2 < 10; ++index2)
        {
          Vector2 key1;
          TerrainFeature terrainFeature;
          if (Utility.TryGetRandom<Vector2, TerrainFeature, NetRef<TerrainFeature>, SerializableDictionary<Vector2, TerrainFeature>, NetVector2Dictionary<TerrainFeature, NetRef<TerrainFeature>>>((NetDictionary<Vector2, TerrainFeature, NetRef<TerrainFeature>, SerializableDictionary<Vector2, TerrainFeature>, NetVector2Dictionary<TerrainFeature, NetRef<TerrainFeature>>>) this.terrainFeatures, out key1, out terrainFeature) && terrainFeature is HoeDirt hoeDirt)
          {
            Crop crop = hoeDirt.crop;
            if ((crop != null ? (crop.currentPhase.Value > 1 ? 1 : 0) : 0) != 0)
            {
              bool flag = false;
              foreach (Vector2 key2 in vector2List)
              {
                int radiusForScarecrow = this.objects[key2].GetRadiusForScarecrow();
                if ((double) Vector2.Distance(key2, key1) < (double) radiusForScarecrow)
                {
                  flag = true;
                  ++this.objects[key2].SpecialVariable;
                  break;
                }
              }
              if (!flag)
              {
                hoeDirt.destroyCrop(false);
                this.spawnCrowEvent.Fire(key1);
                break;
              }
              break;
            }
          }
        }
      }
    }
  }

  private void doSpawnCrow(Vector2 v)
  {
    if (this.critters == null && this.isOutdoors.Value)
      this.critters = new List<Critter>();
    this.critters.Add((Critter) new Crow((int) v.X, (int) v.Y));
  }

  public static Point getFrontDoorPositionForFarmer(Farmer who)
  {
    Point mainFarmHouseEntry = Game1.getFarm().GetMainFarmHouseEntry();
    --mainFarmHouseEntry.Y;
    return mainFarmHouseEntry;
  }

  public override void performTenMinuteUpdate(int timeOfDay)
  {
    base.performTenMinuteUpdate(timeOfDay);
    if (timeOfDay >= 1300 && Game1.IsMasterGame)
    {
      foreach (NPC npc in new List<Character>((IEnumerable<Character>) this.characters))
      {
        if (npc.isMarried())
          npc.returnHomeFromFarmPosition(this);
      }
    }
    foreach (NPC character in this.characters)
    {
      if (character.getSpouse() == Game1.player)
        character.checkForMarriageDialogue(timeOfDay, (GameLocation) this);
      if (character is Child child)
        child.tenMinuteUpdate();
    }
    if (!Game1.spawnMonstersAtNight || Game1.farmEvent != null || Game1.timeOfDay < 1900 || Game1.random.NextDouble() >= 0.25 - Game1.player.team.AverageDailyLuck() / 2.0)
      return;
    if (Game1.random.NextDouble() < 0.25)
    {
      if (!this.Equals(Game1.currentLocation))
        return;
      this.spawnFlyingMonstersOffScreen();
    }
    else
      this.spawnGroundMonsterOffScreen();
  }

  public void spawnGroundMonsterOffScreen()
  {
    for (int index = 0; index < 15; ++index)
    {
      Vector2 randomTile = this.getRandomTile();
      if (Utility.isOnScreen(Utility.Vector2ToPoint(randomTile), 64 /*0x40*/, (GameLocation) this))
        randomTile.X -= (float) (Game1.viewport.Width / 64 /*0x40*/);
      if (this.CanItemBePlacedHere(randomTile))
      {
        int combatLevel = Game1.player.CombatLevel;
        bool flag;
        if (combatLevel >= 8 && Game1.random.NextDouble() < 0.15)
        {
          NetCollection<NPC> characters = this.characters;
          ShadowBrute shadowBrute = new ShadowBrute(randomTile * 64f);
          shadowBrute.focusedOnFarmers = true;
          shadowBrute.wildernessFarmMonster = true;
          characters.Add((NPC) shadowBrute);
          flag = true;
        }
        else if (Game1.random.NextDouble() < (Game1.whichFarm == 4 ? 0.66 : 0.33))
        {
          NetCollection<NPC> characters = this.characters;
          RockGolem rockGolem = new RockGolem(randomTile * 64f, combatLevel);
          rockGolem.wildernessFarmMonster = true;
          characters.Add((NPC) rockGolem);
          flag = true;
        }
        else
        {
          int mineLevel = 1;
          if (combatLevel >= 10)
            mineLevel = 140;
          else if (combatLevel >= 8)
            mineLevel = 100;
          else if (combatLevel >= 4)
            mineLevel = 41;
          NetCollection<NPC> characters = this.characters;
          GreenSlime greenSlime = new GreenSlime(randomTile * 64f, mineLevel);
          greenSlime.wildernessFarmMonster = true;
          characters.Add((NPC) greenSlime);
          flag = true;
        }
        if (!flag || !Game1.currentLocation.Equals((GameLocation) this))
          break;
        using (IEnumerator<KeyValuePair<Vector2, Object>> enumerator = this.objects.Pairs.GetEnumerator())
        {
          while (enumerator.MoveNext())
          {
            KeyValuePair<Vector2, Object> current = enumerator.Current;
            if (current.Value?.QualifiedItemId == "(BC)83")
            {
              current.Value.shakeTimer = 1000;
              current.Value.showNextIndex.Value = true;
              Game1.currentLightSources.Add(new LightSource(current.Value.GenerateLightSourceId(current.Value.TileLocation), 4, current.Key * 64f + new Vector2(32f, 0.0f), 1f, Color.Cyan * 0.75f, onlyLocation: this.NameOrUniqueName));
            }
          }
          break;
        }
      }
    }
  }

  public void spawnFlyingMonstersOffScreen()
  {
    Vector2 zero = Vector2.Zero;
    switch (Game1.random.Next(4))
    {
      case 0:
        zero.X = (float) Game1.random.Next(this.map.Layers[0].LayerWidth);
        break;
      case 1:
        zero.X = (float) (this.map.Layers[0].LayerWidth - 1);
        zero.Y = (float) Game1.random.Next(this.map.Layers[0].LayerHeight);
        break;
      case 2:
        zero.Y = (float) (this.map.Layers[0].LayerHeight - 1);
        zero.X = (float) Game1.random.Next(this.map.Layers[0].LayerWidth);
        break;
      case 3:
        zero.Y = (float) Game1.random.Next(this.map.Layers[0].LayerHeight);
        break;
    }
    if (Utility.isOnScreen(zero * 64f, 64 /*0x40*/))
      zero.X -= (float) Game1.viewport.Width;
    int combatLevel = Game1.player.CombatLevel;
    bool flag;
    if (combatLevel >= 10 && Game1.random.NextDouble() < 0.01 && Game1.player.Items.ContainsId("(W)4"))
    {
      NetCollection<NPC> characters = this.characters;
      Bat bat = new Bat(zero * 64f, 9999);
      bat.focusedOnFarmers = true;
      bat.wildernessFarmMonster = true;
      characters.Add((NPC) bat);
      flag = true;
    }
    else if (combatLevel >= 10 && Game1.random.NextDouble() < 0.25)
    {
      NetCollection<NPC> characters = this.characters;
      Bat bat = new Bat(zero * 64f, 172);
      bat.focusedOnFarmers = true;
      bat.wildernessFarmMonster = true;
      characters.Add((NPC) bat);
      flag = true;
    }
    else if (combatLevel >= 10 && Game1.random.NextDouble() < 0.25)
    {
      NetCollection<NPC> characters = this.characters;
      Serpent serpent = new Serpent(zero * 64f);
      serpent.focusedOnFarmers = true;
      serpent.wildernessFarmMonster = true;
      characters.Add((NPC) serpent);
      flag = true;
    }
    else if (combatLevel >= 8 && Game1.random.NextBool())
    {
      NetCollection<NPC> characters = this.characters;
      Bat bat = new Bat(zero * 64f, 81);
      bat.focusedOnFarmers = true;
      bat.wildernessFarmMonster = true;
      characters.Add((NPC) bat);
      flag = true;
    }
    else if (combatLevel >= 5 && Game1.random.NextBool())
    {
      NetCollection<NPC> characters = this.characters;
      Bat bat = new Bat(zero * 64f, 41);
      bat.focusedOnFarmers = true;
      bat.wildernessFarmMonster = true;
      characters.Add((NPC) bat);
      flag = true;
    }
    else
    {
      NetCollection<NPC> characters = this.characters;
      Bat bat = new Bat(zero * 64f, 1);
      bat.focusedOnFarmers = true;
      bat.wildernessFarmMonster = true;
      characters.Add((NPC) bat);
      flag = true;
    }
    if (!flag || !Game1.currentLocation.Equals((GameLocation) this))
      return;
    foreach (KeyValuePair<Vector2, Object> pair in this.objects.Pairs)
    {
      if (pair.Value?.QualifiedItemId == "(BC)83")
      {
        pair.Value.shakeTimer = 1000;
        pair.Value.showNextIndex.Value = true;
        Game1.currentLightSources.Add(new LightSource(pair.Value.GenerateLightSourceId(pair.Value.TileLocation), 4, pair.Key * 64f + new Vector2(32f, 0.0f), 1f, Color.Cyan * 0.75f, onlyLocation: this.NameOrUniqueName));
      }
    }
  }

  public virtual void requestGrandpaReevaluation()
  {
    this.grandpaScore.Value = 0;
    if (Game1.IsMasterGame)
    {
      Game1.player.eventsSeen.Remove("558292");
      Game1.player.eventsSeen.Add("321777");
    }
    this.removeTemporarySpritesWithID(6666);
  }

  public override void OnMapLoad(Map map)
  {
    this.CacheOffBasePatioArea();
    base.OnMapLoad(map);
  }

  /// <inheritdoc />
  public override void OnBuildingMoved(Building building)
  {
    base.OnBuildingMoved(building);
    if (building.HasIndoorsName("FarmHouse"))
      this.UnsetFarmhouseValues();
    if (building is GreenhouseBuilding)
      this.greenhouseMoved.Value = true;
    if (!(building.GetIndoors() is FarmHouse indoors) || !indoors.HasNpcSpouseOrRoommate())
      return;
    NPC characterFromName = this.getCharacterFromName(indoors.owner.spouse);
    if (characterFromName == null || characterFromName.shouldPlaySpousePatioAnimation.Value)
      return;
    Game1.player.team.requestNPCGoHome.Fire(characterFromName.Name);
  }

  /// <inheritdoc />
  public override bool ShouldExcludeFromNpcPathfinding() => true;

  public override bool checkAction(Location tileLocation, xTile.Dimensions.Rectangle viewport, Farmer who)
  {
    Point grandpaShrinePosition = this.GetGrandpaShrinePosition();
    if (tileLocation.X >= grandpaShrinePosition.X - 1 && tileLocation.X <= grandpaShrinePosition.X + 1 && tileLocation.Y == grandpaShrinePosition.Y)
    {
      if (!this.hasSeenGrandpaNote)
      {
        Game1.addMail("hasSeenGrandpaNote", true);
        this.hasSeenGrandpaNote = true;
        Game1.activeClickableMenu = (IClickableMenu) new LetterViewerMenu(Game1.content.LoadString("Strings\\Locations:Farm_GrandpaNote", (object) Game1.player.Name).Replace('\n', '^'));
        return true;
      }
      if (Game1.year >= 3 && this.grandpaScore.Value > 0 && this.grandpaScore.Value < 4)
      {
        if (who.ActiveObject?.QualifiedItemId == "(O)72" && this.grandpaScore.Value < 4)
        {
          who.reduceActiveItemByOne();
          this.playSound("stoneStep");
          this.playSound("fireball");
          DelayedAction.playSoundAfterDelay("yoba", 800, (GameLocation) this);
          DelayedAction.showDialogueAfterDelay(Game1.content.LoadString("Strings\\Locations:Farm_GrandpaShrine_PlaceDiamond"), 1200);
          Game1.multiplayer.broadcastGrandpaReevaluation();
          Game1.player.freezePause = 1200;
          return true;
        }
        if (who.ActiveObject?.QualifiedItemId != "(O)72")
        {
          Game1.drawObjectDialogue(Game1.content.LoadString("Strings\\Locations:Farm_GrandpaShrine_DiamondSlot"));
          return true;
        }
      }
      else
      {
        if (this.grandpaScore.Value >= 4 && !Utility.doesItemExistAnywhere("(BC)160"))
        {
          who.addItemByMenuIfNecessaryElseHoldUp(ItemRegistry.Create("(BC)160"), new ItemGrabMenu.behaviorOnItemSelect(this.grandpaStatueCallback));
          return true;
        }
        if (this.grandpaScore.Value == 0 && Game1.year >= 3)
        {
          Game1.player.eventsSeen.Remove("558292");
          Game1.player.eventsSeen.Add("321777");
        }
      }
    }
    return base.checkAction(tileLocation, viewport, who);
  }

  public void grandpaStatueCallback(Item item, Farmer who)
  {
    if (!(item is Object @object) || !(@object.QualifiedItemId == "(BC)160") || who == null)
      return;
    who.mailReceived.Add("grandpaPerfect");
  }

  public override void TransferDataFromSavedLocation(GameLocation l)
  {
    Farm farm = (Farm) l;
    base.TransferDataFromSavedLocation(l);
    this.housePaintColor.Value = farm.housePaintColor.Value;
    this.farmCaveReady.Value = farm.farmCaveReady.Value;
    if (farm.hasSeenGrandpaNote)
      Game1.addMail("hasSeenGrandpaNote", true);
    this.UnsetFarmhouseValues();
  }

  public IInventory getShippingBin(Farmer who)
  {
    return Game1.player.team.useSeparateWallets.Value ? (IInventory) who.personalShippingBin.Value : (IInventory) this.sharedShippingBin.Value;
  }

  public void shipItem(Item i, Farmer who)
  {
    if (i == null)
      return;
    who.removeItemFromInventory(i);
    this.getShippingBin(who).Add(i);
    this.showShipment(i, false);
    this.lastItemShipped = i;
    if (Game1.player.ActiveItem != null)
      return;
    Game1.player.showNotCarrying();
    Game1.player.Halt();
  }

  public void UnsetFarmhouseValues()
  {
    this.mainFarmhouseEntry = new Point?();
    this.mapMainMailboxPosition = new Point?();
  }

  public void showShipment(Item item, bool playThrowSound = true)
  {
    if (playThrowSound)
      this.localSound("backpackIN");
    DelayedAction.playSoundAfterDelay("Ship", playThrowSound ? 250 : 0);
    int num = Game1.random.Next();
    this.temporarySprites.Add(new TemporaryAnimatedSprite("LooseSprites\\Cursors", new Microsoft.Xna.Framework.Rectangle(524, 218, 34, 22), new Vector2(71f, 13f) * 64f + new Vector2(0.0f, 5f) * 4f, false, 0.0f, Color.White)
    {
      interval = 100f,
      totalNumberOfLoops = 1,
      animationLength = 3,
      pingPong = true,
      scale = 4f,
      layerDepth = 0.09601f,
      id = num,
      extraInfoForEndBehavior = num,
      endFunction = new TemporaryAnimatedSprite.endBehavior(((GameLocation) this).removeTemporarySpritesWithID)
    });
    this.temporarySprites.Add(new TemporaryAnimatedSprite("LooseSprites\\Cursors", new Microsoft.Xna.Framework.Rectangle(524, 230, 34, 10), new Vector2(71f, 13f) * 64f + new Vector2(0.0f, 17f) * 4f, false, 0.0f, Color.White)
    {
      interval = 100f,
      totalNumberOfLoops = 1,
      animationLength = 3,
      pingPong = true,
      scale = 4f,
      layerDepth = 0.0963f,
      id = num,
      extraInfoForEndBehavior = num
    });
    ParsedItemData dataOrErrorItem = ItemRegistry.GetDataOrErrorItem(item.QualifiedItemId);
    ColoredObject coloredObject = item as ColoredObject;
    Vector2 position = new Vector2(71f, 13f) * 64f + new Vector2((float) (8 + Game1.random.Next(6)), 2f) * 4f;
    bool[] flagArray = new bool[2]{ false, true };
    foreach (bool flag in flagArray)
    {
      if (!flag || coloredObject != null && !coloredObject.ColorSameIndexAsParentSheetIndex)
        this.temporarySprites.Add(new TemporaryAnimatedSprite(dataOrErrorItem.TextureName, dataOrErrorItem.GetSourceRect(flag ? 1 : 0), position, false, 0.0f, Color.White)
        {
          interval = 9999f,
          scale = 4f,
          alphaFade = 0.045f,
          layerDepth = 0.096225f,
          motion = new Vector2(0.0f, 0.3f),
          acceleration = new Vector2(0.0f, 0.2f),
          scaleChange = -0.05f,
          color = coloredObject != null ? coloredObject.color.Value : Color.White
        });
    }
  }

  public override Item getFish(
    float millisecondsAfterNibble,
    string bait,
    int waterDepth,
    Farmer who,
    double baitPotency,
    Vector2 bobberTile,
    string location = null)
  {
    if (this._fishLocationOverride == null)
    {
      this._fishLocationOverride = "";
      string[] propertySplitBySpaces = this.GetMapPropertySplitBySpaces("FarmFishLocationOverride");
      if (propertySplitBySpaces.Length != 0)
      {
        string str;
        string error;
        float num;
        if (!ArgUtility.TryGet(propertySplitBySpaces, 0, out str, out error, name: "string targetLocation") || !ArgUtility.TryGetFloat(propertySplitBySpaces, 1, out num, out error, "float chance"))
        {
          this.LogMapPropertyError("FarmFishLocationOverride", propertySplitBySpaces, error);
        }
        else
        {
          this._fishLocationOverride = str;
          this._fishChanceOverride = num;
        }
      }
    }
    return (double) this._fishChanceOverride > 0.0 && Game1.random.NextDouble() < (double) this._fishChanceOverride ? base.getFish(millisecondsAfterNibble, bait, waterDepth, who, baitPotency, bobberTile, this._fishLocationOverride) : base.getFish(millisecondsAfterNibble, bait, waterDepth, who, baitPotency, bobberTile);
  }

  protected override void resetSharedState()
  {
    base.resetSharedState();
    if (!this.greenhouseUnlocked.Value && Utility.doesMasterPlayerHaveMailReceivedButNotMailForTomorrow("ccPantry"))
      this.greenhouseUnlocked.Value = true;
    for (int index = this.characters.Count - 1; index >= 0; --index)
    {
      if (Game1.timeOfDay >= 1300 && this.characters[index].isMarried() && this.characters[index].controller == null)
      {
        this.characters[index].Halt();
        this.characters[index].drawOffset = Vector2.Zero;
        this.characters[index].Sprite.StopAnimation();
        FarmHouse farmHouse = Game1.RequireLocation<FarmHouse>(this.characters[index].getSpouse().homeLocation.Value);
        Game1.warpCharacter(this.characters[index], this.characters[index].getSpouse().homeLocation.Value, farmHouse.getKitchenStandingSpot());
        break;
      }
    }
  }

  public virtual void UpdatePatio()
  {
    if (Game1.MasterPlayer.isMarriedOrRoommates() && Game1.MasterPlayer.spouse != null)
      this.addSpouseOutdoorArea(Game1.MasterPlayer.spouse);
    else
      this.addSpouseOutdoorArea("");
  }

  public override void MakeMapModifications(bool force = false)
  {
    base.MakeMapModifications(force);
    this.ClearGreenhouseGrassTiles();
    this.UpdatePatio();
  }

  protected override void resetLocalState()
  {
    base.resetLocalState();
    this.hasSeenGrandpaNote = Game1.player.hasOrWillReceiveMail("hasSeenGrandpaNote");
    if (Game1.player.mailReceived.Add("button_tut_2"))
      Game1.onScreenMenus.Add((IClickableMenu) new ButtonTutorialMenu(1));
    for (int index = this.characters.Count - 1; index >= 0; --index)
    {
      if (this.characters[index] is Child character)
        character.resetForPlayerEntry((GameLocation) this);
    }
    this.addGrandpaCandles();
    if (!Game1.MasterPlayer.mailReceived.Contains("Farm_Eternal") || Game1.player.mailReceived.Contains("Farm_Eternal_Parrots") || this.IsRainingHere())
      return;
    for (int index = 0; index < 20; ++index)
      this.temporarySprites.Add(new TemporaryAnimatedSprite("LooseSprites\\parrots", new Microsoft.Xna.Framework.Rectangle(49, 24 * Game1.random.Next(4), 24, 24), new Vector2((float) Game1.viewport.MaxCorner.X, (float) (Game1.viewport.Location.Y + Game1.random.Next(64 /*0x40*/, Game1.viewport.Height / 2))), false, 0.0f, Color.White)
      {
        scale = 4f,
        motion = new Vector2((float) ((double) Game1.random.Next(-10, 11) / 10.0 - 5.0), (float) (4.0 + (double) Game1.random.Next(-10, 11) / 10.0)),
        acceleration = new Vector2(0.0f, -0.02f),
        animationLength = 3,
        interval = 100f,
        pingPong = true,
        totalNumberOfLoops = 999,
        delayBeforeAnimationStart = index * 250,
        drawAboveAlwaysFront = true,
        startSound = "batFlap"
      });
    DelayedAction.playSoundAfterDelay("parrot_squawk", 1000);
    DelayedAction.playSoundAfterDelay("parrot_squawk", 4000);
    DelayedAction.playSoundAfterDelay("parrot", 3000);
    DelayedAction.playSoundAfterDelay("parrot", 5500);
    DelayedAction.playSoundAfterDelay("parrot_squawk", 7000);
    for (int index = 0; index < 20; ++index)
      DelayedAction.playSoundAfterDelay("batFlap", 5000 + index * 250);
    Game1.player.mailReceived.Add("Farm_Eternal_Parrots");
  }

  public virtual Vector2 GetSpouseOutdoorAreaCorner()
  {
    if (!this.mapSpouseAreaCorner.HasValue)
    {
      Vector2 parsed;
      if (!this.TryGetMapPropertyAs("SpouseAreaLocation", out parsed))
        parsed = new Vector2(69f, 6f);
      this.mapSpouseAreaCorner = new Vector2?(parsed);
    }
    return this.mapSpouseAreaCorner.Value;
  }

  public virtual void CacheOffBasePatioArea()
  {
    this._baseSpouseAreaTiles = new Dictionary<string, Dictionary<Point, Tile>>();
    List<string> stringList = new List<string>();
    foreach (Layer layer in this.map.Layers)
      stringList.Add(layer.Id);
    foreach (string str in stringList)
    {
      Layer layer = this.map.GetLayer(str);
      Dictionary<Point, Tile> dictionary = new Dictionary<Point, Tile>();
      this._baseSpouseAreaTiles[str] = dictionary;
      Vector2 outdoorAreaCorner = this.GetSpouseOutdoorAreaCorner();
      for (int x = (int) outdoorAreaCorner.X; x < (int) outdoorAreaCorner.X + 4; ++x)
      {
        for (int y = (int) outdoorAreaCorner.Y; y < (int) outdoorAreaCorner.Y + 4; ++y)
        {
          if (layer == null)
            dictionary[new Point(x, y)] = (Tile) null;
          else
            dictionary[new Point(x, y)] = layer.Tiles[x, y];
        }
      }
    }
  }

  public virtual void ReapplyBasePatioArea()
  {
    foreach (string key1 in this._baseSpouseAreaTiles.Keys)
    {
      Layer layer = this.map.GetLayer(key1);
      foreach (Point key2 in this._baseSpouseAreaTiles[key1].Keys)
      {
        Tile tile = this._baseSpouseAreaTiles[key1][key2];
        if (layer != null)
          layer.Tiles[key2.X, key2.Y] = tile;
      }
    }
  }

  public void addSpouseOutdoorArea(string spouseName)
  {
    this.ReapplyBasePatioArea();
    Point point1 = Utility.Vector2ToPoint(this.GetSpouseOutdoorAreaCorner());
    this.spousePatioSpot = new Point(point1.X + 2, point1.Y + 3);
    CharacterData data;
    CharacterSpousePatioData spousePatio = NPC.TryGetData(spouseName, out data) ? data.SpousePatio : (CharacterSpousePatioData) null;
    if (spousePatio == null)
      return;
    string map_name = spousePatio.MapAsset ?? "spousePatios";
    Microsoft.Xna.Framework.Rectangle mapSourceRect = spousePatio.MapSourceRect;
    int width = Math.Min(mapSourceRect.Width, 4);
    int height = Math.Min(mapSourceRect.Height, 4);
    Point point2 = point1;
    Microsoft.Xna.Framework.Rectangle rect = new Microsoft.Xna.Framework.Rectangle(point2.X, point2.Y, width, height);
    Point location = mapSourceRect.Location;
    if (this._appliedMapOverrides.Contains("spouse_patio"))
      this._appliedMapOverrides.Remove("spouse_patio");
    this.ApplyMapOverride(map_name, "spouse_patio", new Microsoft.Xna.Framework.Rectangle?(new Microsoft.Xna.Framework.Rectangle(location.X, location.Y, rect.Width, rect.Height)), new Microsoft.Xna.Framework.Rectangle?(rect));
    foreach (Point point3 in rect.GetPoints())
    {
      if (this.getTileIndexAt(point3, "Paths") == 7)
      {
        this.spousePatioSpot = point3;
        break;
      }
    }
  }

  public void addGrandpaCandles()
  {
    Point grandpaShrinePosition = this.GetGrandpaShrinePosition();
    if (this.grandpaScore.Value > 0)
    {
      Microsoft.Xna.Framework.Rectangle sourceRect = new Microsoft.Xna.Framework.Rectangle(577, 1985, 2, 5);
      this.removeTemporarySpritesWithIDLocal(6666);
      this.temporarySprites.Add(new TemporaryAnimatedSprite("LooseSprites\\Cursors", sourceRect, 99999f, 1, 9999, new Vector2((float) ((grandpaShrinePosition.X - 1) * 64 /*0x40*/ + 20), (float) ((grandpaShrinePosition.Y - 1) * 64 /*0x40*/ + 20)), false, false, (float) ((grandpaShrinePosition.Y - 1) * 64 /*0x40*/) / 10000f, 0.0f, Color.White, 4f, 0.0f, 0.0f, 0.0f));
      this.temporarySprites.Add(new TemporaryAnimatedSprite("LooseSprites\\Cursors", new Microsoft.Xna.Framework.Rectangle(536, 1945, 8, 8), new Vector2((float) ((grandpaShrinePosition.X - 1) * 64 /*0x40*/ + 12), (float) ((grandpaShrinePosition.Y - 1) * 64 /*0x40*/ - 4)), false, 0.0f, Color.White)
      {
        interval = 50f,
        totalNumberOfLoops = 99999,
        animationLength = 7,
        lightId = "Farm_GrandpaCandles_1",
        id = 6666,
        lightRadius = 1f,
        scale = 3f,
        layerDepth = 0.0385000035f,
        delayBeforeAnimationStart = 0
      });
      if (this.grandpaScore.Value > 1)
      {
        this.temporarySprites.Add(new TemporaryAnimatedSprite("LooseSprites\\Cursors", sourceRect, 99999f, 1, 9999, new Vector2((float) ((grandpaShrinePosition.X - 1) * 64 /*0x40*/ + 40), (float) ((grandpaShrinePosition.Y - 2) * 64 /*0x40*/ + 24)), false, false, (float) ((grandpaShrinePosition.Y - 1) * 64 /*0x40*/) / 10000f, 0.0f, Color.White, 4f, 0.0f, 0.0f, 0.0f));
        this.temporarySprites.Add(new TemporaryAnimatedSprite("LooseSprites\\Cursors", new Microsoft.Xna.Framework.Rectangle(536, 1945, 8, 8), new Vector2((float) ((grandpaShrinePosition.X - 1) * 64 /*0x40*/ + 36), (float) ((grandpaShrinePosition.Y - 2) * 64 /*0x40*/)), false, 0.0f, Color.White)
        {
          interval = 50f,
          totalNumberOfLoops = 99999,
          animationLength = 7,
          lightId = "Farm_GrandpaCandles_2",
          id = 6666,
          lightRadius = 1f,
          scale = 3f,
          layerDepth = 0.0385000035f,
          delayBeforeAnimationStart = 50
        });
      }
      if (this.grandpaScore.Value > 2)
      {
        this.temporarySprites.Add(new TemporaryAnimatedSprite("LooseSprites\\Cursors", sourceRect, 99999f, 1, 9999, new Vector2((float) ((grandpaShrinePosition.X + 1) * 64 /*0x40*/ + 20), (float) ((grandpaShrinePosition.Y - 2) * 64 /*0x40*/ + 24)), false, false, (float) ((grandpaShrinePosition.Y - 1) * 64 /*0x40*/) / 10000f, 0.0f, Color.White, 4f, 0.0f, 0.0f, 0.0f));
        this.temporarySprites.Add(new TemporaryAnimatedSprite("LooseSprites\\Cursors", new Microsoft.Xna.Framework.Rectangle(536, 1945, 8, 8), new Vector2((float) ((grandpaShrinePosition.X + 1) * 64 /*0x40*/ + 16 /*0x10*/), (float) ((grandpaShrinePosition.Y - 2) * 64 /*0x40*/)), false, 0.0f, Color.White)
        {
          interval = 50f,
          totalNumberOfLoops = 99999,
          animationLength = 7,
          lightId = "Farm_GrandpaCandles_3",
          id = 6666,
          lightRadius = 1f,
          scale = 3f,
          layerDepth = 0.0385000035f,
          delayBeforeAnimationStart = 100
        });
      }
      if (this.grandpaScore.Value > 3)
      {
        this.temporarySprites.Add(new TemporaryAnimatedSprite("LooseSprites\\Cursors", sourceRect, 99999f, 1, 9999, new Vector2((float) ((grandpaShrinePosition.X + 1) * 64 /*0x40*/ + 40), (float) ((grandpaShrinePosition.Y - 1) * 64 /*0x40*/ + 20)), false, false, (float) ((grandpaShrinePosition.Y - 1) * 64 /*0x40*/) / 10000f, 0.0f, Color.White, 4f, 0.0f, 0.0f, 0.0f));
        this.temporarySprites.Add(new TemporaryAnimatedSprite("LooseSprites\\Cursors", new Microsoft.Xna.Framework.Rectangle(536, 1945, 8, 8), new Vector2((float) ((grandpaShrinePosition.X + 1) * 64 /*0x40*/ + 36), (float) ((grandpaShrinePosition.Y - 1) * 64 /*0x40*/ - 4)), false, 0.0f, Color.White)
        {
          interval = 50f,
          totalNumberOfLoops = 99999,
          animationLength = 7,
          lightId = "Farm_GrandpaCandles_4",
          id = 6666,
          lightRadius = 1f,
          scale = 3f,
          layerDepth = 0.0385000035f,
          delayBeforeAnimationStart = 150
        });
      }
    }
    if (!Game1.MasterPlayer.mailReceived.Contains("Farm_Eternal"))
      return;
    this.temporarySprites.Add(new TemporaryAnimatedSprite("LooseSprites\\Cursors2", new Microsoft.Xna.Framework.Rectangle(176 /*0xB0*/, 157, 15, 16 /*0x10*/), 99999f, 1, 9999, new Vector2((float) (grandpaShrinePosition.X * 64 /*0x40*/ + 4), (float) ((grandpaShrinePosition.Y - 2) * 64 /*0x40*/ - 24)), false, false, (float) ((grandpaShrinePosition.Y - 1) * 64 /*0x40*/) / 10000f, 0.0f, Color.White, 4f, 0.0f, 0.0f, 0.0f));
  }

  private void openShippingBinLid()
  {
    if (this.shippingBinLid == null)
      return;
    if (this.shippingBinLid.pingPongMotion != 1 && Game1.currentLocation == this)
      this.localSound("doorCreak");
    this.shippingBinLid.pingPongMotion = 1;
    this.shippingBinLid.paused = false;
  }

  private void closeShippingBinLid()
  {
    if (this.shippingBinLid == null || this.shippingBinLid.currentParentTileIndex <= 0)
      return;
    if (this.shippingBinLid.pingPongMotion != -1 && Game1.currentLocation == this)
      this.localSound("doorCreakReverse");
    this.shippingBinLid.pingPongMotion = -1;
    this.shippingBinLid.paused = false;
  }

  private void updateShippingBinLid(GameTime time)
  {
    if (this.isShippingBinLidOpen(true) && this.shippingBinLid.pingPongMotion == 1)
      this.shippingBinLid.paused = true;
    else if (this.shippingBinLid.currentParentTileIndex == 0 && this.shippingBinLid.pingPongMotion == -1)
    {
      if (!this.shippingBinLid.paused && Game1.currentLocation == this)
        this.localSound("woodyStep");
      this.shippingBinLid.paused = true;
    }
    this.shippingBinLid.update(time);
  }

  private bool isShippingBinLidOpen(bool requiredToBeFullyOpen = false)
  {
    return this.shippingBinLid != null && this.shippingBinLid.currentParentTileIndex >= (requiredToBeFullyOpen ? this.shippingBinLid.animationLength - 1 : 1);
  }

  public override void pokeTileForConstruction(Vector2 tile)
  {
    base.pokeTileForConstruction(tile);
    foreach (NPC character in this.characters)
    {
      if (character is Pet pet && pet.Tile == tile)
      {
        pet.FacingDirection = Game1.random.Next(0, 4);
        pet.faceDirection(pet.FacingDirection);
        pet.CurrentBehavior = "Walk";
        pet.forceUpdateTimer = 2000;
        pet.setMovingInFacingDirection();
      }
    }
  }

  public override bool shouldShadowBeDrawnAboveBuildingsLayer(Vector2 p)
  {
    return this.doesTileHaveProperty((int) p.X, (int) p.Y, "NoSpawn", "Back") == "All" && this.doesTileHaveProperty((int) p.X, (int) p.Y, "Type", "Back") == "Wood" || base.shouldShadowBeDrawnAboveBuildingsLayer(p);
  }

  public override void draw(SpriteBatch b)
  {
    base.draw(b);
    if (Game1.mailbox.Count > 0)
    {
      float num1 = (float) (4.0 * Math.Round(Math.Sin(Game1.currentGameTime.TotalGameTime.TotalMilliseconds / 250.0), 2));
      Point mailboxPosition = Game1.player.getMailboxPosition();
      float num2 = (float) ((double) ((mailboxPosition.X + 1) * 64 /*0x40*/) / 10000.0 + (double) (mailboxPosition.Y * 64 /*0x40*/) / 10000.0);
      b.Draw(Game1.mouseCursors, Game1.GlobalToLocal(Game1.viewport, new Vector2((float) (mailboxPosition.X * 64 /*0x40*/), (float) (mailboxPosition.Y * 64 /*0x40*/ - 96 /*0x60*/ - 48 /*0x30*/) + num1)), new Microsoft.Xna.Framework.Rectangle?(new Microsoft.Xna.Framework.Rectangle(141, 465, 20, 24)), Color.White * 0.75f, 0.0f, Vector2.Zero, 4f, SpriteEffects.None, num2 + 1E-06f);
      b.Draw(Game1.mouseCursors, Game1.GlobalToLocal(Game1.viewport, new Vector2((float) (mailboxPosition.X * 64 /*0x40*/ + 32 /*0x20*/ + 4), (float) (mailboxPosition.Y * 64 /*0x40*/ - 64 /*0x40*/ - 24 - 8) + num1)), new Microsoft.Xna.Framework.Rectangle?(new Microsoft.Xna.Framework.Rectangle(189, 423, 15, 13)), Color.White, 0.0f, new Vector2(7f, 6f), 4f, SpriteEffects.None, num2 + 1E-05f);
    }
    this.shippingBinLid?.draw(b);
    if (this.hasSeenGrandpaNote)
      return;
    Point grandpaShrinePosition = this.GetGrandpaShrinePosition();
    b.Draw(Game1.mouseCursors, Game1.GlobalToLocal(Game1.viewport, new Vector2((float) ((grandpaShrinePosition.X + 1) * 64 /*0x40*/), (float) (grandpaShrinePosition.Y * 64 /*0x40*/))), new Microsoft.Xna.Framework.Rectangle?(new Microsoft.Xna.Framework.Rectangle(575, 1972, 11, 8)), Color.White, 0.0f, Vector2.Zero, 4f, SpriteEffects.None, (float) ((double) (grandpaShrinePosition.Y * 64 /*0x40*/) / 10000.0 + 9.9999999747524271E-07));
  }

  public virtual Point GetMainMailboxPosition()
  {
    if (!this.mapMainMailboxPosition.HasValue)
    {
      Point parsed;
      if (!this.TryGetMapPropertyAs("MailboxLocation", out parsed))
        parsed = new Point(68, 16 /*0x10*/);
      this.mapMainMailboxPosition = new Point?(parsed);
      Building mainFarmHouse = this.GetMainFarmHouse();
      BuildingData data = mainFarmHouse?.GetData();
      if (data?.ActionTiles != null)
      {
        foreach (BuildingActionTile actionTile in data.ActionTiles)
        {
          if (actionTile.Action == "Mailbox")
          {
            this.mapMainMailboxPosition = new Point?(new Point(mainFarmHouse.tileX.Value + actionTile.Tile.X, mainFarmHouse.tileY.Value + actionTile.Tile.Y));
            break;
          }
        }
      }
    }
    return this.mapMainMailboxPosition.Value;
  }

  public virtual Point GetGrandpaShrinePosition()
  {
    if (!this.mapGrandpaShrinePosition.HasValue)
    {
      Point parsed;
      if (!this.TryGetMapPropertyAs("GrandpaShrineLocation", out parsed))
        parsed = new Point(8, 7);
      this.mapGrandpaShrinePosition = new Point?(parsed);
    }
    return this.mapGrandpaShrinePosition.Value;
  }

  /// <summary>Get the door tile position for the farmhouse.</summary>
  /// <remarks>See also <see cref="M:StardewValley.Farm.GetStarterFarmhouseLocation" />.</remarks>
  public virtual Point GetMainFarmHouseEntry()
  {
    if (!this.mainFarmhouseEntry.HasValue)
    {
      Point parsed;
      if (!this.TryGetMapPropertyAs("FarmHouseEntry", out parsed))
        parsed = new Point(64 /*0x40*/, 15);
      this.mainFarmhouseEntry = new Point?(parsed);
      Building mainFarmHouse = this.GetMainFarmHouse();
      if (mainFarmHouse != null)
        this.mainFarmhouseEntry = new Point?(new Point(mainFarmHouse.tileX.Value + mainFarmHouse.humanDoor.X, mainFarmHouse.tileY.Value + mainFarmHouse.humanDoor.Y + 1));
    }
    return this.mainFarmhouseEntry.Value;
  }

  /// <summary>Get the main player's farmhouse, if found.</summary>
  public virtual Building GetMainFarmHouse() => this.getBuildingByType("Farmhouse");

  public override void ResetForEvent(Event ev)
  {
    base.ResetForEvent(ev);
    if (!(ev.id != "-2"))
      return;
    Point positionForFarmer = Farm.getFrontDoorPositionForFarmer(ev.farmer);
    ++positionForFarmer.Y;
    int x = positionForFarmer.X - 64 /*0x40*/;
    int y = positionForFarmer.Y - 15;
    ev.eventPositionTileOffset = new Vector2((float) x, (float) y);
  }

  public override void updateEvenIfFarmerIsntHere(GameTime time, bool skipWasUpdatedFlush = false)
  {
    this.spawnCrowEvent.Poll();
    this.lightningStrikeEvent.Poll();
    base.updateEvenIfFarmerIsntHere(time, skipWasUpdatedFlush);
  }

  public bool isTileOpenBesidesTerrainFeatures(Vector2 tile)
  {
    Microsoft.Xna.Framework.Rectangle boundingBox = new Microsoft.Xna.Framework.Rectangle((int) tile.X * 64 /*0x40*/, (int) tile.Y * 64 /*0x40*/, 64 /*0x40*/, 64 /*0x40*/);
    foreach (Building building in this.buildings)
    {
      if (building.intersects(boundingBox))
        return false;
    }
    foreach (TerrainFeature resourceClump in this.resourceClumps)
    {
      if (resourceClump.getBoundingBox().Intersects(boundingBox))
        return false;
    }
    foreach (KeyValuePair<long, FarmAnimal> pair in this.animals.Pairs)
    {
      if (pair.Value.Tile == tile)
        return true;
    }
    return !this.objects.ContainsKey(tile) && this.isTilePassable(new Location((int) tile.X, (int) tile.Y), Game1.viewport);
  }

  private void doLightningStrike(Farm.LightningStrikeEvent lightning)
  {
    if (lightning.smallFlash)
    {
      if (Game1.currentLocation.IsOutdoors && !Game1.newDay && Game1.currentLocation.IsLightningHere())
      {
        Game1.flashAlpha = (float) (0.5 + Game1.random.NextDouble());
        if (Game1.random.NextBool())
          DelayedAction.screenFlashAfterDelay((float) (0.3 + Game1.random.NextDouble()), Game1.random.Next(500, 1000));
        DelayedAction.playSoundAfterDelay("thunder_small", Game1.random.Next(500, 1500));
      }
    }
    else if (lightning.bigFlash && Game1.currentLocation.IsOutdoors && Game1.currentLocation.IsLightningHere() && !Game1.newDay)
    {
      Game1.flashAlpha = (float) (0.5 + Game1.random.NextDouble());
      Game1.playSound("thunder");
    }
    if (!lightning.createBolt || !Game1.currentLocation.name.Equals((object) nameof (Farm)))
      return;
    if (lightning.destroyedTerrainFeature)
      this.temporarySprites.Add(new TemporaryAnimatedSprite(362, 75f, 6, 1, lightning.boltPosition, false, false));
    Utility.drawLightningBolt(lightning.boltPosition, (GameLocation) this);
  }

  public override void UpdateWhenCurrentLocation(GameTime time)
  {
    if (this.wasUpdated && Game1.gameMode != (byte) 0)
      return;
    base.UpdateWhenCurrentLocation(time);
    if (this.shippingBinLid == null)
      return;
    bool flag = false;
    foreach (Character farmer in this.farmers)
    {
      if (farmer.GetBoundingBox().Intersects(this.shippingBinLidOpenArea))
      {
        this.openShippingBinLid();
        flag = true;
      }
    }
    if (!flag)
      this.closeShippingBinLid();
    this.updateShippingBinLid(time);
  }

  public bool ShouldSpawnMountainOres()
  {
    if (!this._mountainForageRectangle.HasValue)
    {
      Microsoft.Xna.Framework.Rectangle parsed;
      this._mountainForageRectangle = new Microsoft.Xna.Framework.Rectangle?(this.TryGetMapPropertyAs("SpawnMountainFarmOreRect", out parsed) ? parsed : Microsoft.Xna.Framework.Rectangle.Empty);
    }
    return this._mountainForageRectangle.Value.Width > 0;
  }

  public bool ShouldSpawnForestFarmForage()
  {
    if (this.map != null)
    {
      if (!this._shouldSpawnForestFarmForage.HasValue)
        this._shouldSpawnForestFarmForage = new bool?(this.map.Properties.ContainsKey("SpawnForestFarmForage"));
      if (this._shouldSpawnForestFarmForage.Value)
        return true;
    }
    return Game1.whichFarm == 2;
  }

  public bool ShouldSpawnBeachFarmForage()
  {
    if (this.map != null)
    {
      if (!this._shouldSpawnBeachFarmForage.HasValue)
        this._shouldSpawnBeachFarmForage = new bool?(this.map.Properties.ContainsKey("SpawnBeachFarmForage"));
      if (this._shouldSpawnBeachFarmForage.Value)
        return true;
    }
    return Game1.whichFarm == 6;
  }

  public bool SpawnsForage()
  {
    return this.ShouldSpawnForestFarmForage() || this.ShouldSpawnBeachFarmForage();
  }

  public bool doesFarmCaveNeedHarvesting() => this.farmCaveReady.Value;

  public class LightningStrikeEvent : NetEventArg
  {
    public Vector2 boltPosition;
    public bool createBolt;
    public bool bigFlash;
    public bool smallFlash;
    public bool destroyedTerrainFeature;

    public void Read(BinaryReader reader)
    {
      this.createBolt = reader.ReadBoolean();
      this.bigFlash = reader.ReadBoolean();
      this.smallFlash = reader.ReadBoolean();
      this.destroyedTerrainFeature = reader.ReadBoolean();
      this.boltPosition.X = (float) reader.ReadInt32();
      this.boltPosition.Y = (float) reader.ReadInt32();
    }

    public void Write(BinaryWriter writer)
    {
      writer.Write(this.createBolt);
      writer.Write(this.bigFlash);
      writer.Write(this.smallFlash);
      writer.Write(this.destroyedTerrainFeature);
      writer.Write((int) this.boltPosition.X);
      writer.Write((int) this.boltPosition.Y);
    }
  }
}
