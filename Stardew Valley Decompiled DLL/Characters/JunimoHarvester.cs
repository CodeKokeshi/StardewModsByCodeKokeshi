// Decompiled with JetBrains decompiler
// Type: StardewValley.Characters.JunimoHarvester
// Assembly: Stardew Valley, Version=1.6.15.24356, Culture=neutral, PublicKeyToken=null
// MVID: AA1F513A-94F0-4EF6-A35F-2D5B4A3721BF
// Assembly location: D:\SteamLibrary\steamapps\common\Stardew Valley\Stardew Valley.dll
// XML documentation location: D:\SteamLibrary\steamapps\common\Stardew Valley\Stardew Valley.xml

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Netcode;
using StardewValley.Buildings;
using StardewValley.Extensions;
using StardewValley.ItemTypeDefinitions;
using StardewValley.Objects;
using StardewValley.Pathfinding;
using StardewValley.TerrainFeatures;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml.Serialization;

#nullable disable
namespace StardewValley.Characters;

public class JunimoHarvester : NPC
{
  protected float alpha = 1f;
  protected float alphaChange;
  protected Vector2 motion = Vector2.Zero;
  protected Rectangle nextPosition;
  protected readonly NetColor color = new NetColor();
  protected bool destroy;
  protected Item lastItemHarvested;
  public int whichJunimoFromThisHut;
  protected int harvestTimer;
  public readonly NetBool isPrismatic = new NetBool(false);
  protected readonly NetGuid netHome = new NetGuid();
  protected readonly NetEvent1Field<int, NetInt> netAnimationEvent = new NetEvent1Field<int, NetInt>();

  public Guid HomeId
  {
    get => this.netHome.Value;
    set => this.netHome.Value = value;
  }

  [XmlIgnore]
  public JunimoHut home
  {
    get
    {
      Building building;
      return !this.currentLocation.buildings.TryGetValue(this.netHome.Value, out building) ? (JunimoHut) null : building as JunimoHut;
    }
    set => this.netHome.Value = this.currentLocation.buildings.GuidOf((Building) value);
  }

  /// <inheritdoc />
  [XmlIgnore]
  public override bool IsVillager => false;

  public JunimoHarvester()
  {
  }

  public JunimoHarvester(
    GameLocation location,
    Vector2 position,
    JunimoHut hut,
    int whichJunimoNumberFromThisHut,
    Color? c)
    : base(new AnimatedSprite("Characters\\Junimo", 0, 16 /*0x10*/, 16 /*0x10*/), position, 2, "Junimo")
  {
    this.currentLocation = location;
    this.home = hut;
    this.whichJunimoFromThisHut = whichJunimoNumberFromThisHut;
    if (!c.HasValue)
      this.pickColor();
    else
      this.color.Value = c.Value;
    this.nextPosition = this.GetBoundingBox();
    this.Breather = false;
    this.speed = 3;
    this.forceUpdateTimer = 9999;
    this.collidesWithOtherCharacters.Value = true;
    this.ignoreMovementAnimation = true;
    this.farmerPassesThrough = true;
    this.Scale = 0.75f;
    this.willDestroyObjectsUnderfoot = false;
    Vector2 v = Vector2.Zero;
    switch (whichJunimoNumberFromThisHut)
    {
      case 0:
        v = Utility.recursiveFindOpenTileForCharacter((Character) this, this.currentLocation, new Vector2((float) (hut.tileX.Value + 1), (float) (hut.tileY.Value + hut.tilesHigh.Value + 1)), 30);
        break;
      case 1:
        v = Utility.recursiveFindOpenTileForCharacter((Character) this, this.currentLocation, new Vector2((float) (hut.tileX.Value - 1), (float) hut.tileY.Value), 30);
        break;
      case 2:
        v = Utility.recursiveFindOpenTileForCharacter((Character) this, this.currentLocation, new Vector2((float) (hut.tileX.Value + hut.tilesWide.Value), (float) hut.tileY.Value), 30);
        break;
    }
    if (v != Vector2.Zero)
      this.controller = new PathFindController((Character) this, this.currentLocation, Utility.Vector2ToPoint(v), -1, new PathFindController.endBehavior(this.reachFirstDestinationFromHut), 100);
    if (this.controller?.pathToEndPoint == null && Game1.IsMasterGame)
    {
      this.pathfindToRandomSpotAroundHut();
      if (this.controller?.pathToEndPoint == null)
        this.destroy = true;
    }
    this.collidesWithOtherCharacters.Value = false;
  }

  protected virtual void pickColor()
  {
    JunimoHut home = this.home;
    if (home == null)
    {
      this.color.Value = Color.White;
    }
    else
    {
      Random random = Utility.CreateRandom((double) home.tileX.Value, (double) home.tileY.Value * 777.0, (double) this.whichJunimoFromThisHut);
      if (random.NextBool(0.25))
      {
        if (random.NextBool(0.01))
        {
          this.color.Value = Color.White;
        }
        else
        {
          switch (random.Next(8))
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
            default:
              this.color.Value = Color.Salmon;
              break;
          }
        }
      }
      else
      {
        switch (random.Next(8))
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
          default:
            this.color.Value = Color.Turquoise;
            break;
        }
      }
    }
  }

  protected override void initNetFields()
  {
    base.initNetFields();
    this.NetFields.AddField((INetSerializable) this.color, "color").AddField(this.netHome.NetFields, "netHome.NetFields").AddField((INetSerializable) this.netAnimationEvent, "netAnimationEvent").AddField((INetSerializable) this.isPrismatic, "isPrismatic");
    this.netAnimationEvent.onEvent += new AbstractNetEvent1<int>.Event(this.doAnimationEvent);
  }

  /// <inheritdoc />
  public override void ChooseAppearance(LocalizedContentManager content = null)
  {
    if (this.Sprite != null)
      return;
    this.Sprite = new AnimatedSprite((ContentManager) (content ?? Game1.content), "Characters\\Junimo");
  }

  protected virtual void doAnimationEvent(int animId)
  {
    switch (animId)
    {
      case 0:
        this.Sprite.CurrentAnimation = (List<FarmerSprite.AnimationFrame>) null;
        break;
      case 2:
        this.Sprite.currentFrame = 0;
        break;
      case 3:
        this.Sprite.currentFrame = 1;
        break;
      case 4:
        this.Sprite.currentFrame = 2;
        break;
      case 5:
        this.Sprite.currentFrame = 44;
        break;
      case 6:
        this.Sprite.setCurrentAnimation(new List<FarmerSprite.AnimationFrame>()
        {
          new FarmerSprite.AnimationFrame(12, 200),
          new FarmerSprite.AnimationFrame(13, 200),
          new FarmerSprite.AnimationFrame(14, 200),
          new FarmerSprite.AnimationFrame(15, 200)
        });
        break;
      case 7:
        this.Sprite.setCurrentAnimation(new List<FarmerSprite.AnimationFrame>()
        {
          new FarmerSprite.AnimationFrame(44, 200),
          new FarmerSprite.AnimationFrame(45, 200),
          new FarmerSprite.AnimationFrame(46, 200),
          new FarmerSprite.AnimationFrame(47, 200)
        });
        break;
      case 8:
        this.Sprite.setCurrentAnimation(new List<FarmerSprite.AnimationFrame>()
        {
          new FarmerSprite.AnimationFrame(28, 100),
          new FarmerSprite.AnimationFrame(29, 100),
          new FarmerSprite.AnimationFrame(30, 100),
          new FarmerSprite.AnimationFrame(31 /*0x1F*/, 100)
        });
        break;
    }
  }

  public virtual void reachFirstDestinationFromHut(Character c, GameLocation l)
  {
    this.tryToHarvestHere();
  }

  public virtual void tryToHarvestHere()
  {
    if (this.currentLocation == null)
      return;
    if (this.isHarvestable())
      this.harvestTimer = 2000;
    else
      this.pokeToHarvest();
  }

  public virtual void pokeToHarvest()
  {
    JunimoHut home = this.home;
    if (home == null)
      return;
    if (!home.isTilePassable(this.Tile) && Game1.IsMasterGame)
    {
      this.destroy = true;
    }
    else
    {
      if (this.harvestTimer > 0 || Game1.random.NextDouble() >= 0.7)
        return;
      this.pathfindToNewCrop();
    }
  }

  public override bool shouldCollideWithBuildingLayer(GameLocation location) => true;

  public void setMoving(int xSpeed, int ySpeed)
  {
    this.motion.X = (float) xSpeed;
    this.motion.Y = (float) ySpeed;
  }

  public void setMoving(Vector2 motion) => this.motion = motion;

  public override void Halt()
  {
    base.Halt();
    this.motion = Vector2.Zero;
  }

  public override bool canTalk() => false;

  public void junimoReachedHut(Character c, GameLocation l)
  {
    this.controller = (PathFindController) null;
    this.motion.X = 0.0f;
    this.motion.Y = -1f;
    this.destroy = true;
  }

  public virtual bool foundCropEndFunction(
    PathNode currentNode,
    Point endPoint,
    GameLocation location,
    Character c)
  {
    TerrainFeature terrainFeature;
    return location.terrainFeatures.TryGetValue(new Vector2((float) currentNode.x, (float) currentNode.y), out terrainFeature) && (location.isCropAtTile(currentNode.x, currentNode.y) && (terrainFeature as HoeDirt).readyForHarvest() || terrainFeature is Bush bush && bush.readyForHarvest());
  }

  public virtual void pathfindToNewCrop()
  {
    JunimoHut home = this.home;
    if (home == null)
      return;
    if (Game1.timeOfDay > 1900)
    {
      if (this.controller != null)
        return;
      this.returnToJunimoHut(this.currentLocation);
    }
    else if (Game1.random.NextDouble() < 0.035 || home.noHarvest.Value)
    {
      this.pathfindToRandomSpotAroundHut();
    }
    else
    {
      this.controller = new PathFindController((Character) this, this.currentLocation, new PathFindController.isAtEnd(this.foundCropEndFunction), -1, new PathFindController.endBehavior(this.reachFirstDestinationFromHut), 100, Point.Zero);
      Stack<Point> pathToEndPoint = this.controller.pathToEndPoint;
      Point? nullable = pathToEndPoint != null ? new Point?(pathToEndPoint.Last<Point>()) : new Point?();
      if (!nullable.HasValue || Math.Abs(nullable.Value.X - (home.tileX.Value + 1)) > home.cropHarvestRadius || Math.Abs(nullable.Value.Y - (home.tileY.Value + 1)) > home.cropHarvestRadius)
      {
        if (Game1.random.NextBool() && !home.lastKnownCropLocation.Equals(Point.Zero))
          this.controller = new PathFindController((Character) this, this.currentLocation, home.lastKnownCropLocation, -1, new PathFindController.endBehavior(this.reachFirstDestinationFromHut), 100);
        else if (Game1.random.NextDouble() < 0.25)
        {
          this.netAnimationEvent.Fire(0);
          this.returnToJunimoHut(this.currentLocation);
        }
        else
          this.pathfindToRandomSpotAroundHut();
      }
      else
        this.netAnimationEvent.Fire(0);
    }
  }

  public virtual void returnToJunimoHut(GameLocation location)
  {
    if (Utility.isOnScreen(Utility.Vector2ToPoint(this.position.Value / 64f), 64 /*0x40*/, this.currentLocation))
      this.jump();
    this.collidesWithOtherCharacters.Value = false;
    if (Game1.IsMasterGame)
    {
      JunimoHut home = this.home;
      if (home == null)
        return;
      this.controller = new PathFindController((Character) this, location, new Point(home.tileX.Value + 1, home.tileY.Value + 1), 0, new PathFindController.endBehavior(this.junimoReachedHut));
      if (this.controller.pathToEndPoint == null || this.controller.pathToEndPoint.Count == 0 || location.isCollidingPosition(this.nextPosition, Game1.viewport, false, 0, false, (Character) this))
        this.destroy = true;
    }
    if (!Utility.isOnScreen(Utility.Vector2ToPoint(this.position.Value / 64f), 64 /*0x40*/, this.currentLocation))
      return;
    location.playSound("junimoMeep1");
  }

  public override void faceDirection(int direction)
  {
  }

  protected override void updateSlaveAnimation(GameTime time)
  {
  }

  protected virtual bool isHarvestable()
  {
    TerrainFeature terrainFeature;
    if (this.currentLocation.terrainFeatures.TryGetValue(this.Tile, out terrainFeature))
    {
      switch (terrainFeature)
      {
        case HoeDirt hoeDirt:
          return hoeDirt.readyForHarvest();
        case Bush bush:
          return bush.readyForHarvest();
      }
    }
    return false;
  }

  public override void update(GameTime time, GameLocation location)
  {
    this.netAnimationEvent.Poll();
    base.update(time, location);
    if (this.isPrismatic.Value)
      this.color.Value = Utility.GetPrismaticColor(this.whichJunimoFromThisHut);
    this.forceUpdateTimer = 99999;
    if (this.EventActor)
      return;
    if (this.destroy)
      this.alphaChange = -0.05f;
    this.alpha += this.alphaChange;
    if ((double) this.alpha > 1.0)
      this.alpha = 1f;
    else if ((double) this.alpha < 0.0)
    {
      this.alpha = 0.0f;
      if (this.destroy && Game1.IsMasterGame)
      {
        location.characters.Remove((NPC) this);
        this.home?.myJunimos.Remove(this);
      }
    }
    if (Game1.IsMasterGame)
    {
      if (this.harvestTimer > 0)
      {
        int harvestTimer = this.harvestTimer;
        this.harvestTimer -= time.ElapsedGameTime.Milliseconds;
        if (this.harvestTimer > 1800)
          this.netAnimationEvent.Fire(2);
        else if (this.harvestTimer > 1600)
          this.netAnimationEvent.Fire(3);
        else if (this.harvestTimer > 1000)
        {
          this.netAnimationEvent.Fire(4);
          this.shake(50);
        }
        else if (harvestTimer >= 1000 && this.harvestTimer < 1000)
        {
          this.netAnimationEvent.Fire(2);
          JunimoHut home = this.home;
          if (this.currentLocation != null && home != null && !home.noHarvest.Value && this.isHarvestable())
          {
            this.netAnimationEvent.Fire(5);
            this.lastItemHarvested = (Item) null;
            TerrainFeature terrainFeature = this.currentLocation.terrainFeatures[this.Tile];
            if (!(terrainFeature is Bush bush))
            {
              if (terrainFeature is HoeDirt soil && soil.crop.harvest(this.TilePoint.X, this.TilePoint.Y, soil, this))
                soil.destroyCrop(this.currentLocation.farmers.Any());
            }
            else if (bush.readyForHarvest())
            {
              this.tryToAddItemToHut(ItemRegistry.Create("(O)815"));
              bush.tileSheetOffset.Value = 0;
              bush.setUpSourceRect();
              if (Utility.isOnScreen(this.TilePoint, 64 /*0x40*/, this.currentLocation))
                bush.performUseAction(this.Tile);
              if (Utility.isOnScreen(this.TilePoint, 64 /*0x40*/, this.currentLocation))
                DelayedAction.playSoundAfterDelay("coin", 260, this.currentLocation);
            }
            if (this.lastItemHarvested != null)
            {
              bool flag = false;
              if (this.home.raisinDays.Value > 0 && Game1.random.NextDouble() < 0.2)
              {
                flag = true;
                Item one = this.lastItemHarvested.getOne();
                one.Quality = this.lastItemHarvested.Quality;
                this.tryToAddItemToHut(one);
              }
              if (this.currentLocation.farmers.Any())
              {
                ParsedItemData dataOrErrorItem = ItemRegistry.GetDataOrErrorItem(this.lastItemHarvested.QualifiedItemId);
                float layerDepth = (float) ((double) this.StandingPixel.Y / 10000.0 + 0.0099999997764825821);
                if (flag)
                {
                  for (int index = 0; index < 2; ++index)
                  {
                    Game1.multiplayer.broadcastSprites(this.currentLocation, new TemporaryAnimatedSprite(dataOrErrorItem.TextureName, dataOrErrorItem.GetSourceRect(), 1000f, 1, 0, this.Position + new Vector2(0.0f, -40f), false, false, layerDepth, 0.02f, Color.White, 4f, -0.01f, 0.0f, 0.0f)
                    {
                      motion = new Vector2((index == 0 ? -1f : 1f) * 0.5f, -0.25f),
                      delayBeforeAnimationStart = 200
                    });
                    if (this.lastItemHarvested is ColoredObject lastItemHarvested)
                    {
                      Rectangle sourceRect = ItemRegistry.GetDataOrErrorItem(this.lastItemHarvested.QualifiedItemId).GetSourceRect(1);
                      Game1.multiplayer.broadcastSprites(this.currentLocation, new TemporaryAnimatedSprite(dataOrErrorItem.TextureName, sourceRect, 1000f, 1, 0, this.Position + new Vector2(0.0f, -40f), false, false, layerDepth + 0.005f, 0.02f, lastItemHarvested.color.Value, 4f, -0.01f, 0.0f, 0.0f)
                      {
                        motion = new Vector2((index == 0 ? -1f : 1f) * 0.5f, -0.25f),
                        delayBeforeAnimationStart = 200
                      });
                    }
                  }
                }
                else
                {
                  Game1.multiplayer.broadcastSprites(this.currentLocation, new TemporaryAnimatedSprite(dataOrErrorItem.TextureName, dataOrErrorItem.GetSourceRect(), 1000f, 1, 0, this.Position + new Vector2(0.0f, -40f), false, false, layerDepth, 0.02f, Color.White, 4f, -0.01f, 0.0f, 0.0f)
                  {
                    motion = new Vector2(0.08f, -0.25f)
                  });
                  if (this.lastItemHarvested is ColoredObject lastItemHarvested)
                  {
                    Rectangle sourceRect = ItemRegistry.GetDataOrErrorItem(this.lastItemHarvested.QualifiedItemId).GetSourceRect(1);
                    Game1.multiplayer.broadcastSprites(this.currentLocation, new TemporaryAnimatedSprite(dataOrErrorItem.TextureName, sourceRect, 1000f, 1, 0, this.Position + new Vector2(0.0f, -40f), false, false, layerDepth + 0.005f, 0.02f, lastItemHarvested.color.Value, 4f, -0.01f, 0.0f, 0.0f)
                    {
                      motion = new Vector2(0.08f, -0.25f)
                    });
                  }
                }
              }
            }
          }
        }
        else if (this.harvestTimer <= 0)
          this.pokeToHarvest();
      }
      else if ((double) this.alpha > 0.0 && this.controller == null)
      {
        if (((double) this.addedSpeed > 0.0 || this.speed > 3 || this.isCharging) && Game1.IsMasterGame)
          this.destroy = true;
        this.nextPosition = this.GetBoundingBox();
        this.nextPosition.X += (int) this.motion.X;
        bool flag = false;
        if (!location.isCollidingPosition(this.nextPosition, Game1.viewport, (Character) this))
        {
          this.position.X += (float) (int) this.motion.X;
          flag = true;
        }
        this.nextPosition.X -= (int) this.motion.X;
        this.nextPosition.Y += (int) this.motion.Y;
        if (!location.isCollidingPosition(this.nextPosition, Game1.viewport, (Character) this))
        {
          this.position.Y += (float) (int) this.motion.Y;
          flag = true;
        }
        if (!this.motion.Equals(Vector2.Zero) & flag && Game1.random.NextDouble() < 0.005)
          Game1.multiplayer.broadcastSprites(location, new TemporaryAnimatedSprite(Game1.random.Choose<int>(10, 11), this.Position, this.color.Value)
          {
            motion = this.motion / 4f,
            alphaFade = 0.01f,
            layerDepth = 0.8f,
            scale = 0.75f,
            alpha = 0.75f
          });
        if (Game1.random.NextDouble() < 0.002)
        {
          switch (Game1.random.Next(6))
          {
            case 0:
              this.netAnimationEvent.Fire(6);
              break;
            case 1:
              this.netAnimationEvent.Fire(7);
              break;
            case 2:
              this.netAnimationEvent.Fire(0);
              break;
            case 3:
              this.jumpWithoutSound();
              this.yJumpVelocity /= 2f;
              this.netAnimationEvent.Fire(0);
              break;
            case 4:
              JunimoHut home = this.home;
              if (home != null && !home.noHarvest.Value)
              {
                this.pathfindToNewCrop();
                break;
              }
              break;
            case 5:
              this.netAnimationEvent.Fire(8);
              break;
          }
        }
      }
    }
    bool moveRight = this.moveRight;
    bool moveLeft = this.moveLeft;
    bool moveUp = this.moveUp;
    bool moveDown = this.moveDown;
    bool flag1;
    bool flag2;
    bool flag3;
    bool flag4;
    if (Game1.IsMasterGame)
    {
      if (this.controller == null && this.motion.Equals(Vector2.Zero))
        return;
      flag1 = ((moveRight ? 1 : 0) | ((double) Math.Abs(this.motion.X) <= (double) Math.Abs(this.motion.Y) ? 0 : ((double) this.motion.X > 0.0 ? 1 : 0))) != 0;
      flag2 = ((moveLeft ? 1 : 0) | ((double) Math.Abs(this.motion.X) <= (double) Math.Abs(this.motion.Y) ? 0 : ((double) this.motion.X < 0.0 ? 1 : 0))) != 0;
      flag3 = ((moveUp ? 1 : 0) | ((double) Math.Abs(this.motion.Y) <= (double) Math.Abs(this.motion.X) ? 0 : ((double) this.motion.Y < 0.0 ? 1 : 0))) != 0;
      flag4 = ((moveDown ? 1 : 0) | ((double) Math.Abs(this.motion.Y) <= (double) Math.Abs(this.motion.X) ? 0 : ((double) this.motion.Y > 0.0 ? 1 : 0))) != 0;
    }
    else
    {
      flag2 = this.IsRemoteMoving() && this.FacingDirection == 3;
      flag1 = this.IsRemoteMoving() && this.FacingDirection == 1;
      flag3 = this.IsRemoteMoving() && this.FacingDirection == 0;
      flag4 = this.IsRemoteMoving() && this.FacingDirection == 2;
      if (!flag1 && !flag2 && !flag3 && !flag4)
        return;
    }
    this.Sprite.CurrentAnimation = (List<FarmerSprite.AnimationFrame>) null;
    if (flag1)
    {
      this.flip = false;
      if (!this.Sprite.Animate(time, 16 /*0x10*/, 8, 50f))
        return;
      this.Sprite.currentFrame = 16 /*0x10*/;
    }
    else if (flag2)
    {
      if (this.Sprite.Animate(time, 16 /*0x10*/, 8, 50f))
        this.Sprite.currentFrame = 16 /*0x10*/;
      this.flip = true;
    }
    else if (flag3)
    {
      if (!this.Sprite.Animate(time, 32 /*0x20*/, 8, 50f))
        return;
      this.Sprite.currentFrame = 32 /*0x20*/;
    }
    else
    {
      if (!flag4)
        return;
      this.Sprite.Animate(time, 0, 8, 50f);
    }
  }

  public virtual void pathfindToRandomSpotAroundHut()
  {
    JunimoHut home = this.home;
    if (home == null)
      return;
    this.controller = new PathFindController((Character) this, this.currentLocation, Utility.Vector2ToPoint(new Vector2((float) (home.tileX.Value + 1 + Game1.random.Next(-home.cropHarvestRadius, home.cropHarvestRadius + 1)), (float) (home.tileY.Value + 1 + Game1.random.Next(-home.cropHarvestRadius, home.cropHarvestRadius + 1)))), -1, new PathFindController.endBehavior(this.reachFirstDestinationFromHut), 100);
  }

  public virtual void tryToAddItemToHut(Item i)
  {
    this.lastItemHarvested = i;
    Item obj = this.home?.GetOutputChest().addItem(i);
    if (obj == null)
      return;
    for (int index = 0; index < obj.Stack; ++index)
      Game1.createItemDebris(i.getOne(), this.Position, -1, this.currentLocation);
  }

  public override void draw(SpriteBatch b, float alpha = 1f)
  {
    if ((double) this.alpha <= 0.0)
      return;
    float val2 = (float) (this.StandingPixel.Y + 2) / 10000f;
    b.Draw(this.Sprite.Texture, this.getLocalPosition(Game1.viewport) + new Vector2((float) (this.Sprite.SpriteWidth * 4 / 2), (float) ((double) this.Sprite.SpriteHeight * 3.0 / 4.0 * 4.0 / Math.Pow((double) (this.Sprite.SpriteHeight / 16 /*0x10*/), 2.0) + (double) this.yJumpOffset - 8.0)) + (this.shakeTimer > 0 ? new Vector2((float) Game1.random.Next(-1, 2), (float) Game1.random.Next(-1, 2)) : Vector2.Zero), new Rectangle?(this.Sprite.SourceRect), this.color.Value * this.alpha, this.rotation, new Vector2((float) (this.Sprite.SpriteWidth * 4 / 2), (float) ((double) (this.Sprite.SpriteHeight * 4) * 3.0 / 4.0)) / 4f, Math.Max(0.2f, this.scale.Value) * 4f, this.flip ? SpriteEffects.FlipHorizontally : SpriteEffects.None, Math.Max(0.0f, this.drawOnTop ? 0.991f : val2));
    if (this.swimming.Value)
      return;
    SpriteBatch spriteBatch = b;
    Texture2D shadowTexture = Game1.shadowTexture;
    Vector2 local = Game1.GlobalToLocal(Game1.viewport, this.Position + new Vector2((float) (this.Sprite.SpriteWidth * 4) / 2f, 44f));
    Rectangle? sourceRectangle = new Rectangle?(Game1.shadowTexture.Bounds);
    Color color = this.color.Value * this.alpha;
    Rectangle bounds = Game1.shadowTexture.Bounds;
    double x = (double) bounds.Center.X;
    bounds = Game1.shadowTexture.Bounds;
    double y = (double) bounds.Center.Y;
    Vector2 origin = new Vector2((float) x, (float) y);
    double scale = (4.0 + (double) this.yJumpOffset / 40.0) * (double) this.scale.Value;
    double layerDepth = (double) Math.Max(0.0f, val2) - 9.9999999747524271E-07;
    spriteBatch.Draw(shadowTexture, local, sourceRectangle, color, 0.0f, origin, (float) scale, SpriteEffects.None, (float) layerDepth);
  }
}
