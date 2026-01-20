// Decompiled with JetBrains decompiler
// Type: StardewValley.Locations.JojaMart
// Assembly: Stardew Valley, Version=1.6.15.24356, Culture=neutral, PublicKeyToken=null
// MVID: AA1F513A-94F0-4EF6-A35F-2D5B4A3721BF
// Assembly location: D:\SteamLibrary\steamapps\common\Stardew Valley\Stardew Valley.dll
// XML documentation location: D:\SteamLibrary\steamapps\common\Stardew Valley\Stardew Valley.xml

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using StardewValley.Menus;
using xTile.Dimensions;

#nullable disable
namespace StardewValley.Locations;

public class JojaMart : GameLocation
{
  public const int JojaMembershipPrice = 5000;
  public static NPC Morris;
  private Texture2D communityDevelopmentTexture;

  public JojaMart()
  {
  }

  public JojaMart(string map, string name)
    : base(map, name)
  {
  }

  private bool signUpForJoja(int response)
  {
    if (response == 0)
    {
      this.createQuestionDialogue(Game1.parseText(Game1.content.LoadString("Strings\\Locations:JojaMart_SignUp")), this.createYesNoResponses(), "JojaSignUp");
      return true;
    }
    Game1.dialogueUp = false;
    Game1.player.forceCanMove();
    this.localSound("smallSelect");
    Game1.currentSpeaker = (NPC) null;
    Game1.dialogueTyping = false;
    return true;
  }

  public override bool answerDialogue(Response answer)
  {
    if (this.lastQuestionKey == null || this.afterQuestion != null || !($"{ArgUtility.SplitBySpaceAndGet(this.lastQuestionKey, 0)}_{answer.responseKey}" == "JojaSignUp_Yes"))
      return base.answerDialogue(answer);
    if (Game1.player.Money >= 5000)
    {
      Game1.player.Money -= 5000;
      Game1.addMailForTomorrow("JojaMember", true, true);
      Game1.player.removeQuest("26");
      JojaMart.Morris.setNewDialogue("Data\\ExtraDialogue:Morris_PlayerSignedUp");
      Game1.drawDialogue(JojaMart.Morris);
    }
    else if (Game1.player.Money < 5000)
      Game1.drawObjectDialogue(Game1.content.LoadString("Strings\\UI:NotEnoughMoney1"));
    return true;
  }

  public override bool checkAction(Location tileLocation, xTile.Dimensions.Rectangle viewport, Farmer who)
  {
    if (this.doesTileHaveProperty(tileLocation.X, tileLocation.Y, "Action", "Buildings") == "JoinJoja")
    {
      JojaMart.Morris.CurrentDialogue.Clear();
      if (Game1.player.mailForTomorrow.Contains("JojaMember%&NL&%"))
      {
        JojaMart.Morris.setNewDialogue("Data\\ExtraDialogue:Morris_ComeBackLater");
        Game1.drawDialogue(JojaMart.Morris);
      }
      else if (!Game1.player.mailReceived.Contains("JojaMember"))
      {
        if (Game1.player.mailReceived.Add("JojaGreeting"))
        {
          JojaMart.Morris.setNewDialogue("Data\\ExtraDialogue:Morris_Greeting");
          Game1.drawDialogue(JojaMart.Morris);
        }
        else if (Game1.stats.DaysPlayed < 0U)
        {
          string translationKey = Game1.dayOfMonth % 7 == 0 || Game1.dayOfMonth % 7 == 6 ? "Data\\ExtraDialogue:Morris_WeekendGreeting" : "Data\\ExtraDialogue:Morris_FirstGreeting";
          JojaMart.Morris.setNewDialogue(translationKey);
          Game1.drawDialogue(JojaMart.Morris);
        }
        else
        {
          string translationKey = Game1.dayOfMonth % 7 == 0 || Game1.dayOfMonth % 7 == 6 ? "Data\\ExtraDialogue:Morris_WeekendGreeting" : "Data\\ExtraDialogue:Morris_FirstGreeting";
          if (Game1.IsMasterGame)
          {
            if (!Game1.player.eventsSeen.Contains("611439"))
            {
              JojaMart.Morris.setNewDialogue(translationKey);
              Game1.drawDialogue(JojaMart.Morris);
            }
            else if (Game1.player.mailReceived.Contains("ccIsComplete"))
            {
              JojaMart.Morris.setNewDialogue(translationKey + "_CommunityCenterComplete");
              Game1.drawDialogue(JojaMart.Morris);
            }
            else
            {
              JojaMart.Morris.setNewDialogue(Dialogue.FromTranslation(JojaMart.Morris, translationKey + "_MembershipAvailable", (object) 5000));
              JojaMart.Morris.CurrentDialogue.Peek().answerQuestionBehavior = new Dialogue.onAnswerQuestion(this.signUpForJoja);
              Game1.drawDialogue(JojaMart.Morris);
            }
          }
          else
          {
            JojaMart.Morris.setNewDialogue(translationKey + "_SecondPlayer");
            Game1.drawDialogue(JojaMart.Morris);
          }
        }
      }
      else
      {
        if (Game1.player.eventsSeen.Contains("502261") && !Game1.player.hasOrWillReceiveMail("ccMovieTheater"))
        {
          JojaMart.Morris.setNewDialogue("Data\\ExtraDialogue:Morris_BuyMovieTheater");
          JojaMart.Morris.CurrentDialogue.Peek().answerQuestionBehavior = new Dialogue.onAnswerQuestion(this.buyMovieTheater);
        }
        else if (Game1.player.mailForTomorrow.Contains("jojaFishTank%&NL&%") || Game1.player.mailForTomorrow.Contains("jojaPantry%&NL&%") || Game1.player.mailForTomorrow.Contains("jojaCraftsRoom%&NL&%") || Game1.player.mailForTomorrow.Contains("jojaBoilerRoom%&NL&%") || Game1.player.mailForTomorrow.Contains("jojaVault%&NL&%"))
          JojaMart.Morris.setNewDialogue("Data\\ExtraDialogue:Morris_StillProcessingOrder");
        else if (Game1.player.eventsSeen.Contains("502261"))
        {
          JojaMart.Morris.setNewDialogue("Data\\ExtraDialogue:Morris_NoMoreCD");
        }
        else
        {
          JojaMart.Morris.setNewDialogue(Game1.player.IsMale ? "Data\\ExtraDialogue:Morris_CommunityDevelopmentForm_PlayerMale" : "Data\\ExtraDialogue:Morris_CommunityDevelopmentForm_PlayerFemale");
          JojaMart.Morris.CurrentDialogue.Peek().answerQuestionBehavior = new Dialogue.onAnswerQuestion(this.viewJojaNote);
        }
        Game1.drawDialogue(JojaMart.Morris);
      }
    }
    return base.checkAction(tileLocation, viewport, who);
  }

  private bool buyMovieTheater(int response)
  {
    if (response == 0)
    {
      if (Game1.player.Money >= 500000)
      {
        Game1.player.Money -= 500000;
        Game1.addMailForTomorrow("ccMovieTheater", true, true);
        Game1.addMailForTomorrow("ccMovieTheaterJoja", true, true);
        if (Game1.player.team.theaterBuildDate.Value < 0L)
          Game1.player.team.theaterBuildDate.Set((long) (Game1.Date.TotalDays + 1));
        JojaMart.Morris.setNewDialogue("Data\\ExtraDialogue:Morris_TheaterBought");
        Game1.drawDialogue(JojaMart.Morris);
      }
      else
        Game1.drawObjectDialogue(Game1.content.LoadString("Strings\\StringsFromCSFiles:PurchaseAnimalsMenu.cs.11325"));
    }
    return true;
  }

  private bool viewJojaNote(int response)
  {
    if (response == 0)
    {
      Game1.activeClickableMenu = (IClickableMenu) new JojaCDMenu(this.communityDevelopmentTexture);
      Game1.player.activeDialogueEvents.TryAdd("joja_Begin", 7);
    }
    Game1.dialogueUp = false;
    Game1.player.forceCanMove();
    this.localSound("smallSelect");
    Game1.currentSpeaker = (NPC) null;
    Game1.dialogueTyping = false;
    return true;
  }

  protected override void resetLocalState()
  {
    this.communityDevelopmentTexture = Game1.temporaryContent.Load<Texture2D>("LooseSprites\\JojaCDForm");
    JojaMart.Morris = new NPC((AnimatedSprite) null, Vector2.Zero, nameof (JojaMart), 2, "Morris", false, Game1.temporaryContent.Load<Texture2D>("Portraits\\Morris"));
    base.resetLocalState();
  }
}
