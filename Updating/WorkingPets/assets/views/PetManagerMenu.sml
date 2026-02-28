<lane orientation="vertical"
      horizontal-content-alignment="middle"
      padding="16">

    <!-- Title banner -->
    <banner background={@Mods/StardewUI/Sprites/BannerBackground}
            background-border-thickness="48,0"
            padding="12"
            text={MenuTitle} />

    <!-- Tab bar -->
    <lane margin="32, 8, 0, -16" z-index="1">
        <tab *repeat={:Tabs}
             layout="64px"
             active={<>Active}
             tooltip={:DisplayName}
             activate=|^OnTabActivated(Name)|>
            <image layout="32px" sprite={:Sprite} vertical-alignment="middle" />
        </tab>
    </lane>

    <!-- Main content frame -->
    <frame layout="720px content"
           background={@Mods/StardewUI/Sprites/MenuBackground}
           border={@Mods/StardewUI/Sprites/MenuBorder}
           border-thickness="36, 36, 40, 36"
           padding="24, 20"
           *switch={SelectedTab}>

        <!-- ═══════════════════════════════════════════════
             TAB 1: Manage (state buttons per pet)
             ═══════════════════════════════════════════════ -->
        <lane *case="Manage"
              orientation="vertical"
              horizontal-content-alignment="middle">

            <label *if={HasNoPets}
                   layout="stretch content"
                   margin="0, 32"
                   text={NoPetsText} />

            <scrollable *if={HasPets}
                        layout="stretch 420px"
                        peeking="64">
                <lane orientation="vertical">

                    <frame *repeat={Pets}
                           layout="stretch content"
                           padding="8, 10"
                           margin="0, 0, 0, 6"
                           background={@Mods/StardewUI/Sprites/MenuSlotTransparent}>
                        <lane orientation="horizontal"
                              vertical-content-alignment="middle">

                            <!-- Left: sprite + name -->
                            <lane orientation="horizontal"
                                  vertical-content-alignment="middle"
                                  layout="250px content">

                                <frame layout="64px 64px"
                                       horizontal-content-alignment="middle"
                                       vertical-content-alignment="middle"
                                       margin="0, 0, 10, 0">
                                    <image layout="56px 56px"
                                           sprite={PetSprite}
                                           fit="Stretch" />
                                </frame>

                                <lane orientation="vertical"
                                      vertical-content-alignment="middle">
                                    <label text={DisplayName}
                                           bold="true" />
                                    <label text={StatusText}
                                           margin="0, 2, 0, 0" />
                                    <label text={LocationText}
                                           margin="0, 1, 0, 0" />
                                </lane>
                            </lane>

                            <!-- Right: mode buttons -->
                            <lane orientation="horizontal"
                                  vertical-content-alignment="middle"
                                  horizontal-content-alignment="end"
                                  layout="stretch content">

                                <!-- Idle -->
                                <button *if={CanSetIdle}
                                        click=|OnSetIdle()|
                                        tooltip={IdleTooltip}
                                        margin="0, 0, 6, 0">
                                    <frame layout="48px 48px"
                                           horizontal-content-alignment="middle"
                                           vertical-content-alignment="middle">
                                        <image sprite={IdleIcon}
                                               layout="40px 40px"
                                               fit="Stretch" />
                                    </frame>
                                </button>
                                <frame *if={IsIdle}
                                       layout="48px 48px"
                                       background={@Mods/StardewUI/Sprites/ButtonDark}
                                       horizontal-content-alignment="middle"
                                       vertical-content-alignment="middle"
                                       tooltip={IdleActiveTooltip}
                                       margin="0, 0, 6, 0">
                                    <image sprite={IdleIcon}
                                           layout="36px 36px"
                                           fit="Stretch" />
                                </frame>

                                <!-- Farm Work -->
                                <button *if={CanSetFarmWork}
                                        click=|OnSetFarmWork()|
                                        tooltip={FarmWorkTooltip}
                                        margin="0, 0, 6, 0">
                                    <frame layout="48px 48px"
                                           horizontal-content-alignment="middle"
                                           vertical-content-alignment="middle">
                                        <image sprite={FarmWorkIcon}
                                               layout="40px 40px"
                                               fit="Stretch" />
                                    </frame>
                                </button>
                                <frame *if={IsFarmWork}
                                       layout="48px 48px"
                                       background={@Mods/StardewUI/Sprites/ButtonDark}
                                       horizontal-content-alignment="middle"
                                       vertical-content-alignment="middle"
                                       tooltip={FarmWorkActiveTooltip}
                                       margin="0, 0, 6, 0">
                                    <image sprite={FarmWorkIcon}
                                           layout="36px 36px"
                                           fit="Stretch" />
                                </frame>

                                <!-- Valley Work -->
                                <button *if={CanSetValleyWork}
                                        click=|OnSetValleyWork()|
                                        tooltip={ValleyWorkTooltip}
                                        margin="0, 0, 6, 0">
                                    <frame layout="48px 48px"
                                           horizontal-content-alignment="middle"
                                           vertical-content-alignment="middle">
                                        <image sprite={ValleyWorkIcon}
                                               layout="40px 40px"
                                               fit="Stretch" />
                                    </frame>
                                </button>
                                <frame *if={IsValleyWork}
                                       layout="48px 48px"
                                       background={@Mods/StardewUI/Sprites/ButtonDark}
                                       horizontal-content-alignment="middle"
                                       vertical-content-alignment="middle"
                                       tooltip={ValleyWorkActiveTooltip}
                                       margin="0, 0, 6, 0">
                                    <image sprite={ValleyWorkIcon}
                                           layout="36px 36px"
                                           fit="Stretch" />
                                </frame>

                                <!-- Exploration -->
                                <button *if={CanSetExploration}
                                        click=|OnSetExploration()|
                                        tooltip={ExplorationTooltip}
                                        margin="0, 0, 6, 0">
                                    <frame layout="48px 48px"
                                           horizontal-content-alignment="middle"
                                           vertical-content-alignment="middle">
                                        <image sprite={ExplorationIcon}
                                               layout="40px 40px"
                                               fit="Stretch" />
                                    </frame>
                                </button>
                                <frame *if={IsExploration}
                                       layout="48px 48px"
                                       background={@Mods/StardewUI/Sprites/ButtonDark}
                                       horizontal-content-alignment="middle"
                                       vertical-content-alignment="middle"
                                       tooltip={ExplorationActiveTooltip}
                                       margin="0, 0, 6, 0">
                                    <image sprite={ExplorationIcon}
                                           layout="36px 36px"
                                           fit="Stretch" />
                                </frame>

                                <!-- Follow -->
                                <button *if={CanSetFollow}
                                        click=|OnSetFollow()|
                                        tooltip={FollowTooltip}>
                                    <frame layout="48px 48px"
                                           horizontal-content-alignment="middle"
                                           vertical-content-alignment="middle">
                                        <image sprite={FollowIcon}
                                               layout="40px 40px"
                                               fit="Stretch" />
                                    </frame>
                                </button>
                                <frame *if={IsFollowing}
                                       layout="48px 48px"
                                       background={@Mods/StardewUI/Sprites/ButtonDark}
                                       horizontal-content-alignment="middle"
                                       vertical-content-alignment="middle"
                                       tooltip={FollowActiveTooltip}>
                                    <image sprite={FollowIcon}
                                           layout="36px 36px"
                                           fit="Stretch" />
                                </frame>

                            </lane>
                        </lane>
                    </frame>

                </lane>
            </scrollable>
        </lane>

        <!-- ═══════════════════════════════════════════════
             TAB 2: Pets (rename)
             ═══════════════════════════════════════════════ -->
        <lane *case="Pets"
              orientation="vertical"
              horizontal-content-alignment="middle">

            <label *if={HasNoPets}
                   layout="stretch content"
                   margin="0, 32"
                   text={NoPetsText} />

            <scrollable *if={HasPets}
                        layout="stretch 420px"
                        peeking="64">
                <lane orientation="vertical">

                    <frame *repeat={RenamePets}
                           layout="stretch content"
                           padding="8, 10"
                           margin="0, 0, 0, 6"
                           background={@Mods/StardewUI/Sprites/MenuSlotTransparent}>
                        <lane orientation="horizontal"
                              vertical-content-alignment="middle">

                            <!-- Left: sprite + name + location -->
                            <lane orientation="horizontal"
                                  vertical-content-alignment="middle"
                                  layout="stretch content">

                                <frame layout="64px 64px"
                                       horizontal-content-alignment="middle"
                                       vertical-content-alignment="middle"
                                       margin="0, 0, 10, 0">
                                    <image layout="56px 56px"
                                           sprite={PetSprite}
                                           fit="Stretch" />
                                </frame>

                                <lane orientation="vertical"
                                      vertical-content-alignment="middle">
                                    <label text={DisplayName}
                                           bold="true" />
                                    <label text={LocationText}
                                           margin="0, 1, 0, 0" />
                                </lane>
                            </lane>

                            <!-- Right: rename button -->
                            <button click=|OnRename()|
                                    tooltip={RenameTooltip}
                                    margin="8, 0, 0, 0">
                                <label text="Rename"
                                       margin="8, 4" />
                            </button>

                        </lane>
                    </frame>

                </lane>
            </scrollable>
        </lane>

        <!-- ═══════════════════════════════════════════════
             TAB 3: Settings (toggles)
             ═══════════════════════════════════════════════ -->
        <scrollable *case="Settings"
                    layout="stretch 420px"
                    peeking="64">
            <lane *context={Settings}
                  orientation="vertical"
                  padding="8, 4">

                <!-- Work Type Toggles -->
                <lane orientation="horizontal" vertical-content-alignment="middle" margin="0, 0, 0, 8">
                    <checkbox layout="content"
                              is-checked={<>ClearDebris}
                              label-text={ClearDebrisLabel} />
                </lane>
                <lane orientation="horizontal" vertical-content-alignment="middle" margin="0, 0, 0, 8">
                    <checkbox layout="content"
                              is-checked={<>ChopTrees}
                              label-text={ChopTreesLabel} />
                </lane>
                <lane orientation="horizontal" vertical-content-alignment="middle" margin="0, 0, 0, 8">
                    <checkbox layout="content"
                              is-checked={<>ClearStumpsAndLogs}
                              label-text={ClearStumpsLabel} />
                </lane>
                <lane orientation="horizontal" vertical-content-alignment="middle" margin="0, 0, 0, 8">
                    <checkbox layout="content"
                              is-checked={<>BreakBoulders}
                              label-text={BreakBouldersLabel} />
                </lane>

                <!-- Separator -->
                <image layout="stretch 4px"
                       margin="0, 8"
                       tint="#444444"
                       sprite={@Mods/StardewUI/Sprites/White} />

                <!-- Behavior Toggles -->
                <lane orientation="horizontal" vertical-content-alignment="middle" margin="0, 0, 0, 8">
                    <checkbox layout="content"
                              is-checked={<>ForageWhileFollowing}
                              label-text={ForageFollowLabel} />
                </lane>
                <lane orientation="horizontal" vertical-content-alignment="middle" margin="0, 0, 0, 8">
                    <checkbox layout="content"
                              is-checked={<>FollowOutsideFarm}
                              label-text={FollowOutsideLabel} />
                </lane>
                <lane orientation="horizontal" vertical-content-alignment="middle" margin="0, 0, 0, 8">
                    <checkbox layout="content"
                              is-checked={<>AutoDepositToChests}
                              label-text={AutoDepositLabel} />
                </lane>

                <!-- Separator -->
                <image layout="stretch 4px"
                       margin="0, 8"
                       tint="#444444"
                       sprite={@Mods/StardewUI/Sprites/White} />

                <!-- Notification Toggles -->
                <lane orientation="horizontal" vertical-content-alignment="middle" margin="0, 0, 0, 8">
                    <checkbox layout="content"
                              is-checked={<>ShowWorkingMessages}
                              label-text={ShowWorkingMsgLabel} />
                </lane>
                <lane orientation="horizontal" vertical-content-alignment="middle" margin="0, 0, 0, 8">
                    <checkbox layout="content"
                              is-checked={<>ShowStateNotifications}
                              label-text={ShowStateNotifLabel} />
                </lane>

            </lane>
        </scrollable>

    </frame>
</lane>
