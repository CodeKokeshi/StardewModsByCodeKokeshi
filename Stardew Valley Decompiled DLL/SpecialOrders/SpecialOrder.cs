// Decompiled with JetBrains decompiler
// Type: StardewValley.SpecialOrders.SpecialOrder
// Assembly: Stardew Valley, Version=1.6.15.24356, Culture=neutral, PublicKeyToken=null
// MVID: AA1F513A-94F0-4EF6-A35F-2D5B4A3721BF
// Assembly location: D:\SteamLibrary\steamapps\common\Stardew Valley\Stardew Valley.dll
// XML documentation location: D:\SteamLibrary\steamapps\common\Stardew Valley\Stardew Valley.xml

using Netcode;
using Netcode.Validation;
using StardewValley.BellsAndWhistles;
using StardewValley.Extensions;
using StardewValley.GameData.SpecialOrders;
using StardewValley.ItemTypeDefinitions;
using StardewValley.Monsters;
using StardewValley.Network;
using StardewValley.Quests;
using StardewValley.SpecialOrders.Objectives;
using StardewValley.SpecialOrders.Rewards;
using StardewValley.TokenizableStrings;
using System;
using System.Collections.Generic;
using System.Xml.Serialization;

#nullable disable
namespace StardewValley.SpecialOrders;

[XmlInclude(typeof (OrderObjective))]
[XmlInclude(typeof (OrderReward))]
[NotImplicitNetField]
public class SpecialOrder : INetObject<NetFields>, IQuest
{
  [XmlIgnore]
  public Action<Farmer, Item, int> onItemShipped;
  [XmlIgnore]
  public Action<Farmer, Monster> onMonsterSlain;
  [XmlIgnore]
  public Action<Farmer, Item> onFishCaught;
  [XmlIgnore]
  public Action<Farmer, NPC, Item> onGiftGiven;
  [XmlIgnore]
  public Func<Farmer, NPC, Item, bool, int> onItemDelivered;
  [XmlIgnore]
  public Action<Farmer, Item> onItemCollected;
  [XmlIgnore]
  public Action<Farmer, int> onMineFloorReached;
  [XmlIgnore]
  public Action<Farmer, int> onJKScoreAchieved;
  [XmlIgnore]
  protected bool _objectiveRegistrationDirty;
  [XmlElement("preSelectedItems")]
  public NetStringDictionary<string, NetString> preSelectedItems = new NetStringDictionary<string, NetString>();
  [XmlElement("selectedRandomElements")]
  public NetStringDictionary<int, NetInt> selectedRandomElements = new NetStringDictionary<int, NetInt>();
  [XmlElement("objectives")]
  public NetList<OrderObjective, NetRef<OrderObjective>> objectives = new NetList<OrderObjective, NetRef<OrderObjective>>();
  [XmlElement("generationSeed")]
  public NetInt generationSeed = new NetInt();
  [XmlElement("seenParticipantsIDs")]
  public NetLongDictionary<bool, NetBool> seenParticipants = new NetLongDictionary<bool, NetBool>();
  [XmlElement("participantsIDs")]
  public NetLongDictionary<bool, NetBool> participants = new NetLongDictionary<bool, NetBool>();
  [XmlElement("unclaimedRewardsIDs")]
  public NetLongDictionary<bool, NetBool> unclaimedRewards = new NetLongDictionary<bool, NetBool>();
  [XmlElement("donatedItems")]
  public readonly NetCollection<Item> donatedItems = new NetCollection<Item>();
  [XmlElement("appliedSpecialRules")]
  public bool appliedSpecialRules;
  [XmlIgnore]
  public readonly NetMutex donateMutex = new NetMutex();
  [XmlIgnore]
  protected int _isIslandOrder = -1;
  [XmlElement("rewards")]
  public NetList<OrderReward, NetRef<OrderReward>> rewards = new NetList<OrderReward, NetRef<OrderReward>>();
  [XmlIgnore]
  protected int _moneyReward = -1;
  [XmlElement("questKey")]
  public NetString questKey = new NetString();
  [XmlElement("questName")]
  public NetString questName = new NetString("Strings\\SpecialOrders:PlaceholderName");
  [XmlElement("questDescription")]
  public NetString questDescription = new NetString("Strings\\SpecialOrders:PlaceholderDescription");
  [XmlElement("requester")]
  public NetString requester = new NetString();
  [XmlElement("orderType")]
  public NetString orderType = new NetString("");
  [XmlElement("specialRule")]
  public NetString specialRule = new NetString("");
  [XmlElement("readyForRemoval")]
  public NetBool readyForRemoval = new NetBool(false);
  [XmlElement("itemToRemoveOnEnd")]
  public NetString itemToRemoveOnEnd = new NetString();
  [XmlElement("mailToRemoveOnEnd")]
  public NetString mailToRemoveOnEnd = new NetString();
  [XmlIgnore]
  protected string _localizedName;
  [XmlIgnore]
  protected string _localizedDescription;
  [XmlElement("dueDate")]
  public NetInt dueDate = new NetInt();
  [XmlElement("duration")]
  public NetEnum<QuestDuration> questDuration = new NetEnum<QuestDuration>();
  [XmlIgnore]
  protected List<OrderObjective> _registeredObjectives = new List<OrderObjective>();
  [XmlIgnore]
  protected Dictionary<Item, bool> _highlightLookup;
  [XmlIgnore]
  protected SpecialOrderData _orderData;
  [XmlElement("questState")]
  public NetEnum<SpecialOrderStatus> questState = new NetEnum<SpecialOrderStatus>(SpecialOrderStatus.InProgress);

  [XmlIgnore]
  public NetFields NetFields { get; } = new NetFields(nameof (SpecialOrder));

  public SpecialOrder() => this.InitializeNetFields();

  public virtual void SetDuration(QuestDuration duration)
  {
    this.questDuration.Value = duration;
    WorldDate worldDate = new WorldDate();
    switch (duration)
    {
      case QuestDuration.Week:
        worldDate = new WorldDate(Game1.year, Game1.season, (Game1.dayOfMonth - 1) / 7 * 7);
        ++worldDate.TotalDays;
        worldDate.TotalDays += 7;
        break;
      case QuestDuration.Month:
        worldDate = new WorldDate(Game1.year, Game1.season, 0);
        ++worldDate.TotalDays;
        worldDate.TotalDays += 28;
        break;
      case QuestDuration.TwoWeeks:
        worldDate = new WorldDate(Game1.year, Game1.season, (Game1.dayOfMonth - 1) / 7 * 7);
        ++worldDate.TotalDays;
        worldDate.TotalDays += 14;
        break;
      case QuestDuration.TwoDays:
        worldDate = WorldDate.Now();
        worldDate.TotalDays += 2;
        break;
      case QuestDuration.ThreeDays:
        worldDate = WorldDate.Now();
        worldDate.TotalDays += 3;
        break;
      case QuestDuration.OneDay:
        worldDate = new WorldDate(Game1.year, Game1.currentSeason, Game1.dayOfMonth);
        ++worldDate.TotalDays;
        break;
    }
    this.dueDate.Value = worldDate.TotalDays;
  }

  public virtual void OnFail()
  {
    foreach (OrderObjective objective in this.objectives)
      objective.OnFail();
    for (int index = 0; index < this.donatedItems.Count; ++index)
    {
      Item donatedItem = this.donatedItems[index];
      this.donatedItems[index] = (Item) null;
      if (donatedItem != null)
      {
        Game1.player.team.returnedDonations.Add(donatedItem);
        Game1.player.team.newLostAndFoundItems.Value = true;
      }
    }
    if (Game1.IsMasterGame)
      this.HostHandleQuestEnd();
    this.questState.Value = SpecialOrderStatus.Failed;
    this._RemoveSpecialRuleIfNecessary();
  }

  public virtual int GetCompleteObjectivesCount()
  {
    int completeObjectivesCount = 0;
    foreach (OrderObjective objective in this.objectives)
    {
      if (objective.IsComplete())
        ++completeObjectivesCount;
    }
    return completeObjectivesCount;
  }

  public virtual void ConfirmCompleteDonations()
  {
    foreach (OrderObjective objective in this.objectives)
    {
      if (objective is DonateObjective donateObjective)
        donateObjective.Confirm();
    }
  }

  public virtual void UpdateDonationCounts()
  {
    this._highlightLookup = (Dictionary<Item, bool>) null;
    int num1 = 0;
    int num2 = 0;
    foreach (OrderObjective objective in this.objectives)
    {
      if (objective is DonateObjective donateObjective)
      {
        int new_count = 0;
        if (donateObjective.GetCount() >= donateObjective.GetMaxCount())
          ++num1;
        foreach (Item donatedItem in this.donatedItems)
        {
          if (donateObjective.IsValidItem(donatedItem))
            new_count += donatedItem.Stack;
        }
        donateObjective.SetCount(new_count);
        if (donateObjective.GetCount() >= donateObjective.GetMaxCount())
          ++num2;
      }
    }
    if (num2 <= num1)
      return;
    Game1.playSound("newArtifact");
  }

  public bool HighlightAcceptableItems(Item item)
  {
    bool flag;
    if (this._highlightLookup != null && this._highlightLookup.TryGetValue(item, out flag))
      return flag;
    if (this._highlightLookup == null)
      this._highlightLookup = new Dictionary<Item, bool>();
    foreach (OrderObjective objective in this.objectives)
    {
      if (objective is DonateObjective donateObjective && donateObjective.GetAcceptCount(item, 1) > 0)
      {
        this._highlightLookup[item] = true;
        return true;
      }
    }
    this._highlightLookup[item] = false;
    return false;
  }

  public virtual int GetAcceptCount(Item item)
  {
    int acceptCount1 = 0;
    int stack = item.Stack;
    foreach (OrderObjective objective in this.objectives)
    {
      if (objective is DonateObjective donateObjective)
      {
        int acceptCount2 = donateObjective.GetAcceptCount(item, stack);
        stack -= acceptCount2;
        acceptCount1 += acceptCount2;
      }
    }
    return acceptCount1;
  }

  public static bool CheckTags(string tag_list)
  {
    if (tag_list == null)
      return true;
    string[] strArray = tag_list.Split(',');
    for (int index = 0; index < strArray.Length; ++index)
      strArray[index] = strArray[index].Trim();
    foreach (string tag in strArray)
    {
      if (tag.Length != 0)
      {
        bool flag = true;
        if (tag.StartsWith('!'))
        {
          flag = false;
          tag = tag.Substring(1);
        }
        if (SpecialOrder.CheckTag(tag) != flag)
          return false;
      }
    }
    return true;
  }

  public static bool CheckTag(string tag)
  {
    if (tag == "NOT_IMPLEMENTED")
      return false;
    if (tag.StartsWith("dropbox_"))
    {
      string box_id = tag.Substring("dropbox_".Length);
      foreach (SpecialOrder specialOrder in Game1.player.team.specialOrders)
      {
        if (specialOrder.UsesDropBox(box_id))
          return true;
      }
    }
    if (tag.StartsWith("rule_") && Game1.player.team.SpecialOrderRuleActive(tag.Substring("rule_".Length)) || tag.StartsWith("completed_") && Game1.player.team.completedSpecialOrders.Contains(tag.Substring("completed_".Length)))
      return true;
    if (tag.StartsWith("season_"))
    {
      if (Game1.currentSeason == tag.Substring("season_".Length))
        return true;
    }
    else if (tag.StartsWith("mail_"))
    {
      if (Game1.MasterPlayer.hasOrWillReceiveMail(tag.Substring("mail_".Length)))
        return true;
    }
    else if (tag.StartsWith("event_"))
    {
      if (Game1.MasterPlayer.eventsSeen.Contains(tag.Substring("event_".Length)))
        return true;
    }
    else
    {
      if (tag == "island")
        return Utility.doesAnyFarmerHaveOrWillReceiveMail("seenBoatJourney");
      if (tag.StartsWith("knows_"))
      {
        string key = tag.Substring("knows_".Length);
        foreach (Farmer allFarmer in Game1.getAllFarmers())
        {
          if (allFarmer.friendshipData.ContainsKey(key))
            return true;
        }
      }
    }
    return false;
  }

  public bool IsIslandOrder()
  {
    SpecialOrderData specialOrderData;
    if (this._isIslandOrder == -1 && DataLoader.SpecialOrders(Game1.content).TryGetValue(this.questKey.Value, out specialOrderData))
    {
      string requiredTags = specialOrderData.RequiredTags;
      this._isIslandOrder = (requiredTags != null ? (requiredTags.Contains("island") ? 1 : 0) : 0) != 0 ? (this._isIslandOrder = 1) : (this._isIslandOrder = 0);
    }
    return this._isIslandOrder == 1;
  }

  public static bool IsSpecialOrdersBoardUnlocked() => Game1.stats.DaysPlayed >= 58U;

  public static void RemoveAllSpecialOrders(string orderType)
  {
    Game1.player.team.availableSpecialOrders.RemoveWhere((Func<SpecialOrder, bool>) (order => order.orderType.Value == orderType));
    Game1.player.team.acceptedSpecialOrderTypes.Remove(orderType);
  }

  public static void UpdateAvailableSpecialOrders(string orderType, bool forceRefresh)
  {
    foreach (SpecialOrder availableSpecialOrder in Game1.player.team.availableSpecialOrders)
    {
      if ((availableSpecialOrder.questDuration.Value == QuestDuration.TwoDays || availableSpecialOrder.questDuration.Value == QuestDuration.ThreeDays) && !Game1.player.team.acceptedSpecialOrderTypes.Contains(availableSpecialOrder.orderType.Value))
        availableSpecialOrder.SetDuration(availableSpecialOrder.questDuration.Value);
    }
    if (!forceRefresh)
    {
      foreach (SpecialOrder availableSpecialOrder in Game1.player.team.availableSpecialOrders)
      {
        if (availableSpecialOrder.orderType.Value == orderType)
          return;
      }
    }
    SpecialOrder.RemoveAllSpecialOrders(orderType);
    List<string> stringList = new List<string>();
    foreach (KeyValuePair<string, SpecialOrderData> specialOrder in DataLoader.SpecialOrders(Game1.content))
    {
      if (specialOrder.Value.OrderType == orderType && SpecialOrder.CanStartOrderNow(specialOrder.Key, specialOrder.Value))
        stringList.Add(specialOrder.Key);
    }
    List<string> collection = new List<string>((IEnumerable<string>) stringList);
    if (orderType == "")
      stringList.RemoveAll((Predicate<string>) (id => Game1.player.team.completedSpecialOrders.Contains(id)));
    Random random = Utility.CreateRandom((double) Game1.uniqueIDForThisGame, (double) Game1.stats.DaysPlayed * 1.3);
    for (int index = 0; index < 2; ++index)
    {
      if (stringList.Count == 0)
      {
        if (collection.Count == 0)
          break;
        stringList = new List<string>((IEnumerable<string>) collection);
      }
      string key = random.ChooseFrom<string>((IList<string>) stringList);
      Game1.player.team.availableSpecialOrders.Add(SpecialOrder.GetSpecialOrder(key, new int?(random.Next())));
      stringList.Remove(key);
      collection.Remove(key);
    }
  }

  /// <summary>Get whether a special order is eligible to be started now by the player.</summary>
  /// <param name="orderId">The order ID in <c>Data/SpecialOrders</c>.</param>
  /// <param name="order">The special order data.</param>
  public static bool CanStartOrderNow(string orderId, SpecialOrderData order)
  {
    if (!order.Repeatable && Game1.MasterPlayer.team.completedSpecialOrders.Contains(orderId) || Game1.dayOfMonth >= 16 /*0x10*/ && order.Duration == QuestDuration.Month || !SpecialOrder.CheckTags(order.RequiredTags) || !GameStateQuery.CheckConditions(order.Condition))
      return false;
    foreach (SpecialOrder specialOrder in Game1.player.team.specialOrders)
    {
      if (specialOrder.questKey.Value == orderId)
        return false;
    }
    return true;
  }

  public static SpecialOrder GetSpecialOrder(string key, int? generation_seed)
  {
    try
    {
      if (!generation_seed.HasValue)
        generation_seed = new int?(Game1.random.Next());
      SpecialOrderData specialOrderData;
      if (DataLoader.SpecialOrders(Game1.content).TryGetValue(key, out specialOrderData))
      {
        Random random = Utility.CreateRandom((double) generation_seed.Value);
        SpecialOrder order = new SpecialOrder();
        order.generationSeed.Value = generation_seed.Value;
        order._orderData = specialOrderData;
        order.questKey.Value = key;
        order.questName.Value = specialOrderData.Name;
        order.requester.Value = specialOrderData.Requester;
        order.orderType.Value = specialOrderData.OrderType.Trim();
        order.specialRule.Value = specialOrderData.SpecialRule.Trim();
        if (specialOrderData.ItemToRemoveOnEnd != null)
          order.itemToRemoveOnEnd.Value = specialOrderData.ItemToRemoveOnEnd;
        if (specialOrderData.MailToRemoveOnEnd != null)
          order.mailToRemoveOnEnd.Value = specialOrderData.MailToRemoveOnEnd;
        order.selectedRandomElements.Clear();
        if (specialOrderData.RandomizedElements != null)
        {
          foreach (RandomizedElement randomizedElement in specialOrderData.RandomizedElements)
          {
            List<int> options1 = new List<int>();
            for (int index = 0; index < randomizedElement.Values.Count; ++index)
            {
              if (SpecialOrder.CheckTags(randomizedElement.Values[index].RequiredTags))
                options1.Add(index);
            }
            int index1 = random.ChooseFrom<int>((IList<int>) options1);
            order.selectedRandomElements[randomizedElement.Name] = index1;
            string str1 = randomizedElement.Values[index1].Value;
            if (str1.StartsWith("PICK_ITEM"))
            {
              string[] strArray = str1.Substring("PICK_ITEM".Length).Split(',');
              List<string> options2 = new List<string>();
              foreach (string str2 in strArray)
              {
                string str3 = str2.Trim();
                if (str3.Length != 0)
                {
                  ParsedItemData data = ItemRegistry.GetData(str3);
                  if (data != null)
                  {
                    options2.Add(data.QualifiedItemId);
                  }
                  else
                  {
                    Item obj = Utility.fuzzyItemSearch(str3);
                    options2.Add(obj.QualifiedItemId);
                  }
                }
              }
              order.preSelectedItems[randomizedElement.Name] = random.ChooseFrom<string>((IList<string>) options2);
            }
          }
        }
        order.SetDuration(specialOrderData.Duration);
        order.questDescription.Value = specialOrderData.Text;
        string str4 = typeof (OrderObjective).Namespace;
        string str5 = typeof (OrderReward).Namespace;
        foreach (SpecialOrderObjectiveData objective in specialOrderData.Objectives)
        {
          Type type = Type.GetType($"{str4}.{objective.Type.Trim()}Objective");
          if (!(type == (Type) null) && type.IsSubclassOf(typeof (OrderObjective)))
          {
            OrderObjective instance = (OrderObjective) Activator.CreateInstance(type);
            if (instance != null)
            {
              instance.description.Value = objective.Text;
              instance.maxCount.Value = int.Parse(order.Parse(objective.RequiredCount));
              instance.Load(order, objective.Data);
              order.objectives.Add(instance);
            }
          }
        }
        foreach (SpecialOrderRewardData reward in specialOrderData.Rewards)
        {
          Type type = Type.GetType($"{str5}.{reward.Type.Trim()}Reward");
          if (!(type == (Type) null) && type.IsSubclassOf(typeof (OrderReward)))
          {
            OrderReward instance = (OrderReward) Activator.CreateInstance(type);
            if (instance != null)
            {
              instance.Load(order, reward.Data);
              order.rewards.Add(instance);
            }
          }
        }
        return order;
      }
    }
    catch (Exception ex)
    {
      Game1.log.Error($"Failed loading special order '{key}'.", ex);
    }
    return (SpecialOrder) null;
  }

  public static string MakeLocalizationReplacements(string data)
  {
    data = data.Trim();
    int startIndex;
    do
    {
      startIndex = data.LastIndexOf('[');
      if (startIndex >= 0)
      {
        int num = data.IndexOf(']', startIndex);
        if (num == -1)
          return data;
        string str1 = data.Substring(startIndex + 1, num - startIndex - 1);
        string str2 = Game1.content.LoadString("Strings\\SpecialOrderStrings:" + str1);
        data = data.Remove(startIndex, num - startIndex + 1);
        data = data.Insert(startIndex, str2);
      }
    }
    while (startIndex >= 0);
    return data;
  }

  public virtual string Parse(string data)
  {
    data = data.Trim();
    this.GetData();
    data = SpecialOrder.MakeLocalizationReplacements(data);
    int startIndex;
    do
    {
      startIndex = data.LastIndexOf('{');
      if (startIndex >= 0)
      {
        int num = data.IndexOf('}', startIndex);
        if (num == -1)
          return data;
        string str1 = data.Substring(startIndex + 1, num - startIndex - 1);
        string str2 = str1;
        string key = str1;
        string str3 = (string) null;
        if (str1.Contains(':'))
        {
          string[] strArray = str1.Split(':');
          key = strArray[0];
          if (strArray.Length > 1)
            str3 = strArray[1];
        }
        if (this._orderData.RandomizedElements != null)
        {
          string itemId;
          if (this.preSelectedItems.TryGetValue(key, out itemId))
          {
            Item obj = ItemRegistry.Create(itemId);
            if (!(str3 == "Text"))
            {
              if (!(str3 == "TextPlural"))
              {
                if (!(str3 == "TextPluralCapitalized"))
                {
                  if (!(str3 == "Tags"))
                  {
                    if (str3 == "Price")
                      str2 = obj is StardewValley.Object @object ? @object.sellToStorePrice(-1L).ToString() ?? "" : "1";
                  }
                  else
                  {
                    string str4 = "id_" + Utility.getStandardDescriptionFromItem(obj, 0, '_');
                    str2 = str4.Substring(0, str4.Length - 2).ToLower();
                  }
                }
                else
                  str2 = Utility.capitalizeFirstLetter(Lexicon.makePlural(obj.DisplayName));
              }
              else
                str2 = Lexicon.makePlural(obj.DisplayName);
            }
            else
              str2 = obj.DisplayName;
          }
          else
          {
            int index;
            if (this.selectedRandomElements.TryGetValue(key, out index))
            {
              foreach (RandomizedElement randomizedElement in this._orderData.RandomizedElements)
              {
                if (randomizedElement.Name == key)
                {
                  str2 = SpecialOrder.MakeLocalizationReplacements(randomizedElement.Values[index].Value);
                  break;
                }
              }
            }
          }
        }
        if (str3 != null)
        {
          string[] strArray = str2.Split('|');
          for (int index = 0; index < strArray.Length; index += 2)
          {
            if (index + 1 <= strArray.Length && strArray[index] == str3)
            {
              str2 = strArray[index + 1];
              break;
            }
          }
        }
        data = data.Remove(startIndex, num - startIndex + 1);
        data = data.Insert(startIndex, str2);
      }
    }
    while (startIndex >= 0);
    return data;
  }

  /// <summary>Get the special order's data from <c>Data/SpecialOrders</c>, if found.</summary>
  public virtual SpecialOrderData GetData()
  {
    if (this._orderData == null)
      SpecialOrder.TryGetData(this.questKey.Value, out this._orderData);
    return this._orderData;
  }

  /// <summary>Try to get a special order's data from <c>Data/SpecialOrders</c>.</summary>
  /// <param name="id">The special order ID (i.e. the key in <c>Data/SpecialOrders</c>).</param>
  /// <param name="data">The special order data, if found.</param>
  /// <returns>Returns whether the special order data was found.</returns>
  public static bool TryGetData(string id, out SpecialOrderData data)
  {
    if (id != null)
      return DataLoader.SpecialOrders(Game1.content).TryGetValue(id, out data);
    data = (SpecialOrderData) null;
    return false;
  }

  public virtual void InitializeNetFields()
  {
    this.NetFields.SetOwner((INetObject<NetFields>) this).AddField((INetSerializable) this.questName, "questName").AddField((INetSerializable) this.questDescription, "questDescription").AddField((INetSerializable) this.dueDate, "dueDate").AddField((INetSerializable) this.objectives, "objectives").AddField((INetSerializable) this.rewards, "rewards").AddField((INetSerializable) this.questState, "questState").AddField((INetSerializable) this.donatedItems, "donatedItems").AddField((INetSerializable) this.questKey, "questKey").AddField((INetSerializable) this.requester, "requester").AddField((INetSerializable) this.generationSeed, "generationSeed").AddField((INetSerializable) this.selectedRandomElements, "selectedRandomElements").AddField((INetSerializable) this.preSelectedItems, "preSelectedItems").AddField((INetSerializable) this.orderType, "orderType").AddField((INetSerializable) this.specialRule, "specialRule").AddField((INetSerializable) this.participants, "participants").AddField((INetSerializable) this.seenParticipants, "seenParticipants").AddField((INetSerializable) this.unclaimedRewards, "unclaimedRewards").AddField((INetSerializable) this.donateMutex.NetFields, "donateMutex.NetFields").AddField((INetSerializable) this.itemToRemoveOnEnd, "itemToRemoveOnEnd").AddField((INetSerializable) this.mailToRemoveOnEnd, "mailToRemoveOnEnd").AddField((INetSerializable) this.questDuration, "questDuration").AddField((INetSerializable) this.readyForRemoval, "readyForRemoval");
    this.objectives.OnArrayReplaced += (NetList<OrderObjective, NetRef<OrderObjective>>.ArrayReplacedEvent) ((_param1, _param2, _param3) => this._objectiveRegistrationDirty = true);
    this.objectives.OnElementChanged += (NetList<OrderObjective, NetRef<OrderObjective>>.ElementChangedEvent) ((_param1, _param2, _param3, _param4) => this._objectiveRegistrationDirty = true);
  }

  protected virtual void _UpdateObjectiveRegistration()
  {
    for (int index = 0; index < this._registeredObjectives.Count; ++index)
    {
      OrderObjective registeredObjective = this._registeredObjectives[index];
      if (!this.objectives.Contains(registeredObjective))
        registeredObjective.Unregister();
    }
    foreach (OrderObjective objective in this.objectives)
    {
      if (!this._registeredObjectives.Contains(objective))
      {
        objective.Register(this);
        this._registeredObjectives.Add(objective);
      }
    }
  }

  public bool UsesDropBox(string box_id)
  {
    if (this.questState.Value != SpecialOrderStatus.InProgress)
      return false;
    foreach (OrderObjective objective in this.objectives)
    {
      if (objective is DonateObjective donateObjective && donateObjective.dropBox.Value == box_id)
        return true;
    }
    return false;
  }

  public int GetMinimumDropBoxCapacity(string box_id)
  {
    int val1 = 9;
    foreach (OrderObjective objective in this.objectives)
    {
      if (objective is DonateObjective donateObjective && donateObjective.dropBox.Value == box_id && donateObjective.minimumCapacity.Value > 0)
        val1 = Math.Max(val1, donateObjective.minimumCapacity.Value);
    }
    return val1;
  }

  public virtual void Update()
  {
    this._AddSpecialRulesIfNecessary();
    if (this._objectiveRegistrationDirty)
    {
      this._objectiveRegistrationDirty = false;
      this._UpdateObjectiveRegistration();
    }
    if (!this.readyForRemoval.Value)
    {
      switch (this.questState.Value)
      {
        case SpecialOrderStatus.InProgress:
          this.participants.TryAdd(Game1.player.UniqueMultiplayerID, true);
          break;
        case SpecialOrderStatus.Complete:
          if (this.unclaimedRewards.Remove(Game1.player.UniqueMultiplayerID))
          {
            ++Game1.stats.QuestsCompleted;
            Game1.playSound("questcomplete");
            Game1.dayTimeMoneyBox.questsDirty = true;
            if (this.orderType.Value == "" && !this.questKey.Value.Contains("QiChallenge") && !this.questKey.Value.Contains("DesertFestival"))
            {
              int num = (int) Game1.player.stats.Increment("specialOrderPrizeTickets");
            }
            foreach (OrderReward reward in this.rewards)
              reward.Grant();
          }
          if (this.participants.ContainsKey(Game1.player.UniqueMultiplayerID) && this.GetMoneyReward() <= 0)
          {
            this.RemoveFromParticipants();
            break;
          }
          break;
      }
    }
    this.donateMutex.Update(Game1.getOnlineFarmers());
    if (this.donateMutex.IsLockHeld() && Game1.activeClickableMenu == null)
      this.donateMutex.ReleaseLock();
    if (Game1.activeClickableMenu == null)
      this._highlightLookup = (Dictionary<Item, bool>) null;
    if (!Game1.IsMasterGame || this.questState.Value == SpecialOrderStatus.InProgress)
      return;
    this.MarkForRemovalIfEmpty();
    if (!this.readyForRemoval.Value)
      return;
    this._RemoveSpecialRuleIfNecessary();
    Game1.player.team.specialOrders.Remove(this);
  }

  public virtual void RemoveFromParticipants()
  {
    this.participants.Remove(Game1.player.UniqueMultiplayerID);
    this.MarkForRemovalIfEmpty();
  }

  public virtual void MarkForRemovalIfEmpty()
  {
    if (this.participants.Length != 0)
      return;
    this.readyForRemoval.Value = true;
  }

  public virtual void HostHandleQuestEnd()
  {
    if (!Game1.IsMasterGame)
      return;
    if (this.itemToRemoveOnEnd.Value != null && !Game1.player.team.itemsToRemoveOvernight.Contains(this.itemToRemoveOnEnd.Value))
      Game1.player.team.itemsToRemoveOvernight.Add(this.itemToRemoveOnEnd.Value);
    if (this.mailToRemoveOnEnd.Value == null || Game1.player.team.mailToRemoveOvernight.Contains(this.mailToRemoveOnEnd.Value))
      return;
    Game1.player.team.mailToRemoveOvernight.Add(this.mailToRemoveOnEnd.Value);
  }

  protected void _AddSpecialRulesIfNecessary()
  {
    if (!Game1.IsMasterGame || this.appliedSpecialRules || this.questState.Value != SpecialOrderStatus.InProgress)
      return;
    this.appliedSpecialRules = true;
    foreach (string str1 in this.specialRule.Value.Split(','))
    {
      string str2 = str1.Trim();
      if (!Game1.player.team.SpecialOrderRuleActive(str2, this))
      {
        this.AddSpecialRule(str2);
        if (Game1.player.team.specialRulesRemovedToday.Contains(str2))
          Game1.player.team.specialRulesRemovedToday.Remove(str2);
      }
    }
  }

  protected void _RemoveSpecialRuleIfNecessary()
  {
    if (!Game1.IsMasterGame || !this.appliedSpecialRules)
      return;
    this.appliedSpecialRules = false;
    foreach (string str1 in this.specialRule.Value.Split(','))
    {
      string str2 = str1.Trim();
      if (!Game1.player.team.SpecialOrderRuleActive(str2, this))
      {
        this.RemoveSpecialRule(str2);
        if (!Game1.player.team.specialRulesRemovedToday.Contains(str2))
          Game1.player.team.specialRulesRemovedToday.Add(str2);
      }
    }
  }

  public virtual void AddSpecialRule(string rule)
  {
    switch (rule)
    {
      case "MINE_HARD":
        ++Game1.netWorldState.Value.MinesDifficulty;
        Game1.player.team.kickOutOfMinesEvent.Fire(120);
        Game1.netWorldState.Value.LowestMineLevelForOrder = 0;
        break;
      case "SC_HARD":
        ++Game1.netWorldState.Value.SkullCavesDifficulty;
        Game1.player.team.kickOutOfMinesEvent.Fire(121);
        break;
    }
  }

  public static void RemoveSpecialRuleAtEndOfDay(string rule)
  {
    switch (rule)
    {
      case "MINE_HARD":
        if (Game1.netWorldState.Value.MinesDifficulty > 0)
          --Game1.netWorldState.Value.MinesDifficulty;
        Game1.netWorldState.Value.LowestMineLevelForOrder = -1;
        break;
      case "SC_HARD":
        if (Game1.netWorldState.Value.SkullCavesDifficulty <= 0)
          break;
        --Game1.netWorldState.Value.SkullCavesDifficulty;
        break;
      case "QI_COOKING":
        Utility.ForEachItem((Func<Item, bool>) (item =>
        {
          if (item is StardewValley.Object object2 && object2.orderData.Value == "QI_COOKING")
          {
            object2.orderData.Value = (string) null;
            object2.MarkContextTagsDirty();
          }
          return true;
        }));
        break;
    }
  }

  public virtual void RemoveSpecialRule(string rule)
  {
    if (!(rule == "QI_BEANS"))
      return;
    Game1.player.team.itemsToRemoveOvernight.Add("890");
    Game1.player.team.itemsToRemoveOvernight.Add("889");
  }

  public virtual bool HasMoneyReward()
  {
    return this.questState.Value == SpecialOrderStatus.Complete && this.GetMoneyReward() > 0 && this.participants.ContainsKey(Game1.player.UniqueMultiplayerID);
  }

  public virtual void Fail()
  {
  }

  public virtual void AddObjective(OrderObjective objective) => this.objectives.Add(objective);

  public void CheckCompletion()
  {
    if (this.questState.Value != SpecialOrderStatus.InProgress)
      return;
    foreach (OrderObjective objective in this.objectives)
    {
      if (objective.failOnCompletion.Value && objective.IsComplete())
      {
        this.OnFail();
        return;
      }
    }
    foreach (OrderObjective objective in this.objectives)
    {
      if (!objective.failOnCompletion.Value && !objective.IsComplete())
        return;
    }
    if (!Game1.IsMasterGame)
      return;
    foreach (long key in this.participants.Keys)
      this.unclaimedRewards.TryAdd(key, true);
    Game1.multiplayer.globalChatInfoMessage("CompletedSpecialOrder", TokenStringBuilder.SpecialOrderName(this.questKey.Value));
    this.HostHandleQuestEnd();
    Game1.player.team.completedSpecialOrders.Add(this.questKey.Value);
    this.questState.Value = SpecialOrderStatus.Complete;
    this._RemoveSpecialRuleIfNecessary();
  }

  public override string ToString()
  {
    string str = "";
    foreach (OrderObjective objective in this.objectives)
    {
      str += objective.description.Value;
      if (objective.GetMaxCount() > 1)
      {
        string[] strArray = new string[6]
        {
          str,
          " (",
          null,
          null,
          null,
          null
        };
        int num = objective.GetCount();
        strArray[2] = num.ToString();
        strArray[3] = "/";
        num = objective.GetMaxCount();
        strArray[4] = num.ToString();
        strArray[5] = ")";
        str = string.Concat(strArray);
      }
      str += "\n";
    }
    return str.Trim();
  }

  public string GetName()
  {
    if (this._localizedName == null)
      this._localizedName = SpecialOrder.MakeLocalizationReplacements(this.questName.Value);
    return this._localizedName;
  }

  public string GetDescription()
  {
    if (this._localizedDescription == null)
      this._localizedDescription = this.Parse(this.questDescription.Value).Trim();
    return this._localizedDescription;
  }

  public List<string> GetObjectiveDescriptions()
  {
    List<string> objectiveDescriptions = new List<string>();
    foreach (OrderObjective objective in this.objectives)
      objectiveDescriptions.Add(this.Parse(objective.GetDescription()));
    return objectiveDescriptions;
  }

  public bool CanBeCancelled() => false;

  public void MarkAsViewed()
  {
    this.seenParticipants.TryAdd(Game1.player.UniqueMultiplayerID, true);
  }

  public bool IsHidden() => !this.participants.ContainsKey(Game1.player.UniqueMultiplayerID);

  public bool ShouldDisplayAsNew()
  {
    return !this.seenParticipants.ContainsKey(Game1.player.UniqueMultiplayerID);
  }

  public bool HasReward() => this.HasMoneyReward();

  public int GetMoneyReward()
  {
    if (this._moneyReward == -1)
    {
      this._moneyReward = 0;
      foreach (OrderReward reward in this.rewards)
      {
        if (reward is MoneyReward moneyReward)
          this._moneyReward += moneyReward.GetRewardMoneyAmount();
      }
    }
    return this._moneyReward;
  }

  public bool ShouldDisplayAsComplete() => this.questState.Value != 0;

  public bool IsTimedQuest() => true;

  public int GetDaysLeft()
  {
    return this.questState.Value != SpecialOrderStatus.InProgress ? 0 : this.dueDate.Value - Game1.Date.TotalDays;
  }

  public void OnMoneyRewardClaimed()
  {
    this.participants.Remove(Game1.player.UniqueMultiplayerID);
    this.MarkForRemovalIfEmpty();
  }

  public bool OnLeaveQuestPage()
  {
    if (this.participants.ContainsKey(Game1.player.UniqueMultiplayerID))
      return false;
    this.MarkForRemovalIfEmpty();
    return true;
  }
}
