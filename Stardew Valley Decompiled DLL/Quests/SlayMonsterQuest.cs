// Decompiled with JetBrains decompiler
// Type: StardewValley.Quests.SlayMonsterQuest
// Assembly: Stardew Valley, Version=1.6.15.24356, Culture=neutral, PublicKeyToken=null
// MVID: AA1F513A-94F0-4EF6-A35F-2D5B4A3721BF
// Assembly location: D:\SteamLibrary\steamapps\common\Stardew Valley\Stardew Valley.dll
// XML documentation location: D:\SteamLibrary\steamapps\common\Stardew Valley\Stardew Valley.xml

using Microsoft.Xna.Framework;
using Netcode;
using StardewValley.Extensions;
using StardewValley.Monsters;
using System;
using System.Collections.Generic;
using System.Xml.Serialization;

#nullable disable
namespace StardewValley.Quests;

public class SlayMonsterQuest : Quest
{
  public string targetMessage;
  [XmlElement("monsterName")]
  public readonly NetString monsterName = new NetString();
  [XmlElement("target")]
  public readonly NetString target = new NetString();
  [XmlElement("monster")]
  public readonly NetRef<Monster> monster = new NetRef<Monster>();
  [XmlElement("numberToKill")]
  public readonly NetInt numberToKill = new NetInt();
  [XmlElement("reward")]
  public readonly NetInt reward = new NetInt();
  [XmlElement("numberKilled")]
  public readonly NetInt numberKilled = new NetInt();
  public readonly NetDescriptionElementList parts = new NetDescriptionElementList();
  public readonly NetDescriptionElementList dialogueparts = new NetDescriptionElementList();
  [XmlElement("objective")]
  public readonly NetDescriptionElementRef objective = new NetDescriptionElementRef();
  /// <summary>Whether to ignore monsters killed on the farm.</summary>
  [XmlElement("ignoreFarmMonsters")]
  public readonly NetBool ignoreFarmMonsters = new NetBool(true);

  public SlayMonsterQuest() => this.questType.Value = 4;

  /// <inheritdoc />
  protected override void initNetFields()
  {
    base.initNetFields();
    this.NetFields.AddField((INetSerializable) this.parts, "parts").AddField((INetSerializable) this.dialogueparts, "dialogueparts").AddField((INetSerializable) this.objective, "objective").AddField((INetSerializable) this.monsterName, "monsterName").AddField((INetSerializable) this.target, "target").AddField((INetSerializable) this.monster, "monster").AddField((INetSerializable) this.numberToKill, "numberToKill").AddField((INetSerializable) this.reward, "reward").AddField((INetSerializable) this.numberKilled, "numberKilled").AddField((INetSerializable) this.ignoreFarmMonsters, "ignoreFarmMonsters");
  }

  public void loadQuestInfo()
  {
    if (this.target.Value != null && (NetFieldBase<Monster, NetRef<Monster>>) this.monster != (NetRef<Monster>) null)
      return;
    Random initializationRandom = this.CreateInitializationRandom();
    for (int index = 0; index < initializationRandom.Next(1, 100); ++index)
      initializationRandom.Next();
    this.questTitle = Game1.content.LoadString("Strings\\StringsFromCSFiles:SlayMonsterQuest.cs.13696");
    List<string> options = new List<string>();
    int deepestMineLevel = Utility.GetAllPlayerDeepestMineLevel();
    if (deepestMineLevel < 39)
    {
      options.Add("Green Slime");
      if (deepestMineLevel > 10)
        options.Add("Rock Crab");
      if (deepestMineLevel > 30)
        options.Add("Duggy");
    }
    else if (deepestMineLevel < 79)
    {
      options.Add("Frost Jelly");
      if (deepestMineLevel > 70)
        options.Add("Skeleton");
      options.Add("Dust Spirit");
    }
    else
    {
      options.Add("Sludge");
      options.Add("Ghost");
      options.Add("Lava Crab");
      options.Add("Squid Kid");
    }
    int num = this.monsterName.Value == null ? 1 : (this.numberToKill.Value == 0 ? 1 : 0);
    if (num != 0)
      this.monsterName.Value = initializationRandom.ChooseFrom<string>((IList<string>) options);
    if (this.monsterName.Value == "Frost Jelly" || this.monsterName.Value == "Sludge")
    {
      this.monster.Value = new Monster("Green Slime", Vector2.Zero);
      this.monster.Value.Name = this.monsterName.Value;
    }
    else
      this.monster.Value = new Monster(this.monsterName.Value, Vector2.Zero);
    if (num != 0)
    {
      string str = this.monsterName.Value;
      if (str != null)
      {
        switch (str.Length)
        {
          case 5:
            switch (str[0])
            {
              case 'D':
                if (str == "Duggy")
                {
                  this.parts.Clear();
                  this.parts.Add(new DescriptionElement("Strings\\StringsFromCSFiles:SlayMonsterQuest.cs.13711", new object[1]
                  {
                    (object) this.numberToKill.Value
                  }));
                  this.target.Value = "Clint";
                  this.numberToKill.Value = initializationRandom.Next(2, 4);
                  this.reward.Value = this.numberToKill.Value * 150;
                  goto label_46;
                }
                break;
              case 'G':
                if (str == "Ghost")
                {
                  this.numberToKill.Value = initializationRandom.Next(2, 4);
                  this.reward.Value = this.numberToKill.Value * 250;
                  goto label_46;
                }
                break;
            }
            break;
          case 6:
            if (str == "Sludge")
            {
              this.numberToKill.Value = initializationRandom.Next(4, 11);
              this.numberToKill.Value -= this.numberToKill.Value % 2;
              this.reward.Value = this.numberToKill.Value * 125;
              goto label_46;
            }
            break;
          case 8:
            if (str == "Skeleton")
            {
              this.numberToKill.Value = initializationRandom.Next(6, 12);
              this.reward.Value = this.numberToKill.Value * 100;
              goto label_46;
            }
            break;
          case 9:
            switch (str[0])
            {
              case 'L':
                if (str == "Lava Crab")
                {
                  this.numberToKill.Value = initializationRandom.Next(2, 6);
                  this.reward.Value = this.numberToKill.Value * 180;
                  goto label_46;
                }
                break;
              case 'R':
                if (str == "Rock Crab")
                {
                  this.numberToKill.Value = initializationRandom.Next(2, 6);
                  this.reward.Value = this.numberToKill.Value * 75;
                  goto label_46;
                }
                break;
              case 'S':
                if (str == "Squid Kid")
                {
                  this.numberToKill.Value = initializationRandom.Next(1, 3);
                  this.reward.Value = this.numberToKill.Value * 350;
                  goto label_46;
                }
                break;
            }
            break;
          case 11:
            switch (str[0])
            {
              case 'D':
                if (str == "Dust Spirit")
                {
                  this.numberToKill.Value = initializationRandom.Next(10, 21);
                  this.reward.Value = this.numberToKill.Value * 60;
                  goto label_46;
                }
                break;
              case 'F':
                if (str == "Frost Jelly")
                {
                  this.numberToKill.Value = initializationRandom.Next(4, 11);
                  this.numberToKill.Value -= this.numberToKill.Value % 2;
                  this.reward.Value = this.numberToKill.Value * 85;
                  goto label_46;
                }
                break;
              case 'G':
                if (str == "Green Slime")
                {
                  this.numberToKill.Value = initializationRandom.Next(4, 11);
                  this.numberToKill.Value -= this.numberToKill.Value % 2;
                  this.reward.Value = this.numberToKill.Value * 60;
                  goto label_46;
                }
                break;
            }
            break;
        }
      }
      this.numberToKill.Value = initializationRandom.Next(3, 7);
      this.reward.Value = this.numberToKill.Value * 120;
    }
label_46:
    switch (this.monsterName.Value)
    {
      case "Green Slime":
      case "Frost Jelly":
      case "Sludge":
        this.parts.Clear();
        this.parts.Add(new DescriptionElement("Strings\\StringsFromCSFiles:SlayMonsterQuest.cs.13723", new object[2]
        {
          (object) this.numberToKill.Value,
          this.monsterName.Value.Equals("Frost Jelly") ? (object) new DescriptionElement("Strings\\StringsFromCSFiles:SlayMonsterQuest.cs.13725", Array.Empty<object>()) : (this.monsterName.Value.Equals("Sludge") ? (object) new DescriptionElement("Strings\\StringsFromCSFiles:SlayMonsterQuest.cs.13727", Array.Empty<object>()) : (object) new DescriptionElement("Strings\\StringsFromCSFiles:SlayMonsterQuest.cs.13728", Array.Empty<object>()))
        }));
        this.target.Value = "Lewis";
        this.dialogueparts.Clear();
        this.dialogueparts.Add("Strings\\StringsFromCSFiles:SlayMonsterQuest.cs.13730");
        if (initializationRandom.NextBool())
        {
          this.dialogueparts.Add("Strings\\StringsFromCSFiles:SlayMonsterQuest.cs.13731");
          this.dialogueparts.Add("Strings\\StringsFromCSFiles:SlayMonsterQuest.cs." + initializationRandom.Choose<string>("13732", "13733"));
          this.dialogueparts.Add(new DescriptionElement("Strings\\StringsFromCSFiles:SlayMonsterQuest.cs.13734", new object[3]
          {
            (object) new DescriptionElement("Strings\\StringsFromCSFiles:SlayMonsterQuest.cs." + initializationRandom.Choose<string>("13735", "13736"), Array.Empty<object>()),
            (object) new DescriptionElement("Strings\\StringsFromCSFiles:Dialogue.cs." + initializationRandom.Choose<string>("795", "796", "797", "798", "799", "800", "801", "802", "803", "804", "805", "806", "807", "808", "809", "810"), Array.Empty<object>()),
            (object) new DescriptionElement("Strings\\StringsFromCSFiles:SlayMonsterQuest.cs." + initializationRandom.Choose<string>("13740", "13741", "13742"), Array.Empty<object>())
          }));
          break;
        }
        this.dialogueparts.Add("Strings\\StringsFromCSFiles:SlayMonsterQuest.cs.13744");
        break;
      case "Rock Crab":
      case "Lava Crab":
        this.parts.Clear();
        this.parts.Add(new DescriptionElement("Strings\\StringsFromCSFiles:SlayMonsterQuest.cs.13747", new object[1]
        {
          (object) this.numberToKill.Value
        }));
        this.target.Value = "Demetrius";
        this.dialogueparts.Clear();
        this.dialogueparts.Add(new DescriptionElement("Strings\\StringsFromCSFiles:SlayMonsterQuest.cs.13750", new object[1]
        {
          (object) this.monster.Value
        }));
        break;
      default:
        this.parts.Clear();
        this.parts.Add(new DescriptionElement("Strings\\StringsFromCSFiles:SlayMonsterQuest.cs.13752", new object[3]
        {
          (object) this.monster.Value,
          (object) this.numberToKill.Value,
          (object) new DescriptionElement("Strings\\StringsFromCSFiles:SlayMonsterQuest.cs." + initializationRandom.Choose<string>("13755", "13756", "13757"), Array.Empty<object>())
        }));
        this.target.Value = "Wizard";
        this.dialogueparts.Clear();
        this.dialogueparts.Add("Strings\\StringsFromCSFiles:SlayMonsterQuest.cs.13760");
        break;
    }
    if (this.target.Value.Equals("Wizard") && !Utility.doesAnyFarmerHaveMail("wizardJunimoNote") && !Utility.doesAnyFarmerHaveMail("JojaMember"))
    {
      this.parts.Clear();
      this.parts.Add(new DescriptionElement("Strings\\StringsFromCSFiles:SlayMonsterQuest.cs.13764", new object[2]
      {
        (object) this.numberToKill.Value,
        (object) this.monster.Value
      }));
      this.target.Value = "Lewis";
      this.dialogueparts.Clear();
      this.dialogueparts.Add("Strings\\StringsFromCSFiles:SlayMonsterQuest.cs.13767");
    }
    this.parts.Add(new DescriptionElement("Strings\\StringsFromCSFiles:FishingQuest.cs.13274", new object[1]
    {
      (object) this.reward.Value
    }));
    this.objective.Value = new DescriptionElement("Strings\\StringsFromCSFiles:SlayMonsterQuest.cs.13770", new object[3]
    {
      (object) "0",
      (object) this.numberToKill.Value,
      (object) this.monster.Value
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
    if (this.numberKilled.Value == 0 && this.HasId())
      return;
    if (this.numberKilled.Value < this.numberToKill.Value)
      this.objective.Value = new DescriptionElement("Strings\\StringsFromCSFiles:SlayMonsterQuest.cs.13770", new object[3]
      {
        (object) this.numberKilled.Value,
        (object) this.numberToKill.Value,
        (object) this.monster.Value
      });
    if (this.objective.Value == null)
      return;
    this.currentObjective = this.objective.Value.loadDescriptionElement();
  }

  private bool isSlimeName(string s)
  {
    return s.Contains("Slime") || s.Contains("Jelly") || s.Contains("Sludge");
  }

  /// <inheritdoc />
  public override bool OnMonsterSlain(
    GameLocation location,
    Monster monster,
    bool killedByBomb,
    bool isTameMonster,
    bool probe = false)
  {
    bool flag = base.OnMonsterSlain(location, monster, killedByBomb, isTameMonster, probe);
    if (this.completed.Value || !monster.Name.Contains(this.monsterName.Value) && (!(this.id.Value == "15") || !this.isSlimeName(monster.Name)) || this.numberKilled.Value >= this.numberToKill.Value)
      return flag;
    if (!probe)
    {
      this.numberKilled.Value = Math.Min(this.numberToKill.Value, this.numberKilled.Value + 1);
      Game1.dayTimeMoneyBox.pingQuest((Quest) this);
      if (this.numberKilled.Value >= this.numberToKill.Value)
      {
        if (this.target.Value == null || this.target.Value.Equals("null"))
        {
          this.questComplete();
        }
        else
        {
          this.objective.Value = new DescriptionElement("Strings\\StringsFromCSFiles:FishingQuest.cs.13277", new object[1]
          {
            (object) Game1.getCharacterFromName(this.target.Value)
          });
          Game1.playSound("jingle1");
        }
      }
      else if (this.monster.Value == null)
      {
        if (this.monsterName.Value == "Frost Jelly" || this.monsterName.Value == "Sludge")
        {
          this.monster.Value = new Monster("Green Slime", Vector2.Zero);
          this.monster.Value.Name = this.monsterName.Value;
        }
        else
          this.monster.Value = new Monster(this.monsterName.Value, Vector2.Zero);
      }
    }
    return true;
  }

  public override bool OnNpcSocialized(NPC npc, bool probe = false)
  {
    bool flag = base.OnNpcSocialized(npc, probe);
    if (this.completed.Value || this.target.Value == null || !(this.target.Value != "null") || this.numberKilled.Value < this.numberToKill.Value || !(npc.Name == this.target.Value) || !npc.IsVillager)
      return flag;
    if (!probe)
    {
      this.reloadDescription();
      npc.CurrentDialogue.Push(new Dialogue(npc, (string) null, this.targetMessage));
      this.moneyReward.Value = this.reward.Value;
      this.questComplete();
      Game1.drawDialogue(npc);
    }
    return true;
  }
}
