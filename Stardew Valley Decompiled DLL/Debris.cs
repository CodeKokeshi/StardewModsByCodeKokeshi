// Decompiled with JetBrains decompiler
// Type: StardewValley.Debris
// Assembly: Stardew Valley, Version=1.6.15.24356, Culture=neutral, PublicKeyToken=null
// MVID: AA1F513A-94F0-4EF6-A35F-2D5B4A3721BF
// Assembly location: D:\SteamLibrary\steamapps\common\Stardew Valley\Stardew Valley.dll
// XML documentation location: D:\SteamLibrary\steamapps\common\Stardew Valley\Stardew Valley.xml

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Netcode;
using StardewValley.Extensions;
using StardewValley.ItemTypeDefinitions;
using StardewValley.Locations;
using StardewValley.Network;
using System;

#nullable disable
namespace StardewValley;

public class Debris : INetObject<NetFields>
{
  public const int copperDebris = 0;
  public const int ironDebris = 2;
  public const int coalDebris = 4;
  public const int goldDebris = 6;
  public const int coinsDebris = 8;
  public const int iridiumDebris = 10;
  public const int woodDebris = 12;
  public const int stoneDebris = 14;
  public const int bigStoneDebris = 32 /*0x20*/;
  public const int bigWoodDebris = 34;
  public const int timesToBounce = 2;
  public const float gravity = 0.4f;
  public const float timeToWaitBeforeRemoval = 600f;
  public const int marginForChunkPickup = 64 /*0x40*/;
  public const int white = 10000;
  public const int green = 100001;
  public const int blue = 100002;
  public const int red = 100003;
  public const int yellow = 100004;
  public const int black = 100005;
  public const int charcoal = 100007;
  public const int gray = 100006;
  private float relativeXPosition;
  private readonly NetObjectShrinkList<Chunk> chunks = new NetObjectShrinkList<Chunk>();
  public readonly NetInt chunkType = new NetInt();
  public readonly NetInt sizeOfSourceRectSquares = new NetInt(8);
  private readonly NetInt netItemQuality = new NetInt();
  private readonly NetInt netChunkFinalYLevel = new NetInt();
  private readonly NetInt netChunkFinalYTarget = new NetInt();
  public float timeSinceDoneBouncing;
  public readonly NetFloat scale = new NetFloat(1f).Interpolated(true, true);
  protected NetBool _chunksMoveTowardsPlayer = new NetBool(false).Interpolated(false, false);
  public readonly NetLong DroppedByPlayerID = new NetLong().Interpolated(false, false);
  private bool movingUp;
  public readonly NetBool floppingFish = new NetBool();
  public bool isFishable;
  public bool movingFinalYLevel;
  public readonly NetEnum<Debris.DebrisType> debrisType = new NetEnum<Debris.DebrisType>(Debris.DebrisType.CHUNKS);
  /// <summary>Whether the debris is in water and will sink soon.</summary>
  public readonly NetBool isSinking = new NetBool(false);
  public readonly NetString debrisMessage = new NetString("");
  public readonly NetColor nonSpriteChunkColor = new NetColor(Color.White);
  public readonly NetColor chunksColor = new NetColor();
  private float animationTimer;
  private int timeBeforeReturnToDroppingPlayer = 1200;
  public readonly NetString spriteChunkSheetName = new NetString();
  private Texture2D _spriteChunkSheet;
  public readonly NetString itemId = new NetString();
  private readonly NetRef<Item> netItem = new NetRef<Item>();
  public Character toHover;
  public readonly NetFarmerRef player = new NetFarmerRef();

  public int itemQuality
  {
    get => this.netItemQuality.Value;
    set => this.netItemQuality.Value = value;
  }

  public int chunkFinalYLevel
  {
    get => this.netChunkFinalYLevel.Value;
    set => this.netChunkFinalYLevel.Value = value;
  }

  public int chunkFinalYTarget
  {
    get => this.netChunkFinalYTarget.Value;
    set => this.netChunkFinalYTarget.Value = value;
  }

  public bool chunksMoveTowardPlayer
  {
    get => this._chunksMoveTowardsPlayer.Value;
    set => this._chunksMoveTowardsPlayer.Value = value;
  }

  public Texture2D spriteChunkSheet
  {
    get
    {
      if (this._spriteChunkSheet == null && this.spriteChunkSheetName.Value != null)
        this._spriteChunkSheet = Game1.content.Load<Texture2D>(this.spriteChunkSheetName.Value);
      return this._spriteChunkSheet;
    }
  }

  public Item item
  {
    get => this.netItem.Value;
    set => this.netItem.Value = value;
  }

  public NetFields NetFields { get; } = new NetFields(nameof (Debris));

  public Debris() => this.InitNetFields();

  public virtual void InitNetFields()
  {
    this.NetFields.SetOwner((INetObject<NetFields>) this).AddField((INetSerializable) this.chunks, "chunks").AddField((INetSerializable) this.chunkType, "chunkType").AddField((INetSerializable) this.sizeOfSourceRectSquares, "sizeOfSourceRectSquares").AddField((INetSerializable) this.netItemQuality, "netItemQuality").AddField((INetSerializable) this.netChunkFinalYLevel, "netChunkFinalYLevel").AddField((INetSerializable) this.netChunkFinalYTarget, "netChunkFinalYTarget").AddField((INetSerializable) this.scale, "scale").AddField((INetSerializable) this.floppingFish, "floppingFish").AddField((INetSerializable) this.debrisType, "debrisType").AddField((INetSerializable) this.isSinking, "isSinking").AddField((INetSerializable) this.debrisMessage, "debrisMessage").AddField((INetSerializable) this.nonSpriteChunkColor, "nonSpriteChunkColor").AddField((INetSerializable) this.chunksColor, "chunksColor").AddField((INetSerializable) this.spriteChunkSheetName, "spriteChunkSheetName").AddField((INetSerializable) this.netItem, "netItem").AddField((INetSerializable) this.player.NetFields, "player.NetFields").AddField((INetSerializable) this.DroppedByPlayerID, "DroppedByPlayerID").AddField((INetSerializable) this._chunksMoveTowardsPlayer, "_chunksMoveTowardsPlayer").AddField((INetSerializable) this.itemId, "itemId");
    this.player.Delayed(false);
  }

  public NetObjectShrinkList<Chunk> Chunks => this.chunks;

  /// <summary>Construct an instance for resource/item debris.</summary>
  public Debris(int debris_type, Vector2 debrisOrigin, Vector2 playerPosition)
    : this(debris_type, 1, debrisOrigin, playerPosition)
  {
  }

  /// <summary>Construct an instance for resource/item type debris.</summary>
  public Debris(
    int resource_type,
    int numberOfChunks,
    Vector2 debrisOrigin,
    Vector2 playerPosition,
    float velocityMultiplyer = 1f)
    : this()
  {
    this.InitializeResource(resource_type);
    this.InitializeChunks(numberOfChunks, debrisOrigin, playerPosition, velocityMultiplyer);
  }

  /// <summary>Construct an instance for cosmetic "chunks".</summary>
  public Debris(
    int debrisType,
    int numberOfChunks,
    Vector2 debrisOrigin,
    Vector2 playerPosition,
    int groundLevel,
    Color? color = null)
    : this()
  {
    this.debrisType.Value = Debris.DebrisType.CHUNKS;
    this.chunkType.Value = debrisType;
    this.chunksColor.Value = color ?? Debris.getColorForDebris(debrisType);
    this.InitializeChunks(numberOfChunks, debrisOrigin, playerPosition);
  }

  /// <summary>Construct an instance for floating items.</summary>
  public Debris(string item_id, Vector2 debrisOrigin, Vector2 playerPosition)
    : this(item_id, 1, debrisOrigin, playerPosition)
  {
  }

  /// <summary>Construct an instance for floating items.</summary>
  public Debris(
    string item_id,
    int numberOfChunks,
    Vector2 debrisOrigin,
    Vector2 playerPosition,
    float velocityMultiplyer = 1f)
    : this()
  {
    this.InitializeItem(item_id);
    this.InitializeChunks(numberOfChunks, debrisOrigin, playerPosition, velocityMultiplyer);
  }

  public virtual void InitializeItem(string item_id)
  {
    if (this.debrisType.Value == Debris.DebrisType.CHUNKS)
      this.debrisType.Value = Debris.DebrisType.OBJECT;
    this.itemId.Value = item_id;
    ParsedItemData data = ItemRegistry.GetData(this.itemId.Value);
    if (this.item != null)
      return;
    if (data.HasTypeObject())
    {
      this.floppingFish.Value = data.Category == -4 && data.InternalName != "Mussel";
      this.isFishable = data.ObjectType == "Fish";
      if (!(data.ObjectType == "Arch"))
        return;
      this.debrisType.Value = Debris.DebrisType.ARCHAEOLOGY;
    }
    else
      this.item = ItemRegistry.Create(this.itemId.Value);
  }

  public virtual void InitializeResource(int item_id)
  {
    this.debrisType.Value = Debris.DebrisType.OBJECT;
    switch (item_id)
    {
      case 0:
      case 378:
        this.itemId.Value = "(O)378";
        this.debrisType.Value = Debris.DebrisType.RESOURCE;
        break;
      case 2:
      case 380:
        this.itemId.Value = "(O)380";
        this.debrisType.Value = Debris.DebrisType.RESOURCE;
        break;
      case 4:
      case 382:
        this.itemId.Value = "(O)382";
        this.debrisType.Value = Debris.DebrisType.RESOURCE;
        break;
      case 6:
      case 384:
        this.itemId.Value = "(O)384";
        this.debrisType.Value = Debris.DebrisType.RESOURCE;
        break;
      case 10:
      case 386:
        this.itemId.Value = "(O)386";
        this.debrisType.Value = Debris.DebrisType.RESOURCE;
        break;
      case 12:
      case 388:
        this.itemId.Value = "(O)388";
        this.debrisType.Value = Debris.DebrisType.RESOURCE;
        break;
      case 14:
      case 390:
        this.itemId.Value = "(O)390";
        this.debrisType.Value = Debris.DebrisType.RESOURCE;
        break;
      default:
        this.itemId.Value = "(O)" + item_id.ToString();
        break;
    }
    if (this.itemId.Value == null)
      return;
    this.InitializeItem(this.itemId.Value);
  }

  /// <summary>Construct an instance for floating items.</summary>
  public Debris(Item item, Vector2 debrisOrigin)
    : this()
  {
    this.item = item;
    item.resetState();
    this.InitializeItem(item.QualifiedItemId);
    this.InitializeChunks(1, debrisOrigin, Utility.PointToVector2(Game1.player.StandingPixel));
  }

  /// <summary>Construct an instance for floating items.</summary>
  public Debris(Item item, Vector2 debrisOrigin, Vector2 targetLocation)
    : this()
  {
    this.item = item;
    item.resetState();
    this.InitializeItem(item.QualifiedItemId);
    this.InitializeChunks(1, debrisOrigin, targetLocation);
  }

  /// <summary>Construct an instance for numbers.</summary>
  public Debris(
    int number,
    Vector2 debrisOrigin,
    Color messageColor,
    float scale,
    Character toHover)
    : this()
  {
    this.chunkType.Value = number;
    this.debrisType.Value = Debris.DebrisType.NUMBERS;
    this.nonSpriteChunkColor.Value = messageColor;
    this.InitializeChunks(1, debrisOrigin, Game1.player.Position);
    this.chunks[0].scale = scale;
    this.toHover = toHover;
    this.chunks[0].xVelocity.Value = (float) Game1.random.Next(-1, 2);
    this.updateHoverPosition(this.chunks[0]);
  }

  /// <summary>Construct an instance for letters.</summary>
  public Debris(
    string message,
    int numberOfChunks,
    Vector2 debrisOrigin,
    Color messageColor,
    float scale,
    float rotation)
    : this()
  {
    this.debrisType.Value = Debris.DebrisType.LETTERS;
    this.debrisMessage.Value = message;
    this.nonSpriteChunkColor.Value = messageColor;
    this.InitializeChunks(numberOfChunks, debrisOrigin, Game1.player.Position);
    this.chunks[0].rotation = rotation;
    this.chunks[0].scale = scale;
  }

  /// <summary>Construct an instance for sprite chunks.</summary>
  public Debris(string spriteSheet, int numberOfChunks, Vector2 debrisOrigin)
    : this()
  {
    this.InitializeChunks(numberOfChunks, debrisOrigin, Game1.player.Position);
    this.debrisType.Value = Debris.DebrisType.SPRITECHUNKS;
    this.spriteChunkSheetName.Value = spriteSheet;
    for (int index = 0; index < this.chunks.Count; ++index)
    {
      Chunk chunk = this.chunks[index];
      chunk.xSpriteSheet.Value = Game1.random.Next(0, 56);
      chunk.ySpriteSheet.Value = Game1.random.Next(0, 88);
      chunk.scale = 1f;
    }
  }

  /// <summary>Construct an instance for sprite chunks.</summary>
  public Debris(
    string spriteSheet,
    Rectangle sourceRect,
    int numberOfChunks,
    Vector2 debrisOrigin)
    : this()
  {
    this.InitializeChunks(numberOfChunks, debrisOrigin, Game1.player.Position);
    this.debrisType.Value = Debris.DebrisType.SPRITECHUNKS;
    this.spriteChunkSheetName.Value = spriteSheet;
    for (int index = 0; index < this.chunks.Count; ++index)
    {
      Chunk chunk = this.chunks[index];
      chunk.xSpriteSheet.Value = Game1.random.Next(sourceRect.X, sourceRect.X + sourceRect.Width - 4);
      chunk.ySpriteSheet.Value = Game1.random.Next(sourceRect.Y, sourceRect.Y + sourceRect.Width - 4);
      chunk.scale = 1f;
    }
  }

  /// <summary>Construct an instance for sprite chunks.</summary>
  public Debris(
    string spriteSheet,
    Rectangle sourceRect,
    int numberOfChunks,
    Vector2 debrisOrigin,
    Vector2 playerPosition,
    int groundLevel,
    int sizeOfSourceRectSquares)
    : this()
  {
    this.InitializeChunks(numberOfChunks, debrisOrigin, Game1.player.Position, 0.6f);
    this.sizeOfSourceRectSquares.Value = sizeOfSourceRectSquares;
    this.debrisType.Value = Debris.DebrisType.SPRITECHUNKS;
    this.spriteChunkSheetName.Value = spriteSheet;
    for (int index = 0; index < this.chunks.Count; ++index)
    {
      Chunk chunk = this.chunks[index];
      chunk.xSpriteSheet.Value = Game1.random.Next(2) * sizeOfSourceRectSquares + sourceRect.X;
      chunk.ySpriteSheet.Value = Game1.random.Next(2) * sizeOfSourceRectSquares + sourceRect.Y;
      chunk.rotationVelocity = Game1.random.NextBool() ? 3.14159274f / (float) Game1.random.Next(-32, -16) : 3.14159274f / (float) Game1.random.Next(16 /*0x10*/, 32 /*0x20*/);
      chunk.xVelocity.Value *= 1.2f;
      chunk.yVelocity.Value *= 1.2f;
      chunk.scale = 4f;
    }
  }

  /// <summary>Construct an instance for sprite chunks.</summary>
  public Debris(
    string spriteSheet,
    Rectangle sourceRect,
    int numberOfChunks,
    Vector2 debrisOrigin,
    Vector2 playerPosition,
    int groundLevel)
    : this()
  {
    this.InitializeChunks(numberOfChunks, debrisOrigin, playerPosition);
    this.debrisType.Value = Debris.DebrisType.SPRITECHUNKS;
    this.spriteChunkSheetName.Value = spriteSheet;
    for (int index = 0; index < this.chunks.Count; ++index)
    {
      Chunk chunk = this.chunks[index];
      chunk.xSpriteSheet.Value = Game1.random.Next(sourceRect.X, sourceRect.X + sourceRect.Width - 4);
      chunk.ySpriteSheet.Value = Game1.random.Next(sourceRect.Y, sourceRect.Y + sourceRect.Width - 4);
      chunk.scale = 1f;
    }
    this.chunkFinalYLevel = groundLevel;
  }

  public virtual bool isEssentialItem()
  {
    return this.itemId.Value == "(O)73" || this.item?.QualifiedItemId == "(O)73" || this.item != null && !this.item.canBeTrashed();
  }

  public virtual bool collect(Farmer farmer, Chunk chunk = null)
  {
    if (this.debrisType.Value == Debris.DebrisType.ARCHAEOLOGY)
      Game1.farmerFindsArtifact(this.itemId.Value);
    else if (this.item != null)
    {
      Item obj = this.item;
      this.item = (Item) null;
      if (!farmer.addItemToInventoryBool(obj))
      {
        this.item = obj;
        return false;
      }
    }
    else if ((this.debrisType.Value != Debris.DebrisType.CHUNKS || this.chunkType.Value != 8) && !farmer.addItemToInventoryBool(ItemRegistry.Create(this.itemId.Value, quality: this.itemQuality)))
      return false;
    return true;
  }

  public static Color getColorForDebris(int type)
  {
    switch (type)
    {
      case 12:
        return new Color(170, 106, 46);
      case 100001:
        return Color.LightGreen;
      case 100002:
        return Color.LightBlue;
      case 100003:
        return Color.Red;
      case 100004:
        return Color.Yellow;
      case 100005:
        return Color.Black;
      case 100006:
        return Color.Gray;
      case 100007:
        return Color.DimGray;
      default:
        return Color.White;
    }
  }

  /// <summary>Initialize the chunks, called from all constructors.</summary>
  public void InitializeChunks(
    int numberOfChunks,
    Vector2 debrisOrigin,
    Vector2 playerPosition,
    float velocityMultiplyer = 1f)
  {
    if (this.itemId.Value != null || this.chunkType.Value != -1)
      playerPosition -= (playerPosition - debrisOrigin) * 2f;
    int num1;
    int num2;
    int num3;
    int num4;
    if ((double) playerPosition.Y >= (double) debrisOrigin.Y - 32.0 && (double) playerPosition.Y <= (double) debrisOrigin.Y + 32.0)
    {
      this.chunkFinalYLevel = (int) debrisOrigin.Y - 32 /*0x20*/;
      num1 = 250;
      num2 = 300;
      if ((double) playerPosition.X < (double) debrisOrigin.X)
      {
        num3 = 20;
        num4 = 110;
      }
      else
      {
        num3 = -110;
        num4 = -20;
      }
    }
    else if ((double) playerPosition.Y < (double) debrisOrigin.Y - 32.0)
    {
      this.chunkFinalYLevel = (int) debrisOrigin.Y + (int) (32.0 * (double) velocityMultiplyer);
      num1 = 180;
      num2 = 230;
      num3 = -50;
      num4 = 50;
    }
    else
    {
      this.movingFinalYLevel = true;
      this.chunkFinalYLevel = (int) debrisOrigin.Y - 1;
      this.chunkFinalYTarget = (int) debrisOrigin.Y - (int) (96.0 * (double) velocityMultiplyer);
      this.movingUp = true;
      num1 = 350;
      num2 = 400;
      num3 = -50;
      num4 = 50;
    }
    debrisOrigin.X -= 32f;
    debrisOrigin.Y -= 32f;
    int minValue1 = (int) ((double) num3 * (double) velocityMultiplyer);
    int maxValue1 = (int) ((double) num4 * (double) velocityMultiplyer);
    int minValue2 = (int) ((double) num1 * (double) velocityMultiplyer);
    int maxValue2 = (int) ((double) num2 * (double) velocityMultiplyer);
    for (int index = 0; index < numberOfChunks; ++index)
      this.chunks.Add(new Chunk(debrisOrigin, (float) Game1.recentMultiplayerRandom.Next(minValue1, maxValue1) / 40f, (float) Game1.recentMultiplayerRandom.Next(minValue2, maxValue2) / 40f, Game1.recentMultiplayerRandom.Next(0, 2)));
  }

  private Vector2 approximatePosition()
  {
    Vector2 vector2 = new Vector2();
    foreach (Chunk chunk in this.Chunks)
      vector2 += chunk.position.Value;
    return vector2 / (float) this.Chunks.Count;
  }

  private bool playerInRange(Vector2 position, Farmer farmer)
  {
    if (this.isEssentialItem())
      return true;
    int appliedMagneticRadius = farmer.GetAppliedMagneticRadius();
    Point standingPixel = farmer.StandingPixel;
    return (double) Math.Abs(position.X + 32f - (float) standingPixel.X) <= (double) appliedMagneticRadius && (double) Math.Abs(position.Y + 32f - (float) standingPixel.Y) <= (double) appliedMagneticRadius;
  }

  private Farmer findBestPlayer(GameLocation location)
  {
    bool? isTemporary = location?.IsTemporary;
    if (isTemporary.HasValue && isTemporary.GetValueOrDefault())
      return Game1.player;
    Vector2 position = this.approximatePosition();
    float num1 = float.MaxValue;
    Farmer bestPlayer = (Farmer) null;
    foreach (Farmer farmer in location.farmers)
    {
      if ((farmer.UniqueMultiplayerID != this.DroppedByPlayerID.Value || bestPlayer == null) && this.playerInRange(position, farmer))
      {
        float num2 = (farmer.Position - position).LengthSquared();
        if ((double) num2 < (double) num1 || bestPlayer != null && bestPlayer.UniqueMultiplayerID == this.DroppedByPlayerID.Value)
        {
          bestPlayer = farmer;
          num1 = num2;
        }
      }
    }
    return bestPlayer;
  }

  public bool shouldControlThis(GameLocation location)
  {
    return Game1.IsMasterGame || (location?.IsTemporary ?? false);
  }

  public bool updateChunks(GameTime time, GameLocation location)
  {
    if (this.chunks.Count == 0)
      return true;
    this.timeSinceDoneBouncing += (float) time.ElapsedGameTime.Milliseconds;
    if ((double) this.timeSinceDoneBouncing >= (this.floppingFish.Value ? 2500.0 : (this.debrisType.Value == Debris.DebrisType.SPRITECHUNKS || this.debrisType.Value == Debris.DebrisType.NUMBERS ? 1800.0 : 600.0)))
    {
      switch (this.debrisType.Value)
      {
        case Debris.DebrisType.CHUNKS:
          if (this.chunkType.Value != 8)
            return true;
          this.chunksMoveTowardPlayer = true;
          break;
        case Debris.DebrisType.LETTERS:
        case Debris.DebrisType.SPRITECHUNKS:
        case Debris.DebrisType.NUMBERS:
          return true;
        case Debris.DebrisType.ARCHAEOLOGY:
        case Debris.DebrisType.OBJECT:
        case Debris.DebrisType.RESOURCE:
          this.chunksMoveTowardPlayer = true;
          break;
      }
      this.timeSinceDoneBouncing = 0.0f;
    }
    if (!location.farmers.Any() && !location.IsTemporary)
      return false;
    Vector2 position = this.approximatePosition();
    Farmer farmer = this.player.Value;
    if (this.isEssentialItem() && this.shouldControlThis(location) && farmer == null)
      farmer = this.findBestPlayer(location);
    TimeSpan timeSpan;
    if (this.chunksMoveTowardPlayer)
    {
      if (this.timeBeforeReturnToDroppingPlayer > 0)
      {
        int toDroppingPlayer = this.timeBeforeReturnToDroppingPlayer;
        timeSpan = time.ElapsedGameTime;
        int totalMilliseconds = (int) timeSpan.TotalMilliseconds;
        this.timeBeforeReturnToDroppingPlayer = toDroppingPlayer - totalMilliseconds;
      }
      if (!this.isEssentialItem())
      {
        if (this.player.Value != null && this.player.Value == Game1.player && !this.playerInRange(position, this.player.Value))
        {
          this.player.Value = (Farmer) null;
          farmer = (Farmer) null;
        }
        if (this.shouldControlThis(location))
        {
          if (this.player.Value != null && this.player.Value.currentLocation != location)
          {
            this.player.Value = (Farmer) null;
            farmer = (Farmer) null;
          }
          if (farmer == null)
            farmer = this.findBestPlayer(location);
        }
        if (farmer != null && this.timeBeforeReturnToDroppingPlayer > 0 && farmer.UniqueMultiplayerID == this.DroppedByPlayerID.Value)
          farmer = (Farmer) null;
      }
    }
    bool flag1 = false;
    for (int index = this.chunks.Count - 1; index >= 0; --index)
    {
      Chunk chunk1 = this.chunks[index];
      chunk1.position.UpdateExtrapolation(chunk1.getSpeed());
      if ((double) chunk1.alpha > 0.10000000149011612 && (this.debrisType.Value == Debris.DebrisType.SPRITECHUNKS || this.debrisType.Value == Debris.DebrisType.NUMBERS) && (double) this.timeSinceDoneBouncing > 600.0)
        chunk1.alpha = (float) ((1800.0 - (double) this.timeSinceDoneBouncing) / 1000.0);
      if ((double) chunk1.position.X < (double) sbyte.MinValue || (double) chunk1.position.Y < -64.0 || (double) chunk1.position.X >= (double) (location.map.DisplayWidth + 64 /*0x40*/) || (double) chunk1.position.Y >= (double) (location.map.DisplayHeight + 64 /*0x40*/))
      {
        this.chunks.RemoveAt(index);
      }
      else
      {
        if (this.item?.QualifiedItemId == "(O)GoldCoin")
        {
          double animationTimer = (double) this.animationTimer;
          timeSpan = time.ElapsedGameTime;
          double totalMilliseconds = (double) (int) timeSpan.TotalMilliseconds;
          this.animationTimer = (float) (animationTimer + totalMilliseconds);
          if ((double) this.animationTimer > 700.0)
          {
            this.animationTimer = 0.0f;
            location.temporarySprites.Add(new TemporaryAnimatedSprite("LooseSprites\\Cursors_1_6", new Rectangle(144 /*0x90*/, 249, 7, 7), 100f, 6, 1, Utility.getRandomPositionInThisRectangle(new Rectangle((int) chunk1.position.X + 32 /*0x20*/ - 4, (int) chunk1.position.Y + 32 /*0x20*/ - 4, 32 /*0x20*/, 28), Game1.random), false, false, (float) (((double) (this.chunkFinalYLevel + 64 /*0x40*/ + 8) + ((double) chunk1.position.X + 1.0) / 10000.0) / 10000.0), 0.0f, Color.White, 4f, 0.0f, 0.0f, 0.0f));
          }
        }
        bool flag2 = farmer != null;
        if (flag2)
        {
          switch (this.debrisType.Value)
          {
            case Debris.DebrisType.ARCHAEOLOGY:
            case Debris.DebrisType.OBJECT:
              if (this.item != null)
              {
                flag2 = farmer.couldInventoryAcceptThisItem(this.item);
                break;
              }
              flag2 = farmer.couldInventoryAcceptThisItem(this.itemId.Value, 1, this.itemQuality);
              if (this.itemId.Value == "(O)102" && farmer.hasMenuOpen.Value)
              {
                flag2 = false;
                break;
              }
              break;
            case Debris.DebrisType.RESOURCE:
              flag2 = farmer.couldInventoryAcceptThisItem(this.itemId.Value, 1);
              break;
            default:
              flag2 = true;
              break;
          }
          flag1 |= flag2;
          if (flag2 && this.shouldControlThis(location))
            this.player.Value = farmer;
        }
        if (((this.chunksMoveTowardPlayer ? 1 : (this.isFishable ? 1 : 0)) & (flag2 ? 1 : 0)) != 0 && this.player.Value != null)
        {
          if (this.player.Value.IsLocalPlayer)
          {
            if ((double) chunk1.position.X < (double) this.player.Value.Position.X - 12.0)
              chunk1.xVelocity.Value = Math.Min(chunk1.xVelocity.Value + 0.8f, 8f);
            else if ((double) chunk1.position.X > (double) this.player.Value.Position.X + 12.0)
              chunk1.xVelocity.Value = Math.Max(chunk1.xVelocity.Value - 0.8f, -8f);
            int y = this.player.Value.StandingPixel.Y;
            if ((double) chunk1.position.Y + 32.0 < (double) (y - 12))
              chunk1.yVelocity.Value = Math.Max(chunk1.yVelocity.Value - 0.8f, -8f);
            else if ((double) chunk1.position.Y + 32.0 > (double) (y + 12))
              chunk1.yVelocity.Value = Math.Min(chunk1.yVelocity.Value + 0.8f, 8f);
            chunk1.position.X += chunk1.xVelocity.Value;
            chunk1.position.Y -= chunk1.yVelocity.Value;
            Point standingPixel = this.player.Value.StandingPixel;
            if ((double) Math.Abs(chunk1.position.X + 32f - (float) standingPixel.X) <= 64.0 && (double) Math.Abs(chunk1.position.Y + 32f - (float) standingPixel.Y) <= 64.0)
            {
              Item obj = this.item;
              if (this.collect(this.player.Value, chunk1))
              {
                if ((double) Game1.debrisSoundInterval <= 0.0)
                {
                  Game1.debrisSoundInterval = 10f;
                  if (obj?.QualifiedItemId != "(O)73" && this.itemId.Value != "(O)73")
                    location.localSound("coin");
                }
                this.chunks.RemoveAt(index);
              }
            }
          }
        }
        else
        {
          if (this.debrisType.Value == Debris.DebrisType.NUMBERS)
            this.updateHoverPosition(chunk1);
          chunk1.position.X += chunk1.xVelocity.Value;
          chunk1.position.Y -= chunk1.yVelocity.Value;
          if (this.movingFinalYLevel)
          {
            this.chunkFinalYLevel -= (int) Math.Ceiling((double) chunk1.yVelocity.Value / 2.0);
            if (this.chunkFinalYLevel <= this.chunkFinalYTarget)
            {
              this.chunkFinalYLevel = this.chunkFinalYTarget;
              this.movingFinalYLevel = false;
            }
          }
          if (chunk1.bounces <= (this.floppingFish.Value ? 65 : 2))
          {
            if (this.debrisType.Value == Debris.DebrisType.SPRITECHUNKS)
              chunk1.yVelocity.Value -= 0.25f;
            else
              chunk1.yVelocity.Value -= 0.4f;
          }
          bool flag3 = false;
          if ((double) chunk1.position.Y >= (double) this.chunkFinalYLevel && chunk1.hasPassedRestingLineOnce.Value)
          {
            Vector2 chunkTile = new Vector2((float) (int) (((double) chunk1.position.X + 32.0) / 64.0), (float) (int) (((double) chunk1.position.Y + 32.0) / 64.0));
            bool flag4 = chunk1.bounces <= (this.floppingFish.Value ? 65 : 2);
            if (flag4)
            {
              Point tile = new Point((int) chunk1.position.X / 64 /*0x40*/, this.chunkFinalYLevel / 64 /*0x40*/);
              if (Game1.currentLocation is IslandNorth && (this.debrisType.Value == Debris.DebrisType.ARCHAEOLOGY || this.debrisType.Value == Debris.DebrisType.OBJECT || this.debrisType.Value == Debris.DebrisType.RESOURCE || this.debrisType.Value == Debris.DebrisType.CHUNKS) && Game1.currentLocation.isTileOnMap(tile.X, tile.Y) && !Game1.currentLocation.hasTileAt(tile, "Back"))
                this.chunkFinalYLevel += 48 /*0x30*/;
              ++chunk1.bounces;
              if (this.floppingFish.Value)
              {
                chunk1.yVelocity.Value = Math.Abs(chunk1.yVelocity.Value) * (!this.movingUp || chunk1.bounces >= 2 ? 0.9f : 0.6f);
                chunk1.xVelocity.Value = (float) Game1.random.Next(-250, 250) / 100f;
              }
              else
              {
                chunk1.yVelocity.Value = Math.Abs((float) ((double) chunk1.yVelocity.Value * 2.0 / 3.0));
                chunk1.rotationVelocity = Game1.random.NextBool() ? chunk1.rotationVelocity / 2f : (float) (-(double) chunk1.rotationVelocity * 2.0 / 3.0);
                chunk1.xVelocity.Value -= chunk1.xVelocity.Value / 2f;
              }
              if (this.debrisType.Value != Debris.DebrisType.LETTERS && this.debrisType.Value != Debris.DebrisType.SPRITECHUNKS && this.debrisType.Value != Debris.DebrisType.NUMBERS && location.doesTileSinkDebris((int) chunkTile.X, (int) chunkTile.Y, this.debrisType.Value))
              {
                flag3 = location.sinkDebris(this, chunkTile, chunk1.position.Value);
                if (this.isSinking.Value)
                {
                  chunk1.xVelocity.Value = 0.0f;
                  chunk1.yVelocity.Value = 0.0f;
                }
              }
              else if (this.debrisType.Value != Debris.DebrisType.LETTERS && this.debrisType.Value != Debris.DebrisType.NUMBERS && this.debrisType.Value != Debris.DebrisType.SPRITECHUNKS && (this.debrisType.Value != Debris.DebrisType.CHUNKS || this.chunkType.Value == 8) && this.shouldControlThis(location))
                location.playSound("shiny4");
            }
            if (this.isSinking.Value)
            {
              if (!flag4)
              {
                Chunk chunk2 = chunk1;
                timeSpan = Game1.currentGameTime.TotalGameTime;
                double num = Math.Sin(timeSpan.TotalSeconds * 1.25 + (double) position.X / 32.0) * 4.0;
                chunk2.bob = (float) num;
              }
              NetInt sinkTimer = chunk1.sinkTimer;
              int num1 = sinkTimer.Value;
              timeSpan = time.ElapsedGameTime;
              int milliseconds = timeSpan.Milliseconds;
              sinkTimer.Value = num1 - milliseconds;
              if (chunk1.sinkTimer.Value <= 0)
                flag3 = location.sinkDebris(this, chunkTile, chunk1.position.Value);
            }
          }
          int num2 = (int) (((double) chunk1.position.X + 32.0) / 64.0);
          int num3 = (int) (((double) chunk1.position.Y + 32.0) / 64.0);
          if (!chunk1.hitWall && location.Map.RequireLayer("Buildings").Tiles[num2, num3] != null && location.doesTileHaveProperty(num2, num3, "Passable", "Buildings") == null || location.Map.RequireLayer("Back").Tiles[num2, num3] == null)
          {
            chunk1.xVelocity.Value = -chunk1.xVelocity.Value;
            chunk1.hitWall = true;
          }
          if ((double) chunk1.position.Y < (double) this.chunkFinalYLevel)
            chunk1.hasPassedRestingLineOnce.Value = true;
          if (chunk1.bounces > (this.floppingFish.Value ? 65 : 2))
          {
            chunk1.yVelocity.Value = 0.0f;
            chunk1.xVelocity.Value = 0.0f;
            chunk1.rotationVelocity = 0.0f;
          }
          chunk1.rotation += chunk1.rotationVelocity;
          if (flag3)
            this.chunks.RemoveAt(index);
        }
      }
    }
    if (!flag1 && this.shouldControlThis(location))
      this.player.Value = (Farmer) null;
    return this.chunks.Count == 0;
  }

  public void updateHoverPosition(Chunk chunk)
  {
    if (this.toHover == null)
      return;
    this.relativeXPosition += chunk.xVelocity.Value;
    chunk.position.X = this.toHover.Position.X + 32f + this.relativeXPosition;
    chunk.scale = Math.Min(2f, Math.Max(1f, (float) (0.89999997615814209 + (double) Math.Abs(chunk.position.Y - (float) this.chunkFinalYLevel) / 128.0)));
    this.chunkFinalYLevel = this.toHover.StandingPixel.Y + 8;
    if ((double) this.timeSinceDoneBouncing > 250.0)
      chunk.alpha = Math.Max(0.0f, chunk.alpha - 0.033f);
    if (this.toHover is Farmer || this.nonSpriteChunkColor.Equals(Color.Yellow) || this.nonSpriteChunkColor.Equals(Color.Green))
      return;
    this.nonSpriteChunkColor.R = (byte) Math.Max((double) Math.Min((int) byte.MaxValue, 200 + this.chunkType.Value), Math.Min((double) Math.Min((int) byte.MaxValue, 220 + this.chunkType.Value), 400.0 * Math.Sin((double) this.timeSinceDoneBouncing / (256.0 * Math.PI) + Math.PI / 12.0)));
    this.nonSpriteChunkColor.G = (byte) Math.Max((double) (150 - this.chunkType.Value), Math.Min((double) ((int) byte.MaxValue - this.chunkType.Value), this.nonSpriteChunkColor.R > (byte) 220 ? 300.0 * Math.Sin((double) this.timeSinceDoneBouncing / (256.0 * Math.PI) + Math.PI / 12.0) : 0.0));
    this.nonSpriteChunkColor.B = (byte) Math.Max(0, Math.Min((int) byte.MaxValue, this.nonSpriteChunkColor.G > (byte) 200 ? (int) this.nonSpriteChunkColor.G - 20 : 0));
  }

  public static string getNameOfDebrisTypeFromIntId(int id)
  {
    switch (id)
    {
      case 0:
      case 1:
        return Game1.content.LoadString("Strings\\StringsFromCSFiles:Debris.cs.621");
      case 2:
      case 3:
        return Game1.content.LoadString("Strings\\StringsFromCSFiles:Debris.cs.622");
      case 4:
      case 5:
        return Game1.content.LoadString("Strings\\StringsFromCSFiles:Debris.cs.623");
      case 6:
      case 7:
        return Game1.content.LoadString("Strings\\StringsFromCSFiles:Debris.cs.624");
      case 8:
      case 9:
        return Game1.content.LoadString("Strings\\StringsFromCSFiles:Debris.cs.625");
      case 10:
      case 11:
        return Game1.content.LoadString("Strings\\StringsFromCSFiles:Debris.cs.626");
      case 12:
      case 13:
        return Game1.content.LoadString("Strings\\StringsFromCSFiles:Debris.cs.627");
      case 14:
      case 15:
        return Game1.content.LoadString("Strings\\StringsFromCSFiles:Debris.cs.628");
      case 28:
      case 29:
        return Game1.content.LoadString("Strings\\StringsFromCSFiles:Debris.cs.629");
      case 30:
      case 31 /*0x1F*/:
        return Game1.content.LoadString("Strings\\StringsFromCSFiles:Debris.cs.630");
      default:
        return "???";
    }
  }

  public enum DebrisType
  {
    /// <summary>The small 'chunks' that appear when hitting a tree with wood.</summary>
    CHUNKS = 0,
    LETTERS = 1,
    ARCHAEOLOGY = 3,
    OBJECT = 4,
    /// <summary>Sprites broken up into square chunks (i.e. the crumbs when you eat).</summary>
    SPRITECHUNKS = 5,
    RESOURCE = 6,
    NUMBERS = 7,
  }
}
