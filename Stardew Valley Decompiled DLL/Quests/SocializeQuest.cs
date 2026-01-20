// Decompiled with JetBrains decompiler
// Type: StardewValley.Quests.SocializeQuest
// Assembly: Stardew Valley, Version=1.6.15.24356, Culture=neutral, PublicKeyToken=null
// MVID: AA1F513A-94F0-4EF6-A35F-2D5B4A3721BF
// Assembly location: D:\SteamLibrary\steamapps\common\Stardew Valley\Stardew Valley.dll
// XML documentation location: D:\SteamLibrary\steamapps\common\Stardew Valley\Stardew Valley.xml

using Microsoft.Xna.Framework;
using Netcode;
using StardewValley.Extensions;
using StardewValley.GameData.Characters;
using System;
using System.Collections.Generic;
using System.Xml.Serialization;

#nullable disable
namespace StardewValley.Quests;

public class SocializeQuest : Quest
{
  public readonly NetStringList whoToGreet = new NetStringList();
  [XmlElement("total")]
  public readonly NetInt total = new NetInt();
  public readonly NetDescriptionElementList parts = new NetDescriptionElementList();
  [XmlElement("objective")]
  public readonly NetDescriptionElementRef objective = new NetDescriptionElementRef();

  public SocializeQuest() => this.questType.Value = 5;

  /// <inheritdoc />
  protected override void initNetFields()
  {
    base.initNetFields();
    this.NetFields.AddField((INetSerializable) this.whoToGreet, "whoToGreet").AddField((INetSerializable) this.total, "total").AddField((INetSerializable) this.parts, "parts").AddField((INetSerializable) this.objective, "objective");
  }

  public void loadQuestInfo()
  {
    if (this.whoToGreet.Count > 0)
      return;
    Random initializationRandom = this.CreateInitializationRandom();
    this.questTitle = Game1.content.LoadString("Strings\\StringsFromCSFiles:SocializeQuest.cs.13785");
    this.parts.Clear();
    this.parts.Add(new DescriptionElement("Strings\\StringsFromCSFiles:SocializeQuest.cs.13786", new object[1]
    {
      (object) new DescriptionElement("Strings\\StringsFromCSFiles:SocializeQuest.cs." + initializationRandom.Choose<string>("13787", "13788", "13789"), Array.Empty<object>())
    }));
    this.parts.Add("Strings\\StringsFromCSFiles:SocializeQuest.cs.13791");
    int num = 0;
    foreach (KeyValuePair<string, CharacterData> keyValuePair in (IEnumerable<KeyValuePair<string, CharacterData>>) Game1.characterData)
    {
      string key = keyValuePair.Key;
      CharacterData characterData = keyValuePair.Value;
      if (((int) characterData.IntroductionsQuest ?? (characterData.HomeRegion == "Town" ? 1 : 0)) != 0)
      {
        ++num;
        if (characterData.SocialTab != SocialTabBehavior.AlwaysShown || this.dailyQuest.Value)
          this.whoToGreet.Add(key);
      }
    }
    this.total.Value = num;
    this.objective.Value = new DescriptionElement("Strings\\StringsFromCSFiles:SocializeQuest.cs.13802", new object[2]
    {
      (object) (this.total.Value - this.whoToGreet.Count),
      (object) this.total.Value
    });
  }

  public override void reloadDescription()
  {
    if (this._questDescription == "")
      this.loadQuestInfo();
    if (this.parts.Count == 0 || this.parts == null)
      return;
    string str = "";
    foreach (DescriptionElement part in (NetList<DescriptionElement, NetDescriptionElementRef>) this.parts)
      str += part.loadDescriptionElement();
    this.questDescription = str;
  }

  public override void reloadObjective()
  {
    this.loadQuestInfo();
    if (this.objective.Value == null && this.whoToGreet.Count > 0)
      this.objective.Value = new DescriptionElement("Strings\\StringsFromCSFiles:SocializeQuest.cs.13802", new object[2]
      {
        (object) (this.total.Value - this.whoToGreet.Count),
        (object) this.total.Value
      });
    if (this.objective.Value == null)
      return;
    this.currentObjective = this.objective.Value.loadDescriptionElement();
  }

  /// <inheritdoc />
  public override bool OnNpcSocialized(NPC npc, bool probe = false)
  {
    bool flag = base.OnNpcSocialized(npc, probe);
    this.loadQuestInfo();
    if (this.whoToGreet.Contains(npc.Name))
    {
      if (!probe)
      {
        this.whoToGreet.Remove(npc.Name);
        Game1.dayTimeMoneyBox.moneyDial.animations.Add(new TemporaryAnimatedSprite("LooseSprites\\Cursors", new Rectangle(387, 497, 3, 8), 800f, 1, 0, Game1.dayTimeMoneyBox.position + new Vector2(228f, 244f), false, false, 1f, 0.01f, Color.White, 4f, 0.3f, 0.0f, 0.0f)
        {
          scaleChangeChange = -0.012f
        });
        Game1.dayTimeMoneyBox.pingQuest((Quest) this);
      }
      flag = true;
    }
    if (this.whoToGreet.Count == 0 && !this.completed.Value)
    {
      if (!probe)
      {
        foreach (string key in Game1.player.friendshipData.Keys)
        {
          if (Game1.player.friendshipData[key].Points < 2729)
            Game1.player.changeFriendship(100, Game1.getCharacterFromName(key));
        }
        this.questComplete();
      }
      return true;
    }
    if (!probe)
      this.objective.Value = new DescriptionElement("Strings\\StringsFromCSFiles:SocializeQuest.cs.13802", new object[2]
      {
        (object) (this.total.Value - this.whoToGreet.Count),
        (object) this.total.Value
      });
    return flag;
  }
}
