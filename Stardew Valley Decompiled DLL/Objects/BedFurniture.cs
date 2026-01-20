// Decompiled with JetBrains decompiler
// Type: StardewValley.Objects.BedFurniture
// Assembly: Stardew Valley, Version=1.6.15.24356, Culture=neutral, PublicKeyToken=null
// MVID: AA1F513A-94F0-4EF6-A35F-2D5B4A3721BF
// Assembly location: D:\SteamLibrary\steamapps\common\Stardew Valley\Stardew Valley.dll
// XML documentation location: D:\SteamLibrary\steamapps\common\Stardew Valley\Stardew Valley.xml

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Netcode;
using StardewValley.ItemTypeDefinitions;
using StardewValley.Locations;
using StardewValley.Network;
using System;
using System.Xml.Serialization;

#nullable disable
namespace StardewValley.Objects;

public class BedFurniture : Furniture
{
  public static string DEFAULT_BED_INDEX = "2048";
  public static string DOUBLE_BED_INDEX = "2052";
  public static string CHILD_BED_INDEX = "2076";
  [XmlIgnore]
  public int bedTileOffset;
  [XmlIgnore]
  protected bool _alreadyAttempingRemoval;
  [XmlIgnore]
  public static bool ignoreContextualBedSpotOffset = false;
  [XmlIgnore]
  protected NetEnum<BedFurniture.BedType> _bedType = new NetEnum<BedFurniture.BedType>(BedFurniture.BedType.Any);
  [XmlIgnore]
  public NetMutex mutex = new NetMutex();

  [XmlElement("bedType")]
  public BedFurniture.BedType bedType
  {
    get
    {
      if (this._bedType.Value == BedFurniture.BedType.Any)
      {
        BedFurniture.BedType bedType = BedFurniture.BedType.Single;
        string[] data = this.getData();
        if (data != null && data.Length > 1)
        {
          string[] strArray = ArgUtility.SplitBySpace(data[1]);
          if (strArray.Length > 1)
          {
            switch (strArray[1])
            {
              case "double":
                bedType = BedFurniture.BedType.Double;
                break;
              case "child":
                bedType = BedFurniture.BedType.Child;
                break;
            }
          }
        }
        this._bedType.Value = bedType;
      }
      return this._bedType.Value;
    }
    set => this._bedType.Value = value;
  }

  public BedFurniture()
  {
  }

  public BedFurniture(string itemId, Vector2 tile, int initialRotations)
    : base(itemId, tile, initialRotations)
  {
  }

  public BedFurniture(string itemId, Vector2 tile)
    : base(itemId, tile)
  {
  }

  /// <inheritdoc />
  protected override void initNetFields()
  {
    base.initNetFields();
    this.NetFields.AddField((INetSerializable) this._bedType, "_bedType").AddField((INetSerializable) this.mutex.NetFields, "mutex.NetFields");
  }

  public virtual bool IsBeingSleptIn()
  {
    GameLocation location = this.Location;
    if (location == null)
      return false;
    if (this.mutex.IsLocked())
      return true;
    Rectangle boundingBox = this.GetBoundingBox();
    foreach (Character farmer in location.farmers)
    {
      if (farmer.GetBoundingBox().Intersects(boundingBox))
        return true;
    }
    return false;
  }

  public override void DayUpdate()
  {
    base.DayUpdate();
    this.mutex.ReleaseLock();
  }

  public virtual void ReserveForNPC() => this.mutex.RequestLock();

  public override void AttemptRemoval(Action<Furniture> removal_action)
  {
    if (this._alreadyAttempingRemoval)
    {
      this._alreadyAttempingRemoval = false;
    }
    else
    {
      this._alreadyAttempingRemoval = true;
      this.mutex.RequestLock((Action) (() =>
      {
        this._alreadyAttempingRemoval = false;
        if (removal_action == null)
          return;
        removal_action((Furniture) this);
        this.mutex.ReleaseLock();
      }), (Action) (() => this._alreadyAttempingRemoval = false));
    }
  }

  public static BedFurniture GetBedAtTile(GameLocation location, int x, int y)
  {
    if (location == null)
      return (BedFurniture) null;
    foreach (Furniture furniture in location.furniture)
    {
      if (Utility.doesRectangleIntersectTile(furniture.GetBoundingBox(), x, y) && furniture is BedFurniture bedAtTile)
        return bedAtTile;
    }
    return (BedFurniture) null;
  }

  public static void ApplyWakeUpPosition(Farmer who)
  {
    string str = who.lastSleepLocation.Value;
    GameLocation locationFromName1 = str == null || !Game1.isLocationAccessible(str) ? (GameLocation) null : Game1.getLocationFromName(str);
    GameLocation locationFromName2 = Game1.getLocationFromName(who.disconnectLocation.Value);
    if (locationFromName2 != null && (long) who.disconnectDay.Value == (long) Game1.MasterPlayer.stats.DaysPlayed && !Game1.newDaySync.hasInstance())
    {
      who.currentLocation = locationFromName2;
      who.Position = who.disconnectPosition.Value;
    }
    else
    {
      bool? nullable = locationFromName1?.CanWakeUpHere(who);
      if (nullable.HasValue && nullable.GetValueOrDefault())
      {
        who.Position = Utility.PointToVector2(who.lastSleepPoint.Value) * 64f;
        who.currentLocation = locationFromName1;
        BedFurniture.ShiftPositionForBed(who);
      }
      else
      {
        if (locationFromName1 != null)
          Game1.log.Verbose($"Can't wake up in last sleep location '{locationFromName1.NameOrUniqueName}' because it has no bed and doesn't have the 'AllowWakeUpWithoutBed: true' map property set.");
        else if (str != null)
          Game1.log.Verbose($"Can't wake up in last sleep location '{str}' because no such location was found.");
        FarmHouse farmHouse = Game1.RequireLocation<FarmHouse>(who.homeLocation.Value);
        who.currentLocation = (GameLocation) farmHouse;
        who.Position = Utility.PointToVector2(farmHouse.GetPlayerBedSpot()) * 64f;
        BedFurniture.ShiftPositionForBed(who);
      }
    }
    if (who != Game1.player)
      return;
    Game1.currentLocation = who.currentLocation;
  }

  public static void ShiftPositionForBed(Farmer who)
  {
    GameLocation currentLocation = who.currentLocation;
    BedFurniture bedAtTile = BedFurniture.GetBedAtTile(currentLocation, (int) ((double) who.position.X / 64.0), (int) ((double) who.position.Y / 64.0));
    if (bedAtTile != null)
    {
      who.Position = Utility.PointToVector2(bedAtTile.GetBedSpot()) * 64f;
      if (bedAtTile.bedType != BedFurniture.BedType.Double)
      {
        if (currentLocation.map == null)
          currentLocation.reloadMap();
        if (!currentLocation.CanItemBePlacedHere(new Vector2(bedAtTile.TileLocation.X - 1f, bedAtTile.TileLocation.Y + 1f)))
        {
          who.faceDirection(3);
        }
        else
        {
          who.position.X -= 64f;
          who.faceDirection(1);
        }
      }
      else
      {
        bool flag = false;
        if (currentLocation is FarmHouse farmHouse && farmHouse.HasOwner)
        {
          long? spouse = farmHouse.owner.team.GetSpouse(farmHouse.owner.UniqueMultiplayerID);
          long uniqueMultiplayerId = who.UniqueMultiplayerID;
          if (spouse.GetValueOrDefault() == uniqueMultiplayerId & spouse.HasValue)
            flag = true;
          else if (farmHouse.owner != who && !farmHouse.owner.isMarriedOrRoommates())
            flag = true;
        }
        if (flag)
        {
          who.position.X += 64f;
          who.faceDirection(3);
        }
        else
        {
          who.position.X -= 64f;
          who.faceDirection(1);
        }
      }
    }
    who.position.Y += 32f;
    if (!(who.NetFields.Root is NetRoot<Farmer> root))
      return;
    // ISSUE: explicit non-virtual call
    __nonvirtual (root.CancelInterpolation());
  }

  public virtual bool CanModifyBed(Farmer who)
  {
    if (who == null)
      return false;
    GameLocation currentLocation = who.currentLocation;
    if (currentLocation == null)
      return false;
    if (currentLocation is FarmHouse farmHouse && farmHouse.owner != who)
    {
      long? spouse = farmHouse.owner.team.GetSpouse(farmHouse.owner.UniqueMultiplayerID);
      long uniqueMultiplayerId = who.UniqueMultiplayerID;
      if (!(spouse.GetValueOrDefault() == uniqueMultiplayerId & spouse.HasValue))
        return false;
    }
    return true;
  }

  public override int GetAdditionalFurniturePlacementStatus(
    GameLocation location,
    int x,
    int y,
    Farmer who = null)
  {
    if (this.bedType == BedFurniture.BedType.Double)
    {
      if (!IsBedsideClear(-1))
        return -1;
    }
    else if (!IsBedsideClear(-1) && !IsBedsideClear(this.getTilesWide()))
      return -1;
    return base.GetAdditionalFurniturePlacementStatus(location, x, y, who);

    bool IsBedsideClear(int offsetX)
    {
      Vector2 tile = new Vector2((float) (x / 64 /*0x40*/ + offsetX), (float) (y / 64 /*0x40*/ + 1));
      return location.CanItemBePlacedHere(tile, ignorePassablesExactly: true);
    }
  }

  /// <inheritdoc />
  public override bool placementAction(GameLocation location, int x, int y, Farmer who = null)
  {
    this._alreadyAttempingRemoval = false;
    this.Location = location;
    if (!this.CanModifyBed(who))
    {
      Game1.showRedMessage(Game1.content.LoadString("Strings\\UI:Bed_CantMoveOthersBeds"));
      return false;
    }
    if (!(location is FarmHouse farmHouse) || (this.bedType != BedFurniture.BedType.Child || farmHouse.upgradeLevel >= 2) && (this.bedType != BedFurniture.BedType.Double || farmHouse.upgradeLevel >= 1))
      return base.placementAction(location, x, y, who);
    Game1.showRedMessage(Game1.content.LoadString("Strings\\UI:Bed_NeedsUpgrade"));
    return false;
  }

  public override void performRemoveAction()
  {
    this._alreadyAttempingRemoval = false;
    base.performRemoveAction();
  }

  public override void hoverAction()
  {
    if (Game1.player.GetBoundingBox().Intersects(this.GetBoundingBox()))
      return;
    base.hoverAction();
  }

  public override bool canBeRemoved(Farmer who)
  {
    if (this.Location == null)
      return false;
    if (!this.CanModifyBed(who))
    {
      if (!Game1.player.GetBoundingBox().Intersects(this.GetBoundingBox()))
        Game1.showRedMessage(Game1.content.LoadString("Strings\\UI:Bed_CantMoveOthersBeds"));
      return false;
    }
    if (!this.IsBeingSleptIn())
      return true;
    if (!Game1.player.GetBoundingBox().Intersects(this.GetBoundingBox()))
      Game1.showRedMessage(Game1.content.LoadString("Strings\\UI:Bed_InUse"));
    return false;
  }

  /// <inheritdoc />
  protected override Item GetOneNew()
  {
    return (Item) new BedFurniture(this.ItemId, this.tileLocation.Value);
  }

  /// <inheritdoc />
  protected override void GetOneCopyFrom(Item source)
  {
    base.GetOneCopyFrom(source);
    if (!(source is BedFurniture bedFurniture))
      return;
    this.bedType = bedFurniture.bedType;
  }

  public virtual Point GetBedSpot()
  {
    return new Point((int) this.tileLocation.X + 1, (int) this.tileLocation.Y + 1);
  }

  /// <inheritdoc />
  public override void actionOnPlayerEntryOrPlacement(GameLocation environment, bool dropDown)
  {
    base.actionOnPlayerEntryOrPlacement(environment, dropDown);
    this.UpdateBedTile(false);
  }

  public virtual void UpdateBedTile(bool check_bounds)
  {
    Rectangle boundingBox = this.GetBoundingBox();
    if (this.bedType == BedFurniture.BedType.Double)
    {
      this.bedTileOffset = 1;
    }
    else
    {
      if (check_bounds && boundingBox.Intersects(Game1.player.GetBoundingBox()))
        return;
      if ((double) Game1.player.Position.X > (double) boundingBox.Center.X)
        this.bedTileOffset = 0;
      else
        this.bedTileOffset = 1;
    }
  }

  public override void updateWhenCurrentLocation(GameTime time)
  {
    if (this.Location != null)
    {
      this.mutex.Update(Game1.getOnlineFarmers());
      this.UpdateBedTile(true);
    }
    base.updateWhenCurrentLocation(time);
  }

  public override void draw(SpriteBatch spriteBatch, int x, int y, float alpha = 1f)
  {
    if (this.isTemporarilyInvisible)
      return;
    if (Furniture.isDrawingLocationFurniture)
    {
      ParsedItemData dataOrErrorItem = ItemRegistry.GetDataOrErrorItem(this.QualifiedItemId);
      Texture2D texture = dataOrErrorItem.GetTexture();
      Rectangle sourceRect = dataOrErrorItem.GetSourceRect();
      spriteBatch.Draw(texture, Game1.GlobalToLocal(Game1.viewport, this.drawPosition.Value + (this.shakeTimer > 0 ? new Vector2((float) Game1.random.Next(-1, 2), (float) Game1.random.Next(-1, 2)) : Vector2.Zero)), new Rectangle?(sourceRect), Color.White * alpha, 0.0f, Vector2.Zero, 4f, this.flipped.Value ? SpriteEffects.FlipHorizontally : SpriteEffects.None, (float) (this.boundingBox.Value.Top + 1) / 10000f);
      sourceRect.X += sourceRect.Width;
      spriteBatch.Draw(texture, Game1.GlobalToLocal(Game1.viewport, this.drawPosition.Value + (this.shakeTimer > 0 ? new Vector2((float) Game1.random.Next(-1, 2), (float) Game1.random.Next(-1, 2)) : Vector2.Zero)), new Rectangle?(sourceRect), Color.White * alpha, 0.0f, Vector2.Zero, 4f, this.flipped.Value ? SpriteEffects.FlipHorizontally : SpriteEffects.None, (float) (this.boundingBox.Value.Bottom - 1) / 10000f);
    }
    else
      base.draw(spriteBatch, x, y, alpha);
  }

  public override bool AllowPlacementOnThisTile(int x, int y)
  {
    return this.bedType == BedFurniture.BedType.Child && (double) y == (double) this.TileLocation.Y + 1.0 || base.AllowPlacementOnThisTile(x, y);
  }

  public override bool IntersectsForCollision(Rectangle rect)
  {
    Rectangle boundingBox = this.GetBoundingBox();
    if ((boundingBox with { Height = 64 /*0x40*/ }).Intersects(rect))
      return true;
    Rectangle rectangle = boundingBox;
    rectangle.Y += 128 /*0x80*/;
    rectangle.Height -= 128 /*0x80*/;
    return rectangle.Intersects(rect);
  }

  public override int GetAdditionalTilePropertyRadius() => 1;

  /// <summary>Get whether a given tile position contains a bed that can be slept in (e.g. bed furniture or map bed).</summary>
  /// <param name="location">The location to check.</param>
  /// <param name="x">The tile X position to check.</param>
  /// <param name="y">The tile Y position to check.</param>
  public static bool IsBedHere(GameLocation location, int x, int y)
  {
    if (location == null)
      return false;
    BedFurniture.ignoreContextualBedSpotOffset = true;
    if (location.doesTileHaveProperty(x, y, "Bed", "Back") != null)
    {
      BedFurniture.ignoreContextualBedSpotOffset = false;
      return true;
    }
    BedFurniture.ignoreContextualBedSpotOffset = false;
    return false;
  }

  public override bool DoesTileHaveProperty(
    int tile_x,
    int tile_y,
    string property_name,
    string layer_name,
    ref string property_value)
  {
    if (this.bedType == BedFurniture.BedType.Double && (double) tile_x == (double) this.tileLocation.X - 1.0 && (double) tile_y == (double) this.tileLocation.Y + 1.0 && layer_name == "Back" && property_name == "NoFurniture")
    {
      property_value = "T";
      return true;
    }
    if ((double) tile_x >= (double) this.tileLocation.X && (double) tile_x < (double) this.tileLocation.X + (double) this.getTilesWide() && (double) tile_y == (double) this.tileLocation.Y + 1.0 && layer_name == "Back")
    {
      if (property_name == "Bed")
      {
        property_value = "T";
        return true;
      }
      if (this.bedType != BedFurniture.BedType.Child)
      {
        int num = (int) this.tileLocation.X + this.bedTileOffset;
        if (BedFurniture.ignoreContextualBedSpotOffset)
          num = (int) this.tileLocation.X + 1;
        if (tile_x == num && property_name == "TouchAction")
        {
          property_value = "Sleep";
          return true;
        }
      }
    }
    return false;
  }

  public enum BedType
  {
    Any = -1, // 0xFFFFFFFF
    Single = 0,
    Double = 1,
    Child = 2,
  }
}
