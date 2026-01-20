// Decompiled with JetBrains decompiler
// Type: StardewValley.Locations.Desert
// Assembly: Stardew Valley, Version=1.6.15.24356, Culture=neutral, PublicKeyToken=null
// MVID: AA1F513A-94F0-4EF6-A35F-2D5B4A3721BF
// Assembly location: D:\SteamLibrary\steamapps\common\Stardew Valley\Stardew Valley.dll
// XML documentation location: D:\SteamLibrary\steamapps\common\Stardew Valley\Stardew Valley.xml

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using StardewValley.Extensions;
using System;
using xTile.Dimensions;

#nullable disable
namespace StardewValley.Locations;

public class Desert : GameLocation
{
  public const int busDefaultXTile = 17;
  public const int busDefaultYTile = 24;
  private TemporaryAnimatedSprite busDoor;
  private Vector2 busPosition;
  private Vector2 busMotion;
  public bool drivingOff;
  public bool drivingBack;
  public bool leaving;
  private int chimneyTimer = 500;
  private Microsoft.Xna.Framework.Rectangle desertMerchantBounds = new Microsoft.Xna.Framework.Rectangle(2112, 1280 /*0x0500*/, 836, 280);
  public static bool warpedToDesert;
  private Microsoft.Xna.Framework.Rectangle busSource = new Microsoft.Xna.Framework.Rectangle(288, 1247, 128 /*0x80*/, 64 /*0x40*/);
  private Microsoft.Xna.Framework.Rectangle pamSource = new Microsoft.Xna.Framework.Rectangle(384, 1311, 15, 19);
  private Microsoft.Xna.Framework.Rectangle transparentWindowSource = new Microsoft.Xna.Framework.Rectangle(0, 0, 21, 41);
  private Vector2 pamOffset = new Vector2(0.0f, 29f);

  public Desert()
  {
  }

  public Desert(string mapPath, string name)
    : base(mapPath, name)
  {
  }

  public override bool checkAction(Location tileLocation, xTile.Dimensions.Rectangle viewport, Farmer who)
  {
    if (this.map.GetLayer("Buildings")?.Tiles[tileLocation] != null)
      return base.checkAction(tileLocation, viewport, who);
    if ((tileLocation.X == 41 || tileLocation.X == 42) && tileLocation.Y == 24)
    {
      this.OnDesertTrader();
      return true;
    }
    if (tileLocation.X < 34 || tileLocation.X > 38 || tileLocation.Y != 24)
      return base.checkAction(tileLocation, viewport, who);
    this.OnCamel();
    return true;
  }

  public virtual void OnDesertTrader()
  {
    Utility.TryOpenShopMenu("DesertTrade", (GameLocation) this);
  }

  public virtual void OnCamel()
  {
    Game1.playSound("camel");
    this.ShowCamelAnimation();
    Game1.player.faceDirection(0);
    Game1.haltAfterCheck = false;
  }

  public virtual void ShowCamelAnimation()
  {
    if (this.getTemporarySpriteByID(999) != null)
      return;
    this.temporarySprites.Add(new TemporaryAnimatedSprite()
    {
      texture = Game1.temporaryContent.Load<Texture2D>("LooseSprites\\temporary_sprites_1"),
      sourceRect = new Microsoft.Xna.Framework.Rectangle(208 /*0xD0*/, 591, 65, 49),
      sourceRectStartingPos = new Vector2(208f, 591f),
      animationLength = 1,
      totalNumberOfLoops = 1,
      interval = 300f,
      scale = 4f,
      position = new Vector2(536f, 340f) * 4f,
      layerDepth = 0.1332f,
      id = 999
    });
  }

  public override string checkForBuriedItem(
    int xLocation,
    int yLocation,
    bool explosion,
    bool detectOnly,
    Farmer who)
  {
    if (!who.secretNotesSeen.Contains(18) || xLocation != 40 || yLocation != 55 || !who.mailReceived.Add("SecretNote18_done"))
      return base.checkForBuriedItem(xLocation, yLocation, explosion, detectOnly, who);
    Game1.createObjectDebris("(O)127", xLocation, yLocation, who.UniqueMultiplayerID, (GameLocation) this);
    return "";
  }

  private void playerReachedBusDoor(Character c, GameLocation l)
  {
    Game1.viewportFreeze = true;
    Game1.player.position.X = -10000f;
    Game1.freezeControls = true;
    Game1.player.CanMove = false;
    this.busDriveOff();
    this.playSound("stoneStep");
  }

  public override bool answerDialogue(Response answer)
  {
    if (this.lastQuestionKey == null || this.afterQuestion != null || !($"{ArgUtility.SplitBySpaceAndGet(this.lastQuestionKey, 0)}_{answer.responseKey}" == "DesertBus_Yes"))
      return base.answerDialogue(answer);
    this.playerReachedBusDoor((Character) Game1.player, (GameLocation) this);
    return true;
  }

  protected override void resetLocalState()
  {
    base.resetLocalState();
    this.leaving = false;
    Game1.ambientLight = Color.White;
    GameLocation locationFromName = Game1.getLocationFromName(Game1.player.previousLocationName);
    bool flag = false;
    if (locationFromName == null || locationFromName.GetLocationContextId() != this.GetLocationContextId())
    {
      Desert.warpedToDesert = true;
      if (Game1.player.previousLocationName == "BusStop" && Game1.player.TilePoint.X == 16 /*0x10*/ && Game1.player.TilePoint.Y == 24)
      {
        Desert.warpedToDesert = false;
        flag = true;
        Game1.changeMusicTrack("silence");
        this.busPosition = new Vector2(17f, 24f) * 64f;
        this.busDoor = new TemporaryAnimatedSprite("LooseSprites\\Cursors", new Microsoft.Xna.Framework.Rectangle(368, 1311, 16 /*0x10*/, 38), this.busPosition + new Vector2(16f, 26f) * 4f, false, 0.0f, Color.White)
        {
          interval = 999999f,
          animationLength = 1,
          holdLastFrame = true,
          layerDepth = (float) (((double) this.busPosition.Y + 192.0) / 10000.0 + 9.9999997473787516E-06),
          scale = 4f
        };
        Game1.displayFarmer = false;
        this.busDriveBack();
      }
    }
    if (!flag)
    {
      this.drivingOff = false;
      this.drivingBack = false;
      this.busMotion = Vector2.Zero;
      this.busPosition = new Vector2(17f, 24f) * 64f;
      this.busDoor = new TemporaryAnimatedSprite("LooseSprites\\Cursors", new Microsoft.Xna.Framework.Rectangle(288, 1311, 16 /*0x10*/, 38), this.busPosition + new Vector2(16f, 26f) * 4f, false, 0.0f, Color.White)
      {
        interval = 999999f,
        animationLength = 6,
        holdLastFrame = true,
        layerDepth = (float) (((double) this.busPosition.Y + 192.0) / 10000.0 + 9.9999997473787516E-06),
        scale = 4f
      };
    }
    if (this.GetType() == typeof (DesertFestival))
      this.temporarySprites.Add(new TemporaryAnimatedSprite()
      {
        texture = Game1.temporaryContent.Load<Texture2D>("LooseSprites\\temporary_sprites_1"),
        sourceRect = new Microsoft.Xna.Framework.Rectangle(208 /*0xD0*/, 524, 65, 49),
        sourceRectStartingPos = new Vector2(208f, 524f),
        animationLength = 1,
        totalNumberOfLoops = 9999,
        interval = 99999f,
        scale = 4f,
        position = new Vector2(536f, 340f) * 4f,
        layerDepth = 0.1324f,
        id = 996
      });
    else
      this.temporarySprites.Add(new TemporaryAnimatedSprite()
      {
        texture = Game1.temporaryContent.Load<Texture2D>("LooseSprites\\temporary_sprites_1"),
        sourceRect = new Microsoft.Xna.Framework.Rectangle(0, 513, 208 /*0xD0*/, 101),
        sourceRectStartingPos = new Vector2(0.0f, 513f),
        animationLength = 1,
        totalNumberOfLoops = 9999,
        interval = 99999f,
        scale = 4f,
        position = new Vector2(528f, 298f) * 4f,
        layerDepth = 0.1324f,
        id = 996
      });
    if (this.IsTravelingDesertMerchantHere())
      this.temporarySprites.Add(new TemporaryAnimatedSprite()
      {
        texture = Game1.temporaryContent.Load<Texture2D>("LooseSprites\\temporary_sprites_1"),
        sourceRect = new Microsoft.Xna.Framework.Rectangle(0, 614, 20, 26),
        sourceRectStartingPos = new Vector2(0.0f, 614f),
        animationLength = 1,
        totalNumberOfLoops = 999,
        interval = 99999f,
        scale = 4f,
        position = new Vector2(663f, 354f) * 4f,
        layerDepth = 0.1328f,
        id = 995
      });
    if (Game1.timeOfDay < Game1.getModeratelyDarkTime((GameLocation) this))
      return;
    this.lightMerchantLamps();
  }

  private bool IsTravelingDesertMerchantHere()
  {
    return !Game1.IsWinter || Game1.dayOfMonth < 15 || Game1.dayOfMonth > 17;
  }

  public override bool isCollidingPosition(
    Microsoft.Xna.Framework.Rectangle position,
    xTile.Dimensions.Rectangle viewport,
    bool isFarmer,
    int damagesFarmer,
    bool glider,
    Character character)
  {
    return position.Intersects(this.desertMerchantBounds) || base.isCollidingPosition(position, viewport, isFarmer, damagesFarmer, glider, character);
  }

  public override void performTenMinuteUpdate(int timeOfDay)
  {
    base.performTenMinuteUpdate(timeOfDay);
    if (Game1.currentLocation != this)
      return;
    if (this.IsTravelingDesertMerchantHere())
    {
      if (Game1.random.NextDouble() < 0.33)
        this.temporarySprites.Add(new TemporaryAnimatedSprite()
        {
          texture = Game1.temporaryContent.Load<Texture2D>("LooseSprites\\temporary_sprites_1"),
          sourceRect = new Microsoft.Xna.Framework.Rectangle(40, 614, 20, 26),
          sourceRectStartingPos = new Vector2(40f, 614f),
          animationLength = 6,
          totalNumberOfLoops = 1,
          interval = 100f,
          scale = 4f,
          position = new Vector2(663f, 354f) * 4f,
          layerDepth = 0.1336f,
          id = 997,
          pingPong = true
        });
      else
        this.temporarySprites.Add(new TemporaryAnimatedSprite()
        {
          texture = Game1.temporaryContent.Load<Texture2D>("LooseSprites\\temporary_sprites_1"),
          sourceRect = new Microsoft.Xna.Framework.Rectangle(20, 614, 20, 26),
          sourceRectStartingPos = new Vector2(20f, 614f),
          animationLength = 1,
          totalNumberOfLoops = 1,
          interval = (float) Game1.random.Next(100, 800),
          scale = 4f,
          position = new Vector2(663f, 354f) * 4f,
          layerDepth = 0.1332f,
          id = 998
        });
    }
    this.ShowCamelAnimation();
    if (timeOfDay != Game1.getModeratelyDarkTime((GameLocation) this) || Game1.currentLocation != this)
      return;
    this.lightMerchantLamps();
  }

  public void lightMerchantLamps()
  {
    if (this.getTemporarySpriteByID(1000) != null)
      return;
    this.temporarySprites.Add(new TemporaryAnimatedSprite()
    {
      texture = Game1.temporaryContent.Load<Texture2D>("LooseSprites\\temporary_sprites_1"),
      sourceRect = new Microsoft.Xna.Framework.Rectangle(181, 633, 7, 6),
      sourceRectStartingPos = new Vector2(181f, 633f),
      animationLength = 1,
      totalNumberOfLoops = 9999,
      interval = 99999f,
      scale = 4f,
      position = new Vector2(545f, 309f) * 4f,
      layerDepth = 0.134f,
      id = 1000,
      lightId = "Desert_MerchantLamp_1",
      lightRadius = 1f,
      lightcolor = Color.Black
    });
    this.temporarySprites.Add(new TemporaryAnimatedSprite()
    {
      texture = Game1.temporaryContent.Load<Texture2D>("LooseSprites\\temporary_sprites_1"),
      sourceRect = new Microsoft.Xna.Framework.Rectangle(181, 633, 7, 6),
      sourceRectStartingPos = new Vector2(181f, 633f),
      animationLength = 1,
      totalNumberOfLoops = 9999,
      interval = 99999f,
      scale = 4f,
      position = new Vector2(644f, 360f) * 4f,
      layerDepth = 0.134f,
      id = 1000,
      lightId = "Desert_MerchantLamp_2",
      lightRadius = 1f,
      lightcolor = Color.Black
    });
    this.temporarySprites.Add(new TemporaryAnimatedSprite()
    {
      texture = Game1.temporaryContent.Load<Texture2D>("LooseSprites\\temporary_sprites_1"),
      sourceRect = new Microsoft.Xna.Framework.Rectangle(181, 633, 7, 6),
      sourceRectStartingPos = new Vector2(181f, 633f),
      animationLength = 1,
      totalNumberOfLoops = 9999,
      interval = 99999f,
      scale = 4f,
      position = new Vector2(717f, 309f) * 4f,
      layerDepth = 0.134f,
      id = 1000,
      lightId = "Desert_MerchantLamp_3",
      lightRadius = 1f,
      lightcolor = Color.Black
    });
  }

  public override void cleanupBeforePlayerExit()
  {
    base.cleanupBeforePlayerExit();
    if (this.farmers.Count > 1)
      return;
    this.busDoor = (TemporaryAnimatedSprite) null;
  }

  public void busDriveOff()
  {
    this.busDoor = new TemporaryAnimatedSprite("LooseSprites\\Cursors", new Microsoft.Xna.Framework.Rectangle(288, 1311, 16 /*0x10*/, 38), this.busPosition + new Vector2(16f, 26f) * 4f, false, 0.0f, Color.White)
    {
      interval = 999999f,
      animationLength = 6,
      holdLastFrame = true,
      layerDepth = (float) (((double) this.busPosition.Y + 192.0) / 10000.0 + 9.9999997473787516E-06),
      scale = 4f
    };
    this.busDoor.timer = 0.0f;
    this.busDoor.interval = 70f;
    this.busDoor.endFunction = new TemporaryAnimatedSprite.endBehavior(this.busStartMovingOff);
    this.localSound("trashcanlid");
    this.drivingBack = false;
    this.busDoor.paused = false;
  }

  public void busDriveBack()
  {
    this.busPosition.X = (float) this.map.RequireLayer("Back").DisplayWidth;
    this.busDoor.Position = this.busPosition + new Vector2(16f, 26f) * 4f;
    this.drivingBack = true;
    this.drivingOff = false;
    this.localSound("busDriveOff");
    this.busMotion = new Vector2(-6f, 0.0f);
  }

  private void busStartMovingOff(int extraInfo)
  {
    Game1.globalFadeToBlack((Game1.afterFadeFunction) (() =>
    {
      Game1.globalFadeToClear();
      this.localSound("batFlap");
      this.drivingOff = true;
      this.localSound("busDriveOff");
      Game1.changeMusicTrack("silence");
    }));
  }

  /// <inheritdoc />
  public override bool IgnoreTouchActions()
  {
    return base.IgnoreTouchActions() || this.drivingBack || this.drivingOff;
  }

  /// <inheritdoc />
  public override void performTouchAction(string[] action, Vector2 playerStandingPosition)
  {
    if (this.IgnoreTouchActions())
      return;
    if (ArgUtility.Get(action, 0) == "DesertBus")
    {
      Response[] answerChoices = new Response[2]
      {
        new Response("Yes", Game1.content.LoadString("Strings\\Locations:Desert_Return_Yes")),
        new Response("Not", Game1.content.LoadString("Strings\\Locations:Desert_Return_No"))
      };
      this.createQuestionDialogue(Game1.content.LoadString("Strings\\Locations:Desert_Return_Question"), answerChoices, "DesertBus");
    }
    else
      base.performTouchAction(action, playerStandingPosition);
  }

  private void doorOpenAfterReturn(int extraInfo)
  {
    this.localSound("batFlap");
    this.busDoor = new TemporaryAnimatedSprite("LooseSprites\\Cursors", new Microsoft.Xna.Framework.Rectangle(288, 1311, 16 /*0x10*/, 38), this.busPosition + new Vector2(16f, 26f) * 4f, false, 0.0f, Color.White)
    {
      interval = 999999f,
      animationLength = 6,
      holdLastFrame = true,
      layerDepth = (float) (((double) this.busPosition.Y + 192.0) / 10000.0 + 9.9999997473787516E-06),
      scale = 4f
    };
    Game1.player.Position = new Vector2(18f, 27f) * 64f;
    this.lastTouchActionLocation = Game1.player.Tile;
    Game1.displayFarmer = true;
    Game1.player.forceCanMove();
    Game1.player.faceDirection(2);
    Game1.changeMusicTrack("none", true);
    GameLocation.HandleMusicChange((GameLocation) null, (GameLocation) this);
  }

  private void busLeftToValley()
  {
    Game1.viewport.Y = -100000;
    Game1.viewportFreeze = true;
    Game1.warpFarmer("BusStop", 22, 10, true);
  }

  public override void UpdateWhenCurrentLocation(GameTime time)
  {
    base.UpdateWhenCurrentLocation(time);
    if (this.drivingBack || this.drivingOff)
    {
      if (Game1.player.currentLocation == this)
      {
        Game1.player.CanMove = false;
      }
      else
      {
        this.drivingBack = false;
        this.drivingOff = false;
      }
    }
    if (this.drivingOff && !this.leaving)
    {
      this.busMotion.X -= 0.075f;
      if ((double) this.busPosition.X + 512.0 < 0.0)
      {
        this.leaving = true;
        Game1.globalFadeToBlack(new Game1.afterFadeFunction(this.busLeftToValley), 0.01f);
      }
    }
    if (this.drivingBack && this.busMotion != Vector2.Zero)
    {
      Game1.player.Position = this.busDoor.position;
      Game1.player.freezePause = 100;
      if ((double) this.busPosition.X - 1088.0 < 256.0)
        this.busMotion.X = Math.Min(-1f, this.busMotion.X * 0.98f);
      if ((double) Math.Abs(this.busPosition.X - 1088f) <= (double) Math.Abs(this.busMotion.X * 1.5f))
      {
        this.busPosition.X = 1088f;
        this.busMotion = Vector2.Zero;
        Game1.globalFadeToBlack((Game1.afterFadeFunction) (() =>
        {
          this.drivingBack = false;
          this.busDoor.Position = this.busPosition + new Vector2(16f, 26f) * 4f;
          this.busDoor.pingPong = true;
          this.busDoor.interval = 70f;
          this.busDoor.currentParentTileIndex = 5;
          this.busDoor.endFunction = new TemporaryAnimatedSprite.endBehavior(this.doorOpenAfterReturn);
          this.localSound("trashcanlid");
          Game1.globalFadeToClear();
        }));
      }
    }
    if (!this.busMotion.Equals(Vector2.Zero))
    {
      this.busPosition += this.busMotion;
      if (this.busDoor != null)
        this.busDoor.Position += this.busMotion;
    }
    this.busDoor?.update(time);
    if (!this.IsTravelingDesertMerchantHere())
      return;
    this.chimneyTimer -= time.ElapsedGameTime.Milliseconds;
    if (this.chimneyTimer > 0)
      return;
    this.chimneyTimer = 500;
    this.temporarySprites.Add(new TemporaryAnimatedSprite("LooseSprites\\Cursors", new Microsoft.Xna.Framework.Rectangle(372, 1956, 10, 10), new Vector2(670f, 308f) * 4f, false, 1f / 500f, new Color((int) byte.MaxValue, 222, 198))
    {
      alpha = 0.05f,
      alphaFade = -0.01f,
      alphaFadeFade = -8E-05f,
      motion = new Vector2(0.0f, -0.5f),
      acceleration = new Vector2(1f / 500f, 0.0f),
      interval = 99999f,
      layerDepth = 1f,
      scale = 3f,
      scaleChange = 0.01f,
      rotationChange = (float) ((double) Game1.random.Next(-5, 6) * 3.1415927410125732 / 256.0),
      drawAboveAlwaysFront = this is DesertFestival
    });
  }

  public override void DayUpdate(int dayOfMonth)
  {
    base.DayUpdate(dayOfMonth);
    this.removeObjectsAndSpawned(33, 20, 13, 6);
  }

  public override bool isTilePlaceable(Vector2 v, bool itemIsPassable = false)
  {
    return ((double) v.X < 33.0 || (double) v.X >= 46.0 || (double) v.Y < 20.0 || (double) v.Y >= 25.0) && base.isTilePlaceable(v, itemIsPassable);
  }

  public override bool shouldHideCharacters() => this.drivingOff || this.drivingBack;

  public override void draw(SpriteBatch spriteBatch)
  {
    base.draw(spriteBatch);
    spriteBatch.Draw(Game1.mouseCursors, Game1.GlobalToLocal(Game1.viewport, new Vector2((float) (int) this.busPosition.X, (float) (int) this.busPosition.Y)), new Microsoft.Xna.Framework.Rectangle?(this.busSource), Color.White, 0.0f, Vector2.Zero, 4f, SpriteEffects.None, (float) (((double) this.busPosition.Y + 192.0) / 10000.0));
    this.busDoor?.draw(spriteBatch);
    if (!this.drivingOff && !this.drivingBack)
      return;
    if (Game1.netWorldState.Value.canDriveYourselfToday.Value || this.drivingOff && Desert.warpedToDesert)
    {
      Game1.player.faceDirection(3);
      Game1.player.blinkTimer = -1000;
      Game1.player.FarmerRenderer.draw(spriteBatch, new FarmerSprite.AnimationFrame(117, 99999, 0, false, true), 117, new Microsoft.Xna.Framework.Rectangle(48 /*0x30*/, 608, 16 /*0x10*/, 32 /*0x20*/), Game1.GlobalToLocal(new Vector2((float) (int) ((double) this.busPosition.X + 4.0), (float) (int) ((double) this.busPosition.Y - 8.0)) + this.pamOffset * 4f), Vector2.Zero, (float) (((double) this.busPosition.Y + 192.0 + 4.0) / 10000.0), Color.White, 0.0f, 1f, Game1.player);
      spriteBatch.Draw(Game1.mouseCursors2, Game1.GlobalToLocal(Game1.viewport, new Vector2((float) (int) this.busPosition.X, (float) ((int) this.busPosition.Y - 40)) + this.pamOffset * 4f), new Microsoft.Xna.Framework.Rectangle?(this.transparentWindowSource), Color.White, 0.0f, Vector2.Zero, 4f, SpriteEffects.None, (float) (((double) this.busPosition.Y + 192.0 + 8.0) / 10000.0));
    }
    else
      spriteBatch.Draw(Game1.mouseCursors, Game1.GlobalToLocal(Game1.viewport, new Vector2((float) (int) this.busPosition.X, (float) (int) this.busPosition.Y) + this.pamOffset * 4f), new Microsoft.Xna.Framework.Rectangle?(this.pamSource), Color.White, 0.0f, Vector2.Zero, 4f, SpriteEffects.None, (float) (((double) this.busPosition.Y + 192.0 + 4.0) / 10000.0));
  }
}
