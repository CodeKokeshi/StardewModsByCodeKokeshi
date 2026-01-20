// Decompiled with JetBrains decompiler
// Type: StardewValley.Quests.ItemHarvestQuest
// Assembly: Stardew Valley, Version=1.6.15.24356, Culture=neutral, PublicKeyToken=null
// MVID: AA1F513A-94F0-4EF6-A35F-2D5B4A3721BF
// Assembly location: D:\SteamLibrary\steamapps\common\Stardew Valley\Stardew Valley.dll
// XML documentation location: D:\SteamLibrary\steamapps\common\Stardew Valley\Stardew Valley.xml

using Netcode;
using System.Xml.Serialization;

#nullable disable
namespace StardewValley.Quests;

public class ItemHarvestQuest : Quest
{
  /// <summary>The qualified item ID to harvest.</summary>
  [XmlElement("itemIndex")]
  public readonly NetString ItemId = new NetString();
  /// <summary>The number of items that must be harvested.</summary>
  [XmlElement("number")]
  public readonly NetInt Number = new NetInt();

  /// <summary>Construct an instance.</summary>
  public ItemHarvestQuest()
  {
  }

  /// <summary>Construct an instance.</summary>
  /// <param name="itemId">The qualified or unqualified item ID to harvest.</param>
  /// <param name="number">The number of items that must be harvested.</param>
  public ItemHarvestQuest(string itemId, int number = 1)
  {
    this.ItemId.Value = ItemRegistry.QualifyItemId(itemId) ?? itemId;
    this.Number.Value = number;
    this.questType.Value = 9;
  }

  /// <inheritdoc />
  protected override void initNetFields()
  {
    base.initNetFields();
    this.NetFields.AddField((INetSerializable) this.ItemId, "ItemId").AddField((INetSerializable) this.Number, "Number");
  }

  /// <inheritdoc />
  public override bool OnItemReceived(Item item, int numberAdded, bool probe = false)
  {
    bool flag1 = base.OnItemReceived(item, numberAdded, probe);
    if (this.completed.Value || !(item.QualifiedItemId == this.ItemId.Value) && (!this.ItemId.Value.StartsWith('-') || !(item.Category.ToString() == this.ItemId.Value)))
      return flag1;
    int num = this.Number.Value - numberAdded;
    bool flag2 = num <= 0;
    if (!probe)
    {
      this.Number.Value = num;
      if (flag2)
        this.questComplete();
    }
    return true;
  }
}
