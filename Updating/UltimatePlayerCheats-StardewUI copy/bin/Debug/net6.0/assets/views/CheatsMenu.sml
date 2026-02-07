<lane orientation="vertical" horizontal-content-alignment="middle">
    <!-- Title Banner -->
    <banner background={@Mods/StardewUI/Sprites/BannerBackground}
            background-border-thickness="48,0"
            padding="12"
            text="Ultimate Player Cheats" />

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
    <frame layout="820px 520px"
           background={@Mods/StardewUI/Sprites/MenuBackground}
           border={@Mods/StardewUI/Sprites/MenuBorder}
           border-thickness="36, 36, 40, 36"
           padding="16, 12"
           *switch={SelectedTab}>

        <!-- ==================== GENERAL TAB ==================== -->
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

        <!-- ==================== MOVEMENT TAB ==================== -->
        <scrollable *case="Movement" peeking="64">
            <lane layout="stretch content" orientation="vertical">
                <section-header text="Movement and Speed" />
                <cheat-slider label="Speed Multiplier"
                              tooltip="Movement speed multiplier. 1.0 = normal, 2.0 = 2x speed."
                              min="0.5" max="20" interval="0.5"
                              value={<>SpeedMultiplier}
                              value-format={:FormatMultiplier} />
                <cheat-slider label="Added Speed Bonus"
                              tooltip="Flat speed bonus added to base movement."
                              min="0" max="20" interval="0.5"
                              value={<>AddedSpeedBonus}
                              value-format={:FormatFlat} />
                <cheat-toggle label="No Clip"
                              tooltip="Walk through walls, buildings, and all obstacles."
                              checked={<>NoClip} />
                <cheat-toggle label="Always Run"
                              tooltip="Always running, never walking."
                              checked={<>AlwaysRun} />
            </lane>
        </scrollable>

        <!-- ==================== HEALTH TAB ==================== -->
        <scrollable *case="Health" peeking="64">
            <lane layout="stretch content" orientation="vertical">
                <section-header text="Health and Stamina" />
                <cheat-toggle label="Infinite Stamina"
                              tooltip="Never get tired. Stamina always stays at max."
                              checked={<>InfiniteStamina} />
                <cheat-toggle label="Infinite Health"
                              tooltip="Never die. Complete invincibility!"
                              checked={<>InfiniteHealth} />
                <cheat-slider label="Max Stamina Override"
                              tooltip="Override max stamina. 0 = use default."
                              min="0" max="10000" interval="50"
                              value={<>MaxStaminaOverride}
                              value-format={:FormatInt} />
                <cheat-slider label="Max Health Override"
                              tooltip="Override max health. 0 = use default."
                              min="0" max="10000" interval="10"
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

        <!-- ==================== COMBAT TAB ==================== -->
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

        <!-- ==================== TOOLS TAB ==================== -->
        <scrollable *case="Tools" peeking="64">
            <lane layout="stretch content" orientation="vertical">
                <section-header text="Tools and Farming" />
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
                <cheat-slider label="Axe Power Bonus"
                              tooltip="Additional power for axe. Makes chopping easier."
                              min="0" max="10" interval="1"
                              value={<>AxePowerBonus}
                              value-format={:FormatInt} />
                <cheat-slider label="Pickaxe Power Bonus"
                              tooltip="Additional power for pickaxe. Makes mining easier."
                              min="0" max="10" interval="1"
                              value={<>PickaxePowerBonus}
                              value-format={:FormatInt} />

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

        <!-- ==================== ITEMS TAB ==================== -->
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
            </lane>
        </scrollable>

        <!-- ==================== SKILLS TAB ==================== -->
        <scrollable *case="Skills" peeking="64">
            <lane layout="stretch content" orientation="vertical">
                <section-header text="Skills and Levels" />
                <cheat-slider label="XP Multiplier"
                              tooltip="Multiply all XP gains. 1.0 = normal."
                              min="1" max="1000" interval="10"
                              value={<>XpMultiplier}
                              value-format={:FormatMultiplier} />
                <cheat-slider label="Farming Level Override"
                              tooltip="Force farming level. -1 = normal."
                              min="-1" max="20" interval="1"
                              value={<>FarmingLevelOverride}
                              value-format={:FormatLevel} />
                <cheat-slider label="Mining Level Override"
                              tooltip="Force mining level. -1 = normal."
                              min="-1" max="20" interval="1"
                              value={<>MiningLevelOverride}
                              value-format={:FormatLevel} />
                <cheat-slider label="Foraging Level Override"
                              tooltip="Force foraging level. -1 = normal."
                              min="-1" max="20" interval="1"
                              value={<>ForagingLevelOverride}
                              value-format={:FormatLevel} />
                <cheat-slider label="Fishing Level Override"
                              tooltip="Force fishing level. -1 = normal."
                              min="-1" max="20" interval="1"
                              value={<>FishingLevelOverride}
                              value-format={:FormatLevel} />
                <cheat-slider label="Combat Level Override"
                              tooltip="Force combat level. -1 = normal."
                              min="-1" max="20" interval="1"
                              value={<>CombatLevelOverride}
                              value-format={:FormatLevel} />
            </lane>
        </scrollable>

        <!-- ==================== FISHING TAB ==================== -->
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

        <!-- ==================== ECONOMY TAB ==================== -->
        <scrollable *case="Economy" peeking="64">
            <lane layout="stretch content" orientation="vertical">
                <section-header text="Quality and Prices" />
                <cheat-slider label="Force Forage Quality"
                              tooltip="-1=disabled, 0=normal, 1=silver, 2=gold, 4=iridium."
                              min="-1" max="4" interval="1"
                              value={<>ForceForageQuality}
                              value-format={:FormatQuality} />
                <cheat-slider label="Sell Price Multiplier"
                              tooltip="Multiply prices when selling. 1.0 = normal."
                              min="1" max="100" interval="0.5"
                              value={<>SellPriceMultiplier}
                              value-format={:FormatMultiplier} />
                <cheat-slider label="Buy Price Multiplier"
                              tooltip="Multiply prices when buying. 0 = free!"
                              min="0" max="2" interval="0.1"
                              value={<>BuyPriceMultiplier}
                              value-format={:FormatBuyPrice} />

                <section-header text="Farm and Animals" />
                <cheat-toggle label="Max Animal Happiness"
                              tooltip="All farm animals are always at max happiness and friendship."
                              checked={<>MaxAnimalHappiness} />
                <cheat-toggle label="Crops Never Die"
                              tooltip="Crops survive season changes and lack of water."
                              checked={<>CropsNeverDie} />
                <cheat-toggle label="Instant Crop Growth"
                              tooltip="All crops grow to full harvest in real-time."
                              checked={<>InstantCropGrowth} />
                <cheat-toggle label="Instant Tree Growth"
                              tooltip="All regular trees grow to full size in real-time."
                              checked={<>InstantTreeGrowth} />
                <cheat-toggle label="Instant Fruit Tree Growth"
                              tooltip="All fruit trees grow to full maturity in real-time."
                              checked={<>InstantFruitTreeGrowth} />

                <section-header text="Shopping" />
                <cheat-toggle label="Free Shop Purchases"
                              tooltip="All shop purchases are free (no gold cost)."
                              checked={<>FreeShopPurchases} />
                <cheat-toggle label="Free Geode Processing"
                              tooltip="Geode processing at Clint's is free (no 25g cost)."
                              checked={<>FreeGeodeProcessing} />

                <section-header text="Machines" />
                <cheat-toggle label="Instant Machine Processing"
                              tooltip="All machines produce output instantly (no processing time)."
                              checked={<>InstantMachineProcessing} />
            </lane>
        </scrollable>

        <!-- ==================== WORLD TAB ==================== -->
        <scrollable *case="World" peeking="64">
            <lane layout="stretch content" orientation="vertical">
                <section-header text="Time" />
                <cheat-toggle label="Freeze Time"
                              tooltip="Time never passes."
                              checked={<>FreezeTime} />
                <cheat-toggle label="Freeze Time Indoors"
                              tooltip="Time stops when you're inside buildings."
                              checked={<>FreezeTimeIndoors} />
                <cheat-toggle label="Never Pass Out"
                              tooltip="Stay awake past 2am without passing out."
                              checked={<>NeverPassOut} />

                <section-header text="Luck" />
                <cheat-toggle label="Always Max Luck"
                              tooltip="Every day is the luckiest day possible."
                              checked={<>AlwaysMaxLuck} />
                <cheat-slider label="Daily Luck Override"
                              tooltip="Set exact daily luck. -1.0 = disabled, range -0.1 to 0.12."
                              min="-1" max="0.12" interval="0.01"
                              value={<>DailyLuckOverride}
                              value-format={:FormatLuck} />

                <section-header text="Relationships" />
                <cheat-slider label="Friendship Multiplier"
                              tooltip="Multiply friendship gains. 1.0 = normal."
                              min="1" max="100" interval="1"
                              value={<>FriendshipMultiplier}
                              value-format={:FormatMultiplier} />
                <cheat-toggle label="No Friendship Decay"
                              tooltip="Friendship never decreases."
                              checked={<>NoFriendshipDecay} />

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

                <section-header text="Weather" />
                <cheat-slider label="Weather Tomorrow"
                              tooltip="Override tomorrow's weather. No Override = game default."
                              min="0" max="5" interval="1"
                              value={<>WeatherIndex}
                              value-format={:FormatWeather} />
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
