// Decompiled with JetBrains decompiler
// Type: StardewValley.SaveMigrations.SaveMigrator_1_3
// Assembly: Stardew Valley, Version=1.6.15.24356, Culture=neutral, PublicKeyToken=null
// MVID: AA1F513A-94F0-4EF6-A35F-2D5B4A3721BF
// Assembly location: D:\SteamLibrary\steamapps\common\Stardew Valley\Stardew Valley.dll
// XML documentation location: D:\SteamLibrary\steamapps\common\Stardew Valley\Stardew Valley.xml

using Microsoft.Xna.Framework;
using StardewValley.Buildings;
using StardewValley.Characters;
using StardewValley.Locations;
using StardewValley.Objects;
using System;
using System.Collections.Generic;

#nullable disable
namespace StardewValley.SaveMigrations;

/// <summary>Migrates existing save files for compatibility with Stardew Valley 1.3.</summary>
public class SaveMigrator_1_3 : ISaveMigrator
{
  /// <inheritdoc />
  public Version GameVersion { get; } = new Version(1, 3);

  /// <inheritdoc />
  public bool ApplySaveFix(SaveFixes saveFix) => false;

  /// <summary>Apply one-time save migrations which predate <see cref="T:StardewValley.SaveMigrations.SaveFixes" />.</summary>
  public static void ApplyLegacyChanges()
  {
    if (!Game1.IsMasterGame)
      return;
    FarmHouse farmHouse = Game1.RequireLocation<FarmHouse>("FarmHouse");
    farmHouse.furniture.Add(new Furniture("1792", Utility.PointToVector2(farmHouse.getFireplacePoint())));
    GameLocation gameLocation = Game1.RequireLocation("Town");
    if (!Game1.MasterPlayer.mailReceived.Contains("JojaMember") && gameLocation.CanItemBePlacedHere(new Vector2(57f, 16f)))
      gameLocation.objects.Add(new Vector2(57f, 16f), ItemRegistry.Create<StardewValley.Object>("(BC)55"));
    SaveMigrator_1_3.MarkFloorChestAsCollectedIfNecessary(10);
    SaveMigrator_1_3.MarkFloorChestAsCollectedIfNecessary(20);
    SaveMigrator_1_3.MarkFloorChestAsCollectedIfNecessary(40);
    SaveMigrator_1_3.MarkFloorChestAsCollectedIfNecessary(50);
    SaveMigrator_1_3.MarkFloorChestAsCollectedIfNecessary(60);
    SaveMigrator_1_3.MarkFloorChestAsCollectedIfNecessary(70);
    SaveMigrator_1_3.MarkFloorChestAsCollectedIfNecessary(80 /*0x50*/);
    SaveMigrator_1_3.MarkFloorChestAsCollectedIfNecessary(90);
    SaveMigrator_1_3.MarkFloorChestAsCollectedIfNecessary(100);
    Utility.ForEachVillager((Func<NPC, bool>) (villager =>
    {
      if (villager.datingFarmer.GetValueOrDefault())
      {
        Friendship friendship;
        if (Game1.player.friendshipData.TryGetValue(villager.Name, out friendship) && !friendship.IsDating())
          friendship.Status = FriendshipStatus.Dating;
        villager.datingFarmer = new bool?();
      }
      if (villager.divorcedFromFarmer.GetValueOrDefault())
      {
        Friendship friendship;
        if (Game1.player.friendshipData.TryGetValue(villager.Name, out friendship) && !friendship.IsDating() && !friendship.IsDivorced())
          friendship.Status = FriendshipStatus.Divorced;
        villager.divorcedFromFarmer = new bool?();
      }
      return true;
    }));
    SaveMigrator_1_3.MigrateHorseIds();
    Game1.hasApplied1_3_UpdateChanges = true;
  }

  /// <summary>Mark a mine floor chest as collected if needed.</summary>
  /// <param name="floorNumber">The mine level.</param>
  /// <remarks>This should only be used on pre-1.3 saves, because the addition of multiplayer means it's not safe to assume that the local player is the one who opened the chest.</remarks>
  public static void MarkFloorChestAsCollectedIfNecessary(int floorNumber)
  {
    MineInfo mineInfo;
    if (MineShaft.permanentMineChanges == null || !MineShaft.permanentMineChanges.TryGetValue(floorNumber, out mineInfo) || mineInfo.chestsLeft > 0)
      return;
    Game1.player.chestConsumedMineLevels[floorNumber] = true;
  }

  /// <summary>Migrate the obsolete <see cref="F:StardewValley.Farmer.obsolete_friendships" /> into the new <see cref="F:StardewValley.Farmer.friendshipData" /> field, if applicable.</summary>
  /// <param name="player">The player whose data to migrate.</param>
  public static void MigrateFriendshipData(Farmer player)
  {
    if (player.obsolete_friendships != null && player.friendshipData.Length == 0)
    {
      foreach (KeyValuePair<string, int[]> obsoleteFriendship in (Dictionary<string, int[]>) player.obsolete_friendships)
        player.friendshipData[obsoleteFriendship.Key] = new Friendship(obsoleteFriendship.Value[0])
        {
          GiftsThisWeek = obsoleteFriendship.Value[1],
          TalkedToToday = obsoleteFriendship.Value[2] != 0,
          GiftsToday = obsoleteFriendship.Value[3],
          ProposalRejected = obsoleteFriendship.Value[4] != 0
        };
      player.obsolete_friendships = (SerializableDictionary<string, int[]>) null;
    }
    if (string.IsNullOrEmpty(player.spouse))
      return;
    bool flag = player.spouse.Contains("engaged");
    string key = player.spouse.Replace("engaged", "");
    Friendship friendship = player.friendshipData[key];
    if (((friendship.Status == FriendshipStatus.Friendly ? 1 : (friendship.Status == FriendshipStatus.Dating ? 1 : 0)) | (flag ? 1 : 0)) == 0)
      return;
    friendship.Status = flag ? FriendshipStatus.Engaged : FriendshipStatus.Married;
    player.spouse = key;
    if (flag)
      return;
    friendship.WeddingDate = WorldDate.Now();
    friendship.WeddingDate.TotalDays -= player.obsolete_daysMarried.GetValueOrDefault();
    player.obsolete_daysMarried = new int?();
  }

  /// <summary>Fix the <see cref="P:StardewValley.Characters.Horse.HorseId" /> value for pre-1.3 horses.</summary>
  private static void MigrateHorseIds()
  {
    List<Stable> stablesMissingHorses = new List<Stable>();
    Utility.ForEachBuilding<Stable>((Func<Stable, bool>) (stable =>
    {
      if (stable.getStableHorse() == null && stable.GetParentLocation() != null)
        stablesMissingHorses.Add(stable);
      return true;
    }));
    for (int index = stablesMissingHorses.Count - 1; index >= 0; --index)
    {
      Stable stable = stablesMissingHorses[index];
      GameLocation parentLocation = stable.GetParentLocation();
      Rectangle boundingBox = stable.GetBoundingBox();
      foreach (NPC character in parentLocation.characters)
      {
        if (character is Horse horse && horse.HorseId == Guid.Empty && boundingBox.Intersects(horse.GetBoundingBox()))
        {
          horse.HorseId = stable.HorseId;
          stablesMissingHorses.RemoveAt(index);
          break;
        }
      }
    }
    for (int index = stablesMissingHorses.Count - 1; index >= 0; --index)
    {
      Stable stable = stablesMissingHorses[index];
      foreach (NPC character in stable.GetParentLocation().characters)
      {
        if (character is Horse horse && horse.HorseId == Guid.Empty)
        {
          horse.HorseId = stable.HorseId;
          stablesMissingHorses.RemoveAt(index);
          break;
        }
      }
    }
    foreach (Stable stable in stablesMissingHorses)
      stable.grabHorse();
  }
}
