// Decompiled with JetBrains decompiler
// Type: StardewValley.Enchantments.RubyEnchantment
// Assembly: Stardew Valley, Version=1.6.15.24356, Culture=neutral, PublicKeyToken=null
// MVID: AA1F513A-94F0-4EF6-A35F-2D5B4A3721BF
// Assembly location: D:\SteamLibrary\steamapps\common\Stardew Valley\Stardew Valley.dll
// XML documentation location: D:\SteamLibrary\steamapps\common\Stardew Valley\Stardew Valley.xml

using StardewValley.GameData.Weapons;
using StardewValley.Tools;
using System;

#nullable disable
namespace StardewValley.Enchantments;

public class RubyEnchantment : BaseWeaponEnchantment
{
  protected override void _ApplyTo(Item item)
  {
    base._ApplyTo(item);
    if (!(item is MeleeWeapon meleeWeapon))
      return;
    WeaponData data = meleeWeapon.GetData();
    if (data == null)
      return;
    int minDamage = data.MinDamage;
    int maxDamage = data.MaxDamage;
    meleeWeapon.minDamage.Value += Math.Max(1, (int) ((double) minDamage * 0.10000000149011612)) * this.GetLevel();
    meleeWeapon.maxDamage.Value += Math.Max(1, (int) ((double) maxDamage * 0.10000000149011612)) * this.GetLevel();
  }

  protected override void _UnapplyTo(Item item)
  {
    base._UnapplyTo(item);
    if (!(item is MeleeWeapon meleeWeapon))
      return;
    WeaponData data = meleeWeapon.GetData();
    if (data == null)
      return;
    int minDamage = data.MinDamage;
    int maxDamage = data.MaxDamage;
    meleeWeapon.minDamage.Value -= Math.Max(1, (int) ((double) minDamage * 0.10000000149011612)) * this.GetLevel();
    meleeWeapon.maxDamage.Value -= Math.Max(1, (int) ((double) maxDamage * 0.10000000149011612)) * this.GetLevel();
  }

  public override bool ShouldBeDisplayed() => false;

  public override bool IsForge() => true;
}
