// Decompiled with JetBrains decompiler
// Type: StardewValley.Objects.Hat
// Assembly: Stardew Valley, Version=1.6.15.24356, Culture=neutral, PublicKeyToken=null
// MVID: AA1F513A-94F0-4EF6-A35F-2D5B4A3721BF
// Assembly location: D:\SteamLibrary\steamapps\common\Stardew Valley\Stardew Valley.dll
// XML documentation location: D:\SteamLibrary\steamapps\common\Stardew Valley\Stardew Valley.xml

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Netcode;
using StardewValley.Enchantments;
using StardewValley.ItemTypeDefinitions;
using System;
using System.Collections.Generic;
using System.Xml.Serialization;

#nullable disable
namespace StardewValley.Objects;

public class Hat : Item
{
  public const int widthOfTileSheetSquare = 20;
  public const int heightOfTileSheetSquare = 20;
  /// <summary>The index in <c>Data/Hats</c> for the internal name field.</summary>
  public const int data_index_internalName = 0;
  /// <summary>The index in <c>Data/Hats</c> for the description field.</summary>
  public const int data_index_description = 1;
  /// <summary>The index in <c>Data/Hats</c> for the 'show full hair' field.</summary>
  public const int data_index_showFullHair = 2;
  /// <summary>The index in <c>Data/Hats</c> for the ignore-hair-offset field.</summary>
  public const int data_index_ignoreHairOffset = 3;
  /// <summary>The index in <c>Data/Hats</c> for the special tags field.</summary>
  public const int data_index_tags = 4;
  /// <summary>The index in <c>Data/Hats</c> for the display name field.</summary>
  public const int data_index_displayName = 5;
  /// <summary>The index in <c>Data/Hats</c> for the texture field.</summary>
  public const int data_index_texture = 7;
  /// <summary>Obsolete. This is only kept to preserve data from old save files. Use <see cref="P:StardewValley.Item.ItemId" /> instead.</summary>
  [XmlElement("which")]
  public int? obsolete_which;
  [XmlElement("skipHairDraw")]
  public bool skipHairDraw;
  [XmlElement("ignoreHairstyleOffset")]
  public readonly NetBool ignoreHairstyleOffset = new NetBool();
  [XmlElement("hairDrawType")]
  public readonly NetInt hairDrawType = new NetInt();
  [XmlElement("isPrismatic")]
  public readonly NetBool isPrismatic = new NetBool(false);
  [XmlIgnore]
  protected int _isMask = -1;
  [XmlElement("enchantments")]
  public List<BaseEnchantment> enchantments = new List<BaseEnchantment>();
  [XmlElement("previousEnchantments")]
  public List<string> previousEnchantments = new List<string>();
  [XmlIgnore]
  public string displayName;
  [XmlIgnore]
  public string description;

  /// <inheritdoc />
  public override string TypeDefinitionId { get; } = "(H)";

  [XmlIgnore]
  public bool isMask
  {
    get
    {
      if (this._isMask == -1)
      {
        this._isMask = !this.Name.Contains("Mask") ? 0 : 1;
        if (this.hairDrawType.Value == 2)
          this._isMask = 0;
      }
      return this._isMask == 1;
    }
  }

  /// <inheritdoc />
  protected override void MigrateLegacyItemId()
  {
    ref int? local = ref this.obsolete_which;
    this.ItemId = (local.HasValue ? local.GetValueOrDefault().ToString() : (string) null) ?? "0";
    this.obsolete_which = new int?();
  }

  public Hat()
  {
  }

  public Hat(string itemId)
  {
    itemId = this.ValidateUnqualifiedItemId(itemId);
    this.ItemId = itemId;
    this.load(this.ItemId);
  }

  /// <inheritdoc />
  protected override void initNetFields()
  {
    base.initNetFields();
    this.NetFields.AddField((INetSerializable) this.ignoreHairstyleOffset, "ignoreHairstyleOffset").AddField((INetSerializable) this.hairDrawType, "hairDrawType").AddField((INetSerializable) this.isPrismatic, "isPrismatic");
    this.itemId.fieldChangeVisibleEvent += (FieldChange<NetString, string>) ((_param1, _param2, _param3) => this.load(this.itemId.Value));
  }

  public void load(string id)
  {
    Dictionary<string, string> dictionary = DataLoader.Hats(Game1.content);
    string str1;
    if (!dictionary.TryGetValue(id, out str1))
    {
      id = "0";
      str1 = dictionary[id];
    }
    string[] array = str1.Split('/');
    this.Name = ArgUtility.Get(array, 0, allowBlank: false) ?? ItemRegistry.GetDataOrErrorItem("(H)" + id).InternalName;
    string str2 = array[2];
    if (str2 == "hide")
      this.hairDrawType.Set(2);
    else if (Convert.ToBoolean(str2))
      this.hairDrawType.Set(0);
    else
      this.hairDrawType.Set(1);
    if (this.skipHairDraw)
    {
      this.skipHairDraw = false;
      this.hairDrawType.Set(0);
    }
    foreach (string str3 in ArgUtility.SplitBySpace(ArgUtility.Get(array, 4)))
    {
      if (str3 == "Prismatic")
        this.isPrismatic.Value = true;
    }
    this.ignoreHairstyleOffset.Value = Convert.ToBoolean(array[3]);
    this.Category = -95;
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
    scaleSize *= 0.75f;
    ParsedItemData dataOrErrorItem = ItemRegistry.GetDataOrErrorItem(this.QualifiedItemId);
    int spriteIndex = dataOrErrorItem.SpriteIndex;
    Texture2D texture = dataOrErrorItem.GetTexture();
    Rectangle rectangle = new Rectangle(spriteIndex * 20 % texture.Width, spriteIndex * 20 / texture.Width * 20 * 4, 20, 20);
    if (dataOrErrorItem.IsErrorItem)
      rectangle = dataOrErrorItem.GetSourceRect();
    spriteBatch.Draw(texture, location + new Vector2(32f, 32f), new Rectangle?(rectangle), this.isPrismatic.Value ? Utility.GetPrismaticColor() * transparency : color * transparency, 0.0f, new Vector2(10f, 10f), 4f * scaleSize, SpriteEffects.None, layerDepth);
    this.DrawMenuIcons(spriteBatch, location, scaleSize, transparency, layerDepth, drawStackNumber, color);
  }

  public void draw(
    SpriteBatch spriteBatch,
    Vector2 location,
    float scaleSize,
    float transparency,
    float layerDepth,
    int direction,
    bool useAnimalTexture = false)
  {
    ParsedItemData dataOrErrorItem = ItemRegistry.GetDataOrErrorItem(this.QualifiedItemId);
    int spriteIndex = dataOrErrorItem.SpriteIndex;
    Texture2D texture;
    if (useAnimalTexture)
    {
      string textureName = dataOrErrorItem.GetTextureName();
      if (Game1.content.DoesAssetExist<Texture2D>(textureName + "_animals"))
        textureName += "_animals";
      texture = Game1.content.Load<Texture2D>(textureName);
    }
    else
      texture = dataOrErrorItem.GetTexture();
    switch (direction)
    {
      case 0:
        direction = 3;
        break;
      case 2:
        direction = 0;
        break;
      case 3:
        direction = 2;
        break;
    }
    Rectangle rectangle = !dataOrErrorItem.IsErrorItem ? new Rectangle(spriteIndex * 20 % texture.Width, spriteIndex * 20 / texture.Width * 20 * 4 + direction * 20, 20, 20) : dataOrErrorItem.GetSourceRect();
    spriteBatch.Draw(texture, location + new Vector2(10f, 10f), new Rectangle?(rectangle), this.isPrismatic.Value ? Utility.GetPrismaticColor() * transparency : Color.White * transparency, 0.0f, new Vector2(3f, 3f), 3f * scaleSize, SpriteEffects.None, layerDepth);
  }

  public override string getDescription()
  {
    if (this.description == null)
      this.loadDisplayFields();
    return Game1.parseText(this.description, Game1.smallFont, this.getDescriptionWidth());
  }

  public override int maximumStackSize() => 1;

  public override bool isPlaceable() => false;

  /// <inheritdoc />
  [XmlIgnore]
  public override string DisplayName
  {
    get
    {
      if (this.displayName == null)
        this.loadDisplayFields();
      return this.displayName;
    }
  }

  /// <inheritdoc />
  protected override Item GetOneNew() => (Item) new Hat(this.ItemId);

  private bool loadDisplayFields()
  {
    ParsedItemData dataOrErrorItem = ItemRegistry.GetDataOrErrorItem(this.QualifiedItemId);
    if (this.Name != null && this.Name != "Error Item" && dataOrErrorItem.IsErrorItem)
    {
      foreach (KeyValuePair<string, string> hat in DataLoader.Hats(Game1.content))
      {
        if (hat.Value.Split('/')[0] == this.Name)
        {
          dataOrErrorItem = ItemRegistry.GetDataOrErrorItem(this.TypeDefinitionId + hat.Key);
          break;
        }
      }
    }
    this.displayName = dataOrErrorItem.DisplayName;
    this.description = dataOrErrorItem.Description;
    return true;
  }

  public enum HairDrawType
  {
    DrawFullHair,
    DrawObscuredHair,
    HideHair,
  }
}
