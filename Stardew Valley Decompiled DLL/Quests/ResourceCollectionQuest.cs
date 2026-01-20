// Decompiled with JetBrains decompiler
// Type: StardewValley.Quests.ResourceCollectionQuest
// Assembly: Stardew Valley, Version=1.6.15.24356, Culture=neutral, PublicKeyToken=null
// MVID: AA1F513A-94F0-4EF6-A35F-2D5B4A3721BF
// Assembly location: D:\SteamLibrary\steamapps\common\Stardew Valley\Stardew Valley.dll
// XML documentation location: D:\SteamLibrary\steamapps\common\Stardew Valley\Stardew Valley.xml

using Netcode;
using StardewValley.Extensions;
using System;
using System.Xml.Serialization;

#nullable disable
namespace StardewValley.Quests;

public class ResourceCollectionQuest : Quest
{
  /// <summary>The internal name for the NPC who gave the quest.</summary>
  [XmlElement("target")]
  public readonly NetString target = new NetString();
  /// <summary>The translated NPC dialogue shown when the quest is completed.</summary>
  [XmlElement("targetMessage")]
  public readonly NetString targetMessage = new NetString();
  /// <summary>The number of items collected so far.</summary>
  [XmlElement("numberCollected")]
  public readonly NetInt numberCollected = new NetInt();
  /// <summary>The number of items which must be collected.</summary>
  [XmlElement("number")]
  public readonly NetInt number = new NetInt();
  /// <summary>The gold reward for finishing the quest.</summary>
  [XmlElement("reward")]
  public readonly NetInt reward = new NetInt();
  /// <summary>The qualified item ID that must be collected.</summary>
  [XmlElement("resource")]
  public readonly NetString ItemId = new NetString();
  /// <summary>The translatable text segments for the quest description shown in the quest log.</summary>
  public readonly NetDescriptionElementList parts = new NetDescriptionElementList();
  /// <summary>The translatable text segments for the <see cref="F:StardewValley.Quests.ResourceCollectionQuest.targetMessage" />.</summary>
  public readonly NetDescriptionElementList dialogueparts = new NetDescriptionElementList();
  /// <summary>The translatable text segments for the objective shown in the quest log (like "0/5 caught").</summary>
  [XmlElement("objective")]
  public readonly NetDescriptionElementRef objective = new NetDescriptionElementRef();

  /// <summary>Construct an instance.</summary>
  public ResourceCollectionQuest() => this.questType.Value = 10;

  /// <inheritdoc />
  protected override void initNetFields()
  {
    base.initNetFields();
    this.NetFields.AddField((INetSerializable) this.parts, "parts").AddField((INetSerializable) this.dialogueparts, "dialogueparts").AddField((INetSerializable) this.objective, "objective").AddField((INetSerializable) this.target, "target").AddField((INetSerializable) this.targetMessage, "targetMessage").AddField((INetSerializable) this.numberCollected, "numberCollected").AddField((INetSerializable) this.number, "number").AddField((INetSerializable) this.reward, "reward").AddField((INetSerializable) this.ItemId, "ItemId");
  }

  public void loadQuestInfo()
  {
    if (this.target.Value != null || Game1.gameMode == (byte) 6)
      return;
    Random initializationRandom = this.CreateInitializationRandom();
    this.questTitle = Game1.content.LoadString("Strings\\StringsFromCSFiles:ResourceCollectionQuest.cs.13640");
    int num = initializationRandom.Next(6) * 2;
    for (int index = 0; index < initializationRandom.Next(1, 100); ++index)
      initializationRandom.Next();
    int val1_1 = 0;
    int val1_2 = 0;
    foreach (Farmer allFarmer in Game1.getAllFarmers())
      val1_1 = Math.Max(val1_1, allFarmer.MiningLevel);
    foreach (Farmer allFarmer in Game1.getAllFarmers())
      val1_2 = Math.Max(val1_2, allFarmer.ForagingLevel);
    switch (num)
    {
      case 0:
        this.ItemId.Value = "(O)378";
        this.number.Value = 20 + val1_1 * 2 + initializationRandom.Next(-2, 4) * 2;
        this.reward.Value = this.number.Value * 10;
        this.number.Value -= this.number.Value % 5;
        this.target.Value = "Clint";
        break;
      case 2:
        this.ItemId.Value = "(O)380";
        this.number.Value = 15 + val1_1 + initializationRandom.Next(-1, 3) * 2;
        this.reward.Value = this.number.Value * 15;
        this.number.Value = (int) ((double) this.number.Value * 0.75);
        this.number.Value -= this.number.Value % 5;
        this.target.Value = "Clint";
        break;
      case 4:
        this.ItemId.Value = "(O)382";
        this.number.Value = 10 + val1_1 + initializationRandom.Next(-1, 3) * 2;
        this.reward.Value = this.number.Value * 25;
        this.number.Value = (int) ((double) this.number.Value * 0.75);
        this.number.Value -= this.number.Value % 5;
        this.target.Value = "Clint";
        break;
      case 6:
        this.ItemId.Value = Utility.GetAllPlayerDeepestMineLevel() > 40 ? "(O)384" : "(O)378";
        this.number.Value = 8 + val1_1 / 2 + initializationRandom.Next(-1, 1) * 2;
        this.reward.Value = this.number.Value * 30;
        this.number.Value = (int) ((double) this.number.Value * 0.75);
        this.number.Value -= this.number.Value % 2;
        this.target.Value = "Clint";
        break;
      case 8:
        this.ItemId.Value = "(O)388";
        this.number.Value = 25 + val1_2 + initializationRandom.Next(-3, 3) * 2;
        this.number.Value -= this.number.Value % 5;
        this.reward.Value = this.number.Value * 8;
        this.target.Value = "Robin";
        break;
      default:
        this.ItemId.Value = "(O)390";
        this.number.Value = 25 + val1_1 + initializationRandom.Next(-3, 3) * 2;
        this.number.Value -= this.number.Value % 5;
        this.reward.Value = this.number.Value * 8;
        this.target.Value = "Robin";
        break;
    }
    if (this.target.Value == null)
      return;
    Item obj = ItemRegistry.Create(this.ItemId.Value);
    if (this.ItemId.Value != "(O)388" && this.ItemId.Value != "(O)390")
    {
      this.parts.Clear();
      int index = initializationRandom.Next(4);
      this.parts.Add(new DescriptionElement("Strings\\StringsFromCSFiles:ResourceCollectionQuest.cs.13647", new object[3]
      {
        (object) this.number.Value,
        (object) obj,
        (object) new DescriptionElement("Strings\\StringsFromCSFiles:ResourceCollectionQuest.cs." + new string[4]
        {
          "13649",
          "13650",
          "13651",
          "13652"
        }[index], Array.Empty<object>())
      }));
      if (index == 3)
      {
        this.dialogueparts.Clear();
        this.dialogueparts.Add("Strings\\StringsFromCSFiles:ResourceCollectionQuest.cs.13655");
        this.dialogueparts.Add("Strings\\StringsFromCSFiles:ResourceCollectionQuest.cs." + initializationRandom.Choose<string>("13656", "13657", "13658"));
        this.dialogueparts.Add("Strings\\StringsFromCSFiles:ResourceCollectionQuest.cs.13659");
      }
      else
      {
        this.dialogueparts.Clear();
        this.dialogueparts.Add("Strings\\StringsFromCSFiles:ResourceCollectionQuest.cs.13662");
        this.dialogueparts.Add("Strings\\StringsFromCSFiles:ResourceCollectionQuest.cs." + initializationRandom.Choose<string>("13656", "13657", "13658"));
        NetDescriptionElementList dialogueparts = this.dialogueparts;
        DescriptionElement descriptionElement;
        if (!initializationRandom.NextBool())
          descriptionElement = new DescriptionElement("Strings\\StringsFromCSFiles:ResourceCollectionQuest.cs.13672", Array.Empty<object>());
        else
          descriptionElement = new DescriptionElement("Strings\\StringsFromCSFiles:ResourceCollectionQuest.cs.13667", new object[1]
          {
            (object) new DescriptionElement("Strings\\StringsFromCSFiles:ResourceCollectionQuest.cs." + initializationRandom.Choose<string>("13668", "13669", "13670"), Array.Empty<object>())
          });
        dialogueparts.Add(descriptionElement);
        this.dialogueparts.Add("Strings\\StringsFromCSFiles:ResourceCollectionQuest.cs.13673");
      }
    }
    else
    {
      this.parts.Clear();
      this.parts.Add(new DescriptionElement("Strings\\StringsFromCSFiles:ResourceCollectionQuest.cs.13674", new object[2]
      {
        (object) this.number.Value,
        (object) obj
      }));
      this.dialogueparts.Clear();
      this.dialogueparts.Add(new DescriptionElement("Strings\\StringsFromCSFiles:ResourceCollectionQuest.cs.13677", new object[1]
      {
        this.ItemId.Value == "(O)388" ? (object) new DescriptionElement("Strings\\StringsFromCSFiles:ResourceCollectionQuest.cs.13678", Array.Empty<object>()) : (object) new DescriptionElement("Strings\\StringsFromCSFiles:ResourceCollectionQuest.cs.13679", Array.Empty<object>())
      }));
      this.dialogueparts.Add("Strings\\StringsFromCSFiles:ResourceCollectionQuest.cs." + initializationRandom.Choose<string>("13681", "13682", "13683"));
    }
    this.parts.Add(new DescriptionElement("Strings\\StringsFromCSFiles:ItemDeliveryQuest.cs.13607", new object[1]
    {
      (object) this.reward.Value
    }));
    this.parts.Add(this.target.Value.Equals("Clint") ? "Strings\\StringsFromCSFiles:ResourceCollectionQuest.cs.13688" : "");
    this.objective.Value = new DescriptionElement("Strings\\StringsFromCSFiles:ResourceCollectionQuest.cs.13691", new object[3]
    {
      (object) "0",
      (object) this.number.Value,
      (object) obj
    });
  }

  public override void reloadDescription()
  {
    if (this._questDescription == "")
      this.loadQuestInfo();
    if (this.parts.Count == 0 || this.parts == null || this.dialogueparts.Count == 0 || this.dialogueparts == null)
      return;
    string str1 = "";
    string str2 = "";
    foreach (DescriptionElement part in (NetList<DescriptionElement, NetDescriptionElementRef>) this.parts)
      str1 += part.loadDescriptionElement();
    foreach (DescriptionElement dialoguepart in (NetList<DescriptionElement, NetDescriptionElementRef>) this.dialogueparts)
      str2 += dialoguepart.loadDescriptionElement();
    this.questDescription = str1;
    this.targetMessage.Value = str2;
  }

  public override void reloadObjective()
  {
    if (this.numberCollected.Value < this.number.Value)
      this.objective.Value = new DescriptionElement("Strings\\StringsFromCSFiles:ResourceCollectionQuest.cs.13691", new object[3]
      {
        (object) this.numberCollected.Value,
        (object) this.number.Value,
        (object) ItemRegistry.Create(this.ItemId.Value)
      });
    if (this.objective.Value == null)
      return;
    this.currentObjective = this.objective.Value.loadDescriptionElement();
  }

  /// <inheritdoc />
  public override bool OnItemReceived(Item item, int numberAdded, bool probe = false)
  {
    bool flag = base.OnItemReceived(item, numberAdded, probe);
    if (this.completed.Value || !(item?.QualifiedItemId == this.ItemId.Value) || numberAdded == -1 || this.numberCollected.Value >= this.number.Value)
      return flag;
    if (!probe)
    {
      this.numberCollected.Value = Math.Min(this.number.Value, this.numberCollected.Value + numberAdded);
      Game1.dayTimeMoneyBox.pingQuest((Quest) this);
      if (this.numberCollected.Value >= this.number.Value)
      {
        this.objective.Value = new DescriptionElement("Strings\\StringsFromCSFiles:FishingQuest.cs.13277", new object[1]
        {
          (object) Game1.getCharacterFromName(this.target.Value)
        });
        Game1.playSound("jingle1");
      }
    }
    return true;
  }

  /// <inheritdoc />
  public override bool OnNpcSocialized(NPC npc, bool probe = false)
  {
    bool flag = base.OnNpcSocialized(npc, probe);
    if (this.completed.Value || !npc.IsVillager || !(npc.Name == this.target.Value) || this.numberCollected.Value < this.number.Value)
      return flag;
    if (!probe)
    {
      npc.CurrentDialogue.Push(new Dialogue(npc, (string) null, this.targetMessage.Value));
      this.moneyReward.Value = this.reward.Value;
      this.questComplete();
      Game1.drawDialogue(npc);
    }
    return true;
  }
}
