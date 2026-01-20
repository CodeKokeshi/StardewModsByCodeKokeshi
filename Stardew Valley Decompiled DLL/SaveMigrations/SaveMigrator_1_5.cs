// Decompiled with JetBrains decompiler
// Type: StardewValley.SaveMigrations.SaveMigrator_1_5
// Assembly: Stardew Valley, Version=1.6.15.24356, Culture=neutral, PublicKeyToken=null
// MVID: AA1F513A-94F0-4EF6-A35F-2D5B4A3721BF
// Assembly location: D:\SteamLibrary\steamapps\common\Stardew Valley\Stardew Valley.dll
// XML documentation location: D:\SteamLibrary\steamapps\common\Stardew Valley\Stardew Valley.xml

using Microsoft.Xna.Framework;
using Netcode;
using StardewValley.Buildings;
using StardewValley.Enchantments;
using StardewValley.Locations;
using StardewValley.Objects;
using StardewValley.Quests;
using StardewValley.TerrainFeatures;
using StardewValley.Tools;
using System;

#nullable disable
namespace StardewValley.SaveMigrations;

/// <summary>Migrates existing save files for compatibility with Stardew Valley 1.5.</summary>
public class SaveMigrator_1_5 : ISaveMigrator
{
  /// <inheritdoc />
  public Version GameVersion { get; } = new Version(1, 5);

  /// <inheritdoc />
  public bool ApplySaveFix(SaveFixes saveFix)
  {
    switch (saveFix)
    {
      case SaveFixes.BedsToFurniture:
        Utility.ForEachLocation((Func<GameLocation, bool>) (location =>
        {
          if (location is FarmHouse farmHouse2)
          {
            bool hasOwner = farmHouse2.HasOwner;
            for (int index1 = 0; index1 < farmHouse2.map.Layers[0].LayerWidth; ++index1)
            {
              for (int index2 = 0; index2 < farmHouse2.map.Layers[0].LayerHeight; ++index2)
              {
                if (farmHouse2.doesTileHaveProperty(index1, index2, "DefaultBedPosition", "Back") != null)
                {
                  if (farmHouse2.upgradeLevel == 0)
                  {
                    farmHouse2.furniture.Add((Furniture) new BedFurniture(BedFurniture.DEFAULT_BED_INDEX, new Vector2((float) index1, (float) index2)));
                  }
                  else
                  {
                    string itemId = BedFurniture.DOUBLE_BED_INDEX;
                    if (hasOwner && !farmHouse2.owner.activeDialogueEvents.ContainsKey("pennyRedecorating"))
                    {
                      if (farmHouse2.owner.mailReceived.Contains("pennyQuilt0"))
                        itemId = "2058";
                      if (farmHouse2.owner.mailReceived.Contains("pennyQuilt1"))
                        itemId = "2064";
                      if (farmHouse2.owner.mailReceived.Contains("pennyQuilt2"))
                        itemId = "2070";
                    }
                    farmHouse2.furniture.Add((Furniture) new BedFurniture(itemId, new Vector2((float) index1, (float) index2)));
                  }
                }
              }
            }
          }
          return true;
        }));
        return true;
      case SaveFixes.ChildBedsToFurniture:
        Utility.ForEachLocation((Func<GameLocation, bool>) (location =>
        {
          if (location is FarmHouse farmHouse4)
          {
            for (int index3 = 0; index3 < farmHouse4.map.Layers[0].LayerWidth; ++index3)
            {
              for (int index4 = 0; index4 < farmHouse4.map.Layers[0].LayerHeight; ++index4)
              {
                if (farmHouse4.doesTileHaveProperty(index3, index4, "DefaultChildBedPosition", "Back") != null)
                  farmHouse4.furniture.Add((Furniture) new BedFurniture(BedFurniture.CHILD_BED_INDEX, new Vector2((float) index3, (float) index4)));
              }
            }
          }
          return true;
        }));
        return true;
      case SaveFixes.ModularizeFarmStructures:
        Game1.getFarm().AddDefaultBuildings(true);
        return true;
      case SaveFixes.FixFlooringFlags:
        Utility.ForEachLocation((Func<GameLocation, bool>) (location =>
        {
          foreach (TerrainFeature terrainFeature in location.terrainFeatures.Values)
          {
            if (terrainFeature is Flooring flooring2)
              flooring2.ApplyFlooringFlags();
          }
          return true;
        }));
        return true;
      case SaveFixes.FixStableOwnership:
        Utility.ForEachBuilding<Stable>((Func<Stable, bool>) (stable =>
        {
          if (stable.owner.Value == -6666666L && Game1.GetPlayer(-6666666L) == null)
            stable.owner.Value = Game1.player.UniqueMultiplayerID;
          return true;
        }));
        return true;
      case SaveFixes.ResetForges:
        SaveMigrator_1_5.ResetForges();
        return true;
      case SaveFixes.MakeDarkSwordVampiric:
        Utility.ForEachItem((Func<Item, bool>) (item =>
        {
          if (item is MeleeWeapon meleeWeapon2 && meleeWeapon2.QualifiedItemId == "(W)2")
            meleeWeapon2.AddEnchantment((BaseEnchantment) new VampiricEnchantment());
          return true;
        }));
        return true;
      case SaveFixes.FixBeachFarmBushes:
        if (Game1.whichFarm == 6)
        {
          Farm farm = Game1.getFarm();
          Vector2[] vector2Array = new Vector2[4]
          {
            new Vector2(77f, 4f),
            new Vector2(78f, 3f),
            new Vector2(83f, 4f),
            new Vector2(83f, 3f)
          };
          foreach (Vector2 vector2 in vector2Array)
          {
            foreach (LargeTerrainFeature largeTerrainFeature in farm.largeTerrainFeatures)
            {
              if (largeTerrainFeature.Tile == vector2)
              {
                if (largeTerrainFeature is Bush bush)
                {
                  bush.Tile = new Vector2(bush.Tile.X, bush.Tile.Y + 1f);
                  break;
                }
                break;
              }
            }
          }
        }
        return true;
      case SaveFixes.OstrichIncubatorFragility:
        Utility.ForEachItem((Func<Item, bool>) (item =>
        {
          if (item is StardewValley.Object object2 && object2.Fragility == 2 && object2.Name == "Ostrich Incubator")
            object2.Fragility = 0;
          return true;
        }));
        return true;
      case SaveFixes.LeoChildrenFix:
        Utility.FixChildNameCollisions();
        return true;
      case SaveFixes.Leo6HeartGermanFix:
        if (Utility.HasAnyPlayerSeenEvent("6497428") && !Game1.MasterPlayer.hasOrWillReceiveMail("leoMoved"))
        {
          Game1.addMailForTomorrow("leoMoved", true, true);
          Game1.player.team.requestLeoMove.Fire();
        }
        return true;
      case SaveFixes.BirdieQuestRemovedFix:
        foreach (Farmer allFarmer in Game1.getAllFarmers())
        {
          if (allFarmer.hasQuest("130"))
          {
            foreach (Quest quest in (NetList<Quest, NetRef<Quest>>) allFarmer.questLog)
            {
              if (quest.id.Value == "130")
                quest.canBeCancelled.Value = true;
            }
          }
          if (allFarmer.hasOrWillReceiveMail("birdieQuestBegun") && !allFarmer.hasOrWillReceiveMail("birdieQuestFinished"))
            allFarmer.addQuest("130");
        }
        return true;
      case SaveFixes.SkippedSummit:
        if (Game1.MasterPlayer.mailReceived.Contains("Farm_Eternal"))
        {
          foreach (Farmer allFarmer in Game1.getAllFarmers())
          {
            if (!allFarmer.songsHeard.Contains("end_credits"))
              allFarmer.mailReceived.Remove("Summit_event");
          }
        }
        return true;
      default:
        return false;
    }
  }

  /// <summary>Reset all weapon stats to reflect any changes in buffs.</summary>
  public static void ResetForges()
  {
    Utility.ForEachItem((Func<Item, bool>) (item =>
    {
      if (item is MeleeWeapon meleeWeapon2)
        meleeWeapon2.RecalculateAppliedForges();
      return true;
    }));
  }
}
