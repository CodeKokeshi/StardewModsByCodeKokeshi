// Decompiled with JetBrains decompiler
// Type: StardewValley.SaveMigrations.SaveMigrator
// Assembly: Stardew Valley, Version=1.6.15.24356, Culture=neutral, PublicKeyToken=null
// MVID: AA1F513A-94F0-4EF6-A35F-2D5B4A3721BF
// Assembly location: D:\SteamLibrary\steamapps\common\Stardew Valley\Stardew Valley.dll
// XML documentation location: D:\SteamLibrary\steamapps\common\Stardew Valley\Stardew Valley.xml

using System;
using System.Collections.Generic;

#nullable disable
namespace StardewValley.SaveMigrations;

/// <summary>Manages and applies migrations to save files.</summary>
public static class SaveMigrator
{
  /// <summary>The highest save fix that can be applied.</summary>
  public static readonly SaveFixes LatestSaveFix = SaveFixes.FixDuplicateMissedMail;

  /// <summary>Apply all applicable save fixes to the currently loaded save file.</summary>
  public static void ApplySaveFixes()
  {
    if (!Game1.hasApplied1_3_UpdateChanges)
      SaveMigrator_1_3.ApplyLegacyChanges();
    if (!Game1.hasApplied1_4_UpdateChanges)
      SaveMigrator_1_4.ApplyLegacyChanges();
    if (Game1.lastAppliedSaveFix >= SaveMigrator.LatestSaveFix)
      return;
    List<ISaveMigrator> allMigrators = SaveMigrator.GetAllMigrators(true);
    for (SaveFixes saveFix = Game1.lastAppliedSaveFix + 1; saveFix < SaveFixes.MAX; ++saveFix)
    {
      if (Enum.IsDefined(typeof (SaveFixes), (object) saveFix))
      {
        Game1.log.Debug("Applying save fix: " + saveFix.ToString());
        foreach (ISaveMigrator saveMigrator in allMigrators)
        {
          if (saveMigrator.ApplySaveFix(saveFix))
            break;
        }
      }
      Game1.lastAppliedSaveFix = saveFix;
    }
  }

  /// <summary>Apply a single save fix to the currently loaded save file.</summary>
  /// <param name="fix">The save fix to apply.</param>
  /// <param name="loadedItems">A list of all items loaded from the save.</param>
  public static void ApplySingleSaveFix(SaveFixes fix, List<Item> loadedItems)
  {
    using (List<ISaveMigrator>.Enumerator enumerator = SaveMigrator.GetAllMigrators().GetEnumerator())
    {
      do
        ;
      while (enumerator.MoveNext() && !enumerator.Current.ApplySaveFix(fix));
    }
  }

  /// <summary>Get all save migrators that can be applied.</summary>
  /// <param name="reverse">Whether to get migrations in reverse order (from newer to older). This is used when applying all migrations, since most fixes applied will be in a newer version.</param>
  public static List<ISaveMigrator> GetAllMigrators(bool reverse = false)
  {
    List<ISaveMigrator> allMigrators = new List<ISaveMigrator>();
    foreach (Type type in typeof (ISaveMigrator).Assembly.GetTypes())
    {
      if (type.IsClass && !type.IsAbstract && typeof (ISaveMigrator).IsAssignableFrom(type))
      {
        ISaveMigrator saveMigrator = (ISaveMigrator) Activator.CreateInstance(type) ?? throw new InvalidOperationException($"Failed to create instance of save migration '{type.FullName}'.");
        allMigrators.Add(saveMigrator);
      }
    }
    if (reverse)
      allMigrators.Sort((Comparison<ISaveMigrator>) ((a, b) => -a.GameVersion.CompareTo(b.GameVersion)));
    else
      allMigrators.Sort((Comparison<ISaveMigrator>) ((a, b) => a.GameVersion.CompareTo(b.GameVersion)));
    return allMigrators;
  }
}
