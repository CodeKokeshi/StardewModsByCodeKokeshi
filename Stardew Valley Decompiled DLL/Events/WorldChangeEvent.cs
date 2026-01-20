// Decompiled with JetBrains decompiler
// Type: StardewValley.Events.WorldChangeEvent
// Assembly: Stardew Valley, Version=1.6.15.24356, Culture=neutral, PublicKeyToken=null
// MVID: AA1F513A-94F0-4EF6-A35F-2D5B4A3721BF
// Assembly location: D:\SteamLibrary\steamapps\common\Stardew Valley\Stardew Valley.dll
// XML documentation location: D:\SteamLibrary\steamapps\common\Stardew Valley\Stardew Valley.xml

using Microsoft.Xna.Framework;
using Netcode;
using StardewValley.BellsAndWhistles;
using StardewValley.Buildings;
using StardewValley.Extensions;
using StardewValley.Locations;
using System;
using System.Collections.Generic;

#nullable disable
namespace StardewValley.Events;

public class WorldChangeEvent : BaseFarmEvent
{
  public const int identifier = 942066;
  public const int jojaGreenhouse = 0;
  public const int junimoGreenHouse = 1;
  public const int jojaBoiler = 2;
  public const int junimoBoiler = 3;
  public const int jojaBridge = 4;
  public const int junimoBridge = 5;
  public const int jojaBus = 6;
  public const int junimoBus = 7;
  public const int jojaBoulder = 8;
  public const int junimoBoulder = 9;
  public const int jojaMovieTheater = 10;
  public const int junimoMovieTheater = 11;
  public const int movieTheaterLightning = 12;
  public const int willyBoatRepair = 13;
  public const int treehouseBuild = 14;
  public const int goldenParrots = 15;
  public readonly NetInt whichEvent = new NetInt();
  private int cutsceneLengthTimer;
  private int timerSinceFade;
  private int soundTimer;
  private int soundInterval = 99999;
  private GameLocation location;
  private string sound;
  private bool wasRaining;
  public GameLocation preEventLocation;

  public WorldChangeEvent()
    : this(0)
  {
  }

  public WorldChangeEvent(int which) => this.whichEvent.Value = which;

  /// <inheritdoc />
  public override void initNetFields()
  {
    base.initNetFields();
    this.NetFields.AddField((INetSerializable) this.whichEvent, "whichEvent");
  }

  private void obliterateJojaMartDoor()
  {
    Town location = Game1.RequireLocation<Town>("Town");
    location.crackOpenAbandonedJojaMartDoor();
    for (int index = 0; index < 16 /*0x10*/; ++index)
      location.temporarySprites.Add(new TemporaryAnimatedSprite("LooseSprites\\Cursors", new Rectangle(372, 1956, 10, 10), new Vector2(96f, 50f) * 64f + new Vector2((float) Game1.random.Next(-32, 64 /*0x40*/), 0.0f), false, 1f / 500f, Color.Gray)
      {
        alpha = 0.75f,
        motion = new Vector2(0.0f, -0.5f) + new Vector2((float) (Game1.random.Next(100) - 50) / 100f, (float) (Game1.random.Next(100) - 50) / 100f),
        interval = 99999f,
        layerDepth = (float) (0.949999988079071 + (double) index * (1.0 / 1000.0)),
        scale = 3f,
        scaleChange = 0.01f,
        rotationChange = (float) ((double) Game1.random.Next(-5, 6) * 3.1415927410125732 / 256.0),
        delayBeforeAnimationStart = index * 25
      });
    Utility.addDirtPuffs((GameLocation) location, 95, 49, 2, 2);
    location.temporarySprites.Add(new TemporaryAnimatedSprite("LooseSprites\\Cursors", new Rectangle(372, 1956, 10, 10), new Vector2(96f, 50f) * 64f + new Vector2((float) Game1.random.Next(-32, 64 /*0x40*/), 0.0f), false, 0.0f, Color.Gray)
    {
      alpha = 0.01f,
      interval = 99999f,
      layerDepth = 0.9f,
      lightId = this.GenerateLightSourceId() + "_obliterateJojaMartDoor",
      lightRadius = 4f,
      lightcolor = new Color(1, 1, 1)
    });
  }

  /// <inheritdoc />
  public override bool setUp()
  {
    this.preEventLocation = Game1.currentLocation;
    this.location = (GameLocation) null;
    Point targetTile = Point.Zero;
    this.wasRaining = Game1.isRaining;
    switch (this.whichEvent.Value)
    {
      case 0:
      case 1:
        this.location = (GameLocation) Game1.getFarm();
        targetTile = Game1.whichFarm == 5 ? new Point(39, 32 /*0x20*/) : new Point(28, 13);
        using (List<Building>.Enumerator enumerator = this.location.buildings.GetEnumerator())
        {
          while (enumerator.MoveNext())
          {
            Building current = enumerator.Current;
            if (current is GreenhouseBuilding)
            {
              targetTile = new Point(current.tileX.Value + 3, current.tileY.Value + 3);
              break;
            }
          }
          break;
        }
      case 2:
      case 3:
        this.location = Game1.RequireLocation("Town");
        targetTile = new Point(105, 79);
        break;
      case 4:
      case 5:
        this.location = Game1.RequireLocation("Mountain");
        targetTile = new Point(95, 27);
        break;
      case 6:
      case 7:
        this.location = Game1.RequireLocation("BusStop");
        targetTile = new Point(24, 8);
        break;
      case 8:
      case 9:
        this.location = Game1.RequireLocation("Mountain");
        targetTile = new Point(48 /*0x30*/, 5);
        break;
      case 10:
        this.location = Game1.RequireLocation("Town");
        targetTile = new Point(52, 18);
        break;
      case 11:
        this.location = Game1.RequireLocation("Town");
        targetTile = new Point(95, 48 /*0x30*/);
        break;
      case 12:
        this.location = Game1.RequireLocation("Town");
        targetTile = new Point(95, 48 /*0x30*/);
        break;
      case 13:
        this.location = Game1.RequireLocation("BoatTunnel");
        targetTile = new Point(7, 7);
        break;
      case 14:
        this.location = Game1.RequireLocation("Mountain");
        targetTile = new Point(16 /*0x10*/, 7);
        break;
      case 15:
        this.location = Game1.RequireLocation("IslandNorth");
        targetTile = new Point(40, 23);
        break;
    }
    Game1.currentLocation = this.location;
    this.resetForPlayerEntry(targetTile);
    return false;
  }

  public void resetForPlayerEntry(Point targetTile)
  {
    this.location.resetForPlayerEntry();
    this.cutsceneLengthTimer = 8000;
    this.wasRaining = Game1.isRaining;
    Game1.isRaining = false;
    Game1.changeMusicTrack("nightTime");
    string id = $"{nameof (WorldChangeEvent)}_{this.whichEvent.Value}";
    switch (this.whichEvent.Value)
    {
      case 0:
        this.location.temporarySprites.Add(new TemporaryAnimatedSprite("LooseSprites\\Cursors", new Rectangle(288, 1349, 19, 28), 150f, 5, 999, new Vector2((float) ((targetTile.X - 3) * 64 /*0x40*/ + 8), (float) ((targetTile.Y - 1) * 64 /*0x40*/ - 32 /*0x20*/)), false, false)
        {
          scale = 4f,
          layerDepth = 1f
        });
        this.location.temporarySprites.Add(new TemporaryAnimatedSprite("LooseSprites\\Cursors", new Rectangle(288, 1377, 19, 28), 140f, 5, 999, new Vector2((float) ((targetTile.X + 3) * 64 /*0x40*/ - 16 /*0x10*/), (float) ((targetTile.Y - 2) * 64 /*0x40*/)), false, false)
        {
          scale = 4f,
          layerDepth = 1f
        });
        this.location.temporarySprites.Add(new TemporaryAnimatedSprite("LooseSprites\\Cursors", new Rectangle(390, 1405, 18, 32 /*0x20*/), 1000f, 2, 999, new Vector2((float) (targetTile.X * 64 /*0x40*/ + 8), (float) ((targetTile.Y - 4) * 64 /*0x40*/)), false, false)
        {
          scale = 4f,
          layerDepth = 1f
        });
        this.soundInterval = 560;
        Game1.currentLightSources.Add(new LightSource(id, 4, new Vector2((float) targetTile.X, (float) targetTile.Y) * 64f, 4f));
        this.sound = "axchop";
        float num1 = (float) ((targetTile.Y + 3) * 64 /*0x40*/) / 10000f;
        TemporaryAnimatedSprite hole = new TemporaryAnimatedSprite("Buildings\\Greenhouse", new Rectangle(25, 133, 31 /*0x1F*/, 19), 99999f, 1, 999, new Vector2((float) (targetTile.X * 64 /*0x40*/), (float) ((targetTile.Y - 1) * 64 /*0x40*/ - 64 /*0x40*/)) + new Vector2(-23f, 53f) * 4f, false, false)
        {
          scale = 4f,
          layerDepth = num1 + 0.0008f
        };
        this.location.temporarySprites.Add(hole);
        TemporaryAnimatedSprite raccoon = new TemporaryAnimatedSprite("Characters\\raccoon", new Rectangle(0, 32 /*0x20*/, 32 /*0x20*/, 32 /*0x20*/), 99999f, 1, 999, new Vector2((float) (targetTile.X * 64 /*0x40*/), (float) ((targetTile.Y - 1) * 64 /*0x40*/ - 64 /*0x40*/)) + new Vector2(-20f, 40f) * 4f, false, false)
        {
          scale = 4f,
          shakeIntensity = 1f,
          layerDepth = num1 + 0.0004f,
          delayBeforeAnimationStart = 3000,
          motion = new Vector2(-1f, -6f),
          acceleration = new Vector2(0.0f, 0.17f),
          xStopCoordinate = targetTile.X * 64 /*0x40*/ - 136,
          startSound = "Raccoon"
        };
        raccoon.reachedStopCoordinate = (TemporaryAnimatedSprite.endBehavior) (x =>
        {
          hole.layerDepth = 0.0f;
          raccoon.motion.X = -1f;
          raccoon.yStopCoordinate = targetTile.Y * 64 /*0x40*/ + 72;
          raccoon.reachedStopCoordinate = (TemporaryAnimatedSprite.endBehavior) (y =>
          {
            raccoon.motion = new Vector2(0.0f, 4f);
            raccoon.acceleration = Vector2.Zero;
            raccoon.sourceRect = new Rectangle(0, 0, 32 /*0x20*/, 32 /*0x20*/);
            raccoon.animationLength = 8;
            raccoon.interval = 80f;
            raccoon.sourceRectStartingPos = Vector2.Zero;
            raccoon.yStopCoordinate = targetTile.Y * 64 /*0x40*/ + 160 /*0xA0*/;
            raccoon.reachedStopCoordinate = (TemporaryAnimatedSprite.endBehavior) (z =>
            {
              raccoon.layerDepth = -1f;
              raccoon.motion = new Vector2(0.0f, 4f);
              raccoon.layerDepthOffset = 0.0128f;
            });
          });
        });
        this.location.temporarySprites.Add(raccoon);
        break;
      case 1:
        Utility.addSprinklesToLocation(this.location, targetTile.X, targetTile.Y - 1, 7, 7, 15000, 150, Color.LightCyan);
        Utility.addStarsAndSpirals(this.location, targetTile.X, targetTile.Y - 1, 7, 7, 15000, 150, Color.White);
        Game1.player.activeDialogueEvents.TryAdd("cc_Greenhouse", 3);
        this.sound = "junimoMeep1";
        Game1.currentLightSources.Add(new LightSource(id, 4, new Vector2((float) targetTile.X, (float) targetTile.Y) * 64f, 4f, Color.DarkGoldenrod));
        this.location.temporarySprites.Add(new TemporaryAnimatedSprite("LooseSprites\\Cursors", new Rectangle(294, 1432, 16 /*0x10*/, 16 /*0x10*/), 300f, 4, 999, new Vector2((float) (targetTile.X * 64 /*0x40*/), (float) ((targetTile.Y - 1) * 64 /*0x40*/ - 64 /*0x40*/)), false, false)
        {
          scale = 4f,
          layerDepth = 1f,
          xPeriodic = true,
          xPeriodicLoopTime = 2000f,
          xPeriodicRange = 16f,
          lightId = id,
          lightcolor = Color.DarkGoldenrod,
          lightRadius = 1f
        });
        this.soundInterval = 800;
        float num2 = (float) ((targetTile.Y + 3) * 64 /*0x40*/) / 10000f;
        TemporaryAnimatedSprite hole2 = new TemporaryAnimatedSprite("Buildings\\Greenhouse", new Rectangle(25, 133, 31 /*0x1F*/, 19), 99999f, 1, 999, new Vector2((float) (targetTile.X * 64 /*0x40*/), (float) ((targetTile.Y - 1) * 64 /*0x40*/ - 64 /*0x40*/)) + new Vector2(-23f, 53f) * 4f, false, false)
        {
          scale = 4f,
          layerDepth = num2 + 0.0008f
        };
        this.location.temporarySprites.Add(hole2);
        TemporaryAnimatedSprite raccoon2 = new TemporaryAnimatedSprite("Characters\\raccoon", new Rectangle(0, 32 /*0x20*/, 32 /*0x20*/, 32 /*0x20*/), 99999f, 1, 999, new Vector2((float) (targetTile.X * 64 /*0x40*/), (float) ((targetTile.Y - 1) * 64 /*0x40*/ - 64 /*0x40*/)) + new Vector2(-20f, 40f) * 4f, false, false)
        {
          scale = 4f,
          shakeIntensity = 1f,
          layerDepth = num2 + 0.0004f,
          delayBeforeAnimationStart = 3000,
          motion = new Vector2(-1f, -6f),
          acceleration = new Vector2(0.0f, 0.17f),
          xStopCoordinate = targetTile.X * 64 /*0x40*/ - 136,
          startSound = "Raccoon"
        };
        raccoon2.reachedStopCoordinate = (TemporaryAnimatedSprite.endBehavior) (x =>
        {
          hole2.layerDepth = 0.0f;
          raccoon2.motion.X = -1f;
          raccoon2.yStopCoordinate = targetTile.Y * 64 /*0x40*/ + 72;
          raccoon2.reachedStopCoordinate = (TemporaryAnimatedSprite.endBehavior) (y =>
          {
            raccoon2.motion = new Vector2(0.0f, 4f);
            raccoon2.acceleration = Vector2.Zero;
            raccoon2.sourceRect = new Rectangle(0, 0, 32 /*0x20*/, 32 /*0x20*/);
            raccoon2.animationLength = 8;
            raccoon2.interval = 80f;
            raccoon2.sourceRectStartingPos = Vector2.Zero;
            raccoon2.yStopCoordinate = targetTile.Y * 64 /*0x40*/ + 160 /*0xA0*/;
            raccoon2.reachedStopCoordinate = (TemporaryAnimatedSprite.endBehavior) (z =>
            {
              raccoon2.layerDepth = -1f;
              raccoon2.motion = new Vector2(0.0f, 4f);
              raccoon2.layerDepthOffset = 0.0128f;
            });
          });
        });
        this.location.temporarySprites.Add(raccoon2);
        break;
      case 2:
        this.location.temporarySprites.Add(new TemporaryAnimatedSprite("LooseSprites\\Cursors", new Rectangle(288, 1377, 19, 28), 100f, 5, 999, new Vector2(6656f, 5024f), false, false)
        {
          scale = 4f,
          layerDepth = 1f
        });
        this.location.temporarySprites.Add(new TemporaryAnimatedSprite("LooseSprites\\Cursors", new Rectangle(288, 1406, 22, 26), 700f, 2, 999, new Vector2(6888f, 5014f), false, false)
        {
          scale = 4f,
          layerDepth = 1f
        });
        this.location.temporarySprites.Add(new TemporaryAnimatedSprite("LooseSprites\\Cursors", new Rectangle(390, 1405, 18, 32 /*0x20*/), 1500f, 2, 999, new Vector2(6792f, 4864f), false, false)
        {
          scale = 4f,
          layerDepth = 1f
        });
        this.location.temporarySprites.Add(new TemporaryAnimatedSprite("LooseSprites\\Cursors", new Rectangle(335, 1410, 21, 21), 999f, 1, 9999, new Vector2(6912f, 5136f), false, false)
        {
          scale = 4f,
          layerDepth = 1f
        });
        Game1.player.activeDialogueEvents.TryAdd("cc_Minecart", 7);
        this.soundInterval = 500;
        Game1.currentLightSources.Add(new LightSource(id, 4, new Vector2((float) targetTile.X, (float) targetTile.Y) * 64f, 4f));
        this.sound = "clank";
        break;
      case 3:
        Utility.addSprinklesToLocation(this.location, targetTile.X + 1, targetTile.Y, 6, 4, 15000, 350, Color.LightCyan);
        Utility.addStarsAndSpirals(this.location, targetTile.X + 1, targetTile.Y, 6, 4, 15000, 350, Color.White);
        Game1.player.activeDialogueEvents.TryAdd("cc_Minecart", 7);
        this.location.temporarySprites.Add(new TemporaryAnimatedSprite("LooseSprites\\Cursors", new Rectangle(294, 1432, 16 /*0x10*/, 16 /*0x10*/), 300f, 4, 999, new Vector2(6656f, 5056f), false, false)
        {
          scale = 4f,
          layerDepth = 1f,
          xPeriodic = true,
          xPeriodicLoopTime = 2000f,
          xPeriodicRange = 16f,
          lightId = id + "_1",
          lightcolor = Color.DarkGoldenrod,
          lightRadius = 1f
        });
        this.location.temporarySprites.Add(new TemporaryAnimatedSprite("LooseSprites\\Cursors", new Rectangle(294, 1432, 16 /*0x10*/, 16 /*0x10*/), 300f, 4, 999, new Vector2(6912f, 5056f), false, false)
        {
          scale = 4f,
          layerDepth = 1f,
          xPeriodic = true,
          xPeriodicLoopTime = 2300f,
          xPeriodicRange = 16f,
          color = Color.HotPink,
          lightId = id + "_2",
          lightcolor = Color.DarkGoldenrod,
          lightRadius = 1f
        });
        this.sound = "junimoMeep1";
        Game1.currentLightSources.Add(new LightSource(id, 4, new Vector2((float) targetTile.X, (float) targetTile.Y) * 64f, 4f, Color.DarkGoldenrod));
        this.soundInterval = 800;
        break;
      case 4:
        this.location.temporarySprites.Add(new TemporaryAnimatedSprite("LooseSprites\\Cursors", new Rectangle(383, 1378, 28, 27), 400f, 2, 999, new Vector2(5504f, 1632f), false, false)
        {
          scale = 4f,
          layerDepth = 1f,
          motion = new Vector2(0.5f, 0.0f)
        });
        this.location.temporarySprites.Add(new TemporaryAnimatedSprite("LooseSprites\\Cursors", new Rectangle(288, 1406, 22, 26), 350f, 2, 999, new Vector2(6272f, 1632f), false, false)
        {
          scale = 4f,
          layerDepth = 1f
        });
        this.location.temporarySprites.Add(new TemporaryAnimatedSprite("LooseSprites\\Cursors", new Rectangle(358, 1415, 31 /*0x1F*/, 20), 999f, 1, 9999, new Vector2(5888f, 1648f), false, false)
        {
          scale = 4f,
          layerDepth = 1f
        });
        this.location.temporarySprites.Add(new TemporaryAnimatedSprite("LooseSprites\\Cursors", new Rectangle(335, 1410, 21, 21), 999f, 1, 9999, new Vector2(6400f, 1648f), false, false)
        {
          scale = 4f,
          layerDepth = 1f
        });
        this.location.temporarySprites.Add(new TemporaryAnimatedSprite("LooseSprites\\Cursors", new Rectangle(390, 1405, 18, 32 /*0x20*/), 1500f, 2, 999, new Vector2(5824f, 1584f), false, false)
        {
          scale = 4f,
          layerDepth = 0.8f
        });
        Game1.player.activeDialogueEvents.TryAdd("cc_Bridge", 7);
        this.soundInterval = 700;
        Game1.currentLightSources.Add(new LightSource(id, 4, new Vector2((float) targetTile.X, (float) targetTile.Y) * 64f, 4f));
        this.sound = "axchop";
        break;
      case 5:
        Utility.addSprinklesToLocation(this.location, targetTile.X, targetTile.Y, 7, 4, 15000, 150, Color.LightCyan);
        Utility.addStarsAndSpirals(this.location, targetTile.X + 1, targetTile.Y, 7, 4, 15000, 350, Color.White);
        this.location.temporarySprites.Add(new TemporaryAnimatedSprite("LooseSprites\\Cursors", new Rectangle(294, 1432, 16 /*0x10*/, 16 /*0x10*/), 300f, 4, 999, new Vector2(5824f, 1648f), false, false)
        {
          scale = 4f,
          layerDepth = 1f,
          xPeriodic = true,
          xPeriodicLoopTime = 2000f,
          xPeriodicRange = 16f,
          lightId = id + "_1",
          lightcolor = Color.DarkGoldenrod,
          lightRadius = 1f
        });
        this.location.temporarySprites.Add(new TemporaryAnimatedSprite("LooseSprites\\Cursors", new Rectangle(294, 1432, 16 /*0x10*/, 16 /*0x10*/), 300f, 4, 999, new Vector2(6336f, 1648f), false, false)
        {
          scale = 4f,
          layerDepth = 1f,
          xPeriodic = true,
          xPeriodicLoopTime = 2300f,
          xPeriodicRange = 16f,
          color = Color.Yellow,
          lightId = id + "_2",
          lightcolor = Color.DarkGoldenrod,
          lightRadius = 1f
        });
        Game1.player.activeDialogueEvents.TryAdd("cc_Bridge", 7);
        this.sound = "junimoMeep1";
        Game1.currentLightSources.Add(new LightSource(id, 4, new Vector2((float) targetTile.X, (float) targetTile.Y) * 64f, 4f, Color.DarkGoldenrod));
        this.soundInterval = 800;
        break;
      case 6:
        this.location.temporarySprites.Add(new TemporaryAnimatedSprite("LooseSprites\\Cursors", new Rectangle(288, 1349, 19, 28), 150f, 5, 999, new Vector2(1856f, 480f), false, false)
        {
          scale = 4f,
          layerDepth = 1f
        });
        this.location.temporarySprites.Add(new TemporaryAnimatedSprite("LooseSprites\\Cursors", new Rectangle(288, 1377, 19, 28), 140f, 5, 999, new Vector2(1280f, 512f), false, false)
        {
          scale = 4f,
          layerDepth = 1f
        });
        this.location.temporarySprites.Add(new TemporaryAnimatedSprite("LooseSprites\\Cursors", new Rectangle(390, 1405, 18, 32 /*0x20*/), 1500f, 2, 999, new Vector2(1544f, 192f), false, false)
        {
          scale = 4f,
          layerDepth = 1f
        });
        Game1.player.activeDialogueEvents.TryAdd("cc_Bus", 7);
        this.soundInterval = 560;
        Game1.currentLightSources.Add(new LightSource(id, 4, new Vector2((float) targetTile.X, (float) targetTile.Y) * 64f, 4f));
        this.sound = "clank";
        break;
      case 7:
        Utility.addSprinklesToLocation(this.location, targetTile.X, targetTile.Y, 9, 4, 10000, 200, Color.LightCyan, motionTowardCenter: true);
        Utility.addStarsAndSpirals(this.location, targetTile.X, targetTile.Y, 9, 4, 15000, 150, Color.White);
        this.sound = "junimoMeep1";
        this.location.temporarySprites.Add(new TemporaryAnimatedSprite("LooseSprites\\Cursors", new Rectangle(294, 1432, 16 /*0x10*/, 16 /*0x10*/), 300f, 4, 999, new Vector2(1280f, 640f), false, false)
        {
          scale = 4f,
          layerDepth = 1f,
          xPeriodic = true,
          xPeriodicLoopTime = 2000f,
          xPeriodicRange = 16f,
          lightId = id + "_1",
          lightcolor = Color.DarkGoldenrod,
          lightRadius = 1f
        });
        this.location.temporarySprites.Add(new TemporaryAnimatedSprite("LooseSprites\\Cursors", new Rectangle(294, 1432, 16 /*0x10*/, 16 /*0x10*/), 300f, 4, 999, new Vector2(1408f, 640f), false, false)
        {
          scale = 4f,
          layerDepth = 1f,
          xPeriodic = true,
          xPeriodicLoopTime = 2300f,
          xPeriodicRange = 16f,
          color = Color.Pink,
          lightId = id + "_2",
          lightcolor = Color.DarkGoldenrod,
          lightRadius = 1f
        });
        this.location.temporarySprites.Add(new TemporaryAnimatedSprite("LooseSprites\\Cursors", new Rectangle(294, 1432, 16 /*0x10*/, 16 /*0x10*/), 300f, 4, 999, new Vector2(1536f, 640f), false, false)
        {
          scale = 4f,
          layerDepth = 1f,
          xPeriodic = true,
          xPeriodicLoopTime = 2200f,
          xPeriodicRange = 16f,
          color = Color.Yellow,
          lightId = id + "_3",
          lightcolor = Color.DarkGoldenrod,
          lightRadius = 1f
        });
        this.location.temporarySprites.Add(new TemporaryAnimatedSprite("LooseSprites\\Cursors", new Rectangle(294, 1432, 16 /*0x10*/, 16 /*0x10*/), 300f, 4, 999, new Vector2(1664f, 640f), false, false)
        {
          scale = 4f,
          layerDepth = 1f,
          xPeriodic = true,
          xPeriodicLoopTime = 2100f,
          xPeriodicRange = 16f,
          color = Color.LightBlue,
          lightId = id + "_4",
          lightcolor = Color.DarkGoldenrod,
          lightRadius = 1f
        });
        Game1.player.activeDialogueEvents.TryAdd("cc_Bus", 7);
        Game1.currentLightSources.Add(new LightSource(id, 4, new Vector2((float) targetTile.X, (float) targetTile.Y) * 64f, 4f, Color.DarkGoldenrod));
        this.soundInterval = 500;
        break;
      case 8:
        this.location.temporarySprites.Add(new TemporaryAnimatedSprite("LooseSprites\\Cursors", new Rectangle(288, 1377, 19, 28), 100f, 5, 999, new Vector2(2880f, 288f), false, false)
        {
          scale = 4f,
          layerDepth = 1f
        });
        this.location.temporarySprites.Add(new TemporaryAnimatedSprite("LooseSprites\\Cursors", new Rectangle(387, 1340, 17, 37), 50f, 2, 99999, new Vector2(3040f, 160f), false, false)
        {
          scale = 4f,
          layerDepth = 1f,
          yPeriodic = true,
          yPeriodicLoopTime = 100f,
          yPeriodicRange = 2f
        });
        this.location.temporarySprites.Add(new TemporaryAnimatedSprite("LooseSprites\\Cursors", new Rectangle(335, 1410, 21, 21), 999f, 1, 9999, new Vector2(2816f, 368f), false, false)
        {
          scale = 4f,
          layerDepth = 1f
        });
        this.location.temporarySprites.Add(new TemporaryAnimatedSprite("LooseSprites\\Cursors", new Rectangle(390, 1405, 18, 32 /*0x20*/), 1500f, 2, 999, new Vector2(3200f, 368f), false, false)
        {
          scale = 4f,
          layerDepth = 1f
        });
        Game1.player.activeDialogueEvents.TryAdd("cc_Boulder", 7);
        this.soundInterval = 100;
        Game1.currentLightSources.Add(new LightSource(id, 4, new Vector2((float) targetTile.X, (float) targetTile.Y) * 64f, 4f));
        this.sound = "thudStep";
        break;
      case 9:
        Game1.player.activeDialogueEvents.TryAdd("cc_Boulder", 7);
        Utility.addSprinklesToLocation(this.location, targetTile.X, targetTile.Y, 4, 4, 15000, 350, Color.LightCyan);
        Utility.addStarsAndSpirals(this.location, targetTile.X + 1, targetTile.Y, 4, 4, 15000, 550, Color.White);
        this.location.temporarySprites.Add(new TemporaryAnimatedSprite("LooseSprites\\Cursors", new Rectangle(294, 1432, 16 /*0x10*/, 16 /*0x10*/), 300f, 4, 999, new Vector2(2880f, 368f), false, false)
        {
          scale = 4f,
          layerDepth = 1f,
          xPeriodic = true,
          xPeriodicLoopTime = 2000f,
          xPeriodicRange = 16f,
          lightId = id + "_1",
          lightcolor = Color.DarkGoldenrod,
          lightRadius = 1f
        });
        this.location.temporarySprites.Add(new TemporaryAnimatedSprite("LooseSprites\\Cursors", new Rectangle(294, 1432, 16 /*0x10*/, 16 /*0x10*/), 300f, 4, 999, new Vector2(3200f, 368f), false, false)
        {
          scale = 4f,
          layerDepth = 1f,
          xPeriodic = true,
          xPeriodicLoopTime = 2300f,
          xPeriodicRange = 16f,
          color = Color.Yellow,
          lightId = id + "_2",
          lightcolor = Color.DarkGoldenrod,
          lightRadius = 1f
        });
        this.sound = "junimoMeep1";
        Game1.currentLightSources.Add(new LightSource(id + "_1", 4, new Vector2((float) targetTile.X, (float) targetTile.Y) * 64f, 1f));
        Game1.currentLightSources.Add(new LightSource(id + "_2", 4, new Vector2((float) targetTile.X, (float) targetTile.Y) * 64f, 1f, Color.DarkCyan));
        Game1.currentLightSources.Add(new LightSource(id + "_3", 4, new Vector2((float) targetTile.X, (float) targetTile.Y) * 64f, 4f, Color.DarkGoldenrod));
        this.soundInterval = 1000;
        break;
      case 10:
        this.location.temporarySprites.Add(new TemporaryAnimatedSprite("LooseSprites\\Cursors", new Rectangle(288, 1349, 19, 28), 150f, 5, 999, new Vector2(3760f, 1056f), false, false)
        {
          scale = 4f,
          layerDepth = 1f
        });
        this.location.temporarySprites.Add(new TemporaryAnimatedSprite("LooseSprites\\Cursors", new Rectangle(288, 1377, 19, 28), 140f, 5, 999, new Vector2(2948f, 1088f), false, false)
        {
          scale = 4f,
          layerDepth = 1f
        });
        this.location.temporarySprites.Add(new TemporaryAnimatedSprite("LooseSprites\\Cursors", new Rectangle(390, 1405, 18, 32 /*0x20*/), 1000f, 2, 999, new Vector2(3144f, 1280f), false, false)
        {
          scale = 4f,
          layerDepth = 1f
        });
        Game1.player.activeDialogueEvents.TryAdd("movieTheater", 3);
        this.soundInterval = 560;
        Game1.currentLightSources.Add(new LightSource(id, 4, new Vector2((float) targetTile.X, (float) targetTile.Y) * 64f, 4f));
        this.sound = "axchop";
        break;
      case 11:
        Utility.addSprinklesToLocation(this.location, targetTile.X, targetTile.Y, 7, 7, 15000, 150, Color.LightCyan);
        Utility.addStarsAndSpirals(this.location, targetTile.X, targetTile.Y, 7, 7, 15000, 150, Color.White);
        Game1.player.activeDialogueEvents.TryAdd("movieTheater", 3);
        this.sound = "junimoMeep1";
        Game1.currentLightSources.Add(new LightSource(id, 4, new Vector2((float) targetTile.X, (float) targetTile.Y) * 64f, 4f, Color.DarkGoldenrod));
        this.location.temporarySprites.Add(new TemporaryAnimatedSprite("LooseSprites\\Cursors", new Rectangle(294, 1432, 16 /*0x10*/, 16 /*0x10*/), 300f, 4, 999, new Vector2(6080f, 2880f), false, false)
        {
          scale = 4f,
          layerDepth = 1f,
          xPeriodic = true,
          xPeriodicLoopTime = 2000f,
          xPeriodicRange = 16f,
          lightId = id,
          lightcolor = Color.DarkGoldenrod,
          lightRadius = 1f
        });
        this.soundInterval = 800;
        break;
      case 12:
        this.cutsceneLengthTimer += 3000;
        Game1.isRaining = true;
        Game1.changeMusicTrack("rain");
        if (Game1.IsMasterGame)
          Game1.addMailForTomorrow("abandonedJojaMartAccessible", true);
        Rectangle sourceRect = new Rectangle(644, 1078, 37, 57);
        Vector2 vector2 = new Vector2(96f, 50f) * 64f;
        for (Vector2 position = vector2 + new Vector2((float) (-sourceRect.Width * 4 / 2), (float) (-sourceRect.Height * 4)); (double) position.Y > (double) (-sourceRect.Height * 4); position.Y -= (float) (sourceRect.Height * 4))
          this.location.temporarySprites.Add(new TemporaryAnimatedSprite("LooseSprites\\Cursors", sourceRect, 9999f, 1, 999, position, false, Game1.random.NextBool(), (float) (((double) vector2.Y + 32.0) / 10000.0 + 1.0 / 1000.0), 0.025f, Color.White, 4f, 0.0f, 0.0f, 0.0f)
          {
            lightId = id,
            lightRadius = 2f,
            delayBeforeAnimationStart = 6200,
            lightcolor = Color.Black
          });
        DelayedAction.playSoundAfterDelay("thunder_small", 6000);
        DelayedAction.playSoundAfterDelay("boulderBreak", 6300);
        DelayedAction.screenFlashAfterDelay(1f, 6000);
        DelayedAction.functionAfterDelay(new Action(this.obliterateJojaMartDoor), 6050);
        break;
      case 13:
        if (Game1.IsMasterGame)
          Game1.addMailForTomorrow("willyBoatFixed", true);
        Game1.mailbox.Add("willyHours");
        this.location.temporarySprites.Add(new TemporaryAnimatedSprite("Characters\\Willy", new Rectangle(0, 320, 16 /*0x10*/, 32 /*0x20*/), 120f, 3, 999, new Vector2(412f, 332f), false, false)
        {
          pingPong = true,
          scale = 4f,
          layerDepth = 1f
        });
        this.location.temporarySprites.Add(new TemporaryAnimatedSprite("Characters\\Robin", new Rectangle(0, 192 /*0xC0*/, 16 /*0x10*/, 32 /*0x20*/), 140f, 4, 999, new Vector2(704f, 256f), false, false)
        {
          scale = 4f,
          layerDepth = 1f
        });
        this.soundInterval = 560;
        this.sound = "crafting";
        break;
      case 14:
        this.cutsceneLengthTimer = 12000;
        Game1.currentLightSources.Add(new LightSource(id, 4, new Vector2((float) targetTile.X, (float) targetTile.Y) * 64f, 4f, Color.DarkGoldenrod));
        this.location.temporarySprites.Add(new TemporaryAnimatedSprite("LooseSprites\\parrots", new Rectangle(0, 0, 24, 24), new Vector2(14f, 4.5f) * 64f, false, 0.0f, Color.White)
        {
          id = 777,
          scale = 4f,
          totalNumberOfLoops = 99999,
          interval = 9999f,
          animationLength = 1,
          layerDepth = 1f,
          drawAboveAlwaysFront = true
        });
        DelayedAction.functionAfterDelay(new Action(this.ParrotSquawk), 2000);
        for (int index = 0; index < 16 /*0x10*/; ++index)
        {
          Rectangle r = new Rectangle(15, 5, 3, 3);
          TemporaryAnimatedSprite temporaryAnimatedSprite1 = new TemporaryAnimatedSprite("LooseSprites\\Cursors2", new Rectangle(49 + 16 /*0x10*/ * Game1.random.Next(3), 229, 16 /*0x10*/, 6), Utility.getRandomPositionInThisRectangle(r, Game1.random) * 64f, Game1.random.NextBool(), 0.0f, Color.White)
          {
            motion = new Vector2((float) Game1.random.Next(-2, 3), -16f),
            acceleration = new Vector2(0.0f, 0.5f),
            rotationChange = (float) Game1.random.Next(-4, 5) * 0.05f,
            scale = 4f,
            animationLength = 1,
            totalNumberOfLoops = 1,
            interval = (float) (1000 + Game1.random.Next(500)),
            layerDepth = 1f,
            drawAboveAlwaysFront = true,
            yStopCoordinate = (r.Bottom + 1) * 64 /*0x40*/,
            delayBeforeAnimationStart = 4000 + index * 250
          };
          temporaryAnimatedSprite1.reachedStopCoordinate = new TemporaryAnimatedSprite.endBehavior(temporaryAnimatedSprite1.bounce);
          this.location.TemporarySprites.Add(temporaryAnimatedSprite1);
          TemporaryAnimatedSprite temporaryAnimatedSprite2 = new TemporaryAnimatedSprite("LooseSprites\\Cursors2", new Rectangle(49 + 16 /*0x10*/ * Game1.random.Next(3), 229, 16 /*0x10*/, 6), Utility.getRandomPositionInThisRectangle(r, Game1.random) * 64f, Game1.random.NextBool(), 0.0f, Color.White)
          {
            motion = new Vector2((float) Game1.random.Next(-2, 3), -16f),
            acceleration = new Vector2(0.0f, 0.5f),
            rotationChange = (float) Game1.random.Next(-4, 5) * 0.05f,
            scale = 4f,
            animationLength = 1,
            totalNumberOfLoops = 1,
            interval = (float) (1000 + Game1.random.Next(500)),
            layerDepth = 1f,
            drawAboveAlwaysFront = true,
            delayBeforeAnimationStart = 4500 + index * 250,
            yStopCoordinate = (r.Bottom + 1) * 64 /*0x40*/
          };
          temporaryAnimatedSprite2.reachedStopCoordinate = new TemporaryAnimatedSprite.endBehavior(temporaryAnimatedSprite2.bounce);
          this.location.TemporarySprites.Add(temporaryAnimatedSprite2);
        }
        for (int index = 0; index < 20; ++index)
        {
          Vector2 position = new Vector2(Utility.RandomFloat(13f, 19f), 0.0f) * 64f;
          float num3 = 1024f - position.X;
          TemporaryAnimatedSprite temporaryAnimatedSprite = new TemporaryAnimatedSprite("LooseSprites\\parrots", new Rectangle(48 /*0x30*/ + Game1.random.Next(2) * 72, Game1.random.Next(2) * 48 /*0x30*/, 24, 24), position, false, 0.0f, Color.White)
          {
            motion = new Vector2(num3 * 0.01f, 10f),
            acceleration = new Vector2(0.0f, -0.05f),
            id = 778,
            scale = 4f,
            yStopCoordinate = 448,
            totalNumberOfLoops = 99999,
            interval = 50f,
            animationLength = 3,
            flipped = (double) num3 > 0.0,
            layerDepth = 1f,
            drawAboveAlwaysFront = true,
            delayBeforeAnimationStart = 3500 + index * 250,
            alpha = 0.0f,
            alphaFade = -0.1f
          };
          DelayedAction.playSoundAfterDelay("batFlap", 3500 + index * 250);
          temporaryAnimatedSprite.reachedStopCoordinateSprite = new Action<TemporaryAnimatedSprite>(this.ParrotBounce);
          this.location.temporarySprites.Add(temporaryAnimatedSprite);
        }
        DelayedAction.functionAfterDelay(new Action(this.FinishTreehouse), 8000);
        DelayedAction.functionAfterDelay(new Action(this.ParrotSquawk), 9000);
        DelayedAction.functionAfterDelay(new Action(this.ParrotFlyAway), 11000);
        break;
      case 15:
        Game1.changeMusicTrack("jungle_ambience");
        this.location.temporarySprites.Add(new TemporaryAnimatedSprite("LooseSprites\\Cursors_1_6", new Rectangle(200, 89, 28, 32 /*0x20*/), new Vector2(39f, 32f) * 64f, false, 0.0f, Color.White)
        {
          animationLength = 2,
          interval = 700f,
          totalNumberOfLoops = 999,
          layerDepth = 0.1f,
          lightId = id + "_1",
          lightcolor = Color.Black,
          lightRadius = 2f,
          scale = 4f
        });
        int num4 = 1 + (130 - Game1.netWorldState.Value.GoldenWalnutsFound) / 10;
        for (int index = 0; index < num4; ++index)
          this.location.temporarySprites.Add(new TemporaryAnimatedSprite("LooseSprites\\Cursors_1_6", new Rectangle(184, 104, 14, 15), new Vector2((float) (39 + index % 3), (float) (34.099998474121094 + (double) (index / 3) * 0.5)) * 64f, false, 0.0f, Color.White)
          {
            animationLength = 1,
            interval = 700f,
            totalNumberOfLoops = 999,
            layerDepth = (float) (0.10000000149011612 + (double) index * 0.0099999997764825821),
            scale = 4f
          });
        this.cutsceneLengthTimer = 10000;
        for (int index = 0; index < 20; ++index)
        {
          Vector2 position = Utility.getRandomPositionInThisRectangle(new Rectangle(20, 1, 40, 2), Game1.random) * 64f;
          float num5 = (double) position.X > (double) (this.location.Map.DisplayWidth / 2) ? -1f : 1f;
          TemporaryAnimatedSprite temporaryAnimatedSprite = new TemporaryAnimatedSprite("LooseSprites\\parrots", new Rectangle(48 /*0x30*/ + Game1.random.Next(2) * 72, 96 /*0x60*/, 24, 24), position, false, 0.0f, Color.White)
          {
            motion = new Vector2(num5 * 3f, 6f + (float) Game1.random.NextDouble()),
            acceleration = new Vector2(0.0f, -0.01f),
            id = 778,
            scale = 4f,
            yStopCoordinate = (int) position.Y + Game1.random.Next(19, 27) * 64 /*0x40*/ + index * 64 /*0x40*/ / 2,
            totalNumberOfLoops = 99999,
            interval = 80f,
            animationLength = 3,
            pingPong = true,
            flipped = (double) num5 > 0.0,
            layerDepth = 1f,
            drawAboveAlwaysFront = true,
            lightId = id + "_2",
            lightcolor = Color.Black,
            lightRadius = 2f,
            alpha = 1f / 1000f,
            alphaFade = -0.01f,
            delayBeforeAnimationStart = index * 250
          };
          DelayedAction.playSoundAfterDelay("parrot_flap", 500 + index * 250);
          DelayedAction.playSoundAfterDelay("parrot_flap", 5500 + index * 250);
          temporaryAnimatedSprite.reachedStopCoordinateSprite = new Action<TemporaryAnimatedSprite>(this.GoldenParrotBounce);
          this.location.temporarySprites.Add(temporaryAnimatedSprite);
        }
        DelayedAction.functionAfterDelay(new Action(this.ParrotSquawk), 9000);
        DelayedAction.functionAfterDelay(new Action(this.ParrotFlyAway), 11000);
        break;
    }
    this.soundTimer = this.soundInterval;
    Game1.fadeClear();
    Game1.nonWarpFade = true;
    Game1.timeOfDay = 2400;
    Game1.displayHUD = false;
    Game1.viewportFreeze = true;
    Game1.player.position.X = -999999f;
    Game1.viewport.X = Math.Max(0, Math.Min(this.location.map.DisplayWidth - Game1.viewport.Width, targetTile.X * 64 /*0x40*/ - Game1.viewport.Width / 2));
    Game1.viewport.Y = Math.Max(0, Math.Min(this.location.map.DisplayHeight - Game1.viewport.Height, targetTile.Y * 64 /*0x40*/ - Game1.viewport.Height / 2));
    if (!this.location.IsOutdoors)
    {
      Game1.viewport.X = targetTile.X * 64 /*0x40*/ - Game1.viewport.Width / 2;
      Game1.viewport.Y = targetTile.Y * 64 /*0x40*/ - Game1.viewport.Height / 2;
    }
    Game1.previousViewportPosition = new Vector2((float) Game1.viewport.X, (float) Game1.viewport.Y);
    List<WeatherDebris> debrisWeather = Game1.debrisWeather;
    // ISSUE: explicit non-virtual call
    if ((debrisWeather != null ? (__nonvirtual (debrisWeather.Count) > 0 ? 1 : 0) : 0) != 0)
      Game1.randomizeDebrisWeatherPositions(Game1.debrisWeather);
    Game1.randomizeRainPositions();
  }

  public virtual void ParrotFlyAway()
  {
    this.location.removeTemporarySpritesWithIDLocal(777);
    this.location.temporarySprites.Add(new TemporaryAnimatedSprite("LooseSprites\\parrots", new Rectangle(48 /*0x30*/, 0, 24, 24), new Vector2(14f, 4.5f) * 64f, false, 0.0f, Color.White)
    {
      id = 777,
      scale = 4f,
      totalNumberOfLoops = 99999,
      layerDepth = 1f,
      drawAboveAlwaysFront = true,
      interval = 50f,
      animationLength = 3,
      motion = new Vector2(-2f, 0.0f),
      acceleration = new Vector2(0.0f, -0.1f)
    });
  }

  public virtual void ParrotSquawk()
  {
    TemporaryAnimatedSprite temporarySpriteById = this.location.getTemporarySpriteByID(777);
    if (temporarySpriteById != null)
    {
      temporarySpriteById.shakeIntensity = 1f;
      temporarySpriteById.sourceRectStartingPos.X = 24f;
      temporarySpriteById.sourceRect.X = 24;
      DelayedAction.functionAfterDelay(new Action(this.ParrotStopSquawk), 500);
    }
    Game1.playSound("parrot");
  }

  public virtual void ParrotStopSquawk()
  {
    TemporaryAnimatedSprite temporarySpriteById = this.location.getTemporarySpriteByID(777);
    temporarySpriteById.shakeIntensity = 0.0f;
    temporarySpriteById.sourceRectStartingPos.X = 0.0f;
    temporarySpriteById.sourceRect.X = 0;
  }

  public virtual void FinishTreehouse()
  {
    Game1.flashAlpha = 1f;
    Game1.playSound("yoba");
    Game1.playSound("axchop");
    (this.location as Mountain).ApplyTreehouseIfNecessary();
    this.location.removeTemporarySpritesWithIDLocal(778);
    for (int index = 0; index < 20; ++index)
    {
      Vector2 position = new Vector2(Utility.RandomFloat(13f, 19f), Utility.RandomFloat(4f, 7f)) * 64f;
      float num = 1024f - position.X;
      this.location.TemporarySprites.Add(new TemporaryAnimatedSprite("LooseSprites\\parrots", new Rectangle(192 /*0xC0*/, Game1.random.Next(2) * 48 /*0x30*/, 24, 24), position, false, 0.0f, Color.White)
      {
        motion = new Vector2(num * -0.01f, Utility.RandomFloat(-2f, 0.0f)),
        acceleration = new Vector2(0.0f, -0.05f),
        id = 778,
        scale = 4f,
        totalNumberOfLoops = 99999,
        interval = 50f,
        animationLength = 3,
        flipped = (double) num > 0.0,
        layerDepth = 1f,
        drawAboveAlwaysFront = true
      });
    }
  }

  public void ParrotBounce(TemporaryAnimatedSprite sprite)
  {
    float num = 1024f - sprite.Position.X;
    sprite.motion.X = (float) Math.Sign(num) * Utility.RandomFloat(0.5f, 4f);
    sprite.motion.Y = Utility.RandomFloat(-15f, -10f);
    sprite.acceleration.Y = 0.5f;
    sprite.yStopCoordinate = 448;
    sprite.flipped = (double) num > 0.0;
    sprite.sourceRectStartingPos.X = (float) (48 /*0x30*/ + Game1.random.Next(2) * 72);
    if (Game1.random.NextDouble() < 0.05000000074505806)
      Game1.playSound("axe");
    else if (Game1.random.NextDouble() < 0.05000000074505806)
      Game1.playSound("crafting");
    else
      Game1.playSound("dirtyHit");
  }

  public void GoldenParrotBounce(TemporaryAnimatedSprite sprite)
  {
    sprite.motion.Y = Utility.RandomFloat(-3f, -5f);
    Game1.playSound("dirtyHit");
    this.location.temporarySprites.Add(new TemporaryAnimatedSprite(12, sprite.position, Color.White, flipped: Game1.random.NextBool(), animationInterval: 50f));
  }

  /// <inheritdoc />
  public override bool tickUpdate(GameTime time)
  {
    Game1.UpdateGameClock(time);
    this.location.updateWater(time);
    if (this.whichEvent.Value == 15)
      ++Game1.viewport.Y;
    this.cutsceneLengthTimer -= time.ElapsedGameTime.Milliseconds;
    if (this.timerSinceFade > 0)
    {
      this.timerSinceFade -= time.ElapsedGameTime.Milliseconds;
      Game1.globalFade = true;
      Game1.fadeToBlackAlpha = 1f;
      return this.timerSinceFade <= 0;
    }
    if (this.cutsceneLengthTimer <= 0 && !Game1.globalFade)
      Game1.globalFadeToBlack(new Game1.afterFadeFunction(this.endEvent), 0.01f);
    this.soundTimer -= time.ElapsedGameTime.Milliseconds;
    if (this.soundTimer <= 0 && this.sound != null)
    {
      Game1.playSound(this.sound);
      this.soundTimer = this.soundInterval;
    }
    return false;
  }

  public override void makeChangesToLocation()
  {
    base.makeChangesToLocation();
    if (this.whichEvent.Value != 15 || !Game1.IsMasterGame)
      return;
    ParrotUpgradePerch.ActivateGoldenParrot();
  }

  public void endEvent()
  {
    this.location.cleanupBeforePlayerExit();
    if (this.preEventLocation != null)
    {
      Game1.currentLocation = this.preEventLocation;
      Game1.currentLocation.resetForPlayerEntry();
      this.preEventLocation = (GameLocation) null;
    }
    this.timerSinceFade = 1500;
    Game1.isRaining = this.wasRaining;
    Game1.getFarm().temporarySprites.Clear();
  }
}
