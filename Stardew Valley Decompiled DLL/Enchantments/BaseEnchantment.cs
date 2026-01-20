// Decompiled with JetBrains decompiler
// Type: StardewValley.Enchantments.BaseEnchantment
// Assembly: Stardew Valley, Version=1.6.15.24356, Culture=neutral, PublicKeyToken=null
// MVID: AA1F513A-94F0-4EF6-A35F-2D5B4A3721BF
// Assembly location: D:\SteamLibrary\steamapps\common\Stardew Valley\Stardew Valley.dll
// XML documentation location: D:\SteamLibrary\steamapps\common\Stardew Valley\Stardew Valley.xml

using Microsoft.Xna.Framework;
using Netcode;
using StardewValley.Buffs;
using StardewValley.Extensions;
using StardewValley.Monsters;
using StardewValley.Tools;
using System;
using System.Collections.Generic;
using System.Xml.Serialization;

#nullable disable
namespace StardewValley.Enchantments;

[XmlInclude(typeof (BaseWeaponEnchantment))]
[XmlInclude(typeof (ArtfulEnchantment))]
[XmlInclude(typeof (BugKillerEnchantment))]
[XmlInclude(typeof (CrusaderEnchantment))]
[XmlInclude(typeof (HaymakerEnchantment))]
[XmlInclude(typeof (MagicEnchantment))]
[XmlInclude(typeof (VampiricEnchantment))]
[XmlInclude(typeof (AxeEnchantment))]
[XmlInclude(typeof (HoeEnchantment))]
[XmlInclude(typeof (MilkPailEnchantment))]
[XmlInclude(typeof (PanEnchantment))]
[XmlInclude(typeof (PickaxeEnchantment))]
[XmlInclude(typeof (ShearsEnchantment))]
[XmlInclude(typeof (WateringCanEnchantment))]
[XmlInclude(typeof (ArchaeologistEnchantment))]
[XmlInclude(typeof (AutoHookEnchantment))]
[XmlInclude(typeof (BottomlessEnchantment))]
[XmlInclude(typeof (EfficientToolEnchantment))]
[XmlInclude(typeof (GenerousEnchantment))]
[XmlInclude(typeof (MasterEnchantment))]
[XmlInclude(typeof (PowerfulEnchantment))]
[XmlInclude(typeof (PreservingEnchantment))]
[XmlInclude(typeof (ReachingToolEnchantment))]
[XmlInclude(typeof (ShavingEnchantment))]
[XmlInclude(typeof (SwiftToolEnchantment))]
[XmlInclude(typeof (FisherEnchantment))]
[XmlInclude(typeof (AmethystEnchantment))]
[XmlInclude(typeof (AquamarineEnchantment))]
[XmlInclude(typeof (DiamondEnchantment))]
[XmlInclude(typeof (EmeraldEnchantment))]
[XmlInclude(typeof (JadeEnchantment))]
[XmlInclude(typeof (RubyEnchantment))]
[XmlInclude(typeof (TopazEnchantment))]
[XmlInclude(typeof (AttackEnchantment))]
[XmlInclude(typeof (DefenseEnchantment))]
[XmlInclude(typeof (SlimeSlayerEnchantment))]
[XmlInclude(typeof (CritEnchantment))]
[XmlInclude(typeof (WeaponSpeedEnchantment))]
[XmlInclude(typeof (CritPowerEnchantment))]
[XmlInclude(typeof (LightweightEnchantment))]
[XmlInclude(typeof (SlimeGathererEnchantment))]
[XmlInclude(typeof (GalaxySoulEnchantment))]
public class BaseEnchantment : INetObject<NetFields>
{
  [XmlIgnore]
  protected string _displayName;
  [XmlIgnore]
  protected bool _applied;
  [XmlIgnore]
  [InstancedStatic]
  public static bool hideEnchantmentName;
  [XmlIgnore]
  [InstancedStatic]
  public static bool hideSecondaryEnchantName;
  protected static List<BaseEnchantment> _enchantments;
  protected readonly NetInt level = new NetInt(1);

  [XmlIgnore]
  public NetFields NetFields { get; } = new NetFields(nameof (BaseEnchantment));

  [XmlElement("level")]
  public int Level
  {
    get => this.level.Value;
    set => this.level.Value = value;
  }

  public BaseEnchantment() => this.InitializeNetFields();

  public static BaseEnchantment GetEnchantmentFromItem(Item base_item, Item item)
  {
    if (base_item == null || base_item is MeleeWeapon meleeWeapon1 && !meleeWeapon1.isScythe())
    {
      string qualifiedItemId = item?.QualifiedItemId;
      if (qualifiedItemId != null)
      {
        switch (qualifiedItemId.Length)
        {
          case 5:
            switch (qualifiedItemId[4])
            {
              case '0':
                switch (qualifiedItemId)
                {
                  case "(O)60":
                    return (BaseEnchantment) new EmeraldEnchantment();
                  case "(O)70":
                    return (BaseEnchantment) new JadeEnchantment();
                }
                break;
              case '2':
                switch (qualifiedItemId)
                {
                  case "(O)62":
                    return (BaseEnchantment) new AquamarineEnchantment();
                  case "(O)72":
                    return (BaseEnchantment) new DiamondEnchantment();
                }
                break;
              case '4':
                if (qualifiedItemId == "(O)64")
                  return (BaseEnchantment) new RubyEnchantment();
                break;
              case '6':
                if (qualifiedItemId == "(O)66")
                  return (BaseEnchantment) new AmethystEnchantment();
                break;
              case '8':
                if (qualifiedItemId == "(O)68")
                  return (BaseEnchantment) new TopazEnchantment();
                break;
            }
            break;
          case 6:
            if (qualifiedItemId == "(O)896")
            {
              bool? nullable = base_item is MeleeWeapon meleeWeapon ? new bool?(meleeWeapon.isGalaxyWeapon()) : new bool?();
              if (nullable.HasValue && nullable.GetValueOrDefault())
                return (BaseEnchantment) new GalaxySoulEnchantment();
              break;
            }
            break;
        }
      }
    }
    return item?.QualifiedItemId == "(O)74" ? Utility.CreateRandom((double) Game1.stats.Get("timesEnchanted"), (double) Game1.uniqueIDForThisGame, (double) Game1.player.UniqueMultiplayerID).ChooseFrom<BaseEnchantment>((IList<BaseEnchantment>) BaseEnchantment.GetAvailableEnchantmentsForItem(base_item as Tool)) : (BaseEnchantment) null;
  }

  public static List<BaseEnchantment> GetAvailableEnchantmentsForItem(Tool item)
  {
    List<BaseEnchantment> enchantmentsForItem = new List<BaseEnchantment>();
    if (item == null)
      return BaseEnchantment.GetAvailableEnchantments();
    List<BaseEnchantment> availableEnchantments = BaseEnchantment.GetAvailableEnchantments();
    HashSet<Type> typeSet = new HashSet<Type>();
    foreach (BaseEnchantment enchantment in item.enchantments)
      typeSet.Add(enchantment.GetType());
    foreach (BaseEnchantment baseEnchantment in availableEnchantments)
    {
      if (baseEnchantment.CanApplyTo((Item) item) && !typeSet.Contains(baseEnchantment.GetType()))
        enchantmentsForItem.Add(baseEnchantment);
    }
    foreach (string previousEnchantment1 in (NetList<string, NetString>) item.previousEnchantments)
    {
      string previousEnchantment = previousEnchantment1;
      if (enchantmentsForItem.Count > 1)
        enchantmentsForItem.RemoveAll((Predicate<BaseEnchantment>) (cur => cur.GetName() == previousEnchantment));
      else
        break;
    }
    return enchantmentsForItem;
  }

  public static List<BaseEnchantment> GetAvailableEnchantments()
  {
    if (BaseEnchantment._enchantments == null)
      BaseEnchantment._enchantments = new List<BaseEnchantment>()
      {
        (BaseEnchantment) new ArtfulEnchantment(),
        (BaseEnchantment) new BugKillerEnchantment(),
        (BaseEnchantment) new VampiricEnchantment(),
        (BaseEnchantment) new CrusaderEnchantment(),
        (BaseEnchantment) new HaymakerEnchantment(),
        (BaseEnchantment) new PowerfulEnchantment(),
        (BaseEnchantment) new ReachingToolEnchantment(),
        (BaseEnchantment) new ShavingEnchantment(),
        (BaseEnchantment) new BottomlessEnchantment(),
        (BaseEnchantment) new GenerousEnchantment(),
        (BaseEnchantment) new ArchaeologistEnchantment(),
        (BaseEnchantment) new MasterEnchantment(),
        (BaseEnchantment) new AutoHookEnchantment(),
        (BaseEnchantment) new PreservingEnchantment(),
        (BaseEnchantment) new EfficientToolEnchantment(),
        (BaseEnchantment) new SwiftToolEnchantment(),
        (BaseEnchantment) new FisherEnchantment()
      };
    return BaseEnchantment._enchantments;
  }

  /// <summary>Reset cached enchantment data.</summary>
  public static void ResetEnchantments()
  {
    BaseEnchantment._enchantments = (List<BaseEnchantment>) null;
  }

  public virtual bool IsForge() => false;

  public virtual bool IsSecondaryEnchantment() => false;

  public virtual void InitializeNetFields()
  {
    this.NetFields.SetOwner((INetObject<NetFields>) this).AddField((INetSerializable) this.level, "level");
  }

  public void OnEquip(Farmer farmer)
  {
    if (this._applied)
      return;
    farmer.enchantments.Add(this);
    this._applied = true;
    this._OnEquip(farmer);
  }

  public void OnUnequip(Farmer farmer)
  {
    if (!this._applied)
      return;
    farmer.enchantments.Remove(this);
    this._applied = false;
    this._OnUnequip(farmer);
  }

  protected virtual void _OnEquip(Farmer who)
  {
  }

  protected virtual void _OnUnequip(Farmer who)
  {
  }

  /// <summary>Apply effects to the base damage applied to a monster before modifiers like the monster's defense stat.</summary>
  /// <param name="monster">The monster being attacked.</param>
  /// <param name="location">The location containing the monster.</param>
  /// <param name="who">The player attacking the monster.</param>
  /// <param name="fromBomb">Whether the damage is from a bomb placed by the player.</param>
  /// <param name="amount">The amount of damage that will be applied.</param>
  public virtual void OnCalculateDamage(
    Monster monster,
    GameLocation location,
    Farmer who,
    bool fromBomb,
    ref int amount)
  {
  }

  /// <summary>Apply effects after damage is applied to a monster.</summary>
  /// <param name="monster">The monster being attacked.</param>
  /// <param name="location">The location containing the monster.</param>
  /// <param name="who">The player attacking the monster.</param>
  /// <param name="fromBomb">Whether the damage is from a bomb placed by the player.</param>
  /// <param name="amount">The amount of damage that was applied, after accounting for modifiers like the monster's defense stat.</param>
  public virtual void OnDealtDamage(
    Monster monster,
    GameLocation location,
    Farmer who,
    bool fromBomb,
    int amount)
  {
  }

  /// <summary>Apply effects when a monster is slain.</summary>
  /// <param name="monster">The monster slain.</param>
  /// <param name="location">The location where the monster was slain.</param>
  /// <param name="who">The player which has the enchantment effects.</param>
  /// <param name="slainByBomb">Whether the monster was killed by a bomb placed by the player.</param>
  public virtual void OnMonsterSlay(
    Monster monster,
    GameLocation location,
    Farmer who,
    bool slainByBomb)
  {
  }

  public virtual void AddEquipmentEffects(BuffEffects effects)
  {
  }

  public void OnCutWeed(Vector2 tile_location, GameLocation location, Farmer who)
  {
    this._OnCutWeed(tile_location, location, who);
  }

  protected virtual void _OnCutWeed(Vector2 tile_location, GameLocation location, Farmer who)
  {
  }

  public virtual BaseEnchantment GetOne()
  {
    BaseEnchantment instance = Activator.CreateInstance(this.GetType()) as BaseEnchantment;
    instance.level.Value = this.level.Value;
    return instance;
  }

  public int GetLevel() => this.level.Value;

  public void SetLevel(Item item, int new_level)
  {
    if (new_level < 1)
      new_level = 1;
    else if (this.GetMaximumLevel() >= 0 && new_level > this.GetMaximumLevel())
      new_level = this.GetMaximumLevel();
    if (this.level.Value == new_level)
      return;
    this.UnapplyTo(item);
    this.level.Value = new_level;
    this.ApplyTo(item);
  }

  public virtual int GetMaximumLevel() => -1;

  public void ApplyTo(Item item, Farmer farmer = null)
  {
    this._ApplyTo(item);
    if (!this.IsItemCurrentlyEquipped(item, farmer))
      return;
    this.OnEquip(farmer);
  }

  protected virtual void _ApplyTo(Item item)
  {
  }

  public bool IsItemCurrentlyEquipped(Item item, Farmer farmer)
  {
    return farmer != null && this._IsCurrentlyEquipped(item, farmer);
  }

  protected virtual bool _IsCurrentlyEquipped(Item item, Farmer farmer)
  {
    return farmer.CurrentTool == item;
  }

  public void UnapplyTo(Item item, Farmer farmer = null)
  {
    this._UnapplyTo(item);
    if (!this.IsItemCurrentlyEquipped(item, farmer))
      return;
    this.OnUnequip(farmer);
  }

  protected virtual void _UnapplyTo(Item item)
  {
  }

  public virtual bool CanApplyTo(Item item) => true;

  public string GetDisplayName()
  {
    if (this._displayName == null)
    {
      this._displayName = Game1.content.LoadStringReturnNullIfNotFound("Strings\\EnchantmentNames:" + this.GetName());
      if (this._displayName == null)
        this._displayName = this.GetName();
    }
    return this._displayName;
  }

  public virtual string GetName() => "Unknown Enchantment";

  public virtual bool ShouldBeDisplayed() => true;
}
