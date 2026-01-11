# Working Pets

Turn your cat or dog into a helpful little farmhand.

## Requirements

- Stardew Valley 1.6+
- SMAPI 3.0+ (newer is fine)
- (Optional) Generic Mod Config Menu (GMCM) by spacechase0 for in-game settings

## What this mod does

When enabled, **talking to your pet** opens a small interaction menu where you can:

- Toggle **Work mode** (on/off)
- Toggle **Follow me** (on/off)
- Open your pet’s **pouch** (a separate inventory)
- Pet them (still gives the normal “petted today” behavior)
- Rename them once

### Work mode (farm helper)

While in **Work mode**, your pet will look around the farm and clear things based on your settings:

- **Debris**: weeds, small stones, twigs
- **Stumps & Logs**: tree stumps + large logs/stumps (hardwood)
- **Trees**: fully grown trees
- **Boulders**: large boulders/resource clumps

As your pet clears items, the drops are placed into the pet’s **pouch**.

### Pouch / inventory

Your pet has a separate inventory (default **36 slots**). You can open it from the pet dialogue.

- If the pouch fills up, extra items are **dropped on the ground** near your pet.

### Daily scavenging

If your pet is set to **Work mode**, it can also find **1–5 random items overnight** and put them in its pouch.

### Resting / sleeping

If the pet is doing a sleep/“zzz” emote, the mod will **let them rest** (it won’t force-move them during that time). Once the emote ends, they’ll continue working/following normally.

## Default behavior (fresh install)

By default, your pet will:

- ✅ Clear **Debris**
- ❌ NOT clear stumps/logs/trees/boulders (you must enable these through GMCM)
- ✅ Show HUD messages when work happens
- ✅ Use priorities (Debris first, then Stumps/Logs, then Trees, then Boulders)
- ❌ Not follow you outside the farm (Follow Outside Farm is off by default can be enable through GMCM)

## How to use in-game

1. Load your save.
2. Walk up to your pet and **interact/talk**.
3. Choose what you want:
   - **Ask {pet} to help around the farm** → enables Work mode
   - **Let {pet} rest** → disables Work mode
   - **Follow me, {pet}!** → enables Follow mode
   - **That’s enough following for now…** → disables Follow mode
   - **Check {pet}’s pouch** → opens the pet inventory

Notes:
- Selecting **Work** will automatically stop **Follow** first.
- Work is intended for the **Farm** location.

## Configuration (GMCM)

If you have GMCM installed, you can configure everything in-game:

### General Settings

- **Mod Enabled**: turns the entire mod on/off.
- **Work Interval (ticks)**: how often your pet scans for work.
  - 60 ticks ≈ 1 second
  - Lower = faster response but more CPU use
- **Work Radius (tiles)**: how far your pet will search from its current position.
- **Show Work Messages**: show HUD messages when your pet clears something.
- **Follow Outside Farm (Experimental)**:
  - When OFF: if you leave the farm, the pet stops following.
  - When ON: the pet can follow you into other locations.

### What Your Pet Will Clear

- **Clear Debris**
- **Clear Stumps & Logs**
- **Chop Trees** (warning: includes trees you planted)
- **Break Boulders**

### Priority Settings

- **Ignore Priority (Target Nearest)**: if enabled, your pet targets the nearest valid thing regardless of type.
- Otherwise, the pet uses priority numbers (**1–99**, lower number = higher priority):
  - Debris Priority
  - Stumps & Logs Priority
  - Trees Priority
  - Boulders Priority

Tip: priorities must be unique; if you set duplicates, the mod will auto-reset them to 1/2/3/4.

## Manual config.json options

You can also edit `config.json` in the mod folder.

Notably:
- `InventorySize` (max 36) controls how many pouch slots your pet has.

## Compatibility notes

- This mod replaces the default “talk to pet” interaction with its own menu.
- Mods which also heavily patch pet interaction may conflict.

## Troubleshooting

- **My build/deploy fails with “WorkingPets.dll is being used by another process”**:
  - Close Stardew Valley/SMAPI (it locks the DLL), then rebuild.
- **Pet seems stuck**:
  - Try increasing Work Radius and/or Work Interval.
  - Ensure the target types you want are enabled.

## Credits

- Built for SMAPI.
- Optional integration with Generic Mod Config Menu by spacechase0.
