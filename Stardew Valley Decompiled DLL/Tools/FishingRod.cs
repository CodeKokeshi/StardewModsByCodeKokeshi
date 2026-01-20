// Decompiled with JetBrains decompiler
// Type: StardewValley.Tools.FishingRod
// Assembly: Stardew Valley, Version=1.6.15.24356, Culture=neutral, PublicKeyToken=null
// MVID: AA1F513A-94F0-4EF6-A35F-2D5B4A3721BF
// Assembly location: D:\SteamLibrary\steamapps\common\Stardew Valley\Stardew Valley.dll
// XML documentation location: D:\SteamLibrary\steamapps\common\Stardew Valley\Stardew Valley.xml

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Netcode;
using StardewValley.BellsAndWhistles;
using StardewValley.Buildings;
using StardewValley.Constants;
using StardewValley.Enchantments;
using StardewValley.Extensions;
using StardewValley.ItemTypeDefinitions;
using StardewValley.Locations;
using StardewValley.Menus;
using StardewValley.Network;
using StardewValley.Objects;
using StardewValley.SpecialOrders;
using StardewValley.TokenizableStrings;
using System;
using System.Collections.Generic;
using System.IO;
using System.Xml.Serialization;

#nullable disable
namespace StardewValley.Tools;

public class FishingRod : Tool
{
  /// <summary>The index in <see cref="F:StardewValley.Tool.attachments" /> for equipped bait.</summary>
  public const int BaitIndex = 0;
  /// <summary>The index in <see cref="F:StardewValley.Tool.attachments" /> for equipped tackle.</summary>
  public const int TackleIndex = 1;
  public const int sizeOfLandCheckRectangle = 11;
  public static int NUM_BOBBER_STYLES = 39;
  [XmlElement("bobber")]
  public readonly NetPosition bobber;
  /// <summary>The underlying field for <see cref="P:StardewValley.Tools.FishingRod.CastDirection" />.</summary>
  private readonly NetInt castDirection;
  public static int minFishingBiteTime = 600;
  public static int maxFishingBiteTime = 30000;
  public static int maxTimeToNibble = 800;
  public static int maxTackleUses = 20;
  private int whichTackleSlotToReplace;
  protected Vector2 _lastAppliedMotion;
  protected Vector2[] _totalMotionBuffer;
  protected int _totalMotionBufferIndex;
  protected NetVector2 _totalMotion;
  public static double baseChanceForTreasure = 0.15;
  [XmlIgnore]
  public int bobberBob;
  [XmlIgnore]
  public float bobberTimeAccumulator;
  [XmlIgnore]
  public float timePerBobberBob;
  [XmlIgnore]
  public float timeUntilFishingBite;
  [XmlIgnore]
  public float fishingBiteAccumulator;
  [XmlIgnore]
  public float fishingNibbleAccumulator;
  [XmlIgnore]
  public float timeUntilFishingNibbleDone;
  [XmlIgnore]
  public float castingPower;
  [XmlIgnore]
  public float castingChosenCountdown;
  [XmlIgnore]
  public float castingTimerSpeed;
  [XmlIgnore]
  public bool isFishing;
  [XmlIgnore]
  public bool hit;
  [XmlIgnore]
  public bool isNibbling;
  [XmlIgnore]
  public bool favBait;
  [XmlIgnore]
  public bool isTimingCast;
  [XmlIgnore]
  public bool isCasting;
  [XmlIgnore]
  public bool castedButBobberStillInAir;
  [XmlIgnore]
  public bool gotTroutDerbyTag;
  /// <summary>The cached value for <see cref="M:StardewValley.Tools.FishingRod.GetWaterColor" />.</summary>
  protected Color? lastWaterColor;
  [XmlIgnore]
  protected bool _hasPlayerAdjustedBobber;
  [XmlIgnore]
  public bool lastCatchWasJunk;
  [XmlIgnore]
  public bool goldenTreasure;
  [XmlIgnore]
  public bool doneWithAnimation;
  [XmlIgnore]
  public bool pullingOutOfWater;
  [XmlIgnore]
  public bool isReeling;
  [XmlIgnore]
  public bool hasDoneFucntionYet;
  [XmlIgnore]
  public bool fishCaught;
  [XmlIgnore]
  public bool recordSize;
  [XmlIgnore]
  public bool treasureCaught;
  [XmlIgnore]
  public bool showingTreasure;
  [XmlIgnore]
  public bool hadBobber;
  [XmlIgnore]
  public bool bossFish;
  [XmlIgnore]
  public bool fromFishPond;
  [XmlIgnore]
  public TemporaryAnimatedSpriteList animations;
  [XmlIgnore]
  public SparklingText sparklingText;
  [XmlIgnore]
  public int fishSize;
  [XmlIgnore]
  public int fishQuality;
  [XmlIgnore]
  public int clearWaterDistance;
  [XmlIgnore]
  public int originalFacingDirection;
  [XmlIgnore]
  public int numberOfFishCaught;
  [XmlIgnore]
  public ItemMetadata whichFish;
  /// <summary>The mail flag to set for the current player when the current <see cref="F:StardewValley.Tools.FishingRod.whichFish" /> is successfully caught.</summary>
  [XmlIgnore]
  public string setFlagOnCatch;
  /// <summary>The delay (in milliseconds) before recasting if the left mouse is held down after closing the 'caught fish' display.</summary>
  [XmlIgnore]
  public int recastTimerMs;
  protected const int RECAST_DELAY_MS = 200;
  [XmlIgnore]
  private readonly NetEventBinary pullFishFromWaterEvent;
  [XmlIgnore]
  private readonly NetEvent1Field<bool, NetBool> doneFishingEvent;
  [XmlIgnore]
  private readonly NetEvent0 startCastingEvent;
  [XmlIgnore]
  private readonly NetEvent0 castingEndEnableMovementEvent;
  [XmlIgnore]
  private readonly NetEvent0 putAwayEvent;
  [XmlIgnore]
  private readonly NetEvent0 beginReelingEvent;
  public static ICue chargeSound;
  public static ICue reelSound;
  private int randomBobberStyle;
  private bool usedGamePadToCast;

  /// <summary>The direction in which the fishing rod was cast.</summary>
  public int CastDirection
  {
    get => this.fishCaught ? 2 : this.castDirection.Value;
    set => this.castDirection.Value = value;
  }

  protected override void initNetFields()
  {
    base.initNetFields();
    this.NetFields.AddField((INetSerializable) this.bobber.NetFields, "bobber.NetFields").AddField((INetSerializable) this.castDirection, "castDirection").AddField((INetSerializable) this.pullFishFromWaterEvent, "pullFishFromWaterEvent").AddField((INetSerializable) this.doneFishingEvent, "doneFishingEvent").AddField((INetSerializable) this.startCastingEvent, "startCastingEvent").AddField((INetSerializable) this.castingEndEnableMovementEvent, "castingEndEnableMovementEvent").AddField((INetSerializable) this.putAwayEvent, "putAwayEvent").AddField((INetSerializable) this._totalMotion, "_totalMotion").AddField((INetSerializable) this.beginReelingEvent, "beginReelingEvent");
    this.pullFishFromWaterEvent.AddReaderHandler(new Action<BinaryReader>(this.doPullFishFromWater));
    this.doneFishingEvent.onEvent += new AbstractNetEvent1<bool>.Event(this.doDoneFishing);
    this.startCastingEvent.onEvent += new NetEvent0.Event(this.doStartCasting);
    this.castingEndEnableMovementEvent.onEvent += new NetEvent0.Event(this.doCastingEndEnableMovement);
    this.beginReelingEvent.onEvent += new NetEvent0.Event(this.beginReeling);
    this.putAwayEvent.onEvent += new NetEvent0.Event(((Item) this).resetState);
  }

  /// <inheritdoc />
  protected override void MigrateLegacyItemId()
  {
    switch (this.UpgradeLevel)
    {
      case 0:
        this.ItemId = "BambooPole";
        break;
      case 1:
        this.ItemId = "TrainingRod";
        break;
      case 2:
        this.ItemId = "FiberglassRod";
        break;
      case 3:
        this.ItemId = "IridiumRod";
        break;
      case 4:
        this.ItemId = "AdvancedIridiumRod";
        break;
      default:
        this.ItemId = "BambooPole";
        break;
    }
  }

  public override void actionWhenStopBeingHeld(Farmer who)
  {
    this.putAwayEvent.Fire();
    base.actionWhenStopBeingHeld(who);
  }

  public FishingRod()
  {
    NetVector2 netVector2 = new NetVector2(Vector2.Zero);
    netVector2.InterpolationEnabled = false;
    netVector2.InterpolationWait = false;
    this._totalMotion = netVector2;
    this.timePerBobberBob = 2000f;
    this.timeUntilFishingBite = -1f;
    this.timeUntilFishingNibbleDone = -1f;
    this.castingTimerSpeed = 1f / 1000f;
    this.animations = new TemporaryAnimatedSpriteList();
    this.numberOfFishCaught = 1;
    this.pullFishFromWaterEvent = new NetEventBinary();
    this.doneFishingEvent = new NetEvent1Field<bool, NetBool>();
    this.startCastingEvent = new NetEvent0();
    this.castingEndEnableMovementEvent = new NetEvent0();
    this.putAwayEvent = new NetEvent0();
    this.beginReelingEvent = new NetEvent0();
    this.randomBobberStyle = -1;
    // ISSUE: explicit constructor call
    base.\u002Ector("Fishing Rod", 0, 189, 8, false, 2);
  }

  public override void resetState()
  {
    this.isNibbling = false;
    this.fishCaught = false;
    this.isFishing = false;
    this.isReeling = false;
    this.isCasting = false;
    this.isTimingCast = false;
    this.doneWithAnimation = false;
    this.pullingOutOfWater = false;
    this.fromFishPond = false;
    this.numberOfFishCaught = 1;
    this.fishingBiteAccumulator = 0.0f;
    this.showingTreasure = false;
    this.fishingNibbleAccumulator = 0.0f;
    this.timeUntilFishingBite = -1f;
    this.timeUntilFishingNibbleDone = -1f;
    this.bobberTimeAccumulator = 0.0f;
    this.castingChosenCountdown = 0.0f;
    this.lastWaterColor = new Color?();
    this.gotTroutDerbyTag = false;
    this._totalMotionBufferIndex = 0;
    for (int index = 0; index < this._totalMotionBuffer.Length; ++index)
      this._totalMotionBuffer[index] = Vector2.Zero;
    if (this.lastUser != null && this.lastUser == Game1.player)
      Game1.screenOverlayTempSprites.RemoveWhere<TemporaryAnimatedSprite>((Predicate<TemporaryAnimatedSprite>) (sprite => sprite.id == 987654321));
    this._totalMotion.Value = Vector2.Zero;
    this._lastAppliedMotion = Vector2.Zero;
    this.pullFishFromWaterEvent.Clear();
    this.doneFishingEvent.Clear();
    this.startCastingEvent.Clear();
    this.castingEndEnableMovementEvent.Clear();
    this.beginReelingEvent.Clear();
    this.bobber.Set(Vector2.Zero);
    this.CastDirection = -1;
  }

  public FishingRod(int upgradeLevel)
  {
    NetVector2 netVector2 = new NetVector2(Vector2.Zero);
    netVector2.InterpolationEnabled = false;
    netVector2.InterpolationWait = false;
    this._totalMotion = netVector2;
    this.timePerBobberBob = 2000f;
    this.timeUntilFishingBite = -1f;
    this.timeUntilFishingNibbleDone = -1f;
    this.castingTimerSpeed = 1f / 1000f;
    this.animations = new TemporaryAnimatedSpriteList();
    this.numberOfFishCaught = 1;
    this.pullFishFromWaterEvent = new NetEventBinary();
    this.doneFishingEvent = new NetEvent1Field<bool, NetBool>();
    this.startCastingEvent = new NetEvent0();
    this.castingEndEnableMovementEvent = new NetEvent0();
    this.putAwayEvent = new NetEvent0();
    this.beginReelingEvent = new NetEvent0();
    this.randomBobberStyle = -1;
    // ISSUE: explicit constructor call
    base.\u002Ector("Fishing Rod", upgradeLevel, 189, 8, false, upgradeLevel == 4 ? 3 : 2);
    this.IndexOfMenuItemView = 8 + upgradeLevel;
  }

  public FishingRod(int upgradeLevel, int numAttachmentSlots)
  {
    NetVector2 netVector2 = new NetVector2(Vector2.Zero);
    netVector2.InterpolationEnabled = false;
    netVector2.InterpolationWait = false;
    this._totalMotion = netVector2;
    this.timePerBobberBob = 2000f;
    this.timeUntilFishingBite = -1f;
    this.timeUntilFishingNibbleDone = -1f;
    this.castingTimerSpeed = 1f / 1000f;
    this.animations = new TemporaryAnimatedSpriteList();
    this.numberOfFishCaught = 1;
    this.pullFishFromWaterEvent = new NetEventBinary();
    this.doneFishingEvent = new NetEvent1Field<bool, NetBool>();
    this.startCastingEvent = new NetEvent0();
    this.castingEndEnableMovementEvent = new NetEvent0();
    this.putAwayEvent = new NetEvent0();
    this.beginReelingEvent = new NetEvent0();
    this.randomBobberStyle = -1;
    // ISSUE: explicit constructor call
    base.\u002Ector("Fishing Rod", upgradeLevel, 189, 8, false, numAttachmentSlots);
    this.IndexOfMenuItemView = 8 + upgradeLevel;
  }

  /// <inheritdoc />
  protected override Item GetOneNew() => (Item) new FishingRod();

  private int getAddedDistance(Farmer who)
  {
    if (who.FishingLevel >= 15)
      return 4;
    if (who.FishingLevel >= 8)
      return 3;
    if (who.FishingLevel >= 4)
      return 2;
    return who.FishingLevel >= 1 ? 1 : 0;
  }

  private Vector2 calculateBobberTile() => new Vector2(this.bobber.X / 64f, this.bobber.Y / 64f);

  public override void DoFunction(GameLocation location, int x, int y, int power, Farmer who)
  {
    who = who ?? this.lastUser;
    if (this.fishCaught || !who.IsLocalPlayer && (this.isReeling || this.isFishing || this.pullingOutOfWater))
      return;
    this.hasDoneFucntionYet = true;
    Vector2 bobberTile = this.calculateBobberTile();
    int x1 = (int) bobberTile.X;
    int y1 = (int) bobberTile.Y;
    base.DoFunction(location, x, y, power, who);
    if (this.doneWithAnimation)
      who.canReleaseTool = true;
    if (Game1.isAnyGamePadButtonBeingPressed())
      Game1.lastCursorMotionWasMouse = false;
    if (!this.isFishing && !this.castedButBobberStillInAir && !this.pullingOutOfWater && !this.isNibbling && !this.hit && !this.showingTreasure)
    {
      if (!Game1.eventUp && who.IsLocalPlayer && !this.hasEnchantmentOfType<EfficientToolEnchantment>())
      {
        float stamina = who.Stamina;
        who.Stamina -= (float) (8.0 - (double) who.FishingLevel * 0.10000000149011612);
        who.checkForExhaustion(stamina);
      }
      if (location.canFishHere() && location.isTileFishable(x1, y1))
      {
        this.clearWaterDistance = FishingRod.distanceToLand((int) ((double) this.bobber.X / 64.0), (int) ((double) this.bobber.Y / 64.0), who.currentLocation);
        this.isFishing = true;
        location.temporarySprites.Add(new TemporaryAnimatedSprite(28, 100f, 2, 1, new Vector2(this.bobber.X - 32f, this.bobber.Y - 32f), false, false));
        if (who.IsLocalPlayer)
        {
          if (this.PlayUseSounds)
            location.playSound("dropItemInWater", new Vector2?(bobberTile));
          ++Game1.stats.TimesFished;
        }
        this.timeUntilFishingBite = this.calculateTimeUntilFishingBite(bobberTile, true, who);
        if ((NetFieldBase<Point, NetPoint>) location.fishSplashPoint != (NetPoint) null)
        {
          bool flag = location.fishFrenzyFish.Value != null && !location.fishFrenzyFish.Equals((object) "");
          Rectangle rectangle = new Rectangle(location.fishSplashPoint.X * 64 /*0x40*/, location.fishSplashPoint.Y * 64 /*0x40*/, 64 /*0x40*/, 64 /*0x40*/);
          if (flag)
            rectangle.Inflate(32 /*0x20*/, 32 /*0x20*/);
          if (new Rectangle((int) this.bobber.X - 32 /*0x20*/, (int) this.bobber.Y - 32 /*0x20*/, 64 /*0x40*/, 64 /*0x40*/).Intersects(rectangle))
          {
            this.timeUntilFishingBite /= flag ? 2f : 4f;
            location.temporarySprites.Add(new TemporaryAnimatedSprite(10, this.bobber.Value - new Vector2(32f, 32f), Color.Cyan));
          }
        }
        who.UsingTool = true;
        who.canMove = false;
      }
      else
      {
        if (this.doneWithAnimation)
          who.UsingTool = false;
        if (!this.doneWithAnimation)
          return;
        who.canMove = true;
      }
    }
    else
    {
      if (this.isCasting || this.pullingOutOfWater)
        return;
      bool fromFishPond = location.isTileBuildingFishable((int) bobberTile.X, (int) bobberTile.Y);
      who.FarmerSprite.PauseForSingleAnimation = false;
      int result = who.FacingDirection;
      switch (result)
      {
        case 0:
          who.FarmerSprite.animateBackwardsOnce(299, 35f);
          break;
        case 1:
          who.FarmerSprite.animateBackwardsOnce(300, 35f);
          break;
        case 2:
          who.FarmerSprite.animateBackwardsOnce(301, 35f);
          break;
        case 3:
          who.FarmerSprite.animateBackwardsOnce(302, 35f);
          break;
      }
      if (this.isNibbling)
      {
        StardewValley.Object bait = this.GetBait();
        double num = bait != null ? (double) bait.Price / 10.0 : 0.0;
        bool flag1 = false;
        if ((NetFieldBase<Point, NetPoint>) location.fishSplashPoint != (NetPoint) null)
          flag1 = new Rectangle(location.fishSplashPoint.X * 64 /*0x40*/, location.fishSplashPoint.Y * 64 /*0x40*/, 64 /*0x40*/, 64 /*0x40*/).Intersects(new Rectangle((int) this.bobber.X - 80 /*0x50*/, (int) this.bobber.Y - 80 /*0x50*/, 64 /*0x40*/, 64 /*0x40*/));
        Item o = location.getFish(this.fishingNibbleAccumulator, bait?.QualifiedItemId, this.clearWaterDistance + (flag1 ? 1 : 0), who, num + (flag1 ? 0.4 : 0.0), bobberTile);
        if (o == null || ItemRegistry.GetDataOrErrorItem(o.QualifiedItemId).IsErrorItem)
        {
          result = Game1.random.Next(167, 173);
          o = ItemRegistry.Create("(O)" + result.ToString());
        }
        if ((o is StardewValley.Object @object ? ((double) @object.scale.X == 1.0 ? 1 : 0) : 0) != 0)
          this.favBait = true;
        Dictionary<string, string> dictionary = DataLoader.Fish(Game1.content);
        bool flag2 = false;
        if (!o.HasTypeObject())
        {
          flag2 = true;
        }
        else
        {
          string str;
          if (dictionary.TryGetValue(o.ItemId, out str))
          {
            if (!int.TryParse(str.Split('/')[1], out result))
              flag2 = true;
          }
          else
            flag2 = true;
        }
        this.lastCatchWasJunk = false;
        string qualifiedItemId = o.QualifiedItemId;
        bool flag3;
        if (qualifiedItemId != null)
        {
          switch (qualifiedItemId.Length)
          {
            case 5:
              switch (qualifiedItemId[4])
              {
                case '3':
                  if (!(qualifiedItemId == "(O)73"))
                    goto label_62;
                  break;
                case '9':
                  if (qualifiedItemId == "(O)79")
                    break;
                  goto label_62;
                default:
                  goto label_62;
              }
              break;
            case 6:
              switch (qualifiedItemId[5])
              {
                case '0':
                  if (qualifiedItemId == "(O)890" || qualifiedItemId == "(O)820")
                    break;
                  goto label_62;
                case '1':
                  if (qualifiedItemId == "(O)821")
                    break;
                  goto label_62;
                case '2':
                  if (qualifiedItemId == "(O)152" || qualifiedItemId == "(O)842" || qualifiedItemId == "(O)822")
                    break;
                  goto label_62;
                case '3':
                  if (qualifiedItemId == "(O)153" || qualifiedItemId == "(O)823")
                    break;
                  goto label_62;
                case '4':
                  if (qualifiedItemId == "(O)824")
                    break;
                  goto label_62;
                case '5':
                  if (qualifiedItemId == "(O)825")
                    break;
                  goto label_62;
                case '6':
                  if (qualifiedItemId == "(O)826")
                    break;
                  goto label_62;
                case '7':
                  if (qualifiedItemId == "(O)157" || qualifiedItemId == "(O)797" || qualifiedItemId == "(O)827")
                    break;
                  goto label_62;
                case '8':
                  if (qualifiedItemId == "(O)828")
                    break;
                  goto label_62;
                default:
                  goto label_62;
              }
              break;
            default:
              goto label_62;
          }
          flag3 = true;
          goto label_63;
        }
label_62:
        flag3 = o.Category == -20 || o.QualifiedItemId == GameLocation.CAROLINES_NECKLACE_ITEM_QID;
label_63:
        if (flag3 | fromFishPond | flag2)
        {
          this.lastCatchWasJunk = true;
          this.pullFishFromWater(o.QualifiedItemId, -1, 0, 0, false, false, fromFishPond, o.SetFlagOnPickup, false, 1);
        }
        else
        {
          if (this.hit || !who.IsLocalPlayer)
            return;
          this.hit = true;
          Game1.screenOverlayTempSprites.Add(new TemporaryAnimatedSprite("LooseSprites\\Cursors", new Rectangle(612, 1913, 74, 30), 1500f, 1, 0, Game1.GlobalToLocal(Game1.viewport, this.bobber.Value + new Vector2(-140f, -160f)), false, false, 1f, 0.005f, Color.White, 4f, 0.075f, 0.0f, 0.0f, true)
          {
            scaleChangeChange = -0.005f,
            motion = new Vector2(0.0f, -0.1f),
            endFunction = (TemporaryAnimatedSprite.endBehavior) (_ => this.startMinigameEndFunction(o)),
            id = 987654321
          });
          if (!this.PlayUseSounds)
            return;
          who.playNearbySoundLocal("FishHit");
        }
      }
      else
      {
        if (fromFishPond && Game1.timeOfDay < 2600)
        {
          Item fish = location.getFish(-1f, (string) null, -1, who, -1.0, bobberTile);
          if (fish != null)
          {
            this.pullFishFromWater(fish.QualifiedItemId, -1, 0, 0, false, false, true, (string) null, false, 1);
            return;
          }
        }
        if (this.PlayUseSounds && who.IsLocalPlayer)
          location.playSound("pullItemFromWater", new Vector2?(bobberTile));
        this.isFishing = false;
        this.pullingOutOfWater = true;
        Point standingPixel = who.StandingPixel;
        if (who.FacingDirection == 1 || who.FacingDirection == 3)
        {
          double num1 = (double) Math.Abs(this.bobber.X - (float) standingPixel.X);
          float y2 = 0.005f;
          double num2 = (double) y2;
          float num3 = -(float) Math.Sqrt(num1 * num2 / 2.0);
          float animationInterval = (float) (2.0 * ((double) Math.Abs(num3 - 0.5f) / (double) y2)) * 1.2f;
          this.animations.Add(new TemporaryAnimatedSprite("TileSheets\\bobbers", Game1.getSourceRectForStandardTileSheet(Game1.bobbersTexture, this.getBobberStyle(who), 16 /*0x10*/, 32 /*0x20*/) with
          {
            Height = 16 /*0x10*/
          }, animationInterval, 1, 0, this.bobber.Value + new Vector2(-32f, -48f), false, false, (float) standingPixel.Y / 10000f, 0.0f, Color.White, 4f, 0.0f, 0.0f, (float) Game1.random.Next(-20, 20) / 100f)
          {
            motion = new Vector2((who.FacingDirection == 3 ? -1f : 1f) * (num3 + 0.2f), num3 - 0.8f),
            acceleration = new Vector2(0.0f, y2),
            endFunction = new TemporaryAnimatedSprite.endBehavior(this.donefishingEndFunction),
            timeBasedMotion = true,
            alphaFade = 1f / 1000f,
            flipped = who.FacingDirection == 1 && this.flipCurrentBobberWhenFacingRight()
          });
        }
        else
        {
          float num4 = this.bobber.Y - (float) standingPixel.Y;
          float num5 = Math.Abs(num4 + 256f);
          float y3 = 0.005f;
          float num6 = (float) Math.Sqrt(2.0 * (double) y3 * (double) num5);
          float animationInterval = (float) (Math.Sqrt(2.0 * ((double) num5 - (double) num4) / (double) y3) + (double) num6 / (double) y3);
          this.animations.Add(new TemporaryAnimatedSprite("TileSheets\\bobbers", Game1.getSourceRectForStandardTileSheet(Game1.bobbersTexture, this.getBobberStyle(who), 16 /*0x10*/, 32 /*0x20*/) with
          {
            Height = 16 /*0x10*/
          }, animationInterval, 1, 0, this.bobber.Value + new Vector2(-32f, -48f), false, false, this.bobber.Y / 10000f, 0.0f, Color.White, 4f, 0.0f, 0.0f, (float) Game1.random.Next(-20, 20) / 100f)
          {
            motion = new Vector2((float) (((double) who.StandingPixel.X - (double) this.bobber.Value.X) / 800.0), -num6),
            acceleration = new Vector2(0.0f, y3),
            endFunction = new TemporaryAnimatedSprite.endBehavior(this.donefishingEndFunction),
            timeBasedMotion = true,
            alphaFade = 1f / 1000f
          });
        }
        who.UsingTool = true;
        who.canReleaseTool = false;
      }
    }
  }

  public int getBobberStyle(Farmer who)
  {
    if (this.GetTackleQualifiedItemIDs().Contains("(O)789"))
      return 39;
    if (who == null)
      return 0;
    if (this.randomBobberStyle == -1 && who.usingRandomizedBobber && this.randomBobberStyle == -1)
    {
      who.bobberStyle.Value = Math.Min(FishingRod.NUM_BOBBER_STYLES - 1, Game1.random.Next(Game1.player.fishCaught.Count() / 2));
      this.randomBobberStyle = who.bobberStyle.Value;
    }
    return who.bobberStyle.Value;
  }

  public bool flipCurrentBobberWhenFacingRight()
  {
    switch (this.getBobberStyle(this.getLastFarmerToUse()))
    {
      case 9:
      case 19:
      case 21:
      case 23:
      case 36:
        return true;
      default:
        return false;
    }
  }

  public Color getFishingLineColor()
  {
    switch (this.getBobberStyle(this.getLastFarmerToUse()))
    {
      case 6:
      case 20:
        return new Color((int) byte.MaxValue, 200, (int) byte.MaxValue);
      case 7:
        return Color.Yellow;
      case 9:
        return new Color((int) byte.MaxValue, (int) byte.MaxValue, 200);
      case 10:
        return new Color((int) byte.MaxValue, 208 /*0xD0*/, 169);
      case 11:
        return new Color(170, 170, (int) byte.MaxValue);
      case 12:
        return Color.DimGray;
      case 13:
        return new Color(228, 228, 172);
      case 14:
      case 22:
        return new Color(178, (int) byte.MaxValue, 112 /*0x70*/);
      case 15:
        return new Color(250, 193, 70);
      case 16 /*0x10*/:
        return new Color((int) byte.MaxValue, 170, 170);
      case 17:
        return new Color(200, 220, (int) byte.MaxValue);
      case 25:
      case 27:
        return Color.White * 0.5f;
      case 29:
      case 32 /*0x20*/:
        return Color.Lime * 0.66f;
      case 31 /*0x1F*/:
        return Color.Red * 0.5f;
      case 35:
      case 39:
        return new Color(180, 160 /*0xA0*/, (int) byte.MaxValue);
      case 37:
      case 38:
        return new Color(200, (int) byte.MaxValue, (int) byte.MaxValue);
      default:
        return Color.White;
    }
  }

  private float calculateTimeUntilFishingBite(Vector2 bobberTile, bool isFirstCast, Farmer who)
  {
    if (Game1.currentLocation.isTileBuildingFishable((int) bobberTile.X, (int) bobberTile.Y) && Game1.currentLocation.getBuildingAt(bobberTile) is FishPond buildingAt && buildingAt.currentOccupants.Value > 0)
      return FishPond.FISHING_MILLISECONDS;
    List<string> qualifiedItemIds = this.GetTackleQualifiedItemIDs();
    string qualifiedItemId = this.GetBait()?.QualifiedItemId;
    int num = 0 + Utility.getStringCountInList(qualifiedItemIds, "(O)687") * 10000 + Utility.getStringCountInList(qualifiedItemIds, "(O)686") * 5000;
    float val2 = (float) Game1.random.Next(FishingRod.minFishingBiteTime, Math.Max(FishingRod.minFishingBiteTime, FishingRod.maxFishingBiteTime - 250 * who.FishingLevel - num));
    if (isFirstCast)
      val2 *= 0.75f;
    if (qualifiedItemId != null)
    {
      val2 *= 0.5f;
      if (!(qualifiedItemId == "(O)774") && !(qualifiedItemId == "(O)ChallengeBait"))
      {
        if (qualifiedItemId == "(O)DeluxeBait")
          val2 *= 0.66f;
      }
      else
        val2 *= 0.75f;
    }
    return Math.Max(500f, val2);
  }

  public Color getColor()
  {
    switch (this.upgradeLevel.Value)
    {
      case 0:
        return Color.Goldenrod;
      case 1:
        return Color.OliveDrab;
      case 2:
        return Color.White;
      case 3:
        return Color.Violet;
      case 4:
        return new Color(128 /*0x80*/, 143, (int) byte.MaxValue);
      default:
        return Color.White;
    }
  }

  public static int distanceToLand(
    int tileX,
    int tileY,
    GameLocation location,
    bool landMustBeAdjacentToWalkableTile = false)
  {
    Rectangle r = new Rectangle(tileX - 1, tileY - 1, 3, 3);
    bool flag = false;
    int num = 1;
    while (!flag && r.Width <= 11)
    {
      foreach (Vector2 vector2 in Utility.getBorderOfThisRectangle(r))
      {
        if (location.isTileOnMap(vector2) && !location.isWaterTile((int) vector2.X, (int) vector2.Y))
        {
          flag = true;
          num = r.Width / 2;
          if (landMustBeAdjacentToWalkableTile)
          {
            flag = false;
            foreach (Vector2 surroundingTileLocations in Utility.getSurroundingTileLocationsArray(vector2))
            {
              if (location.isTilePassable(surroundingTileLocations) && !location.isWaterTile((int) vector2.X, (int) vector2.Y))
              {
                flag = true;
                break;
              }
            }
            break;
          }
          break;
        }
      }
      r.Inflate(1, 1);
    }
    if (r.Width > 11)
      num = 6;
    return num - 1;
  }

  public void startMinigameEndFunction(Item fish)
  {
    fish.TryGetTempData<bool>("IsBossFish", out this.bossFish);
    Farmer lastUser = this.lastUser;
    this.beginReelingEvent.Fire();
    this.isReeling = true;
    this.hit = false;
    switch (lastUser.FacingDirection)
    {
      case 1:
        lastUser.FarmerSprite.setCurrentSingleFrame(48 /*0x30*/);
        break;
      case 3:
        lastUser.FarmerSprite.setCurrentSingleFrame(48 /*0x30*/, flip: true);
        break;
    }
    float num1 = 1f * ((float) this.clearWaterDistance / 5f);
    int num2 = 1 + lastUser.FishingLevel / 2;
    float num3 = num1 * ((float) Game1.random.Next(num2, Math.Max(6, num2)) / 5f);
    if (this.favBait)
      num3 *= 1.2f;
    float fishSize = Math.Max(0.0f, Math.Min(1f, num3 * (float) (1.0 + (double) Game1.random.Next(-10, 11) / 100.0)));
    string qualifiedItemId = this.GetBait()?.QualifiedItemId;
    List<string> qualifiedItemIds = this.GetTackleQualifiedItemIDs();
    double num4 = (double) Utility.getStringCountInList(qualifiedItemIds, "(O)693") * FishingRod.baseChanceForTreasure / 3.0;
    this.goldenTreasure = false;
    int num5;
    if (!Game1.isFestival())
    {
      NetStringIntArrayDictionary fishCaught = lastUser.fishCaught;
      if ((fishCaught != null ? (fishCaught.Length > 1 ? 1 : 0) : 0) != 0)
      {
        num5 = Game1.random.NextDouble() < FishingRod.baseChanceForTreasure + (double) lastUser.LuckLevel * 0.005 + (qualifiedItemId == "(O)703" ? FishingRod.baseChanceForTreasure : 0.0) + num4 + lastUser.DailyLuck / 2.0 + (lastUser.professions.Contains(9) ? FishingRod.baseChanceForTreasure : 0.0) ? 1 : 0;
        goto label_9;
      }
    }
    num5 = 0;
label_9:
    bool treasure = num5 != 0;
    if (treasure && Game1.player.stats.Get(StatKeys.Mastery(1)) > 0U && Game1.random.NextDouble() < 0.25 + Game1.player.team.AverageDailyLuck())
      this.goldenTreasure = true;
    Game1.activeClickableMenu = (IClickableMenu) new BobberBar(fish.ItemId, fishSize, treasure, qualifiedItemIds, fish.SetFlagOnPickup, this.bossFish, qualifiedItemId, this.goldenTreasure);
  }

  /// <summary>Get the equipped tackle, if any.</summary>
  public List<StardewValley.Object> GetTackle()
  {
    List<StardewValley.Object> tackle = new List<StardewValley.Object>();
    if (this.CanUseTackle())
    {
      for (int index = 1; index < this.attachments.Count; ++index)
        tackle.Add(this.attachments[index]);
    }
    return tackle;
  }

  public List<string> GetTackleQualifiedItemIDs()
  {
    List<string> qualifiedItemIds = new List<string>();
    foreach (StardewValley.Object @object in this.GetTackle())
    {
      if (@object != null)
        qualifiedItemIds.Add(@object.QualifiedItemId);
    }
    return qualifiedItemIds;
  }

  /// <summary>Get the equipped bait, if any.</summary>
  public StardewValley.Object GetBait() => !this.CanUseBait() ? (StardewValley.Object) null : this.attachments[0];

  /// <summary>Whether the fishing rod has Magic Bait equipped.</summary>
  public bool HasMagicBait() => this.GetBait()?.QualifiedItemId == "(O)908";

  /// <summary>Whether the fishing rod has a Curiosity Lure equipped.</summary>
  public bool HasCuriosityLure() => this.GetTackleQualifiedItemIDs().Contains("(O)856");

  public bool inUse()
  {
    return this.isFishing || this.isCasting || this.isTimingCast || this.isNibbling || this.isReeling || this.fishCaught;
  }

  public void donefishingEndFunction(int extra)
  {
    Farmer lastUser = this.lastUser;
    this.isFishing = false;
    this.isReeling = false;
    lastUser.canReleaseTool = true;
    lastUser.canMove = true;
    lastUser.UsingTool = false;
    lastUser.FarmerSprite.PauseForSingleAnimation = false;
    this.pullingOutOfWater = false;
    this.doneFishing(lastUser);
  }

  public static void endOfAnimationBehavior(Farmer f)
  {
  }

  public override void drawAttachments(SpriteBatch b, int x, int y)
  {
    y += this.enchantments.Count > 0 ? 8 : 4;
    if (this.CanUseBait())
      this.DrawAttachmentSlot(0, b, x, y);
    y += 68;
    if (!this.CanUseTackle())
      return;
    for (int slot = 1; slot < this.AttachmentSlotsCount; ++slot)
    {
      this.DrawAttachmentSlot(slot, b, x, y);
      x += 68;
    }
  }

  /// <inheritdoc />
  protected override void GetAttachmentSlotSprite(
    int slot,
    out Texture2D texture,
    out Rectangle sourceRect)
  {
    base.GetAttachmentSlotSprite(slot, out texture, out sourceRect);
    if (slot == 0)
    {
      if (this.GetBait() != null)
        return;
      sourceRect = Game1.getSourceRectForStandardTileSheet(Game1.menuTexture, 36);
    }
    else
    {
      if (this.attachments[slot] != null)
        return;
      sourceRect = Game1.getSourceRectForStandardTileSheet(Game1.menuTexture, 37);
    }
  }

  /// <inheritdoc />
  protected override bool canThisBeAttached(StardewValley.Object o, int slot)
  {
    if (o.QualifiedItemId == "(O)789" && slot != 0)
      return true;
    return slot != 0 ? o.Category == -22 && this.CanUseTackle() : o.Category == -21 && this.CanUseBait();
  }

  /// <summary>Whether the fishing rod has a bait attachment slot.</summary>
  public bool CanUseBait() => this.AttachmentSlotsCount > 0;

  /// <summary>Whether the fishing rod has a tackle attachment slot.</summary>
  public bool CanUseTackle() => this.AttachmentSlotsCount > 1;

  public void playerCaughtFishEndFunction(bool isBossFish)
  {
    Farmer lastUser = this.lastUser;
    lastUser.Halt();
    lastUser.armOffset = Vector2.Zero;
    this.castedButBobberStillInAir = false;
    this.fishCaught = true;
    this.isReeling = false;
    this.isFishing = false;
    this.pullingOutOfWater = false;
    lastUser.canReleaseTool = false;
    if (!lastUser.IsLocalPlayer)
      return;
    bool flag = this.whichFish.QualifiedItemId.StartsWith("(O)") && !lastUser.fishCaught.ContainsKey(this.whichFish.QualifiedItemId) && !this.whichFish.QualifiedItemId.Equals("(O)388") && !this.whichFish.QualifiedItemId.Equals("(O)390");
    if (!Game1.isFestival())
    {
      this.recordSize = lastUser.caughtFish(this.whichFish.QualifiedItemId, this.fishSize, this.fromFishPond, this.numberOfFishCaught);
      lastUser.faceDirection(2);
    }
    else
    {
      Game1.currentLocation.currentEvent.caughtFish(this.whichFish.QualifiedItemId, this.fishSize, lastUser);
      this.fishCaught = false;
      this.doneFishing(lastUser);
    }
    if (isBossFish)
    {
      Game1.showGlobalMessage(Game1.content.LoadString("Strings\\StringsFromCSFiles:FishingRod.cs.14068"));
      Game1.multiplayer.globalChatInfoMessage("CaughtLegendaryFish", lastUser.Name, TokenStringBuilder.ItemName(this.whichFish.QualifiedItemId));
    }
    else if (this.recordSize)
    {
      this.sparklingText = new SparklingText(Game1.dialogueFont, Game1.content.LoadString("Strings\\StringsFromCSFiles:FishingRod.cs.14069"), Color.LimeGreen, Color.Azure);
      if (!flag)
        lastUser.playNearbySoundLocal("newRecord");
    }
    else
      lastUser.playNearbySoundLocal("fishSlap");
    if (!flag || !lastUser.fishCaught.ContainsKey(this.whichFish.QualifiedItemId))
      return;
    this.sparklingText = new SparklingText(Game1.dialogueFont, Game1.content.LoadString("Strings\\1_6_Strings:FirstCatch"), new Color(200, (int) byte.MaxValue, 220), Color.White);
    lastUser.playNearbySoundLocal("discoverMineral");
  }

  public void pullFishFromWater(
    string fishId,
    int fishSize,
    int fishQuality,
    int fishDifficulty,
    bool treasureCaught,
    bool wasPerfect,
    bool fromFishPond,
    string setFlagOnCatch,
    bool isBossFish,
    int numCaught)
  {
    this.pullFishFromWaterEvent.Fire((NetEventBinary.ArgWriter) (writer =>
    {
      writer.Write(fishId);
      writer.Write(fishSize);
      writer.Write(fishQuality);
      writer.Write(fishDifficulty);
      writer.Write(treasureCaught);
      writer.Write(wasPerfect);
      writer.Write(fromFishPond);
      writer.Write(setFlagOnCatch ?? string.Empty);
      writer.Write(isBossFish);
      writer.Write(numCaught);
    }));
  }

  private void doPullFishFromWater(BinaryReader argReader)
  {
    Farmer lastUser = this.lastUser;
    string itemId = argReader.ReadString();
    int num1 = argReader.ReadInt32();
    int num2 = argReader.ReadInt32();
    int num3 = argReader.ReadInt32();
    bool flag1 = argReader.ReadBoolean();
    bool flag2 = argReader.ReadBoolean();
    bool flag3 = argReader.ReadBoolean();
    string str = argReader.ReadString();
    bool isBossFish = argReader.ReadBoolean();
    int num4 = argReader.ReadInt32();
    this.treasureCaught = flag1;
    this.fishSize = num1;
    this.fishQuality = num2;
    this.whichFish = ItemRegistry.GetMetadata(itemId);
    this.fromFishPond = flag3;
    this.setFlagOnCatch = str != string.Empty ? str : (string) null;
    this.numberOfFishCaught = num4;
    Vector2 bobberTile = this.calculateBobberTile();
    bool flag4 = this.whichFish.TypeIdentifier == "(O)";
    if (num2 >= 2 & flag2)
      this.fishQuality = 4;
    else if (num2 >= 1 & flag2)
      this.fishQuality = 2;
    if (lastUser == null)
      return;
    if (((Game1.isFestival() || !lastUser.IsLocalPlayer ? 0 : (!flag3 ? 1 : 0)) & (flag4 ? 1 : 0)) != 0)
    {
      int howMuch = Math.Max(1, (num2 + 1) * 3 + num3 / 3);
      if (flag1)
        howMuch += (int) ((double) howMuch * 1.2000000476837158);
      if (flag2)
        howMuch += (int) ((double) howMuch * 1.3999999761581421);
      if (isBossFish)
        howMuch *= 5;
      lastUser.gainExperience(1, howMuch);
    }
    if (this.fishQuality < 0)
      this.fishQuality = 0;
    string textureName;
    Rectangle sourceRect;
    if (flag4)
    {
      ParsedItemData parsedOrErrorData = this.whichFish.GetParsedOrErrorData();
      textureName = parsedOrErrorData.TextureName;
      sourceRect = parsedOrErrorData.GetSourceRect();
    }
    else
    {
      textureName = "LooseSprites\\Cursors";
      sourceRect = new Rectangle(228, 408, 16 /*0x10*/, 16 /*0x10*/);
    }
    float animationInterval;
    if (lastUser.FacingDirection == 1 || lastUser.FacingDirection == 3)
    {
      float num5 = Vector2.Distance(this.bobber.Value, lastUser.Position);
      float y1 = 1f / 1000f;
      float num6 = (float) (128.0 - ((double) lastUser.Position.Y - (double) this.bobber.Y + 10.0));
      double a1 = 4.0 * Math.PI / 11.0;
      float f1 = (float) ((double) num5 * (double) y1 * Math.Tan(a1) / Math.Sqrt(2.0 * (double) num5 * (double) y1 * Math.Tan(a1) - 2.0 * (double) y1 * (double) num6));
      if (float.IsNaN(f1))
        f1 = 0.6f;
      float num7 = f1 * (float) (1.0 / Math.Tan(a1));
      animationInterval = num5 / num7;
      this.animations.Add(new TemporaryAnimatedSprite(textureName, sourceRect, animationInterval, 1, 0, this.bobber.Value, false, false, this.bobber.Y / 10000f, 0.0f, Color.White, 4f, 0.0f, 0.0f, 0.0f)
      {
        motion = new Vector2((lastUser.FacingDirection == 3 ? -1f : 1f) * -num7, -f1),
        acceleration = new Vector2(0.0f, y1),
        timeBasedMotion = true,
        endFunction = (TemporaryAnimatedSprite.endBehavior) (_ => this.playerCaughtFishEndFunction(isBossFish)),
        endSound = "tinyWhip"
      });
      if (this.numberOfFishCaught > 1)
      {
        for (int index = 1; index < this.numberOfFishCaught; ++index)
        {
          float num8 = Vector2.Distance(this.bobber.Value, lastUser.Position);
          float y2 = (float) (0.00079999997979030013 - (double) index * 9.9999997473787516E-05);
          float num9 = (float) (128.0 - ((double) lastUser.Position.Y - (double) this.bobber.Y + 10.0));
          double a2 = 4.0 * Math.PI / 11.0;
          float f2 = (float) ((double) num8 * (double) y2 * Math.Tan(a2) / Math.Sqrt(2.0 * (double) num8 * (double) y2 * Math.Tan(a2) - 2.0 * (double) y2 * (double) num9));
          if (float.IsNaN(f2))
            f2 = 0.6f;
          float num10 = f2 * (float) (1.0 / Math.Tan(a2));
          animationInterval = num8 / num10;
          this.animations.Add(new TemporaryAnimatedSprite(textureName, sourceRect, animationInterval, 1, 0, this.bobber.Value, false, false, this.bobber.Y / 10000f, 0.0f, Color.White, 4f, 0.0f, 0.0f, 0.0f)
          {
            motion = new Vector2((lastUser.FacingDirection == 3 ? -1f : 1f) * -num10, -f2),
            acceleration = new Vector2(0.0f, y2),
            timeBasedMotion = true,
            endSound = "fishSlap",
            Parent = lastUser.currentLocation,
            delayBeforeAnimationStart = (index - 1) * 100
          });
        }
      }
    }
    else
    {
      int y3 = lastUser.StandingPixel.Y;
      float num11 = this.bobber.Y - (float) (y3 - 64 /*0x40*/);
      float num12 = Math.Abs((float) ((double) num11 + 256.0 + 32.0));
      if (lastUser.FacingDirection == 0)
        num12 += 96f;
      float y4 = 3f / 1000f;
      float num13 = (float) Math.Sqrt(2.0 * (double) y4 * (double) num12);
      animationInterval = (float) (Math.Sqrt(2.0 * ((double) num12 - (double) num11) / (double) y4) + (double) num13 / (double) y4);
      float x1 = 0.0f;
      if ((double) animationInterval != 0.0)
        x1 = (lastUser.Position.X - this.bobber.X) / animationInterval;
      this.animations.Add(new TemporaryAnimatedSprite(textureName, sourceRect, animationInterval, 1, 0, this.bobber.Value, false, false, this.bobber.Y / 10000f, 0.0f, Color.White, 4f, 0.0f, 0.0f, 0.0f)
      {
        motion = new Vector2(x1, -num13),
        acceleration = new Vector2(0.0f, y4),
        timeBasedMotion = true,
        endFunction = (TemporaryAnimatedSprite.endBehavior) (_ => this.playerCaughtFishEndFunction(isBossFish)),
        endSound = "tinyWhip"
      });
      if (this.numberOfFishCaught > 1)
      {
        for (int index = 1; index < this.numberOfFishCaught; ++index)
        {
          float num14 = this.bobber.Y - (float) (y3 - 64 /*0x40*/);
          float num15 = Math.Abs((float) ((double) num14 + 256.0 + 32.0));
          if (lastUser.FacingDirection == 0)
            num15 += 96f;
          float y5 = (float) (0.0040000001899898052 - (double) index * 0.00050000002374872565);
          float num16 = (float) Math.Sqrt(2.0 * (double) y5 * (double) num15);
          animationInterval = (float) (Math.Sqrt(2.0 * ((double) num15 - (double) num14) / (double) y5) + (double) num16 / (double) y5);
          float x2 = 0.0f;
          if ((double) animationInterval != 0.0)
            x2 = (lastUser.Position.X - this.bobber.X) / animationInterval;
          this.animations.Add(new TemporaryAnimatedSprite(textureName, sourceRect, animationInterval, 1, 0, new Vector2(this.bobber.X, this.bobber.Y), false, false, this.bobber.Y / 10000f, 0.0f, Color.White, 4f, 0.0f, 0.0f, 0.0f)
          {
            motion = new Vector2(x2, -num16),
            acceleration = new Vector2(0.0f, y5),
            timeBasedMotion = true,
            endSound = "fishSlap",
            Parent = lastUser.currentLocation,
            delayBeforeAnimationStart = (index - 1) * 100
          });
        }
      }
    }
    if (this.PlayUseSounds && lastUser.IsLocalPlayer)
    {
      lastUser.currentLocation.playSound("pullItemFromWater", new Vector2?(bobberTile));
      lastUser.currentLocation.playSound("dwop", new Vector2?(bobberTile));
    }
    this.castedButBobberStillInAir = false;
    this.pullingOutOfWater = true;
    this.isFishing = false;
    this.isReeling = false;
    lastUser.FarmerSprite.PauseForSingleAnimation = false;
    switch (lastUser.FacingDirection)
    {
      case 0:
        lastUser.FarmerSprite.animateBackwardsOnce(299, animationInterval);
        break;
      case 1:
        lastUser.FarmerSprite.animateBackwardsOnce(300, animationInterval);
        break;
      case 2:
        lastUser.FarmerSprite.animateBackwardsOnce(301, animationInterval);
        break;
      case 3:
        lastUser.FarmerSprite.animateBackwardsOnce(302, animationInterval);
        break;
    }
  }

  public override void draw(SpriteBatch b)
  {
    base.draw(b);
    Farmer lastUser = this.lastUser;
    float scale = 4f;
    if (!this.bobber.Equals((object) Vector2.Zero) && this.isFishing)
    {
      Vector2 globalPosition = this.bobber.Value;
      if ((double) this.bobberTimeAccumulator > (double) this.timePerBobberBob)
      {
        if (!this.isNibbling && !this.isReeling || Game1.random.NextDouble() < 0.05)
        {
          if (this.PlayUseSounds)
            lastUser.playNearbySoundLocal("waterSlosh");
          lastUser.currentLocation.temporarySprites.Add(new TemporaryAnimatedSprite("TileSheets\\animations", new Rectangle(0, 0, 64 /*0x40*/, 64 /*0x40*/), 150f, 8, 0, new Vector2(this.bobber.X - 32f, this.bobber.Y - 16f), false, Game1.random.NextBool(), 1f / 1000f, 0.01f, Color.White, 0.75f, 3f / 1000f, 0.0f, 0.0f));
        }
        this.timePerBobberBob = this.bobberBob == 0 ? (float) Game1.random.Next(1500, 3500) : (float) Game1.random.Next(350, 750);
        this.bobberTimeAccumulator = 0.0f;
        if (this.isNibbling || this.isReeling)
        {
          this.timePerBobberBob = (float) Game1.random.Next(25, 75);
          globalPosition.X += (float) Game1.random.Next(-5, 5);
          globalPosition.Y += (float) Game1.random.Next(-5, 5);
          if (!this.isReeling)
            scale += (float) Game1.random.Next(-20, 20) / 100f;
        }
        else if (this.PlayUseSounds && Game1.random.NextDouble() < 0.1)
          lastUser.playNearbySoundLocal("bob");
      }
      float layerDepth = globalPosition.Y / 10000f;
      Rectangle rectangle = Game1.getSourceRectForStandardTileSheet(Game1.bobbersTexture, this.getBobberStyle(this.getLastFarmerToUse()), 16 /*0x10*/, 32 /*0x20*/) with
      {
        Height = 16 /*0x10*/
      };
      rectangle.Y += 16 /*0x10*/;
      b.Draw(Game1.bobbersTexture, Game1.GlobalToLocal(Game1.viewport, globalPosition), new Rectangle?(rectangle), Color.White, 0.0f, new Vector2(8f, 8f), scale, this.getLastFarmerToUse().FacingDirection == 1 ? SpriteEffects.FlipHorizontally : SpriteEffects.None, layerDepth);
      rectangle = new Rectangle(rectangle.X, rectangle.Y + 8, rectangle.Width, rectangle.Height - 8);
    }
    else if ((this.isTimingCast || (double) this.castingChosenCountdown > 0.0) && lastUser.IsLocalPlayer)
    {
      int num1 = (int) (-(double) Math.Abs(this.castingChosenCountdown / 2f - this.castingChosenCountdown) / 50.0);
      float num2 = (double) this.castingChosenCountdown <= 0.0 || (double) this.castingChosenCountdown >= 100.0 ? 1f : this.castingChosenCountdown / 100f;
      b.Draw(Game1.mouseCursors, Game1.GlobalToLocal(Game1.viewport, this.getLastFarmerToUse().Position + new Vector2(-48f, (float) (num1 - 160 /*0xA0*/))), new Rectangle?(new Rectangle(193, 1868, 47, 12)), Color.White * num2, 0.0f, Vector2.Zero, 4f, SpriteEffects.None, 0.885f);
      b.Draw(Game1.staminaRect, new Rectangle((int) Game1.GlobalToLocal(Game1.viewport, this.getLastFarmerToUse().Position).X - 32 /*0x20*/ - 4, (int) Game1.GlobalToLocal(Game1.viewport, this.getLastFarmerToUse().Position).Y + num1 - 128 /*0x80*/ - 32 /*0x20*/ + 12, (int) (164.0 * (double) this.castingPower), 25), new Rectangle?(Game1.staminaRect.Bounds), Utility.getRedToGreenLerpColor(this.castingPower) * num2, 0.0f, Vector2.Zero, SpriteEffects.None, 0.887f);
    }
    for (int index = this.animations.Count - 1; index >= 0; --index)
      this.animations[index].draw(b);
    if (this.sparklingText != null && !this.fishCaught)
      this.sparklingText.draw(b, Game1.GlobalToLocal(Game1.viewport, this.getLastFarmerToUse().Position + new Vector2(-24f, -192f)));
    else if (this.sparklingText != null && this.fishCaught)
      this.sparklingText.draw(b, Game1.GlobalToLocal(Game1.viewport, this.getLastFarmerToUse().Position + new Vector2(-64f, -352f)));
    if (!this.bobber.Value.Equals(Vector2.Zero) && (this.isFishing || this.pullingOutOfWater || this.castedButBobberStillInAir) && lastUser.FarmerSprite.CurrentFrame != 57 && (lastUser.FacingDirection != 0 || !this.pullingOutOfWater || this.whichFish == null))
    {
      Vector2 vector2_1 = this.isFishing ? this.bobber.Value : (this.animations.Count > 0 ? this.animations[0].position + new Vector2(0.0f, 4f * scale) : Vector2.Zero);
      if (this.whichFish != null)
        vector2_1 += new Vector2(32f, 32f);
      Vector2 vector2_2 = Vector2.Zero;
      if (this.castedButBobberStillInAir)
      {
        switch (lastUser.FacingDirection)
        {
          case 0:
            switch (lastUser.FarmerSprite.currentAnimationIndex)
            {
              case 0:
                vector2_2 = Game1.GlobalToLocal(Game1.viewport, lastUser.Position + new Vector2(22f, (float) ((double) lastUser.armOffset.Y - 96.0 + 4.0)));
                break;
              case 1:
                vector2_2 = Game1.GlobalToLocal(Game1.viewport, lastUser.Position + new Vector2(32f, (float) ((double) lastUser.armOffset.Y - 96.0 + 4.0)));
                break;
              case 2:
                vector2_2 = Game1.GlobalToLocal(Game1.viewport, lastUser.Position + new Vector2(36f, (float) ((double) lastUser.armOffset.Y - 64.0 + 40.0)));
                break;
              case 3:
                vector2_2 = Game1.GlobalToLocal(Game1.viewport, lastUser.Position + new Vector2(36f, lastUser.armOffset.Y - 16f));
                break;
              case 4:
                vector2_2 = Game1.GlobalToLocal(Game1.viewport, lastUser.Position + new Vector2(36f, lastUser.armOffset.Y - 32f));
                break;
              case 5:
                vector2_2 = Game1.GlobalToLocal(Game1.viewport, lastUser.Position + new Vector2(36f, lastUser.armOffset.Y - 32f));
                break;
              default:
                vector2_2 = Vector2.Zero;
                break;
            }
            break;
          case 1:
            switch (lastUser.FarmerSprite.currentAnimationIndex)
            {
              case 0:
                vector2_2 = Game1.GlobalToLocal(Game1.viewport, lastUser.Position + new Vector2(-48f, (float) ((double) lastUser.armOffset.Y - 96.0 - 8.0)));
                break;
              case 1:
                vector2_2 = Game1.GlobalToLocal(Game1.viewport, lastUser.Position + new Vector2(-16f, (float) ((double) lastUser.armOffset.Y - 96.0 - 20.0)));
                break;
              case 2:
                vector2_2 = Game1.GlobalToLocal(Game1.viewport, lastUser.Position + new Vector2(84f, (float) ((double) lastUser.armOffset.Y - 96.0 - 20.0)));
                break;
              case 3:
                vector2_2 = Game1.GlobalToLocal(Game1.viewport, lastUser.Position + new Vector2(112f, (float) ((double) lastUser.armOffset.Y - 32.0 - 20.0)));
                break;
              case 4:
                vector2_2 = Game1.GlobalToLocal(Game1.viewport, lastUser.Position + new Vector2(120f, (float) ((double) lastUser.armOffset.Y - 32.0 + 8.0)));
                break;
              case 5:
                vector2_2 = Game1.GlobalToLocal(Game1.viewport, lastUser.Position + new Vector2(120f, (float) ((double) lastUser.armOffset.Y - 32.0 + 8.0)));
                break;
              default:
                vector2_2 = Vector2.Zero;
                break;
            }
            break;
          case 2:
            switch (lastUser.FarmerSprite.currentAnimationIndex)
            {
              case 0:
                vector2_2 = Game1.GlobalToLocal(Game1.viewport, lastUser.Position + new Vector2(8f, (float) ((double) lastUser.armOffset.Y - 96.0 + 4.0)));
                break;
              case 1:
                vector2_2 = Game1.GlobalToLocal(Game1.viewport, lastUser.Position + new Vector2(22f, (float) ((double) lastUser.armOffset.Y - 96.0 + 4.0)));
                break;
              case 2:
                vector2_2 = Game1.GlobalToLocal(Game1.viewport, lastUser.Position + new Vector2(28f, (float) ((double) lastUser.armOffset.Y - 64.0 + 40.0)));
                break;
              case 3:
                vector2_2 = Game1.GlobalToLocal(Game1.viewport, lastUser.Position + new Vector2(28f, lastUser.armOffset.Y - 8f));
                break;
              case 4:
                vector2_2 = Game1.GlobalToLocal(Game1.viewport, lastUser.Position + new Vector2(28f, lastUser.armOffset.Y + 32f));
                break;
              case 5:
                vector2_2 = Game1.GlobalToLocal(Game1.viewport, lastUser.Position + new Vector2(28f, lastUser.armOffset.Y + 32f));
                break;
              default:
                vector2_2 = Vector2.Zero;
                break;
            }
            break;
          case 3:
            switch (lastUser.FarmerSprite.currentAnimationIndex)
            {
              case 0:
                vector2_2 = Game1.GlobalToLocal(Game1.viewport, lastUser.Position + new Vector2(112f, (float) ((double) lastUser.armOffset.Y - 96.0 - 8.0)));
                break;
              case 1:
                vector2_2 = Game1.GlobalToLocal(Game1.viewport, lastUser.Position + new Vector2(80f, (float) ((double) lastUser.armOffset.Y - 96.0 - 20.0)));
                break;
              case 2:
                vector2_2 = Game1.GlobalToLocal(Game1.viewport, lastUser.Position + new Vector2(-20f, (float) ((double) lastUser.armOffset.Y - 96.0 - 20.0)));
                break;
              case 3:
                vector2_2 = Game1.GlobalToLocal(Game1.viewport, lastUser.Position + new Vector2(-48f, (float) ((double) lastUser.armOffset.Y - 32.0 - 20.0)));
                break;
              case 4:
                vector2_2 = Game1.GlobalToLocal(Game1.viewport, lastUser.Position + new Vector2(-56f, (float) ((double) lastUser.armOffset.Y - 32.0 + 8.0)));
                break;
              case 5:
                vector2_2 = Game1.GlobalToLocal(Game1.viewport, lastUser.Position + new Vector2(-56f, (float) ((double) lastUser.armOffset.Y - 32.0 + 8.0)));
                break;
            }
            break;
          default:
            vector2_2 = Vector2.Zero;
            break;
        }
      }
      else if (this.isReeling)
      {
        if (lastUser != null && lastUser.IsLocalPlayer && Game1.didPlayerJustClickAtAll())
        {
          switch (lastUser.FacingDirection)
          {
            case 0:
              vector2_2 = Game1.GlobalToLocal(Game1.viewport, lastUser.Position + new Vector2(24f, (float) ((double) lastUser.armOffset.Y - 96.0 + 12.0)));
              break;
            case 1:
              vector2_2 = Game1.GlobalToLocal(Game1.viewport, lastUser.Position + new Vector2(20f, (float) ((double) lastUser.armOffset.Y - 96.0 - 12.0)));
              break;
            case 2:
              vector2_2 = Game1.GlobalToLocal(Game1.viewport, lastUser.Position + new Vector2(12f, (float) ((double) lastUser.armOffset.Y - 96.0 + 8.0)));
              break;
            case 3:
              vector2_2 = Game1.GlobalToLocal(Game1.viewport, lastUser.Position + new Vector2(48f, (float) ((double) lastUser.armOffset.Y - 96.0 - 12.0)));
              break;
          }
        }
        else
        {
          switch (lastUser.FacingDirection)
          {
            case 0:
              vector2_2 = Game1.GlobalToLocal(Game1.viewport, lastUser.Position + new Vector2(25f, (float) ((double) lastUser.armOffset.Y - 96.0 + 4.0)));
              break;
            case 1:
              vector2_2 = Game1.GlobalToLocal(Game1.viewport, lastUser.Position + new Vector2(28f, (float) ((double) lastUser.armOffset.Y - 96.0 - 8.0)));
              break;
            case 2:
              vector2_2 = Game1.GlobalToLocal(Game1.viewport, lastUser.Position + new Vector2(12f, (float) ((double) lastUser.armOffset.Y - 96.0 + 4.0)));
              break;
            case 3:
              vector2_2 = Game1.GlobalToLocal(Game1.viewport, lastUser.Position + new Vector2(36f, (float) ((double) lastUser.armOffset.Y - 96.0 - 8.0)));
              break;
          }
        }
      }
      else
      {
        switch (lastUser.FacingDirection)
        {
          case 0:
            vector2_2 = this.pullingOutOfWater ? Game1.GlobalToLocal(Game1.viewport, lastUser.Position + new Vector2(22f, (float) ((double) lastUser.armOffset.Y - 96.0 + 4.0))) : Game1.GlobalToLocal(Game1.viewport, lastUser.Position + new Vector2(28f, (float) ((double) lastUser.armOffset.Y - 64.0 - 12.0)));
            break;
          case 1:
            vector2_2 = this.pullingOutOfWater ? Game1.GlobalToLocal(Game1.viewport, lastUser.Position + new Vector2(-48f, (float) ((double) lastUser.armOffset.Y - 96.0 - 8.0))) : Game1.GlobalToLocal(Game1.viewport, lastUser.Position + new Vector2(120f, (float) ((double) lastUser.armOffset.Y - 64.0 + 16.0)));
            break;
          case 2:
            vector2_2 = this.pullingOutOfWater ? Game1.GlobalToLocal(Game1.viewport, lastUser.Position + new Vector2(8f, (float) ((double) lastUser.armOffset.Y - 96.0 + 4.0))) : Game1.GlobalToLocal(Game1.viewport, lastUser.Position + new Vector2(28f, (float) ((double) lastUser.armOffset.Y + 64.0 - 12.0)));
            break;
          case 3:
            vector2_2 = this.pullingOutOfWater ? Game1.GlobalToLocal(Game1.viewport, lastUser.Position + new Vector2(112f, (float) ((double) lastUser.armOffset.Y - 96.0 - 8.0))) : Game1.GlobalToLocal(Game1.viewport, lastUser.Position + new Vector2(-56f, (float) ((double) lastUser.armOffset.Y - 64.0 + 16.0)));
            break;
          default:
            vector2_2 = Vector2.Zero;
            break;
        }
      }
      Vector2 local = Game1.GlobalToLocal(Game1.viewport, vector2_1 + new Vector2(0.0f, (float) (-2.5 * (double) scale + (this.bobberBob == 1 ? 4.0 : 0.0))));
      if (this.isTimingCast || this.isCasting && !lastUser.IsLocalPlayer)
        return;
      if (this.isReeling)
      {
        Utility.drawLineWithScreenCoordinates((int) vector2_2.X, (int) vector2_2.Y, (int) local.X, (int) local.Y, b, this.getFishingLineColor() * 0.5f);
      }
      else
      {
        if (!this.isFishing)
          local += new Vector2(20f, 20f);
        if (this.pullingOutOfWater && this.whichFish != null)
          local += new Vector2(-20f, -30f);
        Vector2 p0 = vector2_2;
        Vector2 p1 = new Vector2(vector2_2.X + (float) (((double) local.X - (double) vector2_2.X) / 3.0), vector2_2.Y + (float) (((double) local.Y - (double) vector2_2.Y) * 2.0 / 3.0));
        Vector2 p2 = new Vector2(vector2_2.X + (float) (((double) local.X - (double) vector2_2.X) * 2.0 / 3.0), vector2_2.Y + (float) (((double) local.Y - (double) vector2_2.Y) * (this.isFishing ? 6.0 : 2.0) / 5.0));
        Vector2 p3 = local;
        float layerDepth = (float) (((double) vector2_1.Y > (double) lastUser.StandingPixel.Y ? (double) vector2_1.Y / 10000.0 : (double) lastUser.StandingPixel.Y / 10000.0) + (lastUser.FacingDirection != 0 ? 0.004999999888241291 : -1.0 / 1000.0));
        for (float t = 0.0f; (double) t < 1.0; t += 0.025f)
        {
          Vector2 curvePoint = Utility.GetCurvePoint(t, p0, p1, p2, p3);
          Utility.drawLineWithScreenCoordinates((int) vector2_2.X, (int) vector2_2.Y, (int) curvePoint.X, (int) curvePoint.Y, b, this.getFishingLineColor() * 0.5f, layerDepth);
          vector2_2 = curvePoint;
        }
      }
    }
    else
    {
      if (!this.fishCaught)
        return;
      bool flag = this.whichFish.TypeIdentifier == "(O)";
      float num = (float) (4.0 * Math.Round(Math.Sin(Game1.currentGameTime.TotalGameTime.TotalMilliseconds / 250.0), 2));
      int y = lastUser.StandingPixel.Y;
      b.Draw(Game1.mouseCursors, Game1.GlobalToLocal(Game1.viewport, lastUser.Position + new Vector2(-120f, num - 288f)), new Rectangle?(new Rectangle(31 /*0x1F*/, 1870, 73, 49)), Color.White * 0.8f, 0.0f, Vector2.Zero, 4f, SpriteEffects.None, (float) ((double) y / 10000.0 + 0.059999998658895493));
      if (flag)
      {
        ParsedItemData parsedOrErrorData = this.whichFish.GetParsedOrErrorData();
        Texture2D texture = parsedOrErrorData.GetTexture();
        Rectangle sourceRect = parsedOrErrorData.GetSourceRect();
        b.Draw(texture, Game1.GlobalToLocal(Game1.viewport, lastUser.Position + new Vector2(-124f, num - 284f) + new Vector2(44f, 68f)), new Rectangle?(sourceRect), Color.White, 0.0f, Vector2.Zero, 4f, SpriteEffects.None, (float) ((double) y / 10000.0 + 9.9999997473787516E-05 + 0.059999998658895493));
        if (this.numberOfFishCaught > 1)
          Utility.drawTinyDigits(this.numberOfFishCaught, b, Game1.GlobalToLocal(Game1.viewport, lastUser.Position + new Vector2(-120f, num - 284f) + new Vector2(23f, 29f) * 4f), 3f, (float) ((double) y / 10000.0 + 9.9999997473787516E-05 + 0.061000000685453415), Color.White);
        b.Draw(texture, Game1.GlobalToLocal(Game1.viewport, lastUser.Position + new Vector2(0.0f, -56f)), new Rectangle?(sourceRect), Color.White, this.fishSize == -1 || this.whichFish.QualifiedItemId == "(O)800" || this.whichFish.QualifiedItemId == "(O)798" || this.whichFish.QualifiedItemId == "(O)149" || this.whichFish.QualifiedItemId == "(O)151" ? 0.0f : 2.3561945f, new Vector2(8f, 8f), 3f, SpriteEffects.None, (float) ((double) y / 10000.0 + 1.0 / 500.0 + 0.059999998658895493));
        if (this.numberOfFishCaught > 1)
        {
          for (int index = 1; index < this.numberOfFishCaught; ++index)
            b.Draw(texture, Game1.GlobalToLocal(Game1.viewport, lastUser.Position + new Vector2((float) -(12 * index), -56f)), new Rectangle?(sourceRect), Color.White, this.fishSize == -1 || this.whichFish.QualifiedItemId == "(O)800" || this.whichFish.QualifiedItemId == "(O)798" || this.whichFish.QualifiedItemId == "(O)149" || this.whichFish.QualifiedItemId == "(O)151" ? 0.0f : (index == 2 ? 3.14159274f : 2.51327419f), new Vector2(8f, 8f), 3f, SpriteEffects.None, (float) ((double) y / 10000.0 + 1.0 / 500.0 + 0.057999998331069946));
        }
      }
      else
      {
        b.Draw(Game1.mouseCursors, Game1.GlobalToLocal(Game1.viewport, lastUser.Position + new Vector2(-124f, num - 284f) + new Vector2(44f, 68f)), new Rectangle?(new Rectangle(228, 408, 16 /*0x10*/, 16 /*0x10*/)), Color.White, 0.0f, Vector2.Zero, 4f, SpriteEffects.None, (float) ((double) y / 10000.0 + 9.9999997473787516E-05 + 0.059999998658895493));
        b.Draw(Game1.mouseCursors, Game1.GlobalToLocal(Game1.viewport, lastUser.Position + new Vector2(0.0f, -56f)), new Rectangle?(new Rectangle(228, 408, 16 /*0x10*/, 16 /*0x10*/)), Color.White, 0.0f, new Vector2(8f, 8f), 3f, SpriteEffects.None, (float) ((double) y / 10000.0 + 1.0 / 500.0 + 0.059999998658895493));
      }
      string text = flag ? this.whichFish.GetParsedOrErrorData().DisplayName : "???";
      b.DrawString(Game1.smallFont, text, Game1.GlobalToLocal(Game1.viewport, lastUser.Position + new Vector2((float) (26.0 - (double) Game1.smallFont.MeasureString(text).X / 2.0), num - 278f)), this.bossFish ? new Color(126, 61, 237) : Game1.textColor, 0.0f, Vector2.Zero, 1f, SpriteEffects.None, (float) ((double) y / 10000.0 + 1.0 / 500.0 + 0.059999998658895493));
      if (this.fishSize == -1)
        return;
      b.DrawString(Game1.smallFont, Game1.content.LoadString("Strings\\StringsFromCSFiles:FishingRod.cs.14082"), Game1.GlobalToLocal(Game1.viewport, lastUser.Position + new Vector2(20f, num - 214f)), Game1.textColor, 0.0f, Vector2.Zero, 1f, SpriteEffects.None, (float) ((double) y / 10000.0 + 1.0 / 500.0 + 0.059999998658895493));
      b.DrawString(Game1.smallFont, Game1.content.LoadString("Strings\\StringsFromCSFiles:FishingRod.cs.14083", (object) (LocalizedContentManager.CurrentLanguageCode != LocalizedContentManager.LanguageCode.en ? Math.Round((double) this.fishSize * 2.54) : (double) this.fishSize)), Game1.GlobalToLocal(Game1.viewport, lastUser.Position + new Vector2((float) (85.0 - (double) Game1.smallFont.MeasureString(Game1.content.LoadString("Strings\\StringsFromCSFiles:FishingRod.cs.14083", (object) (LocalizedContentManager.CurrentLanguageCode != LocalizedContentManager.LanguageCode.en ? Math.Round((double) this.fishSize * 2.54) : (double) this.fishSize))).X / 2.0), num - 179f)), this.recordSize ? Color.Blue * Math.Min(1f, (float) ((double) num / 8.0 + 1.5)) : Game1.textColor, 0.0f, Vector2.Zero, 1f, SpriteEffects.None, (float) ((double) y / 10000.0 + 1.0 / 500.0 + 0.059999998658895493));
    }
  }

  /// <summary>Get the color of the water which the bobber is submerged in.</summary>
  public Color GetWaterColor()
  {
    if (this.lastWaterColor.HasValue)
      return this.lastWaterColor.Value;
    GameLocation gameLocation = this.lastUser?.currentLocation ?? Game1.currentLocation;
    Vector2 bobberTile = this.calculateBobberTile();
    if (bobberTile != Vector2.Zero)
    {
      foreach (Building building in gameLocation.buildings)
      {
        if (building.isTileFishable(bobberTile))
        {
          this.lastWaterColor = building.GetWaterColor(bobberTile);
          if (this.lastWaterColor.HasValue)
            return this.lastWaterColor.Value;
          break;
        }
      }
    }
    this.lastWaterColor = new Color?(gameLocation.waterColor.Value);
    return this.lastWaterColor.Value;
  }

  public override bool beginUsing(GameLocation location, int x, int y, Farmer who)
  {
    if ((double) who.Stamina <= 1.0 && who.IsLocalPlayer)
    {
      if (!who.isEmoting)
        who.doEmote(36);
      who.CanMove = !Game1.eventUp;
      who.UsingTool = false;
      who.canReleaseTool = false;
      this.doneFishing((Farmer) null);
      return true;
    }
    this.usedGamePadToCast = false;
    if (Game1.input.GetGamePadState().IsButtonDown(Buttons.X))
      this.usedGamePadToCast = true;
    this.bossFish = false;
    this.originalFacingDirection = who.FacingDirection;
    if (who.IsLocalPlayer || who.isFakeEventActor)
      this.CastDirection = this.originalFacingDirection;
    who.Halt();
    this.treasureCaught = false;
    this.showingTreasure = false;
    this.isFishing = false;
    this.hit = false;
    this.favBait = false;
    if (this.GetTackle().Count > 0)
    {
      bool flag = false;
      foreach (StardewValley.Object @object in this.GetTackle())
      {
        if (@object != null)
        {
          flag = true;
          break;
        }
      }
      this.hadBobber = flag;
    }
    this.isNibbling = false;
    this.lastUser = who;
    this.lastWaterColor = new Color?();
    this.isTimingCast = true;
    this._totalMotionBufferIndex = 0;
    for (int index = 0; index < this._totalMotionBuffer.Length; ++index)
      this._totalMotionBuffer[index] = Vector2.Zero;
    this._totalMotion.Value = Vector2.Zero;
    this._lastAppliedMotion = Vector2.Zero;
    who.UsingTool = true;
    this.whichFish = (ItemMetadata) null;
    this.recastTimerMs = 0;
    who.canMove = false;
    this.fishCaught = false;
    this.doneWithAnimation = false;
    who.canReleaseTool = false;
    this.hasDoneFucntionYet = false;
    this.isReeling = false;
    this.pullingOutOfWater = false;
    this.castingPower = 0.0f;
    this.castingChosenCountdown = 0.0f;
    this.animations.Clear();
    this.sparklingText = (SparklingText) null;
    this.setTimingCastAnimation(who);
    return true;
  }

  public void setTimingCastAnimation(Farmer who)
  {
    if (who.CurrentTool == null)
      return;
    switch (who.FacingDirection)
    {
      case 0:
        who.FarmerSprite.setCurrentFrame(295);
        who.CurrentTool.Update(0, 0, who);
        break;
      case 1:
        who.FarmerSprite.setCurrentFrame(296);
        who.CurrentTool.Update(1, 0, who);
        break;
      case 2:
        who.FarmerSprite.setCurrentFrame(297);
        who.CurrentTool.Update(2, 0, who);
        break;
      case 3:
        who.FarmerSprite.setCurrentFrame(298);
        who.CurrentTool.Update(3, 0, who);
        break;
    }
  }

  public void doneFishing(Farmer who, bool consumeBaitAndTackle = false)
  {
    this.doneFishingEvent.Fire(consumeBaitAndTackle);
  }

  private void doDoneFishing(bool consumeBaitAndTackle)
  {
    Farmer lastUser = this.lastUser;
    if (consumeBaitAndTackle && lastUser != null && lastUser.IsLocalPlayer)
    {
      float num = 1f;
      if (this.hasEnchantmentOfType<PreservingEnchantment>())
        num = 0.5f;
      StardewValley.Object bait = this.GetBait();
      if (bait != null && Game1.random.NextDouble() < (double) num && bait.ConsumeStack(1) == null)
      {
        this.attachments[0] = (StardewValley.Object) null;
        Game1.showGlobalMessage(Game1.content.LoadString("Strings\\StringsFromCSFiles:FishingRod.cs.14085"));
      }
      int index = 1;
      foreach (StardewValley.Object @object in this.GetTackle())
      {
        if (@object != null && !this.lastCatchWasJunk && Game1.random.NextDouble() < (double) num)
        {
          if (!(@object.QualifiedItemId == "(O)789"))
          {
            ++@object.uses.Value;
            if (@object.uses.Value >= FishingRod.maxTackleUses)
            {
              this.attachments[index] = (StardewValley.Object) null;
              Game1.showGlobalMessage(Game1.content.LoadString("Strings\\StringsFromCSFiles:FishingRod.cs.14086"));
            }
          }
          else
            break;
        }
        ++index;
      }
    }
    if (lastUser != null && lastUser.IsLocalPlayer)
      this.bobber.Set(Vector2.Zero);
    this.isNibbling = false;
    this.fishCaught = false;
    this.isFishing = false;
    this.isReeling = false;
    this.isCasting = false;
    this.isTimingCast = false;
    this.treasureCaught = false;
    this.showingTreasure = false;
    this.doneWithAnimation = false;
    this.pullingOutOfWater = false;
    this.fromFishPond = false;
    this.numberOfFishCaught = 1;
    this.fishingBiteAccumulator = 0.0f;
    this.fishingNibbleAccumulator = 0.0f;
    this.timeUntilFishingBite = -1f;
    this.timeUntilFishingNibbleDone = -1f;
    this.bobberTimeAccumulator = 0.0f;
    if (FishingRod.chargeSound != null && FishingRod.chargeSound.IsPlaying && lastUser.IsLocalPlayer)
    {
      FishingRod.chargeSound.Stop(AudioStopOptions.Immediate);
      FishingRod.chargeSound = (ICue) null;
    }
    if (FishingRod.reelSound != null && FishingRod.reelSound.IsPlaying)
    {
      FishingRod.reelSound.Stop(AudioStopOptions.Immediate);
      FishingRod.reelSound = (ICue) null;
    }
    if (lastUser == null)
      return;
    lastUser.UsingTool = false;
    lastUser.CanMove = true;
    lastUser.completelyStopAnimatingOrDoingAction();
    if (lastUser != Game1.player)
      return;
    lastUser.faceDirection(this.originalFacingDirection);
  }

  public static void doneWithCastingAnimation(Farmer who)
  {
    if (!(who.CurrentTool is FishingRod currentTool))
      return;
    currentTool.doneWithAnimation = true;
    if (!currentTool.hasDoneFucntionYet)
      return;
    who.canReleaseTool = true;
    who.UsingTool = false;
    who.canMove = true;
    Farmer.canMoveNow(who);
  }

  public void castingEndFunction(Farmer who)
  {
    this.lastWaterColor = new Color?();
    this.castedButBobberStillInAir = false;
    if (who == null)
      return;
    float stamina = who.Stamina;
    this.DoFunction(who.currentLocation, (int) this.bobber.X, (int) this.bobber.Y, 1, who);
    who.lastClick = Vector2.Zero;
    FishingRod.reelSound?.Stop(AudioStopOptions.Immediate);
    FishingRod.reelSound = (ICue) null;
    if ((double) who.Stamina <= 0.0 && (double) stamina > 0.0)
      who.doEmote(36);
    if (this.isFishing || !this.doneWithAnimation)
      return;
    this.castingEndEnableMovement();
  }

  private void castingEndEnableMovement() => this.castingEndEnableMovementEvent.Fire();

  private void doCastingEndEnableMovement() => Farmer.canMoveNow(this.lastUser);

  public override void tickUpdate(GameTime time, Farmer who)
  {
    this.lastUser = who;
    this.beginReelingEvent.Poll();
    this.putAwayEvent.Poll();
    this.startCastingEvent.Poll();
    this.pullFishFromWaterEvent.Poll();
    this.doneFishingEvent.Poll();
    this.castingEndEnableMovementEvent.Poll();
    TimeSpan timeSpan;
    if (this.recastTimerMs > 0 && who.IsLocalPlayer && who.freezePause <= 0)
    {
      if (Game1.input.GetMouseState().LeftButton == ButtonState.Pressed || Game1.didPlayerJustClickAtAll() || Game1.isOneOfTheseKeysDown(Game1.oldKBState, Game1.options.useToolButton))
      {
        int recastTimerMs = this.recastTimerMs;
        timeSpan = time.ElapsedGameTime;
        int milliseconds = timeSpan.Milliseconds;
        this.recastTimerMs = recastTimerMs - milliseconds;
        if (this.recastTimerMs <= 0)
        {
          this.recastTimerMs = 0;
          if (Game1.activeClickableMenu == null)
            who.BeginUsingTool();
        }
      }
      else
        this.recastTimerMs = 0;
    }
    if (this.isFishing && !Game1.shouldTimePass())
    {
      switch (Game1.activeClickableMenu)
      {
        case null:
        case BobberBar _:
          break;
        default:
          return;
      }
    }
    if (who.CurrentTool != null && who.CurrentTool.Equals((object) this) && who.UsingTool)
      who.CanMove = false;
    else if (Game1.currentMinigame == null && (!(who.CurrentTool is FishingRod) || !who.UsingTool))
    {
      if (FishingRod.chargeSound == null || !FishingRod.chargeSound.IsPlaying || !who.IsLocalPlayer)
        return;
      FishingRod.chargeSound.Stop(AudioStopOptions.Immediate);
      FishingRod.chargeSound = (ICue) null;
      return;
    }
    this.animations.RemoveWhere<TemporaryAnimatedSprite>((Predicate<TemporaryAnimatedSprite>) (animation => animation.update(time)));
    if (this.sparklingText != null && this.sparklingText.update(time))
      this.sparklingText = (SparklingText) null;
    if ((double) this.castingChosenCountdown > 0.0)
    {
      double castingChosenCountdown = (double) this.castingChosenCountdown;
      timeSpan = time.ElapsedGameTime;
      double milliseconds = (double) timeSpan.Milliseconds;
      this.castingChosenCountdown = (float) (castingChosenCountdown - milliseconds);
      if ((double) this.castingChosenCountdown <= 0.0 && who.CurrentTool != null)
      {
        switch (who.FacingDirection)
        {
          case 0:
            who.FarmerSprite.animateOnce(295, 1f, 1);
            who.CurrentTool.Update(0, 0, who);
            break;
          case 1:
            who.FarmerSprite.animateOnce(296, 1f, 1);
            who.CurrentTool.Update(1, 0, who);
            break;
          case 2:
            who.FarmerSprite.animateOnce(297, 1f, 1);
            who.CurrentTool.Update(2, 0, who);
            break;
          case 3:
            who.FarmerSprite.animateOnce(298, 1f, 1);
            who.CurrentTool.Update(3, 0, who);
            break;
        }
        if (who.FacingDirection == 1 || who.FacingDirection == 3)
        {
          float num1 = Math.Max(128f, (float) ((double) this.castingPower * (double) (this.getAddedDistance(who) + 4) * 64.0)) - 8f;
          float y = 0.005f;
          float num2 = num1 * (float) Math.Sqrt((double) y / (2.0 * ((double) num1 + 96.0)));
          float animationInterval = (float) (2.0 * ((double) num2 / (double) y)) + ((float) Math.Sqrt((double) num2 * (double) num2 + 2.0 * (double) y * 96.0) - num2) / y;
          Point standingPixel = who.StandingPixel;
          if (who.IsLocalPlayer)
            this.bobber.Set(new Vector2((float) standingPixel.X + (who.FacingDirection == 3 ? -1f : 1f) * num1, (float) standingPixel.Y));
          this.animations.Add(new TemporaryAnimatedSprite("TileSheets\\bobbers", Game1.getSourceRectForStandardTileSheet(Game1.bobbersTexture, this.getBobberStyle(who), 16 /*0x10*/, 32 /*0x20*/) with
          {
            Height = 16 /*0x10*/
          }, animationInterval, 1, 0, who.Position + new Vector2(0.0f, -96f), false, false, (float) standingPixel.Y / 10000f, 0.0f, Color.White, 4f, 0.0f, 0.0f, (float) Game1.random.Next(-20, 20) / 100f)
          {
            motion = new Vector2((who.FacingDirection == 3 ? -1f : 1f) * num2, -num2),
            acceleration = new Vector2(0.0f, y),
            endFunction = (TemporaryAnimatedSprite.endBehavior) (_ => this.castingEndFunction(who)),
            timeBasedMotion = true,
            flipped = who.FacingDirection == 1 && this.flipCurrentBobberWhenFacingRight()
          });
        }
        else
        {
          float num3 = -Math.Max(128f, (float) ((double) this.castingPower * (double) (this.getAddedDistance(who) + 3) * 64.0));
          float num4 = Math.Abs(num3 - 64f);
          if (who.FacingDirection == 0)
          {
            num3 = -num3;
            num4 += 64f;
          }
          float y = 0.005f;
          float num5 = (float) Math.Sqrt(2.0 * (double) y * (double) num4);
          float animationInterval = (float) (Math.Sqrt(2.0 * ((double) num4 - (double) num3) / (double) y) + (double) num5 / (double) y) * 1.05f;
          if (who.FacingDirection == 0)
            animationInterval *= 1.05f;
          if (who.IsLocalPlayer)
          {
            Point standingPixel = who.StandingPixel;
            this.bobber.Set(new Vector2((float) standingPixel.X, (float) standingPixel.Y - num3));
          }
          this.animations.Add(new TemporaryAnimatedSprite("TileSheets\\bobbers", Game1.getSourceRectForStandardTileSheet(Game1.bobbersTexture, this.getBobberStyle(who), 16 /*0x10*/, 32 /*0x20*/) with
          {
            Height = 16 /*0x10*/
          }, animationInterval, 1, 0, who.Position + new Vector2(0.0f, -96f), false, false, this.bobber.Y / 10000f, 0.0f, Color.White, 4f, 0.0f, 0.0f, (float) Game1.random.Next(-20, 20) / 100f)
          {
            alphaFade = 0.0001f,
            motion = new Vector2(0.0f, -num5),
            acceleration = new Vector2(0.0f, y),
            endFunction = (TemporaryAnimatedSprite.endBehavior) (_ => this.castingEndFunction(who)),
            timeBasedMotion = true
          });
        }
        this._hasPlayerAdjustedBobber = false;
        this.castedButBobberStillInAir = true;
        this.isCasting = false;
        if (this.PlayUseSounds && who.IsLocalPlayer)
        {
          who.playNearbySoundAll("cast");
          Game1.playSound("slowReel", 1600, out FishingRod.reelSound);
        }
      }
    }
    else if (!this.isTimingCast && (double) this.castingChosenCountdown <= 0.0)
      who.jitterStrength = 0.0f;
    if (this.isTimingCast)
    {
      double castingPower = (double) this.castingPower;
      double castingTimerSpeed = (double) this.castingTimerSpeed;
      timeSpan = time.ElapsedGameTime;
      double milliseconds = (double) timeSpan.Milliseconds;
      double num6 = castingTimerSpeed * milliseconds;
      this.castingPower = Math.Max(0.0f, Math.Min(1f, (float) (castingPower + num6)));
      if (this.PlayUseSounds && who.IsLocalPlayer)
      {
        if (FishingRod.chargeSound == null || !FishingRod.chargeSound.IsPlaying)
          Game1.playSound("SinWave", out FishingRod.chargeSound);
        Game1.sounds.SetPitch(FishingRod.chargeSound, 2400f * this.castingPower);
      }
      if ((double) this.castingPower == 1.0 || (double) this.castingPower == 0.0)
        this.castingTimerSpeed = -this.castingTimerSpeed;
      ref Vector2 local = ref who.armOffset;
      timeSpan = Game1.currentGameTime.TotalGameTime;
      double num7 = 2.0 * Math.Round(Math.Sin(timeSpan.TotalMilliseconds / 250.0), 2);
      local.Y = (float) num7;
      who.jitterStrength = Math.Max(0.0f, this.castingPower - 0.5f);
      if (!who.IsLocalPlayer || (this.usedGamePadToCast || Game1.input.GetMouseState().LeftButton != ButtonState.Released) && (!this.usedGamePadToCast || !Game1.options.gamepadControls || !Game1.input.GetGamePadState().IsButtonUp(Buttons.X)) || !Game1.areAllOfTheseKeysUp(Game1.GetKeyboardState(), Game1.options.useToolButton))
        return;
      this.startCasting();
    }
    else if (this.isReeling)
    {
      if (who.IsLocalPlayer && Game1.didPlayerJustClickAtAll())
      {
        if (Game1.isAnyGamePadButtonBeingPressed())
          Game1.lastCursorMotionWasMouse = false;
        switch (who.FacingDirection)
        {
          case 0:
            who.FarmerSprite.setCurrentSingleFrame(76);
            break;
          case 1:
            who.FarmerSprite.setCurrentSingleFrame(72, (short) 100);
            break;
          case 2:
            who.FarmerSprite.setCurrentSingleFrame(75);
            break;
          case 3:
            who.FarmerSprite.setCurrentSingleFrame(72, (short) 100, flip: true);
            break;
        }
        ref Vector2 local = ref who.armOffset;
        timeSpan = Game1.currentGameTime.TotalGameTime;
        double num = Math.Round(Math.Sin(timeSpan.TotalMilliseconds / 250.0), 2);
        local.Y = (float) num;
        who.jitterStrength = 1f;
      }
      else
      {
        switch (who.FacingDirection)
        {
          case 0:
            who.FarmerSprite.setCurrentSingleFrame(36);
            break;
          case 1:
            who.FarmerSprite.setCurrentSingleFrame(48 /*0x30*/, (short) 100);
            break;
          case 2:
            who.FarmerSprite.setCurrentSingleFrame(66);
            break;
          case 3:
            who.FarmerSprite.setCurrentSingleFrame(48 /*0x30*/, (short) 100, flip: true);
            break;
        }
        who.stopJittering();
      }
      who.armOffset = new Vector2((float) Game1.random.Next(-10, 11) / 10f, (float) Game1.random.Next(-10, 11) / 10f);
      double bobberTimeAccumulator = (double) this.bobberTimeAccumulator;
      timeSpan = time.ElapsedGameTime;
      double milliseconds = (double) timeSpan.Milliseconds;
      this.bobberTimeAccumulator = (float) (bobberTimeAccumulator + milliseconds);
    }
    else if (this.isFishing)
    {
      if (who.IsLocalPlayer)
      {
        NetPosition bobber = this.bobber;
        double y = (double) bobber.Y;
        timeSpan = Game1.currentGameTime.TotalGameTime;
        double num = 0.11999999731779099 * Math.Sin(timeSpan.TotalMilliseconds / 250.0);
        bobber.Y = (float) (y + num);
      }
      who.canReleaseTool = true;
      double bobberTimeAccumulator = (double) this.bobberTimeAccumulator;
      timeSpan = time.ElapsedGameTime;
      double milliseconds1 = (double) timeSpan.Milliseconds;
      this.bobberTimeAccumulator = (float) (bobberTimeAccumulator + milliseconds1);
      switch (who.FacingDirection)
      {
        case 0:
          who.FarmerSprite.setCurrentFrame(44);
          break;
        case 1:
          who.FarmerSprite.setCurrentFrame(89);
          break;
        case 2:
          who.FarmerSprite.setCurrentFrame(70);
          break;
        case 3:
          who.FarmerSprite.setCurrentFrame(89, 0, 10, 1, true, false);
          break;
      }
      ref Vector2 local = ref who.armOffset;
      timeSpan = Game1.currentGameTime.TotalGameTime;
      double num8 = Math.Round(Math.Sin(timeSpan.TotalMilliseconds / 250.0), 2) + (who.FacingDirection == 1 || who.FacingDirection == 3 ? 1.0 : -1.0);
      local.Y = (float) num8;
      if (!who.IsLocalPlayer)
        return;
      if ((double) this.timeUntilFishingBite != -1.0)
      {
        double fishingBiteAccumulator = (double) this.fishingBiteAccumulator;
        timeSpan = time.ElapsedGameTime;
        double milliseconds2 = (double) timeSpan.Milliseconds;
        this.fishingBiteAccumulator = (float) (fishingBiteAccumulator + milliseconds2);
        if ((double) this.fishingBiteAccumulator > (double) this.timeUntilFishingBite)
        {
          this.fishingBiteAccumulator = 0.0f;
          this.timeUntilFishingBite = -1f;
          this.isNibbling = true;
          if (this.hasEnchantmentOfType<AutoHookEnchantment>())
          {
            this.timePerBobberBob = 1f;
            this.timeUntilFishingNibbleDone = (float) FishingRod.maxTimeToNibble;
            this.DoFunction(who.currentLocation, (int) this.bobber.X, (int) this.bobber.Y, 1, who);
            Rumble.rumble(0.95f, 200f);
            return;
          }
          who.PlayFishBiteChime();
          Rumble.rumble(0.75f, 250f);
          this.timeUntilFishingNibbleDone = (float) FishingRod.maxTimeToNibble;
          Point standingPixel = who.StandingPixel;
          Game1.screenOverlayTempSprites.Add(new TemporaryAnimatedSprite("LooseSprites\\Cursors", new Rectangle(395, 497, 3, 8), new Vector2((float) (standingPixel.X - Game1.viewport.X), (float) (standingPixel.Y - 128 /*0x80*/ - 8 - Game1.viewport.Y)), false, 0.02f, Color.White)
          {
            scale = 5f,
            scaleChange = -0.01f,
            motion = new Vector2(0.0f, -0.5f),
            shakeIntensityChange = -0.005f,
            shakeIntensity = 1f
          });
          this.timePerBobberBob = 1f;
        }
      }
      if ((double) this.timeUntilFishingNibbleDone == -1.0 || this.hit)
        return;
      double nibbleAccumulator = (double) this.fishingNibbleAccumulator;
      timeSpan = time.ElapsedGameTime;
      double milliseconds3 = (double) timeSpan.Milliseconds;
      this.fishingNibbleAccumulator = (float) (nibbleAccumulator + milliseconds3);
      if ((double) this.fishingNibbleAccumulator <= (double) this.timeUntilFishingNibbleDone)
        return;
      this.fishingNibbleAccumulator = 0.0f;
      this.timeUntilFishingNibbleDone = -1f;
      this.isNibbling = false;
      this.timeUntilFishingBite = this.calculateTimeUntilFishingBite(this.calculateBobberTile(), false, who);
    }
    else if (who.UsingTool && this.castedButBobberStillInAir)
    {
      Vector2 zero1 = Vector2.Zero;
      if ((Game1.isOneOfTheseKeysDown(Game1.oldKBState, Game1.options.moveDownButton) || Game1.options.gamepadControls && (Game1.oldPadState.IsButtonDown(Buttons.DPadDown) || (double) Game1.input.GetGamePadState().ThumbSticks.Left.Y < 0.0)) && who.FacingDirection != 2 && who.FacingDirection != 0)
      {
        zero1.Y += 4f;
        this._hasPlayerAdjustedBobber = true;
      }
      if ((Game1.isOneOfTheseKeysDown(Game1.oldKBState, Game1.options.moveRightButton) || Game1.options.gamepadControls && (Game1.oldPadState.IsButtonDown(Buttons.DPadRight) || (double) Game1.input.GetGamePadState().ThumbSticks.Left.X > 0.0)) && who.FacingDirection != 1 && who.FacingDirection != 3)
      {
        zero1.X += 2f;
        this._hasPlayerAdjustedBobber = true;
      }
      if ((Game1.isOneOfTheseKeysDown(Game1.oldKBState, Game1.options.moveUpButton) || Game1.options.gamepadControls && (Game1.oldPadState.IsButtonDown(Buttons.DPadUp) || (double) Game1.input.GetGamePadState().ThumbSticks.Left.Y > 0.0)) && who.FacingDirection != 0 && who.FacingDirection != 2)
      {
        zero1.Y -= 4f;
        this._hasPlayerAdjustedBobber = true;
      }
      if ((Game1.isOneOfTheseKeysDown(Game1.oldKBState, Game1.options.moveLeftButton) || Game1.options.gamepadControls && (Game1.oldPadState.IsButtonDown(Buttons.DPadLeft) || (double) Game1.input.GetGamePadState().ThumbSticks.Left.X < 0.0)) && who.FacingDirection != 3 && who.FacingDirection != 1)
      {
        zero1.X -= 2f;
        this._hasPlayerAdjustedBobber = true;
      }
      if (!this._hasPlayerAdjustedBobber)
      {
        Vector2 bobberTile = this.calculateBobberTile();
        if (!who.currentLocation.isTileFishable((int) bobberTile.X, (int) bobberTile.Y))
        {
          if (who.FacingDirection == 3 || who.FacingDirection == 1)
          {
            int num = 1;
            if ((double) bobberTile.Y % 1.0 < 0.5)
              num = -1;
            if (who.currentLocation.isTileFishable((int) bobberTile.X, (int) bobberTile.Y + num))
              zero1.Y += (float) num * 4f;
            else if (who.currentLocation.isTileFishable((int) bobberTile.X, (int) bobberTile.Y - num))
              zero1.Y -= (float) num * 4f;
          }
          if (who.FacingDirection == 0 || who.FacingDirection == 2)
          {
            int num = 1;
            if ((double) bobberTile.X % 1.0 < 0.5)
              num = -1;
            if (who.currentLocation.isTileFishable((int) bobberTile.X + num, (int) bobberTile.Y))
              zero1.X += (float) num * 4f;
            else if (who.currentLocation.isTileFishable((int) bobberTile.X - num, (int) bobberTile.Y))
              zero1.X -= (float) num * 4f;
          }
        }
      }
      if (who.IsLocalPlayer)
      {
        this.bobber.Set(this.bobber.Value + zero1);
        this._totalMotion.Set(this._totalMotion.Value + zero1);
      }
      if (this.animations.Count <= 0)
        return;
      Vector2 zero2 = Vector2.Zero;
      Vector2 vector2;
      if (who.IsLocalPlayer)
      {
        vector2 = this._totalMotion.Value;
      }
      else
      {
        this._totalMotionBuffer[this._totalMotionBufferIndex] = this._totalMotion.Value;
        for (int index = 0; index < this._totalMotionBuffer.Length; ++index)
          zero2 += this._totalMotionBuffer[index];
        vector2 = zero2 / (float) this._totalMotionBuffer.Length;
        this._totalMotionBufferIndex = (this._totalMotionBufferIndex + 1) % this._totalMotionBuffer.Length;
      }
      this.animations[0].position -= this._lastAppliedMotion;
      this._lastAppliedMotion = vector2;
      this.animations[0].position += vector2;
    }
    else if (this.showingTreasure)
      who.FarmerSprite.setCurrentSingleFrame(0);
    else if (this.fishCaught)
    {
      if (!Game1.isFestival())
      {
        who.faceDirection(2);
        who.FarmerSprite.setCurrentFrame(84);
      }
      if (Game1.random.NextDouble() < 0.025)
        who.currentLocation.temporarySprites.Add(new TemporaryAnimatedSprite("LooseSprites\\Cursors", new Rectangle(653, 858, 1, 1), 9999f, 1, 1, who.Position + new Vector2((float) (Game1.random.Next(-3, 2) * 4), -32f), false, false, (float) ((double) who.StandingPixel.Y / 10000.0 + 1.0 / 500.0), 0.04f, Color.LightBlue, 5f, 0.0f, 0.0f, 0.0f)
        {
          acceleration = new Vector2(0.0f, 0.25f)
        });
      if (!who.IsLocalPlayer || Game1.input.GetMouseState().LeftButton != ButtonState.Pressed && !Game1.didPlayerJustClickAtAll() && !Game1.isOneOfTheseKeysDown(Game1.oldKBState, Game1.options.useToolButton))
        return;
      this.doneHoldingFish(who);
    }
    else if (who.UsingTool && this.castedButBobberStillInAir && this.doneWithAnimation)
    {
      switch (who.FacingDirection)
      {
        case 0:
          who.FarmerSprite.setCurrentFrame(39);
          break;
        case 1:
          who.FarmerSprite.setCurrentFrame(89);
          break;
        case 2:
          who.FarmerSprite.setCurrentFrame(28);
          break;
        case 3:
          who.FarmerSprite.setCurrentFrame(89, 0, 10, 1, true, false);
          break;
      }
      ref Vector2 local = ref who.armOffset;
      timeSpan = Game1.currentGameTime.TotalGameTime;
      double num = Math.Round(Math.Sin(timeSpan.TotalMilliseconds / 250.0), 2);
      local.Y = (float) num;
    }
    else
    {
      if (this.castedButBobberStillInAir || this.whichFish == null || this.animations.Count <= 0 || (double) this.animations[0].timer <= 500.0 || Game1.eventUp)
        return;
      who.faceDirection(2);
      who.FarmerSprite.setCurrentFrame(57);
    }
  }

  public void doneHoldingFish(Farmer who, bool endOfNight = false)
  {
    if (this.PlayUseSounds)
      who.playNearbySoundLocal("coin");
    if (!this.fromFishPond && Game1.IsSummer && this.whichFish.QualifiedItemId == "(O)138" && Game1.dayOfMonth >= 20 && Game1.dayOfMonth <= 21 && Game1.random.NextDouble() < 0.33 * (double) this.numberOfFishCaught)
      this.gotTroutDerbyTag = true;
    if (!this.treasureCaught && !this.gotTroutDerbyTag)
    {
      this.recastTimerMs = 200;
      Item obj = this.CreateFish();
      bool flag = obj.HasTypeObject();
      if ((obj.Category == -4 || obj.HasContextTag("counts_as_fish_catch")) && !this.fromFishPond)
      {
        int num = (int) Game1.player.stats.Increment("PreciseFishCaught", Math.Max(1, this.numberOfFishCaught));
      }
      if (obj.QualifiedItemId == "(O)79" || obj.QualifiedItemId == "(O)842")
      {
        obj = (Item) who.currentLocation.tryToCreateUnseenSecretNote(who);
        if (obj == null)
          return;
      }
      bool fromFishPond = this.fromFishPond;
      who.completelyStopAnimatingOrDoingAction();
      this.doneFishing(who, !fromFishPond);
      if (((Game1.isFestival() ? 0 : (!fromFishPond ? 1 : 0)) & (flag ? 1 : 0)) != 0 && who.team.specialOrders != null)
      {
        foreach (SpecialOrder specialOrder in who.team.specialOrders)
        {
          Action<Farmer, Item> onFishCaught = specialOrder.onFishCaught;
          if (onFishCaught != null)
            onFishCaught(who, obj);
        }
      }
      if (Game1.isFestival() || who.addItemToInventoryBool(obj))
        return;
      if (endOfNight)
        Game1.createItemDebris(obj, who.getStandingPosition(), -1, who.currentLocation);
      else
        Game1.activeClickableMenu = (IClickableMenu) new ItemGrabMenu((IList<Item>) new List<Item>()
        {
          obj
        }, (object) this).setEssential(true);
    }
    else
    {
      this.fishCaught = false;
      this.showingTreasure = true;
      who.UsingTool = true;
      Item fish = this.CreateFish();
      if ((fish.Category == -4 || fish.HasContextTag("counts_as_fish_catch")) && !this.fromFishPond)
      {
        int num = (int) Game1.player.stats.Increment("PreciseFishCaught", Math.Max(1, this.numberOfFishCaught));
      }
      if (who.team.specialOrders != null)
      {
        foreach (SpecialOrder specialOrder in who.team.specialOrders)
        {
          Action<Farmer, Item> onFishCaught = specialOrder.onFishCaught;
          if (onFishCaught != null)
            onFishCaught(who, fish);
        }
      }
      bool inventoryBool = who.addItemToInventoryBool(fish);
      if (!endOfNight)
      {
        if (this.treasureCaught)
        {
          this.animations.Add(new TemporaryAnimatedSprite(this.goldenTreasure ? "LooseSprites\\Cursors_1_6" : "LooseSprites\\Cursors", this.goldenTreasure ? new Rectangle(256 /*0x0100*/, 75, 32 /*0x20*/, 32 /*0x20*/) : new Rectangle(64 /*0x40*/, 1920, 32 /*0x20*/, 32 /*0x20*/), 500f, 1, 0, who.Position + new Vector2(-32f, -160f), false, false, (float) ((double) who.StandingPixel.Y / 10000.0 + 1.0 / 1000.0), 0.0f, Color.White, 4f, 0.0f, 0.0f, 0.0f)
          {
            motion = new Vector2(0.0f, -0.128f),
            timeBasedMotion = true,
            endFunction = new TemporaryAnimatedSprite.endBehavior(this.openChestEndFunction),
            extraInfoForEndBehavior = inventoryBool ? 0 : fish.Stack,
            alpha = 0.0f,
            alphaFade = -1f / 500f
          });
        }
        else
        {
          if (!this.gotTroutDerbyTag)
            return;
          this.animations.Add(new TemporaryAnimatedSprite("TileSheets\\Objects_2", new Rectangle(80 /*0x50*/, 16 /*0x10*/, 16 /*0x10*/, 16 /*0x10*/), 500f, 1, 0, who.Position + new Vector2(-8f, (float) sbyte.MinValue), false, false, (float) ((double) who.StandingPixel.Y / 10000.0 + 1.0 / 1000.0), 0.0f, Color.White, 4f, 0.0f, 0.0f, 0.0f)
          {
            motion = new Vector2(0.0f, -0.128f),
            timeBasedMotion = true,
            endFunction = new TemporaryAnimatedSprite.endBehavior(this.openChestEndFunction),
            extraInfoForEndBehavior = inventoryBool ? 0 : fish.Stack,
            alpha = 0.0f,
            alphaFade = -1f / 500f,
            id = 1074
          });
        }
      }
      else
      {
        if (inventoryBool)
          return;
        Game1.createItemDebris(fish, who.getStandingPosition(), -1, who.currentLocation);
      }
    }
  }

  /// <summary>Create a fish instance from the raw fields like <see cref="F:StardewValley.Tools.FishingRod.whichFish" />.</summary>
  private Item CreateFish()
  {
    Item itemOrErrorItem = this.whichFish.CreateItemOrErrorItem(quality: this.fishQuality);
    itemOrErrorItem.SetFlagOnPickup = this.setFlagOnCatch;
    if (itemOrErrorItem.HasTypeObject())
    {
      if (itemOrErrorItem.QualifiedItemId == GameLocation.CAROLINES_NECKLACE_ITEM_QID)
      {
        if (itemOrErrorItem is StardewValley.Object @object)
          @object.questItem.Value = true;
      }
      else if (this.numberOfFishCaught > 1 && itemOrErrorItem.QualifiedItemId != "(O)79" && itemOrErrorItem.QualifiedItemId != "(O)842")
        itemOrErrorItem.Stack = this.numberOfFishCaught;
    }
    return itemOrErrorItem;
  }

  private void startCasting() => this.startCastingEvent.Fire();

  public void beginReeling() => this.isReeling = true;

  private void doStartCasting()
  {
    Farmer lastUser = this.lastUser;
    this.randomBobberStyle = -1;
    if (FishingRod.chargeSound != null && lastUser.IsLocalPlayer)
    {
      FishingRod.chargeSound.Stop(AudioStopOptions.Immediate);
      FishingRod.chargeSound = (ICue) null;
    }
    if (lastUser.currentLocation == null)
      return;
    if (lastUser.IsLocalPlayer)
    {
      if (this.PlayUseSounds)
        lastUser.playNearbySoundLocal("button1");
      Rumble.rumble(0.5f, 150f);
    }
    lastUser.UsingTool = true;
    this.isTimingCast = false;
    this.isCasting = true;
    this.castingChosenCountdown = 350f;
    lastUser.armOffset.Y = 0.0f;
    if ((double) this.castingPower <= 0.99000000953674316 || !lastUser.IsLocalPlayer)
      return;
    Game1.screenOverlayTempSprites.Add(new TemporaryAnimatedSprite("LooseSprites\\Cursors", new Rectangle(545, 1921, 53, 19), 800f, 1, 0, Game1.GlobalToLocal(Game1.viewport, lastUser.Position + new Vector2(0.0f, -192f)), false, false, 1f, 0.01f, Color.White, 2f, 0.0f, 0.0f, 0.0f, true)
    {
      motion = new Vector2(0.0f, -4f),
      acceleration = new Vector2(0.0f, 0.2f),
      delayBeforeAnimationStart = 200
    });
    if (!this.PlayUseSounds)
      return;
    DelayedAction.playSoundAfterDelay("crit", 200);
  }

  public void openChestEndFunction(int remainingFish)
  {
    Farmer lastUser = this.lastUser;
    if (this.gotTroutDerbyTag && !this.treasureCaught)
    {
      lastUser.playNearbySoundLocal("discoverMineral");
      this.animations.Add(new TemporaryAnimatedSprite("TileSheets\\Objects_2", new Rectangle(80 /*0x50*/, 16 /*0x10*/, 16 /*0x10*/, 16 /*0x10*/), 800f, 1, 0, lastUser.Position + new Vector2(-8f, -196f), false, false, (float) ((double) lastUser.StandingPixel.Y / 10000.0 + 1.0 / 1000.0), 0.0f, Color.White, 4f, 0.0f, 0.0f, 0.0f)
      {
        endFunction = new TemporaryAnimatedSprite.endBehavior(this.justGotDerbyTagEndFunction),
        extraInfoForEndBehavior = remainingFish,
        shakeIntensity = 0.0f
      });
      this.animations.AddRange((IEnumerable<TemporaryAnimatedSprite>) Utility.getTemporarySpritesWithinArea(new int[2]
      {
        10,
        11
      }, new Rectangle((int) lastUser.Position.X - 16 /*0x10*/, (int) lastUser.Position.Y - 228 + 16 /*0x10*/, 32 /*0x20*/, 32 /*0x20*/), 4, Color.White));
    }
    else
    {
      lastUser.playNearbySoundLocal("openChest");
      this.animations.Add(new TemporaryAnimatedSprite(this.goldenTreasure ? "LooseSprites\\Cursors_1_6" : "LooseSprites\\Cursors", this.goldenTreasure ? new Rectangle(256 /*0x0100*/, 75, 32 /*0x20*/, 32 /*0x20*/) : new Rectangle(64 /*0x40*/, 1920, 32 /*0x20*/, 32 /*0x20*/), 200f, 4, 0, lastUser.Position + new Vector2(-32f, -228f), false, false, (float) ((double) lastUser.StandingPixel.Y / 10000.0 + 1.0 / 1000.0), 0.0f, Color.White, 4f, 0.0f, 0.0f, 0.0f)
      {
        endFunction = new TemporaryAnimatedSprite.endBehavior(this.openTreasureMenuEndFunction),
        extraInfoForEndBehavior = remainingFish
      });
    }
    this.sparklingText = (SparklingText) null;
  }

  public void justGotDerbyTagEndFunction(int remainingFish)
  {
    Farmer lastUser = this.lastUser;
    lastUser.UsingTool = false;
    this.doneFishing(lastUser, true);
    Item obj1 = ItemRegistry.Create("(O)TroutDerbyTag");
    Item obj2 = (Item) null;
    if (remainingFish == 1)
      obj2 = this.CreateFish();
    if (this.PlayUseSounds)
      Game1.playSound("coin");
    this.gotTroutDerbyTag = false;
    if (!lastUser.addItemToInventoryBool(obj1))
    {
      List<Item> inventory = new List<Item>() { obj1 };
      if (obj2 != null)
        inventory.Add(obj2);
      ItemGrabMenu itemGrabMenu = new ItemGrabMenu((IList<Item>) inventory, (object) this).setEssential(true);
      itemGrabMenu.source = 3;
      Game1.activeClickableMenu = (IClickableMenu) itemGrabMenu;
      lastUser.completelyStopAnimatingOrDoingAction();
    }
    else
    {
      if (obj2 == null || lastUser.addItemToInventoryBool(obj2))
        return;
      ItemGrabMenu itemGrabMenu = new ItemGrabMenu((IList<Item>) new List<Item>()
      {
        obj2
      }, (object) this).setEssential(true);
      itemGrabMenu.source = 3;
      Game1.activeClickableMenu = (IClickableMenu) itemGrabMenu;
      lastUser.completelyStopAnimatingOrDoingAction();
    }
  }

  public override bool doesShowTileLocationMarker() => false;

  public void openTreasureMenuEndFunction(int remainingFish)
  {
    Farmer lastUser = this.lastUser;
    lastUser.gainExperience(5, 10 * (this.clearWaterDistance + 1));
    lastUser.UsingTool = false;
    lastUser.completelyStopAnimatingOrDoingAction();
    int num1 = this.treasureCaught ? 1 : 0;
    this.doneFishing(lastUser, true);
    List<Item> inventory = new List<Item>();
    if (remainingFish == 1)
      inventory.Add(this.CreateFish());
    float num2 = 1f;
    if (num1 != 0)
    {
      int num3 = (int) Game1.player.stats.Increment("FishingTreasures", 1);
      while (Game1.random.NextDouble() <= (double) num2)
      {
        num2 *= this.goldenTreasure ? 0.6f : 0.4f;
        if (Game1.IsSpring && !(lastUser.currentLocation is Beach) && Game1.random.NextDouble() < 0.1)
          inventory.Add(ItemRegistry.Create("(O)273", Game1.random.Next(2, 6) + (Game1.random.NextDouble() < 0.25 ? 5 : 0)));
        if (this.numberOfFishCaught > 1 && lastUser.craftingRecipes.ContainsKey("Wild Bait") && Game1.random.NextBool())
          inventory.Add(ItemRegistry.Create("(O)774", 2 + (Game1.random.NextDouble() < 0.25 ? 2 : 0)));
        if (Game1.random.NextDouble() <= 0.33 && lastUser.team.SpecialOrderRuleActive("DROP_QI_BEANS"))
          inventory.Add(ItemRegistry.Create("(O)890", Game1.random.Next(1, 3) + (Game1.random.NextDouble() < 0.25 ? 2 : 0)));
        while (Utility.tryRollMysteryBox(0.08 + Game1.player.team.AverageDailyLuck() / 5.0))
          inventory.Add(ItemRegistry.Create(Game1.player.stats.Get(StatKeys.Mastery(2)) > 0U ? "(O)GoldenMysteryBox" : "(O)MysteryBox"));
        if (Game1.player.stats.Get(StatKeys.Mastery(0)) > 0U && Game1.random.NextDouble() < 0.05)
          inventory.Add(ItemRegistry.Create("(O)GoldenAnimalCracker"));
        if (this.goldenTreasure && Game1.random.NextDouble() < 0.5)
        {
          switch (Game1.random.Next(13))
          {
            case 0:
              inventory.Add(ItemRegistry.Create("(O)337", Game1.random.Next(1, 6)));
              continue;
            case 1:
              inventory.Add(ItemRegistry.Create("(O)SkillBook_" + Game1.random.Next(5).ToString()));
              continue;
            case 2:
              inventory.Add(Utility.getRaccoonSeedForCurrentTimeOfYear(Game1.player, Game1.random, 8));
              continue;
            case 3:
              inventory.Add(ItemRegistry.Create("(O)213"));
              continue;
            case 4:
              inventory.Add(ItemRegistry.Create("(O)872", Game1.random.Next(3, 6)));
              continue;
            case 5:
              inventory.Add(ItemRegistry.Create("(O)687"));
              continue;
            case 6:
              inventory.Add(ItemRegistry.Create("(O)ChallengeBait", Game1.random.Next(3, 6)));
              continue;
            case 7:
              inventory.Add(ItemRegistry.Create("(O)703", Game1.random.Next(3, 6)));
              continue;
            case 8:
              inventory.Add(ItemRegistry.Create("(O)StardropTea"));
              continue;
            case 9:
              inventory.Add(ItemRegistry.Create("(O)797"));
              continue;
            case 10:
              inventory.Add(ItemRegistry.Create("(O)733"));
              continue;
            case 11:
              inventory.Add(ItemRegistry.Create("(O)728"));
              continue;
            case 12:
              inventory.Add(ItemRegistry.Create("(O)SonarBobber"));
              continue;
            default:
              continue;
          }
        }
        else
        {
          switch (Game1.random.Next(4))
          {
            case 0:
              if (this.clearWaterDistance >= 5 && Game1.random.NextDouble() < 0.03)
              {
                inventory.Add((Item) new StardewValley.Object("386", Game1.random.Next(1, 3)));
                continue;
              }
              List<int> options = new List<int>();
              if (this.clearWaterDistance >= 4)
                options.Add(384);
              if (this.clearWaterDistance >= 3 && (options.Count == 0 || Game1.random.NextDouble() < 0.6))
                options.Add(380);
              if (options.Count == 0 || Game1.random.NextDouble() < 0.6)
                options.Add(378);
              if (options.Count == 0 || Game1.random.NextDouble() < 0.6)
                options.Add(388);
              if (options.Count == 0 || Game1.random.NextDouble() < 0.6)
                options.Add(390);
              options.Add(382);
              Item obj1 = ItemRegistry.Create(Game1.random.ChooseFrom<int>((IList<int>) options).ToString(), Game1.random.Next(2, 7) * (Game1.random.NextDouble() < 0.05 + (double) lastUser.luckLevel.Value * 0.015 ? 2 : 1));
              if (Game1.random.NextDouble() < 0.05 + (double) lastUser.LuckLevel * 0.03)
                obj1.Stack *= 2;
              inventory.Add(obj1);
              continue;
            case 1:
              if (this.clearWaterDistance >= 4 && Game1.random.NextDouble() < 0.1 && lastUser.FishingLevel >= 6)
              {
                inventory.Add(ItemRegistry.Create("(O)687"));
                continue;
              }
              if (Game1.random.NextDouble() < 0.25 && lastUser.craftingRecipes.ContainsKey("Wild Bait"))
              {
                inventory.Add(ItemRegistry.Create("(O)774", 5 + (Game1.random.NextDouble() < 0.25 ? 5 : 0)));
                continue;
              }
              if (Game1.random.NextDouble() < 0.11 && lastUser.FishingLevel >= 6)
              {
                inventory.Add(ItemRegistry.Create("(O)SonarBobber"));
                continue;
              }
              if (lastUser.FishingLevel >= 6)
              {
                inventory.Add(ItemRegistry.Create("(O)DeluxeBait", 5));
                continue;
              }
              inventory.Add(ItemRegistry.Create("(O)685", 10));
              continue;
            case 2:
              if (Game1.random.NextDouble() < 0.1 && Game1.netWorldState.Value.LostBooksFound < 21 && lastUser != null && lastUser.hasOrWillReceiveMail("lostBookFound"))
              {
                inventory.Add(ItemRegistry.Create("(O)102"));
                continue;
              }
              if (lastUser.archaeologyFound.Length > 0)
              {
                if (Game1.random.NextDouble() < 0.25 && lastUser.FishingLevel > 1)
                {
                  inventory.Add(ItemRegistry.Create("(O)" + Game1.random.Next(585, 588).ToString()));
                  continue;
                }
                if (Game1.random.NextBool() && lastUser.FishingLevel > 1)
                {
                  inventory.Add(ItemRegistry.Create("(O)" + Game1.random.Next(103, 120).ToString()));
                  continue;
                }
                inventory.Add(ItemRegistry.Create("(O)535"));
                continue;
              }
              inventory.Add(ItemRegistry.Create("(O)382", Game1.random.Next(1, 3)));
              continue;
            case 3:
              int num4;
              switch (Game1.random.Next(3))
              {
                case 0:
                  Item obj2;
                  if (this.clearWaterDistance >= 4)
                  {
                    num4 = 537 + (Game1.random.NextDouble() < 0.4 ? Game1.random.Next(-2, 0) : 0);
                    obj2 = ItemRegistry.Create("(O)" + num4.ToString(), Game1.random.Next(1, 4));
                  }
                  else if (this.clearWaterDistance >= 3)
                  {
                    num4 = 536 + (Game1.random.NextDouble() < 0.4 ? -1 : 0);
                    obj2 = ItemRegistry.Create("(O)" + num4.ToString(), Game1.random.Next(1, 4));
                  }
                  else
                    obj2 = ItemRegistry.Create("(O)535", Game1.random.Next(1, 4));
                  if (Game1.random.NextDouble() < 0.05 + (double) lastUser.LuckLevel * 0.03)
                    obj2.Stack *= 2;
                  inventory.Add(obj2);
                  continue;
                case 1:
                  if (lastUser.FishingLevel < 2)
                  {
                    inventory.Add(ItemRegistry.Create("(O)382", Game1.random.Next(1, 4)));
                    continue;
                  }
                  Item obj3;
                  if (this.clearWaterDistance >= 4)
                  {
                    List<Item> objList = inventory;
                    num4 = Game1.random.NextDouble() < 0.3 ? 82 : Game1.random.Choose<int>(64 /*0x40*/, 60);
                    string str = num4.ToString();
                    Item obj4;
                    obj3 = obj4 = ItemRegistry.Create("(O)" + str, Game1.random.Next(1, 3));
                    objList.Add(obj4);
                  }
                  else if (this.clearWaterDistance >= 3)
                  {
                    List<Item> objList = inventory;
                    num4 = Game1.random.NextDouble() < 0.3 ? 84 : Game1.random.Choose<int>(70, 62);
                    string str = num4.ToString();
                    Item obj5;
                    obj3 = obj5 = ItemRegistry.Create("(O)" + str, Game1.random.Next(1, 3));
                    objList.Add(obj5);
                  }
                  else
                  {
                    List<Item> objList = inventory;
                    num4 = Game1.random.NextDouble() < 0.3 ? 86 : Game1.random.Choose<int>(66, 68);
                    string str = num4.ToString();
                    Item obj6;
                    obj3 = obj6 = ItemRegistry.Create("(O)" + str, Game1.random.Next(1, 3));
                    objList.Add(obj6);
                  }
                  if (Game1.random.NextDouble() < 0.028 * ((double) this.clearWaterDistance / 5.0))
                    inventory.Add(obj3 = ItemRegistry.Create("(O)72"));
                  if (Game1.random.NextDouble() < 0.05)
                  {
                    obj3.Stack *= 2;
                    continue;
                  }
                  continue;
                case 2:
                  if (lastUser.FishingLevel < 2)
                  {
                    inventory.Add((Item) new StardewValley.Object("770", Game1.random.Next(1, 4)));
                    continue;
                  }
                  float num5 = (float) ((1.0 + lastUser.DailyLuck) * ((double) this.clearWaterDistance / 5.0));
                  if (Game1.random.NextDouble() < 0.05 * (double) num5 && !lastUser.specialItems.Contains("14"))
                  {
                    Item obj7 = MeleeWeapon.attemptAddRandomInnateEnchantment(ItemRegistry.Create("(W)14"), Game1.random);
                    obj7.specialItem = true;
                    inventory.Add(obj7);
                  }
                  if (Game1.random.NextDouble() < 0.05 * (double) num5 && !lastUser.specialItems.Contains("51"))
                  {
                    Item obj8 = MeleeWeapon.attemptAddRandomInnateEnchantment(ItemRegistry.Create("(W)51"), Game1.random);
                    obj8.specialItem = true;
                    inventory.Add(obj8);
                  }
                  if (Game1.random.NextDouble() < 0.07 * (double) num5)
                  {
                    num4 = Game1.random.Next(3);
                    int num6;
                    switch (num4)
                    {
                      case 0:
                        List<Item> objList1 = inventory;
                        num6 = 516 + (Game1.random.NextDouble() < (double) lastUser.LuckLevel / 11.0 ? 1 : 0);
                        Ring ring1 = new Ring(num6.ToString());
                        objList1.Add((Item) ring1);
                        break;
                      case 1:
                        List<Item> objList2 = inventory;
                        num6 = 518 + (Game1.random.NextDouble() < (double) lastUser.LuckLevel / 11.0 ? 1 : 0);
                        Ring ring2 = new Ring(num6.ToString());
                        objList2.Add((Item) ring2);
                        break;
                      case 2:
                        List<Item> objList3 = inventory;
                        num6 = Game1.random.Next(529, 535);
                        Ring ring3 = new Ring(num6.ToString());
                        objList3.Add((Item) ring3);
                        break;
                    }
                  }
                  if (Game1.random.NextDouble() < 0.02 * (double) num5)
                    inventory.Add(ItemRegistry.Create("(O)166"));
                  if (lastUser.FishingLevel > 5 && Game1.random.NextDouble() < 0.001 * (double) num5)
                    inventory.Add(ItemRegistry.Create("(O)74"));
                  if (Game1.random.NextDouble() < 0.01 * (double) num5)
                    inventory.Add(ItemRegistry.Create("(O)127"));
                  if (Game1.random.NextDouble() < 0.01 * (double) num5)
                    inventory.Add(ItemRegistry.Create("(O)126"));
                  if (Game1.random.NextDouble() < 0.01 * (double) num5)
                    inventory.Add((Item) new Ring("527"));
                  if (Game1.random.NextDouble() < 0.01 * (double) num5)
                  {
                    List<Item> objList = inventory;
                    num4 = Game1.random.Next(504, 514);
                    Item obj9 = ItemRegistry.Create("(B)" + num4.ToString());
                    objList.Add(obj9);
                  }
                  if (Game1.MasterPlayer.mailReceived.Contains("Farm_Eternal") && Game1.random.NextDouble() < 0.01 * (double) num5)
                    inventory.Add(ItemRegistry.Create("(O)928"));
                  if (inventory.Count == 1)
                    inventory.Add(ItemRegistry.Create("(O)72"));
                  if (Game1.player.stats.Get("FishingTreasures") > 3U)
                  {
                    Random random = Utility.CreateRandom((double) (Game1.player.stats.Get("FishingTreasures") * 27973U), (double) Game1.uniqueIDForThisGame);
                    if (random.NextDouble() < 0.05 * (double) num5)
                    {
                      List<Item> objList = inventory;
                      num4 = random.Next(5);
                      Item obj10 = ItemRegistry.Create("(O)SkillBook_" + num4.ToString());
                      objList.Add(obj10);
                      num2 = 0.0f;
                      continue;
                    }
                    continue;
                  }
                  continue;
                default:
                  continue;
              }
            default:
              continue;
          }
        }
      }
      if (inventory.Count == 0)
        inventory.Add(ItemRegistry.Create("(O)685", Game1.random.Next(1, 4) * 5));
      if (this.lastUser.hasQuest("98765") && Utility.GetDayOfPassiveFestival("DesertFestival") == 3 && !this.lastUser.Items.ContainsId("GoldenBobber", 1))
      {
        inventory.Clear();
        inventory.Add(ItemRegistry.Create("(O)GoldenBobber"));
      }
      if (Game1.random.NextDouble() < 0.25 && this.lastUser.stats.Get("Book_Roe") > 0U)
      {
        Item fish = this.CreateFish();
        ObjectDataDefinition objectTypeDefinition = ItemRegistry.GetObjectTypeDefinition();
        if (objectTypeDefinition.CanHaveRoe(fish))
        {
          StardewValley.Object flavoredRoe = (StardewValley.Object) objectTypeDefinition.CreateFlavoredRoe(fish as StardewValley.Object);
          flavoredRoe.Stack = Game1.random.Next(1, 3);
          if (Game1.random.NextDouble() < 0.1 + this.lastUser.team.AverageDailyLuck())
            ++flavoredRoe.Stack;
          if (Game1.random.NextDouble() < 0.1 + this.lastUser.team.AverageDailyLuck())
            flavoredRoe.Stack *= 2;
          inventory.Add((Item) flavoredRoe);
        }
      }
      if (Game1.player.fishingLevel.Value > 4 && Game1.player.stats.Get("FishingTreasures") > 2U && Game1.random.NextDouble() < 0.02 + (!Game1.player.mailReceived.Contains("roeBookDropped") ? (double) Game1.player.stats.Get("FishingTreasures") * 0.001 : 0.001))
      {
        inventory.Add(ItemRegistry.Create("(O)Book_Roe"));
        Game1.player.mailReceived.Add("roeBookDropped");
      }
    }
    if (this.gotTroutDerbyTag)
    {
      inventory.Add(ItemRegistry.Create("(O)TroutDerbyTag"));
      this.gotTroutDerbyTag = false;
    }
    ItemGrabMenu itemGrabMenu = new ItemGrabMenu((IList<Item>) inventory, (object) this).setEssential(true);
    itemGrabMenu.source = 3;
    Game1.activeClickableMenu = (IClickableMenu) itemGrabMenu;
    lastUser.completelyStopAnimatingOrDoingAction();
  }
}
