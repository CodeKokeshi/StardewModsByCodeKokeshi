<lane orientation="vertical" horizontal-content-alignment="middle">
    <!-- Title Banner -->
    <banner background={@Mods/StardewUI/Sprites/BannerBackground}
            background-border-thickness="48,0"
            padding="12"
            text="Ultimate Trainer" />

    <!-- Tab Bar -->
    <lane margin="32, 8, 0, -16" z-index="1">
        <tab *repeat={:Tabs}
             layout="64px"
             active={<>Active}
             tooltip={:Name}
             activate=|^OnTabActivated(Name)|>
            <image layout="32px" sprite={:Sprite} vertical-alignment="middle" />
        </tab>
    </lane>

    <!-- Content Area -->
    <frame layout="932px 520px"
           background={@Mods/StardewUI/Sprites/MenuBackground}
           border={@Mods/StardewUI/Sprites/MenuBorder}
           border-thickness="36, 36, 40, 36"
           padding="16, 12"
           *switch={SelectedTab}>

        <!-- ==================== 1. GENERAL TAB ==================== -->
        <scrollable *case="General" peeking="64">
            <lane layout="stretch content" orientation="vertical">
                <section-header text="Master Toggle" />
                <cheat-toggle label="Mod Enabled" tooltip="Enable or disable all cheats." checked={<>ModEnabled} />
                <lane layout="stretch content" margin="0, 16, 0, 0">
                    <label layout="stretch content"
                           margin="16, 0"
                           color="#888"
                           text="Press the menu hotkey (default: K) to open this menu at any time. Configure the hotkey in config.json." />
                </lane>
            </lane>
        </scrollable>

        <!-- ==================== 2. PLAYER TAB ==================== -->
        <scrollable *case="Player" peeking="64">
            <lane layout="stretch content" orientation="vertical">
                <section-header text="Movement and Speed" />
                <cheat-slider label="Speed Multiplier"
                              tooltip="Movement speed multiplier. 1.0 = normal, 2.0 = 2x speed."
                              min="0.5" max="20" interval="0.5"
                              value={<>SpeedMultiplier}
                              value-format={:FormatMultiplier} />
                <cheat-slider label="Speed Bonus"
                              tooltip="Flat speed bonus added to base movement."
                              min="0" max="20" interval="0.5"
                              value={<>AddedSpeedBonus}
                              value-format={:FormatFlat} />
                <cheat-toggle label="No Clip"
                              tooltip="Walk through walls, buildings, and all obstacles."
                              checked={<>NoClip} />

                <section-header text="Health and Stamina" />
                <cheat-toggle label="Infinite Stamina/Energy"
                              tooltip="Never get tired. Stamina always stays at max."
                              checked={<>InfiniteStamina} />
                <cheat-toggle label="Infinite Health"
                              tooltip="Never die. Complete invincibility!"
                              checked={<>InfiniteHealth} />
                <cheat-slider label="Max Stamina/Energy Override"
                              tooltip="Override max stamina. 0 = use default."
                              min="0" max="1100" interval="10"
                              value={<>MaxStaminaOverride}
                              value-format={:FormatInt} />
                <cheat-slider label="Max Health Override"
                              tooltip="Override max health. 0 = use default."
                              min="0" max="620" interval="10"
                              value={<>MaxHealthOverride}
                              value-format={:FormatInt} />
                <cheat-slider label="Stamina Regen / Sec"
                              tooltip="How much stamina regenerates every second. 0 = none."
                              min="0" max="100" interval="1"
                              value={<>StaminaRegenPerSecond}
                              value-format={:FormatInt} />
                <cheat-slider label="Health Regen / Sec"
                              tooltip="How much health regenerates every second. 0 = none."
                              min="0" max="100" interval="1"
                              value={<>HealthRegenPerSecond}
                              value-format={:FormatInt} />
            </lane>
        </scrollable>

        <!-- ==================== 3. COMBAT TAB ==================== -->
        <scrollable *case="Combat" peeking="64">
            <lane layout="stretch content" orientation="vertical">
                <section-header text="Combat" />
                <cheat-toggle label="One Hit Kill"
                              tooltip="All enemies die in one hit."
                              checked={<>OneHitKill} />
                <cheat-toggle label="100% Critical Chance"
                              tooltip="All attacks are critical hits."
                              checked={<>AlwaysCrit} />
                <cheat-slider label="Damage Multiplier"
                              tooltip="Multiply all weapon damage. 1.0 = normal."
                              min="1" max="100" interval="1"
                              value={<>DamageMultiplier}
                              value-format={:FormatMultiplier} />
                <cheat-slider label="Crit Damage Multiplier"
                              tooltip="Multiply critical hit damage. 1.0 = normal."
                              min="1" max="100" interval="1"
                              value={<>CritDamageMultiplier}
                              value-format={:FormatMultiplier} />
                <cheat-slider label="Added Attack"
                              tooltip="Flat bonus attack points."
                              min="0" max="500" interval="5"
                              value={<>AddedAttack}
                              value-format={:FormatInt} />
                <cheat-slider label="Added Defense"
                              tooltip="Flat bonus defense points."
                              min="0" max="500" interval="5"
                              value={<>AddedDefense}
                              value-format={:FormatInt} />
                <cheat-slider label="Added Immunity"
                              tooltip="Flat bonus immunity points."
                              min="0" max="100" interval="1"
                              value={<>AddedImmunity}
                              value-format={:FormatInt} />
                <cheat-toggle label="No Monster Spawns"
                              tooltip="Removes all monsters from the current location."
                              checked={<>NoMonsterSpawns} />
            </lane>
        </scrollable>

        <!-- ==================== 4. SKILLS & LEVELS TAB ==================== -->
        <scrollable *case="Skills" peeking="64">
            <lane layout="stretch content" orientation="vertical">
                <section-header text="Experience" />
                <cheat-slider label="XP Multiplier"
                              tooltip="Multiply all XP gains. 1.0 = normal."
                              min="1" max="1000" interval="10"
                              value={<>XpMultiplier}
                              value-format={:FormatMultiplier} />

                <section-header text="Skill Level Overrides" />
                <cheat-slider label="Farming Level Override"
                              tooltip="Force farming level. -1 = normal."
                              min="-1" max="10" interval="1"
                              value={<>FarmingLevelOverride}
                              value-format={:FormatLevel} />
                <cheat-slider label="Mining Level Override"
                              tooltip="Force mining level. -1 = normal."
                              min="-1" max="10" interval="1"
                              value={<>MiningLevelOverride}
                              value-format={:FormatLevel} />
                <cheat-slider label="Foraging Level Override"
                              tooltip="Force foraging level. -1 = normal."
                              min="-1" max="10" interval="1"
                              value={<>ForagingLevelOverride}
                              value-format={:FormatLevel} />
                <cheat-slider label="Fishing Level Override"
                              tooltip="Force fishing level. -1 = normal."
                              min="-1" max="10" interval="1"
                              value={<>FishingLevelOverride}
                              value-format={:FormatLevel} />
                <cheat-slider label="Combat Level Override"
                              tooltip="Force combat level. -1 = normal."
                              min="-1" max="10" interval="1"
                              value={<>CombatLevelOverride}
                              value-format={:FormatLevel} />
            </lane>
        </scrollable>

        <!-- ==================== 5. TOOLS & CRAFTING TAB ==================== -->
        <scrollable *case="Tools" peeking="64">
            <lane layout="stretch content" orientation="vertical">
                <section-header text="Tools" />
                <cheat-slider label="Tool Area Multiplier"
                              tooltip="Multiply area affected by hoe/watering can. 1 = normal."
                              min="1" max="11" interval="2"
                              value={<>ToolAreaMultiplier}
                              value-format={:FormatToolArea} />
                <cheat-toggle label="No Tool Stamina Cost"
                              tooltip="Using tools costs no stamina."
                              checked={<>NoToolStaminaCost} />
                <cheat-toggle label="Infinite Water"
                              tooltip="Watering can never runs out."
                              checked={<>InfiniteWater} />
                <cheat-toggle label="One Hit Tools"
                              tooltip="Axe destroys trees/stumps in one hit. Pickaxe destroys rocks/boulders in one hit."
                              checked={<>OneHitTools} />

                <section-header text="Tool Upgrades" />
                <cheat-toggle label="Instant Tool Upgrade"
                              tooltip="Tool upgrades at the blacksmith complete instantly."
                              checked={<>InstantToolUpgrade} />

                <section-header text="Crafting" />
                <cheat-toggle label="Free Crafting"
                              tooltip="Crafting does not consume ingredients."
                              checked={<>FreeCrafting} />
            </lane>
        </scrollable>

        <!-- ==================== 6. FARMING TAB ==================== -->
        <scrollable *case="Farming" peeking="64">
            <lane layout="stretch content" orientation="vertical">
                <section-header text="Crop Settings" />
                <cheat-toggle label="Crops Never Die"
                              tooltip="Crops survive season changes and lack of water."
                              checked={<>CropsNeverDie} />
                <cheat-slider label="Force Forage Quality"
                              tooltip="-1=disabled, 0=normal, 1=silver, 2=gold, 4=iridium."
                              min="-1" max="4" interval="1"
                              value={<>ForceForageQuality}
                              value-format={:FormatQuality} />

                <section-header text="Instant Growth Actions" />
                <lane layout="stretch content" margin="16, 4" vertical-content-alignment="middle">
                    <label layout="320px content"
                           margin="0, 6"
                           text="Grow All Crops"
                           tooltip="Instantly grow all crops to harvestable state."
                           shadow-alpha="0.6"
                           shadow-color="#4448"
                           shadow-offset="-1, 1" />
                    <button layout="100px 36px"
                            text="Apply"
                            tooltip="Instantly grow all crops to harvestable state."
                            hover-background={@Mods/StardewUI/Sprites/ButtonLight}
                            left-click=|GrowAllCrops()| />
                </lane>
                <lane layout="stretch content" margin="16, 4" vertical-content-alignment="middle">
                    <label layout="320px content"
                           margin="0, 6"
                           text="Grow All Trees"
                           tooltip="Instantly grow all regular trees to full size."
                           shadow-alpha="0.6"
                           shadow-color="#4448"
                           shadow-offset="-1, 1" />
                    <button layout="100px 36px"
                            text="Apply"
                            tooltip="Instantly grow all regular trees to full size."
                            hover-background={@Mods/StardewUI/Sprites/ButtonLight}
                            left-click=|GrowAllTrees()| />
                </lane>
                <lane layout="stretch content" margin="16, 4" vertical-content-alignment="middle">
                    <label layout="320px content"
                           margin="0, 6"
                           text="Grow All Fruit Trees"
                           tooltip="Instantly grow all fruit trees to full maturity."
                           shadow-alpha="0.6"
                           shadow-color="#4448"
                           shadow-offset="-1, 1" />
                    <button layout="100px 36px"
                            text="Apply"
                            tooltip="Instantly grow all fruit trees to full maturity."
                            hover-background={@Mods/StardewUI/Sprites/ButtonLight}
                            left-click=|GrowAllFruitTrees()| />
                </lane>
                <lane layout="stretch content" margin="16, 4" vertical-content-alignment="middle">
                    <label layout="320px content"
                           margin="0, 6"
                           text="Water All Fields"
                           tooltip="Water all tilled fields on the farm."
                           shadow-alpha="0.6"
                           shadow-color="#4448"
                           shadow-offset="-1, 1" />
                    <button layout="100px 36px"
                            text="Apply"
                            tooltip="Water all tilled fields on the farm."
                            hover-background={@Mods/StardewUI/Sprites/ButtonLight}
                            left-click=|WaterAllFields()| />
                </lane>

                <section-header text="Field Protection" />
                <cheat-toggle label="Prevent Debris Spawn"
                              tooltip="Stop weeds, stones, and twigs from spawning on the farm."
                              checked={<>PreventDebrisSpawn} />
                <cheat-toggle label="Tilled Soil Don't Decay"
                              tooltip="Tilled soil stays tilled indefinitely."
                              checked={<>TilledSoilDontDecay} />
            </lane>
        </scrollable>

        <!-- ==================== 7. ANIMALS & PETS TAB ==================== -->
        <scrollable *case="Animals" peeking="64">
            <lane layout="stretch content" orientation="vertical">
                <section-header text="Farm Animals" />
                <cheat-toggle label="Max Animal Happiness"
                              tooltip="All farm animals are always at max happiness."
                              checked={<>MaxAnimalHappiness} />
                <cheat-toggle label="Buy Animals Fully Matured"
                              tooltip="Purchased animals are fully grown immediately."
                              checked={<>BuyAnimalsFullyMatured} />
                <cheat-toggle label="Auto-Pet Animals"
                              tooltip="Automatically pet all farm animals each day."
                              checked={<>AutoPetAnimals} />
                <cheat-toggle label="Auto-Feed Animals"
                              tooltip="Automatically fill all feeding troughs with hay."
                              checked={<>AutoFeedAnimals} />
                <cheat-toggle label="Animals Produce Daily"
                              tooltip="All animals produce every day regardless of schedule."
                              checked={<>AnimalsProduceDaily} />
                <cheat-slider label="Farm Animal Hearts"
                              tooltip="-1=disabled, 0-10=override friendship hearts for all farm animals."
                              min="-1" max="10" interval="1"
                              value={<>FarmAnimalHeartsOverride}
                              value-format={:FormatHearts} />

                <section-header text="Pets" />
                <cheat-slider label="Pet Hearts"
                              tooltip="-1=disabled, 0-10=override friendship hearts for all pets."
                              min="-1" max="10" interval="1"
                              value={<>PetHeartsOverride}
                              value-format={:FormatHearts} />

                <section-header text="Silos" />
                <cheat-toggle label="Infinite Hay"
                              tooltip="Silos always have infinite hay."
                              checked={<>InfiniteHay} />
            </lane>
        </scrollable>

        <!-- ==================== 8. FISHING TAB ==================== -->
        <scrollable *case="Fishing" peeking="64">
            <lane layout="stretch content" orientation="vertical">
                <section-header text="Fishing" />
                <cheat-toggle label="Instant Fish Bite"
                              tooltip="Fish bite immediately when you cast."
                              checked={<>InstantFishBite} />
                <cheat-toggle label="Instant Catch"
                              tooltip="Skip the fishing minigame entirely."
                              checked={<>InstantCatch} />
                <cheat-toggle label="Max Fish Quality"
                              tooltip="All caught fish are iridium quality."
                              checked={<>MaxFishQuality} />
                <cheat-toggle label="Always Find Treasure"
                              tooltip="Always find treasure when fishing."
                              checked={<>AlwaysFindTreasure} />
            </lane>
        </scrollable>

        <!-- ==================== 9. ITEMS & INVENTORY TAB ==================== -->
        <scrollable *case="Items" peeking="64">
            <lane layout="stretch content" orientation="vertical">
                <section-header text="Items and Inventory" />
                <cheat-slider label="Magnetic Radius Multiplier"
                              tooltip="Item pickup range multiplier. 1.0 = normal."
                              min="1" max="50" interval="1"
                              value={<>MagneticRadiusMultiplier}
                              value-format={:FormatMultiplier} />
                <cheat-slider label="Added Magnetic Radius"
                              tooltip="Flat bonus to pickup range in pixels. 128 = 2 tiles."
                              min="0" max="2000" interval="64"
                              value={<>AddedMagneticRadius}
                              value-format={:FormatRadius} />
                <cheat-toggle label="Infinite Items"
                              tooltip="Items don't get consumed when used. (Experimental)"
                              checked={<>InfiniteItems} />

                <section-header text="Recipes" />
                <lane layout="stretch content" margin="16, 4" vertical-content-alignment="middle">
                    <label layout="320px content"
                           margin="0, 6"
                           text="Unlock All Recipes"
                           tooltip="Unlock all crafting and cooking recipes."
                           shadow-alpha="0.6"
                           shadow-color="#4448"
                           shadow-offset="-1, 1" />
                    <button layout="100px 36px"
                            text="Apply"
                            tooltip="Unlock all crafting and cooking recipes."
                            hover-background={@Mods/StardewUI/Sprites/ButtonLight}
                            left-click=|UnlockAllRecipes()| />
                </lane>
                <lane layout="stretch content" margin="16, 4" vertical-content-alignment="middle">
                    <label layout="320px content"
                           margin="0, 6"
                           text="Unlock All Crafting Recipes"
                           tooltip="Unlock all crafting recipes."
                           shadow-alpha="0.6"
                           shadow-color="#4448"
                           shadow-offset="-1, 1" />
                    <button layout="100px 36px"
                            text="Apply"
                            tooltip="Unlock all crafting recipes."
                            hover-background={@Mods/StardewUI/Sprites/ButtonLight}
                            left-click=|UnlockAllCraftingRecipes()| />
                </lane>
                <lane layout="stretch content" margin="16, 4" vertical-content-alignment="middle">
                    <label layout="320px content"
                           margin="0, 6"
                           text="Unlock All Cooking Recipes"
                           tooltip="Unlock all cooking recipes."
                           shadow-alpha="0.6"
                           shadow-color="#4448"
                           shadow-offset="-1, 1" />
                    <button layout="100px 36px"
                            text="Apply"
                            tooltip="Unlock all cooking recipes."
                            hover-background={@Mods/StardewUI/Sprites/ButtonLight}
                            left-click=|UnlockAllCookingRecipes()| />
                </lane>

                <section-header text="Backpack" />
                <lane layout="stretch content" margin="16, 4" vertical-content-alignment="middle">
                    <label layout="320px content"
                           margin="0, 6"
                           text="Unlock All Inventory Slots"
                           tooltip="Maximize backpack to 36 slots (3 rows)."
                           shadow-alpha="0.6"
                           shadow-color="#4448"
                           shadow-offset="-1, 1" />
                    <button layout="100px 36px"
                            text="Apply"
                            tooltip="Maximize backpack to 36 slots (3 rows)."
                            hover-background={@Mods/StardewUI/Sprites/ButtonLight}
                            left-click=|UnlockAllInventorySlots()| />
                </lane>
            </lane>
        </scrollable>

        <!-- ==================== 10. ECONOMY TAB ==================== -->
        <scrollable *case="Economy" peeking="64">
            <lane layout="stretch content" orientation="vertical">
                <section-header text="Prices" />
                <cheat-slider label="Sell Price Multiplier"
                              tooltip="Multiply prices when selling. 1.0 = normal."
                              min="1" max="100" interval="0.5"
                              value={<>SellPriceMultiplier}
                              value-format={:FormatMultiplier} />
                <cheat-slider label="Buy Price Modifier"
                              tooltip="Multiply prices when buying. 0 = free!"
                              min="0" max="2" interval="0.1"
                              value={<>BuyPriceMultiplier}
                              value-format={:FormatBuyPrice} />

                <section-header text="Shopping" />
                <cheat-toggle label="Free Shop Purchases"
                              tooltip="All shop purchases are free (no gold cost)."
                              checked={<>FreeShopPurchases} />
                <cheat-toggle label="Free Geode Processing"
                              tooltip="Geode processing at Clint's is free (no 25g cost)."
                              checked={<>FreeGeodeProcessing} />

                <section-header text="Add Currency" />
                <!-- Add Money -->
                <lane layout="stretch content" margin="8, 4">
                    <lane layout="200px content" vertical-content-alignment="middle">
                        <label text="Add Money" />
                    </lane>
                    <slider layout="250px content"
                            min="100" max="100000" interval="100"
                            value={<>AddMoneyAmount}
                            value-format={:FormatMoney}
                            tooltip="Choose amount of gold to add." />
                    <button layout="100px 36px"
                            margin="16, 0, 0, 0"
                            text="Add"
                            hover-background={@Mods/StardewUI/Sprites/ButtonLight}
                            tooltip="Add the selected amount to your wallet and profit."
                            left-click=|AddMoney()| />
                </lane>
                <!-- Add Casino Coins -->
                <lane layout="stretch content" margin="8, 4">
                    <lane layout="200px content" vertical-content-alignment="middle">
                        <label text="Add Qi Coins" />
                    </lane>
                    <slider layout="250px content"
                            min="10" max="10000" interval="10"
                            value={<>AddCasinoCoinsAmount}
                            value-format={:FormatQiCoins}
                            tooltip="Choose amount of Qi Coins to add." />
                    <button layout="100px 36px"
                            margin="16, 0, 0, 0"
                            text="Add"
                            hover-background={@Mods/StardewUI/Sprites/ButtonLight}
                            tooltip="Add the selected Qi Coins to your account."
                            left-click=|AddCasinoCoins()| />
                </lane>
            </lane>
        </scrollable>

        <!-- ==================== 11. BUILDINGS & MACHINES TAB ==================== -->
        <scrollable *case="Buildings" peeking="64">
            <lane layout="stretch content" orientation="vertical">
                <section-header text="Machines" />
                <cheat-toggle label="Instant Machine Processing"
                              tooltip="All machines produce output instantly (no processing time)."
                              checked={<>InstantMachineProcessing} />

                <section-header text="Buildings and Construction" />
                <cheat-toggle label="Instant Build Construction"
                              tooltip="Buildings finish constructing instantly when placed."
                              checked={<>InstantBuildConstruction} />
                <cheat-toggle label="Instant Build Upgrade"
                              tooltip="Building upgrades complete instantly."
                              checked={<>InstantBuildUpgrade} />
                <cheat-toggle label="Instant House Upgrade"
                              tooltip="Farmhouse upgrades complete instantly."
                              checked={<>InstantHouseUpgrade} />
                <cheat-toggle label="Instant Community Upgrade"
                              tooltip="Community upgrades (Pam's house, shortcuts) complete instantly."
                              checked={<>InstantCommunityUpgrade} />
                <cheat-toggle label="Free Building Construction"
                              tooltip="Buildings cost no gold or materials to construct."
                              checked={<>FreeBuildingConstruction} />
            </lane>
        </scrollable>

        <!-- ==================== 12. WORLD TAB ==================== -->
        <scrollable *case="World" peeking="64">
            <lane layout="stretch content" orientation="vertical">
                <section-header text="Time" />
                <cheat-toggle label="Freeze Time"
                              tooltip="Time never passes."
                              checked={<>FreezeTime} />
                <cheat-toggle label="Freeze Time Indoors"
                              tooltip="Time stops when you're inside buildings."
                              checked={<>FreezeTimeIndoors} />
                <cheat-toggle label="Freeze Time in Mines"
                              tooltip="Time stops while inside mines, skull cavern, and volcano dungeon."
                              checked={<>FreezeTimeMines} />
                <cheat-toggle label="Never Pass Out"
                              tooltip="Stay awake past 2am without passing out."
                              checked={<>NeverPassOut} />

                <section-header text="Adjust Time" />
                <lane layout="stretch content" margin="16, 4" vertical-content-alignment="middle">
                    <label layout="320px content"
                           margin="0, 6"
                           text="Subtract 10 Minutes"
                           tooltip="Go back 10 minutes in game time."
                           shadow-alpha="0.6"
                           shadow-color="#4448"
                           shadow-offset="-1, 1" />
                    <button layout="128px 36px"
                            text="-10 min"
                            tooltip="Subtract 10 minutes from the current time."
                            hover-background={@Mods/StardewUI/Sprites/ButtonLight}
                            left-click=|SubtractTime()| />
                </lane>
                <lane layout="stretch content" margin="16, 4" vertical-content-alignment="middle">
                    <label layout="320px content"
                           margin="0, 6"
                           text="Add 10 Minutes"
                           tooltip="Advance time by 10 minutes."
                           shadow-alpha="0.6"
                           shadow-color="#4448"
                           shadow-offset="-1, 1" />
                    <button layout="128px 36px"
                            text="+10 min"
                            tooltip="Add 10 minutes to the current time."
                            hover-background={@Mods/StardewUI/Sprites/ButtonLight}
                            left-click=|AddTime()| />
                </lane>
                <lane layout="stretch content" margin="8, 4">
                    <lane layout="200px content" vertical-content-alignment="middle">
                        <label text="Set Current Time" />
                    </lane>
                    <slider layout="250px content"
                            min="600" max="2600" interval="10"
                            value={<>SetTimeTarget}
                            value-format={:FormatTime}
                            tooltip="Choose the time to set." />
                    <button layout="100px 36px"
                            margin="16, 0, 0, 0"
                            text="Set"
                            hover-background={@Mods/StardewUI/Sprites/ButtonLight}
                            tooltip="Set the current game time to the selected value."
                            left-click=|SetCurrentTime()| />
                </lane>

                <section-header text="Luck" />
                <cheat-toggle label="Always Max Luck"
                              tooltip="Every day is the luckiest day possible."
                              checked={<>AlwaysMaxLuck} />

                <section-header text="Weather" />
                <cheat-slider label="Weather Tomorrow"
                              tooltip="Override tomorrow's weather. No Override = game default."
                              min="0" max="5" interval="1"
                              value={<>WeatherIndex}
                              value-format={:FormatWeather} />

                <section-header text="Bypass All Doors" />
                <cheat-toggle label="Bypass Friendship Doors"
                              tooltip="Bypass NPC bedroom doors that require 2+ hearts friendship."
                              checked={<>BypassFriendshipDoors} />
                <cheat-toggle label="Bypass Time Restrictions"
                              tooltip="Enter buildings even when they're closed (shop hours)."
                              checked={<>BypassTimeRestrictions} />
                <cheat-toggle label="Bypass Festival Closures"
                              tooltip="Enter buildings during festivals when everything is normally closed."
                              checked={<>BypassFestivalClosures} />
                <cheat-toggle label="Bypass Conditional Doors"
                              tooltip="Bypass conditional doors that use Game State Queries."
                              checked={<>BypassConditionalDoors} />
                <cheat-toggle label="Bypass Special Closures"
                              tooltip="Bypass special closures like Pierre's Wednesday closure."
                              checked={<>BypassSpecialClosures} />

                <section-header text="Quests" />
                <cheat-toggle label="Auto-Accept Quests"
                              tooltip="Automatically accept the daily quest from the Help Wanted board."
                              checked={<>AutoAcceptQuests} />
                <cheat-toggle label="Infinite Quest Time"
                              tooltip="Quest timers never expire. Quests won't fail from running out of time."
                              checked={<>InfiniteQuestTime} />

                <section-header text="Completion Actions" />
                <lane layout="stretch content" margin="16, 4" vertical-content-alignment="middle">
                    <label layout="320px content"
                           margin="0, 6"
                           text="Complete Community Bundle"
                           tooltip="Complete all community center bundles instantly."
                           shadow-alpha="0.6"
                           shadow-color="#4448"
                           shadow-offset="-1, 1" />
                    <button layout="100px 36px"
                            text="Apply"
                            tooltip="Complete all community center bundles."
                            hover-background={@Mods/StardewUI/Sprites/ButtonLight}
                            left-click=|CompleteCommunityBundle()| />
                </lane>
                <lane layout="stretch content" margin="16, 4" vertical-content-alignment="middle">
                    <label layout="320px content"
                           margin="0, 6"
                           text="Complete Special Orders"
                           tooltip="Complete all active special orders instantly."
                           shadow-alpha="0.6"
                           shadow-color="#4448"
                           shadow-offset="-1, 1" />
                    <button layout="100px 36px"
                            text="Apply"
                            tooltip="Complete all active special orders."
                            hover-background={@Mods/StardewUI/Sprites/ButtonLight}
                            left-click=|CompleteSpecialOrders()| />
                </lane>
            </lane>
        </scrollable>

        <!-- ==================== 13. RELATIONSHIPS TAB ==================== -->
        <scrollable *case="Relationships" peeking="64">
            <lane layout="stretch content" orientation="vertical">
                <section-header text="Relationships" />
                <cheat-slider label="Friendship Multiplier"
                              tooltip="Multiply friendship gains. 1.0 = normal."
                              min="1" max="100" interval="1"
                              value={<>FriendshipMultiplier}
                              value-format={:FormatMultiplier} />
                <cheat-toggle label="No Friendship Decay"
                              tooltip="Friendship never decreases."
                              checked={<>NoFriendshipDecay} />
                <cheat-toggle label="Give Gifts Anytime"
                              tooltip="Bypass daily and weekly gift limits. Give unlimited gifts to NPCs."
                              checked={<>GiveGiftsAnytime} />
            </lane>
        </scrollable>

        <!-- ==================== 14. WARP TAB ==================== -->
        <scrollable *case="Warp" peeking="64">
            <lane layout="stretch content" orientation="vertical">
                <lane layout="stretch content" margin="16, 8, 0, 0">
                    <label layout="stretch content"
                           color="#666"
                           text="Click a location to instantly warp there." />
                </lane>
                <lane layout="stretch content" orientation="vertical" margin="0, 8, 0, 0">
                    <warp-location *repeat={:WarpLocations} />
                </lane>
            </lane>
        </scrollable>

        <!-- ==================== 15. MINING TAB ==================== -->
        <scrollable *case="Mining" peeking="64">
            <lane layout="stretch content" orientation="vertical">
                <section-header text="Ladder Spawn" />
                <cheat-slider label="Force Ladder Chance"
                              tooltip="Chance to force ladder spawn when breaking rocks. 0 = disabled, 100 = always spawn."
                              min="0" max="100" interval="5"
                              value={<>ForceLadderChance}
                              value-format={:FormatLadderChance} />
            </lane>
        </scrollable>
    </frame>

    <!-- Bottom Buttons -->
    <lane layout="stretch content"
          margin="16, 12, 16, 0"
          horizontal-content-alignment="end"
          vertical-content-alignment="middle">
        <button text="Reset Defaults"
                hover-background={@Mods/StardewUI/Sprites/ButtonLight}
                left-click=|ResetToDefaults()| />
        <button margin="16, 0, 0, 0"
                text="Save and Close"
                hover-background={@Mods/StardewUI/Sprites/ButtonLight}
                left-click=|SaveAndClose()| />
    </lane>
</lane>

<!-- ==================== REUSABLE TEMPLATES ==================== -->

<!-- Section header template -->
<template name="section-header">
    <label layout="stretch content"
           margin="8, 12, 0, 4"
           font="dialogue"
           color="#444"
           text={&text}
           shadow-alpha="0.5"
           shadow-offset="-2, 2" />
</template>

<!-- Toggle row template (checkbox + label) -->
<template name="cheat-toggle">
    <lane layout="stretch content" margin="16, 4" vertical-content-alignment="middle">
        <label layout="320px content"
               margin="0, 6"
               text={&label}
               tooltip={&tooltip}
               shadow-alpha="0.6"
               shadow-color="#4448"
               shadow-offset="-1, 1" />
        <checkbox is-checked={&checked} />
    </lane>
</template>

<!-- Slider row template -->
<template name="cheat-slider">
    <lane layout="stretch content" margin="16, 4" vertical-content-alignment="middle">
        <label layout="320px content"
               margin="0, 6"
               text={&label}
               tooltip={&tooltip}
               shadow-alpha="0.6"
               shadow-color="#4448"
               shadow-offset="-1, 1" />
        <slider track-width="200"
                min={&min}
                max={&max}
                interval={&interval}
                value={&value}
                value-format={&value-format} />
    </lane>
</template>

<!-- Warp location row template (button + label) -->
<template name="warp-location">
    <lane layout="stretch content" margin="16, 2" vertical-content-alignment="middle">
        <label layout="300px content"
               margin="0, 6"
               text={:LocationName}
               tooltip={:DisplayName}
               shadow-alpha="0.6"
               shadow-color="#4448"
               shadow-offset="-1, 1" />
        <button layout="128px 36px"
                text="Warp"
                tooltip="Teleport to this location"
                hover-background={@Mods/StardewUI/Sprites/ButtonLight}
                left-click=|^WarpTo(this)| />
    </lane>
</template>
