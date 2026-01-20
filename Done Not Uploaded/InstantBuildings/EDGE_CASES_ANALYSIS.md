# InstantBuildings Mod - Edge Case Analysis

## Overview
This document analyzes all edge cases discovered by comparing the mod's implementation with Stardew Valley 1.6.15 decompiled code.

---

## Critical Edge Cases Found

### 1. Festival Day Construction Block
**Game Behavior:**
```csharp
// Buildings.Building.cs dayUpdate()
if (this.daysOfConstructionLeft.Value > 0 && !Utility.isFestivalDay(dayOfMonth, Game1.season) && (!Game1.isGreenRain || Game1.year > 1))
```

**Issue:** Mod completes buildings even on festival days, breaking Robin's behavior.

**Fix:** Check `Utility.isFestivalDay()` before completing.

---

### 2. Green Rain Year 1 Block
**Game Behavior:**
Robin doesn't work during Green Rain in year 1 (prevents construction progress).

**Issue:** Mod ignores this condition.

**Fix:** Check `Game1.isGreenRain && Game1.year == 1` and skip completion.

---

### 3. House Upgrade Implementation Wrong
**Game Behavior:**
```csharp
// Buildings.Building.cs
if (this.daysUntilUpgrade.Value == 1)
    this.FinishConstruction();
```

House upgrades use the building's `daysUntilUpgrade` property, not a separate player property.

**Issue:** Mod manually manipulates `farmer.HouseUpgradeLevel` instead of using the building upgrade system. This bypasses:
- Building's `OnUpgraded()` callback
- Proper upgrade completion flow
- Building data synchronization

**Fix:** Let the house building handle its own upgrade via `FinishConstruction()`.

---

### 4. Community Upgrade Mail Logic
**Game Behavior:**
```csharp
// Town.cs DayUpdate()
if (this.daysUntilCommunityUpgrade.Value > 0)
{
    --this.daysUntilCommunityUpgrade.Value;
    if (this.daysUntilCommunityUpgrade.Value <= 0)
    {
        if (!Game1.MasterPlayer.mailReceived.Contains("pamHouseUpgrade"))
        {
            Game1.player.team.RequestSetMail(PlayerActionTarget.Host, "pamHouseUpgrade", MailType.Received, true);
            Game1.player.changeFriendship(1000, Game1.getCharacterFromName("Pam"));
        }
        else
            Game1.player.team.RequestSetMail(PlayerActionTarget.Host, "communityUpgradeShortcuts", MailType.Received, true);
    }
}
```

**Issue:** Mod checks `if (value == 3)` before adding mail. If the value is not exactly 3 when checked (e.g., started at 2, or already counting down), mail won't be added. Also uses `mailReceived.Add()` instead of `RequestSetMail()` (multiplayer-safe method).

**Fix:** Check which upgrade is in progress, not the timer value. Use `RequestSetMail()`.

---

### 5. UpdateUnderConstruction() Never Called
**Game Behavior:**
```csharp
// NetWorldState.cs
public void MarkUnderConstruction(string builderName, Building building)
{
    this.builders[building] = builderName;
}
```

Game maintains a `builders` dictionary. `UpdateUnderConstruction()` cleans it up.

**Issue:** Mod prevents `MarkUnderConstruction()` from running, so builders dictionary is never populated or cleaned. Could cause multiplayer sync issues.

**Fix:** Manually update the builders dictionary or call `UpdateUnderConstruction()`.

---

### 6. Building Move Ignored
**Game Behavior:**
```csharp
// PetBowl.cs (example)
if (this.isMoving || this.isUnderConstruction())
    return;
```

Game checks `isMoving` separately from `isUnderConstruction()`.

**Issue:** Mod doesn't check if building is being moved, might complete during moves.

**Fix:** Check `building.isMoving` before completing.

---

### 7. Missing Location Null Checks
**Issue:** `Game1.warpCharacter(builder, "ScienceHouse", new Vector2(8, 18))` doesn't check if ScienceHouse is loaded.

**Fix:** Add null checks before warping.

---

## Implementation Strategy

### Option A: Minimal Fixes (Current Approach)
Keep the Prefix patch but add:
- Festival day checks
- Green Rain year 1 checks
- Building move checks
- Proper community upgrade logic

**Pros:** Fastest instant completion
**Cons:** Bypasses some game systems, may have hidden issues

### Option B: Postfix + DayUpdate Manipulation (Recommended)
Use Postfix on `MarkUnderConstruction()` to:
1. Let game add building to builders dictionary (multiplayer sync)
2. Set `daysOfConstructionLeft = 1` or `daysUntilUpgrade = 1`
3. Immediately trigger `building.dayUpdate()` with proper checks

**Pros:** 
- Uses game's completion flow
- Respects all conditions
- Proper callbacks and mail
- Better multiplayer support

**Cons:** Slightly more complex

---

## Recommended Fixes

### Priority 1: Critical Gameplay Fixes
1. ✅ Add festival day check
2. ✅ Add Green Rain year 1 check
3. ✅ Fix house upgrade to use building system
4. ✅ Fix community upgrade mail logic

### Priority 2: Robustness Fixes
5. ✅ Add building move check
6. ✅ Add location null checks
7. ✅ Ensure UpdateUnderConstruction() is called or builders dict is synced

### Priority 3: Nice-to-Have
8. Consider adding config option to respect festival days (toggle)
9. Add hotkey to toggle instant building on/off
10. Add HUD indicator showing active status

---

## Testing Checklist
- [ ] Purchase building on festival day (should it complete?)
- [ ] Purchase building during Green Rain year 1 (should it complete?)
- [ ] Upgrade house (check mail, achievements, building sync)
- [ ] Purchase Pam's house upgrade (check mail and friendship)
- [ ] Purchase shortcuts upgrade (check mail)
- [ ] Move a building while under construction
- [ ] Multiplayer: Host purchases building
- [ ] Multiplayer: Client purchases building
- [ ] Robin's schedule after instant completion
- [ ] Multiple simultaneous constructions

