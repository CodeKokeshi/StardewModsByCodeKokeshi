// Decompiled with JetBrains decompiler
// Type: StardewValley.Quests.CraftingQuest
// Assembly: Stardew Valley, Version=1.6.15.24356, Culture=neutral, PublicKeyToken=null
// MVID: AA1F513A-94F0-4EF6-A35F-2D5B4A3721BF
// Assembly location: D:\SteamLibrary\steamapps\common\Stardew Valley\Stardew Valley.dll
// XML documentation location: D:\SteamLibrary\steamapps\common\Stardew Valley\Stardew Valley.xml

using Netcode;
using System.Xml.Serialization;

#nullable disable
namespace StardewValley.Quests;

public class CraftingQuest : Quest
{
  /// <summary>Obsolete. This is only kept to preserve data from old save files, and isn't synced in multiplayer. Use <see cref="F:StardewValley.Quests.CraftingQuest.ItemId" /> instead.</summary>
  [XmlElement("isBigCraftable")]
  public bool? obsolete_isBigCraftable;
  /// <summary>The qualified item ID to craft.</summary>
  [XmlElement("indexToCraft")]
  public readonly NetString ItemId = new NetString();

  /// <summary>Construct an instance.</summary>
  public CraftingQuest()
  {
  }

  /// <summary>Construct an instance.</summary>
  /// <param name="itemId">The qualified or unqualified item ID to craft.</param>
  public CraftingQuest(string itemId)
  {
    this.ItemId.Value = ItemRegistry.QualifyItemId(itemId) ?? itemId;
  }

  /// <inheritdoc />
  protected override void initNetFields()
  {
    base.initNetFields();
    this.NetFields.AddField((INetSerializable) this.ItemId, "ItemId");
  }

  /// <inheritdoc />
  public override bool OnRecipeCrafted(CraftingRecipe recipe, Item item, bool probe = false)
  {
    bool flag = base.OnRecipeCrafted(recipe, item, probe);
    if (!(item.QualifiedItemId == this.ItemId.Value))
      return flag;
    if (!probe)
      this.questComplete();
    return true;
  }
}
