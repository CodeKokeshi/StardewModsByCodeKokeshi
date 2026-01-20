// Decompiled with JetBrains decompiler
// Type: StardewValley.Enchantments.PowerfulEnchantment
// Assembly: Stardew Valley, Version=1.6.15.24356, Culture=neutral, PublicKeyToken=null
// MVID: AA1F513A-94F0-4EF6-A35F-2D5B4A3721BF
// Assembly location: D:\SteamLibrary\steamapps\common\Stardew Valley\Stardew Valley.dll
// XML documentation location: D:\SteamLibrary\steamapps\common\Stardew Valley\Stardew Valley.xml

using StardewValley.Tools;

#nullable disable
namespace StardewValley.Enchantments;

public class PowerfulEnchantment : BaseEnchantment
{
  public override string GetName() => "Powerful";

  public override bool CanApplyTo(Item item)
  {
    if (!(item is Tool))
      return false;
    return item is Pickaxe || item is Axe;
  }

  protected override void _ApplyTo(Item item)
  {
    base._ApplyTo(item);
    switch (item)
    {
      case Pickaxe pickaxe:
        pickaxe.additionalPower.Value += this.GetLevel();
        break;
      case Axe axe:
        axe.additionalPower.Value += 2 * this.GetLevel();
        break;
    }
  }

  protected override void _UnapplyTo(Item item)
  {
    base._UnapplyTo(item);
    switch (item)
    {
      case Pickaxe pickaxe:
        pickaxe.additionalPower.Value -= this.GetLevel();
        break;
      case Axe axe:
        axe.additionalPower.Value -= 2 * this.GetLevel();
        break;
    }
  }
}
