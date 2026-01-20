// Decompiled with JetBrains decompiler
// Type: StardewValley.Characters.Horse
// Assembly: Stardew Valley, Version=1.6.15.24356, Culture=neutral, PublicKeyToken=null
// MVID: AA1F513A-94F0-4EF6-A35F-2D5B4A3721BF
// Assembly location: D:\SteamLibrary\steamapps\common\Stardew Valley\Stardew Valley.dll
// XML documentation location: D:\SteamLibrary\steamapps\common\Stardew Valley\Stardew Valley.xml

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Netcode;
using StardewValley.Buildings;
using StardewValley.Menus;
using StardewValley.Network;
using StardewValley.Objects;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml.Serialization;

#nullable disable
namespace StardewValley.Characters;

public class Horse : NPC
{
  private readonly NetGuid horseId = new NetGuid();
  private readonly NetFarmerRef netRider = new NetFarmerRef();
  public readonly NetLong ownerId = new NetLong();
  [XmlIgnore]
  public readonly NetBool mounting = new NetBool();
  [XmlIgnore]
  public readonly NetBool dismounting = new NetBool();
  private Vector2 dismountTile;
  private int ridingAnimationDirection;
  private bool roomForHorseAtDismountTile;
  [XmlElement("hat")]
  public readonly NetRef<Hat> hat = new NetRef<Hat>();
  public readonly NetMutex mutex = new NetMutex();
  [XmlIgnore]
  public Action<string> onFootstepAction;
  [XmlIgnore]
  public bool ateCarrotToday;
  private bool squeezingThroughGate;
  private int munchingCarrotTimer;

  public Guid HorseId
  {
    get => this.horseId.Value;
    set => this.horseId.Value = value;
  }

  [XmlIgnore]
  public Farmer rider
  {
    get => this.netRider.Value;
    set => this.netRider.Value = value;
  }

  /// <inheritdoc />
  [XmlIgnore]
  public override bool IsVillager => false;

  public Horse()
  {
    this.willDestroyObjectsUnderfoot = false;
    this.HideShadow = true;
    this.drawOffset = new Vector2(-16f, 0.0f);
    this.onFootstepAction = new Action<string>(this.PerformDefaultHorseFootstep);
    this.ChooseAppearance((LocalizedContentManager) null);
    this.faceDirection(3);
    this.Breather = false;
  }

  public Horse(Guid horseId, int xTile, int yTile)
    : this()
  {
    this.Name = "";
    this.displayName = this.Name;
    this.Position = new Vector2((float) xTile, (float) yTile) * 64f;
    this.currentLocation = Game1.currentLocation;
    this.HorseId = horseId;
  }

  public override void reloadData()
  {
  }

  protected override string translateName() => this.name.Value.Trim();

  public override bool canTalk() => false;

  protected override void initNetFields()
  {
    base.initNetFields();
    this.NetFields.AddField((INetSerializable) this.horseId, "horseId").AddField((INetSerializable) this.netRider.NetFields, "netRider.NetFields").AddField((INetSerializable) this.mounting, "mounting").AddField((INetSerializable) this.dismounting, "dismounting").AddField((INetSerializable) this.hat, "hat").AddField((INetSerializable) this.mutex.NetFields, "mutex.NetFields").AddField((INetSerializable) this.ownerId, "ownerId");
    this.position.Field.AxisAlignedMovement = false;
    this.facingDirection.fieldChangeEvent += (FieldChange<NetDirection, int>) ((_param1, _param2, _param3) => this.ClearCachedPosition());
  }

  public Farmer getOwner()
  {
    return this.ownerId.Value == 0L ? (Farmer) null : Game1.GetPlayer(this.ownerId.Value);
  }

  /// <inheritdoc />
  public override void reloadSprite(bool onlyAppearance = false)
  {
  }

  /// <inheritdoc />
  public override void ChooseAppearance(LocalizedContentManager content = null)
  {
    if (this.Sprite != null)
      return;
    this.Sprite = new AnimatedSprite("Animals\\horse", 0, 32 /*0x20*/, 32 /*0x20*/);
    this.Sprite.textureUsesFlippedRightForLeft = true;
    this.Sprite.loop = true;
  }

  public override void dayUpdate(int dayOfMonth)
  {
    this.ateCarrotToday = false;
    this.faceDirection(3);
  }

  public override Rectangle GetBoundingBox()
  {
    Rectangle boundingBox = base.GetBoundingBox();
    if (this.squeezingThroughGate && (this.FacingDirection == 0 || this.FacingDirection == 2))
      boundingBox.Inflate(-36, 0);
    return boundingBox;
  }

  public override bool canPassThroughActionTiles() => false;

  public void squeezeForGate()
  {
    if (!this.squeezingThroughGate)
    {
      this.squeezingThroughGate = true;
      this.ClearCachedPosition();
    }
    this.rider?.TemporaryPassableTiles.Add(this.GetBoundingBox());
  }

  public override void update(GameTime time, GameLocation location)
  {
    this.currentLocation = location;
    this.mutex.Update(location);
    if (this.squeezingThroughGate)
    {
      this.squeezingThroughGate = false;
      this.ClearCachedPosition();
    }
    this.faceTowardFarmer = false;
    this.faceTowardFarmerTimer = -1;
    this.Sprite.loop = this.rider != null && !this.rider.hidden.Value;
    if (this.rider != null && this.rider.hidden.Value)
      return;
    if (this.munchingCarrotTimer > 0)
    {
      this.munchingCarrotTimer -= (int) time.ElapsedGameTime.TotalMilliseconds;
      if (this.munchingCarrotTimer <= 0)
        this.mutex.ReleaseLock();
      base.update(time, location);
    }
    else
    {
      if (this.rider != null && this.rider.isAnimatingMount)
        this.rider.showNotCarrying();
      if (this.mounting.Value)
      {
        if (this.rider == null || !this.rider.IsLocalPlayer)
          return;
        if (this.rider.mount != null)
        {
          this.mounting.Value = false;
          this.rider.isAnimatingMount = false;
          this.rider = (Farmer) null;
          this.Halt();
          this.farmerPassesThrough = false;
          return;
        }
        Rectangle boundingBox = this.GetBoundingBox();
        int num = boundingBox.X + 16 /*0x10*/;
        if ((double) this.rider.Position.X < (double) (num - 4))
          this.rider.position.X += 4f;
        else if ((double) this.rider.Position.X > (double) (num + 4))
          this.rider.position.X -= 4f;
        int y = this.rider.StandingPixel.Y;
        if (y < boundingBox.Y - 4)
          this.rider.position.Y += 4f;
        else if (y > boundingBox.Y + 4)
          this.rider.position.Y -= 4f;
        if (this.rider.yJumpOffset >= -8 && (double) this.rider.yJumpVelocity <= 0.0)
        {
          this.Halt();
          this.Sprite.loop = true;
          this.currentLocation.characters.Remove((NPC) this);
          this.rider.mount = this;
          this.rider.freezePause = -1;
          this.mounting.Value = false;
          this.rider.isAnimatingMount = false;
          this.rider.canMove = true;
          if (this.FacingDirection == 1)
            this.rider.xOffset += 8f;
        }
      }
      else if (this.dismounting.Value)
      {
        if (this.rider == null || !this.rider.IsLocalPlayer)
        {
          this.Halt();
          return;
        }
        if (this.rider.isAnimatingMount)
          this.rider.faceDirection(this.FacingDirection);
        Vector2 vector2 = new Vector2((float) ((double) this.dismountTile.X * 64.0 + 32.0) - (float) (this.rider.GetBoundingBox().Width / 2), (float) ((double) this.dismountTile.Y * 64.0 + 4.0));
        if ((double) Math.Abs(this.rider.Position.X - vector2.X) > 4.0)
        {
          if ((double) this.rider.Position.X < (double) vector2.X)
            this.rider.position.X += Math.Min(4f, vector2.X - this.rider.Position.X);
          else if ((double) this.rider.Position.X > (double) vector2.X)
            this.rider.position.X += Math.Max(-4f, vector2.X - this.rider.Position.X);
        }
        if ((double) Math.Abs(this.rider.Position.Y - vector2.Y) > 4.0)
        {
          if ((double) this.rider.Position.Y < (double) vector2.Y)
            this.rider.position.Y += Math.Min(4f, vector2.Y - this.rider.Position.Y);
          else if ((double) this.rider.Position.Y > (double) vector2.Y)
            this.rider.position.Y += Math.Max(-4f, vector2.Y - this.rider.Position.Y);
        }
        if (this.rider.yJumpOffset >= 0 && (double) this.rider.yJumpVelocity <= 0.0)
        {
          this.rider.position.Y += 8f;
          this.rider.position.X = vector2.X;
          int num = 0;
          while (this.rider.currentLocation.isCollidingPosition(this.rider.GetBoundingBox(), Game1.viewport, true, 0, false, (Character) this.rider) && num < 6)
          {
            ++num;
            this.rider.position.Y -= 4f;
          }
          if (num == 6)
          {
            this.rider.Position = this.Position;
            this.dismounting.Value = false;
            this.rider.isAnimatingMount = false;
            this.rider.freezePause = -1;
            this.rider.canMove = true;
            return;
          }
          this.dismount();
        }
      }
      else if (this.rider == null && this.FacingDirection != 2 && this.Sprite.CurrentAnimation == null && Game1.random.NextDouble() < 0.002)
      {
        this.Sprite.loop = false;
        switch (this.FacingDirection)
        {
          case 0:
            this.Sprite.setCurrentAnimation(new List<FarmerSprite.AnimationFrame>()
            {
              new FarmerSprite.AnimationFrame(25, Game1.random.Next(250, 750)),
              new FarmerSprite.AnimationFrame(14, 10)
            });
            break;
          case 1:
            this.Sprite.setCurrentAnimation(new List<FarmerSprite.AnimationFrame>()
            {
              new FarmerSprite.AnimationFrame(21, 100),
              new FarmerSprite.AnimationFrame(22, 100),
              new FarmerSprite.AnimationFrame(23, 400),
              new FarmerSprite.AnimationFrame(24, 400),
              new FarmerSprite.AnimationFrame(23, 400),
              new FarmerSprite.AnimationFrame(24, 400),
              new FarmerSprite.AnimationFrame(23, 400),
              new FarmerSprite.AnimationFrame(24, 400),
              new FarmerSprite.AnimationFrame(23, 400),
              new FarmerSprite.AnimationFrame(22, 100),
              new FarmerSprite.AnimationFrame(21, 100)
            });
            break;
          case 3:
            this.Sprite.setCurrentAnimation(new List<FarmerSprite.AnimationFrame>()
            {
              new FarmerSprite.AnimationFrame(21, 100, false, true),
              new FarmerSprite.AnimationFrame(22, 100, false, true),
              new FarmerSprite.AnimationFrame(23, 100, false, true),
              new FarmerSprite.AnimationFrame(24, 400, false, true),
              new FarmerSprite.AnimationFrame(23, 400, false, true),
              new FarmerSprite.AnimationFrame(24, 400, false, true),
              new FarmerSprite.AnimationFrame(23, 400, false, true),
              new FarmerSprite.AnimationFrame(24, 400, false, true),
              new FarmerSprite.AnimationFrame(23, 400, false, true),
              new FarmerSprite.AnimationFrame(22, 100, false, true),
              new FarmerSprite.AnimationFrame(21, 100, false, true)
            });
            break;
        }
      }
      else if (this.rider != null)
      {
        if (this.FacingDirection != this.rider.FacingDirection || this.ridingAnimationDirection != this.FacingDirection)
        {
          this.Sprite.StopAnimation();
          this.faceDirection(this.rider.FacingDirection);
        }
        int num = !this.rider.movementDirections.Any<int>() || !this.rider.CanMove ? (this.rider.position.Field.IsInterpolating() ? 1 : 0) : 1;
        this.SyncPositionToRider();
        if (num == 0)
        {
          this.Sprite.StopAnimation();
          this.faceDirection(this.rider.FacingDirection);
        }
        else if (this.Sprite.CurrentAnimation == null)
        {
          switch (this.FacingDirection)
          {
            case 0:
              this.Sprite.setCurrentAnimation(new List<FarmerSprite.AnimationFrame>()
              {
                new FarmerSprite.AnimationFrame(15, 70),
                new FarmerSprite.AnimationFrame(16 /*0x10*/, 70, false, false, new AnimatedSprite.endOfAnimationBehavior(this.OnMountFootstep)),
                new FarmerSprite.AnimationFrame(17, 70, false, false, new AnimatedSprite.endOfAnimationBehavior(this.OnMountFootstep)),
                new FarmerSprite.AnimationFrame(18, 70, false, false, new AnimatedSprite.endOfAnimationBehavior(this.OnMountFootstep)),
                new FarmerSprite.AnimationFrame(19, 70),
                new FarmerSprite.AnimationFrame(20, 70)
              });
              break;
            case 1:
              this.Sprite.setCurrentAnimation(new List<FarmerSprite.AnimationFrame>()
              {
                new FarmerSprite.AnimationFrame(8, 70),
                new FarmerSprite.AnimationFrame(9, 70, false, false, new AnimatedSprite.endOfAnimationBehavior(this.OnMountFootstep)),
                new FarmerSprite.AnimationFrame(10, 70, false, false, new AnimatedSprite.endOfAnimationBehavior(this.OnMountFootstep)),
                new FarmerSprite.AnimationFrame(11, 70, false, false, new AnimatedSprite.endOfAnimationBehavior(this.OnMountFootstep)),
                new FarmerSprite.AnimationFrame(12, 70),
                new FarmerSprite.AnimationFrame(13, 70)
              });
              break;
            case 2:
              this.Sprite.setCurrentAnimation(new List<FarmerSprite.AnimationFrame>()
              {
                new FarmerSprite.AnimationFrame(1, 70),
                new FarmerSprite.AnimationFrame(2, 70, false, false, new AnimatedSprite.endOfAnimationBehavior(this.OnMountFootstep)),
                new FarmerSprite.AnimationFrame(3, 70, false, false, new AnimatedSprite.endOfAnimationBehavior(this.OnMountFootstep)),
                new FarmerSprite.AnimationFrame(4, 70, false, false, new AnimatedSprite.endOfAnimationBehavior(this.OnMountFootstep)),
                new FarmerSprite.AnimationFrame(5, 70),
                new FarmerSprite.AnimationFrame(6, 70)
              });
              break;
            case 3:
              this.Sprite.setCurrentAnimation(new List<FarmerSprite.AnimationFrame>()
              {
                new FarmerSprite.AnimationFrame(8, 70, false, true),
                new FarmerSprite.AnimationFrame(9, 70, false, true, new AnimatedSprite.endOfAnimationBehavior(this.OnMountFootstep)),
                new FarmerSprite.AnimationFrame(10, 70, false, true, new AnimatedSprite.endOfAnimationBehavior(this.OnMountFootstep)),
                new FarmerSprite.AnimationFrame(11, 70, false, true, new AnimatedSprite.endOfAnimationBehavior(this.OnMountFootstep)),
                new FarmerSprite.AnimationFrame(12, 70, false, true),
                new FarmerSprite.AnimationFrame(13, 70, false, true)
              });
              break;
          }
          this.ridingAnimationDirection = this.FacingDirection;
        }
      }
      if (this.FacingDirection == 3)
        this.drawOffset = Vector2.Zero;
      else
        this.drawOffset = new Vector2(-16f, 0.0f);
      this.flip = this.FacingDirection == 3;
      base.update(time, location);
    }
  }

  /// <inheritdoc />
  public override void OnLocationRemoved()
  {
    base.OnLocationRemoved();
    GameLocation location;
    Stable stable;
    if (!Game1.IsMasterGame || !this.TryFindStable(out location, out stable))
      return;
    Game1.warpCharacter((NPC) this, location, Utility.PointToVector2(stable.GetDefaultHorseTile()));
  }

  public virtual void OnMountFootstep(Farmer who)
  {
    if (this.onFootstepAction == null || this.rider == null)
      return;
    this.onFootstepAction(this.rider.currentLocation.doesTileHaveProperty(this.rider.TilePoint.X, this.rider.TilePoint.Y, "Type", "Back"));
  }

  public virtual void PerformDefaultHorseFootstep(string step_type)
  {
    if (this.rider == null)
      return;
    switch (step_type)
    {
      case "Stone":
        if (this.rider.ShouldHandleAnimationSound())
          this.rider.currentLocation.localSound("stoneStep", new Vector2?(this.Tile));
        if (this.rider != Game1.player)
          break;
        Rumble.rumble(0.1f, 50f);
        break;
      case "Wood":
        if (this.rider.ShouldHandleAnimationSound())
          this.rider.currentLocation.localSound("woodyStep", new Vector2?(this.Tile));
        if (this.rider != Game1.player)
          break;
        Rumble.rumble(0.1f, 50f);
        break;
      default:
        if (this.rider.ShouldHandleAnimationSound())
          this.rider.currentLocation.localSound("thudStep", new Vector2?(this.Tile));
        if (this.rider != Game1.player)
          break;
        Rumble.rumble(0.3f, 50f);
        break;
    }
  }

  public void dismount(bool from_demolish = false)
  {
    this.mutex.ReleaseLock();
    this.rider.mount = (Horse) null;
    if (this.currentLocation == null)
      return;
    if (!from_demolish && this.TryFindStable() != null && !this.currentLocation.characters.Any<NPC>((Func<NPC, bool>) (c => c is Horse horse && horse.HorseId == this.HorseId)))
      this.currentLocation.characters.Add((NPC) this);
    this.SyncPositionToRider();
    this.rider.TemporaryPassableTiles.Add(new Rectangle((int) this.dismountTile.X * 64 /*0x40*/, (int) this.dismountTile.Y * 64 /*0x40*/, 64 /*0x40*/, 64 /*0x40*/));
    this.rider.freezePause = -1;
    this.dismounting.Value = false;
    this.rider.isAnimatingMount = false;
    this.rider.canMove = true;
    this.rider.forceCanMove();
    this.rider.xOffset = 0.0f;
    this.rider = (Farmer) null;
    this.Halt();
    this.farmerPassesThrough = false;
  }

  /// <summary>Find the stable which this horse calls home, if it exists.</summary>
  public Stable TryFindStable()
  {
    Stable stable;
    return !this.TryFindStable(out GameLocation _, out stable) ? (Stable) null : stable;
  }

  /// <summary>Get the home location and stable for this horse, if available.</summary>
  /// <param name="location">The location containing its home stable, if found.</param>
  /// <param name="stable">Its home stable, if found.</param>
  public bool TryFindStable(out GameLocation location, out Stable stable)
  {
    GameLocation foundLocation = (GameLocation) null;
    Stable foundStable = (Stable) null;
    Utility.ForEachLocation((Func<GameLocation, bool>) (curLocation =>
    {
      foreach (Building building in curLocation.buildings)
      {
        if (building is Stable stable2 && stable2.HorseId == this.HorseId && !stable2.isUnderConstruction())
        {
          foundLocation = curLocation;
          foundStable = stable2;
          if (curLocation.IsActiveLocation())
            return false;
        }
      }
      return true;
    }));
    location = foundLocation;
    stable = foundStable;
    return stable != null;
  }

  public void nameHorse(string name)
  {
    if (name.Length <= 0)
      return;
    Game1.multiplayer.globalChatInfoMessage("HorseNamed", Game1.player.Name, name);
    Utility.ForEachVillager((Func<NPC, bool>) (n =>
    {
      if (n.Name == name)
        name += " ";
      return true;
    }));
    this.Name = name;
    this.displayName = name;
    if (Game1.player.horseName.Value == null)
      Game1.player.horseName.Value = name;
    Game1.exitActiveMenu();
    Game1.playSound("newArtifact");
    if (!this.mutex.IsLockHeld())
      return;
    this.mutex.ReleaseLock();
  }

  public override bool checkAction(Farmer who, GameLocation l)
  {
    if (who != null && !who.canMove || this.munchingCarrotTimer > 0)
      return false;
    if (this.rider == null)
    {
      this.mutex.RequestLock((Action) (() =>
      {
        if (who.mount != null || this.rider != null || who.FarmerSprite.PauseForSingleAnimation || this.currentLocation != who.currentLocation)
        {
          this.mutex.ReleaseLock();
        }
        else
        {
          Stable stable = this.TryFindStable();
          if (stable != null)
          {
            if ((this.getOwner() == Game1.player || this.getOwner() == null && (string.IsNullOrEmpty(Game1.player.horseName.Value) || Utility.findHorseForPlayer(Game1.player.UniqueMultiplayerID) == null)) && this.Name.Length <= 0)
            {
              stable.owner.Value = who.UniqueMultiplayerID;
              stable.updateHorseOwnership();
              Utility.ForEachBuilding<Stable>((Func<Stable, bool>) (curStable =>
              {
                if (curStable.owner.Value == who.UniqueMultiplayerID && curStable != stable)
                {
                  stable.owner.Value = 0L;
                  stable.updateHorseOwnership();
                }
                return true;
              }));
              if (string.IsNullOrEmpty(Game1.player.horseName.Value))
              {
                Game1.activeClickableMenu = (IClickableMenu) new NamingMenu(new NamingMenu.doneNamingBehavior(this.nameHorse), Game1.content.LoadString("Strings\\Characters:NameYourHorse"), Game1.content.LoadString("Strings\\Characters:DefaultHorseName"));
                return;
              }
            }
            else
            {
              if (who.Items.Count > who.CurrentToolIndex && who.Items[who.CurrentToolIndex] is Hat hat2)
              {
                if (this.hat.Value != null)
                {
                  Game1.createItemDebris((Item) this.hat.Value, this.Position, this.FacingDirection);
                  this.hat.Value = (Hat) null;
                }
                else
                {
                  who.Items[who.CurrentToolIndex] = (Item) null;
                  this.hat.Value = hat2;
                  Game1.playSound("dirtyHit");
                }
                this.mutex.ReleaseLock();
                return;
              }
              if (!this.ateCarrotToday && who.Items.Count > who.CurrentToolIndex && who.Items[who.CurrentToolIndex] is StardewValley.Object object2 && object2.QualifiedItemId == "(O)Carrot")
              {
                this.Sprite.StopAnimation();
                this.Sprite.faceDirection(this.FacingDirection);
                Game1.playSound("eat");
                DelayedAction.playSoundAfterDelay("eat", 600);
                DelayedAction.playSoundAfterDelay("eat", 1200);
                this.munchingCarrotTimer = 1500;
                this.doEmote(20, 32 /*0x20*/);
                who.reduceActiveItemByOne();
                this.ateCarrotToday = true;
                return;
              }
            }
          }
          this.rider = who;
          this.rider.freezePause = 5000;
          this.rider.synchronizedJump(6f);
          this.rider.Halt();
          if ((double) this.rider.Position.X < (double) this.Position.X)
            this.rider.faceDirection(1);
          l.playSound("dwop");
          this.mounting.Value = true;
          this.rider.isAnimatingMount = true;
          this.rider.completelyStopAnimatingOrDoingAction();
          this.rider.faceGeneralDirection(Utility.PointToVector2(this.StandingPixel), 0, false, false);
        }
      }));
      return true;
    }
    this.dismounting.Value = true;
    this.rider.isAnimatingMount = true;
    this.farmerPassesThrough = false;
    this.rider.TemporaryPassableTiles.Clear();
    Vector2 tileForCharacter = Utility.recursiveFindOpenTileForCharacter((Character) this.rider, this.rider.currentLocation, this.Tile, 8);
    this.Position = new Vector2((float) ((double) tileForCharacter.X * 64.0 + 32.0) - (float) (this.GetBoundingBox().Width / 2), (float) ((double) tileForCharacter.Y * 64.0 + 4.0));
    this.roomForHorseAtDismountTile = !this.currentLocation.isCollidingPosition(this.GetBoundingBox(), Game1.viewport, true, 0, false, (Character) this);
    this.Position = this.rider.Position;
    this.dismounting.Value = false;
    this.rider.isAnimatingMount = false;
    this.Halt();
    if (!tileForCharacter.Equals(Vector2.Zero) && (double) Vector2.Distance(tileForCharacter, this.Tile) < 2.0)
    {
      this.rider.synchronizedJump(6f);
      l.playSound("dwop");
      this.rider.freezePause = 5000;
      this.rider.Halt();
      this.rider.xOffset = 0.0f;
      this.dismounting.Value = true;
      this.rider.isAnimatingMount = true;
      this.dismountTile = tileForCharacter;
    }
    else
      this.dismount();
    return true;
  }

  public void SyncPositionToRider()
  {
    if (this.rider == null || this.dismounting.Value && !this.roomForHorseAtDismountTile)
      return;
    this.Position = this.rider.Position;
  }

  public override void draw(SpriteBatch b)
  {
    this.flip = this.FacingDirection == 3;
    this.Sprite.UpdateSourceRect();
    base.draw(b);
    if (this.FacingDirection == 2 && this.rider != null)
      b.Draw(this.Sprite.Texture, this.getLocalPosition(Game1.viewport) + new Vector2(48f, -24f - this.rider.yOffset), new Rectangle?(new Rectangle(160 /*0xA0*/, 96 /*0x60*/, 9, 15)), Color.White, 0.0f, Vector2.Zero, 4f, SpriteEffects.None, (float) (((double) this.Position.Y + 64.0) / 10000.0));
    bool flag = true;
    if (this.hat.Value != null)
    {
      Vector2 zero = Vector2.Zero;
      string itemId = this.hat.Value.ItemId;
      if (itemId != null)
      {
        switch (itemId.Length)
        {
          case 1:
            switch (itemId[0])
            {
              case '6':
                zero.Y += 2f;
                if (this.FacingDirection == 2)
                {
                  --zero.Y;
                  goto label_33;
                }
                goto label_33;
              case '9':
                break;
              default:
                goto label_33;
            }
            break;
          case 2:
            switch (itemId[1])
            {
              case '0':
                if (itemId == "10")
                {
                  zero.Y += 3f;
                  if (this.FacingDirection == 0)
                  {
                    flag = false;
                    goto label_33;
                  }
                  goto label_33;
                }
                goto label_33;
              case '1':
                switch (itemId)
                {
                  case "31":
                    ++zero.Y;
                    goto label_33;
                  case "11":
                    break;
                  default:
                    goto label_33;
                }
                break;
              case '2':
                if (itemId == "32")
                  goto label_20;
                goto label_33;
              case '4':
                if (itemId == "14" && this.FacingDirection == 0)
                {
                  zero.X = -100f;
                  goto label_33;
                }
                goto label_33;
              case '6':
                switch (itemId)
                {
                  case "26":
                    if (this.FacingDirection == 3 || this.FacingDirection == 1)
                    {
                      if (this.flip)
                      {
                        ++zero.X;
                        goto label_33;
                      }
                      --zero.X;
                      goto label_33;
                    }
                    goto label_33;
                  case "56":
                    goto label_31;
                  default:
                    goto label_33;
                }
              case '7':
                if (itemId == "67")
                  goto label_31;
                goto label_33;
              case '9':
                if (itemId == "39")
                  break;
                goto label_33;
              default:
                goto label_33;
            }
            if (this.FacingDirection == 3 || this.FacingDirection == 1)
            {
              if (this.flip)
              {
                zero.X += 2f;
                goto label_33;
              }
              zero.X -= 2f;
              goto label_33;
            }
            goto label_33;
label_31:
            if (this.FacingDirection == 0)
            {
              flag = false;
              goto label_33;
            }
            goto label_33;
          default:
            goto label_33;
        }
label_20:
        if (this.FacingDirection == 0 || this.FacingDirection == 2)
          ++zero.Y;
      }
label_33:
      zero *= 4f;
      if (this.shakeTimer > 0)
        zero += new Vector2((float) Game1.random.Next(-1, 2), (float) Game1.random.Next(-1, 2));
      if ((double) zero.X <= -100.0)
        return;
      float num = (float) this.StandingPixel.Y / 10000f;
      if (this.rider != null)
      {
        if (this.FacingDirection == 2)
          num = (float) (((double) this.position.Y + 64.0 + 1.0) / 10000.0);
        else if (this.FacingDirection != 0)
          num = (float) (((double) this.position.Y + 48.0 - 1.0) / 10000.0);
      }
      if (this.munchingCarrotTimer > 0)
      {
        switch (this.FacingDirection)
        {
          case 1:
            b.Draw(this.Sprite.Texture, this.getLocalPosition(Game1.viewport) + new Vector2(80f, -56f), new Rectangle?(new Rectangle(179 + (int) (Game1.currentGameTime.TotalGameTime.TotalMilliseconds % 600.0) / 300 * 16 /*0x10*/, 97, 16 /*0x10*/, 14)), Color.White, 0.0f, Vector2.Zero, 4f, SpriteEffects.None, num + 1E-07f);
            break;
          case 2:
            b.Draw(this.Sprite.Texture, this.getLocalPosition(Game1.viewport) + new Vector2(24f, -24f), new Rectangle?(new Rectangle(170 + (int) (Game1.currentGameTime.TotalGameTime.TotalMilliseconds % 600.0) / 300 * 16 /*0x10*/, 112 /*0x70*/, 16 /*0x10*/, 16 /*0x10*/)), Color.White, 0.0f, Vector2.Zero, 4f, SpriteEffects.None, num + 1E-07f);
            break;
          case 3:
            b.Draw(this.Sprite.Texture, this.getLocalPosition(Game1.viewport) + new Vector2(-16f, -56f), new Rectangle?(new Rectangle(179 + (int) (Game1.currentGameTime.TotalGameTime.TotalMilliseconds % 600.0) / 300 * 16 /*0x10*/, 97, 16 /*0x10*/, 14)), Color.White, 0.0f, Vector2.Zero, 4f, SpriteEffects.FlipHorizontally, num + 1E-07f);
            break;
        }
      }
      if (!flag)
        return;
      float layerDepth = num + 2E-07f;
      switch (this.Sprite.CurrentFrame)
      {
        case 0:
        case 1:
        case 2:
        case 3:
        case 4:
        case 5:
        case 6:
          this.hat.Value.draw(b, Utility.snapDrawPosition(this.getLocalPosition(Game1.viewport) + zero + new Vector2(30f, (float) (-42.0 - (this.rider != null ? (double) this.rider.yOffset : 0.0)))), 1.33333337f, 1f, layerDepth, 2);
          break;
        case 7:
        case 11:
          if (this.flip)
          {
            this.hat.Value.draw(b, this.getLocalPosition(Game1.viewport) + zero + new Vector2(-14f, -74f), 1.33333337f, 1f, layerDepth, 3);
            break;
          }
          this.hat.Value.draw(b, Utility.snapDrawPosition(this.getLocalPosition(Game1.viewport) + zero + new Vector2(66f, -74f)), 1.33333337f, 1f, layerDepth, 1);
          break;
        case 8:
          if (this.flip)
          {
            this.hat.Value.draw(b, this.getLocalPosition(Game1.viewport) + zero + new Vector2(-18f, -74f), 1.33333337f, 1f, layerDepth, 3);
            break;
          }
          this.hat.Value.draw(b, Utility.snapDrawPosition(this.getLocalPosition(Game1.viewport) + zero + new Vector2(70f, -74f)), 1.33333337f, 1f, layerDepth, 1);
          break;
        case 9:
          if (this.flip)
          {
            this.hat.Value.draw(b, this.getLocalPosition(Game1.viewport) + zero + new Vector2(-18f, -70f), 1.33333337f, 1f, layerDepth, 3);
            break;
          }
          this.hat.Value.draw(b, Utility.snapDrawPosition(this.getLocalPosition(Game1.viewport) + zero + new Vector2(70f, -70f)), 1.33333337f, 1f, layerDepth, 1);
          break;
        case 10:
          if (this.flip)
          {
            this.hat.Value.draw(b, this.getLocalPosition(Game1.viewport) + zero + new Vector2(-14f, -70f), 1.33333337f, 1f, layerDepth, 3);
            break;
          }
          this.hat.Value.draw(b, Utility.snapDrawPosition(this.getLocalPosition(Game1.viewport) + zero + new Vector2(66f, -70f)), 1.33333337f, 1f, layerDepth, 1);
          break;
        case 12:
          if (this.flip)
          {
            this.hat.Value.draw(b, Utility.snapDrawPosition(this.getLocalPosition(Game1.viewport) + zero + new Vector2(-14f, -78f)), 1.33333337f, 1f, layerDepth, 3);
            break;
          }
          this.hat.Value.draw(b, Utility.snapDrawPosition(this.getLocalPosition(Game1.viewport) + zero + new Vector2(66f, -78f)), 1.33333337f, 1f, layerDepth, 1);
          break;
        case 13:
          if (this.flip)
          {
            this.hat.Value.draw(b, Utility.snapDrawPosition(this.getLocalPosition(Game1.viewport) + zero + new Vector2(-18f, -78f)), 1.33333337f, 1f, layerDepth, 3);
            break;
          }
          this.hat.Value.draw(b, Utility.snapDrawPosition(this.getLocalPosition(Game1.viewport) + zero + new Vector2(70f, -78f)), 1.33333337f, 1f, layerDepth, 1);
          break;
        case 14:
        case 15:
        case 16 /*0x10*/:
        case 17:
        case 18:
        case 19:
        case 20:
        case 25:
          this.hat.Value.draw(b, this.getLocalPosition(Game1.viewport) + zero + new Vector2(28f, (float) (-106.0 - (this.rider != null ? (double) this.rider.yOffset : 0.0))), 1.33333337f, 1f, layerDepth, 0);
          break;
        case 21:
          if (this.flip)
          {
            this.hat.Value.draw(b, Utility.snapDrawPosition(this.getLocalPosition(Game1.viewport) + zero + new Vector2(-14f, -66f)), 1.33333337f, 1f, layerDepth, 3);
            break;
          }
          this.hat.Value.draw(b, Utility.snapDrawPosition(this.getLocalPosition(Game1.viewport) + zero + new Vector2(66f, -66f)), 1.33333337f, 1f, layerDepth, 1);
          break;
        case 22:
          if (this.flip)
          {
            this.hat.Value.draw(b, Utility.snapDrawPosition(this.getLocalPosition(Game1.viewport) + zero + new Vector2(-18f, -54f)), 1.33333337f, 1f, layerDepth, 3);
            break;
          }
          this.hat.Value.draw(b, Utility.snapDrawPosition(this.getLocalPosition(Game1.viewport) + zero + new Vector2(70f, -54f)), 1.33333337f, 1f, layerDepth, 1);
          break;
        case 23:
          if (this.flip)
          {
            this.hat.Value.draw(b, Utility.snapDrawPosition(this.getLocalPosition(Game1.viewport) + zero + new Vector2(-18f, -42f)), 1.33333337f, 1f, layerDepth, 3);
            break;
          }
          this.hat.Value.draw(b, Utility.snapDrawPosition(this.getLocalPosition(Game1.viewport) + zero + new Vector2(70f, -42f)), 1.33333337f, 1f, layerDepth, 1);
          break;
        case 24:
          if (this.flip)
          {
            this.hat.Value.draw(b, Utility.snapDrawPosition(this.getLocalPosition(Game1.viewport) + zero + new Vector2(-18f, -42f)), 1.33333337f, 1f, layerDepth, 3);
            break;
          }
          this.hat.Value.draw(b, Utility.snapDrawPosition(this.getLocalPosition(Game1.viewport) + zero + new Vector2(70f, -42f)), 1.33333337f, 1f, layerDepth, 1);
          break;
      }
    }
    else
    {
      if (this.munchingCarrotTimer <= 0)
        return;
      switch (this.FacingDirection)
      {
        case 1:
          b.Draw(this.Sprite.Texture, this.getLocalPosition(Game1.viewport) + new Vector2(80f, -56f), new Rectangle?(new Rectangle(179 + (int) (Game1.currentGameTime.TotalGameTime.TotalMilliseconds % 600.0) / 300 * 16 /*0x10*/, 97, 16 /*0x10*/, 14)), Color.White, 0.0f, Vector2.Zero, 4f, SpriteEffects.None, (float) (((double) this.Position.Y + 64.0) / 10000.0 + 1.0000000116860974E-07));
          break;
        case 2:
          b.Draw(this.Sprite.Texture, this.getLocalPosition(Game1.viewport) + new Vector2(24f, -24f), new Rectangle?(new Rectangle(170 + (int) (Game1.currentGameTime.TotalGameTime.TotalMilliseconds % 600.0) / 300 * 16 /*0x10*/, 112 /*0x70*/, 16 /*0x10*/, 16 /*0x10*/)), Color.White, 0.0f, Vector2.Zero, 4f, SpriteEffects.None, (float) (((double) this.Position.Y + 64.0) / 10000.0 + 1.0000000116860974E-07));
          break;
        case 3:
          b.Draw(this.Sprite.Texture, this.getLocalPosition(Game1.viewport) + new Vector2(-16f, -56f), new Rectangle?(new Rectangle(179 + (int) (Game1.currentGameTime.TotalGameTime.TotalMilliseconds % 600.0) / 300 * 16 /*0x10*/, 97, 16 /*0x10*/, 14)), Color.White, 0.0f, Vector2.Zero, 4f, SpriteEffects.FlipHorizontally, (float) (((double) this.Position.Y + 64.0) / 10000.0 + 1.0000000116860974E-07));
          break;
      }
    }
  }
}
