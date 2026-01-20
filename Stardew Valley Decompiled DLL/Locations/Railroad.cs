// Decompiled with JetBrains decompiler
// Type: StardewValley.Locations.Railroad
// Assembly: Stardew Valley, Version=1.6.15.24356, Culture=neutral, PublicKeyToken=null
// MVID: AA1F513A-94F0-4EF6-A35F-2D5B4A3721BF
// Assembly location: D:\SteamLibrary\steamapps\common\Stardew Valley\Stardew Valley.dll
// XML documentation location: D:\SteamLibrary\steamapps\common\Stardew Valley\Stardew Valley.xml

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Graphics;
using Netcode;
using StardewValley.BellsAndWhistles;
using System;
using System.Xml.Serialization;
using xTile.Dimensions;

#nullable disable
namespace StardewValley.Locations;

public class Railroad : GameLocation
{
  /// <summary>The effective chance that a train will appear.</summary>
  private const double TrainChance = 0.09;
  public const int trainSoundDelay = 15000;
  [XmlIgnore]
  public readonly NetRef<Train> train = new NetRef<Train>();
  [XmlElement("hasTrainPassed")]
  private readonly NetBool hasTrainPassed = new NetBool(false);
  private int trainTime = -1;
  [XmlIgnore]
  public readonly NetInt trainTimer = new NetInt(0);
  public static ICue trainLoop;
  [XmlElement("witchStatueGone")]
  public readonly NetBool witchStatueGone = new NetBool(false);
  /// <summary>The chance for a train to appear on a given day. This is computed from <see cref="F:StardewValley.Locations.Railroad.TrainChance" />.</summary>
  /// <remarks>This should not be modified. See <see cref="F:StardewValley.Locations.Railroad.TrainChance" /> to modify the train spawn chance.</remarks>
  private static double DailyTrainChance;

  static Railroad()
  {
    double num = 0.09;
    if (num < 0.0001)
      num = 0.0001;
    else if (num > 0.2499)
      num = 0.2499;
    Railroad.DailyTrainChance = (1.0 - Math.Sqrt(1.0 - 4.0 * num)) * 0.5;
  }

  public Railroad()
  {
  }

  public Railroad(string map, string name)
    : base(map, name)
  {
  }

  public override void ResetForEvent(StardewValley.Event ev)
  {
    base.ResetForEvent(ev);
    if (!(ev?.id == "528052"))
      return;
    ev.eventPositionTileOffset.X -= 8f;
    ev.eventPositionTileOffset.Y -= 2f;
  }

  protected override void initNetFields()
  {
    base.initNetFields();
    this.NetFields.AddField((INetSerializable) this.train, "train").AddField((INetSerializable) this.hasTrainPassed, "hasTrainPassed").AddField((INetSerializable) this.witchStatueGone, "witchStatueGone").AddField((INetSerializable) this.trainTimer, "trainTimer");
    this.witchStatueGone.fieldChangeEvent += (FieldChange<NetBool, bool>) ((field, oldValue, newValue) =>
    {
      if (!(!oldValue & newValue) || this.Map == null)
        return;
      DelayedAction.removeTileAfterDelay(54, 35, 2000, (GameLocation) this, "Buildings");
      DelayedAction.removeTileAfterDelay(54, 34, 2000, (GameLocation) this, "Front");
    });
  }

  public override void MakeMapModifications(bool force = false)
  {
    base.MakeMapModifications(force);
    if (this.witchStatueGone.Value || Game1.MasterPlayer.mailReceived.Contains("witchStatueGone"))
    {
      this.removeTile(54, 35, "Buildings");
      this.removeTile(54, 34, "Front");
    }
    if (!Game1.MasterPlayer.mailReceived.Contains("Farm_Eternal"))
      return;
    this.removeTile(24, 34, "Buildings");
    this.removeTile(25, 34, "Buildings");
    this.removeTile(24, 35, "Buildings");
    this.removeTile(25, 35, "Buildings");
  }

  protected override void resetLocalState()
  {
    base.resetLocalState();
    if (this.IsWinterHere())
      return;
    AmbientLocationSounds.addSound(new Vector2(15f, 56f), 0);
  }

  public override void cleanupBeforePlayerExit()
  {
    base.cleanupBeforePlayerExit();
    Railroad.trainLoop?.Stop(AudioStopOptions.Immediate);
    Railroad.trainLoop = (ICue) null;
  }

  public override string checkForBuriedItem(
    int xLocation,
    int yLocation,
    bool explosion,
    bool detectOnly,
    Farmer who)
  {
    if (!who.secretNotesSeen.Contains(16 /*0x10*/) || xLocation != 12 || yLocation != 38 || !who.mailReceived.Add("SecretNote16_done"))
      return base.checkForBuriedItem(xLocation, yLocation, explosion, detectOnly, who);
    Game1.createObjectDebris("(O)166", xLocation, yLocation, who.UniqueMultiplayerID, (GameLocation) this);
    return "";
  }

  public override bool checkAction(Location tileLocation, xTile.Dimensions.Rectangle viewport, Farmer who)
  {
    if (this.getTileIndexAt(tileLocation, "Buildings", "untitled tile sheet") != 287)
      return base.checkAction(tileLocation, viewport, who);
    if (Game1.player.hasDarkTalisman)
    {
      Game1.player.freezePause = 7000;
      this.playSound("fireball");
      DelayedAction.playSoundAfterDelay("secret1", 2000);
      DelayedAction.removeTemporarySpriteAfterDelay((GameLocation) this, 9999, 2000);
      this.witchStatueGone.Value = true;
      who.mailReceived.Add("witchStatueGone");
      for (int index = 0; index < 22; ++index)
        DelayedAction.playSoundAfterDelay("batFlap", 2220 + 240 /*0xF0*/ * index);
      Game1.multiplayer.broadcastSprites((GameLocation) this, new TemporaryAnimatedSprite("LooseSprites\\Cursors", new Microsoft.Xna.Framework.Rectangle(576, 271, 28, 31 /*0x1F*/), 60f, 3, 999, new Vector2(54f, 34f) * 64f + new Vector2(-2f, 1f) * 4f, false, false, 0.2176f, 0.0f, Color.White, 4f, 0.0f, 0.0f, 0.0f)
      {
        xPeriodic = true,
        xPeriodicLoopTime = 8000f,
        xPeriodicRange = 384f,
        motion = new Vector2(-2f, 0.0f),
        acceleration = new Vector2(0.0f, -0.015f),
        pingPong = true,
        delayBeforeAnimationStart = 2000
      });
      Game1.multiplayer.broadcastSprites((GameLocation) this, new TemporaryAnimatedSprite("LooseSprites\\Cursors", new Microsoft.Xna.Framework.Rectangle(0, 499, 10, 11), 50f, 7, 999, new Vector2(54f, 34f) * 64f + new Vector2(7f, 11f) * 4f, false, false, 0.2177f, 0.0f, Color.White, 4f, 0.0f, 0.0f, 0.0f)
      {
        xPeriodic = true,
        xPeriodicLoopTime = 8000f,
        xPeriodicRange = 384f,
        motion = new Vector2(-2f, 0.0f),
        acceleration = new Vector2(0.0f, -0.015f),
        delayBeforeAnimationStart = 2000
      });
      Game1.multiplayer.broadcastSprites((GameLocation) this, new TemporaryAnimatedSprite("LooseSprites\\Cursors", new Microsoft.Xna.Framework.Rectangle(0, 499, 10, 11), 35.715f, 7, 8, new Vector2(54f, 34f) * 64f + new Vector2(3f, 10f) * 4f, false, false, 0.2305f, 0.0f, Color.White, 4f, 0.0f, 0.0f, 0.0f)
      {
        id = 9999
      });
    }
    else
      Game1.drawObjectDialogue("???");
    return true;
  }

  internal void ResetTrainForNewDay()
  {
    this.hasTrainPassed.Value = false;
    this.trainTime = -1;
    Random random1 = Utility.CreateRandom((double) Game1.stats.DaysPlayed, (double) (Game1.uniqueIDForThisGame / 2UL));
    Random random2 = Utility.CreateRandom((double) (Game1.stats.DaysPlayed + 1U), (double) (Game1.uniqueIDForThisGame / 2UL));
    int num = random1.NextDouble() < Railroad.DailyTrainChance ? 1 : 0;
    bool flag = random2.NextDouble() < Railroad.DailyTrainChance;
    if (num == 0 || flag || !Game1.isLocationAccessible(nameof (Railroad)))
      return;
    this.trainTime = 900;
    this.trainTime -= this.trainTime % 10;
  }

  public override void DayUpdate(int dayOfMonth)
  {
    base.DayUpdate(dayOfMonth);
    this.ResetTrainForNewDay();
  }

  public override bool isCollidingPosition(
    Microsoft.Xna.Framework.Rectangle position,
    xTile.Dimensions.Rectangle viewport,
    bool isFarmer,
    int damagesFarmer,
    bool glider,
    Character character)
  {
    return !Game1.eventUp && this.train.Value != null && this.train.Value.getBoundingBox().Intersects(position) || base.isCollidingPosition(position, viewport, isFarmer, damagesFarmer, glider, character);
  }

  public void setTrainComing(int delay)
  {
    this.trainTimer.Value = delay;
    if (!Game1.IsMasterGame)
      return;
    this.PlayTrainApproach();
    Game1.multiplayer.sendServerToClientsMessage("trainApproach");
  }

  public void PlayTrainApproach()
  {
    bool? isOutdoors = Game1.currentLocation?.IsOutdoors;
    if (!isOutdoors.HasValue || !isOutdoors.GetValueOrDefault() || Game1.isFestival() || !Game1.currentLocation.InValleyContext())
      return;
    Game1.showGlobalMessage(Game1.content.LoadString("Strings\\Locations:Railroad_TrainComing"));
    ICue cue;
    Game1.playSound("distantTrain", out cue);
    cue.SetVariable("Volume", 100f);
  }

  public override Item getFish(
    float millisecondsAfterNibble,
    string bait,
    int waterDepth,
    Farmer who,
    double baitPotency,
    Vector2 bobberTile,
    string locationName = null)
  {
    if (!Game1.player.secretNotesSeen.Contains(GameLocation.NECKLACE_SECRET_NOTE_INDEX) || Game1.player.hasOrWillReceiveMail(GameLocation.CAROLINES_NECKLACE_MAIL))
      return base.getFish(millisecondsAfterNibble, bait, waterDepth, who, baitPotency, bobberTile, locationName);
    Game1.player.mailForTomorrow.Add(GameLocation.CAROLINES_NECKLACE_MAIL + "%&NL&%");
    Item fish = ItemRegistry.Create(GameLocation.CAROLINES_NECKLACE_ITEM_QID);
    Game1.player.addQuest("128");
    Game1.player.addQuest("129");
    return fish;
  }

  public override bool isTileFishable(int tileX, int tileY)
  {
    return !this.IsWinterHere() && base.isTileFishable(tileX, tileY);
  }

  public override void updateEvenIfFarmerIsntHere(GameTime time, bool skipWasUpdatedFlush = false)
  {
    base.updateEvenIfFarmerIsntHere(time, skipWasUpdatedFlush);
    if (this.train.Value != null && this.train.Value.Update(time, (GameLocation) this) && Game1.IsMasterGame)
      this.train.Value = (Train) null;
    if (Game1.IsMasterGame)
    {
      if (Game1.timeOfDay == this.trainTime - this.trainTime % 10 && this.trainTimer.Value <= 0 && !Game1.isFestival() && this.train.Value == null)
        this.setTrainComing(15000);
      if (this.trainTimer.Value > 0)
      {
        this.trainTimer.Value -= time.ElapsedGameTime.Milliseconds;
        if (this.trainTimer.Value <= 0)
        {
          this.train.Value = new Train();
          this.playSound("trainWhistle");
        }
      }
    }
    if (this.trainTimer.Value > 0 && this.trainTimer.Value < 3500)
      this.StartTrainLoopIfNeeded();
    if (this.train.Value != null)
    {
      this.StartTrainLoopIfNeeded();
      ICue trainLoop = Railroad.trainLoop;
      if ((trainLoop != null ? ((double) trainLoop.GetVariable("Volume") < 100.0 ? 1 : 0) : 0) == 0)
        return;
      Railroad.trainLoop.SetVariable("Volume", Railroad.trainLoop.GetVariable("Volume") + 0.5f);
    }
    else if (Railroad.trainLoop != null && this.trainTimer.Value <= 0)
    {
      Railroad.trainLoop.SetVariable("Volume", Railroad.trainLoop.GetVariable("Volume") - 0.15f);
      if ((double) Railroad.trainLoop.GetVariable("Volume") > 0.0)
        return;
      Railroad.trainLoop.Stop(AudioStopOptions.Immediate);
      Railroad.trainLoop = (ICue) null;
    }
    else
    {
      if (this.trainTimer.Value <= 0 || Railroad.trainLoop == null)
        return;
      Railroad.trainLoop.SetVariable("Volume", Railroad.trainLoop.GetVariable("Volume") + 0.15f);
    }
  }

  public override void draw(SpriteBatch b)
  {
    base.draw(b);
    if (this.train.Value == null || Game1.eventUp)
      return;
    this.train.Value.draw(b, (GameLocation) this);
  }

  private void StartTrainLoopIfNeeded()
  {
    if (Game1.currentLocation != this)
      return;
    bool? isPlaying = Railroad.trainLoop?.IsPlaying;
    if (isPlaying.HasValue && isPlaying.GetValueOrDefault())
      return;
    Game1.playSound("trainLoop", out Railroad.trainLoop);
    Railroad.trainLoop.SetVariable("Volume", 0.0f);
  }
}
