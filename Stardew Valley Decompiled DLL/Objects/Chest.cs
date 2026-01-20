// Decompiled with JetBrains decompiler
// Type: StardewValley.Objects.Chest
// Assembly: Stardew Valley, Version=1.6.15.24356, Culture=neutral, PublicKeyToken=null
// MVID: AA1F513A-94F0-4EF6-A35F-2D5B4A3721BF
// Assembly location: D:\SteamLibrary\steamapps\common\Stardew Valley\Stardew Valley.dll
// XML documentation location: D:\SteamLibrary\steamapps\common\Stardew Valley\Stardew Valley.xml

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Netcode;
using StardewValley.Delegates;
using StardewValley.Internal;
using StardewValley.Inventories;
using StardewValley.ItemTypeDefinitions;
using StardewValley.Locations;
using StardewValley.Menus;
using StardewValley.Network;
using StardewValley.Network.ChestHit;
using StardewValley.Tools;
using System;
using System.Collections.Generic;
using System.Xml.Serialization;

#nullable disable
namespace StardewValley.Objects;

public class Chest : StardewValley.Object
{
  public const int capacity = 36;
  /// <summary>The underlying <see cref="T:StardewValley.Network.ChestHit.ChestHitTimer" /> instance used by <see cref="P:StardewValley.Objects.Chest.HitTimerInstance" />.</summary>
  internal ChestHitTimer hitTimerInstance;
  [XmlElement("currentLidFrame")]
  public readonly NetInt startingLidFrame = new NetInt(501);
  public readonly NetInt lidFrameCount = new NetInt(5);
  private int currentLidFrame;
  [XmlElement("frameCounter")]
  public readonly NetInt frameCounter = new NetInt(-1);
  /// <summary>The backing field for <see cref="P:StardewValley.Objects.Chest.Items" />.</summary>
  [XmlElement("items")]
  public NetRef<Inventory> netItems = new NetRef<Inventory>(new Inventory());
  public readonly NetLongDictionary<Inventory, NetRef<Inventory>> separateWalletItems = new NetLongDictionary<Inventory, NetRef<Inventory>>();
  [XmlElement("tint")]
  public readonly NetColor tint = new NetColor(Color.White);
  [XmlElement("playerChoiceColor")]
  public readonly NetColor playerChoiceColor = new NetColor(Color.Black);
  [XmlElement("playerChest")]
  public readonly NetBool playerChest = new NetBool();
  [XmlElement("fridge")]
  public readonly NetBool fridge = new NetBool();
  /// <summary>Whether this is a gift box. This changes the chest's appearance, and when the player interacts with the chest they'll receive all the items directly and the chest will disappear.</summary>
  [XmlElement("giftbox")]
  public readonly NetBool giftbox = new NetBool();
  /// <summary>If <see cref="F:StardewValley.Objects.Chest.giftbox" /> is true, the sprite index to draw from the <see cref="F:StardewValley.Game1.giftboxName" /> texture.</summary>
  [XmlElement("giftboxIndex")]
  public readonly NetInt giftboxIndex = new NetInt();
  /// <summary>If <see cref="F:StardewValley.Objects.Chest.giftbox" /> is true, whether this is the starter gift for a player in their cabin or farmhouse.</summary>
  public readonly NetBool giftboxIsStarterGift = new NetBool();
  [XmlElement("spriteIndexOverride")]
  public readonly NetInt bigCraftableSpriteIndex = new NetInt(-1);
  [XmlElement("dropContents")]
  public readonly NetBool dropContents = new NetBool(false);
  [XmlIgnore]
  public string mailToAddOnItemDump;
  [XmlElement("synchronized")]
  public readonly NetBool synchronized = new NetBool(false);
  [XmlIgnore]
  public int _shippingBinFrameCounter;
  [XmlIgnore]
  public bool _farmerNearby;
  [XmlIgnore]
  public NetVector2 kickStartTile = new NetVector2(new Vector2(-1000f, -1000f));
  [XmlIgnore]
  public Vector2? localKickStartTile;
  [XmlIgnore]
  public float kickProgress = -1f;
  [XmlIgnore]
  public readonly NetEvent0 openChestEvent = new NetEvent0();
  [XmlElement("specialChestType")]
  public readonly NetEnum<Chest.SpecialChestTypes> specialChestType = new NetEnum<Chest.SpecialChestTypes>();
  /// <summary>The backing field for <see cref="P:StardewValley.Objects.Chest.GlobalInventoryId" />.</summary>
  public readonly NetString globalInventoryId = new NetString();
  [XmlIgnore]
  public readonly NetMutex mutex = new NetMutex();

  /// <summary>A read-only <see cref="T:StardewValley.Network.ChestHit.ChestHitTimer" /> that is automatically created or fetched from <see cref="F:StardewValley.Network.ChestHit.ChestHitSynchronizer.SavedTimers" />.</summary>
  private ChestHitTimer HitTimerInstance
  {
    get
    {
      if (this.hitTimerInstance != null)
        return this.hitTimerInstance;
      this.hitTimerInstance = new ChestHitTimer();
      Dictionary<ulong, ChestHitTimer> dictionary;
      if (Game1.IsMasterGame || this.Location == null || !Game1.player.team.chestHit.SavedTimers.TryGetValue(this.Location.NameOrUniqueName, out dictionary))
        return this.hitTimerInstance;
      ulong key = ChestHitSynchronizer.HashPosition((int) this.TileLocation.X, (int) this.TileLocation.Y);
      ChestHitTimer chestHitTimer;
      if (dictionary.TryGetValue(key, out chestHitTimer))
      {
        this.hitTimerInstance = chestHitTimer;
        dictionary.Remove(key);
        if (chestHitTimer.SavedTime >= 0 && Game1.currentGameTime != null)
        {
          chestHitTimer.Milliseconds -= (int) Game1.currentGameTime.TotalGameTime.TotalMilliseconds - chestHitTimer.SavedTime;
          chestHitTimer.SavedTime = -1;
        }
      }
      return this.hitTimerInstance;
    }
  }

  [XmlIgnore]
  public Chest.SpecialChestTypes SpecialChestType
  {
    get => this.specialChestType.Value;
    set => this.specialChestType.Value = value;
  }

  /// <summary>If set, the inventory ID in <see cref="F:StardewValley.FarmerTeam.globalInventories" /> to use for this chest instead of its local item list.</summary>
  [XmlIgnore]
  public string GlobalInventoryId
  {
    get => this.globalInventoryId.Value;
    set => this.globalInventoryId.Value = value;
  }

  [XmlIgnore]
  public Color Tint
  {
    get => this.tint.Value;
    set => this.tint.Value = value;
  }

  [XmlIgnore]
  public Inventory Items => this.netItems.Value;

  /// <inheritdoc />
  protected override void initNetFields()
  {
    base.initNetFields();
    this.NetFields.AddField((INetSerializable) this.startingLidFrame, "startingLidFrame").AddField((INetSerializable) this.frameCounter, "frameCounter").AddField((INetSerializable) this.netItems, "netItems").AddField((INetSerializable) this.tint, "tint").AddField((INetSerializable) this.playerChoiceColor, "playerChoiceColor").AddField((INetSerializable) this.playerChest, "playerChest").AddField((INetSerializable) this.fridge, "fridge").AddField((INetSerializable) this.giftbox, "giftbox").AddField((INetSerializable) this.giftboxIndex, "giftboxIndex").AddField((INetSerializable) this.giftboxIsStarterGift, "giftboxIsStarterGift").AddField((INetSerializable) this.mutex.NetFields, "mutex.NetFields").AddField((INetSerializable) this.lidFrameCount, "lidFrameCount").AddField((INetSerializable) this.bigCraftableSpriteIndex, "bigCraftableSpriteIndex").AddField((INetSerializable) this.dropContents, "dropContents").AddField(this.openChestEvent.NetFields, "openChestEvent.NetFields").AddField((INetSerializable) this.synchronized, "synchronized").AddField((INetSerializable) this.specialChestType, "specialChestType").AddField((INetSerializable) this.kickStartTile, "kickStartTile").AddField((INetSerializable) this.separateWalletItems, "separateWalletItems").AddField((INetSerializable) this.globalInventoryId, "globalInventoryId");
    this.openChestEvent.onEvent += new NetEvent0.Event(this.performOpenChest);
    this.kickStartTile.fieldChangeVisibleEvent += (FieldChange<NetVector2, Vector2>) ((field, old_value, new_value) =>
    {
      if (Game1.gameMode == (byte) 6 || (double) new_value.X == -1000.0 || (double) new_value.Y == -1000.0)
        return;
      this.localKickStartTile = new Vector2?(this.kickStartTile.Value);
      this.kickProgress = 0.0f;
    });
  }

  public Chest()
  {
    this.Name = nameof (Chest);
    this.type.Value = "interactive";
  }

  public Chest(bool playerChest, Vector2 tileLocation, string itemId = "130")
    : base(tileLocation, itemId)
  {
    this.Name = nameof (Chest);
    this.type.Value = "Crafting";
    if (playerChest)
    {
      this.playerChest.Value = playerChest;
      this.startingLidFrame.Value = this.ParentSheetIndex + 1;
      this.bigCraftable.Value = true;
      this.canBeSetDown.Value = true;
    }
    else
      this.lidFrameCount.Value = 3;
    this.SetSpecialChestType();
  }

  public Chest(bool playerChest, string itemId = "130")
    : base(Vector2.Zero, itemId)
  {
    this.Name = nameof (Chest);
    this.type.Value = "Crafting";
    if (playerChest)
    {
      this.playerChest.Value = playerChest;
      this.startingLidFrame.Value = this.ParentSheetIndex + 1;
      this.bigCraftable.Value = true;
      this.canBeSetDown.Value = true;
    }
    else
      this.lidFrameCount.Value = 3;
  }

  public Chest(string itemId, Vector2 tile_location, int starting_lid_frame, int lid_frame_count)
    : base(tile_location, itemId)
  {
    this.playerChest.Value = true;
    this.startingLidFrame.Value = starting_lid_frame;
    this.lidFrameCount.Value = lid_frame_count;
    this.bigCraftable.Value = true;
    this.canBeSetDown.Value = true;
  }

  public Chest(
    List<Item> items,
    Vector2 location,
    bool giftbox = false,
    int giftboxIndex = 0,
    bool giftboxIsStarterGift = false)
  {
    this.name = nameof (Chest);
    this.type.Value = "interactive";
    this.giftbox.Value = giftbox;
    this.giftboxIndex.Value = giftboxIndex;
    this.giftboxIsStarterGift.Value = giftboxIsStarterGift;
    if (!this.giftbox.Value)
      this.lidFrameCount.Value = 3;
    if (items != null)
      this.Items.OverwriteWith((IList<Item>) items);
    this.TileLocation = location;
  }

  public void resetLidFrame() => this.currentLidFrame = this.startingLidFrame.Value;

  public void fixLidFrame()
  {
    if (this.currentLidFrame == 0)
      this.currentLidFrame = this.startingLidFrame.Value;
    if (this.SpecialChestType == Chest.SpecialChestTypes.MiniShippingBin)
      return;
    if (this.playerChest.Value)
    {
      if (this.GetMutex().IsLocked() && !this.GetMutex().IsLockHeld())
      {
        this.currentLidFrame = this.getLastLidFrame();
      }
      else
      {
        if (this.GetMutex().IsLocked())
          return;
        this.currentLidFrame = this.startingLidFrame.Value;
      }
    }
    else
    {
      if (this.currentLidFrame != this.startingLidFrame.Value || !this.GetMutex().IsLocked() || this.GetMutex().IsLockHeld())
        return;
      this.currentLidFrame = this.getLastLidFrame();
    }
  }

  public int getLastLidFrame() => this.startingLidFrame.Value + this.lidFrameCount.Value - 1;

  /// <summary>Handles a player hitting this chest.</summary>
  /// <param name="args">The arguments for the chest hit event.</param>
  public void HandleChestHit(ChestHitArgs args)
  {
    if (!Game1.IsMasterGame)
    {
      Game1.log.Warn("Attempted to call Chest::HandleChestHit as a farmhand.");
    }
    else
    {
      if ((double) this.TileLocation.X == 0.0 && (double) this.TileLocation.Y == 0.0)
        this.TileLocation = Utility.PointToVector2(args.ChestTile);
      this.GetMutex().RequestLock((Action) (() =>
      {
        this.clearNulls();
        if (this.isEmpty())
        {
          this.performRemoveAction();
          if (this.Location.Objects.Remove(Utility.PointToVector2(args.ChestTile)) && this.Type == "Crafting" && this.fragility.Value != 2)
            this.Location.debris.Add(new Debris(this.QualifiedItemId, args.ToolPosition, Utility.PointToVector2(args.StandingPixel)));
          Game1.player.team.chestHit.SignalDelete(this.Location, args.ChestTile.X, args.ChestTile.Y);
        }
        else if (args.ToolCanHit)
        {
          if (args.HoldDownClick || args.RecentlyHit)
          {
            if (this.kickStartTile.Value == this.TileLocation)
              this.kickStartTile.Value = new Vector2(-1000f, -1000f);
            this.TryMoveToSafePosition(new int?(args.Direction));
            Game1.player.team.chestHit.SignalMove(this.Location, args.ChestTile.X, args.ChestTile.Y, (int) this.TileLocation.X, (int) this.TileLocation.Y);
          }
          else
            this.kickStartTile.Value = this.TileLocation;
        }
        this.GetMutex().ReleaseLock();
      }));
    }
  }

  public override bool performToolAction(Tool t)
  {
    if (t?.getLastFarmerToUse() != null && t.getLastFarmerToUse() != Game1.player)
      return false;
    if (this.playerChest.Value)
    {
      if (t == null || t is MeleeWeapon || !t.isHeavyHitter() || !base.performToolAction(t))
        return false;
      GameLocation location = this.Location;
      Farmer lastFarmerToUse = t.getLastFarmerToUse();
      if (lastFarmerToUse != null)
      {
        Vector2 vector2 = this.TileLocation;
        if ((double) vector2.X == 0.0 && (double) vector2.Y == 0.0)
        {
          bool flag = false;
          foreach (KeyValuePair<Vector2, StardewValley.Object> pair in location.objects.Pairs)
          {
            if (pair.Value == this)
            {
              vector2.X = (float) (int) pair.Key.X;
              vector2.Y = (float) (int) pair.Key.Y;
              flag = true;
              break;
            }
          }
          if (!flag)
          {
            vector2 = lastFarmerToUse.GetToolLocation() / 64f;
            vector2.X = (float) (int) vector2.X;
            vector2.Y = (float) (int) vector2.Y;
          }
        }
        if (!this.GetMutex().IsLocked())
        {
          ChestHitArgs args = new ChestHitArgs();
          args.Location = location;
          args.ChestTile = new Point((int) this.TileLocation.X, (int) this.TileLocation.Y);
          args.ToolPosition = lastFarmerToUse.GetToolLocation();
          args.StandingPixel = lastFarmerToUse.StandingPixel;
          args.Direction = lastFarmerToUse.FacingDirection;
          args.HoldDownClick = t != lastFarmerToUse.CurrentTool;
          args.ToolCanHit = t.isHeavyHitter() && !(t is MeleeWeapon);
          args.RecentlyHit = this.HitTimerInstance.Milliseconds > 0;
          if (args.ToolCanHit)
          {
            this.shakeTimer = 100;
            this.HitTimerInstance.Milliseconds = 10000;
          }
          if (args.ChestTile.X == 0 && args.ChestTile.Y == 0)
          {
            if (location.getObjectAtTile((int) vector2.X, (int) vector2.Y) != this)
              return false;
            args.ChestTile = new Point((int) vector2.X, (int) vector2.Y);
          }
          Game1.player.team.chestHit.Sync(args);
        }
      }
      return false;
    }
    if (!(t is Pickaxe) || this.currentLidFrame != this.getLastLidFrame() || this.frameCounter.Value != -1 || !this.isEmpty())
      return false;
    this.Location.playSound("woodWhack");
    for (int index = 0; index < 8; ++index)
      Game1.multiplayer.broadcastSprites(this.Location, new TemporaryAnimatedSprite("LooseSprites\\Cursors", Game1.random.NextDouble() < 0.5 ? new Microsoft.Xna.Framework.Rectangle(598, 1275, 13, 4) : new Microsoft.Xna.Framework.Rectangle(598, 1275, 13, 4), 999f, 1, 0, this.tileLocation.Value * 64f + new Vector2(32f, 64f), false, Game1.random.NextDouble() < 0.5, (float) (((double) this.tileLocation.Y * 64.0 + 64.0) / 10000.0), 0.01f, new Color(204, 132, 87), 4f, 0.0f, (float) ((double) Game1.random.Next(-5, 6) * 3.1415927410125732 / 8.0), (float) ((double) Game1.random.Next(-5, 6) * 3.1415927410125732 / 64.0))
      {
        motion = new Vector2((float) Game1.random.Next(-25, 26) / 10f, (float) Game1.random.Next(-11, -8)),
        acceleration = new Vector2(0.0f, 0.3f)
      });
    Game1.createRadialDebris(this.Location, 12, (int) this.tileLocation.X, (int) this.tileLocation.Y, Game1.random.Next(4, 7), false, color: new Color?(new Color(204, 132, 87)));
    return true;
  }

  /// <summary>Try to shove this chest onto an unoccupied nearby tile.</summary>
  /// <param name="preferDirection">The direction in which to move the chest if possible, matching a constant like <see cref="F:StardewValley.Game1.up" />.</param>
  /// <returns>Returns whether the chest was successfully moved to an unoccupied space.</returns>
  public bool TryMoveToSafePosition(int? preferDirection = null)
  {
    GameLocation location = this.Location;
    Vector2? prioritize_direction;
    if (preferDirection.HasValue)
    {
      switch (preferDirection.GetValueOrDefault())
      {
        case 0:
          prioritize_direction = new Vector2?(new Vector2(0.0f, -1f));
          goto label_6;
        case 1:
          prioritize_direction = new Vector2?(new Vector2(1f, 0.0f));
          goto label_6;
        case 3:
          prioritize_direction = new Vector2?(new Vector2(-1f, 0.0f));
          goto label_6;
      }
    }
    prioritize_direction = new Vector2?(new Vector2(0.0f, 1f));
label_6:
    return TryMoveRecursively(this.tileLocation.Value, 0, prioritize_direction);

    bool TryMoveRecursively(Vector2 tile_position, int depth, Vector2? prioritize_direction)
    {
      List<Vector2> list = new List<Vector2>();
      list.AddRange((IEnumerable<Vector2>) new Vector2[4]
      {
        new Vector2(1f, 0.0f),
        new Vector2(-1f, 0.0f),
        new Vector2(0.0f, -1f),
        new Vector2(0.0f, 1f)
      });
      Utility.Shuffle<Vector2>(Game1.random, list);
      if (prioritize_direction.HasValue)
      {
        list.Remove(-prioritize_direction.Value);
        list.Insert(0, -prioritize_direction.Value);
        list.Remove(prioritize_direction.Value);
        list.Insert(0, prioritize_direction.Value);
      }
      foreach (Vector2 vector2_1 in list)
      {
        Vector2 vector2_2 = tile_position + vector2_1;
        if (this.canBePlacedHere(location, vector2_2, CollisionMask.All, false) && location.CanItemBePlacedHere(vector2_2))
        {
          if (!location.objects.ContainsKey(vector2_2) && location.objects.Remove(this.TileLocation))
          {
            this.kickStartTile.Value = this.TileLocation;
            this.TileLocation = vector2_2;
            location.objects[vector2_2] = (StardewValley.Object) this;
          }
          return true;
        }
      }
      Utility.Shuffle<Vector2>(Game1.random, list);
      if (prioritize_direction.HasValue)
      {
        list.Remove(-prioritize_direction.Value);
        list.Insert(0, -prioritize_direction.Value);
        list.Remove(prioritize_direction.Value);
        list.Insert(0, prioritize_direction.Value);
      }
      if (depth < 3)
      {
        foreach (Vector2 vector2 in list)
        {
          Vector2 tile_position1 = tile_position + vector2;
          if (location.isPointPassable(new xTile.Dimensions.Location((int) ((double) tile_position1.X + 0.5) * 64 /*0x40*/, (int) ((double) tile_position1.Y + 0.5) * 64 /*0x40*/), Game1.viewport) && TryMoveRecursively(tile_position1, depth + 1, prioritize_direction))
            return true;
        }
      }
      return false;
    }
  }

  /// <inheritdoc />
  public override bool placementAction(GameLocation location, int x, int y, Farmer who = null)
  {
    this.localKickStartTile = new Vector2?();
    this.kickProgress = -1f;
    return base.placementAction(location, x, y, who);
  }

  /// <summary>Set the special chest type based on the chest's item ID.</summary>
  public void SetSpecialChestType()
  {
    switch (this.QualifiedItemId)
    {
      case "(BC)BigChest":
      case "(BC)BigStoneChest":
        this.SpecialChestType = Chest.SpecialChestTypes.BigChest;
        break;
      case "(BC)248":
        this.SpecialChestType = Chest.SpecialChestTypes.MiniShippingBin;
        break;
      case "(BC)256":
        this.SpecialChestType = Chest.SpecialChestTypes.JunimoChest;
        break;
      case "(BC)275":
        this.SpecialChestType = Chest.SpecialChestTypes.AutoLoader;
        break;
    }
  }

  public void destroyAndDropContents(Vector2 pointToDropAt)
  {
    GameLocation location = this.Location;
    if (location == null)
      return;
    List<Item> objList = new List<Item>();
    objList.AddRange((IEnumerable<Item>) this.Items);
    if (this.SpecialChestType == Chest.SpecialChestTypes.MiniShippingBin)
    {
      foreach (Inventory collection in this.separateWalletItems.Values)
        objList.AddRange((IEnumerable<Item>) collection);
    }
    if (objList.Count > 0)
      location.playSound("throwDownITem");
    foreach (Item obj in objList)
    {
      if (obj != null)
        Game1.createItemDebris(obj, pointToDropAt, Game1.random.Next(4), location);
    }
    this.Items.Clear();
    this.separateWalletItems.Clear();
    this.clearNulls();
  }

  /// <inheritdoc />
  public override bool performObjectDropInAction(
    Item dropInItem,
    bool probe,
    Farmer who,
    bool returnFalseIfItemConsumed = false)
  {
    if (dropInItem == null || !(dropInItem.QualifiedItemId != this.QualifiedItemId) || !dropInItem.HasContextTag("swappable_chest") || !this.HasContextTag("swappable_chest") || this.Location == null)
      return base.performObjectDropInAction(dropInItem, probe, who);
    if (!probe)
    {
      if (this.GetMutex().IsLocked())
        return false;
      Chest chest = new Chest(true, this.TileLocation, dropInItem.ItemId);
      int actualCapacity = chest.GetActualCapacity();
      if (actualCapacity < this.GetActualCapacity() && actualCapacity < this.Items.CountItemStacks())
        return false;
      if (actualCapacity < this.Items.Count)
        this.clearNulls();
      chest.netItems.Value = this.netItems.Value;
      chest.playerChoiceColor.Value = this.playerChoiceColor.Value;
      chest.Tint = this.Tint;
      chest.modData.CopyFrom(this.modData);
      GameLocation location = this.Location;
      location.Objects.Remove(this.TileLocation);
      location.Objects.Add(this.TileLocation, (StardewValley.Object) chest);
      Game1.createMultipleItemDebris(ItemRegistry.Create(this.QualifiedItemId), this.TileLocation * 64f + new Vector2(32f), -1);
      this.Location.playSound("axchop");
    }
    return true;
  }

  public void dumpContents()
  {
    GameLocation location = this.Location;
    if (location == null)
      return;
    IInventory items = (IInventory) this.Items;
    if (this.synchronized.Value && (this.GetMutex().IsLocked() || !Game1.IsMasterGame) && !this.GetMutex().IsLockHeld())
      return;
    if (items.Count > 0 && (this.GetMutex().IsLockHeld() || !this.playerChest.Value))
    {
      if (this.giftbox.Value && this.giftboxIsStarterGift.Value && location is FarmHouse farmHouse)
      {
        if (!farmHouse.IsOwnedByCurrentPlayer)
        {
          Game1.drawObjectDialogue(Game1.content.LoadString("Strings\\Objects:ParsnipSeedPackage_SomeoneElse"));
          return;
        }
        Game1.player.addQuest(Game1.GetFarmTypeID() == "MeadowlandsFarm" ? "132" : "6");
        Game1.dayTimeMoneyBox.PingQuestLog();
      }
      foreach (Item obj in (IEnumerable<Item>) items)
      {
        if (obj != null)
        {
          obj.SetTempData<bool>("FromStarterGiftBox", true);
          if (obj.QualifiedItemId == "(O)434")
          {
            if (Game1.player.mailReceived.Add(location is FarmHouse ? "CF_Spouse" : "CF_Mines"))
              Game1.player.eatObject(items[0] as StardewValley.Object, true);
          }
          else if (this.dropContents.Value)
          {
            Game1.createItemDebris(obj, this.tileLocation.Value * 64f, -1, location);
            if (location is VolcanoDungeon)
            {
              switch (this.bigCraftableSpriteIndex.Value)
              {
                case 223:
                  Game1.player.team.RequestLimitedNutDrops("VolcanoNormalChest", location, (int) this.tileLocation.Value.X * 64 /*0x40*/, (int) this.tileLocation.Value.Y * 64 /*0x40*/, 1);
                  continue;
                case 227:
                  Game1.player.team.RequestLimitedNutDrops("VolcanoRareChest", location, (int) this.tileLocation.Value.X * 64 /*0x40*/, (int) this.tileLocation.Value.Y * 64 /*0x40*/, 1);
                  continue;
                default:
                  continue;
              }
            }
          }
          else if (!this.synchronized.Value || this.GetMutex().IsLockHeld())
          {
            obj.onDetachedFromParent();
            if (Game1.activeClickableMenu is ItemGrabMenu activeClickableMenu)
              activeClickableMenu.ItemsToGrabMenu.actualInventory.Add(obj);
            else
              Game1.player.addItemByMenuIfNecessaryElseHoldUp(obj);
            if (this.mailToAddOnItemDump != null)
              Game1.player.mailReceived.Add(this.mailToAddOnItemDump);
            if (location is Caldera || Game1.player.currentLocation is Caldera)
              Game1.player.mailReceived.Add("CalderaTreasure");
          }
        }
      }
      items.Clear();
      this.clearNulls();
      Game1.mine?.chestConsumed();
      ItemGrabMenu grabMenu = Game1.activeClickableMenu as ItemGrabMenu;
      if (grabMenu != null)
      {
        ItemGrabMenu itemGrabMenu = grabMenu;
        itemGrabMenu.behaviorBeforeCleanup = itemGrabMenu.behaviorBeforeCleanup + (Action<IClickableMenu>) (_ => grabMenu.DropRemainingItems());
      }
    }
    Game1.player.gainExperience(5, 25 + Game1.CurrentMineLevel);
    if (!this.giftbox.Value)
      return;
    TemporaryAnimatedSprite temporaryAnimatedSprite = new TemporaryAnimatedSprite("LooseSprites\\Giftbox", new Microsoft.Xna.Framework.Rectangle(0, this.giftboxIndex.Value * 32 /*0x20*/, 16 /*0x10*/, 32 /*0x20*/), 80f, 11, 1, this.tileLocation.Value * 64f - new Vector2(0.0f, 52f), false, false, this.tileLocation.Y / 10000f, 0.0f, Color.White, 4f, 0.0f, 0.0f, 0.0f)
    {
      destroyable = false,
      holdLastFrame = true
    };
    StardewValley.Object @object;
    if (location.netObjects.TryGetValue(this.tileLocation.Value, out @object) && @object == this)
    {
      Game1.multiplayer.broadcastSprites(location, temporaryAnimatedSprite);
      location.removeObject(this.tileLocation.Value, false);
    }
    else
      location.temporarySprites.Add(temporaryAnimatedSprite);
  }

  public NetMutex GetMutex()
  {
    if (this.GlobalInventoryId != null)
      return Game1.player.team.GetOrCreateGlobalInventoryMutex(this.GlobalInventoryId);
    return this.specialChestType.Value == Chest.SpecialChestTypes.JunimoChest ? Game1.player.team.GetOrCreateGlobalInventoryMutex("JunimoChests") : this.mutex;
  }

  /// <inheritdoc />
  public override bool checkForAction(Farmer who, bool justCheckingForActivity = false)
  {
    if (justCheckingForActivity)
      return true;
    GameLocation location = this.Location;
    IInventory itemsForPlayer = this.GetItemsForPlayer();
    if (this.giftbox.Value)
    {
      Game1.player.Halt();
      Game1.player.freezePause = 1000;
      location.playSound("Ship");
      this.dumpContents();
    }
    else if (this.playerChest.Value)
    {
      if (!Game1.didPlayerJustRightClick(true))
        return false;
      this.GetMutex().RequestLock((Action) (() =>
      {
        if (this.SpecialChestType == Chest.SpecialChestTypes.MiniShippingBin)
        {
          this.OpenMiniShippingMenu();
        }
        else
        {
          this.frameCounter.Value = 5;
          Game1.playSound(this.fridge.Value ? "doorCreak" : "openChest");
          Game1.player.Halt();
          Game1.player.freezePause = 1000;
        }
      }));
    }
    else if (!this.playerChest.Value)
    {
      if (this.currentLidFrame == this.startingLidFrame.Value && this.frameCounter.Value <= -1)
      {
        location.playSound("openChest");
        if (this.synchronized.Value)
          this.GetMutex().RequestLock(new Action(this.openChestEvent.Fire));
        else
          this.performOpenChest();
      }
      else if (this.currentLidFrame == this.getLastLidFrame() && itemsForPlayer.Count > 0 && !this.synchronized.Value)
      {
        Item obj = itemsForPlayer[0];
        itemsForPlayer.RemoveAt(0);
        if (Game1.mine != null)
          Game1.mine.chestConsumed();
        who.addItemByMenuIfNecessaryElseHoldUp(obj);
        ItemGrabMenu grab_menu = Game1.activeClickableMenu as ItemGrabMenu;
        if (grab_menu != null)
        {
          ItemGrabMenu itemGrabMenu = grab_menu;
          itemGrabMenu.behaviorBeforeCleanup = itemGrabMenu.behaviorBeforeCleanup + (Action<IClickableMenu>) (menu => grab_menu.DropRemainingItems());
        }
      }
    }
    if (itemsForPlayer.Count == 0 && (!this.playerChest.Value || this.giftbox.Value))
    {
      location.removeObject(this.TileLocation, false);
      location.playSound("woodWhack");
      for (int index = 0; index < 8; ++index)
        Game1.multiplayer.broadcastSprites(this.Location, new TemporaryAnimatedSprite("LooseSprites\\Cursors", Game1.random.NextDouble() < 0.5 ? new Microsoft.Xna.Framework.Rectangle(598, 1275, 13, 4) : new Microsoft.Xna.Framework.Rectangle(598, 1275, 13, 4), 999f, 1, 0, this.tileLocation.Value * 64f + new Vector2(32f, 64f), false, Game1.random.NextDouble() < 0.5, (float) (((double) this.tileLocation.Y * 64.0 + 64.0) / 10000.0), 0.01f, new Color(204, 132, 87), 4f, 0.0f, (float) ((double) Game1.random.Next(-5, 6) * 3.1415927410125732 / 8.0), (float) ((double) Game1.random.Next(-5, 6) * 3.1415927410125732 / 64.0))
        {
          motion = new Vector2((float) Game1.random.Next(-25, 26) / 10f, (float) Game1.random.Next(-11, -8)),
          acceleration = new Vector2(0.0f, 0.3f)
        });
      Game1.createRadialDebris(location, 12, (int) this.tileLocation.X, (int) this.tileLocation.Y, Game1.random.Next(4, 7), false, color: new Color?(new Color(204, 132, 87)));
    }
    return true;
  }

  public virtual void OpenMiniShippingMenu()
  {
    Game1.playSound("shwip");
    this.ShowMenu();
  }

  public virtual void performOpenChest() => this.frameCounter.Value = 5;

  public virtual void grabItemFromChest(Item item, Farmer who)
  {
    if (!who.couldInventoryAcceptThisItem(item))
      return;
    this.GetItemsForPlayer().Remove(item);
    this.clearNulls();
    this.ShowMenu();
  }

  public virtual Item addItem(Item item)
  {
    item.resetState();
    this.clearNulls();
    IInventory itemsForPlayer = this.GetItemsForPlayer();
    for (int index = 0; index < itemsForPlayer.Count; ++index)
    {
      if (itemsForPlayer[index] != null && itemsForPlayer[index].canStackWith((ISalable) item))
      {
        int amount = item.Stack - itemsForPlayer[index].addToStack(item);
        if (item.ConsumeStack(amount) == null)
          return (Item) null;
      }
    }
    if (itemsForPlayer.Count >= this.GetActualCapacity())
      return item;
    itemsForPlayer.Add(item);
    return (Item) null;
  }

  public virtual int GetActualCapacity()
  {
    switch (this.SpecialChestType)
    {
      case Chest.SpecialChestTypes.MiniShippingBin:
      case Chest.SpecialChestTypes.JunimoChest:
        return 9;
      case Chest.SpecialChestTypes.Enricher:
        return 1;
      case Chest.SpecialChestTypes.BigChest:
        return 70;
      default:
        return 36;
    }
  }

  /// <summary>If there's an object below this chest, try to auto-load its inventory from this chest.</summary>
  /// <param name="who">The player who interacted with the chest.</param>
  public virtual void CheckAutoLoad(Farmer who)
  {
    GameLocation location = this.Location;
    Vector2 tileLocation = this.TileLocation;
    StardewValley.Object @object;
    if (location == null || !location.objects.TryGetValue(new Vector2(tileLocation.X, tileLocation.Y + 1f), out @object) || @object == null)
      return;
    @object.AttemptAutoLoad(who);
  }

  public virtual void ShowMenu()
  {
    ItemGrabMenu activeClickableMenu1 = Game1.activeClickableMenu as ItemGrabMenu;
    switch (this.SpecialChestType)
    {
      case Chest.SpecialChestTypes.MiniShippingBin:
        Game1.activeClickableMenu = (IClickableMenu) new ItemGrabMenu((IList<Item>) this.GetItemsForPlayer(), false, true, new InventoryMenu.highlightThisItem(Utility.highlightShippableObjects), new ItemGrabMenu.behaviorOnItemSelect(this.grabItemFromInventory), (string) null, new ItemGrabMenu.behaviorOnItemSelect(this.grabItemFromChest), canBeExitedWithKey: true, source: 1, sourceItem: (Item) this, context: (object) this);
        break;
      case Chest.SpecialChestTypes.JunimoChest:
        Game1.activeClickableMenu = (IClickableMenu) new ItemGrabMenu((IList<Item>) this.GetItemsForPlayer(), false, true, new InventoryMenu.highlightThisItem(InventoryMenu.highlightAllItems), new ItemGrabMenu.behaviorOnItemSelect(this.grabItemFromInventory), (string) null, new ItemGrabMenu.behaviorOnItemSelect(this.grabItemFromChest), canBeExitedWithKey: true, showOrganizeButton: true, source: 1, sourceItem: (Item) this, context: (object) this);
        break;
      case Chest.SpecialChestTypes.AutoLoader:
        ItemGrabMenu itemGrabMenu = new ItemGrabMenu((IList<Item>) this.GetItemsForPlayer(), false, true, new InventoryMenu.highlightThisItem(InventoryMenu.highlightAllItems), new ItemGrabMenu.behaviorOnItemSelect(this.grabItemFromInventory), (string) null, new ItemGrabMenu.behaviorOnItemSelect(this.grabItemFromChest), canBeExitedWithKey: true, showOrganizeButton: true, source: 1, sourceItem: (Item) this, context: (object) this);
        itemGrabMenu.exitFunction = itemGrabMenu.exitFunction + (IClickableMenu.onExit) (() => this.CheckAutoLoad(Game1.player));
        Game1.activeClickableMenu = (IClickableMenu) itemGrabMenu;
        break;
      case Chest.SpecialChestTypes.Enricher:
        Game1.activeClickableMenu = (IClickableMenu) new ItemGrabMenu((IList<Item>) this.GetItemsForPlayer(), false, true, new InventoryMenu.highlightThisItem(StardewValley.Object.HighlightFertilizers), new ItemGrabMenu.behaviorOnItemSelect(this.grabItemFromInventory), (string) null, new ItemGrabMenu.behaviorOnItemSelect(this.grabItemFromChest), canBeExitedWithKey: true, showOrganizeButton: true, source: 1, sourceItem: (Item) this, context: (object) this);
        break;
      default:
        Game1.activeClickableMenu = (IClickableMenu) new ItemGrabMenu((IList<Item>) this.GetItemsForPlayer(), false, true, new InventoryMenu.highlightThisItem(InventoryMenu.highlightAllItems), new ItemGrabMenu.behaviorOnItemSelect(this.grabItemFromInventory), (string) null, new ItemGrabMenu.behaviorOnItemSelect(this.grabItemFromChest), canBeExitedWithKey: true, showOrganizeButton: true, source: 1, sourceItem: (Item) this, context: (object) this);
        break;
    }
    if (activeClickableMenu1 == null || !(Game1.activeClickableMenu is ItemGrabMenu activeClickableMenu2))
      return;
    activeClickableMenu2.inventory.moveItemSound = activeClickableMenu1.inventory.moveItemSound;
    activeClickableMenu2.inventory.highlightMethod = activeClickableMenu1.inventory.highlightMethod;
  }

  public virtual void grabItemFromInventory(Item item, Farmer who)
  {
    if (item.Stack == 0)
      item.Stack = 1;
    Item obj = this.addItem(item);
    if (obj == null)
      who.removeItemFromInventory(item);
    else
      obj = who.addItemToInventory(obj);
    this.clearNulls();
    int id = Game1.activeClickableMenu.currentlySnappedComponent != null ? Game1.activeClickableMenu.currentlySnappedComponent.myID : -1;
    this.ShowMenu();
    (Game1.activeClickableMenu as ItemGrabMenu).heldItem = obj;
    if (id == -1)
      return;
    Game1.activeClickableMenu.currentlySnappedComponent = Game1.activeClickableMenu.getComponentWithID(id);
    Game1.activeClickableMenu.snapCursorToCurrentSnappedComponent();
  }

  public IInventory GetItemsForPlayer() => this.GetItemsForPlayer(Game1.player.UniqueMultiplayerID);

  public IInventory GetItemsForPlayer(long id)
  {
    if (this.GlobalInventoryId != null)
      return (IInventory) Game1.player.team.GetOrCreateGlobalInventory(this.GlobalInventoryId);
    switch (this.SpecialChestType)
    {
      case Chest.SpecialChestTypes.MiniShippingBin:
        if (Game1.player.team.useSeparateWallets.Value && this.SpecialChestType == Chest.SpecialChestTypes.MiniShippingBin && Game1.player.team.useSeparateWallets.Value)
        {
          Inventory itemsForPlayer;
          if (!this.separateWalletItems.TryGetValue(id, out itemsForPlayer))
            this.separateWalletItems[id] = itemsForPlayer = new Inventory();
          return (IInventory) itemsForPlayer;
        }
        break;
      case Chest.SpecialChestTypes.JunimoChest:
        return (IInventory) Game1.player.team.GetOrCreateGlobalInventory("JunimoChests");
    }
    return (IInventory) this.Items;
  }

  public virtual bool isEmpty()
  {
    if (this.SpecialChestType != Chest.SpecialChestTypes.MiniShippingBin || !Game1.player.team.useSeparateWallets.Value)
      return !this.GetItemsForPlayer().HasAny();
    foreach (Inventory inventory in this.separateWalletItems.Values)
    {
      if (inventory.HasAny())
        return false;
    }
    return true;
  }

  public virtual void clearNulls() => this.GetItemsForPlayer().RemoveEmptySlots();

  public override void updateWhenCurrentLocation(GameTime time)
  {
    GameLocation location = this.Location;
    if (location == null)
      return;
    if (this.synchronized.Value)
      this.openChestEvent.Poll();
    if (this.localKickStartTile.HasValue)
    {
      if (Game1.currentLocation == location)
      {
        if ((double) this.kickProgress == 0.0)
        {
          if (Utility.isOnScreen((this.localKickStartTile.Value + new Vector2(0.5f, 0.5f)) * 64f, 64 /*0x40*/))
            Game1.playSound("clubhit");
          this.shakeTimer = 100;
        }
      }
      else
      {
        this.localKickStartTile = new Vector2?();
        this.kickProgress = -1f;
      }
      if ((double) this.kickProgress >= 0.0)
      {
        float num = 0.25f;
        this.kickProgress += (float) time.ElapsedGameTime.TotalSeconds / num;
        if ((double) this.kickProgress >= 1.0)
        {
          this.kickProgress = -1f;
          this.localKickStartTile = new Vector2?();
        }
      }
    }
    else
      this.kickProgress = -1f;
    this.fixLidFrame();
    this.mutex.Update(location);
    if (this.shakeTimer > 0)
    {
      this.shakeTimer -= time.ElapsedGameTime.Milliseconds;
      if (this.shakeTimer <= 0)
        this.health = 10;
    }
    this.hitTimerInstance?.Update(time);
    if (this.playerChest.Value)
    {
      if (this.SpecialChestType == Chest.SpecialChestTypes.MiniShippingBin)
      {
        this.UpdateFarmerNearby();
        if (this._shippingBinFrameCounter > -1)
        {
          --this._shippingBinFrameCounter;
          if (this._shippingBinFrameCounter <= 0)
          {
            this._shippingBinFrameCounter = 5;
            if (this._farmerNearby && this.currentLidFrame < this.getLastLidFrame())
              ++this.currentLidFrame;
            else if (!this._farmerNearby && this.currentLidFrame > this.startingLidFrame.Value)
              --this.currentLidFrame;
            else
              this._shippingBinFrameCounter = -1;
          }
        }
        if (Game1.activeClickableMenu != null || !this.GetMutex().IsLockHeld())
          return;
        this.GetMutex().ReleaseLock();
      }
      else if (this.frameCounter.Value > -1 && this.currentLidFrame < this.getLastLidFrame() + 1)
      {
        --this.frameCounter.Value;
        if (this.frameCounter.Value > 0 || !this.GetMutex().IsLockHeld())
          return;
        if (this.currentLidFrame == this.getLastLidFrame())
        {
          this.ShowMenu();
          this.frameCounter.Value = -1;
        }
        else
        {
          this.frameCounter.Value = 5;
          ++this.currentLidFrame;
        }
      }
      else
      {
        if ((this.frameCounter.Value != -1 || this.currentLidFrame <= this.startingLidFrame.Value) && this.currentLidFrame < this.getLastLidFrame() || Game1.activeClickableMenu != null || !this.GetMutex().IsLockHeld())
          return;
        this.GetMutex().ReleaseLock();
        this.currentLidFrame = this.getLastLidFrame();
        this.frameCounter.Value = 2;
        location.localSound("doorCreakReverse");
      }
    }
    else
    {
      if (this.frameCounter.Value <= -1 || this.currentLidFrame > this.getLastLidFrame())
        return;
      --this.frameCounter.Value;
      if (this.frameCounter.Value > 0)
        return;
      if (this.currentLidFrame == this.getLastLidFrame())
      {
        this.dumpContents();
        this.frameCounter.Value = -1;
      }
      else
      {
        this.frameCounter.Value = 10;
        ++this.currentLidFrame;
        if (this.currentLidFrame != this.getLastLidFrame())
          return;
        this.frameCounter.Value += 5;
      }
    }
  }

  public virtual void UpdateFarmerNearby(bool animate = true)
  {
    GameLocation location = this.Location;
    bool flag = false;
    Vector2 vector2 = this.tileLocation.Value;
    foreach (Character farmer in location.farmers)
    {
      Point tilePoint = farmer.TilePoint;
      if ((double) Math.Abs((float) tilePoint.X - vector2.X) <= 1.0 && (double) Math.Abs((float) tilePoint.Y - vector2.Y) <= 1.0)
      {
        flag = true;
        break;
      }
    }
    if (flag == this._farmerNearby)
      return;
    this._farmerNearby = flag;
    this._shippingBinFrameCounter = 5;
    if (!animate)
    {
      this._shippingBinFrameCounter = -1;
      if (this._farmerNearby)
        this.currentLidFrame = this.getLastLidFrame();
      else
        this.currentLidFrame = this.startingLidFrame.Value;
    }
    else
    {
      if (Game1.gameMode == (byte) 6)
        return;
      if (this._farmerNearby)
        location.localSound("doorCreak");
      else
        location.localSound("doorCreakReverse");
    }
  }

  /// <inheritdoc />
  public override void actionOnPlayerEntry()
  {
    base.actionOnPlayerEntry();
    this.fixLidFrame();
    if (this.specialChestType.Value == Chest.SpecialChestTypes.MiniShippingBin)
      this.UpdateFarmerNearby(false);
    this.kickProgress = -1f;
    this.localKickStartTile = new Vector2?();
    if (this.playerChest.Value || this.GetItemsForPlayer().Count != 0)
      return;
    this.currentLidFrame = this.getLastLidFrame();
  }

  public virtual void SetBigCraftableSpriteIndex(
    int sprite_index,
    int starting_lid_frame = -1,
    int lid_frame_count = 3)
  {
    this.bigCraftableSpriteIndex.Value = sprite_index;
    if (starting_lid_frame >= 0)
      this.startingLidFrame.Value = starting_lid_frame;
    else
      this.startingLidFrame.Value = sprite_index + 1;
    this.lidFrameCount.Value = lid_frame_count;
  }

  public override void draw(SpriteBatch spriteBatch, int x, int y, float alpha = 1f)
  {
    float b1 = (float) x;
    float b2 = (float) y;
    if (this.localKickStartTile.HasValue)
    {
      b1 = Utility.Lerp(this.localKickStartTile.Value.X, b1, this.kickProgress);
      b2 = Utility.Lerp(this.localKickStartTile.Value.Y, b2, this.kickProgress);
    }
    float layerDepth = Math.Max(0.0f, (float) ((((double) b2 + 1.0) * 64.0 - 24.0) / 10000.0)) + b1 * 1E-05f;
    if (this.localKickStartTile.HasValue)
    {
      spriteBatch.Draw(Game1.shadowTexture, Game1.GlobalToLocal(Game1.viewport, new Vector2((float) (((double) b1 + 0.5) * 64.0), (float) (((double) b2 + 0.5) * 64.0))), new Microsoft.Xna.Framework.Rectangle?(Game1.shadowTexture.Bounds), Color.Black * 0.5f, 0.0f, new Vector2((float) Game1.shadowTexture.Bounds.Center.X, (float) Game1.shadowTexture.Bounds.Center.Y), 4f, SpriteEffects.None, 0.0001f);
      b2 -= (float) Math.Sin((double) this.kickProgress * Math.PI) * 0.5f;
    }
    if (this.playerChest.Value && (this.QualifiedItemId == "(BC)130" || this.QualifiedItemId == "(BC)232" || this.QualifiedItemId.Equals("(BC)BigChest") || this.QualifiedItemId.Equals("(BC)BigStoneChest")))
    {
      if (this.playerChoiceColor.Value.Equals(Color.Black))
      {
        ParsedItemData dataOrErrorItem = ItemRegistry.GetDataOrErrorItem(this.QualifiedItemId);
        Texture2D texture = dataOrErrorItem.GetTexture();
        spriteBatch.Draw(texture, Game1.GlobalToLocal(Game1.viewport, new Vector2((float) ((double) b1 * 64.0 + (this.shakeTimer > 0 ? (double) Game1.random.Next(-1, 2) : 0.0)), (float) (((double) b2 - 1.0) * 64.0))), new Microsoft.Xna.Framework.Rectangle?(dataOrErrorItem.GetSourceRect()), this.tint.Value * alpha, 0.0f, Vector2.Zero, 4f, SpriteEffects.None, layerDepth);
        spriteBatch.Draw(texture, Game1.GlobalToLocal(Game1.viewport, new Vector2((float) ((double) b1 * 64.0 + (this.shakeTimer > 0 ? (double) Game1.random.Next(-1, 2) : 0.0)), (float) (((double) b2 - 1.0) * 64.0))), new Microsoft.Xna.Framework.Rectangle?(dataOrErrorItem.GetSourceRect(spriteIndex: new int?(this.currentLidFrame))), this.tint.Value * alpha * alpha, 0.0f, Vector2.Zero, 4f, SpriteEffects.None, layerDepth + 1E-05f);
      }
      else
      {
        ParsedItemData dataOrErrorItem = ItemRegistry.GetDataOrErrorItem(this.QualifiedItemId);
        Texture2D texture = dataOrErrorItem.GetTexture();
        int num1 = this.ParentSheetIndex;
        int num2 = this.currentLidFrame + 8;
        int num3 = this.currentLidFrame;
        switch (this.QualifiedItemId)
        {
          case "(BC)130":
            num1 = 168;
            num2 = this.currentLidFrame + 46;
            num3 = this.currentLidFrame + 38;
            break;
          case "(BC)BigChest":
            num1 = 312;
            num2 = this.currentLidFrame + 16 /*0x10*/;
            num3 = this.currentLidFrame + 8;
            break;
        }
        Microsoft.Xna.Framework.Rectangle sourceRect1 = dataOrErrorItem.GetSourceRect(spriteIndex: new int?(num1));
        Microsoft.Xna.Framework.Rectangle sourceRect2 = dataOrErrorItem.GetSourceRect(spriteIndex: new int?(num2));
        Microsoft.Xna.Framework.Rectangle sourceRect3 = dataOrErrorItem.GetSourceRect(spriteIndex: new int?(num3));
        spriteBatch.Draw(texture, Game1.GlobalToLocal(Game1.viewport, new Vector2(b1 * 64f, (float) (((double) b2 - 1.0) * 64.0 + (this.shakeTimer > 0 ? (double) Game1.random.Next(-1, 2) : 0.0)))), new Microsoft.Xna.Framework.Rectangle?(sourceRect1), this.playerChoiceColor.Value * alpha, 0.0f, Vector2.Zero, 4f, SpriteEffects.None, layerDepth);
        spriteBatch.Draw(texture, Game1.GlobalToLocal(Game1.viewport, new Vector2(b1 * 64f, (float) ((double) b2 * 64.0 + 20.0))), new Microsoft.Xna.Framework.Rectangle?(new Microsoft.Xna.Framework.Rectangle(0, num1 / 8 * 32 /*0x20*/ + 53, 16 /*0x10*/, 11)), Color.White * alpha, 0.0f, Vector2.Zero, 4f, SpriteEffects.None, layerDepth + 2E-05f);
        spriteBatch.Draw(texture, Game1.GlobalToLocal(Game1.viewport, new Vector2(b1 * 64f, (float) (((double) b2 - 1.0) * 64.0 + (this.shakeTimer > 0 ? (double) Game1.random.Next(-1, 2) : 0.0)))), new Microsoft.Xna.Framework.Rectangle?(sourceRect2), Color.White * alpha, 0.0f, Vector2.Zero, 4f, SpriteEffects.None, layerDepth + 2E-05f);
        spriteBatch.Draw(texture, Game1.GlobalToLocal(Game1.viewport, new Vector2(b1 * 64f, (float) (((double) b2 - 1.0) * 64.0 + (this.shakeTimer > 0 ? (double) Game1.random.Next(-1, 2) : 0.0)))), new Microsoft.Xna.Framework.Rectangle?(sourceRect3), this.playerChoiceColor.Value * alpha * alpha, 0.0f, Vector2.Zero, 4f, SpriteEffects.None, layerDepth + 1E-05f);
      }
    }
    else if (this.playerChest.Value)
    {
      ParsedItemData dataOrErrorItem = ItemRegistry.GetDataOrErrorItem(this.QualifiedItemId);
      Texture2D texture = dataOrErrorItem.GetTexture();
      spriteBatch.Draw(texture, Game1.GlobalToLocal(Game1.viewport, new Vector2((float) ((double) b1 * 64.0 + (this.shakeTimer > 0 ? (double) Game1.random.Next(-1, 2) : 0.0)), (float) (((double) b2 - 1.0) * 64.0))), new Microsoft.Xna.Framework.Rectangle?(dataOrErrorItem.GetSourceRect()), this.tint.Value * alpha, 0.0f, Vector2.Zero, 4f, SpriteEffects.None, layerDepth);
      spriteBatch.Draw(texture, Game1.GlobalToLocal(Game1.viewport, new Vector2((float) ((double) b1 * 64.0 + (this.shakeTimer > 0 ? (double) Game1.random.Next(-1, 2) : 0.0)), (float) (((double) b2 - 1.0) * 64.0))), new Microsoft.Xna.Framework.Rectangle?(dataOrErrorItem.GetSourceRect(spriteIndex: new int?(this.currentLidFrame))), this.tint.Value * alpha * alpha, 0.0f, Vector2.Zero, 4f, SpriteEffects.None, layerDepth + 1E-05f);
    }
    else if (this.giftbox.Value)
    {
      spriteBatch.Draw(Game1.shadowTexture, this.getLocalPosition(Game1.viewport) + new Vector2(16f, 53f), new Microsoft.Xna.Framework.Rectangle?(Game1.shadowTexture.Bounds), Color.White, 0.0f, new Vector2((float) Game1.shadowTexture.Bounds.Center.X, (float) Game1.shadowTexture.Bounds.Center.Y), 5f, SpriteEffects.None, 1E-07f);
      if (this.GetItemsForPlayer().Count <= 0)
        return;
      int y1 = this.giftboxIndex.Value * 32 /*0x20*/;
      spriteBatch.Draw(Game1.giftboxTexture, Game1.GlobalToLocal(Game1.viewport, new Vector2((float) ((double) b1 * 64.0 + (this.shakeTimer > 0 ? (double) Game1.random.Next(-1, 2) : 0.0)), (float) ((double) b2 * 64.0 - 52.0))), new Microsoft.Xna.Framework.Rectangle?(new Microsoft.Xna.Framework.Rectangle(0, y1, 16 /*0x10*/, 32 /*0x20*/)), this.tint.Value, 0.0f, Vector2.Zero, 4f, SpriteEffects.None, layerDepth);
    }
    else
    {
      int tilePosition = 500;
      Texture2D texture2D = Game1.objectSpriteSheet;
      int height = 16 /*0x10*/;
      int num = 0;
      if (this.bigCraftableSpriteIndex.Value >= 0)
      {
        tilePosition = this.bigCraftableSpriteIndex.Value;
        texture2D = Game1.bigCraftableSpriteSheet;
        height = 32 /*0x20*/;
        num = -64;
      }
      if (this.bigCraftableSpriteIndex.Value < 0)
        spriteBatch.Draw(Game1.shadowTexture, this.getLocalPosition(Game1.viewport) + new Vector2(16f, 53f), new Microsoft.Xna.Framework.Rectangle?(Game1.shadowTexture.Bounds), Color.White, 0.0f, new Vector2((float) Game1.shadowTexture.Bounds.Center.X, (float) Game1.shadowTexture.Bounds.Center.Y), 5f, SpriteEffects.None, 1E-07f);
      spriteBatch.Draw(texture2D, Game1.GlobalToLocal(Game1.viewport, new Vector2(b1 * 64f, b2 * 64f + (float) num)), new Microsoft.Xna.Framework.Rectangle?(Game1.getSourceRectForStandardTileSheet(texture2D, tilePosition, 16 /*0x10*/, height)), this.tint.Value, 0.0f, Vector2.Zero, 4f, SpriteEffects.None, layerDepth);
      Vector2 globalPosition = new Vector2(b1 * 64f, b2 * 64f + (float) num);
      if (this.bigCraftableSpriteIndex.Value < 0)
      {
        switch (this.currentLidFrame)
        {
          case 501:
            globalPosition.Y -= 32f;
            break;
          case 502:
            globalPosition.Y -= 40f;
            break;
          case 503:
            globalPosition.Y -= 60f;
            break;
        }
      }
      spriteBatch.Draw(texture2D, Game1.GlobalToLocal(Game1.viewport, globalPosition), new Microsoft.Xna.Framework.Rectangle?(Game1.getSourceRectForStandardTileSheet(texture2D, this.currentLidFrame, 16 /*0x10*/, height)), this.tint.Value, 0.0f, Vector2.Zero, 4f, SpriteEffects.None, layerDepth + 1E-05f);
    }
  }

  public virtual void draw(SpriteBatch spriteBatch, int x, int y, float alpha = 1f, bool local = false)
  {
    if (!this.playerChest.Value)
      return;
    if (this.playerChoiceColor.Equals(Color.Black))
    {
      ParsedItemData dataOrErrorItem = ItemRegistry.GetDataOrErrorItem(this.QualifiedItemId);
      spriteBatch.Draw(dataOrErrorItem.GetTexture(), local ? new Vector2((float) x, (float) (y - 64 /*0x40*/)) : Game1.GlobalToLocal(Game1.viewport, new Vector2((float) (x * 64 /*0x40*/ + (this.shakeTimer > 0 ? Game1.random.Next(-1, 2) : 0)), (float) ((y - 1) * 64 /*0x40*/))), new Microsoft.Xna.Framework.Rectangle?(dataOrErrorItem.GetSourceRect()), this.tint.Value * alpha, 0.0f, Vector2.Zero, 4f, SpriteEffects.None, local ? 0.89f : (float) (y * 64 /*0x40*/ + 4) / 10000f);
    }
    else
    {
      ParsedItemData data = ItemRegistry.GetData(this.QualifiedItemId);
      if (data == null)
        return;
      int num1 = this.ParentSheetIndex;
      int num2 = this.currentLidFrame + 8;
      int num3 = this.currentLidFrame;
      switch (this.QualifiedItemId)
      {
        case "(BC)130":
          num1 = 168;
          num2 = this.currentLidFrame + 46;
          num3 = this.currentLidFrame + 38;
          break;
        case "(BC)BigChest":
          num1 = 312;
          num2 = this.currentLidFrame + 16 /*0x10*/;
          num3 = this.currentLidFrame + 8;
          break;
        case "(BC)BigStoneChest":
          num2 = this.currentLidFrame + 8;
          num3 = this.currentLidFrame;
          break;
      }
      Microsoft.Xna.Framework.Rectangle sourceRect1 = data.GetSourceRect(spriteIndex: new int?(num1));
      Microsoft.Xna.Framework.Rectangle sourceRect2 = data.GetSourceRect(spriteIndex: new int?(num2));
      Microsoft.Xna.Framework.Rectangle sourceRect3 = data.GetSourceRect(spriteIndex: new int?(num3));
      Texture2D texture = data.GetTexture();
      spriteBatch.Draw(texture, local ? new Vector2((float) x, (float) (y - 64 /*0x40*/)) : Game1.GlobalToLocal(Game1.viewport, new Vector2((float) (x * 64 /*0x40*/), (float) ((y - 1) * 64 /*0x40*/ + (this.shakeTimer > 0 ? Game1.random.Next(-1, 2) : 0)))), new Microsoft.Xna.Framework.Rectangle?(sourceRect1), this.playerChoiceColor.Value * alpha, 0.0f, Vector2.Zero, 4f, SpriteEffects.None, local ? 0.9f : (float) (y * 64 /*0x40*/ + 4) / 10000f);
      spriteBatch.Draw(texture, local ? new Vector2((float) x, (float) (y - 64 /*0x40*/)) : Game1.GlobalToLocal(Game1.viewport, new Vector2((float) (x * 64 /*0x40*/), (float) ((y - 1) * 64 /*0x40*/ + (this.shakeTimer > 0 ? Game1.random.Next(-1, 2) : 0)))), new Microsoft.Xna.Framework.Rectangle?(sourceRect3), this.playerChoiceColor.Value * alpha * alpha, 0.0f, Vector2.Zero, 4f, SpriteEffects.None, local ? 0.9f : (float) (y * 64 /*0x40*/ + 5) / 10000f);
      spriteBatch.Draw(texture, local ? new Vector2((float) x, (float) (y + 20)) : Game1.GlobalToLocal(Game1.viewport, new Vector2((float) (x * 64 /*0x40*/), (float) (y * 64 /*0x40*/ + 20))), new Microsoft.Xna.Framework.Rectangle?(new Microsoft.Xna.Framework.Rectangle(0, num1 / 8 * 32 /*0x20*/ + 53, 16 /*0x10*/, 11)), Color.White * alpha, 0.0f, Vector2.Zero, 4f, SpriteEffects.None, local ? 0.91f : (float) (y * 64 /*0x40*/ + 6) / 10000f);
      spriteBatch.Draw(texture, local ? new Vector2((float) x, (float) (y - 64 /*0x40*/)) : Game1.GlobalToLocal(Game1.viewport, new Vector2((float) (x * 64 /*0x40*/), (float) ((y - 1) * 64 /*0x40*/ + (this.shakeTimer > 0 ? Game1.random.Next(-1, 2) : 0)))), new Microsoft.Xna.Framework.Rectangle?(sourceRect2), Color.White * alpha, 0.0f, Vector2.Zero, 4f, SpriteEffects.None, local ? 0.91f : (float) (y * 64 /*0x40*/ + 6) / 10000f);
    }
  }

  /// <inheritdoc />
  public override bool ForEachItem(ForEachItemDelegate handler, GetForEachItemPathDelegate getPath)
  {
    return base.ForEachItem(handler, getPath) && ForEachItemHelper.ApplyToList<Item>((IList<Item>) this.Items, handler, getPath);
  }

  public enum SpecialChestTypes
  {
    None,
    MiniShippingBin,
    JunimoChest,
    AutoLoader,
    Enricher,
    [Obsolete("This value is only used in mobile versions of the game.")] Mill,
    BigChest,
  }
}
