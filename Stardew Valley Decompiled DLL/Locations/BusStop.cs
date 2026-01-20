// Decompiled with JetBrains decompiler
// Type: StardewValley.Locations.BusStop
// Assembly: Stardew Valley, Version=1.6.15.24356, Culture=neutral, PublicKeyToken=null
// MVID: AA1F513A-94F0-4EF6-A35F-2D5B4A3721BF
// Assembly location: D:\SteamLibrary\steamapps\common\Stardew Valley\Stardew Valley.dll
// XML documentation location: D:\SteamLibrary\steamapps\common\Stardew Valley\Stardew Valley.xml

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using StardewValley.Characters;
using StardewValley.Extensions;
using StardewValley.Pathfinding;
using System;
using System.Xml.Serialization;
using xTile.Dimensions;

#nullable disable
namespace StardewValley.Locations;

public class BusStop : GameLocation
{
  public const int busDefaultXTile = 21;
  public const int busDefaultYTile = 6;
  private TemporaryAnimatedSprite minecartSteam;
  private TemporaryAnimatedSprite busDoor;
  [XmlIgnore]
  public Vector2 busPosition;
  [XmlIgnore]
  public Vector2 busMotion;
  [XmlIgnore]
  public bool drivingOff;
  [XmlIgnore]
  public bool drivingBack;
  [XmlIgnore]
  public bool leaving;
  private int forceWarpTimer;
  private Microsoft.Xna.Framework.Rectangle busSource = new Microsoft.Xna.Framework.Rectangle(288, 1247, 128 /*0x80*/, 64 /*0x40*/);
  private Microsoft.Xna.Framework.Rectangle pamSource = new Microsoft.Xna.Framework.Rectangle(384, 1311, 15, 19);
  private Vector2 pamOffset = new Vector2(0.0f, 29f);

  /// <summary>The gold price to buy a ticket on the bus.</summary>
  [XmlIgnore]
  public int TicketPrice { get; set; } = 500;

  public BusStop()
  {
  }

  public BusStop(string mapPath, string name)
    : base(mapPath, name)
  {
    this.busPosition = new Vector2(21f, 6f) * 64f;
  }

  /// <inheritdoc />
  public override bool IgnoreTouchActions()
  {
    return base.IgnoreTouchActions() || this.drivingBack || this.drivingOff;
  }

  public override bool checkAction(Location tileLocation, xTile.Dimensions.Rectangle viewport, Farmer who)
  {
    switch (this.getTileIndexAt(tileLocation, "Buildings", "outdoors"))
    {
      case 958:
      case 1080:
      case 1081:
        this.ShowMineCartMenu("Default", "Bus");
        return true;
      case 1057:
        if (Game1.MasterPlayer.mailReceived.Contains("ccVault"))
        {
          if (Game1.player.isRidingHorse() && Game1.player.mount != null)
          {
            Game1.player.mount.checkAction(Game1.player, (GameLocation) this);
          }
          else
          {
            string numberWithCommas = Utility.getNumberWithCommas(this.TicketPrice);
            if (Game1.content.GetCurrentLanguage() == LocalizedContentManager.LanguageCode.es)
            {
              this.createQuestionDialogueWithCustomWidth(Game1.content.LoadString("Strings\\Locations:BusStop_BuyTicketToDesert", (object) numberWithCommas), this.createYesNoResponses(), "Bus");
              break;
            }
            this.createQuestionDialogue(Game1.content.LoadString("Strings\\Locations:BusStop_BuyTicketToDesert", (object) numberWithCommas), this.createYesNoResponses(), "Bus");
            break;
          }
        }
        else
          Game1.drawObjectDialogue(Game1.content.LoadString("Strings\\Locations:BusStop_DesertOutOfService"));
        return true;
    }
    return base.checkAction(tileLocation, viewport, who);
  }

  private void playerReachedBusDoor(Character c, GameLocation l)
  {
    this.forceWarpTimer = 0;
    Game1.player.position.X = -10000f;
    Game1.changeMusicTrack("silence");
    this.busDriveOff();
    this.playSound("stoneStep");
    if (Game1.player.mount == null)
      return;
    Game1.player.mount.farmerPassesThrough = false;
  }

  public override void DayUpdate(int dayOfMonth)
  {
    base.DayUpdate(dayOfMonth);
    if (Game1.netWorldState.Value.canDriveYourselfToday.Value && Game1.IsMasterGame)
      Game1.netWorldState.Value.canDriveYourselfToday.Value = false;
    StardewValley.Object objectAtTile = this.getObjectAtTile(25, 10);
    if (objectAtTile == null || objectAtTile.SpecialVariable != 987659)
      return;
    this.objects.Remove(new Vector2(25f, 10f));
  }

  public override bool answerDialogue(Response answer)
  {
    if (this.lastQuestionKey == null || this.afterQuestion != null || !($"{ArgUtility.SplitBySpaceAndGet(this.lastQuestionKey, 0)}_{answer.responseKey}" == "Bus_Yes"))
      return base.answerDialogue(answer);
    NPC characterFromName = Game1.getCharacterFromName("Pam");
    if (!Game1.netWorldState.Value.canDriveYourselfToday.Value && (!this.characters.Contains(characterFromName) || characterFromName.TilePoint.X != 21 || characterFromName.TilePoint.Y != 10))
      Game1.drawObjectDialogue(Game1.content.LoadString("Strings\\Locations:BusStop_NoDriver"));
    else if (Game1.player.Money < this.TicketPrice)
    {
      Game1.drawObjectDialogue(Game1.content.LoadString("Strings\\Locations:BusStop_NotEnoughMoneyForTicket"));
    }
    else
    {
      Game1.player.Money -= this.TicketPrice;
      Game1.freezeControls = true;
      Game1.viewportFreeze = true;
      this.forceWarpTimer = 8000;
      Game1.player.controller = new PathFindController((Character) Game1.player, (GameLocation) this, new Point(22, 9), 0, new PathFindController.endBehavior(this.playerReachedBusDoor));
      Game1.player.setRunning(true);
      if (Game1.player.mount != null)
        Game1.player.mount.farmerPassesThrough = true;
      Desert.warpedToDesert = false;
    }
    return true;
  }

  protected override void resetLocalState()
  {
    base.resetLocalState();
    this.leaving = false;
    if (Game1.MasterPlayer.mailReceived.Contains("ccBoilerRoom"))
      this.minecartSteam = new TemporaryAnimatedSprite(27, new Vector2(1032f, 144f), Color.White)
      {
        totalNumberOfLoops = 999999,
        interval = 60f,
        flipped = true
      };
    if (Game1.getFarm().grandpaScore.Value == 0 && Game1.year >= 3)
      Game1.player.eventsSeen.Remove("558292");
    bool flag = false;
    GameLocation locationFromName = Game1.getLocationFromName(Game1.player.previousLocationName);
    if (locationFromName != null && locationFromName.GetLocationContext() != this.GetLocationContext())
      flag = true;
    if (Game1.player.TilePoint.Y > 16 /*0x10*/ || Game1.eventUp || Game1.player.TilePoint.X <= 10 || Game1.player.isRidingHorse() || !flag)
    {
      this.drivingOff = false;
      this.drivingBack = false;
      this.busMotion = Vector2.Zero;
      this.busPosition = new Vector2(21f, 6f) * 64f;
      this.busDoor = new TemporaryAnimatedSprite("LooseSprites\\Cursors", new Microsoft.Xna.Framework.Rectangle(288, 1311, 16 /*0x10*/, 38), this.busPosition + new Vector2(16f, 26f) * 4f, false, 0.0f, Color.White)
      {
        interval = 999999f,
        animationLength = 6,
        holdLastFrame = true,
        layerDepth = (float) (((double) this.busPosition.Y + 192.0) / 10000.0 + 9.9999997473787516E-06),
        scale = 4f
      };
    }
    else
    {
      Game1.changeMusicTrack("silence");
      this.busPosition = new Vector2(21f, 6f) * 64f;
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
    if (Game1.player.TilePoint.Y <= 16 /*0x10*/ || !Game1.MasterPlayer.mailReceived.Contains("Capsule_Broken") || !Game1.isDarkOut((GameLocation) this) || Game1.random.NextDouble() >= 0.01)
      return;
    this.temporarySprites.Add(new TemporaryAnimatedSprite("LooseSprites\\temporary_sprites_1", new Microsoft.Xna.Framework.Rectangle(448, 546, 16 /*0x10*/, 25), new Vector2(12f, 6.5f) * 64f, true, 0.0f, Color.White)
    {
      scale = 4f,
      motion = new Vector2(-3f, 0.0f),
      animationLength = 4,
      interval = 80f,
      totalNumberOfLoops = 200,
      layerDepth = 0.0448f,
      delayBeforeAnimationStart = Game1.random.Next(1500)
    });
  }

  public override void cleanupBeforePlayerExit()
  {
    base.cleanupBeforePlayerExit();
    if (this.farmers.Count > 1)
      return;
    this.minecartSteam = (TemporaryAnimatedSprite) null;
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
    this.busMotion = new Vector2(-12f, 0.0f);
    Game1.freezeControls = true;
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

  private void doorOpenAfterReturn(int extraInfo)
  {
    this.busDoor = new TemporaryAnimatedSprite("LooseSprites\\Cursors", new Microsoft.Xna.Framework.Rectangle(288, 1311, 16 /*0x10*/, 38), this.busPosition + new Vector2(16f, 26f) * 4f, false, 0.0f, Color.White)
    {
      interval = 999999f,
      animationLength = 6,
      holdLastFrame = true,
      layerDepth = (float) (((double) this.busPosition.Y + 192.0) / 10000.0 + 9.9999997473787516E-06),
      scale = 4f
    };
    Game1.player.Position = new Vector2(22f, 10f) * 64f;
    this.lastTouchActionLocation = Game1.player.Tile;
    Game1.displayFarmer = true;
    Game1.player.forceCanMove();
    Game1.player.faceDirection(2);
    Game1.changeMusicTrack("none", true);
    GameLocation.HandleMusicChange((GameLocation) null, (GameLocation) this);
  }

  private void busLeftToDesert()
  {
    Game1.viewportFreeze = true;
    Game1.warpFarmer("Desert", 16 /*0x10*/, 24, true);
    Game1.globalFade = false;
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
    if (this.forceWarpTimer > 0)
    {
      this.forceWarpTimer -= time.ElapsedGameTime.Milliseconds;
      if (this.forceWarpTimer <= 0)
        this.playerReachedBusDoor((Character) Game1.player, (GameLocation) this);
    }
    this.minecartSteam?.update(time);
    if (this.drivingOff && !this.leaving)
    {
      this.busMotion.X -= 0.075f;
      if ((double) this.busPosition.X + 512.0 < 10.0)
      {
        this.leaving = true;
        this.busLeftToDesert();
      }
    }
    if (this.drivingBack && this.busMotion != Vector2.Zero)
    {
      Game1.player.Position = this.busPosition;
      if ((double) this.busPosition.X - 1344.0 < 512.0)
        this.busMotion.X = Math.Min(-1f, this.busMotion.X * 0.98f);
      if ((double) Math.Abs(this.busPosition.X - 1344f) <= (double) Math.Abs(this.busMotion.X * 1.5f))
      {
        this.busPosition.X = 1344f;
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
          if (!string.IsNullOrEmpty(Game1.player.horseName.Value))
          {
            for (int index = 0; index < this.characters.Count; ++index)
            {
              if (this.characters[index] is Horse character2 && character2.getOwner() == Game1.player)
              {
                if (string.IsNullOrEmpty(this.characters[index].Name))
                {
                  Game1.showGlobalMessage(Game1.content.LoadString("Strings\\Locations:BusStop_ReturnToHorse2", (object) this.characters[index].displayName));
                  break;
                }
                Game1.showGlobalMessage(Game1.content.LoadString("Strings\\Locations:BusStop_ReturnToHorse" + (Game1.random.Next(2) + 1).ToString(), (object) this.characters[index].displayName));
                break;
              }
            }
          }
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
  }

  public override bool shouldHideCharacters() => this.drivingOff || this.drivingBack;

  public override void draw(SpriteBatch spriteBatch)
  {
    base.draw(spriteBatch);
    this.minecartSteam?.draw(spriteBatch);
    spriteBatch.Draw(Game1.mouseCursors, Game1.GlobalToLocal(Game1.viewport, new Vector2((float) (int) this.busPosition.X, (float) (int) this.busPosition.Y)), new Microsoft.Xna.Framework.Rectangle?(this.busSource), Color.White, 0.0f, Vector2.Zero, 4f, SpriteEffects.None, (float) (((double) this.busPosition.Y + 192.0) / 10000.0));
    this.busDoor?.draw(spriteBatch);
    if (Game1.netWorldState.Value.canDriveYourselfToday.Value && (this.drivingOff || this.drivingBack) || this.drivingBack && Desert.warpedToDesert)
    {
      Game1.player.faceDirection(3);
      Game1.player.blinkTimer = -1000;
      Game1.player.FarmerRenderer.draw(spriteBatch, new FarmerSprite.AnimationFrame(117, 99999, 0, false, true), 117, new Microsoft.Xna.Framework.Rectangle(48 /*0x30*/, 608, 16 /*0x10*/, 32 /*0x20*/), Game1.GlobalToLocal(new Vector2((float) (int) ((double) this.busPosition.X + 4.0), (float) (int) ((double) this.busPosition.Y - 8.0)) + this.pamOffset * 4f), Vector2.Zero, (float) (((double) this.busPosition.Y + 192.0 + 4.0) / 10000.0), Color.White, 0.0f, 1f, Game1.player);
      spriteBatch.Draw(Game1.mouseCursors2, Game1.GlobalToLocal(Game1.viewport, new Vector2((float) (int) this.busPosition.X, (float) ((int) this.busPosition.Y - 40)) + this.pamOffset * 4f), new Microsoft.Xna.Framework.Rectangle?(new Microsoft.Xna.Framework.Rectangle(0, 0, 21, 41)), Color.White, 0.0f, Vector2.Zero, 4f, SpriteEffects.None, (float) (((double) this.busPosition.Y + 192.0 + 8.0) / 10000.0));
    }
    else
    {
      if (!this.drivingOff && !this.drivingBack)
        return;
      spriteBatch.Draw(Game1.mouseCursors, Game1.GlobalToLocal(Game1.viewport, new Vector2((float) (int) this.busPosition.X, (float) (int) this.busPosition.Y) + this.pamOffset * 4f), new Microsoft.Xna.Framework.Rectangle?(this.pamSource), Color.White, 0.0f, Vector2.Zero, 4f, SpriteEffects.None, (float) (((double) this.busPosition.Y + 192.0 + 4.0) / 10000.0));
    }
  }
}
