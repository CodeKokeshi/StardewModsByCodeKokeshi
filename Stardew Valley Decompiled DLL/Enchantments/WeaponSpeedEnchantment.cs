// Decompiled with JetBrains decompiler
// Type: StardewValley.Enchantments.WeaponSpeedEnchantment
// Assembly: Stardew Valley, Version=1.6.15.24356, Culture=neutral, PublicKeyToken=null
// MVID: AA1F513A-94F0-4EF6-A35F-2D5B4A3721BF
// Assembly location: D:\SteamLibrary\steamapps\common\Stardew Valley\Stardew Valley.dll
// XML documentation location: D:\SteamLibrary\steamapps\common\Stardew Valley\Stardew Valley.xml

using StardewValley.Buffs;

#nullable disable
namespace StardewValley.Enchantments;

public class WeaponSpeedEnchantment : BaseWeaponEnchantment
{
  public override bool IsSecondaryEnchantment() => true;

  public override bool IsForge() => false;

  public override void AddEquipmentEffects(BuffEffects effects)
  {
    base.AddEquipmentEffects(effects);
    effects.WeaponSpeedMultiplier.Value += (float) this.level.Value * 0.1f;
  }

  public override int GetMaximumLevel() => 3;

  public override string GetName()
  {
    return Game1.content.LoadString("Strings\\1_6_Strings:SpeedEnchantment", (object) this.Level);
  }
}
