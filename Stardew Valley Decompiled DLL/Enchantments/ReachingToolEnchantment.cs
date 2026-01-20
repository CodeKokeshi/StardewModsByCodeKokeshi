// Decompiled with JetBrains decompiler
// Type: StardewValley.Enchantments.ReachingToolEnchantment
// Assembly: Stardew Valley, Version=1.6.15.24356, Culture=neutral, PublicKeyToken=null
// MVID: AA1F513A-94F0-4EF6-A35F-2D5B4A3721BF
// Assembly location: D:\SteamLibrary\steamapps\common\Stardew Valley\Stardew Valley.dll
// XML documentation location: D:\SteamLibrary\steamapps\common\Stardew Valley\Stardew Valley.xml

using StardewValley.Tools;

#nullable disable
namespace StardewValley.Enchantments;

public class ReachingToolEnchantment : BaseEnchantment
{
  public override string GetName() => "Expansive";

  public override bool CanApplyTo(Item item)
  {
    if (item is Tool tool)
    {
      switch (tool)
      {
        case WateringCan _:
        case Hoe _:
        case Pan _:
          return tool.UpgradeLevel == 4;
      }
    }
    return false;
  }
}
