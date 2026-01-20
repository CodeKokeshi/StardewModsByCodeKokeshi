// Decompiled with JetBrains decompiler
// Type: StardewValley.Monsters.DinoMonster
// Assembly: Stardew Valley, Version=1.6.15.24356, Culture=neutral, PublicKeyToken=null
// MVID: AA1F513A-94F0-4EF6-A35F-2D5B4A3721BF
// Assembly location: D:\SteamLibrary\steamapps\common\Stardew Valley\Stardew Valley.dll
// XML documentation location: D:\SteamLibrary\steamapps\common\Stardew Valley\Stardew Valley.xml

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Netcode;
using StardewValley.Extensions;
using StardewValley.Projectiles;
using StardewValley.TerrainFeatures;
using System;
using System.Collections.Generic;
using System.Xml.Serialization;

#nullable disable
namespace StardewValley.Monsters;

[XmlInclude(typeof (DinoMonster.BreathProjectile))]
public class DinoMonster : Monster
{
  public int timeUntilNextAttack;
  public readonly NetBool firing = new NetBool(false);
  public NetInt attackState = new NetInt();
  public int nextFireTime;
  public int totalFireTime;
  public int nextChangeDirectionTime;
  public int nextWanderTime;
  public bool wanderState;
  public readonly NetObjectArray<DinoMonster.BreathProjectile> projectiles = new NetObjectArray<DinoMonster.BreathProjectile>(15);
  public int lastProjectileSlot;

  public DinoMonster()
  {
  }

  public DinoMonster(Vector2 position)
    : base("Pepper Rex", position)
  {
    this.Sprite.SpriteWidth = 32 /*0x20*/;
    this.Sprite.SpriteHeight = 32 /*0x20*/;
    this.Sprite.UpdateSourceRect();
    this.timeUntilNextAttack = 2000;
    this.nextChangeDirectionTime = Game1.random.Next(1000, 3000);
    this.nextWanderTime = Game1.random.Next(1000, 2000);
    for (int index = 0; index < this.projectiles.Count; ++index)
      this.projectiles[index] = new DinoMonster.BreathProjectile();
  }

  protected override void initNetFields()
  {
    base.initNetFields();
    this.NetFields.AddField((INetSerializable) this.attackState, "attackState").AddField((INetSerializable) this.firing, "firing").AddField((INetSerializable) this.projectiles, "projectiles");
  }

  /// <inheritdoc />
  public override void reloadSprite(bool onlyAppearance = false)
  {
    base.reloadSprite(onlyAppearance);
    this.Sprite.SpriteWidth = 32 /*0x20*/;
    this.Sprite.SpriteHeight = 32 /*0x20*/;
    this.Sprite.UpdateSourceRect();
  }

  public override void draw(SpriteBatch b)
  {
    if (this.Health > 0 && !this.IsInvisible && Utility.isOnScreen(this.Position, 128 /*0x80*/))
    {
      int y = this.StandingPixel.Y;
      b.Draw(this.Sprite.Texture, this.getLocalPosition(Game1.viewport) + new Vector2(56f, (float) (16 /*0x10*/ + this.yJumpOffset)), new Rectangle?(this.Sprite.SourceRect), Color.White, this.rotation, new Vector2(16f, 16f), Math.Max(0.2f, this.scale.Value) * 4f, this.flip ? SpriteEffects.FlipHorizontally : SpriteEffects.None, Math.Max(0.0f, this.drawOnTop ? 0.991f : (float) y / 10000f));
      if (this.isGlowing)
        b.Draw(this.Sprite.Texture, this.getLocalPosition(Game1.viewport) + new Vector2(56f, (float) (16 /*0x10*/ + this.yJumpOffset)), new Rectangle?(this.Sprite.SourceRect), this.glowingColor * this.glowingTransparency, 0.0f, new Vector2(16f, 16f), 4f * Math.Max(0.2f, this.scale.Value), this.flip ? SpriteEffects.FlipHorizontally : SpriteEffects.None, Math.Max(0.0f, this.drawOnTop ? 0.991f : (float) ((double) y / 10000.0 + 1.0 / 1000.0)));
    }
    foreach (DinoMonster.BreathProjectile projectile in (NetArray<DinoMonster.BreathProjectile, NetRef<DinoMonster.BreathProjectile>>) this.projectiles)
    {
      if (Utility.isOnScreen(projectile.position.Value, 64 /*0x40*/))
        projectile.Draw(b);
    }
  }

  public override Rectangle GetBoundingBox()
  {
    if (this.Health <= 0)
      return new Rectangle(-100, -100, 0, 0);
    Vector2 position = this.Position;
    return new Rectangle((int) position.X + 8, (int) position.Y, this.Sprite.SpriteWidth * 4 * 3 / 4, 64 /*0x40*/);
  }

  public override List<Item> getExtraDropItems()
  {
    List<Item> extraDropItems = new List<Item>();
    if (Game1.random.NextDouble() < 0.10000000149011612)
    {
      extraDropItems.Add(ItemRegistry.Create("(O)107"));
    }
    else
    {
      Item[] options = new Item[3]
      {
        ItemRegistry.Create("(O)580"),
        ItemRegistry.Create("(O)583"),
        ItemRegistry.Create("(O)584")
      };
      extraDropItems.Add(Game1.random.ChooseFrom<Item>((IList<Item>) options));
    }
    return extraDropItems;
  }

  public override bool ShouldMonsterBeRemoved()
  {
    foreach (DinoMonster.BreathProjectile projectile in (NetArray<DinoMonster.BreathProjectile, NetRef<DinoMonster.BreathProjectile>>) this.projectiles)
    {
      if (projectile.active.Value)
        return false;
    }
    return base.ShouldMonsterBeRemoved();
  }

  protected override void sharedDeathAnimation()
  {
    this.currentLocation.playSound("skeletonDie");
    this.currentLocation.playSound("grunt");
    Rectangle boundingBox = this.GetBoundingBox();
    for (int index = 0; index < 16 /*0x10*/; ++index)
      Game1.createRadialDebris(this.currentLocation, this.Sprite.textureName.Value, new Rectangle(64 /*0x40*/, 128 /*0x80*/, 16 /*0x10*/, 16 /*0x10*/), 16 /*0x10*/, (int) Utility.Lerp((float) boundingBox.Left, (float) boundingBox.Right, (float) Game1.random.NextDouble()), (int) Utility.Lerp((float) boundingBox.Bottom, (float) boundingBox.Top, (float) Game1.random.NextDouble()), 1, this.TilePoint.Y, Color.White, 4f);
  }

  protected override void localDeathAnimation()
  {
    Utility.makeTemporarySpriteJuicier(new TemporaryAnimatedSprite(44, this.Position, Color.HotPink, 10)
    {
      holdLastFrame = true,
      alphaFade = 0.01f,
      interval = 70f
    }, this.currentLocation, 8, 96 /*0x60*/);
  }

  public override void update(GameTime time, GameLocation location)
  {
    if (this.Health > 0)
      base.update(time, location);
    foreach (DinoMonster.BreathProjectile projectile in (NetArray<DinoMonster.BreathProjectile, NetRef<DinoMonster.BreathProjectile>>) this.projectiles)
      projectile.Update(time, location, this);
  }

  public override void behaviorAtGameTick(GameTime time)
  {
    TimeSpan elapsedGameTime;
    if (this.attackState.Value == 1)
    {
      this.IsWalkingTowardPlayer = false;
      this.Halt();
    }
    else if (this.withinPlayerThreshold())
    {
      this.IsWalkingTowardPlayer = true;
    }
    else
    {
      this.IsWalkingTowardPlayer = false;
      int changeDirectionTime = this.nextChangeDirectionTime;
      elapsedGameTime = time.ElapsedGameTime;
      int milliseconds1 = elapsedGameTime.Milliseconds;
      this.nextChangeDirectionTime = changeDirectionTime - milliseconds1;
      int nextWanderTime = this.nextWanderTime;
      elapsedGameTime = time.ElapsedGameTime;
      int milliseconds2 = elapsedGameTime.Milliseconds;
      this.nextWanderTime = nextWanderTime - milliseconds2;
      if (this.nextChangeDirectionTime < 0)
      {
        this.nextChangeDirectionTime = Game1.random.Next(500, 1000);
        this.facingDirection.Value = (this.facingDirection.Value + (Game1.random.Next(0, 3) - 1) + 4) % 4;
      }
      if (this.nextWanderTime < 0)
      {
        this.nextWanderTime = !this.wanderState ? Game1.random.Next(1000, 3000) : Game1.random.Next(1000, 2000);
        this.wanderState = !this.wanderState;
      }
      if (this.wanderState)
      {
        this.moveLeft = this.moveUp = this.moveRight = this.moveDown = false;
        this.tryToMoveInDirection(this.facingDirection.Value, false, this.DamageToFarmer, this.isGlider.Value);
      }
    }
    int timeUntilNextAttack = this.timeUntilNextAttack;
    elapsedGameTime = time.ElapsedGameTime;
    int milliseconds3 = elapsedGameTime.Milliseconds;
    this.timeUntilNextAttack = timeUntilNextAttack - milliseconds3;
    if (this.attackState.Value == 0 && this.withinPlayerThreshold(2))
    {
      this.firing.Set(false);
      if (this.timeUntilNextAttack >= 0)
        return;
      this.timeUntilNextAttack = 0;
      this.attackState.Set(1);
      this.nextFireTime = 500;
      this.totalFireTime = 3000;
      this.currentLocation.playSound("croak");
    }
    else
    {
      if (this.totalFireTime <= 0)
        return;
      if (!this.firing.Value)
      {
        Farmer player = this.Player;
        if (player != null)
          this.faceGeneralDirection(player.Position);
      }
      int totalFireTime = this.totalFireTime;
      elapsedGameTime = time.ElapsedGameTime;
      int milliseconds4 = elapsedGameTime.Milliseconds;
      this.totalFireTime = totalFireTime - milliseconds4;
      if (this.nextFireTime > 0)
      {
        int nextFireTime = this.nextFireTime;
        elapsedGameTime = time.ElapsedGameTime;
        int milliseconds5 = elapsedGameTime.Milliseconds;
        this.nextFireTime = nextFireTime - milliseconds5;
        if (this.nextFireTime <= 0)
        {
          if (!this.firing.Value)
          {
            this.firing.Set(true);
            this.currentLocation.playSound("furnace");
          }
          float num1 = 0.0f;
          Point standingPixel = this.StandingPixel;
          Vector2 vector2_1 = new Vector2((float) standingPixel.X - 32f, (float) standingPixel.Y - 32f);
          switch (this.facingDirection.Value)
          {
            case 0:
              this.yVelocity = -1f;
              vector2_1.Y -= 64f;
              num1 = 90f;
              break;
            case 1:
              this.xVelocity = -1f;
              vector2_1.X += 64f;
              num1 = 0.0f;
              break;
            case 2:
              this.yVelocity = 1f;
              num1 = 270f;
              break;
            case 3:
              this.xVelocity = 1f;
              vector2_1.X -= 64f;
              num1 = 180f;
              break;
          }
          float num2 = num1 + (float) Math.Sin((double) this.totalFireTime / 1000.0 * 180.0 * Math.PI / 180.0) * 25f;
          Vector2 vector2_2 = new Vector2((float) Math.Cos((double) num2 * Math.PI / 180.0), -(float) Math.Sin((double) num2 * Math.PI / 180.0)) * 10f;
          DinoMonster.BreathProjectile projectile = this.projectiles[this.lastProjectileSlot];
          projectile.active.Value = true;
          projectile.position.Value = projectile.startPosition.Value = vector2_1;
          projectile.velocity.Value = vector2_2;
          this.lastProjectileSlot = (this.lastProjectileSlot + 1) % this.projectiles.Count;
          this.nextFireTime = 70;
        }
      }
      if (this.totalFireTime > 0)
        return;
      this.totalFireTime = 0;
      this.nextFireTime = 0;
      this.attackState.Set(0);
      this.timeUntilNextAttack = Game1.random.Next(1000, 2000);
    }
  }

  protected override void updateAnimation(GameTime time)
  {
    int num = 0;
    switch (this.FacingDirection)
    {
      case 0:
        num = 8;
        break;
      case 1:
        num = 4;
        break;
      case 2:
        num = 0;
        break;
      case 3:
        num = 12;
        break;
    }
    if (this.attackState.Value == 1)
    {
      if (this.firing.Value)
        this.Sprite.CurrentFrame = 16 /*0x10*/ + num;
      else
        this.Sprite.CurrentFrame = 17 + num;
    }
    else if (this.isMoving() || this.wanderState)
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
    else
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
      this.Sprite.StopAnimation();
    }
  }

  protected override void updateMonsterSlaveAnimation(GameTime time)
  {
    int num = 0;
    switch (this.FacingDirection)
    {
      case 0:
        num = 8;
        break;
      case 1:
        num = 4;
        break;
      case 2:
        num = 0;
        break;
      case 3:
        num = 12;
        break;
    }
    if (this.attackState.Value == 1)
    {
      if (this.firing.Value)
        this.Sprite.CurrentFrame = 16 /*0x10*/ + num;
      else
        this.Sprite.CurrentFrame = 17 + num;
    }
    else if (this.isMoving())
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
    else
      this.Sprite.StopAnimation();
  }

  public enum AttackState
  {
    None,
    Fireball,
    Charge,
  }

  /// <summary>Lightweight version of projectile for pooling.</summary>
  public class BreathProjectile : INetObject<NetFields>
  {
    public readonly NetBool active = new NetBool();
    public readonly NetVector2 position = new NetVector2();
    public readonly NetVector2 startPosition = new NetVector2();
    public readonly NetVector2 velocity = new NetVector2();
    public float rotation;
    public float alpha;

    public NetFields NetFields { get; } = new NetFields(nameof (BreathProjectile));

    public BreathProjectile()
    {
      this.NetFields.SetOwner((INetObject<NetFields>) this).AddField((INetSerializable) this.active, nameof (active)).AddField((INetSerializable) this.position, nameof (position)).AddField((INetSerializable) this.startPosition, nameof (startPosition)).AddField((INetSerializable) this.velocity, nameof (velocity));
      this.active.InterpolationEnabled = this.active.InterpolationWait = false;
      this.position.InterpolationEnabled = this.position.InterpolationWait = false;
      this.startPosition.InterpolationEnabled = this.startPosition.InterpolationWait = false;
      this.velocity.InterpolationEnabled = this.velocity.InterpolationWait = false;
    }

    public Rectangle GetBoundingBox()
    {
      Vector2 vector2 = this.position.Value;
      int num = (int) (29.0 * 1.0);
      return new Rectangle((int) vector2.X + 32 /*0x20*/ - num / 2, (int) vector2.Y + 32 /*0x20*/ - num / 2, num, num);
    }

    public Rectangle GetSourceRect()
    {
      return Game1.getSourceRectForStandardTileSheet(Projectile.projectileSheet, 10, 16 /*0x10*/, 16 /*0x10*/);
    }

    public void ExplosionAnimation(GameLocation location)
    {
      Rectangle sourceRect = this.GetSourceRect();
      sourceRect.X += 4;
      sourceRect.Y += 4;
      sourceRect.Width = 8;
      sourceRect.Height = 8;
      Game1.createRadialDebris_MoreNatural(location, "TileSheets\\Projectiles", sourceRect, 1, (int) this.position.X + 32 /*0x20*/, (int) this.position.Y + 32 /*0x20*/, 6, (int) ((double) this.position.Y / 64.0) + 1);
    }

    public void Update(GameTime time, GameLocation location, DinoMonster parent)
    {
      if (!this.active.Value)
        return;
      NetVector2 position = this.position;
      position.Value = position.Value + this.velocity.Value;
      if (!Game1.IsMasterGame)
      {
        this.position.MarkClean();
        this.position.ResetNewestReceivedChangeVersion();
      }
      float num = Vector2.Distance(this.position.Value, this.startPosition.Value);
      this.alpha = (double) num <= 128.0 ? 1f : (float) ((256.0 - (double) num) / 128.0);
      if ((double) num > 256.0)
      {
        this.active.Value = false;
      }
      else
      {
        Rectangle boundingBox = this.GetBoundingBox();
        if (Game1.player.currentLocation == location && Game1.player.CanBeDamaged() && boundingBox.Intersects(Game1.player.GetBoundingBox()))
        {
          Game1.player.takeDamage(25, false, (Monster) null);
          this.ExplosionAnimation(location);
          this.active.Value = false;
        }
        else
        {
          foreach (Vector2 key in Utility.getListOfTileLocationsForBordersOfNonTileRectangle(boundingBox))
          {
            TerrainFeature terrainFeature;
            if (location.terrainFeatures.TryGetValue(key, out terrainFeature) && !terrainFeature.isPassable())
            {
              this.ExplosionAnimation(location);
              this.active.Value = false;
              return;
            }
          }
          if (location.isTileOnMap(this.position.Value / 64f) && !location.isCollidingPosition(boundingBox, Game1.viewport, false, 0, true, (Character) parent, false, true))
            return;
          this.ExplosionAnimation(location);
          this.active.Value = false;
        }
      }
    }

    public void Draw(SpriteBatch b)
    {
      if (!this.active.Value)
        return;
      float scale = 4f;
      Texture2D projectileSheet = Projectile.projectileSheet;
      Rectangle sourceRect = this.GetSourceRect();
      Vector2 vector2 = this.position.Value;
      b.Draw(projectileSheet, Game1.GlobalToLocal(Game1.viewport, vector2 + new Vector2(32f, 32f)), new Rectangle?(sourceRect), Color.White * this.alpha, this.rotation, new Vector2(8f, 8f), scale, SpriteEffects.None, (float) (((double) vector2.Y + 96.0) / 10000.0));
    }
  }
}
