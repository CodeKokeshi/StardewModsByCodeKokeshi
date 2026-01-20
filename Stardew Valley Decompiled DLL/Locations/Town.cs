// Decompiled with JetBrains decompiler
// Type: StardewValley.Locations.Town
// Assembly: Stardew Valley, Version=1.6.15.24356, Culture=neutral, PublicKeyToken=null
// MVID: AA1F513A-94F0-4EF6-A35F-2D5B4A3721BF
// Assembly location: D:\SteamLibrary\steamapps\common\Stardew Valley\Stardew Valley.dll
// XML documentation location: D:\SteamLibrary\steamapps\common\Stardew Valley\Stardew Valley.xml

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Netcode;
using StardewValley.BellsAndWhistles;
using StardewValley.Extensions;
using StardewValley.Network;
using StardewValley.Network.NetEvents;
using StardewValley.Objects;
using StardewValley.SpecialOrders;
using StardewValley.TerrainFeatures;
using System;
using System.Xml.Serialization;
using xTile.Dimensions;
using xTile.Layers;
using xTile.Tiles;

#nullable disable
namespace StardewValley.Locations;

public class Town : GameLocation
{
  private TemporaryAnimatedSprite minecartSteam;
  private bool ccRefurbished;
  private bool ccJoja;
  private bool playerCheckedBoard;
  private bool isShowingDestroyedJoja;
  private bool isShowingUpgradedPamHouse;
  private bool isShowingSpecialOrdersBoard;
  private bool showBookseller;
  private LocalizedContentManager mapLoader;
  [XmlElement("daysUntilCommunityUpgrade")]
  public readonly NetInt daysUntilCommunityUpgrade = new NetInt(0);
  private Vector2 clockCenter = new Vector2(3392f, 1056f);
  private Vector2 ccFacadePosition = new Vector2(3044f, 940f);
  private Vector2 ccFacadePositionBottom = new Vector2(3044f, 1140f);
  public static Microsoft.Xna.Framework.Rectangle minuteHandSource = new Microsoft.Xna.Framework.Rectangle(363, 395, 5, 13);
  public static Microsoft.Xna.Framework.Rectangle hourHandSource = new Microsoft.Xna.Framework.Rectangle(369, 399, 5, 9);
  public static Microsoft.Xna.Framework.Rectangle clockNub = new Microsoft.Xna.Framework.Rectangle(375, 404, 4, 4);
  public static Microsoft.Xna.Framework.Rectangle jojaFacadeTop = new Microsoft.Xna.Framework.Rectangle(424, 1275, 174, 50);
  public static Microsoft.Xna.Framework.Rectangle jojaFacadeBottom = new Microsoft.Xna.Framework.Rectangle(424, 1325, 174, 51);
  public static Microsoft.Xna.Framework.Rectangle jojaFacadeWinterOverlay = new Microsoft.Xna.Framework.Rectangle(66, 1678, 174, 25);

  protected override void initNetFields()
  {
    base.initNetFields();
    this.NetFields.AddField((INetSerializable) this.daysUntilCommunityUpgrade, "daysUntilCommunityUpgrade");
  }

  public Town()
  {
  }

  public Town(string map, string name)
    : base(map, name)
  {
  }

  protected override LocalizedContentManager getMapLoader()
  {
    if (this.mapLoader == null)
      this.mapLoader = Game1.game1.xTileContent.CreateTemporary();
    return this.mapLoader;
  }

  public override void UpdateMapSeats()
  {
    base.UpdateMapSeats();
    if (!Game1.IsMasterGame)
      return;
    this.mapSeats.RemoveWhere((Func<MapSeat, bool>) (seat => (double) seat.tilePosition.Value.X == 24.0 && (double) seat.tilePosition.Value.Y == 13.0 && seat.seatType.Value == "swings"));
  }

  public override void performTenMinuteUpdate(int timeOfDay)
  {
    base.performTenMinuteUpdate(timeOfDay);
    if (!Game1.isStartingToGetDarkOut((GameLocation) this))
      this.addClintMachineGraphics();
    else
      AmbientLocationSounds.removeSound(new Vector2(100f, 79f));
  }

  public void checkedBoard() => this.playerCheckedBoard = true;

  private void addClintMachineGraphics()
  {
    TemporaryAnimatedSprite temporaryAnimatedSprite1 = TemporaryAnimatedSprite.GetTemporaryAnimatedSprite("LooseSprites\\Cursors", new Microsoft.Xna.Framework.Rectangle(302, 1946, 15, 16 /*0x10*/), (float) (Game1.realMilliSecondsPerGameTenMinutes - Game1.gameTimeInterval), 1, 1, new Vector2(100f, 79f) * 64f + new Vector2(9f, 6f) * 4f, false, false, 0.5188f, 0.0f, Color.White, 4f, 0.0f, 0.0f, 0.0f);
    temporaryAnimatedSprite1.shakeIntensity = 1f;
    this.temporarySprites.Add(temporaryAnimatedSprite1);
    for (int index = 0; index < 10; ++index)
      Utility.addSmokePuff((GameLocation) this, new Vector2(101f, 78f) * 64f + new Vector2(4f, 4f) * 4f, index * ((Game1.realMilliSecondsPerGameTenMinutes - Game1.gameTimeInterval) / 16 /*0x10*/));
    Microsoft.Xna.Framework.Rectangle sourceRect = this.IsFallHere() ? new Microsoft.Xna.Framework.Rectangle(304, 256 /*0x0100*/, 5, 18) : new Microsoft.Xna.Framework.Rectangle(643, 1305, 5, 18);
    for (int index1 = 0; index1 < Game1.random.Next(1, 4); ++index1)
    {
      for (int index2 = 0; index2 < 16 /*0x10*/; ++index2)
      {
        TemporaryAnimatedSprite temporaryAnimatedSprite2 = TemporaryAnimatedSprite.GetTemporaryAnimatedSprite("LooseSprites\\Cursors", sourceRect, 50f, 4, 1, new Vector2(100f, 78f) * 64f + new Vector2((float) (-5 - index2 * 4), 0.0f) * 4f, false, false, 1f, 0.0f, Color.White, 4f, 0.0f, 0.0f, 0.0f);
        temporaryAnimatedSprite2.delayBeforeAnimationStart = index1 * 1500 + 100 * index2;
        this.temporarySprites.Add(temporaryAnimatedSprite2);
      }
      Utility.addSmokePuff((GameLocation) this, new Vector2(100f, 78f) * 64f + new Vector2(-70f, -6f) * 4f, index1 * 1500 + 1600);
    }
  }

  public override void DayUpdate(int dayOfMonth)
  {
    base.DayUpdate(dayOfMonth);
    this.showBookseller = false;
    if (Game1.dayOfMonth == 2 && Game1.IsSpring && !Game1.MasterPlayer.mailReceived.Contains("JojaMember") && this.CanItemBePlacedHere(new Vector2(57f, 16f)))
      this.objects.Add(new Vector2(57f, 16f), ItemRegistry.Create<StardewValley.Object>("(BC)55"));
    if (this.daysUntilCommunityUpgrade.Value > 0)
    {
      --this.daysUntilCommunityUpgrade.Value;
      if (this.daysUntilCommunityUpgrade.Value <= 0)
      {
        if (!Game1.MasterPlayer.mailReceived.Contains("pamHouseUpgrade"))
        {
          Game1.player.team.RequestSetMail(PlayerActionTarget.Host, "pamHouseUpgrade", MailType.Received, true);
          Game1.player.changeFriendship(1000, Game1.getCharacterFromName("Pam"));
        }
        else
          Game1.player.team.RequestSetMail(PlayerActionTarget.Host, "communityUpgradeShortcuts", MailType.Received, true);
      }
    }
    if (Game1.IsFall && Game1.dayOfMonth == 17)
    {
      this.tryPlaceObject(new Vector2(9f, 86f), ItemRegistry.Create<StardewValley.Object>("(O)746"));
      this.tryPlaceObject(new Vector2(21f, 89f), ItemRegistry.Create<StardewValley.Object>("(O)746"));
      this.tryPlaceObject(new Vector2(70f, 69f), ItemRegistry.Create<StardewValley.Object>("(O)746"));
      this.tryPlaceObject(new Vector2(63f, 63f), ItemRegistry.Create<StardewValley.Object>("(O)746"));
      if (this.ccRefurbished && !Game1.MasterPlayer.mailReceived.Contains("JojaMember"))
      {
        this.tryPlaceObject(new Vector2(50f, 21f), ItemRegistry.Create<StardewValley.Object>("(O)746"));
        this.tryPlaceObject(new Vector2(55f, 21f), ItemRegistry.Create<StardewValley.Object>("(O)746"));
      }
      if (!this.isObjectAtTile(41, 85))
      {
        NetCollection<Furniture> furniture1 = this.furniture;
        Furniture furniture2 = new Furniture("1369", new Vector2(41f, 85f));
        furniture2.CanBeGrabbed = false;
        furniture1.Add(furniture2);
      }
      if (!this.isObjectAtTile(48 /*0x30*/, 86))
      {
        NetCollection<Furniture> furniture3 = this.furniture;
        Furniture furniture4 = new Furniture("1369", new Vector2(48f, 86f));
        furniture4.CanBeGrabbed = false;
        furniture3.Add(furniture4);
      }
      if (!this.isObjectAtTile(43, 89))
      {
        NetCollection<Furniture> furniture5 = this.furniture;
        Furniture furniture6 = new Furniture("1369", new Vector2(43f, 89f));
        furniture6.CanBeGrabbed = false;
        furniture5.Add(furniture6);
      }
      if (!this.isObjectAtTile(52, 86))
      {
        NetCollection<Furniture> furniture7 = this.furniture;
        Furniture furniture8 = new Furniture("1369", new Vector2(52f, 86f));
        furniture8.CanBeGrabbed = false;
        furniture7.Add(furniture8);
      }
    }
    if (!Game1.IsWinter || Game1.dayOfMonth != 1)
      return;
    if (!this.objects.ContainsKey(new Vector2(41f, 85f)))
      this.removeEverythingFromThisTile(41, 85);
    if (!this.objects.ContainsKey(new Vector2(48f, 86f)))
      this.removeEverythingFromThisTile(48 /*0x30*/, 86);
    if (!this.objects.ContainsKey(new Vector2(43f, 89f)))
      this.removeEverythingFromThisTile(43, 89);
    if (!this.objects.ContainsKey(new Vector2(52f, 86f)))
      this.removeEverythingFromThisTile(52, 86);
    this.removeObjectAtTileWithName(9, 86, "Rotten Plant");
    this.removeObjectAtTileWithName(21, 89, "Rotten Plant");
    this.removeObjectAtTileWithName(70, 96 /*0x60*/, "Rotten Plant");
    this.removeObjectAtTileWithName(63 /*0x3F*/, 63 /*0x3F*/, "Rotten Plant");
    this.removeObjectAtTileWithName(50, 21, "Rotten Plant");
    this.removeObjectAtTileWithName(55, 21, "Rotten Plant");
  }

  public bool removeObjectAtTileWithName(int x, int y, string name)
  {
    Vector2 key = new Vector2((float) x, (float) y);
    StardewValley.Object @object;
    if (!this.objects.TryGetValue(key, out @object) || !(@object.Name == name))
      return false;
    this.objects.Remove(key);
    return true;
  }

  public override string checkForBuriedItem(
    int xLocation,
    int yLocation,
    bool explosion,
    bool detectOnly,
    Farmer who)
  {
    if (!who.secretNotesSeen.Contains(17) || xLocation != 98 || yLocation != 5 || !who.mailReceived.Add("SecretNote17_done"))
      return base.checkForBuriedItem(xLocation, yLocation, explosion, detectOnly, who);
    Game1.createObjectDebris("(O)126", xLocation, yLocation, who.UniqueMultiplayerID, (GameLocation) this);
    return "";
  }

  /// <inheritdoc />
  public override bool CanPlantTreesHere(
    string itemId,
    int tileX,
    int tileY,
    out string deniedMessage)
  {
    if (!(this.doesTileHavePropertyNoNull(tileX, tileY, "Type", "Back") != "Dirt"))
      return this.CheckItemPlantRules(itemId, false, false, out deniedMessage);
    deniedMessage = (string) null;
    return false;
  }

  public override bool checkAction(Location tileLocation, xTile.Dimensions.Rectangle viewport, Farmer who)
  {
    Layer layer = this.map.RequireLayer("Buildings");
    if (who.mount == null)
    {
      switch (layer.GetTileIndexAt(tileLocation, nameof (Town)))
      {
        case 599:
          if (Game1.player.secretNotesSeen.Contains(19) && Game1.player.mailReceived.Add("SecretNote19_done"))
          {
            DelayedAction.playSoundAfterDelay("newArtifact", 250);
            Game1.player.addItemByMenuIfNecessary(ItemRegistry.Create("(BC)164"));
            break;
          }
          break;
        case 620:
          if (Utility.HasAnyPlayerSeenEvent("191393"))
            Game1.drawObjectDialogue(Game1.content.LoadString("Strings\\Locations:Town_SeedShopSign").Replace('\n', '^'));
          else
            Game1.drawObjectDialogue($"{Game1.content.LoadString("Strings\\Locations:Town_SeedShopSign").Split('\n')[0]}^{Game1.content.LoadString("Strings\\Locations:SeedShop_LockedWed")}");
          return true;
        case 1080:
        case 1081:
          if (Game1.player.mount != null)
            return true;
          bool? isFestival = this.currentEvent?.isFestival;
          if (isFestival.HasValue && isFestival.GetValueOrDefault() && this.currentEvent.checkAction(tileLocation, viewport, who))
            return true;
          Game1.drawObjectDialogue(Game1.content.LoadString("Strings\\Locations:Town_PickupTruck"));
          return true;
        case 1913:
        case 1914:
        case 1945:
        case 1946:
          if (this.isShowingDestroyedJoja)
          {
            Game1.drawObjectDialogue(Game1.content.LoadString("Strings\\Locations:Town_JojaSign_Destroyed"));
            return true;
          }
          break;
        case 1935:
        case 2270:
          if (Game1.player.secretNotesSeen.Contains(20) && !Game1.player.mailReceived.Contains("SecretNote20_done"))
          {
            this.createQuestionDialogue(Game1.content.LoadString("Strings\\Locations:Town_SpecialCharmQuestion"), this.createYesNoResponses(), "specialCharmQuestion");
            break;
          }
          break;
        case 2000:
        case 2001:
        case 2032:
        case 2033:
          if (this.isShowingDestroyedJoja)
          {
            Rumble.rumble(0.15f, 200f);
            Game1.player.completelyStopAnimatingOrDoingAction();
            this.playSound("stairsdown", new Vector2?(Game1.player.Tile));
            Game1.warpFarmer("AbandonedJojaMart", 9, 13, false);
            return true;
          }
          break;
      }
      switch (layer.GetTileIndexAt(tileLocation, "Landscape"))
      {
        case 958:
        case 1080:
        case 1081:
          if (!Game1.isFestival())
          {
            this.ShowMineCartMenu("Default", nameof (Town));
            return true;
          }
          break;
      }
    }
    return base.checkAction(tileLocation, viewport, who);
  }

  public void crackOpenAbandonedJojaMartDoor()
  {
    this.setMapTile(95, 49, 2000, "Buildings", nameof (Town));
    this.setMapTile(96 /*0x60*/, 49, 2001, "Buildings", nameof (Town));
    this.setMapTile(95, 50, 2032, "Buildings", nameof (Town));
    this.setMapTile(96 /*0x60*/, 50, 2033, "Buildings", nameof (Town));
  }

  private void refurbishCommunityCenter()
  {
    if (this.ccRefurbished)
      return;
    this.ccRefurbished = true;
    if (Game1.MasterPlayer.mailReceived.Contains("JojaMember"))
      this.ccJoja = true;
    if (this._appliedMapOverrides != null)
    {
      if (this._appliedMapOverrides.Contains("ccRefurbished"))
        return;
      this._appliedMapOverrides.Add("ccRefurbished");
    }
    Microsoft.Xna.Framework.Rectangle rectangle = new Microsoft.Xna.Framework.Rectangle(47, 11, 11, 9);
    Layer layer1 = this.map.RequireLayer("Back");
    Layer layer2 = this.map.RequireLayer("Buildings");
    Layer layer3 = this.map.RequireLayer("Front");
    Layer layer4 = this.map.RequireLayer("AlwaysFront");
    for (int x = rectangle.X; x <= rectangle.Right; ++x)
    {
      for (int y = rectangle.Y; y <= rectangle.Bottom; ++y)
      {
        if (layer1.GetTileIndexAt(x, y, nameof (Town)) > 1200)
          layer1.Tiles[x, y].TileIndex += 12;
        if (layer2.GetTileIndexAt(x, y, nameof (Town)) > 1200)
          layer2.Tiles[x, y].TileIndex += 12;
        if (layer3.GetTileIndexAt(x, y, nameof (Town)) > 1200)
          layer3.Tiles[x, y].TileIndex += 12;
        if (layer4.GetTileIndexAt(x, y, nameof (Town)) > 1200)
          layer4.Tiles[x, y].TileIndex += 12;
      }
    }
  }

  private void showDestroyedJoja()
  {
    if (this.isShowingDestroyedJoja)
      return;
    this.isShowingDestroyedJoja = true;
    if (this._appliedMapOverrides != null && this._appliedMapOverrides.Contains("isShowingDestroyedJoja"))
      return;
    this._appliedMapOverrides.Add("isShowingDestroyedJoja");
    Microsoft.Xna.Framework.Rectangle rectangle = new Microsoft.Xna.Framework.Rectangle(90, 42, 11, 9);
    Layer layer1 = this.map.RequireLayer("Back");
    Layer layer2 = this.map.RequireLayer("Buildings");
    Layer layer3 = this.map.RequireLayer("Front");
    Layer layer4 = this.map.RequireLayer("AlwaysFront");
    for (int x = rectangle.X; x <= rectangle.Right; ++x)
    {
      for (int y = rectangle.Y; y <= rectangle.Bottom; ++y)
      {
        int num = x > rectangle.X + 6 ? 1 : (y < rectangle.Y + 9 ? 1 : 0);
        if (num != 0 && layer1.GetTileIndexAt(x, y, nameof (Town)) > 1200)
          layer1.Tiles[x, y].TileIndex += 20;
        if (num != 0 && layer2.GetTileIndexAt(x, y, nameof (Town)) > 1200)
          layer2.Tiles[x, y].TileIndex += 20;
        if (num != 0 && (x != 93 && y != 50 || x != 94 && y != 50) && layer3.GetTileIndexAt(x, y, nameof (Town)) > 1200)
          layer3.Tiles[x, y].TileIndex += 20;
        if (num != 0 && layer4.GetTileIndexAt(x, y, nameof (Town)) > 1200)
          layer4.Tiles[x, y].TileIndex += 20;
      }
    }
  }

  public override bool isTileFishable(int tileX, int tileY)
  {
    return this.GetSeason() != Season.Winter && tileY == 26 && (tileX == 25 || tileX == 26 || tileX == 27) || tileX == 25 && tileY == 25 || tileX == 27 && tileY == 25 || base.isTileFishable(tileX, tileY);
  }

  public void showImprovedPamHouse()
  {
    if (this.isShowingUpgradedPamHouse)
      return;
    this.isShowingUpgradedPamHouse = true;
    if (this._appliedMapOverrides != null)
    {
      if (this._appliedMapOverrides.Contains("isShowingUpgradedPamHouse"))
        return;
      this._appliedMapOverrides.Add("isShowingUpgradedPamHouse");
    }
    Microsoft.Xna.Framework.Rectangle rect1 = new Microsoft.Xna.Framework.Rectangle(69, 66, 8, 3);
    Microsoft.Xna.Framework.Rectangle rect2 = new Microsoft.Xna.Framework.Rectangle(69, 60, 8, 6);
    Layer layer1 = this.map.RequireLayer("Buildings");
    Layer layer2 = this.map.RequireLayer("Front");
    Layer layer3 = this.map.RequireLayer("AlwaysFront");
    foreach (Point point in rect1.GetPoints())
    {
      int x = point.X;
      int y = point.Y;
      if (layer1.Tiles[x, y] != null)
      {
        layer1.Tiles[x, y].TileIndex += 842;
        if (layer1.GetTileIndexAt(x, y, nameof (Town)) == 1568)
          layer1.Tiles[x, y].TileIndex = 1562;
      }
      if (layer2.Tiles[x, y] != null && y < rect1.Bottom - 1)
        layer2.Tiles[x, y].TileIndex += 842;
    }
    foreach (Point point in rect2.GetPoints())
    {
      int x = point.X;
      int y = point.Y;
      if (layer3.Tiles[x, y] == null)
        layer3.Tiles[x, y] = (Tile) new StaticTile(layer3, this.map.RequireTileSheet(nameof (Town)), BlendMode.Alpha, 1336 + (x - rect2.X) + (y - rect2.Y) * 32 /*0x20*/);
    }
    if (Game1.eventUp)
      return;
    this.removeTile(63 /*0x3F*/, 68, "Buildings");
    this.removeTile(62, 72, "Buildings");
    this.removeTile(74, 71, "Buildings");
  }

  public static Point GetTheaterTileOffset()
  {
    return Utility.doesMasterPlayerHaveMailReceivedButNotMailForTomorrow("ccMovieTheaterJoja") ? new Point(-43, -31) : new Point(0, 0);
  }

  public override void MakeMapModifications(bool force = false)
  {
    base.MakeMapModifications(force);
    if (force)
    {
      this.isShowingSpecialOrdersBoard = false;
      this.isShowingUpgradedPamHouse = false;
      this.isShowingDestroyedJoja = false;
      this.ccRefurbished = false;
    }
    if (Game1.MasterPlayer.mailReceived.Contains("ccIsComplete") || Game1.MasterPlayer.mailReceived.Contains("JojaMember") || Game1.MasterPlayer.hasCompletedCommunityCenter())
      this.refurbishCommunityCenter();
    if (!this.isShowingSpecialOrdersBoard && SpecialOrder.IsSpecialOrdersBoardUnlocked())
    {
      this.isShowingSpecialOrdersBoard = true;
      LargeTerrainFeature terrainFeatureAt;
      do
      {
        terrainFeatureAt = this.getLargeTerrainFeatureAt(61, 93);
        if (terrainFeatureAt != null)
          this.largeTerrainFeatures.Remove(terrainFeatureAt);
      }
      while (terrainFeatureAt != null);
      this.setMapTile(61, 93, 2045, "Buildings", nameof (Town), "SpecialOrders");
      this.setMapTile(62, 93, 2046, "Buildings", nameof (Town), "SpecialOrders");
      this.setMapTile(63 /*0x3F*/, 93, 2047 /*0x07FF*/, "Buildings", nameof (Town), "SpecialOrders");
      this.setMapTile(61, 92, 2013, "Front", nameof (Town));
      this.setMapTile(62, 92, 2014, "Front", nameof (Town));
      this.setMapTile(63 /*0x3F*/, 92, 2015, "Front", nameof (Town));
      this.cleanUpTileForMapOverride(new Point(60, 93));
      this.setMapTile(60, 93, 2034, "Buildings", nameof (Town), "SpecialOrdersPrizeTickets");
      this.setMapTile(60, 92, 2002, "Front", nameof (Town));
    }
    if (NetWorldState.checkAnywhereForWorldStateID("trashBearDone") && this.currentEvent?.id != "777111")
    {
      if (!Game1.eventUp || this.mapPath.Value == null || !this.mapPath.Value.Contains("Town-Fair"))
        this.ApplyMapOverride("Town-TrashGone", destination_rect: new Microsoft.Xna.Framework.Rectangle?(new Microsoft.Xna.Framework.Rectangle(57, 68, 17, 5)));
      this.ApplyMapOverride("Town-DogHouse", destination_rect: new Microsoft.Xna.Framework.Rectangle?(new Microsoft.Xna.Framework.Rectangle(51, 65, 5, 6)));
      this.removeTile(121, 57, "Buildings");
      this.removeTile(119, 59, "Buildings");
      this.removeTile(124, 56, "Buildings");
      this.removeTile(126, 59, "Buildings");
      this.removeTile((int) sbyte.MaxValue, 60, "Buildings");
      this.removeTile(125, 61, "Buildings");
      this.removeTile(126, 62, "Buildings");
      this.removeTile(119, 64 /*0x40*/, "Buildings");
      this.removeTile(120, 52, "Buildings");
    }
    if (Utility.doesMasterPlayerHaveMailReceivedButNotMailForTomorrow("ccMovieTheater"))
    {
      if (Utility.doesMasterPlayerHaveMailReceivedButNotMailForTomorrow("ccMovieTheaterJoja"))
      {
        Microsoft.Xna.Framework.Rectangle rectangle = new Microsoft.Xna.Framework.Rectangle(46, 11, 15, 17);
        bool flag = Game1.IsFall && Game1.dayOfMonth == 27 && Game1.year % 2 == 0 && this.loadedMapPath.Contains("Halloween");
        if (flag)
          this._appliedMapOverrides.Remove("Town-TheaterCC");
        this.ApplyMapOverride("Town-TheaterCC" + (flag ? "-Halloween2" : ""), new Microsoft.Xna.Framework.Rectangle?(rectangle), new Microsoft.Xna.Framework.Rectangle?(rectangle));
      }
      else
      {
        Microsoft.Xna.Framework.Rectangle rectangle = new Microsoft.Xna.Framework.Rectangle(84, 41, 27, 15);
        this.ApplyMapOverride("Town-Theater", new Microsoft.Xna.Framework.Rectangle?(rectangle), new Microsoft.Xna.Framework.Rectangle?(rectangle));
      }
    }
    else if (Utility.HasAnyPlayerSeenEvent("191393"))
    {
      this.showDestroyedJoja();
      if (Utility.doesMasterPlayerHaveMailReceivedButNotMailForTomorrow("abandonedJojaMartAccessible"))
        this.crackOpenAbandonedJojaMartDoor();
    }
    if (Game1.MasterPlayer.mailReceived.Contains("pamHouseUpgrade"))
      this.showImprovedPamHouse();
    if (!Game1.MasterPlayer.mailReceived.Contains("communityUpgradeShortcuts"))
      return;
    this.showTownCommunityUpgradeShortcuts();
  }

  protected override void resetLocalState()
  {
    base.resetLocalState();
    if (Utility.doesMasterPlayerHaveMailReceivedButNotMailForTomorrow("ccBoilerRoom"))
      this.minecartSteam = new TemporaryAnimatedSprite(27, new Vector2(6856f, 5008f), Color.White)
      {
        totalNumberOfLoops = 999999,
        interval = 60f,
        flipped = true
      };
    if (NetWorldState.checkAnywhereForWorldStateID("trashBearDone") && this.currentEvent?.id != "777111" && !this.IsRainingHere() && Utility.CreateRandom((double) Game1.uniqueIDForThisGame, (double) Game1.stats.DaysPlayed).NextDouble() < 0.2)
      this.TemporarySprites.Add(new TemporaryAnimatedSprite("LooseSprites\\Cursors", new Microsoft.Xna.Framework.Rectangle(348, 1916, 12, 20), 999f, 1, 999999, new Vector2(53f, 67f) * 64f + new Vector2(3f, 2f) * 4f, false, false, 0.98f, 0.0f, Color.White, 4f, 0.0f, 0.0f, 0.0f)
      {
        id = 1
      });
    if (Utility.doesMasterPlayerHaveMailReceivedButNotMailForTomorrow("ccMovieTheater"))
    {
      if (Game1.player.team.theaterBuildDate.Value < 0L)
        Game1.player.team.theaterBuildDate.Value = (long) Game1.Date.TotalDays;
      Point theaterTileOffset = Town.GetTheaterTileOffset();
      MovieTheater.AddMoviePoster((GameLocation) this, (float) ((91 + theaterTileOffset.X) * 64 /*0x40*/ + 32 /*0x20*/), (float) ((48 /*0x30*/ + theaterTileOffset.Y) * 64 /*0x40*/ + 64 /*0x40*/));
      MovieTheater.AddMoviePoster((GameLocation) this, (float) ((93 + theaterTileOffset.X) * 64 /*0x40*/ + 24), (float) ((48 /*0x30*/ + theaterTileOffset.Y) * 64 /*0x40*/ + 64 /*0x40*/), true);
      Vector2 vector2 = new Vector2((float) theaterTileOffset.X, (float) theaterTileOffset.Y);
      Game1.currentLightSources.Add(new LightSource("Town_Theater1", 4, (new Vector2(91f, 46f) + vector2) * 64f, 1f, onlyLocation: this.NameOrUniqueName));
      Game1.currentLightSources.Add(new LightSource("Town_Theater2", 4, (new Vector2(96f, 47f) + vector2) * 64f, 1f, onlyLocation: this.NameOrUniqueName));
      Game1.currentLightSources.Add(new LightSource("Town_Theater3", 4, (new Vector2(100f, 47f) + vector2) * 64f, 1f, onlyLocation: this.NameOrUniqueName));
      Game1.currentLightSources.Add(new LightSource("Town_Theater4", 4, (new Vector2(96f, 45f) + vector2) * 64f, 1f, onlyLocation: this.NameOrUniqueName));
      Game1.currentLightSources.Add(new LightSource("Town_Theater5", 4, (new Vector2(100f, 45f) + vector2) * 64f, 1f, onlyLocation: this.NameOrUniqueName));
      Game1.currentLightSources.Add(new LightSource("Town_Theater6", 4, (new Vector2(97f, 43f) + vector2) * 64f, 1f, onlyLocation: this.NameOrUniqueName));
      Game1.currentLightSources.Add(new LightSource("Town_Theater7", 4, (new Vector2(99f, 43f) + vector2) * 64f, 1f, onlyLocation: this.NameOrUniqueName));
      Game1.currentLightSources.Add(new LightSource("Town_Theater8", 4, (new Vector2(98f, 49f) + vector2) * 64f, 1f, onlyLocation: this.NameOrUniqueName));
      Game1.currentLightSources.Add(new LightSource("Town_Theater9", 4, (new Vector2(92f, 49f) + vector2) * 64f, 1f, onlyLocation: this.NameOrUniqueName));
      Game1.currentLightSources.Add(new LightSource("Town_Theater10", 4, (new Vector2(94f, 49f) + vector2) * 64f, 1f, onlyLocation: this.NameOrUniqueName));
      Game1.currentLightSources.Add(new LightSource("Town_Theater11", 4, (new Vector2(98f, 51f) + vector2) * 64f, 1f, onlyLocation: this.NameOrUniqueName));
      Game1.currentLightSources.Add(new LightSource("Town_Theater12", 4, (new Vector2(92f, 51f) + vector2) * 64f, 1f, onlyLocation: this.NameOrUniqueName));
      Game1.currentLightSources.Add(new LightSource("Town_Theater13", 4, (new Vector2(94f, 51f) + vector2) * 64f, 1f, onlyLocation: this.NameOrUniqueName));
    }
    int num1 = this.IsWinterHere() ? 1 : 0;
    if (num1 == 0)
    {
      AmbientLocationSounds.addSound(new Vector2(26f, 26f), 0);
      AmbientLocationSounds.addSound(new Vector2(26f, 28f), 0);
    }
    if (!Game1.isStartingToGetDarkOut((GameLocation) this))
    {
      AmbientLocationSounds.addSound(new Vector2(100f, 79f), 2);
      this.addClintMachineGraphics();
    }
    if (Game1.stats.DaysPlayed > 3U)
    {
      int endTime = Utility.ModifyTime(1920, Utility.CreateDaySaveRandom(15.0).Next(390));
      int num2 = Utility.CalculateMinutesBetweenTimes(Game1.timeOfDay, endTime) * Game1.realMilliSecondsPerGameMinute;
      if (num2 > 0)
        this.temporarySprites.Add(new TemporaryAnimatedSprite("Characters\\asldkfjsquaskutanfsldk", new Microsoft.Xna.Framework.Rectangle(0, 0, 32 /*0x20*/, 48 /*0x30*/), new Vector2(7788f, 5837f), false, 0.0f, Color.White)
        {
          animationLength = 8,
          totalNumberOfLoops = 99,
          interval = 100f,
          motion = new Vector2(5f, 0.0f),
          scale = 5.5f,
          delayBeforeAnimationStart = num2
        });
    }
    if (Game1.player.mailReceived.Contains("checkedBulletinOnce"))
      this.playerCheckedBoard = true;
    if (num1 != 0 && Game1.player.eventsSeen.Contains("520702") && !Game1.player.hasMagnifyingGlass)
    {
      this.temporarySprites.Add(new TemporaryAnimatedSprite("LooseSprites\\Cursors", new Microsoft.Xna.Framework.Rectangle(247, 407, 6, 6), 2000f, 1, 10000, new Vector2(14.5f, 52.75f) * 64f, false, false, 1E-06f, 0.0f, Color.White, 3f + (float) Game1.random.NextDouble(), 0.0f, 0.0f, 0.0f));
      this.temporarySprites.Add(new TemporaryAnimatedSprite("LooseSprites\\Cursors", new Microsoft.Xna.Framework.Rectangle(247, 407, 6, 6), 2000f, 1, 10000, new Vector2(13.5f, 53f) * 64f, false, false, 1E-06f, 0.0f, Color.White, 3f + (float) Game1.random.NextDouble(), 0.0f, 0.0f, 0.0f));
      this.temporarySprites.Add(new TemporaryAnimatedSprite("LooseSprites\\Cursors", new Microsoft.Xna.Framework.Rectangle(247, 407, 6, 6), 2000f, 1, 10000, new Vector2(15.5f, 53f) * 64f, false, false, 1E-06f, 0.0f, Color.White, 3f + (float) Game1.random.NextDouble(), 0.0f, 0.0f, 0.0f));
      this.temporarySprites.Add(new TemporaryAnimatedSprite("LooseSprites\\Cursors", new Microsoft.Xna.Framework.Rectangle(247, 407, 6, 6), 2000f, 1, 10000, new Vector2(16f, 52.25f) * 64f, false, false, 1E-06f, 0.0f, Color.White, 3f + (float) Game1.random.NextDouble(), 0.0f, 0.0f, 0.0f));
      this.temporarySprites.Add(new TemporaryAnimatedSprite("LooseSprites\\Cursors", new Microsoft.Xna.Framework.Rectangle(247, 407, 6, 6), 2000f, 1, 10000, new Vector2(17f, 52f) * 64f, false, false, 1E-06f, 0.0f, Color.White, 3f + (float) Game1.random.NextDouble(), 0.0f, 0.0f, 0.0f));
      this.temporarySprites.Add(new TemporaryAnimatedSprite("LooseSprites\\Cursors", new Microsoft.Xna.Framework.Rectangle(247, 407, 6, 6), 2000f, 1, 10000, new Vector2(17f, 51f) * 64f + new Vector2(8f, 0.0f) * 4f, false, false, 1E-06f, 0.0f, Color.White, 3f + (float) Game1.random.NextDouble(), 0.0f, 0.0f, 0.0f));
      this.temporarySprites.Add(new TemporaryAnimatedSprite("LooseSprites\\Cursors", new Microsoft.Xna.Framework.Rectangle(247, 407, 6, 6), 2000f, 1, 10000, new Vector2(18f, 51f) * 64f + new Vector2(5f, -7f) * 4f, false, false, 1E-06f, 0.0f, Color.White, 3f + (float) Game1.random.NextDouble(), 0.0f, 0.0f, 0.0f));
      this.temporarySprites.Add(new TemporaryAnimatedSprite("LooseSprites\\Cursors", new Microsoft.Xna.Framework.Rectangle(247, 407, 6, 6), 2000f, 1, 10000, new Vector2(18f, 50f) * 64f + new Vector2(12f, -2f) * 4f, false, false, 1E-06f, 0.0f, Color.White, 3f + (float) Game1.random.NextDouble(), 0.0f, 0.0f, 0.0f));
      this.temporarySprites.Add(new TemporaryAnimatedSprite("LooseSprites\\Cursors", new Microsoft.Xna.Framework.Rectangle(247, 407, 6, 6), 2000f, 1, 10000, new Vector2(21.75f, 39.5f) * 64f, false, false, 1E-06f, 0.0f, Color.White, 3f + (float) Game1.random.NextDouble(), 0.0f, 0.0f, 0.0f));
      this.temporarySprites.Add(new TemporaryAnimatedSprite("LooseSprites\\Cursors", new Microsoft.Xna.Framework.Rectangle(247, 407, 6, 6), 2000f, 1, 10000, new Vector2(21f, 39f) * 64f, false, false, 1E-06f, 0.0f, Color.White, 3f + (float) Game1.random.NextDouble(), 0.0f, 0.0f, 0.0f));
      this.temporarySprites.Add(new TemporaryAnimatedSprite("LooseSprites\\Cursors", new Microsoft.Xna.Framework.Rectangle(247, 407, 6, 6), 2000f, 1, 10000, new Vector2(21.75f, 38.25f) * 64f, false, false, 1E-06f, 0.0f, Color.White, 3f + (float) Game1.random.NextDouble(), 0.0f, 0.0f, 0.0f));
      this.temporarySprites.Add(new TemporaryAnimatedSprite("LooseSprites\\Cursors", new Microsoft.Xna.Framework.Rectangle(247, 407, 6, 6), 2000f, 1, 10000, new Vector2(22.5f, 37.5f) * 64f, false, false, 1E-06f, 0.0f, Color.White, 3f + (float) Game1.random.NextDouble(), 0.0f, 0.0f, 0.0f));
      this.temporarySprites.Add(new TemporaryAnimatedSprite("LooseSprites\\Cursors", new Microsoft.Xna.Framework.Rectangle(247, 407, 6, 6), 2000f, 1, 10000, new Vector2(21.75f, 36.75f) * 64f, false, false, 1E-06f, 0.0f, Color.White, 3f + (float) Game1.random.NextDouble(), 0.0f, 0.0f, 0.0f));
      this.temporarySprites.Add(new TemporaryAnimatedSprite("LooseSprites\\Cursors", new Microsoft.Xna.Framework.Rectangle(247, 407, 6, 6), 2000f, 1, 10000, new Vector2(23f, 36f) * 64f, false, false, 1E-06f, 0.0f, Color.White, 3f + (float) Game1.random.NextDouble(), 0.0f, 0.0f, 0.0f));
      this.temporarySprites.Add(new TemporaryAnimatedSprite("LooseSprites\\Cursors", new Microsoft.Xna.Framework.Rectangle(247, 407, 6, 6), 2000f, 1, 10000, new Vector2(22.25f, 35.25f) * 64f, false, false, 1E-06f, 0.0f, Color.White, 3f + (float) Game1.random.NextDouble(), 0.0f, 0.0f, 0.0f));
      this.temporarySprites.Add(new TemporaryAnimatedSprite("LooseSprites\\Cursors", new Microsoft.Xna.Framework.Rectangle(247, 407, 6, 6), 2000f, 1, 10000, new Vector2(23.5f, 34.6f) * 64f, false, false, 1E-06f, 0.0f, Color.White, 3f + (float) Game1.random.NextDouble(), 0.0f, 0.0f, 0.0f));
      this.temporarySprites.Add(new TemporaryAnimatedSprite("LooseSprites\\Cursors", new Microsoft.Xna.Framework.Rectangle(247, 407, 6, 6), 2000f, 1, 10000, new Vector2(23.5f, 33.6f) * 64f, false, false, 1E-06f, 0.0f, Color.White, 3f + (float) Game1.random.NextDouble(), 0.0f, 0.0f, 0.0f));
      this.temporarySprites.Add(new TemporaryAnimatedSprite("LooseSprites\\Cursors", new Microsoft.Xna.Framework.Rectangle(247, 407, 6, 6), 2000f, 1, 10000, new Vector2(24.25f, 32.6f) * 64f, false, false, 1E-06f, 0.0f, Color.White, 3f + (float) Game1.random.NextDouble(), 0.0f, 0.0f, 0.0f));
      this.temporarySprites.Add(new TemporaryAnimatedSprite("LooseSprites\\Cursors", new Microsoft.Xna.Framework.Rectangle(247, 407, 6, 6), 2000f, 1, 10000, new Vector2(26.75f, 26.75f) * 64f, false, false, 1E-06f, 0.0f, Color.White, 3f + (float) Game1.random.NextDouble(), 0.0f, 0.0f, 0.0f));
      this.temporarySprites.Add(new TemporaryAnimatedSprite("LooseSprites\\Cursors", new Microsoft.Xna.Framework.Rectangle(247, 407, 6, 6), 2000f, 1, 10000, new Vector2(27.5f, 26f) * 64f, false, false, 1E-06f, 0.0f, Color.White, 3f + (float) Game1.random.NextDouble(), 0.0f, 0.0f, 0.0f));
      this.temporarySprites.Add(new TemporaryAnimatedSprite("LooseSprites\\Cursors", new Microsoft.Xna.Framework.Rectangle(247, 407, 6, 6), 2000f, 1, 10000, new Vector2(30f, 23f) * 64f, false, false, 1E-06f, 0.0f, Color.White, 3f + (float) Game1.random.NextDouble(), 0.0f, 0.0f, 0.0f));
      this.temporarySprites.Add(new TemporaryAnimatedSprite("LooseSprites\\Cursors", new Microsoft.Xna.Framework.Rectangle(247, 407, 6, 6), 2000f, 1, 10000, new Vector2(31f, 22f) * 64f, false, false, 1E-06f, 0.0f, Color.White, 3f + (float) Game1.random.NextDouble(), 0.0f, 0.0f, 0.0f));
      this.temporarySprites.Add(new TemporaryAnimatedSprite("LooseSprites\\Cursors", new Microsoft.Xna.Framework.Rectangle(247, 407, 6, 6), 2000f, 1, 10000, new Vector2(30.5f, 21f) * 64f, false, false, 1E-06f, 0.0f, Color.White, 3f + (float) Game1.random.NextDouble(), 0.0f, 0.0f, 0.0f));
      this.temporarySprites.Add(new TemporaryAnimatedSprite("LooseSprites\\Cursors", new Microsoft.Xna.Framework.Rectangle(247, 407, 6, 6), 2000f, 1, 10000, new Vector2(31f, 20f) * 64f, false, false, 1E-06f, 0.0f, Color.White, 3f + (float) Game1.random.NextDouble(), 0.0f, 0.0f, 0.0f));
      this.temporarySprites.Add(new TemporaryAnimatedSprite("LooseSprites\\Cursors", new Microsoft.Xna.Framework.Rectangle(247, 407, 6, 6), 2000f, 1, 10000, new Vector2(30f, 19f) * 64f, false, false, 1E-06f, 0.0f, Color.White, 3f + (float) Game1.random.NextDouble(), 0.0f, 0.0f, 0.0f));
      this.temporarySprites.Add(new TemporaryAnimatedSprite("LooseSprites\\Cursors", new Microsoft.Xna.Framework.Rectangle(247, 407, 6, 6), 2000f, 1, 10000, new Vector2(29f, 18f) * 64f, false, false, 1E-06f, 0.0f, Color.White, 3f + (float) Game1.random.NextDouble(), 0.0f, 0.0f, 0.0f));
      this.temporarySprites.Add(new TemporaryAnimatedSprite("LooseSprites\\Cursors", new Microsoft.Xna.Framework.Rectangle(247, 407, 6, 6), 2000f, 1, 10000, new Vector2(29.1f, 17f) * 64f, false, false, 1E-06f, 0.0f, Color.White, 3f + (float) Game1.random.NextDouble(), 0.0f, 0.0f, 0.0f));
      this.temporarySprites.Add(new TemporaryAnimatedSprite("LooseSprites\\Cursors", new Microsoft.Xna.Framework.Rectangle(247, 407, 6, 6), 2000f, 1, 10000, new Vector2(30f, 17.7f) * 64f, false, false, 1E-06f, 0.0f, Color.White, 3f + (float) Game1.random.NextDouble(), 0.0f, 0.0f, 0.0f));
      this.temporarySprites.Add(new TemporaryAnimatedSprite("LooseSprites\\Cursors", new Microsoft.Xna.Framework.Rectangle(247, 407, 6, 6), 2000f, 1, 10000, new Vector2(31.5f, 18.2f) * 64f, false, false, 1E-06f, 0.0f, Color.White, 3f + (float) Game1.random.NextDouble(), 0.0f, 0.0f, 0.0f));
      this.temporarySprites.Add(new TemporaryAnimatedSprite("LooseSprites\\Cursors", new Microsoft.Xna.Framework.Rectangle(247, 407, 6, 6), 2000f, 1, 10000, new Vector2(30.5f, 16.8f) * 64f, false, false, 1E-06f, 0.0f, Color.White, 3f + (float) Game1.random.NextDouble(), 0.0f, 0.0f, 0.0f));
    }
    if (Game1.MasterPlayer.mailReceived.Contains("Capsule_Broken") && Game1.isDarkOut((GameLocation) this) && Game1.random.NextDouble() < 0.01)
    {
      this.temporarySprites.Add(new TemporaryAnimatedSprite("LooseSprites\\temporary_sprites_1", new Microsoft.Xna.Framework.Rectangle(448, 546, 16 /*0x10*/, 25), new Vector2(3f, 59f) * 64f, false, 0.0f, Color.White)
      {
        scale = 4f,
        motion = new Vector2(3f, 0.0f),
        animationLength = 4,
        interval = 80f,
        totalNumberOfLoops = 200,
        layerDepth = 0.384f,
        xStopCoordinate = 384
      });
      this.temporarySprites.Add(new TemporaryAnimatedSprite("LooseSprites\\temporary_sprites_1", new Microsoft.Xna.Framework.Rectangle(448, 546, 16 /*0x10*/, 25), new Vector2(58f, 108f) * 64f, false, 0.0f, Color.White)
      {
        scale = 4f,
        motion = new Vector2(3f, 0.0f),
        animationLength = 4,
        interval = 80f,
        totalNumberOfLoops = 200,
        layerDepth = 0.384f,
        xStopCoordinate = 4800
      });
      this.temporarySprites.Add(new TemporaryAnimatedSprite("LooseSprites\\temporary_sprites_1", new Microsoft.Xna.Framework.Rectangle(448, 546, 16 /*0x10*/, 25), new Vector2(20f, 92.5f) * 64f, false, 0.0f, Color.White)
      {
        scale = 4f,
        motion = new Vector2(3f, 0.0f),
        animationLength = 4,
        interval = 80f,
        totalNumberOfLoops = 200,
        layerDepth = 0.384f,
        xStopCoordinate = 1664,
        delayBeforeAnimationStart = 1000
      });
      this.temporarySprites.Add(new TemporaryAnimatedSprite("LooseSprites\\temporary_sprites_1", new Microsoft.Xna.Framework.Rectangle(448, 546, 16 /*0x10*/, 25), new Vector2(75f, 1f) * 64f, true, 0.0f, Color.White)
      {
        scale = 4f,
        motion = new Vector2(-4f, 0.0f),
        animationLength = 4,
        interval = 60f,
        totalNumberOfLoops = 200,
        layerDepth = 0.0064f,
        xStopCoordinate = 4352
      });
    }
    Game1.currentLightSources.Add(new LightSource("Town_Saloon", 4, new Vector2(2803f, 4418f), 1f, onlyLocation: this.NameOrUniqueName));
    if (num1 != 0)
    {
      Game1.currentLightSources.Add(new LightSource("Town_AlexHouse_1", 4, new Vector2(3544f, 4005f), 0.5f, onlyLocation: this.NameOrUniqueName));
      Game1.currentLightSources.Add(new LightSource("Town_AlexHouse_2", 4, new Vector2(3680f, 3832f), 0.5f, onlyLocation: this.NameOrUniqueName));
      Game1.currentLightSources.Add(new LightSource("Town_AlexHouse_3", 4, new Vector2(3877f, 4007f), 0.5f, onlyLocation: this.NameOrUniqueName));
      Game1.currentLightSources.Add(new LightSource("Town_PennyHouse", 4, new Vector2(4836f, 4320f), 1f, onlyLocation: this.NameOrUniqueName));
      Game1.currentLightSources.Add(new LightSource("Town_SeedShop_1", 4, new Vector2(2514f, 3538f), 0.5f, onlyLocation: this.NameOrUniqueName));
      Game1.currentLightSources.Add(new LightSource("Town_Plaza_1", 4, new Vector2(2205f, 4950f), 1f, onlyLocation: this.NameOrUniqueName));
      Game1.currentLightSources.Add(new LightSource("Town_Plaza_2", 4, new Vector2(2205f, 4755f), 0.5f, onlyLocation: this.NameOrUniqueName));
      Game1.currentLightSources.Add(new LightSource("Town_SeedShop_2", 4, new Vector2(2981f, 3497f), 0.5f, onlyLocation: this.NameOrUniqueName));
      Game1.currentLightSources.Add(new LightSource("Town_Hospital_1", 4, new Vector2(2332f, 3192f), 0.5f, onlyLocation: this.NameOrUniqueName));
      Game1.currentLightSources.Add(new LightSource("Town_ManorHouse_1", 4, new Vector2(3675f, 5437f), 0.5f, onlyLocation: this.NameOrUniqueName));
      Game1.currentLightSources.Add(new LightSource("Town_ManorHouse_2", 4, new Vector2(3853f, 5445f), 0.5f, onlyLocation: this.NameOrUniqueName));
      Game1.currentLightSources.Add(new LightSource("Town_HaleyHouse_1", 4, new Vector2(1558f, 5520f), 0.5f, onlyLocation: this.NameOrUniqueName));
      Game1.currentLightSources.Add(new LightSource("Town_HaleyHouse_2", 4, new Vector2(1557f, 5613f), 0.5f, onlyLocation: this.NameOrUniqueName));
      Game1.currentLightSources.Add(new LightSource("Town_HaleyHouse_3", 4, new Vector2(1307f, 5593f), 0.25f, onlyLocation: this.NameOrUniqueName));
      Game1.currentLightSources.Add(new LightSource("Town_SamHouse_1", 4, new Vector2(815f, 5383f), 0.5f, onlyLocation: this.NameOrUniqueName));
      Game1.currentLightSources.Add(new LightSource("Town_SamHouse_2", 4, new Vector2(560f, 5384f), 0.5f, onlyLocation: this.NameOrUniqueName));
      Game1.currentLightSources.Add(new LightSource("Town_SamHouse_3", 4, new Vector2(671f, 5216f), 0.5f, onlyLocation: this.NameOrUniqueName));
      if (this.ccRefurbished && !Game1.MasterPlayer.mailReceived.Contains("JojaMember"))
      {
        Game1.currentLightSources.Add(new LightSource("Town_JojaMart_1", 4, new Vector2(3153f, 1171f), 0.5f, onlyLocation: this.NameOrUniqueName));
        Game1.currentLightSources.Add(new LightSource("Town_JojaMart_2", 4, new Vector2(3630f, 1170f), 0.5f, onlyLocation: this.NameOrUniqueName));
        Game1.currentLightSources.Add(new LightSource("Town_JojaMart_3", 4, new Vector2(3389f, 1053f), 0.5f, onlyLocation: this.NameOrUniqueName));
      }
      Action<int, int> action = (Action<int, int>) ((x, y) =>
      {
        this.temporarySprites.Add(new TemporaryAnimatedSprite("LooseSprites\\Cursors_1_6", new Microsoft.Xna.Framework.Rectangle(256 /*0x0100*/, 432, 64 /*0x40*/, 80 /*0x50*/), new Vector2((float) x, (float) y) * 64f, false, 0.0f, Color.White)
        {
          animationLength = 1,
          interval = (float) int.MaxValue,
          drawAboveAlwaysFront = true,
          scale = 4f
        });
        Game1.currentLightSources.Add(new LightSource($"Town_WinterTree_{x}_{y}", 9, new Vector2((float) x, (float) y) * 64f + new Vector2(128f, 160f), 1f, Color.Black * 0.66f, onlyLocation: this.NameOrUniqueName));
      });
      action(32 /*0x20*/, 83);
      action(42, 96 /*0x60*/);
      action(50, 88);
      action(16 /*0x10*/, 66);
      action(29, 49);
      action(63 /*0x3F*/, 57);
      action(56, 46);
      action(5, 58);
      action(65, 10);
    }
    if (Utility.getDaysOfBooksellerThisSeason().Contains(Game1.dayOfMonth))
    {
      this.showBookseller = true;
      Game1.currentLightSources.Add(new LightSource("Town_Bookseller_1", 4, new Vector2(7202f, 1634f), 0.5f, onlyLocation: this.NameOrUniqueName));
      Game1.currentLightSources.Add(new LightSource("Town_Bookseller_2", 4, new Vector2(7032f, 1634f), 0.5f, onlyLocation: this.NameOrUniqueName));
    }
    this.addOneTimeGiftBox(ItemRegistry.Create("(O)Book_Trash"), 123, 58, 3);
    this.addOneTimeGiftBox(ItemRegistry.Create("(O)PrizeTicket"), 114, 17);
  }

  private void showTownCommunityUpgradeShortcuts()
  {
    this.removeTile(90, 2, "Buildings");
    this.removeTile(90, 1, "Front");
    this.removeTile(90, 1, "Buildings");
    this.removeTile(90, 0, "Buildings");
    this.setMapTile(89, 1, 360, "Front", "Landscape");
    this.setMapTile(89, 2, 385, "Buildings", "Landscape");
    this.setMapTile(89, 1, 436, "Buildings", "Landscape");
    this.setMapTile(89, 0, 411, "Buildings", "Landscape");
    this.removeTile(98, 4, "Buildings");
    this.removeTile(98, 3, "Buildings");
    this.removeTile(98, 2, "Buildings");
    this.removeTile(98, 1, "Buildings");
    this.removeTile(98, 0, "Buildings");
    this.setMapTile(98, 4, 12, "Back", "v16_landscape2");
    this.setMapTile(98, 3, 509, "Back", "Landscape");
    this.setMapTile(98, 2, 217, "Back", "Landscape");
    this.setMapTile(97, 3, 1683, "Buildings", "Landscape");
    this.setMapTile(97, 3, 509, "Back", "Landscape");
    this.setMapTile(97, 2, 1658, "Buildings", "Landscape");
    this.setMapTile(97, 2, 217, "Back", "Landscape");
    this.setMapTile(98, 2, 1659, "AlwaysFront", "Landscape");
    this.removeTile(92, 104, "Buildings");
    this.removeTile(93, 104, "Buildings");
    this.removeTile(94, 104, "Buildings");
    this.removeTile(92, 105, "Buildings");
    this.removeTile(93, 105, "Buildings");
    this.removeTile(94, 105, "Buildings");
    this.removeTile(93, 106, "Buildings");
    this.removeTile(94, 106, "Buildings");
    this.removeTile(92, 103, "Front");
    this.removeTile(93, 103, "Front");
    this.removeTile(94, 103, "Front");
  }

  public override void cleanupBeforePlayerExit()
  {
    base.cleanupBeforePlayerExit();
    this.minecartSteam = (TemporaryAnimatedSprite) null;
    if (this.mapLoader == null)
      return;
    this.mapLoader.Dispose();
    this.mapLoader = (LocalizedContentManager) null;
  }

  public void initiateMarnieLewisBush()
  {
    Game1.player.freezePause = 3000;
    this.temporarySprites.Add(new TemporaryAnimatedSprite("Characters\\Marnie", new Microsoft.Xna.Framework.Rectangle(0, 0, 16 /*0x10*/, 32 /*0x20*/), new Vector2(48f, 98f) * 64f, false, 0.0f, Color.White)
    {
      scale = 4f,
      animationLength = 4,
      interval = 200f,
      totalNumberOfLoops = 99999,
      motion = new Vector2(-3f, -12f),
      acceleration = new Vector2(0.0f, 0.4f),
      xStopCoordinate = 2880,
      yStopCoordinate = 6336,
      layerDepth = 0.64f,
      reachedStopCoordinate = new TemporaryAnimatedSprite.endBehavior(this.marnie_landed),
      id = 888
    });
    this.temporarySprites.Add(new TemporaryAnimatedSprite("Characters\\Lewis", new Microsoft.Xna.Framework.Rectangle(0, 0, 16 /*0x10*/, 32 /*0x20*/), new Vector2(48f, 98f) * 64f, false, 0.0f, Color.White)
    {
      scale = 4f,
      animationLength = 4,
      interval = 200f,
      totalNumberOfLoops = 99999,
      motion = new Vector2(3f, -12f),
      acceleration = new Vector2(0.0f, 0.4f),
      xStopCoordinate = 3264,
      yStopCoordinate = 6336,
      layerDepth = 0.64f,
      id = 777
    });
    Game1.playSound("dwop");
  }

  private void marnie_landed(int extra)
  {
    Game1.player.freezePause = 2000;
    TemporaryAnimatedSprite temporarySpriteById1 = this.getTemporarySpriteByID(777);
    if (temporarySpriteById1 != null)
      this.temporarySprites.Add(new TemporaryAnimatedSprite("Characters\\Lewis", new Microsoft.Xna.Framework.Rectangle(0, 32 /*0x20*/, 16 /*0x10*/, 32 /*0x20*/), temporarySpriteById1.position, false, 0.0f, Color.White)
      {
        scale = 4f,
        animationLength = 4,
        interval = 60f,
        totalNumberOfLoops = 50,
        layerDepth = 0.64f,
        id = 0,
        motion = new Vector2(8f, 0.0f)
      });
    TemporaryAnimatedSprite temporarySpriteById2 = this.getTemporarySpriteByID(888);
    if (temporarySpriteById2 != null)
      this.temporarySprites.Add(new TemporaryAnimatedSprite("Characters\\Marnie", new Microsoft.Xna.Framework.Rectangle(0, 32 /*0x20*/, 16 /*0x10*/, 32 /*0x20*/), temporarySpriteById2.position, true, 0.0f, Color.White)
      {
        scale = 4f,
        animationLength = 4,
        interval = 60f,
        totalNumberOfLoops = 50,
        layerDepth = 0.64f,
        id = 1,
        motion = new Vector2(-8f, 0.0f)
      });
    this.removeTemporarySpritesWithID(777);
    this.removeTemporarySpritesWithID(888);
    for (int index = 0; index < 3200; index += 200)
      DelayedAction.playSoundAfterDelay("grassyStep", 100 + index);
  }

  public void initiateMagnifyingGlassGet()
  {
    Game1.player.freezePause = 3000;
    if (Game1.player.TilePoint.X >= 31 /*0x1F*/)
      this.temporarySprites.Add(new TemporaryAnimatedSprite("Characters\\Krobus", new Microsoft.Xna.Framework.Rectangle(0, 0, 16 /*0x10*/, 24), new Vector2(29f, 13f) * 64f, false, 0.0f, Color.White)
      {
        scale = 4f,
        animationLength = 4,
        interval = 200f,
        totalNumberOfLoops = 99999,
        motion = new Vector2(3f, -12f),
        acceleration = new Vector2(0.0f, 0.4f),
        xStopCoordinate = 2048 /*0x0800*/,
        yStopCoordinate = 960,
        layerDepth = 1f,
        reachedStopCoordinate = new TemporaryAnimatedSprite.endBehavior(this.mgThief_landed),
        id = 777
      });
    else
      this.temporarySprites.Add(new TemporaryAnimatedSprite("Characters\\Krobus", new Microsoft.Xna.Framework.Rectangle(0, 0, 16 /*0x10*/, 24), new Vector2(29f, 13f) * 64f, false, 0.0f, Color.White)
      {
        scale = 4f,
        animationLength = 4,
        interval = 200f,
        totalNumberOfLoops = 99999,
        motion = new Vector2(2f, -12f),
        acceleration = new Vector2(0.0f, 0.4f),
        xStopCoordinate = 1984,
        yStopCoordinate = 832,
        layerDepth = 0.0896f,
        reachedStopCoordinate = new TemporaryAnimatedSprite.endBehavior(this.mgThief_landed),
        id = 777
      });
    Game1.playSound("dwop");
  }

  private void mgThief_landed(int extra)
  {
    TemporaryAnimatedSprite temporarySpriteById = this.getTemporarySpriteByID(777);
    if (temporarySpriteById == null)
      return;
    temporarySpriteById.animationLength = 1;
    temporarySpriteById.shakeIntensity = 1f;
    temporarySpriteById.interval = 1500f;
    temporarySpriteById.timer = 0.0f;
    temporarySpriteById.totalNumberOfLoops = 1;
    temporarySpriteById.currentNumberOfLoops = 0;
    temporarySpriteById.endFunction = new TemporaryAnimatedSprite.endBehavior(this.mgThief_speech);
    Game1.playSound("snowyStep");
  }

  private void mgThief_speech(int extra)
  {
    Game1.drawObjectDialogue(Game1.content.LoadString("Strings\\Locations:Town_mgThiefMessage"));
    Game1.afterDialogues = new Game1.afterFadeFunction(this.mgThief_afterSpeech);
    TemporaryAnimatedSprite temporarySpriteById = this.getTemporarySpriteByID(777);
    if (temporarySpriteById == null)
      return;
    temporarySpriteById.animationLength = 4;
    temporarySpriteById.shakeIntensity = 0.0f;
    temporarySpriteById.interval = 200f;
    temporarySpriteById.timer = 0.0f;
    temporarySpriteById.totalNumberOfLoops = 9999;
    temporarySpriteById.currentNumberOfLoops = 0;
    this.temporarySprites.Add(new TemporaryAnimatedSprite("Characters\\Krobus", new Microsoft.Xna.Framework.Rectangle(0, 0, 16 /*0x10*/, 24), temporarySpriteById.position, false, 0.0f, Color.White)
    {
      scale = 4f,
      animationLength = 4,
      interval = 200f,
      totalNumberOfLoops = 99999,
      layerDepth = 0.0896f,
      id = 777
    });
  }

  private void mgThief_afterSpeech()
  {
    Game1.player.holdUpItemThenMessage((Item) new SpecialItem(5));
    Game1.afterDialogues = new Game1.afterFadeFunction(this.mgThief_afterGlass);
    Game1.player.hasMagnifyingGlass = true;
    Game1.player.removeQuest("31");
  }

  private void mgThief_afterGlass()
  {
    Game1.player.freezePause = 1500;
    TemporaryAnimatedSprite temporarySpriteById = this.getTemporarySpriteByID(777);
    if (temporarySpriteById == null)
      return;
    temporarySpriteById.animationLength = 1;
    temporarySpriteById.shakeIntensity = 1f;
    temporarySpriteById.interval = 500f;
    temporarySpriteById.timer = 0.0f;
    temporarySpriteById.totalNumberOfLoops = 1;
    temporarySpriteById.currentNumberOfLoops = 0;
    temporarySpriteById.endFunction = new TemporaryAnimatedSprite.endBehavior(this.mg_disappear);
  }

  private void mg_disappear(int extra)
  {
    Game1.player.freezePause = 1000;
    TemporaryAnimatedSprite temporarySpriteById = this.getTemporarySpriteByID(777);
    if (temporarySpriteById == null)
      return;
    this.temporarySprites.Add(new TemporaryAnimatedSprite("Characters\\Krobus", new Microsoft.Xna.Framework.Rectangle(0, 0, 16 /*0x10*/, 24), temporarySpriteById.position, false, 0.0f, Color.White)
    {
      scale = 4f,
      animationLength = 4,
      interval = 60f,
      totalNumberOfLoops = 50,
      layerDepth = 0.0896f,
      id = 777,
      motion = new Vector2(0.0f, 8f)
    });
    for (int index = 0; index < 3200; index += 200)
      DelayedAction.playSoundAfterDelay("snowyStep", 100 + index);
  }

  public override void UpdateWhenCurrentLocation(GameTime time)
  {
    base.UpdateWhenCurrentLocation(time);
    this.minecartSteam?.update(time);
  }

  public override void draw(SpriteBatch spriteBatch)
  {
    base.draw(spriteBatch);
    this.minecartSteam?.draw(spriteBatch);
    if (this.ccJoja && !this._appliedMapOverrides.Contains("Town-TheaterCC") && !this._appliedMapOverrides.Contains("Town-TheaterCC-Halloween2"))
      spriteBatch.Draw(Game1.mouseCursors, Game1.GlobalToLocal(Game1.viewport, this.ccFacadePositionBottom), new Microsoft.Xna.Framework.Rectangle?(Town.jojaFacadeBottom), Color.White, 0.0f, Vector2.Zero, 4f, SpriteEffects.None, 0.128f);
    if (!this.playerCheckedBoard)
    {
      float num = (float) (4.0 * Math.Round(Math.Sin(Game1.currentGameTime.TotalGameTime.TotalMilliseconds / 250.0), 2));
      spriteBatch.Draw(Game1.mouseCursors, Game1.GlobalToLocal(Game1.viewport, new Vector2(2616f, 3472f + num)), new Microsoft.Xna.Framework.Rectangle?(new Microsoft.Xna.Framework.Rectangle(141, 465, 20, 24)), Color.White * 0.75f, 0.0f, Vector2.Zero, 4f, SpriteEffects.None, 0.98f);
      spriteBatch.Draw(Game1.mouseCursors, Game1.GlobalToLocal(Game1.viewport, new Vector2(2656f, 3512f + num)), new Microsoft.Xna.Framework.Rectangle?(new Microsoft.Xna.Framework.Rectangle(175, 425, 12, 12)), Color.White * 0.75f, 0.0f, new Vector2(6f, 6f), 4f, SpriteEffects.None, 1f);
    }
    if (Game1.CanAcceptDailyQuest())
    {
      float num = (float) (4.0 * Math.Round(Math.Sin(Game1.currentGameTime.TotalGameTime.TotalMilliseconds / 250.0), 2));
      spriteBatch.Draw(Game1.mouseCursors, Game1.GlobalToLocal(Game1.viewport, new Vector2(2692f, 3528f + num)), new Microsoft.Xna.Framework.Rectangle?(new Microsoft.Xna.Framework.Rectangle(395, 497, 3, 8)), Color.White, 0.0f, new Vector2(1f, 4f), 4f + Math.Max(0.0f, (float) (0.25 - (double) num / 16.0)), SpriteEffects.None, 1f);
    }
    if (SpecialOrder.IsSpecialOrdersBoardUnlocked() && !Game1.player.team.acceptedSpecialOrderTypes.Contains("") && !Game1.eventUp)
    {
      float num = (float) (4.0 * Math.Round(Math.Sin(Game1.currentGameTime.TotalGameTime.TotalMilliseconds / 250.0), 2));
      spriteBatch.Draw(Game1.mouseCursors, Game1.GlobalToLocal(Game1.viewport, new Vector2(3997.6f, 5908.8f + num)), new Microsoft.Xna.Framework.Rectangle?(new Microsoft.Xna.Framework.Rectangle(395, 497, 3, 8)), Color.White, 0.0f, new Vector2(1f, 4f), 4f + Math.Max(0.0f, (float) (0.25 - (double) num / 8.0)), SpriteEffects.None, 1f);
    }
    if (Game1.player.stats.Get("specialOrderPrizeTickets") > 0U && !Game1.isFestival())
    {
      float num = (float) (4.0 * Math.Round(Math.Sin(Game1.currentGameTime.TotalGameTime.TotalMilliseconds / 250.0), 2));
      spriteBatch.Draw(Game1.mouseCursors, Game1.GlobalToLocal(Game1.viewport, new Vector2(3832f, 5840f + num)), new Microsoft.Xna.Framework.Rectangle?(new Microsoft.Xna.Framework.Rectangle(141, 465, 20, 24)), Color.White * 0.75f, 0.0f, Vector2.Zero, 4f, SpriteEffects.None, 0.98f);
      spriteBatch.Draw(Game1.mouseCursors_1_6, Game1.GlobalToLocal(Game1.viewport, new Vector2(3872f, 5880f + num)), new Microsoft.Xna.Framework.Rectangle?(new Microsoft.Xna.Framework.Rectangle(240 /*0xF0*/, 240 /*0xF0*/, 16 /*0x10*/, 16 /*0x10*/)), Color.White * 0.75f, 0.0f, new Vector2(8f, 8f), 4f, SpriteEffects.None, 1f);
    }
    if (!this.showBookseller)
      return;
    spriteBatch.Draw(Game1.mouseCursors_1_6, Game1.GlobalToLocal(new Vector2(52f, 50f) * 64f + new Vector2(6f, 1f) * 4f), new Microsoft.Xna.Framework.Rectangle?(new Microsoft.Xna.Framework.Rectangle(258, 335, 26, 29)), Color.White, 0.0f, Vector2.Zero, 4f, SpriteEffects.None, 0.32f);
    spriteBatch.Draw(Game1.mouseCursors_1_6, Game1.GlobalToLocal(new Vector2(106f, 22f) * 64f + new Vector2(1f, 1f) * 4f), new Microsoft.Xna.Framework.Rectangle?(new Microsoft.Xna.Framework.Rectangle(0, 433, 110, 79)), Color.White, 0.0f, Vector2.Zero, 4f, SpriteEffects.None, 0.1728f);
    spriteBatch.Draw(Game1.mouseCursors, Game1.GlobalToLocal(new Vector2(1832f, 425f) * 4f), new Microsoft.Xna.Framework.Rectangle?(new Microsoft.Xna.Framework.Rectangle(0, 1183, 84, 160 /*0xA0*/)), Color.White, 0.0f, new Vector2(42f, 160f), 4f, SpriteEffects.None, 0.1728f);
    spriteBatch.Draw(Game1.mouseCursors_1_6, Game1.GlobalToLocal(new Vector2(106f, 22f) * 64f + new Vector2(90f, 14f) * 4f), new Microsoft.Xna.Framework.Rectangle?(new Microsoft.Xna.Framework.Rectangle(89, 446, 44, 7)), Color.White, 0.0f, Vector2.Zero, 4f, SpriteEffects.None, 0.17216f);
    if (Game1.currentGameTime.TotalGameTime.TotalMilliseconds % 7000.0 < 200.0)
      spriteBatch.Draw(Game1.mouseCursors_1_6, Game1.GlobalToLocal(new Vector2(106f, 22f) * 64f + new Vector2(54f, 41f) * 4f), new Microsoft.Xna.Framework.Rectangle?(new Microsoft.Xna.Framework.Rectangle(110, 488, 17, 24)), Color.White, 0.0f, Vector2.Zero, 4f, SpriteEffects.None, 0.173440009f);
    if (Game1.currentGameTime.TotalGameTime.TotalMilliseconds % 15000.0 < 1200.0)
      spriteBatch.Draw(Game1.mouseCursors_1_6, Game1.GlobalToLocal(new Vector2(106f, 22f) * 64f + new Vector2(54f, 61f) * 4f), new Microsoft.Xna.Framework.Rectangle?(new Microsoft.Xna.Framework.Rectangle((int) sbyte.MaxValue + (int) Game1.currentGameTime.TotalGameTime.TotalMilliseconds % 400 / 100 * 17, 508, 17, 4)), Color.White, 0.0f, Vector2.Zero, 4f, SpriteEffects.None, 0.17408f);
    spriteBatch.Draw(Game1.mouseCursors_1_6, Game1.GlobalToLocal(new Vector2(106f, 22f) * 64f + new Vector2(107f, 21f) * 4f), new Microsoft.Xna.Framework.Rectangle?(new Microsoft.Xna.Framework.Rectangle(110 + (int) (Game1.currentGameTime.TotalGameTime.TotalMilliseconds % 1000.0) / 100 * 10, 474, 10, 7)), Color.White, 1.57079637f, Vector2.Zero, 4f, SpriteEffects.None, 0.1728f);
    spriteBatch.Draw(Game1.mouseCursors_1_6, Game1.GlobalToLocal(new Vector2(106f, 22f) * 64f + new Vector2(115f, 21f) * 4f), new Microsoft.Xna.Framework.Rectangle?(new Microsoft.Xna.Framework.Rectangle(110 + (int) ((Game1.currentGameTime.TotalGameTime.TotalMilliseconds + 400.0) % 1000.0) / 100 * 10, 467, 10, 7)), Color.White, 1.57079637f, Vector2.Zero, 4f, SpriteEffects.None, 0.1728f);
    spriteBatch.Draw(Game1.mouseCursors_1_6, Game1.GlobalToLocal(new Vector2(106f, 22f) * 64f + new Vector2(123f, 21f) * 4f), new Microsoft.Xna.Framework.Rectangle?(new Microsoft.Xna.Framework.Rectangle(110 + (int) ((Game1.currentGameTime.TotalGameTime.TotalMilliseconds + 200.0) % 1000.0) / 100 * 10, 481, 10, 7)), Color.White, 1.57079637f, Vector2.Zero, 4f, SpriteEffects.None, 0.1728f);
  }

  public override void drawAboveAlwaysFrontLayer(SpriteBatch b)
  {
    if (this.ccJoja)
    {
      if (!this._appliedMapOverrides.Contains("Town-TheaterCC") && !this._appliedMapOverrides.Contains("Town-TheaterCC-Halloween2"))
      {
        b.Draw(Game1.mouseCursors, Game1.GlobalToLocal(Game1.viewport, this.ccFacadePosition), new Microsoft.Xna.Framework.Rectangle?(Town.jojaFacadeTop), Color.White, 0.0f, Vector2.Zero, 4f, SpriteEffects.None, 0.128f);
        if (this.IsWinterHere())
          b.Draw(Game1.mouseCursors, Game1.GlobalToLocal(Game1.viewport, this.ccFacadePosition), new Microsoft.Xna.Framework.Rectangle?(Town.jojaFacadeWinterOverlay), Color.White, 0.0f, Vector2.Zero, 4f, SpriteEffects.None, 0.1281f);
      }
    }
    else if (this.ccRefurbished)
    {
      b.Draw(Game1.mouseCursors, Game1.GlobalToLocal(Game1.viewport, this.clockCenter), new Microsoft.Xna.Framework.Rectangle?(Town.hourHandSource), Color.White, (float) (2.0 * Math.PI * ((double) (Game1.timeOfDay % 1200) / 1200.0) + (double) Game1.gameTimeInterval / (double) Game1.realMilliSecondsPerGameTenMinutes / 23.0), new Vector2(2.5f, 8f), 4f, SpriteEffects.None, 0.98f);
      b.Draw(Game1.mouseCursors, Game1.GlobalToLocal(Game1.viewport, this.clockCenter), new Microsoft.Xna.Framework.Rectangle?(Town.minuteHandSource), Color.White, (float) (2.0 * Math.PI * ((double) (Game1.timeOfDay % 1000 % 100 % 60) / 60.0) + (double) Game1.gameTimeInterval / (double) Game1.realMilliSecondsPerGameTenMinutes * 1.0199999809265137), new Vector2(2.5f, 12f), 4f, SpriteEffects.None, 0.99f);
      b.Draw(Game1.mouseCursors, Game1.GlobalToLocal(Game1.viewport, this.clockCenter), new Microsoft.Xna.Framework.Rectangle?(Town.clockNub), Color.White, 0.0f, new Vector2(2f, 2f), 4f, SpriteEffects.None, 1f);
    }
    base.drawAboveAlwaysFrontLayer(b);
  }
}
