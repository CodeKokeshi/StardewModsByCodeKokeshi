// Decompiled with JetBrains decompiler
// Type: StardewValley.SaveMigrations.SaveMigrator_1_4
// Assembly: Stardew Valley, Version=1.6.15.24356, Culture=neutral, PublicKeyToken=null
// MVID: AA1F513A-94F0-4EF6-A35F-2D5B4A3721BF
// Assembly location: D:\SteamLibrary\steamapps\common\Stardew Valley\Stardew Valley.dll
// XML documentation location: D:\SteamLibrary\steamapps\common\Stardew Valley\Stardew Valley.xml

using Microsoft.Xna.Framework;
using Netcode;
using StardewValley.Buildings;
using StardewValley.Extensions;
using StardewValley.ItemTypeDefinitions;
using StardewValley.Locations;
using StardewValley.Objects;
using StardewValley.TerrainFeatures;
using System;
using System.Collections.Generic;
using System.Linq;

#nullable disable
namespace StardewValley.SaveMigrations;

/// <summary>Migrates existing save files for compatibility with Stardew Valley 1.4.</summary>
public class SaveMigrator_1_4 : ISaveMigrator
{
  /// <inheritdoc />
  public Version GameVersion { get; } = new Version(1, 4);

  /// <inheritdoc />
  public bool ApplySaveFix(SaveFixes saveFix)
  {
    switch (saveFix)
    {
      case SaveFixes.StoredBigCraftablesStackFix:
        Utility.ForEachItem((Func<Item, bool>) (item =>
        {
          if (item is StardewValley.Object object2 && object2.bigCraftable.Value && object2.Stack == 0)
            object2.Stack = 1;
          return true;
        }));
        return true;
      case SaveFixes.PorchedCabinBushesFix:
        Utility.ForEachBuilding((Func<Building, bool>) (building =>
        {
          if (building.daysOfConstructionLeft.Value <= 0 && building.GetIndoors() is Cabin)
            building.removeOverlappingBushes((GameLocation) Game1.getFarm());
          return true;
        }));
        return true;
      case SaveFixes.ChangeObeliskFootprintHeight:
        Utility.ForEachBuilding((Func<Building, bool>) (building =>
        {
          if (building.buildingType.Value.Contains("Obelisk"))
          {
            building.tilesHigh.Value = 2;
            ++building.tileY.Value;
          }
          return true;
        }));
        return true;
      case SaveFixes.CreateStorageDressers:
        Utility.ForEachItem((Func<Item, bool>) (item =>
        {
          if (item is Clothing)
            item.Category = -100;
          return true;
        }));
        Utility.ForEachLocation((Func<GameLocation, bool>) (location =>
        {
          if (location is DecoratableLocation)
          {
            List<Furniture> furnitureList = new List<Furniture>();
            for (int index = 0; index < location.furniture.Count; ++index)
            {
              Furniture furniture = location.furniture[index];
              if (furniture.ItemId == "704" || furniture.ItemId == "709" || furniture.ItemId == "714" || furniture.ItemId == "719")
              {
                StorageFurniture storageFurniture = new StorageFurniture(furniture.ItemId, furniture.TileLocation, furniture.currentRotation.Value);
                furnitureList.Add((Furniture) storageFurniture);
                location.furniture.RemoveAt(index);
                --index;
              }
            }
            foreach (Furniture furniture in furnitureList)
              location.furniture.Add(furniture);
          }
          return true;
        }));
        return true;
      case SaveFixes.InferPreserves:
        string[] preserveItemIndices = new string[4]
        {
          "(O)350",
          "(O)348",
          "(O)344",
          "(O)342"
        };
        string[] suffixes = new string[3]
        {
          " Juice",
          " Wine",
          " Jelly"
        };
        StardewValley.Object.PreserveType[] suffixPreserveTypes = new StardewValley.Object.PreserveType[3]
        {
          StardewValley.Object.PreserveType.Juice,
          StardewValley.Object.PreserveType.Wine,
          StardewValley.Object.PreserveType.Jelly
        };
        string[] prefixes = new string[1]{ "Pickled " };
        StardewValley.Object.PreserveType[] prefixPreserveTypes = new StardewValley.Object.PreserveType[1]
        {
          StardewValley.Object.PreserveType.Pickle
        };
        Utility.ForEachItem((Func<Item, bool>) (item =>
        {
          if (!(item is StardewValley.Object object4) || !Utility.IsNormalObjectAtParentSheetIndex((Item) object4, object4.ItemId) || !((IEnumerable<string>) preserveItemIndices).Contains<string>(object4.QualifiedItemId) || object4.preserve.Value.HasValue)
            return true;
          bool flag = false;
          for (int index = 0; index < suffixes.Length; ++index)
          {
            string str1 = suffixes[index];
            if (object4.Name.EndsWith(str1))
            {
              string str2 = object4.Name.Substring(0, object4.Name.Length - str1.Length);
              string str3 = (string) null;
              foreach (ParsedItemData parsedItemData in ItemRegistry.GetObjectTypeDefinition().GetAllData())
              {
                if (parsedItemData.InternalName == str2)
                {
                  str3 = parsedItemData.ItemId;
                  break;
                }
              }
              if (str3 != null)
              {
                object4.preservedParentSheetIndex.Value = str3;
                object4.preserve.Value = new StardewValley.Object.PreserveType?(suffixPreserveTypes[index]);
                flag = true;
                break;
              }
            }
          }
          if (flag)
            return true;
          for (int index = 0; index < prefixes.Length; ++index)
          {
            string str4 = prefixes[index];
            if (object4.Name.StartsWith(str4))
            {
              string str5 = object4.Name.Substring(str4.Length);
              string str6 = (string) null;
              foreach (ParsedItemData parsedItemData in ItemRegistry.GetObjectTypeDefinition().GetAllData())
              {
                if (parsedItemData.InternalName == str5)
                {
                  str6 = parsedItemData.ItemId;
                  break;
                }
              }
              if (str6 != null)
              {
                object4.preservedParentSheetIndex.Value = str6;
                object4.preserve.Value = new StardewValley.Object.PreserveType?(prefixPreserveTypes[index]);
                break;
              }
            }
          }
          return true;
        }));
        return true;
      case SaveFixes.TransferHatSkipHairFlag:
        Utility.ForEachItem((Func<Item, bool>) (item =>
        {
          if (item is Hat hat2 && hat2.skipHairDraw)
          {
            hat2.hairDrawType.Set(0);
            hat2.skipHairDraw = false;
          }
          return true;
        }));
        return true;
      case SaveFixes.RevealSecretNoteItemTastes:
        Dictionary<int, string> dictionary = DataLoader.SecretNotes(Game1.content);
        for (int key = 0; key < 21; ++key)
        {
          string str;
          if (dictionary.TryGetValue(key, out str) && Game1.player.secretNotesSeen.Contains(key))
            Utility.ParseGiftReveals(str);
        }
        return true;
      case SaveFixes.TransferHoneyTypeToPreserves:
        return true;
      case SaveFixes.TransferNoteBlockScale:
        Utility.ForEachItem((Func<Item, bool>) (item =>
        {
          if (item is StardewValley.Object object6 && (object6.QualifiedItemId == "(O)363" || object6.QualifiedItemId == "(O)464"))
            object6.preservedParentSheetIndex.Value = ((int) object6.scale.X).ToString();
          return true;
        }));
        return true;
      case SaveFixes.FixCropHarvestAmountsAndInferSeedIndex:
        return true;
      case SaveFixes.quarryMineBushes:
        GameLocation location1 = Game1.RequireLocation("Mountain");
        location1.largeTerrainFeatures.Add((LargeTerrainFeature) new Bush(new Vector2(101f, 18f), 1, location1));
        location1.largeTerrainFeatures.Add((LargeTerrainFeature) new Bush(new Vector2(104f, 21f), 0, location1));
        location1.largeTerrainFeatures.Add((LargeTerrainFeature) new Bush(new Vector2(105f, 18f), 0, location1));
        return true;
      case SaveFixes.MissingQisChallenge:
        foreach (Farmer allFarmer in Game1.getAllFarmers())
        {
          if (allFarmer.mailReceived.Contains("skullCave") && !allFarmer.hasQuest("20") && !allFarmer.hasOrWillReceiveMail("QiChallengeComplete"))
            allFarmer.addQuest("20");
        }
        return true;
      case SaveFixes.AddTownBush:
        if (Game1.getLocationFromName("Town") is Town locationFromName)
        {
          Vector2 tileLocation = new Vector2(61f, 93f);
          if (locationFromName.getLargeTerrainFeatureAt((int) tileLocation.X, (int) tileLocation.Y) == null)
            locationFromName.largeTerrainFeatures.Add((LargeTerrainFeature) new Bush(tileLocation, 2, (GameLocation) locationFromName));
        }
        return true;
      default:
        return false;
    }
  }

  /// <summary>Apply one-time save migrations which predate <see cref="T:StardewValley.SaveMigrations.SaveFixes" />.</summary>
  public static void ApplyLegacyChanges()
  {
    foreach (Farmer allFarmer in Game1.getAllFarmers())
    {
      foreach (string key in allFarmer.friendshipData.Keys)
        allFarmer.friendshipData[key].Points = Math.Min(allFarmer.friendshipData[key].Points, 3125);
    }
    foreach (KeyValuePair<string, string> keyValuePair in Game1.netWorldState.Value.BundleData)
    {
      int int32 = Convert.ToInt32(keyValuePair.Key.Split('/')[1]);
      if (!Game1.netWorldState.Value.Bundles.ContainsKey(int32))
        Game1.netWorldState.Value.Bundles.Add(int32, new NetArray<bool, NetBool>(ArgUtility.SplitBySpace(keyValuePair.Value.Split('/')[2]).Length));
      if (!Game1.netWorldState.Value.BundleRewards.ContainsKey(int32))
        Game1.netWorldState.Value.BundleRewards.Add(int32, new NetBool(false));
    }
    foreach (Farmer allFarmer in Game1.getAllFarmers())
    {
      foreach (Item obj in allFarmer.Items)
      {
        if (obj != null)
          obj.HasBeenInInventory = true;
      }
    }
    SaveMigrator_1_4.RecalculateLostBookCount();
    Utility.iterateChestsAndStorage((Action<Item>) (item => item.HasBeenInInventory = true));
    Game1.hasApplied1_4_UpdateChanges = true;
  }

  /// <summary>Recalculate the number of lost books found.</summary>
  public static void RecalculateLostBookCount()
  {
    int val1 = 0;
    foreach (Farmer allFarmer in Game1.getAllFarmers())
    {
      int[] numArray;
      if (allFarmer.archaeologyFound.TryGetValue("102", out numArray) && numArray[0] > 0)
      {
        val1 = Math.Max(val1, numArray[0]);
        allFarmer.mailForTomorrow.Add("lostBookFound%&NL&%");
      }
    }
    Game1.netWorldState.Value.LostBooksFound = val1;
  }
}
