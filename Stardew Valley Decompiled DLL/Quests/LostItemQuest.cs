// Decompiled with JetBrains decompiler
// Type: StardewValley.Quests.LostItemQuest
// Assembly: Stardew Valley, Version=1.6.15.24356, Culture=neutral, PublicKeyToken=null
// MVID: AA1F513A-94F0-4EF6-A35F-2D5B4A3721BF
// Assembly location: D:\SteamLibrary\steamapps\common\Stardew Valley\Stardew Valley.dll
// XML documentation location: D:\SteamLibrary\steamapps\common\Stardew Valley\Stardew Valley.xml

using Microsoft.Xna.Framework;
using Netcode;
using StardewValley.Extensions;
using System;
using System.Xml.Serialization;

#nullable disable
namespace StardewValley.Quests;

public class LostItemQuest : Quest
{
  /// <summary>The internal name for the NPC who gave the quest.</summary>
  [XmlElement("npcName")]
  public readonly NetString npcName = new NetString();
  /// <summary>The internal name for the location where the item can be found.</summary>
  [XmlElement("locationOfItem")]
  public readonly NetString locationOfItem = new NetString();
  /// <summary>The qualified item ID for the item to find.</summary>
  [XmlElement("itemIndex")]
  public readonly NetString ItemId = new NetString();
  /// <summary>The X tile position within the location where the item can be found.</summary>
  [XmlElement("tileX")]
  public readonly NetInt tileX = new NetInt();
  /// <summary>The Y tile position within the location where the item can be found.</summary>
  [XmlElement("tileY")]
  public readonly NetInt tileY = new NetInt();
  /// <summary>Whether the player has found the item yet.</summary>
  [XmlElement("itemFound")]
  public readonly NetBool itemFound = new NetBool();
  /// <summary>The translatable text segments for the objective shown in the quest log.</summary>
  [XmlElement("objective")]
  public readonly NetDescriptionElementRef objective = new NetDescriptionElementRef();

  /// <summary>Construct an instance.</summary>
  public LostItemQuest()
  {
  }

  /// <summary>Construct an instance.</summary>
  /// <param name="npcName">The internal name for the NPC who gave the quest.</param>
  /// <param name="locationOfItem">The internal name for the location where the item can be found.</param>
  /// <param name="itemId">The qualified or unqualified item ID for the item to find.</param>
  /// <param name="tileX">The X tile position within the location where the item can be found.</param>
  /// <param name="tileY">The Y tile position within the location where the item can be found.</param>
  /// <exception cref="T:System.InvalidOperationException">The <paramref name="itemId" /> matches a non-object-type item, which can't be placed in the world.</exception>
  public LostItemQuest(
    string npcName,
    string locationOfItem,
    string itemId,
    int tileX,
    int tileY)
  {
    this.npcName.Value = npcName;
    this.locationOfItem.Value = locationOfItem;
    this.ItemId.Value = ItemRegistry.QualifyItemId(itemId) ?? itemId;
    this.tileX.Value = tileX;
    this.tileY.Value = tileY;
    this.questType.Value = 9;
    if (!ItemRegistry.GetDataOrErrorItem(this.ItemId.Value).HasTypeObject())
      throw new InvalidOperationException($"Can't create {this.GetType().Name} #{this.id.Value} because the lost item ({this.ItemId.Value}) isn't an object-type item.");
  }

  /// <inheritdoc />
  protected override void initNetFields()
  {
    base.initNetFields();
    this.NetFields.AddField((INetSerializable) this.objective, "objective").AddField((INetSerializable) this.npcName, "npcName").AddField((INetSerializable) this.locationOfItem, "locationOfItem").AddField((INetSerializable) this.ItemId, "ItemId").AddField((INetSerializable) this.tileX, "tileX").AddField((INetSerializable) this.tileY, "tileY").AddField((INetSerializable) this.itemFound, "itemFound");
  }

  /// <inheritdoc />
  public override bool OnWarped(GameLocation location, bool probe = false)
  {
    bool flag = base.OnWarped(location, probe);
    if (this.itemFound.Value || !location.name.Equals((object) this.locationOfItem.Value))
      return flag;
    Vector2 key = new Vector2((float) this.tileX.Value, (float) this.tileY.Value);
    location.overlayObjects.Remove(key);
    StardewValley.Object @object = ItemRegistry.Create<StardewValley.Object>(this.ItemId.Value);
    @object.TileLocation = key;
    @object.questItem.Value = true;
    @object.questId.Value = this.id.Value;
    @object.IsSpawnedObject = true;
    location.overlayObjects.Add(key, @object);
    return true;
  }

  public new void reloadObjective()
  {
    if (this.objective.Value == null)
      return;
    this.currentObjective = this.objective.Value.loadDescriptionElement();
  }

  /// <inheritdoc />
  public override bool OnItemReceived(Item item, int numberAdded, bool probe = false)
  {
    bool flag = base.OnItemReceived(item, numberAdded, probe);
    if (this.completed.Value || this.itemFound.Value || item == null || !(item.QualifiedItemId == this.ItemId.Value))
      return flag;
    if (!probe)
    {
      this.itemFound.Value = true;
      string displayName = this.npcName.Value;
      NPC characterFromName = Game1.getCharacterFromName(this.npcName.Value);
      if (characterFromName != null)
        displayName = characterFromName.displayName;
      Game1.player.completelyStopAnimatingOrDoingAction();
      Game1.drawObjectDialogue(Game1.content.LoadString("Strings\\Quests:MessageFoundLostItem", (object) item.DisplayName, (object) displayName));
      this.objective.Value = new DescriptionElement("Strings\\Quests:ObjectiveReturnToNPC", new object[1]
      {
        (object) characterFromName
      });
      Game1.playSound("jingle1");
    }
    return true;
  }

  public override bool OnNpcSocialized(NPC npc, bool probe = false)
  {
    bool flag = base.OnNpcSocialized(npc, probe);
    if (this.completed.Value || !this.itemFound.Value || !(npc.Name == this.npcName.Value) || !npc.IsVillager || !Game1.player.Items.ContainsId(this.ItemId.Value))
      return flag;
    if (!probe)
    {
      this.questComplete();
      string[] rawQuestFields = Quest.GetRawQuestFields(this.id.Value);
      Dialogue dialogue = new Dialogue(npc, (string) null, ArgUtility.Get(rawQuestFields, 9, "Data\\ExtraDialogue:LostItemQuest_DefaultThankYou", false));
      npc.setNewDialogue(dialogue);
      Game1.drawDialogue(npc);
      Game1.player.changeFriendship(250, npc);
      Game1.player.removeFirstOfThisItemFromInventory(this.ItemId.Value);
    }
    return true;
  }
}
