// Decompiled with JetBrains decompiler
// Type: StardewValley.TemporaryAnimatedSprite
// Assembly: Stardew Valley, Version=1.6.15.24356, Culture=neutral, PublicKeyToken=null
// MVID: AA1F513A-94F0-4EF6-A35F-2D5B4A3721BF
// Assembly location: D:\SteamLibrary\steamapps\common\Stardew Valley\Stardew Valley.dll
// XML documentation location: D:\SteamLibrary\steamapps\common\Stardew Valley\Stardew Valley.xml

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Netcode;
using StardewValley.BellsAndWhistles;
using StardewValley.Extensions;
using StardewValley.GameData;
using StardewValley.ItemTypeDefinitions;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;

#nullable disable
namespace StardewValley;

public class TemporaryAnimatedSprite
{
  public const int FireworkType_Heart = 0;
  public const int FireworkType_Star = 1;
  public const int FireworkType_Junimo = 2;
  public static float[] FireworksLifetimeMultiplier = new float[3]
  {
    1f,
    1f,
    1.3f
  };
  public static Color[] FireworksColors = new Color[3]
  {
    new Color(252, 56, 37),
    new Color(144 /*0x90*/, 51, 237),
    new Color(92, 237, 213)
  };
  public static Vector2[][] FireworksLights = new Vector2[3][]
  {
    new Vector2[1]{ new Vector2(0.0f, 0.0f) },
    new Vector2[1]{ new Vector2(0.0f, 0.0f) },
    new Vector2[2]
    {
      new Vector2(-2.5f, 0.0f),
      new Vector2(2.5f, 0.0f)
    }
  };
  public static Vector2[][] FireworksPoints = new Vector2[3][]
  {
    new Vector2[14]
    {
      new Vector2(0.0f, -3f),
      new Vector2(2f, -5f),
      new Vector2(4f, -5f),
      new Vector2(6f, -3f),
      new Vector2(6f, -1f),
      new Vector2(4f, 1f),
      new Vector2(2f, 3f),
      new Vector2(0.0f, 5f),
      new Vector2(-2f, 3f),
      new Vector2(-4f, 1f),
      new Vector2(-6f, -1f),
      new Vector2(-6f, -3f),
      new Vector2(-4f, -5f),
      new Vector2(-2f, -5f)
    },
    new Vector2[20]
    {
      new Vector2(0.0f, -6f),
      new Vector2(1f, -4f),
      new Vector2(2f, -2f),
      new Vector2(4f, -2f),
      new Vector2(6f, -2f),
      new Vector2(4f, 0.0f),
      new Vector2(2f, 1f),
      new Vector2(3f, 3f),
      new Vector2(4f, 5f),
      new Vector2(2f, 4f),
      new Vector2(0.0f, 3f),
      new Vector2(-2f, 4f),
      new Vector2(-4f, 5f),
      new Vector2(-3f, 3f),
      new Vector2(-2f, 1f),
      new Vector2(-4f, 0.0f),
      new Vector2(-6f, -2f),
      new Vector2(-4f, -2f),
      new Vector2(-2f, -2f),
      new Vector2(-1f, -4f)
    },
    new Vector2[31 /*0x1F*/]
    {
      new Vector2(-1f, -8f),
      new Vector2(0.0f, -6f),
      new Vector2(0.0f, -4f),
      new Vector2(2f, -4f),
      new Vector2(4f, -4f),
      new Vector2(6f, -2f),
      new Vector2(8f, -1f),
      new Vector2(9f, -3f),
      new Vector2(8f, -5f),
      new Vector2(6f, 0.0f),
      new Vector2(6f, 2f),
      new Vector2(3f, 2f),
      new Vector2(3f, 1f),
      new Vector2(5f, 4f),
      new Vector2(3f, 5f),
      new Vector2(3f, 7f),
      new Vector2(1f, 5f),
      new Vector2(-1f, 5f),
      new Vector2(-3f, 7f),
      new Vector2(-3f, 5f),
      new Vector2(-5f, 4f),
      new Vector2(-3f, 2f),
      new Vector2(-3f, 1f),
      new Vector2(-6f, 2f),
      new Vector2(-6f, 0.0f),
      new Vector2(-8f, -5f),
      new Vector2(-9f, -3f),
      new Vector2(-8f, -1f),
      new Vector2(-6f, -2f),
      new Vector2(-4f, -4f),
      new Vector2(-2f, -4f)
    }
  };
  public float timer;
  public float interval = 200f;
  public int currentParentTileIndex;
  public int oldCurrentParentTileIndex;
  public int initialParentTileIndex;
  public int totalNumberOfLoops;
  public int currentNumberOfLoops;
  public int xStopCoordinate = -1;
  public int yStopCoordinate = -1;
  public int animationLength;
  public int bombRadius;
  public int pingPongMotion = 1;
  public int bombDamage = -1;
  public int fireworkType = -1;
  public bool flicker;
  public bool timeBasedMotion;
  public bool overrideLocationDestroy;
  public bool pingPong;
  public bool holdLastFrame;
  public bool pulse;
  public int extraInfoForEndBehavior;
  /// <summary>The unique ID of the light source to add when rendering this sprite, or <c>null</c> for no light source.</summary>
  public string lightId;
  public int id;
  public bool bigCraftable;
  public bool swordswipe;
  public bool flash;
  public bool flipped;
  public bool verticalFlipped;
  public bool local;
  public bool hasLit;
  public bool xPeriodic;
  public bool yPeriodic;
  public bool destroyable = true;
  public bool paused;
  public bool stopAcceleratingWhenVelocityIsZero;
  public bool positionFollowsAttachedCharacter;
  public bool usePreciseTiming;
  public float rotation;
  public float alpha = 1f;
  public float alphaFade;
  public float layerDepth = -1f;
  public float scale = 1f;
  public float scaleChange;
  public float scaleChangeChange;
  public float rotationChange;
  public float lightRadius;
  public float xPeriodicRange;
  public float yPeriodicRange;
  public float xPeriodicLoopTime;
  public float yPeriodicLoopTime;
  public float shakeIntensityChange;
  public float shakeIntensity;
  public float pulseTime;
  public float pulseAmount = 1.1f;
  public float alphaFadeFade;
  public int lightFade = -1;
  public float afterAccelStopMotionX;
  public float afterAccelStopMotionY;
  public float layerDepthOffset;
  public Vector2 position;
  public Vector2 sourceRectStartingPos;
  protected GameLocation parent;
  public string textureName;
  public Texture2D texture;
  public Rectangle sourceRect;
  public Color color = Color.White;
  public Color lightcolor = Color.White;
  public Farmer owner;
  public Vector2 motion = Vector2.Zero;
  public Vector2 acceleration = Vector2.Zero;
  public Vector2 accelerationChange = Vector2.Zero;
  public Vector2 initialPosition;
  public Vector2 vectorScale;
  public int delayBeforeAnimationStart;
  public int ticksBeforeAnimationStart;
  public string startSound;
  public string endSound;
  public string text;
  public TemporaryAnimatedSprite.endBehavior endFunction;
  public TemporaryAnimatedSprite.endBehavior reachedStopCoordinate;
  public Action<TemporaryAnimatedSprite> reachedStopCoordinateSprite;
  public TemporaryAnimatedSprite parentSprite;
  public Character attachedCharacter;
  private float pulseTimer;
  private float originalScale;
  public bool drawAboveAlwaysFront;
  public bool dontClearOnAreaEntry;
  private Stopwatch stopWatch;
  private long previousStopwatchTime;
  protected bool _pooled;
  public static List<TemporaryAnimatedSprite> _pool;
  private float totalTimer;

  public static float GetFireworkLifetimeMultiplier(int id)
  {
    return TemporaryAnimatedSprite.FireworksLifetimeMultiplier[id];
  }

  public static Color GetFireworkColor(int id) => TemporaryAnimatedSprite.FireworksColors[id];

  public static Vector2[] GetFireworkLights(int id) => TemporaryAnimatedSprite.FireworksLights[id];

  public static Vector2[] GetFireworkPoints(int id) => TemporaryAnimatedSprite.FireworksPoints[id];

  public bool Pooled => this._pooled;

  public Vector2 Position
  {
    get => this.position;
    set => this.position = value;
  }

  public Texture2D Texture => this.texture;

  public GameLocation Parent
  {
    get => this.parent;
    set => this.parent = value;
  }

  public TemporaryAnimatedSprite getClone()
  {
    TemporaryAnimatedSprite temporaryAnimatedSprite = TemporaryAnimatedSprite.GetTemporaryAnimatedSprite();
    temporaryAnimatedSprite.texture = this.texture;
    temporaryAnimatedSprite.interval = this.interval;
    temporaryAnimatedSprite.currentParentTileIndex = this.currentParentTileIndex;
    temporaryAnimatedSprite.oldCurrentParentTileIndex = this.oldCurrentParentTileIndex;
    temporaryAnimatedSprite.initialParentTileIndex = this.initialParentTileIndex;
    temporaryAnimatedSprite.totalNumberOfLoops = this.totalNumberOfLoops;
    temporaryAnimatedSprite.currentNumberOfLoops = this.currentNumberOfLoops;
    temporaryAnimatedSprite.xStopCoordinate = this.xStopCoordinate;
    temporaryAnimatedSprite.yStopCoordinate = this.yStopCoordinate;
    temporaryAnimatedSprite.animationLength = this.animationLength;
    temporaryAnimatedSprite.bombRadius = this.bombRadius;
    temporaryAnimatedSprite.bombDamage = this.bombDamage;
    temporaryAnimatedSprite.pingPongMotion = this.pingPongMotion;
    temporaryAnimatedSprite.fireworkType = this.fireworkType;
    temporaryAnimatedSprite.flicker = this.flicker;
    temporaryAnimatedSprite.timeBasedMotion = this.timeBasedMotion;
    temporaryAnimatedSprite.overrideLocationDestroy = this.overrideLocationDestroy;
    temporaryAnimatedSprite.pingPong = this.pingPong;
    temporaryAnimatedSprite.holdLastFrame = this.holdLastFrame;
    temporaryAnimatedSprite.extraInfoForEndBehavior = this.extraInfoForEndBehavior;
    temporaryAnimatedSprite.lightId = this.lightId;
    temporaryAnimatedSprite.acceleration = this.acceleration;
    temporaryAnimatedSprite.accelerationChange = this.accelerationChange;
    temporaryAnimatedSprite.alpha = this.alpha;
    temporaryAnimatedSprite.alphaFade = this.alphaFade;
    temporaryAnimatedSprite.attachedCharacter = this.attachedCharacter;
    temporaryAnimatedSprite.bigCraftable = this.bigCraftable;
    temporaryAnimatedSprite.color = this.color;
    temporaryAnimatedSprite.delayBeforeAnimationStart = this.delayBeforeAnimationStart;
    temporaryAnimatedSprite.ticksBeforeAnimationStart = this.ticksBeforeAnimationStart;
    temporaryAnimatedSprite.destroyable = this.destroyable;
    temporaryAnimatedSprite.endFunction = this.endFunction;
    temporaryAnimatedSprite.endSound = this.endSound;
    temporaryAnimatedSprite.flash = this.flash;
    temporaryAnimatedSprite.flipped = this.flipped;
    temporaryAnimatedSprite.hasLit = this.hasLit;
    temporaryAnimatedSprite.id = this.id;
    temporaryAnimatedSprite.initialPosition = this.initialPosition;
    temporaryAnimatedSprite.lightFade = this.lightFade;
    temporaryAnimatedSprite.local = this.local;
    temporaryAnimatedSprite.motion = this.motion;
    temporaryAnimatedSprite.owner = this.owner;
    temporaryAnimatedSprite.parent = this.parent;
    temporaryAnimatedSprite.parentSprite = this.parentSprite;
    temporaryAnimatedSprite.position = this.position;
    temporaryAnimatedSprite.rotation = this.rotation;
    temporaryAnimatedSprite.rotationChange = this.rotationChange;
    temporaryAnimatedSprite.scale = this.scale;
    temporaryAnimatedSprite.scaleChange = this.scaleChange;
    temporaryAnimatedSprite.scaleChangeChange = this.scaleChangeChange;
    temporaryAnimatedSprite.shakeIntensity = this.shakeIntensity;
    temporaryAnimatedSprite.shakeIntensityChange = this.shakeIntensityChange;
    temporaryAnimatedSprite.sourceRect = this.sourceRect;
    temporaryAnimatedSprite.sourceRectStartingPos = this.sourceRectStartingPos;
    temporaryAnimatedSprite.startSound = this.startSound;
    temporaryAnimatedSprite.timeBasedMotion = this.timeBasedMotion;
    temporaryAnimatedSprite.verticalFlipped = this.verticalFlipped;
    temporaryAnimatedSprite.xPeriodic = this.xPeriodic;
    temporaryAnimatedSprite.xPeriodicLoopTime = this.xPeriodicLoopTime;
    temporaryAnimatedSprite.xPeriodicRange = this.xPeriodicRange;
    temporaryAnimatedSprite.yPeriodic = this.yPeriodic;
    temporaryAnimatedSprite.yPeriodicLoopTime = this.yPeriodicLoopTime;
    temporaryAnimatedSprite.yPeriodicRange = this.yPeriodicRange;
    temporaryAnimatedSprite.yStopCoordinate = this.yStopCoordinate;
    temporaryAnimatedSprite.totalNumberOfLoops = this.totalNumberOfLoops;
    temporaryAnimatedSprite.stopAcceleratingWhenVelocityIsZero = this.stopAcceleratingWhenVelocityIsZero;
    temporaryAnimatedSprite.afterAccelStopMotionX = this.afterAccelStopMotionX;
    temporaryAnimatedSprite.afterAccelStopMotionY = this.afterAccelStopMotionY;
    temporaryAnimatedSprite.layerDepthOffset = this.layerDepthOffset;
    temporaryAnimatedSprite.positionFollowsAttachedCharacter = this.positionFollowsAttachedCharacter;
    temporaryAnimatedSprite.dontClearOnAreaEntry = this.dontClearOnAreaEntry;
    return temporaryAnimatedSprite;
  }

  public virtual void Pool()
  {
    this.timer = 0.0f;
    this.interval = 200f;
    this.currentParentTileIndex = 0;
    this.oldCurrentParentTileIndex = 0;
    this.initialParentTileIndex = 0;
    this.totalNumberOfLoops = 0;
    this.currentNumberOfLoops = 0;
    this.xStopCoordinate = -1;
    this.yStopCoordinate = -1;
    this.animationLength = 0;
    this.bombRadius = 0;
    this.pingPongMotion = 1;
    this.bombDamage = -1;
    this.fireworkType = -1;
    this.flicker = false;
    this.timeBasedMotion = false;
    this.overrideLocationDestroy = false;
    this.pingPong = false;
    this.holdLastFrame = false;
    this.pulse = false;
    this.extraInfoForEndBehavior = 0;
    this.lightId = (string) null;
    this.bigCraftable = false;
    this.swordswipe = false;
    this.flash = false;
    this.flipped = false;
    this.verticalFlipped = false;
    this.local = false;
    this.hasLit = false;
    this.xPeriodic = false;
    this.yPeriodic = false;
    this.destroyable = true;
    this.paused = false;
    this.stopAcceleratingWhenVelocityIsZero = false;
    this.positionFollowsAttachedCharacter = false;
    this.rotation = 0.0f;
    this.alpha = 1f;
    this.alphaFade = 0.0f;
    this.layerDepth = -1f;
    this.scale = 1f;
    this.scaleChange = 0.0f;
    this.scaleChangeChange = 0.0f;
    this.rotationChange = 0.0f;
    this.id = 0;
    this.lightRadius = 0.0f;
    this.xPeriodicRange = 0.0f;
    this.yPeriodicRange = 0.0f;
    this.xPeriodicLoopTime = 0.0f;
    this.yPeriodicLoopTime = 0.0f;
    this.shakeIntensityChange = 0.0f;
    this.shakeIntensity = 0.0f;
    this.pulseTime = 0.0f;
    this.pulseAmount = 1.1f;
    this.alphaFadeFade = 0.0f;
    this.lightFade = -1;
    this.layerDepthOffset = 0.0f;
    this.afterAccelStopMotionX = 0.0f;
    this.afterAccelStopMotionY = 0.0f;
    this.position = Vector2.Zero;
    this.sourceRectStartingPos = Vector2.Zero;
    this.parent = (GameLocation) null;
    this.textureName = (string) null;
    this.texture = (Texture2D) null;
    this.sourceRect = Rectangle.Empty;
    this.color = Color.White;
    this.lightcolor = Color.White;
    this.owner = (Farmer) null;
    this.motion = Vector2.Zero;
    this.acceleration = Vector2.Zero;
    this.accelerationChange = Vector2.Zero;
    this.initialPosition = Vector2.Zero;
    this.delayBeforeAnimationStart = 0;
    this.ticksBeforeAnimationStart = 0;
    this.startSound = (string) null;
    this.endSound = (string) null;
    this.text = (string) null;
    this.endFunction = (TemporaryAnimatedSprite.endBehavior) null;
    this.reachedStopCoordinate = (TemporaryAnimatedSprite.endBehavior) null;
    this.reachedStopCoordinateSprite = (Action<TemporaryAnimatedSprite>) null;
    this.parentSprite = (TemporaryAnimatedSprite) null;
    this.attachedCharacter = (Character) null;
    this.pulseTimer = 0.0f;
    this.originalScale = 0.0f;
    this.drawAboveAlwaysFront = false;
    this.dontClearOnAreaEntry = false;
    TemporaryAnimatedSprite._pool.Add(this);
  }

  public static TemporaryAnimatedSprite GetTemporaryAnimatedSprite()
  {
    TemporaryAnimatedSprite temporaryAnimatedSprite1 = (TemporaryAnimatedSprite) null;
    if (TemporaryAnimatedSprite._pool == null)
    {
      TemporaryAnimatedSprite._pool = new List<TemporaryAnimatedSprite>();
      for (int index = 0; index < 256 /*0x0100*/; ++index)
      {
        TemporaryAnimatedSprite temporaryAnimatedSprite2 = new TemporaryAnimatedSprite()
        {
          _pooled = true
        };
        TemporaryAnimatedSprite._pool.Add(temporaryAnimatedSprite2);
      }
    }
    if (TemporaryAnimatedSprite._pool.Count > 0)
    {
      temporaryAnimatedSprite1 = TemporaryAnimatedSprite._pool[TemporaryAnimatedSprite._pool.Count - 1];
      TemporaryAnimatedSprite._pool.RemoveAt(TemporaryAnimatedSprite._pool.Count - 1);
    }
    if (temporaryAnimatedSprite1 == null)
      temporaryAnimatedSprite1 = new TemporaryAnimatedSprite();
    return temporaryAnimatedSprite1;
  }

  public TemporaryAnimatedSprite()
  {
  }

  public static TemporaryAnimatedSprite GetTemporaryAnimatedSprite(
    int initialParentTileIndex,
    float animationInterval,
    int animationLength,
    int numberOfLoops,
    Vector2 position,
    bool flicker,
    bool flipped)
  {
    TemporaryAnimatedSprite temporaryAnimatedSprite = TemporaryAnimatedSprite.GetTemporaryAnimatedSprite();
    if (temporaryAnimatedSprite.initialParentTileIndex == -1)
    {
      temporaryAnimatedSprite.swordswipe = true;
      temporaryAnimatedSprite.currentParentTileIndex = 0;
    }
    else
      temporaryAnimatedSprite.currentParentTileIndex = initialParentTileIndex;
    temporaryAnimatedSprite.initialParentTileIndex = initialParentTileIndex;
    temporaryAnimatedSprite.interval = animationInterval;
    temporaryAnimatedSprite.totalNumberOfLoops = numberOfLoops;
    temporaryAnimatedSprite.position = position;
    temporaryAnimatedSprite.animationLength = animationLength;
    temporaryAnimatedSprite.flicker = flicker;
    temporaryAnimatedSprite.flipped = flipped;
    return temporaryAnimatedSprite;
  }

  public TemporaryAnimatedSprite(
    int initialParentTileIndex,
    float animationInterval,
    int animationLength,
    int numberOfLoops,
    Vector2 position,
    bool flicker,
    bool flipped)
  {
    if (initialParentTileIndex == -1)
    {
      this.swordswipe = true;
      this.currentParentTileIndex = 0;
    }
    else
      this.currentParentTileIndex = initialParentTileIndex;
    this.initialParentTileIndex = initialParentTileIndex;
    this.interval = animationInterval;
    this.totalNumberOfLoops = numberOfLoops;
    this.position = position;
    this.animationLength = animationLength;
    this.flicker = flicker;
    this.flipped = flipped;
  }

  public static TemporaryAnimatedSprite GetTemporaryAnimatedSprite(
    int rowInAnimationTexture,
    Vector2 position,
    Color color,
    int animationLength = 8,
    bool flipped = false,
    float animationInterval = 100f,
    int numberOfLoops = 0,
    int sourceRectWidth = -1,
    float layerDepth = -1f,
    int sourceRectHeight = -1,
    int delay = 0)
  {
    TemporaryAnimatedSprite temporaryAnimatedSprite = TemporaryAnimatedSprite.GetTemporaryAnimatedSprite("TileSheets\\animations", new Rectangle(0, rowInAnimationTexture * 64 /*0x40*/, sourceRectWidth, sourceRectHeight), animationInterval, animationLength, numberOfLoops, position, false, flipped, layerDepth, 0.0f, color, 1f, 0.0f, 0.0f, 0.0f);
    if (sourceRectWidth == -1)
    {
      sourceRectWidth = 64 /*0x40*/;
      temporaryAnimatedSprite.sourceRect.Width = 64 /*0x40*/;
    }
    if (sourceRectHeight == -1)
    {
      sourceRectHeight = 64 /*0x40*/;
      temporaryAnimatedSprite.sourceRect.Height = 64 /*0x40*/;
    }
    if ((double) temporaryAnimatedSprite.layerDepth == -1.0)
      temporaryAnimatedSprite.layerDepth = (float) (((double) temporaryAnimatedSprite.position.Y + 32.0) / 10000.0);
    temporaryAnimatedSprite.delayBeforeAnimationStart = delay;
    return temporaryAnimatedSprite;
  }

  public TemporaryAnimatedSprite(
    int rowInAnimationTexture,
    Vector2 position,
    Color color,
    int animationLength = 8,
    bool flipped = false,
    float animationInterval = 100f,
    int numberOfLoops = 0,
    int sourceRectWidth = -1,
    float layerDepth = -1f,
    int sourceRectHeight = -1,
    int delay = 0)
    : this("TileSheets\\animations", new Rectangle(0, rowInAnimationTexture * 64 /*0x40*/, sourceRectWidth, sourceRectHeight), animationInterval, animationLength, numberOfLoops, position, false, flipped, layerDepth, 0.0f, color, 1f, 0.0f, 0.0f, 0.0f)
  {
    if (sourceRectWidth == -1)
    {
      sourceRectWidth = 64 /*0x40*/;
      this.sourceRect.Width = 64 /*0x40*/;
    }
    if (sourceRectHeight == -1)
    {
      sourceRectHeight = 64 /*0x40*/;
      this.sourceRect.Height = 64 /*0x40*/;
    }
    if ((double) layerDepth == -1.0)
      layerDepth = (float) (((double) position.Y + 32.0) / 10000.0);
    this.delayBeforeAnimationStart = delay;
  }

  public static TemporaryAnimatedSprite GetTemporaryAnimatedSprite(
    int initialParentTileIndex,
    float animationInterval,
    int animationLength,
    int numberOfLoops,
    Vector2 position,
    bool flicker,
    bool flipped,
    bool verticalFlipped,
    float rotation)
  {
    TemporaryAnimatedSprite temporaryAnimatedSprite = TemporaryAnimatedSprite.GetTemporaryAnimatedSprite(initialParentTileIndex, animationInterval, animationLength, numberOfLoops, position, flicker, flipped);
    temporaryAnimatedSprite.rotation = rotation;
    temporaryAnimatedSprite.verticalFlipped = verticalFlipped;
    return temporaryAnimatedSprite;
  }

  public TemporaryAnimatedSprite(
    int initialParentTileIndex,
    float animationInterval,
    int animationLength,
    int numberOfLoops,
    Vector2 position,
    bool flicker,
    bool flipped,
    bool verticalFlipped,
    float rotation)
    : this(initialParentTileIndex, animationInterval, animationLength, numberOfLoops, position, flicker, flipped)
  {
    this.rotation = rotation;
    this.verticalFlipped = verticalFlipped;
  }

  public static TemporaryAnimatedSprite GetTemporaryAnimatedSprite(
    int initialParentTileIndex,
    float animationInterval,
    int animationLength,
    int numberOfLoops,
    Vector2 position,
    bool flicker,
    bool bigCraftable,
    bool flipped)
  {
    TemporaryAnimatedSprite temporaryAnimatedSprite = TemporaryAnimatedSprite.GetTemporaryAnimatedSprite(initialParentTileIndex, animationInterval, animationLength, numberOfLoops, position, flicker, flipped);
    temporaryAnimatedSprite.bigCraftable = bigCraftable;
    if (temporaryAnimatedSprite.bigCraftable)
      temporaryAnimatedSprite.position.Y -= 64f;
    return temporaryAnimatedSprite;
  }

  public TemporaryAnimatedSprite(
    int initialParentTileIndex,
    float animationInterval,
    int animationLength,
    int numberOfLoops,
    Vector2 position,
    bool flicker,
    bool bigCraftable,
    bool flipped)
    : this(initialParentTileIndex, animationInterval, animationLength, numberOfLoops, position, flicker, flipped)
  {
    this.bigCraftable = bigCraftable;
    if (!bigCraftable)
      return;
    this.position.Y -= 64f;
  }

  public TemporaryAnimatedSprite GetTemporaryAnimatedSprite(
    string textureName,
    Rectangle sourceRect,
    float animationInterval,
    int animationLength,
    int numberOfLoops,
    Vector2 position,
    bool flicker,
    bool flipped)
  {
    TemporaryAnimatedSprite temporaryAnimatedSprite = TemporaryAnimatedSprite.GetTemporaryAnimatedSprite(0, animationInterval, animationLength, numberOfLoops, position, flicker, flipped);
    temporaryAnimatedSprite.textureName = textureName;
    temporaryAnimatedSprite.loadTexture();
    temporaryAnimatedSprite.sourceRect = sourceRect;
    temporaryAnimatedSprite.sourceRectStartingPos = new Vector2((float) sourceRect.X, (float) sourceRect.Y);
    temporaryAnimatedSprite.initialPosition = position;
    return temporaryAnimatedSprite;
  }

  public TemporaryAnimatedSprite(
    string textureName,
    Rectangle sourceRect,
    float animationInterval,
    int animationLength,
    int numberOfLoops,
    Vector2 position,
    bool flicker,
    bool flipped)
    : this(0, animationInterval, animationLength, numberOfLoops, position, flicker, flipped)
  {
    this.textureName = textureName;
    this.loadTexture();
    this.sourceRect = sourceRect;
    this.sourceRectStartingPos = new Vector2((float) sourceRect.X, (float) sourceRect.Y);
    this.initialPosition = position;
  }

  public static TemporaryAnimatedSprite GetTemporaryAnimatedSprite(
    string textureName,
    Rectangle sourceRect,
    float animationInterval,
    int animationLength,
    int numberOfLoops,
    Vector2 position,
    bool flicker,
    bool flipped,
    float layerDepth,
    float alphaFade,
    Color color,
    float scale,
    float scaleChange,
    float rotation,
    float rotationChange,
    bool local = false)
  {
    TemporaryAnimatedSprite temporaryAnimatedSprite = TemporaryAnimatedSprite.GetTemporaryAnimatedSprite(0, animationInterval, animationLength, numberOfLoops, position, flicker, flipped);
    temporaryAnimatedSprite.textureName = textureName;
    temporaryAnimatedSprite.loadTexture();
    temporaryAnimatedSprite.sourceRect = sourceRect;
    temporaryAnimatedSprite.sourceRectStartingPos = new Vector2((float) sourceRect.X, (float) sourceRect.Y);
    temporaryAnimatedSprite.layerDepth = layerDepth;
    temporaryAnimatedSprite.alphaFade = Math.Max(0.0f, alphaFade);
    temporaryAnimatedSprite.color = color;
    temporaryAnimatedSprite.scale = scale;
    temporaryAnimatedSprite.scaleChange = scaleChange;
    temporaryAnimatedSprite.rotation = rotation;
    temporaryAnimatedSprite.rotationChange = rotationChange;
    temporaryAnimatedSprite.local = local;
    temporaryAnimatedSprite.initialPosition = position;
    return temporaryAnimatedSprite;
  }

  public TemporaryAnimatedSprite(
    string textureName,
    Rectangle sourceRect,
    float animationInterval,
    int animationLength,
    int numberOfLoops,
    Vector2 position,
    bool flicker,
    bool flipped,
    float layerDepth,
    float alphaFade,
    Color color,
    float scale,
    float scaleChange,
    float rotation,
    float rotationChange,
    bool local = false)
    : this(0, animationInterval, animationLength, numberOfLoops, position, flicker, flipped)
  {
    this.textureName = textureName;
    this.loadTexture();
    this.sourceRect = sourceRect;
    this.sourceRectStartingPos = new Vector2((float) sourceRect.X, (float) sourceRect.Y);
    this.layerDepth = layerDepth;
    this.alphaFade = Math.Max(0.0f, alphaFade);
    this.color = color;
    this.scale = scale;
    this.scaleChange = scaleChange;
    this.rotation = rotation;
    this.rotationChange = rotationChange;
    this.local = local;
    this.initialPosition = position;
  }

  public virtual void CopyAppearanceFromItemId(string itemId, int offset = 0)
  {
    this.scale = 4f * this.scale;
    ParsedItemData dataOrErrorItem = ItemRegistry.GetDataOrErrorItem(itemId);
    this.textureName = dataOrErrorItem.TextureName;
    this.loadTexture();
    this.sourceRect = dataOrErrorItem.GetSourceRect(offset);
    this.sourceRectStartingPos = Utility.PointToVector2(this.sourceRect.Location);
    this.currentParentTileIndex = 0;
    this.initialParentTileIndex = 0;
  }

  public static TemporaryAnimatedSprite GetTemporaryAnimatedSprite(
    string textureName,
    Rectangle sourceRect,
    Vector2 position,
    bool flipped,
    float alphaFade,
    Color color)
  {
    TemporaryAnimatedSprite temporaryAnimatedSprite = TemporaryAnimatedSprite.GetTemporaryAnimatedSprite(0, 999999f, 1, 0, position, false, flipped);
    temporaryAnimatedSprite.textureName = textureName;
    temporaryAnimatedSprite.loadTexture();
    temporaryAnimatedSprite.sourceRect = sourceRect;
    temporaryAnimatedSprite.sourceRectStartingPos = new Vector2((float) sourceRect.X, (float) sourceRect.Y);
    temporaryAnimatedSprite.initialPosition = position;
    temporaryAnimatedSprite.alphaFade = Math.Max(0.0f, alphaFade);
    temporaryAnimatedSprite.color = color;
    return temporaryAnimatedSprite;
  }

  public TemporaryAnimatedSprite(
    string textureName,
    Rectangle sourceRect,
    Vector2 position,
    bool flipped,
    float alphaFade,
    Color color)
    : this(0, 999999f, 1, 0, position, false, flipped)
  {
    this.textureName = textureName;
    this.loadTexture();
    this.sourceRect = sourceRect;
    this.sourceRectStartingPos = new Vector2((float) sourceRect.X, (float) sourceRect.Y);
    this.initialPosition = position;
    this.alphaFade = Math.Max(0.0f, alphaFade);
    this.color = color;
  }

  public static TemporaryAnimatedSprite GetTemporaryAnimatedSprite(
    int initialParentTileIndex,
    float animationInterval,
    int animationLength,
    int numberOfLoops,
    Vector2 position,
    bool flicker,
    bool flipped,
    GameLocation parent,
    Farmer owner)
  {
    TemporaryAnimatedSprite temporaryAnimatedSprite = TemporaryAnimatedSprite.GetTemporaryAnimatedSprite(initialParentTileIndex, animationInterval, animationLength, numberOfLoops, position, flicker, flipped);
    temporaryAnimatedSprite.position.X = (float) (int) temporaryAnimatedSprite.position.X;
    temporaryAnimatedSprite.position.Y = (float) (int) temporaryAnimatedSprite.position.Y;
    temporaryAnimatedSprite.parent = parent;
    switch (temporaryAnimatedSprite.initialParentTileIndex)
    {
      case 286:
        temporaryAnimatedSprite.bombRadius = 3;
        break;
      case 287:
        temporaryAnimatedSprite.bombRadius = 5;
        break;
      case 288:
        temporaryAnimatedSprite.bombRadius = 7;
        break;
    }
    temporaryAnimatedSprite.owner = owner;
    return temporaryAnimatedSprite;
  }

  /// <summary>Construct an instance for a bomb.</summary>
  public TemporaryAnimatedSprite(
    int initialParentTileIndex,
    float animationInterval,
    int animationLength,
    int numberOfLoops,
    Vector2 position,
    bool flicker,
    bool flipped,
    GameLocation parent,
    Farmer owner)
    : this(initialParentTileIndex, animationInterval, animationLength, numberOfLoops, position, flicker, flipped)
  {
    this.position.X = (float) (int) this.position.X;
    this.position.Y = (float) (int) this.position.Y;
    this.parent = parent;
    switch (initialParentTileIndex)
    {
      case 286:
        this.bombRadius = 3;
        break;
      case 287:
        this.bombRadius = 5;
        break;
      case 288:
        this.bombRadius = 7;
        break;
    }
    this.owner = owner;
  }

  private void loadTexture()
  {
    switch (this.textureName)
    {
      case null:
        this.texture = (Texture2D) null;
        break;
      case "":
        this.texture = Game1.staminaRect;
        break;
      default:
        this.texture = Game1.content.Load<Texture2D>(this.textureName);
        break;
    }
  }

  public void Read(BinaryReader reader, GameLocation location)
  {
    this.timer = 0.0f;
    BitArray bitArray = reader.ReadBitArray();
    int index1 = 0;
    int num1 = index1 + 1;
    if (bitArray[index1])
      this.interval = reader.ReadSingle();
    int index2 = num1;
    int num2 = index2 + 1;
    if (bitArray[index2])
      this.currentParentTileIndex = reader.ReadInt32();
    int index3 = num2;
    int num3 = index3 + 1;
    if (bitArray[index3])
      this.oldCurrentParentTileIndex = reader.ReadInt32();
    int index4 = num3;
    int num4 = index4 + 1;
    if (bitArray[index4])
      this.initialParentTileIndex = reader.ReadInt32();
    int index5 = num4;
    int num5 = index5 + 1;
    if (bitArray[index5])
      this.totalNumberOfLoops = reader.ReadInt32();
    int index6 = num5;
    int num6 = index6 + 1;
    if (bitArray[index6])
      this.currentNumberOfLoops = reader.ReadInt32();
    int index7 = num6;
    int num7 = index7 + 1;
    if (bitArray[index7])
      this.xStopCoordinate = reader.ReadInt32();
    int index8 = num7;
    int num8 = index8 + 1;
    if (bitArray[index8])
      this.yStopCoordinate = reader.ReadInt32();
    int index9 = num8;
    int num9 = index9 + 1;
    if (bitArray[index9])
      this.animationLength = reader.ReadInt32();
    int index10 = num9;
    int num10 = index10 + 1;
    if (bitArray[index10])
      this.bombRadius = reader.ReadInt32();
    int index11 = num10;
    int num11 = index11 + 1;
    if (bitArray[index11])
      this.bombDamage = reader.ReadInt32();
    int index12 = num11;
    int num12 = index12 + 1;
    if (bitArray[index12])
      this.pingPongMotion = reader.ReadInt32();
    int index13 = num12;
    int num13 = index13 + 1;
    if (bitArray[index13])
      this.fireworkType = reader.ReadInt32();
    int index14 = num13;
    int num14 = index14 + 1;
    if (bitArray[index14])
      this.flicker = reader.ReadBoolean();
    int index15 = num14;
    int num15 = index15 + 1;
    if (bitArray[index15])
      this.timeBasedMotion = reader.ReadBoolean();
    int index16 = num15;
    int num16 = index16 + 1;
    if (bitArray[index16])
      this.overrideLocationDestroy = reader.ReadBoolean();
    int index17 = num16;
    int num17 = index17 + 1;
    if (bitArray[index17])
      this.pingPong = reader.ReadBoolean();
    int index18 = num17;
    int num18 = index18 + 1;
    if (bitArray[index18])
      this.holdLastFrame = reader.ReadBoolean();
    int index19 = num18;
    int num19 = index19 + 1;
    if (bitArray[index19])
      this.pulse = reader.ReadBoolean();
    int index20 = num19;
    int num20 = index20 + 1;
    if (bitArray[index20])
      this.extraInfoForEndBehavior = reader.ReadInt32();
    int index21 = num20;
    int num21 = index21 + 1;
    if (bitArray[index21])
      this.lightId = reader.ReadString();
    int index22 = num21;
    int num22 = index22 + 1;
    if (bitArray[index22])
      this.bigCraftable = reader.ReadBoolean();
    int index23 = num22;
    int num23 = index23 + 1;
    if (bitArray[index23])
      this.swordswipe = reader.ReadBoolean();
    int index24 = num23;
    int num24 = index24 + 1;
    if (bitArray[index24])
      this.flash = reader.ReadBoolean();
    int index25 = num24;
    int num25 = index25 + 1;
    if (bitArray[index25])
      this.flipped = reader.ReadBoolean();
    int index26 = num25;
    int num26 = index26 + 1;
    if (bitArray[index26])
      this.verticalFlipped = reader.ReadBoolean();
    int index27 = num26;
    int num27 = index27 + 1;
    if (bitArray[index27])
      this.local = reader.ReadBoolean();
    int index28 = num27;
    int num28 = index28 + 1;
    if (bitArray[index28])
      this.lightFade = reader.ReadInt32();
    int index29 = num28;
    int num29 = index29 + 1;
    if (bitArray[index29])
      this.hasLit = reader.ReadBoolean();
    int index30 = num29;
    int num30 = index30 + 1;
    if (bitArray[index30])
      this.xPeriodic = reader.ReadBoolean();
    int index31 = num30;
    int num31 = index31 + 1;
    if (bitArray[index31])
      this.yPeriodic = reader.ReadBoolean();
    int index32 = num31;
    int num32 = index32 + 1;
    if (bitArray[index32])
      this.destroyable = reader.ReadBoolean();
    int index33 = num32;
    int num33 = index33 + 1;
    if (bitArray[index33])
      this.paused = reader.ReadBoolean();
    int index34 = num33;
    int num34 = index34 + 1;
    if (bitArray[index34])
      this.rotation = reader.ReadSingle();
    int index35 = num34;
    int num35 = index35 + 1;
    if (bitArray[index35])
      this.alpha = reader.ReadSingle();
    int index36 = num35;
    int num36 = index36 + 1;
    if (bitArray[index36])
      this.alphaFade = reader.ReadSingle();
    int index37 = num36;
    int num37 = index37 + 1;
    if (bitArray[index37])
      this.layerDepth = reader.ReadSingle();
    int index38 = num37;
    int num38 = index38 + 1;
    if (bitArray[index38])
      this.scale = reader.ReadSingle();
    int index39 = num38;
    int num39 = index39 + 1;
    if (bitArray[index39])
      this.scaleChange = reader.ReadSingle();
    int index40 = num39;
    int num40 = index40 + 1;
    if (bitArray[index40])
      this.scaleChangeChange = reader.ReadSingle();
    int index41 = num40;
    int num41 = index41 + 1;
    if (bitArray[index41])
      this.rotationChange = reader.ReadSingle();
    int index42 = num41;
    int num42 = index42 + 1;
    if (bitArray[index42])
      this.id = reader.ReadInt32();
    int index43 = num42;
    int num43 = index43 + 1;
    if (bitArray[index43])
      this.lightRadius = reader.ReadSingle();
    int index44 = num43;
    int num44 = index44 + 1;
    if (bitArray[index44])
      this.xPeriodicRange = reader.ReadSingle();
    int index45 = num44;
    int num45 = index45 + 1;
    if (bitArray[index45])
      this.yPeriodicRange = reader.ReadSingle();
    int index46 = num45;
    int num46 = index46 + 1;
    if (bitArray[index46])
      this.xPeriodicLoopTime = reader.ReadSingle();
    int index47 = num46;
    int num47 = index47 + 1;
    if (bitArray[index47])
      this.yPeriodicLoopTime = reader.ReadSingle();
    int index48 = num47;
    int num48 = index48 + 1;
    if (bitArray[index48])
      this.shakeIntensityChange = reader.ReadSingle();
    int index49 = num48;
    int num49 = index49 + 1;
    if (bitArray[index49])
      this.shakeIntensity = reader.ReadSingle();
    int index50 = num49;
    int num50 = index50 + 1;
    if (bitArray[index50])
      this.pulseTime = reader.ReadSingle();
    int index51 = num50;
    int num51 = index51 + 1;
    if (bitArray[index51])
      this.pulseAmount = reader.ReadSingle();
    int index52 = num51;
    int num52 = index52 + 1;
    if (bitArray[index52])
      this.position = reader.ReadVector2();
    int index53 = num52;
    int num53 = index53 + 1;
    if (bitArray[index53])
      this.sourceRectStartingPos = reader.ReadVector2();
    int index54 = num53;
    int num54 = index54 + 1;
    if (bitArray[index54])
      this.sourceRect = reader.ReadRectangle();
    int index55 = num54;
    int num55 = index55 + 1;
    if (bitArray[index55])
      this.color = reader.ReadColor();
    int index56 = num55;
    int num56 = index56 + 1;
    if (bitArray[index56])
      this.lightcolor = reader.ReadColor();
    int index57 = num56;
    int num57 = index57 + 1;
    if (bitArray[index57])
      this.motion = reader.ReadVector2();
    int index58 = num57;
    int num58 = index58 + 1;
    if (bitArray[index58])
      this.acceleration = reader.ReadVector2();
    int index59 = num58;
    int num59 = index59 + 1;
    if (bitArray[index59])
      this.accelerationChange = reader.ReadVector2();
    int index60 = num59;
    int num60 = index60 + 1;
    if (bitArray[index60])
      this.initialPosition = reader.ReadVector2();
    int index61 = num60;
    int num61 = index61 + 1;
    if (bitArray[index61])
      this.delayBeforeAnimationStart = reader.ReadInt32();
    int index62 = num61;
    int num62 = index62 + 1;
    if (bitArray[index62])
      this.ticksBeforeAnimationStart = reader.ReadInt32();
    int index63 = num62;
    int num63 = index63 + 1;
    if (bitArray[index63])
      this.startSound = reader.ReadString();
    int index64 = num63;
    int num64 = index64 + 1;
    if (bitArray[index64])
      this.endSound = reader.ReadString();
    int index65 = num64;
    int num65 = index65 + 1;
    if (bitArray[index65])
      this.text = reader.ReadString();
    int index66 = num65;
    int num66 = index66 + 1;
    if (bitArray[index66])
      this.textureName = reader.ReadString();
    int index67 = num66;
    int num67 = index67 + 1;
    if (bitArray[index67])
      this.owner = Game1.GetPlayer(reader.ReadInt64()) ?? Game1.MasterPlayer;
    int index68 = num67;
    int num68 = index68 + 1;
    if (bitArray[index68])
      this.stopAcceleratingWhenVelocityIsZero = reader.ReadBoolean();
    int index69 = num68;
    int num69 = index69 + 1;
    if (bitArray[index69])
      this.layerDepthOffset = reader.ReadSingle();
    int index70 = num69;
    int num70 = index70 + 1;
    if (bitArray[index70])
      this.afterAccelStopMotionX = reader.ReadSingle();
    int index71 = num70;
    int num71 = index71 + 1;
    if (bitArray[index71])
      this.afterAccelStopMotionY = reader.ReadSingle();
    int index72 = num71;
    int num72 = index72 + 1;
    if (bitArray[index72])
      this.positionFollowsAttachedCharacter = reader.ReadBoolean();
    int index73 = num72;
    int num73 = index73 + 1;
    if (bitArray[index73])
      this.dontClearOnAreaEntry = reader.ReadBoolean();
    int index74 = num73;
    int num74 = index74 + 1;
    if (bitArray[index74])
      this.drawAboveAlwaysFront = reader.ReadBoolean();
    this.parent = location;
    this.loadTexture();
    switch (reader.ReadByte())
    {
      case 1:
        this.attachedCharacter = (Character) (Game1.GetPlayer(reader.ReadInt64()) ?? Game1.MasterPlayer);
        break;
      case 2:
        Guid guid = reader.ReadGuid();
        if (!location.characters.ContainsGuid(guid))
        {
          Game1.log.Warn($"Failed to find character with GUID {guid} for TemporaryAniamtedSprite.attachedCharacter");
          break;
        }
        this.attachedCharacter = (Character) location.characters[guid];
        break;
    }
  }

  private void checkDirty<T>(BitArray dirtyBits, ref int i, T value, T defaultValue = null)
  {
    dirtyBits[i++] = !object.Equals((object) value, (object) defaultValue);
  }

  public void Write(BinaryWriter writer, GameLocation location)
  {
    if (this.GetType() != typeof (TemporaryAnimatedSprite))
      throw new InvalidOperationException("TemporaryAnimatedSprite.Write is not implemented for other types");
    BitArray bitArray1 = new BitArray(80 /*0x50*/);
    int i = 0;
    this.checkDirty<float>(bitArray1, ref i, this.interval, 200f);
    this.checkDirty<int>(bitArray1, ref i, this.currentParentTileIndex);
    this.checkDirty<int>(bitArray1, ref i, this.oldCurrentParentTileIndex);
    this.checkDirty<int>(bitArray1, ref i, this.initialParentTileIndex);
    this.checkDirty<int>(bitArray1, ref i, this.totalNumberOfLoops);
    this.checkDirty<int>(bitArray1, ref i, this.currentNumberOfLoops);
    this.checkDirty<int>(bitArray1, ref i, this.xStopCoordinate, -1);
    this.checkDirty<int>(bitArray1, ref i, this.yStopCoordinate, -1);
    this.checkDirty<int>(bitArray1, ref i, this.animationLength);
    this.checkDirty<int>(bitArray1, ref i, this.bombRadius);
    this.checkDirty<int>(bitArray1, ref i, this.bombDamage);
    this.checkDirty<int>(bitArray1, ref i, this.pingPongMotion, -1);
    this.checkDirty<int>(bitArray1, ref i, this.fireworkType, -1);
    this.checkDirty<bool>(bitArray1, ref i, this.flicker);
    this.checkDirty<bool>(bitArray1, ref i, this.timeBasedMotion);
    this.checkDirty<bool>(bitArray1, ref i, this.overrideLocationDestroy);
    this.checkDirty<bool>(bitArray1, ref i, this.pingPong);
    this.checkDirty<bool>(bitArray1, ref i, this.holdLastFrame);
    this.checkDirty<bool>(bitArray1, ref i, this.pulse);
    this.checkDirty<int>(bitArray1, ref i, this.extraInfoForEndBehavior);
    this.checkDirty<string>(bitArray1, ref i, this.lightId);
    this.checkDirty<bool>(bitArray1, ref i, this.bigCraftable);
    this.checkDirty<bool>(bitArray1, ref i, this.swordswipe);
    this.checkDirty<bool>(bitArray1, ref i, this.flash);
    this.checkDirty<bool>(bitArray1, ref i, this.flipped);
    this.checkDirty<bool>(bitArray1, ref i, this.verticalFlipped);
    this.checkDirty<bool>(bitArray1, ref i, this.local);
    this.checkDirty<int>(bitArray1, ref i, this.lightFade);
    this.checkDirty<bool>(bitArray1, ref i, this.hasLit);
    this.checkDirty<bool>(bitArray1, ref i, this.xPeriodic);
    this.checkDirty<bool>(bitArray1, ref i, this.yPeriodic);
    this.checkDirty<bool>(bitArray1, ref i, this.destroyable, true);
    this.checkDirty<bool>(bitArray1, ref i, this.paused);
    this.checkDirty<float>(bitArray1, ref i, this.rotation);
    this.checkDirty<float>(bitArray1, ref i, this.alpha, 1f);
    this.checkDirty<float>(bitArray1, ref i, this.alphaFade);
    this.checkDirty<float>(bitArray1, ref i, this.layerDepth, -1f);
    this.checkDirty<float>(bitArray1, ref i, this.scale, 1f);
    this.checkDirty<float>(bitArray1, ref i, this.scaleChange);
    this.checkDirty<float>(bitArray1, ref i, this.scaleChangeChange);
    this.checkDirty<float>(bitArray1, ref i, this.rotationChange);
    this.checkDirty<int>(bitArray1, ref i, this.id);
    this.checkDirty<float>(bitArray1, ref i, this.lightRadius);
    this.checkDirty<float>(bitArray1, ref i, this.xPeriodicRange);
    this.checkDirty<float>(bitArray1, ref i, this.yPeriodicRange);
    this.checkDirty<float>(bitArray1, ref i, this.xPeriodicLoopTime);
    this.checkDirty<float>(bitArray1, ref i, this.yPeriodicLoopTime);
    this.checkDirty<float>(bitArray1, ref i, this.shakeIntensityChange);
    this.checkDirty<float>(bitArray1, ref i, this.shakeIntensity);
    this.checkDirty<float>(bitArray1, ref i, this.pulseTime);
    this.checkDirty<float>(bitArray1, ref i, this.pulseAmount, 1.1f);
    this.checkDirty<Vector2>(bitArray1, ref i, this.position);
    this.checkDirty<Vector2>(bitArray1, ref i, this.sourceRectStartingPos);
    this.checkDirty<Rectangle>(bitArray1, ref i, this.sourceRect);
    this.checkDirty<Color>(bitArray1, ref i, this.color, Color.White);
    this.checkDirty<Color>(bitArray1, ref i, this.lightcolor, Color.White);
    this.checkDirty<Vector2>(bitArray1, ref i, this.motion, Vector2.Zero);
    this.checkDirty<Vector2>(bitArray1, ref i, this.acceleration, Vector2.Zero);
    this.checkDirty<Vector2>(bitArray1, ref i, this.accelerationChange, Vector2.Zero);
    this.checkDirty<Vector2>(bitArray1, ref i, this.initialPosition);
    this.checkDirty<int>(bitArray1, ref i, this.delayBeforeAnimationStart);
    this.checkDirty<int>(bitArray1, ref i, this.ticksBeforeAnimationStart);
    this.checkDirty<string>(bitArray1, ref i, this.startSound);
    this.checkDirty<string>(bitArray1, ref i, this.endSound);
    this.checkDirty<string>(bitArray1, ref i, this.text);
    this.checkDirty<Texture2D>(bitArray1, ref i, this.texture);
    this.checkDirty<Farmer>(bitArray1, ref i, this.owner);
    this.checkDirty<bool>(bitArray1, ref i, this.stopAcceleratingWhenVelocityIsZero);
    this.checkDirty<float>(bitArray1, ref i, this.layerDepthOffset);
    this.checkDirty<float>(bitArray1, ref i, this.afterAccelStopMotionX);
    this.checkDirty<float>(bitArray1, ref i, this.afterAccelStopMotionY);
    this.checkDirty<bool>(bitArray1, ref i, this.positionFollowsAttachedCharacter);
    this.checkDirty<bool>(bitArray1, ref i, this.dontClearOnAreaEntry);
    this.checkDirty<bool>(bitArray1, ref i, this.drawAboveAlwaysFront);
    writer.WriteBitArray(bitArray1);
    int num1 = 0;
    BitArray bitArray2 = bitArray1;
    int index1 = num1;
    int num2 = index1 + 1;
    if (bitArray2[index1])
      writer.Write(this.interval);
    BitArray bitArray3 = bitArray1;
    int index2 = num2;
    int num3 = index2 + 1;
    if (bitArray3[index2])
      writer.Write(this.currentParentTileIndex);
    BitArray bitArray4 = bitArray1;
    int index3 = num3;
    int num4 = index3 + 1;
    if (bitArray4[index3])
      writer.Write(this.oldCurrentParentTileIndex);
    BitArray bitArray5 = bitArray1;
    int index4 = num4;
    int num5 = index4 + 1;
    if (bitArray5[index4])
      writer.Write(this.initialParentTileIndex);
    BitArray bitArray6 = bitArray1;
    int index5 = num5;
    int num6 = index5 + 1;
    if (bitArray6[index5])
      writer.Write(this.totalNumberOfLoops);
    BitArray bitArray7 = bitArray1;
    int index6 = num6;
    int num7 = index6 + 1;
    if (bitArray7[index6])
      writer.Write(this.currentNumberOfLoops);
    BitArray bitArray8 = bitArray1;
    int index7 = num7;
    int num8 = index7 + 1;
    if (bitArray8[index7])
      writer.Write(this.xStopCoordinate);
    BitArray bitArray9 = bitArray1;
    int index8 = num8;
    int num9 = index8 + 1;
    if (bitArray9[index8])
      writer.Write(this.yStopCoordinate);
    BitArray bitArray10 = bitArray1;
    int index9 = num9;
    int num10 = index9 + 1;
    if (bitArray10[index9])
      writer.Write(this.animationLength);
    BitArray bitArray11 = bitArray1;
    int index10 = num10;
    int num11 = index10 + 1;
    if (bitArray11[index10])
      writer.Write(this.bombRadius);
    BitArray bitArray12 = bitArray1;
    int index11 = num11;
    int num12 = index11 + 1;
    if (bitArray12[index11])
      writer.Write(this.bombDamage);
    BitArray bitArray13 = bitArray1;
    int index12 = num12;
    int num13 = index12 + 1;
    if (bitArray13[index12])
      writer.Write(this.pingPongMotion);
    BitArray bitArray14 = bitArray1;
    int index13 = num13;
    int num14 = index13 + 1;
    if (bitArray14[index13])
      writer.Write(this.fireworkType);
    BitArray bitArray15 = bitArray1;
    int index14 = num14;
    int num15 = index14 + 1;
    if (bitArray15[index14])
      writer.Write(this.flicker);
    BitArray bitArray16 = bitArray1;
    int index15 = num15;
    int num16 = index15 + 1;
    if (bitArray16[index15])
      writer.Write(this.timeBasedMotion);
    BitArray bitArray17 = bitArray1;
    int index16 = num16;
    int num17 = index16 + 1;
    if (bitArray17[index16])
      writer.Write(this.overrideLocationDestroy);
    BitArray bitArray18 = bitArray1;
    int index17 = num17;
    int num18 = index17 + 1;
    if (bitArray18[index17])
      writer.Write(this.pingPong);
    BitArray bitArray19 = bitArray1;
    int index18 = num18;
    int num19 = index18 + 1;
    if (bitArray19[index18])
      writer.Write(this.holdLastFrame);
    BitArray bitArray20 = bitArray1;
    int index19 = num19;
    int num20 = index19 + 1;
    if (bitArray20[index19])
      writer.Write(this.pulse);
    BitArray bitArray21 = bitArray1;
    int index20 = num20;
    int num21 = index20 + 1;
    if (bitArray21[index20])
      writer.Write(this.extraInfoForEndBehavior);
    BitArray bitArray22 = bitArray1;
    int index21 = num21;
    int num22 = index21 + 1;
    if (bitArray22[index21])
      writer.Write(this.lightId);
    BitArray bitArray23 = bitArray1;
    int index22 = num22;
    int num23 = index22 + 1;
    if (bitArray23[index22])
      writer.Write(this.bigCraftable);
    BitArray bitArray24 = bitArray1;
    int index23 = num23;
    int num24 = index23 + 1;
    if (bitArray24[index23])
      writer.Write(this.swordswipe);
    BitArray bitArray25 = bitArray1;
    int index24 = num24;
    int num25 = index24 + 1;
    if (bitArray25[index24])
      writer.Write(this.flash);
    BitArray bitArray26 = bitArray1;
    int index25 = num25;
    int num26 = index25 + 1;
    if (bitArray26[index25])
      writer.Write(this.flipped);
    BitArray bitArray27 = bitArray1;
    int index26 = num26;
    int num27 = index26 + 1;
    if (bitArray27[index26])
      writer.Write(this.verticalFlipped);
    BitArray bitArray28 = bitArray1;
    int index27 = num27;
    int num28 = index27 + 1;
    if (bitArray28[index27])
      writer.Write(this.local);
    BitArray bitArray29 = bitArray1;
    int index28 = num28;
    int num29 = index28 + 1;
    if (bitArray29[index28])
      writer.Write(this.lightFade);
    BitArray bitArray30 = bitArray1;
    int index29 = num29;
    int num30 = index29 + 1;
    if (bitArray30[index29])
      writer.Write(this.hasLit);
    BitArray bitArray31 = bitArray1;
    int index30 = num30;
    int num31 = index30 + 1;
    if (bitArray31[index30])
      writer.Write(this.xPeriodic);
    BitArray bitArray32 = bitArray1;
    int index31 = num31;
    int num32 = index31 + 1;
    if (bitArray32[index31])
      writer.Write(this.yPeriodic);
    BitArray bitArray33 = bitArray1;
    int index32 = num32;
    int num33 = index32 + 1;
    if (bitArray33[index32])
      writer.Write(this.destroyable);
    BitArray bitArray34 = bitArray1;
    int index33 = num33;
    int num34 = index33 + 1;
    if (bitArray34[index33])
      writer.Write(this.paused);
    BitArray bitArray35 = bitArray1;
    int index34 = num34;
    int num35 = index34 + 1;
    if (bitArray35[index34])
      writer.Write(this.rotation);
    BitArray bitArray36 = bitArray1;
    int index35 = num35;
    int num36 = index35 + 1;
    if (bitArray36[index35])
      writer.Write(this.alpha);
    BitArray bitArray37 = bitArray1;
    int index36 = num36;
    int num37 = index36 + 1;
    if (bitArray37[index36])
      writer.Write(this.alphaFade);
    BitArray bitArray38 = bitArray1;
    int index37 = num37;
    int num38 = index37 + 1;
    if (bitArray38[index37])
      writer.Write(this.layerDepth);
    BitArray bitArray39 = bitArray1;
    int index38 = num38;
    int num39 = index38 + 1;
    if (bitArray39[index38])
      writer.Write(this.scale);
    BitArray bitArray40 = bitArray1;
    int index39 = num39;
    int num40 = index39 + 1;
    if (bitArray40[index39])
      writer.Write(this.scaleChange);
    BitArray bitArray41 = bitArray1;
    int index40 = num40;
    int num41 = index40 + 1;
    if (bitArray41[index40])
      writer.Write(this.scaleChangeChange);
    BitArray bitArray42 = bitArray1;
    int index41 = num41;
    int num42 = index41 + 1;
    if (bitArray42[index41])
      writer.Write(this.rotationChange);
    BitArray bitArray43 = bitArray1;
    int index42 = num42;
    int num43 = index42 + 1;
    if (bitArray43[index42])
      writer.Write(this.id);
    BitArray bitArray44 = bitArray1;
    int index43 = num43;
    int num44 = index43 + 1;
    if (bitArray44[index43])
      writer.Write(this.lightRadius);
    BitArray bitArray45 = bitArray1;
    int index44 = num44;
    int num45 = index44 + 1;
    if (bitArray45[index44])
      writer.Write(this.xPeriodicRange);
    BitArray bitArray46 = bitArray1;
    int index45 = num45;
    int num46 = index45 + 1;
    if (bitArray46[index45])
      writer.Write(this.yPeriodicRange);
    BitArray bitArray47 = bitArray1;
    int index46 = num46;
    int num47 = index46 + 1;
    if (bitArray47[index46])
      writer.Write(this.xPeriodicLoopTime);
    BitArray bitArray48 = bitArray1;
    int index47 = num47;
    int num48 = index47 + 1;
    if (bitArray48[index47])
      writer.Write(this.yPeriodicLoopTime);
    BitArray bitArray49 = bitArray1;
    int index48 = num48;
    int num49 = index48 + 1;
    if (bitArray49[index48])
      writer.Write(this.shakeIntensityChange);
    BitArray bitArray50 = bitArray1;
    int index49 = num49;
    int num50 = index49 + 1;
    if (bitArray50[index49])
      writer.Write(this.shakeIntensity);
    BitArray bitArray51 = bitArray1;
    int index50 = num50;
    int num51 = index50 + 1;
    if (bitArray51[index50])
      writer.Write(this.pulseTime);
    BitArray bitArray52 = bitArray1;
    int index51 = num51;
    int num52 = index51 + 1;
    if (bitArray52[index51])
      writer.Write(this.pulseAmount);
    BitArray bitArray53 = bitArray1;
    int index52 = num52;
    int num53 = index52 + 1;
    if (bitArray53[index52])
      writer.WriteVector2(this.position);
    BitArray bitArray54 = bitArray1;
    int index53 = num53;
    int num54 = index53 + 1;
    if (bitArray54[index53])
      writer.WriteVector2(this.sourceRectStartingPos);
    BitArray bitArray55 = bitArray1;
    int index54 = num54;
    int num55 = index54 + 1;
    if (bitArray55[index54])
      writer.WriteRectangle(this.sourceRect);
    BitArray bitArray56 = bitArray1;
    int index55 = num55;
    int num56 = index55 + 1;
    if (bitArray56[index55])
      writer.WriteColor(this.color);
    BitArray bitArray57 = bitArray1;
    int index56 = num56;
    int num57 = index56 + 1;
    if (bitArray57[index56])
      writer.WriteColor(this.lightcolor);
    BitArray bitArray58 = bitArray1;
    int index57 = num57;
    int num58 = index57 + 1;
    if (bitArray58[index57])
      writer.WriteVector2(this.motion);
    BitArray bitArray59 = bitArray1;
    int index58 = num58;
    int num59 = index58 + 1;
    if (bitArray59[index58])
      writer.WriteVector2(this.acceleration);
    BitArray bitArray60 = bitArray1;
    int index59 = num59;
    int num60 = index59 + 1;
    if (bitArray60[index59])
      writer.WriteVector2(this.accelerationChange);
    BitArray bitArray61 = bitArray1;
    int index60 = num60;
    int num61 = index60 + 1;
    if (bitArray61[index60])
      writer.WriteVector2(this.initialPosition);
    BitArray bitArray62 = bitArray1;
    int index61 = num61;
    int num62 = index61 + 1;
    if (bitArray62[index61])
      writer.Write(this.delayBeforeAnimationStart);
    BitArray bitArray63 = bitArray1;
    int index62 = num62;
    int num63 = index62 + 1;
    if (bitArray63[index62])
      writer.Write(this.ticksBeforeAnimationStart);
    BitArray bitArray64 = bitArray1;
    int index63 = num63;
    int num64 = index63 + 1;
    if (bitArray64[index63])
      writer.Write(this.startSound);
    BitArray bitArray65 = bitArray1;
    int index64 = num64;
    int num65 = index64 + 1;
    if (bitArray65[index64])
      writer.Write(this.endSound);
    BitArray bitArray66 = bitArray1;
    int index65 = num65;
    int num66 = index65 + 1;
    if (bitArray66[index65])
      writer.Write(this.text);
    BitArray bitArray67 = bitArray1;
    int index66 = num66;
    int num67 = index66 + 1;
    if (bitArray67[index66])
      writer.Write(this.textureName);
    BitArray bitArray68 = bitArray1;
    int index67 = num67;
    int num68 = index67 + 1;
    if (bitArray68[index67])
      writer.Write(this.owner.uniqueMultiplayerID.Value);
    BitArray bitArray69 = bitArray1;
    int index68 = num68;
    int num69 = index68 + 1;
    if (bitArray69[index68])
      writer.Write(this.stopAcceleratingWhenVelocityIsZero);
    BitArray bitArray70 = bitArray1;
    int index69 = num69;
    int num70 = index69 + 1;
    if (bitArray70[index69])
      writer.Write(this.layerDepthOffset);
    BitArray bitArray71 = bitArray1;
    int index70 = num70;
    int num71 = index70 + 1;
    if (bitArray71[index70])
      writer.Write(this.afterAccelStopMotionX);
    BitArray bitArray72 = bitArray1;
    int index71 = num71;
    int num72 = index71 + 1;
    if (bitArray72[index71])
      writer.Write(this.afterAccelStopMotionY);
    BitArray bitArray73 = bitArray1;
    int index72 = num72;
    int num73 = index72 + 1;
    if (bitArray73[index72])
      writer.Write(this.positionFollowsAttachedCharacter);
    BitArray bitArray74 = bitArray1;
    int index73 = num73;
    int num74 = index73 + 1;
    if (bitArray74[index73])
      writer.Write(this.dontClearOnAreaEntry);
    BitArray bitArray75 = bitArray1;
    int index74 = num74;
    int num75 = index74 + 1;
    if (bitArray75[index74])
      writer.Write(this.drawAboveAlwaysFront);
    switch (this.attachedCharacter)
    {
      case null:
        writer.Write((byte) 0);
        break;
      case Farmer farmer:
        writer.Write((byte) 1);
        writer.Write(farmer.UniqueMultiplayerID);
        break;
      case NPC npc:
        writer.Write((byte) 2);
        writer.WriteGuid(location.characters.GuidOf(npc));
        break;
      default:
        throw new ArgumentException();
    }
  }

  public virtual void draw(
    SpriteBatch spriteBatch,
    bool localPosition = false,
    int xOffset = 0,
    int yOffset = 0,
    float extraAlpha = 1f)
  {
    if (this.local)
      localPosition = true;
    if (this.currentParentTileIndex < 0 || this.delayBeforeAnimationStart > 0 || this.ticksBeforeAnimationStart > 0)
      return;
    if (this.text != null)
    {
      if (this.extraInfoForEndBehavior == -777)
      {
        Vector2 local = Game1.GlobalToLocal(this.position);
        SpriteText.drawString(spriteBatch, this.text, (int) local.X, (int) local.Y, alpha: this.alpha, layerDepth: this.layerDepth, color: new Color?(this.color.Equals(Color.White) ? SpriteText.color_White : SpriteText.color_Black));
      }
      else
        spriteBatch.DrawString(Game1.dialogueFont, this.text, localPosition ? this.Position : Game1.GlobalToLocal(Game1.viewport, this.Position), this.color * this.alpha * extraAlpha, this.rotation, Vector2.Zero, this.scale, SpriteEffects.None, this.layerDepth + this.layerDepthOffset);
    }
    else if (this.Texture != null)
    {
      if (this.positionFollowsAttachedCharacter && this.attachedCharacter != null)
        spriteBatch.Draw(this.Texture, (localPosition ? this.Position : Game1.GlobalToLocal(Game1.viewport, this.attachedCharacter.Position + new Vector2((float) ((int) this.Position.X + xOffset), (float) ((int) this.Position.Y + yOffset)))) + new Vector2((float) (this.sourceRect.Width / 2), (float) (this.sourceRect.Height / 2)) * this.scale + new Vector2((double) this.shakeIntensity > 0.0 ? (float) Game1.random.Next(-(int) this.shakeIntensity, (int) this.shakeIntensity + 1) : 0.0f, (double) this.shakeIntensity > 0.0 ? (float) Game1.random.Next(-(int) this.shakeIntensity, (int) this.shakeIntensity + 1) : 0.0f), new Rectangle?(this.sourceRect), this.color * this.alpha * extraAlpha, this.rotation, new Vector2((float) (this.sourceRect.Width / 2), (float) (this.sourceRect.Height / 2)), this.scale, this.flipped ? SpriteEffects.FlipHorizontally : (this.verticalFlipped ? SpriteEffects.FlipVertically : SpriteEffects.None), ((double) this.layerDepth >= 0.0 ? this.layerDepth : (float) (((double) this.Position.Y + (double) this.sourceRect.Height) / 10000.0)) + this.layerDepthOffset);
      else if (!this.vectorScale.Equals(Vector2.Zero))
        spriteBatch.Draw(this.Texture, (localPosition ? this.Position : Game1.GlobalToLocal(Game1.viewport, new Vector2((float) ((int) this.Position.X + xOffset), (float) ((int) this.Position.Y + yOffset)))) + new Vector2((double) this.shakeIntensity > 0.0 ? (float) Game1.random.Next(-(int) this.shakeIntensity, (int) this.shakeIntensity + 1) : 0.0f, (double) this.shakeIntensity > 0.0 ? (float) Game1.random.Next(-(int) this.shakeIntensity, (int) this.shakeIntensity + 1) : 0.0f), new Rectangle?(this.sourceRect), this.color * this.alpha * extraAlpha, this.rotation, new Vector2((float) (this.sourceRect.Width / 2), (float) (this.sourceRect.Height / 2)), this.vectorScale, this.flipped ? SpriteEffects.FlipHorizontally : (this.verticalFlipped ? SpriteEffects.FlipVertically : SpriteEffects.None), ((double) this.layerDepth >= 0.0 ? this.layerDepth : (float) (((double) this.Position.Y + (double) this.sourceRect.Height) / 10000.0)) + this.layerDepthOffset);
      else
        spriteBatch.Draw(this.Texture, (localPosition ? this.Position : Game1.GlobalToLocal(Game1.viewport, new Vector2((float) ((int) this.Position.X + xOffset), (float) ((int) this.Position.Y + yOffset)))) + new Vector2((float) (this.sourceRect.Width / 2), (float) (this.sourceRect.Height / 2)) * this.scale + new Vector2((double) this.shakeIntensity > 0.0 ? (float) Game1.random.Next(-(int) this.shakeIntensity, (int) this.shakeIntensity + 1) : 0.0f, (double) this.shakeIntensity > 0.0 ? (float) Game1.random.Next(-(int) this.shakeIntensity, (int) this.shakeIntensity + 1) : 0.0f), new Rectangle?(this.sourceRect), this.color * this.alpha * extraAlpha, this.rotation, new Vector2((float) (this.sourceRect.Width / 2), (float) (this.sourceRect.Height / 2)), this.scale, this.flipped ? SpriteEffects.FlipHorizontally : (this.verticalFlipped ? SpriteEffects.FlipVertically : SpriteEffects.None), ((double) this.layerDepth >= 0.0 ? this.layerDepth : (float) (((double) this.Position.Y + (double) this.sourceRect.Height) / 10000.0)) + this.layerDepthOffset);
    }
    else if (this.bigCraftable)
    {
      spriteBatch.Draw(Game1.bigCraftableSpriteSheet, localPosition ? this.Position : Game1.GlobalToLocal(Game1.viewport, new Vector2((float) ((int) this.Position.X + xOffset), (float) ((int) this.Position.Y + yOffset))) + new Vector2((float) (this.sourceRect.Width / 2), (float) (this.sourceRect.Height / 2)), new Rectangle?(Object.getSourceRectForBigCraftable(this.currentParentTileIndex)), Color.White * extraAlpha, 0.0f, new Vector2((float) (this.sourceRect.Width / 2), (float) (this.sourceRect.Height / 2)), this.scale, SpriteEffects.None, (float) (((double) this.Position.Y + 32.0) / 10000.0) + this.layerDepthOffset);
    }
    else
    {
      if (this.swordswipe)
        return;
      if (this.attachedCharacter != null)
      {
        if (this.local)
          this.attachedCharacter.Position = new Vector2((float) Game1.viewport.X + this.Position.X, (float) Game1.viewport.Y + this.Position.Y);
        this.attachedCharacter.draw(spriteBatch);
      }
      else
        spriteBatch.Draw(Game1.objectSpriteSheet, localPosition ? this.Position : Game1.GlobalToLocal(Game1.viewport, new Vector2((float) ((int) this.Position.X + xOffset), (float) ((int) this.Position.Y + yOffset))) + new Vector2(8f, 8f) * 4f + new Vector2((double) this.shakeIntensity > 0.0 ? (float) Game1.random.Next(-(int) this.shakeIntensity, (int) this.shakeIntensity + 1) : 0.0f, (double) this.shakeIntensity > 0.0 ? (float) Game1.random.Next(-(int) this.shakeIntensity, (int) this.shakeIntensity + 1) : 0.0f), new Rectangle?(GameLocation.getSourceRectForObject(this.currentParentTileIndex)), (this.flash ? Color.LightBlue * 0.85f : this.color) * this.alpha * extraAlpha, this.rotation, new Vector2(8f, 8f), 4f * this.scale, this.flipped ? SpriteEffects.FlipHorizontally : SpriteEffects.None, ((double) this.layerDepth >= 0.0 ? this.layerDepth : (float) (((double) this.Position.Y + 32.0) / 10000.0)) + this.layerDepthOffset);
    }
  }

  public void bounce(int extraInfo)
  {
    if ((double) extraInfo > 1.0)
    {
      this.motion.Y = (float) -extraInfo / 2f;
      this.motion.X /= 2f;
      this.rotationChange = this.motion.Y / 50f;
      this.acceleration.Y = 0.7f;
      this.yStopCoordinate = (int) this.initialPosition.Y;
      this.parent?.playSound("thudStep");
    }
    else
    {
      if (this.extraInfoForEndBehavior != -777)
        this.alphaFade = 0.01f;
      this.motion.X = 0.0f;
    }
  }

  public void unload()
  {
    this.PlaySound(this.endSound);
    TemporaryAnimatedSprite.endBehavior endFunction = this.endFunction;
    if (endFunction != null)
      endFunction(this.extraInfoForEndBehavior);
    if (!this.hasLit)
      return;
    Utility.removeLightSource(this.lightId);
  }

  public void reset()
  {
    this.sourceRect.X = (int) this.sourceRectStartingPos.X;
    this.sourceRect.Y = (int) this.sourceRectStartingPos.Y;
    this.currentParentTileIndex = 0;
    this.oldCurrentParentTileIndex = 0;
    this.timer = 0.0f;
    this.totalTimer = 0.0f;
    this.currentNumberOfLoops = 0;
    this.pingPongMotion = 1;
  }

  public void resetEnd()
  {
    this.reset();
    this.currentParentTileIndex = this.initialParentTileIndex + this.animationLength - 1;
  }

  public virtual bool update(GameTime time)
  {
    if (this.paused)
      return false;
    int num1 = (int) time.ElapsedGameTime.TotalMilliseconds;
    if (this.usePreciseTiming)
    {
      if (this.stopWatch == null)
      {
        this.stopWatch = new Stopwatch();
        this.stopWatch.Start();
      }
      num1 = (int) (this.stopWatch.ElapsedMilliseconds - this.previousStopwatchTime);
      this.previousStopwatchTime = this.stopWatch.ElapsedMilliseconds;
    }
    if (this.bombRadius > 0 && !Game1.shouldTimePass())
      return false;
    if (this.ticksBeforeAnimationStart > 0)
    {
      --this.ticksBeforeAnimationStart;
      return false;
    }
    if (this.delayBeforeAnimationStart > 0)
    {
      this.delayBeforeAnimationStart -= num1;
      if (this.delayBeforeAnimationStart <= 0)
      {
        this.PlaySound(this.startSound);
        this.timer = (float) -this.delayBeforeAnimationStart;
      }
      if (this.delayBeforeAnimationStart <= 0 && this.parentSprite != null)
        this.position = this.parentSprite.position + this.position;
      return false;
    }
    if (float.IsNaN(this.motion.X))
      this.motion.X = 0.0f;
    if (float.IsNaN(this.motion.Y))
      this.motion.Y = 0.0f;
    this.timer += (float) num1;
    this.totalTimer += (float) num1;
    this.alpha -= this.alphaFade * (this.timeBasedMotion ? (float) num1 : 1f);
    this.alphaFade -= this.alphaFadeFade * (this.timeBasedMotion ? (float) num1 : 1f);
    if ((double) this.alphaFade > 0.0 && this.lightId != null && (double) this.alpha < 1.0 && (double) this.alpha >= 0.0)
    {
      LightSource lightSource = Utility.getLightSource(this.lightId);
      if (lightSource != null)
        lightSource.color.A = (byte) ((double) byte.MaxValue * (double) this.alpha);
    }
    this.shakeIntensity += this.shakeIntensityChange * (float) num1;
    this.scale += this.scaleChange * (this.timeBasedMotion ? (float) num1 : 1f);
    this.scaleChange += this.scaleChangeChange * (this.timeBasedMotion ? (float) num1 : 1f);
    this.rotation += this.rotationChange;
    if (this.xPeriodic)
      this.position.X = this.initialPosition.X + this.xPeriodicRange * (float) Math.Sin(2.0 * Math.PI / (double) this.xPeriodicLoopTime * (double) this.totalTimer);
    else
      this.position.X += this.motion.X * (this.timeBasedMotion ? (float) num1 : 1f);
    if (this.yPeriodic)
      this.position.Y = this.initialPosition.Y + this.yPeriodicRange * (float) Math.Sin(2.0 * Math.PI / (double) this.yPeriodicLoopTime * (double) (this.totalTimer + this.yPeriodicLoopTime / 2f));
    else
      this.position.Y += this.motion.Y * (this.timeBasedMotion ? (float) num1 : 1f);
    if (this.attachedCharacter != null && !this.positionFollowsAttachedCharacter)
    {
      if (this.xPeriodic)
        this.attachedCharacter.position.X = this.initialPosition.X + this.xPeriodicRange * (float) Math.Sin(2.0 * Math.PI / (double) this.xPeriodicLoopTime * (double) this.totalTimer);
      else
        this.attachedCharacter.position.X += this.motion.X * (this.timeBasedMotion ? (float) num1 : 1f);
      if (this.yPeriodic)
        this.attachedCharacter.position.Y = this.initialPosition.Y + this.yPeriodicRange * (float) Math.Sin(2.0 * Math.PI / (double) this.yPeriodicLoopTime * (double) this.totalTimer);
      else
        this.attachedCharacter.position.Y += this.motion.Y * (this.timeBasedMotion ? (float) num1 : 1f);
    }
    int num2 = Math.Sign(this.motion.X);
    this.motion.X += this.acceleration.X * (this.timeBasedMotion ? (float) num1 : 1f);
    if (this.stopAcceleratingWhenVelocityIsZero && Math.Sign(this.motion.X) != num2)
    {
      this.motion.X = this.afterAccelStopMotionX;
      this.acceleration.X = 0.0f;
      this.accelerationChange.X = 0.0f;
    }
    int num3 = Math.Sign(this.motion.Y);
    this.motion.Y += this.acceleration.Y * (this.timeBasedMotion ? (float) num1 : 1f);
    if (this.stopAcceleratingWhenVelocityIsZero && Math.Sign(this.motion.Y) != num3)
    {
      this.motion.Y = this.afterAccelStopMotionY;
      this.acceleration.Y = 0.0f;
      this.accelerationChange.Y = 0.0f;
    }
    this.acceleration.X += this.accelerationChange.X;
    this.acceleration.Y += this.accelerationChange.Y;
    if (this.xStopCoordinate != -1 || this.yStopCoordinate != -1)
    {
      int y = (int) this.motion.Y;
      if (this.xStopCoordinate != -1 && (double) Math.Abs(this.position.X - (float) this.xStopCoordinate) <= (double) Math.Abs(this.motion.X))
      {
        this.motion.X = 0.0f;
        this.acceleration.X = 0.0f;
        this.xStopCoordinate = -1;
      }
      if (this.yStopCoordinate != -1 && (double) Math.Abs(this.position.Y - (float) this.yStopCoordinate) <= (double) Math.Abs(this.motion.Y))
      {
        this.motion.Y = 0.0f;
        this.acceleration.Y = 0.0f;
        this.yStopCoordinate = -1;
      }
      if (this.xStopCoordinate == -1 && this.yStopCoordinate == -1)
      {
        this.rotationChange = 0.0f;
        TemporaryAnimatedSprite.endBehavior reachedStopCoordinate = this.reachedStopCoordinate;
        if (reachedStopCoordinate != null)
          reachedStopCoordinate(y);
        Action<TemporaryAnimatedSprite> coordinateSprite = this.reachedStopCoordinateSprite;
        if (coordinateSprite != null)
          coordinateSprite(this);
      }
    }
    if (!this.pingPong)
      this.pingPongMotion = 1;
    if (this.pulse)
    {
      this.pulseTimer -= (float) num1;
      if ((double) this.originalScale == 0.0)
        this.originalScale = this.scale;
      if ((double) this.pulseTimer <= 0.0)
      {
        this.pulseTimer = this.pulseTime;
        this.scale = this.originalScale * this.pulseAmount;
      }
      if ((double) this.scale > (double) this.originalScale)
        this.scale -= this.pulseAmount / 100f * (float) num1;
    }
    if (this.lightId != null)
    {
      if (!this.hasLit)
      {
        this.hasLit = true;
        if (this.parent == null || Game1.currentLocation == this.parent)
        {
          Dictionary<string, LightSource> currentLightSources = Game1.currentLightSources;
          LightSource lightSource = new LightSource(this.lightId, 4, this.position + new Vector2(32f, 32f), this.lightRadius, this.lightcolor.Equals(Color.White) ? new Color(0, 65, 128 /*0x80*/) : this.lightcolor);
          lightSource.fadeOut.Add(this.lightFade);
          currentLightSources.Add(lightSource);
        }
      }
      else
        Utility.repositionLightSource(this.lightId, this.position + new Vector2(32f, 32f));
    }
    if ((double) this.alpha <= 0.0 || (double) this.position.X < -2000.0 && !this.overrideLocationDestroy || (double) this.scale <= 0.0)
    {
      this.unload();
      return this.destroyable;
    }
    if ((double) this.timer > (double) this.interval)
    {
      this.currentParentTileIndex += this.pingPongMotion;
      this.sourceRect.X += this.sourceRect.Width * this.pingPongMotion;
      if (this.Texture != null)
      {
        if (!this.pingPong && this.sourceRect.X >= this.Texture.Width)
          this.sourceRect.Y += this.sourceRect.Height;
        if (!this.pingPong)
          this.sourceRect.X %= this.Texture.Width;
        if (this.pingPong)
        {
          if ((double) this.sourceRect.X + ((double) this.sourceRect.Y - (double) this.sourceRectStartingPos.Y) / (double) this.sourceRect.Height * (double) this.Texture.Width >= (double) this.sourceRectStartingPos.X + (double) (this.sourceRect.Width * this.animationLength))
          {
            this.pingPongMotion = -1;
            this.sourceRect.X -= this.sourceRect.Width * 2;
            --this.currentParentTileIndex;
            if (this.sourceRect.X < 0)
              this.sourceRect.X = this.Texture.Width + this.sourceRect.X;
          }
          else if ((double) this.sourceRect.X < (double) this.sourceRectStartingPos.X && (double) this.sourceRect.Y == (double) this.sourceRectStartingPos.Y)
          {
            this.pingPongMotion = 1;
            this.sourceRect.X = (int) this.sourceRectStartingPos.X + this.sourceRect.Width;
            ++this.currentParentTileIndex;
            ++this.currentNumberOfLoops;
            if (this.endFunction != null)
            {
              this.endFunction(this.extraInfoForEndBehavior);
              this.endFunction = (TemporaryAnimatedSprite.endBehavior) null;
            }
            if (this.currentNumberOfLoops >= this.totalNumberOfLoops)
            {
              this.unload();
              return this.destroyable;
            }
          }
        }
        else if (this.totalNumberOfLoops >= 1 && (double) this.sourceRect.X + ((double) this.sourceRect.Y - (double) this.sourceRectStartingPos.Y) / (double) this.sourceRect.Height * (double) this.Texture.Width >= (double) this.sourceRectStartingPos.X + (double) (this.sourceRect.Width * this.animationLength))
        {
          this.sourceRect.X = (int) this.sourceRectStartingPos.X;
          this.sourceRect.Y = (int) this.sourceRectStartingPos.Y;
        }
      }
      this.timer -= this.interval;
      if (this.flicker)
      {
        if (this.currentParentTileIndex < 0 || this.flash)
        {
          this.currentParentTileIndex = this.oldCurrentParentTileIndex;
          this.flash = false;
        }
        else
        {
          this.oldCurrentParentTileIndex = this.currentParentTileIndex;
          if (this.bombRadius > 0)
            this.flash = true;
          else
            this.currentParentTileIndex = -100;
        }
      }
      if (this.currentParentTileIndex - this.initialParentTileIndex >= this.animationLength)
      {
        ++this.currentNumberOfLoops;
        if (this.holdLastFrame)
        {
          this.currentParentTileIndex = this.initialParentTileIndex + this.animationLength - 1;
          if (this.texture != null)
            this.setSourceRectToCurrentTileIndex();
          if (this.endFunction != null)
          {
            this.endFunction(this.extraInfoForEndBehavior);
            this.endFunction = (TemporaryAnimatedSprite.endBehavior) null;
          }
          return false;
        }
        this.currentParentTileIndex = this.initialParentTileIndex;
        if (this.currentNumberOfLoops >= this.totalNumberOfLoops)
        {
          if (this.bombRadius > 0)
          {
            if (Game1.currentLocation == this.parent)
              Game1.flashAlpha = 1f;
            if (Game1.IsMasterGame)
            {
              this.parent.netAudio.StopPlaying("fuse");
              this.parent.playSound("explosion");
              this.parent.explode(new Vector2((float) (int) ((double) this.position.X / 64.0), (float) (int) ((double) this.position.Y / 64.0)), this.bombRadius, this.owner, damage_amount: this.bombDamage);
            }
          }
          if (this.fireworkType >= 0)
          {
            float lifetimeMultiplier = TemporaryAnimatedSprite.GetFireworkLifetimeMultiplier(this.fireworkType);
            Color fireworkColor = TemporaryAnimatedSprite.GetFireworkColor(this.fireworkType);
            if (Game1.currentLocation == this.parent)
              Game1.screenGlowOnce(fireworkColor * 0.8f, false);
            if (Game1.IsMasterGame)
            {
              float num4 = 0.3f;
              float id = (float) this.id;
              Vector2[] fireworkLights = TemporaryAnimatedSprite.GetFireworkLights(this.fireworkType);
              Vector2[] fireworkPoints = TemporaryAnimatedSprite.GetFireworkPoints(this.fireworkType);
              List<TemporaryAnimatedSprite> temporaryAnimatedSpriteList1 = new List<TemporaryAnimatedSprite>();
              foreach (Vector2 vector2 in fireworkLights)
              {
                List<TemporaryAnimatedSprite> temporaryAnimatedSpriteList2 = temporaryAnimatedSpriteList1;
                TemporaryAnimatedSprite temporaryAnimatedSprite = new TemporaryAnimatedSprite("LooseSprites\\Cursors_1_6", new Rectangle(0, 0, 1, 1), 1800f * lifetimeMultiplier, 1, 0, this.position, false, false, -1f, 0.0f, Color.Transparent, 1f, 0.0f, 0.0f, 0.0f);
                temporaryAnimatedSprite.motion = vector2;
                temporaryAnimatedSprite.acceleration = vector2 * num4;
                temporaryAnimatedSprite.accelerationChange = -vector2 / id;
                temporaryAnimatedSprite.stopAcceleratingWhenVelocityIsZero = true;
                temporaryAnimatedSprite.afterAccelStopMotionX = (float) Math.Sign(vector2.X) * 0.1f;
                temporaryAnimatedSprite.afterAccelStopMotionY = 0.33f;
                temporaryAnimatedSprite.layerDepthOffset = 320f;
                temporaryAnimatedSprite.lightId = $"Firework_{this.id}_{vector2.X}_{vector2.Y}";
                temporaryAnimatedSprite.lightRadius = 1.3f;
                temporaryAnimatedSprite.drawAboveAlwaysFront = true;
                temporaryAnimatedSprite.lightFade = 2;
                temporaryAnimatedSpriteList2.Add(temporaryAnimatedSprite);
              }
              foreach (Vector2 vector2 in fireworkPoints)
              {
                temporaryAnimatedSpriteList1.Add(new TemporaryAnimatedSprite("LooseSprites\\Cursors_1_6", new Rectangle(304, 364 + this.fireworkType * 11, 11, 11), 75f * lifetimeMultiplier + (float) Game1.random.Next(-20, 21), 12, 1, this.position, false, false, -1f, 0.0f, Color.White, 4f, 0.0f, (float) (Game1.random.NextDouble() * Math.PI) * 0.5f, 0.0f)
                {
                  motion = vector2,
                  acceleration = vector2 * num4,
                  accelerationChange = -vector2 / id,
                  stopAcceleratingWhenVelocityIsZero = true,
                  afterAccelStopMotionX = (float) Math.Sign(vector2.X) * 0.1f,
                  afterAccelStopMotionY = 0.33f,
                  alpha = 1f,
                  alphaFade = 0.01f,
                  alphaFadeFade = 0.00025f,
                  drawAboveAlwaysFront = true
                });
                int num5 = Game1.random.Next(3) != 0 ? 1 : 0;
                temporaryAnimatedSpriteList1.Add(new TemporaryAnimatedSprite("TileSheets\\animations", new Rectangle(0, 64 /*0x40*/ * (10 + num5), 64 /*0x40*/, 64 /*0x40*/), 100f * lifetimeMultiplier, num5 == 0 ? 9 : 6, 2, this.position, false, false, -1f, 0.0f, Utility.getBlendedColor(fireworkColor, Color.White), 1f, 0.0f, (float) (Game1.random.NextDouble() * Math.PI * 0.5), 0.0f)
                {
                  motion = vector2 * 0.75f,
                  acceleration = vector2 * num4,
                  accelerationChange = -vector2 / id,
                  stopAcceleratingWhenVelocityIsZero = true,
                  afterAccelStopMotionX = (float) Math.Sign(vector2.X) * 0.1f,
                  afterAccelStopMotionY = 0.33f,
                  drawAboveAlwaysFront = true,
                  alpha = 0.5f,
                  delayBeforeAnimationStart = Game1.random.Next(50, 100)
                });
              }
              if (this.id == 30)
              {
                for (int index = 0; index < 8; ++index)
                {
                  Vector2 vector2 = fireworkPoints[Game1.random.Next(fireworkPoints.Length)];
                  List<TemporaryAnimatedSprite> temporaryAnimatedSpriteList3 = temporaryAnimatedSpriteList1;
                  TemporaryAnimatedSprite temporaryAnimatedSprite = new TemporaryAnimatedSprite("LooseSprites\\Cursors_1_6", new Rectangle(304, 397, 11, 11), 75f * lifetimeMultiplier, 12, 5, this.position, false, false, -1f, 0.0f, Utility.getBlendedColor(Color.White, Utility.getRandomRainbowColor()), 4f, 0.0f, 0.0f, 0.0f);
                  temporaryAnimatedSprite.motion = vector2 * 1.1f;
                  temporaryAnimatedSprite.alpha = 1f;
                  temporaryAnimatedSprite.alphaFade = 0.01f;
                  temporaryAnimatedSprite.acceleration = vector2 * num4;
                  temporaryAnimatedSprite.accelerationChange = -vector2 / ((float) this.id * 1.25f);
                  temporaryAnimatedSprite.stopAcceleratingWhenVelocityIsZero = true;
                  temporaryAnimatedSprite.afterAccelStopMotionX = (float) Math.Sign(vector2.X) * 0.1f;
                  temporaryAnimatedSprite.afterAccelStopMotionY = 0.33f;
                  temporaryAnimatedSprite.drawAboveAlwaysFront = true;
                  temporaryAnimatedSprite.lightId = $"Firework_{this.id}_{index}";
                  temporaryAnimatedSprite.lightRadius = 0.33f;
                  temporaryAnimatedSprite.lightFade = 3;
                  temporaryAnimatedSpriteList3.Add(temporaryAnimatedSprite);
                }
              }
              Game1.multiplayer.broadcastSprites(this.parent, temporaryAnimatedSpriteList1.ToArray());
              this.parent.netAudio.StopPlaying("fuse");
            }
          }
          this.unload();
          return this.destroyable;
        }
        if (this.bombRadius > 0 && this.currentNumberOfLoops == this.totalNumberOfLoops - 5)
          this.interval -= this.interval / 3f;
      }
    }
    return false;
  }

  public bool clearOnAreaEntry() => !this.dontClearOnAreaEntry && this.bombRadius <= 0;

  private void setSourceRectToCurrentTileIndex()
  {
    this.sourceRect.X = (int) ((double) this.sourceRectStartingPos.X + (double) (this.currentParentTileIndex * this.sourceRect.Width)) % this.texture.Width;
    if (this.sourceRect.X < 0)
      this.sourceRect.X = 0;
    this.sourceRect.Y = (int) this.sourceRectStartingPos.Y;
  }

  /// <summary>Play a sound locally, preferring the parent location if possible.</summary>
  /// <param name="sound">The sound to play.</param>
  private void PlaySound(string sound)
  {
    if (sound == null)
      return;
    if (this.parent == null)
      Game1.playSound(sound);
    else
      this.parent.localSound(sound);
  }

  public static TemporaryAnimatedSprite CreateFromData(
    TemporaryAnimatedSpriteDefinition temporarySprite,
    float x,
    float y,
    float sortLayer)
  {
    return new TemporaryAnimatedSprite(temporarySprite.Texture, temporarySprite.SourceRect, temporarySprite.Interval, temporarySprite.Frames, temporarySprite.Loops, new Vector2(x, y) * 64f + temporarySprite.PositionOffset * 4f, temporarySprite.Flicker, temporarySprite.Flip, sortLayer + temporarySprite.SortOffset, temporarySprite.AlphaFade, Utility.StringToColor(temporarySprite.Color) ?? Color.White, temporarySprite.Scale * 4f, temporarySprite.ScaleChange, temporarySprite.Rotation, temporarySprite.RotationChange);
  }

  public delegate void endBehavior(int extraInfo);
}
