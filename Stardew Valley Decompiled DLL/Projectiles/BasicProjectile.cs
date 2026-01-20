// Decompiled with JetBrains decompiler
// Type: StardewValley.Projectiles.BasicProjectile
// Assembly: Stardew Valley, Version=1.6.15.24356, Culture=neutral, PublicKeyToken=null
// MVID: AA1F513A-94F0-4EF6-A35F-2D5B4A3721BF
// Assembly location: D:\SteamLibrary\steamapps\common\Stardew Valley\Stardew Valley.dll
// XML documentation location: D:\SteamLibrary\steamapps\common\Stardew Valley\Stardew Valley.xml

using Microsoft.Xna.Framework;
using Netcode;
using StardewValley.BellsAndWhistles;
using StardewValley.Extensions;
using StardewValley.Monsters;
using StardewValley.TerrainFeatures;
using StardewValley.TokenizableStrings;
using System;

#nullable disable
namespace StardewValley.Projectiles;

public class BasicProjectile : Projectile
{
  /// <summary>The amount of damage caused when this projectile hits a monster or player.</summary>
  public readonly NetInt damageToFarmer = new NetInt();
  /// <summary>The sound played when the projectile collides with something.</summary>
  public readonly NetString collisionSound = new NetString();
  /// <summary>Whether the projectile explodes when it collides with something.</summary>
  public readonly NetBool explode = new NetBool();
  /// <summary>A callback to invoke after the projectile collides with a player, monster, or wall.</summary>
  public BasicProjectile.onCollisionBehavior collisionBehavior;
  /// <summary>The buff ID to apply to players hit by this projectile, if any.</summary>
  public NetString debuff = new NetString((string) null);
  /// <summary>The sound to play when <see cref="F:StardewValley.Projectiles.BasicProjectile.debuff" /> is applied to a player.</summary>
  public NetString debuffSound = new NetString("debuffHit");

  /// <summary>Construct an empty instance.</summary>
  public BasicProjectile()
  {
  }

  /// <summary>Construct an instance.</summary>
  /// <param name="damageToFarmer">The amount of damage caused when this projectile hits a monster or player.</param>
  /// <param name="spriteIndex">The index of the sprite to draw in <see cref="F:StardewValley.Projectiles.Projectile.projectileSheetName" />. Ignored if <paramref name="shotItemId" /> is set.</param>
  /// <param name="bouncesTillDestruct">The number of times the projectile can bounce off walls before being destroyed.</param>
  /// <param name="tailLength">The length of the tail which trails behind the main projectile.</param>
  /// <param name="rotationVelocity">The rotation velocity.</param>
  /// <param name="xVelocity">The speed at which the projectile moves along the X axis.</param>
  /// <param name="yVelocity">The speed at which the projectile moves along the Y axis.</param>
  /// <param name="startingPosition">The pixel world position at which the projectile will start moving.</param>
  /// <param name="collisionSound">The sound played when the projectile collides with something.</param>
  /// <param name="bounceSound">The sound played when the projectile bounces off a wall.</param>
  /// <param name="firingSound">The sound played when the projectile is fired.</param>
  /// <param name="explode">Whether the projectile explodes when it collides with something.</param>
  /// <param name="damagesMonsters">Whether the projectile damage monsters (true) or players (false).</param>
  /// <param name="location">The location containing the projectile.</param>
  /// <param name="firer">The character who fired the projectile.</param>
  /// <param name="collisionBehavior">A callback to invoke after the projectile collides with a player, monster, or wall.</param>
  /// <param name="shotItemId">The qualified or unqualified item ID to shoot. If set, this overrides <paramref name="spriteIndex" />.</param>
  public BasicProjectile(
    int damageToFarmer,
    int spriteIndex,
    int bouncesTillDestruct,
    int tailLength,
    float rotationVelocity,
    float xVelocity,
    float yVelocity,
    Vector2 startingPosition,
    string collisionSound = null,
    string bounceSound = null,
    string firingSound = null,
    bool explode = false,
    bool damagesMonsters = false,
    GameLocation location = null,
    Character firer = null,
    BasicProjectile.onCollisionBehavior collisionBehavior = null,
    string shotItemId = null)
    : this()
  {
    this.damageToFarmer.Value = damageToFarmer;
    this.currentTileSheetIndex.Value = spriteIndex;
    this.bouncesLeft.Value = bouncesTillDestruct;
    this.tailLength.Value = tailLength;
    this.rotationVelocity.Value = rotationVelocity;
    this.xVelocity.Value = xVelocity;
    this.yVelocity.Value = yVelocity;
    this.position.Value = startingPosition;
    this.explode.Value = explode;
    this.collisionSound.Value = collisionSound;
    this.bounceSound.Value = bounceSound;
    this.damagesMonsters.Value = damagesMonsters;
    this.theOneWhoFiredMe.Set(location, firer);
    this.collisionBehavior = collisionBehavior;
    this.itemId.Value = ItemRegistry.QualifyItemId(shotItemId) ?? shotItemId;
    if (string.IsNullOrEmpty(firingSound) || location == null)
      return;
    location.playSound(firingSound);
  }

  /// <summary>Construct an instance preconfigured for a spell cast by a monster.</summary>
  /// <param name="damageToFarmer">The amount of damage caused when this projectile hits a monster or player.</param>
  /// <param name="spriteIndex">The index of the sprite to draw in <see cref="F:StardewValley.Projectiles.Projectile.projectileSheetName" />.</param>
  /// <param name="bouncesTillDestruct">The number of times the projectile can bounce off walls before being destroyed.</param>
  /// <param name="tailLength">The length of the tail which trails behind the main projectile.</param>
  /// <param name="rotationVelocity">The rotation velocity.</param>
  /// <param name="xVelocity">The speed at which the projectile moves along the X axis.</param>
  /// <param name="yVelocity">The speed at which the projectile moves along the Y axis.</param>
  /// <param name="startingPosition">The pixel world position at which the projectile will start moving.</param>
  public BasicProjectile(
    int damageToFarmer,
    int spriteIndex,
    int bouncesTillDestruct,
    int tailLength,
    float rotationVelocity,
    float xVelocity,
    float yVelocity,
    Vector2 startingPosition)
    : this(damageToFarmer, spriteIndex, bouncesTillDestruct, tailLength, rotationVelocity, xVelocity, yVelocity, startingPosition, "flameSpellHit", "flameSpell", explode: true)
  {
  }

  public override void updatePosition(GameTime time)
  {
    this.xVelocity.Value += this.acceleration.X;
    this.yVelocity.Value += this.acceleration.Y;
    if ((double) this.maxVelocity.Value != -1.0 && Math.Sqrt((double) this.xVelocity.Value * (double) this.xVelocity.Value + (double) this.yVelocity.Value * (double) this.yVelocity.Value) >= (double) this.maxVelocity.Value)
    {
      this.xVelocity.Value -= this.acceleration.X;
      this.yVelocity.Value -= this.acceleration.Y;
    }
    this.position.X += this.xVelocity.Value;
    this.position.Y += this.yVelocity.Value;
  }

  /// <inheritdoc />
  protected override void InitNetFields()
  {
    base.InitNetFields();
    this.NetFields.AddField((INetSerializable) this.damageToFarmer, "damageToFarmer").AddField((INetSerializable) this.collisionSound, "collisionSound").AddField((INetSerializable) this.explode, "explode").AddField((INetSerializable) this.debuff, "debuff").AddField((INetSerializable) this.debuffSound, "debuffSound");
  }

  public override void behaviorOnCollisionWithPlayer(GameLocation location, Farmer player)
  {
    if (this.damagesMonsters.Value)
      return;
    if (this.debuff.Value != null && player.CanBeDamaged() && Game1.random.Next(11) >= player.Immunity && !player.hasBuff("28") && !player.hasTrinketWithID("BasiliskPaw"))
    {
      if (Game1.player == player)
        player.applyBuff(this.debuff.Value);
      location.playSound(this.debuffSound.Value);
    }
    if (player.CanBeDamaged())
      --this.piercesLeft.Value;
    player.takeDamage(this.damageToFarmer.Value, false, (Monster) null);
    this.explosionAnimation(location);
  }

  public override void behaviorOnCollisionWithTerrainFeature(
    TerrainFeature t,
    Vector2 tileLocation,
    GameLocation location)
  {
    t.performUseAction(tileLocation);
    this.explosionAnimation(location);
    --this.piercesLeft.Value;
  }

  public override void behaviorOnCollisionWithOther(GameLocation location)
  {
    if (this.ignoreObjectCollisions.Value)
      return;
    this.explosionAnimation(location);
    --this.piercesLeft.Value;
  }

  public override void behaviorOnCollisionWithMonster(NPC n, GameLocation location)
  {
    if (!this.damagesMonsters.Value)
      return;
    Farmer playerWhoFiredMe = this.GetPlayerWhoFiredMe(location);
    this.explosionAnimation(location);
    if (n is Monster)
    {
      location.damageMonster(n.GetBoundingBox(), this.damageToFarmer.Value, this.damageToFarmer.Value + 1, false, playerWhoFiredMe, true);
      if (this.currentTileSheetIndex.Value == 15)
        Utility.addRainbowStarExplosion(location, this.position.Value, 11);
      if ((n as Monster).IsInvisible)
        return;
      --this.piercesLeft.Value;
    }
    else
    {
      if (this.itemId.Value == null)
        return;
      n.getHitByPlayer(playerWhoFiredMe, location);
      string word = TokenStringBuilder.ItemName(this.itemId.Value);
      Game1.multiplayer.globalChatInfoMessage("Slingshot_Hit", playerWhoFiredMe.Name, n.GetTokenizedDisplayName(), Lexicon.prependTokenizedArticle(word));
      --this.piercesLeft.Value;
    }
  }

  protected virtual void explosionAnimation(GameLocation location)
  {
    if (this.projectileID.Value == 14)
    {
      for (int index = 0; index < 12; ++index)
      {
        Vector2 position = new Vector2(0.0f, (float) ((double) Game1.random.Next(-10, 11) / 12.0 - 1.5));
        position = Vector2.Transform(position, Matrix.CreateRotationZ((float) (Math.PI / 6.0 + (double) Game1.random.Next(-10, 11) / 50.0) * (float) index));
        location.temporarySprites.Add(new TemporaryAnimatedSprite("LooseSprites\\Cursors_1_6", new Rectangle(144 /*0x90*/, 249, 7, 7), 80f, 6, 1, this.position.Value + new Vector2(8f, 8f) * 4f, false, false, 1f, 0.0f, Utility.Get2PhaseColor(Color.White, Color.Cyan, timeOffset: (float) Game1.random.Next(1000)), 4f, 0.0f, 0.0f, 0.0f)
        {
          drawAboveAlwaysFront = true,
          motion = position
        });
      }
    }
    else
    {
      Rectangle sourceRect = this.GetSourceRect();
      sourceRect.X += 4;
      sourceRect.Y += 4;
      sourceRect.Width = 8;
      sourceRect.Height = 8;
      if (this.itemId.Value != null)
      {
        int debrisType = 12;
        switch (this.itemId.Value)
        {
          case "(O)390":
            debrisType = 14;
            break;
          case "(O)378":
            debrisType = 0;
            break;
          case "(O)380":
            debrisType = 2;
            break;
          case "(O)384":
            debrisType = 6;
            break;
          case "(O)386":
            debrisType = 10;
            break;
          case "(O)382":
            debrisType = 4;
            break;
        }
        Game1.createRadialDebris(location, debrisType, (int) ((double) this.position.X + 32.0) / 64 /*0x40*/, (int) ((double) this.position.Y + 32.0) / 64 /*0x40*/, 6, false);
      }
      else
        Game1.createRadialDebris_MoreNatural(location, "TileSheets\\Projectiles", sourceRect, 1, (int) this.position.X + 32 /*0x20*/, (int) this.position.Y + 32 /*0x20*/, 6, (int) ((double) this.position.Y / 64.0) + 1);
    }
    if (!string.IsNullOrEmpty(this.collisionSound.Value))
      location.playSound(this.collisionSound.Value);
    if (this.explode.Value)
      Game1.multiplayer.broadcastSprites(location, new TemporaryAnimatedSprite(362, (float) Game1.random.Next(30, 90), 6, 1, this.position.Value, false, Game1.random.NextBool()));
    BasicProjectile.onCollisionBehavior collisionBehavior = this.collisionBehavior;
    if (collisionBehavior != null)
    {
      GameLocation location1 = location;
      Rectangle boundingBox = this.getBoundingBox();
      int x = boundingBox.Center.X;
      boundingBox = this.getBoundingBox();
      int y = boundingBox.Center.Y;
      Farmer playerWhoFiredMe = this.GetPlayerWhoFiredMe(location);
      collisionBehavior(location1, x, y, (Character) playerWhoFiredMe);
    }
    this.destroyMe = true;
  }

  public static void explodeOnImpact(GameLocation location, int x, int y, Character who)
  {
    location.explode(new Vector2((float) (x / 64 /*0x40*/), (float) (y / 64 /*0x40*/)), 2, who as Farmer);
  }

  /// <summary>Get the player who fired this projectile.</summary>
  /// <param name="location">The location containing the player.</param>
  public virtual Farmer GetPlayerWhoFiredMe(GameLocation location)
  {
    return this.theOneWhoFiredMe.Get(location) is Farmer farmer ? farmer : Game1.player;
  }

  public delegate void onCollisionBehavior(
    GameLocation location,
    int xPosition,
    int yPosition,
    Character who);
}
