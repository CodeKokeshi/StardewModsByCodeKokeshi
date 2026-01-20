// Decompiled with JetBrains decompiler
// Type: StardewValley.MovieConcession
// Assembly: Stardew Valley, Version=1.6.15.24356, Culture=neutral, PublicKeyToken=null
// MVID: AA1F513A-94F0-4EF6-A35F-2D5B4A3721BF
// Assembly location: D:\SteamLibrary\steamapps\common\Stardew Valley\Stardew Valley.dll
// XML documentation location: D:\SteamLibrary\steamapps\common\Stardew Valley\Stardew Valley.xml

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using StardewValley.GameData.Movies;
using StardewValley.ItemTypeDefinitions;
using StardewValley.TokenizableStrings;
using System.Collections.Generic;
using System.Linq;

#nullable disable
namespace StardewValley;

public class MovieConcession : ISalable, IHaveItemTypeId
{
  /// <summary>The underlying movie concession data.</summary>
  private readonly ConcessionItemData Data;

  /// <inheritdoc />
  public string TypeDefinitionId => "(Salable)";

  /// <inheritdoc />
  public string QualifiedItemId => $"{this.TypeDefinitionId}MovieConcession.{this.Id}";

  public string Id => this.Data.Id;

  /// <inheritdoc />
  public string Name => this.Data.Name;

  /// <inheritdoc />
  public string DisplayName => TokenParser.ParseText(this.Data.DisplayName);

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

  public List<string> Tags { get; }

  public MovieConcession(ConcessionItemData data)
  {
    this.Data = data;
    List<string> itemTags = data.ItemTags;
    this.Tags = itemTags != null ? itemTags.ToList<string>() : (List<string>) null;
  }

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
    if (drawShadow)
      spriteBatch.Draw(Game1.shadowTexture, location + new Vector2(32f, 48f), new Rectangle?(Game1.shadowTexture.Bounds), color * 0.5f, 0.0f, new Vector2((float) Game1.shadowTexture.Bounds.Center.X, (float) Game1.shadowTexture.Bounds.Center.Y), 3f, SpriteEffects.None, layerDepth - 0.0001f);
    spriteBatch.Draw(this.GetTexture(), location + new Vector2((float) (int) (32.0 * (double) scaleSize), (float) (int) (32.0 * (double) scaleSize)), new Rectangle?(Game1.getSourceRectForStandardTileSheet(this.GetTexture(), this.GetSpriteIndex(), 16 /*0x10*/, 16 /*0x10*/)), color * transparency, 0.0f, new Vector2(8f, 8f) * scaleSize, 4f * scaleSize, SpriteEffects.None, layerDepth);
  }

  public Texture2D GetTexture()
  {
    return !(this.Data.Texture == "LooseSprites\\Concessions") ? Game1.content.Load<Texture2D>(this.Data.Texture) : Game1.concessionsSpriteSheet;
  }

  public int GetSpriteIndex() => this.Data.SpriteIndex;

  public bool ShouldDrawIcon() => true;

  public string getDescription()
  {
    return Game1.parseText(TokenParser.ParseText(this.Data.Description), Game1.smallFont, 320);
  }

  public int maximumStackSize() => 1;

  public int addToStack(Item stack) => 1;

  public bool canStackWith(ISalable other) => false;

  /// <inheritdoc />
  public int sellToStorePrice(long specificPlayerID = -1) => -1;

  /// <inheritdoc />
  public int salePrice(bool ignoreProfitMargins = false) => this.Data.Price;

  /// <inheritdoc />
  public bool appliesProfitMargins() => false;

  /// <inheritdoc />
  public bool actionWhenPurchased(string shopId) => true;

  public bool CanBuyItem(Farmer farmer) => true;

  public bool IsInfiniteStock() => true;

  public ISalable GetSalableInstance() => (ISalable) this;

  /// <inheritdoc />
  public void FixStackSize()
  {
  }

  /// <inheritdoc />
  public void FixQuality()
  {
  }

  /// <inheritdoc />
  public string GetItemTypeId() => this.TypeDefinitionId;
}
