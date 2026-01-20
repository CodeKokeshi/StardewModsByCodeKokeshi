// Decompiled with JetBrains decompiler
// Type: StardewValley.Locations.LibraryMuseum
// Assembly: Stardew Valley, Version=1.6.15.24356, Culture=neutral, PublicKeyToken=null
// MVID: AA1F513A-94F0-4EF6-A35F-2D5B4A3721BF
// Assembly location: D:\SteamLibrary\steamapps\common\Stardew Valley\Stardew Valley.dll
// XML documentation location: D:\SteamLibrary\steamapps\common\Stardew Valley\Stardew Valley.xml

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Netcode;
using StardewValley.Extensions;
using StardewValley.GameData.Museum;
using StardewValley.ItemTypeDefinitions;
using StardewValley.Menus;
using StardewValley.Network;
using StardewValley.Triggers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml.Serialization;
using xTile.Dimensions;

#nullable disable
namespace StardewValley.Locations;

public class LibraryMuseum : GameLocation
{
  public const int dwarvenGuide = 0;
  protected static int _totalArtifacts = -1;
  public const int totalNotes = 21;
  private readonly NetMutex mutex = new NetMutex();
  [XmlIgnore]
  protected Dictionary<Item, string> _itemToRewardsLookup = new Dictionary<Item, string>();

  public static int totalArtifacts
  {
    get
    {
      if (LibraryMuseum._totalArtifacts < 0)
      {
        LibraryMuseum._totalArtifacts = 0;
        foreach (string allId in ItemRegistry.RequireTypeDefinition("(O)").GetAllIds())
        {
          if (LibraryMuseum.IsItemSuitableForDonation("(O)" + allId, false))
            ++LibraryMuseum._totalArtifacts;
        }
      }
      return LibraryMuseum._totalArtifacts;
    }
  }

  [XmlElement("museumPieces")]
  public NetVector2Dictionary<string, NetString> museumPieces
  {
    get => Game1.netWorldState.Value.MuseumPieces;
  }

  public LibraryMuseum()
  {
  }

  public LibraryMuseum(string mapPath, string name)
    : base(mapPath, name)
  {
  }

  protected override void initNetFields()
  {
    base.initNetFields();
    this.NetFields.AddField((INetSerializable) this.mutex.NetFields, "mutex.NetFields");
  }

  public override void updateEvenIfFarmerIsntHere(GameTime time, bool skipWasUpdatedFlush = false)
  {
    this.mutex.Update((GameLocation) this);
    base.updateEvenIfFarmerIsntHere(time, skipWasUpdatedFlush);
  }

  /// <summary>Get whether any artifacts have been donated to the museum.</summary>
  public static bool HasDonatedArtifacts() => Game1.netWorldState.Value.MuseumPieces.Length > 0;

  /// <summary>Get whether an artifact has been placed on a given museum tile.</summary>
  /// <param name="tile">The tile position to check.</param>
  public static bool HasDonatedArtifactAt(Vector2 tile)
  {
    return Game1.netWorldState.Value.MuseumPieces.ContainsKey(tile);
  }

  /// <summary>Get whether an artifact has been donated to the museum.</summary>
  /// <param name="itemId">The qualified or unqualified item ID to check.</param>
  public static bool HasDonatedArtifact(string itemId)
  {
    if (itemId == null)
      return false;
    itemId = ItemRegistry.ManuallyQualifyItemId(itemId, "(O)");
    foreach (KeyValuePair<Vector2, string> pair in Game1.netWorldState.Value.MuseumPieces.Pairs)
    {
      if (itemId == "(O)" + pair.Value)
        return true;
    }
    return false;
  }

  public bool isItemSuitableForDonation(Item i)
  {
    return LibraryMuseum.IsItemSuitableForDonation(i?.QualifiedItemId);
  }

  /// <summary>Get whether an item can be donated to the museum.</summary>
  /// <param name="itemId">The qualified or unqualified item ID.</param>
  /// <param name="checkDonatedItems">Whether to return false if the item has already been donated to the museum.</param>
  public static bool IsItemSuitableForDonation(string itemId, bool checkDonatedItems = true)
  {
    if (itemId == null)
      return false;
    itemId = ItemRegistry.ManuallyQualifyItemId(itemId, "(O)");
    ParsedItemData dataOrErrorItem = ItemRegistry.GetDataOrErrorItem(itemId);
    HashSet<string> baseContextTags = ItemContextTagManager.GetBaseContextTags(itemId);
    if (!dataOrErrorItem.HasTypeObject() || baseContextTags.Contains("not_museum_donatable") || checkDonatedItems && LibraryMuseum.HasDonatedArtifact(dataOrErrorItem.QualifiedItemId))
      return false;
    return baseContextTags.Contains("museum_donatable") || baseContextTags.Contains("item_type_arch") || baseContextTags.Contains("item_type_minerals");
  }

  public bool doesFarmerHaveAnythingToDonate(Farmer who)
  {
    for (int index = 0; index < who.maxItems.Value; ++index)
    {
      if (index < who.Items.Count && who.Items[index] is StardewValley.Object i && this.isItemSuitableForDonation((Item) i))
        return true;
    }
    return false;
  }

  private Dictionary<int, Vector2> getLostBooksLocations()
  {
    Dictionary<int, Vector2> lostBooksLocations = new Dictionary<int, Vector2>();
    for (int index1 = 0; index1 < this.map.Layers[0].LayerWidth; ++index1)
    {
      for (int index2 = 0; index2 < this.map.Layers[0].LayerHeight; ++index2)
      {
        string[] propertySplitBySpaces = this.GetTilePropertySplitBySpaces("Action", "Buildings", index1, index2);
        if (ArgUtility.Get(propertySplitBySpaces, 0) == "Notes")
        {
          int key;
          string error;
          if (ArgUtility.TryGetInt(propertySplitBySpaces, 1, out key, out error, "int noteId"))
            lostBooksLocations.Add(key, new Vector2((float) index1, (float) index2));
          else
            this.LogTileActionError(propertySplitBySpaces, index1, index2, error);
        }
      }
    }
    return lostBooksLocations;
  }

  protected override void resetLocalState()
  {
    if (!Game1.player.eventsSeen.Contains("0") && this.doesFarmerHaveAnythingToDonate(Game1.player))
      Game1.player.mailReceived.Add("somethingToDonate");
    if (LibraryMuseum.HasDonatedArtifacts())
      Game1.player.mailReceived.Add("somethingWasDonated");
    base.resetLocalState();
    int lostBooksFound = Game1.netWorldState.Value.LostBooksFound;
    foreach (KeyValuePair<int, Vector2> lostBooksLocation in this.getLostBooksLocations())
    {
      int key = lostBooksLocation.Key;
      Vector2 vector2 = lostBooksLocation.Value;
      if (key <= lostBooksFound && !Game1.player.mailReceived.Contains("lb_" + key.ToString()))
        this.temporarySprites.Add(new TemporaryAnimatedSprite("LooseSprites\\Cursors", new Microsoft.Xna.Framework.Rectangle(144 /*0x90*/, 447, 15, 15), new Vector2(vector2.X * 64f, (float) ((double) vector2.Y * 64.0 - 96.0 - 16.0)), false, 0.0f, Color.White)
        {
          interval = 99999f,
          animationLength = 1,
          totalNumberOfLoops = 9999,
          yPeriodic = true,
          yPeriodicLoopTime = 4000f,
          yPeriodicRange = 16f,
          layerDepth = 1f,
          scale = 4f,
          id = key
        });
    }
  }

  public override void cleanupBeforePlayerExit()
  {
    this._itemToRewardsLookup?.Clear();
    base.cleanupBeforePlayerExit();
  }

  public override bool answerDialogueAction(string questionAndAnswer, string[] questionParams)
  {
    switch (questionAndAnswer)
    {
      case null:
        return false;
      case "Museum_Collect":
        this.OpenRewardMenu();
        break;
      case "Museum_Donate":
        this.OpenDonationMenu();
        break;
      case "Museum_Rearrange_Yes":
        this.OpenRearrangeMenu();
        break;
    }
    return base.answerDialogueAction(questionAndAnswer, questionParams);
  }

  public string getRewardItemKey(Item item)
  {
    return "museumCollectedReward" + Utility.getStandardDescriptionFromItem(item, 1, '_');
  }

  /// <inheritdoc />
  public override bool performAction(string[] action, Farmer who, Location tileLocation)
  {
    if (who.IsLocalPlayer)
    {
      switch (ArgUtility.Get(action, 0))
      {
        case "Gunther":
          this.OpenGuntherDialogueMenu();
          return true;
        case "Rearrange":
          if (!this.doesFarmerHaveAnythingToDonate(Game1.player))
          {
            if (LibraryMuseum.HasDonatedArtifacts())
              this.createQuestionDialogue(Game1.content.LoadString("Strings\\Locations:ArchaeologyHouse_Rearrange"), this.createYesNoResponses(), "Museum_Rearrange");
            return true;
          }
          break;
      }
    }
    return base.performAction(action, who, tileLocation);
  }

  /// <summary>Get the reward items which can be collected by a player.</summary>
  /// <param name="player">The player collecting rewards.</param>
  public List<Item> getRewardsForPlayer(Farmer player)
  {
    this._itemToRewardsLookup.Clear();
    Dictionary<string, MuseumRewards> museumRewardData = DataLoader.MuseumRewards(Game1.content);
    Dictionary<string, int> donatedByContextTag = this.GetDonatedByContextTag(museumRewardData);
    List<Item> rewards = new List<Item>();
    foreach (KeyValuePair<string, MuseumRewards> keyValuePair in museumRewardData)
    {
      string key = keyValuePair.Key;
      MuseumRewards museumRewards = keyValuePair.Value;
      if (this.CanCollectReward(museumRewards, key, player, donatedByContextTag))
      {
        bool flag = false;
        if (museumRewards.RewardItemId != null)
        {
          Item obj = ItemRegistry.Create(museumRewards.RewardItemId, museumRewards.RewardItemCount);
          obj.IsRecipe = museumRewards.RewardItemIsRecipe;
          obj.specialItem = museumRewards.RewardItemIsSpecial;
          if (this.AddRewardItemIfUncollected(player, rewards, obj))
          {
            this._itemToRewardsLookup[obj] = key;
            flag = true;
          }
        }
        if (!flag)
          this.AddNonItemRewards(museumRewards, key, player);
      }
    }
    return rewards;
  }

  /// <summary>Give the player a set of non-item donation rewards.</summary>
  /// <param name="data">The museum donation rewards to give to the player.</param>
  /// <param name="rewardId">The unique ID for <paramref name="data" />.</param>
  /// <param name="player">The player collecting rewards.</param>
  public void AddNonItemRewards(MuseumRewards data, string rewardId, Farmer player)
  {
    if (data.FlagOnCompletion)
      player.mailReceived.Add(rewardId);
    if (data.RewardActions == null)
      return;
    foreach (string rewardAction in data.RewardActions)
    {
      string error;
      Exception exception;
      if (!TriggerActionManager.TryRunAction(rewardAction, out error, out exception))
        Game1.log.Error($"Museum reward {rewardId} ignored invalid event action '{rewardAction}': {error}", exception);
    }
  }

  /// <summary>Add the item to the reward list only if the item hasn't been marked as collected.</summary>
  /// <param name="player">The player collecting rewards.</param>
  /// <param name="rewards">The list of rewards to update.</param>
  /// <param name="rewardItem">The reward to add if it's uncollected.</param>
  public bool AddRewardItemIfUncollected(Farmer player, List<Item> rewards, Item rewardItem)
  {
    if (player.mailReceived.Contains(this.getRewardItemKey(rewardItem)))
      return false;
    rewards.Add(rewardItem);
    return true;
  }

  /// <summary>Get whether the player can collect an item from the reward menu.</summary>
  /// <param name="item">The item to check.</param>
  public bool HighlightCollectableRewards(Item item)
  {
    return Game1.player.couldInventoryAcceptThisItem(item);
  }

  /// <summary>Open the artifact rearranging menu.</summary>
  public void OpenRearrangeMenu()
  {
    if (this.mutex.IsLocked())
      return;
    this.mutex.RequestLock((Action) (() =>
    {
      Game1.activeClickableMenu = (IClickableMenu) new MuseumMenu(new InventoryMenu.highlightThisItem(InventoryMenu.highlightNoItems))
      {
        exitFunction = new IClickableMenu.onExit(this.mutex.ReleaseLock)
      };
    }));
  }

  /// <summary>Open the reward collection menu.</summary>
  public void OpenRewardMenu()
  {
    Game1.activeClickableMenu = (IClickableMenu) new ItemGrabMenu((IList<Item>) this.getRewardsForPlayer(Game1.player), false, true, new InventoryMenu.highlightThisItem(this.HighlightCollectableRewards), (ItemGrabMenu.behaviorOnItemSelect) null, "Rewards", new ItemGrabMenu.behaviorOnItemSelect(this.OnRewardCollected), canBeExitedWithKey: true, playRightClickSound: false, allowRightClick: false, context: (object) this, allowExitWithHeldItem: true);
  }

  /// <summary>Open the artifact donation menu.</summary>
  public void OpenDonationMenu()
  {
    this.mutex.RequestLock((Action) (() =>
    {
      Game1.activeClickableMenu = (IClickableMenu) new MuseumMenu(new InventoryMenu.highlightThisItem(this.isItemSuitableForDonation))
      {
        exitFunction = new IClickableMenu.onExit(this.OnDonationMenuClosed)
      };
    }));
  }

  /// <summary>Handle the player closing the artifact donation screen.</summary>
  public void OnDonationMenuClosed()
  {
    this.mutex.ReleaseLock();
    this.getRewardsForPlayer(Game1.player);
  }

  /// <summary>Handle the player collecting an item from the reward screen.</summary>
  /// <param name="item">The item that was collected.</param>
  /// <param name="who">The player collecting rewards.</param>
  public void OnRewardCollected(Item item, Farmer who)
  {
    if (item == null)
      return;
    string str;
    if (item is StardewValley.Object && this._itemToRewardsLookup.TryGetValue(item, out str))
    {
      MuseumRewards data;
      if (DataLoader.MuseumRewards(Game1.content).TryGetValue(str, out data))
        this.AddNonItemRewards(data, str, who);
      this._itemToRewardsLookup.Remove(item);
    }
    if (who.hasOrWillReceiveMail(this.getRewardItemKey(item)))
      return;
    who.mailReceived.Add(this.getRewardItemKey(item));
    if (!item.QualifiedItemId.Equals("(O)499"))
      return;
    who.craftingRecipes.TryAdd("Ancient Seeds", 0);
  }

  /// <summary>Open the dialogue menu for Gunther.</summary>
  private void OpenGuntherDialogueMenu()
  {
    if (this.doesFarmerHaveAnythingToDonate(Game1.player) && !this.mutex.IsLocked())
    {
      Response[] answerChoices;
      if (this.getRewardsForPlayer(Game1.player).Count > 0)
        answerChoices = new Response[3]
        {
          new Response("Donate", Game1.content.LoadString("Strings\\Locations:ArchaeologyHouse_Gunther_Donate")),
          new Response("Collect", Game1.content.LoadString("Strings\\Locations:ArchaeologyHouse_Gunther_Collect")),
          new Response("Leave", Game1.content.LoadString("Strings\\Locations:ArchaeologyHouse_Gunther_Leave"))
        };
      else
        answerChoices = new Response[2]
        {
          new Response("Donate", Game1.content.LoadString("Strings\\Locations:ArchaeologyHouse_Gunther_Donate")),
          new Response("Leave", Game1.content.LoadString("Strings\\Locations:ArchaeologyHouse_Gunther_Leave"))
        };
      this.createQuestionDialogue("", answerChoices, "Museum");
    }
    else if (this.getRewardsForPlayer(Game1.player).Count > 0)
      this.createQuestionDialogue("", new Response[2]
      {
        new Response("Collect", Game1.content.LoadString("Strings\\Locations:ArchaeologyHouse_Gunther_Collect")),
        new Response("Leave", Game1.content.LoadString("Strings\\Locations:ArchaeologyHouse_Gunther_Leave"))
      }, "Museum");
    else if (this.doesFarmerHaveAnythingToDonate(Game1.player) && this.mutex.IsLocked())
    {
      Game1.drawObjectDialogue(Game1.content.LoadString("Strings\\UI:NPC_Busy", (object) Game1.RequireCharacter("Gunther").displayName));
    }
    else
    {
      NPC characterFromName = Game1.getCharacterFromName("Gunther");
      if (Game1.player.achievements.Contains(5))
        Game1.DrawDialogue(new Dialogue(characterFromName, "Data\\ExtraDialogue:Gunther_MuseumComplete", Game1.parseText(Game1.content.LoadString("Data\\ExtraDialogue:Gunther_MuseumComplete"))));
      else if (Game1.player.mailReceived.Contains("artifactFound"))
        Game1.DrawDialogue(new Dialogue(characterFromName, "Data\\ExtraDialogue:Gunther_NothingToDonate", Game1.parseText(Game1.content.LoadString("Data\\ExtraDialogue:Gunther_NothingToDonate"))));
      else
        Game1.DrawDialogue(characterFromName, "Data\\ExtraDialogue:Gunther_NoArtifactsFound");
    }
  }

  public override bool checkAction(Location tileLocation, xTile.Dimensions.Rectangle viewport, Farmer who)
  {
    string str;
    if (!this.museumPieces.TryGetValue(new Vector2((float) tileLocation.X, (float) tileLocation.Y), out str) && !this.museumPieces.TryGetValue(new Vector2((float) tileLocation.X, (float) (tileLocation.Y - 1)), out str))
      return base.checkAction(tileLocation, viewport, who);
    ParsedItemData dataOrErrorItem = ItemRegistry.GetDataOrErrorItem("(O)" + str);
    Game1.drawObjectDialogue(Game1.parseText($" - {dataOrErrorItem.DisplayName} - ^{dataOrErrorItem.Description}"));
    return true;
  }

  public bool isTileSuitableForMuseumPiece(int x, int y)
  {
    if (!LibraryMuseum.HasDonatedArtifactAt(new Vector2((float) x, (float) y)))
    {
      switch (this.getTileIndexAt(x, y, "Buildings", "untitled tile sheet"))
      {
        case 1072:
        case 1073:
        case 1074:
        case 1237:
        case 1238:
          return true;
      }
    }
    return false;
  }

  /// <summary>Get a count of donated items by context tag.</summary>
  /// <param name="museumRewardData">The museum rewards for which to count context tags.</param>
  public Dictionary<string, int> GetDonatedByContextTag(
    Dictionary<string, MuseumRewards> museumRewardData)
  {
    Dictionary<string, int> donatedByContextTag = new Dictionary<string, int>();
    foreach (MuseumRewards museumRewards in museumRewardData.Values)
    {
      foreach (MuseumDonationRequirement targetContextTag in museumRewards.TargetContextTags)
        donatedByContextTag[targetContextTag.Tag] = 0;
    }
    string[] array = donatedByContextTag.Keys.ToArray<string>();
    foreach (string itemId in this.museumPieces.Values)
    {
      foreach (string str in array)
      {
        if (str == "" || ItemContextTagManager.HasBaseTag(itemId, str))
          ++donatedByContextTag[str];
      }
    }
    return donatedByContextTag;
  }

  /// <summary>Get whether a reward can be collected by a player.</summary>
  /// <param name="reward">The reward data to check.</param>
  /// <param name="rewardId">The unique ID for the <paramref name="reward" />.</param>
  /// <param name="player">The player collecting rewards.</param>
  /// <param name="countsByTag">The number of donated items matching each context tag.</param>
  public bool CanCollectReward(
    MuseumRewards reward,
    string rewardId,
    Farmer player,
    Dictionary<string, int> countsByTag)
  {
    if (reward.FlagOnCompletion && player.mailReceived.Contains(rewardId))
      return false;
    foreach (MuseumDonationRequirement targetContextTag in reward.TargetContextTags)
    {
      if (targetContextTag.Tag == "" && targetContextTag.Count == -1)
      {
        if (countsByTag[targetContextTag.Tag] < LibraryMuseum.totalArtifacts)
          return false;
      }
      else if (countsByTag[targetContextTag.Tag] < targetContextTag.Count)
        return false;
    }
    if (reward.RewardItemId != null)
    {
      if (player.canUnderstandDwarves && ItemRegistry.QualifyItemId(reward.RewardItemId) == "(O)326")
        return false;
      if (reward.RewardItemIsSpecial)
      {
        ParsedItemData dataOrErrorItem = ItemRegistry.GetDataOrErrorItem(reward.RewardItemId);
        if ((dataOrErrorItem.HasTypeId("(F)") || dataOrErrorItem.HasTypeBigCraftable() ? (NetList<string, NetString>) player.specialBigCraftables : (NetList<string, NetString>) player.specialItems).Contains(dataOrErrorItem.ItemId))
          return false;
      }
    }
    return true;
  }

  public Microsoft.Xna.Framework.Rectangle getMuseumDonationBounds()
  {
    return new Microsoft.Xna.Framework.Rectangle(26, 5, 22, 13);
  }

  public Vector2 getFreeDonationSpot()
  {
    Microsoft.Xna.Framework.Rectangle museumDonationBounds = this.getMuseumDonationBounds();
    for (int x = museumDonationBounds.X; x <= museumDonationBounds.Right; ++x)
    {
      for (int y = museumDonationBounds.Y; y <= museumDonationBounds.Bottom; ++y)
      {
        if (this.isTileSuitableForMuseumPiece(x, y))
          return new Vector2((float) x, (float) y);
      }
    }
    return new Vector2(26f, 5f);
  }

  public Vector2 findMuseumPieceLocationInDirection(
    Vector2 startingPoint,
    int direction,
    int distanceToCheck = 8,
    bool ignoreExistingItems = true)
  {
    Vector2 tile = startingPoint;
    Vector2 vector2 = Vector2.Zero;
    switch (direction)
    {
      case 0:
        vector2 = new Vector2(0.0f, -1f);
        break;
      case 1:
        vector2 = new Vector2(1f, 0.0f);
        break;
      case 2:
        vector2 = new Vector2(0.0f, 1f);
        break;
      case 3:
        vector2 = new Vector2(-1f, 0.0f);
        break;
    }
    for (int index1 = 0; index1 < distanceToCheck; ++index1)
    {
      for (int index2 = 0; index2 < distanceToCheck; ++index2)
      {
        tile += vector2;
        if (this.isTileSuitableForMuseumPiece((int) tile.X, (int) tile.Y) || !ignoreExistingItems && LibraryMuseum.HasDonatedArtifactAt(tile))
          return tile;
      }
      tile = startingPoint;
      int num = index1 % 2 == 0 ? -1 : 1;
      switch (direction)
      {
        case 0:
        case 2:
          tile.X += (float) (num * (index1 / 2 + 1));
          break;
        case 1:
        case 3:
          tile.Y += (float) (num * (index1 / 2 + 1));
          break;
      }
    }
    return startingPoint;
  }

  public override void drawAboveAlwaysFrontLayer(SpriteBatch b)
  {
    foreach (TemporaryAnimatedSprite temporarySprite in this.temporarySprites)
    {
      if ((double) temporarySprite.layerDepth >= 1.0)
        temporarySprite.draw(b);
    }
  }

  public override void draw(SpriteBatch b)
  {
    base.draw(b);
    foreach (KeyValuePair<Vector2, string> pair in this.museumPieces.Pairs)
    {
      b.Draw(Game1.shadowTexture, Game1.GlobalToLocal(Game1.viewport, pair.Key * 64f + new Vector2(32f, 52f)), new Microsoft.Xna.Framework.Rectangle?(Game1.shadowTexture.Bounds), Color.White, 0.0f, new Vector2((float) Game1.shadowTexture.Bounds.Center.X, (float) Game1.shadowTexture.Bounds.Center.Y), 4f, SpriteEffects.None, (float) (((double) pair.Key.Y * 64.0 - 2.0) / 10000.0));
      ParsedItemData dataOrErrorItem = ItemRegistry.GetDataOrErrorItem("(O)" + pair.Value);
      b.Draw(dataOrErrorItem.GetTexture(), Game1.GlobalToLocal(Game1.viewport, pair.Key * 64f), new Microsoft.Xna.Framework.Rectangle?(dataOrErrorItem.GetSourceRect()), Color.White, 0.0f, Vector2.Zero, 4f, SpriteEffects.None, (float) ((double) pair.Key.Y * 64.0 / 10000.0));
    }
  }
}
