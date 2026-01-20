// Decompiled with JetBrains decompiler
// Type: StardewValley.Projectiles.Projectile
// Assembly: Stardew Valley, Version=1.6.15.24356, Culture=neutral, PublicKeyToken=null
// MVID: AA1F513A-94F0-4EF6-A35F-2D5B4A3721BF
// Assembly location: D:\SteamLibrary\steamapps\common\Stardew Valley\Stardew Valley.dll
// XML documentation location: D:\SteamLibrary\steamapps\common\Stardew Valley\Stardew Valley.xml

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Netcode;
using StardewValley.Extensions;
using StardewValley.ItemTypeDefinitions;
using StardewValley.Mods;
using StardewValley.Network;
using StardewValley.TerrainFeatures;
using System.Collections.Generic;
using System.Linq;
using System.Xml.Serialization;

#nullable disable
namespace StardewValley.Projectiles;

public abstract class Projectile : INetObject<NetFields>, IHaveModData
{
  public const int travelTimeBeforeCollisionPossible = 100;
  public const int goblinsCurseIndex = 0;
  public const int flameBallIndex = 1;
  public const int fearBolt = 2;
  public const int shadowBall = 3;
  public const int bone = 4;
  public const int throwingKnife = 5;
  public const int snowBall = 6;
  public const int shamanBolt = 7;
  public const int frostBall = 8;
  public const int frozenBolt = 9;
  public const int fireball = 10;
  public const int slash = 11;
  public const int arrowBolt = 12;
  public const int launchedSlime = 13;
  public const int magicArrow = 14;
  public const int iceOrb = 15;
  public const string projectileSheetName = "TileSheets\\Projectiles";
  public const int timePerTailUpdate = 50;
  public readonly NetInt boundingBoxWidth = new NetInt(21);
  public static Texture2D projectileSheet;
  protected float startingAlpha = 1f;
  /// <summary>The index of the sprite to draw in <see cref="F:StardewValley.Projectiles.Projectile.projectileSheetName" />. Ignored if <see cref="F:StardewValley.Projectiles.Projectile.itemId" /> is set.</summary>
  [XmlIgnore]
  public readonly NetInt currentTileSheetIndex = new NetInt();
  /// <summary>The qualified item ID for the item to draw. If set, this overrides <see cref="F:StardewValley.Projectiles.Projectile.currentTileSheetIndex" />.</summary>
  [XmlIgnore]
  public readonly NetString itemId = new NetString();
  /// <summary>The projectile's pixel position in the world.</summary>
  [XmlIgnore]
  public readonly NetPosition position = new NetPosition();
  /// <summary>The length of the tail which trails behind the main projectile.</summary>
  [XmlIgnore]
  public readonly NetInt tailLength = new NetInt();
  [XmlIgnore]
  public int tailCounter = 50;
  /// <summary>The sound to play when the projectile bounces off a wall.</summary>
  public readonly NetString bounceSound = new NetString();
  /// <summary>The number of times the projectile can bounce off walls before being destroyed.</summary>
  [XmlIgnore]
  public readonly NetInt bouncesLeft = new NetInt();
  /// <summary>The number of times the projectile can pierce through an enemy before being destroyed.</summary>
  public readonly NetInt piercesLeft = new NetInt(1);
  public int travelTime;
  protected float? _rotation;
  [XmlIgnore]
  public float hostTimeUntilAttackable = -1f;
  public readonly NetFloat startingRotation = new NetFloat();
  /// <summary>The rotation velocity.</summary>
  [XmlIgnore]
  public readonly NetFloat rotationVelocity = new NetFloat();
  public readonly NetFloat alpha = new NetFloat(1f);
  public readonly NetFloat alphaChange = new NetFloat(0.0f);
  /// <summary>The speed at which the projectile moves along the X axis.</summary>
  [XmlIgnore]
  public readonly NetFloat xVelocity = new NetFloat();
  /// <summary>The speed at which the projectile moves along the Y axis.</summary>
  [XmlIgnore]
  public readonly NetFloat yVelocity = new NetFloat();
  public readonly NetVector2 acceleration = new NetVector2();
  public readonly NetFloat maxVelocity = new NetFloat(-1f);
  public readonly NetColor color = new NetColor(Color.White);
  [XmlIgnore]
  public Queue<Vector2> tail = new Queue<Vector2>();
  public readonly NetInt maxTravelDistance = new NetInt(-1);
  public float travelDistance;
  public readonly NetInt projectileID = new NetInt(-1);
  public readonly NetInt uniqueID = new NetInt(-1);
  public NetFloat height = new NetFloat(0.0f);
  /// <summary>Whether the projectile damage monsters (true) or players (false).</summary>
  [XmlIgnore]
  public readonly NetBool damagesMonsters = new NetBool();
  [XmlIgnore]
  public readonly NetCharacterRef theOneWhoFiredMe = new NetCharacterRef();
  public readonly NetBool ignoreTravelGracePeriod = new NetBool(false);
  public readonly NetBool ignoreLocationCollision = new NetBool();
  public readonly NetBool ignoreObjectCollisions = new NetBool();
  public readonly NetBool ignoreMeleeAttacks = new NetBool(false);
  public readonly NetBool ignoreCharacterCollisions = new NetBool(false);
  public bool destroyMe;
  public readonly NetFloat startingScale = new NetFloat(1f);
  protected float? _localScale;
  public readonly NetFloat scaleGrow = new NetFloat(0.0f);
  public NetBool light = new NetBool();
  public bool hasLit;
  [XmlIgnore]
  public string lightSourceId;

  protected float rotation
  {
    get
    {
      if (!this._rotation.HasValue)
        this._rotation = new float?(this.startingRotation.Value);
      return this._rotation.Value;
    }
    set => this._rotation = new float?(value);
  }

  public bool IgnoreLocationCollision
  {
    get => this.ignoreLocationCollision.Value;
    set => this.ignoreLocationCollision.Value = value;
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

  public NetFields NetFields { get; } = new NetFields(nameof (Projectile));

  /// <summary>Construct an empty instance.</summary>
  public Projectile()
  {
    this.InitNetFields();
    this.uniqueID.Value = Game1.random.Next();
  }

  /// <summary>Initialize the collection of fields to sync in multiplayer.</summary>
  protected virtual void InitNetFields()
  {
    this.NetFields.SetOwner((INetObject<NetFields>) this).AddField((INetSerializable) this.currentTileSheetIndex, "currentTileSheetIndex").AddField((INetSerializable) this.position.NetFields, "position.NetFields").AddField((INetSerializable) this.tailLength, "tailLength").AddField((INetSerializable) this.bouncesLeft, "bouncesLeft").AddField((INetSerializable) this.bounceSound, "bounceSound").AddField((INetSerializable) this.rotationVelocity, "rotationVelocity").AddField((INetSerializable) this.startingRotation, "startingRotation").AddField((INetSerializable) this.xVelocity, "xVelocity").AddField((INetSerializable) this.yVelocity, "yVelocity").AddField((INetSerializable) this.damagesMonsters, "damagesMonsters").AddField((INetSerializable) this.theOneWhoFiredMe.NetFields, "theOneWhoFiredMe.NetFields").AddField((INetSerializable) this.ignoreLocationCollision, "ignoreLocationCollision").AddField((INetSerializable) this.maxTravelDistance, "maxTravelDistance").AddField((INetSerializable) this.ignoreTravelGracePeriod, "ignoreTravelGracePeriod").AddField((INetSerializable) this.ignoreMeleeAttacks, "ignoreMeleeAttacks").AddField((INetSerializable) this.height, "height").AddField((INetSerializable) this.startingScale, "startingScale").AddField((INetSerializable) this.scaleGrow, "scaleGrow").AddField((INetSerializable) this.color, "color").AddField((INetSerializable) this.light, "light").AddField((INetSerializable) this.itemId, "itemId").AddField((INetSerializable) this.projectileID, "projectileID").AddField((INetSerializable) this.ignoreObjectCollisions, "ignoreObjectCollisions").AddField((INetSerializable) this.acceleration, "acceleration").AddField((INetSerializable) this.maxVelocity, "maxVelocity").AddField((INetSerializable) this.alpha, "alpha").AddField((INetSerializable) this.alphaChange, "alphaChange").AddField((INetSerializable) this.boundingBoxWidth, "boundingBoxWidth").AddField((INetSerializable) this.ignoreCharacterCollisions, "ignoreCharacterCollisions").AddField((INetSerializable) this.uniqueID, "uniqueID").AddField((INetSerializable) this.modData, "modData");
  }

  /// <summary>Handle the projectile hitting an obstacle.</summary>
  /// <param name="location">The location containing the projectile.</param>
  /// <param name="target">The target player or monster that was hit, if applicable.</param>
  /// <param name="terrainFeature">The terrain feature that was hit, if applicable.</param>
  private void behaviorOnCollision(
    GameLocation location,
    Character target,
    TerrainFeature terrainFeature)
  {
    bool flag = true;
    if (!(target is Farmer player))
    {
      if (target is NPC n)
      {
        if (!n.IsInvisible)
          this.behaviorOnCollisionWithMonster(n, location);
        else
          flag = false;
      }
      else if (terrainFeature != null)
        this.behaviorOnCollisionWithTerrainFeature(terrainFeature, terrainFeature.Tile, location);
      else
        this.behaviorOnCollisionWithOther(location);
    }
    else
      this.behaviorOnCollisionWithPlayer(location, player);
    if (!flag || this.piercesLeft.Value > 0 || !this.hasLit)
      return;
    LightSource lightSource = Utility.getLightSource(this.lightSourceId);
    if (lightSource == null)
      return;
    lightSource.fadeOut.Value = 3;
  }

  public abstract void behaviorOnCollisionWithPlayer(GameLocation location, Farmer player);

  public abstract void behaviorOnCollisionWithTerrainFeature(
    TerrainFeature t,
    Vector2 tileLocation,
    GameLocation location);

  public abstract void behaviorOnCollisionWithOther(GameLocation location);

  public abstract void behaviorOnCollisionWithMonster(NPC n, GameLocation location);

  [XmlIgnore]
  public virtual float localScale
  {
    get
    {
      if (!this._localScale.HasValue)
        this._localScale = new float?(this.startingScale.Value);
      return this._localScale.Value;
    }
    set => this._localScale = new float?(value);
  }

  public virtual bool update(GameTime time, GameLocation location)
  {
    if (Game1.isTimePaused)
      return false;
    if (Game1.IsMasterGame && (double) this.hostTimeUntilAttackable > 0.0)
    {
      this.hostTimeUntilAttackable -= (float) time.ElapsedGameTime.TotalSeconds;
      if ((double) this.hostTimeUntilAttackable <= 0.0)
      {
        this.ignoreMeleeAttacks.Value = false;
        this.hostTimeUntilAttackable = -1f;
      }
    }
    if (this.light.Value)
    {
      if (!this.hasLit)
      {
        this.hasLit = true;
        this.lightSourceId = $"{this.GetType().Name}_{Game1.random.Next()}";
        if (location.Equals(Game1.currentLocation))
          Game1.currentLightSources.Add(new LightSource(this.lightSourceId, 4, this.position.Value + new Vector2(32f, 32f), 1f, new Color(Utility.getOppositeColor(this.color.Value).ToVector4() * this.alpha.Value), onlyLocation: location.NameOrUniqueName));
      }
      else
      {
        LightSource lightSource = Utility.getLightSource(this.lightSourceId);
        if (lightSource != null)
          lightSource.color.A = (byte) ((double) byte.MaxValue * (double) this.alpha.Value);
        Utility.repositionLightSource(this.lightSourceId, this.position.Value + new Vector2(32f, 32f));
      }
    }
    this.alpha.Value += this.alphaChange.Value;
    this.alpha.Value = Utility.Clamp(this.alpha.Value, 0.0f, 1f);
    this.rotation += this.rotationVelocity.Value;
    this.travelTime += time.ElapsedGameTime.Milliseconds;
    if ((double) this.scaleGrow.Value != 0.0)
      this.localScale += this.scaleGrow.Value;
    Vector2 vector2 = this.position.Value;
    this.updatePosition(time);
    this.updateTail(time);
    this.travelDistance += (vector2 - this.position.Value).Length();
    if (this.maxTravelDistance.Value >= 0)
    {
      if ((double) this.travelDistance > (double) (this.maxTravelDistance.Value - 128 /*0x80*/))
        this.alpha.Value = (float) (((double) this.maxTravelDistance.Value - (double) this.travelDistance) / 128.0);
      if ((double) this.travelDistance >= (double) this.maxTravelDistance.Value)
      {
        if (this.hasLit)
          Utility.removeLightSource(this.lightSourceId);
        return true;
      }
    }
    Character target;
    TerrainFeature terrainFeature;
    if ((this.travelTime > 100 || this.ignoreTravelGracePeriod.Value) && this.isColliding(location, out target, out terrainFeature) && this.ShouldApplyCollisionLocally(location))
    {
      if (this.bouncesLeft.Value > 0 && target == null)
      {
        --this.bouncesLeft.Value;
        bool[] flagArray = Utility.horizontalOrVerticalCollisionDirections(this.getBoundingBox(), this.theOneWhoFiredMe.Get(location), true);
        if (flagArray[0])
          this.xVelocity.Value = -this.xVelocity.Value;
        if (flagArray[1])
          this.yVelocity.Value = -this.yVelocity.Value;
        if (!string.IsNullOrEmpty(this.bounceSound.Value) && location != null)
          location.playSound(this.bounceSound.Value);
      }
      else
      {
        this.behaviorOnCollision(location, target, terrainFeature);
        return this.piercesLeft.Value <= 0;
      }
    }
    return false;
  }

  /// <summary>Get whether this projectile's <see cref="M:StardewValley.Projectiles.Projectile.behaviorOnCollision(StardewValley.GameLocation,StardewValley.Character,StardewValley.TerrainFeatures.TerrainFeature)" /> should be called for the local player.</summary>
  /// <param name="location">The location containing the projectile.</param>
  protected virtual bool ShouldApplyCollisionLocally(GameLocation location)
  {
    if (!(this.theOneWhoFiredMe.Get(location) is Farmer farmer) || farmer == Game1.player)
      return true;
    return Game1.IsMasterGame && farmer.currentLocation != location;
  }

  protected virtual void updateTail(GameTime time)
  {
    this.tailCounter -= time.ElapsedGameTime.Milliseconds;
    if (this.tailCounter > 0)
      return;
    this.tailCounter = 50;
    this.tail.Enqueue(this.position.Value);
    if (this.tail.Count <= this.tailLength.Value)
      return;
    this.tail.Dequeue();
  }

  /// <summary>Get whether the projectile is colliding with a wall or target.</summary>
  /// <param name="location">The location containing the projectile.</param>
  /// <param name="target">The target that was hit, if applicable.</param>
  /// <param name="terrainFeature">The terrain feature that was hit, if applicable.</param>
  public virtual bool isColliding(
    GameLocation location,
    out Character target,
    out TerrainFeature terrainFeature)
  {
    target = (Character) null;
    terrainFeature = (TerrainFeature) null;
    Rectangle boundingBox = this.getBoundingBox();
    if (!this.ignoreCharacterCollisions.Value)
    {
      if (this.damagesMonsters.Value)
      {
        Character character = (Character) location.doesPositionCollideWithCharacter(boundingBox);
        if (character != null)
        {
          if (character is NPC && (character as NPC).IsInvisible)
            return false;
          target = character;
          return true;
        }
      }
      else if (Game1.player.currentLocation == location && Game1.player.GetBoundingBox().Intersects(boundingBox))
      {
        target = (Character) Game1.player;
        return true;
      }
    }
    foreach (Vector2 key in Utility.getListOfTileLocationsForBordersOfNonTileRectangle(boundingBox))
    {
      TerrainFeature terrainFeature1;
      if (location.terrainFeatures.TryGetValue(key, out terrainFeature1) && !terrainFeature1.isPassable())
      {
        terrainFeature = terrainFeature1;
        return true;
      }
    }
    return !location.isTileOnMap(this.position.Value / 64f) || !this.ignoreLocationCollision.Value && location.isCollidingPosition(boundingBox, Game1.viewport, false, 0, true, this.theOneWhoFiredMe.Get(location), false, true);
  }

  public abstract void updatePosition(GameTime time);

  public virtual Rectangle getBoundingBox()
  {
    Vector2 vector2 = this.position.Value;
    int num = (int) ((double) (this.boundingBoxWidth.Value + (this.damagesMonsters.Value ? 8 : 0)) * (double) this.localScale);
    return new Rectangle((int) vector2.X + 32 /*0x20*/ - num / 2, (int) vector2.Y + 32 /*0x20*/ - num / 2, num, num);
  }

  public virtual void draw(SpriteBatch b)
  {
    float scale = 4f * this.localScale;
    Texture2D texture = this.GetTexture();
    Rectangle sourceRect = this.GetSourceRect();
    Vector2 vector2 = this.position.Value;
    b.Draw(texture, Game1.GlobalToLocal(Game1.viewport, vector2 + new Vector2(0.0f, -this.height.Value) + new Vector2(32f, 32f)), new Rectangle?(sourceRect), this.color.Value * this.alpha.Value, this.rotation, new Vector2(8f, 8f), scale, SpriteEffects.None, (float) (((double) vector2.Y + 96.0) / 10000.0));
    if ((double) this.height.Value > 0.0)
      b.Draw(Game1.shadowTexture, Game1.GlobalToLocal(Game1.viewport, vector2 + new Vector2(32f, 32f)), new Rectangle?(Game1.shadowTexture.Bounds), Color.White * this.alpha.Value * 0.75f, 0.0f, new Vector2((float) Game1.shadowTexture.Bounds.Center.X, (float) Game1.shadowTexture.Bounds.Center.Y), 2f, SpriteEffects.None, (float) (((double) vector2.Y - 1.0) / 10000.0));
    float num = this.alpha.Value;
    for (int index = this.tail.Count - 1; index >= 0; --index)
    {
      b.Draw(texture, Game1.GlobalToLocal(Game1.viewport, Vector2.Lerp(index == this.tail.Count - 1 ? vector2 : this.tail.ElementAt<Vector2>(index + 1), this.tail.ElementAt<Vector2>(index), (float) this.tailCounter / 50f) + new Vector2(0.0f, -this.height.Value) + new Vector2(32f, 32f)), new Rectangle?(sourceRect), this.color.Value * num, this.rotation, new Vector2(8f, 8f), scale, SpriteEffects.None, (float) (((double) vector2.Y - (double) (this.tail.Count - index) + 96.0) / 10000.0));
      num -= 1f / (float) this.tail.Count;
      scale = 0.8f * (float) (4 - 4 / (index + 4));
    }
  }

  /// <summary>Get the texture to draw for the projectile.</summary>
  public Texture2D GetTexture()
  {
    return this.itemId.Value == null ? Projectile.projectileSheet : ItemRegistry.GetDataOrErrorItem(this.itemId.Value).GetTexture();
  }

  /// <summary>Get the source rectangle to draw for the projectile.</summary>
  public Rectangle GetSourceRect()
  {
    if (this.itemId.Value == null)
      return Game1.getSourceRectForStandardTileSheet(Projectile.projectileSheet, this.currentTileSheetIndex.Value, 16 /*0x10*/, 16 /*0x10*/);
    ParsedItemData dataOrErrorItem = ItemRegistry.GetDataOrErrorItem(this.itemId.Value);
    string str = this.itemId.Value;
    if (str != null && str.Length == 6)
    {
      switch (str[5])
      {
        case '0':
          if (str == "(O)390" || str == "(O)380")
            break;
          goto label_10;
        case '2':
          if (str == "(O)382")
            break;
          goto label_10;
        case '4':
          if (str == "(O)384")
            break;
          goto label_10;
        case '6':
          if (!(str == "(O)386"))
            goto label_10;
          break;
        case '8':
          if (str == "(O)388" || str == "(O)378")
            break;
          goto label_10;
        default:
          goto label_10;
      }
      return dataOrErrorItem.GetSourceRect(1);
    }
label_10:
    return dataOrErrorItem.GetSourceRect();
  }
}
