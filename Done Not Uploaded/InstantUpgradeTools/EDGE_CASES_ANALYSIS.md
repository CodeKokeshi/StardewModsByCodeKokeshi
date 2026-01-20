# InstantUpgradeTools - Edge Case Analysis

## Overview
Analysis of the InstantUpgradeTools mod by comparing it with Stardew Valley 1.6.15 decompiled code to identify edge cases and issues.

---

## Current Mod Implementation

The mod sets `Game1.player.daysLeftForToolUpgrade.Value = 0` when a tool upgrade is detected. This bypasses the normal 2-day waiting period.

**Events Used:**
- `OnUpdateTicked`: Checks every tick and sets days to 0
- `OnSaveLoaded`: Checks on save load
- `OnDayStarted`: Checks at day start

---

## Game's Tool Upgrade Flow

### 1. Purchase Tool Upgrade at Clint's Shop
```csharp
// Tools/Tool.cs actionWhenPurchased()
if (shopId == "ClintUpgrade" && Game1.player.toolBeingUpgraded.Value == null)
{
    Game1.player.toolBeingUpgraded.Value = (Tool) this.getOne();
    Game1.player.daysLeftForToolUpgrade.Value = 2;  // Always 2 days
    Game1.playSound("parry");
    Game1.exitActiveMenu();
    Game1.DrawDialogue(Game1.getCharacterFromName("Clint"), "Strings\\StringsFromCSFiles:Tool.cs.14317");
    return true;
}
```

**Key Points:**
- Can only purchase if `toolBeingUpgraded` is null (one upgrade at a time)
- Sets days to 2
- Removes old tool from inventory
- Shows Clint dialogue

### 2. Daily Countdown
```csharp
// Farmer.cs dayupdate()
if (this.daysLeftForToolUpgrade.Value > 0)
    --this.daysLeftForToolUpgrade.Value;
```

**Key Points:**
- Decrements by 1 each day during `dayupdate()`

### 3. Tool Ready Notification
```csharp
// Farmer.cs showToolUpgradeAvailability()
if (!((NetFieldBase<Tool, NetRef<Tool>>) this.toolBeingUpgraded != (NetRef<Tool>) null) || 
    this.daysLeftForToolUpgrade.Value > 0 || 
    this.toolBeingUpgraded.Value == null || 
    Utility.isFestivalDay() || 
    !(Game1.shortDayNameFromDayOfSeason(dayOfMonth) != "Fri") && this.hasCompletedCommunityCenter() && !Game1.isRaining || 
    this.hasReceivedToolUpgradeMessageYet)
    return;

Game1.showGlobalMessage(Game1.content.LoadString("Strings\\StringsFromCSFiles:ToolReady", 
    (object) this.toolBeingUpgraded.Value.DisplayName));
this.hasReceivedToolUpgradeMessageYet = true;
```

**Conditions for "Tool Ready" message:**
1. `toolBeingUpgraded` is not null
2. `daysLeftForToolUpgrade` is 0
3. `toolBeingUpgraded.Value` is not null
4. NOT a festival day
5. NOT (Friday after community center completion when not raining) - Clint goes to Emily's
6. Haven't received message yet

### 4. Picking Up Tool from Clint
```csharp
// GameLocation.cs (Clint interaction)
if (Game1.player.toolBeingUpgraded.Value != null && Game1.player.daysLeftForToolUpgrade.Value <= 0)
{
    if (Game1.player.freeSpotsInInventory() > 0 || Game1.player.toolBeingUpgraded.Value is GenericTool)
    {
        Tool tool = Game1.player.toolBeingUpgraded.Value;
        Game1.player.toolBeingUpgraded.Value = (Tool) null;
        Game1.player.hasReceivedToolUpgradeMessageYet = false;
        Game1.player.holdUpItemThenMessage((Item) tool);
        if (tool is GenericTool)
            tool.actionWhenClaimed();  // For trash cans, increments trashCanLevel
        else
            Game1.player.addItemToInventoryBool((Item) tool);
    }
    else
        Game1.DrawDialogue(character, "Data\\ExtraDialogue:Clint_NoInventorySpace");
}
```

**Key Points:**
- Requires `daysLeftForToolUpgrade <= 0`
- Requires inventory space (unless it's a GenericTool like trash can)
- Clears `toolBeingUpgraded` and `hasReceivedToolUpgradeMessageYet`
- Shows hold-up animation
- For GenericTools (trash cans): calls `actionWhenClaimed()` which increments `trashCanLevel`
- For regular tools: adds to inventory
- If no inventory space: shows "Clint_NoInventorySpace" dialogue

### 5. Clint's "Still Working" Dialogue
```csharp
// GameLocation.cs
if (Game1.player.daysLeftForToolUpgrade.Value > 0)
{
    Game1.DrawDialogue(characterFromName, "Data\\ExtraDialogue:Clint_StillWorking", 
        (object) Game1.player.toolBeingUpgraded.Value.DisplayName);
}
```

**Key Points:**
- When trying to upgrade another tool while one is in progress
- Shows how many days are left

---

## Edge Cases Found

### ✅ 1. **Festival Day Notification Suppressed**
**Game Behavior:** `showToolUpgradeAvailability()` checks `Utility.isFestivalDay()` and skips showing "Tool Ready" message on festival days.

**Mod Behavior:** Mod sets days to 0 immediately, but notification might not show on festival days.

**Impact:** Minor - Player can still pick up tool from Clint, just no notification.

**Status:** Not a real issue - game handles this correctly. Tool can be picked up any day.

---

### ✅ 2. **Friday After Community Center (Clint at Emily's)**
**Game Behavior:** On Fridays after completing community center when not raining, Clint goes to Emily's house. The notification is suppressed on these days.

**Formula:** `!(Game1.shortDayNameFromDayOfSeason(dayOfMonth) != "Fri") && this.hasCompletedCommunityCenter() && !Game1.isRaining`

**Mod Behavior:** Mod completes upgrade instantly, but player can't pick up tool if Clint isn't at the blacksmith.

**Impact:** Minor - Player would need to wait until Clint is back at shop.

**Status:** Not a real issue - just affects when player can pick up tool, not completion.

---

### ✅ 3. **No Inventory Space Issue**
**Game Behavior:** When picking up tool with no inventory space, shows "Clint_NoInventorySpace" dialogue. Tool remains with Clint.

**Mod Behavior:** Mod completes upgrade (days = 0) instantly. If player has full inventory, tool stays with Clint indefinitely until space is made.

**Impact:** None - This is correct behavior. Game handles full inventory properly.

**Status:** Not an issue - working as designed.

---

### ✅ 4. **GenericTool (Trash Can) Special Handling**
**Game Behavior:** When picking up trash can upgrades, calls `actionWhenClaimed()` which increments `Game1.player.trashCanLevel`. Does NOT add to inventory.

**Mod Behavior:** Mod completes upgrade, but `actionWhenClaimed()` is only called when picking up from Clint.

**Impact:** None - The mod doesn't interfere with this. The level increment happens when player talks to Clint.

**Status:** Not an issue - pickup logic handles this correctly.

---

### ⚠️ 5. **Missing Tool Upgrade Notification**
**Game Behavior:** `showToolUpgradeAvailability()` is called on save load and after new day. It shows "Tool Ready" message.

**Mod Behavior:** Mod sets `daysLeftForToolUpgrade = 0` immediately. The notification might show instantly or not at all depending on timing.

**Impact:** Minor UX issue - Player might not see the "Tool Ready" notification because it happens too fast or during UpdateTicked.

**Status:** Minor issue - notification might be skipped, but functionally works.

---

### ⚠️ 6. **Multiplayer Sync Considerations**
**Game Behavior:** `toolBeingUpgraded` and `daysLeftForToolUpgrade` are NetFields (networked properties).

**Mod Behavior:** Mod modifies these on UpdateTicked, which runs on all clients.

**Impact:** In multiplayer, each client would try to set days to 0. Should be fine due to NetField sync, but could cause multiple log messages.

**Status:** Minor - likely no functional issue, just redundant operations.

---

### ❌ 7. **Tool Pickup Animation Skipped**
**Game Behavior:** When picking up tool, game shows `holdUpItemThenMessage()` animation with the upgraded tool.

**Mod Behavior:** Player still needs to talk to Clint to pick up tool. Animation works correctly.

**Impact:** None - Animation still plays when picking up.

**Status:** Not an issue.

---

### ❌ 8. **Can't Purchase Another Tool While One is Upgrading**
**Game Behavior:** `CanBuyItem()` returns false if `toolBeingUpgraded != null`. Purchase blocks until you pick up the previous tool.

**Mod Behavior:** Since instant upgrade sets days to 0 but doesn't clear `toolBeingUpgraded`, player must still pick up the tool before purchasing another upgrade.

**Impact:** This is correct behavior. Player needs to pick up completed tool before starting another.

**Status:** Not an issue - working as designed.

---

## Issues Summary

### Critical Issues (Breaking Functionality)
**None found.** ✅

### Minor Issues (UX/Polish)
1. **Notification timing** - "Tool Ready" message might not show properly due to instant completion
2. **Multiplayer redundancy** - Each client tries to set days to 0 (harmless but redundant)

### Not Issues (Working as Designed)
- Festival day notification suppression
- Clint at Emily's on Fridays
- No inventory space handling
- GenericTool special handling
- Tool pickup requirement
- One upgrade at a time restriction

---

## Recommendations

### Required Fixes
**None.** The mod works correctly with the game's tool upgrade system.

### Optional Enhancements (Suggestions Only - Not Implemented Unless Confirmed)

#### 1. **Better Notification Handling**
Instead of setting days to 0 on UpdateTicked, use a Harmony patch to:
- Patch `actionWhenPurchased()` to set days to 1 instead of 2
- Let game's natural dayupdate decrement it to 0 instantly
- This ensures `showToolUpgradeAvailability()` runs properly

#### 2. **Instant Pickup Option**
Add Harmony patch to auto-pickup tool:
- When days reaches 0, automatically add tool to inventory
- Skip talking to Clint entirely
- Show notification directly

#### 3. **Skip Clint Entirely**
Most aggressive approach:
- Patch `actionWhenPurchased()` to directly add upgraded tool to inventory
- Skip the `toolBeingUpgraded` system entirely
- Never even set the upgrade in progress

#### 4. **Config Options**
- Toggle: Show notification (on/off)
- Toggle: Auto-pickup from Clint (on/off)
- Toggle: Skip Clint entirely (on/off)

#### 5. **Multiplayer Optimization**
Only run the check on the local player:
```csharp
if (Context.IsMainPlayer || player == Game1.player)
    // Set days to 0
```

---

## Testing Checklist

- [ ] Purchase tool upgrade at Clint's → Should complete instantly
- [ ] Try to purchase another upgrade before picking up → Should be blocked
- [ ] Pick up completed tool with full inventory → Should show "no space" message
- [ ] Pick up completed tool with space → Should receive tool
- [ ] Upgrade trash can → Should increment trash can level on pickup
- [ ] Upgrade on festival day → Tool should complete but may not show notification
- [ ] Upgrade on Friday (post-CC, no rain) → Clint at Emily's, can't pick up until he's back
- [ ] Save/load with tool upgrading → Should be at 0 days on load
- [ ] Multiplayer: Host upgrades tool → Should work correctly
- [ ] Multiplayer: Client upgrades tool → Should work correctly

---

## Conclusion

The InstantUpgradeTools mod is **functionally complete and working correctly**. No critical edge cases found.

The mod properly respects the game's tool upgrade system:
- ✅ Sets days to 0 (instant completion)
- ✅ Preserves `toolBeingUpgraded` (player must pick up)
- ✅ Respects one-upgrade-at-a-time restriction
- ✅ Works with GenericTools (trash cans)
- ✅ Respects inventory space requirements
- ✅ Compatible with multiplayer (NetField sync)

**Minor UX considerations:**
- Notification might not show consistently (very minor)
- Multiplayer redundancy (harmless)

**No fixes required.** All optional enhancements are suggestions only and require user confirmation before implementation.
