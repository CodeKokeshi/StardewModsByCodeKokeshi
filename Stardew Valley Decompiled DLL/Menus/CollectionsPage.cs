// Decompiled with JetBrains decompiler
// Type: StardewValley.Menus.CollectionsPage
// Assembly: Stardew Valley, Version=1.6.15.24356, Culture=neutral, PublicKeyToken=null
// MVID: AA1F513A-94F0-4EF6-A35F-2D5B4A3721BF
// Assembly location: D:\SteamLibrary\steamapps\common\Stardew Valley\Stardew Valley.dll
// XML documentation location: D:\SteamLibrary\steamapps\common\Stardew Valley\Stardew Valley.xml

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Netcode;
using StardewValley.Extensions;
using StardewValley.GameData.Objects;
using StardewValley.ItemTypeDefinitions;
using StardewValley.Locations;
using System;
using System.Collections.Generic;
using System.Linq;

#nullable disable
namespace StardewValley.Menus;

public class CollectionsPage : IClickableMenu
{
  public const int region_sideTabShipped = 7001;
  public const int region_sideTabFish = 7002;
  public const int region_sideTabArtifacts = 7003;
  public const int region_sideTabMinerals = 7004;
  public const int region_sideTabCooking = 7005;
  public const int region_sideTabAchivements = 7006;
  public const int region_sideTabSecretNotes = 7007;
  public const int region_sideTabLetters = 7008;
  public const int region_forwardButton = 707;
  public const int region_backButton = 706;
  public static int widthToMoveActiveTab = 8;
  public const int organicsTab = 0;
  public const int fishTab = 1;
  public const int archaeologyTab = 2;
  public const int mineralsTab = 3;
  public const int cookingTab = 4;
  public const int achievementsTab = 5;
  public const int secretNotesTab = 6;
  public const int lettersTab = 7;
  public const int distanceFromMenuBottomBeforeNewPage = 128 /*0x80*/;
  private string hoverText = "";
  public ClickableTextureComponent backButton;
  public ClickableTextureComponent forwardButton;
  public Dictionary<int, ClickableTextureComponent> sideTabs = new Dictionary<int, ClickableTextureComponent>();
  public int currentTab;
  public int currentPage;
  public int secretNoteImage = -1;
  public Dictionary<int, List<List<ClickableTextureComponent>>> collections = new Dictionary<int, List<List<ClickableTextureComponent>>>();
  public Dictionary<int, string> secretNotesData;
  public Texture2D secretNoteImageTexture;
  public LetterViewerMenu letterviewerSubMenu;
  private Item hoverItem;
  private CraftingRecipe hoverCraftingRecipe;
  private int value;

  public CollectionsPage(int x, int y, int width, int height)
    : base(x, y, width, height)
  {
    Dictionary<int, ClickableTextureComponent> sideTabs1 = this.sideTabs;
    int num1 = 0;
    ClickableTextureComponent textureComponent1 = new ClickableTextureComponent(num1.ToString() ?? "", new Rectangle(this.xPositionOnScreen - 48 /*0x30*/ + CollectionsPage.widthToMoveActiveTab, this.yPositionOnScreen + 64 /*0x40*/ * (2 + this.sideTabs.Count), 64 /*0x40*/, 64 /*0x40*/), "", Game1.content.LoadString("Strings\\UI:Collections_Shipped"), Game1.mouseCursors, new Rectangle(640, 80 /*0x50*/, 16 /*0x10*/, 16 /*0x10*/), 4f);
    textureComponent1.myID = 7001;
    textureComponent1.downNeighborID = -99998;
    textureComponent1.rightNeighborID = 0;
    sideTabs1.Add(0, textureComponent1);
    this.collections.Add(0, new List<List<ClickableTextureComponent>>());
    Dictionary<int, ClickableTextureComponent> sideTabs2 = this.sideTabs;
    num1 = 1;
    ClickableTextureComponent textureComponent2 = new ClickableTextureComponent(num1.ToString() ?? "", new Rectangle(this.xPositionOnScreen - 48 /*0x30*/, this.yPositionOnScreen + 64 /*0x40*/ * (2 + this.sideTabs.Count), 64 /*0x40*/, 64 /*0x40*/), "", Game1.content.LoadString("Strings\\UI:Collections_Fish"), Game1.mouseCursors, new Rectangle(640, 64 /*0x40*/, 16 /*0x10*/, 16 /*0x10*/), 4f);
    textureComponent2.myID = 7002;
    textureComponent2.upNeighborID = -99998;
    textureComponent2.downNeighborID = -99998;
    textureComponent2.rightNeighborID = 0;
    sideTabs2.Add(1, textureComponent2);
    this.collections.Add(1, new List<List<ClickableTextureComponent>>());
    Dictionary<int, ClickableTextureComponent> sideTabs3 = this.sideTabs;
    num1 = 2;
    ClickableTextureComponent textureComponent3 = new ClickableTextureComponent(num1.ToString() ?? "", new Rectangle(this.xPositionOnScreen - 48 /*0x30*/, this.yPositionOnScreen + 64 /*0x40*/ * (2 + this.sideTabs.Count), 64 /*0x40*/, 64 /*0x40*/), "", Game1.content.LoadString("Strings\\UI:Collections_Artifacts"), Game1.mouseCursors, new Rectangle(656, 64 /*0x40*/, 16 /*0x10*/, 16 /*0x10*/), 4f);
    textureComponent3.myID = 7003;
    textureComponent3.upNeighborID = -99998;
    textureComponent3.downNeighborID = -99998;
    textureComponent3.rightNeighborID = 0;
    sideTabs3.Add(2, textureComponent3);
    this.collections.Add(2, new List<List<ClickableTextureComponent>>());
    Dictionary<int, ClickableTextureComponent> sideTabs4 = this.sideTabs;
    num1 = 3;
    ClickableTextureComponent textureComponent4 = new ClickableTextureComponent(num1.ToString() ?? "", new Rectangle(this.xPositionOnScreen - 48 /*0x30*/, this.yPositionOnScreen + 64 /*0x40*/ * (2 + this.sideTabs.Count), 64 /*0x40*/, 64 /*0x40*/), "", Game1.content.LoadString("Strings\\UI:Collections_Minerals"), Game1.mouseCursors, new Rectangle(672, 64 /*0x40*/, 16 /*0x10*/, 16 /*0x10*/), 4f);
    textureComponent4.myID = 7004;
    textureComponent4.upNeighborID = -99998;
    textureComponent4.downNeighborID = -99998;
    textureComponent4.rightNeighborID = 0;
    sideTabs4.Add(3, textureComponent4);
    this.collections.Add(3, new List<List<ClickableTextureComponent>>());
    Dictionary<int, ClickableTextureComponent> sideTabs5 = this.sideTabs;
    num1 = 4;
    ClickableTextureComponent textureComponent5 = new ClickableTextureComponent(num1.ToString() ?? "", new Rectangle(this.xPositionOnScreen - 48 /*0x30*/, this.yPositionOnScreen + 64 /*0x40*/ * (2 + this.sideTabs.Count), 64 /*0x40*/, 64 /*0x40*/), "", Game1.content.LoadString("Strings\\UI:Collections_Cooking"), Game1.mouseCursors, new Rectangle(688, 64 /*0x40*/, 16 /*0x10*/, 16 /*0x10*/), 4f);
    textureComponent5.myID = 7005;
    textureComponent5.upNeighborID = -99998;
    textureComponent5.downNeighborID = -99998;
    textureComponent5.rightNeighborID = 0;
    sideTabs5.Add(4, textureComponent5);
    this.collections.Add(4, new List<List<ClickableTextureComponent>>());
    Dictionary<int, ClickableTextureComponent> sideTabs6 = this.sideTabs;
    num1 = 5;
    ClickableTextureComponent textureComponent6 = new ClickableTextureComponent(num1.ToString() ?? "", new Rectangle(this.xPositionOnScreen - 48 /*0x30*/, this.yPositionOnScreen + 64 /*0x40*/ * (2 + this.sideTabs.Count), 64 /*0x40*/, 64 /*0x40*/), "", Game1.content.LoadString("Strings\\UI:Collections_Achievements"), Game1.mouseCursors, new Rectangle(656, 80 /*0x50*/, 16 /*0x10*/, 16 /*0x10*/), 4f);
    textureComponent6.myID = 7006;
    textureComponent6.upNeighborID = 7005;
    textureComponent6.downNeighborID = -99998;
    textureComponent6.rightNeighborID = 0;
    sideTabs6.Add(5, textureComponent6);
    this.collections.Add(5, new List<List<ClickableTextureComponent>>());
    Dictionary<int, ClickableTextureComponent> sideTabs7 = this.sideTabs;
    num1 = 7;
    ClickableTextureComponent textureComponent7 = new ClickableTextureComponent(num1.ToString() ?? "", new Rectangle(this.xPositionOnScreen - 48 /*0x30*/, this.yPositionOnScreen + 64 /*0x40*/ * (2 + this.sideTabs.Count), 64 /*0x40*/, 64 /*0x40*/), "", Game1.content.LoadString("Strings\\UI:Collections_Letters"), Game1.mouseCursors, new Rectangle(688, 80 /*0x50*/, 16 /*0x10*/, 16 /*0x10*/), 4f);
    textureComponent7.myID = 7008;
    textureComponent7.upNeighborID = -99998;
    textureComponent7.downNeighborID = -99998;
    textureComponent7.rightNeighborID = 0;
    sideTabs7.Add(7, textureComponent7);
    this.collections.Add(7, new List<List<ClickableTextureComponent>>());
    if (Game1.player.secretNotesSeen.Count > 0)
    {
      Dictionary<int, ClickableTextureComponent> sideTabs8 = this.sideTabs;
      num1 = 6;
      ClickableTextureComponent textureComponent8 = new ClickableTextureComponent(num1.ToString() ?? "", new Rectangle(this.xPositionOnScreen - 48 /*0x30*/, this.yPositionOnScreen + 64 /*0x40*/ * (2 + this.sideTabs.Count), 64 /*0x40*/, 64 /*0x40*/), "", Game1.content.LoadString("Strings\\UI:Collections_SecretNotes"), Game1.mouseCursors, new Rectangle(672, 80 /*0x50*/, 16 /*0x10*/, 16 /*0x10*/), 4f);
      textureComponent8.myID = 7007;
      textureComponent8.upNeighborID = -99998;
      textureComponent8.rightNeighborID = 0;
      sideTabs8.Add(6, textureComponent8);
      this.collections.Add(6, new List<List<ClickableTextureComponent>>());
    }
    this.sideTabs[0].upNeighborID = -1;
    this.sideTabs[0].upNeighborImmutable = true;
    int key1 = 0;
    int num2 = 0;
    foreach (int key2 in this.sideTabs.Keys)
    {
      if (this.sideTabs[key2].bounds.Y > num2)
      {
        num2 = this.sideTabs[key2].bounds.Y;
        key1 = key2;
      }
    }
    this.sideTabs[key1].downNeighborID = -1;
    this.sideTabs[key1].downNeighborImmutable = true;
    CollectionsPage.widthToMoveActiveTab = 8;
    ClickableTextureComponent textureComponent9 = new ClickableTextureComponent(new Rectangle(this.xPositionOnScreen + 48 /*0x30*/, this.yPositionOnScreen + height - 80 /*0x50*/, 48 /*0x30*/, 44), Game1.mouseCursors, new Rectangle(352, 495, 12, 11), 4f);
    textureComponent9.myID = 706;
    textureComponent9.rightNeighborID = -7777;
    this.backButton = textureComponent9;
    ClickableTextureComponent textureComponent10 = new ClickableTextureComponent(new Rectangle(this.xPositionOnScreen + width - 32 /*0x20*/ - 60, this.yPositionOnScreen + height - 80 /*0x50*/, 48 /*0x30*/, 44), Game1.mouseCursors, new Rectangle(365, 495, 12, 11), 4f);
    textureComponent10.myID = 707;
    textureComponent10.leftNeighborID = -7777;
    this.forwardButton = textureComponent10;
    int[] numArray = new int[8];
    int num3 = this.xPositionOnScreen + IClickableMenu.borderWidth + IClickableMenu.spaceToClearSideBorder;
    int num4 = this.yPositionOnScreen + IClickableMenu.borderWidth + IClickableMenu.spaceToClearTopBorder - 16 /*0x10*/;
    int num5 = 10;
    List<ParsedItemData> parsedItemDataList1 = new List<ParsedItemData>((IEnumerable<ParsedItemData>) ItemRegistry.GetObjectTypeDefinition().GetAllData().OrderBy<ParsedItemData, string>((Func<ParsedItemData, string>) (entry => entry.TextureName)).ThenBy<ParsedItemData, int>((Func<ParsedItemData, int>) (entry => entry.SpriteIndex)));
    List<ParsedItemData> parsedItemDataList2 = new List<ParsedItemData>();
    for (int index = parsedItemDataList1.Count - 1; index >= 0; --index)
    {
      string internalName = parsedItemDataList1[index].InternalName;
      if (internalName.Equals("Wine") || internalName.Equals("Pickles") || internalName.Equals("Jelly") || internalName.Equals("Juice"))
      {
        parsedItemDataList2.Add(parsedItemDataList1[index]);
        parsedItemDataList1.RemoveAt(index);
      }
      if (parsedItemDataList2.Count == 4)
        break;
    }
    parsedItemDataList2.Sort((Comparison<ParsedItemData>) ((a, b) => a.InternalName.CompareTo(b.InternalName)));
    parsedItemDataList1.Insert(278, parsedItemDataList2[2]);
    parsedItemDataList1.Insert(279, parsedItemDataList2[0]);
    parsedItemDataList1.Insert(283, parsedItemDataList2[3]);
    parsedItemDataList1.Insert(284, parsedItemDataList2[1]);
    foreach (ParsedItemData parsedItemData in parsedItemDataList1)
    {
      string itemId = parsedItemData.ItemId;
      string objectType = parsedItemData.ObjectType;
      bool drawShadow = false;
      bool flag = false;
      int key3;
      switch (objectType)
      {
        case "Arch":
          key3 = 2;
          drawShadow = LibraryMuseum.HasDonatedArtifact(itemId);
          break;
        case "Fish":
          if (!(parsedItemData.RawData is ObjectData rawData) || !rawData.ExcludeFromFishingCollection)
          {
            key3 = 1;
            drawShadow = Game1.player.fishCaught.ContainsKey(parsedItemData.QualifiedItemId);
            break;
          }
          continue;
        case "Minerals":
          key3 = 3;
          drawShadow = LibraryMuseum.HasDonatedArtifact(itemId);
          break;
        default:
          if (parsedItemData.Category != -2)
          {
            if (objectType == "Cooking" || parsedItemData.Category == -7)
            {
              key3 = 4;
              string key4 = parsedItemData.InternalName;
              if (key4 != null)
              {
                switch (key4.Length)
                {
                  case 6:
                    if (key4 == "Cookie")
                    {
                      key4 = "Cookies";
                      break;
                    }
                    break;
                  case 13:
                    if (key4 == "Cheese Cauli.")
                    {
                      key4 = "Cheese Cauliflower";
                      break;
                    }
                    break;
                  case 15:
                    switch (key4[0])
                    {
                      case 'C':
                        if (key4 == "Cranberry Sauce")
                        {
                          key4 = "Cran. Sauce";
                          break;
                        }
                        break;
                      case 'D':
                        if (key4 == "Dish O' The Sea")
                        {
                          key4 = "Dish o' The Sea";
                          break;
                        }
                        break;
                    }
                    break;
                  case 16 /*0x10*/:
                    if (key4 == "Vegetable Medley")
                    {
                      key4 = "Vegetable Stew";
                      break;
                    }
                    break;
                  case 17:
                    if (key4 == "Eggplant Parmesan")
                    {
                      key4 = "Eggplant Parm.";
                      break;
                    }
                    break;
                  case 18:
                    if (key4 == "Cheese Cauliflower")
                    {
                      key4 = "Cheese Cauli.";
                      break;
                    }
                    break;
                }
              }
              if (Game1.player.recipesCooked.ContainsKey(itemId))
                drawShadow = true;
              else if (Game1.player.cookingRecipes.ContainsKey(key4))
                flag = true;
              if (itemId == "217" || itemId == "772" || itemId == "773" || itemId == "279" || itemId == "873")
                continue;
              break;
            }
            if (StardewValley.Object.isPotentialBasicShipped(itemId, parsedItemData.Category, parsedItemData.ObjectType))
            {
              key3 = 0;
              drawShadow = Game1.player.basicShipped.ContainsKey(itemId);
              break;
            }
            continue;
          }
          goto case "Minerals";
      }
      int x1 = num3 + numArray[key3] % num5 * 68;
      int y1 = num4 + numArray[key3] / num5 * 68;
      if (y1 > this.yPositionOnScreen + height - 128 /*0x80*/)
      {
        this.collections[key3].Add(new List<ClickableTextureComponent>());
        numArray[key3] = 0;
        x1 = num3;
        y1 = num4;
      }
      if (this.collections[key3].Count == 0)
        this.collections[key3].Add(new List<ClickableTextureComponent>());
      List<ClickableTextureComponent> textureComponentList1 = this.collections[key3].Last<List<ClickableTextureComponent>>();
      ParsedItemData dataOrErrorItem = ItemRegistry.GetDataOrErrorItem(itemId);
      List<ClickableTextureComponent> textureComponentList2 = textureComponentList1;
      ClickableTextureComponent textureComponent11 = new ClickableTextureComponent($"{itemId} {drawShadow.ToString()} {flag.ToString()}", new Rectangle(x1, y1, 64 /*0x40*/, 64 /*0x40*/), (string) null, "", dataOrErrorItem.GetTexture(), dataOrErrorItem.GetSourceRect(), 4f, drawShadow);
      textureComponent11.myID = textureComponentList1.Count;
      textureComponent11.rightNeighborID = (textureComponentList1.Count + 1) % num5 == 0 ? -1 : textureComponentList1.Count + 1;
      textureComponent11.leftNeighborID = textureComponentList1.Count % num5 == 0 ? 7001 : textureComponentList1.Count - 1;
      textureComponent11.downNeighborID = y1 + 68 > this.yPositionOnScreen + height - 128 /*0x80*/ ? -7777 : textureComponentList1.Count + num5;
      textureComponent11.upNeighborID = textureComponentList1.Count < num5 ? 12347 : textureComponentList1.Count - num5;
      textureComponent11.fullyImmutable = true;
      textureComponentList2.Add(textureComponent11);
      ++numArray[key3];
    }
    if (this.collections[5].Count == 0)
      this.collections[5].Add(new List<ClickableTextureComponent>());
    foreach (KeyValuePair<int, string> achievement in Game1.achievements)
    {
      bool flag = Game1.player.achievements.Contains(achievement.Key);
      string[] strArray = achievement.Value.Split('^');
      if (flag || strArray[2].Equals("true") && (strArray[3].Equals("-1") || this.farmerHasAchievements(strArray[3])))
      {
        int x2 = num3 + numArray[5] % num5 * 68;
        int y2 = num4 + numArray[5] / num5 * 68;
        this.collections[5][0].Add(new ClickableTextureComponent($"{achievement.Key.ToString()} {flag.ToString()}", new Rectangle(x2, y2, 64 /*0x40*/, 64 /*0x40*/), (string) null, "", Game1.mouseCursors, Game1.getSourceRectForStandardTileSheet(Game1.mouseCursors, 25), 1f));
        ++numArray[5];
      }
      else
      {
        this.collections[5][0].Add(new ClickableTextureComponent("??? false", new Rectangle(num3 + numArray[5] % num5 * 68, num4 + numArray[5] / num5 * 68, 64 /*0x40*/, 64 /*0x40*/), (string) null, "???", Game1.mouseCursors, Game1.getSourceRectForStandardTileSheet(Game1.mouseCursors, 25), 1f));
        ++numArray[5];
      }
    }
    if (Game1.player.secretNotesSeen.Count > 0)
    {
      if (this.collections[6].Count == 0)
        this.collections[6].Add(new List<ClickableTextureComponent>());
      this.secretNotesData = DataLoader.SecretNotes(Game1.content);
      this.secretNoteImageTexture = Game1.temporaryContent.Load<Texture2D>("TileSheets\\SecretNotesImages");
      bool flag = Game1.player.secretNotesSeen.Contains(GameLocation.JOURNAL_INDEX + 1);
      foreach (int key5 in this.secretNotesData.Keys)
      {
        if (key5 >= GameLocation.JOURNAL_INDEX)
        {
          if (!flag)
            continue;
        }
        else if (!Game1.player.hasMagnifyingGlass)
          continue;
        int x3 = num3 + numArray[6] % num5 * 68;
        int y3 = num4 + numArray[6] / num5 * 68;
        if (key5 >= GameLocation.JOURNAL_INDEX)
          this.collections[6][0].Add(new ClickableTextureComponent($"{key5.ToString()} {Game1.player.secretNotesSeen.Contains(key5).ToString()}", new Rectangle(x3, y3, 64 /*0x40*/, 64 /*0x40*/), (string) null, "", Game1.objectSpriteSheet, Game1.getSourceRectForStandardTileSheet(Game1.objectSpriteSheet, 842, 16 /*0x10*/, 16 /*0x10*/), 4f, Game1.player.secretNotesSeen.Contains(key5)));
        else
          this.collections[6][0].Add(new ClickableTextureComponent($"{key5.ToString()} {Game1.player.secretNotesSeen.Contains(key5).ToString()}", new Rectangle(x3, y3, 64 /*0x40*/, 64 /*0x40*/), (string) null, "", Game1.objectSpriteSheet, Game1.getSourceRectForStandardTileSheet(Game1.objectSpriteSheet, 79, 16 /*0x10*/, 16 /*0x10*/), 4f, Game1.player.secretNotesSeen.Contains(key5)));
        ++numArray[6];
      }
    }
    if (this.collections[7].Count == 0)
      this.collections[7].Add(new List<ClickableTextureComponent>());
    List<ClickableTextureComponent> textureComponentList3 = this.collections[7].Last<List<ClickableTextureComponent>>();
    Dictionary<string, string> dictionary = DataLoader.Mail(Game1.content);
    foreach (string key6 in (NetHashSet<string>) Game1.player.mailReceived)
    {
      string str;
      if (dictionary.TryGetValue(key6, out str))
      {
        int x4 = num3 + numArray[7] % num5 * 68;
        int y4 = num4 + numArray[7] / num5 * 68;
        string[] strArray = str.Split("[#]");
        if (y4 > this.yPositionOnScreen + height - 128 /*0x80*/)
        {
          this.collections[7].Add(new List<ClickableTextureComponent>());
          numArray[7] = 0;
          x4 = num3;
          y4 = num4;
          textureComponentList3 = this.collections[7].Last<List<ClickableTextureComponent>>();
        }
        List<ClickableTextureComponent> textureComponentList4 = textureComponentList3;
        ClickableTextureComponent textureComponent12 = new ClickableTextureComponent($"{key6} true {(strArray.Length > 1 ? strArray[1] : "???")}", new Rectangle(x4, y4, 64 /*0x40*/, 64 /*0x40*/), (string) null, "", Game1.mouseCursors, new Rectangle(190, 423, 14, 11), 4f, true);
        textureComponent12.myID = textureComponentList3.Count;
        textureComponent12.rightNeighborID = (textureComponentList3.Count + 1) % num5 == 0 ? -1 : textureComponentList3.Count + 1;
        textureComponent12.leftNeighborID = textureComponentList3.Count % num5 == 0 ? 7008 : textureComponentList3.Count - 1;
        textureComponent12.downNeighborID = y4 + 68 > this.yPositionOnScreen + height - 128 /*0x80*/ ? -7777 : textureComponentList3.Count + num5;
        textureComponent12.upNeighborID = textureComponentList3.Count < num5 ? 12347 : textureComponentList3.Count - num5;
        textureComponent12.fullyImmutable = true;
        textureComponentList4.Add(textureComponent12);
        ++numArray[7];
      }
    }
  }

  protected override void customSnapBehavior(int direction, int oldRegion, int oldID)
  {
    base.customSnapBehavior(direction, oldRegion, oldID);
    switch (direction)
    {
      case 1:
        if (oldID != 706 || this.collections[this.currentTab].Count <= this.currentPage + 1)
          break;
        this.currentlySnappedComponent = this.getComponentWithID(707);
        break;
      case 2:
        if (this.currentPage > 0)
          this.currentlySnappedComponent = this.getComponentWithID(706);
        else if (this.currentPage == 0 && this.collections[this.currentTab].Count > 1)
          this.currentlySnappedComponent = this.getComponentWithID(707);
        this.backButton.upNeighborID = oldID;
        this.forwardButton.upNeighborID = oldID;
        break;
      case 3:
        if (oldID != 707 || this.currentPage <= 0)
          break;
        this.currentlySnappedComponent = this.getComponentWithID(706);
        break;
    }
  }

  public override void snapToDefaultClickableComponent()
  {
    base.snapToDefaultClickableComponent();
    this.currentlySnappedComponent = this.getComponentWithID(0);
    this.snapCursorToCurrentSnappedComponent();
  }

  /// <summary>Restore the page state when it's recreated for a window resize.</summary>
  /// <param name="oldPage">The previous page instance before it was recreated.</param>
  public void postWindowSizeChange(IClickableMenu oldPage)
  {
    if (!(oldPage is CollectionsPage collectionsPage))
      return;
    this.sideTabs[this.currentTab].bounds.X -= CollectionsPage.widthToMoveActiveTab;
    this.currentTab = collectionsPage.currentTab;
    this.currentPage = collectionsPage.currentPage;
    this.sideTabs[this.currentTab].bounds.X += CollectionsPage.widthToMoveActiveTab;
  }

  private bool farmerHasAchievements(string listOfAchievementNumbers)
  {
    foreach (string str in ArgUtility.SplitBySpace(listOfAchievementNumbers))
    {
      if (!Game1.player.achievements.Contains(Convert.ToInt32(str)))
        return false;
    }
    return true;
  }

  public override bool readyToClose() => this.letterviewerSubMenu == null && base.readyToClose();

  /// <inheritdoc />
  public override void update(GameTime time)
  {
    base.update(time);
    if (this.letterviewerSubMenu == null)
      return;
    this.letterviewerSubMenu.update(time);
    if (!this.letterviewerSubMenu.destroy)
      return;
    this.letterviewerSubMenu = (LetterViewerMenu) null;
    if (!Game1.options.SnappyMenus)
      return;
    this.snapCursorToCurrentSnappedComponent();
  }

  /// <inheritdoc />
  public override void receiveKeyPress(Keys key)
  {
    base.receiveKeyPress(key);
    this.letterviewerSubMenu?.receiveKeyPress(key);
  }

  /// <inheritdoc />
  public override void receiveLeftClick(int x, int y, bool playSound = true)
  {
    if (this.letterviewerSubMenu != null)
    {
      this.letterviewerSubMenu.receiveLeftClick(x, y, true);
    }
    else
    {
      foreach (KeyValuePair<int, ClickableTextureComponent> sideTab in this.sideTabs)
      {
        if (sideTab.Value.containsPoint(x, y) && this.currentTab != sideTab.Key)
        {
          Game1.playSound("smallSelect");
          this.sideTabs[this.currentTab].bounds.X -= CollectionsPage.widthToMoveActiveTab;
          this.currentTab = Convert.ToInt32(sideTab.Value.name);
          this.currentPage = 0;
          sideTab.Value.bounds.X += CollectionsPage.widthToMoveActiveTab;
        }
      }
      if (this.currentPage > 0 && this.backButton.containsPoint(x, y))
      {
        --this.currentPage;
        Game1.playSound("shwip");
        this.backButton.scale = this.backButton.baseScale;
        if (Game1.options.snappyMenus && Game1.options.gamepadControls && this.currentPage == 0)
        {
          this.currentlySnappedComponent = (ClickableComponent) this.forwardButton;
          Game1.setMousePosition(this.currentlySnappedComponent.bounds.Center);
        }
      }
      if (this.currentPage < this.collections[this.currentTab].Count - 1 && this.forwardButton.containsPoint(x, y))
      {
        ++this.currentPage;
        Game1.playSound("shwip");
        this.forwardButton.scale = this.forwardButton.baseScale;
        if (Game1.options.snappyMenus && Game1.options.gamepadControls && this.currentPage == this.collections[this.currentTab].Count - 1)
        {
          this.currentlySnappedComponent = (ClickableComponent) this.backButton;
          Game1.setMousePosition(this.currentlySnappedComponent.bounds.Center);
        }
      }
      switch (this.currentTab)
      {
        case 6:
          using (List<ClickableTextureComponent>.Enumerator enumerator = this.collections[this.currentTab][this.currentPage].GetEnumerator())
          {
            while (enumerator.MoveNext())
            {
              ClickableComponent current = (ClickableComponent) enumerator.Current;
              if (current.containsPoint(x, y))
              {
                string[] strArray = ArgUtility.SplitBySpace(current.name);
                int result;
                if (strArray[1] == "True" && int.TryParse(strArray[0], out result))
                {
                  this.letterviewerSubMenu = new LetterViewerMenu(result);
                  this.letterviewerSubMenu.isFromCollection = true;
                  break;
                }
              }
            }
            break;
          }
        case 7:
          Dictionary<string, string> dictionary = DataLoader.Mail(Game1.content);
          using (List<ClickableTextureComponent>.Enumerator enumerator = this.collections[this.currentTab][this.currentPage].GetEnumerator())
          {
            while (enumerator.MoveNext())
            {
              ClickableComponent current = (ClickableComponent) enumerator.Current;
              if (current.containsPoint(x, y))
              {
                string str = ArgUtility.SplitBySpaceAndGet(current.name, 0);
                this.letterviewerSubMenu = new LetterViewerMenu(dictionary[str], str, true);
              }
            }
            break;
          }
      }
    }
  }

  public override bool shouldDrawCloseButton() => this.letterviewerSubMenu == null;

  /// <inheritdoc />
  public override void receiveRightClick(int x, int y, bool playSound = true)
  {
    this.letterviewerSubMenu?.receiveRightClick(x, y, true);
  }

  public override void applyMovementKey(int direction)
  {
    if (this.letterviewerSubMenu != null)
      this.letterviewerSubMenu.applyMovementKey(direction);
    else
      base.applyMovementKey(direction);
  }

  /// <inheritdoc />
  public override void gamePadButtonHeld(Buttons b)
  {
    if (this.letterviewerSubMenu != null)
      this.letterviewerSubMenu.gamePadButtonHeld(b);
    else
      base.gamePadButtonHeld(b);
  }

  /// <inheritdoc />
  public override void receiveGamePadButton(Buttons button)
  {
    if (this.letterviewerSubMenu != null)
      this.letterviewerSubMenu.receiveGamePadButton(button);
    else
      base.receiveGamePadButton(button);
  }

  /// <inheritdoc />
  public override void performHoverAction(int x, int y)
  {
    this.hoverText = "";
    this.value = -1;
    this.secretNoteImage = -1;
    if (this.letterviewerSubMenu != null)
    {
      this.letterviewerSubMenu.performHoverAction(x, y);
    }
    else
    {
      foreach (ClickableTextureComponent textureComponent in this.sideTabs.Values)
      {
        if (textureComponent.containsPoint(x, y))
        {
          this.hoverText = textureComponent.hoverText;
          return;
        }
      }
      bool flag = false;
      foreach (ClickableTextureComponent textureComponent in this.collections[this.currentTab][this.currentPage])
      {
        if (textureComponent.containsPoint(x, y, 2))
        {
          textureComponent.scale = Math.Min(textureComponent.scale + 0.02f, textureComponent.baseScale + 0.1f);
          string[] strArray = ArgUtility.SplitBySpace(textureComponent.name);
          if (this.currentTab == 5 || strArray.Length > 1 && Convert.ToBoolean(strArray[1]) || strArray.Length > 2 && Convert.ToBoolean(strArray[2]))
          {
            this.hoverText = this.currentTab != 7 ? this.createDescription(strArray[0]) : Game1.parseText(textureComponent.name.Substring(textureComponent.name.IndexOf(' ', textureComponent.name.IndexOf(' ') + 1) + 1), Game1.smallFont, 256 /*0x0100*/);
          }
          else
          {
            if (this.hoverText != "???")
              this.hoverItem = (Item) null;
            this.hoverText = "???";
          }
          flag = true;
        }
        else
          textureComponent.scale = Math.Max(textureComponent.scale - 0.02f, textureComponent.baseScale);
      }
      if (!flag)
        this.hoverItem = (Item) null;
      this.forwardButton.tryHover(x, y, 0.5f);
      this.backButton.tryHover(x, y, 0.5f);
    }
  }

  public string createDescription(string id)
  {
    string description = "";
    switch (this.currentTab)
    {
      case 5:
        if (id == "???")
          return "???";
        int key1 = int.Parse(id);
        string[] strArray1 = Game1.achievements[key1].Split('^');
        description = description + strArray1[0] + Environment.NewLine + Environment.NewLine + strArray1[1];
        break;
      case 6:
        if (this.secretNotesData != null)
        {
          int key2 = int.Parse(id);
          description = key2 >= GameLocation.JOURNAL_INDEX ? $"{description}{Game1.content.LoadString("Strings\\Locations:Journal_Name")} #{(key2 - GameLocation.JOURNAL_INDEX).ToString()}" : $"{description}{Game1.content.LoadString("Strings\\Locations:Secret_Note_Name")} #{key2.ToString()}";
          if (this.secretNotesData[key2][0] == '!')
          {
            this.secretNoteImage = Convert.ToInt32(ArgUtility.SplitBySpaceAndGet(this.secretNotesData[key2], 1));
            break;
          }
          string str = Game1.parseText(Utility.ParseGiftReveals(this.secretNotesData[key2]).TrimStart(' ', '^').Replace("^", Environment.NewLine).Replace("@", Game1.player.name.Value), Game1.smallFont, 512 /*0x0200*/);
          string[] strArray2 = str.Split(Environment.NewLine);
          int length = 15;
          if (strArray2.Length > length)
          {
            string[] strArray3 = new string[length];
            for (int index = 0; index < length; ++index)
              strArray3[index] = strArray2[index];
            str = $"{string.Join(Environment.NewLine, strArray3).Trim()}{Environment.NewLine}(...)";
          }
          description = description + Environment.NewLine + Environment.NewLine + str;
          break;
        }
        break;
      default:
        ParsedItemData dataOrErrorItem = ItemRegistry.GetDataOrErrorItem(id);
        string str1 = Game1.content.LoadStringReturnNullIfNotFound($"Strings\\Objects:{dataOrErrorItem.ItemId}_CollectionsTabName") ?? dataOrErrorItem.DisplayName;
        string text = Game1.content.LoadStringReturnNullIfNotFound($"Strings\\Objects:{dataOrErrorItem.ItemId}_CollectionsTabDescription") ?? dataOrErrorItem.Description;
        string str2 = description + str1 + Environment.NewLine + Environment.NewLine + Game1.parseText(text, Game1.smallFont, 256 /*0x0100*/) + Environment.NewLine + Environment.NewLine;
        switch (dataOrErrorItem.ObjectType)
        {
          case "Arch":
            int[] numArray1;
            description = str2 + (Game1.player.archaeologyFound.TryGetValue(id, out numArray1) ? Game1.content.LoadString("Strings\\UI:Collections_Description_ArtifactsFound", (object) numArray1[0]) : "");
            break;
          case "Cooking":
            int sub1;
            description = str2 + (Game1.player.recipesCooked.TryGetValue(id, out sub1) ? Game1.content.LoadString("Strings\\UI:Collections_Description_RecipesCooked", (object) sub1) : "");
            if (this.hoverItem == null || this.hoverItem.ItemId != id)
            {
              this.hoverItem = (Item) new StardewValley.Object(id, 1);
              string name = this.hoverItem.Name;
              if (name != null)
              {
                switch (name.Length)
                {
                  case 6:
                    if (name == "Cookie")
                    {
                      name = "Cookies";
                      break;
                    }
                    break;
                  case 13:
                    if (name == "Cheese Cauli.")
                    {
                      name = "Cheese Cauliflower";
                      break;
                    }
                    break;
                  case 15:
                    switch (name[0])
                    {
                      case 'C':
                        if (name == "Cranberry Sauce")
                        {
                          name = "Cran. Sauce";
                          break;
                        }
                        break;
                      case 'D':
                        if (name == "Dish O' The Sea")
                        {
                          name = "Dish o' The Sea";
                          break;
                        }
                        break;
                    }
                    break;
                  case 16 /*0x10*/:
                    if (name == "Vegetable Medley")
                    {
                      name = "Vegetable Stew";
                      break;
                    }
                    break;
                  case 17:
                    if (name == "Eggplant Parmesan")
                    {
                      name = "Eggplant Parm.";
                      break;
                    }
                    break;
                  case 18:
                    if (name == "Cheese Cauliflower")
                    {
                      name = "Cheese Cauli.";
                      break;
                    }
                    break;
                }
              }
              this.hoverCraftingRecipe = new CraftingRecipe(name, true);
              break;
            }
            break;
          case "Fish":
            int[] numArray2;
            if (Game1.player.fishCaught.TryGetValue("(O)" + id, out numArray2))
            {
              description = str2 + Game1.content.LoadString("Strings\\UI:Collections_Description_FishCaught", (object) numArray2[0]);
              if (numArray2[1] > 0)
              {
                description = description + Environment.NewLine + Game1.content.LoadString("Strings\\UI:Collections_Description_BiggestCatch", (object) Game1.content.LoadString("Strings\\StringsFromCSFiles:FishingRod.cs.14083", (object) (LocalizedContentManager.CurrentLanguageCode != LocalizedContentManager.LanguageCode.en ? Math.Round((double) numArray2[1] * 2.54) : (double) numArray2[1])));
                break;
              }
              break;
            }
            description = str2 + Game1.content.LoadString("Strings\\UI:Collections_Description_FishCaught", (object) 0);
            break;
          default:
            int num1;
            int num2;
            description = dataOrErrorItem.ObjectType == "Minerals" || dataOrErrorItem.Category == -2 ? str2 + Game1.content.LoadString("Strings\\UI:Collections_Description_MineralsFound", (object) (Game1.player.mineralsFound.TryGetValue(id, out num1) ? num1 : 0)) : str2 + Game1.content.LoadString("Strings\\UI:Collections_Description_NumberShipped", (object) (Game1.player.basicShipped.TryGetValue(id, out num2) ? num2 : 0));
            break;
        }
        this.value = ObjectDataDefinition.GetRawPrice(dataOrErrorItem);
        break;
    }
    return description;
  }

  /// <inheritdoc />
  public override void draw(SpriteBatch b)
  {
    foreach (ClickableTextureComponent textureComponent in this.sideTabs.Values)
      textureComponent.draw(b);
    if (this.currentPage > 0)
      this.backButton.draw(b);
    if (this.currentPage < this.collections[this.currentTab].Count - 1)
      this.forwardButton.draw(b);
    b.End();
    b.Begin(SpriteSortMode.FrontToBack, BlendState.AlphaBlend, SamplerState.PointClamp);
    foreach (ClickableTextureComponent textureComponent in this.collections[this.currentTab][this.currentPage])
    {
      string[] strArray = ArgUtility.SplitBySpace(textureComponent.name);
      bool boolean = Convert.ToBoolean(strArray[1]);
      bool flag = this.currentTab == 4 && Convert.ToBoolean(strArray[2]) || this.currentTab == 5 && !boolean && textureComponent.hoverText != "???";
      textureComponent.draw(b, flag ? Color.DimGray * 0.4f : (boolean ? Color.White : Color.Black * 0.2f), 0.86f);
      if (this.currentTab == 5 & boolean)
      {
        int num = Utility.CreateRandom((double) Convert.ToInt32(strArray[0])).Next(12);
        b.Draw(Game1.mouseCursors, new Vector2((float) (textureComponent.bounds.X + 16 /*0x10*/ + 16 /*0x10*/), (float) (textureComponent.bounds.Y + 20 + 16 /*0x10*/)), new Rectangle?(new Rectangle(256 /*0x0100*/ + num % 6 * 64 /*0x40*/ / 2, 128 /*0x80*/ + num / 6 * 64 /*0x40*/ / 2, 32 /*0x20*/, 32 /*0x20*/)), Color.White, 0.0f, new Vector2(16f, 16f), textureComponent.scale, SpriteEffects.None, 0.88f);
      }
    }
    b.End();
    b.Begin(blendState: BlendState.AlphaBlend, samplerState: SamplerState.PointClamp);
    if (this.hoverItem != null)
    {
      string hoverText = this.hoverItem.getDescription();
      string hoverTitle = this.hoverItem.DisplayName;
      if (hoverText.Contains("{0}"))
      {
        string str1 = Game1.content.LoadStringReturnNullIfNotFound($"Strings\\Objects:{this.hoverItem.Name}_CollectionsTabDescription");
        if (str1 != null)
          hoverText = str1;
        string str2 = Game1.content.LoadStringReturnNullIfNotFound($"Strings\\Objects:{this.hoverItem.Name}_CollectionsTabName");
        if (str2 != null)
          hoverTitle = str2;
      }
      IClickableMenu.drawToolTip(b, hoverText, hoverTitle, this.hoverItem, craftingIngredients: this.hoverCraftingRecipe);
    }
    else if (!this.hoverText.Equals(""))
    {
      IClickableMenu.drawHoverText(b, this.hoverText, Game1.smallFont, moneyAmountToDisplayAtBottom: this.value);
      if (this.secretNoteImage != -1)
      {
        IClickableMenu.drawTextureBox(b, Game1.getOldMouseX(), Game1.getOldMouseY() + 64 /*0x40*/ + 32 /*0x20*/, 288, 288, Color.White);
        b.Draw(this.secretNoteImageTexture, new Vector2((float) (Game1.getOldMouseX() + 16 /*0x10*/), (float) (Game1.getOldMouseY() + 64 /*0x40*/ + 32 /*0x20*/ + 16 /*0x10*/)), new Rectangle?(new Rectangle(this.secretNoteImage * 64 /*0x40*/ % this.secretNoteImageTexture.Width, this.secretNoteImage * 64 /*0x40*/ / this.secretNoteImageTexture.Width * 64 /*0x40*/, 64 /*0x40*/, 64 /*0x40*/)), Color.White, 0.0f, Vector2.Zero, 4f, SpriteEffects.None, 0.865f);
      }
    }
    this.letterviewerSubMenu?.draw(b);
  }
}
