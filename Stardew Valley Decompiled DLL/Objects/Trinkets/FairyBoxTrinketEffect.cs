// Decompiled with JetBrains decompiler
// Type: StardewValley.Objects.Trinkets.FairyBoxTrinketEffect
// Assembly: Stardew Valley, Version=1.6.15.24356, Culture=neutral, PublicKeyToken=null
// MVID: AA1F513A-94F0-4EF6-A35F-2D5B4A3721BF
// Assembly location: D:\SteamLibrary\steamapps\common\Stardew Valley\Stardew Valley.dll
// XML documentation location: D:\SteamLibrary\steamapps\common\Stardew Valley\Stardew Valley.xml

using Microsoft.Xna.Framework;
using StardewValley.Companions;
using StardewValley.Extensions;
using StardewValley.Monsters;
using System;

#nullable disable
namespace StardewValley.Objects.Trinkets;

/// <summary>Implements the special behavior for a <see cref="T:StardewValley.Objects.Trinkets.Trinket" /> which summons a fairy which heals the player.</summary>
/// <inheritdoc />
public class FairyBoxTrinketEffect(Trinket trinket) : TrinketEffect(trinket)
{
  /// <summary>The number of milliseconds until the fairy next heals the player.</summary>
  public float HealTimer;
  /// <summary>The number of milliseconds between each heal.</summary>
  public float HealDelay = 4000f;
  /// <summary>The power rating applied to the heal amount.</summary>
  public float Power = 0.25f;
  /// <summary>The amount of damage taken by the player since the last heal.</summary>
  public int DamageSinceLastHeal;

  /// <inheritdoc />
  public override bool GenerateRandomStats(Trinket trinket)
  {
    Random random = Utility.CreateRandom((double) trinket.generationSeed.Value);
    int num = 1;
    if (random.NextBool(0.45))
      num = 2;
    else if (random.NextBool(0.25))
      num = 3;
    else if (random.NextBool(0.125))
      num = 4;
    else if (random.NextBool(27.0 / 400.0))
      num = 5;
    this.HealDelay = (float) (5000 - num * 300);
    this.Power = (float) (0.699999988079071 + (double) num * 0.10000000149011612);
    trinket.descriptionSubstitutionTemplates.Clear();
    trinket.descriptionSubstitutionTemplates.Add(num.ToString());
    return true;
  }

  /// <inheritdoc />
  public override void OnDamageMonster(
    Farmer farmer,
    Monster monster,
    int damageAmount,
    bool isBomb,
    bool isCriticalHit)
  {
    this.DamageSinceLastHeal += damageAmount;
    base.OnDamageMonster(farmer, monster, damageAmount, isBomb, isCriticalHit);
  }

  /// <inheritdoc />
  public override void OnReceiveDamage(Farmer farmer, int damageAmount)
  {
    this.DamageSinceLastHeal += damageAmount;
    base.OnReceiveDamage(farmer, damageAmount);
  }

  /// <inheritdoc />
  public override void Update(Farmer farmer, GameTime time, GameLocation location)
  {
    this.HealTimer += (float) time.ElapsedGameTime.TotalMilliseconds;
    if ((double) this.HealTimer >= (double) this.HealDelay)
    {
      if (farmer.health < farmer.maxHealth && this.DamageSinceLastHeal >= 0)
      {
        int num = (int) ((double) (int) Math.Min(Math.Pow((double) this.DamageSinceLastHeal, 0.33000001311302185), (double) farmer.maxHealth / 10.0) * (double) this.Power);
        int number = num + Game1.random.Next((int) ((double) -num * 0.25), (int) ((double) num * 0.25) + 1);
        if (number > 0)
        {
          farmer.health = Math.Min(farmer.maxHealth, farmer.health + number);
          location.debris.Add(new Debris(number, farmer.getStandingPosition(), Color.Lime, 1f, (Character) farmer));
          Game1.playSound("fairy_heal");
          this.DamageSinceLastHeal = 0;
        }
      }
      this.HealTimer = 0.0f;
    }
    base.Update(farmer, time, location);
  }

  /// <inheritdoc />
  public override void Apply(Farmer farmer)
  {
    this.HealTimer = 0.0f;
    this.DamageSinceLastHeal = 0;
    this.Companion = (Companion) new FlyingCompanion(0);
    if (Game1.gameMode == (byte) 3)
      farmer.AddCompanion(this.Companion);
    base.Apply(farmer);
  }

  /// <inheritdoc />
  public override void Unapply(Farmer farmer) => farmer.RemoveCompanion(this.Companion);
}
