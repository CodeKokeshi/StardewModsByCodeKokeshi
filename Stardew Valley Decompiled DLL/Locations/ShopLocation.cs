// Decompiled with JetBrains decompiler
// Type: StardewValley.Locations.ShopLocation
// Assembly: Stardew Valley, Version=1.6.15.24356, Culture=neutral, PublicKeyToken=null
// MVID: AA1F513A-94F0-4EF6-A35F-2D5B4A3721BF
// Assembly location: D:\SteamLibrary\steamapps\common\Stardew Valley\Stardew Valley.dll
// XML documentation location: D:\SteamLibrary\steamapps\common\Stardew Valley\Stardew Valley.xml

using Netcode;
using StardewValley.BellsAndWhistles;
using StardewValley.Extensions;
using System;

#nullable disable
namespace StardewValley.Locations;

public class ShopLocation : GameLocation
{
  public const int maxItemsToSellFromPlayer = 11;
  public readonly NetObjectList<Item> itemsFromPlayerToSell = new NetObjectList<Item>();
  public readonly NetObjectList<Item> itemsToStartSellingTomorrow = new NetObjectList<Item>();

  public ShopLocation()
  {
  }

  public ShopLocation(string map, string name)
    : base(map, name)
  {
  }

  protected override void initNetFields()
  {
    base.initNetFields();
    this.NetFields.AddField((INetSerializable) this.itemsFromPlayerToSell, "itemsFromPlayerToSell").AddField((INetSerializable) this.itemsToStartSellingTomorrow, "itemsToStartSellingTomorrow");
  }

  /// <summary>Get a dialogue for an NPC when the player purchases an item from the shop, if they have any.</summary>
  /// <param name="i">The item that was purchased.</param>
  /// <param name="n">The NPC for which to get a dialogue.</param>
  /// <returns>Returns a dialogue to use, or <c>null</c> to skip this NPC.</returns>
  public virtual Dialogue getPurchasedItemDialogueForNPC(StardewValley.Object i, NPC n)
  {
    Dialogue itemDialogueForNpc = (Dialogue) null;
    string[] strArray = Game1.content.LoadString("Strings\\Lexicon:GenericPlayerTerm").Split('^');
    string str = strArray[0];
    if (strArray.Length > 1 && !Game1.player.IsMale)
      str = strArray[1];
    string sub1 = Game1.random.NextDouble() < (double) (Game1.player.getFriendshipLevelForNPC(n.Name) / 1250) ? Game1.player.Name : str;
    if (n.Age != 0)
      sub1 = Game1.player.Name;
    string sub2 = LocalizedContentManager.CurrentLanguageCode == LocalizedContentManager.LanguageCode.en ? Lexicon.getProperArticleForWord(i.name) : "";
    if ((i.Category == -4 || i.Category == -75 || i.Category == -79) && Game1.random.NextBool())
      sub2 = Game1.content.LoadString("Strings\\StringsFromCSFiles:SeedShop.cs.9701");
    int num = Game1.random.Next(5);
    if (n.Manners == 2)
      num = 2;
    switch (num)
    {
      case 0:
        if (Game1.random.NextDouble() < (double) i.quality.Value * 0.5 + 0.2)
        {
          itemDialogueForNpc = Dialogue.FromTranslation(n, "Data\\ExtraDialogue:PurchasedItem_1_QualityHigh", (object) sub1, (object) sub2, (object) i.DisplayName, (object) Lexicon.getRandomDeliciousAdjective(n));
          break;
        }
        itemDialogueForNpc = Dialogue.FromTranslation(n, "Data\\ExtraDialogue:PurchasedItem_1_QualityLow", (object) sub1, (object) sub2, (object) i.DisplayName, (object) Lexicon.getRandomNegativeFoodAdjective(n));
        break;
      case 1:
        itemDialogueForNpc = i.quality.Value != 0 ? (!n.Name.Equals("Jodi") ? Dialogue.FromTranslation(n, "Data\\ExtraDialogue:PurchasedItem_2_QualityHigh", (object) sub1, (object) sub2, (object) i.DisplayName) : Dialogue.FromTranslation(n, "Data\\ExtraDialogue:PurchasedItem_2_QualityHigh_Jodi", (object) sub1, (object) sub2, (object) i.DisplayName)) : Dialogue.FromTranslation(n, "Data\\ExtraDialogue:PurchasedItem_2_QualityLow", (object) sub1, (object) sub2, (object) i.DisplayName);
        break;
      case 2:
        if (n.Manners == 2)
        {
          if (i.quality.Value != 2)
          {
            itemDialogueForNpc = Dialogue.FromTranslation(n, "Data\\ExtraDialogue:PurchasedItem_3_QualityLow_Rude", (object) sub1, (object) sub2, (object) i.DisplayName, (object) (i.salePrice(false) / 2), (object) Lexicon.getRandomNegativeFoodAdjective(n), (object) Lexicon.getRandomNegativeItemSlanderNoun());
            break;
          }
          itemDialogueForNpc = Dialogue.FromTranslation(n, "Data\\ExtraDialogue:PurchasedItem_3_QualityHigh_Rude", (object) sub1, (object) sub2, (object) i.DisplayName, (object) (i.salePrice(false) / 2), (object) Lexicon.getRandomSlightlyPositiveAdjectiveForEdibleNoun(n));
          break;
        }
        Dialogue.FromTranslation(n, "Data\\ExtraDialogue:PurchasedItem_3_NonRude", (object) sub1, (object) sub2, (object) i.DisplayName, (object) (i.salePrice(false) / 2));
        break;
      case 3:
        itemDialogueForNpc = Dialogue.FromTranslation(n, "Data\\ExtraDialogue:PurchasedItem_4", (object) sub1, (object) sub2, (object) i.DisplayName);
        break;
      case 4:
        switch (i.Category)
        {
          case -79:
          case -75:
            itemDialogueForNpc = Dialogue.FromTranslation(n, "Data\\ExtraDialogue:PurchasedItem_5_VegetableOrFruit", (object) sub1, (object) sub2, (object) i.DisplayName);
            break;
          case -7:
            string forEventOrPerson = Lexicon.getRandomPositiveAdjectiveForEventOrPerson(n);
            itemDialogueForNpc = Dialogue.FromTranslation(n, "Data\\ExtraDialogue:PurchasedItem_5_Cooking", (object) sub1, (object) sub2, (object) i.DisplayName, (object) Lexicon.getProperArticleForWord(forEventOrPerson), (object) forEventOrPerson);
            break;
          default:
            itemDialogueForNpc = Dialogue.FromTranslation(n, "Data\\ExtraDialogue:PurchasedItem_5_Foraged", (object) sub1, (object) sub2, (object) i.DisplayName);
            break;
        }
        break;
    }
    if (n.Age == 1 && Game1.random.NextDouble() < 0.6)
      itemDialogueForNpc = Dialogue.FromTranslation(n, "Data\\ExtraDialogue:PurchasedItem_Teen", (object) sub1, (object) sub2, (object) i.DisplayName);
    string name = n.Name;
    if (name != null)
    {
      switch (name.Length)
      {
        case 4:
          switch (name[0])
          {
            case 'A':
              if (name == "Alex")
              {
                itemDialogueForNpc = Dialogue.FromTranslation(n, "Data\\ExtraDialogue:PurchasedItem_Alex", (object) sub1, (object) sub2, (object) i.DisplayName);
                break;
              }
              break;
            case 'L':
              if (name == "Leah")
              {
                itemDialogueForNpc = Dialogue.FromTranslation(n, "Data\\ExtraDialogue:PurchasedItem_Leah", (object) sub1, (object) sub2, (object) i.DisplayName);
                break;
              }
              break;
          }
          break;
        case 5:
          if (name == "Haley")
          {
            itemDialogueForNpc = Dialogue.FromTranslation(n, "Data\\ExtraDialogue:PurchasedItem_Haley", (object) sub1, (object) sub2, (object) i.DisplayName);
            break;
          }
          break;
        case 6:
          if (name == "Pierre")
          {
            string translationKey = i.quality.Value == 0 ? "Data\\ExtraDialogue:PurchasedItem_Pierre_QualityLow" : "Data\\ExtraDialogue:PurchasedItem_Pierre_QualityHigh";
            itemDialogueForNpc = Dialogue.FromTranslation(n, translationKey, (object) sub1, (object) sub2, (object) i.DisplayName);
            break;
          }
          break;
        case 7:
          switch (name[0])
          {
            case 'A':
              if (name == "Abigail")
              {
                if (i.quality.Value == 0)
                {
                  itemDialogueForNpc = Dialogue.FromTranslation(n, "Data\\ExtraDialogue:PurchasedItem_Abigail_QualityLow", (object) sub1, (object) sub2, (object) i.DisplayName, (object) Lexicon.getRandomNegativeItemSlanderNoun());
                  break;
                }
                itemDialogueForNpc = Dialogue.FromTranslation(n, "Data\\ExtraDialogue:PurchasedItem_Abigail_QualityHigh", (object) sub1, (object) sub2, (object) i.DisplayName);
                break;
              }
              break;
            case 'E':
              if (name == "Elliott")
              {
                itemDialogueForNpc = Dialogue.FromTranslation(n, "Data\\ExtraDialogue:PurchasedItem_Elliott", (object) sub1, (object) sub2, (object) i.DisplayName);
                break;
              }
              break;
          }
          break;
        case 8:
          if (name == "Caroline")
          {
            string translationKey = i.quality.Value == 0 ? "Data\\ExtraDialogue:PurchasedItem_Caroline_QualityLow" : "Data\\ExtraDialogue:PurchasedItem_Caroline_QualityHigh";
            itemDialogueForNpc = Dialogue.FromTranslation(n, translationKey, (object) sub1, (object) sub2, (object) i.DisplayName);
            break;
          }
          break;
      }
    }
    return itemDialogueForNpc;
  }

  public override void DayUpdate(int dayOfMonth)
  {
    this.itemsToStartSellingTomorrow.RemoveWhere((Func<Item, bool>) (p => p == null));
    this.itemsFromPlayerToSell.RemoveWhere((Func<Item, bool>) (p => p == null));
    for (int index = this.itemsToStartSellingTomorrow.Count - 1; index >= 0; --index)
    {
      Item obj1 = this.itemsToStartSellingTomorrow[index];
      if (this.itemsFromPlayerToSell.Count < 11)
      {
        bool flag = false;
        foreach (Item obj2 in (NetList<Item, NetRef<Item>>) this.itemsFromPlayerToSell)
        {
          if (obj2.Name == obj1.Name && obj2.Quality == obj1.Quality)
          {
            obj2.Stack += obj1.Stack;
            flag = true;
            break;
          }
        }
        this.itemsToStartSellingTomorrow.RemoveAt(index);
        if (!flag)
          this.itemsFromPlayerToSell.Add(obj1);
      }
    }
    base.DayUpdate(dayOfMonth);
  }
}
