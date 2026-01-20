// Decompiled with JetBrains decompiler
// Type: StardewValley.Objects.Trinkets.MagicQuiverTrinketEffect
// Assembly: Stardew Valley, Version=1.6.15.24356, Culture=neutral, PublicKeyToken=null
// MVID: AA1F513A-94F0-4EF6-A35F-2D5B4A3721BF
// Assembly location: D:\SteamLibrary\steamapps\common\Stardew Valley\Stardew Valley.dll
// XML documentation location: D:\SteamLibrary\steamapps\common\Stardew Valley\Stardew Valley.xml

using Microsoft.Xna.Framework;
using StardewValley.Extensions;
using StardewValley.GameData;
using StardewValley.Monsters;
using StardewValley.Projectiles;
using StardewValley.TokenizableStrings;
using System;
using System.Collections.Generic;
using System.Globalization;

#nullable disable
namespace StardewValley.Objects.Trinkets;

/// <summary>Implements the special behavior for a <see cref="T:StardewValley.Objects.Trinkets.Trinket" /> which shoots a damaging projectile at enemies.</summary>
/// <inheritdoc />
public class MagicQuiverTrinketEffect(Trinket trinket) : TrinketEffect(trinket)
{
  /// <summary>The backing field for <see cref="M:StardewValley.Objects.Trinkets.MagicQuiverTrinketEffect.GetIgnoredLocations" />.</summary>
  public static HashSet<string> CachedIgnoreLocations;
  /// <summary>The backing field for <see cref="M:StardewValley.Objects.Trinkets.MagicQuiverTrinketEffect.GetIgnoredMonsterNames" />.</summary>
  public static HashSet<string> CachedIgnoreMonsters;
  /// <summary>The pixel range at which monsters can be targeted.</summary>
  public const int Range = 500;
  /// <summary>The number of milliseconds until the trinket next shoots a projectile.</summary>
  public float ProjectileTimer;
  /// <summary>The number of milliseconds between each projectile.</summary>
  public float ProjectileDelay = 1000f;
  /// <summary>The minimum damage that can be dealt to monsters.</summary>
  public int MinDamage = 10;
  /// <summary>The minimum damage that can be dealt to monsters.</summary>
  public int MaxDamage = 10;

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
    if (random.NextBool(0.04))
    {
      trinket.displayNameOverrideTemplate.Value = TokenStringBuilder.LocalizedText("Strings\\1_6_Strings:PerfectMagicQuiver");
      this.MinDamage = 30;
      this.MaxDamage = 35;
      this.ProjectileDelay = 900f;
    }
    else if (random.NextBool(0.1))
    {
      if (random.NextBool(0.5))
      {
        trinket.displayNameOverrideTemplate.Value = TokenStringBuilder.LocalizedText("Strings\\1_6_Strings:RapidMagicQuiver");
        this.MinDamage = random.Next(10, 15);
        this.MinDamage -= 2;
        this.MaxDamage = this.MinDamage + 5;
        this.ProjectileDelay = (float) (600 + random.Next(11) * 10);
      }
      else
      {
        trinket.displayNameOverrideTemplate.Value = TokenStringBuilder.LocalizedText("Strings\\1_6_Strings:HeavyMagicQuiver");
        this.MinDamage = random.Next(25, 41);
        this.MinDamage -= 2;
        this.MaxDamage = this.MinDamage + 5;
        this.ProjectileDelay = (float) (1500 + random.Next(6) * 100);
      }
    }
    else
    {
      this.MinDamage = random.Next(15, 31 /*0x1F*/);
      this.MinDamage -= 2;
      this.MaxDamage = this.MinDamage + 5;
      this.ProjectileDelay = (float) (1100 + random.Next(11) * 100);
    }
    trinket.descriptionSubstitutionTemplates.Clear();
    trinket.descriptionSubstitutionTemplates.Add(Math.Round((double) this.ProjectileDelay / 1000.0, 2).ToString((IFormatProvider) CultureInfo.InvariantCulture));
    trinket.descriptionSubstitutionTemplates.Add(this.MinDamage.ToString());
    trinket.descriptionSubstitutionTemplates.Add(this.MaxDamage.ToString());
    return true;
  }

  /// <inheritdoc />
  public override void Update(Farmer farmer, GameTime time, GameLocation location)
  {
    base.Update(farmer, time, location);
    if (!Game1.shouldTimePass())
      return;
    this.ProjectileTimer += (float) time.ElapsedGameTime.TotalMilliseconds;
    if ((double) this.ProjectileTimer < (double) this.ProjectileDelay)
      return;
    this.ProjectileTimer = 0.0f;
    HashSet<string> ignoredLocations = this.GetIgnoredLocations();
    if (ignoredLocations.Contains(location.NameOrUniqueName) || ignoredLocations.Contains(location.Name))
      return;
    HashSet<string> ignoreMonsterNames = this.GetIgnoredMonsterNames();
    Monster monsterWithinRange = Utility.findClosestMonsterWithinRange(location, farmer.getStandingPosition(), 500, true, (Func<Monster, bool>) (m => !ignoreMonsterNames.Contains(m.Name)));
    if (monsterWithinRange == null)
      return;
    Vector2 velocityTowardPoint = Utility.getVelocityTowardPoint(farmer.getStandingPosition(), monsterWithinRange.getStandingPosition(), 2f);
    float num = (float) Math.Atan2((double) velocityTowardPoint.Y, (double) velocityTowardPoint.X) + 1.57079637f;
    BasicProjectile basicProjectile = new BasicProjectile(Game1.random.Next(this.MinDamage, this.MaxDamage + 1), 16 /*0x10*/, 0, 0, 0.0f, velocityTowardPoint.X, velocityTowardPoint.Y, farmer.getStandingPosition() - new Vector2(32f, 48f), damagesMonsters: true, location: location, firer: (Character) farmer);
    basicProjectile.IgnoreLocationCollision = true;
    basicProjectile.ignoreObjectCollisions.Value = true;
    basicProjectile.acceleration.Value = velocityTowardPoint;
    basicProjectile.maxVelocity.Value = 24f;
    basicProjectile.projectileID.Value = 14;
    basicProjectile.startingRotation.Value = num;
    basicProjectile.alpha.Value = 1f / 1000f;
    basicProjectile.alphaChange.Value = 0.05f;
    basicProjectile.light.Value = true;
    basicProjectile.collisionSound.Value = "magic_arrow_hit";
    location.projectiles.Add((Projectile) basicProjectile);
    location.playSound("magic_arrow");
  }

  /// <summary>Get the locations which magic quivers should ignore.</summary>
  public HashSet<string> GetIgnoredLocations()
  {
    if (MagicQuiverTrinketEffect.CachedIgnoreLocations == null)
    {
      TrinketData trinketData = this.Trinket.GetTrinketData();
      string input;
      if (trinketData == null)
      {
        input = (string) null;
      }
      else
      {
        Dictionary<string, string> customFields = trinketData.CustomFields;
        input = customFields != null ? customFields.GetValueOrDefault<string, string>("IgnoreLocations") : (string) null;
      }
      MagicQuiverTrinketEffect.CachedIgnoreLocations = new HashSet<string>((IEnumerable<string>) ArgUtility.SplitQuoteAware(input, '/'), (IEqualityComparer<string>) StringComparer.OrdinalIgnoreCase);
    }
    return MagicQuiverTrinketEffect.CachedIgnoreLocations;
  }

  /// <summary>Get the monsters which magic quivers should ignore.</summary>
  public HashSet<string> GetIgnoredMonsterNames()
  {
    if (MagicQuiverTrinketEffect.CachedIgnoreMonsters == null)
    {
      TrinketData trinketData = this.Trinket.GetTrinketData();
      string input;
      if (trinketData == null)
      {
        input = (string) null;
      }
      else
      {
        Dictionary<string, string> customFields = trinketData.CustomFields;
        input = customFields != null ? customFields.GetValueOrDefault<string, string>("IgnoreMonsters") : (string) null;
      }
      MagicQuiverTrinketEffect.CachedIgnoreMonsters = new HashSet<string>((IEnumerable<string>) ArgUtility.SplitQuoteAware(input, '/'), (IEqualityComparer<string>) StringComparer.OrdinalIgnoreCase);
    }
    return MagicQuiverTrinketEffect.CachedIgnoreMonsters;
  }
}
