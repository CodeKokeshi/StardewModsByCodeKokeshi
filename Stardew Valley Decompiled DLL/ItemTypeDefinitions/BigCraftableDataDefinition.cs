// Decompiled with JetBrains decompiler
// Type: StardewValley.ItemTypeDefinitions.BigCraftableDataDefinition
// Assembly: Stardew Valley, Version=1.6.15.24356, Culture=neutral, PublicKeyToken=null
// MVID: AA1F513A-94F0-4EF6-A35F-2D5B4A3721BF
// Assembly location: D:\SteamLibrary\steamapps\common\Stardew Valley\Stardew Valley.dll
// XML documentation location: D:\SteamLibrary\steamapps\common\Stardew Valley\Stardew Valley.xml

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using StardewValley.GameData.BigCraftables;
using StardewValley.Objects;
using StardewValley.TokenizableStrings;
using System;
using System.Collections.Generic;

#nullable disable
namespace StardewValley.ItemTypeDefinitions;

/// <summary>Manages the data for big craftable items.</summary>
public class BigCraftableDataDefinition : BaseItemDataDefinition
{
  /// <inheritdoc />
  public override string Identifier => "(BC)";

  /// <inheritdoc />
  public override string StandardDescriptor => "BO";

  /// <inheritdoc />
  public override IEnumerable<string> GetAllIds()
  {
    return (IEnumerable<string>) Game1.bigCraftableData.Keys;
  }

  /// <inheritdoc />
  public override bool Exists(string itemId)
  {
    return itemId != null && Game1.bigCraftableData.ContainsKey(itemId);
  }

  /// <inheritdoc />
  public override ParsedItemData GetData(string itemId)
  {
    BigCraftableData rawData = this.GetRawData(itemId);
    return rawData == null ? (ParsedItemData) null : new ParsedItemData((IItemDataDefinition) this, itemId, rawData.SpriteIndex, rawData.Texture ?? "TileSheets\\Craftables", rawData.Name, TokenParser.ParseText(rawData.DisplayName), TokenParser.ParseText(rawData.Description), -9, "Crafting", (object) rawData);
  }

  /// <inheritdoc />
  public override Item CreateItem(ParsedItemData data)
  {
    if (data == null)
      throw new ArgumentNullException(nameof (data));
    return data.QualifiedItemId == "(BC)221" ? (Item) new ItemPedestal(Vector2.Zero, (StardewValley.Object) null, false, Color.White) : (Item) new StardewValley.Object(Vector2.Zero, data.ItemId);
  }

  /// <inheritdoc />
  public override Rectangle GetSourceRect(ParsedItemData data, Texture2D texture, int spriteIndex)
  {
    if (data == null)
      throw new ArgumentNullException(nameof (data));
    return texture != null ? StardewValley.Object.getSourceRectForBigCraftable(texture, spriteIndex) : throw new ArgumentNullException(nameof (texture));
  }

  /// <summary>Get the raw data fields for an item.</summary>
  /// <param name="itemId">The unqualified item ID.</param>
  protected BigCraftableData GetRawData(string itemId)
  {
    BigCraftableData bigCraftableData;
    return itemId == null || !Game1.bigCraftableData.TryGetValue(itemId, out bigCraftableData) ? (BigCraftableData) null : bigCraftableData;
  }
}
