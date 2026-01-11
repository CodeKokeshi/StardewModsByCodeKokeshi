# 🐕 Working Pets Mod - Detailed Plan

## 📋 Concept Summary

**Make your pet (cat/dog) actually useful!** Instead of just lying around, your pet will:
- Clear weeds, stones, and sticks
- Chop down trees and stumps
- Store collected items in their personal inventory
- Stay on the farm for easy interaction
- Toggle work mode via dialogue

---

## ✅ Feasibility Analysis

### 1. Using Pet Sprites - **✅ FEASIBLE**
From `NPC.cs`:
```csharp
[XmlInclude(typeof(Cat))]
[XmlInclude(typeof(Dog))]
[XmlInclude(typeof(Pet))]
```

**Two approaches:**
- **Option A**: Patch the existing `Pet` class behavior (cleaner)
- **Option B**: Create a separate "WorkerPet" NPC that uses Pet sprites (safer)

**Recommendation**: Option A - Patch existing Pet. It's YOUR pet, now it works!

### 2. Dialogue System - **✅ FEASIBLE (No XNB needed!)**
From `GameLocation.cs`:
```csharp
public void createQuestionDialogue(string question, Response[] answerChoices, afterQuestionBehavior afterDialogueBehavior, NPC speaker = null)
```

We can create custom dialogue menus entirely through code:
```csharp
Response[] responses = new Response[]
{
    new Response("ToggleWork", "Toggle Work Mode"),
    new Response("OpenChest", "Check Inventory"),
    new Response("Cancel", "Nevermind")
};
location.createQuestionDialogue("What would you like?", responses, HandlePetResponse);
```

### 3. Custom Inventory/Chest - **✅ FEASIBLE**
We can:
- Use `Inventory` class (same as player inventory)
- Store it in `ModData` on the pet
- Open it with `ItemGrabMenu` (same as chest UI)

### 4. Keeping Pet on Farm - **✅ FEASIBLE**
- Use `GameLoop.UpdateTicked` to check pet location
- Warp pet back to farm if they wander off
- Or restrict their movement area

### 5. Clearing Debris/Trees - **✅ Already Confirmed Feasible**
- `Object.performToolAction()` for weeds/stones/sticks
- `Tree.performToolAction()` for trees
- `ResourceClump.performToolAction()` for stumps/boulders

---

## 🎮 Feature Breakdown

### Core Features (MVP)

| Feature | Description | Implementation |
|---------|-------------|----------------|
| **Work Mode Toggle** | Talk to pet → "Start/Stop Working" | Harmony patch `Pet.checkAction()` |
| **Clear Weeds** | Pet removes weeds automatically | Scan `location.objects` for litter |
| **Clear Stones** | Pet breaks small stones | Same as weeds |
| **Clear Sticks** | Pet picks up branches | Same as weeds |
| **Chop Trees** | Pet cuts down fully grown trees | Scan `terrainFeatures` for Trees |
| **Chop Stumps** | Pet removes tree stumps | Same as trees |
| **Pet Inventory** | 36-slot storage (like player) | Custom `Inventory` + `ItemGrabMenu` |
| **Stay on Farm** | Pet doesn't leave farm area | Location check + warp |
| **Idle/Patrol** | Pet moves around when not working | Custom behavior states |

### Dialogue Options

When you interact with your pet:
```
╔═══════════════════════════════════════════╗
║  [Pet Name] looks at you expectantly!     ║
╠═══════════════════════════════════════════╣
║  > Toggle Work Mode [Currently: ON/OFF]   ║
║  > Check Inventory (36 slots)             ║
║  > Pet them (normal pet action)           ║
║  > Cancel                                 ║
╚═══════════════════════════════════════════╝
```

---

## 🏗️ Architecture

### Project Structure
```
WorkingPets/
├── ModEntry.cs                 # SMAPI entry point
├── ModConfig.cs                # Configuration options
├── manifest.json
├── WorkingPets.csproj
│
├── Patches/
│   └── PetPatches.cs           # Harmony patches for Pet class
│
├── Behaviors/
│   ├── PetWorkManager.cs       # Main work logic controller
│   ├── PetStateManager.cs      # Idle/Working/Patrolling states
│   └── PetInventoryManager.cs  # Handles pet's chest/inventory
│
├── Tasks/
│   ├── IWorkTask.cs            # Task interface
│   ├── ClearDebrisTask.cs      # Weeds, stones, sticks
│   └── ChopTreeTask.cs         # Trees and stumps
│
└── UI/
    └── PetDialogueHandler.cs   # Custom dialogue menu
```

### Key Classes

#### 1. ModEntry.cs
```csharp
public class ModEntry : Mod
{
    public static ModEntry Instance;
    public static ModConfig Config;
    public static PetWorkManager WorkManager;
    
    public override void Entry(IModHelper helper)
    {
        Instance = this;
        Config = helper.ReadConfig<ModConfig>();
        
        // Apply Harmony patches
        var harmony = new Harmony(this.ModManifest.UniqueID);
        harmony.PatchAll();
        
        // Register events
        helper.Events.GameLoop.UpdateTicked += OnUpdateTicked;
        helper.Events.GameLoop.SaveLoaded += OnSaveLoaded;
        helper.Events.GameLoop.Saving += OnSaving;
        helper.Events.Input.ButtonPressed += OnButtonPressed;
    }
}
```

#### 2. ModConfig.cs
```csharp
public class ModConfig
{
    // General
    public bool ModEnabled { get; set; } = true;
    public int TicksBetweenActions { get; set; } = 60; // ~1 second
    
    // Work toggles
    public bool ClearWeeds { get; set; } = true;
    public bool ClearStones { get; set; } = true;
    public bool ClearSticks { get; set; } = true;
    public bool ChopTrees { get; set; } = true;
    public bool ChopStumps { get; set; } = true;
    
    // Pet behavior
    public int WorkRadius { get; set; } = 50; // tiles from pet
    public bool ShowWorkingAnimation { get; set; } = true;
    
    // Inventory
    public int InventorySize { get; set; } = 36; // slots
}
```

#### 3. PetPatches.cs (Harmony)
```csharp
[HarmonyPatch(typeof(Pet), nameof(Pet.checkAction))]
public static class PetCheckActionPatch
{
    public static bool Prefix(Pet __instance, Farmer who, GameLocation l, ref bool __result)
    {
        // Show our custom dialogue instead of normal pet action
        if (ModEntry.Config.ModEnabled)
        {
            PetDialogueHandler.ShowPetMenu(__instance, who, l);
            __result = true;
            return false; // Skip original
        }
        return true; // Use original
    }
}
```

#### 4. PetWorkManager.cs
```csharp
public class PetWorkManager
{
    private Pet _pet;
    private bool _isWorking;
    private Inventory _petInventory;
    private int _tickCounter;
    
    public void Update()
    {
        if (!_isWorking || _pet == null) return;
        
        _tickCounter++;
        if (_tickCounter < ModEntry.Config.TicksBetweenActions) return;
        _tickCounter = 0;
        
        // Find and execute work
        var task = FindNearestTask();
        if (task != null)
        {
            ExecuteTask(task);
        }
    }
    
    private IWorkTask FindNearestTask()
    {
        var farm = Game1.getFarm();
        var petTile = _pet.Tile;
        
        // Check for debris (weeds, stones, sticks)
        foreach (var pair in farm.objects.Pairs)
        {
            if (Vector2.Distance(petTile, pair.Key) > ModEntry.Config.WorkRadius)
                continue;
                
            var obj = pair.Value;
            if (IsDebris(obj))
            {
                return new ClearDebrisTask(pair.Key, obj);
            }
        }
        
        // Check for trees
        foreach (var pair in farm.terrainFeatures.Pairs)
        {
            if (pair.Value is Tree tree && tree.growthStage.Value >= 5)
            {
                if (Vector2.Distance(petTile, pair.Key) <= ModEntry.Config.WorkRadius)
                {
                    return new ChopTreeTask(pair.Key, tree);
                }
            }
        }
        
        return null;
    }
}
```

#### 5. PetDialogueHandler.cs
```csharp
public static class PetDialogueHandler
{
    public static void ShowPetMenu(Pet pet, Farmer who, GameLocation location)
    {
        var workManager = ModEntry.WorkManager;
        string workStatus = workManager.IsWorking ? "ON" : "OFF";
        
        Response[] responses = new Response[]
        {
            new Response("ToggleWork", $"Toggle Work Mode [Currently: {workStatus}]"),
            new Response("OpenInventory", $"Check Inventory ({workManager.GetItemCount()}/36)"),
            new Response("PetThem", "Pet them"),
            new Response("Cancel", "Nevermind")
        };
        
        string petName = pet.Name ?? "Your pet";
        location.createQuestionDialogue(
            $"{petName} looks at you expectantly!",
            responses,
            HandleResponse
        );
    }
    
    private static void HandleResponse(Farmer who, string answer)
    {
        switch (answer)
        {
            case "ToggleWork":
                ModEntry.WorkManager.ToggleWork();
                string status = ModEntry.WorkManager.IsWorking ? "started" : "stopped";
                Game1.addHUDMessage(new HUDMessage($"Pet has {status} working!", 2));
                break;
                
            case "OpenInventory":
                OpenPetInventory();
                break;
                
            case "PetThem":
                // Do normal pet action
                Game1.player.currentLocation.localSound("cat"); // or dog
                break;
        }
    }
    
    private static void OpenPetInventory()
    {
        var inventory = ModEntry.WorkManager.GetInventory();
        Game1.activeClickableMenu = new ItemGrabMenu(
            inventory,
            reverseGrab: false,
            showReceivingMenu: true,
            highlightFunction: null,
            behaviorOnItemSelectFunction: null,
            message: "Pet's Inventory",
            behaviorOnItemGrab: null,
            canBeExitedWithKey: true,
            showOrganizeButton: true,
            source: ItemGrabMenu.source_chest
        );
    }
}
```

---

## 🎬 Behavior States

### Pet State Machine
```
┌─────────────┐
│    IDLE     │◄────────────────────┐
│  (normal)   │                     │
└──────┬──────┘                     │
       │ Toggle ON                  │ Toggle OFF
       ▼                            │
┌─────────────┐    No tasks    ┌────┴────────┐
│  SEARCHING  │◄──────────────►│   WORKING   │
│ (looking)   │    Task found  │  (clearing) │
└─────────────┘                └─────────────┘
```

### Pet Movement
- **IDLE**: Normal pet behavior, random movement, sleeps, etc.
- **SEARCHING**: Walks around farm looking for work
- **WORKING**: Moves toward target, performs action, collects items

---

## 💾 Save Data

Store in pet's `modData`:
```csharp
// On pet object
pet.modData["WorkingPets.IsWorking"] = "true";
pet.modData["WorkingPets.Inventory"] = JsonConvert.SerializeObject(inventoryItems);
```

Or use SMAPI's `SaveData` API for more complex storage.

---

## 📝 Implementation Phases

### Phase 1: Basic Structure (Week 1)
- [ ] Create project structure
- [ ] Set up Harmony patches
- [ ] Implement dialogue menu
- [ ] Basic work toggle

### Phase 2: Work Tasks (Week 2)
- [ ] Implement debris clearing (weeds/stones/sticks)
- [ ] Items go to pet inventory
- [ ] Basic pet movement toward targets

### Phase 3: Tree Chopping (Week 3)
- [ ] Implement tree detection
- [ ] Tree chopping logic (multiple hits)
- [ ] Stump removal
- [ ] Wood/sap collection

### Phase 4: Polish (Week 4)
- [ ] Pet animations while working
- [ ] Sound effects
- [ ] HUD messages for progress
- [ ] Config file support
- [ ] Testing & bug fixes

---

## ⚠️ Potential Challenges & Solutions

| Challenge | Solution |
|-----------|----------|
| **Pet wandering off** | Check location every tick, warp back to farm |
| **Multiplayer sync** | Use NetFields or careful single-player-only logic |
| **Performance** | Only scan every N ticks, limit work radius |
| **Tree chopping takes multiple hits** | Track damage per tree in modData |
| **Items dropped vs inventory** | Intercept drops, add directly to inventory |
| **Pet sprite animations** | Use existing Pet animations or custom |

---

## 🎯 MVP Definition

**Minimum Viable Product includes:**
1. ✅ Talk to pet → Custom dialogue appears
2. ✅ Toggle work mode on/off
3. ✅ Pet clears weeds when working
4. ✅ Pet clears stones when working  
5. ✅ Pet clears sticks when working
6. ✅ Pet chops trees when working
7. ✅ Items stored in pet's inventory
8. ✅ Can open pet's inventory via dialogue
9. ✅ Pet stays on farm

**NOT in MVP:**
- Planting seeds
- Watering crops
- Harvesting
- Custom sprites
- Multiplayer support

---

## 🚀 Ready to Build!

This mod is **100% feasible** with SMAPI + Harmony. No XNB editing needed!

**Key selling points:**
- 🐕 Makes your pet actually useful
- 🎮 Simple toggle via talking to pet
- 📦 Built-in inventory storage
- 🌳 Clears the annoying debris automatically
- 💚 No stamina cost (pet does the work!)

Want me to start building the MVP?
