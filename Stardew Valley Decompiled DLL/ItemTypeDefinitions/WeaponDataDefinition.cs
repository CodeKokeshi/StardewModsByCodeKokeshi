// Decompiled with JetBrains decompiler
// Type: StardewValley.ItemTypeDefinitions.WeaponDataDefinition
// Assembly: Stardew Valley, Version=1.6.15.24356, Culture=neutral, PublicKeyToken=null
// MVID: AA1F513A-94F0-4EF6-A35F-2D5B4A3721BF
// Assembly location: D:\SteamLibrary\steamapps\common\Stardew Valley\Stardew Valley.dll
// XML documentation location: D:\SteamLibrary\steamapps\common\Stardew Valley\Stardew Valley.xml

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using StardewValley.GameData.Weapons;
using StardewValley.TokenizableStrings;
using StardewValley.Tools;
using System;
using System.Collections.Generic;

#nullable disable
namespace StardewValley.ItemTypeDefinitions;

/// <summary>Manages the data for weapon items.</summary>
public class WeaponDataDefinition : BaseItemDataDefinition
{
  /// <inheritdoc />
  public override string Identifier => "(W)";

  /// <inheritdoc />
  public override string StandardDescriptor => "W";

  /// <inheritdoc />
  public override IEnumerable<string> GetAllIds() => (IEnumerable<string>) Game1.weaponData.Keys;

  /// <inheritdoc />
  public override bool Exists(string itemId)
  {
    return itemId != null && Game1.weaponData.ContainsKey(itemId);
  }

  /// <inheritdoc />
  public override ParsedItemData GetData(string itemId)
  {
    WeaponData rawData = this.GetRawData(itemId);
    return rawData == null ? (ParsedItemData) null : new ParsedItemData((IItemDataDefinition) this, itemId, rawData.SpriteIndex, rawData.Texture, rawData.Name, TokenParser.ParseText(rawData.DisplayName), TokenParser.ParseText(rawData.Description), MeleeWeapon.IsScythe("(W)" + itemId) ? -99 : -98, (string) null, (object) rawData);
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
    string itemId = data != null ? data.ItemId : throw new ArgumentNullException(nameof (data));
    return !(itemId == "32") && !(itemId == "33") && !(itemId == "34") ? (Item) new MeleeWeapon(itemId) : (Item) new Slingshot(itemId);
  }

  /// <summary>Get the raw data fields for an item.</summary>
  /// <param name="itemId">The unqualified item ID.</param>
  protected WeaponData GetRawData(string itemId)
  {
    WeaponData weaponData;
    return itemId == null || !Game1.weaponData.TryGetValue(itemId, out weaponData) ? (WeaponData) null : weaponData;
  }

  /// <summary>Get the sprite index for an item.</summary>
  /// <param name="itemId">The unqualified item ID.</param>
  /// <param name="fields">The raw data fields.</param>
  protected int GetSpriteIndex(string itemId, string[] fields)
  {
    int spriteIndex = ArgUtility.GetInt(fields, 15, -1);
    if (spriteIndex > -1)
      return spriteIndex;
    int result;
    return int.TryParse(itemId, out result) ? result : -1;
  }
}
