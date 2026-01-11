# Automated Farming Mod - Feasibility Analysis

## 📋 Executive Summary

**Verdict: ✅ HIGHLY FEASIBLE**

Based on analyzing the Stardew Valley DLL files and researching the modding scene, creating an automated farming mod is definitely possible. There are two main approaches, and I recommend a hybrid approach.

---

## 🤖 Approach Analysis

### Option A: Auto-Player (Player automatically performs actions)
**Similar to: Tractor Mod**

#### How it would work:
- Hook into `UpdateTicked` event
- Scan tiles around the player
- Automatically perform tool actions based on what's on each tile
- Use `Tool.DoFunction()` to perform actions

#### Pros:
- Simpler to implement
- Player sees all the action
- Easier to configure (toggle on/off)

#### Cons:
- Player is stuck in place while automation runs
- Consumes player stamina (unless using NoStamina-style patch)

---

### Option B: Robot NPC (New entity does the work)
**Similar to: JunimoHarvester (but way more capable)**

#### How it would work:
- Create a custom NPC class extending `Character` or `NPC`
- Robot spawns from a craftable building/object
- Has its own AI, pathfinding, and task queue
- Works independently while player does other things
- Stores items in a connected chest

#### Pros:
- Player is free to do other things
- More immersive/fun
- Doesn't consume player stamina
- Can be visually cool (robot sprite)

#### Cons:
- Much more complex to implement
- Pathfinding challenges
- Need custom sprites/assets

---

### Option C: HYBRID (Recommended) ⭐

**Invisible helper that performs tasks in an area, with optional robot sprite**

#### How it would work:
1. Player builds a **"Farm Bot Station"** (craftable building)
2. Bot Station has a configurable radius
3. Every few seconds, the bot automatically:
   - Cuts weeds → Items go to chest
   - Breaks stones → Items go to chest
   - Chops tree stumps → Items go to chest
   - Hoes ground if seeds are in chest
   - Plants seeds from chest
   - Waters crops
   - Harvests ready crops → Items go to chest
4. Optional: Visible robot sprite that moves around the area

---

## 🔍 Key APIs from DLL Analysis

### 1. TerrainFeature Manipulation
```csharp
// From GameLocation.cs
public readonly NetVector2Dictionary<TerrainFeature, NetRef<TerrainFeature>> terrainFeatures;

// Trees, Grass, HoeDirt, etc. are all TerrainFeatures
// Can use performToolAction() to interact:
terrainFeature.performToolAction(tool, damage, tileLocation);
```

### 2. Object Interaction
```csharp
// From Object.cs - for weeds, stones, twigs
public virtual bool performToolAction(Tool t)

// Categories (from Object.cs):
public const int litterCategory = -999;  // Weeds, stones, twigs
```

### 3. Tool Functions
```csharp
// From Game1.cs - how tools actually work:
who.CurrentTool.DoFunction(currentLocation, (int)actionTile.X, (int)actionTile.Y, powerupLevel, who);

// Tools in StardewValley.Tools namespace:
// - Axe
// - Pickaxe  
// - Hoe
// - WateringCan
// - Scythe
```

### 4. Stamina Control
```csharp
// From your NoStamina mod - we can patch this!
[HarmonyPatch(typeof(Farmer), nameof(Farmer.Stamina), MethodType.Setter)]
// Or create a "ghost farmer" that isn't the real player
```

### 5. JunimoHarvester Reference (for robot AI)
```csharp
// From NPC.cs - Junimos are characters that can work independently
[XmlInclude(typeof(JunimoHarvester))]

// From GameLocation.cs - they work on terrain features
if (!(character is FarmAnimal) && !(character is JunimoHarvester))
```

### 6. Chest/Inventory Integration
```csharp
// From FarmerTeam.cs - Junimo Chest global storage
public const string GlobalInventoryId_JunimoChest = "JunimoChests";
public readonly NetStringDictionary<Inventory, NetRef<Inventory>> globalInventories;

// Can also just use regular Chest objects
```

---

## 🎮 Existing Mods in the Scene

### What Already Exists:
| Mod | What it does | Gap |
|-----|--------------|-----|
| **Automate** | Connects machines to chests, auto-processes | ❌ No terrain clearing |
| **Tractor Mod** | Player drives tractor, does multi-tile actions | ❌ Player-controlled, not autonomous |
| **Better Junimos** | Junimos can plant, water, fertilize | ❌ No tree cutting, stone breaking, limited radius |

### What's Missing (Our Opportunity):
- ✅ **Autonomous** ground clearing (weeds, stones, sticks)
- ✅ **Autonomous** tree/stump removal
- ✅ **Autonomous** hoeing & planting (based on chest contents)
- ✅ **Autonomous** stone/boulder breaking
- ✅ Works **outside** of Junimo Hut radius
- ✅ **All-in-one** solution

---

## 📐 Proposed Mod Architecture

### Core Classes:

```
AutomatedFarming/
├── ModEntry.cs                 # SMAPI entry point
├── Config/
│   └── ModConfig.cs            # User configuration
├── Core/
│   ├── FarmBot.cs              # Main automation logic
│   ├── TaskScanner.cs          # Scans for work to do
│   └── TaskExecutor.cs         # Executes farming tasks
├── Tasks/
│   ├── IFarmTask.cs            # Task interface
│   ├── ClearDebrisTask.cs      # Weeds, sticks, stones
│   ├── ChopTreeTask.cs         # Trees and stumps
│   ├── BreakRockTask.cs        # Rocks and boulders
│   ├── TillSoilTask.cs         # Hoeing ground
│   ├── PlantSeedTask.cs        # Planting seeds
│   ├── WaterCropTask.cs        # Watering
│   └── HarvestTask.cs          # Harvesting crops
├── Objects/
│   ├── FarmBotStation.cs       # The craftable station
│   └── FarmBotNPC.cs           # Optional visible robot
└── Integration/
    └── ChestManager.cs         # Input/output chest handling
```

### Config Options:
```csharp
public class ModConfig
{
    // General
    public bool Enabled { get; set; } = true;
    public int WorkRadius { get; set; } = 15;  // tiles
    public int TicksBetweenActions { get; set; } = 30;  // ~0.5 seconds
    
    // Tasks Toggle
    public bool ClearWeeds { get; set; } = true;
    public bool ClearStones { get; set; } = true;
    public bool ClearSticks { get; set; } = true;
    public bool ChopTrees { get; set; } = true;
    public bool ChopStumps { get; set; } = true;
    public bool BreakRocks { get; set; } = true;
    public bool TillSoil { get; set; } = true;
    public bool PlantSeeds { get; set; } = true;
    public bool WaterCrops { get; set; } = true;
    public bool HarvestCrops { get; set; } = true;
    
    // Visual
    public bool ShowRobotSprite { get; set; } = true;
    public bool ShowWorkingParticles { get; set; } = true;
}
```

---

## 🛠️ Implementation Strategy

### Phase 1: Basic Automation (MVP)
1. Create mod structure
2. Implement area scanning on UpdateTicked
3. Clear weeds/stones/sticks automatically
4. Drop items on ground (simplest output)

### Phase 2: Chest Integration
1. Add craftable Farm Bot Station
2. Connect to nearby chest for item storage
3. Pull seeds from chest for planting

### Phase 3: Full Farming
1. Add hoeing/tilling capability
2. Add seed planting
3. Add crop watering
4. Add harvesting

### Phase 4: Visual Robot (Optional)
1. Create robot sprite
2. Add FarmBotNPC class
3. Simple pathfinding between tasks
4. Animations for different actions

---

## 📝 Key Code Patterns to Use

### 1. Scanning for Objects to Clear
```csharp
foreach (var pair in location.objects.Pairs)
{
    var obj = pair.Value;
    var tile = pair.Key;
    
    // Check if it's debris (weed/stone/twig)
    if (obj.Category == StardewValley.Object.litterCategory)
    {
        // Clear it!
        obj.performToolAction(GetAppropriateToolFor(obj));
        location.objects.Remove(tile);
        // Drop items or add to chest
    }
}
```

### 2. Scanning for Trees
```csharp
foreach (var pair in location.terrainFeatures.Pairs)
{
    if (pair.Value is Tree tree)
    {
        // Check if fully grown
        if (tree.growthStage.Value >= 5)
        {
            // Chop it
            tree.performToolAction(axe, 0, pair.Key);
        }
    }
}
```

### 3. Hoeing Ground
```csharp
// Check if tile is tillable
if (location.doesTileHaveProperty((int)tile.X, (int)tile.Y, "Diggable", "Back") != null)
{
    // Create hoe dirt
    location.terrainFeatures.Add(tile, new HoeDirt());
}
```

### 4. Planting Seeds
```csharp
if (location.terrainFeatures.TryGetValue(tile, out var feature) && feature is HoeDirt dirt)
{
    if (dirt.crop == null)
    {
        // Plant seed from chest
        dirt.plant(seedItem.ItemId, tile, location, Game1.player, isFertilizer: false);
    }
}
```

---

## ⚠️ Potential Challenges

1. **Multiplayer Sync** - Need to use Net* types for multiplayer compatibility
2. **Performance** - Don't scan every tick, use intervals
3. **Tool Power** - Some rocks need upgraded tools
4. **Pathfinding** - For visual robot, use game's pathfinding or simple approach
5. **Balance** - Make sure it's not too OP (maybe require resources/power?)

---

## 🎯 Recommendation

Start with **Option A (Auto-Player)** as MVP, then evolve to **Option C (Hybrid)**.

### MVP Features:
1. ✅ Clear debris (weeds, stones, sticks) in radius around player
2. ✅ Configurable radius
3. ✅ Toggle on/off via hotkey
4. ✅ Items drop on ground or go to nearby chest

### Then Add:
1. Tree chopping
2. Hoeing + planting
3. Watering + harvesting
4. Visual robot
5. Craftable station

---

## 📚 References

- `GameLocation.cs` - terrainFeatures, objects, performToolAction
- `Object.cs` - performToolAction, litterCategory
- `Farmer.cs` - tool usage, stamina
- `Game1.cs` - Tool.DoFunction pattern
- `NPC.cs` - JunimoHarvester as reference for autonomous NPCs
- `Utility.cs` - helper functions for tiles and items
- Your `NoStamina` mod - stamina patching pattern

---

## 🚀 Next Steps

1. Create new project folder: `AutomatedFarming/`
2. Set up `.csproj` and `manifest.json`
3. Implement basic debris clearing
4. Test and iterate!

**This mod would fill a real gap in the Stardew modding scene!**
