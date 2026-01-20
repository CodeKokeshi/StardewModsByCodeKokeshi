// Decompiled with JetBrains decompiler
// Type: StardewValley.PurchaseableKeyItem
// Assembly: Stardew Valley, Version=1.6.15.24356, Culture=neutral, PublicKeyToken=null
// MVID: AA1F513A-94F0-4EF6-A35F-2D5B4A3721BF
// Assembly location: D:\SteamLibrary\steamapps\common\Stardew Valley\Stardew Valley.dll
// XML documentation location: D:\SteamLibrary\steamapps\common\Stardew Valley\Stardew Valley.xml

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using StardewValley.ItemTypeDefinitions;
using System;
using System.Collections.Generic;

#nullable disable
namespace StardewValley;

public class PurchaseableKeyItem : ISalable, IHaveItemTypeId
{
  protected string _displayName = "";
  protected string _name = "";
  protected string _description = "";
  protected int _price;
  protected int _id;
  protected List<string> _tags;
  protected Action<Farmer> _onPurchase;

  /// <inheritdoc />
  public string TypeDefinitionId => "(Salable)";

  /// <inheritdoc />
  public string QualifiedItemId
  {
    get => $"{this.TypeDefinitionId}PurchaseableKeyItem.{this.id.ToString()}";
  }

  /// <inheritdoc />
  public string DisplayName => this._displayName;

  public int id => this._id;

  public List<string> tags => this._tags;

  /// <inheritdoc />
  public string Name => this._name;

  public bool IsRecipe
  {
    get => false;
    set
    {
    }
  }

  public int Stack
  {
    get => 1;
    set
    {
    }
  }

  public int Quality
  {
    get => 0;
    set
    {
    }
  }

  public PurchaseableKeyItem(
    string display_name,
    string display_description,
    int parent_sheet_index,
    Action<Farmer> on_purchase = null)
  {
    this._id = parent_sheet_index;
    this._name = display_name;
    this._displayName = display_name;
    this._description = display_description;
    this._onPurchase = on_purchase;
  }

  /// <inheritdoc />
  public string GetItemTypeId() => this.TypeDefinitionId;

  public void drawInMenu(
    SpriteBatch spriteBatch,
    Vector2 location,
    float scaleSize,
    float transparency,
    float layerDepth,
    StackDrawType drawStackNumber,
    Color color,
    bool drawShadow)
  {
    spriteBatch.Draw(Game1.objectSpriteSheet, location + new Vector2((float) (int) (32.0 * (double) scaleSize), (float) (int) (32.0 * (double) scaleSize)), new Rectangle?(Game1.getSourceRectForStandardTileSheet(Game1.objectSpriteSheet, this._id, 16 /*0x10*/, 16 /*0x10*/)), color * transparency, 0.0f, new Vector2(8f, 8f) * scaleSize, 4f * scaleSize, SpriteEffects.None, layerDepth);
  }

  public bool ShouldDrawIcon() => true;

  public string getDescription() => this._description;

  public int maximumStackSize() => 1;

  public int addToStack(Item stack) => 1;

  public bool canStackWith(ISalable other) => false;

  /// <inheritdoc />
  public int sellToStorePrice(long specificPlayerID = -1) => -1;

  /// <inheritdoc />
  public int salePrice(bool ignoreProfitMargins = false) => this._price;

  /// <inheritdoc />
  public bool appliesProfitMargins() => false;

  /// <inheritdoc />
  public bool actionWhenPurchased(string shopId)
  {
    Action<Farmer> onPurchase = this._onPurchase;
    if (onPurchase != null)
      onPurchase(Game1.player);
    return true;
  }

  public bool CanBuyItem(Farmer farmer) => true;

  public bool IsInfiniteStock() => false;

  public ISalable GetSalableInstance() => (ISalable) this;

  /// <inheritdoc />
  public void FixStackSize()
  {
  }

  /// <inheritdoc />
  public void FixQuality()
  {
  }
}
