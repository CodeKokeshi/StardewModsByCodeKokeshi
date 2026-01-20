// Decompiled with JetBrains decompiler
// Type: StardewValley.Objects.Ring
// Assembly: Stardew Valley, Version=1.6.15.24356, Culture=neutral, PublicKeyToken=null
// MVID: AA1F513A-94F0-4EF6-A35F-2D5B4A3721BF
// Assembly location: D:\SteamLibrary\steamapps\common\Stardew Valley\Stardew Valley.dll
// XML documentation location: D:\SteamLibrary\steamapps\common\Stardew Valley\Stardew Valley.xml

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Netcode;
using StardewValley.Buffs;
using StardewValley.Extensions;
using StardewValley.GameData.Objects;
using StardewValley.ItemTypeDefinitions;
using StardewValley.Locations;
using StardewValley.Monsters;
using System;
using System.Text;
using System.Xml.Serialization;

#nullable disable
namespace StardewValley.Objects;

[XmlInclude(typeof (CombinedRing))]
public class Ring : Item
{
  public const string SmallGlowRingId = "516";
  public const string GlowRingId = "517";
  public const string SmallMagnetRingId = "518";
  public const string MagnetRingId = "519";
  public const string SlimeCharmerRingId = "520";
  public const string WarriorRingId = "521";
  public const string VampireRingId = "522";
  public const string SavageRingId = "523";
  public const string YobaRingId = "524";
  public const string SturdyRingId = "525";
  public const string BurglarsRingId = "526";
  public const string IridiumBandId = "527";
  public const string AmethystRingId = "529";
  public const string TopazRingId = "530";
  public const string AquamarineRingId = "531";
  public const string JadeRingId = "532";
  public const string EmeraldRingId = "533";
  public const string RubyRingId = "534";
  public const string WeddingRingId = "801";
  public const string CrabshellRingId = "810";
  public const string NapalmRingId = "811";
  public const string ThornsRingId = "839";
  public const string LuckyRingId = "859";
  public const string HotJavaRingId = "860";
  public const string ProtectiveRingId = "861";
  public const string SoulSapperRingId = "862";
  public const string PhoenixRingId = "863";
  public const string CombinedRingId = "880";
  public const string ImmunityBandId = "887";
  public const string GlowstoneRingId = "888";
  [XmlElement("price")]
  public readonly NetInt price = new NetInt();
  /// <summary>Obsolete. This is only kept to preserve data from old save files, and isn't synchronized in multiplayer. Use <see cref="P:StardewValley.Item.ItemId" /> instead.</summary>
  [XmlElement("indexInTileSheet")]
  public int? obsolete_indexInTileSheet;
  [XmlIgnore]
  public string description;
  [XmlIgnore]
  public string displayName;
  [XmlIgnore]
  public string lightSourceId;

  /// <inheritdoc />
  public override string TypeDefinitionId { get; } = "(O)";

  /// <inheritdoc />
  protected override void MigrateLegacyItemId()
  {
    NetString itemId = this.itemId;
    ref int? local = ref this.obsolete_indexInTileSheet;
    int num;
    string str;
    if (!local.HasValue)
    {
      str = (string) null;
    }
    else
    {
      num = local.GetValueOrDefault();
      str = num.ToString();
    }
    if (str == null)
    {
      num = this.ParentSheetIndex;
      str = num.ToString();
    }
    itemId.Value = str;
    this.obsolete_indexInTileSheet = new int?();
  }

  public Ring()
  {
  }

  public Ring(string itemId)
    : this()
  {
    itemId = this.ValidateUnqualifiedItemId(itemId);
    ObjectData objectData = Game1.objectData[itemId];
    this.ItemId = itemId;
    this.Category = -96;
    this.Name = objectData.Name ?? ItemRegistry.GetDataOrErrorItem(this.QualifiedItemId).InternalName;
    this.price.Value = objectData.Price;
    this.ResetParentSheetIndex();
    this.loadDisplayFields();
  }

  /// <inheritdoc />
  protected override void initNetFields()
  {
    base.initNetFields();
    this.NetFields.AddField((INetSerializable) this.price, "price");
  }

  /// <inheritdoc />
  public override bool CanBeLostOnDeath() => false;

  /// <inheritdoc />
  public override void onEquip(Farmer who)
  {
    base.onEquip(who);
    GameLocation currentLocation = who.currentLocation;
    currentLocation.removeLightSource(this.lightSourceId);
    this.lightSourceId = (string) null;
    switch (this.ItemId)
    {
      case "516":
        this.lightSourceId = this.GenerateLightSourceId(who);
        currentLocation.sharedLights.AddLight(new LightSource(this.lightSourceId, 1, new Vector2(who.Position.X + 21f, who.Position.Y + 64f), 5f, new Color(0, 50, 170), playerID: who.UniqueMultiplayerID));
        break;
      case "517":
        this.lightSourceId = this.GenerateLightSourceId(who);
        currentLocation.sharedLights.AddLight(new LightSource(this.lightSourceId, 1, new Vector2(who.Position.X + 21f, who.Position.Y + 64f), 10f, new Color(0, 30, 150), playerID: who.UniqueMultiplayerID));
        break;
      case "888":
      case "527":
        this.lightSourceId = this.GenerateLightSourceId(who);
        currentLocation.sharedLights.AddLight(new LightSource(this.lightSourceId, 1, new Vector2(who.Position.X + 21f, who.Position.Y + 64f), 10f, new Color(0, 80 /*0x50*/, 0), playerID: who.UniqueMultiplayerID));
        break;
    }
  }

  /// <inheritdoc />
  public override void onUnequip(Farmer who)
  {
    base.onUnequip(who);
    string itemId = this.ItemId;
    if (!(itemId == "516") && !(itemId == "517") && !(itemId == "888") && !(itemId == "527"))
      return;
    who.currentLocation.removeLightSource(this.lightSourceId);
    this.lightSourceId = (string) null;
  }

  public override void AddEquipmentEffects(BuffEffects effects)
  {
    base.AddEquipmentEffects(effects);
    string itemId = this.ItemId;
    if (itemId == null || itemId.Length != 3)
      return;
    switch (itemId[2])
    {
      case '0':
        switch (itemId)
        {
          case "530":
            ++effects.Defense.Value;
            return;
          case "810":
            effects.Defense.Value += 5f;
            return;
          default:
            return;
        }
      case '1':
        if (!(itemId == "531"))
          break;
        effects.CriticalChanceMultiplier.Value += 0.1f;
        break;
      case '2':
        if (!(itemId == "532"))
          break;
        effects.CriticalPowerMultiplier.Value += 0.1f;
        break;
      case '3':
        if (!(itemId == "533"))
          break;
        effects.WeaponSpeedMultiplier.Value += 0.1f;
        break;
      case '4':
        if (!(itemId == "534"))
          break;
        effects.AttackMultiplier.Value += 0.1f;
        break;
      case '7':
        switch (itemId)
        {
          case "527":
            effects.MagneticRadius.Value += 128f;
            effects.AttackMultiplier.Value += 0.1f;
            return;
          case "887":
            effects.Immunity.Value += 4f;
            return;
          default:
            return;
        }
      case '8':
        switch (itemId)
        {
          case "518":
            effects.MagneticRadius.Value += 64f;
            return;
          case "888":
            effects.MagneticRadius.Value += 128f;
            return;
          default:
            return;
        }
      case '9':
        switch (itemId)
        {
          case "519":
            effects.MagneticRadius.Value += 128f;
            return;
          case "529":
            effects.KnockbackMultiplier.Value += 0.1f;
            return;
          case "859":
            ++effects.LuckLevel.Value;
            return;
          default:
            return;
        }
    }
  }

  /// <inheritdoc />
  public override string getCategoryName() => StardewValley.Object.GetCategoryDisplayName(-96);

  public virtual void onNewLocation(Farmer who, GameLocation environment)
  {
    environment.removeLightSource(this.lightSourceId);
    this.lightSourceId = (string) null;
    switch (this.ItemId)
    {
      case "516":
      case "517":
        GameLocation currentLocation = who.currentLocation;
        who.currentLocation = environment;
        this.onEquip(who);
        who.currentLocation = currentLocation;
        break;
      case "888":
      case "527":
        this.lightSourceId = this.GenerateLightSourceId(who);
        environment.sharedLights.AddLight(new LightSource(this.lightSourceId, 1, new Vector2(who.Position.X + 21f, who.Position.Y + 64f), 10f, new Color(0, 30, 150), playerID: who.UniqueMultiplayerID));
        break;
    }
  }

  public virtual void onLeaveLocation(Farmer who, GameLocation environment)
  {
    string itemId = this.ItemId;
    if (!(itemId == "516") && !(itemId == "517") && !(itemId == "527") && !(itemId == "888"))
      return;
    environment.removeLightSource(this.lightSourceId);
    this.lightSourceId = (string) null;
  }

  /// <inheritdoc />
  public override int salePrice(bool ignoreProfitMargins = false) => this.price.Value;

  /// <summary>Apply ring effects when a monster is slain.</summary>
  /// <param name="monster">The monster slain.</param>
  /// <param name="location">The location where the monster was slain.</param>
  /// <param name="who">The player receiving ring effects.</param>
  public virtual void onMonsterSlay(Monster monster, GameLocation location, Farmer who)
  {
    switch (this.ItemId)
    {
      case "811":
        if (monster != null && location != null)
        {
          location.explode(monster.Tile, 2, who, false, destroyObjects: !(location is Farm) && !(location is SlimeHutch));
          break;
        }
        break;
      case "860":
        if (Game1.random.NextBool(0.25))
        {
          if (monster != null)
          {
            monster.objectsToDrop.Add("395");
            break;
          }
          break;
        }
        if (Game1.random.NextBool(0.1) && monster != null)
        {
          monster.objectsToDrop.Add("253");
          break;
        }
        break;
    }
    if (!who.IsLocalPlayer)
      return;
    switch (this.ItemId)
    {
      case "521":
        if (!Game1.random.NextBool(0.1 + (double) who.LuckLevel / 100.0))
          break;
        who.applyBuff("20");
        Game1.playSound("warrior");
        break;
      case "522":
        who.health = Math.Min(who.maxHealth, who.health + 2);
        break;
      case "523":
        who.applyBuff("22");
        break;
      case "862":
        who.Stamina = Math.Min((float) who.MaxStamina, who.Stamina + 4f);
        break;
    }
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

  public virtual void update(GameTime time, GameLocation environment, Farmer who)
  {
    if (this.lightSourceId == null)
      return;
    Vector2 zero = Vector2.Zero;
    if (who.shouldShadowBeOffset)
      zero += who.drawOffset;
    environment.repositionLightSource(this.lightSourceId, new Vector2(who.Position.X + 21f, who.Position.Y) + zero);
    if (environment.isOutdoors.Value)
      return;
    switch (environment)
    {
      case MineShaft _:
        break;
      case VolcanoDungeon _:
        break;
      default:
        LightSource lightSource = environment.getLightSource(this.lightSourceId);
        if (lightSource == null)
          break;
        lightSource.radius.Value = 3f;
        break;
    }
  }

  public override int maximumStackSize() => 1;

  public override Point getExtraSpaceNeededForTooltipSpecialIcons(
    SpriteFont font,
    int minWidth,
    int horizontalBuffer,
    int startingHeight,
    StringBuilder descriptionText,
    string boldTitleText,
    int moneyAmountToDisplayAtBottom)
  {
    Point tooltipSpecialIcons = new Point(0, startingHeight);
    int num = 0;
    if (this.GetsEffectOfRing("810"))
      ++num;
    if (this.GetsEffectOfRing("887") || this.GetsEffectOfRing("530"))
      ++num;
    if (this.GetsEffectOfRing("859"))
      ++num;
    tooltipSpecialIcons.X = (int) Math.Max((float) minWidth, font.MeasureString(Game1.content.LoadString("Strings\\UI:ItemHover_DefenseBonus", (object) 9999)).X + (float) horizontalBuffer);
    tooltipSpecialIcons.Y += num * Math.Max((int) font.MeasureString("TT").Y, 48 /*0x30*/);
    return tooltipSpecialIcons;
  }

  public virtual bool GetsEffectOfRing(string ringId) => this.ItemId == ringId;

  public virtual int GetEffectsOfRingMultiplier(string ringId)
  {
    return this.GetsEffectOfRing(ringId) ? 1 : 0;
  }

  public override void drawTooltip(
    SpriteBatch spriteBatch,
    ref int x,
    ref int y,
    SpriteFont font,
    float alpha,
    StringBuilder overrideText)
  {
    if (this.description == null)
      this.loadDisplayFields();
    Utility.drawTextWithShadow(spriteBatch, Game1.parseText(this.description, Game1.smallFont, this.getDescriptionWidth()), font, new Vector2((float) (x + 16 /*0x10*/), (float) (y + 16 /*0x10*/ + 4)), Game1.textColor);
    y += (int) font.MeasureString(Game1.parseText(this.description, Game1.smallFont, this.getDescriptionWidth())).Y;
    if (this.GetsEffectOfRing("810") || this.GetsEffectOfRing("530"))
    {
      Utility.drawWithShadow(spriteBatch, Game1.mouseCursors, new Vector2((float) (x + 16 /*0x10*/ + 4), (float) (y + 16 /*0x10*/ + 4)), new Rectangle(110, 428, 10, 10), Color.White, 0.0f, Vector2.Zero, 4f, layerDepth: 1f);
      Utility.drawTextWithShadow(spriteBatch, Game1.content.LoadString("Strings\\UI:ItemHover_DefenseBonus", (object) (this.GetsEffectOfRing("810") ? 5 * this.GetEffectsOfRingMultiplier("810") : this.GetEffectsOfRingMultiplier("530"))), font, new Vector2((float) (x + 16 /*0x10*/ + 52), (float) (y + 16 /*0x10*/ + 12)), Game1.textColor * 0.9f * alpha);
      y += (int) Math.Max(font.MeasureString("TT").Y, 48f);
    }
    if (this.GetsEffectOfRing("887"))
    {
      Utility.drawWithShadow(spriteBatch, Game1.mouseCursors, new Vector2((float) (x + 16 /*0x10*/ + 4), (float) (y + 16 /*0x10*/ + 4)), new Rectangle(150, 428, 10, 10), Color.White, 0.0f, Vector2.Zero, 4f, layerDepth: 1f);
      Utility.drawTextWithShadow(spriteBatch, Game1.content.LoadString("Strings\\UI:ItemHover_ImmunityBonus", (object) (4 * this.GetEffectsOfRingMultiplier("887"))), font, new Vector2((float) (x + 16 /*0x10*/ + 52), (float) (y + 16 /*0x10*/ + 12)), Game1.textColor * 0.9f * alpha);
      y += (int) Math.Max(font.MeasureString("TT").Y, 48f);
    }
    if (!this.GetsEffectOfRing("859"))
      return;
    Utility.drawWithShadow(spriteBatch, Game1.mouseCursors, new Vector2((float) (x + 16 /*0x10*/ + 4), (float) (y + 16 /*0x10*/ + 4)), new Rectangle(50, 428, 10, 10), Color.White, 0.0f, Vector2.Zero, 4f, layerDepth: 1f);
    Utility.drawTextWithShadow(spriteBatch, "+" + Game1.content.LoadString("Strings\\UI:ItemHover_Buff4", (object) this.GetEffectsOfRingMultiplier("859")), font, new Vector2((float) (x + 16 /*0x10*/ + 52), (float) (y + 16 /*0x10*/ + 12)), Game1.textColor * 0.9f * alpha);
    y += (int) Math.Max(font.MeasureString("TT").Y, 48f);
  }

  public override string getDescription()
  {
    if (this.description == null)
      this.loadDisplayFields();
    return Game1.parseText(this.description, Game1.smallFont, this.getDescriptionWidth());
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
  protected override Item GetOneNew() => (Item) new Ring(this.ItemId);

  protected virtual bool loadDisplayFields()
  {
    ParsedItemData dataOrErrorItem = ItemRegistry.GetDataOrErrorItem(this.QualifiedItemId);
    this.displayName = dataOrErrorItem.DisplayName;
    this.description = dataOrErrorItem.Description;
    return true;
  }

  public virtual bool CanCombine(Ring ring)
  {
    return !(ring is CombinedRing) && !(this is CombinedRing) && !(this.QualifiedItemId == ring.QualifiedItemId);
  }

  public Ring Combine(Ring ring)
  {
    return (Ring) new CombinedRing()
    {
      combinedRings = {
        this.getOne() as Ring,
        ring.getOne() as Ring
      }
    };
  }
}
