// Decompiled with JetBrains decompiler
// Type: StardewValley.Objects.Clothing
// Assembly: Stardew Valley, Version=1.6.15.24356, Culture=neutral, PublicKeyToken=null
// MVID: AA1F513A-94F0-4EF6-A35F-2D5B4A3721BF
// Assembly location: D:\SteamLibrary\steamapps\common\Stardew Valley\Stardew Valley.dll
// XML documentation location: D:\SteamLibrary\steamapps\common\Stardew Valley\Stardew Valley.xml

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Netcode;
using StardewValley.GameData.Pants;
using StardewValley.GameData.Shirts;
using StardewValley.ItemTypeDefinitions;
using StardewValley.TokenizableStrings;
using System;
using System.Xml.Serialization;

#nullable disable
namespace StardewValley.Objects;

public class Clothing : Item
{
  public const int SHIRT_SHEET_WIDTH = 128 /*0x80*/;
  public const string DefaultShirtSheetName = "Characters\\Farmer\\shirts";
  public const string DefaultPantsSheetName = "Characters\\Farmer\\pants";
  public const int MinShirtId = 1000;
  [XmlElement("price")]
  public readonly NetInt price = new NetInt();
  [XmlElement("indexInTileSheet")]
  public readonly NetInt indexInTileSheet = new NetInt();
  /// <summary>Obsolete. This is only kept to preserve data from old save files. Use <see cref="F:StardewValley.Objects.Clothing.indexInTileSheet" /> instead.</summary>
  [XmlElement("indexInTileSheetFemale")]
  public int? obsolete_indexInTileSheetFemale;
  [XmlIgnore]
  public string description;
  [XmlIgnore]
  public string displayName;
  [XmlElement("clothesType")]
  public readonly NetEnum<Clothing.ClothesType> clothesType = new NetEnum<Clothing.ClothesType>();
  [XmlElement("dyeable")]
  public readonly NetBool dyeable = new NetBool(false);
  [XmlElement("clothesColor")]
  public readonly NetColor clothesColor = new NetColor(new Color((int) byte.MaxValue, (int) byte.MaxValue, (int) byte.MaxValue));
  [XmlElement("isPrismatic")]
  public readonly NetBool isPrismatic = new NetBool(false);
  [XmlIgnore]
  protected bool _loadedData;

  /// <inheritdoc />
  public override string TypeDefinitionId
  {
    get => this.clothesType.Value != Clothing.ClothesType.PANTS ? "(S)" : "(P)";
  }

  public int Price
  {
    set => this.price.Value = value;
    get => this.price.Value;
  }

  public Clothing() => this.Category = -100;

  /// <inheritdoc />
  protected override void initNetFields()
  {
    base.initNetFields();
    this.NetFields.AddField((INetSerializable) this.price, "price").AddField((INetSerializable) this.indexInTileSheet, "indexInTileSheet").AddField((INetSerializable) this.clothesType, "clothesType").AddField((INetSerializable) this.dyeable, "dyeable").AddField((INetSerializable) this.clothesColor, "clothesColor").AddField((INetSerializable) this.isPrismatic, "isPrismatic");
  }

  public Clothing(string itemId)
    : this()
  {
    itemId = this.ValidateUnqualifiedItemId(itemId);
    this.Name = nameof (Clothing);
    this.Category = -100;
    this.ItemId = itemId;
    this.LoadData(true);
  }

  /// <summary>Apply the data from <see cref="F:StardewValley.Game1.pantsData" /> or <see cref="F:StardewValley.Game1.shirtData" /> to this item instance.</summary>
  /// <param name="applyColor">Whether to parse the tint color in field 6; else the tint is set to neutral white.</param>
  /// <param name="forceReload">Whether to reapply the latest data, even if this item was previously initialized.</param>
  public virtual void LoadData(bool applyColor = false, bool forceReload = false)
  {
    if (this._loadedData && !forceReload)
      return;
    this.Category = -100;
    PantsData pantsData;
    if (Game1.pantsData.TryGetValue(this.ItemId, out pantsData))
    {
      this.Name = pantsData.Name;
      this.price.Value = pantsData.Price;
      this.indexInTileSheet.Value = pantsData.SpriteIndex;
      this.dyeable.Value = pantsData.CanBeDyed;
      if (applyColor)
        this.clothesColor.Value = Utility.StringToColor(pantsData.DefaultColor) ?? Color.White;
      else if (forceReload)
        this.clothesColor.Value = Color.White;
      this.displayName = TokenParser.ParseText(pantsData.DisplayName);
      this.description = TokenParser.ParseText(pantsData.Description);
      this.clothesType.Value = Clothing.ClothesType.PANTS;
      this.isPrismatic.Value = pantsData.IsPrismatic;
    }
    else
    {
      ShirtData shirtData;
      if (Game1.shirtData.TryGetValue(this.ItemId, out shirtData))
      {
        this.Name = shirtData.Name;
        this.price.Value = shirtData.Price;
        this.indexInTileSheet.Value = shirtData.SpriteIndex;
        this.dyeable.Value = shirtData.CanBeDyed;
        if (applyColor)
          this.clothesColor.Value = Utility.StringToColor(shirtData.DefaultColor) ?? Color.White;
        else if (forceReload)
          this.clothesColor.Value = Color.White;
        this.displayName = TokenParser.ParseText(shirtData.DisplayName);
        this.description = TokenParser.ParseText(shirtData.Description);
        this.clothesType.Value = Clothing.ClothesType.SHIRT;
        this.isPrismatic.Value = shirtData.IsPrismatic;
      }
      else
      {
        ParsedItemData dataOrErrorItem = ItemRegistry.GetDataOrErrorItem(this.QualifiedItemId);
        this.displayName = dataOrErrorItem.DisplayName;
        this.description = dataOrErrorItem.Description;
      }
    }
    if (this.dyeable.Value)
      this.description = this.description + Environment.NewLine + Environment.NewLine + Game1.content.LoadString("Strings\\UI:Clothes_Dyeable");
    this._loadedData = true;
  }

  /// <inheritdoc />
  public override string getCategoryName() => StardewValley.Object.GetCategoryDisplayName(-100);

  /// <inheritdoc />
  public override int salePrice(bool ignoreProfitMargins = false) => this.price.Value;

  public virtual void Dye(Color color, float strength = 0.5f)
  {
    if (!this.dyeable.Value)
      return;
    Color color1 = this.clothesColor.Value;
    this.clothesColor.Value = new Color(Utility.MoveTowards((float) color1.R / (float) byte.MaxValue, (float) color.R / (float) byte.MaxValue, strength), Utility.MoveTowards((float) color1.G / (float) byte.MaxValue, (float) color.G / (float) byte.MaxValue, strength), Utility.MoveTowards((float) color1.B / (float) byte.MaxValue, (float) color.B / (float) byte.MaxValue, strength), Utility.MoveTowards((float) color1.A / (float) byte.MaxValue, (float) color.A / (float) byte.MaxValue, strength));
  }

  public override void drawInMenu(
    SpriteBatch spriteBatch,
    Vector2 location,
    float scaleSize,
    float transparency,
    float layerDepth,
    StackDrawType drawStackNumber,
    Color color,
    bool drawShadow)
  {
    this.AdjustMenuDrawForRecipes(ref transparency, ref scaleSize);
    Color prismaticColor = this.clothesColor.Value;
    ParsedItemData dataOrErrorItem = ItemRegistry.GetDataOrErrorItem(this.QualifiedItemId);
    Texture2D texture = dataOrErrorItem.GetTexture();
    Rectangle sourceRect = dataOrErrorItem.GetSourceRect();
    Rectangle rectangle = Rectangle.Empty;
    if (!dataOrErrorItem.IsErrorItem)
    {
      if (this.clothesType.Value == Clothing.ClothesType.SHIRT)
        rectangle = new Rectangle(sourceRect.X + texture.Width / 2, sourceRect.Y, sourceRect.Width, sourceRect.Height);
      if (this.isPrismatic.Value)
        prismaticColor = Utility.GetPrismaticColor();
    }
    switch (this.clothesType.Value)
    {
      case Clothing.ClothesType.SHIRT:
        float num = 1E-07f;
        if ((double) layerDepth >= 1.0 - (double) num)
          layerDepth = 1f - num;
        Vector2 origin = new Vector2(4f, 4f);
        if (dataOrErrorItem.IsErrorItem)
        {
          origin.X = (float) (sourceRect.Width / 2);
          origin.Y = (float) (sourceRect.Height / 2);
        }
        spriteBatch.Draw(texture, location + new Vector2(32f, 32f), new Rectangle?(sourceRect), color * transparency, 0.0f, origin, scaleSize * 4f, SpriteEffects.None, layerDepth);
        spriteBatch.Draw(texture, location + new Vector2(32f, 32f), new Rectangle?(rectangle), Utility.MultiplyColor(prismaticColor, color) * transparency, 0.0f, origin, scaleSize * 4f, SpriteEffects.None, layerDepth + num);
        break;
      case Clothing.ClothesType.PANTS:
        spriteBatch.Draw(texture, location + new Vector2(32f, 32f), new Rectangle?(sourceRect), Utility.MultiplyColor(prismaticColor, color) * transparency, 0.0f, new Vector2(8f, 8f), scaleSize * 4f, SpriteEffects.None, layerDepth);
        break;
    }
    this.DrawMenuIcons(spriteBatch, location, scaleSize, transparency, layerDepth, drawStackNumber, color);
  }

  public override int maximumStackSize() => 1;

  public override string getDescription()
  {
    if (!this._loadedData)
      this.LoadData();
    return Game1.parseText(this.description, Game1.smallFont, this.getDescriptionWidth());
  }

  public override bool isPlaceable() => false;

  /// <inheritdoc />
  [XmlIgnore]
  public override string DisplayName
  {
    get
    {
      if (!this._loadedData)
        this.LoadData();
      return this.displayName;
    }
  }

  /// <inheritdoc />
  protected override Item GetOneNew() => (Item) new Clothing(this.ItemId);

  /// <inheritdoc />
  protected override void GetOneCopyFrom(Item source)
  {
    base.GetOneCopyFrom(source);
    if (!(source is Clothing clothing))
      return;
    this.clothesColor.Value = clothing.clothesColor.Value;
  }

  public enum ClothesType
  {
    SHIRT,
    PANTS,
  }
}
