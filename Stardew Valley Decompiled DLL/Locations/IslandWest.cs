// Decompiled with JetBrains decompiler
// Type: StardewValley.Locations.IslandWest
// Assembly: Stardew Valley, Version=1.6.15.24356, Culture=neutral, PublicKeyToken=null
// MVID: AA1F513A-94F0-4EF6-A35F-2D5B4A3721BF
// Assembly location: D:\SteamLibrary\steamapps\common\Stardew Valley\Stardew Valley.dll
// XML documentation location: D:\SteamLibrary\steamapps\common\Stardew Valley\Stardew Valley.xml

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Netcode;
using StardewValley.BellsAndWhistles;
using StardewValley.Extensions;
using StardewValley.ItemTypeDefinitions;
using StardewValley.Menus;
using StardewValley.Monsters;
using StardewValley.Objects;
using StardewValley.TerrainFeatures;
using System;
using System.Collections.Generic;
using System.Xml.Serialization;
using xTile.Dimensions;

#nullable disable
namespace StardewValley.Locations;

public class IslandWest : IslandLocation
{
  [XmlElement("addedSlimesToday")]
  private readonly NetBool addedSlimesToday;
  [XmlElement("sandDuggy")]
  public NetRef<SandDuggy> sandDuggy;
  [XmlElement("farmhouseRestored")]
  public readonly NetBool farmhouseRestored;
  [XmlElement("farmhouseMailbox")]
  public readonly NetBool farmhouseMailbox;
  [XmlElement("farmObelisk")]
  public readonly NetBool farmObelisk;
  public Point shippingBinPosition;
  private TemporaryAnimatedSprite shippingBinLid;
  private Microsoft.Xna.Framework.Rectangle shippingBinLidOpenArea;

  public override void SetBuriedNutLocations()
  {
    this.buriedNutPoints.Add(new Point(21, 81));
    this.buriedNutPoints.Add(new Point(62, 76));
    this.buriedNutPoints.Add(new Point(39, 24));
    this.buriedNutPoints.Add(new Point(88, 14));
    this.buriedNutPoints.Add(new Point(43, 74));
    this.buriedNutPoints.Add(new Point(30, 75));
    base.SetBuriedNutLocations();
  }

  /// <inheritdoc />
  public override bool CanPlantSeedsHere(
    string itemId,
    int tileX,
    int tileY,
    bool isGardenPot,
    out string deniedMessage)
  {
    if (!(this.getTileSheetIDAt(tileX, tileY, "Back") != "untitled tile sheet2"))
      return base.CanPlantSeedsHere(itemId, tileX, tileY, isGardenPot, out deniedMessage);
    deniedMessage = (string) null;
    return false;
  }

  /// <inheritdoc />
  public override bool CanPlantTreesHere(
    string itemId,
    int tileX,
    int tileY,
    out string deniedMessage)
  {
    if (this.getTileSheetIDAt(tileX, tileY, "Back") == "untitled tile sheet2" || StardewValley.Object.isWildTreeSeed(itemId))
    {
      string str = this.doesTileHavePropertyNoNull(tileX, tileY, "Type", "Back");
      if (str == "Dirt" || str == "Grass" || str == "")
        return this.CheckItemPlantRules(itemId, false, true, out deniedMessage);
    }
    return base.CanPlantTreesHere(itemId, tileX, tileY, out deniedMessage);
  }

  public IslandWest()
  {
    NetBool netBool1 = new NetBool();
    netBool1.InterpolationWait = false;
    this.farmhouseRestored = netBool1;
    NetBool netBool2 = new NetBool();
    netBool2.InterpolationWait = false;
    this.farmhouseMailbox = netBool2;
    NetBool netBool3 = new NetBool();
    netBool3.InterpolationWait = false;
    this.farmObelisk = netBool3;
    this.shippingBinPosition = new Point(90, 39);
    // ISSUE: explicit constructor call
    base.\u002Ector();
  }

  public override bool performToolAction(Tool t, int tileX, int tileY)
  {
    this.sandDuggy.Value?.PerformToolAction(t, tileX, tileY);
    return base.performToolAction(t, tileX, tileY);
  }

  public override List<Vector2> GetAdditionalWalnutBushes()
  {
    return new List<Vector2>()
    {
      new Vector2(54f, 18f),
      new Vector2(25f, 30f),
      new Vector2(15f, 3f)
    };
  }

  public override void draw(SpriteBatch b)
  {
    this.sandDuggy.Value?.Draw(b);
    if (this.farmhouseRestored.Value)
      this.shippingBinLid?.draw(b);
    if (this.farmhouseMailbox.Value && Game1.mailbox.Count > 0)
    {
      float num1 = (float) (4.0 * Math.Round(Math.Sin(Game1.currentGameTime.TotalGameTime.TotalMilliseconds / 250.0), 2));
      Point point = new Point(81, 40);
      float num2 = (float) ((double) ((point.X + 1) * 64 /*0x40*/) / 10000.0 + (double) (point.Y * 64 /*0x40*/) / 10000.0);
      float num3 = -8f;
      b.Draw(Game1.mouseCursors, Game1.GlobalToLocal(Game1.viewport, new Vector2((float) (point.X * 64 /*0x40*/) + num3, (float) (point.Y * 64 /*0x40*/ - 96 /*0x60*/ - 48 /*0x30*/) + num1)), new Microsoft.Xna.Framework.Rectangle?(new Microsoft.Xna.Framework.Rectangle(141, 465, 20, 24)), Color.White * 0.75f, 0.0f, Vector2.Zero, 4f, SpriteEffects.None, num2 + 1E-06f);
      b.Draw(Game1.mouseCursors, Game1.GlobalToLocal(Game1.viewport, new Vector2((float) (point.X * 64 /*0x40*/ + 32 /*0x20*/ + 4) + num3, (float) (point.Y * 64 /*0x40*/ - 64 /*0x40*/ - 24 - 8) + num1)), new Microsoft.Xna.Framework.Rectangle?(new Microsoft.Xna.Framework.Rectangle(189, 423, 15, 13)), Color.White, 0.0f, new Vector2(7f, 6f), 4f, SpriteEffects.None, num2 + 1E-05f);
    }
    base.draw(b);
  }

  public override void UpdateWhenCurrentLocation(GameTime time)
  {
    this.sandDuggy.Value?.Update(time);
    if (this.farmhouseRestored.Value && this.shippingBinLid != null)
    {
      bool flag = false;
      foreach (Character farmer in this.farmers)
      {
        if (farmer.GetBoundingBox().Intersects(this.shippingBinLidOpenArea))
        {
          this.openShippingBinLid();
          flag = true;
        }
      }
      if (!flag)
        this.closeShippingBinLid();
      this.updateShippingBinLid(time);
    }
    base.UpdateWhenCurrentLocation(time);
  }

  public IslandWest(string map, string name)
  {
    NetBool netBool1 = new NetBool();
    netBool1.InterpolationWait = false;
    this.farmhouseRestored = netBool1;
    NetBool netBool2 = new NetBool();
    netBool2.InterpolationWait = false;
    this.farmhouseMailbox = netBool2;
    NetBool netBool3 = new NetBool();
    netBool3.InterpolationWait = false;
    this.farmObelisk = netBool3;
    this.shippingBinPosition = new Point(90, 39);
    // ISSUE: explicit constructor call
    base.\u002Ector(map, name);
    this.sandDuggy.Value = new SandDuggy((GameLocation) this, new Point[4]
    {
      new Point(37, 87),
      new Point(41, 86),
      new Point(45, 86),
      new Point(48 /*0x30*/, 87)
    });
    this.parrotUpgradePerches.Add(new ParrotUpgradePerch((GameLocation) this, new Point(72, 37), new Microsoft.Xna.Framework.Rectangle(71, 29, 3, 8), 20, (Action) (() =>
    {
      Game1.createItemDebris(ItemRegistry.Create("(O)886"), new Vector2(72f, 37f) * 64f + new Vector2(32f), 2);
      Game1.addMailForTomorrow("Island_W_Obelisk", true, true);
      this.farmObelisk.Value = true;
    }), (Func<bool>) (() => this.farmObelisk.Value), "Obelisk", "Island_UpgradeHouse_Mailbox"));
    this.parrotUpgradePerches.Add(new ParrotUpgradePerch((GameLocation) this, new Point(81, 40), new Microsoft.Xna.Framework.Rectangle(80 /*0x50*/, 39, 3, 2), 5, (Action) (() =>
    {
      Game1.addMailForTomorrow("Island_UpgradeHouse_Mailbox", true, true);
      this.farmhouseMailbox.Value = true;
    }), (Func<bool>) (() => this.farmhouseMailbox.Value), "House_Mailbox", "Island_UpgradeHouse"));
    this.parrotUpgradePerches.Add(new ParrotUpgradePerch((GameLocation) this, new Point(81, 40), new Microsoft.Xna.Framework.Rectangle(74, 36, 7, 4), 20, (Action) (() =>
    {
      Game1.addMailForTomorrow("Island_UpgradeHouse", true, true);
      this.farmhouseRestored.Value = true;
    }), (Func<bool>) (() => this.farmhouseRestored.Value), "House"));
    this.parrotUpgradePerches.Add(new ParrotUpgradePerch((GameLocation) this, new Point(72, 10), new Microsoft.Xna.Framework.Rectangle(73, 5, 3, 5), 10, (Action) (() =>
    {
      Game1.addMailForTomorrow("Island_UpgradeParrotPlatform", true, true);
      Game1.netWorldState.Value.ParrotPlatformsUnlocked = true;
    }), (Func<bool>) (() => Game1.netWorldState.Value.ParrotPlatformsUnlocked), "ParrotPlatforms"));
  }

  /// <inheritdoc />
  public override bool performAction(string[] action, Farmer who, Location tileLocation)
  {
    if (!(ArgUtility.Get(action, 0) == "FarmObelisk"))
      return base.performAction(action, who, tileLocation);
    for (int index = 0; index < 12; ++index)
      who.currentLocation.temporarySprites.Add(new TemporaryAnimatedSprite(354, (float) Game1.random.Next(25, 75), 6, 1, new Vector2((float) Game1.random.Next((int) who.Position.X - 256 /*0x0100*/, (int) who.Position.X + 192 /*0xC0*/), (float) Game1.random.Next((int) who.Position.Y - 256 /*0x0100*/, (int) who.Position.Y + 192 /*0xC0*/)), false, Game1.random.NextBool()));
    who.currentLocation.playSound("wand");
    Game1.displayFarmer = false;
    Game1.player.temporarilyInvincible = true;
    Game1.player.temporaryInvincibilityTimer = -2000;
    Game1.player.freezePause = 1000;
    Game1.flashAlpha = 1f;
    DelayedAction.fadeAfterDelay((Game1.afterFadeFunction) (() =>
    {
      Point parsed;
      if (!Game1.getFarm().TryGetMapPropertyAs("WarpTotemEntry", out parsed))
      {
        switch (Game1.whichFarm)
        {
          case 5:
            parsed = new Point(48 /*0x30*/, 39);
            break;
          case 6:
            parsed = new Point(82, 29);
            break;
          default:
            parsed = new Point(48 /*0x30*/, 7);
            break;
        }
      }
      Game1.warpFarmer("Farm", parsed.X, parsed.Y, false);
      Game1.fadeToBlackAlpha = 0.99f;
      Game1.screenGlow = false;
      Game1.player.temporarilyInvincible = false;
      Game1.player.temporaryInvincibilityTimer = 0;
      Game1.displayFarmer = true;
    }), 1000);
    Microsoft.Xna.Framework.Rectangle boundingBox = who.GetBoundingBox();
    new Microsoft.Xna.Framework.Rectangle(boundingBox.X, boundingBox.Y, 64 /*0x40*/, 64 /*0x40*/).Inflate(192 /*0xC0*/, 192 /*0xC0*/);
    int num = 0;
    Point tilePoint = who.TilePoint;
    for (int x = tilePoint.X + 8; x >= tilePoint.X - 8; --x)
    {
      who.currentLocation.temporarySprites.Add(new TemporaryAnimatedSprite(6, new Vector2((float) x, (float) tilePoint.Y) * 64f, Color.White, animationInterval: 50f)
      {
        layerDepth = 1f,
        delayBeforeAnimationStart = num * 25,
        motion = new Vector2(-0.25f, 0.0f)
      });
      ++num;
    }
    return true;
  }

  public override bool leftClick(int x, int y, Farmer who)
  {
    if (this.farmhouseRestored.Value)
    {
      Item activeItem = who.ActiveItem;
      bool? nullable = activeItem?.canBeShipped();
      if (nullable.HasValue && nullable.GetValueOrDefault() && x / 64 /*0x40*/ >= this.shippingBinPosition.X && x / 64 /*0x40*/ <= this.shippingBinPosition.X + 1 && y / 64 /*0x40*/ >= this.shippingBinPosition.Y - 1 && y / 64 /*0x40*/ <= this.shippingBinPosition.Y && (double) Vector2.Distance(who.Tile, new Vector2((float) this.shippingBinPosition.X + 0.5f, (float) this.shippingBinPosition.Y)) <= 2.0)
      {
        Farm farm = Game1.getFarm();
        farm.getShippingBin(who).Add(activeItem);
        farm.lastItemShipped = activeItem;
        who.showNotCarrying();
        this.showShipment(activeItem);
        who.ActiveItem = (Item) null;
        return true;
      }
    }
    return base.leftClick(x, y, who);
  }

  public void showShipment(Item item, bool playThrowSound = true)
  {
    if (playThrowSound)
      this.localSound("backpackIN");
    DelayedAction.playSoundAfterDelay("Ship", playThrowSound ? 250 : 0);
    int num = Game1.random.Next();
    this.temporarySprites.Add(new TemporaryAnimatedSprite("LooseSprites\\Cursors", new Microsoft.Xna.Framework.Rectangle(524, 218, 34, 22), new Vector2(90f, 38f) * 64f + new Vector2(0.0f, 5f) * 4f, false, 0.0f, Color.White)
    {
      interval = 100f,
      totalNumberOfLoops = 1,
      animationLength = 3,
      pingPong = true,
      scale = 4f,
      layerDepth = 0.256010026f,
      id = num,
      extraInfoForEndBehavior = num,
      endFunction = new TemporaryAnimatedSprite.endBehavior(((GameLocation) this).removeTemporarySpritesWithID)
    });
    this.temporarySprites.Add(new TemporaryAnimatedSprite("LooseSprites\\Cursors", new Microsoft.Xna.Framework.Rectangle(524, 230, 34, 10), new Vector2(90f, 38f) * 64f + new Vector2(0.0f, 17f) * 4f, false, 0.0f, Color.White)
    {
      interval = 100f,
      totalNumberOfLoops = 1,
      animationLength = 3,
      pingPong = true,
      scale = 4f,
      layerDepth = 0.2563f,
      id = num,
      extraInfoForEndBehavior = num
    });
    ParsedItemData dataOrErrorItem = ItemRegistry.GetDataOrErrorItem(item.QualifiedItemId);
    ColoredObject coloredObject = item as ColoredObject;
    Vector2 position = new Vector2(90f, 38f) * 64f + new Vector2((float) (8 + Game1.random.Next(6)), 2f) * 4f;
    bool[] flagArray = new bool[2]{ false, true };
    foreach (bool flag in flagArray)
    {
      if (!flag || coloredObject != null && !coloredObject.ColorSameIndexAsParentSheetIndex)
        this.temporarySprites.Add(new TemporaryAnimatedSprite(dataOrErrorItem.TextureName, dataOrErrorItem.GetSourceRect(flag ? 1 : 0), position, false, 0.0f, Color.White)
        {
          interval = 9999f,
          scale = 4f,
          alphaFade = 0.045f,
          layerDepth = 0.25622502f,
          motion = new Vector2(0.0f, 0.3f),
          acceleration = new Vector2(0.0f, 0.2f),
          scaleChange = -0.05f,
          color = coloredObject != null ? coloredObject.color.Value : Color.White
        });
    }
  }

  public override bool checkAction(Location tileLocation, xTile.Dimensions.Rectangle viewport, Farmer who)
  {
    if (this.farmhouseRestored.Value && tileLocation.X >= this.shippingBinPosition.X && tileLocation.X <= this.shippingBinPosition.X + 1 && tileLocation.Y >= this.shippingBinPosition.Y - 1 && tileLocation.Y <= this.shippingBinPosition.Y)
    {
      ItemGrabMenu itemGrabMenu = new ItemGrabMenu((IList<Item>) null, true, false, new InventoryMenu.highlightThisItem(Utility.highlightShippableObjects), new ItemGrabMenu.behaviorOnItemSelect(Game1.getFarm().shipItem), "", snapToBottom: true, canBeExitedWithKey: true, playRightClickSound: false, context: (object) this);
      itemGrabMenu.initializeUpperRightCloseButton();
      itemGrabMenu.setBackgroundTransparency(false);
      itemGrabMenu.setDestroyItemOnClick(true);
      itemGrabMenu.initializeShippingBin();
      Game1.activeClickableMenu = (IClickableMenu) itemGrabMenu;
      this.playSound("shwip");
      if (Game1.player.FacingDirection == 1)
        Game1.player.Halt();
      Game1.player.showCarrying();
      return true;
    }
    if (this.getTileIndexAt(tileLocation.X, tileLocation.Y, "Buildings", "untitled tile sheet") == 1470)
    {
      int actualFoundWalnutsCount;
      if (!IslandWest.IsQiWalnutRoomDoorUnlocked(out actualFoundWalnutsCount))
      {
        Game1.drawObjectDialogue(Game1.content.LoadString("Strings\\Locations:qiNutDoor", (object) actualFoundWalnutsCount));
      }
      else
      {
        Game1.playSound("doorClose");
        Game1.warpFarmer("QiNutRoom", 7, 8, 0);
      }
      return true;
    }
    NPC characterFromName = this.getCharacterFromName("Birdie");
    if (characterFromName != null && !characterFromName.IsInvisible && (characterFromName.Tile == new Vector2((float) tileLocation.X, (float) tileLocation.Y) || characterFromName.Tile == new Vector2((float) (tileLocation.X - 1), (float) tileLocation.Y)))
    {
      if (who.mailReceived.Add("birdieQuestBegun"))
      {
        who.Halt();
        Game1.globalFadeToBlack((Game1.afterFadeFunction) (() => this.startEvent(new StardewValley.Event(Game1.content.LoadString("Strings\\Locations:IslandSecret_Event_BirdieIntro"), (string) null, "-888999"))));
        return true;
      }
      if (who.hasQuest("130") && who.ActiveObject?.QualifiedItemId == "(O)870" && who.mailReceived.Add("birdieQuestFinished"))
      {
        who.Halt();
        Game1.globalFadeToBlack((Game1.afterFadeFunction) (() =>
        {
          who.reduceActiveItemByOne();
          this.startEvent(new StardewValley.Event(Game1.content.LoadString("Strings\\Locations:IslandSecret_Event_BirdieFinished"), (string) null, "-666777"));
        }));
        return true;
      }
      if (who.mailReceived.Contains("birdieQuestFinished"))
      {
        if (who.ActiveObject != null)
        {
          Game1.DrawDialogue(characterFromName, "Data\\ExtraDialogue:Birdie_NoGift");
        }
        else
        {
          Dialogue dialogue = Dialogue.TryGetDialogue(characterFromName, "Data\\ExtraDialogue:Birdie" + Game1.dayOfMonth.ToString());
          if (dialogue != null)
            Game1.DrawDialogue(dialogue);
          else
            Game1.DrawDialogue(characterFromName, "Data\\ExtraDialogue:Birdie" + (Game1.dayOfMonth % 7).ToString());
        }
      }
    }
    return base.checkAction(tileLocation, viewport, who);
  }

  public static bool IsQiWalnutRoomDoorUnlocked(out int actualFoundWalnutsCount)
  {
    actualFoundWalnutsCount = Math.Max(0, Game1.netWorldState.Value.GoldenWalnutsFound - 1);
    return actualFoundWalnutsCount >= 100;
  }

  public override bool isActionableTile(int xTile, int yTile, Farmer who)
  {
    if (!Game1.eventUp)
    {
      NPC characterFromName = this.getCharacterFromName("Birdie");
      if (characterFromName != null && !characterFromName.IsInvisible && characterFromName.Tile == new Vector2((float) (xTile - 1), (float) yTile) && (!who.mailReceived.Contains("birdieQuestBegun") || who.mailReceived.Contains("birdieQuestFinished")))
      {
        Game1.isSpeechAtCurrentCursorTile = true;
        return true;
      }
    }
    return base.isActionableTile(xTile, yTile, who);
  }

  protected override void initNetFields()
  {
    base.initNetFields();
    this.NetFields.AddField((INetSerializable) this.addedSlimesToday, "addedSlimesToday").AddField((INetSerializable) this.farmhouseRestored, "farmhouseRestored").AddField((INetSerializable) this.sandDuggy, "sandDuggy").AddField((INetSerializable) this.farmhouseMailbox, "farmhouseMailbox").AddField((INetSerializable) this.farmObelisk, "farmObelisk");
    this.farmhouseRestored.fieldChangeEvent += (FieldChange<NetBool, bool>) ((f, oldValue, newValue) =>
    {
      if (!newValue || this.mapPath.Value == null)
        return;
      this.ApplyFarmHouseRestore();
    });
    this.farmhouseMailbox.fieldChangeEvent += (FieldChange<NetBool, bool>) ((f, oldValue, newValue) =>
    {
      if (!newValue || this.mapPath.Value == null)
        return;
      this.ApplyFarmHouseRestore();
    });
    this.farmObelisk.fieldChangeEvent += (FieldChange<NetBool, bool>) ((f, oldValue, newValue) =>
    {
      if (!newValue || this.mapPath.Value == null)
        return;
      this.ApplyFarmObeliskBuild();
    });
  }

  public void ApplyFarmObeliskBuild()
  {
    if (this.map == null || this._appliedMapOverrides.Contains("Island_W_Obelisk"))
      return;
    this.ApplyMapOverride("Island_W_Obelisk", destination_rect: new Microsoft.Xna.Framework.Rectangle?(new Microsoft.Xna.Framework.Rectangle(71, 29, 3, 9)));
  }

  public void ApplyFarmHouseRestore()
  {
    if (this.map == null)
      return;
    if (!this._appliedMapOverrides.Contains("Island_House_Restored"))
    {
      this.ApplyMapOverride("Island_House_Restored", destination_rect: new Microsoft.Xna.Framework.Rectangle?(new Microsoft.Xna.Framework.Rectangle(74, 33, 7, 9)));
      this.ApplyMapOverride("Island_House_Bin", destination_rect: new Microsoft.Xna.Framework.Rectangle?(new Microsoft.Xna.Framework.Rectangle(this.shippingBinPosition.X, this.shippingBinPosition.Y - 1, 2, 2)));
      this.ApplyMapOverride("Island_House_Cave", destination_rect: new Microsoft.Xna.Framework.Rectangle?(new Microsoft.Xna.Framework.Rectangle(95, 30, 3, 4)));
    }
    if (!this.farmhouseMailbox.Value)
      return;
    this.setMapTile(81, 40, 771, "Buildings", "untitled tile sheet", "Mailbox");
    this.setMapTile(81, 39, 739, "Front", "untitled tile sheet");
  }

  public override void monsterDrop(Monster monster, int x, int y, Farmer who)
  {
    base.monsterDrop(monster, x, y, who);
    if (!Game1.MasterPlayer.hasOrWillReceiveMail("tigerSlimeNut"))
    {
      int num = 0;
      foreach (NPC character in this.characters)
      {
        if (character is GreenSlime && character.name.Value == "Tiger Slime")
          ++num;
      }
      if (num == 1)
      {
        Game1.addMailForTomorrow("tigerSlimeNut", true, true);
        Game1.player.team.RequestLimitedNutDrops("TigerSlimeNut", (GameLocation) this, x, y, 1);
      }
    }
    if (Game1.random.NextDouble() >= 0.01)
      return;
    long uniqueMultiplayerId = who != null ? who.UniqueMultiplayerID : 0L;
    Game1.createObjectDebris("(O)826", x, y, uniqueMultiplayerId, (GameLocation) this);
  }

  public override void TransferDataFromSavedLocation(GameLocation l)
  {
    if (l is IslandWest islandWest)
    {
      this.farmhouseRestored.Value = islandWest.farmhouseRestored.Value;
      this.farmhouseMailbox.Value = islandWest.farmhouseMailbox.Value;
      this.farmObelisk.Value = islandWest.farmObelisk.Value;
      this.sandDuggy.Value.whacked.Value = islandWest.sandDuggy.Value.whacked.Value;
    }
    base.TransferDataFromSavedLocation(l);
  }

  public override void spawnObjects()
  {
    base.spawnObjects();
    Microsoft.Xna.Framework.Rectangle r1 = new Microsoft.Xna.Framework.Rectangle(57, 78, 43, 8);
    if (Utility.getNumObjectsOfIndexWithinRectangle(r1, new string[1]
    {
      "(O)25"
    }, (GameLocation) this) < 10)
    {
      Vector2 positionInThisRectangle = Utility.getRandomPositionInThisRectangle(r1, Game1.random);
      if (this.CanItemBePlacedHere(positionInThisRectangle, ignorePassables: CollisionMask.None))
        this.objects.Add(positionInThisRectangle, new StardewValley.Object("25", 1)
        {
          MinutesUntilReady = 8,
          Flipped = Game1.random.NextBool()
        });
    }
    Microsoft.Xna.Framework.Rectangle r2 = new Microsoft.Xna.Framework.Rectangle(20, 71, 28, 16 /*0x10*/);
    if (Utility.getNumObjectsOfIndexWithinRectangle(r2, new string[2]
    {
      "(O)393",
      "(O)397"
    }, (GameLocation) this) >= 5)
      return;
    Vector2 positionInThisRectangle1 = Utility.getRandomPositionInThisRectangle(r2, Game1.random);
    if (!this.CanItemBePlacedHere(positionInThisRectangle1, ignorePassables: CollisionMask.None))
      return;
    StardewValley.Object @object = ItemRegistry.Create<StardewValley.Object>(Game1.random.NextDouble() < 0.1 ? "(O)397" : "(O)393");
    @object.IsSpawnedObject = true;
    @object.CanBeGrabbed = true;
    this.objects.Add(positionInThisRectangle1, @object);
  }

  public override string checkForBuriedItem(
    int xLocation,
    int yLocation,
    bool explosion,
    bool detectOnly,
    Farmer who)
  {
    if (xLocation == 18 && yLocation == 42 && who.secretNotesSeen.Contains(1004))
    {
      Game1.player.team.RequestLimitedNutDrops("Island_W_BuriedTreasureNut", (GameLocation) this, xLocation * 64 /*0x40*/, yLocation * 64 /*0x40*/, 1);
      if (!Game1.player.hasOrWillReceiveMail("Island_W_BuriedTreasure"))
      {
        Game1.createItemDebris(ItemRegistry.Create("(O)877"), new Vector2((float) xLocation, (float) yLocation) * 64f, 1);
        Game1.addMailForTomorrow("Island_W_BuriedTreasure", true);
      }
    }
    else if (xLocation == 104 && yLocation == 74 && who.secretNotesSeen.Contains(1006))
    {
      Game1.player.team.RequestLimitedNutDrops("Island_W_BuriedTreasureNut2", (GameLocation) this, xLocation * 64 /*0x40*/, yLocation * 64 /*0x40*/, 1);
      if (!Game1.player.hasOrWillReceiveMail("Island_W_BuriedTreasure2"))
      {
        Game1.createItemDebris(ItemRegistry.Create("(O)797"), new Vector2((float) xLocation, (float) yLocation) * 64f, 1);
        Game1.addMailForTomorrow("Island_W_BuriedTreasure2", true);
      }
    }
    return base.checkForBuriedItem(xLocation, yLocation, explosion, detectOnly, who);
  }

  protected override bool breakStone(string stoneId, int x, int y, Farmer who, Random r)
  {
    if (r.NextDouble() < (stoneId == "25" ? 0.025 : 0.01))
    {
      long uniqueMultiplayerId = who != null ? who.UniqueMultiplayerID : 0L;
      Game1.createObjectDebris("(O)826", x, y, uniqueMultiplayerId, (GameLocation) this);
    }
    return base.breakStone(stoneId, x, y, who, r);
  }

  public override void DayUpdate(int dayOfMonth)
  {
    base.DayUpdate(dayOfMonth);
    this.characters.RemoveWhere((Func<NPC, bool>) (npc => npc is Monster));
    this.addedSlimesToday.Value = false;
    this.terrainFeatures.RemoveWhere((Func<KeyValuePair<Vector2, TerrainFeature>, bool>) (pair => pair.Value is HoeDirt hoeDirt1 && hoeDirt1.crop != null && hoeDirt1.crop.forageCrop.Value));
    Microsoft.Xna.Framework.Rectangle[] rectangleArray = new Microsoft.Xna.Framework.Rectangle[9]
    {
      new Microsoft.Xna.Framework.Rectangle(31 /*0x1F*/, 43, 7, 6),
      new Microsoft.Xna.Framework.Rectangle(37, 62, 6, 5),
      new Microsoft.Xna.Framework.Rectangle(48 /*0x30*/, 42, 5, 4),
      new Microsoft.Xna.Framework.Rectangle(71, 12, 5, 4),
      new Microsoft.Xna.Framework.Rectangle(50, 59, 1, 1),
      new Microsoft.Xna.Framework.Rectangle(47, 64 /*0x40*/, 1, 1),
      new Microsoft.Xna.Framework.Rectangle(36, 58, 1, 1),
      new Microsoft.Xna.Framework.Rectangle(56, 48 /*0x30*/, 1, 1),
      new Microsoft.Xna.Framework.Rectangle(29, 46, 1, 1)
    };
    for (int index = 0; index < 5; ++index)
    {
      Microsoft.Xna.Framework.Rectangle rectangle = rectangleArray[Game1.random.Next(rectangleArray.Length)];
      Vector2 tileLocation = new Vector2((float) Game1.random.Next(rectangle.X, rectangle.Right), (float) Game1.random.Next(rectangle.Y, rectangle.Bottom));
      foreach (Vector2 openTile in Utility.recursiveFindOpenTiles((GameLocation) this, tileLocation, 16 /*0x10*/))
      {
        string str = this.doesTileHaveProperty((int) openTile.X, (int) openTile.Y, "Diggable", "Back", false);
        if (!this.terrainFeatures.ContainsKey(openTile) && str != null && Game1.random.NextDouble() < 1.0 - (double) Vector2.Distance(tileLocation, openTile) * 0.34999999403953552)
        {
          HoeDirt hoeDirt2 = new HoeDirt(0, new Crop(true, "2", (int) openTile.X, (int) openTile.Y, (GameLocation) this));
          hoeDirt2.state.Value = 2;
          this.terrainFeatures.Add(openTile, (TerrainFeature) hoeDirt2);
        }
      }
    }
    if (!Game1.MasterPlayer.mailReceived.Contains("Island_Turtle"))
      return;
    this.spawnWeedsAndStones(20, true);
    if (Game1.dayOfMonth % 7 != 1)
      return;
    this.spawnWeedsAndStones(20, true, false);
  }

  /// <inheritdoc />
  public override double GetDirtDecayChance(Vector2 tile)
  {
    return this.getTileSheetIDAt((int) tile.X, (int) tile.Y, "Back") != "untitled tile sheet2" ? 1.0 : base.GetDirtDecayChance(tile);
  }

  public override void MakeMapModifications(bool force = false)
  {
    base.MakeMapModifications(force);
    if (this.farmhouseRestored.Value)
      this.ApplyFarmHouseRestore();
    if (!this.farmObelisk.Value)
      return;
    this.ApplyFarmObeliskBuild();
  }

  protected override void resetLocalState()
  {
    base.resetLocalState();
    this.shippingBinLidOpenArea = new Microsoft.Xna.Framework.Rectangle((this.shippingBinPosition.X - 1) * 64 /*0x40*/, (this.shippingBinPosition.Y - 1) * 64 /*0x40*/, 256 /*0x0100*/, 192 /*0xC0*/);
    this.shippingBinLid = new TemporaryAnimatedSprite("LooseSprites\\Cursors", new Microsoft.Xna.Framework.Rectangle(134, 226, 30, 25), new Vector2((float) this.shippingBinPosition.X, (float) (this.shippingBinPosition.Y - 1)) * 64f + new Vector2(2f, -7f) * 4f, false, 0.0f, Color.White)
    {
      holdLastFrame = true,
      destroyable = false,
      interval = 20f,
      animationLength = 13,
      paused = true,
      scale = 4f,
      layerDepth = (float) ((double) ((this.shippingBinPosition.Y + 1) * 64 /*0x40*/) / 10000.0 + 9.9999997473787516E-05),
      pingPong = true,
      pingPongMotion = 0
    };
    this.sandDuggy.Value?.ResetForPlayerEntry();
    NPC characterFromName = this.getCharacterFromName("Birdie");
    if (characterFromName != null)
    {
      if (characterFromName.Sprite.SourceRect.Width < 32 /*0x20*/)
        characterFromName.extendSourceRect(16 /*0x10*/, 0);
      characterFromName.Sprite.SpriteWidth = 32 /*0x20*/;
      characterFromName.Sprite.ignoreSourceRectUpdates = false;
      characterFromName.Sprite.setCurrentAnimation(new List<FarmerSprite.AnimationFrame>()
      {
        new FarmerSprite.AnimationFrame(8, 1000, 0, false, false),
        new FarmerSprite.AnimationFrame(9, 1000, 0, false, false)
      });
      characterFromName.Sprite.loop = true;
      characterFromName.HideShadow = true;
      characterFromName.IsInvisible = this.IsRainingHere();
    }
    if (Game1.timeOfDay > 1700)
    {
      this.temporarySprites.Add(new TemporaryAnimatedSprite("LooseSprites\\Cursors", new Microsoft.Xna.Framework.Rectangle(276, 1985, 12, 11), new Vector2(23f, 58f) * 64f + new Vector2(-16f, -32f), false, 0.0f, Color.White)
      {
        interval = 50f,
        totalNumberOfLoops = 99999,
        animationLength = 4,
        lightId = "IslandWest_Birdie",
        id = 987654,
        lightRadius = 2f,
        scale = 4f,
        layerDepth = 0.37824f
      });
      AmbientLocationSounds.addSound(new Vector2(23f, 58f), 1);
    }
    if (!this.AreMoonlightJelliesOut())
      return;
    this.addMoonlightJellies(100, Utility.CreateRandom((double) Game1.stats.DaysPlayed, (double) Game1.uniqueIDForThisGame, -24917.0), new Microsoft.Xna.Framework.Rectangle(35, 0, 60, 60));
  }

  protected override void resetSharedState()
  {
    base.resetSharedState();
    if (this.addedSlimesToday.Value)
      return;
    this.addedSlimesToday.Value = true;
    Random random = Utility.CreateRandom((double) Game1.stats.DaysPlayed, (double) Game1.uniqueIDForThisGame, 12.0);
    Microsoft.Xna.Framework.Rectangle r = new Microsoft.Xna.Framework.Rectangle(28, 24, 19, 8);
    for (int index = 5; index > 0; --index)
    {
      Vector2 positionInThisRectangle = Utility.getRandomPositionInThisRectangle(r, random);
      if (this.CanItemBePlacedHere(positionInThisRectangle))
      {
        GreenSlime greenSlime = new GreenSlime(positionInThisRectangle * 64f, 0);
        greenSlime.makeTigerSlime();
        this.characters.Add((NPC) greenSlime);
      }
    }
  }

  private void openShippingBinLid()
  {
    if (this.shippingBinLid == null)
      return;
    if (this.shippingBinLid.pingPongMotion != 1 && Game1.currentLocation == this)
      this.localSound("doorCreak");
    this.shippingBinLid.pingPongMotion = 1;
    this.shippingBinLid.paused = false;
  }

  private void closeShippingBinLid()
  {
    TemporaryAnimatedSprite shippingBinLid = this.shippingBinLid;
    if ((shippingBinLid != null ? (shippingBinLid.currentParentTileIndex > 0 ? 1 : 0) : 0) == 0)
      return;
    if (this.shippingBinLid.pingPongMotion != -1 && Game1.currentLocation == this)
      this.localSound("doorCreakReverse");
    this.shippingBinLid.pingPongMotion = -1;
    this.shippingBinLid.paused = false;
  }

  private void updateShippingBinLid(GameTime time)
  {
    if (this.isShippingBinLidOpen(true) && this.shippingBinLid.pingPongMotion == 1)
      this.shippingBinLid.paused = true;
    else if (this.shippingBinLid.currentParentTileIndex == 0 && this.shippingBinLid.pingPongMotion == -1)
    {
      if (!this.shippingBinLid.paused && Game1.currentLocation == this)
        this.localSound("woodyStep");
      this.shippingBinLid.paused = true;
    }
    this.shippingBinLid.update(time);
  }

  private bool isShippingBinLidOpen(bool requiredToBeFullyOpen = false)
  {
    return this.shippingBinLid != null && this.shippingBinLid.currentParentTileIndex >= (requiredToBeFullyOpen ? this.shippingBinLid.animationLength - 1 : 1);
  }
}
