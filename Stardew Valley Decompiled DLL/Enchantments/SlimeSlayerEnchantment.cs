// Decompiled with JetBrains decompiler
// Type: StardewValley.Enchantments.SlimeSlayerEnchantment
// Assembly: Stardew Valley, Version=1.6.15.24356, Culture=neutral, PublicKeyToken=null
// MVID: AA1F513A-94F0-4EF6-A35F-2D5B4A3721BF
// Assembly location: D:\SteamLibrary\steamapps\common\Stardew Valley\Stardew Valley.dll
// XML documentation location: D:\SteamLibrary\steamapps\common\Stardew Valley\Stardew Valley.xml

using StardewValley.Monsters;

#nullable disable
namespace StardewValley.Enchantments;

public class SlimeSlayerEnchantment : BaseWeaponEnchantment
{
  public override bool IsSecondaryEnchantment() => true;

  public override bool IsForge() => false;

  /// <inheritdoc />
  public override void OnCalculateDamage(
    Monster monster,
    GameLocation location,
    Farmer who,
    bool fromBomb,
    ref int amount)
  {
    base.OnCalculateDamage(monster, location, who, fromBomb, ref amount);
    if (fromBomb || !(monster is GreenSlime))
      return;
    amount = (int) ((double) amount * 1.3300000429153442 + 1.0);
  }

  public override int GetMaximumLevel() => 5;

  public override string GetName()
  {
    return Game1.content.LoadString("Strings\\1_6_Strings:SlimeSlayerEnchantment");
  }
}
