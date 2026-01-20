// Decompiled with JetBrains decompiler
// Type: StardewValley.Tool
// Assembly: Stardew Valley, Version=1.6.15.24356, Culture=neutral, PublicKeyToken=null
// MVID: AA1F513A-94F0-4EF6-A35F-2D5B4A3721BF
// Assembly location: D:\SteamLibrary\steamapps\common\Stardew Valley\Stardew Valley.dll
// XML documentation location: D:\SteamLibrary\steamapps\common\Stardew Valley\Stardew Valley.xml

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Netcode;
using StardewValley.Buffs;
using StardewValley.Enchantments;
using StardewValley.GameData.Tools;
using StardewValley.Internal;
using StardewValley.ItemTypeDefinitions;
using StardewValley.TokenizableStrings;
using StardewValley.Tools;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Serialization;

#nullable disable
namespace StardewValley;

[XmlInclude(typeof (Axe))]
[XmlInclude(typeof (ErrorTool))]
[XmlInclude(typeof (FishingRod))]
[XmlInclude(typeof (GenericTool))]
[XmlInclude(typeof (Hoe))]
[XmlInclude(typeof (MeleeWeapon))]
[XmlInclude(typeof (MilkPail))]
[XmlInclude(typeof (Pan))]
[XmlInclude(typeof (Pickaxe))]
[XmlInclude(typeof (Shears))]
[XmlInclude(typeof (Slingshot))]
[XmlInclude(typeof (Wand))]
[XmlInclude(typeof (WateringCan))]
public abstract class Tool : Item
{
  public const int standardStaminaReduction = 2;
  public const int stone = 0;
  public const int copper = 1;
  public const int steel = 2;
  public const int gold = 3;
  public const int iridium = 4;
  public const int hammerSpriteIndex = 105;
  public const int wateringCanSpriteIndex = 273;
  public const int fishingRodSpriteIndex = 8;
  public const int wateringCanMenuIndex = 296;
  public const string weaponsTextureName = "TileSheets\\weapons";
  public static Texture2D weaponsTexture;
  [XmlElement("initialParentTileIndex")]
  public readonly NetInt initialParentTileIndex = new NetInt();
  [XmlElement("currentParentTileIndex")]
  public readonly NetInt currentParentTileIndex = new NetInt();
  [XmlElement("indexOfMenuItemView")]
  public readonly NetInt indexOfMenuItemView = new NetInt();
  [XmlElement("instantUse")]
  public readonly NetBool instantUse = new NetBool();
  [XmlElement("isEfficient")]
  public readonly NetBool isEfficient = new NetBool();
  [XmlElement("animationSpeedModifier")]
  public readonly NetFloat animationSpeedModifier = new NetFloat(1f);
  /// <summary>
  /// increments every swing. Not accurate for how many times the tool has been swung
  /// </summary>
  public int swingTicker = Game1.random.Next(999999);
  [XmlIgnore]
  private string _description;
  [XmlElement("upgradeLevel")]
  public readonly NetInt upgradeLevel = new NetInt();
  [XmlElement("numAttachmentSlots")]
  public readonly NetInt numAttachmentSlots = new NetInt();
  /// <summary>The last player who used this tool, if any.</summary>
  /// <remarks>Most code should use <see cref="M:StardewValley.Tool.getLastFarmerToUse" /> instead.</remarks>
  [XmlIgnore]
  public Farmer lastUser;
  public readonly NetObjectArray<Object> attachments = new NetObjectArray<Object>();
  /// <summary>The cached value for <see cref="P:StardewValley.Tool.DisplayName" />.</summary>
  [XmlIgnore]
  protected string displayName;
  [XmlElement("enchantments")]
  public readonly NetList<BaseEnchantment, NetRef<BaseEnchantment>> enchantments = new NetList<BaseEnchantment, NetRef<BaseEnchantment>>();
  [XmlElement("previousEnchantments")]
  public readonly NetStringList previousEnchantments = new NetStringList();
  /// <summary>Whether to play sounds when this tool is applied to a tile.</summary>
  /// <remarks>This should nearly always be true. It can be disabled for automated tools to avoid hitting audio instance limits.</remarks>
  [XmlIgnore]
  public bool PlayUseSounds = true;

  [XmlIgnore]
  public string description
  {
    get
    {
      if (this._description == null)
        this._description = this.loadDescription();
      return this._description;
    }
    set => this._description = value;
  }

  /// <inheritdoc />
  public override string TypeDefinitionId { get; } = "(T)";

  /// <inheritdoc />
  [XmlIgnore]
  public override string DisplayName => this.loadDisplayName();

  public string Description => this.description;

  [XmlIgnore]
  public int CurrentParentTileIndex
  {
    get => this.currentParentTileIndex.Value;
    set => this.currentParentTileIndex.Set(value);
  }

  public int InitialParentTileIndex
  {
    get => this.initialParentTileIndex.Value;
    set => this.initialParentTileIndex.Set(value);
  }

  public int IndexOfMenuItemView
  {
    get => this.indexOfMenuItemView.Value;
    set => this.indexOfMenuItemView.Set(value);
  }

  [XmlIgnore]
  public int UpgradeLevel
  {
    get => this.upgradeLevel.Value;
    set => this.upgradeLevel.Value = value;
  }

  [XmlIgnore]
  public int AttachmentSlotsCount
  {
    get => this.attachmentSlots();
    set
    {
      this.numAttachmentSlots.Value = value;
      this.attachments.SetCount(value);
    }
  }

  public bool InstantUse
  {
    get => this.instantUse.Value;
    set => this.instantUse.Value = value;
  }

  public bool IsEfficient
  {
    get => this.isEfficient.Value;
    set => this.isEfficient.Value = value;
  }

  public float AnimationSpeedModifier
  {
    get => this.animationSpeedModifier.Value;
    set => this.animationSpeedModifier.Value = value;
  }

  public Tool()
  {
    this.initNetFields();
    this.Category = -99;
  }

  public Tool(
    string name,
    int upgradeLevel,
    int initialParentTileIndex,
    int indexOfMenuItemView,
    bool stackable,
    int numAttachmentSlots = 0)
    : this()
  {
    this.Name = name ?? ItemRegistry.GetDataOrErrorItem(this.QualifiedItemId).InternalName;
    this.SetSpriteIndex(initialParentTileIndex);
    this.IndexOfMenuItemView = indexOfMenuItemView;
    this.AttachmentSlotsCount = Math.Max(0, numAttachmentSlots);
    this.Category = -99;
    this.UpgradeLevel = upgradeLevel;
  }

  /// <summary>Set the single sprite index to display for this tool.</summary>
  /// <param name="spriteIndex">The sprite index.</param>
  /// <remarks>This overrides upgrade level adjustments, so this should be called before setting the upgrade level for tools that have a dynamic sprite index.</remarks>
  public virtual void SetSpriteIndex(int spriteIndex)
  {
    this.InitialParentTileIndex = spriteIndex;
    this.IndexOfMenuItemView = spriteIndex;
    this.CurrentParentTileIndex = spriteIndex;
  }

  protected new virtual void initNetFields()
  {
    this.NetFields.SetOwner((INetObject<NetFields>) this).AddField((INetSerializable) this.initialParentTileIndex, "initialParentTileIndex").AddField((INetSerializable) this.currentParentTileIndex, "currentParentTileIndex").AddField((INetSerializable) this.indexOfMenuItemView, "indexOfMenuItemView").AddField((INetSerializable) this.instantUse, "instantUse").AddField((INetSerializable) this.upgradeLevel, "upgradeLevel").AddField((INetSerializable) this.numAttachmentSlots, "numAttachmentSlots").AddField((INetSerializable) this.attachments, "attachments").AddField((INetSerializable) this.enchantments, "enchantments").AddField((INetSerializable) this.isEfficient, "isEfficient").AddField((INetSerializable) this.animationSpeedModifier, "animationSpeedModifier").AddField((INetSerializable) this.previousEnchantments, "previousEnchantments");
  }

  /// <inheritdoc />
  protected override void MigrateLegacyItemId() => this.ItemId = this.GetType().Name;

  protected virtual string loadDisplayName()
  {
    return ItemRegistry.GetDataOrErrorItem(this.QualifiedItemId).DisplayName;
  }

  protected virtual string loadDescription()
  {
    return ItemRegistry.GetDataOrErrorItem(this.QualifiedItemId).Description;
  }

  /// <inheritdoc />
  public override bool CanBeLostOnDeath()
  {
    if (!base.CanBeLostOnDeath())
      return false;
    ToolData toolData = this.GetToolData();
    return toolData == null || toolData.CanBeLostOnDeath;
  }

  /// <inheritdoc />
  public override string getCategoryName() => Object.GetCategoryDisplayName(-99);

  /// <inheritdoc />
  protected override void GetOneCopyFrom(Item source)
  {
    base.GetOneCopyFrom(source);
    if (!(source is Tool source1))
      return;
    this.SetSpriteIndex(source1.InitialParentTileIndex);
    this.Name = source.Name;
    this.CurrentParentTileIndex = source1.CurrentParentTileIndex;
    this.IndexOfMenuItemView = source1.IndexOfMenuItemView;
    this.InstantUse = source1.InstantUse;
    this.IsEfficient = source1.IsEfficient;
    this.AnimationSpeedModifier = source1.AnimationSpeedModifier;
    this.UpgradeLevel = source1.UpgradeLevel;
    this.AttachmentSlotsCount = source1.AttachmentSlotsCount;
    this.CopyEnchantments(source1, this);
  }

  /// <summary>Update this tool when it's created by upgrading a previous tool.</summary>
  /// <param name="other">The previous tool instance being upgraded into this tool.</param>
  public virtual void UpgradeFrom(Tool other) => this.CopyEnchantments(other, this);

  /// <inheritdoc />
  public override Color getCategoryColor() => Color.DarkSlateGray;

  /// <summary>Get the underlying tool data from <c>Data/Tools</c>, if available.</summary>
  public ToolData GetToolData() => ItemRegistry.GetData(this.QualifiedItemId)?.RawData as ToolData;

  public virtual void draw(SpriteBatch b)
  {
    Farmer lastUser = this.lastUser;
    if ((lastUser != null ? (lastUser.toolPower.Value > 0 ? 1 : 0) : 0) == 0 || !this.lastUser.canReleaseTool || !this.lastUser.IsLocalPlayer)
      return;
    foreach (Vector2 vector2 in this.tilesAffected(this.lastUser.GetToolLocation() / 64f, this.lastUser.toolPower.Value, this.lastUser))
      b.Draw(Game1.mouseCursors, Game1.GlobalToLocal(new Vector2((float) ((int) vector2.X * 64 /*0x40*/), (float) ((int) vector2.Y * 64 /*0x40*/))), new Rectangle?(new Rectangle(194, 388, 16 /*0x10*/, 16 /*0x10*/)), Color.White, 0.0f, Vector2.Zero, 4f, SpriteEffects.None, 0.01f);
  }

  public override void drawAttachments(SpriteBatch b, int x, int y)
  {
    y += this.enchantments.Count > 0 ? 8 : 4;
    for (int slot = 0; slot < this.AttachmentSlotsCount; ++slot)
      this.DrawAttachmentSlot(slot, b, x, y + slot * 68);
  }

  /// <summary>Draw an attachment slot at the given position.</summary>
  /// <param name="slot">The attachment slot index.</param>
  /// <param name="b">The sprite batch being drawn.</param>
  /// <param name="x">The X position at which to draw the slot.</param>
  /// <param name="y">The Y position at which to draw the slot.</param>
  /// <remarks>This should draw a 64x64 slot.</remarks>
  protected virtual void DrawAttachmentSlot(int slot, SpriteBatch b, int x, int y)
  {
    Vector2 vector2 = new Vector2((float) x, (float) y);
    Texture2D texture;
    Rectangle sourceRect;
    this.GetAttachmentSlotSprite(slot, out texture, out sourceRect);
    b.Draw(texture, vector2, new Rectangle?(sourceRect), Color.White, 0.0f, Vector2.Zero, 1f, SpriteEffects.None, 0.86f);
    this.attachments[slot]?.drawInMenu(b, vector2, 1f);
  }

  /// <summary>Get the sprite to draw for an attachment slot background.</summary>
  /// <param name="slot">The attachment slot index.</param>
  /// <param name="texture">The texture to draw.</param>
  /// <param name="sourceRect">The pixel area within the texture to draw.</param>
  protected virtual void GetAttachmentSlotSprite(
    int slot,
    out Texture2D texture,
    out Rectangle sourceRect)
  {
    texture = Game1.menuTexture;
    sourceRect = Game1.getSourceRectForStandardTileSheet(Game1.menuTexture, 10);
  }

  public override void drawTooltip(
    SpriteBatch spriteBatch,
    ref int x,
    ref int y,
    SpriteFont font,
    float alpha,
    StringBuilder overrideText)
  {
    base.drawTooltip(spriteBatch, ref x, ref y, font, alpha, overrideText);
    foreach (BaseEnchantment enchantment in this.enchantments)
    {
      if (enchantment.ShouldBeDisplayed())
      {
        Utility.drawWithShadow(spriteBatch, Game1.mouseCursors2, new Vector2((float) (x + 16 /*0x10*/ + 4), (float) (y + 16 /*0x10*/ + 4)), new Rectangle((int) sbyte.MaxValue, 35, 10, 10), Color.White, 0.0f, Vector2.Zero, 4f, layerDepth: 1f);
        Utility.drawTextWithShadow(spriteBatch, BaseEnchantment.hideEnchantmentName ? "???" : enchantment.GetDisplayName(), font, new Vector2((float) (x + 16 /*0x10*/ + 52), (float) (y + 16 /*0x10*/ + 12)), new Color(120, 0, 210) * 0.9f * alpha);
        y += (int) Math.Max(font.MeasureString("TT").Y, 48f);
      }
    }
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
    Point tooltipSpecialIcons = base.getExtraSpaceNeededForTooltipSpecialIcons(font, minWidth, horizontalBuffer, startingHeight, descriptionText, boldTitleText, moneyAmountToDisplayAtBottom) with
    {
      Y = startingHeight
    };
    foreach (BaseEnchantment enchantment in this.enchantments)
    {
      if (enchantment.ShouldBeDisplayed())
        tooltipSpecialIcons.Y += (int) Math.Max(font.MeasureString("TT").Y, 48f);
    }
    return tooltipSpecialIcons;
  }

  public virtual void tickUpdate(GameTime time, Farmer who)
  {
  }

  public virtual bool isHeavyHitter()
  {
    switch (this)
    {
      case MeleeWeapon _:
      case Hoe _:
      case Axe _:
        return true;
      default:
        return this is Pickaxe;
    }
  }

  /// <summary>Get whether this is a scythe tool.</summary>
  public virtual bool isScythe() => false;

  public virtual void Update(int direction, int farmerMotionFrame, Farmer who)
  {
    int num = 0;
    Tool tool = this;
    if (!(tool is WateringCan))
    {
      if (tool is FishingRod)
      {
        switch (direction)
        {
          case 0:
            num = 3;
            break;
          case 1:
            num = 0;
            break;
          case 3:
            num = 0;
            break;
        }
      }
      else
      {
        switch (direction)
        {
          case 0:
            num = 3;
            break;
          case 1:
            num = 2;
            break;
          case 3:
            num = 2;
            break;
        }
      }
    }
    else
    {
      switch (direction)
      {
        case 0:
          num = 4;
          break;
        case 1:
          num = 2;
          break;
        case 2:
          num = 0;
          break;
        case 3:
          num = 2;
          break;
      }
    }
    if (this.QualifiedItemId != "(T)WateringCan")
    {
      if (farmerMotionFrame < 1)
        this.CurrentParentTileIndex = this.InitialParentTileIndex;
      else if (who.FacingDirection == 0 || who.FacingDirection == 2 && farmerMotionFrame >= 2)
        this.CurrentParentTileIndex = this.InitialParentTileIndex + 1;
    }
    else
      this.CurrentParentTileIndex = farmerMotionFrame < 5 || direction == 0 ? this.InitialParentTileIndex : this.InitialParentTileIndex + 1;
    this.CurrentParentTileIndex += num;
  }

  /// <inheritdoc />
  public override int salePrice(bool ignoreProfitMargins = false)
  {
    ToolData toolData = this.GetToolData();
    return toolData == null || toolData.SalePrice < 0 ? base.salePrice(ignoreProfitMargins) : toolData.SalePrice;
  }

  public override int attachmentSlots() => this.numAttachmentSlots.Value;

  /// <summary>Get the last player who used this tool, if any.</summary>
  public Farmer getLastFarmerToUse() => this.lastUser;

  public virtual void leftClick(Farmer who)
  {
  }

  public virtual void DoFunction(GameLocation location, int x, int y, int power, Farmer who)
  {
    this.lastUser = who;
    Game1.recentMultiplayerRandom = Utility.CreateRandom((double) (short) Game1.random.Next((int) short.MinValue, 32768 /*0x8000*/));
    if (this.isHeavyHitter() && !(this is MeleeWeapon))
    {
      Rumble.rumble(0.1f + (float) (Game1.random.NextDouble() / 4.0), (float) (100 + Game1.random.Next(50)));
      location.damageMonster(new Rectangle(x - 32 /*0x20*/, y - 32 /*0x20*/, 64 /*0x40*/, 64 /*0x40*/), this.upgradeLevel.Value + 1, (this.upgradeLevel.Value + 1) * 3, false, who);
    }
    if (!(this is MeleeWeapon meleeWeapon) || who.UsingTool && Game1.mouseClickPolling < 50 && meleeWeapon.type.Value != 1 && meleeWeapon.ItemId != "47" && MeleeWeapon.timedHitTimer <= 0 && who.FarmerSprite.currentAnimationIndex == 5 && (double) who.FarmerSprite.timer < (double) who.FarmerSprite.interval / 4.0)
      return;
    if (meleeWeapon.type.Value == 2 && meleeWeapon.isOnSpecial)
    {
      meleeWeapon.triggerClubFunction(who);
    }
    else
    {
      if (who.FarmerSprite.currentAnimationIndex <= 0)
        return;
      MeleeWeapon.timedHitTimer = 500;
    }
  }

  public virtual void endUsing(GameLocation location, Farmer who)
  {
    ++this.swingTicker;
    who.stopJittering();
    who.canReleaseTool = false;
    int num = (double) who.Stamina <= 0.0 ? 2 : 1;
    if (Game1.isAnyGamePadButtonBeingPressed() || !who.IsLocalPlayer)
      who.lastClick = who.GetToolLocation();
    switch (this)
    {
      case WateringCan wateringCan:
        if (wateringCan.WaterLeft > 0 && who.ShouldHandleAnimationSound() && this.PlayUseSounds)
          who.playNearbySoundLocal("wateringCan");
        switch (who.FacingDirection)
        {
          case 0:
            ((FarmerSprite) who.Sprite).animateOnce(180, 125f * (float) num, 3);
            return;
          case 1:
            ((FarmerSprite) who.Sprite).animateOnce(172, 125f * (float) num, 3);
            return;
          case 2:
            ((FarmerSprite) who.Sprite).animateOnce(164, 125f * (float) num, 3);
            return;
          case 3:
            ((FarmerSprite) who.Sprite).animateOnce(188, 125f * (float) num, 3);
            return;
          default:
            return;
        }
      case FishingRod fishingRod when who.IsLocalPlayer && Game1.activeClickableMenu == null:
        if (fishingRod.hit)
          break;
        this.DoFunction(who.currentLocation, (int) who.lastClick.X, (int) who.lastClick.Y, 1, who);
        break;
      case MeleeWeapon _:
      case Pan _:
      case Shears _:
      case MilkPail _:
      case Slingshot _:
        break;
      default:
        switch (who.FacingDirection)
        {
          case 0:
            ((FarmerSprite) who.Sprite).animateOnce(176 /*0xB0*/, 60f * (float) num, 8);
            return;
          case 1:
            ((FarmerSprite) who.Sprite).animateOnce(168, 60f * (float) num, 8);
            return;
          case 2:
            ((FarmerSprite) who.Sprite).animateOnce(160 /*0xA0*/, 60f * (float) num, 8);
            return;
          case 3:
            ((FarmerSprite) who.Sprite).animateOnce(184, 60f * (float) num, 8);
            return;
          default:
            return;
        }
    }
  }

  public virtual bool beginUsing(GameLocation location, int x, int y, Farmer who)
  {
    this.lastUser = who;
    if (!this.instantUse.Value)
    {
      who.Halt();
      this.Update(who.FacingDirection, 0, who);
      if (!(this is FishingRod) && this.upgradeLevel.Value <= 0 && !(this is MeleeWeapon) || this is Pickaxe)
      {
        who.EndUsingTool();
        return true;
      }
    }
    if (this.instantUse.Value)
    {
      Game1.toolAnimationDone(who);
      who.CanMove = true;
      who.canReleaseTool = false;
      who.UsingTool = false;
    }
    else
    {
      switch (this)
      {
        case WateringCan _ when location.CanRefillWateringCanOnTile((int) who.GetToolLocation().X / 64 /*0x40*/, (int) who.GetToolLocation().Y / 64 /*0x40*/):
          switch (who.FacingDirection)
          {
            case 0:
              ((FarmerSprite) who.Sprite).animateOnce(182, 250f, 2);
              this.Update(0, 1, who);
              break;
            case 1:
              ((FarmerSprite) who.Sprite).animateOnce(174, 250f, 2);
              this.Update(1, 0, who);
              break;
            case 2:
              ((FarmerSprite) who.Sprite).animateOnce(166, 250f, 2);
              this.Update(2, 1, who);
              break;
            case 3:
              ((FarmerSprite) who.Sprite).animateOnce(190, 250f, 2);
              this.Update(3, 0, who);
              break;
          }
          who.canReleaseTool = false;
          break;
        case WateringCan wateringCan when wateringCan.WaterLeft <= 0:
          Game1.toolAnimationDone(who);
          who.CanMove = true;
          who.canReleaseTool = false;
          break;
        case WateringCan _:
          who.jitterStrength = 0.25f;
          switch (who.FacingDirection)
          {
            case 0:
              who.FarmerSprite.setCurrentFrame(180);
              this.Update(0, 0, who);
              break;
            case 1:
              who.FarmerSprite.setCurrentFrame(172);
              this.Update(1, 0, who);
              break;
            case 2:
              who.FarmerSprite.setCurrentFrame(164);
              this.Update(2, 0, who);
              break;
            case 3:
              who.FarmerSprite.setCurrentFrame(188);
              this.Update(3, 0, who);
              break;
          }
          break;
        case FishingRod _:
          switch (who.FacingDirection)
          {
            case 0:
              ((FarmerSprite) who.Sprite).animateOnce(295, 35f, 8, new AnimatedSprite.endOfAnimationBehavior(FishingRod.endOfAnimationBehavior));
              this.Update(0, 0, who);
              break;
            case 1:
              ((FarmerSprite) who.Sprite).animateOnce(296, 35f, 8, new AnimatedSprite.endOfAnimationBehavior(FishingRod.endOfAnimationBehavior));
              this.Update(1, 0, who);
              break;
            case 2:
              ((FarmerSprite) who.Sprite).animateOnce(297, 35f, 8, new AnimatedSprite.endOfAnimationBehavior(FishingRod.endOfAnimationBehavior));
              this.Update(2, 0, who);
              break;
            case 3:
              ((FarmerSprite) who.Sprite).animateOnce(298, 35f, 8, new AnimatedSprite.endOfAnimationBehavior(FishingRod.endOfAnimationBehavior));
              this.Update(3, 0, who);
              break;
          }
          who.canReleaseTool = false;
          break;
        case MeleeWeapon _:
          ((MeleeWeapon) this).setFarmerAnimating(who);
          break;
        default:
          switch (who.FacingDirection)
          {
            case 0:
              who.FarmerSprite.setCurrentFrame(176 /*0xB0*/);
              this.Update(0, 0, who);
              break;
            case 1:
              who.FarmerSprite.setCurrentFrame(168);
              this.Update(1, 0, who);
              break;
            case 2:
              who.FarmerSprite.setCurrentFrame(160 /*0xA0*/);
              this.Update(2, 0, who);
              break;
            case 3:
              who.FarmerSprite.setCurrentFrame(184);
              this.Update(3, 0, who);
              break;
          }
          break;
      }
    }
    return false;
  }

  public virtual bool onRelease(GameLocation location, int x, int y, Farmer who) => false;

  public override bool canBeDropped() => false;

  /// <summary>Get whether an item can be added to or removed from an attachment slot.</summary>
  /// <param name="o">The item to attach, or <c>null</c> to remove an attached item.</param>
  public virtual bool canThisBeAttached(Object o)
  {
    NetObjectArray<Object> attachments = this.attachments;
    // ISSUE: explicit non-virtual call
    if ((attachments != null ? (__nonvirtual (attachments.Count) > 0 ? 1 : 0) : 0) != 0)
    {
      if (o == null)
        return true;
      for (int slot = 0; slot < this.attachments.Length; ++slot)
      {
        if (this.canThisBeAttached(o, slot))
          return true;
      }
    }
    return false;
  }

  /// <summary>Get whether an item can be added to or removed from an attachment slot.</summary>
  /// <param name="o">The item to attach.</param>
  /// <param name="slot">The slot index. This is always a valid index when the method is called.</param>
  protected virtual bool canThisBeAttached(Object o, int slot) => true;

  /// <summary>Add an item to or remove it from an attachment slot.</summary>
  /// <param name="o">The item to attach, or <c>null</c> to remove an attached item.</param>
  public virtual Object attach(Object o)
  {
    if (o == null)
    {
      for (int index = 0; index < this.attachments.Length; ++index)
      {
        Object attachment = this.attachments[index];
        if (attachment != null)
        {
          this.attachments[index] = (Object) null;
          Game1.playSound("dwop");
          return attachment;
        }
      }
      return (Object) null;
    }
    int stack = o.Stack;
    for (int index = 0; index < this.attachments.Length; ++index)
    {
      if (this.canThisBeAttached(o, index))
      {
        Object attachment = this.attachments[index];
        if (attachment == null)
        {
          this.attachments[index] = o;
          o = (Object) null;
          break;
        }
        if (attachment.canStackWith((ISalable) o))
        {
          int amount = o.Stack - attachment.addToStack((Item) o);
          if (o.ConsumeStack(amount) == null)
          {
            o = (Object) null;
            break;
          }
        }
      }
    }
    if (o == null || o.Stack != stack)
    {
      Game1.playSound("button1");
      return o;
    }
    for (int index = 0; index < this.attachments.Length; ++index)
    {
      Object attachment = this.attachments[index];
      this.attachments[index] = (Object) null;
      if (this.canThisBeAttached(o, index))
      {
        this.attachments[index] = o;
        Game1.playSound("button1");
        return attachment;
      }
      this.attachments[index] = attachment;
    }
    return o;
  }

  public virtual void actionWhenClaimed()
  {
    if (!(this is GenericTool))
      return;
    switch (this.indexOfMenuItemView.Value)
    {
      case 13:
      case 14:
      case 15:
      case 16 /*0x10*/:
        ++Game1.player.trashCanLevel;
        break;
    }
  }

  public override bool CanBuyItem(Farmer who)
  {
    if (Game1.player.toolBeingUpgraded.Value == null)
    {
      switch (this)
      {
        case Axe _:
        case Pickaxe _:
        case Hoe _:
        case WateringCan _:
label_3:
          return true;
        case GenericTool _:
          if (this.indexOfMenuItemView.Value < 13 || this.indexOfMenuItemView.Value > 16 /*0x10*/)
            break;
          goto label_3;
      }
    }
    return base.CanBuyItem(who);
  }

  /// <inheritdoc />
  public override bool actionWhenPurchased(string shopId)
  {
    if (shopId == "ClintUpgrade" && Game1.player.toolBeingUpgraded.Value == null)
    {
      switch (this)
      {
        case Axe _:
        case Pickaxe _:
        case Hoe _:
        case WateringCan _:
        case Pan _:
          string requireToolId = ShopBuilder.GetToolUpgradeData(this.GetToolData(), Game1.player)?.RequireToolId;
          if (requireToolId != null)
          {
            Item which = Game1.player.Items.GetById(requireToolId).FirstOrDefault<Item>();
            Game1.player.removeItemFromInventory(which);
            if (which is Tool other)
              this.UpgradeFrom(other);
          }
          Game1.player.toolBeingUpgraded.Value = (Tool) this.getOne();
          Game1.player.daysLeftForToolUpgrade.Value = 2;
          Game1.playSound("parry");
          Game1.exitActiveMenu();
          Game1.DrawDialogue(Game1.getCharacterFromName("Clint"), "Strings\\StringsFromCSFiles:Tool.cs.14317");
          return true;
        case GenericTool _:
          switch (this.indexOfMenuItemView.Value)
          {
            case 13:
            case 14:
            case 15:
            case 16 /*0x10*/:
              Game1.player.toolBeingUpgraded.Value = (Tool) this.getOne();
              Game1.player.daysLeftForToolUpgrade.Value = 2;
              Game1.playSound("parry");
              Game1.exitActiveMenu();
              Game1.DrawDialogue(Game1.getCharacterFromName("Clint"), "Strings\\StringsFromCSFiles:Tool.cs.14317");
              return true;
          }
          break;
      }
    }
    return base.actionWhenPurchased(shopId);
  }

  protected List<Vector2> tilesAffected(Vector2 tileLocation, int power, Farmer who)
  {
    ++power;
    List<Vector2> vector2List = new List<Vector2>();
    vector2List.Add(tileLocation);
    Vector2 vector2 = Vector2.Zero;
    switch (who.FacingDirection)
    {
      case 0:
        if (power >= 6)
        {
          vector2 = new Vector2(tileLocation.X, tileLocation.Y - 2f);
          break;
        }
        if (power >= 2)
        {
          vector2List.Add(tileLocation + new Vector2(0.0f, -1f));
          vector2List.Add(tileLocation + new Vector2(0.0f, -2f));
        }
        if (power >= 3)
        {
          vector2List.Add(tileLocation + new Vector2(0.0f, -3f));
          vector2List.Add(tileLocation + new Vector2(0.0f, -4f));
        }
        if (power >= 4)
        {
          vector2List.RemoveAt(vector2List.Count - 1);
          vector2List.RemoveAt(vector2List.Count - 1);
          vector2List.Add(tileLocation + new Vector2(1f, -2f));
          vector2List.Add(tileLocation + new Vector2(1f, -1f));
          vector2List.Add(tileLocation + new Vector2(1f, 0.0f));
          vector2List.Add(tileLocation + new Vector2(-1f, -2f));
          vector2List.Add(tileLocation + new Vector2(-1f, -1f));
          vector2List.Add(tileLocation + new Vector2(-1f, 0.0f));
        }
        if (power >= 5)
        {
          for (int index = vector2List.Count - 1; index >= 0; --index)
            vector2List.Add(vector2List[index] + new Vector2(0.0f, -3f));
          break;
        }
        break;
      case 1:
        if (power >= 6)
        {
          vector2 = new Vector2(tileLocation.X + 2f, tileLocation.Y);
          break;
        }
        if (power >= 2)
        {
          vector2List.Add(tileLocation + new Vector2(1f, 0.0f));
          vector2List.Add(tileLocation + new Vector2(2f, 0.0f));
        }
        if (power >= 3)
        {
          vector2List.Add(tileLocation + new Vector2(3f, 0.0f));
          vector2List.Add(tileLocation + new Vector2(4f, 0.0f));
        }
        if (power >= 4)
        {
          vector2List.RemoveAt(vector2List.Count - 1);
          vector2List.RemoveAt(vector2List.Count - 1);
          vector2List.Add(tileLocation + new Vector2(0.0f, -1f));
          vector2List.Add(tileLocation + new Vector2(1f, -1f));
          vector2List.Add(tileLocation + new Vector2(2f, -1f));
          vector2List.Add(tileLocation + new Vector2(0.0f, 1f));
          vector2List.Add(tileLocation + new Vector2(1f, 1f));
          vector2List.Add(tileLocation + new Vector2(2f, 1f));
        }
        if (power >= 5)
        {
          for (int index = vector2List.Count - 1; index >= 0; --index)
            vector2List.Add(vector2List[index] + new Vector2(3f, 0.0f));
          break;
        }
        break;
      case 2:
        if (power >= 6)
        {
          vector2 = new Vector2(tileLocation.X, tileLocation.Y + 2f);
          break;
        }
        if (power >= 2)
        {
          vector2List.Add(tileLocation + new Vector2(0.0f, 1f));
          vector2List.Add(tileLocation + new Vector2(0.0f, 2f));
        }
        if (power >= 3)
        {
          vector2List.Add(tileLocation + new Vector2(0.0f, 3f));
          vector2List.Add(tileLocation + new Vector2(0.0f, 4f));
        }
        if (power >= 4)
        {
          vector2List.RemoveAt(vector2List.Count - 1);
          vector2List.RemoveAt(vector2List.Count - 1);
          vector2List.Add(tileLocation + new Vector2(1f, 2f));
          vector2List.Add(tileLocation + new Vector2(1f, 1f));
          vector2List.Add(tileLocation + new Vector2(1f, 0.0f));
          vector2List.Add(tileLocation + new Vector2(-1f, 2f));
          vector2List.Add(tileLocation + new Vector2(-1f, 1f));
          vector2List.Add(tileLocation + new Vector2(-1f, 0.0f));
        }
        if (power >= 5)
        {
          for (int index = vector2List.Count - 1; index >= 0; --index)
            vector2List.Add(vector2List[index] + new Vector2(0.0f, 3f));
          break;
        }
        break;
      case 3:
        if (power >= 6)
        {
          vector2 = new Vector2(tileLocation.X - 2f, tileLocation.Y);
          break;
        }
        if (power >= 2)
        {
          vector2List.Add(tileLocation + new Vector2(-1f, 0.0f));
          vector2List.Add(tileLocation + new Vector2(-2f, 0.0f));
        }
        if (power >= 3)
        {
          vector2List.Add(tileLocation + new Vector2(-3f, 0.0f));
          vector2List.Add(tileLocation + new Vector2(-4f, 0.0f));
        }
        if (power >= 4)
        {
          vector2List.RemoveAt(vector2List.Count - 1);
          vector2List.RemoveAt(vector2List.Count - 1);
          vector2List.Add(tileLocation + new Vector2(0.0f, -1f));
          vector2List.Add(tileLocation + new Vector2(-1f, -1f));
          vector2List.Add(tileLocation + new Vector2(-2f, -1f));
          vector2List.Add(tileLocation + new Vector2(0.0f, 1f));
          vector2List.Add(tileLocation + new Vector2(-1f, 1f));
          vector2List.Add(tileLocation + new Vector2(-2f, 1f));
        }
        if (power >= 5)
        {
          for (int index = vector2List.Count - 1; index >= 0; --index)
            vector2List.Add(vector2List[index] + new Vector2(-3f, 0.0f));
          break;
        }
        break;
    }
    if (power >= 6)
    {
      vector2List.Clear();
      for (int x = (int) vector2.X - 2; (double) x <= (double) vector2.X + 2.0; ++x)
      {
        for (int y = (int) vector2.Y - 2; (double) y <= (double) vector2.Y + 2.0; ++y)
          vector2List.Add(new Vector2((float) x, (float) y));
      }
    }
    return vector2List;
  }

  public virtual bool doesShowTileLocationMarker() => true;

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
    spriteBatch.Draw(dataOrErrorItem.GetTexture(), location + new Vector2(32f, 32f), new Rectangle?(dataOrErrorItem.GetSourceRect()), color * transparency, 0.0f, new Vector2(8f, 8f), 4f * scaleSize, SpriteEffects.None, layerDepth);
    this.DrawMenuIcons(spriteBatch, location, scaleSize, transparency, layerDepth, drawStackNumber, color);
  }

  public override bool isPlaceable() => false;

  public override int maximumStackSize() => 1;

  public override string getDescription()
  {
    return Game1.parseText(this.description, Game1.smallFont, this.getDescriptionWidth());
  }

  protected override int getDescriptionWidth()
  {
    int val1 = base.getDescriptionWidth();
    foreach (BaseEnchantment enchantment in this.enchantments)
      val1 = Math.Max(val1, (int) ((double) Game1.smallFont.MeasureString(enchantment.GetDisplayName()).X + 128.0));
    return val1;
  }

  public virtual void ClearEnchantments()
  {
    for (int index = this.enchantments.Count - 1; index >= 0; --index)
      this.enchantments[index].UnapplyTo((Item) this);
    this.enchantments.Clear();
  }

  public virtual int GetMaxForges() => 0;

  public virtual bool CanAddEnchantment(BaseEnchantment enchantment)
  {
    if (!enchantment.IsForge() && !enchantment.IsSecondaryEnchantment())
      return true;
    if (this.GetTotalForgeLevels() >= this.GetMaxForges() && !enchantment.IsSecondaryEnchantment() || enchantment == null)
      return false;
    foreach (BaseEnchantment enchantment1 in this.enchantments)
    {
      if (enchantment.GetType() == enchantment1.GetType())
        return enchantment1.GetMaximumLevel() < 0 || enchantment1.GetLevel() < enchantment1.GetMaximumLevel();
    }
    return true;
  }

  public virtual void CopyEnchantments(Tool source, Tool destination)
  {
    foreach (BaseEnchantment enchantment in source.enchantments)
    {
      destination.enchantments.Add(enchantment.GetOne());
      enchantment.GetOne().ApplyTo((Item) destination);
    }
    destination.previousEnchantments.Clear();
    destination.previousEnchantments.AddRange((IEnumerable<string>) source.previousEnchantments);
  }

  public int GetTotalForgeLevels(bool for_unforge = false)
  {
    int totalForgeLevels = 0;
    foreach (BaseEnchantment enchantment in this.enchantments)
    {
      if (enchantment is DiamondEnchantment)
      {
        if (for_unforge)
          return totalForgeLevels;
      }
      else if (enchantment.IsForge())
        totalForgeLevels += enchantment.GetLevel();
    }
    return totalForgeLevels;
  }

  public virtual bool AddEnchantment(BaseEnchantment enchantment)
  {
    if (enchantment == null)
      return false;
    if (this is MeleeWeapon && (enchantment.IsForge() || enchantment.IsSecondaryEnchantment()))
    {
      foreach (BaseEnchantment enchantment1 in this.enchantments)
      {
        if (enchantment.GetType() == enchantment1.GetType())
        {
          if (enchantment1.GetMaximumLevel() >= 0 && enchantment1.GetLevel() >= enchantment1.GetMaximumLevel())
            return false;
          enchantment1.SetLevel((Item) this, enchantment1.GetLevel() + 1);
          return true;
        }
      }
      this.enchantments.Add(enchantment);
      enchantment.ApplyTo((Item) this, this.lastUser);
      return true;
    }
    for (int index = this.enchantments.Count - 1; index >= 0; --index)
    {
      BaseEnchantment enchantment2 = this.enchantments[index];
      if (!enchantment2.IsForge() && !enchantment2.IsSecondaryEnchantment())
      {
        enchantment2.UnapplyTo((Item) this);
        this.enchantments.RemoveAt(index);
      }
    }
    this.enchantments.Add(enchantment);
    enchantment.ApplyTo((Item) this, this.lastUser);
    return true;
  }

  public bool hasEnchantmentOfType<T>()
  {
    foreach (BaseEnchantment enchantment in this.enchantments)
    {
      if (enchantment is T)
        return true;
    }
    return false;
  }

  public virtual void RemoveEnchantment(BaseEnchantment enchantment)
  {
    if (enchantment == null)
      return;
    this.enchantments.Remove(enchantment);
    enchantment.UnapplyTo((Item) this, this.lastUser);
  }

  public override void actionWhenBeingHeld(Farmer who)
  {
    base.actionWhenBeingHeld(who);
    if (!who.IsLocalPlayer)
      return;
    foreach (BaseEnchantment enchantment in this.enchantments)
      enchantment.OnEquip(who);
  }

  public override void actionWhenStopBeingHeld(Farmer who)
  {
    base.actionWhenStopBeingHeld(who);
    if (who.UsingTool)
    {
      who.UsingTool = false;
      if (who.FarmerSprite.PauseForSingleAnimation)
        who.FarmerSprite.PauseForSingleAnimation = false;
    }
    if (!who.IsLocalPlayer)
      return;
    foreach (BaseEnchantment enchantment in this.enchantments)
      enchantment.OnUnequip(who);
  }

  public virtual bool CanUseOnStandingTile() => false;

  public override void AddEquipmentEffects(BuffEffects effects)
  {
    base.AddEquipmentEffects(effects);
    if (!this.hasEnchantmentOfType<MasterEnchantment>())
      return;
    ++effects.FishingLevel.Value;
  }

  public virtual bool CanForge(Item item)
  {
    BaseEnchantment enchantmentFromItem = BaseEnchantment.GetEnchantmentFromItem((Item) this, item);
    return enchantmentFromItem != null && this.CanAddEnchantment(enchantmentFromItem) || item != null && item.QualifiedItemId == "(O)852" && this is MeleeWeapon meleeWeapon && meleeWeapon.getItemLevel() < 15 && !this.Name.Contains("Galaxy");
  }

  public T GetEnchantmentOfType<T>() where T : BaseEnchantment
  {
    foreach (BaseEnchantment enchantment in this.enchantments)
    {
      if (enchantment.GetType() == typeof (T))
        return enchantment as T;
    }
    return default (T);
  }

  public int GetEnchantmentLevel<T>() where T : BaseEnchantment
  {
    int enchantmentLevel = 0;
    foreach (BaseEnchantment enchantment in this.enchantments)
    {
      if (enchantment.GetType() == typeof (T))
        enchantmentLevel += enchantment.GetLevel();
    }
    return enchantmentLevel;
  }

  public virtual bool Forge(Item item, bool count_towards_stats = false)
  {
    BaseEnchantment enchantmentFromItem = BaseEnchantment.GetEnchantmentFromItem((Item) this, item);
    if (enchantmentFromItem != null)
    {
      if (this.AddEnchantment(enchantmentFromItem))
      {
        if (!(enchantmentFromItem is DiamondEnchantment))
        {
          if (enchantmentFromItem is GalaxySoulEnchantment && this is MeleeWeapon meleeWeapon && meleeWeapon.isGalaxyWeapon() && meleeWeapon.GetEnchantmentLevel<GalaxySoulEnchantment>() >= 3)
          {
            string newItemId = (string) null;
            switch (this.QualifiedItemId)
            {
              case "(W)4":
                newItemId = "62";
                break;
              case "(W)29":
                newItemId = "63";
                break;
              case "(W)23":
                newItemId = "64";
                break;
            }
            if (newItemId != null)
            {
              meleeWeapon.transform(newItemId);
              if (count_towards_stats)
              {
                DelayedAction.playSoundAfterDelay("discoverMineral", 400);
                Game1.multiplayer.globalChatInfoMessage("InfinityWeapon", Game1.player.name.Value, TokenStringBuilder.ItemNameFor((Item) this));
                Game1.getAchievement(42);
              }
            }
            GalaxySoulEnchantment enchantmentOfType = this.GetEnchantmentOfType<GalaxySoulEnchantment>();
            if (enchantmentOfType != null)
              this.RemoveEnchantment((BaseEnchantment) enchantmentOfType);
          }
        }
        else
        {
          int num1 = this.GetMaxForges() - this.GetTotalForgeLevels();
          List<int> intList = new List<int>();
          if (!this.hasEnchantmentOfType<EmeraldEnchantment>())
            intList.Add(0);
          if (!this.hasEnchantmentOfType<AquamarineEnchantment>())
            intList.Add(1);
          if (!this.hasEnchantmentOfType<RubyEnchantment>())
            intList.Add(2);
          if (!this.hasEnchantmentOfType<AmethystEnchantment>())
            intList.Add(3);
          if (!this.hasEnchantmentOfType<TopazEnchantment>())
            intList.Add(4);
          if (!this.hasEnchantmentOfType<JadeEnchantment>())
            intList.Add(5);
          for (int index1 = 0; index1 < num1 && intList.Count != 0; ++index1)
          {
            int index2 = Game1.random.Next(intList.Count);
            int num2 = intList[index2];
            intList.RemoveAt(index2);
            switch (num2)
            {
              case 0:
                this.AddEnchantment((BaseEnchantment) new EmeraldEnchantment());
                break;
              case 1:
                this.AddEnchantment((BaseEnchantment) new AquamarineEnchantment());
                break;
              case 2:
                this.AddEnchantment((BaseEnchantment) new RubyEnchantment());
                break;
              case 3:
                this.AddEnchantment((BaseEnchantment) new AmethystEnchantment());
                break;
              case 4:
                this.AddEnchantment((BaseEnchantment) new TopazEnchantment());
                break;
              case 5:
                this.AddEnchantment((BaseEnchantment) new JadeEnchantment());
                break;
            }
          }
        }
        if (count_towards_stats && !enchantmentFromItem.IsForge())
        {
          this.previousEnchantments.Insert(0, enchantmentFromItem.GetName());
          while (this.previousEnchantments.Count > 2)
            this.previousEnchantments.RemoveAt(this.previousEnchantments.Count - 1);
          int num = (int) Game1.stats.Increment("timesEnchanted");
        }
        return true;
      }
    }
    else if (item.QualifiedItemId == "(O)852" && this is MeleeWeapon meleeWeapon1)
    {
      List<BaseEnchantment> oldEnchantments = new List<BaseEnchantment>();
      meleeWeapon1.enchantments.RemoveWhere((Func<BaseEnchantment, bool>) (curEnchantment =>
      {
        if (!curEnchantment.IsSecondaryEnchantment() || curEnchantment is GalaxySoulEnchantment)
          return false;
        oldEnchantments.Add(curEnchantment);
        return true;
      }));
      MeleeWeapon.attemptAddRandomInnateEnchantment((Item) meleeWeapon1, Game1.random, true, oldEnchantments);
      return true;
    }
    return false;
  }
}
