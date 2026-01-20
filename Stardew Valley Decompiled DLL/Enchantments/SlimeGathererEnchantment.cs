// Decompiled with JetBrains decompiler
// Type: StardewValley.Enchantments.SlimeGathererEnchantment
// Assembly: Stardew Valley, Version=1.6.15.24356, Culture=neutral, PublicKeyToken=null
// MVID: AA1F513A-94F0-4EF6-A35F-2D5B4A3721BF
// Assembly location: D:\SteamLibrary\steamapps\common\Stardew Valley\Stardew Valley.dll
// XML documentation location: D:\SteamLibrary\steamapps\common\Stardew Valley\Stardew Valley.xml

using StardewValley.Monsters;
using System;

#nullable disable
namespace StardewValley.Enchantments;

public class SlimeGathererEnchantment : BaseWeaponEnchantment
{
  public override bool IsSecondaryEnchantment() => true;

  public override bool IsForge() => false;

  /// <inheritdoc />
  public override void OnMonsterSlay(
    Monster monster,
    GameLocation location,
    Farmer who,
    bool slainByBomb)
  {
    base.OnMonsterSlay(monster, location, who, slainByBomb);
    if (slainByBomb)
      return;
    switch (monster)
    {
      case GreenSlime _:
      case BigSlime _:
        Game1.createMultipleItemDebris(ItemRegistry.Create("(O)766", 1 + Game1.random.Next((int) Math.Ceiling(Math.Sqrt((double) monster.MaxHealth) / 3.0))), monster.getStandingPosition(), -1);
        break;
    }
  }

  public override int GetMaximumLevel() => 5;

  public override string GetName()
  {
    return Game1.content.LoadString("Strings\\1_6_Strings:SlimeGathererEnchantment");
  }
}
