// Decompiled with JetBrains decompiler
// Type: StardewValley.Enchantments.JadeEnchantment
// Assembly: Stardew Valley, Version=1.6.15.24356, Culture=neutral, PublicKeyToken=null
// MVID: AA1F513A-94F0-4EF6-A35F-2D5B4A3721BF
// Assembly location: D:\SteamLibrary\steamapps\common\Stardew Valley\Stardew Valley.dll
// XML documentation location: D:\SteamLibrary\steamapps\common\Stardew Valley\Stardew Valley.xml

using StardewValley.Tools;

#nullable disable
namespace StardewValley.Enchantments;

public class JadeEnchantment : BaseWeaponEnchantment
{
  protected override void _ApplyTo(Item item)
  {
    base._ApplyTo(item);
    if (!(item is MeleeWeapon meleeWeapon))
      return;
    meleeWeapon.critMultiplier.Value += 0.1f * (float) this.GetLevel();
  }

  protected override void _UnapplyTo(Item item)
  {
    base._UnapplyTo(item);
    if (!(item is MeleeWeapon meleeWeapon))
      return;
    meleeWeapon.critMultiplier.Value -= 0.1f * (float) this.GetLevel();
  }

  public override bool ShouldBeDisplayed() => false;

  public override bool IsForge() => true;
}
