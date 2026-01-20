// Decompiled with JetBrains decompiler
// Type: StardewValley.Objects.Boots
// Assembly: Stardew Valley, Version=1.6.15.24356, Culture=neutral, PublicKeyToken=null
// MVID: AA1F513A-94F0-4EF6-A35F-2D5B4A3721BF
// Assembly location: D:\SteamLibrary\steamapps\common\Stardew Valley\Stardew Valley.dll
// XML documentation location: D:\SteamLibrary\steamapps\common\Stardew Valley\Stardew Valley.xml

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Netcode;
using StardewValley.Buffs;
using StardewValley.ItemTypeDefinitions;
using System;
using System.Text;
using System.Xml.Serialization;

#nullable disable
namespace StardewValley.Objects;

public class Boots : Item
{
  [XmlElement("defenseBonus")]
  public readonly NetInt defenseBonus = new NetInt();
  [XmlElement("immunityBonus")]
  public readonly NetInt immunityBonus = new NetInt();
  [XmlElement("indexInTileSheet")]
  public readonly NetInt indexInTileSheet = new NetInt();
  [XmlElement("price")]
  public readonly NetInt price = new NetInt();
  [XmlElement("indexInColorSheet")]
  public readonly NetInt indexInColorSheet = new NetInt();
  [XmlElement("appliedBootSheetIndex")]
  public readonly NetString appliedBootSheetIndex = new NetString();
  /// <summary>The cached value for <see cref="P:StardewValley.Objects.Boots.DisplayName" />.</summary>
  [XmlIgnore]
  public string displayName;
  [XmlIgnore]
  public string description;

  /// <inheritdoc />
  public override string TypeDefinitionId { get; } = "(B)";

  public Boots() => this.Category = -97;

  public Boots(string itemId)
    : this()
  {
    itemId = this.ValidateUnqualifiedItemId(itemId);
    this.ItemId = itemId;
    this.reloadData();
    this.Category = -97;
  }

  /// <inheritdoc />
  protected override void MigrateLegacyItemId()
  {
    this.ItemId = this.indexInTileSheet.Value.ToString();
  }

  /// <inheritdoc />
  protected override void initNetFields()
  {
    base.initNetFields();
    this.NetFields.AddField((INetSerializable) this.defenseBonus, "defenseBonus").AddField((INetSerializable) this.immunityBonus, "immunityBonus").AddField((INetSerializable) this.indexInTileSheet, "indexInTileSheet").AddField((INetSerializable) this.price, "price").AddField((INetSerializable) this.indexInColorSheet, "indexInColorSheet").AddField((INetSerializable) this.appliedBootSheetIndex, "appliedBootSheetIndex");
  }

  public virtual void reloadData()
  {
    ParsedItemData dataOrErrorItem = ItemRegistry.GetDataOrErrorItem(this.QualifiedItemId);
    string[] array = DataLoader.Boots(Game1.content)[this.ItemId].Split('/');
    this.Name = ArgUtility.Get(array, 0, allowBlank: false) ?? dataOrErrorItem.InternalName;
    this.price.Value = Convert.ToInt32(array[2]);
    this.defenseBonus.Value = Convert.ToInt32(array[3]);
    this.immunityBonus.Value = Convert.ToInt32(array[4]);
    this.indexInColorSheet.Value = Convert.ToInt32(array[5]);
    this.indexInTileSheet.Value = dataOrErrorItem.SpriteIndex;
  }

  public void applyStats(Boots applied_boots)
  {
    this.reloadData();
    if (this.defenseBonus.Value == applied_boots.defenseBonus.Value && this.immunityBonus.Value == applied_boots.immunityBonus.Value)
      this.appliedBootSheetIndex.Value = (string) null;
    else
      this.appliedBootSheetIndex.Value = applied_boots.getStatsIndex();
    this.defenseBonus.Value = applied_boots.defenseBonus.Value;
    this.immunityBonus.Value = applied_boots.immunityBonus.Value;
    this.price.Value = applied_boots.price.Value;
    this.loadDisplayFields();
  }

  public virtual string getStatsIndex() => this.appliedBootSheetIndex.Value ?? this.ItemId;

  /// <inheritdoc />
  public override int salePrice(bool ignoreProfitMargins = false)
  {
    return this.defenseBonus.Value * 100 + this.immunityBonus.Value * 100;
  }

  /// <inheritdoc />
  public override void onEquip(Farmer who)
  {
    base.onEquip(who);
    who.changeShoeColor(this.GetBootsColorString());
  }

  /// <inheritdoc />
  public override void onUnequip(Farmer who)
  {
    base.onUnequip(who);
    who.changeShoeColor("12");
  }

  public override void AddEquipmentEffects(BuffEffects effects)
  {
    base.AddEquipmentEffects(effects);
    effects.Defense.Value += (float) this.defenseBonus.Value;
    effects.Immunity.Value += (float) this.immunityBonus.Value;
  }

  public string GetBootsColorString()
  {
    string str;
    if (DataLoader.Boots(Game1.content).TryGetValue(this.ItemId, out str))
    {
      string[] strArray = str.Split('/');
      if (strArray.Length > 7 && strArray[7] != "")
        return $"{strArray[7]}:{this.indexInColorSheet.Value.ToString()}";
    }
    return this.indexInColorSheet.Value.ToString();
  }

  public int getNumberOfDescriptionCategories()
  {
    return this.immunityBonus.Value > 0 && this.defenseBonus.Value > 0 ? 2 : 1;
  }

  public override void drawTooltip(
    SpriteBatch spriteBatch,
    ref int x,
    ref int y,
    SpriteFont font,
    float alpha,
    StringBuilder overrideText)
  {
    Utility.drawTextWithShadow(spriteBatch, Game1.parseText(this.description, Game1.smallFont, this.getDescriptionWidth()), font, new Vector2((float) (x + 16 /*0x10*/), (float) (y + 16 /*0x10*/ + 4)), Game1.textColor);
    y += (int) font.MeasureString(Game1.parseText(this.description, Game1.smallFont, this.getDescriptionWidth())).Y;
    if (this.defenseBonus.Value > 0)
    {
      Utility.drawWithShadow(spriteBatch, Game1.mouseCursors, new Vector2((float) (x + 16 /*0x10*/ + 4), (float) (y + 16 /*0x10*/ + 4)), new Rectangle(110, 428, 10, 10), Color.White, 0.0f, Vector2.Zero, 4f, layerDepth: 1f);
      Utility.drawTextWithShadow(spriteBatch, Game1.content.LoadString("Strings\\UI:ItemHover_DefenseBonus", (object) this.defenseBonus), font, new Vector2((float) (x + 16 /*0x10*/ + 52), (float) (y + 16 /*0x10*/ + 12)), Game1.textColor * 0.9f * alpha);
      y += (int) Math.Max(font.MeasureString("TT").Y, 48f);
    }
    if (this.immunityBonus.Value <= 0)
      return;
    Utility.drawWithShadow(spriteBatch, Game1.mouseCursors, new Vector2((float) (x + 16 /*0x10*/ + 4), (float) (y + 16 /*0x10*/ + 4)), new Rectangle(150, 428, 10, 10), Color.White, 0.0f, Vector2.Zero, 4f, layerDepth: 1f);
    Utility.drawTextWithShadow(spriteBatch, Game1.content.LoadString("Strings\\UI:ItemHover_ImmunityBonus", (object) this.immunityBonus), font, new Vector2((float) (x + 16 /*0x10*/ + 52), (float) (y + 16 /*0x10*/ + 12)), Game1.textColor * 0.9f * alpha);
    y += (int) Math.Max(font.MeasureString("TT").Y, 48f);
  }

  public override Point getExtraSpaceNeededForTooltipSpecialIcons(
    SpriteFont font,
    int minWidth,
    int horizontalBuffer,
    int startingHeight,
    StringBuilder descriptionText,
    string boldTitleText,
    int moneyAmountToDisplayAtBottom)
  {
    int sub1 = 9999;
    Point tooltipSpecialIcons = new Point(0, startingHeight);
    tooltipSpecialIcons.Y -= (int) font.MeasureString(descriptionText).Y;
    tooltipSpecialIcons.Y += (int) ((double) (this.getNumberOfDescriptionCategories() * 4 * 12) + (double) font.MeasureString(Game1.parseText(this.description, Game1.smallFont, this.getDescriptionWidth())).Y);
    tooltipSpecialIcons.X = (int) Math.Max((float) minWidth, Math.Max(font.MeasureString(Game1.content.LoadString("Strings\\UI:ItemHover_DefenseBonus", (object) sub1)).X + (float) horizontalBuffer, font.MeasureString(Game1.content.LoadString("Strings\\UI:ItemHover_ImmunityBonus", (object) sub1)).X + (float) horizontalBuffer));
    return tooltipSpecialIcons;
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
    ParsedItemData dataOrErrorItem = ItemRegistry.GetDataOrErrorItem(this.QualifiedItemId);
    spriteBatch.Draw(dataOrErrorItem.GetTexture(), location + new Vector2(32f, 32f) * scaleSize, new Rectangle?(dataOrErrorItem.GetSourceRect()), color * transparency, 0.0f, new Vector2(8f, 8f) * scaleSize, scaleSize * 4f, SpriteEffects.None, layerDepth);
    this.DrawMenuIcons(spriteBatch, location, scaleSize, transparency, layerDepth, drawStackNumber, color);
  }

  public override int maximumStackSize() => 1;

  /// <inheritdoc />
  public override string getCategoryName() => StardewValley.Object.GetCategoryDisplayName(-97);

  public override string getDescription()
  {
    if (this.description == null)
      this.loadDisplayFields();
    return Game1.parseText(this.description + Environment.NewLine + Environment.NewLine + Game1.content.LoadString("Strings\\StringsFromCSFiles:Boots.cs.12500", (object) (this.immunityBonus.Value + this.defenseBonus.Value)), Game1.smallFont, this.getDescriptionWidth());
  }

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
  protected override Item GetOneNew() => (Item) new Boots(this.ItemId);

  /// <inheritdoc />
  protected override void GetOneCopyFrom(Item source)
  {
    base.GetOneCopyFrom(source);
    if (!(source is Boots boots))
      return;
    this.appliedBootSheetIndex.Value = boots.appliedBootSheetIndex.Value;
    this.indexInColorSheet.Value = boots.indexInColorSheet.Value;
    this.defenseBonus.Value = boots.defenseBonus.Value;
    this.immunityBonus.Value = boots.immunityBonus.Value;
    this.loadDisplayFields();
  }

  protected virtual bool loadDisplayFields()
  {
    string str;
    if (!DataLoader.Boots(Game1.content).TryGetValue(this.ItemId, out str))
      return false;
    string[] strArray = str.Split('/');
    this.displayName = this.Name;
    if (strArray.Length > 6)
      this.displayName = strArray[6];
    if (this.appliedBootSheetIndex.Value != null)
      this.displayName = Game1.content.LoadString("Strings\\StringsFromCSFiles:CustomizedBootItemName", (object) this.DisplayName);
    this.description = strArray[1];
    return true;
  }
}
