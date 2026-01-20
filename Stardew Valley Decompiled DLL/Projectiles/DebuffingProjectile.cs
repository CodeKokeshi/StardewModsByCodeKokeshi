// Decompiled with JetBrains decompiler
// Type: StardewValley.Projectiles.DebuffingProjectile
// Assembly: Stardew Valley, Version=1.6.15.24356, Culture=neutral, PublicKeyToken=null
// MVID: AA1F513A-94F0-4EF6-A35F-2D5B4A3721BF
// Assembly location: D:\SteamLibrary\steamapps\common\Stardew Valley\Stardew Valley.dll
// XML documentation location: D:\SteamLibrary\steamapps\common\Stardew Valley\Stardew Valley.xml

using Microsoft.Xna.Framework;
using Netcode;
using StardewValley.Monsters;
using StardewValley.TerrainFeatures;
using System;

#nullable disable
namespace StardewValley.Projectiles;

public class DebuffingProjectile : Projectile
{
  /// <summary>The buff ID to apply to players hit by this projectile.</summary>
  public readonly NetString debuff = new NetString();
  public NetBool wavyMotion = new NetBool(true);
  public NetInt debuffIntensity = new NetInt(-1);
  private float periodicEffectTimer;

  /// <summary>Construct an empty instance.</summary>
  public DebuffingProjectile()
  {
  }

  /// <summary>Construct an instance.</summary>
  /// <param name="debuff">The debuff ID to apply to players hit by this projectile.</param>
  /// <param name="spriteIndex">The index of the sprite to draw in <see cref="F:StardewValley.Projectiles.Projectile.projectileSheetName" />.</param>
  /// <param name="bouncesTillDestruct">The number of times the projectile can bounce off walls before being destroyed.</param>
  /// <param name="tailLength">The length of the tail which trails behind the main projectile.</param>
  /// <param name="rotationVelocity">The rotation velocity.</param>
  /// <param name="xVelocity">The speed at which the projectile moves along the X axis.</param>
  /// <param name="yVelocity">The speed at which the projectile moves along the Y axis.</param>
  /// <param name="startingPosition">The pixel world position at which the projectile will start moving.</param>
  /// <param name="location">The location containing the projectile.</param>
  /// <param name="owner">The character who fired the projectile.</param>
  public DebuffingProjectile(
    string debuff,
    int spriteIndex,
    int bouncesTillDestruct,
    int tailLength,
    float rotationVelocity,
    float xVelocity,
    float yVelocity,
    Vector2 startingPosition,
    GameLocation location = null,
    Character owner = null,
    bool hitsMonsters = false,
    bool playDefaultSoundOnFire = true)
    : this()
  {
    this.theOneWhoFiredMe.Set(location, owner);
    this.debuff.Value = debuff;
    this.currentTileSheetIndex.Value = spriteIndex;
    this.bouncesLeft.Value = bouncesTillDestruct;
    this.tailLength.Value = tailLength;
    this.rotationVelocity.Value = rotationVelocity;
    this.xVelocity.Value = xVelocity;
    this.yVelocity.Value = yVelocity;
    this.position.Value = startingPosition;
    this.damagesMonsters.Value = hitsMonsters;
    if (!playDefaultSoundOnFire)
      return;
    if (location == null)
      Game1.playSound("debuffSpell");
    else
      location.playSound("debuffSpell");
  }

  /// <inheritdoc />
  protected override void InitNetFields()
  {
    base.InitNetFields();
    this.NetFields.AddField((INetSerializable) this.debuff, "debuff").AddField((INetSerializable) this.wavyMotion, "wavyMotion").AddField((INetSerializable) this.debuffIntensity, "debuffIntensity");
  }

  public override void updatePosition(GameTime time)
  {
    this.xVelocity.Value += this.acceleration.X;
    this.yVelocity.Value += this.acceleration.Y;
    this.position.X += this.xVelocity.Value;
    this.position.Y += this.yVelocity.Value;
    if (!this.wavyMotion.Value)
      return;
    this.position.X += (float) (Math.Sin((double) time.TotalGameTime.Milliseconds * Math.PI / 128.0) * 8.0);
    this.position.Y += (float) (Math.Cos((double) time.TotalGameTime.Milliseconds * Math.PI / 128.0) * 8.0);
  }

  public override bool update(GameTime time, GameLocation location)
  {
    if (this.debuff.Value == "frozen")
    {
      this.periodicEffectTimer += (float) time.ElapsedGameTime.TotalMilliseconds;
      if ((double) this.periodicEffectTimer > 50.0)
      {
        this.periodicEffectTimer = 0.0f;
        location.temporarySprites.Add(new TemporaryAnimatedSprite("TileSheets\\Projectiles", new Rectangle(32 /*0x20*/, 32 /*0x20*/, 16 /*0x10*/, 16 /*0x10*/), 9999f, 1, 1, this.position.Value, false, false, 1f, 0.01f, Color.White, 4f, 0.0f, 0.0f, 0.0f)
        {
          motion = Utility.getRandom360degreeVector(1f) + new Vector2(this.xVelocity.Value, this.yVelocity.Value),
          drawAboveAlwaysFront = true
        });
      }
    }
    return base.update(time, location);
  }

  public override void behaviorOnCollisionWithPlayer(GameLocation location, Farmer player)
  {
    if (this.damagesMonsters.Value || Game1.random.Next(11) < player.Immunity || player.hasBuff("28") || player.hasTrinketWithID("BasiliskPaw"))
      return;
    --this.piercesLeft.Value;
    if (Game1.player == player)
      player.applyBuff(this.debuff.Value);
    this.explosionAnimation(location);
    if (this.debuff.Value == "19")
      location.playSound("frozen");
    else
      location.playSound("debuffHit");
  }

  public override void behaviorOnCollisionWithTerrainFeature(
    TerrainFeature t,
    Vector2 tileLocation,
    GameLocation location)
  {
    this.explosionAnimation(location);
    --this.piercesLeft.Value;
  }

  public override void behaviorOnCollisionWithOther(GameLocation location)
  {
    this.explosionAnimation(location);
    --this.piercesLeft.Value;
  }

  protected virtual void explosionAnimation(GameLocation location)
  {
    if (this.debuff.Value == "frozen")
      return;
    Game1.multiplayer.broadcastSprites(location, new TemporaryAnimatedSprite(352, (float) Game1.random.Next(100, 150), 2, 1, this.position.Value, false, false));
  }

  public override void behaviorOnCollisionWithMonster(NPC n, GameLocation location)
  {
    if (!this.damagesMonsters.Value || !(n is Monster) || !(this.debuff.Value == "frozen") || n is Leaper leaper && leaper.leaping.Value)
      return;
    if ((n as Monster).stunTime.Value < 51)
      --this.piercesLeft.Value;
    if ((n as Monster).stunTime.Value < this.debuffIntensity.Value - 1000)
    {
      location.playSound("frozen");
      Game1.multiplayer.broadcastSprites(location, new TemporaryAnimatedSprite("LooseSprites\\Cursors2", new Rectangle(118, 227, 16 /*0x10*/, 13), new Vector2(0.0f, 0.0f), false, 0.0f, Color.White)
      {
        layerDepth = (float) (n.StandingPixel.Y + 2) / 10000f,
        animationLength = 1,
        interval = (float) this.debuffIntensity.Value,
        scale = 4f,
        id = (int) ((double) n.position.X * 777.0 + (double) n.position.Y * 77777.0),
        positionFollowsAttachedCharacter = true,
        attachedCharacter = (Character) n
      });
    }
    (n as Monster).stunTime.Value = this.debuffIntensity.Value;
  }
}
