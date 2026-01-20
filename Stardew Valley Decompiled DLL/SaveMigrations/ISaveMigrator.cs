// Decompiled with JetBrains decompiler
// Type: StardewValley.SaveMigrations.ISaveMigrator
// Assembly: Stardew Valley, Version=1.6.15.24356, Culture=neutral, PublicKeyToken=null
// MVID: AA1F513A-94F0-4EF6-A35F-2D5B4A3721BF
// Assembly location: D:\SteamLibrary\steamapps\common\Stardew Valley\Stardew Valley.dll
// XML documentation location: D:\SteamLibrary\steamapps\common\Stardew Valley\Stardew Valley.xml

using System;

#nullable disable
namespace StardewValley.SaveMigrations;

/// <summary>Migrates existing save files for compatibility with a newer game version.</summary>
public interface ISaveMigrator
{
  /// <summary>The game version to which the migration applies.</summary>
  Version GameVersion { get; }

  /// <summary>Apply a migration to the currently loaded save file.</summary>
  /// <param name="saveFix">The save migration to apply.</param>
  /// <returns>Returns whether the migration was applied.</returns>
  bool ApplySaveFix(SaveFixes saveFix);
}
