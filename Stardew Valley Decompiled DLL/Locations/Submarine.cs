// Decompiled with JetBrains decompiler
// Type: StardewValley.Locations.Submarine
// Assembly: Stardew Valley, Version=1.6.15.24356, Culture=neutral, PublicKeyToken=null
// MVID: AA1F513A-94F0-4EF6-A35F-2D5B4A3721BF
// Assembly location: D:\SteamLibrary\steamapps\common\Stardew Valley\Stardew Valley.dll
// XML documentation location: D:\SteamLibrary\steamapps\common\Stardew Valley\Stardew Valley.xml

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Netcode;
using StardewValley.Menus;
using StardewValley.Tools;
using System;
using System.Xml.Serialization;
using xTile.Dimensions;

#nullable disable
namespace StardewValley.Locations;

public class Submarine : GameLocation
{
  public const float submergeTime = 20000f;
  /// <summary>The main tile sheet ID for submarine tiles.</summary>
  public const string MainTileSheetId = "submarine tiles";
  [XmlElement("submerged")]
  public readonly NetBool submerged = new NetBool();
  [XmlElement("ascending")]
  public readonly NetBool ascending = new NetBool();
  private Texture2D submarineSprites;
  private float curtainMovement;
  private float curtainOpenPercent;
  private float submergeTimer;
  private Color ambientLightTargetColor;
  private bool hasLitSubmergeLight;
  private bool hasLitAscendLight;
  private bool doneUntilReset;
  private bool localAscending;

  public Submarine()
  {
  }

  public Submarine(string mapPath, string name)
    : base(mapPath, name)
  {
  }

  protected override void initNetFields()
  {
    base.initNetFields();
    this.NetFields.AddField((INetSerializable) this.submerged, "submerged").AddField((INetSerializable) this.ascending, "ascending");
  }

  public override void draw(SpriteBatch b)
  {
    base.draw(b);
    b.Draw(this.submarineSprites, Game1.GlobalToLocal(new Vector2(9f, 7f) * 64f) + new Vector2(0.0f, -2f) * 4f, new Microsoft.Xna.Framework.Rectangle?(new Microsoft.Xna.Framework.Rectangle((int) (257.0 + 100.0 * (double) this.curtainOpenPercent), 0, (int) (100.0 * (1.0 - (double) this.curtainOpenPercent)), 80 /*0x50*/)), Color.White, 0.0f, Vector2.Zero, 4f, SpriteEffects.None, 1f / 1000f);
    b.Draw(this.submarineSprites, Game1.GlobalToLocal(new Vector2(15f, 7f) * 64f + new Vector2(-3f, -2f) * 4f + new Vector2(100f * this.curtainOpenPercent, 0.0f) * 4f), new Microsoft.Xna.Framework.Rectangle?(new Microsoft.Xna.Framework.Rectangle(357, 0, (int) (100.0 * (1.0 - (double) this.curtainOpenPercent)), 80 /*0x50*/)), Color.White, 0.0f, Vector2.Zero, 4f, SpriteEffects.None, 1f / 1000f);
    b.Draw(this.submarineSprites, Game1.GlobalToLocal(new Vector2(82f, 123f) * 4f + new Vector2(0.0f, !this.submerged.Value || this.doneUntilReset ? 0.0f : (float) (104.0 * (1.0 - (double) this.submergeTimer / 20000.0)))), new Microsoft.Xna.Framework.Rectangle?(new Microsoft.Xna.Framework.Rectangle(457, 0, 9, 4)), Color.White, 0.0f, Vector2.Zero, 4f, SpriteEffects.None, 1f / 1000f);
  }

  public override void DayUpdate(int dayOfMonth)
  {
    base.DayUpdate(dayOfMonth);
    this.hasLitSubmergeLight = false;
    this.curtainOpenPercent = 0.0f;
    this.curtainMovement = 0.0f;
    this.submergeTimer = 0.0f;
    this.submerged.Value = false;
    this.hasLitAscendLight = false;
    this.doneUntilReset = false;
    if (this.submerged.Value)
      this.submerged.Value = false;
    if (this.ascending.Value)
      this.ascending.Value = false;
    Game1.netWorldState.Value.IsSubmarineLocked = false;
  }

  public override bool checkAction(Location tileLocation, xTile.Dimensions.Rectangle viewport, Farmer who)
  {
    if (this.getTileIndexAt(tileLocation, "Buildings", "submarine tiles") != 217)
      return base.checkAction(tileLocation, viewport, who);
    if (this.doneUntilReset)
    {
      Game1.drawObjectDialogue(Game1.content.LoadString("Strings\\Locations:Submarine_Done"));
      return false;
    }
    if (!this.submerged.Value)
      this.createQuestionDialogue(Game1.content.LoadString("Strings\\Locations:Submarine_SubmergeQuestion"), this.createYesNoResponses(), "SubmergeQuestion");
    else if ((double) this.submergeTimer <= 0.0 && (double) this.curtainOpenPercent >= 1.0)
      this.createQuestionDialogue(Game1.content.LoadString("Strings\\Locations:Submarine_AscendQuestion"), this.createYesNoResponses(), "AscendQuestion");
    return true;
  }

  public override bool answerDialogueAction(string questionAndAnswer, string[] questionParams)
  {
    switch (questionAndAnswer)
    {
      case null:
        return false;
      case "SubmergeQuestion_Yes":
        if (Game1.player.Money < 1000)
        {
          Game1.drawObjectDialogue(Game1.content.LoadString("Strings\\Locations:BusStop_NotEnoughMoneyForTicket"));
          break;
        }
        Game1.player.Money -= 1000;
        this.submerged.Value = true;
        Game1.netWorldState.Value.IsSubmarineLocked = true;
        break;
      case "AscendQuestion_Yes":
        this.ascending.Value = true;
        this.localAscending = true;
        break;
    }
    return base.answerDialogueAction(questionAndAnswer, questionParams);
  }

  private void changeSubmergeLight(bool red, bool clear = false)
  {
    if (clear)
    {
      this.setMapTile(3, 4, 98, "Buildings", "submarine tiles");
      this.setMapTile(4, 4, 99, "Buildings", "submarine tiles");
      this.setMapTile(3, 5, 122, "Buildings", "submarine tiles");
      this.setMapTile(4, 5, 123, "Buildings", "submarine tiles");
    }
    else if (red)
    {
      this.setMapTile(3, 4, 425, "Buildings", "submarine tiles");
      this.setMapTile(4, 4, 426, "Buildings", "submarine tiles");
      this.setMapTile(3, 5, 449, "Buildings", "submarine tiles");
      this.setMapTile(4, 5, 450, "Buildings", "submarine tiles");
    }
    else
    {
      this.setMapTile(3, 4, 427, "Buildings", "submarine tiles");
      this.setMapTile(4, 4, 428, "Buildings", "submarine tiles");
      this.setMapTile(3, 5, 451, "Buildings", "submarine tiles");
      this.setMapTile(4, 5, 452, "Buildings", "submarine tiles");
    }
  }

  protected override void resetSharedState()
  {
    base.resetSharedState();
    this.submerged.Value = false;
    this.ascending.Value = false;
    Game1.netWorldState.Value.IsSubmarineLocked = false;
  }

  protected override void resetLocalState()
  {
    base.resetLocalState();
    this.submarineSprites = Game1.temporaryContent.Load<Texture2D>("LooseSprites\\temporary_sprites_1");
    Game1.ambientLight = Color.Black;
    this.ambientLightTargetColor = Color.Black;
    this.hasLitSubmergeLight = false;
    Game1.background = new Background((GameLocation) this, new Color(0, 50, (int) byte.MaxValue), true);
    this.curtainOpenPercent = 0.0f;
    this.curtainMovement = 0.0f;
    this.submergeTimer = 0.0f;
    this.hasLitAscendLight = false;
    this.doneUntilReset = false;
    this.localAscending = false;
  }

  public override bool canFishHere() => (double) this.curtainOpenPercent >= 1.0;

  public override void performTenMinuteUpdate(int timeOfDay)
  {
    base.performTenMinuteUpdate(timeOfDay);
    Random daySaveRandom = Utility.CreateDaySaveRandom((double) timeOfDay);
    if (this.fishSplashPoint.Value.Equals(Point.Zero) && daySaveRandom.NextDouble() < 1.0 && (double) this.curtainOpenPercent >= 1.0)
    {
      for (int index = 0; index < 2; ++index)
      {
        Point point = new Point(daySaveRandom.Next(9, 21), daySaveRandom.Next(7, 12));
        if (this.isOpenWater(point.X, point.Y))
        {
          int land = FishingRod.distanceToLand(point.X, point.Y, (GameLocation) this);
          if (land > 1 && land < 5)
          {
            if (Game1.player.currentLocation.Equals((GameLocation) this))
              this.playSound("waterSlosh");
            this.fishSplashPoint.Value = point;
            break;
          }
        }
      }
    }
    else
    {
      if (this.fishSplashPoint.Value.Equals(Point.Zero) || daySaveRandom.NextDouble() >= 0.25)
        return;
      this.fishSplashPoint.Value = Point.Zero;
    }
  }

  public override void UpdateWhenCurrentLocation(GameTime time)
  {
    base.UpdateWhenCurrentLocation(time);
    if (!Game1.player.currentLocation.Equals((GameLocation) this) || !Game1.shouldTimePass())
      return;
    if ((double) this.curtainMovement != 0.0)
    {
      float curtainOpenPercent = this.curtainOpenPercent;
      this.curtainOpenPercent = Math.Max(0.0f, Math.Min(1f, this.curtainOpenPercent + this.curtainMovement * (float) time.ElapsedGameTime.Milliseconds));
      if ((double) this.curtainOpenPercent >= 1.0 && (double) curtainOpenPercent < 1.0)
      {
        this.curtainMovement = 0.0f;
        this.changeSubmergeLight(false);
        this.ambientLightTargetColor = new Color(200, 150, 100);
        Game1.playSound("newArtifact");
        Game1.changeMusicTrack("submarine_song");
      }
    }
    if (this.submerged.Value && !this.hasLitSubmergeLight)
    {
      this.changeSubmergeLight(true);
      DelayedAction.playSoundAfterDelay("cowboy_monsterhit", 200);
      DelayedAction.playSoundAfterDelay("cowboy_monsterhit", 400);
      Game1.changeMusicTrack("Hospital_Ambient");
      this.submergeTimer = 20000f;
      this.hasLitSubmergeLight = true;
      this.ignoreWarps = true;
      this.temporarySprites.Add(new TemporaryAnimatedSprite()
      {
        texture = this.submarineSprites,
        sourceRectStartingPos = new Vector2(457f, 11f),
        sourceRect = new Microsoft.Xna.Framework.Rectangle(457, 11, 14, 18),
        initialPosition = new Vector2(21f, 143f) * 4f,
        animationLength = 3,
        pingPong = true,
        position = new Vector2(21f, 143f) * 4f,
        scale = 4f
      });
    }
    if (this.ascending.Value && !this.hasLitAscendLight)
    {
      this.changeSubmergeLight(true);
      DelayedAction.playSoundAfterDelay("cowboy_monsterhit", 200);
      DelayedAction.playSoundAfterDelay("cowboy_monsterhit", 400);
      Game1.changeMusicTrack("Hospital_Ambient");
      this.submergeTimer = 1f;
      this.hasLitAscendLight = true;
      this.curtainMovement = -0.0002f;
      Game1.playSound("submarine_landing");
      this.temporarySprites.Add(new TemporaryAnimatedSprite()
      {
        texture = this.submarineSprites,
        sourceRectStartingPos = new Vector2(457f, 11f),
        sourceRect = new Microsoft.Xna.Framework.Rectangle(457, 11, 14, 18),
        initialPosition = new Vector2(21f, 143f) * 4f,
        animationLength = 3,
        pingPong = true,
        position = new Vector2(21f, 143f) * 4f,
        scale = 4f
      });
      if (Game1.IsMasterGame)
        this.fishSplashPoint.Value = Point.Zero;
      if (Game1.activeClickableMenu is BobberBar)
        Game1.activeClickableMenu.emergencyShutDown();
      if (Game1.player.UsingTool && Game1.player.CurrentTool is FishingRod currentTool)
        currentTool.doneFishing(Game1.player);
      Game1.player.completelyStopAnimatingOrDoingAction();
      foreach (TemporaryAnimatedSprite tempSprite in Game1.background.tempSprites)
      {
        tempSprite.yStopCoordinate = (double) tempSprite.position.X > 320.0 ? 320 : 896;
        tempSprite.motion = new Vector2(0.0f, 2f);
        tempSprite.yPeriodic = false;
      }
    }
    if ((double) this.submergeTimer > 0.0)
    {
      if (this.ascending.Value && !this.localAscending)
        this.localAscending = true;
      this.submergeTimer -= (float) ((this.localAscending ? -1 : 1) * time.ElapsedGameTime.Milliseconds);
      Game1.background.c.B = (byte) ((double) Math.Max(this.submergeTimer / 20000f, 0.2f) * (double) byte.MaxValue);
      Game1.background.c.G = (byte) ((double) Math.Max(this.submergeTimer / 20000f, 0.0f) * 50.0);
      if ((double) this.submergeTimer <= 0.0)
      {
        this.curtainMovement = 0.0002f;
        Game1.changeMusicTrack("none");
        Game1.playSound("submarine_landing");
        Game1.background.tempSprites.Add(new TemporaryAnimatedSprite()
        {
          motion = new Vector2(0.0f, -1f),
          yStopCoordinate = 120,
          texture = this.submarineSprites,
          sourceRect = new Microsoft.Xna.Framework.Rectangle(257, 98, 182, 25),
          animationLength = 1,
          interval = 999999f,
          position = new Vector2(148f, 56f) * 4f,
          scale = 4f
        });
        Game1.background.tempSprites.Add(new TemporaryAnimatedSprite()
        {
          motion = new Vector2(0.0f, -1f),
          yStopCoordinate = 460,
          texture = this.submarineSprites,
          sourceRect = new Microsoft.Xna.Framework.Rectangle(441, 86, 66, 37),
          animationLength = 1,
          interval = 999999f,
          position = new Vector2(18f, 149f) * 4f,
          scale = 4f
        });
      }
      else
      {
        this.ambientLightTargetColor = new Color((int) (byte) (250.0 - (double) this.submergeTimer / 20000.0 * 250.0), (int) (byte) (200.0 - (double) this.submergeTimer / 20000.0 * 200.0), (int) (byte) (150.0 - (double) this.submergeTimer / 20000.0 * 150.0));
        if (Game1.random.NextDouble() < 0.11)
        {
          Vector2 vector2 = new Vector2((float) Game1.random.Next(12, this.map.DisplayWidth - 64 /*0x40*/), this.ascending.Value ? 1f : 640f);
          int num = Game1.random.Next(3);
          Game1.background.tempSprites.Add(new TemporaryAnimatedSprite()
          {
            motion = new Vector2(0.0f, (float) ((this.ascending.Value ? -1.0 : 1.0) * ((double) num - 3.0))),
            yStopCoordinate = this.ascending.Value ? 832 : 1,
            texture = this.submarineSprites,
            sourceRect = new Microsoft.Xna.Framework.Rectangle(132 + num * 8, 20, 8, 8),
            xPeriodic = true,
            xPeriodicLoopTime = 1500f,
            xPeriodicRange = 12f,
            initialPosition = vector2,
            animationLength = 1,
            interval = 5000f,
            position = vector2,
            scale = 4f
          });
        }
      }
      if ((double) this.submergeTimer >= 20000.0)
      {
        Game1.changeMusicTrack("night_market");
        this.ignoreWarps = false;
        this.changeSubmergeLight(true, true);
        Game1.playSound("pullItemFromWater");
        Game1.ambientLight = Color.Black;
        this.ambientLightTargetColor = Color.Black;
        this.hasLitSubmergeLight = false;
        Game1.background = new Background((GameLocation) this, new Color(0, 50, (int) byte.MaxValue), true);
        this.curtainOpenPercent = 0.0f;
        this.curtainMovement = 0.0f;
        this.submergeTimer = 0.0f;
        this.submerged.Value = false;
        this.ascending.Value = false;
        Game1.netWorldState.Value.IsSubmarineLocked = false;
        this.hasLitAscendLight = false;
        this.doneUntilReset = false;
        this.localAscending = false;
      }
    }
    else if (this.submerged.Value && !this.doneUntilReset)
    {
      if (Game1.random.NextDouble() < 0.01)
      {
        Vector2 vector2 = new Vector2((float) Game1.random.Next(384, this.map.DisplayWidth - 64 /*0x40*/), 320f);
        int num = Game1.random.Next(3);
        Game1.background.tempSprites.Add(new TemporaryAnimatedSprite()
        {
          motion = new Vector2(0.0f, (float) ((double) num * 0.20000000298023224 - 1.0)),
          yStopCoordinate = 1,
          texture = this.submarineSprites,
          sourceRect = new Microsoft.Xna.Framework.Rectangle(132 + num * 8, 20, 8, 8),
          animationLength = 1,
          interval = 20000f,
          xPeriodic = true,
          xPeriodicLoopTime = 1500f,
          xPeriodicRange = 12f,
          initialPosition = vector2,
          position = vector2,
          scale = 4f
        });
      }
      if (Game1.random.NextDouble() < 0.001)
      {
        Vector2 vector2 = new Vector2(1344f, (float) Game1.random.Next(448, 704));
        Game1.background.tempSprites.Add(new TemporaryAnimatedSprite()
        {
          motion = new Vector2(-0.5f, 0.0f),
          xStopCoordinate = 448,
          texture = this.submarineSprites,
          sourceRect = new Microsoft.Xna.Framework.Rectangle(3, 194, 16 /*0x10*/, 16 /*0x10*/),
          animationLength = 1,
          interval = 50000f,
          alpha = 0.5f,
          yPeriodic = true,
          yPeriodicLoopTime = 5500f,
          yPeriodicRange = 32f,
          initialPosition = vector2,
          position = vector2,
          scale = 4f
        });
      }
      if (Game1.random.NextDouble() < 0.001)
        Game1.background.tempSprites.Insert(0, new TemporaryAnimatedSprite()
        {
          texture = this.submarineSprites,
          sourceRect = new Microsoft.Xna.Framework.Rectangle(0, 146, 16 /*0x10*/, 13),
          animationLength = 9,
          interval = 100f,
          position = new Vector2((float) (Game1.random.Next(96 /*0x60*/, 381) * 4), (float) (Game1.random.Next(24, 66) * 4)),
          scale = 4f
        });
      if (Game1.random.NextDouble() < 5E-05)
      {
        Vector2 vector2 = new Vector2(3f, 10f) * 64f;
        Game1.background.tempSprites.Add(new TemporaryAnimatedSprite()
        {
          motion = new Vector2(-0.0f, -1f),
          color = new Color(0, 50, 150),
          yStopCoordinate = 64 /*0x40*/,
          texture = this.submarineSprites,
          sourceRectStartingPos = new Vector2(67f, 189f),
          sourceRect = new Microsoft.Xna.Framework.Rectangle(67, 189, 24, 53),
          totalNumberOfLoops = 50,
          animationLength = 3,
          pingPong = true,
          interval = 192f,
          xPeriodic = true,
          xPeriodicLoopTime = 3500f,
          xPeriodicRange = 12f,
          initialPosition = vector2,
          position = vector2,
          scale = 4f
        });
      }
      if (Game1.random.NextDouble() < 0.00035)
      {
        Vector2 vector2 = new Vector2(24f, 2f) * 64f;
        int num = Game1.random.Next(3);
        Game1.background.tempSprites.Add(new TemporaryAnimatedSprite()
        {
          motion = new Vector2(-0.5f, 0.0f),
          xStopCoordinate = 64 /*0x40*/,
          texture = this.submarineSprites,
          sourceRectStartingPos = new Vector2((float) (257 + num * 48 /*0x30*/), 81f),
          sourceRect = new Microsoft.Xna.Framework.Rectangle(257 + num * 48 /*0x30*/, 81, 16 /*0x10*/, 16 /*0x10*/),
          totalNumberOfLoops = 250,
          animationLength = 3,
          interval = 200f,
          pingPong = true,
          yPeriodic = true,
          yPeriodicLoopTime = 3500f,
          yPeriodicRange = 12f,
          initialPosition = vector2,
          position = vector2,
          scale = 4f
        });
      }
    }
    if (Game1.ambientLight.Equals(this.ambientLightTargetColor))
      return;
    if ((int) Game1.ambientLight.R < (int) this.ambientLightTargetColor.R)
      ++Game1.ambientLight.R;
    else if ((int) Game1.ambientLight.R > (int) this.ambientLightTargetColor.R)
      --Game1.ambientLight.R;
    if ((int) Game1.ambientLight.G < (int) this.ambientLightTargetColor.G)
      ++Game1.ambientLight.G;
    else if ((int) Game1.ambientLight.G > (int) this.ambientLightTargetColor.G)
      --Game1.ambientLight.G;
    if ((int) Game1.ambientLight.B < (int) this.ambientLightTargetColor.B)
    {
      ++Game1.ambientLight.B;
    }
    else
    {
      if ((int) Game1.ambientLight.B <= (int) this.ambientLightTargetColor.B)
        return;
      --Game1.ambientLight.B;
    }
  }

  public override void cleanupBeforePlayerExit()
  {
    base.cleanupBeforePlayerExit();
    Game1.background = (Background) null;
  }
}
