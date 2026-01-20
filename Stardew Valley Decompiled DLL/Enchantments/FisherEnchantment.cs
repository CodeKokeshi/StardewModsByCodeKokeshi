// Decompiled with JetBrains decompiler
// Type: StardewValley.Enchantments.FisherEnchantment
// Assembly: Stardew Valley, Version=1.6.15.24356, Culture=neutral, PublicKeyToken=null
// MVID: AA1F513A-94F0-4EF6-A35F-2D5B4A3721BF
// Assembly location: D:\SteamLibrary\steamapps\common\Stardew Valley\Stardew Valley.dll
// XML documentation location: D:\SteamLibrary\steamapps\common\Stardew Valley\Stardew Valley.xml

using StardewValley.Tools;

#nullable disable
namespace StardewValley.Enchantments;

public class FisherEnchantment : BaseEnchantment
{
  public override string GetName() => "Fisher";

  public override bool CanApplyTo(Item item) => item is Tool && item is Pan;
}
