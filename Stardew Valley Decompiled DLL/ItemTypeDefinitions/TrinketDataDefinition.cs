// Decompiled with JetBrains decompiler
// Type: StardewValley.ItemTypeDefinitions.TrinketDataDefinition
// Assembly: Stardew Valley, Version=1.6.15.24356, Culture=neutral, PublicKeyToken=null
// MVID: AA1F513A-94F0-4EF6-A35F-2D5B4A3721BF
// Assembly location: D:\SteamLibrary\steamapps\common\Stardew Valley\Stardew Valley.dll
// XML documentation location: D:\SteamLibrary\steamapps\common\Stardew Valley\Stardew Valley.xml

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using StardewValley.GameData;
using StardewValley.Objects.Trinkets;
using StardewValley.TokenizableStrings;
using System.Collections.Generic;

#nullable disable
namespace StardewValley.ItemTypeDefinitions;

/// <summary>Manages the data for trinket items.</summary>
public class TrinketDataDefinition : BaseItemDataDefinition
{
  /// <inheritdoc />
  public override string Identifier => "(TR)";

  /// <inheritdoc />
  public override string StandardDescriptor => "TR";

  /// <inheritdoc />
  public override IEnumerable<string> GetAllIds() => (IEnumerable<string>) this.GetDataSheet().Keys;

  /// <inheritdoc />
  public override bool Exists(string itemId) => this.GetDataSheet().ContainsKey(itemId);

  /// <inheritdoc />
  public override ParsedItemData GetData(string itemId)
  {
    TrinketData trinketData;
    return !this.GetDataSheet().TryGetValue(itemId, out trinketData) ? (ParsedItemData) null : new ParsedItemData((IItemDataDefinition) this, itemId, trinketData.SheetIndex, trinketData.Texture, itemId, TokenParser.ParseText(trinketData.DisplayName), TokenParser.ParseText(trinketData.Description), -101, (string) null, (object) null);
  }

  /// <inheritdoc />
  public override Item CreateItem(ParsedItemData data)
  {
    return (Item) new Trinket(data.ItemId, Game1.random.Next(9999999));
  }

  /// <inheritdoc />
  public override Rectangle GetSourceRect(ParsedItemData data, Texture2D texture, int spriteIndex)
  {
    return Game1.getSourceRectForStandardTileSheet(texture, spriteIndex, 16 /*0x10*/, 16 /*0x10*/);
  }

  /// <summary>Get the item type's data asset.</summary>
  protected Dictionary<string, TrinketData> GetDataSheet() => DataLoader.Trinkets(Game1.content);
}
