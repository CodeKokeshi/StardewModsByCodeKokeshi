// Decompiled with JetBrains decompiler
// Type: StardewValley.Locations.Cabin
// Assembly: Stardew Valley, Version=1.6.15.24356, Culture=neutral, PublicKeyToken=null
// MVID: AA1F513A-94F0-4EF6-A35F-2D5B4A3721BF
// Assembly location: D:\SteamLibrary\steamapps\common\Stardew Valley\Stardew Valley.dll
// XML documentation location: D:\SteamLibrary\steamapps\common\Stardew Valley\Stardew Valley.xml

using Microsoft.Xna.Framework;
using Netcode;
using StardewValley.Buildings;
using StardewValley.Characters;
using StardewValley.Inventories;
using StardewValley.Menus;
using StardewValley.Network;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml.Serialization;
using xTile.Dimensions;

#nullable disable
namespace StardewValley.Locations;

public class Cabin : FarmHouse
{
  /// <summary>Obsolete. This is only kept to preserve data from old save files. Use <see cref="P:StardewValley.Locations.Cabin.owner" /> instead.</summary>
  [XmlElement("farmhand")]
  public Farmer obsolete_farmhand;
  /// <summary>A net reference to the farmhand who owns this cabin. Most code should use <see cref="P:StardewValley.Locations.Cabin.owner" /> instead.</summary>
  [XmlElement("farmhandReference")]
  public readonly NetFarmerRef farmhandReference = new NetFarmerRef();
  [XmlIgnore]
  public readonly NetMutex inventoryMutex = new NetMutex();

  /// <inheritdoc />
  [XmlIgnore]
  public override Farmer owner => this.farmhandReference.Value;

  public Cabin()
  {
  }

  public Cabin(string map)
    : base(map, nameof (Cabin))
  {
  }

  protected override void initNetFields()
  {
    base.initNetFields();
    this.NetFields.AddField((INetSerializable) this.farmhandReference.NetFields, "farmhandReference.NetFields").AddField((INetSerializable) this.inventoryMutex.NetFields, "inventoryMutex.NetFields");
  }

  public void CreateFarmhand()
  {
    if (this.HasOwner)
      return;
    long id;
    do
    {
      id = Utility.RandomLong();
    }
    while (Game1.GetPlayer(id) != null);
    Farmer farmhand = new Farmer(new FarmerSprite((string) null), new Vector2(0.0f, 0.0f), 1, "", Farmer.initialTools(), true)
    {
      UniqueMultiplayerID = id
    };
    farmhand.addQuest("9");
    farmhand.homeLocation.Value = this.NameOrUniqueName;
    Game1.netWorldState.Value.farmhandData[farmhand.UniqueMultiplayerID] = farmhand;
    this.AssignFarmhand(farmhand);
    Game1.netWorldState.Value.ResetFarmhandState(farmhand);
  }

  /// <summary>Fully delete the farmhand associated with this cabin. This will permanently remove their data if the game is saved.</summary>
  public void DeleteFarmhand()
  {
    if (!this.HasOwner)
      return;
    Game1.player.team.DeleteFarmhand(this.owner);
    this.farmhandReference.Value = (Farmer) null;
  }

  /// <summary>Get whether this cabin is available to assign to a farmhand.</summary>
  /// <param name="farmhand">The farmhand to check.</param>
  public bool CanAssignTo(Farmer farmhand)
  {
    return !this.HasOwner || this.OwnerId == farmhand.UniqueMultiplayerID || this.owner.isUnclaimedFarmhand;
  }

  /// <summary>Assign a farmhand to this cabin.</summary>
  /// <param name="farmhand">The farmhand to assign to this cabin.</param>
  /// <exception cref="T:System.InvalidOperationException">The farmhand can't be assigned to this cabin because an existing player is already assigned. You must call <see cref="M:StardewValley.Locations.Cabin.DeleteFarmhand" /> first in that case.</exception>
  public void AssignFarmhand(Farmer farmhand)
  {
    if (this.HasOwner && this.OwnerId != farmhand.UniqueMultiplayerID)
    {
      if (this.owner.isUnclaimedFarmhand)
        this.DeleteFarmhand();
      else
        throw new InvalidOperationException($"Can't assign cabin to {farmhand.Name} ({farmhand.UniqueMultiplayerID}) because it's already assigned to {this.owner.Name} ({this.owner.UniqueMultiplayerID}).");
    }
    this.farmhandReference.Value = farmhand;
    farmhand.homeLocation.Value = this.NameOrUniqueName;
  }

  public override bool checkAction(Location tileLocation, xTile.Dimensions.Rectangle viewport, Farmer who)
  {
    switch (this.getTileIndexAt(tileLocation, "Buildings", "indoor"))
    {
      case 647:
      case 648:
        if (!this.IsOwnerActivated)
        {
          this.inventoryMutex.RequestLock((Action) (() =>
          {
            this.playSound("Ship");
            this.openFarmhandInventory();
          }));
          return true;
        }
        break;
    }
    return base.checkAction(tileLocation, viewport, who);
  }

  public override void updateEvenIfFarmerIsntHere(GameTime time, bool skipWasUpdatedFlush = false)
  {
    base.updateEvenIfFarmerIsntHere(time, skipWasUpdatedFlush);
    this.inventoryMutex.Update(Game1.getOnlineFarmers());
    if (!this.inventoryMutex.IsLockHeld() || Game1.activeClickableMenu is ItemGrabMenu)
      return;
    this.inventoryMutex.ReleaseLock();
  }

  public IInventory getInventory()
  {
    Farmer owner = this.owner;
    return owner == null ? (IInventory) null : (IInventory) owner.Items;
  }

  public void openFarmhandInventory()
  {
    Game1.activeClickableMenu = (IClickableMenu) new ItemGrabMenu((IList<Item>) this.getInventory(), false, true, new InventoryMenu.highlightThisItem(InventoryMenu.highlightAllItems), new ItemGrabMenu.behaviorOnItemSelect(this.grabItemFromPlayerInventory), (string) null, new ItemGrabMenu.behaviorOnItemSelect(this.grabItemFromFarmhandInventory), canBeExitedWithKey: true, showOrganizeButton: true, source: 1, context: (object) this);
  }

  public bool isInventoryOpen() => this.inventoryMutex.IsLocked();

  private void grabItemFromPlayerInventory(Item item, Farmer who)
  {
    if (!this.HasOwner)
      return;
    item.FixStackSize();
    Item inventory = this.owner.addItemToInventory(item);
    if (inventory == null)
      who.removeItemFromInventory(item);
    else
      who.addItemToInventory(inventory);
    int id = Game1.activeClickableMenu.currentlySnappedComponent != null ? Game1.activeClickableMenu.currentlySnappedComponent.myID : -1;
    this.openFarmhandInventory();
    if (id == -1)
      return;
    Game1.activeClickableMenu.currentlySnappedComponent = Game1.activeClickableMenu.getComponentWithID(id);
    Game1.activeClickableMenu.snapCursorToCurrentSnappedComponent();
  }

  private void grabItemFromFarmhandInventory(Item item, Farmer who)
  {
    if (!who.couldInventoryAcceptThisItem(item))
      return;
    this.getInventory().Remove(item);
    this.openFarmhandInventory();
  }

  public override void updateWarps()
  {
    if (Game1.IsClient)
      return;
    base.updateWarps();
  }

  public List<Item> demolish()
  {
    List<Item> list = new List<Item>((IEnumerable<Item>) this.getInventory()).Where<Item>((Func<Item, bool>) (item => item != null)).ToList<Item>();
    this.getInventory().Clear();
    Farmer.removeInitialTools(list);
    foreach (NPC character in new List<NPC>((IEnumerable<NPC>) this.characters))
    {
      if (character.IsVillager && Game1.characterData.ContainsKey(character.Name))
      {
        character.reloadDefaultLocation();
        character.ClearSchedule();
        Game1.warpCharacter(character, character.DefaultMap, character.DefaultPosition / 64f);
      }
      if (character is Pet pet)
        pet.warpToFarmHouse(Game1.MasterPlayer);
    }
    Cellar cellar = this.GetCellar();
    if (cellar != null)
    {
      cellar.objects.Clear();
      cellar.setUpAgingBoards();
    }
    if (this.HasOwner)
      Game1.player.team.DeleteFarmhand(this.owner);
    Game1.updateCellarAssignments();
    return list;
  }

  public override void DayUpdate(int dayOfMonth)
  {
    base.DayUpdate(dayOfMonth);
    if (!this.HasOwner)
      return;
    this.owner.stamina = (float) this.owner.MaxStamina;
  }

  public override Point getPorchStandingSpot()
  {
    Building parentBuilding = this.ParentBuilding;
    return parentBuilding == null ? base.getPorchStandingSpot() : parentBuilding.getPorchStandingSpot();
  }
}
