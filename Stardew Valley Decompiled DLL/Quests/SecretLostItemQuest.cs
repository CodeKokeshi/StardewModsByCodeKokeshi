// Decompiled with JetBrains decompiler
// Type: StardewValley.Quests.SecretLostItemQuest
// Assembly: Stardew Valley, Version=1.6.15.24356, Culture=neutral, PublicKeyToken=null
// MVID: AA1F513A-94F0-4EF6-A35F-2D5B4A3721BF
// Assembly location: D:\SteamLibrary\steamapps\common\Stardew Valley\Stardew Valley.dll
// XML documentation location: D:\SteamLibrary\steamapps\common\Stardew Valley\Stardew Valley.xml

using Netcode;
using System.Xml.Serialization;

#nullable disable
namespace StardewValley.Quests;

public class SecretLostItemQuest : Quest
{
  /// <summary>The internal name for the NPC who gave the quest.</summary>
  [XmlElement("npcName")]
  public readonly NetString npcName = new NetString();
  /// <summary>The friendship point reward for completing the quest.</summary>
  [XmlElement("friendshipReward")]
  public readonly NetInt friendshipReward = new NetInt();
  /// <summary>If set, the ID for another quest to remove when this quest is completed.</summary>
  [XmlElement("exclusiveQuestId")]
  public readonly NetString exclusiveQuestId = new NetString();
  /// <summary>The qualified item ID that must be collected.</summary>
  [XmlElement("itemIndex")]
  public readonly NetString ItemId = new NetString();
  /// <summary>Whether the player has found the lost item.</summary>
  [XmlElement("itemFound")]
  public readonly NetBool itemFound = new NetBool();

  /// <summary>Construct an instance.</summary>
  public SecretLostItemQuest()
  {
  }

  /// <summary>Construct an instance.</summary>
  /// <param name="npcName">The internal name for the NPC who gave the quest.</param>
  /// <param name="itemId">The qualified or unqualified item ID that must be collected.</param>
  /// <param name="friendshipReward">The friendship point reward for completing the quest.</param>
  /// <param name="exclusiveQuestId">If set, the ID for another quest to remove when this quest is completed.</param>
  public SecretLostItemQuest(
    string npcName,
    string itemId,
    int friendshipReward,
    string exclusiveQuestId)
  {
    this.npcName.Value = npcName;
    this.ItemId.Value = ItemRegistry.QualifyItemId(itemId) ?? itemId;
    this.friendshipReward.Value = friendshipReward;
    this.exclusiveQuestId.Value = exclusiveQuestId;
    this.questType.Value = 9;
  }

  /// <inheritdoc />
  protected override void initNetFields()
  {
    base.initNetFields();
    this.NetFields.AddField((INetSerializable) this.npcName, "npcName").AddField((INetSerializable) this.friendshipReward, "friendshipReward").AddField((INetSerializable) this.exclusiveQuestId, "exclusiveQuestId").AddField((INetSerializable) this.ItemId, "ItemId").AddField((INetSerializable) this.itemFound, "itemFound");
  }

  public override bool isSecretQuest() => true;

  /// <inheritdoc />
  public override bool OnItemReceived(Item item, int numberAdded, bool probe = false)
  {
    bool flag = base.OnItemReceived(item, numberAdded, probe);
    if (this.completed.Value || this.itemFound.Value || !(item?.QualifiedItemId == this.ItemId.Value))
      return flag;
    if (!probe)
    {
      this.itemFound.Value = true;
      Game1.playSound("jingle1");
    }
    return true;
  }

  /// <inheritdoc />
  public override bool OnNpcSocialized(NPC npc, bool probe = false)
  {
    bool flag = base.OnNpcSocialized(npc, probe);
    if (this.completed.Value || !this.itemFound.Value || !npc.IsVillager || !(npc.Name == this.npcName.Value) || !Game1.player.Items.ContainsId(this.ItemId.Value))
      return flag;
    if (!probe)
    {
      this.questComplete();
      string[] rawQuestFields = Quest.GetRawQuestFields(this.id.Value);
      Dialogue dialogue = new Dialogue(npc, (string) null, ArgUtility.Get(rawQuestFields, 9, "Data\\ExtraDialogue:LostItemQuest_DefaultThankYou", false));
      npc.setNewDialogue(dialogue);
      Game1.drawDialogue(npc);
      Game1.player.changeFriendship(this.friendshipReward.Value, npc);
      Game1.player.removeFirstOfThisItemFromInventory(this.ItemId.Value);
    }
    return true;
  }

  public override void questComplete()
  {
    if (this.completed.Value)
      return;
    this.completed.Value = true;
    Game1.player.questLog.Remove((Quest) this);
    foreach (Quest quest in (NetList<Quest, NetRef<Quest>>) Game1.player.questLog)
    {
      if (quest != null && quest.id.Value == this.exclusiveQuestId.Value)
        quest.destroy.Value = true;
    }
    Game1.playSound("questcomplete");
  }
}
