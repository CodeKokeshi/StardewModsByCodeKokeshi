// Decompiled with JetBrains decompiler
// Type: StardewValley.Characters.Junimo
// Assembly: Stardew Valley, Version=1.6.15.24356, Culture=neutral, PublicKeyToken=null
// MVID: AA1F513A-94F0-4EF6-A35F-2D5B4A3721BF
// Assembly location: D:\SteamLibrary\steamapps\common\Stardew Valley\Stardew Valley.dll
// XML documentation location: D:\SteamLibrary\steamapps\common\Stardew Valley\Stardew Valley.xml

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Netcode;
using StardewValley.BellsAndWhistles;
using StardewValley.Extensions;
using StardewValley.Locations;
using StardewValley.Pathfinding;
using System;
using System.Xml.Serialization;

#nullable disable
namespace StardewValley.Characters;

public class Junimo : NPC
{
  private readonly NetFloat alpha = new NetFloat(1f);
  private readonly NetFloat alphaChange = new NetFloat();
  public readonly NetInt whichArea = new NetInt();
  public readonly NetBool friendly = new NetBool();
  public readonly NetBool holdingStar = new NetBool();
  public readonly NetBool holdingBundle = new NetBool();
  public readonly NetBool temporaryJunimo = new NetBool();
  public readonly NetBool stayPut = new NetBool();
  private readonly NetVector2 motion = new NetVector2(Vector2.Zero);
  private readonly NetRectangle nextPosition = new NetRectangle();
  private readonly NetColor color = new NetColor();
  private readonly NetColor bundleColor = new NetColor();
  private readonly NetBool sayingGoodbye = new NetBool();
  private readonly NetEvent0 setReturnToJunimoHutToFetchStarControllerEvent = new NetEvent0();
  private readonly NetEvent0 setBringBundleBackToHutControllerEvent = new NetEvent0();
  private readonly NetEvent0 setJunimoReachedHutToFetchStarControllerEvent = new NetEvent0();
  private readonly NetEvent0 starDoneSpinningEvent = new NetEvent0();
  private readonly NetEvent0 returnToJunimoHutToFetchFinalStarEvent = new NetEvent0();
  private int farmerCloseCheckTimer = 100;
  private static int soundTimer;

  /// <inheritdoc />
  [XmlIgnore]
  public override bool IsVillager => false;

  public Junimo() => this.forceUpdateTimer = 9999;

  public Junimo(Vector2 position, int whichArea, bool temporary = false)
    : base(new AnimatedSprite("Characters\\Junimo", 0, 16 /*0x10*/, 16 /*0x10*/), position, 2, nameof (Junimo))
  {
    this.whichArea.Value = whichArea;
    try
    {
      this.friendly.Value = Game1.RequireLocation<CommunityCenter>("CommunityCenter").areasComplete[whichArea];
    }
    catch (Exception ex)
    {
      this.friendly.Value = true;
    }
    if (whichArea == 6)
      this.friendly.Value = false;
    this.temporaryJunimo.Value = temporary;
    this.nextPosition.Value = this.GetBoundingBox();
    this.Breather = false;
    this.speed = 3;
    this.forceUpdateTimer = 9999;
    this.collidesWithOtherCharacters.Value = true;
    this.farmerPassesThrough = true;
    this.Scale = 0.75f;
    if (this.temporaryJunimo.Value)
    {
      if (Game1.random.NextDouble() < 0.01)
      {
        switch (Game1.random.Next(8))
        {
          case 0:
            this.color.Value = Color.Red;
            break;
          case 1:
            this.color.Value = Color.Goldenrod;
            break;
          case 2:
            this.color.Value = Color.Yellow;
            break;
          case 3:
            this.color.Value = Color.Lime;
            break;
          case 4:
            this.color.Value = new Color(0, (int) byte.MaxValue, 180);
            break;
          case 5:
            this.color.Value = new Color(0, 100, (int) byte.MaxValue);
            break;
          case 6:
            this.color.Value = Color.MediumPurple;
            break;
          case 7:
            this.color.Value = Color.Salmon;
            break;
        }
        if (Game1.random.NextDouble() >= 0.01)
          return;
        this.color.Value = Color.White;
      }
      else
      {
        switch (Game1.random.Next(8))
        {
          case 0:
            this.color.Value = Color.LimeGreen;
            break;
          case 1:
            this.color.Value = Color.Orange;
            break;
          case 2:
            this.color.Value = Color.LightGreen;
            break;
          case 3:
            this.color.Value = Color.Tan;
            break;
          case 4:
            this.color.Value = Color.GreenYellow;
            break;
          case 5:
            this.color.Value = Color.LawnGreen;
            break;
          case 6:
            this.color.Value = Color.PaleGreen;
            break;
          case 7:
            this.color.Value = Color.Turquoise;
            break;
        }
      }
    }
    else
    {
      switch (whichArea)
      {
        case -1:
        case 0:
          this.color.Value = Color.LimeGreen;
          break;
        case 1:
          this.color.Value = Color.Orange;
          break;
        case 2:
          this.color.Value = Color.Turquoise;
          break;
        case 3:
          this.color.Value = Color.Tan;
          break;
        case 4:
          this.color.Value = Color.Gold;
          break;
        case 5:
          this.color.Value = Color.BlanchedAlmond;
          break;
        case 6:
          this.color.Value = new Color(160 /*0xA0*/, 20, 220);
          break;
      }
    }
  }

  protected override void initNetFields()
  {
    base.initNetFields();
    this.NetFields.AddField((INetSerializable) this.alpha, "alpha").AddField((INetSerializable) this.alphaChange, "alphaChange").AddField((INetSerializable) this.whichArea, "whichArea").AddField((INetSerializable) this.friendly, "friendly").AddField((INetSerializable) this.holdingStar, "holdingStar").AddField((INetSerializable) this.holdingBundle, "holdingBundle").AddField((INetSerializable) this.temporaryJunimo, "temporaryJunimo").AddField((INetSerializable) this.stayPut, "stayPut").AddField((INetSerializable) this.motion, "motion").AddField((INetSerializable) this.nextPosition, "nextPosition").AddField((INetSerializable) this.color, "color").AddField((INetSerializable) this.bundleColor, "bundleColor").AddField((INetSerializable) this.sayingGoodbye, "sayingGoodbye").AddField((INetSerializable) this.setReturnToJunimoHutToFetchStarControllerEvent, "setReturnToJunimoHutToFetchStarControllerEvent").AddField((INetSerializable) this.setBringBundleBackToHutControllerEvent, "setBringBundleBackToHutControllerEvent").AddField((INetSerializable) this.setJunimoReachedHutToFetchStarControllerEvent, "setJunimoReachedHutToFetchStarControllerEvent").AddField((INetSerializable) this.starDoneSpinningEvent, "starDoneSpinningEvent").AddField((INetSerializable) this.returnToJunimoHutToFetchFinalStarEvent, "returnToJunimoHutToFetchFinalStarEvent");
    this.setReturnToJunimoHutToFetchStarControllerEvent.onEvent += new NetEvent0.Event(this.setReturnToJunimoHutToFetchStarController);
    this.setBringBundleBackToHutControllerEvent.onEvent += new NetEvent0.Event(this.setBringBundleBackToHutController);
    this.setJunimoReachedHutToFetchStarControllerEvent.onEvent += new NetEvent0.Event(this.setJunimoReachedHutToFetchStarController);
    this.starDoneSpinningEvent.onEvent += new NetEvent0.Event(this.performStartDoneSpinning);
    this.returnToJunimoHutToFetchFinalStarEvent.onEvent += new NetEvent0.Event(this.returnToJunimoHutToFetchFinalStar);
    this.position.Field.AxisAlignedMovement = false;
  }

  public override bool canPassThroughActionTiles() => false;

  public override bool shouldCollideWithBuildingLayer(GameLocation location) => true;

  public override bool canTalk() => false;

  public override void ChooseAppearance(LocalizedContentManager content = null)
  {
  }

  public void fadeAway()
  {
    this.collidesWithOtherCharacters.Value = false;
    this.alphaChange.Value = this.stayPut.Value ? -0.005f : -0.015f;
  }

  public void setAlpha(float a) => this.alpha.Value = a;

  public void fadeBack()
  {
    this.alpha.Value = 0.0f;
    this.alphaChange.Value = 0.02f;
    this.IsInvisible = false;
  }

  public void setMoving(int xSpeed, int ySpeed)
  {
    this.motion.X = (float) xSpeed;
    this.motion.Y = (float) ySpeed;
  }

  public void setMoving(Vector2 motion) => this.motion.Value = motion;

  public override void Halt()
  {
    base.Halt();
    this.motion.Value = Vector2.Zero;
  }

  public void returnToJunimoHut(GameLocation location)
  {
    this.currentLocation = location;
    this.jump();
    this.collidesWithOtherCharacters.Value = false;
    this.controller = new PathFindController((Character) this, location, new Point(25, 10), 0, new PathFindController.endBehavior(this.junimoReachedHut));
    location.playSound("junimoMeep1");
  }

  public void stayStill()
  {
    this.stayPut.Value = true;
    this.motion.Value = Vector2.Zero;
  }

  public void allowToMoveAgain() => this.stayPut.Value = false;

  private void returnToJunimoHutToFetchFinalStar()
  {
    if (this.currentLocation != Game1.currentLocation)
      return;
    Game1.globalFadeToBlack(new Game1.afterFadeFunction(this.finalCutscene), 0.005f);
    Game1.freezeControls = true;
    Game1.flashAlpha = 1f;
  }

  public void returnToJunimoHutToFetchStar(GameLocation location)
  {
    this.currentLocation = location;
    this.friendly.Value = true;
    CommunityCenter other = Game1.RequireLocation<CommunityCenter>("CommunityCenter");
    if (other.areAllAreasComplete())
    {
      this.returnToJunimoHutToFetchFinalStarEvent.Fire();
      this.collidesWithOtherCharacters.Value = false;
      this.farmerPassesThrough = false;
      this.stayStill();
      this.faceDirection(0);
      Game1.player.mailReceived.Add("ccIsComplete");
      if (!Game1.currentLocation.Equals((GameLocation) other))
        return;
      other.addStarToPlaque();
    }
    else
    {
      DelayedAction.textAboveHeadAfterDelay(Game1.random.NextBool() ? Game1.content.LoadString("Strings\\Characters:JunimoTextAboveHead1") : Game1.content.LoadString("Strings\\Characters:JunimoTextAboveHead2"), (NPC) this, Game1.random.Next(3000, 6000));
      this.setReturnToJunimoHutToFetchStarControllerEvent.Fire();
      location.playSound("junimoMeep1");
      this.collidesWithOtherCharacters.Value = false;
      this.farmerPassesThrough = false;
      this.holdingBundle.Value = true;
      this.speed = 3;
    }
  }

  private void setReturnToJunimoHutToFetchStarController()
  {
    if (!Game1.IsMasterGame)
      return;
    this.controller = new PathFindController((Character) this, this.currentLocation, new Point(25, 10), 0, new PathFindController.endBehavior(this.junimoReachedHutToFetchStar));
  }

  private void finalCutscene()
  {
    this.collidesWithOtherCharacters.Value = false;
    this.farmerPassesThrough = false;
    Game1.RequireLocation<CommunityCenter>("CommunityCenter").prepareForJunimoDance();
    Game1.player.Position = new Vector2(29f, 11f) * 64f;
    Game1.player.completelyStopAnimatingOrDoingAction();
    Game1.player.faceDirection(3);
    Point standingPixel = Game1.player.StandingPixel;
    Game1.UpdateViewPort(true, standingPixel);
    Game1.viewport.X = standingPixel.X - Game1.viewport.Width / 2;
    Game1.viewport.Y = standingPixel.Y - Game1.viewport.Height / 2;
    Game1.viewportTarget = Vector2.Zero;
    Game1.viewportCenter = standingPixel;
    Game1.moveViewportTo(new Vector2(32.5f, 6f) * 64f, 2f, 999999);
    Game1.globalFadeToClear(new Game1.afterFadeFunction(this.goodbyeDance), 0.005f);
    Game1.pauseTime = 1000f;
    Game1.freezeControls = true;
  }

  public void bringBundleBackToHut(Color bundleColor, GameLocation location)
  {
    this.currentLocation = location;
    if (this.holdingBundle.Value)
      return;
    this.Position = Utility.getRandomAdjacentOpenTile(Game1.player.Tile, location) * 64f;
    int num;
    for (num = 0; location.isCollidingPosition(this.GetBoundingBox(), Game1.viewport, (Character) this) && num < 5; ++num)
      this.Position = Utility.getRandomAdjacentOpenTile(Game1.player.Tile, location) * 64f;
    if (num >= 5)
      return;
    if (Game1.random.NextDouble() < 0.25)
      DelayedAction.textAboveHeadAfterDelay(Game1.random.NextBool() ? Game1.content.LoadString("Strings\\Characters:JunimoThankYou1") : Game1.content.LoadString("Strings\\Characters:JunimoThankYou2"), (NPC) this, Game1.random.Next(3000, 6000));
    this.fadeBack();
    this.bundleColor.Value = bundleColor;
    this.setBringBundleBackToHutControllerEvent.Fire();
    this.collidesWithOtherCharacters.Value = false;
    this.farmerPassesThrough = false;
    this.holdingBundle.Value = true;
    this.speed = 1;
  }

  private void setBringBundleBackToHutController()
  {
    if (!Game1.IsMasterGame)
      return;
    this.controller = new PathFindController((Character) this, this.currentLocation, new Point(25, 10), 0, new PathFindController.endBehavior(this.junimoReachedHutToReturnBundle));
  }

  private void junimoReachedHutToReturnBundle(Character c, GameLocation l)
  {
    this.currentLocation = l;
    this.holdingBundle.Value = false;
    this.collidesWithOtherCharacters.Value = true;
    this.farmerPassesThrough = true;
    l.playSound("Ship");
  }

  private void junimoReachedHutToFetchStar(Character c, GameLocation l)
  {
    this.currentLocation = l;
    this.holdingStar.Value = true;
    this.holdingBundle.Value = false;
    this.speed = 1;
    this.collidesWithOtherCharacters.Value = false;
    this.farmerPassesThrough = false;
    this.setJunimoReachedHutToFetchStarControllerEvent.Fire();
    l.playSound("dwop");
    this.farmerPassesThrough = false;
  }

  private void setJunimoReachedHutToFetchStarController()
  {
    if (!Game1.IsMasterGame)
      return;
    this.controller = new PathFindController((Character) this, this.currentLocation, new Point(32 /*0x20*/, 9), 2, new PathFindController.endBehavior(this.placeStar));
  }

  private void placeStar(Character c, GameLocation l)
  {
    this.currentLocation = l;
    this.collidesWithOtherCharacters.Value = false;
    this.farmerPassesThrough = true;
    this.holdingStar.Value = false;
    l.playSound("tinyWhip");
    this.friendly.Value = true;
    this.speed = 3;
    Game1.multiplayer.broadcastSprites(l, new TemporaryAnimatedSprite(this.Sprite.textureName.Value, new Rectangle(0, 109, 16 /*0x10*/, 19), 40f, 8, 10, this.Position + new Vector2(0.0f, -64f), false, false, 1f, 0.0f, Color.White, 4f * this.scale.Value, 0.0f, 0.0f, 0.0f)
    {
      endFunction = new TemporaryAnimatedSprite.endBehavior(this.starDoneSpinning),
      motion = new Vector2(0.22f, -2f),
      acceleration = new Vector2(0.0f, 0.01f),
      id = 777
    });
  }

  public void sayGoodbye()
  {
    this.sayingGoodbye.Value = true;
    this.farmerPassesThrough = true;
  }

  private void goodbyeDance()
  {
    Game1.player.faceDirection(3);
    Game1.RequireLocation<CommunityCenter>("CommunityCenter").junimoGoodbyeDance();
  }

  private void starDoneSpinning(int extraInfo)
  {
    this.starDoneSpinningEvent.Fire();
    (this.currentLocation as CommunityCenter).addStarToPlaque();
  }

  private void performStartDoneSpinning()
  {
    if (!(Game1.currentLocation is CommunityCenter))
      return;
    Game1.playSound("yoba");
    Game1.flashAlpha = 1f;
    Game1.playSound("yoba");
  }

  public override void drawAboveAlwaysFrontLayer(SpriteBatch b)
  {
    if (this.textAboveHeadTimer <= 0 || this.textAboveHead == null)
      return;
    Point standingPixel = this.StandingPixel;
    Vector2 local = Game1.GlobalToLocal(new Vector2((float) standingPixel.X, (float) standingPixel.Y - 128f + (float) this.yJumpOffset));
    if (this.textAboveHeadStyle == 0)
      local += new Vector2((float) Game1.random.Next(-1, 2), (float) Game1.random.Next(-1, 2));
    SpriteText.drawStringWithScrollCenteredAt(b, this.textAboveHead, (int) local.X, (int) local.Y, alpha: this.textAboveHeadAlpha, color: this.textAboveHeadColor, scrollType: 1, layerDepth: (float) ((double) (this.TilePoint.Y * 64 /*0x40*/) / 10000.0 + 1.0 / 1000.0 + (double) this.TilePoint.X / 10000.0), junimoText: !this.sayingGoodbye.Value);
  }

  public void junimoReachedHut(Character c, GameLocation l)
  {
    this.currentLocation = l;
    this.fadeAway();
    this.controller = (PathFindController) null;
    this.motion.X = 0.0f;
    this.motion.Y = -1f;
  }

  protected override void updateSlaveAnimation(GameTime time)
  {
    if (this.sayingGoodbye.Value || this.temporaryJunimo.Value)
      return;
    if (this.holdingStar.Value || this.holdingBundle.Value)
      this.Sprite.Animate(time, 44, 4, 200f);
    else if (this.position.IsInterpolating())
    {
      switch (this.FacingDirection)
      {
        case 0:
          this.Sprite.Animate(time, 32 /*0x20*/, 8, 50f);
          break;
        case 1:
          this.flip = false;
          this.Sprite.Animate(time, 16 /*0x10*/, 8, 50f);
          break;
        case 3:
          this.flip = true;
          this.Sprite.Animate(time, 16 /*0x10*/, 8, 50f);
          break;
        default:
          this.Sprite.Animate(time, 0, 8, 50f);
          break;
      }
    }
    else
      this.Sprite.Animate(time, 8, 4, 100f);
  }

  public override void update(GameTime time, GameLocation location)
  {
    this.currentLocation = location;
    this.setReturnToJunimoHutToFetchStarControllerEvent.Poll();
    this.setBringBundleBackToHutControllerEvent.Poll();
    this.setJunimoReachedHutToFetchStarControllerEvent.Poll();
    this.starDoneSpinningEvent.Poll();
    this.returnToJunimoHutToFetchFinalStarEvent.Poll();
    base.update(time, location);
    this.forceUpdateTimer = 99999;
    if (this.sayingGoodbye.Value)
    {
      this.flip = false;
      if (this.whichArea.Value % 2 == 0)
        this.Sprite.Animate(time, 16 /*0x10*/, 8, 50f);
      else
        this.Sprite.Animate(time, 28, 4, 80f);
      if (!this.IsInvisible && Game1.random.NextDouble() < 0.0099999997764825821 && this.yJumpOffset == 0)
      {
        this.jump();
        if (Game1.random.NextDouble() < 0.15 && (double) Game1.player.Tile.X == 29.0 && (double) Game1.player.Tile.Y == 11.0)
          this.showTextAboveHead(Game1.random.NextBool() ? Game1.content.LoadString("Strings\\StringsFromCSFiles:Junimo.cs.6625") : Game1.content.LoadString("Strings\\StringsFromCSFiles:Junimo.cs.6626"));
      }
      this.alpha.Value += this.alphaChange.Value;
      if ((double) this.alpha.Value > 1.0)
      {
        this.alpha.Value = 1f;
        this.alphaChange.Value = 0.0f;
      }
      if ((double) this.alpha.Value >= 0.0)
        return;
      this.alpha.Value = 0.0f;
      this.IsInvisible = true;
      this.HideShadow = true;
    }
    else if (this.temporaryJunimo.Value)
    {
      this.Sprite.Animate(time, 12, 4, 100f);
      if (Game1.random.NextDouble() >= 0.001)
        return;
      this.jumpWithoutSound();
      location.localSound("junimoMeep1");
    }
    else
    {
      if (this.EventActor)
        return;
      this.alpha.Value += this.alphaChange.Value;
      if ((double) this.alpha.Value > 1.0)
      {
        this.alpha.Value = 1f;
        this.HideShadow = false;
      }
      else if ((double) this.alpha.Value < 0.0)
      {
        this.alpha.Value = 0.0f;
        this.IsInvisible = true;
        this.HideShadow = true;
      }
      --Junimo.soundTimer;
      this.farmerCloseCheckTimer -= time.ElapsedGameTime.Milliseconds;
      if (this.sayingGoodbye.Value || this.temporaryJunimo.Value || !Game1.IsMasterGame)
        return;
      if (!this.IsInvisible && this.farmerCloseCheckTimer <= 0 && this.controller == null && (double) this.alpha.Value >= 1.0 && !this.stayPut.Value && Game1.IsMasterGame)
      {
        this.farmerCloseCheckTimer = 100;
        if (this.holdingStar.Value)
        {
          this.setJunimoReachedHutToFetchStarController();
        }
        else
        {
          Farmer farmer = Utility.isThereAFarmerWithinDistance(this.Tile, 5, this.currentLocation);
          if (farmer != null)
          {
            if (this.friendly.Value && (double) Vector2.Distance(this.Position, farmer.Position) > (double) (this.speed * 4))
            {
              if (this.motion.Equals((object) Vector2.Zero) && Junimo.soundTimer <= 0)
              {
                this.jump();
                location.localSound("junimoMeep1");
                Junimo.soundTimer = 400;
              }
              if (Game1.random.NextDouble() < 0.007)
                this.jumpWithoutSound((float) Game1.random.Next(6, 9));
              this.setMoving(Utility.getVelocityTowardPlayer(new Point((int) this.Position.X, (int) this.Position.Y), (float) this.speed, farmer));
            }
            else if (!this.friendly.Value)
            {
              this.fadeAway();
              Vector2 playerTrajectory = Utility.getAwayFromPlayerTrajectory(this.GetBoundingBox(), farmer);
              playerTrajectory.Normalize();
              playerTrajectory.Y *= -1f;
              this.setMoving(playerTrajectory * (float) this.speed);
            }
            else if ((double) this.alpha.Value >= 1.0)
              this.motion.Value = Vector2.Zero;
          }
          else if ((double) this.alpha.Value >= 1.0)
            this.motion.Value = Vector2.Zero;
        }
      }
      if (!this.IsInvisible && this.controller == null)
      {
        this.nextPosition.Value = this.GetBoundingBox();
        this.nextPosition.X += (int) this.motion.X;
        bool flag = false;
        if (!location.isCollidingPosition(this.nextPosition.Value, Game1.viewport, (Character) this))
        {
          this.position.X += (float) (int) this.motion.X;
          flag = true;
        }
        this.nextPosition.X -= (int) this.motion.X;
        this.nextPosition.Y += (int) this.motion.Y;
        if (!location.isCollidingPosition(this.nextPosition.Value, Game1.viewport, (Character) this))
        {
          this.position.Y += (float) (int) this.motion.Y;
          flag = true;
        }
        if (!this.motion.Equals((object) Vector2.Zero) & flag && Game1.random.NextDouble() < 0.005)
          location.temporarySprites.Add(new TemporaryAnimatedSprite(Game1.random.Choose<int>(10, 11), this.Position, this.color.Value)
          {
            motion = this.motion.Value / 4f,
            alphaFade = 0.01f,
            layerDepth = 0.8f,
            scale = 0.75f,
            alpha = 0.75f
          });
      }
      if (this.controller != null || !this.motion.Equals((object) Vector2.Zero))
      {
        if (this.holdingStar.Value || this.holdingBundle.Value)
          this.Sprite.Animate(time, 44, 4, 200f);
        else if (this.moveRight || (double) Math.Abs(this.motion.X) > (double) Math.Abs(this.motion.Y) && (double) this.motion.X > 0.0)
        {
          this.flip = false;
          this.Sprite.Animate(time, 16 /*0x10*/, 8, 50f);
        }
        else if (this.moveLeft || (double) Math.Abs(this.motion.X) > (double) Math.Abs(this.motion.Y) && (double) this.motion.X < 0.0)
        {
          this.Sprite.Animate(time, 16 /*0x10*/, 8, 50f);
          this.flip = true;
        }
        else if (this.moveUp || (double) Math.Abs(this.motion.Y) > (double) Math.Abs(this.motion.X) && (double) this.motion.Y < 0.0)
          this.Sprite.Animate(time, 32 /*0x20*/, 8, 50f);
        else
          this.Sprite.Animate(time, 0, 8, 50f);
      }
      else
        this.Sprite.Animate(time, 8, 4, 100f);
    }
  }

  public override void draw(SpriteBatch b, float alpha = 1f)
  {
    if (this.IsInvisible)
      return;
    this.Sprite.UpdateSourceRect();
    b.Draw(this.Sprite.Texture, this.getLocalPosition(Game1.viewport) + new Vector2((float) (this.Sprite.SpriteWidth * 4 / 2), (float) ((double) this.Sprite.SpriteHeight * 3.0 / 4.0 * 4.0 / Math.Pow((double) (this.Sprite.SpriteHeight / 16 /*0x10*/), 2.0) + (double) this.yJumpOffset - 8.0)) + (this.shakeTimer > 0 ? new Vector2((float) Game1.random.Next(-1, 2), (float) Game1.random.Next(-1, 2)) : Vector2.Zero), new Rectangle?(this.Sprite.SourceRect), this.color.Value * this.alpha.Value, this.rotation, new Vector2((float) (this.Sprite.SpriteWidth * 4 / 2), (float) ((double) (this.Sprite.SpriteHeight * 4) * 3.0 / 4.0)) / 4f, Math.Max(0.2f, this.scale.Value) * 4f, this.flip ? SpriteEffects.FlipHorizontally : SpriteEffects.None, Math.Max(0.0f, this.drawOnTop ? 0.991f : (float) this.StandingPixel.Y / 10000f));
    if (this.holdingStar.Value)
    {
      b.Draw(this.Sprite.Texture, Game1.GlobalToLocal(Game1.viewport, this.Position + new Vector2(8f, (float) (-64.0 * (double) this.scale.Value + 4.0) + (float) this.yJumpOffset)), new Rectangle?(new Rectangle(0, 109, 16 /*0x10*/, 19)), Color.White * this.alpha.Value, 0.0f, Vector2.Zero, 4f * this.scale.Value, SpriteEffects.None, (float) ((double) this.Position.Y / 10000.0 + 9.9999997473787516E-05));
    }
    else
    {
      if (!this.holdingBundle.Value)
        return;
      b.Draw(this.Sprite.Texture, Game1.GlobalToLocal(Game1.viewport, this.Position + new Vector2(8f, (float) (-64.0 * (double) this.scale.Value + 20.0) + (float) this.yJumpOffset)), new Rectangle?(new Rectangle(0, 96 /*0x60*/, 16 /*0x10*/, 13)), this.bundleColor.Value * this.alpha.Value, 0.0f, Vector2.Zero, 4f * this.scale.Value, SpriteEffects.None, (float) ((double) this.Position.Y / 10000.0 + 9.9999997473787516E-05));
    }
  }

  /// <inheritdoc />
  public override void DrawShadow(SpriteBatch b)
  {
    SpriteBatch spriteBatch = b;
    Texture2D shadowTexture = Game1.shadowTexture;
    Vector2 local = Game1.GlobalToLocal(Game1.viewport, this.Position + new Vector2((float) (this.Sprite.SpriteWidth * 4) / 2f, 44f));
    Rectangle? sourceRectangle = new Rectangle?(Game1.shadowTexture.Bounds);
    Color color = this.color.Value * this.alpha.Value;
    Rectangle bounds = Game1.shadowTexture.Bounds;
    double x = (double) bounds.Center.X;
    bounds = Game1.shadowTexture.Bounds;
    double y = (double) bounds.Center.Y;
    Vector2 origin = new Vector2((float) x, (float) y);
    double scale = (4.0 + (double) this.yJumpOffset / 40.0) * (double) this.scale.Value;
    double layerDepth = (double) Math.Max(0.0f, (float) this.StandingPixel.Y / 10000f) - 9.9999999747524271E-07;
    spriteBatch.Draw(shadowTexture, local, sourceRectangle, color, 0.0f, origin, (float) scale, SpriteEffects.None, (float) layerDepth);
  }
}
