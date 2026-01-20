// Decompiled with JetBrains decompiler
// Type: StardewValley.ItemTypeDefinitions.BootsDataDefinition
// Assembly: Stardew Valley, Version=1.6.15.24356, Culture=neutral, PublicKeyToken=null
// MVID: AA1F513A-94F0-4EF6-A35F-2D5B4A3721BF
// Assembly location: D:\SteamLibrary\steamapps\common\Stardew Valley\Stardew Valley.dll
// XML documentation location: D:\SteamLibrary\steamapps\common\Stardew Valley\Stardew Valley.xml

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using StardewValley.Objects;
using System;
using System.Collections.Generic;

#nullable disable
namespace StardewValley.ItemTypeDefinitions;

/// <summary>Manages the data for boot items.</summary>
public class BootsDataDefinition : BaseItemDataDefinition
{
  /// <inheritdoc />
  public override string Identifier => "(B)";

  /// <inheritdoc />
  public override string StandardDescriptor => "B";

  /// <inheritdoc />
  public override IEnumerable<string> GetAllIds() => (IEnumerable<string>) this.GetDataSheet().Keys;

  /// <inheritdoc />
  public override bool Exists(string itemId)
  {
    return itemId != null && this.GetDataSheet().ContainsKey(itemId);
  }

  /// <inheritdoc />
  public override ParsedItemData GetData(string itemId)
  {
    string[] rawData = this.GetRawData(itemId);
    return rawData == null ? (ParsedItemData) null : new ParsedItemData((IItemDataDefinition) this, itemId, this.GetSpriteIndex(itemId, rawData), ArgUtility.Get(rawData, 9) ?? "Maps\\springobjects", ArgUtility.Get(rawData, 0), ArgUtility.Get(rawData, 6), ArgUtility.Get(rawData, 1), -97, (string) null, (object) rawData);
  }

  /// <inheritdoc />
  public override Rectangle GetSourceRect(ParsedItemData data, Texture2D texture, int spriteIndex)
  {
    if (data == null)
      throw new ArgumentNullException(nameof (data));
    return texture != null ? Game1.getSourceRectForStandardTileSheet(texture, spriteIndex, 16 /*0x10*/, 16 /*0x10*/) : throw new ArgumentNullException(nameof (texture));
  }

  /// <inheritdoc />
  public override Item CreateItem(ParsedItemData data)
  {
    return data != null ? (Item) new Boots(data.ItemId) : throw new ArgumentNullException(nameof (data));
  }

  /// <summary>Get the item type's data asset.</summary>
  protected Dictionary<string, string> GetDataSheet() => DataLoader.Boots(Game1.content);

  /// <summary>Get the raw data fields for an item.</summary>
  /// <param name="itemId">The unqualified item ID.</param>
  protected string[] GetRawData(string itemId)
  {
    string str;
    return itemId == null || !this.GetDataSheet().TryGetValue(itemId, out str) ? (string[]) null : str.Split('/');
  }

  /// <summary>Get the sprite index for an item.</summary>
  /// <param name="itemId">The unqualified item ID.</param>
  /// <param name="fields">The raw data fields.</param>
  protected int GetSpriteIndex(string itemId, string[] fields)
  {
    int spriteIndex = ArgUtility.GetInt(fields, 8, -1);
    if (spriteIndex > -1)
      return spriteIndex;
    int result;
    return int.TryParse(itemId, out result) ? result : -1;
  }
}
