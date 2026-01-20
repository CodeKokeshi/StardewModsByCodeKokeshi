// Decompiled with JetBrains decompiler
// Type: StardewValley.Quests.Quest
// Assembly: Stardew Valley, Version=1.6.15.24356, Culture=neutral, PublicKeyToken=null
// MVID: AA1F513A-94F0-4EF6-A35F-2D5B4A3721BF
// Assembly location: D:\SteamLibrary\steamapps\common\Stardew Valley\Stardew Valley.dll
// XML documentation location: D:\SteamLibrary\steamapps\common\Stardew Valley\Stardew Valley.xml

using Netcode;
using StardewValley.ItemTypeDefinitions;
using StardewValley.Mods;
using StardewValley.Monsters;
using System;
using System.Collections.Generic;
using System.Xml.Serialization;

#nullable disable
namespace StardewValley.Quests;

[XmlInclude(typeof (CraftingQuest))]
[XmlInclude(typeof (DescriptionElement))]
[XmlInclude(typeof (FishingQuest))]
[XmlInclude(typeof (GoSomewhereQuest))]
[XmlInclude(typeof (HaveBuildingQuest))]
[XmlInclude(typeof (ItemDeliveryQuest))]
[XmlInclude(typeof (ItemHarvestQuest))]
[XmlInclude(typeof (LostItemQuest))]
[XmlInclude(typeof (ResourceCollectionQuest))]
[XmlInclude(typeof (SecretLostItemQuest))]
[XmlInclude(typeof (SlayMonsterQuest))]
[XmlInclude(typeof (SocializeQuest))]
public class Quest : INetObject<NetFields>, IQuest, IHaveModData
{
  public const int type_basic = 1;
  public const int type_crafting = 2;
  public const int type_itemDelivery = 3;
  public const int type_monster = 4;
  public const int type_socialize = 5;
  public const int type_location = 6;
  public const int type_fishing = 7;
  public const int type_building = 8;
  public const int type_harvest = 9;
  public const int type_resource = 10;
  public const int type_weeding = 11;
  public string _currentObjective = "";
  public string _questDescription = "";
  public string _questTitle = "";
  [XmlElement("rewardDescription")]
  public readonly NetString rewardDescription = new NetString();
  [XmlElement("accepted")]
  public readonly NetBool accepted = new NetBool();
  [XmlElement("completed")]
  public readonly NetBool completed = new NetBool();
  [XmlElement("dailyQuest")]
  public readonly NetBool dailyQuest = new NetBool();
  [XmlElement("showNew")]
  public readonly NetBool showNew = new NetBool();
  [XmlElement("canBeCancelled")]
  public readonly NetBool canBeCancelled = new NetBool();
  [XmlElement("destroy")]
  public readonly NetBool destroy = new NetBool();
  [XmlElement("id")]
  public readonly NetString id = new NetString();
  [XmlElement("moneyReward")]
  public readonly NetInt moneyReward = new NetInt();
  [XmlElement("questType")]
  public readonly NetInt questType = new NetInt();
  [XmlElement("daysLeft")]
  public readonly NetInt daysLeft = new NetInt();
  [XmlElement("dayQuestAccepted")]
  public readonly NetInt dayQuestAccepted = new NetInt(-1);
  [XmlArrayItem("int")]
  public readonly NetStringList nextQuests = new NetStringList();
  /// <summary>Obsolete since 1.6.9. This is only kept to preserve data from old save files; use more specific fields like <see cref="F:StardewValley.Quests.HaveBuildingQuest.buildingType" /> instead.</summary>
  [XmlElement("completionString")]
  public string obsolete_completionString;
  private bool _loadedDescription;
  protected bool _loadedTitle;

  /// <inheritdoc />
  [XmlIgnore]
  public ModDataDictionary modData { get; } = new ModDataDictionary();

  /// <inheritdoc />
  [XmlElement("modData")]
  public ModDataDictionary modDataForSerialization
  {
    get => this.modData.GetForSerialization();
    set => this.modData.SetFromSerialization(value);
  }

  public NetFields NetFields { get; }

  public Quest()
  {
    this.NetFields = new NetFields(NetFields.GetNameForInstance<Quest>(this));
    this.initNetFields();
  }

  /// <summary>Register all net fields and their events.</summary>
  protected virtual void initNetFields()
  {
    this.NetFields.SetOwner((INetObject<NetFields>) this).AddField((INetSerializable) this.rewardDescription, "rewardDescription").AddField((INetSerializable) this.accepted, "accepted").AddField((INetSerializable) this.completed, "completed").AddField((INetSerializable) this.dailyQuest, "dailyQuest").AddField((INetSerializable) this.showNew, "showNew").AddField((INetSerializable) this.canBeCancelled, "canBeCancelled").AddField((INetSerializable) this.destroy, "destroy").AddField((INetSerializable) this.id, "id").AddField((INetSerializable) this.moneyReward, "moneyReward").AddField((INetSerializable) this.questType, "questType").AddField((INetSerializable) this.daysLeft, "daysLeft").AddField((INetSerializable) this.nextQuests, "nextQuests").AddField((INetSerializable) this.dayQuestAccepted, "dayQuestAccepted").AddField((INetSerializable) this.modData, "modData");
  }

  public string questTitle
  {
    get
    {
      if (!this._loadedTitle)
      {
        switch (this.questType.Value)
        {
          case 3:
            this._questTitle = !(this is ItemDeliveryQuest itemDeliveryQuest) || itemDeliveryQuest.target.Value == null ? Game1.content.LoadString("Strings\\StringsFromCSFiles:ItemDeliveryQuest.cs.13285") : Game1.content.LoadString("Strings\\1_6_Strings:ItemDeliveryQuestTitle", (object) NPC.GetDisplayName(itemDeliveryQuest.target.Value));
            break;
          case 4:
            this._questTitle = !(this is SlayMonsterQuest slayMonsterQuest) || slayMonsterQuest.monsterName.Value == null ? Game1.content.LoadString("Strings\\StringsFromCSFiles:SlayMonsterQuest.cs.13696") : Game1.content.LoadString("Strings\\1_6_Strings:MonsterQuestTitle", (object) Monster.GetDisplayName(slayMonsterQuest.monsterName.Value));
            break;
          case 5:
            this._questTitle = Game1.content.LoadString("Strings\\StringsFromCSFiles:SocializeQuest.cs.13785");
            break;
          case 7:
            if (this is FishingQuest fishingQuest && fishingQuest.ItemId.Value != null)
            {
              string sub1 = "???";
              ParsedItemData dataOrErrorItem = ItemRegistry.GetDataOrErrorItem(fishingQuest.ItemId.Value);
              if (!dataOrErrorItem.IsErrorItem)
                sub1 = dataOrErrorItem.DisplayName;
              this._questTitle = Game1.content.LoadString("Strings\\1_6_Strings:FishingQuestTitle", (object) sub1);
              break;
            }
            this._questTitle = Game1.content.LoadString("Strings\\StringsFromCSFiles:FishingQuest.cs.13227");
            break;
          case 10:
            if (this is ResourceCollectionQuest resourceCollectionQuest && resourceCollectionQuest.ItemId.Value != null)
            {
              string sub1 = "???";
              ParsedItemData dataOrErrorItem = ItemRegistry.GetDataOrErrorItem(resourceCollectionQuest.ItemId.Value);
              if (!dataOrErrorItem.IsErrorItem)
                sub1 = dataOrErrorItem.DisplayName;
              this._questTitle = Game1.content.LoadString("Strings\\1_6_Strings:ResourceQuestTitle", (object) sub1);
              break;
            }
            this._questTitle = Game1.content.LoadString("Strings\\StringsFromCSFiles:ResourceCollectionQuest.cs.13640");
            break;
        }
        this._questTitle = ArgUtility.Get(Quest.GetRawQuestFields(this.id.Value), 1, this._questTitle);
        this._loadedTitle = true;
      }
      if (this._questTitle == null)
        this._questTitle = "";
      return this._questTitle;
    }
    set => this._questTitle = value;
  }

  [XmlIgnore]
  public string questDescription
  {
    get
    {
      if (!this._loadedDescription)
      {
        this.reloadDescription();
        this._questDescription = ArgUtility.Get(Quest.GetRawQuestFields(this.id.Value), 2, this._questDescription);
        this._loadedDescription = true;
      }
      if (this._questDescription == null)
        this._questDescription = "";
      return this._questDescription;
    }
    set => this._questDescription = value;
  }

  [XmlIgnore]
  public string currentObjective
  {
    get
    {
      this._currentObjective = ArgUtility.Get(Quest.GetRawQuestFields(this.id.Value), 3, this._currentObjective, false);
      this.reloadObjective();
      if (this._currentObjective == null)
        this._currentObjective = "";
      return this._currentObjective;
    }
    set => this._currentObjective = value;
  }

  public static string[] GetRawQuestFields(string id)
  {
    if (id == null)
      return (string[]) null;
    Dictionary<string, string> dictionary = DataLoader.Quests(Game1.content);
    string str;
    return dictionary == null || !dictionary.TryGetValue(id, out str) ? (string[]) null : str.Split('/');
  }

  public static Quest getQuestFromId(string id)
  {
    string[] rawQuestFields = Quest.GetRawQuestFields(id);
    if (rawQuestFields == null)
      return (Quest) null;
    string str1;
    string error;
    string str2;
    string str3;
    string str4;
    string str5;
    int num1;
    string str6;
    bool flag1;
    if (!ArgUtility.TryGet(rawQuestFields, 0, out str1, out error, false, "string questType") || !ArgUtility.TryGet(rawQuestFields, 1, out str2, out error, false, "string title") || !ArgUtility.TryGet(rawQuestFields, 2, out str3, out error, false, "string description") || !ArgUtility.TryGetOptional(rawQuestFields, 3, out str4, out error, allowBlank: false, name: "string objective") || !ArgUtility.TryGetOptional(rawQuestFields, 5, out str5, out error, allowBlank: false, name: "string rawNextQuests") || !ArgUtility.TryGetInt(rawQuestFields, 6, out num1, out error, "int moneyReward") || !ArgUtility.TryGetOptional(rawQuestFields, 7, out str6, out error, allowBlank: false, name: "string rewardDescription") || !ArgUtility.TryGetOptionalBool(rawQuestFields, 8, out flag1, out error, name: "bool canBeCancelled"))
      return Quest.LogParseError(id, error);
    string[] strArray = ArgUtility.SplitBySpace(str5);
    if (str1 != null)
    {
      Quest questFromId;
      switch (str1.Length)
      {
        case 5:
          if (str1 == "Basic")
          {
            questFromId = new Quest();
            questFromId.questType.Value = 1;
            break;
          }
          goto label_65;
        case 6:
          if (str1 == "Social")
          {
            SocializeQuest socializeQuest = new SocializeQuest();
            socializeQuest.loadQuestInfo();
            questFromId = (Quest) socializeQuest;
            break;
          }
          goto label_65;
        case 7:
          if (str1 == "Monster")
          {
            string[] conditions;
            if (!Quest.TryParseConditions(rawQuestFields, out conditions, out error))
              return Quest.LogParseError(id, error);
            string str7;
            int num2;
            string str8;
            bool flag2;
            if (!ArgUtility.TryGet(conditions, 0, out str7, out error, false, "string monsterName") || !ArgUtility.TryGetInt(conditions, 1, out num2, out error, "int numberToKill") || !ArgUtility.TryGetOptional(conditions, 2, out str8, out error, name: "string targetNpc") || !ArgUtility.TryGetOptionalBool(conditions, 3, out flag2, out error, true, "bool ignoreFarmMonsters"))
              return Quest.LogConditionsParseError(id, error);
            SlayMonsterQuest slayMonsterQuest = new SlayMonsterQuest();
            slayMonsterQuest.loadQuestInfo();
            slayMonsterQuest.monster.Value.Name = str7.Replace('_', ' ');
            slayMonsterQuest.monsterName.Value = slayMonsterQuest.monster.Value.Name;
            slayMonsterQuest.numberToKill.Value = num2;
            slayMonsterQuest.ignoreFarmMonsters.Value = flag2;
            slayMonsterQuest.target.Value = str8 ?? "null";
            slayMonsterQuest.questType.Value = 4;
            questFromId = (Quest) slayMonsterQuest;
            break;
          }
          goto label_65;
        case 8:
          switch (str1[2])
          {
            case 'a':
              if (str1 == "Crafting")
              {
                string[] conditions;
                if (!Quest.TryParseConditions(rawQuestFields, out conditions, out error))
                  return Quest.LogParseError(id, error);
                string itemId;
                if (!ArgUtility.TryGet(conditions, 0, out itemId, out error, false, "string itemId"))
                  return Quest.LogConditionsParseError(id, error);
                bool? nullable = new bool?();
                if (ArgUtility.HasIndex<string>(conditions, 1))
                {
                  bool flag3;
                  if (!ArgUtility.TryGetOptionalBool(conditions, 1, out flag3, out error, name: "bool isBigCraftableValue"))
                    return Quest.LogConditionsParseError(id, error);
                  nullable = new bool?(flag3);
                }
                if (!ItemRegistry.IsQualifiedItemId(itemId))
                  itemId = !nullable.HasValue ? ItemRegistry.QualifyItemId(itemId) ?? itemId : (nullable.Value ? "(BC)" + itemId : "(O)" + itemId);
                questFromId = (Quest) new CraftingQuest(itemId);
                questFromId.questType.Value = 2;
                break;
              }
              goto label_65;
            case 'c':
              if (str1 == "Location")
              {
                string[] conditions;
                if (!Quest.TryParseConditions(rawQuestFields, out conditions, out error))
                  return Quest.LogParseError(id, error);
                string where;
                if (!ArgUtility.TryGet(conditions, 0, out where, out error, false, "string locationName"))
                  return Quest.LogConditionsParseError(id, error);
                questFromId = (Quest) new GoSomewhereQuest(where);
                questFromId.questType.Value = 6;
                break;
              }
              goto label_65;
            case 'i':
              if (str1 == "Building")
              {
                string[] conditions;
                if (!Quest.TryParseConditions(rawQuestFields, out conditions, out error))
                  return Quest.LogParseError(id, error);
                string buildingType;
                if (!ArgUtility.TryGet(conditions, 0, out buildingType, out error, false, "string buildingType"))
                  return Quest.LogConditionsParseError(id, error);
                questFromId = (Quest) new HaveBuildingQuest(buildingType);
                break;
              }
              goto label_65;
            case 's':
              if (str1 == "LostItem")
              {
                string[] conditions;
                if (!Quest.TryParseConditions(rawQuestFields, out conditions, out error))
                  return Quest.LogParseError(id, error);
                string npcName;
                string itemId;
                string locationOfItem;
                int tileX;
                int tileY;
                if (!ArgUtility.TryGet(conditions, 0, out npcName, out error, false, "string npcName") || !ArgUtility.TryGet(conditions, 1, out itemId, out error, false, "string itemId") || !ArgUtility.TryGet(conditions, 2, out locationOfItem, out error, false, "string locationOfItem") || !ArgUtility.TryGetInt(conditions, 3, out tileX, out error, "int tileX") || !ArgUtility.TryGetInt(conditions, 4, out tileY, out error, "int tileY"))
                  return Quest.LogConditionsParseError(id, error);
                questFromId = (Quest) new LostItemQuest(npcName, locationOfItem, itemId, tileX, tileY);
                break;
              }
              goto label_65;
            default:
              goto label_65;
          }
          break;
        case 11:
          if (str1 == "ItemHarvest")
          {
            string[] conditions;
            if (!Quest.TryParseConditions(rawQuestFields, out conditions, out error))
              return Quest.LogParseError(id, error);
            string itemId;
            int number;
            if (!ArgUtility.TryGet(conditions, 0, out itemId, out error, false, "string itemId") || !ArgUtility.TryGetOptionalInt(conditions, 1, out number, out error, 1, "int numberRequired"))
              return Quest.LogConditionsParseError(id, error);
            questFromId = (Quest) new ItemHarvestQuest(itemId, number);
            break;
          }
          goto label_65;
        case 12:
          if (str1 == "ItemDelivery")
          {
            string[] conditions;
            string str9;
            if (!Quest.TryParseConditions(rawQuestFields, out conditions, out error) || !ArgUtility.TryGet(rawQuestFields, 9, out str9, out error, false, "string targetMessage"))
              return Quest.LogParseError(id, error);
            string target;
            string itemId;
            int num3;
            if (!ArgUtility.TryGet(conditions, 0, out target, out error, false, "string npcName") || !ArgUtility.TryGet(conditions, 1, out itemId, out error, false, "string itemId") || !ArgUtility.TryGetOptionalInt(conditions, 2, out num3, out error, 1, "int numberRequired"))
              return Quest.LogConditionsParseError(id, error);
            ItemDeliveryQuest itemDeliveryQuest = new ItemDeliveryQuest(target, itemId);
            itemDeliveryQuest.targetMessage = str9;
            itemDeliveryQuest.number.Value = num3;
            itemDeliveryQuest.questType.Value = 3;
            questFromId = (Quest) itemDeliveryQuest;
            break;
          }
          goto label_65;
        case 14:
          if (str1 == "SecretLostItem")
          {
            string[] conditions;
            if (!Quest.TryParseConditions(rawQuestFields, out conditions, out error))
              return Quest.LogParseError(id, error);
            string npcName;
            string itemId;
            int friendshipReward;
            string exclusiveQuestId;
            if (!ArgUtility.TryGet(conditions, 0, out npcName, out error, false, "string npcName") || !ArgUtility.TryGet(conditions, 1, out itemId, out error, false, "string itemId") || !ArgUtility.TryGetInt(conditions, 2, out friendshipReward, out error, "int friendshipReward") || !ArgUtility.TryGetOptional(conditions, 3, out exclusiveQuestId, out error, allowBlank: false, name: "string exclusiveQuestId"))
              return Quest.LogConditionsParseError(id, error);
            questFromId = (Quest) new SecretLostItemQuest(npcName, itemId, friendshipReward, exclusiveQuestId);
            break;
          }
          goto label_65;
        default:
          goto label_65;
      }
      questFromId.id.Value = id;
      questFromId.questTitle = str2;
      questFromId.questDescription = str3;
      questFromId.currentObjective = str4;
      foreach (string str10 in strArray)
      {
        if (str10.StartsWith('h'))
        {
          if (Game1.IsMasterGame)
            str10 = str10.Substring(1);
          else
            continue;
        }
        questFromId.nextQuests.Add(str10);
      }
      questFromId.showNew.Value = true;
      questFromId.moneyReward.Value = num1;
      questFromId.rewardDescription.Value = num1 == -1 ? (string) null : str6;
      questFromId.canBeCancelled.Value = flag1;
      return questFromId;
    }
label_65:
    return Quest.LogParseError(id, $"quest type '{str1}' doesn't match a known type.");
  }

  public virtual void reloadObjective()
  {
  }

  public virtual void reloadDescription()
  {
  }

  public virtual void accept() => this.accepted.Value = true;

  /// <summary>Handle a building type existing in the save. This is called for each constructed building type which exists in the save.</summary>
  /// <param name="buildingType">The building type.</param>
  /// <param name="probe">Whether to return whether the quest would advance, without actually changing the quest state.</param>
  /// <returns>Returns whether the quest state changed (e.g. closer to completion or completed).</returns>
  public virtual bool OnBuildingExists(string buildingType, bool probe = false) => false;

  /// <summary>Handle the local player catching a fish.</summary>
  /// <param name="fishId">The qualified item ID of the caught fish.</param>
  /// <param name="numberCaught">The number of fish caught.</param>
  /// <param name="size">The fish size in inches.</param>
  /// <param name="probe">Whether to return whether the quest would advance, without actually changing the quest state.</param>
  /// <returns>Returns whether the quest state changed (e.g. closer to completion or completed).</returns>
  public virtual bool OnFishCaught(string fishId, int numberCaught, int size, bool probe = false)
  {
    return false;
  }

  /// <summary>Handle the local player catching a fish.</summary>
  /// <param name="item">The item that was received.</param>
  /// <param name="numberAdded">The number of items added to the player's inventory.</param>
  /// <param name="probe">Whether to return whether the quest would advance, without actually changing the quest state.</param>
  /// <returns>Returns whether the quest state changed (e.g. closer to completion or completed).</returns>
  public virtual bool OnItemReceived(Item item, int numberAdded, bool probe = false) => false;

  /// <summary>Handle the local player slaying a monster.</summary>
  /// <param name="location">The location containing the monster.</param>
  /// <param name="monster">The monster that was slain.</param>
  /// <param name="killedByBomb">Whether the monster was killed by a bomb placed by the player.</param>
  /// <param name="isTameMonster">Whether the slain monster was tame (e.g. a slime in a slime hutch).</param>
  /// <param name="probe">Whether to return whether the quest would advance, without actually changing the quest state.</param>
  /// <returns>Returns whether the quest state changed (e.g. closer to completion or completed).</returns>
  public virtual bool OnMonsterSlain(
    GameLocation location,
    Monster monster,
    bool killedByBomb,
    bool isTameMonster,
    bool probe = false)
  {
    return false;
  }

  /// <summary>Handle the local player talking to an NPC.</summary>
  /// <param name="npc">The NPC they talked to.</param>
  /// <param name="probe">Whether to return whether the quest would advance, without actually changing the quest state.</param>
  /// <returns>Returns whether the quest state changed (e.g. closer to completion or completed).</returns>
  public virtual bool OnNpcSocialized(NPC npc, bool probe = false) => false;

  /// <summary>Handle the local player crafting an item.</summary>
  /// <param name="recipe">The recipe that was crafted.</param>
  /// <param name="item">The item produced by the recipe.</param>
  /// <param name="probe">Whether to return whether the quest would advance, without actually changing the quest state.</param>
  /// <returns>Returns whether the quest state changed (e.g. closer to completion or completed).</returns>
  public virtual bool OnRecipeCrafted(CraftingRecipe recipe, Item item, bool probe = false)
  {
    return false;
  }

  /// <summary>Handle the local player arriving in a location.</summary>
  /// <param name="location">The recipe that was crafted.</param>
  /// <param name="probe">Whether to return whether the quest would advance, without actually changing the quest state.</param>
  /// <returns>Returns whether the quest state changed (e.g. closer to completion or completed).</returns>
  public virtual bool OnWarped(GameLocation location, bool probe = false) => false;

  /// <summary>Handle the local player offering an item to an NPC.</summary>
  /// <param name="npc">The NPC who would receive the item.</param>
  /// <param name="item">The item being offered.</param>
  /// <param name="probe">Whether to return whether the quest would advance, without actually changing the quest state.</param>
  /// <remarks>The item isn't deducted automatically from the player's inventory. If the quest should consume the item, it can do so within this method (e.g. via <see cref="M:StardewValley.Inventories.IInventory.Reduce(StardewValley.Item,System.Int32,System.Boolean)" />).</remarks>
  /// <returns>Returns whether the quest state changed (e.g. closer to completion or completed). Returning true prevents further processing (e.g. gifting the item to the NPC).</returns>
  public virtual bool OnItemOfferedToNpc(NPC npc, Item item, bool probe = false) => false;

  public bool hasReward()
  {
    if (this.moneyReward.Value > 0)
      return true;
    string str = this.rewardDescription.Value;
    return str != null && str.Length > 2;
  }

  public virtual bool isSecretQuest() => false;

  public virtual void questComplete()
  {
    if (this.completed.Value)
      return;
    if (this.dailyQuest.Value)
    {
      int num = (int) Game1.stats.Increment("BillboardQuestsDone");
      if (!Game1.player.mailReceived.Contains("completedFirstBillboardQuest"))
        Game1.player.mailReceived.Add("completedFirstBillboardQuest");
      if (Game1.stats.Get("BillboardQuestsDone") % 3U == 0U)
      {
        if (!Game1.player.addItemToInventoryBool(ItemRegistry.Create("(O)PrizeTicket")))
          Game1.createItemDebris(ItemRegistry.Create("(O)PrizeTicket"), Game1.player.getStandingPosition(), 2);
        if (Game1.stats.Get("BillboardQuestsDone") >= 6U && !Game1.player.mailReceived.Contains("gotFirstBillboardPrizeTicket"))
          Game1.player.mailReceived.Add("gotFirstBillboardPrizeTicket");
      }
    }
    if (this.dailyQuest.Value || this.questType.Value == 7)
      ++Game1.stats.QuestsCompleted;
    this.completed.Value = true;
    Game1.player.currentLocation?.customQuestCompleteBehavior(this.id.Value);
    if (this.nextQuests.Count > 0)
    {
      foreach (string nextQuest in (NetList<string, NetString>) this.nextQuests)
      {
        if (this.IsValidId(nextQuest))
          Game1.player.addQuest(nextQuest);
      }
      Game1.addHUDMessage(new HUDMessage(Game1.content.LoadString("Strings\\StringsFromCSFiles:Quest.cs.13636"), 2));
    }
    if (this.moneyReward.Value <= 0 && (this.rewardDescription.Value == null || this.rewardDescription.Value.Length <= 2))
      Game1.player.questLog.Remove(this);
    else
      Game1.addHUDMessage(new HUDMessage(Game1.content.LoadString("Strings\\StringsFromCSFiles:Quest.cs.13636"), 2));
    Game1.playSound("questcomplete");
    if (this.id.Value == "126")
    {
      Game1.player.mailReceived.Add("emilyFiber");
      Game1.player.activeDialogueEvents["emilyFiber"] = 2;
    }
    Game1.dayTimeMoneyBox.questsDirty = true;
    Game1.player.autoGenerateActiveDialogueEvent("questComplete_" + this.id.Value);
  }

  public string GetName() => this.questTitle;

  public string GetDescription() => this.questDescription;

  public bool IsHidden() => this.isSecretQuest();

  public List<string> GetObjectiveDescriptions()
  {
    return new List<string>() { this.currentObjective };
  }

  public bool CanBeCancelled() => this.canBeCancelled.Value;

  public bool HasReward()
  {
    if (this.HasMoneyReward())
      return true;
    string str = this.rewardDescription.Value;
    return str != null && str.Length > 2;
  }

  public bool HasMoneyReward() => this.completed.Value && this.moneyReward.Value > 0;

  public void MarkAsViewed() => this.showNew.Value = false;

  public bool ShouldDisplayAsNew() => this.showNew.Value;

  public bool ShouldDisplayAsComplete() => this.completed.Value && !this.IsHidden();

  public bool IsTimedQuest() => this.dailyQuest.Value || this.GetDaysLeft() > 0;

  public int GetDaysLeft() => this.daysLeft.Value;

  public int GetMoneyReward() => this.moneyReward.Value;

  public void OnMoneyRewardClaimed()
  {
    this.moneyReward.Value = 0;
    this.destroy.Value = true;
  }

  public bool OnLeaveQuestPage()
  {
    if (this.completed.Value && this.moneyReward.Value <= 0)
      this.destroy.Value = true;
    if (!this.destroy.Value)
      return false;
    Game1.player.questLog.Remove(this);
    return true;
  }

  /// <summary>Get whether the <see cref="F:StardewValley.Quests.Quest.id" /> is set to a valid value.</summary>
  protected bool HasId() => this.IsValidId(this.id.Value);

  /// <summary>Get whether the given quest ID is valid.</summary>
  /// <param name="id">The quest ID to check.</param>
  protected bool IsValidId(string id)
  {
    switch (id)
    {
      case "7":
        return Game1.GetFarmTypeID() != "MeadowlandsFarm";
      case null:
      case "-1":
      case "0":
        return false;
      default:
        return true;
    }
  }

  /// <summary>Create an RNG instance intended to initialize the quest fields.</summary>
  protected Random CreateInitializationRandom()
  {
    return Utility.CreateRandom((double) Game1.uniqueIDForThisGame, (double) Game1.stats.DaysPlayed);
  }

  /// <summary>Get the split quest conditions from raw quest fields, if it's found and valid.</summary>
  /// <param name="questFields">The raw quest fields.</param>
  /// <param name="conditions">The parsed conditions.</param>
  /// <param name="error">The error message indicating why parsing failed.</param>
  /// <param name="allowBlank">Whether to match the argument even if it's null or whitespace. If false, it will be treated as invalid in that case.</param>
  /// <returns>Returns whether the conditions field was found and valid.</returns>
  protected static bool TryParseConditions(
    string[] questFields,
    out string[] conditions,
    out string error,
    bool allowBlank = false)
  {
    string str;
    if (!ArgUtility.TryGet(questFields, 4, out str, out error, allowBlank, "string rawConditions"))
    {
      conditions = (string[]) null;
      return false;
    }
    conditions = ArgUtility.SplitBySpace(str);
    error = (string) null;
    return true;
  }

  /// <summary>Log an error message indicating that the quest data couldn't be parsed.</summary>
  /// <param name="id">The quest ID being parsed.</param>
  /// <param name="error">The error message indicating why parsing failed.</param>
  /// <returns>Returns a null quest for convenience.</returns>
  protected static Quest LogParseError(string id, string error)
  {
    Game1.log.Error($"Failed to parse data for quest '{id}': {error}");
    return (Quest) null;
  }

  /// <summary>Log an error message indicating that the conditions field in the quest data couldn't be parsed.</summary>
  /// <param name="id">The quest ID being parsed.</param>
  /// <param name="error">The error message indicating why parsing failed.</param>
  /// <returns>Returns a null quest for convenience.</returns>
  protected static Quest LogConditionsParseError(string id, string error)
  {
    Game1.log.Error($"Failed to parse for quest '{id}': conditions field (index 4) is invalid: {error}");
    return (Quest) null;
  }
}
