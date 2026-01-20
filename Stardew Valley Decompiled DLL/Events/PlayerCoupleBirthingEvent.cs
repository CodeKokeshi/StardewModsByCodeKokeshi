// Decompiled with JetBrains decompiler
// Type: StardewValley.Events.PlayerCoupleBirthingEvent
// Assembly: Stardew Valley, Version=1.6.15.24356, Culture=neutral, PublicKeyToken=null
// MVID: AA1F513A-94F0-4EF6-A35F-2D5B4A3721BF
// Assembly location: D:\SteamLibrary\steamapps\common\Stardew Valley\Stardew Valley.dll
// XML documentation location: D:\SteamLibrary\steamapps\common\Stardew Valley\Stardew Valley.xml

using Microsoft.Xna.Framework;
using StardewValley.BellsAndWhistles;
using StardewValley.Characters;
using StardewValley.Extensions;
using StardewValley.Locations;
using StardewValley.Menus;
using System;
using System.Collections.Generic;

#nullable disable
namespace StardewValley.Events;

public class PlayerCoupleBirthingEvent : BaseFarmEvent
{
  private int timer;
  private string soundName;
  private string message;
  private string babyName;
  private bool playedSound;
  private bool isMale;
  private bool getBabyName;
  private bool naming;
  private FarmHouse farmHouse;
  private long spouseID;
  private Farmer spouse;
  private bool isPlayersTurn;
  private Child child;

  public PlayerCoupleBirthingEvent()
  {
    this.spouseID = Game1.player.team.GetSpouse(Game1.player.UniqueMultiplayerID).Value;
    Game1.otherFarmers.TryGetValue(this.spouseID, out this.spouse);
    this.farmHouse = this.chooseHome();
  }

  private bool isSuitableHome(FarmHouse home)
  {
    return home.getChildrenCount() < 2 && home.upgradeLevel >= 2;
  }

  private FarmHouse chooseHome()
  {
    List<Farmer> farmerList = new List<Farmer>()
    {
      Game1.player,
      this.spouse
    };
    farmerList.Sort((Comparison<Farmer>) ((p1, p2) => p1.UniqueMultiplayerID.CompareTo(p2.UniqueMultiplayerID)));
    foreach (Farmer farmer in farmerList)
    {
      if (Game1.getLocationFromName(farmer.homeLocation.Value) is FarmHouse locationFromName && locationFromName == farmer.currentLocation && this.isSuitableHome(locationFromName))
        return locationFromName;
    }
    foreach (Farmer farmer in farmerList)
    {
      if (Game1.getLocationFromName(farmer.homeLocation.Value) is FarmHouse locationFromName && this.isSuitableHome(locationFromName))
        return locationFromName;
    }
    return Game1.player.currentLocation as FarmHouse;
  }

  /// <inheritdoc />
  public override bool setUp()
  {
    if (this.spouse == null || this.farmHouse == null)
      return true;
    Random random = Utility.CreateRandom((double) Game1.uniqueIDForThisGame, (double) Game1.Date.TotalDays);
    Game1.player.CanMove = false;
    this.isMale = this.farmHouse.getChildrenCount() != 0 ? this.farmHouse.getChildren()[0].Gender == Gender.Female : random.NextBool();
    this.isPlayersTurn = Game1.player.GetSpouseFriendship().Proposer != Game1.player.UniqueMultiplayerID == (this.farmHouse.getChildrenCount() % 2 == 0);
    this.message = this.spouse.IsMale != Game1.player.IsMale ? (!this.spouse.IsMale ? Game1.content.LoadString("Strings\\Events:BirthMessage_SpouseMother", (object) Lexicon.getGenderedChildTerm(this.isMale), (object) this.spouse.Name) : Game1.content.LoadString("Strings\\Events:BirthMessage_PlayerMother", (object) Lexicon.getGenderedChildTerm(this.isMale))) : Game1.content.LoadString("Strings\\Events:BirthMessage_Adoption", (object) Lexicon.getGenderedChildTerm(this.isMale));
    return false;
  }

  public void returnBabyName(string name)
  {
    this.babyName = name;
    Game1.exitActiveMenu();
  }

  public void afterMessage()
  {
    if (this.isPlayersTurn)
    {
      this.getBabyName = true;
      double num = (this.spouse.hasDarkSkin() ? 0.5 : 0.0) + (Game1.player.hasDarkSkin() ? 0.5 : 0.0);
      this.farmHouse.characters.Add((NPC) (this.child = new Child("Baby", this.isMale, Utility.CreateRandom((double) Game1.uniqueIDForThisGame, (double) Game1.stats.DaysPlayed).NextDouble() < num, Game1.player)));
      this.child.Age = 0;
      this.child.Position = new Vector2(16f, 4f) * 64f + new Vector2(0.0f, -24f);
      Game1.player.stats.checkForFullHouseAchievement(true);
      Game1.player.GetSpouseFriendship().NextBirthingDate = (WorldDate) null;
    }
    else
    {
      Game1.afterDialogues = (Game1.afterFadeFunction) (() => this.getBabyName = true);
      Game1.drawObjectDialogue(Game1.content.LoadString("Strings\\Events:BirthMessage_SpouseNaming_" + (this.isMale ? "Male" : "Female"), (object) this.spouse.Name));
    }
  }

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
      if (!this.isPlayersTurn)
      {
        Game1.globalFadeToClear();
        return true;
      }
      if (!this.naming)
      {
        Game1.activeClickableMenu = (IClickableMenu) new NamingMenu(new NamingMenu.doneNamingBehavior(this.returnBabyName), Game1.content.LoadString(this.isMale ? "Strings\\Events:BabyNamingTitle_Male" : "Strings\\Events:BabyNamingTitle_Female"), "");
        this.naming = true;
      }
      if (!string.IsNullOrEmpty(this.babyName) && this.babyName.Length > 0)
      {
        string babyName = this.babyName;
        List<NPC> allCharacters = Utility.getAllCharacters();
        bool flag;
        do
        {
          flag = false;
          foreach (Character character in allCharacters)
          {
            if (character.Name == babyName)
            {
              babyName += " ";
              flag = true;
              break;
            }
          }
        }
        while (flag);
        this.child.Name = babyName;
        Game1.playSound("smallSelect");
        if (Game1.keyboardDispatcher != null)
          Game1.keyboardDispatcher.Subscriber = (IKeyboardSubscriber) null;
        Game1.globalFadeToClear();
        return true;
      }
    }
    return false;
  }
}
