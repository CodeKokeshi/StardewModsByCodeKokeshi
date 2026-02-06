# Ultimate Player Cheats

The ultimate player customization mod for Stardew Valley! Customize virtually every player-related variable through an easy-to-use GMCM (Generic Mod Config Menu) interface.

**Press K to instantly open this mod's config menu!** (Customizable hotkey)

## Features

All features are **disabled by default** and can be enabled through the in-game config menu.

### Master Toggle
- **Mod Enabled** - Toggle all cheats on/off
- **Open Menu Hotkey** - Press K (default) to open config menu anywhere

### Movement & Speed
- **Speed Multiplier** - Move 1x to 20x faster
- **Added Speed Bonus** - Flat speed boost
- **No Clip** - Walk through walls and obstacles
- **Always Run** - Never walk, always sprint

### Health & Stamina
- **Infinite Stamina** - Never get tired
- **Infinite Health** - Complete invincibility, never die
- **Max Stamina/Health Override** - Set custom max values (up to 10,000!)
- **Stamina/Health Regen** - Regenerate HP and stamina every second

### Combat
- **One Hit Kill** - All enemies die in one hit from ANY weapon or tool!
- **100% Critical Chance** - Every attack is a crit
- **Damage Multiplier** - Deal up to 100x damage
- **Crit Damage Multiplier** - Massive crit damage
- **Added Attack/Defense/Immunity** - Bonus combat stats
- **Custom Invincibility Duration** - Stay invincible longer after hits
- **No Monster Spawns** - Remove all monsters

### Tools & Farming
- **Tool Area Multiplier** - Hoe/water up to 11x11 tiles at once!
- **Tool Power Override** - Always max charge level
- **No Tool Stamina Cost** - Tools don't tire you
- **Infinite Water** - Watering can never empties
- **Axe/Pickaxe Power Bonus** - Chop trees and break rocks faster

### Items & Inventory
- **Magnetic Radius Multiplier** - Pick up items from 50x farther
- **Added Magnetic Radius** - Flat pickup range boost
- **Infinite Items** - Items never get consumed when used!

### Skills & Levels
- **XP Multiplier** - Gain up to **1000x** experience for instant level ups!
- **Skill Level Overrides** - Set any skill to level 0-20
  - Farming, Mining, Foraging, Fishing, Combat

### Luck
- **Always Max Luck** - Every day is the luckiest (applies immediately in real-time)
- **Daily Luck Override** - Set exact luck value (applies immediately in real-time)

### Fishing
- **Instant Fish Bite** - Fish bite immediately
- **Max Fish Quality** - Always catch iridium quality fish!

### Quality & Prices
- **Force Harvest Quality** - Force all harvested crops to a specific quality
- **Sell Price Multiplier** - Multiply sell prices 1x-100x (patches `Object.sellToStorePrice`)
- **Buy Price Multiplier** - Multiply buy prices 0x-2x, set to 0 for free items (patches `Object.salePrice`)

### Relationships
- **Friendship Multiplier** - Befriend NPCs faster
- **No Friendship Decay** - Friendships never decrease

### Time
- **Freeze Time** - Time never passes
- **Freeze Time Indoors** - Time stops inside buildings
- **Never Pass Out** - Stay awake past 2am

### Miscellaneous
- **Max Animal Happiness** - All animals always happy
- **Instant Crop Growth** - All crops across all locations grow to full harvest in real-time

## Requirements

- [SMAPI](https://smapi.io/) 4.0.0 or later
- [Generic Mod Config Menu](https://www.nexusmods.com/stardewvalley/mods/5098) (**required** dependency)

## Installation

1. Install SMAPI and Generic Mod Config Menu
2. Download this mod
3. Extract to your `Stardew Valley/Mods` folder
4. Launch the game and press **K** to configure!

## Configuration

**Press K in-game** to instantly open this mod's config menu! (You can change the hotkey in the menu itself)

You can also edit `config.json` directly in the mod folder if needed.

## Compatibility

- Works in single-player and multiplayer (each player needs the mod installed)
- Compatible with most other mods
- Uses Harmony for runtime patching - thoroughly tested to avoid crashes
- All patches include null-safety checks and edge case handling

## Technical Notes

- **One Hit Kill** works with ALL tools (pickaxe, axe, hoe, etc.) not just weapons
- **Infinite Items** patches both `reduceActiveItemByOne` and `ConsumeStack` for complete coverage
- **Max Fish Quality** patches `pullFishFromWater` to ensure iridium fish every time
- **XP Multiplier** can go up to 1000x - at max level, even tiny XP gains give instant level ups
- **Sell/Buy Price** patches `Object.sellToStorePrice` and `Object.salePrice` - affects all shops
- **Luck** now applies in real-time every second, not just at day start
- **Instant Crop Growth** iterates ALL game locations, not just the farm, and applies in real-time

## Changelog

### 1.1.0
- Added sell price multiplier (patches `Object.sellToStorePrice`)
- Added buy price multiplier (patches `Object.salePrice`)
- Luck now applies in real-time (every second) instead of only at day start
- Instant crop growth now applies in real-time across ALL locations
- Removed emojis from GMCM section headers to fix rendering issues
- Added GMCM as a required dependency in manifest.json
- Updated tooltips for clarity

### 1.0.0
- Initial release with full GMCM integration
- 40+ customizable options across 13 categories
- Hotkey support for instant menu access (default: K)
- Fixed infinite items implementation
- Fixed max fish quality to work properly
- Increased XP multiplier max to 1000x
- All features verified against game code for stability

## Credits

Created with love for the Stardew Valley modding community!

Special thanks to the SMAPI and Harmony teams for making modding possible.
