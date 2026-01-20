// Decompiled with JetBrains decompiler
// Type: StardewValley.Buildings.Mill
// Assembly: Stardew Valley, Version=1.6.15.24356, Culture=neutral, PublicKeyToken=null
// MVID: AA1F513A-94F0-4EF6-A35F-2D5B4A3721BF
// Assembly location: D:\SteamLibrary\steamapps\common\Stardew Valley\Stardew Valley.dll
// XML documentation location: D:\SteamLibrary\steamapps\common\Stardew Valley\Stardew Valley.xml

using Microsoft.Xna.Framework;
using StardewValley.Inventories;
using StardewValley.Objects;
using System;
using System.Xml.Serialization;

#nullable disable
namespace StardewValley.Buildings;

[Obsolete("The Mill class is only used to preserve data from old save files. All mills were converted into plain Building instances based on the rules in Data/Buildings. The input and output items are now stored in Building.buildingChests with the 'Input' and 'Output' keys respectively.")]
public class Mill(Vector2 tileLocation) : Building(nameof (Mill), tileLocation)
{
  /// <summary>Obsolete. The <c>Mill</c> class is only used to preserve data from old save files. All mills were converted into plain <see cref="T:StardewValley.Buildings.Building" /> instances, with the input items in <see cref="F:StardewValley.Buildings.Building.buildingChests" /> with the <c>Input</c> key.</summary>
  [XmlElement("input")]
  public Chest obsolete_input;
  /// <summary>Obsolete. The <c>Mill</c> class is only used to preserve data from old save files. All mills were converted into plain <see cref="T:StardewValley.Buildings.Building" /> instances, with the output items in <see cref="F:StardewValley.Buildings.Building.buildingChests" /> with the <c>Output</c> key.</summary>
  [XmlElement("output")]
  public Chest obsolete_output;

  public Mill()
    : this(Vector2.Zero)
  {
  }

  /// <summary>Copy the data from this mill to a new data-driven building instance.</summary>
  /// <param name="targetBuilding">The new building that will replace this instance.</param>
  public void TransferValuesToNewBuilding(Building targetBuilding)
  {
    Chest obsoleteInput = this.obsolete_input;
    int? count;
    int num1;
    if (obsoleteInput == null)
    {
      num1 = 0;
    }
    else
    {
      count = obsoleteInput.Items?.Count;
      int num2 = 0;
      num1 = count.GetValueOrDefault() > num2 & count.HasValue ? 1 : 0;
    }
    if (num1 != 0)
    {
      IInventory items = (IInventory) this.obsolete_input.Items;
      Chest buildingChest = targetBuilding.GetBuildingChest("Input");
      for (int index = 0; index < items.Count; ++index)
      {
        Item obj = items[index];
        if (obj != null)
        {
          items[index] = (Item) null;
          buildingChest.addItem(obj);
        }
      }
      this.obsolete_input = (Chest) null;
    }
    Chest obsoleteOutput = this.obsolete_output;
    int num3;
    if (obsoleteOutput == null)
    {
      num3 = 0;
    }
    else
    {
      count = obsoleteOutput.Items?.Count;
      int num4 = 0;
      num3 = count.GetValueOrDefault() > num4 & count.HasValue ? 1 : 0;
    }
    if (num3 == 0)
      return;
    IInventory items1 = (IInventory) this.obsolete_output.Items;
    Chest buildingChest1 = targetBuilding.GetBuildingChest("Output");
    for (int index = 0; index < items1.Count; ++index)
    {
      Item obj = items1[index];
      if (obj != null)
      {
        items1[index] = (Item) null;
        buildingChest1.addItem(obj);
      }
    }
    this.obsolete_output = (Chest) null;
  }
}
