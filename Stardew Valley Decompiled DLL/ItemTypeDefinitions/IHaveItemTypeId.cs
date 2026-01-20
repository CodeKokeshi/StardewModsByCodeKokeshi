// Decompiled with JetBrains decompiler
// Type: StardewValley.ItemTypeDefinitions.IHaveItemTypeId
// Assembly: Stardew Valley, Version=1.6.15.24356, Culture=neutral, PublicKeyToken=null
// MVID: AA1F513A-94F0-4EF6-A35F-2D5B4A3721BF
// Assembly location: D:\SteamLibrary\steamapps\common\Stardew Valley\Stardew Valley.dll
// XML documentation location: D:\SteamLibrary\steamapps\common\Stardew Valley\Stardew Valley.xml

#nullable disable
namespace StardewValley.ItemTypeDefinitions;

/// <summary>An item or item data which has an item data definition ID.</summary>
public interface IHaveItemTypeId
{
  /// <summary>Get the unique ID of the item data definition which specifies this item, like <c>(H)</c> for a hat.</summary>
  /// <remarks>For vanilla items, this matches one of the <see cref="T:StardewValley.ItemRegistry" />'s <c>type_*</c> fields.</remarks>
  string GetItemTypeId();
}
