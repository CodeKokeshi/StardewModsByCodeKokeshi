// Decompiled with JetBrains decompiler
// Type: StardewValley.ItemTypeDefinitions.BaseItemDataDefinition
// Assembly: Stardew Valley, Version=1.6.15.24356, Culture=neutral, PublicKeyToken=null
// MVID: AA1F513A-94F0-4EF6-A35F-2D5B4A3721BF
// Assembly location: D:\SteamLibrary\steamapps\common\Stardew Valley\Stardew Valley.dll
// XML documentation location: D:\SteamLibrary\steamapps\common\Stardew Valley\Stardew Valley.xml

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System.Collections.Generic;

#nullable disable
namespace StardewValley.ItemTypeDefinitions;

/// <summary>The base implementation for <see cref="T:StardewValley.ItemTypeDefinitions.IItemDataDefinition" /> instances.</summary>
public abstract class BaseItemDataDefinition : IItemDataDefinition
{
  /// <summary>A cache of parsed data by item ID.</summary>
  public Dictionary<string, ParsedItemData> ParsedItemCache = new Dictionary<string, ParsedItemData>();

  /// <inheritdoc />
  public abstract string Identifier { get; }

  /// <inheritdoc />
  public virtual string StandardDescriptor => (string) null;

  /// <inheritdoc />
  public abstract IEnumerable<string> GetAllIds();

  /// <inheritdoc />
  public abstract bool Exists(string itemId);

  /// <inheritdoc />
  public abstract ParsedItemData GetData(string itemId);

  /// <inheritdoc />
  public ParsedItemData GetErrorData(string itemId)
  {
    return new ParsedItemData((IItemDataDefinition) this, itemId, 0, this.GetErrorTextureName(), "ErrorItem", ItemRegistry.GetErrorItemName(itemId), "???", -1, (string) null, (object) null, true);
  }

  /// <inheritdoc />
  public abstract Item CreateItem(ParsedItemData data);

  /// <inheritdoc />
  public abstract Rectangle GetSourceRect(ParsedItemData data, Texture2D texture, int spriteIndex);

  /// <inheritdoc />
  public virtual Texture2D GetErrorTexture() => Game1.mouseCursors;

  /// <inheritdoc />
  public virtual string GetErrorTextureName() => "LooseSprites\\Cursors";

  /// <inheritdoc />
  public virtual Rectangle GetErrorSourceRect()
  {
    return new Rectangle(320, 496, 16 /*0x10*/, 16 /*0x10*/);
  }
}
