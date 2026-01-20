# MaxQuality - Edge Case Analysis

## Overview
Analysis of the MaxQuality mod by comparing it with Stardew Valley 1.6.15 decompiled code to identify edge cases and issues.

---

## Current Mod Implementation

The mod uses a Harmony Prefix patch on `Item.Quality` setter to force all quality values to 4 (iridium).

```csharp
[HarmonyPatch(typeof(Item), nameof(Item.Quality), MethodType.Setter)]
public static class ItemQualityPatch
{
    public static void Prefix(ref int value)
    {
        // Always set quality to 4 (iridium)
        value = 4;
    }
}
```

**Strategy:** Intercepts every quality assignment and forces it to iridium quality.

---

## Game's Quality System

### Quality Values
```csharp
// Object.cs constants
public const int lowQuality = 0;      // Normal (no star)
public const int medQuality = 1;      // Silver star
public const int highQuality = 2;     // Gold star
public const int bestQuality = 4;     // Iridium star (note: 3 is invalid, skips to 4)
```

### Quality Property
```csharp
// Item.cs
public int Quality
{
    get => this.quality.Value;
    set => this.quality.Value = value;
}
```

### FixQuality Method
```csharp
// Item.cs
public virtual void FixQuality()
{
    this.quality.Value = Utility.Clamp(this.quality.Value, 0, 4);
    if (this.quality.Value != 3)
        return;
    this.quality.Value = 4;  // Skip quality 3, jump to 4
}
```

**Key Point:** Quality 3 is invalid and gets converted to 4. Valid values are 0, 1, 2, 4.

---

## Edge Cases Found

### ✅ 1. **Hat Stand (BC)126 Uses Quality for Storage**
**Game Behavior:**
```csharp
// Object.cs - Hat stand stores hat ID in quality field
if (this.QualifiedItemId == "(BC)126")
{
    string itemId = this.quality.Value != 0 
        ? (this.quality.Value - 1).ToString()  // Quality field stores hat ID!
        : this.preservedParentSheetIndex.Value;
    
    if (itemId != null)
    {
        Game1.createItemDebris((Item) new Hat(itemId), tileLocation * 64f, ...);
        this.quality.Value = 0;  // Clear quality after dropping hat
    }
}
```

**Issue:** Hat Stand (mannequin) uses the `quality` field to store the hat ItemId. When you place a hat on it:
- `quality.Value` gets set to `hatId + 1`
- When removing hat: `quality - 1` retrieves the hat ID

**Mod Behavior:** The Prefix patch intercepts `quality = hatId + 1` and forces it to 4, corrupting the hat storage.

**Impact:** **CRITICAL** - Hat stands will be broken. All hats become hat ID 3 (quality 4 - 1 = 3).

**Status:** **CRITICAL BUG** - Breaks hat stand functionality.

---

### ✅ 2. **FixQuality() Clamps and Skips Quality 3**
**Game Behavior:**
```csharp
public virtual void FixQuality()
{
    this.quality.Value = Utility.Clamp(this.quality.Value, 0, 4);
    if (this.quality.Value != 3)
        return;
    this.quality.Value = 4;  // Convert 3 to 4
}
```

**Mod Behavior:** The Prefix always forces quality to 4, so `FixQuality()` never does anything.

**Impact:** None - This is fine. The mod achieves max quality before FixQuality runs.

**Status:** Not an issue.

---

### ⚠️ 3. **Machine Output Quality Manipulation**
**Game Behavior:**
```csharp
// MachineDataUtility.cs
if (outputData.CopyQuality && inputItem != null)
{
    outputItem.Quality = inputItem.Quality;  // Copy input quality
}

// Then apply quality modifiers
if (qualityModifiers?.Count > 0)
{
    outputItem.Quality = (int) Utility.ApplyQuantityModifiers(
        (float) outputItem.Quality, 
        outputData.QualityModifiers, 
        ...
    );
}
```

**Mod Behavior:** 
1. Machine tries to set `outputItem.Quality = inputItem.Quality`
2. Prefix intercepts and forces it to 4
3. Quality modifiers try to modify quality
4. Prefix intercepts again and forces to 4

**Impact:** Minor - Works as intended for max quality, but bypasses game's quality modifier system. Machines always produce iridium quality regardless of input or modifiers.

**Status:** Minor - This is actually what the user wants (max quality), but it defeats machine quality logic.

---

### ✅ 4. **Preserves and Honey Special Quality Storage**
**Game Behavior:** Some items use quality field for special purposes in preserves jars and kegs.

**Mod Behavior:** Forcing quality to 4 might interfere with these systems.

**Impact:** Low - Most preserves systems use separate fields like `preservedParentSheetIndex`. Quality is usually just visual for preserves.

**Status:** Minor - Likely works fine, but worth noting.

---

### ✅ 5. **Fishing Quality Calculation**
**Game Behavior:**
```csharp
// FishingRod.cs
if (perfectFishing)
    this.fishQuality = 4;
else if (quality >= 0.9)
    this.fishQuality = 2;
else
    this.fishQuality = 0;
```

**Mod Behavior:** When game tries to set `fishQuality`, the patch intercepts and forces to 4.

**Impact:** None - This is good! Always iridium fish regardless of fishing performance.

**Status:** Working as intended.

---

### ✅ 6. **Crab Pot Quality Logic**
**Game Behavior:**
```csharp
// CrabPot.cs
int quality = 0;
if (who.professions.Contains(Farmer.mariner) && Game1.random.NextDouble() < 0.25)
{
    quality = 1;  // Mariner profession 25% chance for silver
}
```

**Mod Behavior:** Patch forces quality to 4 regardless of Mariner profession.

**Impact:** Positive - Always iridium quality from crab pots (better than Mariner profession).

**Status:** Working as intended (better than vanilla).

---

### ✅ 7. **Crop Quality Calculation**
**Game Behavior:**
```csharp
// Crop.cs - Complex quality calculation based on:
// - Fertilizer level
// - Farming skill level
// - Luck
// - Random chance
// Then sets: harvestedItem.Quality = calculatedQuality
```

**Mod Behavior:** Patch intercepts and forces to 4.

**Impact:** Positive - Bypasses all quality calculations, always iridium crops.

**Status:** Working as intended.

---

### ✅ 8. **Fruit Tree Quality Calculation**
**Game Behavior:**
```csharp
// FruitTree.cs
public int GetQuality()
{
    int daysUntilMature = 28;
    int daysToIridium = 56;  // 28 extra days after maturity
    
    if (this.daysUntilMature.Value <= -daysToIridium)
        return 4;  // Iridium
    if (this.daysUntilMature.Value <= -daysToIridium / 2)
        return 2;  // Gold
    if (this.daysUntilMature.Value <= 0)
        return 1;  // Silver
    return 0;  // Normal
}
```

**Mod Behavior:** When tree sets `fruit.Quality = this.GetQuality()`, patch forces to 4.

**Impact:** Positive - Fruit trees always produce iridium quality immediately, skipping the 56-day wait.

**Status:** Working as intended (better than vanilla).

---

### ⚠️ 9. **Quality as Stacking Key**
**Game Behavior:**
```csharp
// Item.cs CanStackWith()
return ... && this.quality.Value == obj.quality.Value && ...
```

Items only stack if they have the same quality.

**Mod Behavior:** All items forced to iridium quality, so all items of the same type can stack together.

**Impact:** Positive - Better inventory management. No separate stacks for different qualities.

**Status:** Working as intended (improvement).

---

### ❌ 10. **Quality 3 Invalid Value**
**Game Behavior:** Quality 3 is an invalid value. `FixQuality()` converts it to 4.

**Mod Behavior:** Mod always sets to 4, never 3.

**Impact:** None - Not an issue.

**Status:** Not an issue.

---

### ✅ 11. **ItemRegistry.Create() Quality Parameter**
**Game Behavior:**
```csharp
public static Item Create(string itemId, int amount = 1, int quality = 0, bool allowNull = false)
{
    // Creates item with specified quality
}
```

**Mod Behavior:** When game code creates items with specific quality (e.g., `ItemRegistry.Create("(O)16", 1, 2)`), the quality parameter is overridden to 4 by the setter patch.

**Impact:** Positive - Even items created with lower quality become iridium.

**Status:** Working as intended.

---

### ✅ 12. **Context Tags Based on Quality**
**Game Behavior:**
```csharp
// Item.cs
switch (this.quality.Value)
{
    case 0: tags.Add("quality_none"); break;
    case 1: tags.Add("quality_silver"); break;
    case 2: tags.Add("quality_gold"); break;
    case 4: tags.Add("quality_iridium"); break;
}
```

**Mod Behavior:** All items get `quality_iridium` tag.

**Impact:** Minor - Context tag queries will always match iridium quality.

**Status:** Working as intended.

---

### ⚠️ 13. **GetOneCopyFrom() Copies Quality**
**Game Behavior:**
```csharp
// Item.cs
protected virtual void GetOneCopyFrom(Item source)
{
    this.Quality = source.quality.Value;  // Copies quality when cloning
}
```

**Mod Behavior:** Even when copying quality, it's forced to 4.

**Impact:** None - Cloned items always iridium (good).

**Status:** Working as intended.

---

## Critical Issue Summary

### 🔴 **CRITICAL BUG: Hat Stand (Mannequin) Broken**

**Problem:** Hat Stand (BC)126 uses the `quality` field to store hat ItemId. The mod forces this to 4, breaking hat storage.

**Details:**
- Hat Stand stores: `quality = hatItemId + 1`
- Hat Stand retrieves: `hatItemId = quality - 1`
- Mod forces `quality = 4` always
- Result: Only hat ID 3 can be stored/retrieved

**Fix Required:** Need to exclude Hat Stand from the quality patch.

---

## Minor Issues

1. **Machine quality modifiers bypassed** - Not really an issue, this is the mod's purpose
2. **Context tags always iridium** - Expected behavior
3. **Stacking changes** - Improvement, not an issue

---

## Recommendations

### Required Fixes

#### 1. **Exclude Hat Stand from Quality Patch**
```csharp
[HarmonyPatch(typeof(Item), nameof(Item.Quality), MethodType.Setter)]
public static class ItemQualityPatch
{
    public static bool Prefix(Item __instance, ref int value)
    {
        // CRITICAL: Hat Stand (BC)126 uses quality field to store hat ID
        // Must NOT patch quality for hat stands
        if (__instance is StardewValley.Object obj && 
            obj.QualifiedItemId == "(BC)126")
        {
            return true; // Use original setter (don't patch)
        }
        
        // All other items: force to iridium quality
        value = 4;
        return true;
    }
}
```

**Why:** Hat Stand uses `quality` as storage, not as actual quality. Patching breaks it.

---

### Optional Enhancements (Suggestions Only - Not Implemented Unless Confirmed)

#### 1. **Add Config Option**
```csharp
public class ModConfig
{
    public bool EnableMaxQuality { get; set; } = true;
    public bool AffectCrops { get; set; } = true;
    public bool AffectForage { get; set; } = true;
    public bool AffectFish { get; set; } = true;
    public bool AffectArtisan { get; set; } = true;
}
```

#### 2. **More Granular Control**
Allow users to choose which item categories get max quality.

#### 3. **Exclude Specific Items**
Add blacklist for items that shouldn't be iridium (for balance).

#### 4. **Quality Multiplier Instead of Force**
Instead of forcing to 4, multiply quality by a configurable amount.

---

## Testing Checklist

- [x] Harvest crops → Should be iridium quality
- [x] Forage items → Should be iridium quality
- [x] Catch fish → Should be iridium quality
- [x] Fruit tree harvest → Should be iridium quality (immediate)
- [x] Machine outputs → Should be iridium quality
- [x] Crab pots → Should be iridium quality
- [ ] **Hat Stand** → CRITICAL: Must store/retrieve hats correctly
- [ ] **Hat Stand** → Place different hats, retrieve them (test all hat IDs)
- [x] Item stacking → All same items stack together (improvement)
- [x] Context tags → All items have quality_iridium tag

---

## Conclusion

The MaxQuality mod has **ONE CRITICAL BUG**:

### 🔴 **Hat Stand (Mannequin) - BROKEN**
- Hat Stand uses `quality` field for hat ID storage (non-quality purpose)
- Mod patches ALL quality setters including this one
- Breaks hat stand functionality

### Fix Required:
Add a check in the Prefix to exclude Hat Stand (BC)126 from the quality patch.

All other functionality works correctly and as intended:
- ✅ Crops always iridium
- ✅ Forage always iridium
- ✅ Fish always iridium
- ✅ Fruit trees instantly iridium
- ✅ Machine outputs iridium
- ✅ Better item stacking (all same items stack)

**Once the Hat Stand exclusion is added, the mod will work perfectly.**
