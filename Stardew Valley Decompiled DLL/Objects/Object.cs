// Decompiled with JetBrains decompiler
// Type: StardewValley.Object
// Assembly: Stardew Valley, Version=1.6.15.24356, Culture=neutral, PublicKeyToken=null
// MVID: AA1F513A-94F0-4EF6-A35F-2D5B4A3721BF
// Assembly location: D:\SteamLibrary\steamapps\common\Stardew Valley\Stardew Valley.dll
// XML documentation location: D:\SteamLibrary\steamapps\common\Stardew Valley\Stardew Valley.xml

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Graphics;
using Netcode;
using StardewValley.Audio;
using StardewValley.BellsAndWhistles;
using StardewValley.Buffs;
using StardewValley.Buildings;
using StardewValley.Characters;
using StardewValley.Constants;
using StardewValley.Delegates;
using StardewValley.Enchantments;
using StardewValley.Extensions;
using StardewValley.GameData;
using StardewValley.GameData.BigCraftables;
using StardewValley.GameData.Buildings;
using StardewValley.GameData.Crops;
using StardewValley.GameData.FarmAnimals;
using StardewValley.GameData.Fences;
using StardewValley.GameData.FloorsAndPaths;
using StardewValley.GameData.LocationContexts;
using StardewValley.GameData.Machines;
using StardewValley.GameData.Objects;
using StardewValley.GameData.WildTrees;
using StardewValley.Internal;
using StardewValley.Inventories;
using StardewValley.ItemTypeDefinitions;
using StardewValley.Locations;
using StardewValley.Menus;
using StardewValley.Monsters;
using StardewValley.Network;
using StardewValley.Network.NetEvents;
using StardewValley.Objects;
using StardewValley.Objects.Trinkets;
using StardewValley.TerrainFeatures;
using StardewValley.TokenizableStrings;
using StardewValley.Tools;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;

#nullable enable
namespace StardewValley;

[XmlInclude(typeof (BreakableContainer))]
[XmlInclude(typeof (Cask))]
[XmlInclude(typeof (Chest))]
[XmlInclude(typeof (ColoredObject))]
[XmlInclude(typeof (CrabPot))]
[XmlInclude(typeof (Fence))]
[XmlInclude(typeof (Furniture))]
[XmlInclude(typeof (IndoorPot))]
[XmlInclude(typeof (ItemPedestal))]
[XmlInclude(typeof (Mannequin))]
[XmlInclude(typeof (MiniJukebox))]
[XmlInclude(typeof (Phone))]
[XmlInclude(typeof (Sign))]
[XmlInclude(typeof (Torch))]
[XmlInclude(typeof (Trinket))]
[XmlInclude(typeof (Wallpaper))]
[XmlInclude(typeof (WoodChipper))]
[XmlInclude(typeof (Workbench))]
public class Object : Item
{
  public const int wood = 388;
  public const int stone = 390;
  public const int copper = 378;
  public const int iron = 380;
  public const int coal = 382;
  public const int gold = 384;
  public const int iridium = 386;
  public const 
  #nullable disable
  string artifactSpotID = "590";
  public const string hayID = "178";
  public const string iridiumBarID = "337";
  public const string woodID = "388";
  public const string stoneID = "390";
  public const string copperID = "378";
  public const string ironID = "380";
  public const string coalID = "382";
  public const string goldID = "384";
  public const string iridiumID = "386";
  public const string amethystClusterID = "66";
  public const string aquamarineID = "62";
  public const string bobberID = "133";
  public const string caveCarrotID = "78";
  public const string diamondID = "72";
  public const string emeraldID = "60";
  public const string prismaticShardID = "74";
  public const string quartzID = "80";
  public const string rubyID = "64";
  public const string sapphireID = "70";
  public const string stardropID = "434";
  public const string topazID = "68";
  public const string artifactSpotQID = "(O)590";
  public const string hayQID = "(O)178";
  public const string copperBarQID = "(O)334";
  public const string ironBarQID = "(O)335";
  public const string goldBarQID = "(O)336";
  public const string iridiumBarQID = "(O)337";
  public const string woodQID = "(O)388";
  public const string stoneQID = "(O)390";
  public const string copperQID = "(O)378";
  public const string ironQID = "(O)380";
  public const string coalQID = "(O)382";
  public const string goldQID = "(O)384";
  public const string iridiumQID = "(O)386";
  public const string amethystClusterQID = "(O)66";
  public const string aquamarineQID = "(O)62";
  public const string caveCarrotQID = "(O)78";
  public const string diamondQID = "(O)72";
  public const string emeraldQID = "(O)60";
  public const string prismaticShardQID = "(O)74";
  public const string rubyQID = "(O)64";
  public const string sapphireQID = "(O)70";
  public const string stardropQID = "(O)434";
  public const string topazQID = "(O)68";
  public const int inedible = -300;
  public const int GreensCategory = -81;
  public const int GemCategory = -2;
  public const int VegetableCategory = -75;
  public const int FishCategory = -4;
  public const int EggCategory = -5;
  public const int MilkCategory = -6;
  public const int CookingCategory = -7;
  public const int CraftingCategory = -8;
  public const int BigCraftableCategory = -9;
  public const int FruitsCategory = -79;
  public const int SeedsCategory = -74;
  public const int mineralsCategory = -12;
  public const int flowersCategory = -80;
  public const int meatCategory = -14;
  public const int metalResources = -15;
  public const int buildingResources = -16;
  public const int sellAtPierres = -17;
  public const int sellAtPierresAndMarnies = -18;
  public const int fertilizerCategory = -19;
  public const int junkCategory = -20;
  public const int baitCategory = -21;
  public const int tackleCategory = -22;
  public const int sellAtFishShopCategory = -23;
  public const int furnitureCategory = -24;
  public const int ingredientsCategory = -25;
  public const int artisanGoodsCategory = -26;
  public const int syrupCategory = -27;
  public const int monsterLootCategory = -28;
  public const int equipmentCategory = -29;
  public const int clothingCategorySortValue = -94;
  public const int hatCategory = -95;
  public const int ringCategory = -96;
  public const int weaponCategory = -98;
  public const int bootsCategory = -97;
  public const int toolCategory = -99;
  public const int clothingCategory = -100;
  public const int trinketCategory = -101;
  public const int booksCategory = -102;
  public const int skillBooksCategory = -103;
  /// <summary>The category for spawned twigs, weeds, and stones which spawn randomly in a location.</summary>
  public const int litterCategory = -999;
  public const int WildHorseradishIndex = 16 /*0x10*/;
  public const int LeekIndex = 20;
  public const int DandelionIndex = 22;
  public const int HandCursorIndex = 26;
  public const int WaterAnimationIndex = 28;
  public const int LumberIndex = 30;
  public const int mineStoneGrey1Index = 32 /*0x20*/;
  public const int mineStoneBlue1Index = 34;
  public const int mineStoneBlue2Index = 36;
  public const int mineStoneGrey2Index = 38;
  public const int mineStoneBrown1Index = 40;
  public const int mineStoneBrown2Index = 42;
  public const int mineStonePurpleIndex = 44;
  public const int mineStoneMysticIndex = 46;
  public const int mineStoneSnow1 = 48 /*0x30*/;
  public const int mineStoneSnow3 = 52;
  public const int mineStoneRed1Index = 56;
  public const int mineStoneRed2Index = 58;
  public const int emeraldIndex = 60;
  public const int aquamarineIndex = 62;
  public const int rubyIndex = 64 /*0x40*/;
  public const int amethystClusterIndex = 66;
  public const int topazIndex = 68;
  public const int sapphireIndex = 70;
  public const int diamondIndex = 72;
  public const int prismaticShardIndex = 74;
  public const int stardrop = 434;
  /// <summary>The <see cref="F:StardewValley.Object.preservedParentSheetIndex" /> value for Wild Honey.</summary>
  public const string WildHoneyPreservedId = "-1";
  public const int lowQuality = 0;
  public const int medQuality = 1;
  public const int highQuality = 2;
  public const int bestQuality = 4;
  public const int fragility_Removable = 0;
  public const int fragility_Delicate = 1;
  public const int fragility_Indestructable = 2;
  public const int spriteSheetTileSize = 16 /*0x10*/;
  public const float wobbleAmountWhenWorking = 10f;
  /// <summary>The suffix added to <see cref="P:StardewValley.Object.Name" /> if this is a recipe.</summary>
  public const string RecipeNameSuffix = " Recipe";
  /// <summary>The backing field for <see cref="P:StardewValley.Object.TileLocation" />.</summary>
  /// <remarks>When changing this value, most code should use <see cref="P:StardewValley.Object.TileLocation" /> instead so the bounding box is recalculated.</remarks>
  [XmlElement("tileLocation")]
  public readonly NetVector2 tileLocation = new NetVector2();
  [XmlElement("owner")]
  public readonly NetLong owner = new NetLong();
  [XmlElement("type")]
  public readonly NetString type = new NetString();
  [XmlElement("canBeSetDown")]
  public readonly NetBool canBeSetDown = new NetBool(false);
  [XmlElement("canBeGrabbed")]
  public readonly NetBool canBeGrabbed = new NetBool(true);
  [XmlElement("isSpawnedObject")]
  public readonly NetBool isSpawnedObject = new NetBool(false);
  [XmlElement("questItem")]
  public readonly NetBool questItem = new NetBool(false);
  [XmlElement("questId")]
  public readonly NetString questId = new NetString();
  [XmlElement("isOn")]
  public readonly NetBool isOn = new NetBool(true);
  [XmlElement("fragility")]
  public readonly NetInt fragility = new NetInt(0);
  [XmlElement("price")]
  public readonly NetInt price = new NetInt();
  [XmlElement("edibility")]
  public readonly NetInt edibility = new NetInt(-300);
  [XmlElement("bigCraftable")]
  public readonly NetBool bigCraftable = new NetBool();
  [XmlElement("setOutdoors")]
  public readonly NetBool setOutdoors = new NetBool();
  [XmlElement("setIndoors")]
  public readonly NetBool setIndoors = new NetBool();
  [XmlElement("readyForHarvest")]
  public readonly NetBool readyForHarvest = new NetBool();
  [XmlElement("showNextIndex")]
  public readonly NetBool showNextIndex = new NetBool();
  [XmlElement("flipped")]
  public readonly NetBool flipped = new NetBool();
  [XmlElement("isLamp")]
  public readonly NetBool isLamp = new NetBool();
  [XmlElement("heldObject")]
  public readonly NetRef<Object> heldObject = new NetRef<Object>();
  /// <summary>If this is a machine, the <see cref="F:StardewValley.GameData.Machines.MachineOutputRule.Id" /> value for the last rule which set the <see cref="F:StardewValley.Object.heldObject" /> value.</summary>
  [XmlElement("lastOutputRuleId")]
  public readonly NetString lastOutputRuleId = new NetString();
  /// <summary>If this is a machine, the last input item for which output was produced.</summary>
  [XmlElement("lastInputItem")]
  public readonly NetRef<Item> lastInputItem = new NetRef<Item>();
  [XmlElement("minutesUntilReady")]
  public readonly NetIntDelta minutesUntilReady = new NetIntDelta();
  [XmlElement("boundingBox")]
  public readonly NetRectangle boundingBox = new NetRectangle();
  public Vector2 scale;
  [XmlElement("uses")]
  public readonly NetInt uses = new NetInt();
  [XmlIgnore]
  private readonly NetRef<LightSource> netLightSource = new NetRef<LightSource>();
  /// <summary>The backing field <see cref="P:StardewValley.Object.displayNameFormat" />.</summary>
  [XmlIgnore]
  public readonly NetString netDisplayNameFormat = new NetString();
  [XmlIgnore]
  public bool isTemporarilyInvisible;
  [XmlIgnore]
  protected NetBool _destroyOvernight = new NetBool(false);
  [XmlIgnore]
  public bool shouldShowSign;
  /// <summary>If set, a custom buff to apply when the item is consumed, in addition to any buffs normally applied by the item.</summary>
  [XmlIgnore]
  public Func<Buff> customBuff;
  /// <summary>The raw net-synced text to show on hover if <see cref="M:StardewValley.Object.IsTextSign" /> is true, including any tokens and pre-filtered text.</summary>
  /// <remarks>Most code should use <see cref="P:StardewValley.Object.SignText" /> instead.</remarks>
  [XmlElement("signText")]
  public readonly NetString signText = new NetString();
  protected MachineEffects _machineAnimation;
  protected bool _machineAnimationLoop;
  protected int _machineAnimationIndex;
  protected int _machineAnimationFrame = -1;
  protected int _machineAnimationInterval;
  [XmlElement("orderData")]
  public readonly NetString orderData = new NetString();
  /// <summary>The inventory from which items are being auto-loaded, if any.</summary>
  /// <remarks>This is set during auto-loading, and unset immediately after the auto-load succeeds or fails.</remarks>
  [XmlIgnore]
  public static IInventory autoLoadFrom;
  [XmlIgnore]
  public int shakeTimer;
  [XmlIgnore]
  public int lastNoteBlockSoundTime;
  [XmlIgnore]
  public ICue internalSound;
  [XmlElement("preserve")]
  public readonly NetNullableEnum<Object.PreserveType> preserve = new NetNullableEnum<Object.PreserveType>();
  [XmlElement("preservedParentSheetIndex")]
  public readonly NetString preservedParentSheetIndex = new NetString();
  /// <summary>Obsolete. This is only kept to preserve data from old save files, and isn't synchronized in multiplayer. Use <see cref="F:StardewValley.Object.preservedParentSheetIndex" /> instead.</summary>
  [XmlElement("honeyType")]
  public string obsolete_honeyType;
  /// <summary>The cached value for <see cref="P:StardewValley.Object.DisplayName" />.</summary>
  [XmlIgnore]
  public string displayName;
  protected bool _hasHeldObject;
  protected bool _hasLightSource;
  public static int CurrentParsedItemCount;
  protected int health = 10;
  [XmlIgnore]
  public bool hovering;

  public bool destroyOvernight
  {
    get => this._destroyOvernight.Value;
    set => this._destroyOvernight.Value = value;
  }

  [XmlIgnore]
  public LightSource lightSource
  {
    get => this.netLightSource.Value;
    set => this.netLightSource.Value = value;
  }

  /// <summary>The location containing this object, if it's placed in the world.</summary>
  [XmlIgnore]
  public virtual GameLocation Location { get; set; }

  /// <summary>The item's tile location in the world.</summary>
  [XmlIgnore]
  public virtual Vector2 TileLocation
  {
    get => this.tileLocation.Value;
    set
    {
      if (!(this.tileLocation.Value != value))
        return;
      this.tileLocation.Value = value;
      this.RecalculateBoundingBox();
    }
  }

  [XmlIgnore]
  public string name
  {
    get => this.netName.Value;
    set => this.netName.Value = value;
  }

  /// <summary>If set, a tokenizable string for the display name to use instead of the item data.</summary>
  /// <remarks>This shouldn't contain translation text directly, since it's synced in multiplayer. Instead use tokens like <c>[LOCALIZED_TEXT key]</c>, or <c>%DISPLAY_NAME</c>/<c>%DISPLAY_NAME_LOWERCASE</c> for the display name from data and <c>%PRESERVED_DISPLAY_NAME</c>/<c>%PRESERVED_DISPLAY_NAME_LOWERCASE</c> for the display name of the preserved item (if applicable).</remarks>
  [XmlElement("displayNameFormat")]
  public string displayNameFormat
  {
    get => this.netDisplayNameFormat.Value;
    set => this.netDisplayNameFormat.Value = value;
  }

  /// <inheritdoc />
  public override string TypeDefinitionId => !this.bigCraftable.Value ? "(O)" : "(BC)";

  /// <inheritdoc />
  [XmlIgnore]
  public override string DisplayName
  {
    get
    {
      this.displayName = this.loadDisplayName();
      if (this.orderData.Value == "QI_COOKING")
        this.displayName = Game1.content.LoadString("Strings\\StringsFromCSFiles:Fresh_Prefix", (object) this.displayName);
      if (!this.isRecipe.Value)
        return this.displayName;
      string str1 = this.displayName;
      string str2;
      if (CraftingRecipe.craftingRecipes.TryGetValue(this.displayName, out str2))
      {
        string str3 = ArgUtility.SplitBySpaceAndGet(ArgUtility.Get(str2.Split('/'), 2), 1);
        if (str3 != null)
          str1 = $"{str1} x{str3}";
      }
      return str1 + Game1.content.LoadString("Strings\\StringsFromCSFiles:Object.cs.12657");
    }
  }

  /// <inheritdoc />
  [XmlIgnore]
  public override string Name
  {
    get => !this.isRecipe.Value ? this.name : this.name + " Recipe";
    set => this.name = value;
  }

  /// <inheritdoc />
  public override string BaseName => this.name;

  [XmlIgnore]
  public string Type
  {
    get => this.type.Value;
    set => this.type.Value = value;
  }

  [XmlIgnore]
  public bool CanBeSetDown
  {
    get => this.canBeSetDown.Value;
    set => this.canBeSetDown.Value = value;
  }

  [XmlIgnore]
  public bool CanBeGrabbed
  {
    get => this.canBeGrabbed.Value;
    set => this.canBeGrabbed.Value = value;
  }

  [XmlIgnore]
  public bool IsOn
  {
    get => this.isOn.Value;
    set => this.isOn.Value = value;
  }

  [XmlIgnore]
  public bool IsSpawnedObject
  {
    get => this.isSpawnedObject.Value;
    set => this.isSpawnedObject.Value = value;
  }

  [XmlIgnore]
  public bool Flipped
  {
    get => this.flipped.Value;
    set => this.flipped.Value = value;
  }

  [XmlIgnore]
  public int Price
  {
    get => this.price.Value;
    set => this.price.Value = value;
  }

  [XmlIgnore]
  public int Edibility
  {
    get => this.edibility.Value;
    set => this.edibility.Value = value;
  }

  [XmlIgnore]
  public int Fragility
  {
    get => this.fragility.Value;
    set => this.fragility.Value = value;
  }

  [XmlIgnore]
  public Vector2 Scale
  {
    get => this.scale;
    set => this.scale = value;
  }

  [XmlIgnore]
  public int MinutesUntilReady
  {
    get => this.minutesUntilReady.Value;
    set => this.minutesUntilReady.Value = value;
  }

  /// <summary>The text to show on hover if <see cref="M:StardewValley.Object.IsTextSign" /> is true, formatted for the local player.</summary>
  /// <remarks>To set the text, see the underlying <see cref="F:StardewValley.Object.signText" /> field.</remarks>
  [XmlIgnore]
  public string SignText { get; private set; }

  /// <inheritdoc />
  protected override void initNetFields()
  {
    base.initNetFields();
    this.NetFields.AddField((INetSerializable) this.tileLocation, "tileLocation").AddField((INetSerializable) this.owner, "owner").AddField((INetSerializable) this.type, "type").AddField((INetSerializable) this.canBeSetDown, "canBeSetDown").AddField((INetSerializable) this.canBeGrabbed, "canBeGrabbed").AddField((INetSerializable) this.isSpawnedObject, "isSpawnedObject").AddField((INetSerializable) this.questItem, "questItem").AddField((INetSerializable) this.questId, "questId").AddField((INetSerializable) this.isOn, "isOn").AddField((INetSerializable) this.fragility, "fragility").AddField((INetSerializable) this.price, "price").AddField((INetSerializable) this.edibility, "edibility").AddField((INetSerializable) this.uses, "uses").AddField((INetSerializable) this.bigCraftable, "bigCraftable").AddField((INetSerializable) this.setOutdoors, "setOutdoors").AddField((INetSerializable) this.setIndoors, "setIndoors").AddField((INetSerializable) this.readyForHarvest, "readyForHarvest").AddField((INetSerializable) this.showNextIndex, "showNextIndex").AddField((INetSerializable) this.flipped, "flipped").AddField((INetSerializable) this.isLamp, "isLamp").AddField((INetSerializable) this.heldObject, "heldObject").AddField((INetSerializable) this.lastInputItem, "lastInputItem").AddField((INetSerializable) this.lastOutputRuleId, "lastOutputRuleId").AddField((INetSerializable) this.minutesUntilReady, "minutesUntilReady").AddField((INetSerializable) this.boundingBox, "boundingBox").AddField((INetSerializable) this.preserve, "preserve").AddField((INetSerializable) this.preservedParentSheetIndex, "preservedParentSheetIndex").AddField((INetSerializable) this.netDisplayNameFormat, "netDisplayNameFormat").AddField((INetSerializable) this.netLightSource, "netLightSource").AddField((INetSerializable) this.orderData, "orderData").AddField((INetSerializable) this._destroyOvernight, "_destroyOvernight").AddField((INetSerializable) this.signText, "signText");
    this.heldObject.fieldChangeVisibleEvent += (FieldChange<NetRef<Object>, Object>) ((field, oldValue, newValue) => this._hasHeldObject = this.heldObject.Value != null);
    this.netLightSource.fieldChangeVisibleEvent += (FieldChange<NetRef<LightSource>, LightSource>) ((field, oldValue, newValue) => this._hasLightSource = this.netLightSource.Value != null);
    this.bigCraftable.fieldChangeVisibleEvent += (FieldChange<NetBool, bool>) ((field, oldValue, newValue) =>
    {
      this._qualifiedItemId = (string) null;
      this.MarkContextTagsDirty();
    });
    this.signText.fieldChangeVisibleEvent += (FieldChange<NetString, string>) ((field, oldValue, newValue) =>
    {
      newValue = TokenParser.ParseText(newValue);
      this.SignText = Utility.FilterDirtyWords(newValue);
    });
    this.preserve.fieldChangeVisibleEvent += (FieldChange<NetNullableEnum<Object.PreserveType>, Object.PreserveType?>) ((_param1, _param2, _param3) => this.MarkContextTagsDirty());
    this.preservedParentSheetIndex.fieldChangeVisibleEvent += (FieldChange<NetString, string>) ((_param1, _param2, _param3) => this.MarkContextTagsDirty());
  }

  /// <summary>Construct an item with default data.</summary>
  public Object()
  {
  }

  /// <summary>Construct a <see cref="F:StardewValley.ItemRegistry.type_bigCraftable" />-type item.</summary>
  public Object(Vector2 tileLocation, string itemId, bool isRecipe = false)
    : this()
  {
    itemId = this.ValidateUnqualifiedItemId(itemId);
    this.isRecipe.Value = isRecipe;
    this.ItemId = itemId;
    this.canBeSetDown.Value = true;
    this.bigCraftable.Value = true;
    BigCraftableData bigCraftableData;
    if (Game1.bigCraftableData.TryGetValue(itemId, out bigCraftableData))
    {
      this.name = bigCraftableData.Name ?? ItemRegistry.GetDataOrErrorItem(this.QualifiedItemId).InternalName;
      this.price.Value = bigCraftableData.Price;
      this.type.Value = "Crafting";
      this.Category = -9;
      this.setOutdoors.Value = bigCraftableData.CanBePlacedOutdoors;
      this.setIndoors.Value = bigCraftableData.CanBePlacedIndoors;
      this.fragility.Value = bigCraftableData.Fragility;
      this.isLamp.Value = bigCraftableData.IsLamp;
    }
    this.ResetParentSheetIndex();
    this.TileLocation = tileLocation;
    this.initializeLightSource(this.tileLocation.Value);
  }

  /// <summary>Construct a <see cref="F:StardewValley.ItemRegistry.type_object" />-type item.</summary>
  public Object(string itemId, int initialStack, bool isRecipe = false, int price = -1, int quality = 0)
    : this()
  {
    itemId = this.ValidateUnqualifiedItemId(itemId);
    this.stack.Value = initialStack;
    this.isRecipe.Value = isRecipe;
    this.quality.Value = quality;
    this.ItemId = itemId;
    this.ResetParentSheetIndex();
    ObjectData objectData;
    if (Game1.objectData.TryGetValue(itemId, out objectData))
    {
      this.name = objectData.Name ?? ItemRegistry.GetDataOrErrorItem(this.QualifiedItemId).InternalName;
      this.price.Value = objectData.Price;
      this.edibility.Value = objectData.Edibility;
      this.type.Value = objectData.Type;
      this.Category = objectData.Category;
    }
    if (price != -1)
      this.price.Value = price;
    this.canBeSetDown.Value = true;
    this.canBeGrabbed.Value = true;
    this.isSpawnedObject.Value = false;
    if (Game1.random.NextBool() && Utility.IsLegacyIdAbove(itemId, 52) && !Utility.IsLegacyIdBetween(itemId, 8, 15) && !Utility.IsLegacyIdBetween(itemId, 384, 391))
      this.flipped.Value = true;
    if (this.QualifiedItemId == "(O)463" || this.QualifiedItemId == "(O)464")
      this.scale = new Vector2(1f, 1f);
    if (itemId == "449" || this.IsWeeds() || this.IsTwig())
      this.fragility.Value = 2;
    else if (this.name.Contains("Fence"))
      this.scale = new Vector2(10f, 0.0f);
    else if (this.IsBreakableStone())
    {
      switch (itemId)
      {
        case "8":
          this.minutesUntilReady.Value = 4;
          break;
        case "10":
          this.minutesUntilReady.Value = 8;
          break;
        case "12":
          this.minutesUntilReady.Value = 16 /*0x10*/;
          break;
        case "14":
          this.minutesUntilReady.Value = 12;
          break;
        case "25":
          this.minutesUntilReady.Value = 8;
          break;
        default:
          this.minutesUntilReady.Value = 1;
          break;
      }
    }
    if (this.Category != -22)
      return;
    this.scale.Y = 1f;
  }

  /// <summary>Change the item's ID and parent sheet index without changing any other item data.</summary>
  /// <param name="spriteIndex">The new parent sheet index and item ID to set.</param>
  [Obsolete("This is used for specialized game behavior and only supports vanilla objects. New code should place a new object instance instead.")]
  public virtual void SetIdAndSprite(int spriteIndex)
  {
    this.ParentSheetIndex = spriteIndex;
    this.ItemId = spriteIndex.ToString();
  }

  /// <summary>Recalculate the item's bounding box based on its current position.</summary>
  /// <remarks>This is only needed in cases where the position was moved manually instead of using the <see cref="P:StardewValley.Object.TileLocation" /> property.</remarks>
  public virtual void RecalculateBoundingBox()
  {
    Vector2 tileLocation = this.TileLocation;
    this.boundingBox.Value = new Microsoft.Xna.Framework.Rectangle((int) tileLocation.X * 64 /*0x40*/, (int) tileLocation.Y * 64 /*0x40*/, 64 /*0x40*/, 64 /*0x40*/);
  }

  public virtual bool IsHeldOverHead() => true;

  protected override void _PopulateContextTags(HashSet<string> tags)
  {
    base._PopulateContextTags(tags);
    if (this.orderData.Value == "QI_COOKING")
      tags.Add("quality_qi");
    if ((NetFieldBase<Object.PreserveType?, NetNullableEnum<Object.PreserveType>>) this.preserve != (NetNullableEnum<Object.PreserveType>) null)
    {
      Object.PreserveType? nullable = this.preserve.Value;
      if (nullable.HasValue)
      {
        nullable = this.preserve.Value;
        if (nullable.HasValue)
        {
          switch (nullable.GetValueOrDefault())
          {
            case Object.PreserveType.Wine:
              tags.Add("wine_item");
              break;
            case Object.PreserveType.Jelly:
              tags.Add("jelly_item");
              break;
            case Object.PreserveType.Pickle:
              tags.Add("pickle_item");
              break;
            case Object.PreserveType.Juice:
              tags.Add("juice_item");
              break;
            case Object.PreserveType.Honey:
              tags.Add("honey_item");
              break;
          }
        }
      }
    }
    if (this.preservedParentSheetIndex.Value == null)
      return;
    tags.Add("preserve_sheet_index_" + ItemContextTagManager.SanitizeContextTag(this.preservedParentSheetIndex.Value));
  }

  /// <summary>Get the translated display name for the item, excluding metadata like "(Recipe)".</summary>
  /// <remarks>Most code should use <see cref="P:StardewValley.Object.DisplayName" /> instead, which caches the value.</remarks>
  protected virtual string loadDisplayName()
  {
    return Object.GetObjectDisplayName(this.QualifiedItemId, this.preserve.Value, this.preservedParentSheetIndex.Value, this.displayNameFormat);
  }

  /// <summary>Get the display name for an object including any preserved flavor, like "Blueberry Wine".</summary>
  /// <param name="itemId">The item ID for the flavored item, like <c>(O)348</c> for the wine in "Blueberry Wine".</param>
  /// <param name="preserveType">The preserve type.</param>
  /// <param name="preservedId">The item ID for the flavor item, like <c>(O)258</c> for the blueberry in "Blueberry Wine".</param>
  /// <param name="displayNameFormat">If set, a tokenizable string for the display name to use instead of the item data. See remarks on <see cref="P:StardewValley.Object.displayNameFormat" />.</param>
  /// <param name="defaultBaseName">If set, the base name to use if the <paramref name="itemId" /> doesn't match valid item data.</param>
  /// <returns>Returns the base name (if unflavored), or the flavored item name (if flavored), or a default error item name (if invalid).</returns>
  /// <remarks>This is a low-level method; most code should use <see cref="P:StardewValley.Object.DisplayName" /> instead. This only supports <see cref="F:StardewValley.ItemRegistry.type_object" /> items, not sub-types like <see cref="T:StardewValley.Objects.Furniture" />.</remarks>
  public static string GetObjectDisplayName(
    string itemId,
    Object.PreserveType? preserveType,
    string preservedId,
    string displayNameFormat = null,
    string defaultBaseName = null)
  {
    string newValue = defaultBaseName != null ? ItemRegistry.GetData(itemId)?.DisplayName ?? defaultBaseName : ItemRegistry.GetDataOrErrorItem(itemId).DisplayName;
    string preservedItemId = Object.GetPreservedItemId(preserveType, preservedId);
    ParsedItemData dataOrErrorItem = preservedItemId != null ? ItemRegistry.GetDataOrErrorItem(preservedItemId) : (ParsedItemData) null;
    string displayName = dataOrErrorItem?.DisplayName;
    string lowerInvariant = displayName?.ToLowerInvariant();
    if (displayNameFormat != null)
    {
      string objectDisplayName = TokenParser.ParseText(displayNameFormat);
      if (objectDisplayName.Contains('%'))
        objectDisplayName = objectDisplayName.Replace("%DISPLAY_NAME_LOWERCASE", newValue).Replace("%DISPLAY_NAME", newValue).Replace("%PRESERVED_DISPLAY_NAME_LOWERCASE", lowerInvariant).Replace("%PRESERVED_DISPLAY_NAME", displayName);
      return objectDisplayName;
    }
    if (preserveType.HasValue)
    {
      switch (preserveType.GetValueOrDefault())
      {
        case Object.PreserveType.Wine:
          return Game1.content.LoadStringReturnNullIfNotFound($"Strings\\Objects:Wine_Flavored_{dataOrErrorItem?.QualifiedItemId}_Name", displayName, lowerInvariant, false) ?? Game1.content.LoadString("Strings\\Objects:Wine_Flavored_Name", (object) displayName, (object) lowerInvariant);
        case Object.PreserveType.Jelly:
          return Game1.content.LoadStringReturnNullIfNotFound($"Strings\\Objects:Jelly_Flavored_{dataOrErrorItem?.QualifiedItemId}_Name", displayName, lowerInvariant, false) ?? Game1.content.LoadString("Strings\\Objects:Jelly_Flavored_Name", (object) displayName, (object) lowerInvariant);
        case Object.PreserveType.Pickle:
          return Game1.content.LoadStringReturnNullIfNotFound($"Strings\\Objects:Pickles_Flavored_{dataOrErrorItem?.QualifiedItemId}_Name", displayName, lowerInvariant, false) ?? Game1.content.LoadString("Strings\\Objects:Pickles_Flavored_Name", (object) displayName, (object) lowerInvariant);
        case Object.PreserveType.Juice:
          return Game1.content.LoadStringReturnNullIfNotFound($"Strings\\Objects:Juice_Flavored_{dataOrErrorItem?.QualifiedItemId}_Name", displayName, lowerInvariant, false) ?? Game1.content.LoadString("Strings\\Objects:Juice_Flavored_Name", (object) displayName, (object) lowerInvariant);
        case Object.PreserveType.Roe:
          return Game1.content.LoadStringReturnNullIfNotFound($"Strings\\Objects:Roe_Flavored_{dataOrErrorItem?.QualifiedItemId}_Name", displayName, lowerInvariant, false) ?? Game1.content.LoadString("Strings\\Objects:Roe_Flavored_Name", (object) displayName?.TrimEnd('鱼'), (object) lowerInvariant?.TrimEnd('鱼'));
        case Object.PreserveType.AgedRoe:
          if (preservedItemId != null)
            return Game1.content.LoadStringReturnNullIfNotFound($"Strings\\Objects:AgedRoe_Flavored_{dataOrErrorItem?.QualifiedItemId}_Name", displayName, lowerInvariant, false) ?? Game1.content.LoadString("Strings\\Objects:AgedRoe_Flavored_Name", (object) displayName?.TrimEnd('鱼'), (object) lowerInvariant?.TrimEnd('鱼'));
          break;
        case Object.PreserveType.Honey:
          if (preservedId == "-1")
            return Game1.content.LoadString("Strings\\Objects:Honey_Wild_Name");
          return displayName == null ? newValue : Game1.content.LoadStringReturnNullIfNotFound($"Strings\\Objects:Honey_Flavored_{dataOrErrorItem?.QualifiedItemId}_Name", displayName, lowerInvariant, false) ?? Game1.content.LoadString("Strings\\Objects:Honey_Flavored_Name", (object) displayName, (object) lowerInvariant);
        case Object.PreserveType.Bait:
          return Game1.content.LoadStringReturnNullIfNotFound($"Strings\\Objects:SpecificBait_Flavored_{dataOrErrorItem?.QualifiedItemId}_Name", displayName, lowerInvariant, false) ?? Game1.content.LoadString("Strings\\Objects:SpecificBait_Flavored_Name", (object) displayName, (object) lowerInvariant);
        case Object.PreserveType.DriedFruit:
        case Object.PreserveType.DriedMushroom:
          return Game1.content.LoadStringReturnNullIfNotFound($"Strings\\Objects:DriedFruit_Flavored_{dataOrErrorItem?.QualifiedItemId}_Name", displayName, lowerInvariant, false) ?? Lexicon.makePlural(Game1.content.LoadString("Strings\\Objects:DriedFruit_Flavored_Name", (object) displayName, (object) lowerInvariant));
        case Object.PreserveType.SmokedFish:
          return Game1.content.LoadStringReturnNullIfNotFound($"Strings\\Objects:SmokedFish_Flavored_{dataOrErrorItem?.QualifiedItemId}_Name", displayName, lowerInvariant, false) ?? Game1.content.LoadString("Strings\\Objects:SmokedFish_Flavored_Name", (object) displayName, (object) lowerInvariant);
      }
    }
    return newValue;
  }

  public Vector2 getLocalPosition(xTile.Dimensions.Rectangle viewport)
  {
    return new Vector2(this.tileLocation.X * 64f - (float) viewport.X, this.tileLocation.Y * 64f - (float) viewport.Y);
  }

  public static Microsoft.Xna.Framework.Rectangle getSourceRectForBigCraftable(int index)
  {
    return Object.getSourceRectForBigCraftable(Game1.bigCraftableSpriteSheet, index);
  }

  public static Microsoft.Xna.Framework.Rectangle getSourceRectForBigCraftable(
    Texture2D texture,
    int index)
  {
    return new Microsoft.Xna.Framework.Rectangle(index % (texture.Width / 16 /*0x10*/) * 16 /*0x10*/, index * 16 /*0x10*/ / texture.Width * 16 /*0x10*/ * 2, 16 /*0x10*/, 32 /*0x20*/);
  }

  public virtual bool performToolAction(Tool t)
  {
    GameLocation location = this.Location;
    if (this.isTemporarilyInvisible)
      return false;
    if (this.QualifiedItemId == "(BC)165" && this.heldObject.Value is Chest chest1 && !chest1.isEmpty())
    {
      chest1.clearNulls();
      if (t != null && t.isHeavyHitter() && !(t is MeleeWeapon))
      {
        this.playNearbySoundAll("hammer");
        this.shakeTimer = 100;
      }
      return false;
    }
    if (t == null)
    {
      Object @object;
      if (location.objects.TryGetValue(this.tileLocation.Value, out @object) && @object.Equals((object) this))
      {
        if (location.farmers.Count > 0)
          Game1.createRadialDebris(location, 12, (int) this.tileLocation.X, (int) this.tileLocation.Y, Game1.random.Next(4, 10), false);
        location.objects.Remove(this.tileLocation.Value);
      }
      return false;
    }
    if (this.IsBreakableStone() && t is Pickaxe)
    {
      int num = t.upgradeLevel.Value + 1;
      if (this.QualifiedItemId == "(O)12" && t.upgradeLevel.Value == 1 || (this.QualifiedItemId == "(O)12" || this.QualifiedItemId == "(O)14") && t.upgradeLevel.Value == 0)
      {
        num = 0;
        this.playNearbySoundAll("crafting");
      }
      this.MinutesUntilReady -= num;
      if (this.MinutesUntilReady <= 0)
        return true;
      this.playNearbySoundAll("hammer");
      this.shakeTimer = 100;
      return false;
    }
    if (this.IsBreakableStone() && t is Pickaxe)
      return false;
    if (this.name.Equals("Boulder") && (t.upgradeLevel.Value < 4 || !(t is Pickaxe)))
    {
      if (t.isHeavyHitter())
        this.playNearbySoundAll("hammer");
      return false;
    }
    if (this.IsWeeds() && t.isHeavyHitter())
    {
      int num = 1;
      if (t is MeleeWeapon && t.isScythe() && t.QualifiedItemId != "(W)47")
        num = 2;
      if (this.shakeTimer <= 0)
        this.minutesUntilReady.Value -= num;
      if (this.minutesUntilReady.Value <= 0)
      {
        if (!(this.QualifiedItemId == "(O)319") && !(this.QualifiedItemId == "(O)320") && !(this.QualifiedItemId == "(O)321") && t.getLastFarmerToUse() != null)
        {
          foreach (BaseEnchantment enchantment in t.getLastFarmerToUse().enchantments)
            enchantment.OnCutWeed(this.tileLocation.Value, location, t.getLastFarmerToUse());
        }
        this.cutWeed(t.getLastFarmerToUse());
        return true;
      }
      if (this.shakeTimer <= 0)
      {
        Game1.playSound("weed_cut");
        this.shakeTimer = 200;
        return false;
      }
    }
    else
    {
      if (this.IsTwig() && t is Axe)
      {
        this.fragility.Value = 2;
        this.playNearbySoundAll("axchop");
        location.debris.Add(new Debris(ItemRegistry.Create("(O)388"), this.tileLocation.Value * 64f));
        Game1.createRadialDebris(location, 12, (int) this.tileLocation.X, (int) this.tileLocation.Y, Game1.random.Next(4, 10), false);
        Game1.multiplayer.broadcastSprites(location, new TemporaryAnimatedSprite(12, new Vector2(this.tileLocation.X * 64f, this.tileLocation.Y * 64f), Color.White, flipped: Game1.random.NextBool(), animationInterval: 50f));
        t.getLastFarmerToUse().gainExperience(2, 1);
        return true;
      }
      if (this.name.Contains("SupplyCrate") && t.isHeavyHitter())
      {
        this.MinutesUntilReady -= t.upgradeLevel.Value + 1;
        if (this.MinutesUntilReady <= 0)
        {
          this.fragility.Value = 2;
          this.playNearbySoundAll("barrelBreak");
          Random random = Utility.CreateRandom((double) Game1.uniqueIDForThisGame, (double) this.tileLocation.X * 777.0, (double) this.tileLocation.Y * 7.0);
          int houseUpgradeLevel = t.getLastFarmerToUse().HouseUpgradeLevel;
          int x = (int) this.tileLocation.X;
          int y = (int) this.tileLocation.Y;
          switch (houseUpgradeLevel)
          {
            case 0:
              switch (random.Next(7))
              {
                case 0:
                  Game1.createMultipleObjectDebris("(O)770", x, y, random.Next(3, 6), location);
                  break;
                case 1:
                  Game1.createMultipleObjectDebris("(O)371", x, y, random.Next(5, 8), location);
                  break;
                case 2:
                  Game1.createMultipleObjectDebris("(O)535", x, y, random.Next(2, 5), location);
                  break;
                case 3:
                  Game1.createMultipleObjectDebris("(O)241", x, y, random.Next(1, 3), location);
                  break;
                case 4:
                  Game1.createMultipleObjectDebris("(O)395", x, y, random.Next(1, 3), location);
                  break;
                case 5:
                  Game1.createMultipleObjectDebris("(O)286", x, y, random.Next(3, 6), location);
                  break;
                default:
                  Game1.createMultipleObjectDebris("(O)286", x, y, random.Next(3, 6), location);
                  break;
              }
              break;
            case 1:
              switch (random.Next(10))
              {
                case 0:
                  Game1.createMultipleObjectDebris("(O)770", x, y, random.Next(3, 6), location);
                  break;
                case 1:
                  Game1.createMultipleObjectDebris("(O)371", x, y, random.Next(5, 8), location);
                  break;
                case 2:
                  Game1.createMultipleObjectDebris("(O)749", x, y, random.Next(2, 5), location);
                  break;
                case 3:
                  Game1.createMultipleObjectDebris("(O)253", x, y, random.Next(1, 3), location);
                  break;
                case 4:
                  Game1.createMultipleObjectDebris("(O)237", x, y, random.Next(1, 3), location);
                  break;
                case 5:
                  Game1.createMultipleObjectDebris("(O)246", x, y, random.Next(4, 8), location);
                  break;
                case 6:
                  Game1.createMultipleObjectDebris("(O)247", x, y, random.Next(2, 5), location);
                  break;
                case 7:
                  Game1.createMultipleObjectDebris("(O)245", x, y, random.Next(4, 8), location);
                  break;
                case 8:
                  Game1.createMultipleObjectDebris("(O)287", x, y, random.Next(3, 6), location);
                  break;
                default:
                  Game1.createMultipleObjectDebris("MixedFlowerSeeds", x, y, random.Next(4, 6), location);
                  break;
              }
              break;
            default:
              switch (random.Next(9))
              {
                case 0:
                  Game1.createMultipleObjectDebris("(O)770", x, y, random.Next(3, 6), location);
                  break;
                case 1:
                  Game1.createMultipleObjectDebris("(O)920", x, y, random.Next(5, 8), location);
                  break;
                case 2:
                  Game1.createMultipleObjectDebris("(O)749", x, y, random.Next(2, 5), location);
                  break;
                case 3:
                  Game1.createMultipleObjectDebris("(O)253", x, y, random.Next(2, 4), location);
                  break;
                case 4:
                  Game1.createMultipleObjectDebris(random.Choose<string>("(O)904", "(O)905"), x, y, random.Next(1, 3), location);
                  break;
                case 5:
                  Game1.createMultipleObjectDebris("(O)246", x, y, random.Next(4, 8), location);
                  Game1.createMultipleObjectDebris("(O)247", x, y, random.Next(2, 5), location);
                  Game1.createMultipleObjectDebris("(O)245", x, y, random.Next(4, 8), location);
                  break;
                case 6:
                  Game1.createMultipleObjectDebris("(O)275", x, y, 2, location);
                  break;
                case 7:
                  Game1.createMultipleObjectDebris("(O)288", x, y, random.Next(3, 6), location);
                  break;
                default:
                  Game1.createMultipleObjectDebris("MixedFlowerSeeds", x, y, random.Next(5, 6), location);
                  break;
              }
              break;
          }
          Game1.createRadialDebris(location, 12, (int) this.tileLocation.X, (int) this.tileLocation.Y, Game1.random.Next(4, 10), false);
          Game1.multiplayer.broadcastSprites(location, new TemporaryAnimatedSprite(12, new Vector2(this.tileLocation.X * 64f, this.tileLocation.Y * 64f), Color.White, flipped: Game1.random.NextBool(), animationInterval: 50f));
          return true;
        }
        this.shakeTimer = 200;
        this.playNearbySoundAll("woodWhack");
        return false;
      }
    }
    if (this.QualifiedItemId == "(O)590" || this.QualifiedItemId == "(O)SeedSpot")
    {
      if (t is Hoe)
      {
        Random daySaveRandom = Utility.CreateDaySaveRandom(-(double) this.tileLocation.X * 7.0, (double) this.tileLocation.Y * 777.0, (double) (Game1.netWorldState.Value.TreasureTotemsUsed * 777));
        int num = (int) t.getLastFarmerToUse().stats.Increment("ArtifactSpotsDug", 1);
        if (t.getLastFarmerToUse().stats.Get("ArtifactSpotsDug") > 2U && daySaveRandom.NextDouble() < 0.008 + (!t.getLastFarmerToUse().mailReceived.Contains("DefenseBookDropped") ? (double) t.getLastFarmerToUse().stats.Get("ArtifactSpotsDug") * 0.002 : 0.005))
        {
          t.getLastFarmerToUse().mailReceived.Add("DefenseBookDropped");
          Vector2 pixelOrigin = this.TileLocation * 64f;
          Game1.createMultipleItemDebris(ItemRegistry.Create("(O)Book_Defense"), pixelOrigin, Utility.GetOppositeFacingDirection(t.getLastFarmerToUse().FacingDirection), location);
        }
        if (this.QualifiedItemId == "(O)SeedSpot")
          Game1.createMultipleItemDebris(Utility.getRaccoonSeedForCurrentTimeOfYear(t.getLastFarmerToUse(), daySaveRandom), this.TileLocation * 64f, Utility.GetOppositeFacingDirection(t.getLastFarmerToUse().FacingDirection), location);
        else
          location.digUpArtifactSpot((int) this.tileLocation.X, (int) this.tileLocation.Y, t.getLastFarmerToUse());
        location.makeHoeDirt(this.tileLocation.Value, true);
        this.playNearbySoundAll("hoeHit");
        t.getLastFarmerToUse().gainExperience(2, 15);
        location.objects.Remove(this.tileLocation.Value);
      }
      return false;
    }
    if (this.bigCraftable.Value && !(t is MeleeWeapon) && t.isHeavyHitter() && ItemRegistry.GetDataOrErrorItem(this.QualifiedItemId).IsErrorItem)
    {
      this.playNearbySoundAll("hammer");
      this.performRemoveAction();
      location.objects.Remove(this.tileLocation.Value);
      return false;
    }
    if (this.fragility.Value == 2 || !(this.Type == "Crafting") || t is MeleeWeapon || !t.isHeavyHitter() || t is Hoe && this.IsSprinkler())
      return false;
    this.playNearbySoundAll("hammer");
    if (this.fragility.Value == 1)
    {
      Game1.createRadialDebris(location, 12, (int) this.tileLocation.X, (int) this.tileLocation.Y, Game1.random.Next(3, 6), false);
      Game1.createRadialDebris(location, 14, (int) this.tileLocation.X, (int) this.tileLocation.Y, Game1.random.Next(3, 6), false);
      DelayedAction.functionAfterDelay((Action) (() =>
      {
        Game1.createRadialDebris(location, 12, (int) this.tileLocation.X, (int) this.tileLocation.Y, Game1.random.Next(2, 5), false);
        Game1.createRadialDebris(location, 14, (int) this.tileLocation.X, (int) this.tileLocation.Y, Game1.random.Next(2, 5), false);
      }), 80 /*0x50*/);
      Game1.multiplayer.broadcastSprites(location, new TemporaryAnimatedSprite(12, new Vector2(this.tileLocation.X * 64f, this.tileLocation.Y * 64f), Color.White, flipped: Game1.random.NextBool(), animationInterval: 50f));
      this.performRemoveAction();
      location.objects.Remove(this.tileLocation.Value);
      return false;
    }
    TerrainFeature terrainFeature;
    if (this.IsTapper() && location.terrainFeatures.TryGetValue(this.tileLocation.Value, out terrainFeature) && terrainFeature is Tree tree)
      tree.tapped.Value = false;
    switch (this.QualifiedItemId)
    {
      case "(BC)254":
        if (this.heldObject.Value != null)
        {
          this.ResetParentSheetIndex();
          location.debris.Add(new Debris((Item) this.heldObject.Value, this.tileLocation.Value * 64f + new Vector2(32f, 32f)));
          this.heldObject.Value = (Object) null;
        }
        return true;
      case "(BC)21":
        if (this.heldObject.Value != null)
        {
          location.debris.Add(new Debris((Item) this.heldObject.Value, this.tileLocation.Value * 64f + new Vector2(32f, 32f)));
          this.heldObject.Value = (Object) null;
          break;
        }
        break;
    }
    if (this.IsSprinkler() && this.heldObject.Value != null)
    {
      if (this.heldObject.Value.heldObject.Value != null)
      {
        Chest chest = this.heldObject.Value.heldObject.Value as Chest;
        if (chest != null)
          chest.GetMutex().RequestLock((Action) (() =>
          {
            List<Item> objList = new List<Item>((IEnumerable<Item>) chest.Items);
            chest.Items.Clear();
            foreach (Item obj in objList)
            {
              if (obj != null)
                location.debris.Add(new Debris(obj, this.tileLocation.Value * 64f + new Vector2(32f, 32f)));
            }
            Object @object = this.heldObject.Value;
            this.heldObject.Value = (Object) null;
            location.debris.Add(new Debris((Item) @object, this.tileLocation.Value * 64f + new Vector2(32f, 32f)));
            chest.GetMutex().ReleaseLock();
          }));
        return false;
      }
      location.debris.Add(new Debris((Item) this.heldObject.Value, this.tileLocation.Value * 64f + new Vector2(32f, 32f)));
      this.heldObject.Value = (Object) null;
      return false;
    }
    if (this.IsSprinkler() && this.SpecialVariable == 999999)
      location.debris.Add(new Debris(ItemRegistry.Create("(O)93"), this.tileLocation.Value * 64f + new Vector2(32f, 32f)));
    if (this.heldObject.Value != null && this.readyForHarvest.Value)
      location.debris.Add(new Debris((Item) this.heldObject.Value, this.tileLocation.Value * 64f + new Vector2(32f, 32f)));
    if (this.QualifiedItemId == "(BC)156")
    {
      this.ResetParentSheetIndex();
      this.heldObject.Value = (Object) null;
      this.minutesUntilReady.Value = -1;
    }
    if (this.name.Contains("Seasonal"))
      this.ParentSheetIndex -= this.ParentSheetIndex % 4;
    return true;
  }

  public virtual void cutWeed(Farmer who)
  {
    GameLocation location = this.Location;
    Color color = Color.Green;
    string audioName = "cut";
    int rowInAnimationTexture = 50;
    this.fragility.Value = 2;
    string itemId = (string) null;
    if (Game1.random.NextBool())
      itemId = "771";
    else if (Game1.random.NextDouble() < 0.05 + (who.stats.Get("Book_WildSeeds") > 0U ? 0.04 : 0.0))
      itemId = "770";
    else if (Game1.currentSeason == "summer" && Game1.random.NextDouble() < 0.05 + (who.stats.Get("Book_WildSeeds") > 0U ? 0.04 : 0.0))
      itemId = "MixedFlowerSeeds";
    if (this.name.Contains("GreenRainWeeds") && Game1.random.NextDouble() < 0.1)
      itemId = "Moss";
    string qualifiedItemId = this.QualifiedItemId;
    if (qualifiedItemId != null)
    {
      switch (qualifiedItemId.Length)
      {
        case 6:
          switch (qualifiedItemId[5])
          {
            case '0':
              if (qualifiedItemId == "(O)320")
              {
                color = new Color(175, 143, (int) byte.MaxValue);
                audioName = "breakingGlass";
                rowInAnimationTexture = 47;
                this.playNearbySoundAll("drumkit2");
                itemId = (string) null;
                goto label_42;
              }
              goto label_42;
            case '1':
              if (qualifiedItemId == "(O)321")
              {
                color = new Color(73, (int) byte.MaxValue, 158);
                audioName = "breakingGlass";
                rowInAnimationTexture = 47;
                this.playNearbySoundAll("drumkit2");
                itemId = (string) null;
                goto label_42;
              }
              goto label_42;
            case '2':
              switch (qualifiedItemId)
              {
                case "(O)792":
                  goto label_34;
                case "(O)882":
                  goto label_35;
                default:
                  goto label_42;
              }
            case '3':
              switch (qualifiedItemId)
              {
                case "(O)313":
                  break;
                case "(O)793":
                  goto label_34;
                case "(O)883":
                  goto label_35;
                default:
                  goto label_42;
              }
              break;
            case '4':
              switch (qualifiedItemId)
              {
                case "(O)314":
                  break;
                case "(O)794":
                  goto label_34;
                case "(O)884":
                  goto label_35;
                default:
                  goto label_42;
              }
              break;
            case '5':
              if (qualifiedItemId == "(O)315")
                break;
              goto label_42;
            case '6':
              if (qualifiedItemId == "(O)316")
                goto label_30;
              goto label_42;
            case '7':
              if (qualifiedItemId == "(O)317")
                goto label_30;
              goto label_42;
            case '8':
              switch (qualifiedItemId)
              {
                case "(O)678":
                  color = new Color(228, 109, 159);
                  goto label_42;
                case "(O)318":
                  goto label_30;
                default:
                  goto label_42;
              }
            case '9':
              switch (qualifiedItemId)
              {
                case "(O)679":
                  color = new Color(253, 191, 46);
                  goto label_42;
                case "(O)319":
                  color = new Color(30, 216, (int) byte.MaxValue);
                  audioName = "breakingGlass";
                  rowInAnimationTexture = 47;
                  this.playNearbySoundAll("drumkit2");
                  itemId = (string) null;
                  goto label_42;
                default:
                  goto label_42;
              }
            default:
              goto label_42;
          }
          color = new Color(84, 101, 27);
          break;
label_30:
          color = new Color(109, 49, 196);
          break;
label_34:
          itemId = "770";
          break;
label_35:
          color = new Color(30, 97, 68);
          if (Game1.MasterPlayer.hasOrWillReceiveMail("islandNorthCaveOpened") && Game1.random.NextDouble() < 0.1 && !Game1.MasterPlayer.hasOrWillReceiveMail("gotMummifiedFrog"))
          {
            Game1.addMailForTomorrow("gotMummifiedFrog", true, true);
            itemId = "828";
            break;
          }
          if (Game1.random.NextDouble() < 0.01)
          {
            itemId = "828";
            break;
          }
          if (Game1.random.NextDouble() < 0.08)
          {
            itemId = "831";
            break;
          }
          break;
        case 15:
          switch (qualifiedItemId[14])
          {
            case '0':
              if (qualifiedItemId == "GreenRainWeeds0")
                break;
              goto label_42;
            case '1':
              if (qualifiedItemId == "GreenRainWeeds1")
                break;
              goto label_42;
            case '4':
              if (qualifiedItemId == "GreenRainWeeds4")
                break;
              goto label_42;
            default:
              goto label_42;
          }
          audioName = "weed_cut";
          break;
      }
    }
label_42:
    if (audioName.Equals("breakingGlass") && Game1.random.NextDouble() < 1.0 / 400.0)
      itemId = "338";
    this.playNearbySoundAll(audioName);
    Game1.multiplayer.broadcastSprites(location, new TemporaryAnimatedSprite(rowInAnimationTexture, this.tileLocation.Value * 64f, color));
    Game1.multiplayer.broadcastSprites(location, new TemporaryAnimatedSprite(rowInAnimationTexture, this.tileLocation.Value * 64f + new Vector2((float) Game1.random.Next(-16, 16 /*0x10*/), (float) Game1.random.Next(-48, 48 /*0x30*/)), color * 0.75f)
    {
      scale = 0.75f,
      flipped = true
    });
    Game1.multiplayer.broadcastSprites(location, new TemporaryAnimatedSprite(rowInAnimationTexture, this.tileLocation.Value * 64f + new Vector2((float) Game1.random.Next(-16, 16 /*0x10*/), (float) Game1.random.Next(-48, 48 /*0x30*/)), color * 0.75f)
    {
      scale = 0.75f,
      delayBeforeAnimationStart = 50
    });
    Game1.multiplayer.broadcastSprites(location, new TemporaryAnimatedSprite(rowInAnimationTexture, this.tileLocation.Value * 64f + new Vector2((float) Game1.random.Next(-16, 16 /*0x10*/), (float) Game1.random.Next(-48, 48 /*0x30*/)), color * 0.75f)
    {
      scale = 0.75f,
      flipped = true,
      delayBeforeAnimationStart = 100
    });
    if (!audioName.Equals("breakingGlass"))
    {
      if (Game1.random.NextDouble() < 1E-05)
        location.debris.Add(new Debris(ItemRegistry.Create("(H)40"), this.tileLocation.Value * 64f + new Vector2(32f, 32f)));
      if (Game1.random.NextDouble() <= 0.01 && Game1.player.team.SpecialOrderRuleActive("DROP_QI_BEANS"))
        location.debris.Add(new Debris(ItemRegistry.Create("(O)890"), this.tileLocation.Value * 64f + new Vector2(32f, 32f)));
    }
    if (itemId != null)
      location.debris.Add(new Debris((Item) new Object(itemId, 1), this.tileLocation.Value * 64f + new Vector2(32f, 32f)));
    if (Game1.random.NextDouble() < 0.02)
      location.addJumperFrog(this.tileLocation.Value);
    if (!location.HasUnlockedAreaSecretNotes(who) || Game1.random.NextDouble() >= 0.009)
      return;
    Object unseenSecretNote = location.tryToCreateUnseenSecretNote(who);
    if (unseenSecretNote == null)
      return;
    Game1.createItemDebris((Item) unseenSecretNote, new Vector2(this.tileLocation.X + 0.5f, this.tileLocation.Y + 0.75f) * 64f, Game1.player.FacingDirection, location);
  }

  public virtual bool isAnimalProduct()
  {
    return this.Category == -18 || this.Category == -5 || this.Category == -6 || this.QualifiedItemId == "(O)430";
  }

  public virtual bool onExplosion(Farmer who)
  {
    if (who == null)
      return false;
    GameLocation location = this.Location;
    if (this.IsWeeds())
    {
      this.fragility.Value = 0;
      this.cutWeed(who);
      location.removeObject(this.tileLocation.Value, false);
    }
    if (this.IsTwig())
    {
      this.fragility.Value = 0;
      Game1.createRadialDebris(location, 12, (int) this.tileLocation.X, (int) this.tileLocation.Y, Game1.random.Next(4, 10), false);
      location.debris.Add(new Debris(ItemRegistry.Create("(O)388"), this.tileLocation.Value * 64f));
    }
    if (this.IsBreakableStone())
      this.fragility.Value = 0;
    this.performRemoveAction();
    return true;
  }

  /// <inheritdoc />
  public override bool canBeShipped()
  {
    return !this.bigCraftable.Value && this.type.Value != null && this.Type != "Quest" && this.canBeTrashed() && !(this is Furniture) && !(this is Wallpaper);
  }

  public virtual void ApplySprinkler(Vector2 tile)
  {
    GameLocation location = this.Location;
    TerrainFeature terrainFeature;
    if (location.doesTileHavePropertyNoNull((int) tile.X, (int) tile.Y, "NoSprinklers", "Back") == "T" || !location.terrainFeatures.TryGetValue(tile, out terrainFeature) || !(terrainFeature is HoeDirt hoeDirt) || hoeDirt.state.Value == 2)
      return;
    hoeDirt.state.Value = 1;
  }

  public virtual void ApplySprinklerAnimation()
  {
    GameLocation location = this.Location;
    int radiusForSprinkler = this.GetModifiedRadiusForSprinkler();
    int x = (int) this.tileLocation.X;
    int y = (int) this.tileLocation.Y;
    if (radiusForSprinkler != 0)
    {
      if (radiusForSprinkler == 1)
      {
        location.temporarySprites.Add(new TemporaryAnimatedSprite("TileSheets\\animations", new Microsoft.Xna.Framework.Rectangle(0, 1984, 192 /*0xC0*/, 192 /*0xC0*/), 60f, 3, 100, this.tileLocation.Value * 64f + new Vector2(-64f, -64f), false, false)
        {
          color = Color.White * 0.4f,
          delayBeforeAnimationStart = Game1.random.Next(1000),
          id = x * 4000 + y
        });
      }
      else
      {
        if (radiusForSprinkler <= 0)
          return;
        float num = (float) radiusForSprinkler / 2f;
        location.temporarySprites.Add(new TemporaryAnimatedSprite("TileSheets\\animations", new Microsoft.Xna.Framework.Rectangle(0, 2176, 320, 320), 60f, 4, 100, this.tileLocation.Value * 64f + new Vector2(32f, 32f) + new Vector2(-160f, -160f) * num, false, false)
        {
          color = Color.White * 0.4f,
          delayBeforeAnimationStart = Game1.random.Next(1000),
          id = x * 4000 + y,
          scale = num
        });
      }
    }
    else
    {
      int num = Game1.random.Next(1000);
      location.temporarySprites.Add(new TemporaryAnimatedSprite(29, this.tileLocation.Value * 64f + new Vector2(0.0f, -48f), Color.White * 0.5f, 4, animationInterval: 60f, numberOfLoops: 100)
      {
        delayBeforeAnimationStart = num,
        id = x * 4000 + y
      });
      location.temporarySprites.Add(new TemporaryAnimatedSprite(29, this.tileLocation.Value * 64f + new Vector2(48f, 0.0f), Color.White * 0.5f, 4, animationInterval: 60f, numberOfLoops: 100)
      {
        rotation = 1.57079637f,
        delayBeforeAnimationStart = num,
        id = x * 4000 + y
      });
      location.temporarySprites.Add(new TemporaryAnimatedSprite(29, this.tileLocation.Value * 64f + new Vector2(0.0f, 48f), Color.White * 0.5f, 4, animationInterval: 60f, numberOfLoops: 100)
      {
        rotation = 3.14159274f,
        delayBeforeAnimationStart = num,
        id = x * 4000 + y
      });
      location.temporarySprites.Add(new TemporaryAnimatedSprite(29, this.tileLocation.Value * 64f + new Vector2(-48f, 0.0f), Color.White * 0.5f, 4, animationInterval: 60f, numberOfLoops: 100)
      {
        rotation = 4.712389f,
        delayBeforeAnimationStart = num,
        id = x * 4000 + y
      });
    }
  }

  public virtual List<Vector2> GetSprinklerTiles()
  {
    int radiusForSprinkler = this.GetModifiedRadiusForSprinkler();
    if (radiusForSprinkler == 0)
      return Utility.getAdjacentTileLocations(this.tileLocation.Value);
    if (radiusForSprinkler <= 0)
      return new List<Vector2>();
    List<Vector2> sprinklerTiles = new List<Vector2>();
    for (int x = (int) this.tileLocation.X - radiusForSprinkler; (double) x <= (double) this.tileLocation.X + (double) radiusForSprinkler; ++x)
    {
      for (int y = (int) this.tileLocation.Y - radiusForSprinkler; (double) y <= (double) this.tileLocation.Y + (double) radiusForSprinkler; ++y)
        sprinklerTiles.Add(new Vector2((float) x, (float) y));
    }
    return sprinklerTiles;
  }

  public virtual bool IsInSprinklerRangeBroadphase(Vector2 target)
  {
    int num = this.GetModifiedRadiusForSprinkler();
    if (num == 0)
      num = 1;
    return (double) Math.Abs(target.X - this.TileLocation.X) <= (double) num && (double) Math.Abs(target.Y - this.TileLocation.Y) <= (double) num;
  }

  public virtual void DayUpdate()
  {
    GameLocation location1 = this.Location;
    this.health = 10;
    if (this.IsSprinkler() && (!location1.isOutdoors.Value || !location1.IsRainingHere()) && this.GetModifiedRadiusForSprinkler() >= 0)
      location1.postFarmEventOvernightActions.Add((Action) (() =>
      {
        if (Game1.player.team.SpecialOrderRuleActive("NO_SPRINKLER"))
          return;
        foreach (Vector2 sprinklerTile in this.GetSprinklerTiles())
          this.ApplySprinkler(sprinklerTile);
        this.ApplySprinklerAnimation();
      }));
    MachineData machineData = this.GetMachineData();
    if (machineData != null)
    {
      if (machineData.ClearContentsOvernightCondition != null)
      {
        string overnightCondition = machineData.ClearContentsOvernightCondition;
        GameLocation location2 = location1;
        Item obj = this.lastInputItem.Value;
        Object targetItem = this.heldObject.Value;
        Item inputItem = obj;
        if (GameStateQuery.CheckConditions(overnightCondition, location2, targetItem: (Item) targetItem, inputItem: inputItem))
        {
          this.ResetParentSheetIndex();
          this.heldObject.Value = (Object) null;
          this.readyForHarvest.Value = false;
          this.showNextIndex.Value = false;
          this.minutesUntilReady.Value = -1;
        }
      }
      MachineOutputRule rule;
      if (this.heldObject.Value == null && MachineDataUtility.TryGetMachineOutputRule(this, machineData, MachineOutputTrigger.DayUpdate, (Item) null, (Farmer) null, location1, out rule, out MachineOutputTriggerRule _, out MachineOutputRule _, out MachineOutputTriggerRule _))
        this.OutputMachine(machineData, rule, (Item) null, (Farmer) null, location1, false);
    }
    string qualifiedItemId = this.QualifiedItemId;
    if (qualifiedItemId != null)
    {
      switch (qualifiedItemId.Length)
      {
        case 6:
          switch (qualifiedItemId[5])
          {
            case '4':
              switch (qualifiedItemId)
              {
                case "(O)784":
                  goto label_67;
                case "(O)674":
                  goto label_69;
                default:
                  goto label_73;
              }
            case '5':
              switch (qualifiedItemId)
              {
                case "(O)785":
                  goto label_67;
                case "(O)675":
                  goto label_69;
                default:
                  goto label_73;
              }
            case '6':
              switch (qualifiedItemId)
              {
                case "(O)746":
                  if (location1.IsWinterHere())
                  {
                    this.rot();
                    goto label_73;
                  }
                  goto label_73;
                case "(O)676":
                  goto label_71;
                default:
                  goto label_73;
              }
            case '7':
              switch (qualifiedItemId)
              {
                case "(O)747":
                  break;
                case "(O)677":
                  goto label_71;
                default:
                  goto label_73;
              }
              break;
            case '8':
              if (qualifiedItemId == "(O)748")
                break;
              goto label_73;
            default:
              goto label_73;
          }
          this.destroyOvernight = true;
          break;
label_67:
          if (Game1.dayOfMonth == 1 && !location1.IsSpringHere() && location1.isOutdoors.Value)
          {
            ++this.ParentSheetIndex;
            break;
          }
          break;
label_69:
          if (Game1.dayOfMonth == 1 && location1.IsSummerHere() && location1.isOutdoors.Value)
          {
            this.ParentSheetIndex += 2;
            break;
          }
          break;
label_71:
          if (Game1.dayOfMonth == 1 && location1.IsFallHere() && location1.isOutdoors.Value)
          {
            this.ParentSheetIndex += 2;
            break;
          }
          break;
        case 7:
          switch (qualifiedItemId[6])
          {
            case '2':
              if (qualifiedItemId == "(BC)272" && location1 is AnimalHouse animalHouse1)
              {
                using (NetDictionary<long, FarmAnimal, NetRef<FarmAnimal>, SerializableDictionary<long, FarmAnimal>, NetLongDictionary<FarmAnimal, NetRef<FarmAnimal>>>.PairsCollection.Enumerator enumerator = animalHouse1.animals.Pairs.GetEnumerator())
                {
                  while (enumerator.MoveNext())
                    enumerator.Current.Value.pet(Game1.player, true);
                  break;
                }
              }
              break;
            case '4':
              switch (qualifiedItemId)
              {
                case "(BC)104":
                  this.minutesUntilReady.Value = location1.IsWinterHere() ? 9999 : -1;
                  break;
                case "(BC)164":
                  if (location1 is Town)
                  {
                    if (Game1.random.NextDouble() < 0.9)
                    {
                      GameLocation gameLocation = Game1.RequireLocation("ManorHouse");
                      if (gameLocation.CanItemBePlacedHere(new Vector2(22f, 6f)))
                      {
                        if (!Game1.player.hasOrWillReceiveMail("lewisStatue"))
                          Game1.mailbox.Add("lewisStatue");
                        this.rot();
                        gameLocation.objects.Add(new Vector2(22f, 6f), ItemRegistry.Create<Object>("(BC)164"));
                        break;
                      }
                      break;
                    }
                    GameLocation gameLocation1 = Game1.RequireLocation("AnimalShop");
                    if (gameLocation1.CanItemBePlacedHere(new Vector2(11f, 6f)))
                    {
                      if (!Game1.player.hasOrWillReceiveMail("lewisStatue"))
                        Game1.mailbox.Add("lewisStatue");
                      this.rot();
                      gameLocation1.objects.Add(new Vector2(11f, 6f), ItemRegistry.Create<Object>("(BC)164"));
                      break;
                    }
                    break;
                  }
                  break;
              }
              break;
            case '5':
              if (qualifiedItemId == "(BC)165" && location1 is AnimalHouse animalHouse2 && this.heldObject.Value is Chest chest)
              {
                using (NetDictionary<long, FarmAnimal, NetRef<FarmAnimal>, SerializableDictionary<long, FarmAnimal>, NetLongDictionary<FarmAnimal, NetRef<FarmAnimal>>>.ValuesCollection.Enumerator enumerator = animalHouse2.animals.Values.GetEnumerator())
                {
                  while (enumerator.MoveNext())
                  {
                    FarmAnimal current = enumerator.Current;
                    if (current.GetHarvestType().GetValueOrDefault() == FarmAnimalHarvestType.HarvestWithTool && current.currentProduce.Value != null)
                    {
                      Object @object = ItemRegistry.Create<Object>("(O)" + current.currentProduce.Value);
                      @object.CanBeSetDown = false;
                      @object.Quality = current.produceQuality.Value;
                      if (current.hasEatenAnimalCracker.Value)
                        @object.Stack = 2;
                      if (chest.addItem((Item) @object) == null)
                      {
                        current.HandleStatsOnProduceCollected((Item) @object, (uint) @object.Stack);
                        current.currentProduce.Value = (string) null;
                        current.ReloadTextureIfNeeded();
                        this.showNextIndex.Value = true;
                      }
                    }
                  }
                  break;
                }
              }
              break;
            case '6':
              if (qualifiedItemId == "(BC)156" && this.MinutesUntilReady <= 0 && this.heldObject.Value != null)
              {
                if (location1.canSlimeHatchHere())
                {
                  GreenSlime c = (GreenSlime) null;
                  Vector2 position = new Vector2((float) (int) this.tileLocation.X, (float) ((int) this.tileLocation.Y + 1)) * 64f;
                  switch (this.heldObject.Value.QualifiedItemId)
                  {
                    case "(O)680":
                      c = new GreenSlime(position, 0);
                      break;
                    case "(O)413":
                      c = new GreenSlime(position, 40);
                      break;
                    case "(O)437":
                      c = new GreenSlime(position, 80 /*0x50*/);
                      break;
                    case "(O)439":
                      c = new GreenSlime(position, 121);
                      break;
                    case "(O)857":
                      c = new GreenSlime(position, 121);
                      c.makeTigerSlime();
                      break;
                  }
                  if (c != null)
                  {
                    Game1.showGlobalMessage(c.cute.Value ? Game1.content.LoadString("Strings\\StringsFromCSFiles:Object.cs.12689") : Game1.content.LoadString("Strings\\StringsFromCSFiles:Object.cs.12691"));
                    Vector2 tileForCharacter = Utility.recursiveFindOpenTileForCharacter((Character) c, location1, this.tileLocation.Value + new Vector2(0.0f, 1f), 10, false);
                    c.setTilePosition((int) tileForCharacter.X, (int) tileForCharacter.Y);
                    location1.characters.Add((NPC) c);
                    this.ResetParentSheetIndex();
                    this.heldObject.Value = (Object) null;
                    this.minutesUntilReady.Value = -1;
                    break;
                  }
                  break;
                }
                this.minutesUntilReady.Value = Utility.CalculateMinutesUntilMorning(Game1.timeOfDay);
                this.readyForHarvest.Value = false;
                break;
              }
              break;
            case '8':
              if (qualifiedItemId == "(BC)108")
              {
                this.ResetParentSheetIndex();
                Season season = location1.GetSeason();
                if (this.Location.IsOutdoors && (season == Season.Winter || season == Season.Fall))
                {
                  this.ParentSheetIndex = 109;
                  break;
                }
                break;
              }
              break;
          }
          break;
        case 15:
          if (qualifiedItemId == "(BC)MushroomLog" && Game1.IsRainingHere(location1))
          {
            this.minutesUntilReady.Value -= Utility.CalculateMinutesUntilMorning(Game1.timeOfDay);
            break;
          }
          break;
        case 21:
          if (qualifiedItemId == "(BC)StatueOfBlessings")
          {
            this.showNextIndex.Value = false;
            break;
          }
          break;
      }
    }
label_73:
    if (!this.bigCraftable.Value || !this.name.Contains("Seasonal"))
      return;
    this.ParentSheetIndex = this.ParentSheetIndex - this.ParentSheetIndex % 4 + location1.GetSeasonIndex();
  }

  public virtual void rot()
  {
    this.SetIdAndSprite(Utility.CreateRandom((double) Game1.year * 999.0, (double) Game1.dayOfMonth, (double) Game1.seasonIndex).Choose<int>(747, 748));
    this.price.Value = 0;
    this.quality.Value = 0;
    this.name = "Rotten Plant";
    this.displayName = (string) null;
    this.lightSource = (LightSource) null;
    this.bigCraftable.Value = false;
  }

  public override void actionWhenBeingHeld(Farmer who)
  {
    GameLocation currentLocation = who.currentLocation;
    if (currentLocation != null)
    {
      if (Game1.eventUp && Game1.CurrentEvent != null && Game1.CurrentEvent.isFestival)
      {
        currentLocation.removeLightSource(this.lightSource?.Id);
        base.actionWhenBeingHeld(who);
        return;
      }
      if (this.lightSource != null && (!this.bigCraftable.Value || this.isLamp.Value))
      {
        if (!currentLocation.hasLightSource(this.lightSource.Id))
          currentLocation.sharedLights.AddLight(new LightSource(this.lightSource.Id, this.lightSource.textureIndex.Value, this.lightSource.position.Value, this.lightSource.radius.Value, this.lightSource.color.Value, playerID: who.UniqueMultiplayerID, onlyLocation: currentLocation.NameOrUniqueName));
        currentLocation.repositionLightSource(this.lightSource.Id, who.Position + new Vector2(32f, -64f));
      }
    }
    base.actionWhenBeingHeld(who);
  }

  public override void actionWhenStopBeingHeld(Farmer who)
  {
    who.currentLocation?.removeLightSource(this.lightSource?.Id);
    base.actionWhenStopBeingHeld(who);
  }

  public static void ConsumeInventoryItem(Farmer who, Item drop_in, int amount)
  {
    if (drop_in.ConsumeStack(amount) != null)
      return;
    (Object.autoLoadFrom ?? (IInventory) who.Items).RemoveButKeepEmptySlot(drop_in);
    Object.autoLoadFrom?.RemoveEmptySlots();
  }

  /// <summary>Try to add an item to the object (e.g. input for a machine, placed on a table, etc).</summary>
  /// <param name="dropInItem">The item to add.</param>
  /// <param name="probe">Whether to return whether the item would be accepted without making any changes.</param>
  /// <param name="who">The player adding the item.</param>
  /// <param name="returnFalseIfItemConsumed">Whether to return false if the item was accepted, but it was already deducted from the inventory.</param>
  /// <returns>Usually returns whether the item was accepted by the machine.</returns>
  public virtual bool performObjectDropInAction(
    Item dropInItem,
    bool probe,
    Farmer who,
    bool returnFalseIfItemConsumed = false)
  {
    if (this.isTemporarilyInvisible || !(dropInItem is Object @object))
      return false;
    GameLocation location = this.Location;
    if (this.IsSprinkler())
    {
      if (this.heldObject.Value == null && (@object.QualifiedItemId == "(O)915" || @object.QualifiedItemId == "(O)913"))
      {
        if (probe)
          return true;
        if (location is MineShaft || location is VolcanoDungeon && @object.QualifiedItemId == "(O)913")
        {
          Game1.showRedMessage(Game1.content.LoadString("Strings\\StringsFromCSFiles:Object.cs.13053"));
          return false;
        }
        // ISSUE: explicit non-virtual call
        if ((@object.getOne() is Object one ? __nonvirtual (one.QualifiedItemId) : (string) null) == "(O)913" && one.heldObject.Value == null)
          one.heldObject.Value = (Object) new Chest()
          {
            SpecialChestType = Chest.SpecialChestTypes.Enricher
          };
        location.playSound("axe");
        this.heldObject.Value = one;
        this.minutesUntilReady.Value = -1;
        return true;
      }
      if (@object.QualifiedItemId == "(O)93" && this.SpecialVariable != 999999)
      {
        if (probe)
          return true;
        this.SpecialVariable = 999999;
        Game1.playSound("woodyStep");
        this.lightSource = new LightSource(this.GenerateLightSourceId(this.TileLocation), 4, new Vector2((float) ((double) this.tileLocation.X * 64.0 + 16.0), (float) ((double) this.tileLocation.Y * 64.0 + 16.0)), 1.25f, new Color(1, 1, 1) * 0.9f, onlyLocation: location.NameOrUniqueName);
        return true;
      }
    }
    if (@object.QualifiedItemId == "(O)872" && Object.autoLoadFrom == null && this.TryApplyFairyDust(probe))
      return true;
    MachineData machineData = this.GetMachineData();
    if (machineData != null)
      return (this.heldObject.Value == null || machineData.AllowLoadWhenFull) && (!probe || this.MinutesUntilReady <= 0) && this.PlaceInMachine(machineData, dropInItem, probe, who) && (!returnFalseIfItemConsumed || probe);
    if (this.QualifiedItemId == "(BC)99" && @object.QualifiedItemId == "(O)178")
    {
      GameLocation rootLocation = location.GetRootLocation();
      if (rootLocation.GetHayCapacity() <= 0)
      {
        if (Object.autoLoadFrom == null && !probe)
          Game1.showRedMessage(Game1.content.LoadString("Strings\\Buildings:NeedSilo"));
        return false;
      }
      if (probe)
        return true;
      location.playSound("Ship");
      DelayedAction.playSoundAfterDelay("grassyStep", 100);
      if (@object.Stack == 0)
        @object.Stack = 1;
      int num1 = rootLocation.piecesOfHay.Value;
      int addHay = rootLocation.tryToAddHay(@object.Stack);
      int num2 = rootLocation.piecesOfHay.Value;
      if (num1 <= 0 && num2 > 0)
        this.showNextIndex.Value = true;
      else if (num2 <= 0)
        this.showNextIndex.Value = false;
      @object.Stack = addHay;
      if (addHay <= 0)
        return true;
    }
    return false;
  }

  /// <summary>Update the machine for the effects of fairy dust, if applicable.</summary>
  /// <param name="probe">Whether the game is only checking whether fairy dust would be accepted.</param>
  /// <returns>Returns whether the machine was updated (or if <paramref name="probe" /> is true, whether it would have been updated).</returns>
  public virtual bool TryApplyFairyDust(bool probe = false)
  {
    if (this.MinutesUntilReady > 0)
    {
      bool? allowFairyDust = this.GetMachineData()?.AllowFairyDust;
      if (allowFairyDust.HasValue && allowFairyDust.GetValueOrDefault())
      {
        if (!probe)
        {
          Utility.addSprinklesToLocation(this.Location, (int) this.tileLocation.X, (int) this.tileLocation.Y, 1, 2, 400, 40, Color.White);
          Game1.playSound("yoba");
          this.MinutesUntilReady = 10;
          DelayedAction.functionAfterDelay((Action) (() => this.minutesElapsed(10)), 50);
        }
        return true;
      }
    }
    return false;
  }

  /// <summary>Get the output item to produce for a Solar Panel.</summary>
  /// <inheritdoc cref="T:StardewValley.Delegates.MachineOutputDelegate" />
  public static Item OutputSolarPanel(
    Object machine,
    Item inputItem,
    bool probe,
    MachineItemOutput outputData,
    Farmer player,
    out int? overrideMinutesUntilReady)
  {
    int num = machine.MinutesUntilReady;
    GameLocation location = machine.Location;
    Object @object = machine.heldObject.Value;
    if (@object == null)
    {
      @object = ItemRegistry.Create<Object>("(O)787");
      @object.CanBeSetDown = false;
      num = Utility.CalculateMinutesUntilMorning(Game1.timeOfDay, 7);
    }
    if (num > 0 && location.IsOutdoors && !location.IsRainingHere())
      num = Math.Max(0, num - 2400);
    overrideMinutesUntilReady = num != machine.MinutesUntilReady ? new int?(num) : new int?();
    return (Item) @object;
  }

  /// <summary>Get the output item to produce for a Statue of Endless Fortune.</summary>
  /// <inheritdoc cref="T:StardewValley.Delegates.MachineOutputDelegate" />
  public static Item OutputStatueOfEndlessFortune(
    Object machine,
    Item inputItem,
    bool probe,
    MachineItemOutput outputData,
    Farmer player,
    out int? overrideMinutesUntilReady)
  {
    overrideMinutesUntilReady = new int?();
    Item favoriteItem = Utility.getTodaysBirthdayNPC()?.getFavoriteItem();
    if (favoriteItem != null)
      return favoriteItem;
    string itemId = "80";
    switch (Game1.random.Next(4))
    {
      case 0:
        itemId = "72";
        break;
      case 1:
        itemId = "337";
        break;
      case 2:
        itemId = "749";
        break;
      case 3:
        itemId = "336";
        break;
    }
    return (Item) new Object(itemId, 1);
  }

  /// <summary>Get the output item to produce for a Deconstructor.</summary>
  /// <inheritdoc cref="T:StardewValley.Delegates.MachineOutputDelegate" />
  public static Item OutputDeconstructor(
    Object machine,
    Item inputItem,
    bool probe,
    MachineItemOutput outputData,
    Farmer player,
    out int? overrideMinutesUntilReady)
  {
    overrideMinutesUntilReady = new int?();
    if (!inputItem.HasTypeObject() && !inputItem.HasTypeBigCraftable())
      return (Item) null;
    string str;
    if (!CraftingRecipe.craftingRecipes.TryGetValue(inputItem.Name, out str))
      return (Item) null;
    string[] array1 = str.Split('/');
    if (ArgUtility.SplitBySpace(ArgUtility.Get(array1, 2)).Length > 1)
      return (Item) null;
    if (inputItem.QualifiedItemId == "(O)710")
      return ItemRegistry.Create("(O)334", 2);
    Object object1 = (Object) null;
    string[] array2 = ArgUtility.SplitBySpace(ArgUtility.Get(array1, 0));
    for (int index = 0; index < array2.Length; index += 2)
    {
      Object object2 = new Object(ArgUtility.Get(array2, index), ArgUtility.GetInt(array2, index + 1, 1));
      if (object1 == null || object2.sellToStorePrice(-1L) * object2.Stack > object1.sellToStorePrice(-1L) * object1.Stack)
        object1 = object2;
    }
    return (Item) object1;
  }

  /// <summary>Get the output item to produce for an Anvil.</summary>
  /// <inheritdoc cref="T:StardewValley.Delegates.MachineOutputDelegate" />
  public static Item OutputAnvil(
    Object machine,
    Item inputItem,
    bool probe,
    MachineItemOutput outputData,
    Farmer player,
    out int? overrideMinutesUntilReady)
  {
    overrideMinutesUntilReady = new int?();
    if (!(inputItem is Trinket trinket))
      return (Item) null;
    if (!trinket.GetTrinketData().CanBeReforged)
    {
      if (!probe)
        Game1.showRedMessage(Game1.content.LoadString("Strings\\1_6_Strings:Anvil_wrongtrinket"));
      return (Item) null;
    }
    Trinket one = (Trinket) inputItem.getOne();
    if (!one.RerollStats(Game1.random.Next(9999999)))
    {
      if (!probe && player != null)
        player.doEmote(40);
      return (Item) null;
    }
    if (!probe)
    {
      Game1.currentLocation.playSound("metal_tap");
      DelayedAction.playSoundAfterDelay("metal_tap", 250);
      DelayedAction.playSoundAfterDelay("metal_tap", 500);
    }
    overrideMinutesUntilReady = new int?(10);
    return (Item) one;
  }

  /// <summary>Get the output item to produce for a Geode Crusher.</summary>
  /// <inheritdoc cref="T:StardewValley.Delegates.MachineOutputDelegate" />
  public static Item OutputGeodeCrusher(
    Object machine,
    Item inputItem,
    bool probe,
    MachineItemOutput outputData,
    Farmer player,
    out int? overrideMinutesUntilReady)
  {
    overrideMinutesUntilReady = new int?();
    if (!Utility.IsGeode(inputItem, true))
      return (Item) null;
    Item treasureFromGeode = Utility.getTreasureFromGeode(inputItem);
    if (probe)
      return treasureFromGeode;
    GameLocation location = machine.Location;
    Vector2 vector2 = machine.tileLocation.Value * 64f;
    Utility.addSmokePuff(location, vector2 + new Vector2(4f, -48f), 200);
    Utility.addSmokePuff(location, vector2 + new Vector2(-16f, -56f), 300);
    Utility.addSmokePuff(location, vector2 + new Vector2(16f, -52f), 400);
    Utility.addSmokePuff(location, vector2 + new Vector2(32f, -56f), 200);
    Utility.addSmokePuff(location, vector2 + new Vector2(40f, -44f), 500);
    return treasureFromGeode;
  }

  /// <summary>Get the output item to produce for an Incubator or Ostrich Incubator.</summary>
  /// <inheritdoc cref="T:StardewValley.Delegates.MachineOutputDelegate" />
  public static Item OutputIncubator(
    Object machine,
    Item inputItem,
    bool probe,
    MachineItemOutput outputData,
    Farmer player,
    out int? overrideMinutesUntilReady)
  {
    BuildingData data = machine.Location.ParentBuilding?.GetData();
    if (data == null)
    {
      overrideMinutesUntilReady = new int?();
      return (Item) null;
    }
    FarmAnimalData animalDataFromEgg = FarmAnimal.GetAnimalDataFromEgg(inputItem, machine.Location);
    if (animalDataFromEgg == null || !data.ValidOccupantTypes.Contains(animalDataFromEgg.House))
    {
      overrideMinutesUntilReady = new int?();
      return (Item) null;
    }
    overrideMinutesUntilReady = new int?(animalDataFromEgg.IncubationTime > 0 ? animalDataFromEgg.IncubationTime : 9000);
    return inputItem.getOne();
  }

  /// <summary>Get the output item to produce for a Seed Maker.</summary>
  /// <inheritdoc cref="T:StardewValley.Delegates.MachineOutputDelegate" />
  public static Item OutputSeedMaker(
    Object machine,
    Item inputItem,
    bool probe,
    MachineItemOutput outputData,
    Farmer player,
    out int? overrideMinutesUntilReady)
  {
    overrideMinutesUntilReady = new int?();
    if (!inputItem.HasTypeObject())
      return (Item) null;
    string itemId = (string) null;
    foreach (KeyValuePair<string, CropData> keyValuePair in (IEnumerable<KeyValuePair<string, CropData>>) Game1.cropData)
    {
      if (ItemRegistry.HasItemId(inputItem, keyValuePair.Value.HarvestItemId))
      {
        itemId = keyValuePair.Key;
        break;
      }
    }
    if (itemId == null)
      return (Item) null;
    Vector2 vector2 = machine.tileLocation.Value;
    Random daySaveRandom = Utility.CreateDaySaveRandom((double) vector2.X, (double) vector2.Y * 77.0, (double) Game1.timeOfDay);
    return daySaveRandom.NextDouble() >= 0.005 ? (daySaveRandom.NextDouble() >= 0.02 ? (Item) new Object(itemId, daySaveRandom.Next(1, 4)) : (Item) new Object("770", daySaveRandom.Next(1, 5))) : (Item) new Object("499", 1);
  }

  public static Item OutputMushroomLog(
    Object machine,
    Item inputItem,
    bool probe,
    MachineItemOutput outputData,
    Farmer player,
    out int? overrideMinutesUntilReady)
  {
    overrideMinutesUntilReady = new int?();
    List<Tree> treeList = new List<Tree>();
    for (int x = (int) machine.TileLocation.X - 3; x < (int) machine.TileLocation.X + 4; ++x)
    {
      for (int y = (int) machine.TileLocation.Y - 3; y < (int) machine.TileLocation.Y + 4; ++y)
      {
        Vector2 key = new Vector2((float) x, (float) y);
        if (machine.Location.terrainFeatures.GetValueOrDefault(key) is Tree valueOrDefault)
          treeList.Add(valueOrDefault);
      }
    }
    int count = treeList.Count;
    List<string> options = new List<string>();
    int num1 = 0;
    foreach (Tree tree in treeList)
    {
      if (tree.growthStage.Value >= 5)
      {
        string str = Game1.random.NextBool(0.05) ? "(O)422" : (Game1.random.NextBool(0.15) ? "(O)420" : "(O)404");
        switch (tree.treeType.Value)
        {
          case "2":
            str = Game1.random.NextBool(0.1) ? "(O)422" : "(O)420";
            break;
          case "1":
            str = "(O)257";
            break;
          case "3":
            str = "(O)281";
            break;
          case "13":
            str = "(O)422";
            break;
        }
        options.Add(str);
        if (tree.hasMoss.Value)
          ++num1;
      }
    }
    for (int index = 0; index < Math.Max(1, (int) ((double) treeList.Count * 0.75)); ++index)
      options.Add(Game1.random.NextBool(0.05) ? "(O)422" : (Game1.random.NextBool(0.15) ? "(O)420" : "(O)404"));
    int amount = Math.Max(1, Math.Min(5, Game1.random.Next(1, 3) * (treeList.Count / 2)));
    int quality = 0;
    float num2 = (float) ((double) num1 * 0.02500000037252903 + (double) count * 0.02500000037252903);
    while (Game1.random.NextDouble() < (double) num2)
    {
      ++quality;
      if (quality == 3)
      {
        quality = 4;
        break;
      }
    }
    return ItemRegistry.Create(Game1.random.ChooseFrom<string>((IList<string>) options), amount, quality);
  }

  public bool ParseItemCount(string[] query, out string replacement, Random random, Farmer player)
  {
    if (query[0] == "ItemCount")
    {
      replacement = Object.CurrentParsedItemCount.ToString();
      return true;
    }
    replacement = (string) null;
    return false;
  }

  /// <summary>Place an item in this machine.</summary>
  /// <param name="machineData">The machine data to apply.</param>
  /// <param name="inputItem">The item to place in the machine.</param>
  /// <param name="probe">Whether to return whether the item would be placed successfully without making any changes.</param>
  /// <param name="who">The player placing an item in the machine.</param>
  /// <param name="showMessages">Whether to show UI messages for the player.</param>
  /// <param name="playSounds">Whether to play sounds when the item is placed.</param>
  public bool PlaceInMachine(
    MachineData machineData,
    Item inputItem,
    bool probe,
    Farmer who,
    bool showMessages = true,
    bool playSounds = true)
  {
    if (machineData == null || inputItem == null || this.heldObject.Value != null && (!machineData.AllowLoadWhenFull || inputItem.QualifiedItemId == this.lastInputItem.Value?.QualifiedItemId))
      return false;
    MachineItemAdditionalConsumedItems failedRequirement;
    if (!MachineDataUtility.HasAdditionalRequirements(Object.autoLoadFrom ?? (IInventory) who.Items, (IList<MachineItemAdditionalConsumedItems>) machineData.AdditionalConsumedItems, out failedRequirement))
    {
      if (showMessages && failedRequirement.InvalidCountMessage != null && !probe && Object.autoLoadFrom == null)
      {
        Object.CurrentParsedItemCount = failedRequirement.RequiredCount;
        Game1.showRedMessage(TokenParser.ParseText(failedRequirement.InvalidCountMessage, customParser: new TokenParserDelegate(this.ParseItemCount)));
        who.ignoreItemConsumptionThisFrame = true;
      }
      return false;
    }
    GameLocation location = this.Location;
    MachineOutputRule rule;
    MachineOutputTriggerRule triggerRule;
    MachineOutputRule ruleIgnoringCount;
    MachineOutputTriggerRule triggerIgnoringCount;
    if (!MachineDataUtility.TryGetMachineOutputRule(this, machineData, MachineOutputTrigger.ItemPlacedInMachine, inputItem, who, location, out rule, out triggerRule, out ruleIgnoringCount, out triggerIgnoringCount))
    {
      if (showMessages && !probe && Object.autoLoadFrom == null)
      {
        if (ruleIgnoringCount != null)
        {
          string text = ruleIgnoringCount.InvalidCountMessage ?? machineData.InvalidCountMessage;
          if (!string.IsNullOrWhiteSpace(text))
          {
            Object.CurrentParsedItemCount = triggerIgnoringCount.RequiredCount;
            Game1.showRedMessage(TokenParser.ParseText(text, customParser: new TokenParserDelegate(this.ParseItemCount)));
            who.ignoreItemConsumptionThisFrame = true;
          }
        }
        else if (machineData.InvalidItemMessage != null && GameStateQuery.CheckConditions(machineData.InvalidItemMessageCondition, location, who, inputItem: (Item) who.ActiveObject))
        {
          Game1.showRedMessage(TokenParser.ParseText(machineData.InvalidItemMessage));
          who.ignoreItemConsumptionThisFrame = true;
        }
      }
      return false;
    }
    if (probe)
      return true;
    if (!this.OutputMachine(machineData, rule, inputItem, who, location, probe))
      return false;
    if (machineData.AdditionalConsumedItems != null)
    {
      IInventory inventory = Object.autoLoadFrom ?? (IInventory) who.Items;
      foreach (MachineItemAdditionalConsumedItems additionalConsumedItem in machineData.AdditionalConsumedItems)
        inventory.ReduceId(additionalConsumedItem.ItemId, additionalConsumedItem.RequiredCount);
    }
    if (triggerRule.RequiredCount > 0)
      Object.ConsumeInventoryItem(who, inputItem, triggerRule.RequiredCount);
    if (machineData.LoadEffects != null)
    {
      foreach (MachineEffects loadEffect in machineData.LoadEffects)
      {
        if (this.PlayMachineEffect(loadEffect, playSounds))
        {
          this._machineAnimation = loadEffect;
          this._machineAnimationLoop = false;
          this._machineAnimationIndex = 0;
          this._machineAnimationFrame = -1;
          this._machineAnimationInterval = 0;
          break;
        }
      }
    }
    this.playCustomMachineLoadEffects();
    MachineDataUtility.UpdateStats(machineData.StatsToIncrementWhenLoaded, inputItem, 1);
    return true;
  }

  private void playCustomMachineLoadEffects()
  {
    if (!(this.ItemId == "FishSmoker"))
      return;
    for (int index = 0; index < 12; ++index)
      this.Location.temporarySprites.Add(new TemporaryAnimatedSprite("LooseSprites\\Cursors", new Microsoft.Xna.Framework.Rectangle(372, 1956, 10, 10), 9999f, 1, 1, new Vector2((float) ((int) this.TileLocation.X * 64 /*0x40*/) + 18f, (float) (((double) (int) this.TileLocation.Y - 1.1499999761581421) * 64.0)), false, false)
      {
        color = new Color(60, 60, 60),
        alphaFade = -0.02f,
        alpha = 0.01f,
        alphaFadeFade = -0.0003f,
        motion = new Vector2(0.25f, -0.1f),
        acceleration = new Vector2(0.0f, -0.01f),
        rotationChange = (float) Game1.random.Next(-10, 10) / 500f,
        scale = 1.5f,
        scaleChange = 0.024f,
        layerDepth = Math.Max(0.0f, (float) ((((double) this.tileLocation.Y + 1.0) * 64.0 - 24.0 + (double) index) / 10000.0)) + this.tileLocation.X * 1E-05f,
        delayBeforeAnimationStart = index * 550
      });
  }

  /// <summary>Cause the machine to produce output, if applicable.</summary>
  /// <param name="machine">The machine data to apply.</param>
  /// <param name="outputRule">The output rule to apply, or <c>null</c> to get a matching rule from the machine data.</param>
  /// <param name="inputItem">The input item for which to produce an item, if applicable.</param>
  /// <param name="who">The player for which to start producing output, or <c>null</c> for the main player.</param>
  /// <param name="location">The location containing the machine.</param>
  /// <param name="probe">Whether to return whether the machine would produce output without making any changes.</param>
  /// <param name="heldObjectOnly">Whether to change the held object without changing any other machine fields (e.g. to implement <see cref="F:StardewValley.GameData.Machines.MachineOutputRule.RecalculateOnCollect" />).</param>
  public virtual bool OutputMachine(
    MachineData machine,
    MachineOutputRule outputRule,
    Item inputItem,
    Farmer who,
    GameLocation location,
    bool probe,
    bool heldObjectOnly = false)
  {
    who = who ?? Game1.MasterPlayer;
    if (machine == null || this.heldObject.Value != null && !machine.AllowLoadWhenFull || outputRule == null && !MachineDataUtility.TryGetMachineOutputRule(this, machine, MachineOutputTrigger.ItemPlacedInMachine, inputItem, who, location, out outputRule, out MachineOutputTriggerRule _, out MachineOutputRule _, out MachineOutputTriggerRule _))
      return false;
    MachineItemOutput outputData = MachineDataUtility.GetOutputData(this, machine, outputRule, inputItem, who, location);
    int? overrideMinutesUntilReady;
    Item outputItem = MachineDataUtility.GetOutputItem(this, outputData, inputItem, who, heldObjectOnly | probe, out overrideMinutesUntilReady);
    if (outputItem == null)
      return false;
    if (probe)
      return true;
    outputItem.FixQuality();
    outputItem.FixStackSize();
    this.heldObject.Value = (Object) outputItem;
    if (!heldObjectOnly)
    {
      int num1 = 0;
      int? nullable = overrideMinutesUntilReady;
      int num2 = 0;
      if (nullable.GetValueOrDefault() >= num2 & nullable.HasValue)
        num1 = overrideMinutesUntilReady.Value;
      else if (outputRule.MinutesUntilReady >= 0 || outputRule.DaysUntilReady >= 0)
        num1 = outputRule.DaysUntilReady >= 0 ? Utility.CalculateMinutesUntilMorning(Game1.timeOfDay, outputRule.DaysUntilReady) : outputRule.MinutesUntilReady;
      this.MinutesUntilReady = (int) Utility.ApplyQuantityModifiers((float) num1, (IList<QuantityModifier>) machine.ReadyTimeModifiers, machine.ReadyTimeModifierMode, location, who, (Item) this.heldObject.Value, inputItem);
      if (this.MinutesUntilReady == 0)
        this.readyForHarvest.Value = true;
      this.lastOutputRuleId.Value = outputRule.Id;
      if (inputItem != null)
      {
        this.lastInputItem.Value = inputItem.getOne();
        this.lastInputItem.Value.Stack = inputItem.Stack;
      }
      else
        this.lastInputItem.Value = (Item) null;
      if (machine.IsIncubator && location is AnimalHouse animalHouse)
        animalHouse.hasShownIncubatorBuildingFullMessage = false;
      this.ResetParentSheetIndex();
      this.ParentSheetIndex += outputData.IncrementMachineParentSheetIndex;
      if (machine.LightWhileWorking != null)
        this.initializeLightSource(this.tileLocation.Value);
      if (machine.ShowNextIndexWhileWorking)
        this.showNextIndex.Value = true;
      if (machine.WobbleWhileWorking)
        this.scale.X = 5f;
      this.minutesElapsed(0);
    }
    return true;
  }

  /// <summary>Apply a machine effect, if it's valid and its fields match.</summary>
  /// <param name="effect">The machine effect to apply.</param>
  /// <param name="playSounds">Whether to play sounds when the item is placed.</param>
  public virtual bool PlayMachineEffect(MachineEffects effect, bool playSounds = true)
  {
    return MachineDataUtility.PlayEffects(this, effect, playSounds);
  }

  public virtual void updateWhenCurrentLocation(GameTime time)
  {
    GameLocation location1 = this.Location;
    if (location1 == null)
      return;
    if (this.readyForHarvest.Value && !this._hasHeldObject)
      this.readyForHarvest.Value = false;
    if (this._hasLightSource)
    {
      LightSource lightSource = this.netLightSource.Get();
      if (lightSource != null && this.isOn.Value && !location1.hasLightSource(lightSource.Id))
        location1.sharedLights.AddLight(lightSource.Clone());
    }
    if (this._machineAnimation != null)
    {
      List<int> frames = this._machineAnimation.Frames;
      // ISSUE: explicit non-virtual call
      if ((frames != null ? (__nonvirtual (frames.Count) > 0 ? 1 : 0) : 0) != 0)
      {
        this._machineAnimationInterval += (int) time.ElapsedGameTime.TotalMilliseconds;
        if (this._machineAnimation.Interval > 0 && this._machineAnimationInterval >= this._machineAnimation.Interval)
        {
          this._machineAnimationIndex += this._machineAnimationInterval / this._machineAnimation.Interval;
          this._machineAnimationInterval %= this._machineAnimation.Interval;
          if (this._machineAnimationIndex >= this._machineAnimation.Frames.Count)
          {
            if (this._machineAnimationLoop)
            {
              this._machineAnimationIndex %= this._machineAnimation.Frames.Count;
            }
            else
            {
              this._machineAnimation = (MachineEffects) null;
              this._machineAnimationFrame = -1;
            }
          }
        }
        if (this._machineAnimation != null)
          this._machineAnimationFrame = this._machineAnimation.Frames[this._machineAnimationIndex];
      }
      else
        this._machineAnimationFrame = -1;
    }
    if (this._hasHeldObject)
    {
      Object @object = this.heldObject.Get();
      if (@object.QualifiedItemId == "(O)913" && this.IsSprinkler() && @object.heldObject.Value is Chest chest)
      {
        chest.mutex.Update(location1);
        if (Game1.activeClickableMenu == null && chest.GetMutex().IsLockHeld())
          chest.GetMutex().ReleaseLock();
      }
      if (@object._hasLightSource)
      {
        this.lightSource = @object.netLightSource.Get();
        if (this.lightSource != null && !location1.hasLightSource(this.lightSource.Id))
          location1.sharedLights.AddLight(this.lightSource.Clone());
      }
      if (!this.readyForHarvest.Value)
      {
        if (this._machineAnimation == null)
        {
          MachineData machineData = this.GetMachineData();
          if (machineData?.WorkingEffects != null)
          {
            foreach (MachineEffects workingEffect in machineData.WorkingEffects)
            {
              if (workingEffect != null)
              {
                string condition = workingEffect.Condition;
                GameLocation location2 = this.Location;
                Item obj = this.lastInputItem.Value;
                Object targetItem = @object;
                Item inputItem = obj;
                if (GameStateQuery.CheckConditions(condition, location2, targetItem: (Item) targetItem, inputItem: inputItem))
                {
                  this._machineAnimation = workingEffect;
                  this._machineAnimationLoop = true;
                  this._machineAnimationIndex = 0;
                  this._machineAnimationFrame = -1;
                  MachineEffects machineAnimation = this._machineAnimation;
                  int num1;
                  if (machineAnimation == null)
                  {
                    num1 = 0;
                  }
                  else
                  {
                    int? count = machineAnimation.Frames?.Count;
                    int num2 = 0;
                    num1 = count.GetValueOrDefault() > num2 & count.HasValue ? 1 : 0;
                  }
                  this._machineAnimationInterval = num1 == 0 || this._machineAnimation.Interval <= 0 ? 0 : (int) (((double) (long) ((double) this.tileLocation.X * (double) (this._machineAnimation.Interval / 2) + (double) this.tileLocation.Y * (double) (this._machineAnimation.Interval / 2 * 10)) + time.TotalGameTime.TotalMilliseconds) % (double) (this._machineAnimation.Interval * this._machineAnimation.Frames.Count));
                  break;
                }
              }
            }
          }
        }
      }
      else if (this._machineAnimation != null && this._machineAnimationLoop)
        this._machineAnimation = (MachineEffects) null;
    }
    else if (this._machineAnimation != null && this._machineAnimationLoop)
      this._machineAnimation = (MachineEffects) null;
    if (this.shakeTimer > 0)
    {
      this.shakeTimer -= time.ElapsedGameTime.Milliseconds;
      if (this.shakeTimer <= 0)
        this.health = 10;
    }
    switch (this.QualifiedItemId)
    {
      case "(O)590":
      case "(O)SeedSpot":
        if (Game1.random.NextDouble() < 0.01)
        {
          this.shakeTimer = 100;
          break;
        }
        break;
      case "(BC)56":
        this.ResetParentSheetIndex();
        this.ParentSheetIndex += (int) (time.TotalGameTime.TotalMilliseconds % 600.0 / 100.0);
        break;
    }
    if (!this.IsTextSign())
      return;
    if (this.shouldShowSign)
    {
      this.shouldShowSign = false;
      this.lastNoteBlockSoundTime += (int) time.ElapsedGameTime.TotalMilliseconds;
      if (this.lastNoteBlockSoundTime <= 125)
        return;
      this.lastNoteBlockSoundTime = 125;
    }
    else
    {
      if (this.lastNoteBlockSoundTime <= 0)
        return;
      this.lastNoteBlockSoundTime -= (int) time.ElapsedGameTime.TotalMilliseconds;
      if (this.lastNoteBlockSoundTime >= 0)
        return;
      this.lastNoteBlockSoundTime = 0;
    }
  }

  /// <summary>Handle the player entering the location containing the object.</summary>
  public virtual void actionOnPlayerEntry()
  {
    this.isTemporarilyInvisible = false;
    this.health = 10;
    if (!(this.QualifiedItemId == "(BC)99"))
      return;
    this.showNextIndex.Value = this.Location.GetRootLocation().piecesOfHay.Value > 0;
  }

  /// <inheritdoc />
  public override bool canBeTrashed()
  {
    if (this.questItem.Value || !base.canBeTrashed())
      return false;
    ObjectData objectData;
    return !Game1.objectData.TryGetValue(this.ItemId, out objectData) || objectData.CanBeTrashed;
  }

  public virtual bool isForage()
  {
    return this.Category == -79 || this.Category == -81 || this.Category == -80 || this.Category == -75 || this.Category == -23 || this.HasContextTag("forage_item") || this.QualifiedItemId == "(O)430";
  }

  /// <summary>Initialize and register a light source for this instance, if applicable for its data and state.</summary>
  /// <param name="tileLocation">The object's tile position.</param>
  /// <param name="mineShaft">Whether the object is in the mines.</param>
  public virtual void initializeLightSource(Vector2 tileLocation, bool mineShaft = false)
  {
    if (this.name == "Error Item")
      return;
    if (this is Furniture furniture && furniture.furniture_type.Value == 14 && furniture.isOn.Value)
      this.lightSource = new LightSource(this.GenerateLightSourceId(tileLocation), 4, new Vector2((float) ((double) tileLocation.X * 64.0 + 32.0), (float) ((double) tileLocation.Y * 64.0 - 64.0)), 2.5f, new Color(0, 80 /*0x50*/, 160 /*0xA0*/), onlyLocation: this.Location?.NameOrUniqueName);
    else if (furniture != null && furniture.furniture_type.Value == 16 /*0x10*/ && furniture.isOn.Value)
    {
      this.lightSource = new LightSource(this.GenerateLightSourceId(tileLocation), 4, new Vector2((float) ((double) tileLocation.X * 64.0 + 32.0), (float) ((double) tileLocation.Y * 64.0 - 64.0)), 1.5f, new Color(0, 80 /*0x50*/, 160 /*0xA0*/), onlyLocation: this.Location?.NameOrUniqueName);
    }
    else
    {
      if (this.bigCraftable.Value)
      {
        if (this is Torch && this.isOn.Value)
        {
          float num = -64f;
          if (ItemContextTagManager.HasBaseTag(this.QualifiedItemId, "campfire_item"))
            num = 32f;
          this.lightSource = new LightSource(this.GenerateLightSourceId(tileLocation), 4, new Vector2((float) ((double) tileLocation.X * 64.0 + 32.0), tileLocation.Y * 64f + num), 2.5f, new Color(0, 80 /*0x50*/, 160 /*0xA0*/), onlyLocation: this.Location?.NameOrUniqueName);
          return;
        }
        if (this.isLamp.Value)
        {
          this.lightSource = new LightSource(this.GenerateLightSourceId(tileLocation), 4, new Vector2((float) ((double) tileLocation.X * 64.0 + 32.0), (float) ((double) tileLocation.Y * 64.0 - 64.0)), 3f, new Color(0, 40, 80 /*0x50*/), onlyLocation: this.Location?.NameOrUniqueName);
          return;
        }
        switch (this.QualifiedItemId)
        {
          case "(BC)74":
            this.lightSource = new LightSource(this.GenerateLightSourceId(tileLocation), 4, new Vector2((float) ((double) tileLocation.X * 64.0 + 32.0), tileLocation.Y * 64f), 1.5f, Color.DarkCyan, onlyLocation: this.Location?.NameOrUniqueName);
            return;
          case "(BC)96":
            this.lightSource = new LightSource(this.GenerateLightSourceId(tileLocation), 4, new Vector2((float) ((double) tileLocation.X * 64.0 + 32.0), tileLocation.Y * 64f), 1f, Color.HotPink * 0.75f, onlyLocation: this.Location?.NameOrUniqueName);
            return;
        }
      }
      else if (Utility.IsNormalObjectAtParentSheetIndex((Item) this, this.ItemId) || this is Torch)
      {
        if (this.QualifiedItemId == "(O)95" || ItemContextTagManager.HasBaseTag(this.QualifiedItemId, "torch_item"))
        {
          Color color;
          switch (this.ItemId)
          {
            case "94":
              color = Color.Yellow;
              break;
            case "95":
              color = new Color(70, 0, 150) * 0.9f;
              break;
            default:
              color = new Color(1, 1, 1) * 0.9f;
              break;
          }
          this.lightSource = new LightSource(this.GenerateLightSourceId(tileLocation), 4, new Vector2((float) ((double) tileLocation.X * 64.0 + 16.0), (float) ((double) tileLocation.Y * 64.0 + 16.0)), mineShaft ? 1.5f : 1.25f, color, onlyLocation: this.Location?.NameOrUniqueName);
          return;
        }
        if (this.QualifiedItemId == "(O)746")
        {
          this.lightSource = new LightSource(this.GenerateLightSourceId(tileLocation), 4, new Vector2((float) ((double) tileLocation.X * 64.0 + 32.0), (float) ((double) tileLocation.Y * 64.0 + 48.0)), 0.5f, new Color(1, 1, 1) * 0.65f, onlyLocation: this.Location?.NameOrUniqueName);
          return;
        }
        if (this.IsSprinkler() && this.SpecialVariable == 999999)
          this.lightSource = new LightSource(this.GenerateLightSourceId(tileLocation), 4, new Vector2((float) ((double) tileLocation.X * 64.0 + 16.0), (float) ((double) tileLocation.Y * 64.0 + 16.0)), 1.25f, new Color(1, 1, 1) * 0.9f, onlyLocation: this.Location?.NameOrUniqueName);
      }
      if (this.MinutesUntilReady <= 0)
        return;
      MachineLight lightWhileWorking = this.GetMachineData()?.LightWhileWorking;
      if (lightWhileWorking == null)
        return;
      this.lightSource = new LightSource(this.GenerateLightSourceId(tileLocation), 4, new Vector2((float) ((double) tileLocation.X * 64.0 + 32.0), tileLocation.Y * 64f), lightWhileWorking.Radius, Utility.StringToColor(lightWhileWorking.Color) ?? Color.White, onlyLocation: this.Location?.NameOrUniqueName);
    }
  }

  public virtual void performRemoveAction()
  {
    GameLocation location = this.Location;
    Vector2 tileLocation = this.TileLocation;
    if (location != null)
    {
      location.removeLightSource(this.lightSource?.Id);
      TerrainFeature terrainFeature;
      if (this.IsTapper() && location.terrainFeatures != null && location.terrainFeatures.TryGetValue(tileLocation, out terrainFeature) && terrainFeature is Tree tree)
        tree.tapped.Value = false;
      if (this.IsSprinkler())
        location.removeTemporarySpritesWithID((int) tileLocation.X * 4000 + (int) tileLocation.Y);
    }
    if (this.QualifiedItemId == "(BC)126")
    {
      string itemId = this.quality.Value != 0 ? (this.quality.Value - 1).ToString() : this.preservedParentSheetIndex.Value;
      if (itemId != null)
      {
        Game1.createItemDebris((Item) new Hat(itemId), tileLocation * 64f, (Game1.player.FacingDirection + 2) % 4);
        this.quality.Value = 0;
        this.preservedParentSheetIndex.Value = (string) null;
      }
    }
    if (!this.name.Contains("Seasonal") || !this.bigCraftable.Value)
      return;
    this.ResetParentSheetIndex();
  }

  public virtual void dropItem(GameLocation location, Vector2 origin, Vector2 destination)
  {
    if (!(this.Type == "Crafting") && !(this.Type == "interactive") || this.fragility.Value == 2)
      return;
    location.debris.Add(new Debris(this.QualifiedItemId, origin, destination));
  }

  public virtual bool isPassable()
  {
    if (this.isTemporarilyInvisible)
      return true;
    if (this.bigCraftable.Value)
      return false;
    string qualifiedItemId1 = this.QualifiedItemId;
    if (qualifiedItemId1 != null)
    {
      switch (qualifiedItemId1.Length)
      {
        case 5:
          if (!(qualifiedItemId1 == "(O)93"))
            goto label_18;
          break;
        case 6:
          switch (qualifiedItemId1[5])
          {
            case '0':
              if (qualifiedItemId1 == "(O)590")
                break;
              goto label_18;
            case '3':
              if (qualifiedItemId1 == "(O)893")
                break;
              goto label_18;
            case '4':
              if (qualifiedItemId1 == "(O)894")
                break;
              goto label_18;
            case '5':
              if (qualifiedItemId1 == "(O)895")
                break;
              goto label_18;
            case '6':
              if (qualifiedItemId1 == "(O)286")
                break;
              goto label_18;
            case '7':
              if (qualifiedItemId1 == "(O)287" || qualifiedItemId1 == "(O)297")
                break;
              goto label_18;
            case '8':
              if (qualifiedItemId1 == "(O)288")
                break;
              goto label_18;
            default:
              goto label_18;
          }
          break;
        case 11:
          if (qualifiedItemId1 == "(O)SeedSpot")
            break;
          goto label_18;
        case 19:
          if (qualifiedItemId1 == "(O)BlueGrassStarter")
            break;
          goto label_18;
        default:
          goto label_18;
      }
      return true;
    }
label_18:
    if (this.IsFloorPathItem())
      return true;
    if (this.Category != -74 && this.Category != -19 || this.isSapling())
      return false;
    string qualifiedItemId2 = this.QualifiedItemId;
    return !(qualifiedItemId2 == "(O)301") && !(qualifiedItemId2 == "(O)302") && !(qualifiedItemId2 == "(O)473");
  }

  public virtual void reloadSprite() => this.initializeLightSource(this.tileLocation.Value);

  /// <summary>Get the pixel area containing the object.</summary>
  public Microsoft.Xna.Framework.Rectangle GetBoundingBox()
  {
    Vector2 vector2 = this.tileLocation.Value;
    return this.GetBoundingBoxAt((int) vector2.X, (int) vector2.Y);
  }

  /// <summary>Get the pixel area containing the object, adjusted for the given tile position.</summary>
  /// <param name="x">The tile X position to use instead of the object's current position.</param>
  /// <param name="y">The tile Y position to use instead of the object's current position.</param>
  public virtual Microsoft.Xna.Framework.Rectangle GetBoundingBoxAt(int x, int y)
  {
    Microsoft.Xna.Framework.Rectangle boundingBoxAt = this.boundingBox.Value;
    if (this is Torch && !this.bigCraftable.Value || this.QualifiedItemId == "(O)590")
    {
      boundingBoxAt.X = (int) this.tileLocation.X * 64 /*0x40*/ + 24;
      boundingBoxAt.Y = (int) this.tileLocation.Y * 64 /*0x40*/ + 24;
    }
    else
    {
      boundingBoxAt.X = (int) this.tileLocation.X * 64 /*0x40*/;
      boundingBoxAt.Y = (int) this.tileLocation.Y * 64 /*0x40*/;
    }
    if (this.boundingBox.Value != boundingBoxAt)
      this.boundingBox.Value = boundingBoxAt;
    return boundingBoxAt;
  }

  /// <inheritdoc />
  public override bool canBeGivenAsGift()
  {
    if (this.bigCraftable.Value || this is Furniture || this is Wallpaper)
      return false;
    ObjectData objectData;
    return !Game1.objectData.TryGetValue(this.ItemId, out objectData) || objectData.CanBeGivenAsGift;
  }

  /// <summary>Update the object instance before it's placed in the world.</summary>
  /// <param name="who">The player placing the item.</param>
  /// <returns>Returns <c>true</c> if the item should be destroyed, or <c>false</c> if it should be set down.</returns>
  /// <remarks>This is called on the object instance being placed, after it's already been split from the inventory stack if applicable. At this point the <see cref="P:StardewValley.Object.Location" /> and <see cref="P:StardewValley.Object.TileLocation" /> values should already be set.</remarks>
  public virtual bool performDropDownAction(Farmer who)
  {
    if (who == null)
      who = Game1.GetPlayer(this.owner.Value) ?? Game1.player;
    GameLocation location = this.Location;
    MachineData machineData = this.GetMachineData();
    MachineOutputRule rule;
    if (MachineDataUtility.TryGetMachineOutputRule(this, machineData, MachineOutputTrigger.MachinePutDown, (Item) null, who, location, out rule, out MachineOutputTriggerRule _, out MachineOutputRule _, out MachineOutputTriggerRule _))
    {
      this.OutputMachine(machineData, rule, (Item) null, who, location, false);
      return false;
    }
    switch (this.QualifiedItemId)
    {
      case "(BC)96":
        this.minutesUntilReady.Value = Utility.CalculateMinutesUntilMorning(Game1.timeOfDay, 3);
        break;
      case "(BC)99":
        this.showNextIndex.Value = location.GetRootLocation().piecesOfHay.Value >= 0;
        break;
    }
    return false;
  }

  private void totemWarp(Farmer who)
  {
    GameLocation currentLocation = who.currentLocation;
    for (int index = 0; index < 12; ++index)
      Game1.multiplayer.broadcastSprites(currentLocation, new TemporaryAnimatedSprite(354, (float) Game1.random.Next(25, 75), 6, 1, new Vector2((float) Game1.random.Next((int) who.Position.X - 256 /*0x0100*/, (int) who.Position.X + 192 /*0xC0*/), (float) Game1.random.Next((int) who.Position.Y - 256 /*0x0100*/, (int) who.Position.Y + 192 /*0xC0*/)), false, Game1.random.NextBool()));
    who.playNearbySoundAll("wand");
    Game1.displayFarmer = false;
    Game1.player.temporarilyInvincible = true;
    Game1.player.temporaryInvincibilityTimer = -2000;
    Game1.player.freezePause = 1000;
    Game1.flashAlpha = 1f;
    DelayedAction.fadeAfterDelay(new Game1.afterFadeFunction(this.totemWarpForReal), 1000);
    Microsoft.Xna.Framework.Rectangle boundingBox = who.GetBoundingBox();
    new Microsoft.Xna.Framework.Rectangle(boundingBox.X, boundingBox.Y, 64 /*0x40*/, 64 /*0x40*/).Inflate(192 /*0xC0*/, 192 /*0xC0*/);
    int num = 0;
    Point tilePoint = who.TilePoint;
    for (int x = tilePoint.X + 8; x >= tilePoint.X - 8; --x)
    {
      Game1.multiplayer.broadcastSprites(currentLocation, new TemporaryAnimatedSprite(6, new Vector2((float) x, (float) tilePoint.Y) * 64f, Color.White, animationInterval: 50f)
      {
        layerDepth = 1f,
        delayBeforeAnimationStart = num * 25,
        motion = new Vector2(-0.25f, 0.0f)
      });
      ++num;
    }
  }

  private void totemWarpForReal()
  {
    switch (this.QualifiedItemId)
    {
      case "(O)688":
        Point parsed;
        if (!Game1.getFarm().TryGetMapPropertyAs("WarpTotemEntry", out parsed))
        {
          switch (Game1.whichFarm)
          {
            case 5:
              parsed = new Point(48 /*0x30*/, 39);
              break;
            case 6:
              parsed = new Point(82, 29);
              break;
            default:
              parsed = new Point(48 /*0x30*/, 7);
              break;
          }
        }
        Game1.warpFarmer("Farm", parsed.X, parsed.Y, false);
        break;
      case "(O)689":
        Game1.warpFarmer("Mountain", 31 /*0x1F*/, 20, false);
        break;
      case "(O)690":
        Game1.warpFarmer("Beach", 20, 4, false);
        break;
      case "(O)261":
        Game1.warpFarmer("Desert", 35, 43, false);
        break;
      case "(O)886":
        Game1.warpFarmer("IslandSouth", 11, 11, false);
        break;
    }
    Game1.fadeToBlackAlpha = 0.99f;
    Game1.screenGlow = false;
    Game1.player.temporarilyInvincible = false;
    Game1.player.temporaryInvincibilityTimer = 0;
    Game1.displayFarmer = true;
  }

  public void MonsterMusk(Farmer who)
  {
    GameLocation currentLocation = who.currentLocation;
    who.FarmerSprite.PauseForSingleAnimation = false;
    who.FarmerSprite.StopAnimation();
    who.FarmerSprite.animateOnce(new FarmerSprite.AnimationFrame[4]
    {
      new FarmerSprite.AnimationFrame(104, 350, false, false),
      new FarmerSprite.AnimationFrame(105, 350, false, false),
      new FarmerSprite.AnimationFrame(104, 350, false, false),
      new FarmerSprite.AnimationFrame(105, 350, false, false)
    });
    currentLocation.playSound("croak");
    who.applyBuff("24");
  }

  public override void ModifyItemBuffs(BuffEffects effects)
  {
    if (effects != null && this.Category == -7)
    {
      int num = 0;
      if (this.Quality != 0)
        num = 1;
      if (num > 0)
      {
        NetFloat[] netFloatArray = new NetFloat[9]
        {
          effects.FarmingLevel,
          effects.FishingLevel,
          effects.MiningLevel,
          effects.LuckLevel,
          effects.ForagingLevel,
          effects.MaxStamina,
          effects.MagneticRadius,
          effects.Defense,
          effects.Attack
        };
        foreach (NetFloat netFloat in netFloatArray)
        {
          if ((double) netFloat.Value != 0.0)
            netFloat.Value += (float) num;
        }
      }
    }
    base.ModifyItemBuffs(effects);
  }

  private void treasureTotem(Farmer who, GameLocation gameLocation)
  {
    Game1.playSound("treasure_totem");
    ++Game1.netWorldState.Value.TreasureTotemsUsed;
    Vector2 tile = who.Tile;
    int num = 4;
    for (int index1 = (int) tile.X - num; (double) index1 < (double) tile.X + (double) num; ++index1)
    {
      for (int index2 = (int) tile.Y - num; (double) index2 < (double) tile.Y + (double) num; ++index2)
      {
        if (Math.Round((double) Utility.distance((float) index1, tile.X, (float) index2, tile.Y)) == (double) (num - 1))
        {
          Vector2 vector2 = new Vector2((float) index1, (float) index2);
          if (gameLocation.CanItemBePlacedHere(vector2) && !gameLocation.IsTileOccupiedBy(vector2) && !gameLocation.hasTileAt(index1, index2, "AlwaysFront") && !gameLocation.hasTileAt(index1, index2, "Front") && !gameLocation.isBehindBush(vector2) && (gameLocation.doesTileHaveProperty(index1, index2, "Diggable", "Back") != null || gameLocation.GetSeason() == Season.Winter && gameLocation.doesTileHaveProperty(index1, index2, "Type", "Back") == "Grass"))
          {
            if ((!this.name.Equals("Forest") || index1 < 93 || index2 > 22) && gameLocation.IsOutdoors)
              gameLocation.objects.Add(vector2, ItemRegistry.Create<Object>("(O)590"));
            else
              continue;
          }
          Utility.addRainbowStarExplosion(gameLocation, new Vector2((float) index1, (float) index2) * 64f, 1);
          Utility.addStarsAndSpirals(gameLocation, index1, index2, 1, 1, 100, 100, Color.White);
        }
        if (Math.Round((double) Utility.distance((float) index1, tile.X, (float) index2, tile.Y)) <= (double) (num - 1))
          Utility.makeTemporarySpriteJuicier(new TemporaryAnimatedSprite("LooseSprites\\Cursors_1_6", new Microsoft.Xna.Framework.Rectangle(144 /*0x90*/, 249, 7, 7), (float) Game1.random.Next(100, 200), 6, 1, new Vector2((float) index1, (float) index2) * 64f + new Vector2((float) (32 /*0x20*/ + Game1.random.Next(-16, 16 /*0x10*/)), (float) (32 /*0x20*/ + Game1.random.Next(-16, 16 /*0x10*/))), false, false, 1f / 1000f, 0.0f, Game1.random.NextDouble() < 0.5 ? new Color((int) byte.MaxValue, (int) byte.MaxValue, 100) : Color.White, 4f, 0.0f, 0.0f, 0.0f), gameLocation);
      }
    }
  }

  private void rainTotem(Farmer who)
  {
    GameLocation currentLocation = who.currentLocation;
    string str = currentLocation.GetLocationContextId();
    LocationContextData locationContext = currentLocation.GetLocationContext();
    if (!locationContext.AllowRainTotem)
    {
      Game1.showRedMessageUsingLoadString("Strings\\UI:Item_CantBeUsedHere");
    }
    else
    {
      if (locationContext.RainTotemAffectsContext != null)
        str = locationContext.RainTotemAffectsContext;
      bool flag = false;
      if (str == "Default")
      {
        if (!Utility.isFestivalDay(Game1.dayOfMonth + 1, Game1.season))
        {
          Game1.netWorldState.Value.WeatherForTomorrow = Game1.weatherForTomorrow = "Rain";
          flag = true;
        }
      }
      else
      {
        currentLocation.GetWeather().WeatherForTomorrow = "Rain";
        flag = true;
      }
      if (flag)
        Game1.pauseThenMessage(2000, Game1.content.LoadString("Strings\\StringsFromCSFiles:Object.cs.12822"));
      Game1.screenGlow = false;
      currentLocation.playSound("thunder");
      who.canMove = false;
      Game1.screenGlowOnce(Color.SlateBlue, false);
      Game1.player.faceDirection(2);
      Game1.player.FarmerSprite.animateOnce(new FarmerSprite.AnimationFrame[1]
      {
        new FarmerSprite.AnimationFrame(57, 2000, false, false, new AnimatedSprite.endOfAnimationBehavior(Farmer.canMoveNow), true)
      });
      for (int index = 0; index < 6; ++index)
      {
        Game1.multiplayer.broadcastSprites(currentLocation, new TemporaryAnimatedSprite("LooseSprites\\Cursors", new Microsoft.Xna.Framework.Rectangle(648, 1045, 52, 33), 9999f, 1, 999, who.Position + new Vector2(0.0f, (float) sbyte.MinValue), false, false, 1f, 0.01f, Color.White * 0.8f, 2f, 0.01f, 0.0f, 0.0f)
        {
          motion = new Vector2((float) Game1.random.Next(-10, 11) / 10f, -2f),
          delayBeforeAnimationStart = index * 200
        });
        Game1.multiplayer.broadcastSprites(currentLocation, new TemporaryAnimatedSprite("LooseSprites\\Cursors", new Microsoft.Xna.Framework.Rectangle(648, 1045, 52, 33), 9999f, 1, 999, who.Position + new Vector2(0.0f, (float) sbyte.MinValue), false, false, 1f, 0.01f, Color.White * 0.8f, 1f, 0.01f, 0.0f, 0.0f)
        {
          motion = new Vector2((float) Game1.random.Next(-30, -10) / 10f, -1f),
          delayBeforeAnimationStart = 100 + index * 200
        });
        Game1.multiplayer.broadcastSprites(currentLocation, new TemporaryAnimatedSprite("LooseSprites\\Cursors", new Microsoft.Xna.Framework.Rectangle(648, 1045, 52, 33), 9999f, 1, 999, who.Position + new Vector2(0.0f, (float) sbyte.MinValue), false, false, 1f, 0.01f, Color.White * 0.8f, 1f, 0.01f, 0.0f, 0.0f)
        {
          motion = new Vector2((float) Game1.random.Next(10, 30) / 10f, -1f),
          delayBeforeAnimationStart = 200 + index * 200
        });
      }
      TemporaryAnimatedSprite temporaryAnimatedSprite = new TemporaryAnimatedSprite(0, 9999f, 1, 999, Game1.player.Position + new Vector2(0.0f, -96f), false, false, false, 0.0f)
      {
        motion = new Vector2(0.0f, -7f),
        acceleration = new Vector2(0.0f, 0.1f),
        scaleChange = 0.015f,
        alpha = 1f,
        alphaFade = 0.0075f,
        shakeIntensity = 1f,
        initialPosition = Game1.player.Position + new Vector2(0.0f, -96f),
        xPeriodic = true,
        xPeriodicLoopTime = 1000f,
        xPeriodicRange = 4f,
        layerDepth = 1f
      };
      temporaryAnimatedSprite.CopyAppearanceFromItemId(this.QualifiedItemId);
      Game1.multiplayer.broadcastSprites(currentLocation, temporaryAnimatedSprite);
      DelayedAction.playSoundAfterDelay("rainsound", 2000);
    }
  }

  private void readBook(GameLocation location)
  {
    Game1.player.canMove = false;
    Game1.player.freezePause = 1030;
    Game1.player.faceDirection(2);
    Game1.player.FarmerSprite.animateOnce(new FarmerSprite.AnimationFrame[1]
    {
      new FarmerSprite.AnimationFrame(57, 1000, false, false, new AnimatedSprite.endOfAnimationBehavior(Farmer.canMoveNow), true)
      {
        frameEndBehavior = (AnimatedSprite.endOfAnimationBehavior) (x =>
        {
          location.removeTemporarySpritesWithID(1987654);
          Utility.addRainbowStarExplosion(location, Game1.player.getStandingPosition() + new Vector2(-40f, -156f), 8);
        })
      }
    });
    Game1.MusicDuckTimer = 4000f;
    Game1.playSound("book_read");
    Game1.multiplayer.broadcastSprites(location, new TemporaryAnimatedSprite("LooseSprites\\Book_Animation", new Microsoft.Xna.Framework.Rectangle(0, 0, 20, 20), 10f, 45, 1, Game1.player.getStandingPosition() + new Vector2(-48f, -156f), false, false, Game1.player.getDrawLayer() + 1f / 1000f, 0.0f, Color.White, 4f, 0.0f, 0.0f, 0.0f)
    {
      holdLastFrame = true,
      id = 1987654
    });
    Color? colorFromTags = ItemContextTagManager.GetColorFromTags((Item) this);
    if (colorFromTags.HasValue)
      Game1.multiplayer.broadcastSprites(location, new TemporaryAnimatedSprite("LooseSprites\\Book_Animation", new Microsoft.Xna.Framework.Rectangle(0, 20, 20, 20), 10f, 45, 1, Game1.player.getStandingPosition() + new Vector2(-48f, -156f), false, false, Game1.player.getDrawLayer() + 0.0012f, 0.0f, colorFromTags.Value, 4f, 0.0f, 0.0f, 0.0f)
      {
        holdLastFrame = true,
        id = 1987654
      });
    if (this.ItemId.StartsWith("SkillBook_"))
    {
      int count = Game1.player.newLevels.Count;
      Game1.player.gainExperience(Convert.ToInt32(this.ItemId.Last<char>().ToString() ?? ""), 250);
      if (Game1.player.newLevels.Count != count && (Game1.player.newLevels.Count <= 1 || count < 1))
        return;
      DelayedAction.functionAfterDelay((Action) (() => Game1.showGlobalMessage(Game1.content.LoadString("Strings\\1_6_Strings:SkillBookMessage", (object) Game1.content.LoadString("Strings\\1_6_Strings:SkillName_" + this.ItemId.Last<char>().ToString()).ToLower()))), 1000);
    }
    else if (Game1.player.stats.Get(this.itemId.Value) > 0U && this.ItemId != "Book_PriceCatalogue" && this.ItemId != "Book_AnimalCatalogue")
    {
      if (!Game1.player.mailReceived.Contains("read_a_book"))
        Game1.player.mailReceived.Add("read_a_book");
      bool flag = false;
      foreach (string contextTag in this.GetContextTags())
      {
        if (contextTag.StartsWithIgnoreCase("book_xp_"))
        {
          flag = true;
          Game1.player.gainExperience(Farmer.getSkillNumberFromName(contextTag.Split('_')[2]), 100);
          break;
        }
      }
      if (flag)
        return;
      for (int which = 0; which < 5; ++which)
        Game1.player.gainExperience(which, 20);
    }
    else
    {
      switch (this.ItemId)
      {
        case "Book_QueenOfSauce":
          Dictionary<string, string> dictionary = DataLoader.Tv_CookingChannel(Game1.content);
          int num1 = 0;
          foreach (KeyValuePair<string, string> keyValuePair in dictionary)
          {
            if (Game1.player.cookingRecipes.TryAdd(keyValuePair.Value.Split("/")[0], 0))
              ++num1;
          }
          int num2 = (int) Game1.player.stats.Increment(this.itemId.Value);
          Game1.showGlobalMessage(Game1.content.LoadString("Strings\\1_6_Strings:QoS_Cookbook", (object) (num1.ToString() ?? "")));
          break;
        case "PurpleBook":
          Game1.player.gainExperience(0, 250);
          Game1.player.gainExperience(1, 250);
          Game1.player.gainExperience(2, 250);
          Game1.player.gainExperience(3, 250);
          Game1.player.gainExperience(4, 250);
          break;
        default:
          int num3 = (int) Game1.player.stats.Increment(this.itemId.Value);
          DelayedAction.functionAfterDelay((Action) (() => Game1.showGlobalMessage(Game1.content.LoadString("Strings\\1_6_Strings:LearnedANewPower"))), 1000);
          if (!Game1.player.mailReceived.Contains("read_a_book"))
            Game1.player.mailReceived.Add("read_a_book");
          Game1.stats.checkForBooksReadAchievement();
          break;
      }
    }
  }

  public virtual bool performUseAction(GameLocation location)
  {
    if (!Game1.player.canMove || this.isTemporarilyInvisible)
      return false;
    bool flag = !Game1.eventUp && !Game1.isFestival() && !Game1.fadeToBlack && !Game1.player.swimming.Value && !Game1.player.bathingClothes.Value && !Game1.player.onBridge.Value;
    if (flag && (this.Category == -102 || this.Category == -103))
    {
      this.readBook(location);
      return true;
    }
    if (this.name.Contains("Totem"))
    {
      if (flag)
      {
        string qualifiedItemId = this.QualifiedItemId;
        if (qualifiedItemId != null)
        {
          switch (qualifiedItemId.Length)
          {
            case 6:
              switch (qualifiedItemId[5])
              {
                case '0':
                  if (qualifiedItemId == "(O)690")
                    break;
                  goto label_34;
                case '1':
                  switch (qualifiedItemId)
                  {
                    case "(O)681":
                      this.rainTotem(Game1.player);
                      return true;
                    case "(O)261":
                      break;
                    default:
                      goto label_34;
                  }
                  break;
                case '6':
                  if (qualifiedItemId == "(O)886")
                    break;
                  goto label_34;
                case '8':
                  if (qualifiedItemId == "(O)688")
                    break;
                  goto label_34;
                case '9':
                  if (qualifiedItemId == "(O)689")
                    break;
                  goto label_34;
                default:
                  goto label_34;
              }
              Game1.player.jitterStrength = 1f;
              Color glowColor = this.QualifiedItemId == "(O)681" ? Color.SlateBlue : (this.QualifiedItemId == "(O)688" ? Color.LimeGreen : (this.QualifiedItemId == "(O)689" ? Color.OrangeRed : (this.QualifiedItemId == "(O)261" ? new Color((int) byte.MaxValue, 200, 0) : Color.LightBlue)));
              location.playSound("warrior");
              Game1.player.faceDirection(2);
              Game1.player.CanMove = false;
              Game1.player.temporarilyInvincible = true;
              Game1.player.temporaryInvincibilityTimer = -4000;
              Game1.changeMusicTrack("silence");
              if (this.QualifiedItemId == "(O)681")
                Game1.player.FarmerSprite.animateOnce(new FarmerSprite.AnimationFrame[2]
                {
                  new FarmerSprite.AnimationFrame(57, 2000, false, false),
                  new FarmerSprite.AnimationFrame((int) (short) Game1.player.FarmerSprite.CurrentFrame, 0, false, false, new AnimatedSprite.endOfAnimationBehavior(this.rainTotem), true)
                });
              else
                Game1.player.FarmerSprite.animateOnce(new FarmerSprite.AnimationFrame[2]
                {
                  new FarmerSprite.AnimationFrame(57, 2000, false, false),
                  new FarmerSprite.AnimationFrame((int) (short) Game1.player.FarmerSprite.CurrentFrame, 0, false, false, new AnimatedSprite.endOfAnimationBehavior(this.totemWarp), true)
                });
              TemporaryAnimatedSprite temporaryAnimatedSprite1 = new TemporaryAnimatedSprite(0, 9999f, 1, 999, Game1.player.Position + new Vector2(0.0f, -96f), false, false, false, 0.0f)
              {
                motion = new Vector2(0.0f, -1f),
                scaleChange = 0.01f,
                alpha = 1f,
                alphaFade = 0.0075f,
                shakeIntensity = 1f,
                initialPosition = Game1.player.Position + new Vector2(0.0f, -96f),
                xPeriodic = true,
                xPeriodicLoopTime = 1000f,
                xPeriodicRange = 4f,
                layerDepth = 1f
              };
              temporaryAnimatedSprite1.CopyAppearanceFromItemId(this.QualifiedItemId);
              Game1.multiplayer.broadcastSprites(location, temporaryAnimatedSprite1);
              TemporaryAnimatedSprite temporaryAnimatedSprite2 = new TemporaryAnimatedSprite(0, 9999f, 1, 999, Game1.player.Position + new Vector2(-64f, -96f), false, false, false, 0.0f)
              {
                motion = new Vector2(0.0f, -0.5f),
                scaleChange = 0.005f,
                scale = 0.5f,
                alpha = 1f,
                alphaFade = 0.0075f,
                shakeIntensity = 1f,
                delayBeforeAnimationStart = 10,
                initialPosition = Game1.player.Position + new Vector2(-64f, -96f),
                xPeriodic = true,
                xPeriodicLoopTime = 1000f,
                xPeriodicRange = 4f,
                layerDepth = 0.9999f
              };
              temporaryAnimatedSprite2.CopyAppearanceFromItemId(this.QualifiedItemId);
              Game1.multiplayer.broadcastSprites(location, temporaryAnimatedSprite2);
              TemporaryAnimatedSprite temporaryAnimatedSprite3 = new TemporaryAnimatedSprite(0, 9999f, 1, 999, Game1.player.Position + new Vector2(64f, -96f), false, false, false, 0.0f)
              {
                motion = new Vector2(0.0f, -0.5f),
                scaleChange = 0.005f,
                scale = 0.5f,
                alpha = 1f,
                alphaFade = 0.0075f,
                delayBeforeAnimationStart = 20,
                shakeIntensity = 1f,
                initialPosition = Game1.player.Position + new Vector2(64f, -96f),
                xPeriodic = true,
                xPeriodicLoopTime = 1000f,
                xPeriodicRange = 4f,
                layerDepth = 0.9988f
              };
              temporaryAnimatedSprite3.CopyAppearanceFromItemId(this.QualifiedItemId);
              Game1.multiplayer.broadcastSprites(location, temporaryAnimatedSprite3);
              Game1.screenGlowOnce(glowColor, false);
              Utility.addSprinklesToLocation(location, Game1.player.TilePoint.X, Game1.player.TilePoint.Y, 16 /*0x10*/, 16 /*0x10*/, 1300, 20, Color.White, motionTowardCenter: true);
              return true;
            case 16 /*0x10*/:
              if (qualifiedItemId == "(O)TreasureTotem")
              {
                if (!location.IsOutdoors)
                {
                  Game1.showRedMessageUsingLoadString("Strings\\StringsFromCSFiles:Object.cs.13053");
                  return false;
                }
                this.treasureTotem(Game1.player, location);
                return true;
              }
              break;
          }
        }
      }
    }
    else if (this.QualifiedItemId == "(O)79" || this.QualifiedItemId == "(O)842")
    {
      bool journal = this.QualifiedItemId == "(O)842";
      int[] unseenSecretNotes = Utility.GetUnseenSecretNotes(Game1.player, journal, out int _);
      if (unseenSecretNotes.Length == 0)
        return false;
      Random random = Utility.CreateRandom((double) Game1.uniqueIDForThisGame, (double) Game1.player.UniqueMultiplayerID, (double) (unseenSecretNotes.Length * 777));
      int secretNoteIndex = journal ? ((IEnumerable<int>) unseenSecretNotes).Min() : random.ChooseFrom<int>((IList<int>) unseenSecretNotes);
      if (!Game1.player.secretNotesSeen.Add(secretNoteIndex))
        return false;
      switch (secretNoteIndex)
      {
        case 10:
          if (!Game1.player.mailReceived.Contains("qiCave"))
          {
            Game1.player.addQuest("30");
            break;
          }
          break;
        case 23:
          if (!Game1.player.eventsSeen.Contains("2120303"))
          {
            Game1.player.addQuest("29");
            break;
          }
          break;
      }
      Game1.activeClickableMenu = (IClickableMenu) new LetterViewerMenu(secretNoteIndex);
      return true;
    }
label_34:
    if (this.QualifiedItemId == "(O)911")
    {
      if (!flag)
        return false;
      string warpErrorMessage1 = Utility.GetHorseWarpErrorMessage(Utility.GetHorseWarpRestrictionsForFarmer(Game1.player));
      if (warpErrorMessage1 != null)
      {
        Game1.showRedMessage(warpErrorMessage1);
      }
      else
      {
        Horse horse1 = (Horse) null;
        foreach (NPC character in location.characters)
        {
          if (character is Horse horse2 && horse2.getOwner() == Game1.player)
          {
            horse1 = horse2;
            break;
          }
        }
        if (horse1 == null || Math.Abs(Game1.player.TilePoint.X - horse1.TilePoint.X) > 1 || Math.Abs(Game1.player.TilePoint.Y - horse1.TilePoint.Y) > 1)
        {
          Game1.player.faceDirection(2);
          Game1.MusicDuckTimer = 2000f;
          Game1.playSound("horse_flute");
          Game1.player.FarmerSprite.animateOnce(new FarmerSprite.AnimationFrame[6]
          {
            new FarmerSprite.AnimationFrame(98, 400, true, false),
            new FarmerSprite.AnimationFrame(99, 200, true, false),
            new FarmerSprite.AnimationFrame(100, 200, true, false),
            new FarmerSprite.AnimationFrame(99, 200, true, false),
            new FarmerSprite.AnimationFrame(98, 400, true, false),
            new FarmerSprite.AnimationFrame(99, 200, true, false)
          });
          Game1.player.freezePause = 1500;
          DelayedAction.functionAfterDelay((Action) (() =>
          {
            string warpErrorMessage2 = Utility.GetHorseWarpErrorMessage(Utility.GetHorseWarpRestrictionsForFarmer(Game1.player));
            if (warpErrorMessage2 != null)
              Game1.showRedMessage(warpErrorMessage2);
            else
              Game1.player.team.requestHorseWarpEvent.Fire(Game1.player.UniqueMultiplayerID);
          }), 1500);
        }
        ++this.stack.Value;
        return true;
      }
    }
    if (!(this.QualifiedItemId == "(O)879") || !flag)
      return false;
    Game1.player.faceDirection(2);
    Game1.player.freezePause = 1750;
    Game1.player.FarmerSprite.animateOnce(new FarmerSprite.AnimationFrame[2]
    {
      new FarmerSprite.AnimationFrame(57, 750, false, false),
      new FarmerSprite.AnimationFrame((int) (short) Game1.player.FarmerSprite.CurrentFrame, 0, false, false, new AnimatedSprite.endOfAnimationBehavior(this.MonsterMusk), true)
    });
    for (int index = 0; index < 3; ++index)
      Game1.multiplayer.broadcastSprites(location, new TemporaryAnimatedSprite(5, new Vector2(16f, (float) (32 /*0x20*/ * index - 64 /*0x40*/)), Color.Purple)
      {
        motion = new Vector2(Utility.RandomFloat(-1f, 1f), -0.5f),
        scaleChange = 0.005f,
        scale = 0.5f,
        alpha = 1f,
        alphaFade = 0.0075f,
        shakeIntensity = 1f,
        delayBeforeAnimationStart = 100 * index,
        layerDepth = 0.9999f,
        positionFollowsAttachedCharacter = true,
        attachedCharacter = (Character) Game1.player
      });
    location.playSound("steam");
    return true;
  }

  /// <inheritdoc />
  public override Color getCategoryColor()
  {
    return this.type.Value == "Arch" ? new Color(110, 0, 90) : base.getCategoryColor();
  }

  /// <inheritdoc />
  public override string getCategoryName()
  {
    if (this is Furniture furniture)
    {
      switch (furniture.placementRestriction)
      {
        case 1:
          return Game1.content.LoadString("Strings\\StringsFromCSFiles:Furniture_Outdoors");
        case 2:
          return Game1.content.LoadString("Strings\\StringsFromCSFiles:Furniture_Decoration");
        default:
          return Game1.content.LoadString("Strings\\StringsFromCSFiles:Object.cs.12847");
      }
    }
    else
      return this.Type == "Arch" ? Game1.content.LoadString("Strings\\StringsFromCSFiles:Object.cs.12849") : base.getCategoryName();
  }

  /// <summary>Get the translated display name for an object category, if any.</summary>
  /// <param name="category">The object category.</param>
  /// <returns>Returns the display name, or an empty string if there is none.</returns>
  public static string GetCategoryDisplayName(int category)
  {
    switch (category)
    {
      case -103:
        return Game1.content.LoadString("Strings\\1_6_Strings:skillBook_Category");
      case -102:
        return Game1.content.LoadString("Strings\\1_6_Strings:Book_Category");
      case -100:
        return Game1.content.LoadString("Strings\\StringsFromCSFiles:category_clothes");
      case -99:
        return Game1.content.LoadString("Strings\\StringsFromCSFiles:Tool.cs.14307");
      case -97:
        return Game1.content.LoadString("Strings\\StringsFromCSFiles:Boots.cs.12501");
      case -96:
        return Game1.content.LoadString("Strings\\StringsFromCSFiles:Ring.cs.1");
      case -81:
        return Game1.content.LoadString("Strings\\StringsFromCSFiles:Object.cs.12869");
      case -80:
        return Game1.content.LoadString("Strings\\StringsFromCSFiles:Object.cs.12866");
      case -79:
        return Game1.content.LoadString("Strings\\StringsFromCSFiles:Object.cs.12854");
      case -75:
        return Game1.content.LoadString("Strings\\StringsFromCSFiles:Object.cs.12851");
      case -74:
        return Game1.content.LoadString("Strings\\StringsFromCSFiles:Object.cs.12855");
      case -28:
        return Game1.content.LoadString("Strings\\StringsFromCSFiles:Object.cs.12867");
      case -27:
      case -26:
        return Game1.content.LoadString("Strings\\StringsFromCSFiles:Object.cs.12862");
      case -25:
      case -7:
        return Game1.content.LoadString("Strings\\StringsFromCSFiles:Object.cs.12853");
      case -24:
        return Game1.content.LoadString("Strings\\StringsFromCSFiles:Object.cs.12859");
      case -22:
        return Game1.content.LoadString("Strings\\StringsFromCSFiles:Object.cs.12858");
      case -21:
        return Game1.content.LoadString("Strings\\StringsFromCSFiles:Object.cs.12857");
      case -20:
        return Game1.content.LoadString("Strings\\StringsFromCSFiles:Object.cs.12860");
      case -19:
        return Game1.content.LoadString("Strings\\StringsFromCSFiles:Object.cs.12856");
      case -18:
      case -14:
      case -6:
      case -5:
        return Game1.content.LoadString("Strings\\StringsFromCSFiles:Object.cs.12864");
      case -16:
      case -15:
        return Game1.content.LoadString("Strings\\StringsFromCSFiles:Object.cs.12868");
      case -12:
      case -2:
        return Game1.content.LoadString("Strings\\StringsFromCSFiles:Object.cs.12850");
      case -8:
        return Game1.content.LoadString("Strings\\StringsFromCSFiles:Object.cs.12863");
      case -4:
        return Game1.content.LoadString("Strings\\StringsFromCSFiles:Object.cs.12852");
      default:
        return "";
    }
  }

  /// <summary>Get the display color for an object category.</summary>
  /// <param name="category">The object category.</param>
  public static Color GetCategoryColor(int category)
  {
    switch (category)
    {
      case -103:
        return new Color(122, 93, 39);
      case -102:
        return new Color(85, 47, 27);
      case -81:
        return new Color(10, 130, 50);
      case -80:
        return new Color(219, 54, 211);
      case -79:
        return Color.DeepPink;
      case -75:
        return Color.Green;
      case -74:
        return Color.Brown;
      case -28:
        return new Color(50, 10, 70);
      case -27:
      case -26:
        return new Color(0, 155, 111);
      case -24:
        return new Color(150, 80 /*0x50*/, 190);
      case -22:
        return Color.DarkCyan;
      case -21:
        return Color.DarkRed;
      case -20:
        return Color.DimGray;
      case -19:
        return Color.SlateGray;
      case -18:
      case -14:
      case -6:
      case -5:
        return new Color((int) byte.MaxValue, 0, 100);
      case -16:
      case -15:
        return new Color(64 /*0x40*/, 102, 114);
      case -12:
      case -2:
        return new Color(110, 0, 90);
      case -8:
        return new Color(148, 61, 40);
      case -7:
        return new Color(220, 60, 0);
      case -4:
        return Color.DarkBlue;
      default:
        return Color.Black;
    }
  }

  public virtual bool isActionable(Farmer who)
  {
    return !this.isTemporarilyInvisible && this.checkForAction(who, true);
  }

  public int getHealth() => this.health;

  public void setHealth(int health) => this.health = health;

  protected virtual void grabItemFromAutoGrabber(Item item, Farmer who)
  {
    if (!(this.heldObject.Value is Chest chest))
      return;
    if (who.couldInventoryAcceptThisItem(item))
    {
      chest.Items.Remove(item);
      chest.clearNulls();
      Game1.activeClickableMenu = (IClickableMenu) new ItemGrabMenu((IList<Item>) chest.Items, false, true, new InventoryMenu.highlightThisItem(InventoryMenu.highlightAllItems), new ItemGrabMenu.behaviorOnItemSelect(chest.grabItemFromInventory), (string) null, new ItemGrabMenu.behaviorOnItemSelect(this.grabItemFromAutoGrabber), canBeExitedWithKey: true, showOrganizeButton: true, source: 1, sourceItem: (Item) this, context: (object) this);
    }
    if (!chest.isEmpty())
      return;
    this.showNextIndex.Value = false;
  }

  public static bool HighlightFertilizers(Item i) => i.Category == -19;

  public override int healthRecoveredOnConsumption()
  {
    if (this.Edibility < 0)
      return 0;
    switch (this.QualifiedItemId)
    {
      case "(O)874":
        return (int) ((double) this.staminaRecoveredOnConsumption() * 0.68000000715255737);
      case "(O)434":
      case "(O)349":
        return 0;
      case "(O)773":
        return 999;
      default:
        return (int) ((double) this.staminaRecoveredOnConsumption() * 0.44999998807907104);
    }
  }

  public override int staminaRecoveredOnConsumption()
  {
    switch (this.QualifiedItemId)
    {
      case "(O)773":
        return 0;
      case "(O)434":
        return 999;
      default:
        return (int) Math.Ceiling((double) this.Edibility * 2.5) + this.Quality * this.Edibility;
    }
  }

  /// <summary>Perform an action when the user interacts with this object.</summary>
  /// <param name="who">The player interacting with the object.</param>
  /// <param name="justCheckingForActivity">Whether to check if an action would be performed, without actually doing it. Setting this to true may have inconsistent effects depending on the action.</param>
  /// <returns>Returns true if the action was performed, or false if the player should pick up the item instead.</returns>
  public virtual bool checkForAction(Farmer who, bool justCheckingForActivity = false)
  {
    if (this.isTemporarilyInvisible)
      return true;
    if (!justCheckingForActivity && who != null)
    {
      GameLocation location = this.Location;
      Point tilePoint = who.TilePoint;
      if (location.isObjectAtTile(tilePoint.X, tilePoint.Y - 1) && location.isObjectAtTile(tilePoint.X, tilePoint.Y + 1) && location.isObjectAtTile(tilePoint.X + 1, tilePoint.Y) && location.isObjectAtTile(tilePoint.X - 1, tilePoint.Y) && !location.getObjectAtTile(tilePoint.X, tilePoint.Y - 1).isPassable() && !location.getObjectAtTile(tilePoint.X, tilePoint.Y + 1).isPassable() && !location.getObjectAtTile(tilePoint.X - 1, tilePoint.Y).isPassable() && !location.getObjectAtTile(tilePoint.X + 1, tilePoint.Y).isPassable())
        this.performToolAction((Tool) null);
    }
    string qualifiedItemId = this.QualifiedItemId;
    if (qualifiedItemId != null)
    {
      switch (qualifiedItemId.Length)
      {
        case 5:
          switch (qualifiedItemId[4])
          {
            case '0':
              if (qualifiedItemId == "(BC)0")
                break;
              goto label_59;
            case '1':
              if (qualifiedItemId == "(BC)1")
                break;
              goto label_59;
            case '2':
              if (qualifiedItemId == "(BC)2")
                break;
              goto label_59;
            case '3':
              if (qualifiedItemId == "(BC)3")
                break;
              goto label_59;
            case '4':
              if (qualifiedItemId == "(BC)4")
                break;
              goto label_59;
            case '5':
              if (qualifiedItemId == "(BC)5")
                break;
              goto label_59;
            case '6':
              if (qualifiedItemId == "(BC)6")
                break;
              goto label_59;
            case '7':
              if (qualifiedItemId == "(BC)7")
                break;
              goto label_59;
            default:
              goto label_59;
          }
          return this.CheckForActionOnHousePlant(who, justCheckingForActivity);
        case 6:
          switch (qualifiedItemId[5])
          {
            case '1':
              if (qualifiedItemId == "(BC)71")
                return this.CheckForActionOnStaircase(who, justCheckingForActivity);
              break;
            case '3':
              if (qualifiedItemId == "(O)463")
                return this.CheckForActionOnDrumBlock(who, justCheckingForActivity);
              break;
            case '4':
              switch (qualifiedItemId)
              {
                case "(BC)94":
                  return this.CheckForActionOnSingingStone(who, justCheckingForActivity);
                case "(O)464":
                  return this.CheckForActionOnFluteBlock(who, justCheckingForActivity);
              }
              break;
            case '6':
              if (qualifiedItemId == "(BC)56")
                return this.CheckForActionOnSlimeBall(who, justCheckingForActivity);
              break;
            case '9':
              if (qualifiedItemId == "(BC)99")
                return this.CheckForActionOnFeedHopper(who, justCheckingForActivity);
              break;
          }
          break;
        case 7:
          switch (qualifiedItemId[6])
          {
            case '1':
              if (qualifiedItemId == "(BC)141")
                return this.CheckForActionOnPrairieKingArcadeSystem(who, justCheckingForActivity);
              break;
            case '5':
              if (qualifiedItemId == "(BC)165")
                return this.CheckForActionOnAutoGrabber(who, justCheckingForActivity);
              break;
            case '7':
              if (qualifiedItemId == "(BC)247")
                return this.CheckForActionOnSewingMachine(who, justCheckingForActivity);
              break;
            case '8':
              if (qualifiedItemId == "(BC)238")
                return this.CheckForActionOnMiniObelisk(who, justCheckingForActivity);
              break;
            case '9':
              switch (qualifiedItemId)
              {
                case "(BC)159":
                  return this.CheckForActionOnJunimoKartArcadeSystem(who, justCheckingForActivity);
                case "(BC)239":
                  return this.CheckForActionOnFarmComputer(who, justCheckingForActivity);
              }
              break;
          }
          break;
        case 12:
          if (qualifiedItemId == "(O)PotOfGold")
          {
            if (!justCheckingForActivity)
            {
              Game1.playSound("hammer");
              Game1.playSound("moneyDial");
              Game1.createMultipleItemDebris(ItemRegistry.Create("(O)GoldCoin", Math.Min(100, 7 + Game1.year)), this.TileLocation * 64f + new Vector2(32f), 1);
              Game1.createMultipleItemDebris(ItemRegistry.Create("(H)LeprechuanHat"), this.TileLocation * 64f + new Vector2(32f), 1);
              this.Location.removeObject(this.TileLocation, false);
              Utility.addDirtPuffs(this.Location, (int) this.TileLocation.X, (int) this.TileLocation.Y, 1, 1, 3);
              Utility.addStarsAndSpirals(this.Location, (int) this.TileLocation.X, (int) this.TileLocation.Y, 1, 1, 100, 30, Color.White);
            }
            return true;
          }
          break;
        case 13:
          if (qualifiedItemId == "(BC)MiniForge")
          {
            if (!justCheckingForActivity)
              Game1.activeClickableMenu = (IClickableMenu) new ForgeMenu();
            return true;
          }
          break;
        case 21:
          if (qualifiedItemId == "(BC)StatueOfBlessings")
            return this.CheckForActionOnBlessedStatue(who, who.currentLocation, justCheckingForActivity);
          break;
        case 24:
          if (qualifiedItemId == "(BC)StatueOfTheDwarfKing")
          {
            if (!justCheckingForActivity)
            {
              if (who.stats.Get(StatKeys.Mastery(3)) < 1U)
              {
                Game1.showGlobalMessage(Game1.content.LoadString("Strings\\1_6_Strings:MasteryRequirement"));
                Game1.playSound("cancel");
              }
              else if (!who.hasBuffWithNameContainingString("dwarfStatue"))
              {
                Game1.activeClickableMenu = (IClickableMenu) new ChooseFromIconsMenu("dwarfStatue");
                (Game1.activeClickableMenu as ChooseFromIconsMenu).sourceObject = this;
              }
              else
              {
                this.shakeTimer = 400;
                Game1.playSound("cancel");
              }
            }
            return true;
          }
          break;
      }
    }
label_59:
    return this.IsSprinkler() && this.CheckForActionOnSprinkler(who, justCheckingForActivity) || this.IsScarecrow() && this.CheckForActionOnScarecrow(who, justCheckingForActivity) || this.IsTextSign() && this.CheckForActionOnTextSign(who, justCheckingForActivity) || this.CheckForActionOnMachine(who, justCheckingForActivity);
  }

  /// <summary>Perform an action when the user interacts with this object, assuming it's a sewing machine.</summary>
  /// <inheritdoc cref="M:StardewValley.Object.checkForAction(StardewValley.Farmer,System.Boolean)" />
  protected virtual bool CheckForActionOnSewingMachine(Farmer who, bool justCheckingForActivity = false)
  {
    if (justCheckingForActivity)
      return true;
    Game1.activeClickableMenu = (IClickableMenu) new TailoringMenu();
    return true;
  }

  /// <summary>Perform an action when the user interacts with this object, assuming it's an auto-grabber.</summary>
  /// <inheritdoc cref="M:StardewValley.Object.checkForAction(StardewValley.Farmer,System.Boolean)" />
  protected virtual bool CheckForActionOnAutoGrabber(Farmer who, bool justCheckingForActivity = false)
  {
    if (justCheckingForActivity)
      return true;
    if (!(this.heldObject.Value is Chest chest) || chest.isEmpty())
      return false;
    Game1.activeClickableMenu = (IClickableMenu) new ItemGrabMenu((IList<Item>) chest.Items, false, true, new InventoryMenu.highlightThisItem(InventoryMenu.highlightAllItems), new ItemGrabMenu.behaviorOnItemSelect(chest.grabItemFromInventory), (string) null, new ItemGrabMenu.behaviorOnItemSelect(this.grabItemFromAutoGrabber), canBeExitedWithKey: true, showOrganizeButton: true, source: 1, context: (object) this);
    return true;
  }

  /// <summary>Perform an action when the user interacts with this object, assuming it's a farm computer.</summary>
  /// <inheritdoc cref="M:StardewValley.Object.checkForAction(StardewValley.Farmer,System.Boolean)" />
  protected virtual bool CheckForActionOnFarmComputer(Farmer who, bool justCheckingForActivity = false)
  {
    if (justCheckingForActivity)
      return true;
    this.shakeTimer = 500;
    this.Location.localSound("DwarvishSentry");
    who.freezePause = 500;
    DelayedAction.functionAfterDelay((Action) (() => this.ShowFarmComputerReport(who)), 500);
    return true;
  }

  /// <summary>Show a farm computer analysis for a player's current location.</summary>
  /// <param name="who">The player viewing the farm report.</param>
  protected virtual void ShowFarmComputerReport(Farmer who)
  {
    GameLocation rootLocation = (this.Location ?? who.currentLocation).GetRootLocation();
    Farm farm = rootLocation as Farm;
    int num = rootLocation.IsBuildableLocation() ? 1 : (rootLocation.buildings.Any<Building>() ? 1 : 0);
    string displayName = rootLocation.GetDisplayName();
    int totalCrops = rootLocation.getTotalCrops();
    int totalOpenHoeDirt = rootLocation.getTotalOpenHoeDirt();
    int cropsReadyForHarvest = rootLocation.getTotalCropsReadyForHarvest();
    int totalUnwateredCrops = rootLocation.getTotalUnwateredCrops();
    int? sub1 = rootLocation.HasMinBuildings("Greenhouse", 1) ? rootLocation.getTotalGreenhouseCropsReadyForHarvest() : new int?();
    int totalForageItems = rootLocation.getTotalForageItems();
    int machinesReadyForHarvest = rootLocation.getNumberOfMachinesReadyForHarvest();
    bool? nullable = farm?.doesFarmCaveNeedHarvesting();
    StringBuilder stringBuilder = new StringBuilder();
    if (rootLocation is Farm)
      stringBuilder.Append(Game1.content.LoadString("Strings\\StringsFromCSFiles:FarmComputer_Intro_Farm", (object) Game1.player.farmName.Value));
    else if (!string.IsNullOrWhiteSpace(displayName))
      stringBuilder.Append(Game1.content.LoadString("Strings\\StringsFromCSFiles:FarmComputer_Intro_NamedLocation", (object) displayName));
    else
      stringBuilder.Append(Game1.content.LoadString("Strings\\StringsFromCSFiles:FarmComputer_Intro_Generic"));
    stringBuilder.Append("^--------------^");
    if (num != 0)
      stringBuilder.Append(Game1.content.LoadString("Strings\\StringsFromCSFiles:FarmComputer_PiecesHay", (object) rootLocation.piecesOfHay, (object) rootLocation.GetHayCapacity())).Append(" ^");
    stringBuilder.Append(Game1.content.LoadString("Strings\\StringsFromCSFiles:FarmComputer_TotalCrops", (object) totalCrops)).Append("  ^").Append(Game1.content.LoadString("Strings\\StringsFromCSFiles:FarmComputer_CropsReadyForHarvest", (object) cropsReadyForHarvest)).Append("  ^").Append(Game1.content.LoadString("Strings\\StringsFromCSFiles:FarmComputer_CropsUnwatered", (object) totalUnwateredCrops)).Append("  ^");
    if (sub1.HasValue)
      stringBuilder.Append(Game1.content.LoadString("Strings\\StringsFromCSFiles:FarmComputer_CropsReadyForHarvest_Greenhouse", (object) sub1)).Append("  ^");
    stringBuilder.Append(Game1.content.LoadString("Strings\\StringsFromCSFiles:FarmComputer_TotalOpenHoeDirt", (object) totalOpenHoeDirt)).Append("  ^");
    if (farm == null || farm.SpawnsForage())
      stringBuilder.Append(Game1.content.LoadString("Strings\\StringsFromCSFiles:FarmComputer_TotalForage", (object) totalForageItems)).Append("  ^");
    stringBuilder.Append(Game1.content.LoadString("Strings\\StringsFromCSFiles:FarmComputer_MachinesReady", (object) machinesReadyForHarvest)).Append("  ^");
    if (nullable.HasValue)
      stringBuilder.Append(Game1.content.LoadString("Strings\\StringsFromCSFiles:FarmComputer_FarmCave", nullable.Value ? (object) Game1.content.LoadString("Strings\\Lexicon:QuestionDialogue_Yes") : (object) Game1.content.LoadString("Strings\\Lexicon:QuestionDialogue_No")));
    Game1.multipleDialogues(new string[1]
    {
      stringBuilder.ToString()
    });
  }

  /// <summary>Perform an action when the user interacts with this object, assuming it's a mini-obelisk.</summary>
  /// <inheritdoc cref="M:StardewValley.Object.checkForAction(StardewValley.Farmer,System.Boolean)" />
  protected virtual bool CheckForActionOnMiniObelisk(Farmer who, bool justCheckingForActivity = false)
  {
    if (justCheckingForActivity)
      return true;
    GameLocation location = this.Location;
    Vector2 vector2_1 = Vector2.Zero;
    Vector2 vector2_2 = Vector2.Zero;
    foreach (KeyValuePair<Vector2, Object> pair in location.objects.Pairs)
    {
      if (pair.Value.bigCraftable.Value && pair.Value.QualifiedItemId == "(BC)238")
      {
        if (vector2_1 == Vector2.Zero)
          vector2_1 = pair.Key;
        else if (vector2_2 == Vector2.Zero)
        {
          vector2_2 = pair.Key;
          break;
        }
      }
    }
    if (vector2_2 == Vector2.Zero)
    {
      Game1.showRedMessage(Game1.content.LoadString("Strings\\StringsFromCSFiles:MiniObelisk_NeedsPair"));
      return false;
    }
    Vector2 vector2_3 = (double) Vector2.Distance(who.Tile, vector2_1) > (double) Vector2.Distance(who.Tile, vector2_2) ? vector2_1 : vector2_2;
    Vector2[] vector2Array = new Vector2[4]
    {
      new Vector2(vector2_3.X, vector2_3.Y + 1f),
      new Vector2(vector2_3.X - 1f, vector2_3.Y),
      new Vector2(vector2_3.X + 1f, vector2_3.Y),
      new Vector2(vector2_3.X, vector2_3.Y - 1f)
    };
    foreach (Vector2 vector2_4 in vector2Array)
    {
      Vector2 v = vector2_4;
      if (!location.IsTileBlockedBy(v, ignorePassables: CollisionMask.All))
      {
        for (int index = 0; index < 12; ++index)
          location.temporarySprites.Add(new TemporaryAnimatedSprite(354, (float) Game1.random.Next(25, 75), 6, 1, new Vector2((float) Game1.random.Next((int) who.Position.X - 256 /*0x0100*/, (int) who.Position.X + 192 /*0xC0*/), (float) Game1.random.Next((int) who.Position.Y - 256 /*0x0100*/, (int) who.Position.Y + 192 /*0xC0*/)), false, Game1.random.NextBool()));
        location.playSound("wand");
        Game1.displayFarmer = false;
        Game1.player.freezePause = 50;
        Game1.flashAlpha = 1f;
        DelayedAction.fadeAfterDelay((Game1.afterFadeFunction) (() =>
        {
          who.setTileLocation(v);
          Game1.displayFarmer = true;
          Game1.globalFadeToClear();
        }), 50);
        Microsoft.Xna.Framework.Rectangle boundingBox = who.GetBoundingBox();
        new Microsoft.Xna.Framework.Rectangle(boundingBox.X, boundingBox.Y, 64 /*0x40*/, 64 /*0x40*/).Inflate(192 /*0xC0*/, 192 /*0xC0*/);
        int num = 0;
        Point tilePoint = who.TilePoint;
        for (int x = tilePoint.X + 8; x >= tilePoint.X - 8; --x)
        {
          location.temporarySprites.Add(new TemporaryAnimatedSprite(6, new Vector2((float) x, (float) tilePoint.Y) * 64f, Color.White, animationInterval: 50f)
          {
            layerDepth = 1f,
            delayBeforeAnimationStart = num * 25,
            motion = new Vector2(-0.25f, 0.0f)
          });
          ++num;
        }
        return true;
      }
    }
    Game1.showRedMessage(Game1.content.LoadString("Strings\\StringsFromCSFiles:MiniObelisk_NeedsSpace"));
    return false;
  }

  /// <summary>Perform an action when the user interacts with this object, assuming it's a Prairie King arcade system.</summary>
  /// <inheritdoc cref="M:StardewValley.Object.checkForAction(StardewValley.Farmer,System.Boolean)" />
  protected virtual bool CheckForActionOnPrairieKingArcadeSystem(
    Farmer who,
    bool justCheckingForActivity = false)
  {
    if (justCheckingForActivity)
      return true;
    this.Location.showPrairieKingMenu();
    return true;
  }

  /// <summary>Perform an action when the user interacts with this object, assuming it's a Junimo Kart arcade system.</summary>
  /// <inheritdoc cref="M:StardewValley.Object.checkForAction(StardewValley.Farmer,System.Boolean)" />
  protected virtual bool CheckForActionOnJunimoKartArcadeSystem(
    Farmer who,
    bool justCheckingForActivity = false)
  {
    if (justCheckingForActivity)
      return true;
    Response[] answerChoices = new Response[3]
    {
      new Response("Progress", Game1.content.LoadString("Strings\\StringsFromCSFiles:Object.cs.12873")),
      new Response("Endless", Game1.content.LoadString("Strings\\StringsFromCSFiles:Object.cs.12875")),
      new Response("Exit", Game1.content.LoadString("Strings\\StringsFromCSFiles:TitleMenu.cs.11738"))
    };
    this.Location.createQuestionDialogue(Game1.content.LoadString("Strings\\Locations:Saloon_Arcade_Minecart_Menu"), answerChoices, "MinecartGame");
    return true;
  }

  /// <summary>Perform an action when the user interacts with this object, assuming it's a staircase.</summary>
  /// <inheritdoc cref="M:StardewValley.Object.checkForAction(StardewValley.Farmer,System.Boolean)" />
  protected virtual bool CheckForActionOnStaircase(Farmer who, bool justCheckingForActivity = false)
  {
    if (this.Location is MineShaft location && location.shouldCreateLadderOnThisLevel())
    {
      if (justCheckingForActivity)
        return true;
      Game1.enterMine(Game1.CurrentMineLevel + 1);
      Game1.playSound("stairsdown");
    }
    else if (this.Location.Name.Equals("ManorHouse"))
    {
      if (justCheckingForActivity)
        return true;
      Game1.warpFarmer("LewisBasement", 4, 4, 2);
      Game1.playSound("stairsdown");
    }
    return false;
  }

  /// <summary>Perform an action when the user interacts with this object, assuming it's a slime ball.</summary>
  /// <inheritdoc cref="M:StardewValley.Object.checkForAction(StardewValley.Farmer,System.Boolean)" />
  protected virtual bool CheckForActionOnSlimeBall(Farmer who, bool justCheckingForActivity = false)
  {
    if (justCheckingForActivity)
      return true;
    GameLocation location = this.Location;
    location.objects.Remove(this.tileLocation.Value);
    DelayedAction.playSoundAfterDelay("slimedead", 40);
    DelayedAction.playSoundAfterDelay("slimeHit", 100);
    location.playSound("slimeHit");
    Random random = Utility.CreateRandom((double) Game1.stats.DaysPlayed, (double) Game1.uniqueIDForThisGame, (double) this.tileLocation.X * 77.0, (double) this.tileLocation.Y * 777.0, 2.0);
    Game1.createMultipleObjectDebris("(O)766", (int) this.tileLocation.X, (int) this.tileLocation.Y, random.Next(10, 21), (float) (1.0 + (who.FacingDirection == 2 ? 0.0 : Game1.random.NextDouble())));
    Utility.makeTemporarySpriteJuicier(new TemporaryAnimatedSprite(44, this.tileLocation.Value * 64f, Color.Lime, 10)
    {
      interval = 70f,
      holdLastFrame = true,
      alphaFade = 0.01f
    }, location);
    Game1.multiplayer.broadcastSprites(location, new TemporaryAnimatedSprite(44, this.tileLocation.Value * 64f + new Vector2(-16f, 0.0f), Color.Lime, 10)
    {
      interval = 70f,
      delayBeforeAnimationStart = 0,
      holdLastFrame = true,
      alphaFade = 0.01f
    });
    Game1.multiplayer.broadcastSprites(location, new TemporaryAnimatedSprite(44, this.tileLocation.Value * 64f + new Vector2(0.0f, 16f), Color.Lime, 10)
    {
      interval = 70f,
      delayBeforeAnimationStart = 100,
      holdLastFrame = true,
      alphaFade = 0.01f
    });
    Game1.multiplayer.broadcastSprites(location, new TemporaryAnimatedSprite(44, this.tileLocation.Value * 64f + new Vector2(16f, 0.0f), Color.Lime, 10)
    {
      interval = 70f,
      delayBeforeAnimationStart = 200,
      holdLastFrame = true,
      alphaFade = 0.01f
    });
    while (random.NextDouble() < 0.33)
      Game1.createObjectDebris("(O)557", (int) this.tileLocation.X, (int) this.tileLocation.Y, who.UniqueMultiplayerID);
    return true;
  }

  protected virtual bool CheckForActionOnBlessedStatue(
    Farmer who,
    GameLocation location,
    bool justCheckingForActivitiy = false)
  {
    if (who.stats.Get(StatKeys.Mastery(0)) < 1U && !justCheckingForActivitiy)
    {
      Game1.showGlobalMessage(Game1.content.LoadString("Strings\\1_6_Strings:MasteryRequirement"));
      Game1.playSound("cancel");
      return true;
    }
    if (who.hasBuffWithNameContainingString("statue_of_blessings_") || who.hasBeenBlessedByStatueToday)
      return false;
    if (justCheckingForActivitiy)
      return true;
    who.hasBeenBlessedByStatueToday = true;
    Random daySaveRandom = Utility.CreateDaySaveRandom((double) (Game1.stats.DaysPlayed * 777U));
    for (int index = 0; index < 8; ++index)
      daySaveRandom.Next();
    who.applyBuff("statue_of_blessings_" + daySaveRandom.Next(Game1.isRaining || Utility.isFestivalDay() ? 6 : 7).ToString());
    Game1.playSound("statue_of_blessings");
    this.showNextIndex.Value = true;
    if (location.critters == null)
      location.critters = new List<Critter>();
    location.critters.Add((Critter) new Butterfly(location, this.TileLocation + new Vector2(1f, 0.0f), baseFrameOverride: 163));
    location.critters.Add((Critter) new Butterfly(location, this.TileLocation + new Vector2(0.33f, 0.25f), baseFrameOverride: 163));
    location.critters.Add((Critter) new Butterfly(location, this.TileLocation + new Vector2(1.58f, 0.25f), baseFrameOverride: 163));
    location.temporarySprites.Add(new TemporaryAnimatedSprite("LooseSprites\\Cursors_1_6", new Microsoft.Xna.Framework.Rectangle(221, 225, 15, 31 /*0x1F*/), 9000f, 1, 1, this.TileLocation * 64f + new Vector2(1f, -16f) * 4f, false, false, Math.Max(0.0f, (float) ((((double) this.TileLocation.Y + 1.0) * 64.0 - 20.0) / 10000.0)) + this.TileLocation.X * 1E-05f, 0.02f, Color.White, 4f, 0.0f, 0.0f, 0.0f));
    for (int index = 0; index < 6; ++index)
      Utility.makeTemporarySpriteJuicier(new TemporaryAnimatedSprite("LooseSprites\\Cursors_1_6", new Microsoft.Xna.Framework.Rectangle(144 /*0x90*/, 249, 7, 7), (float) Game1.random.Next(100, 200), 6, 1, this.TileLocation * 64f + new Vector2((float) (32 /*0x20*/ + Game1.random.Next(-64, 64 /*0x40*/)), (float) Game1.random.Next(-64, 64 /*0x40*/)), false, false, Math.Max(0.0f, (float) ((((double) this.TileLocation.Y + 1.0) * 64.0 - 24.0) / 10000.0)) + this.TileLocation.X * 1E-05f, 0.0f, Game1.random.NextDouble() < 0.5 ? new Color((int) byte.MaxValue, 180, 210) : Color.White, 4f, 0.0f, 0.0f, 0.0f), location);
    return true;
  }

  /// <summary>Perform an action when the user interacts with this object, assuming it's a house plant.</summary>
  /// <inheritdoc cref="M:StardewValley.Object.checkForAction(StardewValley.Farmer,System.Boolean)" />
  protected virtual bool CheckForActionOnHousePlant(Farmer who, bool justCheckingForActivity = false)
  {
    if (justCheckingForActivity)
      return true;
    ++this.ParentSheetIndex;
    int num1 = -1;
    int num2 = -1;
    if (this.name == "House Plant")
    {
      num1 = 8;
      num2 = 0;
    }
    if (this.ParentSheetIndex != num2 + num1)
      return true;
    this.ParentSheetIndex -= num1;
    return false;
  }

  /// <summary>Perform an action when the user interacts with this object, assuming it's a flute block.</summary>
  /// <inheritdoc cref="M:StardewValley.Object.checkForAction(StardewValley.Farmer,System.Boolean)" />
  protected virtual bool CheckForActionOnFluteBlock(Farmer who, bool justCheckingForActivity = false)
  {
    if (justCheckingForActivity)
      return true;
    int result;
    int.TryParse(this.preservedParentSheetIndex.Value, out result);
    switch (result)
    {
      case 2300:
        result = 2400;
        break;
      case 2400:
        result = 0;
        break;
      default:
        result = (result + 100) % 2400;
        break;
    }
    this.preservedParentSheetIndex.Value = result.ToString();
    this.shakeTimer = 200;
    string cueName = "flute";
    if (who.ActiveObject != null)
      cueName = this.getFluteBlockSoundFromHeldObject(who.ActiveObject);
    this.internalSound?.Stop(AudioStopOptions.Immediate);
    Game1.playSound(cueName, result, out this.internalSound);
    this.scale.Y = 1.3f;
    this.shakeTimer = 200;
    return true;
  }

  /// <summary>Perform an action when the user interacts with this object, assuming it's a drum block.</summary>
  /// <inheritdoc cref="M:StardewValley.Object.checkForAction(StardewValley.Farmer,System.Boolean)" />
  protected virtual bool CheckForActionOnDrumBlock(Farmer who, bool justCheckingForActivity = false)
  {
    if (justCheckingForActivity)
      return true;
    int result;
    int.TryParse(this.preservedParentSheetIndex.Value, out result);
    result = (result + 1) % 7;
    this.preservedParentSheetIndex.Value = result.ToString();
    this.shakeTimer = 200;
    this.internalSound?.Stop(AudioStopOptions.Immediate);
    Game1.playSound("drumkit" + result.ToString(), out this.internalSound);
    this.scale.Y = 1.3f;
    this.shakeTimer = 200;
    return true;
  }

  /// <summary>Perform an action when the user interacts with this object, assuming it's a sprinkler.</summary>
  /// <inheritdoc cref="M:StardewValley.Object.checkForAction(StardewValley.Farmer,System.Boolean)" />
  protected bool CheckForActionOnSprinkler(Farmer who, bool justCheckingForActivity = false)
  {
    if (this.heldObject.Value == null || !(this.heldObject.Value.QualifiedItemId == "(O)913"))
      return false;
    if (justCheckingForActivity)
      return true;
    if (!Game1.didPlayerJustRightClick(true) || !(this.heldObject.Value.heldObject.Value is Chest chest))
      return false;
    chest.GetMutex().RequestLock(new Action(chest.ShowMenu));
    return true;
  }

  /// <summary>Perform an action when the user interacts with this object, assuming it's a scarecrow.</summary>
  /// <inheritdoc cref="M:StardewValley.Object.checkForAction(StardewValley.Farmer,System.Boolean)" />
  protected bool CheckForActionOnScarecrow(Farmer who, bool justCheckingForActivity = false)
  {
    if (justCheckingForActivity)
      return true;
    if (this.QualifiedItemId == "(BC)126" && who.CurrentItem is Hat currentItem)
    {
      this.shakeTimer = 100;
      if (this.quality.Value != 0)
      {
        Game1.createItemDebris(ItemRegistry.Create("(H)" + (this.quality.Value - 1).ToString()), this.tileLocation.Value * 64f, (who.FacingDirection + 2) % 4);
        this.quality.Value = 0;
      }
      if (this.preservedParentSheetIndex.Value != null)
        Game1.createItemDebris((Item) new Hat(this.preservedParentSheetIndex.Value), this.tileLocation.Value * 64f, (who.FacingDirection + 2) % 4);
      this.preservedParentSheetIndex.Value = currentItem.ItemId;
      who.Items[who.CurrentToolIndex] = (Item) null;
      this.Location.playSound("dirtyHit");
      return true;
    }
    if (!Game1.didPlayerJustRightClick(true))
      return false;
    this.shakeTimer = 100;
    if (this.SpecialVariable == 0)
      Game1.drawObjectDialogue(Game1.content.LoadString("Strings\\StringsFromCSFiles:Object.cs.12926"));
    else
      Game1.drawObjectDialogue(this.SpecialVariable == 1 ? Game1.content.LoadString("Strings\\StringsFromCSFiles:Object.cs.12927") : Game1.content.LoadString("Strings\\StringsFromCSFiles:Object.cs.12929", (object) this.SpecialVariable));
    return true;
  }

  /// <summary>Perform an action when the user interacts with this object, assuming it's a singing stone.</summary>
  /// <inheritdoc cref="M:StardewValley.Object.checkForAction(StardewValley.Farmer,System.Boolean)" />
  protected bool CheckForActionOnSingingStone(Farmer who, bool justCheckingForActivity = false)
  {
    if (justCheckingForActivity)
      return true;
    int num = Game1.random.Next(2400);
    Game1.playSound("crystal", new int?(num - num % 100));
    this.shakeTimer = 100;
    return true;
  }

  /// <summary>Perform an action when the user interacts with this object, assuming it's a text sign.</summary>
  /// <inheritdoc cref="M:StardewValley.Object.checkForAction(StardewValley.Farmer,System.Boolean)" />
  protected virtual bool CheckForActionOnTextSign(Farmer who, bool justCheckingForActivity = false)
  {
    if (justCheckingForActivity)
      return true;
    if (Game1.activeClickableMenu != null)
      return false;
    TitleTextInputMenu signMenu = new TitleTextInputMenu(Game1.content.LoadString("Strings\\UI:TextSignEntry"), (NamingMenu.doneNamingBehavior) null, this.SignText, (string) null);
    signMenu.pasteButton.visible = false;
    signMenu.doneNaming = (NamingMenu.doneNamingBehavior) (text =>
    {
      this.signText.Value = text.Trim();
      signMenu.exitThisMenu();
      this.showNextIndex.Value = string.IsNullOrEmpty(this.SignText);
    });
    signMenu.textBox.textLimit = 60;
    Game1.activeClickableMenu = (IClickableMenu) signMenu;
    return true;
  }

  /// <summary>Perform an action when the user interacts with this object, assuming it's a feed hopper.</summary>
  /// <inheritdoc cref="M:StardewValley.Object.checkForAction(StardewValley.Farmer,System.Boolean)" />
  protected bool CheckForActionOnFeedHopper(Farmer who, bool justCheckingForActivity = false)
  {
    if (justCheckingForActivity)
      return true;
    if (who.ActiveObject != null)
      return false;
    if (who.freeSpotsInInventory() > 0)
    {
      GameLocation location = this.Location;
      GameLocation rootLocation = location.GetRootLocation();
      int val2 = rootLocation.piecesOfHay.Value;
      if (val2 > 0)
      {
        bool flag = false;
        if (location is AnimalHouse animalHouse)
        {
          int val1 = Math.Max(1, Math.Min(animalHouse.animalsThatLiveHere.Count, val2));
          int num1 = animalHouse.numberOfObjectsWithName("Hay");
          int num2 = Math.Min(val1, animalHouse.animalLimit.Value - num1);
          if (num2 != 0 && Game1.player.couldInventoryAcceptThisItem("(O)178", num2))
          {
            rootLocation.piecesOfHay.Value -= Math.Max(1, num2);
            who.addItemToInventoryBool(ItemRegistry.Create("(O)178", num2));
            Game1.playSound("shwip");
            flag = true;
          }
        }
        else if (Game1.player.couldInventoryAcceptThisItem("(O)178", 1))
        {
          --rootLocation.piecesOfHay.Value;
          who.addItemToInventoryBool(ItemRegistry.Create("(O)178"));
          Game1.playSound("shwip");
        }
        if (rootLocation.piecesOfHay.Value <= 0)
          this.showNextIndex.Value = false;
        int num = flag ? 1 : 0;
        return true;
      }
      Game1.drawObjectDialogue(Game1.content.LoadString("Strings\\StringsFromCSFiles:Object.cs.12942"));
    }
    else
      Game1.showRedMessage(Game1.content.LoadString("Strings\\StringsFromCSFiles:Crop.cs.588"));
    return true;
  }

  /// <summary>Perform an action when the user interacts with this object, assuming it's a machine.</summary>
  /// <inheritdoc cref="M:StardewValley.Object.checkForAction(StardewValley.Farmer,System.Boolean)" />
  protected bool CheckForActionOnMachine(Farmer who, bool justCheckingForActivity = false)
  {
    GameLocation location = this.Location;
    if (this.readyForHarvest.Value)
    {
      if (justCheckingForActivity)
        return true;
      if (who.isMoving())
        Game1.haltAfterCheck = false;
      MachineData machineData = this.GetMachineData();
      Object previousOutput = this.heldObject.Value;
      if (this.lastOutputRuleId.Value != null)
      {
        List<MachineOutputRule> outputRules = machineData.OutputRules;
        MachineOutputRule outputRule = outputRules != null ? outputRules.FirstOrDefault<MachineOutputRule>((Func<MachineOutputRule, bool>) (p => p.Id == this.lastOutputRuleId.Value)) : (MachineOutputRule) null;
        if (outputRule != null && outputRule.RecalculateOnCollect)
        {
          this.heldObject.Value = (Object) null;
          this.OutputMachine(machineData, outputRule, this.lastInputItem.Value, who, location, false, true);
          if (this.heldObject.Value != null)
            previousOutput = this.heldObject.Value;
          else
            this.heldObject.Value = previousOutput;
        }
      }
      bool flag = false;
      if (who.IsLocalPlayer)
      {
        this.heldObject.Value = (Object) null;
        if (!who.addItemToInventoryBool((Item) previousOutput))
        {
          this.heldObject.Value = previousOutput;
          Game1.showRedMessage(Game1.content.LoadString("Strings\\StringsFromCSFiles:Crop.cs.588"));
          return false;
        }
        Game1.playSound("coin");
        flag = true;
        MachineDataUtility.UpdateStats(machineData?.StatsToIncrementWhenHarvested, (Item) previousOutput, previousOutput.Stack);
      }
      this.heldObject.Value = (Object) null;
      this.readyForHarvest.Value = false;
      this.showNextIndex.Value = false;
      this.ResetParentSheetIndex();
      MachineOutputRule rule;
      if (MachineDataUtility.TryGetMachineOutputRule(this, machineData, MachineOutputTrigger.OutputCollected, previousOutput.getOne(), who, location, out rule, out MachineOutputTriggerRule _, out MachineOutputRule _, out MachineOutputTriggerRule _))
        this.OutputMachine(machineData, rule, this.lastInputItem.Value, who, location, false);
      TerrainFeature terrainFeature;
      if (this.IsTapper() && location.terrainFeatures.TryGetValue(this.tileLocation.Value, out terrainFeature) && terrainFeature is Tree tree)
        tree.UpdateTapperProduct(this, previousOutput);
      if (machineData != null && machineData.ExperienceGainOnHarvest != null)
      {
        string[] array = machineData.ExperienceGainOnHarvest.Split(' ');
        for (int index = 0; index < array.Length; index += 2)
        {
          int skillNumberFromName = Farmer.getSkillNumberFromName(array[index]);
          int howMuch;
          if (skillNumberFromName != -1 && ArgUtility.TryGetInt(array, index + 1, out howMuch, out string _, "int amount"))
            who.gainExperience(skillNumberFromName, howMuch);
        }
      }
      if (flag)
        this.AttemptAutoLoad(who);
      return true;
    }
    MachineData machineData1 = this.GetMachineData();
    if (machineData1 != null && machineData1.InteractMethod != null)
    {
      MachineInteractDelegate createdDelegate;
      string error;
      if (StaticDelegateBuilder.TryCreateDelegate<MachineInteractDelegate>(machineData1.InteractMethod, out createdDelegate, out error))
        return justCheckingForActivity || createdDelegate(this, location, who);
      Game1.log.Warn($"Machine {this.ItemId} has invalid interaction method '{machineData1.InteractMethod}': {error}");
    }
    return false;
  }

  /// <summary>Play a sound for the current player only if they're near this object.</summary>
  /// <param name="audioName">The sound ID to play.</param>
  /// <param name="pitch">The pitch modifier to apply, or <c>null</c> to keep it as-is.</param>
  /// <param name="context">The source which triggered the sound.</param>
  public void playNearbySoundLocal(string audioName, int? pitch = null, SoundContext context = SoundContext.Default)
  {
    Game1.sounds.PlayLocal(audioName, this.Location, new Vector2?(this.tileLocation.Value), pitch, context, out ICue _);
  }

  /// <summary>Play a sound for each nearby online player.</summary>
  /// <param name="audioName">The sound ID to play.</param>
  /// <param name="pitch">The pitch modifier to apply, or <c>null</c> to keep it as-is.</param>
  /// <param name="context">The source which triggered the sound.</param>
  public void playNearbySoundAll(string audioName, int? pitch = null, SoundContext context = SoundContext.Default)
  {
    Game1.sounds.PlayAll(audioName, this.Location, new Vector2?(this.tileLocation.Value), pitch, context);
  }

  public virtual bool IsScarecrow()
  {
    return this.HasContextTag("crow_scare") || this.Name.Contains("arecrow");
  }

  public virtual int GetRadiusForScarecrow()
  {
    foreach (string contextTag in this.GetContextTags())
    {
      int result;
      if (contextTag.StartsWithIgnoreCase("crow_scare_radius_") && int.TryParse(contextTag.Substring("crow_scare_radius_".Length), out result))
        return result;
    }
    return this.Name.StartsWith("Deluxe") ? 17 : 9;
  }

  /// <summary>If there's a chest above this object, try to auto-load the held object from the chest.</summary>
  /// <param name="who">The player interacting with the machine, if applicable.</param>
  /// <returns>If a chest is found, this method will acquire a mutex lock so the auto-load may not happen during the same tick. The returned task will complete once the auto-load happens, and contain true (an item was loaded) or false (no item was loaded).</returns>
  public virtual Task<bool> AttemptAutoLoad(Farmer who)
  {
    GameLocation location = this.Location;
    Object @object;
    if (location != null && location.objects.TryGetValue(new Vector2(this.TileLocation.X, this.TileLocation.Y - 1f), out @object))
    {
      Chest chest = @object as Chest;
      if (chest != null && chest.specialChestType.Value == Chest.SpecialChestTypes.AutoLoader)
      {
        TaskCompletionSource<bool> taskSource = new TaskCompletionSource<bool>();
        chest.GetMutex().RequestLock((Action) (() =>
        {
          try
          {
            chest.GetMutex().ReleaseLock();
            taskSource.SetResult(this.AttemptAutoLoad((IInventory) chest.Items, who));
          }
          catch (Exception ex)
          {
            taskSource.SetException(ex);
          }
        }));
        return taskSource.Task;
      }
    }
    return Task.FromResult<bool>(false);
  }

  /// <summary>Try to auto-load the held object from the given inventory.</summary>
  /// <param name="inventory">The inventory from which to take items.</param>
  /// <param name="who">The player interacting with the machine, if applicable.</param>
  public virtual bool AttemptAutoLoad(IInventory inventory, Farmer who)
  {
    if (this.heldObject.Value != null)
      return false;
    Object.autoLoadFrom = inventory;
    foreach (Item dropInItem in (IEnumerable<Item>) inventory)
    {
      if (this.performObjectDropInAction(dropInItem, false, who))
      {
        Object.autoLoadFrom = (IInventory) null;
        return true;
      }
    }
    Object.autoLoadFrom = (IInventory) null;
    return false;
  }

  private string getFluteBlockSoundFromHeldObject(Object o)
  {
    string qualifiedItemId = o.QualifiedItemId;
    if (qualifiedItemId != null)
    {
      switch (qualifiedItemId.Length)
      {
        case 5:
          switch (qualifiedItemId[3])
          {
            case '6':
              if (qualifiedItemId == "(O)66")
                return "miniharp_note";
              goto label_22;
            case '8':
              if (qualifiedItemId == "(O)80")
                break;
              goto label_22;
            default:
              goto label_22;
          }
          break;
        case 6:
          switch (qualifiedItemId[5])
          {
            case '0':
              if (qualifiedItemId == "(O)430")
                return "pig";
              goto label_22;
            case '2':
              switch (qualifiedItemId)
              {
                case "(O)372":
                  break;
                case "(O)382":
                  return "dustMeep";
                default:
                  goto label_22;
              }
              break;
            case '4':
              if (qualifiedItemId == "(O)444")
                return "Duck";
              goto label_22;
            case '6':
              if (qualifiedItemId == "(O)746")
                goto label_20;
              goto label_22;
            case '7':
              switch (qualifiedItemId)
              {
                case "(O)797":
                  break;
                case "(O)577":
                  goto label_18;
                default:
                  goto label_22;
              }
              break;
            case '8':
              if (qualifiedItemId == "(O)578" || qualifiedItemId == "(O)338")
                goto label_18;
              goto label_22;
            case '9':
              if (qualifiedItemId == "(O)769")
                goto label_20;
              goto label_22;
            default:
              goto label_22;
          }
          return "clam_tone";
label_20:
          return "toyPiano";
        case 7:
          if (qualifiedItemId == "(BC)214")
            return "telephone_buttonPush";
          goto label_22;
        default:
          goto label_22;
      }
label_18:
      return "crystal";
    }
label_22:
    return "flute";
  }

  public virtual void farmerAdjacentAction(Farmer who, bool diagonal = false)
  {
    if (this.name == "Error Item" || this.isTemporarilyInvisible)
      return;
    GameLocation location = this.Location;
    switch (this.QualifiedItemId)
    {
      case "(O)464":
        if (this.internalSound != null && ((int) Game1.currentGameTime.TotalGameTime.TotalMilliseconds - this.lastNoteBlockSoundTime < 1000 || this.internalSound.IsPlaying) || Game1.dialogueUp || diagonal)
          break;
        int result1;
        int.TryParse(this.preservedParentSheetIndex.Value, out result1);
        string cueName = "flute";
        if (who.ActiveObject != null)
          cueName = this.getFluteBlockSoundFromHeldObject(who.ActiveObject);
        Game1.playSound(cueName, result1, out this.internalSound);
        this.scale.Y = 1.3f;
        this.shakeTimer = 200;
        this.lastNoteBlockSoundTime = (int) Game1.currentGameTime.TotalGameTime.TotalMilliseconds;
        if (!(location is IslandSouthEast islandSouthEast))
          break;
        islandSouthEast.OnFlutePlayed(result1);
        break;
      case "(O)463":
        if (this.internalSound != null && (Game1.currentGameTime.TotalGameTime.TotalMilliseconds - (double) this.lastNoteBlockSoundTime < 1000.0 || this.internalSound.IsPlaying) || Game1.dialogueUp || diagonal)
          break;
        int result2;
        int.TryParse(this.preservedParentSheetIndex.Value, out result2);
        Game1.playSound("drumkit" + result2.ToString(), out this.internalSound);
        this.scale.Y = 1.3f;
        this.shakeTimer = 200;
        this.lastNoteBlockSoundTime = (int) Game1.currentGameTime.TotalGameTime.TotalMilliseconds;
        break;
      case "(BC)29":
        if (diagonal)
          break;
        ++this.scale.X;
        if ((double) this.scale.X > 30.0)
        {
          this.ParentSheetIndex = this.ParentSheetIndex == 29 ? 30 : 29;
          this.scale.X = 0.0f;
          this.scale.Y += 2f;
        }
        if ((double) this.scale.Y < 20.0 || Game1.random.NextDouble() >= 0.0001 || location.characters.Count >= 4)
          break;
        Vector2 tile = Game1.player.Tile;
        foreach (Vector2 adjacentTilesOffset in Character.AdjacentTilesOffsets)
        {
          Vector2 vector2 = tile + adjacentTilesOffset;
          if (!location.IsTileOccupiedBy(vector2) && location.isTilePassable(new xTile.Dimensions.Location((int) vector2.X, (int) vector2.Y), Game1.viewport) && location.isCharacterAtTile(vector2) == null)
          {
            if (Game1.random.NextDouble() < 0.1)
              location.characters.Add((NPC) new GreenSlime(vector2 * new Vector2(64f, 64f)));
            else if (Game1.random.NextBool())
              location.characters.Add((NPC) new ShadowGuy(vector2 * new Vector2(64f, 64f)));
            else
              location.characters.Add((NPC) new ShadowGirl(vector2 * new Vector2(64f, 64f)));
            location.characters[location.characters.Count - 1].moveTowardPlayerThreshold.Value = 4;
            Game1.multiplayer.broadcastSprites(location, new TemporaryAnimatedSprite(352, 400f, 2, 1, vector2 * new Vector2(64f, 64f), false, false));
            location.playSound("shadowpeep");
            break;
          }
        }
        break;
      default:
        if (this.IsTextSign())
        {
          this.hovering = true;
          break;
        }
        Object @object;
        if (diagonal || !this.Location.objects.TryGetValue(new Vector2(this.TileLocation.X, this.TileLocation.Y - 1f), out @object) || !@object.IsTextSign())
          break;
        @object.hovering = true;
        break;
    }
  }

  public virtual void addWorkingAnimation()
  {
    GameLocation location = this.Location;
    if (location == null || !location.farmers.Any())
      return;
    MachineData machineData = this.GetMachineData();
    if (machineData?.WorkingEffects == null)
      return;
    using (List<MachineEffects>.Enumerator enumerator = machineData.WorkingEffects.GetEnumerator())
    {
      do
        ;
      while (enumerator.MoveNext() && !this.PlayMachineEffect(enumerator.Current));
    }
  }

  public virtual void onReadyForHarvest()
  {
  }

  /// <summary>Update the object when the time of day changes.</summary>
  /// <param name="minutes">The number of minutes that passed. If this item is a machine, this method may be called with zero minutes immediately after the machine begins processing.</param>
  /// <returns>Returns <c>true</c> if the object should be removed, else <c>false</c>.</returns>
  public virtual bool minutesElapsed(int minutes)
  {
    GameLocation location = this.Location;
    if (location == null)
      return false;
    if (this.heldObject.Value != null && this.QualifiedItemId != "(BC)165")
    {
      if (this.IsSprinkler())
        return false;
      MachineData machineData = this.GetMachineData();
      if (Game1.IsMasterGame && (machineData == null || this.ShouldTimePassForMachine()))
        this.minutesUntilReady.Value -= minutes;
      if (this.MinutesUntilReady <= 0 && ((machineData != null ? (!machineData.OnlyCompleteOvernight ? 1 : 0) : 1) != 0 || Game1.newDaySync.hasInstance()))
      {
        if (!this.readyForHarvest.Value && (!Game1.newDaySync.hasInstance() || Game1.newDaySync.hasFinished()))
          location.playSound("dwop");
        this.readyForHarvest.Value = true;
        this.minutesUntilReady.Value = 0;
        this.onReadyForHarvest();
        this.showNextIndex.Value = machineData != null && machineData.ShowNextIndexWhenReady;
        if (this.lightSource != null)
        {
          location.removeLightSource(this.lightSource.Id);
          this.lightSource = (LightSource) null;
        }
      }
      if (machineData != null)
      {
        if (!this.readyForHarvest.Value && machineData.WorkingEffects != null && Game1.random.NextDouble() < (double) machineData.WorkingEffectChance)
          this.addWorkingAnimation();
      }
      else if (!this.readyForHarvest.Value && Game1.random.NextDouble() < 0.33)
        this.addWorkingAnimation();
    }
    else
    {
      switch (this.QualifiedItemId)
      {
        case "(BC)29":
          this.scale.Y = Math.Max(0.0f, this.scale.Y -= (float) (minutes / 2 + 1));
          break;
        case "(BC)96":
          this.MinutesUntilReady -= minutes;
          this.showNextIndex.Value = !this.showNextIndex.Value;
          if (this.MinutesUntilReady <= 0)
          {
            this.performRemoveAction();
            location.objects.Remove(this.tileLocation.Value);
            location.objects.Add(this.tileLocation.Value, ItemRegistry.Create<Object>("(BC)98"));
            Game1.player.team.RequestSetMail(PlayerActionTarget.Host, "Capsule_Broken", MailType.Received, true);
            break;
          }
          break;
        case "(BC)141":
          this.showNextIndex.Value = !this.showNextIndex.Value;
          break;
        case "(BC)83":
          this.showNextIndex.Value = false;
          location.removeLightSource(this.GenerateLightSourceId(this.tileLocation.Value));
          break;
      }
    }
    return false;
  }

  public virtual bool ShouldTimePassForMachine()
  {
    GameLocation location = this.Location;
    MachineData machineData = this.GetMachineData();
    if (location == null || machineData == null)
      return false;
    if (machineData.PreventTimePass != null)
    {
      foreach (int num in machineData.PreventTimePass)
      {
        switch (num)
        {
          case 0:
            if (location.IsOutdoors)
              return false;
            continue;
          case 1:
            if (!location.IsOutdoors)
              return false;
            continue;
          case 2:
            if (location.IsSpringHere())
              return false;
            continue;
          case 3:
            if (location.IsSummerHere())
              return false;
            continue;
          case 4:
            if (location.IsFallHere())
              return false;
            continue;
          case 5:
            if (location.IsWinterHere())
              return false;
            continue;
          case 6:
            if (!location.IsRainingHere())
              return false;
            continue;
          case 7:
            if (location.IsRainingHere())
              return false;
            continue;
          case 8:
            return false;
          default:
            continue;
        }
      }
    }
    return true;
  }

  public override string checkForSpecialItemHoldUpMeessage()
  {
    if (!this.bigCraftable.Value && this.Type == "Arch")
      return Game1.content.LoadString("Strings\\StringsFromCSFiles:Object.cs.12993");
    switch (this.QualifiedItemId)
    {
      case "(O)102":
        return Game1.content.LoadString("Strings\\StringsFromCSFiles:Object.cs.12994");
      case "(O)535":
        return Game1.content.LoadString("Strings\\StringsFromCSFiles:Object.cs.12995");
      case "(BC)160":
        return Game1.content.LoadString("Strings\\StringsFromCSFiles:Object.cs.12996");
      default:
        return base.checkForSpecialItemHoldUpMeessage();
    }
  }

  public virtual bool countsForShippedCollection()
  {
    if (string.IsNullOrWhiteSpace(this.type.Value) || this.Type == "Arch" || this.bigCraftable.Value)
      return false;
    if (this.QualifiedItemId == "(O)433")
      return true;
    int category = this.Category;
    if (category <= -14)
    {
      if (category <= -74)
      {
        if (category != -999 && category != -74)
          goto label_13;
      }
      else
      {
        switch (category - -29)
        {
          case 0:
          case 5:
          case 7:
          case 8:
          case 9:
          case 10:
            break;
          case 1:
          case 2:
          case 3:
          case 4:
          case 6:
            goto label_13;
          default:
            if (category == -14)
              break;
            goto label_13;
        }
      }
    }
    else if (category <= -7)
    {
      if (category != -12 && (uint) (category - -8) > 1U)
        goto label_13;
    }
    else if (category != -2 && category != 0)
      goto label_13;
    return false;
label_13:
    ObjectData objectData;
    return !Game1.objectData.TryGetValue(this.ItemId, out objectData) || !objectData.ExcludeFromShippingCollection;
  }

  public static bool isPotentialBasicShipped(string itemId, int category, string objectType)
  {
    if (itemId == "433")
      return true;
    if (objectType == "Arch" || objectType == "Fish" || objectType == "Minerals" || objectType == "Cooking")
      return false;
    switch (category)
    {
      case -999:
      case -103:
      case -102:
      case -96:
      case -74:
      case -29:
      case -24:
      case -22:
      case -21:
      case -20:
      case -19:
      case -14:
      case -12:
      case -8:
      case -7:
      case -2:
      case 0:
        return false;
      default:
        ObjectData objectData;
        return !Game1.objectData.TryGetValue(itemId, out objectData) || !objectData.ExcludeFromShippingCollection;
    }
  }

  public override IEnumerable<Buff> GetFoodOrDrinkBuffs()
  {
    Object @object = this;
    // ISSUE: reference to a compiler-generated method
    foreach (Buff foodOrDrinkBuff in @object.\u003C\u003En__0())
      yield return foodOrDrinkBuff;
    if (@object.customBuff != null)
    {
      Buff foodOrDrinkBuff = @object.customBuff();
      if (foodOrDrinkBuff != null)
        yield return foodOrDrinkBuff;
    }
    ObjectData objectData;
    if (@object.edibility.Value > -300 && Game1.objectData.TryGetValue(@object.ItemId, out objectData))
    {
      List<ObjectBuffData> buffs = objectData.Buffs;
      // ISSUE: explicit non-virtual call
      if ((buffs != null ? (__nonvirtual (buffs.Count) > 0 ? 1 : 0) : 0) != 0)
      {
        // ISSUE: explicit non-virtual call
        float durationMultiplier = __nonvirtual (@object.Quality) != 0 ? 1.5f : 1f;
        foreach (Buff foodOrDrinkBuff in Object.TryCreateBuffsFromData(objectData, @object.Name, @object.DisplayName, durationMultiplier, new Action<BuffEffects>(((Item) @object).ModifyItemBuffs)))
          yield return foodOrDrinkBuff;
      }
    }
  }

  /// <summary>Create buffs matching data from <c>Data/Objects</c>, if valid.</summary>
  /// <param name="obj">The raw data from <c>Data/Objects</c> to parse.</param>
  /// <param name="name">The buff source name (usually the <see cref="P:StardewValley.Item.Name" />).</param>
  /// <param name="displayName">The translated buff source name (usually the <see cref="P:StardewValley.Item.DisplayName" />).</param>
  /// <param name="durationMultiplier">A multiplier to apply to food or drink buff durations. This only applies to food/drink buffs defined directly in <c>Data/Objects</c>, not to buff IDs which reference <c>Data/Buffs</c>.</param>
  /// <param name="adjustEffects">Adjust the parsed attribute effects before the buff is constructed.</param>
  public static IEnumerable<Buff> TryCreateBuffsFromData(
    ObjectData obj,
    string name,
    string displayName,
    float durationMultiplier = 1f,
    Action<BuffEffects> adjustEffects = null)
  {
    List<ObjectBuffData> buffs = obj.Buffs;
    // ISSUE: explicit non-virtual call
    if ((buffs != null ? (__nonvirtual (buffs.Count) > 0 ? 1 : 0) : 0) != 0)
    {
      foreach (ObjectBuffData buff1 in obj.Buffs)
      {
        if (buff1 != null)
        {
          string id = buff1.BuffId;
          int num = !string.IsNullOrWhiteSpace(id) ? 1 : 0;
          if (num == 0)
            id = obj.IsDrink ? "drink" : "food";
          BuffEffects effects = new BuffEffects(buff1.CustomAttributes);
          Action<BuffEffects> action = adjustEffects;
          if (action != null)
            action(effects);
          Texture2D iconTexture = (Texture2D) null;
          int iconSheetIndex = -1;
          if (buff1.IconTexture != null)
          {
            iconTexture = Game1.content.Load<Texture2D>(buff1.IconTexture);
            iconSheetIndex = buff1.IconSpriteIndex;
          }
          int duration = -1;
          if (buff1.Duration == -2)
            duration = -2;
          else if (buff1.Duration != 0)
            duration = (int) ((double) buff1.Duration * (double) durationMultiplier) * Game1.realMilliSecondsPerGameMinute;
          bool isDebuff = buff1.IsDebuff;
          Color? color = Utility.StringToColor(buff1.GlowColor);
          if (num != 0 || (duration > 0 || duration == -2) && effects.HasAnyValue())
          {
            Buff buff2 = new Buff(id, name, displayName, duration, iconTexture, iconSheetIndex, effects, new bool?(isDebuff));
            buff2.customFields.TryAddMany<string, string>(buff1.CustomFields);
            if (color.HasValue)
              buff2.glow = color.Value;
            yield return buff2;
          }
        }
      }
    }
  }

  /// <summary>Get whether the object scale should pulse currently.</summary>
  public virtual bool ShouldWobble()
  {
    if (this.minutesUntilReady.Value > 0 && !this.readyForHarvest.Value)
    {
      MachineData machineData = this.GetMachineData();
      if (machineData != null)
        return machineData.WobbleWhileWorking && this.heldObject.Value != null;
      if (this.bigCraftable.Value)
      {
        string qualifiedItemId = this.QualifiedItemId;
        return !(qualifiedItemId == "(BC)22") && !(qualifiedItemId == "(BC)23") && !(qualifiedItemId == "(BC)65") && !(qualifiedItemId == "(BC)66") && !(qualifiedItemId == "(BC)165");
      }
    }
    return false;
  }

  public virtual Vector2 getScale()
  {
    if (this.Category == -22)
      return Vector2.Zero;
    if (!this.bigCraftable.Value)
    {
      this.scale.Y = Math.Max(4f, this.scale.Y - 0.04f);
      return this.scale;
    }
    if (!this.ShouldWobble())
      return Vector2.Zero;
    if (this.QualifiedItemId.Equals("(BC)17"))
    {
      this.scale.X = (float) (((double) this.scale.X + 0.039999999105930328) % (2.0 * Math.PI));
      return Vector2.Zero;
    }
    this.scale.X -= 0.1f;
    this.scale.Y += 0.1f;
    if ((double) this.scale.X <= 0.0)
      this.scale.X = 10f;
    if ((double) this.scale.Y >= 10.0)
      this.scale.Y = 0.0f;
    return new Vector2(Math.Abs(this.scale.X - 5f), Math.Abs(this.scale.Y - 5f));
  }

  public virtual void drawWhenHeld(SpriteBatch spriteBatch, Vector2 objectPosition, Farmer f)
  {
    ParsedItemData dataOrErrorItem = ItemRegistry.GetDataOrErrorItem(this.QualifiedItemId);
    float layerDepth = Math.Max(0.0f, (float) (f.StandingPixel.Y + 3) / 10000f);
    Texture2D texture = dataOrErrorItem.GetTexture();
    int offset = 0;
    if (this is Mannequin)
      offset = 2;
    spriteBatch.Draw(texture, objectPosition, new Microsoft.Xna.Framework.Rectangle?(dataOrErrorItem.GetSourceRect(offset, new int?(this.ParentSheetIndex))), Color.White, 0.0f, Vector2.Zero, 4f, SpriteEffects.None, layerDepth);
  }

  /// <summary>Draw a green or red placement tile for the held item, if applicable.</summary>
  /// <param name="spriteBatch">The sprite batch being drawn.</param>
  /// <param name="location">The current location.</param>
  public virtual void drawPlacementBounds(SpriteBatch spriteBatch, GameLocation location)
  {
    if (!this.isPlaceable() || this is Wallpaper)
      return;
    Game1.isCheckingNonMousePlacement = !Game1.IsPerformingMousePlacement();
    int x = (int) Game1.GetPlacementGrabTile().X * 64 /*0x40*/;
    int y = (int) Game1.GetPlacementGrabTile().Y * 64 /*0x40*/;
    if (Game1.isCheckingNonMousePlacement)
    {
      Vector2 placementPosition = Utility.GetNearbyValidPlacementPosition(Game1.player, location, (Item) this, x, y);
      x = (int) placementPosition.X;
      y = (int) placementPosition.Y;
    }
    Vector2 key = new Vector2((float) (x / 64 /*0x40*/), (float) (y / 64 /*0x40*/));
    if (this.Equals((object) Game1.player.ActiveObject))
      this.TileLocation = key;
    Object @object;
    if (Utility.isThereAnObjectHereWhichAcceptsThisItem(location, (Item) this, x, y) && (!location.objects.TryGetValue(key, out @object) || !(@object is IndoorPot indoorPot) ? 0 : (indoorPot.IsPlantableItem((Item) this) ? 1 : 0)) == 0)
      return;
    bool flag = Utility.playerCanPlaceItemHere(location, (Item) this, x, y, Game1.player) || Utility.isThereAnObjectHereWhichAcceptsThisItem(location, (Item) this, x, y) && Utility.withinRadiusOfPlayer(x, y, 1, Game1.player);
    Game1.isCheckingNonMousePlacement = false;
    int num1 = 1;
    int num2 = 1;
    if (this is Furniture furniture)
    {
      num1 = furniture.getTilesWide();
      num2 = furniture.getTilesHigh();
    }
    for (int index1 = 0; index1 < num1; ++index1)
    {
      for (int index2 = 0; index2 < num2; ++index2)
        spriteBatch.Draw(Game1.mouseCursors, new Vector2((float) (((double) key.X + (double) index1) * 64.0) - (float) Game1.viewport.X, (float) (((double) key.Y + (double) index2) * 64.0) - (float) Game1.viewport.Y), new Microsoft.Xna.Framework.Rectangle?(new Microsoft.Xna.Framework.Rectangle(flag ? 194 : 210, 388, 16 /*0x10*/, 16 /*0x10*/)), Color.White, 0.0f, Vector2.Zero, 4f, SpriteEffects.None, 0.01f);
    }
    if (!this.bigCraftable.Value && !(this is Furniture) && (this.category.Value == -74 || this.category.Value == -19))
      return;
    this.draw(spriteBatch, (int) key.X, (int) key.Y, 0.5f);
  }

  public override void drawInMenu(
    SpriteBatch spriteBatch,
    Vector2 location,
    float scaleSize,
    float transparency,
    float layerDepth,
    StackDrawType drawStackNumber,
    Color color,
    bool drawShadow)
  {
    this.AdjustMenuDrawForRecipes(ref transparency, ref scaleSize);
    if (drawShadow && !this.bigCraftable.Value && this.QualifiedItemId != "(O)590" && this.QualifiedItemId != "(O)SeedSpot")
      this.DrawShadow(spriteBatch, location, color, layerDepth);
    ParsedItemData dataOrErrorItem = ItemRegistry.GetDataOrErrorItem(this.QualifiedItemId);
    float num = scaleSize;
    if (this.bigCraftable.Value && (double) num > 0.20000000298023224)
      num /= 2f;
    int offset = 0;
    if (this is Mannequin)
      offset = 2;
    Microsoft.Xna.Framework.Rectangle sourceRect = dataOrErrorItem.GetSourceRect(offset, new int?(this.ParentSheetIndex));
    spriteBatch.Draw(dataOrErrorItem.GetTexture(), location + new Vector2(32f, 32f), new Microsoft.Xna.Framework.Rectangle?(sourceRect), color * transparency, 0.0f, new Vector2((float) (sourceRect.Width / 2), (float) (sourceRect.Height / 2)), 4f * num, SpriteEffects.None, layerDepth);
    this.DrawMenuIcons(spriteBatch, location, scaleSize, transparency, layerDepth, drawStackNumber, color);
  }

  /// <summary>Draw the shadow behind the item in menus.</summary>
  /// <param name="spriteBatch">The sprite batch being drawn to the screen.</param>
  /// <param name="position">The pixel position at which the item is being drawn.</param>
  /// <param name="color">The item color tint to apply.</param>
  /// <param name="layerDepth">The layer depth relative to other sprites in the current sprite batch.</param>
  public virtual void DrawShadow(
    SpriteBatch spriteBatch,
    Vector2 position,
    Color color,
    float layerDepth)
  {
    SpriteBatch spriteBatch1 = spriteBatch;
    Texture2D shadowTexture = Game1.shadowTexture;
    Vector2 position1 = position + new Vector2(32f, 48f);
    Microsoft.Xna.Framework.Rectangle? sourceRectangle = new Microsoft.Xna.Framework.Rectangle?(Game1.shadowTexture.Bounds);
    Color color1 = color * 0.5f;
    Microsoft.Xna.Framework.Rectangle bounds = Game1.shadowTexture.Bounds;
    double x = (double) bounds.Center.X;
    bounds = Game1.shadowTexture.Bounds;
    double y = (double) bounds.Center.Y;
    Vector2 origin = new Vector2((float) x, (float) y);
    double layerDepth1 = (double) layerDepth - 9.9999997473787516E-05;
    spriteBatch1.Draw(shadowTexture, position1, sourceRectangle, color1, 0.0f, origin, 3f, SpriteEffects.None, (float) layerDepth1);
  }

  public override void DrawIconBar(
    SpriteBatch spriteBatch,
    Vector2 location,
    float scaleSize,
    float transparency,
    float layerDepth,
    StackDrawType drawStackNumber,
    Color color)
  {
    if (this.Category != -22 || this.uses.Value <= 0)
      return;
    float power = ((float) (FishingRod.maxTackleUses - this.uses.Value) + 0.0f) / (float) FishingRod.maxTackleUses;
    spriteBatch.Draw(Game1.staminaRect, new Microsoft.Xna.Framework.Rectangle((int) location.X, (int) ((double) location.Y + 56.0 * (double) scaleSize), (int) (64.0 * (double) scaleSize * (double) power), (int) (8.0 * (double) scaleSize)), Utility.getRedToGreenLerpColor(power));
  }

  public virtual void drawAsProp(SpriteBatch b)
  {
    if (this.isTemporarilyInvisible)
      return;
    int x1 = (int) this.tileLocation.X;
    int y1 = (int) this.tileLocation.Y;
    if (this.bigCraftable.Value)
    {
      int offset = 0;
      if (this.showNextIndex.Value)
        offset = 1;
      ParsedItemData dataOrErrorItem = ItemRegistry.GetDataOrErrorItem(this.QualifiedItemId);
      Texture2D texture = dataOrErrorItem.GetTexture();
      Vector2 vector2 = this.getScale() * 4f;
      Vector2 local = Game1.GlobalToLocal(Game1.viewport, new Vector2((float) (x1 * 64 /*0x40*/), (float) (y1 * 64 /*0x40*/ - 64 /*0x40*/)));
      Microsoft.Xna.Framework.Rectangle destinationRectangle = new Microsoft.Xna.Framework.Rectangle((int) ((double) local.X - (double) vector2.X / 2.0), (int) ((double) local.Y - (double) vector2.Y / 2.0), (int) (64.0 + (double) vector2.X), (int) (128.0 + (double) vector2.Y / 2.0));
      b.Draw(texture, destinationRectangle, new Microsoft.Xna.Framework.Rectangle?(dataOrErrorItem.GetSourceRect(offset, new int?(this.ParentSheetIndex))), Color.White, 0.0f, Vector2.Zero, SpriteEffects.None, Math.Max(0.0f, (float) ((y1 + 1) * 64 /*0x40*/ - 1) / 10000f) + (this.IsTapper() ? 0.0015f : 0.0f));
      if (!(this.QualifiedItemId == "(BC)17") || this.MinutesUntilReady <= 0)
        return;
      b.Draw(Game1.objectSpriteSheet, this.getLocalPosition(Game1.viewport) + new Vector2(32f, 0.0f), new Microsoft.Xna.Framework.Rectangle?(Game1.getSourceRectForStandardTileSheet(Game1.objectSpriteSheet, 435)), Color.White, this.scale.X, new Vector2(32f, 32f), 1f, SpriteEffects.None, Math.Max(0.0f, (float) ((double) ((y1 + 1) * 64 /*0x40*/ - 1) / 10000.0 + 9.9999997473787516E-05)));
    }
    else
    {
      Microsoft.Xna.Framework.Rectangle boundingBoxAt = this.GetBoundingBoxAt(x1, y1);
      if (this.QualifiedItemId != "(O)590" && this.QualifiedItemId != "(O)742" && this.QualifiedItemId != "(O)SeedSpot")
      {
        SpriteBatch spriteBatch = b;
        Texture2D shadowTexture = Game1.shadowTexture;
        Vector2 position = this.getLocalPosition(Game1.viewport) + new Vector2(32f, 53f);
        Microsoft.Xna.Framework.Rectangle? sourceRectangle = new Microsoft.Xna.Framework.Rectangle?(Game1.shadowTexture.Bounds);
        Color white = Color.White;
        Microsoft.Xna.Framework.Rectangle bounds = Game1.shadowTexture.Bounds;
        double x2 = (double) bounds.Center.X;
        bounds = Game1.shadowTexture.Bounds;
        double y2 = (double) bounds.Center.Y;
        Vector2 origin = new Vector2((float) x2, (float) y2);
        double layerDepth = (double) boundingBoxAt.Bottom / 15000.0;
        spriteBatch.Draw(shadowTexture, position, sourceRectangle, white, 0.0f, origin, 4f, SpriteEffects.None, (float) layerDepth);
      }
      ParsedItemData dataOrErrorItem = ItemRegistry.GetDataOrErrorItem(this.QualifiedItemId);
      b.Draw(dataOrErrorItem.GetTexture(), Game1.GlobalToLocal(Game1.viewport, new Vector2((float) (x1 * 64 /*0x40*/ + 32 /*0x20*/), (float) (y1 * 64 /*0x40*/ + 32 /*0x20*/))), new Microsoft.Xna.Framework.Rectangle?(dataOrErrorItem.GetSourceRect()), Color.White, 0.0f, new Vector2(8f, 8f), (double) this.scale.Y > 1.0 ? this.getScale().Y : 4f, this.flipped.Value ? SpriteEffects.FlipHorizontally : SpriteEffects.None, (float) boundingBoxAt.Bottom / 10000f);
    }
  }

  public virtual void drawAboveFrontLayer(SpriteBatch spriteBatch, int x, int y, float alpha = 1f)
  {
  }

  public virtual void draw(SpriteBatch spriteBatch, int x, int y, float alpha = 1f)
  {
    if (this.isTemporarilyInvisible)
      return;
    GameLocation location = this.Location;
    if (this.hovering)
    {
      if (this.IsTextSign() && !string.IsNullOrEmpty(this.SignText))
      {
        Vector2 local = Game1.GlobalToLocal(Game1.viewport, new Vector2((float) (x * 64 /*0x40*/ + 32 /*0x20*/), (float) (y * 64 /*0x40*/ - 64 /*0x40*/)));
        SpriteText.drawSmallTextBubble(spriteBatch, this.SignText, local, 256 /*0x0100*/, (float) (0.98000001907348633 + (double) this.TileLocation.X * 9.9999997473787516E-05 + (double) this.TileLocation.Y * 9.9999999747524271E-07));
      }
      this.hovering = false;
    }
    if (this.bigCraftable.Value)
    {
      Vector2 vector2 = this.getScale() * 4f;
      Vector2 local = Game1.GlobalToLocal(Game1.viewport, new Vector2((float) (x * 64 /*0x40*/), (float) (y * 64 /*0x40*/ - 64 /*0x40*/)));
      Microsoft.Xna.Framework.Rectangle destinationRectangle = new Microsoft.Xna.Framework.Rectangle((int) ((double) local.X - (double) vector2.X / 2.0) + (this.shakeTimer > 0 ? Game1.random.Next(-1, 2) : 0), (int) ((double) local.Y - (double) vector2.Y / 2.0) + (this.shakeTimer > 0 ? Game1.random.Next(-1, 2) : 0), (int) (64.0 + (double) vector2.X), (int) (128.0 + (double) vector2.Y / 2.0));
      float layerDepth = Math.Max(0.0f, (float) ((y + 1) * 64 /*0x40*/ - 24) / 10000f) + (float) x * 1E-05f;
      int offset = 0;
      if (this.showNextIndex.Value)
        offset = 1;
      ParsedItemData dataOrErrorItem1 = ItemRegistry.GetDataOrErrorItem(this.QualifiedItemId);
      if (this.heldObject.Value != null)
      {
        MachineData machineData = this.GetMachineData();
        if ((machineData != null ? (machineData.IsIncubator ? 1 : 0) : 0) != 0)
        {
          FarmAnimalData animalDataFromEgg = FarmAnimal.GetAnimalDataFromEgg((Item) this.heldObject.Value, location);
          offset = animalDataFromEgg != null ? animalDataFromEgg.IncubatorParentSheetOffset : 1;
        }
      }
      if (this._machineAnimationFrame >= 0 && this._machineAnimation != null)
        offset = this._machineAnimationFrame;
      if (this is Mannequin mannequin)
        offset = mannequin.facing.Value;
      if (this.IsTapper())
        layerDepth = Math.Max(0.0f, (float) ((y + 1) * 64 /*0x40*/ + 2) / 10000f) + (float) x / 1000000f;
      if (this.QualifiedItemId == "(BC)272")
      {
        Texture2D texture = dataOrErrorItem1.GetTexture();
        spriteBatch.Draw(texture, destinationRectangle, new Microsoft.Xna.Framework.Rectangle?(dataOrErrorItem1.GetSourceRect(1, new int?(this.ParentSheetIndex))), Color.White * alpha, 0.0f, Vector2.Zero, SpriteEffects.None, layerDepth);
        spriteBatch.Draw(texture, local + new Vector2(8.5f, 12f) * 4f, new Microsoft.Xna.Framework.Rectangle?(dataOrErrorItem1.GetSourceRect(2, new int?(this.ParentSheetIndex))), Color.White * alpha, (float) (Game1.currentGameTime.TotalGameTime.TotalSeconds * -1.5), new Vector2(7.5f, 15.5f), 4f, SpriteEffects.None, layerDepth + 1E-05f);
        return;
      }
      spriteBatch.Draw(dataOrErrorItem1.GetTexture(), destinationRectangle, new Microsoft.Xna.Framework.Rectangle?(dataOrErrorItem1.GetSourceRect(offset, new int?(this.ParentSheetIndex))), Color.White * alpha, 0.0f, Vector2.Zero, SpriteEffects.None, layerDepth);
      if (this.QualifiedItemId == "(BC)17" && this.MinutesUntilReady > 0)
        spriteBatch.Draw(Game1.objectSpriteSheet, this.getLocalPosition(Game1.viewport) + new Vector2(32f, 0.0f), new Microsoft.Xna.Framework.Rectangle?(Game1.getSourceRectForStandardTileSheet(Game1.objectSpriteSheet, 435, 16 /*0x10*/, 16 /*0x10*/)), Color.White * alpha, this.scale.X, new Vector2(8f, 8f), 4f, SpriteEffects.None, Math.Max(0.0f, (float) ((double) ((y + 1) * 64 /*0x40*/) / 10000.0 + 9.9999997473787516E-05 + (double) x * 9.9999997473787516E-06)));
      if (this.isLamp.Value && Game1.isDarkOut(this.Location))
        spriteBatch.Draw(Game1.mouseCursors, local + new Vector2(-32f, -32f), new Microsoft.Xna.Framework.Rectangle?(new Microsoft.Xna.Framework.Rectangle(88, 1779, 32 /*0x20*/, 32 /*0x20*/)), Color.White * 0.75f, 0.0f, Vector2.Zero, 4f, SpriteEffects.None, Math.Max(0.0f, (float) ((y + 1) * 64 /*0x40*/ - 20) / 10000f) + (float) x / 1000000f);
      if (this.QualifiedItemId == "(BC)126")
      {
        string str = this.quality.Value != 0 ? (this.quality.Value - 1).ToString() : this.preservedParentSheetIndex.Value;
        if (str != null)
        {
          ParsedItemData dataOrErrorItem2 = ItemRegistry.GetDataOrErrorItem("(H)" + str);
          Texture2D texture = dataOrErrorItem2.GetTexture();
          int spriteIndex = dataOrErrorItem2.SpriteIndex;
          bool flag = ItemContextTagManager.HasBaseTag("(H)" + str, "Prismatic");
          spriteBatch.Draw(texture, local + new Vector2(-3f, -6f) * 4f, new Microsoft.Xna.Framework.Rectangle?(new Microsoft.Xna.Framework.Rectangle(spriteIndex * 20 % texture.Width, spriteIndex * 20 / texture.Width * 20 * 4, 20, 20)), (flag ? Utility.GetPrismaticColor() : Color.White) * alpha, 0.0f, Vector2.Zero, 4f, SpriteEffects.None, Math.Max(0.0f, (float) ((y + 1) * 64 /*0x40*/ - 20) / 10000f) + (float) x * 1E-05f);
        }
      }
    }
    else if (!Game1.eventUp || Game1.CurrentEvent != null && !Game1.CurrentEvent.isTileWalkedOn(x, y))
    {
      Microsoft.Xna.Framework.Rectangle boundingBoxAt = this.GetBoundingBoxAt(x, y);
      switch (this.QualifiedItemId)
      {
        case "(O)590":
          spriteBatch.Draw(Game1.mouseCursors, Game1.GlobalToLocal(Game1.viewport, new Vector2((float) (x * 64 /*0x40*/ + 32 /*0x20*/ + (this.shakeTimer > 0 ? Game1.random.Next(-1, 2) : 0)), (float) (y * 64 /*0x40*/ + 32 /*0x20*/ + (this.shakeTimer > 0 ? Game1.random.Next(-1, 2) : 0)))), new Microsoft.Xna.Framework.Rectangle?(new Microsoft.Xna.Framework.Rectangle(368 + (Game1.currentGameTime.TotalGameTime.TotalMilliseconds % 1200.0 <= 400.0 ? (int) (Game1.currentGameTime.TotalGameTime.TotalMilliseconds % 400.0 / 100.0) * 16 /*0x10*/ : 0), 32 /*0x20*/, 16 /*0x10*/, 16 /*0x10*/)), Color.White * alpha, 0.0f, new Vector2(8f, 8f), (double) this.scale.Y > 1.0 ? this.getScale().Y : 4f, this.flipped.Value ? SpriteEffects.FlipHorizontally : SpriteEffects.None, (float) ((this.isPassable() ? (double) boundingBoxAt.Top : (double) boundingBoxAt.Bottom) / 10000.0));
          return;
        case "(O)SeedSpot":
          SpriteBatch spriteBatch1 = spriteBatch;
          Texture2D mouseCursors16 = Game1.mouseCursors_1_6;
          Vector2 local = Game1.GlobalToLocal(Game1.viewport, new Vector2((float) (x * 64 /*0x40*/ + 32 /*0x20*/ + (this.shakeTimer > 0 ? Game1.random.Next(-1, 2) : 0)), (float) (y * 64 /*0x40*/ + 32 /*0x20*/ + (this.shakeTimer > 0 ? Game1.random.Next(-1, 2) : 0))));
          TimeSpan totalGameTime;
          int num;
          if (Game1.currentGameTime.TotalGameTime.TotalMilliseconds % 1600.0 > 800.0)
          {
            num = 0;
          }
          else
          {
            totalGameTime = Game1.currentGameTime.TotalGameTime;
            num = (int) (totalGameTime.TotalMilliseconds % 400.0 / 100.0) * 16 /*0x10*/;
          }
          Microsoft.Xna.Framework.Rectangle? sourceRectangle = new Microsoft.Xna.Framework.Rectangle?(new Microsoft.Xna.Framework.Rectangle(160 /*0xA0*/ + num, 0, 17, 16 /*0x10*/));
          Color color = Color.White * alpha;
          Vector2 origin = new Vector2(8f, 8f);
          double scale = (double) this.scale.Y > 1.0 ? (double) this.getScale().Y : 4.0;
          totalGameTime = Game1.currentGameTime.TotalGameTime;
          int effects = totalGameTime.TotalMilliseconds % 1600.0 <= 400.0 ? 1 : 0;
          double layerDepth = (this.isPassable() ? (double) boundingBoxAt.Top : (double) boundingBoxAt.Bottom) / 10000.0;
          spriteBatch1.Draw(mouseCursors16, local, sourceRectangle, color, 0.0f, origin, (float) scale, (SpriteEffects) effects, (float) layerDepth);
          return;
        default:
          if (this.fragility.Value != 2)
            spriteBatch.Draw(Game1.shadowTexture, Game1.GlobalToLocal(Game1.viewport, new Vector2((float) (x * 64 /*0x40*/ + 32 /*0x20*/), (float) (y * 64 /*0x40*/ + 51 + 4))), new Microsoft.Xna.Framework.Rectangle?(Game1.shadowTexture.Bounds), Color.White * alpha, 0.0f, new Vector2((float) Game1.shadowTexture.Bounds.Center.X, (float) Game1.shadowTexture.Bounds.Center.Y), 4f, SpriteEffects.None, (float) boundingBoxAt.Bottom / 15000f);
          ParsedItemData dataOrErrorItem3 = ItemRegistry.GetDataOrErrorItem(this.QualifiedItemId);
          spriteBatch.Draw(dataOrErrorItem3.GetTexture(), Game1.GlobalToLocal(Game1.viewport, new Vector2((float) (x * 64 /*0x40*/ + 32 /*0x20*/ + (this.shakeTimer > 0 ? Game1.random.Next(-1, 2) : 0)), (float) (y * 64 /*0x40*/ + 32 /*0x20*/ + (this.shakeTimer > 0 ? Game1.random.Next(-1, 2) : 0)))), new Microsoft.Xna.Framework.Rectangle?(dataOrErrorItem3.GetSourceRect()), Color.White * alpha, 0.0f, new Vector2(8f, 8f), (double) this.scale.Y > 1.0 ? this.getScale().Y : 4f, this.flipped.Value ? SpriteEffects.FlipHorizontally : SpriteEffects.None, (float) ((this.isPassable() ? (double) boundingBoxAt.Top : (double) boundingBoxAt.Center.Y) / 10000.0));
          if (this.IsSprinkler())
          {
            if (this.heldObject.Value != null)
            {
              Vector2 vector2 = Vector2.Zero;
              if (this.heldObject.Value.QualifiedItemId == "(O)913")
                vector2 = new Vector2(0.0f, -20f);
              ParsedItemData dataOrErrorItem4 = ItemRegistry.GetDataOrErrorItem(this.heldObject.Value.QualifiedItemId);
              spriteBatch.Draw(dataOrErrorItem4.GetTexture(), Game1.GlobalToLocal(Game1.viewport, new Vector2((float) (x * 64 /*0x40*/ + 32 /*0x20*/ + (this.shakeTimer > 0 ? Game1.random.Next(-1, 2) : 0)), (float) (y * 64 /*0x40*/ + 32 /*0x20*/ + (this.shakeTimer > 0 ? Game1.random.Next(-1, 2) : 0))) + vector2), new Microsoft.Xna.Framework.Rectangle?(dataOrErrorItem4.GetSourceRect(1)), Color.White * alpha, 0.0f, new Vector2(8f, 8f), (double) this.scale.Y > 1.0 ? this.getScale().Y : 4f, this.flipped.Value ? SpriteEffects.FlipHorizontally : SpriteEffects.None, (float) ((this.isPassable() ? (double) boundingBoxAt.Top : (double) boundingBoxAt.Bottom) / 10000.0 + 9.9999997473787516E-06));
            }
            if (this.SpecialVariable == 999999)
            {
              if (this.heldObject.Value != null && this.heldObject.Value.QualifiedItemId == "(O)913")
              {
                Torch.drawBasicTorch(spriteBatch, (float) (x * 64 /*0x40*/) - 2f, (float) (y * 64 /*0x40*/ - 32 /*0x20*/), (float) ((double) boundingBoxAt.Bottom / 10000.0 + 9.9999999747524271E-07));
                break;
              }
              Torch.drawBasicTorch(spriteBatch, (float) (x * 64 /*0x40*/) - 2f, (float) (y * 64 /*0x40*/ - 32 /*0x20*/ + 12), (float) (boundingBoxAt.Bottom + 2) / 10000f);
              break;
            }
            break;
          }
          break;
      }
    }
    if (!this.readyForHarvest.Value)
      return;
    float num1 = (float) ((double) ((y + 1) * 64 /*0x40*/) / 10000.0 + (double) this.tileLocation.X / 50000.0);
    if (this.IsTapper() || this.QualifiedItemId.Equals("(BC)MushroomLog"))
      num1 += 0.02f;
    float num2 = (float) (4.0 * Math.Round(Math.Sin(Game1.currentGameTime.TotalGameTime.TotalMilliseconds / 250.0), 2));
    spriteBatch.Draw(Game1.mouseCursors, Game1.GlobalToLocal(Game1.viewport, new Vector2((float) (x * 64 /*0x40*/ - 8), (float) (y * 64 /*0x40*/ - 96 /*0x60*/ - 16 /*0x10*/) + num2)), new Microsoft.Xna.Framework.Rectangle?(new Microsoft.Xna.Framework.Rectangle(141, 465, 20, 24)), Color.White * 0.75f, 0.0f, Vector2.Zero, 4f, SpriteEffects.None, num1 + 1E-06f);
    if (this.heldObject.Value == null)
      return;
    ParsedItemData dataOrErrorItem = ItemRegistry.GetDataOrErrorItem(this.heldObject.Value.QualifiedItemId);
    Texture2D texture1 = dataOrErrorItem.GetTexture();
    if (this.heldObject.Value is ColoredObject coloredObject)
    {
      coloredObject.drawInMenu(spriteBatch, Game1.GlobalToLocal(Game1.viewport, new Vector2((float) (x * 64 /*0x40*/), (float) ((double) (y * 64 /*0x40*/) - 96.0 - 8.0) + num2)), 1f, 0.75f, num1 + 1.1E-05f);
    }
    else
    {
      spriteBatch.Draw(texture1, Game1.GlobalToLocal(Game1.viewport, new Vector2((float) (x * 64 /*0x40*/ + 32 /*0x20*/), (float) (y * 64 /*0x40*/ - 64 /*0x40*/ - 8) + num2)), new Microsoft.Xna.Framework.Rectangle?(dataOrErrorItem.GetSourceRect()), Color.White * 0.75f, 0.0f, new Vector2(8f, 8f), 4f, SpriteEffects.None, num1 + 1E-05f);
      if (this.heldObject.Value.Stack > 1)
      {
        this.heldObject.Value.DrawMenuIcons(spriteBatch, Game1.GlobalToLocal(Game1.viewport, new Vector2((float) (x * 64 /*0x40*/), (float) ((double) (y * 64 /*0x40*/ - 64 /*0x40*/ - 32 /*0x20*/) + (double) num2 - 4.0))), 1f, 1f, num1 + 1.2E-05f, StackDrawType.Draw, Color.White);
      }
      else
      {
        if (this.heldObject.Value.Quality <= 0)
          return;
        this.heldObject.Value.DrawMenuIcons(spriteBatch, Game1.GlobalToLocal(Game1.viewport, new Vector2((float) (x * 64 /*0x40*/), (float) ((double) (y * 64 /*0x40*/ - 64 /*0x40*/ - 32 /*0x20*/) + (double) num2 - 4.0))), 1f, 1f, num1 + 1.2E-05f, StackDrawType.HideButShowQuality, Color.White);
      }
    }
  }

  public virtual void draw(
    SpriteBatch spriteBatch,
    int xNonTile,
    int yNonTile,
    float layerDepth,
    float alpha = 1f)
  {
    if (this.isTemporarilyInvisible)
      return;
    if (this.bigCraftable.Value)
    {
      Vector2 vector2 = this.getScale() * 4f;
      Vector2 local = Game1.GlobalToLocal(Game1.viewport, new Vector2((float) xNonTile, (float) yNonTile));
      Microsoft.Xna.Framework.Rectangle destinationRectangle = new Microsoft.Xna.Framework.Rectangle((int) ((double) local.X - (double) vector2.X / 2.0) + (this.shakeTimer > 0 ? Game1.random.Next(-1, 2) : 0), (int) ((double) local.Y - (double) vector2.Y / 2.0) + (this.shakeTimer > 0 ? Game1.random.Next(-1, 2) : 0), (int) (64.0 + (double) vector2.X), (int) (128.0 + (double) vector2.Y / 2.0));
      int offset = 0;
      if (this.showNextIndex.Value)
        offset = 1;
      ParsedItemData dataOrErrorItem = ItemRegistry.GetDataOrErrorItem(this.QualifiedItemId);
      spriteBatch.Draw(dataOrErrorItem.GetTexture(), destinationRectangle, new Microsoft.Xna.Framework.Rectangle?(dataOrErrorItem.GetSourceRect(offset, new int?(this.ParentSheetIndex))), Color.White * alpha, 0.0f, Vector2.Zero, SpriteEffects.None, layerDepth);
      if (this.QualifiedItemId == "(BC)17" && this.MinutesUntilReady > 0)
        spriteBatch.Draw(Game1.objectSpriteSheet, Game1.GlobalToLocal(local) + new Vector2(32f, 0.0f), new Microsoft.Xna.Framework.Rectangle?(Game1.getSourceRectForStandardTileSheet(Game1.objectSpriteSheet, 435, 16 /*0x10*/, 16 /*0x10*/)), Color.White * alpha, this.scale.X, new Vector2(8f, 8f), 4f, SpriteEffects.None, layerDepth);
      if (!this.isLamp.Value || !Game1.isDarkOut(this.Location))
        return;
      spriteBatch.Draw(Game1.mouseCursors, local + new Vector2(-32f, -32f), new Microsoft.Xna.Framework.Rectangle?(new Microsoft.Xna.Framework.Rectangle(88, 1779, 32 /*0x20*/, 32 /*0x20*/)), Color.White * 0.75f, 0.0f, Vector2.Zero, 4f, SpriteEffects.None, layerDepth);
    }
    else
    {
      if (Game1.eventUp && Game1.CurrentEvent.isTileWalkedOn(xNonTile / 64 /*0x40*/, yNonTile / 64 /*0x40*/))
        return;
      if (this.QualifiedItemId != "(O)590" && this.QualifiedItemId != "(O)SeedSpot" && this.fragility.Value != 2)
      {
        SpriteBatch spriteBatch1 = spriteBatch;
        Texture2D shadowTexture = Game1.shadowTexture;
        Vector2 local = Game1.GlobalToLocal(Game1.viewport, new Vector2((float) (xNonTile + 32 /*0x20*/), (float) (yNonTile + 51 + 4)));
        Microsoft.Xna.Framework.Rectangle? sourceRectangle = new Microsoft.Xna.Framework.Rectangle?(Game1.shadowTexture.Bounds);
        Color color = Color.White * alpha;
        Microsoft.Xna.Framework.Rectangle bounds = Game1.shadowTexture.Bounds;
        double x = (double) bounds.Center.X;
        bounds = Game1.shadowTexture.Bounds;
        double y = (double) bounds.Center.Y;
        Vector2 origin = new Vector2((float) x, (float) y);
        double layerDepth1 = (double) layerDepth - 9.9999999747524271E-07;
        spriteBatch1.Draw(shadowTexture, local, sourceRectangle, color, 0.0f, origin, 4f, SpriteEffects.None, (float) layerDepth1);
      }
      ParsedItemData dataOrErrorItem = ItemRegistry.GetDataOrErrorItem(this.QualifiedItemId);
      spriteBatch.Draw(dataOrErrorItem.GetTexture(), Game1.GlobalToLocal(Game1.viewport, new Vector2((float) (xNonTile + 32 /*0x20*/ + (this.shakeTimer > 0 ? Game1.random.Next(-1, 2) : 0)), (float) (yNonTile + 32 /*0x20*/ + (this.shakeTimer > 0 ? Game1.random.Next(-1, 2) : 0)))), new Microsoft.Xna.Framework.Rectangle?(dataOrErrorItem.GetSourceRect(spriteIndex: new int?(this.ParentSheetIndex))), Color.White * alpha, 0.0f, new Vector2(8f, 8f), (double) this.scale.Y > 1.0 ? this.getScale().Y : 4f, this.flipped.Value ? SpriteEffects.FlipHorizontally : SpriteEffects.None, layerDepth);
    }
  }

  public override int maximumStackSize()
  {
    string qualifiedItemId = this.QualifiedItemId;
    return qualifiedItemId == "(O)79" || qualifiedItemId == "(O)842" || qualifiedItemId == "(O)911" || this.Category == -22 ? 1 : 999;
  }

  public virtual void hoverAction() => this.hovering = true;

  public virtual bool clicked(Farmer who) => false;

  /// <inheritdoc />
  protected override Item GetOneNew()
  {
    return !this.bigCraftable.Value ? (Item) new Object(this.ItemId, 1) : (Item) new Object(this.tileLocation.Value, this.ItemId);
  }

  /// <inheritdoc />
  protected override void GetOneCopyFrom(Item source)
  {
    base.GetOneCopyFrom(source);
    if (!(source is Object @object))
      return;
    this.Scale = @object.scale;
    this.IsSpawnedObject = @object.isSpawnedObject.Value;
    this.Price = @object.price.Value;
    this.Edibility = @object.edibility.Value;
    this.name = @object.name;
    this.displayNameFormat = @object.displayNameFormat;
    this.TileLocation = @object.TileLocation;
    this.uses.Value = @object.uses.Value;
    this.questItem.Value = @object.questItem.Value;
    this.questId.Value = @object.questId.Value;
    this.preserve.Value = @object.preserve.Value;
    this.preservedParentSheetIndex.Value = @object.preservedParentSheetIndex.Value;
    this.orderData.Value = @object.orderData.Value;
    this.owner.Value = @object.owner.Value;
  }

  public override bool canBePlacedHere(
    GameLocation l,
    Vector2 tile,
    CollisionMask collisionMask = CollisionMask.All,
    bool showError = false)
  {
    if (this.QualifiedItemId == "(O)710")
      return CrabPot.IsValidCrabPotLocationTile(l, (int) tile.X, (int) tile.Y);
    if (this.IsTapper() && l.terrainFeatures.GetValueOrDefault(tile) is Tree valueOrDefault1 && !l.objects.ContainsKey(tile))
    {
      bool? nullable = valueOrDefault1.GetData()?.CanBeTapped();
      if (nullable.HasValue && nullable.GetValueOrDefault())
        return true;
    }
    switch (this.QualifiedItemId)
    {
      case "(O)805":
        if (l.terrainFeatures.GetValueOrDefault(tile) is Tree)
          return true;
        break;
      case "(O)419":
        return l.terrainFeatures.GetValueOrDefault(tile) is Tree valueOrDefault2 && !valueOrDefault2.stopGrowingMoss.Value;
    }
    if (Object.isWildTreeSeed(this.ItemId))
    {
      if (!l.CanItemBePlacedHere(tile, true, collisionMask))
        return false;
      string deniedMessage;
      if (this.canPlaceWildTreeSeed(l, tile, out deniedMessage))
        return true;
      if (showError && deniedMessage != null)
        Game1.showRedMessage(deniedMessage);
      return false;
    }
    switch (this.category.Value)
    {
      case -74:
        HoeDirt hoeDirtAtTile1 = l.GetHoeDirtAtTile(tile);
        Object objectAtTile1 = l.getObjectAtTile((int) tile.X, (int) tile.Y);
        IndoorPot indoorPot = objectAtTile1 as IndoorPot;
        if (hoeDirtAtTile1?.crop != null || hoeDirtAtTile1 == null && l.terrainFeatures.TryGetValue(tile, out TerrainFeature _))
          return false;
        if (this.IsFruitTreeSapling())
        {
          if (objectAtTile1 != null || hoeDirtAtTile1 != null)
            return false;
          if (FruitTree.IsTooCloseToAnotherTree(tile, l, !this.IsFruitTreeSapling()))
          {
            if (showError)
              Game1.showRedMessage(Game1.content.LoadString("Strings\\StringsFromCSFiles:Object.cs.13060"));
            return false;
          }
          if (FruitTree.IsGrowthBlocked(tile, l))
          {
            if (showError)
              Game1.showRedMessage(Game1.content.LoadString("Strings\\UI:FruitTree_PlacementWarning", (object) this.DisplayName));
            return false;
          }
          if (!l.CanItemBePlacedHere(tile, true, collisionMask))
            return false;
          string deniedMessage;
          if (l.CanPlantTreesHere(this.ItemId, (int) tile.X, (int) tile.Y, out deniedMessage))
            return true;
          if (showError && deniedMessage != null)
            Game1.showRedMessage(deniedMessage);
          return false;
        }
        if (this.IsTeaSapling())
        {
          bool isGardenPot = indoorPot != null && indoorPot.bush.Value == null && indoorPot.hoeDirt.Value.crop == null;
          if (isGardenPot)
          {
            if (!l.IsOutdoors)
              return true;
          }
          else if (objectAtTile1 != null || hoeDirtAtTile1 != null || !l.CanItemBePlacedHere(tile, true, collisionMask) || l.IsGreenhouse && l.doesTileHaveProperty((int) tile.X, (int) tile.Y, "Diggable", "Back") == null)
            return false;
          string deniedMessage;
          if (l.CheckItemPlantRules(this.QualifiedItemId, isGardenPot, l.isOutdoors.Value || l.IsGreenhouse, out deniedMessage))
            return true;
          if (showError && deniedMessage != null)
            Game1.showRedMessage(Game1.content.LoadString(deniedMessage));
          return false;
        }
        if (this.IsWildTreeSapling())
        {
          if (objectAtTile1 != null)
            return false;
          if (!FruitTree.IsTooCloseToAnotherTree(tile, l, true))
            return l.CanItemBePlacedHere(tile, true, collisionMask);
          if (showError)
            Game1.showRedMessage(Game1.content.LoadString("Strings\\StringsFromCSFiles:Object.cs.13060_Fruit"));
          return false;
        }
        if (this.HasTypeObject())
        {
          if (indoorPot != null)
            return indoorPot.IsPlantableItem((Item) this) && indoorPot.bush.Value == null && indoorPot.hoeDirt.Value.canPlantThisSeedHere(this.ItemId);
          if (hoeDirtAtTile1 != null && l.CanItemBePlacedHere(tile, true, collisionMask) && hoeDirtAtTile1.canPlantThisSeedHere(this.ItemId))
            return true;
        }
        return false;
      case -19:
        HoeDirt hoeDirtAtTile2 = l.GetHoeDirtAtTile(tile);
        if (hoeDirtAtTile2 == null || !hoeDirtAtTile2.CanApplyFertilizer(this.QualifiedItemId))
          return false;
        return !(l.getObjectAtTile((int) tile.X, (int) tile.Y) is IndoorPot objectAtTile2) || objectAtTile2.IsPlantableItem((Item) this);
      default:
        if (l != null)
        {
          Vector2 vector2 = tile * 64f * 64f;
          vector2.X += 32f;
          vector2.Y += 32f;
          foreach (Furniture furniture in l.furniture)
          {
            if (furniture.furniture_type.Value == 11 && furniture.GetBoundingBox().Contains((int) vector2.X, (int) vector2.Y) && furniture.heldObject.Value == null)
              return true;
          }
        }
        if (this.IsFloorPathItem())
          collisionMask &= CollisionMask.Characters | CollisionMask.Farmers | CollisionMask.Flooring | CollisionMask.Furniture | CollisionMask.Objects | CollisionMask.TerrainFeatures | CollisionMask.LocationSpecific;
        return l.CanItemBePlacedHere(tile, this.isPassable(), collisionMask);
    }
  }

  public override bool isPlaceable()
  {
    return this.HasContextTag("placeable") || !this.HasContextTag("not_placeable") && this.type.Value != null && (this.Category == -8 || this.Category == -9 || this.Type == "Crafting" || this.isSapling() || this.QualifiedItemId == "(O)710" || this.Category == -74 || this.Category == -19) && (this.edibility.Value < 0 || this.IsWildTreeSapling());
  }

  public bool IsConsideredReadyMachineForComputer()
  {
    if (this.bigCraftable.Value && this.heldObject.Value != null)
    {
      if (!(this.heldObject.Value is Chest chest))
        return this.minutesUntilReady.Value <= 0;
      if (!chest.isEmpty())
        return true;
    }
    return false;
  }

  public MachineData GetMachineData()
  {
    return DataLoader.Machines(Game1.content).GetValueOrDefault<string, MachineData>(this.QualifiedItemId);
  }

  public virtual bool isSapling()
  {
    return this.IsTeaSapling() || this.IsWildTreeSapling() || this.IsFruitTreeSapling();
  }

  public virtual bool IsTeaSapling() => this.QualifiedItemId == "(O)251";

  public virtual bool IsFruitTreeSapling()
  {
    return this.HasTypeObject() && Game1.fruitTreeData.ContainsKey(this.ItemId);
  }

  public virtual bool IsWildTreeSapling()
  {
    return this.HasTypeObject() && Object.isWildTreeSeed(this.ItemId);
  }

  public virtual bool IsFloorPathItem()
  {
    return this.HasTypeObject() && Object.IsFloorPathItem(this.ItemId);
  }

  public static bool IsFloorPathItem(string itemId)
  {
    return itemId != null && Flooring.GetFloorPathItemLookup().ContainsKey(itemId);
  }

  public virtual bool IsFenceItem()
  {
    return this.HasTypeObject() && Fence.GetFenceLookup().ContainsKey(this.ItemId);
  }

  public static bool isWildTreeSeed(string itemId)
  {
    return itemId != null && Tree.GetWildTreeSeedLookup().ContainsKey(itemId);
  }

  private bool canPlaceWildTreeSeed(GameLocation location, Vector2 tile, out string deniedMessage)
  {
    if (location.IsNoSpawnTile(tile, "Tree", true))
    {
      deniedMessage = (string) null;
      return false;
    }
    if (location.IsNoSpawnTile(tile, "Tree") && !location.doesEitherTileOrTileIndexPropertyEqual((int) tile.X, (int) tile.Y, "CanPlantTrees", "Back", "T"))
    {
      deniedMessage = (string) null;
      return false;
    }
    if (location.objects.ContainsKey(tile))
    {
      deniedMessage = (string) null;
      return false;
    }
    TerrainFeature terrainFeature;
    if (location.terrainFeatures.TryGetValue(tile, out terrainFeature) && !(terrainFeature is HoeDirt))
    {
      deniedMessage = (string) null;
      return false;
    }
    return location.CanPlantTreesHere(this.ItemId, (int) tile.X, (int) tile.Y, out deniedMessage) && location.CheckItemPlantRules(this.QualifiedItemId, false, location is Farm || location.doesTileHaveProperty((int) tile.X, (int) tile.Y, "Diggable", "Back") != null || location.doesEitherTileOrTileIndexPropertyEqual((int) tile.X, (int) tile.Y, "CanPlantTrees", "Back", "T"), out deniedMessage);
  }

  public virtual bool IsSprinkler() => this.GetBaseRadiusForSprinkler() >= 0;

  /// <summary>Get whether this is a stone litter item which can be broken by a pickaxe.</summary>
  public bool IsBreakableStone() => this.Category == -999 && this.Name == "Stone";

  /// <summary>Get whether this is a text sign which shows the <see cref="P:StardewValley.Object.SignText" /> text on hover.</summary>
  public virtual bool IsTextSign() => this.ItemId == "TextSign";

  /// <summary>Get whether this is a twig litter item.</summary>
  public bool IsTwig() => this.Category == -999 && this.Name == "Twig";

  public bool isDebrisOrForage()
  {
    return this.IsWeeds() || this.IsBreakableStone() || this.IsTwig() || this.isForage();
  }

  /// <summary>Get whether this is a weed litter item.</summary>
  public bool IsWeeds() => this.Category == -999 && this.Name.ContainsIgnoreCase("weeds");

  /// <summary>Get whether this is a tree tapper item.</summary>
  public virtual bool IsTapper() => this.HasContextTag("tapper_item");

  /// <summary>Get whether this is an ore bar.</summary>
  public virtual bool IsBar()
  {
    return this.QualifiedItemId == "(O)334" || this.QualifiedItemId == "(O)335" || this.QualifiedItemId == "(O)336" || this.QualifiedItemId == "(O)337" || this.QualifiedItemId == "(O)910";
  }

  /// <summary>Get the item ID which was preserved as part of this item (e.g. the tulip ID if this item is tulip honey).</summary>
  /// <inheritdoc cref="M:StardewValley.Object.GetPreservedItemId(System.Nullable{StardewValley.Object.PreserveType},System.String)" path="/remarks" />
  public string GetPreservedItemId()
  {
    return Object.GetPreservedItemId(this.preserve.Value, this.preservedParentSheetIndex.Value);
  }

  /// <summary>Get the item ID which was preserved as part of an item (e.g. the tulip ID for a tulip honey item).</summary>
  /// <param name="preserveType">The preserve type.</param>
  /// <param name="preservedId">The item ID for the flavor item, like <c>(O)258</c> for the blueberry in "Blueberry Wine".</param>
  /// <remarks>This returns the <see cref="F:StardewValley.Object.preservedParentSheetIndex" />, except in special cases like <see cref="F:StardewValley.Object.WildHoneyPreservedId" /> where it doesn't match an item ID.</remarks>
  public static string GetPreservedItemId(Object.PreserveType? preserveType, string preservedId)
  {
    if (preservedId == "-1" && preserveType.GetValueOrDefault() == Object.PreserveType.Honey)
      preservedId = (string) null;
    return preservedId;
  }

  public virtual int GetModifiedRadiusForSprinkler()
  {
    int radiusForSprinkler = this.GetBaseRadiusForSprinkler();
    if (radiusForSprinkler < 0)
      return -1;
    if (this.heldObject.Value != null && this.heldObject.Value.QualifiedItemId == "(O)915")
      ++radiusForSprinkler;
    return radiusForSprinkler;
  }

  public virtual int GetBaseRadiusForSprinkler()
  {
    switch (this.QualifiedItemId)
    {
      case "(O)599":
        return 0;
      case "(O)621":
        return 1;
      case "(O)645":
        return 2;
      default:
        return -1;
    }
  }

  /// <summary>Check whether the object can be added to a location, and (sometimes) add it to the location.</summary>
  /// <param name="location">The location in which to place it.</param>
  /// <param name="x">The X tile position at which to place it.</param>
  /// <param name="y">The Y tile position at which to place it.</param>
  /// <param name="who">The player placing the object, if applicable.</param>
  /// <returns>Returns whether the object should be (or was) added to the location.</returns>
  /// <remarks>For legacy reasons, the behavior of this method is inconsistent. It'll sometimes add the object to the location itself, and sometimes expect the caller to do it.</remarks>
  public virtual bool placementAction(GameLocation location, int x, int y, Farmer who = null)
  {
    Vector2 vector2_1 = new Vector2((float) (x / 64 /*0x40*/), (float) (y / 64 /*0x40*/));
    this.health = 10;
    this.Location = location;
    this.TileLocation = vector2_1;
    NetLong owner = this.owner;
    Farmer farmer = who;
    long num1 = farmer != null ? farmer.UniqueMultiplayerID : Game1.player.UniqueMultiplayerID;
    owner.Value = num1;
    if (!this.bigCraftable.Value && !(this is Furniture))
    {
      if (this.IsSprinkler() && location.doesTileHavePropertyNoNull((int) vector2_1.X, (int) vector2_1.Y, "NoSprinklers", "Back") == "T")
      {
        Game1.showRedMessage(Game1.content.LoadString("Strings\\StringsFromCSFiles:NoSprinklers"));
        return false;
      }
      if (this.IsWildTreeSapling())
      {
        string deniedMessage;
        if (!this.canPlaceWildTreeSeed(location, vector2_1, out deniedMessage))
        {
          if (deniedMessage == null)
            deniedMessage = Game1.content.LoadString("Strings\\StringsFromCSFiles:Object.cs.13021");
          Game1.showRedMessage(deniedMessage);
          return false;
        }
        string id = Tree.ResolveTreeTypeFromSeed(this.QualifiedItemId);
        if (id == null)
          return false;
        int num2 = (int) Game1.stats.Increment("wildtreesplanted");
        location.terrainFeatures.Remove(vector2_1);
        location.terrainFeatures.Add(vector2_1, (TerrainFeature) new Tree(id, 0));
        location.playSound("dirtyHit");
        return true;
      }
      if (this.IsFloorPathItem())
      {
        if (location.terrainFeatures.ContainsKey(vector2_1))
          return false;
        string str = Flooring.GetFloorPathItemLookup()[this.ItemId];
        location.terrainFeatures.Add(vector2_1, (TerrainFeature) new Flooring(str));
        FloorPathData floorPathData;
        if (Game1.floorPathData.TryGetValue(str, out floorPathData) && floorPathData.PlacementSound != null)
          location.playSound(floorPathData.PlacementSound);
        return true;
      }
      if (ItemContextTagManager.HasBaseTag(this.QualifiedItemId, "torch_item"))
      {
        if (location.objects.ContainsKey(vector2_1))
          return false;
        location.removeLightSource(this.GenerateLightSourceId(this.tileLocation.Value));
        location.removeLightSource(this.lightSource?.Id);
        new Torch(1, this.ItemId).placementAction(location, x, y, who ?? Game1.player);
        return true;
      }
      if (this.IsFenceItem())
      {
        if (location.objects.ContainsKey(vector2_1))
          return false;
        FenceData fenceData = Fence.GetFenceLookup()[this.ItemId];
        location.objects.Add(vector2_1, (Object) new Fence(vector2_1, this.ItemId, this.ItemId == "325"));
        if (fenceData.PlacementSound != null)
          location.playSound(fenceData.PlacementSound);
        return true;
      }
      string qualifiedItemId = this.QualifiedItemId;
      if (qualifiedItemId != null)
      {
        switch (qualifiedItemId.Length)
        {
          case 6:
            switch (qualifiedItemId[5])
            {
              case '0':
                if (qualifiedItemId == "(O)710")
                {
                  if (!CrabPot.IsValidCrabPotLocationTile(location, (int) vector2_1.X, (int) vector2_1.Y))
                    return false;
                  new CrabPot().placementAction(location, x, y, who);
                  return true;
                }
                goto label_211;
              case '3':
                if (qualifiedItemId == "(O)893")
                  break;
                goto label_211;
              case '4':
                if (qualifiedItemId == "(O)894")
                  break;
                goto label_211;
              case '5':
                switch (qualifiedItemId)
                {
                  case "(O)895":
                    break;
                  case "(O)805":
                    TerrainFeature terrainFeature1;
                    return location.terrainFeatures.TryGetValue(vector2_1, out terrainFeature1) && terrainFeature1 is Tree tree1 && tree1.fertilize();
                  default:
                    goto label_211;
                }
                break;
              case '6':
                switch (qualifiedItemId)
                {
                  case "(O)926":
                    if (location.objects.ContainsKey(vector2_1) || location.terrainFeatures.ContainsKey(vector2_1))
                      return false;
                    OverlaidDictionary objects = location.objects;
                    Vector2 key1 = vector2_1;
                    Torch torch = new Torch("278", true);
                    torch.Fragility = 1;
                    torch.destroyOvernight = true;
                    objects.Add(key1, (Object) torch);
                    Utility.addSmokePuff(location, new Vector2((float) x, (float) y));
                    Utility.addSmokePuff(location, new Vector2((float) (x + 16 /*0x10*/), (float) (y + 16 /*0x10*/)));
                    Utility.addSmokePuff(location, new Vector2((float) (x + 32 /*0x20*/), (float) y));
                    Utility.addSmokePuff(location, new Vector2((float) (x + 48 /*0x30*/), (float) (y + 16 /*0x10*/)));
                    Utility.addSmokePuff(location, new Vector2((float) (x + 32 /*0x20*/), (float) (y + 32 /*0x20*/)));
                    Game1.playSound("fireball");
                    return true;
                  case "(O)286":
                    foreach (TemporaryAnimatedSprite temporarySprite in location.temporarySprites)
                    {
                      if (temporarySprite.position.Equals(vector2_1 * 64f))
                        return false;
                    }
                    int num3 = Game1.random.Next();
                    location.playSound("thudStep");
                    Game1.multiplayer.broadcastSprites(location, new TemporaryAnimatedSprite(this.ParentSheetIndex, 100f, 1, 24, vector2_1 * 64f, true, false, location, who)
                    {
                      shakeIntensity = 0.5f,
                      shakeIntensityChange = 1f / 500f,
                      extraInfoForEndBehavior = num3,
                      endFunction = new TemporaryAnimatedSprite.endBehavior(location.removeTemporarySpritesWithID)
                    });
                    Game1.multiplayer.broadcastSprites(location, new TemporaryAnimatedSprite("LooseSprites\\Cursors", new Microsoft.Xna.Framework.Rectangle(598, 1279 /*0x04FF*/, 3, 4), 53f, 5, 9, vector2_1 * 64f + new Vector2(5f, 3f) * 4f, true, false, (float) (y + 7) / 10000f, 0.0f, Color.Yellow, 4f, 0.0f, 0.0f, 0.0f)
                    {
                      id = num3
                    });
                    Game1.multiplayer.broadcastSprites(location, new TemporaryAnimatedSprite("LooseSprites\\Cursors", new Microsoft.Xna.Framework.Rectangle(598, 1279 /*0x04FF*/, 3, 4), 53f, 5, 9, vector2_1 * 64f + new Vector2(5f, 3f) * 4f, true, true, (float) (y + 7) / 10000f, 0.0f, Color.Orange, 4f, 0.0f, 0.0f, 0.0f)
                    {
                      delayBeforeAnimationStart = 100,
                      id = num3
                    });
                    Game1.multiplayer.broadcastSprites(location, new TemporaryAnimatedSprite("LooseSprites\\Cursors", new Microsoft.Xna.Framework.Rectangle(598, 1279 /*0x04FF*/, 3, 4), 53f, 5, 9, vector2_1 * 64f + new Vector2(5f, 3f) * 4f, true, false, (float) (y + 7) / 10000f, 0.0f, Color.White, 3f, 0.0f, 0.0f, 0.0f)
                    {
                      delayBeforeAnimationStart = 200,
                      id = num3
                    });
                    location.netAudio.StartPlaying("fuse");
                    return true;
                  default:
                    goto label_211;
                }
              case '7':
                switch (qualifiedItemId)
                {
                  case "(O)287":
                    foreach (TemporaryAnimatedSprite temporarySprite in location.temporarySprites)
                    {
                      if (temporarySprite.position.Equals(vector2_1 * 64f))
                        return false;
                    }
                    int num4 = Game1.random.Next();
                    location.playSound("thudStep");
                    Game1.multiplayer.broadcastSprites(location, new TemporaryAnimatedSprite(this.ParentSheetIndex, 100f, 1, 24, vector2_1 * 64f, true, false, location, who)
                    {
                      shakeIntensity = 0.5f,
                      shakeIntensityChange = 1f / 500f,
                      extraInfoForEndBehavior = num4,
                      endFunction = new TemporaryAnimatedSprite.endBehavior(location.removeTemporarySpritesWithID)
                    });
                    Game1.multiplayer.broadcastSprites(location, new TemporaryAnimatedSprite("LooseSprites\\Cursors", new Microsoft.Xna.Framework.Rectangle(598, 1279 /*0x04FF*/, 3, 4), 53f, 5, 9, vector2_1 * 64f, true, false, (float) (y + 7) / 10000f, 0.0f, Color.Yellow, 4f, 0.0f, 0.0f, 0.0f)
                    {
                      id = num4
                    });
                    Game1.multiplayer.broadcastSprites(location, new TemporaryAnimatedSprite("LooseSprites\\Cursors", new Microsoft.Xna.Framework.Rectangle(598, 1279 /*0x04FF*/, 3, 4), 53f, 5, 9, vector2_1 * 64f, true, false, (float) (y + 7) / 10000f, 0.0f, Color.Orange, 4f, 0.0f, 0.0f, 0.0f)
                    {
                      delayBeforeAnimationStart = 100,
                      id = num4
                    });
                    Game1.multiplayer.broadcastSprites(location, new TemporaryAnimatedSprite("LooseSprites\\Cursors", new Microsoft.Xna.Framework.Rectangle(598, 1279 /*0x04FF*/, 3, 4), 53f, 5, 9, vector2_1 * 64f, true, false, (float) (y + 7) / 10000f, 0.0f, Color.White, 3f, 0.0f, 0.0f, 0.0f)
                    {
                      delayBeforeAnimationStart = 200,
                      id = num4
                    });
                    location.netAudio.StartPlaying("fuse");
                    return true;
                  case "(O)297":
                    if (location.objects.ContainsKey(vector2_1) || location.terrainFeatures.ContainsKey(vector2_1))
                      return false;
                    location.terrainFeatures.Add(vector2_1, (TerrainFeature) new Grass(1, 4));
                    location.playSound("dirtyHit");
                    return true;
                  default:
                    goto label_211;
                }
              case '8':
                if (qualifiedItemId == "(O)288")
                {
                  foreach (TemporaryAnimatedSprite temporarySprite in location.temporarySprites)
                  {
                    if (temporarySprite.position.Equals(vector2_1 * 64f))
                      return false;
                  }
                  int num5 = Game1.random.Next();
                  location.playSound("thudStep");
                  Game1.multiplayer.broadcastSprites(location, new TemporaryAnimatedSprite(this.ParentSheetIndex, 100f, 1, 24, vector2_1 * 64f, true, false, location, who)
                  {
                    shakeIntensity = 0.5f,
                    shakeIntensityChange = 1f / 500f,
                    extraInfoForEndBehavior = num5,
                    endFunction = new TemporaryAnimatedSprite.endBehavior(location.removeTemporarySpritesWithID)
                  });
                  Game1.multiplayer.broadcastSprites(location, new TemporaryAnimatedSprite("LooseSprites\\Cursors", new Microsoft.Xna.Framework.Rectangle(598, 1279 /*0x04FF*/, 3, 4), 53f, 5, 9, vector2_1 * 64f + new Vector2(5f, 0.0f) * 4f, true, false, (float) (y + 7) / 10000f, 0.0f, Color.Yellow, 4f, 0.0f, 0.0f, 0.0f)
                  {
                    id = num5
                  });
                  Game1.multiplayer.broadcastSprites(location, new TemporaryAnimatedSprite("LooseSprites\\Cursors", new Microsoft.Xna.Framework.Rectangle(598, 1279 /*0x04FF*/, 3, 4), 53f, 5, 9, vector2_1 * 64f + new Vector2(5f, 0.0f) * 4f, true, true, (float) (y + 7) / 10000f, 0.0f, Color.Orange, 4f, 0.0f, 0.0f, 0.0f)
                  {
                    delayBeforeAnimationStart = 100,
                    id = num5
                  });
                  Game1.multiplayer.broadcastSprites(location, new TemporaryAnimatedSprite("LooseSprites\\Cursors", new Microsoft.Xna.Framework.Rectangle(598, 1279 /*0x04FF*/, 3, 4), 53f, 5, 9, vector2_1 * 64f + new Vector2(5f, 0.0f) * 4f, true, false, (float) (y + 7) / 10000f, 0.0f, Color.White, 3f, 0.0f, 0.0f, 0.0f)
                  {
                    delayBeforeAnimationStart = 200,
                    id = num5
                  });
                  location.netAudio.StartPlaying("fuse");
                  return true;
                }
                goto label_211;
              case '9':
                if (qualifiedItemId == "(O)419")
                {
                  TerrainFeature terrainFeature2;
                  if (!location.terrainFeatures.TryGetValue(vector2_1, out terrainFeature2) || !(terrainFeature2 is Tree tree2) || tree2.stopGrowingMoss.Value)
                    return false;
                  tree2.hasMoss.Value = false;
                  tree2.stopGrowingMoss.Value = true;
                  Game1.playSound("slosh");
                  Game1.playSound("glug");
                  Utility.makeTemporarySpriteJuicier(new TemporaryAnimatedSprite(21, tree2.Tile * 64f + new Vector2(0.0f, -64f), new Color(165, 100, (int) byte.MaxValue), animationInterval: 80f, numberOfLoops: 1, layerDepth: (float) (((double) tree2.Tile.Y + 1.25) * 64.0 / 10000.0), sourceRectHeight: 128 /*0x80*/), location);
                  return true;
                }
                goto label_211;
              default:
                goto label_211;
            }
            int num6 = this.ParentSheetIndex - 893;
            int x1 = 256 /*0x0100*/ + num6 * 16 /*0x10*/;
            foreach (TemporaryAnimatedSprite temporarySprite in location.temporarySprites)
            {
              if (temporarySprite.position.Equals(vector2_1 * 64f))
                return false;
            }
            int num7 = Game1.random.Next();
            int num8 = Game1.random.Next();
            location.playSound("thudStep");
            Game1.multiplayer.broadcastSprites(location, new TemporaryAnimatedSprite("LooseSprites\\Cursors_1_6", new Microsoft.Xna.Framework.Rectangle(x1, 397, 16 /*0x10*/, 16 /*0x10*/), 2400f, 1, 1, vector2_1 * 64f, false, false, -1f, 0.0f, Color.White, 4f, 0.0f, 0.0f, 0.0f)
            {
              shakeIntensity = 0.5f,
              shakeIntensityChange = 1f / 500f,
              extraInfoForEndBehavior = num7,
              endFunction = new TemporaryAnimatedSprite.endBehavior(location.removeTemporarySpritesWithID),
              layerDepth = (float) (((double) vector2_1.Y * 64.0 + 64.0 - 16.0) / 10000.0)
            });
            Game1.multiplayer.broadcastSprites(location, new TemporaryAnimatedSprite("LooseSprites\\Cursors_1_6", new Microsoft.Xna.Framework.Rectangle(x1, 397, 16 /*0x10*/, 16 /*0x10*/), 800f, 1, 0, vector2_1 * 64f, false, false, -1f, 0.0f, Color.White, 4f, 0.0f, 0.0f, 0.0f)
            {
              fireworkType = num6,
              delayBeforeAnimationStart = 2400,
              acceleration = new Vector2(0.0f, (float) ((double) Game1.random.Next(10) / 100.0 - 0.36000001430511475)),
              drawAboveAlwaysFront = true,
              startSound = "firework",
              shakeIntensity = 0.5f,
              shakeIntensityChange = 1f / 500f,
              extraInfoForEndBehavior = num8,
              endFunction = new TemporaryAnimatedSprite.endBehavior(location.removeTemporarySpritesWithID),
              id = Game1.random.Next(20, 31 /*0x1F*/),
              Parent = location,
              owner = who
            });
            Game1.multiplayer.broadcastSprites(location, new TemporaryAnimatedSprite("LooseSprites\\Cursors", new Microsoft.Xna.Framework.Rectangle(598, 1279 /*0x04FF*/, 3, 4), 40f, 5, 5, vector2_1 * 64f + new Vector2(11f, 12f) * 4f, true, false, (float) (y + 7) / 10000f, 0.0f, Color.Yellow, 4f, 0.0f, 0.0f, 0.0f)
            {
              id = num7
            });
            Game1.multiplayer.broadcastSprites(location, new TemporaryAnimatedSprite("LooseSprites\\Cursors", new Microsoft.Xna.Framework.Rectangle(598, 1279 /*0x04FF*/, 3, 4), 40f, 5, 5, vector2_1 * 64f + new Vector2(11f, 12f) * 4f, true, true, (float) (y + 7) / 10000f, 0.0f, Color.Orange, 4f, 0.0f, 0.0f, 0.0f)
            {
              delayBeforeAnimationStart = 100,
              id = num7
            });
            Game1.multiplayer.broadcastSprites(location, new TemporaryAnimatedSprite("LooseSprites\\Cursors", new Microsoft.Xna.Framework.Rectangle(598, 1279 /*0x04FF*/, 3, 4), 40f, 5, 5, vector2_1 * 64f + new Vector2(11f, 12f) * 4f, true, false, (float) (y + 7) / 10000f, 0.0f, Color.White, 3f, 0.0f, 0.0f, 0.0f)
            {
              delayBeforeAnimationStart = 200,
              id = num7
            });
            location.netAudio.StartPlaying("fuse");
            DelayedAction.functionAfterDelay((Action) (() => location.netAudio.StopPlaying("fuse")), 2400);
            return true;
          case 10:
            if (qualifiedItemId == "(O)TentKit")
            {
              if (location == null || !location.IsOutdoors)
              {
                Game1.showRedMessage(Game1.content.LoadString("Strings\\StringsFromCSFiles:Furniture_Outdoors_Message"));
                return false;
              }
              if (Utility.isFestivalDay((Game1.dayOfMonth + 1) % 28, Game1.dayOfMonth == 28 ? (Season) ((int) (Game1.season + 1) % 4) : Game1.season, location.GetLocationContextId()))
              {
                Game1.showRedMessage(Game1.content.LoadString("Strings\\1_6_Strings:FestivalTentWarning"));
                return false;
              }
              PassiveFestivalData data = (PassiveFestivalData) null;
              string id = (string) null;
              if (Utility.TryGetPassiveFestivalDataForDay((Game1.dayOfMonth + 1) % 28, Game1.dayOfMonth == 28 ? (Season) ((int) (Game1.season + 1) % 4) : Game1.season, (string) null, out id, out data) && data != null)
              {
                if (data.MapReplacements != null)
                {
                  foreach (string key2 in data.MapReplacements.Keys)
                  {
                    if (key2.Equals(location.Name))
                    {
                      Game1.showRedMessage(Game1.content.LoadString("Strings\\1_6_Strings:FestivalTentWarning"));
                      return false;
                    }
                  }
                }
                if ((id.Equals("TroutDerby") && location.Name.Equals("Forest") || id.Equals("SquidFest") && location.Name.Equals("Beach")) && data.StartDay > Game1.dayOfMonth)
                {
                  Game1.showRedMessage(Game1.content.LoadString("Strings\\1_6_Strings:FestivalTentWarning"));
                  return false;
                }
              }
              if (who != null)
              {
                Microsoft.Xna.Framework.Rectangle area = Microsoft.Xna.Framework.Rectangle.Empty;
                switch (Utility.getDirectionFromChange(vector2_1, who.Tile))
                {
                  case 0:
                    area = new Microsoft.Xna.Framework.Rectangle((int) ((double) vector2_1.X - 1.0), (int) ((double) vector2_1.Y - 1.0), 3, 2);
                    break;
                  case 1:
                    area = new Microsoft.Xna.Framework.Rectangle((int) vector2_1.X, (int) ((double) vector2_1.Y - 1.0), 3, 2);
                    break;
                  case 2:
                    area = new Microsoft.Xna.Framework.Rectangle((int) ((double) vector2_1.X - 1.0), (int) vector2_1.Y, 3, 2);
                    break;
                  case 3:
                    area = new Microsoft.Xna.Framework.Rectangle((int) ((double) vector2_1.X - 2.0), (int) ((double) vector2_1.Y - 1.0), 3, 2);
                    break;
                }
                if (area != Microsoft.Xna.Framework.Rectangle.Empty && location.isAreaClear(area))
                {
                  location.largeTerrainFeatures.Add((LargeTerrainFeature) new Tent(new Vector2((float) (area.X + 1), (float) (area.Y + 1))));
                  Game1.playSound("moss_cut");
                  Game1.playSound("woodyHit");
                  Microsoft.Xna.Framework.Rectangle rectangle = new Microsoft.Xna.Framework.Rectangle(area.X * 64 /*0x40*/, area.Y * 64 /*0x40*/, 192 /*0xC0*/, 128 /*0x80*/);
                  Utility.addDirtPuffs(location, area.X, area.Y, 3, 2, 9);
                  return true;
                }
                Game1.showRedMessage(Game1.content.LoadString("Strings\\1_6_Strings:Tent_Blocked"));
                return false;
              }
              break;
            }
            break;
          case 19:
            if (qualifiedItemId == "(O)BlueGrassStarter")
            {
              if (location.objects.ContainsKey(vector2_1) || location.terrainFeatures.ContainsKey(vector2_1))
                return false;
              location.terrainFeatures.Add(vector2_1, (TerrainFeature) new Grass(7, 4));
              location.playSound("dirtyHit");
              return true;
            }
            break;
        }
      }
    }
    else
    {
      if (this.IsTapper())
      {
        TerrainFeature terrainFeature;
        if (location.terrainFeatures.TryGetValue(vector2_1, out terrainFeature) && terrainFeature is Tree tree && tree.growthStage.Value >= 5 && !tree.stump.Value && !location.objects.ContainsKey(vector2_1) && (!tree.isTemporaryGreenRainTree.Value || Game1.season != Season.Summer))
        {
          WildTreeData data = tree.GetData();
          if ((data != null ? (data.CanBeTapped() ? 1 : 0) : 0) != 0)
          {
            Object one = (Object) this.getOne();
            one.heldObject.Value = (Object) null;
            one.TileLocation = vector2_1;
            location.objects.Add(vector2_1, one);
            tree.tapped.Value = true;
            tree.UpdateTapperProduct(one);
            location.playSound("axe");
            return true;
          }
        }
        return false;
      }
      if (this.HasContextTag("sign_item"))
      {
        if (location.objects.ContainsKey(vector2_1))
          return false;
        location.objects.Add(vector2_1, (Object) new Sign(vector2_1, this.ItemId));
        location.playSound("axe");
        return true;
      }
      if (this.HasContextTag("torch_item"))
      {
        if (location.objects.ContainsKey(vector2_1))
          return false;
        Torch torch = new Torch(this.ItemId, true);
        torch.shakeTimer = 25;
        torch.placementAction(location, x, y, who);
        return true;
      }
      string qualifiedItemId = this.QualifiedItemId;
      if (qualifiedItemId != null)
      {
        switch (qualifiedItemId.Length)
        {
          case 6:
            switch (qualifiedItemId[4])
            {
              case '6':
                if (qualifiedItemId == "(BC)62")
                {
                  location.objects.Add(vector2_1, (Object) new IndoorPot(vector2_1));
                  goto label_211;
                }
                goto label_211;
              case '7':
                if (qualifiedItemId == "(BC)71")
                {
                  if (location is MineShaft mineShaft)
                  {
                    if (mineShaft.shouldCreateLadderOnThisLevel() && mineShaft.recursiveTryToCreateLadderDown(vector2_1))
                    {
                      ++MineShaft.numberOfCraftedStairsUsedThisRun;
                      return true;
                    }
                    Game1.showRedMessage(Game1.content.LoadString("Strings\\StringsFromCSFiles:Object.cs.13053"));
                  }
                  else if (location.Name.Equals("ManorHouse") && x >= 1088)
                  {
                    Game1.warpFarmer("LewisBasement", 4, 4, 2);
                    Game1.playSound("stairsdown");
                    Game1.screenGlowOnce(Color.Black, true, 1f, 1f);
                    return true;
                  }
                  return false;
                }
                goto label_211;
              default:
                goto label_211;
            }
          case 7:
            switch (qualifiedItemId[6])
            {
              case '0':
                if (qualifiedItemId == "(BC)130")
                  break;
                goto label_211;
              case '1':
                if (qualifiedItemId == "(BC)211")
                {
                  if (!(this is WoodChipper woodChipper1))
                    woodChipper1 = new WoodChipper(vector2_1);
                  WoodChipper woodChipper2 = woodChipper1;
                  woodChipper2.placementAction(location, x, y, (Farmer) null);
                  location.objects.Add(vector2_1, (Object) woodChipper2);
                  location.playSound("hammer");
                  return true;
                }
                goto label_211;
              case '2':
                if (qualifiedItemId == "(BC)232")
                  break;
                goto label_211;
              case '3':
                if (qualifiedItemId == "(BC)163")
                {
                  location.objects.Add(vector2_1, (Object) new Cask(vector2_1));
                  location.playSound("hammer");
                  goto label_211;
                }
                goto label_211;
              case '4':
                switch (qualifiedItemId)
                {
                  case "(BC)214":
                    if (!(this is Phone phone1))
                      phone1 = new Phone(vector2_1);
                    Phone phone2 = phone1;
                    location.objects.Add(vector2_1, (Object) phone2);
                    location.playSound("hammer");
                    return true;
                  case "(BC)254":
                    if (!(location is AnimalHouse animalHouse) || !animalHouse.name.Value.Contains("Barn"))
                    {
                      Game1.showRedMessage(Game1.content.LoadString("Strings\\StringsFromCSFiles:MustBePlacedInBarn"));
                      return false;
                    }
                    goto label_211;
                  default:
                    goto label_211;
                }
              case '5':
                switch (qualifiedItemId)
                {
                  case "(BC)165":
                    Object @object = ItemRegistry.Create<Object>("(BC)165");
                    location.objects.Add(vector2_1, @object);
                    @object.heldObject.Value = (Object) new Chest();
                    location.playSound("axe");
                    return true;
                  case "(BC)275":
                    if (location.objects.ContainsKey(vector2_1) || location is MineShaft || location is VolcanoDungeon)
                    {
                      Game1.showRedMessage(Game1.content.LoadString("Strings\\StringsFromCSFiles:Object.cs.13053"));
                      return false;
                    }
                    Chest chest1 = new Chest(true, vector2_1, this.ItemId);
                    chest1.name = this.name;
                    chest1.shakeTimer = 50;
                    Chest chest2 = chest1;
                    chest2.lidFrameCount.Value = 2;
                    location.objects.Add(vector2_1, (Object) chest2);
                    location.playSound("axe");
                    return true;
                  default:
                    goto label_211;
                }
              case '6':
                switch (qualifiedItemId)
                {
                  case "(BC)216":
                    if (location.objects.ContainsKey(vector2_1))
                    {
                      Game1.showRedMessage(Game1.content.LoadString("Strings\\StringsFromCSFiles:Object.cs.13053"));
                      return false;
                    }
                    bool parsed;
                    if (!location.TryGetMapPropertyAs("AllowMiniFridges", out parsed))
                    {
                      if (location is FarmHouse farmHouse && farmHouse.upgradeLevel < 1)
                      {
                        Game1.showRedMessage(Game1.content.LoadString("Strings\\UI:MiniFridge_NoKitchen"));
                        return false;
                      }
                      parsed = location is FarmHouse || location is IslandFarmHouse;
                    }
                    if (!parsed)
                    {
                      Game1.showRedMessage(Game1.content.LoadString("Strings\\StringsFromCSFiles:Object.cs.13053"));
                      return false;
                    }
                    Chest chest3 = new Chest("216", vector2_1, 217, 2);
                    chest3.shakeTimer = 50;
                    Chest chest4 = chest3;
                    chest4.fridge.Value = true;
                    location.objects.Add(vector2_1, (Object) chest4);
                    location.playSound("hammer");
                    return true;
                  case "(BC)256":
                    if (location.objects.ContainsKey(vector2_1) || location is MineShaft || location is VolcanoDungeon)
                    {
                      Game1.showRedMessage(Game1.content.LoadString("Strings\\StringsFromCSFiles:Object.cs.13053"));
                      return false;
                    }
                    OverlaidDictionary objects1 = location.objects;
                    Vector2 key3 = vector2_1;
                    Chest chest5 = new Chest(true, vector2_1, this.ItemId);
                    chest5.name = this.name;
                    chest5.shakeTimer = 50;
                    objects1.Add(key3, (Object) chest5);
                    location.playSound("axe");
                    return true;
                  default:
                    goto label_211;
                }
              case '8':
                switch (qualifiedItemId)
                {
                  case "(BC)108":
                    Object one = (Object) this.getOne();
                    one.ResetParentSheetIndex();
                    Season season = location.GetSeason();
                    if (this.Location.IsOutdoors && (season == Season.Winter || season == Season.Fall))
                      one.ParentSheetIndex = 109;
                    location.Objects.Add(vector2_1, one);
                    Game1.playSound("axe");
                    return true;
                  case "(BC)208":
                    location.objects.Add(vector2_1, (Object) new Workbench(vector2_1));
                    location.playSound("axe");
                    return true;
                  case "(BC)248":
                    if (location.objects.ContainsKey(vector2_1) || location is MineShaft || location is VolcanoDungeon)
                    {
                      Game1.showRedMessage(Game1.content.LoadString("Strings\\StringsFromCSFiles:Object.cs.13053"));
                      return false;
                    }
                    OverlaidDictionary objects2 = location.objects;
                    Vector2 key4 = vector2_1;
                    Chest chest6 = new Chest(true, vector2_1, this.ItemId);
                    chest6.name = this.name;
                    chest6.shakeTimer = 50;
                    objects2.Add(key4, (Object) chest6);
                    location.playSound("axe");
                    return true;
                  case "(BC)238":
                    if (!(location is Farm))
                    {
                      Game1.showRedMessage(Game1.content.LoadString("Strings\\StringsFromCSFiles:OnlyPlaceOnFarm"));
                      return false;
                    }
                    Vector2 vector2_2 = Vector2.Zero;
                    Vector2 vector2_3 = Vector2.Zero;
                    foreach (KeyValuePair<Vector2, Object> pair in location.objects.Pairs)
                    {
                      if (pair.Value.QualifiedItemId == "(BC)238")
                      {
                        if (vector2_2.Equals(Vector2.Zero))
                          vector2_2 = pair.Key;
                        else if (vector2_3.Equals(Vector2.Zero))
                        {
                          vector2_3 = pair.Key;
                          break;
                        }
                      }
                    }
                    if (!vector2_2.Equals(Vector2.Zero) && !vector2_3.Equals(Vector2.Zero))
                    {
                      Game1.showRedMessage(Game1.content.LoadString("Strings\\StringsFromCSFiles:OnlyPlaceTwo"));
                      return false;
                    }
                    goto label_211;
                  default:
                    goto label_211;
                }
              case '9':
                if (qualifiedItemId == "(BC)209")
                {
                  if (!(this is MiniJukebox miniJukebox1))
                    miniJukebox1 = new MiniJukebox(vector2_1);
                  MiniJukebox miniJukebox2 = miniJukebox1;
                  location.objects.Add(vector2_1, (Object) miniJukebox2);
                  miniJukebox2.RegisterToLocation();
                  location.playSound("hammer");
                  return true;
                }
                goto label_211;
              default:
                goto label_211;
            }
            if (location.objects.ContainsKey(vector2_1) || location is MineShaft || location is VolcanoDungeon)
            {
              Game1.showRedMessage(Game1.content.LoadString("Strings\\StringsFromCSFiles:Object.cs.13053"));
              return false;
            }
            OverlaidDictionary objects3 = location.objects;
            Vector2 key5 = vector2_1;
            Chest chest7 = new Chest(true, vector2_1, this.ItemId);
            chest7.name = this.name;
            chest7.shakeTimer = 50;
            objects3.Add(key5, (Object) chest7);
            location.playSound(this.QualifiedItemId == "(BC)130" ? "axe" : "hammer");
            return true;
          case 12:
            if (qualifiedItemId == "(BC)BigChest")
              break;
            goto label_211;
          case 17:
            if (qualifiedItemId == "(BC)BigStoneChest")
              break;
            goto label_211;
          default:
            goto label_211;
        }
        if (location.objects.ContainsKey(vector2_1) || location is MineShaft || location is VolcanoDungeon)
        {
          Game1.showRedMessage(Game1.content.LoadString("Strings\\StringsFromCSFiles:Object.cs.13053"));
          return false;
        }
        Chest chest8 = new Chest(true, vector2_1, this.ItemId);
        chest8.shakeTimer = 50;
        Chest chest9 = chest8;
        location.objects.Add(vector2_1, (Object) chest9);
        location.playSound(this.QualifiedItemId == "(BC)BigChest" ? "axe" : "hammer");
        return true;
      }
    }
label_211:
    TerrainFeature terrainFeature3;
    if (this.Category == -19 && location.terrainFeatures.TryGetValue(vector2_1, out terrainFeature3) && terrainFeature3 is HoeDirt hoeDirt1 && hoeDirt1.crop != null && (this.QualifiedItemId == "(O)369" || this.QualifiedItemId == "(O)368") && hoeDirt1.crop.currentPhase.Value != 0)
    {
      Game1.showRedMessage(Game1.content.LoadString("Strings\\StringsFromCSFiles:HoeDirt.cs.13916"));
      return false;
    }
    if (this.isSapling())
    {
      if (this.IsWildTreeSapling() || this.IsFruitTreeSapling())
      {
        if (FruitTree.IsTooCloseToAnotherTree(new Vector2((float) (x / 64 /*0x40*/), (float) (y / 64 /*0x40*/)), location))
        {
          Game1.showRedMessage(Game1.content.LoadString("Strings\\StringsFromCSFiles:Object.cs.13060"));
          return false;
        }
        if (FruitTree.IsGrowthBlocked(new Vector2((float) (x / 64 /*0x40*/), (float) (y / 64 /*0x40*/)), location))
        {
          Game1.showRedMessage(Game1.content.LoadString("Strings\\UI:FruitTree_PlacementWarning", (object) this.DisplayName));
          return false;
        }
      }
      TerrainFeature terrainFeature4;
      if (location.terrainFeatures.TryGetValue(vector2_1, out terrainFeature4))
      {
        if (!(terrainFeature4 is HoeDirt hoeDirt2) || hoeDirt2.crop != null)
          return false;
        location.terrainFeatures.Remove(vector2_1);
      }
      string deniedMessage = (string) null;
      bool flag1 = location.doesTileHaveProperty((int) vector2_1.X, (int) vector2_1.Y, "Diggable", "Back") != null;
      string str = location.doesTileHaveProperty((int) vector2_1.X, (int) vector2_1.Y, "Type", "Back");
      bool flag2 = location.doesEitherTileOrTileIndexPropertyEqual((int) vector2_1.X, (int) vector2_1.Y, "CanPlantTrees", "Back", "T");
      if (location is Farm && ((flag1 || str == "Grass" ? 1 : (str == "Dirt" ? 1 : 0)) | (flag2 ? 1 : 0)) != 0 && !location.IsNoSpawnTile(vector2_1, "Tree") | flag2 || (flag1 || str == "Stone") && location.CanPlantTreesHere(this.ItemId, (int) vector2_1.X, (int) vector2_1.Y, out deniedMessage))
      {
        location.playSound("dirtyHit");
        DelayedAction.playSoundAfterDelay("coin", 100);
        if (this.IsTeaSapling())
        {
          location.terrainFeatures.Add(vector2_1, (TerrainFeature) new Bush(vector2_1, 3, location));
          return true;
        }
        FruitTree fruitTree = new FruitTree(this.ItemId)
        {
          GreenHouseTileTree = location.IsGreenhouse && str == "Stone"
        };
        fruitTree.growthRate.Value = Math.Max(1, this.Quality + 1);
        location.terrainFeatures.Add(vector2_1, (TerrainFeature) fruitTree);
        return true;
      }
      if (deniedMessage == null)
        deniedMessage = Game1.content.LoadString("Strings\\StringsFromCSFiles:Object.cs.13068");
      Game1.showRedMessage(deniedMessage);
      return false;
    }
    if (this.Category == -74 || this.Category == -19)
    {
      TerrainFeature terrainFeature5;
      if (location.terrainFeatures.TryGetValue(vector2_1, out terrainFeature5))
      {
        HoeDirt dirt = terrainFeature5 as HoeDirt;
        if (dirt != null)
        {
          string itemId = Crop.ResolveSeedId(who.ActiveObject.ItemId, location);
          if (!dirt.canPlantThisSeedHere(itemId, who.ActiveObject.Category == -19) || !dirt.plant(itemId, who, who.ActiveObject.Category == -19) || !who.IsLocalPlayer)
            return false;
          if (this.Category == -74)
          {
            foreach (Object @object in location.Objects.Values)
            {
              if (@object.IsSprinkler() && @object.heldObject.Value != null && @object.heldObject.Value.QualifiedItemId == "(O)913" && @object.IsInSprinklerRangeBroadphase(vector2_1))
              {
                if (@object.GetSprinklerTiles().Contains(vector2_1))
                {
                  Chest chest = @object.heldObject.Value.heldObject.Value as Chest;
                  if (chest != null)
                  {
                    IInventory items = (IInventory) chest.Items;
                    if (items.Count > 0 && items[0] != null && !chest.GetMutex().IsLocked())
                    {
                      chest.GetMutex().RequestLock((Action) (() =>
                      {
                        if (items.Count > 0 && items[0] != null)
                        {
                          Item obj = items[0];
                          if (obj.Category == -19 && dirt.plant(obj.ItemId, who, true))
                            items[0] = obj.ConsumeStack(1);
                        }
                        chest.GetMutex().ReleaseLock();
                      }));
                      break;
                    }
                  }
                }
              }
            }
          }
          Game1.haltAfterCheck = false;
          return true;
        }
      }
      return false;
    }
    if (!this.performDropDownAction(who))
    {
      Object object1 = (Object) this.getOne();
      bool flag = false;
      if (object1.GetType() == typeof (Furniture) && Furniture.GetFurnitureInstance(this.ItemId, new Vector2?(new Vector2((float) (x / 64 /*0x40*/), (float) (y / 64 /*0x40*/)))).GetType() != object1.GetType())
      {
        StorageFurniture storageFurniture = new StorageFurniture(this.ItemId, new Vector2((float) (x / 64 /*0x40*/), (float) (y / 64 /*0x40*/)));
        storageFurniture.currentRotation.Value = (this as Furniture).currentRotation.Value;
        storageFurniture.updateRotation();
        object1 = (Object) storageFurniture;
        flag = true;
      }
      object1.shakeTimer = 50;
      object1.Location = location;
      object1.TileLocation = vector2_1;
      object1.performDropDownAction(who);
      if (this.IsTextSign())
      {
        object1.signText.Value = (string) null;
        object1.showNextIndex.Value = object1.QualifiedItemId == "(BC)TextSign";
      }
      if (object1.name.Contains("Seasonal"))
      {
        int num9 = object1.ParentSheetIndex - object1.ParentSheetIndex % 4;
        object1.ParentSheetIndex = num9 + location.GetSeasonIndex();
      }
      Object object2;
      if (!(object1 is Furniture) && location.objects.TryGetValue(vector2_1, out object2))
      {
        if (object2.QualifiedItemId != this.QualifiedItemId)
        {
          Game1.createItemDebris((Item) object2, vector2_1 * 64f, Game1.random.Next(4));
          location.objects[vector2_1] = object1;
        }
      }
      else if (object1 is Furniture furniture)
      {
        if (flag)
          location.furniture.Add(furniture);
        else
          location.furniture.Add(this as Furniture);
      }
      else
        location.objects.Add(vector2_1, object1);
      object1.initializeLightSource(vector2_1);
    }
    location.playSound("woodyStep");
    return true;
  }

  /// <inheritdoc />
  protected override void MigrateLegacyItemId()
  {
    if (this.bigCraftable.Value)
    {
      IDictionary<string, BigCraftableData> bigCraftableData1 = Game1.bigCraftableData;
      int num = this.ParentSheetIndex;
      string key1 = num.ToString();
      if (!bigCraftableData1.ContainsKey(key1))
      {
        if (this.ParentSheetIndex >= 56 && this.ParentSheetIndex <= 61)
        {
          this.ItemId = "56";
          return;
        }
        if (this.ParentSheetIndex >= 101 && this.ParentSheetIndex <= 103)
        {
          this.SetIdAndSprite(101);
          return;
        }
        if (this.name.Contains("Seasonal"))
        {
          num = this.ParentSheetIndex - this.ParentSheetIndex % 4;
          this.ItemId = num.ToString();
          return;
        }
        IDictionary<string, BigCraftableData> bigCraftableData2 = Game1.bigCraftableData;
        num = this.ParentSheetIndex - 1;
        string key2 = num.ToString();
        if (bigCraftableData2.ContainsKey(key2))
        {
          num = this.ParentSheetIndex - 1;
          this.ItemId = num.ToString();
          return;
        }
      }
    }
    base.MigrateLegacyItemId();
  }

  /// <inheritdoc />
  public override bool actionWhenPurchased(string shopId)
  {
    if (this.QualifiedItemId == "(O)434")
    {
      if (!Game1.isFestival())
        Game1.player.mailReceived.Add("CF_Sewer");
      else
        Game1.player.mailReceived.Add("CF_Fair");
      Game1.exitActiveMenu();
      Game1.player.eatObject(this, true);
    }
    return base.actionWhenPurchased(shopId) || this.isRecipe.Value;
  }

  public virtual bool needsToBeDonated()
  {
    return LibraryMuseum.IsItemSuitableForDonation(this.QualifiedItemId);
  }

  public override string getDescription()
  {
    if (this.Category == -102 && Game1.player.stats.Get(this.itemId.Value) > 0U && this.ItemId != "Book_PriceCatalogue" && this.ItemId != "Book_AnimalCatalogue")
    {
      foreach (string contextTag in this.GetContextTags())
      {
        if (contextTag.StartsWithIgnoreCase("book_xp_"))
        {
          string name = contextTag.Split('_')[2];
          return Game1.parseText(Game1.content.LoadString("Strings\\1_6_Strings:alreadyreadbook", (object) Farmer.getSkillDisplayNameFromIndex(Farmer.getSkillNumberFromName(name)).ToLower()), Game1.smallFont, this.getDescriptionWidth());
        }
      }
      return Game1.parseText(Game1.content.LoadString("Strings\\1_6_Strings:alreadyreadbook_random"), Game1.smallFont, this.getDescriptionWidth());
    }
    if (this.isRecipe.Value)
      return this.Category == -7 ? Game1.content.LoadString("Strings\\StringsFromCSFiles:Object.cs.13073", (object) this.loadDisplayName()) : Game1.content.LoadString("Strings\\StringsFromCSFiles:Object.cs.13074", (object) this.loadDisplayName());
    if (this.needsToBeDonated())
      return Game1.parseText(Game1.content.LoadString("Strings\\StringsFromCSFiles:Object.cs.13078"), Game1.smallFont, this.getDescriptionWidth());
    string str = ItemRegistry.GetDataOrErrorItem(this.QualifiedItemId).Description;
    string preservedItemId = this.GetPreservedItemId();
    if (preservedItemId != null)
    {
      ParsedItemData dataOrErrorItem = ItemRegistry.GetDataOrErrorItem(preservedItemId);
      str = string.Format(str, (object) dataOrErrorItem.DisplayName, (object) dataOrErrorItem.DisplayName.ToLower());
    }
    return Game1.parseText(str, Game1.smallFont, this.getDescriptionWidth());
  }

  /// <summary>Auto-generate a default light source ID for this item when placed.</summary>
  /// <param name="equippedBy">The object's tile position.</param>
  public virtual string GenerateLightSourceId(Vector2 position)
  {
    if (this.Location == null)
      return $"{this.GetType().Name}_Held_{Game1.random.Next()}";
    return $"{this.GetType().Name}_{this.Location.NameOrUniqueName}_{position.X}_{position.Y}";
  }

  /// <inheritdoc />
  public override int sellToStorePrice(long specificPlayerID = -1)
  {
    if (this is Fence)
      return this.price.Value;
    if (this.Category == -22)
      return (int) ((double) this.price.Value * (1.0 + (double) this.quality.Value * 0.25) * (((double) (FishingRod.maxTackleUses - this.uses.Value) + 0.0) / (double) FishingRod.maxTackleUses));
    float storePrice = this.getPriceAfterMultipliers((float) (int) ((double) this.price.Value * (1.0 + (double) this.Quality * 0.25)), specificPlayerID);
    if (this.QualifiedItemId == "(O)493")
      storePrice /= 2f;
    if ((double) storePrice > 0.0)
      storePrice = Math.Max(1f, storePrice * Game1.MasterPlayer.difficultyModifier);
    return (int) storePrice;
  }

  /// <inheritdoc />
  public override int salePrice(bool ignoreProfitMargins = false)
  {
    if (this is Fence)
      return this.price.Value;
    if (this.isRecipe.Value)
      return this.price.Value * 10;
    switch (this.QualifiedItemId)
    {
      case "(O)388":
        return Game1.year <= 1 ? 10 : 50;
      case "(O)390":
        return Game1.year <= 1 ? 20 : 100;
      case "(O)382":
        return Game1.year <= 1 ? 120 : 250;
      case "(O)378":
        return Game1.year <= 1 ? 80 /*0x50*/ : 160 /*0xA0*/;
      case "(O)380":
        return Game1.year <= 1 ? 150 : 250;
      case "(O)384":
        return Game1.year <= 1 ? 350 : 750;
      default:
        float num = (float) (int) ((double) (this.price.Value * 2) * (1.0 + (double) this.quality.Value * 0.25));
        if (!ignoreProfitMargins && this.appliesProfitMargins())
          num = (float) (int) Math.Max(1f, num * Game1.MasterPlayer.difficultyModifier);
        return (int) num;
    }
  }

  /// <inheritdoc />
  public override bool appliesProfitMargins()
  {
    return this.category.Value == -74 || this.isSapling() || base.appliesProfitMargins();
  }

  protected virtual float getPriceAfterMultipliers(float startPrice, long specificPlayerID = -1)
  {
    string lower = this.name.ToLower();
    bool flag = lower.Contains("mayonnaise") || lower.Contains("cheese") || lower.Contains("cloth") || lower.Contains("wool");
    float val1 = 1f;
    foreach (Farmer allFarmer in Game1.getAllFarmers())
    {
      if (Game1.player.useSeparateWallets)
      {
        if (specificPlayerID == -1L)
        {
          if (allFarmer.UniqueMultiplayerID != Game1.player.UniqueMultiplayerID || !allFarmer.isActive())
            continue;
        }
        else if (allFarmer.UniqueMultiplayerID != specificPlayerID)
          continue;
      }
      else if (!allFarmer.isActive())
        continue;
      float val2 = 1f;
      if (allFarmer.professions.Contains(0) && (flag || this.Category == -5 || this.Category == -6 || this.Category == -18))
        val2 *= 1.2f;
      if (allFarmer.professions.Contains(1) && (this.Category == -75 || this.Category == -80 || this.Category == -79 && !this.isSpawnedObject.Value))
        val2 *= 1.1f;
      if (allFarmer.professions.Contains(4) && this.Category == -26)
        val2 *= 1.4f;
      if (allFarmer.professions.Contains(6))
      {
        if (this.Category != -4)
        {
          if ((NetFieldBase<Object.PreserveType?, NetNullableEnum<Object.PreserveType>>) this.preserve != (NetNullableEnum<Object.PreserveType>) null)
          {
            Object.PreserveType? nullable = this.preserve.Value;
            if (nullable.HasValue)
            {
              nullable = this.preserve.Value;
              if (nullable.GetValueOrDefault() != Object.PreserveType.SmokedFish)
                goto label_19;
            }
            else
              goto label_19;
          }
          else
            goto label_19;
        }
        val2 *= allFarmer.professions.Contains(8) ? 1.5f : 1.25f;
      }
label_19:
      if (allFarmer.professions.Contains(15) && this.Category == -27)
        val2 *= 1.25f;
      if (allFarmer.professions.Contains(20) && this.IsBar())
        val2 *= 1.5f;
      if (allFarmer.professions.Contains(23) && (this.Category == -2 || this.Category == -12))
        val2 *= 1.3f;
      if (allFarmer.eventsSeen.Contains("2120303") && (this.QualifiedItemId == "(O)296" || this.QualifiedItemId == "(O)410"))
        val2 *= 3f;
      if (allFarmer.eventsSeen.Contains("3910979") && this.QualifiedItemId == "(O)399")
        val2 *= 5f;
      if (allFarmer.stats.Get("Book_Artifact") > 0U && this.Type != null && this.Type.Equals("Arch"))
        val2 *= 3f;
      val1 = Math.Max(val1, val2);
    }
    return startPrice * val1;
  }

  /// <inheritdoc />
  public override bool ForEachItem(ForEachItemDelegate handler, GetForEachItemPathDelegate getPath)
  {
    return base.ForEachItem(handler, getPath) && ForEachItemHelper.ApplyToField<Object>(this.heldObject, handler, getPath);
  }

  public enum PreserveType
  {
    Wine,
    Jelly,
    Pickle,
    Juice,
    Roe,
    AgedRoe,
    Honey,
    Bait,
    DriedFruit,
    DriedMushroom,
    SmokedFish,
  }
}
