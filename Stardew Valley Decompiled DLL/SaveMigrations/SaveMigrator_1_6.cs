// Decompiled with JetBrains decompiler
// Type: StardewValley.SaveMigrations.SaveMigrator_1_6
// Assembly: Stardew Valley, Version=1.6.15.24356, Culture=neutral, PublicKeyToken=null
// MVID: AA1F513A-94F0-4EF6-A35F-2D5B4A3721BF
// Assembly location: D:\SteamLibrary\steamapps\common\Stardew Valley\Stardew Valley.dll
// XML documentation location: D:\SteamLibrary\steamapps\common\Stardew Valley\Stardew Valley.xml

using Microsoft.Xna.Framework;
using Netcode;
using StardewValley.Buildings;
using StardewValley.Characters;
using StardewValley.Delegates;
using StardewValley.Extensions;
using StardewValley.GameData.Buildings;
using StardewValley.GameData.Crops;
using StardewValley.GameData.Tools;
using StardewValley.Internal;
using StardewValley.ItemTypeDefinitions;
using StardewValley.Locations;
using StardewValley.Network;
using StardewValley.Objects;
using StardewValley.Quests;
using StardewValley.SpecialOrders;
using StardewValley.TerrainFeatures;
using StardewValley.Tools;
using StardewValley.Util;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Xml;
using System.Xml.Serialization;
using xTile.Layers;
using xTile.Tiles;

#nullable disable
namespace StardewValley.SaveMigrations;

/// <summary>Migrates existing save files for compatibility with Stardew Valley 1.6.</summary>
public class SaveMigrator_1_6 : ISaveMigrator
{
  /// <inheritdoc />
  public Version GameVersion { get; } = new Version(1, 5);

  /// <inheritdoc />
  public bool ApplySaveFix(SaveFixes saveFix)
  {
    switch (saveFix)
    {
      case SaveFixes.MigrateBuildingsToData:
        Utility.ForEachBuilding((Func<Building, bool>) (building =>
        {
          if (building is JunimoHut junimoHut2 && junimoHut2.obsolete_output != null)
          {
            junimoHut2.GetOutputChest().Items.AddRange((ICollection<Item>) junimoHut2.obsolete_output.Items);
            junimoHut2.obsolete_output = (Chest) null;
          }
          if (building.isUnderConstruction(false))
          {
            Game1.netWorldState.Value.MarkUnderConstruction("Robin", building);
            if (building.daysUntilUpgrade.Value > 0 && string.IsNullOrWhiteSpace(building.upgradeName.Value))
              building.upgradeName.Value = SaveMigrator_1_6.InferBuildingUpgradingTo(building.buildingType.Value);
          }
          return true;
        }));
        return true;
      case SaveFixes.ModularizeFarmhouse:
        Game1.getFarm().AddDefaultBuildings(true);
        return true;
      case SaveFixes.ModularizePets:
        foreach (Farmer allFarmer in Game1.getAllFarmers())
        {
          bool? obsoleteCatPerson = allFarmer.obsolete_catPerson;
          allFarmer.whichPetType = !obsoleteCatPerson.HasValue || !obsoleteCatPerson.GetValueOrDefault() ? "Dog" : "Cat";
          allFarmer.obsolete_catPerson = new bool?();
        }
        Utility.ForEachLocation((Func<GameLocation, bool>) (location =>
        {
          for (int index = location.characters.Count - 1; index >= 0; --index)
          {
            if (location.characters[index] is Pet character2)
            {
              string petType = (string) null;
              if (character2.GetType() == typeof (Cat))
                petType = "Cat";
              else if (character2.GetType() == typeof (Dog))
                petType = "Dog";
              if (petType != null)
              {
                Pet pet = new Pet((int) ((double) character2.Position.X / 64.0), (int) ((double) character2.Position.X / 64.0), character2.whichBreed.Value, petType);
                pet.Name = character2.Name;
                pet.displayName = character2.displayName;
                if (character2.currentLocation != null)
                  pet.currentLocation = character2.currentLocation;
                pet.friendshipTowardFarmer.Value = character2.friendshipTowardFarmer.Value;
                pet.grantedFriendshipForPet.Value = character2.grantedFriendshipForPet.Value;
                pet.lastPetDay.Clear();
                pet.lastPetDay.CopyFrom((IEnumerable<KeyValuePair<long, int>>) character2.lastPetDay.Pairs);
                pet.isSleepingOnFarmerBed.Value = character2.isSleepingOnFarmerBed.Value;
                pet.modData.CopyFrom(character2.modData);
                location.characters[index] = (NPC) pet;
              }
            }
          }
          return true;
        }));
        Farm farm1 = Game1.getFarm();
        farm1.AddDefaultBuilding("Pet Bowl", farm1.GetStarterPetBowlLocation());
        PetBowl buildingByType1 = farm1.getBuildingByType("Pet Bowl") as PetBowl;
        Pet pet1 = Game1.player.getPet();
        if (buildingByType1 != null && pet1 != null)
        {
          buildingByType1.AssignPet(pet1);
          pet1.setAtFarmPosition();
        }
        return true;
      case SaveFixes.AddNpcRemovalFlags:
        GameLocation locationFromName1 = Game1.getLocationFromName("WitchSwamp");
        if (locationFromName1 != null && locationFromName1.getCharacterFromName("Henchman") == null)
          Game1.addMail("henchmanGone", true, true);
        GameLocation locationFromName2 = Game1.getLocationFromName("SandyHouse");
        if (locationFromName2 != null && locationFromName2.getCharacterFromName("Bouncer") == null)
          Game1.addMail("bouncerGone", true, true);
        return true;
      case SaveFixes.MigrateFarmhands:
        return true;
      case SaveFixes.MigrateLitterItemData:
        Utility.ForEachItem((Func<Item, bool>) (item =>
        {
          switch (item.QualifiedItemId)
          {
            case "(O)0":
            case "(O)10":
            case "(O)12":
            case "(O)14":
            case "(O)2":
            case "(O)25":
            case "(O)290":
            case "(O)294":
            case "(O)295":
            case "(O)313":
            case "(O)314":
            case "(O)315":
            case "(O)316":
            case "(O)317":
            case "(O)318":
            case "(O)319":
            case "(O)32":
            case "(O)320":
            case "(O)321":
            case "(O)34":
            case "(O)343":
            case "(O)36":
            case "(O)38":
            case "(O)4":
            case "(O)40":
            case "(O)42":
            case "(O)44":
            case "(O)450":
            case "(O)452":
            case "(O)46":
            case "(O)48":
            case "(O)50":
            case "(O)52":
            case "(O)54":
            case "(O)56":
            case "(O)58":
            case "(O)6":
            case "(O)668":
            case "(O)670":
            case "(O)674":
            case "(O)675":
            case "(O)676":
            case "(O)677":
            case "(O)678":
            case "(O)679":
            case "(O)75":
            case "(O)750":
            case "(O)751":
            case "(O)76":
            case "(O)760":
            case "(O)762":
            case "(O)764":
            case "(O)765":
            case "(O)77":
            case "(O)784":
            case "(O)785":
            case "(O)786":
            case "(O)792":
            case "(O)793":
            case "(O)794":
            case "(O)8":
            case "(O)816":
            case "(O)817":
            case "(O)818":
            case "(O)819":
            case "(O)843":
            case "(O)844":
            case "(O)845":
            case "(O)846":
            case "(O)847":
            case "(O)849":
            case "(O)850":
            case "(O)882":
            case "(O)883":
            case "(O)884":
            case "(O)95":
              item.Category = -999;
              if (item is StardewValley.Object object3)
              {
                object3.Type = "Litter";
                break;
              }
              break;
            case "(O)372":
              item.Category = -4;
              if (item is StardewValley.Object object4)
              {
                object4.Type = "Fish";
                break;
              }
              break;
          }
          return true;
        }));
        return true;
      case SaveFixes.MigrateHoneyItems:
        Utility.ForEachItem((Func<Item, bool>) (item =>
        {
          if (!(item is StardewValley.Object object6) || object6.QualifiedItemId != "(O)340")
            return true;
          object6.preserve.Value = new StardewValley.Object.PreserveType?(StardewValley.Object.PreserveType.Honey);
          if (object6.preservedParentSheetIndex.Value == null || object6.preservedParentSheetIndex.Value == "0")
          {
            string str = object6.obsolete_honeyType;
            if (string.IsNullOrWhiteSpace(str) && object6.name.EndsWith(" Honey"))
              str = object6.name.Substring(0, object6.name.Length - " Honey".Length).Replace(" ", "");
            switch (str)
            {
              case "Poppy":
                object6.preservedParentSheetIndex.Value = "376";
                break;
              case "Tulip":
                object6.preservedParentSheetIndex.Value = "591";
                break;
              case "SummerSpangle":
                object6.preservedParentSheetIndex.Value = "593";
                break;
              case "FairyRose":
                object6.preservedParentSheetIndex.Value = "595";
                break;
              case "BlueJazz":
                object6.preservedParentSheetIndex.Value = "597";
                break;
              default:
                object6.Name = "Wild Honey";
                object6.preservedParentSheetIndex.Value = (string) null;
                break;
            }
          }
          if (object6.Name == "Honey" && object6.preservedParentSheetIndex.Value == "-1")
            object6.Name = "Wild Honey";
          object6.obsolete_honeyType = (string) null;
          return true;
        }));
        return true;
      case SaveFixes.MigrateMachineLastOutputRule:
        Utility.ForEachItem((Func<Item, bool>) (item =>
        {
          if (item is StardewValley.Object machine2)
            SaveMigrator_1_6.InferMachineInputOutputFields(machine2);
          return true;
        }));
        return true;
      case SaveFixes.StandardizeBundleFields:
        return true;
      case SaveFixes.MigrateAdventurerGoalFlags:
        Dictionary<string, string> dictionary1 = new Dictionary<string, string>()
        {
          ["Gil_Slime Charmer Ring"] = "Gil_Slimes",
          ["Gil_Slime Charmer Ring"] = "Gil_Slimes",
          ["Gil_Savage Ring"] = "Gil_Shadows",
          ["Gil_Vampire Ring"] = "Gil_Bats",
          ["Gil_Skeleton Mask"] = "Gil_Skeletons",
          ["Gil_Insect Head"] = "Gil_Insects",
          ["Gil_Hard Hat"] = "Gil_Duggy",
          ["Gil_Burglar's Ring"] = "Gil_DustSpirits",
          ["Gil_Crabshell Ring"] = "Gil_Crabs",
          ["Gil_Arcane Hat"] = "Gil_Mummies",
          ["Gil_Knight's Helmet"] = "Gil_Dinos",
          ["Gil_Napalm Ring"] = "Gil_Serpents",
          ["Gil_Telephone"] = "Gil_FlameSpirits"
        };
        foreach (Farmer allFarmer in Game1.getAllFarmers())
        {
          NetStringHashSet[] netStringHashSetArray = new NetStringHashSet[2]
          {
            allFarmer.mailReceived,
            allFarmer.mailForTomorrow
          };
          foreach (NetStringHashSet netStringHashSet in netStringHashSetArray)
          {
            foreach (KeyValuePair<string, string> keyValuePair in dictionary1)
            {
              if (netStringHashSet.Remove(keyValuePair.Key))
                netStringHashSet.Add(keyValuePair.Value);
            }
          }
          IList<string> mailbox = Game1.mailbox;
          for (int index = 0; index < mailbox.Count; ++index)
          {
            string str;
            if (dictionary1.TryGetValue(mailbox[index], out str))
              mailbox[index] = str;
          }
        }
        return true;
      case SaveFixes.SetCropSeedId:
        Dictionary<string, string> seedsByHarvestId = new Dictionary<string, string>();
        foreach (KeyValuePair<string, CropData> keyValuePair in (IEnumerable<KeyValuePair<string, CropData>>) Game1.cropData)
        {
          string key = keyValuePair.Key;
          string harvestItemId = keyValuePair.Value.HarvestItemId;
          if (harvestItemId != null)
            seedsByHarvestId.TryAdd(harvestItemId, key);
        }
        Utility.ForEachCrop((Func<Crop, bool>) (crop =>
        {
          if (crop.netSeedIndex.Value == "-1")
            crop.netSeedIndex.Value = (string) null;
          string str;
          if (!string.IsNullOrWhiteSpace(crop.netSeedIndex.Value) || crop.isWildSeedCrop() || crop.forageCrop.Value || crop.indexOfHarvest.Value == null || !seedsByHarvestId.TryGetValue(crop.indexOfHarvest.Value, out str))
            return true;
          crop.netSeedIndex.Value = str;
          return true;
        }));
        return true;
      case SaveFixes.FixMineBoulderCollisions:
        Mine mine = Game1.RequireLocation<Mine>("Mine");
        Vector2 boulderPosition = mine.GetBoulderPosition();
        StardewValley.Object object7;
        if (mine.objects.TryGetValue(boulderPosition, out object7) && object7.QualifiedItemId == "(BC)78" && object7.TileLocation == Vector2.Zero)
          object7.TileLocation = boulderPosition;
        return true;
      case SaveFixes.MigratePetAndPetBowlIds:
        Pet pet2 = Game1.player.getPet();
        if (pet2 != null)
        {
          pet2.petId.Value = Guid.NewGuid();
          PetBowl buildingByType2 = (PetBowl) Game1.getFarm().getBuildingByType("Pet Bowl");
          if (buildingByType2 != null)
          {
            buildingByType2.AssignPet(pet2);
            pet2.setAtFarmPosition();
          }
        }
        return true;
      case SaveFixes.MigrateHousePaint:
        Farm farm2 = Game1.getFarm();
        if (farm2.housePaintColor.Value != null)
        {
          farm2.GetMainFarmHouse().netBuildingPaintColor.Value.CopyFrom(farm2.housePaintColor.Value);
          farm2.housePaintColor.Value = (BuildingPaintColor) null;
        }
        return true;
      case SaveFixes.MigrateShedFloorWallIds:
        Utility.ForEachLocation((Func<GameLocation, bool>) (location =>
        {
          if (location is Shed shed2)
          {
            string str1;
            if (shed2.appliedFloor.TryGetValue("Floor_0", out str1))
            {
              shed2.appliedFloor.Remove("Floor_0");
              shed2.appliedFloor["Floor"] = str1;
            }
            string str2;
            if (shed2.appliedWallpaper.TryGetValue("Wall_0", out str2))
            {
              shed2.appliedWallpaper.Remove("Wall_0");
              shed2.appliedWallpaper["Wall"] = str2;
            }
          }
          return true;
        }));
        return true;
      case SaveFixes.MigrateItemIds:
        Utility.ForEachItem((Func<Item, bool>) (item =>
        {
          switch (item)
          {
            case Boots boots2:
              if (boots2.appliedBootSheetIndex.Value == "-1")
              {
                boots2.appliedBootSheetIndex.Value = (string) null;
                break;
              }
              break;
            case MeleeWeapon meleeWeapon2:
              meleeWeapon2.appearance.Value = string.IsNullOrWhiteSpace(meleeWeapon2.appearance.Value) || !(meleeWeapon2.appearance.Value != "-1") ? (string) null : ItemRegistry.ManuallyQualifyItemId(meleeWeapon2.appearance.Value, "(W)");
              break;
            case Fence fence2:
              if (fence2.obsolete_whichType.HasValue)
              {
                item.itemId.Value = (string) null;
                break;
              }
              break;
            case Slingshot slingshot2:
              slingshot2.ItemId = (string) null;
              break;
            case Torch _:
              if (item.itemId.Value != item.ParentSheetIndex.ToString())
              {
                item.itemId.Value = (string) null;
                break;
              }
              break;
          }
          string itemId = item.ItemId;
          return true;
        }));
        foreach (Farmer allFarmer in Game1.getAllFarmers())
        {
          NetStringIntArrayDictionary fishCaught = allFarmer.fishCaught;
          if (fishCaught != null)
          {
            foreach (KeyValuePair<string, int[]> keyValuePair in fishCaught.Pairs.ToArray<KeyValuePair<string, int[]>>())
            {
              fishCaught.Remove(keyValuePair.Key);
              fishCaught[ItemRegistry.ManuallyQualifyItemId(keyValuePair.Key, "(O)")] = keyValuePair.Value;
            }
          }
          if (allFarmer.toolBeingUpgraded.Value != null)
          {
            switch (allFarmer.toolBeingUpgraded.Value.InitialParentTileIndex)
            {
              case 13:
                allFarmer.toolBeingUpgraded.Value = ItemRegistry.Create<Tool>("(T)CopperTrashCan");
                break;
              case 14:
                allFarmer.toolBeingUpgraded.Value = ItemRegistry.Create<Tool>("(T)SteelTrashCan");
                break;
              case 15:
                allFarmer.toolBeingUpgraded.Value = ItemRegistry.Create<Tool>("(T)GoldTrashCan");
                break;
              case 16 /*0x10*/:
                allFarmer.toolBeingUpgraded.Value = ItemRegistry.Create<Tool>("(T)IridiumTrashCan");
                break;
            }
          }
          if (((int) allFarmer.obsolete_isMale ?? (allFarmer.IsMale ? 1 : 0)) == 0)
          {
            NetRef<Clothing>[] netRefArray = new NetRef<Clothing>[2]
            {
              allFarmer.shirtItem,
              allFarmer.pantsItem
            };
            foreach (NetRef<Clothing> netRef in netRefArray)
            {
              Clothing clothing = netRef.Value;
              if (clothing != null)
              {
                int? inTileSheetFemale = clothing.obsolete_indexInTileSheetFemale;
                int num1 = -1;
                if (inTileSheetFemale.GetValueOrDefault() > num1 & inTileSheetFemale.HasValue)
                {
                  int num2 = clothing.obsolete_indexInTileSheetFemale.Value;
                  if (clothing.HasTypeId("(S)"))
                    num2 += 1000;
                  ItemMetadata metadata = ItemRegistry.GetMetadata(clothing.TypeDefinitionId + num2.ToString());
                  if (metadata.Exists())
                  {
                    Clothing itemOrErrorItem = (Clothing) metadata.CreateItemOrErrorItem();
                    itemOrErrorItem.clothesColor.Value = clothing.clothesColor.Value;
                    itemOrErrorItem.modData.CopyFrom(clothing.modData);
                    netRef.Value = itemOrErrorItem;
                  }
                }
                clothing.obsolete_indexInTileSheetFemale = new int?();
              }
            }
          }
          foreach (Quest quest in (NetList<Quest, NetRef<Quest>>) allFarmer.questLog)
          {
            switch (quest)
            {
              case CraftingQuest craftingQuest:
                craftingQuest.ItemId.Value = ItemRegistry.ManuallyQualifyItemId(craftingQuest.ItemId.Value, craftingQuest.obsolete_isBigCraftable.GetValueOrDefault() ? "(BC)" : "(O)");
                craftingQuest.obsolete_isBigCraftable = new bool?();
                continue;
              case FishingQuest fishingQuest:
                fishingQuest.ItemId.Value = ItemRegistry.ManuallyQualifyItemId(fishingQuest.ItemId.Value, "(O)");
                continue;
              case ItemDeliveryQuest itemDeliveryQuest:
                itemDeliveryQuest.ItemId.Value = ItemRegistry.ManuallyQualifyItemId(itemDeliveryQuest.ItemId.Value, "(O)");
                if (itemDeliveryQuest.dailyQuest.Value)
                {
                  itemDeliveryQuest.moneyReward.Value = itemDeliveryQuest.GetGoldRewardPerItem(ItemRegistry.Create(itemDeliveryQuest.ItemId.Value));
                  continue;
                }
                continue;
              case ItemHarvestQuest itemHarvestQuest:
                itemHarvestQuest.ItemId.Value = ItemRegistry.ManuallyQualifyItemId(itemHarvestQuest.ItemId.Value, "(O)");
                continue;
              case LostItemQuest lostItemQuest:
                lostItemQuest.ItemId.Value = ItemRegistry.ManuallyQualifyItemId(lostItemQuest.ItemId.Value, "(O)");
                continue;
              case ResourceCollectionQuest resourceCollectionQuest:
                resourceCollectionQuest.ItemId.Value = ItemRegistry.ManuallyQualifyItemId(resourceCollectionQuest.ItemId.Value, "(O)");
                continue;
              case SecretLostItemQuest secretLostItemQuest:
                secretLostItemQuest.ItemId.Value = ItemRegistry.ManuallyQualifyItemId(secretLostItemQuest.ItemId.Value, "(O)");
                continue;
              default:
                continue;
            }
          }
        }
        foreach (SpecialOrder specialOrder in Game1.player.team.specialOrders)
        {
          if (specialOrder.itemToRemoveOnEnd.Value == "-1")
            specialOrder.itemToRemoveOnEnd.Value = (string) null;
        }
        Utility.ForEachLocation((Func<GameLocation, bool>) (location =>
        {
          if (location is IslandShrine islandShrine2)
            islandShrine2.AddMissingPedestals();
          foreach (KeyValuePair<Vector2, StardewValley.Object> pair in location.objects.Pairs)
          {
            if (pair.Value is Fence fence4 && fence4.obsolete_whichType.HasValue)
              fence4.ItemId = (string) null;
          }
          foreach (TerrainFeature terrainFeature in location.terrainFeatures.Values)
          {
            if (terrainFeature is FruitTree fruitTree2)
            {
              if (fruitTree2.obsolete_treeType != null)
              {
                string obsoleteTreeType = fruitTree2.obsolete_treeType;
                if (obsoleteTreeType != null && obsoleteTreeType.Length == 1)
                {
                  switch (obsoleteTreeType[0])
                  {
                    case '0':
                      fruitTree2.treeId.Value = "628";
                      goto label_25;
                    case '1':
                      fruitTree2.treeId.Value = "629";
                      goto label_25;
                    case '2':
                      fruitTree2.treeId.Value = "630";
                      goto label_25;
                    case '3':
                      fruitTree2.treeId.Value = "631";
                      goto label_25;
                    case '4':
                      fruitTree2.treeId.Value = "632";
                      goto label_25;
                    case '5':
                      fruitTree2.treeId.Value = "633";
                      goto label_25;
                    case '7':
                      fruitTree2.treeId.Value = "69";
                      goto label_25;
                    case '8':
                      fruitTree2.treeId.Value = "835";
                      goto label_25;
                  }
                }
                fruitTree2.treeId.Value = fruitTree2.obsolete_treeType;
label_25:
                fruitTree2.obsolete_treeType = (string) null;
              }
              if (fruitTree2.obsolete_fruitsOnTree.HasValue)
              {
                bool isGreenhouse = fruitTree2.Location.IsGreenhouse;
                try
                {
                  fruitTree2.Location.IsGreenhouse = true;
                  int num3 = 0;
                  while (true)
                  {
                    int num4 = num3;
                    int? obsoleteFruitsOnTree = fruitTree2.obsolete_fruitsOnTree;
                    int valueOrDefault = obsoleteFruitsOnTree.GetValueOrDefault();
                    if (num4 < valueOrDefault & obsoleteFruitsOnTree.HasValue)
                    {
                      fruitTree2.TryAddFruit();
                      ++num3;
                    }
                    else
                      break;
                  }
                }
                finally
                {
                  fruitTree2.Location.IsGreenhouse = isGreenhouse;
                }
                fruitTree2.obsolete_fruitsOnTree = new int?();
              }
            }
          }
          foreach (Building building in location.buildings)
          {
            if (building is FishPond fishPond2 && fishPond2.fishType.Value == "-1")
              fishPond2.fishType.Value = (string) null;
          }
          foreach (FarmAnimal farmAnimal in location.animals.Values)
          {
            if (farmAnimal.currentProduce.Value == "-1")
            {
              farmAnimal.currentProduce.Value = (string) null;
              farmAnimal.ReloadTextureIfNeeded();
            }
          }
          return true;
        }));
        return true;
      case SaveFixes.RemoveMeatFromAnimalBundle:
        string str3;
        if (Game1.netWorldState.Value.BundleData.TryGetValue("Pantry/4", out str3) && str3.StartsWith("Animal/"))
        {
          string[] strArray = str3.Split('/');
          List<string> list = ((IEnumerable<string>) ArgUtility.SplitBySpace(ArgUtility.Get(str3.Split('/'), 2))).ToList<string>();
          for (int index = 0; index < list.Count; index += 3)
          {
            string str4 = list[index];
            if ((str4 == "639" || str4 == "640" || str4 == "641" || str4 == "642" || str4 == "643") && ItemRegistry.ResolveMetadata("(O)" + str4) == null)
            {
              list.RemoveRange(index, Math.Min(3, list.Count - 1));
              index -= 3;
            }
          }
          strArray[2] = string.Join(" ", (IEnumerable<string>) list);
          Game1.netWorldState.Value.BundleData["Pantry/4"] = string.Join("/", strArray);
          bool[] array;
          if (Game1.netWorldState.Value.Bundles.TryGetValue(4, out array) && array.Length > list.Count)
          {
            Array.Resize<bool>(ref array, list.Count);
            Game1.netWorldState.Value.Bundles.Remove(4);
            Game1.netWorldState.Value.Bundles.Add(4, array);
          }
        }
        return true;
      case SaveFixes.RemoveMasteryRoomFoliage:
        GameLocation locationFromName3 = Game1.getLocationFromName("Forest");
        if (locationFromName3 != null)
        {
          locationFromName3.largeTerrainFeatures.RemoveWhere((Func<LargeTerrainFeature, bool>) (feature => feature.Tile == new Vector2(100f, 74f) || feature.Tile == new Vector2(101f, 76f)));
          StardewValley.Object object8;
          if (locationFromName3.terrainFeatures.GetValueOrDefault(new Vector2(98f, 75f)) is Tree valueOrDefault && valueOrDefault.tapped.Value && locationFromName3.objects.TryGetValue(new Vector2(98f, 75f), out object8))
          {
            if (object8.readyForHarvest.Value && (NetFieldBase<StardewValley.Object, NetRef<StardewValley.Object>>) object8.heldObject != (NetRef<StardewValley.Object>) null)
              Game1.player.team.returnedDonations.Add((Item) object8.heldObject.Value);
            Game1.player.team.returnedDonations.Add((Item) object8);
            Game1.player.team.newLostAndFoundItems.Value = true;
          }
          locationFromName3.terrainFeatures.Remove(new Vector2(98f, 75f));
        }
        return true;
      case SaveFixes.AddTownTrees:
        GameLocation locationFromName4 = Game1.getLocationFromName("Town");
        Layer layer = locationFromName4.map?.GetLayer("Paths");
        if (layer == null)
          return false;
        for (int x = 0; x < locationFromName4.map.Layers[0].LayerWidth; ++x)
        {
          for (int y = 0; y < locationFromName4.map.Layers[0].LayerHeight; ++y)
          {
            Tile tile = layer.Tiles[x, y];
            if (tile != null)
            {
              Vector2 vector2 = new Vector2((float) x, (float) y);
              string treeId;
              int? growthStageOnLoad;
              bool isFruitTree;
              if (locationFromName4.TryGetTreeIdForTile(tile, out treeId, out growthStageOnLoad, out int? _, out isFruitTree) && locationFromName4.GetFurnitureAt(vector2) == null && !locationFromName4.terrainFeatures.ContainsKey(vector2) && !locationFromName4.objects.ContainsKey(vector2))
              {
                if (isFruitTree)
                  locationFromName4.terrainFeatures.Add(vector2, (TerrainFeature) new FruitTree(treeId, growthStageOnLoad ?? 4));
                else
                  locationFromName4.terrainFeatures.Add(vector2, (TerrainFeature) new Tree(treeId, growthStageOnLoad ?? 5));
              }
            }
          }
        }
        return true;
      case SaveFixes.MapAdjustments_1_6:
        Game1.getLocationFromName("BusStop").shiftContents(10, 0);
        List<Point> pointList1 = new List<Point>();
        pointList1.Add(new Point(78, 17));
        pointList1.Add(new Point(79, 17));
        pointList1.Add(new Point(79, 18));
        pointList1.Add(new Point(80 /*0x50*/, 17));
        pointList1.Add(new Point(80 /*0x50*/, 18));
        pointList1.Add(new Point(80 /*0x50*/, 19));
        pointList1.Add(new Point(81, 16 /*0x10*/));
        pointList1.Add(new Point(81, 17));
        pointList1.Add(new Point(81, 18));
        pointList1.Add(new Point(81, 19));
        pointList1.Add(new Point(82, 15));
        pointList1.Add(new Point(82, 16 /*0x10*/));
        pointList1.Add(new Point(82, 17));
        pointList1.Add(new Point(82, 18));
        pointList1.Add(new Point(83, 13));
        pointList1.Add(new Point(83, 14));
        pointList1.Add(new Point(83, 15));
        pointList1.Add(new Point(83, 16 /*0x10*/));
        pointList1.Add(new Point(83, 17));
        pointList1.Add(new Point(84, 13));
        pointList1.Add(new Point(84, 14));
        pointList1.Add(new Point(84, 15));
        pointList1.Add(new Point(84, 16 /*0x10*/));
        pointList1.Add(new Point(84, 17));
        pointList1.Add(new Point(84, 18));
        pointList1.Add(new Point(85, 13));
        pointList1.Add(new Point(85, 14));
        pointList1.Add(new Point(85, 15));
        pointList1.Add(new Point(85, 16 /*0x10*/));
        pointList1.Add(new Point(85, 17));
        pointList1.Add(new Point(85, 18));
        pointList1.Add(new Point(86, 14));
        pointList1.Add(new Point(86, 15));
        pointList1.Add(new Point(86, 16 /*0x10*/));
        pointList1.Add(new Point(86, 17));
        pointList1.Add(new Point(86, 18));
        pointList1.Add(new Point(87, 14));
        pointList1.Add(new Point(87, 15));
        pointList1.Add(new Point(87, 16 /*0x10*/));
        pointList1.Add(new Point(87, 17));
        pointList1.Add(new Point(87, 18));
        pointList1.Add(new Point(87, 19));
        pointList1.Add(new Point(88, 13));
        pointList1.Add(new Point(88, 14));
        pointList1.Add(new Point(88, 15));
        pointList1.Add(new Point(88, 16 /*0x10*/));
        pointList1.Add(new Point(88, 17));
        pointList1.Add(new Point(88, 18));
        pointList1.Add(new Point(88, 19));
        pointList1.Add(new Point(89, 13));
        pointList1.Add(new Point(89, 14));
        pointList1.Add(new Point(89, 15));
        pointList1.Add(new Point(89, 16 /*0x10*/));
        pointList1.Add(new Point(89, 17));
        pointList1.Add(new Point(79, 21));
        pointList1.Add(new Point(79, 22));
        pointList1.Add(new Point(79, 23));
        pointList1.Add(new Point(79, 24));
        pointList1.Add(new Point(79, 25));
        pointList1.Add(new Point(76, 16 /*0x10*/));
        pointList1.Add(new Point(75, 16 /*0x10*/));
        pointList1.Add(new Point(74, 16 /*0x10*/));
        GameLocation locationFromName5 = Game1.getLocationFromName("Mountain");
        foreach (Point tile in pointList1)
          locationFromName5.cleanUpTileForMapOverride(tile);
        locationFromName5.terrainFeatures.Remove(new Vector2(79f, 20f));
        locationFromName5.terrainFeatures.Remove(new Vector2(79f, 19f));
        locationFromName5.terrainFeatures.Remove(new Vector2(79f, 16f));
        locationFromName5.terrainFeatures.Remove(new Vector2(80f, 20f));
        locationFromName5.largeTerrainFeatures.Remove(locationFromName5.getLargeTerrainFeatureAt(82, 11));
        locationFromName5.largeTerrainFeatures.Remove(locationFromName5.getLargeTerrainFeatureAt(86, 13));
        locationFromName5.largeTerrainFeatures.Remove(locationFromName5.getLargeTerrainFeatureAt(85, 16 /*0x10*/));
        locationFromName5.largeTerrainFeatures.Add((LargeTerrainFeature) new Bush(new Vector2(81f, 9f), 1, locationFromName5));
        locationFromName5.largeTerrainFeatures.Add((LargeTerrainFeature) new Bush(new Vector2(84f, 18f), 2, locationFromName5));
        locationFromName5.largeTerrainFeatures.Add((LargeTerrainFeature) new Bush(new Vector2(87f, 19f), 1, locationFromName5));
        List<Point> pointList2 = new List<Point>();
        pointList2.Add(new Point(92, 10));
        pointList2.Add(new Point(93, 10));
        pointList2.Add(new Point(94, 10));
        pointList2.Add(new Point(93, 13));
        pointList2.Add(new Point(95, 13));
        pointList2.Add(new Point(92, 5));
        pointList2.Add(new Point(92, 6));
        pointList2.Add(new Point(97, 9));
        pointList2.Add(new Point(91, 10));
        pointList2.Add(new Point(91, 9));
        pointList2.Add(new Point(91, 8));
        pointList2.Add(new Point(93, 11));
        pointList2.Add(new Point(94, 11));
        pointList2.Add(new Point(95, 11));
        GameLocation locationFromName6 = Game1.getLocationFromName("Town");
        foreach (Point tile in pointList2)
          locationFromName6.cleanUpTileForMapOverride(tile);
        locationFromName6.loadPathsLayerObjectsInArea(103, 16 /*0x10*/, 16 /*0x10*/, 27);
        locationFromName6.loadPathsLayerObjectsInArea(120, 57, 7, 12);
        locationFromName6.largeTerrainFeatures.Remove(locationFromName6.getLargeTerrainFeatureAt(105, 42));
        locationFromName6.largeTerrainFeatures.Remove(locationFromName6.getLargeTerrainFeatureAt(108, 42));
        List<Point> pointList3 = new List<Point>();
        pointList3.Add(new Point(63 /*0x3F*/, 77));
        pointList3.Add(new Point(63 /*0x3F*/, 78));
        pointList3.Add(new Point(63 /*0x3F*/, 79));
        pointList3.Add(new Point(63 /*0x3F*/, 80 /*0x50*/));
        pointList3.Add(new Point(46, 26));
        pointList3.Add(new Point(46, 27));
        pointList3.Add(new Point(46, 28));
        pointList3.Add(new Point(46, 29));
        GameLocation locationFromName7 = Game1.getLocationFromName("Forest");
        foreach (Point tile in pointList3)
          locationFromName7.cleanUpTileForMapOverride(tile);
        locationFromName7.largeTerrainFeatures.Add((LargeTerrainFeature) new Bush(new Vector2(54f, 8f), 0, locationFromName7));
        locationFromName7.largeTerrainFeatures.Add((LargeTerrainFeature) new Bush(new Vector2(58f, 8f), 0, locationFromName7));
        return true;
      case SaveFixes.MigrateWalletItems:
        Farmer masterPlayer1 = Game1.MasterPlayer;
        Farmer farmer1 = masterPlayer1;
        bool? nullable;
        int num5;
        if (!masterPlayer1.hasRustyKey)
        {
          nullable = masterPlayer1.obsolete_hasRustyKey;
          num5 = !nullable.HasValue ? 0 : (nullable.GetValueOrDefault() ? 1 : 0);
        }
        else
          num5 = 1;
        farmer1.hasRustyKey = num5 != 0;
        Farmer farmer2 = masterPlayer1;
        int num6;
        if (!masterPlayer1.hasSkullKey)
        {
          nullable = masterPlayer1.obsolete_hasSkullKey;
          num6 = !nullable.HasValue ? 0 : (nullable.GetValueOrDefault() ? 1 : 0);
        }
        else
          num6 = 1;
        farmer2.hasSkullKey = num6 != 0;
        Farmer farmer3 = masterPlayer1;
        int num7;
        if (!masterPlayer1.canUnderstandDwarves)
        {
          nullable = masterPlayer1.obsolete_canUnderstandDwarves;
          num7 = !nullable.HasValue ? 0 : (nullable.GetValueOrDefault() ? 1 : 0);
        }
        else
          num7 = 1;
        farmer3.canUnderstandDwarves = num7 != 0;
        masterPlayer1.obsolete_hasRustyKey = new bool?();
        masterPlayer1.obsolete_hasSkullKey = new bool?();
        masterPlayer1.obsolete_canUnderstandDwarves = new bool?();
        foreach (Farmer allFarmer in Game1.getAllFarmers())
        {
          Farmer farmer4 = allFarmer;
          int num8;
          if (!allFarmer.hasClubCard)
          {
            nullable = allFarmer.obsolete_hasClubCard;
            num8 = !nullable.HasValue ? 0 : (nullable.GetValueOrDefault() ? 1 : 0);
          }
          else
            num8 = 1;
          farmer4.hasClubCard = num8 != 0;
          Farmer farmer5 = allFarmer;
          int num9;
          if (!allFarmer.hasDarkTalisman)
          {
            nullable = allFarmer.obsolete_hasDarkTalisman;
            num9 = !nullable.HasValue ? 0 : (nullable.GetValueOrDefault() ? 1 : 0);
          }
          else
            num9 = 1;
          farmer5.hasDarkTalisman = num9 != 0;
          Farmer farmer6 = allFarmer;
          int num10;
          if (!allFarmer.hasMagicInk)
          {
            nullable = allFarmer.obsolete_hasMagicInk;
            num10 = !nullable.HasValue ? 0 : (nullable.GetValueOrDefault() ? 1 : 0);
          }
          else
            num10 = 1;
          farmer6.hasMagicInk = num10 != 0;
          Farmer farmer7 = allFarmer;
          int num11;
          if (!allFarmer.hasMagnifyingGlass)
          {
            nullable = allFarmer.obsolete_hasMagnifyingGlass;
            num11 = !nullable.HasValue ? 0 : (nullable.GetValueOrDefault() ? 1 : 0);
          }
          else
            num11 = 1;
          farmer7.hasMagnifyingGlass = num11 != 0;
          Farmer farmer8 = allFarmer;
          int num12;
          if (!allFarmer.hasSpecialCharm)
          {
            nullable = allFarmer.obsolete_hasSpecialCharm;
            num12 = !nullable.HasValue ? 0 : (nullable.GetValueOrDefault() ? 1 : 0);
          }
          else
            num12 = 1;
          farmer8.hasSpecialCharm = num12 != 0;
          Farmer farmer9 = allFarmer;
          int num13;
          if (!allFarmer.HasTownKey)
          {
            nullable = allFarmer.obsolete_hasTownKey;
            num13 = !nullable.HasValue ? 0 : (nullable.GetValueOrDefault() ? 1 : 0);
          }
          else
            num13 = 1;
          farmer9.HasTownKey = num13 != 0;
          Farmer farmer10 = allFarmer;
          int num14;
          if (!allFarmer.hasUnlockedSkullDoor)
          {
            nullable = allFarmer.obsolete_hasUnlockedSkullDoor;
            num14 = !nullable.HasValue ? 0 : (nullable.GetValueOrDefault() ? 1 : 0);
          }
          else
            num14 = 1;
          farmer10.hasUnlockedSkullDoor = num14 != 0;
          allFarmer.obsolete_hasClubCard = new bool?();
          allFarmer.obsolete_hasDarkTalisman = new bool?();
          allFarmer.obsolete_hasMagicInk = new bool?();
          allFarmer.obsolete_hasMagnifyingGlass = new bool?();
          allFarmer.obsolete_hasSpecialCharm = new bool?();
          allFarmer.obsolete_hasTownKey = new bool?();
          allFarmer.obsolete_hasUnlockedSkullDoor = new bool?();
          allFarmer.obsolete_daysMarried = new int?();
        }
        return true;
      case SaveFixes.MigrateResourceClumps:
        Utility.ForEachLocation((Func<GameLocation, bool>) (location =>
        {
          switch (location)
          {
            case Forest forest2:
              if (forest2.obsolete_log != null)
              {
                forest2.resourceClumps.Add(forest2.obsolete_log);
                forest2.obsolete_log = (ResourceClump) null;
                break;
              }
              break;
            case Woods woods2:
              woods2.DayUpdate(Game1.dayOfMonth);
              break;
          }
          return true;
        }), false);
        return true;
      case SaveFixes.MigrateFishingRodAttachmentSlots:
        Utility.ForEachItem((Func<Item, bool>) (item =>
        {
          if (item is FishingRod fishingRod2)
          {
            ToolData toolData = fishingRod2.GetToolData();
            if (toolData == null || toolData.AttachmentSlots < 0 || fishingRod2.AttachmentSlotsCount <= toolData.AttachmentSlots)
              return true;
            INetSerializable parent = fishingRod2.attachments.Parent;
            fishingRod2.attachments.Parent = (INetSerializable) null;
            try
            {
              int index = fishingRod2.AttachmentSlotsCount - 1;
              while (fishingRod2.AttachmentSlotsCount > toolData.AttachmentSlots)
              {
                if (index >= 0)
                {
                  if (fishingRod2.attachments.Count <= index)
                    --fishingRod2.AttachmentSlotsCount;
                  else if (fishingRod2.attachments[index] == null)
                    --fishingRod2.AttachmentSlotsCount;
                  --index;
                }
                else
                  break;
              }
            }
            finally
            {
              fishingRod2.attachments.Parent = parent;
            }
          }
          return true;
        }));
        return true;
      case SaveFixes.MoveSlimeHutches:
        Farm farm3 = Game1.getFarm();
        for (int index = farm3.buildings.Count - 1; index >= 0; --index)
        {
          if (farm3.buildings[index].buildingType.Value == "Slime Hutch")
          {
            farm3.buildings[index].tileX.Value += 2;
            farm3.buildings[index].tileY.Value += 2;
            farm3.buildings[index].ReloadBuildingData();
            farm3.buildings[index].updateInteriorWarps();
          }
        }
        return true;
      case SaveFixes.AddLocationsVisited:
        foreach (Farmer allFarmer in Game1.getAllFarmers())
        {
          NetStringHashSet locationsVisited = allFarmer.locationsVisited;
          Farmer masterPlayer2 = Game1.MasterPlayer;
          locationsVisited.AddRange<string>((IEnumerable<string>) new string[30]
          {
            "Farm",
            "FarmHouse",
            "FarmCave",
            "Cellar",
            "Town",
            "JoshHouse",
            "HaleyHouse",
            "SamHouse",
            "Blacksmith",
            "ManorHouse",
            "SeedShop",
            "Saloon",
            "Trailer",
            "Hospital",
            "HarveyRoom",
            "ArchaeologyHouse",
            "JojaMart",
            "Beach",
            "ElliottHouse",
            "FishShop",
            "Mountain",
            "ScienceHouse",
            "SebastianRoom",
            "Tent",
            "Forest",
            "AnimalShop",
            "LeahHouse",
            "Backwoods",
            "BusStop",
            "Tunnel"
          });
          if (masterPlayer2.mailReceived.Contains("ccPantry"))
            locationsVisited.Add("Greenhouse");
          if (Game1.isLocationAccessible("CommunityCenter"))
            locationsVisited.Add("CommunityCenter");
          if (allFarmer.eventsSeen.Contains("100162"))
            locationsVisited.Add("Mine");
          if (masterPlayer2.mailReceived.Contains("ccVault"))
            locationsVisited.AddRange<string>((IEnumerable<string>) new string[2]
            {
              "Desert",
              "SkullCave"
            });
          if (allFarmer.eventsSeen.Contains("67"))
            locationsVisited.Add("SandyHouse");
          if (masterPlayer2.mailReceived.Contains("bouncerGone"))
            locationsVisited.Add("Club");
          if (Game1.isLocationAccessible("Railroad"))
            locationsVisited.AddRange<string>((IEnumerable<string>) new string[4]
            {
              "Railroad",
              "BathHouse_Entry",
              allFarmer.IsMale ? "BathHouse_MensLocker" : "BathHouse_WomensLocker",
              "BathHouse_Pool"
            });
          if (masterPlayer2.mailReceived.Contains("Farm_Eternal"))
            locationsVisited.Add("Summit");
          if (masterPlayer2.mailReceived.Contains("witchStatueGone"))
            locationsVisited.AddRange<string>((IEnumerable<string>) new string[2]
            {
              "WitchSwamp",
              "WitchWarpCave"
            });
          if (masterPlayer2.mailReceived.Contains("henchmanGone"))
            locationsVisited.Add("WitchHut");
          if (allFarmer.mailReceived.Contains("beenToWoods"))
            locationsVisited.Add("Woods");
          if (Forest.isWizardHouseUnlocked())
          {
            locationsVisited.Add("WizardHouse");
            if (allFarmer.getFriendshipHeartLevelForNPC("Wizard") >= 4)
              locationsVisited.Add("WizardHouseBasement");
          }
          if (allFarmer.mailReceived.Add("guildMember"))
            locationsVisited.Add("AdventureGuild");
          if (allFarmer.mailReceived.Contains("OpenedSewer"))
            locationsVisited.Add("Sewer");
          if (allFarmer.mailReceived.Contains("krobusUnseal"))
            locationsVisited.Add("BugLand");
          if (masterPlayer2.mailReceived.Contains("abandonedJojaMartAccessible"))
            locationsVisited.Add("AbandonedJojaMart");
          if (masterPlayer2.mailReceived.Contains("ccMovieTheater"))
            locationsVisited.Add("MovieTheater");
          if (masterPlayer2.mailReceived.Contains("pamHouseUpgrade"))
            locationsVisited.Add("Trailer_Big");
          if (allFarmer.getFriendshipHeartLevelForNPC("Caroline") >= 2)
            locationsVisited.Add("Sunroom");
          if (Game1.year > 1 || Game1.season == Season.Winter && Game1.dayOfMonth >= 15)
            locationsVisited.AddRange<string>((IEnumerable<string>) new string[3]
            {
              "BeachNightMarket",
              "MermaidHouse",
              "Submarine"
            });
          if (allFarmer.mailReceived.Contains("willyBackRoomInvitation"))
            locationsVisited.Add("BoatTunnel");
          if (allFarmer.mailReceived.Contains("Visited_Island"))
          {
            locationsVisited.AddRange<string>((IEnumerable<string>) new string[4]
            {
              "IslandSouth",
              "IslandEast",
              "IslandHut",
              "IslandShrine"
            });
            if (masterPlayer2.mailReceived.Contains("Island_FirstParrot"))
              locationsVisited.AddRange<string>((IEnumerable<string>) new string[2]
              {
                "IslandNorth",
                "IslandFieldOffice"
              });
            if (masterPlayer2.mailReceived.Contains("islandNorthCaveOpened"))
              locationsVisited.Add("IslandNorthCave1");
            if (masterPlayer2.mailReceived.Contains("reachedCaldera"))
              locationsVisited.Add("Caldera");
            if (masterPlayer2.mailReceived.Contains("Island_Turtle"))
              locationsVisited.AddRange<string>((IEnumerable<string>) new string[2]
              {
                "IslandWest",
                "IslandWestCave1"
              });
            if (masterPlayer2.mailReceived.Contains("Island_UpgradeHouse"))
              locationsVisited.AddRange<string>((IEnumerable<string>) new string[2]
              {
                "IslandFarmHouse",
                "IslandFarmCave"
              });
            if (masterPlayer2.team.collectedNutTracker.Contains("Bush_CaptainRoom_2_4"))
              locationsVisited.Add("CaptainRoom");
            if (IslandWest.IsQiWalnutRoomDoorUnlocked(out int _))
              locationsVisited.Add("QiNutRoom");
            if (masterPlayer2.mailReceived.Contains("Island_Resort"))
              locationsVisited.AddRange<string>((IEnumerable<string>) new string[2]
              {
                "IslandSouthEast",
                "IslandSouthEastCave"
              });
          }
          if (masterPlayer2.mailReceived.Contains("leoMoved"))
            locationsVisited.Add("LeoTreeHouse");
        }
        return true;
      case SaveFixes.MarkStarterGiftBoxes:
        Utility.ForEachLocation((Func<GameLocation, bool>) (location =>
        {
          if (location is FarmHouse)
          {
            foreach (StardewValley.Object object9 in location.objects.Values)
            {
              if (object9 is Chest chest2 && chest2.giftbox.Value && !chest2.playerChest.Value)
                chest2.giftboxIsStarterGift.Value = true;
            }
          }
          return true;
        }));
        return true;
      case SaveFixes.MigrateMailEventsToTriggerActions:
        Dictionary<string, string> dictionary2 = new Dictionary<string, string>()
        {
          ["2346097"] = "Mail_Abigail_8heart",
          ["2346096"] = "Mail_Penny_10heart",
          ["2346095"] = "Mail_Elliott_8heart",
          ["2346094"] = "Mail_Elliott_10heart",
          ["3333094"] = "Mail_Pierre_ExtendedHours",
          ["2346093"] = "Mail_Harvey_10heart",
          ["2346092"] = "Mail_Sam_10heart",
          ["2346091"] = "Mail_Alex_10heart",
          ["68"] = "Mail_Mom_5K",
          ["69"] = "Mail_Mom_15K",
          ["70"] = "Mail_Mom_32K",
          ["71"] = "Mail_Mom_120K",
          ["72"] = "Mail_Dad_5K",
          ["73"] = "Mail_Dad_15K",
          ["74"] = "Mail_Dad_32K",
          ["75"] = "Mail_Dad_120K",
          ["76"] = "Mail_Tribune_UpAndComing",
          ["706"] = "Mail_Pierre_Fertilizers",
          ["707"] = "Mail_Pierre_FertilizersHighQuality",
          ["909"] = "Mail_Robin_Woodchipper",
          ["3872126"] = "Mail_Willy_BackRoomUnlocked"
        };
        Dictionary<string, string> dictionary3 = new Dictionary<string, string>()
        {
          ["2111194"] = "Mail_Emily_8heart",
          ["2111294"] = "Mail_Emily_10heart",
          ["3912126"] = "Mail_Elliott_Tour1",
          ["3912127"] = "Mail_Elliott_Tour2",
          ["3912128"] = "Mail_Elliott_Tour3",
          ["3912129"] = "Mail_Elliott_Tour4",
          ["3912130"] = "Mail_Elliott_Tour5",
          ["3912131"] = "Mail_Elliott_Tour6"
        };
        foreach (Farmer allFarmer in Game1.getAllFarmers())
        {
          NetStringHashSet eventsSeen = allFarmer.eventsSeen;
          NetStringHashSet triggerActionsRun = allFarmer.triggerActionsRun;
          foreach (KeyValuePair<string, string> keyValuePair in dictionary2)
          {
            if (eventsSeen.Remove(keyValuePair.Key))
              triggerActionsRun.Add(keyValuePair.Value);
          }
          foreach (KeyValuePair<string, string> keyValuePair in dictionary3)
          {
            if (eventsSeen.Contains(keyValuePair.Key))
              triggerActionsRun.Add(keyValuePair.Value);
          }
        }
        return true;
      case SaveFixes.ShiftFarmHouseFurnitureForExpansion:
        Utility.ForEachLocation((Func<GameLocation, bool>) (location =>
        {
          FarmHouse house = location as FarmHouse;
          if (house != null && house.upgradeLevel >= 2)
          {
            house.shiftContents(15, 10, (Func<Vector2, object, bool>) ((tile, entity) =>
            {
              switch (entity)
              {
                case BedFurniture _:
                  int x = (int) tile.X;
                  int y = (int) tile.Y;
                  return house.doesTileHaveProperty(x, y, "DefaultBedPosition", "Back") == null && house.doesTileHaveProperty(x, y, "DefaultChildBedPosition", "Back") == null;
                case Furniture furniture2:
                  if (furniture2.QualifiedItemId == "(F)1792")
                  {
                    Vector2 vector2 = tile - Utility.PointToVector2(house.getFireplacePoint());
                    return (double) Math.Abs(vector2.X) > 9.9999997473787516E-06 || (double) Math.Abs(vector2.Y) > 9.9999997473787516E-06;
                  }
                  break;
              }
              return true;
            }));
            foreach (NPC character3 in house.characters)
            {
              if (!character3.TilePoint.Equals(house.getKitchenStandingSpot()))
                character3.Position += new Vector2(15f, 10f) * 64f;
              if (house.hasTileAt(character3.TilePoint, "Buildings") || !house.hasTileAt(character3.TilePoint, "Back"))
              {
                Vector2 tileForCharacter = Utility.recursiveFindOpenTileForCharacter((Character) character3, (GameLocation) house, Utility.PointToVector2(house.getKitchenStandingSpot()), 99, false);
                if (tileForCharacter != Vector2.Zero)
                  character3.setTileLocation(tileForCharacter);
                else
                  character3.setTileLocation(Utility.PointToVector2(house.getKitchenStandingSpot()));
              }
            }
          }
          return true;
        }));
        foreach (Farmer allFarmer in Game1.getAllFarmers())
        {
          if (allFarmer.currentLocation is FarmHouse currentLocation && currentLocation.upgradeLevel >= 2)
          {
            Farmer farmer11 = allFarmer;
            farmer11.Position = farmer11.Position + new Vector2(15f, 10f) * 64f;
          }
        }
        return true;
      case SaveFixes.MigratePreservesTo16:
        ObjectDataDefinition objTypeDefinition = ItemRegistry.GetObjectTypeDefinition();
        Utility.ForEachItemContext(new ForEachItemDelegate(HandleItem));
        return true;

        bool HandleItem(in ForEachItemContext context)
        {
          if (!(context.Item is StardewValley.Object object10))
            return true;
          string str = object10.preservedParentSheetIndex.Value;
          if (str == null || str == "0")
          {
            object10.preservedParentSheetIndex.Value = (string) null;
            return true;
          }
          if (!object10.isRecipe.Value && !(object10 is ColoredObject))
          {
            StardewValley.Object object2 = (StardewValley.Object) null;
            switch (context.Item.QualifiedItemId)
            {
              case "(O)344":
                object2 = objTypeDefinition.CreateFlavoredJelly(ItemRegistry.Create<StardewValley.Object>("(O)" + str));
                break;
              case "(O)350":
                object2 = objTypeDefinition.CreateFlavoredJuice(ItemRegistry.Create<StardewValley.Object>("(O)" + str));
                break;
              case "(O)342":
                object2 = objTypeDefinition.CreateFlavoredPickle(ItemRegistry.Create<StardewValley.Object>("(O)" + str));
                break;
              case "(O)348":
                object2 = objTypeDefinition.CreateFlavoredWine(ItemRegistry.Create<StardewValley.Object>("(O)" + str));
                break;
            }
            if (object2 != null)
            {
              object2.Name = object10.Name;
              object2.Price = object10.Price;
              object2.Stack = object10.Stack;
              object2.Quality = object10.Quality;
              object2.CanBeGrabbed = object10.CanBeGrabbed;
              object2.CanBeSetDown = object10.CanBeSetDown;
              object2.Edibility = object10.Edibility;
              object2.Fragility = object10.Fragility;
              object2.HasBeenInInventory = object10.HasBeenInInventory;
              object2.questId.Value = object10.questId.Value;
              object2.questItem.Value = object10.questItem.Value;
              foreach (KeyValuePair<string, string> pair in object10.modData.Pairs)
                object2.modData[pair.Key] = pair.Value;
              context.ReplaceItemWith((Item) object2);
            }
          }
          return true;
        }
      case SaveFixes.MigrateQuestDataTo16:
        Lazy<XmlSerializer> serializer = new Lazy<XmlSerializer>((Func<XmlSerializer>) (() => new XmlSerializer(typeof (SaveMigrator_1_6.LegacyDescriptionElement), new Type[3]
        {
          typeof (DescriptionElement),
          typeof (Character),
          typeof (Item)
        })));
        foreach (Farmer allFarmer in Game1.getAllFarmers())
        {
          foreach (Quest quest in (NetList<Quest, NetRef<Quest>>) allFarmer.questLog)
          {
            foreach (FieldInfo field in quest.GetType().GetFields())
            {
              if (field.FieldType == typeof (NetDescriptionElementList))
              {
                NetDescriptionElementList descriptionElementList = (NetDescriptionElementList) field.GetValue((object) quest);
                if (descriptionElementList != null)
                {
                  foreach (DescriptionElement element in (NetList<DescriptionElement, NetDescriptionElementRef>) descriptionElementList)
                    SaveMigrator_1_6.MigrateLegacyDescriptionElement(serializer, element);
                }
              }
              else if (field.FieldType == typeof (NetDescriptionElementRef))
              {
                NetDescriptionElementRef descriptionElementRef = (NetDescriptionElementRef) field.GetValue((object) quest);
                SaveMigrator_1_6.MigrateLegacyDescriptionElement(serializer, descriptionElementRef?.Value);
              }
            }
          }
        }
        return true;
      case SaveFixes.SetBushesInPots:
        Utility.ForEachItem((Func<Item, bool>) (item =>
        {
          if (item is IndoorPot indoorPot2 && indoorPot2.bush.Value != null)
            indoorPot2.bush.Value.inPot.Value = true;
          return true;
        }));
        return true;
      case SaveFixes.FixItemsNotMarkedAsInInventory:
        foreach (Farmer allFarmer in Game1.getAllFarmers())
        {
          foreach (Item equippedItem in allFarmer.GetEquippedItems())
            equippedItem.HasBeenInInventory = true;
          foreach (Item obj in allFarmer.Items)
          {
            if (obj != null)
              obj.HasBeenInInventory = true;
          }
        }
        return true;
      case SaveFixes.BetaFixesFor16:
        Utility.ForEachItem((Func<Item, bool>) (item =>
        {
          switch (item)
          {
            case Boots _:
            case Clothing _:
            case Hat _:
              item.FixStackSize();
              break;
          }
          return true;
        }));
        return true;
      case SaveFixes.FixBasicWines:
        Utility.ForEachItem((Func<Item, bool>) (item =>
        {
          if (item.ParentSheetIndex == 348 && item.QualifiedItemId.Equals("(O)348"))
            item.ParentSheetIndex = 123;
          return true;
        }));
        return true;
      case SaveFixes.ResetForges_1_6:
        SaveMigrator_1_5.ResetForges();
        return true;
      case SaveFixes.RestoreAncientSeedRecipe_1_6:
        foreach (Farmer allFarmer in Game1.getAllFarmers())
        {
          if (allFarmer.mailReceived.Contains("museumCollectedRewardO_499_1"))
            allFarmer.craftingRecipes.TryAdd("Ancient Seeds", 0);
        }
        return true;
      case SaveFixes.FixInstancedInterior:
        Utility.ForEachBuilding((Func<Building, bool>) (building =>
        {
          if (building.GetIndoorsType() == IndoorsType.Instanced)
          {
            GameLocation indoors = building.GetIndoors();
            if (indoors.uniqueName.Value == null)
              indoors.uniqueName.Value = (building.GetData()?.IndoorMap ?? indoors.Name) + GuidHelper.NewGuid().ToString();
            if (indoors is AnimalHouse animalHouse2)
              animalHouse2.animalsThatLiveHere.RemoveWhere((Func<long, bool>) (id => Utility.getAnimal(id)?.home != building));
          }
          return true;
        }));
        return true;
      case SaveFixes.FixNonInstancedInterior:
        Utility.ForEachBuilding((Func<Building, bool>) (building =>
        {
          if (building.GetIndoorsType() == IndoorsType.Global)
            building.GetIndoors().uniqueName.Value = (string) null;
          return true;
        }));
        return true;
      case SaveFixes.PopulateConstructedBuildings:
        Utility.ForEachBuilding((Func<Building, bool>) (building =>
        {
          if (!string.IsNullOrWhiteSpace(building.buildingType.Value))
          {
            if (!building.isUnderConstruction(false))
              Game1.player.team.constructedBuildings.Add(building.buildingType.Value);
            for (BuildingData data = building.GetData(); !string.IsNullOrWhiteSpace(data?.BuildingToUpgrade); Building.TryGetData(data.BuildingToUpgrade, out data))
              Game1.player.team.constructedBuildings.Add(data.BuildingToUpgrade);
          }
          return true;
        }), false);
        return true;
      case SaveFixes.FixRacoonQuestCompletion:
        if (NetWorldState.checkAnywhereForWorldStateID("forestStumpFixed"))
        {
          Game1.player.removeQuest("134");
          foreach (Farmer offlineFarmhand in Game1.getOfflineFarmhands())
            offlineFarmhand.removeQuest("134");
        }
        return true;
      case SaveFixes.RestoreDwarvish:
        if (Game1.player.hasOrWillReceiveMail("museumCollectedRewardO_326_1"))
          Game1.player.canUnderstandDwarves = true;
        return true;
      case SaveFixes.FixTubOFlowers:
        Utility.ForEachItem((Func<Item, bool>) (item =>
        {
          if (item.QualifiedItemId == "(BC)109")
          {
            item.ItemId = "108";
            item.ResetParentSheetIndex();
            if (item is StardewValley.Object object12)
            {
              bool? isOutdoors = object12.Location?.IsOutdoors;
              if (isOutdoors.HasValue && isOutdoors.GetValueOrDefault())
              {
                switch (object12.Location.GetSeason())
                {
                  case Season.Fall:
                  case Season.Winter:
                    item.ParentSheetIndex = 109;
                    break;
                }
              }
            }
          }
          return true;
        }));
        return true;
      case SaveFixes.MigrateStatFields:
        foreach (Farmer allFarmer in Game1.getAllFarmers())
        {
          Stats stats = allFarmer.stats;
          SerializableDictionary<string, uint> obsoleteStatDictionary = stats.obsolete_stat_dictionary;
          // ISSUE: explicit non-virtual call
          if ((obsoleteStatDictionary != null ? (__nonvirtual (obsoleteStatDictionary.Count) > 0 ? 1 : 0) : 0) != 0)
          {
            foreach (KeyValuePair<string, uint> obsoleteStat in (Dictionary<string, uint>) stats.obsolete_stat_dictionary)
            {
              uint num15;
              stats.Values[obsoleteStat.Key] = stats.Values.TryGetValue(obsoleteStat.Key, out num15) ? num15 + obsoleteStat.Value : obsoleteStat.Value;
            }
            stats.obsolete_stat_dictionary = (SerializableDictionary<string, uint>) null;
          }
          uint num16;
          if (stats.Values.TryGetValue("walnutsFound", out num16))
          {
            Game1.netWorldState.Value.GoldenWalnutsFound += (int) num16;
            stats.Values.Remove("walnutsFound");
          }
          foreach (KeyValuePair<string, uint> keyValuePair in stats.Values.ToArray<KeyValuePair<string, uint>>())
          {
            if (keyValuePair.Value == 0U)
              stats.Values.Remove(keyValuePair.Key);
          }
          if (stats.AverageBedtime == 0U)
            stats.Set("averageBedtime", stats.obsolete_averageBedtime.GetValueOrDefault());
          stats.obsolete_averageBedtime = new uint?();
          stats.obsolete_beveragesMade = MergeStats("beveragesMade", stats.obsolete_beveragesMade);
          stats.obsolete_caveCarrotsFound = MergeStats("caveCarrotsFound", stats.obsolete_caveCarrotsFound);
          stats.obsolete_cheeseMade = MergeStats("cheeseMade", stats.obsolete_cheeseMade);
          stats.obsolete_chickenEggsLayed = MergeStats("chickenEggsLayed", stats.obsolete_chickenEggsLayed);
          stats.obsolete_copperFound = MergeStats("copperFound", stats.obsolete_copperFound);
          stats.obsolete_cowMilkProduced = MergeStats("cowMilkProduced", stats.obsolete_cowMilkProduced);
          stats.obsolete_cropsShipped = MergeStats("cropsShipped", stats.obsolete_cropsShipped);
          stats.obsolete_daysPlayed = MergeStats("daysPlayed", stats.obsolete_daysPlayed);
          stats.obsolete_diamondsFound = MergeStats("diamondsFound", stats.obsolete_diamondsFound);
          stats.obsolete_dirtHoed = MergeStats("dirtHoed", stats.obsolete_dirtHoed);
          stats.obsolete_duckEggsLayed = MergeStats("duckEggsLayed", stats.obsolete_duckEggsLayed);
          stats.obsolete_fishCaught = MergeStats("fishCaught", stats.obsolete_fishCaught);
          stats.obsolete_geodesCracked = MergeStats("geodesCracked", stats.obsolete_geodesCracked);
          stats.obsolete_giftsGiven = MergeStats("giftsGiven", stats.obsolete_giftsGiven);
          stats.obsolete_goatCheeseMade = MergeStats("goatCheeseMade", stats.obsolete_goatCheeseMade);
          stats.obsolete_goatMilkProduced = MergeStats("goatMilkProduced", stats.obsolete_goatMilkProduced);
          stats.obsolete_goldFound = MergeStats("goldFound", stats.obsolete_goldFound);
          stats.obsolete_goodFriends = MergeStats("goodFriends", stats.obsolete_goodFriends);
          stats.obsolete_individualMoneyEarned = MergeStats("individualMoneyEarned", stats.obsolete_individualMoneyEarned);
          stats.obsolete_iridiumFound = MergeStats("iridiumFound", stats.obsolete_iridiumFound);
          stats.obsolete_ironFound = MergeStats("ironFound", stats.obsolete_ironFound);
          stats.obsolete_itemsCooked = MergeStats("itemsCooked", stats.obsolete_itemsCooked);
          stats.obsolete_itemsCrafted = MergeStats("itemsCrafted", stats.obsolete_itemsCrafted);
          stats.obsolete_itemsForaged = MergeStats("itemsForaged", stats.obsolete_itemsForaged);
          stats.obsolete_itemsShipped = MergeStats("itemsShipped", stats.obsolete_itemsShipped);
          stats.obsolete_monstersKilled = MergeStats("monstersKilled", stats.obsolete_monstersKilled);
          stats.obsolete_mysticStonesCrushed = MergeStats("mysticStonesCrushed", stats.obsolete_mysticStonesCrushed);
          stats.obsolete_notesFound = MergeStats("notesFound", stats.obsolete_notesFound);
          stats.obsolete_otherPreciousGemsFound = MergeStats("otherPreciousGemsFound", stats.obsolete_otherPreciousGemsFound);
          stats.obsolete_piecesOfTrashRecycled = MergeStats("piecesOfTrashRecycled", stats.obsolete_piecesOfTrashRecycled);
          stats.obsolete_preservesMade = MergeStats("preservesMade", stats.obsolete_preservesMade);
          stats.obsolete_prismaticShardsFound = MergeStats("prismaticShardsFound", stats.obsolete_prismaticShardsFound);
          stats.obsolete_questsCompleted = MergeStats("questsCompleted", stats.obsolete_questsCompleted);
          stats.obsolete_rabbitWoolProduced = MergeStats("rabbitWoolProduced", stats.obsolete_rabbitWoolProduced);
          stats.obsolete_rocksCrushed = MergeStats("rocksCrushed", stats.obsolete_rocksCrushed);
          stats.obsolete_sheepWoolProduced = MergeStats("sheepWoolProduced", stats.obsolete_sheepWoolProduced);
          stats.obsolete_slimesKilled = MergeStats("slimesKilled", stats.obsolete_slimesKilled);
          stats.obsolete_stepsTaken = MergeStats("stepsTaken", stats.obsolete_stepsTaken);
          stats.obsolete_stoneGathered = MergeStats("stoneGathered", stats.obsolete_stoneGathered);
          stats.obsolete_stumpsChopped = MergeStats("stumpsChopped", stats.obsolete_stumpsChopped);
          stats.obsolete_timesFished = MergeStats("timesFished", stats.obsolete_timesFished);
          stats.obsolete_timesUnconscious = MergeStats("timesUnconscious", stats.obsolete_timesUnconscious);
          stats.obsolete_totalMoneyGifted = MergeStats("totalMoneyGifted", stats.obsolete_totalMoneyGifted);
          stats.obsolete_trufflesFound = MergeStats("trufflesFound", stats.obsolete_trufflesFound);
          stats.obsolete_weedsEliminated = MergeStats("weedsEliminated", stats.obsolete_weedsEliminated);
          stats.obsolete_seedsSown = MergeStats("seedsSown", stats.obsolete_seedsSown);

          uint? MergeStats(string newKey, uint? oldValue)
          {
            int num = (int) stats.Increment(newKey, oldValue.GetValueOrDefault());
            return new uint?();
          }
        }
        return true;
      case SaveFixes.MakeWildSeedsDeterministic:
        Utility.ForEachCrop((Func<Crop, bool>) (crop =>
        {
          if (crop.isWildSeedCrop())
            crop.replaceWithObjectOnFullGrown.Value = crop.getRandomWildCropForSeason(true);
          return true;
        }));
        return true;
      case SaveFixes.FixTranslatedInternalNames:
        Utility.ForEachItem((Func<Item, bool>) (item =>
        {
          string qualifiedItemId = item.QualifiedItemId;
          if (qualifiedItemId != null)
          {
            switch (qualifiedItemId.Length)
            {
              case 5:
                switch (qualifiedItemId[3])
                {
                  case '1':
                    if (qualifiedItemId == "(H)15" || qualifiedItemId == "(H)17" || qualifiedItemId == "(H)18")
                      break;
                    goto label_18;
                  case '2':
                    if (qualifiedItemId == "(H)23" || qualifiedItemId == "(H)28")
                      break;
                    goto label_18;
                  case '3':
                    if (qualifiedItemId == "(H)35")
                      break;
                    goto label_18;
                  case '4':
                    if (qualifiedItemId == "(H)41")
                      break;
                    goto label_18;
                  case '5':
                    if (qualifiedItemId == "(H)50" || qualifiedItemId == "(H)51")
                      break;
                    goto label_18;
                  case '8':
                    if (qualifiedItemId == "(H)82")
                      break;
                    goto label_18;
                  case '9':
                    if (qualifiedItemId == "(H)90")
                      break;
                    goto label_18;
                  default:
                    goto label_18;
                }
                break;
              case 6:
                if (qualifiedItemId == "(O)804")
                  break;
                goto label_18;
              case 10:
                if (qualifiedItemId == "(H)GilsHat")
                  break;
                goto label_18;
              case 13:
                if (qualifiedItemId == "(H)GoldPanHat" && item.Name == "Steel Pan")
                {
                  item.Name = ItemRegistry.GetData(item.QualifiedItemId)?.InternalName ?? item.Name;
                  goto label_18;
                }
                goto label_18;
              case 14:
                if (qualifiedItemId == "(H)AbigailsBow")
                  break;
                goto label_18;
              case 15:
                if (qualifiedItemId == "(H)GovernorsHat")
                  break;
                goto label_18;
              default:
                goto label_18;
            }
            if (item.Name.Contains('’'))
              item.Name = ItemRegistry.GetData(item.QualifiedItemId)?.InternalName ?? item.Name;
          }
label_18:
          return true;
        }));
        return true;
      case SaveFixes.ConvertBuildingQuests:
        foreach (Farmer allFarmer in Game1.getAllFarmers())
        {
          for (int index = 0; index < allFarmer.questLog.Count; ++index)
          {
            Quest quest = allFarmer.questLog[index];
            if (quest.questType.Value == 8)
              allFarmer.questLog[index] = (Quest) new HaveBuildingQuest(quest.obsolete_completionString);
          }
        }
        return true;
      case SaveFixes.AddJunimoKartAndPrairieKingStats:
        foreach (Farmer allFarmer in Game1.getAllFarmers())
        {
          if (allFarmer.hasOrWillReceiveMail("JunimoKart"))
          {
            int num17 = (int) allFarmer.stats.Increment("completedJunimoKart", 1);
          }
          if (allFarmer.hasOrWillReceiveMail("Beat_PK"))
          {
            int num18 = (int) allFarmer.stats.Increment("completedPrairieKing", 1);
          }
        }
        return true;
      case SaveFixes.FixEmptyLostAndFoundItemStacks:
        foreach (Item returnedDonation in Game1.player.team.returnedDonations)
        {
          if (returnedDonation != null && returnedDonation.Stack < 1)
            returnedDonation.Stack = 1;
        }
        return true;
      case SaveFixes.FixDuplicateMissedMail:
        HashSet<string> stringSet = new HashSet<string>();
        List<int> intList = new List<int>();
        foreach (Farmer allFarmer in Game1.getAllFarmers())
        {
          stringSet.Clear();
          intList.Clear();
          for (int index = 0; index < allFarmer.mailbox.Count; ++index)
          {
            string str5 = allFarmer.mailbox[index];
            if (!stringSet.Add(str5) && (str5 == "robinKitchenLetter" || str5 == "marnieAutoGrabber" || str5 == "JunimoKart" || str5 == "Beat_PK"))
              intList.Add(index);
          }
          intList.Reverse();
          foreach (int index in intList)
            allFarmer.mailbox.RemoveAt(index);
        }
        return true;
      default:
        return false;
    }
  }

  /// <summary>Convert individually implemented buildings that were saved before Stardew Valley 1.6 to the new Data/BuildingsData format.</summary>
  /// <param name="location">The location whose buildings to convert.</param>
  public static void ConvertBuildingsToData(GameLocation location)
  {
    for (int index = location.buildings.Count - 1; index >= 0; --index)
    {
      Building building = location.buildings[index];
      GameLocation indoors = building.GetIndoors();
      if (indoors != null)
        SaveMigrator_1_6.ConvertBuildingsToData(indoors);
      string str = building.buildingType.Value;
      if (str == "Log Cabin" || str == "Plank Cabin" || str == "Stone Cabin")
      {
        building.skinId.Value = building.buildingType.Value;
        building.buildingType.Value = "Cabin";
        building.ReloadBuildingData();
        building.updateInteriorWarps();
      }
      string buildingType = building.GetData()?.BuildingType;
      if (buildingType != null && buildingType != building.GetType().FullName)
      {
        Building instanceFromId = Building.CreateInstanceFromId(building.buildingType.Value, new Vector2((float) building.tileX.Value, (float) building.tileY.Value));
        if (instanceFromId != null)
        {
          instanceFromId.indoors.Value = building.indoors.Value;
          instanceFromId.buildingType.Value = building.buildingType.Value;
          instanceFromId.tileX.Value = building.tileX.Value;
          instanceFromId.tileY.Value = building.tileY.Value;
          location.buildings.RemoveAt(index);
          location.buildings.Add(instanceFromId);
          SaveMigrator_1_6.TransferValuesToDataBuilding(building, instanceFromId);
        }
      }
    }
  }

  /// <summary>Copy values from an older pre-1.6 building to a new data-driven <see cref="T:StardewValley.Buildings.Building" /> instance.</summary>
  /// <param name="oldBuilding">The pre-1.6 building instance.</param>
  /// <param name="newBuilding">The new data-driven building instance that will replace <paramref name="oldBuilding" />.</param>
  public static void TransferValuesToDataBuilding(Building oldBuilding, Building newBuilding)
  {
    newBuilding.animalDoorOpen.Value = oldBuilding.animalDoorOpen.Value;
    newBuilding.animalDoorOpenAmount.Value = oldBuilding.animalDoorOpenAmount.Value;
    newBuilding.netBuildingPaintColor.Value.CopyFrom(oldBuilding.netBuildingPaintColor.Value);
    newBuilding.modData.CopyFrom((IEnumerable<KeyValuePair<string, string>>) oldBuilding.modData.Pairs);
    if (!(oldBuilding is Mill mill))
      return;
    mill.TransferValuesToNewBuilding(newBuilding);
  }

  /// <summary>Migrate all farmhands from Cabin.deprecatedFarmhand into NetWorldState.</summary>
  /// <param name="locations">The locations to scan for cabins.</param>
  public static void MigrateFarmhands(List<GameLocation> locations)
  {
    foreach (GameLocation location in locations)
    {
      foreach (Building building in location.buildings)
      {
        if (building.GetIndoors() is Cabin indoors)
        {
          Farmer obsoleteFarmhand = indoors.obsolete_farmhand;
          indoors.obsolete_farmhand = (Farmer) null;
          Game1.netWorldState.Value.farmhandData[obsoleteFarmhand.UniqueMultiplayerID] = obsoleteFarmhand;
          indoors.farmhandReference.Value = obsoleteFarmhand;
        }
      }
    }
  }

  /// <summary>Migrate saved bundle data from Stardew Valley 1.5.6 or earlier to the new format.</summary>
  /// <param name="bundleData">The raw bundle data to standardize.</param>
  public static void StandardizeBundleFields(Dictionary<string, string> bundleData)
  {
    foreach (string key in bundleData.Keys.ToArray<string>())
    {
      string[] array = bundleData[key].Split('/');
      if (array.Length < 7)
      {
        Array.Resize<string>(ref array, 7);
        array[6] = array[0];
        bundleData[key] = string.Join("/", array);
      }
    }
  }

  /// <summary>For a building with an upgrade started before 1.6, get the building type it should be upgraded to if possible.</summary>
  /// <param name="fromBuildingType">The building type before the upgrade finishes.</param>
  public static string InferBuildingUpgradingTo(string fromBuildingType)
  {
    switch (fromBuildingType)
    {
      case "Coop":
        return "Big Coop";
      case "Big Coop":
        return "Deluxe Coop";
      case "Barn":
        return "Big Barn";
      case "Big Barn":
        return "Deluxe Barn";
      case "Shed":
        return "Big Shed";
      default:
        foreach (KeyValuePair<string, BuildingData> keyValuePair in (IEnumerable<KeyValuePair<string, BuildingData>>) Game1.buildingData)
        {
          if (keyValuePair.Value.BuildingToUpgrade == fromBuildingType)
            return keyValuePair.Key;
        }
        return (string) null;
    }
  }

  /// <summary>For a machine which contains output produced before 1.6, set the <see cref="F:StardewValley.Object.lastInputItem" /> and <see cref="F:StardewValley.Object.lastOutputRuleId" /> values when possible. This ensures that some machine logic works as expected (e.g. crystalariums resuming on collect).</summary>
  /// <param name="machine">The machine which produced output.</param>
  /// <remarks>This is heuristic, and some fields may not be set if it's not possible to retroactively infer them.</remarks>
  public static void InferMachineInputOutputFields(StardewValley.Object machine)
  {
    StardewValley.Object @object = machine.heldObject.Value;
    string qualifiedItemId1 = @object?.QualifiedItemId;
    if (qualifiedItemId1 == null)
      return;
    NetRef<Item> lastInputItem = machine.lastInputItem;
    NetString lastOutputRuleId = machine.lastOutputRuleId;
    string qualifiedItemId2 = machine.QualifiedItemId;
    if (qualifiedItemId2 == null)
      return;
    switch (qualifiedItemId2.Length)
    {
      case 5:
        if (!(qualifiedItemId2 == "(BC)9"))
          return;
        break;
      case 6:
        switch (qualifiedItemId2[5])
        {
          case '0':
            switch (qualifiedItemId2)
            {
              case "(BC)90":
                if (!(qualifiedItemId1 == "(O)466") && !(qualifiedItemId1 == "(O)465") && !(qualifiedItemId1 == "(O)369") && !(qualifiedItemId1 == "(O)805"))
                  return;
                lastOutputRuleId.Value = "Default";
                return;
              case "(BC)20":
                if (qualifiedItemId1 == null)
                  return;
                switch (qualifiedItemId1.Length)
                {
                  case 5:
                    if (!(qualifiedItemId1 == "(O)93"))
                      return;
                    break;
                  case 6:
                    switch (qualifiedItemId1[4])
                    {
                      case '2':
                        if (!(qualifiedItemId1 == "(O)428"))
                          return;
                        goto label_124;
                      case '3':
                        int num1 = qualifiedItemId1 == "(O)338" ? 1 : 0;
                        return;
                      case '8':
                        switch (qualifiedItemId1)
                        {
                          case "(O)382":
                          case "(O)380":
                            break;
                          case "(O)388":
                            lastOutputRuleId.Value = "Default_Driftwood";
                            lastInputItem.Value = ItemRegistry.Create("(O)169");
                            return;
                          default:
                            return;
                        }
                        break;
                      case '9':
                        if (!(qualifiedItemId1 == "(O)390"))
                          return;
                        break;
                      default:
                        return;
                    }
                    lastOutputRuleId.Value = "Default_Trash";
                    lastInputItem.Value = ItemRegistry.Create("(O)168");
                    return;
                  default:
                    return;
                }
label_124:
                lastOutputRuleId.Value = "Default_SoggyNewspaper";
                lastInputItem.Value = ItemRegistry.Create("(O)172");
                return;
              case "(BC)10":
                break;
              default:
                return;
            }
            break;
          case '1':
            if (!(qualifiedItemId2 == "(BC)21"))
              return;
            lastOutputRuleId.Value = "Default";
            lastInputItem.Value = @object.getOne();
            return;
          case '2':
            if (!(qualifiedItemId2 == "(BC)12"))
              return;
            switch (qualifiedItemId1)
            {
              case "(O)346":
                lastOutputRuleId.Value = "Default_Wheat";
                lastInputItem.Value = ItemRegistry.Create("(O)262");
                return;
              case "(O)303":
                lastOutputRuleId.Value = "Default_Hops";
                lastInputItem.Value = ItemRegistry.Create("(O)304");
                return;
              case "(O)614":
                lastOutputRuleId.Value = "Default_TeaLeaves";
                lastInputItem.Value = ItemRegistry.Create("(O)815");
                return;
              case "(O)395":
                lastOutputRuleId.Value = "Default_CoffeeBeans";
                lastInputItem.Value = ItemRegistry.Create("(O)433", 5);
                return;
              case "(O)340":
                lastOutputRuleId.Value = "Default_Honey";
                lastInputItem.Value = ItemRegistry.Create("(O)459", 5);
                return;
              default:
                StardewValley.Object.PreserveType? nullable = @object.preserve.Value;
                if (!nullable.HasValue)
                  return;
                switch (nullable.GetValueOrDefault())
                {
                  case StardewValley.Object.PreserveType.Wine:
                    lastOutputRuleId.Value = "Default_Wine";
                    lastInputItem.Value = ItemRegistry.Create(@object.preservedParentSheetIndex.Value, allowNull: true);
                    return;
                  case StardewValley.Object.PreserveType.Juice:
                    lastOutputRuleId.Value = "Default_Juice";
                    lastInputItem.Value = ItemRegistry.Create(@object.preservedParentSheetIndex.Value, allowNull: true);
                    return;
                  default:
                    return;
                }
            }
          case '3':
            if (!(qualifiedItemId2 == "(BC)13") || qualifiedItemId1 == null || qualifiedItemId1.Length != 6)
              return;
            switch (qualifiedItemId1[5])
            {
              case '0':
                if (!(qualifiedItemId1 == "(O)910"))
                  return;
                lastOutputRuleId.Value = "Default_RadioactiveOre";
                lastInputItem.Value = ItemRegistry.Create("(O)909", 5);
                return;
              case '1':
                return;
              case '2':
                return;
              case '3':
                return;
              case '4':
                if (!(qualifiedItemId1 == "(O)334"))
                  return;
                lastOutputRuleId.Value = "Default_CopperOre";
                lastInputItem.Value = ItemRegistry.Create("(O)378", 5);
                return;
              case '5':
                if (!(qualifiedItemId1 == "(O)335"))
                  return;
                lastOutputRuleId.Value = "Default_IronOre";
                lastInputItem.Value = ItemRegistry.Create("(O)380", 5);
                return;
              case '6':
                if (!(qualifiedItemId1 == "(O)336"))
                  return;
                lastOutputRuleId.Value = "Default_GoldOre";
                lastInputItem.Value = ItemRegistry.Create("(O)384", 5);
                return;
              case '7':
                switch (qualifiedItemId1)
                {
                  case "(O)337":
                    lastOutputRuleId.Value = "Default_IridiumOre";
                    lastInputItem.Value = ItemRegistry.Create("(O)386", 5);
                    return;
                  case "(O)277":
                    lastOutputRuleId.Value = "Default_Bouquet";
                    lastInputItem.Value = ItemRegistry.Create("(O)458");
                    return;
                  default:
                    return;
                }
              case '8':
                if (!(qualifiedItemId1 == "(O)338"))
                  return;
                if (@object.Stack > 1)
                {
                  lastOutputRuleId.Value = "Default_FireQuartz";
                  lastInputItem.Value = ItemRegistry.Create("(O)82");
                  return;
                }
                lastOutputRuleId.Value = "Default_Quartz";
                lastInputItem.Value = ItemRegistry.Create("(O)80");
                return;
              default:
                return;
            }
          case '4':
            if (!(qualifiedItemId2 == "(BC)24"))
              return;
            switch (qualifiedItemId1)
            {
              case "(O)306":
                switch (@object.Stack)
                {
                  case 3:
                    lastOutputRuleId.Value = "Default_GoldenEgg";
                    lastInputItem.Value = ItemRegistry.Create("(O)928");
                    return;
                  case 10:
                    lastOutputRuleId.Value = "Default_OstrichEgg";
                    lastInputItem.Value = ItemRegistry.Create("(O)289", quality: @object.Quality);
                    return;
                  default:
                    if (@object.Quality == 2)
                    {
                      lastOutputRuleId.Value = "Default_LargeEgg";
                      lastInputItem.Value = ItemRegistry.Create("(O)174");
                      return;
                    }
                    lastOutputRuleId.Value = "Default_Egg";
                    lastInputItem.Value = ItemRegistry.Create("(O)176");
                    return;
                }
              case "(O)307":
                lastOutputRuleId.Value = "Default_DuckEgg";
                lastInputItem.Value = ItemRegistry.Create("(O)442");
                return;
              case "(O)308":
                lastOutputRuleId.Value = "Default_VoidEgg";
                lastInputItem.Value = ItemRegistry.Create("(O)305");
                return;
              case "(O)807":
                lastOutputRuleId.Value = "Default_DinosaurEgg";
                lastInputItem.Value = ItemRegistry.Create("(O)107");
                return;
              default:
                return;
            }
          case '5':
            switch (qualifiedItemId2)
            {
              case "(BC)15":
                switch (qualifiedItemId1)
                {
                  case "(O)445":
                    lastOutputRuleId.Value = "Default_SturgeonRoe";
                    lastInputItem.Value = (Item) ItemRegistry.GetObjectTypeDefinition().CreateFlavoredRoe(ItemRegistry.Create<StardewValley.Object>("(O)698"));
                    return;
                  case "(O)447":
                    lastOutputRuleId.Value = "Default_Roe";
                    lastInputItem.Value = (Item) ItemRegistry.GetObjectTypeDefinition().CreateFlavoredRoe(ItemRegistry.Create<StardewValley.Object>(@object.preservedParentSheetIndex.Value));
                    return;
                  case "(O)342":
                    lastOutputRuleId.Value = "Default_Pickled";
                    lastInputItem.Value = ItemRegistry.Create(@object.preservedParentSheetIndex.Value, allowNull: true);
                    return;
                  case "(O)344":
                    lastOutputRuleId.Value = "Default_Jelly";
                    lastInputItem.Value = ItemRegistry.Create(@object.preservedParentSheetIndex.Value, allowNull: true);
                    return;
                  default:
                    return;
                }
              case "(BC)25":
                lastOutputRuleId.Value = "Default";
                CropData cropData;
                if (!(qualifiedItemId1 != "(O)499") || !@object.HasTypeObject() || !Game1.cropData.TryGetValue(@object.ItemId, out cropData) || cropData.HarvestItemId == null)
                  return;
                lastInputItem.Value = ItemRegistry.Create(cropData.HarvestItemId, allowNull: true);
                return;
              default:
                return;
            }
          case '6':
            if (!(qualifiedItemId2 == "(BC)16"))
              return;
            switch (qualifiedItemId1)
            {
              case "(O)426":
                if (@object.Quality == 0)
                {
                  lastOutputRuleId.Value = "Default_GoatMilk";
                  lastInputItem.Value = ItemRegistry.Create("(O)436");
                  return;
                }
                lastOutputRuleId.Value = "Default_LargeGoatMilk";
                lastInputItem.Value = ItemRegistry.Create("(O)438");
                return;
              case "(O)424":
                if (@object.Quality == 0)
                {
                  lastOutputRuleId.Value = "Default_Milk";
                  lastInputItem.Value = ItemRegistry.Create("(O)184");
                  return;
                }
                lastOutputRuleId.Value = "Default_LargeMilk";
                lastInputItem.Value = ItemRegistry.Create("(O)186");
                return;
              default:
                return;
            }
          case '7':
            if (!(qualifiedItemId2 == "(BC)17") || !(qualifiedItemId1 == "(O)428"))
              return;
            lastOutputRuleId.Value = "Default";
            lastInputItem.Value = ItemRegistry.Create("(O)440");
            return;
          case '8':
            return;
          case '9':
            if (!(qualifiedItemId2 == "(BC)19"))
              return;
            switch (qualifiedItemId1)
            {
              case "(O)247":
                return;
              case "(O)432":
                lastOutputRuleId.Value = "Default_Truffle";
                lastInputItem.Value = ItemRegistry.Create("(O)430");
                return;
              default:
                return;
            }
          default:
            return;
        }
      case 7:
        switch (qualifiedItemId2[5])
        {
          case '0':
            if (!(qualifiedItemId2 == "(BC)101"))
            {
              int num2 = qualifiedItemId2 == "(BC)105" ? 1 : 0;
              return;
            }
            break;
          case '1':
            switch (qualifiedItemId2)
            {
              case "(BC)114":
                if (!(qualifiedItemId1 == "(O)382"))
                  return;
                lastOutputRuleId.Value = "Default";
                lastInputItem.Value = ItemRegistry.Create("(O)388", 10);
                return;
              case "(BC)211":
                return;
              case "(BC)117":
                goto label_144;
              default:
                return;
            }
          case '2':
            if (!(qualifiedItemId2 == "(BC)127") && !(qualifiedItemId2 == "(BC)128"))
              return;
            goto label_144;
          case '3':
            if (!(qualifiedItemId2 == "(BC)231"))
              return;
            goto label_144;
          case '4':
            if (!(qualifiedItemId2 == "(BC)246"))
              return;
            goto label_144;
          case '5':
            switch (qualifiedItemId2)
            {
              case "(BC)254":
              case "(BC)156":
                break;
              case "(BC)158":
                lastOutputRuleId.Value = "Default";
                lastInputItem.Value = ItemRegistry.Create("(O)766", 100);
                return;
              case "(BC)154":
                goto label_144;
              default:
                return;
            }
            break;
          case '6':
            switch (qualifiedItemId2)
            {
              case "(BC)163":
                switch (qualifiedItemId1)
                {
                  case "(O)424":
                    lastOutputRuleId.Value = "Cheese";
                    break;
                  case "(O)426":
                    lastOutputRuleId.Value = "GoatCheese";
                    break;
                  case "(O)348":
                    lastOutputRuleId.Value = "Wine";
                    break;
                  case "(O)459":
                    lastOutputRuleId.Value = "Mead";
                    break;
                  case "(O)303":
                    lastOutputRuleId.Value = "PaleAle";
                    break;
                  case "(O)346":
                    lastOutputRuleId.Value = "Beer";
                    break;
                }
                if (lastOutputRuleId.Value == null)
                  return;
                lastInputItem.Value = @object.getOne();
                lastInputItem.Value.Quality = 0;
                return;
              case "(BC)265":
                lastOutputRuleId.Value = "Default";
                return;
              case "(BC)160":
                goto label_144;
              default:
                int num3 = qualifiedItemId2 == "(BC)264" ? 1 : 0;
                return;
            }
          case '7':
            return;
          case '8':
            switch (qualifiedItemId2)
            {
              case "(BC)182":
                lastOutputRuleId.Value = "Default";
                return;
              case "(BC)280":
                goto label_144;
              default:
                return;
            }
          default:
            return;
        }
        lastOutputRuleId.Value = "Default";
        lastInputItem.Value = @object.getOne();
        return;
      default:
        return;
    }
label_144:
    lastOutputRuleId.Value = "Default";
  }

  /// <summary>Migrate a pre-1.6 quest to the new format.</summary>
  /// <param name="serializer">The XML serializer with which to serialize/deserialize <see cref="T:StardewValley.Quests.DescriptionElement" /> and <see cref="T:StardewValley.SaveMigrations.SaveMigrator_1_6.LegacyDescriptionElement" /> values.</param>
  /// <param name="element">The description element to migrate.</param>
  /// <remarks>
  ///   This updates quest data for two changes in 1.6:
  /// 
  ///   <list type="bullet">
  ///     <item><description>
  ///       The way <see cref="F:StardewValley.Quests.DescriptionElement.substitutions" /> values are stored in the save XML changed from this:
  /// 
  ///       <code>
  ///         &lt;objective&gt;
  ///           &lt;xmlKey&gt;Strings\StringsFromCSFiles:SocializeQuest.cs.13802&lt;/xmlKey&gt;
  ///           &lt;param&gt;
  ///             &lt;anyType xsi:type="xsd:int"&gt;4&lt;/anyType&gt;
  ///             &lt;anyType xsi:type="xsd:int"&gt;28&lt;/anyType&gt;
  ///           &lt;/param&gt;
  ///         &lt;/objective&gt;
  ///       </code>
  /// 
  ///      To this:
  /// 
  ///       <code>
  ///         &lt;objective&gt;
  ///           &lt;xmlKey&gt;Strings\StringsFromCSFiles:SocializeQuest.cs.13802&lt;/xmlKey&gt;
  ///           &lt;param xsi:type="xsd:int"&gt;4&lt;/param&gt;
  ///           &lt;param xsi:type="xsd:int"&gt;28&lt;/param&gt;
  ///         &lt;/objective&gt;
  ///       </code>
  /// 
  ///       If the given description element is affected, this method re-deserializes the data into the correct format.
  ///   </description></item>
  /// 
  ///   <item><description>Some translation keys were merged to fix gender issues.</description></item>
  ///   </list>
  /// </remarks>
  public static void MigrateLegacyDescriptionElement(
    Lazy<XmlSerializer> serializer,
    DescriptionElement element)
  {
    if (element == null)
      return;
    List<object> substitutions1 = element.substitutions;
    // ISSUE: explicit non-virtual call
    if ((substitutions1 != null ? (__nonvirtual (substitutions1.Count) == 1 ? 1 : 0) : 0) != 0 && element.substitutions[0] is XmlNode[] substitution1)
    {
      StringBuilder stringBuilder = new StringBuilder("<?xml version=\"1.0\" encoding=\"utf-8\"?><LegacyDescriptionElement xmlns:xsd=\"http://www.w3.org/2001/XMLSchema\" xmlns:xsi=\"http://www.w3.org/2001/XMLSchema-instance\"><param>");
      for (int index = 0; index < substitution1.Length; ++index)
      {
        XmlNode xmlNode = substitution1[index];
        stringBuilder.Append(xmlNode.OuterXml);
      }
      stringBuilder.Append("</param></LegacyDescriptionElement>");
      SaveMigrator_1_6.LegacyDescriptionElement descriptionElement;
      using (StringReader input = new StringReader(stringBuilder.ToString()))
      {
        using (XmlReader xmlReader = (XmlReader) new XmlTextReader((TextReader) input))
          descriptionElement = (SaveMigrator_1_6.LegacyDescriptionElement) serializer.Value.Deserialize(xmlReader);
      }
      if (descriptionElement != null)
        element.substitutions = descriptionElement.param;
    }
    switch (element.translationKey)
    {
      case "Strings\\StringsFromCSFiles:FishingQuest.cs.13251":
        element.translationKey = "Strings\\StringsFromCSFiles:FishingQuest.cs.13248";
        break;
      case "Strings\\StringsFromCSFiles:ItemDeliveryQuest.cs.13563":
        element.translationKey = "Strings\\StringsFromCSFiles:ItemDeliveryQuest.cs.13560";
        break;
      case "Strings\\StringsFromCSFiles:ItemDeliveryQuest.cs.13574":
        element.translationKey = "Strings\\StringsFromCSFiles:ItemDeliveryQuest.cs.13571";
        break;
    }
    List<object> substitutions2 = element.substitutions;
    // ISSUE: explicit non-virtual call
    if ((substitutions2 != null ? (__nonvirtual (substitutions2.Count) > 0 ? 1 : 0) : 0) == 0)
      return;
    foreach (object substitution2 in element.substitutions)
    {
      if (substitution2 is DescriptionElement element1)
        SaveMigrator_1_6.MigrateLegacyDescriptionElement(serializer, element1);
    }
  }

  /// <summary>The pre-1.6 structure of <see cref="T:StardewValley.Quests.DescriptionElement" />.</summary>
  public class LegacyDescriptionElement
  {
    /// <summary>The translation key for the text to render.</summary>
    public string xmlKey;
    /// <summary>The values to substitute for placeholders like <c>{0}</c> in the translation text.</summary>
    public List<object> param;
  }
}
