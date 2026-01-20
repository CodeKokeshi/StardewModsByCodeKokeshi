// Decompiled with JetBrains decompiler
// Type: StardewValley.Objects.DefaultPhoneHandler
// Assembly: Stardew Valley, Version=1.6.15.24356, Culture=neutral, PublicKeyToken=null
// MVID: AA1F513A-94F0-4EF6-A35F-2D5B4A3721BF
// Assembly location: D:\SteamLibrary\steamapps\common\Stardew Valley\Stardew Valley.dll
// XML documentation location: D:\SteamLibrary\steamapps\common\Stardew Valley\Stardew Valley.xml

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using StardewValley.Extensions;
using StardewValley.GameData;
using StardewValley.Locations;
using StardewValley.Network;
using StardewValley.TokenizableStrings;
using System;
using System.Collections.Generic;

#nullable disable
namespace StardewValley.Objects;

/// <summary>Handles incoming and outgoing phone calls for the base game.</summary>
public class DefaultPhoneHandler : IPhoneHandler
{
  /// <inheritdoc />
  public string CheckForIncomingCall(Random random)
  {
    List<string> options = new List<string>();
    bool flag = random.NextDouble() < 0.01;
    foreach (KeyValuePair<string, IncomingPhoneCallData> incomingPhoneCall in DataLoader.IncomingPhoneCalls(Game1.content))
    {
      if ((flag || incomingPhoneCall.Value.IgnoreBaseChance) && (incomingPhoneCall.Value.TriggerCondition == null || GameStateQuery.CheckConditions(incomingPhoneCall.Value.TriggerCondition, Game1.currentLocation, Game1.player, random: random)))
        options.Add(incomingPhoneCall.Key);
    }
    return random.ChooseFrom<string>((IList<string>) options);
  }

  /// <inheritdoc />
  public bool TryHandleIncomingCall(string callId, out Action showDialogue)
  {
    showDialogue = (Action) null;
    int num;
    IncomingPhoneCallData call;
    if (!DataLoader.IncomingPhoneCalls(Game1.content).TryGetValue(callId, out call) || call.MaxCalls > -1 && Game1.player.callsReceived.TryGetValue(callId, out num) && num >= call.MaxCalls || call.RingCondition != null && !GameStateQuery.CheckConditions(call.RingCondition, Game1.currentLocation, Game1.player) || Game1.IsGreenRainingHere())
      return false;
    showDialogue = (Action) (() =>
    {
      if (!string.IsNullOrWhiteSpace(call.SimpleDialogueSplitBy))
      {
        Game1.multipleDialogues((TokenParser.ParseText(call.Dialogue) ?? Dialogue.GetFallbackTextForError()).Split(call.SimpleDialogueSplitBy));
      }
      else
      {
        NPC speaker = (NPC) null;
        if (call.FromNpc != null)
        {
          speaker = Game1.getCharacterFromName(call.FromNpc);
          if (speaker == null)
            Game1.log.Warn($"Can't find NPC '{call.FromNpc}' for incoming call ID '{callId}'.");
        }
        string text = TokenParser.ParseText(call.FromDisplayName);
        Texture2D portrait = (Texture2D) null;
        if (call.FromPortrait != null)
        {
          if (!Game1.content.DoesAssetExist<Texture2D>(call.FromPortrait))
            Game1.log.Warn($"Can't load custom portrait '{call.FromPortrait}' for incoming call ID '{callId}' because that texture doesn't exist.");
          else
            portrait = Game1.content.Load<Texture2D>(call.FromPortrait);
        }
        if (portrait != null || text != null)
        {
          if (speaker != null)
          {
            speaker = new NPC(speaker.Sprite, Vector2.Zero, "", 0, speaker.Name, portrait ?? speaker.Portrait, false);
            speaker.displayName = text ?? speaker.displayName;
          }
          else if (portrait != null)
            speaker = new NPC(new AnimatedSprite("Characters\\Abigail", 0, 16 /*0x10*/, 16 /*0x10*/), Vector2.Zero, "", 0, "???", portrait, false)
            {
              displayName = text ?? "???"
            };
        }
        string translationKey = "Data\\IncomingPhoneCalls:" + callId;
        string dialogueText = TokenParser.ParseText(call.Dialogue) ?? Dialogue.GetFallbackTextForError();
        Game1.DrawDialogue(new Dialogue(speaker, translationKey, dialogueText));
      }
    });
    return true;
  }

  /// <inheritdoc />
  public IEnumerable<KeyValuePair<string, string>> GetOutgoingNumbers()
  {
    List<KeyValuePair<string, string>> numbers = new List<KeyValuePair<string, string>>(6);
    AddNumber("Carpenter", "Robin");
    AddNumber("Blacksmith", "Clint");
    AddNumber("SeedShop", "Pierre");
    AddNumber("AnimalShop", "Marnie");
    AddNumber("Saloon", "Gus");
    if (Game1.player.mailReceived.Contains("Gil_Telephone") || Game1.player.mailReceived.Contains("Gil_FlameSpirits"))
      AddNumber("AdventureGuild", "Marlon");
    return (IEnumerable<KeyValuePair<string, string>>) numbers;

    void AddNumber(string callerId, string npcName)
    {
      NPC characterFromName = Game1.getCharacterFromName(npcName);
      if (characterFromName == null)
        return;
      numbers.Add(new KeyValuePair<string, string>(callerId, characterFromName.displayName));
    }
  }

  /// <inheritdoc />
  public bool TryHandleOutgoingCall(string callId)
  {
    switch (callId)
    {
      case "AdventureGuild":
        this.CallAdventureGuild();
        return true;
      case "AnimalShop":
        this.CallAnimalShop();
        return true;
      case "Blacksmith":
        this.CallBlacksmith();
        return true;
      case "Carpenter":
        this.CallCarpenter();
        return true;
      case "Saloon":
        this.CallSaloon();
        return true;
      case "SeedShop":
        this.CallSeedShop();
        return true;
      default:
        return false;
    }
  }

  /// <summary>Handle an outgoing call to the Adventurer's Guild.</summary>
  public void CallAdventureGuild()
  {
    Game1.currentLocation.playShopPhoneNumberSounds("AdventureGuild");
    Game1.player.freezePause = 4950;
    DelayedAction.functionAfterDelay((Action) (() =>
    {
      Game1.playSound("bigSelect");
      NPC character = Game1.getCharacterFromName("Marlon");
      if (Game1.player.mailForTomorrow.Contains("MarlonRecovery"))
      {
        Game1.DrawDialogue(character, "Strings\\Characters:Phone_Marlon_AlreadyRecovering");
      }
      else
      {
        Game1.DrawDialogue(character, "Strings\\Characters:Phone_Marlon_Open");
        Game1.afterDialogues += (Game1.afterFadeFunction) (() =>
        {
          if (Game1.player.itemsLostLastDeath.Count > 0)
          {
            Game1.player.forceCanMove();
            Utility.TryOpenShopMenu("AdventureGuildRecovery", "Marlon");
          }
          else
            Game1.DrawDialogue(character, "Strings\\Characters:Phone_Marlon_NoDeathItems");
        });
      }
    }), 4950);
  }

  /// <summary>Handle an outgoing call to Marnie's animal shop.</summary>
  public void CallAnimalShop()
  {
    GameLocation location = Game1.currentLocation;
    location.playShopPhoneNumberSounds("AnimalShop");
    Game1.player.freezePause = 4950;
    DelayedAction.functionAfterDelay((Action) (() =>
    {
      Game1.playSound("bigSelect");
      NPC characterFromName = Game1.getCharacterFromName("Marnie");
      if (GameLocation.AreStoresClosedForFestival())
        Game1.DrawAnsweringMachineDialogue(characterFromName, "Strings\\Characters:Phone_Marnie_ClosedDay");
      else if (characterFromName.ScheduleKey == "fall_18" || characterFromName.ScheduleKey == "winter_18" || characterFromName.ScheduleKey == "Tue" || characterFromName.ScheduleKey == "Mon")
        Game1.DrawAnsweringMachineDialogue(characterFromName, "Strings\\Characters:Phone_Marnie_ClosedDay");
      else if (Game1.timeOfDay >= 900 && Game1.timeOfDay < 1600)
        Game1.DrawDialogue(characterFromName, "Strings\\Characters:Phone_Marnie_Open" + (Game1.random.NextDouble() < 0.01 ? "_Rare" : ""));
      else
        Game1.DrawAnsweringMachineDialogue(characterFromName, "Strings\\Characters:Phone_Marnie_Closed");
      Game1.afterDialogues += (Game1.afterFadeFunction) (() =>
      {
        Response[] answerChoices = new Response[2]
        {
          new Response("AnimalShop_CheckAnimalPrices", Game1.content.LoadString("Strings\\Characters:Phone_CheckAnimalPrices")),
          new Response("HangUp", Game1.content.LoadString("Strings\\Characters:Phone_HangUp"))
        };
        location.createQuestionDialogue(Game1.content.LoadString("Strings\\Characters:Phone_SelectOption"), answerChoices, "telephone");
      });
    }), 4950);
  }

  /// <summary>Handle an outgoing call to Clint's blacksmith shop.</summary>
  public void CallBlacksmith()
  {
    GameLocation location = Game1.currentLocation;
    location.playShopPhoneNumberSounds("Blacksmith");
    Game1.player.freezePause = 4950;
    DelayedAction.functionAfterDelay((Action) (() =>
    {
      Game1.playSound("bigSelect");
      NPC characterFromName = Game1.getCharacterFromName("Clint");
      if (GameLocation.AreStoresClosedForFestival())
        Game1.DrawAnsweringMachineDialogue(characterFromName, "Strings\\Characters:Phone_Clint_Festival");
      else if (Game1.player.daysLeftForToolUpgrade.Value > 0)
      {
        int num = Game1.player.daysLeftForToolUpgrade.Value;
        if (num == 1)
          Game1.DrawDialogue(characterFromName, "Strings\\Characters:Phone_Clint_Working_OneDay");
        else
          Game1.DrawDialogue(characterFromName, "Strings\\Characters:Phone_Clint_Working", (object) num);
      }
      else
      {
        switch (characterFromName.ScheduleKey)
        {
          case "winter_16":
            Game1.DrawAnsweringMachineDialogue(characterFromName, "Strings\\Characters:Phone_Clint_Festival");
            break;
          case "Fri":
            Game1.DrawAnsweringMachineDialogue(characterFromName, "Strings\\Characters:Phone_Clint_Festival");
            break;
          default:
            if (Game1.timeOfDay >= 900 && Game1.timeOfDay < 1600)
            {
              Game1.DrawDialogue(characterFromName, "Strings\\Characters:Phone_Clint_Open" + (Game1.random.NextDouble() < 0.01 ? "_Rare" : ""));
              break;
            }
            Game1.DrawAnsweringMachineDialogue(characterFromName, "Strings\\Characters:Phone_Clint_Closed");
            break;
        }
      }
      Game1.afterDialogues += (Game1.afterFadeFunction) (() =>
      {
        Response[] answerChoices = new Response[2]
        {
          new Response("Blacksmith_UpgradeCost", Game1.content.LoadString("Strings\\Characters:Phone_CheckToolCost")),
          new Response("HangUp", Game1.content.LoadString("Strings\\Characters:Phone_HangUp"))
        };
        location.createQuestionDialogue(Game1.content.LoadString("Strings\\Characters:Phone_SelectOption"), answerChoices, "telephone");
      });
    }), 4950);
  }

  /// <summary>Handle an outgoing call to Robin's shop.</summary>
  public void CallCarpenter()
  {
    GameLocation location = Game1.currentLocation;
    location.playShopPhoneNumberSounds("Carpenter");
    Game1.player.freezePause = 4950;
    DelayedAction.functionAfterDelay((Action) (() =>
    {
      Game1.playSound("bigSelect");
      NPC characterFromName = Game1.getCharacterFromName("Robin");
      if (GameLocation.AreStoresClosedForFestival())
        Game1.DrawAnsweringMachineDialogue(characterFromName, "Strings\\Characters:Phone_Robin_Festival");
      else if (Game1.getLocationFromName("Town") is Town locationFromName2 && locationFromName2.daysUntilCommunityUpgrade.Value > 0)
      {
        int num = locationFromName2.daysUntilCommunityUpgrade.Value;
        if (num == 1)
          Game1.DrawDialogue(characterFromName, "Strings\\Characters:Phone_Robin_Working_OneDay");
        else
          Game1.DrawDialogue(characterFromName, "Strings\\Characters:Phone_Robin_Working", (object) num);
      }
      else if (Game1.IsThereABuildingUnderConstruction())
      {
        BuilderData builderData = Game1.netWorldState.Value.GetBuilderData("Robin");
        int num = 0;
        if (builderData != null)
          num = builderData.daysUntilBuilt.Value;
        if (num == 1)
          Game1.DrawDialogue(characterFromName, "Strings\\Characters:Phone_Robin_Working_OneDay");
        else
          Game1.DrawDialogue(characterFromName, "Strings\\Characters:Phone_Robin_Working", (object) num);
      }
      else
      {
        switch (characterFromName.ScheduleKey)
        {
          case "summer_18":
            Game1.DrawAnsweringMachineDialogue(characterFromName, "Strings\\Characters:Phone_Robin_Festival");
            break;
          case "Tue":
            Game1.DrawAnsweringMachineDialogue(characterFromName, "Strings\\Characters:Phone_Robin_Workout");
            break;
          default:
            if (Game1.timeOfDay >= 900 && Game1.timeOfDay < 1700)
            {
              Game1.DrawDialogue(characterFromName, "Strings\\Characters:Phone_Robin_Open" + (Game1.random.NextDouble() < 0.01 ? "_Rare" : ""));
              break;
            }
            Game1.DrawAnsweringMachineDialogue(characterFromName, "Strings\\Characters:Phone_Robin_Closed");
            break;
        }
      }
      Game1.afterDialogues += (Game1.afterFadeFunction) (() =>
      {
        List<Response> responseList = new List<Response>();
        responseList.Add(new Response("Carpenter_ShopStock", Game1.content.LoadString("Strings\\Characters:Phone_CheckSeedStock")));
        if (Game1.player.houseUpgradeLevel.Value < 3)
          responseList.Add(new Response("Carpenter_HouseCost", Game1.content.LoadString("Strings\\Characters:Phone_CheckHouseCost")));
        responseList.Add(new Response("Carpenter_BuildingCost", Game1.content.LoadString("Strings\\Characters:Phone_CheckBuildingCost")));
        responseList.Add(new Response("HangUp", Game1.content.LoadString("Strings\\Characters:Phone_HangUp")));
        location.createQuestionDialogue(Game1.content.LoadString("Strings\\Characters:Phone_SelectOption"), responseList.ToArray(), "telephone");
      });
    }), 4950);
  }

  /// <summary>Handle an outgoing call to Gus' saloon.</summary>
  public void CallSaloon()
  {
    GameLocation location = Game1.currentLocation;
    location.playShopPhoneNumberSounds("Saloon");
    Game1.player.freezePause = 4950;
    DelayedAction.functionAfterDelay((Action) (() =>
    {
      Game1.playSound("bigSelect");
      NPC characterFromName = Game1.getCharacterFromName("Gus");
      if (GameLocation.AreStoresClosedForFestival())
        Game1.DrawAnsweringMachineDialogue(characterFromName, "Strings\\Characters:Phone_Gus_Festival");
      else if (Game1.timeOfDay >= 1200 && Game1.timeOfDay < 2400 && (characterFromName.ScheduleKey != "fall_4" || Game1.timeOfDay >= 1700))
      {
        if (Game1.dishOfTheDay != null)
          Game1.DrawDialogue(characterFromName, "Strings\\Characters:Phone_Gus_Open" + (Game1.random.NextDouble() < 0.01 ? "_Rare" : ""), (object) Game1.dishOfTheDay.DisplayName);
        else
          Game1.DrawDialogue(characterFromName, "Strings\\Characters:Phone_Gus_Open_NoDishOfTheDay");
      }
      else if (Game1.dishOfTheDay != null && Game1.timeOfDay < 2400)
        Game1.DrawAnsweringMachineDialogue(characterFromName, "Strings\\Characters:Phone_Gus_Closed", (object) Game1.dishOfTheDay.DisplayName);
      else
        Game1.DrawAnsweringMachineDialogue(characterFromName, "Strings\\Characters:Phone_Gus_Closed_NoDishOfTheDay");
      location.answerDialogueAction("HangUp", LegacyShims.EmptyArray<string>());
    }), 4950);
  }

  /// <summary>Handle an outgoing call to Pierre's shop.</summary>
  public void CallSeedShop()
  {
    GameLocation location = Game1.currentLocation;
    location.playShopPhoneNumberSounds("SeedShop");
    Game1.player.freezePause = 4950;
    DelayedAction.functionAfterDelay((Action) (() =>
    {
      Game1.playSound("bigSelect");
      NPC characterFromName = Game1.getCharacterFromName("Pierre");
      string str = Game1.shortDayNameFromDayOfSeason(Game1.dayOfMonth);
      if (GameLocation.AreStoresClosedForFestival())
        Game1.DrawAnsweringMachineDialogue(characterFromName, "Strings\\Characters:Phone_Pierre_Festival");
      else if ((Game1.isLocationAccessible("CommunityCenter") || str != "Wed") && Game1.timeOfDay >= 900 && Game1.timeOfDay < 1700)
        Game1.DrawDialogue(characterFromName, "Strings\\Characters:Phone_Pierre_Open" + (Game1.random.NextDouble() < 0.01 ? "_Rare" : ""));
      else
        Game1.DrawAnsweringMachineDialogue(characterFromName, "Strings\\Characters:Phone_Pierre_Closed");
      Game1.afterDialogues += (Game1.afterFadeFunction) (() =>
      {
        Response[] answerChoices = new Response[2]
        {
          new Response("SeedShop_CheckSeedStock", Game1.content.LoadString("Strings\\Characters:Phone_CheckSeedStock")),
          new Response("HangUp", Game1.content.LoadString("Strings\\Characters:Phone_HangUp"))
        };
        location.createQuestionDialogue(Game1.content.LoadString("Strings\\Characters:Phone_SelectOption"), answerChoices, "telephone");
      });
    }), 4950);
  }

  /// <summary>The call IDs for phone numbers the player can call.</summary>
  public static class OutgoingCallIds
  {
    /// <summary>An outgoing call to the Adventurer's Guild.</summary>
    public const string AdventureGuild = "AdventureGuild";
    /// <summary>An outgoing call to Marnie's animal shop.</summary>
    public const string AnimalShop = "AnimalShop";
    /// <summary>An outgoing call to Clint's blacksmith shop.</summary>
    public const string Blacksmith = "Blacksmith";
    /// <summary>An outgoing call to Robin's shop.</summary>
    public const string Carpenter = "Carpenter";
    /// <summary>An outgoing call to Gus' Saloon.</summary>
    public const string Saloon = "Saloon";
    /// <summary>An outgoing call to Pierre's shop.</summary>
    public const string SeedShop = "SeedShop";
  }
}
