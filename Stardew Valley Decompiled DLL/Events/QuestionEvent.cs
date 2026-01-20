// Decompiled with JetBrains decompiler
// Type: StardewValley.Events.QuestionEvent
// Assembly: Stardew Valley, Version=1.6.15.24356, Culture=neutral, PublicKeyToken=null
// MVID: AA1F513A-94F0-4EF6-A35F-2D5B4A3721BF
// Assembly location: D:\SteamLibrary\steamapps\common\Stardew Valley\Stardew Valley.dll
// XML documentation location: D:\SteamLibrary\steamapps\common\Stardew Valley\Stardew Valley.xml

using Microsoft.Xna.Framework;
using StardewValley.Buildings;
using StardewValley.Menus;
using System;

#nullable disable
namespace StardewValley.Events;

public class QuestionEvent : BaseFarmEvent
{
  public const int pregnancyQuestion = 1;
  public const int barnBirth = 2;
  public const int playerPregnancyQuestion = 3;
  private int whichQuestion;
  private AnimalHouse animalHouse;
  public FarmAnimal animal;
  public bool forceProceed;

  public QuestionEvent(int whichQuestion) => this.whichQuestion = whichQuestion;

  /// <inheritdoc />
  public override bool setUp()
  {
    switch (this.whichQuestion)
    {
      case 1:
        Response[] answerChoices1 = new Response[2]
        {
          new Response("Yes", Game1.content.LoadString("Strings\\Events:HaveBabyAnswer_Yes")),
          new Response("Not", Game1.content.LoadString("Strings\\Events:HaveBabyAnswer_No"))
        };
        NPC speaker = Game1.RequireCharacter(Game1.player.spouse);
        string path = !speaker.isAdoptionSpouse() ? "Strings\\Events:HaveBabyQuestion" : "Strings\\Events:HaveBabyQuestion_Adoption";
        Game1.currentLocation.createQuestionDialogue(Game1.content.LoadString(path, (object) Game1.player.Name), answerChoices1, new GameLocation.afterQuestionBehavior(this.answerPregnancyQuestion), speaker);
        Game1.messagePause = true;
        return false;
      case 2:
        FarmAnimal a = (FarmAnimal) null;
        Utility.ForEachBuilding((Func<Building, bool>) (b =>
        {
          if (b.owner.Value != Game1.player.UniqueMultiplayerID && Game1.IsMultiplayer || !b.AllowsAnimalPregnancy() || !(b.GetIndoors() is AnimalHouse indoors2) || indoors2.isFull() || Game1.random.NextDouble() >= (double) indoors2.animalsThatLiveHere.Count * 0.0055)
            return true;
          a = Utility.getAnimal(indoors2.animalsThatLiveHere[Game1.random.Next(indoors2.animalsThatLiveHere.Count)]);
          this.animalHouse = indoors2;
          return false;
        }));
        if (a != null && !a.isBaby() && a.allowReproduction.Value && a.CanHavePregnancy())
        {
          Game1.drawObjectDialogue(Game1.content.LoadString("Strings\\Events:AnimalBirth", (object) a.displayName, (object) a.shortDisplayType()));
          Game1.messagePause = true;
          this.animal = a;
          return false;
        }
        break;
      case 3:
        Response[] answerChoices2 = new Response[2]
        {
          new Response("Yes", Game1.content.LoadString("Strings\\Events:HaveBabyAnswer_Yes")),
          new Response("Not", Game1.content.LoadString("Strings\\Events:HaveBabyAnswer_No"))
        };
        long key = Game1.player.team.GetSpouse(Game1.player.UniqueMultiplayerID).Value;
        Farmer otherFarmer = Game1.otherFarmers[key];
        if (otherFarmer.IsMale != Game1.player.IsMale)
          Game1.currentLocation.createQuestionDialogue(Game1.content.LoadString("Strings\\Events:HavePlayerBabyQuestion", (object) otherFarmer.displayName), answerChoices2, new GameLocation.afterQuestionBehavior(this.answerPlayerPregnancyQuestion));
        else
          Game1.currentLocation.createQuestionDialogue(Game1.content.LoadString("Strings\\Events:HavePlayerBabyQuestion_Adoption", (object) otherFarmer.displayName), answerChoices2, new GameLocation.afterQuestionBehavior(this.answerPlayerPregnancyQuestion));
        Game1.messagePause = true;
        return false;
    }
    return true;
  }

  private void answerPregnancyQuestion(Farmer who, string answer)
  {
    if (!answer.Equals("Yes"))
      return;
    WorldDate worldDate = new WorldDate(Game1.Date);
    worldDate.TotalDays += 14;
    who.GetSpouseFriendship().NextBirthingDate = worldDate;
  }

  private void answerPlayerPregnancyQuestion(Farmer who, string answer)
  {
    if (!answer.Equals("Yes"))
      return;
    long key = Game1.player.team.GetSpouse(Game1.player.UniqueMultiplayerID).Value;
    Game1.player.team.SendProposal(Game1.otherFarmers[key], ProposalType.Baby);
  }

  /// <inheritdoc />
  public override bool tickUpdate(GameTime time)
  {
    if (this.forceProceed)
      return true;
    if (this.whichQuestion != 2 || Game1.dialogueUp)
      return !Game1.dialogueUp;
    if (Game1.activeClickableMenu == null)
      Game1.activeClickableMenu = (IClickableMenu) new NamingMenu(new NamingMenu.doneNamingBehavior(this.animalHouse.addNewHatchedAnimal), this.animal != null ? Game1.content.LoadString("Strings\\Events:AnimalNamingTitle", (object) this.animal.displayType) : Game1.content.LoadString("Strings\\StringsFromCSFiles:QuestionEvent.cs.6692"));
    return false;
  }

  /// <inheritdoc />
  public override void makeChangesToLocation() => Game1.messagePause = false;
}
