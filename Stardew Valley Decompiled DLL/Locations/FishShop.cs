// Decompiled with JetBrains decompiler
// Type: StardewValley.Locations.FishShop
// Assembly: Stardew Valley, Version=1.6.15.24356, Culture=neutral, PublicKeyToken=null
// MVID: AA1F513A-94F0-4EF6-A35F-2D5B4A3721BF
// Assembly location: D:\SteamLibrary\steamapps\common\Stardew Valley\Stardew Valley.dll
// XML documentation location: D:\SteamLibrary\steamapps\common\Stardew Valley\Stardew Valley.xml

using StardewValley.BellsAndWhistles;
using StardewValley.Extensions;
using xTile.Dimensions;

#nullable disable
namespace StardewValley.Locations;

public class FishShop : ShopLocation
{
  public FishShop()
  {
  }

  public FishShop(string map, string name)
    : base(map, name)
  {
  }

  /// <inheritdoc />
  public override Dialogue getPurchasedItemDialogueForNPC(Object i, NPC n)
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
      case 4:
        if (Game1.random.NextDouble() < (double) i.quality.Value * 0.5 + 0.2)
        {
          itemDialogueForNpc = Dialogue.FromTranslation(n, "Data\\ExtraDialogue:PurchasedItem_1_QualityHigh_Willy", (object) sub1, (object) sub2, (object) i.DisplayName, (object) Lexicon.getRandomDeliciousAdjective(n));
          break;
        }
        itemDialogueForNpc = Dialogue.FromTranslation(n, "Data\\ExtraDialogue:PurchasedItem_1_QualityLow_Willy", (object) sub1, (object) sub2, (object) i.DisplayName, (object) Lexicon.getRandomNegativeFoodAdjective(n));
        break;
      case 1:
        itemDialogueForNpc = i.quality.Value != 0 ? (!n.Name.Equals("Jodi") ? Dialogue.FromTranslation(n, "Data\\ExtraDialogue:PurchasedItem_2_QualityHigh_Willy", (object) sub1, (object) sub2, (object) i.DisplayName) : Dialogue.FromTranslation(n, "Data\\ExtraDialogue:PurchasedItem_2_QualityHigh_Jodi_Willy", (object) sub1, (object) sub2, (object) i.DisplayName)) : Dialogue.FromTranslation(n, "Data\\ExtraDialogue:PurchasedItem_2_QualityLow_Willy", (object) sub1, (object) sub2, (object) i.DisplayName);
        break;
      case 2:
        if (n.Manners == 2)
        {
          if (i.quality.Value < 2)
          {
            itemDialogueForNpc = Dialogue.FromTranslation(n, "Data\\ExtraDialogue:PurchasedItem_3_QualityLow_Rude_Willy", (object) sub1, (object) sub2, (object) i.DisplayName, (object) (i.salePrice(false) / 2), (object) Lexicon.getRandomNegativeFoodAdjective(n), (object) Lexicon.getRandomNegativeItemSlanderNoun());
            break;
          }
          itemDialogueForNpc = Dialogue.FromTranslation(n, "Data\\ExtraDialogue:PurchasedItem_3_QualityHigh_Rude_Willy", (object) sub1, (object) sub2, (object) i.DisplayName, (object) (i.salePrice(false) / 2), (object) Lexicon.getRandomSlightlyPositiveAdjectiveForEdibleNoun(n));
          break;
        }
        itemDialogueForNpc = Dialogue.FromTranslation(n, "Data\\ExtraDialogue:PurchasedItem_3_NonRude_Willy", (object) sub1, (object) sub2, (object) i.DisplayName, (object) (i.salePrice(false) / 2));
        break;
      case 3:
        itemDialogueForNpc = Dialogue.FromTranslation(n, "Data\\ExtraDialogue:PurchasedItem_4_Willy", (object) sub1, (object) sub2, (object) i.DisplayName);
        break;
    }
    if (n.Name == "Willy")
    {
      string translationKey = i.quality.Value == 0 ? "Data\\ExtraDialogue:PurchasedItem_Pierre_QualityLow_Willy" : "Data\\ExtraDialogue:PurchasedItem_Pierre_QualityHigh_Willy";
      itemDialogueForNpc = Dialogue.FromTranslation(n, translationKey, (object) sub1, (object) sub2, (object) i.DisplayName);
    }
    return itemDialogueForNpc;
  }

  /// <inheritdoc />
  public override bool performAction(string[] action, Farmer who, Location tileLocation)
  {
    if (ArgUtility.Get(action, 0) == "WarpBoatTunnel")
    {
      if (Game1.player.mailReceived.Contains("willyBackRoomInvitation"))
      {
        Game1.warpFarmer("BoatTunnel", 6, 12, false);
        this.playSound("doorClose");
      }
      else
        Game1.drawObjectDialogue(Game1.content.LoadString("Strings\\Locations:LockedDoor"));
    }
    return base.performAction(action, who, tileLocation);
  }
}
