// Decompiled with JetBrains decompiler
// Type: StardewValley.Quests.FishingQuest
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

public class FishingQuest : Quest
{
  /// <summary>The internal name for the NPC who gave the quest.</summary>
  [XmlElement("target")]
  public readonly NetString target = new NetString();
  /// <summary>The translated text for the NPC dialogue shown when the quest is completed.</summary>
  public string targetMessage;
  /// <summary>The number of fish which must be caught.</summary>
  [XmlElement("numberToFish")]
  public readonly NetInt numberToFish = new NetInt();
  /// <summary>The gold reward for finishing the quest.</summary>
  [XmlElement("reward")]
  public readonly NetInt reward = new NetInt();
  /// <summary>The number of fish caught so far.</summary>
  [XmlElement("numberFished")]
  public readonly NetInt numberFished = new NetInt();
  /// <summary>The qualified item ID for the fish to catch.</summary>
  [XmlElement("whichFish")]
  public readonly NetString ItemId = new NetString();
  /// <summary>The translatable text segments for the quest description in the quest log.</summary>
  public readonly NetDescriptionElementList parts = new NetDescriptionElementList();
  /// <summary>The translatable text segments for the NPC dialogue shown when the quest is completed.</summary>
  public readonly NetDescriptionElementList dialogueparts = new NetDescriptionElementList();
  /// <summary>The translatable text segments for the objective shown in the quest log (like "0/5 caught").</summary>
  [XmlElement("objective")]
  public readonly NetDescriptionElementRef objective = new NetDescriptionElementRef();

  /// <summary>Construct an instance.</summary>
  public FishingQuest() => this.questType.Value = 7;

  /// <summary>Construct an instance.</summary>
  /// <param name="itemId">The qualified item ID for the fish to catch.</param>
  /// <param name="numberToFish">The number of fish which must be caught.</param>
  /// <param name="target">The internal name for the NPC who gave the quest.</param>
  /// <param name="returnDialogue">The translated text for the NPC dialogue shown when the quest is completed.</param>
  public FishingQuest(
    string itemId,
    int numberToFish,
    string target,
    string questTitle,
    string questDescription,
    string returnDialogue)
    : this()
  {
    this.ItemId.Value = ItemRegistry.QualifyItemId(itemId);
    this.numberToFish.Value = numberToFish;
    this.target.Value = target;
    this.questDescription = questDescription;
    this.questTitle = questTitle;
    this._loadedTitle = true;
    this.targetMessage = returnDialogue;
  }

  /// <inheritdoc />
  protected override void initNetFields()
  {
    base.initNetFields();
    this.NetFields.AddField((INetSerializable) this.parts, "parts").AddField((INetSerializable) this.dialogueparts, "dialogueparts").AddField((INetSerializable) this.objective, "objective").AddField((INetSerializable) this.target, "target").AddField((INetSerializable) this.numberToFish, "numberToFish").AddField((INetSerializable) this.reward, "reward").AddField((INetSerializable) this.numberFished, "numberFished").AddField((INetSerializable) this.ItemId, "ItemId");
  }

  public void loadQuestInfo()
  {
    if (this.target.Value != null && this.ItemId.Value != null)
      return;
    Random initializationRandom = this.CreateInitializationRandom();
    this.questTitle = Game1.content.LoadString("Strings\\StringsFromCSFiles:FishingQuest.cs.13227");
    if (initializationRandom.NextBool())
    {
      switch (Game1.season)
      {
        case Season.Spring:
          this.ItemId.Value = initializationRandom.Choose<string>("(O)129", "(O)131", "(O)136", "(O)137", "(O)142", "(O)143", "(O)145", "(O)147");
          break;
        case Season.Summer:
          this.ItemId.Value = initializationRandom.Choose<string>("(O)130", "(O)136", "(O)138", "(O)142", "(O)144", "(O)145", "(O)146", "(O)149", "(O)150");
          break;
        case Season.Fall:
          this.ItemId.Value = initializationRandom.Choose<string>("(O)129", "(O)131", "(O)136", "(O)137", "(O)139", "(O)142", "(O)143", "(O)150");
          break;
        case Season.Winter:
          this.ItemId.Value = initializationRandom.Choose<string>("(O)130", "(O)131", "(O)136", "(O)141", "(O)144", "(O)146", "(O)147", "(O)150", "(O)151");
          break;
      }
      Item obj = ItemRegistry.Create(this.ItemId.Value);
      bool flag = this.ItemId.Value == "(O)149";
      this.numberToFish.Value = (int) Math.Ceiling(90.0 / (double) Math.Max(1, this.GetGoldRewardPerItem(obj))) + Game1.player.FishingLevel / 5;
      this.reward.Value = this.numberToFish.Value * this.GetGoldRewardPerItem(obj);
      this.target.Value = "Demetrius";
      this.parts.Clear();
      this.parts.Add(new DescriptionElement("Strings\\StringsFromCSFiles:FishingQuest.cs.13228", new object[2]
      {
        (object) obj,
        (object) this.numberToFish.Value
      }));
      this.dialogueparts.Clear();
      this.dialogueparts.Add(new DescriptionElement("Strings\\StringsFromCSFiles:FishingQuest.cs.13231", new object[2]
      {
        (object) obj,
        (object) initializationRandom.Choose<DescriptionElement>(new DescriptionElement("Strings\\StringsFromCSFiles:FishingQuest.cs.13233", Array.Empty<object>()), new DescriptionElement("Strings\\StringsFromCSFiles:FishingQuest.cs.13234", Array.Empty<object>()), new DescriptionElement("Strings\\StringsFromCSFiles:FishingQuest.cs.13235", Array.Empty<object>()), new DescriptionElement("Strings\\StringsFromCSFiles:FishingQuest.cs.13236", new object[1]
        {
          (object) obj
        }))
      }));
      NetDescriptionElementRef objective = this.objective;
      DescriptionElement descriptionElement;
      if (!flag)
        descriptionElement = new DescriptionElement("Strings\\StringsFromCSFiles:FishingQuest.cs.13244", new object[3]
        {
          (object) 0,
          (object) this.numberToFish.Value,
          (object) obj
        });
      else
        descriptionElement = new DescriptionElement("Strings\\StringsFromCSFiles:FishingQuest.cs.13243", new object[2]
        {
          (object) 0,
          (object) this.numberToFish.Value
        });
      objective.Value = descriptionElement;
    }
    else
    {
      switch (Game1.season)
      {
        case Season.Spring:
          this.ItemId.Value = initializationRandom.Choose<string>("(O)129", "(O)131", "(O)136", "(O)137", "(O)142", "(O)143", "(O)145", "(O)147", "(O)702");
          break;
        case Season.Summer:
          this.ItemId.Value = initializationRandom.Choose<string>("(O)128", "(O)130", "(O)136", "(O)138", "(O)142", "(O)144", "(O)145", "(O)146", "(O)149", "(O)150", "(O)702");
          break;
        case Season.Fall:
          this.ItemId.Value = initializationRandom.Choose<string>("(O)129", "(O)131", "(O)136", "(O)137", "(O)139", "(O)142", "(O)143", "(O)150", "(O)699", "(O)702", "(O)705");
          break;
        case Season.Winter:
          this.ItemId.Value = initializationRandom.Choose<string>("(O)130", "(O)131", "(O)136", "(O)141", "(O)143", "(O)144", "(O)146", "(O)147", "(O)151", "(O)699", "(O)702", "(O)705");
          break;
      }
      this.target.Value = "Willy";
      Item obj = ItemRegistry.Create(this.ItemId.Value);
      bool flag = this.ItemId.Value == "(O)151";
      this.numberToFish.Value = (int) Math.Ceiling(90.0 / (double) Math.Max(1, this.GetGoldRewardPerItem(obj))) + Game1.player.FishingLevel / 5;
      this.reward.Value = this.numberToFish.Value * this.GetGoldRewardPerItem(obj);
      this.parts.Clear();
      NetDescriptionElementList parts = this.parts;
      DescriptionElement descriptionElement1;
      if (!flag)
        descriptionElement1 = new DescriptionElement("Strings\\StringsFromCSFiles:FishingQuest.cs.13248", new object[3]
        {
          (object) this.reward.Value,
          (object) this.numberToFish.Value,
          (object) obj
        });
      else
        descriptionElement1 = new DescriptionElement("Strings\\StringsFromCSFiles:FishingQuest.cs.13248", new object[3]
        {
          (object) this.reward.Value,
          (object) this.numberToFish.Value,
          (object) new DescriptionElement("Strings\\StringsFromCSFiles:FishingQuest.cs.13253", Array.Empty<object>())
        });
      parts.Add(descriptionElement1);
      this.dialogueparts.Clear();
      this.dialogueparts.Add(new DescriptionElement("Strings\\StringsFromCSFiles:FishingQuest.cs.13256", new object[1]
      {
        (object) obj
      }));
      this.dialogueparts.Add(initializationRandom.Choose<DescriptionElement>(new DescriptionElement("Strings\\StringsFromCSFiles:FishingQuest.cs.13258", Array.Empty<object>()), new DescriptionElement("Strings\\StringsFromCSFiles:FishingQuest.cs.13259", Array.Empty<object>()), new DescriptionElement("Strings\\StringsFromCSFiles:FishingQuest.cs.13260", new object[1]
      {
        (object) new DescriptionElement("Strings\\StringsFromCSFiles:FishingQuest.cs." + initializationRandom.Choose<string>("13261", "13262", "13263", "13264", "13265", "13266"), Array.Empty<object>())
      }), new DescriptionElement("Strings\\StringsFromCSFiles:FishingQuest.cs.13267", Array.Empty<object>())));
      this.dialogueparts.Add(new DescriptionElement("Strings\\StringsFromCSFiles:FishingQuest.cs.13268", Array.Empty<object>()));
      NetDescriptionElementRef objective = this.objective;
      DescriptionElement descriptionElement2;
      if (!flag)
        descriptionElement2 = new DescriptionElement("Strings\\StringsFromCSFiles:FishingQuest.cs.13244", new object[3]
        {
          (object) 0,
          (object) this.numberToFish.Value,
          (object) obj
        });
      else
        descriptionElement2 = new DescriptionElement("Strings\\StringsFromCSFiles:FishingQuest.cs.13255", new object[2]
        {
          (object) 0,
          (object) this.numberToFish.Value
        });
      objective.Value = descriptionElement2;
    }
    this.parts.Add(new DescriptionElement("Strings\\StringsFromCSFiles:FishingQuest.cs.13274", new object[1]
    {
      (object) this.reward.Value
    }));
    this.parts.Add("Strings\\StringsFromCSFiles:FishingQuest.cs.13275");
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
    this.targetMessage = str2;
  }

  public override void reloadObjective()
  {
    bool flag1 = this.ItemId.Value == "(O)149";
    bool flag2 = this.ItemId.Value == "(O)151";
    if (this.numberFished.Value < this.numberToFish.Value)
    {
      NetDescriptionElementRef objective = this.objective;
      DescriptionElement descriptionElement;
      if (!flag1)
      {
        if (!flag2)
          descriptionElement = new DescriptionElement("Strings\\StringsFromCSFiles:FishingQuest.cs.13244", new object[3]
          {
            (object) this.numberFished.Value,
            (object) this.numberToFish.Value,
            (object) ItemRegistry.Create(this.ItemId.Value)
          });
        else
          descriptionElement = new DescriptionElement("Strings\\StringsFromCSFiles:FishingQuest.cs.13255", new object[2]
          {
            (object) this.numberFished.Value,
            (object) this.numberToFish.Value
          });
      }
      else
        descriptionElement = new DescriptionElement("Strings\\StringsFromCSFiles:FishingQuest.cs.13243", new object[2]
        {
          (object) this.numberFished.Value,
          (object) this.numberToFish.Value
        });
      objective.Value = descriptionElement;
    }
    if (this.objective.Value == null)
      return;
    this.currentObjective = this.objective.Value.loadDescriptionElement();
  }

  /// <inheritdoc />
  public override bool OnFishCaught(string fishId, int numberCaught, int size, bool probe = false)
  {
    bool flag = base.OnFishCaught(fishId, numberCaught, size, probe);
    this.loadQuestInfo();
    if (!(fishId == this.ItemId.Value) || this.numberFished.Value >= this.numberToFish.Value)
      return flag;
    if (!probe)
    {
      this.numberFished.Value = Math.Min(this.numberToFish.Value, this.numberFished.Value + numberCaught);
      Game1.dayTimeMoneyBox.pingQuest((Quest) this);
      if (this.numberFished.Value >= this.numberToFish.Value)
      {
        if (this.target.Value == null)
          this.target.Value = "Willy";
        this.objective.Value = new DescriptionElement("Strings\\Quests:ObjectiveReturnToNPC", new object[1]
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
    if (this.numberFished.Value < this.numberToFish.Value || this.target.Value == null || !(npc.Name == this.target.Value) || !npc.IsVillager || this.completed.Value)
      return flag;
    if (!probe)
    {
      npc.CurrentDialogue.Push(new Dialogue(npc, (string) null, this.targetMessage));
      this.moneyReward.Value = this.reward.Value;
      this.questComplete();
      Game1.drawDialogue(npc);
    }
    return true;
  }

  /// <summary>Get the gold reward for a given item.</summary>
  /// <param name="item">The item instance.</param>
  private int GetGoldRewardPerItem(Item item)
  {
    return item is StardewValley.Object @object ? @object.Price : (int) ((double) item.salePrice(false) * 1.5);
  }
}
