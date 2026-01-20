// Decompiled with JetBrains decompiler
// Type: StardewValley.Locations.IslandSecret
// Assembly: Stardew Valley, Version=1.6.15.24356, Culture=neutral, PublicKeyToken=null
// MVID: AA1F513A-94F0-4EF6-A35F-2D5B4A3721BF
// Assembly location: D:\SteamLibrary\steamapps\common\Stardew Valley\Stardew Valley.dll
// XML documentation location: D:\SteamLibrary\steamapps\common\Stardew Valley\Stardew Valley.xml

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Netcode;
using StardewValley.BellsAndWhistles;
using StardewValley.Extensions;
using StardewValley.Monsters;
using System;
using System.Collections.Generic;
using System.Xml.Serialization;
using xTile.Dimensions;

#nullable disable
namespace StardewValley.Locations;

public class IslandSecret : IslandLocation
{
  [XmlIgnore]
  public List<SuspensionBridge> suspensionBridges = new List<SuspensionBridge>();
  [XmlElement("addedSlimesToday")]
  private readonly NetBool addedSlimesToday = new NetBool();

  public IslandSecret()
  {
  }

  public IslandSecret(string map, string name)
    : base(map, name)
  {
  }

  protected override void initNetFields()
  {
    base.initNetFields();
    this.NetFields.AddField((INetSerializable) this.addedSlimesToday, "addedSlimesToday");
  }

  protected override void resetSharedState()
  {
    base.resetSharedState();
    if (this.addedSlimesToday.Value)
      return;
    this.addedSlimesToday.Value = true;
    Random random = Utility.CreateRandom((double) Game1.stats.DaysPlayed, (double) Game1.uniqueIDForThisGame, 12.0);
    Microsoft.Xna.Framework.Rectangle r = new Microsoft.Xna.Framework.Rectangle(13, 15, 7, 6);
    for (int index = 5; index > 0; --index)
    {
      Vector2 positionInThisRectangle = Utility.getRandomPositionInThisRectangle(r, random);
      if (this.CanItemBePlacedHere(positionInThisRectangle))
        this.characters.Add((NPC) new GreenSlime(positionInThisRectangle * 64f, 9999899));
    }
    if (random.NextBool() && this.CanItemBePlacedHere(new Vector2(17f, 18f)))
      this.objects.Add(new Vector2(17f, 18f), ItemRegistry.Create<StardewValley.Object>("(BC)56"));
    GreenSlime greenSlime1 = new GreenSlime(new Vector2(42f, 34f) * 64f);
    greenSlime1.makeTigerSlime();
    this.characters.Add((NPC) greenSlime1);
    GreenSlime greenSlime2 = new GreenSlime(new Vector2(38f, 33f) * 64f);
    greenSlime2.makeTigerSlime();
    this.characters.Add((NPC) greenSlime2);
  }

  public override string checkForBuriedItem(
    int xLocation,
    int yLocation,
    bool explosion,
    bool detectOnly,
    Farmer who)
  {
    if (xLocation == 82 && yLocation == 83 && who.secretNotesSeen.Contains(1002))
    {
      if (!Game1.MasterPlayer.hasOrWillReceiveMail("Island_Secret_BuriedTreasureNut"))
      {
        Game1.createItemDebris(ItemRegistry.Create("(O)73"), new Vector2((float) xLocation, (float) yLocation) * 64f, 1);
        Game1.addMailForTomorrow("Island_Secret_BuriedTreasureNut", true, true);
      }
      if (!Game1.player.hasOrWillReceiveMail("Island_Secret_BuriedTreasure"))
      {
        Game1.createItemDebris(ItemRegistry.Create("(O)166"), new Vector2((float) xLocation, (float) yLocation) * 64f, 1);
        Game1.addMailForTomorrow("Island_Secret_BuriedTreasure", true);
      }
    }
    return base.checkForBuriedItem(xLocation, yLocation, explosion, detectOnly, who);
  }

  protected override void resetLocalState()
  {
    base.resetLocalState();
    this.suspensionBridges.Clear();
    this.suspensionBridges.Add(new SuspensionBridge(46, 44));
    this.suspensionBridges.Add(new SuspensionBridge(47, 34));
    NPC characterFromName = this.getCharacterFromName("Birdie");
    if (characterFromName == null)
      return;
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

  public override bool checkAction(Location tileLocation, xTile.Dimensions.Rectangle viewport, Farmer who)
  {
    NPC characterFromName = this.getCharacterFromName("Birdie");
    if (characterFromName != null && !characterFromName.IsInvisible && characterFromName.Tile == new Vector2((float) tileLocation.X, (float) tileLocation.Y))
    {
      if (who.mailReceived.Add("birdieQuestBegun"))
        Game1.globalFadeToBlack((Game1.afterFadeFunction) (() => this.startEvent(new StardewValley.Event(Game1.content.LoadString("Strings\\Locations:IslandSecret_Event_BirdieIntro")))));
      else if (!who.mailReceived.Contains("birdieQuestFinished") && who.ActiveObject?.QualifiedItemId == "(O)870")
      {
        Game1.globalFadeToBlack((Game1.afterFadeFunction) (() =>
        {
          this.startEvent(new StardewValley.Event(Game1.content.LoadString("Strings\\Locations:IslandSecret_Event_BirdieFinished")));
          who.ActiveObject = (StardewValley.Object) null;
        }));
        who.mailReceived.Add("birdieQuestFinished");
      }
    }
    return base.checkAction(tileLocation, viewport, who);
  }

  public override void DayUpdate(int dayOfMonth)
  {
    this.characters.RemoveWhere((Func<NPC, bool>) (npc => npc is Monster));
    this.addedSlimesToday.Value = false;
    base.DayUpdate(dayOfMonth);
  }

  /// <inheritdoc />
  public override bool performAction(string[] action, Farmer who, Location tileLocation)
  {
    if (!(ArgUtility.Get(action, 0) == "BananaShrine"))
      return base.performAction(action, who, tileLocation);
    if (who.CurrentItem?.QualifiedItemId == "(O)91" && this.getTemporarySpriteByID(777) == null)
    {
      this.temporarySprites.Add(new TemporaryAnimatedSprite("Maps\\springobjects", new Microsoft.Xna.Framework.Rectangle(304, 48 /*0x30*/, 16 /*0x10*/, 16 /*0x10*/), new Vector2((float) tileLocation.X, (float) (tileLocation.Y - 1)) * 64f, false, 0.0f, Color.White)
      {
        id = 888,
        scale = 4f,
        layerDepth = (float) (((double) tileLocation.Y + 1.2000000476837158) * 64.0 / 10000.0)
      });
      this.temporarySprites.Add(new TemporaryAnimatedSprite("TileSheets\\critters", new Microsoft.Xna.Framework.Rectangle(32 /*0x20*/, 352, 32 /*0x20*/, 32 /*0x20*/), 400f, 2, 999, new Vector2(15.5f, 20f) * 64f, false, false, 0.128f, 0.0f, Color.White, 4f, 0.0f, 0.0f, 0.0f)
      {
        id = 777,
        yStopCoordinate = 1561,
        motion = new Vector2(0.0f, 2f),
        reachedStopCoordinate = new TemporaryAnimatedSprite.endBehavior(this.gorillaReachedShrine),
        delayBeforeAnimationStart = 1000
      });
      this.playSound("coin");
      DelayedAction.playSoundAfterDelay("grassyStep", 1400);
      DelayedAction.playSoundAfterDelay("grassyStep", 1800);
      DelayedAction.playSoundAfterDelay("grassyStep", 2200);
      DelayedAction.playSoundAfterDelay("grassyStep", 2600);
      DelayedAction.playSoundAfterDelay("grassyStep", 3000);
      who.reduceActiveItemByOne();
      Game1.changeMusicTrack("none");
      DelayedAction.playSoundAfterDelay("gorilla_intro", 2000);
    }
    return true;
  }

  private void gorillaReachedShrine(int extra)
  {
    TemporaryAnimatedSprite temporarySpriteById = this.getTemporarySpriteByID(777);
    temporarySpriteById.sourceRect = new Microsoft.Xna.Framework.Rectangle(0, 352, 32 /*0x20*/, 32 /*0x20*/);
    temporarySpriteById.sourceRectStartingPos = Utility.PointToVector2(temporarySpriteById.sourceRect.Location);
    temporarySpriteById.currentNumberOfLoops = 0;
    temporarySpriteById.totalNumberOfLoops = 1;
    temporarySpriteById.interval = 1000f;
    temporarySpriteById.timer = 0.0f;
    temporarySpriteById.motion = Vector2.Zero;
    temporarySpriteById.animationLength = 1;
    temporarySpriteById.endFunction = new TemporaryAnimatedSprite.endBehavior(this.gorillaGrabBanana);
  }

  private void gorillaGrabBanana(int extra)
  {
    TemporaryAnimatedSprite temporarySpriteById = this.getTemporarySpriteByID(777);
    this.removeTemporarySpritesWithID(888);
    this.playSound("slimeHit");
    temporarySpriteById.sourceRect = new Microsoft.Xna.Framework.Rectangle(96 /*0x60*/, 352, 32 /*0x20*/, 32 /*0x20*/);
    temporarySpriteById.sourceRectStartingPos = Utility.PointToVector2(temporarySpriteById.sourceRect.Location);
    temporarySpriteById.currentNumberOfLoops = 0;
    temporarySpriteById.totalNumberOfLoops = 1;
    temporarySpriteById.interval = 1000f;
    temporarySpriteById.timer = 0.0f;
    temporarySpriteById.animationLength = 1;
    temporarySpriteById.endFunction = new TemporaryAnimatedSprite.endBehavior(this.gorillaEatBanana);
    this.temporarySprites.Add(temporarySpriteById);
  }

  private void gorillaEatBanana(int extra)
  {
    TemporaryAnimatedSprite temporarySpriteById = this.getTemporarySpriteByID(777);
    temporarySpriteById.sourceRect = new Microsoft.Xna.Framework.Rectangle(128 /*0x80*/, 352, 32 /*0x20*/, 32 /*0x20*/);
    temporarySpriteById.sourceRectStartingPos = Utility.PointToVector2(temporarySpriteById.sourceRect.Location);
    temporarySpriteById.currentNumberOfLoops = 0;
    temporarySpriteById.totalNumberOfLoops = 5;
    temporarySpriteById.interval = 300f;
    temporarySpriteById.timer = 0.0f;
    temporarySpriteById.animationLength = 2;
    temporarySpriteById.endFunction = new TemporaryAnimatedSprite.endBehavior(this.gorillaAfterEat);
    this.playSound("eat");
    DelayedAction.playSoundAfterDelay("eat", 600);
    DelayedAction.playSoundAfterDelay("eat", 1200);
    DelayedAction.playSoundAfterDelay("eat", 1800);
    DelayedAction.playSoundAfterDelay("eat", 2400);
    this.temporarySprites.Add(temporarySpriteById);
  }

  private void gorillaAfterEat(int extra)
  {
    TemporaryAnimatedSprite temporarySpriteById = this.getTemporarySpriteByID(777);
    temporarySpriteById.sourceRect = new Microsoft.Xna.Framework.Rectangle(0, 352, 32 /*0x20*/, 32 /*0x20*/);
    temporarySpriteById.sourceRectStartingPos = Utility.PointToVector2(temporarySpriteById.sourceRect.Location);
    temporarySpriteById.currentNumberOfLoops = 0;
    temporarySpriteById.totalNumberOfLoops = 1;
    temporarySpriteById.interval = 1000f;
    temporarySpriteById.timer = 0.0f;
    temporarySpriteById.motion = Vector2.Zero;
    temporarySpriteById.animationLength = 1;
    temporarySpriteById.endFunction = new TemporaryAnimatedSprite.endBehavior(this.gorillaSpawnNut);
    temporarySpriteById.shakeIntensity = 1f;
    temporarySpriteById.shakeIntensityChange = -0.01f;
    this.temporarySprites.Add(temporarySpriteById);
  }

  private void gorillaSpawnNut(int extra)
  {
    TemporaryAnimatedSprite temporarySpriteById = this.getTemporarySpriteByID(777);
    temporarySpriteById.sourceRect = new Microsoft.Xna.Framework.Rectangle(0, 352, 32 /*0x20*/, 32 /*0x20*/);
    temporarySpriteById.sourceRectStartingPos = Utility.PointToVector2(temporarySpriteById.sourceRect.Location);
    temporarySpriteById.currentNumberOfLoops = 0;
    temporarySpriteById.totalNumberOfLoops = 1;
    temporarySpriteById.interval = 1000f;
    temporarySpriteById.shakeIntensity = 2f;
    temporarySpriteById.shakeIntensityChange = -0.01f;
    this.playSound("grunt");
    Game1.createItemDebris(ItemRegistry.Create("(O)73"), new Vector2(16.5f, 25f) * 64f, 0, (GameLocation) this, 1280 /*0x0500*/);
    temporarySpriteById.timer = 0.0f;
    temporarySpriteById.motion = Vector2.Zero;
    temporarySpriteById.animationLength = 1;
    temporarySpriteById.endFunction = new TemporaryAnimatedSprite.endBehavior(this.gorillaReturn);
    this.temporarySprites.Add(temporarySpriteById);
  }

  private void gorillaReturn(int extra)
  {
    TemporaryAnimatedSprite temporarySpriteById = this.getTemporarySpriteByID(777);
    temporarySpriteById.sourceRect = new Microsoft.Xna.Framework.Rectangle(32 /*0x20*/, 352, 32 /*0x20*/, 32 /*0x20*/);
    temporarySpriteById.sourceRectStartingPos = Utility.PointToVector2(temporarySpriteById.sourceRect.Location);
    temporarySpriteById.currentNumberOfLoops = 0;
    temporarySpriteById.totalNumberOfLoops = 6;
    temporarySpriteById.interval = 200f;
    temporarySpriteById.timer = 0.0f;
    temporarySpriteById.motion = new Vector2(0.0f, -3f);
    temporarySpriteById.animationLength = 2;
    temporarySpriteById.yStopCoordinate = 1280 /*0x0500*/;
    temporarySpriteById.reachedStopCoordinate = (TemporaryAnimatedSprite.endBehavior) (x => this.removeTemporarySpritesWithID(777));
    this.temporarySprites.Add(temporarySpriteById);
    DelayedAction.functionAfterDelay((Action) (() => Game1.playMorningSong()), 3000);
  }

  public override void SetBuriedNutLocations()
  {
    this.buriedNutPoints.Add(new Point(23, 47));
    this.buriedNutPoints.Add(new Point(61, 21));
    base.SetBuriedNutLocations();
  }

  public override void UpdateWhenCurrentLocation(GameTime time)
  {
    base.UpdateWhenCurrentLocation(time);
    foreach (SuspensionBridge suspensionBridge in this.suspensionBridges)
      suspensionBridge.Update(time);
  }

  public override void draw(SpriteBatch b)
  {
    base.draw(b);
    foreach (SuspensionBridge suspensionBridge in this.suspensionBridges)
      suspensionBridge.Draw(b);
  }

  public override bool IsLocationSpecificPlacementRestriction(Vector2 tileLocation)
  {
    foreach (SuspensionBridge suspensionBridge in this.suspensionBridges)
    {
      if (suspensionBridge.CheckPlacementPrevention(tileLocation))
        return true;
    }
    return base.IsLocationSpecificPlacementRestriction(tileLocation);
  }
}
