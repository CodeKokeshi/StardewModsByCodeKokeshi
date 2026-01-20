// Decompiled with JetBrains decompiler
// Type: StardewValley.Enchantments.BottomlessEnchantment
// Assembly: Stardew Valley, Version=1.6.15.24356, Culture=neutral, PublicKeyToken=null
// MVID: AA1F513A-94F0-4EF6-A35F-2D5B4A3721BF
// Assembly location: D:\SteamLibrary\steamapps\common\Stardew Valley\Stardew Valley.dll
// XML documentation location: D:\SteamLibrary\steamapps\common\Stardew Valley\Stardew Valley.xml

using StardewValley.Tools;

#nullable disable
namespace StardewValley.Enchantments;

public class BottomlessEnchantment : WateringCanEnchantment
{
  public override string GetName() => "Bottomless";

  protected override void _ApplyTo(Item item)
  {
    base._ApplyTo(item);
    if (!(item is WateringCan wateringCan))
      return;
    wateringCan.IsBottomless = true;
    wateringCan.WaterLeft = wateringCan.waterCanMax;
  }

  protected override void _UnapplyTo(Item item)
  {
    base._UnapplyTo(item);
    if (!(item is WateringCan wateringCan))
      return;
    wateringCan.IsBottomless = false;
  }
}
