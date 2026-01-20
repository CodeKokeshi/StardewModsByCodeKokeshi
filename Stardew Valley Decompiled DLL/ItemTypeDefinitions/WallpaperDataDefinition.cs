// Decompiled with JetBrains decompiler
// Type: StardewValley.ItemTypeDefinitions.WallpaperDataDefinition
// Assembly: Stardew Valley, Version=1.6.15.24356, Culture=neutral, PublicKeyToken=null
// MVID: AA1F513A-94F0-4EF6-A35F-2D5B4A3721BF
// Assembly location: D:\SteamLibrary\steamapps\common\Stardew Valley\Stardew Valley.dll
// XML documentation location: D:\SteamLibrary\steamapps\common\Stardew Valley\Stardew Valley.xml

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using StardewValley.GameData;
using StardewValley.Objects;
using System;
using System.Collections.Generic;

#nullable enable
namespace StardewValley.ItemTypeDefinitions;

/// <summary>Manages the data for wallpaper items.</summary>
public class WallpaperDataDefinition : BaseItemDataDefinition
{
  /// <summary>The number of older wallpapers in <c>Maps\walls_and_floors</c> that aren't defined in a data asset.</summary>
  protected const int LegacyWallpaperCount = 112 /*0x70*/;

  /// <inheritdoc />
  public override 
  #nullable disable
  string Identifier => "(WP)";

  /// <inheritdoc />
  public override IEnumerable<string> GetAllIds()
  {
    int i;
    for (i = 0; i < 112 /*0x70*/; ++i)
      yield return i.ToString();
    foreach (ModWallpaperOrFlooring set in DataLoader.AdditionalWallpaperFlooring(Game1.content))
    {
      if (!set.IsFlooring)
      {
        for (i = 0; i < set.Count; ++i)
          yield return $"{set.Id}:{i.ToString()}";
      }
    }
  }

  /// <inheritdoc />
  public override bool Exists(string itemId)
  {
    if (itemId == null)
      return false;
    if (this.TryParseLegacyId(itemId, out int _))
      return true;
    string id;
    int index;
    this.ParseStandardId(itemId, out id, out index);
    ModWallpaperOrFlooring wallpaperSet = this.GetWallpaperSet(id);
    int num = index;
    int? count = wallpaperSet?.Count;
    return num < count.GetValueOrDefault() & count.HasValue;
  }

  /// <inheritdoc />
  public override ParsedItemData GetData(string itemId)
  {
    if (itemId != null)
    {
      int legacyId;
      if (this.TryParseLegacyId(itemId, out legacyId))
        return this.GetData(itemId, legacyId, "Maps\\walls_and_floors", (object) null);
      string id;
      int index;
      this.ParseStandardId(itemId, out id, out index);
      ModWallpaperOrFlooring wallpaperSet = this.GetWallpaperSet(id);
      if (wallpaperSet != null)
        return this.GetData(itemId, index, wallpaperSet.Texture, (object) wallpaperSet);
    }
    return (ParsedItemData) null;
  }

  /// <inheritdoc />
  public override Rectangle GetSourceRect(ParsedItemData data, Texture2D texture, int spriteIndex)
  {
    if (data == null)
      throw new ArgumentNullException(nameof (data));
    return texture != null ? Game1.getSourceRectForStandardTileSheet(texture, spriteIndex, 16 /*0x10*/, 48 /*0x30*/) : throw new ArgumentNullException(nameof (texture));
  }

  /// <inheritdoc />
  public override Item CreateItem(ParsedItemData data)
  {
    if (data == null)
      throw new ArgumentNullException(nameof (data));
    int legacyId;
    if (this.TryParseLegacyId(data.ItemId, out legacyId))
      return (Item) new Wallpaper(legacyId);
    string id;
    int index;
    this.ParseStandardId(data.ItemId, out id, out index);
    return (Item) new Wallpaper(id, index);
  }

  /// <summary>Try to parse the ID as a vanilla wallpaper ID that's not defined in <c>Data/AdditionalWallpaperFlooring</c>.</summary>
  /// <param name="raw">The item ID to parse.</param>
  /// <param name="legacyId">The parsed legacy ID, if applicable.</param>
  /// <returns>Returns whether the ID is a legacy vanilla ID.</returns>
  protected bool TryParseLegacyId(string raw, out int legacyId)
  {
    return int.TryParse(raw, out legacyId) && legacyId >= 0 && legacyId < 112 /*0x70*/;
  }

  /// <summary>Parse a standard wallpaper ID that should be defined in <c>Data/AdditionalWallpaperFlooring</c>. This may include a sprite index within the texture. For example, <c>ExampleMod.CustomWallpapers:5</c> is a wallpaper at index 5 in the spritesheet texture defined under <c>ExampleMod.CustomWallpapers</c> in <c>Data/AdditionalWallpaperFlooring</c>.</summary>
  /// <param name="raw">The item ID to parse.</param>
  /// <param name="id">The item ID without the index.</param>
  /// <param name="index">The sprite index, if any.</param>
  protected void ParseStandardId(string raw, out string id, out int index)
  {
    id = raw;
    index = 0;
    string[] strArray = raw.Split(':', 2);
    int result;
    if (strArray.Length != 2 || !int.TryParse(strArray[1], out result))
      return;
    id = strArray[0];
    index = result;
  }

  /// <summary>Get a set of wallpaper items from the data asset.</summary>
  /// <param name="setId">The unqualified item ID.</param>
  protected ModWallpaperOrFlooring GetWallpaperSet(string setId)
  {
    foreach (ModWallpaperOrFlooring wallpaperOrFlooring in DataLoader.AdditionalWallpaperFlooring(Game1.content))
    {
      if (wallpaperOrFlooring.Id == setId)
        return wallpaperOrFlooring.IsFlooring ? (ModWallpaperOrFlooring) null : wallpaperOrFlooring;
    }
    return (ModWallpaperOrFlooring) null;
  }

  /// <summary>Get the base data for a wallpaper item.</summary>
  /// <param name="itemId">The unqualified item ID.</param>
  /// <param name="spriteIndex">The item's index within the sprite sheet.</param>
  /// <param name="textureName">The asset name for the sprite sheet to use when drawing the item to the screen.</param>
  /// <param name="rawData">The raw data fields from the underlying data asset if applicable, else <c>null</c>.</param>
  protected ParsedItemData GetData(
    string itemId,
    int spriteIndex,
    string textureName,
    object rawData)
  {
    return new ParsedItemData((IItemDataDefinition) this, itemId, spriteIndex, textureName, "Wallpaper", Game1.content.LoadString("Strings\\StringsFromCSFiles:Wallpaper.cs.13204"), Game1.content.LoadString("Strings\\StringsFromCSFiles:Wallpaper.cs.13206"), 0, (string) null, rawData);
  }
}
