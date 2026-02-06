# Combat Cheats - Complete Explanation

## What Changed & Why

### 1. **GOD MODE** (Replaces "Infinite HP")

**Before (Infinite HP):**
- Only patched `Farmer.takeDamage` to block damage
- You could still get hit, just took 0 damage
- Wouldn't protect from environmental damage

**After (GOD MODE):**
- Sets `Game1.player.temporarilyInvincible = true` every tick
- This is a NATIVE GAME FLAG that makes enemies not even TRY to hit you!
- Monsters check this flag BEFORE dealing damage
- You literally cannot be touched - it's how the game implements invincibility items

**How It Works:**
```csharp
// Every frame, we force the invincibility flag ON
Game1.player.temporarilyInvincible = true;
Game1.player.temporaryInvincibilityTimer = -1000000; // Never expires
```

**Why This Is Better:**
- Exploits the SAME SYSTEM the game uses for star fruits, invincibility rings, etc.
- Monsters WON'T EVEN TRY to attack you
- Covers ALL damage sources (monsters, projectiles, lava, etc.)

---

### 2. **MAX DROP RATE** - Fixed!

**Before (BROKEN):**
- Used Prefix patch on `GameLocation.monsterDrop`
- Tried to add to `objectsToDrop` BEFORE the game read it
- Only multiplied extra drops list (which is usually empty!)

**After (WORKS):**
- Uses **POSTFIX** patch on `GameLocation.monsterDrop`
- POSTFIX = runs AFTER the original method
- This means drops are already spawned, and we just ADD MORE!

**How It Works:**
```csharp
[HarmonyPostfix] // <-- Runs AFTER original method
[HarmonyPatch(typeof(GameLocation), nameof(GameLocation.monsterDrop))]
public static void GameLocation_monsterDrop_Postfix(Monster monster, int x, int y, Farmer who)
{
    // Drops are ALREADY created by original method
    // Now we add 5x MORE of the same drops!
    for (int i = 0; i < originalDrops.Count * 4; i++)
    {
        location.debris.Add(new Debris(...)); // Add directly to world
    }
}
```

**Why Postfix:**
- Prefix = runs BEFORE original (we were too early!)
- Postfix = runs AFTER original (perfect timing!)
- We can now spawn as many extra drops as we want

---

### 3. **ARTIFACT DROPS** - Like Fling Trainer!

**What Fling Trainer Does:**
- When monsters die, they randomly drop artifacts
- Dwarven Scrolls, Ancient Seeds, Fossils, etc.
- This is NOT normally possible!

**How We Do It:**
```csharp
if (Config.DropArtifacts && Game1.random.NextDouble() < 0.15) // 15% chance
{
    var artifacts = new List<string> { "96", "97", "98", "99", ... }; // All artifacts
    string artifactId = Game1.random.ChooseFrom(artifacts);
    
    // Spawn it as debris (item drop)
    location.debris.Add(new Debris(artifactId, position, playerPos));
}
```

**Artifact List Includes:**
- All 4 Dwarven Scrolls (96-99)
- Ancient Seed (114)
- Dinosaur Egg (107)
- ALL fossils (579-589)
- Rare collectibles (Golden Mask, Strange Dolls, etc.)

**Why 15% Chance:**
- Not too common (you'd fill inventory instantly)
- But common enough to feel rewarding
- Fling trainer uses similar rates

---

### 4. **LUCK MULTIPLIER**

**What It Does:**
- Boosts `Game1.player.team.sharedDailyLuck`
- This affects:
  - Drop rates from monsters
  - Quality of drops
  - Rare item chances
  - Critical hit chances
  - Finding ladders in mines

**How It Works:**
```csharp
// Every tick, boost luck to minimum value
Game1.player.team.sharedDailyLuck.Value = Math.Max(
    currentLuck,
    0.1f * Config.LuckMultiplier
);
```

**Why This Helps Drops:**
- Many drop calculations use `AverageDailyLuck()` in formulas
- Higher luck = better chances for rare drops
- Stacks with Max Drop Rate for INSANE loot

---

## Summary Table

| Feature | Method | Why It Works |
|---------|--------|--------------|
| **God Mode** | Set `temporarilyInvincible` flag | Uses game's built-in invincibility system |
| **Max Drops** | Postfix on `monsterDrop` | Adds drops AFTER game spawns them |
| **Artifacts** | Spawn debris in postfix | Directly creates item drops in world |
| **Luck** | Modify `sharedDailyLuck` | Affects all RNG calculations |

---

## Why These Are Better Than Old Versions

1. **God Mode vs Infinite HP:** Actually prevents hits instead of just blocking damage
2. **Max Drops:** Actually works now (postfix timing)
3. **Artifacts:** Completely new feature (like Fling trainer)
4. **Luck:** New feature that affects ALL aspects of the game

---

## Technical Notes

### Harmony Patch Types:
- **Prefix:** Runs BEFORE original method (can cancel it)
- **Postfix:** Runs AFTER original method (can modify results)
- **Transpiler:** Rewrites the method's IL code (advanced)

### Why We Use Each:
- **One Hit Kill:** Prefix (modify damage BEFORE calculation)
- **100% Crit:** Prefix (set crit chance BEFORE attack)
- **Max Drops:** Postfix (add drops AFTER they're spawned)
- **God Mode:** Update loop (set flag every frame)
