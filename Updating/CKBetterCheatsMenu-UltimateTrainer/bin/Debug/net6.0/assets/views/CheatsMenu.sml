<lane orientation="vertical" horizontal-content-alignment="middle">
    <!-- Title Banner -->
    <banner background={@Mods/StardewUI/Sprites/BannerBackground}
            background-border-thickness="48,0"
            padding="12"
            text={#banner.title} />

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
                <section-header text={#general.section.master-toggle} />
                <cheat-toggle label={#general.mod-enabled.label} tooltip={#general.mod-enabled.tooltip} checked={<>ModEnabled} />
                <lane layout="stretch content" margin="0, 16, 0, 0">
                    <label layout="stretch content"
                           margin="16, 0"
                           color="#888"
                           text={#general.hotkey-hint} />
                </lane>
            </lane>
        </scrollable>

        <!-- ==================== 2. PLAYER TAB ==================== -->
        <scrollable *case="Player" peeking="64">
            <lane layout="stretch content" orientation="vertical">
                <section-header text={#player.section.movement} />
                <cheat-slider label={#player.speed-multiplier.label}
                              tooltip={#player.speed-multiplier.tooltip}
                              min="0.5" max="20" interval="0.5"
                              value={<>SpeedMultiplier}
                              value-format={:FormatMultiplier} />
                <cheat-slider label={#player.speed-bonus.label}
                              tooltip={#player.speed-bonus.tooltip}
                              min="0" max="20" interval="0.5"
                              value={<>AddedSpeedBonus}
                              value-format={:FormatFlat} />
                <cheat-toggle label={#player.no-clip.label}
                              tooltip={#player.no-clip.tooltip}
                              checked={<>NoClip} />

                <section-header text={#player.section.health-stamina} />
                <cheat-toggle label={#player.infinite-stamina.label}
                              tooltip={#player.infinite-stamina.tooltip}
                              checked={<>InfiniteStamina} />
                <cheat-toggle label={#player.infinite-health.label}
                              tooltip={#player.infinite-health.tooltip}
                              checked={<>InfiniteHealth} />
                <cheat-slider label={#player.max-stamina-override.label}
                              tooltip={#player.max-stamina-override.tooltip}
                              min="0" max="1100" interval="10"
                              value={<>MaxStaminaOverride}
                              value-format={:FormatInt} />
                <cheat-slider label={#player.max-health-override.label}
                              tooltip={#player.max-health-override.tooltip}
                              min="0" max="620" interval="10"
                              value={<>MaxHealthOverride}
                              value-format={:FormatInt} />
                <cheat-slider label={#player.stamina-regen.label}
                              tooltip={#player.stamina-regen.tooltip}
                              min="0" max="100" interval="1"
                              value={<>StaminaRegenPerSecond}
                              value-format={:FormatInt} />
                <cheat-slider label={#player.health-regen.label}
                              tooltip={#player.health-regen.tooltip}
                              min="0" max="100" interval="1"
                              value={<>HealthRegenPerSecond}
                              value-format={:FormatInt} />
            </lane>
        </scrollable>

        <!-- ==================== 3. COMBAT TAB ==================== -->
        <scrollable *case="Combat" peeking="64">
            <lane layout="stretch content" orientation="vertical">
                <section-header text={#combat.section.combat} />
                <cheat-toggle label={#combat.one-hit-kill.label}
                              tooltip={#combat.one-hit-kill.tooltip}
                              checked={<>OneHitKill} />
                <cheat-toggle label={#combat.always-crit.label}
                              tooltip={#combat.always-crit.tooltip}
                              checked={<>AlwaysCrit} />
                <cheat-slider label={#combat.damage-multiplier.label}
                              tooltip={#combat.damage-multiplier.tooltip}
                              min="1" max="100" interval="1"
                              value={<>DamageMultiplier}
                              value-format={:FormatMultiplier} />
                <cheat-slider label={#combat.crit-damage-multiplier.label}
                              tooltip={#combat.crit-damage-multiplier.tooltip}
                              min="1" max="100" interval="1"
                              value={<>CritDamageMultiplier}
                              value-format={:FormatMultiplier} />
                <cheat-slider label={#combat.added-attack.label}
                              tooltip={#combat.added-attack.tooltip}
                              min="0" max="500" interval="5"
                              value={<>AddedAttack}
                              value-format={:FormatInt} />
                <cheat-slider label={#combat.added-defense.label}
                              tooltip={#combat.added-defense.tooltip}
                              min="0" max="500" interval="5"
                              value={<>AddedDefense}
                              value-format={:FormatInt} />
                <cheat-slider label={#combat.added-immunity.label}
                              tooltip={#combat.added-immunity.tooltip}
                              min="0" max="100" interval="1"
                              value={<>AddedImmunity}
                              value-format={:FormatInt} />
                <cheat-toggle label={#combat.no-monster-spawns.label}
                              tooltip={#combat.no-monster-spawns.tooltip}
                              checked={<>NoMonsterSpawns} />
            </lane>
        </scrollable>

        <!-- ==================== 4. SKILLS & LEVELS TAB ==================== -->
        <scrollable *case="Skills" peeking="64">
            <lane layout="stretch content" orientation="vertical">
                <section-header text={#skills.section.experience} />
                <cheat-slider label={#skills.xp-multiplier.label}
                              tooltip={#skills.xp-multiplier.tooltip}
                              min="1" max="1000" interval="10"
                              value={<>XpMultiplier}
                              value-format={:FormatMultiplier} />

                <section-header text={#skills.section.overrides} />
                <cheat-slider label={#skills.farming-override.label}
                              tooltip={#skills.farming-override.tooltip}
                              min="-1" max="10" interval="1"
                              value={<>FarmingLevelOverride}
                              value-format={:FormatLevel} />
                <cheat-slider label={#skills.mining-override.label}
                              tooltip={#skills.mining-override.tooltip}
                              min="-1" max="10" interval="1"
                              value={<>MiningLevelOverride}
                              value-format={:FormatLevel} />
                <cheat-slider label={#skills.foraging-override.label}
                              tooltip={#skills.foraging-override.tooltip}
                              min="-1" max="10" interval="1"
                              value={<>ForagingLevelOverride}
                              value-format={:FormatLevel} />
                <cheat-slider label={#skills.fishing-override.label}
                              tooltip={#skills.fishing-override.tooltip}
                              min="-1" max="10" interval="1"
                              value={<>FishingLevelOverride}
                              value-format={:FormatLevel} />
                <cheat-slider label={#skills.combat-override.label}
                              tooltip={#skills.combat-override.tooltip}
                              min="-1" max="10" interval="1"
                              value={<>CombatLevelOverride}
                              value-format={:FormatLevel} />
            </lane>
        </scrollable>

        <!-- ==================== 5. TOOLS & CRAFTING TAB ==================== -->
        <scrollable *case="Tools" peeking="64">
            <lane layout="stretch content" orientation="vertical">
                <section-header text={#tools.section.tools} />
                <cheat-slider label={#tools.tool-area-multiplier.label}
                              tooltip={#tools.tool-area-multiplier.tooltip}
                              min="1" max="11" interval="2"
                              value={<>ToolAreaMultiplier}
                              value-format={:FormatToolArea} />
                <cheat-toggle label={#tools.no-tool-stamina.label}
                              tooltip={#tools.no-tool-stamina.tooltip}
                              checked={<>NoToolStaminaCost} />
                <cheat-toggle label={#tools.infinite-water.label}
                              tooltip={#tools.infinite-water.tooltip}
                              checked={<>InfiniteWater} />
                <cheat-toggle label={#tools.one-hit-tools.label}
                              tooltip={#tools.one-hit-tools.tooltip}
                              checked={<>OneHitTools} />

                <section-header text={#tools.section.upgrades} />
                <cheat-toggle label={#tools.instant-tool-upgrade.label}
                              tooltip={#tools.instant-tool-upgrade.tooltip}
                              checked={<>InstantToolUpgrade} />

                <section-header text={#tools.section.crafting} />
                <cheat-toggle label={#tools.free-crafting.label}
                              tooltip={#tools.free-crafting.tooltip}
                              checked={<>FreeCrafting} />
            </lane>
        </scrollable>

        <!-- ==================== 6. FARMING TAB ==================== -->
        <scrollable *case="Farming" peeking="64">
            <lane layout="stretch content" orientation="vertical">
                <section-header text={#farming.section.crops} />
                <cheat-toggle label={#farming.crops-never-die.label}
                              tooltip={#farming.crops-never-die.tooltip}
                              checked={<>CropsNeverDie} />
                <cheat-slider label={#farming.forage-quality.label}
                              tooltip={#farming.forage-quality.tooltip}
                              min="-1" max="4" interval="1"
                              value={<>ForceForageQuality}
                              value-format={:FormatQuality} />

                <section-header text={#farming.section.growth} />
                <lane layout="stretch content" margin="16, 4" vertical-content-alignment="middle">
                    <label layout="320px content"
                           margin="0, 6"
                           text={#farming.grow-crops.label}
                           tooltip={#farming.grow-crops.tooltip}
                           shadow-alpha="0.6"
                           shadow-color="#4448"
                           shadow-offset="-1, 1" />
                    <button layout="100px 36px"
                            text={#button.apply}
                            tooltip={#farming.grow-crops.tooltip}
                            hover-background={@Mods/StardewUI/Sprites/ButtonLight}
                            left-click=|GrowAllCrops()| />
                </lane>
                <lane layout="stretch content" margin="16, 4" vertical-content-alignment="middle">
                    <label layout="320px content"
                           margin="0, 6"
                           text={#farming.grow-trees.label}
                           tooltip={#farming.grow-trees.tooltip}
                           shadow-alpha="0.6"
                           shadow-color="#4448"
                           shadow-offset="-1, 1" />
                    <button layout="100px 36px"
                            text={#button.apply}
                            tooltip={#farming.grow-trees.tooltip}
                            hover-background={@Mods/StardewUI/Sprites/ButtonLight}
                            left-click=|GrowAllTrees()| />
                </lane>
                <lane layout="stretch content" margin="16, 4" vertical-content-alignment="middle">
                    <label layout="320px content"
                           margin="0, 6"
                           text={#farming.grow-fruit-trees.label}
                           tooltip={#farming.grow-fruit-trees.tooltip}
                           shadow-alpha="0.6"
                           shadow-color="#4448"
                           shadow-offset="-1, 1" />
                    <button layout="100px 36px"
                            text={#button.apply}
                            tooltip={#farming.grow-fruit-trees.tooltip}
                            hover-background={@Mods/StardewUI/Sprites/ButtonLight}
                            left-click=|GrowAllFruitTrees()| />
                </lane>
                <lane layout="stretch content" margin="16, 4" vertical-content-alignment="middle">
                    <label layout="320px content"
                           margin="0, 6"
                           text={#farming.water-fields.label}
                           tooltip={#farming.water-fields.tooltip}
                           shadow-alpha="0.6"
                           shadow-color="#4448"
                           shadow-offset="-1, 1" />
                    <button layout="100px 36px"
                            text={#button.apply}
                            tooltip={#farming.water-fields.tooltip}
                            hover-background={@Mods/StardewUI/Sprites/ButtonLight}
                            left-click=|WaterAllFields()| />
                </lane>

                <section-header text={#farming.section.protection} />
                <cheat-toggle label={#farming.prevent-debris.label}
                              tooltip={#farming.prevent-debris.tooltip}
                              checked={<>PreventDebrisSpawn} />
                <cheat-toggle label={#farming.soil-dont-decay.label}
                              tooltip={#farming.soil-dont-decay.tooltip}
                              checked={<>TilledSoilDontDecay} />
            </lane>
        </scrollable>

        <!-- ==================== 7. ANIMALS & PETS TAB ==================== -->
        <scrollable *case="Animals" peeking="64">
            <lane layout="stretch content" orientation="vertical">
                <section-header text={#animals.section.farm-animals} />
                <cheat-toggle label={#animals.max-happiness.label}
                              tooltip={#animals.max-happiness.tooltip}
                              checked={<>MaxAnimalHappiness} />
                <cheat-toggle label={#animals.buy-matured.label}
                              tooltip={#animals.buy-matured.tooltip}
                              checked={<>BuyAnimalsFullyMatured} />
                <cheat-toggle label={#animals.auto-pet.label}
                              tooltip={#animals.auto-pet.tooltip}
                              checked={<>AutoPetAnimals} />
                <cheat-toggle label={#animals.auto-feed.label}
                              tooltip={#animals.auto-feed.tooltip}
                              checked={<>AutoFeedAnimals} />
                <cheat-toggle label={#animals.produce-daily.label}
                              tooltip={#animals.produce-daily.tooltip}
                              checked={<>AnimalsProduceDaily} />
                <cheat-slider label={#animals.farm-hearts.label}
                              tooltip={#animals.farm-hearts.tooltip}
                              min="-1" max="10" interval="1"
                              value={<>FarmAnimalHeartsOverride}
                              value-format={:FormatHearts} />

                <section-header text={#animals.section.pets} />
                <cheat-slider label={#animals.pet-hearts.label}
                              tooltip={#animals.pet-hearts.tooltip}
                              min="-1" max="10" interval="1"
                              value={<>PetHeartsOverride}
                              value-format={:FormatHearts} />

                <section-header text={#animals.section.silos} />
                <cheat-toggle label={#animals.infinite-hay.label}
                              tooltip={#animals.infinite-hay.tooltip}
                              checked={<>InfiniteHay} />
            </lane>
        </scrollable>

        <!-- ==================== 8. FISHING TAB ==================== -->
        <scrollable *case="Fishing" peeking="64">
            <lane layout="stretch content" orientation="vertical">
                <section-header text={#fishing.section.fishing} />
                <cheat-toggle label={#fishing.instant-bite.label}
                              tooltip={#fishing.instant-bite.tooltip}
                              checked={<>InstantFishBite} />
                <cheat-toggle label={#fishing.instant-catch.label}
                              tooltip={#fishing.instant-catch.tooltip}
                              checked={<>InstantCatch} />
                <cheat-toggle label={#fishing.max-quality.label}
                              tooltip={#fishing.max-quality.tooltip}
                              checked={<>MaxFishQuality} />
                <cheat-toggle label={#fishing.always-treasure.label}
                              tooltip={#fishing.always-treasure.tooltip}
                              checked={<>AlwaysFindTreasure} />
            </lane>
        </scrollable>

        <!-- ==================== 9. ITEMS & INVENTORY TAB ==================== -->
        <scrollable *case="Items" peeking="64">
            <lane layout="stretch content" orientation="vertical">
                <section-header text={#items.section.items} />
                <cheat-slider label={#items.magnetic-multiplier.label}
                              tooltip={#items.magnetic-multiplier.tooltip}
                              min="1" max="50" interval="1"
                              value={<>MagneticRadiusMultiplier}
                              value-format={:FormatMultiplier} />
                <cheat-slider label={#items.magnetic-bonus.label}
                              tooltip={#items.magnetic-bonus.tooltip}
                              min="0" max="2000" interval="64"
                              value={<>AddedMagneticRadius}
                              value-format={:FormatRadius} />
                <cheat-toggle label={#items.infinite-items.label}
                              tooltip={#items.infinite-items.tooltip}
                              checked={<>InfiniteItems} />

                <section-header text={#items.section.recipes} />
                <lane layout="stretch content" margin="16, 4" vertical-content-alignment="middle">
                    <label layout="320px content"
                           margin="0, 6"
                           text={#items.unlock-all-recipes.label}
                           tooltip={#items.unlock-all-recipes.tooltip}
                           shadow-alpha="0.6"
                           shadow-color="#4448"
                           shadow-offset="-1, 1" />
                    <button layout="100px 36px"
                            text={#button.apply}
                            tooltip={#items.unlock-all-recipes.tooltip}
                            hover-background={@Mods/StardewUI/Sprites/ButtonLight}
                            left-click=|UnlockAllRecipes()| />
                </lane>
                <lane layout="stretch content" margin="16, 4" vertical-content-alignment="middle">
                    <label layout="320px content"
                           margin="0, 6"
                           text={#items.unlock-crafting.label}
                           tooltip={#items.unlock-crafting.tooltip}
                           shadow-alpha="0.6"
                           shadow-color="#4448"
                           shadow-offset="-1, 1" />
                    <button layout="100px 36px"
                            text={#button.apply}
                            tooltip={#items.unlock-crafting.tooltip}
                            hover-background={@Mods/StardewUI/Sprites/ButtonLight}
                            left-click=|UnlockAllCraftingRecipes()| />
                </lane>
                <lane layout="stretch content" margin="16, 4" vertical-content-alignment="middle">
                    <label layout="320px content"
                           margin="0, 6"
                           text={#items.unlock-cooking.label}
                           tooltip={#items.unlock-cooking.tooltip}
                           shadow-alpha="0.6"
                           shadow-color="#4448"
                           shadow-offset="-1, 1" />
                    <button layout="100px 36px"
                            text={#button.apply}
                            tooltip={#items.unlock-cooking.tooltip}
                            hover-background={@Mods/StardewUI/Sprites/ButtonLight}
                            left-click=|UnlockAllCookingRecipes()| />
                </lane>

                <section-header text={#items.section.backpack} />
                <lane layout="stretch content" margin="16, 4" vertical-content-alignment="middle">
                    <label layout="320px content"
                           margin="0, 6"
                           text={#items.unlock-inventory.label}
                           tooltip={#items.unlock-inventory.tooltip}
                           shadow-alpha="0.6"
                           shadow-color="#4448"
                           shadow-offset="-1, 1" />
                    <button layout="100px 36px"
                            text={#button.apply}
                            tooltip={#items.unlock-inventory.tooltip}
                            hover-background={@Mods/StardewUI/Sprites/ButtonLight}
                            left-click=|UnlockAllInventorySlots()| />
                </lane>
            </lane>
        </scrollable>

        <!-- ==================== 10. ECONOMY TAB ==================== -->
        <scrollable *case="Economy" peeking="64">
            <lane layout="stretch content" orientation="vertical">
                <section-header text={#economy.section.prices} />
                <cheat-slider label={#economy.sell-price.label}
                              tooltip={#economy.sell-price.tooltip}
                              min="1" max="100" interval="0.5"
                              value={<>SellPriceMultiplier}
                              value-format={:FormatMultiplier} />
                <cheat-slider label={#economy.buy-price.label}
                              tooltip={#economy.buy-price.tooltip}
                              min="0" max="2" interval="0.1"
                              value={<>BuyPriceMultiplier}
                              value-format={:FormatBuyPrice} />

                <section-header text={#economy.section.shopping} />
                <cheat-toggle label={#economy.free-shop.label}
                              tooltip={#economy.free-shop.tooltip}
                              checked={<>FreeShopPurchases} />
                <cheat-toggle label={#economy.free-geodes.label}
                              tooltip={#economy.free-geodes.tooltip}
                              checked={<>FreeGeodeProcessing} />

                <section-header text={#economy.section.currency} />
                <!-- Add Money -->
                <lane layout="stretch content" margin="8, 4">
                    <lane layout="200px content" vertical-content-alignment="middle">
                        <label text={#economy.add-money.label} />
                    </lane>
                    <slider layout="250px content"
                            min="100" max="100000" interval="100"
                            value={<>AddMoneyAmount}
                            value-format={:FormatMoney}
                            tooltip={#economy.add-money-slider.tooltip} />
                    <button layout="100px 36px"
                            margin="16, 0, 0, 0"
                            text={#button.add}
                            hover-background={@Mods/StardewUI/Sprites/ButtonLight}
                            tooltip={#economy.add-money-button.tooltip}
                            left-click=|AddMoney()| />
                </lane>
                <!-- Add Casino Coins -->
                <lane layout="stretch content" margin="8, 4">
                    <lane layout="200px content" vertical-content-alignment="middle">
                        <label text={#economy.add-qi-coins.label} />
                    </lane>
                    <slider layout="250px content"
                            min="10" max="10000" interval="10"
                            value={<>AddCasinoCoinsAmount}
                            value-format={:FormatQiCoins}
                            tooltip={#economy.add-qi-slider.tooltip} />
                    <button layout="100px 36px"
                            margin="16, 0, 0, 0"
                            text={#button.add}
                            hover-background={@Mods/StardewUI/Sprites/ButtonLight}
                            tooltip={#economy.add-qi-button.tooltip}
                            left-click=|AddCasinoCoins()| />
                </lane>
            </lane>
        </scrollable>

        <!-- ==================== 11. BUILDINGS & MACHINES TAB ==================== -->
        <scrollable *case="Buildings" peeking="64">
            <lane layout="stretch content" orientation="vertical">
                <section-header text={#buildings.section.machines} />
                <cheat-toggle label={#buildings.instant-machines.label}
                              tooltip={#buildings.instant-machines.tooltip}
                              checked={<>InstantMachineProcessing} />

                <section-header text={#buildings.section.construction} />
                <cheat-toggle label={#buildings.instant-build.label}
                              tooltip={#buildings.instant-build.tooltip}
                              checked={<>InstantBuildConstruction} />
                <cheat-toggle label={#buildings.instant-upgrade.label}
                              tooltip={#buildings.instant-upgrade.tooltip}
                              checked={<>InstantBuildUpgrade} />
                <cheat-toggle label={#buildings.instant-house.label}
                              tooltip={#buildings.instant-house.tooltip}
                              checked={<>InstantHouseUpgrade} />
                <cheat-toggle label={#buildings.instant-community.label}
                              tooltip={#buildings.instant-community.tooltip}
                              checked={<>InstantCommunityUpgrade} />
                <cheat-toggle label={#buildings.free-construction.label}
                              tooltip={#buildings.free-construction.tooltip}
                              checked={<>FreeBuildingConstruction} />
            </lane>
        </scrollable>

        <!-- ==================== 12. WORLD TAB ==================== -->
        <scrollable *case="World" peeking="64">
            <lane layout="stretch content" orientation="vertical">
                <section-header text={#world.section.time} />
                <cheat-toggle label={#world.freeze-time.label}
                              tooltip={#world.freeze-time.tooltip}
                              checked={<>FreezeTime} />
                <cheat-toggle label={#world.freeze-time-indoors.label}
                              tooltip={#world.freeze-time-indoors.tooltip}
                              checked={<>FreezeTimeIndoors} />
                <cheat-toggle label={#world.freeze-time-mines.label}
                              tooltip={#world.freeze-time-mines.tooltip}
                              checked={<>FreezeTimeMines} />
                <cheat-toggle label={#world.never-pass-out.label}
                              tooltip={#world.never-pass-out.tooltip}
                              checked={<>NeverPassOut} />

                <section-header text={#world.section.adjust-time} />
                <lane layout="stretch content" margin="16, 4" vertical-content-alignment="middle">
                    <label layout="320px content"
                           margin="0, 6"
                           text={#world.subtract-time.label}
                           tooltip={#world.subtract-time.tooltip}
                           shadow-alpha="0.6"
                           shadow-color="#4448"
                           shadow-offset="-1, 1" />
                    <button layout="128px 36px"
                            text={#world.subtract-time-button}
                            tooltip={#world.subtract-time-button.tooltip}
                            hover-background={@Mods/StardewUI/Sprites/ButtonLight}
                            left-click=|SubtractTime()| />
                </lane>
                <lane layout="stretch content" margin="16, 4" vertical-content-alignment="middle">
                    <label layout="320px content"
                           margin="0, 6"
                           text={#world.add-time.label}
                           tooltip={#world.add-time.tooltip}
                           shadow-alpha="0.6"
                           shadow-color="#4448"
                           shadow-offset="-1, 1" />
                    <button layout="128px 36px"
                            text={#world.add-time-button}
                            tooltip={#world.add-time-button.tooltip}
                            hover-background={@Mods/StardewUI/Sprites/ButtonLight}
                            left-click=|AddTime()| />
                </lane>
                <lane layout="stretch content" margin="8, 4">
                    <lane layout="200px content" vertical-content-alignment="middle">
                        <label text={#world.set-time.label} />
                    </lane>
                    <slider layout="250px content"
                            min="600" max="2600" interval="10"
                            value={<>SetTimeTarget}
                            value-format={:FormatTime}
                            tooltip={#world.set-time-slider.tooltip} />
                    <button layout="100px 36px"
                            margin="16, 0, 0, 0"
                            text={#button.set}
                            hover-background={@Mods/StardewUI/Sprites/ButtonLight}
                            tooltip={#world.set-time-button.tooltip}
                            left-click=|SetCurrentTime()| />
                </lane>

                <section-header text={#world.section.luck} />
                <cheat-toggle label={#world.max-luck.label}
                              tooltip={#world.max-luck.tooltip}
                              checked={<>AlwaysMaxLuck} />

                <section-header text={#world.section.weather} />
                <cheat-slider label={#world.weather.label}
                              tooltip={#world.weather.tooltip}
                              min="0" max="5" interval="1"
                              value={<>WeatherIndex}
                              value-format={:FormatWeather} />

                <section-header text={#world.section.doors} />
                <cheat-toggle label={#world.bypass-friendship.label}
                              tooltip={#world.bypass-friendship.tooltip}
                              checked={<>BypassFriendshipDoors} />
                <cheat-toggle label={#world.bypass-time.label}
                              tooltip={#world.bypass-time.tooltip}
                              checked={<>BypassTimeRestrictions} />
                <cheat-toggle label={#world.bypass-festivals.label}
                              tooltip={#world.bypass-festivals.tooltip}
                              checked={<>BypassFestivalClosures} />
                <cheat-toggle label={#world.bypass-conditional.label}
                              tooltip={#world.bypass-conditional.tooltip}
                              checked={<>BypassConditionalDoors} />
                <cheat-toggle label={#world.bypass-special.label}
                              tooltip={#world.bypass-special.tooltip}
                              checked={<>BypassSpecialClosures} />

                <section-header text={#world.section.quests} />
                <cheat-toggle label={#world.auto-accept-quests.label}
                              tooltip={#world.auto-accept-quests.tooltip}
                              checked={<>AutoAcceptQuests} />
                <cheat-toggle label={#world.infinite-quest-time.label}
                              tooltip={#world.infinite-quest-time.tooltip}
                              checked={<>InfiniteQuestTime} />

                <section-header text={#world.section.completion} />
                <lane layout="stretch content" margin="16, 4" vertical-content-alignment="middle">
                    <label layout="320px content"
                           margin="0, 6"
                           text={#world.complete-bundles.label}
                           tooltip={#world.complete-bundles.tooltip}
                           shadow-alpha="0.6"
                           shadow-color="#4448"
                           shadow-offset="-1, 1" />
                    <button layout="100px 36px"
                            text={#button.apply}
                            tooltip={#world.complete-bundles.tooltip}
                            hover-background={@Mods/StardewUI/Sprites/ButtonLight}
                            left-click=|CompleteCommunityBundle()| />
                </lane>
                <lane layout="stretch content" margin="16, 4" vertical-content-alignment="middle">
                    <label layout="320px content"
                           margin="0, 6"
                           text={#world.complete-orders.label}
                           tooltip={#world.complete-orders.tooltip}
                           shadow-alpha="0.6"
                           shadow-color="#4448"
                           shadow-offset="-1, 1" />
                    <button layout="100px 36px"
                            text={#button.apply}
                            tooltip={#world.complete-orders.tooltip}
                            hover-background={@Mods/StardewUI/Sprites/ButtonLight}
                            left-click=|CompleteSpecialOrders()| />
                </lane>
            </lane>
        </scrollable>

        <!-- ==================== 13. RELATIONSHIPS TAB ==================== -->
        <scrollable *case="Relationships" peeking="64">
            <lane layout="stretch content" orientation="vertical">
                <section-header text={#relationships.section.relationships} />
                <cheat-slider label={#relationships.friendship-multiplier.label}
                              tooltip={#relationships.friendship-multiplier.tooltip}
                              min="1" max="100" interval="1"
                              value={<>FriendshipMultiplier}
                              value-format={:FormatMultiplier} />
                <cheat-toggle label={#relationships.no-decay.label}
                              tooltip={#relationships.no-decay.tooltip}
                              checked={<>NoFriendshipDecay} />
                <cheat-toggle label={#relationships.gifts-anytime.label}
                              tooltip={#relationships.gifts-anytime.tooltip}
                              checked={<>GiveGiftsAnytime} />
            </lane>
        </scrollable>

        <!-- ==================== 14. WARP TAB ==================== -->
        <scrollable *case="Warp" peeking="64">
            <lane layout="stretch content" orientation="vertical">
                <lane layout="stretch content" margin="16, 8, 0, 0">
                    <label layout="stretch content"
                           color="#666"
                           text={#warp.hint} />
                </lane>
                <lane layout="stretch content" orientation="vertical" margin="0, 8, 0, 0">
                    <warp-location *repeat={:WarpLocations} />
                </lane>
            </lane>
        </scrollable>

        <!-- ==================== 15. MINING TAB ==================== -->
        <scrollable *case="Mining" peeking="64">
            <lane layout="stretch content" orientation="vertical">
                <section-header text={#mining.section.ladder} />
                <cheat-slider label={#mining.ladder-chance.label}
                              tooltip={#mining.ladder-chance.tooltip}
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
        <button text={#button.reset-defaults}
                hover-background={@Mods/StardewUI/Sprites/ButtonLight}
                left-click=|ResetToDefaults()| />
        <button margin="16, 0, 0, 0"
                text={#button.save-close}
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
                text={#warp.button}
                tooltip={#warp.button.tooltip}
                hover-background={@Mods/StardewUI/Sprites/ButtonLight}
                left-click=|^WarpTo(this)| />
    </lane>
</template>
