// Decompiled with JetBrains decompiler
// Type: StardewValley.Enchantments.BugKillerEnchantment
// Assembly: Stardew Valley, Version=1.6.15.24356, Culture=neutral, PublicKeyToken=null
// MVID: AA1F513A-94F0-4EF6-A35F-2D5B4A3721BF
// Assembly location: D:\SteamLibrary\steamapps\common\Stardew Valley\Stardew Valley.dll
// XML documentation location: D:\SteamLibrary\steamapps\common\Stardew Valley\Stardew Valley.xml

using StardewValley.Monsters;

#nullable disable
namespace StardewValley.Enchantments;

public class BugKillerEnchantment : BaseWeaponEnchantment
{
  /// <inheritdoc />
  public override void OnCalculateDamage(
    Monster monster,
    GameLocation location,
    Farmer who,
    bool fromBomb,
    ref int amount)
  {
    base.OnCalculateDamage(monster, location, who, fromBomb, ref amount);
    if (fromBomb)
      return;
    switch (monster)
    {
      case Grub _:
      case Fly _:
      case Bug _:
      case Leaper _:
      case RockCrab _:
        amount = (int) ((double) amount * 2.0);
        break;
    }
  }

  public override string GetName() => "Bug Killer";
}
