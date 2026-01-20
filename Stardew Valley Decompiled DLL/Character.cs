// Decompiled with JetBrains decompiler
// Type: StardewValley.Character
// Assembly: Stardew Valley, Version=1.6.15.24356, Culture=neutral, PublicKeyToken=null
// MVID: AA1F513A-94F0-4EF6-A35F-2D5B4A3721BF
// Assembly location: D:\SteamLibrary\steamapps\common\Stardew Valley\Stardew Valley.dll
// XML documentation location: D:\SteamLibrary\steamapps\common\Stardew Valley\Stardew Valley.xml

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Netcode;
using Netcode.Validation;
using StardewValley.Audio;
using StardewValley.Extensions;
using StardewValley.GameData.Characters;
using StardewValley.Mods;
using StardewValley.Network;
using StardewValley.Pathfinding;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml.Serialization;
using xTile.Dimensions;

#nullable disable
namespace StardewValley;

[InstanceStatics]
[XmlInclude(typeof (FarmAnimal))]
[XmlInclude(typeof (Farmer))]
[XmlInclude(typeof (NPC))]
[NotImplicitNetField]
public class Character : INetObject<NetFields>, IHaveModData
{
  public const float emoteBeginInterval = 20f;
  public const float emoteNormalInterval = 250f;
  public const int emptyCanEmote = 4;
  public const int questionMarkEmote = 8;
  public const int angryEmote = 12;
  public const int exclamationEmote = 16 /*0x10*/;
  public const int heartEmote = 20;
  public const int sleepEmote = 24;
  public const int sadEmote = 28;
  public const int happyEmote = 32 /*0x20*/;
  public const int xEmote = 36;
  public const int pauseEmote = 40;
  public const int videoGameEmote = 52;
  public const int musicNoteEmote = 56;
  public const int blushEmote = 60;
  public const int blockedIntervalBeforeEmote = 3000;
  public const int blockedIntervalBeforeSprint = 5000;
  public const double chanceForSound = 0.001;
  /// <summary>A position value that's invalid, used to force cached position info to update.</summary>
  private static Vector2 ClearPositionValue = new Vector2((float) int.MinValue);
  /// <summary>The backing field for <see cref="P:StardewValley.Character.StandingPixel" />.</summary>
  private Point cachedStandingPixel;
  /// <summary>The backing field for <see cref="P:StardewValley.Character.Tile" />.</summary>
  private Vector2 cachedTile;
  /// <summary>The backing field for <see cref="P:StardewValley.Character.TilePoint" />.</summary>
  private Point cachedTilePoint;
  /// <summary>The position value for which <see cref="F:StardewValley.Character.cachedStandingPixel" /> was calculated.</summary>
  private Vector2 pixelPositionForCachedStandingPixel;
  /// <summary>The position value for which <see cref="F:StardewValley.Character.cachedTile" /> was calculated.</summary>
  private Vector2 pixelPositionForCachedTile;
  /// <summary>The position value for which <see cref="F:StardewValley.Character.cachedTilePoint" /> was calculated.</summary>
  private Vector2 pixelPositionForCachedTilePoint;
  /// <summary>Whether to hide this character from the animal social menu, if it would normally be shown.</summary>
  [XmlIgnore]
  public readonly NetBool hideFromAnimalSocialMenu = new NetBool();
  [XmlIgnore]
  public readonly NetRef<AnimatedSprite> sprite = new NetRef<AnimatedSprite>();
  /// <summary>The backing field for <see cref="P:StardewValley.Character.Position" />.</summary>
  [XmlIgnore]
  public readonly NetPosition position = new NetPosition();
  [XmlIgnore]
  private readonly NetInt netSpeed = new NetInt();
  [XmlIgnore]
  private readonly NetFloat netAddedSpeed = new NetFloat();
  [XmlIgnore]
  public readonly NetDirection facingDirection = new NetDirection(2);
  [XmlIgnore]
  public int blockedInterval;
  [XmlIgnore]
  public int faceTowardFarmerTimer;
  [XmlIgnore]
  public int forceUpdateTimer;
  [XmlIgnore]
  public int movementPause;
  [XmlIgnore]
  public NetEvent1Field<int, NetInt> faceTowardFarmerEvent = new NetEvent1Field<int, NetInt>();
  [XmlIgnore]
  public readonly NetInt faceTowardFarmerRadius = new NetInt();
  [XmlIgnore]
  public readonly NetBool simpleNonVillagerNPC = new NetBool();
  [XmlIgnore]
  public int nonVillagerNPCTimesTalked;
  [XmlElement("name")]
  public readonly NetString name = new NetString();
  [XmlElement("forceOneTileWide")]
  public readonly NetBool forceOneTileWide = new NetBool(false);
  protected bool moveUp;
  protected bool moveRight;
  protected bool moveDown;
  protected bool moveLeft;
  protected bool freezeMotion;
  [XmlIgnore]
  private string _displayName;
  public bool isEmoting;
  public bool isCharging;
  public bool isGlowing;
  public bool coloredBorder;
  public bool flip;
  public bool drawOnTop;
  public bool faceTowardFarmer;
  public bool ignoreMovementAnimation;
  [XmlIgnore]
  public bool hasJustStartedFacingPlayer;
  [XmlElement("faceAwayFromFarmer")]
  public readonly NetBool faceAwayFromFarmer = new NetBool();
  protected int currentEmote;
  protected int currentEmoteFrame;
  protected readonly NetInt facingDirectionBeforeSpeakingToPlayer = new NetInt(-1);
  [XmlIgnore]
  public float emoteInterval;
  [XmlIgnore]
  public float xVelocity;
  [XmlIgnore]
  public float yVelocity;
  [XmlIgnore]
  public Vector2 lastClick = Vector2.Zero;
  public readonly NetFloat scale = new NetFloat(1f);
  public float glowingTransparency;
  public float glowRate;
  private bool glowUp;
  [XmlIgnore]
  public readonly NetBool swimming = new NetBool();
  [XmlIgnore]
  public bool nextEventcommandAfterEmote;
  [XmlIgnore]
  public bool farmerPassesThrough;
  [XmlIgnore]
  public NetBool netEventActor = new NetBool();
  [XmlIgnore]
  public readonly NetBool collidesWithOtherCharacters = new NetBool();
  protected bool ignoreMovementAnimations;
  [XmlIgnore]
  public int yJumpOffset;
  [XmlIgnore]
  public int ySourceRectOffset;
  [XmlIgnore]
  public float yJumpVelocity;
  [XmlIgnore]
  public float yJumpGravity = -0.5f;
  [XmlIgnore]
  public bool wasJumpWithSound;
  [XmlIgnore]
  private readonly NetFarmerRef whoToFace = new NetFarmerRef();
  [XmlIgnore]
  public Color glowingColor;
  [XmlIgnore]
  public PathFindController controller;
  private bool emoteFading;
  [XmlIgnore]
  private readonly NetBool _willDestroyObjectsUnderfoot = new NetBool(true);
  [XmlIgnore]
  protected readonly NetLocationRef currentLocationRef = new NetLocationRef();
  private Microsoft.Xna.Framework.Rectangle originalSourceRect;
  protected int emoteYOffset;
  public static readonly Vector2[] AdjacentTilesOffsets = new Vector2[4]
  {
    new Vector2(1f, 0.0f),
    new Vector2(-1f, 0.0f),
    new Vector2(0.0f, -1f),
    new Vector2(0.0f, 1f)
  };
  [XmlIgnore]
  public Vector2 drawOffset = Vector2.Zero;
  [XmlIgnore]
  public bool shouldShadowBeOffset;

  /// <summary>The character's gender identity.</summary>
  public virtual Gender Gender { get; set; } = Gender.Undefined;

  [XmlIgnore]
  public int speed
  {
    get => this.netSpeed.Value;
    set => this.netSpeed.Value = value;
  }

  [XmlIgnore]
  public virtual float addedSpeed
  {
    get => this.netAddedSpeed.Value;
    set => this.netAddedSpeed.Value = value;
  }

  [XmlIgnore]
  public virtual string displayName
  {
    get => this._displayName ?? (this._displayName = this.translateName());
    set => this._displayName = value;
  }

  [XmlIgnore]
  public virtual bool EventActor
  {
    get => this.netEventActor.Value;
    set => this.netEventActor.Value = value;
  }

  public bool willDestroyObjectsUnderfoot
  {
    get => this._willDestroyObjectsUnderfoot.Value;
    set => this._willDestroyObjectsUnderfoot.Value = value;
  }

  /// <summary>The character's pixel coordinates within their current location, ignoring their bounding box, relative to the top-left corner of the map.</summary>
  /// <remarks>See also <see cref="P:StardewValley.Character.StandingPixel" /> for the pixel coordinates at the center of their bounding box, and <see cref="P:StardewValley.Character.Tile" /> and <see cref="P:StardewValley.Character.TilePoint" /> for the tile coordinates.</remarks>
  public Vector2 Position
  {
    get => this.position.Value;
    set
    {
      if (!(this.position.Value != value))
        return;
      this.position.Set(value);
    }
  }

  /// <summary>The pixel coordinates at the center of this character's bounding box, relative to the top-left corner of the map.</summary>
  /// <remarks>See also <see cref="M:StardewValley.Character.getStandingPosition" /> for a vector version, <see cref="P:StardewValley.Character.Tile" /> and <see cref="P:StardewValley.Character.TilePoint" /> for the tile coordinates, or <see cref="P:StardewValley.Character.Position" /> for the raw pixel position.</remarks>
  public Point StandingPixel
  {
    get
    {
      if ((double) this.position.X != (double) this.pixelPositionForCachedStandingPixel.X || (double) this.position.Y != (double) this.pixelPositionForCachedStandingPixel.Y)
      {
        this.cachedStandingPixel = this.GetBoundingBox().Center;
        this.pixelPositionForCachedStandingPixel = this.position.Value;
      }
      return this.cachedStandingPixel;
    }
  }

  /// <summary>The character's tile position within their current location.</summary>
  /// <remarks>See also <see cref="P:StardewValley.Character.TilePoint" /> for a point version, <see cref="P:StardewValley.Character.StandingPixel" /> the pixel coordinates at the center of their bounding box, or <see cref="P:StardewValley.Character.Position" /> for the raw pixel position.</remarks>
  public Vector2 Tile
  {
    get
    {
      if ((double) this.position.X != (double) this.pixelPositionForCachedTile.X || (double) this.position.Y != (double) this.pixelPositionForCachedTile.Y)
      {
        Point standingPixel = this.StandingPixel;
        this.cachedTile = new Vector2((float) (standingPixel.X / 64 /*0x40*/), (float) (standingPixel.Y / 64 /*0x40*/));
        this.pixelPositionForCachedTile = this.position.Value;
      }
      return this.cachedTile;
    }
  }

  /// <summary>The character's tile position within their current location as a <see cref="T:Microsoft.Xna.Framework.Point" />.</summary>
  /// <remarks>See also <see cref="P:StardewValley.Character.Tile" /> for a vector version, <see cref="P:StardewValley.Character.StandingPixel" /> the pixel coordinates at the center of their bounding box, or <see cref="P:StardewValley.Character.Position" /> for the raw pixel position.</remarks>
  public Point TilePoint
  {
    get
    {
      if ((double) this.position.X != (double) this.pixelPositionForCachedTilePoint.X || (double) this.position.Y != (double) this.pixelPositionForCachedTilePoint.Y)
      {
        Vector2 tile = this.Tile;
        this.cachedTilePoint = new Point((int) tile.X, (int) tile.Y);
        this.pixelPositionForCachedTilePoint = this.position.Value;
      }
      return this.cachedTilePoint;
    }
  }

  public int Speed
  {
    get => this.speed;
    set => this.speed = value;
  }

  public virtual int FacingDirection
  {
    get => this.facingDirection.Value;
    set => this.facingDirection.Set(value);
  }

  [XmlIgnore]
  public string Name
  {
    get => this.name.Value;
    set => this.name.Set(value);
  }

  [XmlIgnore]
  public bool SimpleNonVillagerNPC
  {
    get => this.simpleNonVillagerNPC.Value;
    set => this.simpleNonVillagerNPC.Set(value);
  }

  [XmlIgnore]
  public virtual AnimatedSprite Sprite
  {
    get => this.sprite.Value;
    set => this.sprite.Value = value;
  }

  public bool IsEmoting
  {
    get => this.isEmoting;
    set => this.isEmoting = value;
  }

  public int CurrentEmote
  {
    get => this.currentEmote;
    set => this.currentEmote = value;
  }

  public int CurrentEmoteIndex => this.currentEmoteFrame;

  /// <summary>Whether this is a monster NPC type, regardless of whether they're present in <c>Data/Monsters</c>.</summary>
  public virtual bool IsMonster => false;

  /// <summary>Whether this is a villager NPC type, regardless of whether they're present in <c>Data/Characters</c>.</summary>
  [XmlIgnore]
  public virtual bool IsVillager => false;

  public float Scale
  {
    get => this.scale.Value;
    set => this.scale.Value = value;
  }

  [XmlIgnore]
  public GameLocation currentLocation
  {
    get => this.currentLocationRef.Value;
    set => this.currentLocationRef.Value = value;
  }

  /// <inheritdoc />
  [XmlIgnore]
  public ModDataDictionary modData { get; } = new ModDataDictionary();

  /// <inheritdoc />
  [XmlElement("modData")]
  public ModDataDictionary modDataForSerialization
  {
    get => this.modData.GetForSerialization();
    set => this.modData.SetFromSerialization(value);
  }

  public NetFields NetFields { get; }

  public Character()
  {
    this.NetFields = new NetFields(NetFields.GetNameForInstance<Character>(this));
    this.initNetFields();
  }

  protected virtual void initNetFields()
  {
    this.NetFields.SetOwner((INetObject<NetFields>) this).AddField((INetSerializable) this.sprite, "sprite").AddField((INetSerializable) this.position.NetFields, "position.NetFields").AddField((INetSerializable) this.facingDirection, "facingDirection").AddField((INetSerializable) this.netSpeed, "netSpeed").AddField((INetSerializable) this.netAddedSpeed, "netAddedSpeed").AddField((INetSerializable) this.name, "name").AddField((INetSerializable) this.scale, "scale").AddField((INetSerializable) this.currentLocationRef.NetFields, "currentLocationRef.NetFields").AddField((INetSerializable) this.swimming, "swimming").AddField((INetSerializable) this.collidesWithOtherCharacters, "collidesWithOtherCharacters").AddField((INetSerializable) this.facingDirectionBeforeSpeakingToPlayer, "facingDirectionBeforeSpeakingToPlayer").AddField((INetSerializable) this.faceTowardFarmerRadius, "faceTowardFarmerRadius").AddField((INetSerializable) this.faceAwayFromFarmer, "faceAwayFromFarmer").AddField((INetSerializable) this.whoToFace.NetFields, "whoToFace.NetFields").AddField((INetSerializable) this.faceTowardFarmerEvent, "faceTowardFarmerEvent").AddField((INetSerializable) this._willDestroyObjectsUnderfoot, "_willDestroyObjectsUnderfoot").AddField((INetSerializable) this.forceOneTileWide, "forceOneTileWide").AddField((INetSerializable) this.simpleNonVillagerNPC, "simpleNonVillagerNPC").AddField((INetSerializable) this.hideFromAnimalSocialMenu, "hideFromAnimalSocialMenu").AddField((INetSerializable) this.netEventActor, "netEventActor").AddField((INetSerializable) this.modData, "modData");
    this.facingDirection.Position = this.position;
    this.faceTowardFarmerEvent.onEvent += new AbstractNetEvent1<int>.Event(this.performFaceTowardFarmerEvent);
    this.sprite.fieldChangeEvent += (FieldChange<NetRef<AnimatedSprite>, AnimatedSprite>) ((field, value, newValue) =>
    {
      newValue?.SetOwner(this);
      this.ClearCachedPosition();
    });
  }

  public Character(AnimatedSprite sprite, Vector2 position, int speed, string name)
    : this()
  {
    this.Sprite = sprite;
    this.Position = position;
    this.speed = speed;
    this.Name = name;
    if (sprite == null)
      return;
    this.originalSourceRect = sprite.SourceRect;
  }

  protected virtual string translateName() => this.name.Value;

  /// <summary>Forget the cached bounding box values so they're recalculated on the next request.</summary>
  internal void ClearCachedPosition()
  {
    this.pixelPositionForCachedStandingPixel = Character.ClearPositionValue;
    this.pixelPositionForCachedTile = Character.ClearPositionValue;
    this.pixelPositionForCachedTilePoint = Character.ClearPositionValue;
  }

  /// <summary>Reset the cached display name, so <see cref="M:StardewValley.Character.translateName" /> is called again next time it's requested.</summary>
  protected void resetCachedDisplayName() => this._displayName = (string) null;

  public virtual void SetMovingUp(bool b)
  {
    this.moveUp = b;
    if (b)
      return;
    this.Halt();
  }

  public virtual void SetMovingRight(bool b)
  {
    this.moveRight = b;
    if (b)
      return;
    this.Halt();
  }

  public virtual void SetMovingDown(bool b)
  {
    this.moveDown = b;
    if (b)
      return;
    this.Halt();
  }

  public virtual void SetMovingLeft(bool b)
  {
    this.moveLeft = b;
    if (b)
      return;
    this.Halt();
  }

  public void setMovingInFacingDirection()
  {
    switch (this.FacingDirection)
    {
      case 0:
        this.SetMovingUp(true);
        break;
      case 1:
        this.SetMovingRight(true);
        break;
      case 2:
        this.SetMovingDown(true);
        break;
      case 3:
        this.SetMovingLeft(true);
        break;
    }
  }

  public int getFacingDirection()
  {
    if (this.Sprite.currentFrame < 4)
      return 2;
    if (this.Sprite.currentFrame < 8)
      return 1;
    return this.Sprite.currentFrame < 12 ? 0 : 3;
  }

  public void setTrajectory(int xVelocity, int yVelocity)
  {
    this.setTrajectory(new Vector2((float) xVelocity, (float) yVelocity));
  }

  public virtual void setTrajectory(Vector2 trajectory)
  {
    this.xVelocity = trajectory.X;
    this.yVelocity = trajectory.Y;
  }

  public virtual void Halt()
  {
    this.moveUp = false;
    this.moveDown = false;
    this.moveRight = false;
    this.moveLeft = false;
    this.Sprite.StopAnimation();
  }

  public void extendSourceRect(int horizontal, int vertical, bool ignoreSourceRectUpdates = true)
  {
    this.Sprite.sourceRect.Inflate(Math.Abs(horizontal) / 2, Math.Abs(vertical) / 2);
    this.Sprite.sourceRect.Offset(horizontal / 2, vertical / 2);
    Microsoft.Xna.Framework.Rectangle originalSourceRect = this.originalSourceRect;
    if (this.Sprite.SourceRect.Equals(this.originalSourceRect))
      this.Sprite.ignoreSourceRectUpdates = false;
    else
      this.Sprite.ignoreSourceRectUpdates = ignoreSourceRectUpdates;
  }

  public virtual bool collideWith(Object o) => true;

  public virtual void faceDirection(int direction)
  {
    if (this.SimpleNonVillagerNPC)
      return;
    if (direction != -3)
    {
      this.FacingDirection = direction;
      this.Sprite?.faceDirection(direction);
      this.faceTowardFarmer = false;
    }
    else
      this.faceTowardFarmer = true;
  }

  public int getDirection()
  {
    if (this.moveUp)
      return 0;
    if (this.moveRight)
      return 1;
    if (this.moveDown)
      return 2;
    if (this.moveLeft)
      return 3;
    return this.IsRemoteMoving() ? this.FacingDirection : -1;
  }

  public bool IsRemoteMoving()
  {
    if (!LocalMultiplayer.IsLocalMultiplayer(true))
      return this.position.Field.IsInterpolating();
    return this.position.moving.Value || this.position.Field.IsInterpolating();
  }

  public void tryToMoveInDirection(int direction, bool isFarmer, int damagesFarmer, bool glider)
  {
    if (this.currentLocation.isCollidingPosition(this.nextPosition(direction), Game1.viewport, isFarmer, damagesFarmer, glider, this))
      return;
    switch (direction)
    {
      case 0:
        this.position.Y -= (float) this.speed + this.addedSpeed;
        break;
      case 1:
        this.position.X += (float) this.speed + this.addedSpeed;
        break;
      case 2:
        this.position.Y += (float) this.speed + this.addedSpeed;
        break;
      case 3:
        this.position.X -= (float) this.speed + this.addedSpeed;
        break;
    }
  }

  public virtual Vector2 GetShadowOffset()
  {
    return this.shouldShadowBeOffset ? this.drawOffset : Vector2.Zero;
  }

  public virtual bool shouldCollideWithBuildingLayer(GameLocation location)
  {
    return this.controller == null && !this.IsMonster;
  }

  protected void applyVelocity(GameLocation currentLocation)
  {
    Microsoft.Xna.Framework.Rectangle boundingBox = this.GetBoundingBox();
    boundingBox.X += (int) this.xVelocity;
    boundingBox.Y -= (int) this.yVelocity;
    if (currentLocation == null || !currentLocation.isCollidingPosition(boundingBox, Game1.viewport, false, 0, false, this))
    {
      this.position.X += this.xVelocity;
      this.position.Y -= this.yVelocity;
    }
    this.xVelocity = (float) (int) ((double) this.xVelocity - (double) this.xVelocity / 2.0);
    this.yVelocity = (float) (int) ((double) this.yVelocity - (double) this.yVelocity / 2.0);
  }

  public virtual void MovePosition(GameTime time, xTile.Dimensions.Rectangle viewport, GameLocation currentLocation)
  {
    if (this is FarmAnimal)
      this.willDestroyObjectsUnderfoot = false;
    int num;
    if (this.willDestroyObjectsUnderfoot)
    {
      PathFindController controller = this.controller;
      num = controller != null ? (!controller.nonDestructivePathing ? 1 : 0) : 1;
    }
    else
      num = 0;
    bool flag = num != 0;
    if ((double) this.xVelocity != 0.0 || (double) this.yVelocity != 0.0)
      this.applyVelocity(currentLocation);
    else if (this.moveUp)
    {
      if (currentLocation == null || !currentLocation.isCollidingPosition(this.nextPosition(0), viewport, false, 0, false, this) || this.isCharging)
      {
        this.position.Y -= (float) this.speed + this.addedSpeed;
        if (!this.ignoreMovementAnimation)
        {
          this.Sprite.AnimateUp(time, (this.speed - 2 + (int) this.addedSpeed) * -25, Utility.isOnScreen(this.TilePoint, 1, currentLocation) ? "Cowboy_Footstep" : "");
          this.faceDirection(0);
        }
      }
      else if (!currentLocation.isTilePassable(this.nextPosition(0), viewport) || !flag)
        this.Halt();
      else if (flag)
      {
        if (currentLocation.characterDestroyObjectWithinRectangle(this.nextPosition(0), true))
        {
          this.doEmote(12);
          this.position.Y -= (float) this.speed + this.addedSpeed;
        }
        else
          this.blockedInterval += time.ElapsedGameTime.Milliseconds;
      }
    }
    else if (this.moveRight)
    {
      if (currentLocation == null || !currentLocation.isCollidingPosition(this.nextPosition(1), viewport, false, 0, false, this) || this.isCharging)
      {
        this.position.X += (float) this.speed + this.addedSpeed;
        if (!this.ignoreMovementAnimation)
        {
          this.Sprite.AnimateRight(time, (this.speed - 2 + (int) this.addedSpeed) * -25, Utility.isOnScreen(this.TilePoint, 1, currentLocation) ? "Cowboy_Footstep" : "");
          this.faceDirection(1);
        }
      }
      else if (!currentLocation.isTilePassable(this.nextPosition(1), viewport) || !flag)
        this.Halt();
      else if (flag)
      {
        if (currentLocation.characterDestroyObjectWithinRectangle(this.nextPosition(1), true))
        {
          this.doEmote(12);
          this.position.X += (float) this.speed + this.addedSpeed;
        }
        else
          this.blockedInterval += time.ElapsedGameTime.Milliseconds;
      }
    }
    else if (this.moveDown)
    {
      if (currentLocation == null || !currentLocation.isCollidingPosition(this.nextPosition(2), viewport, false, 0, false, this) || this.isCharging)
      {
        this.position.Y += (float) this.speed + this.addedSpeed;
        if (!this.ignoreMovementAnimation)
        {
          this.Sprite.AnimateDown(time, (this.speed - 2 + (int) this.addedSpeed) * -25, Utility.isOnScreen(this.TilePoint, 1, currentLocation) ? "Cowboy_Footstep" : "");
          this.faceDirection(2);
        }
      }
      else if (!currentLocation.isTilePassable(this.nextPosition(2), viewport) || !flag)
        this.Halt();
      else if (flag)
      {
        if (currentLocation.characterDestroyObjectWithinRectangle(this.nextPosition(2), true))
        {
          this.doEmote(12);
          this.position.Y += (float) this.speed + this.addedSpeed;
        }
        else
          this.blockedInterval += time.ElapsedGameTime.Milliseconds;
      }
    }
    else if (this.moveLeft)
    {
      if (currentLocation == null || !currentLocation.isCollidingPosition(this.nextPosition(3), viewport, false, 0, false, this) || this.isCharging)
      {
        this.position.X -= (float) this.speed + this.addedSpeed;
        if (!this.ignoreMovementAnimation)
        {
          this.Sprite.AnimateLeft(time, (this.speed - 2 + (int) this.addedSpeed) * -25, Utility.isOnScreen(this.TilePoint, 1, currentLocation) ? "Cowboy_Footstep" : "");
          this.faceDirection(3);
        }
      }
      else if (!currentLocation.isTilePassable(this.nextPosition(3), viewport) || !flag)
        this.Halt();
      else if (flag)
      {
        if (currentLocation.characterDestroyObjectWithinRectangle(this.nextPosition(3), true))
        {
          this.doEmote(12);
          this.position.X -= (float) this.speed + this.addedSpeed;
        }
        else
          this.blockedInterval += time.ElapsedGameTime.Milliseconds;
      }
    }
    else
      this.Sprite.animateOnce(time);
    if (flag && currentLocation != null && this.isMoving())
      currentLocation.characterTrampleTile(this.Tile);
    if (this.blockedInterval >= 3000 && (double) this.blockedInterval <= 3750.0 && !Game1.eventUp)
    {
      this.doEmote(Game1.random.Choose<int>(8, 40));
      this.blockedInterval = 3750;
    }
    else
    {
      if (this.blockedInterval < 5000)
        return;
      this.speed = 4;
      this.isCharging = true;
      this.blockedInterval = 0;
    }
  }

  public virtual bool canPassThroughActionTiles() => false;

  public virtual Microsoft.Xna.Framework.Rectangle nextPosition(int direction)
  {
    Microsoft.Xna.Framework.Rectangle boundingBox = this.GetBoundingBox();
    switch (direction)
    {
      case 0:
        boundingBox.Y -= this.speed + (int) this.addedSpeed;
        break;
      case 1:
        boundingBox.X += this.speed + (int) this.addedSpeed;
        break;
      case 2:
        boundingBox.Y += this.speed + (int) this.addedSpeed;
        break;
      case 3:
        boundingBox.X -= this.speed + (int) this.addedSpeed;
        break;
    }
    return boundingBox;
  }

  public Location nextPositionPoint()
  {
    Location location = new Location();
    Point standingPixel = this.StandingPixel;
    switch (this.getDirection())
    {
      case 0:
        location = new Location(standingPixel.X, standingPixel.Y - 64 /*0x40*/);
        break;
      case 1:
        location = new Location(standingPixel.X + 64 /*0x40*/, standingPixel.Y);
        break;
      case 2:
        location = new Location(standingPixel.X, standingPixel.Y + 64 /*0x40*/);
        break;
      case 3:
        location = new Location(standingPixel.X - 64 /*0x40*/, standingPixel.Y);
        break;
    }
    return location;
  }

  public int getHorizontalMovement()
  {
    if (this.moveRight)
      return this.speed + (int) this.addedSpeed;
    return !this.moveLeft ? 0 : -this.speed - (int) this.addedSpeed;
  }

  public int getVerticalMovement()
  {
    if (this.moveDown)
      return this.speed + (int) this.addedSpeed;
    return !this.moveUp ? 0 : -this.speed - (int) this.addedSpeed;
  }

  public Vector2 nextPositionVector2()
  {
    Point standingPixel = this.StandingPixel;
    return new Vector2((float) (standingPixel.X + this.getHorizontalMovement()), (float) (standingPixel.Y + this.getVerticalMovement()));
  }

  public Location nextPositionTile()
  {
    Location location = this.nextPositionPoint();
    location.X /= 64 /*0x40*/;
    location.Y /= 64 /*0x40*/;
    return location;
  }

  public virtual void doEmote(int whichEmote, bool playSound, bool nextEventCommand = true)
  {
    if (this.isEmoting || Game1.eventUp && !(this is Farmer) && (Game1.currentLocation.currentEvent == null || !((IEnumerable<Character>) Game1.currentLocation.currentEvent.actors).Contains<Character>(this)))
      return;
    this.emoteYOffset = 0;
    this.isEmoting = true;
    this.currentEmote = whichEmote;
    this.currentEmoteFrame = 0;
    this.emoteInterval = 0.0f;
    this.nextEventcommandAfterEmote = nextEventCommand;
  }

  public void doEmote(int whichEmote, bool nextEventCommand = true)
  {
    this.doEmote(whichEmote, true, nextEventCommand);
  }

  public void doEmote(int whichEmote, int emoteYOffset)
  {
    this.doEmote(whichEmote, true, false);
    this.emoteYOffset = emoteYOffset;
  }

  public void updateEmote(GameTime time)
  {
    if (!this.isEmoting)
      return;
    this.emoteInterval += (float) time.ElapsedGameTime.Milliseconds;
    if (this.emoteFading && (double) this.emoteInterval > 20.0)
    {
      this.emoteInterval = 0.0f;
      --this.currentEmoteFrame;
      if (this.currentEmoteFrame >= 0)
        return;
      this.emoteFading = false;
      this.isEmoting = false;
      if (!this.nextEventcommandAfterEmote || Game1.currentLocation.currentEvent == null || !((IEnumerable<Character>) Game1.currentLocation.currentEvent.actors).Contains<Character>(this) && !((IEnumerable<Character>) Game1.currentLocation.currentEvent.farmerActors).Contains<Character>(this) && !this.Name.Equals(Game1.player.Name))
        return;
      ++Game1.currentLocation.currentEvent.CurrentCommand;
    }
    else if (!this.emoteFading && (double) this.emoteInterval > 20.0 && this.currentEmoteFrame <= 3)
    {
      this.emoteInterval = 0.0f;
      ++this.currentEmoteFrame;
      if (this.currentEmoteFrame != 4)
        return;
      this.currentEmoteFrame = this.currentEmote;
    }
    else
    {
      if (this.emoteFading || (double) this.emoteInterval <= 250.0)
        return;
      this.emoteInterval = 0.0f;
      ++this.currentEmoteFrame;
      if (this.currentEmoteFrame < this.currentEmote + 4)
        return;
      this.emoteFading = true;
      this.currentEmoteFrame = 3;
    }
  }

  /// <summary>Play a sound for the current player only if they're near this player.</summary>
  /// <param name="audioName">The sound ID to play.</param>
  /// <param name="pitch">The pitch modifier to apply, or <c>null</c> to keep it as-is.</param>
  /// <param name="context">The source which triggered the sound.</param>
  public void playNearbySoundLocal(string audioName, int? pitch = null, SoundContext context = SoundContext.Default)
  {
    if (this.currentLocation == null && (this is Farmer farmer ? (!farmer.IsLocalPlayer ? 1 : 0) : 1) != 0)
      return;
    Game1.sounds.PlayLocal(audioName, this.currentLocation, new Vector2?(this.Tile), pitch, context, out ICue _);
  }

  /// <summary>Play a sound for each nearby online player.</summary>
  /// <param name="audioName">The sound ID to play.</param>
  /// <param name="pitch">The pitch modifier to apply, or <c>null</c> to keep it as-is.</param>
  /// <param name="context">The source which triggered the sound.</param>
  public void playNearbySoundAll(string audioName, int? pitch = null, SoundContext context = SoundContext.Default)
  {
    if (this.currentLocation == null)
    {
      if ((this is Farmer farmer ? (!farmer.IsLocalPlayer ? 1 : 0) : 1) != 0)
        return;
      Game1.sounds.PlayLocal(audioName, (GameLocation) null, new Vector2?(), pitch, context, out ICue _);
    }
    else
      Game1.sounds.PlayAll(audioName, this.currentLocation, new Vector2?(this.Tile), pitch, context);
  }

  public Vector2 GetGrabTile()
  {
    Microsoft.Xna.Framework.Rectangle boundingBox = this.GetBoundingBox();
    switch (this.FacingDirection)
    {
      case 0:
        return new Vector2((float) ((boundingBox.X + boundingBox.Width / 2) / 64 /*0x40*/), (float) ((boundingBox.Y - 5) / 64 /*0x40*/));
      case 1:
        return new Vector2((float) ((boundingBox.X + boundingBox.Width + 5) / 64 /*0x40*/), (float) ((boundingBox.Y + boundingBox.Height / 2) / 64 /*0x40*/));
      case 2:
        return new Vector2((float) ((boundingBox.X + boundingBox.Width / 2) / 64 /*0x40*/), (float) ((boundingBox.Y + boundingBox.Height + 5) / 64 /*0x40*/));
      case 3:
        return new Vector2((float) ((boundingBox.X - 5) / 64 /*0x40*/), (float) ((boundingBox.Y + boundingBox.Height / 2) / 64 /*0x40*/));
      default:
        return this.getStandingPosition();
    }
  }

  public Vector2 GetDropLocation()
  {
    Microsoft.Xna.Framework.Rectangle boundingBox = this.GetBoundingBox();
    switch (this.FacingDirection)
    {
      case 0:
        return new Vector2((float) (boundingBox.X + 16 /*0x10*/), (float) (boundingBox.Y - 64 /*0x40*/));
      case 1:
        return new Vector2((float) (boundingBox.X + boundingBox.Width + 64 /*0x40*/), (float) (boundingBox.Y + 16 /*0x10*/));
      case 2:
        return new Vector2((float) (boundingBox.X + 16 /*0x10*/), (float) (boundingBox.Y + boundingBox.Height + 64 /*0x40*/));
      case 3:
        return new Vector2((float) (boundingBox.X - 64 /*0x40*/), (float) (boundingBox.Y + 16 /*0x10*/));
      default:
        return this.getStandingPosition();
    }
  }

  public virtual Vector2 GetToolLocation(Vector2 target_position, bool ignoreClick = false)
  {
    int num = this.FacingDirection;
    if ((Game1.player.CurrentTool == null || !Game1.player.CurrentTool.CanUseOnStandingTile()) && (int) ((double) target_position.X / 64.0) == Game1.player.TilePoint.X && (int) ((double) target_position.Y / 64.0) == Game1.player.TilePoint.Y)
    {
      Microsoft.Xna.Framework.Rectangle boundingBox = this.GetBoundingBox();
      switch (this.FacingDirection)
      {
        case 0:
          return new Vector2((float) (boundingBox.X + boundingBox.Width / 2), (float) (boundingBox.Y - 64 /*0x40*/));
        case 1:
          return new Vector2((float) (boundingBox.X + boundingBox.Width + 64 /*0x40*/), (float) (boundingBox.Y + boundingBox.Height / 2));
        case 2:
          return new Vector2((float) (boundingBox.X + boundingBox.Width / 2), (float) (boundingBox.Y + boundingBox.Height + 64 /*0x40*/));
        case 3:
          return new Vector2((float) (boundingBox.X - 64 /*0x40*/), (float) (boundingBox.Y + boundingBox.Height / 2));
      }
    }
    if (!ignoreClick && !target_position.Equals(Vector2.Zero) && this.Name.Equals(Game1.player.Name))
    {
      bool flag = false;
      if (Game1.player.CurrentTool != null && Game1.player.CurrentTool.CanUseOnStandingTile())
        flag = true;
      if (Utility.withinRadiusOfPlayer((int) target_position.X, (int) target_position.Y, 1, Game1.player))
      {
        num = Game1.player.getGeneralDirectionTowards(new Vector2((float) (int) target_position.X, (float) (int) target_position.Y));
        if (flag)
          return target_position;
        Point standingPixel = Game1.player.StandingPixel;
        if ((double) Math.Abs(target_position.X - (float) standingPixel.X) >= 32.0 || (double) Math.Abs(target_position.Y - (float) standingPixel.Y) >= 32.0)
          return target_position;
      }
    }
    Microsoft.Xna.Framework.Rectangle boundingBox1 = this.GetBoundingBox();
    switch (num)
    {
      case 0:
        return new Vector2((float) (boundingBox1.X + boundingBox1.Width / 2), (float) (boundingBox1.Y - 48 /*0x30*/));
      case 1:
        return new Vector2((float) (boundingBox1.X + boundingBox1.Width + 48 /*0x30*/), (float) (boundingBox1.Y + boundingBox1.Height / 2));
      case 2:
        return new Vector2((float) (boundingBox1.X + boundingBox1.Width / 2), (float) (boundingBox1.Y + boundingBox1.Height + 48 /*0x30*/));
      case 3:
        return new Vector2((float) (boundingBox1.X - 48 /*0x30*/), (float) (boundingBox1.Y + boundingBox1.Height / 2));
      default:
        return this.getStandingPosition();
    }
  }

  public virtual Vector2 GetToolLocation(bool ignoreClick = false)
  {
    if (!Game1.wasMouseVisibleThisFrame || Game1.isAnyGamePadButtonBeingHeld())
      ignoreClick = true;
    return this.GetToolLocation(this.lastClick, ignoreClick);
  }

  public int getGeneralDirectionTowards(
    Vector2 target,
    int yBias = 0,
    bool opposite = false,
    bool useTileCalculations = true)
  {
    int num1 = opposite ? -1 : 1;
    Point standingPixel = this.StandingPixel;
    int num2;
    int num3;
    if (useTileCalculations)
    {
      Point tilePoint = this.TilePoint;
      num2 = ((int) ((double) target.X / 64.0) - tilePoint.X) * num1;
      num3 = ((int) ((double) target.Y / 64.0) - tilePoint.Y) * num1;
      if (num2 == 0 && num3 == 0)
      {
        Vector2 vector2 = new Vector2((float) (((double) (int) ((double) target.X / 64.0) + 0.5) * 64.0), (float) (((double) (int) ((double) target.Y / 64.0) + 0.5) * 64.0));
        num2 = (int) ((double) vector2.X - (double) standingPixel.X) * num1;
        num3 = (int) ((double) vector2.Y - (double) standingPixel.Y) * num1;
        yBias *= 64 /*0x40*/;
      }
    }
    else
    {
      num2 = (int) ((double) target.X - (double) standingPixel.X) * num1;
      num3 = (int) ((double) target.Y - (double) standingPixel.Y) * num1;
    }
    if (num2 > Math.Abs(num3) + yBias)
      return 1;
    if (Math.Abs(num2) > Math.Abs(num3) + yBias)
      return 3;
    return num3 > 0 || ((double) standingPixel.Y - (double) target.Y) * (double) num1 < 0.0 ? 2 : 0;
  }

  public void faceGeneralDirection(
    Vector2 target,
    int yBias,
    bool opposite,
    bool useTileCalculations)
  {
    this.faceDirection(this.getGeneralDirectionTowards(target, yBias, opposite, useTileCalculations));
  }

  public void faceGeneralDirection(Vector2 target, int yBias = 0, bool opposite = false)
  {
    this.faceGeneralDirection(target, yBias, opposite, true);
  }

  public virtual void draw(SpriteBatch b) => this.draw(b, 1f);

  public virtual void drawAboveAlwaysFrontLayer(SpriteBatch b)
  {
  }

  public virtual void draw(SpriteBatch b, float alpha = 1f)
  {
    Vector2 position = this.Position;
    this.Sprite.draw(b, Game1.GlobalToLocal(Game1.viewport, position), (float) this.StandingPixel.Y / 10000f);
    if (!this.IsEmoting)
      return;
    Vector2 localPosition = this.getLocalPosition(Game1.viewport);
    localPosition.Y -= 96f;
    localPosition.Y += (float) this.emoteYOffset;
    localPosition.X += (float) ((double) (this.Sprite.SourceRect.Width * 4) / 2.0 - 32.0);
    b.Draw(Game1.emoteSpriteSheet, localPosition, new Microsoft.Xna.Framework.Rectangle?(new Microsoft.Xna.Framework.Rectangle(this.CurrentEmoteIndex * 16 /*0x10*/ % Game1.emoteSpriteSheet.Width, this.CurrentEmoteIndex * 16 /*0x10*/ / Game1.emoteSpriteSheet.Width * 16 /*0x10*/, 16 /*0x10*/, 16 /*0x10*/)), Color.White * alpha, 0.0f, Vector2.Zero, 4f, SpriteEffects.None, (float) this.StandingPixel.Y / 10000f);
  }

  public virtual void draw(SpriteBatch b, int ySourceRectOffset, float alpha = 1f)
  {
    Microsoft.Xna.Framework.Rectangle boundingBox = this.GetBoundingBox();
    this.Sprite.draw(b, Game1.GlobalToLocal(Game1.viewport, this.Position) + new Vector2((float) (this.GetSpriteWidthForPositioning() * 4 / 2), (float) (boundingBox.Height / 2)), (float) boundingBox.Center.Y / 10000f, 0, ySourceRectOffset, Color.White, scale: 4f, characterSourceRectOffset: true);
    if (!this.IsEmoting)
      return;
    Vector2 localPosition = this.getLocalPosition(Game1.viewport);
    localPosition.Y -= 96f;
    localPosition.Y += (float) this.emoteYOffset;
    localPosition.X += (float) ((double) (this.Sprite.SourceRect.Width * 4) / 2.0 - 32.0);
    b.Draw(Game1.emoteSpriteSheet, localPosition, new Microsoft.Xna.Framework.Rectangle?(new Microsoft.Xna.Framework.Rectangle(this.CurrentEmoteIndex * 16 /*0x10*/ % Game1.emoteSpriteSheet.Width, this.CurrentEmoteIndex * 16 /*0x10*/ / Game1.emoteSpriteSheet.Width * 16 /*0x10*/, 16 /*0x10*/, 16 /*0x10*/)), Color.White * alpha, 0.0f, Vector2.Zero, 4f, SpriteEffects.None, (float) this.StandingPixel.Y / 10000f);
  }

  public int GetSpriteWidthForPositioning()
  {
    return this.forceOneTileWide.Value ? 16 /*0x10*/ : this.Sprite.SpriteWidth;
  }

  public virtual Microsoft.Xna.Framework.Rectangle GetBoundingBox()
  {
    if (this.Sprite == null)
      return Microsoft.Xna.Framework.Rectangle.Empty;
    Vector2 position = this.Position;
    int width = this.GetSpriteWidthForPositioning() * 4 * 3 / 4;
    return new Microsoft.Xna.Framework.Rectangle((int) position.X + 8, (int) position.Y + 16 /*0x10*/, width, 32 /*0x20*/);
  }

  public void stopWithoutChangingFrame()
  {
    this.moveDown = false;
    this.moveLeft = false;
    this.moveRight = false;
    this.moveUp = false;
  }

  public virtual void collisionWithFarmerBehavior()
  {
  }

  /// <summary>Get the pixel coordinates at the center of this character's bounding box as a vector, relative to the top-left corner of the map.</summary>
  /// <remarks>See <see cref="P:StardewValley.Character.StandingPixel" /> for a point version.</remarks>
  public Vector2 getStandingPosition()
  {
    Point standingPixel = this.StandingPixel;
    return new Vector2((float) standingPixel.X, (float) standingPixel.Y);
  }

  public Vector2 getLocalPosition(xTile.Dimensions.Rectangle viewport)
  {
    Vector2 position = this.Position;
    return new Vector2(position.X - (float) viewport.X, position.Y - (float) viewport.Y + (float) this.yJumpOffset) + this.drawOffset;
  }

  public virtual bool isMoving()
  {
    return this.moveUp || this.moveDown || this.moveRight || this.moveLeft || this.position.Field.IsInterpolating();
  }

  public void setTileLocation(Vector2 tileLocation)
  {
    float num1 = (float) (((double) tileLocation.X + 0.5) * 64.0);
    float num2 = (float) (((double) tileLocation.Y + 0.5) * 64.0);
    Vector2 position = this.Position;
    Microsoft.Xna.Framework.Rectangle boundingBox = this.GetBoundingBox();
    position.X += num1 - (float) boundingBox.Center.X;
    position.Y += num2 - (float) boundingBox.Center.Y;
    this.Position = position;
  }

  public void startGlowing(Color glowingColor, bool border, float glowRate)
  {
    if (this.glowingColor.Equals(glowingColor))
      return;
    this.isGlowing = true;
    this.coloredBorder = border;
    this.glowingColor = glowingColor;
    this.glowUp = true;
    this.glowRate = glowRate;
    this.glowingTransparency = 0.0f;
  }

  public void stopGlowing()
  {
    this.isGlowing = false;
    this.glowingColor = Color.White;
  }

  public virtual void jumpWithoutSound(float velocity = 8f)
  {
    this.yJumpVelocity = velocity;
    this.yJumpOffset = -1;
    this.yJumpGravity = -0.5f;
  }

  public virtual void jump()
  {
    this.yJumpVelocity = 8f;
    this.yJumpOffset = -1;
    this.yJumpGravity = -0.5f;
    this.wasJumpWithSound = true;
    this.currentLocation?.localSound("dwop");
  }

  public virtual void jump(float jumpVelocity)
  {
    this.yJumpVelocity = jumpVelocity;
    this.yJumpOffset = -1;
    this.yJumpGravity = -0.5f;
    this.wasJumpWithSound = true;
    this.currentLocation?.localSound("dwop");
  }

  public void faceTowardFarmerForPeriod(int milliseconds, int radius, bool faceAway, Farmer who)
  {
    if (this.SimpleNonVillagerNPC || (this.Sprite == null || this.Sprite.CurrentAnimation != null) && !this.isMoving())
      return;
    if (this.isMoving())
      milliseconds /= 2;
    this.faceTowardFarmerEvent.Fire(milliseconds);
    this.faceTowardFarmerEvent.Poll();
    if (this.facingDirectionBeforeSpeakingToPlayer.Value == -1)
      this.facingDirectionBeforeSpeakingToPlayer.Value = this.FacingDirection;
    this.faceTowardFarmerRadius.Value = radius;
    this.faceAwayFromFarmer.Value = faceAway;
    this.whoToFace.Value = who;
    this.hasJustStartedFacingPlayer = true;
  }

  protected void performFaceTowardFarmerEvent(int milliseconds)
  {
    if ((this.Sprite == null || this.Sprite.CurrentAnimation != null) && !this.isMoving())
      return;
    this.Halt();
    this.faceTowardFarmerTimer = milliseconds;
    this.movementPause = milliseconds;
  }

  public virtual void update(GameTime time, GameLocation location)
  {
    this.position.UpdateExtrapolation((float) this.speed + this.addedSpeed);
    this.update(time, location, 0L, true);
  }

  /// <summary>Update when the location containing this character is removed from the game (e.g. a mine level that was unloaded).</summary>
  public virtual void OnLocationRemoved()
  {
  }

  public virtual void checkForFootstep() => Game1.currentLocation.playTerrainSound(this.Tile, this);

  public virtual void update(GameTime time, GameLocation location, long id, bool move)
  {
    this.position.UpdateExtrapolation((float) this.speed + this.addedSpeed);
    this.currentLocation = location;
    this.faceTowardFarmerEvent.Poll();
    if (this.yJumpOffset != 0)
    {
      this.yJumpVelocity += this.yJumpGravity;
      this.yJumpOffset -= (int) this.yJumpVelocity;
      if (this.yJumpOffset >= 0)
      {
        this.yJumpOffset = 0;
        this.yJumpVelocity = 0.0f;
        if (!this.IsMonster && (location == null || location.Equals(Game1.currentLocation)) && this.wasJumpWithSound)
          this.checkForFootstep();
      }
    }
    if (this.forceUpdateTimer > 0)
      this.forceUpdateTimer -= time.ElapsedGameTime.Milliseconds;
    this.updateGlow();
    this.updateEmote(time);
    this.updateFaceTowardsFarmer(time, location);
    bool flag = false;
    if (location.currentEvent != null)
    {
      if (location.IsTemporary)
        flag = true;
      else if (((IEnumerable<Character>) location.currentEvent.actors).Contains<Character>(this))
        flag = true;
    }
    if (Game1.IsMasterGame | flag)
    {
      if (this.controller == null & move && !this.freezeMotion)
        this.updateMovement(location, time);
      if (this.controller != null && !this.freezeMotion && this.controller.update(time))
        this.controller = (PathFindController) null;
    }
    else
      this.updateSlaveAnimation(time);
    this.hasJustStartedFacingPlayer = false;
  }

  public virtual void updateFaceTowardsFarmer(GameTime time, GameLocation location)
  {
    if (this.faceTowardFarmerTimer > 0)
    {
      this.faceTowardFarmerTimer -= time.ElapsedGameTime.Milliseconds;
      if (this.whoToFace.Value != null)
      {
        Vector2 tile = this.Tile;
        if (!this.faceTowardFarmer && this.faceTowardFarmerTimer > 0 && Utility.tileWithinRadiusOfPlayer((int) tile.X, (int) tile.Y, this.faceTowardFarmerRadius.Value, this.whoToFace.Value))
          this.faceTowardFarmer = true;
        else if (!Utility.tileWithinRadiusOfPlayer((int) tile.X, (int) tile.Y, this.faceTowardFarmerRadius.Value, this.whoToFace.Value) || this.faceTowardFarmerTimer <= 0)
        {
          this.faceDirection(this.facingDirectionBeforeSpeakingToPlayer.Value);
          if (this.faceTowardFarmerTimer <= 0)
          {
            this.facingDirectionBeforeSpeakingToPlayer.Value = -1;
            this.faceTowardFarmer = false;
            this.faceAwayFromFarmer.Value = false;
            this.faceTowardFarmerTimer = 0;
          }
        }
      }
    }
    if ((Game1.IsMasterGame || location.currentEvent != null) && this.faceTowardFarmer && this.whoToFace.Value != null)
    {
      this.faceGeneralDirection(this.whoToFace.Value.getStandingPosition(), 0, false, true);
      if (this.faceAwayFromFarmer.Value)
        this.faceDirection((this.FacingDirection + 2) % 4);
    }
    this.hasJustStartedFacingPlayer = false;
  }

  public virtual bool hasSpecialCollisionRules() => false;

  /// <summary>
  /// 
  /// make sure that you also override hasSpecialCollisionRules() in any class that overrides isColliding().
  /// Otherwise isColliding() will never be called.
  /// dumb I kno
  /// </summary>
  /// <param name="l"></param>
  /// <param name="tile"></param>
  /// <returns></returns>
  public virtual bool isColliding(GameLocation l, Vector2 tile) => false;

  public virtual void animateInFacingDirection(GameTime time)
  {
    switch (this.FacingDirection)
    {
      case 0:
        this.Sprite.AnimateUp(time);
        break;
      case 1:
        this.Sprite.AnimateRight(time);
        break;
      case 2:
        this.Sprite.AnimateDown(time);
        break;
      case 3:
        this.Sprite.AnimateLeft(time);
        break;
    }
  }

  public virtual void updateMovement(GameLocation location, GameTime time)
  {
  }

  protected virtual void updateSlaveAnimation(GameTime time)
  {
    if (this.Sprite.CurrentAnimation != null)
    {
      this.Sprite.animateOnce(time);
    }
    else
    {
      if (this.SimpleNonVillagerNPC)
        return;
      this.faceDirection(this.FacingDirection);
      if (this.isMoving())
        this.animateInFacingDirection(time);
      else
        this.Sprite.StopAnimation();
    }
  }

  public void updateGlow()
  {
    if (!this.isGlowing)
      return;
    if (this.glowUp)
    {
      this.glowingTransparency += this.glowRate;
      if ((double) this.glowingTransparency < 1.0)
        return;
      this.glowingTransparency = 1f;
      this.glowUp = false;
    }
    else
    {
      this.glowingTransparency -= this.glowRate;
      if ((double) this.glowingTransparency > 0.0)
        return;
      this.glowingTransparency = 0.0f;
      this.glowUp = true;
    }
  }

  public void convertEventMotionCommandToMovement(Vector2 command)
  {
    if ((double) command.X < 0.0)
      this.SetMovingLeft(true);
    else if ((double) command.X > 0.0)
      this.SetMovingRight(true);
    else if ((double) command.Y < 0.0)
    {
      this.SetMovingUp(true);
    }
    else
    {
      if ((double) command.Y <= 0.0)
        return;
      this.SetMovingDown(true);
    }
  }

  /// <summary>Draw the shadow under this character.</summary>
  /// <param name="b">The sprite batch being drawn.</param>
  public virtual void DrawShadow(SpriteBatch b)
  {
    int x = this.GetSpriteWidthForPositioning() * 4 / 2;
    int height = this.GetBoundingBox().Height;
    float scale = Math.Max(0.0f, (float) (4.0 + (double) this.yJumpOffset / 40.0)) * this.scale.Value;
    if (!this.IsMonster)
    {
      if (Game1.CurrentEvent != null && this.Sprite.SpriteHeight <= 16 /*0x10*/)
        height += -4;
      else
        height += 12;
    }
    CharacterData data;
    if (this.IsVillager && NPC.TryGetData(this.Name, out data) && data.Shadow != null)
    {
      CharacterShadowData shadow = data.Shadow;
      if (!shadow.Visible)
        return;
      x += shadow.Offset.X;
      height += shadow.Offset.Y;
      scale = Math.Max(0.0f, scale * shadow.Scale);
    }
    b.Draw(Game1.shadowTexture, Game1.GlobalToLocal(Game1.viewport, this.GetShadowOffset() + this.Position + new Vector2((float) x, (float) height)), new Microsoft.Xna.Framework.Rectangle?(Game1.shadowTexture.Bounds), Color.White, 0.0f, new Vector2((float) Game1.shadowTexture.Bounds.Center.X, (float) Game1.shadowTexture.Bounds.Center.Y), scale, SpriteEffects.None, Math.Max(0.0f, (float) this.StandingPixel.Y / 10000f) - 1E-06f);
  }
}
