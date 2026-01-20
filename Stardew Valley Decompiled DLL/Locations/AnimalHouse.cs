// Decompiled with JetBrains decompiler
// Type: StardewValley.AnimalHouse
// Assembly: Stardew Valley, Version=1.6.15.24356, Culture=neutral, PublicKeyToken=null
// MVID: AA1F513A-94F0-4EF6-A35F-2D5B4A3721BF
// Assembly location: D:\SteamLibrary\steamapps\common\Stardew Valley\Stardew Valley.dll
// XML documentation location: D:\SteamLibrary\steamapps\common\Stardew Valley\Stardew Valley.xml

using Microsoft.Xna.Framework;
using Netcode;
using StardewValley.Buildings;
using StardewValley.Events;
using StardewValley.GameData.Buildings;
using StardewValley.GameData.FarmAnimals;
using StardewValley.GameData.Machines;
using StardewValley.TokenizableStrings;
using System.Collections.Generic;
using System.Xml.Serialization;
using xTile.Dimensions;

#nullable disable
namespace StardewValley;

public class AnimalHouse : GameLocation
{
  [XmlElement("animalLimit")]
  public readonly NetInt animalLimit = new NetInt(4);
  public readonly NetLongList animalsThatLiveHere = new NetLongList();
  [XmlIgnore]
  public bool hasShownIncubatorBuildingFullMessage;

  public AnimalHouse()
  {
  }

  public AnimalHouse(string mapPath, string name)
    : base(mapPath, name)
  {
  }

  protected override void initNetFields()
  {
    base.initNetFields();
    this.NetFields.AddField((INetSerializable) this.animalLimit, "animalLimit").AddField((INetSerializable) this.animalsThatLiveHere, "animalsThatLiveHere");
  }

  /// <inheritdoc />
  public override void OnParentBuildingUpgraded(Building building)
  {
    base.OnParentBuildingUpgraded(building);
    BuildingData data = building.GetData();
    if (data != null)
      this.animalLimit.Value = data.MaxOccupants;
    this.resetPositionsOfAllAnimals();
    this.loadLights();
  }

  public bool isFull() => this.animalsThatLiveHere.Count >= this.animalLimit.Value;

  public override bool checkAction(Location tileLocation, xTile.Dimensions.Rectangle viewport, Farmer who)
  {
    if (!(who.ActiveObject?.QualifiedItemId == "(O)178") || this.doesTileHaveProperty(tileLocation.X, tileLocation.Y, "Trough", "Back") == null || this.objects.ContainsKey(new Vector2((float) tileLocation.X, (float) tileLocation.Y)))
      return base.checkAction(tileLocation, viewport, who);
    this.objects.Add(new Vector2((float) tileLocation.X, (float) tileLocation.Y), (Object) who.ActiveObject.getOne());
    who.reduceActiveItemByOne();
    who.currentLocation.playSound("coin");
    Game1.haltAfterCheck = false;
    return true;
  }

  protected override void resetSharedState()
  {
    this.resetPositionsOfAllAnimals();
    foreach (Object @object in this.objects.Values)
    {
      if (@object.bigCraftable.Value)
      {
        MachineData machineData = @object.GetMachineData();
        if ((machineData != null ? (machineData.IsIncubator ? 1 : 0) : 0) != 0 && @object.heldObject.Value != null && @object.MinutesUntilReady <= 0)
        {
          if (!this.isFull())
          {
            string str = "??";
            FarmAnimalData animalDataFromEgg = FarmAnimal.GetAnimalDataFromEgg((Item) @object.heldObject.Value, (GameLocation) this);
            if (animalDataFromEgg != null && animalDataFromEgg.BirthText != null)
              str = TokenParser.ParseText(animalDataFromEgg.BirthText);
            this.currentEvent = new Event($"none/-1000 -1000/farmer 2 9 0/pause 250/message \"{str}\"/pause 500/animalNaming/pause 500/end");
            break;
          }
          if (!this.hasShownIncubatorBuildingFullMessage)
          {
            this.hasShownIncubatorBuildingFullMessage = true;
            Game1.showGlobalMessage(Game1.content.LoadString("Strings\\Locations:AnimalHouse_Incubator_HouseFull"));
          }
        }
      }
    }
    base.resetSharedState();
  }

  /// <summary>Hatch an incubated animal egg that's ready to hatch, if there are any.</summary>
  /// <param name="name">The name of the animal to set.</param>
  public void addNewHatchedAnimal(string name)
  {
    bool flag = false;
    foreach (Object @object in this.objects.Values)
    {
      if (@object.bigCraftable.Value)
      {
        MachineData machineData = @object.GetMachineData();
        if ((machineData != null ? (machineData.IsIncubator ? 1 : 0) : 0) != 0 && @object.heldObject.Value != null && @object.MinutesUntilReady <= 0 && !this.isFull())
        {
          flag = true;
          string id;
          FarmAnimal animal = new FarmAnimal(FarmAnimal.TryGetAnimalDataFromEgg((Item) @object.heldObject.Value, (GameLocation) this, out id, out FarmAnimalData _) ? id : "White Chicken", Game1.multiplayer.getNewID(), Game1.player.UniqueMultiplayerID);
          animal.Name = name;
          animal.displayName = name;
          @object.heldObject.Value = (Object) null;
          this.adoptAnimal(animal);
          break;
        }
      }
    }
    if (!flag && Game1.farmEvent is QuestionEvent farmEvent)
    {
      FarmAnimal animal = new FarmAnimal(farmEvent.animal.type.Value, Game1.multiplayer.getNewID(), Game1.player.UniqueMultiplayerID);
      animal.Name = name;
      animal.displayName = name;
      animal.parentId.Value = farmEvent.animal.myID.Value;
      this.adoptAnimal(animal);
      farmEvent.forceProceed = true;
    }
    Game1.exitActiveMenu();
  }

  /// <summary>Add an animal to this location and set the location as the animal's home.</summary>
  /// <param name="animal">The animal to adopt.</param>
  public void adoptAnimal(FarmAnimal animal)
  {
    this.animals.Add(animal.myID.Value, animal);
    animal.currentLocation = (GameLocation) this;
    this.animalsThatLiveHere.Add(animal.myID.Value);
    animal.homeInterior = (GameLocation) this;
    animal.setRandomPosition((GameLocation) this);
    foreach (Farmer allFarmer in Game1.getAllFarmers())
    {
      string str = animal.displayType;
      if (str == "White Chicken" || str == "Brown Chicken")
        str = "Chicken";
      string eventName = "purchasedAnimal_" + str;
      allFarmer.autoGenerateActiveDialogueEvent(eventName);
    }
  }

  public void resetPositionsOfAllAnimals()
  {
    foreach (KeyValuePair<long, FarmAnimal> pair in this.animals.Pairs)
      pair.Value.setRandomPosition((GameLocation) this);
  }

  /// <inheritdoc />
  public override bool dropObject(
    Object obj,
    Vector2 location,
    xTile.Dimensions.Rectangle viewport,
    bool initialPlacement,
    Farmer who = null)
  {
    Vector2 key = new Vector2((float) (int) ((double) location.X / 64.0), (float) (int) ((double) location.Y / 64.0));
    return obj.QualifiedItemId == "(O)178" && this.doesTileHaveProperty((int) key.X, (int) key.Y, "Trough", "Back") != null ? this.objects.TryAdd(key, obj) : base.dropObject(obj, location, viewport, initialPlacement);
  }

  public override void TransferDataFromSavedLocation(GameLocation l)
  {
    this.animalLimit.Value = ((AnimalHouse) l).animalLimit.Value;
    base.TransferDataFromSavedLocation(l);
  }

  public override void DayUpdate(int dayOfMonth)
  {
    base.DayUpdate(dayOfMonth);
    if (!this.HasMapPropertyWithValue("AutoFeed"))
      return;
    this.feedAllAnimals();
  }

  public void feedAllAnimals()
  {
    GameLocation rootLocation = this.GetRootLocation();
    int num = 0;
    for (int index1 = 0; index1 < this.map.Layers[0].LayerWidth; ++index1)
    {
      for (int index2 = 0; index2 < this.map.Layers[0].LayerHeight; ++index2)
      {
        if (this.doesTileHaveProperty(index1, index2, "Trough", "Back") != null)
        {
          Vector2 key = new Vector2((float) index1, (float) index2);
          if (!this.objects.ContainsKey(key))
          {
            Object hayFromAnySilo = GameLocation.GetHayFromAnySilo(rootLocation);
            if (hayFromAnySilo == null)
              return;
            this.objects.Add(key, hayFromAnySilo);
            ++num;
          }
          if (num >= this.animalLimit.Value)
            return;
        }
      }
    }
  }
}
