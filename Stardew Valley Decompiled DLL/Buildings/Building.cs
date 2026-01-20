// Decompiled with JetBrains decompiler
// Type: StardewValley.Buildings.Building
// Assembly: Stardew Valley, Version=1.6.15.24356, Culture=neutral, PublicKeyToken=null
// MVID: AA1F513A-94F0-4EF6-A35F-2D5B4A3721BF
// Assembly location: D:\SteamLibrary\steamapps\common\Stardew Valley\Stardew Valley.dll
// XML documentation location: D:\SteamLibrary\steamapps\common\Stardew Valley\Stardew Valley.xml

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Netcode;
using Netcode.Validation;
using StardewValley.Delegates;
using StardewValley.Extensions;
using StardewValley.GameData;
using StardewValley.GameData.Buildings;
using StardewValley.Internal;
using StardewValley.ItemTypeDefinitions;
using StardewValley.Locations;
using StardewValley.Menus;
using StardewValley.Mods;
using StardewValley.Objects;
using StardewValley.TerrainFeatures;
using StardewValley.TokenizableStrings;
using StardewValley.Util;
using System;
using System.Collections.Generic;
using System.Xml.Serialization;
using xTile.Dimensions;

#nullable disable
namespace StardewValley.Buildings;

[XmlInclude(typeof (Barn))]
[XmlInclude(typeof (Coop))]
[XmlInclude(typeof (FishPond))]
[XmlInclude(typeof (GreenhouseBuilding))]
[XmlInclude(typeof (JunimoHut))]
[XmlInclude(typeof (Mill))]
[XmlInclude(typeof (PetBowl))]
[XmlInclude(typeof (ShippingBin))]
[XmlInclude(typeof (Stable))]
[NotImplicitNetField]
public class Building : INetObject<NetFields>, IHaveModData
{
  /// <summary>A unique identifier for this specific building instance.</summary>
  [XmlElement("id")]
  public readonly NetGuid id;
  [XmlIgnore]
  public Lazy<Texture2D> texture;
  [XmlIgnore]
  public Texture2D paintedTexture;
  public NetString skinId;
  /// <summary>The indoor location created for this building, if any.</summary>
  /// <remarks>This is mutually exclusive with <see cref="F:StardewValley.Buildings.Building.nonInstancedIndoorsName" />. Most code should use <see cref="M:StardewValley.Buildings.Building.GetIndoors" /> instead, which handles both.</remarks>
  [XmlElement("indoors")]
  public readonly NetRef<GameLocation> indoors;
  /// <summary>The unique ID of the separate location treated as the building interior (like <c>FarmHouse</c> for the farmhouse), if any.</summary>
  /// <remarks>This is mutually exclusive with <see cref="F:StardewValley.Buildings.Building.indoors" />. Most code should use <see cref="M:StardewValley.Buildings.Building.GetIndoors" /> instead, which handles both.</remarks>
  public readonly NetString nonInstancedIndoorsName;
  [XmlElement("tileX")]
  public readonly NetInt tileX;
  [XmlElement("tileY")]
  public readonly NetInt tileY;
  [XmlElement("tilesWide")]
  public readonly NetInt tilesWide;
  [XmlElement("tilesHigh")]
  public readonly NetInt tilesHigh;
  [XmlElement("maxOccupants")]
  public readonly NetInt maxOccupants;
  [XmlElement("currentOccupants")]
  public readonly NetInt currentOccupants;
  [XmlElement("daysOfConstructionLeft")]
  public readonly NetInt daysOfConstructionLeft;
  [XmlElement("daysUntilUpgrade")]
  public readonly NetInt daysUntilUpgrade;
  [XmlElement("upgradeName")]
  public readonly NetString upgradeName;
  [XmlElement("buildingType")]
  public readonly NetString buildingType;
  [XmlElement("buildingPaintColor")]
  public NetRef<BuildingPaintColor> netBuildingPaintColor;
  [XmlElement("hayCapacity")]
  public NetInt hayCapacity;
  public NetList<Chest, NetRef<Chest>> buildingChests;
  /// <summary>The unique name of the location which contains this building.</summary>
  [XmlIgnore]
  public NetString parentLocationName;
  [XmlIgnore]
  public bool hasLoaded;
  [XmlIgnore]
  protected Dictionary<string, string> buildingMetadata;
  protected int lastHouseUpgradeLevel;
  protected bool? hasChimney;
  protected Vector2 chimneyPosition;
  protected int chimneyTimer;
  [XmlElement("humanDoor")]
  public readonly NetPoint humanDoor;
  [XmlElement("animalDoor")]
  public readonly NetPoint animalDoor;
  /// <summary>A temporary color applied to the building sprite when it's highlighted in a menu.</summary>
  [XmlIgnore]
  public Color color;
  [XmlElement("animalDoorOpen")]
  public readonly NetBool animalDoorOpen;
  [XmlElement("animalDoorOpenAmount")]
  public readonly NetFloat animalDoorOpenAmount;
  [XmlElement("magical")]
  public readonly NetBool magical;
  /// <summary>Whether this building should fade into semi-transparency when the local player is behind it.</summary>
  [XmlElement("fadeWhenPlayerIsBehind")]
  public readonly NetBool fadeWhenPlayerIsBehind;
  [XmlElement("owner")]
  public readonly NetLong owner;
  [XmlElement("newConstructionTimer")]
  protected readonly NetInt newConstructionTimer;
  /// <summary>The building's opacity for the local player as a value between 0 (transparent) and 1 (opaque), accounting for <see cref="F:StardewValley.Buildings.Building.fadeWhenPlayerIsBehind" />.</summary>
  [XmlIgnore]
  public float alpha;
  [XmlIgnore]
  protected bool _isMoving;
  public static Microsoft.Xna.Framework.Rectangle leftShadow = new Microsoft.Xna.Framework.Rectangle(656, 394, 16 /*0x10*/, 16 /*0x10*/);
  public static Microsoft.Xna.Framework.Rectangle middleShadow = new Microsoft.Xna.Framework.Rectangle(672, 394, 16 /*0x10*/, 16 /*0x10*/);
  public static Microsoft.Xna.Framework.Rectangle rightShadow = new Microsoft.Xna.Framework.Rectangle(688, 394, 16 /*0x10*/, 16 /*0x10*/);

  /// <inheritdoc />
  [XmlIgnore]
  public ModDataDictionary modData { get; }

  /// <inheritdoc />
  [XmlElement("modData")]
  public ModDataDictionary modDataForSerialization
  {
    get => this.modData.GetForSerialization();
    set => this.modData.SetFromSerialization(value);
  }

  /// <summary>Get whether this is a farmhand cabin.</summary>
  /// <remarks>To check whether a farmhand has claimed it, use <see cref="M:StardewValley.Buildings.Building.GetIndoors" /> to get the <see cref="T:StardewValley.Locations.Cabin" /> or <see cref="T:StardewValley.Locations.FarmHouse" /> instance and call methods like <see cref="P:StardewValley.Locations.FarmHouse.HasOwner" />.</remarks>
  public bool isCabin => this.buildingType.Value == "Cabin";

  public bool isMoving
  {
    get => this._isMoving;
    set
    {
      if (this._isMoving == value)
        return;
      this._isMoving = value;
      if (this._isMoving)
        this.OnStartMove();
      if (this._isMoving)
        return;
      this.OnEndMove();
    }
  }

  public NetFields NetFields { get; }

  /// <summary>Construct an instance.</summary>
  public Building()
  {
    NetFloat netFloat = new NetFloat();
    netFloat.InterpolationWait = false;
    this.animalDoorOpenAmount = netFloat;
    this.magical = new NetBool();
    this.fadeWhenPlayerIsBehind = new NetBool(true);
    this.owner = new NetLong();
    this.newConstructionTimer = new NetInt();
    this.alpha = 1f;
    this.NetFields = new NetFields(nameof (Building));
    // ISSUE: explicit constructor call
    base.\u002Ector();
    this.id.Value = Guid.NewGuid();
    this.resetTexture();
    this.initNetFields();
  }

  /// <summary>Construct an instance.</summary>
  /// <param name="type">The building type ID in <see cref="F:StardewValley.Game1.buildingData" />.</param>
  /// <param name="tile">The top-left tile position of the building.</param>
  public Building(string type, Vector2 tile)
    : this()
  {
    this.tileX.Value = (int) tile.X;
    this.tileY.Value = (int) tile.Y;
    this.buildingType.Value = type;
    BuildingData buildingData = this.ReloadBuildingData();
    this.daysOfConstructionLeft.Value = buildingData != null ? buildingData.BuildDays : 0;
  }

  /// <summary>Get whether the building has any skins that can be applied to it currently.</summary>
  /// <param name="ignoreSeparateConstructionEntries">Whether to ignore skins with <see cref="F:StardewValley.GameData.Buildings.BuildingSkin.ShowAsSeparateConstructionEntry" /> set to true.</param>
  public virtual bool CanBeReskinned(bool ignoreSeparateConstructionEntries = false)
  {
    BuildingData data = this.GetData();
    if (this.skinId.Value != null)
      return true;
    if (data?.Skins != null)
    {
      foreach (BuildingSkin skin in data.Skins)
      {
        if (!(skin.Id == this.skinId.Value) && (!ignoreSeparateConstructionEntries || !skin.ShowAsSeparateConstructionEntry) && GameStateQuery.CheckConditions(skin.Condition, this.GetParentLocation()))
          return true;
      }
    }
    return false;
  }

  /// <summary>Get whether animals within this building can get pregnant and produce offspring.</summary>
  public bool AllowsAnimalPregnancy()
  {
    BuildingData data = this.GetData();
    return data != null && data.AllowAnimalPregnancy;
  }

  /// <summary>Get whether players can repaint this building.</summary>
  public virtual bool CanBePainted()
  {
    return (!(this is GreenhouseBuilding) || Game1.getFarm().greenhouseUnlocked.Value) && (!this.isCabin && !this.HasIndoorsName("Farmhouse") || !(this.GetIndoors() is FarmHouse indoors) || indoors.upgradeLevel >= 2) && this.GetPaintDataKey() != null;
  }

  /// <summary>Get the building's current skin, if applicable.</summary>
  public BuildingSkin GetSkin() => Building.GetSkin(this.skinId.Value, this.GetData());

  /// <summary>Get a building skin from data, if it exists.</summary>
  /// <param name="skinId">The building skin ID to find.</param>
  /// <param name="data">The building data to search.</param>
  /// <returns>Returns the matching building skin if found, else <c>null</c>.</returns>
  public static BuildingSkin GetSkin(string skinId, BuildingData data)
  {
    if (skinId != null && data?.Skins != null)
    {
      foreach (BuildingSkin skin in data.Skins)
      {
        if (skin.Id == skinId)
          return skin;
      }
    }
    return (BuildingSkin) null;
  }

  /// <summary>Get the key in <c>Data/PaintData</c> for the building, if it has any.</summary>
  public virtual string GetPaintDataKey()
  {
    return this.GetPaintDataKey(DataLoader.PaintData(Game1.content));
  }

  /// <summary>Get the key in <c>Data/PaintData</c> for the building, if it has any.</summary>
  /// <param name="paintData">The loaded <c>Data/PaintData</c> asset.</param>
  public virtual string GetPaintDataKey(Dictionary<string, string> paintData)
  {
    if (this.skinId.Value != null && paintData.ContainsKey(this.skinId.Value))
      return this.skinId.Value;
    string key;
    switch (this.buildingType.Value)
    {
      case "Farmhouse":
        key = "House";
        break;
      case "Cabin":
        key = "Stone Cabin";
        break;
      default:
        key = this.buildingType.Value;
        break;
    }
    return !paintData.ContainsKey(key) ? (string) null : key;
  }

  public string GetMetadata(string key)
  {
    if (this.buildingMetadata == null)
    {
      this.buildingMetadata = new Dictionary<string, string>();
      BuildingData data = this.GetData();
      if (data != null)
      {
        foreach (KeyValuePair<string, string> keyValuePair in data.Metadata)
          this.buildingMetadata[keyValuePair.Key] = keyValuePair.Value;
        BuildingSkin skin = Building.GetSkin(this.skinId.Value, data);
        if (skin != null)
        {
          foreach (KeyValuePair<string, string> keyValuePair in skin.Metadata)
            this.buildingMetadata[keyValuePair.Key] = keyValuePair.Value;
        }
      }
    }
    return !this.buildingMetadata.TryGetValue(key, out key) ? (string) null : key;
  }

  /// <summary>Get the location which contains this building.</summary>
  public GameLocation GetParentLocation()
  {
    return Game1.getLocationFromName(this.parentLocationName.Value);
  }

  /// <summary>Get whether the building is in <see cref="P:StardewValley.Game1.currentLocation" />.</summary>
  public bool IsInCurrentLocation()
  {
    return Game1.currentLocation != null && Game1.currentLocation.NameOrUniqueName == this.parentLocationName.Value;
  }

  public virtual bool hasCarpenterPermissions()
  {
    return Game1.IsMasterGame || this.owner.Value == Game1.player.UniqueMultiplayerID || this.GetIndoors() is FarmHouse indoors && indoors.IsOwnedByCurrentPlayer;
  }

  protected virtual void initNetFields()
  {
    this.NetFields.SetOwner((INetObject<NetFields>) this).AddField((INetSerializable) this.id, "id").AddField((INetSerializable) this.indoors, "indoors").AddField((INetSerializable) this.nonInstancedIndoorsName, "nonInstancedIndoorsName").AddField((INetSerializable) this.tileX, "tileX").AddField((INetSerializable) this.tileY, "tileY").AddField((INetSerializable) this.tilesWide, "tilesWide").AddField((INetSerializable) this.tilesHigh, "tilesHigh").AddField((INetSerializable) this.maxOccupants, "maxOccupants").AddField((INetSerializable) this.currentOccupants, "currentOccupants").AddField((INetSerializable) this.daysOfConstructionLeft, "daysOfConstructionLeft").AddField((INetSerializable) this.daysUntilUpgrade, "daysUntilUpgrade").AddField((INetSerializable) this.buildingType, "buildingType").AddField((INetSerializable) this.humanDoor, "humanDoor").AddField((INetSerializable) this.animalDoor, "animalDoor").AddField((INetSerializable) this.magical, "magical").AddField((INetSerializable) this.fadeWhenPlayerIsBehind, "fadeWhenPlayerIsBehind").AddField((INetSerializable) this.animalDoorOpen, "animalDoorOpen").AddField((INetSerializable) this.owner, "owner").AddField((INetSerializable) this.newConstructionTimer, "newConstructionTimer").AddField((INetSerializable) this.netBuildingPaintColor, "netBuildingPaintColor").AddField((INetSerializable) this.buildingChests, "buildingChests").AddField((INetSerializable) this.animalDoorOpenAmount, "animalDoorOpenAmount").AddField((INetSerializable) this.hayCapacity, "hayCapacity").AddField((INetSerializable) this.parentLocationName, "parentLocationName").AddField((INetSerializable) this.upgradeName, "upgradeName").AddField((INetSerializable) this.skinId, "skinId").AddField((INetSerializable) this.modData, "modData");
    this.buildingType.fieldChangeVisibleEvent += (FieldChange<NetString, string>) ((a, b, c) =>
    {
      this.hasChimney = new bool?();
      this.ReloadBuildingData(b != null && b != c);
    });
    this.skinId.fieldChangeVisibleEvent += (FieldChange<NetString, string>) ((a, b, c) =>
    {
      this.hasChimney = new bool?();
      this.buildingMetadata = (Dictionary<string, string>) null;
      this.resetTexture();
    });
    this.buildingType.fieldChangeVisibleEvent += (FieldChange<NetString, string>) ((a, b, c) =>
    {
      this.hasChimney = new bool?();
      this.buildingMetadata = (Dictionary<string, string>) null;
      this.resetTexture();
    });
    this.indoors.fieldChangeVisibleEvent += (FieldChange<NetRef<GameLocation>, GameLocation>) ((field, oldValue, newValue) => this.UpdateIndoorParent());
    this.parentLocationName.fieldChangeVisibleEvent += (FieldChange<NetString, string>) ((field, oldValue, newValue) => this.UpdateIndoorParent());
    if (this.netBuildingPaintColor.Value != null)
      return;
    this.netBuildingPaintColor.Value = new BuildingPaintColor();
  }

  public virtual void UpdateIndoorParent()
  {
    GameLocation indoors = this.GetIndoors();
    if (indoors == null)
      return;
    indoors.ParentBuilding = this;
    indoors.parentLocationName.Value = this.parentLocationName.Value;
  }

  /// <summary>Get the building's data from <see cref="F:StardewValley.Game1.buildingData" />, if found.</summary>
  public virtual BuildingData GetData()
  {
    BuildingData data;
    return !Building.TryGetData(this.buildingType.Value, out data) ? (BuildingData) null : data;
  }

  /// <summary>Try to get a building's data from <see cref="F:StardewValley.Game1.buildingData" />.</summary>
  /// <param name="buildingType">The building type (i.e. the key in <see cref="F:StardewValley.Game1.buildingData" />).</param>
  /// <param name="data">The building data, if found.</param>
  /// <returns>Returns whether the building data was found.</returns>
  public static bool TryGetData(string buildingType, out BuildingData data)
  {
    if (buildingType != null)
      return Game1.buildingData.TryGetValue(buildingType, out data);
    data = (BuildingData) null;
    return false;
  }

  /// <summary>Reload the building's data from <see cref="F:StardewValley.Game1.buildingData" /> and reapply it to the building's fields.</summary>
  /// <param name="forUpgrade">Whether the building is being upgraded.</param>
  /// <param name="forConstruction">Whether the building is being constructed.</param>
  /// <returns>Returns the loaded building data, if any.</returns>
  /// <remarks>See also <see cref="M:StardewValley.Buildings.Building.LoadFromBuildingData(StardewValley.GameData.Buildings.BuildingData,System.Boolean,System.Boolean)" />.</remarks>
  public virtual BuildingData ReloadBuildingData(bool forUpgrade = false, bool forConstruction = false)
  {
    BuildingData data = this.GetData();
    if (data != null)
      this.LoadFromBuildingData(data, forUpgrade, forConstruction);
    return data;
  }

  /// <summary>Reapply the loaded data to the building's fields.</summary>
  /// <param name="data">The building data to load.</param>
  /// <param name="forUpgrade">Whether the building is being upgraded.</param>
  /// <param name="forConstruction">Whether the building is being constructed.</param>
  /// <remarks>This doesn't reload the underlying data; see <see cref="M:StardewValley.Buildings.Building.ReloadBuildingData(System.Boolean,System.Boolean)" /> if you need to do that.</remarks>
  public virtual void LoadFromBuildingData(
    BuildingData data,
    bool forUpgrade = false,
    bool forConstruction = false)
  {
    if (data == null)
      return;
    this.tilesWide.Value = data.Size.X;
    this.tilesHigh.Value = data.Size.Y;
    this.humanDoor.X = data.HumanDoor.X;
    this.humanDoor.Y = data.HumanDoor.Y;
    this.animalDoor.Value = data.AnimalDoor.Location;
    if (data.MaxOccupants >= 0)
      this.maxOccupants.Value = data.MaxOccupants;
    this.hayCapacity.Value = data.HayCapacity;
    this.magical.Value = data.Builder == "Wizard";
    this.fadeWhenPlayerIsBehind.Value = data.FadeWhenBehind;
    foreach (KeyValuePair<string, string> keyValuePair in data.ModData)
      this.modData[keyValuePair.Key] = keyValuePair.Value;
    this.GetIndoors()?.InvalidateCachedMultiplayerMap(Game1.multiplayer.cachedMultiplayerMaps);
    if (!Game1.IsMasterGame)
      return;
    if (this.hasLoaded | forConstruction)
    {
      if (this.nonInstancedIndoorsName.Value == null)
      {
        string indoorMap = data.IndoorMap;
        string indoorMapType = typeof (GameLocation).ToString();
        if (data.IndoorMapType != null)
          indoorMapType = data.IndoorMapType;
        if (indoorMap != null)
        {
          string str = "Maps\\" + indoorMap;
          if (this.indoors.Value == null)
          {
            this.indoors.Value = this.createIndoors(data, data.IndoorMap);
            this.InitializeIndoor(data, forConstruction, forUpgrade);
          }
          else if (this.indoors.Value.mapPath.Value == str)
          {
            if (forUpgrade)
              this.InitializeIndoor(data, forConstruction, true);
          }
          else
          {
            if (this.indoors.Value.GetType().ToString() != indoorMapType)
            {
              this.load();
            }
            else
            {
              this.indoors.Value.mapPath.Value = str;
              this.indoors.Value.updateMap();
            }
            this.updateInteriorWarps(this.indoors.Value);
            this.InitializeIndoor(data, forConstruction, forUpgrade);
          }
        }
      }
      else
        this.updateInteriorWarps();
    }
    if (!(this.hasLoaded | forConstruction))
      return;
    HashSet<string> validChests = new HashSet<string>();
    if (data.Chests != null)
    {
      foreach (BuildingChest chest in data.Chests)
        validChests.Add(chest.Id);
    }
    this.buildingChests.RemoveWhere((Func<Chest, bool>) (chest => !validChests.Contains(chest.Name)));
    if (data.Chests == null)
      return;
    foreach (BuildingChest chest1 in data.Chests)
    {
      if (this.GetBuildingChest(chest1.Id) == null)
      {
        Chest chest2 = new Chest(true);
        chest2.Name = chest1.Id;
        this.buildingChests.Add(chest2);
      }
    }
  }

  /// <summary>Create a building instance from its type ID.</summary>
  /// <param name="typeId">The building type ID in <c>Data/Buildings</c>.</param>
  /// <param name="tile">The top-left tile position of the building.</param>
  public static Building CreateInstanceFromId(string typeId, Vector2 tile)
  {
    BuildingData buildingData;
    if (typeId != null && Game1.buildingData.TryGetValue(typeId, out buildingData))
    {
      Type type = buildingData.BuildingType != null ? Type.GetType(buildingData.BuildingType) : (Type) null;
      if (type != (Type) null)
      {
        if (type != typeof (Building))
        {
          try
          {
            return (Building) Activator.CreateInstance(type, (object) typeId, (object) tile);
          }
          catch (MissingMethodException ex1)
          {
            try
            {
              Building instance = (Building) Activator.CreateInstance(type, (object) tile);
              instance.buildingType.Value = typeId;
              return instance;
            }
            catch (Exception ex2)
            {
              Game1.log.Error($"Error trying to instantiate building for type '{typeId}'", ex2);
            }
          }
        }
      }
    }
    return new Building(typeId, tile);
  }

  public virtual void InitializeIndoor(BuildingData data, bool forConstruction, bool forUpgrade)
  {
    if (data == null)
      return;
    GameLocation indoors = this.GetIndoors();
    if (indoors == null)
      return;
    if (indoors is AnimalHouse animalHouse && data.MaxOccupants > 0)
      animalHouse.animalLimit.Value = data.MaxOccupants;
    if (forUpgrade && data.IndoorItemMoves != null)
    {
      foreach (IndoorItemMove indoorItemMove in data.IndoorItemMoves)
      {
        for (int index1 = 0; index1 < indoorItemMove.Size.X; ++index1)
        {
          for (int index2 = 0; index2 < indoorItemMove.Size.Y; ++index2)
            indoors.moveContents(indoorItemMove.Source.X + index1, indoorItemMove.Source.Y + index2, indoorItemMove.Destination.X + index1, indoorItemMove.Destination.Y + index2, indoorItemMove.UnlessItemId);
        }
      }
    }
    if (!(forConstruction | forUpgrade) || data.IndoorItems == null)
      return;
    foreach (IndoorItemAdd indoorItem in data.IndoorItems)
    {
      Vector2 vector2 = Utility.PointToVector2(indoorItem.Tile);
      StardewValley.Object @object = ItemRegistry.Create(indoorItem.ItemId) as StardewValley.Object;
      Furniture furniture = @object as Furniture;
      if (@object != null)
      {
        if (indoorItem.ClearTile)
        {
          if (furniture != null)
          {
            int num1 = 0;
            for (int tilesHigh = furniture.getTilesHigh(); num1 < tilesHigh; ++num1)
            {
              int num2 = 0;
              for (int tilesWide = furniture.getTilesWide(); num2 < tilesWide; ++num2)
                indoors.cleanUpTileForMapOverride(new Point((int) vector2.X + num2, (int) vector2.Y + num1), indoorItem.ItemId);
            }
          }
          else
            indoors.cleanUpTileForMapOverride(Utility.Vector2ToPoint(vector2), indoorItem.ItemId);
        }
        if (!indoors.IsTileBlockedBy(vector2, CollisionMask.Furniture | CollisionMask.Objects))
        {
          if (indoorItem.Indestructible)
            @object.fragility.Value = 2;
          @object.TileLocation = vector2;
          if (furniture != null)
            indoors.furniture.Add(furniture);
          else
            indoors.objects.Add(vector2, @object);
        }
      }
    }
  }

  public BuildingItemConversion GetItemConversionForItem(Item item, Chest chest)
  {
    if (item == null || chest == null)
      return (BuildingItemConversion) null;
    BuildingData data = this.GetData();
    if (data?.ItemConversions != null)
    {
      foreach (BuildingItemConversion itemConversion in data.ItemConversions)
      {
        if (itemConversion.SourceChest == chest.Name)
        {
          bool flag = false;
          foreach (string requiredTag in itemConversion.RequiredTags)
          {
            if (!item.HasContextTag(requiredTag))
            {
              flag = true;
              break;
            }
          }
          if (!flag)
            return itemConversion;
        }
      }
    }
    return (BuildingItemConversion) null;
  }

  public bool IsValidObjectForChest(Item item, Chest chest)
  {
    return this.GetItemConversionForItem(item, chest) != null;
  }

  public bool PerformBuildingChestAction(string name, Farmer who)
  {
    Chest chest = this.GetBuildingChest(name);
    if (chest == null)
      return false;
    BuildingChest buildingChestData = this.GetBuildingChestData(name);
    if (buildingChestData == null)
      return false;
    switch (buildingChestData.Type)
    {
      case BuildingChestType.Chest:
        ((MenuWithInventory) (Game1.activeClickableMenu = (IClickableMenu) new ItemGrabMenu((IList<Item>) chest.Items, false, true, (InventoryMenu.highlightThisItem) (item => this.IsValidObjectForChest(item, chest)), new ItemGrabMenu.behaviorOnItemSelect(chest.grabItemFromInventory), (string) null, new ItemGrabMenu.behaviorOnItemSelect(chest.grabItemFromChest), canBeExitedWithKey: true, showOrganizeButton: true, source: 1, context: (object) this))).inventory.moveItemSound = buildingChestData.Sound;
        return true;
      case BuildingChestType.Collect:
        Utility.CollectSingleItemOrShowChestMenu(chest);
        return true;
      case BuildingChestType.Load:
        if (who?.ActiveObject != null)
        {
          if (!this.IsValidObjectForChest((Item) who.ActiveObject, chest))
          {
            if (buildingChestData.InvalidItemMessage != null && (buildingChestData.InvalidItemMessageCondition == null || GameStateQuery.CheckConditions(buildingChestData.InvalidItemMessageCondition, this.GetParentLocation(), who, (Item) who.ActiveObject, (Item) who.ActiveObject)))
              Game1.showRedMessage(TokenParser.ParseText(buildingChestData.InvalidItemMessage));
            return false;
          }
          BuildingItemConversion conversionForItem = this.GetItemConversionForItem((Item) who.ActiveObject, chest);
          Utility.consolidateStacks((IList<Item>) chest.Items);
          chest.clearNulls();
          int thisInventoryList = Utility.GetNumberOfItemThatCanBeAddedToThisInventoryList((Item) who.ActiveObject, (IList<Item>) chest.Items, 36);
          if (who.ActiveObject.Stack > conversionForItem.RequiredCount && thisInventoryList < conversionForItem.RequiredCount)
          {
            Game1.showRedMessage(TokenParser.ParseText(buildingChestData.ChestFullMessage));
            return false;
          }
          int amount = Math.Min(thisInventoryList, who.ActiveObject.Stack) / conversionForItem.RequiredCount * conversionForItem.RequiredCount;
          if (amount == 0)
          {
            if (buildingChestData.InvalidCountMessage != null)
              Game1.showRedMessage(TokenParser.ParseText(buildingChestData.InvalidCountMessage));
            return false;
          }
          Item one = who.ActiveObject.getOne();
          if (who.ActiveObject.ConsumeStack(amount) == null)
            who.ActiveObject = (StardewValley.Object) null;
          one.Stack = amount;
          Utility.addItemToThisInventoryList(one, (IList<Item>) chest.Items, 36);
          if (buildingChestData.Sound != null)
            Game1.playSound(buildingChestData.Sound);
        }
        return true;
      default:
        return false;
    }
  }

  public BuildingChest GetBuildingChestData(string name)
  {
    return Building.GetBuildingChestData(this.GetData(), name);
  }

  public static BuildingChest GetBuildingChestData(BuildingData data, string name)
  {
    if (data == null)
      return (BuildingChest) null;
    foreach (BuildingChest chest in data.Chests)
    {
      if (chest.Id == name)
        return chest;
    }
    return (BuildingChest) null;
  }

  public Chest GetBuildingChest(string name)
  {
    foreach (Chest buildingChest in this.buildingChests)
    {
      if (buildingChest.Name == name)
        return buildingChest;
    }
    return (Chest) null;
  }

  public virtual string textureName()
  {
    BuildingData data = this.GetData();
    return Building.GetSkin(this.skinId.Value, data)?.Texture ?? data?.Texture ?? "Buildings\\" + this.buildingType.Value;
  }

  public virtual void resetTexture()
  {
    this.texture = new Lazy<Texture2D>((Func<Texture2D>) (() =>
    {
      if (this.paintedTexture != null)
      {
        this.paintedTexture.Dispose();
        this.paintedTexture = (Texture2D) null;
      }
      string assetName = this.textureName();
      Texture2D base_texture;
      try
      {
        base_texture = Game1.content.Load<Texture2D>(assetName);
      }
      catch
      {
        return Game1.content.Load<Texture2D>("Buildings\\Error");
      }
      this.paintedTexture = BuildingPainter.Apply(base_texture, assetName + "_PaintMask", this.netBuildingPaintColor.Value);
      if (this.paintedTexture != null)
        base_texture = this.paintedTexture;
      return base_texture;
    }));
  }

  public int getTileSheetIndexForStructurePlacementTile(int x, int y)
  {
    if (x == this.humanDoor.X && y == this.humanDoor.Y)
      return 2;
    return x == this.animalDoor.X && y == this.animalDoor.Y ? 4 : 0;
  }

  public virtual void performTenMinuteAction(int timeElapsed)
  {
  }

  public virtual void resetLocalState()
  {
    this.alpha = 1f;
    this.color = Color.White;
    this.isMoving = false;
  }

  public virtual bool CanLeftClick(int x, int y) => this.intersects(new Microsoft.Xna.Framework.Rectangle(x, y, 1, 1));

  public virtual bool leftClicked() => false;

  public virtual void ToggleAnimalDoor(Farmer who)
  {
    BuildingData data = this.GetData();
    string audioName = this.animalDoorOpen.Value ? data?.AnimalDoorOpenSound : data?.AnimalDoorCloseSound;
    if (audioName != null)
      who.currentLocation.playSound(audioName);
    this.animalDoorOpen.Value = !this.animalDoorOpen.Value;
  }

  public virtual bool OnUseHumanDoor(Farmer who) => true;

  public virtual bool doAction(Vector2 tileLocation, Farmer who)
  {
    if (who.isRidingHorse())
      return false;
    if (who.IsLocalPlayer && this.occupiesTile(tileLocation) && this.daysOfConstructionLeft.Value > 0)
    {
      Game1.drawObjectDialogue(Game1.content.LoadString("Strings\\Buildings:UnderConstruction"));
    }
    else
    {
      if (who.ActiveObject != null && who.ActiveObject.IsFloorPathItem() && who.currentLocation != null && !who.currentLocation.terrainFeatures.ContainsKey(tileLocation))
        return false;
      GameLocation indoors = this.GetIndoors();
      if (who.IsLocalPlayer && (double) tileLocation.X == (double) (this.humanDoor.X + this.tileX.Value) && (double) tileLocation.Y == (double) (this.humanDoor.Y + this.tileY.Value) && indoors != null)
      {
        if (who.mount != null)
        {
          Game1.showRedMessage(Game1.content.LoadString("Strings\\Buildings:DismountBeforeEntering"));
          return false;
        }
        if (who.team.demolishLock.IsLocked())
        {
          Game1.showRedMessage(Game1.content.LoadString("Strings\\Buildings:CantEnter"));
          return false;
        }
        if (this.OnUseHumanDoor(who))
        {
          who.currentLocation.playSound("doorClose", new Vector2?(tileLocation));
          bool isStructure = this.indoors.Value != null;
          Game1.warpFarmer(indoors.NameOrUniqueName, indoors.warps[0].X, indoors.warps[0].Y - 1, Game1.player.FacingDirection, isStructure);
        }
        return true;
      }
      BuildingData data = this.GetData();
      if (data != null)
      {
        Microsoft.Xna.Framework.Rectangle rectForAnimalDoor = this.getRectForAnimalDoor(data);
        rectForAnimalDoor.Width /= 64 /*0x40*/;
        rectForAnimalDoor.Height /= 64 /*0x40*/;
        rectForAnimalDoor.X /= 64 /*0x40*/;
        rectForAnimalDoor.Y /= 64 /*0x40*/;
        if (this.daysOfConstructionLeft.Value <= 0 && rectForAnimalDoor != Microsoft.Xna.Framework.Rectangle.Empty && rectForAnimalDoor.Contains(Utility.Vector2ToPoint(tileLocation)) && Game1.didPlayerJustRightClick(true))
        {
          this.ToggleAnimalDoor(who);
          return true;
        }
        if (who.IsLocalPlayer && this.occupiesTile(tileLocation, true) && !this.isTilePassable(tileLocation))
        {
          string actionAtTile = data.GetActionAtTile((int) tileLocation.X - this.tileX.Value, (int) tileLocation.Y - this.tileY.Value);
          if (actionAtTile != null)
          {
            string text = TokenParser.ParseText(actionAtTile);
            if (who.currentLocation.performAction(text, who, new Location((int) tileLocation.X, (int) tileLocation.Y)))
              return true;
          }
        }
      }
      else if (who.IsLocalPlayer)
      {
        if (!this.isTilePassable(tileLocation) && Building.TryPerformObeliskWarp(this.buildingType.Value, who))
          return true;
        if (who.ActiveObject != null && !this.isTilePassable(tileLocation))
          return this.performActiveObjectDropInAction(who, false);
      }
    }
    return false;
  }

  public static bool TryPerformObeliskWarp(string buildingType, Farmer who)
  {
    switch (buildingType)
    {
      case "Desert Obelisk":
        Building.PerformObeliskWarp("Desert", 35, 43, true, who);
        return true;
      case "Water Obelisk":
        Building.PerformObeliskWarp("Beach", 20, 4, false, who);
        return true;
      case "Earth Obelisk":
        Building.PerformObeliskWarp("Mountain", 31 /*0x1F*/, 20, false, who);
        return true;
      case "Island Obelisk":
        Building.PerformObeliskWarp("IslandSouth", 11, 11, false, who);
        return true;
      default:
        return false;
    }
  }

  public static void PerformObeliskWarp(
    string destination,
    int warp_x,
    int warp_y,
    bool force_dismount,
    Farmer who)
  {
    if (force_dismount && who.isRidingHorse() && who.mount != null)
    {
      who.mount.checkAction(who, who.currentLocation);
    }
    else
    {
      for (int index = 0; index < 12; ++index)
        who.currentLocation.temporarySprites.Add(new TemporaryAnimatedSprite(354, (float) Game1.random.Next(25, 75), 6, 1, new Vector2((float) Game1.random.Next((int) who.Position.X - 256 /*0x0100*/, (int) who.Position.X + 192 /*0xC0*/), (float) Game1.random.Next((int) who.Position.Y - 256 /*0x0100*/, (int) who.Position.Y + 192 /*0xC0*/)), false, Game1.random.NextBool()));
      who.currentLocation.playSound("wand");
      Game1.displayFarmer = false;
      Game1.player.temporarilyInvincible = true;
      Game1.player.temporaryInvincibilityTimer = -2000;
      Game1.player.freezePause = 1000;
      Game1.flashAlpha = 1f;
      Microsoft.Xna.Framework.Rectangle boundingBox = who.GetBoundingBox();
      DelayedAction.fadeAfterDelay((Game1.afterFadeFunction) (() => Building.obeliskWarpForReal(destination, warp_x, warp_y, who)), 1000);
      new Microsoft.Xna.Framework.Rectangle(boundingBox.X, boundingBox.Y, 64 /*0x40*/, 64 /*0x40*/).Inflate(192 /*0xC0*/, 192 /*0xC0*/);
      int num = 0;
      Point tilePoint = who.TilePoint;
      for (int x = tilePoint.X + 8; x >= tilePoint.X - 8; --x)
      {
        who.currentLocation.temporarySprites.Add(new TemporaryAnimatedSprite(6, new Vector2((float) x, (float) tilePoint.Y) * 64f, Color.White, animationInterval: 50f)
        {
          layerDepth = 1f,
          delayBeforeAnimationStart = num * 25,
          motion = new Vector2(-0.25f, 0.0f)
        });
        ++num;
      }
    }
  }

  private static void obeliskWarpForReal(string destination, int warp_x, int warp_y, Farmer who)
  {
    Game1.warpFarmer(destination, warp_x, warp_y, false);
    Game1.fadeToBlackAlpha = 0.99f;
    Game1.screenGlow = false;
    Game1.player.temporarilyInvincible = false;
    Game1.player.temporaryInvincibilityTimer = 0;
    Game1.displayFarmer = true;
  }

  public virtual bool isActionableTile(int xTile, int yTile, Farmer who)
  {
    BuildingData data = this.GetData();
    if (data != null)
    {
      Vector2 tile = new Vector2((float) xTile, (float) yTile);
      if (this.occupiesTile(tile, true) && !this.isTilePassable(tile) && data.GetActionAtTile(xTile - this.tileX.Value, yTile - this.tileY.Value) != null)
        return true;
    }
    if (this.humanDoor.X >= 0 && xTile == this.tileX.Value + this.humanDoor.X && yTile == this.tileY.Value + this.humanDoor.Y)
      return true;
    Microsoft.Xna.Framework.Rectangle rectForAnimalDoor = this.getRectForAnimalDoor(data);
    rectForAnimalDoor.Width /= 64 /*0x40*/;
    rectForAnimalDoor.Height /= 64 /*0x40*/;
    rectForAnimalDoor.X /= 64 /*0x40*/;
    rectForAnimalDoor.Y /= 64 /*0x40*/;
    return rectForAnimalDoor != Microsoft.Xna.Framework.Rectangle.Empty && rectForAnimalDoor.Contains(new Point(xTile, yTile));
  }

  /// <summary>Handle the building being moved within its location by any player.</summary>
  public virtual void performActionOnBuildingPlacement()
  {
    GameLocation parentLocation = this.GetParentLocation();
    if (parentLocation == null)
      return;
    for (int index1 = 0; index1 < this.tilesHigh.Value; ++index1)
    {
      for (int index2 = 0; index2 < this.tilesWide.Value; ++index2)
      {
        Vector2 key = new Vector2((float) (this.tileX.Value + index2), (float) (this.tileY.Value + index1));
        if (parentLocation.terrainFeatures.GetValueOrDefault(key) is Flooring)
        {
          bool? flooringUnderneath = this.GetData()?.AllowsFlooringUnderneath;
          if (flooringUnderneath.HasValue && flooringUnderneath.GetValueOrDefault())
            continue;
        }
        parentLocation.terrainFeatures.Remove(key);
      }
    }
    foreach (BuildingPlacementTile additionalPlacementTile in this.GetAdditionalPlacementTiles())
    {
      bool needsToBePassable = additionalPlacementTile.OnlyNeedsToBePassable;
      foreach (Point point in additionalPlacementTile.TileArea.GetPoints())
      {
        Vector2 key = new Vector2((float) (this.tileX.Value + point.X), (float) (this.tileY.Value + point.Y));
        TerrainFeature terrainFeature;
        if (!needsToBePassable || parentLocation.terrainFeatures.TryGetValue(key, out terrainFeature) && !terrainFeature.isPassable())
        {
          if (parentLocation.terrainFeatures.GetValueOrDefault(key) is Flooring)
          {
            bool? flooringUnderneath = this.GetData()?.AllowsFlooringUnderneath;
            if (flooringUnderneath.HasValue && flooringUnderneath.GetValueOrDefault())
              continue;
          }
          parentLocation.terrainFeatures.Remove(key);
        }
      }
    }
  }

  /// <summary>Handle the building being constructed.</summary>
  /// <param name="location">The location containing the building.</param>
  /// <param name="who">The player that constructed the building.</param>
  public virtual void performActionOnConstruction(GameLocation location, Farmer who)
  {
    BuildingData data = this.GetData();
    this.LoadFromBuildingData(data, forConstruction: true);
    Vector2 vector2 = new Vector2((float) this.tileX.Value + (float) this.tilesWide.Value * 0.5f, (float) this.tileY.Value + (float) this.tilesHigh.Value * 0.5f);
    location.localSound("axchop", new Vector2?(vector2));
    this.newConstructionTimer.Value = this.magical.Value || this.daysOfConstructionLeft.Value <= 0 ? 2000 : 1000;
    if (data?.AddMailOnBuild != null)
    {
      foreach (string mailName in data.AddMailOnBuild)
        Game1.addMail(mailName, sendToEveryone: true);
    }
    if (!this.magical.Value)
    {
      location.localSound("axchop", new Vector2?(vector2));
      for (int x = this.tileX.Value; x < this.tileX.Value + this.tilesWide.Value; ++x)
      {
        for (int y = this.tileY.Value; y < this.tileY.Value + this.tilesHigh.Value; ++y)
        {
          for (int index = 0; index < 5; ++index)
            location.temporarySprites.Add(new TemporaryAnimatedSprite(Game1.random.Choose<int>(46, 12), new Vector2((float) x, (float) y) * 64f + new Vector2((float) Game1.random.Next(-16, 32 /*0x20*/), (float) Game1.random.Next(-16, 32 /*0x20*/)), Color.White, 10, Game1.random.NextBool())
            {
              delayBeforeAnimationStart = Math.Max(0, Game1.random.Next(-200, 400)),
              motion = new Vector2(0.0f, -1f),
              interval = (float) Game1.random.Next(50, 80 /*0x50*/)
            });
          location.temporarySprites.Add(new TemporaryAnimatedSprite(14, new Vector2((float) x, (float) y) * 64f + new Vector2((float) Game1.random.Next(-16, 32 /*0x20*/), (float) Game1.random.Next(-16, 32 /*0x20*/)), Color.White, 10, Game1.random.NextBool()));
        }
      }
      for (int index = 0; index < 8; ++index)
        DelayedAction.playSoundAfterDelay("dirtyHit", 250 + index * 150, location, new Vector2?(vector2), local: true);
    }
    else
    {
      for (int index = 0; index < 8; ++index)
        DelayedAction.playSoundAfterDelay("dirtyHit", 100 + index * 210, location, new Vector2?(vector2), local: true);
      if (Game1.player == who)
        Game1.flashAlpha = 2f;
      location.localSound("wand", new Vector2?(vector2));
      Microsoft.Xna.Framework.Rectangle sourceRect = this.getSourceRect();
      Microsoft.Xna.Framework.Rectangle rectangle = this.getSourceRectForMenu() ?? sourceRect;
      int num1 = 0;
      for (int index1 = sourceRect.Height / 16 /*0x10*/ * 2; num1 <= index1; ++num1)
      {
        int num2 = 0;
        for (int index2 = rectangle.Width / 16 /*0x10*/ * 2; num2 < index2; ++num2)
        {
          location.temporarySprites.Add(new TemporaryAnimatedSprite("LooseSprites\\Cursors", new Microsoft.Xna.Framework.Rectangle(666, 1851, 8, 8), 40f, 4, 2, new Vector2((float) this.tileX.Value, (float) this.tileY.Value) * 64f + new Vector2((float) (num2 * 64 /*0x40*/ / 2), (float) (num1 * 64 /*0x40*/ / 2 - sourceRect.Height * 4 + this.tilesHigh.Value * 64 /*0x40*/)) + new Vector2((float) Game1.random.Next(-32, 32 /*0x20*/), (float) Game1.random.Next(-32, 32 /*0x20*/)), false, false)
          {
            layerDepth = (float) ((double) ((this.tileY.Value + this.tilesHigh.Value) * 64 /*0x40*/) / 10000.0 + (double) num2 / 10000.0),
            pingPong = true,
            delayBeforeAnimationStart = (sourceRect.Height / 16 /*0x10*/ * 2 - num1) * 100,
            scale = 4f,
            alphaFade = 0.01f,
            color = Color.AliceBlue
          });
          location.temporarySprites.Add(new TemporaryAnimatedSprite("LooseSprites\\Cursors", new Microsoft.Xna.Framework.Rectangle(666, 1851, 8, 8), 40f, 4, 2, new Vector2((float) this.tileX.Value, (float) this.tileY.Value) * 64f + new Vector2((float) (num2 * 64 /*0x40*/ / 2), (float) (num1 * 64 /*0x40*/ / 2 - sourceRect.Height * 4 + this.tilesHigh.Value * 64 /*0x40*/)) + new Vector2((float) Game1.random.Next(-32, 32 /*0x20*/), (float) Game1.random.Next(-32, 32 /*0x20*/)), false, false)
          {
            layerDepth = (float) ((double) ((this.tileY.Value + this.tilesHigh.Value) * 64 /*0x40*/) / 10000.0 + (double) num2 / 10000.0 + 9.9999997473787516E-05),
            pingPong = true,
            delayBeforeAnimationStart = (sourceRect.Height / 16 /*0x10*/ * 2 - num1) * 100,
            scale = 4f,
            alphaFade = 0.01f,
            color = Color.AliceBlue
          });
        }
      }
    }
    if (!(this.GetIndoors() is Cabin indoors) || indoors.HasOwner)
      return;
    indoors.CreateFarmhand();
    if (!Game1.IsMasterGame)
      return;
    this.hasLoaded = true;
  }

  /// <summary>Handle the building being demolished.</summary>
  /// <param name="location">The location which previously contained the building.</param>
  public virtual void performActionOnDemolition(GameLocation location)
  {
    if (this.GetIndoors() is Cabin indoors)
      indoors.DeleteFarmhand();
    if (this.indoors.Value == null)
      return;
    Game1.multiplayer.broadcastRemoveLocationFromLookup(this.indoors.Value);
    this.indoors.Value.OnRemoved();
    this.indoors.Value = (GameLocation) null;
  }

  /// <summary>Perform an action for each item within the building instance, excluding those in the interior location.</summary>
  /// <param name="action">The action to perform for each item.  This should return true (continue iterating) or false (stop).</param>
  /// <returns>Returns whether to continue iterating.</returns>
  /// <remarks>For items in the interior location, use <see cref="M:StardewValley.Utility.ForEachItemIn(StardewValley.GameLocation,System.Func{StardewValley.Item,System.Boolean})" /> instead. See also <see cref="M:StardewValley.Buildings.Building.ForEachItemContextExcludingInterior(StardewValley.Delegates.ForEachItemDelegate,StardewValley.Delegates.GetForEachItemPathDelegate)" /> for more advanced scenarios like replacing items.</remarks>
  public virtual bool ForEachItemExcludingInterior(Func<Item, bool> action)
  {
    return this.ForEachItemContextExcludingInterior(new ForEachItemDelegate(Handle), new GetForEachItemPathDelegate(GetParentPath));

    bool Handle(in ForEachItemContext context) => action(context.Item);

    IList<object> GetParentPath()
    {
      return (IList<object>) new List<object>()
      {
        (object) this.GetParentLocation()
      };
    }
  }

  /// <summary>Perform an action for each item within the building instance, excluding those in the interior location.</summary>
  /// <param name="handler">The action to perform for each item.</param>
  /// <param name="getParentPath">The contextual path leading to this building (excluding the building itself), or <c>null</c> to treat this as the root.</param>
  /// <returns>Returns whether to continue iterating.</returns>
  /// <remarks>For items in the interior location, use <see cref="M:StardewValley.Utility.ForEachItemIn(StardewValley.GameLocation,System.Func{StardewValley.Item,System.Boolean})" /> instead. See also <see cref="M:StardewValley.Buildings.Building.ForEachItemExcludingInterior(System.Func{StardewValley.Item,System.Boolean})" /> if you only need to iterate items.</remarks>
  public virtual bool ForEachItemContextExcludingInterior(
    ForEachItemDelegate handler,
    GetForEachItemPathDelegate getParentPath)
  {
    foreach (Chest buildingChest in this.buildingChests)
    {
      Chest chest = buildingChest;
      if (!chest.ForEachItem(handler, new GetForEachItemPathDelegate(GetPath)))
        return false;

      IList<object> GetPath()
      {
        return ForEachItemHelper.CombinePath(getParentPath, (object) this, (object) this.buildingChests, (object) chest);
      }
    }
    return true;
  }

  public virtual void BeforeDemolish()
  {
    List<Item> quest_items = new List<Item>();
    this.ForEachItemExcludingInterior((Func<Item, bool>) (item =>
    {
      CollectQuestItem(item);
      return true;
    }));
    if (this.indoors.Value != null)
    {
      Utility.ForEachItemIn(this.indoors.Value, (Func<Item, bool>) (item =>
      {
        CollectQuestItem(item);
        return true;
      }));
      if (this.indoors.Value is Cabin cabin)
      {
        Cellar cellar = cabin.GetCellar();
        if (cellar != null)
          Utility.ForEachItemIn((GameLocation) cellar, (Func<Item, bool>) (item =>
          {
            CollectQuestItem(item);
            return true;
          }));
      }
    }
    if (quest_items.Count <= 0)
      return;
    Game1.showGlobalMessage(Game1.content.LoadString("Strings\\StringsFromCSFiles:NewLostAndFoundItems"));
    for (int index = 0; index < quest_items.Count; ++index)
      Game1.player.team.returnedDonations.Add(quest_items[index]);

    void CollectQuestItem(Item item)
    {
      if (!(item is StardewValley.Object @object) || !@object.questItem.Value)
        return;
      Item one = @object.getOne();
      one.Stack = @object.Stack;
      quest_items.Add(one);
    }
  }

  public virtual void performActionOnUpgrade(GameLocation location)
  {
    if (!(location is Farm farm))
      return;
    farm.UnsetFarmhouseValues();
  }

  public virtual string isThereAnythingtoPreventConstruction(
    GameLocation location,
    Vector2 tile_location)
  {
    return (string) null;
  }

  public virtual bool performActiveObjectDropInAction(Farmer who, bool probe) => false;

  public virtual void performToolAction(Tool t, int tileX, int tileY)
  {
  }

  public virtual void updateWhenFarmNotCurrentLocation(GameTime time)
  {
    if (this.indoors.Value != null && Game1.currentLocation != this.indoors.Value)
      this.indoors.Value.netAudio.Update();
    this.netBuildingPaintColor.Value?.Poll(new Action(this.resetTexture));
    if (this.newConstructionTimer.Value > 0)
    {
      this.newConstructionTimer.Value -= time.ElapsedGameTime.Milliseconds;
      if (this.newConstructionTimer.Value <= 0 && this.magical.Value)
        this.daysOfConstructionLeft.Value = 0;
    }
    if (!Game1.IsMasterGame)
      return;
    BuildingData data = this.GetData();
    if (data == null)
      return;
    if (this.animalDoorOpen.Value)
    {
      if ((double) this.animalDoorOpenAmount.Value >= 1.0)
        return;
      this.animalDoorOpenAmount.Value = (double) data.AnimalDoorOpenDuration > 0.0 ? Utility.MoveTowards(this.animalDoorOpenAmount.Value, 1f, (float) time.ElapsedGameTime.TotalSeconds / data.AnimalDoorOpenDuration) : 1f;
    }
    else
    {
      if ((double) this.animalDoorOpenAmount.Value <= 0.0)
        return;
      this.animalDoorOpenAmount.Value = (double) data.AnimalDoorCloseDuration > 0.0 ? Utility.MoveTowards(this.animalDoorOpenAmount.Value, 0.0f, (float) time.ElapsedGameTime.TotalSeconds / data.AnimalDoorCloseDuration) : 0.0f;
    }
  }

  public virtual void Update(GameTime time)
  {
    if (!this.hasLoaded && Game1.IsMasterGame && Game1.hasLoadedGame)
    {
      this.ReloadBuildingData(forConstruction: true);
      this.load();
    }
    this.UpdateTransparency();
    if (this.isUnderConstruction())
      return;
    if (!this.hasChimney.HasValue)
    {
      string metadata = this.GetMetadata("ChimneyPosition");
      if (metadata != null)
      {
        this.hasChimney = new bool?(true);
        string[] strArray = ArgUtility.SplitBySpace(metadata);
        this.chimneyPosition.X = (float) int.Parse(strArray[0]);
        this.chimneyPosition.Y = (float) int.Parse(strArray[1]);
      }
      else
        this.hasChimney = new bool?(false);
    }
    GameLocation indoors = this.GetIndoors();
    if (indoors is FarmHouse farmHouse)
    {
      int upgradeLevel = farmHouse.upgradeLevel;
      if (this.lastHouseUpgradeLevel != upgradeLevel)
      {
        this.lastHouseUpgradeLevel = upgradeLevel;
        string str = (string) null;
        for (int index = 1; index <= this.lastHouseUpgradeLevel; ++index)
        {
          string metadata = this.GetMetadata("ChimneyPosition" + (index + 1).ToString());
          if (metadata != null)
            str = metadata;
        }
        if (str != null)
        {
          this.hasChimney = new bool?(true);
          string[] strArray = ArgUtility.SplitBySpace(str);
          this.chimneyPosition.X = (float) int.Parse(strArray[0]);
          this.chimneyPosition.Y = (float) int.Parse(strArray[1]);
        }
      }
    }
    if (!this.hasChimney.GetValueOrDefault() || indoors == null)
      return;
    this.chimneyTimer -= time.ElapsedGameTime.Milliseconds;
    if (this.chimneyTimer > 0)
      return;
    if (indoors.hasActiveFireplace())
    {
      GameLocation parentLocation = this.GetParentLocation();
      Vector2 vector2_1 = new Vector2((float) (this.tileX.Value * 64 /*0x40*/), (float) (this.tileY.Value * 64 /*0x40*/ + this.tilesHigh.Value * 64 /*0x40*/ - this.getSourceRect().Height * 4));
      BuildingData data = this.GetData();
      Vector2 vector2_2 = data != null ? data.DrawOffset * 4f : Vector2.Zero;
      TemporaryAnimatedSprite temporaryAnimatedSprite = TemporaryAnimatedSprite.GetTemporaryAnimatedSprite("LooseSprites\\Cursors", new Microsoft.Xna.Framework.Rectangle(372, 1956, 10, 10), new Vector2(vector2_1.X + vector2_2.X, vector2_1.Y + vector2_2.Y) + this.chimneyPosition * 4f + new Vector2(-8f, -12f), false, 1f / 500f, Color.Gray);
      temporaryAnimatedSprite.alpha = 0.75f;
      temporaryAnimatedSprite.motion = new Vector2(0.0f, -0.5f);
      temporaryAnimatedSprite.acceleration = new Vector2(1f / 500f, 0.0f);
      temporaryAnimatedSprite.interval = 99999f;
      temporaryAnimatedSprite.layerDepth = 1f;
      temporaryAnimatedSprite.scale = 2f;
      temporaryAnimatedSprite.scaleChange = 0.02f;
      temporaryAnimatedSprite.rotationChange = (float) ((double) Game1.random.Next(-5, 6) * 3.1415927410125732 / 256.0);
      parentLocation.temporarySprites.Add(temporaryAnimatedSprite);
    }
    this.chimneyTimer = 500;
  }

  /// <summary>Update the building transparency on tick for the local player's position.</summary>
  public virtual void UpdateTransparency()
  {
    if (this.fadeWhenPlayerIsBehind.Value)
    {
      Microsoft.Xna.Framework.Rectangle rectangle1 = this.getSourceRectForMenu() ?? this.getSourceRect();
      Microsoft.Xna.Framework.Rectangle rectangle2 = new Microsoft.Xna.Framework.Rectangle(this.tileX.Value * 64 /*0x40*/, (this.tileY.Value + (-(rectangle1.Height / 16 /*0x10*/) + this.tilesHigh.Value)) * 64 /*0x40*/, this.tilesWide.Value * 64 /*0x40*/, (rectangle1.Height / 16 /*0x10*/ - this.tilesHigh.Value) * 64 /*0x40*/ + 32 /*0x20*/);
      if (Game1.player.GetBoundingBox().Intersects(rectangle2))
      {
        if ((double) this.alpha <= 0.40000000596046448)
          return;
        this.alpha = Math.Max(0.4f, this.alpha - 0.04f);
        return;
      }
    }
    if ((double) this.alpha >= 1.0)
      return;
    this.alpha = Math.Min(1f, this.alpha + 0.05f);
  }

  public virtual void showUpgradeAnimation(GameLocation location)
  {
    this.color = Color.White;
    location.temporarySprites.Add(new TemporaryAnimatedSprite(46, this.getUpgradeSignLocation() + new Vector2((float) Game1.random.Next(-16, 16 /*0x10*/), (float) Game1.random.Next(-16, 16 /*0x10*/)), Color.Beige, 10, Game1.random.NextBool(), 75f)
    {
      motion = new Vector2(0.0f, -0.5f),
      acceleration = new Vector2(-0.02f, 0.01f),
      delayBeforeAnimationStart = Game1.random.Next(100),
      layerDepth = 0.89f
    });
    location.temporarySprites.Add(new TemporaryAnimatedSprite(46, this.getUpgradeSignLocation() + new Vector2((float) Game1.random.Next(-16, 16 /*0x10*/), (float) Game1.random.Next(-16, 16 /*0x10*/)), Color.Beige, 10, Game1.random.NextBool(), 75f)
    {
      motion = new Vector2(0.0f, -0.5f),
      acceleration = new Vector2(-0.02f, 0.01f),
      delayBeforeAnimationStart = Game1.random.Next(40),
      layerDepth = 0.89f
    });
  }

  public virtual Vector2 getUpgradeSignLocation()
  {
    BuildingData data = this.GetData();
    Vector2 vector2 = data != null ? data.UpgradeSignTile : new Vector2(0.5f, 0.0f);
    float num = data != null ? data.UpgradeSignHeight : 8f;
    return new Vector2((float) (((double) this.tileX.Value + (double) vector2.X) * 64.0), (float) (((double) this.tileY.Value + (double) vector2.Y) * 64.0 - (double) num * 4.0));
  }

  public virtual void showDestroyedAnimation(GameLocation location)
  {
    for (int x = this.tileX.Value; x < this.tileX.Value + this.tilesWide.Value; ++x)
    {
      for (int y = this.tileY.Value; y < this.tileY.Value + this.tilesHigh.Value; ++y)
      {
        location.temporarySprites.Add(new TemporaryAnimatedSprite(362, (float) Game1.random.Next(30, 90), 6, 1, new Vector2((float) (x * 64 /*0x40*/), (float) (y * 64 /*0x40*/)) + new Vector2((float) Game1.random.Next(-16, 16 /*0x10*/), (float) Game1.random.Next(-16, 16 /*0x10*/)), false, Game1.random.NextBool())
        {
          delayBeforeAnimationStart = Game1.random.Next(300)
        });
        location.temporarySprites.Add(new TemporaryAnimatedSprite(362, (float) Game1.random.Next(30, 90), 6, 1, new Vector2((float) (x * 64 /*0x40*/), (float) (y * 64 /*0x40*/)) + new Vector2((float) Game1.random.Next(-16, 16 /*0x10*/), (float) Game1.random.Next(-16, 16 /*0x10*/)), false, Game1.random.NextBool())
        {
          delayBeforeAnimationStart = 250 + Game1.random.Next(300)
        });
        location.temporarySprites.Add(new TemporaryAnimatedSprite("LooseSprites\\Cursors", new Microsoft.Xna.Framework.Rectangle(276, 1985, 12, 11), new Vector2((float) x, (float) y) * 64f + new Vector2(32f, -32f) + new Vector2((float) Game1.random.Next(-32, 32 /*0x20*/), (float) Game1.random.Next(-16, 16 /*0x10*/)), false, 0.0f, Color.White)
        {
          interval = 30f,
          totalNumberOfLoops = 99999,
          animationLength = 4,
          scale = 4f,
          alphaFade = 0.01f
        });
      }
    }
  }

  /// <summary>Instantly finish constructing or upgrading the building, if applicable.</summary>
  public void FinishConstruction(bool onGameStart = false)
  {
    bool flag = false;
    if (this.daysOfConstructionLeft.Value > 0)
    {
      Game1.player.team.constructedBuildings.Add(this.buildingType.Value);
      if (this.buildingType.Value == "Slime Hutch")
        Game1.player.mailReceived.Add("slimeHutchBuilt");
      this.daysOfConstructionLeft.Value = 0;
      flag = true;
    }
    if (this.daysUntilUpgrade.Value > 0)
    {
      string str = this.upgradeName.Value ?? "Well";
      Game1.player.team.constructedBuildings.Add(str);
      this.buildingType.Value = str;
      this.ReloadBuildingData(true);
      this.daysUntilUpgrade.Value = 0;
      this.OnUpgraded();
      flag = true;
    }
    if (flag)
    {
      Game1.netWorldState.Value.UpdateUnderConstruction();
      this.resetTexture();
    }
    if (onGameStart)
      return;
    foreach (Farmer allFarmer in Game1.getAllFarmers())
      allFarmer.autoGenerateActiveDialogueEvent("structureBuilt_" + this.buildingType.Value);
  }

  public virtual void dayUpdate(int dayOfMonth)
  {
    if (this.daysOfConstructionLeft.Value > 0 && !Utility.isFestivalDay(dayOfMonth, Game1.season) && (!Game1.isGreenRain || Game1.year > 1))
    {
      if (this.daysOfConstructionLeft.Value == 1)
        this.FinishConstruction();
      else
        --this.daysOfConstructionLeft.Value;
    }
    else
    {
      if (this.daysUntilUpgrade.Value > 0 && !Utility.isFestivalDay(dayOfMonth, Game1.season) && (!Game1.isGreenRain || Game1.year > 1))
      {
        if (this.daysUntilUpgrade.Value == 1)
          this.FinishConstruction();
        else
          --this.daysUntilUpgrade.Value;
      }
      GameLocation indoors = this.GetIndoors();
      if (indoors is AnimalHouse animalHouse)
        this.currentOccupants.Value = animalHouse.animals.Length;
      if (this.GetIndoorsType() == IndoorsType.Instanced && indoors != null)
        indoors.DayUpdate(dayOfMonth);
      BuildingData data = this.GetData();
      if (data == null)
        return;
      int? count = data.ItemConversions?.Count;
      int num = 0;
      if (!(count.GetValueOrDefault() > num & count.HasValue))
        return;
      ItemQueryContext itemQueryContext = new ItemQueryContext(this.GetParentLocation(), (Farmer) null, (Random) null, $"building '{this.buildingType.Value}' > item conversion rules");
      foreach (BuildingItemConversion itemConversion in data.ItemConversions)
        this.CheckItemConversionRule(itemConversion, itemQueryContext);
    }
  }

  public virtual void CheckItemConversionRule(
    BuildingItemConversion conversion,
    ItemQueryContext itemQueryContext)
  {
    int num1 = 0;
    int num2 = 0;
    Chest buildingChest1 = this.GetBuildingChest(conversion.SourceChest);
    Chest buildingChest2 = this.GetBuildingChest(conversion.DestinationChest);
    if (buildingChest1 == null)
      return;
    foreach (Item obj in buildingChest1.Items)
    {
      if (obj != null)
      {
        bool flag = false;
        foreach (string requiredTag in conversion.RequiredTags)
        {
          if (!obj.HasContextTag(requiredTag))
          {
            flag = true;
            break;
          }
        }
        if (!flag)
        {
          num2 += obj.Stack;
          if (num2 >= conversion.RequiredCount)
          {
            int val1 = num2 / conversion.RequiredCount;
            if (conversion.MaxDailyConversions >= 0)
              val1 = Math.Min(val1, conversion.MaxDailyConversions - num1);
            num1 += val1;
            num2 -= val1 * conversion.RequiredCount;
          }
          if (conversion.MaxDailyConversions >= 0)
          {
            if (num1 >= conversion.MaxDailyConversions)
              break;
          }
        }
      }
    }
    if (num1 == 0)
      return;
    int num3 = 0;
    for (int index1 = 0; index1 < num1; ++index1)
    {
      bool flag = false;
      for (int index2 = 0; index2 < conversion.ProducedItems.Count; ++index2)
      {
        GenericSpawnItemDataWithCondition producedItem = conversion.ProducedItems[index2];
        if (GameStateQuery.CheckConditions(producedItem.Condition, this.GetParentLocation()))
        {
          Item obj1 = ItemQueryResolver.TryResolveRandomItem((ISpawnItemData) producedItem, itemQueryContext);
          int stack = obj1.Stack;
          Item obj2 = buildingChest2.addItem(obj1);
          if ((obj2 != null ? (obj2.Stack != stack ? 1 : 0) : 1) != 0)
            flag = true;
        }
      }
      if (flag)
        ++num3;
    }
    if (num3 <= 0)
      return;
    int val1_1 = num3 * conversion.RequiredCount;
    for (int index = 0; index < buildingChest1.Items.Count; ++index)
    {
      Item obj = buildingChest1.Items[index];
      if (obj != null)
      {
        bool flag = false;
        foreach (string requiredTag in conversion.RequiredTags)
        {
          if (!obj.HasContextTag(requiredTag))
          {
            flag = true;
            break;
          }
        }
        if (!flag)
        {
          int amount = Math.Min(val1_1, obj.Stack);
          buildingChest1.Items[index] = obj.ConsumeStack(amount);
          val1_1 -= amount;
          if (val1_1 <= 0)
            break;
        }
      }
    }
  }

  public virtual void OnUpgraded()
  {
    this.GetIndoors()?.OnParentBuildingUpgraded(this);
    BuildingData data = this.GetData();
    if (data?.AddMailOnBuild == null)
      return;
    foreach (string mailName in data.AddMailOnBuild)
      Game1.addMail(mailName, sendToEveryone: true);
  }

  public virtual Microsoft.Xna.Framework.Rectangle getSourceRect()
  {
    BuildingData data = this.GetData();
    if (data != null)
    {
      Microsoft.Xna.Framework.Rectangle sourceRect1 = data.SourceRect;
      if (sourceRect1 == Microsoft.Xna.Framework.Rectangle.Empty)
        return this.texture.Value.Bounds;
      GameLocation indoors = this.GetIndoors();
      if (indoors is FarmHouse farmHouse)
      {
        if (indoors is Cabin)
          sourceRect1.X += sourceRect1.Width * Math.Min(farmHouse.upgradeLevel, 2);
        else
          sourceRect1.Y += sourceRect1.Height * Math.Min(farmHouse.upgradeLevel, 2);
      }
      Microsoft.Xna.Framework.Rectangle sourceRect2 = this.ApplySourceRectOffsets(sourceRect1);
      if (this.buildingType.Value == "Greenhouse" && this.GetParentLocation() is Farm parentLocation && !parentLocation.greenhouseUnlocked.Value)
        sourceRect2.Y -= sourceRect2.Height;
      return sourceRect2;
    }
    return this.isCabin ? new Microsoft.Xna.Framework.Rectangle((this.GetIndoors() is Cabin indoors1 ? Math.Min(indoors1.upgradeLevel, 2) : 0) * 80 /*0x50*/, 0, 80 /*0x50*/, 112 /*0x70*/) : this.texture.Value.Bounds;
  }

  public virtual Microsoft.Xna.Framework.Rectangle ApplySourceRectOffsets(Microsoft.Xna.Framework.Rectangle source)
  {
    BuildingData data = this.GetData();
    if (data != null && data.SeasonOffset != Point.Zero)
    {
      int indexForLocation = Game1.GetSeasonIndexForLocation(this.GetParentLocation());
      source.X += data.SeasonOffset.X * indexForLocation;
      source.Y += data.SeasonOffset.Y * indexForLocation;
    }
    return source;
  }

  public virtual Microsoft.Xna.Framework.Rectangle? getSourceRectForMenu() => new Microsoft.Xna.Framework.Rectangle?();

  public virtual void updateInteriorWarps(GameLocation interior = null)
  {
    interior = interior ?? this.GetIndoors();
    if (interior == null)
      return;
    GameLocation parentLocation = this.GetParentLocation();
    foreach (Warp warp in (NetList<Warp, NetRef<Warp>>) interior.warps)
    {
      if ((warp.TargetName == "Farm" ? 1 : (parentLocation == null ? 0 : (warp.TargetName == parentLocation.NameOrUniqueName ? 1 : 0))) != 0)
      {
        warp.TargetName = parentLocation?.NameOrUniqueName ?? warp.TargetName;
        warp.TargetX = this.humanDoor.X + this.tileX.Value;
        warp.TargetY = this.humanDoor.Y + this.tileY.Value + 1;
      }
    }
  }

  /// <summary>Get whether the building has an interior location.</summary>
  public bool HasIndoors()
  {
    return this.indoors.Value != null || this.nonInstancedIndoorsName.Value != null;
  }

  /// <summary>Get whether the building has an interior location with the given unique name.</summary>
  /// <param name="name">The name to check.</param>
  public bool HasIndoorsName(string name)
  {
    return name != null && this.GetIndoorsName().EqualsIgnoreCase(name);
  }

  /// <summary>Get the unique name of the location within this building, if it's linked to an instanced or non-instanced interior.</summary>
  public string GetIndoorsName()
  {
    return this.indoors.Value?.NameOrUniqueName ?? this.nonInstancedIndoorsName.Value;
  }

  /// <summary>Get the type of indoors location this building has.</summary>
  public IndoorsType GetIndoorsType()
  {
    if (this.indoors.Value != null)
      return IndoorsType.Instanced;
    return this.nonInstancedIndoorsName.Value != null ? IndoorsType.Global : IndoorsType.None;
  }

  /// <summary>Get the location within this building, if it's linked to an instanced or non-instanced interior.</summary>
  public GameLocation GetIndoors()
  {
    if (this.indoors.Value != null)
      return this.indoors.Value;
    return this.nonInstancedIndoorsName.Value != null ? Game1.getLocationFromName(this.nonInstancedIndoorsName.Value) : (GameLocation) null;
  }

  protected virtual GameLocation createIndoors(BuildingData data, string nameOfIndoorsWithoutUnique)
  {
    GameLocation interior = (GameLocation) null;
    if (data != null && !string.IsNullOrEmpty(data.IndoorMap))
    {
      Type type = typeof (GameLocation);
      if (data.IndoorMapType != null)
      {
        Exception exception = (Exception) null;
        try
        {
          type = Type.GetType(data.IndoorMapType);
        }
        catch (Exception ex)
        {
          exception = ex;
        }
        if ((object) type == null || exception != null)
        {
          Game1.log.Error($"Error constructing interior type '{data.IndoorMapType}' for building '{this.buildingType.Value}'" + (exception != null ? "." : ": that type doesn't exist."));
          type = typeof (GameLocation);
        }
      }
      string str = "Maps\\" + data.IndoorMap;
      try
      {
        interior = (GameLocation) Activator.CreateInstance(type, (object) str, (object) this.buildingType.Value);
      }
      catch (Exception ex1)
      {
        try
        {
          interior = (GameLocation) Activator.CreateInstance(type, (object) str);
        }
        catch (Exception ex2)
        {
          Game1.log.Error($"Error trying to instantiate indoors for '{this.buildingType}'", ex2);
          interior = new GameLocation("Maps\\" + nameOfIndoorsWithoutUnique, this.buildingType.Value);
        }
      }
    }
    if (interior != null)
    {
      interior.uniqueName.Value = nameOfIndoorsWithoutUnique + GuidHelper.NewGuid().ToString();
      interior.IsFarm = true;
      interior.isStructure.Value = true;
      interior.ParentBuilding = this;
      this.updateInteriorWarps(interior);
    }
    return interior;
  }

  public virtual Point getPointForHumanDoor()
  {
    return new Point(this.tileX.Value + this.humanDoor.Value.X, this.tileY.Value + this.humanDoor.Value.Y);
  }

  public virtual Microsoft.Xna.Framework.Rectangle getRectForHumanDoor()
  {
    return new Microsoft.Xna.Framework.Rectangle(this.getPointForHumanDoor().X * 64 /*0x40*/, this.getPointForHumanDoor().Y * 64 /*0x40*/, 64 /*0x40*/, 64 /*0x40*/);
  }

  public Microsoft.Xna.Framework.Rectangle getRectForAnimalDoor()
  {
    return this.getRectForAnimalDoor(this.GetData());
  }

  public virtual Microsoft.Xna.Framework.Rectangle getRectForAnimalDoor(BuildingData data)
  {
    if (data == null)
      return new Microsoft.Xna.Framework.Rectangle((this.animalDoor.X + this.tileX.Value) * 64 /*0x40*/, (this.tileY.Value + this.animalDoor.Y) * 64 /*0x40*/, 64 /*0x40*/, 64 /*0x40*/);
    Microsoft.Xna.Framework.Rectangle animalDoor = data.AnimalDoor;
    return new Microsoft.Xna.Framework.Rectangle((animalDoor.X + this.tileX.Value) * 64 /*0x40*/, (animalDoor.Y + this.tileY.Value) * 64 /*0x40*/, animalDoor.Width * 64 /*0x40*/, animalDoor.Height * 64 /*0x40*/);
  }

  public virtual void load()
  {
    if (!Game1.IsMasterGame)
      return;
    BuildingData data = this.GetData();
    if (!this.hasLoaded)
    {
      this.hasLoaded = true;
      if (data != null)
      {
        if (data.NonInstancedIndoorLocation == null && this.nonInstancedIndoorsName.Value != null)
        {
          GameLocation indoors = this.GetIndoors();
          if (indoors != null)
            indoors.parentLocationName.Value = (string) null;
          this.nonInstancedIndoorsName.Value = (string) null;
        }
        else if (data.NonInstancedIndoorLocation != null)
        {
          bool nonInstancedLocationAlreadyUsed = false;
          Utility.ForEachBuilding((Func<Building, bool>) (building =>
          {
            if (!building.HasIndoorsName(data.NonInstancedIndoorLocation))
              return true;
            nonInstancedLocationAlreadyUsed = true;
            return false;
          }));
          if (!nonInstancedLocationAlreadyUsed)
            this.nonInstancedIndoorsName.Value = Game1.RequireLocation(data.NonInstancedIndoorLocation).NameOrUniqueName;
        }
      }
      this.LoadFromBuildingData(data);
    }
    if (this.nonInstancedIndoorsName.Value != null)
    {
      this.UpdateIndoorParent();
    }
    else
    {
      string nameOfIndoorsWithoutUnique = data?.IndoorMap ?? this.indoors.Value?.Name;
      GameLocation indoors = this.createIndoors(data, nameOfIndoorsWithoutUnique);
      if (indoors != null && this.indoors.Value != null)
      {
        indoors.characters.Set((ICollection<NPC>) this.indoors.Value.characters);
        indoors.netObjects.MoveFrom(this.indoors.Value.netObjects);
        indoors.terrainFeatures.MoveFrom(this.indoors.Value.terrainFeatures);
        indoors.IsFarm = true;
        indoors.IsOutdoors = false;
        indoors.isStructure.Value = true;
        indoors.miniJukeboxCount.Set(this.indoors.Value.miniJukeboxCount.Value);
        indoors.miniJukeboxTrack.Set(this.indoors.Value.miniJukeboxTrack.Value);
        indoors.uniqueName.Value = this.indoors.Value.uniqueName.Value ?? nameOfIndoorsWithoutUnique + (this.tileX.Value * 2000 + this.tileY.Value).ToString();
        indoors.numberOfSpawnedObjectsOnMap = this.indoors.Value.numberOfSpawnedObjectsOnMap;
        indoors.animals.MoveFrom(this.indoors.Value.animals);
        if (this.indoors.Value is AnimalHouse animalHouse1 && indoors is AnimalHouse animalHouse2)
          animalHouse2.animalsThatLiveHere.Set((IList<long>) animalHouse1.animalsThatLiveHere);
        foreach (KeyValuePair<long, FarmAnimal> pair in indoors.animals.Pairs)
          pair.Value.reload(indoors);
        indoors.furniture.Set((ICollection<Furniture>) this.indoors.Value.furniture);
        foreach (Furniture furniture in indoors.furniture)
          furniture.updateDrawPosition();
        if (this.indoors.Value is Cabin cabin1 && indoors is Cabin cabin2)
        {
          cabin2.fridge.Value = cabin1.fridge.Value;
          cabin2.farmhandReference.Value = cabin1.farmhandReference.Value;
        }
        indoors.TransferDataFromSavedLocation(this.indoors.Value);
        this.indoors.Value = indoors;
      }
      this.updateInteriorWarps();
      if (this.indoors.Value != null)
      {
        for (int index = this.indoors.Value.characters.Count - 1; index >= 0; --index)
          SaveGame.initializeCharacter(this.indoors.Value.characters[index], this.indoors.Value);
        foreach (TerrainFeature terrainFeature in this.indoors.Value.terrainFeatures.Values)
          terrainFeature.loadSprite();
        foreach (KeyValuePair<Vector2, StardewValley.Object> pair in this.indoors.Value.objects.Pairs)
        {
          pair.Value.initializeLightSource(pair.Key);
          pair.Value.reloadSprite();
        }
      }
    }
    if (data == null)
      return;
    this.humanDoor.X = data.HumanDoor.X;
    this.humanDoor.Y = data.HumanDoor.Y;
  }

  /// <summary>Get the extra tiles to treat as part of the building when placing it through a construction menu, if any. For example, the farmhouse uses this to make sure the stairs are clear.</summary>
  public IEnumerable<BuildingPlacementTile> GetAdditionalPlacementTiles()
  {
    return (IEnumerable<BuildingPlacementTile>) this.GetData()?.AdditionalPlacementTiles ?? (IEnumerable<BuildingPlacementTile>) LegacyShims.EmptyArray<BuildingPlacementTile>();
  }

  public bool isUnderConstruction(bool ignoreUpgrades = true)
  {
    return !ignoreUpgrades && this.daysUntilUpgrade.Value > 0 || this.daysOfConstructionLeft.Value > 0;
  }

  /// <summary>Get whether the building's bounds covers a given tile coordinate.</summary>
  /// <param name="tile">The tile position to check.</param>
  /// <param name="applyTilePropertyRadius">Whether to check the extra tiles around the building itself for which it may add tile properties.</param>
  public bool occupiesTile(Vector2 tile, bool applyTilePropertyRadius = false)
  {
    return this.occupiesTile((int) tile.X, (int) tile.Y, applyTilePropertyRadius);
  }

  /// <summary>Get whether the building's bounds covers a given tile coordinate.</summary>
  /// <param name="x">The X tile position to check.</param>
  /// <param name="y">The Y tile position to check</param>
  /// <param name="applyTilePropertyRadius">Whether to check the extra tiles around the building itself for which it may add tile properties.</param>
  public virtual bool occupiesTile(int x, int y, bool applyTilePropertyRadius = false)
  {
    int tilePropertyRadius = applyTilePropertyRadius ? this.GetAdditionalTilePropertyRadius() : 0;
    int num1 = this.tileX.Value;
    int num2 = this.tileY.Value;
    int num3 = this.tilesWide.Value;
    int num4 = this.tilesHigh.Value;
    return x >= num1 - tilePropertyRadius && x < num1 + num3 + tilePropertyRadius && y >= num2 - tilePropertyRadius && y < num2 + num4 + tilePropertyRadius;
  }

  public virtual bool isTilePassable(Vector2 tile)
  {
    bool flag = this.occupiesTile(tile);
    if (flag && this.isUnderConstruction())
      return false;
    BuildingData data = this.GetData();
    return data != null && this.occupiesTile(tile, true) ? data.IsTilePassable((int) tile.X - this.tileX.Value, (int) tile.Y - this.tileY.Value) : !flag;
  }

  public virtual bool isTileOccupiedForPlacement(Vector2 tile, StardewValley.Object to_place)
  {
    return !this.isTilePassable(tile);
  }

  /// <summary>If this building is fishable, get the color of the water at the given tile position.</summary>
  /// <param name="tile">The tile position.</param>
  /// <returns>Returns the water color to use, or <c>null</c> to use the location's default water color.</returns>
  public virtual Color? GetWaterColor(Vector2 tile) => new Color?();

  public virtual bool isTileFishable(Vector2 tile) => false;

  /// <summary>Whether watering cans can be refilled from any tile covered by this building.</summary>
  /// <remarks>If this is false, watering cans may still be refillable based on tile data (e.g. the <c>WaterSource</c> back tile property).</remarks>
  public virtual bool CanRefillWateringCan() => false;

  /// <summary>Create a pixel rectangle for the building's ground footprint within its location.</summary>
  public Microsoft.Xna.Framework.Rectangle GetBoundingBox()
  {
    return new Microsoft.Xna.Framework.Rectangle(this.tileX.Value * 64 /*0x40*/, this.tileY.Value * 64 /*0x40*/, this.tilesWide.Value * 64 /*0x40*/, this.tilesHigh.Value * 64 /*0x40*/);
  }

  public virtual bool intersects(Microsoft.Xna.Framework.Rectangle boundingBox)
  {
    Microsoft.Xna.Framework.Rectangle boundingBox1 = this.GetBoundingBox();
    int tilePropertyRadius = this.GetAdditionalTilePropertyRadius();
    if (tilePropertyRadius > 0)
      boundingBox1.Inflate(tilePropertyRadius * 64 /*0x40*/, tilePropertyRadius * 64 /*0x40*/);
    if (boundingBox1.Intersects(boundingBox))
    {
      int y = boundingBox.Top / 64 /*0x40*/;
      for (int index1 = boundingBox.Bottom / 64 /*0x40*/; y <= index1; ++y)
      {
        int x = boundingBox.Left / 64 /*0x40*/;
        for (int index2 = boundingBox.Right / 64 /*0x40*/; x <= index2; ++x)
        {
          if (!this.isTilePassable(new Vector2((float) x, (float) y)))
            return true;
        }
      }
    }
    return false;
  }

  public virtual void drawInMenu(SpriteBatch b, int x, int y)
  {
    BuildingData data = this.GetData();
    if (data != null)
    {
      x += (int) ((double) data.DrawOffset.X * 4.0);
      y += (int) ((double) data.DrawOffset.Y * 4.0);
    }
    float num1 = (float) (this.tilesHigh.Value * 64 /*0x40*/);
    float num2 = num1;
    if (data != null)
      num2 -= data.SortTileOffset * 64f;
    float layerDepth1 = num2 / 10000f;
    if (this.ShouldDrawShadow(data))
      this.drawShadow(b, x, y);
    Microsoft.Xna.Framework.Rectangle sourceRect = this.getSourceRect();
    b.Draw(this.texture.Value, new Vector2((float) x, (float) y), new Microsoft.Xna.Framework.Rectangle?(sourceRect), this.color, 0.0f, new Vector2(0.0f, 0.0f), 4f, SpriteEffects.None, layerDepth1);
    if (data?.DrawLayers == null)
      return;
    foreach (BuildingDrawLayer drawLayer in data.DrawLayers)
    {
      if (drawLayer.OnlyDrawIfChestHasContents == null)
      {
        float num3 = num1 - drawLayer.SortTileOffset * 64f + 1f;
        if (drawLayer.DrawInBackground)
          num3 = 0.0f;
        float layerDepth2 = num3 / 10000f;
        Microsoft.Xna.Framework.Rectangle rectangle = this.ApplySourceRectOffsets(drawLayer.GetSourceRect((int) Game1.currentGameTime.TotalGameTime.TotalMilliseconds));
        Texture2D texture = this.texture.Value;
        if (drawLayer.Texture != null)
          texture = Game1.content.Load<Texture2D>(drawLayer.Texture);
        b.Draw(texture, new Vector2((float) x, (float) y) + drawLayer.DrawPosition * 4f, new Microsoft.Xna.Framework.Rectangle?(rectangle), Color.White, 0.0f, new Vector2(0.0f, 0.0f), 4f, SpriteEffects.None, layerDepth2);
      }
    }
  }

  public virtual void drawBackground(SpriteBatch b)
  {
    if (this.isMoving || this.daysOfConstructionLeft.Value > 0 || this.newConstructionTimer.Value > 0)
      return;
    BuildingData data = this.GetData();
    if (data?.DrawLayers == null)
      return;
    Vector2 vector2_1 = new Vector2(0.0f, (float) this.getSourceRect().Height);
    Vector2 vector2_2 = new Vector2((float) (this.tileX.Value * 64 /*0x40*/), (float) (this.tileY.Value * 64 /*0x40*/ + this.tilesHigh.Value * 64 /*0x40*/));
    foreach (BuildingDrawLayer drawLayer in data.DrawLayers)
    {
      if (drawLayer.DrawInBackground)
      {
        if (drawLayer.OnlyDrawIfChestHasContents != null)
        {
          Chest buildingChest = this.GetBuildingChest(drawLayer.OnlyDrawIfChestHasContents);
          if (buildingChest == null || buildingChest.isEmpty())
            continue;
        }
        Microsoft.Xna.Framework.Rectangle rectangle = this.ApplySourceRectOffsets(drawLayer.GetSourceRect((int) Game1.currentGameTime.TotalGameTime.TotalMilliseconds));
        Vector2 vector2_3 = Vector2.Zero;
        if (drawLayer.AnimalDoorOffset != Point.Zero)
          vector2_3 = new Vector2((float) drawLayer.AnimalDoorOffset.X * this.animalDoorOpenAmount.Value, (float) drawLayer.AnimalDoorOffset.Y * this.animalDoorOpenAmount.Value);
        Texture2D texture = this.texture.Value;
        if (drawLayer.Texture != null)
          texture = Game1.content.Load<Texture2D>(drawLayer.Texture);
        b.Draw(texture, Game1.GlobalToLocal(Game1.viewport, vector2_2 + (vector2_3 - vector2_1 + drawLayer.DrawPosition) * 4f), new Microsoft.Xna.Framework.Rectangle?(rectangle), this.color * this.alpha, 0.0f, new Vector2(0.0f, 0.0f), 4f, SpriteEffects.None, 0.0f);
      }
    }
  }

  public virtual void draw(SpriteBatch b)
  {
    if (this.isMoving)
      return;
    if (this.daysOfConstructionLeft.Value > 0 || this.newConstructionTimer.Value > 0)
    {
      this.drawInConstruction(b);
    }
    else
    {
      BuildingData data = this.GetData();
      if (this.ShouldDrawShadow(data))
        this.drawShadow(b);
      float num1 = (float) ((this.tileY.Value + this.tilesHigh.Value) * 64 /*0x40*/);
      float num2 = num1;
      if (data != null)
        num2 -= data.SortTileOffset * 64f;
      float layerDepth1 = num2 / 10000f;
      Vector2 vector2_1 = new Vector2((float) (this.tileX.Value * 64 /*0x40*/), (float) (this.tileY.Value * 64 /*0x40*/ + this.tilesHigh.Value * 64 /*0x40*/));
      Vector2 vector2_2 = Vector2.Zero;
      if (data != null)
        vector2_2 = data.DrawOffset * 4f;
      Microsoft.Xna.Framework.Rectangle sourceRect = this.getSourceRect();
      Vector2 origin = new Vector2(0.0f, (float) sourceRect.Height);
      b.Draw(this.texture.Value, Game1.GlobalToLocal(Game1.viewport, vector2_1 + vector2_2), new Microsoft.Xna.Framework.Rectangle?(sourceRect), this.color * this.alpha, 0.0f, origin, 4f, SpriteEffects.None, layerDepth1);
      if (this.magical.Value && this.buildingType.Value.Equals("Gold Clock"))
      {
        if (Game1.netWorldState.Value.goldenClocksTurnedOff.Value)
        {
          b.Draw(Game1.mouseCursors_1_6, Game1.GlobalToLocal(Game1.viewport, new Vector2((float) (this.tileX.Value * 64 /*0x40*/ + 68), (float) (this.tileY.Value * 64 /*0x40*/ - 56))), new Microsoft.Xna.Framework.Rectangle?(new Microsoft.Xna.Framework.Rectangle(498, 368, 13, 9)), Color.White * this.alpha, 0.0f, Vector2.Zero, 4f, SpriteEffects.None, (float) ((double) ((this.tileY.Value + this.tilesHigh.Value) * 64 /*0x40*/) / 10000.0 + 9.9999997473787516E-05));
        }
        else
        {
          b.Draw(Game1.mouseCursors, Game1.GlobalToLocal(Game1.viewport, new Vector2((float) (this.tileX.Value * 64 /*0x40*/ + 92), (float) (this.tileY.Value * 64 /*0x40*/ - 40))), new Microsoft.Xna.Framework.Rectangle?(Town.hourHandSource), Color.White * this.alpha, (float) (2.0 * Math.PI * ((double) (Game1.timeOfDay % 1200) / 1200.0) + (double) Game1.gameTimeInterval / (double) Game1.realMilliSecondsPerGameTenMinutes / 23.0), new Vector2(2.5f, 8f), 3f, SpriteEffects.None, (float) ((double) ((this.tileY.Value + this.tilesHigh.Value) * 64 /*0x40*/) / 10000.0 + 9.9999997473787516E-05));
          b.Draw(Game1.mouseCursors, Game1.GlobalToLocal(Game1.viewport, new Vector2((float) (this.tileX.Value * 64 /*0x40*/ + 92), (float) (this.tileY.Value * 64 /*0x40*/ - 40))), new Microsoft.Xna.Framework.Rectangle?(Town.minuteHandSource), Color.White * this.alpha, (float) (2.0 * Math.PI * ((double) (Game1.timeOfDay % 1000 % 100 % 60) / 60.0) + (double) Game1.gameTimeInterval / (double) Game1.realMilliSecondsPerGameTenMinutes * 1.0199999809265137), new Vector2(2.5f, 12f), 3f, SpriteEffects.None, (float) ((double) ((this.tileY.Value + this.tilesHigh.Value) * 64 /*0x40*/) / 10000.0 + 0.00011000000085914508));
          b.Draw(Game1.mouseCursors, Game1.GlobalToLocal(Game1.viewport, new Vector2((float) (this.tileX.Value * 64 /*0x40*/ + 92), (float) (this.tileY.Value * 64 /*0x40*/ - 40))), new Microsoft.Xna.Framework.Rectangle?(Town.clockNub), Color.White * this.alpha, 0.0f, new Vector2(2f, 2f), 4f, SpriteEffects.None, (float) ((double) ((this.tileY.Value + this.tilesHigh.Value) * 64 /*0x40*/) / 10000.0 + 0.00011999999696854502));
        }
      }
      if (data != null)
      {
        foreach (Chest buildingChest in this.buildingChests)
        {
          BuildingChest buildingChestData = Building.GetBuildingChestData(data, buildingChest.Name);
          if ((double) buildingChestData.DisplayTile.X != -1.0 && (double) buildingChestData.DisplayTile.Y != -1.0 && buildingChest.Items.Count > 0 && buildingChest.Items[0] != null)
          {
            float num3 = (float) (((double) this.tileY.Value + (double) buildingChestData.DisplayTile.Y + 1.0) * 64.0) + 1f;
            float num4 = (float) (4.0 * Math.Round(Math.Sin(Game1.currentGameTime.TotalGameTime.TotalMilliseconds / 250.0), 2) - (double) buildingChestData.DisplayHeight * 64.0);
            float x = (float) (((double) this.tileX.Value + (double) buildingChestData.DisplayTile.X) * 64.0);
            float num5 = (float) (((double) this.tileY.Value + (double) buildingChestData.DisplayTile.Y - 1.0) * 64.0);
            b.Draw(Game1.mouseCursors, Game1.GlobalToLocal(Game1.viewport, new Vector2(x, num5 + num4)), new Microsoft.Xna.Framework.Rectangle?(new Microsoft.Xna.Framework.Rectangle(141, 465, 20, 24)), Color.White * 0.75f, 0.0f, Vector2.Zero, 4f, SpriteEffects.None, num3 / 10000f);
            ParsedItemData dataOrErrorItem = ItemRegistry.GetDataOrErrorItem(buildingChest.Items[0].QualifiedItemId);
            b.Draw(dataOrErrorItem.GetTexture(), Game1.GlobalToLocal(Game1.viewport, new Vector2((float) ((double) x + 32.0 + 4.0), num5 + 32f + num4)), new Microsoft.Xna.Framework.Rectangle?(dataOrErrorItem.GetSourceRect()), Color.White * 0.75f, 0.0f, new Vector2(8f, 8f), 4f, SpriteEffects.None, (float) (((double) num3 + 1.0) / 10000.0));
          }
        }
        if (data.DrawLayers != null)
        {
          foreach (BuildingDrawLayer drawLayer in data.DrawLayers)
          {
            if (!drawLayer.DrawInBackground)
            {
              if (drawLayer.OnlyDrawIfChestHasContents != null)
              {
                Chest buildingChest = this.GetBuildingChest(drawLayer.OnlyDrawIfChestHasContents);
                if (buildingChest == null || buildingChest.isEmpty())
                  continue;
              }
              float layerDepth2 = (num1 - drawLayer.SortTileOffset * 64f + 1f) / 10000f;
              Microsoft.Xna.Framework.Rectangle rectangle = this.ApplySourceRectOffsets(drawLayer.GetSourceRect((int) Game1.currentGameTime.TotalGameTime.TotalMilliseconds));
              Vector2 vector2_3 = Vector2.Zero;
              if (drawLayer.AnimalDoorOffset != Point.Zero)
                vector2_3 = new Vector2((float) drawLayer.AnimalDoorOffset.X * this.animalDoorOpenAmount.Value, (float) drawLayer.AnimalDoorOffset.Y * this.animalDoorOpenAmount.Value);
              Texture2D texture = this.texture.Value;
              if (drawLayer.Texture != null)
                texture = Game1.content.Load<Texture2D>(drawLayer.Texture);
              b.Draw(texture, Game1.GlobalToLocal(Game1.viewport, vector2_1 + (vector2_3 - origin + drawLayer.DrawPosition) * 4f), new Microsoft.Xna.Framework.Rectangle?(rectangle), this.color * this.alpha, 0.0f, new Vector2(0.0f, 0.0f), 4f, SpriteEffects.None, layerDepth2);
            }
          }
        }
      }
      if (this.daysUntilUpgrade.Value <= 0)
        return;
      if (data != null)
      {
        if ((double) data.UpgradeSignTile.X < 0.0)
          return;
        float layerDepth3 = ((float) (((double) this.tileY.Value + (double) data.UpgradeSignTile.Y + 1.0) * 64.0) + 2f) / 10000f;
        b.Draw(Game1.mouseCursors, Game1.GlobalToLocal(Game1.viewport, this.getUpgradeSignLocation()), new Microsoft.Xna.Framework.Rectangle?(new Microsoft.Xna.Framework.Rectangle(367, 309, 16 /*0x10*/, 15)), Color.White * this.alpha, 0.0f, Vector2.Zero, 4f, SpriteEffects.None, layerDepth3);
      }
      else
      {
        if (!(this.GetIndoors() is Shed))
          return;
        b.Draw(Game1.mouseCursors, Game1.GlobalToLocal(Game1.viewport, this.getUpgradeSignLocation()), new Microsoft.Xna.Framework.Rectangle?(new Microsoft.Xna.Framework.Rectangle(367, 309, 16 /*0x10*/, 15)), Color.White * this.alpha, 0.0f, Vector2.Zero, 4f, SpriteEffects.None, (float) ((double) ((this.tileY.Value + this.tilesHigh.Value) * 64 /*0x40*/) / 10000.0 + 9.9999997473787516E-05));
      }
    }
  }

  public bool ShouldDrawShadow(BuildingData data) => data == null || data.DrawShadow;

  public virtual void drawShadow(SpriteBatch b, int localX = -1, int localY = -1)
  {
    Microsoft.Xna.Framework.Rectangle rectangle = this.getSourceRectForMenu() ?? this.getSourceRect();
    Vector2 position = localX == -1 ? Game1.GlobalToLocal(new Vector2((float) (this.tileX.Value * 64 /*0x40*/), (float) ((this.tileY.Value + this.tilesHigh.Value) * 64 /*0x40*/))) : new Vector2((float) localX, (float) (localY + rectangle.Height * 4));
    b.Draw(Game1.mouseCursors, position, new Microsoft.Xna.Framework.Rectangle?(Building.leftShadow), Color.White * (localX == -1 ? this.alpha : 1f), 0.0f, Vector2.Zero, 4f, SpriteEffects.None, 1E-05f);
    for (int index = 1; index < this.tilesWide.Value - 1; ++index)
      b.Draw(Game1.mouseCursors, position + new Vector2((float) (index * 64 /*0x40*/), 0.0f), new Microsoft.Xna.Framework.Rectangle?(Building.middleShadow), Color.White * (localX == -1 ? this.alpha : 1f), 0.0f, Vector2.Zero, 4f, SpriteEffects.None, 1E-05f);
    b.Draw(Game1.mouseCursors, position + new Vector2((float) ((this.tilesWide.Value - 1) * 64 /*0x40*/), 0.0f), new Microsoft.Xna.Framework.Rectangle?(Building.rightShadow), Color.White * (localX == -1 ? this.alpha : 1f), 0.0f, Vector2.Zero, 4f, SpriteEffects.None, 1E-05f);
  }

  public virtual void OnStartMove()
  {
  }

  public virtual void OnEndMove()
  {
    Game1.player.team.SendBuildingMovedEvent(this.GetParentLocation(), this);
  }

  public Point getPorchStandingSpot()
  {
    return this.isCabin ? new Point(this.tileX.Value + 1, this.tileY.Value + this.tilesHigh.Value - 1) : new Point(0, 0);
  }

  public virtual bool doesTileHaveProperty(
    int tile_x,
    int tile_y,
    string property_name,
    string layer_name,
    ref string property_value)
  {
    BuildingData data = this.GetData();
    if (data != null && this.daysOfConstructionLeft.Value <= 0 && data.HasPropertyAtTile(tile_x - this.tileX.Value, tile_y - this.tileY.Value, property_name, layer_name, ref property_value))
      return true;
    if (!(property_name == "NoSpawn") || !(layer_name == "Back") || !this.occupiesTile(tile_x, tile_y))
      return false;
    property_value = "All";
    return true;
  }

  public Point getMailboxPosition()
  {
    return this.isCabin ? new Point(this.tileX.Value + this.tilesWide.Value - 1, this.tileY.Value + this.tilesHigh.Value - 1) : new Point(68, 16 /*0x10*/);
  }

  /// <summary>Get the number of extra tiles around the building for which it may add tile properties, but without hiding tile properties from the underlying ground that aren't overwritten by the building data.</summary>
  public virtual int GetAdditionalTilePropertyRadius()
  {
    BuildingData data = this.GetData();
    return data == null ? 0 : data.AdditionalTilePropertyRadius;
  }

  public void removeOverlappingBushes(GameLocation location)
  {
    for (int index1 = this.tileX.Value; index1 < this.tileX.Value + this.tilesWide.Value; ++index1)
    {
      for (int index2 = this.tileY.Value; index2 < this.tileY.Value + this.tilesHigh.Value; ++index2)
      {
        if (location.isTerrainFeatureAt(index1, index2))
        {
          LargeTerrainFeature terrainFeatureAt = location.getLargeTerrainFeatureAt(index1, index2);
          if (terrainFeatureAt is Bush)
            location.largeTerrainFeatures.Remove(terrainFeatureAt);
        }
      }
    }
  }

  public virtual void drawInConstruction(SpriteBatch b)
  {
    int height = Math.Min(16 /*0x10*/, Math.Max(0, (int) (16.0 - (double) this.newConstructionTimer.Value / 1000.0 * 16.0)));
    float num1 = (float) (2000 - this.newConstructionTimer.Value) / 2000f;
    if (this.magical.Value || this.daysOfConstructionLeft.Value <= 0)
    {
      BuildingData data = this.GetData();
      if (this.ShouldDrawShadow(data))
        this.drawShadow(b);
      Microsoft.Xna.Framework.Rectangle sourceRect = this.getSourceRect();
      Microsoft.Xna.Framework.Rectangle rectangle = this.getSourceRectForMenu() ?? sourceRect;
      int num2 = (int) ((double) (sourceRect.Height * 4) * (1.0 - (double) num1));
      float num3 = (float) ((this.tileY.Value + this.tilesHigh.Value) * 64 /*0x40*/);
      float num4 = num3;
      if (data != null)
        num4 -= data.SortTileOffset * 64f;
      float layerDepth1 = num4 / 10000f;
      Vector2 vector2_1 = new Vector2((float) (this.tileX.Value * 64 /*0x40*/), (float) (this.tileY.Value * 64 /*0x40*/ + this.tilesHigh.Value * 64 /*0x40*/));
      Vector2 vector2_2 = Vector2.Zero;
      if (data != null)
        vector2_2 = data.DrawOffset * 4f;
      Vector2 vector2_3 = new Vector2(0.0f, (float) (num2 + 4 - num2 % 4));
      Vector2 vector2_4 = new Vector2(0.0f, (float) sourceRect.Height);
      b.Draw(this.texture.Value, Game1.GlobalToLocal(Game1.viewport, vector2_1 + vector2_3 + vector2_2), new Microsoft.Xna.Framework.Rectangle?(new Microsoft.Xna.Framework.Rectangle(sourceRect.Left, sourceRect.Bottom - (int) ((double) num1 * (double) sourceRect.Height), rectangle.Width, (int) ((double) sourceRect.Height * (double) num1))), this.color * this.alpha, 0.0f, new Vector2(0.0f, (float) sourceRect.Height), 4f, SpriteEffects.None, layerDepth1);
      if (data?.DrawLayers != null)
      {
        foreach (BuildingDrawLayer drawLayer in data.DrawLayers)
        {
          if (drawLayer.OnlyDrawIfChestHasContents == null)
          {
            float layerDepth2 = (num3 - drawLayer.SortTileOffset * 64f + 1f) / 10000f;
            Microsoft.Xna.Framework.Rectangle source = drawLayer.GetSourceRect((int) Game1.currentGameTime.TotalGameTime.TotalMilliseconds);
            source = this.ApplySourceRectOffsets(source);
            float num5 = (float) (num2 / 4) - drawLayer.DrawPosition.Y;
            Vector2 zero = Vector2.Zero;
            if ((double) num5 <= (double) source.Height)
            {
              if ((double) num5 > 0.0)
              {
                zero.Y += num5;
                source.Y += (int) num5;
                source.Height -= (int) num5;
              }
              Texture2D texture = this.texture.Value;
              if (drawLayer.Texture != null)
                texture = Game1.content.Load<Texture2D>(drawLayer.Texture);
              b.Draw(texture, Game1.GlobalToLocal(Game1.viewport, vector2_1 + (zero - vector2_4 + drawLayer.DrawPosition) * 4f), new Microsoft.Xna.Framework.Rectangle?(source), this.color * this.alpha, 0.0f, new Vector2(0.0f, 0.0f), 4f, SpriteEffects.None, layerDepth2);
            }
          }
        }
      }
      if (this.magical.Value)
      {
        for (int index = 0; index < this.tilesWide.Value * 4; ++index)
        {
          b.Draw(Game1.mouseCursors, Game1.GlobalToLocal(Game1.viewport, new Vector2((float) (this.tileX.Value * 64 /*0x40*/ + index * 16 /*0x10*/), (float) (this.tileY.Value * 64 /*0x40*/ - sourceRect.Height * 4 + this.tilesHigh.Value * 64 /*0x40*/) + (float) (sourceRect.Height * 4) * (1f - num1))) + new Vector2((float) Game1.random.Next(-1, 2), (float) (Game1.random.Next(-1, 2) - (index % 2 == 0 ? 32 /*0x20*/ : 8))), new Microsoft.Xna.Framework.Rectangle?(new Microsoft.Xna.Framework.Rectangle(536 + (this.newConstructionTimer.Value + index * 4) % 56 / 8 * 8, 1945, 8, 8)), index % 2 == 1 ? Color.Pink * this.alpha : Color.LightPink * this.alpha, 0.0f, new Vector2(0.0f, 0.0f), (float) (4.0 + (double) Game1.random.Next(100) / 100.0), SpriteEffects.None, (float) ((double) ((this.tileY.Value + this.tilesHigh.Value) * 64 /*0x40*/) / 10000.0 + 9.9999997473787516E-05));
          if (index % 2 == 0)
            b.Draw(Game1.mouseCursors, Game1.GlobalToLocal(Game1.viewport, new Vector2((float) (this.tileX.Value * 64 /*0x40*/ + index * 16 /*0x10*/), (float) (this.tileY.Value * 64 /*0x40*/ - sourceRect.Height * 4 + this.tilesHigh.Value * 64 /*0x40*/) + (float) (sourceRect.Height * 4) * (1f - num1))) + new Vector2((float) Game1.random.Next(-1, 2), (float) (Game1.random.Next(-1, 2) + (index % 2 == 0 ? 32 /*0x20*/ : 8))), new Microsoft.Xna.Framework.Rectangle?(new Microsoft.Xna.Framework.Rectangle(536 + (this.newConstructionTimer.Value + index * 4) % 56 / 8 * 8, 1945, 8, 8)), Color.White * this.alpha, 0.0f, new Vector2(0.0f, 0.0f), (float) (4.0 + (double) Game1.random.Next(100) / 100.0), SpriteEffects.None, (float) ((double) ((this.tileY.Value + this.tilesHigh.Value) * 64 /*0x40*/) / 10000.0 + 9.9999997473787516E-05));
        }
      }
      else
      {
        for (int index = 0; index < this.tilesWide.Value * 4; ++index)
        {
          b.Draw(Game1.animations, Game1.GlobalToLocal(Game1.viewport, new Vector2((float) (this.tileX.Value * 64 /*0x40*/ - 16 /*0x10*/ + index * 16 /*0x10*/), (float) (this.tileY.Value * 64 /*0x40*/ - sourceRect.Height * 4 + this.tilesHigh.Value * 64 /*0x40*/) + (float) (sourceRect.Height * 4) * (1f - num1))) + new Vector2((float) Game1.random.Next(-1, 2), (float) (Game1.random.Next(-1, 2) - (index % 2 == 0 ? 32 /*0x20*/ : 8))), new Microsoft.Xna.Framework.Rectangle?(new Microsoft.Xna.Framework.Rectangle((this.newConstructionTimer.Value + index * 20) % 304 / 38 * 64 /*0x40*/, 768 /*0x0300*/, 64 /*0x40*/, 64 /*0x40*/)), Color.White * this.alpha * ((float) this.newConstructionTimer.Value / 500f), 0.0f, new Vector2(0.0f, 0.0f), 1f, SpriteEffects.None, (float) ((double) ((this.tileY.Value + this.tilesHigh.Value) * 64 /*0x40*/) / 10000.0 + 9.9999997473787516E-05));
          if (index % 2 == 0)
            b.Draw(Game1.animations, Game1.GlobalToLocal(Game1.viewport, new Vector2((float) (this.tileX.Value * 64 /*0x40*/ - 16 /*0x10*/ + index * 16 /*0x10*/), (float) (this.tileY.Value * 64 /*0x40*/ - sourceRect.Height * 4 + this.tilesHigh.Value * 64 /*0x40*/) + (float) (sourceRect.Height * 4) * (1f - num1))) + new Vector2((float) Game1.random.Next(-1, 2), (float) (Game1.random.Next(-1, 2) - (index % 2 == 0 ? 32 /*0x20*/ : 8))), new Microsoft.Xna.Framework.Rectangle?(new Microsoft.Xna.Framework.Rectangle((this.newConstructionTimer.Value + index * 20) % 400 / 50 * 64 /*0x40*/, 2944, 64 /*0x40*/, 64 /*0x40*/)), Color.White * this.alpha * ((float) this.newConstructionTimer.Value / 500f), 0.0f, new Vector2(0.0f, 0.0f), 1f, SpriteEffects.None, (float) ((double) ((this.tileY.Value + this.tilesHigh.Value) * 64 /*0x40*/) / 10000.0 + 9.9999997473787516E-05));
        }
      }
    }
    else
    {
      bool flag = this.daysOfConstructionLeft.Value == 1;
      for (int x = this.tileX.Value; x < this.tileX.Value + this.tilesWide.Value; ++x)
      {
        for (int y = this.tileY.Value; y < this.tileY.Value + this.tilesHigh.Value; ++y)
        {
          if (x == this.tileX.Value + this.tilesWide.Value / 2 && y == this.tileY.Value + this.tilesHigh.Value - 1)
          {
            if (flag)
              b.Draw(Game1.mouseCursors, Game1.GlobalToLocal(Game1.viewport, new Vector2((float) x, (float) y) * 64f) + new Vector2(0.0f, (float) (64 /*0x40*/ - height * 4 + 16 /*0x10*/ - 4)), new Microsoft.Xna.Framework.Rectangle?(new Microsoft.Xna.Framework.Rectangle(367, 277, 16 /*0x10*/, 16 /*0x10*/)), Color.White, 0.0f, Vector2.Zero, 4f, SpriteEffects.None, 1E-05f);
            b.Draw(Game1.mouseCursors, Game1.GlobalToLocal(Game1.viewport, new Vector2((float) x, (float) y) * 64f) + new Vector2(0.0f, (float) (64 /*0x40*/ - height * 4)) + (this.newConstructionTimer.Value > 0 ? new Vector2((float) Game1.random.Next(-1, 2), (float) Game1.random.Next(-1, 2)) : Vector2.Zero), new Microsoft.Xna.Framework.Rectangle?(new Microsoft.Xna.Framework.Rectangle(367, 309, 16 /*0x10*/, height)), Color.White, 0.0f, Vector2.Zero, 4f, SpriteEffects.None, (float) (y * 64 /*0x40*/ + 64 /*0x40*/ - 1) / 10000f);
          }
          else if (x == this.tileX.Value && y == this.tileY.Value)
          {
            if (flag)
              b.Draw(Game1.mouseCursors, Game1.GlobalToLocal(Game1.viewport, new Vector2((float) x, (float) y) * 64f) + new Vector2(0.0f, (float) (64 /*0x40*/ - height * 4 + 16 /*0x10*/)), new Microsoft.Xna.Framework.Rectangle?(new Microsoft.Xna.Framework.Rectangle(351, 261, 16 /*0x10*/, 16 /*0x10*/)), Color.White, 0.0f, Vector2.Zero, 4f, SpriteEffects.None, 1E-05f);
            b.Draw(Game1.mouseCursors, Game1.GlobalToLocal(Game1.viewport, new Vector2((float) x, (float) y) * 64f) + new Vector2(0.0f, (float) (64 /*0x40*/ - height * 4)) + (this.newConstructionTimer.Value > 0 ? new Vector2((float) Game1.random.Next(-1, 2), (float) Game1.random.Next(-1, 2)) : Vector2.Zero), new Microsoft.Xna.Framework.Rectangle?(new Microsoft.Xna.Framework.Rectangle(351, 293, 16 /*0x10*/, height)), Color.White, 0.0f, Vector2.Zero, 4f, SpriteEffects.None, (float) (y * 64 /*0x40*/ + 64 /*0x40*/ - 1) / 10000f);
          }
          else if (x == this.tileX.Value + this.tilesWide.Value - 1 && y == this.tileY.Value)
          {
            if (flag)
              b.Draw(Game1.mouseCursors, Game1.GlobalToLocal(Game1.viewport, new Vector2((float) x, (float) y) * 64f) + new Vector2(0.0f, (float) (64 /*0x40*/ - height * 4 + 16 /*0x10*/)), new Microsoft.Xna.Framework.Rectangle?(new Microsoft.Xna.Framework.Rectangle(383, 261, 16 /*0x10*/, 16 /*0x10*/)), Color.White, 0.0f, Vector2.Zero, 4f, SpriteEffects.None, 1E-05f);
            b.Draw(Game1.mouseCursors, Game1.GlobalToLocal(Game1.viewport, new Vector2((float) x, (float) y) * 64f) + new Vector2(0.0f, (float) (64 /*0x40*/ - height * 4)) + (this.newConstructionTimer.Value > 0 ? new Vector2((float) Game1.random.Next(-1, 2), (float) Game1.random.Next(-1, 2)) : Vector2.Zero), new Microsoft.Xna.Framework.Rectangle?(new Microsoft.Xna.Framework.Rectangle(383, 293, 16 /*0x10*/, height)), Color.White, 0.0f, Vector2.Zero, 4f, SpriteEffects.None, (float) (y * 64 /*0x40*/ + 64 /*0x40*/ - 1) / 10000f);
          }
          else if (x == this.tileX.Value + this.tilesWide.Value - 1 && y == this.tileY.Value + this.tilesHigh.Value - 1)
          {
            if (flag)
              b.Draw(Game1.mouseCursors, Game1.GlobalToLocal(Game1.viewport, new Vector2((float) x, (float) y) * 64f) + new Vector2(0.0f, (float) (64 /*0x40*/ - height * 4 + 16 /*0x10*/)), new Microsoft.Xna.Framework.Rectangle?(new Microsoft.Xna.Framework.Rectangle(383, 277, 16 /*0x10*/, 16 /*0x10*/)), Color.White, 0.0f, Vector2.Zero, 4f, SpriteEffects.None, 1E-05f);
            b.Draw(Game1.mouseCursors, Game1.GlobalToLocal(Game1.viewport, new Vector2((float) x, (float) y) * 64f) + new Vector2(0.0f, (float) (64 /*0x40*/ - height * 4)) + (this.newConstructionTimer.Value > 0 ? new Vector2((float) Game1.random.Next(-1, 2), (float) Game1.random.Next(-1, 2)) : Vector2.Zero), new Microsoft.Xna.Framework.Rectangle?(new Microsoft.Xna.Framework.Rectangle(383, 325, 16 /*0x10*/, height)), Color.White, 0.0f, Vector2.Zero, 4f, SpriteEffects.None, (float) (y * 64 /*0x40*/) / 10000f);
          }
          else if (x == this.tileX.Value && y == this.tileY.Value + this.tilesHigh.Value - 1)
          {
            if (flag)
              b.Draw(Game1.mouseCursors, Game1.GlobalToLocal(Game1.viewport, new Vector2((float) x, (float) y) * 64f) + new Vector2(0.0f, (float) (64 /*0x40*/ - height * 4 + 16 /*0x10*/)), new Microsoft.Xna.Framework.Rectangle?(new Microsoft.Xna.Framework.Rectangle(351, 277, 16 /*0x10*/, 16 /*0x10*/)), Color.White, 0.0f, Vector2.Zero, 4f, SpriteEffects.None, 1E-05f);
            b.Draw(Game1.mouseCursors, Game1.GlobalToLocal(Game1.viewport, new Vector2((float) x, (float) y) * 64f) + new Vector2(0.0f, (float) (64 /*0x40*/ - height * 4)) + (this.newConstructionTimer.Value > 0 ? new Vector2((float) Game1.random.Next(-1, 2), (float) Game1.random.Next(-1, 2)) : Vector2.Zero), new Microsoft.Xna.Framework.Rectangle?(new Microsoft.Xna.Framework.Rectangle(351, 325, 16 /*0x10*/, height)), Color.White, 0.0f, Vector2.Zero, 4f, SpriteEffects.None, (float) (y * 64 /*0x40*/) / 10000f);
          }
          else if (x == this.tileX.Value + this.tilesWide.Value - 1)
          {
            if (flag)
              b.Draw(Game1.mouseCursors, Game1.GlobalToLocal(Game1.viewport, new Vector2((float) x, (float) y) * 64f) + new Vector2(0.0f, (float) (64 /*0x40*/ - height * 4 + 16 /*0x10*/)), new Microsoft.Xna.Framework.Rectangle?(new Microsoft.Xna.Framework.Rectangle(383, 261, 16 /*0x10*/, 16 /*0x10*/)), Color.White, 0.0f, Vector2.Zero, 4f, SpriteEffects.None, 1E-05f);
            b.Draw(Game1.mouseCursors, Game1.GlobalToLocal(Game1.viewport, new Vector2((float) x, (float) y) * 64f) + new Vector2(0.0f, (float) (64 /*0x40*/ - height * 4)) + (this.newConstructionTimer.Value > 0 ? new Vector2((float) Game1.random.Next(-1, 2), (float) Game1.random.Next(-1, 2)) : Vector2.Zero), new Microsoft.Xna.Framework.Rectangle?(new Microsoft.Xna.Framework.Rectangle(383, 309, 16 /*0x10*/, height)), Color.White, 0.0f, Vector2.Zero, 4f, SpriteEffects.None, (float) (y * 64 /*0x40*/) / 10000f);
          }
          else if (y == this.tileY.Value + this.tilesHigh.Value - 1)
          {
            if (flag)
              b.Draw(Game1.mouseCursors, Game1.GlobalToLocal(Game1.viewport, new Vector2((float) x, (float) y) * 64f) + new Vector2(0.0f, (float) (64 /*0x40*/ - height * 4 + 16 /*0x10*/)), new Microsoft.Xna.Framework.Rectangle?(new Microsoft.Xna.Framework.Rectangle(367, 277, 16 /*0x10*/, 16 /*0x10*/)), Color.White, 0.0f, Vector2.Zero, 4f, SpriteEffects.None, 1E-05f);
            b.Draw(Game1.mouseCursors, Game1.GlobalToLocal(Game1.viewport, new Vector2((float) x, (float) y) * 64f) + new Vector2(0.0f, (float) (64 /*0x40*/ - height * 4)) + (this.newConstructionTimer.Value > 0 ? new Vector2((float) Game1.random.Next(-1, 2), (float) Game1.random.Next(-1, 2)) : Vector2.Zero), new Microsoft.Xna.Framework.Rectangle?(new Microsoft.Xna.Framework.Rectangle(367, 325, 16 /*0x10*/, height)), Color.White, 0.0f, Vector2.Zero, 4f, SpriteEffects.None, (float) (y * 64 /*0x40*/) / 10000f);
          }
          else if (x == this.tileX.Value)
          {
            if (flag)
              b.Draw(Game1.mouseCursors, Game1.GlobalToLocal(Game1.viewport, new Vector2((float) x, (float) y) * 64f) + new Vector2(0.0f, (float) (64 /*0x40*/ - height * 4 + 16 /*0x10*/)), new Microsoft.Xna.Framework.Rectangle?(new Microsoft.Xna.Framework.Rectangle(351, 261, 16 /*0x10*/, 16 /*0x10*/)), Color.White, 0.0f, Vector2.Zero, 4f, SpriteEffects.None, 1E-05f);
            b.Draw(Game1.mouseCursors, Game1.GlobalToLocal(Game1.viewport, new Vector2((float) x, (float) y) * 64f) + new Vector2(0.0f, (float) (64 /*0x40*/ - height * 4)) + (this.newConstructionTimer.Value > 0 ? new Vector2((float) Game1.random.Next(-1, 2), (float) Game1.random.Next(-1, 2)) : Vector2.Zero), new Microsoft.Xna.Framework.Rectangle?(new Microsoft.Xna.Framework.Rectangle(351, 309, 16 /*0x10*/, height)), Color.White, 0.0f, Vector2.Zero, 4f, SpriteEffects.None, (float) (y * 64 /*0x40*/) / 10000f);
          }
          else if (y == this.tileY.Value)
          {
            if (flag)
              b.Draw(Game1.mouseCursors, Game1.GlobalToLocal(Game1.viewport, new Vector2((float) x, (float) y) * 64f) + new Vector2(0.0f, (float) (64 /*0x40*/ - height * 4 + 16 /*0x10*/)), new Microsoft.Xna.Framework.Rectangle?(new Microsoft.Xna.Framework.Rectangle(367, 261, 16 /*0x10*/, 16 /*0x10*/)), Color.White, 0.0f, Vector2.Zero, 4f, SpriteEffects.None, 1E-05f);
            b.Draw(Game1.mouseCursors, Game1.GlobalToLocal(Game1.viewport, new Vector2((float) x, (float) y) * 64f) + new Vector2(0.0f, (float) (64 /*0x40*/ - height * 4)) + (this.newConstructionTimer.Value > 0 ? new Vector2((float) Game1.random.Next(-1, 2), (float) Game1.random.Next(-1, 2)) : Vector2.Zero), new Microsoft.Xna.Framework.Rectangle?(new Microsoft.Xna.Framework.Rectangle(367, 293, 16 /*0x10*/, height)), Color.White, 0.0f, Vector2.Zero, 4f, SpriteEffects.None, (float) (y * 64 /*0x40*/ + 64 /*0x40*/ - 1) / 10000f);
          }
          else if (flag)
            b.Draw(Game1.mouseCursors, Game1.GlobalToLocal(Game1.viewport, new Vector2((float) x, (float) y) * 64f) + new Vector2(0.0f, (float) (64 /*0x40*/ - height * 4 + 16 /*0x10*/)), new Microsoft.Xna.Framework.Rectangle?(new Microsoft.Xna.Framework.Rectangle(367, 261, 16 /*0x10*/, 16 /*0x10*/)), Color.White, 0.0f, Vector2.Zero, 4f, SpriteEffects.None, 1E-05f);
        }
      }
    }
  }
}
