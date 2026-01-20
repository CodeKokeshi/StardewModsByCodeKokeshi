// Decompiled with JetBrains decompiler
// Type: StardewValley.Objects.Trinkets.IceOrbTrinketEffect
// Assembly: Stardew Valley, Version=1.6.15.24356, Culture=neutral, PublicKeyToken=null
// MVID: AA1F513A-94F0-4EF6-A35F-2D5B4A3721BF
// Assembly location: D:\SteamLibrary\steamapps\common\Stardew Valley\Stardew Valley.dll
// XML documentation location: D:\SteamLibrary\steamapps\common\Stardew Valley\Stardew Valley.xml

using Microsoft.Xna.Framework;
using StardewValley.Monsters;
using StardewValley.Projectiles;
using StardewValley.TokenizableStrings;
using System;
using System.Globalization;

#nullable disable
namespace StardewValley.Objects.Trinkets;

/// <summary>Implements the special behavior for a <see cref="T:StardewValley.Objects.Trinkets.Trinket" /> which shoots a freezing projectile at enemies.</summary>
/// <inheritdoc />
public class IceOrbTrinketEffect(Trinket trinket) : TrinketEffect(trinket)
{
  /// <summary>The pixel range at which monsters can be targeted.</summary>
  public const int Range = 600;
  /// <summary>The number of milliseconds until the trinket next shoots a projectile.</summary>
  public float ProjectileTimer;
  /// <summary>The number of milliseconds between each projectile.</summary>
  public float ProjectileDelay = 4000f;
  /// <summary>The number of milliseconds for which a monster is frozen.</summary>
  public int FreezeTime = 4000;

  /// <inheritdoc />
  public override void Apply(Farmer farmer)
  {
    this.ProjectileTimer = 0.0f;
    base.Apply(farmer);
  }

  /// <inheritdoc />
  public override bool GenerateRandomStats(Trinket trinket)
  {
    Random random = Utility.CreateRandom((double) trinket.generationSeed.Value);
    this.ProjectileDelay = (float) random.Next(3000, 5001);
    this.FreezeTime = random.Next(2000, 4001);
    if (random.NextDouble() < 0.05)
    {
      trinket.displayNameOverrideTemplate.Value = TokenStringBuilder.LocalizedText("Strings\\1_6_Strings:PerfectIceRod");
      this.ProjectileDelay = 3000f;
      this.FreezeTime = 4000;
    }
    trinket.descriptionSubstitutionTemplates.Clear();
    trinket.descriptionSubstitutionTemplates.Add(Math.Round((double) this.ProjectileDelay / 1000.0, 1).ToString((IFormatProvider) CultureInfo.InvariantCulture));
    trinket.descriptionSubstitutionTemplates.Add(Math.Round((double) this.FreezeTime / 1000.0, 1).ToString((IFormatProvider) CultureInfo.InvariantCulture));
    return true;
  }

  /// <inheritdoc />
  public override void Update(Farmer farmer, GameTime time, GameLocation location)
  {
    if (!Game1.shouldTimePass())
      return;
    this.ProjectileTimer += (float) time.ElapsedGameTime.TotalMilliseconds;
    if ((double) this.ProjectileTimer >= (double) this.ProjectileDelay)
    {
      Monster monsterWithinRange = Utility.findClosestMonsterWithinRange(location, farmer.getStandingPosition(), 600);
      if (monsterWithinRange != null)
      {
        Vector2 velocityTowardPoint = Utility.getVelocityTowardPoint(farmer.getStandingPosition(), monsterWithinRange.getStandingPosition(), 5f);
        DebuffingProjectile debuffingProjectile = new DebuffingProjectile("frozen", 17, 0, 0, 0.0f, velocityTowardPoint.X, velocityTowardPoint.Y, farmer.getStandingPosition() - new Vector2(32f, 48f), location, (Character) farmer, true, false);
        debuffingProjectile.wavyMotion.Value = false;
        debuffingProjectile.piercesLeft.Value = 99999;
        debuffingProjectile.maxTravelDistance.Value = 3000;
        debuffingProjectile.IgnoreLocationCollision = true;
        debuffingProjectile.ignoreObjectCollisions.Value = true;
        debuffingProjectile.maxVelocity.Value = 12f;
        debuffingProjectile.projectileID.Value = 15;
        debuffingProjectile.alpha.Value = 1f / 1000f;
        debuffingProjectile.alphaChange.Value = 0.05f;
        debuffingProjectile.light.Value = true;
        debuffingProjectile.debuffIntensity.Value = this.FreezeTime;
        debuffingProjectile.boundingBoxWidth.Value = 32 /*0x20*/;
        location.projectiles.Add((Projectile) debuffingProjectile);
        location.playSound("fireball");
      }
      this.ProjectileTimer = 0.0f;
    }
    base.Update(farmer, time, location);
  }
}
