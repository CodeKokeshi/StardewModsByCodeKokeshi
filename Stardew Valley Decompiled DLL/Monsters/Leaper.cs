// Decompiled with JetBrains decompiler
// Type: StardewValley.Monsters.Leaper
// Assembly: Stardew Valley, Version=1.6.15.24356, Culture=neutral, PublicKeyToken=null
// MVID: AA1F513A-94F0-4EF6-A35F-2D5B4A3721BF
// Assembly location: D:\SteamLibrary\steamapps\common\Stardew Valley\Stardew Valley.dll
// XML documentation location: D:\SteamLibrary\steamapps\common\Stardew Valley\Stardew Valley.xml

using Microsoft.Xna.Framework;
using Netcode;
using StardewValley.Locations;
using System;
using xTile.Dimensions;

#nullable disable
namespace StardewValley.Monsters;

public class Leaper : Monster
{
  public NetFloat leapDuration = new NetFloat(0.75f);
  public NetFloat leapProgress = new NetFloat(0.0f);
  public NetBool leaping = new NetBool(false);
  public NetVector2 leapStartPosition = new NetVector2();
  public NetVector2 leapEndPosition = new NetVector2();
  public float nextLeap;

  public Leaper()
  {
  }

  public Leaper(Vector2 position)
    : base("Spider", position)
  {
    this.forceOneTileWide.Value = true;
    this.IsWalkingTowardPlayer = false;
    this.nextLeap = Utility.RandomFloat(1f, 1.5f);
    this.isHardModeMonster.Value = true;
    this.reloadSprite(false);
  }

  public override int GetBaseDifficultyLevel() => 1;

  /// <inheritdoc />
  public override void reloadSprite(bool onlyAppearance = false)
  {
    base.reloadSprite(onlyAppearance);
    this.Sprite.SpriteWidth = 32 /*0x20*/;
    this.Sprite.SpriteHeight = 32 /*0x20*/;
    this.Sprite.UpdateSourceRect();
  }

  protected override void initNetFields()
  {
    base.initNetFields();
    this.NetFields.AddField((INetSerializable) this.leapDuration, "leapDuration").AddField((INetSerializable) this.leapProgress, "leapProgress").AddField((INetSerializable) this.leapStartPosition, "leapStartPosition").AddField((INetSerializable) this.leapEndPosition, "leapEndPosition").AddField((INetSerializable) this.leaping, "leaping");
    this.leapProgress.Interpolated(true, true);
    this.leaping.Interpolated(true, true);
    this.leaping.fieldChangeVisibleEvent += new FieldChange<NetBool, bool>(this.OnLeapingChanged);
  }

  public virtual void OnLeapingChanged(NetBool field, bool old_value, bool new_value)
  {
  }

  public override bool isInvincible() => this.leaping.Value || base.isInvincible();

  public override void updateMovement(GameLocation location, GameTime time)
  {
  }

  protected override void localDeathAnimation()
  {
    this.currentLocation.localSound("monsterdead");
    Utility.makeTemporarySpriteJuicier(new TemporaryAnimatedSprite(44, this.Position, Color.DarkRed, 10)
    {
      holdLastFrame = true,
      alphaFade = 0.01f,
      interval = 70f
    }, this.currentLocation);
  }

  protected override void sharedDeathAnimation()
  {
  }

  public override void defaultMovementBehavior(GameTime time)
  {
  }

  public override void noMovementProgressNearPlayerBehavior()
  {
  }

  public override void update(GameTime time, GameLocation location)
  {
    this.farmerPassesThrough = true;
    base.update(time, location);
    if (this.leaping.Value)
    {
      this.yJumpGravity = 0.0f;
      float num1 = this.leapProgress.Value;
      if (!Game1.IsMasterGame)
      {
        float num2 = (this.leapStartPosition.Value - this.leapEndPosition.Value).Length();
        num1 = (double) num2 != 0.0 ? (this.leapStartPosition.Value - this.Position).Length() / num2 : 0.0f;
        if ((double) num1 < 0.0)
          num1 = 0.0f;
        if ((double) num1 > 1.0)
          num1 = 1f;
      }
      this.yJumpOffset = (int) (Math.Sin((double) num1 * Math.PI) * -64.0 * 3.0);
    }
    else
      this.yJumpOffset = 0;
  }

  protected override void updateAnimation(GameTime time)
  {
    if (this.leaping.Value)
      this.Sprite.CurrentFrame = 2;
    else
      this.Sprite.Animate(time, 0, 2, 500f);
    this.Sprite.UpdateSourceRect();
  }

  public virtual bool IsValidLandingTile(Vector2 tile, bool check_other_characters = false)
  {
    if (this.currentLocation is MineShaft currentLocation && !currentLocation.isTileOnClearAndSolidGround(tile) || this.currentLocation.IsTileOccupiedBy(tile, CollisionMask.Buildings | CollisionMask.Flooring | CollisionMask.Furniture | CollisionMask.Objects | CollisionMask.TerrainFeatures | CollisionMask.LocationSpecific) || !this.currentLocation.isTileOnMap(tile) || !this.currentLocation.isTilePassable(new Location((int) tile.X, (int) tile.Y), Game1.viewport))
      return false;
    Microsoft.Xna.Framework.Rectangle boundingBox = this.GetBoundingBox();
    if (check_other_characters && this.currentLocation != null)
    {
      foreach (Character character in this.currentLocation.characters)
      {
        if (character != this && character.GetBoundingBox().Intersects(boundingBox))
          return false;
      }
    }
    return true;
  }

  public override void behaviorAtGameTick(GameTime time)
  {
    base.behaviorAtGameTick(time);
    if (this.leaping.Value)
    {
      this.leapProgress.Value += (float) time.ElapsedGameTime.TotalSeconds / this.leapDuration.Value;
      if ((double) this.leapProgress.Value >= 1.0)
        this.leapProgress.Value = 1f;
      this.Position = new Vector2(Utility.Lerp(this.leapStartPosition.X, this.leapEndPosition.X, this.leapProgress.Value), Utility.Lerp(this.leapStartPosition.Y, this.leapEndPosition.Y, this.leapProgress.Value));
      if ((double) this.leapProgress.Value != 1.0)
        return;
      this.leaping.Value = false;
      this.leapProgress.Value = 0.0f;
      if (this.IsValidLandingTile(this.Tile, true))
        return;
      this.nextLeap = 0.1f;
    }
    else
    {
      if ((double) this.nextLeap > 0.0)
        this.nextLeap -= (float) time.ElapsedGameTime.TotalSeconds;
      if ((double) this.nextLeap > 0.0)
        return;
      Vector2? nullable = new Vector2?();
      Vector2 tile1 = this.Tile;
      tile1.X = (float) (int) tile1.X;
      tile1.X = (float) (int) tile1.X;
      if (this.withinPlayerThreshold(5) && this.Player != null)
      {
        Vector2 tile2 = this.Tile;
        if (Game1.random.NextDouble() < 0.60000002384185791)
        {
          this.nextLeap = Utility.RandomFloat(1.25f, 1.5f);
          tile2 = this.Player.Tile;
          tile2.X = (float) (int) Math.Round((double) tile2.X);
          tile2.Y = (float) (int) Math.Round((double) tile2.Y);
          tile2.X += (float) Game1.random.Next(-1, 2);
          tile2.Y += (float) Game1.random.Next(-1, 2);
        }
        else
        {
          this.nextLeap = Utility.RandomFloat(0.1f, 0.2f);
          tile2.X += (float) Game1.random.Next(-1, 2);
          tile2.Y += (float) Game1.random.Next(-1, 2);
        }
        if (this.IsValidLandingTile(tile2))
          nullable = new Vector2?(tile2);
      }
      if (!nullable.HasValue)
      {
        for (int index = 0; index < 8; ++index)
        {
          Vector2 vector2 = new Vector2((float) Game1.random.Next(-4, 5), (float) Game1.random.Next(-4, 5));
          if (!(vector2 == Vector2.Zero))
          {
            Vector2 tile3 = tile1 + vector2;
            if (this.IsValidLandingTile(tile3))
            {
              this.nextLeap = Utility.RandomFloat(0.6f, 1.5f);
              nullable = new Vector2?(tile3);
              break;
            }
          }
        }
      }
      if (nullable.HasValue)
      {
        if (Utility.isOnScreen(this.Position, 128 /*0x80*/))
          this.currentLocation.playSound("batFlap");
        this.leapProgress.Value = 0.0f;
        this.leaping.Value = true;
        this.leapStartPosition.Value = this.Position;
        this.leapEndPosition.Value = nullable.Value * 64f;
      }
      else
        this.nextLeap = Utility.RandomFloat(0.25f, 0.5f);
    }
  }

  public override void shedChunks(int number, float scale)
  {
    Point standingPixel = this.StandingPixel;
    Game1.createRadialDebris(this.currentLocation, this.Sprite.textureName.Value, new Microsoft.Xna.Framework.Rectangle(0, 64 /*0x40*/, 16 /*0x10*/, 16 /*0x10*/), 8, standingPixel.X, standingPixel.Y, number, this.TilePoint.Y, Color.White, 4f);
  }
}
