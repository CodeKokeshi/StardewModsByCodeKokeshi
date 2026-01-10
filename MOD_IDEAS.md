# Potential Stardew Valley Mods Based on Source Code Analysis

Here is a list of potential mods derived from analyzing the Stardew Valley source code (`.cs` files). Each idea includes the target method to patch and the expected logic.

## 1. FlashSpeed (Movement Speed)
*   **Target Method**: `StardewValley.Farmer.getMovementSpeed()` in `Farmer.cs`
*   **How it works**: This method calculates the player's movement speed every frame, factoring in horses, coffee, and tile effects.
*   **Implementation**: Patch the return value of this method. Multiply the final speed by `2.0` (or a configurable amount).
*   **Why**: A simpler, more "native" speed mod that stacks correctly with coffee and horses, rather than forcefully setting `position`.

## 2. No Soil Decay (Eternal Tilled Soil)
*   **Target Method**: `StardewValley.GameLocation.GetDirtDecayChance(Vector2 tile)` in `GameLocation.cs`
*   **How it works**: The game calls this method overnight for every single tile of hoed dirt to determine if it should revert to grass/dirt. It returns a probability (e.g., 0.5 for 50%).
*   **Implementation**: Force this method to always return `0.0` (0% chance).
*   **Why**: Keeps your farm layout permanent. No need to re-hoe soil at the start of a season or after a few days of not planting.

## 3. Faster Friendship (Double Rep / No Decay)
*   **Target Method**: `StardewValley.Farmer.changeFriendship(int amount, NPC n)` in `Farmer.cs`
*   **How it works**: This is the central hub for all relationship changes. Whether you give a gift, talk to someone, or complete a quest, this method is called.
*   **Implementation**: 
    *   If `amount > 0` (gain): Multiply by a multiplier (e.g., x2 or x10).
    *   If `amount < 0` (loss): Set to `0` to prevent friendship decay or penalties for bad gifts.
*   **Why**: The most robust way to handle friendship. It covers *every* possible interaction automatically.

## 4. Infinite Gift Giving
*   **Target Reference**: `StardewValley.NPC.maxGiftsPerWeek = 2` constant and `CanReceiveGifts()` checks.
*   **Target Method**: `StardewValley.NPC.receiveGift(...)` in `NPC.cs`
*   **How it works**: The game checks `friendshipData[npc].GiftsThisWeek < 2` before allowing a gift.
*   **Implementation**: We can patch `Farmer.canUnnlock...` or simply reset the `GiftsThisWeek` counter to `0` after every gift given in a `Postfix` patch on `receiveGift`.
*   **Why**: Allows you to spam gifts to level up friendship instantly.

## 5. Instant Tree Chopping
*   **Target Search**: `Tree.health`, `Tree.performToolAction`.
*   **Target Method**: `StardewValley.TerrainFeatures.Tree.performToolAction(Tool t, int damage, Vector2 tileLocation)`
*   **How it works**: Trees have health (usually 10-15 hits). Every axe hit reduces health.
*   **Implementation**: In the `Prefix`, if the tool is an Axe, set the `damage` parameter to a huge number (e.g., 100) or set the tree's health to 0 immediately.
*   **Why**: One-hit chop for all trees, saving massive time and energy.

## 6. Unbreakable Tackles
*   **Target Search**: `FishingRod.doneFishing`.
*   **Target Method**: `StardewValley.Tools.FishingRod.doneFishing(...)`
*   **How it works**: When you catch a fish, the game reduces the durability of your tackle (bait/spinner). 
*   **Implementation**: Patch the logic where `attachement.scale.X` (durability) is minimized. Prevent it from decreasing.
*   **Why**: Iridium Sprinklers and high-end tackles are expensive; this makes them infinite.

## 7. No Fence Decay
*   **Target Search**: `Fence.performObjectDropInAction`, `Fence.minutesElapsed`.
*   **Target Method**: `StardewValley.Fence.minutesElapsed(int minutes)` or specifically `Fence.countdown` logic.
*   **How it works**: Fences have a `health` or `age` value that ticks up every day until they break.
*   **Implementation**: Reset the fence's age to 0 every day or prevent the decay method from running.
*   **Why**: Fences are tedious to repair. This makes them look pristine forever.

## 8. Universal Teleport (Mockup)
*   **Target Reference**: `Game1.warpFarmer(...)` 
*   **Target Method**: Logic in `Game1.cs` or map click handling.
*   **How it works**: The game uses `warpFarmer` to move players between maps.
*   **Implementation**: Listen for a keypress (e.g., 'T') + Mouse Click on the map. Calculate the map name from the UI and call `Game1.warpFarmer`.
*   **Why**: Debug teleport without needing the full debug menu enabled.

---
*Will continue analyzing source code for more specific hooks...*
