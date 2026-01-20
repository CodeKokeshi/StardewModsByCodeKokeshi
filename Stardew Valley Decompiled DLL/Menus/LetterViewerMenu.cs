// Decompiled with JetBrains decompiler
// Type: StardewValley.Menus.LetterViewerMenu
// Assembly: Stardew Valley, Version=1.6.15.24356, Culture=neutral, PublicKeyToken=null
// MVID: AA1F513A-94F0-4EF6-A35F-2D5B4A3721BF
// Assembly location: D:\SteamLibrary\steamapps\common\Stardew Valley\Stardew Valley.dll
// XML documentation location: D:\SteamLibrary\steamapps\common\Stardew Valley\Stardew Valley.xml

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using StardewValley.BellsAndWhistles;
using StardewValley.Extensions;
using StardewValley.Locations;
using StardewValley.Triggers;
using System;
using System.Collections.Generic;

#nullable disable
namespace StardewValley.Menus;

public class LetterViewerMenu : IClickableMenu
{
  public const int region_backButton = 101;
  public const int region_forwardButton = 102;
  public const int region_acceptQuestButton = 103;
  public const int region_itemGrabButton = 104;
  public const int letterWidth = 320;
  public const int letterHeight = 180;
  public Texture2D letterTexture;
  public Texture2D secretNoteImageTexture;
  public int moneyIncluded;
  public int secretNoteImage = -1;
  public int whichBG;
  /// <summary>The ID of the quest attached to the letter being viewed, if any.</summary>
  public string questID;
  /// <summary>The ID of the special order attached to the letter being viewed, if any.</summary>
  public string specialOrderId;
  /// <summary>The translated name of the recipe learned from this letter, if any.</summary>
  public string learnedRecipe = "";
  public string cookingOrCrafting = "";
  public string mailTitle;
  public List<string> mailMessage = new List<string>();
  public int page;
  public readonly List<ClickableComponent> itemsToGrab = new List<ClickableComponent>();
  public float scale;
  public bool isMail;
  public bool isFromCollection;
  public new bool destroy;
  public Color? customTextColor;
  public bool usingCustomBackground;
  public ClickableTextureComponent backButton;
  public ClickableTextureComponent forwardButton;
  public ClickableComponent acceptQuestButton;
  public const float scaleChange = 0.003f;

  /// <summary>Whether the letter has an attached quest or special order which the player can accept.</summary>
  public bool HasQuestOrSpecialOrder => this.questID != null || this.specialOrderId != null;

  public LetterViewerMenu(string text)
    : base((int) Utility.getTopLeftPositionForCenteringOnScreen(1280 /*0x0500*/, 720).X, (int) Utility.getTopLeftPositionForCenteringOnScreen(1280 /*0x0500*/, 720).Y, 1280 /*0x0500*/, 720, true)
  {
    Game1.playSound("shwip");
    ClickableTextureComponent textureComponent1 = new ClickableTextureComponent(new Rectangle(this.xPositionOnScreen + 32 /*0x20*/, this.yPositionOnScreen + this.height - 32 /*0x20*/ - 64 /*0x40*/, 48 /*0x30*/, 44), Game1.mouseCursors, new Rectangle(352, 495, 12, 11), 4f);
    textureComponent1.myID = 101;
    textureComponent1.rightNeighborID = 102;
    this.backButton = textureComponent1;
    ClickableTextureComponent textureComponent2 = new ClickableTextureComponent(new Rectangle(this.xPositionOnScreen + this.width - 32 /*0x20*/ - 48 /*0x30*/, this.yPositionOnScreen + this.height - 32 /*0x20*/ - 64 /*0x40*/, 48 /*0x30*/, 44), Game1.mouseCursors, new Rectangle(365, 495, 12, 11), 4f);
    textureComponent2.myID = 102;
    textureComponent2.leftNeighborID = 101;
    this.forwardButton = textureComponent2;
    this.letterTexture = Game1.temporaryContent.Load<Texture2D>("LooseSprites\\letterBG");
    text = this.ApplyCustomFormatting(text);
    this.mailMessage = SpriteText.getStringBrokenIntoSectionsOfHeight(text, this.width - 64 /*0x40*/, this.height - 128 /*0x80*/);
    this.forwardButton.visible = this.page < this.mailMessage.Count - 1;
    this.backButton.visible = this.page > 0;
    this.OnPageChange();
    this.populateClickableComponentList();
    if (!Game1.options.SnappyMenus)
      return;
    this.snapToDefaultClickableComponent();
  }

  public LetterViewerMenu(int secretNoteIndex)
    : base((int) Utility.getTopLeftPositionForCenteringOnScreen(1280 /*0x0500*/, 720).X, (int) Utility.getTopLeftPositionForCenteringOnScreen(1280 /*0x0500*/, 720).Y, 1280 /*0x0500*/, 720, true)
  {
    Game1.playSound("shwip");
    ClickableTextureComponent textureComponent1 = new ClickableTextureComponent(new Rectangle(this.xPositionOnScreen + 32 /*0x20*/, this.yPositionOnScreen + this.height - 32 /*0x20*/ - 64 /*0x40*/, 48 /*0x30*/, 44), Game1.mouseCursors, new Rectangle(352, 495, 12, 11), 4f);
    textureComponent1.myID = 101;
    textureComponent1.rightNeighborID = 102;
    this.backButton = textureComponent1;
    ClickableTextureComponent textureComponent2 = new ClickableTextureComponent(new Rectangle(this.xPositionOnScreen + this.width - 32 /*0x20*/ - 48 /*0x30*/, this.yPositionOnScreen + this.height - 32 /*0x20*/ - 64 /*0x40*/, 48 /*0x30*/, 44), Game1.mouseCursors, new Rectangle(365, 495, 12, 11), 4f);
    textureComponent2.myID = 102;
    textureComponent2.leftNeighborID = 101;
    this.forwardButton = textureComponent2;
    this.letterTexture = Game1.temporaryContent.Load<Texture2D>("LooseSprites\\letterBG");
    string secretNote = DataLoader.SecretNotes(Game1.content)[secretNoteIndex];
    if (secretNote[0] == '!')
    {
      this.secretNoteImageTexture = Game1.temporaryContent.Load<Texture2D>("TileSheets\\SecretNotesImages");
      this.secretNoteImage = Convert.ToInt32(ArgUtility.SplitBySpaceAndGet(secretNote, 1));
    }
    else
    {
      this.whichBG = secretNoteIndex <= 1000 ? 1 : 0;
      this.mailMessage = SpriteText.getStringBrokenIntoSectionsOfHeight(this.ApplyCustomFormatting(Utility.ParseGiftReveals(secretNote.Replace("@", Game1.player.name.Value))), this.width - 64 /*0x40*/, this.height - 128 /*0x80*/);
    }
    this.OnPageChange();
    this.forwardButton.visible = this.page < this.mailMessage.Count - 1;
    this.backButton.visible = this.page > 0;
    this.populateClickableComponentList();
    if (!Game1.options.SnappyMenus)
      return;
    this.snapToDefaultClickableComponent();
  }

  public virtual void OnPageChange()
  {
    this.forwardButton.visible = this.page < this.mailMessage.Count - 1;
    this.backButton.visible = this.page > 0;
    foreach (ClickableComponent clickableComponent in this.itemsToGrab)
      clickableComponent.visible = this.ShouldShowInteractable();
    if (this.acceptQuestButton != null)
      this.acceptQuestButton.visible = this.ShouldShowInteractable();
    if (!Game1.options.SnappyMenus || this.currentlySnappedComponent != null && this.currentlySnappedComponent.visible)
      return;
    this.snapToDefaultClickableComponent();
  }

  public LetterViewerMenu(string mail, string mailTitle, bool fromCollection = false)
    : base((int) Utility.getTopLeftPositionForCenteringOnScreen(1280 /*0x0500*/, 720).X, (int) Utility.getTopLeftPositionForCenteringOnScreen(1280 /*0x0500*/, 720).Y, 1280 /*0x0500*/, 720, true)
  {
    this.isFromCollection = fromCollection;
    this.mailTitle = mailTitle;
    this.isMail = true;
    Game1.playSound("shwip");
    ClickableTextureComponent textureComponent1 = new ClickableTextureComponent(new Rectangle(this.xPositionOnScreen + 32 /*0x20*/, this.yPositionOnScreen + this.height - 32 /*0x20*/ - 64 /*0x40*/, 48 /*0x30*/, 44), Game1.mouseCursors, new Rectangle(352, 495, 12, 11), 4f);
    textureComponent1.myID = 101;
    textureComponent1.rightNeighborID = 102;
    this.backButton = textureComponent1;
    ClickableTextureComponent textureComponent2 = new ClickableTextureComponent(new Rectangle(this.xPositionOnScreen + this.width - 32 /*0x20*/ - 48 /*0x30*/, this.yPositionOnScreen + this.height - 32 /*0x20*/ - 64 /*0x40*/, 48 /*0x30*/, 44), Game1.mouseCursors, new Rectangle(365, 495, 12, 11), 4f);
    textureComponent2.myID = 102;
    textureComponent2.leftNeighborID = 101;
    this.forwardButton = textureComponent2;
    this.acceptQuestButton = new ClickableComponent(new Rectangle(this.xPositionOnScreen + this.width / 2 - 128 /*0x80*/, this.yPositionOnScreen + this.height - 128 /*0x80*/, (int) Game1.dialogueFont.MeasureString(Game1.content.LoadString("Strings\\UI:AcceptQuest")).X + 24, (int) Game1.dialogueFont.MeasureString(Game1.content.LoadString("Strings\\UI:AcceptQuest")).Y + 24), "")
    {
      myID = 103,
      rightNeighborID = 102,
      leftNeighborID = 101
    };
    this.letterTexture = Game1.temporaryContent.Load<Texture2D>("LooseSprites\\letterBG");
    if (mailTitle.Equals("winter_5_2") || mailTitle.Equals("winter_12_1") || mailTitle.ContainsIgnoreCase("wizard"))
      this.whichBG = 2;
    else if (mailTitle.Equals("Sandy"))
      this.whichBG = 1;
    else if (mailTitle.Contains("Krobus"))
      this.whichBG = 3;
    else if (mailTitle.Contains("passedOut1") || mailTitle.Equals("landslideDone") || mailTitle.Equals("FizzIntro"))
      this.whichBG = 4;
    try
    {
      mail = mail.Split("[#]")[0];
      mail = mail.Replace("@", Game1.player.Name);
      mail = Dialogue.applyGenderSwitch(Game1.player.Gender, mail, true);
      mail = this.ApplyCustomFormatting(mail);
      mail = this.HandleActionCommand(mail);
      mail = this.HandleItemCommand(mail);
      bool flag = fromCollection && (Game1.season != Season.Winter || Game1.dayOfMonth < 18 || Game1.dayOfMonth > 25);
      mail = mail.Replace("%secretsanta", flag ? "???" : Utility.GetRandomWinterStarParticipant().displayName);
      if (mailTitle.Equals("winter_18"))
      {
        if (!fromCollection)
          Game1.player.mailReceived.Add("sawSecretSanta" + Game1.year.ToString());
      }
    }
    catch (Exception ex)
    {
      Game1.log.Error($"Letter '{this.mailTitle}' couldn't be parsed.", ex);
      mail = "...";
    }
    if (mailTitle == "ccBulletinThankYou" && !Game1.player.hasOrWillReceiveMail("ccBulletinThankYouReceived"))
    {
      Utility.ForEachVillager((Func<NPC, bool>) (n =>
      {
        if (!n.datable.Value)
          Game1.player.changeFriendship(500, n);
        return true;
      }));
      Game1.addMailForTomorrow("ccBulletinThankYouReceived", true);
    }
    int height = this.height - 128 /*0x80*/;
    if (this.HasInteractable())
      height = this.height - 128 /*0x80*/ - 32 /*0x20*/;
    this.mailMessage = SpriteText.getStringBrokenIntoSectionsOfHeight(mail, this.width - 64 /*0x40*/, height);
    if (this.mailMessage.Count == 0)
      this.mailMessage.Add($"[{mailTitle}]");
    this.forwardButton.visible = this.page < this.mailMessage.Count - 1;
    this.backButton.visible = this.page > 0;
    if (!Game1.options.SnappyMenus)
      return;
    this.populateClickableComponentList();
    this.snapToDefaultClickableComponent();
    if (this.mailMessage.Count > 1)
      return;
    this.backButton.myID = -100;
    this.forwardButton.myID = -100;
  }

  /// <summary>Handle the <c>%action</c> command in the mail text, if present. This runs the action(s) and return the mail text with the commands stripped.</summary>
  /// <param name="mail">The mail text to parse.</param>
  public string HandleActionCommand(string mail)
  {
    int startIndex = 0;
    while (true)
    {
      string action;
      string error;
      Exception exception;
      do
      {
        int num1 = mail.IndexOf("%action", startIndex, StringComparison.InvariantCulture);
        if (num1 >= 0)
        {
          int num2 = mail.IndexOf("%%", num1, StringComparison.InvariantCulture);
          if (num2 >= 0)
          {
            string str = mail.Substring(num1, num2 + 2 - num1);
            mail = mail.Substring(0, num1) + mail.Substring(num1 + str.Length);
            action = str.Substring("%action".Length, str.Length - "%action".Length - "%%".Length);
            startIndex = num1;
          }
          else
            goto label_5;
        }
        else
          goto label_5;
      }
      while (this.isFromCollection || TriggerActionManager.TryRunAction(action, out error, out exception));
      Game1.log.Error($"Letter '{this.mailTitle}' has invalid action command '{action}': {error}", exception);
    }
label_5:
    return mail;
  }

  /// <summary>Handle the <c>%item</c> command in the mail text, if present. This adds the matching item to the letter and return the mail text with the command stripped.</summary>
  /// <param name="mail">The mail text to parse.</param>
  public string HandleItemCommand(string mail)
  {
    int startIndex = 0;
    while (true)
    {
      string[] strArray1;
      Dictionary<string, string> cookingRecipes;
      string str1;
      do
      {
        string lower;
        do
        {
          do
          {
            do
            {
              do
              {
                do
                {
                  do
                  {
                    do
                    {
                      do
                      {
                        string str2;
                        do
                        {
                          int num1 = mail.IndexOf("%item", startIndex, StringComparison.InvariantCulture);
                          if (num1 >= 0)
                          {
                            int num2 = mail.IndexOf("%%", num1, StringComparison.InvariantCulture);
                            if (num2 >= 0)
                            {
                              string str3 = mail.Substring(num1, num2 + 2 - num1);
                              mail = mail.Substring(0, num1) + mail.Substring(num1 + str3.Length);
                              string[] strArray2 = ArgUtility.SplitBySpace(str3.Substring("%item".Length, str3.Length - "%item".Length - "%%".Length), 2);
                              str2 = strArray2[0];
                              strArray1 = strArray2.Length > 1 ? ArgUtility.SplitBySpace(strArray2[1]) : LegacyShims.EmptyArray<string>();
                              startIndex = num1;
                            }
                            else
                              goto label_69;
                          }
                          else
                            goto label_69;
                        }
                        while (this.isFromCollection);
                        lower = str2.ToLower();
                      }
                      while (lower == null);
                      switch (lower.Length)
                      {
                        case 2:
                          continue;
                        case 5:
                          switch (lower[0])
                          {
                            case 'm':
                              goto label_12;
                            case 'q':
                              goto label_13;
                            case 't':
                              goto label_11;
                            default:
                              continue;
                          }
                        case 6:
                          goto label_10;
                        case 9:
                          switch (lower[0])
                          {
                            case 'b':
                              goto label_14;
                            case 'f':
                              goto label_15;
                            default:
                              continue;
                          }
                        case 12:
                          switch (lower[0])
                          {
                            case 'i':
                              goto label_19;
                            case 's':
                              goto label_20;
                            default:
                              continue;
                          }
                        case 13:
                          goto label_17;
                        case 14:
                          goto label_18;
                        case 17:
                          goto label_16;
                        default:
                          continue;
                      }
                    }
                    while (!(lower == "id"));
                    goto label_21;
label_10:
                    if (lower == "object")
                      goto label_25;
                    continue;
label_11:
                    if (lower == "tools")
                      goto label_26;
                    continue;
label_12:;
                  }
                  while (!(lower == "money"));
                  goto label_37;
label_13:;
                }
                while (!(lower == "quest"));
                goto label_59;
label_14:;
              }
              while (!(lower == "bigobject"));
              goto label_35;
label_15:;
            }
            while (!(lower == "furniture"));
            goto label_36;
label_16:
            if (lower == "conversationtopic")
              goto label_38;
            continue;
label_17:
            if (lower == "cookingrecipe")
              goto label_40;
            continue;
label_18:
            if (lower == "craftingrecipe")
              goto label_52;
            continue;
label_19:;
          }
          while (!(lower == "itemrecovery") || Game1.player.recoveredItem == null);
          goto label_58;
label_20:;
        }
        while (!(lower == "specialorder"));
        goto label_64;
label_21:
        string itemId;
        int amount;
        if (strArray1.Length == 1)
        {
          itemId = strArray1[0];
          amount = 1;
        }
        else
        {
          int num = Game1.random.Next(strArray1.Length);
          int index = num - num % 2;
          itemId = strArray1[index];
          amount = int.Parse(strArray1[index + 1]);
        }
        this.itemsToGrab.Add(new ClickableComponent(new Rectangle(this.xPositionOnScreen + this.width / 2 - 48 /*0x30*/, this.yPositionOnScreen + this.height - 32 /*0x20*/ - 96 /*0x60*/, 96 /*0x60*/, 96 /*0x60*/), ItemRegistry.Create(itemId, amount))
        {
          myID = 104,
          leftNeighborID = 101,
          rightNeighborID = 102
        });
        this.backButton.rightNeighborID = 104;
        this.forwardButton.leftNeighborID = 104;
        continue;
label_25:
        int num3 = Game1.random.Next(strArray1.Length);
        int index1 = num3 - num3 % 2;
        this.itemsToGrab.Add(new ClickableComponent(new Rectangle(this.xPositionOnScreen + this.width / 2 - 48 /*0x30*/, this.yPositionOnScreen + this.height - 32 /*0x20*/ - 96 /*0x60*/, 96 /*0x60*/, 96 /*0x60*/), ItemRegistry.Create(strArray1[index1], int.Parse(strArray1[index1 + 1])))
        {
          myID = 104,
          leftNeighborID = 101,
          rightNeighborID = 102
        });
        this.backButton.rightNeighborID = 104;
        this.forwardButton.leftNeighborID = 104;
        continue;
label_26:
        foreach (string str4 in strArray1)
        {
          Item obj = (Item) null;
          switch (str4)
          {
            case "Axe":
            case "Hoe":
            case "Pickaxe":
              obj = ItemRegistry.Create("(T)" + str4);
              break;
            case "Can":
              obj = ItemRegistry.Create("(T)WateringCan");
              break;
            case "Scythe":
              obj = ItemRegistry.Create("(W)47");
              break;
          }
          if (obj != null)
            this.itemsToGrab.Add(new ClickableComponent(new Rectangle(this.xPositionOnScreen + this.width / 2 - 48 /*0x30*/, this.yPositionOnScreen + this.height - 32 /*0x20*/ - 96 /*0x60*/, 96 /*0x60*/, 96 /*0x60*/), obj));
        }
        continue;
label_35:
        this.itemsToGrab.Add(new ClickableComponent(new Rectangle(this.xPositionOnScreen + this.width / 2 - 48 /*0x30*/, this.yPositionOnScreen + this.height - 32 /*0x20*/ - 96 /*0x60*/, 96 /*0x60*/, 96 /*0x60*/), ItemRegistry.Create("(BC)" + Game1.random.ChooseFrom<string>((IList<string>) strArray1)))
        {
          myID = 104,
          leftNeighborID = 101,
          rightNeighborID = 102
        });
        this.backButton.rightNeighborID = 104;
        this.forwardButton.leftNeighborID = 104;
        continue;
label_36:
        this.itemsToGrab.Add(new ClickableComponent(new Rectangle(this.xPositionOnScreen + this.width / 2 - 48 /*0x30*/, this.yPositionOnScreen + this.height - 32 /*0x20*/ - 96 /*0x60*/, 96 /*0x60*/, 96 /*0x60*/), ItemRegistry.Create("(F)" + Game1.random.ChooseFrom<string>((IList<string>) strArray1)))
        {
          myID = 104,
          leftNeighborID = 101,
          rightNeighborID = 102
        });
        this.backButton.rightNeighborID = 104;
        this.forwardButton.leftNeighborID = 104;
        continue;
label_37:
        int num4 = strArray1.Length > 1 ? Game1.random.Next(Convert.ToInt32(strArray1[0]), Convert.ToInt32(strArray1[1])) : Convert.ToInt32(strArray1[0]);
        int num5 = num4 - num4 % 10;
        Game1.player.Money += num5;
        this.moneyIncluded = num5;
        continue;
label_38:
        string key1 = strArray1[0];
        int int32_1 = Convert.ToInt32(strArray1[1]);
        Game1.player.activeDialogueEvents[key1] = int32_1;
        if (key1.Equals("ElliottGone3"))
        {
          Utility.getHomeOfFarmer(Game1.player).fridge.Value.addItem(ItemRegistry.Create("(O)732"));
          continue;
        }
        continue;
label_40:
        cookingRecipes = CraftingRecipe.cookingRecipes;
        str1 = string.Join(" ", strArray1);
        if (string.IsNullOrWhiteSpace(str1))
        {
          int num6 = 1000;
          foreach (string key2 in cookingRecipes.Keys)
          {
            string[] array = ArgUtility.SplitBySpace(ArgUtility.Get(cookingRecipes[key2].Split('/'), 3));
            string str5 = ArgUtility.Get(array, 0);
            string str6 = ArgUtility.Get(array, 1);
            if (str5 == "f" && str6 == this.mailTitle.Replace("Cooking", "") && !Game1.player.cookingRecipes.ContainsKey(key2))
            {
              int int32_2 = Convert.ToInt32(array[2]);
              if (int32_2 <= num6)
              {
                num6 = int32_2;
                str1 = key2;
              }
            }
          }
        }
      }
      while (string.IsNullOrWhiteSpace(str1));
      if (cookingRecipes.ContainsKey(str1))
      {
        Game1.player.cookingRecipes.TryAdd(str1, 0);
        this.learnedRecipe = new CraftingRecipe(str1, true).DisplayName;
        this.cookingOrCrafting = Game1.content.LoadString("Strings\\UI:LearnedRecipe_cooking");
        continue;
      }
      Game1.log.Warn($"Letter '{this.mailTitle}' has unknown cooking recipe '{str1}'.");
      continue;
label_52:
      Dictionary<string, string> craftingRecipes = CraftingRecipe.craftingRecipes;
      if (craftingRecipes.ContainsKey(strArray1[0]))
      {
        this.learnedRecipe = strArray1[0];
      }
      else
      {
        string key = strArray1[0].Replace('_', ' ');
        if (craftingRecipes.ContainsKey(key))
        {
          this.learnedRecipe = key;
        }
        else
        {
          Game1.log.Warn($"Letter '{this.mailTitle}' has unknown crafting recipe '{strArray1[0]}'{(strArray1[0] != key ? $" or '{key}'" : "")}.");
          continue;
        }
      }
      Game1.player.craftingRecipes.TryAdd(this.learnedRecipe, 0);
      this.learnedRecipe = new CraftingRecipe(this.learnedRecipe, false).DisplayName;
      this.cookingOrCrafting = Game1.content.LoadString("Strings\\UI:LearnedRecipe_crafting");
      continue;
label_58:
      Item recoveredItem = Game1.player.recoveredItem;
      Game1.player.recoveredItem = (Item) null;
      this.itemsToGrab.Add(new ClickableComponent(new Rectangle(this.xPositionOnScreen + this.width / 2 - 48 /*0x30*/, this.yPositionOnScreen + this.height - 32 /*0x20*/ - 96 /*0x60*/, 96 /*0x60*/, 96 /*0x60*/), recoveredItem)
      {
        myID = 104,
        leftNeighborID = 101,
        rightNeighborID = 102
      });
      this.backButton.rightNeighborID = 104;
      this.forwardButton.leftNeighborID = 104;
      continue;
label_59:
      this.questID = strArray1[0];
      if (strArray1.Length > 1)
      {
        if (!Game1.player.mailReceived.Contains("NOQUEST_" + this.questID))
          Game1.player.addQuest(this.questID);
        this.questID = (string) null;
      }
      this.backButton.rightNeighborID = 103;
      this.forwardButton.leftNeighborID = 103;
      continue;
label_64:
      this.specialOrderId = strArray1[0];
      bool flag;
      if (ArgUtility.TryGetBool(strArray1, 1, out flag, out string _, "bool addImmediately") & flag)
      {
        if (!Game1.player.mailReceived.Contains("NOSPECIALORDER_" + this.specialOrderId))
          Game1.player.team.AddSpecialOrder(this.specialOrderId);
        this.specialOrderId = (string) null;
      }
      this.backButton.rightNeighborID = 103;
      this.forwardButton.leftNeighborID = 103;
    }
label_69:
    return mail;
  }

  public virtual string ApplyCustomFormatting(string text)
  {
    text = Dialogue.applyGenderSwitchBlocks(Game1.player.Gender, text);
    for (int startIndex = text.IndexOf("["); startIndex >= 0; startIndex = text.IndexOf("[", startIndex + 1))
    {
      int num = text.IndexOf("]", startIndex);
      if (num >= 0)
      {
        bool flag = false;
        try
        {
          string[] strArray1 = ArgUtility.SplitBySpace(text.Substring(startIndex + 1, num - startIndex - 1));
          switch (strArray1[0])
          {
            case "letterbg":
              switch (strArray1.Length)
              {
                case 2:
                  this.whichBG = int.Parse(strArray1[1]);
                  break;
                case 3:
                  this.usingCustomBackground = true;
                  this.letterTexture = Game1.temporaryContent.Load<Texture2D>(strArray1[1]);
                  this.whichBG = int.Parse(strArray1[2]);
                  break;
              }
              flag = true;
              break;
            case "textcolor":
              string lower = strArray1[1].ToLower();
              string[] strArray2 = new string[10]
              {
                "black",
                "blue",
                "red",
                "purple",
                "white",
                "orange",
                "green",
                "cyan",
                "gray",
                "jojablue"
              };
              this.customTextColor = new Color?();
              for (int index = 0; index < strArray2.Length; ++index)
              {
                if (lower == strArray2[index])
                {
                  this.customTextColor = new Color?(SpriteText.getColorFromIndex(index));
                  break;
                }
              }
              flag = true;
              break;
          }
        }
        catch (Exception ex)
        {
        }
        if (flag)
        {
          text = text.Remove(startIndex, num - startIndex + 1);
          --startIndex;
        }
      }
    }
    return text;
  }

  public override void snapToDefaultClickableComponent()
  {
    if (this.HasQuestOrSpecialOrder && this.ShouldShowInteractable())
      this.currentlySnappedComponent = this.getComponentWithID(103);
    else if (this.itemsToGrab.Count > 0 && this.ShouldShowInteractable())
      this.currentlySnappedComponent = this.getComponentWithID(104);
    else if (this.currentlySnappedComponent == null || this.currentlySnappedComponent != this.backButton && this.currentlySnappedComponent != this.forwardButton)
      this.currentlySnappedComponent = (ClickableComponent) this.forwardButton;
    this.snapCursorToCurrentSnappedComponent();
  }

  /// <inheritdoc />
  public override void gameWindowSizeChanged(Rectangle oldBounds, Rectangle newBounds)
  {
    this.xPositionOnScreen = (int) Utility.getTopLeftPositionForCenteringOnScreen(1280 /*0x0500*/, 720).X;
    this.yPositionOnScreen = (int) Utility.getTopLeftPositionForCenteringOnScreen(1280 /*0x0500*/, 720).Y;
    ClickableTextureComponent textureComponent1 = new ClickableTextureComponent(new Rectangle(this.xPositionOnScreen + 32 /*0x20*/, this.yPositionOnScreen + this.height - 32 /*0x20*/ - 64 /*0x40*/, 48 /*0x30*/, 44), Game1.mouseCursors, new Rectangle(352, 495, 12, 11), 4f);
    textureComponent1.myID = 101;
    textureComponent1.rightNeighborID = 102;
    this.backButton = textureComponent1;
    ClickableTextureComponent textureComponent2 = new ClickableTextureComponent(new Rectangle(this.xPositionOnScreen + this.width - 32 /*0x20*/ - 48 /*0x30*/, this.yPositionOnScreen + this.height - 32 /*0x20*/ - 64 /*0x40*/, 48 /*0x30*/, 44), Game1.mouseCursors, new Rectangle(365, 495, 12, 11), 4f);
    textureComponent2.myID = 102;
    textureComponent2.leftNeighborID = 101;
    this.forwardButton = textureComponent2;
    this.acceptQuestButton = new ClickableComponent(new Rectangle(this.xPositionOnScreen + this.width / 2 - 128 /*0x80*/, this.yPositionOnScreen + this.height - 128 /*0x80*/, (int) Game1.dialogueFont.MeasureString(Game1.content.LoadString("Strings\\UI:AcceptQuest")).X + 24, (int) Game1.dialogueFont.MeasureString(Game1.content.LoadString("Strings\\UI:AcceptQuest")).Y + 24), "")
    {
      myID = 103,
      rightNeighborID = 102,
      leftNeighborID = 101
    };
    foreach (ClickableComponent clickableComponent in this.itemsToGrab)
      clickableComponent.bounds = new Rectangle(this.xPositionOnScreen + this.width / 2 - 48 /*0x30*/, this.yPositionOnScreen + this.height - 32 /*0x20*/ - 96 /*0x60*/, 96 /*0x60*/, 96 /*0x60*/);
  }

  /// <inheritdoc />
  public override void receiveKeyPress(Keys key)
  {
    if (key == Keys.None)
      return;
    if (Game1.options.doesInputListContain(Game1.options.menuButton, key) && this.readyToClose())
      this.exitThisMenu(this.ShouldPlayExitSound());
    else
      base.receiveKeyPress(key);
  }

  /// <inheritdoc />
  public override void receiveGamePadButton(Buttons button)
  {
    base.receiveGamePadButton(button);
    switch (button)
    {
      case Buttons.B:
        if (!this.isFromCollection)
          break;
        this.exitThisMenu(false);
        break;
      case Buttons.RightTrigger:
        if (this.page >= this.mailMessage.Count - 1)
          break;
        ++this.page;
        Game1.playSound("shwip");
        this.OnPageChange();
        break;
      case Buttons.LeftTrigger:
        if (this.page <= 0)
          break;
        --this.page;
        Game1.playSound("shwip");
        this.OnPageChange();
        break;
    }
  }

  /// <inheritdoc />
  public override void receiveLeftClick(int x, int y, bool playSound = true)
  {
    if ((double) this.scale < 1.0)
      return;
    if (this.upperRightCloseButton != null && this.readyToClose() && this.upperRightCloseButton.containsPoint(x, y))
    {
      if (playSound)
        Game1.playSound("bigDeSelect");
      if (!this.isFromCollection)
        this.exitThisMenu(this.ShouldPlayExitSound());
      else
        this.destroy = true;
    }
    if (Game1.activeClickableMenu == null && Game1.currentMinigame == null)
    {
      this.unload();
    }
    else
    {
      if (this.ShouldShowInteractable())
      {
        for (int index = 0; index < this.itemsToGrab.Count; ++index)
        {
          ClickableComponent clickableComponent = this.itemsToGrab[index];
          if (clickableComponent.containsPoint(x, y) && clickableComponent.item != null)
          {
            Game1.playSound("coin");
            Game1.player.addItemByMenuIfNecessary(clickableComponent.item);
            clickableComponent.item = (Item) null;
            if (this.itemsToGrab.Count <= 1)
              return;
            this.itemsToGrab.RemoveAt(index);
            return;
          }
        }
      }
      if (this.backButton.containsPoint(x, y) && this.page > 0)
      {
        --this.page;
        Game1.playSound("shwip");
        this.OnPageChange();
      }
      else if (this.forwardButton.containsPoint(x, y) && this.page < this.mailMessage.Count - 1)
      {
        ++this.page;
        Game1.playSound("shwip");
        this.OnPageChange();
      }
      else if (this.ShouldShowInteractable() && this.acceptQuestButton != null && this.acceptQuestButton.containsPoint(x, y))
        this.AcceptQuest();
      else if (this.isWithinBounds(x, y))
      {
        if (this.page < this.mailMessage.Count - 1)
        {
          ++this.page;
          Game1.playSound("shwip");
          this.OnPageChange();
        }
        else if (!this.isMail)
        {
          this.exitThisMenuNoSound();
          Game1.playSound("shwip");
        }
        else
        {
          if (!this.isFromCollection)
            return;
          this.destroy = true;
        }
      }
      else
      {
        if (this.itemsLeftToGrab())
          return;
        if (!this.isFromCollection)
        {
          this.exitThisMenuNoSound();
          Game1.playSound("shwip");
        }
        else
          this.destroy = true;
      }
    }
  }

  public virtual bool ShouldPlayExitSound()
  {
    return !this.HasQuestOrSpecialOrder && !this.isFromCollection;
  }

  public bool itemsLeftToGrab()
  {
    foreach (ClickableComponent clickableComponent in this.itemsToGrab)
    {
      if (clickableComponent.item != null)
        return true;
    }
    return false;
  }

  /// <summary>Add the attached quest or special order to the player.</summary>
  public void AcceptQuest()
  {
    if (this.questID != null)
    {
      Game1.player.addQuest(this.questID);
      if (this.questID == "20")
        MineShaft.CheckForQiChallengeCompletion();
      this.questID = (string) null;
      Game1.playSound("newArtifact");
    }
    else
    {
      if (this.specialOrderId == null)
        return;
      Game1.player.team.AddSpecialOrder(this.specialOrderId);
      this.specialOrderId = (string) null;
      Game1.playSound("newArtifact");
    }
  }

  /// <inheritdoc />
  public override void performHoverAction(int x, int y)
  {
    base.performHoverAction(x, y);
    if (this.ShouldShowInteractable())
    {
      foreach (ClickableComponent clickableComponent in this.itemsToGrab)
        clickableComponent.scale = !clickableComponent.containsPoint(x, y) ? Math.Max(1f, clickableComponent.scale - 0.03f) : Math.Min(clickableComponent.scale + 0.03f, 1.1f);
    }
    this.backButton.tryHover(x, y, 0.6f);
    this.forwardButton.tryHover(x, y, 0.6f);
    if (!this.ShouldShowInteractable() || !this.HasQuestOrSpecialOrder)
      return;
    float scale = this.acceptQuestButton.scale;
    this.acceptQuestButton.scale = this.acceptQuestButton.bounds.Contains(x, y) ? 1.5f : 1f;
    if ((double) this.acceptQuestButton.scale <= (double) scale)
      return;
    Game1.playSound("Cowboy_gunshot");
  }

  /// <inheritdoc />
  public override void update(GameTime time)
  {
    base.update(time);
    this.forwardButton.visible = this.page < this.mailMessage.Count - 1;
    this.backButton.visible = this.page > 0;
    if ((double) this.scale < 1.0)
    {
      this.scale += (float) time.ElapsedGameTime.Milliseconds * (3f / 1000f);
      if ((double) this.scale >= 1.0)
        this.scale = 1f;
    }
    if (this.page >= this.mailMessage.Count - 1 || this.forwardButton.containsPoint(Game1.getOldMouseX(), Game1.getOldMouseY()))
      return;
    this.forwardButton.scale = (float) (4.0 + Math.Sin((double) time.TotalGameTime.Milliseconds / (64.0 * Math.PI)) / 1.5);
  }

  public virtual Color? getTextColor()
  {
    if (this.customTextColor.HasValue)
      return new Color?(this.customTextColor.Value);
    if (this.usingCustomBackground)
      return new Color?();
    switch (this.whichBG)
    {
      case 1:
        return new Color?(SpriteText.color_Gray);
      case 2:
        return new Color?(SpriteText.color_Cyan);
      case 3:
        return new Color?(SpriteText.color_White);
      case 4:
        return new Color?(SpriteText.color_JojaBlue);
      default:
        return new Color?();
    }
  }

  /// <inheritdoc />
  public override void draw(SpriteBatch b)
  {
    if (!Game1.options.showClearBackgrounds)
      b.Draw(Game1.fadeToBlackRect, Game1.graphics.GraphicsDevice.Viewport.Bounds, Color.Black * 0.4f);
    b.Draw(this.letterTexture, new Vector2((float) (this.xPositionOnScreen + this.width / 2), (float) (this.yPositionOnScreen + this.height / 2)), new Rectangle?(new Rectangle(this.whichBG % 4 * 320, this.whichBG >= 4 ? 204 + (this.whichBG / 4 - 1) * 180 : 0, 320, 180)), Color.White, 0.0f, new Vector2(160f, 90f), 4f * this.scale, SpriteEffects.None, 0.86f);
    if ((double) this.scale == 1.0)
    {
      if (this.secretNoteImage != -1)
      {
        b.Draw(this.secretNoteImageTexture, new Vector2((float) (this.xPositionOnScreen + this.width / 2 - 128 /*0x80*/ - 4), (float) (this.yPositionOnScreen + this.height / 2 - 128 /*0x80*/ + 8)), new Rectangle?(new Rectangle(this.secretNoteImage * 64 /*0x40*/ % this.secretNoteImageTexture.Width, this.secretNoteImage * 64 /*0x40*/ / this.secretNoteImageTexture.Width * 64 /*0x40*/, 64 /*0x40*/, 64 /*0x40*/)), Color.Black * 0.4f, 0.0f, Vector2.Zero, 4f, SpriteEffects.None, 0.865f);
        b.Draw(this.secretNoteImageTexture, new Vector2((float) (this.xPositionOnScreen + this.width / 2 - 128 /*0x80*/), (float) (this.yPositionOnScreen + this.height / 2 - 128 /*0x80*/)), new Rectangle?(new Rectangle(this.secretNoteImage * 64 /*0x40*/ % this.secretNoteImageTexture.Width, this.secretNoteImage * 64 /*0x40*/ / this.secretNoteImageTexture.Width * 64 /*0x40*/, 64 /*0x40*/, 64 /*0x40*/)), Color.White, 0.0f, Vector2.Zero, 4f, SpriteEffects.None, 0.865f);
        b.Draw(this.secretNoteImageTexture, new Vector2((float) (this.xPositionOnScreen + this.width / 2 - 40), (float) (this.yPositionOnScreen + this.height / 2 - 192 /*0xC0*/)), new Rectangle?(new Rectangle(193, 65, 14, 21)), Color.White, 0.0f, Vector2.Zero, 4f, SpriteEffects.None, 0.867f);
      }
      else
        SpriteText.drawString(b, this.mailMessage[this.page], this.xPositionOnScreen + 32 /*0x20*/, this.yPositionOnScreen + 32 /*0x20*/, width: this.width - 64 /*0x40*/, alpha: 0.75f, layerDepth: 0.865f, color: this.getTextColor());
      if (this.ShouldShowInteractable())
      {
        using (List<ClickableComponent>.Enumerator enumerator = this.itemsToGrab.GetEnumerator())
        {
          if (enumerator.MoveNext())
          {
            ClickableComponent current = enumerator.Current;
            b.Draw(this.letterTexture, current.bounds, new Rectangle?(new Rectangle(this.whichBG * 24, 180, 24, 24)), Color.White);
            current.item?.drawInMenu(b, new Vector2((float) (current.bounds.X + 16 /*0x10*/), (float) (current.bounds.Y + 16 /*0x10*/)), current.scale);
          }
        }
        if (this.moneyIncluded > 0)
        {
          string s = Game1.content.LoadString("Strings\\UI:LetterViewer_MoneyIncluded", (object) this.moneyIncluded);
          SpriteText.drawString(b, s, this.xPositionOnScreen + this.width / 2 - SpriteText.getWidthOfString(s) / 2, this.yPositionOnScreen + this.height - 96 /*0x60*/, height: 9999, alpha: 0.75f, layerDepth: 0.865f, color: this.getTextColor());
        }
        else
        {
          string learnedRecipe = this.learnedRecipe;
          if ((learnedRecipe != null ? (learnedRecipe.Length > 0 ? 1 : 0) : 0) != 0)
          {
            string s = Game1.content.LoadString("Strings\\UI:LetterViewer_LearnedRecipe", (object) this.cookingOrCrafting);
            SpriteText.drawStringHorizontallyCenteredAt(b, s, this.xPositionOnScreen + this.width / 2, this.yPositionOnScreen + this.height - 32 /*0x20*/ - SpriteText.getHeightOfString(s) * 2, height: 9999, alpha: 0.65f, layerDepth: 0.865f, color: this.getTextColor());
            SpriteText.drawStringHorizontallyCenteredAt(b, Game1.content.LoadString("Strings\\UI:LetterViewer_LearnedRecipeName", (object) this.learnedRecipe), this.xPositionOnScreen + this.width / 2, this.yPositionOnScreen + this.height - 32 /*0x20*/ - SpriteText.getHeightOfString("t"), height: 9999, alpha: 0.9f, layerDepth: 0.865f, color: this.getTextColor());
          }
        }
      }
      base.draw(b);
      this.forwardButton.draw(b);
      this.backButton.draw(b);
      if (this.ShouldShowInteractable() && this.HasQuestOrSpecialOrder)
      {
        IClickableMenu.drawTextureBox(b, Game1.mouseCursors, new Rectangle(403, 373, 9, 9), this.acceptQuestButton.bounds.X, this.acceptQuestButton.bounds.Y, this.acceptQuestButton.bounds.Width, this.acceptQuestButton.bounds.Height, (double) this.acceptQuestButton.scale > 1.0 ? Color.LightPink : Color.White, 4f * this.acceptQuestButton.scale);
        Utility.drawTextWithShadow(b, Game1.content.LoadString("Strings\\UI:AcceptQuest"), Game1.dialogueFont, new Vector2((float) (this.acceptQuestButton.bounds.X + 12), (float) (this.acceptQuestButton.bounds.Y + (LocalizedContentManager.CurrentLanguageLatin ? 16 /*0x10*/ : 12))), Game1.textColor);
      }
    }
    if (Game1.options.SnappyMenus && (double) this.scale < 1.0 || Game1.options.SnappyMenus && !this.forwardButton.visible && !this.backButton.visible && !this.HasQuestOrSpecialOrder && !this.itemsLeftToGrab())
      return;
    this.drawMouse(b);
  }

  public virtual bool ShouldShowInteractable()
  {
    return this.HasInteractable() && this.page == this.mailMessage.Count - 1;
  }

  public virtual bool HasInteractable()
  {
    if (this.isFromCollection)
      return false;
    if (this.HasQuestOrSpecialOrder || this.moneyIncluded > 0 || this.itemsToGrab.Count > 0)
      return true;
    string learnedRecipe = this.learnedRecipe;
    return (learnedRecipe != null ? (learnedRecipe.Length > 0 ? 1 : 0) : 0) != 0;
  }

  public void unload()
  {
  }

  /// <inheritdoc />
  protected override void cleanupBeforeExit()
  {
    if (this.HasQuestOrSpecialOrder)
      this.AcceptQuest();
    if (this.itemsLeftToGrab())
    {
      List<Item> itemsToAdd = new List<Item>();
      foreach (ClickableComponent clickableComponent in this.itemsToGrab)
      {
        if (clickableComponent.item != null)
          itemsToAdd.Add(clickableComponent.item);
      }
      this.itemsToGrab.Clear();
      if (itemsToAdd.Count > 0)
      {
        Game1.playSound("coin");
        Game1.player.addItemsByMenuIfNecessary(itemsToAdd);
      }
    }
    if (this.isFromCollection)
    {
      this.destroy = true;
      Game1.oldKBState = Game1.GetKeyboardState();
      Game1.oldMouseState = Game1.input.GetMouseState();
      Game1.oldPadState = Game1.input.GetGamePadState();
    }
    base.cleanupBeforeExit();
  }

  /// <inheritdoc />
  public override void receiveRightClick(int x, int y, bool playSound = true)
  {
    if (this.isFromCollection)
      this.destroy = true;
    else
      this.receiveLeftClick(x, y, playSound);
  }
}
