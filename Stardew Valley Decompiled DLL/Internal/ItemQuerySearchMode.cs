// Decompiled with JetBrains decompiler
// Type: StardewValley.Internal.ItemQuerySearchMode
// Assembly: Stardew Valley, Version=1.6.15.24356, Culture=neutral, PublicKeyToken=null
// MVID: AA1F513A-94F0-4EF6-A35F-2D5B4A3721BF
// Assembly location: D:\SteamLibrary\steamapps\common\Stardew Valley\Stardew Valley.dll
// XML documentation location: D:\SteamLibrary\steamapps\common\Stardew Valley\Stardew Valley.xml

#nullable disable
namespace StardewValley.Internal;

/// <summary>The filter to apply to an item query's search results.</summary>
public enum ItemQuerySearchMode
{
  /// <summary>Return all matches.</summary>
  All,
  /// <summary>Return all matches which are a concrete <see cref="T:StardewValley.Item" /> (instead of a different <see cref="T:StardewValley.ISalable" /> type).</summary>
  AllOfTypeItem,
  /// <summary>Return the first match which is a concrete <see cref="T:StardewValley.Item" /> (instead of a different <see cref="T:StardewValley.ISalable" /> type).</summary>
  FirstOfTypeItem,
  /// <summary>Return a random match which is a concrete <see cref="T:StardewValley.Item" /> (instead of a different <see cref="T:StardewValley.ISalable" /> type).</summary>
  RandomOfTypeItem,
}
