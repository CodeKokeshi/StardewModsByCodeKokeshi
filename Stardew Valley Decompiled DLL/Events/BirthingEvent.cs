// Decompiled with JetBrains decompiler
// Type: StardewValley.Events.BirthingEvent
// Assembly: Stardew Valley, Version=1.6.15.24356, Culture=neutral, PublicKeyToken=null
// MVID: AA1F513A-94F0-4EF6-A35F-2D5B4A3721BF
// Assembly location: D:\SteamLibrary\steamapps\common\Stardew Valley\Stardew Valley.dll
// XML documentation location: D:\SteamLibrary\steamapps\common\Stardew Valley\Stardew Valley.xml

using Microsoft.Xna.Framework;
using StardewValley.BellsAndWhistles;
using StardewValley.Characters;
using StardewValley.Extensions;
using StardewValley.Menus;
using System;
using System.Collections.Generic;

#nullable disable
namespace StardewValley.Events;

public class BirthingEvent : BaseFarmEvent
{
  private int timer;
  private string soundName;
  private string message;
  private string babyName;
  private bool playedSound;
  private bool isMale;
  private bool getBabyName;
  private bool naming;

  /// <inheritdoc />
  public override bool setUp()
  {
    Random random = Utility.CreateRandom((double) Game1.uniqueIDForThisGame, (double) Game1.stats.DaysPlayed);
    NPC npc = Game1.RequireCharacter(Game1.player.spouse);
    Game1.player.CanMove = false;
    this.isMale = Game1.player.getNumberOfChildren() != 0 ? Game1.player.getChildren()[0].Gender == Gender.Female : random.NextBool();
    this.message = !npc.isAdoptionSpouse() ? (npc.Gender != Gender.Male ? Game1.content.LoadString("Strings\\Events:BirthMessage_SpouseMother", (object) Lexicon.getGenderedChildTerm(this.isMale), (object) npc.displayName) : Game1.content.LoadString("Strings\\Events:BirthMessage_PlayerMother", (object) Lexicon.getGenderedChildTerm(this.isMale))) : Game1.content.LoadString("Strings\\Events:BirthMessage_Adoption", (object) Lexicon.getGenderedChildTerm(this.isMale));
    return false;
  }

  public void returnBabyName(string name)
  {
    this.babyName = name;
    Game1.exitActiveMenu();
  }

  public void afterMessage() => this.getBabyName = true;

  /// <inheritdoc />
  public override bool tickUpdate(GameTime time)
  {
    Game1.player.CanMove = false;
    this.timer += time.ElapsedGameTime.Milliseconds;
    Game1.fadeToBlackAlpha = 1f;
    if (this.timer > 1500 && !this.playedSound && !this.getBabyName)
    {
      if (!string.IsNullOrEmpty(this.soundName))
      {
        Game1.playSound(this.soundName);
        this.playedSound = true;
      }
      if (!this.playedSound && this.message != null && !Game1.dialogueUp && Game1.activeClickableMenu == null)
      {
        Game1.drawObjectDialogue(this.message);
        Game1.afterDialogues = new Game1.afterFadeFunction(this.afterMessage);
      }
    }
    else if (this.getBabyName)
    {
      if (!this.naming)
      {
        Game1.activeClickableMenu = (IClickableMenu) new NamingMenu(new NamingMenu.doneNamingBehavior(this.returnBabyName), Game1.content.LoadString(this.isMale ? "Strings\\Events:BabyNamingTitle_Male" : "Strings\\Events:BabyNamingTitle_Female"), "");
        this.naming = true;
      }
      if (!string.IsNullOrEmpty(this.babyName) && this.babyName.Length > 0)
      {
        NPC spouse = Game1.player.getSpouse();
        double chance = (spouse.hasDarkSkin() ? 0.5 : 0.0) + (Game1.player.hasDarkSkin() ? 0.5 : 0.0);
        bool isDarkSkinned = Utility.CreateRandom((double) Game1.uniqueIDForThisGame, (double) Game1.stats.DaysPlayed).NextBool(chance);
        string babyName = this.babyName;
        List<NPC> allCharacters = Utility.getAllCharacters();
        bool flag;
        do
        {
          flag = false;
          if (Game1.characterData.ContainsKey(babyName))
          {
            babyName += " ";
            flag = true;
          }
          else
          {
            foreach (Character character in allCharacters)
            {
              if (character.Name == babyName)
              {
                babyName += " ";
                flag = true;
              }
            }
          }
        }
        while (flag);
        Child baby = new Child(babyName, this.isMale, isDarkSkinned, Game1.player);
        baby.Age = 0;
        baby.Position = new Vector2(16f, 4f) * 64f + new Vector2(0.0f, -24f);
        Utility.getHomeOfFarmer(Game1.player).characters.Add((NPC) baby);
        Game1.stats.checkForFullHouseAchievement(true);
        Game1.playSound("smallSelect");
        spouse.daysAfterLastBirth = 5;
        Game1.player.GetSpouseFriendship().NextBirthingDate = (WorldDate) null;
        if (Game1.player.getChildrenCount() == 2)
        {
          spouse.shouldSayMarriageDialogue.Value = true;
          spouse.currentMarriageDialogue.Insert(0, new MarriageDialogueReference("Data\\ExtraDialogue", "NewChild_SecondChild" + Game1.random.Next(1, 3).ToString(), true, Array.Empty<string>()));
        }
        else if (spouse.isAdoptionSpouse())
          spouse.currentMarriageDialogue.Insert(0, new MarriageDialogueReference("Data\\ExtraDialogue", "NewChild_Adoption", true, new string[1]
          {
            this.babyName
          }));
        else
          spouse.currentMarriageDialogue.Insert(0, new MarriageDialogueReference("Data\\ExtraDialogue", "NewChild_FirstChild", true, new string[1]
          {
            this.babyName
          }));
        Game1.morningQueue.Enqueue((Action) (() =>
        {
          string str = Game1.getCharacterFromName(Game1.player.spouse)?.GetTokenizedDisplayName() ?? Game1.player.spouse;
          Game1.multiplayer.globalChatInfoMessage("Baby", Lexicon.capitalize(Game1.player.Name), str, Lexicon.getTokenizedGenderedChildTerm(this.isMale), Lexicon.getTokenizedPronoun(this.isMale), baby.displayName);
        }));
        if (Game1.keyboardDispatcher != null)
          Game1.keyboardDispatcher.Subscriber = (IKeyboardSubscriber) null;
        Game1.player.Position = Utility.PointToVector2(Utility.getHomeOfFarmer(Game1.player).GetPlayerBedSpot()) * 64f;
        Game1.globalFadeToClear();
        return true;
      }
    }
    return false;
  }
}
