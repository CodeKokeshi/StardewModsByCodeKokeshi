// Decompiled with JetBrains decompiler
// Type: StardewValley.Quests.ItemDeliveryQuest
// Assembly: Stardew Valley, Version=1.6.15.24356, Culture=neutral, PublicKeyToken=null
// MVID: AA1F513A-94F0-4EF6-A35F-2D5B4A3721BF
// Assembly location: D:\SteamLibrary\steamapps\common\Stardew Valley\Stardew Valley.dll
// XML documentation location: D:\SteamLibrary\steamapps\common\Stardew Valley\Stardew Valley.xml

using Netcode;
using StardewValley.Extensions;
using StardewValley.GameData.Characters;
using StardewValley.Network;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml.Serialization;

#nullable enable
namespace StardewValley.Quests;

public class ItemDeliveryQuest : Quest
{
  /// <summary>The translated NPC dialogue shown when the quest is completed.</summary>
  public 
  #nullable disable
  string targetMessage;
  /// <summary>The internal name for the NPC who gave the quest.</summary>
  [XmlElement("target")]
  public readonly NetString target = new NetString();
  /// <summary>The qualified item ID that must be delivered.</summary>
  [XmlElement("item")]
  public readonly NetString ItemId = new NetString();
  /// <summary>The number of items that must be delivered.</summary>
  [XmlElement("number")]
  public readonly NetInt number = new NetInt(1);
  /// <summary>The translatable text segments for the quest description shown in the quest log.</summary>
  public readonly NetDescriptionElementList parts = new NetDescriptionElementList();
  /// <summary>The translatable text segments for the <see cref="F:StardewValley.Quests.ItemDeliveryQuest.targetMessage" />.</summary>
  public readonly NetDescriptionElementList dialogueparts = new NetDescriptionElementList();
  /// <summary>The translatable text segments for the objective shown in the quest log (like "0/5 caught").</summary>
  [XmlElement("objective")]
  public readonly NetDescriptionElementRef objective = new NetDescriptionElementRef();

  /// <summary>Construct an instance.</summary>
  public ItemDeliveryQuest() => this.questType.Value = 3;

  /// <summary>Construct an instance.</summary>
  /// <param name="target">The internal name for the NPC who gave the quest.</param>
  /// <param name="itemId">The qualified or unqualified item ID that must be delivered.</param>
  public ItemDeliveryQuest(string target, string itemId)
    : this()
  {
    this.target.Value = target;
    this.ItemId.Value = ItemRegistry.QualifyItemId(itemId) ?? itemId;
  }

  /// <summary>Construct an instance.</summary>
  /// <param name="target">The internal name for the NPC who gave the quest.</param>
  /// <param name="itemId">The qualified or unqualified item ID that must be delivered.</param>
  /// <param name="objective">The translatable text segments for the objective shown in the quest log (like "0/5 caught").</param>
  /// <param name="returnDialogue">The translated NPC dialogue shown when the quest is completed.</param>
  public ItemDeliveryQuest(
    string target,
    string itemId,
    string questTitle,
    string questDescription,
    string objective,
    string returnDialogue)
    : this(target, itemId)
  {
    this.questDescription = questDescription;
    this.questTitle = questTitle;
    this._loadedTitle = true;
    this.targetMessage = returnDialogue;
    this.objective = new NetDescriptionElementRef(new DescriptionElement(objective, Array.Empty<object>()));
  }

  /// <inheritdoc />
  protected override void initNetFields()
  {
    base.initNetFields();
    this.NetFields.AddField((INetSerializable) this.target, "target").AddField((INetSerializable) this.ItemId, "ItemId").AddField((INetSerializable) this.number, "number").AddField((INetSerializable) this.parts, "parts").AddField((INetSerializable) this.dialogueparts, "dialogueparts").AddField((INetSerializable) this.objective, "objective");
  }

  public List<NPC> GetValidTargetList()
  {
    Farmer[] array = Game1.getAllFarmers().ToArray<Farmer>();
    HashSet<string> stringSet1 = new HashSet<string>(((IEnumerable<Farmer>) array).SelectMany<Farmer, string>((Func<Farmer, IEnumerable<string>>) (player => (IEnumerable<string>) player.friendshipData.Keys)));
    HashSet<string> stringSet2 = new HashSet<string>(((IEnumerable<Farmer>) array).Select<Farmer, string>((Func<Farmer, string>) (p => p.spouse)));
    List<NPC> validTargetList = new List<NPC>();
    foreach (KeyValuePair<string, CharacterData> keyValuePair in (IEnumerable<KeyValuePair<string, CharacterData>>) Game1.characterData)
    {
      CharacterData characterData = keyValuePair.Value;
      if ((!GameStateQuery.CheckConditions(characterData.CanSocialize) ? 0 : (characterData.ItemDeliveryQuests != null ? (GameStateQuery.CheckConditions(characterData.ItemDeliveryQuests) ? 1 : 0) : (characterData.HomeRegion == "Town" ? 1 : 0))) != 0 && stringSet1.Contains(keyValuePair.Key) && !stringSet2.Contains(keyValuePair.Key) && keyValuePair.Value.Age != NpcAge.Child)
      {
        NPC characterFromName = Game1.getCharacterFromName(keyValuePair.Key);
        if (characterFromName != null && !characterFromName.IsInvisible)
          validTargetList.Add(characterFromName);
      }
    }
    return validTargetList;
  }

  public void loadQuestInfo()
  {
    if (this.target.Value != null)
      return;
    Random initializationRandom = this.CreateInitializationRandom();
    List<NPC> validTargetList = this.GetValidTargetList();
    NetStringDictionary<Friendship, NetRef<Friendship>> friendshipData = Game1.player.friendshipData;
    if ((friendshipData != null ? (friendshipData.Length > 0 ? 1 : 0) : 0) == 0 || validTargetList.Count <= 0)
      return;
    NPC characterFromName = validTargetList[initializationRandom.Next(validTargetList.Count)];
    if (characterFromName == null)
      return;
    this.target.Value = characterFromName.name.Value;
    if (this.target.Value.Equals("Wizard") && !Game1.player.mailReceived.Contains("wizardJunimoNote") && !Game1.player.mailReceived.Contains("JojaMember"))
    {
      this.target.Value = "Demetrius";
      characterFromName = Game1.getCharacterFromName(this.target.Value);
    }
    this.questTitle = Game1.content.LoadString("Strings\\1_6_Strings:ItemDeliveryQuestTitle", (object) NPC.GetDisplayName(this.target.Value));
    Item obj;
    if (Game1.season != Season.Winter && initializationRandom.NextDouble() < 0.15)
    {
      this.ItemId.Value = initializationRandom.ChooseFrom<string>((IList<string>) Utility.possibleCropsAtThisTime(Game1.season, Game1.dayOfMonth <= 7));
      this.ItemId.Value = ItemRegistry.QualifyItemId(this.ItemId.Value) ?? this.ItemId.Value;
      obj = ItemRegistry.Create(this.ItemId.Value);
      if (this.dailyQuest.Value || this.moneyReward.Value == 0)
        this.moneyReward.Value = this.GetGoldRewardPerItem(obj);
      switch (this.target.Value)
      {
        case "Demetrius":
          this.parts.Clear();
          this.parts.Add(new DescriptionElement("Strings\\StringsFromCSFiles:ItemDeliveryQuest.cs." + initializationRandom.Choose<string>("13311", "13314"), new object[1]
          {
            (object) obj
          }));
          break;
        case "Marnie":
          this.parts.Clear();
          this.parts.Add(new DescriptionElement("Strings\\StringsFromCSFiles:ItemDeliveryQuest.cs." + initializationRandom.Choose<string>("13317", "13320"), new object[1]
          {
            (object) obj
          }));
          break;
        case "Sebastian":
          this.parts.Clear();
          this.parts.Add(new DescriptionElement("Strings\\StringsFromCSFiles:ItemDeliveryQuest.cs." + initializationRandom.Choose<string>("13324", "13327"), new object[1]
          {
            (object) obj
          }));
          break;
        default:
          this.parts.Clear();
          this.parts.Add("Strings\\StringsFromCSFiles:ItemDeliveryQuest.cs." + initializationRandom.Choose<string>("13299", "13300", "13301"));
          this.parts.Add(new DescriptionElement("Strings\\StringsFromCSFiles:ItemDeliveryQuest.cs." + initializationRandom.Choose<string>("13302", "13303", "13304"), new object[1]
          {
            (object) obj
          }));
          this.parts.Add(initializationRandom.Choose<string>("Strings\\StringsFromCSFiles:ItemDeliveryQuest.cs.13306", "Strings\\StringsFromCSFiles:ItemDeliveryQuest.cs.13307", "", "Strings\\StringsFromCSFiles:ItemDeliveryQuest.cs.13308"));
          this.parts.Add(new DescriptionElement("Strings\\StringsFromCSFiles:ItemDeliveryQuest.cs.13620", new object[1]
          {
            (object) characterFromName
          }));
          break;
      }
    }
    else
    {
      string randomItemFromSeason = Utility.getRandomItemFromSeason(Game1.season, 1000, true);
      switch (randomItemFromSeason)
      {
        case "-5":
          this.ItemId.Value = "(O)176";
          break;
        case "-6":
          this.ItemId.Value = "(O)184";
          break;
        default:
          this.ItemId.Value = ItemRegistry.QualifyItemId(randomItemFromSeason) ?? randomItemFromSeason;
          break;
      }
      obj = ItemRegistry.Create(this.ItemId.Value);
      if (this.dailyQuest.Value || this.moneyReward.Value == 0)
        this.moneyReward.Value = this.GetGoldRewardPerItem(obj);
      DescriptionElement[] descriptionElementArray1 = (DescriptionElement[]) null;
      DescriptionElement[] descriptionElementArray2 = (DescriptionElement[]) null;
      DescriptionElement[] descriptionElementArray3 = (DescriptionElement[]) null;
      if ((obj is StardewValley.Object object3 ? object3.Type : (string) null) == "Cooking" && this.target.Value != "Wizard")
      {
        if (initializationRandom.NextDouble() < 0.33)
        {
          DescriptionElement[] descriptionElementArray4 = new DescriptionElement[12]
          {
            new DescriptionElement("Strings\\StringsFromCSFiles:ItemDeliveryQuest.cs.13336", Array.Empty<object>()),
            new DescriptionElement("Strings\\StringsFromCSFiles:ItemDeliveryQuest.cs.13337", Array.Empty<object>()),
            new DescriptionElement("Strings\\StringsFromCSFiles:ItemDeliveryQuest.cs.13338", Array.Empty<object>()),
            new DescriptionElement("Strings\\StringsFromCSFiles:ItemDeliveryQuest.cs.13339", Array.Empty<object>()),
            new DescriptionElement("Strings\\StringsFromCSFiles:ItemDeliveryQuest.cs.13340", Array.Empty<object>()),
            new DescriptionElement("Strings\\StringsFromCSFiles:ItemDeliveryQuest.cs.13341", Array.Empty<object>()),
            null,
            null,
            null,
            null,
            null,
            null
          };
          DescriptionElement descriptionElement1;
          if (!(Game1.samBandName == Game1.content.LoadString("Strings\\StringsFromCSFiles:Game1.cs.2156")))
            descriptionElement1 = new DescriptionElement("Strings\\StringsFromCSFiles:ItemDeliveryQuest.cs.13347", new object[1]
            {
              (object) new DescriptionElement("Strings\\StringsFromCSFiles:Game1.cs.2156", Array.Empty<object>())
            });
          else if (!(Game1.elliottBookName != Game1.content.LoadString("Strings\\StringsFromCSFiles:Game1.cs.2157")))
            descriptionElement1 = new DescriptionElement("Strings\\StringsFromCSFiles:ItemDeliveryQuest.cs.13346", Array.Empty<object>());
          else
            descriptionElement1 = new DescriptionElement("Strings\\StringsFromCSFiles:ItemDeliveryQuest.cs.13342", new object[1]
            {
              (object) new DescriptionElement("Strings\\StringsFromCSFiles:Game1.cs.2157", Array.Empty<object>())
            });
          descriptionElementArray4[6] = descriptionElement1;
          descriptionElementArray4[7] = new DescriptionElement("Strings\\StringsFromCSFiles:ItemDeliveryQuest.cs.13349", Array.Empty<object>());
          descriptionElementArray4[8] = new DescriptionElement("Strings\\StringsFromCSFiles:ItemDeliveryQuest.cs.13350", Array.Empty<object>());
          descriptionElementArray4[9] = new DescriptionElement("Strings\\StringsFromCSFiles:ItemDeliveryQuest.cs.13351", Array.Empty<object>());
          DescriptionElement descriptionElement2;
          switch (Game1.season)
          {
            case Season.Summer:
              descriptionElement2 = new DescriptionElement("Strings\\StringsFromCSFiles:ItemDeliveryQuest.cs.13355", Array.Empty<object>());
              break;
            case Season.Winter:
              descriptionElement2 = new DescriptionElement("Strings\\StringsFromCSFiles:ItemDeliveryQuest.cs.13353", Array.Empty<object>());
              break;
            default:
              descriptionElement2 = new DescriptionElement("Strings\\StringsFromCSFiles:ItemDeliveryQuest.cs.13356", Array.Empty<object>());
              break;
          }
          descriptionElementArray4[10] = descriptionElement2;
          descriptionElementArray4[11] = new DescriptionElement("Strings\\StringsFromCSFiles:ItemDeliveryQuest.cs.13357", Array.Empty<object>());
          DescriptionElement[] options = descriptionElementArray4;
          this.parts.Clear();
          this.parts.Add(new DescriptionElement("Strings\\StringsFromCSFiles:ItemDeliveryQuest.cs." + initializationRandom.Choose<string>("13333", "13334"), new object[2]
          {
            (object) obj,
            (object) initializationRandom.ChooseFrom<DescriptionElement>((IList<DescriptionElement>) options)
          }));
          this.parts.Add(new DescriptionElement("Strings\\StringsFromCSFiles:ItemDeliveryQuest.cs.13620", new object[1]
          {
            (object) characterFromName
          }));
        }
        else
        {
          DescriptionElement descriptionElement;
          switch (Game1.dayOfMonth % 7)
          {
            case 0:
              descriptionElement = new DescriptionElement("Strings\\StringsFromCSFiles:Game1.cs.3042", Array.Empty<object>());
              break;
            case 1:
              descriptionElement = new DescriptionElement("Strings\\StringsFromCSFiles:Game1.cs.3043", Array.Empty<object>());
              break;
            case 2:
              descriptionElement = new DescriptionElement("Strings\\StringsFromCSFiles:Game1.cs.3044", Array.Empty<object>());
              break;
            case 3:
              descriptionElement = new DescriptionElement("Strings\\StringsFromCSFiles:Game1.cs.3045", Array.Empty<object>());
              break;
            case 4:
              descriptionElement = new DescriptionElement("Strings\\StringsFromCSFiles:Game1.cs.3046", Array.Empty<object>());
              break;
            case 5:
              descriptionElement = new DescriptionElement("Strings\\StringsFromCSFiles:Game1.cs.3047", Array.Empty<object>());
              break;
            default:
              descriptionElement = new DescriptionElement("Strings\\StringsFromCSFiles:Game1.cs.3048", Array.Empty<object>());
              break;
          }
          descriptionElementArray1 = new DescriptionElement[5]
          {
            new DescriptionElement("Strings\\StringsFromCSFiles:ItemDeliveryQuest.cs.13360", new object[1]
            {
              (object) obj
            }),
            new DescriptionElement("Strings\\StringsFromCSFiles:ItemDeliveryQuest.cs.13364", new object[1]
            {
              (object) obj
            }),
            new DescriptionElement("Strings\\StringsFromCSFiles:ItemDeliveryQuest.cs.13367", new object[1]
            {
              (object) obj
            }),
            new DescriptionElement("Strings\\StringsFromCSFiles:ItemDeliveryQuest.cs.13370", new object[1]
            {
              (object) obj
            }),
            new DescriptionElement("Strings\\StringsFromCSFiles:ItemDeliveryQuest.cs.13373", new object[3]
            {
              (object) descriptionElement,
              (object) obj,
              (object) characterFromName
            })
          };
          descriptionElementArray2 = new DescriptionElement[5]
          {
            new DescriptionElement("Strings\\StringsFromCSFiles:ItemDeliveryQuest.cs.13620", new object[1]
            {
              (object) characterFromName
            }),
            new DescriptionElement("Strings\\StringsFromCSFiles:ItemDeliveryQuest.cs.13620", new object[1]
            {
              (object) characterFromName
            }),
            new DescriptionElement("Strings\\StringsFromCSFiles:ItemDeliveryQuest.cs.13620", new object[1]
            {
              (object) characterFromName
            }),
            new DescriptionElement("Strings\\StringsFromCSFiles:ItemDeliveryQuest.cs.13620", new object[1]
            {
              (object) characterFromName
            }),
            new DescriptionElement("", Array.Empty<object>())
          };
          descriptionElementArray3 = new DescriptionElement[5]
          {
            new DescriptionElement("", Array.Empty<object>()),
            new DescriptionElement("", Array.Empty<object>()),
            new DescriptionElement("", Array.Empty<object>()),
            new DescriptionElement("", Array.Empty<object>()),
            new DescriptionElement("", Array.Empty<object>())
          };
        }
        this.parts.Clear();
        int index = initializationRandom.Next(descriptionElementArray1.Length);
        this.parts.Add(descriptionElementArray1[index]);
        this.parts.Add(descriptionElementArray2[index]);
        this.parts.Add(descriptionElementArray3[index]);
        if (this.target.Value.Equals("Sebastian"))
        {
          this.parts.Clear();
          this.parts.Add(new DescriptionElement("Strings\\StringsFromCSFiles:ItemDeliveryQuest.cs." + initializationRandom.Choose<string>("13378", "13381"), new object[1]
          {
            (object) obj
          }));
        }
      }
      else if (initializationRandom.NextBool() && (obj is StardewValley.Object object2 ? (object2.Edibility > 0 ? 1 : 0) : 0) != 0)
      {
        DescriptionElement[] descriptionElementArray5 = new DescriptionElement[1]
        {
          new DescriptionElement("Strings\\StringsFromCSFiles:ItemDeliveryQuest.cs.13383", new object[3]
          {
            (object) obj,
            (object) new DescriptionElement("Strings\\StringsFromCSFiles:ItemDeliveryQuest.cs." + initializationRandom.Choose<string>("13385", "13386", "13387", "13388", "13389", "13390", "13391", "13392", "13393", "13394", "13395", "13396"), Array.Empty<object>()),
            (object) new DescriptionElement("Strings\\StringsFromCSFiles:ItemDeliveryQuest.cs.13400", new object[1]
            {
              (object) obj
            })
          })
        };
        DescriptionElement[] descriptionElementArray6 = new DescriptionElement[2]
        {
          new DescriptionElement(initializationRandom.Choose<string>("", "Strings\\StringsFromCSFiles:ItemDeliveryQuest.cs.13398"), Array.Empty<object>()),
          new DescriptionElement(initializationRandom.Choose<string>("", "Strings\\StringsFromCSFiles:ItemDeliveryQuest.cs.13402"), Array.Empty<object>())
        };
        DescriptionElement[] descriptionElementArray7 = new DescriptionElement[2]
        {
          new DescriptionElement("Strings\\StringsFromCSFiles:ItemDeliveryQuest.cs.13620", new object[1]
          {
            (object) characterFromName
          }),
          new DescriptionElement("Strings\\StringsFromCSFiles:ItemDeliveryQuest.cs.13620", new object[1]
          {
            (object) characterFromName
          })
        };
        if (initializationRandom.NextDouble() < 0.33)
        {
          DescriptionElement[] descriptionElementArray8 = new DescriptionElement[12]
          {
            new DescriptionElement("Strings\\StringsFromCSFiles:ItemDeliveryQuest.cs.13336", Array.Empty<object>()),
            new DescriptionElement("Strings\\StringsFromCSFiles:ItemDeliveryQuest.cs.13337", Array.Empty<object>()),
            new DescriptionElement("Strings\\StringsFromCSFiles:ItemDeliveryQuest.cs.13338", Array.Empty<object>()),
            new DescriptionElement("Strings\\StringsFromCSFiles:ItemDeliveryQuest.cs.13339", Array.Empty<object>()),
            new DescriptionElement("Strings\\StringsFromCSFiles:ItemDeliveryQuest.cs.13340", Array.Empty<object>()),
            new DescriptionElement("Strings\\StringsFromCSFiles:ItemDeliveryQuest.cs.13341", Array.Empty<object>()),
            null,
            null,
            null,
            null,
            null,
            null
          };
          DescriptionElement descriptionElement3;
          if (!(Game1.samBandName == Game1.content.LoadString("Strings\\StringsFromCSFiles:Game1.cs.2156")))
            descriptionElement3 = new DescriptionElement("Strings\\StringsFromCSFiles:ItemDeliveryQuest.cs.13347", new object[1]
            {
              (object) new DescriptionElement("Strings\\StringsFromCSFiles:Game1.cs.2156", Array.Empty<object>())
            });
          else if (!(Game1.elliottBookName != Game1.content.LoadString("Strings\\StringsFromCSFiles:Game1.cs.2157")))
            descriptionElement3 = new DescriptionElement("Strings\\StringsFromCSFiles:ItemDeliveryQuest.cs.13346", Array.Empty<object>());
          else
            descriptionElement3 = new DescriptionElement("Strings\\StringsFromCSFiles:ItemDeliveryQuest.cs.13342", new object[1]
            {
              (object) new DescriptionElement("Strings\\StringsFromCSFiles:Game1.cs.2157", Array.Empty<object>())
            });
          descriptionElementArray8[6] = descriptionElement3;
          descriptionElementArray8[7] = new DescriptionElement("Strings\\StringsFromCSFiles:ItemDeliveryQuest.cs.13420", Array.Empty<object>());
          descriptionElementArray8[8] = new DescriptionElement("Strings\\StringsFromCSFiles:ItemDeliveryQuest.cs.13421", Array.Empty<object>());
          descriptionElementArray8[9] = new DescriptionElement("Strings\\StringsFromCSFiles:ItemDeliveryQuest.cs.13422", Array.Empty<object>());
          DescriptionElement descriptionElement4;
          switch (Game1.season)
          {
            case Season.Summer:
              descriptionElement4 = new DescriptionElement("Strings\\StringsFromCSFiles:ItemDeliveryQuest.cs.13426", Array.Empty<object>());
              break;
            case Season.Winter:
              descriptionElement4 = new DescriptionElement("Strings\\StringsFromCSFiles:ItemDeliveryQuest.cs.13424", Array.Empty<object>());
              break;
            default:
              descriptionElement4 = new DescriptionElement("Strings\\StringsFromCSFiles:ItemDeliveryQuest.cs.13427", Array.Empty<object>());
              break;
          }
          descriptionElementArray8[10] = descriptionElement4;
          descriptionElementArray8[11] = new DescriptionElement("Strings\\StringsFromCSFiles:ItemDeliveryQuest.cs.13357", Array.Empty<object>());
          DescriptionElement[] options = descriptionElementArray8;
          this.parts.Clear();
          this.parts.Add(new DescriptionElement("Strings\\StringsFromCSFiles:ItemDeliveryQuest.cs." + initializationRandom.Choose<string>("13333", "13334"), new object[2]
          {
            (object) obj,
            (object) initializationRandom.ChooseFrom<DescriptionElement>((IList<DescriptionElement>) options)
          }));
          this.parts.Add(new DescriptionElement("Strings\\StringsFromCSFiles:ItemDeliveryQuest.cs.13620", new object[1]
          {
            (object) characterFromName
          }));
        }
        else
        {
          this.parts.Clear();
          int index = initializationRandom.Next(descriptionElementArray5.Length);
          this.parts.Add(descriptionElementArray5[index]);
          this.parts.Add(descriptionElementArray6[index]);
          this.parts.Add(descriptionElementArray7[index]);
        }
        switch (this.target.Value)
        {
          case "Demetrius":
            this.parts.Clear();
            this.parts.Add(new DescriptionElement("Strings\\StringsFromCSFiles:ItemDeliveryQuest.cs." + initializationRandom.Choose<string>("13311", "13314"), new object[1]
            {
              (object) obj
            }));
            break;
          case "Marnie":
            this.parts.Clear();
            this.parts.Add(new DescriptionElement("Strings\\StringsFromCSFiles:ItemDeliveryQuest.cs." + initializationRandom.Choose<string>("13317", "13320"), new object[1]
            {
              (object) obj
            }));
            break;
          case "Harvey":
            this.parts.Clear();
            this.parts.Add(new DescriptionElement("Strings\\StringsFromCSFiles:ItemDeliveryQuest.cs.13446", new object[2]
            {
              (object) obj,
              (object) new DescriptionElement("Strings\\StringsFromCSFiles:ItemDeliveryQuest.cs." + initializationRandom.Choose<string>("13448", "13449", "13450", "13451", "13452", "13453", "13454", "13455", "13456", "13457", "13458", "13459"), Array.Empty<object>())
            }));
            break;
          case "Gus":
            if (initializationRandom.NextDouble() < 0.6)
            {
              this.parts.Clear();
              this.parts.Add(new DescriptionElement("Strings\\StringsFromCSFiles:ItemDeliveryQuest.cs.13462", new object[1]
              {
                (object) obj
              }));
              break;
            }
            break;
        }
      }
      else if (initializationRandom.NextBool() && (obj is StardewValley.Object object1 ? (object1.Edibility >= 0 ? 1 : 0) : 0) == 0)
      {
        this.parts.Clear();
        this.parts.Add(new DescriptionElement("Strings\\StringsFromCSFiles:ItemDeliveryQuest.cs.13464", new object[2]
        {
          (object) obj,
          (object) new DescriptionElement("Strings\\StringsFromCSFiles:ItemDeliveryQuest.cs." + initializationRandom.Choose<string>("13465", "13466", "13467", "13468", "13469"), Array.Empty<object>())
        }));
        this.parts.Add(new DescriptionElement("Strings\\StringsFromCSFiles:ItemDeliveryQuest.cs.13620", new object[1]
        {
          (object) characterFromName
        }));
        if (this.target.Value.Equals("Emily"))
        {
          this.parts.Clear();
          this.parts.Add(new DescriptionElement("Strings\\StringsFromCSFiles:ItemDeliveryQuest.cs." + initializationRandom.Choose<string>("13473", "13476"), new object[1]
          {
            (object) obj
          }));
        }
      }
      else
      {
        DescriptionElement[] descriptionElementArray9 = new DescriptionElement[9]
        {
          new DescriptionElement("Strings\\StringsFromCSFiles:ItemDeliveryQuest.cs.13480", new object[2]
          {
            (object) characterFromName,
            (object) obj
          }),
          new DescriptionElement("Strings\\StringsFromCSFiles:ItemDeliveryQuest.cs.13481", new object[1]
          {
            (object) obj
          }),
          new DescriptionElement("Strings\\StringsFromCSFiles:ItemDeliveryQuest.cs.13485", new object[1]
          {
            (object) obj
          }),
          new DescriptionElement("Strings\\StringsFromCSFiles:ItemDeliveryQuest.cs." + initializationRandom.Choose<string>("13491", "13492"), new object[1]
          {
            (object) obj
          }),
          new DescriptionElement("Strings\\StringsFromCSFiles:ItemDeliveryQuest.cs.13494", new object[1]
          {
            (object) obj
          }),
          new DescriptionElement("Strings\\StringsFromCSFiles:ItemDeliveryQuest.cs.13497", new object[1]
          {
            (object) obj
          }),
          new DescriptionElement("Strings\\StringsFromCSFiles:ItemDeliveryQuest.cs.13500", new object[2]
          {
            (object) obj,
            (object) new DescriptionElement("Strings\\StringsFromCSFiles:ItemDeliveryQuest.cs." + initializationRandom.Choose<string>("13502", "13503", "13504", "13505", "13506", "13507", "13508", "13509", "13510", "13511", "13512", "13513"), Array.Empty<object>())
          }),
          new DescriptionElement("Strings\\StringsFromCSFiles:ItemDeliveryQuest.cs.13518", new object[2]
          {
            (object) characterFromName,
            (object) obj
          }),
          new DescriptionElement("Strings\\StringsFromCSFiles:ItemDeliveryQuest.cs." + initializationRandom.Choose<string>("13520", "13523"), new object[1]
          {
            (object) obj
          })
        };
        DescriptionElement[] descriptionElementArray10 = new DescriptionElement[9]
        {
          new DescriptionElement("", Array.Empty<object>()),
          new DescriptionElement(initializationRandom.Choose<string>("Strings\\StringsFromCSFiles:ItemDeliveryQuest.cs.13482", "", "Strings\\StringsFromCSFiles:ItemDeliveryQuest.cs.13483"), Array.Empty<object>()),
          new DescriptionElement(initializationRandom.Choose<string>("Strings\\StringsFromCSFiles:ItemDeliveryQuest.cs.13487", "Strings\\StringsFromCSFiles:ItemDeliveryQuest.cs.13488", "", "Strings\\StringsFromCSFiles:ItemDeliveryQuest.cs.13489"), Array.Empty<object>()),
          new DescriptionElement("Strings\\StringsFromCSFiles:ItemDeliveryQuest.cs.13620", new object[1]
          {
            (object) characterFromName
          }),
          new DescriptionElement("Strings\\StringsFromCSFiles:ItemDeliveryQuest.cs.13620", new object[1]
          {
            (object) characterFromName
          }),
          new DescriptionElement("Strings\\StringsFromCSFiles:ItemDeliveryQuest.cs.13620", new object[1]
          {
            (object) characterFromName
          }),
          new DescriptionElement("Strings\\StringsFromCSFiles:ItemDeliveryQuest.cs." + initializationRandom.Choose<string>("13514", "13516"), Array.Empty<object>()),
          new DescriptionElement("", Array.Empty<object>()),
          new DescriptionElement("Strings\\StringsFromCSFiles:ItemDeliveryQuest.cs.13620", new object[1]
          {
            (object) characterFromName
          })
        };
        DescriptionElement[] descriptionElementArray11 = new DescriptionElement[9]
        {
          new DescriptionElement("", Array.Empty<object>()),
          new DescriptionElement("Strings\\StringsFromCSFiles:ItemDeliveryQuest.cs.13620", new object[1]
          {
            (object) characterFromName
          }),
          new DescriptionElement("Strings\\StringsFromCSFiles:ItemDeliveryQuest.cs.13620", new object[1]
          {
            (object) characterFromName
          }),
          new DescriptionElement("", Array.Empty<object>()),
          new DescriptionElement("", Array.Empty<object>()),
          new DescriptionElement("", Array.Empty<object>()),
          new DescriptionElement("Strings\\StringsFromCSFiles:ItemDeliveryQuest.cs.13620", new object[1]
          {
            (object) characterFromName
          }),
          new DescriptionElement("", Array.Empty<object>()),
          new DescriptionElement("", Array.Empty<object>())
        };
        this.parts.Clear();
        int index = initializationRandom.Next(descriptionElementArray9.Length);
        this.parts.Add(descriptionElementArray9[index]);
        this.parts.Add(descriptionElementArray10[index]);
        this.parts.Add(descriptionElementArray11[index]);
      }
    }
    this.dialogueparts.Clear();
    this.dialogueparts.Add(initializationRandom.NextBool(0.3) || this.target.Value == "Evelyn" ? new DescriptionElement("Strings\\StringsFromCSFiles:ItemDeliveryQuest.cs.13526", Array.Empty<object>()) : new DescriptionElement("Strings\\StringsFromCSFiles:ItemDeliveryQuest.cs." + initializationRandom.Choose<string>("13527", "13528"), Array.Empty<object>()));
    NetDescriptionElementList dialogueparts = this.dialogueparts;
    DescriptionElement descriptionElement5;
    if (!initializationRandom.NextBool(0.3))
      descriptionElement5 = initializationRandom.NextBool() ? new DescriptionElement("Strings\\StringsFromCSFiles:ItemDeliveryQuest.cs.13532", Array.Empty<object>()) : new DescriptionElement("Strings\\StringsFromCSFiles:ItemDeliveryQuest.cs." + initializationRandom.Choose<string>("13534", "13535", "13536"), Array.Empty<object>());
    else
      descriptionElement5 = new DescriptionElement("Strings\\StringsFromCSFiles:ItemDeliveryQuest.cs.13530", new object[1]
      {
        (object) obj
      });
    dialogueparts.Add(descriptionElement5);
    this.dialogueparts.Add("Strings\\StringsFromCSFiles:ItemDeliveryQuest.cs." + initializationRandom.Choose<string>("13538", "13539", "13540"));
    this.dialogueparts.Add("Strings\\StringsFromCSFiles:ItemDeliveryQuest.cs." + initializationRandom.Choose<string>("13542", "13543", "13544"));
    string str = this.target.Value;
    if (str != null)
    {
      switch (str.Length)
      {
        case 3:
          if (str == "Sam")
          {
            this.parts.Clear();
            this.parts.Add(new DescriptionElement("Strings\\StringsFromCSFiles:ItemDeliveryQuest.cs." + initializationRandom.Choose<string>("13568", "13571"), new object[1]
            {
              (object) obj
            }));
            this.dialogueparts.Clear();
            this.dialogueparts.Add(new DescriptionElement("Strings\\StringsFromCSFiles:ItemDeliveryQuest.cs.13577", Array.Empty<object>()));
            break;
          }
          break;
        case 4:
          if (str == "Maru")
          {
            bool flag = initializationRandom.NextBool();
            this.parts.Clear();
            this.parts.Add(new DescriptionElement("Strings\\StringsFromCSFiles:ItemDeliveryQuest.cs." + (flag ? "13580" : "13583"), new object[1]
            {
              (object) obj
            }));
            this.dialogueparts.Clear();
            this.dialogueparts.Add(new DescriptionElement("Strings\\StringsFromCSFiles:ItemDeliveryQuest.cs." + (flag ? "13585" : "13587"), Array.Empty<object>()));
            break;
          }
          break;
        case 5:
          if (str == "Haley")
          {
            this.parts.Clear();
            this.parts.Add(new DescriptionElement("Strings\\StringsFromCSFiles:ItemDeliveryQuest.cs." + initializationRandom.Choose<string>("13557", "13560"), new object[1]
            {
              (object) obj
            }));
            this.dialogueparts.Clear();
            this.dialogueparts.Add("Strings\\StringsFromCSFiles:ItemDeliveryQuest.cs.13566");
            break;
          }
          break;
        case 6:
          if (str == "Wizard")
          {
            this.parts.Clear();
            this.parts.Add(new DescriptionElement("Strings\\StringsFromCSFiles:ItemDeliveryQuest.cs." + initializationRandom.Choose<string>("13546", "13548", "13551", "13553"), new object[1]
            {
              (object) obj
            }));
            this.dialogueparts.Clear();
            this.dialogueparts.Add("Strings\\StringsFromCSFiles:ItemDeliveryQuest.cs.13555");
            break;
          }
          break;
        case 7:
          switch (str[0])
          {
            case 'A':
              if (str == "Abigail")
              {
                bool flag = initializationRandom.NextBool();
                this.parts.Clear();
                this.parts.Add(new DescriptionElement("Strings\\StringsFromCSFiles:ItemDeliveryQuest.cs." + (flag ? "13590" : "13593"), new object[1]
                {
                  (object) obj
                }));
                this.dialogueparts.Clear();
                this.dialogueparts.Add(new DescriptionElement("Strings\\StringsFromCSFiles:ItemDeliveryQuest.cs." + (flag ? "13597" : "13599"), Array.Empty<object>()));
                break;
              }
              break;
            case 'E':
              if (str == "Elliott")
              {
                this.dialogueparts.Clear();
                this.dialogueparts.Add(new DescriptionElement("Strings\\StringsFromCSFiles:ItemDeliveryQuest.cs.13604", new object[1]
                {
                  (object) obj
                }));
                break;
              }
              break;
          }
          break;
        case 9:
          if (str == "Sebastian")
          {
            this.dialogueparts.Clear();
            this.dialogueparts.Add("Strings\\StringsFromCSFiles:ItemDeliveryQuest.cs.13602");
            break;
          }
          break;
      }
    }
    DescriptionElement descriptionElement6 = new DescriptionElement("Strings\\StringsFromCSFiles:ItemDeliveryQuest.cs." + initializationRandom.Choose<string>("13608", "13610", "13612"), new object[1]
    {
      (object) characterFromName
    });
    this.parts.Add(new DescriptionElement("Strings\\StringsFromCSFiles:ItemDeliveryQuest.cs.13607", new object[1]
    {
      (object) this.moneyReward.Value
    }));
    this.parts.Add(descriptionElement6);
    this.objective.Value = new DescriptionElement("Strings\\StringsFromCSFiles:ItemDeliveryQuest.cs.13614", new object[2]
    {
      (object) characterFromName,
      (object) obj
    });
  }

  public override void reloadDescription()
  {
    if (this._questDescription == "")
      this.loadQuestInfo();
    string str1 = "";
    string str2 = "";
    if (this.parts != null && this.parts.Count != 0)
    {
      foreach (DescriptionElement part in (NetList<DescriptionElement, NetDescriptionElementRef>) this.parts)
        str1 += part.loadDescriptionElement();
      this.questDescription = str1;
    }
    if (this.dialogueparts != null && this.dialogueparts.Count != 0)
    {
      foreach (DescriptionElement dialoguepart in (NetList<DescriptionElement, NetDescriptionElementRef>) this.dialogueparts)
        str2 += dialoguepart.loadDescriptionElement();
      this.targetMessage = str2;
    }
    else
    {
      if (!this.HasId())
        return;
      this.targetMessage = ArgUtility.Get(Quest.GetRawQuestFields(this.id.Value), 9, this.targetMessage, false);
    }
  }

  public override void reloadObjective()
  {
    if (this.objective.Value == null)
      return;
    this.currentObjective = this.objective.Value.loadDescriptionElement();
  }

  /// <inheritdoc />
  public override bool OnItemOfferedToNpc(NPC npc, Item item, bool probe = false)
  {
    bool npc1 = base.OnItemOfferedToNpc(npc, item, probe);
    if (this.completed.Value)
      return false;
    if (npc.IsVillager && npc.Name == this.target.Value && item.QualifiedItemId == this.ItemId.Value)
    {
      if (item.Stack >= this.number.Value)
      {
        if (!probe)
        {
          Game1.player.Items.Reduce(item, this.number.Value, false);
          this.reloadDescription();
          npc.CurrentDialogue.Push(new Dialogue(npc, (string) null, this.targetMessage));
          Game1.drawDialogue(npc);
          if (this.dailyQuest.Value)
            Game1.player.changeFriendship(150, npc);
          else
            Game1.player.changeFriendship((int) byte.MaxValue, npc);
          this.questComplete();
        }
        return true;
      }
      if (!probe)
      {
        npc.CurrentDialogue.Push(Dialogue.FromTranslation(npc, "Strings\\StringsFromCSFiles:ItemDeliveryQuest.cs.13615", (object) this.number.Value));
        Game1.drawDialogue(npc);
      }
    }
    return npc1;
  }

  /// <summary>Get the gold reward for a given item.</summary>
  /// <param name="item">The item instance.</param>
  public int GetGoldRewardPerItem(Item item)
  {
    return item is StardewValley.Object @object ? @object.Price * 3 : (int) ((double) item.salePrice(false) * 1.5);
  }
}
