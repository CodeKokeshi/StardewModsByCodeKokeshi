// Decompiled with JetBrains decompiler
// Type: StardewValley.ItemTypeDefinitions.PantsDataDefinition
// Assembly: Stardew Valley, Version=1.6.15.24356, Culture=neutral, PublicKeyToken=null
// MVID: AA1F513A-94F0-4EF6-A35F-2D5B4A3721BF
// Assembly location: D:\SteamLibrary\steamapps\common\Stardew Valley\Stardew Valley.dll
// XML documentation location: D:\SteamLibrary\steamapps\common\Stardew Valley\Stardew Valley.xml

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using StardewValley.GameData.Pants;
using StardewValley.Objects;
using StardewValley.TokenizableStrings;
using System;
using System.Collections.Generic;

#nullable disable
namespace StardewValley.ItemTypeDefinitions;

/// <summary>Manages the data for pants clothing items.</summary>
public class PantsDataDefinition : BaseItemDataDefinition
{
  /// <inheritdoc />
  public override string Identifier => "(P)";

  /// <inheritdoc />
  public override string StandardDescriptor => "C";

  /// <inheritdoc />
  public override IEnumerable<string> GetAllIds() => (IEnumerable<string>) Game1.pantsData.Keys;

  /// <inheritdoc />
  public override bool Exists(string itemId)
  {
    return itemId != null && Game1.pantsData.ContainsKey(itemId);
  }

  /// <inheritdoc />
  public override ParsedItemData GetData(string itemId)
  {
    PantsData rawData;
    return itemId == null || !Game1.pantsData.TryGetValue(itemId, out rawData) ? (ParsedItemData) null : new ParsedItemData((IItemDataDefinition) this, itemId, rawData.SpriteIndex, rawData.Texture ?? "Characters\\Farmer\\pants", rawData.Name, TokenParser.ParseText(rawData.DisplayName), TokenParser.ParseText(rawData.Description), -100, (string) null, (object) rawData);
  }

  /// <inheritdoc />
  public override Rectangle GetSourceRect(ParsedItemData data, Texture2D texture, int spriteIndex)
  {
    if (data == null)
      throw new ArgumentNullException(nameof (data));
    if (texture == null)
      throw new ArgumentNullException(nameof (texture));
    return new Rectangle(192 /*0xC0*/ * (spriteIndex % (texture.Width / 192 /*0xC0*/)), 688 * (spriteIndex / (texture.Width / 192 /*0xC0*/)) + 672, 16 /*0x10*/, 16 /*0x10*/);
  }

  /// <inheritdoc />
  public override Item CreateItem(ParsedItemData data)
  {
    return data != null ? (Item) new Clothing(data.ItemId) : throw new ArgumentNullException(nameof (data));
  }
}
