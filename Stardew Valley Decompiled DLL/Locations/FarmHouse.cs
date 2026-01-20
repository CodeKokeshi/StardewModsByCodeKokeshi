// Decompiled with JetBrains decompiler
// Type: StardewValley.Locations.FarmHouse
// Assembly: Stardew Valley, Version=1.6.15.24356, Culture=neutral, PublicKeyToken=null
// MVID: AA1F513A-94F0-4EF6-A35F-2D5B4A3721BF
// Assembly location: D:\SteamLibrary\steamapps\common\Stardew Valley\Stardew Valley.dll
// XML documentation location: D:\SteamLibrary\steamapps\common\Stardew Valley\Stardew Valley.xml

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Netcode;
using StardewValley.BellsAndWhistles;
using StardewValley.Characters;
using StardewValley.Extensions;
using StardewValley.GameData.Characters;
using StardewValley.Monsters;
using StardewValley.Objects;
using StardewValley.Pathfinding;
using StardewValley.TerrainFeatures;
using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Xml.Serialization;
using xTile;
using xTile.Dimensions;
using xTile.Layers;
using xTile.ObjectModel;
using xTile.Tiles;

#nullable disable
namespace StardewValley.Locations;

public class FarmHouse : DecoratableLocation
{
  [XmlElement("fridge")]
  public readonly NetRef<Chest> fridge;
  [XmlIgnore]
  public readonly NetInt synchronizedDisplayedLevel;
  /// <summary>The tile position containing the map fridge, or <see cref="P:Microsoft.Xna.Framework.Point.Zero" /> if not found.</summary>
  public Point fridgePosition;
  [XmlIgnore]
  public Point spouseRoomSpot;
  private string lastSpouseRoom;
  [XmlIgnore]
  private LocalizedContentManager mapLoader;
  public List<Warp> cellarWarps;
  [XmlElement("cribStyle")]
  public readonly NetInt cribStyle;
  [XmlIgnore]
  public int previousUpgradeLevel;
  private int currentlyDisplayedUpgradeLevel;
  private bool displayingSpouseRoom;
  private Color nightLightingColor;
  private Color rainLightingColor;

  /// <summary>The player who owns this home.</summary>
  [XmlIgnore]
  public virtual Farmer owner => Game1.MasterPlayer;

  /// <summary>Whether the home has an assigned player, regardless of whether they've finished creating their character..</summary>
  /// <remarks>See also <see cref="P:StardewValley.Locations.FarmHouse.IsOwnerActivated" />.</remarks>
  [XmlIgnore]
  [MemberNotNullWhen(true, "owner")]
  public virtual bool HasOwner
  {
    [MemberNotNullWhen(true, "owner")] get => this.owner != null;
  }

  /// <summary>The unique ID of the player who owns this home, if any.</summary>
  public virtual long OwnerId
  {
    get
    {
      Farmer owner = this.owner;
      return owner == null ? 0L : owner.UniqueMultiplayerID;
    }
  }

  /// <summary>Whether the home has an assigned player and they've finished creating their character.</summary>
  /// <remarks>See also <see cref="P:StardewValley.Locations.FarmHouse.HasOwner" />.</remarks>
  [MemberNotNullWhen(true, "owner")]
  public bool IsOwnerActivated
  {
    [MemberNotNullWhen(true, "owner")] get
    {
      Farmer owner = this.owner;
      return owner != null && owner.isActive();
    }
  }

  /// <summary>Whether the home is owned by the current player.</summary>
  [MemberNotNullWhen(true, "owner")]
  public bool IsOwnedByCurrentPlayer
  {
    [MemberNotNullWhen(true, "owner")] get
    {
      long? uniqueMultiplayerId1 = this.owner?.UniqueMultiplayerID;
      long uniqueMultiplayerId2 = Game1.player.UniqueMultiplayerID;
      return uniqueMultiplayerId1.GetValueOrDefault() == uniqueMultiplayerId2 & uniqueMultiplayerId1.HasValue;
    }
  }

  [XmlIgnore]
  public virtual int upgradeLevel
  {
    get
    {
      Farmer owner = this.owner;
      return owner == null ? 0 : owner.HouseUpgradeLevel;
    }
    set
    {
      if (!this.HasOwner)
        return;
      this.owner.houseUpgradeLevel.Value = value;
    }
  }

  public FarmHouse()
  {
    NetInt netInt = new NetInt(1);
    netInt.InterpolationEnabled = false;
    this.cribStyle = netInt;
    this.previousUpgradeLevel = -1;
    this.nightLightingColor = new Color(180, 180, 0);
    this.rainLightingColor = new Color(90, 90, 0);
    // ISSUE: explicit constructor call
    base.\u002Ector();
    this.fridge.Value.Location = (GameLocation) this;
  }

  public FarmHouse(string m, string name)
  {
    NetInt netInt = new NetInt(1);
    netInt.InterpolationEnabled = false;
    this.cribStyle = netInt;
    this.previousUpgradeLevel = -1;
    this.nightLightingColor = new Color(180, 180, 0);
    this.rainLightingColor = new Color(90, 90, 0);
    // ISSUE: explicit constructor call
    base.\u002Ector(m, name);
    this.fridge.Value.Location = (GameLocation) this;
    this.ReadWallpaperAndFloorTileData();
    Farm farm = Game1.getFarm();
    this.AddStarterGiftBox(farm);
    this.AddStarterFurniture(farm);
    this.SetStarterFlooring(farm);
    this.SetStarterWallpaper(farm);
  }

  /// <summary>Place the starter gift box when the farmhouse is first created.</summary>
  /// <param name="farm">The farm instance to which a farmhouse is being added.</param>
  private void AddStarterGiftBox(Farm farm)
  {
    Chest chest = new Chest((List<Item>) null, Vector2.Zero, true, giftboxIsStarterGift: true);
    string[] propertySplitBySpaces = farm.GetMapPropertySplitBySpaces("FarmHouseStarterGift");
    for (int index = 0; index < propertySplitBySpaces.Length; index += 2)
    {
      string itemId;
      string error;
      int amount;
      if (!ArgUtility.TryGet(propertySplitBySpaces, index, out itemId, out error, false, "string giftId") || !ArgUtility.TryGetOptionalInt(propertySplitBySpaces, index + 1, out amount, out error, name: "int count"))
        farm.LogMapPropertyError("FarmHouseStarterGift", propertySplitBySpaces, error);
      else
        chest.Items.Add(ItemRegistry.Create(itemId, amount));
    }
    if (!chest.Items.Any<Item>())
    {
      Item obj = ItemRegistry.Create("(O)472", 15);
      chest.Items.Add(obj);
    }
    Vector2 parsed;
    if (!farm.TryGetMapPropertyAs("FarmHouseStarterSeedsPosition", out parsed))
    {
      switch (Game1.whichFarm)
      {
        case 1:
        case 2:
        case 4:
          parsed = new Vector2(4f, 7f);
          break;
        case 3:
          parsed = new Vector2(2f, 9f);
          break;
        case 6:
          parsed = new Vector2(8f, 6f);
          break;
        default:
          parsed = new Vector2(3f, 7f);
          break;
      }
    }
    this.objects.Add(parsed, (StardewValley.Object) chest);
  }

  /// <summary>Place the starter furniture when the farmhouse is first created.</summary>
  /// <param name="farm">The farm instance to which a farmhouse is being added.</param>
  private void AddStarterFurniture(Farm farm)
  {
    this.furniture.Add((Furniture) new BedFurniture(BedFurniture.DEFAULT_BED_INDEX, new Vector2(9f, 8f)));
    string[] propertySplitBySpaces = farm.GetMapPropertySplitBySpaces("FarmHouseFurniture");
    if (((IEnumerable<string>) propertySplitBySpaces).Any<string>())
    {
      for (int index1 = 0; index1 < propertySplitBySpaces.Length; index1 += 4)
      {
        int num1;
        string error;
        Vector2 vector2;
        int num2;
        if (!ArgUtility.TryGetInt(propertySplitBySpaces, index1, out num1, out error, "int index") || !ArgUtility.TryGetVector2(propertySplitBySpaces, index1 + 1, out vector2, out error, name: "Vector2 tile") || !ArgUtility.TryGetInt(propertySplitBySpaces, index1 + 3, out num2, out error, "int rotations"))
        {
          farm.LogMapPropertyError("FarmHouseFurniture", propertySplitBySpaces, error);
        }
        else
        {
          Furniture furniture = ItemRegistry.Create<Furniture>("(F)" + num1.ToString());
          furniture.InitializeAtTile(vector2);
          furniture.isOn.Value = true;
          for (int index2 = 0; index2 < num2; ++index2)
            furniture.rotate();
          Furniture furnitureAt = this.GetFurnitureAt(vector2);
          if (furnitureAt != null)
            furnitureAt.heldObject.Value = (StardewValley.Object) furniture;
          else
            this.furniture.Add(furniture);
        }
      }
    }
    else
    {
      switch (Game1.whichFarm)
      {
        case 0:
          this.furniture.Add(ItemRegistry.Create<Furniture>("(F)1120").SetPlacement(5, 4).SetHeldObject((StardewValley.Object) ItemRegistry.Create<Furniture>("(F)1364")));
          this.furniture.Add(ItemRegistry.Create<Furniture>("(F)1376").SetPlacement(1, 10));
          this.furniture.Add(ItemRegistry.Create<Furniture>("(F)0").SetPlacement(4, 4));
          this.furniture.Add(ItemRegistry.Create<Furniture>("(F)1466").SetPlacement(1, 4));
          this.furniture.Add(ItemRegistry.Create<Furniture>("(F)1614").SetPlacement(3, 1));
          this.furniture.Add(ItemRegistry.Create<Furniture>("(F)1618").SetPlacement(6, 8));
          this.furniture.Add(ItemRegistry.Create<Furniture>("(F)1602").SetPlacement(5, 1));
          this.furniture.Add(ItemRegistry.Create<Furniture>("(F)1792").SetPlacement(this.getFireplacePoint()));
          break;
        case 1:
          this.furniture.Add(ItemRegistry.Create<Furniture>("(F)1122").SetPlacement(1, 6).SetHeldObject((StardewValley.Object) ItemRegistry.Create<Furniture>("(F)1367")));
          this.furniture.Add(ItemRegistry.Create<Furniture>("(F)3").SetPlacement(1, 5));
          this.furniture.Add(ItemRegistry.Create<Furniture>("(F)1680").SetPlacement(5, 4));
          this.furniture.Add(ItemRegistry.Create<Furniture>("(F)1673").SetPlacement(1, 1));
          this.furniture.Add(ItemRegistry.Create<Furniture>("(F)1673").SetPlacement(3, 1));
          this.furniture.Add(ItemRegistry.Create<Furniture>("(F)1676").SetPlacement(5, 1));
          this.furniture.Add(ItemRegistry.Create<Furniture>("(F)1737").SetPlacement(6, 8));
          this.furniture.Add(ItemRegistry.Create<Furniture>("(F)1742").SetPlacement(5, 5));
          this.furniture.Add(ItemRegistry.Create<Furniture>("(F)1675").SetPlacement(10, 1));
          this.furniture.Add(ItemRegistry.Create<Furniture>("(F)1792").SetPlacement(this.getFireplacePoint()));
          this.objects.Add(new Vector2(4f, 4f), ItemRegistry.Create<StardewValley.Object>("(BC)FishSmoker"));
          break;
        case 2:
          this.furniture.Add(ItemRegistry.Create<Furniture>("(F)1134").SetPlacement(1, 7).SetHeldObject((StardewValley.Object) ItemRegistry.Create<Furniture>("(F)1748")));
          this.furniture.Add(ItemRegistry.Create<Furniture>("(F)3").SetPlacement(1, 6));
          this.furniture.Add(ItemRegistry.Create<Furniture>("(F)1680").SetPlacement(6, 4));
          this.furniture.Add(ItemRegistry.Create<Furniture>("(F)1296").SetPlacement(1, 4));
          this.furniture.Add(ItemRegistry.Create<Furniture>("(F)1682").SetPlacement(3, 1));
          this.furniture.Add(ItemRegistry.Create<Furniture>("(F)1777").SetPlacement(6, 5));
          this.furniture.Add(ItemRegistry.Create<Furniture>("(F)1745").SetPlacement(6, 1));
          this.furniture.Add(ItemRegistry.Create<Furniture>("(F)1747").SetPlacement(5, 4));
          this.furniture.Add(ItemRegistry.Create<Furniture>("(F)1296").SetPlacement(10, 4));
          this.furniture.Add(ItemRegistry.Create<Furniture>("(F)1792").SetPlacement(this.getFireplacePoint()));
          break;
        case 3:
          this.furniture.Add(ItemRegistry.Create<Furniture>("(F)1218").SetPlacement(1, 6).SetHeldObject((StardewValley.Object) ItemRegistry.Create<Furniture>("(F)1368")));
          this.furniture.Add(ItemRegistry.Create<Furniture>("(F)1755").SetPlacement(1, 5));
          this.furniture.Add(ItemRegistry.Create<Furniture>("(F)1755").SetPlacement(3, 6, 1));
          this.furniture.Add(ItemRegistry.Create<Furniture>("(F)1680").SetPlacement(5, 4));
          this.furniture.Add(ItemRegistry.Create<Furniture>("(F)1751").SetPlacement(5, 10));
          this.furniture.Add(ItemRegistry.Create<Furniture>("(F)1749").SetPlacement(3, 1));
          this.furniture.Add(ItemRegistry.Create<Furniture>("(F)1753").SetPlacement(5, 1));
          this.furniture.Add(ItemRegistry.Create<Furniture>("(F)1742").SetPlacement(5, 5));
          this.furniture.Add(ItemRegistry.Create<Furniture>("(F)1794").SetPlacement(this.getFireplacePoint()));
          break;
        case 4:
          this.furniture.Add(ItemRegistry.Create<Furniture>("(F)1680").SetPlacement(1, 4));
          this.furniture.Add(ItemRegistry.Create<Furniture>("(F)1628").SetPlacement(1, 5));
          this.furniture.Add(ItemRegistry.Create<Furniture>("(F)1393").SetPlacement(3, 4).SetHeldObject((StardewValley.Object) ItemRegistry.Create<Furniture>("(F)1369")));
          this.furniture.Add(ItemRegistry.Create<Furniture>("(F)1678").SetPlacement(10, 1));
          this.furniture.Add(ItemRegistry.Create<Furniture>("(F)1812").SetPlacement(3, 1));
          this.furniture.Add(ItemRegistry.Create<Furniture>("(F)1630").SetPlacement(1, 1));
          this.furniture.Add(ItemRegistry.Create<Furniture>("(F)1811").SetPlacement(6, 1));
          this.furniture.Add(ItemRegistry.Create<Furniture>("(F)1389").SetPlacement(10, 4));
          this.furniture.Add(ItemRegistry.Create<Furniture>("(F)1758").SetPlacement(1, 10));
          this.furniture.Add(ItemRegistry.Create<Furniture>("(F)1794").SetPlacement(this.getFireplacePoint()));
          break;
        case 5:
          this.furniture.Add(ItemRegistry.Create<Furniture>("(F)1466").SetPlacement(1, 4));
          this.furniture.Add(ItemRegistry.Create<Furniture>("(F)1614").SetPlacement(3, 1));
          this.furniture.Add(ItemRegistry.Create<Furniture>("(F)1614").SetPlacement(6, 1));
          this.furniture.Add(ItemRegistry.Create<Furniture>("(F)1601").SetPlacement(10, 1));
          this.furniture.Add(ItemRegistry.Create<Furniture>("(F)202").SetPlacement(3, 4, 1));
          this.furniture.Add(ItemRegistry.Create<Furniture>("(F)1124").SetPlacement(4, 4, 1).SetHeldObject((StardewValley.Object) ItemRegistry.Create<Furniture>("(F)1379")));
          this.furniture.Add(ItemRegistry.Create<Furniture>("(F)202").SetPlacement(6, 4, 3));
          this.furniture.Add(ItemRegistry.Create<Furniture>("(F)1378").SetPlacement(10, 4));
          this.furniture.Add(ItemRegistry.Create<Furniture>("(F)1377").SetPlacement(1, 9));
          this.furniture.Add(ItemRegistry.Create<Furniture>("(F)1445").SetPlacement(1, 10));
          this.furniture.Add(ItemRegistry.Create<Furniture>("(F)1618").SetPlacement(2, 9));
          this.furniture.Add(ItemRegistry.Create<Furniture>("(F)1792").SetPlacement(this.getFireplacePoint()));
          break;
        case 6:
          this.furniture.Add(ItemRegistry.Create<Furniture>("(F)1680").SetPlacement(4, 4));
          this.furniture.Add(ItemRegistry.Create<Furniture>("(F)1614").SetPlacement(7, 1));
          this.furniture.Add(ItemRegistry.Create<Furniture>("(F)1294").SetPlacement(3, 4));
          this.furniture.Add(ItemRegistry.Create<Furniture>("(F)1283").SetPlacement(1, 4));
          this.furniture.Add(ItemRegistry.Create<Furniture>("(F)1614").SetPlacement(8, 1));
          this.furniture.Add(ItemRegistry.Create<Furniture>("(F)202").SetPlacement(7, 4));
          this.furniture.Add(ItemRegistry.Create<Furniture>("(F)1294").SetPlacement(10, 4));
          this.furniture.Add(ItemRegistry.Create<Furniture>("(F)6").SetPlacement(2, 6, 1));
          this.furniture.Add(ItemRegistry.Create<Furniture>("(F)6").SetPlacement(5, 7, 3));
          this.furniture.Add(ItemRegistry.Create<Furniture>("(F)1124").SetPlacement(3, 6).SetHeldObject((StardewValley.Object) ItemRegistry.Create<Furniture>("(F)1362")));
          this.furniture.Add(ItemRegistry.Create<Furniture>("(F)1228").SetPlacement(2, 9));
          break;
      }
    }
  }

  /// <summary>Gets the initial flooring type for a farmhouse being created, if any.</summary>
  /// <param name="farm">The farm instance to which a farmhouse is being added.</param>
  public static string GetStarterFlooring(Farm farm)
  {
    string mapProperty = farm?.getMapProperty("FarmHouseFlooring");
    if (mapProperty != null)
      return mapProperty;
    string starterFlooring;
    switch (Game1.whichFarm)
    {
      case 1:
        starterFlooring = "1";
        break;
      case 2:
        starterFlooring = "34";
        break;
      case 3:
        starterFlooring = "18";
        break;
      case 4:
        starterFlooring = "4";
        break;
      case 5:
        starterFlooring = "5";
        break;
      case 6:
        starterFlooring = "35";
        break;
      default:
        starterFlooring = (string) null;
        break;
    }
    return starterFlooring;
  }

  /// <summary>Gets the initial wallpaper type for a farmhouse being created, if any.</summary>
  /// <param name="farm">The farm instance to which a farmhouse is being added.</param>
  public static string GetStarterWallpaper(Farm farm)
  {
    string mapProperty = farm?.getMapProperty("FarmHouseWallpaper");
    if (mapProperty != null)
      return mapProperty;
    string starterWallpaper;
    switch (Game1.whichFarm)
    {
      case 1:
        starterWallpaper = "11";
        break;
      case 2:
        starterWallpaper = "92";
        break;
      case 3:
        starterWallpaper = "12";
        break;
      case 4:
        starterWallpaper = "95";
        break;
      case 5:
        starterWallpaper = "65";
        break;
      case 6:
        starterWallpaper = "106";
        break;
      default:
        starterWallpaper = (string) null;
        break;
    }
    return starterWallpaper;
  }

  /// <summary>Set the initial flooring when the farmhouse is first created, if any.</summary>
  /// <param name="farm">The farm instance to which a farmhouse is being added.</param>
  /// <param name="styleToOverride">Unused.</param>
  private void SetStarterFlooring(Farm farm, string styleToOverride = null)
  {
    string starterFlooring = FarmHouse.GetStarterFlooring(farm);
    if (starterFlooring == null)
      return;
    this.SetFloor(starterFlooring, (string) null);
  }

  /// <summary>Set the initial wallpaper when the farmhouse is first created, if any.</summary>
  /// <param name="farm">The farm instance to which a farmhouse is being added.</param>
  /// <param name="styleToOverride">Unused.</param>
  private void SetStarterWallpaper(Farm farm, string styleToOverride = null)
  {
    string starterWallpaper = FarmHouse.GetStarterWallpaper(farm);
    if (starterWallpaper == null)
      return;
    this.SetWallpaper(starterWallpaper, (string) null);
  }

  protected override void initNetFields()
  {
    base.initNetFields();
    this.NetFields.AddField((INetSerializable) this.fridge, "fridge").AddField((INetSerializable) this.cribStyle, "cribStyle").AddField((INetSerializable) this.synchronizedDisplayedLevel, "synchronizedDisplayedLevel");
    this.cribStyle.fieldChangeVisibleEvent += (FieldChange<NetInt, int>) ((field, old_value, new_value) =>
    {
      if (this.map == null)
        return;
      if (this._appliedMapOverrides != null && this._appliedMapOverrides.Contains("crib"))
        this._appliedMapOverrides.Remove("crib");
      this.UpdateChildRoom();
      this.ReadWallpaperAndFloorTileData();
      this.setWallpapers();
      this.setFloors();
    });
    this.fridge.fieldChangeEvent += (FieldChange<NetRef<Chest>, Chest>) ((field, oldValue, newValue) => newValue.Location = (GameLocation) this);
  }

  public List<Child> getChildren() => this.characters.OfType<Child>().ToList<Child>();

  public int getChildrenCount()
  {
    int childrenCount = 0;
    foreach (NPC character in this.characters)
    {
      if (character is Child)
        ++childrenCount;
    }
    return childrenCount;
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
    return base.isCollidingPosition(position, viewport, isFarmer, damagesFarmer, glider, character, pathfinding);
  }

  public override void performTenMinuteUpdate(int timeOfDay)
  {
    base.performTenMinuteUpdate(timeOfDay);
    foreach (NPC character in this.characters)
    {
      if (character.isMarried())
      {
        if (character.getSpouse() == Game1.player)
          character.checkForMarriageDialogue(timeOfDay, (GameLocation) this);
        if (Game1.IsMasterGame && Game1.timeOfDay >= 2200 && Game1.IsMasterGame && character.TilePoint != this.getSpouseBedSpot(character.Name) && (timeOfDay == 2200 || character.controller == null && timeOfDay % 100 % 30 == 0))
        {
          Point spouseBedSpot = this.getSpouseBedSpot(character.Name);
          character.controller = (PathFindController) null;
          PathFindController.endBehavior endBehaviorFunction = (PathFindController.endBehavior) null;
          bool flag = this.GetSpouseBed() != null;
          if (flag)
            endBehaviorFunction = new PathFindController.endBehavior(FarmHouse.spouseSleepEndFunction);
          character.controller = new PathFindController((Character) character, (GameLocation) this, spouseBedSpot, 0, endBehaviorFunction);
          if (character.controller.pathToEndPoint == null || !this.isTileOnMap(character.controller.pathToEndPoint.Last<Point>()))
            character.controller = (PathFindController) null;
          else if (flag)
          {
            foreach (Furniture furniture in this.furniture)
            {
              if (furniture is BedFurniture bedFurniture && bedFurniture.GetBoundingBox().Intersects(new Microsoft.Xna.Framework.Rectangle(spouseBedSpot.X * 64 /*0x40*/, spouseBedSpot.Y * 64 /*0x40*/, 64 /*0x40*/, 64 /*0x40*/)))
              {
                bedFurniture.ReserveForNPC();
                break;
              }
            }
          }
        }
      }
      if (character is Child child)
        child.tenMinuteUpdate();
    }
  }

  public static void spouseSleepEndFunction(Character c, GameLocation location)
  {
    if (!(c is NPC npc))
      return;
    if (DataLoader.AnimationDescriptions(Game1.content).ContainsKey(npc.name.Value.ToLower() + "_sleep"))
      npc.playSleepingAnimation();
    Microsoft.Xna.Framework.Rectangle boundingBox = npc.GetBoundingBox();
    foreach (Furniture furniture in location.furniture)
    {
      if (furniture is BedFurniture bedFurniture && bedFurniture.GetBoundingBox().Intersects(boundingBox))
      {
        bedFurniture.ReserveForNPC();
        break;
      }
    }
    if (Game1.random.NextDouble() >= 0.1)
      return;
    if (Game1.random.NextDouble() < 0.8)
      npc.showTextAboveHead(Game1.content.LoadString("Strings\\1_6_Strings:Spouse_Goodnight0", (object) npc.getTermOfSpousalEndearment(Game1.random.NextDouble() < 0.1)));
    else
      npc.showTextAboveHead(Game1.content.LoadString("Strings\\1_6_Strings:Spouse_Goodnight1"));
  }

  public virtual Point getFrontDoorSpot()
  {
    foreach (Warp warp in (NetList<Warp, NetRef<Warp>>) this.warps)
    {
      if (warp.TargetName == "Farm")
      {
        if (this is Cabin)
          return new Point(warp.TargetX, warp.TargetY);
        return warp.TargetX == 64 /*0x40*/ && warp.TargetY == 15 ? Game1.getFarm().GetMainFarmHouseEntry() : new Point(warp.TargetX, warp.TargetY);
      }
    }
    return Game1.getFarm().GetMainFarmHouseEntry();
  }

  public virtual Point getPorchStandingSpot()
  {
    Point mainFarmHouseEntry = Game1.getFarm().GetMainFarmHouseEntry();
    mainFarmHouseEntry.X += 2;
    return mainFarmHouseEntry;
  }

  public Point getKitchenStandingSpot()
  {
    Point parsed;
    if (this.TryGetMapPropertyAs("KitchenStandingLocation", out parsed))
      return parsed;
    switch (this.upgradeLevel)
    {
      case 1:
        return new Point(4, 5);
      case 2:
      case 3:
        return new Point(22, 24);
      default:
        return new Point(-1000, -1000);
    }
  }

  public virtual BedFurniture GetSpouseBed()
  {
    if (this.HasOwner)
    {
      if (this.owner.getSpouse()?.Name == "Krobus")
        return (BedFurniture) null;
      if (this.owner.hasCurrentOrPendingRoommate() && this.GetBed(BedFurniture.BedType.Single) != null)
        return this.GetBed(BedFurniture.BedType.Single);
    }
    return this.GetBed(BedFurniture.BedType.Double);
  }

  public Point getSpouseBedSpot(string spouseName)
  {
    if (spouseName == "Krobus")
    {
      NPC characterFromName = Game1.getCharacterFromName(this.name.Value);
      if ((characterFromName != null ? (characterFromName.isRoommate() ? 1 : 0) : 0) != 0)
        goto label_3;
    }
    if (this.GetSpouseBed() != null)
    {
      BedFurniture spouseBed = this.GetSpouseBed();
      Point bedSpot = this.GetSpouseBed().GetBedSpot();
      if (spouseBed.bedType == BedFurniture.BedType.Double)
        ++bedSpot.X;
      return bedSpot;
    }
label_3:
    return this.GetSpouseRoomSpot();
  }

  public Point GetSpouseRoomSpot()
  {
    return this.upgradeLevel == 0 ? new Point(-1000, -1000) : this.spouseRoomSpot;
  }

  public BedFurniture GetBed(BedFurniture.BedType bed_type = BedFurniture.BedType.Any, int index = 0)
  {
    foreach (Furniture furniture in this.furniture)
    {
      if (furniture is BedFurniture bed && (bed_type == BedFurniture.BedType.Any || bed.bedType == bed_type))
      {
        if (index == 0)
          return bed;
        --index;
      }
    }
    return (BedFurniture) null;
  }

  public Point GetPlayerBedSpot()
  {
    BedFurniture playerBed = this.GetPlayerBed();
    return playerBed != null ? playerBed.GetBedSpot() : this.getEntryLocation();
  }

  public BedFurniture GetPlayerBed()
  {
    return this.upgradeLevel == 0 ? this.GetBed(BedFurniture.BedType.Single) : this.GetBed(BedFurniture.BedType.Double);
  }

  public Point getBedSpot(BedFurniture.BedType bed_type = BedFurniture.BedType.Any)
  {
    BedFurniture bed = this.GetBed(bed_type);
    return bed != null ? bed.GetBedSpot() : new Point(-1000, -1000);
  }

  public Point getEntryLocation()
  {
    Point parsed;
    if (this.TryGetMapPropertyAs("EntryLocation", out parsed))
      return parsed;
    switch (this.upgradeLevel)
    {
      case 0:
        return new Point(3, 11);
      case 1:
        return new Point(9, 11);
      case 2:
      case 3:
        return new Point(27, 30);
      default:
        return new Point(-1000, -1000);
    }
  }

  public BedFurniture GetChildBed(int index) => this.GetBed(BedFurniture.BedType.Child, index);

  public Point GetChildBedSpot(int index)
  {
    BedFurniture childBed = this.GetChildBed(index);
    return childBed != null ? childBed.GetBedSpot() : Point.Zero;
  }

  public override bool isTilePlaceable(Vector2 v, bool itemIsPassable = false)
  {
    return (!this.isTileOnMap(v) || this.getTileIndexAt((int) v.X, (int) v.Y, "Back", "indoor") != 0) && base.isTilePlaceable(v, itemIsPassable);
  }

  public Point getRandomOpenPointInHouse(Random r, int buffer = 0, int tries = 30)
  {
    for (int index = 0; index < tries; ++index)
    {
      Point openPointInHouse = new Point(r.Next(this.map.Layers[0].LayerWidth), r.Next(this.map.Layers[0].LayerHeight));
      Microsoft.Xna.Framework.Rectangle rect = new Microsoft.Xna.Framework.Rectangle(openPointInHouse.X - buffer, openPointInHouse.Y - buffer, 1 + buffer * 2, 1 + buffer * 2);
      bool flag = false;
      foreach (Point point in rect.GetPoints())
      {
        int x = point.X;
        int y = point.Y;
        flag = !this.hasTileAt(x, y, "Back") || !this.CanItemBePlacedHere(new Vector2((float) x, (float) y)) || this.isTileOnWall(x, y);
        if (this.getTileIndexAt(x, y, "Back", "indoor") == 0)
          flag = true;
        if (flag)
          break;
      }
      if (!flag)
        return openPointInHouse;
    }
    return Point.Zero;
  }

  public override bool checkAction(Location tileLocation, xTile.Dimensions.Rectangle viewport, Farmer who)
  {
    if (this.getTileIndexAt(tileLocation, "Buildings", "untitled tile sheet") == 173)
    {
      this.fridge.Value.fridge.Value = true;
      this.fridge.Value.checkForAction(who, false);
      return true;
    }
    if (this.getTileIndexAt(tileLocation, "Buildings", "indoor") != 2173)
      return base.checkAction(tileLocation, viewport, who);
    if (Game1.player.eventsSeen.Contains("463391") && Game1.player.spouse == "Emily" && this.getTemporarySpriteByID(5858585) is EmilysParrot temporarySpriteById)
      temporarySpriteById.doAction();
    return true;
  }

  public override void updateEvenIfFarmerIsntHere(GameTime time, bool ignoreWasUpdatedFlush = false)
  {
    base.updateEvenIfFarmerIsntHere(time, ignoreWasUpdatedFlush);
    if (!this.HasOwner || !Game1.IsMasterGame)
      return;
    foreach (NPC character in this.characters)
    {
      long? uniqueMultiplayerId = character.getSpouse()?.UniqueMultiplayerID;
      long ownerId = this.OwnerId;
      if (uniqueMultiplayerId.GetValueOrDefault() == ownerId & uniqueMultiplayerId.HasValue && Game1.timeOfDay < 1500 && Game1.random.NextDouble() < 0.0006 && character.controller == null && character.Schedule == null && character.TilePoint != this.getSpouseBedSpot(Game1.player.spouse) && this.furniture.Count > 0)
      {
        Furniture furniture = this.furniture[Game1.random.Next(this.furniture.Count)];
        Microsoft.Xna.Framework.Rectangle rectangle = furniture.boundingBox.Value;
        Vector2 tile = new Vector2((float) (rectangle.X / 64 /*0x40*/), (float) (rectangle.Y / 64 /*0x40*/));
        if (furniture.furniture_type.Value != 15 && furniture.furniture_type.Value != 12)
        {
          int num1 = 0;
          int finalFacingDirection = -3;
          for (; num1 < 3; ++num1)
          {
            int num2 = Game1.random.Next(-1, 2);
            int num3 = Game1.random.Next(-1, 2);
            tile.X += (float) num2;
            if (num2 == 0)
              tile.Y += (float) num3;
            if (num2 != -1)
            {
              if (num2 == 1)
              {
                finalFacingDirection = 3;
              }
              else
              {
                switch (num3)
                {
                  case -1:
                    finalFacingDirection = 2;
                    break;
                  case 1:
                    finalFacingDirection = 0;
                    break;
                }
              }
            }
            else
              finalFacingDirection = 1;
            if (this.CanItemBePlacedHere(tile))
              break;
          }
          if (num1 < 3)
            character.controller = new PathFindController((Character) character, (GameLocation) this, new Point((int) tile.X, (int) tile.Y), finalFacingDirection, false);
        }
      }
    }
  }

  public override void UpdateWhenCurrentLocation(GameTime time)
  {
    if (this.wasUpdated)
      return;
    base.UpdateWhenCurrentLocation(time);
    this.fridge.Value.updateWhenCurrentLocation(time);
    if (!Game1.player.isMarriedOrRoommates() || Game1.player.spouse == null)
      return;
    NPC characterFromName = this.getCharacterFromName(Game1.player.spouse);
    if (characterFromName == null || characterFromName.isEmoting)
      return;
    Vector2 tile = characterFromName.Tile;
    foreach (Vector2 adjacentTilesOffset in Character.AdjacentTilesOffsets)
    {
      Vector2 tileLocation = tile + adjacentTilesOffset;
      if (this.isCharacterAtTile(tileLocation) is Monster monster)
      {
        Microsoft.Xna.Framework.Rectangle boundingBox = monster.GetBoundingBox();
        Point center = boundingBox.Center;
        characterFromName.faceGeneralDirection(tileLocation * new Vector2(64f, 64f));
        Game1.showSwordswipeAnimation(characterFromName.FacingDirection, characterFromName.Position, 60f, false);
        this.localSound("swordswipe");
        characterFromName.shake(500);
        characterFromName.showTextAboveHead(Game1.content.LoadString("Strings\\Locations:FarmHouse_SpouseAttacked" + (Game1.random.Next(12) + 1).ToString()));
        monster.takeDamage(50, (int) Utility.getAwayFromPositionTrajectory(boundingBox, characterFromName.Position).X, (int) Utility.getAwayFromPositionTrajectory(boundingBox, characterFromName.Position).Y, false, 1.0, Game1.player);
        if (monster.Health <= 0)
        {
          this.debris.Add(new Debris(monster.Sprite.textureName.Value, Game1.random.Next(6, 16 /*0x10*/), Utility.PointToVector2(center)));
          this.monsterDrop(monster, center.X, center.Y, this.owner);
          this.characters.Remove((NPC) monster);
          ++Game1.stats.MonstersKilled;
          Game1.player.changeFriendship(-10, characterFromName);
        }
        else
          monster.shedChunks(4);
        characterFromName.CurrentDialogue.Clear();
        characterFromName.CurrentDialogue.Push(characterFromName.TryGetDialogue("Spouse_MonstersInHouse") ?? new Dialogue(characterFromName, "Data\\ExtraDialogue:Spouse_MonstersInHouse"));
      }
    }
  }

  public Point getFireplacePoint()
  {
    switch (this.upgradeLevel)
    {
      case 0:
        return new Point(8, 4);
      case 1:
        return new Point(26, 4);
      case 2:
      case 3:
        return new Point(17, 23);
      default:
        return new Point(-50, -50);
    }
  }

  /// <summary>Get whether the player who owns this home is married to or roommates with an NPC.</summary>
  public bool HasNpcSpouseOrRoommate()
  {
    return this.owner?.spouse != null && this.owner.isMarriedOrRoommates();
  }

  /// <summary>Get whether the player who owns this home is married to or roommates with the given NPC.</summary>
  /// <param name="spouseName">The NPC name.</param>
  public bool HasNpcSpouseOrRoommate(string spouseName)
  {
    return spouseName != null && this.owner?.spouse == spouseName && this.owner.isMarriedOrRoommates();
  }

  public virtual void showSpouseRoom()
  {
    bool flag = this.HasNpcSpouseOrRoommate();
    int num = this.displayingSpouseRoom ? 1 : 0;
    this.displayingSpouseRoom = flag;
    this.updateMap();
    if (num != 0 && !this.displayingSpouseRoom)
    {
      Point spouseRoomCorner = this.GetSpouseRoomCorner();
      Microsoft.Xna.Framework.Rectangle rectangle1 = CharacterSpouseRoomData.DefaultMapSourceRect;
      CharacterData data;
      if (NPC.TryGetData(this.owner.spouse, out data))
      {
        CharacterSpouseRoomData spouseRoom = data.SpouseRoom;
        rectangle1 = spouseRoom != null ? spouseRoom.MapSourceRect : rectangle1;
      }
      Microsoft.Xna.Framework.Rectangle rectangle2 = new Microsoft.Xna.Framework.Rectangle(spouseRoomCorner.X, spouseRoomCorner.Y, rectangle1.Width, rectangle1.Height);
      --rectangle2.X;
      List<Item> overflow_items = new List<Item>();
      Microsoft.Xna.Framework.Rectangle rectangle3 = new Microsoft.Xna.Framework.Rectangle(rectangle2.X * 64 /*0x40*/, rectangle2.Y * 64 /*0x40*/, rectangle2.Width * 64 /*0x40*/, rectangle2.Height * 64 /*0x40*/);
      foreach (Furniture furniture in new List<Furniture>((IEnumerable<Furniture>) this.furniture))
      {
        if (furniture.GetBoundingBox().Intersects(rectangle3))
        {
          if (furniture is StorageFurniture storageFurniture)
          {
            overflow_items.AddRange((IEnumerable<Item>) storageFurniture.heldItems);
            storageFurniture.heldItems.Clear();
          }
          if (furniture.heldObject.Value != null)
          {
            overflow_items.Add((Item) furniture.heldObject.Value);
            furniture.heldObject.Value = (StardewValley.Object) null;
          }
          overflow_items.Add((Item) furniture);
          this.furniture.Remove(furniture);
        }
      }
      for (int x = rectangle2.X; x <= rectangle2.Right; ++x)
      {
        for (int y = rectangle2.Y; y <= rectangle2.Bottom; ++y)
        {
          StardewValley.Object @object = this.getObjectAtTile(x, y);
          switch (@object)
          {
            case null:
            case Furniture _:
              continue;
            default:
              @object.performRemoveAction();
              switch (@object)
              {
                case Fence fence:
                  @object = new StardewValley.Object(fence.ItemId, 1);
                  break;
                case IndoorPot indoorPot:
                  if (indoorPot.hoeDirt.Value?.crop != null)
                  {
                    indoorPot.hoeDirt.Value.destroyCrop(false);
                    break;
                  }
                  break;
                case Chest chest:
                  overflow_items.AddRange((IEnumerable<Item>) chest.Items);
                  chest.Items.Clear();
                  break;
              }
              @object.heldObject.Value = (StardewValley.Object) null;
              @object.minutesUntilReady.Value = -1;
              @object.readyForHarvest.Value = false;
              overflow_items.Add((Item) @object);
              this.objects.Remove(new Vector2((float) x, (float) y));
              continue;
          }
        }
      }
      if (this.upgradeLevel >= 2)
        Utility.createOverflowChest((GameLocation) this, new Vector2(39f, 32f), overflow_items);
      else
        Utility.createOverflowChest((GameLocation) this, new Vector2(21f, 10f), overflow_items);
    }
    this.loadObjects();
    if (this.upgradeLevel == 3)
    {
      this.AddCellarTiles();
      this.createCellarWarps();
      Game1.player.craftingRecipes.TryAdd("Cask", 0);
    }
    if (flag)
      this.loadSpouseRoom();
    this.lastSpouseRoom = this.owner?.spouse;
  }

  public virtual void AddCellarTiles()
  {
    if (this._appliedMapOverrides.Contains("cellar"))
      this._appliedMapOverrides.Remove("cellar");
    this.ApplyMapOverride("FarmHouse_Cellar", "cellar");
  }

  /// <summary>Get the cellar location linked to this cabin, or <c>null</c> if there is none.</summary>
  public Cellar GetCellar()
  {
    string cellarName = this.GetCellarName();
    return cellarName == null ? (Cellar) null : Game1.RequireLocation<Cellar>(cellarName);
  }

  /// <summary>Get the name of the cellar location linked to this cabin, or <c>null</c> if there is none.</summary>
  public string GetCellarName()
  {
    int num = -1;
    if (this.HasOwner)
    {
      foreach (int key in Game1.player.team.cellarAssignments.Keys)
      {
        if (Game1.player.team.cellarAssignments[key] == this.OwnerId)
          num = key;
      }
    }
    switch (num)
    {
      case -1:
        return (string) null;
      case 0:
      case 1:
        return "Cellar";
      default:
        return "Cellar" + num.ToString();
    }
  }

  protected override void resetSharedState()
  {
    base.resetSharedState();
    if (this.HasOwner)
    {
      if (Game1.timeOfDay >= 2200 && this.owner.spouse != null && this.getCharacterFromName(this.owner.spouse) != null && !this.owner.isEngaged())
        Game1.player.team.requestSpouseSleepEvent.Fire(this.owner.UniqueMultiplayerID);
      if (Game1.timeOfDay >= 2000 && this.IsOwnedByCurrentPlayer && Game1.getFarm().farmers.Count <= 1)
        Game1.player.team.requestPetWarpHomeEvent.Fire(this.owner.UniqueMultiplayerID);
    }
    if (!Game1.IsMasterGame)
      return;
    Farm farm = Game1.getFarm();
    for (int index = this.characters.Count - 1; index >= 0; --index)
    {
      if (this.characters[index] is Pet character)
      {
        Point tilePoint = character.TilePoint;
        Microsoft.Xna.Framework.Rectangle boundingBox = character.GetBoundingBox();
        if (!this.isTileOnMap(tilePoint.X, tilePoint.Y) || this.hasTileAt(boundingBox.Left / 64 /*0x40*/, tilePoint.Y, "Buildings") || this.hasTileAt(boundingBox.Right / 64 /*0x40*/, tilePoint.Y, "Buildings"))
        {
          character.WarpToPetBowl();
          break;
        }
      }
    }
    for (int index1 = this.characters.Count - 1; index1 >= 0; --index1)
    {
      for (int index2 = index1 - 1; index2 >= 0; --index2)
      {
        if (index1 < this.characters.Count && index2 < this.characters.Count && (this.characters[index2].Equals((object) this.characters[index1]) || this.characters[index2].Name.Equals(this.characters[index1].Name) && this.characters[index2].IsVillager && this.characters[index1].IsVillager) && index2 != index1)
          this.characters.RemoveAt(index2);
      }
      for (int index3 = farm.characters.Count - 1; index3 >= 0; --index3)
      {
        if (index1 < this.characters.Count && index3 < this.characters.Count && farm.characters[index3].Equals((object) this.characters[index1]))
          farm.characters.RemoveAt(index3);
      }
    }
  }

  public void UpdateForRenovation()
  {
    this.updateFarmLayout();
    this.setWallpapers();
    this.setFloors();
  }

  public void updateFarmLayout()
  {
    if (this.currentlyDisplayedUpgradeLevel != this.upgradeLevel)
      this.setMapForUpgradeLevel(this.upgradeLevel);
    this._ApplyRenovations();
    if (this.displayingSpouseRoom != this.HasNpcSpouseOrRoommate() || this.lastSpouseRoom != this.owner?.spouse)
      this.showSpouseRoom();
    this.UpdateChildRoom();
    this.ReadWallpaperAndFloorTileData();
  }

  protected virtual void _ApplyRenovations()
  {
    bool hasOwner = this.HasOwner;
    if (this.upgradeLevel >= 2)
    {
      if (this._appliedMapOverrides.Contains("bedroom_open"))
        this._appliedMapOverrides.Remove("bedroom_open");
      if (hasOwner && this.owner.mailReceived.Contains("renovation_bedroom_open"))
        this.ApplyMapOverride("FarmHouse_Bedroom_Open", "bedroom_open");
      else
        this.ApplyMapOverride("FarmHouse_Bedroom_Normal", "bedroom_open");
      if (this._appliedMapOverrides.Contains("southernroom_open"))
        this._appliedMapOverrides.Remove("southernroom_open");
      if (hasOwner && this.owner.mailReceived.Contains("renovation_southern_open"))
        this.ApplyMapOverride("FarmHouse_SouthernRoom_Add", "southernroom_open");
      else
        this.ApplyMapOverride("FarmHouse_SouthernRoom_Remove", "southernroom_open");
      if (this._appliedMapOverrides.Contains("cornerroom_open"))
        this._appliedMapOverrides.Remove("cornerroom_open");
      if (hasOwner && this.owner.mailReceived.Contains("renovation_corner_open"))
      {
        this.ApplyMapOverride("FarmHouse_CornerRoom_Add", "cornerroom_open");
        if (this.displayingSpouseRoom)
          this.setMapTile(49, 19, 229, "Front", "untitled tile sheet");
      }
      else
      {
        this.ApplyMapOverride("FarmHouse_CornerRoom_Remove", "cornerroom_open");
        if (this.displayingSpouseRoom)
          this.setMapTile(49, 19, 87, "Front", "untitled tile sheet");
      }
      if (this._appliedMapOverrides.Contains("diningroom_open"))
        this._appliedMapOverrides.Remove("diningroom_open");
      if (hasOwner && this.owner.mailReceived.Contains("renovation_dining_open"))
        this.ApplyMapOverride("FarmHouse_DiningRoom_Add", "diningroom_open");
      else
        this.ApplyMapOverride("FarmHouse_DiningRoom_Remove", "diningroom_open");
      if (this._appliedMapOverrides.Contains("cubby_open"))
        this._appliedMapOverrides.Remove("cubby_open");
      if (hasOwner && this.owner.mailReceived.Contains("renovation_cubby_open"))
        this.ApplyMapOverride("FarmHouse_Cubby_Add", "cubby_open");
      else
        this.ApplyMapOverride("FarmHouse_Cubby_Remove", "cubby_open");
      if (this._appliedMapOverrides.Contains("farupperroom_open"))
        this._appliedMapOverrides.Remove("farupperroom_open");
      if (hasOwner && this.owner.mailReceived.Contains("renovation_farupperroom_open"))
        this.ApplyMapOverride("FarmHouse_FarUpperRoom_Add", "farupperroom_open");
      else
        this.ApplyMapOverride("FarmHouse_FarUpperRoom_Remove", "farupperroom_open");
      if (this._appliedMapOverrides.Contains("extendedcorner_open"))
        this._appliedMapOverrides.Remove("extendedcorner_open");
      if (hasOwner && this.owner.mailReceived.Contains("renovation_extendedcorner_open"))
        this.ApplyMapOverride("FarmHouse_ExtendedCornerRoom_Add", "extendedcorner_open");
      else if (hasOwner && this.owner.mailReceived.Contains("renovation_corner_open"))
        this.ApplyMapOverride("FarmHouse_ExtendedCornerRoom_Remove", "extendedcorner_open");
      if (this._appliedMapOverrides.Contains("diningroomwall_open"))
        this._appliedMapOverrides.Remove("diningroomwall_open");
      if (hasOwner && this.owner.mailReceived.Contains("renovation_diningroomwall_open"))
        this.ApplyMapOverride("FarmHouse_DiningRoomWall_Add", "diningroomwall_open");
      else if (hasOwner && this.owner.mailReceived.Contains("renovation_dining_open"))
        this.ApplyMapOverride("FarmHouse_DiningRoomWall_Remove", "diningroomwall_open");
    }
    string propertyValue;
    if (!this.TryGetMapProperty("AdditionalRenovations", out propertyValue))
      return;
    foreach (string str1 in propertyValue.Split(','))
    {
      string[] strArray = ArgUtility.SplitBySpace(str1);
      if (strArray.Length >= 4)
      {
        string override_key_name = strArray[0];
        string str2 = strArray[1];
        string map_name1 = strArray[2];
        string map_name2 = strArray[3];
        Microsoft.Xna.Framework.Rectangle? destination_rect = new Microsoft.Xna.Framework.Rectangle?();
        if (strArray.Length >= 8)
        {
          try
          {
            destination_rect = new Microsoft.Xna.Framework.Rectangle?(new Microsoft.Xna.Framework.Rectangle()
            {
              X = int.Parse(strArray[4]),
              Y = int.Parse(strArray[5]),
              Width = int.Parse(strArray[6]),
              Height = int.Parse(strArray[7])
            });
          }
          catch (Exception ex)
          {
            destination_rect = new Microsoft.Xna.Framework.Rectangle?();
          }
        }
        if (this._appliedMapOverrides.Contains(override_key_name))
          this._appliedMapOverrides.Remove(override_key_name);
        if (hasOwner && this.owner.mailReceived.Contains(str2))
          this.ApplyMapOverride(map_name1, override_key_name, destination_rect: destination_rect);
        else
          this.ApplyMapOverride(map_name2, override_key_name, destination_rect: destination_rect);
      }
    }
  }

  public override void MakeMapModifications(bool force = false)
  {
    base.MakeMapModifications(force);
    this.updateFarmLayout();
    this.setWallpapers();
    this.setFloors();
    if (!this.HasNpcSpouseOrRoommate("Sebastian") || !Game1.netWorldState.Value.hasWorldStateID("sebastianFrog"))
      return;
    Point spouseRoomCorner = this.GetSpouseRoomCorner();
    ++spouseRoomCorner.X;
    spouseRoomCorner.Y += 6;
    Vector2 vector2 = Utility.PointToVector2(spouseRoomCorner);
    this.removeTile((int) vector2.X, (int) vector2.Y - 1, "Front");
    this.removeTile((int) vector2.X + 1, (int) vector2.Y - 1, "Front");
    this.removeTile((int) vector2.X + 2, (int) vector2.Y - 1, "Front");
  }

  protected override void resetLocalState()
  {
    base.resetLocalState();
    if (this.HasNpcSpouseOrRoommate("Emily") && Game1.player.eventsSeen.Contains("463391"))
    {
      Vector2 location = new Vector2(2064f, 160f);
      switch (this.upgradeLevel)
      {
        case 2:
        case 3:
          location = new Vector2(3408f, 1376f);
          break;
      }
      this.temporarySprites.Add((TemporaryAnimatedSprite) new EmilysParrot(location));
    }
    if (Game1.player.currentLocation == null || !Game1.player.currentLocation.Equals((GameLocation) this) && !Game1.player.currentLocation.name.Value.StartsWith("Cellar"))
    {
      Game1.player.Position = Utility.PointToVector2(this.getEntryLocation()) * 64f;
      Game1.xLocationAfterWarp = Game1.player.TilePoint.X;
      Game1.yLocationAfterWarp = Game1.player.TilePoint.Y;
      Game1.player.currentLocation = (GameLocation) this;
    }
    foreach (NPC character in this.characters)
    {
      if (character is Child child)
        child.resetForPlayerEntry((GameLocation) this);
      if (Game1.IsMasterGame && Game1.timeOfDay >= 2000 && !(character is Pet))
      {
        character.controller = (PathFindController) null;
        character.Halt();
      }
    }
    if (this.IsOwnedByCurrentPlayer && Game1.player.team.GetSpouse(Game1.player.UniqueMultiplayerID).HasValue && Game1.player.team.IsMarried(Game1.player.UniqueMultiplayerID) && !Game1.player.mailReceived.Contains("CF_Spouse"))
    {
      Vector2 vector2 = Utility.PointToVector2(this.getEntryLocation()) + new Vector2(0.0f, -1f);
      Chest chest = new Chest(new List<Item>()
      {
        ItemRegistry.Create("(O)434")
      }, vector2, true, 1);
      this.overlayObjects[vector2] = (StardewValley.Object) chest;
    }
    if (this.IsOwnedByCurrentPlayer && !Game1.player.activeDialogueEvents.ContainsKey("pennyRedecorating"))
    {
      int whichStyle = -1;
      if (Game1.player.mailReceived.Contains("pennyQuilt0"))
        whichStyle = 0;
      else if (Game1.player.mailReceived.Contains("pennyQuilt1"))
        whichStyle = 1;
      else if (Game1.player.mailReceived.Contains("pennyQuilt2"))
        whichStyle = 2;
      if (whichStyle != -1 && !Game1.player.mailReceived.Contains("pennyRefurbished"))
      {
        List<StardewValley.Object> objectsToStoreInChests = new List<StardewValley.Object>();
        foreach (Furniture furniture in this.furniture)
        {
          if (furniture is BedFurniture bedFurniture && bedFurniture.bedType == BedFurniture.BedType.Double)
          {
            string itemId = (string) null;
            if (this.owner.mailReceived.Contains("pennyQuilt0"))
              itemId = "2058";
            if (this.owner.mailReceived.Contains("pennyQuilt1"))
              itemId = "2064";
            if (this.owner.mailReceived.Contains("pennyQuilt2"))
              itemId = "2070";
            if (itemId != null)
            {
              Vector2 tileLocation = bedFurniture.TileLocation;
              bedFurniture.performRemoveAction();
              objectsToStoreInChests.Add((StardewValley.Object) bedFurniture);
              this.furniture.Remove(this.furniture.GuidOf((Furniture) bedFurniture));
              this.furniture.Add((Furniture) new BedFurniture(itemId, new Vector2(tileLocation.X, tileLocation.Y)));
              break;
            }
            break;
          }
        }
        Game1.player.mailReceived.Add("pennyRefurbished");
        Microsoft.Xna.Framework.Rectangle rectangle = this.upgradeLevel >= 2 ? new Microsoft.Xna.Framework.Rectangle(38, 20, 11, 13) : new Microsoft.Xna.Framework.Rectangle(20, 1, 8, 10);
        for (int x = rectangle.X; x <= rectangle.Right; ++x)
        {
          for (int y = rectangle.Y; y <= rectangle.Bottom; ++y)
          {
            if (this.getObjectAtTile(x, y) != null)
            {
              StardewValley.Object object1 = this.getObjectAtTile(x, y);
              switch (object1)
              {
                case null:
                case Chest _:
                case StorageFurniture _:
                case IndoorPot _:
                case BedFurniture _:
                  continue;
                default:
                  if (object1.heldObject.Value != null)
                  {
                    bool? nullable = object1 is Furniture furniture ? new bool?(furniture.IsTable()) : new bool?();
                    if (nullable.HasValue && nullable.GetValueOrDefault())
                    {
                      StardewValley.Object object2 = object1.heldObject.Value;
                      object1.heldObject.Value = (StardewValley.Object) null;
                      objectsToStoreInChests.Add(object2);
                    }
                  }
                  object1.performRemoveAction();
                  if (object1 is Fence fence)
                    object1 = new StardewValley.Object(fence.ItemId, 1);
                  objectsToStoreInChests.Add(object1);
                  this.objects.Remove(new Vector2((float) x, (float) y));
                  if (object1 is Furniture furniture1)
                  {
                    this.furniture.Remove(furniture1);
                    continue;
                  }
                  continue;
              }
            }
          }
        }
        this.decoratePennyRoom(whichStyle, objectsToStoreInChests);
      }
    }
    if (!this.HasNpcSpouseOrRoommate("Sebastian") || !Game1.netWorldState.Value.hasWorldStateID("sebastianFrog"))
      return;
    Point spouseRoomCorner = this.GetSpouseRoomCorner();
    ++spouseRoomCorner.X;
    spouseRoomCorner.Y += 6;
    Vector2 vector2_1 = Utility.PointToVector2(spouseRoomCorner);
    this.temporarySprites.Add(new TemporaryAnimatedSprite()
    {
      texture = Game1.mouseCursors,
      sourceRect = new Microsoft.Xna.Framework.Rectangle(641, 1534, 48 /*0x30*/, 37),
      animationLength = 1,
      sourceRectStartingPos = new Vector2(641f, 1534f),
      interval = 5000f,
      totalNumberOfLoops = 9999,
      position = vector2_1 * 64f + new Vector2(0.0f, -5f) * 4f,
      scale = 4f,
      layerDepth = (float) (((double) vector2_1.Y + 2.0 + 0.10000000149011612) * 64.0 / 10000.0)
    });
    if (Game1.random.NextDouble() < 0.85)
    {
      Texture2D texture2D = Game1.temporaryContent.Load<Texture2D>("TileSheets\\critters");
      TemporaryAnimatedSpriteList temporarySprites = this.TemporarySprites;
      SebsFrogs sebsFrogs = new SebsFrogs();
      sebsFrogs.texture = texture2D;
      sebsFrogs.sourceRect = new Microsoft.Xna.Framework.Rectangle(64 /*0x40*/, 224 /*0xE0*/, 16 /*0x10*/, 16 /*0x10*/);
      sebsFrogs.animationLength = 1;
      sebsFrogs.sourceRectStartingPos = new Vector2(64f, 224f);
      sebsFrogs.interval = 100f;
      sebsFrogs.totalNumberOfLoops = 9999;
      sebsFrogs.position = vector2_1 * 64f + new Vector2((float) Game1.random.Choose<int>(22, 25), (float) Game1.random.Choose<int>(2, 1)) * 4f;
      sebsFrogs.scale = 4f;
      sebsFrogs.flipped = Game1.random.NextBool();
      sebsFrogs.layerDepth = (float) (((double) vector2_1.Y + 2.0 + 0.10999999940395355) * 64.0 / 10000.0);
      sebsFrogs.Parent = (GameLocation) this;
      temporarySprites.Add((TemporaryAnimatedSprite) sebsFrogs);
    }
    if (Game1.player.activeDialogueEvents.ContainsKey("sebastianFrog2") || !Game1.random.NextBool())
      return;
    Texture2D texture2D1 = Game1.temporaryContent.Load<Texture2D>("TileSheets\\critters");
    TemporaryAnimatedSpriteList temporarySprites1 = this.TemporarySprites;
    SebsFrogs sebsFrogs1 = new SebsFrogs();
    sebsFrogs1.texture = texture2D1;
    sebsFrogs1.sourceRect = new Microsoft.Xna.Framework.Rectangle(64 /*0x40*/, 240 /*0xF0*/, 16 /*0x10*/, 16 /*0x10*/);
    sebsFrogs1.animationLength = 1;
    sebsFrogs1.sourceRectStartingPos = new Vector2(64f, 240f);
    sebsFrogs1.interval = 150f;
    sebsFrogs1.totalNumberOfLoops = 9999;
    sebsFrogs1.position = vector2_1 * 64f + new Vector2(8f, 3f) * 4f;
    sebsFrogs1.scale = 4f;
    sebsFrogs1.layerDepth = (float) (((double) vector2_1.Y + 2.0 + 0.10999999940395355) * 64.0 / 10000.0);
    sebsFrogs1.flipped = Game1.random.NextBool();
    sebsFrogs1.pingPong = false;
    sebsFrogs1.Parent = (GameLocation) this;
    temporarySprites1.Add((TemporaryAnimatedSprite) sebsFrogs1);
    if (Game1.random.NextDouble() >= 0.1 || Game1.timeOfDay <= 610)
      return;
    DelayedAction.playSoundAfterDelay("croak", 1000);
  }

  private void addFurnitureIfSpaceIsFreePenny(
    List<StardewValley.Object> objectsToStoreInChests,
    Furniture f,
    Furniture heldObject = null)
  {
    bool flag = false;
    foreach (Furniture furniture in this.furniture)
    {
      if (f.GetBoundingBox().Intersects(furniture.GetBoundingBox()))
      {
        flag = true;
        break;
      }
    }
    if (this.objects.ContainsKey(f.TileLocation))
      flag = true;
    if (!flag)
    {
      this.furniture.Add(f);
      if (heldObject == null)
        return;
      f.heldObject.Value = (StardewValley.Object) heldObject;
    }
    else
    {
      objectsToStoreInChests.Add((StardewValley.Object) f);
      if (heldObject == null)
        return;
      objectsToStoreInChests.Add((StardewValley.Object) heldObject);
    }
  }

  private void decoratePennyRoom(int whichStyle, List<StardewValley.Object> objectsToStoreInChests)
  {
    List<Chest> chestList = new List<Chest>();
    List<Vector2> vector2List = new List<Vector2>();
    Color color = new Color();
    switch (whichStyle)
    {
      case 0:
        if (this.upgradeLevel == 1)
        {
          this.addFurnitureIfSpaceIsFreePenny(objectsToStoreInChests, ItemRegistry.Create<Furniture>("(F)1916").SetPlacement(20, 1));
          this.addFurnitureIfSpaceIsFreePenny(objectsToStoreInChests, ItemRegistry.Create<Furniture>("(F)1914").SetPlacement(21, 1));
          this.addFurnitureIfSpaceIsFreePenny(objectsToStoreInChests, ItemRegistry.Create<Furniture>("(F)1915").SetPlacement(22, 1));
          this.addFurnitureIfSpaceIsFreePenny(objectsToStoreInChests, ItemRegistry.Create<Furniture>("(F)1914").SetPlacement(23, 1));
          this.addFurnitureIfSpaceIsFreePenny(objectsToStoreInChests, ItemRegistry.Create<Furniture>("(F)1916").SetPlacement(24, 1));
          this.addFurnitureIfSpaceIsFreePenny(objectsToStoreInChests, ItemRegistry.Create<Furniture>("(F)1682").SetPlacement(26, 1));
          this.addFurnitureIfSpaceIsFreePenny(objectsToStoreInChests, ItemRegistry.Create<Furniture>("(F)1747").SetPlacement(25, 4));
          this.addFurnitureIfSpaceIsFreePenny(objectsToStoreInChests, ItemRegistry.Create<Furniture>("(F)1395").SetPlacement(26, 4), ItemRegistry.Create<Furniture>("(F)1363"));
          this.addFurnitureIfSpaceIsFreePenny(objectsToStoreInChests, ItemRegistry.Create<Furniture>("(F)1443").SetPlacement(27, 4));
          this.addFurnitureIfSpaceIsFreePenny(objectsToStoreInChests, ItemRegistry.Create<Furniture>("(F)1664").SetPlacement(27, 5, 1));
          this.addFurnitureIfSpaceIsFreePenny(objectsToStoreInChests, ItemRegistry.Create<Furniture>("(F)1978").SetPlacement(21, 6));
          this.addFurnitureIfSpaceIsFreePenny(objectsToStoreInChests, ItemRegistry.Create<Furniture>("(F)1124").SetPlacement(26, 9), ItemRegistry.Create<Furniture>("(F)1368"));
          this.addFurnitureIfSpaceIsFreePenny(objectsToStoreInChests, ItemRegistry.Create<Furniture>("(F)6").SetPlacement(25, 10, 1));
          this.addFurnitureIfSpaceIsFreePenny(objectsToStoreInChests, ItemRegistry.Create<Furniture>("(F)1296").SetPlacement(28, 10));
          this.addFurnitureIfSpaceIsFreePenny(objectsToStoreInChests, ItemRegistry.Create<Furniture>("(F)1747").SetPlacement(24, 10));
          this.SetWallpaper("107", "Bedroom");
          this.SetFloor("2", "Bedroom");
          color = new Color(85, 85, (int) byte.MaxValue);
          vector2List.Add(new Vector2(21f, 10f));
          vector2List.Add(new Vector2(22f, 10f));
          break;
        }
        this.addFurnitureIfSpaceIsFreePenny(objectsToStoreInChests, ItemRegistry.Create<Furniture>("(F)1916").SetPlacement(38, 20));
        this.addFurnitureIfSpaceIsFreePenny(objectsToStoreInChests, ItemRegistry.Create<Furniture>("(F)1914").SetPlacement(39, 20));
        this.addFurnitureIfSpaceIsFreePenny(objectsToStoreInChests, ItemRegistry.Create<Furniture>("(F)1604").SetPlacement(41, 20));
        this.addFurnitureIfSpaceIsFreePenny(objectsToStoreInChests, ItemRegistry.Create<Furniture>("(F)1915").SetPlacement(43, 20));
        this.addFurnitureIfSpaceIsFreePenny(objectsToStoreInChests, ItemRegistry.Create<Furniture>("(F)1916").SetPlacement(45, 20));
        this.addFurnitureIfSpaceIsFreePenny(objectsToStoreInChests, ItemRegistry.Create<Furniture>("(F)1914").SetPlacement(47, 20));
        this.addFurnitureIfSpaceIsFreePenny(objectsToStoreInChests, ItemRegistry.Create<Furniture>("(F)1916").SetPlacement(48 /*0x30*/, 20));
        this.addFurnitureIfSpaceIsFreePenny(objectsToStoreInChests, ItemRegistry.Create<Furniture>("(F)1443").SetPlacement(38, 23));
        this.addFurnitureIfSpaceIsFreePenny(objectsToStoreInChests, ItemRegistry.Create<Furniture>("(F)1747").SetPlacement(39, 23));
        this.addFurnitureIfSpaceIsFreePenny(objectsToStoreInChests, ItemRegistry.Create<Furniture>("(F)1395").SetPlacement(40, 23), ItemRegistry.Create<Furniture>("(F)1363"));
        this.addFurnitureIfSpaceIsFreePenny(objectsToStoreInChests, ItemRegistry.Create<Furniture>("(F)714").SetPlacement(46, 23));
        this.addFurnitureIfSpaceIsFreePenny(objectsToStoreInChests, ItemRegistry.Create<Furniture>("(F)1443").SetPlacement(48 /*0x30*/, 23));
        this.addFurnitureIfSpaceIsFreePenny(objectsToStoreInChests, ItemRegistry.Create<Furniture>("(F)1978").SetPlacement(42, 25));
        this.addFurnitureIfSpaceIsFreePenny(objectsToStoreInChests, ItemRegistry.Create<Furniture>("(F)1664").SetPlacement(47, 25, 1));
        this.addFurnitureIfSpaceIsFreePenny(objectsToStoreInChests, ItemRegistry.Create<Furniture>("(F)1664").SetPlacement(38, 27, 1));
        this.addFurnitureIfSpaceIsFreePenny(objectsToStoreInChests, ItemRegistry.Create<Furniture>("(F)1124").SetPlacement(46, 31 /*0x1F*/), ItemRegistry.Create<Furniture>("(F)1368"));
        this.addFurnitureIfSpaceIsFreePenny(objectsToStoreInChests, ItemRegistry.Create<Furniture>("(F)416").SetPlacement(40, 32 /*0x20*/, 2));
        this.addFurnitureIfSpaceIsFreePenny(objectsToStoreInChests, ItemRegistry.Create<Furniture>("(F)1296").SetPlacement(38, 32 /*0x20*/));
        this.addFurnitureIfSpaceIsFreePenny(objectsToStoreInChests, ItemRegistry.Create<Furniture>("(F)6").SetPlacement(45, 32 /*0x20*/, 1));
        this.addFurnitureIfSpaceIsFreePenny(objectsToStoreInChests, ItemRegistry.Create<Furniture>("(F)1296").SetPlacement(48 /*0x30*/, 32 /*0x20*/));
        this.SetWallpaper("107", "Bedroom");
        this.SetFloor("2", "Bedroom");
        color = new Color(85, 85, (int) byte.MaxValue);
        vector2List.Add(new Vector2(38f, 24f));
        vector2List.Add(new Vector2(39f, 24f));
        break;
      case 1:
        if (this.upgradeLevel == 1)
        {
          this.addFurnitureIfSpaceIsFreePenny(objectsToStoreInChests, ItemRegistry.Create<Furniture>("(F)1678").SetPlacement(20, 1));
          this.addFurnitureIfSpaceIsFreePenny(objectsToStoreInChests, ItemRegistry.Create<Furniture>("(F)1814").SetPlacement(21, 1));
          this.addFurnitureIfSpaceIsFreePenny(objectsToStoreInChests, ItemRegistry.Create<Furniture>("(F)1814").SetPlacement(22, 1));
          this.addFurnitureIfSpaceIsFreePenny(objectsToStoreInChests, ItemRegistry.Create<Furniture>("(F)1814").SetPlacement(23, 1));
          this.addFurnitureIfSpaceIsFreePenny(objectsToStoreInChests, ItemRegistry.Create<Furniture>("(F)1907").SetPlacement(24, 1));
          this.addFurnitureIfSpaceIsFreePenny(objectsToStoreInChests, ItemRegistry.Create<Furniture>("(F)1400").SetPlacement(25, 4), ItemRegistry.Create<Furniture>("(F)1365"));
          this.addFurnitureIfSpaceIsFreePenny(objectsToStoreInChests, ItemRegistry.Create<Furniture>("(F)1866").SetPlacement(26, 4));
          this.addFurnitureIfSpaceIsFreePenny(objectsToStoreInChests, ItemRegistry.Create<Furniture>("(F)1909").SetPlacement(27, 6, 1));
          this.addFurnitureIfSpaceIsFreePenny(objectsToStoreInChests, ItemRegistry.Create<Furniture>("(F)1451").SetPlacement(21, 6));
          this.addFurnitureIfSpaceIsFreePenny(objectsToStoreInChests, ItemRegistry.Create<Furniture>("(F)1138").SetPlacement(27, 9), ItemRegistry.Create<Furniture>("(F)1378"));
          this.addFurnitureIfSpaceIsFreePenny(objectsToStoreInChests, ItemRegistry.Create<Furniture>("(F)12").SetPlacement(26, 10, 1));
          this.addFurnitureIfSpaceIsFreePenny(objectsToStoreInChests, ItemRegistry.Create<Furniture>("(F)1758").SetPlacement(24, 10));
          this.addFurnitureIfSpaceIsFreePenny(objectsToStoreInChests, ItemRegistry.Create<Furniture>("(F)1618").SetPlacement(21, 9));
          this.addFurnitureIfSpaceIsFreePenny(objectsToStoreInChests, ItemRegistry.Create<Furniture>("(F)1390").SetPlacement(22, 10));
          this.SetWallpaper("84", "Bedroom");
          this.SetFloor("35", "Bedroom");
          color = new Color((int) byte.MaxValue, 85, 85);
          vector2List.Add(new Vector2(21f, 10f));
          vector2List.Add(new Vector2(23f, 10f));
          break;
        }
        this.addFurnitureIfSpaceIsFreePenny(objectsToStoreInChests, ItemRegistry.Create<Furniture>("(F)1678").SetPlacement(39, 20));
        this.addFurnitureIfSpaceIsFreePenny(objectsToStoreInChests, ItemRegistry.Create<Furniture>("(F)1907").SetPlacement(40, 20));
        this.addFurnitureIfSpaceIsFreePenny(objectsToStoreInChests, ItemRegistry.Create<Furniture>("(F)1814").SetPlacement(42, 20));
        this.addFurnitureIfSpaceIsFreePenny(objectsToStoreInChests, ItemRegistry.Create<Furniture>("(F)1814").SetPlacement(43, 20));
        this.addFurnitureIfSpaceIsFreePenny(objectsToStoreInChests, ItemRegistry.Create<Furniture>("(F)1814").SetPlacement(44, 20));
        this.addFurnitureIfSpaceIsFreePenny(objectsToStoreInChests, ItemRegistry.Create<Furniture>("(F)1907").SetPlacement(45, 20));
        this.addFurnitureIfSpaceIsFreePenny(objectsToStoreInChests, ItemRegistry.Create<Furniture>("(F)1916").SetPlacement(48 /*0x30*/, 20));
        this.addFurnitureIfSpaceIsFreePenny(objectsToStoreInChests, ItemRegistry.Create<Furniture>("(F)1758").SetPlacement(38, 23));
        this.addFurnitureIfSpaceIsFreePenny(objectsToStoreInChests, ItemRegistry.Create<Furniture>("(F)1400").SetPlacement(40, 23), ItemRegistry.Create<Furniture>("(F)1365"));
        this.addFurnitureIfSpaceIsFreePenny(objectsToStoreInChests, ItemRegistry.Create<Furniture>("(F)1390").SetPlacement(46, 23));
        this.addFurnitureIfSpaceIsFreePenny(objectsToStoreInChests, ItemRegistry.Create<Furniture>("(F)1866").SetPlacement(47, 23));
        this.addFurnitureIfSpaceIsFreePenny(objectsToStoreInChests, ItemRegistry.Create<Furniture>("(F)1387").SetPlacement(38, 24));
        this.addFurnitureIfSpaceIsFreePenny(objectsToStoreInChests, ItemRegistry.Create<Furniture>("(F)1909").SetPlacement(47, 24, 1));
        this.addFurnitureIfSpaceIsFreePenny(objectsToStoreInChests, ItemRegistry.Create<Furniture>("(F)719").SetPlacement(38, 25, 1));
        this.addFurnitureIfSpaceIsFreePenny(objectsToStoreInChests, ItemRegistry.Create<Furniture>("(F)1451").SetPlacement(42, 25));
        this.addFurnitureIfSpaceIsFreePenny(objectsToStoreInChests, ItemRegistry.Create<Furniture>("(F)1909").SetPlacement(38, 27, 1));
        this.addFurnitureIfSpaceIsFreePenny(objectsToStoreInChests, ItemRegistry.Create<Furniture>("(F)1389").SetPlacement(47, 29));
        this.addFurnitureIfSpaceIsFreePenny(objectsToStoreInChests, ItemRegistry.Create<Furniture>("(F)1377").SetPlacement(48 /*0x30*/, 29));
        this.addFurnitureIfSpaceIsFreePenny(objectsToStoreInChests, ItemRegistry.Create<Furniture>("(F)1758").SetPlacement(41, 30));
        this.addFurnitureIfSpaceIsFreePenny(objectsToStoreInChests, ItemRegistry.Create<Furniture>("(F)424").SetPlacement(42, 30, 1));
        this.addFurnitureIfSpaceIsFreePenny(objectsToStoreInChests, ItemRegistry.Create<Furniture>("(F)1618").SetPlacement(44, 30));
        this.addFurnitureIfSpaceIsFreePenny(objectsToStoreInChests, ItemRegistry.Create<Furniture>("(F)536").SetPlacement(47, 30, 3));
        this.addFurnitureIfSpaceIsFreePenny(objectsToStoreInChests, ItemRegistry.Create<Furniture>("(F)1138").SetPlacement(38, 31 /*0x1F*/), ItemRegistry.Create<Furniture>("(F)1378"));
        this.addFurnitureIfSpaceIsFreePenny(objectsToStoreInChests, ItemRegistry.Create<Furniture>("(F)1383").SetPlacement(41, 31 /*0x1F*/));
        this.addFurnitureIfSpaceIsFreePenny(objectsToStoreInChests, ItemRegistry.Create<Furniture>("(F)1449").SetPlacement(48 /*0x30*/, 32 /*0x20*/));
        this.SetWallpaper("84", "Bedroom");
        this.SetFloor("35", "Bedroom");
        color = new Color((int) byte.MaxValue, 85, 85);
        vector2List.Add(new Vector2(39f, 23f));
        vector2List.Add(new Vector2(43f, 25f));
        break;
      case 2:
        if (this.upgradeLevel == 1)
        {
          this.addFurnitureIfSpaceIsFreePenny(objectsToStoreInChests, ItemRegistry.Create<Furniture>("(F)1673").SetPlacement(20, 1));
          this.addFurnitureIfSpaceIsFreePenny(objectsToStoreInChests, ItemRegistry.Create<Furniture>("(F)1547").SetPlacement(21, 1));
          this.addFurnitureIfSpaceIsFreePenny(objectsToStoreInChests, ItemRegistry.Create<Furniture>("(F)1675").SetPlacement(24, 1));
          this.addFurnitureIfSpaceIsFreePenny(objectsToStoreInChests, ItemRegistry.Create<Furniture>("(F)1900").SetPlacement(25, 1));
          this.addFurnitureIfSpaceIsFreePenny(objectsToStoreInChests, ItemRegistry.Create<Furniture>("(F)1393").SetPlacement(25, 4), ItemRegistry.Create<Furniture>("(F)1367"));
          this.addFurnitureIfSpaceIsFreePenny(objectsToStoreInChests, ItemRegistry.Create<Furniture>("(F)1798").SetPlacement(26, 4));
          this.addFurnitureIfSpaceIsFreePenny(objectsToStoreInChests, ItemRegistry.Create<Furniture>("(F)1902").SetPlacement(25, 5));
          this.addFurnitureIfSpaceIsFreePenny(objectsToStoreInChests, ItemRegistry.Create<Furniture>("(F)1751").SetPlacement(22, 6));
          this.addFurnitureIfSpaceIsFreePenny(objectsToStoreInChests, ItemRegistry.Create<Furniture>("(F)1122").SetPlacement(26, 9), ItemRegistry.Create<Furniture>("(F)1378"));
          this.addFurnitureIfSpaceIsFreePenny(objectsToStoreInChests, ItemRegistry.Create<Furniture>("(F)197").SetPlacement(28, 9, 3));
          this.addFurnitureIfSpaceIsFreePenny(objectsToStoreInChests, ItemRegistry.Create<Furniture>("(F)3").SetPlacement(25, 10, 1));
          this.addFurnitureIfSpaceIsFreePenny(objectsToStoreInChests, ItemRegistry.Create<Furniture>("(F)1294").SetPlacement(20, 10));
          this.addFurnitureIfSpaceIsFreePenny(objectsToStoreInChests, ItemRegistry.Create<Furniture>("(F)1294").SetPlacement(24, 10));
          this.addFurnitureIfSpaceIsFreePenny(objectsToStoreInChests, ItemRegistry.Create<Furniture>("(F)1964").SetPlacement(21, 8));
          this.SetWallpaper("95", "Bedroom");
          this.SetFloor("1", "Bedroom");
          color = new Color(85, 85, 85);
          vector2List.Add(new Vector2(22f, 10f));
          vector2List.Add(new Vector2(23f, 10f));
          break;
        }
        this.addFurnitureIfSpaceIsFreePenny(objectsToStoreInChests, ItemRegistry.Create<Furniture>("(F)1673").SetPlacement(38, 20));
        this.addFurnitureIfSpaceIsFreePenny(objectsToStoreInChests, ItemRegistry.Create<Furniture>("(F)1675").SetPlacement(40, 20));
        this.addFurnitureIfSpaceIsFreePenny(objectsToStoreInChests, ItemRegistry.Create<Furniture>("(F)1547").SetPlacement(42, 20));
        this.addFurnitureIfSpaceIsFreePenny(objectsToStoreInChests, ItemRegistry.Create<Furniture>("(F)1900").SetPlacement(45, 20));
        this.addFurnitureIfSpaceIsFreePenny(objectsToStoreInChests, ItemRegistry.Create<Furniture>("(F)1751").SetPlacement(38, 23));
        this.addFurnitureIfSpaceIsFreePenny(objectsToStoreInChests, ItemRegistry.Create<Furniture>("(F)1393").SetPlacement(40, 23), ItemRegistry.Create<Furniture>("(F)1367"));
        this.addFurnitureIfSpaceIsFreePenny(objectsToStoreInChests, ItemRegistry.Create<Furniture>("(F)1798").SetPlacement(47, 23));
        this.addFurnitureIfSpaceIsFreePenny(objectsToStoreInChests, ItemRegistry.Create<Furniture>("(F)1902").SetPlacement(46, 24));
        this.addFurnitureIfSpaceIsFreePenny(objectsToStoreInChests, ItemRegistry.Create<Furniture>("(F)1964").SetPlacement(42, 25));
        this.addFurnitureIfSpaceIsFreePenny(objectsToStoreInChests, ItemRegistry.Create<Furniture>("(F)1294").SetPlacement(38, 26));
        this.addFurnitureIfSpaceIsFreePenny(objectsToStoreInChests, ItemRegistry.Create<Furniture>("(F)3").SetPlacement(46, 29));
        this.addFurnitureIfSpaceIsFreePenny(objectsToStoreInChests, ItemRegistry.Create<Furniture>("(F)1294").SetPlacement(38, 30));
        this.addFurnitureIfSpaceIsFreePenny(objectsToStoreInChests, ItemRegistry.Create<Furniture>("(F)1122").SetPlacement(46, 30), ItemRegistry.Create<Furniture>("(F)1369"));
        this.addFurnitureIfSpaceIsFreePenny(objectsToStoreInChests, ItemRegistry.Create<Furniture>("(F)197").SetPlacement(48 /*0x30*/, 30, 3));
        this.addFurnitureIfSpaceIsFreePenny(objectsToStoreInChests, ItemRegistry.Create<Furniture>("(F)709").SetPlacement(38, 31 /*0x1F*/, 1));
        this.addFurnitureIfSpaceIsFreePenny(objectsToStoreInChests, ItemRegistry.Create<Furniture>("(F)3").SetPlacement(47, 32 /*0x20*/, 2));
        this.SetWallpaper("95", "Bedroom");
        this.SetFloor("1", "Bedroom");
        color = new Color(85, 85, 85);
        vector2List.Add(new Vector2(39f, 23f));
        vector2List.Add(new Vector2(46f, 23f));
        break;
    }
    if (objectsToStoreInChests != null)
    {
      foreach (StardewValley.Object objectsToStoreInChest in objectsToStoreInChests)
      {
        if (chestList.Count == 0)
          chestList.Add(new Chest(true));
        bool flag = false;
        foreach (Chest chest in chestList)
        {
          if (chest.addItem((Item) objectsToStoreInChest) == null)
            flag = true;
        }
        if (!flag)
        {
          Chest chest = new Chest(true);
          chestList.Add(chest);
          chest.addItem((Item) objectsToStoreInChest);
        }
      }
    }
    for (int index = 0; index < chestList.Count; ++index)
    {
      Chest o = chestList[index];
      o.playerChoiceColor.Value = color;
      this.PlaceInNearbySpace(vector2List[Math.Min(index, vector2List.Count - 1)], (StardewValley.Object) o);
    }
  }

  public void PlaceInNearbySpace(Vector2 tileLocation, StardewValley.Object o)
  {
    if (o == null || tileLocation.Equals(Vector2.Zero))
      return;
    int num = 0;
    Queue<Vector2> vector2Queue = new Queue<Vector2>();
    HashSet<Vector2> vector2Set = new HashSet<Vector2>();
    vector2Queue.Enqueue(tileLocation);
    Vector2 vector2 = Vector2.Zero;
    for (; num < 100; ++num)
    {
      vector2 = vector2Queue.Dequeue();
      if (!this.CanItemBePlacedHere(vector2))
      {
        vector2Set.Add(vector2);
        foreach (Vector2 adjacentTileLocation in Utility.getAdjacentTileLocations(vector2))
        {
          if (!vector2Set.Contains(adjacentTileLocation))
            vector2Queue.Enqueue(adjacentTileLocation);
        }
      }
      else
        break;
    }
    if (vector2.Equals(Vector2.Zero) || !this.CanItemBePlacedHere(vector2))
      return;
    o.TileLocation = vector2;
    this.objects.Add(vector2, o);
  }

  public virtual void RefreshFloorObjectNeighbors()
  {
    foreach (Vector2 key in this.terrainFeatures.Keys)
    {
      if (this.terrainFeatures[key] is Flooring terrainFeature)
        terrainFeature.OnAdded((GameLocation) this, key);
    }
  }

  public void moveObjectsForHouseUpgrade(int whichUpgrade)
  {
    this.previousUpgradeLevel = this.upgradeLevel;
    this.overlayObjects.Clear();
    switch (whichUpgrade)
    {
      case 0:
        if (this.upgradeLevel != 1)
          break;
        this.shiftContents(-6, 0);
        break;
      case 1:
        switch (this.upgradeLevel)
        {
          case 0:
            this.shiftContents(6, 0);
            return;
          case 2:
            this.shiftContents(-3, 0);
            return;
          default:
            return;
        }
      case 2:
      case 3:
        switch (this.upgradeLevel)
        {
          case 0:
            this.shiftContents(24, 19);
            return;
          case 1:
            this.shiftContents(18, 19);
            foreach (Furniture furniture in this.furniture)
            {
              if ((double) furniture.tileLocation.X >= 25.0 && (double) furniture.tileLocation.X <= 28.0 && (double) furniture.tileLocation.Y >= 20.0 && (double) furniture.tileLocation.Y <= 21.0)
                furniture.TileLocation = new Vector2(furniture.tileLocation.X - 3f, furniture.tileLocation.Y - 9f);
            }
            this.moveFurniture(42, 23, 16 /*0x10*/, 14);
            this.moveFurniture(43, 23, 17, 14);
            this.moveFurniture(44, 23, 18, 14);
            this.moveFurniture(43, 24, 22, 14);
            this.moveFurniture(44, 24, 23, 14);
            this.moveFurniture(42, 24, 19, 14);
            this.moveFurniture(43, 25, 20, 14);
            this.moveFurniture(44, 26, 21, 14);
            return;
          default:
            return;
        }
    }
  }

  protected override LocalizedContentManager getMapLoader()
  {
    if (this.mapLoader == null)
      this.mapLoader = Game1.game1.xTileContent.CreateTemporary();
    return this.mapLoader;
  }

  protected override void _updateAmbientLighting()
  {
    if (Game1.isStartingToGetDarkOut((GameLocation) this) || (double) this.lightLevel.Value > 0.0)
    {
      float t = 1f - Utility.Clamp((float) Utility.CalculateMinutesBetweenTimes(Game1.timeOfDay + Game1.gameTimeInterval / (Game1.realMilliSecondsPerGameMinute + this.ExtraMillisecondsPerInGameMinute), Game1.getTrulyDarkTime((GameLocation) this)) / 120f, 0.0f, 1f);
      Game1.ambientLight = new Color((int) (byte) Utility.Lerp(Game1.isRaining ? (float) this.rainLightingColor.R : 0.0f, (float) this.nightLightingColor.R, t), (int) (byte) Utility.Lerp(Game1.isRaining ? (float) this.rainLightingColor.G : 0.0f, (float) this.nightLightingColor.G, t), (int) (byte) Utility.Lerp(0.0f, (float) this.nightLightingColor.B, t));
    }
    else
      Game1.ambientLight = Game1.isRaining ? this.rainLightingColor : Color.White;
  }

  public override void drawAboveFrontLayer(SpriteBatch b)
  {
    base.drawAboveFrontLayer(b);
    if (!this.fridge.Value.mutex.IsLocked())
      return;
    b.Draw(Game1.mouseCursors2, Game1.GlobalToLocal(Game1.viewport, new Vector2((float) this.fridgePosition.X, (float) (this.fridgePosition.Y - 1)) * 64f), new Microsoft.Xna.Framework.Rectangle?(new Microsoft.Xna.Framework.Rectangle(0, 192 /*0xC0*/, 16 /*0x10*/, 32 /*0x20*/)), Color.White, 0.0f, Vector2.Zero, 4f, SpriteEffects.None, (float) ((this.fridgePosition.Y + 1) * 64 /*0x40*/ + 1) / 10000f);
  }

  public override void updateMap()
  {
    bool flag = this.HasNpcSpouseOrRoommate();
    this.mapPath.Value = $"Maps\\FarmHouse{(this.upgradeLevel == 0 ? "" : (this.upgradeLevel == 3 ? "2" : this.upgradeLevel.ToString() ?? ""))}{(flag ? "_marriage" : "")}";
    base.updateMap();
  }

  public virtual void setMapForUpgradeLevel(int level)
  {
    this.upgradeLevel = level;
    int num = this.synchronizedDisplayedLevel.Value;
    this.currentlyDisplayedUpgradeLevel = level;
    this.synchronizedDisplayedLevel.Value = level;
    bool flag1 = this.HasNpcSpouseOrRoommate();
    if (this.displayingSpouseRoom && !flag1)
      this.displayingSpouseRoom = false;
    this.updateMap();
    this.RefreshFloorObjectNeighbors();
    if (flag1)
      this.showSpouseRoom();
    this.loadObjects();
    if (level == 3)
    {
      this.AddCellarTiles();
      this.createCellarWarps();
      Game1.player.craftingRecipes.TryAdd("Cask", 0);
    }
    bool flag2 = this.previousUpgradeLevel == 0 && this.upgradeLevel >= 0;
    if (this.previousUpgradeLevel >= 0)
    {
      if (this.previousUpgradeLevel < 2 && this.upgradeLevel >= 2)
      {
        for (int index1 = 0; index1 < this.map.Layers[0].LayerWidth; ++index1)
        {
          for (int index2 = 0; index2 < this.map.Layers[0].LayerHeight; ++index2)
          {
            if (this.doesTileHaveProperty(index1, index2, "DefaultChildBedPosition", "Back") != null)
            {
              this.furniture.Add((Furniture) new BedFurniture(BedFurniture.CHILD_BED_INDEX, new Vector2((float) index1, (float) index2)));
              break;
            }
          }
        }
      }
      Furniture furniture1 = (Furniture) null;
      if (this.previousUpgradeLevel == 0)
      {
        foreach (Furniture furniture2 in this.furniture)
        {
          if (furniture2 is BedFurniture bedFurniture && bedFurniture.bedType == BedFurniture.BedType.Single)
          {
            furniture1 = (Furniture) bedFurniture;
            break;
          }
        }
      }
      else
      {
        foreach (Furniture furniture3 in this.furniture)
        {
          if (furniture3 is BedFurniture bedFurniture && bedFurniture.bedType == BedFurniture.BedType.Double)
          {
            furniture1 = (Furniture) bedFurniture;
            break;
          }
        }
      }
      if (this.upgradeLevel != 3 || flag2)
      {
        for (int index3 = 0; index3 < this.map.Layers[0].LayerWidth; ++index3)
        {
          for (int index4 = 0; index4 < this.map.Layers[0].LayerHeight; ++index4)
          {
            if (this.doesTileHaveProperty(index3, index4, "DefaultBedPosition", "Back") != null)
            {
              string bedId = BedFurniture.DEFAULT_BED_INDEX;
              if (this.previousUpgradeLevel != 1 || furniture1 == null || (double) furniture1.tileLocation.X == 39.0 && (double) furniture1.tileLocation.Y == 22.0)
              {
                if (furniture1 != null)
                  bedId = furniture1.ItemId;
                if (this.previousUpgradeLevel == 0 && furniture1 != null)
                {
                  furniture1.performRemoveAction();
                  this.furniture.Remove(this.furniture.GuidOf(furniture1));
                  this.furniture.Add((Furniture) new BedFurniture(Utility.GetDoubleWideVersionOfBed(bedId), new Vector2((float) index3, (float) index4)));
                  break;
                }
                if (furniture1 != null)
                {
                  furniture1.performRemoveAction();
                  this.furniture.Remove(this.furniture.GuidOf(furniture1));
                  this.furniture.Add((Furniture) new BedFurniture(furniture1.ItemId, new Vector2((float) index3, (float) index4)));
                  break;
                }
                break;
              }
              break;
            }
          }
        }
      }
      this.previousUpgradeLevel = -1;
    }
    if (num != level)
      this.lightGlows.Clear();
    this.fridgePosition = this.GetFridgePositionFromMap() ?? Point.Zero;
  }

  /// <summary>Get the fridge position by scanning the map tiles for the sprite index.</summary>
  /// <remarks>This is relatively expensive. Most code should use the cached <see cref="F:StardewValley.Locations.FarmHouse.fridgePosition" /> instead.</remarks>
  public Point? GetFridgePositionFromMap()
  {
    Layer layer = this.map.RequireLayer("Buildings");
    for (int y = 0; y < layer.LayerHeight; ++y)
    {
      for (int x = 0; x < layer.LayerWidth; ++x)
      {
        if (layer.GetTileIndexAt(x, y, "untitled tile sheet") == 173)
          return new Point?(new Point(x, y));
      }
    }
    return new Point?();
  }

  public void createCellarWarps() => this.updateCellarWarps();

  public void updateCellarWarps()
  {
    Layer layer = this.map.RequireLayer("Back");
    string cellarName = this.GetCellarName();
    if (cellarName == null)
      return;
    for (int tileX = 0; tileX < layer.LayerWidth; ++tileX)
    {
      for (int tileY = 0; tileY < layer.LayerHeight; ++tileY)
      {
        string[] propertySplitBySpaces = this.GetTilePropertySplitBySpaces("TouchAction", "Back", tileX, tileY);
        if (ArgUtility.Get(propertySplitBySpaces, 0) == "Warp" && ArgUtility.Get(propertySplitBySpaces, 1, "").StartsWith("Cellar"))
        {
          propertySplitBySpaces[1] = cellarName;
          this.setTileProperty(tileX, tileY, "Back", "TouchAction", string.Join(" ", propertySplitBySpaces));
        }
      }
    }
    if (this.cellarWarps == null)
      return;
    foreach (Warp cellarWarp in this.cellarWarps)
    {
      if (!this.warps.Contains(cellarWarp))
        this.warps.Add(cellarWarp);
      cellarWarp.TargetName = cellarName;
    }
  }

  public virtual Point GetSpouseRoomCorner()
  {
    Point parsed;
    if (this.TryGetMapPropertyAs("SpouseRoomPosition", out parsed))
      return parsed;
    return this.upgradeLevel != 1 ? new Point(50, 20) : new Point(29, 1);
  }

  public virtual void loadSpouseRoom()
  {
    string spouse = this.owner?.spouse == null || !this.owner.isMarriedOrRoommates() ? (string) null : this.owner.spouse;
    CharacterData data;
    CharacterSpouseRoomData spouseRoom = NPC.TryGetData(spouse, out data) ? data?.SpouseRoom : (CharacterSpouseRoomData) null;
    this.spouseRoomSpot = this.GetSpouseRoomCorner();
    this.spouseRoomSpot.X += 3;
    this.spouseRoomSpot.Y += 4;
    if (spouse == null)
      return;
    string map_name = spouseRoom?.MapAsset ?? "spouseRooms";
    Microsoft.Xna.Framework.Rectangle rectangle = spouseRoom != null ? spouseRoom.MapSourceRect : CharacterSpouseRoomData.DefaultMapSourceRect;
    Point spouseRoomCorner = this.GetSpouseRoomCorner();
    Microsoft.Xna.Framework.Rectangle rect = new Microsoft.Xna.Framework.Rectangle(spouseRoomCorner.X, spouseRoomCorner.Y, rectangle.Width, rectangle.Height);
    Map map = Game1.game1.xTileContent.Load<Map>("Maps\\" + map_name);
    Point location = rectangle.Location;
    ((IDictionary<string, PropertyValue>) this.map.Properties).Remove("Light");
    ((IDictionary<string, PropertyValue>) this.map.Properties).Remove("DayTiles");
    ((IDictionary<string, PropertyValue>) this.map.Properties).Remove("NightTiles");
    List<KeyValuePair<Point, Tile>> keyValuePairList = new List<KeyValuePair<Point, Tile>>();
    Layer layer1 = this.map.RequireLayer("Front");
    for (int left = rect.Left; left < rect.Right; ++left)
    {
      Point key = new Point(left, rect.Bottom - 1);
      Tile tile = layer1.Tiles[key.X, key.Y];
      if (tile != null)
        keyValuePairList.Add(new KeyValuePair<Point, Tile>(key, tile));
    }
    if (this._appliedMapOverrides.Contains("spouse_room"))
      this._appliedMapOverrides.Remove("spouse_room");
    this.ApplyMapOverride(map_name, "spouse_room", new Microsoft.Xna.Framework.Rectangle?(new Microsoft.Xna.Framework.Rectangle(location.X, location.Y, rect.Width, rect.Height)), new Microsoft.Xna.Framework.Rectangle?(rect));
    Layer layer2 = map.RequireLayer("Buildings");
    Layer layer3 = map.RequireLayer("Front");
    for (int index1 = 0; index1 < rect.Width; ++index1)
    {
      for (int index2 = 0; index2 < rect.Height; ++index2)
      {
        int tileIndexAt1 = layer2.GetTileIndexAt(location.X + index1, location.Y + index2);
        if (tileIndexAt1 != -1)
          this.adjustMapLightPropertiesForLamp(tileIndexAt1, rect.X + index1, rect.Y + index2, "Buildings");
        if (index2 < rect.Height - 1)
        {
          int tileIndexAt2 = layer3.GetTileIndexAt(location.X + index1, location.Y + index2);
          if (tileIndexAt2 != -1)
            this.adjustMapLightPropertiesForLamp(tileIndexAt2, rect.X + index1, rect.Y + index2, "Front");
        }
      }
    }
    foreach (Point point in rect.GetPoints())
    {
      if (this.getTileIndexAt(point, "Paths") == 7)
      {
        this.spouseRoomSpot = point;
        break;
      }
    }
    Point spouseRoomSpot = this.GetSpouseRoomSpot();
    this.setTileProperty(spouseRoomSpot.X, spouseRoomSpot.Y, "Back", "NoFurniture", "T");
    foreach (KeyValuePair<Point, Tile> keyValuePair in keyValuePairList)
      layer1.Tiles[keyValuePair.Key.X, keyValuePair.Key.Y] = keyValuePair.Value;
  }

  public virtual Microsoft.Xna.Framework.Rectangle? GetCribBounds()
  {
    return this.upgradeLevel < 2 ? new Microsoft.Xna.Framework.Rectangle?() : new Microsoft.Xna.Framework.Rectangle?(new Microsoft.Xna.Framework.Rectangle(30, 12, 3, 4));
  }

  public virtual void UpdateChildRoom()
  {
    Microsoft.Xna.Framework.Rectangle? cribBounds = this.GetCribBounds();
    if (!cribBounds.HasValue)
      return;
    if (this._appliedMapOverrides.Contains("crib"))
      this._appliedMapOverrides.Remove("crib");
    this.ApplyMapOverride("FarmHouse_Crib_" + this.cribStyle.Value.ToString(), "crib", destination_rect: cribBounds);
  }

  public void playerDivorced() => this.displayingSpouseRoom = false;

  public virtual List<Microsoft.Xna.Framework.Rectangle> getForbiddenPetWarpTiles()
  {
    List<Microsoft.Xna.Framework.Rectangle> forbiddenPetWarpTiles = new List<Microsoft.Xna.Framework.Rectangle>();
    switch (this.upgradeLevel)
    {
      case 0:
        forbiddenPetWarpTiles.Add(new Microsoft.Xna.Framework.Rectangle(2, 8, 3, 4));
        break;
      case 1:
        forbiddenPetWarpTiles.Add(new Microsoft.Xna.Framework.Rectangle(8, 8, 3, 4));
        forbiddenPetWarpTiles.Add(new Microsoft.Xna.Framework.Rectangle(17, 8, 4, 3));
        break;
      case 2:
      case 3:
        forbiddenPetWarpTiles.Add(new Microsoft.Xna.Framework.Rectangle(26, 27, 3, 4));
        forbiddenPetWarpTiles.Add(new Microsoft.Xna.Framework.Rectangle(35, 27, 4, 3));
        forbiddenPetWarpTiles.Add(new Microsoft.Xna.Framework.Rectangle(27, 15, 4, 3));
        forbiddenPetWarpTiles.Add(new Microsoft.Xna.Framework.Rectangle(26, 17, 2, 6));
        break;
    }
    return forbiddenPetWarpTiles;
  }

  public bool canPetWarpHere(Vector2 tile_position)
  {
    foreach (Microsoft.Xna.Framework.Rectangle forbiddenPetWarpTile in this.getForbiddenPetWarpTiles())
    {
      if (forbiddenPetWarpTile.Contains((int) tile_position.X, (int) tile_position.Y))
        return false;
    }
    return true;
  }

  public override List<Microsoft.Xna.Framework.Rectangle> getWalls()
  {
    List<Microsoft.Xna.Framework.Rectangle> walls = new List<Microsoft.Xna.Framework.Rectangle>();
    switch (this.upgradeLevel)
    {
      case 0:
        walls.Add(new Microsoft.Xna.Framework.Rectangle(1, 1, 10, 3));
        break;
      case 1:
        walls.Add(new Microsoft.Xna.Framework.Rectangle(1, 1, 17, 3));
        walls.Add(new Microsoft.Xna.Framework.Rectangle(18, 6, 2, 2));
        walls.Add(new Microsoft.Xna.Framework.Rectangle(20, 1, 9, 3));
        break;
      case 2:
      case 3:
        int num1 = this.HasOwner ? 1 : 0;
        walls.Add(new Microsoft.Xna.Framework.Rectangle(1, 1, 12, 3));
        walls.Add(new Microsoft.Xna.Framework.Rectangle(15, 1, 13, 3));
        walls.Add(new Microsoft.Xna.Framework.Rectangle(13, 3, 2, 2));
        walls.Add(new Microsoft.Xna.Framework.Rectangle(1, 10, 10, 3));
        walls.Add(new Microsoft.Xna.Framework.Rectangle(13, 10, 8, 3));
        int num2 = num1 == 0 || !this.owner.hasOrWillReceiveMail("renovation_corner_open") ? 0 : -3;
        if (num1 != 0 && this.owner.hasOrWillReceiveMail("renovation_bedroom_open"))
        {
          walls.Add(new Microsoft.Xna.Framework.Rectangle(21, 15, 0, 2));
          walls.Add(new Microsoft.Xna.Framework.Rectangle(21, 10, 13 + num2, 3));
        }
        else
        {
          walls.Add(new Microsoft.Xna.Framework.Rectangle(21, 15, 2, 2));
          walls.Add(new Microsoft.Xna.Framework.Rectangle(23, 10, 11 + num2, 3));
        }
        if (num1 != 0 && this.owner.hasOrWillReceiveMail("renovation_southern_open"))
        {
          walls.Add(new Microsoft.Xna.Framework.Rectangle(23, 24, 3, 3));
          walls.Add(new Microsoft.Xna.Framework.Rectangle(31 /*0x1F*/, 24, 3, 3));
        }
        else
        {
          walls.Add(new Microsoft.Xna.Framework.Rectangle(0, 0, 0, 0));
          walls.Add(new Microsoft.Xna.Framework.Rectangle(0, 0, 0, 0));
        }
        if (num1 != 0 && this.owner.hasOrWillReceiveMail("renovation_corner_open"))
        {
          walls.Add(new Microsoft.Xna.Framework.Rectangle(30, 1, 9, 3));
          walls.Add(new Microsoft.Xna.Framework.Rectangle(28, 3, 2, 2));
        }
        else
        {
          walls.Add(new Microsoft.Xna.Framework.Rectangle(0, 0, 0, 0));
          walls.Add(new Microsoft.Xna.Framework.Rectangle(0, 0, 0, 0));
        }
        using (List<Microsoft.Xna.Framework.Rectangle>.Enumerator enumerator = walls.GetEnumerator())
        {
          while (enumerator.MoveNext())
            enumerator.Current.Offset(15, 10);
          break;
        }
    }
    return walls;
  }

  public override void TransferDataFromSavedLocation(GameLocation l)
  {
    if (l is FarmHouse farmHouse)
      this.cribStyle.Value = farmHouse.cribStyle.Value;
    base.TransferDataFromSavedLocation(l);
  }

  public override List<Microsoft.Xna.Framework.Rectangle> getFloors()
  {
    List<Microsoft.Xna.Framework.Rectangle> floors = new List<Microsoft.Xna.Framework.Rectangle>();
    switch (this.upgradeLevel)
    {
      case 0:
        floors.Add(new Microsoft.Xna.Framework.Rectangle(1, 3, 10, 9));
        break;
      case 1:
        floors.Add(new Microsoft.Xna.Framework.Rectangle(1, 3, 6, 9));
        floors.Add(new Microsoft.Xna.Framework.Rectangle(7, 3, 11, 9));
        floors.Add(new Microsoft.Xna.Framework.Rectangle(18, 8, 2, 2));
        floors.Add(new Microsoft.Xna.Framework.Rectangle(20, 3, 9, 8));
        break;
      case 2:
      case 3:
        int num = this.HasOwner ? 1 : 0;
        floors.Add(new Microsoft.Xna.Framework.Rectangle(1, 3, 12, 6));
        floors.Add(new Microsoft.Xna.Framework.Rectangle(15, 3, 13, 6));
        floors.Add(new Microsoft.Xna.Framework.Rectangle(13, 5, 2, 2));
        floors.Add(new Microsoft.Xna.Framework.Rectangle(0, 12, 10, 11));
        floors.Add(new Microsoft.Xna.Framework.Rectangle(10, 12, 11, 9));
        if (num != 0 && this.owner.mailReceived.Contains("renovation_bedroom_open"))
        {
          floors.Add(new Microsoft.Xna.Framework.Rectangle(21, 17, 0, 2));
          floors.Add(new Microsoft.Xna.Framework.Rectangle(21, 12, 14, 11));
        }
        else
        {
          floors.Add(new Microsoft.Xna.Framework.Rectangle(21, 17, 2, 2));
          floors.Add(new Microsoft.Xna.Framework.Rectangle(23, 12, 12, 11));
        }
        if (num != 0 && this.owner.hasOrWillReceiveMail("renovation_southern_open"))
          floors.Add(new Microsoft.Xna.Framework.Rectangle(23, 26, 11, 8));
        else
          floors.Add(new Microsoft.Xna.Framework.Rectangle(0, 0, 0, 0));
        if (num != 0 && this.owner.hasOrWillReceiveMail("renovation_corner_open"))
        {
          floors.Add(new Microsoft.Xna.Framework.Rectangle(28, 5, 2, 3));
          floors.Add(new Microsoft.Xna.Framework.Rectangle(30, 3, 9, 6));
        }
        else
        {
          floors.Add(new Microsoft.Xna.Framework.Rectangle(0, 0, 0, 0));
          floors.Add(new Microsoft.Xna.Framework.Rectangle(0, 0, 0, 0));
        }
        using (List<Microsoft.Xna.Framework.Rectangle>.Enumerator enumerator = floors.GetEnumerator())
        {
          while (enumerator.MoveNext())
            enumerator.Current.Offset(15, 10);
          break;
        }
    }
    return floors;
  }

  public virtual bool CanModifyCrib()
  {
    if (!this.HasOwner || this.owner.isMarriedOrRoommates() && this.owner.GetSpouseFriendship().DaysUntilBirthing != -1)
      return false;
    foreach (NPC child in this.owner.getChildren())
    {
      if (child.Age < 3)
        return false;
    }
    return true;
  }
}
