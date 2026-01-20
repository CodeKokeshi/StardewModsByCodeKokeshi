// Decompiled with JetBrains decompiler
// Type: StardewValley.ItemTypeDefinitions.ShirtDataDefinition
// Assembly: Stardew Valley, Version=1.6.15.24356, Culture=neutral, PublicKeyToken=null
// MVID: AA1F513A-94F0-4EF6-A35F-2D5B4A3721BF
// Assembly location: D:\SteamLibrary\steamapps\common\Stardew Valley\Stardew Valley.dll
// XML documentation location: D:\SteamLibrary\steamapps\common\Stardew Valley\Stardew Valley.xml

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using StardewValley.GameData.Shirts;
using StardewValley.Objects;
using StardewValley.TokenizableStrings;
using System;
using System.Collections.Generic;

#nullable disable
namespace StardewValley.ItemTypeDefinitions;

/// <summary>Manages the data for shirt clothing items.</summary>
/// <remarks>Shirt textures must be evenly split into two parts: the left half contains the clothing sprites, and the right half contains equivalent dye masks (if any). The texture can be any width as long as it's evenly split (e.g. three clothing sprites + three dye masks wide).</remarks>
public class ShirtDataDefinition : BaseItemDataDefinition
{
  /// <inheritdoc />
  public override string Identifier => "(S)";

  /// <inheritdoc />
  public override string StandardDescriptor => "C";

  /// <inheritdoc />
  public override IEnumerable<string> GetAllIds() => (IEnumerable<string>) Game1.shirtData.Keys;

  /// <inheritdoc />
  public override bool Exists(string itemId)
  {
    return itemId != null && Game1.shirtData.ContainsKey(itemId);
  }

  /// <inheritdoc />
  public override ParsedItemData GetData(string itemId)
  {
    ShirtData rawData;
    return itemId == null || !Game1.shirtData.TryGetValue(itemId, out rawData) ? (ParsedItemData) null : new ParsedItemData((IItemDataDefinition) this, itemId, rawData.SpriteIndex, rawData.Texture ?? "Characters\\Farmer\\shirts", rawData.Name, TokenParser.ParseText(rawData.DisplayName), TokenParser.ParseText(rawData.Description), -100, (string) null, (object) rawData);
  }

  /// <inheritdoc />
  public override Rectangle GetSourceRect(ParsedItemData data, Texture2D texture, int spriteIndex)
  {
    if (data == null)
      throw new ArgumentNullException(nameof (data));
    if (texture == null)
      throw new ArgumentNullException(nameof (texture));
    int num = texture.Width / 2;
    return new Rectangle(spriteIndex * 8 % num, spriteIndex * 8 / num * 32 /*0x20*/, 8, 8);
  }

  /// <inheritdoc />
  public override Item CreateItem(ParsedItemData data)
  {
    return data != null ? (Item) new Clothing(data.ItemId) : throw new ArgumentNullException(nameof (data));
  }
}
