<lane orientation="vertical"
      horizontal-content-alignment="middle"
      padding="16">

    <!-- Title banner -->
    <banner background={@Mods/StardewUI/Sprites/BannerBackground}
            background-border-thickness="48,0"
            padding="12"
            text="Pet Adopter" />

    <!-- Main content frame -->
    <frame layout="480px content"
           background={@Mods/StardewUI/Sprites/MenuBackground}
           border={@Mods/StardewUI/Sprites/MenuBorder}
           border-thickness="36, 36, 40, 36"
           padding="24, 20"
           margin="0, -12, 0, 0">

        <lane orientation="vertical" horizontal-content-alignment="middle">

            <!-- Pet icon row: <-- [icon] --> -->
            <lane orientation="horizontal"
                  horizontal-content-alignment="middle"
                  vertical-content-alignment="middle"
                  margin="0, 8, 0, 4">

                <!-- Left arrow -->
                <button click=|OnPrevious()|
                        margin="0, 0, 16, 0">
                    <lane layout="40px 40px"
                          horizontal-content-alignment="middle"
                          vertical-content-alignment="middle">
                        <image layout="24px"
                               sprite={@Mods/StardewUI/Sprites/SmallLeftArrow}
                               fit="Stretch" />
                    </lane>
                </button>

                <!-- Pet portrait -->
                <frame layout="96px 96px"
                       background={@Mods/StardewUI/Sprites/MenuSlotTransparent}
                       horizontal-content-alignment="middle"
                       vertical-content-alignment="middle">
                    <image layout="80px"
                           sprite={<>PetSprite}
                           fit="Stretch" />
                </frame>

                <!-- Right arrow -->
                <button click=|OnNext()|
                        margin="16, 0, 0, 0">
                    <lane layout="40px 40px"
                          horizontal-content-alignment="middle"
                          vertical-content-alignment="middle">
                        <image layout="24px"
                               sprite={@Mods/StardewUI/Sprites/SmallRightArrow}
                               fit="Stretch" />
                    </lane>
                </button>

            </lane>

            <!-- Counter text  "2 / 6" -->
            <label layout="content"
                   margin="0, 2, 0, 6"
                   color="#666"
                   text={CounterText} />

            <!-- Pet name label -->
            <label layout="content"
                   margin="0, 0, 0, 12"
                   bold="true"
                   text={PetLabel} />

            <!-- Adopt button -->
            <button click=|OnAdopt()|
                    margin="0, 4, 0, 8"
                    *if={CanAdopt}>
                <frame layout="200px content"
                       background={@Mods/StardewUI/Sprites/ButtonDark}
                       padding="12, 8"
                       horizontal-content-alignment="middle">
                    <label text="Adopt" bold="true" color="#FFF" />
                </frame>
            </button>

            <!-- Status text -->
            <label layout="content"
                   margin="0, 4, 0, 0"
                   color="#888"
                   text={StatusText} />

        </lane>
    </frame>
</lane>
