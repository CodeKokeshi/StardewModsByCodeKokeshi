// Decompiled with JetBrains decompiler
// Type: StardewValley.ItemTypeDefinitions.MannequinDataDefinition
// Assembly: Stardew Valley, Version=1.6.15.24356, Culture=neutral, PublicKeyToken=null
// MVID: AA1F513A-94F0-4EF6-A35F-2D5B4A3721BF
// Assembly location: D:\SteamLibrary\steamapps\common\Stardew Valley\Stardew Valley.dll
// XML documentation location: D:\SteamLibrary\steamapps\common\Stardew Valley\Stardew Valley.xml

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using StardewValley.GameData;
using StardewValley.Objects;
using StardewValley.TokenizableStrings;
using System.Collections.Generic;

#nullable disable
namespace StardewValley.ItemTypeDefinitions;

/// <summary>Manages the data for mannequin items.</summary>
public class MannequinDataDefinition : BaseItemDataDefinition
{
  /// <inheritdoc />
  public override string Identifier => "(M)";

  /// <inheritdoc />
  public override string StandardDescriptor => "M";

  /// <inheritdoc />
  public override IEnumerable<string> GetAllIds() => (IEnumerable<string>) this.GetDataSheet().Keys;

  /// <inheritdoc />
  public override bool Exists(string itemId) => this.GetDataSheet().ContainsKey(itemId);

  /// <inheritdoc />
  public override ParsedItemData GetData(string itemId)
  {
    MannequinData mannequinData;
    return !this.GetDataSheet().TryGetValue(itemId, out mannequinData) ? (ParsedItemData) null : new ParsedItemData((IItemDataDefinition) this, itemId, mannequinData.SheetIndex, mannequinData.Texture ?? "TileSheets/Mannequins", itemId, TokenParser.ParseText(mannequinData.DisplayName), TokenParser.ParseText(mannequinData.Description), -24, (string) null, (object) null);
  }

  /// <inheritdoc />
  public override Item CreateItem(ParsedItemData data) => (Item) new Mannequin(data.ItemId);

  /// <inheritdoc />
  public override Rectangle GetSourceRect(ParsedItemData data, Texture2D texture, int spriteIndex)
  {
    return Object.getSourceRectForBigCraftable(texture, spriteIndex);
  }

  /// <summary>Get the item type's data asset.</summary>
  protected Dictionary<string, MannequinData> GetDataSheet()
  {
    return DataLoader.Mannequins(Game1.content);
  }
}
