// Decompiled with JetBrains decompiler
// Type: StardewValley.Locations.IslandEast
// Assembly: Stardew Valley, Version=1.6.15.24356, Culture=neutral, PublicKeyToken=null
// MVID: AA1F513A-94F0-4EF6-A35F-2D5B4A3721BF
// Assembly location: D:\SteamLibrary\steamapps\common\Stardew Valley\Stardew Valley.dll
// XML documentation location: D:\SteamLibrary\steamapps\common\Stardew Valley\Stardew Valley.xml

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Netcode;
using StardewValley.BellsAndWhistles;
using System;
using xTile.Dimensions;

#nullable disable
namespace StardewValley.Locations;

public class IslandEast : IslandForestLocation
{
  protected PerchingBirds _parrots;
  protected Texture2D _parrotTextures;
  protected NetEvent0 bananaShrineEvent = new NetEvent0();
  public NetBool bananaShrineComplete = new NetBool();
  public NetBool bananaShrineNutAwarded = new NetBool();

  public IslandEast()
  {
  }

  public IslandEast(string map, string name)
    : base(map, name)
  {
  }

  protected override void initNetFields()
  {
    base.initNetFields();
    this.NetFields.AddField(this.bananaShrineEvent.NetFields, "bananaShrineEvent.NetFields").AddField((INetSerializable) this.bananaShrineComplete, "bananaShrineComplete").AddField((INetSerializable) this.bananaShrineNutAwarded, "bananaShrineNutAwarded");
    this.bananaShrineEvent.onEvent += new NetEvent0.Event(this.OnBananaShrine);
  }

  public virtual void AddTorchLights()
  {
    this.removeTemporarySpritesWithIDLocal(6666);
    int num1 = 1280 /*0x0500*/;
    int num2 = 704;
    this.temporarySprites.Add(new TemporaryAnimatedSprite("LooseSprites\\Cursors", new Microsoft.Xna.Framework.Rectangle(276, 1965, 8, 8), new Vector2((float) (num1 + 24), (float) (num2 + 48 /*0x30*/)), false, 0.0f, Color.White)
    {
      interval = 50f,
      totalNumberOfLoops = 99999,
      animationLength = 7,
      lightId = "IslandEast_TorchLight_1",
      id = 6666,
      lightRadius = 1f,
      scale = 3f,
      layerDepth = (float) ((double) (num2 + 48 /*0x30*/) / 10000.0 + 9.9999997473787516E-05),
      delayBeforeAnimationStart = 0
    });
    this.temporarySprites.Add(new TemporaryAnimatedSprite("LooseSprites\\Cursors", new Microsoft.Xna.Framework.Rectangle(276, 1984, 12, 12), new Vector2((float) (num1 + 16 /*0x10*/), (float) (num2 + 28)), false, 0.0f, Color.White)
    {
      interval = 50f,
      totalNumberOfLoops = 99999,
      animationLength = 4,
      lightId = "IslandEast_TorchLight_2",
      id = 6666,
      lightRadius = 1f,
      scale = 3f,
      layerDepth = (float) ((double) (num2 + 28) / 10000.0 + 9.9999997473787516E-05),
      delayBeforeAnimationStart = 0
    });
    int num3 = 1472;
    int num4 = 704;
    this.temporarySprites.Add(new TemporaryAnimatedSprite("LooseSprites\\Cursors", new Microsoft.Xna.Framework.Rectangle(276, 1965, 8, 8), new Vector2((float) (num3 + 24), (float) (num4 + 48 /*0x30*/)), false, 0.0f, Color.White)
    {
      interval = 50f,
      totalNumberOfLoops = 99999,
      animationLength = 7,
      lightId = "IslandEast_TorchLight_3",
      id = 6666,
      lightRadius = 1f,
      scale = 3f,
      layerDepth = (float) ((double) (num4 + 48 /*0x30*/) / 10000.0 + 9.9999997473787516E-05),
      delayBeforeAnimationStart = 0
    });
    this.temporarySprites.Add(new TemporaryAnimatedSprite("LooseSprites\\Cursors", new Microsoft.Xna.Framework.Rectangle(276, 1984, 12, 12), new Vector2((float) (num3 + 16 /*0x10*/), (float) (num4 + 28)), false, 0.0f, Color.White)
    {
      interval = 50f,
      totalNumberOfLoops = 99999,
      animationLength = 4,
      lightId = "IslandEast_TorchLight_4",
      id = 6666,
      lightRadius = 1f,
      scale = 3f,
      layerDepth = (float) ((double) (num4 + 28) / 10000.0 + 9.9999997473787516E-05),
      delayBeforeAnimationStart = 0
    });
  }

  protected override void resetLocalState()
  {
    this._parrotTextures = Game1.temporaryContent.Load<Texture2D>("LooseSprites\\parrots");
    base.resetLocalState();
    for (int index = 0; index < 5; ++index)
      this.critters.Add((Critter) new Firefly(Utility.getRandomPositionInThisRectangle(new Microsoft.Xna.Framework.Rectangle(14, 3, 16 /*0x10*/, 12), Game1.random)));
    this.AddTorchLights();
    if (this.bananaShrineComplete.Value)
      this.AddGorillaShrineTorches(0);
    this._parrots = new PerchingBirds(this._parrotTextures, 3, 24, 24, new Vector2(12f, 19f), new Point[9]
    {
      new Point(18, 8),
      new Point(17, 9),
      new Point(20, 7),
      new Point(21, 8),
      new Point(22, 7),
      new Point(23, 8),
      new Point(18, 12),
      new Point(25, 11),
      new Point(27, 8)
    }, new Point[0]);
    this._parrots.peckDuration = 0;
    for (int index = 0; index < 5; ++index)
      this._parrots.AddBird(Game1.random.Next(0, 4));
    if (this.bananaShrineComplete.Value && Utility.CreateRandom((double) Game1.uniqueIDForThisGame, (double) Game1.stats.DaysPlayed, 1111.0).NextDouble() < 0.1)
      this.temporarySprites.Add(new TemporaryAnimatedSprite("TileSheets\\critters", new Microsoft.Xna.Framework.Rectangle(32 /*0x20*/, 352, 32 /*0x20*/, 32 /*0x20*/), 500f, 2, 999, new Vector2(15.5f, 19f) * 64f, false, false, 0.1216f, 0.0f, Color.White, 4f, 0.0f, 0.0f, 0.0f)
      {
        id = 888,
        yStopCoordinate = 1497,
        motion = new Vector2(0.0f, 1f),
        reachedStopCoordinate = new TemporaryAnimatedSprite.endBehavior(this.gorillaReachedShrineCosmetic),
        delayBeforeAnimationStart = 1000
      });
    this.addOneTimeGiftBox(ItemRegistry.Create("(O)TentKit", 3), 30, 40, 4);
  }

  public override void cleanupBeforePlayerExit()
  {
    this._parrots = (PerchingBirds) null;
    this._parrotTextures = (Texture2D) null;
    base.cleanupBeforePlayerExit();
  }

  public override void UpdateWhenCurrentLocation(GameTime time)
  {
    base.UpdateWhenCurrentLocation(time);
    this.bananaShrineEvent.Poll();
    this._parrots?.Update(time);
    if (!this.bananaShrineComplete.Value || Game1.random.NextDouble() >= 0.005)
      return;
    TemporaryAnimatedSprite temporarySpriteById = this.getTemporarySpriteByID(888);
    if (temporarySpriteById == null || !temporarySpriteById.motion.Equals(Vector2.Zero))
      return;
    this.temporarySprites.Add(new TemporaryAnimatedSprite("TileSheets\\critters", new Microsoft.Xna.Framework.Rectangle(128 /*0x80*/, 352, 32 /*0x20*/, 32 /*0x20*/), (float) (200 + (Game1.random.NextDouble() < 0.1 ? Game1.random.Next(1000, 3000) : 0)), 1, 1, temporarySpriteById.position, false, false, 0.12224f, 0.0f, Color.White, 4f, 0.0f, 0.0f, 0.0f));
  }

  public virtual void SpawnBananaNutReward()
  {
    if (this.bananaShrineNutAwarded.Value || !Game1.IsMasterGame)
      return;
    Game1.player.team.MarkCollectedNut("BananaShrine");
    this.bananaShrineNutAwarded.Value = true;
    for (int index = 0; index < 3; ++index)
      Game1.createItemDebris(ItemRegistry.Create("(O)73"), new Vector2(16.5f, 25f) * 64f, 0, (GameLocation) this, 1280 /*0x0500*/);
  }

  public override void DayUpdate(int dayOfMonth)
  {
    if (Game1.IsMasterGame && this.bananaShrineComplete.Value && !this.bananaShrineNutAwarded.Value)
      this.SpawnBananaNutReward();
    base.DayUpdate(dayOfMonth);
    Microsoft.Xna.Framework.Rectangle rectangle = new Microsoft.Xna.Framework.Rectangle(27, 27, 3, 3);
    for (int index = 0; index < 8; ++index)
    {
      Vector2 randomTile = this.getRandomTile();
      if ((double) randomTile.Y < 24.0)
        randomTile.Y += 24f;
      if ((double) randomTile.X > 4.0 && !this.hasTileAt((int) randomTile.X, (int) randomTile.Y, "AlwaysFront") && this.CanItemBePlacedHere(randomTile, ignorePassables: CollisionMask.None) && this.doesTileHavePropertyNoNull((int) randomTile.X, (int) randomTile.Y, "Type", "Back") == "Grass" && !this.IsNoSpawnTile(randomTile) && this.doesTileHavePropertyNoNull((int) randomTile.X + 1, (int) randomTile.Y, "Type", "Back") != "Stone" && this.doesTileHavePropertyNoNull((int) randomTile.X - 1, (int) randomTile.Y, "Type", "Back") != "Stone" && this.doesTileHavePropertyNoNull((int) randomTile.X, (int) randomTile.Y + 1, "Type", "Back") != "Stone" && this.doesTileHavePropertyNoNull((int) randomTile.X, (int) randomTile.Y - 1, "Type", "Back") != "Stone" && !rectangle.Contains((int) randomTile.X, (int) randomTile.Y))
      {
        if (Game1.random.NextDouble() < 0.04)
        {
          StardewValley.Object @object = ItemRegistry.Create<StardewValley.Object>("(O)259");
          @object.isSpawnedObject.Value = true;
          this.objects.Add(randomTile, @object);
        }
        else
          this.objects.Add(randomTile, ItemRegistry.Create<StardewValley.Object>("(O)" + (882 + Game1.random.Next(3)).ToString()));
      }
    }
  }

  public override void drawAboveAlwaysFrontLayer(SpriteBatch b)
  {
    this._parrots?.Draw(b);
    base.drawAboveAlwaysFrontLayer(b);
  }

  public virtual void AddGorillaShrineTorches(int delay)
  {
    if (this.getTemporarySpriteByID(12038) != null)
      return;
    this.temporarySprites.Add(new TemporaryAnimatedSprite("LooseSprites\\Cursors", new Microsoft.Xna.Framework.Rectangle(276, 1985, 12, 11), new Vector2(15f, 24f) * 64f + new Vector2(8f, -16f), false, 0.0f, Color.White)
    {
      interval = 50f,
      totalNumberOfLoops = 99999,
      animationLength = 4,
      lightId = "IslandEast_GorillaTorch_1",
      lightRadius = 2f,
      delayBeforeAnimationStart = delay,
      scale = 4f,
      layerDepth = 0.16704f,
      id = 12038
    });
    this.temporarySprites.Add(new TemporaryAnimatedSprite("LooseSprites\\Cursors", new Microsoft.Xna.Framework.Rectangle(276, 1985, 12, 11), new Vector2(17f, 24f) * 64f + new Vector2(8f, -16f), false, 0.0f, Color.White)
    {
      interval = 50f,
      totalNumberOfLoops = 99999,
      animationLength = 4,
      lightId = "IslandEast_GorillaTorch_2",
      lightRadius = 2f,
      delayBeforeAnimationStart = delay,
      scale = 4f,
      layerDepth = 0.16704f,
      id = 12097
    });
  }

  public override void TransferDataFromSavedLocation(GameLocation l)
  {
    base.TransferDataFromSavedLocation(l);
    if (!(l is IslandEast islandEast))
      return;
    this.bananaShrineComplete.Value = islandEast.bananaShrineComplete.Value;
    this.bananaShrineNutAwarded.Value = islandEast.bananaShrineNutAwarded.Value;
  }

  public virtual void OnBananaShrine()
  {
    Location location = new Location(16 /*0x10*/, 26);
    this.temporarySprites.Add(new TemporaryAnimatedSprite("Maps\\springobjects", new Microsoft.Xna.Framework.Rectangle(304, 48 /*0x30*/, 16 /*0x10*/, 16 /*0x10*/), new Vector2(16f, (float) (location.Y - 1)) * 64f, false, 0.0f, Color.White)
    {
      id = 88976,
      scale = 4f,
      layerDepth = (float) (((double) location.Y + 1.2000000476837158) * 64.0 / 10000.0),
      dontClearOnAreaEntry = true
    });
    this.temporarySprites.Add(new TemporaryAnimatedSprite("TileSheets\\critters", new Microsoft.Xna.Framework.Rectangle(32 /*0x20*/, 352, 32 /*0x20*/, 32 /*0x20*/), 400f, 2, 999, new Vector2(15.5f, 19f) * 64f, false, false, 0.1216f, 0.0f, Color.White, 4f, 0.0f, 0.0f, 0.0f)
    {
      id = 777,
      yStopCoordinate = 1497,
      motion = new Vector2(0.0f, 2f),
      reachedStopCoordinate = new TemporaryAnimatedSprite.endBehavior(this.gorillaReachedShrine),
      delayBeforeAnimationStart = 1000,
      dontClearOnAreaEntry = true
    });
    if (Game1.currentLocation == this)
    {
      Game1.playSound("coin");
      DelayedAction.playSoundAfterDelay("fireball", 800);
    }
    this.AddGorillaShrineTorches(800);
    if (Game1.currentLocation != this)
      return;
    DelayedAction.playSoundAfterDelay("grassyStep", 1400);
    DelayedAction.playSoundAfterDelay("grassyStep", 1800);
    DelayedAction.playSoundAfterDelay("grassyStep", 2200);
    DelayedAction.playSoundAfterDelay("grassyStep", 2600);
    DelayedAction.playSoundAfterDelay("grassyStep", 3000);
    Game1.changeMusicTrack("none");
    DelayedAction.playSoundAfterDelay("gorilla_intro", 2000);
  }

  /// <inheritdoc />
  public override bool performAction(string[] action, Farmer who, Location tileLocation)
  {
    if (ArgUtility.Get(action, 0) == "BananaShrine")
    {
      if (who.CurrentItem?.QualifiedItemId == "(O)91" && this.getTemporarySpriteByID(777) == null && !this.bananaShrineComplete.Value)
      {
        this.bananaShrineComplete.Value = true;
        who.reduceActiveItemByOne();
        this.bananaShrineEvent.Fire();
        return true;
      }
      if (this.getTemporarySpriteByID(777) == null && !this.bananaShrineComplete.Value)
        who.doEmote(8);
    }
    return base.performAction(action, who, tileLocation);
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

  private void gorillaReachedShrineCosmetic(int extra)
  {
    TemporaryAnimatedSprite temporarySpriteById = this.getTemporarySpriteByID(888);
    temporarySpriteById.sourceRect = new Microsoft.Xna.Framework.Rectangle(192 /*0xC0*/, 352, 32 /*0x20*/, 32 /*0x20*/);
    temporarySpriteById.sourceRectStartingPos = Utility.PointToVector2(temporarySpriteById.sourceRect.Location);
    temporarySpriteById.currentNumberOfLoops = 0;
    temporarySpriteById.totalNumberOfLoops = 999999;
    temporarySpriteById.interval = 8000f;
    temporarySpriteById.timer = 0.0f;
    temporarySpriteById.motion = Vector2.Zero;
    temporarySpriteById.animationLength = 1;
  }

  private void gorillaGrabBanana(int extra)
  {
    TemporaryAnimatedSprite temporarySpriteById = this.getTemporarySpriteByID(777);
    DelayedAction.functionAfterDelay((Action) (() => this.removeTemporarySpritesWithID(88976)), 50);
    if (Game1.currentLocation == this)
      Game1.playSound("slimeHit");
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
    if (Game1.currentLocation == this)
    {
      Game1.playSound("eat");
      DelayedAction.playSoundAfterDelay("eat", 600);
      DelayedAction.playSoundAfterDelay("eat", 1200);
      DelayedAction.playSoundAfterDelay("eat", 1800);
      DelayedAction.playSoundAfterDelay("eat", 2400);
    }
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
    if (Game1.currentLocation == this)
      Game1.playSound("grunt");
    if (Game1.IsMasterGame)
      this.SpawnBananaNutReward();
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
    if (Game1.currentLocation != this)
      return;
    DelayedAction.functionAfterDelay((Action) (() => Game1.playMorningSong()), 3000);
  }
}
