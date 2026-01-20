// Decompiled with JetBrains decompiler
// Type: StardewValley.Enchantments.GenerousEnchantment
// Assembly: Stardew Valley, Version=1.6.15.24356, Culture=neutral, PublicKeyToken=null
// MVID: AA1F513A-94F0-4EF6-A35F-2D5B4A3721BF
// Assembly location: D:\SteamLibrary\steamapps\common\Stardew Valley\Stardew Valley.dll
// XML documentation location: D:\SteamLibrary\steamapps\common\Stardew Valley\Stardew Valley.xml

using StardewValley.Tools;

#nullable disable
namespace StardewValley.Enchantments;

public class GenerousEnchantment : HoeEnchantment
{
  public override string GetName() => "Generous";

  public override bool CanApplyTo(Item item)
  {
    if (!(item is Tool))
      return false;
    return item is Hoe || item is Pan;
  }
}
