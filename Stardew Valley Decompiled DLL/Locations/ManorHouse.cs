// Decompiled with JetBrains decompiler
// Type: StardewValley.Locations.ManorHouse
// Assembly: Stardew Valley, Version=1.6.15.24356, Culture=neutral, PublicKeyToken=null
// MVID: AA1F513A-94F0-4EF6-A35F-2D5B4A3721BF
// Assembly location: D:\SteamLibrary\steamapps\common\Stardew Valley\Stardew Valley.dll
// XML documentation location: D:\SteamLibrary\steamapps\common\Stardew Valley\Stardew Valley.xml

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using StardewValley.Menus;
using StardewValley.SpecialOrders;
using StardewValley.TokenizableStrings;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml.Serialization;
using xTile.Dimensions;

#nullable disable
namespace StardewValley.Locations;

public class ManorHouse : GameLocation
{
  [XmlIgnore]
  private Dictionary<string, Farmer> sendMoneyMapping = new Dictionary<string, Farmer>();
  private static readonly bool changeWalletTypeImmediately;

  public ManorHouse()
  {
  }

  public ManorHouse(string mapPath, string name)
    : base(mapPath, name)
  {
  }

  /// <inheritdoc />
  public override bool performAction(string[] action, Farmer who, Location tileLocation)
  {
    if (who.IsLocalPlayer)
    {
      switch (ArgUtility.Get(action, 0))
      {
        case "LostAndFound":
          this.CheckLostAndFound();
          break;
        case "MayorFridge":
          if (who.Items.ContainsId("(O)284", 10) && !who.hasOrWillReceiveMail("TH_MayorFridge") && who.hasOrWillReceiveMail("TH_Railroad"))
          {
            who.Items.ReduceId("(O)284", 10);
            Game1.player.CanMove = false;
            this.localSound("coin");
            Game1.player.mailReceived.Add("TH_MayorFridge");
            Game1.multipleDialogues(new string[2]
            {
              Game1.content.LoadString("Strings\\Locations:ManorHouse_MayorFridge_ConsumeBeets"),
              Game1.content.LoadString("Strings\\Locations:ManorHouse_MayorFridge_MrQiNote")
            });
            Game1.player.removeQuest("3");
            Game1.player.addQuest("4");
            break;
          }
          if (who.hasOrWillReceiveMail("TH_MayorFridge"))
          {
            Game1.drawObjectDialogue(Game1.content.LoadString("Strings\\Locations:ManorHouse_MayorFridge_MrQiNote"));
            break;
          }
          Game1.drawObjectDialogue(Game1.content.LoadString("Strings\\Locations:ManorHouse_MayorFridge_Initial"));
          break;
        case "DivorceBook":
          if (Game1.player.divorceTonight.Value)
          {
            string question = (string) null;
            if (Game1.player.hasCurrentOrPendingRoommate())
              question = Game1.content.LoadString("Strings\\Locations:ManorHouse_DivorceBook_CancelQuestion_Krobus", (object) Game1.player.getSpouse().displayName);
            if (question == null)
              question = Game1.content.LoadStringReturnNullIfNotFound("Strings\\Locations:ManorHouse_DivorceBook_CancelQuestion");
            this.createQuestionDialogue(question, this.createYesNoResponses(), "divorceCancel");
            break;
          }
          if (Game1.player.isMarriedOrRoommates())
          {
            string question = (string) null;
            if (Game1.player.hasCurrentOrPendingRoommate())
              question = Game1.content.LoadString("Strings\\Locations:ManorHouse_DivorceBook_Question_Krobus", (object) Game1.player.getSpouse().displayName);
            if (question == null)
              question = Game1.content.LoadStringReturnNullIfNotFound("Strings\\Locations:ManorHouse_DivorceBook_Question");
            this.createQuestionDialogue(question, this.createYesNoResponses(), "divorce");
            break;
          }
          Game1.drawObjectDialogue(Game1.content.LoadString("Strings\\Locations:ManorHouse_DivorceBook_NoSpouse"));
          break;
        case "LedgerBook":
          this.readLedgerBook();
          break;
      }
    }
    return base.performAction(action, who, tileLocation);
  }

  public override void MakeMapModifications(bool force = false)
  {
    base.MakeMapModifications(force);
    if (Game1.eventUp && Game1.CurrentEvent.id != "prizeTicketIntro")
    {
      this.removeTile(4, 5, "Buildings");
      this.removeTile(4, 4, "Front");
      this.removeTile(4, 3, "Front");
      this.setMapTile(4, 6, 635, "Back", "1");
    }
    else
    {
      this.setMapTile(4, 5, 109, "Buildings", "untitled tile sheet2", "LostAndFound");
      this.setMapTile(4, 4, 77, "Front", "untitled tile sheet2");
      this.setMapTile(4, 3, 110, "Front", "untitled tile sheet2");
      this.setMapTile(4, 6, 604, "Back", "1");
    }
  }

  public void CheckLostAndFound()
  {
    string str = SpecialOrder.IsSpecialOrdersBoardUnlocked() ? Game1.content.LoadString("Strings\\Locations:ManorHouse_LAF_Check_OrdersUnlocked") : Game1.content.LoadString("Strings\\Locations:ManorHouse_LAF_Check");
    List<Response> responseList = new List<Response>();
    if (Game1.player.team.returnedDonations.Count > 0 && !Game1.player.team.returnedDonationsMutex.IsLocked())
      responseList.Add(new Response("CheckDonations", Game1.content.LoadString("Strings\\Locations:ManorHouse_LAF_DonationItems")));
    if (this.GetRetrievableFarmers().Count > 0)
      responseList.Add(new Response("RetrieveFarmhandItems", Game1.content.LoadString("Strings\\Locations:ManorHouse_LAF_FarmhandItems")));
    if (responseList.Count > 0)
      responseList.Add(new Response("Cancel", Game1.content.LoadString("Strings\\Locations:ManorHouse_LedgerBook_TransferCancel")));
    if (responseList.Count > 0)
      this.createQuestionDialogue(str, responseList.ToArray(), "lostAndFound");
    else
      Game1.drawObjectDialogue(str);
  }

  public List<Farmer> GetRetrievableFarmers()
  {
    List<Farmer> retrievableFarmers = new List<Farmer>(Game1.getAllFarmers());
    foreach (Farmer onlineFarmer in Game1.getOnlineFarmers())
      retrievableFarmers.Remove(onlineFarmer);
    for (int index = 0; index < retrievableFarmers.Count; ++index)
    {
      Farmer who = retrievableFarmers[index];
      if (Utility.getHomeOfFarmer(who) is Cabin homeOfFarmer && (who.isUnclaimedFarmhand || homeOfFarmer.inventoryMutex.IsLocked()))
      {
        retrievableFarmers.RemoveAt(index);
        --index;
      }
    }
    return retrievableFarmers;
  }

  public override void draw(SpriteBatch b)
  {
    base.draw(b);
    if (Game1.player.team.returnedDonations.Count <= 0 || Game1.eventUp)
      return;
    float num = (float) (4.0 * Math.Round(Math.Sin(Game1.currentGameTime.ElapsedGameTime.TotalMilliseconds / 250.0), 2));
    Vector2 vector2 = new Vector2(4f, 4f) * 64f + new Vector2(7f, 0.0f) * 4f;
    b.Draw(Game1.mouseCursors2, Game1.GlobalToLocal(Game1.viewport, new Vector2(vector2.X, vector2.Y + num)), new Microsoft.Xna.Framework.Rectangle?(new Microsoft.Xna.Framework.Rectangle(114, 53, 6, 10)), Color.White, 0.0f, new Vector2(1f, 4f), 4f, SpriteEffects.None, 1f);
  }

  private void readLedgerBook()
  {
    if (Game1.player.useSeparateWallets)
    {
      if (Game1.IsMasterGame)
      {
        List<Response> responseList = new List<Response>();
        responseList.Add(new Response("SendMoney", Game1.content.LoadString("Strings\\Locations:ManorHouse_LedgerBook_SendMoney")));
        if (Game1.player.changeWalletTypeTonight.Value)
          responseList.Add(new Response("CancelMerge", Game1.content.LoadString("Strings\\Locations:ManorHouse_LedgerBook_CancelMerge")));
        else
          responseList.Add(new Response("MergeWallets", Game1.content.LoadString("Strings\\Locations:ManorHouse_LedgerBook_MergeWallets")));
        responseList.Add(new Response("Leave", Game1.content.LoadString("Strings\\Locations:ManorHouse_LedgerBook_Leave")));
        this.createQuestionDialogue(Game1.content.LoadString("Strings\\Locations:ManorHouse_LedgerBook_SeparateWallets_HostQuestion"), responseList.ToArray(), "ledgerOptions");
      }
      else
        this.ChooseRecipient();
    }
    else if (!Game1.getAllFarmhands().Any<Farmer>())
      Game1.drawObjectDialogue(Game1.content.LoadString("Strings\\Locations:ManorHouse_LedgerBook_Singleplayer"));
    else if (Game1.IsMasterGame)
    {
      if (Game1.player.changeWalletTypeTonight.Value)
        this.createQuestionDialogue(Game1.content.LoadString("Strings\\Locations:ManorHouse_LedgerBook_SharedWallets_CancelQuestion"), this.createYesNoResponses(), "cancelSeparateWallets");
      else
        this.createQuestionDialogue(Game1.content.LoadString("Strings\\Locations:ManorHouse_LedgerBook_SharedWallets_SeparateQuestion"), this.createYesNoResponses(), "separateWallets");
    }
    else
      Game1.drawObjectDialogue(Game1.content.LoadString("Strings\\Locations:ManorHouse_LedgerBook_SharedWallets_Client"));
  }

  public void ShowOfflineFarmhandItemList()
  {
    List<Response> responseList = new List<Response>();
    foreach (Farmer retrievableFarmer in this.GetRetrievableFarmers())
    {
      string responseKey = retrievableFarmer.UniqueMultiplayerID.ToString() ?? "";
      string responseText = retrievableFarmer.Name;
      if (retrievableFarmer.Name == "")
        responseText = Game1.content.LoadString("Strings\\UI:Chat_PlayerJoinedNewName");
      responseList.Add(new Response(responseKey, responseText));
    }
    responseList.Add(new Response("Cancel", Game1.content.LoadString("Strings\\Locations:ManorHouse_LedgerBook_TransferCancel")));
    Game1.currentLocation.createQuestionDialogue(Game1.content.LoadString("Strings\\Locations:ManorHouse_LAF_FarmhandItemsQuestion"), responseList.ToArray(), "CheckItems");
  }

  public void ChooseRecipient()
  {
    this.sendMoneyMapping.Clear();
    List<Response> responseList = new List<Response>();
    foreach (Farmer allFarmer in Game1.getAllFarmers())
    {
      if (allFarmer.UniqueMultiplayerID != Game1.player.UniqueMultiplayerID && !allFarmer.isUnclaimedFarmhand)
      {
        string str = "Transfer" + (responseList.Count + 1).ToString();
        string responseText = allFarmer.Name;
        if (allFarmer.Name == "")
          responseText = Game1.content.LoadString("Strings\\UI:Chat_PlayerJoinedNewName");
        responseList.Add(new Response(str, responseText));
        this.sendMoneyMapping.Add(str, allFarmer);
      }
    }
    if (responseList.Count == 0)
    {
      Game1.drawObjectDialogue(Game1.content.LoadString("Strings\\Locations:ManorHouse_LedgerBook_NoFarmhands"));
    }
    else
    {
      responseList.Sort((Comparison<Response>) ((x, y) => string.Compare(x.responseKey, y.responseKey)));
      responseList.Add(new Response("Cancel", Game1.content.LoadString("Strings\\Locations:ManorHouse_LedgerBook_TransferCancel")));
      Game1.currentLocation.createQuestionDialogue(Game1.content.LoadString("Strings\\Locations:ManorHouse_LedgerBook_SeparateWallets_TransferQuestion"), responseList.ToArray(), "chooseRecipient");
    }
  }

  private void beginSendMoney(Farmer recipient)
  {
    Game1.activeClickableMenu = (IClickableMenu) new DigitEntryMenu(Game1.content.LoadString("Strings\\Locations:ManorHouse_LedgerBook_SeparateWallets_HowMuchQuestion"), (NumberSelectionMenu.behaviorOnNumberSelect) ((currentValue, price, who) => this.sendMoney(recipient, currentValue)), minValue: 1, maxValue: Game1.player.Money);
  }

  public void sendMoney(Farmer recipient, int amount)
  {
    Game1.playSound("smallSelect");
    Game1.player.Money -= amount;
    Game1.player.team.AddIndividualMoney(recipient, amount);
    Game1.player.stats.onMoneyGifted((uint) amount);
    if (amount == 1)
      Game1.multiplayer.globalChatInfoMessage("Sent1g", Game1.player.Name, recipient.Name);
    else
      Game1.multiplayer.globalChatInfoMessage("SentMoney", Game1.player.Name, recipient.Name, TokenStringBuilder.NumberWithSeparators(amount));
    Game1.exitActiveMenu();
  }

  public static void SeparateWallets()
  {
    if (Game1.player.useSeparateWallets || !Game1.IsMasterGame)
      return;
    Game1.player.changeWalletTypeTonight.Value = false;
    int money = Game1.player.Money;
    int val1 = 0;
    foreach (Farmer allFarmer in Game1.getAllFarmers())
    {
      if (!allFarmer.isUnclaimedFarmhand)
        ++val1;
    }
    int num = money / Math.Max(val1, 1);
    Game1.player.team.useSeparateWallets.Value = true;
    foreach (Farmer allFarmer in Game1.getAllFarmers())
    {
      if (!allFarmer.isUnclaimedFarmhand)
        Game1.player.team.SetIndividualMoney(allFarmer, num);
    }
    Game1.multiplayer.globalChatInfoMessage("SeparatedWallets", Game1.player.Name, num.ToString());
  }

  public static void MergeWallets()
  {
    if (!Game1.player.useSeparateWallets || !Game1.IsMasterGame)
      return;
    Game1.player.changeWalletTypeTonight.Value = false;
    int num = 0;
    foreach (Farmer allFarmer in Game1.getAllFarmers())
    {
      if (!allFarmer.isUnclaimedFarmhand)
        num += Game1.player.team.GetIndividualMoney(allFarmer);
    }
    Game1.player.team.useSeparateWallets.Value = false;
    Game1.player.team.money.Value = num;
    Game1.multiplayer.globalChatInfoMessage("MergedWallets", Game1.player.Name);
  }

  public override bool answerDialogueAction(string questionAndAnswer, string[] questionParams)
  {
    string dialogue = (string) null;
    if (questionAndAnswer == null)
      return false;
    if (questionAndAnswer != null)
    {
      switch (questionAndAnswer.Length)
      {
        case 11:
          if (questionAndAnswer == "divorce_Yes")
          {
            if (Game1.player.Money >= 50000 || Game1.player.hasCurrentOrPendingRoommate())
            {
              if (!Game1.player.hasRoommate())
                Game1.player.Money -= 50000;
              Game1.player.divorceTonight.Value = true;
              if (Game1.player.hasCurrentOrPendingRoommate())
                dialogue = Game1.content.LoadString("Strings\\Locations:ManorHouse_DivorceBook_Filed_Krobus", (object) Game1.player.getSpouse().displayName);
              if (dialogue == null)
                dialogue = Game1.content.LoadStringReturnNullIfNotFound("Strings\\Locations:ManorHouse_DivorceBook_Filed");
              Game1.drawObjectDialogue(dialogue);
              if (!Game1.player.hasRoommate())
              {
                Game1.multiplayer.globalChatInfoMessage("Divorce", Game1.player.Name);
                break;
              }
              break;
            }
            Game1.drawObjectDialogue(Game1.content.LoadString("Strings\\UI:NotEnoughMoney1"));
            break;
          }
          break;
        case 16 /*0x10*/:
          if (questionAndAnswer == "mergeWallets_Yes")
          {
            if (ManorHouse.changeWalletTypeImmediately)
            {
              ManorHouse.MergeWallets();
              break;
            }
            Game1.player.changeWalletTypeTonight.Value = true;
            Game1.multiplayer.globalChatInfoMessage("MergeWallets", Game1.player.Name);
            break;
          }
          break;
        case 17:
          if (questionAndAnswer == "divorceCancel_Yes" && Game1.player.divorceTonight.Value)
          {
            Game1.player.divorceTonight.Value = false;
            if (!Game1.player.hasRoommate())
              Game1.player.addUnearnedMoney(50000);
            if (Game1.player.hasCurrentOrPendingRoommate())
              dialogue = Game1.content.LoadString("Strings\\Locations:ManorHouse_DivorceBook_Cancelled_Krobus", (object) Game1.player.getSpouse().displayName);
            if (dialogue == null)
              dialogue = Game1.content.LoadStringReturnNullIfNotFound("Strings\\Locations:ManorHouse_DivorceBook_Cancelled");
            Game1.drawObjectDialogue(dialogue);
            if (!Game1.player.hasRoommate())
            {
              Game1.multiplayer.globalChatInfoMessage("DivorceCancel", Game1.player.Name);
              break;
            }
            break;
          }
          break;
        case 19:
          if (questionAndAnswer == "separateWallets_Yes")
          {
            if (ManorHouse.changeWalletTypeImmediately)
            {
              ManorHouse.SeparateWallets();
              break;
            }
            Game1.player.changeWalletTypeTonight.Value = true;
            Game1.multiplayer.globalChatInfoMessage("SeparateWallets", Game1.player.Name);
            break;
          }
          break;
        case 22:
          if (questionAndAnswer == "cancelMergeWallets_Yes")
          {
            Game1.player.changeWalletTypeTonight.Value = false;
            Game1.multiplayer.globalChatInfoMessage("MergeWalletsCancel", Game1.player.Name);
            break;
          }
          break;
        case 23:
          if (questionAndAnswer == "ledgerOptions_SendMoney")
          {
            this.ChooseRecipient();
            break;
          }
          break;
        case 25:
          switch (questionAndAnswer[0])
          {
            case 'c':
              if (questionAndAnswer == "cancelSeparateWallets_Yes")
              {
                Game1.player.changeWalletTypeTonight.Value = false;
                Game1.multiplayer.globalChatInfoMessage("SeparateWalletsCancel", Game1.player.Name);
                break;
              }
              break;
            case 'l':
              if (questionAndAnswer == "ledgerOptions_CancelMerge")
              {
                this.createQuestionDialogue(Game1.content.LoadString("Strings\\Locations:ManorHouse_LedgerBook_SeparateWallets_CancelQuestion"), this.createYesNoResponses(), "cancelMergeWallets");
                break;
              }
              break;
          }
          break;
        case 26:
          if (questionAndAnswer == "ledgerOptions_MergeWallets")
          {
            this.createQuestionDialogue(Game1.content.LoadString("Strings\\Locations:ManorHouse_LedgerBook_SeparateWallets_MergeQuestion"), this.createYesNoResponses(), "mergeWallets");
            break;
          }
          break;
        case 27:
          if (questionAndAnswer == "lostAndFound_CheckDonations")
          {
            Game1.player.team.CheckReturnedDonations();
            return true;
          }
          break;
        case 34:
          if (questionAndAnswer == "lostAndFound_RetrieveFarmhandItems")
          {
            this.ShowOfflineFarmhandItemList();
            return true;
          }
          break;
      }
    }
    if (questionAndAnswer.StartsWith("CheckItems"))
    {
      long result;
      if (long.TryParse(questionAndAnswer.Split('_')[1], out result))
      {
        Farmer player = Game1.GetPlayer(result);
        if (player != null && Utility.getHomeOfFarmer(player) is Cabin homeOfFarmer && !player.isActive())
          homeOfFarmer.inventoryMutex.RequestLock(new Action(homeOfFarmer.openFarmhandInventory));
      }
      return true;
    }
    if (questionAndAnswer.Contains("Transfer"))
    {
      this.beginSendMoney(this.sendMoneyMapping[questionAndAnswer.Split('_')[1]]);
      this.sendMoneyMapping.Clear();
    }
    return base.answerDialogueAction(questionAndAnswer, questionParams);
  }
}
