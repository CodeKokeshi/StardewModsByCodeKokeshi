// Decompiled with JetBrains decompiler
// Type: StardewValley.Enchantments.VampiricEnchantment
// Assembly: Stardew Valley, Version=1.6.15.24356, Culture=neutral, PublicKeyToken=null
// MVID: AA1F513A-94F0-4EF6-A35F-2D5B4A3721BF
// Assembly location: D:\SteamLibrary\steamapps\common\Stardew Valley\Stardew Valley.dll
// XML documentation location: D:\SteamLibrary\steamapps\common\Stardew Valley\Stardew Valley.xml

using Microsoft.Xna.Framework;
using StardewValley.Monsters;
using System;

#nullable disable
namespace StardewValley.Enchantments;

/// <summary>Siphons health from monsters sometimes.</summary>
public class VampiricEnchantment : BaseWeaponEnchantment
{
  /// <inheritdoc />
  public override void OnMonsterSlay(
    Monster monster,
    GameLocation location,
    Farmer who,
    bool slainByBomb)
  {
    base.OnMonsterSlay(monster, location, who, slainByBomb);
    if (slainByBomb || Game1.random.NextDouble() >= 0.090000003576278687)
      return;
    int number = Math.Max(1, (int) ((double) (monster.MaxHealth + Game1.random.Next(-monster.MaxHealth / 10, monster.MaxHealth / 15 + 1)) * 0.10000000149011612));
    who.health = Math.Min(who.maxHealth, who.health + number);
    location.debris.Add(new Debris(number, who.getStandingPosition(), Color.Lime, 1f, (Character) who));
    Game1.playSound("healSound");
  }

  public override string GetName() => "Vampiric";
}
