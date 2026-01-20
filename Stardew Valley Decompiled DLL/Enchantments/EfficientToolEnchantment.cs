// Decompiled with JetBrains decompiler
// Type: StardewValley.Enchantments.EfficientToolEnchantment
// Assembly: Stardew Valley, Version=1.6.15.24356, Culture=neutral, PublicKeyToken=null
// MVID: AA1F513A-94F0-4EF6-A35F-2D5B4A3721BF
// Assembly location: D:\SteamLibrary\steamapps\common\Stardew Valley\Stardew Valley.dll
// XML documentation location: D:\SteamLibrary\steamapps\common\Stardew Valley\Stardew Valley.xml

using StardewValley.Tools;

#nullable disable
namespace StardewValley.Enchantments;

public class EfficientToolEnchantment : BaseEnchantment
{
  public override string GetName() => "Efficient";

  public override bool CanApplyTo(Item item)
  {
    return item is Tool && !(item is MilkPail) && !(item is MeleeWeapon) && !(item is Shears) && !(item is Pan) && !(item is Wand) && !(item is Slingshot);
  }

  protected override void _ApplyTo(Item item)
  {
    base._ApplyTo(item);
    if (!(item is Tool tool))
      return;
    tool.IsEfficient = true;
  }

  protected override void _UnapplyTo(Item item)
  {
    base._UnapplyTo(item);
    if (!(item is Tool tool))
      return;
    tool.IsEfficient = false;
  }
}
