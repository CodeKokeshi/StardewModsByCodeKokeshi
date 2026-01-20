// Decompiled with JetBrains decompiler
// Type: StardewValley.Item
// Assembly: Stardew Valley, Version=1.6.15.24356, Culture=neutral, PublicKeyToken=null
// MVID: AA1F513A-94F0-4EF6-A35F-2D5B4A3721BF
// Assembly location: D:\SteamLibrary\steamapps\common\Stardew Valley\Stardew Valley.dll
// XML documentation location: D:\SteamLibrary\steamapps\common\Stardew Valley\Stardew Valley.xml

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Netcode;
using Netcode.Validation;
using StardewValley.BellsAndWhistles;
using StardewValley.Buffs;
using StardewValley.Delegates;
using StardewValley.ItemTypeDefinitions;
using StardewValley.Mods;
using StardewValley.Network;
using StardewValley.Objects;
using StardewValley.Objects.Trinkets;
using StardewValley.Tools;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Serialization;

#nullable enable
namespace StardewValley;

[XmlInclude(typeof (Boots))]
[XmlInclude(typeof (Clothing))]
[XmlInclude(typeof (Hat))]
[XmlInclude(typeof (ModDataDictionary))]
[XmlInclude(typeof (Object))]
[XmlInclude(typeof (Ring))]
[XmlInclude(typeof (SpecialItem))]
[XmlInclude(typeof (Tool))]
[InstanceStatics]
[NotImplicitNetField]
public abstract class Item : 
  IComparable,
  INetObject<
  #nullable disable
  NetFields>,
  ISalable,
  IHaveItemTypeId,
  IHaveModData
{
  /// <summary>The internal name for an Error Item.</summary>
  public const string ErrorItemName = "Error Item";
  public bool isLostItem;
  private readonly NetInt specialVariable = new NetInt();
  [XmlElement("category")]
  public readonly NetInt category = new NetInt();
  [XmlElement("hasBeenInInventory")]
  public readonly NetBool hasbeenInInventory = new NetBool();
  private HashSet<string> _contextTags;
  protected bool _contextTagsDirty;
  /// <summary>Temporary data used for special purposes like fish catches. This isn't synchronized or saved.</summary>
  [XmlIgnore]
  public Dictionary<string, object> tempData;
  /// <summary>The mail flag to set for the player when it's added to their inventory for the first time.</summary>
  /// <remarks>This is a temporary value (e.g. for a freshly caught fish), and isn't persisted or synchronized.</remarks>
  [XmlIgnore]
  public string SetFlagOnPickup;
  [XmlElement("name")]
  public readonly NetString netName = new NetString("Error Item");
  /// <summary>The backing field for <see cref="P:StardewValley.Item.ParentSheetIndex" />.</summary>
  [XmlElement("parentSheetIndex")]
  public readonly NetInt parentSheetIndex = new NetInt();
  /// <summary>The backing field for <see cref="P:StardewValley.Item.ItemId" />.</summary>
  [XmlElement("itemId")]
  public NetString itemId = new NetString();
  /// <summary>The backing field for <see cref="P:StardewValley.Item.QualifiedItemId" />.</summary>
  [XmlIgnore]
  protected string _qualifiedItemId;
  public bool specialItem;
  [XmlElement("isRecipe")]
  public readonly NetBool isRecipe = new NetBool();
  [XmlElement("quality")]
  public readonly NetInt quality = new NetInt(0);
  [XmlElement("stack")]
  public readonly NetInt stack = new NetInt(1);

  /// <inheritdoc />
  [XmlIgnore]
  public ModDataDictionary modData { get; } = new ModDataDictionary();

  /// <inheritdoc />
  [XmlElement("modData")]
  public ModDataDictionary modDataForSerialization
  {
    get => this.modData.GetForSerialization();
    set => this.modData.SetFromSerialization(value);
  }

  public int SpecialVariable
  {
    get => this.specialVariable.Value;
    set => this.specialVariable.Set(value);
  }

  [XmlIgnore]
  public int Category
  {
    get => this.category.Value;
    set => this.category.Set(value);
  }

  [XmlIgnore]
  public bool HasBeenInInventory
  {
    get => this.hasbeenInInventory.Value;
    set => this.hasbeenInInventory.Set(value);
  }

  [XmlIgnore]
  public NetFields NetFields { get; } = new NetFields(nameof (Item));

  public bool IsInfiniteStock() => this.isLostItem;

  public void MarkContextTagsDirty() => this._contextTagsDirty = true;

  public HashSet<string> GetContextTags()
  {
    if (this._contextTags == null || this._contextTagsDirty)
      this._GenerateContextTags();
    return this._contextTags;
  }

  /// <summary>Get whether the item has the given context tag.</summary>
  /// <param name="tag">The tag to match. This can be negated by prefixing with <c>!</c> (like <c>!wine_item</c> to check if the tags <em>don't</em> contain <c>wine_item</c>).</param>
  public bool HasContextTag(string tag)
  {
    return ItemContextTagManager.DoesTagMatch(tag, this.GetContextTags());
  }

  protected void _GenerateContextTags()
  {
    this._contextTagsDirty = false;
    this._contextTags = new HashSet<string>((IEnumerable<string>) ItemContextTagManager.GetBaseContextTags(this.QualifiedItemId), (IEqualityComparer<string>) StringComparer.OrdinalIgnoreCase);
    this._PopulateContextTags(this._contextTags);
  }

  protected virtual void _PopulateContextTags(HashSet<string> tags)
  {
    switch (this.quality.Value)
    {
      case 0:
        tags.Add("quality_none");
        break;
      case 1:
        tags.Add("quality_silver");
        break;
      case 2:
        tags.Add("quality_gold");
        break;
      case 4:
        tags.Add("quality_iridium");
        break;
    }
  }

  protected Item()
  {
    this.initNetFields();
    this.parentSheetIndex.Value = -1;
  }

  /// <summary>The sprite index within the item's spritesheet texture to draw for this item.</summary>
  [XmlIgnore]
  public int ParentSheetIndex
  {
    get => this.parentSheetIndex.Value;
    set => this.parentSheetIndex.Value = value;
  }

  /// <inheritdoc />
  public abstract string TypeDefinitionId { get; }

  /// <summary>A key which uniquely identifies the item data among others of the same type.</summary>
  /// <remarks>
  ///   <para>This identifies the item data, not the instance itself. For example, Wine and Pufferfish have different item IDs, but two instances of Wine have the same ID. Note that flavored items use the same ID as their base item (like Blueberry Wine and Wine).</para>
  /// 
  ///   <para>This should only contain alphanumeric, underscore, and dot characters. This should be globally unique for custom items, but vanilla items may not be for legacy reasons; you can use <see cref="P:StardewValley.Item.QualifiedItemId" /> instead as the globally unique ID.</para>
  /// </remarks>
  [XmlIgnore]
  public string ItemId
  {
    get
    {
      if (this.itemId.Value == null)
        this.MigrateLegacyItemId();
      return this.itemId.Value;
    }
    set
    {
      this.itemId.Value = value;
      this._qualifiedItemId = (string) null;
    }
  }

  /// <summary>A globally unique item ID which includes the <see cref="P:StardewValley.Item.TypeDefinitionId" /> and <see cref="P:StardewValley.Item.ItemId" />.</summary>
  [XmlIgnore]
  public string QualifiedItemId
  {
    get
    {
      if (this._qualifiedItemId == null)
        this._qualifiedItemId = this.TypeDefinitionId + this.ItemId;
      return this._qualifiedItemId;
    }
  }

  /// <summary>Get whether the <see cref="F:StardewValley.Item.parentSheetIndex" /> field should be serialized.</summary>
  public virtual bool ShouldSerializeparentSheetIndex() => this.parentSheetIndex.Value != -1;

  /// <summary>Set the <see cref="P:StardewValley.Item.ItemId" /> to match the <see cref="P:StardewValley.Item.ParentSheetIndex" />, for vanilla items in saves created before Stardew Valley 1.6.</summary>
  protected virtual void MigrateLegacyItemId()
  {
    this.itemId.Value = this.ParentSheetIndex.ToString();
  }

  /// <summary>Initialize the collection of fields to sync in multiplayer.</summary>
  protected virtual void initNetFields()
  {
    this.NetFields.SetOwner((INetObject<NetFields>) this).AddField((INetSerializable) this.specialVariable, "specialVariable").AddField((INetSerializable) this.category, "category").AddField((INetSerializable) this.netName, "netName").AddField((INetSerializable) this.parentSheetIndex, "parentSheetIndex").AddField((INetSerializable) this.hasbeenInInventory, "hasbeenInInventory").AddField((INetSerializable) this.itemId, "itemId").AddField((INetSerializable) this.stack, "stack").AddField((INetSerializable) this.quality, "quality").AddField((INetSerializable) this.isRecipe, "isRecipe").AddField((INetSerializable) this.modData, "modData");
    this.itemId.fieldChangeVisibleEvent += (FieldChange<NetString, string>) ((_param1, _param2, _param3) =>
    {
      this._qualifiedItemId = (string) null;
      this.MarkContextTagsDirty();
    });
    this.netName.fieldChangeVisibleEvent += (FieldChange<NetString, string>) ((field, oldValue, newValue) =>
    {
      if (newValue != null)
        return;
      field.Value = "Error Item";
    });
    this.quality.fieldChangeVisibleEvent += (FieldChange<NetInt, int>) ((_param1, _param2, _param3) => this.MarkContextTagsDirty());
  }

  /// <summary>Reset the parent sheet index to match the underlying item data.</summary>
  /// <remarks>
  ///   <para>This should only be used for regular items (e.g. machines) before fully recalculating their offset.</para>
  ///   <para>Limitations:</para>
  ///   <list type="bullet">
  ///     <item><description>This doesn't change <see cref="F:StardewValley.Object.showNextIndex" />.</description></item>
  ///     <item><description>This doesn't reapply dynamic offset logic (e.g. seasonal items with season-specific sprite indexes).</description></item>
  ///   </list>
  /// </remarks>
  public void ResetParentSheetIndex()
  {
    this.ParentSheetIndex = ItemRegistry.GetDataOrErrorItem(this.QualifiedItemId).SpriteIndex;
  }

  /// <summary>Validate that the given item ID is unqualified. This logs a warning if the item ID is qualified, and removes the qualifier if it matches <see cref="P:StardewValley.Item.TypeDefinitionId" />.</summary>
  /// <param name="id">The item ID to validate and normalize.</param>
  protected string ValidateUnqualifiedItemId(string id)
  {
    if (ItemRegistry.IsQualifiedItemId(id))
    {
      string typeDefinitionId = this.TypeDefinitionId;
      if (id.StartsWith(typeDefinitionId))
      {
        Game1.log.Warn($"The {this.GetType().FullName} constructor was called with qualified item ID '{id}'. The '{typeDefinitionId}' prefix will be removed automatically.");
        id = id.Substring(typeDefinitionId.Length).TrimStart();
      }
      else
        Game1.log.Warn($"The {this.GetType().FullName} constructor was called with qualified item ID '{id}'. This will likely result in an error item.");
    }
    return id;
  }

  /// <inheritdoc />
  public string GetItemTypeId() => this.TypeDefinitionId;

  public virtual void drawTooltip(
    SpriteBatch spriteBatch,
    ref int x,
    ref int y,
    SpriteFont font,
    float alpha,
    StringBuilder overrideText)
  {
    if (overrideText == null || overrideText.Length == 0 || overrideText.Length == 1 && overrideText[0] == ' ')
      return;
    spriteBatch.DrawString(font, overrideText, new Vector2((float) (x + 16 /*0x10*/), (float) (y + 16 /*0x10*/ + 4)) + new Vector2(2f, 2f), Game1.textShadowColor * alpha);
    spriteBatch.DrawString(font, overrideText, new Vector2((float) (x + 16 /*0x10*/), (float) (y + 16 /*0x10*/ + 4)) + new Vector2(0.0f, 2f), Game1.textShadowColor * alpha);
    spriteBatch.DrawString(font, overrideText, new Vector2((float) (x + 16 /*0x10*/), (float) (y + 16 /*0x10*/ + 4)) + new Vector2(2f, 0.0f), Game1.textShadowColor * alpha);
    spriteBatch.DrawString(font, overrideText, new Vector2((float) (x + 16 /*0x10*/), (float) (y + 16 /*0x10*/ + 4)), Game1.textColor * 0.9f * alpha);
    y += (int) font.MeasureString(overrideText).Y + 4;
  }

  public virtual void ModifyItemBuffs(BuffEffects buffs)
  {
  }

  public virtual Point getExtraSpaceNeededForTooltipSpecialIcons(
    SpriteFont font,
    int minWidth,
    int horizontalBuffer,
    int startingHeight,
    StringBuilder descriptionText,
    string boldTitleText,
    int moneyAmountToDisplayAtBottom)
  {
    return Point.Zero;
  }

  public bool ShouldDrawIcon() => true;

  public abstract void drawInMenu(
    SpriteBatch spriteBatch,
    Vector2 location,
    float scaleSize,
    float transparency,
    float layerDepth,
    StackDrawType drawStackNumber,
    Color color,
    bool drawShadow);

  public void drawInMenu(
    SpriteBatch spriteBatch,
    Vector2 location,
    float scaleSize,
    float transparency,
    float layerDepth,
    StackDrawType drawStackNumber)
  {
    this.drawInMenu(spriteBatch, location, scaleSize, transparency, layerDepth, drawStackNumber, Color.White, true);
  }

  public void drawInMenu(
    SpriteBatch spriteBatch,
    Vector2 location,
    float scaleSize,
    float transparency,
    float layerDepth)
  {
    this.drawInMenu(spriteBatch, location, scaleSize, transparency, layerDepth, StackDrawType.Draw, Color.White, true);
  }

  public void drawInMenu(SpriteBatch spriteBatch, Vector2 location, float scaleSize)
  {
    this.drawInMenu(spriteBatch, location, scaleSize, 1f, 0.9f, StackDrawType.Draw, Color.White, true);
  }

  public abstract int maximumStackSize();

  public void AdjustMenuDrawForRecipes(ref float transparency, ref float scale)
  {
    if (!this.isRecipe.Value)
      return;
    transparency = 0.5f;
    scale *= 0.75f;
  }

  public virtual void DrawMenuIcons(
    SpriteBatch sb,
    Vector2 location,
    float scale_size,
    float transparency,
    float layer_depth,
    StackDrawType drawStackNumber,
    Color color)
  {
    int stack = this.Stack;
    bool flag = (drawStackNumber == StackDrawType.Draw && this.maximumStackSize() > 1 && stack > 1 || drawStackNumber == StackDrawType.Draw_OneInclusive) && (double) scale_size > 0.3 && stack != int.MaxValue;
    if (this.IsRecipe)
      flag = false;
    if (flag)
      Utility.drawTinyDigits(stack, sb, location + new Vector2((float) (64 /*0x40*/ - Utility.getWidthOfTinyDigitString(stack, 3f * scale_size)) + 3f * scale_size, (float) (64.0 - 18.0 * (double) scale_size + 1.0)), 3f * scale_size, Math.Min(1f, layer_depth + 1E-06f), color);
    if (drawStackNumber != StackDrawType.Hide && this.quality.Value > 0)
    {
      Rectangle rectangle = this.quality.Value < 4 ? new Rectangle(338 + (this.quality.Value - 1) * 8, 400, 8, 8) : new Rectangle(346, 392, 8, 8);
      Texture2D mouseCursors = Game1.mouseCursors;
      float num = this.quality.Value < 4 ? 0.0f : (float) ((Math.Cos((double) Game1.currentGameTime.TotalGameTime.Milliseconds * Math.PI / 512.0) + 1.0) * 0.05000000074505806);
      sb.Draw(mouseCursors, location + new Vector2(12f, 52f + num), new Rectangle?(rectangle), color * transparency, 0.0f, new Vector2(4f, 4f), (float) (3.0 * (double) scale_size * (1.0 + (double) num)), SpriteEffects.None, layer_depth);
    }
    else if (drawStackNumber != StackDrawType.Hide && this.Category == -102 && Game1.player.stats.Get(this.itemId.Value) > 0U)
      sb.Draw(Game1.mouseCursors_1_6, location + new Vector2(12f, 44f), new Rectangle?(new Rectangle(244, 271, 9, 11)), color * transparency, 0.0f, new Vector2(4f, 4f), (float) (3.0 * (double) scale_size * 1.0), SpriteEffects.None, layer_depth);
    this.DrawIconBar(sb, location, scale_size, transparency, layer_depth, drawStackNumber, color);
    if (!this.isRecipe.Value)
      return;
    sb.Draw(Game1.objectSpriteSheet, location + new Vector2(16f, 16f), new Rectangle?(Game1.getSourceRectForStandardTileSheet(Game1.objectSpriteSheet, 451, 16 /*0x10*/, 16 /*0x10*/)), color, 0.0f, Vector2.Zero, 3f, SpriteEffects.None, layer_depth + 0.0001f);
  }

  public virtual void DrawIconBar(
    SpriteBatch spriteBatch,
    Vector2 location,
    float scaleSize,
    float transparency,
    float layerDepth,
    StackDrawType drawStackNumber,
    Color color)
  {
  }

  /// <summary>Merge an item's stack into this one if they're stackable and this stack has room.</summary>
  /// <param name="otherStack">The other stack to merge into this one.</param>
  /// <returns>Returns the amount that couldn't be added to this stack (e.g. because it reached the maximum stack size or the provided item can't stack with this one).</returns>
  public virtual int addToStack(Item otherStack)
  {
    int num = this.maximumStackSize();
    if (num == 1)
      return otherStack.Stack;
    this.stack.Value += otherStack.Stack;
    if (this is Object object1 && otherStack is Object object2 && object1.IsSpawnedObject && !object2.IsSpawnedObject)
      object1.IsSpawnedObject = false;
    if (this.stack.Value <= num)
      return 0;
    int stack = this.stack.Value - num;
    this.stack.Value = num;
    return stack;
  }

  public abstract string getDescription();

  public abstract bool isPlaceable();

  /// <inheritdoc />
  public virtual int sellToStorePrice(long specificPlayerID = -1) => this.salePrice(false) / 2;

  /// <inheritdoc />
  public virtual int salePrice(bool ignoreProfitMargins = false) => -1;

  /// <inheritdoc />
  public virtual bool appliesProfitMargins() => false;

  /// <summary>Get whether the player can lose this item when they die.</summary>
  public virtual bool CanBeLostOnDeath()
  {
    return this.canBeTrashed() && !this.HasContextTag("prevent_loss_on_death");
  }

  /// <summary>Get whether this item can be trashed by players.</summary>
  public virtual bool canBeTrashed()
  {
    if (this.specialItem)
      return false;
    switch (this)
    {
      case MeleeWeapon meleeWeapon:
        return !meleeWeapon.isScythe();
      case FishingRod _:
      case Pan _:
      case Slingshot _:
        return true;
      default:
        return !(this is Tool);
    }
  }

  /// <inheritdoc />
  public virtual bool actionWhenPurchased(string shopId)
  {
    if (!this.isLostItem)
      return false;
    Game1.player.itemsLostLastDeath.Clear();
    this.isLostItem = false;
    Game1.player.recoveredItem = this;
    Game1.player.mailReceived.Remove("MarlonRecovery");
    Game1.addMailForTomorrow("MarlonRecovery");
    Game1.playSound("newArtifact");
    Game1.exitActiveMenu();
    bool flag = this.Stack > 1;
    Game1.DrawDialogue(Game1.getCharacterFromName("Marlon"), flag ? "Strings\\StringsFromCSFiles:ItemRecovery_Engaged_Stack" : "Strings\\StringsFromCSFiles:ItemRecovery_Engaged", (object) Lexicon.makePlural(this.DisplayName, !flag));
    return true;
  }

  /// <summary>Learn the recipe for this item.</summary>
  /// <param name="player">The player who should learn the recipe, or <c>null</c> for the local player.</param>
  /// <returns>Returns whether the recipe was added (i.e. it wasn't already known).</returns>
  /// <remarks>This adds the item to the current player's list of known recipes regardless of whether it exists in <c>Data/CraftingRecipes</c> or <c>Data/CookingRecipes</c>.</remarks>
  public bool LearnRecipe(Farmer player = null)
  {
    if (player == null)
      player = Game1.player;
    return (this.Category == -7 ? (NetDictionary<string, int, NetInt, SerializableDictionary<string, int>, NetStringDictionary<int, NetInt>>) player.cookingRecipes : (NetDictionary<string, int, NetInt, SerializableDictionary<string, int>, NetStringDictionary<int, NetInt>>) player.craftingRecipes).TryAdd(this.BaseName, 0);
  }

  public virtual bool CanBuyItem(Farmer who) => Game1.player.couldInventoryAcceptThisItem(this);

  public virtual bool canBeDropped() => true;

  /// <summary>Get whether this item can be sold through the shipping bin.</summary>
  public virtual bool canBeShipped() => false;

  /// <summary>Detach the item from its net sync parent when it's removed from an inventory/player/container. This avoids 'changing net field parent' warnings when it's added to a new parent.</summary>
  public virtual void onDetachedFromParent() => this.NetFields.Parent = (INetSerializable) null;

  /// <summary>Handle the item being equipped by the player (i.e. added to an equipment slot, or selected as the active tool).</summary>
  /// <param name="who">The player who equipped the item.</param>
  public virtual void onEquip(Farmer who)
  {
  }

  /// <summary>Handle the item being unequipped by the player (i.e. removed from an equipment slot, or deselected as the active tool).</summary>
  /// <param name="who">The player who unequipped the item.</param>
  public virtual void onUnequip(Farmer who)
  {
  }

  public virtual void actionWhenBeingHeld(Farmer who)
  {
  }

  public virtual void actionWhenStopBeingHeld(Farmer who)
  {
  }

  public int getRemainingStackSpace() => this.maximumStackSize() - this.Stack;

  /// <summary>Reduce the item's stack by the given amount.</summary>
  /// <param name="amount">The amount by which to reduce the item's <see cref="P:StardewValley.Item.Stack" /> size.</param>
  /// <returns>Returns null if the stack has been reduced to zero, else the item.</returns>
  /// <remarks>This allows handling items whose stack size is hardcoded (like weapons or tools), where simply decrementing the stack isn't enough to know if an item has been fully consumed.</remarks>
  public virtual 
  #nullable enable
  Item? ConsumeStack(int amount)
  {
    if (amount == 0)
      return this;
    if (this.Stack - amount <= 0)
      return (Item) null;
    this.Stack -= amount;
    return this;
  }

  public virtual int healthRecoveredOnConsumption() => 0;

  public virtual int staminaRecoveredOnConsumption() => 0;

  public virtual 
  #nullable disable
  string getHoverBoxText(Item hoveredItem) => (string) null;

  /// <summary>Get whether this item can be given to NPCs as a gift by default.</summary>
  /// <remarks>This doesn't prevent giving the item for non-gift reasons (e.g. quests), and setting this to true won't allow gifting where there's a specific exclusion (e.g. only Pierre will accept Pierre's Missing Stocklist).</remarks>
  public virtual bool canBeGivenAsGift() => false;

  public virtual void drawAttachments(SpriteBatch b, int x, int y)
  {
  }

  public virtual bool canBePlacedHere(
    GameLocation l,
    Vector2 tile,
    CollisionMask collisionMask = CollisionMask.All,
    bool showError = false)
  {
    return false;
  }

  public virtual int attachmentSlots() => 0;

  /// <summary>Get the translated display name for the item's category, or an empty string if there is none.</summary>
  public virtual string getCategoryName() => Object.GetCategoryDisplayName(this.Category);

  /// <summary>Get the display color for the item's category.</summary>
  public virtual Color getCategoryColor() => Object.GetCategoryColor(this.Category);

  public virtual bool canStackWith(ISalable other)
  {
    return other is Item obj && !(other.GetType() != this.GetType()) && (!(this is ColoredObject coloredObject1) || !(other is ColoredObject coloredObject2) || coloredObject1.color.Value.Equals(coloredObject2.color.Value)) && this.maximumStackSize() > 1 && other.maximumStackSize() > 1 && (!(this is Object object1) || !(other is Object object2) || !(object2.orderData.Value != object1.orderData.Value)) && this.quality.Value == obj.quality.Value && !(this.QualifiedItemId != obj.QualifiedItemId) && this.Name.Equals(other.Name);
  }

  public virtual string checkForSpecialItemHoldUpMeessage() => (string) null;

  /// <inheritdoc />
  public abstract string DisplayName { get; }

  /// <inheritdoc />
  [XmlIgnore]
  public virtual string Name
  {
    get => this.netName.Value;
    set => this.netName.Value = value;
  }

  /// <summary>If the <see cref="P:StardewValley.Item.Name" /> contains temporary dynamic changes like the " Recipe" suffix for a recipe, the item name without those changes.</summary>
  [XmlIgnore]
  public virtual string BaseName => this.Name;

  /// <summary>The quantity of items in this item stack.</summary>
  [XmlIgnore]
  public virtual int Stack
  {
    get => Math.Max(0, this.stack.Value);
    set
    {
      if (Game1.gameMode != (byte) 3)
        this.stack.Value = value;
      else
        this.stack.Value = Math.Min(Math.Max(0, value), value == int.MaxValue ? value : this.maximumStackSize());
    }
  }

  [XmlIgnore]
  public int Quality
  {
    get => this.quality.Value;
    set => this.quality.Value = value;
  }

  [XmlIgnore]
  public bool IsRecipe
  {
    get => this.isRecipe.Value;
    set => this.isRecipe.Value = value;
  }

  /// <summary>Get a new copy of this item, with its stack size reset to one.</summary>
  public Item getOne()
  {
    Item oneNew = this.GetOneNew();
    oneNew.GetOneCopyFrom(this);
    return oneNew;
  }

  /// <summary>Called from <see cref="M:StardewValley.Item.getOne" /> to create a minimal instance of the same item type.</summary>
  /// <remarks>
  ///   Items are cloned in two steps:
  ///   <list type="number">
  ///    <item><description><see cref="M:StardewValley.Item.GetOneNew" /> is called on the original item to create a new instance of the same type.</description></item>
  ///    <item><description><see cref="M:StardewValley.Item.GetOneCopyFrom(StardewValley.Item)" /> is called on the new item to copy its values from the original.</description></item>
  ///   </list>
  /// 
  ///   This separation is needed to let subclasses extend the base copy logic without needing to duplicate it, so <see cref="M:StardewValley.Item.GetOneNew" /> shouldn't copy any values beyond calling the constructor.
  /// </remarks>
  protected abstract Item GetOneNew();

  /// <summary>Called from <see cref="M:StardewValley.Item.getOne" /> to copy values from the original instance.</summary>
  /// <param name="source">The original item instance to copy from.</param>
  /// <inheritdoc cref="M:StardewValley.Item.GetOneNew" path="/remarks" />
  protected virtual void GetOneCopyFrom(Item source)
  {
    this.ItemId = source.ItemId;
    this.IsRecipe = source.isRecipe.Value;
    this.Quality = source.quality.Value;
    this.Stack = 1;
    this.HasBeenInInventory = source.HasBeenInInventory;
    this.SpecialVariable = source.SpecialVariable;
    Dictionary<string, object> tempData = source.tempData;
    // ISSUE: explicit non-virtual call
    if ((tempData != null ? (__nonvirtual (tempData.Count) > 0 ? 1 : 0) : 0) != 0)
    {
      foreach (KeyValuePair<string, object> keyValuePair in source.tempData)
        this.SetTempData<object>(keyValuePair.Key, keyValuePair.Value);
    }
    this.modData.Clear();
    foreach (string key in source.modData.Keys)
      this.modData[key] = source.modData[key];
  }

  /// <summary>Copy field values from another item into this instance.</summary>
  /// <param name="source">The original item instance to copy from.</param>
  public void CopyFieldsFrom(Item source) => this.GetOneCopyFrom(source);

  public ISalable GetSalableInstance() => (ISalable) this.getOne();

  /// <inheritdoc />
  public virtual int CompareTo(object other)
  {
    if (!(other is Item obj))
      return 0;
    if (obj.Category != this.Category)
      return obj.getCategorySortValue() - this.getCategorySortValue();
    string strA = this.Name == "" ? this.DisplayName : this.Name;
    string str = obj.Name == "" ? obj.DisplayName : obj.Name;
    if (str != strA)
    {
      if (!(this is Object object1) || !(obj is Object object2))
        return string.Compare(strA, obj.Name);
      if (object1.HasContextTag("use_reverse_name_for_sorting") || object1 is Trinket)
        strA = string.Join("", ((IEnumerable<string>) strA.Split(' ')).Reverse<string>());
      if (object2.HasContextTag("use_reverse_name_for_sorting") || object2 is Trinket)
        str = string.Join("", ((IEnumerable<string>) str.Split(' ')).Reverse<string>());
      return string.Compare(object1.type.Value + strA, object2.type.Value + str);
    }
    if (obj.Quality != this.Quality)
      return obj.Quality.CompareTo(this.Quality);
    return this is ColoredObject coloredObject1 && obj is ColoredObject coloredObject2 && coloredObject1.color.Value != coloredObject2.color.Value ? coloredObject2.GetHue().CompareTo(coloredObject1.GetHue()) : this.Stack - obj.Stack;
  }

  public int getCategorySortValue() => this.Category == -100 ? -94 : this.Category;

  protected virtual int getDescriptionWidth()
  {
    int val1;
    switch (LocalizedContentManager.CurrentLanguageCode)
    {
      case LocalizedContentManager.LanguageCode.fr:
        val1 = 384;
        break;
      case LocalizedContentManager.LanguageCode.tr:
        val1 = 336;
        break;
      default:
        val1 = 272;
        break;
    }
    return Math.Max(val1, (int) Game1.dialogueFont.MeasureString(this.DisplayName == null ? "" : this.DisplayName).X);
  }

  /// <summary>Set a temporary value that's not synchronized or saved, and can be read from <see cref="F:StardewValley.Item.tempData" />.</summary>
  /// <typeparam name="T">The value type.</typeparam>
  /// <param name="key">The temporary data kay.</param>
  /// <param name="value">The value to save.</param>
  public void SetTempData<T>(string key, T value)
  {
    if (this.tempData == null)
      this.tempData = new Dictionary<string, object>();
    this.tempData[key] = (object) value;
  }

  /// <summary>Get a temporary value that was added via <see cref="M:StardewValley.Item.SetTempData``1(System.String,``0)" />, if found.</summary>
  /// <typeparam name="T">The expected value type.</typeparam>
  /// <param name="key">The temporary data kay.</param>
  /// <param name="value">The value that was saved, if found.</param>
  /// <returns>Returns whether the <paramref name="key" /> was found.</returns>
  public bool TryGetTempData<T>(string key, out T value)
  {
    object obj1;
    if (this.tempData == null || !this.tempData.TryGetValue(key, out obj1))
    {
      value = default (T);
      return false;
    }
    if (obj1 == null)
    {
      value = default (T);
      return (object) value == null;
    }
    if (obj1 is T obj2)
    {
      value = obj2;
      return true;
    }
    value = default (T);
    return false;
  }

  /// <summary>Ensure the <see cref="F:StardewValley.Item.stack" /> is set to a valid value.</summary>
  public virtual void FixStackSize()
  {
    this.stack.Value = Utility.Clamp(this.stack.Value, 1, this.maximumStackSize());
  }

  /// <summary>Ensure the <see cref="F:StardewValley.Item.quality" /> is set to a valid value.</summary>
  public virtual void FixQuality()
  {
    this.quality.Value = Utility.Clamp(this.quality.Value, 0, 4);
    if (this.quality.Value != 3)
      return;
    this.quality.Value = 4;
  }

  public virtual void resetState()
  {
  }

  public virtual bool HasEquipmentBuffs()
  {
    BuffEffects effects = new BuffEffects();
    this.AddEquipmentEffects(effects);
    return effects.HasAnyValue();
  }

  public virtual void AddEquipmentEffects(BuffEffects effects)
  {
  }

  public virtual IEnumerable<Buff> GetFoodOrDrinkBuffs()
  {
    return (IEnumerable<Buff>) LegacyShims.EmptyArray<Buff>();
  }

  /// <summary>Auto-generate a default light source ID for this item when held or equipped by a player.</summary>
  /// <param name="heldBy">The player holding this item.</param>
  /// <remarks>This generates a new ID each time it's called. Make sure to save the value if you need to reference the same ID later.</remarks>
  public virtual string GenerateLightSourceId(Farmer heldBy)
  {
    return $"{this.GetType().Name}_{(heldBy != null ? heldBy.UniqueMultiplayerID : -1L)}_{Game1.random.Next()}";
  }

  /// <summary>Perform an action for each item contained within this item (e.g. items stored within a chest, item placed on a table, etc). This doesn't include the item itself.</summary>
  /// <param name="handler">The action to perform for each item.</param>
  /// <param name="getPath">The contextual path leading to this item (including the item), or <c>null</c> to treat this as the root.</param>
  /// <returns>Returns whether to continue iterating.</returns>
  public virtual bool ForEachItem(ForEachItemDelegate handler, GetForEachItemPathDelegate getPath)
  {
    return true;
  }
}
