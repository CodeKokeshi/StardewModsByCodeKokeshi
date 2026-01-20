// Decompiled with JetBrains decompiler
// Type: StardewValley.Tools.MeleeWeapon
// Assembly: Stardew Valley, Version=1.6.15.24356, Culture=neutral, PublicKeyToken=null
// MVID: AA1F513A-94F0-4EF6-A35F-2D5B4A3721BF
// Assembly location: D:\SteamLibrary\steamapps\common\Stardew Valley\Stardew Valley.dll
// XML documentation location: D:\SteamLibrary\steamapps\common\Stardew Valley\Stardew Valley.xml

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Netcode;
using StardewValley.Buffs;
using StardewValley.Enchantments;
using StardewValley.Extensions;
using StardewValley.GameData;
using StardewValley.GameData.Weapons;
using StardewValley.Internal;
using StardewValley.ItemTypeDefinitions;
using StardewValley.Menus;
using StardewValley.Projectiles;
using StardewValley.TerrainFeatures;
using System;
using System.Collections.Generic;
using System.Text;
using System.Xml.Serialization;

#nullable disable
namespace StardewValley.Tools;

public class MeleeWeapon : Tool
{
  public const int defenseCooldownTime = 1500;
  public const int daggerCooldownTime = 3000;
  public const int clubCooldownTime = 6000;
  public const int millisecondsPerSpeedPoint = 40;
  public const int defaultSpeed = 400;
  public const int stabbingSword = 0;
  public const int dagger = 1;
  public const int club = 2;
  public const int defenseSword = 3;
  public const int baseClubSpeed = -8;
  public const string scytheId = "47";
  public const string goldenScytheId = "53";
  public const string iridiumScytheID = "66";
  public const string galaxySwordId = "4";
  public const int MAX_FORGES = 3;
  [XmlElement("type")]
  public readonly NetInt type = new NetInt();
  [XmlElement("minDamage")]
  public readonly NetInt minDamage = new NetInt();
  [XmlElement("maxDamage")]
  public readonly NetInt maxDamage = new NetInt();
  [XmlElement("speed")]
  public readonly NetInt speed = new NetInt();
  [XmlElement("addedPrecision")]
  public readonly NetInt addedPrecision = new NetInt();
  [XmlElement("addedDefense")]
  public readonly NetInt addedDefense = new NetInt();
  [XmlElement("addedAreaOfEffect")]
  public readonly NetInt addedAreaOfEffect = new NetInt();
  [XmlElement("knockback")]
  public readonly NetFloat knockback = new NetFloat();
  [XmlElement("critChance")]
  public readonly NetFloat critChance = new NetFloat();
  [XmlElement("critMultiplier")]
  public readonly NetFloat critMultiplier = new NetFloat();
  /// <summary>The qualified item ID for the item whose appearance to use, or <c>null</c> to use the weapon's default appearance.</summary>
  [XmlElement("appearance")]
  public readonly NetString appearance = new NetString((string) null);
  public bool isOnSpecial;
  public static int defenseCooldown;
  public static int attackSwordCooldown;
  public static int daggerCooldown;
  public static int clubCooldown;
  public static int daggerHitsLeft;
  public static int timedHitTimer;
  private static float addedSwordScale = 0.0f;
  private static float addedClubScale = 0.0f;
  private static float addedDaggerScale = 0.0f;
  private float swipeSpeed;
  [XmlIgnore]
  public Rectangle mostRecentArea;
  [XmlIgnore]
  private readonly NetEvent0 animateSpecialMoveEvent = new NetEvent0();
  [XmlIgnore]
  private readonly NetEvent0 defenseSwordEvent = new NetEvent0();
  [XmlIgnore]
  private readonly NetEvent1Field<int, NetInt> daggerEvent = new NetEvent1Field<int, NetInt>();
  private WeaponData cachedData;
  private bool anotherClick;
  private static Vector2 center = new Vector2(1f, 15f);

  /// <inheritdoc />
  public override string TypeDefinitionId { get; } = "(W)";

  public MeleeWeapon() => this.Category = -98;

  public MeleeWeapon(string itemId)
    : this()
  {
    itemId = this.ValidateUnqualifiedItemId(itemId);
    this.ItemId = itemId;
    this.Stack = 1;
    this.ReloadData();
  }

  protected void ReloadData()
  {
    ParsedItemData dataOrErrorItem = ItemRegistry.GetDataOrErrorItem(this.QualifiedItemId);
    WeaponData data;
    if (MeleeWeapon.TryGetData(this.itemId.Value, out data))
    {
      this.cachedData = data;
      this.Name = data.Name ?? dataOrErrorItem.InternalName;
      this.minDamage.Value = data.MinDamage;
      this.maxDamage.Value = data.MaxDamage;
      this.knockback.Value = data.Knockback;
      this.speed.Value = data.Speed;
      this.addedPrecision.Value = data.Precision;
      this.addedDefense.Value = data.Defense;
      this.type.Value = data.Type;
      this.addedAreaOfEffect.Value = data.AreaOfEffect;
      this.critChance.Value = data.CritChance;
      this.critMultiplier.Value = data.CritMultiplier;
      if (this.type.Value == 0)
        this.type.Value = 3;
    }
    else
      this.Name = "Error Item";
    this.InitialParentTileIndex = dataOrErrorItem.SpriteIndex;
    this.CurrentParentTileIndex = dataOrErrorItem.SpriteIndex;
    this.IndexOfMenuItemView = dataOrErrorItem.SpriteIndex;
    this.Category = this.isScythe() ? -99 : -98;
  }

  /// <inheritdoc />
  protected override void MigrateLegacyItemId()
  {
    this.ItemId = this.InitialParentTileIndex.ToString();
  }

  /// <summary>Get the weapon's data from <see cref="F:StardewValley.Game1.weaponData" />, if found.</summary>
  public WeaponData GetData()
  {
    if (this.cachedData == null)
      MeleeWeapon.TryGetData(this.ItemId, out this.cachedData);
    return this.cachedData;
  }

  /// <summary>Try to get a weapon's data from <see cref="F:StardewValley.Game1.weaponData" />.</summary>
  /// <param name="itemId">The weapon's unqualified item ID (i.e. the key in <see cref="F:StardewValley.Game1.weaponData" />).</param>
  /// <param name="data">The weapon data, if found.</param>
  /// <returns>Returns whether the crop data was found.</returns>
  public static bool TryGetData(string itemId, out WeaponData data)
  {
    if (itemId != null)
      return Game1.weaponData.TryGetValue(itemId, out data);
    data = (WeaponData) null;
    return false;
  }

  /// <inheritdoc />
  public override bool CanBeLostOnDeath()
  {
    if (!base.CanBeLostOnDeath())
      return false;
    WeaponData data = this.GetData();
    return data == null || data.CanBeLostOnDeath;
  }

  public override void AddEquipmentEffects(BuffEffects effects)
  {
    base.AddEquipmentEffects(effects);
    effects.Defense.Value += (float) this.addedDefense.Value;
    foreach (BaseEnchantment enchantment in this.enchantments)
      enchantment.AddEquipmentEffects(effects);
  }

  public override int GetMaxForges() => 3;

  /// <inheritdoc />
  protected override Item GetOneNew() => (Item) new MeleeWeapon(this.ItemId);

  /// <inheritdoc />
  protected override void GetOneCopyFrom(Item source)
  {
    base.GetOneCopyFrom(source);
    if (!(source is MeleeWeapon meleeWeapon))
      return;
    this.appearance.Value = meleeWeapon.appearance.Value;
    this.IndexOfMenuItemView = meleeWeapon.IndexOfMenuItemView;
  }

  protected override string loadDisplayName()
  {
    return ItemRegistry.GetDataOrErrorItem(this.QualifiedItemId).DisplayName;
  }

  protected override string loadDescription()
  {
    return ItemRegistry.GetDataOrErrorItem(this.QualifiedItemId).Description;
  }

  /// <inheritdoc />
  public override string getCategoryName()
  {
    if (this.isScythe())
      return base.getCategoryName();
    string path;
    switch (this.type.Value)
    {
      case 1:
        path = "Strings\\StringsFromCSFiles:Tool.cs.14304";
        break;
      case 2:
        path = "Strings\\StringsFromCSFiles:Tool.cs.14305";
        break;
      default:
        path = "Strings\\StringsFromCSFiles:Tool.cs.14306";
        break;
    }
    return Game1.content.LoadString("Strings\\StringsFromCSFiles:Tool.cs.14303", (object) this.getItemLevel(), (object) Game1.content.LoadString(path));
  }

  protected override void initNetFields()
  {
    base.initNetFields();
    this.NetFields.AddField((INetSerializable) this.type, "type").AddField((INetSerializable) this.minDamage, "minDamage").AddField((INetSerializable) this.maxDamage, "maxDamage").AddField((INetSerializable) this.speed, "speed").AddField((INetSerializable) this.addedPrecision, "addedPrecision").AddField((INetSerializable) this.addedDefense, "addedDefense").AddField((INetSerializable) this.addedAreaOfEffect, "addedAreaOfEffect").AddField((INetSerializable) this.knockback, "knockback").AddField((INetSerializable) this.critChance, "critChance").AddField((INetSerializable) this.critMultiplier, "critMultiplier").AddField((INetSerializable) this.appearance, "appearance").AddField((INetSerializable) this.animateSpecialMoveEvent, "animateSpecialMoveEvent").AddField((INetSerializable) this.defenseSwordEvent, "defenseSwordEvent").AddField((INetSerializable) this.daggerEvent, "daggerEvent");
    this.animateSpecialMoveEvent.onEvent += new NetEvent0.Event(this.doAnimateSpecialMove);
    this.defenseSwordEvent.onEvent += new NetEvent0.Event(this.doDefenseSwordFunction);
    this.daggerEvent.onEvent += new AbstractNetEvent1<int>.Event(this.doDaggerFunction);
    this.itemId.fieldChangeVisibleEvent += (FieldChange<NetString, string>) ((a, b, c) => this.ReloadData());
  }

  public override string checkForSpecialItemHoldUpMeessage()
  {
    return this.QualifiedItemId == "(W)4" ? Game1.content.LoadString("Strings\\StringsFromCSFiles:MeleeWeapon.cs.14122") : (string) null;
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
    float num1 = 0.0f;
    float num2 = 0.0f;
    if (!this.isScythe())
    {
      switch (this.type.Value)
      {
        case 0:
        case 3:
          if (MeleeWeapon.defenseCooldown > 0)
            num1 = (float) MeleeWeapon.defenseCooldown / 1500f;
          num2 = MeleeWeapon.addedSwordScale;
          break;
        case 1:
          if (MeleeWeapon.daggerCooldown > 0)
            num1 = (float) MeleeWeapon.daggerCooldown / 3000f;
          num2 = MeleeWeapon.addedDaggerScale;
          break;
        case 2:
          if (MeleeWeapon.clubCooldown > 0)
            num1 = (float) MeleeWeapon.clubCooldown / 6000f;
          num2 = MeleeWeapon.addedClubScale;
          break;
      }
    }
    bool flag = drawShadow && drawStackNumber == StackDrawType.Hide;
    if (!drawShadow | flag)
      num2 = 0.0f;
    ParsedItemData dataOrErrorItem = ItemRegistry.GetDataOrErrorItem(this.GetDrawnItemId());
    Texture2D texture = dataOrErrorItem.GetTexture();
    Rectangle sourceRect = dataOrErrorItem.GetSourceRect();
    spriteBatch.Draw(texture, location + (this.type.Value == 1 ? new Vector2(38f, 25f) : new Vector2(32f, 32f)), new Rectangle?(sourceRect), color * transparency, 0.0f, new Vector2(8f, 8f), (float) (4.0 * ((double) scaleSize + (double) num2)), SpriteEffects.None, layerDepth);
    if ((double) num1 > 0.0 & drawShadow && !flag && !this.isScythe() && (Game1.activeClickableMenu == null || !(Game1.activeClickableMenu is ShopMenu) || (double) scaleSize != 1.0))
      spriteBatch.Draw(Game1.staminaRect, new Rectangle((int) location.X, (int) location.Y + (64 /*0x40*/ - (int) ((double) num1 * 64.0)), 64 /*0x40*/, (int) ((double) num1 * 64.0)), Color.Red * 0.66f);
    this.DrawMenuIcons(spriteBatch, location, scaleSize, transparency, layerDepth, drawStackNumber, color);
  }

  public override int maximumStackSize() => 1;

  /// <inheritdoc />
  public override int salePrice(bool ignoreProfitMargins = false)
  {
    return !MeleeWeapon.IsScythe(this.itemId.Value) ? this.getItemLevel() * 100 : 0;
  }

  public static void weaponsTypeUpdate(GameTime time)
  {
    if ((double) MeleeWeapon.addedSwordScale > 0.0)
      MeleeWeapon.addedSwordScale -= 0.01f;
    if ((double) MeleeWeapon.addedClubScale > 0.0)
      MeleeWeapon.addedClubScale -= 0.01f;
    if ((double) MeleeWeapon.addedDaggerScale > 0.0)
      MeleeWeapon.addedDaggerScale -= 0.01f;
    if ((double) MeleeWeapon.timedHitTimer > 0.0)
      MeleeWeapon.timedHitTimer -= (int) time.ElapsedGameTime.TotalMilliseconds;
    if (MeleeWeapon.defenseCooldown > 0)
    {
      MeleeWeapon.defenseCooldown -= time.ElapsedGameTime.Milliseconds;
      if (MeleeWeapon.defenseCooldown <= 0)
      {
        MeleeWeapon.addedSwordScale = 0.5f;
        Game1.playSound("objectiveComplete");
      }
    }
    if (MeleeWeapon.attackSwordCooldown > 0)
    {
      MeleeWeapon.attackSwordCooldown -= time.ElapsedGameTime.Milliseconds;
      if (MeleeWeapon.attackSwordCooldown <= 0)
      {
        MeleeWeapon.addedSwordScale = 0.5f;
        Game1.playSound("objectiveComplete");
      }
    }
    if (MeleeWeapon.daggerCooldown > 0)
    {
      MeleeWeapon.daggerCooldown -= time.ElapsedGameTime.Milliseconds;
      if (MeleeWeapon.daggerCooldown <= 0)
      {
        MeleeWeapon.addedDaggerScale = 0.5f;
        Game1.playSound("objectiveComplete");
      }
    }
    if (MeleeWeapon.clubCooldown <= 0)
      return;
    MeleeWeapon.clubCooldown -= time.ElapsedGameTime.Milliseconds;
    if (MeleeWeapon.clubCooldown > 0)
      return;
    MeleeWeapon.addedClubScale = 0.5f;
    Game1.playSound("objectiveComplete");
  }

  public override void tickUpdate(GameTime time, Farmer who)
  {
    this.lastUser = who;
    base.tickUpdate(time, who);
    this.animateSpecialMoveEvent.Poll();
    this.defenseSwordEvent.Poll();
    this.daggerEvent.Poll();
    if (this.isOnSpecial && this.type.Value == 1 && MeleeWeapon.daggerHitsLeft > 0 && !who.UsingTool)
    {
      this.quickStab(who);
      this.triggerDaggerFunction(who, MeleeWeapon.daggerHitsLeft);
    }
    if (!this.anotherClick)
      return;
    this.leftClick(who);
  }

  public override bool doesShowTileLocationMarker() => false;

  public int getNumberOfDescriptionCategories()
  {
    int descriptionCategories = 1;
    if (this.speed.Value != (this.type.Value == 2 ? -8 : 0))
      ++descriptionCategories;
    if (this.addedDefense.Value > 0)
      ++descriptionCategories;
    float num = this.critChance.Value;
    if (this.type.Value == 1)
      num = (num + 0.005f) * 1.12f;
    if ((double) num / 0.02 >= 1.1000000238418579)
      ++descriptionCategories;
    if (((double) this.critMultiplier.Value - 3.0) / 0.02 >= 1.0)
      ++descriptionCategories;
    if ((double) this.knockback.Value != (double) this.defaultKnockBackForThisType(this.type.Value))
      ++descriptionCategories;
    if (this.enchantments.Count > 0 && this.enchantments[this.enchantments.Count - 1] is DiamondEnchantment)
      ++descriptionCategories;
    return descriptionCategories;
  }

  public override void leftClick(Farmer who)
  {
    if (who.health <= 0 || Game1.activeClickableMenu != null || Game1.farmEvent != null || Game1.eventUp || who.swimming.Value || who.bathingClothes.Value || who.onBridge.Value)
      return;
    if (!this.isScythe() && who.FarmerSprite.currentAnimationIndex > (this.type.Value == 2 ? 5 : (this.type.Value == 1 ? 0 : 5)))
    {
      who.completelyStopAnimatingOrDoingAction();
      who.CanMove = false;
      who.UsingTool = true;
      who.canReleaseTool = true;
      this.setFarmerAnimating(who);
    }
    else
    {
      if (this.isScythe() || who.FarmerSprite.currentAnimationIndex <= (this.type.Value == 2 ? 3 : (this.type.Value == 1 ? 0 : 3)))
        return;
      this.anotherClick = true;
    }
  }

  /// <inheritdoc />
  public override bool isScythe() => MeleeWeapon.IsScythe(this.QualifiedItemId);

  /// <summary>Get whether an item ID matches a scythe tool.</summary>
  /// <param name="id">The item ID.</param>
  public static bool IsScythe(string id)
  {
    return id == "(W)47" || id == "(W)53" || id == "(W)66" || id == "47" || id == "53" || id == "66";
  }

  public virtual int getItemLevel()
  {
    float num = 0.0f + (float) (int) ((double) ((this.maxDamage.Value + this.minDamage.Value) / 2) * (1.0 + 0.03 * (double) (Math.Max(0, this.speed.Value) + (this.type.Value == 1 ? 15 : 0)))) + (float) (int) ((double) (this.addedPrecision.Value / 2 + this.addedDefense.Value) + ((double) this.critChance.Value - 0.02) * 200.0 + ((double) this.critMultiplier.Value - 3.0) * 6.0);
    switch (this.QualifiedItemId)
    {
      case "(W)2":
        num += 20f;
        break;
      case "(W)3":
        num += 15f;
        break;
    }
    return (int) ((double) (num + (float) (this.addedDefense.Value * 2)) / 7.0 + 1.0);
  }

  public static Item attemptAddRandomInnateEnchantment(
    Item item,
    Random r,
    bool force = false,
    List<BaseEnchantment> enchantsToReroll = null)
  {
    if (r == null)
      r = Game1.random;
    if (item is MeleeWeapon meleeWeapon5 && (force || r.NextBool()))
    {
      while (true)
      {
        int itemLevel = meleeWeapon5.getItemLevel();
        if (r.NextDouble() < 0.125 && itemLevel <= 10)
        {
          MeleeWeapon meleeWeapon = meleeWeapon5;
          DefenseEnchantment enchantment = new DefenseEnchantment();
          enchantment.Level = Math.Max(1, Math.Min(2, r.Next(itemLevel + 1) / 2 + 1));
          meleeWeapon.AddEnchantment((BaseEnchantment) enchantment);
        }
        else if (r.NextDouble() < 0.125)
        {
          MeleeWeapon meleeWeapon = meleeWeapon5;
          LightweightEnchantment enchantment = new LightweightEnchantment();
          enchantment.Level = r.Next(1, 6);
          meleeWeapon.AddEnchantment((BaseEnchantment) enchantment);
        }
        else if (r.NextDouble() < 0.125)
          meleeWeapon5.AddEnchantment((BaseEnchantment) new SlimeGathererEnchantment());
        switch (r.Next(5))
        {
          case 0:
            MeleeWeapon meleeWeapon1 = meleeWeapon5;
            AttackEnchantment enchantment1 = new AttackEnchantment();
            enchantment1.Level = Math.Max(1, Math.Min(5, r.Next(itemLevel + 1) / 2 + 1));
            meleeWeapon1.AddEnchantment((BaseEnchantment) enchantment1);
            break;
          case 1:
            MeleeWeapon meleeWeapon2 = meleeWeapon5;
            CritEnchantment enchantment2 = new CritEnchantment();
            enchantment2.Level = Math.Max(1, Math.Min(3, r.Next(itemLevel) / 3));
            meleeWeapon2.AddEnchantment((BaseEnchantment) enchantment2);
            break;
          case 2:
            MeleeWeapon meleeWeapon3 = meleeWeapon5;
            WeaponSpeedEnchantment enchantment3 = new WeaponSpeedEnchantment();
            enchantment3.Level = Math.Max(1, Math.Min(Math.Max(1, 4 - meleeWeapon5.speed.Value), r.Next(itemLevel)));
            meleeWeapon3.AddEnchantment((BaseEnchantment) enchantment3);
            break;
          case 3:
            meleeWeapon5.AddEnchantment((BaseEnchantment) new SlimeSlayerEnchantment());
            break;
          case 4:
            MeleeWeapon meleeWeapon4 = meleeWeapon5;
            CritPowerEnchantment enchantment4 = new CritPowerEnchantment();
            enchantment4.Level = Math.Max(1, Math.Min(3, r.Next(itemLevel) / 3));
            meleeWeapon4.AddEnchantment((BaseEnchantment) enchantment4);
            break;
        }
        if (enchantsToReroll != null)
        {
          bool flag = false;
          foreach (BaseEnchantment baseEnchantment in enchantsToReroll)
          {
            foreach (BaseEnchantment enchantment5 in meleeWeapon5.enchantments)
            {
              if (baseEnchantment.GetType().Equals(enchantment5.GetType()))
              {
                flag = true;
                break;
              }
            }
            if (flag)
              break;
          }
          if (flag)
            meleeWeapon5.enchantments.RemoveWhere((Func<BaseEnchantment, bool>) (enchantment => enchantment.IsSecondaryEnchantment() && !(enchantment is GalaxySoulEnchantment)));
          else
            break;
        }
        else
          break;
      }
    }
    return item;
  }

  public override string getDescription()
  {
    if (this.isScythe())
      return Game1.parseText(this.description, Game1.smallFont, this.getDescriptionWidth());
    StringBuilder stringBuilder = new StringBuilder();
    stringBuilder.AppendLine(Game1.parseText(this.description, Game1.smallFont, this.getDescriptionWidth()));
    stringBuilder.AppendLine();
    stringBuilder.AppendLine(Game1.content.LoadString("Strings\\StringsFromCSFiles:MeleeWeapon.cs.14132", (object) this.minDamage, (object) this.maxDamage));
    if (this.speed.Value != 0)
      stringBuilder.AppendLine(Game1.content.LoadString("Strings\\StringsFromCSFiles:MeleeWeapon.cs.14134", this.speed.Value > 0 ? (object) "+" : (object) "-", (object) Math.Abs(this.speed.Value)));
    if (this.addedAreaOfEffect.Value > 0)
      stringBuilder.AppendLine(Game1.content.LoadString("Strings\\StringsFromCSFiles:MeleeWeapon.cs.14136", (object) this.addedAreaOfEffect));
    if (this.addedPrecision.Value > 0)
      stringBuilder.AppendLine(Game1.content.LoadString("Strings\\StringsFromCSFiles:MeleeWeapon.cs.14138", (object) this.addedPrecision));
    if (this.addedDefense.Value > 0)
      stringBuilder.AppendLine(Game1.content.LoadString("Strings\\StringsFromCSFiles:MeleeWeapon.cs.14140", (object) this.addedDefense));
    if ((double) this.critChance.Value / 0.02 >= 2.0)
      stringBuilder.AppendLine(Game1.content.LoadString("Strings\\StringsFromCSFiles:MeleeWeapon.cs.14142", (object) (int) ((double) this.critChance.Value / 0.02)));
    if (((double) this.critMultiplier.Value - 3.0) / 0.02 >= 1.0)
      stringBuilder.AppendLine(Game1.content.LoadString("Strings\\StringsFromCSFiles:MeleeWeapon.cs.14144", (object) (int) (((double) this.critMultiplier.Value - 3.0) / 0.02)));
    if ((double) this.knockback.Value != (double) this.defaultKnockBackForThisType(this.type.Value))
      stringBuilder.AppendLine(Game1.content.LoadString("Strings\\StringsFromCSFiles:MeleeWeapon.cs.14140", (double) this.knockback.Value > (double) this.defaultKnockBackForThisType(this.type.Value) ? (object) "+" : (object) "", (object) (int) Math.Ceiling((double) Math.Abs(this.knockback.Value - this.defaultKnockBackForThisType(this.type.Value)) * 10.0)));
    return stringBuilder.ToString();
  }

  public virtual float defaultKnockBackForThisType(int type)
  {
    switch (type)
    {
      case 0:
      case 3:
        return 1f;
      case 1:
        return 0.5f;
      case 2:
        return 1.5f;
      default:
        return -1f;
    }
  }

  public virtual Rectangle getAreaOfEffect(
    int x,
    int y,
    int facingDirection,
    ref Vector2 tileLocation1,
    ref Vector2 tileLocation2,
    Rectangle wielderBoundingBox,
    int indexInCurrentAnimation)
  {
    Rectangle areaOfEffect = Rectangle.Empty;
    int num1;
    int width;
    int num2;
    int num3;
    if (this.type.Value == 1)
    {
      num1 = 74;
      width = 48 /*0x30*/;
      num2 = 42;
      num3 = -32;
    }
    else
    {
      num1 = 64 /*0x40*/;
      width = 64 /*0x40*/;
      num3 = -32;
      num2 = 0;
    }
    if (this.type.Value == 1)
    {
      switch (facingDirection)
      {
        case 0:
          areaOfEffect = new Rectangle(x - num1 / 2, wielderBoundingBox.Y - width - num2, num1 / 2, width + num2);
          tileLocation1 = new Vector2((float) (Game1.random.Choose<int>(areaOfEffect.Left, areaOfEffect.Right) / 64 /*0x40*/), (float) (areaOfEffect.Top / 64 /*0x40*/));
          tileLocation2 = new Vector2((float) (areaOfEffect.Center.X / 64 /*0x40*/), (float) (areaOfEffect.Top / 64 /*0x40*/));
          areaOfEffect.Offset(20, -16);
          areaOfEffect.Height += 16 /*0x10*/;
          areaOfEffect.Width += 20;
          break;
        case 1:
          areaOfEffect = new Rectangle(wielderBoundingBox.Right, y - width / 2 + num3, (int) ((double) width * 1.1499999761581421), num1);
          tileLocation1 = new Vector2((float) (areaOfEffect.Center.X / 64 /*0x40*/), (float) (Game1.random.Choose<int>(areaOfEffect.Top, areaOfEffect.Bottom) / 64 /*0x40*/));
          tileLocation2 = new Vector2((float) (areaOfEffect.Center.X / 64 /*0x40*/), (float) (areaOfEffect.Center.Y / 64 /*0x40*/));
          areaOfEffect.Offset(-4, 0);
          areaOfEffect.Width += 16 /*0x10*/;
          break;
        case 2:
          areaOfEffect = new Rectangle(x - num1 / 2, wielderBoundingBox.Bottom, num1, (int) ((double) width * 1.75));
          tileLocation1 = new Vector2((float) (Game1.random.Choose<int>(areaOfEffect.Left, areaOfEffect.Right) / 64 /*0x40*/), (float) (areaOfEffect.Center.Y / 64 /*0x40*/));
          tileLocation2 = new Vector2((float) (areaOfEffect.Center.X / 64 /*0x40*/), (float) (areaOfEffect.Center.Y / 64 /*0x40*/));
          areaOfEffect.Offset(12, -8);
          areaOfEffect.Width -= 21;
          break;
        case 3:
          areaOfEffect = new Rectangle(wielderBoundingBox.Left - (int) ((double) width * 1.1499999761581421), y - width / 2 + num3, (int) ((double) width * 1.1499999761581421), num1);
          tileLocation1 = new Vector2((float) (areaOfEffect.Left / 64 /*0x40*/), (float) (Game1.random.Choose<int>(areaOfEffect.Top, areaOfEffect.Bottom) / 64 /*0x40*/));
          tileLocation2 = new Vector2((float) (areaOfEffect.Left / 64 /*0x40*/), (float) (areaOfEffect.Center.Y / 64 /*0x40*/));
          areaOfEffect.Offset(-12, 0);
          areaOfEffect.Width += 16 /*0x10*/;
          break;
      }
    }
    else
    {
      switch (facingDirection)
      {
        case 0:
          areaOfEffect = new Rectangle(x - num1 / 2, wielderBoundingBox.Y - width - num2, num1, width + num2);
          tileLocation1 = new Vector2((float) (Game1.random.Choose<int>(areaOfEffect.Left, areaOfEffect.Right) / 64 /*0x40*/), (float) (areaOfEffect.Top / 64 /*0x40*/));
          tileLocation2 = new Vector2((float) (areaOfEffect.Center.X / 64 /*0x40*/), (float) (areaOfEffect.Top / 64 /*0x40*/));
          switch (indexInCurrentAnimation)
          {
            case 0:
              areaOfEffect.Offset(-60, -12);
              break;
            case 1:
              areaOfEffect.Offset(-48, -56);
              areaOfEffect.Height += 32 /*0x20*/;
              break;
            case 2:
              areaOfEffect.Offset(-12, -68);
              areaOfEffect.Height += 48 /*0x30*/;
              break;
            case 3:
              areaOfEffect.Offset(40, -60);
              areaOfEffect.Height += 48 /*0x30*/;
              break;
            case 4:
              areaOfEffect.Offset(56, -32);
              areaOfEffect.Height += 32 /*0x20*/;
              break;
            case 5:
              areaOfEffect.Offset(76, -32);
              break;
          }
          break;
        case 1:
          areaOfEffect = new Rectangle(wielderBoundingBox.Right, y - width / 2 + num3, width, num1);
          tileLocation1 = new Vector2((float) (areaOfEffect.Center.X / 64 /*0x40*/), (float) (Game1.random.Choose<int>(areaOfEffect.Top, areaOfEffect.Bottom) / 64 /*0x40*/));
          tileLocation2 = new Vector2((float) (areaOfEffect.Center.X / 64 /*0x40*/), (float) (areaOfEffect.Center.Y / 64 /*0x40*/));
          switch (indexInCurrentAnimation)
          {
            case 0:
              areaOfEffect.Offset(-44, -84);
              break;
            case 1:
              areaOfEffect.Offset(4, -44);
              break;
            case 2:
              areaOfEffect.Offset(12, -4);
              break;
            case 3:
              areaOfEffect.Offset(12, 37);
              break;
            case 4:
              areaOfEffect.Offset(-28, 60);
              break;
            case 5:
              areaOfEffect.Offset(-60, 72);
              break;
          }
          break;
        case 2:
          areaOfEffect = new Rectangle(x - num1 / 2, wielderBoundingBox.Bottom, num1, (int) ((double) width * 1.5));
          tileLocation1 = new Vector2((float) (Game1.random.Choose<int>(areaOfEffect.Left, areaOfEffect.Right) / 64 /*0x40*/), (float) (areaOfEffect.Center.Y / 64 /*0x40*/));
          tileLocation2 = new Vector2((float) (areaOfEffect.Center.X / 64 /*0x40*/), (float) (areaOfEffect.Center.Y / 64 /*0x40*/));
          switch (indexInCurrentAnimation)
          {
            case 0:
              areaOfEffect.Offset(72, -92);
              break;
            case 1:
              areaOfEffect.Offset(56, -32);
              break;
            case 2:
              areaOfEffect.Offset(40, -28);
              break;
            case 3:
              areaOfEffect.Offset(-12, -8);
              break;
            case 4:
              areaOfEffect.Offset(-80, -24);
              areaOfEffect.Width += 32 /*0x20*/;
              break;
            case 5:
              areaOfEffect.Offset(-68, -44);
              break;
          }
          break;
        case 3:
          areaOfEffect = new Rectangle(wielderBoundingBox.Left - width, y - width / 2 + num3, width, num1);
          tileLocation1 = new Vector2((float) (areaOfEffect.Left / 64 /*0x40*/), (float) (Game1.random.Choose<int>(areaOfEffect.Top, areaOfEffect.Bottom) / 64 /*0x40*/));
          tileLocation2 = new Vector2((float) (areaOfEffect.Left / 64 /*0x40*/), (float) (areaOfEffect.Center.Y / 64 /*0x40*/));
          switch (indexInCurrentAnimation)
          {
            case 0:
              areaOfEffect.Offset(56, -76);
              break;
            case 1:
              areaOfEffect.Offset(-8, -56);
              break;
            case 2:
              areaOfEffect.Offset(-16, -4);
              break;
            case 3:
              areaOfEffect.Offset(0, 37);
              break;
            case 4:
              areaOfEffect.Offset(24, 60);
              break;
            case 5:
              areaOfEffect.Offset(64 /*0x40*/, 64 /*0x40*/);
              break;
          }
          break;
      }
    }
    areaOfEffect.Inflate(this.addedAreaOfEffect.Value, this.addedAreaOfEffect.Value);
    return areaOfEffect;
  }

  public void triggerDefenseSwordFunction(Farmer who) => this.defenseSwordEvent.Fire();

  private void doDefenseSwordFunction()
  {
    this.isOnSpecial = false;
    this.lastUser.UsingTool = false;
    this.lastUser.CanMove = true;
    this.lastUser.FarmerSprite.PauseForSingleAnimation = false;
  }

  public void triggerDaggerFunction(Farmer who, int dagger_hits_left)
  {
    this.daggerEvent.Fire(dagger_hits_left);
  }

  private void doDaggerFunction(int dagger_hits)
  {
    Vector2 positionAwayFromBox = this.lastUser.getUniformPositionAwayFromBox(this.lastUser.FacingDirection, 48 /*0x30*/);
    int daggerHitsLeft = MeleeWeapon.daggerHitsLeft;
    MeleeWeapon.daggerHitsLeft = dagger_hits;
    this.DoDamage(Game1.currentLocation, (int) positionAwayFromBox.X, (int) positionAwayFromBox.Y, this.lastUser.FacingDirection, 1, this.lastUser);
    MeleeWeapon.daggerHitsLeft = daggerHitsLeft;
    if (this.lastUser != null && this.lastUser.IsLocalPlayer)
      --MeleeWeapon.daggerHitsLeft;
    this.isOnSpecial = false;
    this.lastUser.UsingTool = false;
    this.lastUser.CanMove = true;
    this.lastUser.FarmerSprite.PauseForSingleAnimation = false;
    if (MeleeWeapon.daggerHitsLeft <= 0 || this.lastUser == null || !this.lastUser.IsLocalPlayer)
      return;
    this.quickStab(this.lastUser);
  }

  public void triggerClubFunction(Farmer who)
  {
    if (this.PlayUseSounds)
      who.playNearbySoundAll("clubSmash");
    who.currentLocation.damageMonster(new Rectangle((int) who.Position.X - 192 /*0xC0*/, who.GetBoundingBox().Y - 192 /*0xC0*/, 384, 384), this.minDamage.Value, this.maxDamage.Value, false, 1.5f, 100, 0.0f, 1f, false, who);
    Game1.viewport.Y -= 21;
    Game1.viewport.X += Game1.random.Next(-32, 32 /*0x20*/);
    Vector2 positionAwayFromBox = who.getUniformPositionAwayFromBox(who.FacingDirection, 64 /*0x40*/);
    switch (who.FacingDirection)
    {
      case 0:
      case 2:
        positionAwayFromBox.X -= 32f;
        positionAwayFromBox.Y -= 32f;
        break;
      case 1:
        positionAwayFromBox.X -= 42f;
        positionAwayFromBox.Y -= 32f;
        break;
      case 3:
        positionAwayFromBox.Y -= 32f;
        break;
    }
    Game1.multiplayer.broadcastSprites(who.currentLocation, new TemporaryAnimatedSprite("TileSheets\\animations", new Rectangle(0, 128 /*0x80*/, 64 /*0x40*/, 64 /*0x40*/), 40f, 4, 0, positionAwayFromBox, false, who.FacingDirection == 1));
    who.jitterStrength = 2f;
  }

  private void beginSpecialMove(Farmer who)
  {
    if (Game1.fadeToBlack)
      return;
    this.isOnSpecial = true;
    who.UsingTool = true;
    who.CanMove = false;
  }

  private void quickStab(Farmer who)
  {
    AnimatedSprite.endOfAnimationBehavior endOfBehaviorFunction = (AnimatedSprite.endOfAnimationBehavior) (f => this.triggerDaggerFunction(f, MeleeWeapon.daggerHitsLeft));
    if (!who.IsLocalPlayer)
      endOfBehaviorFunction = (AnimatedSprite.endOfAnimationBehavior) null;
    switch (who.FacingDirection)
    {
      case 0:
        ((FarmerSprite) who.Sprite).animateOnce(276, 15f, 2, endOfBehaviorFunction);
        this.Update(0, 0, who);
        break;
      case 1:
        ((FarmerSprite) who.Sprite).animateOnce(274, 15f, 2, endOfBehaviorFunction);
        this.Update(1, 0, who);
        break;
      case 2:
        ((FarmerSprite) who.Sprite).animateOnce(272, 15f, 2, endOfBehaviorFunction);
        this.Update(2, 0, who);
        break;
      case 3:
        ((FarmerSprite) who.Sprite).animateOnce(278, 15f, 2, endOfBehaviorFunction);
        this.Update(3, 0, who);
        break;
    }
    this.FireProjectile(who);
    this.beginSpecialMove(who);
    if (!this.PlayUseSounds)
      return;
    who.playNearbySoundLocal("daggerswipe");
  }

  protected virtual int specialCooldown()
  {
    switch (this.type.Value)
    {
      case 0:
        return MeleeWeapon.attackSwordCooldown;
      case 1:
        return MeleeWeapon.daggerCooldown;
      case 2:
        return MeleeWeapon.clubCooldown;
      case 3:
        return MeleeWeapon.defenseCooldown;
      default:
        return 0;
    }
  }

  public virtual void animateSpecialMove(Farmer who)
  {
    this.lastUser = who;
    if (this.type.Value == 3 && (this.Name.Contains("Scythe") || this.isScythe()) || Game1.fadeToBlack || this.specialCooldown() > 0)
      return;
    this.animateSpecialMoveEvent.Fire();
  }

  protected virtual void doAnimateSpecialMove()
  {
    if (this.lastUser == null || this.lastUser.CurrentTool != this)
      return;
    if (this.lastUser.isEmoteAnimating)
      this.lastUser.EndEmoteAnimation();
    switch (this.type.Value)
    {
      case 1:
        MeleeWeapon.daggerHitsLeft = 4;
        this.quickStab(this.lastUser);
        if (this.lastUser.IsLocalPlayer)
          MeleeWeapon.daggerCooldown = 3000;
        if (this.lastUser.professions.Contains(28))
          MeleeWeapon.daggerCooldown /= 2;
        if (!this.hasEnchantmentOfType<ArtfulEnchantment>())
          break;
        MeleeWeapon.daggerCooldown /= 2;
        break;
      case 2:
        AnimatedSprite.endOfAnimationBehavior endOfBehaviorFunction1 = new AnimatedSprite.endOfAnimationBehavior(this.triggerClubFunction);
        if (!this.lastUser.IsLocalPlayer)
          endOfBehaviorFunction1 = (AnimatedSprite.endOfAnimationBehavior) null;
        if (this.PlayUseSounds)
          this.lastUser.playNearbySoundLocal("clubswipe");
        switch (this.lastUser.FacingDirection)
        {
          case 0:
            ((FarmerSprite) this.lastUser.Sprite).animateOnce(176 /*0xB0*/, 40f, 8, endOfBehaviorFunction1);
            this.Update(0, 0, this.lastUser);
            break;
          case 1:
            ((FarmerSprite) this.lastUser.Sprite).animateOnce(168, 40f, 8, endOfBehaviorFunction1);
            this.Update(1, 0, this.lastUser);
            break;
          case 2:
            ((FarmerSprite) this.lastUser.Sprite).animateOnce(160 /*0xA0*/, 40f, 8, endOfBehaviorFunction1);
            this.Update(2, 0, this.lastUser);
            break;
          case 3:
            ((FarmerSprite) this.lastUser.Sprite).animateOnce(184, 40f, 8, endOfBehaviorFunction1);
            this.Update(3, 0, this.lastUser);
            break;
        }
        this.beginSpecialMove(this.lastUser);
        if (this.lastUser.IsLocalPlayer)
          MeleeWeapon.clubCooldown = 6000;
        if (this.lastUser.professions.Contains(28))
          MeleeWeapon.clubCooldown /= 2;
        if (!this.hasEnchantmentOfType<ArtfulEnchantment>())
          break;
        MeleeWeapon.clubCooldown /= 2;
        break;
      case 3:
        AnimatedSprite.endOfAnimationBehavior endOfBehaviorFunction2 = new AnimatedSprite.endOfAnimationBehavior(this.triggerDefenseSwordFunction);
        if (!this.lastUser.IsLocalPlayer)
          endOfBehaviorFunction2 = (AnimatedSprite.endOfAnimationBehavior) null;
        switch (this.lastUser.FacingDirection)
        {
          case 0:
            ((FarmerSprite) this.lastUser.Sprite).animateOnce(252, 500f, 1, endOfBehaviorFunction2);
            this.Update(0, 0, this.lastUser);
            break;
          case 1:
            ((FarmerSprite) this.lastUser.Sprite).animateOnce(243, 500f, 1, endOfBehaviorFunction2);
            this.Update(1, 0, this.lastUser);
            break;
          case 2:
            ((FarmerSprite) this.lastUser.Sprite).animateOnce(234, 500f, 1, endOfBehaviorFunction2);
            this.Update(2, 0, this.lastUser);
            break;
          case 3:
            ((FarmerSprite) this.lastUser.Sprite).animateOnce(259, 500f, 1, endOfBehaviorFunction2);
            this.Update(3, 0, this.lastUser);
            break;
        }
        if (this.PlayUseSounds)
          this.lastUser.playNearbySoundLocal("batFlap");
        this.beginSpecialMove(this.lastUser);
        if (this.lastUser.IsLocalPlayer)
          MeleeWeapon.defenseCooldown = 1500;
        if (this.lastUser.professions.Contains(28))
          MeleeWeapon.defenseCooldown /= 2;
        if (!this.hasEnchantmentOfType<ArtfulEnchantment>())
          break;
        MeleeWeapon.defenseCooldown /= 2;
        break;
    }
  }

  public void doSwipe(
    int type,
    Vector2 position,
    int facingDirection,
    float swipeSpeed,
    Farmer f)
  {
    if (f == null || f.CurrentTool != this)
      return;
    if (f.IsLocalPlayer)
    {
      f.TemporaryPassableTiles.Clear();
      f.currentLocation.lastTouchActionLocation = Vector2.Zero;
    }
    swipeSpeed *= 1.3f;
    switch (type)
    {
      case 2:
        if (f.CurrentTool == this)
        {
          switch (f.FacingDirection)
          {
            case 0:
              ((FarmerSprite) f.Sprite).animateOnce(248, swipeSpeed, 8);
              this.Update(0, 0, f);
              break;
            case 1:
              ((FarmerSprite) f.Sprite).animateOnce(240 /*0xF0*/, swipeSpeed, 8);
              this.Update(1, 0, f);
              break;
            case 2:
              ((FarmerSprite) f.Sprite).animateOnce(232, swipeSpeed, 8);
              this.Update(2, 0, f);
              break;
            case 3:
              ((FarmerSprite) f.Sprite).animateOnce(256 /*0x0100*/, swipeSpeed, 8);
              this.Update(3, 0, f);
              break;
          }
        }
        if (!this.PlayUseSounds)
          break;
        f.playNearbySoundLocal("clubswipe");
        break;
      case 3:
        if (f.CurrentTool == this)
        {
          switch (f.FacingDirection)
          {
            case 0:
              ((FarmerSprite) f.Sprite).animateOnce(248, swipeSpeed, 6);
              this.Update(0, 0, f);
              break;
            case 1:
              ((FarmerSprite) f.Sprite).animateOnce(240 /*0xF0*/, swipeSpeed, 6);
              this.Update(1, 0, f);
              break;
            case 2:
              ((FarmerSprite) f.Sprite).animateOnce(232, swipeSpeed, 6);
              this.Update(2, 0, f);
              break;
            case 3:
              ((FarmerSprite) f.Sprite).animateOnce(256 /*0x0100*/, swipeSpeed, 6);
              this.Update(3, 0, f);
              break;
          }
        }
        if (!this.PlayUseSounds || !f.ShouldHandleAnimationSound())
          break;
        f.playNearbySoundLocal("swordswipe");
        break;
    }
  }

  public virtual void FireProjectile(Farmer who)
  {
    if (this.cachedData?.Projectiles == null)
      return;
    foreach (WeaponProjectile projectile in this.cachedData.Projectiles)
    {
      float num1 = 0.0f;
      float num2 = 1f;
      switch (who.facingDirection.Value)
      {
        case 0:
          num1 = 90f;
          break;
        case 1:
          num1 = 0.0f;
          break;
        case 2:
          num1 = 270f;
          break;
        case 3:
          num1 = 180f;
          num2 = -1f;
          break;
      }
      float num3 = (num1 + (projectile.MinAngleOffset + (float) (Game1.random.NextDouble() * ((double) projectile.MaxAngleOffset - (double) projectile.MinAngleOffset))) * num2) * ((float) Math.PI / 180f);
      string str = (string) null;
      if (projectile.Item != null)
      {
        str = ItemQueryResolver.TryResolveRandomItem((ISpawnItemData) projectile.Item, new ItemQueryContext(who.currentLocation, who, (Random) null, $"weapon '{this.QualifiedItemId}' > projectile data '{projectile.Id}'"))?.QualifiedItemId;
        if (str == null)
          continue;
      }
      Vector2 vector2 = who.getStandingPosition() - new Vector2(32f, 32f);
      int damage = projectile.Damage;
      int spriteIndex = projectile.SpriteIndex;
      int bounces = projectile.Bounces;
      int tailLength = projectile.TailLength;
      double rotationVelocity = (double) projectile.RotationVelocity * (Math.PI / 180.0);
      double xVelocity = (double) projectile.Velocity * Math.Cos((double) num3);
      double yVelocity = (double) projectile.Velocity * -Math.Sin((double) num3);
      Vector2 startingPosition = vector2;
      string fireSound = projectile.FireSound;
      string collisionSound = projectile.CollisionSound;
      string bounceSound = projectile.BounceSound;
      string firingSound = fireSound;
      int num4 = projectile.Explodes ? 1 : 0;
      GameLocation currentLocation = who.currentLocation;
      Farmer firer = who;
      string shotItemId = str;
      BasicProjectile basicProjectile = new BasicProjectile(damage, spriteIndex, bounces, tailLength, (float) rotationVelocity, (float) xVelocity, (float) yVelocity, startingPosition, collisionSound, bounceSound, firingSound, num4 != 0, true, currentLocation, (Character) firer, shotItemId: shotItemId);
      basicProjectile.ignoreTravelGracePeriod.Value = true;
      basicProjectile.ignoreMeleeAttacks.Value = true;
      basicProjectile.maxTravelDistance.Value = projectile.MaxDistance * 64 /*0x40*/;
      basicProjectile.height.Value = 32f;
      who.currentLocation.projectiles.Add((Projectile) basicProjectile);
    }
  }

  public virtual void setFarmerAnimating(Farmer who)
  {
    this.anotherClick = false;
    who.FarmerSprite.PauseForSingleAnimation = false;
    who.FarmerSprite.StopAnimation();
    this.swipeSpeed = (float) (400 - this.speed.Value * 40) - who.addedSpeed * 40f;
    this.swipeSpeed *= 1f - who.buffs.WeaponSpeedMultiplier;
    if (who.IsLocalPlayer)
    {
      foreach (BaseEnchantment enchantment in this.enchantments)
      {
        if (enchantment is BaseWeaponEnchantment weaponEnchantment)
          weaponEnchantment.OnSwing(this, who);
      }
      this.FireProjectile(who);
    }
    if (this.type.Value != 1)
    {
      this.doSwipe(this.type.Value, who.Position, who.FacingDirection, this.swipeSpeed / (this.type.Value == 2 ? 5f : 8f), who);
      who.lastClick = Vector2.Zero;
      Vector2 toolLocation = who.GetToolLocation(true);
      this.DoDamage(who.currentLocation, (int) toolLocation.X, (int) toolLocation.Y, who.FacingDirection, 1, who);
    }
    else
    {
      if (this.PlayUseSounds && who.IsLocalPlayer)
        who.playNearbySoundAll("daggerswipe");
      this.swipeSpeed /= 4f;
      switch (who.FacingDirection)
      {
        case 0:
          ((FarmerSprite) who.Sprite).animateOnce(276, this.swipeSpeed, 2);
          this.Update(0, 0, who);
          break;
        case 1:
          ((FarmerSprite) who.Sprite).animateOnce(274, this.swipeSpeed, 2);
          this.Update(1, 0, who);
          break;
        case 2:
          ((FarmerSprite) who.Sprite).animateOnce(272, this.swipeSpeed, 2);
          this.Update(2, 0, who);
          break;
        case 3:
          ((FarmerSprite) who.Sprite).animateOnce(278, this.swipeSpeed, 2);
          this.Update(3, 0, who);
          break;
      }
      Vector2 toolLocation = who.GetToolLocation(true);
      this.DoDamage(who.currentLocation, (int) toolLocation.X, (int) toolLocation.Y, who.FacingDirection, 1, who);
    }
    if (who.CurrentTool != null)
      return;
    who.completelyStopAnimatingOrDoingAction();
    who.forceCanMove();
  }

  public override void actionWhenStopBeingHeld(Farmer who)
  {
    who.UsingTool = false;
    this.anotherClick = false;
    base.actionWhenStopBeingHeld(who);
  }

  public virtual void RecalculateAppliedForges(bool force = false)
  {
    if (this.enchantments.Count == 0 && !force)
      return;
    foreach (BaseEnchantment enchantment in this.enchantments)
    {
      if (enchantment.IsForge())
        enchantment.UnapplyTo((Item) this);
    }
    WeaponData data = this.GetData();
    if (data != null)
    {
      this.Name = data.Name;
      this.minDamage.Value = data.MinDamage;
      this.maxDamage.Value = data.MaxDamage;
      this.knockback.Value = data.Knockback;
      this.speed.Value = data.Speed;
      this.addedPrecision.Value = data.Precision;
      this.addedDefense.Value = data.Defense;
      this.type.Value = data.Type;
      this.addedAreaOfEffect.Value = data.AreaOfEffect;
      this.critChance.Value = data.CritChance;
      this.critMultiplier.Value = data.CritMultiplier;
      if (this.type.Value == 0)
        this.type.Value = 3;
    }
    foreach (BaseEnchantment enchantment in this.enchantments)
    {
      if (enchantment.IsForge())
        enchantment.ApplyTo((Item) this);
    }
  }

  public virtual void DoDamage(
    GameLocation location,
    int x,
    int y,
    int facingDirection,
    int power,
    Farmer who)
  {
    if (!who.IsLocalPlayer)
      return;
    this.isOnSpecial = false;
    if (this.type.Value != 2)
      this.DoFunction(location, x, y, power, who);
    this.lastUser = who;
    Vector2 zero1 = Vector2.Zero;
    Vector2 zero2 = Vector2.Zero;
    Rectangle areaOfEffect = this.getAreaOfEffect(x, y, facingDirection, ref zero1, ref zero2, who.GetBoundingBox(), who.FarmerSprite.currentAnimationIndex);
    this.mostRecentArea = areaOfEffect;
    float num = this.critChance.Value;
    if (this.type.Value == 1)
      num = (num + 0.005f) * 1.12f;
    if (location.damageMonster(areaOfEffect, (int) ((double) this.minDamage.Value * (1.0 + (double) who.buffs.AttackMultiplier)), (int) ((double) this.maxDamage.Value * (1.0 + (double) who.buffs.AttackMultiplier)), false, this.knockback.Value * (1f + who.buffs.KnockbackMultiplier), (int) ((double) this.addedPrecision.Value * (1.0 + (double) who.buffs.WeaponPrecisionMultiplier)), num * (1f + who.buffs.CriticalChanceMultiplier), this.critMultiplier.Value * (1f + who.buffs.CriticalPowerMultiplier), this.type.Value != 1 || !this.isOnSpecial, who) && this.type.Value == 2 && this.PlayUseSounds)
      who.playNearbySoundAll("clubhit");
    string cueName = "";
    location.projectiles.RemoveWhere((Func<Projectile, bool>) (projectile =>
    {
      if (areaOfEffect.Intersects(projectile.getBoundingBox()) && !projectile.ignoreMeleeAttacks.Value)
        projectile.behaviorOnCollisionWithOther(location);
      return projectile.destroyMe;
    }));
    foreach (Vector2 removeDuplicate in Utility.removeDuplicates(Utility.getListOfTileLocationsForBordersOfNonTileRectangle(areaOfEffect)))
    {
      TerrainFeature terrainFeature;
      if (location.terrainFeatures.TryGetValue(removeDuplicate, out terrainFeature) && terrainFeature.performToolAction((Tool) this, 0, removeDuplicate))
        location.terrainFeatures.Remove(removeDuplicate);
      StardewValley.Object @object;
      if (location.objects.TryGetValue(removeDuplicate, out @object) && @object.performToolAction((Tool) this))
        location.objects.Remove(removeDuplicate);
      if (location.performToolAction((Tool) this, (int) removeDuplicate.X, (int) removeDuplicate.Y))
        break;
    }
    if (this.PlayUseSounds && !cueName.Equals(""))
      Game1.playSound(cueName);
    this.CurrentParentTileIndex = this.IndexOfMenuItemView;
    if (who == null || !who.isRidingHorse())
      return;
    who.completelyStopAnimatingOrDoingAction();
  }

  /// <summary>Get the qualified item ID to draw for this weapon.</summary>
  public string GetDrawnItemId() => this.appearance.Value ?? this.QualifiedItemId;

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
    if (this.isScythe())
      return;
    Utility.drawWithShadow(spriteBatch, Game1.mouseCursors, new Vector2((float) (x + 16 /*0x10*/ + 4), (float) (y + 16 /*0x10*/ + 4)), new Rectangle(120, 428, 10, 10), Color.White, 0.0f, Vector2.Zero, 4f, layerDepth: 1f);
    Color color1 = Game1.textColor;
    if (this.hasEnchantmentOfType<RubyEnchantment>())
      color1 = new Color(0, 120, 120);
    Utility.drawTextWithShadow(spriteBatch, Game1.content.LoadString("Strings\\UI:ItemHover_Damage", (object) this.minDamage, (object) this.maxDamage), font, new Vector2((float) (x + 16 /*0x10*/ + 52), (float) (y + 16 /*0x10*/ + 12)), color1 * 0.9f * alpha);
    y += (int) Math.Max(font.MeasureString("TT").Y, 48f);
    if (this.speed.Value != (this.type.Value == 2 ? -8 : 0))
    {
      Utility.drawWithShadow(spriteBatch, Game1.mouseCursors, new Vector2((float) (x + 16 /*0x10*/ + 4), (float) (y + 16 /*0x10*/ + 4)), new Rectangle(130, 428, 10, 10), Color.White, 0.0f, Vector2.Zero, 4f, layerDepth: 1f);
      bool flag = this.type.Value == 2 && this.speed.Value < -8 || this.type.Value != 2 && this.speed.Value < 0;
      Color color2 = Game1.textColor;
      if (this.hasEnchantmentOfType<EmeraldEnchantment>())
        color2 = new Color(0, 120, 120);
      Utility.drawTextWithShadow(spriteBatch, Game1.content.LoadString("Strings\\UI:ItemHover_Speed", (object) (((this.type.Value == 2 ? this.speed.Value - -8 : this.speed.Value) > 0 ? "+" : "") + ((this.type.Value == 2 ? this.speed.Value - -8 : this.speed.Value) / 2).ToString())), font, new Vector2((float) (x + 16 /*0x10*/ + 52), (float) (y + 16 /*0x10*/ + 12)), flag ? Color.DarkRed : color2 * 0.9f * alpha);
      y += (int) Math.Max(font.MeasureString("TT").Y, 48f);
    }
    if (this.addedDefense.Value > 0)
    {
      Color color3 = Game1.textColor;
      if (this.hasEnchantmentOfType<TopazEnchantment>())
        color3 = new Color(0, 120, 120);
      Utility.drawWithShadow(spriteBatch, Game1.mouseCursors, new Vector2((float) (x + 16 /*0x10*/ + 4), (float) (y + 16 /*0x10*/ + 4)), new Rectangle(110, 428, 10, 10), Color.White, 0.0f, Vector2.Zero, 4f, layerDepth: 1f);
      Utility.drawTextWithShadow(spriteBatch, Game1.content.LoadString("Strings\\UI:ItemHover_DefenseBonus", (object) this.addedDefense), font, new Vector2((float) (x + 16 /*0x10*/ + 52), (float) (y + 16 /*0x10*/ + 12)), color3 * 0.9f * alpha);
      y += (int) Math.Max(font.MeasureString("TT").Y, 48f);
    }
    float num = this.critChance.Value;
    if (this.type.Value == 1)
      num = (num + 0.005f) * 1.12f;
    if ((double) num / 0.02 >= 1.1000000238418579)
    {
      Color color4 = Game1.textColor;
      if (this.hasEnchantmentOfType<AquamarineEnchantment>())
        color4 = new Color(0, 120, 120);
      Utility.drawWithShadow(spriteBatch, Game1.mouseCursors, new Vector2((float) (x + 16 /*0x10*/ + 4), (float) (y + 16 /*0x10*/ + 4)), new Rectangle(40, 428, 10, 10), Color.White, 0.0f, Vector2.Zero, 4f, layerDepth: 1f);
      Utility.drawTextWithShadow(spriteBatch, Game1.content.LoadString("Strings\\UI:ItemHover_CritChanceBonus", (object) (int) Math.Round(((double) num - 1.0 / 1000.0) / 0.02)), font, new Vector2((float) (x + 16 /*0x10*/ + 52), (float) (y + 16 /*0x10*/ + 12)), color4 * 0.9f * alpha);
      y += (int) Math.Max(font.MeasureString("TT").Y, 48f);
    }
    if (((double) this.critMultiplier.Value - 3.0) / 0.02 >= 1.0)
    {
      Color color5 = Game1.textColor;
      if (this.hasEnchantmentOfType<JadeEnchantment>())
        color5 = new Color(0, 120, 120);
      Utility.drawWithShadow(spriteBatch, Game1.mouseCursors, new Vector2((float) (x + 16 /*0x10*/), (float) (y + 16 /*0x10*/ + 4)), new Rectangle(160 /*0xA0*/, 428, 10, 10), Color.White, 0.0f, Vector2.Zero, 4f, layerDepth: 1f);
      Utility.drawTextWithShadow(spriteBatch, Game1.content.LoadString("Strings\\UI:ItemHover_CritPowerBonus", (object) (int) (((double) this.critMultiplier.Value - 3.0) / 0.02)), font, new Vector2((float) (x + 16 /*0x10*/ + 44), (float) (y + 16 /*0x10*/ + 12)), color5 * 0.9f * alpha);
      y += (int) Math.Max(font.MeasureString("TT").Y, 48f);
    }
    if ((double) this.knockback.Value != (double) this.defaultKnockBackForThisType(this.type.Value))
    {
      Color color6 = Game1.textColor;
      if (this.hasEnchantmentOfType<AmethystEnchantment>())
        color6 = new Color(0, 120, 120);
      Utility.drawWithShadow(spriteBatch, Game1.mouseCursors, new Vector2((float) (x + 16 /*0x10*/ + 4), (float) (y + 16 /*0x10*/ + 4)), new Rectangle(70, 428, 10, 10), Color.White, 0.0f, Vector2.Zero, 4f, layerDepth: 1f);
      Utility.drawTextWithShadow(spriteBatch, Game1.content.LoadString("Strings\\UI:ItemHover_Weight", (object) (((double) (int) Math.Ceiling((double) Math.Abs(this.knockback.Value - this.defaultKnockBackForThisType(this.type.Value)) * 10.0) > (double) this.defaultKnockBackForThisType(this.type.Value) ? "+" : "") + ((int) Math.Ceiling((double) Math.Abs(this.knockback.Value - this.defaultKnockBackForThisType(this.type.Value)) * 10.0)).ToString())), font, new Vector2((float) (x + 16 /*0x10*/ + 52), (float) (y + 16 /*0x10*/ + 12)), color6 * 0.9f * alpha);
      y += (int) Math.Max(font.MeasureString("TT").Y, 48f);
    }
    if (this.enchantments.Count > 0 && this.enchantments[this.enchantments.Count - 1] is DiamondEnchantment)
    {
      Color color7 = new Color(0, 120, 120);
      int sub1 = this.GetMaxForges() - this.GetTotalForgeLevels();
      string text = sub1 == 1 ? Game1.content.LoadString("Strings\\UI:ItemHover_DiamondForge_Singular", (object) sub1) : Game1.content.LoadString("Strings\\UI:ItemHover_DiamondForge_Plural", (object) sub1);
      Utility.drawTextWithShadow(spriteBatch, text, font, new Vector2((float) (x + 16 /*0x10*/), (float) (y + 16 /*0x10*/ + 12)), color7 * 0.9f * alpha);
      y += (int) Math.Max(font.MeasureString("TT").Y, 48f);
    }
    foreach (BaseEnchantment enchantment in this.enchantments)
    {
      if (enchantment.ShouldBeDisplayed())
      {
        Color color8 = new Color(120, 0, 210);
        if (enchantment.IsSecondaryEnchantment())
        {
          Utility.drawWithShadow(spriteBatch, Game1.mouseCursors_1_6, new Vector2((float) (x + 16 /*0x10*/ + 4), (float) (y + 16 /*0x10*/ + 4)), new Rectangle(502, 430, 10, 10), Color.White, 0.0f, Vector2.Zero, 4f, layerDepth: 1f);
          color8 = new Color(120, 50, 100);
        }
        else
          Utility.drawWithShadow(spriteBatch, Game1.mouseCursors2, new Vector2((float) (x + 16 /*0x10*/ + 4), (float) (y + 16 /*0x10*/ + 4)), new Rectangle((int) sbyte.MaxValue, 35, 10, 10), Color.White, 0.0f, Vector2.Zero, 4f, layerDepth: 1f);
        Utility.drawTextWithShadow(spriteBatch, BaseEnchantment.hideEnchantmentName && !enchantment.IsSecondaryEnchantment() || BaseEnchantment.hideSecondaryEnchantName && enchantment.IsSecondaryEnchantment() ? "???" : enchantment.GetDisplayName(), font, new Vector2((float) (x + 16 /*0x10*/ + 52), (float) (y + 16 /*0x10*/ + 12)), color8 * 0.9f * alpha);
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
    int num = 9999;
    Point tooltipSpecialIcons = new Point(0, 0);
    tooltipSpecialIcons.Y += Math.Max(60, (boldTitleText != null ? (int) ((double) Game1.dialogueFont.MeasureString(boldTitleText).Y + 16.0) : 0) + 32 /*0x20*/) + (int) font.MeasureString("T").Y + (moneyAmountToDisplayAtBottom > -1 ? (int) ((double) font.MeasureString(moneyAmountToDisplayAtBottom.ToString() ?? "").Y + 4.0) : 0);
    tooltipSpecialIcons.Y += this.isScythe() ? 0 : this.getNumberOfDescriptionCategories() * 4 * 12;
    tooltipSpecialIcons.Y += (int) font.MeasureString(Game1.parseText(this.description, Game1.smallFont, this.getDescriptionWidth())).Y;
    tooltipSpecialIcons.X = (int) Math.Max((float) minWidth, Math.Max(font.MeasureString(Game1.content.LoadString("Strings\\UI:ItemHover_Damage", (object) num, (object) num)).X + (float) horizontalBuffer, Math.Max(font.MeasureString(Game1.content.LoadString("Strings\\UI:ItemHover_Speed", (object) num)).X + (float) horizontalBuffer, Math.Max(font.MeasureString(Game1.content.LoadString("Strings\\UI:ItemHover_DefenseBonus", (object) num)).X + (float) horizontalBuffer, Math.Max(font.MeasureString(Game1.content.LoadString("Strings\\UI:ItemHover_CritChanceBonus", (object) num)).X + (float) horizontalBuffer, Math.Max(font.MeasureString(Game1.content.LoadString("Strings\\UI:ItemHover_CritPowerBonus", (object) num)).X + (float) horizontalBuffer, font.MeasureString(Game1.content.LoadString("Strings\\UI:ItemHover_Weight", (object) num)).X + (float) horizontalBuffer))))));
    if (this.enchantments.Count > 0 && this.enchantments[this.enchantments.Count - 1] is DiamondEnchantment)
      tooltipSpecialIcons.X = (int) Math.Max((float) tooltipSpecialIcons.X, font.MeasureString(Game1.content.LoadString("Strings\\UI:ItemHover_DiamondForge_Plural", (object) this.GetMaxForges())).X);
    foreach (BaseEnchantment enchantment in this.enchantments)
    {
      if (enchantment.ShouldBeDisplayed())
        tooltipSpecialIcons.Y += (int) Math.Max(font.MeasureString("TT").Y, 48f);
    }
    return tooltipSpecialIcons;
  }

  public virtual void ResetIndexOfMenuItemView()
  {
    this.IndexOfMenuItemView = this.InitialParentTileIndex;
  }

  public virtual void drawDuringUse(
    int frameOfFarmerAnimation,
    int facingDirection,
    SpriteBatch spriteBatch,
    Vector2 playerPosition,
    Farmer f)
  {
    MeleeWeapon.drawDuringUse(frameOfFarmerAnimation, facingDirection, spriteBatch, playerPosition, f, this.GetDrawnItemId(), this.type.Value, this.isOnSpecial);
  }

  public override bool CanForge(Item item)
  {
    return item is MeleeWeapon meleeWeapon && meleeWeapon.type.Value == this.type.Value || base.CanForge(item);
  }

  public override bool CanAddEnchantment(BaseEnchantment enchantment)
  {
    return (!(enchantment is GalaxySoulEnchantment) || this.isGalaxyWeapon()) && base.CanAddEnchantment(enchantment);
  }

  public bool isGalaxyWeapon()
  {
    return this.QualifiedItemId == "(W)4" || this.QualifiedItemId == "(W)23" || this.QualifiedItemId == "(W)29";
  }

  /// <summary>Convert this weapon to a new item ID. This reloads the weapon data but keeps any previous enchantments, mod data, etc.</summary>
  /// <param name="newItemId">The new unqualified item ID.</param>
  public void transform(string newItemId)
  {
    this.ItemId = newItemId;
    this.appearance.Value = (string) null;
    this.RecalculateAppliedForges(true);
  }

  public override bool Forge(Item item, bool count_towards_stats = false)
  {
    if (this.isScythe())
      return false;
    if (!(item is MeleeWeapon meleeWeapon) || meleeWeapon.type.Value != this.type.Value)
      return base.Forge(item, count_towards_stats);
    this.appearance.Value = meleeWeapon.QualifiedItemId;
    return true;
  }

  public static void drawDuringUse(
    int frameOfFarmerAnimation,
    int facingDirection,
    SpriteBatch spriteBatch,
    Vector2 playerPosition,
    Farmer f,
    string weaponItemId,
    int type,
    bool isOnSpecial)
  {
    ParsedItemData dataOrErrorItem = ItemRegistry.GetDataOrErrorItem(weaponItemId);
    Texture2D texture = dataOrErrorItem.GetTexture() ?? Tool.weaponsTexture;
    Rectangle sourceRect = dataOrErrorItem.GetSourceRect();
    float drawLayer = f.getDrawLayer();
    FarmerRenderer.FarmerSpriteLayers layer;
    switch (f.FacingDirection)
    {
      case 0:
        layer = FarmerRenderer.FarmerSpriteLayers.ToolUp;
        break;
      case 2:
        layer = FarmerRenderer.FarmerSpriteLayers.ToolDown;
        break;
      default:
        layer = FarmerRenderer.FarmerSpriteLayers.TOOL_IN_USE_SIDE;
        break;
    }
    float layerDepth1 = FarmerRenderer.GetLayerDepth(drawLayer, FarmerRenderer.FarmerSpriteLayers.ToolUp);
    float layerDepth2 = FarmerRenderer.GetLayerDepth(drawLayer, layer);
    if (type != 1)
    {
      if (isOnSpecial)
      {
        switch (type)
        {
          case 2:
            switch (facingDirection)
            {
              case 1:
                switch (frameOfFarmerAnimation)
                {
                  case 0:
                    spriteBatch.Draw(texture, new Vector2((float) ((double) playerPosition.X - 32.0 - 12.0), playerPosition.Y - 80f), new Rectangle?(sourceRect), Color.White, -3f * (float) Math.PI / 8f, Vector2.Zero, 4f, SpriteEffects.None, layerDepth2);
                    return;
                  case 1:
                    spriteBatch.Draw(texture, new Vector2(playerPosition.X + 64f, (float) ((double) playerPosition.Y - 64.0 - 48.0)), new Rectangle?(sourceRect), Color.White, 0.3926991f, Vector2.Zero, 4f, SpriteEffects.None, layerDepth2);
                    return;
                  case 2:
                    spriteBatch.Draw(texture, new Vector2((float) ((double) playerPosition.X + 128.0 - 16.0), (float) ((double) playerPosition.Y - 64.0 - 12.0)), new Rectangle?(sourceRect), Color.White, 3f * (float) Math.PI / 8f, Vector2.Zero, 4f, SpriteEffects.None, layerDepth2);
                    return;
                  case 3:
                    spriteBatch.Draw(texture, new Vector2(playerPosition.X + 72f, (float) ((double) playerPosition.Y - 64.0 + 16.0 - 32.0)), new Rectangle?(sourceRect), Color.White, 0.3926991f, Vector2.Zero, 4f, SpriteEffects.None, layerDepth2);
                    return;
                  case 4:
                    spriteBatch.Draw(texture, new Vector2(playerPosition.X + 96f, (float) ((double) playerPosition.Y - 64.0 + 16.0 - 16.0)), new Rectangle?(sourceRect), Color.White, 0.7853982f, Vector2.Zero, 4f, SpriteEffects.None, layerDepth2);
                    return;
                  case 5:
                    spriteBatch.Draw(texture, new Vector2((float) ((double) playerPosition.X + 96.0 - 12.0), (float) ((double) playerPosition.Y - 64.0 + 16.0)), new Rectangle?(sourceRect), Color.White, 0.7853982f, Vector2.Zero, 4f, SpriteEffects.None, layerDepth2);
                    return;
                  case 6:
                    spriteBatch.Draw(texture, new Vector2((float) ((double) playerPosition.X + 96.0 - 16.0), (float) ((double) playerPosition.Y - 64.0 + 40.0 - 8.0)), new Rectangle?(sourceRect), Color.White, 0.7853982f, Vector2.Zero, 4f, SpriteEffects.None, layerDepth2);
                    return;
                  case 7:
                    spriteBatch.Draw(texture, new Vector2((float) ((double) playerPosition.X + 96.0 - 8.0), playerPosition.Y + 40f), new Rectangle?(sourceRect), Color.White, 0.981747746f, Vector2.Zero, 4f, SpriteEffects.None, layerDepth2);
                    return;
                  default:
                    return;
                }
              case 3:
                switch (frameOfFarmerAnimation)
                {
                  case 0:
                    spriteBatch.Draw(texture, new Vector2((float) ((double) playerPosition.X + 64.0 - 4.0 + 8.0), (float) ((double) playerPosition.Y - 56.0 - 64.0)), new Rectangle?(sourceRect), Color.White, 0.3926991f, Vector2.Zero, 4f, SpriteEffects.None, layerDepth2);
                    return;
                  case 1:
                    spriteBatch.Draw(texture, new Vector2(playerPosition.X - 32f, playerPosition.Y - 32f), new Rectangle?(sourceRect), Color.White, -1.96349549f, Vector2.Zero, 4f, SpriteEffects.None, layerDepth2);
                    return;
                  case 2:
                    spriteBatch.Draw(texture, new Vector2(playerPosition.X - 12f, playerPosition.Y + 8f), new Rectangle?(sourceRect), Color.White, -2.74889374f, Vector2.Zero, 4f, SpriteEffects.None, layerDepth2);
                    return;
                  case 3:
                    spriteBatch.Draw(texture, new Vector2((float) ((double) playerPosition.X - 32.0 - 4.0), playerPosition.Y + 8f), new Rectangle?(sourceRect), Color.White, -2.3561945f, Vector2.Zero, 4f, SpriteEffects.None, layerDepth2);
                    return;
                  case 4:
                    spriteBatch.Draw(texture, new Vector2((float) ((double) playerPosition.X - 16.0 - 24.0), (float) ((double) playerPosition.Y + 64.0 + 12.0 - 64.0)), new Rectangle?(sourceRect), Color.White, 4.31969f, Vector2.Zero, 4f, SpriteEffects.None, layerDepth2);
                    return;
                  case 5:
                    spriteBatch.Draw(texture, new Vector2(playerPosition.X - 20f, (float) ((double) playerPosition.Y + 64.0 + 40.0 - 64.0)), new Rectangle?(sourceRect), Color.White, 3.926991f, Vector2.Zero, 4f, SpriteEffects.None, layerDepth2);
                    return;
                  case 6:
                    spriteBatch.Draw(texture, new Vector2(playerPosition.X - 16f, (float) ((double) playerPosition.Y + 64.0 + 56.0)), new Rectangle?(sourceRect), Color.White, 3.926991f, Vector2.Zero, 4f, SpriteEffects.None, layerDepth2);
                    return;
                  case 7:
                    spriteBatch.Draw(texture, new Vector2(playerPosition.X - 8f, (float) ((double) playerPosition.Y + 64.0 + 64.0)), new Rectangle?(sourceRect), Color.White, 3.73064137f, Vector2.Zero, 4f, SpriteEffects.None, layerDepth2);
                    return;
                  default:
                    return;
                }
              default:
                switch (frameOfFarmerAnimation)
                {
                  case 0:
                    spriteBatch.Draw(texture, new Vector2(playerPosition.X - 24f, (float) ((double) playerPosition.Y - 21.0 - 8.0 - 64.0)), new Rectangle?(sourceRect), Color.White, -0.7853982f, Vector2.Zero, 4f, SpriteEffects.None, layerDepth2);
                    break;
                  case 1:
                    spriteBatch.Draw(texture, new Vector2(playerPosition.X - 16f, (float) ((double) playerPosition.Y - 21.0 - 64.0 + 4.0)), new Rectangle?(sourceRect), Color.White, -0.7853982f, Vector2.Zero, 4f, SpriteEffects.None, layerDepth2);
                    break;
                  case 2:
                    spriteBatch.Draw(texture, new Vector2(playerPosition.X - 16f, (float) ((double) playerPosition.Y - 21.0 + 20.0 - 64.0)), new Rectangle?(sourceRect), Color.White, -0.7853982f, Vector2.Zero, 4f, SpriteEffects.None, layerDepth2);
                    break;
                  case 3:
                    if (facingDirection == 2)
                    {
                      spriteBatch.Draw(texture, new Vector2((float) ((double) playerPosition.X + 64.0 + 8.0), playerPosition.Y + 32f), new Rectangle?(sourceRect), Color.White, -3.926991f, Vector2.Zero, 4f, SpriteEffects.None, layerDepth2);
                      break;
                    }
                    spriteBatch.Draw(texture, new Vector2(playerPosition.X - 16f, (float) ((double) playerPosition.Y - 21.0 + 32.0 - 64.0)), new Rectangle?(sourceRect), Color.White, -0.7853982f, Vector2.Zero, 4f, SpriteEffects.None, layerDepth2);
                    break;
                  case 4:
                    if (facingDirection == 2)
                    {
                      spriteBatch.Draw(texture, new Vector2((float) ((double) playerPosition.X + 64.0 + 8.0), playerPosition.Y + 32f), new Rectangle?(sourceRect), Color.White, -3.926991f, Vector2.Zero, 4f, SpriteEffects.None, layerDepth2);
                      break;
                    }
                    break;
                  case 5:
                    if (facingDirection == 2)
                    {
                      spriteBatch.Draw(texture, new Vector2((float) ((double) playerPosition.X + 64.0 + 12.0), (float) ((double) playerPosition.Y + 64.0 - 20.0)), new Rectangle?(sourceRect), Color.White, 2.3561945f, Vector2.Zero, 4f, SpriteEffects.None, layerDepth2);
                      break;
                    }
                    break;
                  case 6:
                    if (facingDirection == 2)
                    {
                      spriteBatch.Draw(texture, new Vector2((float) ((double) playerPosition.X + 64.0 + 12.0), (float) ((double) playerPosition.Y + 64.0 + 54.0)), new Rectangle?(sourceRect), Color.White, 2.3561945f, Vector2.Zero, 4f, SpriteEffects.None, layerDepth2);
                      break;
                    }
                    break;
                  case 7:
                    if (facingDirection == 2)
                    {
                      spriteBatch.Draw(texture, new Vector2((float) ((double) playerPosition.X + 64.0 + 12.0), (float) ((double) playerPosition.Y + 64.0 + 58.0)), new Rectangle?(sourceRect), Color.White, 2.3561945f, Vector2.Zero, 4f, SpriteEffects.None, layerDepth2);
                      break;
                    }
                    break;
                }
                if (f.FacingDirection != 0)
                  return;
                f.FarmerRenderer.draw(spriteBatch, f.FarmerSprite, f.FarmerSprite.SourceRect, f.getLocalPosition(Game1.viewport), new Vector2(0.0f, (float) (((double) f.yOffset + 128.0 - (double) (f.GetBoundingBox().Height / 2)) / 4.0 + 4.0)), layerDepth2, Color.White, 0.0f, f);
                return;
            }
          case 3:
            switch (f.FacingDirection)
            {
              case 0:
                spriteBatch.Draw(texture, new Vector2((float) ((double) playerPosition.X + 64.0 - 8.0), playerPosition.Y - 44f), new Rectangle?(sourceRect), Color.White, -1.76714587f, MeleeWeapon.center, 4f, SpriteEffects.None, layerDepth2);
                return;
              case 1:
                spriteBatch.Draw(texture, new Vector2((float) ((double) playerPosition.X + 64.0 - 8.0), playerPosition.Y - 4f), new Rectangle?(sourceRect), Color.White, -3f * (float) Math.PI / 16f, MeleeWeapon.center, 4f, SpriteEffects.None, layerDepth2);
                return;
              case 2:
                spriteBatch.Draw(texture, new Vector2((float) ((double) playerPosition.X + 64.0 - 52.0), playerPosition.Y + 4f), new Rectangle?(sourceRect), Color.White, -5.105088f, MeleeWeapon.center, 4f, SpriteEffects.None, layerDepth2);
                return;
              case 3:
                spriteBatch.Draw(texture, new Vector2((float) ((double) playerPosition.X + 64.0 - 56.0), playerPosition.Y - 4f), new Rectangle?(sourceRect), Color.White, 3f * (float) Math.PI / 16f, new Vector2(15f, 15f), 4f, SpriteEffects.FlipHorizontally, layerDepth2);
                return;
              default:
                return;
            }
        }
      }
      else
      {
        switch (facingDirection)
        {
          case 0:
            switch (frameOfFarmerAnimation)
            {
              case 0:
                spriteBatch.Draw(texture, new Vector2(playerPosition.X + 32f, playerPosition.Y - 32f), new Rectangle?(sourceRect), Color.White, -2.3561945f, MeleeWeapon.center, 4f, SpriteEffects.None, layerDepth2);
                return;
              case 1:
                spriteBatch.Draw(texture, new Vector2(playerPosition.X + 32f, playerPosition.Y - 48f), new Rectangle?(sourceRect), Color.White, -1.57079637f, MeleeWeapon.center, 4f, SpriteEffects.None, layerDepth2);
                return;
              case 2:
                spriteBatch.Draw(texture, new Vector2(playerPosition.X + 48f, playerPosition.Y - 52f), new Rectangle?(sourceRect), Color.White, -3f * (float) Math.PI / 8f, MeleeWeapon.center, 4f, SpriteEffects.None, layerDepth2);
                return;
              case 3:
                spriteBatch.Draw(texture, new Vector2(playerPosition.X + 48f, playerPosition.Y - 52f), new Rectangle?(sourceRect), Color.White, -0.3926991f, MeleeWeapon.center, 4f, SpriteEffects.None, layerDepth2);
                return;
              case 4:
                spriteBatch.Draw(texture, new Vector2((float) ((double) playerPosition.X + 64.0 - 8.0), playerPosition.Y - 40f), new Rectangle?(sourceRect), Color.White, 0.0f, MeleeWeapon.center, 4f, SpriteEffects.None, layerDepth2);
                return;
              case 5:
                spriteBatch.Draw(texture, new Vector2(playerPosition.X + 64f, playerPosition.Y - 40f), new Rectangle?(sourceRect), Color.White, 0.3926991f, MeleeWeapon.center, 4f, SpriteEffects.None, layerDepth2);
                return;
              case 6:
                spriteBatch.Draw(texture, new Vector2(playerPosition.X + 64f, playerPosition.Y - 40f), new Rectangle?(sourceRect), Color.White, 0.3926991f, MeleeWeapon.center, 4f, SpriteEffects.None, layerDepth2);
                return;
              case 7:
                spriteBatch.Draw(texture, new Vector2((float) ((double) playerPosition.X + 64.0 - 44.0), playerPosition.Y + 64f), new Rectangle?(sourceRect), Color.White, -1.96349537f, MeleeWeapon.center, 4f, SpriteEffects.None, layerDepth2);
                return;
              default:
                return;
            }
          case 1:
            switch (frameOfFarmerAnimation)
            {
              case 0:
                spriteBatch.Draw(texture, new Vector2(playerPosition.X + 40f, (float) ((double) playerPosition.Y - 64.0 + 8.0)), new Rectangle?(sourceRect), Color.White, -0.7853982f, MeleeWeapon.center, 4f, SpriteEffects.None, layerDepth1);
                return;
              case 1:
                spriteBatch.Draw(texture, new Vector2(playerPosition.X + 56f, (float) ((double) playerPosition.Y - 64.0 + 28.0)), new Rectangle?(sourceRect), Color.White, 0.0f, MeleeWeapon.center, 4f, SpriteEffects.None, layerDepth1);
                return;
              case 2:
                spriteBatch.Draw(texture, new Vector2((float) ((double) playerPosition.X + 64.0 - 4.0), playerPosition.Y - 16f), new Rectangle?(sourceRect), Color.White, 0.7853982f, MeleeWeapon.center, 4f, SpriteEffects.None, layerDepth2);
                return;
              case 3:
                spriteBatch.Draw(texture, new Vector2((float) ((double) playerPosition.X + 64.0 - 4.0), playerPosition.Y - 4f), new Rectangle?(sourceRect), Color.White, 1.57079637f, MeleeWeapon.center, 4f, SpriteEffects.None, layerDepth2);
                return;
              case 4:
                spriteBatch.Draw(texture, new Vector2((float) ((double) playerPosition.X + 64.0 - 28.0), playerPosition.Y + 4f), new Rectangle?(sourceRect), Color.White, 1.96349549f, MeleeWeapon.center, 4f, SpriteEffects.None, layerDepth2);
                return;
              case 5:
                spriteBatch.Draw(texture, new Vector2((float) ((double) playerPosition.X + 64.0 - 48.0), playerPosition.Y + 4f), new Rectangle?(sourceRect), Color.White, 2.3561945f, MeleeWeapon.center, 4f, SpriteEffects.None, layerDepth2);
                return;
              case 6:
                spriteBatch.Draw(texture, new Vector2((float) ((double) playerPosition.X + 64.0 - 48.0), playerPosition.Y + 4f), new Rectangle?(sourceRect), Color.White, 2.3561945f, MeleeWeapon.center, 4f, SpriteEffects.None, layerDepth2);
                return;
              case 7:
                spriteBatch.Draw(texture, new Vector2((float) ((double) playerPosition.X + 64.0 - 16.0), (float) ((double) playerPosition.Y + 64.0 + 12.0)), new Rectangle?(sourceRect), Color.White, 1.96349537f, MeleeWeapon.center, 4f, SpriteEffects.None, layerDepth2);
                return;
              default:
                return;
            }
          case 2:
            switch (frameOfFarmerAnimation)
            {
              case 0:
                spriteBatch.Draw(texture, new Vector2(playerPosition.X + 56f, playerPosition.Y - 16f), new Rectangle?(sourceRect), Color.White, 0.3926991f, MeleeWeapon.center, 4f, SpriteEffects.None, layerDepth2);
                return;
              case 1:
                spriteBatch.Draw(texture, new Vector2(playerPosition.X + 52f, playerPosition.Y - 8f), new Rectangle?(sourceRect), Color.White, 1.57079637f, MeleeWeapon.center, 4f, SpriteEffects.None, layerDepth2);
                return;
              case 2:
                spriteBatch.Draw(texture, new Vector2(playerPosition.X + 40f, playerPosition.Y), new Rectangle?(sourceRect), Color.White, 1.57079637f, MeleeWeapon.center, 4f, SpriteEffects.None, layerDepth2);
                return;
              case 3:
                spriteBatch.Draw(texture, new Vector2(playerPosition.X + 16f, playerPosition.Y + 4f), new Rectangle?(sourceRect), Color.White, 2.3561945f, MeleeWeapon.center, 4f, SpriteEffects.None, layerDepth2);
                return;
              case 4:
                spriteBatch.Draw(texture, new Vector2(playerPosition.X + 8f, playerPosition.Y + 8f), new Rectangle?(sourceRect), Color.White, 3.14159274f, MeleeWeapon.center, 4f, SpriteEffects.None, layerDepth2);
                return;
              case 5:
                spriteBatch.Draw(texture, new Vector2(playerPosition.X + 12f, playerPosition.Y), new Rectangle?(sourceRect), Color.White, 3.53429174f, MeleeWeapon.center, 4f, SpriteEffects.None, layerDepth2);
                return;
              case 6:
                spriteBatch.Draw(texture, new Vector2(playerPosition.X + 12f, playerPosition.Y), new Rectangle?(sourceRect), Color.White, 3.53429174f, MeleeWeapon.center, 4f, SpriteEffects.None, layerDepth2);
                return;
              case 7:
                spriteBatch.Draw(texture, new Vector2(playerPosition.X + 44f, playerPosition.Y + 64f), new Rectangle?(sourceRect), Color.White, -5.105088f, MeleeWeapon.center, 4f, SpriteEffects.None, layerDepth2);
                return;
              default:
                return;
            }
          case 3:
            switch (frameOfFarmerAnimation)
            {
              case 0:
                spriteBatch.Draw(texture, new Vector2(playerPosition.X - 16f, (float) ((double) playerPosition.Y - 64.0 - 16.0)), new Rectangle?(sourceRect), Color.White, 0.7853982f, MeleeWeapon.center, 4f, SpriteEffects.FlipHorizontally, layerDepth1);
                return;
              case 1:
                spriteBatch.Draw(texture, new Vector2(playerPosition.X - 48f, (float) ((double) playerPosition.Y - 64.0 + 20.0)), new Rectangle?(sourceRect), Color.White, 0.0f, MeleeWeapon.center, 4f, SpriteEffects.FlipHorizontally, layerDepth1);
                return;
              case 2:
                spriteBatch.Draw(texture, new Vector2((float) ((double) playerPosition.X - 64.0 + 32.0), playerPosition.Y + 16f), new Rectangle?(sourceRect), Color.White, -0.7853982f, MeleeWeapon.center, 4f, SpriteEffects.FlipHorizontally, layerDepth2);
                return;
              case 3:
                spriteBatch.Draw(texture, new Vector2(playerPosition.X + 4f, playerPosition.Y + 44f), new Rectangle?(sourceRect), Color.White, -1.57079637f, MeleeWeapon.center, 4f, SpriteEffects.FlipHorizontally, layerDepth2);
                return;
              case 4:
                spriteBatch.Draw(texture, new Vector2(playerPosition.X + 44f, playerPosition.Y + 52f), new Rectangle?(sourceRect), Color.White, -1.96349549f, MeleeWeapon.center, 4f, SpriteEffects.FlipHorizontally, layerDepth2);
                return;
              case 5:
                spriteBatch.Draw(texture, new Vector2(playerPosition.X + 80f, playerPosition.Y + 40f), new Rectangle?(sourceRect), Color.White, -2.3561945f, MeleeWeapon.center, 4f, SpriteEffects.FlipHorizontally, layerDepth2);
                return;
              case 6:
                spriteBatch.Draw(texture, new Vector2(playerPosition.X + 80f, playerPosition.Y + 40f), new Rectangle?(sourceRect), Color.White, -2.3561945f, MeleeWeapon.center, 4f, SpriteEffects.FlipHorizontally, layerDepth2);
                return;
              case 7:
                spriteBatch.Draw(texture, new Vector2(playerPosition.X - 44f, playerPosition.Y + 96f), new Rectangle?(sourceRect), Color.White, -5.105088f, MeleeWeapon.center, 4f, SpriteEffects.FlipVertically, layerDepth2);
                return;
              default:
                return;
            }
        }
      }
    }
    else
    {
      frameOfFarmerAnimation %= 2;
      switch (facingDirection)
      {
        case 0:
          if (frameOfFarmerAnimation != 0)
          {
            if (frameOfFarmerAnimation != 1)
              break;
            spriteBatch.Draw(texture, new Vector2((float) ((double) playerPosition.X + 64.0 - 16.0), playerPosition.Y - 48f), new Rectangle?(sourceRect), Color.White, -0.7853982f, MeleeWeapon.center, 4f, SpriteEffects.None, layerDepth2);
            break;
          }
          spriteBatch.Draw(texture, new Vector2((float) ((double) playerPosition.X + 64.0 - 4.0), playerPosition.Y - 40f), new Rectangle?(sourceRect), Color.White, -0.7853982f, MeleeWeapon.center, 4f, SpriteEffects.None, layerDepth2);
          break;
        case 1:
          if (frameOfFarmerAnimation != 0)
          {
            if (frameOfFarmerAnimation != 1)
              break;
            spriteBatch.Draw(texture, new Vector2((float) ((double) playerPosition.X + 64.0 - 8.0), playerPosition.Y - 24f), new Rectangle?(sourceRect), Color.White, 0.7853982f, MeleeWeapon.center, 4f, SpriteEffects.None, layerDepth2);
            break;
          }
          spriteBatch.Draw(texture, new Vector2((float) ((double) playerPosition.X + 64.0 - 16.0), playerPosition.Y - 16f), new Rectangle?(sourceRect), Color.White, 0.7853982f, MeleeWeapon.center, 4f, SpriteEffects.None, layerDepth2);
          break;
        case 2:
          if (frameOfFarmerAnimation != 0)
          {
            if (frameOfFarmerAnimation != 1)
              break;
            spriteBatch.Draw(texture, new Vector2(playerPosition.X + 21f, playerPosition.Y + 20f), new Rectangle?(sourceRect), Color.White, 2.3561945f, MeleeWeapon.center, 4f, SpriteEffects.None, layerDepth2);
            break;
          }
          spriteBatch.Draw(texture, new Vector2(playerPosition.X + 32f, playerPosition.Y - 8f), new Rectangle?(sourceRect), Color.White, 2.3561945f, MeleeWeapon.center, 4f, SpriteEffects.None, layerDepth2);
          break;
        case 3:
          if (frameOfFarmerAnimation != 0)
          {
            if (frameOfFarmerAnimation != 1)
              break;
            spriteBatch.Draw(texture, new Vector2(playerPosition.X + 8f, playerPosition.Y - 24f), new Rectangle?(sourceRect), Color.White, -2.3561945f, MeleeWeapon.center, 4f, SpriteEffects.None, layerDepth2);
            break;
          }
          spriteBatch.Draw(texture, new Vector2(playerPosition.X + 16f, playerPosition.Y - 16f), new Rectangle?(sourceRect), Color.White, -2.3561945f, MeleeWeapon.center, 4f, SpriteEffects.None, layerDepth2);
          break;
      }
    }
  }
}
