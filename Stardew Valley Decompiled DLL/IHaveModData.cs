// Decompiled with JetBrains decompiler
// Type: StardewValley.IHaveModData
// Assembly: Stardew Valley, Version=1.6.15.24356, Culture=neutral, PublicKeyToken=null
// MVID: AA1F513A-94F0-4EF6-A35F-2D5B4A3721BF
// Assembly location: D:\SteamLibrary\steamapps\common\Stardew Valley\Stardew Valley.dll
// XML documentation location: D:\SteamLibrary\steamapps\common\Stardew Valley\Stardew Valley.xml

using StardewValley.Mods;

#nullable disable
namespace StardewValley;

/// <summary>An instance with a <see cref="T:StardewValley.Mods.ModDataDictionary" /> field for custom mod data.</summary>
public interface IHaveModData
{
  /// <summary>Custom metadata for this instance, synchronized in multiplayer and persisted in the save file.</summary>
  ModDataDictionary modData { get; }

  /// <summary>The <see cref="P:StardewValley.IHaveModData.modData" /> adjusted for save file serialization. This returns null during save if it's empty. Most code should use <see cref="P:StardewValley.IHaveModData.modData" /> instead.</summary>
  ModDataDictionary modDataForSerialization { get; set; }
}
