// Decompiled with JetBrains decompiler
// Type: StardewValley.Internal.ForEachItemContext
// Assembly: Stardew Valley, Version=1.6.15.24356, Culture=neutral, PublicKeyToken=null
// MVID: AA1F513A-94F0-4EF6-A35F-2D5B4A3721BF
// Assembly location: D:\SteamLibrary\steamapps\common\Stardew Valley\Stardew Valley.dll
// XML documentation location: D:\SteamLibrary\steamapps\common\Stardew Valley\Stardew Valley.xml

using Microsoft.Xna.Framework;
using Netcode;
using StardewValley.Buildings;
using StardewValley.Delegates;
using StardewValley.Inventories;
using StardewValley.Network;
using System;
using System.Collections.Generic;

#nullable disable
namespace StardewValley.Internal;

/// <summary>The metadata and operations for an item being iterated via a method like <see cref="M:StardewValley.Utility.ForEachItem(System.Func{StardewValley.Item,System.Boolean})" />.</summary>
/// <summary>Set the contextual values. This should only be called by the code which implements the iteration.</summary>
/// <param name="item"><inheritdoc cref="F:StardewValley.Internal.ForEachItemContext.Item" path="/summary" /></param>
/// <param name="remove"><inheritdoc cref="F:StardewValley.Internal.ForEachItemContext.RemoveItem" path="/summary" /></param>
/// <param name="replaceWith"><inheritdoc cref="F:StardewValley.Internal.ForEachItemContext.ReplaceItemWith" path="/summary" /></param>
/// <param name="getPath"><inheritdoc cref="F:StardewValley.Internal.ForEachItemContext.GetPath" path="/summary" /></param>
public readonly struct ForEachItemContext(
  Item item,
  Action remove,
  Action<Item> replaceWith,
  GetForEachItemPathDelegate getPath)
{
  /// <summary>The current item in the iteration.</summary>
  public readonly Item Item = item;
  /// <summary>Delete this item from the game.</summary>
  public readonly Action RemoveItem = remove;
  /// <summary>Remove this item and replace it with the given instance.</summary>
  public readonly Action<Item> ReplaceItemWith = replaceWith;
  /// <summary>Get the contextual path leading to this item. For example, an item inside a chest would have the location and chest as path values.</summary>
  public readonly GetForEachItemPathDelegate GetPath = getPath;

  /// <summary>Get a human-readable representation of the <see cref="F:StardewValley.Internal.ForEachItemContext.GetPath" /> values.</summary>
  /// <param name="includeItem">Whether to add a segment for the item itself.</param>
  public IList<string> GetDisplayPath(bool includeItem = false)
  {
    List<string> path = new List<string>();
    foreach (object pathValue in (IEnumerable<object>) this.GetPath())
      this.AddDisplayPath((IList<string>) path, pathValue);
    if (includeItem)
      this.AddDisplayPath((IList<string>) path, (object) this.Item);
    return (IList<string>) path;
  }

  /// <summary>Add human-readable path segments path for a raw <see cref="F:StardewValley.Internal.ForEachItemContext.GetPath" /> value.</summary>
  /// <param name="path">The path to populate.</param>
  /// <param name="pathValue">The segment from <see cref="F:StardewValley.Internal.ForEachItemContext.GetPath" /> to represent.</param>
  private void AddDisplayPath(IList<string> path, object pathValue)
  {
    switch (pathValue)
    {
      case GameLocation gameLocation:
        if (path.Count == 0 && gameLocation.ParentBuilding != null)
          this.AddDisplayPath(path, (object) gameLocation.ParentBuilding);
        path.Add(gameLocation.NameOrUniqueName);
        break;
      case Building building:
        if (path.Count == 0)
        {
          GameLocation parentLocation = building.GetParentLocation();
          if (parentLocation != null)
            this.AddDisplayPath(path, (object) parentLocation);
        }
        path.Add($"{building.buildingType.Value} at {building.tileX.Value}, {building.tileY.Value}");
        break;
      case StardewValley.Object @object:
        if (path.Count == 0 && @object.Location != null)
          this.AddDisplayPath(path, (object) @object.Location);
        IList<string> stringList = path;
        string str;
        if (!(@object.TileLocation != Vector2.Zero))
          str = @object.Name;
        else
          str = $"{@object.Name} at {@object.TileLocation.X}, {@object.TileLocation.Y}";
        stringList.Add(str);
        break;
      case Farmer farmer:
        path.Add($"player '{farmer.Name}'");
        break;
      case Item obj:
        path.Add(obj.Name);
        break;
      case INetSerializable netSerializable:
        path.Add(netSerializable.Name);
        break;
      case IInventory _:
        break;
      case OverlaidDictionary _:
        break;
      default:
        path.Add(pathValue.ToString());
        break;
    }
  }
}
