# Potential Stardew Valley Mods Based on Source Code Analysis

Here is a list of potential mods derived from analyzing the Stardew Valley source code (`.cs` files). Each idea includes the target method to patch and the expected logic.

---

## 🏃 PLAYER MECHANICS

### 1. FlashSpeed (Movement Speed Multiplier)
*   **Target Method**: `StardewValley.Farmer.getMovementSpeed()` in `Farmer.cs`
*   **How it works**: This method calculates the player's movement speed every frame, factoring in horses, coffee, and tile effects.
*   **Implementation**: Patch the return value of this method. Multiply the final speed by `2.0` (or a configurable amount).
*   **Why**: A simpler, more "native" speed mod that stacks correctly with coffee and horses.

### 2. No Pass Out (Stay Awake Forever)
*   **Target Method**: `StardewValley.Farmer.passOutFromTired(Farmer who)` in `Farmer.cs`
*   **How it works**: Called at 2am or when stamina drops to -16. Triggers the pass-out animation and money loss.
*   **Implementation**: Return `false` / skip the original method entirely.
*   **Why**: Allows night owls to farm all night without penalties.

### 3. Godmode (No Damage)
*   **Target Method**: `StardewValley.Farmer.takeDamage(int damage, bool overrideParry, Monster damager)` in `Farmer.cs`
*   **How it works**: Called whenever a monster hits you. Reduces health and can trigger death.
*   **Implementation**: Set `damage = 0` in a `Prefix` patch, or skip the method.
*   **Why**: Makes skull cavern and mines trivial for resource farming.

### 4. Infinite Health (Auto-Heal)
*   **Target Field**: `StardewValley.Farmer.health` in `Farmer.cs`
*   **How it works**: The `health` field is directly modified when taking damage.
*   **Implementation**: In `UpdateTicked`, set `Game1.player.health = Game1.player.maxHealth`.
*   **Why**: Similar to Godmode but allows the damage animation to play.

### 5. Max Daily Luck (Always Lucky)
*   **Target Property**: `StardewValley.Farmer.DailyLuck` in `Farmer.cs`
*   **How it works**: Returns a value between -0.12 and +0.12 based on `team.sharedDailyLuck`.
*   **Implementation**: Patch the getter to always return `0.12` (max luck).
*   **Why**: Affects geode drops, fishing treasure, mine ladders, and more.

---

## 💰 ECONOMY & EXPERIENCE

### 6. Double XP (Experience Multiplier)
*   **Target Method**: `StardewValley.Farmer.gainExperience(int which, int howMuch)` in `Farmer.cs`
*   **How it works**: Central method for ALL skill XP gains (farming, mining, fishing, foraging, combat).
*   **Implementation**: Multiply `howMuch` by a configurable amount (e.g., x2, x10).
*   **Why**: Faster skill leveling without cheating items directly.

### 7. Better Sell Prices (Profit Boost)
*   **Target Method**: `StardewValley.Object.sellToStorePrice(long specificPlayerID)` in `Object.cs`
*   **How it works**: Calculates the sell price of any item, factoring quality and profession bonuses.
*   **Implementation**: Multiply the return value by a configurable amount (e.g., x1.5 for 50% more profit).
*   **Why**: Makes the shipping bin more rewarding.

### 8. Free Purchases (Everything Costs 0)
*   **Target Reference**: `StardewValley.Menus.ShopMenu` 
*   **Target Method**: Logic in `ShopMenu.receiveLeftClick()` that checks money.
*   **How it works**: The game subtracts `price` from `Game1.player.Money` when purchasing.
*   **Implementation**: Patch the purchase logic to skip the money check or set price to 0.
*   **Why**: Debug/sandbox mode for testing builds.

---

## ❤️ SOCIAL & NPCs

### 9. Faster Friendship (Double Rep / No Decay)
*   **Target Method**: `StardewValley.Farmer.changeFriendship(int amount, NPC n)` in `Farmer.cs`
*   **How it works**: This is the central hub for all relationship changes. Gifts, talking, quests all call this.
*   **Implementation**: 
    *   If `amount > 0` (gain): Multiply by a multiplier (e.g., x2 or x10).
    *   If `amount < 0` (loss): Set to `0` to prevent friendship decay.
*   **Why**: Covers *every* possible interaction automatically.

### 10. Infinite Gift Giving
*   **Target Reference**: `StardewValley.NPC.maxGiftsPerWeek = 2` constant.
*   **Target Method**: `StardewValley.NPC.receiveGift(...)` in `NPC.cs`
*   **How it works**: The game checks `friendshipData[npc].GiftsThisWeek < 2`.
*   **Implementation**: Reset `GiftsThisWeek` to `0` after every gift in a `Postfix` patch.
*   **Why**: Spam gifts to max out hearts fast.

### 11. Everyone Loves Everything (Gift Taste Override)
*   **Target Method**: `StardewValley.NPC.getGiftTasteForThisItem(Item item)` in `NPC.cs`
*   **How it works**: Returns a taste value (0=Love, 2=Like, 4=Dislike, 6=Hate, 8=Neutral).
*   **Implementation**: Force the return value to always be `0` (Love).
*   **Why**: Every gift is a loved gift. Maximum friendship gains.

---

## 🌱 FARMING & CROPS

### 12. No Soil Decay (Eternal Tilled Soil)
*   **Target Method**: `StardewValley.GameLocation.GetDirtDecayChance(Vector2 tile)` in `GameLocation.cs`
*   **How it works**: Returns a probability (0.0 to 1.0) that hoed dirt reverts overnight.
*   **Implementation**: Force return `0.0` (0% chance).
*   **Why**: Keeps your farm layout permanent across seasons.

### 13. Instant Machine Processing
*   **Target Field**: `StardewValley.Object.MinutesUntilReady` in `Object.cs`
*   **How it works**: Machines (Kegs, Jars, Furnaces) count down this value every 10 minutes.
*   **Implementation**: When an item is placed in a machine (`performObjectDropInAction`), set `MinutesUntilReady = 0` or `10`.
*   **Why**: Kegs produce wine instantly, furnaces smelt instantly, etc.

### 14. Always Iridium Quality
*   **Target Field**: `StardewValley.Object.quality` in `Object.cs`
*   **How it works**: Quality is 0 (normal), 1 (silver), 2 (gold), 4 (iridium).
*   **Implementation**: Patch item creation or harvest logic to set `quality.Value = 4`.
*   **Why**: Every crop, forage, and product is iridium quality.

### 15. Auto-Water All Crops (Spouse Helper++)
*   **Target Reference**: `StardewValley.NPC.hasSomeoneWateredCrops` in `NPC.cs`
*   **Target Method**: `GameLocation.DayUpdate(int dayOfMonth)` in `GameLocation.cs`
*   **How it works**: On day start, the game iterates through `HoeDirt` and checks if it's watered.
*   **Implementation**: In `DayStarted` event, iterate `Game1.getFarm().terrainFeatures` and call `dirt.state.Value = 1` (watered).
*   **Why**: Removes the need for sprinklers or watering cans entirely.

---

## ⛏️ MINING & COMBAT

### 16. One-Hit Kill Monsters
*   **Target Method**: `StardewValley.GameLocation.damageMonster(...)` in `GameLocation.cs`
*   **How it works**: Calculates damage dealt to monsters based on weapon stats and buffs.
*   **Implementation**: Set `minDamage` and `maxDamage` to 9999 in a `Prefix`.
*   **Why**: Clears mines and skull cavern with ease.

### 17. No Monster Spawns
*   **Target Search**: Monster spawn logic in `MineShaft.cs` or `GameLocation.cs`.
*   **Target Method**: Logic that adds monsters to `characters` list.
*   **How it works**: Mines spawn monsters based on level and conditions.
*   **Implementation**: Skip the spawn logic or clear `characters` of monsters on entry.
*   **Why**: Peaceful mining for resources only.

### 18. Ladder on Every Rock
*   **Target Search**: Ladder spawn chance in `MineShaft.cs`.
*   **How it works**: Breaking rocks has a small chance to spawn a ladder/hole.
*   **Implementation**: Force the ladder spawn chance to 100%.
*   **Why**: Speed-run to floor 100+ easily.

---

## 🎣 FISHING

### 19. Auto-Catch Fish (Skip Minigame)
*   **Target Method**: `StardewValley.Menus.BobberBar` constructor or `update()`.
*   **How it works**: The minigame tracks a `distanceFromCatching` value (0-1).
*   **Implementation**: Set `distanceFromCatching = 1` immediately to trigger a catch.
*   **Why**: No minigame, instant perfect catch.

### 20. Always Perfect Catch
*   **Target Field**: `StardewValley.Menus.BobberBar.perfect` 
*   **How it works**: Set to `true` if the bar never left the fish zone.
*   **Implementation**: Force `perfect = true` on catch.
*   **Why**: Guaranteed quality boost for every fish.

### 21. Unbreakable Tackles
*   **Target Field**: `StardewValley.Object.uses` (for tackle items).
*   **How it works**: Tackle durability decreases after each catch.
*   **Implementation**: Prevent `uses.Value` from increasing.
*   **Why**: Infinite spinner/trap bobber durability.

---

## 🏠 BUILDINGS & FARM

### 22. No Fence Decay
*   **Target Method**: `StardewValley.Fence.minutesElapsed(int minutes)` or `Fence.health`.
*   **How it works**: Fences have a health value that decreases over time.
*   **Implementation**: Prevent health from decreasing or reset to max.
*   **Why**: Fences stay pristine forever.

### 23. Instant Animal Produce
*   **Target Search**: Animal produce logic in `FarmAnimal.cs`.
*   **Target Field**: `FarmAnimal.daysSinceLastLay`
*   **How it works**: Animals produce based on days since last produce.
*   **Implementation**: Set `daysSinceLastLay` to the required threshold on day start.
*   **Why**: Eggs and milk every single day.

### 24. Bigger Stack Sizes
*   **Target Field**: `StardewValley.Item.maximumStackSize()` 
*   **How it works**: Returns 999 for most items.
*   **Implementation**: Patch to return 9999 or 99999.
*   **Why**: Carry more items in one slot.

---

## 🌲 FORAGING & TREES

### 25. Instant Tree Chopping
*   **Target Method**: `StardewValley.TerrainFeatures.Tree.performToolAction(Tool t, int damage, Vector2 tileLocation)`
*   **How it works**: Trees have health (usually 10-15 hits). Every axe hit reduces health.
*   **Implementation**: Set `damage` to 100 or set tree health to 0 immediately.
*   **Why**: One-hit chop for all trees.

### 26. Instant Tree Growth
*   **Target Field**: `StardewValley.TerrainFeatures.Tree.growthStage`
*   **How it works**: Trees grow from stage 0-5 over many days.
*   **Implementation**: On plant, set `growthStage.Value = 5` (fully grown).
*   **Why**: No waiting for trees to mature.

### 27. Forageables Respawn Daily
*   **Target Method**: `StardewValley.GameLocation.spawnObjects()` in `GameLocation.cs`
*   **How it works**: Called on day update to spawn forageables.
*   **Implementation**: Increase spawn chance or call multiple times.
*   **Why**: More leeks, daffodils, berries everywhere.

---

## ⏰ TIME & WORLD

### 28. Freeze Time
*   **Target Method**: `StardewValley.Game1.performTenMinuteClockUpdate()` in `Game1.cs`
*   **How it works**: Called every 10 in-game minutes. Increments `Game1.timeOfDay` by 10.
*   **Implementation**: Skip the method or don't increment `timeOfDay`.
*   **Why**: Days that never end.

### 29. Slow Time (Half Speed)
*   **Target Field**: `StardewValley.Game1.gameTimeInterval`
*   **How it works**: Tracks milliseconds until next 10-minute update (default ~7000ms).
*   **Implementation**: Double the threshold before clock updates.
*   **Why**: Twice as long per day without fully freezing.

### 30. Skip Cutscenes
*   **Target Method**: `StardewValley.Event.skipEvent()` in `Event.cs`
*   **How it works**: Called when you press escape during an event.
*   **Implementation**: Auto-call `skipEvent()` when an event starts.
*   **Why**: No more watching the same cutscenes on new saves.

---

## 🔧 TOOLS & ITEMS

### 31. Infinite Watering Can
*   **Target Field**: `StardewValley.Tools.WateringCan.waterCanMax` and `WaterLeft`
*   **How it works**: The can has limited water that depletes.
*   **Implementation**: Always set `WaterLeft = waterCanMax`.
*   **Why**: Never visit the pond again.

### 32. All Tools Max Level
*   **Target Field**: `StardewValley.Tool.upgradeLevel`
*   **How it works**: Tools have levels 0-4 (basic to iridium).
*   **Implementation**: On game load, set all tool `upgradeLevel.Value = 4`.
*   **Why**: Start with iridium tools.

### 33. Tool Range Increase
*   **Target Method**: Logic in `Tool.tilesAffected()` 
*   **How it works**: Calculates which tiles are affected by a charged tool swing.
*   **Implementation**: Multiply the area or add more tiles.
*   **Why**: Hoe/Water 9x9 instead of 3x3.

---

## 📦 INVENTORY & UI

### 34. Auto-Loot (Magnet) --> PRIORITY!!!
*   **Target Search**: Item pickup radius in `Farmer.cs`.
*   **Target Field**: `Farmer.magneticRadius`
*   **How it works**: Items within this radius are pulled toward the player.
*   **Implementation**: Set to a huge value like 9999.
*   **Why**: Items fly to you from across the screen.

### 35. See All Fish Locations --> PRIORITY BUT HOW DO WE DO THIS?
*   **Target Search**: Fish location data in `Locations.xnb`.
*   **Implementation**: Display overlay showing where each fish can be caught.
*   **Why**: No more guessing for legendary fish.

### 36. Unlimited Inventory Rows
*   **Target Reference**: Inventory size in `Farmer.Items`.
*   **Implementation**: Expand the max inventory slots beyond 36.
*   **Why**: Carry everything without chests.

---

## 🎰 MISC CHEATS

### 37. Casino Always Win
*   **Target Search**: Slot machine / blackjack logic in casino menus.
*   **Implementation**: Force winning outcomes.
*   **Why**: Infinite Qi coins.

### 38. Instant Geode Processing
*   **Target Method**: Geode menu animation logic.
*   **How it works**: Clint's geode animation takes several seconds.
*   **Implementation**: Skip the animation, instantly reveal contents.
*   **Why**: Process hundreds of geodes quickly.

### 39. Reveal All Map Tiles --> COULD BE USEFUL CAN'T TEST YET I'M ONLY AT DAY 10 AFTER ALL. BTW I CREATE MODS AS I PLAY THE GAME AND NOTICE INCONVIENCE.
*   **Target Field**: `Farmer.mapExploredNow` or fog-of-war logic.
*   **Implementation**: Mark all tiles as explored on game load.
*   **Why**: See the entire mine/cave layout.

### 40. Duplicate Items
*   **Target Method**: Item stack handling in `Farmer.addItemToInventory()`.
*   **Implementation**: When adding an item, add two copies instead of one.
*   **Why**: Instant wealth through duplication.

---

*List compiled from analysis of: Farmer.cs, Game1.cs, GameLocation.cs, NPC.cs, Object.cs, Building.cs, Utility.cs, Event.cs, FarmerTeam.cs, NetWorldState.cs, IClickableMenu.cs*
