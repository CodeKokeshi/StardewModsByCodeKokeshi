// Decompiled with JetBrains decompiler
// Type: StardewValley.Locations.AdventureGuild
// Assembly: Stardew Valley, Version=1.6.15.24356, Culture=neutral, PublicKeyToken=null
// MVID: AA1F513A-94F0-4EF6-A35F-2D5B4A3721BF
// Assembly location: D:\SteamLibrary\steamapps\common\Stardew Valley\Stardew Valley.dll
// XML documentation location: D:\SteamLibrary\steamapps\common\Stardew Valley\Stardew Valley.xml

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using StardewValley.GameData;
using StardewValley.Menus;
using StardewValley.TokenizableStrings;
using System;
using System.Collections.Generic;
using System.Text;
using xTile.Dimensions;

#nullable disable
namespace StardewValley.Locations;

public class AdventureGuild : GameLocation
{
  public NPC Gil = new NPC((AnimatedSprite) null, new Vector2(-1000f, -1000f), nameof (AdventureGuild), 2, nameof (Gil), false, Game1.content.Load<Texture2D>("Portraits\\Gil"));
  public bool talkedToGil;

  public AdventureGuild()
  {
  }

  public AdventureGuild(string mapPath, string name)
    : base(mapPath, name)
  {
  }

  public override bool checkAction(Location tileLocation, xTile.Dimensions.Rectangle viewport, Farmer who)
  {
    switch (this.getTileIndexAt(tileLocation, "Buildings", "1"))
    {
      case 1291:
      case 1292:
      case 1355:
      case 1356:
      case 1357:
      case 1358:
        this.gil();
        return true;
      case 1306:
        this.showMonsterKillList();
        return true;
      default:
        return base.checkAction(tileLocation, viewport, who);
    }
  }

  protected override void resetLocalState()
  {
    base.resetLocalState();
    this.talkedToGil = false;
    Game1.player.mailReceived.Add("guildMember");
    this.addOneTimeGiftBox(ItemRegistry.Create("(O)Book_Marlon"), 10, 4);
  }

  public override void draw(SpriteBatch b)
  {
    base.draw(b);
    if (Game1.player.mailReceived.Contains("checkedMonsterBoard"))
      return;
    float num = (float) (4.0 * Math.Round(Math.Sin(Game1.currentGameTime.TotalGameTime.TotalMilliseconds / 250.0), 2));
    b.Draw(Game1.mouseCursors, Game1.GlobalToLocal(Game1.viewport, new Vector2(504f, 464f + num)), new Microsoft.Xna.Framework.Rectangle?(new Microsoft.Xna.Framework.Rectangle(141, 465, 20, 24)), Color.White * 0.75f, 0.0f, Vector2.Zero, 4f, SpriteEffects.None, 0.064801f);
    b.Draw(Game1.mouseCursors, Game1.GlobalToLocal(Game1.viewport, new Vector2(544f, 504f + num)), new Microsoft.Xna.Framework.Rectangle?(new Microsoft.Xna.Framework.Rectangle(175, 425, 12, 12)), Color.White * 0.75f, 0.0f, new Vector2(6f, 6f), 4f, SpriteEffects.None, 0.06481f);
  }

  private string killListLine(string monsterNamePlural, int killCount, int target)
  {
    if (killCount == 0)
      return Game1.content.LoadString("Strings\\Locations:AdventureGuild_KillList_LineFormat_None", (object) killCount, (object) target, (object) monsterNamePlural) + "^";
    return killCount >= target ? Game1.content.LoadString("Strings\\Locations:AdventureGuild_KillList_LineFormat_OverTarget", (object) killCount, (object) target, (object) monsterNamePlural) + "^" : Game1.content.LoadString("Strings\\Locations:AdventureGuild_KillList_LineFormat", (object) killCount, (object) target, (object) monsterNamePlural) + "^";
  }

  public void showMonsterKillList()
  {
    Game1.player.mailReceived.Add("checkedMonsterBoard");
    StringBuilder stringBuilder = new StringBuilder();
    stringBuilder.Append(Game1.content.LoadString("Strings\\Locations:AdventureGuild_KillList_Header").Replace('\n', '^') + "^");
    foreach (MonsterSlayerQuestData monsterSlayerQuestData in DataLoader.MonsterSlayerQuests(Game1.content).Values)
    {
      int killCount = 0;
      if (monsterSlayerQuestData.Targets != null)
      {
        foreach (string target in monsterSlayerQuestData.Targets)
          killCount += Game1.stats.getMonstersKilled(target);
      }
      stringBuilder.Append(this.killListLine(TokenParser.ParseText(monsterSlayerQuestData.DisplayName), killCount, monsterSlayerQuestData.Count));
    }
    stringBuilder.Append(Game1.content.LoadString("Strings\\Locations:AdventureGuild_KillList_Footer").Replace('\n', '^'));
    Game1.drawLetterMessage(stringBuilder.ToString());
  }

  public static bool areAllMonsterSlayerQuestsComplete()
  {
    foreach (MonsterSlayerQuestData monsterSlayerQuestData in DataLoader.MonsterSlayerQuests(Game1.content).Values)
    {
      int num = 0;
      if (monsterSlayerQuestData.Targets != null)
      {
        foreach (string target in monsterSlayerQuestData.Targets)
        {
          num += Game1.stats.getMonstersKilled(target);
          if (num >= monsterSlayerQuestData.Count)
            break;
        }
        if (num < monsterSlayerQuestData.Count)
          return false;
      }
    }
    return true;
  }

  public static bool willThisKillCompleteAMonsterSlayerQuest(string nameOfMonster)
  {
    foreach (MonsterSlayerQuestData monsterSlayerQuestData in DataLoader.MonsterSlayerQuests(Game1.content).Values)
    {
      if (monsterSlayerQuestData.Targets.Contains(nameOfMonster))
      {
        int num = 0;
        if (monsterSlayerQuestData.Targets != null)
        {
          foreach (string target in monsterSlayerQuestData.Targets)
          {
            num += Game1.stats.getMonstersKilled(target);
            if (num >= monsterSlayerQuestData.Count)
              break;
          }
          if (num < monsterSlayerQuestData.Count && num + 1 >= monsterSlayerQuestData.Count)
            return true;
        }
      }
    }
    return false;
  }

  /// <summary>Handle a reward item collected for a completed monster eradication goal.</summary>
  /// <param name="item">The item that was collected.</param>
  /// <param name="who">The player who collected the item.</param>
  /// <param name="completedGoals">The goals for which rewards are being collected.</param>
  public void OnRewardCollected(
    Item item,
    Farmer who,
    List<KeyValuePair<string, MonsterSlayerQuestData>> completedGoals)
  {
    if (item == null)
      return;
    int specialVariable = item.SpecialVariable;
    if (specialVariable < 0 || specialVariable >= completedGoals.Count)
      return;
    KeyValuePair<string, MonsterSlayerQuestData> completedGoal = completedGoals[specialVariable];
    who.mailReceived.Add("Gil_" + completedGoal.Key);
  }

  private void gil()
  {
    List<Item> rewards = new List<Item>();
    List<KeyValuePair<string, MonsterSlayerQuestData>> completedGoals = new List<KeyValuePair<string, MonsterSlayerQuestData>>();
    List<string> values = new List<string>();
    foreach (KeyValuePair<string, MonsterSlayerQuestData> monsterSlayerQuest in DataLoader.MonsterSlayerQuests(Game1.content))
    {
      string key = monsterSlayerQuest.Key;
      MonsterSlayerQuestData goal = monsterSlayerQuest.Value;
      if (!AdventureGuild.HasCollectedReward(Game1.player, key) && AdventureGuild.IsComplete(goal))
      {
        completedGoals.Add(monsterSlayerQuest);
        if (goal.RewardItemId != null)
        {
          Item obj = ItemRegistry.Create(goal.RewardItemId);
          obj.SpecialVariable = completedGoals.Count - 1;
          if (obj is StardewValley.Object @object)
            @object.specialItem = true;
          rewards.Add(obj);
        }
        if (goal.RewardDialogue != null && (goal.RewardDialogueFlag == null || !Game1.player.mailReceived.Contains(goal.RewardDialogueFlag)))
          values.Add(TokenParser.ParseText(goal.RewardDialogue));
        if (goal.RewardMail != null)
          Game1.addMailForTomorrow(goal.RewardMail);
        if (goal.RewardMailAll != null)
          Game1.addMailForTomorrow(goal.RewardMailAll, sendToEveryone: true);
        if (goal.RewardFlag != null)
          Game1.addMail(goal.RewardFlag, true);
        if (goal.RewardFlagAll != null)
          Game1.addMail(goal.RewardFlagAll, true, true);
      }
    }
    if (rewards.Count > 0 || values.Count > 0)
    {
      if (values.Count > 0)
      {
        Game1.DrawDialogue(new Dialogue(this.Gil, (string) null, string.Join("#$b#", (IEnumerable<string>) values)));
        Game1.afterDialogues += (Game1.afterFadeFunction) (() => this.OpenRewardMenuIfNeeded(rewards, completedGoals));
      }
      else
        this.OpenRewardMenuIfNeeded(rewards, completedGoals);
    }
    else
    {
      if (this.talkedToGil)
        Game1.DrawDialogue(this.Gil, "Characters\\Dialogue\\Gil:Snoring");
      else
        Game1.DrawDialogue(this.Gil, "Characters\\Dialogue\\Gil:ComeBackLater");
      this.talkedToGil = true;
    }
  }

  /// <summary>Get whether a player has collected the reward for a monster eradication goal.</summary>
  /// <param name="player">The player to check.</param>
  /// <param name="goalId">The monster eradication goal ID.</param>
  public static bool HasCollectedReward(Farmer player, string goalId)
  {
    return player.mailReceived.Contains("Gil_" + goalId);
  }

  /// <summary>Get whether a monster eradication goal has been completed, regardless of whether the player has collected the rewards yet.</summary>
  /// <param name="goal">The monster eradication goal data.</param>
  public static bool IsComplete(MonsterSlayerQuestData goal)
  {
    if (goal.Targets == null)
      return true;
    int num = 0;
    foreach (string target in goal.Targets)
    {
      num += Game1.stats.getMonstersKilled(target);
      if (num >= goal.Count)
        return true;
    }
    return false;
  }

  /// <summary>Open a menu to collect rewards for completed goals, if any.</summary>
  /// <param name="rewards">The rewards to collect.</param>
  /// <param name="completedGoals">The goals for which rewards are being collected.</param>
  private void OpenRewardMenuIfNeeded(
    List<Item> rewards,
    List<KeyValuePair<string, MonsterSlayerQuestData>> completedGoals)
  {
    if (rewards.Count == 0)
      return;
    Game1.activeClickableMenu = (IClickableMenu) new ItemGrabMenu((IList<Item>) rewards, (object) this)
    {
      behaviorOnItemGrab = (ItemGrabMenu.behaviorOnItemSelect) ((item, who) => this.OnRewardCollected(item, who, completedGoals))
    };
  }
}
